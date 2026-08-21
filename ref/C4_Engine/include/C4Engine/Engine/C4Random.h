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


#ifndef C4Random_h
#define C4Random_h


//# \component	Math Library
//# \prefix		Math/


#include "C4Vector3D.h"
#include "C4Quaternion.h"


namespace C4
{
	namespace Math
	{
		C4API void GetRandomSeed(unsigned_int32 *n);
		C4API void SetRandomSeed(const unsigned_int32 *n);
		
		C4API unsigned_int32 Random(unsigned_int32 n);
		C4API float RandomFloat(float f);
		
		C4API Vector3D RandomUnitVector(void);
		C4API Quaternion RandomUnitQuaternion(void);
	}
}


#endif

// ZYURVUR
