//
//  Game.h
//  phazex
//
//  Created by toddha on 11/27/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#include "Card.h"

@class PXCardCollection;
@class PXGame;
@class PXGroup;
@class PXGameRules;
@class PXPhazeRules;
@class PXPlayer;
@class PXTable;

#import "GameState.h"

@protocol PXGameDelegate <NSObject>

- (void)gameStarting:(PXGame *)game;
- (void)game:(PXGame *)game player:(PXPlayer *)player discarded:(PXCard *)card;
- (void)game:(PXGame *)game playerPickedDeckCard:(PXPlayer *)player;
- (void)game:(PXGame *)game player:(PXPlayer *)player gotDiscard:(PXCard *)card;
- (void)game:(PXGame *)game playerSkipped:(PXPlayer *)player;

-(void)gameFinished:(PXGame *)game winningPlayers:(NSArray<PXPlayer *> *)winningPlayers;
-(void)tableChangedInGame:(PXGame *)game; // TODO : Remove this & anybody that cares can monitor table.
-(void)turnStartedInGame:(PXGame *)game;
-(void)turnEndedInGame:(PXGame *)game;
-(void)handStartedInGame:(PXGame *)game;
-(void)handEndedInGame:(PXGame *)game;

- (void)game:(PXGame *)game discardChanged:(PXCardCollection *)discard;
@end


@interface PXGame : NSObject
{
	id _delegate;
	NSThread *_gameThread; // TODO : Move outside of this. Put it in application
	
	enum PXGameState _state;
	
	NSMutableArray<PXPlayer *> *_players;
	PXCardCollection *_deck;
	PXCardCollection *_discard;
	PXTable *_table;
	PXPhazeRules *_phazeRules;
	PXGameRules *_gameRules;
	
	// game specific variables (not used in the hands)
	int _handNumber;
	
	// hand specific variables
	int _turnNumber;
	PXPlayer *_dealingPlayer;
	PXPlayer *_currentPlayer;
}

+(PXGame *)game;

@property (readwrite, assign, nonatomic) id<PXGameDelegate> delegate;
@property (readonly, assign, nonatomic) enum PXGameState state;
@property (readonly, strong, nonatomic) PXCardCollection *discard;
@property (readonly, strong, nonatomic) PXPhazeRules *phazeRules;
@property (readonly, strong, nonatomic) PXGameRules *gameRules;
@property (readonly, strong, nonatomic) PXTable *table;
@property (readonly, strong, nonatomic) NSArray<PXPlayer *> *players;

-(void)addPlayer:(PXPlayer *)player;

-(void)startGame;
-(void)startGameInBackground;
-(void)endGame;
-(void)waitForGameToFinish;

-(void)startHand;
-(void)deal;

-(BOOL)endHand;
-(BOOL)endTurn;

-(void)checkForStalemate;

-(PXCard *)readDiscard;

-(NSArray<PXPlayer *> *)allPlayersExcept:(PXPlayer *)player;

-(PXCard *)deckCardForPlayer:(PXPlayer *)player;
-(PXCard *)discardForPlayer:(PXPlayer *)player;
-(PXCard *)getDeckCard:(BOOL)deckCard forPlayer:(PXPlayer *)player;
-(void)makePhaze:(NSArray *)groups forPlayer:(PXPlayer *)player;
-(void)playCard:(PXCard *)card onTableGroup:(PXGroup *)group forPlayer:(PXPlayer *)player;
-(void)discard:(PXCard *)card skip:(PXPlayer *)skippedPlayer forPlayer:(PXPlayer *)player;
-(void)discard:(PXCard *)card forPlayer:(PXPlayer *)player;

-(PXPlayer *)playerAfterPlayer:(PXPlayer *)player;

@end

