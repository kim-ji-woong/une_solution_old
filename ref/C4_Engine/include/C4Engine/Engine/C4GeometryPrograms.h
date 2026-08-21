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


#ifndef C4GeometryPrograms_h
#define C4GeometryPrograms_h


#include "C4FragmentPrograms.h"


namespace C4
{
	class GeometryProgram : public Render::GeometryProgramObject, public Shared, public HashTableElement<GeometryProgram>, public LinkTarget<GeometryProgram>
	{
		public:
			
			typedef ShaderSignature KeyType;
		
		private:
			
			static HashTable<GeometryProgram>	*hashTable;
			static char							hashTableStorage[sizeof(HashTable<GeometryProgram>)];
			
			unsigned_int32		shaderSignature[1];
			
			GeometryProgram(const char *source, unsigned_int32 size, const unsigned_int32 *signature);
			~GeometryProgram();
		
		public:
			
			GeometryProgram(const char *source);
			
			KeyType GetKey(void) const
			{
				return (ShaderSignature(shaderSignature));
			}
			
			static unsigned_int32 Hash(const KeyType& key);
			
			static void Initialize(void);
			static void Terminate(void);
			
			static void Flush(void);
	};
}


#endif

// ZYURVUR
