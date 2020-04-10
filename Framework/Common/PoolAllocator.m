#import <Foundation/Foundation.h>
#include "PoolAllocator.h"

#if DEBUG
#define dassert(x) assert(x)
#else
#define dassert(x)
#endif

#if VALIDATE_ALLOCATORS
#define MarkAllocatorUsed(allocator) allocator->_used = 1
#define MarkAllocatorUnused(allocator) allocator->_used = 0
void CheckAllocationPool(PXPoolAllocator* usedFirst, PXPoolAllocator* unusedFirst);
void CheckAllocationPool(PXPoolAllocator* usedFirst, PXPoolAllocator* unusedFirst)
{
	// this gets REALLY slow for debug builds (like 45x slower)
	static int s_count = 0;
	s_count++;
	if (s_count < 50) return;
	s_count = 0;
	
	// check no unused ones in used list
	{
		PXPoolAllocator* allocator = usedFirst;
		while (allocator != NULL)
		{
			assert(allocator->_used == 1);
			allocator = allocator->_next;
		}
	}

	// check no used ones in unused list
	{
		PXPoolAllocator* allocator = unusedFirst;
		while (allocator != NULL)
		{
			assert(allocator->_used == 0);
			allocator = allocator->_next;
		}
	}
}
#else
#define CheckAllocationPool(used, unused)
#define MarkAllocatorUsed(allocator)
#define MarkAllocatorUnused(allocator)
#endif

PXPoolAllocator* AllocatePoolObject(size_t size, PXMemoryPool *memoryPool)
{
	dassert(memoryPool != NULL);
	if (memoryPool == NULL)
		return NULL;
	
	CheckAllocationPool(memoryPool->_usedFirst, memoryPool->_unusedFirst);
	PXPoolAllocator* current = memoryPool->_unusedFirst;
	if (current != NULL)
	{
		// pop off of first
		dassert(current->_previous == NULL);
		memoryPool->_unusedFirst = current->_next;
		if (memoryPool->_unusedFirst != NULL)
		{
			memoryPool->_unusedFirst->_previous = NULL;
		}
		
		CheckPoolThread(memoryPool);
	}
	else
	{
		// create a new one in the used list
		current = (PXPoolAllocator *)malloc(sizeof(PXPoolAllocator));
		current->_object = malloc(size);
		current->_previous = NULL;
	}
	
	MarkAllocatorUsed(current);
	
	// add to beginning of used list
	current->_next = memoryPool->_usedFirst;
	if (memoryPool->_usedFirst != NULL)
	{
		memoryPool->_usedFirst->_previous = current;
	}
	
	memoryPool->_usedFirst = current;
	
	return current;
}

void DeallocatePoolObject(PXPoolAllocator* allocator)
{
	dassert(allocator);
	if (!allocator)
		return;
	
	PXMemoryPool *memoryPool = allocator->_pool;
	CheckAllocationPool(memoryPool->_usedFirst, memoryPool->_unusedFirst);
	
	{
		MarkAllocatorUnused(allocator);

		// remove from used list
		if (allocator->_previous == NULL)
		{
			memoryPool->_usedFirst = allocator->_next;
			if (allocator->_next != NULL)
			{
				allocator->_next->_previous = NULL;
			}
		}
		else
		{
			allocator->_previous->_next = allocator->_next;
			if (allocator->_next != NULL)
			{
				allocator->_next->_previous = allocator->_previous;
			}

			allocator->_previous = NULL;
		}

		// add to unused list
		allocator->_next = memoryPool->_unusedFirst;
		if (memoryPool->_unusedFirst != NULL)
		{
			memoryPool->_unusedFirst->_previous = allocator;
		}

		memoryPool->_unusedFirst = allocator;
	}
}
