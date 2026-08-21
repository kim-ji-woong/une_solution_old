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


#ifndef C4FragmentPrograms_h
#define C4FragmentPrograms_h


#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

#endif //]


#include "C4Render.h"


namespace C4
{
	enum
	{
		kFragmentParamConstant0						= 0,
		kFragmentParamConstant1						= 1,
		kFragmentParamConstant2						= 2,
		kFragmentParamConstant3						= 3,
		kFragmentParamConstant4						= 4,
		kFragmentParamConstant5						= 5,
		kFragmentParamConstant6						= 6,
		kFragmentParamConstant7						= 7,
		
		kFragmentParamLightColor					= 8,
		kFragmentParamAmbientDelta					= 9,
		
		kFragmentParamFogColor						= 10,
		kFragmentParamFogParams						= 11,		// Fog params (F dot C <= 0, 0.0, 0.0, 0.0)
		
		kFragmentParamShaderTime					= 12,		// (normalized absolute time, 0.0, 0.0, 0.0)
		kFragmentParamDetailLevel					= 13,		// (detail level parameter, 0.0, 0.0, 0.0)
		kFragmentParamParallaxScale					= 14,		// Parallax scale (s scale, t scale, 0.0, 0.0)
		
		kFragmentParamVelocityScale					= 15,		// Velocity scale (x scale, y scale, 0.0, 0.0)
		kFragmentParamDistortionScale				= 16,		// Distortion scale (x scale, y scale, 0.0, 0.0)
		
		kFragmentParamShadowSample1					= 17,		// Shadow sample offsets (ds1, dt1, ds2, dt2)
		kFragmentParamShadowSample2					= 18,		// Shadow sample offsets (ds3, dt3, ds4, dt4)
		kFragmentParamShadowMapScale1				= 19,		// Shadow map (s, t, p) texcoord scale, section 1
		kFragmentParamShadowMapScale2				= 20,		// Shadow map (s, t, p) texcoord scale, section 2
		kFragmentParamShadowMapScale3				= 21,		// Shadow map (s, t, p) texcoord scale, section 3
		kFragmentParamShadowMapOffset1				= 22,		// Shadow map (s, t, p) texcoord offset, section 1
		kFragmentParamShadowMapOffset2				= 23,		// Shadow map (s, t, p) texcoord offset, section 2
		kFragmentParamShadowMapOffset3				= 24,		// Shadow map (s, t, p) texcoord offset, section 3
		kFragmentParamShadowViewDirection			= 25,		// Shadow-space scaled view direction, section 0
		
		kFragmentParamImpostorShadowBlend			= 26,		// Impostor shadow map elevation blend
		kFragmentParamImpostorShadowScale			= 27,		// Impostor shadow map elevation scales
		kFragmentParamImpostorDistance				= 28		// (impostor distance, 0.0, 0.0, 0.0)
		
		// Render::kMaxFragmentParamCount
	};
	
	
	#define FRAGMENT_PARAM_CONSTANT0				"0"
	#define FRAGMENT_PARAM_CONSTANT1				"1"
	#define FRAGMENT_PARAM_CONSTANT2				"2"
	#define FRAGMENT_PARAM_CONSTANT3				"3"
	#define FRAGMENT_PARAM_CONSTANT4				"4"
	#define FRAGMENT_PARAM_CONSTANT5				"5"
	#define FRAGMENT_PARAM_CONSTANT6				"6"
	#define FRAGMENT_PARAM_CONSTANT7				"7"
	
	#define FRAGMENT_PARAM_LIGHT_COLOR				"8"
	#define FRAGMENT_PARAM_AMBIENT_DELTA			"9"
	
	#define FRAGMENT_PARAM_FOG_COLOR				"10"
	#define FRAGMENT_PARAM_FOG_PARAMS				"11"
	
	#define FRAGMENT_PARAM_SHADER_TIME				"12"
	#define FRAGMENT_PARAM_DETAIL_LEVEL				"13"
	#define FRAGMENT_PARAM_PARALLAX_SCALE			"14"
	
	#define FRAGMENT_PARAM_VELOCITY_SCALE			"15"
	#define FRAGMENT_PARAM_DISTORTION_SCALE			"16"
	
	#define FRAGMENT_PARAM_SHADOW_SAMPLE1			"17"
	#define FRAGMENT_PARAM_SHADOW_SAMPLE2			"18"
	#define FRAGMENT_PARAM_SHADOW_MAP_SCALE1		"19"
	#define FRAGMENT_PARAM_SHADOW_MAP_SCALE2		"20"
	#define FRAGMENT_PARAM_SHADOW_MAP_SCALE3		"21"
	#define FRAGMENT_PARAM_SHADOW_MAP_OFFSET1		"22"
	#define FRAGMENT_PARAM_SHADOW_MAP_OFFSET2		"23"
	#define FRAGMENT_PARAM_SHADOW_MAP_OFFSET3		"24"
	#define FRAGMENT_PARAM_SHADOW_VIEW_DIRECTION	"25"
	
	#define FRAGMENT_PARAM_IMPOSTOR_SHADOW_BLEND	"26"
	#define FRAGMENT_PARAM_IMPOSTOR_SHADOW_SCALE	"27"
	#define FRAGMENT_PARAM_IMPOSTOR_DISTANCE		"28"
	
	#define FRAGMENT_PARAM_COUNT					"29"
	
	
	class ShaderSignature
	{
		private:
			
			const unsigned_int32	*signature; 
		
		public: 
			 
			ShaderSignature(const unsigned_int32 *sig) 
			{
				signature = sig; 
			}
			
			unsigned_int32 operator [](machine k) const
			{ 
				return (signature[k]);
			}
			
			friend bool operator ==(const ShaderSignature& x, const ShaderSignature& y); 
	};
	
	
	class FragmentProgram : public Render::FragmentProgramObject, public Shared, public HashTableElement<FragmentProgram>, public LinkTarget<FragmentProgram>
	{
		public:
			
			typedef ShaderSignature KeyType;
		
		private:
			
			static HashTable<FragmentProgram>	*hashTable;
			static char							hashTableStorage[sizeof(HashTable<FragmentProgram>)];
			
			unsigned_int32		shaderSignature[1];
			
			FragmentProgram(const char *source, unsigned_int32 size, bool programFlag, const unsigned_int32 *signature);
			
			#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

			#endif //]
			
			~FragmentProgram();
		
		public:
			
			static const char copyLightColor[];
			static const char copyVertexColor[];
			
			FragmentProgram(const char *source);
			
			KeyType GetKey(void) const
			{
				return (ShaderSignature(shaderSignature));
			}
			
			static unsigned_int32 Hash(const KeyType& key);
			
			static void Initialize(void);
			static void Terminate(void);
			
			static FragmentProgram *Get(const unsigned_int32 *signature);
			static FragmentProgram *New(const char *source, unsigned_int32 size, bool programFlag, const unsigned_int32 *signature);
			
			static void Flush(void);
			
			#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

			#endif //]
	};
}


#endif

// ZYURVUR
