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


#ifndef C4Renderable_h
#define C4Renderable_h


//# \component	Graphics Manager
//# \prefix		GraphicsMgr/

//# \import		C4MaterialObjects.h


#include "C4VertexPrograms.h"
#include "C4MaterialObjects.h"
#include "C4Computation.h"


namespace C4
{
	enum
	{
		kMaxShaderDetailLevelCount		= 2,
		kMaxShaderRegisterCount			= 24,
		kMaxShaderTexcoordCount			= 8,
		kMaxShaderInterpolantCount		= 16,
		kMaxShaderTextureCount			= 10,
		kMaxShaderConstantCount			= 8,
		kMaxShaderStateDataCount		= 10 + kMaxShaderConstantCount
	};
	
	
	enum
	{
		kMaxRenderParameterCount		= 6,
		kMaxTexcoordParameterCount		= 1,
		kMaxTerrainParameterCount		= 2
	};
	
	
	enum
	{
		kGroupKeyAmbient,
		kGroupKeyLight,
		kMaxGroupKeyCount
	};
	
	
	enum
	{
		kVertexBufferStaticArray,
		kVertexBufferDynamicArray,
		kVertexBufferIndexArray,
		kVertexBufferCount
	};


	enum
	{
		kVertexBufferAttribute		= 0,
		kVertexBufferIndex			= 1 << 0,
		kVertexBufferStatic			= 0,
		kVertexBufferDynamic		= 1 << 0
	};
	
	
	//# \enum	RenderState
	
	enum
	{
		kRenderDepthTest		= 1 << 0,	//## The depth test is enabled. If the depth test is disabled, then depth is also not written.
		kRenderColorInhibit		= 1 << 1,	//## Writes to the color buffer are disabled.
		kRenderDepthInhibit		= 1 << 2,	//## Writes to the depth buffer are disabled.
		kRenderDepthOffset		= 1 << 3,	//## Apply depth offset. See the $@Renderable::SetDepthOffset@$ function.
		kRenderAlphaTest		= 1 << 4,	//## Apply the alpha test to prevent fragments with zero alpha from being rendered.
		kRenderLineSmooth		= 1 << 5,	//## Line smoothing is enabled (only valid if rendering lines).
		kRenderWireframe		= 1 << 6,	//## Render wireframe instead of filled polygons.
		kRenderStencilTest		= 1 << 7
	};
	
	
	//# \enum	RenderableFlags
	
	enum
	{
		kRenderableCameraTransformInhibit	= 1 << 0,	//## Do not apply the world-space to camera-space transformation.
		kRenderableStructureBufferInhibit	= 1 << 1,	//## Do not render into the structure buffer at all.
		kRenderableStructureVelocityZero	= 1 << 2,	//## Always render zero as the velocity in the structure buffer.
		kRenderableStructureDepthZero		= 1 << 3,	//## Always render zero as the depth in the structure buffer. $kRenderableStructureVelocityZero$ must also be set.
		kRenderableMotionBlurGradient		= 1 << 4,	//## Account for the depth gradient when rendering motion blur.
		kRenderableFogInhibit				= 1 << 5,	//## Do not render with fog.
		kRenderableUnfog					= 1 << 6
	};
	
	
	//# \enum	ShaderFlags
	
	enum
	{
		kShaderAmbientEffect				= 1 << 0,
		kShaderNormalizeBasisVectors		= 1 << 1,	//## Normalize the vertex-space basis vectors (normal and tangent).
		kShaderCubeLightInhibit				= 1 << 2,	//## Render using point light shaders when illuminated by a cube light.
		kShaderAlphaFogFraction				= 1 << 3,
		kShaderFireArrays					= 1 << 4,
		kShaderDistortion					= 1 << 5, 
		kShaderGenerateTexcoord				= 1 << 6,
		kShaderGenerateTangent				= 1 << 7, 
		kShaderTerrainBorder				= 1 << 8, 
		kShaderWaterElevation				= 1 << 9, 
		kShaderVertexInfinite				= 1 << 16,
		kShaderVertexBillboard				= 1 << 17, 
		kShaderVertexPostboard				= 1 << 18,
		kShaderVertexPolyboard				= 1 << 19,
		kShaderLinearPolyboard				= 1 << 20,
		kShaderOrthoPolyboard				= 1 << 21, 
		kShaderScaleVertex					= 1 << 22,
		kShaderOffsetVertex					= 1 << 23,
		kShaderNormalExpandVertex			= 1 << 24,
		kShaderTexcoordVertex				= 1 << 25 
	};
	
	
	enum
	{
		kArrayVertex				= 0,
		kArrayPosition0				= 0,
		kArrayPosition1				= 1,
		kArrayNormal				= 2,
		kArrayColor0				= 3,
		kArrayColor1				= 4,
		kArrayColor2				= 5,
		kArrayTangent				= 6,
		kArrayRadius				= 6,
		kArrayOffset				= 6,
		kArrayTexture0				= 8,
		kArrayTexture1				= 9,
		kArrayTexture2				= 10,
		kArrayTexture3				= 11,
		kArrayPrevious				= 12,
		kArrayVelocity				= 13,
		kArrayBillboard				= 14,
		kMaxAttributeArrayCount		= 16
	};
	
	
	enum
	{
		kShaderArrayPosition0		= 0,
		kShaderArrayPosition1		= 1,
		kShaderArrayPrevious		= 2,
		kShaderArrayVelocity		= 2,
		kShaderArrayNormal			= 3,
		kShaderArrayTangent			= 4,
		kShaderArrayRadius			= 4,
		kShaderArrayOffset			= 4,
		kShaderArrayColor0			= 5,
		kShaderArrayColor1			= 6,
		kShaderArrayColor2			= 7,
		kShaderArrayTexture0		= 8,
		kShaderArrayTexture1		= 9,
		kMaxShaderArrayCount		= 10
	};
	
	
	//# \enum	BlendFactor
	
	enum BlendFactor
	{
		kBlendZero					= 0,			//## Zero.
		kBlendOne					= 1,			//## One.
		kBlendSourceColor			= 2,			//## Source color.
		kBlendDestColor				= 3,			//## Destination color.
		kBlendConstColor			= 4,
		kBlendSourceAlpha			= 5,			//## Source alpha.
		kBlendDestAlpha				= 6,			//## Destination alpha.
		kBlendConstAlpha			= 7,
		kBlendInvSourceColor		= 8,			//## One minus source color.
		kBlendInvDestColor			= 9,			//## One minus destination color.
		kBlendInvConstColor			= 10,
		kBlendInvSourceAlpha		= 11,			//## One minus source alpha.
		kBlendInvDestAlpha			= 12,			//## One minus destination alpha.
		kBlendInvConstAlpha			= 13,
		kBlendFactorCount
	};
	
	
	//# \enum	BlendState
	
	enum
	{
		kBlendReplace				= (kBlendZero << 4) | kBlendOne,						//## Replace the destination color with the source color.
		kBlendAccumulate			= (kBlendOne << 4) | kBlendOne,							//## Add the source color to the destination color.
		kBlendModulate				= (kBlendZero << 4) | kBlendDestColor,					//## Multiply the destination color by the source color.
		kBlendInterpolate			= (kBlendInvSourceAlpha << 4) | kBlendSourceAlpha,		//## Interpolate between the source and destination colors using the source alpha.
		kBlendAlphaPreserve			= (kBlendOne << 12) | (kBlendZero << 8),
		kBlendAlphaReplace			= (kBlendZero << 12) | (kBlendOne << 8),
		kBlendAlphaAccumulate		= (kBlendOne << 12) | (kBlendOne << 8),
		
		kBlendColorMask				= 0x00FF,
		kBlendAlphaMask				= 0xFF00
	};
	
	
	enum ShaderType
	{
		kShaderNone = -1,
		kShaderAmbient,
		kShaderAmbientGradient,
		kShaderAmbientSpace,
		kShaderInfiniteLight,
		kShaderDepthLight,
		kShaderLandscapeLight,
		kShaderPointLight,
		kShaderCubeLight,
		kShaderSpotLight,
		kShaderShadowMap,
		kShaderStructure,
		kShaderTypeCount,
		
		kShaderFirstAmbient = kShaderAmbient,
		kShaderLastAmbient = kShaderAmbientSpace,
		kShaderFirstLight = kShaderInfiniteLight,
		kShaderLastLight = kShaderSpotLight,
		kShaderFirstInfiniteLight = kShaderInfiniteLight,
		kShaderLastInfiniteLight = kShaderLandscapeLight,
		kShaderFirstPointLight = kShaderPointLight,
		kShaderLastPointLight = kShaderSpotLight,
		kShaderFirstPlain = kShaderShadowMap,
		kShaderLastPlain = kShaderStructure
	};
	
	
	enum ShaderVariant
	{
		kShaderVariantNormal,
		kShaderVariantConstantFog,
		kShaderVariantLinearFog,
		kShaderVariantCount
	};
	
	
	//# \enum	RenderType
	
