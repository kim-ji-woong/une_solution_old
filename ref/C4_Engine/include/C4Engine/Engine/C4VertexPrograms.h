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


#ifndef C4VertexPrograms_h
#define C4VertexPrograms_h


#include "C4FragmentPrograms.h"


namespace C4
{
	enum
	{
		kVertexParamMatrixMVP					= 0,		// Model-view-projection matrix
		kVertexParamMatrixVelocityA				= 4,		// Object to viewport transform, previous frame
		kVertexParamMatrixVelocityB				= 8,		// Object to viewport transform, current frame
		kVertexParamMatrixWorld					= 12,		// Object to world transform
		kVertexParamMatrixCamera				= 15,		// Object to camera transform
		kVertexParamMatrixLight					= 18,		// Object to light transform
		kVertexParamMatrixSpace					= 21,		// Object to space transform
		kVertexParamMatrixShadow				= 24,		// Object to shadow transform A
		
		kVertexParamCameraPosition				= 27,		// Object-space camera position
		kVertexParamCameraRight					= 28,		// Object-space camera right direction
		kVertexParamCameraDown					= 29,		// Object-space camera down direction
		
		kVertexParamViewportTransform			= 30,		// Viewport (w/2, h/2, w/2 + l, h/2 + b)
		
		kVertexParamLightPosition				= 31,		// Object-space light position
		kVertexParamLightRange					= 32,		// Light range (r, 0.0, 0.0, 1 / r)
		kVertexParamAmbientPlane				= 33,		// Ambient gradient base plane
		
		kVertexParamFogPlane					= 34,		// Object-space fog plane
		kVertexParamFogParams					= 35,		// (F dot C, F dot C <= 0, sgn(F dot C), density / ln 2)
		
		kVertexParamShaderTime					= 36,		// (absolute time, delta time, unused, 0.0)
		kVertexParamFireParams					= 37,		// Fire params (intensity, 0.0, 0.0, 0.0)
		
		kVertexParamTexcoordGenerate			= 38,		// (x scale, y scale, 0.0, 0.0)
		kVertexParamTexcoordTransform0			= 39,		// (x scale, y scale, x offset, y offset)
		kVertexParamTexcoordTransform1			= 40,		// (x scale, y scale, x offset, y offset)
		kVertexParamTexcoordVelocity0			= 41,		// (v1.x, v1.y, 0.0, 0.0) or (v1.x, v1.y, v2.x, v2.y)
		kVertexParamTexcoordVelocity1			= 42,		// (v2.x, v2.y, 0.0, 0.0) or (v3.x, v3.y, 0.0, 0.0)
		
		kVertexParamTerrainTexcoordScale		= 43,		// (scale, 0.0, 0.0, 0.0)
		kVertexParamTerrainParameter0			= 44,		// Terrain border parameters, positive faces
		kVertexParamTerrainParameter1			= 45,		// Terrain border parameters, negative faces
		
		kVertexParamShadowSectionPlane1			= 46,		// Shadow section 0-1 blend plane
		kVertexParamShadowSectionPlane2			= 47,		// Shadow section 1-2 blend plane
		kVertexParamShadowSectionPlane3			= 48,		// Shadow section 2-3 blend plane
		
		kVertexParamSpaceScale					= 49,		// Reciprocal ambient space size (x, y, z, 0.0)
		kVertexParamVertexScaleOffset			= 50,		// (scale.x, scale.y, scale.z, offset)
		kVertexParamRadiusPointFactor			= 51,		// Radius-to-point-size factor
		kVertexParamPointCameraPlane			= 52,		// World-space camera plane over radius-to-point-size factor
		kVertexParamDistortCameraPlane			= 53,		// Object-space camera plane over focal length
		kVertexParamReflectionScale				= 54,		// (reflection offset scale, 0.0, 0.0, 0.0)
		kVertexParamRefractionScale				= 55,		// (refraction offset scale, 0.0, 0.0, 0.0)
		
