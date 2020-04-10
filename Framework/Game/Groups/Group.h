//
//  Group.h
//  phazex
//
//  Created by toddha on 12/18/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import "CardCollection.h"
#import "GroupType.h"

@class PXGameRules;

@interface PXGroup : PXCardCollection
{
	enum PXGroupType _type;
	int _cardsRequired;
	int _groupId;
}

@property (readonly) enum PXGroupType type;
@property (readonly) int cardsRequired;
@property (assign) int groupId;

-(id)initWithCardsRequired:(int)cardsRequired;

-(id)copy;

-(BOOL)check;
-(BOOL)checkWithCard:(PXCard *)card byRules:(PXGameRules *)rules;
-(BOOL)guessWildCard:(PXCard *)card byRules:(PXGameRules *)rules;

@end
