//
//  MainWindowController.m
//  phazex
//
//  Created by toddha on 10/20/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import "MainWindowController.h"
#import "GameState.h"
#import "CardStackView.h"
#import "PhazeRule.h"
#import "PhazeRules.h"
#import "Player.h"
#import "Card.h"

@interface MainWindowController () <CardGroupViewDelegate, CardStackViewDelegate, PXUIGameDelegate>
@end

@implementation MainWindowController

-(NSString *)description
{
	return @"MainWindowController for PhazeX";
}

-(void)dealloc
{
	[_game release];
	[super dealloc];
}

-(BOOL)applicationShouldTerminateAfterLastWindowClosed:(NSApplication *)app
{
	return TRUE;
}

- (void)awakeFromNib
{
	[NSColor setIgnoresAlpha:NO];
	
	[_discardView setHovers:NO];
	[_discardView setStacked:YES];
	[_discardView setDelegate:self];
	
	[_handView setFanEffect:YES];
	[_handView setHovers:YES];
	[_handView setStacked:NO];
	[_handView setDelegate:self];
	
	[_currentPhazeView setDelegate:self];
	//[_currentPhazeView setEnabled:NO];
	
	[self startNewGame:nil];
}




#pragma mark -
#pragma mark IBActions

- (IBAction) startNewGame:(id)sender
{
	_game = [[PXUIGame alloc] init];
	[_game setDelegate:self];
	[_game start];
}

- (IBAction)getDeckCard:(id)sender
{
	[_game getDeckCard];
}

- (IBAction)getDiscard:(id)sender
{
	[_game getDiscard];
}

#pragma mark -
#pragma mark PXUIGameDelegate

- (void)gameReadyForPlay:(PXUIGame *)game {}
- (void)game:(PXUIGame *)game updateHandEnabled:(BOOL)enabled
{
	[_handView setEnabled:enabled];
}

- (void)game:(PXUIGame *)game updateDeckEnabled:(BOOL)enabled
{
	[_deckView setEnabled:enabled];
}

- (void)game:(PXUIGame *)game updateDiscardEnabled:(BOOL)enabled
{
	[_discardView setEnabled:enabled];
}

- (void)game:(PXUIGame *)game updatePlayers:(NSArray<PXPlayer *> *)players
{
	[_playerStatusView setPlayers:players];
}

- (void)game:(PXUIGame *)game updateTableGroups:(NSArray<PXGroup *> *)groups
{
	[_tableView setGroups:groups];
}

- (void)game:(PXUIGame *)game updateDiscard:(PXCardCollection *)discard
{
	[_discardView setCards:discard];
}

- (void)game:(PXUIGame *)game updatePlayerGroups:(NSArray<PXGroup *> *)groups
{
	[_currentPhazeView setGroups:groups];
}

- (void)game:(PXUIGame *)game updateHandCards:(PXCardCollection *)cards
{
	[_handView setCards:cards];
}

- (void)gameFinished:(PXUIGame *)game
{
	// TODO
}

#pragma mark -
#pragma mark CardGroupsView delegates & helpers
-(void)clickedOnPhazeView:(CardGroupsView *)phazeView onCardGroup:(CardStackView *)cardGroup onCardView:(CardView *)cardView isMainGroup:(BOOL)isMainCardGroup movesGroups:(BOOL*)movesGroups
{
	(*movesGroups) = (cardGroup == nil) || (!isMainCardGroup);
	
	if (!isMainCardGroup)
		return;
	
	if ([_handView selectedCardView] != nil)
	{
		PXCard *card = [[_handView selectedCardView] card];
		
		BOOL groupsAlreadyContainCard = NO;
		NSArray *groups = [phazeView groups];
		for (PXGroup *group in groups)
		{
			if ([group containsCardWithId:CardId(card)])
			{
				groupsAlreadyContainCard = YES;
				break;
			}
		}
		
		if (!groupsAlreadyContainCard)
		{
			PXGroup *group = [phazeView currentGroup];
			[group addCardCopy:card];
			(*movesGroups) = NO;
			[[_handView selectedCardView] setUnavailable:YES];
			[_handView setSelectedCardView:nil];
			[_handView setNeedsDisplay:YES];
			[cardGroup setNeedsDisplay:YES];
		}
	}
	else if (cardView != nil)
	{
		if ([cardGroup selectedCardView] == cardView)
		{
			PXGroup *group = [phazeView currentGroup];
			PXCard *card = [cardView card];
			[group removeCard:card];
			[cardView setUnavailable:NO];
			(*movesGroups) = NO;
		}
		else
		{
			[cardGroup setSelectedCardView:cardView];
			(*movesGroups) = NO;
		}
	}
}

-(void)cardGroupsView:(CardGroupsView *)view mouseClickedAtPoint:(NSPoint)point clickCount:(int)clicks onCardGroup:(CardStackView *)cardGroup onCardView:(CardView *)cardView isMainCardGroup:(BOOL)isMainCardGroup movesGroups:(BOOL*)movesGroups
{
	(*movesGroups) = YES;
	if (view == _currentPhazeView)
	{
		[self clickedOnPhazeView:view onCardGroup:cardGroup onCardView:cardView isMainGroup:isMainCardGroup movesGroups:movesGroups];
	}
}


#pragma mark -
#pragma mark CardStackView delegates

-(void)cardStackView:(CardStackView *)view mouseClickedAtPoint:(NSPoint)point clickCount:(int)clicks onCardView:(CardView *)cardView
{	
	if (view == _discardView && [_game state] == gsPickingCard)
	{
		if (clicks == 2)
		{
			[_game getDiscard];
		}
	}
	else if (view == _handView)
	{
		if ([_game state] == gsPlaying)
		{
			if (clicks == 2)
			{
				if (cardView == nil) return;
				
				[_game discardCard:[cardView card]];
			}
		}
	}
}

@end