		kVertexParamImpostorCameraPosition		= 56,		// Camera position used for impostor transitions
		kVertexParamImpostorTransition			= 57,		// (transition scale, transition bias, 0.0, 0.0)
		kVertexParamImpostorDepth				= 58,		// (impostor depth scale, impostor depth offset, tan(elevation), 0.0)
		kVertexParamImpostorPlaneS				= 59,		// Geometry noisy blend impostor s-ccordinate generation plane
		kVertexParamImpostorPlaneT				= 60,		// Geometry noisy blend impostor t-ccordinate generation plane
		
		kVertexParamPaintPlaneS					= 61,		// Paint space s-ccordinate generation plane
		kVertexParamPaintPlaneT					= 62		// Paint space t-ccordinate generation plane
	};
	
	
	#define VERTEX_PARAM_MATRIX_MVP0				"0"
	#define VERTEX_PARAM_MATRIX_MVP1				"1"
	#define VERTEX_PARAM_MATRIX_MVP2				"2"
	#define VERTEX_PARAM_MATRIX_MVP3				"3"
	
	#define VERTEX_PARAM_MATRIX_VELOCITY_A0			"4"
	#define VERTEX_PARAM_MATRIX_VELOCITY_A1			"5"
	#define VERTEX_PARAM_MATRIX_VELOCITY_A2			"6"
	#define VERTEX_PARAM_MATRIX_VELOCITY_A3			"7"
	
	#define VERTEX_PARAM_MATRIX_VELOCITY_B0			"8"
	#define VERTEX_PARAM_MATRIX_VELOCITY_B1			"9"
	#define VERTEX_PARAM_MATRIX_VELOCITY_B2			"10"
	#define VERTEX_PARAM_MATRIX_VELOCITY_B3			"11"
	
	#define VERTEX_PARAM_MATRIX_WORLD0				"12"
	#define VERTEX_PARAM_MATRIX_WORLD1				"13"
	#define VERTEX_PARAM_MATRIX_WORLD2				"14"
	
	#define VERTEX_PARAM_MATRIX_CAMERA0				"15"
	#define VERTEX_PARAM_MATRIX_CAMERA1				"16"
	#define VERTEX_PARAM_MATRIX_CAMERA2				"17"
	
	#define VERTEX_PARAM_MATRIX_LIGHT0				"18"
	#define VERTEX_PARAM_MATRIX_LIGHT1				"19"
	#define VERTEX_PARAM_MATRIX_LIGHT2				"20"
	
	#define VERTEX_PARAM_MATRIX_SPACE0				"21"
	#define VERTEX_PARAM_MATRIX_SPACE1				"22"
	#define VERTEX_PARAM_MATRIX_SPACE2				"23"
	
	#define VERTEX_PARAM_MATRIX_SHADOW0				"24"
	#define VERTEX_PARAM_MATRIX_SHADOW1				"25" 
	#define VERTEX_PARAM_MATRIX_SHADOW2				"26"
	 
	#define VERTEX_PARAM_CAMERA_POSITION			"27" 
	#define VERTEX_PARAM_CAMERA_RIGHT				"28" 
	#define VERTEX_PARAM_CAMERA_DOWN				"29"
	 
	#define VERTEX_PARAM_VIEWPORT_TRANSFORM			"30"
	
	#define VERTEX_PARAM_LIGHT_POSITION				"31"
	#define VERTEX_PARAM_LIGHT_RANGE				"32" 
	#define VERTEX_PARAM_AMBIENT_PLANE				"33"
	
	#define VERTEX_PARAM_FOG_PLANE					"34"
	#define VERTEX_PARAM_FOG_PARAMS					"35" 
	
	#define VERTEX_PARAM_SHADER_TIME				"36"
	#define VERTEX_PARAM_FIRE_PARAMS				"37"
	
