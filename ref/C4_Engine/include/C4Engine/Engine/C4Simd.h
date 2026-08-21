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


#ifndef C4Simd_h
#define C4Simd_h


#if C4WINDOWS && C4FASTBUILD

	typedef union __declspec(intrin_type) __declspec(align(16)) __m128
	{
		float				m128_f32[4];
		unsigned __int64	m128_u64[2];
		__int8				m128_i8[16];
		__int16				m128_i16[8];
		__int32				m128_i32[4];
		__int64				m128_i64[2];
		unsigned __int8		m128_u8[16];
		unsigned __int16	m128_u16[8];
		unsigned __int32	m128_u32[4];
	} __m128;
	
	extern "C"
	{
		extern __m128 _mm_add_ss(__m128, __m128);
		extern __m128 _mm_add_ps(__m128, __m128);
		extern __m128 _mm_sub_ss(__m128, __m128);
		extern __m128 _mm_sub_ps(__m128, __m128);
		extern __m128 _mm_mul_ss(__m128, __m128);
		extern __m128 _mm_mul_ps(__m128, __m128);
		extern __m128 _mm_div_ss(__m128, __m128);
		extern __m128 _mm_div_ps(__m128, __m128);
		extern __m128 _mm_rsqrt_ss(__m128);
		extern __m128 _mm_rsqrt_ps(__m128);
		extern __m128 _mm_min_ss(__m128, __m128);
		extern __m128 _mm_min_ps(__m128, __m128);
		extern __m128 _mm_max_ss(__m128, __m128);
		extern __m128 _mm_max_ps(__m128, __m128);
		extern __m128 _mm_and_ps(__m128, __m128);
		extern __m128 _mm_andnot_ps(__m128, __m128);
		extern __m128 _mm_or_ps(__m128, __m128);
		extern __m128 _mm_xor_ps(__m128, __m128);
		extern __m128 _mm_cmpeq_ps(__m128, __m128);
		extern __m128 _mm_cmplt_ps(__m128, __m128);
		extern int _mm_comieq_ss(__m128, __m128);
		extern int _mm_comilt_ss(__m128, __m128);
		extern int _mm_comigt_ss(__m128, __m128);
		extern int _mm_cvtt_ss2si(__m128);
		extern __m128 _mm_shuffle_ps(__m128, __m128, unsigned int);
		extern __m128 _mm_setzero_ps(void);
		extern __m128 _mm_load_ss(const float *);
		extern __m128 _mm_load_ps(const float *);
		extern __m128 _mm_loadu_ps(const float *);
		extern void _mm_store_ss(float *, __m128);
		extern void _mm_store_ps(float *, __m128);
		extern unsigned int _mm_getcsr(void);
		extern void _mm_setcsr(unsigned int);
	}
	
	#define _MM_SHUFFLE(fp3,fp2,fp1,fp0) (((fp3) << 6) | ((fp2) << 4) | ((fp1) << 2) | ((fp0)))

#endif


namespace C4
{
	#if C4INTEL

		typedef __m128 float4;

	#elif C4POWERPC

		typedef vector float float4;

	#endif
	
	
	inline float4 SimdGetZero(void)
	{
		#if C4INTEL
		
			return (_mm_setzero_ps());
		
		#elif C4POWERPC
		
			return ((float4) vec_splat_s32(0));
		
		#endif
	}
	
	inline float4 SimdGetNegativeZero(void)
	{
		#if C4INTEL
		
			static const unsigned_int32 align_address(16) float_80000000[4] = {0x80000000, 0x80000000, 0x80000000, 0x80000000};
			return (_mm_load_ps(reinterpret_cast<const float *>(float_80000000)));
		
		#elif C4POWERPC
		
			return ((float4) vec_rl(vec_splat_u32(1), vec_splat_u32(-1)));
		
		#endif
	}
	
	template <unsigned int value> inline float4 SimdLoadConstant(void) 
	{
		static const unsigned_int32 align_address(16) k[4] = {value, value, value, value}; 
		 
		#if C4INTEL 
		
			return (_mm_load_ps(reinterpret_cast<const float *>(k))); 
		
		#elif C4POWERPC
		
			return (vec_ld(0, reinterpret_cast<const float *>(k))); 
		
		#endif
	}
	 
	inline float4 SimdSmearX(const float4& v)
	{
		#if C4INTEL
		
			return (_mm_shuffle_ps(v, v, _MM_SHUFFLE(0, 0, 0, 0)));
		
		#elif C4POWERPC
		
			return (vec_splat(v, 0));
		
		#endif
	}
	
