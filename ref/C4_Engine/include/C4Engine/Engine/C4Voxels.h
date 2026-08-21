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


#ifndef C4Voxels_h
#define C4Voxels_h


#include "C4Types.h"


namespace C4
{
	typedef int8			Voxel;
	typedef unsigned_int8	UnsignedVoxel;
	
	
	enum
	{
		kVoxelFractionSize		= 8,
		kVoxelFixedUnit			= 1 << kVoxelFractionSize
	};
	
	
	namespace Transvoxel
	{
		struct RegularCellData
		{
			unsigned_int8	geometryCounts;
			unsigned_int8	vertexIndex[15];
			
			int32 GetVertexCount(void) const
			{
				return (geometryCounts >> 4);
			}
			
			int32 GetTriangleCount(void) const
			{
				return (geometryCounts & 0x0F);
			}
		};
		
		
		struct TransitionCellData
		{
			int32			geometryCounts;
			unsigned_int8	vertexIndex[36];
			
			int32 GetVertexCount(void) const
			{
				return (geometryCounts >> 4);
			}
			
			int32 GetTriangleCount(void) const
			{
				return (geometryCounts & 0x0F);
			}
		};
		
		
		struct InternalEdgeData
		{
			unsigned_int8	edgeCount;
			unsigned_int8	vertexIndex[4][4];
		};
		
		
		extern const unsigned_int8 regularCellClass[256];
		extern const RegularCellData regularCellData[2][16];
		extern const InternalEdgeData regularInternalEdgeData[2][16];
		extern const unsigned_int16 regularVertexData[256][12];
		
		extern const unsigned_int8 transitionCellClass[512];
		extern const TransitionCellData transitionCellData[56];
		extern const unsigned_int8 transitionCornerData[13];
		extern const unsigned_int16 transitionVertexData[512][12];
	}
}


#endif

// ZYURVUR
