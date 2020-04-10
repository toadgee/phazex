//
//  Run.m
//  phazex
//
//  Created by toddha on 12/18/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Run.h"
#import "Logging.h"
#import "Card.h"

@implementation PXRun

+(id)runWithCardsRequired:(int)cardsRequired
{
	return [[[PXRun alloc] initWithCardsRequired:cardsRequired] autorelease];
}

-(id)initWithCardsRequired:(int)cardsRequired
{
	self = [super initWithCardsRequired:cardsRequired];
	
	if (self)
	{
		_type = gtRun;
	}
	
	return self;
}

-(id)copy
{
	PXRun *run = [[PXRun runWithCardsRequired:_cardsRequired] retain];
	
	PXCard *c = _head;
	while (c != NULL)
	{
		[run addCardCopy:c];
		c = CardNext(c);
	}
	
	return run;
}

-(BOOL)checkWithCard:(PXCard *)card byRules:(PXGameRules *)rules
{
	PXRun *r = [[self copy] autorelease];
	[r addCardCopy:card];
	return [r check];
}

-(BOOL)check
{
	int count = [self count];
	if (count == 0)
		return NO;

	if (count < _cardsRequired)
		return NO;
	
	int lowest = kCardMaximumNumber + 1;
	int highest = kCardMinimumNumber - 1;

	PXCard *card = _head;
	while (card != NULL)
	{
		if (!CardIsUnassignedWild(card) && !CardIsNumbered(card))
			return NO;

		if (CardNumber(card) < lowest)
			lowest = CardNumber(card);

		if (CardNumber(card) > highest)
			highest = CardNumber(card);
		
		card = CardNext(card);
	}

	if ((highest - lowest + 1) != count)
		return NO;

	int num;
	for (num = lowest; num <= highest; num++)
	{
		BOOL found = NO;
		PXCard *card = _head;
		while (card != NULL)
		{
			if (CardNumber(card) == num)
			{
				found = YES;
				break;
			}
			
			card = CardNext(card);
		}

		if (!found)
		{
			return NO;
		}
	}

	return YES;
}

-(BOOL)guessWildCard:(PXCard *)card byRules:(PXGameRules *)rules
{
	int count = [self count];
	if (count == 0)
	{
		[PXLogging raiseError:@"Tried to guess wild card for a run with no cards!"];
		return NO;
	}

	int guess = kCardWildNumber;
	
	// Find min and max cards
	int min = kCardMaximumNumber + 1;
	int max = kCardMinimumNumber - 1;
	PXCard *c = _head;
	while (c != NULL)
	{
		if (CardNumber(c) < min)
			min = CardNumber(c);
		if (CardNumber(c) > max)
			max = CardNumber(c);
		c = CardNext(c);
	}
	
	// Find the first hole in the cards and fill it
	PXCardCollection *cc = [[self copy] autorelease];
	[cc sortByNumber];
	int last = -1;
	c = [cc cardAtIndex:0];
	while (c != NULL)
	{
		if (last == -1)
			last = CardNumber(c);
		else if (CardNumber(c) > (last + 1))
		{
			guess = last + 1;
			break;
		}
		else
			last++;
		c = CardNext(c);
	}
	
	// If we didn't find a hole, add a card to the end or the beginning of the run
	if (guess == kCardWildNumber)
	{
		if ([cc maximumCardNumber] < kCardMaximumNumber)
		{
			guess = [cc maximumCardNumber] + 1;
		}
		else if ([cc minimumCardNumber] > kCardMinimumNumber)
		{
			guess = [cc minimumCardNumber] - 1;
		}
	}

	if (guess == kCardWildNumber)
		return NO;

	CardSetNumber(card, guess);
	return YES;
}

+(NSArray *)findAllRunsInHand:(PXCardCollection *)hand cardsRequired:(int)cardsRequired cardsToAdd:(int)cardsToAdd
{
	int min = kCardMinimumNumber;
	int max = kCardMaximumNumber;
	
	NSMutableArray *runs = nil;

	// start with each number from min to max
	int num;
	for (num = min; num <= max; num++)
	{
		// easy out
		if (max - num + 1 < cardsRequired)
			{
			// stop -- can't go any further
			break; 
		}

		int cardsAdded = 0;
		PXCardCollection *handCopy = [[hand copy] autorelease];
		PXRun *run = [PXRun runWithCardsRequired:cardsRequired];

		int cardsLeft;
		for (cardsLeft = cardsRequired; cardsLeft > 0; cardsLeft--)
		{
			if ([handCopy hasCardWithNumber:(num + cardsRequired - cardsLeft)])
			{
				PXCard *card = RetainCard([handCopy cardWithNumber:(num + cardsRequired - cardsLeft)]);
				[handCopy removeCard:card];
				[run addCard:card];
				ReleaseCard(card);
			}
			else if ([handCopy hasCardWithNumber:(kCardWildNumber)])
			{
				// check for wilds
				PXCard *card = RetainCard([handCopy cardWithNumber:kCardWildNumber]);
				[handCopy removeCard:card];
				CardSetNumber(card, (num + cardsRequired - cardsLeft));
				[run addCard:card];
				ReleaseCard(card);
			}
			else
			{
				cardsAdded++;
				if (cardsAdded > cardsToAdd)
				{
					run = nil;
					break;
				}

				PXCard *card = CreateCard(NULL, (num + cardsRequired - cardsLeft), kCardMinimumColor, kCardInvalidId);
				[run addCard:card];
				ReleaseCard(card);
			}
		}

		if (run != nil)
		{
			if ([run check])
			{
				if (runs == nil)
				{
					runs = [NSMutableArray arrayWithCapacity:(max - min + 1)];
				}
				
				[runs addObject:run];
			}
		}
	}

	NSArray *retval = nil;
	if (runs != nil)
	{
		retval = [NSArray arrayWithArray:runs];
	}
	
	return retval;
}

-(NSString *)description
{
	return [NSString stringWithFormat:@"Run of %d cards : %@", _cardsRequired, [super description]];
}

@end
