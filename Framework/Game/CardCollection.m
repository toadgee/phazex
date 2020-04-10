//
//  CardCollection.m
//  phazex
//
//  Created by toddha on 11/27/08.
//  Copyright 2008 toadgee.com. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "CardCollection.h"
#import "Card.h"
#import "Logging.h"

@interface PXCardCollection (Internals)
- (void)collectionChanged;
- (PXCard *)head;
- (PXCard *)tail;
- (void)specialClear;
- (void)internalSort:(BOOL)byNumber reversed:(BOOL)reverse; // Number->NO for color
@end

@implementation PXCardCollection
@synthesize count = _count;
@synthesize delegate = _delegate;

+(id)cards
{
	return [[PXCardCollection new] autorelease];
}

+(id)randomCards:(int)count
{
	PXCardCollection *cards = [[PXCardCollection new] autorelease];
	for (int i = 0; i < count; i++)
	{
		PXCard *card = CreateCard(NULL, (random() % 14), (random() % 4), kCardInvalidId);
		[cards addCard:card];
		ReleaseCard(card);
	}
	return cards;
}

+ (id)deck
{
	PXCardCollection *cards = [[PXCardCollection new] autorelease];
	
	// TODO : Use rules here
	int n, c;
	int i = 0;
	for (n = kCardMinimumNumberValue; n <= kCardMaximumNumberValue; n++)
	{
		for (c = kCardMinimumColor; c <= kCardMaximumColor; c++)
		{
			int count;
			if ((n == kCardSkipNumber) && (c == kCardSkipColor))
			{
				count = 4;
			}
			else if (n == kCardSkipNumber)
			{
				count = 0;
			}
			else
			{
				count = 2;
			}
			
			int j;
			for (j = 0; j < count; j++)
			{
				PXCard *card = CreateCard(NULL, n, c, i);
				[cards addCard:card];
				ReleaseCard(card);
				i++;
			}
		}
	}
	
	[cards shuffle];
	return cards;
}

-(id)init
{
	self = [super init];
	if (self)
	{
		_fireChanged = YES;
		_count = 0;
		_head = NULL;
		_tail = NULL;
	}
	
	return self;
}

-(void)dealloc
{
	_delegate = nil;
	
	// we know we always have 2 references on every card, so let's go kill those
	if (_count > 0)
	{
		[self removeReferenceFromAllCards];
		[self removeReferenceFromAllCards];
	}
	
	[super dealloc];
}

- (PXCard *)head
{
	return _head;
}

-(PXCard *)tail
{
	return _tail;
}

-(instancetype)copy
{
	return [self copyWithPool:NULL];
}

- (instancetype)copyWithPool:(PXMemoryPool *)pool
{
	PXCardCollection *coll = [PXCardCollection new];
	PXCard *card = _head;
	while (card != NULL)
	{
		[coll addCardCopy:card pool:pool];
		card = CardNext(card);
	}
	
	return coll;
}

-(void)removeCardWithId:(int)cardId
{
	PXCard *card = _head;
	while (card != NULL)
	{
		if (CardId(card) == cardId)
		{
			[self removeCard:card];
			break;
		}
		
		card = CardNext(card);
	}
}

- (void)internalSort:(BOOL)byNumber reversed:(BOOL)reverse
{
	NSMutableArray<NSValue *> *array = [[NSMutableArray alloc] initWithCapacity:_count];
	
	[self addReferenceToAllCards];
	// add reference to all cards (and add object to array)
	{
		PXCard *c = _head;
		while (c != NULL)
		{
			[array addObject:[NSValue valueWithPointer:c]];
			c = CardNext(c);
		}
	}
	
	_fireChanged = NO;
	[self specialClear];
	
	NSArray<NSValue *> *sorted = [array sortedArrayUsingComparator:^(id obj1, id obj2) {
		NSValue *v1 = (NSValue *)obj1;
		NSValue *v2 = (NSValue *)obj2;
		PXCard *c1 = (PXCard *)[v1 pointerValue];
		PXCard *c2 = (PXCard *)[v2 pointerValue];
		
		int val1 = 0;
		int val2 = 0;
		if (byNumber)
		{
			if (reverse)
			{
				val1 = CardNumber(c2);
				val2 = CardNumber(c1);
			}
			else
			{
				val1 = CardNumber(c1);
				val2 = CardNumber(c2);
			}
		}
		else
		{
			if (reverse)
			{
				val1 = CardColor(c2);
				val2 = CardColor(c1);
			}
			else
			{
				val1 = CardColor(c1);
				val2 = CardColor(c2);
			}
		}
		
		if (val1 == val2)
		{
			return NSOrderedSame;
		}
		else if (val1 > val2)
		{
			return NSOrderedDescending;
		}
		else
		{
			return NSOrderedAscending;
		}

	}];
	
	for (NSValue *cardValue in sorted)
	{
		PXCard *card = (PXCard *)[cardValue pointerValue];
		CardSetNext(card, NULL);
		CardSetPrevious(card, NULL);
		[self addCard:card];
	}
	
	[array release];
	
	[self removeReferenceFromAllCards];
	
	_fireChanged = YES;
	[self collectionChanged];
}

