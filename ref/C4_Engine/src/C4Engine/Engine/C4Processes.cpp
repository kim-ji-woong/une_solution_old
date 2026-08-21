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


#include "C4Horizon.h"
#include "C4Graphics.h"
#include "C4SpaceObjects.h"
#include "C4Portals.h"
#include "C4Configuration.h"


using namespace C4;


#if C4OPENGL

	#define RESULT_COLOR			"gl_FragColor"
	#define RESULT_DEPTH			"gl_FragDepth"
	#define FRAGMENT_POSITION		"gl_FragCoord"
	#define FRAGMENT_COLOR			"gl_Color"
	#define FRAGMENT_COLOR1			"gl_SecondaryColor"
	
	#define FLOAT2					"vec2"
	#define FLOAT3					"vec3"
	#define FLOAT4					"vec4"
	#define HALF3					"vec3"
	#define DDX						"dFdx"
	#define DDY						"dFdy"
	#define LERP					"mix"
	#define FRAC					"fract"

#else

	#define RESULT_COLOR			"result.color"
	#define RESULT_DEPTH			"result.depth"
	#define FRAGMENT_POSITION		"fragment.position"
	#define FRAGMENT_COLOR			"fragment.color"
	#define FRAGMENT_COLOR1			"fragment.color1"
	
	#define FLOAT2					"float2"
	#define FLOAT3					"float3"
	#define FLOAT4					"float4"
	#define HALF3					"half3"
	#define DDX						"ddx"
	#define DDY						"ddy"
	#define LERP					"lerp"
	#define FRAC					"frac"

#endif


const unsigned_int8 Route::swizzleTable[26] =
{
	3, 2, 4, 4, 4, 4, 1, 4, 4, 4, 4, 4, 4, 4, 4, 2, 3, 0, 0, 1, 4, 4, 3, 0, 1, 2
};


const char *const ConstantProcess::constantIdentifier[2][kMaxShaderConstantCount] =
{
	{"param[" FRAGMENT_PARAM_CONSTANT0 "]", "param[" FRAGMENT_PARAM_CONSTANT1 "]", "param[" FRAGMENT_PARAM_CONSTANT2 "]", "param[" FRAGMENT_PARAM_CONSTANT3 "]",
	 "param[" FRAGMENT_PARAM_CONSTANT4 "]", "param[" FRAGMENT_PARAM_CONSTANT5 "]", "param[" FRAGMENT_PARAM_CONSTANT6 "]", "param[" FRAGMENT_PARAM_CONSTANT7 "]"},
	 
	{"program.env[" FRAGMENT_PARAM_CONSTANT0 "]", "program.env[" FRAGMENT_PARAM_CONSTANT1 "]", "program.env[" FRAGMENT_PARAM_CONSTANT2 "]", "program.env[" FRAGMENT_PARAM_CONSTANT3 "]",
	 "program.env[" FRAGMENT_PARAM_CONSTANT4 "]", "program.env[" FRAGMENT_PARAM_CONSTANT5 "]", "program.env[" FRAGMENT_PARAM_CONSTANT6 "]", "program.env[" FRAGMENT_PARAM_CONSTANT7 "]"}
};

ShaderData::ShaderStateFunc *const ConstantProcess::scalarStateFunction[kMaxShaderConstantCount] =
{
	&StateFunc_LoadScalar0, &StateFunc_LoadScalar1, &StateFunc_LoadScalar2, &StateFunc_LoadScalar3,
	&StateFunc_LoadScalar4, &StateFunc_LoadScalar5, &StateFunc_LoadScalar6, &StateFunc_LoadScalar7
};

ShaderData::ShaderStateFunc *const ConstantProcess::vectorStateFunction[kMaxShaderConstantCount] =
{
	&StateFunc_LoadVector0, &StateFunc_LoadVector1, &StateFunc_LoadVector2, &StateFunc_LoadVector3,
	&StateFunc_LoadVector4, &StateFunc_LoadVector5, &StateFunc_LoadVector6, &StateFunc_LoadVector7
};


namespace C4
{
	template <> Heap Memory<Process>::heap("Process", 65536, kHeapMutexless);
	template class Memory<Process>;
}


ProcessRegistration::ProcessRegistration(ProcessType type, const char *name, ProcessGroup group) : Registration<Process, ProcessRegistration>(type)
{
	processName = name;
	processGroup = group;
}

ProcessRegistration::~ProcessRegistration()
{
}


Route::Route(Process *start, Process *finish, int32 port) : GraphEdge<Process, Route>(start, finish)
{
	routeFlags = 0;
	routePort = port;
	routeNegation = false;
	routeSwizzle = 'xyzw';
}

Route::Route(const Route& route, Process *start, Process *finish) : GraphEdge<Process, Route>(start, finish) 
{
	routeFlags = route.routeFlags; 
	routePort = route.routePort; 
	routeNegation = route.routeNegation; 
	routeSwizzle = route.routeSwizzle;
} 

Route::~Route()
{
} 

void Route::Pack(Packer& data, unsigned_int32 packFlags) const
{
	data << ChunkHeader('FLAG', 4); 
	data << routeFlags;
	
	data << ChunkHeader('PORT', 4);
	data << routePort;
	
	data << ChunkHeader('NEGA', 4);
	data << routeNegation;
	
	data << ChunkHeader('SWIZ', 4);
	data << routeSwizzle;
	
	data << TerminatorChunk;
}

void Route::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<Route>(data, unpackFlags);
}

bool Route::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'FLAG':
			
			data >> routeFlags;
			return (true);
		
		case 'PORT':
			
			data >> routePort;
			return (true);
		
		case 'NEGA':
			
			data >> routeNegation;
			return (true);
		
		case 'SWIZ':
			
			data >> routeSwizzle;
			return (true);
	}
	
	return (false);
}

bool Route::SwizzleFilter(unsigned_int32 code)
{
	code -= 'A';
	if (code < 26U) return (swizzleTable[code] < 4);
	
	code -= 0x0020;
	if (code < 26U) return (swizzleTable[code] < 4);
	
	return (false);
}

int32 Route::GetSettingCount(void) const
{
	return (2);
}

Setting *Route::GetSetting(int32 index) const
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('ROUT', 'NEGA'));
		return (new BooleanSetting('NEGA', routeNegation, title));
	}
	else if (index == 1)
	{
		const char *title = table->GetString(StringID('ROUT', 'SWIZ'));
		
		String<4> string = Text::TypeToString(routeSwizzle);
		char c = string[0];
		if ((string[1] == c) && (string[2] == c) && (string[3] == c)) string[1] = 0;
		
		return (new TextSetting('SWIZ', string, title, 4, &SwizzleFilter));
	}
	
	return (nullptr);
}

void Route::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'NEGA')
	{
		routeNegation = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
	}
	else if (identifier == 'SWIZ')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		unsigned_int32 swizzle = *text++;
		if (swizzle != 0)
		{
			if (swizzle < 'a') swizzle += 32;
			unsigned_int32 last = swizzle;
			
			for (machine a = 0; a < 3; a++)
			{
				unsigned_int32 c = *text;
				if (c != 0)
				{
					if (c < 'a') c += 32;
					swizzle = (swizzle << 8) | c;
					last = c;
					text++;
				}
				else
				{
					swizzle = (swizzle << 8) | last;
				}
			}
			
			routeSwizzle = swizzle;
		}
		else
		{
			routeSwizzle = 'xyzw';
		}
	}
}

bool Route::operator ==(const Route& route) const
{
	return ((routePort == route.routePort) && (routeNegation == route.routeNegation) && (routeSwizzle == route.routeSwizzle));
}

int32 Route::GenerateOutputSize(void) const
{
	unsigned_int8 c1 = swizzleTable[(routeSwizzle >> 24) - 'a'];
	unsigned_int8 c2 = swizzleTable[((routeSwizzle >> 16) & 0xFF) - 'a'];
	unsigned_int8 c3 = swizzleTable[((routeSwizzle >> 8) & 0xFF) - 'a'];
	unsigned_int8 c4 = swizzleTable[(routeSwizzle & 0xFF) - 'a'];
	
	if ((c1 == c2) && (c1 == c3) && (c1 == c4)) return (1);
	
	return (GetStartElement()->GetProcessData()->outputSize);
}

int32 Route::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	swizzleData->negate ^= routeNegation & !swizzleData->absolute;
	
	const Process *process = GetStartElement();
	int32 maxComponent = process->GetProcessData()->outputSize - 1;
	
	int32 size = swizzleData->size;
	for (machine a = 0; a < size; a++)
	{
		unsigned_int32 c = (routeSwizzle >> (24 - swizzleData->component[a] * 8)) & 0xFF;
		swizzleData->component[a] = Min(swizzleTable[c - 'a'], maxComponent);
	}
	
	return (process->GenerateOutputIdentifier(compileData, allocData, swizzleData, name));
}

unsigned_int32 Route::GenerateRouteSignature(void) const
{
	unsigned_int8 c1 = swizzleTable[(routeSwizzle >> 24) - 'a'];
	unsigned_int8 c2 = swizzleTable[((routeSwizzle >> 16) & 0xFF) - 'a'];
	unsigned_int8 c3 = swizzleTable[((routeSwizzle >> 8) & 0xFF) - 'a'];
	unsigned_int8 c4 = swizzleTable[(routeSwizzle & 0xFF) - 'a'];
	
	return ((routePort << 16) | (routeNegation << 8) | (c1 << 6) | (c2 << 4) | (c3 << 2) | c4);
}


Process::Process(ProcessType type)
{
	processType = type;
	baseProcessType = 0;
	
	processFlags = 0;
	
	processPosition.Set(0.0F, 0.0F);
}

Process::Process(const Process& process) : processComment(process.processComment)
{
	processType = process.processType;
	baseProcessType = process.baseProcessType;
	
	processFlags = process.processFlags;
	
	processPosition = process.processPosition;
}

Process::~Process()
{
}

Process *Process::New(ProcessType type)
{
	Type	data[2];
	
	switch (type)
	{
		case kProcessRawTexcoord:
			
			return (new RawTexcoordProcess);
		
		case kProcessImpostorTexcoord:
			
			return (new ImpostorTexcoordProcess);
		
		case kProcessImpostorBlend:
			
			return (new ImpostorBlendProcess);
		
		case kProcessTerrainTexcoord:
			
			return (new TerrainTexcoordProcess);
		
		case kProcessTriplanarBlend:
			
			return (new TriplanarBlendProcess);
		
		case kProcessTerrainLightDirection:
			
			return (new TerrainLightDirectionProcess);
		
		case kProcessTerrainViewDirection:
			
			return (new TerrainViewDirectionProcess);
		
		case kProcessTerrainHalfwayDirection:
			
			return (new TerrainHalfwayDirectionProcess);
	}
	
	data[0] = type;
	data[1] = 0;
	
	Unpacker unpacker(data);
	return (Construct(unpacker));
}

bool Process::ValidShader(ProcessType type, int32 shader)
{
	return (true);
}

void Process::RegisterStandardProcesses(void)
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	static ProcessReg<SectionProcess> sectionRegistration(kProcessSection, "");
	
	static ProcessReg<ScalarProcess> scalarRegistration(kProcessScalar, table->GetString(StringID('PROC', kProcessScalar)), 'BASC');
	static ProcessReg<VectorProcess> vectorRegistration(kProcessVector, table->GetString(StringID('PROC', kProcessVector)), 'BASC');
	static ProcessReg<ColorProcess> colorRegistration(kProcessColor, table->GetString(StringID('PROC', kProcessColor)), 'BASC');
	static ProcessReg<DetailLevelProcess> detailLevelRegistration(kProcessDetailLevel, table->GetString(StringID('PROC', kProcessDetailLevel)), 'BASC');
	static ProcessReg<TimeProcess> timeRegistration(kProcessTime, table->GetString(StringID('PROC', kProcessTime)), 'BASC');
	static ProcessReg<TextureMapProcess> textureMapRegistration(kProcessTextureMap, table->GetString(StringID('PROC', kProcessTextureMap)), 'BASC');
	static ProcessReg<NormalMapProcess> normalMapRegistration(kProcessNormalMap, table->GetString(StringID('PROC', kProcessNormalMap)), 'BASC');
	static ProcessReg<ImpostorTextureProcess> impostorTextureRegistration(kProcessImpostorTexture, table->GetString(StringID('PROC', kProcessImpostorTexture)), 'BASC');
	static ProcessReg<ImpostorNormalProcess> impostorNormalRegistration(kProcessImpostorNormal, table->GetString(StringID('PROC', kProcessImpostorNormal)), 'BASC');
	static ProcessReg<TerrainTextureProcess> terrainTextureRegistration(kProcessTerrainTexture, table->GetString(StringID('PROC', kProcessTerrainTexture)), 'BASC');
	static ProcessReg<TerrainNormalProcess> terrainNormalRegistration(kProcessTerrainNormal, table->GetString(StringID('PROC', kProcessTerrainNormal)), 'BASC');
	static ProcessReg<TerrainNormal2Process> terrainNormal2Registration(kProcessTerrainNormal2, table->GetString(StringID('PROC', kProcessTerrainNormal2)), 'BASC');
	static ProcessReg<TerrainNormal3Process> terrainNormal3Registration(kProcessTerrainNormal3, table->GetString(StringID('PROC', kProcessTerrainNormal3)), 'BASC');
	static ProcessReg<PaintTextureProcess> paintTextureRegistration(kProcessPaintTexture, table->GetString(StringID('PROC', kProcessPaintTexture)), 'BASC');
	static ProcessReg<Merge2Process> merge2Registration(kProcessMerge2, table->GetString(StringID('PROC', kProcessMerge2)), 'BASC');
	static ProcessReg<Merge3Process> merge3Registration(kProcessMerge3, table->GetString(StringID('PROC', kProcessMerge3)), 'BASC');
	static ProcessReg<Merge4Process> merge4Registration(kProcessMerge4, table->GetString(StringID('PROC', kProcessMerge4)), 'BASC');
	
	static ProcessReg<Texcoord0Process> texcoord0Registration(kProcessTexcoord0, table->GetString(StringID('PROC', kProcessTexcoord0)), 'TERP');
	static ProcessReg<Texcoord1Process> texcoord1Registration(kProcessTexcoord1, table->GetString(StringID('PROC', kProcessTexcoord1)), 'TERP');
	static ProcessReg<PaintTexcoordProcess> paintTexcoordRegistration(kProcessPaintTexcoord, table->GetString(StringID('PROC', kProcessPaintTexcoord)), 'TERP');
	static ProcessReg<VertexColorProcess> vertexColorRegistration(kProcessVertexColor, table->GetString(StringID('PROC', kProcessVertexColor)), 'TERP');
	static ProcessReg<VertexGeometryProcess> vertexGeometryRegistration(kProcessVertexGeometry, table->GetString(StringID('PROC', kProcessVertexGeometry)), 'TERP');
	static ProcessReg<ObjectPositionProcess> objectPositionRegistration(kProcessObjectPosition, table->GetString(StringID('PROC', kProcessObjectPosition)), 'TERP');
	static ProcessReg<WorldPositionProcess> worldPositionRegistration(kProcessWorldPosition, table->GetString(StringID('PROC', kProcessWorldPosition)), 'TERP');
	static ProcessReg<ObjectNormalProcess> objectNormalRegistration(kProcessObjectNormal, table->GetString(StringID('PROC', kProcessObjectNormal)), 'TERP');
	static ProcessReg<ObjectTangentProcess> objectTangentRegistration(kProcessObjectTangent, table->GetString(StringID('PROC', kProcessObjectTangent)), 'TERP');
	static ProcessReg<ObjectBitangentProcess> objectBitangentRegistration(kProcessObjectBitangent, table->GetString(StringID('PROC', kProcessObjectBitangent)), 'TERP');
	static ProcessReg<WorldNormalProcess> worldNormalRegistration(kProcessWorldNormal, table->GetString(StringID('PROC', kProcessWorldNormal)), 'TERP');
	static ProcessReg<WorldTangentProcess> worldTangentRegistration(kProcessWorldTangent, table->GetString(StringID('PROC', kProcessWorldTangent)), 'TERP');
	static ProcessReg<WorldBitangentProcess> worldBitangentRegistration(kProcessWorldBitangent, table->GetString(StringID('PROC', kProcessWorldBitangent)), 'TERP');
	static ProcessReg<TangentLightDirectionProcess> tangentLightDirectionRegistration(kProcessTangentLightDirection, table->GetString(StringID('PROC', kProcessTangentLightDirection)), 'TERP');
	static ProcessReg<TangentViewDirectionProcess> tangentViewDirectionRegistration(kProcessTangentViewDirection, table->GetString(StringID('PROC', kProcessTangentViewDirection)), 'TERP');
	static ProcessReg<TangentHalfwayDirectionProcess> tangentHalfwayDirectionRegistration(kProcessTangentHalfwayDirection, table->GetString(StringID('PROC', kProcessTangentHalfwayDirection)), 'TERP');
	static ProcessReg<ObjectLightDirectionProcess> objectLightDirectionRegistration(kProcessObjectLightDirection, table->GetString(StringID('PROC', kProcessObjectLightDirection)), 'TERP');
	static ProcessReg<ObjectViewDirectionProcess> objectViewDirectionRegistration(kProcessObjectViewDirection, table->GetString(StringID('PROC', kProcessObjectViewDirection)), 'TERP');
	static ProcessReg<ObjectHalfwayDirectionProcess> objectHalfwayDirectionRegistration(kProcessObjectHalfwayDirection, table->GetString(StringID('PROC', kProcessObjectHalfwayDirection)), 'TERP');
	
	static ProcessReg<AbsoluteProcess> absoluteRegistration(kProcessAbsolute, table->GetString(StringID('PROC', kProcessAbsolute)), 'MATH');
	static ProcessReg<InvertProcess> invertRegistration(kProcessInvert, table->GetString(StringID('PROC', kProcessInvert)), 'MATH');
	static ProcessReg<ExpandProcess> expandRegistration(kProcessExpand, table->GetString(StringID('PROC', kProcessExpand)), 'MATH');
	static ProcessReg<ReciprocalProcess> reciprocalRegistration(kProcessReciprocal, table->GetString(StringID('PROC', kProcessReciprocal)), 'MATH');
	static ProcessReg<ReciprocalSquareRootProcess> reciprocalSquareRootRegistration(kProcessReciprocalSquareRoot, table->GetString(StringID('PROC', kProcessReciprocalSquareRoot)), 'MATH');
	static ProcessReg<SquareRootProcess> squareRootRegistration(kProcessSquareRoot, table->GetString(StringID('PROC', kProcessSquareRoot)), 'MATH');
	static ProcessReg<MagnitudeProcess> magnitudeRegistration(kProcessMagnitude, table->GetString(StringID('PROC', kProcessMagnitude)), 'MATH');
	static ProcessReg<NormalizeProcess> normalizeRegistration(kProcessNormalize, table->GetString(StringID('PROC', kProcessNormalize)), 'MATH');
	static ProcessReg<FloorProcess> floorRegistration(kProcessFloor, table->GetString(StringID('PROC', kProcessFloor)), 'MATH');
	static ProcessReg<RoundProcess> roundRegistration(kProcessRound, table->GetString(StringID('PROC', kProcessRound)), 'MATH');
	static ProcessReg<FractionProcess> fractionRegistration(kProcessFraction, table->GetString(StringID('PROC', kProcessFraction)), 'MATH');
	static ProcessReg<SaturateProcess> saturateRegistration(kProcessSaturate, table->GetString(StringID('PROC', kProcessSaturate)), 'MATH');
	static ProcessReg<SineProcess> sineRegistration(kProcessSine, table->GetString(StringID('PROC', kProcessSine)), 'MATH');
	static ProcessReg<CosineProcess> cosineRegistration(kProcessCosine, table->GetString(StringID('PROC', kProcessCosine)), 'MATH');
	static ProcessReg<Exp2Process> exp2Registration(kProcessExp2, table->GetString(StringID('PROC', kProcessExp2)), 'MATH');
	static ProcessReg<Log2Process> log2Registration(kProcessLog2, table->GetString(StringID('PROC', kProcessLog2)), 'MATH');
	
	static ProcessReg<AddProcess> addRegistration(kProcessAdd, table->GetString(StringID('PROC', kProcessAdd)), 'MATH');
	static ProcessReg<SubtractProcess> subtractRegistration(kProcessSubtract, table->GetString(StringID('PROC', kProcessSubtract)), 'MATH');
	static ProcessReg<AverageProcess> averageRegistration(kProcessAverage, table->GetString(StringID('PROC', kProcessAverage)), 'MATH');
	static ProcessReg<MultiplyProcess> multiplyRegistration(kProcessMultiply, table->GetString(StringID('PROC', kProcessMultiply)), 'MATH');
	static ProcessReg<DivideProcess> divideRegistration(kProcessDivide, table->GetString(StringID('PROC', kProcessDivide)), 'MATH');
	static ProcessReg<Dot3Process> dot3Registration(kProcessDot3, table->GetString(StringID('PROC', kProcessDot3)), 'MATH');
	static ProcessReg<Dot4Process> dot4Registration(kProcessDot4, table->GetString(StringID('PROC', kProcessDot4)), 'MATH');
	static ProcessReg<CrossProcess> crossRegistration(kProcessCross, table->GetString(StringID('PROC', kProcessCross)), 'MATH');
	static ProcessReg<MinimumProcess> minimumRegistration(kProcessMinimum, table->GetString(StringID('PROC', kProcessMinimum)), 'MATH');
	static ProcessReg<MaximumProcess> maximumRegistration(kProcessMaximum, table->GetString(StringID('PROC', kProcessMaximum)), 'MATH');
	static ProcessReg<SetLessThanProcess> setLessThanRegistration(kProcessSetLessThan, table->GetString(StringID('PROC', kProcessSetLessThan)), 'MATH');
	static ProcessReg<SetGreaterEqualProcess> setGreaterEqualRegistration(kProcessSetGreaterEqual, table->GetString(StringID('PROC', kProcessSetGreaterEqual)), 'MATH');
	static ProcessReg<PowerProcess> powerRegistration(kProcessPower, table->GetString(StringID('PROC', kProcessPower)), 'MATH');
	
	static ProcessReg<MultiplyAddProcess> multiplyAddRegistration(kProcessMultiplyAdd, table->GetString(StringID('PROC', kProcessMultiplyAdd)), 'MATH');
	static ProcessReg<LerpProcess> lerpRegistration(kProcessLerp, table->GetString(StringID('PROC', kProcessLerp)), 'MATH');
	
	static ProcessReg<DiffuseProcess> diffuseRegistration(kProcessDiffuse, table->GetString(StringID('PROC', kProcessDiffuse)), 'COMP');
	static ProcessReg<SpecularProcess> specularRegistration(kProcessSpecular, table->GetString(StringID('PROC', kProcessSpecular)), 'COMP');
	static ProcessReg<MicrofacetProcess> microfacetRegistration(kProcessMicrofacet, table->GetString(StringID('PROC', kProcessMicrofacet)), 'COMP');
	static ProcessReg<TerrainDiffuseProcess> terrainDiffuseRegistration(kProcessTerrainDiffuse, table->GetString(StringID('PROC', kProcessTerrainDiffuse)), 'COMP');
	static ProcessReg<TerrainSpecularProcess> terrainSpecularRegistration(kProcessTerrainSpecular, table->GetString(StringID('PROC', kProcessTerrainSpecular)), 'COMP');
	static ProcessReg<GenerateImpostorNormalProcess> generateImpostorNormalRegistration(kProcessGenerateImpostorNormal, table->GetString(StringID('PROC', kProcessGenerateImpostorNormal)), 'COMP');
	static ProcessReg<ImpostorDepthProcess> impostorDepthRegistration(kProcessImpostorDepth, table->GetString(StringID('PROC', kProcessImpostorDepth)), 'COMP');
	static ProcessReg<CombineNormalsProcess> combineNormalsRegistration(kProcessCombineNormals, table->GetString(StringID('PROC', kProcessCombineNormals)), 'COMP');
	static ProcessReg<FrontNormalProcess> frontNormalRegistration(kProcessFrontNormal, table->GetString(StringID('PROC', kProcessFrontNormal)), 'COMP');
	static ProcessReg<ReflectVectorProcess> reflectVectorRegistration(kProcessReflectVector, table->GetString(StringID('PROC', kProcessReflectVector)), 'COMP');
	static ProcessReg<LinearRampProcess> linearRampRegistration(kProcessLinearRamp, table->GetString(StringID('PROC', kProcessLinearRamp)), 'COMP');
	static ProcessReg<SmoothParameterProcess> smoothParameterRegistration(kProcessSmoothParameter, table->GetString(StringID('PROC', kProcessSmoothParameter)), 'COMP');
	static ProcessReg<SteepParameterProcess> steepParameterRegistration(kProcessSteepParameter, table->GetString(StringID('PROC', kProcessSteepParameter)), 'COMP');
	static ProcessReg<WorldTransformProcess> worldTransformRegistration(kProcessWorldTransform, table->GetString(StringID('PROC', kProcessWorldTransform)), 'COMP');
	static ProcessReg<DeltaDepthProcess> deltaDepthRegistration(kProcessDeltaDepth, table->GetString(StringID('PROC', kProcessDeltaDepth)), 'COMP');
	static ProcessReg<ParallaxProcess> parallaxRegistration(kProcessParallax, table->GetString(StringID('PROC', kProcessParallax)), 'COMP');
	static ProcessReg<HorizonProcess> horizonRegistration(kProcessHorizon, table->GetString(StringID('PROC', kProcessHorizon)), 'COMP');
	static ProcessReg<KillProcess> killRegistration(kProcessKill, table->GetString(StringID('PROC', kProcessKill)), 'COMP');
	static ProcessReg<ImpostorTransitionProcess> impostorTransitionRegistration(kProcessImpostorTransition, table->GetString(StringID('PROC', kProcessImpostorTransition)), 'COMP');
	static ProcessReg<GeometryTransitionProcess> geometryTransitionRegistration(kProcessGeometryTransition, table->GetString(StringID('PROC', kProcessGeometryTransition)), 'COMP');
	
	static ProcessReg<AmbientOutputProcess> ambientOutputRegistration(kProcessAmbientOutput, table->GetString(StringID('PROC', kProcessAmbientOutput)));
	static ProcessReg<AmbientAlphaOutputProcess> ambientAlphaOutputRegistration(kProcessAmbientAlphaOutput, table->GetString(StringID('PROC', kProcessAmbientAlphaOutput)));
	static ProcessReg<EmissionOutputProcess> emissionOutputRegistration(kProcessEmissionOutput, table->GetString(StringID('PROC', kProcessEmissionOutput)));
	static ProcessReg<ReflectionOutputProcess> reflectionOutputRegistration(kProcessReflectionOutput, table->GetString(StringID('PROC', kProcessReflectionOutput)));
	static ProcessReg<RefractionOutputProcess> refractionOutputRegistration(kProcessRefractionOutput, table->GetString(StringID('PROC', kProcessRefractionOutput)));
	static ProcessReg<EnvironmentOutputProcess> environmentOutputRegistration(kProcessEnvironmentOutput, table->GetString(StringID('PROC', kProcessEnvironmentOutput)));
	static ProcessReg<TerrainEnvironmentOutputProcess> terrainEnvironmentOutputRegistration(kProcessTerrainEnvironmentOutput, table->GetString(StringID('PROC', kProcessTerrainEnvironmentOutput)));
	static ProcessReg<GlowOutputProcess> glowOutputRegistration(kProcessGlowOutput, table->GetString(StringID('PROC', kProcessGlowOutput)));
	static ProcessReg<ImpostorDepthOutputProcess> impostorDepthOutputRegistration(kProcessImpostorDepthOutput, table->GetString(StringID('PROC', kProcessImpostorDepthOutput)));
	static ProcessReg<LightOutputProcess> lightOutputRegistration(kProcessLightOutput, table->GetString(StringID('PROC', kProcessLightOutput)));
	static ProcessReg<BloomOutputProcess> bloomOutputRegistration(kProcessBloomOutput, table->GetString(StringID('PROC', kProcessBloomOutput)));
	static ProcessReg<AlphaTestOutputProcess> alphaTestOutputRegistration(kProcessAlphaTestOutput, table->GetString(StringID('PROC', kProcessAlphaTestOutput)));
}

void Process::PackType(Packer& data) const
{
	data << processType;
}

void Process::Pack(Packer& data, unsigned_int32 packFlags) const
{
	data << ChunkHeader('FLAG', 4);
	data << processFlags;
	
	if (packFlags & kPackEditor)
	{
		data << ChunkHeader('POSI', sizeof(Point2D));
		data << processPosition;
		
		if (processComment.Length() != 0)
		{
			PackHandle handle = data.BeginChunk('CMNT');
			data << processComment;
			data.EndChunk(handle);
		}
	}
	
	data << TerminatorChunk;
}

void Process::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<Process>(data, unpackFlags);
}

bool Process::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'FLAG':
			
			data >> processFlags;
			return (true);
		
		case 'POSI':
			
			data >> processPosition;
			return (true);
		
		case 'CMNT':
			
			if (unpackFlags & kUnpackEditor)
			{
				data >> processComment;
				return (true);
			}
	}
	
	return (false);
}

void *Process::BeginSettingsUnpack(void)
{
	processComment.Clear();
	return (nullptr);
}

int32 Process::GetSettingCount(void) const
{
	return (1);
}

Setting *Process::GetSetting(int32 index) const
{
	if (index == 0)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		const char *title = table->GetString(StringID('PROC', 'CMNT'));
		const char *string = processComment;
		return (new TextSetting('CMNT', (string) ? string : "", title, 255));
	}
	
	return (nullptr);
}

void Process::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'CMNT')
	{
		processComment = static_cast<const TextSetting *>(setting)->GetText();
	}
}

bool Process::operator ==(const Process& process) const
{
	if (processType != process.processType) return (false);
	if (processFlags != process.processFlags) return (false);
	
	int32 portCount = GetPortCount();
	for (machine port = 0; port < portCount; port++)
	{
		const Route *route = GetPortRoute(port);
		const Route *processRoute = process.GetPortRoute(port);
		
		if (route)
		{
			if (!processRoute) return (false);
			if (route->GetStartElement()->GetProcessIndex() != processRoute->GetStartElement()->GetProcessIndex()) return (false);
			if (!(*route == *processRoute)) return (false);
		}
		else
		{
			if (processRoute) return (false);
		}
	}
	
	return (true);
}

Route *Process::GetPortRoute(int32 port) const
{
	Route *route = GetFirstIncomingEdge();
	while (route)
	{
		if (route->GetRoutePort() == port) return (route);
		route = route->GetNextIncomingEdge();
	}
	
	return (nullptr);
}

int32 Process::GetPortCount(void) const
{
	return (0);
}

unsigned_int32 Process::GetPortFlags(int32 index) const
{
	return (0);
}

const char *Process::GetPortName(int32 index) const
{
	return (nullptr);
}

#if C4PLAYSTATION3

	unsigned_int32 Process::GetPortCompileFlags(int32 index) const
	{
		return (0);
	}

#endif

void Process::ReferenceStateParams(const Process *process)
{
}

void Process::GenerateSourceData(const ShaderCompileData *compileData) const
{
}

int32 Process::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	signature[0] = processType;
	int32 count = 1;
	
	int32 portCount = GetPortCount();
	for (machine port = 0; port < portCount; port++)
	{
		const Route *route = GetPortRoute(port);
		if (route)
		{
			signature[count++] = (port << 24) | route->GetStartElement()->GetProcessIndex();
			
			unsigned_int32 routeSignature = route->GenerateRouteSignature();
			if ((routeSignature & 0xFFFF) != 0x001B) signature[count++] = routeSignature;
		}
	}
	
	return (count);
}

int32 Process::GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const
{
	return (0);
}

void Process::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
}

int32 Process::PregenerateOutputIdentifier(const SwizzleData *swizzleData, char *name)
{
	char *start = name;
	
	if (swizzleData->negate) *name++ = '-';
	if (swizzleData->absolute) *name++ = '|';
	
	return (name - start);
}

int32 Process::PostgenerateOutputIdentifier(const ShaderCompileData *compileData, const SwizzleData *swizzleData, char *name)
{
	int32 len = 0;
	int32 size = swizzleData->size;
	
	unsigned_int8 c1 = swizzleData->component[0];
	
	if (size == 1)
	{
		name[0] = '.';
		name[1] = Route::GetSwizzleChar(c1);
		len = 2;
	}
	else
	{
		unsigned_int8 c2 = swizzleData->component[1];
		
		if (size == 2)
		{
			if (c1 == c2)
			{
				name[0] = '.';
				name[1] = Route::GetSwizzleChar(c1);
				len = 2;
			}
			else
			{
				if (compileData->programFlag)
				{
					if ((c1 != 0) || (c2 != 1))
					{
						name[0] = '.';
						name[1] = Route::GetSwizzleChar(c1);
						name[2] = name[3] = name[4] = Route::GetSwizzleChar(c2);
						len = 5;
					}
				}
				else
				{
					name[0] = '.';
					name[1] = Route::GetSwizzleChar(c1);
					name[2] = Route::GetSwizzleChar(c2);
					len = 3;
				}
			}
		}
		else
		{
			unsigned_int8 c3 = swizzleData->component[2];
			
			if (size == 3)
			{
				if ((c1 == c2) && (c1 == c3))
				{
					name[0] = '.';
					name[1] = Route::GetSwizzleChar(c1);
					len = 2;
				}
				else
				{
					if (compileData->programFlag)
					{
						if ((c1 != 0) || (c2 != 1) || (c3 != 2))
						{
							name[0] = '.';
							name[1] = Route::GetSwizzleChar(c1);
							name[2] = Route::GetSwizzleChar(c2);
							name[3] = name[4] = Route::GetSwizzleChar(c3);
							len = 5;
						}
					}
					else
					{
						name[0] = '.';
						name[1] = Route::GetSwizzleChar(c1);
						name[2] = Route::GetSwizzleChar(c2);
						name[3] = Route::GetSwizzleChar(c3);
						len = 4;
					}
				}
			}
			else
			{
				unsigned_int8 c4 = swizzleData->component[3];
				
				if ((c1 == c2) && (c1 == c3) && (c1 == c4))
				{
					name[0] = '.';
					name[1] = Route::GetSwizzleChar(c1);
					len = 2;
				}
				else if ((c1 != 0) || (c2 != 1) || (c3 != 2) || (c4 != 3))
				{
					name[0] = '.';
					name[1] = Route::GetSwizzleChar(c1);
					name[2] = Route::GetSwizzleChar(c2);
					name[3] = Route::GetSwizzleChar(c3);
					name[4] = Route::GetSwizzleChar(c4);
					len = 5;
				}
			}
		}
	}
	
	if (swizzleData->absolute)
	{
		name[len] = '|';
		len++;
	}
	
	return (len);
}

int32 Process::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 reg = GetProcessData()->outputRegister;
	if (reg < 0) return (0);
	
	if (swizzleData)
	{
		int32 len = PregenerateOutputIdentifier(swizzleData, name);
		name += len;
		
		name[0] = 'r';
		if (reg < 10)
		{
			name[1] = (char) (reg + 48);
			return (PostgenerateOutputIdentifier(compileData, swizzleData, name + 2) + len + 2);
		}
		
		int32 d = reg / 10;
		name[1] = (char) (d + 48);
		name[2] = reg - d * 10 + 48;
		return (PostgenerateOutputIdentifier(compileData, swizzleData, name + 3) + len + 3);
	}
	
	name[0] = 'r';
	if (reg < 10)
	{
		name[1] = (char) (reg + 48);
		return (2);
	}
	
	int32 d = reg / 10;
	name[1] = (char) (d + 48);
	name[2] = reg - d * 10 + 48;
	return (3);
}

int32 Process::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	return (0);
}

int32 Process::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	return (0);
}


SectionProcess::SectionProcess() : Process(kProcessSection)
{
	sectionWidth = 0.0F;
	sectionHeight = 0.0F;
	
	sectionColor.Set(0.96875F, 0.96875F, 0.96875F);
}

SectionProcess::SectionProcess(const SectionProcess& sectionProcess) : Process(sectionProcess)
{
	sectionWidth = sectionProcess.sectionWidth;
	sectionHeight = sectionProcess.sectionHeight;
	
	sectionColor = sectionProcess.sectionColor;
}

SectionProcess::~SectionProcess()
{
}

Process *SectionProcess::Replicate(void) const
{
	return (new SectionProcess(*this));
}

void SectionProcess::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Process::Pack(data, packFlags);
	
	data << ChunkHeader('SIZE', 8);
	data << sectionWidth;
	data << sectionHeight;
	
	data << ChunkHeader('COLR', sizeof(ColorRGBA));
	data << sectionColor;
	
	data << TerminatorChunk;
}

void SectionProcess::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Process::Unpack(data, unpackFlags);
	UnpackChunkList<SectionProcess>(data, unpackFlags);
}

bool SectionProcess::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'SIZE':
			
			data >> sectionWidth;
			data >> sectionHeight;
			return (true);
		
		case 'COLR':
			
			data >> sectionColor;
			return (true);
	}
	
	return (false);
}

int32 SectionProcess::GetSettingCount(void) const
{
	return (Process::GetSettingCount() + 1);
}

Setting *SectionProcess::GetSetting(int32 index) const
{
	int32 count = Process::GetSettingCount();
	if (index < count) return (Process::GetSetting(index));
	
	if (index == count)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		const char *title = table->GetString(StringID('PROC', kProcessSection, 'COLR'));
		const char *picker = table->GetString(StringID('PROC', kProcessSection, 'PICK'));
		return (new ColorSetting('COLR', sectionColor, title, picker));
	}
	
	return (nullptr);
}

void SectionProcess::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'COLR')
	{
		sectionColor = static_cast<const ColorSetting *>(setting)->GetColor();
	}
	else
	{
		Process::SetSetting(setting);
	}
}


ConstantProcess::ConstantProcess(ProcessType type) : Process(type)
{
	SetBaseProcessType(kProcessConstant);
	
	parameterSlot = -1;
}

ConstantProcess::ConstantProcess(const ConstantProcess& constantProcess) : Process(constantProcess)
{
	parameterSlot = constantProcess.parameterSlot;
	parameterData = constantProcess.parameterData;
}

ConstantProcess::~ConstantProcess()
{
}

void ConstantProcess::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Process::Pack(data, packFlags);
	
	data << parameterSlot;
}

void ConstantProcess::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Process::Unpack(data, unpackFlags);
	
	data >> parameterSlot;
}

int32 ConstantProcess::GetSettingCount(void) const
{
	return (Process::GetSettingCount() + 1);
}

Setting *ConstantProcess::GetSetting(int32 index) const
{
	int32 count = Process::GetSettingCount();
	if (index < count) return (Process::GetSetting(index));
	
	if (index == count)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		const char *title = table->GetString(StringID('PROC', kProcessConstant, 'SLOT'));
		MenuSetting *menu = new MenuSetting('SLOT', parameterSlot + 1, title, 9);
		
		menu->SetMenuItemString(0, table->GetString(StringID('PROC', kProcessConstant, 'SLOT', 'CNST')));
		for (machine a = 0; a < 8; a++) menu->SetMenuItemString(a + 1, table->GetString(StringID('PROC', kProcessConstant, 'SLOT', 'PRM0' + a)));
		
		return (menu);
	}
	
	return (nullptr);
}

void ConstantProcess::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'SLOT')
	{
		parameterSlot = static_cast<const MenuSetting *>(setting)->GetMenuSelection() - 1;
	}
	else
	{
		Process::SetSetting(setting);
	}
}

bool ConstantProcess::operator ==(const Process& process) const
{
	if (Process::operator ==(process))
	{
		const ConstantProcess& constantProcess = static_cast<const ConstantProcess&>(process);
		return (parameterSlot == constantProcess.parameterSlot);
	}
	
	return (false);
}

int32 ConstantProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = Process::GenerateProcessSignature(compileData, signature);
	signature[count] = parameterSlot;
	return (count + 1);
}

void ConstantProcess::StateFunc_LoadScalar0(const Renderable *renderable, const void *cookie)
{
	float f = *static_cast<const float *>(cookie);
	Render::SetFragmentProgramParameter4f(0, f, f, f, f);
}

void ConstantProcess::StateFunc_LoadScalar1(const Renderable *renderable, const void *cookie)
{
	float f = *static_cast<const float *>(cookie);
	Render::SetFragmentProgramParameter4f(1, f, f, f, f);
}

void ConstantProcess::StateFunc_LoadScalar2(const Renderable *renderable, const void *cookie)
{
	float f = *static_cast<const float *>(cookie);
	Render::SetFragmentProgramParameter4f(2, f, f, f, f);
}

void ConstantProcess::StateFunc_LoadScalar3(const Renderable *renderable, const void *cookie)
{
	float f = *static_cast<const float *>(cookie);
	Render::SetFragmentProgramParameter4f(3, f, f, f, f);
}

void ConstantProcess::StateFunc_LoadScalar4(const Renderable *renderable, const void *cookie)
{
	float f = *static_cast<const float *>(cookie);
	Render::SetFragmentProgramParameter4f(4, f, f, f, f);
}

void ConstantProcess::StateFunc_LoadScalar5(const Renderable *renderable, const void *cookie)
{
	float f = *static_cast<const float *>(cookie);
	Render::SetFragmentProgramParameter4f(5, f, f, f, f);
}

void ConstantProcess::StateFunc_LoadScalar6(const Renderable *renderable, const void *cookie)
{
	float f = *static_cast<const float *>(cookie);
	Render::SetFragmentProgramParameter4f(6, f, f, f, f);
}

void ConstantProcess::StateFunc_LoadScalar7(const Renderable *renderable, const void *cookie)
{
	float f = *static_cast<const float *>(cookie);
	Render::SetFragmentProgramParameter4f(7, f, f, f, f);
}

void ConstantProcess::StateFunc_LoadVector0(const Renderable *renderable, const void *cookie)
{
	Render::SetFragmentProgramParameter4fv(0, static_cast<const float *>(cookie));
}

void ConstantProcess::StateFunc_LoadVector1(const Renderable *renderable, const void *cookie)
{
	Render::SetFragmentProgramParameter4fv(1, static_cast<const float *>(cookie));
}

void ConstantProcess::StateFunc_LoadVector2(const Renderable *renderable, const void *cookie)
{
	Render::SetFragmentProgramParameter4fv(2, static_cast<const float *>(cookie));
}

void ConstantProcess::StateFunc_LoadVector3(const Renderable *renderable, const void *cookie)
{
	Render::SetFragmentProgramParameter4fv(3, static_cast<const float *>(cookie));
}

void ConstantProcess::StateFunc_LoadVector4(const Renderable *renderable, const void *cookie)
{
	Render::SetFragmentProgramParameter4fv(4, static_cast<const float *>(cookie));
}

void ConstantProcess::StateFunc_LoadVector5(const Renderable *renderable, const void *cookie)
{
	Render::SetFragmentProgramParameter4fv(5, static_cast<const float *>(cookie));
}

void ConstantProcess::StateFunc_LoadVector6(const Renderable *renderable, const void *cookie)
{
	Render::SetFragmentProgramParameter4fv(6, static_cast<const float *>(cookie));
}

void ConstantProcess::StateFunc_LoadVector7(const Renderable *renderable, const void *cookie)
{
	Render::SetFragmentProgramParameter4fv(7, static_cast<const float *>(cookie));
}


ScalarProcess::ScalarProcess() : ConstantProcess(kProcessScalar)
{
	scalarValue = 1.0F;
	parameterData = &scalarValue;
}

ScalarProcess::ScalarProcess(const ScalarProcess& scalarProcess) : ConstantProcess(scalarProcess)
{
	scalarValue = scalarProcess.scalarValue;
	parameterData = &scalarValue;
}

ScalarProcess::~ScalarProcess()
{
}

Process *ScalarProcess::Replicate(void) const
{
	return (new ScalarProcess(*this));
}

void ScalarProcess::SetParameterValue(const Vector4D& param)
{
	scalarValue = param.x;
}

void ScalarProcess::Pack(Packer& data, unsigned_int32 packFlags) const
{
	ConstantProcess::Pack(data, packFlags);
	
	data << scalarValue;
}

void ScalarProcess::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	ConstantProcess::Unpack(data, unpackFlags);
	
	data >> scalarValue;
}

int32 ScalarProcess::GetSettingCount(void) const
{
	return (ConstantProcess::GetSettingCount() + 1);
}

Setting *ScalarProcess::GetSetting(int32 index) const
{
	int32 count = ConstantProcess::GetSettingCount();
	if (index < count) return (ConstantProcess::GetSetting(index));
	
	if (index == count)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		const char *title = table->GetString(StringID('PROC', kProcessScalar, 'VALU'));
		return (new TextSetting('VALU', scalarValue, title));
	}
	
	return (nullptr);
}

void ScalarProcess::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'VALU')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		scalarValue = Text::StringToFloat(text);
	}
	else
	{
		ConstantProcess::SetSetting(setting);
	}
}

bool ScalarProcess::operator ==(const Process& process) const
{
	if (ConstantProcess::operator ==(process))
	{
		if (GetParameterSlot() < 0)
		{
			const ScalarProcess& scalarProcess = static_cast<const ScalarProcess&>(process);
			return (scalarValue == scalarProcess.scalarValue);
		}
		
		return (true);
	}
	
	return (false);
}

void ScalarProcess::ReferenceStateParams(const Process *process)
{
	parameterData = static_cast<const ScalarProcess *>(process)->parameterData;
}

int32 ScalarProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = ConstantProcess::GenerateProcessSignature(compileData, signature);
	if (GetParameterSlot() < 0) signature[count++] = *reinterpret_cast<const unsigned_int32 *>(&scalarValue);
	return (count);
}

void ScalarProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->outputSize = 1;
	
	int32 slot = GetParameterSlot();
	if (slot >= 0) compileData->shaderData->AddStateFunction(scalarStateFunction[slot], parameterData);
}

int32 ScalarProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 slot = GetParameterSlot();
	if (slot < 0)
	{
		float v = scalarValue;
		if (swizzleData->absolute) v = Fabs(v);
		if (swizzleData->negate) v = -v;
		
		if (compileData->programFlag)
		{
			name[0] = '{';
			int32 len = Text::CopyText(Text::FloatToString(v), name + 1) + 1;
			name[len] = '}';
			name[len + 1] = '.';
			name[len + 2] = 'x';
			return (len + 3);
		}
		
		return (Text::CopyText(Text::FloatToString(v), name));
	}
	
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	len += Text::CopyText(constantIdentifier[compileData->programFlag][slot], name + len);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + len) + len);
}


VectorProcess::VectorProcess() : ConstantProcess(kProcessVector)
{
	vectorValue.Set(0.0F, 0.0F, 0.0F, 0.0F);
	parameterData = &vectorValue.x;
}

VectorProcess::VectorProcess(const VectorProcess& vectorProcess) : ConstantProcess(vectorProcess)
{
	vectorValue = vectorProcess.vectorValue;
	parameterData = &vectorValue.x;
}

VectorProcess::~VectorProcess()
{
}

Process *VectorProcess::Replicate(void) const
{
	return (new VectorProcess(*this));
}

void VectorProcess::SetParameterValue(const Vector4D& param)
{
	vectorValue = param;
}

void VectorProcess::Pack(Packer& data, unsigned_int32 packFlags) const
{
	ConstantProcess::Pack(data, packFlags);
	
	data << vectorValue;
}

void VectorProcess::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	ConstantProcess::Unpack(data, unpackFlags);
	
	data >> vectorValue;
}

int32 VectorProcess::GetSettingCount(void) const
{
	return (ConstantProcess::GetSettingCount() + 4);
}

Setting *VectorProcess::GetSetting(int32 index) const
{
	int32 count = ConstantProcess::GetSettingCount();
	if (index < count) return (ConstantProcess::GetSetting(index));
	
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (index == count)
	{
		const char *title = table->GetString(StringID('PROC', kProcessVector, 'XXXX'));
		return (new TextSetting('XXXX', vectorValue.x, title));
	}
	
	if (index == count + 1)
	{
		const char *title = table->GetString(StringID('PROC', kProcessVector, 'YYYY'));
		return (new TextSetting('YYYY', vectorValue.y, title));
	}
	
	if (index == count + 2)
	{
		const char *title = table->GetString(StringID('PROC', kProcessVector, 'ZZZZ'));
		return (new TextSetting('ZZZZ', vectorValue.z, title));
	}
	
	if (index == count + 3)
	{
		const char *title = table->GetString(StringID('PROC', kProcessVector, 'WWWW'));
		return (new TextSetting('WWWW', vectorValue.w, title));
	}
	
	return (nullptr);
}

void VectorProcess::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'XXXX')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		vectorValue.x = Text::StringToFloat(text);
	}
	else if (identifier == 'YYYY')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		vectorValue.y = Text::StringToFloat(text);
	}
	else if (identifier == 'ZZZZ')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		vectorValue.z = Text::StringToFloat(text);
	}
	else if (identifier == 'WWWW')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		vectorValue.w = Text::StringToFloat(text);
	}
	else
	{
		ConstantProcess::SetSetting(setting);
	}
}

bool VectorProcess::operator ==(const Process& process) const
{
	if (ConstantProcess::operator ==(process))
	{
		if (GetParameterSlot() < 0)
		{
			const VectorProcess& vectorProcess = static_cast<const VectorProcess&>(process);
			return (vectorValue == vectorProcess.vectorValue);
		}
		
		return (true);
	}
	
	return (false);
}

void VectorProcess::ReferenceStateParams(const Process *process)
{
	parameterData = static_cast<const VectorProcess *>(process)->parameterData;
}

int32 VectorProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = ConstantProcess::GenerateProcessSignature(compileData, signature);
	
	if (GetParameterSlot() < 0)
	{
		signature += count;
		signature[0] = *reinterpret_cast<const unsigned_int32 *>(&vectorValue.x);
		signature[1] = *reinterpret_cast<const unsigned_int32 *>(&vectorValue.y);
		signature[2] = *reinterpret_cast<const unsigned_int32 *>(&vectorValue.z);
		signature[3] = *reinterpret_cast<const unsigned_int32 *>(&vectorValue.w);
		count += 4;
	}
	
	return (count);
}

void VectorProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->outputSize = 4;
	
	int32 slot = GetParameterSlot();
	if (slot >= 0) compileData->shaderData->AddStateFunction(vectorStateFunction[slot], parameterData);
}

int32 VectorProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 slot = GetParameterSlot();
	if (slot < 0)
	{
		int32 size = swizzleData->size;
		unsigned_int8 c1 = swizzleData->component[0];
		for (machine a = 1; a < size; a++)
		{
			if (swizzleData->component[a] != c1)
			{
				Vector4D	v;
				
				if (!swizzleData->absolute) v = vectorValue;
				else for (machine a = 0; a < 4; a++) v[a] = Fabs(vectorValue[a]);
				if (swizzleData->negate) v = -v;
				
				char *start = name;
				
				if (compileData->programFlag)
				{
					*name++ = '{';
					
					name += Text::CopyText(Text::FloatToString(v[swizzleData->component[0]]), name);
					name[0] = ',';
					name[1] = ' ';
					name += 2;
					
					name += Text::CopyText(Text::FloatToString(v[swizzleData->component[1]]), name);
					name[0] = ',';
					name[1] = ' ';
					name += 2;
					
					name += Text::CopyText(Text::FloatToString(v[swizzleData->component[2]]), name);
					name[0] = ',';
					name[1] = ' ';
					name += 2;
					
					name += Text::CopyText(Text::FloatToString(v[swizzleData->component[3]]), name);
					
					name[0] = '}';
					name[1] = 0;
					name++;
				}
				else
				{
					if (size > 1)
					{
						#if C4OPENGL
						
							name += Text::CopyText("vec", name);
						
						#else
						
							name += Text::CopyText("float", name);
						
						#endif
						
						name[0] = size + '0';
						name[1] = '(';
						name += 2;
						
						for (machine a = 0; a < size - 1; a++)
						{
							name += Text::CopyText(Text::FloatToString(v[swizzleData->component[a]]), name);
							name[0] = ',';
							name[1] = ' ';
							name += 2;
						}
						
						name += Text::CopyText(Text::FloatToString(v[swizzleData->component[size - 1]]), name);
						
						name[0] = ')';
						name[1] = 0;
						name++;
					}
					else
					{
						name += Text::CopyText(Text::FloatToString(v[swizzleData->component[0]]), name);
					}
				}
				
				return (name - start);
			}
		}
		
		float v = vectorValue[c1];
		if (swizzleData->absolute) v = Fabs(v);
		if (swizzleData->negate) v = -v;
		
		if (compileData->programFlag)
		{
			name[0] = '{';
			int32 len = Text::CopyText(Text::FloatToString(v), name + 1) + 1;
			name[len] = '}';
			name[len + 1] = '.';
			name[len + 2] = 'x';
			return (len + 3);
		}
		
		return (Text::CopyText(Text::FloatToString(v), name));
	}
	
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	len += Text::CopyText(constantIdentifier[compileData->programFlag][slot], name + len);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + len) + len);
}


ColorProcess::ColorProcess() : ConstantProcess(kProcessColor)
{
	colorValue.Set(1.0F, 1.0F, 1.0F, 1.0F);
	parameterData = &colorValue.red;
}

ColorProcess::ColorProcess(const ColorProcess& colorProcess) : ConstantProcess(colorProcess)
{
	colorValue = colorProcess.colorValue;
	parameterData = &colorValue.red;
}

ColorProcess::~ColorProcess()
{
}

Process *ColorProcess::Replicate(void) const
{
	return (new ColorProcess(*this));
}

void ColorProcess::SetParameterValue(const Vector4D& param)
{
	colorValue.Set(param.x, param.y, param.z, param.w);
}

void ColorProcess::Pack(Packer& data, unsigned_int32 packFlags) const
{
	ConstantProcess::Pack(data, packFlags);
	
	data << colorValue;
}

void ColorProcess::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	ConstantProcess::Unpack(data, unpackFlags);
	
	data >> colorValue;
}

int32 ColorProcess::GetSettingCount(void) const
{
	return (ConstantProcess::GetSettingCount() + 1);
}

Setting *ColorProcess::GetSetting(int32 index) const
{
	int32 count = ConstantProcess::GetSettingCount();
	if (index < count) return (ConstantProcess::GetSetting(index));
	
	if (index == count)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		const char *title = table->GetString(StringID('PROC', kProcessColor, 'COLR'));
		const char *picker = table->GetString(StringID('PROC', kProcessColor, 'PICK'));
		return (new ColorSetting('COLR', colorValue, title, picker, kColorPickerAlpha));
	}
	
	return (nullptr);
}

void ColorProcess::SetSetting(const Setting *setting)
{
	if (setting->GetSettingIdentifier() == 'COLR')
	{
		colorValue = static_cast<const ColorSetting *>(setting)->GetColor();
	}
	else
	{
		ConstantProcess::SetSetting(setting);
	}
}

bool ColorProcess::operator ==(const Process& process) const
{
	if (ConstantProcess::operator ==(process))
	{
		if (GetParameterSlot() < 0)
		{
			const ColorProcess& colorProcess = static_cast<const ColorProcess&>(process);
			return (colorValue == colorProcess.colorValue);
		}
		
		return (true);
	}
	
	return (false);
}

void ColorProcess::ReferenceStateParams(const Process *process)
{
	parameterData = static_cast<const ColorProcess *>(process)->parameterData;
}

int32 ColorProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = ConstantProcess::GenerateProcessSignature(compileData, signature);
	
	if (GetParameterSlot() < 0)
	{
		signature += count;
		signature[0] = *reinterpret_cast<const unsigned_int32 *>(&colorValue.red);
		signature[1] = *reinterpret_cast<const unsigned_int32 *>(&colorValue.green);
		signature[2] = *reinterpret_cast<const unsigned_int32 *>(&colorValue.blue);
		signature[3] = *reinterpret_cast<const unsigned_int32 *>(&colorValue.alpha);
		count += 4;
	}
	
	return (count);
}

void ColorProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->outputSize = 4;
	
	int32 slot = GetParameterSlot();
	if (slot >= 0) compileData->shaderData->AddStateFunction(vectorStateFunction[slot], parameterData);
}

int32 ColorProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 slot = GetParameterSlot();
	if (slot < 0)
	{
		ColorRGBA	v;
		
		if (!swizzleData->absolute) v = colorValue;
		else for (machine a = 0; a < 4; a++) v[a] = Fabs(colorValue[a]);
		if (swizzleData->negate) v = -v;
		
		char *start = name;
		
		if (compileData->programFlag)
		{
			*name++ = '{';
			
			name += Text::CopyText(Text::FloatToString(v[swizzleData->component[0]]), name);
			name[0] = ',';
			name[1] = ' ';
			name += 2;
			
			name += Text::CopyText(Text::FloatToString(v[swizzleData->component[1]]), name);
			name[0] = ',';
			name[1] = ' ';
			name += 2;
			
			name += Text::CopyText(Text::FloatToString(v[swizzleData->component[2]]), name);
			name[0] = ',';
			name[1] = ' ';
			name += 2;
			
			name += Text::CopyText(Text::FloatToString(v[swizzleData->component[3]]), name);
			
			name[0] = '}';
			name[1] = 0;
			name++;
		}
		else
		{
			int32 size = swizzleData->size;
			if (size > 1)
			{
				#if C4OPENGL
				
					name += Text::CopyText("vec", name);
				
				#else
				
					name += Text::CopyText("float", name);
				
				#endif
				
				name[0] = size + '0';
				name[1] = '(';
				name += 2;
				
				for (machine a = 0; a < size - 1; a++)
				{
					name += Text::CopyText(Text::FloatToString(v[swizzleData->component[a]]), name);
					name[0] = ',';
					name[1] = ' ';
					name += 2;
				}
				
				name += Text::CopyText(Text::FloatToString(v[swizzleData->component[size - 1]]), name);
				
				name[0] = ')';
				name[1] = 0;
				name++;
			}
			else
			{
				name += Text::CopyText(Text::FloatToString(v[swizzleData->component[0]]), name);
			}
		}
		
		return (name - start);
	}
	
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	len += Text::CopyText(constantIdentifier[compileData->programFlag][slot], name + len);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + len) + len);
}


TimeProcess::TimeProcess() : Process(kProcessTime)
{
	SetBaseProcessType(kProcessParameter);
}

TimeProcess::TimeProcess(const TimeProcess& timeProcess) : Process(timeProcess)
{
}

TimeProcess::~TimeProcess()
{
}

Process *TimeProcess::Replicate(void) const
{
	return (new TimeProcess(*this));
}

void TimeProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->outputSize = 1;
}

int32 TimeProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	len += Text::CopyText((compileData->programFlag) ? "program.env[" FRAGMENT_PARAM_SHADER_TIME "]" : "param[" FRAGMENT_PARAM_SHADER_TIME "]", name + len);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + len) + len);
}


DetailLevelProcess::DetailLevelProcess() : Process(kProcessDetailLevel)
{
	SetBaseProcessType(kProcessParameter);
}

DetailLevelProcess::DetailLevelProcess(const DetailLevelProcess& detailLevelProcess) : Process(detailLevelProcess)
{
}

DetailLevelProcess::~DetailLevelProcess()
{
}

Process *DetailLevelProcess::Replicate(void) const
{
	return (new DetailLevelProcess(*this));
}

void DetailLevelProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->outputSize = 1;
	
	compileData->shaderData->AddStateFunction(&StateFunc_SetDetailLevelParam);
}

int32 DetailLevelProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	len += Text::CopyText((compileData->programFlag) ? "program.env[" FRAGMENT_PARAM_DETAIL_LEVEL "]" : "param[" FRAGMENT_PARAM_DETAIL_LEVEL "]", name + len);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + len) + len);
}

void DetailLevelProcess::StateFunc_SetDetailLevelParam(const Renderable *renderable, const void *cookie)
{
	Render::SetFragmentProgramParameter4f(kFragmentParamDetailLevel, renderable->GetShaderDetailParameter(), 0.0F, 0.0F, 0.0F);
}


TextureMapProcess::TextureMapProcess() : Process(kProcessTextureMap)
{
	SetBaseProcessType(kProcessTextureMap);
	
	textureName[0] = 0;
	textureObject = nullptr;
}

TextureMapProcess::TextureMapProcess(ProcessType type) : Process(type)
{
	SetBaseProcessType(kProcessTextureMap);
	
	textureName[0] = 0;
	textureObject = nullptr;
}

TextureMapProcess::TextureMapProcess(const TextureMapProcess& textureMapProcess) : Process(textureMapProcess)
{
	textureName = textureMapProcess.textureName;
	
	Texture *texture = textureMapProcess.GetTexture();
	texture->Retain();
	textureObject = texture;
}

TextureMapProcess::~TextureMapProcess()
{
	if (textureObject) textureObject->Release();
}

Process *TextureMapProcess::Replicate(void) const
{
	return (new TextureMapProcess(*this));
}

void TextureMapProcess::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Process::Pack(data, packFlags);
	
	data << textureName;
}

void TextureMapProcess::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Process::Unpack(data, unpackFlags);
	
	data >> textureName;
	SetTexture(textureName);
}

void *TextureMapProcess::BeginSettingsUnpack(void)
{
	if (textureObject)
	{
		textureObject->Release();
		textureObject = nullptr;
	}
	
	return (Process::BeginSettingsUnpack());
}

int32 TextureMapProcess::GetSettingCount(void) const
{
	return (Process::GetSettingCount() + 1);
}

Setting *TextureMapProcess::GetSetting(int32 index) const
{
	int32 count = Process::GetSettingCount();
	if (index < count) return (Process::GetSetting(index));
	
	if (index == count)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		const char *title = table->GetString(StringID('PROC', kProcessTextureMap, 'TNAM'));
		const char *picker = table->GetString(StringID('PROC', kProcessTextureMap, 'PICK'));
		return (new ResourceSetting('TNAM', textureName, title, picker, TextureResource::GetDescriptor()));
	}
	
	return (nullptr);
}

void TextureMapProcess::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'TNAM')
	{
		SetTexture(static_cast<const ResourceSetting *>(setting)->GetResourceName());
	}
	else
	{
		Process::SetSetting(setting);
	}
}

bool TextureMapProcess::operator ==(const Process& process) const
{
	if (Process::operator ==(process))
	{
		const TextureMapProcess& textureMapProcess = static_cast<const TextureMapProcess&>(process);
		return (textureName == textureMapProcess.textureName);
	}
	
	return (false);
}

int32 TextureMapProcess::GetTexcoordSize(const Texture *texture)
{
	static const char texcoordSize[Render::kTextureTargetCount] =
	{
		2, 3, 2, 3, 3
	};
	
	return (texcoordSize[texture->GetTextureTargetIndex()]);
}

Texture *TextureMapProcess::GetTexture(void) const
{
	if (textureObject) return (textureObject);
	
	textureObject = Texture::Get("");
	return (textureObject);
}

void TextureMapProcess::SetTexture(const char *name)
{
	Texture *object = textureObject;
	
	if (name)
	{
		if (name != &textureName[0]) textureName = name;
		textureObject = Texture::Get(name);
	}
	else
	{
		textureName[0] = 0;
		textureObject = nullptr;
	}
	
	if (object) object->Release();
}

void TextureMapProcess::SetTexture(Texture *texture)
{
	Texture *object = textureObject;
	textureObject = texture;
	
	if (texture) texture->Retain();
	if (object) object->Release();
	
	textureName[0] = 0;
}

void TextureMapProcess::SetTexture(const TextureHeader *header, const void *image)
{
	Texture *object = textureObject;
	
	textureName[0] = 0;
	if (header) textureObject = Texture::Get(header, image);
	else textureObject = nullptr;
	
	if (object) object->Release();
}

int32 TextureMapProcess::GetPortCount(void) const
{
	return (1);
}

const char *TextureMapProcess::GetPortName(int32 index) const
{
	return ("TEXC");
}

#if C4PLAYSTATION3

	unsigned_int32 TextureMapProcess::GetPortCompileFlags(int32 index) const
	{
		return (kProcessDependentTexture);
	}

#endif

#if C4OPENGL

	void TextureMapProcess::GenerateSourceData(const ShaderCompileData *compileData) const
	{
		const Texture *texture = GetTexture();
		if ((texture->GetTextureTargetIndex() == Render::kTextureTargetArray2D) && (!TheGraphicsMgr->GetCapabilities()->capabilityFlag[kCapabilityProgramTextureArray])) compileData->programFlag = false;
	}

#endif

int32 TextureMapProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = Process::GenerateProcessSignature(compileData, signature);
	
	const Texture *texture = GetTexture();
	unsigned_int32 sig = texture->GetTextureType();
	if (texture->GetTextureFlags() & kTextureImagePalette) sig |= 0x80000000;
	if (texture->GetAlphaSemantic() == kTextureSemanticNormal) sig |= 0x00800000;
	
	signature += count;
	signature[0] = sig;
	signature[1] = 0;
	
	signatureUnit = &signature[1];
	return (count + 2);
}

void TextureMapProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->textureCount = 1;
	data->outputSize = 4;
	
	const Texture *texture = GetTexture();
	data->inputSize[0] = GetTexcoordSize(texture);
	data->textureObject[0] = texture;
}

int32 TextureMapProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	const Texture *texture = GetTexture();
	if ((texture->GetTextureTargetIndex() != Render::kTextureTarget2D) || (!(texture->GetTextureFlags() & kTextureImagePalette)))
	{
		static const char code[] =
		{
			"TEX		#, %0, %IMG0, %TRG0;\n"
		};
		
		programCode[0] = code;
	}
	else
	{
		static const char code[] =
		{
			"MOV		temp.xy, %0;\n"
			"MOV		temp.w, -10.0;\n"
			"TXB		#, temp, %IMG0, 2D;\n"
		};
		
		programCode[0] = code;
	}
	
	return (1);
}

int32 TextureMapProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	const Texture *texture = GetTexture();
	if ((texture->GetTextureTargetIndex() != Render::kTextureTarget2D) || (!(texture->GetTextureFlags() & kTextureImagePalette)))
	{
		static const char code[] =
		{
			"# = %TRG0(%IMG0, %0);\n"
		};
		
		shaderCode[0] = code;
	}
	else
	{
		static const char code[] =
		{
			#if C4OPENGL
			
				"# = texture2D(%IMG0, %0, -10.0);\n"
			
			#else
			
				"temp.xy = %0;\n"
				"temp.w = -10.0;\n"
				"# = tex2Dbias(%IMG0, temp);\n"
			
			#endif
		};
		
		shaderCode[0] = code;
	}
	
	return (1);
}


NormalMapProcess::NormalMapProcess() : TextureMapProcess(kProcessNormalMap)
{
}

NormalMapProcess::NormalMapProcess(const NormalMapProcess& normalMapProcess) : TextureMapProcess(normalMapProcess)
{
}

NormalMapProcess::~NormalMapProcess()
{
}

Process *NormalMapProcess::Replicate(void) const
{
	return (new NormalMapProcess(*this));
}

int32 NormalMapProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	const Texture *texture = GetTexture();
	
	if ((texture->GetTextureTargetIndex() != Render::kTextureTarget2D) || (!(texture->GetTextureFlags() & kTextureImagePalette)))
	{
		static const char code[] =
		{
			"TEX		#, %0, %IMG0, %TRG0;\n"
			"MAD		#.xyz, ##, 2.0, -1.0;\n"
		};
		
		static const char normalizeCode[] =
		{
			"TEX		#, %0, %IMG0, %TRG0;\n"
			"MAD		#.xyz, ##, 2.0, -1.0;\n"
			"DP3		temp.x, ##, ##;\n"
			"RSQ		temp.x, temp.x;\n"
			"MUL		#.xyz, ##, temp.x;\n"
		};
		
		static const char normalizeCode2[] =
		{
			"TEX		#, %0, %IMG0, %TRG0;\n"
			"MADH		#.xyz, ##, 2.0, -1.0;\n"
			"NRMH		#.xyz, ##;\n"
		};
		
		static const char compressedCode[] =
		{
			"TEX		temp.yw, %0, %IMG0, %TRG0;\n"
			"MAD		#.xyz, temp.wyyy, {2.0, 2.0, 0.0, 0.0}, {-1.0, -1.0, 0.0, 0.0};\n"
			"DP3		temp.z, ##, ##;\n"
			"SUB		temp.z, 1.0, temp.z;\n"
			"MAX		temp.z, temp.z, 0.03125;\n"
			"RSQ		temp.w, temp.z;\n"
			"MUL		#.z, temp.z, temp.w;\n"
		};
		
		static const char compressedCode2[] =
		{
			"TEX		temp.yw, %0, %IMG0, %TRG0;\n"
			"MAD		#.xy, temp.wyyy, 2.0, -1.0;\n"
			"DP2A		temp.z, ##, ##, -1.0;\n"
			"MAX		temp.z, -temp.z, 0.03125;\n"
			"RSQ		temp.w, temp.z;\n"
			"MUL		#.z, temp.z, temp.w;\n"
		};
		
		if (texture->GetImageFormat() == kTextureBC13)
		{
			if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = compressedCode2;
			else programCode[0] = compressedCode;
		}
		else
		{
			if (TheGraphicsMgr->GetRenderOptionFlags() & kRenderOptionNormalizeBumps)
			{
				if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = normalizeCode2;
				else programCode[0] = normalizeCode;
			}
			else
			{
				programCode[0] = code;
			}
		}
	}
	else
	{
		static const char code[] =
		{
			"MOV		temp.xy, %0;\n"
			"MOV		temp.w, -10.0;\n"
			"TXB		#, temp, %IMG0, 2D;\n"
			"MAD		#.xyz, ##, 2.0, -1.0;\n"
		};
		
		static const char compressedCode[] =
		{
			"MOV		temp.xy, %0;\n"
			"MOV		temp.w, -10.0;\n"
			"TXB		temp.yw, temp, %IMG0, 2D;\n"
			"MAD		#.xyz, temp.wyyy, {2.0, 2.0, 0.0, 0.0}, {-1.0, -1.0, 0.0, 0.0};\n"
			"DP3		temp.z, ##, ##;\n"
			"SUB		temp.z, 1.0, temp.z;\n"
			"MAX		temp.z, temp.z, 0.03125;\n"
			"RSQ		temp.w, temp.z;\n"
			"MUL		#.z, temp.z, temp.w;\n"
		};
		
		static const char compressedCode2[] =
		{
			"MOV		temp.xy, %0;\n"
			"MOV		temp.w, -10.0;\n"
			"TXB		temp.yw, temp, %IMG0, 2D;\n"
			"MAD		#.xy, temp.wyyy, 2.0, -1.0;\n"
			"DP2A		temp.z, ##, ##, -1.0;\n"
			"MAX		temp.z, -temp.z, 0.03125;\n"
			"RSQ		temp.w, temp.z;\n"
			"MUL		#.z, temp.z, temp.w;\n"
		};
		
		if (texture->GetImageFormat() == kTextureBC13)
		{
			if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = compressedCode2;
			else programCode[0] = compressedCode;
		}
		else
		{
			programCode[0] = code;
		}
	}
	
	return (1);
}

int32 NormalMapProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	const Texture *texture = GetTexture();
	
	if ((texture->GetTextureTargetIndex() != Render::kTextureTarget2D) || (!(texture->GetTextureFlags() & kTextureImagePalette)))
	{
		static const char code[] =
		{
			"# = %TRG0(%IMG0, %0);\n"
			"#.xyz = ##.xyz * 2.0 - 1.0;\n"
		};
		
		static const char normalizeCode[] =
		{
			"# = %TRG0(%IMG0, %0);\n"
			"#.xyz = normalize(##.xyz * 2.0 - 1.0);\n"
		};
		
		static const char compressedCode[] =
		{
			"#.xy = %TRG0(%IMG0, %0).wy * 2.0 - 1.0;\n"
			"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
		};
		
		if (texture->GetImageFormat() == kTextureBC13)
		{
			shaderCode[0] = compressedCode;
		}
		else
		{
			if (TheGraphicsMgr->GetRenderOptionFlags() & kRenderOptionNormalizeBumps) shaderCode[0] = normalizeCode;
			else shaderCode[0] = code;
		}
	}
	else
	{
		static const char code[] =
		{
			#if C4OPENGL
			
				"# = texture2D(%IMG0, %0, -10.0);\n"
				"#.xyz = ##.xyz * 2.0 - 1.0;\n"
			
			#else
			
				"temp.xy = %0;\n"
				"temp.w = -10.0;\n"
				"# = tex2Dbias(%IMG0, temp);\n"
				"#.xyz = ##.xyz * 2.0 - 1.0;\n"
			
			#endif
		};
		
		static const char compressedCode[] =
		{
			#if C4OPENGL
			
				"#.xy = texture2D(%IMG0, %0, -10.0).wy * 2.0 - 1.0;\n"
				"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
			
			#else
			
				"temp.xy = %0;\n"
				"temp.w = -10.0;\n"
				"#.xy = tex2Dbias(%IMG0, temp).wy * 2.0 - 1.0;\n"
				"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
			
			#endif
		};
		
		if (texture->GetImageFormat() == kTextureBC13) shaderCode[0] = compressedCode;
		else shaderCode[0] = code;
	}
	
	return (1);
}


ImpostorTextureProcess::ImpostorTextureProcess() : TextureMapProcess(kProcessImpostorTexture)
{
	// Hold onto the screen texture here because the ImpostorBlendProcess that really needs it
	// is always a temporary node in the graph undergoing compilation.
	
	screenTextureObject = Texture::Get("C4/screen");
}

ImpostorTextureProcess::ImpostorTextureProcess(ProcessType type) : TextureMapProcess(type)
{
	screenTextureObject = Texture::Get("C4/screen");
}

ImpostorTextureProcess::ImpostorTextureProcess(const ImpostorTextureProcess& impostorTextureProcess) : TextureMapProcess(impostorTextureProcess)
{
	screenTextureObject = impostorTextureProcess.screenTextureObject;
	screenTextureObject->Retain();
}

ImpostorTextureProcess::~ImpostorTextureProcess()
{
	screenTextureObject->Release();
}

Process *ImpostorTextureProcess::Replicate(void) const
{
	return (new ImpostorTextureProcess(*this));
}

int32 ImpostorTextureProcess::GetPortCount(void) const
{
	return (0);
}

int32 ImpostorTextureProcess::GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const
{
	type[0] = kProcessImpostorBlend;
	return (1);
}

void ImpostorTextureProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->temporaryCount = 1;
	data->outputSize = 4;
	
	data->textureCount = 1;
	data->textureObject[0] = GetTexture();
}

int32 ImpostorTextureProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEX		temp, $IMPT.zyyy, %IMG0, %TRG0;\n"
		"TEX		tmp1, $IMPT.wyyy, %IMG0, %TRG0;\n"
		"LRP		#, ibld.x, tmp1, temp;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 ImpostorTextureProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"temp = %TRG0(%IMG0, $IMPT.zy);\n"
		"tmp1 = %TRG0(%IMG0, $IMPT.wy);\n"
		"# = " LERP "(temp, tmp1, ibld.x);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


ImpostorNormalProcess::ImpostorNormalProcess() : ImpostorTextureProcess(kProcessImpostorNormal)
{
}

ImpostorNormalProcess::ImpostorNormalProcess(const ImpostorNormalProcess& impostorNormalProcess) : ImpostorTextureProcess(impostorNormalProcess)
{
}

ImpostorNormalProcess::~ImpostorNormalProcess()
{
}

Process *ImpostorNormalProcess::Replicate(void) const
{
	return (new ImpostorNormalProcess(*this));
}

int32 ImpostorNormalProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEX		temp, $IMPT.zyyy, %IMG0, %TRG0;\n"
		"TEX		tmp1, $IMPT.wyyy, %IMG0, %TRG0;\n"
		"LRP		#, ibld.x, tmp1, temp;\n"
		"MAD		#.xyz, ##, 2.0, -1.0;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 ImpostorNormalProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"temp = %TRG0(%IMG0, $IMPT.zy);\n"
		"tmp1 = %TRG0(%IMG0, $IMPT.wy);\n"
		"# = " LERP "(temp, tmp1, ibld.x);\n"
		"#.xyz = ##.xyz * 2.0 - 1.0;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


TerrainTextureProcess::TerrainTextureProcess() : TextureMapProcess(kProcessTerrainTexture)
{
	blendMode = kTerrainBlendFull;
}

TerrainTextureProcess::TerrainTextureProcess(ProcessType type) : TextureMapProcess(type)
{
	blendMode = kTerrainBlendFull;
}

TerrainTextureProcess::TerrainTextureProcess(const TerrainTextureProcess& terrainTextureProcess) : TextureMapProcess(terrainTextureProcess)
{
	blendMode = terrainTextureProcess.blendMode;
}

TerrainTextureProcess::~TerrainTextureProcess()
{
}

Process *TerrainTextureProcess::Replicate(void) const
{
	return (new TerrainTextureProcess(*this));
}

void TerrainTextureProcess::Pack(Packer& data, unsigned_int32 packFlags) const
{
	TextureMapProcess::Pack(data, packFlags);
	
	data << blendMode;
}

void TerrainTextureProcess::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	TextureMapProcess::Unpack(data, unpackFlags);
	
	#if C4LEGACY
	
		if (data.GetVersion() >= 27) data >> blendMode;
	
	#else
	
		data >> blendMode;
	
	#endif
}

int32 TerrainTextureProcess::GetSettingCount(void) const
{
	return (TextureMapProcess::GetSettingCount() + 1);
}

Setting *TerrainTextureProcess::GetSetting(int32 index) const
{
	int32 count = TextureMapProcess::GetSettingCount();
	if (index < count) return (TextureMapProcess::GetSetting(index));
	
	if (index == count)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		const char *title = table->GetString(StringID('PROC', kProcessTerrainTexture, 'BMOD'));
		MenuSetting *menu = new MenuSetting('BMOD', blendMode, title, 3);
		
		menu->SetMenuItemString(0, table->GetString(StringID('PROC', kProcessTerrainTexture, 'BMOD', 'BLND')));
		menu->SetMenuItemString(1, table->GetString(StringID('PROC', kProcessTerrainTexture, 'BMOD', 'TEXA')));
		menu->SetMenuItemString(2, table->GetString(StringID('PROC', kProcessTerrainTexture, 'BMOD', 'TEXB')));
		
		return (menu);
	}
	
	return (nullptr);
}

void TerrainTextureProcess::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'BMOD')
	{
		blendMode = static_cast<const MenuSetting *>(setting)->GetMenuSelection();
	}
	else
	{
		TextureMapProcess::SetSetting(setting);
	}
}

bool TerrainTextureProcess::operator ==(const Process& process) const
{
	if (TextureMapProcess::operator ==(process))
	{
		const TerrainTextureProcess& terrainTextureProcess = static_cast<const TerrainTextureProcess&>(process);
		return (blendMode == terrainTextureProcess.blendMode);
	}
	
	return (false);
}

int32 TerrainTextureProcess::GetPortCount(void) const
{
	return (0);
}

#if C4OPENGL

	void TerrainTextureProcess::GenerateSourceData(const ShaderCompileData *compileData) const
	{
		if (!TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2])
		{
			compileData->programFlag = false;
		}
		else if ((GetTexture()->GetTextureTargetIndex() == Render::kTextureTargetArray2D) && (!TheGraphicsMgr->GetCapabilities()->capabilityFlag[kCapabilityProgramTextureArray]))
		{
			compileData->programFlag = false;
		}
	}

#endif

int32 TerrainTextureProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = TextureMapProcess::GenerateProcessSignature(compileData, signature);
	signature[count] = blendMode;
	return (count + 1);
}

int32 TerrainTextureProcess::GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const
{
	type[0] = kProcessTerrainTexcoord;
	type[1] = kProcessTriplanarBlend;
	return (2);
}

void TerrainTextureProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->temporaryCount = 5;
	data->outputSize = 4;
	
	data->textureCount = 1;
	data->textureObject[0] = GetTexture();
}

int32 TerrainTextureProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char fullArrayCode[] =
	{
		"TEX		temp, trc1.xzww, %IMG0, ARRAY2D;\n"
		"TEX		tmp1, trc1.yzww, %IMG0, ARRAY2D;\n"
		"TEX		tmp2, trc3, %IMG0, ARRAY2D;\n"
		
		"MUL		temp, temp, tbld.x;\n"
		"MAD		temp, tmp1, tbld.y, temp;\n"
		"MAD		temp, tmp2, tbld.z, temp;\n"
		
		"TEX		tmp3, trc2.xzww, %IMG0, ARRAY2D;\n"
		"TEX		tmp4, trc2.yzww, %IMG0, ARRAY2D;\n"
		"TEX		tmp5, trc3.xyww, %IMG0, ARRAY2D;\n"
		
		"MUL		tmp3, tmp3, tbld.x;\n"
		"MAD		tmp3, tmp4, tbld.y, tmp3;\n"
		"MAD		tmp3, tmp5, tbld.z, tmp3;\n"
		
		"LRP		#, fragment.color.z, tmp3, temp;\n"
	};
	
	static const char primaryArrayCode[] =
	{
		"TEX		temp, trc1.xzww, %IMG0, ARRAY2D;\n"
		"TEX		tmp1, trc1.yzww, %IMG0, ARRAY2D;\n"
		"TEX		tmp2, trc3, %IMG0, ARRAY2D;\n"
		
		"MUL		temp, temp, tbld.x;\n"
		"MAD		temp, tmp1, tbld.y, temp;\n"
		"MAD		#, tmp2, tbld.z, temp;\n"
	};
	
	static const char secondaryArrayCode[] =
	{
		"TEX		tmp3, trc2.xzww, %IMG0, ARRAY2D;\n"
		"TEX		tmp4, trc2.yzww, %IMG0, ARRAY2D;\n"
		"TEX		tmp5, trc3.xyww, %IMG0, ARRAY2D;\n"
		
		"MUL		tmp3, tmp3, tbld.x;\n"
		"MAD		tmp3, tmp4, tbld.y, tmp3;\n"
		"MAD		#, tmp5, tbld.z, tmp3;\n"
	};
	
	static const char fullCode[] =
	{
		"MOV		tmp3.w, tlod.x;\n"
		"MOV		tmp4.w, tlod.y;\n"
		"MOV		tmp5.w, tlod.z;\n"
		
		"MOV		tmp3.xy, trc1.xzzz;\n"
		"TXL		temp, tmp3, %IMG0, 2D;\n"
		"MOV		tmp4.xy, trc1.ywww;\n"
		"TXL		tmp1, tmp4, %IMG0, 2D;\n"
		"MOV		tmp5.xy, trc3.xzzz;\n"
		"TXL		tmp2, tmp5, %IMG0, 2D;\n"
		
		"MUL		temp, temp, tbld.x;\n"
		"MAD		temp, tmp1, tbld.y, temp;\n"
		"MAD		temp, tmp2, tbld.z, temp;\n"
		
		"MOV		tmp3.xy, trc2.xzzz;\n"
		"TXL		tmp3, tmp3, %IMG0, 2D;\n"
		"MOV		tmp4.xy, trc2.ywww;\n"
		"TXL		tmp4, tmp4, %IMG0, 2D;\n"
		"MOV		tmp5.xy, trc3.ywww;\n"
		"TXL		tmp5, tmp5, %IMG0, 2D;\n"
		
		"MUL		tmp3, tmp3, tbld.x;\n"
		"MAD		tmp3, tmp4, tbld.y, tmp3;\n"
		"MAD		tmp3, tmp5, tbld.z, tmp3;\n"
		
		"LRP		#, fragment.color.z, tmp3, temp;\n"
	};
	
	static const char primaryCode[] =
	{
		"MOV		tmp3.w, tlod.x;\n"
		"MOV		tmp4.w, tlod.y;\n"
		"MOV		tmp5.w, tlod.z;\n"
		
		"MOV		tmp3.xy, trc1.xzzz;\n"
		"TXL		temp, tmp3, %IMG0, 2D;\n"
		"MOV		tmp4.xy, trc1.ywww;\n"
		"TXL		tmp1, tmp4, %IMG0, 2D;\n"
		"MOV		tmp5.xy, trc3.xzzz;\n"
		"TXL		tmp2, tmp5, %IMG0, 2D;\n"
		
		"MUL		temp, temp, tbld.x;\n"
		"MAD		temp, tmp1, tbld.y, temp;\n"
		"MAD		#, tmp2, tbld.z, temp;\n"
	};
	
	static const char secondaryCode[] =
	{
		"MOV		tmp3.w, tlod.x;\n"
		"MOV		tmp4.w, tlod.y;\n"
		"MOV		tmp5.w, tlod.z;\n"
		
		"MOV		tmp3.xy, trc2.xzzz;\n"
		"TXL		tmp3, tmp3, %IMG0, 2D;\n"
		"MOV		tmp4.xy, trc2.ywww;\n"
		"TXL		tmp4, tmp4, %IMG0, 2D;\n"
		"MOV		tmp5.xy, trc3.ywww;\n"
		"TXL		tmp5, tmp5, %IMG0, 2D;\n"
		
		"MUL		tmp3, tmp3, tbld.x;\n"
		"MAD		tmp3, tmp4, tbld.y, tmp3;\n"
		"MAD		#, tmp5, tbld.z, tmp3;\n"
	};
	
	if (GetTexture()->GetTextureTargetIndex() == Render::kTextureTargetArray2D)
	{
		if (blendMode == kTerrainBlendFull) programCode[0] = fullArrayCode;
		else if (blendMode == kTerrainBlendPrimary) programCode[0] = primaryArrayCode;
		else programCode[0] = secondaryArrayCode;
	}
	else
	{
		if (blendMode == kTerrainBlendFull) programCode[0] = fullCode;
		else if (blendMode == kTerrainBlendPrimary) programCode[0] = primaryCode;
		else programCode[0] = secondaryCode;
	}
	
	return (1);
}

int32 TerrainTextureProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char fullArrayCode[] =
	{
		"temp = texture2DArray(%IMG0, trc1.xzw);\n"
		"tmp1 = texture2DArray(%IMG0, trc1.yzw);\n"
		"tmp2 = texture2DArray(%IMG0, trc3.xyz);\n"
		
		"tmp3 = texture2DArray(%IMG0, trc2.xzw);\n"
		"tmp4 = texture2DArray(%IMG0, trc2.yzw);\n"
		"tmp5 = texture2DArray(%IMG0, trc3.xyw);\n"
		
		"temp = temp * tbld.x + tmp1 * tbld.y + tmp2 * tbld.z;\n"
		"tmp3 = tmp3 * tbld.x + tmp4 * tbld.y + tmp5 * tbld.z;\n"
		"# = " LERP "(temp, tmp3, " FRAGMENT_COLOR ".z);\n"
	};
	
	static const char primaryArrayCode[] =
	{
		"temp = texture2DArray(%IMG0, trc1.xzw);\n"
		"tmp1 = texture2DArray(%IMG0, trc1.yzw);\n"
		"tmp2 = texture2DArray(%IMG0, trc3.xyz);\n"
		
		"# = temp * tbld.x + tmp1 * tbld.y + tmp2 * tbld.z;\n"
	};
	
	static const char secondaryArrayCode[] =
	{
		"tmp3 = texture2DArray(%IMG0, trc2.xzw);\n"
		"tmp4 = texture2DArray(%IMG0, trc2.yzw);\n"
		"tmp5 = texture2DArray(%IMG0, trc3.xyw);\n"
		
		"# = tmp3 * tbld.x + tmp4 * tbld.y + tmp5 * tbld.z;\n"
	};
	
	static const char fullCode[] =
	{
		#if C4OPENGL
		
			"temp = texture2DLod(%IMG0, trc1.xz, tlod.x);\n"
			"tmp1 = texture2DLod(%IMG0, trc1.yw, tlod.y);\n"
			"tmp2 = texture2DLod(%IMG0, trc3.xz, tlod.z);\n"
			
			"tmp3 = texture2DLod(%IMG0, trc2.xz, tlod.x);\n"
			"tmp4 = texture2DLod(%IMG0, trc2.yw, tlod.y);\n"
			"tmp5 = texture2DLod(%IMG0, trc3.yw, tlod.z);\n"
		
		#else
		
			"tmp3.w = tlod.x;\n"
			"tmp4.w = tlod.y;\n"
			"tmp5.w = tlod.z;\n"
			
			"tmp3.xy = trc1.xz;\n"
			"temp = tex2Dlod(%IMG0, tmp3);\n"
			"tmp4.xy = trc1.yw;\n"
			"tmp1 = tex2Dlod(%IMG0, tmp4);\n"
			"tmp5.xy = trc3.xz;\n"
			"tmp2 = tex2Dlod(%IMG0, tmp5);\n"
			
			"tmp3.xy = trc2.xz;\n"
			"tmp3 = tex2Dlod(%IMG0, tmp3);\n"
			"tmp4.xy = trc2.yw;\n"
			"tmp4 = tex2Dlod(%IMG0, tmp4);\n"
			"tmp5.xy = trc3.yw;\n"
			"tmp5 = tex2Dlod(%IMG0, tmp5);\n"
		
		#endif
		
		"temp = temp * tbld.x + tmp1 * tbld.y + tmp2 * tbld.z;\n"
		"tmp3 = tmp3 * tbld.x + tmp4 * tbld.y + tmp5 * tbld.z;\n"
		"# = " LERP "(temp, tmp3, " FRAGMENT_COLOR ".z);\n"
	};
	
	static const char primaryCode[] =
	{
		#if C4OPENGL
		
			"temp = texture2DLod(%IMG0, trc1.xz, tlod.x);\n"
			"tmp1 = texture2DLod(%IMG0, trc1.yw, tlod.y);\n"
			"tmp2 = texture2DLod(%IMG0, trc3.xz, tlod.z);\n"
		
		#else
		
			"tmp3.w = tlod.x;\n"
			"tmp4.w = tlod.y;\n"
			"tmp5.w = tlod.z;\n"
			
			"tmp3.xy = trc1.xz;\n"
			"temp = tex2Dlod(%IMG0, tmp3);\n"
			"tmp4.xy = trc1.yw;\n"
			"tmp1 = tex2Dlod(%IMG0, tmp4);\n"
			"tmp5.xy = trc3.xz;\n"
			"tmp2 = tex2Dlod(%IMG0, tmp5);\n"
		
		#endif
		
		"# = temp * tbld.x + tmp1 * tbld.y + tmp2 * tbld.z;\n"
	};
	
	static const char secondaryCode[] =
	{
		#if C4OPENGL
		
			"tmp3 = texture2DLod(%IMG0, trc2.xz, tlod.x);\n"
			"tmp4 = texture2DLod(%IMG0, trc2.yw, tlod.y);\n"
			"tmp5 = texture2DLod(%IMG0, trc3.yw, tlod.z);\n"
		
		#else
		
			"tmp3.w = tlod.x;\n"
			"tmp4.w = tlod.y;\n"
			"tmp5.w = tlod.z;\n"
			
			"tmp3.xy = trc2.xz;\n"
			"tmp3 = tex2Dlod(%IMG0, tmp3);\n"
			"tmp4.xy = trc2.yw;\n"
			"tmp4 = tex2Dlod(%IMG0, tmp4);\n"
			"tmp5.xy = trc3.yw;\n"
			"tmp5 = tex2Dlod(%IMG0, tmp5);\n"
		
		#endif
		
		"# = tmp3 * tbld.x + tmp4 * tbld.y + tmp5 * tbld.z;\n"
	};
	
	if (GetTexture()->GetTextureTargetIndex() == Render::kTextureTargetArray2D)
	{
		if (blendMode == kTerrainBlendFull) shaderCode[0] = fullArrayCode;
		else if (blendMode == kTerrainBlendPrimary) shaderCode[0] = primaryArrayCode;
		else shaderCode[0] = secondaryArrayCode;
	}
	else
	{
		if (blendMode == kTerrainBlendFull) shaderCode[0] = fullCode;
		else if (blendMode == kTerrainBlendPrimary) shaderCode[0] = primaryCode;
		else shaderCode[0] = secondaryCode;
	}
	
	return (1);
}


TerrainNormalProcess::TerrainNormalProcess() : TerrainTextureProcess(kProcessTerrainNormal)
{
}

TerrainNormalProcess::TerrainNormalProcess(const TerrainNormalProcess& terrainNormalProcess) : TerrainTextureProcess(terrainNormalProcess)
{
}

TerrainNormalProcess::~TerrainNormalProcess()
{
}

Process *TerrainNormalProcess::Replicate(void) const
{
	return (new TerrainNormalProcess(*this));
}

void TerrainNormalProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->temporaryCount = 1;
	data->outputSize = 3;
	
	data->textureCount = 1;
	data->textureObject[0] = GetTexture();
}

int32 TerrainNormalProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char fullArrayCode[] =
	{
		"TEX		temp, trc1.xzww, %IMG0, ARRAY2D;\n"
		"TEX		tmp1, trc2.xzww, %IMG0, ARRAY2D;\n"
		"MAD		temp, temp, 2.0, -1.0;\n"
		"MAD		tmp1, tmp1, 2.0, -1.0;\n"
		
		"LRP		#, fragment.color.z, tmp1, temp;\n"
	};
	
	static const char primaryArrayCode[] =
	{
		"TEX		temp, trc1.xzww, %IMG0, ARRAY2D;\n"
		"MAD		#, temp, 2.0, -1.0;\n"
	};
	
	static const char secondaryArrayCode[] =
	{
		"TEX		tmp1, trc2.xzww, %IMG0, ARRAY2D;\n"
		"MAD		#, tmp1, 2.0, -1.0;\n"
	};
	
	static const char fullArrayCompressedCode[] =
	{
		"TEX		temp.yw, trc1.xzww, %IMG0, ARRAY2D;\n"
		"TEX		tmp1.yw, trc2.xzww, %IMG0, ARRAY2D;\n"
		
		"MAD		temp.xy, temp.wyyy, 2.0, -1.0;\n"
		"DP2A		temp.z, temp, temp, -1.0;\n"
		"MAX		temp.z, -temp.z, 0.03125;\n"
		"RSQ		temp.w, temp.z;\n"
		"MUL		temp.z, temp.z, temp.w;\n"
		
		"MAD		tmp1.xy, tmp1.wyyy, 2.0, -1.0;\n"
		"DP2A		tmp1.z, tmp1, tmp1, -1.0;\n"
		"MAX		tmp1.z, -tmp1.z, 0.03125;\n"
		"RSQ		tmp1.w, tmp1.z;\n"
		"MUL		tmp1.z, tmp1.z, tmp1.w;\n"
		
		"LRP		#, fragment.color.z, tmp1, temp;\n"
	};
	
	static const char primaryArrayCompressedCode[] =
	{
		"TEX		temp.yw, trc1.xzww, %IMG0, ARRAY2D;\n"
		
		"MAD		#.xy, temp.wyyy, 2.0, -1.0;\n"
		"DP2A		temp.z, ##, ##, -1.0;\n"
		"MAX		temp.z, -temp.z, 0.03125;\n"
		"RSQ		temp.w, temp.z;\n"
		"MUL		#.z, temp.z, temp.w;\n"
	};
	
	static const char secondaryArrayCompressedCode[] =
	{
		"TEX		tmp1.yw, trc2.xzww, %IMG0, ARRAY2D;\n"
		
		"MAD		#.xy, tmp1.wyyy, 2.0, -1.0;\n"
		"DP2A		tmp1.z, ##, ##, -1.0;\n"
		"MAX		tmp1.z, -tmp1.z, 0.03125;\n"
		"RSQ		tmp1.w, tmp1.z;\n"
		"MUL		#.z, tmp1.z, tmp1.w;\n"
	};
	
	static const char fullCode[] =
	{
		"MOV		tmp1.w, tlod.x;\n"
		
		"MOV		tmp1.xy, trc1.xzzz;\n"
		"TXL		temp, tmp1, %IMG0, 2D;\n"
		"MOV		tmp1.xy, trc2.xzzz;\n"
		"TXL		tmp1, tmp1, %IMG0, 2D;\n"
		"MAD		temp, temp, 2.0, -1.0;\n"
		"MAD		tmp1, tmp1, 2.0, -1.0;\n"
		
		"LRP		#, fragment.color.z, tmp1, temp;\n"
	};
	
	static const char primaryCode[] =
	{
		"MOV		tmp1.w, tlod.x;\n"
		
		"MOV		tmp1.xy, trc1.xzzz;\n"
		"TXL		temp, tmp1, %IMG0, 2D;\n"
		"MAD		#, temp, 2.0, -1.0;\n"
	};
	
	static const char secondaryCode[] =
	{
		"MOV		tmp1.w, tlod.x;\n"
		
		"MOV		tmp1.xy, trc2.xzzz;\n"
		"TXL		tmp1, tmp1, %IMG0, 2D;\n"
		"MAD		#, tmp1, 2.0, -1.0;\n"
	};
	
	static const char fullCompressedCode[] =
	{
		"MOV		tmp1.w, tlod.x;\n"
		
		"MOV		tmp1.xy, trc1.xzzz;\n"
		"TXL		temp.yw, tmp1, %IMG0, 2D;\n"
		"MOV		tmp1.xy, trc2.xzzz;\n"
		"TXL		tmp1.yw, tmp1, %IMG0, 2D;\n"
		
		"MAD		temp.xyz, temp.wyyy, {2.0, 2.0, 0.0, 0.0}, {-1.0, -1.0, 0.0, 0.0};\n"
		"DP3		temp.z, temp, temp;\n"
		"SUB		temp.z, 1.0, temp.z;\n"
		"MAX		temp.z, temp.z, 0.03125;\n"
		"RSQ		temp.w, temp.z;\n"
		"MUL		temp.z, temp.z, temp.w;\n"
		
		"MAD		tmp1.xyz, tmp1.wyyy, {2.0, 2.0, 0.0, 0.0}, {-1.0, -1.0, 0.0, 0.0};\n"
		"DP3		tmp1.z, tmp1, tmp1;\n"
		"SUB		tmp1.z, 1.0, tmp1.z;\n"
		"MAX		tmp1.z, tmp1.z, 0.03125;\n"
		"RSQ		tmp1.w, tmp1.z;\n"
		"MUL		tmp1.z, tmp1.z, tmp1.w;\n"
		
		"LRP		#, fragment.color.z, tmp1, temp;\n"
	};
	
	static const char primaryCompressedCode[] =
	{
		"MOV		tmp1.w, tlod.x;\n"
		
		"MOV		tmp1.xy, trc1.xzzz;\n"
		"TXL		temp.yw, tmp1, %IMG0, 2D;\n"
		
		"MAD		#.xyz, temp.wyyy, {2.0, 2.0, 0.0, 0.0}, {-1.0, -1.0, 0.0, 0.0};\n"
		"DP3		temp.z, ##, ##;\n"
		"SUB		temp.z, 1.0, temp.z;\n"
		"MAX		temp.z, temp.z, 0.03125;\n"
		"RSQ		temp.w, temp.z;\n"
		"MUL		#.z, temp.z, temp.w;\n"
	};
	
	static const char secondaryCompressedCode[] =
	{
		"MOV		tmp1.w, tlod.x;\n"
		
		"MOV		tmp1.xy, trc2.xzzz;\n"
		"TXL		tmp1.yw, tmp1, %IMG0, 2D;\n"
		
		"MAD		#.xyz, tmp1.wyyy, {2.0, 2.0, 0.0, 0.0}, {-1.0, -1.0, 0.0, 0.0};\n"
		"DP3		tmp1.z, ##, ##;\n"
		"SUB		tmp1.z, 1.0, tmp1.z;\n"
		"MAX		tmp1.z, tmp1.z, 0.03125;\n"
		"RSQ		tmp1.w, tmp1.z;\n"
		"MUL		#.z, tmp1.z, tmp1.w;\n"
	};
	
	static const char fullCompressedCode2[] =
	{
		"MOV		tmp1.w, tlod.x;\n"
		
		"MOV		tmp1.xy, trc1.xzzz;\n"
		"TXL		temp.yw, tmp1, %IMG0, 2D;\n"
		"MOV		tmp1.xy, trc2.xzzz;\n"
		"TXL		tmp1.yw, tmp1, %IMG0, 2D;\n"
		
		"MAD		temp.xy, temp.wyyy, 2.0, -1.0;\n"
		"DP2A		temp.z, temp, temp, -1.0;\n"
		"MAX		temp.z, -temp.z, 0.03125;\n"
		"RSQ		temp.w, temp.z;\n"
		"MUL		temp.z, temp.z, temp.w;\n"
		
		"MAD		tmp1.xy, tmp1.wyyy, 2.0, -1.0;\n"
		"DP2A		tmp1.z, tmp1, tmp1, -1.0;\n"
		"MAX		tmp1.z, -tmp1.z, 0.03125;\n"
		"RSQ		tmp1.w, tmp1.z;\n"
		"MUL		tmp1.z, tmp1.z, tmp1.w;\n"
		
		"LRP		#, fragment.color.z, tmp1, temp;\n"
	};
	
	static const char primaryCompressedCode2[] =
	{
		"MOV		tmp1.w, tlod.x;\n"
		
		"MOV		tmp1.xy, trc1.xzzz;\n"
		"TXL		temp.yw, tmp1, %IMG0, 2D;\n"
		
		"MAD		#.xy, temp.wyyy, 2.0, -1.0;\n"
		"DP2A		temp.z, ##, ##, -1.0;\n"
		"MAX		temp.z, -temp.z, 0.03125;\n"
		"RSQ		temp.w, temp.z;\n"
		"MUL		#.z, temp.z, temp.w;\n"
	};
	
	static const char secondaryCompressedCode2[] =
	{
		"MOV		tmp1.w, tlod.x;\n"
		
		"MOV		tmp1.xy, trc2.xzzz;\n"
		"TXL		tmp1.yw, tmp1, %IMG0, 2D;\n"
		
		"MAD		#.xy, tmp1.wyyy, 2.0, -1.0;\n"
		"DP2A		tmp1.z, ##, ##, -1.0;\n"
		"MAX		tmp1.z, -tmp1.z, 0.03125;\n"
		"RSQ		tmp1.w, tmp1.z;\n"
		"MUL		#.z, tmp1.z, tmp1.w;\n"
	};
	
	const Texture *texture = GetTexture();
	if (texture->GetTextureTargetIndex() == Render::kTextureTargetArray2D)
	{
		if (texture->GetImageFormat() == kTextureBC13)
		{
			if (blendMode == kTerrainBlendFull) programCode[0] = fullArrayCompressedCode;
			else if (blendMode == kTerrainBlendPrimary) programCode[0] = primaryArrayCompressedCode;
			else programCode[0] = secondaryArrayCompressedCode;
		}
		else
		{
			if (blendMode == kTerrainBlendFull) programCode[0] = fullArrayCode;
			else if (blendMode == kTerrainBlendPrimary) programCode[0] = primaryArrayCode;
			else programCode[0] = secondaryArrayCode;
		}
	}
	else
	{
		if (texture->GetImageFormat() == kTextureBC13)
		{
			if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2])
			{
				if (blendMode == kTerrainBlendFull) programCode[0] = fullCompressedCode2;
				else if (blendMode == kTerrainBlendPrimary) programCode[0] = primaryCompressedCode2;
				else programCode[0] = secondaryCompressedCode2;
			}
			else
			{
				if (blendMode == kTerrainBlendFull) programCode[0] = fullCompressedCode;
				else if (blendMode == kTerrainBlendPrimary) programCode[0] = primaryCompressedCode;
				else programCode[0] = secondaryCompressedCode;
			}
		}
		else
		{
			if (blendMode == kTerrainBlendFull) programCode[0] = fullCode;
			else if (blendMode == kTerrainBlendPrimary) programCode[0] = primaryCode;
			else programCode[0] = secondaryCode;
		}
	}
	
	return (1);
}

int32 TerrainNormalProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char fullArrayCode[] =
	{
		"temp.xyz = texture2DArray(%IMG0, trc1.xzw).xyz * 2.0 - 1.0;\n"
		"tmp1.xyz = texture2DArray(%IMG0, trc2.xzw).xyz * 2.0 - 1.0;\n"
		
		"# = " LERP "(temp.xyz, tmp1.xyz, " FRAGMENT_COLOR ".z);\n"
	};
	
	static const char primaryArrayCode[] =
	{
		"# = texture2DArray(%IMG0, trc1.xzw).xyz * 2.0 - 1.0;\n"
	};
	
	static const char secondaryArrayCode[] =
	{
		"# = texture2DArray(%IMG0, trc2.xzw).xyz * 2.0 - 1.0;\n"
	};
	
	static const char fullArrayCompressedCode[] =
	{
		"temp.xy = texture2DArray(%IMG0, trc1.xzw).wy * 2.0 - 1.0;\n"
		"temp.z = sqrt(max(1.0 - dot(temp.xy, temp.xy), 0.03125));\n"
		"tmp1.xy = texture2DArray(%IMG0, trc2.xzw).wy * 2.0 - 1.0;\n"
		"tmp1.z = sqrt(max(1.0 - dot(tmp1.xy, tmp1.xy), 0.03125));\n"
		
		"# = " LERP "(temp.xyz, tmp1.xyz, " FRAGMENT_COLOR ".z);\n"
	};
	
	static const char primaryArrayCompressedCode[] =
	{
		"#.xy = texture2DArray(%IMG0, trc1.xzw).wy * 2.0 - 1.0;\n"
		"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
	};
	
	static const char secondaryArrayCompressedCode[] =
	{
		"#.xy = texture2DArray(%IMG0, trc2.xzw).wy * 2.0 - 1.0;\n"
		"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
	};
	
	static const char fullCode[] =
	{
		#if C4OPENGL
		
			"temp.xyz = texture2DLod(%IMG0, trc1.xz, tlod.x).xyz * 2.0 - 1.0;\n"
			"tmp1.xyz = texture2DLod(%IMG0, trc2.xz, tlod.x).xyz * 2.0 - 1.0;\n"
		
		#else
		
			"tmp1.w = tlod.x;\n"
			
			"tmp1.xy = trc1.xz;\n"
			"temp.xyz = tex2Dlod(%IMG0, tmp1).xyz * 2.0 - 1.0;\n"
			"tmp1.xy = trc2.xz;\n"
			"tmp1.xyz = tex2Dlod(%IMG0, tmp1).xyz * 2.0 - 1.0;\n"
		
		#endif
		
		"# = " LERP "(temp.xyz, tmp1.xyz, " FRAGMENT_COLOR ".z);\n"
	};
	
	static const char primaryCode[] =
	{
		#if C4OPENGL
		
			"# = texture2DLod(%IMG0, trc1.xz, tlod.x).xyz * 2.0 - 1.0;\n"
		
		#else
		
			"tmp1.w = tlod.x;\n"
			
			"tmp1.xy = trc1.xz;\n"
			"# = tex2Dlod(%IMG0, tmp1).xyz * 2.0 - 1.0;\n"
		
		#endif
	};
	
	static const char secondaryCode[] =
	{
		#if C4OPENGL
		
			"tmp1 = texture2DLod(%IMG0, trc2.xz, tlod.x).xyz * 2.0 - 1.0;\n"
		
		#else
		
			"tmp1.w = tlod.x;\n"
			
			"tmp1.xy = trc2.xz;\n"
			"# = tex2Dlod(%IMG0, tmp1).xyz * 2.0 - 1.0;\n"
		
		#endif
	};
	
	static const char fullCompressedCode[] =
	{
		#if C4OPENGL
		
			"temp.xy = texture2DLod(%IMG0, trc1.xz, tlod.x).wy * 2.0 - 1.0;\n"
			"temp.z = sqrt(max(1.0 - dot(temp.xy, temp.xy), 0.03125));\n"
			"tmp1.xy = texture2DLod(%IMG0, trc2.xz, tlod.x).wy * 2.0 - 1.0;\n"
			"tmp1.z = sqrt(max(1.0 - dot(tmp1.xy, tmp1.xy), 0.03125));\n"
		
		#else
		
			"tmp1.w = tlod.x;\n"
			
			"tmp1.xy = trc1.xz;\n"
			"temp.xy = tex2Dlod(%IMG0, tmp1).wy * 2.0 - 1.0;\n"
			"temp.z = sqrt(max(1.0 - dot(temp.xy, temp.xy), 0.03125));\n"
			"tmp1.xy = trc2.xz;\n"
			"tmp1.xy = tex2Dlod(%IMG0, tmp1).wy * 2.0 - 1.0;\n"
			"tmp1.z = sqrt(max(1.0 - dot(tmp1.xy, tmp1.xy), 0.03125));\n"
		
		#endif
		
		"# = " LERP "(temp.xyz, tmp1.xyz, " FRAGMENT_COLOR ".z);\n"
	};
	
	static const char primaryCompressedCode[] =
	{
		#if C4OPENGL
		
			"#.xy = texture2DLod(%IMG0, trc1.xz, tlod.x).wy * 2.0 - 1.0;\n"
			"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
		
		#else
		
			"tmp1.w = tlod.x;\n"
			
			"tmp1.xy = trc1.xz;\n"
			"#.xy = tex2Dlod(%IMG0, tmp1).wy * 2.0 - 1.0;\n"
			"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
		
		#endif
	};
	
	static const char secondaryCompressedCode[] =
	{
		#if C4OPENGL
		
			"#.xy = texture2DLod(%IMG0, trc2.xz, tlod.x).wy * 2.0 - 1.0;\n"
			"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
		
		#else
		
			"tmp1.w = tlod.x;\n"
			
			"tmp1.xy = trc2.xz;\n"
			"#.xy = tex2Dlod(%IMG0, tmp1).wy * 2.0 - 1.0;\n"
			"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
		
		#endif
	};
	
	const Texture *texture = GetTexture();
	if (texture->GetTextureTargetIndex() == Render::kTextureTargetArray2D)
	{
		if (texture->GetImageFormat() == kTextureBC13)
		{
			if (blendMode == kTerrainBlendFull) shaderCode[0] = fullArrayCompressedCode;
			else if (blendMode == kTerrainBlendPrimary) shaderCode[0] = primaryArrayCompressedCode;
			else shaderCode[0] = secondaryArrayCompressedCode;
		}
		else
		{
			if (blendMode == kTerrainBlendFull) shaderCode[0] = fullArrayCode;
			else if (blendMode == kTerrainBlendPrimary) shaderCode[0] = primaryArrayCode;
			else shaderCode[0] = secondaryArrayCode;
		}
	}
	else
	{
		if (texture->GetImageFormat() == kTextureBC13)
		{
			if (blendMode == kTerrainBlendFull) shaderCode[0] = fullCompressedCode;
			else if (blendMode == kTerrainBlendPrimary) shaderCode[0] = primaryCompressedCode;
			else shaderCode[0] = secondaryCompressedCode;
		}
		else
		{
			if (blendMode == kTerrainBlendFull) shaderCode[0] = fullCode;
			else if (blendMode == kTerrainBlendPrimary) shaderCode[0] = primaryCode;
			else shaderCode[0] = secondaryCode;
		}
	}
	
	return (1);
}


TerrainNormal2Process::TerrainNormal2Process() : TerrainTextureProcess(kProcessTerrainNormal2)
{
}

TerrainNormal2Process::TerrainNormal2Process(const TerrainNormal2Process& terrainNormal2Process) : TerrainTextureProcess(terrainNormal2Process)
{
}

TerrainNormal2Process::~TerrainNormal2Process()
{
}

Process *TerrainNormal2Process::Replicate(void) const
{
	return (new TerrainNormal2Process(*this));
}

void TerrainNormal2Process::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->temporaryCount = 1;
	data->outputSize = 3;
	
	data->textureCount = 1;
	data->textureObject[0] = GetTexture();
}

int32 TerrainNormal2Process::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char fullArrayCode[] =
	{
		"TEX		temp, trc1.yzww, %IMG0, ARRAY2D;\n"
		"TEX		tmp1, trc2.yzww, %IMG0, ARRAY2D;\n"
		"MAD		temp, temp, 2.0, -1.0;\n"
		"MAD		tmp1, tmp1, 2.0, -1.0;\n"
		
		"LRP		#, fragment.color.z, tmp1, temp;\n"
	};
	
	static const char primaryArrayCode[] =
	{
		"TEX		temp, trc1.yzww, %IMG0, ARRAY2D;\n"
		"MAD		#, temp, 2.0, -1.0;\n"
	};
	
	static const char secondaryArrayCode[] =
	{
		"TEX		tmp1, trc2.yzww, %IMG0, ARRAY2D;\n"
		"MAD		#, tmp1, 2.0, -1.0;\n"
	};
	
	static const char fullArrayCompressedCode[] =
	{
		"TEX		temp.yw, trc1.yzww, %IMG0, ARRAY2D;\n"
		"TEX		tmp1.yw, trc2.yzww, %IMG0, ARRAY2D;\n"
		
		"MAD		temp.xy, temp.wyyy, 2.0, -1.0;\n"
		"DP2A		temp.z, temp, temp, -1.0;\n"
		"MAX		temp.z, -temp.z, 0.03125;\n"
		"RSQ		temp.w, temp.z;\n"
		"MUL		temp.z, temp.z, temp.w;\n"
		
		"MAD		tmp1.xy, tmp1.wyyy, 2.0, -1.0;\n"
		"DP2A		tmp1.z, tmp1, tmp1, -1.0;\n"
		"MAX		tmp1.z, -tmp1.z, 0.03125;\n"
		"RSQ		tmp1.w, tmp1.z;\n"
		"MUL		tmp1.z, tmp1.z, tmp1.w;\n"
		
		"LRP		#, fragment.color.z, tmp1, temp;\n"
	};
	
	static const char primaryArrayCompressedCode[] =
	{
		"TEX		temp.yw, trc1.yzww, %IMG0, ARRAY2D;\n"
		
		"MAD		#.xy, temp.wyyy, 2.0, -1.0;\n"
		"DP2A		temp.z, ##, ##, -1.0;\n"
		"MAX		temp.z, -temp.z, 0.03125;\n"
		"RSQ		temp.w, temp.z;\n"
		"MUL		#.z, temp.z, temp.w;\n"
	};
	
	static const char secondaryArrayCompressedCode[] =
	{
		"TEX		tmp1.yw, trc2.yzww, %IMG0, ARRAY2D;\n"
		
		"MAD		#.xy, tmp1.wyyy, 2.0, -1.0;\n"
		"DP2A		tmp1.z, ##, ##, -1.0;\n"
		"MAX		tmp1.z, -tmp1.z, 0.03125;\n"
		"RSQ		tmp1.w, tmp1.z;\n"
		"MUL		#.z, tmp1.z, tmp1.w;\n"
	};
	
	static const char fullCode[] =
	{
		"MOV		tmp1.w, tlod.y;\n"
		
		"MOV		tmp1.xy, trc1.ywww;\n"
		"TXL		temp, tmp1, %IMG0, 2D;\n"
		"MOV		tmp1.xy, trc2.ywww;\n"
		"TXL		tmp1, tmp1, %IMG0, 2D;\n"
		"MAD		temp, temp, 2.0, -1.0;\n"
		"MAD		tmp1, tmp1, 2.0, -1.0;\n"
		
		"LRP		#, fragment.color.z, tmp1, temp;\n"
	};
	
	static const char primaryCode[] =
	{
		"MOV		tmp1.w, tlod.y;\n"
		
		"MOV		tmp1.xy, trc1.ywww;\n"
		"TXL		temp, tmp1, %IMG0, 2D;\n"
		"MAD		#, temp, 2.0, -1.0;\n"
	};
	
	static const char secondaryCode[] =
	{
		"MOV		tmp1.w, tlod.y;\n"
		
		"MOV		tmp1.xy, trc2.ywww;\n"
		"TXL		tmp1, tmp1, %IMG0, 2D;\n"
		"MAD		#, tmp1, 2.0, -1.0;\n"
	};
	
	static const char fullCompressedCode[] =
	{
		"MOV		tmp1.w, tlod.y;\n"
		
		"MOV		tmp1.xy, trc1.ywww;\n"
		"TXL		temp.yw, tmp1, %IMG0, 2D;\n"
		"MOV		tmp1.xy, trc2.ywww;\n"
		"TXL		tmp1.yw, tmp1, %IMG0, 2D;\n"
		
		"MAD		temp.xyz, temp.wyyy, {2.0, 2.0, 0.0, 0.0}, {-1.0, -1.0, 0.0, 0.0};\n"
		"DP3		temp.z, temp, temp;\n"
		"SUB		temp.z, 1.0, temp.z;\n"
		"MAX		temp.z, temp.z, 0.03125;\n"
		"RSQ		temp.w, temp.z;\n"
		"MUL		temp.z, temp.z, temp.w;\n"
		
		"MAD		tmp1.xyz, tmp1.wyyy, {2.0, 2.0, 0.0, 0.0}, {-1.0, -1.0, 0.0, 0.0};\n"
		"DP3		tmp1.z, tmp1, tmp1;\n"
		"SUB		tmp1.z, 1.0, tmp1.z;\n"
		"MAX		tmp1.z, tmp1.z, 0.03125;\n"
		"RSQ		tmp1.w, tmp1.z;\n"
		"MUL		tmp1.z, tmp1.z, tmp1.w;\n"
		
		"LRP		#, fragment.color.z, tmp1, temp;\n"
	};
	
	static const char primaryCompressedCode[] =
	{
		"MOV		tmp1.w, tlod.y;\n"
		
		"MOV		tmp1.xy, trc1.ywww;\n"
		"TXL		temp.yw, tmp1, %IMG0, 2D;\n"
		
		"MAD		#.xyz, temp.wyyy, {2.0, 2.0, 0.0, 0.0}, {-1.0, -1.0, 0.0, 0.0};\n"
		"DP3		temp.z, ##, ##;\n"
		"SUB		temp.z, 1.0, temp.z;\n"
		"MAX		temp.z, temp.z, 0.03125;\n"
		"RSQ		temp.w, temp.z;\n"
		"MUL		#.z, temp.z, temp.w;\n"
	};
	
	static const char secondaryCompressedCode[] =
	{
		"MOV		tmp1.w, tlod.y;\n"
		
		"MOV		tmp1.xy, trc2.ywww;\n"
		"TXL		tmp1.yw, tmp1, %IMG0, 2D;\n"
		
		"MAD		#.xyz, tmp1.wyyy, {2.0, 2.0, 0.0, 0.0}, {-1.0, -1.0, 0.0, 0.0};\n"
		"DP3		tmp1.z, ##, ##;\n"
		"SUB		tmp1.z, 1.0, tmp1.z;\n"
		"MAX		tmp1.z, tmp1.z, 0.03125;\n"
		"RSQ		tmp1.w, tmp1.z;\n"
		"MUL		#.z, tmp1.z, tmp1.w;\n"
	};
	
	static const char fullCompressedCode2[] =
	{
		"MOV		tmp1.w, tlod.y;\n"
		
		"MOV		tmp1.xy, trc1.ywww;\n"
		"TXL		temp.yw, tmp1, %IMG0, 2D;\n"
		"MOV		tmp1.xy, trc2.ywww;\n"
		"TXL		tmp1.yw, tmp1, %IMG0, 2D;\n"
		
		"MAD		temp.xy, temp.wyyy, 2.0, -1.0;\n"
		"DP2A		temp.z, temp, temp, -1.0;\n"
		"MAX		temp.z, -temp.z, 0.03125;\n"
		"RSQ		temp.w, temp.z;\n"
		"MUL		temp.z, temp.z, temp.w;\n"
		
		"MAD		tmp1.xy, tmp1.wyyy, 2.0, -1.0;\n"
		"DP2A		tmp1.z, tmp1, tmp1, -1.0;\n"
		"MAX		tmp1.z, -tmp1.z, 0.03125;\n"
		"RSQ		tmp1.w, tmp1.z;\n"
		"MUL		tmp1.z, tmp1.z, tmp1.w;\n"
		
		"LRP		#, fragment.color.z, tmp1, temp;\n"
	};
	
	static const char primaryCompressedCode2[] =
	{
		"MOV		tmp1.w, tlod.y;\n"
		
		"MOV		tmp1.xy, trc1.ywww;\n"
		"TXL		temp.yw, tmp1, %IMG0, 2D;\n"
		
		"MAD		#.xy, temp.wyyy, 2.0, -1.0;\n"
		"DP2A		temp.z, ##, ##, -1.0;\n"
		"MAX		temp.z, -temp.z, 0.03125;\n"
		"RSQ		temp.w, temp.z;\n"
		"MUL		#.z, temp.z, temp.w;\n"
	};
	
	static const char secondaryCompressedCode2[] =
	{
		"MOV		tmp1.w, tlod.y;\n"
		
		"MOV		tmp1.xy, trc2.ywww;\n"
		"TXL		tmp1.yw, tmp1, %IMG0, 2D;\n"
		
		"MAD		#.xy, tmp1.wyyy, 2.0, -1.0;\n"
		"DP2A		tmp1.z, ##, ##, -1.0;\n"
		"MAX		tmp1.z, -tmp1.z, 0.03125;\n"
		"RSQ		tmp1.w, tmp1.z;\n"
		"MUL		#.z, tmp1.z, tmp1.w;\n"
	};
	
	const Texture *texture = GetTexture();
	if (texture->GetTextureTargetIndex() == Render::kTextureTargetArray2D)
	{
		if (texture->GetImageFormat() == kTextureBC13)
		{
			if (blendMode == kTerrainBlendFull) programCode[0] = fullArrayCompressedCode;
			else if (blendMode == kTerrainBlendPrimary) programCode[0] = primaryArrayCompressedCode;
			else programCode[0] = secondaryArrayCompressedCode;
		}
		else
		{
			if (blendMode == kTerrainBlendFull) programCode[0] = fullArrayCode;
			else if (blendMode == kTerrainBlendPrimary) programCode[0] = primaryArrayCode;
			else programCode[0] = secondaryArrayCode;
		}
	}
	else
	{
		if (texture->GetImageFormat() == kTextureBC13)
		{
			if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2])
			{
				if (blendMode == kTerrainBlendFull) programCode[0] = fullCompressedCode2;
				else if (blendMode == kTerrainBlendPrimary) programCode[0] = primaryCompressedCode2;
				else programCode[0] = secondaryCompressedCode2;
			}
			else
			{
				if (blendMode == kTerrainBlendFull) programCode[0] = fullCompressedCode;
				else if (blendMode == kTerrainBlendPrimary) programCode[0] = primaryCompressedCode;
				else programCode[0] = secondaryCompressedCode;
			}
		}
		else
		{
			if (blendMode == kTerrainBlendFull) programCode[0] = fullCode;
			else if (blendMode == kTerrainBlendPrimary) programCode[0] = primaryCode;
			else programCode[0] = secondaryCode;
		}
	}
	
	return (1);
}

int32 TerrainNormal2Process::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char fullArrayCode[] =
	{
		"temp.xyz = texture2DArray(%IMG0, trc1.yzw).xyz * 2.0 - 1.0;\n"
		"tmp1.xyz = texture2DArray(%IMG0, trc2.yzw).xyz * 2.0 - 1.0;\n"
		
		"# = " LERP "(temp.xyz, tmp1.xyz, " FRAGMENT_COLOR ".z);\n"
	};
	
	static const char primaryArrayCode[] =
	{
		"# = texture2DArray(%IMG0, trc1.yzw).xyz * 2.0 - 1.0;\n"
	};
	
	static const char secondaryArrayCode[] =
	{
		"# = texture2DArray(%IMG0, trc2.yzw).xyz * 2.0 - 1.0;\n"
	};
	
	static const char fullArrayCompressedCode[] =
	{
		"temp.xy = texture2DArray(%IMG0, trc1.yzw).wy * 2.0 - 1.0;\n"
		"temp.z = sqrt(max(1.0 - dot(temp.xy, temp.xy), 0.03125));\n"
		"tmp1.xy = texture2DArray(%IMG0, trc2.yzw).wy * 2.0 - 1.0;\n"
		"tmp1.z = sqrt(max(1.0 - dot(tmp1.xy, tmp1.xy), 0.03125));\n"
		
		"# = " LERP "(temp.xyz, tmp1.xyz, " FRAGMENT_COLOR ".z);\n"
	};
	
	static const char primaryArrayCompressedCode[] =
	{
		"#.xy = texture2DArray(%IMG0, trc1.yzw).wy * 2.0 - 1.0;\n"
		"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
	};
	
	static const char secondaryArrayCompressedCode[] =
	{
		"#.xy = texture2DArray(%IMG0, trc2.yzw).wy * 2.0 - 1.0;\n"
		"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
	};
	
	static const char fullCode[] =
	{
		#if C4OPENGL
		
			"temp.xyz = texture2DLod(%IMG0, trc1.yw, tlod.y).xyz * 2.0 - 1.0;\n"
			"tmp1.xyz = texture2DLod(%IMG0, trc2.yw, tlod.y).xyz * 2.0 - 1.0;\n"
		
		#else
		
			"tmp1.w = tlod.y;\n"
			
			"tmp1.xy = trc1.yw;\n"
			"temp.xyz = tex2Dlod(%IMG0, tmp1).xyz * 2.0 - 1.0;\n"
			"tmp1.xy = trc2.yw;\n"
			"tmp1.xyz = tex2Dlod(%IMG0, tmp1).xyz * 2.0 - 1.0;\n"
		
		#endif
		
		"# = " LERP "(temp.xyz, tmp1.xyz, " FRAGMENT_COLOR ".z);\n"
	};
	
	static const char primaryCode[] =
	{
		#if C4OPENGL
		
			"# = texture2DLod(%IMG0, trc1.yw, tlod.y).xyz * 2.0 - 1.0;\n"
		
		#else
		
			"tmp1.w = tlod.y;\n"
			
			"tmp1.xy = trc1.yw;\n"
			"# = tex2Dlod(%IMG0, tmp1).xyz * 2.0 - 1.0;\n"
		
		#endif
	};
	
	static const char secondaryCode[] =
	{
		#if C4OPENGL
		
			"tmp1 = texture2DLod(%IMG0, trc2.yw, tlod.y).xyz * 2.0 - 1.0;\n"
		
		#else
		
			"tmp1.w = tlod.y;\n"
			
			"tmp1.xy = trc2.yw;\n"
			"# = tex2Dlod(%IMG0, tmp1).xyz * 2.0 - 1.0;\n"
		
		#endif
	};
	
	static const char fullCompressedCode[] =
	{
		#if C4OPENGL
		
			"temp.xy = texture2DLod(%IMG0, trc1.yw, tlod.y).wy * 2.0 - 1.0;\n"
			"temp.z = sqrt(max(1.0 - dot(temp.xy, temp.xy), 0.03125));\n"
			"tmp1.xy = texture2DLod(%IMG0, trc2.yw, tlod.y).wy * 2.0 - 1.0;\n"
			"tmp1.z = sqrt(max(1.0 - dot(tmp1.xy, tmp1.xy), 0.03125));\n"
		
		#else
		
			"tmp1.w = tlod.y;\n"
			
			"tmp1.xy = trc1.yw;\n"
			"temp.xy = tex2Dlod(%IMG0, tmp1).wy * 2.0 - 1.0;\n"
			"temp.z = sqrt(max(1.0 - dot(temp.xy, temp.xy), 0.03125));\n"
			"tmp1.xy = trc2.yw;\n"
			"tmp1.xy = tex2Dlod(%IMG0, tmp1).wy * 2.0 - 1.0;\n"
			"tmp1.z = sqrt(max(1.0 - dot(tmp1.xy, tmp1.xy), 0.03125));\n"
		
		#endif
		
		"# = " LERP "(temp.xyz, tmp1.xyz, " FRAGMENT_COLOR ".z);\n"
	};
	
	static const char primaryCompressedCode[] =
	{
		#if C4OPENGL
		
			"#.xy = texture2DLod(%IMG0, trc1.yw, tlod.y).wy * 2.0 - 1.0;\n"
			"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
		
		#else
		
			"tmp1.w = tlod.y;\n"
			
			"tmp1.xy = trc1.yw;\n"
			"#.xy = tex2Dlod(%IMG0, tmp1).wy * 2.0 - 1.0;\n"
			"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
		
		#endif
	};
	
	static const char secondaryCompressedCode[] =
	{
		#if C4OPENGL
		
			"#.xy = texture2DLod(%IMG0, trc2.yw, tlod.y).wy * 2.0 - 1.0;\n"
			"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
		
		#else
		
			"tmp1.w = tlod.y;\n"
			
			"tmp1.xy = trc2.yw;\n"
			"#.xy = tex2Dlod(%IMG0, tmp1).wy * 2.0 - 1.0;\n"
			"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
		
		#endif
	};
	
	const Texture *texture = GetTexture();
	if (texture->GetTextureTargetIndex() == Render::kTextureTargetArray2D)
	{
		if (texture->GetImageFormat() == kTextureBC13)
		{
			if (blendMode == kTerrainBlendFull) shaderCode[0] = fullArrayCompressedCode;
			else if (blendMode == kTerrainBlendPrimary) shaderCode[0] = primaryArrayCompressedCode;
			else shaderCode[0] = secondaryArrayCompressedCode;
		}
		else
		{
			if (blendMode == kTerrainBlendFull) shaderCode[0] = fullArrayCode;
			else if (blendMode == kTerrainBlendPrimary) shaderCode[0] = primaryArrayCode;
			else shaderCode[0] = secondaryArrayCode;
		}
	}
	else
	{
		if (texture->GetImageFormat() == kTextureBC13)
		{
			if (blendMode == kTerrainBlendFull) shaderCode[0] = fullCompressedCode;
			else if (blendMode == kTerrainBlendPrimary) shaderCode[0] = primaryCompressedCode;
			else shaderCode[0] = secondaryCompressedCode;
		}
		else
		{
			if (blendMode == kTerrainBlendFull) shaderCode[0] = fullCode;
			else if (blendMode == kTerrainBlendPrimary) shaderCode[0] = primaryCode;
			else shaderCode[0] = secondaryCode;
		}
	}
	
	return (1);
}


TerrainNormal3Process::TerrainNormal3Process() : TerrainTextureProcess(kProcessTerrainNormal3)
{
}

TerrainNormal3Process::TerrainNormal3Process(const TerrainNormal3Process& terrainNormal3Process) : TerrainTextureProcess(terrainNormal3Process)
{
}

TerrainNormal3Process::~TerrainNormal3Process()
{
}

Process *TerrainNormal3Process::Replicate(void) const
{
	return (new TerrainNormal3Process(*this));
}

void TerrainNormal3Process::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->temporaryCount = 1;
	data->outputSize = 3;
	
	data->textureCount = 1;
	data->textureObject[0] = GetTexture();
}

int32 TerrainNormal3Process::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char fullArrayCode[] =
	{
		"TEX		temp, trc3, %IMG0, ARRAY2D;\n"
		"TEX		tmp1, trc3.xyww, %IMG0, ARRAY2D;\n"
		"MAD		temp, temp, 2.0, -1.0;\n"
		"MAD		tmp1, tmp1, 2.0, -1.0;\n"
		
		"LRP		#, fragment.color.z, tmp1, temp;\n"
	};
	
	static const char primaryArrayCode[] =
	{
		"TEX		temp, trc3, %IMG0, ARRAY2D;\n"
		"MAD		#, temp, 2.0, -1.0;\n"
	};
	
	static const char secondaryArrayCode[] =
	{
		"TEX		tmp1, trc3.xyww, %IMG0, ARRAY2D;\n"
		"MAD		#, tmp1, 2.0, -1.0;\n"
	};
	
	static const char fullArrayCompressedCode[] =
	{
		"TEX		temp.yw, trc3, %IMG0, ARRAY2D;\n"
		"TEX		tmp1.yw, trc3.xyww, %IMG0, ARRAY2D;\n"
		
		"MAD		temp.xy, temp.wyyy, 2.0, -1.0;\n"
		"DP2A		temp.z, temp, temp, -1.0;\n"
		"MAX		temp.z, -temp.z, 0.03125;\n"
		"RSQ		temp.w, temp.z;\n"
		"MUL		temp.z, temp.z, temp.w;\n"
		
		"MAD		tmp1.xy, tmp1.wyyy, 2.0, -1.0;\n"
		"DP2A		tmp1.z, tmp1, tmp1, -1.0;\n"
		"MAX		tmp1.z, -tmp1.z, 0.03125;\n"
		"RSQ		tmp1.w, tmp1.z;\n"
		"MUL		tmp1.z, tmp1.z, tmp1.w;\n"
		
		"LRP		#, fragment.color.z, tmp1, temp;\n"
	};
	
	static const char primaryArrayCompressedCode[] =
	{
		"TEX		temp.yw, trc3, %IMG0, ARRAY2D;\n"
		
		"MAD		#.xy, temp.wyyy, 2.0, -1.0;\n"
		"DP2A		temp.z, ##, ##, -1.0;\n"
		"MAX		temp.z, -temp.z, 0.03125;\n"
		"RSQ		temp.w, temp.z;\n"
		"MUL		#.z, temp.z, temp.w;\n"
	};
	
	static const char secondaryArrayCompressedCode[] =
	{
		"TEX		tmp1.yw, trc3.xyww, %IMG0, ARRAY2D;\n"
		
		"MAD		#.xy, tmp1.wyyy, 2.0, -1.0;\n"
		"DP2A		tmp1.z, ##, ##, -1.0;\n"
		"MAX		tmp1.z, -tmp1.z, 0.03125;\n"
		"RSQ		tmp1.w, tmp1.z;\n"
		"MUL		#.z, tmp1.z, tmp1.w;\n"
	};
	
	static const char fullCode[] =
	{
		"MOV		tmp1.w, tlod.z;\n"
		
		"MOV		tmp1.xy, trc3.xzzz;\n"
		"TXL		temp, tmp1, %IMG0, 2D;\n"
		"MOV		tmp1.xy, trc3.ywww;\n"
		"TXL		tmp1, tmp1, %IMG0, 2D;\n"
		"MAD		temp, temp, 2.0, -1.0;\n"
		"MAD		tmp1, tmp1, 2.0, -1.0;\n"
		
		"LRP		#, fragment.color.z, tmp1, temp;\n"
	};
	
	static const char primaryCode[] =
	{
		"MOV		tmp1.w, tlod.z;\n"
		
		"MOV		tmp1.xy, trc3.xzzz;\n"
		"TXL		temp, tmp1, %IMG0, 2D;\n"
		"MAD		#, temp, 2.0, -1.0;\n"
	};
	
	static const char secondaryCode[] =
	{
		"MOV		tmp1.w, tlod.z;\n"
		
		"MOV		tmp1.xy, trc3.ywww;\n"
		"TXL		tmp1, tmp1, %IMG0, 2D;\n"
		"MAD		#, tmp1, 2.0, -1.0;\n"
	};
	
	static const char fullCompressedCode[] =
	{
		"MOV		tmp1.w, tlod.z;\n"
		
		"MOV		tmp1.xy, trc3.xzzz;\n"
		"TXL		temp.yw, tmp1, %IMG0, 2D;\n"
		"MOV		tmp1.xy, trc3.ywww;\n"
		"TXL		tmp1.yw, tmp1, %IMG0, 2D;\n"
		
		"MAD		temp.xyz, temp.wyyy, {2.0, 2.0, 0.0, 0.0}, {-1.0, -1.0, 0.0, 0.0};\n"
		"DP3		temp.z, temp, temp;\n"
		"SUB		temp.z, 1.0, temp.z;\n"
		"MAX		temp.z, temp.z, 0.03125;\n"
		"RSQ		temp.w, temp.z;\n"
		"MUL		temp.z, temp.z, temp.w;\n"
		
		"MAD		tmp1.xyz, tmp1.wyyy, {2.0, 2.0, 0.0, 0.0}, {-1.0, -1.0, 0.0, 0.0};\n"
		"DP3		tmp1.z, tmp1, tmp1;\n"
		"SUB		tmp1.z, 1.0, tmp1.z;\n"
		"MAX		tmp1.z, tmp1.z, 0.03125;\n"
		"RSQ		tmp1.w, tmp1.z;\n"
		"MUL		tmp1.z, tmp1.z, tmp1.w;\n"
		
		"LRP		#, fragment.color.z, tmp1, temp;\n"
	};
	
	static const char primaryCompressedCode[] =
	{
		"MOV		tmp1.w, tlod.z;\n"
		
		"MOV		tmp1.xy, trc3.xzzz;\n"
		"TXL		temp.yw, tmp1, %IMG0, 2D;\n"
		
		"MAD		#.xyz, temp.wyyy, {2.0, 2.0, 0.0, 0.0}, {-1.0, -1.0, 0.0, 0.0};\n"
		"DP3		temp.z, ##, ##;\n"
		"SUB		temp.z, 1.0, temp.z;\n"
		"MAX		temp.z, temp.z, 0.03125;\n"
		"RSQ		temp.w, temp.z;\n"
		"MUL		#.z, temp.z, temp.w;\n"
	};
	
	static const char secondaryCompressedCode[] =
	{
		"MOV		tmp1.w, tlod.z;\n"
		
		"MOV		tmp1.xy, trc3.ywww;\n"
		"TXL		tmp1.yw, tmp1, %IMG0, 2D;\n"
		
		"MAD		#.xyz, tmp1.wyyy, {2.0, 2.0, 0.0, 0.0}, {-1.0, -1.0, 0.0, 0.0};\n"
		"DP3		tmp1.z, ##, ##;\n"
		"SUB		tmp1.z, 1.0, tmp1.z;\n"
		"MAX		tmp1.z, tmp1.z, 0.03125;\n"
		"RSQ		tmp1.w, tmp1.z;\n"
		"MUL		#.z, tmp1.z, tmp1.w;\n"
	};
	
	static const char fullCompressedCode2[] =
	{
		"MOV		tmp1.w, tlod.z;\n"
		
		"MOV		tmp1.xy, trc3.xzzz;\n"
		"TXL		temp.yw, tmp1, %IMG0, 2D;\n"
		"MOV		tmp1.xy, trc3.ywww;\n"
		"TXL		tmp1.yw, tmp1, %IMG0, 2D;\n"
		
		"MAD		temp.xy, temp.wyyy, 2.0, -1.0;\n"
		"DP2A		temp.z, temp, temp, -1.0;\n"
		"MAX		temp.z, -temp.z, 0.03125;\n"
		"RSQ		temp.w, temp.z;\n"
		"MUL		temp.z, temp.z, temp.w;\n"
		
		"MAD		tmp1.xy, tmp1.wyyy, 2.0, -1.0;\n"
		"DP2A		tmp1.z, tmp1, tmp1, -1.0;\n"
		"MAX		tmp1.z, -tmp1.z, 0.03125;\n"
		"RSQ		tmp1.w, tmp1.z;\n"
		"MUL		tmp1.z, tmp1.z, tmp1.w;\n"
		
		"LRP		#, fragment.color.z, tmp1, temp;\n"
	};
	
	static const char primaryCompressedCode2[] =
	{
		"MOV		tmp1.w, tlod.z;\n"
		
		"MOV		tmp1.xy, trc3.xzzz;\n"
		"TXL		temp.yw, tmp1, %IMG0, 2D;\n"
		
		"MAD		#.xy, temp.wyyy, 2.0, -1.0;\n"
		"DP2A		temp.z, ##, ##, -1.0;\n"
		"MAX		temp.z, -temp.z, 0.03125;\n"
		"RSQ		temp.w, temp.z;\n"
		"MUL		#.z, temp.z, temp.w;\n"
	};
	
	static const char secondaryCompressedCode2[] =
	{
		"MOV		tmp1.w, tlod.z;\n"
		
		"MOV		tmp1.xy, trc3.ywww;\n"
		"TXL		tmp1.yw, tmp1, %IMG0, 2D;\n"
		
		"MAD		#.xy, tmp1.wyyy, 2.0, -1.0;\n"
		"DP2A		tmp1.z, ##, ##, -1.0;\n"
		"MAX		tmp1.z, -tmp1.z, 0.03125;\n"
		"RSQ		tmp1.w, tmp1.z;\n"
		"MUL		#.z, tmp1.z, tmp1.w;\n"
	};
	
	const Texture *texture = GetTexture();
	if (texture->GetTextureTargetIndex() == Render::kTextureTargetArray2D)
	{
		if (texture->GetImageFormat() == kTextureBC13)
		{
			if (blendMode == kTerrainBlendFull) programCode[0] = fullArrayCompressedCode;
			else if (blendMode == kTerrainBlendPrimary) programCode[0] = primaryArrayCompressedCode;
			else programCode[0] = secondaryArrayCompressedCode;
		}
		else
		{
			if (blendMode == kTerrainBlendFull) programCode[0] = fullArrayCode;
			else if (blendMode == kTerrainBlendPrimary) programCode[0] = primaryArrayCode;
			else programCode[0] = secondaryArrayCode;
		}
	}
	else
	{
		if (texture->GetImageFormat() == kTextureBC13)
		{
			if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2])
			{
				if (blendMode == kTerrainBlendFull) programCode[0] = fullCompressedCode2;
				else if (blendMode == kTerrainBlendPrimary) programCode[0] = primaryCompressedCode2;
				else programCode[0] = secondaryCompressedCode2;
			}
			else
			{
				if (blendMode == kTerrainBlendFull) programCode[0] = fullCompressedCode;
				else if (blendMode == kTerrainBlendPrimary) programCode[0] = primaryCompressedCode;
				else programCode[0] = secondaryCompressedCode;
			}
		}
		else
		{
			if (blendMode == kTerrainBlendFull) programCode[0] = fullCode;
			else if (blendMode == kTerrainBlendPrimary) programCode[0] = primaryCode;
			else programCode[0] = secondaryCode;
		}
	}
	
	return (1);
}

int32 TerrainNormal3Process::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char fullArrayCode[] =
	{
		"temp.xyz = texture2DArray(%IMG0, trc3.xyz).xyz * 2.0 - 1.0;\n"
		"tmp1.xyz = texture2DArray(%IMG0, trc3.xyw).xyz * 2.0 - 1.0;\n"
		
		"# = " LERP "(temp.xyz, tmp1.xyz, " FRAGMENT_COLOR ".z);\n"
	};
	
	static const char primaryArrayCode[] =
	{
		"# = texture2DArray(%IMG0, trc3.xyz).xyz * 2.0 - 1.0;\n"
	};
	
	static const char secondaryArrayCode[] =
	{
		"# = texture2DArray(%IMG0, trc3.xyw).xyz * 2.0 - 1.0;\n"
	};
	
	static const char fullArrayCompressedCode[] =
	{
		"temp.xy = texture2DArray(%IMG0, trc3.xyz).wy * 2.0 - 1.0;\n"
		"temp.z = sqrt(max(1.0 - dot(temp.xy, temp.xy), 0.03125));\n"
		"tmp1.xy = texture2DArray(%IMG0, trc3.xyw).wy * 2.0 - 1.0;\n"
		"tmp1.z = sqrt(max(1.0 - dot(tmp1.xy, tmp1.xy), 0.03125));\n"
		
		"# = " LERP "(temp.xyz, tmp1.xyz, " FRAGMENT_COLOR ".z);\n"
	};
	
	static const char primaryArrayCompressedCode[] =
	{
		"#.xy = texture2DArray(%IMG0, trc3.xyz).wy * 2.0 - 1.0;\n"
		"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
	};
	
	static const char secondaryArrayCompressedCode[] =
	{
		"#.xy = texture2DArray(%IMG0, trc3.xyw).wy * 2.0 - 1.0;\n"
		"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
	};
	
	static const char fullCode[] =
	{
		#if C4OPENGL
		
			"temp.xyz = texture2DLod(%IMG0, trc3.xz, tlod.z).xyz * 2.0 - 1.0;\n"
			"tmp1.xyz = texture2DLod(%IMG0, trc3.yw, tlod.z).xyz * 2.0 - 1.0;\n"
		
		#else
		
			"tmp1.w = tlod.z;\n"
			
			"tmp1.xy = trc3.xz;\n"
			"temp.xyz = tex2Dlod(%IMG0, tmp1).xyz * 2.0 - 1.0;\n"
			"tmp1.xy = trc3.yw;\n"
			"tmp1.xyz = tex2Dlod(%IMG0, tmp1).xyz * 2.0 - 1.0;\n"
		
		#endif
		
		"# = " LERP "(temp.xyz, tmp1.xyz, " FRAGMENT_COLOR ".z);\n"
	};
	
	static const char primaryCode[] =
	{
		#if C4OPENGL
		
			"# = texture2DLod(%IMG0, trc3.xz, tlod.z).xyz * 2.0 - 1.0;\n"
		
		#else
		
			"tmp1.w = tlod.z;\n"
			
			"tmp1.xy = trc3.xz;\n"
			"# = tex2Dlod(%IMG0, tmp1).xyz * 2.0 - 1.0;\n"
		
		#endif
	};
	
	static const char secondaryCode[] =
	{
		#if C4OPENGL
		
			"tmp1 = texture2DLod(%IMG0, trc3.yw, tlod.z).xyz * 2.0 - 1.0;\n"
		
		#else
		
			"tmp1.w = tlod.z;\n"
			
			"tmp1.xy = trc3.yw;\n"
			"# = tex2Dlod(%IMG0, tmp1).xyz * 2.0 - 1.0;\n"
		
		#endif
	};
	
	static const char fullCompressedCode[] =
	{
		#if C4OPENGL
		
			"temp.xy = texture2DLod(%IMG0, trc3.xz, tlod.z).wy * 2.0 - 1.0;\n"
			"temp.z = sqrt(max(1.0 - dot(temp.xy, temp.xy), 0.03125));\n"
			"tmp1.xy = texture2DLod(%IMG0, trc3.yw, tlod.z).wy * 2.0 - 1.0;\n"
			"tmp1.z = sqrt(max(1.0 - dot(tmp1.xy, tmp1.xy), 0.03125));\n"
		
		#else
		
			"tmp1.w = tlod.z;\n"
			
			"tmp1.xy = trc3.xz;\n"
			"temp.xy = tex2Dlod(%IMG0, tmp1).wy * 2.0 - 1.0;\n"
			"temp.z = sqrt(max(1.0 - dot(temp.xy, temp.xy), 0.03125));\n"
			"tmp1.xy = trc3.yw;\n"
			"tmp1.xy = tex2Dlod(%IMG0, tmp1).wy * 2.0 - 1.0;\n"
			"tmp1.z = sqrt(max(1.0 - dot(tmp1.xy, tmp1.xy), 0.03125));\n"
		
		#endif
		
		"# = " LERP "(temp.xyz, tmp1.xyz, " FRAGMENT_COLOR ".z);\n"
	};
	
	static const char primaryCompressedCode[] =
	{
		#if C4OPENGL
		
			"#.xy = texture2DLod(%IMG0, trc3.xz, tlod.z).wy * 2.0 - 1.0;\n"
			"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
		
		#else
		
			"tmp1.w = tlod.z;\n"
			
			"tmp1.xy = trc3.xz;\n"
			"#.xy = tex2Dlod(%IMG0, tmp1).wy * 2.0 - 1.0;\n"
			"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
		
		#endif
	};
	
	static const char secondaryCompressedCode[] =
	{
		#if C4OPENGL
		
			"#.xy = texture2DLod(%IMG0, trc3.yw, tlod.z).wy * 2.0 - 1.0;\n"
			"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
		
		#else
		
			"tmp1.w = tlod.z;\n"
			
			"tmp1.xy = trc3.yw;\n"
			"#.xy = tex2Dlod(%IMG0, tmp1).wy * 2.0 - 1.0;\n"
			"#.z = sqrt(max(1.0 - dot(##.xy, ##.xy), 0.03125));\n"
		
		#endif
	};
	
	const Texture *texture = GetTexture();
	if (texture->GetTextureTargetIndex() == Render::kTextureTargetArray2D)
	{
		if (texture->GetImageFormat() == kTextureBC13)
		{
			if (blendMode == kTerrainBlendFull) shaderCode[0] = fullArrayCompressedCode;
			else if (blendMode == kTerrainBlendPrimary) shaderCode[0] = primaryArrayCompressedCode;
			else shaderCode[0] = secondaryArrayCompressedCode;
		}
		else
		{
			if (blendMode == kTerrainBlendFull) shaderCode[0] = fullArrayCode;
			else if (blendMode == kTerrainBlendPrimary) shaderCode[0] = primaryArrayCode;
			else shaderCode[0] = secondaryArrayCode;
		}
	}
	else
	{
		if (texture->GetImageFormat() == kTextureBC13)
		{
			if (blendMode == kTerrainBlendFull) shaderCode[0] = fullCompressedCode;
			else if (blendMode == kTerrainBlendPrimary) shaderCode[0] = primaryCompressedCode;
			else shaderCode[0] = secondaryCompressedCode;
		}
		else
		{
			if (blendMode == kTerrainBlendFull) shaderCode[0] = fullCode;
			else if (blendMode == kTerrainBlendPrimary) shaderCode[0] = primaryCode;
			else shaderCode[0] = secondaryCode;
		}
	}
	
	return (1);
}


PaintTextureProcess::PaintTextureProcess() : Process(kProcessPaintTexture)
{
}

PaintTextureProcess::PaintTextureProcess(const PaintTextureProcess& paintTextureProcess) : Process(paintTextureProcess)
{
}

PaintTextureProcess::~PaintTextureProcess()
{
}

Process *PaintTextureProcess::Replicate(void) const
{
	return (new PaintTextureProcess(*this));
}

int32 PaintTextureProcess::GetPortCount(void) const
{
	return (1);
}

const char *PaintTextureProcess::GetPortName(int32 index) const
{
	return ("TEXC");
}

#if C4PLAYSTATION3

	unsigned_int32 PaintTextureProcess::GetPortCompileFlags(int32 index) const
	{
		return (kProcessDependentTexture);
	}

#endif

void PaintTextureProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->textureCount = 1;
	data->outputSize = 4;
	data->inputSize[0] = 2;
	
	const Texture *const *texture = compileData->renderable->GetPaintEnvironment()->paintTexture;
	data->textureObject[0] = (texture) ? *texture : TheGraphicsMgr->GetNullTexture();
}

int32 PaintTextureProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEX		#, %0, %IMG0, 2D;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 PaintTextureProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = " TEX2D "(%IMG0, %0);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


Merge2Process::Merge2Process() : Process(kProcessMerge2)
{
}

Merge2Process::Merge2Process(const Merge2Process& merge2Process) : Process(merge2Process)
{
}

Merge2Process::~Merge2Process()
{
}

Process *Merge2Process::Replicate(void) const
{
	return (new Merge2Process(*this));
}

int32 Merge2Process::GetPortCount(void) const
{
	return (2);
}

const char *Merge2Process::GetPortName(int32 index) const
{
	static const char *const portName[2] =
	{
		"x", "y"
	};
	
	return (portName[index]);
}

void Merge2Process::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->preregisterCount = 1;
	data->outputSize = 2;
	
	data->inputSize[0] = 1;
	data->inputSize[1] = 1;
}

int32 Merge2Process::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"MOV		#.x, %0.x;\n"
		"MOV		#.y, %1.y;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 Merge2Process::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"#.x = %0.x;\n"
		"#.y = %1.y;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


Merge3Process::Merge3Process() : Process(kProcessMerge3)
{
}

Merge3Process::Merge3Process(const Merge3Process& merge3Process) : Process(merge3Process)
{
}

Merge3Process::~Merge3Process()
{
}

Process *Merge3Process::Replicate(void) const
{
	return (new Merge3Process(*this));
}

int32 Merge3Process::GetPortCount(void) const
{
	return (3);
}

const char *Merge3Process::GetPortName(int32 index) const
{
	static const char *const portName[3] =
	{
		"x", "y", "z"
	};
	
	return (portName[index]);
}

void Merge3Process::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->preregisterCount = 1;
	data->outputSize = 3;
	
	data->inputSize[0] = 1;
	data->inputSize[1] = 1;
	data->inputSize[2] = 1;
}

int32 Merge3Process::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"MOV		#.x, %0.x;\n"
		"MOV		#.y, %1.y;\n"
		"MOV		#.z, %2.z;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 Merge3Process::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"#.x = %0.x;\n"
		"#.y = %1.y;\n"
		"#.z = %2.z;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


Merge4Process::Merge4Process() : Process(kProcessMerge4)
{
}

Merge4Process::Merge4Process(const Merge4Process& merge4Process) : Process(merge4Process)
{
}

Merge4Process::~Merge4Process()
{
}

Process *Merge4Process::Replicate(void) const
{
	return (new Merge4Process(*this));
}

int32 Merge4Process::GetPortCount(void) const
{
	return (4);
}

const char *Merge4Process::GetPortName(int32 index) const
{
	static const char *const portName[4] =
	{
		"x", "y", "z", "w"
	};
	
	return (portName[index]);
}

void Merge4Process::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->preregisterCount = 1;
	data->outputSize = 4;
	
	data->inputSize[0] = 1;
	data->inputSize[1] = 1;
	data->inputSize[2] = 1;
	data->inputSize[3] = 1;
}

int32 Merge4Process::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"MOV		#.x, %0.x;\n"
		"MOV		#.y, %1.y;\n"
		"MOV		#.z, %2.z;\n"
		"MOV		#.w, %3.w;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 Merge4Process::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"#.x = %0.x;\n"
		"#.y = %1.y;\n"
		"#.z = %2.z;\n"
		"#.w = %3.w;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


InterpolantProcess::InterpolantProcess(ProcessType type) : Process(type)
{
	SetBaseProcessType(kProcessInterpolant);
}

InterpolantProcess::InterpolantProcess(const InterpolantProcess& interpolantProcess) : Process(interpolantProcess)
{
}

InterpolantProcess::~InterpolantProcess()
{
}

int32 InterpolantProcess::GetInterpolantSize(Type type)
{
	switch (type)
	{
		case 'FDTP':
		case 'FDTV':
		case 'FOGK':
		case 'DDEP':
		case 'AMGD':
		case 'SHDZ':
		case 'IDEP':
		case 'IBLD':
		case 'IXBL':
			
			return (1);
		
		case 'TEX0':
		case 'TEX1':
		case 'PTXC':
		case 'FIR2':
		case 'SHAD':
		case 'IRAD':
		case 'ISRD':
		case 'GITX':
		case 'TLD2':
		case 'TVD2':
			
			return (2);
		
		case 'RTXC':
		case 'TERA':
		case 'POSI':
		case 'NRML':
		case 'TANG':
		case 'BTNG':
		case 'WPOS':
		case 'WNRM':
		case 'WTAN':
		case 'WBTN':
		case 'NRMC':
		case 'GEOM':
		case 'LDIR':
		case 'VDIR':
		case 'OLDR':
		case 'OVDR':
		case 'TLDR':
		case 'TVDR':
		case 'TWNM':
		case 'TWB1':
		case 'TWB2':
		case 'FIRE':
		case 'ATTN':
		case 'AMBT':
		case 'APOS':
		case 'LAND':
		case 'SECT':
			
			return (3);
		
		case 'TWTN':
		case 'IMPT':
		case 'FIR1':
		case 'WARP':
		case 'RGHT':
		case 'DOWN':
		case 'PROJ':
		case 'VELA':
		case 'VELB':
			
			return (4);
	}
	
	return (0);
}

int32 InterpolantProcess::GetInterpolantName(Type type, const ShaderCompileData *compileData, const ShaderAllocationData *allocData, char *name, SwizzleData *swizzleData)
{
	static const char *const texcoordName[2][kMaxShaderTexcoordCount] =
	{
		#if C4OPENGL
		
			{"gl_TexCoord[0]", "gl_TexCoord[1]", "gl_TexCoord[2]", "gl_TexCoord[3]",
			 "gl_TexCoord[4]", "gl_TexCoord[5]", "gl_TexCoord[6]", "gl_TexCoord[7]"},
		
		#else
		
			{"fragment.texcoord", "fragment.texcoord1", "fragment.texcoord2", "fragment.texcoord3",
			 "fragment.texcoord4", "fragment.texcoord5", "fragment.texcoord6", "fragment.texcoord7"},
		
		#endif
		
		{"fragment.texcoord", "fragment.texcoord[1]", "fragment.texcoord[2]", "fragment.texcoord[3]",
		 "fragment.texcoord[4]", "fragment.texcoord[5]", "fragment.texcoord[6]", "fragment.texcoord[7]"}
	};
	
	int32 count = allocData->interpolantCount;
	const InterpolantData *interpolantData = allocData->interpolantData;
	
	for (machine a = 0; a < count; a++)
	{
		if (interpolantData->interpolantType == type)
		{
			int32 len = Text::CopyText(texcoordName[compileData->programFlag][interpolantData->texcoordIndex], name);
			name += len;
			
			if (swizzleData)
			{
				int32 size = swizzleData->size;
				for (machine a = 0; a < size; a++) swizzleData->component[a] = interpolantData->swizzleData.component[swizzleData->component[a]];
				return (Process::PostgenerateOutputIdentifier(compileData, swizzleData, name) + len);
			}
			
			return (Process::PostgenerateOutputIdentifier(compileData, &interpolantData->swizzleData, name) + len);
		}
		
		interpolantData++;
	}
	
	Assert(false, "Missing interpolant");
	return (0);
}

void InterpolantProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->outputSize = GetInterpolantSize(GetProcessType());
	
	data->interpolantCount = 1;
	data->interpolantType[0] = GetProcessType();
}

int32 InterpolantProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	return (GetInterpolantName(GetProcessType(), compileData, allocData, name + len, swizzleData) + len);
}


Texcoord0Process::Texcoord0Process() : InterpolantProcess(kProcessTexcoord0)
{
}

Texcoord0Process::Texcoord0Process(const Texcoord0Process& texcoord0Process) : InterpolantProcess(texcoord0Process)
{
}

Texcoord0Process::~Texcoord0Process()
{
}

Process *Texcoord0Process::Replicate(void) const
{
	return (new Texcoord0Process(*this));
}


Texcoord1Process::Texcoord1Process() : InterpolantProcess(kProcessTexcoord1)
{
}

Texcoord1Process::Texcoord1Process(const Texcoord1Process& texcoord1Process) : InterpolantProcess(texcoord1Process)
{
}

Texcoord1Process::~Texcoord1Process()
{
}

Process *Texcoord1Process::Replicate(void) const
{
	return (new Texcoord1Process(*this));
}


RawTexcoordProcess::RawTexcoordProcess() : InterpolantProcess(kProcessRawTexcoord)
{
}

RawTexcoordProcess::RawTexcoordProcess(const RawTexcoordProcess& rawTexcoordProcess) : InterpolantProcess(rawTexcoordProcess)
{
}

RawTexcoordProcess::~RawTexcoordProcess()
{
}

Process *RawTexcoordProcess::Replicate(void) const
{
	return (new RawTexcoordProcess(*this));
}


ImpostorTexcoordProcess::ImpostorTexcoordProcess() : InterpolantProcess(kProcessImpostorTexcoord)
{
}

ImpostorTexcoordProcess::ImpostorTexcoordProcess(const ImpostorTexcoordProcess& impostorTexcoordProcess) : InterpolantProcess(impostorTexcoordProcess)
{
}

ImpostorTexcoordProcess::~ImpostorTexcoordProcess()
{
}

Process *ImpostorTexcoordProcess::Replicate(void) const
{
	return (new ImpostorTexcoordProcess(*this));
}


ImpostorBlendProcess::ImpostorBlendProcess() : InterpolantProcess(kProcessImpostorBlend)
{
	SetBaseProcessType(kProcessDerived);
	
	textureObject = Texture::Get("C4/screen");
}

ImpostorBlendProcess::ImpostorBlendProcess(const ImpostorBlendProcess& impostorBlendProcess) : InterpolantProcess(impostorBlendProcess)
{
	textureObject = impostorBlendProcess.textureObject;
	textureObject->Retain();
}

ImpostorBlendProcess::~ImpostorBlendProcess()
{
	textureObject->Release();
}

Process *ImpostorBlendProcess::Replicate(void) const
{
	return (new ImpostorBlendProcess(*this));
}

void ImpostorBlendProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->outputSize = 2;
	
	data->interpolantCount = 3;
	data->interpolantType[0] = 'IMPT';
	data->interpolantType[1] = 'IBLD';
	data->interpolantType[2] = 'IXBL';
	
	data->textureCount = 1;
	data->textureObject[0] = textureObject;
}

int32 ImpostorBlendProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		ibld;\n"
		
		"TEX		ibld.y, $IMPT, %IMG0, 2D;\n"
		"SLT		ibld.x, ibld.y, $IBLD;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 ImpostorBlendProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		FLOAT2 " ibld;\n"
		
		"ibld.y = " TEX2D "(%IMG0, $IMPT.xy).x;\n"
		"ibld.x = float(ibld.y < $IBLD);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


TerrainTexcoordProcess::TerrainTexcoordProcess() : InterpolantProcess(kProcessTerrainTexcoord)
{
	SetBaseProcessType(kProcessDerived);
}

TerrainTexcoordProcess::TerrainTexcoordProcess(const TerrainTexcoordProcess& terrainTexcoordProcess) : InterpolantProcess(terrainTexcoordProcess)
{
}

TerrainTexcoordProcess::~TerrainTexcoordProcess()
{
}

Process *TerrainTexcoordProcess::Replicate(void) const
{
	return (new TerrainTexcoordProcess(*this));
}

bool TerrainTexcoordProcess::GetTexturePaletteSize(int32 *size) const
{
	const TextureMapProcess *process = static_cast<const TextureMapProcess *>(GetFirstOutgoingEdge()->GetFinishElement());
	const Texture *texture = process->GetTexture();
	
	if (texture->GetTextureFlags() & kTextureImagePalette)
	{
		const unsigned_int32 *paletteSize = texture->GetPaletteSize();
		size[0] = paletteSize[0];
		size[1] = paletteSize[1];
	}
	else
	{
		size[0] = 1;
		size[1] = 1;
	}
	
	return (texture->GetTextureTargetIndex() == Render::kTextureTargetArray2D);
}

void TerrainTexcoordProcess::GenerateSourceData(const ShaderCompileData *compileData) const
{
	#if C4OPENGL
	
		if (!TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) compileData->programFlag = false;
	
	#endif
	
	compileData->shaderSourceFlags |= kShaderSourcePrimaryColor | kShaderSourceSecondaryColor;
}

int32 TerrainTexcoordProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32	paletteSize[2];
	
	int32 count = InterpolantProcess::GenerateProcessSignature(compileData, signature);
	
	bool arrayTexture = GetTexturePaletteSize(paletteSize);
	signature[count] = (arrayTexture << 31) | (paletteSize[1] << 16) | paletteSize[0];
	return (count + 1);
}

void TerrainTexcoordProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->temporaryCount = 4;
	
	data->interpolantCount = 2;
	data->interpolantType[0] = 'TERA';
	data->interpolantType[1] = 'NRML';
}

int32 TerrainTexcoordProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char selectCode[] =
	{
		"TEMP		vert, trc1, trc2, trc3;\n"
		
		"SLT		temp.xz, $NRML, 0.0;\n"
		"SGE		temp.y, $NRML, 0.0;\n"
		
		"LRP		vert.xy, temp.z, fragment.color.secondary.ywww, fragment.color.secondary.xzzz;\n"
	};
	
	static const char arrayCode[] =
	{
		"LRP		trc1.xy, temp.xyyy, -$TERA.yxxx, $TERA.yxxx;\n"
		"MOV		trc1.z, $TERA.z;\n"
		"MUL		trc1.w, fragment.color.x, 255.0;\n"			// trc1 = (sx, sy, tx|ty, xy index 1)
		
		"MOV		trc2.xyz, trc1;\n"
		"MUL		trc2.w, fragment.color.y, 255.0;\n"			// trc2 = (sx, sy, tx|ty, xy index 2)
		
		"LRP		trc3.x, temp.z, -$TERA.x, $TERA.x;\n"
		"MOV		trc3.y, $TERA.y;\n"
		"MUL		trc3.zw, vert.xxxy, 255.0;\n"				// trc3 = (sz, tz, z index 1, z index 2)
	};
	
	static const char fractionCode[] =
	{
		"FRC		tmp1.xy, -$TERA;\n"
		"FRC		tmp2.xyz, $TERA;\n"
		"LRP		tmp1.xyz, temp, tmp1.yxxx, tmp2.yxxx;\n"
		
		"MUL		trc1.xy, fragment.color, 255.0;\n"
		"MUL		trc1.zw, vert.xxxy, 255.0;\n"				// trc1 = (xy index 1, xy index 2, z index 1, z index 2)
	};
	
	static const char palette3x3Code[] =
	{
		"MAD		tmp1.xyz, tmp1, 0.25, 0.03125;\n"			// tmp1 = (sx, sy, sz, --)
		"MAD		tmp2.xyz, tmp2.zzyy, 0.25, 0.03125;\n"		// tmp2 = (tx, ty, tz, --)
		
		"MUL		tmp4, trc1, 0.33334;\n"
		"FLR		tmp4, tmp4;\n"								// tmp4 = (xy entry j1, xy entry j2, z entry j1, z entry j2)
		"MAD		tmp3, tmp4, -3.0, trc1;\n"					// tmp3 = (xy entry i1, xy entry i2, z entry i1, z entry i2)
		
		"MAD		trc1.xy, tmp3.x, 0.3125, tmp1;\n"			// trc1 = (sx1, sy1, --, --)
		"MAD		trc1.zw, tmp4.x, 0.3125, tmp2.xxxy;\n"		// trc1 = (sx1, sy1, tx1, ty1)
		
		"MAD		trc2.xy, tmp3.y, 0.3125, tmp1;\n"			// trc3 = (sx2, sy2, --, --)
		"MAD		trc2.zw, tmp4.y, 0.3125, tmp2.xxxy;\n"		// trc3 = (sx2, sy2, tx2, ty2)
		
		"MAD		trc3.xy, tmp3.zwww, 0.3125, tmp1.z;\n"		// trc2 = (sz1, sz2, --, --)
		"MAD		trc3.zw, tmp4.zzzw, 0.3125, tmp2.z;\n"		// trc2 = (sz1, sz2, tz1, tz2)
	};
	
	static const char palette6x3Code[] =
	{
		"MAD		tmp1.xyz, tmp1, 0.125, 0.015625;\n"			// tmp1 = (sx, sy, sz, --)
		"MAD		tmp2.xyz, tmp2.zzyy, 0.25, 0.03125;\n"		// tmp2 = (tx, ty, tz, --)
		
		"MUL		tmp4, trc1, 0.16667;\n"
		"FLR		tmp4, tmp4;\n"								// tmp4 = (xy entry j1, xy entry j2, z entry j1, z entry j2)
		"MAD		tmp3, tmp4, -6.0, trc1;\n"					// tmp3 = (xy entry i1, xy entry i2, z entry i1, z entry i2)
		
		"MAD		trc1.xy, tmp3.x, 0.15625, tmp1;\n"			// trc1 = (sx1, sy1, --, --)
		"MAD		trc1.zw, tmp4.x, 0.3125, tmp2.xxxy;\n"		// trc1 = (sx1, sy1, tx1, ty1)
		
		"MAD		trc2.xy, tmp3.y, 0.15625, tmp1;\n"			// trc3 = (sx2, sy2, --, --)
		"MAD		trc2.zw, tmp4.y, 0.3125, tmp2.xxxy;\n"		// trc3 = (sx2, sy2, tx2, ty2)
		
		"MAD		trc3.xy, tmp3.zwww, 0.15625, tmp1.z;\n"		// trc2 = (sz1, sz2, --, --)
		"MAD		trc3.zw, tmp4.zzzw, 0.3125, tmp2.z;\n"		// trc2 = (sz1, sz2, tz1, tz2)
	};
	
	static const char palette6x6Code[] =
	{
		"MAD		tmp1.xyz, tmp1, 0.125, 0.015625;\n"			// tmp1 = (sx, sy, sz, --)
		"MAD		tmp2.xyz, tmp2.zzyy, 0.125, 0.015625;\n"	// tmp2 = (tx, ty, tz, --)
		
		"MUL		tmp4, trc1, 0.16667;\n"
		"FLR		tmp4, tmp4;\n"								// tmp4 = (xy entry j1, xy entry j2, z entry j1, z entry j2)
		"MAD		tmp3, tmp4, -6.0, trc1;\n"					// tmp3 = (xy entry i1, xy entry i2, z entry i1, z entry i2)
		
		"MAD		trc1.xy, tmp3.x, 0.15625, tmp1;\n"			// trc1 = (sx1, sy1, --, --)
		"MAD		trc1.zw, tmp4.x, 0.15625, tmp2.xxxy;\n"		// trc1 = (sx1, sy1, tx1, ty1)
		
		"MAD		trc2.xy, tmp3.y, 0.15625, tmp1;\n"			// trc3 = (sx2, sy2, --, --)
		"MAD		trc2.zw, tmp4.y, 0.15625, tmp2.xxxy;\n"		// trc3 = (sx2, sy2, tx2, ty2)
		
		"MAD		trc3.xy, tmp3.zwww, 0.15625, tmp1.z;\n"		// trc2 = (sz1, sz2, --, --)
		"MAD		trc3.zw, tmp4.zzzw, 0.15625, tmp2.z;\n"		// trc2 = (sz1, sz2, tz1, tz2)
	};
	
	static const char derivativeCode[] =
	{
		"TEMP		tlod;\n"
		
		"DDX		tmp1.xyz, $TERA;\n"
		"DDY		tmp2.xyz, $TERA;\n"
		"MAX		temp.xyz, |tmp1|, |tmp2|;\n"
		"MAX		temp.xyz, temp.yxxx, temp.zzyy;\n"
		"LG2		tlod.x, temp.x;\n"
		"LG2		tlod.y, temp.y;\n"
		"LG2		tlod.z, temp.z;\n"
	};
	
	int32	paletteSize[2];
	
	programCode[0] = selectCode;
	
	if (GetTexturePaletteSize(paletteSize))
	{
		programCode[1] = arrayCode;
		return (2);
	}
	
	programCode[1] = fractionCode;
	
	if (paletteSize[0] <= 3) programCode[2] = palette3x3Code;
	else if (paletteSize[1] == 3) programCode[2] = palette6x3Code;
	else programCode[2] = palette6x6Code;
	
	programCode[3] = derivativeCode;
	return (4);
}

int32 TerrainTexcoordProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char selectCode[] =
	{
		#if C4OPENGL
		
			"vec4 trc1, trc2, trc3;\n"
			
			"temp.xz = vec2($NRML.x < 0.0, $NRML.z < 0.0);\n"
			"temp.y = float($NRML.y >= 0.0);\n"
			"vec2 vert = mix(" FRAGMENT_COLOR1 ".xz, " FRAGMENT_COLOR1 ".yw, temp.z);\n"
		
		#else
		
			"float4 trc1, trc2, trc3;\n"
			
			"temp.xz = ($NRML.xz < 0.0);\n"
			"temp.y = ($NRML.y >= 0.0);\n"
			"float2 vert = lerp(" FRAGMENT_COLOR1 ".xz, " FRAGMENT_COLOR1 ".yw, temp.z);\n"
		
		#endif
	};
	
	static const char arrayCode[] =
	{
		"trc1.xy = " LERP "($TERA.yx, -$TERA.yx, temp.xy);\n"
		"trc1.z = $TERA.z;\n"
		"trc1.w = " FRAGMENT_COLOR ".x * 255.0;\n"
		
		"trc2.xyz = trc1.xyz;\n"
		"trc2.w = " FRAGMENT_COLOR ".y * 255.0;\n"
		
		"trc3.x = " LERP "($TERA.x, -$TERA.x, temp.z);\n"
		"trc3.y = $TERA.y;\n"
		"trc3.zw = vert * 255.0;\n"
	};
	
	static const char fractionCode[] =
	{
		"tmp1.xy = " FRAC "(-$TERA.xy);\n"
		"tmp2.xyz = " FRAC "($TERA);\n"
		
		"tmp1.xyz = " LERP "(tmp2.yxx, tmp1.yxx, temp.xyz);\n"
		
		"trc1.xy = " FRAGMENT_COLOR ".xy * 255.0;\n"
		"trc1.zw = vert * 255.0;\n"
	};
	
	static const char palette3x3Code[] =
	{
		"tmp1.xyz = tmp1.xyz * 0.25 + 0.03125;\n"
		"tmp2.xyz = tmp2.zzy * 0.25 + 0.03125;\n"
		
		"tmp4 = floor(trc1 * 0.3334);\n"
		"tmp3 = trc1 - tmp4 * 3.0;\n"
		
		"trc1.xy = tmp3.x * 0.3125 + tmp1.xy;\n"
		"trc1.zw = tmp4.x * 0.3125 + tmp2.xy;\n"
		
		"trc2.xy = tmp3.y * 0.3125 + tmp1.xy;\n"
		"trc2.zw = tmp4.y * 0.3125 + tmp2.xy;\n"
		
		"trc3.xy = tmp3.zw * 0.3125 + tmp1.z;\n"
		"trc3.zw = tmp4.zw * 0.3125 + tmp2.z;\n"
	};
	
	static const char palette6x3Code[] =
	{
		"tmp1.xyz = tmp1.xyz * 0.125 + 0.015625;\n"
		"tmp2.xyz = tmp2.zzy * 0.25 + 0.03125;\n"
		
		"tmp4 = floor(trc1 * 0.16667);\n"
		"tmp3 = trc1 - tmp4 * 6.0;\n"
		
		"trc1.xy = tmp3.x * 0.15625 + tmp1.xy;\n"
		"trc1.zw = tmp4.x * 0.3125 + tmp2.xy;\n"
		
		"trc2.xy = tmp3.y * 0.15625 + tmp1.xy;\n"
		"trc2.zw = tmp4.y * 0.3125 + tmp2.xy;\n"
		
		"trc3.xy = tmp3.zw * 0.15625 + tmp1.z;\n"
		"trc3.zw = tmp4.zw * 0.3125 + tmp2.z;\n"
	};
	
	static const char palette6x6Code[] =
	{
		"tmp1.xyz = tmp1.xyz * 0.125 + 0.015625;\n"
		"tmp2.xyz = tmp2.zzy * 0.125 + 0.015625;\n"
		
		"tmp4 = floor(trc1 * 0.16667);\n"
		"tmp3 = trc1 - tmp4 * 6.0;\n"
		
		"trc1.xy = tmp3.x * 0.15625 + tmp1.xy;\n"
		"trc1.zw = tmp4.x * 0.15625 + tmp2.xy;\n"
		
		"trc2.xy = tmp3.y * 0.15625 + tmp1.xy;\n"
		"trc2.zw = tmp4.y * 0.15625 + tmp2.xy;\n"
		
		"trc3.xy = tmp3.zw * 0.15625 + tmp1.z;\n"
		"trc3.zw = tmp4.zw * 0.15625 + tmp2.z;\n"
	};
	
	static const char derivativeCode[] =
	{
		"temp.xyz = max(abs(" DDX "($TERA)), abs(" DDY "($TERA)));\n"
		"temp.xyz = max(temp.yxx, temp.zzy);\n"
		FLOAT3 " tlod = " FLOAT3 "(log2(temp.x), log2(temp.y), log2(temp.z));\n"
	};
	
	int32	paletteSize[2];
	
	shaderCode[0] = selectCode;
	
	if (GetTexturePaletteSize(paletteSize))
	{
		shaderCode[1] = arrayCode;
		return (2);
	}
	
	shaderCode[1] = fractionCode;
	
	if (paletteSize[0] <= 3) shaderCode[2] = palette3x3Code;
	else if (paletteSize[1] == 3) shaderCode[2] = palette6x3Code;
	else shaderCode[2] = palette6x6Code;
	
	shaderCode[3] = derivativeCode;
	return (4);
}


TriplanarBlendProcess::TriplanarBlendProcess() : InterpolantProcess(kProcessTriplanarBlend)
{
	SetBaseProcessType(kProcessDerived);
}

TriplanarBlendProcess::TriplanarBlendProcess(const TriplanarBlendProcess& triplanarBlendProcess) : InterpolantProcess(triplanarBlendProcess)
{
}

TriplanarBlendProcess::~TriplanarBlendProcess()
{
}

Process *TriplanarBlendProcess::Replicate(void) const
{
	return (new TriplanarBlendProcess(*this));
}

void TriplanarBlendProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->interpolantCount = 1;
	data->interpolantType[0] = 'NRML';
}

int32 TriplanarBlendProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		tbld;\n"
		
		"DP3		tbld.w, $NRML, $NRML;\n"
		"RSQ		tbld.w, tbld.w;\n"
		"MUL		tbld.xyz, $NRML, tbld.w;\n"
		"ABS		tbld.xyz, tbld;\n"
		"SUB_SAT	tbld.xyz, tbld, 0.5;\n"
		"MUL		tbld.xyz, tbld, tbld;\n"
		"MUL		tbld.xyz, tbld, tbld;\n"
		"DP3		tbld.w, tbld, 1.0;\n"
		"RCP		tbld.w, tbld.w;\n"
		"MUL		tbld.xyz, tbld, tbld.w;\n"
	};
	
	static const char code2[] =
	{
		"TEMP		tbld;\n"
		
		"NRMH		tbld.xyz, $NRML;\n"
		"SUB_SAT	tbld.xyz, |tbld|, 0.5;\n"
		"MUL		tbld.xyz, tbld, tbld;\n"
		"MUL		tbld.xyz, tbld, tbld;\n"
		"DP3		tbld.w, tbld, 1.0;\n"
		"DIV		tbld.xyz, tbld, tbld.w;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 TriplanarBlendProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		#if C4OPENGL
		
			"vec3 tbld = clamp(abs(normalize($NRML)) - 0.5, 0.0, 1.0);\n"
		
		#else
		
			"float3 tbld = saturate(abs(normalize($NRML)) - 0.5);\n"
		
		#endif
		
		"tbld *= tbld;\n"
		"tbld *= tbld;\n"
		"tbld /= dot(tbld, " FLOAT3 "(1.0, 1.0, 1.0));\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


PaintTexcoordProcess::PaintTexcoordProcess() : InterpolantProcess(kProcessPaintTexcoord)
{
}

PaintTexcoordProcess::PaintTexcoordProcess(const PaintTexcoordProcess& paintTexcoordProcess) : InterpolantProcess(paintTexcoordProcess)
{
}

PaintTexcoordProcess::~PaintTexcoordProcess()
{
}

Process *PaintTexcoordProcess::Replicate(void) const
{
	return (new PaintTexcoordProcess(*this));
}


VertexColorProcess::VertexColorProcess() : Process(kProcessVertexColor)
{
	SetBaseProcessType(kProcessParameter);
}

VertexColorProcess::VertexColorProcess(const VertexColorProcess& vertexColorProcess) : Process(vertexColorProcess)
{
}

VertexColorProcess::~VertexColorProcess()
{
}

Process *VertexColorProcess::Replicate(void) const
{
	return (new VertexColorProcess(*this));
}

void VertexColorProcess::GenerateSourceData(const ShaderCompileData *compileData) const
{
	compileData->shaderSourceFlags |= kShaderSourcePrimaryColor;
}

void VertexColorProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->outputSize = 4;
}

int32 VertexColorProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	len += Text::CopyText((compileData->programFlag) ? "fragment.color" : FRAGMENT_COLOR, name + len);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + len) + len);
}


FragmentPositionProcess::FragmentPositionProcess() : Process(kProcessFragmentPosition)
{
	SetBaseProcessType(kProcessParameter);
}

FragmentPositionProcess::FragmentPositionProcess(const FragmentPositionProcess& fragmentPositionProcess) : Process(fragmentPositionProcess)
{
}

FragmentPositionProcess::~FragmentPositionProcess()
{
}

Process *FragmentPositionProcess::Replicate(void) const
{
	return (new FragmentPositionProcess(*this));
}

void FragmentPositionProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->outputSize = 4;
}

int32 FragmentPositionProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	len += Text::CopyText((compileData->programFlag) ? "fragment.position" : FRAGMENT_POSITION, name + len);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + len) + len);
}


VertexGeometryProcess::VertexGeometryProcess() : InterpolantProcess(kProcessVertexGeometry)
{
}

VertexGeometryProcess::VertexGeometryProcess(const VertexGeometryProcess& vertexGeometryProcess) : InterpolantProcess(vertexGeometryProcess)
{
}

VertexGeometryProcess::~VertexGeometryProcess()
{
}

Process *VertexGeometryProcess::Replicate(void) const
{
	return (new VertexGeometryProcess(*this));
}


ObjectPositionProcess::ObjectPositionProcess() : InterpolantProcess(kProcessObjectPosition)
{
}

ObjectPositionProcess::ObjectPositionProcess(const ObjectPositionProcess& objectPositionProcess) : InterpolantProcess(objectPositionProcess)
{
}

ObjectPositionProcess::~ObjectPositionProcess()
{
}

Process *ObjectPositionProcess::Replicate(void) const
{
	return (new ObjectPositionProcess(*this));
}


WorldPositionProcess::WorldPositionProcess() : InterpolantProcess(kProcessWorldPosition)
{
}

WorldPositionProcess::WorldPositionProcess(const WorldPositionProcess& worldPositionProcess) : InterpolantProcess(worldPositionProcess)
{
}

WorldPositionProcess::~WorldPositionProcess()
{
}

Process *WorldPositionProcess::Replicate(void) const
{
	return (new WorldPositionProcess(*this));
}


ObjectNormalProcess::ObjectNormalProcess() : InterpolantProcess(kProcessObjectNormal)
{
	SetBaseProcessType(kProcessDerived);
}

ObjectNormalProcess::ObjectNormalProcess(const ObjectNormalProcess& objectNormalProcess) : InterpolantProcess(objectNormalProcess)
{
}

ObjectNormalProcess::~ObjectNormalProcess()
{
}

Process *ObjectNormalProcess::Replicate(void) const
{
	return (new ObjectNormalProcess(*this));
}

int32 ObjectNormalProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	name += len;
	
	Text::CopyText("onrm", name);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + 4) + (len + 4));
}

int32 ObjectNormalProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		onrm;\n"
		
		"DP3		onrm.w, $NRML, $NRML;\n"
		"RSQ		onrm.w, onrm.w;\n"
		"MUL		onrm.xyz, $NRML, onrm.w;\n"
	};
	
	static const char code2[] =
	{
		"TEMP		onrm;\n"
		
		"NRMH		onrm.xyz, $NRML;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 ObjectNormalProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		HALF3 " onrm = normalize($NRML);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


ObjectTangentProcess::ObjectTangentProcess() : InterpolantProcess(kProcessObjectTangent)
{
	SetBaseProcessType(kProcessDerived);
}

ObjectTangentProcess::ObjectTangentProcess(const ObjectTangentProcess& objectTangentProcess) : InterpolantProcess(objectTangentProcess)
{
}

ObjectTangentProcess::~ObjectTangentProcess()
{
}

Process *ObjectTangentProcess::Replicate(void) const
{
	return (new ObjectTangentProcess(*this));
}

int32 ObjectTangentProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	name += len;
	
	Text::CopyText("otan", name);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + 4) + (len + 4));
}

int32 ObjectTangentProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		otan;\n"
		
		"DP3		otan.w, $TANG, $TANG;\n"
		"RSQ		otan.w, otan.w;\n"
		"MUL		otan.xyz, $TANG, otan.w;\n"
	};
	
	static const char code2[] =
	{
		"TEMP		otan;\n"
		
		"NRMH		otan.xyz, $TANG;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 ObjectTangentProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		HALF3 " otan = normalize($TANG);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


ObjectBitangentProcess::ObjectBitangentProcess() : InterpolantProcess(kProcessObjectBitangent)
{
	SetBaseProcessType(kProcessDerived);
}

ObjectBitangentProcess::ObjectBitangentProcess(const ObjectBitangentProcess& objectBitangentProcess) : InterpolantProcess(objectBitangentProcess)
{
}

ObjectBitangentProcess::~ObjectBitangentProcess()
{
}

Process *ObjectBitangentProcess::Replicate(void) const
{
	return (new ObjectBitangentProcess(*this));
}

int32 ObjectBitangentProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	name += len;
	
	Text::CopyText("obtn", name);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + 4) + (len + 4));
}

int32 ObjectBitangentProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		obtn;\n"
		
		"DP3		obtn.w, $BTNG, $BTNG;\n"
		"RSQ		obtn.w, obtn.w;\n"
		"MUL		obtn.xyz, $BTNG, obtn.w;\n"
	};
	
	static const char code2[] =
	{
		"TEMP		obtn;\n"
		
		"NRMH		obtn.xyz, $BTNG;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 ObjectBitangentProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		HALF3 " obtn = normalize($BTNG);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


WorldNormalProcess::WorldNormalProcess() : InterpolantProcess(kProcessWorldNormal)
{
	SetBaseProcessType(kProcessDerived);
}

WorldNormalProcess::WorldNormalProcess(const WorldNormalProcess& worldNormalProcess) : InterpolantProcess(worldNormalProcess)
{
}

WorldNormalProcess::~WorldNormalProcess()
{
}

Process *WorldNormalProcess::Replicate(void) const
{
	return (new WorldNormalProcess(*this));
}

int32 WorldNormalProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	name += len;
	
	Text::CopyText("wnrm", name);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + 4) + (len + 4));
}

int32 WorldNormalProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		wnrm;\n"
		
		"DP3		wnrm.w, $WNRM, $WNRM;\n"
		"RSQ		wnrm.w, wnrm.w;\n"
		"MUL		wnrm.xyz, $WNRM, wnrm.w;\n"
	};
	
	static const char code2[] =
	{
		"TEMP		wnrm;\n"
		
		"NRMH		wnrm.xyz, $WNRM;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 WorldNormalProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		HALF3 " wnrm = normalize($WNRM);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


WorldTangentProcess::WorldTangentProcess() : InterpolantProcess(kProcessWorldTangent)
{
	SetBaseProcessType(kProcessDerived);
}

WorldTangentProcess::WorldTangentProcess(const WorldTangentProcess& worldTangentProcess) : InterpolantProcess(worldTangentProcess)
{
}

WorldTangentProcess::~WorldTangentProcess()
{
}

Process *WorldTangentProcess::Replicate(void) const
{
	return (new WorldTangentProcess(*this));
}

int32 WorldTangentProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	name += len;
	
	Text::CopyText("wtan", name);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + 4) + (len + 4));
}

int32 WorldTangentProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		wtan;\n"
		
		"DP3		wtan.w, $WTAN, $WTAN;\n"
		"RSQ		wtan.w, wtan.w;\n"
		"MUL		wtan.xyz, $WTAN, wtan.w;\n"
	};
	
	static const char code2[] =
	{
		"TEMP		wtan;\n"
		
		"NRMH		wtan.xyz, $WTAN;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 WorldTangentProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		HALF3 " wtan = normalize($WTAN);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


WorldBitangentProcess::WorldBitangentProcess() : InterpolantProcess(kProcessWorldBitangent)
{
	SetBaseProcessType(kProcessDerived);
}

WorldBitangentProcess::WorldBitangentProcess(const WorldBitangentProcess& worldBitangentProcess) : InterpolantProcess(worldBitangentProcess)
{
}

WorldBitangentProcess::~WorldBitangentProcess()
{
}

Process *WorldBitangentProcess::Replicate(void) const
{
	return (new WorldBitangentProcess(*this));
}

int32 WorldBitangentProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	name += len;
	
	Text::CopyText("wbtn", name);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + 4) + (len + 4));
}

int32 WorldBitangentProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		wbtn;\n"
		
		"DP3		wbtn.w, $WBTN, $WBTN;\n"
		"RSQ		wbtn.w, wbtn.w;\n"
		"MUL		wbtn.xyz, $WBTN, wbtn.w;\n"
	};
	
	static const char code2[] =
	{
		"TEMP		wbtn;\n"
		
		"NRMH		wbtn.xyz, $WBTN;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 WorldBitangentProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		HALF3 " wbtn = normalize($WBTN);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


TangentLightDirectionProcess::TangentLightDirectionProcess() : InterpolantProcess(kProcessTangentLightDirection)
{
	SetBaseProcessType(kProcessDerived);
}

TangentLightDirectionProcess::TangentLightDirectionProcess(const TangentLightDirectionProcess& tangentLightDirectionProcess) : InterpolantProcess(tangentLightDirectionProcess)
{
}

TangentLightDirectionProcess::~TangentLightDirectionProcess()
{
}

Process *TangentLightDirectionProcess::Replicate(void) const
{
	return (new TangentLightDirectionProcess(*this));
}

bool TangentLightDirectionProcess::ValidShader(ProcessType type, int32 shader)
{
	return (shader == kShaderGraphLight);
}

int32 TangentLightDirectionProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	name += len;
	
	Text::CopyText("ldir", name);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + 4) + (len + 4));
}

int32 TangentLightDirectionProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		ldir;\n"
		
		"DP3		ldir.w, $LDIR, $LDIR;\n"
		"RSQ		ldir.w, ldir.w;\n"
		"MUL		ldir.xyz, $LDIR, ldir.w;\n"
	};
	
	static const char code2[] =
	{
		"TEMP		ldir;\n"
		
		"NRMH		ldir.xyz, $LDIR;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 TangentLightDirectionProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		HALF3 " ldir = normalize($LDIR);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


TangentViewDirectionProcess::TangentViewDirectionProcess() : InterpolantProcess(kProcessTangentViewDirection)
{
	SetBaseProcessType(kProcessDerived);
}

TangentViewDirectionProcess::TangentViewDirectionProcess(const TangentViewDirectionProcess& tangentViewDirectionProcess) : InterpolantProcess(tangentViewDirectionProcess)
{
}

TangentViewDirectionProcess::~TangentViewDirectionProcess()
{
}

Process *TangentViewDirectionProcess::Replicate(void) const
{
	return (new TangentViewDirectionProcess(*this));
}

int32 TangentViewDirectionProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	name += len;
	
	Text::CopyText("vdir", name);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + 4) + (len + 4));
}

int32 TangentViewDirectionProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		vdir;\n"
		
		"DP3		vdir.w, $VDIR, $VDIR;\n"
		"RSQ		vdir.w, vdir.w;\n"
		"MUL		vdir.xyz, $VDIR, vdir.w;\n"
	};
	
	static const char code2[] =
	{
		"TEMP		vdir;\n"
		
		"NRMH		vdir.xyz, $VDIR;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 TangentViewDirectionProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		HALF3 " vdir = normalize($VDIR);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


TangentHalfwayDirectionProcess::TangentHalfwayDirectionProcess() : InterpolantProcess(kProcessTangentHalfwayDirection)
{
	SetBaseProcessType(kProcessDerived);
}

TangentHalfwayDirectionProcess::TangentHalfwayDirectionProcess(const TangentHalfwayDirectionProcess& tangentHalfwayDirectionProcess) : InterpolantProcess(tangentHalfwayDirectionProcess)
{
}

TangentHalfwayDirectionProcess::~TangentHalfwayDirectionProcess()
{
}

Process *TangentHalfwayDirectionProcess::Replicate(void) const
{
	return (new TangentHalfwayDirectionProcess(*this));
}

bool TangentHalfwayDirectionProcess::ValidShader(ProcessType type, int32 shader)
{
	return (shader == kShaderGraphLight);
}

int32 TangentHalfwayDirectionProcess::GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const
{
	type[0] = kProcessTangentLightDirection;
	type[1] = kProcessTangentViewDirection;
	return (2);
}

void TangentHalfwayDirectionProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->outputSize = 3;
}

int32 TangentHalfwayDirectionProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	name += len;
	
	Text::CopyText("hdir", name);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + 4) + (len + 4));
}

int32 TangentHalfwayDirectionProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		hdir;\n"
		
		"ADD		hdir.xyz, ldir, vdir;\n"
		"DP3		hdir.w, hdir, hdir;\n"
		"RSQ		hdir.w, hdir.w;\n"
		"MUL		hdir.xyz, hdir, hdir.w;\n"
	};
	
	static const char code2[] =
	{
		"TEMP		hdir;\n"
		
		"ADDH		hdir.xyz, ldir, vdir;\n"
		"NRMH		hdir.xyz, hdir;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 TangentHalfwayDirectionProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		HALF3 " hdir = normalize(ldir + vdir);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


ObjectLightDirectionProcess::ObjectLightDirectionProcess() : InterpolantProcess(kProcessObjectLightDirection)
{
	SetBaseProcessType(kProcessDerived);
}

ObjectLightDirectionProcess::ObjectLightDirectionProcess(const ObjectLightDirectionProcess& objectLightDirectionProcess) : InterpolantProcess(objectLightDirectionProcess)
{
}

ObjectLightDirectionProcess::~ObjectLightDirectionProcess()
{
}

Process *ObjectLightDirectionProcess::Replicate(void) const
{
	return (new ObjectLightDirectionProcess(*this));
}

bool ObjectLightDirectionProcess::ValidShader(ProcessType type, int32 shader)
{
	return (shader == kShaderGraphLight);
}

int32 ObjectLightDirectionProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	name += len;
	
	Text::CopyText("oldr", name);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + 4) + (len + 4));
}

int32 ObjectLightDirectionProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		oldr;\n"
		
		"DP3		oldr.w, $OLDR, $OLDR;\n"
		"RSQ		oldr.w, oldr.w;\n"
		"MUL		oldr.xyz, $OLDR, oldr.w;\n"
	};
	
	static const char code2[] =
	{
		"TEMP		oldr;\n"
		
		"NRMH		oldr.xyz, $OLDR;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 ObjectLightDirectionProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		HALF3 " oldr = normalize($OLDR);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


ObjectViewDirectionProcess::ObjectViewDirectionProcess() : InterpolantProcess(kProcessObjectViewDirection)
{
	SetBaseProcessType(kProcessDerived);
}

ObjectViewDirectionProcess::ObjectViewDirectionProcess(const ObjectViewDirectionProcess& objectViewDirectionProcess) : InterpolantProcess(objectViewDirectionProcess)
{
}

ObjectViewDirectionProcess::~ObjectViewDirectionProcess()
{
}

Process *ObjectViewDirectionProcess::Replicate(void) const
{
	return (new ObjectViewDirectionProcess(*this));
}

int32 ObjectViewDirectionProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	name += len;
	
	Text::CopyText("ovdr", name);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + 4) + (len + 4));
}

int32 ObjectViewDirectionProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		ovdr;\n"
		
		"DP3		ovdr.w, $OVDR, $OVDR;\n"
		"RSQ		ovdr.w, ovdr.w;\n"
		"MUL		ovdr.xyz, $OVDR, ovdr.w;\n"
	};
	
	static const char code2[] =
	{
		"TEMP		ovdr;\n"
		
		"NRMH		ovdr.xyz, $OVDR;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 ObjectViewDirectionProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		HALF3 " ovdr = normalize($OVDR);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


ObjectHalfwayDirectionProcess::ObjectHalfwayDirectionProcess() : InterpolantProcess(kProcessObjectHalfwayDirection)
{
	SetBaseProcessType(kProcessDerived);
}

ObjectHalfwayDirectionProcess::ObjectHalfwayDirectionProcess(const ObjectHalfwayDirectionProcess& objectHalfwayDirectionProcess) : InterpolantProcess(objectHalfwayDirectionProcess)
{
}

ObjectHalfwayDirectionProcess::~ObjectHalfwayDirectionProcess()
{
}

Process *ObjectHalfwayDirectionProcess::Replicate(void) const
{
	return (new ObjectHalfwayDirectionProcess(*this));
}

bool ObjectHalfwayDirectionProcess::ValidShader(ProcessType type, int32 shader)
{
	return (shader == kShaderGraphLight);
}

int32 ObjectHalfwayDirectionProcess::GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const
{
	type[0] = kProcessObjectLightDirection;
	type[1] = kProcessObjectViewDirection;
	return (2);
}

void ObjectHalfwayDirectionProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->outputSize = 3;
}

int32 ObjectHalfwayDirectionProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 len = PregenerateOutputIdentifier(swizzleData, name);
	name += len;
	
	Text::CopyText("ohdr", name);
	return (PostgenerateOutputIdentifier(compileData, swizzleData, name + 4) + (len + 4));
}

int32 ObjectHalfwayDirectionProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		ohdr;\n"
		
		"ADD		ohdr.xyz, oldr, ovdr;\n"
		"DP3		ohdr.w, ohdr, ohdr;\n"
		"RSQ		ohdr.w, ohdr.w;\n"
		"MUL		ohdr.xyz, ohdr, ohdr.w;\n"
	};
	
	static const char code2[] =
	{
		"TEMP		ohdr;\n"
		
		"ADDH		ohdr.xyz, oldr, ovdr;\n"
		"NRMH		ohdr.xyz, ohdr;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 ObjectHalfwayDirectionProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		HALF3 " ohdr = normalize(oldr + ovdr);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


TerrainLightDirectionProcess::TerrainLightDirectionProcess() : InterpolantProcess(kProcessTerrainLightDirection)
{
	SetBaseProcessType(kProcessDerived);
}

TerrainLightDirectionProcess::TerrainLightDirectionProcess(const TerrainLightDirectionProcess& terrainLightDirectionProcess) : InterpolantProcess(terrainLightDirectionProcess)
{
}

TerrainLightDirectionProcess::~TerrainLightDirectionProcess()
{
}

Process *TerrainLightDirectionProcess::Replicate(void) const
{
	return (new TerrainLightDirectionProcess(*this));
}

void TerrainLightDirectionProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->interpolantCount = 2;
	data->interpolantType[0] = 'TLDR';
	data->interpolantType[1] = 'TLD2';
}

int32 TerrainLightDirectionProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		tldr, tld2;\n"
		
		"DP3		tldr.w, $TLDR, $TLDR;\n"
		"RSQ		tldr.w, tldr.w;\n"
		"MUL		tldr.xyz, $TLDR, tldr.w;\n"
		
		"MOV		tld2.xy, $TLD2;\n"
		"MOV		tld2.z, $TLDR.z;\n"
		"DP3		tld2.w, tld2, tld2;\n"
		"RSQ		tld2.w, tld2.w;\n"
		"MUL		tld2.xyz, tld2, tld2.w;\n"
	};
	
	static const char code2[] =
	{
		"TEMP		tldr, tld2;\n"
		
		"NRMH		tldr.xyz, $TLDR;\n"
		"MOV		tld2.xy, $TLD2;\n"
		"MOV		tld2.z, $TLDR.z;\n"
		"NRMH		tld2.xyz, tld2;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 TerrainLightDirectionProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		HALF3 " tldr = normalize($TLDR);\n"
		HALF3 " tld2 = normalize(" FLOAT3 "($TLD2.x, $TLD2.y, $TLDR.z));\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


TerrainViewDirectionProcess::TerrainViewDirectionProcess() : InterpolantProcess(kProcessTerrainViewDirection)
{
	SetBaseProcessType(kProcessDerived);
}

TerrainViewDirectionProcess::TerrainViewDirectionProcess(const TerrainViewDirectionProcess& terrainViewDirectionProcess) : InterpolantProcess(terrainViewDirectionProcess)
{
}

TerrainViewDirectionProcess::~TerrainViewDirectionProcess()
{
}

Process *TerrainViewDirectionProcess::Replicate(void) const
{
	return (new TerrainViewDirectionProcess(*this));
}

void TerrainViewDirectionProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->interpolantCount = 2;
	data->interpolantType[0] = 'TVDR';
	data->interpolantType[1] = 'TVD2';
}

int32 TerrainViewDirectionProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		tvdr, tvd2;\n"
		
		"DP3		tvdr.w, $TVDR, $TVDR;\n"
		"RSQ		tvdr.w, tvdr.w;\n"
		"MUL		tvdr.xyz, $TVDR, tvdr.w;\n"
		
		"MOV		tvd2.xy, $TVD2;\n"
		"MOV		tvd2.z, $TVDR.z;\n"
		"DP3		tvd2.w, tvd2, tvd2;\n"
		"RSQ		tvd2.w, tvd2.w;\n"
		"MUL		tvd2.xyz, tvd2, tvd2.w;\n"
	};
	
	static const char code2[] =
	{
		"TEMP		tvdr, tvd2;\n"
		
		"NRMH		tvdr.xyz, $TVDR;\n"
		"MOVH		tvd2.xy, $TVD2;\n"
		"MOVH		tvd2.z, $TVDR.z;\n"
		"NRMH		tvd2.xyz, tvd2;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 TerrainViewDirectionProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		HALF3 " tvdr = normalize($TVDR);\n"
		HALF3 " tvd2 = normalize(" FLOAT3 "($TVD2.x, $TVD2.y, $TVDR.z));\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


TerrainHalfwayDirectionProcess::TerrainHalfwayDirectionProcess() : InterpolantProcess(kProcessTerrainHalfwayDirection)
{
	SetBaseProcessType(kProcessDerived);
}

TerrainHalfwayDirectionProcess::TerrainHalfwayDirectionProcess(const TerrainHalfwayDirectionProcess& terrainHalfwayDirectionProcess) : InterpolantProcess(terrainHalfwayDirectionProcess)
{
}

TerrainHalfwayDirectionProcess::~TerrainHalfwayDirectionProcess()
{
}

Process *TerrainHalfwayDirectionProcess::Replicate(void) const
{
	return (new TerrainHalfwayDirectionProcess(*this));
}

int32 TerrainHalfwayDirectionProcess::GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const
{
	type[0] = kProcessTerrainLightDirection;
	type[1] = kProcessTerrainViewDirection;
	return (2);
}

void TerrainHalfwayDirectionProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->outputSize = 3;
}

int32 TerrainHalfwayDirectionProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		thdr, thd2;\n"
		
		"ADD		thdr.xyz, tldr, tvdr;\n"
		"DP3		thdr.w, thdr, thdr;\n"
		"RSQ		thdr.w, thdr.w;\n"
		"MUL		thdr.xyz, thdr, thdr.w;\n"
		
		"ADD		thd2.xyz, tld2, tvd2;\n"
		"DP3		thd2.w, thd2, thd2;\n"
		"RSQ		thd2.w, thd2.w;\n"
		"MUL		thd2.xyz, thd2, thd2.w;\n"
	};
	
	static const char code2[] =
	{
		"TEMP		thdr, thd2;\n"
		
		"ADDH		thdr.xyz, tldr, tvdr;\n"
		"NRMH		thdr.xyz, thdr;\n"
		
		"ADDH		thd2.xyz, tld2, tvd2;\n"
		"NRMH		thd2.xyz, thd2;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 TerrainHalfwayDirectionProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		HALF3 " thdr = normalize(tldr + tvdr);\n"
		HALF3 " thd2 = normalize(tld2 + tvd2);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


UnaryProcess::UnaryProcess(ProcessType type) : Process(type)
{
}

UnaryProcess::UnaryProcess(const UnaryProcess& unaryProcess) : Process(unaryProcess)
{
}

UnaryProcess::~UnaryProcess()
{
}

int32 UnaryProcess::GetPortCount(void) const
{
	return (1);
}

const char *UnaryProcess::GetPortName(int32 index) const
{
	return ("A");
}

void UnaryProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	
	int32 size = GetPortRoute(0)->GenerateOutputSize();
	data->outputSize = size;
	data->inputSize[0] = size;
}


BinaryProcess::BinaryProcess(ProcessType type) : Process(type)
{
}

BinaryProcess::BinaryProcess(const BinaryProcess& binaryProcess) : Process(binaryProcess)
{
}

BinaryProcess::~BinaryProcess()
{
}

int32 BinaryProcess::GetPortCount(void) const
{
	return (2);
}

const char *BinaryProcess::GetPortName(int32 index) const
{
	return ((index == 0) ? "A" : "B");
}

void BinaryProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	int32	size;
	
	const Route *routeA = GetPortRoute(0);
	const Route *routeB = GetPortRoute(1);
	
	if (!routeA)
	{
		data->passthruPort = 1;
		size = routeB->GenerateOutputSize();
	}
	else if ((!routeB) && (GetPortFlags(1) & kProcessPortOmissible))
	{
		data->passthruPort = 0;
		size = routeA->GenerateOutputSize();
	}
	else
	{
		data->registerCount = 1;
		
		size = routeA->GenerateOutputSize();
		if (routeB) size = Max(size, routeB->GenerateOutputSize());
	}
	
	data->outputSize = size;
	data->inputSize[0] = size;
	data->inputSize[1] = size;
}

int32 BinaryProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 port = GetProcessData()->passthruPort;
	if (port >= 0) return (GetPortRoute(port)->GenerateOutputIdentifier(compileData, allocData, swizzleData, name));
	
	return (Process::GenerateOutputIdentifier(compileData, allocData, swizzleData, name));
}


TrinaryProcess::TrinaryProcess(ProcessType type) : Process(type)
{
}

TrinaryProcess::TrinaryProcess(const TrinaryProcess& trinaryProcess) : Process(trinaryProcess)
{
}

TrinaryProcess::~TrinaryProcess()
{
}

int32 TrinaryProcess::GetPortCount(void) const
{
	return (3);
}

void TrinaryProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	
	int32 size1 = GetPortRoute(0)->GenerateOutputSize();
	int32 size2 = GetPortRoute(1)->GenerateOutputSize();
	int32 size3 = GetPortRoute(2)->GenerateOutputSize();
	int32 size = Max(Max(size1, size2), size3);
	
	data->outputSize = size;
	data->inputSize[0] = size;
	data->inputSize[1] = size;
	data->inputSize[2] = size;
}

int32 TrinaryProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	int32 port = GetProcessData()->passthruPort;
	if (port >= 0) return (GetPortRoute(port)->GenerateOutputIdentifier(compileData, allocData, swizzleData, name));
	
	return (Process::GenerateOutputIdentifier(compileData, allocData, swizzleData, name));
}


AbsoluteProcess::AbsoluteProcess() : UnaryProcess(kProcessAbsolute)
{
}

AbsoluteProcess::AbsoluteProcess(const AbsoluteProcess& absoluteProcess) : UnaryProcess(absoluteProcess)
{
}

AbsoluteProcess::~AbsoluteProcess()
{
}

Process *AbsoluteProcess::Replicate(void) const
{
	return (new AbsoluteProcess(*this));
}

void AbsoluteProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	UnaryProcess::GenerateProcessData(compileData, data);
	
	if ((compileData->programFlag) && (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]))
	{
		data->registerCount = 0;
		data->passthruPort = 0;
	}
}

int32 AbsoluteProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	if ((compileData->programFlag) && (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]))
	{
		swizzleData->absolute = true;
		return (GetPortRoute(0)->GenerateOutputIdentifier(compileData, allocData, swizzleData, name));
	}
	
	return (UnaryProcess::GenerateOutputIdentifier(compileData, allocData, swizzleData, name));
}

int32 AbsoluteProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	if (!(TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]))
	{
		static const char code[] =
		{
			"ABS		#, %0;\n"
		};
		
		programCode[0] = code;
		return (1);
	}
	
	return (0);
}

int32 AbsoluteProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = abs(%0);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


InvertProcess::InvertProcess() : UnaryProcess(kProcessInvert)
{
}

InvertProcess::InvertProcess(const InvertProcess& invertProcess) : UnaryProcess(invertProcess)
{
}

InvertProcess::~InvertProcess()
{
}

Process *InvertProcess::Replicate(void) const
{
	return (new InvertProcess(*this));
}

int32 InvertProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"SUB		#, 1.0, %0;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 InvertProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = 1.0 - %0;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


ExpandProcess::ExpandProcess() : UnaryProcess(kProcessExpand)
{
}

ExpandProcess::ExpandProcess(const ExpandProcess& expandProcess) : UnaryProcess(expandProcess)
{
}

ExpandProcess::~ExpandProcess()
{
}

Process *ExpandProcess::Replicate(void) const
{
	return (new ExpandProcess(*this));
}

int32 ExpandProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"MAD		#, %0, 2.0, -1.0;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 ExpandProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = %0 * 2.0 - 1.0;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


ReciprocalProcess::ReciprocalProcess() : UnaryProcess(kProcessReciprocal)
{
}

ReciprocalProcess::ReciprocalProcess(const ReciprocalProcess& reciprocalProcess) : UnaryProcess(reciprocalProcess)
{
}

ReciprocalProcess::~ReciprocalProcess()
{
}

Process *ReciprocalProcess::Replicate(void) const
{
	return (new ReciprocalProcess(*this));
}

void ReciprocalProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 1;
	data->inputSize[0] = 1;
}

int32 ReciprocalProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"RCP		#, %0;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 ReciprocalProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = 1.0 / %0;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


ReciprocalSquareRootProcess::ReciprocalSquareRootProcess() : UnaryProcess(kProcessReciprocalSquareRoot)
{
}

ReciprocalSquareRootProcess::ReciprocalSquareRootProcess(const ReciprocalSquareRootProcess& reciprocalSquareRootProcess) : UnaryProcess(reciprocalSquareRootProcess)
{
}

ReciprocalSquareRootProcess::~ReciprocalSquareRootProcess()
{
}

Process *ReciprocalSquareRootProcess::Replicate(void) const
{
	return (new ReciprocalSquareRootProcess(*this));
}

void ReciprocalSquareRootProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 1;
	data->inputSize[0] = 1;
}

int32 ReciprocalSquareRootProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"RSQ		#, %0;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 ReciprocalSquareRootProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		#if C4OPENGL
		
			"# = inversesqrt(%0);\n"
		
		#else
		
			"# = rsqrt(%0);\n"
		
		#endif
	};
	
	shaderCode[0] = code;
	return (1);
}


SquareRootProcess::SquareRootProcess() : UnaryProcess(kProcessSquareRoot)
{
}

SquareRootProcess::SquareRootProcess(const SquareRootProcess& squareRootProcess) : UnaryProcess(squareRootProcess)
{
}

SquareRootProcess::~SquareRootProcess()
{
}

Process *SquareRootProcess::Replicate(void) const
{
	return (new SquareRootProcess(*this));
}

int32 SquareRootProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"RSQ		temp.x, %0;\n"
		"MUL		#, %0, temp.x;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 SquareRootProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = sqrt(%0);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}

void SquareRootProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 1;
	data->inputSize[0] = 1;
}


MagnitudeProcess::MagnitudeProcess() : UnaryProcess(kProcessMagnitude)
{
}

MagnitudeProcess::MagnitudeProcess(const MagnitudeProcess& magnitudeProcess) : UnaryProcess(magnitudeProcess)
{
}

MagnitudeProcess::~MagnitudeProcess()
{
}

Process *MagnitudeProcess::Replicate(void) const
{
	return (new MagnitudeProcess(*this));
}

void MagnitudeProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 1;
	data->inputSize[0] = 3;
}

int32 MagnitudeProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"DP3		temp.x, %0, %0;\n"
		"RSQ		temp.y, temp.x;\n"
		"MUL		#, temp.x, temp.y;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 MagnitudeProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = length(%0);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


NormalizeProcess::NormalizeProcess() : UnaryProcess(kProcessNormalize)
{
}

NormalizeProcess::NormalizeProcess(const NormalizeProcess& normalizeProcess) : UnaryProcess(normalizeProcess)
{
}

NormalizeProcess::~NormalizeProcess()
{
}

Process *NormalizeProcess::Replicate(void) const
{
	return (new NormalizeProcess(*this));
}

void NormalizeProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 3;
	data->inputSize[0] = 3;
}

int32 NormalizeProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"DP3		temp.x, %0, %0;\n"
		"RSQ		temp.x, temp.x;\n"
		"MUL		#, %0, temp.x;\n"
	};
	
	static const char code2[] =
	{
		"NRMH		#, %0;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 NormalizeProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = normalize(%0);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


FloorProcess::FloorProcess() : UnaryProcess(kProcessFloor)
{
}

FloorProcess::FloorProcess(const FloorProcess& floorProcess) : UnaryProcess(floorProcess)
{
}

FloorProcess::~FloorProcess()
{
}

Process *FloorProcess::Replicate(void) const
{
	return (new FloorProcess(*this));
}

int32 FloorProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"FLR		#, %0;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 FloorProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = floor(%0);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


RoundProcess::RoundProcess() : UnaryProcess(kProcessRound)
{
}

RoundProcess::RoundProcess(const RoundProcess& roundProcess) : UnaryProcess(roundProcess)
{
}

RoundProcess::~RoundProcess()
{
}

Process *RoundProcess::Replicate(void) const
{
	return (new RoundProcess(*this));
}

int32 RoundProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"ADD		#, %0, 0.5;\n"
		"FLR		#, ##;\n"
	};
	
	static const char code4[] =
	{
		"ROUND		#, %0;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionGpuProgram4]) programCode[0] = code4;
	else programCode[0] = code;
	
	return (1);
}

int32 RoundProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		#if CGSHADER
		
			"# = round(%0);\n"
		
		#else
		
			"# = floor(%0 + 0.5);\n"
		
		#endif
	};
	
	shaderCode[0] = code;
	return (1);
}


FractionProcess::FractionProcess() : UnaryProcess(kProcessFraction)
{
}

FractionProcess::FractionProcess(const FractionProcess& fractionProcess) : UnaryProcess(fractionProcess)
{
}

FractionProcess::~FractionProcess()
{
}

Process *FractionProcess::Replicate(void) const
{
	return (new FractionProcess(*this));
}

int32 FractionProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"FRC		#, %0;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 FractionProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = " FRAC "(%0);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


SaturateProcess::SaturateProcess() : UnaryProcess(kProcessSaturate)
{
}

SaturateProcess::SaturateProcess(const SaturateProcess& saturateProcess) : UnaryProcess(saturateProcess)
{
}

SaturateProcess::~SaturateProcess()
{
}

Process *SaturateProcess::Replicate(void) const
{
	return (new SaturateProcess(*this));
}

int32 SaturateProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"MOV_SAT	#, %0;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 SaturateProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		#if C4OPENGL
		
			"# = clamp(%0, 0.0, 1.0);\n"
		
		#else
		
			"# = saturate(%0);\n"
		
		#endif
	};
	
	shaderCode[0] = code;
	return (1);
}


SineProcess::SineProcess() : UnaryProcess(kProcessSine)
{
}

SineProcess::SineProcess(const SineProcess& sineProcess) : UnaryProcess(sineProcess)
{
}

SineProcess::~SineProcess()
{
}

Process *SineProcess::Replicate(void) const
{
	return (new SineProcess(*this));
}

void SineProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 1;
	data->inputSize[0] = 1;
}

int32 SineProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"SIN		#, %0;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 SineProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = sin(%0);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


CosineProcess::CosineProcess() : UnaryProcess(kProcessCosine)
{
}

CosineProcess::CosineProcess(const CosineProcess& cosineProcess) : UnaryProcess(cosineProcess)
{
}

CosineProcess::~CosineProcess()
{
}

Process *CosineProcess::Replicate(void) const
{
	return (new CosineProcess(*this));
}

void CosineProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 1;
	data->inputSize[0] = 1;
}

int32 CosineProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"COS		#, %0;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 CosineProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = cos(%0);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


Exp2Process::Exp2Process() : UnaryProcess(kProcessExp2)
{
}

Exp2Process::Exp2Process(const Exp2Process& exp2Process) : UnaryProcess(exp2Process)
{
}

Exp2Process::~Exp2Process()
{
}

Process *Exp2Process::Replicate(void) const
{
	return (new Exp2Process(*this));
}

void Exp2Process::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 1;
	data->inputSize[0] = 1;
}

int32 Exp2Process::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"EX2		#, %0;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 Exp2Process::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = exp2(%0);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


Log2Process::Log2Process() : UnaryProcess(kProcessLog2)
{
}

Log2Process::Log2Process(const Log2Process& log2Process) : UnaryProcess(log2Process)
{
}

Log2Process::~Log2Process()
{
}

Process *Log2Process::Replicate(void) const
{
	return (new Log2Process(*this));
}

void Log2Process::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 1;
	data->inputSize[0] = 1;
}

int32 Log2Process::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"LG2		#, %0;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 Log2Process::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = log2(%0);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


AddProcess::AddProcess() : BinaryProcess(kProcessAdd)
{
}

AddProcess::AddProcess(const AddProcess& addProcess) : BinaryProcess(addProcess)
{
}

AddProcess::~AddProcess()
{
}

Process *AddProcess::Replicate(void) const
{
	return (new AddProcess(*this));
}

unsigned_int32 AddProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOmissible);
}

int32 AddProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	if ((GetPortRoute(0)) && (GetPortRoute(1)))
	{
		static const char code[] =
		{
			"ADD		#, %0, %1;\n"
		};
		
		programCode[0] = code;
		return (1);
	}
	
	return (0);
}

int32 AddProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	if ((GetPortRoute(0)) && (GetPortRoute(1)))
	{
		static const char code[] =
		{
			"# = %0 + %1;\n"
		};
		
		shaderCode[0] = code;
		return (1);
	}
	
	return (0);
}


SubtractProcess::SubtractProcess() : BinaryProcess(kProcessSubtract)
{
}

SubtractProcess::SubtractProcess(const SubtractProcess& subtractProcess) : BinaryProcess(subtractProcess)
{
}

SubtractProcess::~SubtractProcess()
{
}

Process *SubtractProcess::Replicate(void) const
{
	return (new SubtractProcess(*this));
}

unsigned_int32 SubtractProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOmissible);
}

int32 SubtractProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	if ((GetPortRoute(0)) && (GetPortRoute(1)))
	{
		static const char code[] =
		{
			"SUB		#, %0, %1;\n"
		};
		
		programCode[0] = code;
		return (1);
	}
	
	return (0);
}

int32 SubtractProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	if ((GetPortRoute(0)) && (GetPortRoute(1)))
	{
		static const char code[] =
		{
			"# = %0 - %1;\n"
		};
		
		shaderCode[0] = code;
		return (1);
	}
	
	return (0);
}


AverageProcess::AverageProcess() : BinaryProcess(kProcessAverage)
{
}

AverageProcess::AverageProcess(const AverageProcess& averageProcess) : BinaryProcess(averageProcess)
{
}

AverageProcess::~AverageProcess()
{
}

Process *AverageProcess::Replicate(void) const
{
	return (new AverageProcess(*this));
}

unsigned_int32 AverageProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOmissible);
}

int32 AverageProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	if ((GetPortRoute(0)) && (GetPortRoute(1)))
	{
		static const char code[] =
		{
			"ADD		temp, %0, %1;\n"
			"MUL		#, temp, 0.5;\n"
		};
		
		programCode[0] = code;
		return (1);
	}
	
	return (0);
}

int32 AverageProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	if ((GetPortRoute(0)) && (GetPortRoute(1)))
	{
		static const char code[] =
		{
			"# = (%0 + %1) * 0.5;\n"
		};
		
		shaderCode[0] = code;
		return (1);
	}
	
	return (0);
}


MultiplyProcess::MultiplyProcess() : BinaryProcess(kProcessMultiply)
{
}

MultiplyProcess::MultiplyProcess(const MultiplyProcess& multiplyProcess) : BinaryProcess(multiplyProcess)
{
}

MultiplyProcess::~MultiplyProcess()
{
}

Process *MultiplyProcess::Replicate(void) const
{
	return (new MultiplyProcess(*this));
}

unsigned_int32 MultiplyProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOmissible);
}

int32 MultiplyProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	if ((GetPortRoute(0)) && (GetPortRoute(1)))
	{
		static const char code[] =
		{
			"MUL		#, %0, %1;\n"
		};
		
		programCode[0] = code;
		return (1);
	}
	
	return (0);
}

int32 MultiplyProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	if ((GetPortRoute(0)) && (GetPortRoute(1)))
	{
		static const char code[] =
		{
			"# = %0 * %1;\n"
		};
		
		shaderCode[0] = code;
		return (1);
	}
	
	return (0);
}


DivideProcess::DivideProcess() : BinaryProcess(kProcessDivide)
{
}

DivideProcess::DivideProcess(const DivideProcess& divideProcess) : BinaryProcess(divideProcess)
{
}

DivideProcess::~DivideProcess()
{
}

Process *DivideProcess::Replicate(void) const
{
	return (new DivideProcess(*this));
}

void DivideProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	
	int32 size = GetPortRoute(0)->GenerateOutputSize();
	
	data->outputSize = size;
	data->inputSize[0] = size;
	data->inputSize[1] = 1;
}

int32 DivideProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"RCP		temp.x, %1;\n"
		"MUL		#, %0, temp.x;\n"
	};
	
	static const char code2[] =
	{
		"DIV		#, %0, %1;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 DivideProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = %0 / %1;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


Dot3Process::Dot3Process() : BinaryProcess(kProcessDot3)
{
}

Dot3Process::Dot3Process(const Dot3Process& dot3Process) : BinaryProcess(dot3Process)
{
}

Dot3Process::~Dot3Process()
{
}

Process *Dot3Process::Replicate(void) const
{
	return (new Dot3Process(*this));
}

void Dot3Process::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 1;
	data->inputSize[0] = 3;
	data->inputSize[1] = 3;
}

int32 Dot3Process::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"DP3		#, %0, %1;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 Dot3Process::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = dot(%0, %1);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


Dot4Process::Dot4Process() : BinaryProcess(kProcessDot4)
{
}

Dot4Process::Dot4Process(const Dot4Process& dot4Process) : BinaryProcess(dot4Process)
{
}

Dot4Process::~Dot4Process()
{
}

Process *Dot4Process::Replicate(void) const
{
	return (new Dot4Process(*this));
}

void Dot4Process::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 1;
	data->inputSize[0] = 4;
	data->inputSize[1] = 4;
}

int32 Dot4Process::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"DP4		#, %0, %1;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 Dot4Process::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = dot(%0, %1);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


CrossProcess::CrossProcess() : BinaryProcess(kProcessCross)
{
}

CrossProcess::CrossProcess(const CrossProcess& crossProcess) : BinaryProcess(crossProcess)
{
}

CrossProcess::~CrossProcess()
{
}

Process *CrossProcess::Replicate(void) const
{
	return (new CrossProcess(*this));
}

void CrossProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 3;
	data->inputSize[0] = 3;
	data->inputSize[1] = 3;
}

int32 CrossProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"XPD		#, %0, %1;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 CrossProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = cross(%0, %1);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


MinimumProcess::MinimumProcess() : BinaryProcess(kProcessMinimum)
{
}

MinimumProcess::MinimumProcess(const MinimumProcess& minimumProcess) : BinaryProcess(minimumProcess)
{
}

MinimumProcess::~MinimumProcess()
{
}

Process *MinimumProcess::Replicate(void) const
{
	return (new MinimumProcess(*this));
}

unsigned_int32 MinimumProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOmissible);
}

int32 MinimumProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	if ((GetPortRoute(0)) && (GetPortRoute(1)))
	{
		static const char code[] =
		{
			"MIN		#, %0, %1;\n"
		};
		
		programCode[0] = code;
		return (1);
	}
	
	return (0);
}

int32 MinimumProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	if ((GetPortRoute(0)) && (GetPortRoute(1)))
	{
		static const char code[] =
		{
			"# = min(%0, %1);\n"
		};
		
		shaderCode[0] = code;
		return (1);
	}
	
	return (0);
}


MaximumProcess::MaximumProcess() : BinaryProcess(kProcessMaximum)
{
}

MaximumProcess::MaximumProcess(const MaximumProcess& maximumProcess) : BinaryProcess(maximumProcess)
{
}

MaximumProcess::~MaximumProcess()
{
}

Process *MaximumProcess::Replicate(void) const
{
	return (new MaximumProcess(*this));
}

unsigned_int32 MaximumProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOmissible);
}

int32 MaximumProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	if ((GetPortRoute(0)) && (GetPortRoute(1)))
	{
		static const char code[] =
		{
			"MAX		#, %0, %1;\n"
		};
		
		programCode[0] = code;
		return (1);
	}
	
	return (0);
}

int32 MaximumProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	if ((GetPortRoute(0)) && (GetPortRoute(1)))
	{
		static const char code[] =
		{
			"# = max(%0, %1);\n"
		};
		
		shaderCode[0] = code;
		return (1);
	}
	
	return (0);
}


SetLessThanProcess::SetLessThanProcess() : BinaryProcess(kProcessSetLessThan)
{
}

SetLessThanProcess::SetLessThanProcess(const SetLessThanProcess& setLessThanProcess) : BinaryProcess(setLessThanProcess)
{
}

SetLessThanProcess::~SetLessThanProcess()
{
}

Process *SetLessThanProcess::Replicate(void) const
{
	return (new SetLessThanProcess(*this));
}

unsigned_int32 SetLessThanProcess::GetPortFlags(int32 index) const
{
	return ((index == 0) ? 0 : kProcessPortOptional);
}

int32 SetLessThanProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char fullCode[] =
	{
		"SLT		#, %0, %1;\n"
	};
	
	static const char zeroCode[] =
	{
		"SLT		#, %0, 0.0;\n"
	};
	
	if (GetPortRoute(1)) programCode[0] = fullCode;
	else programCode[0] = zeroCode;
	
	return (1);
}

int32 SetLessThanProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	#if C4OPENGL
	
		static const char *fullCode[4] =
		{
			"# = float(%0 < %1);\n",
			"# = vec2(%0.x < %1.x, %0.y < %1.y);\n",
			"# = vec3(%0.x < %1.x, %0.y < %1.y, %0.z < %1.z);\n",
			"# = vec4(%0.x < %1.x, %0.y < %1.y, %0.z < %1.z, %0.w < %1.w);\n"
		};
		
		static const char *zeroCode[4] =
		{
			"# = float(%0 < 0.0);\n",
			"# = vec2(%0.x < 0.0, %0.y < 0.0);\n",
			"# = vec3(%0.x < 0.0, %0.y < 0.0, %0.z < 0.0);\n",
			"# = vec4(%0.x < 0.0, %0.y < 0.0, %0.z < 0.0, %0.w < 0.0);\n"
		};
		
		const Route *routeA = GetPortRoute(0);
		const Route *routeB = GetPortRoute(1);
		
		if (routeB)
		{
			int32 sizeA = routeA->GenerateOutputSize();
			int32 sizeB = routeB->GenerateOutputSize();
			shaderCode[0] = fullCode[Max(sizeA, sizeB) - 1];
		}
		else
		{
			shaderCode[0] = zeroCode[routeA->GenerateOutputSize() - 1];
		}
	
	#else
	
		static const char fullCode[] =
		{
			"# = (%0 < %1);\n"
		};
		
		static const char zeroCode[] =
		{
			"# = (%0 < 0.0);\n"
		};
		
		if (GetPortRoute(1)) shaderCode[0] = fullCode;
		else shaderCode[0] = zeroCode;
	
	#endif
	
	return (1);
}


SetGreaterEqualProcess::SetGreaterEqualProcess() : BinaryProcess(kProcessSetGreaterEqual)
{
}

SetGreaterEqualProcess::SetGreaterEqualProcess(const SetGreaterEqualProcess& setGreaterEqualProcess) : BinaryProcess(setGreaterEqualProcess)
{
}

SetGreaterEqualProcess::~SetGreaterEqualProcess()
{
}

Process *SetGreaterEqualProcess::Replicate(void) const
{
	return (new SetGreaterEqualProcess(*this));
}

unsigned_int32 SetGreaterEqualProcess::GetPortFlags(int32 index) const
{
	return ((index == 0) ? 0 : kProcessPortOptional);
}

int32 SetGreaterEqualProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char fullCode[] =
	{
		"SGE		#, %0, %1;\n"
	};
	
	static const char zeroCode[] =
	{
		"SGE		#, %0, 0.0;\n"
	};
	
	if (GetPortRoute(1)) programCode[0] = fullCode;
	else programCode[0] = zeroCode;
	
	return (1);
}

int32 SetGreaterEqualProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	#if C4OPENGL
	
		static const char *fullCode[4] =
		{
			"# = float(%0 >= %1);\n",
			"# = vec2(%0.x >= %1.x, %0.y >= %1.y);\n",
			"# = vec3(%0.x >= %1.x, %0.y >= %1.y, %0.z >= %1.z);\n",
			"# = vec4(%0.x >= %1.x, %0.y >= %1.y, %0.z >= %1.z, %0.w >= %1.w);\n"
		};
		
		static const char *zeroCode[4] =
		{
			"# = float(%0 >= 0.0);\n",
			"# = vec2(%0.x >= 0.0, %0.y >= 0.0);\n",
			"# = vec3(%0.x >= 0.0, %0.y >= 0.0, %0.z >= 0.0);\n",
			"# = vec4(%0.x >= 0.0, %0.y >= 0.0, %0.z >= 0.0, %0.w >= 0.0);\n"
		};
		
		const Route *routeA = GetPortRoute(0);
		const Route *routeB = GetPortRoute(1);
		
		if (routeB)
		{
			int32 sizeA = routeA->GenerateOutputSize();
			int32 sizeB = routeB->GenerateOutputSize();
			shaderCode[0] = fullCode[Max(sizeA, sizeB) - 1];
		}
		else
		{
			shaderCode[0] = zeroCode[routeA->GenerateOutputSize() - 1];
		}
	
	#else
	
		static const char fullCode[] =
		{
			"# = (%0 >= %1);\n"
		};
		
		static const char zeroCode[] =
		{
			"# = (%0 >= 0.0);\n"
		};
		
		if (GetPortRoute(1)) shaderCode[0] = fullCode;
		else shaderCode[0] = zeroCode;
	
	#endif
	
	return (1);
}


PowerProcess::PowerProcess() : BinaryProcess(kProcessPower)
{
}

PowerProcess::PowerProcess(const PowerProcess& powerProcess) : BinaryProcess(powerProcess)
{
}

PowerProcess::~PowerProcess()
{
}

Process *PowerProcess::Replicate(void) const
{
	return (new PowerProcess(*this));
}

void PowerProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 1;
	data->inputSize[0] = 1;
	data->inputSize[1] = 1;
}

int32 PowerProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"POW		#, %0, %1;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 PowerProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = pow(%0, %1);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


MultiplyAddProcess::MultiplyAddProcess() : TrinaryProcess(kProcessMultiplyAdd)
{
}

MultiplyAddProcess::MultiplyAddProcess(const MultiplyAddProcess& multiplyAddProcess) : TrinaryProcess(multiplyAddProcess)
{
}

MultiplyAddProcess::~MultiplyAddProcess()
{
}

Process *MultiplyAddProcess::Replicate(void) const
{
	return (new MultiplyAddProcess(*this));
}

unsigned_int32 MultiplyAddProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOmissible);
}

const char *MultiplyAddProcess::GetPortName(int32 index) const
{
	static const char *const portName[3] =
	{
		"A", "B", "C"
	};
	
	return (portName[index]);
}

void MultiplyAddProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	int32	size;
	
	const Route *routeA = GetPortRoute(0);
	const Route *routeB = GetPortRoute(1);
	const Route *routeC = GetPortRoute(2);
	
	if (routeA)
	{
		size = routeA->GenerateOutputSize();
		
		if (routeB)
		{
			data->registerCount = 1;
			
			size = Max(size, routeB->GenerateOutputSize());
			if (routeC) size = Max(size, routeC->GenerateOutputSize());
		}
		else
		{
			if (routeC)
			{
				data->registerCount = 1;
				size = Max(size, routeC->GenerateOutputSize());
			}
			else
			{
				data->passthruPort = 0;
			}
		}
	}
	else
	{
		if (routeB)
		{
			size = routeB->GenerateOutputSize();
			
			if (routeC)
			{
				data->registerCount = 1;
				size = Max(size, routeC->GenerateOutputSize());
			}
			else
			{
				data->passthruPort = 1;
			}
		}
		else
		{
			size = routeC->GenerateOutputSize();
			data->passthruPort = 2;
		}
	}
	
	data->outputSize = size;
	data->inputSize[0] = size;
	data->inputSize[1] = size;
	data->inputSize[2] = size;
}

int32 MultiplyAddProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	const Route *routeA = GetPortRoute(0);
	const Route *routeB = GetPortRoute(1);
	const Route *routeC = GetPortRoute(2);
	
	if (routeA)
	{
		if (routeB)
		{
			if (routeC)
			{
				static const char code[] =
				{
					"MAD		#, %0, %1, %2;\n"
				};
				
				programCode[0] = code;
				return (1);
			}
			
			static const char code[] =
			{
				"MUL		#, %0, %1;\n"
			};
			
			programCode[0] = code;
			return (1);
		}
		
		if (routeC)
		{
			static const char code[] =
			{
				"ADD		#, %0, %2;\n"
			};
			
			programCode[0] = code;
			return (1);
		}
	}
	else
	{
		if ((routeB) && (routeC))
		{
			static const char code[] =
			{
				"ADD		#, %1, %2;\n"
			};
			
			programCode[0] = code;
			return (1);
		}
	}
	
	return (0);
}

int32 MultiplyAddProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	const Route *routeA = GetPortRoute(0);
	const Route *routeB = GetPortRoute(1);
	const Route *routeC = GetPortRoute(2);
	
	if (routeA)
	{
		if (routeB)
		{
			if (routeC)
			{
				static const char code[] =
				{
					"# = %0 * %1 + %2;\n"
				};
				
				shaderCode[0] = code;
				return (1);
			}
			
			static const char code[] =
			{
				"# = %0 * %1;\n"
			};
			
			shaderCode[0] = code;
			return (1);
		}
		
		if (routeC)
		{
			static const char code[] =
			{
				"# = %0 + %2;\n"
			};
			
			shaderCode[0] = code;
			return (1);
		}
	}
	else
	{
		if ((routeB) && (routeC))
		{
			static const char code[] =
			{
				"# = %1 + %2;\n"
			};
			
			shaderCode[0] = code;
			return (1);
		}
	}
	
	return (0);
}


LerpProcess::LerpProcess() : TrinaryProcess(kProcessLerp)
{
}

LerpProcess::LerpProcess(const LerpProcess& lerpProcess) : TrinaryProcess(lerpProcess)
{
}

LerpProcess::~LerpProcess()
{
}

Process *LerpProcess::Replicate(void) const
{
	return (new LerpProcess(*this));
}

unsigned_int32 LerpProcess::GetPortFlags(int32 index) const
{
	return ((index < 2) ? kProcessPortOmissible : 0);
}

const char *LerpProcess::GetPortName(int32 index) const
{
	static const char *const portName[3] =
	{
		"A", "B", "t"
	};
	
	return (portName[index]);
}

void LerpProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	int32	size;
	
	const Route *routeA = GetPortRoute(0);
	const Route *routeB = GetPortRoute(1);
	
	if (routeA)
	{
		size = routeA->GenerateOutputSize();
		
		if (routeB)
		{
			data->registerCount = 1;
			size = Max(Max(size, routeB->GenerateOutputSize()), GetPortRoute(2)->GenerateOutputSize());
		}
		else
		{
			data->passthruPort = 0;
		}
	}
	else
	{
		size = routeB->GenerateOutputSize();
		data->passthruPort = 1;
	}
	
	data->outputSize = size;
	data->inputSize[0] = size;
	data->inputSize[1] = size;
	data->inputSize[2] = size;
}

int32 LerpProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	if ((GetPortRoute(0)) && (GetPortRoute(1)))
	{
		static const char code[] =
		{
			"LRP		#, %2, %1, %0;\n"
		};
		
		programCode[0] = code;
		return (1);
	}
	
	return (0);
}

int32 LerpProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	if ((GetPortRoute(0)) && (GetPortRoute(1)))
	{
		#if C4OPENGL
		
			static const char code[] =
			{
				"# = mix(%0, %1, %2);\n"
			};
		
		#else
		
			static const char code[] =
			{
				"# = lerp(%0, %1, %2);\n"
			};
		
		#endif
		
		shaderCode[0] = code;
		return (1);
	}
	
	return (0);
}


DiffuseProcess::DiffuseProcess() : Process(kProcessDiffuse)
{
}

DiffuseProcess::DiffuseProcess(const DiffuseProcess& diffuseProcess) : Process(diffuseProcess)
{
}

DiffuseProcess::~DiffuseProcess()
{
}

Process *DiffuseProcess::Replicate(void) const
{
	return (new DiffuseProcess(*this));
}

bool DiffuseProcess::ValidShader(ProcessType type, int32 shader)
{
	return (shader == kShaderGraphLight);
}

int32 DiffuseProcess::GetPortCount(void) const
{
	return (1);
}

unsigned_int32 DiffuseProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOptional);
}

const char *DiffuseProcess::GetPortName(int32 index) const
{
	return ("N");
}

int32 DiffuseProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = Process::GenerateProcessSignature(compileData, signature);
	signature[count] = ((GetPortRoute(0)) || (compileData->renderable->TangentAvailable()));
	return (count + 1);
}

int32 DiffuseProcess::GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const
{
	if ((GetPortRoute(0)) || (compileData->renderable->TangentAvailable()))
	{
		type[0] = kProcessTangentLightDirection;
		return (1);
	}
	
	type[0] = kProcessObjectNormal;
	type[1] = kProcessObjectLightDirection;
	return (2);
}

void DiffuseProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 1;
	data->inputSize[0] = 3;
}

int32 DiffuseProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char tangentCode[] =
	{
		"MOV_SAT	#, ldir.z;\n"
	};
	
	static const char normalCode[] =
	{
		"DP3_SAT	#, %0, ldir;\n"
	};
	
	static const char objectCode[] =
	{
		"DP3_SAT	#, onrm, oldr;\n"
	};
	
	if (GetPortRoute(0)) programCode[0] = normalCode;
	else if (compileData->renderable->TangentAvailable()) programCode[0] = tangentCode;
	else programCode[0] = objectCode;
	
	return (1);
}

int32 DiffuseProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	#if C4OPENGL
	
		static const char tangentCode[] =
		{
			"# = clamp(ldir.z, 0.0, 1.0);\n"
		};
		
		static const char normalCode[] =
		{
			"# = clamp(dot(%0, ldir), 0.0, 1.0);\n"
		};
		
		static const char objectCode[] =
		{
			"# = clamp(dot(onrm, oldr), 0.0, 1.0);\n"
		};
	
	#else
	
		static const char tangentCode[] =
		{
			"# = saturate(ldir.z);\n"
		};
		
		static const char normalCode[] =
		{
			"# = saturate(dot(%0, ldir));\n"
		};
		
		static const char objectCode[] =
		{
			"# = saturate(dot(onrm, oldr));\n"
		};
	
	#endif
	
	if (GetPortRoute(0)) shaderCode[0] = normalCode;
	else if (compileData->renderable->TangentAvailable()) shaderCode[0] = tangentCode;
	else shaderCode[0] = objectCode;
	
	return (1);
}


SpecularProcess::SpecularProcess() : Process(kProcessSpecular)
{
}

SpecularProcess::SpecularProcess(const SpecularProcess& specularProcess) : Process(specularProcess)
{
}

SpecularProcess::~SpecularProcess()
{
}

Process *SpecularProcess::Replicate(void) const
{
	return (new SpecularProcess(*this));
}

bool SpecularProcess::ValidShader(ProcessType type, int32 shader)
{
	return (shader == kShaderGraphLight);
}

int32 SpecularProcess::GetPortCount(void) const
{
	return (2);
}

unsigned_int32 SpecularProcess::GetPortFlags(int32 index) const
{
	return ((index == 0) ? kProcessPortOptional : 0);
}

const char *SpecularProcess::GetPortName(int32 index) const
{
	return ((index == 0) ? "N" : "p");
}

int32 SpecularProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = Process::GenerateProcessSignature(compileData, signature);
	signature[count] = ((GetPortRoute(0)) || (compileData->renderable->TangentAvailable()));
	return (count + 1);
}

int32 SpecularProcess::GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const
{
	if ((GetPortRoute(0)) || (compileData->renderable->TangentAvailable()))
	{
		type[0] = kProcessTangentHalfwayDirection;
		return (1);
	}
	
	type[0] = kProcessObjectNormal;
	type[1] = kProcessObjectHalfwayDirection;
	return (2);
}

void SpecularProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 1;
	data->inputSize[0] = 3;
	data->inputSize[1] = 1;
}

int32 SpecularProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char tangentCode[] =
	{
		"MOV_SAT	temp.z, hdir.z;\n"
		"POW		#, temp.z, %1;\n"
	};
	
	static const char normalCode[] =
	{
		"DP3_SAT	temp.x, %0, hdir;\n"
		"POW		#, temp.x, %1;\n"
	};
	
	static const char objectCode[] =
	{
		"DP3_SAT	temp.x, onrm, ohdr;\n"
		"POW		#, temp.x, %1;\n"
	};
	
	if (GetPortRoute(0)) programCode[0] = normalCode;
	else if (compileData->renderable->TangentAvailable()) programCode[0] = tangentCode;
	else programCode[0] = objectCode;
	
	return (1);
}

int32 SpecularProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	#if C4OPENGL
	
		static const char tangentCode[] =
		{
			"# = pow(clamp(hdir.z, 0.0, 1.0), %1);\n"
		};
		
		static const char normalCode[] =
		{
			"# = pow(clamp(dot(%0, hdir), 0.0, 1.0), %1);\n"
		};
		
		static const char objectCode[] =
		{
			"# = pow(clamp(dot(onrm, ohdr), 0.0, 1.0), %1);\n"
		};
	
	#else
	
		static const char tangentCode[] =
		{
			"# = pow(saturate(hdir.z), %1);\n"
		};
		
		static const char normalCode[] =
		{
			"# = pow(saturate(dot(%0, hdir)), %1);\n"
		};
		
		static const char objectCode[] =
		{
			"# = pow(saturate(dot(onrm, ohdr)), %1);\n"
		};
	
	#endif
	
	if (GetPortRoute(0)) shaderCode[0] = normalCode;
	else if (compileData->renderable->TangentAvailable()) shaderCode[0] = tangentCode;
	else shaderCode[0] = objectCode;
	
	return (1);
}


MicrofacetProcess::MicrofacetProcess() : Process(kProcessMicrofacet)
{
	microfacetParams.microfacetColor = K::white;
	microfacetParams.microfacetSlope.Set(0.5F, 0.5F);
	
	microfacetData = &microfacetParams;
}

MicrofacetProcess::MicrofacetProcess(const MicrofacetProcess& microfacetProcess) : Process(microfacetProcess)
{
	microfacetParams.microfacetColor = microfacetProcess.microfacetParams.microfacetColor;
	microfacetParams.microfacetSlope = microfacetProcess.microfacetParams.microfacetSlope;
	
	microfacetData = &microfacetParams;
}

MicrofacetProcess::~MicrofacetProcess()
{
	MicrofacetAttribute::MicrofacetTexture *texture = microfacetParams.microfacetTexture;
	if (texture) texture->Release();
}

Process *MicrofacetProcess::Replicate(void) const
{
	return (new MicrofacetProcess(*this));
}

bool MicrofacetProcess::ValidShader(ProcessType type, int32 shader)
{
	return (shader == kShaderGraphLight);
}

void MicrofacetProcess::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Process::Pack(data, packFlags);
	
	data << microfacetParams.microfacetColor;
	data << microfacetParams.microfacetSlope;
}

void MicrofacetProcess::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Process::Unpack(data, unpackFlags);
	
	data >> microfacetParams.microfacetColor;
	data >> microfacetParams.microfacetSlope;
}

int32 MicrofacetProcess::GetSettingCount(void) const
{
	return (Process::GetSettingCount() + 3);
}

Setting *MicrofacetProcess::GetSetting(int32 index) const
{
	int32 count = Process::GetSettingCount();
	if (index < count) return (Process::GetSetting(index));
	
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (index == count)
	{
		const char *title = table->GetString(StringID('PROC', kProcessMicrofacet, 'COLR'));
		const char *picker = table->GetString(StringID('PROC', kProcessMicrofacet, 'PICK'));
		return (new ColorSetting('COLR', microfacetParams.microfacetColor, title, picker));
	}
	
	if (index == count + 1)
	{
		const char *title = table->GetString(StringID('PROC', kProcessMicrofacet, 'SLPX'));
		return (new FloatSetting('SLPX', microfacetParams.microfacetSlope.x, title, 0.01F, 0.5F, 0.01F));
	}
	
	if (index == count + 2)
	{
		const char *title = table->GetString(StringID('PROC', kProcessMicrofacet, 'SLPY'));
		return (new FloatSetting('SLPY', microfacetParams.microfacetSlope.y, title, 0.01F, 0.5F, 0.01F));
	}
	
	return (nullptr);
}

void MicrofacetProcess::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'COLR')
	{
		microfacetParams.microfacetColor = static_cast<const ColorSetting *>(setting)->GetColor();
		microfacetParams.Invalidate();
	}
	else if (identifier == 'SLPX')
	{
		microfacetParams.microfacetSlope.x = static_cast<const FloatSetting *>(setting)->GetFloatValue();
		microfacetParams.Invalidate();
	}
	else if (identifier == 'SLPY')
	{
		microfacetParams.microfacetSlope.y = static_cast<const FloatSetting *>(setting)->GetFloatValue();
		microfacetParams.Invalidate();
	}
	else
	{
		Process::SetSetting(setting);
	}
}

bool MicrofacetProcess::operator ==(const Process& process) const
{
	if (Process::operator ==(process))
	{
		const MicrofacetProcess& microfacetProcess = static_cast<const MicrofacetProcess&>(process);
		return ((microfacetParams.microfacetColor == microfacetProcess.microfacetParams.microfacetColor) && (microfacetParams.microfacetSlope == microfacetProcess.microfacetParams.microfacetSlope));
	}
	
	return (false);
}

int32 MicrofacetProcess::GetPortCount(void) const
{
	return (1);
}

unsigned_int32 MicrofacetProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOptional);
}

const char *MicrofacetProcess::GetPortName(int32 index) const
{
	return ("N");
}

void MicrofacetProcess::ReferenceStateParams(const Process *process)
{
	microfacetData = static_cast<const MicrofacetProcess *>(process)->microfacetData;
}

int32 MicrofacetProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = Process::GenerateProcessSignature(compileData, signature);
	signature += count;
	
	signature[0] = *reinterpret_cast<const unsigned_int32 *>(&microfacetParams.microfacetColor.red);
	signature[1] = *reinterpret_cast<const unsigned_int32 *>(&microfacetParams.microfacetColor.green);
	signature[2] = *reinterpret_cast<const unsigned_int32 *>(&microfacetParams.microfacetColor.blue);
	signature[3] = *reinterpret_cast<const unsigned_int32 *>(&microfacetParams.microfacetSlope.x);
	signature[4] = *reinterpret_cast<const unsigned_int32 *>(&microfacetParams.microfacetSlope.y);
	
	return (count + 5);
}

int32 MicrofacetProcess::GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const
{
	type[0] = kProcessTangentHalfwayDirection;
	return (1);
}

void MicrofacetProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 3;
	data->inputSize[0] = 3;
	data->inputSize[1] = 1;
	
	data->textureCount = 1;
	data->textureObject[0] = MicrofacetAttribute::GetTextureObject(microfacetData);
	
	// The literal constants must be stored after the MicrofacetAttribute::GetTextureObject()
	// function is called because that's where microfacetThreshold is calculated.
	
	float value = 1.0F / (1.0F - microfacetData->microfacetThreshold);
	
	data->literalCount = 2;
	data->literalData[0].literalType = 'MTH1';
	data->literalData[0].literalValue = value;
	data->literalData[1].literalType = 'MTH2';
	data->literalData[1].literalValue = 1.0F - value;
}

int32 MicrofacetProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	if (microfacetParams.microfacetSlope.x == microfacetParams.microfacetSlope.y)
	{
		static const char flatCode[] =
		{
			"MAD		temp.x, hdir.z, &MTH1, &MTH2;\n"
			"DP3		temp.y, ldir, hdir;\n"
			"TEX		temp.xyz, temp, %IMG0, 2D;\n"
			"RCP		temp.w, vdir.z;\n"
			"MUL		#, temp, temp.w;\n"
		};
		
		static const char flatCode2[] =
		{
			"MAD		temp.x, hdir.z, &MTH1, &MTH2;\n"
			"DP3		temp.y, ldir, hdir;\n"
			"TEX		temp.xyz, temp, %IMG0, 2D;\n"
			"DIV		#, temp, vdir.z;\n"
		};
		
		static const char bumpCode[] =
		{
			"DP3		temp.x, %0, hdir;\n"
			"MAD		temp.x, temp.x, &MTH1, &MTH2;\n"
			"DP3		temp.y, ldir, hdir;\n"
			"TEX		temp.xyz, temp, %IMG0, 2D;\n"
			"DP3		temp.w, %0, vdir;\n"
			"RCP		temp.w, temp.w;\n"
			"MUL		#, temp, temp.w;\n"
		};
		
		static const char bumpCode2[] =
		{
			"DP3		temp.x, %0, hdir;\n"
			"MAD		temp.x, temp.x, &MTH1, &MTH2;\n"
			"DP3		temp.y, ldir, hdir;\n"
			"TEX		temp.xyz, temp, %IMG0, 2D;\n"
			"DP3		temp.w, %0, vdir;\n"
			"DIV		#, temp, temp.w;\n"
		};
		
		if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2])
		{
			if (GetPortRoute(0)) programCode[0] = bumpCode2;
			else programCode[0] = flatCode2;
		}
		else
		{
			if (GetPortRoute(0)) programCode[0] = bumpCode;
			else programCode[0] = flatCode;
		}
	}
	else
	{
		static const char flatCode[] =
		{
			"MAD		temp.x, hdir.z, &MTH1, &MTH2;\n"
			"DP3		temp.y, ldir, hdir;\n"
			"MUL		temp.w, hdir.x, hdir.x;\n"
			"MAD		temp.z, hdir.y, hdir.y, temp.w;\n"
			"RCP		temp.z, temp.z;\n"
			"MUL		temp.z, temp.w, temp.z;\n"
			"TEX		temp.xyz, temp, %IMG0, 3D;\n"
			"RCP		temp.w, vdir.z;\n"
			"MUL		#, temp, temp.w;\n"
		};
		
		static const char flatCode2[] =
		{
			"MAD		temp.x, hdir.z, &MTH1, &MTH2;\n"
			"DP3		temp.y, ldir, hdir;\n"
			"MUL		temp.w, hdir.x, hdir.x;\n"
			"MAD		temp.z, hdir.y, hdir.y, temp.w;\n"
			"DIV		temp.z, temp.w, temp.z;\n"
			"TEX		temp.xyz, temp, %IMG0, 3D;\n"
			"DIV		#, temp, vdir.z;\n"
		};
		
		static const char bumpCode[] =
		{
			"DP3		temp.x, %0, hdir;\n"
			"MAD		temp.x, temp.x, &MTH1, &MTH2;\n"
			"DP3		temp.y, ldir, hdir;\n"
			"MUL		temp.w, hdir.x, hdir.x;\n"
			"MAD		temp.z, hdir.y, hdir.y, temp.w;\n"
			"RCP		temp.z, temp.z;\n"
			"MUL		temp.z, temp.w, temp.z;\n"
			"TEX		temp.xyz, temp, %IMG0, 3D;\n"
			"DP3		temp.w, %0, vdir;\n"
			"RCP		temp.w, temp.w;\n"
			"MUL		#, temp, temp.w;\n"
		};
		
		static const char bumpCode2[] =
		{
			"DP3		temp.x, %0, hdir;\n"
			"MAD		temp.x, temp.x, &MTH1, &MTH2;\n"
			"DP3		temp.y, ldir, hdir;\n"
			"MUL		temp.w, hdir.x, hdir.x;\n"
			"MAD		temp.z, hdir.y, hdir.y, temp.w;\n"
			"DIV		temp.z, temp.w, temp.z;\n"
			"TEX		temp.xyz, temp, %IMG0, 3D;\n"
			"DP3		temp.w, %0, vdir;\n"
			"DIV		#, temp, temp.w;\n"
		};
		
		if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2])
		{
			if (GetPortRoute(0)) programCode[0] = bumpCode2;
			else programCode[0] = flatCode2;
		}
		
		if (GetPortRoute(0)) programCode[0] = bumpCode;
		else programCode[0] = flatCode;
	}
	
	return (1);
}

int32 MicrofacetProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	if (microfacetParams.microfacetSlope.x == microfacetParams.microfacetSlope.y)
	{
		static const char flatCode[] =
		{
			"temp.x = hdir.z * &MTH1 + &MTH2;\n"
			"temp.y = dot(ldir, hdir);\n"
			"# = " TEX2D "(%IMG0, temp.xy).xyz / vdir.z;\n"
		};
		
		static const char bumpCode[] =
		{
			"temp.x = dot(%0, hdir) * &MTH1 + &MTH2;\n"
			"temp.y = dot(ldir, hdir);\n"
			"# = " TEX2D "(%IMG0, temp.xy).xyz / dot(%0, vdir);\n"
		};
		
		if (GetPortRoute(0)) shaderCode[0] = bumpCode;
		else shaderCode[0] = flatCode;
	}
	else
	{
		static const char flatCode[] =
		{
			"temp.x = hdir.z * &MTH1 + &MTH2;\n"
			"temp.y = dot(ldir, hdir);\n"
			"temp.w = hdir.x * hdir.x;\n"
			"temp.z = temp.w / (temp.w + hdir.y * hdir.y);\n"
			"# = " TEX3D "(%IMG0, temp.xyz).xyz / vdir.z;\n"
		};
		
		static const char bumpCode[] =
		{
			"temp.x = dot(%0, hdir) * &MTH1 + &MTH2;\n"
			"temp.y = dot(ldir, hdir);\n"
			"temp.w = hdir.x * hdir.x;\n"
			"temp.z = temp.w / (temp.w + hdir.y * hdir.y);\n"
			"# = " TEX3D "(%IMG0, temp.xyz).xyz / dot(%0, vdir);\n"
		};
		
		if (GetPortRoute(0)) shaderCode[0] = bumpCode;
		else shaderCode[0] = flatCode;
	}
	
	return (1);
}


TerrainDiffuseProcess::TerrainDiffuseProcess() : Process(kProcessTerrainDiffuse)
{
}

TerrainDiffuseProcess::TerrainDiffuseProcess(const TerrainDiffuseProcess& terrainDiffuseProcess) : Process(terrainDiffuseProcess)
{
}

TerrainDiffuseProcess::~TerrainDiffuseProcess()
{
}

Process *TerrainDiffuseProcess::Replicate(void) const
{
	return (new TerrainDiffuseProcess(*this));
}

bool TerrainDiffuseProcess::ValidShader(ProcessType type, int32 shader)
{
	return (shader == kShaderGraphLight);
}

int32 TerrainDiffuseProcess::GetPortCount(void) const
{
	return (3);
}

unsigned_int32 TerrainDiffuseProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOptional);
}

const char *TerrainDiffuseProcess::GetPortName(int32 index) const
{
	static const char *const portName[3] =
	{
		"N1", "N2", "N3"
	};
	
	return (portName[index]);
}

bool TerrainDiffuseProcess::BumpEnabled(void) const
{
	if (!(TheGraphicsMgr->GetRenderOptionFlags() & kRenderOptionTerrainBumps)) return (false);
	return ((GetPortRoute(0)) && (GetPortRoute(1)) && (GetPortRoute(2)));
}

int32 TerrainDiffuseProcess::GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const
{
	if (BumpEnabled())
	{
		type[0] = kProcessTriplanarBlend;
		type[1] = kProcessTerrainLightDirection;
		return (2);
	}
	
	type[0] = kProcessObjectLightDirection;
	return (1);
}

void TerrainDiffuseProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 1;
	data->inputSize[0] = 3;
	data->inputSize[1] = 3;
	data->inputSize[2] = 3;
	
	if (!BumpEnabled())
	{
		data->interpolantCount = 1;
		data->interpolantType[0] = 'NRML';
	}
}

int32 TerrainDiffuseProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char flatCode[] =
	{
		"DP3_SAT	#, $NRML, oldr;\n"
	};
	
	static const char bumpCode[] =
	{
		"DP3_SAT	temp.x, %0, tldr;\n"
		"DP3_SAT	temp.y, %1, tldr;\n"
		"DP3_SAT	temp.z, %2, tld2;\n"
		"DP3		#, temp, tbld;\n"
	};
	
	if (BumpEnabled()) programCode[0] = bumpCode;
	else programCode[0] = flatCode;
	
	return (1);
}

int32 TerrainDiffuseProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	#if C4OPENGL
	
		static const char flatCode[] =
		{
			"# = clamp(dot($NRML, oldr), 0.0, 1.0);\n"
		};
		
		static const char bumpCode[] =
		{
			"temp.x = clamp(dot(%0, tldr), 0.0, 1.0);\n"
			"temp.y = clamp(dot(%1, tldr), 0.0, 1.0);\n"
			"temp.z = clamp(dot(%2, tld2), 0.0, 1.0);\n"
			"# = dot(temp.xyz, tbld);\n"
		};
	
	#else
	
		static const char flatCode[] =
		{
			"# = saturate(dot($NRML, oldr));\n"
		};
		
		static const char bumpCode[] =
		{
			"temp.x = saturate(dot(%0, tldr));\n"
			"temp.y = saturate(dot(%1, tldr));\n"
			"temp.z = saturate(dot(%2, tld2));\n"
			"# = dot(temp.xyz, tbld);\n"
		};
	
	#endif
	
	if (BumpEnabled()) shaderCode[0] = bumpCode;
	else shaderCode[0] = flatCode;
	
	return (1);
}


TerrainSpecularProcess::TerrainSpecularProcess() : Process(kProcessTerrainSpecular)
{
}

TerrainSpecularProcess::TerrainSpecularProcess(const TerrainSpecularProcess& terrainSpecularProcess) : Process(terrainSpecularProcess)
{
}

TerrainSpecularProcess::~TerrainSpecularProcess()
{
}

Process *TerrainSpecularProcess::Replicate(void) const
{
	return (new TerrainSpecularProcess(*this));
}

bool TerrainSpecularProcess::ValidShader(ProcessType type, int32 shader)
{
	return (shader == kShaderGraphLight);
}

int32 TerrainSpecularProcess::GetPortCount(void) const
{
	return (4);
}

unsigned_int32 TerrainSpecularProcess::GetPortFlags(int32 index) const
{
	return ((index < 3) ? kProcessPortOptional : 0);
}

const char *TerrainSpecularProcess::GetPortName(int32 index) const
{
	static const char *const portName[4] =
	{
		"N1", "N2", "N3", "p"
	};
	
	return (portName[index]);
}

bool TerrainSpecularProcess::BumpEnabled(void) const
{
	if (!(TheGraphicsMgr->GetRenderOptionFlags() & kRenderOptionTerrainBumps)) return (false);
	return ((GetPortRoute(0)) && (GetPortRoute(1)) && (GetPortRoute(2)));
}

int32 TerrainSpecularProcess::GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const
{
	if (BumpEnabled())
	{
		type[0] = kProcessTriplanarBlend;
		type[1] = kProcessTerrainHalfwayDirection;
		return (2);
	}
	
	type[0] = kProcessObjectHalfwayDirection;
	return (1);
}

void TerrainSpecularProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 1;
	data->inputSize[0] = 3;
	data->inputSize[1] = 3;
	data->inputSize[2] = 3;
	data->inputSize[3] = 1;
	
	if (!BumpEnabled())
	{
		data->interpolantCount = 1;
		data->interpolantType[0] = 'NRML';
	}
}

int32 TerrainSpecularProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char flatCode[] =
	{
		"DP3_SAT	temp.x, $NRML, ohdr;\n"
		"POW		#, temp.x, %3;\n"
	};
	
	static const char bumpCode[] =
	{
		"DP3_SAT	temp.x, %0, thdr;\n"
		"DP3_SAT	temp.y, %1, thdr;\n"
		"DP3_SAT	temp.z, %2, thd2;\n"
		"DP3		temp.w, temp, tbld;\n"
		"POW		#, temp.w, %3;\n"
	};
	
	if (BumpEnabled()) programCode[0] = bumpCode;
	else programCode[0] = flatCode;
	
	return (1);
}

int32 TerrainSpecularProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	#if C4OPENGL
	
		static const char flatCode[] =
		{
			"# = pow(clamp(dot($NRML, ohdr), 0.0, 1.0), %3);\n"
		};
		
		static const char bumpCode[] =
		{
			"temp.x = clamp(dot(%0, thdr), 0.0, 1.0);\n"
			"temp.y = clamp(dot(%1, thdr), 0.0, 1.0);\n"
			"temp.z = clamp(dot(%2, thd2), 0.0, 1.0);\n"
			"# = pow(dot(temp.xyz, tbld), %3);\n"
		};
	
	#else
	
		static const char flatCode[] =
		{
			"# = pow(saturate(dot($NRML, ohdr)), %3);\n"
		};
		
		static const char bumpCode[] =
		{
			"temp.x = saturate(dot(%0, thdr));\n"
			"temp.y = saturate(dot(%1, thdr));\n"
			"temp.z = saturate(dot(%2, thd2));\n"
			"# = pow(dot(temp.xyz, tbld), %3);\n"
		};
	
	#endif
	
	if (BumpEnabled()) shaderCode[0] = bumpCode;
	else shaderCode[0] = flatCode;
	
	return (1);
}


GenerateImpostorNormalProcess::GenerateImpostorNormalProcess() : Process(kProcessGenerateImpostorNormal)
{
}

GenerateImpostorNormalProcess::GenerateImpostorNormalProcess(const GenerateImpostorNormalProcess& generateImpostorNormalProcess) : Process(generateImpostorNormalProcess)
{
}

GenerateImpostorNormalProcess::~GenerateImpostorNormalProcess()
{
}

Process *GenerateImpostorNormalProcess::Replicate(void) const
{
	return (new GenerateImpostorNormalProcess(*this));
}

bool GenerateImpostorNormalProcess::ValidShader(ProcessType type, int32 shader)
{
	return (shader == kShaderGraphAmbient);
}

void GenerateImpostorNormalProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 3;
	
	data->interpolantCount = 1;
	data->interpolantType[0] = 'NRMC';
}

int32 GenerateImpostorNormalProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"DP3		temp.w, $NRMC, $NRMC;\n"
		"RSQ		temp.w, temp.w;\n"
		"MUL		temp.xyz, $NRMC, temp.w;\n"
		"MAD		#, temp, {0.5, -0.5, -0.5, 0.0}, 0.5;\n"
	};
	
	static const char code2[] =
	{
		"NRMH		temp.xyz, $NRMC;\n"
		"MADH		#, temp, {0.5, -0.5, -0.5, 0.0}, 0.5;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 GenerateImpostorNormalProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = normalize($NRMC) * " FLOAT3 "(0.5, -0.5, -0.5) + " FLOAT3 "(0.5, 0.5, 0.5);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


ImpostorDepthProcess::ImpostorDepthProcess() : InterpolantProcess(kProcessImpostorDepth)
{
}

ImpostorDepthProcess::ImpostorDepthProcess(const ImpostorDepthProcess& impostorDepthProcess) : InterpolantProcess(impostorDepthProcess)
{
}

ImpostorDepthProcess::~ImpostorDepthProcess()
{
}

Process *ImpostorDepthProcess::Replicate(void) const
{
	return (new ImpostorDepthProcess(*this));
}

bool ImpostorDepthProcess::ValidShader(ProcessType type, int32 shader)
{
	return (shader == kShaderGraphAmbient);
}


CombineNormalsProcess::CombineNormalsProcess() : Process(kProcessCombineNormals)
{
}

CombineNormalsProcess::CombineNormalsProcess(const CombineNormalsProcess& combineNormalsProcess) : Process(combineNormalsProcess)
{
}

CombineNormalsProcess::~CombineNormalsProcess()
{
}

Process *CombineNormalsProcess::Replicate(void) const
{
	return (new CombineNormalsProcess(*this));
}

int32 CombineNormalsProcess::GetPortCount(void) const
{
	return (2);
}

const char *CombineNormalsProcess::GetPortName(int32 index) const
{
	return ((index == 0) ? "N1" : "N2");
}

void CombineNormalsProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	if (compileData->programFlag) data->temporaryCount = 1;
	
	data->outputSize = 3;
	data->inputSize[0] = 3;
	data->inputSize[1] = 3;
}

int32 CombineNormalsProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"RCP		temp.w, %0.z;\n"
		"RCP		tmp1.w, %1.z;\n"
		"MUL		temp.xy, %0, temp.w;\n"
		"MAD		tmp1.xy, %1, tmp1.w, temp;\n"
		"MOV		tmp1.z, 1.0;\n"
		
		"DP3		temp.x, tmp1, tmp1;\n"
		"RSQ		temp.x, temp.x;\n"
		"MUL		#, tmp1, temp.x;\n"
	};
	
	static const char code2[] =
	{
		"DIV		temp.xy, %0, %0.z;\n"
		"DIV		tmp1.xy, %1, %1.z;\n"
		"ADDH		tmp1.xy, temp, tmp1;\n"
		"MOVH		tmp1.z, 1.0;\n"
		
		"NRMH		#, tmp1;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 CombineNormalsProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"temp.xy = %0.xy / %0.z + %1.xy / %1.z;\n"
		"temp.z = 1.0;\n"
		"# = normalize(temp.xyz);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


FrontNormalProcess::FrontNormalProcess() : Process(kProcessFrontNormal)
{
}

FrontNormalProcess::FrontNormalProcess(const FrontNormalProcess& frontNormalProcess) : Process(frontNormalProcess)
{
}

FrontNormalProcess::~FrontNormalProcess()
{
}

Process *FrontNormalProcess::Replicate(void) const
{
	return (new FrontNormalProcess(*this));
}

bool FrontNormalProcess::ValidShader(ProcessType type, int32 shader)
{
	return (shader == kShaderGraphLight);
}

int32 FrontNormalProcess::GetPortCount(void) const
{
	return (1);
}

const char *FrontNormalProcess::GetPortName(int32 index) const
{
	return ("N");
}

int32 FrontNormalProcess::GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const
{
	type[0] = kProcessTangentLightDirection;
	return (1);
}

void FrontNormalProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	const Route *route = GetFirstIncomingEdge();
	ProcessType type = route->GetStartElement()->GetBaseProcessType();
	
	if ((type == kProcessConstant) || (type == kProcessParameter) || (type == kProcessInterpolant) || (type == kProcessDerived) || (route->GetRouteNegation()) || (route->GetRouteSwizzle() != 'xyzw'))
	{
		data->registerCount = 1;
	}
	else
	{
		data->passthruPort = 0;
	}
	
	data->outputSize = 3;
	data->inputSize[0] = 3;
}

int32 FrontNormalProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	if (GetProcessData()->passthruPort >= 0)
	{
		static const char code[] =
		{
			"SGE		temp.z, ldir.z, 0.0;\n"
			"MAD		temp.z, temp.z, 2.0, -1.0;\n"
			"MUL		#.z, %0, temp.z;\n"
		};
		
		static const char code2[] =
		{
			"SLTC		temp.z, ldir.z, 0.0;\n"
			"MOV		#.z (GT.z), -%0;\n"
		};
		
		if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
		else programCode[0] = code;
	}
	else
	{
		static const char code[] =
		{
			"MOV		#.xy, %0;\n"
			"SGE		temp.z, ldir.z, 0.0;\n"
			"MAD		temp.z, temp.z, 2.0, -1.0;\n"
			"MUL		#.z, %0, temp.z;\n"
		};
		
		static const char code2[] =
		{
			"MOV		#.xyz, %0;\n"
			"SLTC		temp.z, ldir.z, 0.0;\n"
			"MOV		#.z (GT.z), -%0;\n"
		};
		
		if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
		else programCode[0] = code;
	}
	
	return (1);
}

int32 FrontNormalProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"if (ldir.z < 0.0) #.z = -%0.z;\n"
	};
	
	static const char moveCode[] =
	{
		"#.xy = %0.xy;\n"
		"#.z = (ldir.z < 0.0) ? -%0.z : %0.z;\n"
	};
	
	if (GetProcessData()->passthruPort >= 0) shaderCode[0] = code;
	else shaderCode[0] = moveCode;
	
	return (1);
}


ReflectVectorProcess::ReflectVectorProcess() : Process(kProcessReflectVector)
{
}

ReflectVectorProcess::ReflectVectorProcess(const ReflectVectorProcess& reflectVectorProcess) : Process(reflectVectorProcess)
{
}

ReflectVectorProcess::~ReflectVectorProcess()
{
}

Process *ReflectVectorProcess::Replicate(void) const
{
	return (new ReflectVectorProcess(*this));
}

int32 ReflectVectorProcess::GetPortCount(void) const
{
	return (2);
}

unsigned_int32 ReflectVectorProcess::GetPortFlags(int32 index) const
{
	return ((index == 0) ? 0 : kProcessPortOptional);
}

const char *ReflectVectorProcess::GetPortName(int32 index) const
{
	return ((index == 0) ? "V" : "N");
}

void ReflectVectorProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 3;
	data->inputSize[0] = 3;
	data->inputSize[1] = 3;
}

int32 ReflectVectorProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char flatCode[] =
	{
		"MUL		#, %0, {-1.0, -1.0, 1.0, 1.0};\n"
	};
	
	static const char bumpCode[] =
	{
		"DP3		temp.x, %0, %1;\n"
		"MUL		temp.x, temp.x, 2.0;\n"
		"MAD		#, %1, temp.x, -%0;\n"
	};
	
	if (GetPortRoute(1)) programCode[0] = bumpCode;
	else programCode[0] = flatCode;
	
	return (1);
}

int32 ReflectVectorProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char flatCode[] =
	{
		"# = %0 * " FLOAT3 "(-1.0, -1.0, 1.0);\n"
	};
	
	static const char bumpCode[] =
	{
		"# = %1 * (dot(%0, %1) * 2.0) - %0;\n"
	};
	
	if (GetPortRoute(1)) shaderCode[0] = bumpCode;
	else shaderCode[0] = flatCode;
	
	return (1);
}


LinearRampProcess::LinearRampProcess() : UnaryProcess(kProcessLinearRamp)
{
	rampCenter = 0.5F;
	rampWidth = 0.1F;
}

LinearRampProcess::LinearRampProcess(const LinearRampProcess& linearRampProcess) : UnaryProcess(linearRampProcess)
{
	rampCenter = linearRampProcess.rampCenter;
	rampWidth = linearRampProcess.rampWidth;
}

LinearRampProcess::~LinearRampProcess()
{
}

Process *LinearRampProcess::Replicate(void) const
{
	return (new LinearRampProcess(*this));
}

void LinearRampProcess::Pack(Packer& data, unsigned_int32 packFlags) const
{
	UnaryProcess::Pack(data, packFlags);
	
	data << rampCenter;
	data << rampWidth;
}

void LinearRampProcess::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnaryProcess::Unpack(data, unpackFlags);
	
	data >> rampCenter;
	data >> rampWidth;
}

int32 LinearRampProcess::GetSettingCount(void) const
{
	return (UnaryProcess::GetSettingCount() + 2);
}

Setting *LinearRampProcess::GetSetting(int32 index) const
{
	int32 count = UnaryProcess::GetSettingCount();
	if (index < count) return (UnaryProcess::GetSetting(index));
	
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (index == count)
	{
		const char *title = table->GetString(StringID('PROC', kProcessLinearRamp, 'CENT'));
		return (new TextSetting('CENT', rampCenter, title));
	}
	
	if (index == count + 1)
	{
		const char *title = table->GetString(StringID('PROC', kProcessLinearRamp, 'WIDE'));
		return (new FloatSetting('WIDE', rampWidth, title, 0.01F, 0.99F, 0.01F));
	}
	
	return (nullptr);
}

void LinearRampProcess::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'CENT')
	{
		rampCenter = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
	}
	else if (identifier == 'WIDE')
	{
		rampWidth = static_cast<const FloatSetting *>(setting)->GetFloatValue();
	}
	else
	{
		UnaryProcess::SetSetting(setting);
	}
}

bool LinearRampProcess::operator ==(const Process& process) const
{
	if (UnaryProcess::operator ==(process))
	{
		const LinearRampProcess& linearRampProcess = static_cast<const LinearRampProcess&>(process);
		return (rampWidth == linearRampProcess.rampWidth);
	}
	
	return (false);
}

int32 LinearRampProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = UnaryProcess::GenerateProcessSignature(compileData, signature);
	signature[count] = *reinterpret_cast<const unsigned_int32 *>(&rampCenter);
	signature[count + 1] = *reinterpret_cast<const unsigned_int32 *>(&rampWidth);
	return (count + 2);
}

void LinearRampProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	UnaryProcess::GenerateProcessData(compileData, data);
	
	float scale = 1.0F / rampWidth;
	
	data->literalCount = 2;
	data->literalData[0].literalType = 'RMUL';
	data->literalData[0].literalValue = scale;
	data->literalData[1].literalType = 'RADD';
	data->literalData[1].literalValue = 0.5F - rampCenter * scale;
}

int32 LinearRampProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"MAD_SAT	#, %0, &RMUL, &RADD;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 LinearRampProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		#if C4OPENGL
		
			"# = clamp(%0 * &RMUL + &RADD, 0.0, 1.0);\n"
		
		#else
		
			"# = saturate(%0 * &RMUL + &RADD);\n"
		
		#endif
	};
	
	shaderCode[0] = code;
	return (1);
}


SmoothParameterProcess::SmoothParameterProcess() : UnaryProcess(kProcessSmoothParameter)
{
}

SmoothParameterProcess::SmoothParameterProcess(const SmoothParameterProcess& smoothParameterProcess) : UnaryProcess(smoothParameterProcess)
{
}

SmoothParameterProcess::~SmoothParameterProcess()
{
}

Process *SmoothParameterProcess::Replicate(void) const
{
	return (new SmoothParameterProcess(*this));
}

const char *SmoothParameterProcess::GetPortName(int32 index) const
{
	return ("t");
}

int32 SmoothParameterProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"MUL		temp, %0, %0;\n"
		"MAD		#, %0, -2.0, 3.0;\n"
		"MUL		#, ##, temp;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 SmoothParameterProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = (3.0 - %0 * 2.0) * (%0 * %0);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


SteepParameterProcess::SteepParameterProcess() : UnaryProcess(kProcessSteepParameter)
{
}

SteepParameterProcess::SteepParameterProcess(const SteepParameterProcess& steepParameterProcess) : UnaryProcess(steepParameterProcess)
{
}

SteepParameterProcess::~SteepParameterProcess()
{
}

Process *SteepParameterProcess::Replicate(void) const
{
	return (new SteepParameterProcess(*this));
}

const char *SteepParameterProcess::GetPortName(int32 index) const
{
	return ("t");
}

int32 SteepParameterProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"MUL		temp, %0, %0;\n"
		"MAD		#, %0, 2.0, -temp;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 SteepParameterProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = %0 * 2.0 - %0 * %0;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


WorldTransformProcess::WorldTransformProcess() : Process(kProcessWorldTransform)
{
}

WorldTransformProcess::WorldTransformProcess(const WorldTransformProcess& worldTransformProcess) : Process(worldTransformProcess)
{
}

WorldTransformProcess::~WorldTransformProcess()
{
}

Process *WorldTransformProcess::Replicate(void) const
{
	return (new WorldTransformProcess(*this));
}

int32 WorldTransformProcess::GetPortCount(void) const
{
	return (1);
}

const char *WorldTransformProcess::GetPortName(int32 index) const
{
	return ("V");
}

void WorldTransformProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	if (compileData->programFlag) data->preregisterCount = 1;
	else data->registerCount = 1;
	
	data->outputSize = 3;
	data->inputSize[0] = 3;
	
	data->interpolantCount = 3;
	data->interpolantType[0] = 'WTAN';
	data->interpolantType[1] = 'WBTN';
	data->interpolantType[2] = 'WNRM';
}

int32 WorldTransformProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"MUL		temp.xyz, $WTAN, %0.x;\n"
		"MAD		temp.xyz, $WBTN, %0.y, temp;\n"
		"MAD		#, $WNRM, %0.z, temp;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 WorldTransformProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = $WTAN * %0.x + $WBTN * %0.y + $WNRM * %0.z;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


DeltaDepthProcess::DeltaDepthProcess() : Process(kProcessDeltaDepth)
{
	deltaScale = 1.0F;
}

DeltaDepthProcess::DeltaDepthProcess(const DeltaDepthProcess& deltaDepthProcess) : Process(deltaDepthProcess)
{
	deltaScale = deltaDepthProcess.deltaScale;
}

DeltaDepthProcess::~DeltaDepthProcess()
{
}

Process *DeltaDepthProcess::Replicate(void) const
{
	return (new DeltaDepthProcess(*this));
}

void DeltaDepthProcess::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Process::Pack(data, packFlags);
	
	data << deltaScale;
}

void DeltaDepthProcess::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Process::Unpack(data, unpackFlags);
	
	data >> deltaScale;
}

int32 DeltaDepthProcess::GetSettingCount(void) const
{
	return (Process::GetSettingCount() + 1);
}

Setting *DeltaDepthProcess::GetSetting(int32 index) const
{
	int32 count = Process::GetSettingCount();
	if (index < count) return (Process::GetSetting(index));
	
	if (index == count)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		const char *title = table->GetString(StringID('PROC', kProcessDeltaDepth, 'SCAL'));
		return (new TextSetting('SCAL', deltaScale, title));
	}
	
	return (nullptr);
}

void DeltaDepthProcess::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'SCAL')
	{
		deltaScale = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
	}
	else
	{
		Process::SetSetting(setting);
	}
}

bool DeltaDepthProcess::operator ==(const Process& process) const
{
	if (Process::operator ==(process))
	{
		const DeltaDepthProcess& deltaDepthProcess = static_cast<const DeltaDepthProcess&>(process);
		return (deltaScale == deltaDepthProcess.deltaScale);
	}
	
	return (false);
}

bool DeltaDepthProcess::StructureEffectsEnabled(void)
{
	return (((TheGraphicsMgr->GetRenderTargetMask() & (1 << kRenderTargetStructure)) != 0) && ((TheGraphicsMgr->GetRenderOptionFlags() & kRenderOptionStructureEffects) != 0));
}

int32 DeltaDepthProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = Process::GenerateProcessSignature(compileData, signature);
	signature[count] = (StructureEffectsEnabled()) ? *reinterpret_cast<const unsigned_int32 *>(&deltaScale) : -1;
	return (count + 1);
}

void DeltaDepthProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 1;
	
	if (StructureEffectsEnabled())
	{
		data->temporaryCount = 1;
		
		data->literalCount = 1;
		data->literalData[0].literalType = 'DSCL';
		data->literalData[0].literalValue = deltaScale;
		
		data->textureCount = 1;
		data->textureObject[0] = TheGraphicsMgr->GetStructureTexture();
	}
}

int32 DeltaDepthProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char enabledCode[] =
	{
		"RCP		temp.w, fragment.position.w;\n"
		"TEX		temp.z, fragment.position, %IMG0, RECT;\n"
		"SUB		temp.z, temp.z, temp.w;\n"
		"MUL_SAT	#, temp.z, &DSCL;\n"
	};
	
	static const char disabledCode[] =
	{
		"MOV		#, 1.0;\n"
	};
	
	if (StructureEffectsEnabled()) programCode[0] = enabledCode;
	else programCode[0] = disabledCode;
	
	return (1);
}

int32 DeltaDepthProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char enabledCode[] =
	{
		#if C4OPENGL
		
			"temp.z = texture2DRect(%IMG0, gl_FragCoord.xy).z;\n"
			"# = clamp((temp.z - 1.0 / gl_FragCoord.w) * &DSCL, 0.0, 1.0);\n"
		
		#else
		
			"temp.z = texRECT(%IMG0, fragment.position.xy).z;\n"
			"# = saturate((temp.z - 1.0 / fragment.position.w) * &DSCL);\n"
		
		#endif
	};
	
	static const char disabledCode[] =
	{
		"# = 1.0;\n"
	};
	
	if (StructureEffectsEnabled()) shaderCode[0] = enabledCode;
	else shaderCode[0] = disabledCode;
	
	return (1);
}


ParallaxProcess::ParallaxProcess() : TextureMapProcess(kProcessParallax)
{
}

ParallaxProcess::ParallaxProcess(const ParallaxProcess& parallaxProcess) : TextureMapProcess(parallaxProcess)
{
}

ParallaxProcess::~ParallaxProcess()
{
}

Process *ParallaxProcess::Replicate(void) const
{
	return (new ParallaxProcess(*this));
}

int32 ParallaxProcess::GetSettingCount(void) const
{
	return (TextureMapProcess::GetSettingCount() + 1);
}

Setting *ParallaxProcess::GetSetting(int32 index) const
{
	int32 count = TextureMapProcess::GetSettingCount();
	if (index < count) return (TextureMapProcess::GetSetting(index));
	
	if (index == count)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		const char *title = table->GetString(StringID('PROC', kProcessParallax, 'DETL'));
		return (new BooleanSetting('DETL', ((GetProcessFlags() & kProcessHighDetail) != 0), title));
	}
	
	return (nullptr);
}

void ParallaxProcess::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'DETL')
	{
		unsigned_int32 flags = GetProcessFlags();
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) SetProcessFlags(flags | kProcessHighDetail);
		else SetProcessFlags(flags & ~kProcessHighDetail);
	}
	else
	{
		TextureMapProcess::SetSetting(setting);
	}
}

bool ParallaxProcess::ProcessEnabled(const ShaderCompileData *compileData) const
{
	if ((compileData->detailLevel > 0) && (GetProcessFlags() & kProcessHighDetail)) return (false);
	return (TheGraphicsMgr->GetRenderOptionFlags() & kRenderOptionParallaxMapping);
}

int32 ParallaxProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = TextureMapProcess::GenerateProcessSignature(compileData, signature);
	signature[count] = ProcessEnabled(compileData);
	return (count + 1);
}

int32 ParallaxProcess::GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const
{
	if (ProcessEnabled(compileData))
	{
		type[0] = kProcessTangentViewDirection;
		return (1);
	}
	
	return (0);
}

void ParallaxProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	if (ProcessEnabled(compileData))
	{
		data->registerCount = 1;
		data->temporaryCount = 1;
		
		data->textureCount = 1;
		const Texture *texture = GetTexture();
		data->textureObject[0] = texture;
		
		compileData->shaderData->AddStateFunction(&StateFunc_CalculateParallaxScale, &texture->GetParallaxScale());
	}
	else
	{
		data->passthruPort = 0;
	}
	
	data->outputSize = 2;
	data->inputSize[0] = 2;
}

int32 ParallaxProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	if (ProcessEnabled(compileData)) return (TextureMapProcess::GenerateOutputIdentifier(compileData, allocData, swizzleData, name));
	return (GetPortRoute(0)->GenerateOutputIdentifier(compileData, allocData, swizzleData, name));
}

int32 ParallaxProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	if (ProcessEnabled(compileData))
	{
		static const char code[] =
		{
			"TEX		temp, %0, %IMG0, %TRG0;\n"
			"MAD		temp, temp, 2.0, -1.0;\n"
			"MUL		tmp1.xy, temp.w, program.env[" FRAGMENT_PARAM_PARALLAX_SCALE "];\n"
			
			"DP3		temp.z, temp, vdir;\n"
			"MAX		temp.z, temp.z, 0.5;\n"
			"RCP		temp.z, temp.z;\n"
			"MUL		tmp1.xy, tmp1, temp.z;\n"
			"MAD		#, tmp1, vdir, %0;\n"
		};
		
		static const char code2[] =
		{
			"TEX		temp, %0, %IMG0, %TRG0;\n"
			"MAD		temp, temp, 2.0, -1.0;\n"
			"MUL		tmp1.xy, temp.w, program.env[" FRAGMENT_PARAM_PARALLAX_SCALE "];\n"
			
			"DP3		temp.z, temp, vdir;\n"
			"MAX		temp.z, temp.z, 0.5;\n"
			"DIV		tmp1.xy, tmp1, temp.z;\n"
			"MAD		#, tmp1, vdir, %0;\n"
		};
		
		if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
		else programCode[0] = code;
		
		return (1);
	}
	
	return (0);
}

int32 ParallaxProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	if (ProcessEnabled(compileData))
	{
		static const char code[] =
		{
			"temp = %TRG0(%IMG0, %0) * 2.0 - 1.0;\n"
			"tmp1.xy = param[" FRAGMENT_PARAM_PARALLAX_SCALE "].xy * temp.w;\n"
			"# = %0 + vdir.xy * (tmp1.xy / max(dot(temp.xyz, vdir), 0.5));\n"
		};
		
		shaderCode[0] = code;
		return (1);
	}
	
	return (0);
}

void ParallaxProcess::StateFunc_CalculateParallaxScale(const Renderable *renderable, const void *cookie)
{
	const Vector2D *parallaxScale = static_cast<const Vector2D *>(cookie);
	
	float scale = renderable->GetShaderDetailParameter() * 0.5F;
	float sx = parallaxScale->x * scale;
	float sy = parallaxScale->y * scale;
	Render::SetFragmentProgramParameter4f(kFragmentParamParallaxScale, sx, sy, 0.0F, 0.0F);
}


C4::KillProcess::KillProcess() : Process(kProcessKill)
{
	SetBaseProcessType(kProcessTerminal);
}

C4::KillProcess::KillProcess(const KillProcess& killProcess) : Process(killProcess)
{
}

C4::KillProcess::~KillProcess()
{
}

Process *C4::KillProcess::Replicate(void) const
{
	return (new KillProcess(*this));
}

int32 C4::KillProcess::GetPortCount(void) const
{
	return (2);
}

unsigned_int32 C4::KillProcess::GetPortFlags(int32 index) const
{
	return ((index == 0) ? 0 : kProcessPortOptional);
}

const char *C4::KillProcess::GetPortName(int32 index) const
{
	return ((index == 0) ? "A" : "B");
}

#if C4MACOS

	void C4::KillProcess::GenerateSourceData(const ShaderCompileData *compileData) const
	{
		// KIL instruction broken on Mac
		compileData->programFlag = false;
	}

#endif

void C4::KillProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->inputSize[0] = 1;
	data->inputSize[1] = 1;
}

int32 C4::KillProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char zeroCode[] =
	{
		"KIL		%0;\n"
	};
	
	static const char fullCode[] =
	{
		"SUB		temp.x, %0, %1;\n"
		"KIL		temp.x;\n"
	};
	
	if (GetPortRoute(1)) programCode[0] = fullCode;
	else programCode[0] = zeroCode;
	
	return (1);
}

int32 C4::KillProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char zeroCode[] =
	{
		"if (%0 < 0.0) discard;\n"
	};
	
	static const char fullCode[] =
	{
		"if (%0 < %1) discard;\n"
	};
	
	if (GetPortRoute(1)) shaderCode[0] = fullCode;
	else shaderCode[0] = zeroCode;
	
	return (1);
}


ImpostorTransitionProcess::ImpostorTransitionProcess() : Process(kProcessImpostorTransition)
{
	SetBaseProcessType(kProcessTerminal);
}

ImpostorTransitionProcess::ImpostorTransitionProcess(const ImpostorTransitionProcess& impostorTransitionProcess) : Process(impostorTransitionProcess)
{
}

ImpostorTransitionProcess::~ImpostorTransitionProcess()
{
}

Process *ImpostorTransitionProcess::Replicate(void) const
{
	return (new ImpostorTransitionProcess(*this));
}

#if C4MACOS

	void ImpostorTransitionProcess::GenerateSourceData(const ShaderCompileData *compileData) const
	{
		// KIL instruction broken on Mac
		compileData->programFlag = false;
	}

#endif

int32 ImpostorTransitionProcess::GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const
{
	type[0] = kProcessImpostorBlend;
	return (1);
}

void ImpostorTransitionProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->interpolantCount = 1;
	data->interpolantType[0] = 'IXBL';
}

int32 ImpostorTransitionProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"SUB		temp.x, $IXBL, ibld.y;\n"
		"KIL		temp.x;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 ImpostorTransitionProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"if ($IXBL < ibld.y) discard;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


GeometryTransitionProcess::GeometryTransitionProcess() : Process(kProcessGeometryTransition)
{
	SetBaseProcessType(kProcessTerminal);
	
	textureObject = Texture::Get("C4/screen");
}

GeometryTransitionProcess::GeometryTransitionProcess(const GeometryTransitionProcess& geometryTransitionProcess) : Process(geometryTransitionProcess)
{
	textureObject = geometryTransitionProcess.textureObject;
	textureObject->Retain();
}

GeometryTransitionProcess::~GeometryTransitionProcess()
{
	textureObject->Release();
}

Process *GeometryTransitionProcess::Replicate(void) const
{
	return (new GeometryTransitionProcess(*this));
}

int32 GeometryTransitionProcess::GetSettingCount(void) const
{
	return (Process::GetSettingCount() + 1);
}

Setting *GeometryTransitionProcess::GetSetting(int32 index) const
{
	int32 count = Process::GetSettingCount();
	if (index < count) return (Process::GetSetting(index));
	
	if (index == count)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		const char *title = table->GetString(StringID('PROC', kProcessGeometryTransition, 'DETL'));
		return (new BooleanSetting('DETL', ((GetProcessFlags() & kProcessLowDetail) != 0), title));
	}
	
	return (nullptr);
}

void GeometryTransitionProcess::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'DETL')
	{
		unsigned_int32 flags = GetProcessFlags();
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) SetProcessFlags(flags | kProcessLowDetail);
		else SetProcessFlags(flags & ~kProcessLowDetail);
	}
	else
	{
		Process::SetSetting(setting);
	}
}

bool GeometryTransitionProcess::ProcessEnabled(const ShaderCompileData *compileData) const
{
	return ((compileData->detailLevel > 0) || (!(GetProcessFlags() & kProcessLowDetail)));
}

#if C4MACOS

	void GeometryTransitionProcess::GenerateSourceData(const ShaderCompileData *compileData) const
	{
		// KIL instruction broken on Mac
		if (ProcessEnabled(compileData)) compileData->programFlag = false;
	}

#endif

int32 GeometryTransitionProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = Process::GenerateProcessSignature(compileData, signature);
	signature[count] = ProcessEnabled(compileData);
	return (count + 1);
}

void GeometryTransitionProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	if (ProcessEnabled(compileData))
	{
		data->interpolantCount = 1;
		data->interpolantType[0] = 'GITX';
		
		data->textureCount = 1;
		data->textureObject[0] = textureObject;
	}
}

int32 GeometryTransitionProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	if (ProcessEnabled(compileData))
	{
		static const char code[] =
		{
			"TEX		temp.x, $GITX, %IMG0, 2D;\n"
			"ADD		temp.x, temp.x, 0.25;\n"
			"SGE		temp.x, program.env[" FRAGMENT_PARAM_IMPOSTOR_DISTANCE "].x, temp.x;\n"
			"KIL		-temp.x;\n"
		};
		
		programCode[0] = code;
		return (1);
	}
	
	return (0);
}

int32 GeometryTransitionProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	if (ProcessEnabled(compileData))
	{
		static const char code[] =
		{
			"temp.x = " TEX2D "(%IMG0, $GITX).x;\n"
			"if (param[" FRAGMENT_PARAM_IMPOSTOR_DISTANCE "].x >= temp.x + 0.25) discard;\n"
		};
		
		shaderCode[0] = code;
		return (1);
	}
	
	return (0);
}


FireProcess::FireProcess() : TextureMapProcess(kProcessFire)
{
	SetTexture("C4/noise");
	
	fireParams.fireIntensity = 0.25F;
	fireParams.noiseVelocity[0].Set(0.0F, 0.0F);
	fireParams.noiseVelocity[1].Set(0.0F, 0.0F);
	fireParams.noiseVelocity[2].Set(0.0F, 0.0F);
	
	fireData = &fireParams;
}

FireProcess::FireProcess(const FireProcess& fireProcess) : TextureMapProcess(fireProcess)
{
	fireParams.fireIntensity = fireProcess.fireParams.fireIntensity;
	fireParams.noiseVelocity[0] = fireProcess.fireParams.noiseVelocity[0];
	fireParams.noiseVelocity[1] = fireProcess.fireParams.noiseVelocity[1];
	fireParams.noiseVelocity[2] = fireProcess.fireParams.noiseVelocity[2];
	
	fireData = &fireParams;
}

FireProcess::~FireProcess()
{
}

Process *FireProcess::Replicate(void) const
{
	return (new FireProcess(*this));
}

void FireProcess::Pack(Packer& data, unsigned_int32 packFlags) const
{
	TextureMapProcess::Pack(data, packFlags);
	
	data << fireParams.fireIntensity;
	data << fireParams.noiseVelocity[0];
	data << fireParams.noiseVelocity[1];
	data << fireParams.noiseVelocity[2];
}

void FireProcess::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	TextureMapProcess::Unpack(data, unpackFlags);
	
	data >> fireParams.fireIntensity;
	data >> fireParams.noiseVelocity[0];
	data >> fireParams.noiseVelocity[1];
	data >> fireParams.noiseVelocity[2];
}

int32 FireProcess::GetPortCount(void) const
{
	return (0);
}

void FireProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	if (compileData->programFlag) data->temporaryCount = 3;
	
	data->registerCount = 1;
	data->outputSize = 2;
	
	data->interpolantCount = 3;
	data->interpolantType[0] = 'FIRE';
	data->interpolantType[1] = 'FIR1';
	data->interpolantType[2] = 'FIR2';
	
	data->textureCount = 1;
	data->textureObject[0] = GetTexture();
	
	if (!(compileData->renderable->GetShaderFlags() & kShaderFireArrays)) compileData->shaderData->AddStateFunction(&StateFunc_SetFireParams, fireData);
}

int32 FireProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEX		tmp1.xy, $FIR1, %IMG0, %TRG0;\n"
		"MAD		temp.xy, tmp1, 2.0, -3.0;\n"
		"TEX		tmp2.xy, $FIR1.zwzw, %IMG0, %TRG0;\n"
		"MAD		temp.xy, tmp2, 2.0, temp;\n"
		"TEX		tmp3.xy, $FIR2, %IMG0, %TRG0;\n"
		"MAD		temp.xy, tmp3, 2.0, temp;\n"
		"MAD		#, temp, $FIRE.z, $FIRE;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 FireProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"temp.xy = %TRG0(%IMG0, $FIR1.xy).xy * 2.0 - 3.0;\n"
		"temp.xy += %TRG0(%IMG0, $FIR1.zw).xy * 2.0;\n"
		"temp.xy += %TRG0(%IMG0, $FIR2).xy * 2.0;\n"
		"# = $FIRE.xy + temp.xy * $FIRE.z;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}

void FireProcess::StateFunc_SetFireParams(const Renderable *renderable, const void *cookie)
{
	const FireAttribute::FireParams *params = static_cast<const FireAttribute::FireParams *>(cookie);
	
	const Vector2D& velocity1 = params->noiseVelocity[0];
	const Vector2D& velocity2 = params->noiseVelocity[1];
	const Vector2D& velocity3 = params->noiseVelocity[2];
	
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordVelocity0, velocity1.x, velocity1.y, velocity2.x, velocity2.y);
	Render::SetVertexProgramParameter4f(kVertexParamTexcoordVelocity1, velocity3.x, velocity3.y, 0.0F, 0.0F);
	Render::SetVertexProgramParameter4f(kVertexParamFireParams, params->fireIntensity, 0.0F, 0.0F, 0.0F);
}


DistortionProcess::DistortionProcess() : TextureMapProcess(kProcessDistortion)
{
}

DistortionProcess::DistortionProcess(const DistortionProcess& distortionProcess) : TextureMapProcess(distortionProcess)
{
}

DistortionProcess::~DistortionProcess()
{
}

Process *DistortionProcess::Replicate(void) const
{
	return (new DistortionProcess(*this));
}

void DistortionProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 4;
	data->inputSize[0] = 2;
	
	data->textureCount = 1;
	data->textureObject[0] = GetTexture();
	
	data->interpolantCount = 1;
	data->interpolantType[0] = 'DDEP';
	
	if (compileData->renderable->GetTransformable()) compileData->shaderData->AddStateFunction(&StateFunc_TransformDistortionPlane);
	else compileData->shaderData->AddStateFunction(&StateFunc_CopyDistortionPlane);
}

int32 DistortionProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEX		temp.xy, %0, %IMG0, %TRG0;\n"
		"MAD		temp.xy, temp, 2.0, -1.0;\n"
		"RCP		temp.w, $DDEP;\n"
		"MUL		temp.xy, temp, temp.w;\n"
		"MOV		#.xy, temp;\n"
		"MOV		#.zw, -temp.xxxy;\n"
	};
	
	static const char code2[] =
	{
		"TEX		temp.xy, %0, %IMG0, %TRG0;\n"
		"MAD		temp.xy, temp, 2.0, -1.0;\n"
		"DIV		temp.xy, temp, $DDEP;\n"
		"MOV		#.xy, temp;\n"
		"MOV		#.zw, -temp.xxxy;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 DistortionProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"temp.xy = %TRG0(%IMG0, %0).xy * 2.0 - 1.0;\n"
		"temp.xy /= $DDEP;\n"
		"#.xy = temp.xy;\n"
		"#.zw = -temp.xy;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}

void DistortionProcess::StateFunc_CopyDistortionPlane(const Renderable *renderable, const void *cookie)
{
	Render::SetVertexProgramParameter4fv(kVertexParamDistortCameraPlane, &TheGraphicsMgr->GetDistortionPlane().x);
}

void DistortionProcess::StateFunc_TransformDistortionPlane(const Renderable *renderable, const void *cookie)
{
	Antivector4D plane = TheGraphicsMgr->GetDistortionPlane() * renderable->GetTransformable()->GetWorldTransform();
	Render::SetVertexProgramParameter4fv(kVertexParamDistortCameraPlane, &plane.x);
}


FrameBufferProcess::FrameBufferProcess() : Process(kProcessFrameBuffer)
{
}

FrameBufferProcess::FrameBufferProcess(const FrameBufferProcess& frameBufferProcess) : Process(frameBufferProcess)
{
}

FrameBufferProcess::~FrameBufferProcess()
{
}

Process *FrameBufferProcess::Replicate(void) const
{
	return (new FrameBufferProcess(*this));
}

int32 FrameBufferProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = Process::GenerateProcessSignature(compileData, signature);
	signature[count] = 0;
	return (count + 1);
}

void FrameBufferProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->interpolantCount = 1;
	data->textureCount = 1;
	data->outputSize = 4;
	data->inputSize[0] = 2;
	data->interpolantType[0] = kProcessTexcoord0;
	data->textureObject[0] = TheGraphicsMgr->GetShadowMapTexture();
}

int32 FrameBufferProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEX		#, $TEX0, %IMG0, 2D;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 FrameBufferProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = " TEX2D "(%IMG0, $TEX0);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


OutputProcess::OutputProcess(ProcessType type) : Process(type)
{
	SetBaseProcessType(kProcessOutput);
}

OutputProcess::OutputProcess(const OutputProcess& outputProcess) : Process(outputProcess)
{
}

OutputProcess::~OutputProcess()
{
}

bool OutputProcess::ValidShader(ProcessType type, int32 shader)
{
	return (false);
}


NullOutputProcess::NullOutputProcess() : OutputProcess(kProcessNullOutput)
{
}

NullOutputProcess::NullOutputProcess(const NullOutputProcess& nullOutputProcess) : OutputProcess(nullOutputProcess)
{
}

NullOutputProcess::~NullOutputProcess()
{
}

Process *NullOutputProcess::Replicate(void) const
{
	return (new NullOutputProcess(*this));
}

int32 NullOutputProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"MOV		result.color.xyz, 0.0;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 NullOutputProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		#if C4OPENGL
		
			RESULT_COLOR ".xyz = vec3(0.0, 0.0, 0.0);\n"
		
		#else
		
			RESULT_COLOR ".xyz = 0.0;\n"
		
		#endif
	};
	
	shaderCode[0] = code;
	return (1);
}


AddOutputProcess::AddOutputProcess() : OutputProcess(kProcessAddOutput)
{
}

AddOutputProcess::AddOutputProcess(const AddOutputProcess& addOutputProcess) : OutputProcess(addOutputProcess)
{
}

AddOutputProcess::~AddOutputProcess()
{
}

Process *AddOutputProcess::Replicate(void) const
{
	return (new AddOutputProcess(*this));
}

int32 AddOutputProcess::GetPortCount(void) const
{
	return (2);
}

const char *AddOutputProcess::GetPortName(int32 index) const
{
	return ((index == 0) ? "A" : "B");
}

void AddOutputProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	if (GetFirstOutgoingEdge()) data->registerCount = 1;
	
	int32 size1 = GetPortRoute(0)->GenerateOutputSize();
	int32 size2 = GetPortRoute(1)->GenerateOutputSize();
	int32 size = Max(size1, size2);
	
	data->outputSize = size;
	data->inputSize[0] = size;
	data->inputSize[1] = size;
}

int32 AddOutputProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	if (GetFirstOutgoingEdge()) return (OutputProcess::GenerateOutputIdentifier(compileData, allocData, swizzleData, name));
	
	if (compileData->programFlag) return (Text::CopyText("result.color", name));
	return (Text::CopyText(RESULT_COLOR, name));
}

int32 AddOutputProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"ADD		#, %0, %1;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 AddOutputProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = %0 + %1;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


AmbientOutputProcess::AmbientOutputProcess() : OutputProcess(kProcessAmbientOutput)
{
}

AmbientOutputProcess::AmbientOutputProcess(const AmbientOutputProcess& ambientOutputProcess) : OutputProcess(ambientOutputProcess)
{
}

AmbientOutputProcess::~AmbientOutputProcess()
{
}

Process *AmbientOutputProcess::Replicate(void) const
{
	return (new AmbientOutputProcess(*this));
}

int32 AmbientOutputProcess::GetPortCount(void) const
{
	return (2);
}

unsigned_int32 AmbientOutputProcess::GetPortFlags(int32 index) const
{
	return ((index == 0) ? 0 : kProcessPortOptional);
}

const char *AmbientOutputProcess::GetPortName(int32 index) const
{
	return ((index == 0) ? "RGB" : "N");
}

ShaderType AmbientOutputProcess::GetAmbientShaderType(const ShaderCompileData *compileData)
{
	if (TheGraphicsMgr->GetAmbientMode() != kAmbientBright)
	{
		ShaderType type = compileData->shaderType;
		if ((type != kShaderAmbient) || (compileData->renderable->GetAmbientEnvironment()->ambientLightColor)) return (type);
	}
	
	return (kShaderNone);
}

int32 AmbientOutputProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = OutputProcess::GenerateProcessSignature(compileData, signature);
	signature[count] = GetAmbientShaderType(compileData);
	return (count + 1);
}

void AmbientOutputProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	const Renderable *renderable = compileData->renderable;
	const AmbientEnvironment *environment = renderable->GetAmbientEnvironment();
	const ColorRGBA *light = environment->ambientLightColor;
	
	switch (GetAmbientShaderType(compileData))
	{
		case kShaderAmbient:
		{
			if (GetFirstOutgoingEdge()) data->registerCount = 1;
			
			compileData->shaderData->AddStateFunction(&StateFunc_ConfigureAmbientLight, light);
			break;
		}
		
		case kShaderAmbientGradient:
		{
			if (GetFirstOutgoingEdge()) data->registerCount = 1;
			
			data->interpolantCount = 1;
			data->interpolantType[0] = 'AMGD';
			
			if (renderable->GetTransformable()) compileData->shaderData->AddStateFunction(&StateFunc_ConfigureTransformAmbientGradient, environment);
			else compileData->shaderData->AddStateFunction(&StateFunc_ConfigureAmbientGradient, environment);
			break;
		}
		
		case kShaderAmbientSpace:
		{
			if (GetFirstOutgoingEdge()) data->registerCount = 1;
			
			data->temporaryCount = 2;
			data->textureCount = 2;
			
			data->interpolantCount = 2;
			data->interpolantType[0] = 'APOS';
			data->interpolantType[1] = 'AMBT';
			
			const AmbientSpaceObject *object = environment->ambientSpaceObject;
			data->textureObject[0] = object->GetAmbientMap(0);
			data->textureObject[1] = object->GetAmbientMap(1);
			
			if (renderable->GetTransformable()) compileData->shaderData->AddStateFunction(&StateFunc_ConfigureTransformAmbientSpace, environment);
			else compileData->shaderData->AddStateFunction(&StateFunc_ConfigureAmbientSpace, environment);
			break;
		}
		
		default:
		{
			if (GetFirstOutgoingEdge()) data->passthruPort = 0;
			break;
		}
	}
	
	data->outputSize = 3;
	data->inputSize[0] = 3;
	data->inputSize[1] = 3;
}

int32 AmbientOutputProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	if (GetFirstOutgoingEdge())
	{
		if (GetAmbientShaderType(compileData) != kShaderNone) return (OutputProcess::GenerateOutputIdentifier(compileData, allocData, swizzleData, name));
		return (GetPortRoute(0)->GenerateOutputIdentifier(compileData, allocData, swizzleData, name));
	}
	
	if (compileData->programFlag) return (Text::CopyText("result.color", name));
	return (Text::CopyText(RESULT_COLOR, name));
}

int32 AmbientOutputProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	switch (GetAmbientShaderType(compileData))
	{
		case kShaderAmbient:
		{
			static const char code[] =
			{
				"MUL		#, %0, program.env[" FRAGMENT_PARAM_LIGHT_COLOR "];\n"
			};
			
			programCode[0] = code;
			return (1);
		}
		
		case kShaderAmbientGradient:
		{
			static const char code[] =
			{
				"MOV_SAT	temp.x, $AMGD;\n"
				"MAD		temp.xyz, program.env[" FRAGMENT_PARAM_AMBIENT_DELTA "], temp.x, program.env[" FRAGMENT_PARAM_LIGHT_COLOR "];\n"
				"MUL		#, %0, temp;\n"
			};
			
			programCode[0] = code;
			return (1);
		}
		
		case kShaderAmbientSpace:
		{
			static const char code[] =
			{
				"ABS		tmp2.xyz, $AMBT;\n"
				
				"MUL_SAT	temp.xyz, tmp2, $AMBT;\n"
				"TEX		tmp1.xyz, $APOS, texture["TEXTURE_UNIT_AMBIENT_SPACE1"], 3D;\n"
				"DP3		tmp1.w, temp, tmp1;\n"
				
				"MUL_SAT	temp.xyz, tmp2, -$AMBT;\n"
				"TEX		tmp2.xyz, $APOS, texture["TEXTURE_UNIT_AMBIENT_SPACE2"], 3D;\n"
				"DP3		tmp2.w, temp, tmp2;\n"
				
				"ADD		tmp1.w, tmp1.w, tmp2.w;\n"
				"MUL		tmp1.xyz, tmp1.w, program.env[" FRAGMENT_PARAM_LIGHT_COLOR "];\n"
				"MUL		#, %0, tmp1;\n"
			};
			
			static const char code2[] =
			{
				"TEX		tmp1.xyz, $APOS, texture[" TEXTURE_UNIT_AMBIENT_SPACE1 "], 3D;\n"
				"TEX		tmp2.xyz, $APOS, texture[" TEXTURE_UNIT_AMBIENT_SPACE2 "], 3D;\n"
				
				"MUL_SAT	temp.xyz, |$AMBT|, $AMBT;\n"
				"DP3		tmp1.w, temp, tmp1;\n"
				"MUL_SAT	temp.xyz, |$AMBT|, -$AMBT;\n"
				"DP3		tmp2.w, temp, tmp2;\n"
				
				"ADD		tmp1.w, tmp1.w, tmp2.w;\n"
				"MUL		tmp1.xyz, tmp1.w, program.env[" FRAGMENT_PARAM_LIGHT_COLOR "];\n"
				"MUL		#, %0, tmp1;\n"
			};
			
			if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
			else programCode[0] = code;
			
			return (1);
		}
		
		default:
		{
			if (!GetFirstOutgoingEdge())
			{
				static const char code[] =
				{
					"MOV		#, %0;\n"
				};
				
				programCode[0] = code;
				return (1);
			}
			
			break;
		}
	}
	
	return (0);
}

int32 AmbientOutputProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	switch (GetAmbientShaderType(compileData))
	{
		case kShaderAmbient:
		{
			static const char code[] =
			{
				"# = %0 * param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz;\n"
			};
			
			shaderCode[0] = code;
			return (1);
		}
		
		case kShaderAmbientGradient:
		{
			static const char code[] =
			{
				#if C4OPENGL
				
					"# = %0 * (param[" FRAGMENT_PARAM_AMBIENT_DELTA "].xyz * clamp($AMGD, 0.0, 1.0) + param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz);\n"
				
				#else
				
					"# = %0 * (param[" FRAGMENT_PARAM_AMBIENT_DELTA "].xyz * saturate($AMGD) + param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz);\n"
				
				#endif
			};
			
			shaderCode[0] = code;
			return (1);
		}
		
		case kShaderAmbientSpace:
		{
			static const char code[] =
			{
				#if C4OPENGL
				
					"tmp1.xyz = " TEX3D "(ambientTexture1, $APOS).xyz;\n"
					"tmp2.xyz = " TEX3D "(ambientTexture2, $APOS).xyz;\n"
					"tmp1.w = dot(tmp1.xyz, clamp(abs($AMBT) * $AMBT, 0.0, 1.0));\n"
					"tmp2.w = dot(tmp2.xyz, clamp(abs($AMBT) * -$AMBT, 0.0, 1.0));\n"
					"# = %0 * (tmp1.w + tmp2.w) * param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz;\n"
				
				#else
				
					"tmp1.xyz = " TEX3D "(ambientTexture1, $APOS).xyz;\n"
					"tmp2.xyz = " TEX3D "(ambientTexture2, $APOS).xyz;\n"
					"tmp1.w = dot(tmp1.xyz, saturate(abs($AMBT) * $AMBT));\n"
					"tmp2.w = dot(tmp2.xyz, saturate(abs($AMBT) * -$AMBT));\n"
					"# = %0 * (tmp1.w + tmp2.w) * param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz;\n"
				
				#endif
			};
			
			shaderCode[0] = code;
			return (1);
		}
		
		default:
		{
			if (!GetFirstOutgoingEdge())
			{
				static const char code[] =
				{
					"# = %0;\n"
				};
				
				shaderCode[0] = code;
				return (1);
			}
			
			break;
		}
	}
	
	return (0);
}

void AmbientOutputProcess::StateFunc_ConfigureAmbientLight(const Renderable *renderable, const void *cookie)
{
	Render::SetFragmentProgramParameter4fv(kFragmentParamLightColor, &static_cast<const ColorRGBA *>(cookie)->red);
}

void AmbientOutputProcess::StateFunc_ConfigureAmbientGradient(const Renderable *renderable, const void *cookie)
{
	const AmbientEnvironment *environment = static_cast<const AmbientEnvironment *>(cookie);
	
	const Portal *portal1 = environment->gradientPortal[0];
	const Portal *portal2 = environment->gradientPortal[1];
	float d = InverseMag(portal2->GetBoundingSphere()->GetCenter() - portal1->GetBoundingSphere()->GetCenter());
	
	const Antivector4D& plane = portal1->GetWorldPlane();
	Render::SetVertexProgramParameter4f(kVertexParamAmbientPlane, plane.x * d, plane.y * d, plane.z * d, plane.w * d);
	
	const ColorRGBA& color1 = *environment->ambientLightColor;
	const ColorRGBA& color2 = *environment->gradientLightColor;
	ColorRGBA delta = color2 - color1;
	
	Render::SetFragmentProgramParameter4fv(kFragmentParamLightColor, &color1.red);
	Render::SetFragmentProgramParameter4fv(kFragmentParamAmbientDelta, &delta.red);
}

void AmbientOutputProcess::StateFunc_ConfigureTransformAmbientGradient(const Renderable *renderable, const void *cookie)
{
	const AmbientEnvironment *environment = static_cast<const AmbientEnvironment *>(cookie);
	
	const Portal *portal1 = environment->gradientPortal[0];
	const Portal *portal2 = environment->gradientPortal[1];
	float d = InverseMag(portal2->GetBoundingSphere()->GetCenter() - portal1->GetBoundingSphere()->GetCenter());
	
	Antivector4D plane = portal1->GetWorldPlane() * renderable->GetTransformable()->GetWorldTransform();
	Render::SetVertexProgramParameter4f(kVertexParamAmbientPlane, plane.x * d, plane.y * d, plane.z * d, plane.w * d);
	
	const ColorRGBA& color1 = *environment->ambientLightColor;
	const ColorRGBA& color2 = *environment->gradientLightColor;
	ColorRGBA delta = color2 - color1;
	
	Render::SetFragmentProgramParameter4fv(kFragmentParamLightColor, &color1.red);
	Render::SetFragmentProgramParameter4fv(kFragmentParamAmbientDelta, &delta.red);
}

void AmbientOutputProcess::StateFunc_ConfigureAmbientSpace(const Renderable *renderable, const void *cookie)
{
	const AmbientEnvironment *environment = static_cast<const AmbientEnvironment *>(cookie);
	
	const AmbientSpaceObject *space = environment->ambientSpaceObject;
	Render::BindTexture(kTextureUnitAmbientSpace1, space->GetAmbientMap(0));
	Render::BindTexture(kTextureUnitAmbientSpace2, space->GetAmbientMap(1));
	
	const Transform4D& m = environment->ambientSpaceTransformable->GetInverseWorldTransform();
	Render::SetVertexProgramParameter4f(kVertexParamMatrixSpace, m(0,0), m(0,1), m(0,2), m(0,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixSpace + 1, m(1,0), m(1,1), m(1,2), m(1,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixSpace + 2, m(2,0), m(2,1), m(2,2), m(2,3));
	
	const Vector3D& size = space->GetBoxSize();
	Render::SetVertexProgramParameter4f(kVertexParamSpaceScale, 1.0F / size.x, 1.0F / size.y, 1.0F / size.z, 0.0F);
	
	Render::SetFragmentProgramParameter4fv(kFragmentParamLightColor, &environment->ambientLightColor->red);
}

void AmbientOutputProcess::StateFunc_ConfigureTransformAmbientSpace(const Renderable *renderable, const void *cookie)
{
	const AmbientEnvironment *environment = static_cast<const AmbientEnvironment *>(cookie);
	
	const AmbientSpaceObject *space = environment->ambientSpaceObject;
	Render::BindTexture(kTextureUnitAmbientSpace1, space->GetAmbientMap(0));
	Render::BindTexture(kTextureUnitAmbientSpace2, space->GetAmbientMap(1));
	
	Transform4D m = environment->ambientSpaceTransformable->GetInverseWorldTransform() * renderable->GetTransformable()->GetWorldTransform();
	Render::SetVertexProgramParameter4f(kVertexParamMatrixSpace, m(0,0), m(0,1), m(0,2), m(0,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixSpace + 1, m(1,0), m(1,1), m(1,2), m(1,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixSpace + 2, m(2,0), m(2,1), m(2,2), m(2,3));
	
	const Vector3D& size = space->GetBoxSize();
	Render::SetVertexProgramParameter4f(kVertexParamSpaceScale, 1.0F / size.x, 1.0F / size.y, 1.0F / size.z, 0.0F);
	
	Render::SetFragmentProgramParameter4fv(kFragmentParamLightColor, &environment->ambientLightColor->red);
}


AmbientAlphaOutputProcess::AmbientAlphaOutputProcess() : OutputProcess(kProcessAmbientAlphaOutput)
{
}

AmbientAlphaOutputProcess::AmbientAlphaOutputProcess(const AmbientAlphaOutputProcess& ambientAlphaOutputProcess) : OutputProcess(ambientAlphaOutputProcess)
{
}

AmbientAlphaOutputProcess::~AmbientAlphaOutputProcess()
{
}

Process *AmbientAlphaOutputProcess::Replicate(void) const
{
	return (new AmbientAlphaOutputProcess(*this));
}

int32 AmbientAlphaOutputProcess::GetPortCount(void) const
{
	return (1);
}

unsigned_int32 AmbientAlphaOutputProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOptional);
}

const char *AmbientAlphaOutputProcess::GetPortName(int32 index) const
{
	return ("A");
}

void AmbientAlphaOutputProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->inputSize[0] = 1;
}

int32 AmbientAlphaOutputProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"MOV		result.color.w, %0;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 AmbientAlphaOutputProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		RESULT_COLOR ".w = %0;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


AmbientOcclusionOutputProcess::AmbientOcclusionOutputProcess() : OutputProcess(kProcessAmbientOcclusionOutput)
{
}

AmbientOcclusionOutputProcess::AmbientOcclusionOutputProcess(const AmbientOcclusionOutputProcess& ambientOcclusionOutputProcess) : OutputProcess(ambientOcclusionOutputProcess)
{
}

AmbientOcclusionOutputProcess::~AmbientOcclusionOutputProcess()
{
}

Process *AmbientOcclusionOutputProcess::Replicate(void) const
{
	return (new AmbientOcclusionOutputProcess(*this));
}

int32 AmbientOcclusionOutputProcess::GetPortCount(void) const
{
	return (1);
}

const char *AmbientOcclusionOutputProcess::GetPortName(int32 index) const
{
	return ("A");
}

void AmbientOcclusionOutputProcess::GenerateSourceData(const ShaderCompileData *compileData) const
{
	compileData->shaderSourceFlags |= kShaderSourcePrimaryColor;
}

void AmbientOcclusionOutputProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	if (GetFirstOutgoingEdge()) data->registerCount = 1;
	
	int32 size = GetPortRoute(0)->GenerateOutputSize();
	data->outputSize = size;
	data->inputSize[0] = size;
}

int32 AmbientOcclusionOutputProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	if (GetFirstOutgoingEdge()) return (OutputProcess::GenerateOutputIdentifier(compileData, allocData, swizzleData, name));
	
	if (compileData->programFlag) return (Text::CopyText("result.color", name));
	return (Text::CopyText(RESULT_COLOR, name));
}

int32 AmbientOcclusionOutputProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"MUL		#, %0, fragment.color.w;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 AmbientOcclusionOutputProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = %0 * " FRAGMENT_COLOR ".w;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


EmissionOutputProcess::EmissionOutputProcess() : OutputProcess(kProcessEmissionOutput)
{
}

EmissionOutputProcess::EmissionOutputProcess(const EmissionOutputProcess& emissionOutputProcess) : OutputProcess(emissionOutputProcess)
{
}

EmissionOutputProcess::~EmissionOutputProcess()
{
}

Process *EmissionOutputProcess::Replicate(void) const
{
	return (new EmissionOutputProcess(*this));
}

int32 EmissionOutputProcess::GetPortCount(void) const
{
	return (1);
}

unsigned_int32 EmissionOutputProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOptional);
}

const char *EmissionOutputProcess::GetPortName(int32 index) const
{
	return ("RGB");
}

void EmissionOutputProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->passthruPort = 0;
	data->outputSize = 3;
	data->inputSize[0] = 3;
}

int32 EmissionOutputProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	return (GetPortRoute(0)->GenerateOutputIdentifier(compileData, allocData, swizzleData, name));
}


ReflectionOutputProcess::ReflectionOutputProcess() : OutputProcess(kProcessReflectionOutput)
{
	reflectionParams.normalIncidenceReflectivity = 1.0F;
	reflectionParams.reflectionOffsetScale = 1.0F;
	
	reflectionData = &reflectionParams;
}

ReflectionOutputProcess::ReflectionOutputProcess(const ReflectionOutputProcess& reflectionOutputProcess) : OutputProcess(reflectionOutputProcess)
{
	reflectionParams.normalIncidenceReflectivity = reflectionOutputProcess.reflectionParams.normalIncidenceReflectivity;
	reflectionParams.reflectionOffsetScale = reflectionOutputProcess.reflectionParams.reflectionOffsetScale;
	
	reflectionData = &reflectionParams;
}

ReflectionOutputProcess::~ReflectionOutputProcess()
{
}

Process *ReflectionOutputProcess::Replicate(void) const
{
	return (new ReflectionOutputProcess(*this));
}

void ReflectionOutputProcess::Pack(Packer& data, unsigned_int32 packFlags) const
{
	OutputProcess::Pack(data, packFlags);
	
	data << reflectionParams.normalIncidenceReflectivity;
	data << reflectionParams.reflectionOffsetScale;
}

void ReflectionOutputProcess::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	OutputProcess::Unpack(data, unpackFlags);
	
	data >> reflectionParams.normalIncidenceReflectivity;
	data >> reflectionParams.reflectionOffsetScale;
}

int32 ReflectionOutputProcess::GetSettingCount(void) const
{
	return (OutputProcess::GetSettingCount() + 2);
}

Setting *ReflectionOutputProcess::GetSetting(int32 index) const
{
	int32 count = OutputProcess::GetSettingCount();
	if (index < count) return (Process::GetSetting(index));
	
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (index == count)
	{
		const char *title = table->GetString(StringID('PROC', kProcessReflectionOutput, 'NINC'));
		return (new IntegerSetting('NINC', (int32) (reflectionParams.normalIncidenceReflectivity * 100.0F + 0.5F), title, 0, 100, 1));
	}
	
	if (index == count + 1)
	{
		const char *title = table->GetString(StringID('PROC', kProcessReflectionOutput, 'RFLO'));
		return (new TextSetting('RFLO', reflectionParams.reflectionOffsetScale, title));
	}
	
	return (nullptr);
}

void ReflectionOutputProcess::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'NINC')
	{
		reflectionParams.normalIncidenceReflectivity = (float) static_cast<const IntegerSetting *>(setting)->GetIntegerValue() * 0.01F;
	}
	else if (identifier == 'RFLO')
	{
		reflectionParams.reflectionOffsetScale = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
	}
	else
	{
		OutputProcess::SetSetting(setting);
	}
}

bool ReflectionOutputProcess::operator ==(const Process& process) const
{
	if (OutputProcess::operator ==(process))
	{
		const ReflectionOutputProcess& reflectionOutputProcess = static_cast<const ReflectionOutputProcess&>(process);
		if (reflectionParams.normalIncidenceReflectivity != reflectionOutputProcess.reflectionParams.normalIncidenceReflectivity) return (false);
		return (reflectionParams.reflectionOffsetScale == reflectionOutputProcess.reflectionParams.reflectionOffsetScale);
	}
	
	return (false);
}

int32 ReflectionOutputProcess::GetPortCount(void) const
{
	return (2);
}

unsigned_int32 ReflectionOutputProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOptional);
}

const char *ReflectionOutputProcess::GetPortName(int32 index) const
{
	return ((index == 0) ? "RGB" : "N");
}

void ReflectionOutputProcess::ReferenceStateParams(const Process *process)
{
	reflectionData = static_cast<const ReflectionOutputProcess *>(process)->reflectionData;
}

int32 ReflectionOutputProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = OutputProcess::GenerateProcessSignature(compileData, signature);
	signature[count] = *reinterpret_cast<const unsigned_int32 *>(&reflectionData->normalIncidenceReflectivity);
	return (count + 1);
}

int32 ReflectionOutputProcess::GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const
{
	type[0] = kProcessTangentViewDirection;
	return (1);
}

void ReflectionOutputProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->temporaryCount = 1;
	
	data->outputSize = 3;
	data->inputSize[0] = 3;
	data->inputSize[1] = 3;
	
	if (GetPortRoute(1))
	{
		data->interpolantCount = 2;
		data->interpolantType[0] = 'RGHT';
		data->interpolantType[1] = 'DOWN';
	}
	else
	{
		data->interpolantCount = 1;
		data->interpolantType[0] = 'WARP';
	}
	
	data->textureCount = 1;
	data->textureObject[0] = TheGraphicsMgr->GetReflectionTexture();
	
	float value = reflectionData->normalIncidenceReflectivity;
	
	data->literalCount = 2;
	data->literalData[0].literalType = 'NIR1';
	data->literalData[0].literalValue = value;
	data->literalData[1].literalType = 'NIR2';
	data->literalData[1].literalValue = 1.0F - value;
	
	compileData->shaderData->AddStateFunction(&StateFunc_CalculateReflectionScale, reflectionData);
}

int32 ReflectionOutputProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char flatCode[] =
	{
		"SUB		temp.z, 1.0, vdir.z;\n"
		"MUL		temp.w, temp.z, temp.z;\n"
		"MUL		temp.w, temp.w, temp.w;\n"
		"MUL		temp.w, temp.w, temp.z;\n"
		"MAD		temp.w, temp.w, &NIR2, &NIR1;\n"
		"MUL		temp.xyz, %0, temp.w;\n"
		
		"MUL		tmp1.xy, $WARP, $WARP.z;\n"
		"MIN		tmp1.xy, tmp1, 8.0;\n"
		"MAX		tmp1.xy, tmp1, -8.0;\n"
		"ADD		tmp1.xy, tmp1, fragment.position;\n"
		"TEX		tmp1.xyz, tmp1, %IMG0, RECT;\n"
		"MUL		#, temp, tmp1;\n"
	};
	
	static const char bumpCode[] =
	{
		"SWZ		temp, %1, x, y, z, -1;\n"
		"DP4		tmp1.x, $RGHT.xyzz, temp;\n"
		"DP4		tmp1.y, $DOWN.xyzz, temp;\n"
		
		"DP3		temp.z, %1, vdir;\n"
		"SUB		temp.z, 1.0, temp.z;\n"
		"MUL		temp.w, temp.z, temp.z;\n"
		"MUL		temp.w, temp.w, temp.w;\n"
		"MUL		temp.w, temp.w, temp.z;\n"
		"MAD		temp.w, temp.w, &NIR2, &NIR1;\n"
		"MUL		temp.xyz, %0, temp.w;\n"
		
		"MAD		tmp1.xy, tmp1, $RGHT.w, fragment.position;\n"
		"TEX		tmp1.xyz, tmp1, %IMG0, RECT;\n"
		"MUL		#, temp, tmp1;\n"
	};
	
	if (GetPortRoute(1)) programCode[0] = bumpCode;
	else programCode[0] = flatCode;
	
	return (1);
}

int32 ReflectionOutputProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char flatCode[] =
	{
		"temp.z = 1.0 - vdir.z;\n"
		"temp.w = temp.z * temp.z;\n"
		"temp.w = temp.w * temp.w * temp.z * &NIR2 + &NIR1;\n"
		"temp.xyz = %0 * temp.w;\n"
		
		"tmp1.xy = max(min($WARP.xy * $WARP.z, 8.0), -8.0) + " FRAGMENT_POSITION ".xy;\n"
		"# = " TEXRECT "(%IMG0, tmp1.xy).xyz * temp.xyz;\n"
	};
	
	static const char bumpCode[] =
	{
		"temp = " FLOAT4 "(%1, -1);\n"
		"tmp1.xy = " FLOAT2 "(dot($RGHT.xyzz, temp), dot($DOWN.xyzz, temp));\n"
		
		"temp.z = 1.0 - dot(%1, vdir);\n"
		"temp.w = temp.z * temp.z;\n"
		"temp.w = temp.w * temp.w * temp.z * &NIR2 + &NIR1;\n"
		"temp.xyz = %0 * temp.w;\n"
		
		"tmp1.xy = tmp1.xy * $RGHT.w + " FRAGMENT_POSITION ".xy;\n"
		"# = " TEXRECT "(%IMG0, tmp1.xy).xyz * temp.xyz;\n"
	};
	
	if (GetPortRoute(1)) shaderCode[0] = bumpCode;
	else shaderCode[0] = flatCode;
	
	return (1);
}

void ReflectionOutputProcess::StateFunc_CalculateReflectionScale(const Renderable *renderable, const void *cookie)
{
	const ReflectionAttribute::ReflectionParams *params = static_cast<const ReflectionAttribute::ReflectionParams *>(cookie);
	
	float x = params->reflectionOffsetScale * TheGraphicsMgr->GetRenderTargetOffsetSize();
	Render::SetVertexProgramParameter4f(kVertexParamReflectionScale, x, 0.0F, 0.0F, 0.0F);
}


RefractionOutputProcess::RefractionOutputProcess() : OutputProcess(kProcessRefractionOutput)
{
	refractionParams.refractionOffsetScale = 1.0F;
	
	refractionData = &refractionParams;
}

RefractionOutputProcess::RefractionOutputProcess(const RefractionOutputProcess& refractionOutputProcess) : OutputProcess(refractionOutputProcess)
{
	refractionParams.refractionOffsetScale = refractionOutputProcess.refractionParams.refractionOffsetScale;
	
	refractionData = &refractionParams;
}

RefractionOutputProcess::~RefractionOutputProcess()
{
}

Process *RefractionOutputProcess::Replicate(void) const
{
	return (new RefractionOutputProcess(*this));
}

void RefractionOutputProcess::Pack(Packer& data, unsigned_int32 packFlags) const
{
	OutputProcess::Pack(data, packFlags);
	
	data << refractionParams.refractionOffsetScale;
}

void RefractionOutputProcess::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	OutputProcess::Unpack(data, unpackFlags);
	
	data >> refractionParams.refractionOffsetScale;
}

int32 RefractionOutputProcess::GetSettingCount(void) const
{
	return (OutputProcess::GetSettingCount() + 1);
}

Setting *RefractionOutputProcess::GetSetting(int32 index) const
{
	int32 count = OutputProcess::GetSettingCount();
	if (index < count) return (Process::GetSetting(index));
	
	if (index == count)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		const char *title = table->GetString(StringID('PROC', kProcessRefractionOutput, 'RFRO'));
		return (new TextSetting('RFRO', refractionParams.refractionOffsetScale, title));
	}
	
	return (nullptr);
}

void RefractionOutputProcess::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'RFRO')
	{
		refractionParams.refractionOffsetScale = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
	}
	else
	{
		OutputProcess::SetSetting(setting);
	}
}

bool RefractionOutputProcess::operator ==(const Process& process) const
{
	if (OutputProcess::operator ==(process))
	{
		const RefractionOutputProcess& refractionOutputProcess = static_cast<const RefractionOutputProcess&>(process);
		return (refractionParams.refractionOffsetScale == refractionOutputProcess.refractionParams.refractionOffsetScale);
	}
	
	return (false);
}

int32 RefractionOutputProcess::GetPortCount(void) const
{
	return (2);
}

unsigned_int32 RefractionOutputProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOptional);
}

const char *RefractionOutputProcess::GetPortName(int32 index) const
{
	return ((index == 0) ? "RGB" : "N");
}

void RefractionOutputProcess::ReferenceStateParams(const Process *process)
{
	refractionData = static_cast<const RefractionOutputProcess *>(process)->refractionData;
}

void RefractionOutputProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->temporaryCount = 1;
	
	data->outputSize = 3;
	data->inputSize[0] = 3;
	data->inputSize[1] = 3;
	
	if (GetPortRoute(1))
	{
		data->interpolantCount = 2;
		data->interpolantType[0] = 'RGHT';
		data->interpolantType[1] = 'DOWN';
	}
	else
	{
		data->interpolantCount = 1;
		data->interpolantType[0] = 'WARP';
	}
	
	data->textureCount = 1;
	data->textureObject[0] = TheGraphicsMgr->GetRefractionTexture();
	
	compileData->shaderData->AddStateFunction(&StateFunc_CalculateRefractionParams, refractionData);
}

int32 RefractionOutputProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char flatCode[] =
	{
		"MUL		temp.xy, $WARP, $WARP.w;\n"
		"MIN		temp.xy, temp, 8.0;\n"
		"MAX		temp.xy, temp, -8.0;\n"
		"ADD		temp.xy, temp, fragment.position;\n"
		"TEX		temp.xyz, temp, %IMG0, RECT;\n"
		"MUL		#, temp, %0;\n"
	};
	
	static const char bumpCode[] =
	{
		"SWZ		temp, %1, x, y, z, -1;\n"
		"DP4		tmp1.x, $RGHT.xyzz, temp;\n"
		"DP4		tmp1.y, $DOWN.xyzz, temp;\n"
		
		"MAD		temp.xy, tmp1, $DOWN.w, fragment.position;\n"
		"TEX		temp.xyz, temp, %IMG0, RECT;\n"
		"MUL		#, temp, %0;\n"
	};
	
	if (GetPortRoute(1)) programCode[0] = bumpCode;
	else programCode[0] = flatCode;
	
	return (1);
}

int32 RefractionOutputProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char flatCode[] =
	{
		"temp.xy = max(min($WARP.xy * $WARP.w, 8.0), -8.0) + " FRAGMENT_POSITION ".xy;\n"
		"# = " TEXRECT "(%IMG0, temp.xy).xyz * %0;\n"
	};
	
	static const char bumpCode[] =
	{
		"temp = " FLOAT4 "(%1, -1);\n"
		"tmp1.xy = " FLOAT2 "(dot($RGHT.xyzz, temp), dot($DOWN.xyzz, temp));\n"
		
		"temp.xy = tmp1.xy * $DOWN.w + " FRAGMENT_POSITION ".xy;\n"
		"# = " TEXRECT "(%IMG0, temp.xy).xyz * %0;\n"
	};
	
	if (GetPortRoute(1)) shaderCode[0] = bumpCode;
	else shaderCode[0] = flatCode;
	
	return (1);
}

void RefractionOutputProcess::StateFunc_CalculateRefractionParams(const Renderable *renderable, const void *cookie)
{
	const RefractionAttribute::RefractionParams *params = static_cast<const RefractionAttribute::RefractionParams *>(cookie);
	
	float x = params->refractionOffsetScale * TheGraphicsMgr->GetRenderTargetOffsetSize();
	Render::SetVertexProgramParameter4f(kVertexParamRefractionScale, x, 0.0F, 0.0F, 0.0F);
}


EnvironmentOutputProcess::EnvironmentOutputProcess() : OutputProcess(kProcessEnvironmentOutput)
{
	textureName[0] = 0;
	textureObject = nullptr;
}

EnvironmentOutputProcess::EnvironmentOutputProcess(const EnvironmentOutputProcess& environmentOutputProcess) : OutputProcess(environmentOutputProcess)
{
	textureName = environmentOutputProcess.textureName;
	
	Texture *texture = environmentOutputProcess.textureObject;
	if (texture) texture->Retain();
	textureObject = texture;
}

EnvironmentOutputProcess::~EnvironmentOutputProcess()
{
	if (textureObject) textureObject->Release();
}

Process *EnvironmentOutputProcess::Replicate(void) const
{
	return (new EnvironmentOutputProcess(*this));
}

void EnvironmentOutputProcess::Pack(Packer& data, unsigned_int32 packFlags) const
{
	OutputProcess::Pack(data, packFlags);
	
	data << textureName;
}

void EnvironmentOutputProcess::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	OutputProcess::Unpack(data, unpackFlags);
	
	data >> textureName;
	SetTexture(textureName);
}

void *EnvironmentOutputProcess::BeginSettingsUnpack(void)
{
	if (textureObject)
	{
		textureObject->Release();
		textureObject = nullptr;
	}
	
	return (OutputProcess::BeginSettingsUnpack());
}

int32 EnvironmentOutputProcess::GetSettingCount(void) const
{
	return (OutputProcess::GetSettingCount() + 1);
}

Setting *EnvironmentOutputProcess::GetSetting(int32 index) const
{
	int32 count = OutputProcess::GetSettingCount();
	if (index < count) return (Process::GetSetting(index));
	
	if (index == count)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		const char *title = table->GetString(StringID('PROC', kProcessEnvironmentOutput, 'TNAM'));
		const char *picker = table->GetString(StringID('PROC', kProcessEnvironmentOutput, 'PICK'));
		return (new ResourceSetting('TNAM', textureName, title, picker, TextureResource::GetDescriptor()));
	}
	
	return (nullptr);
}

void EnvironmentOutputProcess::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'TNAM')
	{
		SetTexture(static_cast<const ResourceSetting *>(setting)->GetResourceName());
	}
	else
	{
		OutputProcess::SetSetting(setting);
	}
}

bool EnvironmentOutputProcess::operator ==(const Process& process) const
{
	if (OutputProcess::operator ==(process))
	{
		const EnvironmentOutputProcess& environmentOutputProcess = static_cast<const EnvironmentOutputProcess&>(process);
		return (textureName == environmentOutputProcess.textureName);
	}
	
	return (false);
}

void EnvironmentOutputProcess::SetTexture(const char *name)
{
	Texture *object = textureObject;
	
	if ((name) && (name[0] != 0))
	{
		if (name != &textureName[0]) textureName = name;
		textureObject = Texture::Get(name);
	}
	else
	{
		textureName[0] = 0;
		textureObject = nullptr;
	}
	
	if (object) object->Release();
}

int32 EnvironmentOutputProcess::GetPortCount(void) const
{
	return (2);
}

unsigned_int32 EnvironmentOutputProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOptional);
}

const char *EnvironmentOutputProcess::GetPortName(int32 index) const
{
	return ((index == 0) ? "RGB" : "N");
}

int32 EnvironmentOutputProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = OutputProcess::GenerateProcessSignature(compileData, signature);
	signature[count] = ((GetPortRoute(1)) || (compileData->renderable->TangentAvailable()));
	return (count + 1);
}

void EnvironmentOutputProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->temporaryCount = 1;
	
	data->outputSize = 3;
	data->inputSize[0] = 3;
	data->inputSize[1] = 3;
	
	if ((GetPortRoute(1)) || (compileData->renderable->TangentAvailable()))
	{
		data->interpolantCount = 4;
		data->interpolantType[0] = 'VDIR';
		data->interpolantType[1] = 'WTAN';
		data->interpolantType[2] = 'WBTN';
		data->interpolantType[3] = 'WNRM';
	}
	else
	{
		data->interpolantCount = 2;
		data->interpolantType[0] = 'NRML';
		data->interpolantType[1] = 'OVDR';
	}
	
	data->textureCount = 1;
	if (textureObject) data->textureObject[0] = textureObject;
	else data->textureObject[0] = *compileData->renderable->GetAmbientEnvironment()->environmentMap;
}

int32 EnvironmentOutputProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char tangentCode[] =
	{
		"MUL		temp.xyz, $WTAN, -$VDIR.x;\n"
		"MAD		temp.xyz, $WBTN, -$VDIR.y, temp;\n"
		"MAD		temp.xyz, $WNRM, $VDIR.z, temp;\n"
		"TEX		temp.xyz, temp, %IMG0, CUBE;\n"
		"MUL		#, temp, %0;\n"
	};
	
	static const char normalCode[] =
	{
		"DP3		temp.x, %1, $VDIR;\n"
		"MUL		temp.x, temp.x, 2.0;\n"
		"MAD		temp.xyz, %1, temp.x, -$VDIR;\n"
		
		"MUL		tmp1.xyz, $WTAN, temp.x;\n"
		"MAD		tmp1.xyz, $WBTN, temp.y, tmp1;\n"
		"MAD		tmp1.xyz, $WNRM, temp.z, tmp1;\n"
		"TEX		temp.xyz, tmp1, %IMG0, CUBE;\n"
		"MUL		#, temp, %0;\n"
	};
	
	static const char objectCode[] =
	{
		"DP3		temp.x, $NRML, $OVDR;\n"
		"MUL		temp.x, temp.x, 2.0;\n"
		"MAD		temp.xyz, $NRML, temp.x, -$OVDR;\n"
		"TEX		temp.xyz, temp, %IMG0, CUBE;\n"
		"MUL		#, temp, %0;\n"
	};
	
	if (GetPortRoute(1)) programCode[0] = normalCode;
	else if (compileData->renderable->TangentAvailable()) programCode[0] = tangentCode;
	else programCode[0] = objectCode;
	
	return (1);
}

int32 EnvironmentOutputProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char tangentCode[] =
	{
		"temp.xyz = $WTAN * -$VDIR.x + $WBTN * -$VDIR.y + $WNRM * $VDIR.z;\n"
		"# = " TEXCUBE "(%IMG0, temp.xyz).xyz * %0;\n"
	};
	
	static const char normalCode[] =
	{
		"temp.xyz = %1 * (dot(%1, $VDIR) * 2.0) - $VDIR;\n"
		"tmp1.xyz = $WTAN * temp.x + $WBTN * temp.y + $WNRM * temp.z;\n"
		"# = " TEXCUBE "(%IMG0, tmp1.xyz).xyz * %0;\n"
	};
	
	static const char objectCode[] =
	{
		"temp.xyz = $NRML * (dot($NRML, $OVDR) * 2.0) - $OVDR;\n"
		"# = " TEXCUBE "(%IMG0, temp.xyz).xyz * %0;\n"
	};
	
	if (GetPortRoute(1)) shaderCode[0] = normalCode;
	else if (compileData->renderable->TangentAvailable()) shaderCode[0] = tangentCode;
	else shaderCode[0] = objectCode;
	
	return (1);
}


TerrainEnvironmentOutputProcess::TerrainEnvironmentOutputProcess() : OutputProcess(kProcessTerrainEnvironmentOutput)
{
	textureName[0] = 0;
	textureObject = nullptr;
}

TerrainEnvironmentOutputProcess::TerrainEnvironmentOutputProcess(const TerrainEnvironmentOutputProcess& terrainEnvironmentOutputProcess) : OutputProcess(terrainEnvironmentOutputProcess)
{
	textureName = terrainEnvironmentOutputProcess.textureName;
	
	Texture *texture = terrainEnvironmentOutputProcess.textureObject;
	if (texture) texture->Retain();
	textureObject = texture;
}

TerrainEnvironmentOutputProcess::~TerrainEnvironmentOutputProcess()
{
	if (textureObject) textureObject->Release();
}

Process *TerrainEnvironmentOutputProcess::Replicate(void) const
{
	return (new TerrainEnvironmentOutputProcess(*this));
}

void TerrainEnvironmentOutputProcess::Pack(Packer& data, unsigned_int32 packFlags) const
{
	OutputProcess::Pack(data, packFlags);
	
	data << textureName;
}

void TerrainEnvironmentOutputProcess::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	OutputProcess::Unpack(data, unpackFlags);
	
	data >> textureName;
	SetTextureName(textureName);
}

void *TerrainEnvironmentOutputProcess::BeginSettingsUnpack(void)
{
	if (textureObject)
	{
		textureObject->Release();
		textureObject = nullptr;
	}
	
	return (OutputProcess::BeginSettingsUnpack());
}

int32 TerrainEnvironmentOutputProcess::GetSettingCount(void) const
{
	return (OutputProcess::GetSettingCount() + 1);
}

Setting *TerrainEnvironmentOutputProcess::GetSetting(int32 index) const
{
	int32 count = OutputProcess::GetSettingCount();
	if (index < count) return (Process::GetSetting(index));
	
	if (index == count)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		const char *title = table->GetString(StringID('PROC', kProcessTerrainEnvironmentOutput, 'TNAM'));
		const char *picker = table->GetString(StringID('PROC', kProcessTerrainEnvironmentOutput, 'PICK'));
		return (new ResourceSetting('TNAM', textureName, title, picker, TextureResource::GetDescriptor()));
	}
	
	return (nullptr);
}

void TerrainEnvironmentOutputProcess::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'TNAM')
	{
		SetTextureName(static_cast<const ResourceSetting *>(setting)->GetResourceName());
	}
	else
	{
		OutputProcess::SetSetting(setting);
	}
}

bool TerrainEnvironmentOutputProcess::operator ==(const Process& process) const
{
	if (OutputProcess::operator ==(process))
	{
		const TerrainEnvironmentOutputProcess& terrainEnvironmentOutputProcess = static_cast<const TerrainEnvironmentOutputProcess&>(process);
		return (textureName == terrainEnvironmentOutputProcess.textureName);
	}
	
	return (false);
}

void TerrainEnvironmentOutputProcess::SetTextureName(const char *name)
{
	Texture *object = textureObject;
	
	if ((name) && (name[0] != 0))
	{
		if (name != &textureName[0]) textureName = name;
		textureObject = Texture::Get(name);
	}
	else
	{
		textureName[0] = 0;
		textureObject = nullptr;
	}
	
	if (object) object->Release();
}

int32 TerrainEnvironmentOutputProcess::GetPortCount(void) const
{
	return (4);
}

unsigned_int32 TerrainEnvironmentOutputProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOptional);
}

const char *TerrainEnvironmentOutputProcess::GetPortName(int32 index) const
{
	static const char *const portName[4] =
	{
		"RGB", "N1", "N2", "N3"
	};
	
	return (portName[index]);
}

bool TerrainEnvironmentOutputProcess::BumpEnabled(void) const
{
	if (!(TheGraphicsMgr->GetRenderOptionFlags() & kRenderOptionTerrainBumps)) return (false);
	return ((GetPortRoute(1)) && (GetPortRoute(2)) && (GetPortRoute(3)));
}

int32 TerrainEnvironmentOutputProcess::GenerateDerivedInterpolantTypes(const ShaderCompileData *compileData, ProcessType *type) const
{
	type[0] = kProcessTerrainViewDirection;
	type[1] = kProcessTriplanarBlend;
	return (2);
}

void TerrainEnvironmentOutputProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->temporaryCount = 2;
	
	data->outputSize = 3;
	data->inputSize[0] = 3;
	data->inputSize[1] = 3;
	data->inputSize[2] = 3;
	data->inputSize[3] = 3;
	
	data->interpolantCount = 4;
	data->interpolantType[0] = 'TWNM';
	data->interpolantType[1] = 'TWTN';
	data->interpolantType[2] = 'TWB1';
	data->interpolantType[3] = 'TWB2';
	
	data->textureCount = 1;
	if (textureObject) data->textureObject[0] = textureObject;
	else data->textureObject[0] = *compileData->renderable->GetAmbientEnvironment()->environmentMap;
}

int32 TerrainEnvironmentOutputProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char flatCode[] =
	{
		"ADD		temp.x, tbld.x, tbld.y;\n"
		"MUL		tmp1.xyz, tvdr, temp.x;\n"
		
		"MOV		temp.z, 0.0;\n"
		"MUL		temp.xy, $TWTN, -tmp1.x;\n"
		"MAD		temp.xyz, $TWB1, -tmp1.y, temp;\n"
		"MAD		tmp1.xyz, $TWNM, tmp1.z, temp;\n"
		
		"MOV		temp.y, 0.0;\n"
		"MUL		temp.xz, $TWTN.zzww, -tvd2.x;\n"
		"MAD		temp.xyz, $TWB2, -tvd2.y, temp;\n"
		"MAD		tmp2.xyz, $TWNM, tvd2.z, temp;\n"
		
		"MAD		tmp1.xyz, tmp2, tbld.z, tmp1;\n"
		"TEX		temp.xyz, tmp1, %IMG0, CUBE;\n"
		"MUL		#, temp, %0;\n"
	};
	
	static const char bumpCode[] =
	{
		"DP3		temp.x, %1, tvdr;\n"
		"MUL		temp.x, temp.x, 2.0;\n"
		"MAD		temp.xyz, %1, temp.x, -tvdr;\n"
		
		"DP3		tmp1.x, %2, tvdr;\n"
		"MUL		tmp1.x, tmp1.x, 2.0;\n"
		"MAD		tmp1.xyz, %2, tmp1.x, -tvdr;\n"
		
		"DP3		tmp2.x, %3, tvd2;\n"
		"MUL		tmp2.x, tmp2.x, 2.0;\n"
		"MAD		tmp2.xyz, %3, tmp2.x, -tvd2;\n"
		
		"MUL		temp.xyz, temp, tbld.x;\n"
		"MAD		tmp1.xyz, tmp1, tbld.y, temp;\n"
		
		"MOV		temp.z, 0.0;\n"
		"MUL		temp.xy, $TWTN, tmp1.x;\n"
		"MAD		temp.xyz, $TWB1, tmp1.y, temp;\n"
		"MAD		tmp1.xyz, $TWNM, tmp1.z, temp;\n"
		
		"MOV		temp.y, 0.0;\n"
		"MUL		temp.xz, $TWTN.zzww, tmp2.x;\n"
		"MAD		temp.xyz, $TWB2, tmp2.y, temp;\n"
		"MAD		tmp2.xyz, $TWNM, tmp2.z, temp;\n"
		
		"MAD		tmp1.xyz, tmp2, tbld.z, tmp1;\n"
		"TEX		temp.xyz, tmp1, %IMG0, CUBE;\n"
		"MUL		#, temp, %0;\n"
	};
	
	if (BumpEnabled()) programCode[0] = bumpCode;
	else programCode[0] = flatCode;
	
	return (1);
}

int32 TerrainEnvironmentOutputProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char flatCode[] =
	{
		"tmp1.xyz = tvdr * (tbld.x + tbld.y);\n"
		
		"tmp1.xyz = " FLOAT3 "($TWTN.x, $TWTN.y, 0.0) * -tmp1.x + $TWB1 * -tmp1.y + $TWNM * tmp1.z;\n"
		"tmp2.xyz = " FLOAT3 "($TWTN.z, 0.0, $TWTN.w) * -tvd2.x + $TWB2 * -tvd2.y + $TWNM * tvd2.z;\n"
		
		"tmp1.xyz += tmp2.xyz * tbld.z;\n"
		"# = " TEXCUBE "(%IMG0, tmp1.xyz).xyz * %0;\n"
	};
	
	static const char bumpCode[] =
	{
		"temp.xyz = %1 * (dot(%1, tvdr) * 2.0) - tvdr;\n"
		"tmp1.xyz = %2 * (dot(%2, tvdr) * 2.0) - tvdr;\n"
		"tmp2.xyz = %3 * (dot(%3, tvd2) * 2.0) - tvd2;\n"
		
		"tmp1.xyz = temp.xyz * tbld.x + tmp1.xyz * tbld.y;\n"
		
		"tmp1.xyz = " FLOAT3 "($TWTN.x, $TWTN.y, 0.0) * tmp1.x + $TWB1 * tmp1.y + $TWNM * tmp1.z;\n"
		"tmp2.xyz = " FLOAT3 "($TWTN.z, 0.0, $TWTN.w) * tmp2.x + $TWB2 * tmp2.y + $TWNM * tmp2.z;\n"
		
		"tmp1.xyz += tmp2.xyz * tbld.z;\n"
		"# = " TEXCUBE "(%IMG0, tmp1.xyz).xyz * %0;\n"
	};
	
	if (BumpEnabled()) shaderCode[0] = bumpCode;
	else shaderCode[0] = flatCode;
	
	return (1);
}


GlowOutputProcess::GlowOutputProcess() : OutputProcess(kProcessGlowOutput)
{
}

GlowOutputProcess::GlowOutputProcess(const GlowOutputProcess& glowOutputProcess) : OutputProcess(glowOutputProcess)
{
}

GlowOutputProcess::~GlowOutputProcess()
{
}

Process *GlowOutputProcess::Replicate(void) const
{
	return (new GlowOutputProcess(*this));
}

int32 GlowOutputProcess::GetPortCount(void) const
{
	return (1);
}

unsigned_int32 GlowOutputProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOptional);
}

const char *GlowOutputProcess::GetPortName(int32 index) const
{
	return ("A");
}

void GlowOutputProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->inputSize[0] = 1;
	
	compileData->shaderData->blendState = (compileData->renderable->GetAmbientBlendState() & kBlendColorMask) | kBlendAlphaReplace;
}

int32 GlowOutputProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"MOV		result.color.w, %0;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 GlowOutputProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		RESULT_COLOR ".w = %0;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


ImpostorDepthOutputProcess::ImpostorDepthOutputProcess() : OutputProcess(kProcessImpostorDepthOutput)
{
}

ImpostorDepthOutputProcess::ImpostorDepthOutputProcess(const ImpostorDepthOutputProcess& impostorDepthOutputProcess) : OutputProcess(impostorDepthOutputProcess)
{
}

ImpostorDepthOutputProcess::~ImpostorDepthOutputProcess()
{
}

Process *ImpostorDepthOutputProcess::Replicate(void) const
{
	return (new ImpostorDepthOutputProcess(*this));
}

int32 ImpostorDepthOutputProcess::GetPortCount(void) const
{
	return (1);
}

unsigned_int32 ImpostorDepthOutputProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOptional);
}

const char *ImpostorDepthOutputProcess::GetPortName(int32 index) const
{
	return ("SHAD");
}

#if C4MACOS

	void ImpostorDepthOutputProcess::GenerateSourceData(const ShaderCompileData *compileData) const
	{
		// KIL instruction broken on Mac
		compileData->programFlag = false;
	}

#endif

void ImpostorDepthOutputProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->inputSize[0] = 4;
	
	data->interpolantCount = 1;
	data->interpolantType[0] = 'ISRD';
}

int32 ImpostorDepthOutputProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"DP4		temp.w, %0, program.env[" FRAGMENT_PARAM_IMPOSTOR_SHADOW_BLEND "];\n"
		"ADD		temp.w, -temp.w, 0.5;\n"
		"KIL		temp.w;\n"
		
		"MAD		temp, %0, $ISRD.x, $ISRD.y;\n"
		"DP4		temp.x, temp, program.env[" FRAGMENT_PARAM_IMPOSTOR_SHADOW_SCALE "];\n"
		"ADD		result.depth.z, fragment.position, temp.x;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 ImpostorDepthOutputProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"if (dot(%0, param[" FRAGMENT_PARAM_IMPOSTOR_SHADOW_BLEND "]) > 0.5) discard;\n"
		
		RESULT_DEPTH " = dot(%0 * $ISRD.x + $ISRD.y, param[" FRAGMENT_PARAM_IMPOSTOR_SHADOW_SCALE "]) + " FRAGMENT_POSITION ".z;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


LightOutputProcess::LightOutputProcess() : OutputProcess(kProcessLightOutput)
{
}

LightOutputProcess::LightOutputProcess(const LightOutputProcess& lightOutputProcess) : OutputProcess(lightOutputProcess)
{
}

LightOutputProcess::~LightOutputProcess()
{
}

Process *LightOutputProcess::Replicate(void) const
{
	return (new LightOutputProcess(*this));
}

int32 LightOutputProcess::GetPortCount(void) const
{
	return (2);
}

unsigned_int32 LightOutputProcess::GetPortFlags(int32 index) const
{
	return ((index == 0) ? 0 : kProcessPortOptional);
}

const char *LightOutputProcess::GetPortName(int32 index) const
{
	return ((index == 0) ? "RGB" : "Z");
}

ShaderType LightOutputProcess::GetLightShaderType(const ShaderCompileData *compileData)
{
	ShaderType type = compileData->shaderType;
	if ((type == kShaderCubeLight) && (compileData->renderable->GetShaderFlags() & kShaderCubeLightInhibit)) type = kShaderPointLight;
	return (type);
}

#if C4OPENGL

	void LightOutputProcess::GenerateSourceData(const ShaderCompileData *compileData) const
	{
		ShaderType type = GetLightShaderType(compileData);
		if (((type == kShaderDepthLight) || (type == kShaderLandscapeLight)) && (!TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgramShadow])) compileData->programFlag = false;
	}

#endif

int32 LightOutputProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = OutputProcess::GenerateProcessSignature(compileData, signature);
	signature[count] = GetLightShaderType(compileData);
	return (count + 1);
}

void LightOutputProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	switch (GetLightShaderType(compileData))
	{
		case kShaderDepthLight:
			
			data->interpolantType[0] = 'SHAD';
			data->interpolantType[1] = 'SHDZ';
			
			if (!GetPortRoute(1))
			{
				data->interpolantCount = 2;
			}
			else
			{
				data->interpolantCount = 3;
				data->interpolantType[2] = 'IRAD';
			}
			
			break;
		
		case kShaderLandscapeLight:
			
			data->interpolantType[0] = 'LAND';
			data->interpolantType[1] = 'SECT';
			
			if (!GetPortRoute(1))
			{
				data->interpolantCount = 2;
			}
			else
			{
				data->interpolantCount = 3;
				data->interpolantType[2] = 'IRAD';
			}
			
			break;
		
		case kShaderPointLight:
			
			data->interpolantCount = 1;
			data->interpolantType[0] = 'ATTN';
			break;
		
		case kShaderCubeLight:
		case kShaderSpotLight:
			
			data->interpolantCount = 2;
			data->interpolantType[0] = 'PROJ';
			data->interpolantType[1] = 'ATTN';
			break;
	}
	
	if (GetFirstOutgoingEdge()) data->registerCount = 1;
	if (GetPortRoute(1)) data->temporaryCount = 1;
	
	data->outputSize = 3;
	data->inputSize[0] = 3;
	data->inputSize[1] = 1;
}

int32 LightOutputProcess::GenerateOutputIdentifier(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, SwizzleData *swizzleData, char *name) const
{
	const Route *route = GetFirstOutgoingEdge();
	if ((route) && (route->GetRoutePort() != kProcessPortHiddenDependency)) return (OutputProcess::GenerateOutputIdentifier(compileData, allocData, swizzleData, name));
	
	if (compileData->programFlag) return (Text::CopyText("result.color", name));
	return (Text::CopyText(RESULT_COLOR, name));
}

int32 LightOutputProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	switch (GetLightShaderType(compileData))
	{
		case kShaderDepthLight:
		{
			static const char code[] =
			{
				"TEMP		lshd;\n"
				
				"MOV		temp.z, $SHDZ;\n"
				
				"ADD		temp.xy, $SHAD, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].xyxy;\n"
				"TEX		lshd.x, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"ADD		temp.xy, $SHAD, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].zwzw;\n"
				"TEX		lshd.y, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"ADD		temp.xy, $SHAD, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].xyxy;\n"
				"TEX		lshd.z, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"ADD		temp.xy, $SHAD, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].zwzw;\n"
				"TEX		lshd.w, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				
				"DP4		lshd.w, lshd, 0.25;\n"
				"MUL		temp.xyz, %0, program.env[" FRAGMENT_PARAM_LIGHT_COLOR "];\n"
				"MUL		#, temp, lshd.w;\n"
			};
			
			static const char impostorCode[] =
			{
				"TEMP		lshd;\n"
				
				"MAD		temp.w, %1, $IRAD.x, $IRAD.y;\n"
				"MAD		temp.z, program.env[" FRAGMENT_PARAM_SHADOW_VIEW_DIRECTION "].z, temp.w, $SHDZ;\n"
				"MAD		tmp1.xy, program.env[" FRAGMENT_PARAM_SHADOW_VIEW_DIRECTION "], temp.w, $SHAD;\n"
				
				"ADD		temp.xy, tmp1, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].xyxy;\n"
				"TEX		lshd.x, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"ADD		temp.xy, tmp1, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].zwzw;\n"
				"TEX		lshd.y, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"ADD		temp.xy, tmp1, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].xyxy;\n"
				"TEX		lshd.z, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"ADD		temp.xy, tmp1, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].zwzw;\n"
				"TEX		lshd.w, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				
				"DP4		lshd.w, lshd, 0.25;\n"
				"MUL		temp.xyz, %0, program.env[" FRAGMENT_PARAM_LIGHT_COLOR "];\n"
				"MUL		#, temp, lshd.w;\n"
			};
			
			if (!GetPortRoute(1)) programCode[0] = code;
			else programCode[0] = impostorCode;
			
			return (1);
		}
		
		case kShaderLandscapeLight:
		{
			static const char code[] =
			{
				"TEMP		wght, lnd1, lnd2, lnd3, lshd, lsh2;\n"
				
				"MOV_SAT	wght.xyz, $SECT;\n"
				"MOV		wght.w, 1.0;\n"
				"SUB		wght.xy, wght.ywww, wght.zxxx;\n"	// wght.xy = (w2, w0)
				
				"MAD		lshd.xyz, $LAND, program.env[" FRAGMENT_PARAM_SHADOW_MAP_SCALE1 "], program.env[" FRAGMENT_PARAM_SHADOW_MAP_OFFSET1 "];\n"
				"MAD		lnd2.xyz, $LAND, program.env[" FRAGMENT_PARAM_SHADOW_MAP_SCALE2 "], program.env[" FRAGMENT_PARAM_SHADOW_MAP_OFFSET2 "];\n"
				"MAD		lnd3.xyz, $LAND, program.env[" FRAGMENT_PARAM_SHADOW_MAP_SCALE3 "], program.env[" FRAGMENT_PARAM_SHADOW_MAP_OFFSET3 "];\n"
				
				"SGE		temp.xy, $SECT.yzzz, 0.0;\n"
				"LRP		lnd1.xyz, temp.x, lnd2, $LAND;\n"
				"LRP		lnd2.xyz, temp.y, lnd3, lshd;\n"
				"LRP		wght.x, temp.x, wght.x, wght.y;\n"
				
				"MOV		temp.z, lnd1.z;\n"
				"ADD		temp.xy, lnd1, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].xyxy;\n"
				"TEX		lshd.x, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"ADD		temp.xy, lnd1, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].zwzw;\n"
				"TEX		lshd.y, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"ADD		temp.xy, lnd1, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].xyxy;\n"
				"TEX		lshd.z, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"ADD		temp.xy, lnd1, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].zwzw;\n"
				"TEX		lshd.w, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"DP4		lshd.w, lshd, 0.25;\n"
				
				"MOV		temp.z, lnd2.z;\n"
				"ADD		temp.xy, lnd2, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].xyxy;\n"
				"TEX		lsh2.x, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"ADD		temp.xy, lnd2, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].zwzw;\n"
				"TEX		lsh2.y, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"ADD		temp.xy, lnd2, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].xyxy;\n"
				"TEX		lsh2.z, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"ADD		temp.xy, lnd2, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].zwzw;\n"
				"TEX		lsh2.w, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"DP4		lsh2.w, lsh2, 0.25;\n"
				
				"LRP		lshd.w, wght.x, lshd.w, lsh2.w;\n"
				
				"MUL		temp.xyz, %0, program.env[" FRAGMENT_PARAM_LIGHT_COLOR "];\n"
				"MUL		#, temp, lshd.w;\n"
			};
			
			static const char impostorCode[] =
			{
				"TEMP		wght, lnd1, lnd2, lnd3, lshd, lsh2;\n"
				
				"MOV_SAT	wght.xyz, $SECT;\n"
				"MOV		wght.w, 1.0;\n"
				"SUB		wght.xy, wght.ywww, wght.zxxx;\n"	// wght.xy = (w2, w0)
				
				"MAD		temp.w, %1, $IRAD.x, $IRAD.y;\n"
				"MAD		tmp1.xyz, program.env[" FRAGMENT_PARAM_SHADOW_VIEW_DIRECTION "], temp.w, $LAND;\n"
				
				"MAD		lshd.xyz, tmp1, program.env[" FRAGMENT_PARAM_SHADOW_MAP_SCALE1 "], program.env[" FRAGMENT_PARAM_SHADOW_MAP_OFFSET1 "];\n"
				"MAD		lnd2.xyz, tmp1, program.env[" FRAGMENT_PARAM_SHADOW_MAP_SCALE2 "], program.env[" FRAGMENT_PARAM_SHADOW_MAP_OFFSET2 "];\n"
				"MAD		lnd3.xyz, tmp1, program.env[" FRAGMENT_PARAM_SHADOW_MAP_SCALE3 "], program.env[" FRAGMENT_PARAM_SHADOW_MAP_OFFSET3 "];\n"
				
				"SGE		temp.xy, $SECT.yzzz, 0.0;\n"
				"LRP		lnd1.xyz, temp.x, lnd2, tmp1;\n"
				"LRP		lnd2.xyz, temp.y, lnd3, lshd;\n"
				"LRP		wght.x, temp.x, wght.x, wght.y;\n"
				
				"MOV		temp.z, lnd1.z;\n"
				"ADD		temp.xy, lnd1, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].xyxy;\n"
				"TEX		lshd.x, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"ADD		temp.xy, lnd1, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].zwzw;\n"
				"TEX		lshd.y, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"ADD		temp.xy, lnd1, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].xyxy;\n"
				"TEX		lshd.z, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"ADD		temp.xy, lnd1, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].zwzw;\n"
				"TEX		lshd.w, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"DP4		lshd.w, lshd, 0.25;\n"
				
				"MOV		temp.z, lnd2.z;\n"
				"ADD		temp.xy, lnd2, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].xyxy;\n"
				"TEX		lsh2.x, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"ADD		temp.xy, lnd2, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].zwzw;\n"
				"TEX		lsh2.y, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"ADD		temp.xy, lnd2, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].xyxy;\n"
				"TEX		lsh2.z, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"ADD		temp.xy, lnd2, program.env[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].zwzw;\n"
				"TEX		lsh2.w, temp, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], SHADOW2D;\n"
				"DP4		lsh2.w, lsh2, 0.25;\n"
				
				"LRP		lshd.w, wght.x, lshd.w, lsh2.w;\n"
				
				"MUL		temp.xyz, %0, program.env[" FRAGMENT_PARAM_LIGHT_COLOR "];\n"
				"MUL		#, temp, lshd.w;\n"
			};
			
			if (!GetPortRoute(1)) programCode[0] = code;
			else programCode[0] = impostorCode;
			
			return (1);
		}
		
		case kShaderPointLight:
		{
			static const char code[] =
			{
				"TEMP		lshd;\n"
				
				"DP3		lshd.w, $ATTN, $ATTN;\n"
				"MUL		lshd.w, lshd.w, -5.77078;\n"
				"EX2		lshd.w, lshd.w;\n"
				"MAD_SAT	lshd.w, lshd.w, 1.01865736, -0.01865736;\n"
				
				"MUL		lshd.xyz, lshd.w, program.env[" FRAGMENT_PARAM_LIGHT_COLOR "];\n"
				"MUL		#, %0, lshd;\n"
			};
			
			programCode[0] = code;
			return (1);
		}
		
		case kShaderCubeLight:
		{
			static const char code[] =
			{
				"TEMP		lshd;\n"
				
				"DP3		temp.x, $ATTN, $ATTN;\n"
				"MUL		temp.x, temp.x, -5.77078;\n"
				"EX2		temp.x, temp.x;\n"
				"MAD_SAT	temp.x, temp.x, 1.01865736, -0.01865736;\n"
				
				"TEX		lshd, $PROJ, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], CUBE;\n"
				"MUL		lshd, lshd, temp.x;\n"
				"MUL		lshd.xyz, lshd, program.env[" FRAGMENT_PARAM_LIGHT_COLOR "];\n"
				"MUL		#, %0, lshd;\n"
			};
			
			programCode[0] = code;
			return (1);
		}
		
		case kShaderSpotLight:
		{
			static const char code[] =
			{
				"TEMP		lshd;\n"
				
				"DP3		temp.x, $ATTN, $ATTN;\n"
				"MUL		temp.x, temp.x, -5.77078;\n"
				"EX2		temp.x, temp.x;\n"
				"MAD_SAT	temp.x, temp.x, 1.01865736, -0.01865736;\n"
				"SGE		temp.w, $ATTN.z, 0.0;\n"
				"MUL		temp.x, temp.x, temp.w;\n"
				
				"TXP		lshd, $PROJ, texture[" TEXTURE_UNIT_LIGHT_PROJECTION "], 2D;\n"
				"MUL		lshd, lshd, temp.x;\n"
				"MUL		lshd.xyz, lshd, program.env[" FRAGMENT_PARAM_LIGHT_COLOR "];\n"
				"MUL		#, %0, lshd;\n"
			};
			
			programCode[0] = code;
			return (1);
		}
		
		default:
		{
			static const char code[] =
			{
				"MUL		#, %0, program.env[" FRAGMENT_PARAM_LIGHT_COLOR "];\n"
			};
			
			programCode[0] = code;
			return (1);
		}
	}
}

int32 LightOutputProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	switch (GetLightShaderType(compileData))
	{
		case kShaderDepthLight:
		{
			static const char code[] =
			{
				#if C4OPENGL
				
					"vec4 lshd;\n"
					
					"temp.z = $SHDZ;\n"
					
					"temp.xy = $SHAD + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].xy;\n"
					"lshd.x = shadow2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = $SHAD + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].zw;\n"
					"lshd.y = shadow2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = $SHAD + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].xy;\n"
					"lshd.z = shadow2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = $SHAD + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].zw;\n"
					"lshd.w = shadow2D(shadowTexture, temp.xyz).x;\n"
					
					"lshd.w = dot(lshd, vec4(0.25, 0.25, 0.25, 0.25));\n"
					"# = %0 * param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz * lshd.w;\n"
				
				#else
				
					"float4 lshd;\n"
					
					"temp.z = $SHDZ;\n"
					
					"temp.xy = $SHAD + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].xy;\n"
					"lshd.x = tex2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = $SHAD + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].zw;\n"
					"lshd.y = tex2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = $SHAD + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].xy;\n"
					"lshd.z = tex2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = $SHAD + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].zw;\n"
					"lshd.w = tex2D(shadowTexture, temp.xyz).x;\n"
					
					"lshd.w = dot(lshd, 0.25);\n"
					"# = %0 * param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz * lshd.w;\n"
				
				#endif
			};
			
			static const char impostorCode[] =
			{
				#if C4OPENGL
				
					"vec4 lshd;\n"
					
					"temp.w = %1 * $IRAD.x + $IRAD.y;\n"
					"temp.z = param[" FRAGMENT_PARAM_SHADOW_VIEW_DIRECTION "].z * temp.w + $SHDZ;\n"
					"tmp1.xy = param[" FRAGMENT_PARAM_SHADOW_VIEW_DIRECTION "].xy * temp.w + $SHAD;\n"
					
					"temp.xy = tmp1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].xy;\n"
					"lshd.x = shadow2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = tmp1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].zw;\n"
					"lshd.y = shadow2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = tmp1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].xy;\n"
					"lshd.z = shadow2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = tmp1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].zw;\n"
					"lshd.w = shadow2D(shadowTexture, temp.xyz).x;\n"
					
					"lshd.w = dot(lshd, vec4(0.25, 0.25, 0.25, 0.25));\n"
					"# = %0 * param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz * lshd.w;\n"
				
				#else
				
					"float4 lshd;\n"
					
					"temp.w = %1 * $IRAD.x + $IRAD.y;\n"
					"temp.z = param[" FRAGMENT_PARAM_SHADOW_VIEW_DIRECTION "].z * temp.w + $SHDZ;\n"
					"tmp1.xy = param[" FRAGMENT_PARAM_SHADOW_VIEW_DIRECTION "].xy * temp.w + $SHAD;\n"
					
					"temp.xy = tmp1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].xy;\n"
					"lshd.x = tex2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = tmp1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].zw;\n"
					"lshd.y = tex2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = tmp1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].xy;\n"
					"lshd.z = tex2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = tmp1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].zw;\n"
					"lshd.w = tex2D(shadowTexture, temp.xyz).x;\n"
					
					"lshd.w = dot(lshd, 0.25);\n"
					"# = %0 * param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz * lshd.w;\n"
				
				#endif
			};
			
			if (!GetPortRoute(1)) shaderCode[0] = code;
			else shaderCode[0] = impostorCode;
			
			return (1);
		}
		
		case kShaderLandscapeLight:
		{
			static const char code[] =
			{
				#if C4OPENGL
				
					"vec4 wght, lnd1, lnd2, lnd3, lshd, lsh2;\n"
					
					"wght.xyz = clamp($SECT, 0.0, 1.0);\n"
					"wght.w = 1.0;\n"
					"wght.xy = wght.yw - wght.zx;\n"
					
					"lshd.xyz = $LAND * param[" FRAGMENT_PARAM_SHADOW_MAP_SCALE1 "].xyz + param[" FRAGMENT_PARAM_SHADOW_MAP_OFFSET1 "].xyz;\n"
					"lnd2.xyz = $LAND * param[" FRAGMENT_PARAM_SHADOW_MAP_SCALE2 "].xyz + param[" FRAGMENT_PARAM_SHADOW_MAP_OFFSET2 "].xyz;\n"
					"lnd3.xyz = $LAND * param[" FRAGMENT_PARAM_SHADOW_MAP_SCALE3 "].xyz + param[" FRAGMENT_PARAM_SHADOW_MAP_OFFSET3 "].xyz;\n"
					
					"temp.xy = vec2($SECT.y >= 0.0, $SECT.z >= 0.0);\n"
					"lnd1.xyz = mix($LAND, lnd2.xyz, temp.x);\n"
					"lnd2.xyz = mix(lshd.xyz, lnd3.xyz, temp.y);\n"
					"wght.x = mix(wght.y, wght.x, temp.x);\n"
					
					"temp.z = lnd1.z;\n"
					"temp.xy = lnd1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].xy;\n"
					"lshd.x = shadow2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].zw;\n"
					"lshd.y = shadow2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].xy;\n"
					"lshd.z = shadow2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].zw;\n"
					"lshd.w = shadow2D(shadowTexture, temp.xyz).x;\n"
					"lshd.w = dot(lshd, vec4(0.25, 0.25, 0.25, 0.25));\n"
					
					"temp.z = lnd2.z;\n"
					"temp.xy = lnd2.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].xy;\n"
					"lsh2.x = shadow2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd2.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].zw;\n"
					"lsh2.y = shadow2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd2.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].xy;\n"
					"lsh2.z = shadow2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd2.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].zw;\n"
					"lsh2.w = shadow2D(shadowTexture, temp.xyz).x;\n"
					"lsh2.w = dot(lsh2, vec4(0.25, 0.25, 0.25, 0.25));\n"
					
					"lshd.w = mix(lsh2.w, lshd.w, wght.x);\n"
					"# = %0 * param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz * lshd.w;\n"
				
				#else
				
					"float4 wght, lnd1, lnd2, lnd3, lshd, lsh2;\n"
					
					"wght.xyz = saturate($SECT);\n"
					"wght.w = 1.0;\n"
					"wght.xy = wght.yw - wght.zx;\n"
					
					"lshd.xyz = $LAND * param[" FRAGMENT_PARAM_SHADOW_MAP_SCALE1 "].xyz + param[" FRAGMENT_PARAM_SHADOW_MAP_OFFSET1 "].xyz;\n"
					"lnd2.xyz = $LAND * param[" FRAGMENT_PARAM_SHADOW_MAP_SCALE2 "].xyz + param[" FRAGMENT_PARAM_SHADOW_MAP_OFFSET2 "].xyz;\n"
					"lnd3.xyz = $LAND * param[" FRAGMENT_PARAM_SHADOW_MAP_SCALE3 "].xyz + param[" FRAGMENT_PARAM_SHADOW_MAP_OFFSET3 "].xyz;\n"
					
					"temp.xy = ($SECT.yz >= 0.0);\n"
					"lnd1.xyz = lerp($LAND, lnd2.xyz, temp.x);\n"
					"lnd2.xyz = lerp(lshd.xyz, lnd3.xyz, temp.y);\n"
					"wght.x = lerp(wght.y, wght.x, temp.x);\n"
					
					"temp.z = lnd1.z;\n"
					"temp.xy = lnd1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].xy;\n"
					"lshd.x = tex2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].zw;\n"
					"lshd.y = tex2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].xy;\n"
					"lshd.z = tex2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].zw;\n"
					"lshd.w = tex2D(shadowTexture, temp.xyz).x;\n"
					"lshd.w = dot(lshd, 0.25);\n"
					
					"temp.z = lnd2.z;\n"
					"temp.xy = lnd2.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].xy;\n"
					"lsh2.x = tex2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd2.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].zw;\n"
					"lsh2.y = tex2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd2.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].xy;\n"
					"lsh2.z = tex2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd2.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].zw;\n"
					"lsh2.w = tex2D(shadowTexture, temp.xyz).x;\n"
					"lsh2.w = dot(lsh2, 0.25);\n"
					
					"lshd.w = lerp(lsh2.w, lshd.w, wght.x);\n"
					"# = %0 * param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz * lshd.w;\n"
				
				#endif
			};
			
			static const char impostorCode[] =
			{
				#if C4OPENGL
				
					"vec4 wght, lnd1, lnd2, lnd3, lshd, lsh2;\n"
					
					"wght.xyz = clamp($SECT, 0.0, 1.0);\n"
					"wght.w = 1.0;\n"
					"wght.xyw -= wght.yzx;\n"
					
					"temp.w = %1 * $IRAD.x + $IRAD.y;\n"
					"tmp1.xyz = param[" FRAGMENT_PARAM_SHADOW_VIEW_DIRECTION "].xyz * temp.w + $LAND;\n"
					
					"lshd.xyz = tmp1.xyz * param[" FRAGMENT_PARAM_SHADOW_MAP_SCALE1 "].xyz + param[" FRAGMENT_PARAM_SHADOW_MAP_OFFSET1 "].xyz;\n"
					"lnd2.xyz = tmp1.xyz * param[" FRAGMENT_PARAM_SHADOW_MAP_SCALE2 "].xyz + param[" FRAGMENT_PARAM_SHADOW_MAP_OFFSET2 "].xyz;\n"
					"lnd3.xyz = tmp1.xyz * param[" FRAGMENT_PARAM_SHADOW_MAP_SCALE3 "].xyz + param[" FRAGMENT_PARAM_SHADOW_MAP_OFFSET3 "].xyz;\n"
					
					"temp.xy = vec2($SECT.y >= 0.0, $SECT.z >= 0.0);\n"
					"lnd1.xyz = mix($LAND, lnd2.xyz, temp.x);\n"
					"lnd2.xyz = mix(lshd.xyz, lnd3.xyz, temp.y);\n"
					"wght.w = mix(wght.w, wght.y, temp.x);\n"
					
					"temp.z = lnd1.z;\n"
					"temp.xy = lnd1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].xy;\n"
					"lshd.x = shadow2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].zw;\n"
					"lshd.y = shadow2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].xy;\n"
					"lshd.z = shadow2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].zw;\n"
					"lshd.w = shadow2D(shadowTexture, temp.xyz).x;\n"
					"lshd.w = dot(lshd, vec4(0.25, 0.25, 0.25, 0.25));\n"
					
					"temp.z = lnd2.z;\n"
					"temp.xy = lnd2.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].xy;\n"
					"lsh2.x = shadow2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd2.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].zw;\n"
					"lsh2.y = shadow2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd2.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].xy;\n"
					"lsh2.z = shadow2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd2.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].zw;\n"
					"lsh2.w = shadow2D(shadowTexture, temp.xyz).x;\n"
					"lsh2.w = dot(lsh2, vec4(0.25, 0.25, 0.25, 0.25));\n"
					
					"lshd.w = mix(lsh2.w, lshd.w, wght.w);\n"
					"# = %0 * param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz * lshd.w;\n"
				
				#else
				
					"float4 wght, lnd1, lnd2, lnd3, lshd, lsh2;\n"
					
					"wght.xyz = saturate($SECT);\n"
					"wght.w = 1.0;\n"
					"wght.xyw -= wght.yzx;\n"
					
					"temp.w = %1 * $IRAD.x + $IRAD.y;\n"
					"tmp1.xyz = param[" FRAGMENT_PARAM_SHADOW_VIEW_DIRECTION "].xyz * temp.w + $LAND;\n"
					
					"lshd.xyz = $LAND * param[" FRAGMENT_PARAM_SHADOW_MAP_SCALE1 "].xyz + param[" FRAGMENT_PARAM_SHADOW_MAP_OFFSET1 "].xyz;\n"
					"lnd2.xyz = $LAND * param[" FRAGMENT_PARAM_SHADOW_MAP_SCALE2 "].xyz + param[" FRAGMENT_PARAM_SHADOW_MAP_OFFSET2 "].xyz;\n"
					"lnd3.xyz = $LAND * param[" FRAGMENT_PARAM_SHADOW_MAP_SCALE3 "].xyz + param[" FRAGMENT_PARAM_SHADOW_MAP_OFFSET3 "].xyz;\n"
					
					"temp.xy = ($SECT.yz >= 0.0);\n"
					"lnd1.xyz = lerp($LAND, lnd2.xyz, temp.x);\n"
					"lnd2.xyz = lerp(lshd.xyz, lnd3.xyz, temp.y);\n"
					"wght.w = lerp(wght.w, wght.y, temp.x);\n"
					
					"temp.z = lnd1.z;\n"
					"temp.xy = lnd1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].xy;\n"
					"lshd.x = tex2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].zw;\n"
					"lshd.y = tex2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].xy;\n"
					"lshd.z = tex2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd1.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].zw;\n"
					"lshd.w = tex2D(shadowTexture, temp.xyz).x;\n"
					"lshd.w = dot(lshd, 0.25);\n"
					
					"temp.z = lnd2.z;\n"
					"temp.xy = lnd2.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].xy;\n"
					"lsh2.x = tex2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd2.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE1 "].zw;\n"
					"lsh2.y = tex2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd2.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].xy;\n"
					"lsh2.z = tex2D(shadowTexture, temp.xyz).x;\n"
					"temp.xy = lnd2.xy + param[" FRAGMENT_PARAM_SHADOW_SAMPLE2 "].zw;\n"
					"lsh2.w = tex2D(shadowTexture, temp.xyz).x;\n"
					"lsh2.w = dot(lsh2, 0.25);\n"
					
					"lshd.w = lerp(lsh2.w, lshd.w, wght.w);\n"
					"# = %0 * param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz * lshd.w;\n"
				
				#endif
			};
			
			if (!GetPortRoute(1)) shaderCode[0] = code;
			else shaderCode[0] = impostorCode;
			
			return (1);
		}
		
		case kShaderPointLight:
		{
			static const char code[] =
			{
				#if C4OPENGL
				
					"vec4 lshd;\n"
					
					"lshd.w = clamp(exp2(dot($ATTN, $ATTN) * -5.77078) * 1.01865736 - 0.01865736, 0.0, 1.0);\n"
					"lshd.xyz = param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz * lshd.w;\n"
					"# = %0 * lshd.xyz;\n"
				
				#else
				
					"float4 lshd;\n"
					
					"lshd.w = saturate(exp2(dot($ATTN, $ATTN) * -5.77078) * 1.01865736 - 0.01865736);\n"
					"lshd.xyz = param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz * lshd.w;\n"
					"# = %0 * lshd.xyz;\n"
				
				#endif
			};
			
			shaderCode[0] = code;
			return (1);
		}
		
		case kShaderCubeLight:
		{
			static const char code[] =
			{
				#if C4OPENGL
				
					"temp.x = clamp(exp2(dot($ATTN, $ATTN) * -5.77078) * 1.01865736 - 0.01865736, 0.0, 1.0);\n"
					"vec4 lshd = textureCube(projectionCUBE, $PROJ.xyz) * temp.x;\n"
					"lshd.xyz *= param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz;\n"
					"# = %0 * lshd.xyz;\n"
				
				#else
				
					"temp.x = saturate(exp2(dot($ATTN, $ATTN) * -5.77078) * 1.01865736 - 0.01865736);\n"
					"float4 lshd = " TEXCUBE "(projectionCUBE, $PROJ.xyz) * temp.x;\n"
					"lshd.xyz *= param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz;\n"
					"# = %0 * lshd.xyz;\n"
				
				#endif
			};
			
			shaderCode[0] = code;
			return (1);
		}
		
		case kShaderSpotLight:
		{
			static const char code[] =
			{
				#if C4OPENGL
				
					"temp.x = clamp(exp2(dot($ATTN, $ATTN) * -5.77078) * 1.01865736 - 0.01865736, 0.0, 1.0) * float($ATTN.z >= 0.0);\n"
					"vec4 lshd = texture2DProj(projection2D, $PROJ.xyw) * temp.x;\n"
					"lshd.xyz *= param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz;\n"
					"# = %0 * lshd.xyz;\n"
				
				#else
				
					"temp.x = saturate(exp2(dot($ATTN, $ATTN) * -5.77078) * 1.01865736 - 0.01865736) * ($ATTN.z >= 0.0);\n"
					"float4 lshd = tex2Dproj(projection2D, $PROJ.xyw) * temp.x;\n"
					"lshd.xyz *= param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz;\n"
					"# = %0 * lshd.xyz;\n"
				
				#endif
			};
			
			shaderCode[0] = code;
			return (1);
		}
		
		default:
		{
			static const char code[] =
			{
				"# = %0 * param[" FRAGMENT_PARAM_LIGHT_COLOR "].xyz;\n"
			};
			
			shaderCode[0] = code;
			return (1);
		}
	}
}


BloomOutputProcess::BloomOutputProcess() : OutputProcess(kProcessBloomOutput)
{
}

BloomOutputProcess::BloomOutputProcess(const BloomOutputProcess& bloomOutputProcess) : OutputProcess(bloomOutputProcess)
{
}

BloomOutputProcess::~BloomOutputProcess()
{
}

Process *BloomOutputProcess::Replicate(void) const
{
	return (new BloomOutputProcess(*this));
}

int32 BloomOutputProcess::GetPortCount(void) const
{
	return (1);
}

unsigned_int32 BloomOutputProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOptional);
}

const char *BloomOutputProcess::GetPortName(int32 index) const
{
	return ("A");
}

void BloomOutputProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->inputSize[0] = 1;
}

int32 BloomOutputProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	ShaderType type = LightOutputProcess::GetLightShaderType(compileData);
	if (type == kShaderInfiniteLight)
	{
		static const char code[] =
		{
			"MOV		result.color.w, %0;\n"
		};
		
		programCode[0] = code;
	}
	else
	{
		static const char code[] =
		{
			"MUL		result.color.w, %0, lshd.w;\n"
		};
		
		programCode[0] = code;
	}
	
	return (1);
}

int32 BloomOutputProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	ShaderType type = LightOutputProcess::GetLightShaderType(compileData);
	if (type == kShaderInfiniteLight)
	{
		static const char code[] =
		{
			RESULT_COLOR ".w = %0;\n"
		};
		
		shaderCode[0] = code;
	}
	else
	{
		static const char code[] =
		{
			RESULT_COLOR ".w = %0 * lshd.w;\n"
		};
		
		shaderCode[0] = code;
	}
	
	return (1);
}


AlphaTestOutputProcess::AlphaTestOutputProcess() : OutputProcess(kProcessAlphaTestOutput)
{
}

AlphaTestOutputProcess::AlphaTestOutputProcess(const AlphaTestOutputProcess& alphaTestOutputProcess) : OutputProcess(alphaTestOutputProcess)
{
}

AlphaTestOutputProcess::~AlphaTestOutputProcess()
{
}

Process *AlphaTestOutputProcess::Replicate(void) const
{
	return (new AlphaTestOutputProcess(*this));
}

int32 AlphaTestOutputProcess::GetPortCount(void) const
{
	return (1);
}

unsigned_int32 AlphaTestOutputProcess::GetPortFlags(int32 index) const
{
	return (kProcessPortOptional);
}

const char *AlphaTestOutputProcess::GetPortName(int32 index) const
{
	return ("A");
}

void AlphaTestOutputProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->inputSize[0] = 1;
}

int32 AlphaTestOutputProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char killCode[] =
	{
		"SUB		temp.x, %0, 0.5;\n"
		"KIL		temp.x;\n"
	};
	
	static const char alphaCode[] =
	{
		"MOV		result.color.w, %0;\n"
	};
	
	programCode[0] = killCode;
	
	if (compileData->shaderData->materialState & kMaterialAlphaCoverage)
	{
		programCode[1] = alphaCode;
		return (2);
	}
	
	return (1);
}

int32 AlphaTestOutputProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char killCode[] =
	{
		"if (%0 < 0.5) discard;\n"
	};
	
	static const char alphaCode[] =
	{
		RESULT_COLOR ".w = %0;\n"
	};
	
	shaderCode[0] = killCode;
	
	if (compileData->shaderData->materialState & kMaterialAlphaCoverage)
	{
		shaderCode[1] = alphaCode;
		return (2);
	}
	
	return (1);
}


StructureOutputProcess::StructureOutputProcess() : OutputProcess(kProcessStructureOutput)
{
}

StructureOutputProcess::StructureOutputProcess(const StructureOutputProcess& structureOutputProcess) : OutputProcess(structureOutputProcess)
{
}

StructureOutputProcess::~StructureOutputProcess()
{
}

Process *StructureOutputProcess::Replicate(void) const
{
	return (new StructureOutputProcess(*this));
}

unsigned_int32 StructureOutputProcess::GetStructureRenderFlags(unsigned_int32 renderableFlags)
{
	unsigned_int32 flags = kStructureRenderVelocity | kStructureRenderDepth | kStructureRenderGradient;
	if (renderableFlags & kRenderableStructureVelocityZero) flags &= ~(kStructureRenderVelocity | kStructureRenderGradient);
	if (renderableFlags & kRenderableStructureDepthZero) flags &= ~kStructureRenderDepth;
	return (flags & TheGraphicsMgr->GetStructureFlags());
}

#if C4OPENGL

	void StructureOutputProcess::GenerateSourceData(const ShaderCompileData *compileData) const
	{
		unsigned_int32 renderableFlags = compileData->renderable->GetRenderableFlags();
		if (GetStructureRenderFlags(renderableFlags) & (kStructureRenderVelocity | kStructureRenderGradient))
		{
			if (!TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) compileData->programFlag = false;
		}
	}

#endif

int32 StructureOutputProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = OutputProcess::GenerateProcessSignature(compileData, signature);
	signature[count] = GetStructureRenderFlags(compileData->renderable->GetRenderableFlags());
	return (count + 1);
}

void StructureOutputProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	unsigned_int32 flags = GetStructureRenderFlags(compileData->renderable->GetRenderableFlags());
	if (flags & kStructureRenderVelocity)
	{
		data->temporaryCount = 2;
		
		data->interpolantCount = 2;
		data->interpolantType[0] = 'VELA';
		data->interpolantType[1] = 'VELB';
	}
}

int32 StructureOutputProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char velocityCode[] =
	{
		"DIV		tmp1.xy, $VELA, $VELA.w;\n"
		"DIV		tmp2.xy, $VELB, $VELB.w;\n"
		"SUB		temp.xy, tmp2, tmp1;\n"
		
		"MUL		temp.xy, temp, program.env[" FRAGMENT_PARAM_VELOCITY_SCALE "];\n"
		"MAX		temp.z, |temp.x|, |temp.y|;\n"
		"MAX		temp.z, temp.z, 1.0;\n"
		"DIV		result.color.xy, temp, temp.z;\n"
		"MOV		result.color.z, $VELB.w;\n"
		
		"DDX		temp.x, $VELB.w;\n"
		"DDY		temp.y, $VELB.w;\n"
		"MAX		result.color.w, |temp.x|, |temp.y|;\n"
	};
	
	static const char depthCode[] =
	{
		"RCP		result.color.z, fragment.position.w;\n"
		"MOV		result.color.xyw, {0.0, 0.0, 0.0, 0.0};\n"
	};
	
	static const char zeroCode[] =
	{
		"MOV		result.color, {0.0, 0.0, 0.0, 0.0};\n"
	};
	
	unsigned_int32 flags = GetStructureRenderFlags(compileData->renderable->GetRenderableFlags());
	if (flags & kStructureRenderVelocity) programCode[0] = velocityCode;
	else if (flags & kStructureRenderDepth) programCode[0] = depthCode;
	else programCode[0] = zeroCode;
	
	return (1);
}

int32 StructureOutputProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char velocityCode[] =
	{
		"tmp1.xy = $VELA.xy / $VELA.w;\n"
		"tmp2.xy = $VELB.xy / $VELB.w;\n"
		"temp.xy = (tmp2.xy - tmp1.xy) * param[" FRAGMENT_PARAM_VELOCITY_SCALE "].xy;\n"
		RESULT_COLOR ".xy = temp.xy / max(max(abs(temp.x), abs(temp.y)), 1.0);\n"
		RESULT_COLOR ".z = $VELB.w;\n"
		RESULT_COLOR ".w = max(abs(" DDX "($VELB.w)), abs(" DDY "($VELB.w)));\n"
	};
	
	static const char depthCode[] =
	{
		RESULT_COLOR ".z = 1.0 / " FRAGMENT_POSITION ".w;\n"
		RESULT_COLOR ".xyw = " FLOAT3 "(0.0, 0.0, 0.0);\n"
	};
	
	static const char zeroCode[] =
	{
		RESULT_COLOR " = " FLOAT4 "(0.0, 0.0, 0.0, 0.0);\n"
	};
	
	unsigned_int32 flags = GetStructureRenderFlags(compileData->renderable->GetRenderableFlags());
	if (flags & kStructureRenderVelocity) shaderCode[0] = velocityCode;
	else if (flags & kStructureRenderDepth) shaderCode[0] = depthCode;
	else shaderCode[0] = zeroCode;
	
	return (1);
}


ConstantFogProcess::ConstantFogProcess() : Process(kProcessConstantFog)
{
}

ConstantFogProcess::ConstantFogProcess(const ConstantFogProcess& constantFogProcess) : Process(constantFogProcess)
{
}

ConstantFogProcess::~ConstantFogProcess()
{
}

Process *ConstantFogProcess::Replicate(void) const
{
	return (new ConstantFogProcess(*this));
}

void ConstantFogProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 1;
	
	data->interpolantCount = 3;
	data->interpolantType[0] = 'VDIR';
	data->interpolantType[1] = 'FDTP';
	data->interpolantType[2] = 'FDTV';
	
	if (compileData->renderable->GetTransformable()) compileData->shaderData->fogStateFunc = &StateFunc_TransformFogPlane;
	else compileData->shaderData->fogStateFunc = &StateFunc_CopyFogPlane;
}

int32 ConstantFogProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"ABS		temp.x, $FDTV;\n"
		"RCP		temp.x, temp.x;\n"
		"MAD_SAT	temp.x, $FDTP, -temp.x, program.env[" FRAGMENT_PARAM_FOG_PARAMS "].x;\n"
		
		"DP3		temp.y, $VDIR, $VDIR;\n"
		"RSQ		temp.w, temp.y;\n"
		"MUL		temp.y, temp.y, temp.w;\n"
		"MUL		temp.x, temp.x, temp.y;\n"
		"EX2_SAT	#, -temp.x;\n"
	};
	
	static const char code2[] =
	{
		"RCP		temp.x, |$FDTV|;\n"
		"MAD_SAT	temp.x, $FDTP, -temp.x, program.env[" FRAGMENT_PARAM_FOG_PARAMS "].x;\n"
		
		"DP3		temp.y, $VDIR, $VDIR;\n"
		"RSQ		temp.w, temp.y;\n"
		"MUL		temp.y, temp.y, temp.w;\n"
		"MUL		temp.x, temp.x, temp.y;\n"
		"EX2_SAT	#, -temp.x;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 ConstantFogProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		#if C4OPENGL
		
			"temp.x = clamp(param[" FRAGMENT_PARAM_FOG_PARAMS "].x - $FDTP / abs($FDTV), 0.0, 1.0);\n"
			"# = clamp(exp2(-temp.x * length($VDIR)), 0.0, 1.0);\n"
		
		#else
		
			"temp.x = saturate(param[" FRAGMENT_PARAM_FOG_PARAMS "].x - $FDTP / abs($FDTV));\n"
			"# = saturate(exp2(-temp.x * length($VDIR)));\n"
		
		#endif
	};
	
	shaderCode[0] = code;
	return (1);
}

void ConstantFogProcess::StateFunc_CopyFogPlane(const Renderable *renderable, const void *cookie)
{
	Render::SetVertexProgramParameter4fv(kVertexParamFogPlane, &TheGraphicsMgr->GetFogPlane().x);
}

void ConstantFogProcess::StateFunc_TransformFogPlane(const Renderable *renderable, const void *cookie)
{
	Antivector4D plane = TheGraphicsMgr->GetFogPlane() * renderable->GetTransformable()->GetWorldTransform();
	Render::SetVertexProgramParameter4fv(kVertexParamFogPlane, &plane.x);
}


LinearFogProcess::LinearFogProcess() : Process(kProcessLinearFog)
{
}

LinearFogProcess::LinearFogProcess(const LinearFogProcess& linearFogProcess) : Process(linearFogProcess)
{
}

LinearFogProcess::~LinearFogProcess()
{
}

Process *LinearFogProcess::Replicate(void) const
{
	return (new LinearFogProcess(*this));
}

void LinearFogProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 1;
	
	data->interpolantCount = 4;
	data->interpolantType[0] = 'VDIR';
	data->interpolantType[1] = 'FDTP';
	data->interpolantType[2] = 'FDTV';
	data->interpolantType[3] = 'FOGK';
	
	if (compileData->renderable->GetTransformable()) compileData->shaderData->fogStateFunc = &ConstantFogProcess::StateFunc_TransformFogPlane;
	else compileData->shaderData->fogStateFunc = &ConstantFogProcess::StateFunc_CopyFogPlane;
}

int32 LinearFogProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"MIN		temp.w, $FDTP, 0.0;\n"
		"ABS		temp.y, $FDTV;\n"
		"RCP		temp.y, temp.y;\n"
		"MUL		temp.x, temp.w, temp.y;\n"
		"MAD		temp.x, -temp.x, temp.w, $FOGK;\n"
		
		"DP3		temp.y, $VDIR, $VDIR;\n"
		"RSQ		temp.w, temp.y;\n"
		"MUL		temp.y, temp.y, temp.w;\n"
		"MUL		temp.x, temp.x, temp.y;\n"
		"EX2_SAT	#, temp.x;\n"
	};
	
	static const char code2[] =
	{
		"MIN		temp.w, $FDTP, 0.0;\n"
		"DIV		temp.x, temp.w, |$FDTV|;\n"
		"MAD		temp.x, -temp.x, temp.w, $FOGK;\n"
		
		"DP3		temp.y, $VDIR, $VDIR;\n"
		"RSQ		temp.w, temp.y;\n"
		"MUL		temp.y, temp.y, temp.w;\n"
		"MUL		temp.x, temp.x, temp.y;\n"
		"EX2_SAT	#, temp.x;\n"
	};
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = code2;
	else programCode[0] = code;
	
	return (1);
}

int32 LinearFogProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		#if C4OPENGL
		
			"temp.w = min($FDTP, 0.0);\n"
			"temp.x = $FOGK - temp.w * temp.w / abs($FDTV);\n"
			"# = clamp(exp2(temp.x * length($VDIR)), 0.0, 1.0);\n"
		
		#else
		
			"temp.w = min($FDTP, 0.0);\n"
			"temp.x = $FOGK - temp.w * temp.w / abs($FDTV);\n"
			"# = saturate(exp2(temp.x * length($VDIR)));\n"
		
		#endif
	};
	
	shaderCode[0] = code;
	return (1);
}


AmbientFogProcess::AmbientFogProcess() : Process(kProcessAmbientFog)
{
}

AmbientFogProcess::AmbientFogProcess(const AmbientFogProcess& ambientFogProcess) : Process(ambientFogProcess)
{
}

AmbientFogProcess::~AmbientFogProcess()
{
}

Process *AmbientFogProcess::Replicate(void) const
{
	return (new AmbientFogProcess(*this));
}

void AmbientFogProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->inputSize[0] = 3;
	data->inputSize[1] = 1;
}

int32 AmbientFogProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"LRP		result.color.xyz, %1, %0, program.env[" FRAGMENT_PARAM_FOG_COLOR "];\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 AmbientFogProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		RESULT_COLOR ".xyz = " LERP "(param[" FRAGMENT_PARAM_FOG_COLOR "].xyz, %0, %1);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


LightFogProcess::LightFogProcess() : Process(kProcessLightFog)
{
}

LightFogProcess::LightFogProcess(const LightFogProcess& lightFogProcess) : Process(lightFogProcess)
{
}

LightFogProcess::~LightFogProcess()
{
}

Process *LightFogProcess::Replicate(void) const
{
	return (new LightFogProcess(*this));
}

void LightFogProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->inputSize[0] = 3;
	data->inputSize[1] = 1;
}

int32 LightFogProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"MUL		result.color.xyz, %0, %1;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 LightFogProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		RESULT_COLOR ".xyz = %0 * %1;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


AlphaFogProcess::AlphaFogProcess() : Process(kProcessAlphaFog)
{
}

AlphaFogProcess::AlphaFogProcess(const AlphaFogProcess& alphaFogProcess) : Process(alphaFogProcess)
{
}

AlphaFogProcess::~AlphaFogProcess()
{
}

Process *AlphaFogProcess::Replicate(void) const
{
	return (new AlphaFogProcess(*this));
}

void AlphaFogProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->inputSize[0] = 3;
	data->inputSize[1] = 1;
}

int32 AlphaFogProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"LRP		result.color.xyz, %1, %0, program.env[" FRAGMENT_PARAM_FOG_COLOR "];\n"
		"SUB		result.color.w, 1.0, %1;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 AlphaFogProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		RESULT_COLOR ".xyz = " LERP "(param[" FRAGMENT_PARAM_FOG_COLOR "].xyz, %0, %1);\n"
		RESULT_COLOR ".w = 1.0 - %1;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


ColorPostProcess::ColorPostProcess() : Process(kProcessColorPost)
{
}

ColorPostProcess::ColorPostProcess(const ColorPostProcess& colorPostProcess) : Process(colorPostProcess)
{
}

ColorPostProcess::~ColorPostProcess()
{
}

Process *ColorPostProcess::Replicate(void) const
{
	return (new ColorPostProcess(*this));
}

int32 ColorPostProcess::GetPortCount(void) const
{
	return (1);
}

const char *ColorPostProcess::GetPortName(int32 index) const
{
	return ("P");
}

void ColorPostProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 4;
	data->inputSize[0] = 2;
}

int32 ColorPostProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEX		#, %0, texture[0], RECT;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 ColorPostProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"# = " TEXRECT "(colorTexture, %0);\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


DistortPostProcess::DistortPostProcess() : Process(kProcessDistortPost)
{
}

DistortPostProcess::DistortPostProcess(const DistortPostProcess& distortPostProcess) : Process(distortPostProcess)
{
}

DistortPostProcess::~DistortPostProcess()
{
}

Process *DistortPostProcess::Replicate(void) const
{
	return (new DistortPostProcess(*this));
}

void DistortPostProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->outputSize = 2;
}

int32 DistortPostProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEX		temp, fragment.position, texture[2], RECT;\n"
		"SUB		temp.xy, temp, temp.zwww;\n"
		"MAD		#, temp, program.env[" FRAGMENT_PARAM_DISTORTION_SCALE "], fragment.position;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 DistortPostProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"temp = " TEXRECT "(distortionTexture, " FRAGMENT_POSITION ".xy);\n"
		"# = (temp.xy - temp.zw) * param[" FRAGMENT_PARAM_DISTORTION_SCALE "].xy + " FRAGMENT_POSITION ".xy;\n"
	};
	
	shaderCode[0] = code;
	return (1);
}


MotionBlurPostProcess::MotionBlurPostProcess(bool gradient) : Process(kProcessMotionBlurPost)
{
	gradientFlag = gradient;
}

MotionBlurPostProcess::MotionBlurPostProcess(const MotionBlurPostProcess& motionBlurPostProcess) : Process(motionBlurPostProcess)
{
	gradientFlag = motionBlurPostProcess.gradientFlag;
}

MotionBlurPostProcess::~MotionBlurPostProcess()
{
}

Process *MotionBlurPostProcess::Replicate(void) const
{
	return (new MotionBlurPostProcess(*this));
}

int32 MotionBlurPostProcess::GetPortCount(void) const
{
	return (1);
}

const char *MotionBlurPostProcess::GetPortName(int32 index) const
{
	return ("P");
}

int32 MotionBlurPostProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = Process::GenerateProcessSignature(compileData, signature);
	signature[count] = gradientFlag;
	return (count + 1);
}

void MotionBlurPostProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->temporaryCount = 1;
	data->outputSize = 3;
	data->inputSize[0] = 2;
}

int32 MotionBlurPostProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		velo, samp;\n"
		
		"TEX		temp.xyz, %0, texture[0], RECT;\n"
		"MUL		temp.xyz, temp, 0.1111111;\n"
		
		"TEX		velo.xy, %0, texture[1], RECT;\n"
		
		"MAD		tmp1, velo.xyxy, {1.75, 1.75, -1.75, -1.75}, %0.xyxy;\n"
		"TEX		samp.xyz, tmp1, texture[0], RECT;\n"
		"MAD		temp.xyz, samp, 0.1111111, temp;\n"
		"TEX		samp.xyz, tmp1.zwww, texture[0], RECT;\n"
		"MAD		temp.xyz, samp, 0.1111111, temp;\n"
		
		"MAD		tmp1, velo.xyxy, {3.5, 3.5, -3.5, -3.5}, %0.xyxy;\n"
		"TEX		samp.xyz, tmp1, texture[0], RECT;\n"
		"MAD		temp.xyz, samp, 0.1111111, temp;\n"
		"TEX		samp.xyz, tmp1.zwww, texture[0], RECT;\n"
		"MAD		temp.xyz, samp, 0.1111111, temp;\n"
		
		"MAD		tmp1, velo.xyxy, {5.25, 5.25, -5.25, -5.25}, %0.xyxy;\n"
		"TEX		samp.xyz, tmp1, texture[0], RECT;\n"
		"MAD		temp.xyz, samp, 0.1111111, temp;\n"
		"TEX		samp.xyz, tmp1.zwww, texture[0], RECT;\n"
		"MAD		temp.xyz, samp, 0.1111111, temp;\n"
		
		"MAD		tmp1, velo.xyxy, {7.0, 7.0, -7.0, -7.0}, %0.xyxy;\n"
		"TEX		samp.xyz, tmp1, texture[0], RECT;\n"
		"MAD		temp.xyz, samp, 0.1111111, temp;\n"
		"TEX		samp.xyz, tmp1.zwww, texture[0], RECT;\n"
		"MAD		#, samp, 0.1111111, temp;\n"
	};
	
	static const char gradCode[] =
	{
		"TEMP		velo, dpth, samp;\n"
		
		"MOV		samp.w, 1.0;\n"
		"MOV		temp.w, 1.0;\n"
		"TEX		temp.xyz, %0, texture[0], RECT;\n"
		
		"TEX		velo, %0, texture[1], RECT;\n"
		"MAX		velo.w, velo.w, 0.00112;\n"			// 2/255 / 7.0
		"MAD		dpth.w, velo.w, -7.0, velo.z;\n"
		
		"MAD		tmp1, velo.xyxy, {1.75, 1.75, -1.75, -1.75}, %0.xyxy;\n"
		"TEX		samp.xyz, tmp1, texture[0], RECT;\n"
		"TEX		dpth.z, tmp1, texture[1], RECT;\n"
		"SGE		dpth.z, dpth.z, dpth.w;\n"
		"MAD		temp, samp, dpth.z, temp;\n"
		"TEX		samp.xyz, tmp1.zwww, texture[0], RECT;\n"
		"TEX		dpth.z, tmp1.zwww, texture[1], RECT;\n"
		"SGE		dpth.z, dpth.z, dpth.w;\n"
		"MAD		temp, samp, dpth.z, temp;\n"
		
		"MAD		tmp1, velo.xyxy, {3.5, 3.5, -3.5, -3.5}, %0.xyxy;\n"
		"TEX		samp.xyz, tmp1, texture[0], RECT;\n"
		"TEX		dpth.z, tmp1, texture[1], RECT;\n"
		"SGE		dpth.z, dpth.z, dpth.w;\n"
		"MAD		temp, samp, dpth.z, temp;\n"
		"TEX		samp.xyz, tmp1.zwww, texture[0], RECT;\n"
		"TEX		dpth.z, tmp1.zwww, texture[1], RECT;\n"
		"SGE		dpth.z, dpth.z, dpth.w;\n"
		"MAD		temp, samp, dpth.z, temp;\n"
		
		"MAD		tmp1, velo.xyxy, {5.25, 5.25, -5.25, -5.25}, %0.xyxy;\n"
		"TEX		samp.xyz, tmp1, texture[0], RECT;\n"
		"TEX		dpth.z, tmp1, texture[1], RECT;\n"
		"SGE		dpth.z, dpth.z, dpth.w;\n"
		"MAD		temp, samp, dpth.z, temp;\n"
		"TEX		samp.xyz, tmp1.zwww, texture[0], RECT;\n"
		"TEX		dpth.z, tmp1.zwww, texture[1], RECT;\n"
		"SGE		dpth.z, dpth.z, dpth.w;\n"
		"MAD		temp, samp, dpth.z, temp;\n"
		
		"MAD		tmp1, velo.xyxy, {7.0, 7.0, -7.0, -7.0}, %0.xyxy;\n"
		"TEX		samp.xyz, tmp1, texture[0], RECT;\n"
		"TEX		dpth.z, tmp1, texture[1], RECT;\n"
		"SGE		dpth.z, dpth.z, dpth.w;\n"
		"MAD		temp, samp, dpth.z, temp;\n"
		"TEX		samp.xyz, tmp1.zwww, texture[0], RECT;\n"
		"TEX		dpth.z, tmp1.zwww, texture[1], RECT;\n"
		"SGE		dpth.z, dpth.z, dpth.w;\n"
		"MAD		temp, samp, dpth.z, temp;\n"
		
		"RCP		temp.w, temp.w;\n"
		"MUL		#, temp, temp.w;\n"
	};
	
	static const char gradCode2[] =
	{
		"TEMP		velo, dpth, samp;\n"
		
		"MOV		samp.w, 1.0;\n"
		"MOV		temp.w, 1.0;\n"
		"TEX		temp.xyz, %0, texture[0], RECT;\n"
		
		"TEX		velo, %0, texture[1], RECT;\n"
		"MAX		velo.w, velo.w, 0.00112;\n"			// 2/255 / 7.0
		"MAD		dpth.w, velo.w, -7.0, velo.z;\n"
		
		"MAD		tmp1, velo.xyxy, {1.75, 1.75, -1.75, -1.75}, %0.xyxy;\n"
		"TEX		samp.xyz, tmp1, texture[0], RECT;\n"
		"TEX		dpth.z, tmp1, texture[1], RECT;\n"
		"SGEC		dpth.z, dpth.z, dpth.w;\n"
		"ADD		temp (NE.z), samp, temp;\n"
		"TEX		samp.xyz, tmp1.zwww, texture[0], RECT;\n"
		"TEX		dpth.z, tmp1.zwww, texture[1], RECT;\n"
		"SGEC		dpth.z, dpth.z, dpth.w;\n"
		"ADD		temp (NE.z), samp, temp;\n"
		
		"MAD		tmp1, velo.xyxy, {3.5, 3.5, -3.5, -3.5}, %0.xyxy;\n"
		"TEX		samp.xyz, tmp1, texture[0], RECT;\n"
		"TEX		dpth.z, tmp1, texture[1], RECT;\n"
		"SGEC		dpth.z, dpth.z, dpth.w;\n"
		"ADD		temp (NE.z), samp, temp;\n"
		"TEX		samp.xyz, tmp1.zwww, texture[0], RECT;\n"
		"TEX		dpth.z, tmp1.zwww, texture[1], RECT;\n"
		"SGEC		dpth.z, dpth.z, dpth.w;\n"
		"ADD		temp (NE.z), samp, temp;\n"
		
		"MAD		tmp1, velo.xyxy, {5.25, 5.25, -5.25, -5.25}, %0.xyxy;\n"
		"TEX		samp.xyz, tmp1, texture[0], RECT;\n"
		"TEX		dpth.z, tmp1, texture[1], RECT;\n"
		"SGEC		dpth.z, dpth.z, dpth.w;\n"
		"ADD		temp (NE.z), samp, temp;\n"
		"TEX		samp.xyz, tmp1.zwww, texture[0], RECT;\n"
		"TEX		dpth.z, tmp1.zwww, texture[1], RECT;\n"
		"SGEC		dpth.z, dpth.z, dpth.w;\n"
		"ADD		temp (NE.z), samp, temp;\n"
		
		"MAD		tmp1, velo.xyxy, {7.0, 7.0, -7.0, -7.0}, %0.xyxy;\n"
		"TEX		samp.xyz, tmp1, texture[0], RECT;\n"
		"TEX		dpth.z, tmp1, texture[1], RECT;\n"
		"SGEC		dpth.z, dpth.z, dpth.w;\n"
		"ADD		temp (NE.z), samp, temp;\n"
		"TEX		samp.xyz, tmp1.zwww, texture[0], RECT;\n"
		"TEX		dpth.z, tmp1.zwww, texture[1], RECT;\n"
		"SGEC		dpth.z, dpth.z, dpth.w;\n"
		"ADD		temp (NE.z), samp, temp;\n"
		
		"RCP		temp.w, temp.w;\n"
		"MUL		#, temp, temp.w;\n"
	};
	
	if (gradientFlag)
	{
		if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionFragmentProgram2]) programCode[0] = gradCode2;
		else programCode[0] = gradCode;
	}
	else
	{
		programCode[0] = code;
	}
	
	return (1);
}

int32 MotionBlurPostProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		"temp.xyz = " TEXRECT "(colorTexture, %0).xyz * 0.1111111;\n"
		FLOAT2 " velo = " TEXRECT "(velocityTexture, %0).xy;\n"
		
		"tmp1 = velo.xyxy * " FLOAT4 "(1.75, 1.75, -1.75, -1.75) + %0.xyxy;\n"
		"temp.xyz += " TEXRECT "(colorTexture, tmp1.xy).xyz * 0.1111111;\n"
		"temp.xyz += " TEXRECT "(colorTexture, tmp1.zw).xyz * 0.1111111;\n"
		
		"tmp1 = velo.xyxy * " FLOAT4 "(3.5, 3.5, -3.5, -3.5) + %0.xyxy;\n"
		"temp.xyz += " TEXRECT "(colorTexture, tmp1.xy).xyz * 0.1111111;\n"
		"temp.xyz += " TEXRECT "(colorTexture, tmp1.zw).xyz * 0.1111111;\n"
		
		"tmp1 = velo.xyxy * " FLOAT4 "(5.5, 5.5, -5.5, -5.5) + %0.xyxy;\n"
		"temp.xyz += " TEXRECT "(colorTexture, tmp1.xy).xyz * 0.1111111;\n"
		"temp.xyz += " TEXRECT "(colorTexture, tmp1.zw).xyz * 0.1111111;\n"
		
		"tmp1 = velo.xyxy * " FLOAT4 "(7.5, 7.5, -7.5, -7.5) + %0.xyxy;\n"
		"temp.xyz += " TEXRECT "(colorTexture, tmp1.xy).xyz * 0.1111111;\n"
		"# = temp.xyz + " TEXRECT "(colorTexture, tmp1.zw).xyz * 0.1111111;\n"
	};
	
	static const char gradCode[] =
	{
		FLOAT4 " samp, dpth;\n"
		
		"samp.w = 1.0;\n"
		"temp.w = 1.0;\n"
		"temp.xyz = " TEXRECT "(colorTexture, %0).xyz;\n"
		
		FLOAT4 " velo = " TEXRECT "(velocityTexture, %0);\n"
		"dpth.w = velo.z - max(velo.w, 0.00112) * 7.0;\n"
		
		"tmp1 = velo.xyxy * " FLOAT4 "(1.75, 1.75, -1.75, -1.75) + %0.xyxy;\n"
		"samp.xyz = " TEXRECT "(colorTexture, tmp1.xy).xyz;\n"
		"dpth.z = " TEXRECT "(velocityTexture, tmp1.xy).z;\n"
		"if (dpth.z >= dpth.w) temp += samp;\n"
		"samp.xyz = " TEXRECT "(colorTexture, tmp1.zw).xyz;\n"
		"dpth.z = " TEXRECT "(velocityTexture, tmp1.zw).z;\n"
		"if (dpth.z >= dpth.w) temp += samp;\n"
		
		"tmp1 = velo.xyxy * " FLOAT4 "(3.5, 3.5, -3.5, -3.5) + %0.xyxy;\n"
		"samp.xyz = " TEXRECT "(colorTexture, tmp1.xy).xyz;\n"
		"dpth.z = " TEXRECT "(velocityTexture, tmp1.xy).z;\n"
		"if (dpth.z >= dpth.w) temp += samp;\n"
		"samp.xyz = " TEXRECT "(colorTexture, tmp1.zw).xyz;\n"
		"dpth.z = " TEXRECT "(velocityTexture, tmp1.zw).z;\n"
		"if (dpth.z >= dpth.w) temp += samp;\n"
		
		"tmp1 = velo.xyxy * " FLOAT4 "(5.25, 5.25, -5.25, -5.25) + %0.xyxy;\n"
		"samp.xyz = " TEXRECT "(colorTexture, tmp1.xy).xyz;\n"
		"dpth.z = " TEXRECT "(velocityTexture, tmp1.xy).z;\n"
		"if (dpth.z >= dpth.w) temp += samp;\n"
		"samp.xyz = " TEXRECT "(colorTexture, tmp1.zw).xyz;\n"
		"dpth.z = " TEXRECT "(velocityTexture, tmp1.zw).z;\n"
		"if (dpth.z >= dpth.w) temp += samp;\n"
		
		"tmp1 = velo.xyxy * " FLOAT4 "(7.0, 7.0, -7.0, -7.0) + %0.xyxy;\n"
		"samp.xyz = " TEXRECT "(colorTexture, tmp1.xy).xyz;\n"
		"dpth.z = " TEXRECT "(velocityTexture, tmp1.xy).z;\n"
		"if (dpth.z >= dpth.w) temp += samp;\n"
		"samp.xyz = " TEXRECT "(colorTexture, tmp1.zw).xyz;\n"
		"dpth.z = " TEXRECT "(velocityTexture, tmp1.zw).z;\n"
		"if (dpth.z >= dpth.w) temp += samp;\n"
		
		"# = temp.xyz / temp.w;\n"
	};
	
	if (gradientFlag) shaderCode[0] = gradCode;
	else shaderCode[0] = code;
	
	return (1);
}


ExtractPostProcess::ExtractPostProcess() : Process(kProcessExtractPost)
{
}

ExtractPostProcess::ExtractPostProcess(const ExtractPostProcess& extractPostProcess) : Process(extractPostProcess)
{
}

ExtractPostProcess::~ExtractPostProcess()
{
}

Process *ExtractPostProcess::Replicate(void) const
{
	return (new ExtractPostProcess(*this));
}

void ExtractPostProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->temporaryCount = 2;
}

int32 ExtractPostProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		col1, col2;\n"
		
		"ADD		tmp1, fragment.texcoord.xyyx, {-2.0, 0.0, 0.0, 2.0};\n"
		"ADD		tmp2, fragment.texcoord.xyyx, {-4.0, 0.0, 0.0, 4.0};\n"
		
		"TEX		col1, fragment.texcoord, texture[0], RECT;\n"
		"MUL		col1.xyz, col1, col1.w;\n"
		
		"TEX		temp, tmp1, texture[0], RECT;\n"
		"MAD		col1.xyz, temp, temp.w, col1;\n"
		"TEX		temp, tmp1.wzyx, texture[0], RECT;\n"
		"MAD		col1.xyz, temp, temp.w, col1;\n"
		
		"TEX		col2, tmp2, texture[0], RECT;\n"
		"MUL		col2.xyz, col2, col2.w;\n"
		"TEX		temp, tmp2.wzyx, texture[0], RECT;\n"
		"MAD		col2.xyz, temp, temp.w, col2;\n"
		
		"MAD		temp.xyz, col2, 0.5, col1;\n"
		"MUL		result.color.xyz, temp, 0.25;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 ExtractPostProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		#if C4OPENGL
		
			"tmp1 = gl_TexCoord[0].xyxy + vec4(-2.0, 0.0, 2.0, 0.0);\n"
			"tmp2 = gl_TexCoord[0].xyxy + vec4(-4.0, 0.0, 4.0, 0.0);\n"
			
			"vec4 col1 = texture2DRect(colorTexture, gl_TexCoord[0].xy);\n"
			"col1.xyz *= col1.w;\n"
			
			"temp = texture2DRect(colorTexture, tmp1.xy);\n"
			"col1.xyz += temp.xyz * temp.w;\n"
			"temp = texture2DRect(colorTexture, tmp1.zw);\n"
			"col1.xyz += temp.xyz * temp.w;\n"
			
			"vec4 col2 = texture2DRect(colorTexture, tmp2.xy);\n"
			"col2.xyz *= col2.w;\n"
			"temp = texture2DRect(colorTexture, tmp2.zw);\n"
			"col2.xyz += temp.xyz * temp.w;\n"
			
			"gl_FragColor.xyz = (col2.xyz * 0.5 + col1.xyz) * 0.25;\n"
		
		#else
		
			"tmp1 = fragment.texcoord.xyxy + float4(-2.0, 0.0, 2.0, 0.0);\n"
			"tmp2 = fragment.texcoord.xyxy + float4(-4.0, 0.0, 4.0, 0.0);\n"
			
			"float4 col1 = texRECT(colorTexture, fragment.texcoord.xy);\n"
			"col1.xyz *= col1.w;\n"
			
			"temp = texRECT(colorTexture, tmp1.xy);\n"
			"col1.xyz += temp.xyz * temp.w;\n"
			"temp = texRECT(colorTexture, tmp1.zw);\n"
			"col1.xyz += temp.xyz * temp.w;\n"
			
			"float4 col2 = texRECT(colorTexture, tmp2.xy);\n"
			"col2.xyz *= col2.w;\n"
			"temp = texRECT(colorTexture, tmp2.zw);\n"
			"col2.xyz += temp.xyz * temp.w;\n"
			
			"result.color.xyz = (col2.xyz * 0.5 + col1.xyz) * 0.25;\n"
		
		#endif
	};
	
	shaderCode[0] = code;
	return (1);
}


GlowPostProcess::GlowPostProcess() : Process(kProcessGlowPost)
{
}

GlowPostProcess::GlowPostProcess(const GlowPostProcess& glowPostProcess) : Process(glowPostProcess)
{
}

GlowPostProcess::~GlowPostProcess()
{
}

Process *GlowPostProcess::Replicate(void) const
{
	return (new GlowPostProcess(*this));
}

int32 GlowPostProcess::GetPortCount(void) const
{
	return (1);
}

const char *GlowPostProcess::GetPortName(int32 index) const
{
	return ("RGB");
}

void GlowPostProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->temporaryCount = 2;
	data->outputSize = 3;
	data->inputSize[0] = 3;
}

int32 GlowPostProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"TEMP		col1, col2;\n"
		
		"ADD		tmp1, fragment.texcoord.xyyx, {0.0, -1.0, 1.0, 0.0};\n"
		"ADD		tmp2, fragment.texcoord.xyyx, {0.0, -2.0, 2.0, 0.0};\n"
		
		"TEX		col1.xyz, fragment.texcoord, texture[3], RECT;\n"
		"TEX		temp.xyz, tmp1, texture[3], RECT;\n"
		"ADD		col1.xyz, col1, temp;\n"
		"TEX		temp.xyz, tmp1.wzyx, texture[3], RECT;\n"
		"ADD		col1.xyz, col1, temp;\n"
		
		"TEX		col2.xyz, tmp2, texture[3], RECT;\n"
		"TEX		temp.xyz, tmp2.wzyx, texture[3], RECT;\n"
		"ADD		col2.xyz, col2, temp;\n"
		
		"MAD		temp.xyz, col2, 0.5, col1;\n"
		"MAD		#, temp, 0.25, %0;\n"
	};
	
	programCode[0] = code;
	return (1);
}

int32 GlowPostProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		#if C4OPENGL
		
			"tmp1 = gl_TexCoord[0].xyxy + vec4(0.0, -1.0, 0.0, 1.0);\n"
			"tmp2 = gl_TexCoord[0].xyxy + vec4(0.0, -2.0, 0.0, 2.0);\n"
			
			"vec3 col1 = texture2DRect(glowTexture, gl_TexCoord[0].xy).xyz;\n"
			"col1 += texture2DRect(glowTexture, tmp1.xy).xyz;\n"
			"col1 += texture2DRect(glowTexture, tmp1.zw).xyz;\n"
			
			"vec3 col2 = texture2DRect(glowTexture, tmp2.xy).xyz;\n"
			"col2 += texture2DRect(glowTexture, tmp2.zw).xyz;\n"
			
			"# = (col2 * 0.5 + col1) * 0.25 + %0;\n"
		
		#else
		
			"tmp1 = fragment.texcoord.xyxy + float4(0.0, -1.0, 0.0, 1.0);\n"
			"tmp2 = fragment.texcoord.xyxy + float4(0.0, -2.0, 0.0, 2.0);\n"
			
			"float3 col1 = texRECT(glowTexture, fragment.texcoord.xy).xyz;\n"
			"col1 += texRECT(glowTexture, tmp1.xy).xyz;\n"
			"col1 += texRECT(glowTexture, tmp1.zw).xyz;\n"
			
			"float3 col2 = texRECT(glowTexture, tmp2.xy).xyz;\n"
			"col2 += texRECT(glowTexture, tmp2.zw).xyz;\n"
			
			"# = (col2 * 0.5 + col1) * 0.25 + %0;\n"
		
		#endif
	};
	
	shaderCode[0] = code;
	return (1);
}


TransformPostProcess::TransformPostProcess(bool matrixFlag) : Process(kProcessTransformPost)
{
	colorMatrixFlag = matrixFlag;
}

TransformPostProcess::TransformPostProcess(const TransformPostProcess& transformPostProcess) : Process(transformPostProcess)
{
	colorMatrixFlag = transformPostProcess.colorMatrixFlag;
}

TransformPostProcess::~TransformPostProcess()
{
}

Process *TransformPostProcess::Replicate(void) const
{
	return (new TransformPostProcess(*this));
}

int32 TransformPostProcess::GetPortCount(void) const
{
	return (1);
}

const char *TransformPostProcess::GetPortName(int32 index) const
{
	return ("RGB");
}

int32 TransformPostProcess::GenerateProcessSignature(const ShaderCompileData *compileData, unsigned_int32 *signature) const
{
	int32 count = Process::GenerateProcessSignature(compileData, signature);
	signature[count] = colorMatrixFlag;
	return (count + 1);
}

void TransformPostProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->inputSize[0] = 3;
}

int32 TransformPostProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	if (colorMatrixFlag)
	{
		static const char code[] =
		{
			"DP3		temp.x, %0, program.env[" FRAGMENT_PARAM_CONSTANT0 "];\n"
			"DP3		temp.y, %0, program.env[" FRAGMENT_PARAM_CONSTANT1 "];\n"
			"DP3		temp.z, %0, program.env[" FRAGMENT_PARAM_CONSTANT2 "];\n"
			"ADD		result.color, temp, program.env[" FRAGMENT_PARAM_CONSTANT3 "];\n"
		};
		
		programCode[0] = code;
	}
	else
	{
		static const char code[] =
		{
			"MAD		result.color, %0, program.env[" FRAGMENT_PARAM_CONSTANT0 "], program.env[" FRAGMENT_PARAM_CONSTANT3 "];\n"
		};
		
		programCode[0] = code;
	}
	
	return (1);
}

int32 TransformPostProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	if (colorMatrixFlag)
	{
		static const char code[] =
		{
			"temp.x = dot(%0, param[" FRAGMENT_PARAM_CONSTANT0 "].xyz);\n"
			"temp.y = dot(%0, param[" FRAGMENT_PARAM_CONSTANT1 "].xyz);\n"
			"temp.z = dot(%0, param[" FRAGMENT_PARAM_CONSTANT2 "].xyz);\n"
			RESULT_COLOR ".xyz = temp.xyz + param[" FRAGMENT_PARAM_CONSTANT3 "].xyz;\n"
		};
		
		shaderCode[0] = code;
	}
	else
	{
		static const char code[] =
		{
			RESULT_COLOR ".xyz = %0 * param[" FRAGMENT_PARAM_CONSTANT0 "].xyz + param[" FRAGMENT_PARAM_CONSTANT3 "].xyz;\n"
		};
		
		shaderCode[0] = code;
	}
	
	return (1);
}

// ZYURVUR