	enum RenderType
	{
		kRenderPoints,						//## A set of <i>n</i> points.
		kRenderLines,						//## A set of <i>n</i>&nbsp;/&nbsp;2 unconnected line segments.
		kRenderLineStrip,					//## A set of <i>n</i>&nbsp;&minus;&nbsp;1 connected line segments.
		kRenderLineLoop,					//## A set of <i>n</i> connected line segments forming a closed loop.
		kRenderIndexedLines,				//## A set of line segments with indexed vertices.
		kRenderTriangles,					//## A set of <i>n</i>&nbsp;/&nbsp;3 unconnected triangles.
		kRenderTriangleStrip,				//## A set of <i>n</i>&nbsp;&minus;&nbsp;2 triangles connected as a strip.
		kRenderIndexedTriangles,			//## A set of triangles with indexed vertices.
		kRenderQuads,						//## A set of <i>n</i>&nbsp;/&nbsp;4 unconnected quads.
		kRenderMultiIndexedTriangles,
		kRenderMaskedMultiIndexedTriangles
	};
	
	
	enum
	{
		kShaderStateCameraPosition			= 1 << 0,
		kShaderStateCameraPosition4D		= 1 << 1,
		kShaderStateCameraDirections		= 1 << 2,
		kShaderStateCameraTransform			= 1 << 3,
		kShaderStateWorldTransform			= 1 << 4,
		kShaderStatePaintTransform			= 1 << 5,
		kShaderStateVertexScaleOffset		= 1 << 6,
		kShaderStateTerrainBorder			= 1 << 7,
		kShaderStateImpostorRadius			= 1 << 8,
		kShaderStateImpostorTransition		= 1 << 9,
		kShaderStateGeometryTransition		= 1 << 10,
		kShaderStateBaseTexcoord			= 1 << 11,
		kShaderStateTexcoordGenerate		= 1 << 12,
		kShaderStateTexcoordTransform0		= 1 << 13,
		kShaderStateTexcoordTransform1		= 1 << 14,
		kShaderStateTexcoordVelocity0		= 1 << 15,
		kShaderStateTexcoordVelocity1		= 1 << 16,
		kShaderStateTerrainTexcoordScale	= 1 << 17
	};
	
	
	class Renderable;
	class Box3D;
	class AmbientSpaceObject;
	class Portal;
	
	
	inline unsigned_int32 BlendState(BlendFactor sc, BlendFactor dc, BlendFactor sa = kBlendZero, BlendFactor da = kBlendZero)
	{
		return ((da << 12) | (sa << 8) | (dc << 4) | sc);
	}
	
	inline BlendFactor GetBlendSource(unsigned_int32 state)
	{
		return (static_cast<BlendFactor>(state & 0x0F));
	}
	
	inline BlendFactor GetBlendDest(unsigned_int32 state)
	{
		return (static_cast<BlendFactor>((state >> 4) & 0x0F));
	}
	
	inline BlendFactor GetBlendSourceAlpha(unsigned_int32 state)
	{
		return (static_cast<BlendFactor>((state >> 8) & 0x0F));
	}
	
	inline BlendFactor GetBlendDestAlpha(unsigned_int32 state)
	{
		return (static_cast<BlendFactor>((state >> 12) & 0x0F));
	}
	
	
	struct PaintEnvironment
	{
		Transform4D						paintTransform;
		const Texture					*const *paintTexture;
	};
	
	
	struct AmbientEnvironment
	{
		ShaderType						ambientShaderType;
		
		const ColorRGBA					*ambientLightColor;
		const ColorRGBA					*gradientLightColor;
		const Portal					*gradientPortal[2];
		
		const AmbientSpaceObject		*ambientSpaceObject;
		const Transformable				*ambientSpaceTransformable;
		
		const Texture					*const *environmentMap;
	};
	
	
	class VertexBuffer : public Render::VertexBufferObject, public ExclusiveObservable<VertexBuffer>, public ListElement<VertexBuffer>
	{
		private:
			
			bool						activeFlag;
			unsigned_int32				bufferSize;
			unsigned_int32				vertexStride;
			
			static int32				totalVertexBufferCount;
			static unsigned_int32		totalVertexBufferMemory;
			
			static List<VertexBuffer>	vertexBufferList;
		
		public:
			
			VertexBuffer(unsigned_int32 flags);
			~VertexBuffer();
			
			unsigned_int32 GetVertexStride(void) const
			{
				return (vertexStride);
			}
			
			bool Active(void) const
			{
				return (activeFlag);
			}
			
			static int32 GetTotalVertexBufferCount(void)
			{
				return (totalVertexBufferCount);
			}
			
			static unsigned_int32 GetTotalVertexBufferMemory(void)
			{
				return (totalVertexBufferMemory);
			}
			
			void Activate(void);
			void Deactivate(void);
			
			C4API void Initialize(unsigned_int32 size, unsigned_int32 stride, ObserverType *observer = nullptr);
			
			static void DeactivateAll(void);
			static void ReactivateAll(void);
	};


	template <class observerType> class VertexBufferObserver : public ExclusiveObserver<observerType, VertexBuffer>
	{
		public:
			
			VertexBufferObserver(observerType *observer, void (observerType::*callback)(VertexBuffer *)) : ExclusiveObserver<observerType, VertexBuffer>(observer, callback)
			{
			}
	};
	
	
	//# \class	OcclusionQuery		Represents an occlusion query operation.
	//
	//# The $OcclusionQuery$ class represents an occlusion query operation.
	//
	//# \def	class OcclusionQuery : public ListElement<OcclusionQuery>
	//
	//# \ctor	OcclusionQuery(RenderProc *proc, void *cookie);
	//
	//# \param	proc	A pointer to the occlusion query's callback procedure.
	//# \param	cookie	The cookie that is passed to the callback procedure as its last parameter.
	//
	//# \desc
	//# 
	//# \code	typedef void RenderProc(OcclusionQuery *, List<Renderable> *, void *);
	//
	//# \base	Utilities/ListElement<OcclusionQuery>	Used internally by the Graphics Manager.
	//
	//# \also	$@Renderable::GetOcclusionQuery@$
	//# \also	$@Renderable::SetOcclusionQuery@$
	
	
	//# \function	OcclusionQuery::GetUnoccludedArea		Returns the area that was unoccluded during the occlusion query.
	//
	//# \proto	float GetUnoccludedArea(void) const;
	//
	//# \desc
	//# 
	
	
	class OcclusionQuery : public Render::QueryObject, public ListElement<OcclusionQuery>
	{
		friend class RenderSegment;
		friend class GraphicsMgr;
		
		public:
			
			typedef void RenderProc(OcclusionQuery *, List<Renderable> *, void *);
		
		private:
			
			bool			activeFlag;
			
			RenderProc		*renderProc;
			void			*renderCookie;
			
			float			unoccludedArea;
			
			static List<OcclusionQuery>		occlusionQueryList;
			
			void Activate(void);
			void Deactivate(void);
			
			static void DeactivateAll(void);
		
		public:
			
			OcclusionQuery(RenderProc *proc, void *cookie);
			~OcclusionQuery();
			
			RenderProc *GetRenderProc(void) const
			{
				return (renderProc);
			}
			
			void SetRenderProc(RenderProc *proc)
			{
				renderProc = proc;
			}
			
			void *GetRenderCookie(void) const
			{
				return (renderCookie);
			}
			
			void SetRenderCookie(void *cookie)
			{
				renderCookie = cookie;
			}
			
			void SetRenderProc(RenderProc *proc, void *cookie)
			{
				renderProc = proc;
				renderCookie = cookie;
			}
			
			float GetUnoccludedArea(void) const
			{
				return (unoccludedArea);
			}
	};
	
	
	struct ShaderProgramData
	{
		VertexProgram		*vertexProgram;
		FragmentProgram		*fragmentProgram;
		
		ShaderProgramData();
		~ShaderProgramData();
	};
	
	
	class ShaderData : public ListElement<ShaderData>, public Memory<ShaderData>
	{
		private:	
			
			ShaderData						**shaderDataPointer;
			
			static List<ShaderData>			shaderDataList;
		
		public:
			
			typedef void ShaderStateFunc(const Renderable *, const void *);
			
			struct ShaderStateData
			{
				ShaderStateFunc		*stateFunc;
				const void			*stateCookie;
			};
			
