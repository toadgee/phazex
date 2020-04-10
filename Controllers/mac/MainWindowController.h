//
//  MainWindowController.h
//  phazex
//
//  Created by toddha on 10/20/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Cocoa/Cocoa.h>

#import "UIGame.h"
#import "CardGroupsView.h"
#import "CardStackView.h"
#import "Player.h"
#import "PlayerStatusView_mac.h"

const int conditionUnused = -1;
const int conditionPickedCard = 0;
const int conditionDiscarded = 1;

@interface MainWindowController : NSWindowController {

	IBOutlet CardStackView *_discardView;
	IBOutlet CardStackView *_handView;
	IBOutlet CardGroupsView *_tableView;
	IBOutlet CardGroupsView *_currentPhazeView;
	IBOutlet NSButton *_deckView;
	
	
	// hacks - change to playerstatuslistview or whatever
	IBOutlet PlayerStatusView *_playerStatusView;

	PXUIGame *_game;
}

- (IBAction)startNewGame:(id)sender;
- (IBAction)getDeckCard:(id)sender;
- (IBAction)getDiscard:(id)sender;

@end
