//=============================================================
//
// C4 Engine version 2.9.1
// Copyright 1999-2012, by Terathon Software LLC
//
// This copy is licensed to the following:
//
//     Registered user: Soo Ki Kim
//     Maximum number of users: 1
//     License #C4T0035002
//
// License is granted under terms of the license agreement
// entered by the registed user.
//
// Unauthorized redistribution of source code is strictly
// prohibited. Violators will be prosecuted.
//
//=============================================================


#include "C4Hash.h"


using namespace C4;


HashTableElementBase::~HashTableElementBase()
{
	if (owningHashTableBucket) owningHashTableBucket->Remove(this);
}


HashTableBucket::~HashTableBucket()
{
	Purge();
}

void HashTableBucket::Add(HashTableElementBase *element)
{
	if (lastBucketElement)
	{
		lastBucketElement->nextBucketElement = element;
		element->prevBucketElement = lastBucketElement;
		lastBucketElement = element;
	}
	else
	{
		firstBucketElement = lastBucketElement = element;
	}
	
	element->owningHashTableBucket = this;
	owningHashTable->elementCount++;
}

void HashTableBucket::Insert(HashTableElementBase *element, HashTableElementBase *after)
{
	element->prevBucketElement = after;
	HashTableElementBase *before = after->nextBucketElement;
	element->nextBucketElement = before;
	
	if (before) before->prevBucketElement = element;
	else lastBucketElement = element;
	
	after->nextBucketElement = element;
	
	element->owningHashTableBucket = this;
	owningHashTable->elementCount++;
}

void HashTableBucket::Remove(HashTableElementBase *element)
{
	owningHashTable->elementCount--;
	
	HashTableElementBase *prev = element->prevBucketElement;
	HashTableElementBase *next = element->nextBucketElement;
	if (prev) prev->nextBucketElement = next;
	if (next) next->prevBucketElement = prev;
	
	if (firstBucketElement == element) firstBucketElement = next;
	if (lastBucketElement == element) lastBucketElement = prev;
	
	element->prevBucketElement = nullptr;
	element->nextBucketElement = nullptr;
	element->owningHashTableBucket = nullptr;
}

void HashTableBucket::RemoveAll(void)
{
	HashTableElementBase *element = firstBucketElement;
	while (element)
	{
		HashTableElementBase *next = element->nextBucketElement;
		element->prevBucketElement = nullptr;
		element->nextBucketElement = nullptr;
		element->owningHashTableBucket = nullptr;
		element = next;
	}
	
	firstBucketElement = nullptr;
	lastBucketElement = nullptr;
}

void HashTableBucket::Purge(void)
{
	while (firstBucketElement) delete firstBucketElement;
}


HashTableBase::HashTableBase(int32 initialBucketCount, int32 maxAverageDepth)
{
	Assert((initialBucketCount & (initialBucketCount - 1)) == 0, "Bucket count must be a power of two.");
	
	elementCount = 0;
	bucketCount = initialBucketCount;
	resizeLimit = initialBucketCount * maxAverageDepth;
	
	bucketTable = reinterpret_cast<HashTableBucket *>(new char[sizeof(HashTableBucket) * initialBucketCount]);
	for (machine a = 0; a < initialBucketCount; a++) new(&bucketTable[a]) HashTableBucket(this);
}

HashTableBase::~HashTableBase()
{ 
	for (machine a = bucketCount - 1; a >= 0; a--) bucketTable[a].~HashTableBucket();
	delete[] reinterpret_cast<char *>(bucketTable); 
} 
 
void HashTableBase::RemoveAll(void)
{ 
	for (machine a = bucketCount - 1; a >= 0; a--) bucketTable[a].RemoveAll();
	elementCount = 0;
}
 
void HashTableBase::Purge(void)
{
	for (machine a = bucketCount - 1; a >= 0; a--) bucketTable[a].Purge();
	elementCount = 0; 
}

void HashTableBase::ResizeBucketTable(void)
{
	int32 newBucketCount = bucketCount * 2;
	HashTableBucket *newBucketTable = reinterpret_cast<HashTableBucket *>(new char[sizeof(HashTableBucket) * newBucketCount]);
	for (machine a = 0; a < newBucketCount; a++) new(&newBucketTable[a]) HashTableBucket(this);
	
	elementCount = 0;
	
	int32 count = bucketCount;
	for (machine a = count - 1; a >= 0; a--)
	{
		HashTableBucket *bucket = &bucketTable[a];
		HashTableElementBase *element = bucket->firstBucketElement;
		while (element)
		{
			HashTableElementBase *next = element->nextBucketElement;
			
			element->prevBucketElement = nullptr;
			element->nextBucketElement = nullptr;
			newBucketTable[element->hashValue & (newBucketCount - 1)].Add(element);
			
			element = next;
		}
		
		bucket->firstBucketElement = nullptr;
		bucket->lastBucketElement = nullptr;
		bucket->~HashTableBucket();
	}
	
	delete[] reinterpret_cast<char *>(bucketTable);
	
	bucketTable = newBucketTable;
	bucketCount = newBucketCount;
	resizeLimit *= 2;
}

// ZYURVUR