			unsigned_int32					blendState;
			unsigned_int32					materialState;
			
			unsigned_int32					variantMask;
			ShaderProgramData				programData[kShaderVariantCount];
			
			union
			{
				const float					*const *shaderArray[kMaxShaderArrayCount];
				const unsigned_int32		*shaderOffset[kMaxShaderArrayCount];
			};
			
			const VertexBuffer				*indexBuffer;
			const VertexBuffer				*vertexBuffer[kMaxShaderArrayCount];
			char							componentCount[kMaxShaderArrayCount];
			
			int32							textureUnitCount;
			const Render::TextureObject		*textureObject[kMaxShaderTextureCount];
			
			ShaderStateFunc					*fogStateFunc;
			
			int32							shaderStateDataCount;
			ShaderStateData					shaderStateData[kMaxShaderStateDataCount];
			
			ShaderData(ShaderData **pointer, unsigned_int32 blend, unsigned_int32 material);
			~ShaderData();
			
			static void Purge(void)
			{
				shaderDataList.Purge();
			}
			
			void AddStateFunction(ShaderStateFunc *func, const void *cookie = nullptr);
	};
	
	
	//# \class	RenderSegment		Stores rendering information for one segment of a renderable object.
	//
	//# The $RenderSegment$ class stores rendering information for one segment of a renderable object.
	//
	//# \def	class RenderSegment
	//
	//# \ctor	RenderSegment(unsigned_int32 state = 0);
	//
	//# \param	state	Flags that determine various material states to be applied. See below for possible values.
	//
	//# \desc
	//
	//# \value	kMaterialTwoSided	The material should be rendered two-sided.
	//# \value	kMaterialAlphaTest	Use alpha testing with the material.
	
	
	//# \function	RenderSegment::GetNextRenderSegment		Returns the next segment in the linked list of render segments.
	//
	//# \proto	RenderSegment *GetNextRenderSegment(void) const;
	//
	//# \desc
	//
	//# \also	$@RenderSegment::SetNextRenderSegment@$
	//# \also	$@Renderable::GetFirstRenderSegment@$
	
	
	//# \function	RenderSegment::SetNextRenderSegment		Sets the next segment in the linked list of render segments.
	//
	//# \proto	void SetNextRenderSegment(RenderSegment *segment);
	//
	//# \param	segment		The render segment will follow the segment for which this function is called.
	//
	//# \desc
	//
	//# \also	$@RenderSegment::GetNextRenderSegment@$
	//# \also	$@Renderable::GetFirstRenderSegment@$
	
	
	//# \function	RenderSegment::GetFaceStart		Returns the starting index of faces belonging to a render segment.
	//
	//# \proto	int32 GetFaceStart(void) const;
	//
	//# \desc
	//
	//# \also	$@RenderSegment::GetFaceCount@$
	//# \also	$@RenderSegment::SetFaceRange@$
	
	
	//# \function	RenderSegment::GetFaceCount		Returns the number of faces belonging to a render segment.
	//
	//# \proto	int32 GetFaceCount(void) const;
	//
	//# \desc
	//
	//# \also	$@RenderSegment::GetFaceStart@$
	//# \also	$@RenderSegment::SetFaceRange@$
	
	
	//# \function	RenderSegment::SetFaceRange		Sets the starting index and the number of faces belonging to a render segment.
	//
	//# \proto	void SetFaceRange(int32 start, int32 count);
	//
	//# \param	start	The face index at which the render segment begins.
	//# \param	count	The number of faces in the render segment.
	//
	//# \desc
	//
	//# \also	$@RenderSegment::GetFaceCount@$
	//# \also	$@RenderSegment::GetFaceStart@$
	
	
	//# \function	RenderSegment::GetMaterialState		Returns the material state flags.
	//
	//# \proto	unsigned_int32 GetMaterialState(void) const;
	//
	//# \desc
	//# The $GetMaterialState$ function returns the material state flags for a render segment, which can be zero
	//# or a combination (through logical OR) the following values.
	//
	//# \table	MaterialFlags
	//
	//# \also	$@RenderSegment::SetMaterialState@$
	
	
	//# \function	RenderSegment::SetMaterialState		Sets the material state flags.
	//
	//# \proto	void SetMaterialState(unsigned_int32 state);
	//
	//# \desc
	//# The $SetMaterialState$ function sets the material state flags for a render segment, which can be zero
	//# or a combination (through logical OR) the following values.
	//
	//# \table	MaterialFlags
	//
	//# \also	$@RenderSegment::GetMaterialState@$
	
	
	//# \function	RenderSegment::GetMaterialObjectPointer		Returns the material object pointer.
	//
	//# \proto	MaterialObject *const *GetMaterialObjectPointer(void) const;
	//
	//# \desc
	//# The $GetMaterialObjectPointer$ function returns the pointer to the location at which a pointer to a
	//# material object resides.
	//
	//# \also	$@RenderSegment::SetMaterialObjectPointer@$
	//# \also	$@RenderSegment::GetMaterialAttributeList@$
	//# \also	$@RenderSegment::SetMaterialAttributeList@$
	//# \also	$@MaterialObject@$
	
	
	//# \function	RenderSegment::SetMaterialObjectPointer		Sets the material object pointer.
	//
	//# \proto	void SetMaterialObjectPointer(MaterialObject *const *object);
	//
	//# \param	object		A pointer to a location holding a pointer to a material object.
	//
	//# \desc
	//# The $SetMaterialObjectPointer$ function sets the pointer to the location at which a pointer to a
	//# material object resides.
	//
	//# \also	$@RenderSegment::GetMaterialObjectPointer@$
	//# \also	$@RenderSegment::GetMaterialAttributeList@$
	//# \also	$@RenderSegment::SetMaterialAttributeList@$
	//# \also	$@MaterialObject@$
	
	
	//# \function	RenderSegment::GetMaterialAttributeList		Returns the material attribute list.
	//
	//# \proto	List<Attribute> *GetMaterialAttributeList(void) const;
	//
	//# \desc
	//# The $GetMaterialAttributeList$ function returns a pointer to the material attribute list assigned to a render segment.
	//# The presence of a material attribute list is optional, and $nullptr$ is returned if the render segment does
	//# not have an attribute list. If present, the list contains shading attributes that either augment or override
	//# the attributes stored in the material object assigned to the render segment.
	//
	//# \also	$@RenderSegment::SetMaterialAttributeList@$
	//# \also	$@RenderSegment::GetMaterialObjectPointer@$
	//# \also	$@RenderSegment::SetMaterialObjectPointer@$
	//# \also	$@Attribute@$
	
	
	//# \function	RenderSegment::SetMaterialAttributeList		Sets the material attribute list.
	//
	//# \proto	void SetMaterialAttributeList(List<Attribute> *list);
	//
	//# \param	list	A pointer to a list of material attributes.
	//
	//# \desc
	//# The $SetMaterialAttributeList$ function assigns a material attribute list to a render segment. The presence
	//# of a material attribute list is optional, and the $list$ attribute may be $nullptr$ to indicate that
	//# the render segment has no material attribute list. If present, the shading attributes in the list
	//# either augment or override the attributes stored in the material object assigned to the render segment.
	//
	//# \also	$@RenderSegment::GetMaterialAttributeList@$
	//# \also	$@RenderSegment::GetMaterialObjectPointer@$
	//# \also	$@RenderSegment::SetMaterialObjectPointer@$
	//# \also	$@Attribute@$
	
	
	//# \function	RenderSegment::InvalidateShaderData		Invalidates the shader data for a render segment.
	//
	//# \proto	void InvalidateShaderData(void);
	//
	//# \desc
	//# The $InvalidateShaderData$ function causes the internal shader data for a render segment to be discarded.
	//# The shader data is rebuilt the next time the segment is rendered. It is necessary to call this function
	//# whenever a segment's material object or material attribute list is altered. However, it is usually not
	//# necessary to call this function in the case that a color or texture is changed in an existing attribute.
	//
	//# \also	$@Renderable::InvalidateShaderData@$
	
	
	class RenderSegment
	{
		private:
			
			RenderSegment		*nextSegment;
			
			int32				faceStart;
			
			union
			{
				int32			faceCount;
				int32			multiRenderCount;
			};
			
			unsigned_int32		materialState;
			
			MaterialObject		*const *materialObject;
			List<Attribute>		*materialAttributeList;
			
			union
			{
				const unsigned_int32	*multiCountArray;
				unsigned_int32			multiRenderMask;
			};
			
			union
			{
				const machine_address	*multiOffsetArray;
				const int32				*multiRenderData;
			};
			
