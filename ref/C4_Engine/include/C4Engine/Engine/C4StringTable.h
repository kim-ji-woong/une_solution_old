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


#ifndef C4StringTable_h
#define C4StringTable_h


#include "C4Resources.h"


namespace C4
{
	struct StringHeader
	{
		friend class StringTableResource;
		
		private:
			
			unsigned_int32	stringID;
			
			int32			nextStringOffset;
			int32			firstSubstringOffset;
		
		public:
			
			StringHeader() {}
			
			StringHeader(unsigned_int32 id)
			{
				stringID = id;
				nextStringOffset = 0;
				firstSubstringOffset = 0;
			}
			
			unsigned_int32 GetStringID(void) const
			{
				return (stringID);
			}
			
			const char *GetString(void) const
			{
				return (reinterpret_cast<const char *>(this + 1));
			}
			
			const StringHeader *GetNextString(void) const
			{
				return ((nextStringOffset != 0) ? reinterpret_cast<const StringHeader *>(reinterpret_cast<const char *>(this) + nextStringOffset) : nullptr);
			}
			
			const StringHeader *GetFirstSubstring(void) const
			{
				return ((firstSubstringOffset != 0) ? reinterpret_cast<const StringHeader *>(reinterpret_cast<const char *>(this) + firstSubstringOffset) : nullptr);
			}
			
			void SetNextStringOffset(int32 offset)
			{
				nextStringOffset = offset;
			}
			
			void SetFirstSubstringOffset(int32 offset)
			{
				firstSubstringOffset = offset;
			}
	};
	
	
	class StringID
	{
		friend class StringTable;
		
		private:
			
			unsigned_int32	size;
			unsigned_int32	id[5];
			
		public:
			
			explicit StringID(unsigned_int32 a)
			{
				size = 1;
				id[0] = a;
			}
			
			StringID(unsigned_int32 a, unsigned_int32 b)
			{
				size = 2;
				id[0] = a;
				id[1] = b;
			}
			
			StringID(unsigned_int32 a, unsigned_int32 b, unsigned_int32 c)
			{
				size = 3;
				id[0] = a;
				id[1] = b;
				id[2] = c;
			}
			
			StringID(unsigned_int32 a, unsigned_int32 b, unsigned_int32 c, unsigned_int32 d)
			{
				size = 4;
				id[0] = a;
				id[1] = b;
				id[2] = c;
				id[3] = d; 
			}
			 
			StringID(unsigned_int32 a, unsigned_int32 b, unsigned_int32 c, unsigned_int32 d, unsigned_int32 e) 
			{ 
				size = 5;
				id[0] = a; 
				id[1] = b;
				id[2] = c;
				id[3] = d;
				id[4] = e; 
			}
	};
	
	 
	class StringTableResource : public Resource<StringTableResource>
	{
		friend class Resource<StringTableResource>;
		
		private:
			
			static C4API ResourceDescriptor		descriptor;
			
			~StringTableResource();
			
			void Preprocess(void);
			void ProcessString(StringHeader *sh) const;
		
		public:
			
			StringTableResource(const char *name, ResourceCatalog *catalog);
			
			const StringHeader *GetStringHeader(void) const
			{
				return (reinterpret_cast<const StringHeader *>(static_cast<const int32 *>(GetData()) + 1));
			}
	};
	
	
	class StringTable
	{
		private:
			
			StringTableResource		*stringTableResource;
			const StringHeader		*stringHeader;
			
		public:
			
			C4API StringTable(const char *name);
			C4API ~StringTable();
			
			const StringHeader *GetRootStringHeader(void) const
			{
				return (stringHeader);
			}
			
			C4API const char *GetString(const StringID& stringID) const;
	};
}


#endif

// ZYURVUR