	inline float4 SimdSmearY(const float4& v)
	{
		#if C4INTEL
		
			return (_mm_shuffle_ps(v, v, _MM_SHUFFLE(1, 1, 1, 1)));
		
		#elif C4POWERPC
		
			return (vec_splat(v, 1));
		
		#endif
	}
	
	inline float4 SimdSmearZ(const float4& v)
	{
		#if C4INTEL
		
			return (_mm_shuffle_ps(v, v, _MM_SHUFFLE(2, 2, 2, 2)));
		
		#elif C4POWERPC
		
			return (vec_splat(v, 2));
		
		#endif
	}
	
	inline float4 SimdSmearW(const float4& v)
	{
		#if C4INTEL
		
			return (_mm_shuffle_ps(v, v, _MM_SHUFFLE(3, 3, 3, 3)));
		
		#elif C4POWERPC
		
			return (vec_splat(v, 3));
		
		#endif
	}
	
	inline float4 SimdLoad(const float *ptr, machine offset = 0)
	{
		#if C4INTEL
		
			return (_mm_load_ps(&ptr[offset]));
		
		#elif C4POWERPC
		
			return (vec_ld(offset << 2, ptr));
		
		#endif
	}
	
	inline float4 SimdLoadUnaligned(const float *ptr)
	{
		#if C4INTEL
		
			return (_mm_loadu_ps(ptr));
		
		#elif C4POWERPC
		
			return (vec_perm(vec_ld(0, ptr), vec_ld(16, ptr), vec_lvsl(0, ptr)));
		
		#endif
	}
	
	inline float4 SimdLoadScalar(const float *ptr, machine offset = 0)
	{
		#if C4INTEL
		
			return (_mm_load_ss(&ptr[offset]));
		
		#elif C4POWERPC
		
			offset <<= 2;
			float4 v = vec_ld(offset, ptr);
			return (vec_perm(v, v, vec_lvsl(offset, ptr)));
		
		#endif
	}
	
	inline float4 SimdLoadSmearScalar(const float *ptr, machine offset = 0)
	{
		#if C4INTEL
		
			float4 v = _mm_load_ss(&ptr[offset]);
			return (_mm_shuffle_ps(v, v, _MM_SHUFFLE(0, 0, 0, 0)));
		
		#elif C4POWERPC
		
			offset <<= 2;
			float4 v = vec_ld(offset, ptr);
			return (vec_perm(v, v, (vector unsigned char) vec_splat((vector unsigned int) vec_lvsl(offset, ptr), 0)));
		
		#endif
	}
	
	inline void SimdStore(const float4& v, float *ptr, machine offset = 0)
	{
		#if C4INTEL
		
			_mm_store_ps(&ptr[offset], v);
		
		#elif C4POWERPC
		
			vec_st(v, offset << 2, ptr);
		
		#endif
	}
	
	inline void SimdStoreUnaligned(const float4& v, float *ptr)
	{
		#if C4INTEL
		
			_mm_store_ss(&ptr[0], v);
			_mm_store_ss(&ptr[1], SimdSmearY(v));
			_mm_store_ss(&ptr[2], SimdSmearZ(v));
			_mm_store_ss(&ptr[3], SimdSmearW(v));
		
		#elif C4POWERPC
		
			float4 u = vec_perm(v, v, vec_lvsr(0, ptr));
			vec_ste(u, 0, ptr);
			vec_ste(u, 4, ptr);
			vec_ste(u, 8, ptr);
			vec_ste(u, 12, ptr);
		
		#endif
	}
	
	inline void SimdStoreScalar(const float4& v, float *ptr, machine offset = 0)
	{
		#if C4INTEL
		
			_mm_store_ss(&ptr[offset], v);
		
		#elif C4POWERPC
		
			vec_ste(vec_splat(v, 0), offset << 2, ptr);
		
		#endif
	}
	
	inline void SimdStoreX(const float4& v, float *ptr, machine offset = 0)
	{
		#if C4INTEL
		
			_mm_store_ss(&ptr[offset], _mm_shuffle_ps(v, v, _MM_SHUFFLE(0, 0, 0, 0)));
		
		#elif C4POWERPC
		
			vec_ste(vec_splat(v, 0), offset << 2, ptr);
		
		#endif
	}
	
	inline void SimdStoreY(const float4& v, float *ptr, machine offset = 0)
	{
		#if C4INTEL
		
			_mm_store_ss(&ptr[offset], _mm_shuffle_ps(v, v, _MM_SHUFFLE(1, 1, 1, 1)));
		
		#elif C4POWERPC
		
			vec_ste(vec_splat(v, 1), offset << 2, ptr);
		
		#endif
	}
	
