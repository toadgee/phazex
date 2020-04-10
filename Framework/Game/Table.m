//
//  Table.m
//  phazex
//
//  Created by toddha on 12/18/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Table.h"
#import "Logging.h"
#import "Group.h"

@interface PXTable (Internals)
-(void)markTableChanged;
@end

@implementation PXTable

+(id)table
{
	return [[[PXTable alloc] init] autorelease];
}

-(id)init
{
	self = [super init];
	if (self)
	{
		_groups = [[NSMutableArray arrayWithCapacity:20] retain];
	}
	
	return self;
}

-(void)dealloc
{
	[_groups release];
	[super dealloc];
}

-(int)count
{
	return (int)[_groups count];
}

-(BOOL)canPlayCard:(PXCard *)card byRules:(PXGameRules *)rules
{
	return [self canPlayCard:card byRules:rules outGroup:nil];
}

-(BOOL)canPlayCard:(PXCard *)card byRules:(PXGameRules *)rules outGroup:(PXGroup **)group
{
	for (PXGroup *g in _groups)
    {
		if ([g checkWithCard:card byRules:rules])
		{
			if (group != nil)
			{
				(*group) = g;
				return YES;
			}
		}
    }

	return NO;
}

-(BOOL)playCard:(PXCard *)card onGroup:(PXGroup *)group byRules:(PXGameRules *)rules
{
	if (![self hasGroup:group])
	{
		[PXLogging raiseError:@"Tried to play card on group somebody thought we had, but we really didn't."];
		return NO;
	}

    if ([group checkWithCard:card byRules:rules])
    {
		[group addCard:card];
		[self markTableChanged];
        return YES;
    }

    return NO;
}

-(void)addGroup:(PXGroup *)group
{
	[_groups addObject:group];
	[self markTableChanged];
}

-(BOOL)hasGroup:(PXGroup *)group
{
	for (PXGroup *g in _groups)
	{
		if (g == group)
			return YES;
	}
	
	return NO;
}

- (NSUInteger)countByEnumeratingWithState:(NSFastEnumerationState *)state objects:(id *)stackbuf count:(NSUInteger)len
{
	return [_groups countByEnumeratingWithState:state objects:stackbuf count:len];
}

-(NSArray *)groups
{
	return [NSArray arrayWithArray:_groups];
}

-(void)markTableChanged
{
	if ([_delegate respondsToSelector:@selector(tableChanged:)])
		[_delegate tableChanged:self];
}

@end
