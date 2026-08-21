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


#ifndef C4GraphicsMgr_h
#define C4GraphicsMgr_h


//# \component	Graphics Manager
//# \prefix		GraphicsMgr/


#define C4CREATE_DEBUG_CONTEXT		0


#include "C4Variables.h"
#include "C4Renderable.h"
#include "C4VertexPrograms.h"
#include "C4CameraObjects.h"
#include "C4LightObjects.h"


namespace C4
{
	typedef EngineResult GraphicsResult;
	
	
	enum
	{
		kGraphicsOkay				= kEngineOkay,
		kGraphicsFormatFailed		= (kManagerGraphics << 16) | 0x0001,
		kGraphicsContextFailed		= (kManagerGraphics << 16) | 0x0002,
		kGraphicsNoHardware			= (kManagerGraphics << 16) | 0x0003
	};
	
	
	enum
	{
		kExtensionColorBufferFloat,
		kExtensionConservativeDepth,
		kExtensionDebugOutput,
		kExtensionDepthClamp,
		kExtensionDepthTexture,
		kExtensionFragmentProgram,
		kExtensionFragmentProgramShadow,
		kExtensionFragmentShader,
		kExtensionFramebufferObject,
		kExtensionFramebufferBlit,
		kExtensionFramebufferMultisample,
		kExtensionFramebufferSRGB,
		kExtensionGetProgramBinary,
		kExtensionHalfFloatPixel,
		kExtensionInstancedArrays,
		kExtensionMultisample,
		kExtensionMultitexture,
		kExtensionOcclusionQuery,
		kExtensionPackedDepthStencil,
		kExtensionPixelBufferObject,
		kExtensionPointSprite,
		kExtensionSampleShading,
		kExtensionSeamlessCubeMap,
		kExtensionSeparateShaderObjects,
		kExtensionShadow,
		kExtensionShaderObjects,
		kExtensionShaderTextureLod,
		kExtensionTessellationShader,
		kExtensionTextureArray,
		kExtensionTextureBorderClamp,
		kExtensionTextureCompression,
		kExtensionTextureCompressionRGTC,
		kExtensionTextureCubeMap,
		kExtensionTextureFloat,
		kExtensionTextureMirroredRepeat,
		kExtensionTextureRectangle,
		kExtensionTimerQuery,
		kExtensionUniformBufferObject,
		kExtensionVertexBufferObject,
		kExtensionVertexProgram,
		kExtensionBlendColor,
		kExtensionBlendMinmax,
		kExtensionBlendSubtract,
		kExtensionBlendFuncSeparate,
		kExtensionDepthBoundsTest,
		kExtensionDirectStateAccess,
		kExtensionDrawRangeElements,
		kExtensionGpuShader4,
		kExtensionMultiDrawArrays,
		kExtensionPackedPixels,
		kExtensionStencilWrap,
		kExtensionTexture3D,
		kExtensionTextureCompressionS3TC,
		kExtensionTextureEdgeClamp,
		kExtensionTextureFilterAnisotropic,
		kExtensionTextureMirrorClamp,
		kExtensionTextureSRGB,
		kExtensionConditionalRender,
		kExtensionExplicitMultisample,
		kExtensionFragmentProgram2,
		kExtensionFramebufferMultisampleCoverage,
		kExtensionGpuProgram4,
		kExtensionShaderBufferLoad,
		kExtensionTransformFeedback,
		kExtensionVertexBufferUnifiedMemory,
		kGraphicsExtensionCount
	};
	 
	
	enum 
	{ 
		kCapabilityOcclusionQuery, 
		kCapabilityTextureCompressionS3TC,
		kCapabilityFramebufferSRGB, 
		kCapabilityFramebufferFloat,
		kCapabilityTextureArray,
		kCapabilityProgramTextureArray,
		kCapabilityShaderTextureArray, 
		kCapabilityUnifiedMemory,
		kGraphicsCapabilityCount
	};
	 
	
	#if C4WINDOWS
	
		enum
		{
			kWindowSystemExtensionCreateContext,
			kWindowSystemExtensionPixelFormat,
			kWindowSystemExtensionSwapControl,
			kWindowSystemExtensionSwapControlTear,
			kWindowSystemExtensionCount
		};
	
	#elif C4LINUX
	
		enum
		{
			kWindowSystemExtensionSwapControl,
			kWindowSystemExtensionSwapControlTear,
			kWindowSystemExtensionCount
		};
	