	inline void SimdStoreZ(const float4& v, float *ptr, machine offset = 0)
	{
		#if C4INTEL
		
			_mm_store_ss(&ptr[offset], _mm_shuffle_ps(v, v, _MM_SHUFFLE(2, 2, 2, 2)));
		
		#elif C4POWERPC
		
			vec_ste(vec_splat(v, 2), offset << 2, ptr);
		
		#endif
	}
	
	inline void SimdStoreW(const float4& v, float *ptr, machine offset = 0)
	{
		#if C4INTEL
		
			_mm_store_ss(&ptr[offset], _mm_shuffle_ps(v, v, _MM_SHUFFLE(3, 3, 3, 3)));
		
		#elif C4POWERPC
		
			vec_ste(vec_splat(v, 3), offset << 2, ptr);
		
		#endif
	}
	
	inline void SimdStore3D(const float4& v, float *ptr)
	{
		#if C4INTEL
		
			_mm_store_ss(&ptr[0], v);
			_mm_store_ss(&ptr[1], _mm_shuffle_ps(v, v, _MM_SHUFFLE(1, 1, 1, 1)));
			_mm_store_ss(&ptr[2], _mm_shuffle_ps(v, v, _MM_SHUFFLE(2, 2, 2, 2)));
		
		#elif C4POWERPC
		
			float4 u = vec_perm(v, v, vec_lvsr(0, ptr));
			vec_ste(u, 0, ptr);
			vec_ste(u, 4, ptr);
			vec_ste(u, 8, ptr);
		
		#endif
	}
	
	inline int32 SimdTruncateConvert(const float4& v)
	{
		#if C4INTEL
		
			return (_mm_cvtt_ss2si(v));
		
		#elif C4POWERPC
		
			union
			{
				vector signed int	v;
				int32				i[4];
			} u;
			
			u.v = vec_cts(v, 0);
			return (u.i[0]);
		
		#endif
	}
	
	inline float4 SimdNegate(const float4& v)
	{
		#if C4INTEL
		
			return (_mm_sub_ps(_mm_setzero_ps(), v));
		
		#elif C4POWERPC
		
			return (vec_sub((float4) vec_splat_s32(0), v));
		
		#endif
	}
	
	inline float4 SimdMin(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_min_ps(v1, v2));
		
		#elif C4POWERPC
		
			return (vec_min(v1, v2));
		