	#define VERTEX_PARAM_TEXCOORD_GENERATE			"38"
	#define VERTEX_PARAM_TEXCOORD_TRANSFORM0		"39"
	#define VERTEX_PARAM_TEXCOORD_TRANSFORM1		"40"
	#define VERTEX_PARAM_TEXCOORD_VELOCITY0			"41"
	#define VERTEX_PARAM_TEXCOORD_VELOCITY1			"42"
	
	#define VERTEX_PARAM_TERRAIN_TEXCOORD_SCALE		"43"
	#define VERTEX_PARAM_TERRAIN_PARAMETER0			"44"
	#define VERTEX_PARAM_TERRAIN_PARAMETER1			"45"
	
	#define VERTEX_PARAM_SHADOW_SECTION_PLANE1		"46"
	#define VERTEX_PARAM_SHADOW_SECTION_PLANE2		"47"
	#define VERTEX_PARAM_SHADOW_SECTION_PLANE3		"48"
	
	#define VERTEX_PARAM_SPACE_SCALE				"49"
	#define VERTEX_PARAM_VERTEX_SCALE_OFFSET		"50"
	#define VERTEX_PARAM_RADIUS_POINT_FACTOR		"51"
	#define VERTEX_PARAM_POINT_CAMERA_PLANE			"52"
	#define VERTEX_PARAM_DISTORT_CAMERA_PLANE		"53"
	#define VERTEX_PARAM_REFLECTION_SCALE			"54"
	#define VERTEX_PARAM_REFRACTION_SCALE			"55"
	
	#define VERTEX_PARAM_IMPOSTOR_CAMERA_POSITION	"56"
	#define VERTEX_PARAM_IMPOSTOR_TRANSITION		"57"
	#define VERTEX_PARAM_IMPOSTOR_DEPTH				"58"
	#define VERTEX_PARAM_IMPOSTOR_PLANE_S			"59"
	#define VERTEX_PARAM_IMPOSTOR_PLANE_T			"60"
	
	#define VERTEX_PARAM_PAINT_PLANE_S				"61"
	#define VERTEX_PARAM_PAINT_PLANE_T				"62"
	
	#define VERTEX_PARAM_COUNT						"63"
	
	
	enum
	{
		kMaxVertexSnippetCount		= 32
	};
	
	
	enum
	{
		kVertexSnippetOutputObjectPosition,
		kVertexSnippetOutputObjectNormal,
		kVertexSnippetOutputObjectTangent,
		kVertexSnippetOutputObjectBitangent,
		kVertexSnippetOutputWorldPosition,
		kVertexSnippetOutputWorldNormal,
		kVertexSnippetOutputWorldTangent,
		kVertexSnippetOutputWorldBitangent,
		kVertexSnippetOutputCameraNormal,
		kVertexSnippetOutputVertexGeometry,
	
		kVertexSnippetOutputObjectInfiniteLightDirection,
		kVertexSnippetCalculateObjectPointLightDirection,
		kVertexSnippetOutputObjectPointLightDirection,
		kVertexSnippetOutputTangentInfiniteLightDirection,
		kVertexSnippetOutputTangentPointLightDirection,
	
		kVertexSnippetCalculateObjectViewDirection,
		kVertexSnippetOutputObjectViewDirection,
		kVertexSnippetOutputTangentViewDirection,
		kVertexSnippetOutputTangentViewFogDirection,
		kVertexSnippetOutputAlternateViewFogDirection,
		
		kVertexSnippetOutputBillboardInfiniteLightDirection,
		kVertexSnippetOutputBillboardPointLightDirection,
		
		kVertexSnippetCalculateTerrainTangentData,
		kVertexSnippetOutputTerrainInfiniteLightDirection,
		kVertexSnippetOutputTerrainPointLightDirection,
		kVertexSnippetOutputTerrainViewDirection,
		kVertexSnippetOutputTerrainWorldTangentFrame,
		
		kVertexSnippetOutputRawTexcoords,
		kVertexSnippetOutputTerrainTexcoords,
		kVertexSnippetOutputImpostorTexcoords,
		kVertexSnippetOutputImpostorTransitionBlend,
		kVertexSnippetOutputGeometryImpostorTexcoords,
		kVertexSnippetOutputPaintTexcoords,
	