	#endif
	
	
	enum
	{
		kRenderOptionNormalizeBumps			= 1 << 0,
		kRenderOptionParallaxMapping		= 1 << 1,
		kRenderOptionHorizonMapping			= 1 << 2,
		kRenderOptionTerrainBumps			= 1 << 3,
		kRenderOptionStructureEffects		= 1 << 4,
		kRenderOptionMotionBlur				= 1 << 5,
		kRenderOptionDistortion				= 1 << 6,
		kRenderOptionGlowBloom				= 1 << 7
	};
	
	
	enum
	{
		kStructureClearBuffer				= 1 << 0,
		kStructureZeroBackgroundVelocity	= 1 << 1,
		kStructureRenderVelocity			= 1 << 2,
		kStructureRenderDepth				= 1 << 3,
		kStructureRenderGradient			= 1 << 4
	};
	
	
	enum
	{
		kWireframeColor			= 1 << 0,
		kWireframeTwoSided		= 1 << 1,
		kWireframeDepthTest		= 1 << 2
	};
	
	
	enum
	{
		kHardwareUnknown		= 0,
		kHardwareNV				= 1,
		kHardwareATI			= 2
	};
	
	
	enum
	{
		kProcessGridWidth		= 16,
		kProcessGridHeight		= 12
	};
	
	
	enum
	{
		kTextureUnitAmbientSpace1				= 14,
		kTextureUnitAmbientSpace2				= 15,
		kTextureUnitLightProjection				= 15
	};
	
	
	#define TEXTURE_UNIT_AMBIENT_SPACE1			"14"
	#define TEXTURE_UNIT_AMBIENT_SPACE2			"15"
	#define TEXTURE_UNIT_LIGHT_PROJECTION		"15"
	
	
	enum
	{
		kDiagnosticWireframe			= 1 << 0,
		kDiagnosticDepthTest			= 1 << 1,
		kDiagnosticNormals				= 1 << 2,
		kDiagnosticTangents				= 1 << 3,
		kDiagnosticShadows				= 1 << 4,
		kDiagnosticShadowBounds			= 1 << 5,
		kDiagnosticTimer				= 1 << 6
	};
	
	
	enum
	{
		kGraphicsCounterDirectVertices,
		kGraphicsCounterDirectPrimitives,
		kGraphicsCounterDirectCommands,
		kGraphicsCounterShadowVertices,
		kGraphicsCounterShadowPrimitives,
		kGraphicsCounterShadowCommands,
		kGraphicsCounterStencilVertices,
		kGraphicsCounterStencilPrimitives,
		kGraphicsCounterStencilCommands,
		kGraphicsCounterVelocityVertices,
		kGraphicsCounterVelocityPrimitives,
		kGraphicsCounterVelocityCommands,
		kGraphicsCounterDistortionVertices,
		kGraphicsCounterDistortionPrimitives,
		kGraphicsCounterDistortionCommands,
		kGraphicsCounterStencilClears,
		kGraphicsCounterCount
	};
	
	
	enum RenderTargetMode
	{
		kRenderTargetFrameBufferObject,
		kRenderTargetCopyTexture
	};
	
	
	enum RenderTargetType
	{
		kRenderTargetDisplay = -1,
		kRenderTargetPrimary,
		kRenderTargetReflection,
		kRenderTargetRefraction,
		kRenderTargetGlowBloom = kRenderTargetReflection,
		kRenderTargetDistortion = kRenderTargetRefraction,
		kRenderTargetStructure,
		kRenderTargetCount
	};
	
	
	enum StencilType
	{
		kStencilInfiniteExtrusion,
		kStencilPointExtrusion,
		kStencilEndcapProjection,
		kStencilEndcapIdentity,
		kStencilTypeCount
	};
	
	
	enum StencilMode
	{
		kStencilNone,
		kStencilPass,
		kStencilFail,
		kStencilDark
	};
	
	
	enum AmbientMode
	{
		kAmbientNormal,
		kAmbientBright,
		kAmbientDark
	};
	
	
	const float kShaderTimePeriod = 120000.0F;
	const float kInverseShaderTimePeriod = 1.0F / kShaderTimePeriod;
	
	
	class FogSpaceObject;
	
	
	struct GraphicsCapabilities
	{
		unsigned_int16		hardwareType;
		unsigned_int16		hardwareSpeed;
		unsigned_int32		openglVersion;
		
		bool				extensionFlag[kGraphicsExtensionCount];
		bool				capabilityFlag[kGraphicsCapabilityCount];
		
