//
//  Game.m
//  phazex
//
//  Created by toddha on 11/27/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Card.h"
#import "ComputerPlayer.h"
#import "Game.h"
#import "Group.h"
#import "Logging.h"
#import "PhazeRule.h"
#import "PhazeRules.h"
#import "Scoreboard.h"
#import "SessionScore.h"
#import "Table.h"

@interface PXGame () <PXCardCollectionDelegate>
-(void)fireTableChanged;

@property (readwrite, strong, nonatomic) PXCardCollection *deck;
@property (readwrite, strong, nonatomic) PXCardCollection *discard;
@property (readwrite, strong, nonatomic) PXTable *table;
@property (readwrite, strong, nonatomic) PXPhazeRules *phazeRules;
@property (readwrite, strong, nonatomic) PXPlayer *dealingPlayer;
@property (readwrite, strong, nonatomic) PXPlayer *currentPlayer;

@end

@implementation PXGame

@synthesize delegate = _delegate;
@synthesize state = _state;
@synthesize discard = _discard;
@synthesize phazeRules = _phazeRules;
@synthesize gameRules = _gameRules;
@synthesize table = _table;

+(PXGame *)game
{
	return [[[PXGame alloc] init] autorelease];
}

-(id)init
{
	self = [super init];
	if (self)
	{
		_state = gsStarting;
		_players = [[NSMutableArray alloc] initWithCapacity:8];
		_handNumber = 0;
	}
	
	return self;
}

-(void)dealloc
{
	[_gameThread cancel];
	[_gameThread release];
	[_players release];
	[_deck release];
	[_discard release];
	[_table release];
	[_phazeRules release];
	[_gameRules release];
	[_dealingPlayer release];
	[_currentPlayer release];
	
	[super dealloc];
}

-(void)addPlayer:(PXPlayer *)player
{
	if (_state != gsStarting)
	{
		[PXLogging raiseError:@"Tried to add a player to the game when game was not in the starting state."];
		return;
	}
	
	[player setGameUniqueId:(int)[_players count]];
	[_players addObject:player];
}

-(void)startGameInBackground
{
	[self waitForGameToFinish];
	_gameThread = [[NSThread alloc] initWithTarget:self selector:@selector(startGameThreadEntry:) object:nil];
	[_gameThread setName:@"pxGame thread"];
	[_gameThread start];
}

-(void)waitForGameToFinish
{
	if (_gameThread != nil)
	{
		int loops = 0;
		while (![_gameThread isFinished])
		{
			NSLog(@"%d", loops);
			loops++;
			[NSThread sleepForTimeInterval:0.1];
		}
		[_gameThread release];
		_gameThread = nil;
	}
}

-(void)startGameThreadEntry:(NSObject*)arg
{
	[self startGame];
}

-(void)startGame
{
	if (_state != gsStarting)
	{
		[PXLogging raiseError:@"Tried to start game when game was not in the starting state."];
		return;
	}
	
	[PXLogging logGameMessage:@"Starting game..."];
	[self setDiscard:[PXCardCollection cards]];
	[[self discard] setDelegate:self];
	
	
#if DEBUG
	for (PXPlayer *player in [self players])
	{
		for (PXPlayer *player2 in [self players])
		{
			assert(player == player2 || [player gameUniqueId] != [player2 gameUniqueId]);
		}
	}
#endif
	
	while (_state != gsGameOver && ![[NSThread currentThread] isCancelled])
	{
		@autoreleasepool
		{
#if SUPERDEBUG
			[self debug_ValidateCardsInPlayCountIsCorrect];
#endif
			switch (_state)
			{
				case gsStarting:
					[PXLogging logGameMessage:@"Starting game..."];
					[self setPhazeRules:[PXPhazeRules defaultRules]];
					for (PXPlayer *player in [self players]) [player setPhazeRuleNumber:1];
					[_delegate gameStarting:self];
					_state = gsHandStarting;
					break;

				case gsHandStarting:
					[PXLogging logGameMessage:@"Starting hand..."];
					[self startHand];
					_state = gsTurnStarting;
					[_delegate handStartedInGame:self];
					break;

				case gsTurnStarting:
					_turnNumber++;
					[PXLogging logGameMessage:[NSString stringWithFormat:@"Current player is : %@", ([_currentPlayer isComputerPlayer] ? @"automated" : @"REAL")]];
					[PXLogging logGameMessage:[NSString stringWithFormat:@"Current player phaze : %@", [_currentPlayer phazeRuleInGame:self]]];
					for (PXPlayer *player in [self players]) [player playerStartingTurn:_currentPlayer];
					_state = gsPickingCard;
					[_delegate turnStartedInGame:self];
					break;
				
				case gsPickingCard:
					[PXLogging logGameMessage:@"Picking card..."];
					[_currentPlayer pickCard];
					_state = gsPlaying;
					break;
					
				case gsPlaying:
					[PXLogging logGameMessage:@"Playing..."];
					[_currentPlayer play];
					_state = gsTurnEnding;
					break;
				
				case gsTurnEnding:
					[PXLogging logGameMessage:@"Ending turn..."];
					if ([self endTurn])
					{
						_state = gsHandEnding;
					}
					else
					{
						[self checkForStalemate];
						_state = gsTurnStarting;
					}
					
					[_delegate turnEndedInGame:self];
					break;

				case gsHandEnding:
					[PXLogging logGameMessage:@"Ending hand"];
					if ([self endHand])
					{
						_state = gsEnding;
					}
					else
					{
						_state = gsHandStarting;
					}
					
					[_delegate handEndedInGame:self];
					break;

				case gsEnding:
					[self endGame];
					_state = gsGameOver;
					break;

				default:
					[PXLogging raiseError:[NSString stringWithFormat:@"The game state is not implemented %d", _state]];
					break;
			}
		}
	}
}

