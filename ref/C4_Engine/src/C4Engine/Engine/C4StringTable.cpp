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


#include "C4StringTable.h"


using namespace C4;


ResourceDescriptor StringTableResource::descriptor("str");


StringTableResource::StringTableResource(const char *name, ResourceCatalog *catalog) : Resource<StringTableResource>(name, catalog)
{
}

StringTableResource::~StringTableResource()
{
}

void StringTableResource::Preprocess(void)
{
	int32 *data = static_cast<int32 *>(GetData());
	if (data[0] != 1) ProcessString(reinterpret_cast<StringHeader *>(data + 1));
}

void StringTableResource::ProcessString(StringHeader *sh) const
{
	Reverse(&sh->stringID);
	Reverse(&sh->nextStringOffset);
	Reverse(&sh->firstSubstringOffset);
	
	if (sh->firstSubstringOffset != 0) ProcessString(reinterpret_cast<StringHeader *>(reinterpret_cast<char *>(sh) + sh->firstSubstringOffset));
	if (sh->nextStringOffset != 0) ProcessString(reinterpret_cast<StringHeader *>(reinterpret_cast<char *>(sh) + sh->nextStringOffset));
}


StringTable::StringTable(const char *name)
{
	stringTableResource = StringTableResource::Get(name);
	stringHeader = (stringTableResource) ? stringTableResource->GetStringHeader() : nullptr;
}

StringTable::~StringTable()
{
	if (stringTableResource) stringTableResource->Release();
}

const char *StringTable::GetString(const StringID& stringID) const
{
	unsigned_int32 count = 0;
	unsigned_int32 size = stringID.size;
	
	const StringHeader *sh = stringHeader;
	while (sh)
	{
		unsigned_int32 id = stringID.id[count];
		for (;;)
		{
			if (sh->GetStringID() == id)
			{
				if (++count == size) return (sh->GetString());
				break;
			}
			
			sh = sh->GetNextString();
			if (!sh) goto missing;
		}
		
		sh = sh->GetFirstSubstring();
	}
	
	missing:
	return ("<missing>");
}

// ZYURVUR