		#if C4WINDOWS || C4LINUX
		
			bool			windowSystemExtensionFlag[kWindowSystemExtensionCount];
		
		#endif
		
		int32				maxTextureSize;
		int32				max3DTextureSize;
		int32				maxCubeTextureSize;
		int32				maxArrayTextureLayers;
		float				maxTextureAnisotropy;
		float				maxTextureLodBias;
		
		int32				maxColorAttachments;
		int32				maxRenderbufferSize;
		int32				maxMultisampleSamples;
		
		int32				maxTransformFeedbackInterleavedComponents;
		int32				maxTransformFeedbackSeparateComponents;
		int32				maxTransformFeedbackSeparateAttribs;
		
		int32				maxTextureCoordCount;
		int32				maxTextureImageCount;
		
		int32				maxVertexProgramInstructionCount;
		int32				maxVertexProgramTemporaryCount;
		int32				maxVertexProgramAttributeCount;
		int32				maxVertexProgramParameterCount;
		int32				maxVertexProgramAddressRegisterCount;
		
		int32				maxFragmentProgramALUInstructionCount;
		int32				maxFragmentProgramTEXInstructionCount;
		int32				maxFragmentProgramTEXIndirectionCount;
		int32				maxFragmentProgramTemporaryCount;
		int32				maxFragmentProgramParameterCount;
		
		int32				maxGeometryProgramOutputVertexCount;
		int32				maxGeometryProgramOutputComponentCount;
		int32				maxGeometryProgramTextureImageCount;
		
		int32				maxVertexProgramResultComponentCount;
		int32				maxGeometryProgramResultComponentCount;
	};
	
	
	struct GraphicsExtensionData
	{
		const char			*name1;
		const char			*name2;
		unsigned_int16		version;
		bool				required;
		mutable bool		enabled;
	};
	
	
	#if C4WINDOWS || C4LINUX
	
		struct WindowSystemExtensionData
		{
			const char		*name;
			mutable bool	enabled;
		};
	
	#endif
	
	
	#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#endif //]
	
	
	class C4_API NormalFrameBuffer : public Render::FrameBufferObject
	{
		private:
			
			unsigned_int32					renderTargetMask;
			Render::TextureObject			textureObject[kRenderTargetCount];
			
			Render::RenderBufferObject		depthRenderBuffer;
		
		public:
			
			NormalFrameBuffer(int32 width, int32 height, unsigned_int32 mask);
			~NormalFrameBuffer();
			
			unsigned_int32 GetRenderTargetMask(void) const
			{
				return (renderTargetMask);
			}
			
			const Render::TextureObject *GetRenderTargetTexture(int32 target) const
			{
				return (&textureObject[target]);
			}
	};
	
	
	class C4_API ShadowFrameBuffer : public Render::FrameBufferObject
	{
		private:
			
			Render::TextureObject			textureObject;
		
		public:
			
			ShadowFrameBuffer(int32 width, int32 height);
			~ShadowFrameBuffer();
			
			Render::TextureObject *GetRenderTargetTexture(void)
			{
				return (&textureObject);
			}
			
			void SetTextureAnisotropy(int32 anisotropy);
	};
	
	
	class C4_API MultisampleFrameBuffer : public Render::FrameBufferObject
	{
		private:
			
			Render::RenderBufferObject		colorRenderBuffer;
			Render::RenderBufferObject		depthRenderBuffer;
			
			float							sampleDivider;
		
		public:
			
			MultisampleFrameBuffer(int32 width, int32 height, int32 sampleCount);
			~MultisampleFrameBuffer();
			