- (void)sortByColor
{
	[self internalSort:NO reversed:NO];
}

- (void)sortByNumber
{
	[self internalSort:YES reversed:NO];
}

- (void)sortReverseByColor
{
	[self internalSort:NO reversed:YES];
}

- (void)sortReverseByNumber
{
	[self internalSort:YES reversed:YES];
}

- (void)shuffle
{
	unsigned unshuffledSize = _count;
	while (unshuffledSize != 0)
	{
		unsigned cardLocation = (random() % unshuffledSize);
		PXCard *c = RetainCard([self cardAtIndex:cardLocation]);
		[self removeCardAtIndex:cardLocation];
		[self addCard:c];
		ReleaseCard(c);
		unshuffledSize--;
	}
	
	[self collectionChanged];
}

- (PXCardCollection *)allExceptTop
{
	PXCardCollection *coll = [PXCardCollection cards];
	
	while (_count > 1)
	{
		PXCard *card = RetainCard([self cardAtIndex:0]);
		[self removeCardAtIndex:0];
		[coll addCard:card];
		ReleaseCard(card);
	}
	
	return coll;
}

-(int)minimumCardNumber
{
	int min = kCardMaximumNumberValue;
	PXCard *card = _head;
	while (card != NULL)
	{
		if (CardNumber(card) < min)
		{
			min = CardNumber(card);
		}
		
		card = CardNext(card);
	}
	
	return min;
}

-(int)maximumCardNumber
{
	int max = kCardMinimumNumberValue - 1;
	PXCard *card = _head;
	while (card != NULL)
	{
		if (CardNumber(card) > max)
		{
			max = CardNumber(card);
		}
		
		card = CardNext(card);
	}
	
	return max;
}

-(int)countCardWithNumber:(int)number
{
	int count = 0;
	PXCard *card = _head;
	while (card != NULL)
	{
		if (CardNumber(card) == number)
		{
			count++;
		}
		
		card = CardNext(card);
	}
	
	return count;
}

-(BOOL)hasCardWithNumber:(int)number
{
	BOOL hasCard = NO;
	PXCard *card = _head;
	while (card != NULL)
	{
		if (CardNumber(card) == number)
		{
			hasCard = YES;
			break;
		}
		
		card = CardNext(card);
	}
	
	return hasCard;
}

-(PXCard *)cardWithNumber:(int)number
{
	PXCard *card = _head;
	while (card != NULL)
	{
		if (CardNumber(card) == number)
		{
			return card;
		}
		
		card = CardNext(card);
	}
	
	return nil;
}

-(int)countCardWithColor:(int)color notWithNumber:(int)number
{
	int count = 0;
	PXCard *card = _head;
	while (card != NULL)
	{
		if (CardColor(card) == color)
		{
			if (CardNumber(card) != number)
			{
				count++;
			}
		}
		
		card = CardNext(card);
	}
	
	return count;
}

-(BOOL)hasCardWithColor:(int)color notWithNumber:(int)number
{
	BOOL hasCard = NO;
	PXCard *card = _head;
	while (card != NULL)
	{
		if (CardColor(card) == color)
		{
			if (CardNumber(card) != number)
			{
				hasCard = YES;
				break;
			}
		}
		
		card = CardNext(card);
	}
	
	return hasCard;
}

-(PXCard *)cardWithColor:(int)color notWithNumber:(int)number
{
	PXCard *card = _head;
	while (card != NULL)
	{
		if (CardColor(card) == color && CardNumber(card) != number)
		{
			return card;
		}
		
		card = CardNext(card);
	}
	
	return nil;
}

