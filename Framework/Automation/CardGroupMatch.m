//
//  CardGroupMatch.m
//  phazex
//
//  Created by toddha on 12/18/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "CardGroupMatch.h"
#import "Card.h"
#import "Group.h"

@implementation PXCardGroupMatch

@synthesize group = _group;
@synthesize card = _card;

+(id)matchWithCard:(PXCard *)card withGroup:(PXGroup *)group
{
	return [[[PXCardGroupMatch alloc] initWithCard:card withGroup:group] autorelease];
}

-(id)initWithCard:(PXCard *)card withGroup:(PXGroup *)group
{
	self = [super init];
	
	if (self)
	{
		_group = [group retain];
		_card = RetainCard(card);
	}
	
	return self;
}

-(void)dealloc
{
	[_group release];
	ReleaseCard(_card);
	[super dealloc];
}

@end
