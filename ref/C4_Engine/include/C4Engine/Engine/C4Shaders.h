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


#ifndef C4Shaders_h
#define C4Shaders_h


//# \component	Graphics Manager
//# \prefix		GraphicsMgr/


#include "C4Horizon.h"


namespace C4
{
	typedef EngineResult	ShaderResult;
	
	
	enum
	{
		kShaderOkay							= kEngineOkay,
		kShaderIncomplete					= 0x0001,
		kShaderRegisterOverflow				= 0x0002,
		kShaderLiteralOverflow				= 0x0003,
		kShaderTexcoordOverflow				= 0x0004,
		kShaderTextureUnitOverflow			= 0x0005,
		kShaderResultCount					= 6
	};
	
	
	enum
	{
		kAmbientGraphTexcoord1,
		kAmbientGraphTexcoord2,
		kAmbientGraphParallax,
		kAmbientGraphDiffuseColor,
		kAmbientGraphEmissionColor,
		kAmbientGraphReflectionColor,
		kAmbientGraphRefractionColor,
		kAmbientGraphEnvironmentColor,
		kAmbientGraphTextureMap1,
		kAmbientGraphTextureMap2,
		kAmbientGraphNormalMap1,
		kAmbientGraphNormalMap2,
		kAmbientGraphEmissionMap,
		kAmbientGraphGlossMap,
		kAmbientGraphOpacityMap,
		kAmbientGraphVertexColor,
		kAmbientGraphTextureCombiner,
		kAmbientGraphNormalCombiner,
		kAmbientGraphColorMultiply,
		kAmbientGraphDiffuseMultiply,
		kAmbientGraphOcclusionMultiply,
		kAmbientGraphEmissionMultiply,
		kAmbientGraphOpacityMultiply,
		kAmbientGraphConstantUnity,
		kAmbientGraphOpacitySubtract,
		kAmbientGraphRefractionMultiply,
		kAmbientGraphEnvironmentMultiply,
		kAmbientGraphAmbientOutput,
		kAmbientGraphAmbientAlphaOutput,
		kAmbientGraphAlphaTestOutput,
		kAmbientGraphEmissionOutput,
		kAmbientGraphReflectionOutput,
		kAmbientGraphRefractionOutput,
		kAmbientGraphEnvironmentOutput,
		kAmbientGraphTerrainEnvironmentOutput,
		kAmbientGraphGlowOutput,
		kAmbientGraphImpostorDepthOutput,
		kAmbientGraphProcessCount
	};
	
	
	enum
	{
		kLightGraphTexcoord1,
		kLightGraphTexcoord2,
		kLightGraphParallax,
		kLightGraphDiffuseColor,
		kLightGraphSpecularColor,
		kLightGraphSpecularExponent,
		kLightGraphMicrofacet,
		kLightGraphMicrofacetReflectivity,
		kLightGraphTextureMap1,
		kLightGraphTextureMap2,
		kLightGraphNormalMap1,
		kLightGraphNormalMap2,
		kLightGraphGlossMap,
		kLightGraphVertexColor,
		kLightGraphTextureCombiner,
		kLightGraphNormalCombiner,
		kLightGraphColorMultiply,
		kLightGraphDiffuseMultiply1,
		kLightGraphDiffuseMultiply2,
		kLightGraphSpecularMultiply1,
		kLightGraphSpecularMultiply2,
		kLightGraphDiffuseReflection,
		kLightGraphSpecularReflection,
		kLightGraphTangentLightDirection,
		kLightGraphLightSum,
		kLightGraphHorizon,
		kLightGraphBloom1,
		kLightGraphBloom2,
		kLightGraphLightOutput, 
		kLightGraphAlphaTestOutput,
		kLightGraphBloomOutput, 
		kLightGraphProcessCount 
	}; 
	
	 
	enum
	{
		kEffectGraphTexcoord,
		kEffectGraphEffectColor, 
		kEffectGraphVertexColor,
		kEffectGraphTextureMap,
		kEffectGraphDeltaDepth,
		kEffectGraphFire, 
		kEffectGraphDistortion,
		kEffectGraphColorMultiply,
		kEffectGraphAlphaMultiply,
		kEffectGraphEffectMultiply,
		kEffectGraphColorOutput,
		kEffectGraphAlphaOutput,
		kEffectGraphAlphaTestOutput,
		kEffectGraphProcessCount
	};
	
	
	enum
	{
		kPlainGraphTexcoord1,
		kPlainGraphTexcoord2,
		kPlainGraphDiffuseColor,
		kPlainGraphTextureMap1,
		kPlainGraphTextureMap2,
		kPlainGraphVertexColor,
		kPlainGraphTextureCombiner,
		kPlainGraphColorMultiply,
		kPlainGraphDiffuseMultiply1,
		kPlainGraphDiffuseMultiply2,
		kPlainGraphAlphaTestOutput,
		kPlainGraphProcessCount
	};
	
	
	class VertexProgram;
	
	
	//# \class	ShaderAttribute		Shader graph container.
	//
	//# The $ShaderAttribute$ class is a container for a set of shader graphs.
	//
	//# \def	class ShaderAttribute : public Attribute
	//
	//# \ctor	ShaderAttribute();
	//
	//# \desc
	//# The $ShaderAttribute$ class is used to store a set of shader graphs in a material object.
	//
	//# \base	Attribute		A shader attribute is a special type of attribute.
	//
	//# \also	$@Process@$
	//# \also	$@Route@$
	
	
	//# \function	ShaderAttribute::GetShaderGraph		Returns the route flags.
	//
	//# \proto	ShaderGraph *GetShaderGraph(int32 index);
	//# \proto	const ShaderGraph *GetShaderGraph(int32 index) const;
	//
	//# \param	index	The shader graph index. This can be $kShaderGraphAmbient$ or $kShaderGraphLight$.
	//
	//# \desc
	//# The $GetShaderGraph$ function returns a pointer to the shader graph for the pass specified by the
	//# index parameter.
	
	
	class ShaderAttribute : public Attribute
	{
		private:
			