			ShaderData			*shaderData[kShaderTypeCount][kMaxShaderDetailLevelCount];
			
			unsigned_int32 GetShaderDataMaterialState(ShaderType type);
			
			ShaderData *InitAmbientShaderData(Renderable *renderable, ShaderType type, ShaderVariant variant, int32 level);
			ShaderData *InitEffectShaderData(Renderable *renderable, ShaderType type, ShaderVariant variant, int32 level);
			ShaderData *InitLightShaderData(Renderable *renderable, ShaderType type, ShaderVariant variant, int32 level);
			ShaderData *InitPlainShaderData(Renderable *renderable, ShaderType type, int32 level);
		
		public:
			
			RenderSegment(unsigned_int32 state = 0);
			~RenderSegment();
			
			RenderSegment *GetNextRenderSegment(void) const
			{
				return (nextSegment);
			}
			
			void SetNextRenderSegment(RenderSegment *segment)
			{
				nextSegment = segment;
			}
			
			int32 GetFaceStart(void) const
			{
				return (faceStart);
			}
			
			int32 GetFaceCount(void) const
			{
				return (faceCount);
			}
			
			void SetFaceRange(int32 start, int32 count)
			{
				faceStart = start;
				faceCount = count;
			}
			
			int32 GetMultiRenderCount(void) const
			{
				return (multiRenderCount);
			}
			
			void SetMultiRenderCount(int32 count)
			{
				multiRenderCount = count;
			}
			
			unsigned_int32 GetMaterialState(void) const
			{
				return (materialState);
			}
			
			void SetMaterialState(unsigned_int32 state)
			{
				materialState = state;
			}
			
			MaterialObject *const *GetMaterialObjectPointer(void) const
			{
				return (materialObject);
			}
			
			void SetMaterialObjectPointer(MaterialObject *const *object)
			{
				materialObject = object;
			}
			
			List<Attribute> *GetMaterialAttributeList(void) const
			{
				return (materialAttributeList);
			}
			
			void SetMaterialAttributeList(List<Attribute> *list)
			{
				materialAttributeList = list;
			}
			
			const unsigned_int32 *GetMultiCountArray(void) const
			{
				return (multiCountArray);
			}
			
			const machine_address *GetMultiOffsetArray(void) const
			{
				return (multiOffsetArray);
			}
			
			void SetMultiRenderArrays(const unsigned_int32 *count, const machine_address *offset)
			{
				multiCountArray = count;
				multiOffsetArray = offset;
			}
			
			unsigned_int32 GetMultiRenderMask(void) const
			{
				return (multiRenderMask);
			}
			
			void SetMultiRenderMask(unsigned_int32 mask)
			{
				multiRenderMask = mask;
			}
			
			const int32 *GetMultiRenderData(void) const
			{
				return (multiRenderData);
			}
			
			void SetMultiRenderData(const int32 *data)
			{
				multiRenderData = data;
			}
			
			const ShaderData *GetShaderData(int32 type, int32 level = 0) const
			{
				return (shaderData[type][level]);
			}
			
			ShaderData *InitShaderData(Renderable *renderable, ShaderType type, ShaderVariant variant = kShaderVariantNormal);
			
