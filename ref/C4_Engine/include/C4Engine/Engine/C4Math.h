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


#ifndef C4Math_h
#define C4Math_h


//# \component	Math Library
//# \prefix		Math/


#include "C4Memory.h"


#if C4WINDOWS && C4FASTBUILD

	extern "C"
	{
		double __cdecl fabs(double);
		double __cdecl ceil(double);
		double __cdecl floor(double);
        double __cdecl acos(double);
        double __cdecl asin(double);
        double __cdecl atan(double);
        double __cdecl atan2(double, double);
        double __cdecl exp(double);
        double __cdecl log(double);
        double __cdecl log10(double);
        double __cdecl pow(double, double);
	}

#endif


namespace C4
{
	//# \namespace	C4		The namespace containing global mathematical functions.
	//
	//# The $C4$ namespace contains global functions that don't belong to any other class or namespace.
	//
	//# \def	namespace C4 {...}
	//
	//# \also	$@Math@$
	
	
	//# \function	CosSin		Calculates the sine and cosine of an angle simultaneously.
	//
	//# \proto	Vector2D CosSin(float f);
	//
	//# \param	f	The angle for which to calculate sine and cosine, in radians.
	//
	//# \desc
	//# The $CosSin$ function returns a $@Vector2D@$ object for which the <i>x</i> coordinate contains the
	//# cosine of the angle specified by the $f$ parameter and the <i>y</i> coordinate contains its sine.
	//#
	//# This function executes with much higher performance compared to calculating the sine and cosine separately.
	//
	//# \also	$@Math::GetTrigTable@$
	
	
	#if C4POWERPC
	
		inline float Fabs(float x)
		{
			return (__fabs(x));
		}
		
		inline float Fnabs(float x)
		{
			return (__fnabs(x));
		}
		
		inline float Fmin(float x, float y)
		{
			return (__fsel(x - y, y, x));
		}
		
		inline float Fmin(float x, float y, float z)
		{
			return (Fmin(Fmin(x, y), z));
		}
		
		inline float Fmin(float x, float y, float z, float w)
		{
			return (Fmin(Fmin(x, y), Fmin(z, w)));
		}
		
		inline float Fmax(float x, float y)
		{
			return (__fsel(x - y, x, y));
		}
		
		inline float Fmax(float x, float y, float z)
		{
			return (Fmax(Fmax(x, y), z));
		}
		
		inline float Fmax(float x, float y, float z, float w)
		{
			return (Fmax(Fmax(x, y), Fmax(z, w)));
		}
		
		inline float FminZero(float x)
		{
			return (__fsel(x, 0.0F, x)); 
		}
		 
		inline float FmaxZero(float x) 
		{ 
			return (__fsel(x, x, 0.0F));
		} 
		
		inline float Saturate(float x)
		{
			x = __fsel(x, x, 0.0F); 
			return (__fsel(x - 1.0F, 1.0F, x));
		}
		
		inline float Clamp(float x, float y, float z) 
		{
			x = __fsel(x - y, x, y);
			return (__fsel(x - z, z, x));
		}
	
	#else
	
		inline float Fabs(float x)
		{
			return ((float) fabs(x));
		}
		
		inline float Fnabs(float x)
		{
			return (-(float) fabs(x));
		}
		
		inline float Fmin(const float& x, const float& y)
		{
			#if C4SIMD
			
				float	result;
				
				SimdStoreScalar(SimdMinScalar(SimdLoadScalar(&x), SimdLoadScalar(&y)), &result);
				return (result);
			
			#else
			
				return ((x < y) ? x : y);
			
			#endif
		}
		
		inline float Fmin(const float& x, const float& y, const float& z)
		{
			#if C4SIMD
			
				float	result;
				
				SimdStoreScalar(SimdMinScalar(SimdMinScalar(SimdLoadScalar(&x), SimdLoadScalar(&y)), SimdLoadScalar(&z)), &result);
				return (result);
			
			#else
			
				return (Fmin(Fmin(x, y), z));
			
			#endif
		}
		
		inline float Fmin(const float& x, const float& y, const float& z, const float& w)
		{
			#if C4SIMD
			
				float	result;
				
				SimdStoreScalar(SimdMinScalar(SimdMinScalar(SimdLoadScalar(&x), SimdLoadScalar(&y)), SimdMinScalar(SimdLoadScalar(&z), SimdLoadScalar(&w))), &result);
				return (result);
			
			#else
			
				return (Fmin(Fmin(x, y), Fmin(z, w)));
			
			#endif
		}
		
