//
//  CardGroupMatch.h
//  phazex
//
//  Created by toddha on 12/18/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#include "Card.h"

@class PXGroup;

@interface PXCardGroupMatch : NSObject
{
	PXCard * _card;
	PXGroup *_group;
}

@property (readonly) PXCard *card;
@property (readonly) PXGroup *group;

+(id)matchWithCard:(PXCard *)card withGroup:(PXGroup *)group;
-(id)initWithCard:(PXCard *)card withGroup:(PXGroup *)group;

@end
