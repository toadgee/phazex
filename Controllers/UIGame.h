//
//  UIGame.h
//  phazex
//
//  Created by toddha on 11/26/16.
//
//

#import "Card.h"
@class PXGame;
@class PXPlayer;
@class PXUIGame;
@class PXGroup;
@class PXCardCollection;

enum PXUIGameAction
{
	PXUIGameAction_None = 0,
	PXUIGameAction_GetDiscard = 1,
	PXUIGameAction_GetDeckCard = 2,
	PXUIGameAction_DiscardCard = 3,
};


@protocol PXUIGameDelegate <NSObject>
- (void)gameReadyForPlay:(PXUIGame *)game;
- (void)game:(PXUIGame *)game updateHandEnabled:(BOOL)enabled;
- (void)game:(PXUIGame *)game updateDiscardEnabled:(BOOL)enabled;
- (void)game:(PXUIGame *)game updateDeckEnabled:(BOOL)enabled;
- (void)game:(PXUIGame *)game updatePlayers:(NSArray<PXPlayer *> *)players;
- (void)game:(PXUIGame *)game updateTableGroups:(NSArray<PXGroup *> *)groups;
- (void)game:(PXUIGame *)game updateDiscard:(PXCardCollection *)discard;
- (void)game:(PXUIGame *)game updatePlayerGroups:(NSArray<PXGroup *> *)groups;
- (void)game:(PXUIGame *)game updateHandCards:(PXCardCollection *)cards;
- (void)gameFinished:(PXUIGame *)game;
@end

@interface PXUIGame : NSObject
{
	NSCondition *_semaphore;
	PXPlayer *_player;
	PXGame *_game;
	id <PXUIGameDelegate> _delegate;
}

@property (readwrite, retain, nonatomic) PXGame *game;
@property (readwrite, retain, nonatomic) PXPlayer *player;

@property (readonly, assign, nonatomic) enum PXGameState state;
@property (readwrite, assign, nonatomic) id<PXUIGameDelegate> delegate;
@property (readonly, assign, nonatomic) enum PXUIGameAction action;

- (void)start;
- (void)getDeckCard;
- (void)getDiscard;
- (void)waitForAction;

- (void)discardCard:(PXCard *)card;
@end
