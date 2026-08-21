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


#include "C4FragmentPrograms.h"
#include "C4Graphics.h"
#include "C4Engine.h"
#include "C4Time.h"


#define C4LOG_FRAGMENT_PROGRAMS		0


using namespace C4;


HashTable<FragmentProgram> *FragmentProgram::hashTable;
char FragmentProgram::hashTableStorage[sizeof(HashTable<FragmentProgram>)];


bool C4::operator ==(const ShaderSignature& x, const ShaderSignature& y)
{
	unsigned_int32 size = x[0];
	if (y[0] != size) return (false);
	
	for (unsigned_machine a = 1; a <= size; a++) if (x[a] != y[a]) return (false);
	return (true);
}


const char FragmentProgram::copyLightColor[] =
{
	#if C4CGSHADER
	
		"struct resultStruct\n"
		"{\n"
			"half4 color : COLOR;\n"
		"};\n"
		
		"struct programStruct\n"
		"{\n"
			"uniform float4 light_color : C" FRAGMENT_PARAM_LIGHT_COLOR ";\n"
		"};\n"
		
		"resultStruct main(programStruct program)\n"
		"{\n"
			"resultStruct result;\n"
			"result.color = program.light_color;\n"
			"return result;\n"
		"}\n"
	
	#else
	
		"!!ARBfp1.0\n"
		"OPTION ARB_precision_hint_fastest;\n"
		
		"MOV		result.color, program.env[" FRAGMENT_PARAM_LIGHT_COLOR "];\n"
		"END"
	
	#endif
};

const char FragmentProgram::copyVertexColor[] =
{
	#if C4CGSHADER
	
		"struct fragmentStruct\n"
		"{\n"
			"half4 color : COL0;\n"
		"};\n"
		
		"struct resultStruct\n"
		"{\n"
			"half4 color : COLOR;\n"
		"};\n"
		
		"resultStruct main(fragmentStruct fragment)\n"
		"{\n"
			"resultStruct result;\n"
			"result.color = fragment.color;\n"
			"return result;\n"
		"}\n"
	
	#else
	
		"!!ARBfp1.0\n"
		"OPTION ARB_precision_hint_fastest;\n"
		
		"MOV		result.color, fragment.color;\n"
		"END"
	
	#endif
};


FragmentProgram::FragmentProgram(const char *source, unsigned_int32 size, bool programFlag, const unsigned_int32 *signature)
{
	MemoryMgr::CopyMemory(signature, shaderSignature, signature[0] * 4 + 4);
	
	Construct(programFlag);
	SetSourceCode(source, size);
	
	#if C4LOG_FRAGMENT_PROGRAMS
	
		Engine::LogSource(source);
	 
	#endif
} 
 
FragmentProgram::FragmentProgram(const char *source) 
{
	shaderSignature[0] = 0; 
	hashTable->Insert(this);
	
	Construct();
	SetSourceCode(source, Text::GetTextLength(source)); 
	
	#if C4LOG_FRAGMENT_PROGRAMS
	
		Engine::LogSource(source); 
	
	#endif
}

#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

#endif //]

FragmentProgram::~FragmentProgram()
{
	Destruct();
}

unsigned_int32 FragmentProgram::Hash(const KeyType& key)
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

void FragmentProgram::Initialize(void)
{
	hashTable = new(hashTableStorage) HashTable<FragmentProgram>(16, 16);
}

void FragmentProgram::Terminate(void)
{
	hashTable->~HashTable();
}

FragmentProgram *FragmentProgram::Get(const unsigned_int32 *signature)
{
	FragmentProgram *program = hashTable->Find(signature);
	if (program) program->Retain();
	return (program);
}

FragmentProgram *FragmentProgram::New(const char *source, unsigned_int32 size, bool programFlag, const unsigned_int32 *signature)
{
	FragmentProgram *program = MemoryMgr::GetMainHeap()->New<FragmentProgram>(sizeof(FragmentProgram) + signature[0] * 4);
	new(program) FragmentProgram(source, size, programFlag, signature);
	
	program->Retain();
	hashTable->Insert(program);
	return (program);
}

void FragmentProgram::Flush(void)
{
	int32 bucketCount = hashTable->GetBucketCount();
	for (machine a = 0; a < bucketCount; a++)
	{
		FragmentProgram *program = hashTable->GetFirstBucketElement(a);
		while (program)
		{
			FragmentProgram *next = program->Next();
			if (program->GetReferenceCount() == 1) program->Release();
			program = next;
		}
	}
}

#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

#endif //]

// ZYURVUR
