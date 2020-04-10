//
//  Flush.m
//  phazex
//
//  Created by toddha on 12/18/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Flush.h"
#import "Card.h"

@implementation PXFlush

+(id)flushWithCardsRequired:(int)cardsRequired
{
	return [[[PXFlush alloc] initWithCardsRequired:cardsRequired] autorelease];
}

-(id)initWithCardsRequired:(int)cardsRequired
{
	self = [super initWithCardsRequired:cardsRequired];
	if (self)
	{
		_type = gtFlush;
	}
	
	return self;
}

-(BOOL)checkWithCard:(PXCard *)card byRules:(PXGameRules *)rules
{
	PXFlush *g = [[self copy] autorelease];
	[g addCardCopy:card];
	return [g check];
}

-(BOOL)guessWildCard:(PXCard *)card byRules:(PXGameRules *)rules
{
	// we don't support wild cards changing colors, but if this is configurable in the rules, go for it.
	return NO;
}

-(id)copy
{
	PXFlush *flush = [[PXFlush flushWithCardsRequired:_cardsRequired] retain];
	
	PXCard *card = _head;
	while (card != NULL)
	{
		[flush addCardCopy:card];
		card = CardNext(card);
	}
	
	return flush;
}

-(BOOL)check
{
	int count = [self count];
	if (count < _cardsRequired)
		return NO;
	
    int col = -1;
	PXCard *card = _head;
	while (card != NULL)
	{
        if (card == _head)
            col = CardColor(card);
        else if (col != CardColor(card))
            return NO;
		card = CardNext(card);
    }

    return YES;
}

+(NSArray *)findAllFlushesInHand:(PXCardCollection *)hand cardsRequired:(int)cardsRequired cardsToAdd:(int)cardsToAdd
{
	int min = kCardMinimumColor;
	int max = kCardMaximumColor;
	
	NSMutableArray *flushGroup = nil;
	
	int col;
	for (col = min; col <= max; col++)
	{
		BOOL valid = NO;
		
		int cardsNeeded = cardsRequired - [hand countCardWithColor:col notWithNumber:kCardSkipNumber];
		if (cardsNeeded <= cardsToAdd)
		{
			PXCardCollection *handCopy = [[hand copy] autorelease];
			PXFlush *flush = [PXFlush flushWithCardsRequired:cardsRequired];
			
			while ([handCopy hasCardWithColor:col notWithNumber:kCardSkipNumber])
			{
				PXCard *card = RetainCard([handCopy cardWithColor:col notWithNumber:kCardSkipNumber]);
				[handCopy removeCard:card];
				[flush addCard:card];
				ReleaseCard(card);
				valid = YES;
			}
			
			int ctr;
			for (ctr = 0; ctr < cardsNeeded; ctr++)
			{
				// TODO : check to see if rules allow for wild color, if so, look for wild cards
				PXCard *card = CreateCardWithWild(NULL, 1, col, kCardInvalidId, NO);
				[flush addCard:card];
				ReleaseCard(card);
            }

            if (valid)
            {
				if (flushGroup == nil)
				{
					flushGroup = [NSMutableArray arrayWithCapacity:(max - min + 1)];
				}
				
				[flushGroup addObject:flush];
            }
        }
    }

	NSArray *retval = nil;
	if (flushGroup != nil)
	{
		retval = [NSArray arrayWithArray:flushGroup];
	}
	
    return retval;
}

-(NSString *)description
{
	return [NSString stringWithFormat:@"Flush of %d cards : %@", _cardsRequired, [super description]];
}

@end
