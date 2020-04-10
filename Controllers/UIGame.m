//
//  UIGame.m
//  phazex
//
//  Created by toddha on 11/26/16.
//
//

#import "UIGame.h"
#import "Game.h"
#import "ComputerPlayer.h"
#import "NameFactory.h"
#import "Table.h"
#import "PhazeRules.h"
#import "CardCollection.h"

@interface PXUIGame () <PXPlayerDelegate, PXGameDelegate>
{
	int _playingCardId;
}
@end

@implementation PXUIGame

-(instancetype)init
{
	self = [super init];
	if (self)
	{
		_game = [[PXGame alloc] init];
		[_game setDelegate:self];
		_semaphore = [[NSCondition alloc] init];
		[_semaphore setName:@"PXUIGame Semaphore"];
		
		_player = [[PXPlayer playerWithGame:nil] retain];
		[_player setDelegate:self];
		[_player setName:@"Me"]; // TODO : Make full name
	}
	
	return self;
}

- (void)dealloc
{
	[_player release];
	[_semaphore release];
	[_game release];
	
	[super dealloc];
}

- (void)start
{
	[_player setGame:_game];
	for (int i = 0; i < 3; ++i) {
		[_game addPlayer:[PXComputerPlayer playerWithName:[PXNameFactory name] game:_game]];
	}
	
	[_game addPlayer:_player];
	[_game startGameInBackground];
}

- (void)getDeckCard
{
	[self updateDeckAndDiscardActions:NO];
	_action = PXUIGameAction_GetDiscard;
	[_semaphore signal];
}

- (void)getDiscard
{
	[self updateDeckAndDiscardActions:NO];
	_action = PXUIGameAction_GetDiscard;
	[_semaphore signal];
}

- (void)waitForAction
{
	[_semaphore lock];
	[_semaphore wait];
	[_semaphore unlock];
}

-(void)updateDeckAndDiscardActions:(BOOL)buttonsEnabled
{
	[[self delegate] game:self updateDeckEnabled:buttonsEnabled];
	[[self delegate] game:self updateDiscardEnabled:buttonsEnabled];
}

- (enum PXGameState)state
{
	return [_game state];
}

- (void)discardCard:(PXCard *)card
{
	_action = PXUIGameAction_DiscardCard;
	_playingCardId = CardId(card);
	[_semaphore signal];
}

#pragma mark -
#pragma mark PXGameDelegate
-(void)gameStarting:(PXGame *)game
{
	dispatch_async(dispatch_get_main_queue(), ^{
		[[self delegate] game:self updatePlayers:[game players]];
	});
}

-(void)gameFinished:(PXGame *)game winningPlayers:(NSArray<PXPlayer *> *)winningPlayers
{
	dispatch_async(dispatch_get_main_queue(), ^{
		[[self delegate] gameFinished:self];
		[[self delegate] game:self updatePlayers:[game players]];
	});
}

-(void)tableChangedInGame:(PXGame *)game
{
	dispatch_async(dispatch_get_main_queue(), ^{
		[[self delegate] game:self updatePlayers:[game players]];
		[[self delegate] game:self updateTableGroups:[[game table] groups]];
	});
}

- (void)game:(PXGame *)game discardChanged:(PXCardCollection *)discard
{
	if ([NSThread isMainThread]) {
		assert(false);
		[[self delegate] game:self updatePlayers:[game players]];
		[[self delegate] game:self updateDiscard:discard];
	} else {
		dispatch_sync(dispatch_get_main_queue(), ^{
			[[self delegate] game:self updatePlayers:[game players]];
			[[self delegate] game:self updateDiscard:discard];
		});
	}
}

- (void)game:(PXGame *)game player:(PXPlayer *)player discarded:(PXCard *)card
{
	dispatch_async(dispatch_get_main_queue(), ^{
		[[self delegate] game:self updatePlayers:[game players]];
	});
}