			C4API void InvalidateShaderData(void);
			C4API void InvalidateAmbientShaderData(void);
	};
	
	
	//# \class	Renderable		Stores general rendering information for a renderable object.
	//
	//# The $Renderable$ class stores general rendering information for a renderable object.
	//
	//# \def	class Renderable : public ListElement<Renderable>
	//
	//# \ctor	Renderable(RenderType type, unsigned_int32 state = 0);
	//
	//# \param	type	The primitive type of the renderable object. See below for possible values.
	//# \param	state	Flags that determine various render states to be applied. See below for possible values.
	//
	//# \desc
	//# The $Renderable$ class stores general rendering information about a single renderable object. The use of the
	//# $Renderable$ class is the sole means by which an object can be rendered by the Graphics Manager. Objects are
	//# rendered by storing their associated $Renderable$ objects (which may be base classes of more specialized structures)
	//# in a list and passing the list to the $@GraphicsMgr::DrawRenderList@$ function.
	//# 
	//# The $type$ parameter passed to the constructor specifies the rendering primitive used by the object and may be any
	//# one of the following values, where <i>n</i> represents the number of vertices.
	//
	//# \table	RenderType
	//
	//# When an object is rendered, the current light (as established using the $@GraphicsMgr::SetLight@$ function) determines
	//# how the object is shaded. The $state$ parameter specifies light-independent rendering state and may be any
	//# combination of the following bit flags.
	//
	//# \table	RenderState
	//
	//# \base	Utilities/ListElement<Renderable>	$Renderable$ objects are stored in a list that is passed to the
	//#												$@GraphicsMgr::DrawRenderList@$ function.
	
	
	//# \function	Renderable::GetRenderType		Returns the primitive render type.
	//
	//# \proto	RenderType GetRenderType(void) const;
	//
	//# \desc
	//# The $GetRenderType$ function returns one of the following values, representing the primitive render type of an object,
	//# where <i>n</i> represents the number of vertices.
	//
	//# \table	RenderType
	//
	//# \also	$@Renderable::SetRenderType@$
	
	
	//# \function	Renderable::SetRenderType		Sets the primitive render type.
	//
	//# \proto	void SetRenderType(RenderType type);
	//
	//# \param	type	The primitive type of the renderable object. See below for possible values.
	//
	//# \desc
	//# The $SetRenderType$ function sets the primitive render type of an object. The $type$ parameter may be one of the
	//# following values, where <i>n</i> represents the number of vertices.
	//
	//# \table	RenderType
	//
	//# \also	$@Renderable::GetRenderType@$
	
	
	//# \function	Renderable::GetRenderState		Returns the render state flags that pertain to rendering.
	//
	//# \proto	unsigned_int32 GetRenderState(void) const;
	//
	//# \desc
	//# The $GetRenderState$ function returns the light-independent rendering state, which can be a combination (through logical OR) of the
	//# following bit flags.
	//
	//# \table	RenderState
	//
	//# \also	$@Renderable::SetRenderState@$
	
	
	//# \function	Renderable::SetRenderState		Sets the render state flags that pertain to rendering.
	//
	//# \proto	void SetRenderState(unsigned_int32 state);
	//
	//# \param	state	The new render state flags.
	//
	//# \desc
	//# The $SetRenderState$ function sets the light-independent render state flags. The $state$ parameter may be any
	//# combination of the following bit flags.
	//
	//# \table	RenderState
	//
	//# \also	$@Renderable::GetRenderState@$
	
	
	//# \function	Renderable::GetRenderableFlags		Returns the miscellaneous renderable flags.
	//
	//# \proto	unsigned_int32 GetRenderableFlags(void) const;
	//
	//# \desc
	//# The $GetRenderableFlags$ function returns the miscellaneous renderable flags, which can be a combination (through logical OR) of the
	//# following bit flags.
	//
	//# \table	RenderableFlags
	//
	//# By default, none of a renderable object's miscellaneous renderable flags are set.
	//
	//# \also	$@Renderable::SetRenderableFlags@$
	
	
	//# \function	Renderable::SetRenderableFlags		Sets the miscellaneous renderable flags.
	//
	//# \proto	void SetRenderableFlags(unsigned_int32 flags);
	//
	//# \param	flags	The new renderable flags.
	//
	//# \desc
	//# The $SetRenderableFlags$ function sets the miscellaneous renderable flags. The $flags$ parameter may be any
	//# combination of the following bit flags.
	//
	//# \table	RenderableFlags
	//
	//# By default, none of a renderable object's miscellaneous renderable flags are set.
	//
	//# \also	$@Renderable::GetRenderableFlags@$
	
	
	//# \function	Renderable::GetShaderFlags		Returns the shader initialization flags.
	//
	//# \proto	unsigned_int32 GetShaderFlags(void) const;
	//
	//# \desc
	//# The $GetShaderFlags$ function returns the shader initialization flags, which can be a combination (through logical OR) of the
	//# following bit flags.
	//
	//# \table	ShaderFlags
	//
	//# By default, none of a renderable object's shader initialization flags are set.
	//
	//# \also	$@Renderable::SetShaderFlags@$
	
	
	//# \function	Renderable::SetShaderFlags		Sets the shader initialization flags.
	//
	//# \proto	void SetShaderFlags(unsigned_int32 flags);
	//
	//# \param	flags	The new shader initialization flags.
	//
	//# \desc
	//# The $SetShaderFlags$ function sets the shader initialization flags. The $flags$ parameter may be any
	//# combination of the following bit flags.
	//
	//# \table	ShaderFlags
	//
	//# By default, none of a renderable object's shader initialization flags are set.
	//
	//# \also	$@Renderable::GetShaderFlags@$
	
	
	//# \function	Renderable::GetBlendState		Returns the ambient pass blend state.
	//
	//# \proto	unsigned_int32 GetBlendState(void) const;
	//
	//# \desc
	//# The $GetBlendState$ function returns the blending function used when an object is rendered in the ambient
	//# pass. See the $@Renderable::SetBlendState@$ function for more information about the value returned.
	//
	//# \also	$@Renderable::SetBlendState@$
	
	
	//# \function	Renderable::SetBlendState		Sets the ambient pass blend state.
	//
	//# \proto	void SetBlendState(unsigned_int32 state);
	//
	//# \param	state	The new ambient pass blend state.
	//
	//# \desc
	//# The $SetBlendState$ function sets the blending function used when an object is rendered in the ambient
	//# pass. The $state$ parameter encodes the blending factors and can be constructed using the $BlendState$
	//# function. The first two parameters of the $BlendState$ function specify one of the following constants
	//# for the source blend factor and the destination blend factor, respectively.
	//
	//# \table	BlendFactor
	//
	//# For example, $BlendState(kBlendOne, kBlendZero)$ returns the blend state corresponding to replacement
	//# of the destination color by the source color. $BlendState(kBlendOne, kBlendOne)$ adds the source color
	//# to the destination color.
	//# 
	//# There are several predefined constants that can also be passed to the $state$ parameter, listed below.
	//
	//# \table	BlendState
	//
	//# \also	$@Renderable::GetBlendState@$
	
	
	//# \function	Renderable::GetDepthOffsetDelta		Returns the depth offset delta value.
	//
	//# \proto	float GetDepthOffsetDelta(void) const;
	//
	//# \desc
	//# 
	//
	//# \also	$@Renderable::GetDepthOffsetPoint@$
	//# \also	$@Renderable::SetDepthOffset@$
	
	
	//# \function	Renderable::GetDepthOffsetPoint		Returns the depth offset center point.
	//
	//# \proto	const Point3D& GetDepthOffsetPoint(void) const;
	//
	//# \desc
	//# 
	//
	//# \also	$@Renderable::GetDepthOffsetDelta@$
	//# \also	$@Renderable::SetDepthOffset@$
	
	
	//# \function	Renderable::SetDepthOffset		Sets the depth offset parameters.
	//
	//# \proto	void SetDepthOffset(float delta, const Point3D *point);
	//
	//# \param	delta	The depth offset delta value.
	//# \param	point	A pointer to the depth offset center point, in world-space coordinates.
	//
	//# \desc
	//# 
	//
	//# \also	$@Renderable::GetDepthOffsetDelta@$
	//# \also	$@Renderable::GetDepthOffsetPoint@$
	
	
	//# \div
	//# \function	Renderable::GetTransformable		Returns a pointer to the $@Utilities/Transformable@$ object.
	//
	//# \proto	const Transformable *GetTransformable(void) const;
	//
	//# \desc
	//# The $GetTransformable$ function returns a pointer to the $@Utilities/Transformable@$ object that determines the
	//# transformation from object space to world space. If the renderable object has no $@Utilities/Transformable@$ object
	//# assigned to it (the default state), then the return value is $nullptr$. In this case, the renderable object's vertices
	//# are assumed to exist in world space.
	//
	//# \also	$@Renderable::SetTransformable@$
	
	
	//# \function	Renderable::SetTransformable		Sets a pointer to the $@Utilities/Transformable@$ object.
	//
	//# \proto	void SetTransformable(const Transformable *transform);
	//
	//# \param	transform	A pointer to a $@Utilities/Transformable@$ object. Specifying $nullptr$ indicates that the
	//#						transformation from object space to world space is the identity transform.
	//
	//# \desc
	//# The $SetTransformable$ function sets a pointer to the $@Utilities/Transformable@$ object that determines the
	//# transformation from object space to world space. By default, a renderable object has no $@Utilities/Transformable@$
	//# object assigned to it, meaning that the object's vertices exist in world space.
	//
	//# \special
	//# The 3&nbsp;&times;&nbsp;3 rotation portion of the object-to-world transformation contained within the
	//# $@Utilities/Transformable@$ object should be orthogonal and should have a determinant of +1. That is, it must
	//# represent only a rotation and contain no scale, skew, or mirroring of any kind. Failure to meet this requirement
	//# will result in incorrect lighting for the renderable object.
	//
	//# \also	$@Renderable::GetTransformable@$
	
	
	//# \function	Renderable::GetTransparentAttachment		Returns the transparent attachment.
	//
	//# \proto	Renderable *GetTransparentAttachment(void) const;
	//
	//# \desc
	//# The $GetTransparentAttachment$ function returns the object that is attached to a renderable to enforce
	//# a particular transparency sorting order. For more information about transparent attachments, see the
	//# $@Renderable::SetTransparentAttachment@$ function.
	//# 
	//# Initially, the transparent attachment is $nullptr$.
	//
	//# \also	$@Renderable::SetTransparentAttachment@$
	//# \also	$@Renderable::GetTransparentPosition@$
	//# \also	$@Renderable::SetTransparentPosition@$
	
	
	//# \function	Renderable::SetTransparentAttachment		Sets the transparent attachment.
	//
	//# \proto	void SetTransparentAttachment(Renderable *attachment);
	//
	//# \param	attachment		The renderable object that will be attached.
	//
	//# \desc
	//# A renderable object may have another renderable object attached to it for the purpose of specifying a
	//# rendering order. The presence of an attachment qualifies a renderable object for transparent sorting, and an
	//# object is always rendered immediately before its attachment. This is useful when two transparent objects
	//# occupy the same space and one should always be behind the other.
	//# 
	//# The $SetTransparentAttachment$ function sets the object that is attached to a renderable. If the
	//# $attachment$ parameter is $nullptr$, then the attachment is cleared.
	//# 
	//# Initially, the transparent attachment is $nullptr$.
	//
	//# \also	$@Renderable::GetTransparentAttachment@$
	//# \also	$@Renderable::GetTransparentPosition@$
	//# \also	$@Renderable::SetTransparentPosition@$
	
	
	//# \function	Renderable::GetTransparentPosition		Returns the transparent position pointer.
	//
	//# \proto	const Point3D *GetTransparentPosition(void) const;
	//
	//# \desc
	//# The $GetTransparentPosition$ function returns the pointer to a renderable object's transparent
	//# position. This position is specified in world-space coordinates and represents the general location
	//# of a transparent object. Renderable objects having a transparent position are sorted and rendered
	//# from back to front with respect to the camera view direction.
	//# 
	//# If a renderable object has an attachment specified with the $@Renderable::SetTransparentAttachment@$
	//# function, then the transparent position is ignored, and the object is always rendered immediately
	//# before its attachment.
	//# 
	//# Initially, the transparent position is $nullptr$.
	//
	//# \also	$@Renderable::SetTransparentPosition@$
	//# \also	$@Renderable::GetTransparentAttachment@$
	//# \also	$@Renderable::SetTransparentAttachment@$
	
	
	//# \function	Renderable::SetTransparentPosition		Sets the transparent position pointer.
	//
	//# \proto	void SetTransparentPosition(const Point3D *position);
	//
	//# \param	position	A pointer to a world-space position.
	//
	//# \desc
	//# The $SetTransparentPosition$ function sets the pointer to a renderable object's transparent
	//# position. This position is specified in world-space coordinates and represents the general location
	//# of a transparent object. Renderable objects having a transparent position are sorted and rendered
	//# from back to front with respect to the camera view direction.
	//# 
	//# If a renderable object has an attachment specified with the $@Renderable::SetTransparentAttachment@$
	//# function, then the transparent position is ignored, and the object is always rendered immediately
	//# before its attachment.
	//# 
	//# Initially, the transparent position is $nullptr$.
	//
	//# \also	$@Renderable::SetTransparentPosition@$
	//# \also	$@Renderable::GetTransparentAttachment@$
	//# \also	$@Renderable::SetTransparentAttachment@$
	
	
	//# \div
	//# \function	Renderable::GetVertexCount		Returns the vertex count.
	//
	//# \proto	int32 GetVertexCount(void) const;
	//
	//# \desc
	//# 
	//
	//# \also	$@Renderable::SetVertexCount@$
	
	
	//# \function	Renderable::SetVertexCount		Sets the vertex count.
	//
	//# \proto	void SetVertexCount(int32 count);
	//
	//# \param	count	The vertex count.
	//
	//# \desc
	//# 
	//
	//# \also	$@Renderable::GetVertexCount@$
	
	
	//# \function	Renderable::GetAttributeArray		Returns one of the vertex attribute arrays.
	//
	//# \proto	const float *GetAttributeArray(int32 index) const;
	//# \proto	template <typename type> const type *GetAttributeArray(int32 index) const;
	//
	//# \param	index	The index of the attribute array.
	//
	//# \desc
	//# 
	//
	//# \also	$@Renderable::SetAttributeArray@$
	//# \also	$@Renderable::GetComponentCount@$
	
	
	//# \function	Renderable::SetAttributeArray		Sets one of the vertex attribute arrays.
	//
	//# \proto	void SetAttributeArray(int32 index, const float *array, int32 count = 1);
	//# \proto	void SetAttributeArray(int32 index, const Vector2D *array);
	//# \proto	void SetAttributeArray(int32 index, const Vector3D *array);
	//# \proto	void SetAttributeArray(int32 index, const Vector4D *array);
	//# \proto	void SetAttributeArray(int32 index, const ColorRGB *array);
	//# \proto	void SetAttributeArray(int32 index, const ColorRGBA *array);
	//
	//# \param	index	The index of the attribute array.
	//# \param	array	A pointer to the attribute array.
	//# \param	count	The number of components per vertex.
	//
	//# \desc
	//# 
	//
	//# \also	$@Renderable::GetAttributeArray@$
	//# \also	$@Renderable::GetComponentCount@$
	
	
	//# \function	Renderable::GetComponentCount		Returns the number of components used by one of the vertex attribute arrays.
	//
	//# \proto	char GetComponentCount(int32 index) const;
	//
	//# \param	index	The index of the attribute array.
	//
	//# \desc
	//# 
	//
	//# \also	$@Renderable::GetAttributeArray@$
	//# \also	$@Renderable::SetAttributeArray@$
	
	
	//# \function	Renderable::GetFaceCount		Returns the face count.
	//
	//# \proto	int32 GetFaceCount(void) const;
	//
	//# \desc
	//# 
	//
	//# \also	$@Renderable::SetFaceCount@$
	//# \also	$@Renderable::GetFaceArray@$
	//# \also	$@Renderable::SetFaceArray@$
	
	
	//# \function	Renderable::SetFaceCount		Sets the face count.
	//
	//# \proto	void SetFaceCount(int32 count);
	//
	//# \param	count	The face count.
	//
	//# \desc
	//# 
	//
	//# \also	$@Renderable::GetFaceCount@$
	//# \also	$@Renderable::GetFaceArray@$
	//# \also	$@Renderable::SetFaceArray@$
	
	
	//# \function	Renderable::GetFaceArray		Returns the face array.
	//
	//# \proto	const unsigned_int16 *GetFaceArray(void) const;
	//
	//# \desc
	//# 
	//
	//# \also	$@Renderable::SetFaceArray@$
	//# \also	$@Renderable::GetFaceCount@$
	
	
	//# \function	Renderable::SetFaceArray		Sets the face array.
	//
	//# \proto	void SetFaceArray(const unsigned_int16 *face);
	//
	//# \param	face	A pointer to the face array.
	//
	//# \desc
	//# 
	//
	//# \also	$@Renderable::GetFaceArray@$
	//# \also	$@Renderable::GetFaceCount@$
	
	
	//# \function	Renderable::GetOcclusionQuery		Returns the pointer to an occlusion query object.
	//
	//# \proto	OcclusionQuery *GetOcclusionQuery(void) const;
	//
	//# \desc
	//# 
	//
	//# \also	$@Renderable::SetOcclusionQuery@$
	//# \also	$@OcclusionQuery@$
	
	
	//# \function	Renderable::SetOcclusionQuery		Sets the pointer to an occlusion query object.
	//
	//# \proto	void SetOcclusionQuery(OcclusionQuery *query);
	//
	//# \param	query	The pointer to an occlusion query object.
	//
	//# \desc
	//# 
	//
	//# \also	$@Renderable::GetOcclusionQuery@$
	//# \also	$@OcclusionQuery@$
	
	
	//# \function	Renderable::GetFirstRenderSegment	Returns the first render segment belonging to a renderable object.
	//
	//# \proto	RenderSegment *GetFirstRenderSegment(void);
	//# \proto	const RenderSegment *GetFirstRenderSegment(void) const;
	//
	//# \desc
	//# The $GetFirstRenderSegment$ function returns the first render segment belonging to a renderable object.
	//# Every renderable object has at least one render segment. Additional render segments can be added to a
	//# renderable object using the $@RenderSegment::SetNextRenderSegment@$ function.
	//
	//# \also	$@RenderSegment@$
	//# \also	$@RenderSegment::GetNextRenderSegment@$
	//# \also	$@RenderSegment::SetNextRenderSegment@$
	
	
	//# \function	Renderable::InvalidateShaderData	Invalidates the shader data for all render segments belonging to a renderable object.
	//
	//# \proto	void InvalidateShaderData(void);
	//
	//# \desc
	//# The $InvalidateShaderData$ function causes the internal shader data for all render segments belonging to
	//# a renderable object to be discarded.
	//
	//# \also	$@RenderSegment::InvalidateShaderData@$
	
	
	class C4_API Renderable : public ListElement<Renderable>
	{
		friend class RenderSegment;
		friend class ShaderAttribute;
		