			float GetSampleDivider(void) const
			{
				return (sampleDivider);
			}
	};
	
	
	//# \class	GraphicsMgr		The Graphics Manager class.
	//
	//# \def	class GraphicsMgr : public Manager<GraphicsMgr>
	//
	//# \desc
	//# The $GraphicsMgr$ class encapsulates the 3D graphics rendering facilities of the C4 Engine.
	//# The single instance of the Graphics Manager is constructed during an application's initialization
	//# and destroyed at termination.
	//# 
	//# The Graphics Manager's member functions are accessed through the global pointer $TheGraphicsMgr$.
	//
	//# \also	$@Renderable@$
	
	
	//# \function	GraphicsMgr::GetCameraObject	Returns the current camera object.
	//
	//# \proto	const CameraObject *GetCameraObject(void) const;
	//
	//# \desc
	//# 
	//
	//# \also	$@GraphicsMgr::GetCameraTransformable@$
	//# \also	$@GraphicsMgr::SetCamera@$
	
	
	//# \function	GraphicsMgr::GetCameraTransformable		Returns the current camera transform.
	//
	//# \proto	const Transformable *GetCameraTransformable(void) const;
	//
	//# \desc
	//# 
	//
	//# \also	$@GraphicsMgr::GetCameraObject@$
	//# \also	$@GraphicsMgr::SetCamera@$
	//# \also	$@Utilities/Transformable@$
	
	
	//# \function	GraphicsMgr::SetCamera		Sets the current camera.
	//
	//# \proto	void SetCamera(const CameraObject *camera, const Transformable *transformable);
	//
	//# \param	camera			A pointer to the camera object.
	//# \param	transformable	A pointer to the camera's transform. This can be $nullptr$.
	//
	//# \desc
	//# 
	//
	//# \also	$@GraphicsMgr::GetCameraObject@$
	//# \also	$@GraphicsMgr::GetCameraTransformable@$
	
	
	//# \function	GraphicsMgr::GetLightObject		Returns the current light object.
	//
	//# \proto	const LightObject *GetLightObject(void) const;
	//
	//# \desc
	//# 
	//
	//# \also	$@GraphicsMgr::GetLightTransformable@$
	//# \also	$@GraphicsMgr::SetLight@$
	//# \also	$@GraphicsMgr::SetAmbient@$
	
	
	//# \function	GraphicsMgr::GetLightTransformable		Returns the current light transform.
	//
	//# \proto	const Transformable *GetLightTransformable(void) const;
	//
	//# \desc
	//# 
	//
	//# \also	$@GraphicsMgr::GetLightObject@$
	//# \also	$@GraphicsMgr::SetLight@$
	//# \also	$@GraphicsMgr::SetAmbient@$
	
	
	//# \function	GraphicsMgr::SetLight		Sets the current light.
	//
	//# \proto	bool SetLight(const LightObject *light, const Transformable *transformable);
	//
	//# \param	light			A pointer to the light object.
	//# \param	transformable	A pointer to the light's transform. This cannot be $nullptr$.
	//
	//# \desc
	//# 
	//
	//# \also	$@GraphicsMgr::GetLightObject@$
	//# \also	$@GraphicsMgr::GetLightTransformable@$
	
	
	//# \function	GraphicsMgr::SetAmbient		Sets the lighting mode to ambient.
	//
	//# \proto	ShaderType SetAmbient(unsigned_int32 flags = 0);
	//
	//# \param	flags		Used internally. Always set to 0.
	//
	//# \desc
	//# 
	//
	//# \also	$@GraphicsMgr::SetLight@$
	
	
	//# \function	GraphicsMgr::BeginRendering		Begins the process of rendering a frame.
	//
	//# \proto	void BeginRendering(void);
	//
	//# \desc
	//# 
	//
	//# \also	$@GraphicsMgr::EndRendering@$
	
	
	//# \function	GraphicsMgr::EndRendering		Ends the process of rendering a frame and displays the final image.
	//
	//# \proto	void EndRendering(void);
	//
	//# \desc
	//# 
	//
	//# \also	$@GraphicsMgr::BeginRendering@$
	
	
	//# \function	GraphicsMgr::DrawRenderList		Draws a list of renderable objects.
	//
	//# \proto	void DrawRenderList(const List<Renderable> *renderList);
	//
	//# \desc
	//# 
	//
	//# \also	$@Renderable@$
	
	
	class C4_API GraphicsMgr : public Manager<GraphicsMgr>
	{
		private:
			
			#if C4WINDOWS
			
				HDC								deviceContext;
				HGLRC							openglContext;
				
			#elif C4MACOS
			
				AGLContext						openglContext;
				CFBundleRef						openglBundle;
			
			#elif C4LINUX
			
				::Display						*openglDisplay;
				::Colormap						openglColormap;
				::Window						openglWindow;
				GLXContext						openglContext;
			
			#endif
			
			#if C4OPENGL
			
				const char						*extensionsString;
			
				#if C4WINDOWS || C4LINUX
				
					const char					*windowSystemExtensionsString;
				
				#endif
			
			#endif
			
			GraphicsCapabilities				capabilities;
			
			unsigned_int32						driverVersion;
			
			int32								dynamicShadowMapSize;
			unsigned_int32						targetDisableMask;
			
			int32								frameCount;
			Render::QueryObject					timerQuery[4];
			
