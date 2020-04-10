//
//  Tracker.m
//  phazex
//
//  Created by toddha on 12/19/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#include "CardCollection.h"
#import "Tracker.h"

@implementation PXTracker

@synthesize player = _player;

+(id)trackerForPlayer:(PXPlayer *)player
{
	return [[[PXTracker alloc] initWithPlayer:player] autorelease];
}

-(id)initWithPlayer:(PXPlayer *)player
{
	self = [super init];
	
	if (self)
	{
		_player = [player retain];
		_cardsWanted = [[PXCardCollection alloc] init];
		_cardsNotWanted = [[PXCardCollection alloc] init];
	}
	
	return self;
}

-(void)dealloc
{
	[_player release];
	[_cardsWanted release];
	[_cardsNotWanted release];
	[super dealloc];
}

-(PXCardCollection *)cardsWanted
{
	return [[_cardsWanted copy] autorelease];
}

-(PXCardCollection *)cardsNotWanted
{
	return [[_cardsNotWanted copy] autorelease];
}

-(void)addWantedCard:(PXCard *)card
{
	[_cardsWanted addCardCopy:card];
}

-(void)addNotWantedCard:(PXCard *)card
{
	[_cardsNotWanted addCardCopy:card];
}


@end
