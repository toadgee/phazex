//
//  ObservableMutableArray.m
//  phazex
//
//  Created by toddha on 11/14/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "ObservableMutableArray.h"

@implementation PXObservableMutableArray

@synthesize delegate = _delegate;

- (id)initWithCapacity:(int)capacity
{
	if ((self = [super init]))
	{
		_cards = [[NSMutableArray arrayWithCapacity:capacity] retain];
	}
	
	return self;
}

+ (id)arrayWithCapacity:(int)capacity
{
	return [[[PXObservableMutableArray alloc] initWithCapacity:capacity] autorelease];
}

-(void)dealloc
{
	[_cards release];
	[super dealloc];
}

-(int)count
{
	return (int)[_cards count];
}

- (NSUInteger)countByEnumeratingWithState:(NSFastEnumerationState *)state objects:(id *)stackbuf count:(NSUInteger)len
{
	return [_cards countByEnumeratingWithState:state objects:stackbuf count:len];
}

- (void)collectionChanged
{
	if ([_delegate respondsToSelector:@selector(collectionChanged)])
		[_delegate performSelector:@selector(collectionChanged)];
}

-(id)objectAtIndex:(int)index
{
	return [_cards objectAtIndex:index];
}

-(void)insertObject:(id)object atIndex:(int)index
{
	[_cards insertObject:object atIndex:index];
	[self collectionChanged];
}

-(void)addObject:(id)object
{
	[_cards addObject:object];
	[self collectionChanged];
}

-(void)addObjectsFromArray:(NSArray *)otherArray
{
	[_cards addObjectsFromArray:otherArray];
	[self collectionChanged];
}

-(void)getObjects:(id *)aBuffer
{
	[_cards getObjects:aBuffer];
}

-(void)removeObject:(id)object
{
	[_cards removeObject:object];
	[self collectionChanged];
}

-(void)removeObjectAtIndex:(int)index
{
	[_cards removeObjectAtIndex:index];
	[self collectionChanged];
}

-(void)clear
{
	[_cards removeAllObjects];
	[self collectionChanged];
}

-(BOOL)containsObject:(id)object
{
	return [_cards containsObject:object];
}

@end
