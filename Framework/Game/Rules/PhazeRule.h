//
//  PhazeRule.h
//  phazex
//
//  Created by toddha on 12/18/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#include "Card.h"
@class PXCardCollection;
@class PXGameRules;
@class PXGroup;

@interface PXPhazeRule : NSObject <NSFastEnumeration>
{
	NSArray<PXGroup *> *_groups;
	int _ruleNumber;
}

@property (readonly) int ruleNumber;
@property (readonly) NSArray<PXGroup *> *groups;
@property (readonly) int cardsRequired;
@property (readonly) int count;

+(int)minimumCardsNeededToCompletePhaze:(PXPhazeRule *)rule withHand:(PXCardCollection *)hand byAddingCard:(PXCard *)card byRules:(PXGameRules *)rules;
+(int)minimumCardsNeededToCompletePhaze:(PXPhazeRule *)rule withHand:(PXCardCollection *)hand byRules:(PXGameRules *)rules;

+(NSArray *)completePhaze:(PXPhazeRule *)rule withHand:(PXCardCollection *)hand byRules:(PXGameRules *)rules;
+(NSArray *)completePhaze:(PXPhazeRule *)rule withHand:(PXCardCollection *)hand cardsToAdd:(int)cardsToAdd byRules:(PXGameRules *)rules;
+(NSArray *)completePhaze:(PXPhazeRule *)rule withHand:(PXCardCollection *)hand cardsToAdd:(int)cardsToAdd byRules:(PXGameRules *)rules startingAtGroup:(int)groupPosition;

+(id)phazeRuleWithNumber:(int)ruleNumber groupsInRule:(NSArray<PXGroup *> *)groups;
-(id)initWithNumber:(int)ruleNumber groupsInRule:(NSArray<PXGroup *> *)groups;

-(PXGroup *)groupAt:(int)position;

@end
