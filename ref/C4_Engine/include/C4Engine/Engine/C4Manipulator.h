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


#ifndef C4Manipulator_h
#define C4Manipulator_h


#include "C4Construction.h"


namespace C4
{
	typedef Type	ManipulatorType;
	
	
	enum
	{
		kManipulatorSelected	= 1 << 0,
		kManipulatorHidden		= 1 << 1,
		kManipulatorBaseState	= 1 << 2
	};
	
	
	class Node;
	
	
	class C4_API Manipulator : public Packable, public Constructable<Manipulator, Node *>
	{
		private:
			
			Node				*targetNode;
			unsigned_int32		manipulatorState;
		
		public:
			
			Manipulator(Node *node);
			virtual ~Manipulator();
			
			static Manipulator *Construct(Node *node);
			
			Node *GetTargetNode(void) const
			{
				return (targetNode);
			}
			
			unsigned_int32 GetManipulatorState(void) const
			{
				return (manipulatorState);
			}
			
			void SetManipulatorState(unsigned_int32 state)
			{
				manipulatorState = state;
			}
			
			bool Selected(void) const
			{
				return ((manipulatorState & kManipulatorSelected) != 0);
			}
			
			bool Hidden(void) const
			{
				return ((manipulatorState & kManipulatorHidden) != 0);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			virtual void Preprocess(void);
			virtual void Neutralize(void);
			virtual void Invalidate(void);
	};
}


#endif

// ZYURVUR
