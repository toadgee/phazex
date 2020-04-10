//
//  Scoreboard.m
//  phazex
//
//  Created by toddha on 12/23/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Scoreboard.h"
#import "SessionScore.h"

@interface PXScoreboard ()
@property (readwrite, strong, nonatomic) PXSessionScore *currentSession;
@end

@implementation PXScoreboard

-(id)init
{
	self = [super init];
	
	if (self)
	{
		_sessions = [[NSMutableArray alloc] initWithCapacity:50];
	}
	
	return self;
}

-(void)dealloc
{
	[_sessions release];
	[_currentSession release];
	
	[super dealloc];
}

-(int)count
{
	return (int)[_sessions count];
}

-(PXSessionScore *)addSessionWherePlayerDealt:(BOOL)dealt forPhazeNumber:(int)phazeNumber
{
	[self setCurrentSession:[PXSessionScore sessionForPlayerWhoDealt:dealt forPhazeNumber:phazeNumber]];
	[_sessions addObject:[self currentSession]];
	return [self currentSession];
}

-(int)timesSkippedOtherPlayers
{
	int retval = 0;
	
	for (PXSessionScore *session in _sessions)
	{
		retval += [[session playerIdsSkipped] count];
	}
	
	return retval;
}

-(int)timesSkippedByOtherPlayers
{
	int retval = 0;
	
	for (PXSessionScore *session in _sessions)
	{
		retval += [[session playerWasSkippedByIds] count];
	}
	
	return retval;
}

-(int)points
{
	int retval = 0;
	
	for (PXSessionScore *session in _sessions)
	{
		retval += [session points];
	}
	
	return retval;
}

-(int)completedPhazeNumber
{
	int retval = 0;
	
	for (PXSessionScore *session in _sessions)
	{
		if ([session madePhaze])
		{
			retval++;
		}
	}
	
	return retval;
}

@end