		inline float Fmax(const float& x, const float& y)
		{
			#if C4SIMD
			
				float	result;
				
				SimdStoreScalar(SimdMaxScalar(SimdLoadScalar(&x), SimdLoadScalar(&y)), &result);
				return (result);
			
			#else
			
				return ((x < y) ? y : x);
			
			#endif
		}
		
		inline float Fmax(const float& x, const float& y, const float& z)
		{
			#if C4SIMD
			
				float	result;
				
				SimdStoreScalar(SimdMaxScalar(SimdMaxScalar(SimdLoadScalar(&x), SimdLoadScalar(&y)), SimdLoadScalar(&z)), &result);
				return (result);
			
			#else
			
				return (Fmax(Fmax(x, y), z));
			
			#endif
		}
		
		inline float Fmax(const float& x, const float& y, const float& z, const float& w)
		{
			#if C4SIMD
			
				float	result;
				
				SimdStoreScalar(SimdMaxScalar(SimdMaxScalar(SimdLoadScalar(&x), SimdLoadScalar(&y)), SimdMaxScalar(SimdLoadScalar(&z), SimdLoadScalar(&w))), &result);
				return (result);
			
			#else
			
				return (Fmax(Fmax(x, y), Fmax(z, w)));
			
			#endif
		}
		
		inline float FminZero(const float& x)
		{
			#if C4SIMD
			
				float	result;
				
				SimdStoreScalar(SimdMinScalar(SimdLoadScalar(&x), SimdGetZero()), &result);
				return (result);
			
			#else
			
				return ((x < 0.0F) ? x : 0.0F);
			
			#endif
		}
		
		inline float FmaxZero(const float& x)
		{
			#if C4SIMD
			
				float	result;
				
				SimdStoreScalar(SimdMaxScalar(SimdLoadScalar(&x), SimdGetZero()), &result);
				return (result);
			
			#else
			
				return ((x < 0.0F) ? 0.0F : x);
			
			#endif
		}
		
		inline float Saturate(const float& x)
		{
			#if C4SIMD
			
				float	result;
				
				SimdStoreScalar(SimdMinScalar(SimdMaxScalar(SimdLoadScalar(&x), SimdGetZero()), SimdLoadConstant<0x3F800000>()), &result);
				return (result);
			
			#else
			
				float f = (x < 0.0F) ? 0.0F : x;
				return ((f < 1.0F) ? f : 1.0F);
			
			#endif
		}
		
		inline float Clamp(const float& x, const float& y, const float& z)
		{
			#if C4SIMD
			
				float	result;
				
				SimdStoreScalar(SimdMinScalar(SimdMaxScalar(SimdLoadScalar(&x), SimdLoadScalar(&y)), SimdLoadScalar(&z)), &result);
				return (result);
			
			#else
			
				float f = (x < y) ? y : x;
				return ((f < z) ? f : z);
			
			#endif
		}
	
	#endif
	
	
	inline float Floor(const float& x)
	{
		#if C4SIMD
		
			float	result;
			
			SimdStoreScalar(SimdFloorScalar(SimdLoadScalar(&x)), &result);
			return (result);
		
		#else
		
			return ((float) floor(x));
		
		#endif
	}
	
	inline float PositiveFloor(const float& x)
	{
		#if C4SIMD
		
			float	result;
			
			SimdStoreScalar(SimdPositiveFloorScalar(SimdLoadScalar(&x)), &result);
			return (result);
		
		#else
		
			return ((float) floor(x));
		
		#endif
	}
	
	inline float NegativeFloor(const float& x)
	{
		#if C4SIMD
		
			float	result;
			
			SimdStoreScalar(SimdNegativeFloorScalar(SimdLoadScalar(&x)), &result);
			return (result);
		
		#else
		
			return ((float) floor(x));
		
		#endif
	}
	
	inline float Ceil(const float& x)
	{
		#if C4SIMD
		
			float	result;
			
			SimdStoreScalar(SimdCeilScalar(SimdLoadScalar(&x)), &result);
			return (result);
		
		#else
		
			return ((float) ceil(x));
		
		#endif
	}
	
	inline float PositiveCeil(const float& x)
	{
		#if C4SIMD
		
			float	result;
			
			SimdStoreScalar(SimdPositiveCeilScalar(SimdLoadScalar(&x)), &result);
			return (result);
		
		#else
		
			return ((float) ceil(x));
		
		#endif
	}
	
