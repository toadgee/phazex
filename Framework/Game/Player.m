//
//  Player.m
//  phazex
//
//  Created by toddha on 12/8/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Player.h"
#import "Logging.h"
#import "PhazeRule.h"
#import "PhazeRules.h"
#import "Game.h"
#import "Scoreboard.h"
#import "CardCollection.h"

@interface PXPlayer () <PXCardCollectionDelegate>
@property (readwrite, strong, nonatomic) PXCardCollection *hand;
@end

@implementation PXPlayer

@synthesize gameUniqueId = _gameUniqueId;
@synthesize name = _name;
@synthesize delegate = _delegate;
@synthesize isMyTurn = _isMyTurn;
@synthesize completedPhaze = _completedPhaze;
@synthesize isComputerPlayer = _isComputerPlayer;
@synthesize hand = _hand;
@synthesize game = _game;
@synthesize scoreboard = _scoreboard;

-(void)notifyChanged
{
	// no-op currently
}

-(NSString *)description
{
	return [NSString stringWithFormat:@"%@Player : %@", (_isComputerPlayer ? @"Computer " : @""), [self name]];
}

+(id)playerWithGame:(PXGame *)game
{
	return [[[PXPlayer alloc] initWithGame:game] autorelease];
}

-(id)initWithGame:(PXGame *)game
{
	self = [super init];
	if (self)
	{
		_skipCount = 0;
		_scoreboard = [[PXScoreboard alloc] init];
		_isComputerPlayer = NO;
		_game = game;
		_delegate = nil;
		_isMyTurn = NO;
	}
	
	return self;
}

-(void)dealloc
{
	[_scoreboard release];
	[_hand release];
	[_name release];
	[super dealloc];
}

-(PXPhazeRule *)phazeRuleInGame:(PXGame *)game
{
	return [[game phazeRules] phazeRuleNumber:_phazeRuleNumber];
}



-(BOOL)isMyTurn
{
	return _isMyTurn;
}

-(void)setIsMyTurn:(BOOL)myTurn
{
	if (_isMyTurn != myTurn)
	{
		_isMyTurn = myTurn;
		[self notifyChanged];
	}
}

-(int)phazeRuleNumber
{
	return _phazeRuleNumber;
}

-(void)setPhazeRuleNumber:(int)prNum
{
	if (_phazeRuleNumber != prNum)
	{
		_phazeRuleNumber = prNum;
		[self notifyChanged];
	}
}


-(int)skipCount
{
	return _skipCount;
}

-(void)setSkipCount:(int)count
{
	if (_skipCount != count)
	{
		_skipCount = count;
		[self notifyChanged];
	}
}



-(void)skip
{
	
}





-(void)startingHand
{
	
}

-(void)dealtCards:(PXCardCollection *)hand
{
	[[self hand] setDelegate:nil];
	[self setHand:[PXCardCollection cards]];
	[[self hand] setDelegate:self];
	while ([hand count] > 0)
	{
		PXCard *card = RetainCard([hand cardAtIndex:0]);
		[hand removeCardAtIndex:0];
		[[self hand] addCard:card];
		ReleaseCard(card);
	}
}

-(void)pickCard
{
	_isMyTurn = YES;
	[_delegate playerPickCard:self];
}

-(void)play
{
	[_delegate performSelector:@selector(playerShouldPlay:)];
}

-(void)playerGotDeckCard:(PXPlayer *)player
{
	[_delegate player:self playerGotDeckCard:player];
}

-(void)player:(PXPlayer *)player gotDiscard:(PXCard *)card {}

-(void)playerStartingTurn:(PXPlayer *)player
{
	[_delegate player:self playerStartingTurn:player];
}

- (void)cardCollectionChanged:(PXCardCollection *)collection
{
	if (collection == [self hand])
	{
		[[self delegate] player:self handChanged:collection];
	}
}

@end
