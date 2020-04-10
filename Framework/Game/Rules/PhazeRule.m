//
//  PhazeRule.m
//  phazex
//
//  Created by toddha on 12/18/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Run.h"
#import "Set.h"
#import "Flush.h"
#import "PhazeRule.h"
#import "GroupType.h"
#import "Card.h"
#import "GameRules.h"

@implementation PXPhazeRule

@synthesize ruleNumber = _ruleNumber;
@synthesize groups = _groups;

+(int)minimumCardsNeededToCompletePhaze:(PXPhazeRule *)rule withHand:(PXCardCollection *)hand byAddingCard:(PXCard *)card byRules:(PXGameRules *)rules
{
	PXCardCollection *handCopy = [[hand copy] autorelease];
	[handCopy addCardCopy:card];
	return [PXPhazeRule minimumCardsNeededToCompletePhaze:rule withHand:handCopy byRules:rules];
}

+(int)minimumCardsNeededToCompletePhaze:(PXPhazeRule *)rule withHand:(PXCardCollection *)hand byRules:(PXGameRules *)rules
{
    // if we can complete the phaze without adding any cards, return 0
	if ([PXPhazeRule completePhaze:rule withHand:hand byRules:rules])
	{
		return 0;
	}

    // the number of cards required
    // subtract [rule count] as we know that we can always have one
    // card in a phaze without it being invalid
    int cardsRequired = [rule cardsRequired] - [rule count];

    // try to complete the phaze with the maximum number of cards required (-1)
	NSArray *llg = [PXPhazeRule completePhaze:rule withHand:hand cardsToAdd:cardsRequired byRules:rules];
    if (llg == nil)
    {
        return [rule cardsRequired];
    }

    // start with minimum as cards required
    int minimum = cardsRequired;
    int current;

	for (NSArray *lg in llg)
    {
        current = 0;
        
        // count number of cards that have -1 as id
		for (PXGroup *group in lg)
        {
			for (int i = 0; i < [group count]; ++i)
			{
				PXCard *card = [group cardAtIndex:i];
                if (CardIsInvalidId(card))
                {
                    current++;
                }
            }
        }

        // if the number of cards we added (have -1 as id) is less than
        // minimum, current is our new best
        if (current < minimum)
        {
            minimum = current;
        }
    }

    return minimum;
}

+(NSArray *)completePhaze:(PXPhazeRule *)rule withHand:(PXCardCollection *)hand byRules:(PXGameRules *)rules
{
	return [PXPhazeRule completePhaze:rule withHand:hand cardsToAdd:0 byRules:rules startingAtGroup:0];
}

+(NSArray *)completePhaze:(PXPhazeRule *)rule withHand:(PXCardCollection *)hand cardsToAdd:(int)cardsToAdd byRules:(PXGameRules *)rules
{
	return [PXPhazeRule completePhaze:rule withHand:hand cardsToAdd:cardsToAdd byRules:rules startingAtGroup:0];
}

