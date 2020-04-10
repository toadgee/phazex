//
//  Flush.h
//  phazex
//
//  Created by toddha on 12/18/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import "Group.h"
@class PXCardCollection;

@interface PXFlush : PXGroup
{
}

+(id)flushWithCardsRequired:(int)cardsRequired;
-(id)initWithCardsRequired:(int)cardsRequired;
+(NSArray *)findAllFlushesInHand:(PXCardCollection *)hand cardsRequired:(int)cardsRequired cardsToAdd:(int)cardsToAdd;


@end