		private:
			
			RenderType					renderType;
			unsigned_int32				renderState;
			unsigned_int32				renderableFlags;
			unsigned_int32				shaderFlags;
			
			unsigned_int32				ambientBlendState;
			unsigned_int32				lightBlendState;
			
			const Transformable			*transformable;
			const Transform4D			*previousWorldTransform;
			
			const PaintEnvironment		*paintEnvironment;
			const AmbientEnvironment	*ambientEnvironment;
			const Box3D					*motionBlurBox;
			
			Renderable					*transparentAttachment;
			const Point3D				*transparentPosition;
			float						transparentDepth;
			
			float						depthOffsetDelta;
			const Point3D				*depthOffsetPoint;
			
			const VertexBuffer			*vertexBuffer[kVertexBufferCount];
			unsigned_int32				dynamicArrayFlags;
			
			int32						vertexCount;
			const float					*attributeArray[kMaxAttributeArrayCount];
			unsigned_int32				attributeOffset[kMaxAttributeArrayCount];
			char						componentCount[kMaxAttributeArrayCount];
			
			const unsigned_int16		*faceArray;
			unsigned_int32				faceOffset;
			
			const Vector4D				*renderParameter;
			const Vector4D				*texcoordParameter;
			const Vector4D				*terrainParameter;
			
			OcclusionQuery				*occlusionQuery;
			const ColorRGBA				*wireColor;
			
			int32						shaderDetailLevel;
			float						shaderDetailParameter;
			machine_address				groupKey[kMaxGroupKeyCount][kMaxShaderDetailLevelCount];
			
			RenderSegment				renderSegment;
			
			static const PaintEnvironment		nullPaintEnvironment;
			static const AmbientEnvironment	nullAmbientEnvironment;
			
			static const ConstVector4D		nullRenderParameterTable[kMaxRenderParameterCount];
			static const ConstVector4D		nullTexcoordParameterTable[kMaxTexcoordParameterCount];
			static const ConstVector4D		nullTerrainParameterTable[kMaxTerrainParameterCount];
			
			int32 SetShaderArray(ShaderData *data, int32 shaderIndex, int32 renderIndex) const;
			
			unsigned_int32 BuildVertexTransform(ShaderData *data, VertexAssembly *assembly) const;
			unsigned_int32 BuildTexcoord0Transform(const RenderSegment *segment, ShaderData *data, VertexAssembly *assembly, unsigned_int32 stateFlags) const;
			unsigned_int32 BuildTexcoord1Transform(const RenderSegment *segment, ShaderData *data, VertexAssembly *assembly, unsigned_int32 stateFlags) const;
			
			static void StateFunc_CopyCameraPosition(const Renderable *renderable, const void *cookie);
			static void StateFunc_CopyCameraDirections(const Renderable *renderable, const void *cookie);
			static void StateFunc_CopyCameraPositionAndDirections(const Renderable *renderable, const void *cookie);
			static void StateFunc_TransformCameraPosition(const Renderable *renderable, const void *cookie);
			static void StateFunc_TransformCameraDirections(const Renderable *renderable, const void *cookie);
			static void StateFunc_TransformCameraPositionAndDirections(const Renderable *renderable, const void *cookie);
			
			static void StateFunc_CopyCameraPosition4D(const Renderable *renderable, const void *cookie);
			static void StateFunc_TransformCameraPosition4D(const Renderable *renderable, const void *cookie);
			