			enum
			{
				kMaxShaderSourceSize		= 8192,
				kMaxShaderSignatureSize		= 1024
			};
			
			ShaderGraph			shaderGraph[kShaderGraphCount];
			
			Attribute *Replicate(void) const override;
			
			static void PackShader(const ShaderGraph *graph, Packer& data, unsigned_int32 packFlags);
			static Process **UnpackShader(ShaderGraph *graph, Unpacker& data, unsigned_int32 unpackFlags);
			
			static ShaderResult PrepareProcessPorts(const Process *process, const ShaderCompileData *compileData);
			static ShaderResult PrepareAmbientShader(const ShaderCompileData *compileData, ShaderGraph *graph, List<Process> *terminalList);
			static ShaderResult PrepareLightShader(const ShaderCompileData *compileData, ShaderGraph *graph, List<Process> *terminalList);
			static ShaderResult PreparePlainShader(ShaderType type, const ShaderCompileData *compileData, ShaderGraph *graph, List<Process> *terminalList);
			
			static Process *FindDerivedInterpolant(ProcessType type, int32 count, Process *const *interpolant);
			static void OrganizeDerivedInterpolants(const ShaderCompileData *compileData, ShaderGraph *graph);
			
			static void OptimizeTextureMaps(const ShaderGraph *graph);
			static void EliminateDeadCode(const ShaderGraph *graph, List<Process> *terminalList);
			static void CalculatePathLengths(const ShaderGraph *graph, List<Process> *processList, List<Process> *readyList);
			static int32 ScheduleShader(const ShaderCompileData *compileData, List<Process> *readyList, List<Process> *scheduleList, unsigned_int32 *shaderSignature);
			
