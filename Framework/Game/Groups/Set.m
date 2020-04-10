//
//  Set.m
//  phazex
//
//  Created by toddha on 12/18/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Logging.h"
#import "Card.h"
#import "Set.h"

@implementation PXSet

+(id)setWithCardsRequired:(int)cardsRequired
{
	return [[[PXSet alloc] initWithCardsRequired:cardsRequired] autorelease];
}

-(id)initWithCardsRequired:(int)cardsRequired
{
	self = [super initWithCardsRequired:cardsRequired];
	
	if (self)
	{
		_type = gtSet;
	}
	
	return self;
}

-(BOOL)checkWithCard:(PXCard *)card byRules:(PXGameRules *)rules
{
	//NSLog(@"Checking set by card");
	PXSet *s = [[self copy] autorelease];
	[s addCardCopy:card];
	return [s check];
}

-(id)copy
{
	PXSet *set = [[PXSet setWithCardsRequired:_cardsRequired] retain];
	
	PXCard *c = _head;
	while (c != NULL)
	{
		[set addCardCopy:c];
		c = CardNext(c);
	}
	
	return set;
}

-(BOOL)check
{
	int count = [self count];
	if (count < _cardsRequired)
	{
		return NO;
	}
	
	int i = kCardMinimumNumberValue - 1;
	PXCard *card = _head;
	while (card != NULL)
	{
		if (!CardIsNumbered(card))
		{
			return NO;
		}
		
		if (card == _head)
		{
			i = CardNumber(card);
		}
		else if (i != CardNumber(card))
		{
			return NO;
		}
		
		card = CardNext(card);
	}
	
	return YES;
}

-(BOOL)guessWildCard:(PXCard *)card byRules:(PXGameRules *)rules
{
	if (_head == NULL)
	{
		[PXLogging raiseError:@"Tried to guess wild card for a set with no cards!"];
		return NO;
	}
	
	int guess = kCardWildNumber;
	PXCard *c = _head;
	while (c != NULL)
	{
		if (CardIsUnassignedWild(c))
		{
			c = CardNext(c);
			continue;
		}
		
		if (guess == kCardWildNumber)
		{
			guess = CardNumber(c);
		}
		else if (guess != CardNumber(c))
		{
			guess = kCardWildNumber;
			break;
		}
		
		c = CardNext(c);
	}
	
	if (guess == kCardWildNumber)
	{
		return NO;
	}
	
	CardSetNumber(card, guess);
	return YES;
}


+(NSArray *)findAllSetsInHand:(PXCardCollection *)hand cardsRequired:(int)cardsRequired cardsToAdd:(int)cardsToAdd
{
	int min = kCardMinimumNumber;
	int max = kCardMaximumNumber;
	
	NSMutableArray *sets = nil;
	
	//NSLog(@"Finding all sets in hand");
	// start with each number from min to max
	int num;
	for (num = min; num <= max; num++)
	{
		BOOL valid = NO;
		int cardsNeeded = cardsRequired - [hand countCardWithNumber:num] - [hand countCardWithNumber:kCardWildNumber];
		if (cardsNeeded <= cardsToAdd)
		{
			PXCardCollection *handCopy = [[hand copy] autorelease];
			PXSet *set = [PXSet setWithCardsRequired:cardsRequired];
			
			while ([handCopy hasCardWithNumber:num])
			{
				PXCard *card = RetainCard([handCopy cardWithNumber:num]);
				[handCopy removeCard:card];
				[set addCard:card];
				ReleaseCard(card);
				valid = YES;
			}
			
			if (valid)
			{
				int ctr;
				for (ctr = [set count]; ctr < cardsRequired; ctr++)
				{
					// use wild cards first
					if ([handCopy hasCardWithNumber:kCardWildNumber])
					{
						PXCard *wild = RetainCard([handCopy cardWithNumber:kCardWildNumber]);
						[handCopy removeCard:wild];
						CardSetNumber(wild, num);
						[set addCard:wild];
						ReleaseCard(wild);
					}
					else
					{
						PXCard *card = CreateCardWithWild(NULL, num, kCardMinimumColor, kCardInvalidId, NO);
						[set addCard:card];
						ReleaseCard(card);
					}
				}
				
				if (sets == nil)
				{
					sets = [NSMutableArray arrayWithCapacity:(max - min + 1)];
				}
				
				[sets addObject:set];
			}
		}
	}
	
	return sets;
}

-(NSString *)description
{
	return [NSString stringWithFormat:@"Set of %d cards : %@", _cardsRequired, [super description]];
}

@end