			const CameraObject					*cameraObject;
			const Transformable					*cameraTransformable;
			Vector4D							cameraPosition4D;
			Point3D								directCameraPosition;
			Transform4D							cameraSpaceTransform;
			Transform4D							previousCameraSpaceTransform;
			
			const CameraObject					*savedCameraObject;
			const Transformable					*savedCameraTransformable;
			
			const FogSpaceObject				*fogSpaceObject;
			const Transformable					*fogSpaceTransformable;
			Antivector4D						worldFogPlane;
			
			const LightObject					*lightObject;
			const Transformable					*lightTransformable;
			const LightShadowData				*lightShadowData;
			
			const Transformable					*geometryTransformable;
			Vector4D							geometryLightPosition;
			
			unsigned_int32						currentBlendState;
			StencilMode							currentStencilMode;
			
			unsigned_int32						currentGraphicsState;
			unsigned_int32						currentRenderState;
			unsigned_int32						disabledRenderState;
			
			unsigned_int32						currentMaterialState;
			ShaderType							currentShaderType;
			ShaderVariant						currentShaderVariant;
			AmbientMode							currentAmbientMode;
			
			float								currentNearDepth;
			unsigned_int32						currentFrustumFlags;
			
			Matrix4D							cameraProjectionMatrix;
			Matrix4D							standardProjectionMatrix;
			Matrix4D							currentProjectionMatrix;
			Matrix4D							currentMVPMatrix;
			
			unsigned_int32						currentStructureFlags;
			float								renderTargetOffsetSize;
			float								depthOffsetConstant;
			
			unsigned_int32						colorTransformFlags;
			float								brightnessMultiplier;
			ColorRGBA							finalColorScale[3];
			ColorRGBA							finalColorBias;
			
			Rect								clipRect;
			Rect								viewportRect;
			Rect								cameraRect;
			Rect								scissorRect;
			Rect								lightRect;
			Rect								shadowRect;
			Range<float>						lightDepthBounds;
			Point2D								lightVertex[4];
			
			unsigned_int32						currentArrayState;
			const VertexBuffer					*currentVertexBuffer[kMaxShaderArrayCount];
			const float							*shaderArrayPointer[kMaxShaderArrayCount];
			
			OcclusionQuery						*currentOcclusionQuery;
			List<OcclusionQuery>				occlusionQueryList;
			
			Antivector4D						distortionPlane;
			float								occlusionAreaNormalizer;
			
			float								motionBlurBoxLeftOffset;
			float								motionBlurBoxRightOffset;
			float								motionBlurBoxBottomOffset;
			float								motionBlurBoxTopOffset;
			
			Texture								*nullTexture;
			
			unsigned_int32						renderOptionFlags;
			int32								textureDetailLevel;
			int32								paletteDetailLevel;
			int32								textureFilterAnisotropy;
			
			RenderTargetType					currentRenderTargetType;
			int32								renderTargetHeight;
			
			Render::FrameBufferObject			genericFrameBuffer;
			NormalFrameBuffer					*normalFrameBuffer;
			ShadowFrameBuffer					*shadowFrameBuffer;
			MultisampleFrameBuffer				*multisampleFrameBuffer;
			
			Point2D								processGridVertex[(kProcessGridWidth + 1) * (kProcessGridHeight + 1)];
			Quad								processGridQuad[kProcessGridWidth * kProcessGridHeight];
			bool								motionGridFlag[kProcessGridWidth * kProcessGridHeight];
			
			unsigned_int32						diagnosticFlags;
			int32								graphicsCounter[kGraphicsCounterCount];

			VariableObserver<GraphicsMgr>		textureDetailLevelObserver;
			VariableObserver<GraphicsMgr>		paletteDetailLevelObserver;
			VariableObserver<GraphicsMgr>		textureAnisotropyObserver;
			VariableObserver<GraphicsMgr>		renderNormalizeBumpsObserver;
			VariableObserver<GraphicsMgr>		renderParallaxMappingObserver;
			VariableObserver<GraphicsMgr>		renderHorizonMappingObserver;
			VariableObserver<GraphicsMgr>		renderTerrainBumpsObserver;
			VariableObserver<GraphicsMgr>		renderStructureEffectsObserver;
			VariableObserver<GraphicsMgr>		postBrightnessObserver;
			VariableObserver<GraphicsMgr>		postMotionBlurObserver;
			VariableObserver<GraphicsMgr>		postDistortionObserver;
			VariableObserver<GraphicsMgr>		postGlowBloomObserver;
			
			static GraphicsExtensionData		extensionData[kGraphicsExtensionCount];
			
