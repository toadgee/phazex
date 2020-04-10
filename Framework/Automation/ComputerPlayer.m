//
//  ComputerPlayer.m
//  phazex
//
//  Created by toddha on 12/8/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Card.h"
#import "CardGroupMatch.h"
#import "CardTracker.h"
#import "ComputerPlayer.h"
#import "Flush.h"
#import "Game.h"
#import "Logging.h"
#import "PhazeRule.h"
#import "Run.h"
#import "Set.h"
#import "Table.h"
static NSTimeInterval s_sleepTime = 0.5f;

@interface PXComputerPlayer ()
@end

@implementation PXComputerPlayer

+ (void)setSleepTime:(NSTimeInterval)sleepTime
{
	s_sleepTime = sleepTime;
}

+ (NSTimeInterval)sleepTime
{
	return s_sleepTime;
}

- (void)logThoughts:(NSString *)thoughts
{
	//NSLog(@"%@ : [Thoughts] %@", [self name], thoughts);
}

+(instancetype)playerWithName:(NSString *)name game:(PXGame *)game
{
	return [[[PXComputerPlayer alloc] initWithName:name game:game] autorelease];
}

- (id)initWithGame:(PXGame *)game
{
	return [self initWithName:nil game:game];
}

- (instancetype)initWithName:(NSString *)name game:(PXGame *)game
{
	self = [super initWithGame:game];
	
	if (self)
	{
		[self setDelegate:self];
		_isComputerPlayer = YES;
		[self setName:name];
	}
	
	return self;
}

- (void)dealloc
{
	[_tracker release];
	[super dealloc];
}

- (void)sleep
{
	if (s_sleepTime > 0.0)
	{
		[NSThread sleepForTimeInterval:s_sleepTime];
	}
}

- (void)gameStartingHand
{
	[_tracker release];
	_tracker = [[PXCardTracker alloc] init];
}

-(void)player:(PXPlayer *)player playerStartingTurn:(PXPlayer *)playerStartingTurn {}

- (void)playerPickCard:(PXPlayer *)player
{
	[self logThoughts:@"Starting turn"];
	//NSLog(@"%@ : Hand is : %@", [self name], [[_hand copy] autorelease]);
	//NSLog(@"%@ : Need %d cards to complete phaze.", [self name], [PXPhazeRule minimumCardsNeededToCompletePhaze:[self phazeRule] withHand:_hand byRules:[_game gameRules]]);
	[self sleep];
	
	BOOL grabDiscard = [self determineFirstPlay];
	
	if (grabDiscard)
	{
		//NSLog(@"%@ : Getting discard...", [self name]);
		[_game discardForPlayer:self];
	}
	else
	{
		//NSLog(@"%@ : Getting deck card...", [self name]);
		[_game deckCardForPlayer:self];
	}
	
	_isMyTurn = NO;
}

- (void)playerShouldPlay:(PXPlayer *)player
{
	//NSLog(@"%@ : Hand is : %@", [self name], [[_hand copy] autorelease]);
	//NSLog(@"%@ : Need %d cards to complete phaze.", [self name], [PXPhazeRule minimumCardsNeededToCompletePhaze:[self phazeRule] withHand:_hand byRules:[_game gameRules]]);
	
	if (!_completedPhaze)
	{
		[self phazePlay];
	}
	
	if (_completedPhaze)
	{
		while ([self tablePlay]);
	}
	
	[self discardPlay];
}

- (void)player:(PXPlayer *)player playerGotDeckCard:(PXPlayer *)playingPlayer
{
	// don't include skip cards, since you can't pick them up (MOST of the time)
	PXCard *card = [_game readDiscard];
	if (CardIsUnassignedWild(card) || CardIsNumbered(card))
    {
		[_tracker player:playingPlayer didNotWantCard:card];
    }
}

- (void)player:(PXPlayer *)player gotDiscard:(PXCard *)card
{
	// shouldn't this use the game?
	[super player:player gotDiscard:card];
	[_tracker player:player wantedCard:card];
}

- (void)player:(PXPlayer *)player handChanged:(PXCardCollection *)hand {}

