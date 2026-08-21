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


#include "C4Manipulator.h"


using namespace C4;


namespace C4
{
	template class Constructable<Manipulator, Node *>;
}


Manipulator::Manipulator(Node *node)
{
	targetNode = node;
	manipulatorState = 0;
}

Manipulator::~Manipulator()
{
}

Manipulator *Manipulator::Construct(Node *node)
{
	return (Constructable<Manipulator, Node *>::Construct(node));
}

void Manipulator::Pack(Packer& data, unsigned_int32 packFlags) const
{
	data << ChunkHeader('STAT', 4);
	
	unsigned_int32 state = manipulatorState & kManipulatorHidden;
	data << state;
	
	data << TerminatorChunk;
}

void Manipulator::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<Manipulator>(data, unpackFlags);
}

bool Manipulator::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'STAT':
			
			data >> manipulatorState;
			return (true);
	}
	
	return (false);
}

void Manipulator::Preprocess(void)
{
}

void Manipulator::Neutralize(void)
{
}

void Manipulator::Invalidate(void)
{
}

// ZYURVUR
