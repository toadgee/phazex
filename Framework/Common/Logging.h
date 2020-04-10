//
//  Logging.h
//  phazex
//
//  Created by toddha on 12/8/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

@interface PXLogging : NSObject 
{
}

+ (void)setLoggingGameMessage:(BOOL)on;

+(void)logGameMessage:(NSString *)string; // used from Game.m

+(void)assertThat:(BOOL)assertion because:(NSString *)message;

+(void)raiseError:(NSString *)errorString;
+(void)raiseWarning:(NSString *)warningString;
+(void)raiseErrorInClass:(NSString *)class forFunction:(NSString *)function withMessage:(NSString *)message;

+(void)notImplemented;

@end