+(NSArray *)completePhaze:(PXPhazeRule *)rule withHand:(PXCardCollection *)hand cardsToAdd:(int)cardsToAdd byRules:(PXGameRules *)rules startingAtGroup:(int)groupPosition
{
	NSArray *lg = nil;
    PXGroup *group = [rule groupAt:groupPosition];

    // make all possible combinations
    switch ([group type])
    {
        case gtRun:
			lg = [PXRun findAllRunsInHand:hand cardsRequired:[group cardsRequired] cardsToAdd:cardsToAdd];
            break;
        
		case gtSet:
			lg = [PXSet findAllSetsInHand:hand cardsRequired:[group cardsRequired] cardsToAdd:cardsToAdd];
            break;

		case gtFlush:
			lg = [PXFlush findAllFlushesInHand:hand cardsRequired:[group cardsRequired] cardsToAdd:cardsToAdd];
            break;
    }
    
    if ([lg count] == 0)
    {
        return nil;
    }

	NSMutableArray *retval = [NSMutableArray arrayWithCapacity:[lg count]];
    if (groupPosition == ([rule count] - 1))
    {
        // end of calling chain
		for (PXGroup *gr in lg)
        {
			[retval addObject:[NSArray arrayWithObject:gr]];
        }
    }
    else
    {
		//NSLog(@"Completing phaze");
		for (PXGroup *gr in lg)
        {
            // create hand copy
            PXCardCollection *handCopy = [[hand copy] autorelease];
            int cardsLeftToAdd = cardsToAdd;

            // figure out how many cards out of the CardsToAdd we've used up
            // remove cards from hand that we've already used
			for (int i = 0; i < [gr count]; ++i)
			{
				PXCard *card = [gr cardAtIndex:i];
                if (CardIsInvalidId(card))
                {
                    cardsLeftToAdd--;
                }
                else
                {
					[handCopy removeCardWithId:CardId(card)];
                }
            }

            // now do the next list
			NSArray *llgNext = [PXPhazeRule completePhaze:rule withHand:handCopy cardsToAdd:cardsLeftToAdd byRules:rules startingAtGroup:(groupPosition + 1)];
            if (llgNext == nil)
            {
                continue;
            }

			for (NSArray *lgNext in llgNext)
            {   
                // check that none use the same id
                BOOL invalid = NO;
				for (int i = 0; i < [gr count]; ++i)
				{
					PXCard *card = [gr cardAtIndex:i];
					for (PXGroup *groupNext in lgNext)
                    {
						for (int j = 0; j < [groupNext count]; ++j)
						{
							PXCard *cardNext = [groupNext cardAtIndex:j];
                            if ((CardId(card) == CardId(cardNext)) && (!CardIsInvalidId(card)))
                            {
                                invalid = YES;
                                break;
                            }
                        }

                        if (invalid)
                        {
                            break;
                        }
                    }

                    if (invalid)
                    {
                        break;
                    }
                }

                if (invalid)
                {
                    continue;
                }

				NSMutableArray *lgNew = [NSMutableArray arrayWithCapacity:([lgNext count] + 1)];
				[lgNew addObjectsFromArray:lgNext];
				[lgNew insertObject:gr atIndex:0];
				[retval addObject:lgNew];
            }
        }
    }

    if ([retval count] == 0)
    {
        return nil;
    }

    return [NSArray arrayWithArray:retval];
}

+(id)phazeRuleWithNumber:(int)ruleNumber groupsInRule:(NSArray<PXGroup *> *)groups
{
	return [[[PXPhazeRule alloc] initWithNumber:ruleNumber groupsInRule:groups] autorelease];
}

-(id)initWithNumber:(int)ruleNumber groupsInRule:(NSArray<PXGroup *> *)groups
{
	self = [super init];
	
	if (self)
	{
		_ruleNumber = ruleNumber;
		_groups = [[NSArray arrayWithArray:groups] retain];
	}
	
	return self;
}

-(void)dealloc
{
	[_groups release];
	[super dealloc];
}

-(int)cardsRequired
{
	int i = 0;
	for (PXGroup *group in _groups)
	{
		i += [group cardsRequired];
	}
	
	return i;
}

-(PXGroup *)groupAt:(int)position
{
	return [_groups objectAtIndex:position];
}

-(int)count
{
	return (int)[_groups count];
}

-(NSUInteger)countByEnumeratingWithState:(NSFastEnumerationState *)state objects:(id *)stackbuf count:(NSUInteger)len
{
	return [_groups countByEnumeratingWithState:state objects:stackbuf count:len];
}

-(NSString *)description
{
	NSMutableString *str = [NSMutableString stringWithCapacity:100];
	
	for (PXGroup *group in _groups)
	{
		[str appendString:[group description]];
		[str appendString:@", "];
	}
	
	return str;
}

@end