	inline float NegativeCeil(const float& x)
	{
		#if C4SIMD
		
			float	result;
			
			SimdStoreScalar(SimdNegativeCeilScalar(SimdLoadScalar(&x)), &result);
			return (result);
		
		#else
		
			return ((float) ceil(x));
		
		#endif
	}
	
	inline void FloorCeil(const float& x, float *f, float *c)
	{
		#if C4SIMD
		
			float4		vf, vc;
			
			SimdFloorCeilScalar(SimdLoadScalar(&x), &vf, &vc);
			SimdStoreScalar(vf, f);
			SimdStoreScalar(vc, c);
		
		#else
		
			*f = (float) floor(x);
			*c = (float) ceil(x);
		
		#endif
	}
	
	inline void PositiveFloorCeil(const float& x, float *f, float *c)
	{
		#if C4SIMD
		
			float4		vf, vc;
			
			SimdPositiveFloorCeilScalar(SimdLoadScalar(&x), &vf, &vc);
			SimdStoreScalar(vf, f);
			SimdStoreScalar(vc, c);
		
		#else
		
			*f = (float) floor(x);
			*c = (float) ceil(x);
		
		#endif
	}
	
	inline void NegativeFloorCeil(const float& x, float *f, float *c)
	{
		#if C4SIMD
		
			float4		vf, vc;
			
			SimdNegativeFloorCeilScalar(SimdLoadScalar(&x), &vf, &vc);
			SimdStoreScalar(vf, f);
			SimdStoreScalar(vc, c);
		
		#else
		
			*f = (float) floor(x);
			*c = (float) ceil(x);
		
		#endif
	}
	
	inline float Frac(const float& x)
	{
		return (x - Floor(x));
	}
	
	inline float PositiveFrac(const float& x)
	{
		return (x - PositiveFloor(x));
	}
	
	inline float NegativeFrac(const float& x)
	{
		return (x - NegativeFloor(x));
	}
	
	inline float Fsgn(const float& x)
	{
		#if C4SIMD
		
			float	result;
			
			SimdStoreScalar(SimdFsgn(SimdLoadScalar(&x)), &result);
			return (result);
		
		#else
		
			return ((x < 0.0F) ? -1.0F : ((x > 0.0F) ? 1.0 : 0.0F));
		
		#endif
	}
	
	inline float NonzeroFsgn(const float& x)
	{
		#if C4SIMD
		
			float	result;
			
			SimdStoreScalar(SimdNonzeroFsgn(SimdLoadScalar(&x)), &result);
			return (result);
		
		#else
		
			return ((x < 0.0F) ? -1.0F : 1.0F);
		
		#endif
	}
	
	inline float Asin(float x)
	{
		return ((float) asin(Clamp(x, -1.0F, 1.0F)));
	}
	
	inline float Acos(float x)
	{
		return ((float) acos(Clamp(x, -1.0F, 1.0F)));
	}
	
	inline float Atan(float x)
	{
		return ((float) atan(x));
	}
	
	inline float Atan(float y, float x)
	{
		return ((float) atan2(y, x));
	}
	
	inline float Exp(float x)
	{
		return ((float) exp(x));
	}
	
	inline float Log(float x)
	{
		return ((float) log(x));
	}
	
	inline float Log10(float x)
	{
		return ((float) log10(x));
	}
	
	inline float Pow(float base, float exponent)
	{
		return ((float) pow(base, exponent));
	}
	
	
	class Vector2D;
	class Vector3D;
	struct ConstVector2D;
	
	
	C4API float Sqrt(float x);
	C4API float InverseSqrt(float x);
	
	C4API float Sin(float x);
	C4API float Cos(float x);
	C4API float Tan(float x);
	C4API void CosSin(float x, float *c, float *s);
	C4API Vector2D CosSin(float x);
	
	
	namespace Math
	{
		C4API extern const unsigned_int32 trigTable[256][2];
		
		inline const ConstVector2D *GetTrigTable(void)
		{
			return (reinterpret_cast<const ConstVector2D *>(trigTable));
		}
		
		C4API Vector3D CreatePerpendicular(const Vector3D& v);
		C4API Vector3D CreateUnitPerpendicular(const Vector3D& v);
	}
}


#endif

// ZYURVUR
