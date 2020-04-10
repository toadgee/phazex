//
//  PhazeRules.m
//  phazex
//
//  Created by toddha on 12/19/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "PhazeRules.h"
#import "PhazeRule.h"
#import "Set.h"
#import "Run.h"
#import "Flush.h"

@implementation PXPhazeRules

+(id)defaultRules
{
	NSMutableArray<PXPhazeRule *> *defaultRules = [NSMutableArray arrayWithCapacity:10];
	
	[defaultRules addObject:[PXPhazeRule phazeRuleWithNumber:1 groupsInRule:@[
		[PXSet setWithCardsRequired:3],
		[PXSet setWithCardsRequired:3]]]];
	[defaultRules addObject:[PXPhazeRule phazeRuleWithNumber:2 groupsInRule:@[
		[PXSet setWithCardsRequired:3],
		[PXRun runWithCardsRequired:4]]]];
	[defaultRules addObject:[PXPhazeRule phazeRuleWithNumber:3 groupsInRule:@[
		[PXSet setWithCardsRequired:4],
		[PXRun runWithCardsRequired:4]]]];
	[defaultRules addObject:[PXPhazeRule phazeRuleWithNumber:4 groupsInRule:@[
		[PXRun runWithCardsRequired:7]]]];
	[defaultRules addObject:[PXPhazeRule phazeRuleWithNumber:5 groupsInRule:@[
		[PXRun runWithCardsRequired:8]]]];
	[defaultRules addObject:[PXPhazeRule phazeRuleWithNumber:6 groupsInRule:@[
		[PXRun runWithCardsRequired:9]]]];
	[defaultRules addObject:[PXPhazeRule phazeRuleWithNumber:7 groupsInRule:@[
		[PXSet setWithCardsRequired:4],
		[PXSet setWithCardsRequired:4]]]];
	[defaultRules addObject:[PXPhazeRule phazeRuleWithNumber:8 groupsInRule:@[
		[PXFlush flushWithCardsRequired:7]]]];
	[defaultRules addObject:[PXPhazeRule phazeRuleWithNumber:9 groupsInRule:@[
		[PXSet setWithCardsRequired:5],
		[PXSet setWithCardsRequired:2]]]];
	[defaultRules addObject:[PXPhazeRule phazeRuleWithNumber:10 groupsInRule:@[
		[PXSet setWithCardsRequired:5],
		[PXSet setWithCardsRequired:3]]]];
	
	return [[[PXPhazeRules alloc] initWithPhazeRules:defaultRules] autorelease];
}

-(id)initWithPhazeRules:(NSArray<PXPhazeRule *> *)phazeRules
{
	self = [super init];
	
	if (self)
	{
		_phazeRules = [[NSArray arrayWithArray:phazeRules] retain];
	}
	
	return self;
}

- (void)dealloc
{
	[_phazeRules release];
	[super dealloc];
}

-(int)count
{
	return (int)[_phazeRules count];
}

-(PXPhazeRule *)phazeRuleNumber:(int)number
{
	for (PXPhazeRule *rule in _phazeRules)
	{
		if ([rule ruleNumber] == number)
		{
			return rule;
		}
	}
	
	return nil;
}

@end