		#endif
	}
	
	inline float4 SimdMinScalar(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_min_ss(v1, v2));
		
		#elif C4POWERPC
		
			return (vec_min(v1, v2));
		
		#endif
	}
	
	inline float4 SimdMax(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_max_ps(v1, v2));
		
		#elif C4POWERPC
		
			return (vec_max(v1, v2));
		
		#endif
	}
	
	inline float4 SimdMaxScalar(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_max_ss(v1, v2));
		
		#elif C4POWERPC
		
			return (vec_max(v1, v2));
		
		#endif
	}
	
	inline float4 SimdAdd(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_add_ps(v1, v2));
		
		#elif C4POWERPC
		
			return (vec_add(v1, v2));
		
		#endif
	}
	
	inline float4 SimdAddScalar(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_add_ss(v1, v2));
		
		#elif C4POWERPC
		
			return (vec_add(v1, v2));
		
		#endif
	}
	
	inline float4 SimdSub(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_sub_ps(v1, v2));
		
		#elif C4POWERPC
		
			return (vec_sub(v1, v2));
		
		#endif
	}
	
	inline float4 SimdSubScalar(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_sub_ss(v1, v2));
		
		#elif C4POWERPC
		
			return (vec_sub(v1, v2));
		
		#endif
	}
	
	inline float4 SimdMul(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_mul_ps(v1, v2));
		
		#elif C4POWERPC
		
			return (vec_madd(v1, v2, SimdGetZero()));
		
		#endif
	}
	
	inline float4 SimdMulScalar(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_mul_ss(v1, v2));
		
		#elif C4POWERPC
		
			return (vec_madd(v1, v2, SimdGetZero()));
		
		#endif
	}
	
	inline float4 SimdMadd(const float4& v1, const float4& v2, const float4& v3)
	{
		#if C4INTEL
		
			return (_mm_add_ps(_mm_mul_ps(v1, v2), v3));
		
		#elif C4POWERPC
		
			return (vec_madd(v1, v2, v3));
		
		#endif
	}
	
	inline float4 SimdMaddScalar(const float4& v1, const float4& v2, const float4& v3)
	{
		#if C4INTEL
		
			return (_mm_add_ss(_mm_mul_ss(v1, v2), v3));
		
		#elif C4POWERPC
		
			return (vec_madd(v1, v2, v3));
		
		#endif
	}
	
	inline float4 SimdNmsub(const float4& v1, const float4& v2, const float4& v3)
	{
		#if C4INTEL
		
			return (_mm_sub_ps(v3, _mm_mul_ps(v1, v2)));
		
		#elif C4POWERPC
		
			return (vec_nmsub(v1, v2, v3));
		
		#endif
	}
	
	inline float4 SimdNmsubScalar(const float4& v1, const float4& v2, const float4& v3)
	{
		#if C4INTEL
		
			return (_mm_sub_ss(v3, _mm_mul_ss(v1, v2)));
		
		#elif C4POWERPC
		
			return (vec_nmsub(v1, v2, v3));
		
		#endif
	}
	
	inline float4 SimdDiv(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_div_ps(v1, v2));
		
		#elif C4POWERPC
		
			register const float4 zero = SimdGetZero();
			register const float4 one = SimdLoadConstant<0x3F800000>();
			
			float4 f = vec_re(v2);
			f = vec_madd(vec_nmsub(f, v2, one), f, f);
			return (vec_madd(v1, f, zero));
		
		#endif
	}
	
	inline float4 SimdDivScalar(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_div_ss(v1, v2));
		
		#elif C4POWERPC
		
			register const float4 zero = SimdGetZero();
			register const float4 one = SimdLoadConstant<0x3F800000>();
			
			float4 f = vec_re(v2);
			f = vec_madd(vec_nmsub(f, v2, one), f, f);
			return (vec_madd(v1, f, zero));
		
		#endif
	}
	
	inline float4 SimdAnd(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_and_ps(v1, v2));
		
		#elif C4POWERPC
		
			return (vec_and(v1, v2));
		
		#endif
	}
	
	inline float4 SimdAndc(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_andnot_ps(v2, v1));
		
		#elif C4POWERPC
		
			return (vec_andc(v1, v2));
		
		#endif
	}
	
	inline float4 SimdOr(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_or_ps(v1, v2));
		
		#elif C4POWERPC
		
			return (vec_or(v1, v2));
		
		#endif
	}
	
	inline float4 SimdXor(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_xor_ps(v1, v2));
		
		#elif C4POWERPC
		
			return (vec_xor(v1, v2));
		
		#endif
	}
	
	inline float4 SimdSelect(const float4& v1, const float4& v2, const float4& mask)
	{
		#if C4INTEL
		
			return (_mm_or_ps(_mm_andnot_ps(mask, v1), _mm_and_ps(mask, v2)));
		
		#elif C4POWERPC
		
			return (vec_sel(v1, v2, (vector unsigned int) mask));
		
		#endif
	}
	
	inline float4 SimdMaskCmpeq(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_cmpeq_ps(v1, v2));
		
		#elif C4POWERPC
		
			return ((vector float) vec_cmpeq(v1, v2));
		
		#endif
	}
	
	inline float4 SimdMaskCmplt(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_cmplt_ps(v1, v2));
		
		#elif C4POWERPC
		
			return ((vector float) vec_cmpgt(v2, v1));
		
		#endif
	}
	
	inline float4 SimdMaskCmpgt(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_cmplt_ps(v2, v1));
		
		#elif C4POWERPC
		
			return ((vector float) vec_cmpgt(v1, v2));
		
		#endif
	}
	
	inline bool SimdCmpeqScalar(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_comieq_ss(v1, v2));
		
		#elif C4POWERPC
		
			return (vec_all_eq(vec_splat(v1, 0), vec_splat(v2, 0)));
		
		#endif
	}
	
	inline bool SimdCmpltScalar(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_comilt_ss(v1, v2));
		
		#elif C4POWERPC
		
			return (vec_all_lt(vec_splat(v1, 0), vec_splat(v2, 0)));
		
		#endif
	}
	
	inline bool SimdCmpgtScalar(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (_mm_comigt_ss(v1, v2));
		
		#elif C4POWERPC
		
			return (vec_all_gt(vec_splat(v1, 0), vec_splat(v2, 0)));
		
		#endif
	}
	
	inline bool SimdCmpltAny3D(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (((_mm_comilt_ss(v1, v2)) | (_mm_comilt_ss(SimdSmearY(v1), SimdSmearY(v2))) | (_mm_comilt_ss(SimdSmearZ(v1), SimdSmearZ(v2)))) != 0);
		
		#elif C4POWERPC
		
			const vector unsigned char p = vec_lvsl(12, (const float *) 0);
			float4 u1 = vec_perm(vec_sld(v1, v1, 4), v1, p);
			float4 u2 = vec_perm(vec_sld(v2, v2, 4), v2, p);
			return (vec_any_gt(u2, u1));
		
		#endif
	}
	
	inline bool SimdCmpgtAny3D(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			return (((_mm_comigt_ss(v1, v2)) | (_mm_comigt_ss(SimdSmearY(v1), SimdSmearY(v2))) | (_mm_comigt_ss(SimdSmearZ(v1), SimdSmearZ(v2)))) != 0);
		
		#elif C4POWERPC
		
			const vector unsigned char p = vec_lvsl(12, (const float *) 0);
			float4 u1 = vec_perm(vec_sld(v1, v1, 4), v1, p);
			float4 u2 = vec_perm(vec_sld(v2, v2, 4), v2, p);
			return (vec_any_gt(u1, u2));
		
		#endif
	}
	
	inline float4 SimdInverseSqrt(const float4& v)
	{
		#if C4INTEL
		
			register const float4 three = SimdLoadConstant<0x40400000>();
			register const float4 half = SimdLoadConstant<0x3F000000>();
			
			float4 f = _mm_rsqrt_ps(v);
			return (_mm_mul_ps(_mm_mul_ps(_mm_sub_ps(three, _mm_mul_ps(v, _mm_mul_ps(f, f))), f), half));
		
		#elif C4POWERPC
		
			register const float4 zero = SimdGetZero();
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 half = SimdLoadConstant<0x3F000000>();
			
			float4 f = vec_rsqrte(v);
			return (vec_madd(vec_nmsub(v, vec_madd(f, f, zero), one), vec_madd(f, half, zero), f));
		
		#endif
	}
	
	inline float4 SimdInverseSqrtScalar(const float4& v)
	{
		#if C4INTEL
		
			register const float4 three = SimdLoadConstant<0x40400000>();
			register const float4 half = SimdLoadConstant<0x3F000000>();
			
			float4 f = _mm_rsqrt_ss(v);
			return (_mm_mul_ss(_mm_mul_ss(_mm_sub_ss(three, _mm_mul_ss(v, _mm_mul_ss(f, f))), f), half));
		
		#elif C4POWERPC
		
			register const float4 zero = SimdGetZero();
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 half = SimdLoadConstant<0x3F000000>();
			
			float4 f = vec_rsqrte(v);
			return (vec_madd(vec_nmsub(v, vec_madd(f, f, zero), one), vec_madd(f, half, zero), f));
		
		#endif
	}
	
	inline float4 SimdSqrt(const float4& v)
	{
		float4 mask = SimdMaskCmpeq(v, SimdGetZero());
		return (SimdAndc(SimdMul(SimdInverseSqrt(v), v), mask));
	}
	
	inline float4 SimdSqrtScalar(const float4& v)
	{
		float4 mask = SimdMaskCmpeq(v, SimdGetZero());
		return (SimdAndc(SimdMulScalar(SimdInverseSqrtScalar(v), v), mask));
	}
	
	inline float4 SimdFloor(const float4& v)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			
			float4 result = _mm_sub_ps(_mm_add_ps(_mm_add_ps(_mm_sub_ps(v, two23), two23), two23), two23);
			result = _mm_sub_ps(result, _mm_and_ps(one, _mm_cmplt_ps(v, result)));
			
			float4 mask = _mm_cmplt_ps(two23, _mm_andnot_ps(SimdGetNegativeZero(), v));
			return (_mm_or_ps(_mm_andnot_ps(mask, result), _mm_and_ps(mask, v)));
		
		#elif C4POWERPC
		
			return (vec_floor(v));
		
		#endif
	}
	
	inline float4 SimdFloorScalar(const float4& v)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			
			float4 result = _mm_sub_ss(_mm_add_ss(_mm_add_ss(_mm_sub_ss(v, two23), two23), two23), two23);
			result = _mm_sub_ss(result, _mm_and_ps(one, _mm_cmplt_ps(v, result)));
			
			float4 mask = _mm_cmplt_ps(two23, _mm_andnot_ps(SimdGetNegativeZero(), v));
			return (_mm_or_ps(_mm_andnot_ps(mask, result), _mm_and_ps(mask, v)));
		
		#elif C4POWERPC
		
			return (vec_floor(v));
		
		#endif
	}
	
	inline float4 SimdPositiveFloor(const float4& v)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			
			float4 result = _mm_sub_ps(_mm_add_ps(v, two23), two23);
			return (_mm_sub_ps(result, _mm_and_ps(one, _mm_cmplt_ps(v, result))));
		
		#elif C4POWERPC
		
			return (vec_floor(v));
		
		#endif
	}
	
	inline float4 SimdPositiveFloorScalar(const float4& v)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			
			float4 result = _mm_sub_ss(_mm_add_ss(v, two23), two23);
			return (_mm_sub_ss(result, _mm_and_ps(one, _mm_cmplt_ps(v, result))));
		
		#elif C4POWERPC
		
			return (vec_floor(v));
		
		#endif
	}
	
	inline float4 SimdNegativeFloor(const float4& v)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			
			float4 result = _mm_add_ps(_mm_sub_ps(v, two23), two23);
			return (_mm_sub_ps(result, _mm_and_ps(one, _mm_cmplt_ps(v, result))));
		
		#elif C4POWERPC
		
			return (vec_floor(v));
		
		#endif
	}
	
	inline float4 SimdNegativeFloorScalar(const float4& v)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			
			float4 result = _mm_add_ss(_mm_sub_ss(v, two23), two23);
			return (_mm_sub_ss(result, _mm_and_ps(one, _mm_cmplt_ps(v, result))));
		
		#elif C4POWERPC
		
			return (vec_floor(v));
		
		#endif
	}
	
	inline float4 SimdCeil(const float4& v)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			
			float4 result = _mm_sub_ps(_mm_add_ps(_mm_add_ps(_mm_sub_ps(v, two23), two23), two23), two23);
			result = _mm_add_ps(result, _mm_and_ps(one, _mm_cmplt_ps(result, v)));
			
			float4 mask = _mm_cmplt_ps(two23, _mm_andnot_ps(SimdGetNegativeZero(), v));
			return (_mm_or_ps(_mm_andnot_ps(mask, result), _mm_and_ps(mask, v)));
		
		#elif C4POWERPC
		
			return (vec_ceil(v));
		
		#endif
	}
	
	inline float4 SimdCeilScalar(const float4& v)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			
			float4 result = _mm_sub_ss(_mm_add_ss(_mm_add_ss(_mm_sub_ss(v, two23), two23), two23), two23);
			result = _mm_add_ss(result, _mm_and_ps(one, _mm_cmplt_ps(result, v)));
			
			float4 mask = _mm_cmplt_ps(two23, _mm_andnot_ps(SimdGetNegativeZero(), v));
			return (_mm_or_ps(_mm_andnot_ps(mask, result), _mm_and_ps(mask, v)));
		
		#elif C4POWERPC
		
			return (vec_ceil(v));
		
		#endif
	}
	
	inline float4 SimdPositiveCeil(const float4& v)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			
			float4 result = _mm_sub_ps(_mm_add_ps(v, two23), two23);
			return (_mm_add_ps(result, _mm_and_ps(one, _mm_cmplt_ps(result, v))));
		
		#elif C4POWERPC
		
			return (vec_ceil(v));
		
		#endif
	}
	
	inline float4 SimdPositiveCeilScalar(const float4& v)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			
			float4 result = _mm_sub_ss(_mm_add_ss(v, two23), two23);
			return (_mm_add_ss(result, _mm_and_ps(one, _mm_cmplt_ps(result, v))));
		
		#elif C4POWERPC
		
			return (vec_ceil(v));
		
		#endif
	}
	
	inline float4 SimdNegativeCeil(const float4& v)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			
			float4 result = _mm_add_ps(_mm_sub_ps(v, two23), two23);
			return (_mm_add_ps(result, _mm_and_ps(one, _mm_cmplt_ps(result, v))));
		
		#elif C4POWERPC
		
			return (vec_ceil(v));
		
		#endif
	}
	
	inline float4 SimdNegativeCeilScalar(const float4& v)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			
			float4 result = _mm_add_ss(_mm_sub_ss(v, two23), two23);
			return (_mm_add_ss(result, _mm_and_ps(one, _mm_cmplt_ps(result, v))));
		
		#elif C4POWERPC
		
			return (vec_ceil(v));
		
		#endif
	}
	
	inline void SimdFloorCeil(const float4& v, float4 *f, float4 *c)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			
			float4 result = _mm_sub_ps(_mm_add_ps(_mm_add_ps(_mm_sub_ps(v, two23), two23), two23), two23);
			*f = _mm_sub_ps(result, _mm_and_ps(one, _mm_cmplt_ps(v, result)));
			*c = _mm_add_ps(result, _mm_and_ps(one, _mm_cmplt_ps(result, v)));
		
		#elif C4POWERPC
		
			*f = vec_floor(v);
			*c = vec_ceil(v);
		
		#endif
	}
	
	inline void SimdFloorCeilScalar(const float4& v, float4 *f, float4 *c)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			
			float4 result = _mm_sub_ss(_mm_add_ss(_mm_add_ss(_mm_sub_ss(v, two23), two23), two23), two23);
			*f = _mm_sub_ss(result, _mm_and_ps(one, _mm_cmplt_ps(v, result)));
			*c = _mm_add_ss(result, _mm_and_ps(one, _mm_cmplt_ps(result, v)));
		
		#elif C4POWERPC
		
			*f = vec_floor(v);
			*c = vec_ceil(v);
		
		#endif
	}
	
	inline void SimdPositiveFloorCeil(const float4& v, float4 *f, float4 *c)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			
			float4 result = _mm_sub_ps(_mm_add_ps(v, two23), two23);
			*f = _mm_sub_ps(result, _mm_and_ps(one, _mm_cmplt_ps(v, result)));
			*c = _mm_add_ps(result, _mm_and_ps(one, _mm_cmplt_ps(result, v)));
		
		#elif C4POWERPC
		
			*f = vec_floor(v);
			*c = vec_ceil(v);
		
		#endif
	}
	
	inline void SimdPositiveFloorCeilScalar(const float4& v, float4 *f, float4 *c)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			
			float4 result = _mm_sub_ss(_mm_add_ss(v, two23), two23);
			*f = _mm_sub_ss(result, _mm_and_ps(one, _mm_cmplt_ps(v, result)));
			*c = _mm_add_ss(result, _mm_and_ps(one, _mm_cmplt_ps(result, v)));
		
		#elif C4POWERPC
		
			*f = vec_floor(v);
			*c = vec_ceil(v);
		
		#endif
	}
	
	inline void SimdNegativeFloorCeil(const float4& v, float4 *f, float4 *c)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			
			float4 result = _mm_add_ps(_mm_sub_ps(v, two23), two23);
			*f = _mm_sub_ps(result, _mm_and_ps(one, _mm_cmplt_ps(v, result)));
			*c = _mm_add_ps(result, _mm_and_ps(one, _mm_cmplt_ps(result, v)));
		
		#elif C4POWERPC
		
			*f = vec_floor(v);
			*c = vec_ceil(v);
		
		#endif
	}
	
	inline void SimdNegativeFloorCeilScalar(const float4& v, float4 *f, float4 *c)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			
			float4 result = _mm_add_ss(_mm_sub_ss(v, two23), two23);
			*f = _mm_sub_ss(result, _mm_and_ps(one, _mm_cmplt_ps(v, result)));
			*c = _mm_add_ss(result, _mm_and_ps(one, _mm_cmplt_ps(result, v)));
		
		#elif C4POWERPC
		
			*f = vec_floor(v);
			*c = vec_ceil(v);
		
		#endif
	}
	
	inline float4 SimdTrunc(const float4& v)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			register const float4 negzero = SimdGetNegativeZero();
			
			float4 w = _mm_andnot_ps(negzero, v);
			float4 result = _mm_sub_ps(_mm_add_ps(w, two23), two23);
			result = _mm_sub_ps(result, _mm_and_ps(one, _mm_cmplt_ps(w, result)));
			result = _mm_or_ps(result, _mm_and_ps(negzero, v));
			
			float4 mask = _mm_cmplt_ps(two23, w);
			return (_mm_or_ps(_mm_andnot_ps(mask, result), _mm_and_ps(mask, v)));
		
		#elif C4POWERPC
		
			return (vec_trunc(v));
		
		#endif
	}
	
	inline float4 SimdTruncScalar(const float4& v)
	{
		#if C4INTEL
		
			register const float4 one = SimdLoadConstant<0x3F800000>();
			register const float4 two23 = SimdLoadConstant<0x4B000000>();
			register const float4 negzero = SimdGetNegativeZero();
			
			float4 w = _mm_andnot_ps(negzero, v);
			float4 result = _mm_sub_ss(_mm_add_ss(w, two23), two23);
			result = _mm_sub_ss(result, _mm_and_ps(one, _mm_cmplt_ps(w, result)));
			result = _mm_or_ps(result, _mm_and_ps(negzero, v));
			
			float4 mask = _mm_cmplt_ps(two23, w);
			return (_mm_or_ps(_mm_andnot_ps(mask, result), _mm_and_ps(mask, v)));
		
		#elif C4POWERPC
		
			return (vec_trunc(v));
		
		#endif
	}
	
	inline float4 SimdFsgn(const float4& v)
	{
		float4 result = SimdLoadConstant<0x3F800000>();
		result = SimdOr(result, SimdAnd(v, SimdGetNegativeZero()));
		return (SimdAndc(result, SimdMaskCmpeq(v, SimdGetZero())));
	}
	
	inline float4 SimdNonzeroFsgn(const float4& v)
	{
		float4 result = SimdLoadConstant<0x3F800000>();
		return (SimdOr(result, SimdAnd(v, SimdGetNegativeZero())));
	}
	
	inline float4 SimdDot3D(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			float4 d = _mm_mul_ps(v1, v2);
			return (_mm_add_ss(_mm_add_ss(d, SimdSmearY(d)), SimdSmearZ(d)));
		
		#elif C4POWERPC
		
			float4 d = vec_madd(v1, v2, SimdGetZero());
			return (vec_add(vec_add(d, SimdSmearY(d)), SimdSmearZ(d)));
		
		#endif
	}
	
	inline float4 SimdPlaneWedgePoint3D(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			float4 d = _mm_mul_ps(v1, v2);
			return (_mm_add_ss(_mm_add_ss(_mm_add_ss(d, SimdSmearY(d)), SimdSmearZ(d)), SimdSmearW(v1)));
		
		#elif C4POWERPC
		
			float4 d = vec_madd(v1, v2, SimdGetZero());
			return (vec_add(vec_add(vec_add(d, SimdSmearY(d)), SimdSmearZ(d)), SimdSmearW(v1)));
		
		#endif
	}
	
	inline float4 SimdProjectOnto3D(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			float4 d = SimdDot3D(v1, v2);
			return (_mm_mul_ps(v2, SimdSmearX(d)));
		
		#elif C4POWERPC
		
			float4 d = SimdDot3D(v1, v2);
			return (vec_madd(v2, SimdSmearX(d), SimdGetZero()));
		
		#endif
	}
	
	inline float4 SimdCross3D(const float4& v1, const float4& v2)
	{
		#if C4INTEL
		
			float4 a = _mm_shuffle_ps(v1, v1, _MM_SHUFFLE(0, 0, 2, 1));
			float4 b = _mm_shuffle_ps(v2, v2, _MM_SHUFFLE(0, 1, 0, 2));
			float4 c = _mm_mul_ps(a, b);
			
			a = _mm_shuffle_ps(v1, v1, _MM_SHUFFLE(0, 1, 0, 2));
			b = _mm_shuffle_ps(v2, v2, _MM_SHUFFLE(0, 0, 2, 1));
			return (_mm_sub_ps(c, _mm_mul_ps(a, b)));
		
		#elif C4POWERPC
		
			const vector unsigned char d1 = vec_lvsl(12, (const float *) 0);
			const vector unsigned char d2 = vec_lvsl(0, (const float *) 0);
			const vector unsigned char p1 = vec_sld(d1, d2, 8);
			const vector unsigned char p2 = vec_sld(d1, d2, 12);
			
			float4 c = vec_madd(vec_perm(v1, v1, p1), vec_perm(v2, v2, p2), SimdGetZero());
			return (vec_nmsub(vec_perm(v1, v1, p2), vec_perm(v2, v2, p1), c));
		
		#endif
	}
	
	inline float4 SimdTransformVector3D(const float4& c1, const float4& c2, const float4& c3, const float4& v)
	{
		#if C4INTEL
		
			float4 result = _mm_mul_ps(c1, SimdSmearX(v));
			result = _mm_add_ps(result, _mm_mul_ps(c2, SimdSmearY(v)));
			return (_mm_add_ps(result, _mm_mul_ps(c3, SimdSmearZ(v))));
		
		#elif C4POWERPC
		
			float4 result = vec_madd(c1, SimdSmearX(v), SimdGetZero());
			result = vec_madd(c2, SimdSmearY(v), result);
			return (vec_madd(c3, SimdSmearZ(v), result));
		
		#endif
	}
	
	inline float4 SimdTransformPoint3D(const float4& c1, const float4& c2, const float4& c3, const float4& c4, const float4& p)
	{
		#if C4INTEL
		
			float4 result = _mm_mul_ps(c1, SimdSmearX(p));
			result = _mm_add_ps(result, _mm_mul_ps(c2, SimdSmearY(p)));
			result = _mm_add_ps(result, _mm_mul_ps(c3, SimdSmearZ(p)));
			return (_mm_add_ps(result, c4));
		
		#elif C4POWERPC
		
			float4 result = vec_madd(c1, SimdSmearX(p), SimdGetZero());
			result = vec_madd(c2, SimdSmearY(p), result);
			result = vec_madd(c3, SimdSmearZ(p), result);
			return (vec_add(result, c4));
		
		#endif
	}
}


#endif

// ZYURVUR
