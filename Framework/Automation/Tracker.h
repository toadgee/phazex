//
//  Tracker.h
//  phazex
//
//  Created by toddha on 12/19/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#include "Card.h"
@class PXCardCollection;
@class PXPlayer;

@interface PXTracker : NSObject
{
	PXPlayer *_player;
	PXCardCollection *_cardsWanted;
	PXCardCollection *_cardsNotWanted;
}

@property (readonly) PXPlayer *player;

+(id)trackerForPlayer:(PXPlayer *)player;
-(id)initWithPlayer:(PXPlayer *)player;

-(PXCardCollection *)cardsWanted;
-(PXCardCollection *)cardsNotWanted;

-(void)addWantedCard:(PXCard *)card;
-(void)addNotWantedCard:(PXCard *)card;

@end
