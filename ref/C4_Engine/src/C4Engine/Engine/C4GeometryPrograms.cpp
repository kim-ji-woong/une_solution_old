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


#include "C4GeometryPrograms.h"
#include "C4Graphics.h"
#include "C4Engine.h"


#define C4LOG_GEOMETRY_PROGRAMS		0


using namespace C4;


HashTable<GeometryProgram> *GeometryProgram::hashTable;
char GeometryProgram::hashTableStorage[sizeof(HashTable<GeometryProgram>)];


GeometryProgram::GeometryProgram(const char *source, unsigned_int32 size, const unsigned_int32 *signature)
{
	MemoryMgr::CopyMemory(signature, shaderSignature, signature[0] * 4 + 4);
	
	Construct();
	SetSourceCode(source, size);
	
	#if C4LOG_GEOMETRY_PROGRAMS
	
		Engine::LogSource(source);
	
	#endif
}

GeometryProgram::GeometryProgram(const char *source)
{
	shaderSignature[0] = 0;
	hashTable->Insert(this);
	
	Construct();
	SetSourceCode(source, Text::GetTextLength(source));
	
	#if C4LOG_GEOMETRY_PROGRAMS
	
		Engine::LogSource(source);
	
	#endif
}

GeometryProgram::~GeometryProgram()
{
	Destruct();
}

unsigned_int32 GeometryProgram::Hash(const KeyType& key)
{
	unsigned_int32 hash = 0;
	
	int32 count = key[0];
	for (machine a = 1; a <= count; a++)
	{
		hash += key[a];
		hash = (hash << 5) | (hash >> 27);
	}
	
	return (hash);
}

void GeometryProgram::Initialize(void)
{
	hashTable = new(hashTableStorage) HashTable<GeometryProgram>(16, 16);
}

void GeometryProgram::Terminate(void)
{
	hashTable->~HashTable();
}

void GeometryProgram::Flush(void)
{
	int32 bucketCount = hashTable->GetBucketCount();
	for (machine a = 0; a < bucketCount; a++)
	{
		GeometryProgram *program = hashTable->GetFirstBucketElement(a);
		while (program)
		{
			GeometryProgram *next = program->Next();
			if (program->GetReferenceCount() == 1) program->Release();
			program = next;
		}
	}
}

// ZYURVUR