-(void)endGame
{
	[PXLogging logGameMessage:@"Ending game..."];
	
	NSMutableArray<PXPlayer *> *winners = [NSMutableArray arrayWithCapacity:[[self players] count]];
	for (PXPlayer *player in [self players])
	{
		if ([player phazeRuleNumber] > [[self phazeRules] count])
		{
			[winners addObject:player];
		}
	}
	
	// TODO : check for scores
	if ([winners count] > 1)
	{
		NSMutableArray<PXPlayer *> *realWinners = [NSMutableArray arrayWithCapacity:[winners count]];
		int minScore = 0;
		for (PXPlayer *player in winners)
		{
			int playerScore = [[player scoreboard] points];
			if (playerScore < minScore || [realWinners count] == 0)
			{
				minScore = playerScore;
				[realWinners removeAllObjects];
				[realWinners addObject:player];
			}
			else if (minScore == playerScore)
			{
				[realWinners addObject:player];
			}
		}
		
		winners = realWinners;
	}
	
	
	[PXLogging logGameMessage:[NSString stringWithFormat:@"----- %lu game winners!", (unsigned long)[winners count]]];
	for (PXPlayer *player in winners)
		[PXLogging logGameMessage:[NSString stringWithFormat:@"WINNER: %@", player]];
	
	[_delegate gameFinished:self winningPlayers:winners];
}

-(void)startHand
{
	_handNumber++;
	_turnNumber = 0;
	
	int playerCount = (int)[[self players] count];
	[self setDealingPlayer:[[self players] objectAtIndex:((_handNumber - 1) % playerCount)]];
	[self setCurrentPlayer:[[self players] objectAtIndex:(_handNumber % playerCount)]];
	
	[self setTable:[PXTable table]];
	[self fireTableChanged];
	
	[self setDeck:[PXCardCollection deck]];
	[[self discard] clear];
	
	for (PXPlayer *player in [self players])
	{
		[PXLogging logGameMessage:[NSString stringWithFormat:@"%@ is on phaze %d for this hand.", player, [player phazeRuleNumber]]];
		[[player scoreboard] addSessionWherePlayerDealt:(_dealingPlayer == player) forPhazeNumber:[player phazeRuleNumber]];
		[player setCompletedPhaze:NO];
		[player startingHand];
		[player setSkipCount:0];
	}
	
	[self deal];
	
	PXCard *discard = RetainCard([[self deck] cardAtIndex:0]);
	[[self deck] removeCardAtIndex:0];
	[[self discard] addCard:discard];
	ReleaseCard(discard);
	
	[[self delegate] game:self player:nil discarded:discard];
}

-(void)deal
{
	for (PXPlayer *player in [self players])
	{
		PXCardCollection *hand = [PXCardCollection cards];
		while ([hand count] != 10)
		{
			PXCard *c = RetainCard([[self deck] cardAtIndex:0]);
			[[self deck] removeCardAtIndex:0];
			[hand addCard:c];
			ReleaseCard(c);
		}
		
		[player dealtCards:hand];
	}
}

-(BOOL)endHand
{
	// check to see if each player completed their phaze
	// if so, increase their phaze number
	for (PXPlayer *player in [self players])
	{
		// do score counting
		PXSessionScore *session = [[player scoreboard] currentSession];
		[session setMadePhaze:[player completedPhaze]];
		[session setPoints:[[player hand] pointsCountByRules:_gameRules]];
		
		if ([player completedPhaze])
		{
			[player setPhazeRuleNumber:([player phazeRuleNumber] + 1)];
		}
	}
	
	BOOL endGame = NO;
	for (PXPlayer *player in [self players])
	{
		if ([player phazeRuleNumber] > [[self phazeRules] count])
		{
			endGame = YES;
		}
	}
	
	return endGame;
}