- (void)phazePlay
{
	//NSLog(@"Phaze Play");
	//NSLog(@"%@ : Considering Phaze Play...", [self name]);
	if (_completedPhaze)
	{
		[PXLogging raiseError:@"entered phazePlay for computer player when we have already completed our phaze."];
		return;
	}

	PXPhazeRule *pr = [self phazeRuleInGame:[self game]];

	int needed = [PXPhazeRule minimumCardsNeededToCompletePhaze:pr withHand:_hand byRules:[_game gameRules]];

    if (needed == 0)
    {
		[self sleep];
		
		// the problem with the groups we get back is that they don't necessarily (something)
		NSArray *groups = [self determinePhazePlay];
		
		NSMutableArray *actualGroups = [NSMutableArray arrayWithCapacity:[groups count]];
		for (PXGroup *group in groups)
		{
			PXGroup *actualGroup = nil;
			switch ([group type])
			{
				case gtRun:
					actualGroup = [PXRun runWithCardsRequired:[group cardsRequired]];
					break;
				
				case gtSet:
					actualGroup = [PXSet setWithCardsRequired:[group cardsRequired]];
					break;
				
				case gtFlush:
					actualGroup = [PXFlush flushWithCardsRequired:[group cardsRequired]];
					break;
			}
			
			for (int i = 0; i < [group count]; ++i)
			{
				PXCard *card = [group cardAtIndex:i];
				PXCard *actual = [_hand cardWithId:CardId(card)];
				if (actual == nil)
				{
					[PXLogging raiseError:[NSString stringWithFormat:@"Could not find a translation for card : %@", CardDescription(card)]];
					return;
				}
				
				PXCard *actual2 = CopyCard(NULL, actual);
				if (CardIsWild(card))
				{
					CardSetNumber(actual2, CardNumber(card));
				}
				
				[actualGroup addCard:actual2];
				ReleaseCard(actual2);
			}
			
			
			if ([actualGroup count] != [group count])
			{
				[PXLogging raiseError:@"Could not make an appropriate translation of the groups!"];
				return;
			}
			
			[actualGroups addObject:actualGroup];
		}
		
		[_game makePhaze:actualGroups forPlayer:self];
    }

	return;
}

- (BOOL)tablePlay
{
	if (!_completedPhaze)
    {
		[PXLogging raiseError:@"Called tablePlay on ComputerPlayer even though we haven't completed our phaze yet."];
		return NO;
    }

	[self sleep];
	
	if ([_hand count] < 2)
	{
		return NO;
	}

	PXCardGroupMatch *match = [self determineTablePlayCards];
    if (match != nil)
    {
		PXCard *card = nil;
		if (CardIsWild([match card]))
		{
			card = [_hand cardWithNumber:kCardWildNumber withColor:CardColor([match card])];
			CardSetNumber(card, CardNumber([match card]));
		}
		else
		{
			card = [_hand cardWithNumber:CardNumber([match card]) withColor:CardColor([match card])];
		}
		
		if (card == nil)
		{
			[PXLogging raiseError:@"After determining table play cards, we couldn't find the cards that were referenced in the match."];
			return NO;
		}
		
		[_game playCard:card onTableGroup:[match group] forPlayer:self];
		return YES;
    }

	return NO;
}

-(void)skip
{
	[self sleep];
}

- (void)discardPlay
{
	[self sleep];
	[self logThoughts:@"Discarding"];
	PXCard *fakeCard = [self determineDiscardPlay];
	PXCard *card = [_hand cardWithId:CardId(fakeCard)];
	if (CardIsSkip(card))
	{
		//NSLog(@"%@ : Skipping somebody...", [self name]);
		PXPlayer *p = [self determinePlayerToSkip];
		[_game discard:card skip:p forPlayer:self];
	}
	else
	{
		[_game discard:card forPlayer:self];
	}
}


- (PXPlayer *)determinePlayerToSkip
{
	// TODO : make this better, for sure.
	return [_game playerAfterPlayer:self];
}

