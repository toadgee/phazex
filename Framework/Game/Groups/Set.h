//
//  Set.h
//  phazex
//
//  Created by toddha on 12/18/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import "Group.h"

@interface PXSet : PXGroup
{

}

+(id)setWithCardsRequired:(int)cardsRequired;
-(id)initWithCardsRequired:(int)cardsRequired;

-(BOOL)check;
+(NSArray *)findAllSetsInHand:(PXCardCollection *)hand cardsRequired:(int)cardsRequired cardsToAdd:(int)cardsToAdd;

@end