			static bool AllocateOutputRegister(ProcessData *data, unsigned_int8 *registerLive);
			static bool AllocateInterpolant(Type type, int32 size, ShaderAllocationData *allocData, bool (*usage)[4]);
			static int32 AllocateTextureUnit(ShaderData *shaderData, const Render::TextureObject *textureObject);
			static ShaderResult AllocateShaderResources(const ShaderCompileData *compileData, ShaderAllocationData *allocData, int32 processCount, ProcessData *processData, const List<Process> *scheduleList);
			
			static int32 GenerateSwizzleData(const char *code, SwizzleData *swizzleData);
			static int32 GenerateLiteralConstantValue(Type type, const ShaderAllocationData *allocData, char *value);
			static void GenerateShaderCode(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, const ProcessData *processData, const List<Process> *scheduleList, char *program, int32 *length);
			static int32 GenerateShaderProlog(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, char *program);
			static int32 GenerateShaderEpilog(const ShaderCompileData *compileData, char *program);
			
			static int32 GenerateVertexOutputName(Type type, const ShaderAllocationData *allocData, int32 mask, char *name);
			static unsigned_int32 GenerateVertexProgram(const ShaderCompileData *compileData, const ShaderAllocationData *allocData, VertexProgram **vertexProgram);
			static void BuildStateFunctionList(const ShaderCompileData *compileData, unsigned_int32 stateFlags);
			
			#if C4OPENGL
			
				static void BindShaderUniforms(ShaderType type, FragmentProgram *fragmentShader);
			
			#endif
			
			static Process *BuildTextureCombiner(const MaterialObject *materialObject, ShaderGraph *graph, Process **textureMap1, Process **textureMap2, Process **textureCombiner, Process **vertexColor);
		
		public:
			
			static char				sourceStorage[kMaxShaderSourceSize];
			static unsigned_int32	signatureStorage[kMaxShaderSignatureSize];
			
			C4API ShaderAttribute();
			C4API ShaderAttribute(const ShaderAttribute& shaderAttribute);
			C4API ~ShaderAttribute();
			
			ShaderGraph *GetShaderGraph(int32 index)
			{
				return (&shaderGraph[index]);
			}
			
			const ShaderGraph *GetShaderGraph(int32 index) const
			{
				return (&shaderGraph[index]);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			bool operator ==(const Attribute& attribute) const;
			
			C4API static void CloneShader(const ShaderGraph *sourceGraph, ShaderGraph *destinGraph, bool reference = false);
			
			C4API ShaderResult CompileShader(ShaderType type, ShaderVariant variant, int32 level, const Renderable *renderable, const RenderSegment *renderSegment, ShaderData *shaderData, ShaderGraph *graph = nullptr) const;
			C4API ShaderResult TestShader(ShaderType type, ShaderVariant variant, int32 level, const Renderable *renderable, const RenderSegment *renderSegment) const;
			static FragmentProgram *CompilePostShader(const ShaderGraph *graph);
			
			C4API static void BuildAmbientShaderGraph(const Renderable *renderable, const RenderSegment *renderSegment, const MaterialObject *materialObject, const List<Attribute> *attributeList, ShaderGraph *graph, Process **process);
			C4API static void BuildLightShaderGraph(const Renderable *renderable, const RenderSegment *renderSegment, const MaterialObject *materialObject, const List<Attribute> *attributeList, ShaderGraph *graph, Process **process);
			static void BuildEffectShaderGraph(const Renderable *renderable, const RenderSegment *renderSegment, const MaterialObject *materialObject, const List<Attribute> *attributeList, ShaderGraph *graph);
			static void BuildPlainShaderGraph(const Renderable *renderable, const RenderSegment *renderSegment, const MaterialObject *materialObject, const List<Attribute> *attributeList, ShaderGraph *graph);
			
			C4API void SetParameterValue(int32 slot, const Vector4D& param);
	};
}


#endif

// ZYURVUR