			static void StateFunc_CopyCameraMatrix(const Renderable *renderable, const void *cookie);
			static void StateFunc_TransformCameraMatrix(const Renderable *renderable, const void *cookie);
			
			static void StateFunc_CopyWorldMatrix(const Renderable *renderable, const void *cookie);
			static void StateFunc_TransformWorldMatrix(const Renderable *renderable, const void *cookie);
			
			static void StateFunc_TransformTexcoord0(const Renderable *renderable, const void *cookie);
			static void StateFunc_AnimateTexcoord0(const Renderable *renderable, const void *cookie);
			static void StateFunc_TransformAnimateTexcoord0(const Renderable *renderable, const void *cookie);
			static void StateFunc_TransformTexcoord1(const Renderable *renderable, const void *cookie);
			static void StateFunc_AnimateTexcoord1(const Renderable *renderable, const void *cookie);
			static void StateFunc_TransformAnimateTexcoord1(const Renderable *renderable, const void *cookie);
			static void StateFunc_ScaleTerrainTexcoord(const Renderable *renderable, const void *cookie);
			
			static void StateFunc_GenerateTexcoord(const Renderable *renderable, const void *cookie);
			static void StateFunc_GenerateTransformTexcoord0(const Renderable *renderable, const void *cookie);
			static void StateFunc_GenerateAnimateTexcoord0(const Renderable *renderable, const void *cookie);
			static void StateFunc_GenerateTransformAnimateTexcoord0(const Renderable *renderable, const void *cookie);
			static void StateFunc_GenerateTransformTexcoord1(const Renderable *renderable, const void *cookie);
			static void StateFunc_GenerateAnimateTexcoord1(const Renderable *renderable, const void *cookie);
			static void StateFunc_GenerateTransformAnimateTexcoord1(const Renderable *renderable, const void *cookie);
			static void StateFunc_GenerateAnimateDualTexcoords(const Renderable *renderable, const void *cookie);
			
			static void StateFunc_ConfigureInfiniteLight(const Renderable *renderable, const void *cookie);
			static void StateFunc_ConfigureTransformInfiniteLight(const Renderable *renderable, const void *cookie);
			static void StateFunc_ConfigureDepthLight(const Renderable *renderable, const void *cookie);
			static void StateFunc_ConfigureTransformDepthLight(const Renderable *renderable, const void *cookie);
			static void StateFunc_ConfigureLandscapeLight(const Renderable *renderable, const void *cookie);
			static void StateFunc_ConfigureTransformLandscapeLight(const Renderable *renderable, const void *cookie);
			static void StateFunc_ConfigureLandscapeLightImpostor(const Renderable *renderable, const void *cookie);
			static void StateFunc_ConfigurePointLight(const Renderable *renderable, const void *cookie);
			static void StateFunc_ConfigureTransformPointLight(const Renderable *renderable, const void *cookie);
			static void StateFunc_ConfigureCubeLight(const Renderable *renderable, const void *cookie);
			static void StateFunc_ConfigureTransformCubeLight(const Renderable *renderable, const void *cookie);
			static void StateFunc_ConfigureSpotLight(const Renderable *renderable, const void *cookie);
			static void StateFunc_ConfigureTransformSpotLight(const Renderable *renderable, const void *cookie);
			
			static void StateFunc_CopyVertexScaleOffset(const Renderable *renderable, const void *cookie);
			static void StateFunc_CopyTerrainParameters(const Renderable *renderable, const void *cookie);
			static void StateFunc_CopyImpostorTransition(const Renderable *renderable, const void *cookie);
			static void StateFunc_CopyGeometryTransition(const Renderable *renderable, const void *cookie);
			static void StateFunc_TransformGeometryTransition(const Renderable *renderable, const void *cookie);
			
			static void StateFunc_CopyPaintSpace(const Renderable *renderable, const void *cookie);
			static void StateFunc_TransformPaintSpace(const Renderable *renderable, const void *cookie);
			
			static void StateFunc_SetOcclusionQuery(const Renderable *renderable, const void *cookie);
		
		public:
			
			Renderable(RenderType type, unsigned_int32 state = 0);
			virtual ~Renderable();
			
			RenderType GetRenderType(void) const
			{
				return (renderType);
			}
			
			void SetRenderType(RenderType type)
			{
				renderType = type;
			}
			
			unsigned_int32 GetRenderState(void) const
			{
				return (renderState);
			}
			
			void SetRenderState(unsigned_int32 state)
			{
				renderState = state;
			}
			
			unsigned_int32 GetRenderableFlags(void) const
			{
				return (renderableFlags);
			}
			
			void SetRenderableFlags(unsigned_int32 flags)
			{
				renderableFlags = flags;
			}
			
			unsigned_int32 GetShaderFlags(void) const
			{
				return (shaderFlags);
			}
			
			void SetShaderFlags(unsigned_int32 flags)
			{
				shaderFlags = flags;
			}
			
			unsigned_int32 GetAmbientBlendState(void) const
			{
				return (ambientBlendState);
			}
			
			void SetAmbientBlendState(unsigned_int32 state)
			{
				ambientBlendState = state;
			}
			
			unsigned_int32 GetLightBlendState(void) const
			{
				return (lightBlendState);
			}
			
			void SetLightBlendState(unsigned_int32 state)
			{
				lightBlendState = state;
			}
			
			const Transformable *GetTransformable(void) const
			{
				return (transformable);
			}
			
			void SetTransformable(const Transformable *transform)
			{
				transformable = transform;
			}
			
			const Transform4D *GetPreviousWorldTransformPointer(void) const
			{
				return (previousWorldTransform);
			}
			
			void SetPreviousWorldTransformPointer(const Transform4D *transform)
			{
				previousWorldTransform = transform;
			}
			
			const PaintEnvironment *GetPaintEnvironment(void) const
			{
				return (paintEnvironment);
			}
			
			void SetPaintEnvironment(const PaintEnvironment *environment)
			{
				paintEnvironment = environment;
			}
			
			void SetNullPaintEnvironment(void)
			{
				paintEnvironment = &nullPaintEnvironment;
			}
			
			const AmbientEnvironment *GetAmbientEnvironment(void) const
			{
				return (ambientEnvironment);
			}
			
			void SetAmbientEnvironment(const AmbientEnvironment *environment)
			{
				ambientEnvironment = environment;
			}
			
			void SetNullAmbientEnvironment(void)
			{
				ambientEnvironment = &nullAmbientEnvironment;
			}
			
			const Box3D *GetMotionBlurBox(void) const
			{
				return (motionBlurBox);
			}
			
			void SetMotionBlurBox(const Box3D *box)
			{
				motionBlurBox = box;
			}
			
			Renderable *GetTransparentAttachment(void) const
			{
				return (transparentAttachment);
			}
			
			void SetTransparentAttachment(Renderable *attachment)
			{
				transparentAttachment = attachment;
			}
			
			const Point3D *GetTransparentPosition(void) const
			{
				return (transparentPosition);
			}
			
			void SetTransparentPosition(const Point3D *position)
			{
				transparentPosition = position;
			}
			
			float GetTransparentDepth(void) const
			{
				return (transparentDepth);
			}
			
			void SetTransparentDepth(float depth)
			{
				transparentDepth = depth;
			}
			
			float GetDepthOffsetDelta(void) const
			{
				return (depthOffsetDelta);
			}
			
			const Point3D& GetDepthOffsetPoint(void) const
			{
				return (*depthOffsetPoint);
			}
			
			void SetDepthOffset(float delta, const Point3D *point)
			{
				depthOffsetDelta = delta;
				depthOffsetPoint = point;
			}
			
			const VertexBuffer *GetVertexBuffer(unsigned_int32 index) const
			{
				return (vertexBuffer[index]);
			}
			
			void SetVertexBuffer(unsigned_int32 index, const VertexBuffer *buffer)
			{
				vertexBuffer[index] = buffer;
			}
			
			unsigned_int32 GetDynamicArrayFlags(void) const
			{
				return (dynamicArrayFlags);
			}
			
			void SetDynamicArrayFlags(unsigned_int32 flags)
			{
				dynamicArrayFlags = flags;
			}
			
			int32 GetVertexCount(void) const
			{
				return (vertexCount);
			}
			
			void SetVertexCount(int32 count)
			{
				vertexCount = count;
			}
			
			const float *GetAttributeArray(int32 index) const
			{
				return (attributeArray[index]);
			}
			
			template <typename type> const type *GetAttributeArray(int32 index) const
			{
				return (reinterpret_cast<const type *>(attributeArray[index]));
			}
			
