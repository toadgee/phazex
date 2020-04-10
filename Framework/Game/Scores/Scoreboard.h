//
//  Scoreboard.h
//  phazex
//
//  Created by toddha on 12/23/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

@class PXSessionScore;
@interface PXScoreboard : NSObject
{
	NSMutableArray<PXSessionScore *> *_sessions;
	PXSessionScore *_currentSession;
}

@property (readonly, strong, nonatomic) PXSessionScore *currentSession;
@property (readonly, assign, nonatomic) int count;
@property (readonly, assign, nonatomic) int timesSkippedOtherPlayers;
@property (readonly, assign, nonatomic) int timesSkippedByOtherPlayers;
@property (readonly, assign, nonatomic) int points;
@property (readonly, assign, nonatomic) int completedPhazeNumber;


-(PXSessionScore *)addSessionWherePlayerDealt:(BOOL)dealt forPhazeNumber:(int)phazeNumber;


@end
