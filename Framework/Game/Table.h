//
//  Table.h
//  phazex
//
//  Created by toddha on 12/18/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#include "Card.h"

@class PXGroup;
@class PXGameRules;

@interface PXTable : NSObject <NSFastEnumeration>
{
	NSMutableArray *_groups;
	id _delegate;
}

@property (readonly) int count;
@property (readonly) NSArray *groups;

+(id)table;

-(BOOL)canPlayCard:(PXCard*)card byRules:(PXGameRules *)rules;
-(BOOL)canPlayCard:(PXCard *)card byRules:(PXGameRules *)rules outGroup:(PXGroup **)group;
-(BOOL)playCard:(PXCard *)card onGroup:(PXGroup *)group byRules:(PXGameRules *)rules;
-(void)addGroup:(PXGroup *)group;
-(BOOL)hasGroup:(PXGroup *)group;

@end

@interface NSObject (NSObject_PXTableDelegateMethods)
-(void)tableChanged:(PXTable *)table;
@end