			void SetAttributeArray(int32 index, const float *array, int32 count = 1)
			{
				componentCount[index] = count;
				attributeArray[index] = array;
			}
			
			void SetAttributeArray(int32 index, const Vector2D *array)
			{
				componentCount[index] = 2;
				attributeArray[index] = &array->x;
			}
			
			void SetAttributeArray(int32 index, const Vector3D *array)
			{
				componentCount[index] = 3;
				attributeArray[index] = &array->x;
			}
			
			void SetAttributeArray(int32 index, const Vector4D *array)
			{
				componentCount[index] = 4;
				attributeArray[index] = &array->x;
			}
			
			void SetAttributeArray(int32 index, const ColorRGB *array)
			{
				componentCount[index] = 3;
				attributeArray[index] = &array->red;
			}
			
			void SetAttributeArray(int32 index, const ColorRGBA *array)
			{
				componentCount[index] = 4;
				attributeArray[index] = &array->red;
			}
			
			unsigned_int32 GetAttributeOffset(int32 index) const
			{
				return (attributeOffset[index]);
			}
			
			void SetAttributeOffset(int32 index, unsigned_int32 offset)
			{
				attributeOffset[index] = offset;
			}
			
			void SetAttributeOffset(int32 index, unsigned_int32 offset, int32 count)
			{
				attributeOffset[index] = offset;
				componentCount[index] = count;
			}
			
			char GetComponentCount(int32 index) const
			{
				return (componentCount[index]);
			}
			
			bool AttributeArrayEnabled(int32 index) const
			{
				return (componentCount[index] != 0);
			}
			
			bool TangentAvailable(void) const
			{
				return ((AttributeArrayEnabled(kArrayTangent)) || (shaderFlags & kShaderGenerateTangent));
			}
			
			const unsigned_int16 *GetFaceArray(void) const
			{
				return (faceArray);
			}
			
			void SetFaceArray(const unsigned_int16 *face)
			{
				faceArray = face;
			}
			
			const Line *GetLineArray(void) const
			{
				return (reinterpret_cast<const Line *>(faceArray));
			}
			
			void SetLineArray(const Line *line)
			{
				faceArray = line->index;
			}
			
			void SetLineArray(int32 count, const Line *line)
			{
				renderSegment.SetFaceRange(0, count);
				faceArray = line->index;
			}
			
			const Triangle *GetTriangleArray(void) const
			{
				return (reinterpret_cast<const Triangle *>(faceArray));
			}
			
			void SetTriangleArray(const Triangle *triangle)
			{
				faceArray = triangle->index;
			}
			
			void SetTriangleArray(int32 count, const Triangle *triangle)
			{
				renderSegment.SetFaceRange(0, count);
				faceArray = triangle->index;
			}
			
			const Quad *GetQuadArray(void) const
			{
				return (reinterpret_cast<const Quad *>(faceArray));
			}
			
			void SetQuadArray(const Quad *quad)
			{
				faceArray = quad->index;
			}
			
			void SetQuadArray(int32 count, const Quad *quad)
			{
				renderSegment.SetFaceRange(0, count);
				faceArray = quad->index;
			}
			
			unsigned_int32 GetFaceOffset(void) const
			{
				return (faceOffset);
			}
			
			void SetFaceOffset(unsigned_int32 offset)
			{
				faceOffset = offset;
			}
			
			const Vector4D *GetRenderParameterPointer(void) const
			{
				return (renderParameter);
			}
			
			void SetRenderParameterPointer(const Vector4D *param)
			{
				renderParameter = param;
			}
			
			void SetNullRenderParameterPointer(void)
			{
				renderParameter = &nullRenderParameterTable[0];
			}
			
			const Vector4D *GetTexcoordParameterPointer(void) const
			{
				return (texcoordParameter);
			}
			
			void SetTexcoordParameterPointer(const Vector4D *param)
			{
				texcoordParameter = param;
			}
			
			void SetNullTexcoordParameterPointer(void)
			{
				texcoordParameter = &nullTexcoordParameterTable[0];
			}
			
			const Vector4D *GetTerrainParameterPointer(void) const
			{
				return (terrainParameter);
			}
			
			void SetTerrainParameterPointer(const Vector4D *param)
			{
				terrainParameter = param;
			}
			
			void SetNullTerrainParameterPointer(void)
			{
				terrainParameter = &nullTerrainParameterTable[0];
			}
			
			OcclusionQuery *GetOcclusionQuery(void) const
			{
				return (occlusionQuery);
			}
			
			void SetOcclusionQuery(OcclusionQuery *query)
			{
				occlusionQuery = query;
			}
			
			const ColorRGBA *GetWireframeColorPointer(void) const
			{
				return (wireColor);
			}
			
			void SetWireframeColorPointer(const ColorRGBA *color)
			{
				wireColor = color;
			}
			
			int32 GetShaderDetailLevel(void) const
			{
				return (shaderDetailLevel);
			}
			
			void SetShaderDetailLevel(int32 level)
			{
				shaderDetailLevel = Min(level, kMaxShaderDetailLevelCount - 1);
			}
			
			float GetShaderDetailParameter(void) const
			{
				return (shaderDetailParameter);
			}
			
			void SetShaderDetailParameter(float parameter)
			{
				shaderDetailParameter = parameter;
			}
			
			int32 GetFaceCount(void) const
			{
				return (renderSegment.GetFaceCount());
			}
			
			void SetFaceCount(int32 count)
			{
				renderSegment.SetFaceRange(0, count);
			}
			
			MaterialObject *const *GetMaterialObjectPointer(void) const
			{
				return (renderSegment.GetMaterialObjectPointer());
			}
			
			void SetMaterialObjectPointer(MaterialObject *const *object)
			{
				renderSegment.SetMaterialObjectPointer(object);
			}
			
			List<Attribute> *GetMaterialAttributeList(void) const
			{
				return (renderSegment.GetMaterialAttributeList());
			}
			
			void SetMaterialAttributeList(List<Attribute> *list)
			{
				renderSegment.SetMaterialAttributeList(list);
			}
			
			machine_address GetGroupKey(int32 index) const
			{
				return (groupKey[index][shaderDetailLevel]);
			}
			
			RenderSegment *GetFirstRenderSegment(void)
			{
				return (&renderSegment);
			}
			
			const RenderSegment *GetFirstRenderSegment(void) const
			{
				return (&renderSegment);
			}
			
			void InvalidateShaderData(void);
			void InvalidateAmbientShaderData(void);
	};
	
	
	class C4_API StencilShadow
	{
		protected:
			
			Polyhedron			*shadowPolyhedron;
			
			int32				geometryVertexCount;
			int32				extrusionEdgeCount;
			
			int32				frontEndcapTriangleCount;
			int32				backEndcapTriangleCount;
			
			const Point3D		*geometryVertexArray;
			Vector4D			*extrusionVertexArray;
			
			Triangle			*frontEndcapTriangleArray;
			Triangle			*backEndcapTriangleArray;
		
		public:
			
			Polyhedron *GetShadowPolyhedron(void) const
			{
				return (shadowPolyhedron);
			}
			
			int32 GetGeometryVertexCount(void) const
			{
				return (geometryVertexCount);
			}
			
			int32 GetExtrusionEdgeCount(void) const
			{
				return (extrusionEdgeCount);
			}
			
			void SetExtrusionEdgeCount(int32 count)
			{
				extrusionEdgeCount = count;
			}
			
			int32 GetFrontEndcapTriangleCount(void) const
			{
				return (frontEndcapTriangleCount);
			}
			
			void SetFrontEndcapTriangleCount(int32 count)
			{
				frontEndcapTriangleCount = count;
			}
			
			int32 GetBackEndcapTriangleCount(void) const
			{
				return (backEndcapTriangleCount);
			}
			
			void SetBackEndcapTriangleCount(int32 count)
			{
				backEndcapTriangleCount = count;
			}
			
			const Point3D *GetGeometryVertexArray(void) const
			{
				return (geometryVertexArray);
			}
			
			void SetGeometryVertexArray(int32 count, const Point3D *vertex)
			{
				geometryVertexCount = count;
				geometryVertexArray = vertex;
			}
			
			Vector4D *GetExtrusionVertexArray(void) const
			{
				return (extrusionVertexArray);
			}
			
			Triangle *GetFrontEndcapTriangleArray(void) const
			{
				return (frontEndcapTriangleArray);
			}
			
			Triangle *GetBackEndcapTriangleArray(void) const
			{
				return (backEndcapTriangleArray);
			}
			
			void SetBackEndcapTriangleArray(Triangle *triangle)
			{
				backEndcapTriangleArray = triangle;
			}
	};
}


#endif

// ZYURVUR
