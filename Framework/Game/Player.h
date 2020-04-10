//
//  Player.h
//  phazex
//
//  Created by toddha on 12/8/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#include "Card.h"

@class PXCardCollection;
@class PXGame;
@class PXPhazeRule;
@class PXPlayer;
@class PXScoreboard;

@protocol PXPlayerDelegate <NSObject>
- (void)playerPickCard:(PXPlayer *)player;
- (void)playerShouldPlay:(PXPlayer *)player;
- (void)player:(PXPlayer *)player handChanged:(PXCardCollection *)hand;
- (void)player:(PXPlayer *)player playerGotDeckCard:(PXPlayer *)playerGotDeckCard;
- (void)player:(PXPlayer *)player playerStartingTurn:(PXPlayer *)playerStartingTurn;
@end


@interface PXPlayer : NSObject
{
	int                  _gameUniqueId; // unique for a game
	BOOL                 _isComputerPlayer; // really for debugging only to know if we hung in the UI or the automated player
	BOOL                 _isMyTurn;
	BOOL                 _completedPhaze;
	id<PXPlayerDelegate> _delegate;
	
	NSString            *_name;
	PXGame              *_game; // weak ref
	PXScoreboard          *_scoreboard;
	PXCardCollection    *_hand;
	int                  _phazeRuleNumber;
	int                  _skipCount;
}

@property (readwrite, assign, nonatomic) int gameUniqueId;
@property (retain, readwrite) NSString *name;
@property (assign, readwrite) id<PXPlayerDelegate> delegate;
@property (assign, readwrite) BOOL isMyTurn;
@property (assign, readwrite) BOOL completedPhaze;
@property (assign, readwrite) PXGame *game;
@property (assign, readwrite) int phazeRuleNumber;
@property (assign, readwrite) int skipCount;
@property (readonly) BOOL isComputerPlayer;
@property (readonly) PXCardCollection *hand;
@property (readonly) PXScoreboard *scoreboard;

+(id)playerWithGame:(PXGame *)game;

-(id)initWithGame:(PXGame *)game;

-(void)startingHand;
-(void)skip;
-(void)pickCard;
-(void)play;
-(void)dealtCards:(PXCardCollection *)hand;
-(void)playerGotDeckCard:(PXPlayer *)player;
-(void)player:(PXPlayer *)player gotDiscard:(PXCard *)card NS_REQUIRES_SUPER;

-(void)playerStartingTurn:(PXPlayer *)player;

// TODO : this should be set - and then we can get rid of game entirely on the player
- (PXPhazeRule *)phazeRuleInGame:(PXGame *)game;

@end


