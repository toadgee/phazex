//
//  ComputerPlayer.h
//  phazex
//
//  Created by toddha on 12/8/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import "Player.h"
#include "Card.h"

@class PXCardGroupMatch;
@class PXCardTracker;
@class PXGame;

@interface PXComputerPlayer : PXPlayer <PXPlayerDelegate>
{
	PXCardTracker *_tracker;
}

+ (instancetype)playerWithName:(NSString *)name game:(PXGame *)game;
+ (void)setSleepTime:(NSTimeInterval)sleepTime;
+ (NSTimeInterval)sleepTime;

- (instancetype)initWithName:(NSString *)name game:(PXGame *)game;
-(void)sleep;

-(void)phazePlay;
-(BOOL)tablePlay;
-(void)discardPlay;

-(BOOL)determineFirstPlay;
-(PXCard *)determineDiscardPlay;
-(PXCardGroupMatch *)determineTablePlayCards;
-(NSArray *)determinePhazePlay;
-(PXPlayer *)determinePlayerToSkip;

@end