-(BOOL)endTurn
{	
	if ([[_currentPlayer hand] count] > 0)
	{
		[self setCurrentPlayer:[self playerAfterPlayer:_currentPlayer]];
		
		while ([_currentPlayer skipCount] != 0)
		{
			[PXLogging logGameMessage:[NSString stringWithFormat:@"%@ is skipped this turn", _currentPlayer]];
			[[self delegate] game:self playerSkipped:_currentPlayer];
			[_currentPlayer skip];
			[_currentPlayer setSkipCount:([_currentPlayer skipCount] - 1)];
			[self setCurrentPlayer:[self playerAfterPlayer:_currentPlayer]];
		}
		
		return NO;
	}
	else
	{
		// TODO : Tally scores
		// TODO : Check to see if the end of the game!
		return YES;
	}
}

-(void)checkForStalemate
{
	/*BOOL stalemate = YES;
	
    for (int i = 0; i < this.players.Count; i++)
    {
        if (!this.players[i].CompletedPhaze)
        {
            stalemate = false;
            break;
        }
    }

    // check to see if any cards in the discard pile, hands, or deck can be played
    if (stalemate)
    {
        List<Card> deckBackup = new List<Card>();
        Stack<Card> discardBackup = new Stack<Card>();
        CardCollection cards = new CardCollection();

        // get all cards in all hands
        for (int i = 0; i < this.players.Count; i++)
        {
            cards.AddRange(this.players[i].Hand.Copy());
        }

        // get all cards in the deck
        while (!this.deck.IsTopCardNull())
        {
            Card c = this.deck.RemoveCard();
            cards.Add(c.CarbonCopy());
            deckBackup.Add(c);
        }

        // restore all cards in the deck
        this.deck.ShuffleAndAddUnused(deckBackup);

        // get all cards in the discard
        while (this.discard.ReadTopCard() != null)
        {
            Card c = this.discard.RemoveTopCard();
            cards.Add(c.CarbonCopy());
            discardBackup.Push(c);
        }

        // restore all cards in the discard
        while (discardBackup.Count > 0)
        {
            this.discard.AddTopCard(discardBackup.Pop());
        }

        // go through each group, see if we can add it.
        for (int i = 0; i < this.Table.Count; i++)
        {
            Group g = this.Table.ReadGroup(i);
            for (int j = 0; j < cards.Count; j++)
            {
                Card c = cards[j];

                if (g.CanAdd(c, this.Rules))
                {
                    stalemate = false;
                    break;
                }
            }

            if (!stalemate)
            {
                break;
            }
        }
    }

    // add a new deck
    if (stalemate)
    {
        PhazeXLog.LogInformation("Hand reached stalemate, adding new deck.");

        List<Card> cc = new List<Card>();
        Deck d = new Deck(this.Rules);

        while (!d.IsTopCardNull())
        {
            cc.Add(d.RemoveCard());
        }

        this.deck.ShuffleAndAddUnused(cc);
    }*/
}

-(PXCard *)readDiscard
{
	return [[self discard] cardAtIndex:([[self discard] count] - 1)];
}

-(NSArray<PXPlayer *> *)allPlayersExcept:(PXPlayer *)player
{
	NSMutableArray<PXPlayer *> *array = [NSMutableArray arrayWithCapacity:[[self players] count]];
	for (PXPlayer *p in [self players])
	{
		if (p != player)
		{
			[array addObject:p];
		}
	}
	
	return array;
}

-(PXCard *)deckCardForPlayer:(PXPlayer *)player
{
	return [self getDeckCard:YES forPlayer:player];
}

-(PXCard *)discardForPlayer:(PXPlayer *)player
{
	return [self getDeckCard:NO forPlayer:player];
}