		kVertexSnippetOutputFireTexcoords,
		kVertexSnippetOutputFireArrayTexcoords,
	
		kVertexSnippetCalculateCameraDistance,
		kVertexSnippetOutputCameraWarpFunction,
		kVertexSnippetOutputCameraBumpWarpFunction,
		
		kVertexSnippetOutputDistortionDepth,
		
		kVertexSnippetOutputImpostorDepth,
		kVertexSnippetOutputImpostorRadius,
		kVertexSnippetOutputImpostorShadowRadius,
	
		kVertexSnippetOutputPointLightAttenuation,
		kVertexSnippetOutputSpotLightAttenuation,
		kVertexSnippetOutputDepthProjectTexcoord,
		kVertexSnippetOutputLandscapeProjectTexcoord,
		kVertexSnippetOutputCubeProjectTexcoord,
		kVertexSnippetOutputSpotProjectTexcoord,
		
		kVertexSnippetOutputAmbientGradientDistance,
		kVertexSnippetOutputAmbientSpaceVector,
	
		kVertexSnippetOutputFiniteConstantFogFactors,
		kVertexSnippetOutputInfiniteConstantFogFactors,
		kVertexSnippetOutputFiniteLinearFogFactors,
		kVertexSnippetOutputInfiniteLinearFogFactors,
	
		kVertexSnippetMotionBlurTransform,
		kVertexSnippetDeformMotionBlurTransform,
		kVertexSnippetVelocityMotionBlurTransform,
		kVertexSnippetInfiniteMotionBlurTransform,
		
		kVertexSnippetCount
	};
	
	
	enum
	{
		kVertexSnippetPositionFlag		= 1 << 0,
		kVertexSnippetNormalFlag		= 1 << 1,
		kVertexSnippetTangentFlag		= 1 << 2
	};
	
	
	struct VertexSnippet
	{
		Type				signature;
		unsigned_int32		flags;
		
		const char			*programCode;
		const char			*shaderCode;
	};
	
	
	struct VertexAssembly
	{
		unsigned_int32			*signatureStorage;
		const VertexSnippet		*vertexSnippet[kMaxVertexSnippetCount];
		
		VertexAssembly(unsigned_int32 *storage)
		{
			signatureStorage = storage;
			storage[0] = 0;
		}
		
		void AddSnippet(const VertexSnippet *snippet)
		{
			unsigned_int32 count = signatureStorage[0];
			Assert(count < kMaxVertexSnippetCount, "Vertex snippet table overflow");
			
			vertexSnippet[count] = snippet;
			signatureStorage[++count] = snippet->signature;
			signatureStorage[0] = count;
		}
	};
	
	
	class VertexProgram : public Render::VertexProgramObject, public Shared, public HashTableElement<VertexProgram>, public LinkTarget<VertexProgram>
	{
		public:
			
			typedef ShaderSignature KeyType;
		
		private:
			
			static HashTable<VertexProgram>		*hashTable;
			static char							hashTableStorage[sizeof(HashTable<VertexProgram>)];
			
			unsigned_int32		shaderSignature[1];
			
			VertexProgram(const char *source, unsigned_int32 size, const unsigned_int32 *signature);
			
			#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

			#endif //]
			
			~VertexProgram();
			
		public:
			
			static const VertexSnippet		nullTransform;
			static const VertexSnippet		modelviewProjectTransform;
			static const VertexSnippet		modelviewProjectTransformInfinite;
			static const VertexSnippet		modelviewProjectTransformHomogeneous;
			
			static const VertexSnippet		calculateCameraDirection;
			static const VertexSnippet		calculateCameraDirection4D;
			static const VertexSnippet		scaleVertexCalculateCameraDirection;
			static const VertexSnippet		scaleVertexCalculateCameraDirection4D;
			
