//
//  HiResTimer.h
//  phazex
//
//  Created by toddha on 12/8/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

// all are nanoseconds, divide by 1,000,000 to get milliseconds

@interface PXHiResTimer : NSObject
{
	BOOL _started;
	uint64_t _start; // start time
	uint64_t _total; // if timer is used mulitple times and not reset, this is the total # of nanoseconds run
	uint64_t _starts;
}

@property (readonly) BOOL started;
@property (readonly) uint64_t elapsed; // returns the # elapsed since the timer was started
@property (readonly) uint64_t total;
@property (readonly) uint64_t starts;
@property (readonly) double totalSeconds;
@property (readonly) uint64_t totalMilliseconds;

+(PXHiResTimer *)timer;
+(PXHiResTimer *)startedTimer;

-(void)start;
-(uint64_t)stop; // returns the elapsed for just this call to start

-(void)reset;

@end
