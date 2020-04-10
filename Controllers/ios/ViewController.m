//
//  ViewController.m
//  PhazeXMobile
//
//  Created by toddha on 11/8/16.
//
//

#import "ViewController.h"
#import "CardView.h"
#import "DeckView.h"
#import "HandView.h"
#import "CardCollection.h"
#import "CardGroupsView.h"
#import "Game.h"
#import "Player.h"
#import "PlayerStatusView_ios.h"
#import "ComputerPlayer.h"
#import "UIGame.h"

@interface ViewController () <PXUIGameDelegate, PXCardViewDelegate, PXDeckViewDelegate>
{
	PXUIGame *_game;
}

@property (readwrite, assign, nonatomic) IBOutlet PXCardGroupsView *tableView;
@property (readwrite, assign, nonatomic) IBOutlet PXCardView *discardView;
@property (readwrite, assign, nonatomic) IBOutlet PXDeckView *deckView;
@property (readwrite, assign, nonatomic) IBOutlet PXHandView *handView;
@property (readwrite, assign, nonatomic) IBOutlet PXPlayerStatusView *playerStatusView;
@end

@implementation ViewController

- (void)viewDidLoad {
	[super viewDidLoad];
	[_deckView setImage:[UIImage imageNamed:@"phazex_deck.jpg"]];
	[_deckView setDelegate:self];
	[_discardView setDelegate:self];
	[_discardView setUserInteractionEnabled:YES];
}

- (void)viewDidAppear:(BOOL)animated
{
	[super viewDidAppear:animated];
	//[PXComputerPlayer setSleepTime:0.0];
	[self startNewGame];
}

- (void)startNewGame
{
	[[_game game] waitForGameToFinish];
	[_game release];
	_game = [[PXUIGame alloc] init];
	[_game setDelegate:self];
	[_game start];
}

- (void)gameFinished:(PXUIGame *)game
{
	[self startNewGame];
}

- (void)gameReadyForPlay:(PXUIGame *)game
{
}

- (void)game:(PXUIGame *)game updateHandEnabled:(BOOL)enabled
{
	// we handle this internally - maybe we should draw it?
}

- (void)game:(PXUIGame *)game updateDiscardEnabled:(BOOL)enabled
{
	// we handle this internally - maybe we should draw it?
}

- (void)game:(PXUIGame *)game updateDeckEnabled:(BOOL)enabled
{
	// we handle this internally - maybe we should draw it?
}

- (void)game:(PXUIGame *)game updatePlayers:(NSArray<PXPlayer *> *)players
{
	[[self playerStatusView] setPlayers:players];
}

- (void)game:(PXUIGame *)game updateTableGroups:(NSArray<PXGroup *> *)groups
{
	[_tableView setGroups:groups];
}

- (void)game:(PXUIGame *)game updateDiscard:(PXCardCollection *)discard
{
	[[self discardView] setCard:[discard lastCard]];
}

- (void)game:(PXUIGame *)game updatePlayerGroups:(NSArray<PXGroup *> *)groups
{
	// TODO
}

- (void)game:(PXUIGame *)game updateHandCards:(PXCardCollection *)cards
{
	[[self handView] setCardCollection:cards];
}

- (void)cardView:(PXCardView *)cardView beganTouches:(NSSet<UITouch *> *)touches {}
- (void)cardView:(PXCardView *)cardView movedTouches:(NSSet<UITouch *> *)touches {}
- (void)cardView:(PXCardView *)cardView endedTouches:(NSSet<UITouch *> *)touches
{
	if (cardView == [self discardView])
	{
		if ([_game state] == gsPickingCard)
		{
			[_game getDiscard];
		}
		else if ([_game state] == gsPlaying)
		{
			PXCard *cardToDiscard = [_handView selectedCard];
			if (cardToDiscard)
			{
				[_game discardCard:cardToDiscard];
			}
		}
	}
}

- (void)deckView:(PXDeckView *)deckView beganTouches:(NSSet<UITouch *> *)touches
{
	if (deckView == [self deckView])
	{
		if ([_game state] == gsPickingCard)
		{
			[_game getDeckCard];
		}
	}
}

- (void)deckView:(PXDeckView *)deckView movedTouches:(NSSet<UITouch *> *)touches {}
- (void)deckView:(PXDeckView *)deckView endedTouches:(NSSet<UITouch *> *)touches {}


@end