			static const VertexSnippet		calculateBillboardPosition;
			static const VertexSnippet		calculateBillboardScalePosition;
			static const VertexSnippet		calculateVertexBillboardPosition;
			static const VertexSnippet		calculateVertexBillboardScalePosition;
			static const VertexSnippet		calculateLightedBillboardPosition;
			static const VertexSnippet		calculatePostboardPosition;
			static const VertexSnippet		calculatePostboardScalePosition;
			static const VertexSnippet		calculatePolyboardNormal;
			static const VertexSnippet		calculateLinearPolyboardNormal;
			static const VertexSnippet		calculatePolyboardPosition;
			static const VertexSnippet		calculatePolyboardScalePosition;
			
			static const VertexSnippet		calculateScalePosition;
			static const VertexSnippet		calculateScaleOffsetPosition;
			static const VertexSnippet		calculateExpandNormalPosition;
			
			static const VertexSnippet		calculateTerrainBorderPosition;
			static const VertexSnippet		calculateWaterHeightPosition;
			
			static const VertexSnippet		texcoordVertexTransform;
			static const VertexSnippet		extractGlowTransform;
			static const VertexSnippet		postProcessTransform;
			
			static const VertexSnippet		shadowInfiniteExtrusionTransform;
			static const VertexSnippet		shadowPointExtrusionTransform;
			static const VertexSnippet		shadowEndcapProjectionTransform;
			
			static const VertexSnippet		outputPrimaryColor;
			static const VertexSnippet		outputSecondaryColor;
			static const VertexSnippet		outputPointSize;
			static const VertexSnippet		outputInfinitePointSize;
			
			static const VertexSnippet		copyPrimaryTexcoord0;
			static const VertexSnippet		copyPrimaryTexcoord1;
			static const VertexSnippet		copySecondaryTexcoord1;
			static const VertexSnippet		transformPrimaryTexcoord0;
			static const VertexSnippet		transformPrimaryTexcoord1;
			static const VertexSnippet		transformSecondaryTexcoord1;
			static const VertexSnippet		animatePrimaryTexcoord0;
			static const VertexSnippet		animatePrimaryTexcoord1;
			static const VertexSnippet		animateSecondaryTexcoord1;
			static const VertexSnippet		transformAnimatePrimaryTexcoord0;
			static const VertexSnippet		transformAnimatePrimaryTexcoord1;
			static const VertexSnippet		transformAnimateSecondaryTexcoord1;
			static const VertexSnippet		generateTexcoord0;
			static const VertexSnippet		generateTexcoord1;
			static const VertexSnippet		generateTransformTexcoord0;
			static const VertexSnippet		generateTransformTexcoord1;
			static const VertexSnippet		generateBaseTexcoord;
			static const VertexSnippet		generateAnimateTexcoord0;
			static const VertexSnippet		generateAnimateTexcoord1;
			static const VertexSnippet		generateTransformAnimateTexcoord0;
			static const VertexSnippet		generateTransformAnimateTexcoord1;
			
			static const VertexSnippet		normalizeNormal;
			static const VertexSnippet		normalizeTangent;
			static const VertexSnippet		orthonormalizeTangent;
			static const VertexSnippet		generateTangent;
			static const VertexSnippet		generateImpostorFrame;
			static const VertexSnippet		calculateBitangent;
			static const VertexSnippet		adjustBitangent;
			
			static const VertexSnippet		vertexSnippet[kVertexSnippetCount];
			
			KeyType GetKey(void) const
			{
				return (ShaderSignature(shaderSignature));
			}
			
			static unsigned_int32 Hash(const KeyType& key);
			
			static void Initialize(void);
			static void Terminate(void);
			
			static VertexProgram *Get(const unsigned_int32 *signature);
			static VertexProgram *New(const VertexAssembly *assembly);
			static VertexProgram *New(const char *source, unsigned_int32 size, const unsigned_int32 *signature);
			
			static void Flush(void);
			
			#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

			#endif //]
	};
}


#endif

// ZYURVUR
