//
//  Logging.m
//  phazex
//
//  Created by toddha on 12/8/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Logging.h"

@implementation PXLogging

static BOOL s_loggingGameMessages = NO;
+ (void)setLoggingGameMessage:(BOOL)on
{
	s_loggingGameMessages = on;
}

+(void)logGameMessage:(NSString *)string
{
	if (s_loggingGameMessages)
	{
		NSLog(@"%@", string);
	}
}

+(void)assertThat:(BOOL)assertion because:(NSString *)message
{
	if (assertion) return;
	
	[PXLogging raiseError:message];
}

+(void)raiseError:(NSString *)errorString
{
	NSLog(@"ERROR : %@", errorString);
	
	NSException *exception = [[NSException alloc] initWithName:@"PhazeX internal exception" reason:errorString userInfo:nil];
	[exception raise];
}

+(void)raiseWarning:(NSString *)warningString
{
	NSLog(@"WARNING : %@", warningString);
	
	//NSException *exception = [[NSException alloc] initWithName:@"PhazeX internal exception" reason:warningString userInfo:nil];
	//[exception raise];
}

+(void)raiseErrorInClass:(NSString *)class forFunction:(NSString *)function withMessage:(NSString *)message
{
	[PXLogging raiseError:[NSString stringWithFormat:@"%@::%@ : %@", class, function, message]];
}

+(void)notImplemented
{
	[self raiseError:@"function is not implemented!"];
}

@end
