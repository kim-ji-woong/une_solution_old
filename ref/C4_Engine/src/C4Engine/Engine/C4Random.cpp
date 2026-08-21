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


#include "C4Random.h"
#include "C4Constants.h"


using namespace C4;


namespace
{
	struct RandomSeed
	{
		unsigned_int32	n[4];
		
		RandomSeed();
	};
	
	
	RandomSeed	seed;
}


RandomSeed::RandomSeed()
{
	#if C4WINDOWS
	
		SYSTEMTIME	time;
		
		unsigned_int32 a = GetTickCount();
		GetSystemTime(&time);
		unsigned_int32 b = (time.wMinute << 16) | time.wSecond;
	
	#elif C4MACOS
	
		unsigned_int32 a = TickCount();
		unsigned_int32 b = (unsigned_int32) CFAbsoluteTimeGetCurrent();
	
	#elif C4LINUX
	
		timeval		value;
		
		gettimeofday(&value, nullptr);
		unsigned_int32 a = (unsigned_int32) value.tv_sec;
		unsigned_int32 b = (unsigned_int32) value.tv_usec;
	
	#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#endif //]
	
	n[0] = (unsigned_int32) a;
	n[1] = (unsigned_int32) ~a;
	n[2] = (unsigned_int32) b;
	n[3] = (unsigned_int32) ~b;
}

void Math::GetRandomSeed(unsigned_int32 *n)
{
	n[0] = seed.n[0];
	n[1] = seed.n[1];
	n[2] = seed.n[2];
	n[3] = seed.n[3];
}

void Math::SetRandomSeed(const unsigned_int32 *n)
{
	seed.n[0] = n[0];
	seed.n[1] = n[1];
	seed.n[2] = n[2];
	seed.n[3] = n[3];
}

unsigned_int32 Math::Random(unsigned_int32 n)
{
	unsigned_int32 a = seed.n[0] * 0xBC658A9D + 1;
	unsigned_int32 b = seed.n[1] * 0x102F38E5 + 1;
	unsigned_int32 c = seed.n[2] * 0x8712D6BD + 1;
	unsigned_int32 d = seed.n[3] * 0x9DEA7405 + 1;
	
	seed.n[0] = a;
	seed.n[1] = b;
	seed.n[2] = c;
	seed.n[3] = d;
	
	return ((((a + b + c + d) >> 16) * n) >> 16);
}

float Math::RandomFloat(float f)
{
	unsigned_int32 a = seed.n[0] * 0xBC658A9D + 1;
	unsigned_int32 b = seed.n[1] * 0x102F38E5 + 1;
	unsigned_int32 c = seed.n[2] * 0x8712D6BD + 1;
	unsigned_int32 d = seed.n[3] * 0x9DEA7405 + 1;
	
	seed.n[0] = a;
	seed.n[1] = b;
	seed.n[2] = c;
	seed.n[3] = d;
	
	return ((float) ((a + b + c + d) >> 16) * f * K::one_over_65536);
}
 
Vector3D Math::RandomUnitVector(void)
{ 
	float z = RandomFloat(2.0F) - 1.0F; 
	float sp = Sqrt(1.0F - z * z); 
	
	float theta = RandomFloat(K::two_pi); 
	return (Vector3D(CosSin(theta) * sp, z));
}

Quaternion Math::RandomUnitQuaternion(void) 
{
	float z = RandomFloat(2.0F) - 1.0F;
	float sp = Sqrt(1.0F - z * z);
	 
	Vector2D t = CosSin(RandomFloat(K::two_pi)) * sp;
	Vector2D u = CosSin(RandomFloat(K::pi));
	
	return (Quaternion(u.y * t.x, u.y * t.y, u.y * z, u.x));
}

// ZYURVUR
