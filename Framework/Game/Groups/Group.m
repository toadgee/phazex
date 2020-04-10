//
//  Group.m
//  phazex
//
//  Created by toddha on 12/18/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Group.h"
#import "Logging.h"

@implementation PXGroup

@synthesize type = _type;
@synthesize cardsRequired = _cardsRequired;
@synthesize groupId = _groupId;

-(id)initWithCardsRequired:(int)cardsRequired
{
	self = [super init];
	
	if (self)
	{
		_cardsRequired = cardsRequired;
		_groupId = -1;
	}
	
	return self;
}

-(void)dealloc
{
	[super dealloc];
}

-(id)copy
{
	[PXLogging raiseErrorInClass:@"Group" forFunction:@"copy" withMessage:[NSString stringWithFormat:@"%d : Deriving classes need to implement.", _type]];
	return nil;
}

-(BOOL)check
{
	[PXLogging raiseErrorInClass:@"Group" forFunction:@"check" withMessage:[NSString stringWithFormat:@"%d : Deriving classes need to implement.", _type]];
	return NO;
}

-(BOOL)checkWithCard:(PXCard *)card byRules:(PXGameRules *)rules
{
	[PXLogging raiseErrorInClass:@"Group" forFunction:@"checkWithCard:byRules:" withMessage:[NSString stringWithFormat:@"%d : Deriving classes need to implement.", _type]];
	return NO;
}

-(BOOL)guessWildCard:(PXCard *)card byRules:(PXGameRules *)rules
{
	[PXLogging raiseErrorInClass:@"Group" forFunction:@"guessWildCard:byRules:" withMessage:[NSString stringWithFormat:@"%d : Deriving classes need to implement.", _type]];
	return NO;
}

@end
