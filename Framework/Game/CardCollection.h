//
//  CardCollection.h
//  phazex
//
//  Created by toddha on 11/27/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#include "PoolAllocator.h"
#include "Card.h"
@class PXGameRules;
@class PXCardCollection;
@protocol PXCardCollectionDelegate <NSObject>
-(void)cardCollectionChanged:(PXCardCollection *)collection;
@end


@interface PXCardCollection : NSObject
{
	id<PXCardCollectionDelegate> _delegate;
	
	BOOL _fireChanged;
	
	unsigned _count;
	PXCard *_head;
	PXCard *_tail;
}

+(id)cards;
+(id)randomCards:(int)count;
+(id)deck;

@property (readonly) int minimumCardNumber;
@property (readonly) int maximumCardNumber;
@property (readonly) PXCardCollection *allExceptTop;
@property (readwrite, assign) id<PXCardCollectionDelegate> delegate;
@property (readonly) unsigned count;

-(id)copy;
- (instancetype)copyWithPool:(PXMemoryPool *)pool;

-(void)sortByColor;
-(void)sortByNumber;
-(void)sortReverseByColor;
-(void)sortReverseByNumber;

-(void)shuffle;

-(PXCard *)cardAtIndex:(unsigned)index;
-(PXCard *)lastCard;

-(int)countCardWithNumber:(int)number;
-(BOOL)hasCardWithNumber:(int)number;
-(PXCard *)cardWithNumber:(int)number;

-(int)countCardWithColor:(int)color notWithNumber:(int)number;
-(BOOL)hasCardWithColor:(int)color notWithNumber:(int)number;
-(PXCard *)cardWithColor:(int)color notWithNumber:(int)number;

-(PXCard *)cardWithNumber:(int)number withColor:(int)color;

-(PXCard *)cardWithId:(int)cardId;
-(void)removeCardWithId:(int)cardId;

-(int)pointsCountByRules:(PXGameRules *)rules;

- (void)replaceCard:(PXCard *)oldCard withCard:(PXCard *)newCard;
-(void)addCardCopy:(PXCard *)card;
-(void)insertCardCopy:(PXCard *)card atIndex:(unsigned)index;

// NSArray methods
-(void)addCard:(PXCard *)card;
-(void)insertCard:(PXCard *)card atIndex:(unsigned)index;
-(void)removeCard:(PXCard *)card;
-(BOOL)containsCardWithId:(int)cardId;
-(void)addCards:(PXCardCollection *)cards; // the cards collection passed in is now empty after this
-(void)clear;
-(void)removeCardAtIndex:(unsigned)index;


- (void)debug_validateRetainCount;
- (int)debug_count;
@end