-(PXCard *)getDeckCard:(BOOL)deckCard forPlayer:(PXPlayer *)player
{
	if (player != _currentPlayer)
	{
		[PXLogging raiseError:[NSString stringWithFormat:@"A player (%p, %@) other than the current player (%p, %@) tried to get a card from the deck/discard pile", player, player, _currentPlayer, _currentPlayer]];
		return nil;
	}
	
	PXCard *card = nil;
	if (deckCard)
	{
		card = RetainCard([[self deck] cardAtIndex:0]);
		[[self deck] removeCardAtIndex:0];
		[[player hand] addCard:card];
		ReleaseCard(card);
		
		[PXLogging logGameMessage:[NSString stringWithFormat:@"Got %@ from deck", CardDescription(card)]];
		
		if ([[self deck] count] == 0)
		{
			[PXLogging logGameMessage:@"Game status : Removing all discards from the pile and shuffling them back into the deck."];
			PXCardCollection *underlings = [[self discard] allExceptTop];
			[[self deck] addCards:underlings]; // after, using underlings is unsafe
			[[self deck] shuffle];
			[PXLogging logGameMessage:[NSString stringWithFormat:@"Game status : New deck count is %d", [[self deck] count]]];
		}
		
		[[self delegate] game:self playerPickedDeckCard:player];
		for (PXPlayer *p in [self players])
		{
			[p playerGotDeckCard:player];
		}
	}
	else
	{
		int idx = ([[self discard] count] - 1);
		card = RetainCard([[self discard] cardAtIndex:idx]);
		[[self discard] removeCardAtIndex:idx];
		[[player hand] addCard:card];
		ReleaseCard(card);
		
		[PXLogging logGameMessage:[NSString stringWithFormat:@"Got %@ from discard", CardDescription(card)]];
		
		for (PXPlayer *p in [self players])
		{
			[p player:player gotDiscard:card];
		}
		
		[[self delegate] game:self player:player gotDiscard:card];
	}
	
	return card;
}

-(void)discard:(PXCard *)card skip:(PXPlayer *)skippedPlayer forPlayer:(PXPlayer *)player
{
	if (player != _currentPlayer)
	{
		[PXLogging raiseError:[NSString stringWithFormat:@"A player (%p, %@) other than the current player (%p, %@) tried to discard a card (%@).", player, player, _currentPlayer, _currentPlayer, CardDescription(card)]];
		return;
	}
	
	if (![[player hand] containsCardWithId:CardId(card)])
	{
		[PXLogging raiseError:@"Player does not have card in hand!"];
		return;
	}
	
	if (!CardIsSkip(card))
	{
		[PXLogging raiseError:[NSString stringWithFormat:@"Card (%@) is not a skip card.", CardDescription(card)]];
		return;
	}
	
	[PXLogging logGameMessage:[NSString stringWithFormat:@"%@ has been skipped by %@", skippedPlayer, _currentPlayer]];
	
	[skippedPlayer setSkipCount:([skippedPlayer skipCount] + 1)];
	[[[skippedPlayer scoreboard] currentSession] skippedByPlayer:_currentPlayer];
	[[[_currentPlayer scoreboard] currentSession] skippedPlayer:skippedPlayer];
	
	PXCard *realCard = RetainCard([[player hand] cardWithId:CardId(card)]);
	[[player hand] removeCard:realCard];
	[[self discard] addCard:realCard];
	[[self delegate] game:self player:player discarded:realCard];
	ReleaseCard(realCard);
}

-(void)discard:(PXCard *)card forPlayer:(PXPlayer *)player
{
	if (player != _currentPlayer)
	{
		[PXLogging raiseError:[NSString stringWithFormat:@"A player (%p, %@) other than the current player (%p, %@) tried to discard a card (%@).", player, player, _currentPlayer, _currentPlayer, CardDescription(card)]];
		return;
	}
		
	if (![[player hand] containsCardWithId:CardId(card)])
	{
		[PXLogging raiseError:@"Player does not have card in hand!"];
		return;
	}
	
	if (CardIsSkip(card))
	{
		[PXLogging raiseError:@"Player tried to discard a skip without picking a player to skip."];
		return;
	}
	
	[PXLogging logGameMessage:[NSString stringWithFormat:@"Discarding %@", CardDescription(card)]];
	
	PXCard *realCard = RetainCard([[player hand] cardWithId:CardId(card)]);
	[[player hand] removeCard:realCard];
	[[self discard] addCard:realCard];
	[[self delegate] game:self player:player discarded:realCard];
	ReleaseCard(realCard);
}

