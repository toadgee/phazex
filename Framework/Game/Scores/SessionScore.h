//
//  SessionScore.h
//  phazex
//
//  Created by toddha on 12/23/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

@class PXPlayer;

@interface PXSessionScore : NSObject
{
	BOOL                        _dealt;
	BOOL                        _madePhaze;
	int                         _phazeNumber;
	int                         _points;
	NSMutableArray<NSNumber *> *_playerWasSkippedByIds; // list of players this player was skipped by
	NSMutableArray<NSNumber *> *_playerIdsSkipped; // list of players this player skipped
}

@property (readonly) BOOL dealt;
@property (readwrite, assign) BOOL madePhaze;
@property (readonly) int phazeNumber;
@property (readwrite, assign) int points;
@property (readonly) NSArray<NSNumber *> *playerWasSkippedByIds;
@property (readonly) NSArray<NSNumber *> *playerIdsSkipped;

+(id)sessionForPlayerWhoDealt:(BOOL)dealt forPhazeNumber:(int)phazeNumber;
-(id)initForPlayerWhoDealt:(BOOL)dealt forPhazeNumber:(int)phazeNumber;

-(void)skippedByPlayer:(PXPlayer *)player;
-(void)skippedPlayer:(PXPlayer *)player;

@end
