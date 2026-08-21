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


#ifndef C4Compression_h
#define C4Compression_h


#include "C4Types.h"


namespace C4
{
	namespace Comp
	{
		C4API unsigned_int32 CompressData(const void *input, unsigned_int32 dataSize, unsigned_int8 *restrict code);
		C4API void DecompressData(const unsigned_int8 *code, unsigned_int32 codeSize, void *restrict output);
	}
}


#endif

// ZYURVUR