-(PXCard *)cardWithNumber:(int)number withColor:(int)color
{
	PXCard *card = _head;
	while (card != NULL)
	{
		if (CardColor(card) == color && CardNumber(card) == number)
		{
			return card;
		}
		
		card = CardNext(card);
	}
	
	return nil;
}

-(PXCard *)cardWithId:(int)cardId
{
	PXCard *card = _head;
	while (card != NULL)
	{
		if (CardId(card) == cardId)
		{
			return card;
		}
		
		card = CardNext(card);
	}
	
	return nil;
}

- (void)replaceCard:(PXCard *)oldCard withCard:(PXCard *)newCard
{
#if DEBUG
	{
		BOOL found = NO;
		for (int i = 0; i < _count && !found; ++i)
		{
			PXCard *card = [self cardAtIndex:i];
			if (oldCard == card)
			{
				found = YES;
			}
		}
		
		if (!found)
		{
			[PXLogging raiseError:@"Cannot remove a card not in the list"];
		}
		
		if (CardNext(newCard) != NULL)
		{
			[PXLogging raiseError:@"Cannot add new card that has a next pointer"];
		}
		
		if (CardPrevious(newCard) != NULL)
		{
			[PXLogging raiseError:@"Cannot add new card that has a previous pointer"];
		}
	}
#endif
	
	PXCard *prev = CardPrevious(oldCard);
	PXCard *next = CardNext(oldCard);
	RetainCard(oldCard);
	CardSetPrevious(newCard, prev);
	CardSetNext(newCard, next);
	if (prev != NULL)
	{
		CardSetNext(prev, newCard);
	}
	
	if (next != NULL)
	{
		CardSetPrevious(next, newCard);
	}
	
	if (_head == oldCard)
	{
		ReleaseCard(_head);
		_head = RetainCard(newCard);
	}
	
	if (_tail == oldCard)
	{
		ReleaseCard(_tail);
		_tail = RetainCard(newCard);
	}
	
	CardSetNext(oldCard, NULL);
	CardSetPrevious(oldCard, NULL);
	ReleaseCard(oldCard);
}

-(void)addCardCopy:(PXCard *)card
{
	return [self addCardCopy:card pool:NULL];
}

-(void)addCardCopy:(PXCard *)card pool:(PXMemoryPool *)pool
{
	PXCard *copy = CopyCard(pool, card);
	[self addCard:copy];
	ReleaseCard(copy);
}

#pragma mark -
#pragma mark NSArray type methods

-(void)addCard:(PXCard *)card
{
#if DEBUG
	{
		// must not be nil
		if (card == NULL)
		{
			[PXLogging raiseError:@"Trying to add a nil card!"];
			return;
		}
	
		// must not already be in list
		PXCard *c = _head;
		while (c != NULL)
		{
			if (c == card)
				[PXLogging raiseError:@"Trying to add a card already in the list!"];
			c = CardNext(c);
		}
	
		// card pointers must be nil!
		if (CardNext(card) != NULL)
			[PXLogging raiseError:@"Trying to add a card that has a next pointer!"];
	
		if (CardPrevious(card) != NULL)
			[PXLogging raiseError:@"Trying to add a card that has a previous pointer!"];
	}
#endif
	
	if (_head == NULL)
	{
		_head = RetainCard(card);
		_tail = RetainCard(card);
	}
	else 
	{
		CardSetNext(_tail, card);
		CardSetPrevious(card, _tail);
		ReleaseCard(_tail);
		_tail = RetainCard(card);
	}
	
	_count++;
	[self collectionChanged];
}

-(void)insertCardCopy:(PXCard *)card atIndex:(unsigned)index
{
	PXCard *copy = CopyCard(NULL, card);
	[self insertCard:copy atIndex:index];
	ReleaseCard(copy);
}

