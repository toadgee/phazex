#pragma once

#if DEBUG
#define MarkPoolThread(pool) pool->_thread = [NSThread currentThread]
#define CheckPoolThread(pool) assert(pool->_thread == [NSThread currentThread])
#else
#define MarkPoolThread(pool)
#define CheckPoolThread(pool)
#endif

struct PXMemoryPoolStruct;
#define PXPoolAllocator struct PXPoolAllocatorStruct
struct PXPoolAllocatorStruct
{
#if DEBUG
	int8_t _used;
#endif

	struct PXMemoryPoolStruct *_pool;
	void* _object;
	PXPoolAllocator* _next;
	PXPoolAllocator* _previous;
};

#define PXMemoryPool struct PXMemoryPoolStruct
struct PXMemoryPoolStruct
{
#if DEBUG
	NSThread *_thread;
#endif

	PXPoolAllocator *_unusedFirst;
	PXPoolAllocator *_usedFirst;
};

#define AllocateImplementation(type, methodName, livingCount, log, logName) \
	type * methodName(struct PXMemoryPoolStruct *memoryPool); \
	type* methodName(struct PXMemoryPoolStruct *memoryPool) \
	{ \
		PXPoolAllocator* allocator = AllocatePoolObject(sizeof(type), memoryPool); \
		type* obj = (type *)allocator->_object; \
		obj->_holder = allocator; \
		allocator->_pool = memoryPool; \
		obj->_retainCount = 1; \
		MemIncreaseGlobalCount(livingCount); \
		if (log) \
		{ \
			NSLog(@"<<" logName @" CREATING %p : 1 (+)>>", obj); \
		} \
		return obj; \
	}

#define DeallocateImplementation(type, name, livingCount, log, logName) \
	void name(type* obj); \
	void name(type* obj) \
	{ \
		assert(obj); \
		if (!obj) return; \
		PXPoolAllocator* allocator = (PXPoolAllocator*)obj->_holder; \
		memset(obj, (int)0xDEADBEEF, sizeof(type)); \
		obj->_retainCount = 0; \
		obj->_holder = allocator; \
		DeallocatePoolObject(allocator); \
		MemDecreaseGlobalCount(livingCount); \
		if (log) \
		{ \
			NSLog(@"<<" logName @" DESTROYING %p : 0 (+)>>", obj); \
		} \
	}

#define RetainImplementation(type, name, log, logName) \
	type* name(type* obj) \
	{ \
		if (obj) \
		{ \
			CheckPoolThread(((PXPoolAllocator *)obj->_holder)->_pool); \
			int32_t rc = MemIncreaseRetainCount(obj->_retainCount); \
			if (log) \
			{ \
				NSLog(@"<<" logName @" %p : %d (+)>>", obj, rc); \
			} \
		} \
		return obj; \
	}

#define ReleaseImplementation(type, name, log, logName, deallocateMethod) \
	void name(type* obj) \
	{ \
		if (!obj) return; \
		CheckPoolThread(((PXPoolAllocator *)obj->_holder)->_pool); \
		int32_t rc = MemDecreaseRetainCount(obj->_retainCount); \
		if (log) \
		{ \
			NSLog(@"<<" logName @" %p : %d (-)>>", obj, rc); \
		} \
		assert(rc >= 0); \
		if (rc == 0) \
		{ \
			deallocateMethod(obj); \
		} \
	} \



#define MemIncreaseRetainCount(x) (++x)
#define MemDecreaseRetainCount(x) (--x)


#if DEBUG
// global count doesn't need atomic increment & decrement right now
#define MemIncreaseGlobalCount(x) (++x)
#define MemDecreaseGlobalCount(x) (--x)
#else
#define MemIncreaseGlobalCount(x)
#define MemDecreaseGlobalCount(x)
#endif

PXPoolAllocator* AllocatePoolObject(size_t size, PXMemoryPool *memoryPool);
void DeallocatePoolObject(PXPoolAllocator* allocator);
