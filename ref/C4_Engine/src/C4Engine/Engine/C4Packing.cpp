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


#include "C4Packing.h"


using namespace C4;


namespace C4
{
	template <> Heap Memory<ObjectLink>::heap("Packing", MemoryMgr::CalculatePoolSize(64, sizeof(ObjectLink)), kHeapMutexless);
	template class Memory<ObjectLink>;
}


C4::ChunkHeader C4::TerminatorChunk(0, 0);


void C4::Reverse(ChunkHeader *ch)
{
	Reverse(&ch->chunkType);
	Reverse(&ch->chunkSize);
}


Package::Package(void *storage, unsigned_int32 size)
{
	logicalSize = 0;
	physicalSize = size;
	
	activeStorage = static_cast<char *>(storage);
	packageStorage = nullptr;
}

Package::~Package()
{
	delete[] packageStorage;
}

void Package::Resize(unsigned_int32 size)
{
	size = Max(Max(physicalSize * 2, kPackageDefaultSize), size + ((size / 2 + 15) & ~15));
	physicalSize = size;
	
	char *newStorage = new char[size];
	MemoryMgr::CopyMemory(activeStorage, newStorage, logicalSize);
	
	delete[] packageStorage;
	packageStorage = newStorage;
	activeStorage = newStorage;
}

void *Package::Reserve(unsigned_int32 size)
{
	unsigned_int32 newSize = logicalSize + size;
	if (newSize > physicalSize) Resize(newSize);
	
	char *ptr = activeStorage + logicalSize;
	logicalSize = newSize;
	return (ptr);
}


Packer& Packer::operator <<(const char *string)
{
	unsigned_int32 length = Text::GetTextLength(string);
	unsigned_int32 size = ((length + 4) & ~3);
	unsigned_int32 *ptr = static_cast<unsigned_int32 *>(outputPackage->Reserve(size + 4));
	
	ptr[0] = size;
	char *data = reinterpret_cast<char *>(ptr + 1);
	MemoryMgr::CopyMemory(string, data, length);
	
	data += length;
	int32 pad = size - length;
	for (machine a = 0; a < pad; a++) data[a] = 0;
	
	return (*this);
}

PackHandle Packer::BeginSection(void)
{
	PackHandle handle = outputPackage->GetSize();
	outputPackage->Reserve(sizeof(PackHandle));
	return (handle);
}

void Packer::EndSection(PackHandle handle)
{
	unsigned_int32 *ptr = reinterpret_cast<unsigned_int32 *>(outputPackage->activeStorage + handle);
	ptr[0] = outputPackage->logicalSize - handle - sizeof(PackHandle);
}

PackHandle Packer::BeginChunk(unsigned_int32 type)
{
	PackHandle handle = outputPackage->GetSize();
	ChunkHeader *ptr = static_cast<ChunkHeader *>(outputPackage->Reserve(sizeof(ChunkHeader)));
	ptr->chunkType = type;
	return (handle + 4);
}

void Packer::EndChunk(PackHandle handle)
{
	unsigned_int32 *ptr = reinterpret_cast<unsigned_int32 *>(outputPackage->activeStorage + handle); 
	ptr[0] = outputPackage->logicalSize - handle - sizeof(PackHandle);
} 
 
 
Unpacker::Unpacker(const void *ptr, int32 end, int32 ver)
{ 
	pointer = ptr;
	endian = end;
	version = ver;
	 
	if (end != 1) Reverse(&version);
}

Unpacker::~Unpacker() 
{
}

const char *Unpacker::ReadString(void)
{
	const unsigned_int32 *ptr = static_cast<const unsigned_int32 *>(pointer);
	unsigned_int32 physicalLength = *ptr;
	if (endian != 1) Reverse(&physicalLength);
	
	const char *data = reinterpret_cast<const char *>(ptr + 1);
	pointer = data + physicalLength;
	return (data);
}


void Packable::PackType(Packer& data) const
{
}

void Packable::Prepack(List<Object> *linkList) const
{
}

void Packable::Pack(Packer& data, unsigned_int32 packFlags) const
{
}

void Packable::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
}

void *Packable::BeginSettingsUnpack(void)
{
	return (nullptr);
}

void Packable::EndSettingsUnpack(void *cookie)
{
}

// ZYURVUR