			#if C4WINDOWS || C4LINUX
			
				static WindowSystemExtensionData	windowSystemExtensionData[kWindowSystemExtensionCount];
				
				#if C4WINDOWS
				
					void InitializeWglExtensions(PIXELFORMATDESCRIPTOR *formatDescriptor);
					static LRESULT CALLBACK WglWindowProc(HWND window, UINT message, WPARAM wparam, LPARAM lparam);
				
				#elif C4LINUX
				
					void InitializeGlxExtensions(void);
				
				#endif
			
			#endif
			
			#if C4CREATE_DEBUG_CONTEXT
			
				static void OPENGLAPI DebugCallback(GLenum source, GLenum type, GLuint id, GLenum severity, GLsizei length, const GLchar *message, void *userParam);
			
			#endif
			
			static void LogExtensions(const char *string);
			void UpdateLog(void) const;
			
			GraphicsResult InitializeGraphicsContext(int32 displayWidth, int32 displayHeight, unsigned_int32 displayFlags);
			void TerminateGraphicsContext(void);
			
			void DetermineHardwareType(void);
			bool InitializeOpenglExtensions(void);
			
			void InitializeProcessGrid(void);
			
			unsigned_int32 InitializeVariables(void);
			void HandleTextureDetailLevelEvent(Variable *variable);
			void HandlePaletteDetailLevelEvent(Variable *variable);
			void HandleTextureAnisotropyEvent(Variable *variable);
			void HandleRenderNormalizeBumpsEvent(Variable *variable);
			void HandleRenderParallaxMappingEvent(Variable *variable);
			void HandleRenderHorizonMappingEvent(Variable *variable);
			void HandleRenderTerrainBumpsEvent(Variable *variable);
			void HandleRenderStructureEffectsEvent(Variable *variable);
			void HandlePostBrightnessEvent(Variable *variable);
			void HandlePostMotionBlurEvent(Variable *variable);
			void HandlePostDistortionEvent(Variable *variable);
			void HandlePostGlowBloomEvent(Variable *variable);
			
			void SetPostProcessingProgram(unsigned_int32 postFlags);
			void SetDisplayRenderTarget(void);
			
			template <int32 index> static void GroupRenderSublist(List<Renderable> *list, unsigned_int32 bit, List<Renderable> *final);
			static void SortRenderSublist(List<Renderable> *list, float zmin, float zmax, List<Renderable> *final);
			
			void SetModelviewMatrix(const Transform4D& matrix);
			
			void SetLocalVertexProgram(Link<VertexProgram> *programLink, const VertexSnippet *snippet);
			void SetLocalVertexProgram(Link<VertexProgram> *programLink, int32 snippetCount, const VertexSnippet *const *snippet);
			void SetLocalFragmentProgram(int32 programIndex);
			
			void SetBlendState(unsigned_int32 newBlendState);
			void SetRenderState(unsigned_int32 newRenderState);
			void SetMaterialState(const RenderSegment *segment, unsigned_int32 newMaterialState);
			void ResetArrayState(int32 texcoordPreserveCount = 0);
			
			void SetVertexArray(const ShaderData *shaderData);
			void SetPosition1Array(const ShaderData *shaderData);
			void SetPreviousArray(const ShaderData *shaderData);
			void SetNormalArray(const ShaderData *shaderData);
			void SetTangentArray(const ShaderData *shaderData);
			void SetOffsetArray(const ShaderData *shaderData);
			void SetColorArray(const ShaderData *shaderData);
			void SetAuxColorArray(const ShaderData *shaderData, int32 index);
			void SetTexcoordArray(const ShaderData *shaderData, int32 index);
			
			#if C4DIAGNOSTICS
			
				void DrawShadowBoundsDiagnostic(const StencilShadow *shadow);
				void DrawStencilDiagnostic(const StencilShadow *shadow, StencilType type, StencilMode mode);
			
			#endif
			
		public:
			
			GraphicsMgr(int);
			~GraphicsMgr();
			
			EngineResult Construct(void);
			void Destruct(void);
			
			#if C4WINDOWS
			
				HDC GetDeviceContext(void) const
				{
					return (deviceContext);
				}
			
			#elif C4MACOS
			
				void UpdateOpenGLContext(void)
				{
					aglUpdateContext(openglContext);
				}
			
			#endif
			
			const GraphicsCapabilities *GetCapabilities(void) const
			{
				return (&capabilities);
			}
			
