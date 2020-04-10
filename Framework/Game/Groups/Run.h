//
//  Run.h
//  phazex
//
//  Created by toddha on 12/18/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import "Group.h"

@class PXGameRules;

@interface PXRun : PXGroup
{

}

+(id)runWithCardsRequired:(int)cardsRequired;
-(id)initWithCardsRequired:(int)cardsRequired;

-(BOOL)check;
-(BOOL)checkWithCard:(PXCard *)card byRules:(PXGameRules *)rules;

+(NSArray *)findAllRunsInHand:(PXCardCollection *)hand cardsRequired:(int)cardsRequired cardsToAdd:(int)cardsToAdd;

@end
