//
//  SessionScore.m
//  phazex
//
//  Created by toddha on 12/23/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "SessionScore.h"
#import "Player.h"

@implementation PXSessionScore

@synthesize dealt = _dealt;
@synthesize madePhaze = _madePhaze;
@synthesize phazeNumber = _phazeNumber;
@synthesize points = _points;

+(id)sessionForPlayerWhoDealt:(BOOL)dealt forPhazeNumber:(int)phazeNumber
{
	return [[[PXSessionScore alloc] initForPlayerWhoDealt:dealt forPhazeNumber:phazeNumber] autorelease];
}

-(id)initForPlayerWhoDealt:(BOOL)dealt forPhazeNumber:(int)phazeNumber
{
	self = [super init];
	
	if (self)
	{
		_dealt = dealt;
		_phazeNumber = phazeNumber;
		_points = 0;
		_madePhaze = NO;
		
		_playerWasSkippedByIds = [[NSMutableArray arrayWithCapacity:8] retain];
		_playerIdsSkipped = [[NSMutableArray arrayWithCapacity:8] retain];
	}
	
	return self;
}

-(void)dealloc
{
	[_playerWasSkippedByIds release];
	[_playerIdsSkipped release];
	[super dealloc];
}

-(NSArray<NSNumber *> *)playerWasSkippedBy
{
	return [NSArray arrayWithArray:_playerWasSkippedByIds];
}

-(NSArray<NSNumber *> *)playersSkipped
{
	return [NSArray arrayWithArray:_playerIdsSkipped];
}

-(void)skippedByPlayer:(PXPlayer *)player
{
	[_playerWasSkippedByIds addObject:@([player gameUniqueId])];
}

-(void)skippedPlayer:(PXPlayer *)player
{
	[_playerIdsSkipped addObject:@([player gameUniqueId])];
}

@end