- (PXCardGroupMatch *)determineTablePlayCards
{
	// NSLog(@"Determine table play cards");
	if ([_hand count] <= 1)
	{
		[self logThoughts:[NSString stringWithFormat:@"I'm not going to play on the table because my hand only contains %d card.", [_hand count]]];
        return nil;
	}
    
    if ([[_game table] count] == 0)
	{
		[self logThoughts:@"I'm not going to play on the table because there are not groups on the table."];
        return nil;
	}

    // compile an array list for each diff type
    NSMutableArray *runs = [NSMutableArray arrayWithCapacity:[[_game table] count]];
    NSMutableArray *sets = [NSMutableArray arrayWithCapacity:[[_game table] count]];
	NSMutableArray *colors = [NSMutableArray arrayWithCapacity:[[_game table] count]];

	PXCardCollection *hc = [[_hand copy] autorelease];

    // parse groups on table
	for (PXGroup *group in [_game table])
	{
        switch ([group type])
        {
            case gtFlush:
				[colors addObject:group];
                break;

            case gtRun:
				[runs addObject:group];
                break;

            case gtSet:
                [sets addObject:group];
                break;
        }
    }

    // compile into master array list
	NSArray *llg = [NSArray arrayWithObjects:runs, colors, sets, nil];

	for (NSMutableArray *groups in llg)
    {
		for (PXGroup *group in groups)
        {
            if ([hc count] > 1)
            {
				for (int i = 0; i < [hc count]; ++i)
				{
					PXCard *card = [hc cardAtIndex:i];
                    switch ([group type])
                    {
                        case gtFlush:
							if ([group checkWithCard:card byRules:[_game gameRules]])
                            {
								// log thoughts?
								return [PXCardGroupMatch matchWithCard:card withGroup:group];
                            }

                            break;

                        case gtRun:
							if (CardIsNumbered(card))
                            {
                                if ([group checkWithCard:card byRules:[_game gameRules]])
                                {
									// log thoughts?
                                    return [PXCardGroupMatch matchWithCard:card withGroup:group];
                                }
                            }
                            else if (CardIsUnassignedWild(card))
                            {
								if ([group guessWildCard:card byRules:[_game gameRules]])
								{
									// log thoughts?
									return [PXCardGroupMatch matchWithCard:card withGroup:group];
								}
                            }

                            break;

                        case gtSet:
                            if (CardIsNumbered(card))
                            {
								if ([group checkWithCard:card byRules:[_game gameRules]])
                                {
									// log thoughts?
    								return [PXCardGroupMatch matchWithCard:card withGroup:group];
                                }
                            }
                            else if (CardIsUnassignedWild(card))
                            {
								if ([group guessWildCard:card byRules:[_game gameRules]])
								{
									// log thoughts?
									return [PXCardGroupMatch matchWithCard:card withGroup:group];
								}
                            }

                            break;
                    }
                }
            }
        }
    }
    
    return nil;
}



- (NSArray *)determinePhazePlay
{
	//NSLog(@"%@ : Determining phaze play...", [self name]);
	NSArray *llg = [PXPhazeRule completePhaze:[self phazeRuleInGame:[self game]] withHand:_hand byRules:[_game gameRules]];
    
	// todo
    // look through the groups returned and decide which has most cards in it
    // or decide which group has the most points in the hand
    // or decide which cards we can keep to play on the table
	NSArray *groupsToMeld = [llg objectAtIndex:0];

    // assign numbers to wild cards
	NSMutableArray *goodGroupsToMeld = [NSMutableArray arrayWithCapacity:[groupsToMeld count]];
	for (PXGroup *group in groupsToMeld)
    {
		for (int i = 0; i < [group count]; ++i)
		{
			PXCard *card = [group cardAtIndex:i];
			if (CardIsUnassignedWild(card))
            {
				CardSetNumber(card, [group guessWildCard:card byRules:[_game gameRules]]);
            }
        }

        if ([group type] == gtRun)
        {
			[group sortByNumber];
        }

		[goodGroupsToMeld addObject:group];
    }

	return [NSArray arrayWithArray:goodGroupsToMeld];
}


