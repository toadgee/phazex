//
//  PlayerStatusView_mac.h
//  phazex
//
//  Created by toddha on 12/12/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Cocoa/Cocoa.h>
@class PXPlayer;
@interface PlayerStatusView : NSView
- (void)setPlayers:(NSArray<PXPlayer *> *)players;
@end
