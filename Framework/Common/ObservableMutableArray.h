//
//  ObservableMutableArray.h
//  phazex
//
//  Created by toddha on 11/14/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

@interface PXObservableMutableArray : NSObject <NSFastEnumeration>
{
	NSMutableArray *_cards;
	id _delegate;
}

@property(assign, readwrite) id delegate;

+ (id)arrayWithCapacity:(int)capacity;
- (id)initWithCapacity:(int)capacity;

-(int)count;
-(id)objectAtIndex:(int)index;
-(void)insertObject:(id)object atIndex:(int)index;
-(void)addObject:(id)object;
-(void)addObjectsFromArray:(NSArray *)otherArray;
-(void)removeObject:(id)object;
-(void)removeObjectAtIndex:(int)index;
-(void)clear;
-(BOOL)containsObject:(id)object;
-(void)collectionChanged;

@end

/*@interface NSObject (ObservableMutableArrayDelegateMethods)
-(void)collectionChanged:(ObservableMutableArray *)array;
-(void)collection:(ObservableMutableArray *)array addedObject:(id)object atIndex:(int)index;
-(void)collection:(ObservableMutableArray *)array removedObject:(id)object atIndex:(int)index;
-(void)collectionCleared:(ObservableMutableArray *)array;
@end*/