-(void)insertCard:(PXCard *)card atIndex:(unsigned)index
{
#if DEBUG
	{
		// must not be nil
		if (card == NULL)
		{
			[PXLogging raiseError:@"Trying to insert a nil card!"];
			return;
		}
		
		// must be in range
		if (index >= _count)
			[PXLogging raiseError:@"Trying to insert a card at an index which doesn't exist!"];
		
		// must not be already be in list
		PXCard *c = _head;
		while (c != NULL)
		{
			if (c == card)
				[PXLogging raiseError:@"Trying to insert a card already in the list!"];
			
			c = CardNext(c);
		}
		
		// card pointers must be nil!
		if (CardNext(card) != NULL)
			[PXLogging raiseError:@"Trying to insert a card that has a next pointer!"];
	
		if (CardPrevious(card) != NULL)
			[PXLogging raiseError:@"Trying to insert a card that has a previous pointer!"];
	}
#endif
	
	
	if (_count == 0 && index == 0)
	{
		_head = RetainCard(card);
		_tail = RetainCard(card);
	}
	else if (index == 0)
	{
		CardSetPrevious(_head, card);
		CardSetNext(card, _head);
		RetainCard(card);
		ReleaseCard(_head);
		_head = card;
	}
	else if (index == _count)
	{
		CardSetNext(_tail, card);
		CardSetPrevious(card, _tail);
		RetainCard(card);
		ReleaseCard(_tail);
		_tail = card;
	}
	else
	{
		PXCard *c = _head;
		for (unsigned i = 0; i < index; i++)
		{
			assert(c != NULL);
			c = CardNext(c);
		}
		
		assert(c != NULL);
		CardSetNext(CardPrevious(c), card); // c.previous.next = card;
		CardSetPrevious(card, CardPrevious(c)); // card.previous = c.previous
		CardSetPrevious(c, card); // c.previous = card;
		CardSetNext(card, c); // card.next = c;
	}
	
	_count++;
	[self collectionChanged];
}

-(void)removeCard:(PXCard *)card
{
#if DEBUG
	{
		// must not be nil
		if (card == nil)
			[PXLogging raiseError:@"Trying to remove a nil card!"];

		// must be already be in list
		PXCard *c = _head;
		BOOL found = NO;
		while (c != nil)
		{
			if (c == card)
			{
				if (found)
					[PXLogging raiseError:@"Trying to remove card, found it twice!"];
				found = YES;
			}
			
			c = CardNext(c);
		}
		
		if (!found)
			[PXLogging raiseError:@"Trying to remove card, didn't find it!"];
	}
#endif

	BOOL removed = NO;
	int index = 0;
	PXCard *c = _head;
	while (c != NULL)
	{
		if (c == card)
		{
			[self removeCardAtIndex:index];
			removed = YES;
			break;
		}
		
		index++;
		c = CardNext(c);
	}
	
	assert(removed);
}

-(BOOL)containsCardWithId:(int)cardId
{
#if DEBUG
	{
		// must not be nil
		if (cardId == kCardInvalidId)
			[PXLogging raiseError:@"Trying to check for a nil card!"];
	}
#endif
	
	PXCard *c = _head;
	while (c != nil)
	{
		if (CardId(c) == cardId)
			return YES;
		
		c = CardNext(c);
	}
	
	return NO;
}

-(PXCard *)lastCard
{
	return _tail;
}

-(PXCard *)cardAtIndex:(unsigned)index
{
#if DEBUG
	{
		// must be in range
		if (index >= _count)
			[PXLogging raiseError:@"Trying to remove a card at an index which doesn't exist!"];
	}
	
	if (_head == NULL && _tail != NULL)
	{
		[PXLogging raiseError:@"Head and tail must match!"];
	}
	
#endif
	
	PXCard *c = _head;
	for (unsigned i = 0; i < index && c != NULL; i++)
	{
		c = CardNext(c);
	}
	
	return c;
}

-(void)removeCardAtIndex:(unsigned)index
{
#if DEBUG
	{
		// must not be in range
		if (index >= _count)
			[PXLogging raiseError:@"Trying to remove a card at an index which doesn't exist!"];
	}
#endif

	PXCard *c = NULL;
	if (index == _count - 1)
	{
		c = RetainCard(_tail);
		ReleaseCard(_tail);
		if (CardPrevious(_tail) != NULL)
		{
			_tail = RetainCard(CardPrevious(_tail));
			CardSetNext(_tail, NULL);
		}
		else
		{
			_tail = NULL;
		}
	}
	else if (index == 0)
	{
		c = RetainCard(_head);
		ReleaseCard(_head);
		if (CardNext(_head) != NULL)
		{
			_head = RetainCard(CardNext(_head));
			CardSetPrevious(_head, NULL);
		}
		else
		{
			_head = NULL;
		}
	}
	else
	{
		c = _head;
		for (unsigned i = 0; i < index; i++)
		{
			c = CardNext(c);
		}
		
		RetainCard(c);
		CardSetNext(CardPrevious(c), CardNext(c));
		CardSetPrevious(CardNext(c), CardPrevious(c));
	}
	
	CardSetNext(c, NULL);
	CardSetPrevious(c, NULL);
	ReleaseCard(c);
	
	if (_tail == NULL && _head != NULL)
	{
		ReleaseCard(_head);
		_head = NULL;
	}
	
	if (_head == NULL && _tail != NULL)
	{
		ReleaseCard(_tail);
		_tail = NULL;
	}
	
	_count--;
	[self collectionChanged];
}

