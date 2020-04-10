#import <Foundation/Foundation.h>

#import "HiResTimer.h"
#import "Game.h"
#import "Logging.h"
#import "CardCollection.h"
#import "ComputerPlayer.h"

@interface GameTestHarness : NSObject < PXGameDelegate >
{
	PXHiResTimer *_gameTimer;
	PXHiResTimer *_handTimer;
	PXHiResTimer *_turnTimer;
	PXGame *_game;
}

- (void)start;
@end

@implementation GameTestHarness

- (void)dealloc
{
	[_handTimer release];
	[_turnTimer release];
	[_gameTimer release];
	[_game release];
	[super dealloc];
}

- (void)start
{
	int games = 100; // good to flush out bugs in algorithms
	int status = 10;
	int players = 4;
	
#if DEBUG
	games = 100; // good enough for perf?
	status = 1;
#endif

	// TODO : Spin off in background operations
	_gameTimer = [[PXHiResTimer timer] retain];
	_handTimer = [[PXHiResTimer timer] retain];
	_turnTimer = [[PXHiResTimer timer] retain];
	
	printf("Running %d games\n", games);
	for (int i = 1; i <= games; i++)
	{
		uint64_t elapsed = 0;
		uint64_t cardsLivingBegin = CardsLiving();
		
		@autoreleasepool
		{
			if (i % status == 0)
				printf("Starting game %d\n", i);
			
			_game = [[PXGame game] retain];
			for (int playerNum = 0; playerNum < players; ++playerNum)
			{
				[_game addPlayer:[PXComputerPlayer playerWithName:[NSString stringWithFormat:@"Player-%d", playerNum + 1] game:_game]];
			}
			
			_game.delegate = self;
			[_gameTimer start];
			[_game startGame];
			elapsed = [_gameTimer stop];
			_game.delegate = nil;
			[_game release];
			_game = nil;
		}
		
		uint64_t cardsLivingEnd = CardsLiving();
		if (cardsLivingBegin != cardsLivingEnd)
		{
			NSLog(@"%llu != %llu", cardsLivingBegin, cardsLivingEnd);
			PrintAllUsedCards();
		}
		
#pragma unused(elapsed)
		//printf("Game %d took %llu milliseconds\n", i, elapsed / 1000000);
	}
	
	printf("TOTALS\n");
	printf("Took %llu milliseconds per game, %llu games\n", [_gameTimer totalMilliseconds] / [_gameTimer starts], [_gameTimer starts]);
	printf("Took %llu milliseconds per turn, %llu turns\n", [_turnTimer totalMilliseconds] / [_turnTimer starts], [_turnTimer starts]);
	printf("Took %llu milliseconds per hand, %llu hands\n", [_handTimer totalMilliseconds] / [_handTimer starts], [_handTimer starts]);
	
	return;
}

- (void)game:(PXGame *)game discardChanged:(PXCardCollection *)discard {}
- (void)game:(PXGame *)game playerSkipped:(PXPlayer *)player {}
- (void)game:(PXGame *)game playerPickedDeckCard:(PXPlayer *)player {}
- (void)game:(PXGame *)game player:(PXPlayer *)player discarded:(PXCard *)card {}
- (void)game:(PXGame *)game player:(PXPlayer *)player gotDiscard:(struct PXCardStruct *)card {}
- (void)tableChangedInGame:(PXGame *)game {}
- (void)gameStarting:(PXGame *)game {}

- (void)turnStartedInGame:(PXGame *)game
{
	if (game != _game)
		[PXLogging raiseError:@"Game doesn't match expected!"];
		
	if ([_turnTimer started])
		[PXLogging raiseError:@"Turn started before ending!"];
	
	[_turnTimer start];
}

- (void)turnEndedInGame:(PXGame *)game
{
	if (game != _game)
		[PXLogging raiseError:@"Game doesn't match expected!"];
	
	if (![_turnTimer started])
		[PXLogging raiseError:@"Turn ended before started!"];
	
	[_turnTimer stop];
}

- (void)handStartedInGame:(PXGame *)game
{
	if (game != _game)
		[PXLogging raiseError:@"Game doesn't match expected!"];
		
	if ([_handTimer started])
		[PXLogging raiseError:@"Hand started before ending!"];
	
	[_handTimer start];
}

- (void)handEndedInGame:(PXGame *)game
{
	if (game != _game)
		[PXLogging raiseError:@"Game doesn't match expected!"];
	
	if (![_handTimer started])
		[PXLogging raiseError:@"Hand ended before started!"];
	
	[_handTimer stop];
}

- (void)gameFinished:(PXGame *)game winningPlayers:(NSArray *)winningPlayers
{
	if (game != _game)
		[PXLogging raiseError:@"Game doesn't match expected!"];
		
	if ([winningPlayers count] == 0)
		[PXLogging raiseError:@"Winning players is empty!!"];
	
#if PRINT_GAME_OUTPUT
	NSLog(@"%lu winning players", (unsigned long)[winningPlayers count]);
	for (id player in winningPlayers)
		NSLog(@"%@", player);
#endif
}

@end


int main (int argc, const char * argv[])
{
#if DEBUG
	srandom(105);
#else
	//srandom((unsigned int)time(0));
	srandom(105);
#endif
	@autoreleasepool
	{
		[PXComputerPlayer setSleepTime:0.0];
		GameTestHarness *harness = [[GameTestHarness new] autorelease];
		[harness start];
	}
	
    return 0;
}
