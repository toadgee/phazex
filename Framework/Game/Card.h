#pragma once

#import "PoolAllocator.h"

struct PXCardStruct
{
	int _retainCount;
	void* _holder;
	
	struct PXCardStruct *_previous;
	struct PXCardStruct *_next;
	
	int _cardId;
	int _number;
	int _color;
	BOOL _wild;
};

typedef struct PXCardStruct PXCard;

#define kCardInvalidId (-1)
#define kCardMinimumNumberValue 0
#define kCardMaximumNumberValue 13
#define kCardWildNumber 0
#define kCardSkipNumber 13
#define kCardSkipColor 3
#define kCardMinimumNumber 1
#define kCardMaximumNumber 12
#define kCardMinimumColor 0
#define kCardMaximumColor 3

#define CardId(c) (c->_cardId)
#define CardNumber(c) (c->_number)
#define CardColor(c) (c->_color)
#define CardIsWild(c) (c->_wild)
#define CardNext(c) (c->_next)
#define CardPrevious(c) (c->_previous)
#define CardIsNumbered(c) (CardNumber(c) >= kCardMinimumNumber && CardNumber(c) <= kCardMaximumNumber)
#define CardIsSkip(c) (c->_number == kCardSkipNumber)
#define CardIsUnassignedWild(c) (CardNumber(c) == kCardWildNumber)
#define CardIsInvalidId(c) (CardId(c) == kCardInvalidId)
#define CardIsMinimumOrMaximum(c) (CardNumber(c) == kCardMinimumNumber || CardNumber(c) == kCardMaximumNumber)

PXCard* RetainCard(PXCard* card);
void ReleaseCard(PXCard* card);

NSString *CardDescription(PXCard *card);
PXCard *CreateCard(PXMemoryPool *memoryPool, int number, int color, int cardId);
PXCard *CreateCardWithWild(PXMemoryPool *memoryPool, int number, int color, int cardId, BOOL wild);
PXCard *CopyCard(PXMemoryPool *memoryPool, PXCard *card);
void CardSetNumber(PXCard *card, int number);
void CardSetNext(PXCard *card, PXCard *next);
void CardSetPrevious(PXCard *card, PXCard *previous);

PXMemoryPool *GetUICardPool();
uint64_t CardsLiving();
void PrintAllUsedCards();
