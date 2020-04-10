//
//  PhazeRules.h
//  phazex
//
//  Created by toddha on 12/19/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

@class PXPhazeRule;
@interface PXPhazeRules : NSObject
{
	NSArray<PXPhazeRule *> *_phazeRules;
}

+(id)defaultRules;
-(id)initWithPhazeRules:(NSArray<PXPhazeRule *> *)phazeRules;

-(int)count;
-(PXPhazeRule *)phazeRuleNumber:(int)number;

@end