- (PXCard *)determineDiscardPlay
{
	//NSLog(@"Determining discard play");
	if ([_hand count] < 2)
    {
		[self logThoughts:[NSString stringWithFormat:@"I'm discarding the only card in my hand, because it contains %d card.", [_hand count]]];
		return [_hand cardAtIndex:0];
    }

    // construct list of cards that we can discard
    PXCardCollection *possibleCards = [[_hand copy] autorelease];
	PXCardCollection *neededList = [PXCardCollection cards];

    // if we've made our phaze, then we don't need to worry about discarding a card that the next
    // player can play on the table -- we should have already played everything!
    if (!_completedPhaze)
    {
        // don't discard any cards we need for our phaze
        // so figure out which cards aren't needed for the phaze
        // number of cards we need to make our phaze without removing any cards
		int baseCards = [PXPhazeRule minimumCardsNeededToCompletePhaze:[self phazeRuleInGame:[self game]] withHand:_hand byRules:[_game gameRules]];

        // number of cards we need to make our phaze by removing one card
        int modCards;

		for (int i = 0; i < [possibleCards count]; ++i)
		{
			PXCard *card = [possibleCards cardAtIndex:i];
            PXCardCollection *handCopy = [[_hand copy] autorelease];
			PXCard *copy = RetainCard([handCopy cardWithId:CardId(card)]);
			[handCopy removeCardWithId:CardId(card)];
			modCards = [PXPhazeRule minimumCardsNeededToCompletePhaze:[self phazeRuleInGame:[self game]] withHand:handCopy byRules:[_game gameRules]];

            // if the number of cards we need without this card is the greater
            // than the number of cards WITH this card, then we need it
            if (modCards > baseCards)
            {
				[neededList addCard:copy];
            }
			
			ReleaseCard(copy);
        }

        // if we need all the cards, discard the highest one (which should make sense in
        // most circumstances. if not, who cares.)
        if ([neededList count] == [possibleCards count])
        {
			[self logThoughts:@"I'm discarding the biggest card, since I seem to need all the cards that I could discard."];
			PXCardCollection *handCopy = [[_hand copy] autorelease];
			[handCopy sortReverseByNumber];
			return [handCopy cardAtIndex:0];
        }

        // remove all the cards we need, leaving just the cards we don't need
		for (int i = 0; i < [neededList count]; ++i)
		{
			PXCard *card = [neededList cardAtIndex:i];
			[possibleCards removeCardWithId:CardId(card)];
        }
    }

    // if the next player can play on the table, try not to discard any cards 
    // that they might can play on the table
    if ([[_game playerAfterPlayer:self] completedPhaze])
    {
		[neededList clear];

		NSMutableArray *groupList = [NSMutableArray arrayWithCapacity:[[_game table] count]];
		for (int i = 0; i < [possibleCards count]; ++i)
		{
			PXCard *card = [possibleCards cardAtIndex:i];
			PXGroup *g = nil;
			if ([[_game table] canPlayCard:card byRules:[_game gameRules] outGroup:&g])
            {
				[neededList addCardCopy:card];
				[groupList addObject:g];
            }
        }

        // what if all our cards can be played on the table?
        if ([neededList count] == [possibleCards count])
        {
            // this goes under the basic principle that playing on
            // sets/color sets is better than runs/color runs
            // this is regardless of how many cards are in the player's hand
            // because if we play on a set/color set it doesn't open
            // any new possibilities
            // check to see if there are any sets/color sets
            int i = -1;
            for (int ctr = 0; ctr < [groupList count]; ctr++)
            {
				PXGroup *group = [groupList objectAtIndex:ctr];
                if (([group type] == gtSet) || ([group type] == gtFlush))
                {
                    i = ctr;
                    break;
                }
            }

            // if so, discard that card
            if (i != -1)
            {
				[self logThoughts:@"It looks like all the cards that I have that I could discard could be played on the table. And since I see that there is a \"Set\" or a \"Colors\" group on the table, these are the safest to play on, so I will discard a card that can be played on there."];
				return [neededList cardAtIndex:i];
            }

            // otherwise we have to play on a run
            // if we can play a 1 or a 12, it's the best
			for (int i = 0; i < [neededList count]; ++i)
			{
				PXCard *card = [neededList cardAtIndex:i];
				if (CardIsMinimumOrMaximum(card))
                {
					[self logThoughts:@"It looks like all the cards that I have that I could discard could be played on the table. And since I see that there are not \"Set\" or a \"Colors\" groups on the table, I have to play on a run. The safest card to discard in this situation is a 1 or a 12 (since they can't be built off of further), so I will do that."];
                    return card;
                }
            }

            // otherwise just play random card
			[self logThoughts:@"It looks like all the cards that I have that I could discard could be played on the table. Since I'm too dumb to look to think about what they might have, I'm going to simply discard the card with the most points in my hand."];
			PXCardCollection *cc = [[possibleCards copy] autorelease];
			[cc sortReverseByNumber];
            return [cc cardAtIndex:0];
        }

        // remove cards that are needed for the table
		for (int i = 0; i < [neededList count]; ++i)
		{
			PXCard *card = [neededList cardAtIndex:i];
			[possibleCards removeCardWithId:CardId(card)];
        }

		[self logThoughts:@"I'm discarding the biggest card I have that can't be played on the table."];
		[possibleCards sortReverseByNumber];
		return [possibleCards cardAtIndex:0];
    }
    else
    {
        // so we need to figure out if the player has seen any of the cards that
        // we have. if not, then it's a crapshoot on which one to discard
        // so we need to figure out if the phaze rule that the player is on
        // is number based or color based (or both). not that if it's both then
        // the amount of possibility removal vastly decreases.
        BOOL colorBased = NO;
        BOOL numberBased = NO;
		PXPlayer *nextPlayer = [_game playerAfterPlayer:self];
		PXPhazeRule *nextPlayerPhazeRule = [nextPlayer phazeRuleInGame:[self game]];
		for (PXGroup *group in nextPlayerPhazeRule)
        {
            if ([group type] == gtFlush)
                colorBased = YES;
			else if ([group type] == gtRun || [group type] == gtSet)
                numberBased = YES;
        }

        BOOL seenOne = NO;
		PXCardCollection *wilds = [PXCardCollection cards];
		for (int i = 0; i < [possibleCards count]; ++i)
		{
			PXCard *card = [possibleCards cardAtIndex:i];
            if (numberBased && colorBased)
            {
				if ([_tracker hasPlayer:nextPlayer seenAnyCardsWithNumber:CardNumber(card)] && [_tracker hasPlayer:nextPlayer seenAnyCardsWithColor:CardColor(card)])
                {
                    seenOne = YES;
                }

                if (CardIsUnassignedWild(card))
                {
					[wilds addCard:card];
                }
            }
            else if (numberBased)
            {
                if ([_tracker hasPlayer:nextPlayer seenAnyCardsWithNumber:CardNumber(card)])
                {
                    seenOne = YES;
                }

                if (CardIsUnassignedWild(card))
                {
					[wilds addCardCopy:card];
                }
            }
            else if (colorBased)
            {
                if ([_tracker hasPlayer:nextPlayer seenAnyCardsWithColor:CardColor(card)])
                {
                    seenOne = NO;
                }
            }
        }

        // remove wild cards (this is hit if number matters)
		if ([wilds count] == [possibleCards count])
        {
            // just play random card
			[self logThoughts:@"It looks like all the cards that I have are wilds and the groups on the table are number based. Unfortunately I have to discard a wild card. Bummer."];
            return [wilds cardAtIndex:0];
        }

		for (int i = 0; i < [wilds count]; ++i)
		{
			PXCard *wildCard = [wilds cardAtIndex:i];
			[possibleCards removeCardWithId:CardId(wildCard)];
        }

        // we haven't seen any cards in our hand
        if (!seenOne)
        {
            // otherwise just play random card
			[self logThoughts:@"The next player hasn't seen any of the cards we could discard, so I can't make any rational choices about what I should discard. Therefore I will discard the biggest card."];
			PXCardCollection *cc = [[possibleCards copy] autorelease];
			[cc sortReverseByNumber];
            return [cc cardAtIndex:0];
        }

        // calculate the score
		NSMutableArray *scores = [NSMutableArray arrayWithCapacity:[possibleCards count]];
		for (int i = 0; i < [possibleCards count]; ++i)
		{
			PXCard *card = [possibleCards cardAtIndex:i];
            int score = 0;
            if (numberBased && colorBased)
            {
				score = [_tracker player:nextPlayer percentageWantedCardsWithColor:CardColor(card)];
                score += [_tracker player:nextPlayer percentageWantedCardsWithNumber:CardNumber(card)];
                score /= 2;
            }
            else if (numberBased)
            {
                score += [_tracker player:nextPlayer percentageWantedCardsWithNumber:CardNumber(card)];
            }
            else if (colorBased)
            {
				score = [_tracker player:nextPlayer percentageWantedCardsWithColor:CardColor(card)];
            }

			[scores addObject:[NSNumber numberWithInt:score]];
        }

        // now order by score
        PXCardCollection *cc = [PXCardCollection cards];
		NSMutableArray *ic = [NSMutableArray arrayWithCapacity:[possibleCards count]];
		int ctr, ctr2;
        for (ctr = 0; ctr < [possibleCards count]; ctr++)
        {
            BOOL inserted = NO;
            for (ctr2 = 0; ctr2 < [cc count]; ctr2++)
            {
				int scoresCtr = [[scores objectAtIndex:ctr] intValue];
				int icCtr2 = [[ic objectAtIndex:ctr2] intValue];
                if (scoresCtr < icCtr2)
                {
					[cc insertCardCopy:[possibleCards cardAtIndex:ctr] atIndex:ctr2];
					[ic insertObject:[scores objectAtIndex:ctr] atIndex:ctr2];
                    inserted = YES;
                    break;
                }
            }

            if (!inserted)
            {
				[cc addCardCopy:[possibleCards cardAtIndex:ctr]];
				[ic addObject:[scores objectAtIndex:ctr]];
            }
        }

		[self logThoughts:[NSString stringWithFormat:@"I'm discarding the card (%@) that the next player wants the least, with a score of %@", CardDescription([cc cardAtIndex:0]), [scores objectAtIndex:0]]];
		return [cc cardAtIndex:0];
    }
}