-(void)makePhaze:(NSArray *)groups forPlayer:(PXPlayer *)player
{
	[PXLogging logGameMessage:[NSString stringWithFormat:@"Player %@ is making phaze.", player]];
	if (player != _currentPlayer)
	{
		[PXLogging raiseError:[NSString stringWithFormat:@"A player (%p, %@) other than the current player (%p, %@) tried to make their phaze.", player, player, _currentPlayer, _currentPlayer]];
		return;
	}
	
	PXPhazeRule *pr = [player phazeRuleInGame:self];
	NSMutableArray *phazeRuleGroups = [NSMutableArray arrayWithCapacity:[pr count]];
	for (PXGroup *g in pr)
	{
		[phazeRuleGroups addObject:g];
	}
	
	for (PXGroup *group in groups)
    {
		// remove from the phaze rule groups the corresponding item
		PXGroup *found = nil;
		for (PXGroup *phazeRuleGroup in phazeRuleGroups)
		{
			if ([group type] == [phazeRuleGroup type] && [group cardsRequired] == [phazeRuleGroup cardsRequired])
			{
				found = phazeRuleGroup;
				break;
			}
		}
		
		if (found == nil)
		{
			[PXLogging raiseError:@"Player completed a different phaze!?"];
			return;
		}
		else
		{
			[phazeRuleGroups removeObject:found];
		}
		
		if (![group check])
		{
			[PXLogging raiseError:[NSString stringWithFormat:@"Group (%@) is incorrect!", group]];
			return;
		}

		for (int i = 0; i < [group count]; ++i)
		{
			PXCard *card = [group cardAtIndex:i];
			if (![[player hand] containsCardWithId:CardId(card)])
			{
				[PXLogging raiseError:@"Player does not have card in hand that they were trying to meld!"];
				return;
			}
        }
    }

	if ([phazeRuleGroups count] != 0)
	{
		[PXLogging raiseError:@"Player did not complete all of the phaze!"];
		return;
	}

	// do the actual adding to the table
	for (PXGroup *group in groups)
	{
		// remove all the cards from the players hand
		for (int i = 0; i < [group count]; ++i)
		{
			PXCard *card = [group cardAtIndex:i];
			PXCard *realCard = RetainCard([[player hand] cardWithId:CardId(card)]);
			[[player hand] removeCard:realCard];
			if (CardIsWild(realCard))
			{
				CardSetNumber(realCard, CardNumber(card));
			}
			
			[group replaceCard:card withCard:realCard];
			ReleaseCard(realCard);
		}
		
		[[self table] addGroup:group];
	}
	
	[self fireTableChanged];
	[player setCompletedPhaze:YES];

	// TODO : Tell players that this guy made his phaze!
	// who? THIS GUY.
}

-(void)playCard:(PXCard *)card onTableGroup:(PXGroup *)group forPlayer:(PXPlayer *)player
{
	[PXLogging logGameMessage:[NSString stringWithFormat:@"Player %@ is playing on group %@ with card %@", player, group, CardDescription(card)]];
	
	if (![group checkWithCard:card byRules:_gameRules])
    {
		[PXLogging raiseError:@"Card doesn't fit on group!"];
		return;
    }

	if (![[player hand] containsCardWithId:CardId(card)])
	{
		[PXLogging raiseError:@"Player does not have card!"];
		return;
	}
	
	if (![[self table] hasGroup:group])
	{
		[PXLogging raiseError:@"Table does not have group!"];
		return;
	}

	PXCard *realCard = RetainCard([[player hand] cardWithId:CardId(card)]);
	[[player hand] removeCard:realCard];
	[[self table] playCard:realCard onGroup:group byRules:_gameRules];
	ReleaseCard(realCard);
	[self fireTableChanged];
}

-(PXPlayer *)playerAfterPlayer:(PXPlayer *)player
{
	int i;
	for (i = 0; i < [[self players] count]; i++)
	{
		PXPlayer *p = [[self players] objectAtIndex:i];
		if (p == player)
		{
			return [[self players] objectAtIndex:((i + 1) % [[self players] count])];
		}
	}
	
	return nil;
}

-(void)fireTableChanged
{
	[_delegate tableChangedInGame:self];
}

#pragma mark -
#pragma mark Card Collection
- (void)cardCollectionChanged:(PXCardCollection *)collection
{
	if (collection == [self discard])
	{
		[[self delegate] game:self discardChanged:[self discard]];
	}
}

#pragma mark -
#pragma mark Debug

#if DEBUG
- (void)debug_ValidateCardsInPlayCountIsCorrect
{
	// expected is 108
	NSMutableArray<NSNumber *> *counts = [NSMutableArray array];
	
	for (PXPlayer *player in [self players])
	{
		[counts addObject:@( [[player hand] count] )];
	}
	
	for (PXGroup *group in [[self table] groups])
	{
		[counts addObject:@( [group count] )];
	}
	
	[counts addObject:@([[self deck] count])];
	[counts addObject:@([[self discard] count])];
	
	int count = 0;
	for (NSNumber *num in counts)
	{
		count = [num intValue] + count;
	}
	
	if (count != 0 && count != 108)
	{
		NSLog(@"%@", counts);
	}
	
	assert(count == 0 || count == 108);
	
}
#endif

@end
