//
//  CardTracker.m
//  phazex
//
//  Created by toddha on 12/19/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Card.h"
#import "CardCollection.h"
#import "CardTracker.h"
#import "Tracker.h"

@implementation PXCardTracker

-(id)init
{
	self = [super init];
	
	if (self)
	{
		_trackers = [[NSMutableArray arrayWithCapacity:8] retain];
	}
	
	return self;
}

-(void)dealloc
{
	[_trackers release];
	[super dealloc];
}

-(void)clear
{
	[_trackers removeAllObjects];
}

-(void)player:(PXPlayer *)player wantedCard:(PXCard *)card
{	
	for (PXTracker *tracker in _trackers)
	{
		if ([tracker player] == player)
		{
			[tracker addWantedCard:card];
			return;
		}
	}
	
	PXTracker *tracker = [PXTracker trackerForPlayer:player];
	[tracker addWantedCard:card];
	[_trackers addObject:tracker];
}

-(void)player:(PXPlayer *)player didNotWantCard:(PXCard *)card
{
	for (PXTracker *tracker in _trackers)
	{
		if ([tracker player] == player)
		{
			[tracker addNotWantedCard:card];
			return;
		}
	}
	
	PXTracker *tracker = [PXTracker trackerForPlayer:player];
	[tracker addNotWantedCard:card];
	[_trackers addObject:tracker];
}

-(int)player:(PXPlayer *)player percentageWantedCardsWithNumber:(int)number
{
	int wanted = 0;
    int total = 0;

	for (PXTracker *tracker in _trackers)
    {
        if ([tracker player] == player)
        {
			for (int i = 0; i < [[tracker cardsWanted] count]; ++i)
			{
				PXCard *card = [[tracker cardsWanted] cardAtIndex:i];
				if (CardNumber(card) == number)
				{
					wanted++;
				}
			}
            
            total = wanted;
            for (int i = 0; i < [[tracker cardsNotWanted] count]; ++i)
			{
				PXCard *card = [[tracker cardsNotWanted] cardAtIndex:i];
				if (CardNumber(card) == number)
				{
					total++;
				}
			}

            break;
        }
    }

    if (total == 0)
		return 0;

    return wanted * 100 / total;
}

-(int)player:(PXPlayer *)player percentageWantedCardsWithColor:(int)color
{
	int wanted = 0;
    int total = 0;

	for (PXTracker *tracker in _trackers)
    {
        if ([tracker player] == player)
        {
			for (int i = 0; i < [[tracker cardsWanted] count]; ++i)
			{
				PXCard *card = [[tracker cardsWanted] cardAtIndex:i];
				if (CardColor(card) == color)
				{
					wanted++;
				}
			}
            
            total = wanted;
            for (int i = 0; i < [[tracker cardsNotWanted] count]; ++i)
			{
				PXCard *card = [[tracker cardsNotWanted] cardAtIndex:i];
				if (CardColor(card) == color)
				{
					total++;
				}
			}

            break;
        }
    }

    if (total == 0)
		return 0;

    return wanted * 100 / total;
}

-(BOOL)hasPlayer:(PXPlayer *)player seenAnyCardsWithNumber:(int)number
{
	for (PXTracker *tracker in _trackers)
    {
        if ([tracker player] == player)
        {
			for (int i = 0; i < [[tracker cardsWanted] count]; ++i)
			{
				PXCard *card = [[tracker cardsWanted] cardAtIndex:i];
				if (CardNumber(card) == number)
				{
					return YES;
				}
			}
            
            for (int i = 0; i < [[tracker cardsNotWanted] count]; ++i)
			{
				PXCard *card = [[tracker cardsNotWanted] cardAtIndex:i];
				if (CardNumber(card) == number)
				{
					return YES;
				}
			}

            break;
        }
    }

    return NO;
}

-(BOOL)hasPlayer:(PXPlayer *)player seenAnyCardsWithColor:(int)color
{
	for (PXTracker *tracker in _trackers)
    {
        if ([tracker player] == player)
        {
			for (int i = 0; i < [[tracker cardsWanted] count]; ++i)
			{
				PXCard *card = [[tracker cardsWanted] cardAtIndex:i];
				if (CardColor(card) == color)
				{
					return YES;
				}
			}
            
            for (int i = 0; i < [[tracker cardsNotWanted] count]; ++i)
			{
				PXCard *card = [[tracker cardsNotWanted] cardAtIndex:i];
				if (CardColor(card) == color)
				{
					return YES;
				}
			}

            break;
        }
    }

    return NO;
}


@end
