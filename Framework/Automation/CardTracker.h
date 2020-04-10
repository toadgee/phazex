//
//  CardTracker.h
//  phazex
//
//  Created by toddha on 12/19/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#include "Card.h"
@class PXPlayer;
@class PXTracker;

@interface PXCardTracker : NSObject
{
	NSMutableArray<PXTracker *> *_trackers;
}

-(id)init;
-(void)clear;

-(void)player:(PXPlayer *)player wantedCard:(PXCard *)card;
-(void)player:(PXPlayer *)player didNotWantCard:(PXCard *)card;

-(int)player:(PXPlayer *)player percentageWantedCardsWithNumber:(int)number;
-(int)player:(PXPlayer *)player percentageWantedCardsWithColor:(int)color;

-(BOOL)hasPlayer:(PXPlayer *)player seenAnyCardsWithNumber:(int)number;
-(BOOL)hasPlayer:(PXPlayer *)player seenAnyCardsWithColor:(int)color;

@end