			static const GraphicsExtensionData *GetExtensionData(void)
			{
				return (extensionData);
			}
			
			#if C4OPENGL
			
				#if C4WINDOWS || C4LINUX
				
					static const WindowSystemExtensionData *GetWindowSystemExtensionData(void)
					{
						return (windowSystemExtensionData);
					}
				
				#endif
				
				static const char *GetOpenGLVendor(void)
				{
					return (reinterpret_cast<const char *>(glGetString(GL_VENDOR)));
				}
				
				static const char *GetOpenGLRenderer(void)
				{
					return (reinterpret_cast<const char *>(glGetString(GL_RENDERER)));
				}
				
				static const char *GetOpenGLVersion(void)
				{
					return (reinterpret_cast<const char *>(glGetString(GL_VERSION)));
				}
			
			#endif
			
			int32 GetDynamicShadowMapSize(void) const
			{
				return (dynamicShadowMapSize);
			}
			
			unsigned_int32 GetTargetDisableMask(void) const
			{
				return (targetDisableMask);
			}
			
			void SetTargetDisableMask(unsigned_int32 mask)
			{
				targetDisableMask = mask;
			}
			
			const CameraObject *GetCameraObject(void) const
			{
				return (cameraObject);
			}
			
			const Transformable *GetCameraTransformable(void) const
			{
				return (cameraTransformable);
			}
			
			const Vector4D& GetCameraPosition4D(void) const
			{
				return (cameraPosition4D);
			}
			
			const Point3D& GetDirectCameraPosition(void) const
			{
				return (directCameraPosition);
			}
			
			const Transform4D& GetCameraSpaceTransform(void) const
			{
				return (cameraSpaceTransform);
			}
			
			const FogSpaceObject *GetFogSpaceObject(void) const
			{
				return (fogSpaceObject);
			}
			
			const Transformable *GetFogSpaceTransformable(void) const
			{
				return (fogSpaceTransformable);
			}
			
			const Antivector4D& GetFogPlane(void) const
			{
				return (worldFogPlane);
			}
			
			const LightObject *GetLightObject(void) const
			{
				return (lightObject);
			}
			
			const Transformable *GetLightTransformable(void) const
			{
				return (lightTransformable);
			}
			
			const LightShadowData *GetLightShadowData(void) const
			{
				return (lightShadowData);
			}
			
			AmbientMode GetAmbientMode(void) const
			{
				return (currentAmbientMode);
			}
			
			void SetAmbientMode(AmbientMode mode)
			{
				currentAmbientMode = mode;
			}
			
			unsigned_int32 GetStructureFlags(void) const
			{
				return (currentStructureFlags);
			}
			
			float GetBrightnessMultiplier(void) const
			{
				return (brightnessMultiplier);
			}
			
			void SetBrightnessMultiplier(float brightness)
			{
				brightnessMultiplier = brightness;
			}
			
			const ColorRGBA& GetFinalColorScale(int32 index = 0) const
			{
				return (finalColorScale[index]);
			}
			
			const ColorRGBA& GetFinalColorBias(void) const
			{
				return (finalColorBias);
			}
			
			float GetRenderTargetOffsetSize(void) const
			{
				return (renderTargetOffsetSize);
			}
			
			Texture *GetNullTexture(void) const
			{
				return (nullTexture);
			}
			
			const Render::TextureObject *GetReflectionTexture(void) const
			{
				return (normalFrameBuffer->GetRenderTargetTexture(kRenderTargetReflection));
			}
			
			const Render::TextureObject *GetRefractionTexture(void) const
			{
				return (normalFrameBuffer->GetRenderTargetTexture(kRenderTargetRefraction));
			}
			
			const Render::TextureObject *GetStructureTexture(void) const
			{
				return (normalFrameBuffer->GetRenderTargetTexture(kRenderTargetStructure));
			}
			
			const Render::TextureObject *GetShadowMapTexture(void) const
			{
				return (shadowFrameBuffer->GetRenderTargetTexture());
			}
			
			unsigned_int32 GetRenderTargetMask(void) const
			{
				return (normalFrameBuffer->GetRenderTargetMask());
			}
			
			unsigned_int32 GetRenderOptionFlags(void) const
			{
				return (renderOptionFlags);
			}
			
			void SetRenderOptionFlags(unsigned_int32 flags)
			{
				renderOptionFlags = flags;
			}
			
			int32 GetTextureDetailLevel(void) const
			{
				return (textureDetailLevel);
			}
			