- (void)game:(PXGame *)game playerSkipped:(PXPlayer *)player
{
	dispatch_async(dispatch_get_main_queue(), ^{
		[[self delegate] game:self updatePlayers:[game players]];
	});
}

- (void)game:(PXGame *)game playerPickedDeckCard:(PXPlayer *)player
{
	dispatch_async(dispatch_get_main_queue(), ^{
		[[self delegate] game:self updatePlayers:[game players]];
	});
}

- (void)turnStartedInGame:(PXGame *)game
{
	dispatch_async(dispatch_get_main_queue(), ^{
		[[self delegate] game:self updatePlayers:[game players]];
	});
}

- (void)turnEndedInGame:(PXGame *)game
{
	dispatch_async(dispatch_get_main_queue(), ^{
		[[self delegate] game:self updatePlayers:[game players]];
	});
}

- (void)handStartedInGame:(PXGame *)game
{
	dispatch_async(dispatch_get_main_queue(), ^{
		PXPhazeRule *rule = [[_game phazeRules] phazeRuleNumber:[_player phazeRuleNumber]];
		[[self delegate] game:self updatePlayerGroups:[rule groups]];
		[[self delegate] game:self updatePlayers:[game players]];
	});
}
-(void)handEndedInGame:(PXGame *)game {}


#pragma mark -
#pragma mark PXPlayerDelegate

- (void)player:(PXPlayer *)player playerStartingTurn:(PXPlayer *)playerStartingTurn {} // this should be on the game!
-(void)playerPickCard:(PXPlayer *)player
{
	dispatch_async(dispatch_get_main_queue(), ^{
		[self updateDeckAndDiscardActions:YES];
		[[self delegate] game:self updateHandEnabled:NO];
		[[self delegate] gameReadyForPlay:self];
	});
	
	[self waitForAction];
	
	switch (_action)
	{
		case PXUIGameAction_GetDiscard:
			[_game discardForPlayer:_player];
			break;
		case PXUIGameAction_GetDeckCard:
			[_game deckCardForPlayer:_player];
			break;
		case PXUIGameAction_None:
		case PXUIGameAction_DiscardCard:
			assert(false);
			break;
	}
	
	_action = PXUIGameAction_None;
}

- (void)player:(PXPlayer *)player handChanged:(PXCardCollection *)hand
{
	if ([NSThread isMainThread])
	{
		[[self delegate] game:self updateHandCards:hand];
	}
	else
	{
		dispatch_sync(dispatch_get_main_queue(), ^{
			[[self delegate] game:self updateHandCards:hand];
		});
	}
}

#pragma mark -
#pragma mark Player delegates and helper functions

-(void)playerShouldPlay:(PXPlayer *)player
{
	dispatch_async(dispatch_get_main_queue(), ^{
		[[self delegate] game:self updateHandEnabled:YES];
		[[self delegate] gameReadyForPlay:self];
	});
	
	[self waitForAction];
	
	switch (_action)
	{
		case PXUIGameAction_DiscardCard:
		{
			// TODO : Skip choice should live in UI.
			PXCard *card = [[_player hand] cardWithId:_playingCardId];
			assert(card != NULL);
			if (CardIsSkip(card))
			{
				[_game discard:card skip:[_game playerAfterPlayer:_player] forPlayer:_player];
			}
			else
			{
				[_game discard:card forPlayer:_player];
			}
			
			break;
		}
		
		case PXUIGameAction_GetDiscard:
		case PXUIGameAction_GetDeckCard:
		case PXUIGameAction_None:
			assert(false);
			break;
	}
}

- (void)player:(PXPlayer *)player playerGotDeckCard:(PXPlayer *)playingPlayer
{
	dispatch_async(dispatch_get_main_queue(), ^{
		[self updateDeckAndDiscardActions:NO];
	});
}

- (void)game:(PXGame *)game player:(PXPlayer *)player gotDiscard:(PXCard *)card
{
	dispatch_async(dispatch_get_main_queue(), ^{
		[self updateDeckAndDiscardActions:NO];
	});
}



@end