- (BOOL)determineFirstPlay
{
	// never grab the skip card if it's the first card
	PXCard *discard = [_game readDiscard];
    if (CardIsSkip(discard))
    {
		[self logThoughts:@"I want the deck card because the discard is a skip card."];
        return NO;
    }

    if (_completedPhaze)
    {
        // determine if this card can be played on the table.
        // if so, grab it. Otherwise grab the deck card.
		for (PXGroup *group in [_game table])
        {
			if ([group checkWithCard:discard byRules:[_game gameRules]])
            {
				if ([group checkWithCard:discard byRules:[_game gameRules]])
                {
					[self logThoughts:@"I've completed my phaze, and I've noticed that the discard can be played on the table, so I'm picking it."];
                    return YES;
                }
            }
        }

		[self logThoughts:@"I've completed my phaze, and I've noticed that the discard can't be played on the table. So I'm getting the deck card."];
        return NO;
    }

    int cardsNeededWithoutDiscard = [PXPhazeRule minimumCardsNeededToCompletePhaze:[self phazeRuleInGame:[self game]] withHand:_hand byRules:[_game gameRules]];
    if (cardsNeededWithoutDiscard != 0)
    {
        int cardsNeededWithDiscard = cardsNeededWithoutDiscard + 1;
		for (int i = 0; i < [_hand count]; ++i)
		{
			PXCard *card = [_hand cardAtIndex:i];
            PXCardCollection *handCopy = [[_hand copy] autorelease];
			[handCopy addCardCopy:discard];
			[handCopy removeCardWithId:CardId(card)];
            int cn = [PXPhazeRule minimumCardsNeededToCompletePhaze:[self phazeRuleInGame:[self game]] withHand:handCopy byRules:[_game gameRules]];

            if (cn < cardsNeededWithDiscard)
            {
                cardsNeededWithDiscard = cn;
            }
        }

		[self logThoughts:[NSString stringWithFormat:@"If I were going to get the discard, I'd need %d cards, without it I need %d cards. Thus I will%@ get the discard.", cardsNeededWithDiscard, cardsNeededWithoutDiscard, (cardsNeededWithDiscard < cardsNeededWithoutDiscard) ? @"" : @" NOT"]];
		return (cardsNeededWithDiscard < cardsNeededWithoutDiscard);
    }
	else
	{
		[self logThoughts:@"I can make my phaze without the discard!"];
	}

    if (CardIsUnassignedWild(discard))
    {
		for (PXGroup *group in [self phazeRuleInGame:[self game]])
        {
			switch ([group type])
            {
                case gtRun:
                case gtSet:
					[self logThoughts:@"The discard is a wild, and I'm working on sets or runs. I'm taking the card."];
                    return YES;
				default:
					break;
            }
        }

		NSArray *arrayOfGroups = [PXPhazeRule completePhaze:[self phazeRuleInGame:[self game]] withHand:_hand byRules:[_game gameRules]];
		
		for (NSArray *groups in arrayOfGroups)
        {
            for (PXGroup *group in groups)
            {
				if ([group checkWithCard:discard byRules:[_game gameRules]])
                {
					[self logThoughts:@"The discard is a wild and it works to my advantage to get it because it can be played on my groups that I'm going to meld this hand."];
                    return YES;
                }
            }
        }
    }

	[self logThoughts:@"Doesn't look like I need the discard, I'm getting the deck card."];
    return NO;
}

@end
