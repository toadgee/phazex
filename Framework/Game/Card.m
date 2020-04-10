#import <Foundation/Foundation.h>
#import "Card.h"
#include "PoolAllocator.h"

int64_t g_cardsLiving = 0;
static PXMemoryPool *s_cardPool = NULL; // game thread
static PXMemoryPool *s_UICardPool = NULL;

uint64_t CardsLiving() { return g_cardsLiving; }

#define CardLogging NO
AllocateImplementation(PXCard, AllocateCard, g_cardsLiving, CardLogging, @"CARD");
DeallocateImplementation(PXCard, DeallocateCard, g_cardsLiving, CardLogging, @"CARD");
RetainImplementation(PXCard, RetainCard, CardLogging, @"CARD");
ReleaseImplementation(PXCard, ReleaseCard, CardLogging, @"CARD", DeallocateCard);

PXMemoryPool *GetUICardPool()
{
	if (s_UICardPool == NULL)
	{
		s_UICardPool = (PXMemoryPool *)malloc(sizeof(PXMemoryPool));
		s_UICardPool->_unusedFirst = NULL;
		s_UICardPool->_usedFirst = NULL;
		MarkPoolThread(s_UICardPool);
	}
	
	return s_UICardPool;
}

PXMemoryPool *GetCardPool(PXMemoryPool *explicitPool);
PXMemoryPool *GetCardPool(PXMemoryPool *explicitPool)
{
	if (explicitPool != NULL)
		return explicitPool;
	
	if (s_cardPool == NULL)
	{
		s_cardPool = (PXMemoryPool *)malloc(sizeof(PXMemoryPool));
		s_cardPool->_unusedFirst = NULL;
		s_cardPool->_usedFirst = NULL;
		MarkPoolThread(s_cardPool);
	}
	
	return s_cardPool;
}

PXCard *CreateCard(PXMemoryPool *memoryPool, int number, int color, int cardId)
{
	return CreateCardWithWild(memoryPool, number, color, cardId, (number == 0));
}

PXCard *CreateCardWithWild(PXMemoryPool *memoryPool, int number, int color, int cardId, BOOL wild)
{
	PXCard *card = AllocateCard(GetCardPool(memoryPool));
	card->_cardId = cardId;
	card->_number = number;
	card->_color = color;
	card->_wild = wild;
	card->_previous = NULL;
	card->_next = NULL;
	return card;
}

PXCard *CopyCard(PXMemoryPool *memoryPool, PXCard *card)
{
	PXCard *copy = AllocateCard(GetCardPool(memoryPool));
	copy->_cardId = card->_cardId;
	copy->_number = card->_number;
	copy->_color = card->_color;
	copy->_wild = card->_wild;
	
	copy->_previous = NULL;
	copy->_next = NULL;
	
	return copy;
}

void CardSetNumber(PXCard *card, int number)
{
	card->_number = number;
}

void CardSetNext(PXCard *card, PXCard *next)
{
	if (card->_next != NULL)
	{
		ReleaseCard(card->_next);
	}
	
	card->_next = RetainCard(next);
}

void CardSetPrevious(PXCard *card, PXCard *previous)
{
	if (card->_previous != NULL)
	{
		ReleaseCard(card->_previous);
	}
	
	card->_previous = RetainCard(previous);
}

NSString *CardDescription(PXCard *card)
{
	NSMutableString *str = [NSMutableString stringWithCapacity:14];
	switch (CardColor(card))
	{
		case 0:
		[str appendString:@"Red "];
		break;
		
		case 1:
		[str appendString:@"Yellow "];
		break;
		
		case 2:
		[str appendString:@"Green "];
		break;
		
		case 3:
		[str appendString:@"Blue "];
		break;
	}
	
	switch (CardNumber(card))
	{
		case 0:
		[str appendString:@"Wild"];
		break;
		
		case 13:
		[str appendString:@"Skip"];
		break;
		
		default:
		[str appendFormat:@"%d", CardNumber(card)];
		break;
	}
	
	[str appendFormat:@" (%d)", CardId(card)];
	return str;
}

void PrintAllUsedCards()
{
	PXPoolAllocator* cardAllocator = s_cardPool->_usedFirst;
	while (cardAllocator != NULL)
	{
		PXCard *card = (PXCard *)cardAllocator->_object;
		NSLog(@"Card %p [%d]    %p <-- --> %p", card, card->_retainCount, card->_previous, card->_next);
		cardAllocator = cardAllocator->_next;
	}
}
