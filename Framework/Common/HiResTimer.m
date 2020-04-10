//
//  HiResTimer.m
//  phazex
//
//  Created by toddha on 12/8/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "HiResTimer.h"
#include <mach/mach_time.h>

@implementation PXHiResTimer
@synthesize total = _total;
@synthesize starts = _starts;
@synthesize started = _started;

static inline uint64_t internalElapsed(uint64_t start)
{
	uint64_t end = mach_absolute_time();
	uint64_t diff = end - start;
	
	static mach_timebase_info_data_t info = {0,0};
	if (info.denom == 0)
		mach_timebase_info(&info);
	
	uint64_t elapsedNanoseconds = diff * (info.numer / info.denom);
	return elapsedNanoseconds;
}

+(PXHiResTimer *)timer
{
	return [[[PXHiResTimer alloc] init] autorelease];
}

+(PXHiResTimer *)startedTimer
{
	PXHiResTimer *timer = [PXHiResTimer timer];
	[timer start];
	return timer;
}

-(void)start
{
	_started = YES;
	_start = mach_absolute_time();
	_starts++;
}

-(uint64_t)elapsed
{
	return internalElapsed(_start);
}

-(uint64_t)stop
{
	assert(_started);
	uint64_t elapsed = internalElapsed(_start);
	_total += elapsed;
	_started = NO;
	return elapsed;
}

-(double)totalSeconds
{
	double seconds = ((double)_total / 1000000000.0);
	return seconds;
}

-(uint64_t)totalMilliseconds
{
	return _total / 1000000;
}

-(void)reset
{
	_started = NO;
	_total = 0LL;
	_starts = 0LL;
}


@end