-(void)addCards:(PXCardCollection *)cards
{
	if ([cards count] > 0)
	{
		_fireChanged = NO;
		while ([cards count] > 0)
		{
			PXCard *card = [cards cardAtIndex:0];
			RetainCard(card);
			[cards removeCardAtIndex:0];
			[self addCard:card];
			ReleaseCard(card);
		}
		
		_fireChanged = YES;
		[self collectionChanged];
	}
}

- (void)specialClear
{
	ReleaseCard(_tail);
	_tail = NULL;
	
	ReleaseCard(_head);
	_head = NULL;
	
	_count = 0;
}

-(void)clear
{
	if (_count > 0)
	{
		PXCard *c = _head;
		while (c != NULL)
		{
			PXCard *d = CardNext(c);
			CardSetNext(c, NULL);
			CardSetPrevious(c, NULL);
			c = d;
		}

		[self specialClear];
		[self collectionChanged];
	}
}

- (NSString *)description
{
	NSMutableString *s = [NSMutableString stringWithCapacity:300];
	[s appendFormat:@"PXCardCollection with %d cards : ", _count];
	PXCard *card = _head;
	while (card != nil)
	{
		[s appendFormat:@"%@, ", CardDescription(card)];
		card = CardNext(card);
	}
	return s;
}

-(int)pointsCountByRules:(PXGameRules *)rules
{
	int points = 0;
	PXCard *card = _head;
	while (card != NULL)
	{
		if (CardIsWild(card))
			points += 50;
		else if (CardIsSkip(card))
			points += 25;
		else if (CardIsNumbered(card))
			points += CardNumber(card);
		card = CardNext(card);
	}
	
	return points;
}

- (void)collectionChanged
{
	
#if DEBUG
	if (_head != NULL)
	{
		[PXLogging assertThat:(CardPrevious(_head) == NULL) because:@"Head previous must be nil!"];
	}
	if (_tail != NULL)
	{
		[PXLogging assertThat:(CardNext(_tail) == nil) because:@"Tail next must be nil!"];
	}
	
	if (_head != NULL)
	{
		if (_head == _tail)
		{
			[PXLogging assertThat:(CardNext(_head) == nil) because:@"Head next must be nil!"];
			[PXLogging assertThat:(CardPrevious(_tail) == nil) because:@"Tail previous must be nil!"];
		}
		else
		{
			assert(_tail != NULL);
			[PXLogging assertThat:(CardNext(_head) != nil) because:@"Head next must NOT be nil!"];
			[PXLogging assertThat:(CardPrevious(_tail) != nil) because:@"Tail previous must NOT be nil!"];
		}
	}
	
	unsigned actual = 0;
	PXCard *c = _head;
	while (c != nil)
	{
		actual++;
		c = CardNext(c);
	}
	
	[PXLogging assertThat:(actual == _count) because:[NSString stringWithFormat:@"Actual %u must match expected %u", actual, _count]];
	
#endif

	
	if (!_fireChanged) return;
	
	if ([_delegate respondsToSelector:@selector(cardCollectionChanged:)])
		[_delegate cardCollectionChanged:self];
}

- (void)addReferenceToAllCards
{
	PXCard *c = _head;
	while (c != NULL)
	{
		RetainCard(c);
		c = CardNext(c);
	}
}

- (void)removeReferenceFromAllCards
{
	PXCard *c = _head;
	while (c != NULL)
	{
		PXCard *next = CardNext(c);
		ReleaseCard(c);
		c = next;
	}
}

- (void)debug_validateRetainCount
{
	for (int i = 0; i < [self count]; ++i)
	{
		PXCard *card = [self cardAtIndex:i];
		assert(card->_retainCount >= 2);
	}
}

- (int)debug_count
{
	int count = 0;
	PXCard *c = _head;
	while (c != NULL)
	{
		count++;
		c = CardNext(c);
	}
	
	assert(count == [self count]);
	return count;
}


@end