			int32 GetPaletteDetailLevel(void) const
			{
				return (paletteDetailLevel);
			}
			
			int32 GetTextureFilterAnisotropy(void) const
			{
				return (textureFilterAnisotropy);
			}
			
			void SetOcclusionQuery(OcclusionQuery *query)
			{
				currentOcclusionQuery = query;
				Render::BeginQuery(query, Render::kQuerySamplesPassed);
			}
			
			const Antivector4D& GetDistortionPlane(void) const
			{
				return (distortionPlane);
			}
			
			unsigned_int32 GetDiagnosticFlags(void) const
			{
				return (diagnosticFlags);
			}
			
			void SetDiagnosticFlags(unsigned_int32 flags)
			{
				diagnosticFlags = flags;
			}
			
			int32 GetGraphicsCounter(int32 index) const
			{
				return (graphicsCounter[index]);
			}
			
			static void SetShaderTime(float time, float delta);
			static void SetImpostorDepthParams(float scale, float offset, float tangent);
			
			static void ResetShaders(void);
			void InvalidateVertexBuffer(const VertexBuffer *buffer);
			
			void BeginRendering(void);
			void EndRendering(void);
			
			unsigned_int64 GetRenderingTime(void);
			
			void SetFinalColorTransform(const ColorRGBA& scale, const ColorRGBA& bias);
			void SetFinalColorTransform(const ColorRGBA& red, const ColorRGBA& green, const ColorRGBA& blue, const ColorRGBA& bias);
			
			void SetRenderTarget(RenderTargetType type);
			void CopyRenderTarget(Texture *texture, const Rect& rect);
			
			void SetOrtho(const Rect& rect, float orthoLeft, float orthoRight, float orthoTop, float orthoBottom, float nearDepth, float farDepth);
			void SetFrustum(const Rect& rect, float focalLength, float aspectRatio, float nearDepth, float farDepth, unsigned_int32 flags);
			void SetSubfrustum(const Rect& rect, const ProjectionRect& frustumBoundary, float focalLength, float aspectRatio, float nearDepth, float farDepth, unsigned_int32 flags, const Antivector4D& worldClipPlane);
			
			void SetCamera(const CameraObject *camera, const Transformable *transformable, unsigned_int32 clearMask = ~0, bool reset = true);
			void SetFogSpace(const FogSpaceObject *fogSpace, const Transformable *transformable);
			
			void SetAmbient(void);
			void BeginClip(const Rect& rect);
			void EndClip(void);
			
			bool SetLight(const LightObject *light, const Transformable *transformable, const LightShadowData *shadowData = nullptr);
			const Vector4D& SetGeometryTransformable(const Transformable *transformable);
			
			void GroupAmbientRenderList(List<Renderable> *renderList);
			void GroupLightRenderList(List<Renderable> *renderList);
			void SortRenderList(List<Renderable> *renderList);
			
			void DrawRenderList(const List<Renderable> *renderList);
			
			bool BeginStructureRendering(const Transform4D& previousCameraWorldTransform, unsigned_int32 structureFlags, float velocityScale);
			void EndStructureRendering(void);
			void DrawStructureList(const List<Renderable> *renderList);
			void DrawStructureDepthList(const List<Renderable> *renderList);
			
			bool BeginDistortionRendering(void);
			void EndDistortionRendering(void);
			void DrawDistortionList(const List<Renderable> *renderList);
			
			bool ActivateShadowBounds(const StencilShadow *shadow);
			void DeactivateShadowBounds(void);
			
			void BeginStencilShadow(void);
			void EndStencilShadow(void);
			void DrawStencilShadow(const StencilShadow *shadow, StencilType type, StencilMode mode);
			
			void BeginShadowMap(void);
			void EndShadowMap(void);
			void DrawShadowMapList(const List<Renderable> *renderList);
			
			void DrawWireframe(unsigned_int32 flags, const List<Renderable> *renderList);
			void DrawVectors(int32 array, const List<Renderable> *renderList);
			
			void ProcessOcclusionQueries(void);
			
			static void ReadImageBuffer(const Rect& rect, Color4C *image, int32 rowPixels, const Integer2D& p);
			static void ReadDepthBuffer(const Rect& rect, unsigned_int16 *depth, int32 rowPixels, const Integer2D& p);
			static void ReadDepthBuffer(const Rect& rect, unsigned_int32 *depth, int32 rowPixels, const Integer2D& p);
	};
	
	
	C4_API extern GraphicsMgr *TheGraphicsMgr;
}


#endif

// ZYURVUR
