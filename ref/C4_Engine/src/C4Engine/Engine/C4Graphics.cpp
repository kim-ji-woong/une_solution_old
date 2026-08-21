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


#include "C4Display.h"
#include "C4Graphics.h"
#include "C4Shaders.h"
#include "C4SpaceObjects.h"
#include "C4Engine.h"

#if C4WINDOWS

	#include "C4Nvidia.h"

#endif


using namespace C4;


namespace
{
	const float kFrustumEpsilon			= 2.0e-6F;
	const float kVelocityMultiplier		= 0.142857F;
	const float kMotionBlurBoxExpand	= 8.0F;
	
	
	enum
	{
		kGraphicsDepthTestLess			= 1 << 0,
		kGraphicsAmbientLessEqual		= 1 << 1,
		kGraphicsCullFaceBack			= 1 << 2,
		kGraphicsFrontFaceCCW			= 1 << 3,
		kGraphicsClipEnabled			= 1 << 4,
		kGraphicsLightScissor			= 1 << 5,
		kGraphicsShadowScissor			= 1 << 6,
		kGraphicsUpdateShadowScissor	= 1 << 7,
		kGraphicsDepthBoundsAvail		= 1 << 8,
		kGraphicsObliqueFrustum			= 1 << 9,
		kGraphicsLightDepthBounds		= 1 << 10,
		kGraphicsShadowDepthBounds		= 1 << 11,
		kGraphicsStencilClear			= 1 << 12,
		kGraphicsStencilValid			= 1 << 13,
		kGraphicsRenderLight			= 1 << 14,
		kGraphicsRenderStencil			= 1 << 15,
		kGraphicsRenderShadowMap		= 1 << 16,
		kGraphicsReactivateTextures		= 1 << 17,
		kGraphicsMotionBlurAvail		= 1 << 18,
		kGraphicsDistortionAvail		= 1 << 19,
		kGraphicsGlowBloomAvail			= 1 << 20,
		
		kGraphicsScissorMask			= kGraphicsLightScissor | kGraphicsShadowScissor,
		kGraphicsDepthBoundsMask		= kGraphicsLightDepthBounds | kGraphicsShadowDepthBounds
	};
	
	
	enum
	{
		kPostColorMatrix			= 1 << 0,
		kPostMotionBlur				= 1 << 1,
		kPostMotionBlurGradient		= 1 << 2,
		kPostDistortion				= 1 << 3,
		kPostGlowBloom				= 1 << 4,
		kPostProgramCount			= 1 << 5
	};
	
	
	enum
	{
		kLocalProgramCopyLightColor,
		kLocalProgramCopyVertexColor,
		kLocalProgramCount
	};
	
	
	inline float *ArrayOffsetToPtr(unsigned_int32 offset)
	{
		return ((float *) ((char *) nullptr + offset));
	}
	
	inline unsigned_int16 *IndexArrayOffsetToPtr(unsigned_int32 offset)
	{
		return ((unsigned_int16 *) ((char *) nullptr + offset));
	}
	
	
	const TextureHeader nullTextureHeader =
	{
		kTexture2D,
		0,
		kTextureSemanticNone,
		kTextureSemanticNone,
		kTextureI8,
		4, 4, 1,
		{kTextureRepeat, kTextureRepeat, kTextureRepeat},
		5
	};
	
	const unsigned_int8 align_address(32) nullTextureImage[21] =
	{
		0xFF, 0x00, 0xFF, 0x00, 0x00, 0xFF, 0x00, 0xFF, 0xFF, 0x00, 0xFF, 0x00, 0x00, 0xFF, 0x00, 0xFF,
		0xFF, 0x00, 0xFF, 0x00,
		0xFF
	};
}
 

GraphicsMgr *C4::TheGraphicsMgr = nullptr; 
 
 
namespace C4
{ 
	template <> GraphicsMgr Manager<GraphicsMgr>::managerObject(0);
	template <> GraphicsMgr **Manager<GraphicsMgr>::managerPointer = &TheGraphicsMgr;
	
	template class Manager<GraphicsMgr>; 
}


GraphicsExtensionData GraphicsMgr::extensionData[kGraphicsExtensionCount] = 
{
	{"GL_ARB_color_buffer_float",				nullptr,							0x0300,	false,	true},
	{"GL_ARB_conservative_depth",				nullptr,							0x0420,	false,	true},
	{"GL_ARB_debug_output",						nullptr,							0xFFFF,	false,	true},
	{"GL_ARB_depth_clamp",						"GL_NV_depth_clamp",				0x0320,	false,	true},
	{"GL_ARB_depth_texture",					nullptr,							0x0140,	false,	true},
	{"GL_ARB_fragment_program",					nullptr,							0xFFFF,	true,	true},
	{"GL_ARB_fragment_program_shadow",			nullptr,							0xFFFF,	false,	true},
	{"GL_ARB_fragment_shader",					nullptr,							0x0200,	false,	true},
	{"GL_ARB_framebuffer_object",				"GL_EXT_framebuffer_object",		0x0300,	true,	true},
	{"GL_EXT_framebuffer_blit",					nullptr,							0x0300,	false,	true},
	{"GL_EXT_framebuffer_multisample",			nullptr,							0x0300,	false,	true},
	{"GL_ARB_framebuffer_sRGB",					"GL_EXT_framebuffer_sRGB",			0x0300,	false,	true},
	{"GL_ARB_get_program_binary",				nullptr,							0x0410,	false,	true},
	{"GL_ARB_half_float_pixel",					nullptr,							0x0300,	false,	true},
	{"GL_ARB_instanced_arrays",					nullptr,							0x0330,	false,	true},
	{"GL_ARB_multisample",						nullptr,							0x0130,	false,	true},
	{"GL_ARB_multitexture",						nullptr,							0x0130,	true,	true},
	{"GL_ARB_occlusion_query",					nullptr,							0x0150,	false,	true},
	{"GL_ARB_packed_depth_stencil",				"GL_EXT_packed_depth_stencil",		0x0300,	true,	true},
	{"GL_ARB_pixel_buffer_object",				"GL_EXT_pixel_buffer_object",		0x0210,	false,	true},
	{"GL_ARB_point_sprite",						nullptr,							0x0200,	false,	true},
	{"GL_ARB_sample_shading",					nullptr,							0x0400,	false,	true},
	{"GL_ARB_seamless_cube_map",				nullptr,							0x0320,	false,	true},
	{"GL_ARB_separate_shader_objects",			nullptr,							0x0410,	false,	true},
	{"GL_ARB_shadow",							nullptr,							0x0140,	true,	true},
	{"GL_ARB_shader_objects",					nullptr,							0x0200,	false,	true},
	{"GL_ARB_shader_texture_lod",				"GL_ATI_shader_texture_lod",		0xFFFF,	false,	true},
	{"GL_ARB_tessellation_shader",				nullptr,							0x0400,	false,	true},
	{"GL_ARB_texture_array",					"GL_EXT_texture_array",				0x0300,	false,	true},
	{"GL_ARB_texture_border_clamp",				nullptr,							0x0130,	true,	true},
	{"GL_ARB_texture_compression",				nullptr,							0x0130,	true,	true},
	{"GL_ARB_texture_compression_rgtc",			"GL_EXT_texture_compression_rgtc",	0x0300,	false,	true},
	{"GL_ARB_texture_cube_map",					"GL_EXT_texture_cube_map",			0x0130,	true,	true},
	{"GL_ARB_texture_float",					"GL_ATI_texture_float",				0x0300,	false,	true},
	{"GL_ARB_texture_mirrored_repeat",			"GL_IBM_texture_mirrored_repeat",	0x0140,	false,	true},
	{"GL_ARB_texture_rectangle",				"GL_EXT_texture_rectangle",			0x0310,	true,	true},
	{"GL_ARB_timer_query",						nullptr,							0x0330,	false,	true},
	{"GL_ARB_uniform_buffer_object",			nullptr,							0x0310,	false,	true},
	{"GL_ARB_vertex_buffer_object",				nullptr,							0x0150,	true,	true},
	{"GL_ARB_vertex_program",					nullptr,							0xFFFF,	true,	true},
	{"GL_EXT_blend_color",						nullptr,							0x0120,	true,	true},
	{"GL_EXT_blend_minmax",						nullptr,							0x0120,	false,	true},
	{"GL_EXT_blend_subtract",					nullptr,							0x0120,	false,	true},
	{"GL_EXT_blend_func_separate",				nullptr,							0x0140,	true,	true},
	{"GL_EXT_depth_bounds_test",				nullptr,							0xFFFF,	false,	true},
	{"GL_EXT_direct_state_access",				nullptr,							0xFFFF,	false,	true},
	{"GL_EXT_draw_range_elements",				nullptr,							0x0120,	true,	true},
	{"GL_EXT_gpu_shader4",						nullptr,							0xFFFF,	false,	true},
	{"GL_EXT_multi_draw_arrays",				nullptr,							0x0140,	true,	true},
	{"GL_EXT_packed_pixels",					nullptr,							0x0120,	false,	true},
	{"GL_EXT_stencil_wrap",						nullptr,							0x0140,	true,	true},
	{"GL_EXT_texture3D",						nullptr,							0x0120,	true,	true},
	{"GL_EXT_texture_compression_s3tc",			nullptr,							0xFFFF,	true,	true},
	{"GL_EXT_texture_edge_clamp",				"GL_SGIS_texture_edge_clamp",		0x0120,	true,	true},
	{"GL_EXT_texture_filter_anisotropic",		nullptr,							0xFFFF,	false,	true},
	{"GL_EXT_texture_mirror_clamp",				"GL_ATI_texture_mirror_once",		0xFFFF,	false,	true},
	{"GL_EXT_texture_sRGB",						nullptr,							0x0210,	false,	true},
	{"GL_NV_conditional_render",				nullptr,							0x0300,	false,	true},
	{"GL_NV_explicit_multisample",				nullptr,							0xFFFF,	false,	true},
	{"GL_NV_fragment_program2",					nullptr,							0xFFFF,	false,	true},
	{"GL_NV_framebuffer_multisample_coverage",	nullptr,							0xFFFF,	false,	true},
	{"GL_NV_gpu_program4",						nullptr,							0xFFFF,	false,	true},
	{"GL_NV_shader_buffer_load",				nullptr,							0xFFFF,	false,	true},
	{"GL_NV_transform_feedback",				nullptr,							0xFFFF,	false,	true},
	{"GL_NV_vertex_buffer_unified_memory",		nullptr,							0xFFFF,	false,	true}
};


#if C4WINDOWS

	WindowSystemExtensionData GraphicsMgr::windowSystemExtensionData[kWindowSystemExtensionCount] =
	{
		{"WGL_ARB_create_context", true},
		{"WGL_ARB_pixel_format", true},
		{"WGL_EXT_swap_control", true},
		{"WGL_EXT_swap_control_tear", true}
	};

#elif C4LINUX

	WindowSystemExtensionData GraphicsMgr::windowSystemExtensionData[kWindowSystemExtensionCount] =
	{
		{"GLX_EXT_swap_control", true},
		{"GLX_EXT_swap_control_tear", true}
	};

#endif


#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

#endif //]


NormalFrameBuffer::NormalFrameBuffer(int32 width, int32 height, unsigned_int32 mask)
{
	static const unsigned_int32 format[kRenderTargetCount] =
	{
		Render::kTextureRenderBufferRGBA8, Render::kTextureRenderBufferRGBA8, Render::kTextureRenderBufferRGBA8, Render::kTextureRenderBufferRGBA16F
	};
	
	Render::FrameBufferObject::Construct();
	
	renderTargetMask = mask;
	for (machine a = 0; a < kRenderTargetCount; a++)
	{
		if (mask & 1)
		{
			Render::TextureObject *texture = &textureObject[a];
			texture->Construct(Render::kTextureTargetRectangle);
			texture->AllocateStorageRect(format[a], width, height, true);
		}
		
		mask >>= 1;
	}
	
	depthRenderBuffer.Construct();
	depthRenderBuffer.AllocateStorage(width, height, Render::kRenderBufferDepthStencil);
	
	SetDepthStencilRenderBuffer(&depthRenderBuffer);
	Render::ResetFrameBuffer();
}

NormalFrameBuffer::~NormalFrameBuffer()
{
	depthRenderBuffer.Destruct();
	
	unsigned_int32 mask = renderTargetMask;
	for (machine a = kRenderTargetCount - 1; a >= 0; a--)
	{
		if (mask & (1 << a)) textureObject[a].Destruct();
	}
	
	Render::FrameBufferObject::Destruct();
}


ShadowFrameBuffer::ShadowFrameBuffer(int32 width, int32 height)
{
	Render::FrameBufferObject::Construct();
	
	textureObject.Construct(Render::kTextureTarget2D);
	textureObject.AllocateStorage2D(Render::kTextureDepth24, width, height, true);
	
	textureObject.SetCompareFunc(Render::kShadowLessEqual);
	textureObject.SetBorderColor(ColorRGBA(1.0F, 1.0F, 1.0F, 1.0F));
	
	SetTextureAnisotropy(TheGraphicsMgr->GetTextureFilterAnisotropy());
	
	unsigned_int32 wrapMode = (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionTextureBorderClamp]) ? Render::kWrapClampToBorder : Render::kWrapClamp;
	textureObject.SetSWrapMode(wrapMode);
	textureObject.SetTWrapMode(wrapMode);
	
	SetDepthRenderTexture(&textureObject);
	Render::ResetFrameBuffer();
}

ShadowFrameBuffer::~ShadowFrameBuffer()
{
	textureObject.Destruct();
	Render::FrameBufferObject::Destruct();
}

void ShadowFrameBuffer::SetTextureAnisotropy(int32 anisotropy)
{
	const GraphicsCapabilities *capabilities = TheGraphicsMgr->GetCapabilities();
	if (capabilities->extensionFlag[kExtensionTextureFilterAnisotropic])
	{
		textureObject.SetMaxAnisotropy(Fmin((float) anisotropy, capabilities->maxTextureAnisotropy));
	}
}


MultisampleFrameBuffer::MultisampleFrameBuffer(int32 width, int32 height, int32 sampleCount)
{
	Render::FrameBufferObject::Construct();
	
	colorRenderBuffer.Construct();
	unsigned_int32 format = Render::kRenderBufferRGBA8;
	colorRenderBuffer.AllocateMultisampleStorage(width, height, sampleCount, format);
	
	depthRenderBuffer.Construct();
	depthRenderBuffer.AllocateMultisampleStorage(width, height, sampleCount, Render::kRenderBufferDepthStencil);
	
	SetColorRenderBuffer(&colorRenderBuffer);
	SetDepthStencilRenderBuffer(&depthRenderBuffer);
	
	#if C4OPENGL
	
		GLint	samples;
		
		glGetIntegerv(GL_SAMPLES, &samples);
		sampleDivider = 1.0F / (float) samples;
	
	#else
	
		sampleDivider = 1.0F / (float) sampleCount;
	
	#endif
	
	Render::ResetFrameBuffer();
}

MultisampleFrameBuffer::~MultisampleFrameBuffer()
{
	depthRenderBuffer.Destruct();
	colorRenderBuffer.Destruct();
	Render::FrameBufferObject::Destruct();
}


GraphicsMgr::GraphicsMgr(int) :
		textureDetailLevelObserver(this, &GraphicsMgr::HandleTextureDetailLevelEvent),
		paletteDetailLevelObserver(this, &GraphicsMgr::HandlePaletteDetailLevelEvent),
		textureAnisotropyObserver(this, &GraphicsMgr::HandleTextureAnisotropyEvent),
		renderNormalizeBumpsObserver(this, &GraphicsMgr::HandleRenderNormalizeBumpsEvent),
		renderParallaxMappingObserver(this, &GraphicsMgr::HandleRenderParallaxMappingEvent),
		renderHorizonMappingObserver(this, &GraphicsMgr::HandleRenderHorizonMappingEvent),
		renderTerrainBumpsObserver(this, &GraphicsMgr::HandleRenderTerrainBumpsEvent),
		renderStructureEffectsObserver(this, &GraphicsMgr::HandleRenderStructureEffectsEvent),
		postBrightnessObserver(this, &GraphicsMgr::HandlePostBrightnessEvent),
		postMotionBlurObserver(this, &GraphicsMgr::HandlePostMotionBlurEvent),
		postDistortionObserver(this, &GraphicsMgr::HandlePostDistortionEvent),
		postGlowBloomObserver(this, &GraphicsMgr::HandlePostGlowBloomEvent)
{
	driverVersion = 0;
	
	cameraObject = nullptr;
	cameraTransformable = nullptr;
	
	cameraSpaceTransform(3,0) = cameraSpaceTransform(3,1) = cameraSpaceTransform(3,2) = 0.0F;
	cameraSpaceTransform(3,3) = 1.0F;
	
	#if C4WINDOWS && !C4SERVER
	
		if (NvAPI_Initialize() == NVAPI_OK)
		{
			NV_DISPLAY_DRIVER_VERSION	version;
			
			version.version = NV_DISPLAY_DRIVER_VERSION_VER;
			if (NvAPI_GetDisplayDriverVersion(NVAPI_DEFAULT_HANDLE, &version) == NVAPI_OK)
			{
				driverVersion = version.drvVersion;
				if (driverVersion < 25896) GraphicsMgr::extensionData[kExtensionDirectStateAccess].enabled = false;
			}
			
			NvAPI_Unload();
		}
	
	#endif
}

GraphicsMgr::~GraphicsMgr()
{
}

EngineResult GraphicsMgr::Construct(void)
{
	int32 displayWidth = TheDisplayMgr->GetDisplayWidth();
	int32 displayHeight = TheDisplayMgr->GetDisplayHeight();
	int32 displaySamples = TheDisplayMgr->GetDisplaySamples();
	unsigned_int32 displayFlags = TheDisplayMgr->GetDisplayFlags();
	
	GraphicsResult result = InitializeGraphicsContext(displayWidth, displayHeight, displayFlags);
	if (result != kGraphicsOkay) return (result);
	
	bool extensionsAvailable = InitializeOpenglExtensions();
	UpdateLog();
	
	if (!extensionsAvailable)
	{
		TerminateGraphicsContext();
		return (kGraphicsNoHardware);
	}
	
	VertexProgram::Initialize();
	FragmentProgram::Initialize();
	MicrofacetAttribute::Initialize();
	
	dynamicShadowMapSize = Min(Min(capabilities.maxTextureSize, capabilities.maxRenderbufferSize) / kMaxShadowSectionCount, 1024);
	targetDisableMask = 0;
	
	fogSpaceObject = nullptr;
	fogSpaceTransformable = nullptr;
	
	lightObject = nullptr;
	lightTransformable = nullptr;
	
	geometryTransformable = nullptr;
	geometryLightPosition.Set(0.0F, 0.0F, 0.0F, 1.0F);
	
	currentBlendState = kBlendReplace;
	currentStencilMode = kStencilNone;
	
	currentGraphicsState = kGraphicsDepthTestLess | kGraphicsCullFaceBack | kGraphicsFrontFaceCCW | kGraphicsReactivateTextures;
	currentRenderState = 0;
	currentMaterialState = 0;
	currentShaderType = kShaderAmbient;
	currentShaderVariant = kShaderVariantNormal;
	currentAmbientMode = kAmbientNormal;
	
	currentOcclusionQuery = nullptr;
	
	currentArrayState = 0;
	for (machine a = 0; a < kMaxShaderArrayCount; a++)
	{
		currentVertexBuffer[a] = nullptr;
		shaderArrayPointer[a] = nullptr;
	}
	
	colorTransformFlags = 0;
	finalColorScale[0].Set(1.0F, 1.0F, 1.0F, 1.0F);
	finalColorBias.Set(0.0F, 0.0F, 0.0F, 0.0F);
	
	diagnosticFlags = 0;
	
	Render::EnableBlend();
	Render::EnableCullFace();
	Render::SetAlphaFunc(Render::kAlphaGreater, 0.0F);
	Render::SetBlendFunc(Render::kBlendOne, Render::kBlendZero, Render::kBlendZero, Render::kBlendZero);
	Render::SetDepthFunc(Render::kDepthLess);
	Render::SetStencilFunc(Render::kStencilAlways, 0, ~0);
	Render::SetCullFace(Render::kCullBack);
	Render::SetClearColor(0.0F, 0.0F, 0.0F, 1.0F);
	
	#if C4OPENGL
	
		#if C4CREATE_DEBUG_CONTEXT
		
			if (capabilities.extensionFlag[kExtensionDebugOutput])
			{
				glDebugMessageCallbackARB(&DebugCallback, this);
			}
		
		#endif
		
		glEnable(GL_SCISSOR_TEST);
		glEnable(GL_VERTEX_PROGRAM_ARB);
		glEnable(GL_FRAGMENT_PROGRAM_ARB);
		
		glActiveTexture(GL_TEXTURE0);
		glEnableVertexAttribArrayARB(0);
		
		if (capabilities.extensionFlag[kExtensionPointSprite])
		{
			glTexEnvi(GL_POINT_SPRITE, GL_COORD_REPLACE, true);
			glPointParameteri(GL_POINT_SPRITE_COORD_ORIGIN, GL_LOWER_LEFT);
		}
		
		glPixelStorei(GL_UNPACK_ALIGNMENT, 1);
	
	#endif
	
	if (capabilities.extensionFlag[kExtensionTimerQuery])
	{
		frameCount = 0;
		timerQuery[0].Construct();
		timerQuery[1].Construct();
		timerQuery[2].Construct();
		timerQuery[3].Construct();
	}
	
	if (capabilities.extensionFlag[kExtensionSampleShading]) Render::SetMinSampleShading(1.0F);
	
	renderOptionFlags = InitializeVariables();
	
	currentRenderTargetType = kRenderTargetDisplay;
	renderTargetHeight = displayHeight;
	
	genericFrameBuffer.Construct();
	
	unsigned_int32 mask = (1 << kRenderTargetPrimary) | (1 << kRenderTargetReflection) | (1 << kRenderTargetRefraction);
	if ((renderOptionFlags & (kRenderOptionStructureEffects | kRenderOptionMotionBlur)) && (capabilities.capabilityFlag[kCapabilityFramebufferFloat])) mask |= (1 << kRenderTargetStructure);
	normalFrameBuffer = new NormalFrameBuffer(displayWidth, displayHeight, mask);
	
	if (!capabilities.extensionFlag[kExtensionFramebufferMultisample]) displaySamples = 1;
	if (displaySamples > 1)
	{
		int32 samples = Min(displaySamples, capabilities.maxMultisampleSamples);
		multisampleFrameBuffer = new MultisampleFrameBuffer(displayWidth, displayHeight, samples);
	}
	else
	{
		multisampleFrameBuffer = nullptr;
	}
	
	shadowFrameBuffer = new ShadowFrameBuffer(dynamicShadowMapSize, dynamicShadowMapSize * kMaxShadowSectionCount);
	
	float f = kMotionBlurBoxExpand / (float) displayWidth;
	motionBlurBoxLeftOffset = 0.5F - f;
	motionBlurBoxRightOffset = 0.5F + f;
	
	f = kMotionBlurBoxExpand / (float) displayHeight;
	motionBlurBoxBottomOffset = 0.5F - f;
	motionBlurBoxTopOffset = 0.5F + f;
	
	float scale = (float) displayWidth * 0.03125F;
	
	#if C4OPENGL
	
		Render::SetFragmentProgramParameter4f(kFragmentParamDistortionScale, scale, scale, 0.0F, 0.0F);
	
	#else
	
		Render::SetFragmentProgramParameter4f(kFragmentParamDistortionScale, scale, -scale, 0.0F, 0.0F);
	
	#endif
	
	nullTexture = Texture::Get(&nullTextureHeader, nullTextureImage);
	HorizonProcess::Initialize();
	
	InitializeProcessGrid();
	
	VertexBuffer::ReactivateAll();
	if (cameraObject) SetCamera(cameraObject, cameraTransformable);
	
	return (kEngineOkay);
}

void GraphicsMgr::Destruct(void)
{
	VertexBuffer::DeactivateAll();
	Texture::DeactivateAll();
	ShaderData::Purge();
	
	HorizonProcess::Terminate();
	nullTexture->Release();
	
	delete shadowFrameBuffer;
	delete multisampleFrameBuffer;
	delete normalFrameBuffer;
	genericFrameBuffer.Destruct();
	
	if (capabilities.extensionFlag[kExtensionTimerQuery])
	{
		timerQuery[3].Destruct();
		timerQuery[2].Destruct();
		timerQuery[1].Destruct();
		timerQuery[0].Destruct();
	}
	
	MicrofacetAttribute::Terminate();
	FragmentProgram::Terminate();
	VertexProgram::Terminate();
	
	TerminateGraphicsContext();
}

#if C4CREATE_DEBUG_CONTEXT

	void GraphicsMgr::DebugCallback(GLenum source, GLenum type, GLuint id, GLenum severity, GLsizei length, const GLchar *message, void *userParam)
	{
		Engine::Report(String<>(message) += "<br />\r\n", kReportLog);
	}

#endif

void GraphicsMgr::LogExtensions(const char *string)
{
	if (string) while (*string != 0)
	{
		String<63>	line;
		
		const char *s = string;
		if (*s < 33)
		{
			string++;
			continue;
		}
		
		while (*s > 32) s++;
		int32 len = Min(s - string, 63);
		
		for (machine b = 0; b < len; b++) line[b] = string[b];
		line[len] = 0;
		string = s;
		
		Engine::Report(line, kReportLog);
		Engine::Report("<br/>\r\n", kReportLog);
	}
}

void GraphicsMgr::UpdateLog(void) const
{
	static bool logUpdated = false;
	if (!logUpdated)
	{
		logUpdated = true;
		
		Engine::Report("Graphics Manager", kReportLog | kReportHeading);
		
		#if C4OPENGL
		
			Engine::Report("<table cellspacing=\"0\" cellpadding=\"0\">\r\n", kReportLog);
			
			Engine::Report("<tr><th>GL_VENDOR</th><td>", kReportLog);
			Engine::Report(GetOpenGLVendor(), kReportLog);
			Engine::Report("</td></tr>\r\n", kReportLog);
			
			Engine::Report("<tr><th>GL_RENDERER</th><td>", kReportLog);
			Engine::Report(GetOpenGLRenderer(), kReportLog);
			Engine::Report("</td></tr>\r\n", kReportLog);
			
			Engine::Report("<tr><th>GL_VERSION</th><td>", kReportLog);
			Engine::Report(GetOpenGLVersion(), kReportLog);
			Engine::Report("</td></tr>\r\n", kReportLog);
			
			if (driverVersion != 0)
			{
				Engine::Report("<tr><th>Driver version</th><td>", kReportLog);
				Engine::Report((String<63>(driverVersion / 100) += '.') += driverVersion % 100, kReportLog);
				Engine::Report("</td></tr>\r\n", kReportLog);
			}
			
			Engine::Report("<tr><th>GL_EXTENSIONS</th><td><div style=\"height: 128px; overflow: auto;\">\r\n", kReportLog);
			LogExtensions(extensionsString);
			
			#if C4WINDOWS
			
				Engine::Report("</div></td></tr>\r\n<tr><th>WGL_EXTENSIONS</th><td><div style=\"height: 128px; overflow: auto;\">\r\n", kReportLog);
				LogExtensions(windowSystemExtensionsString);
			
			#elif C4LINUX
			
				Engine::Report("</div></td></tr>\r\n<tr><th>GLX_EXTENSIONS</th><td><div style=\"height: 128px; overflow: auto;\">\r\n", kReportLog);
				LogExtensions(windowSystemExtensionsString);
			
			#endif
			
			Engine::Report("</div></td></tr>\r\n", kReportLog);
			
			Engine::Report("<tr><th>Texture limits</th><td>", kReportLog);
			Engine::Report("Max texture 2D size: ", kReportLog);
			Engine::Report(Text::IntegerToString(capabilities.maxTextureSize), kReportLog);
			Engine::Report("<br />Max texture 3D size: ", kReportLog);
			Engine::Report(Text::IntegerToString(capabilities.max3DTextureSize), kReportLog);
			Engine::Report("<br />Max texture cube size: ", kReportLog);
			Engine::Report(Text::IntegerToString(capabilities.maxCubeTextureSize), kReportLog);
			
			if (capabilities.extensionFlag[kExtensionTextureArray])
			{
				Engine::Report("<br />Max array texture layers: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxArrayTextureLayers), kReportLog);
			}
			
			Engine::Report("<br />Max texture lod bias: ", kReportLog);
			Engine::Report(Text::FloatToString(capabilities.maxTextureLodBias), kReportLog);
			
			if (capabilities.extensionFlag[kExtensionTextureFilterAnisotropic])
			{
				Engine::Report("<br />Max texture anisotropy: ", kReportLog);
				Engine::Report(Text::FloatToString(capabilities.maxTextureAnisotropy), kReportLog);
			}
			
			Engine::Report("</td></tr>\r\n", kReportLog);
			
			if (capabilities.extensionFlag[kExtensionVertexProgram])
			{
				Engine::Report("<tr><th>Vertex program limits</th><td>", kReportLog);
				Engine::Report("Max vertex instructions: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxVertexProgramInstructionCount), kReportLog);
				Engine::Report("<br />Max vertex temporaries: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxVertexProgramTemporaryCount), kReportLog);
				Engine::Report("<br />Max vertex attributes: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxVertexProgramAttributeCount), kReportLog);
				Engine::Report("<br />Max vertex parameters: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxVertexProgramParameterCount), kReportLog);
				Engine::Report("<br />Max vertex address registers: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxVertexProgramAddressRegisterCount), kReportLog);
				Engine::Report("</td></tr>\r\n", kReportLog);
			}
			
			if (capabilities.extensionFlag[kExtensionFragmentProgram])
			{
				Engine::Report("<tr><th>Fragment program limits</th><td>", kReportLog);
				Engine::Report("Max fragment ALU instructions: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxFragmentProgramALUInstructionCount), kReportLog);
				Engine::Report("<br />Max fragment TEX instructions: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxFragmentProgramTEXInstructionCount), kReportLog);
				Engine::Report("<br />Max fragment TEX indirections: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxFragmentProgramTEXIndirectionCount), kReportLog);
				Engine::Report("<br />Max fragment temporaries: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxFragmentProgramTemporaryCount), kReportLog);
				Engine::Report("<br />Max fragment parameters: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxFragmentProgramParameterCount), kReportLog);
				Engine::Report("<br />Max fragment texture coords: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxTextureCoordCount), kReportLog);
				Engine::Report("<br />Max fragment texture images: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxTextureImageCount), kReportLog);
				Engine::Report("</td></tr>\r\n", kReportLog);
			}
			
			if (capabilities.extensionFlag[kExtensionGpuProgram4])
			{
				Engine::Report("<tr><th>Geometry program limits</th><td>", kReportLog);
				Engine::Report("Max geometry output vertices: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxGeometryProgramOutputVertexCount), kReportLog);
				Engine::Report("<br />Max geometry output components: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxGeometryProgramOutputComponentCount), kReportLog);
				Engine::Report("<br />Max geometry texture images: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxGeometryProgramTextureImageCount), kReportLog);
				Engine::Report("<br />Max vertex result components: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxVertexProgramResultComponentCount), kReportLog);
				Engine::Report("<br />Max geometry result components: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxGeometryProgramResultComponentCount), kReportLog);
				Engine::Report("</td></tr>\r\n", kReportLog);
			}
			
			if (capabilities.extensionFlag[kExtensionFramebufferObject])
			{
				Engine::Report("<tr><th>Framebuffer object limits</th><td>", kReportLog);
				Engine::Report("Max framebuffer color attachments: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxColorAttachments), kReportLog);
				Engine::Report("<br />Max framebuffer render buffer size: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxRenderbufferSize), kReportLog);
				
				if (capabilities.extensionFlag[kExtensionFramebufferMultisample])
				{
					Engine::Report("<br />Max framebuffer samples: ", kReportLog);
					Engine::Report(Text::IntegerToString(capabilities.maxMultisampleSamples), kReportLog);
				}
				
				Engine::Report("</td></tr>\r\n", kReportLog);
			}
			
			if (capabilities.extensionFlag[kExtensionTransformFeedback])
			{
				Engine::Report("<tr><th>Transform feedback limits</th><td>", kReportLog);
				Engine::Report("Max feedback interleaved components: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxTransformFeedbackInterleavedComponents), kReportLog);
				Engine::Report("<br />Max feedback separate components: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxTransformFeedbackSeparateComponents), kReportLog);
				Engine::Report("<br />Max feedback separate attributes: ", kReportLog);
				Engine::Report(Text::IntegerToString(capabilities.maxTransformFeedbackSeparateAttribs), kReportLog);
				Engine::Report("</td></tr>\r\n", kReportLog);
			}
			
			Engine::Report("</table>\r\n", kReportLog);
		
		#endif
	}
}

GraphicsResult GraphicsMgr::InitializeGraphicsContext(int32 displayWidth, int32 displayHeight, unsigned_int32 displayFlags)
{
	Render::Initialize();
	
	#if !C4SERVER
	
		#if C4WINDOWS
		
			PIXELFORMATDESCRIPTOR	formatDescriptor;
			int						pixelFormat;
			
			MemoryMgr::ClearMemory(&formatDescriptor, sizeof(PIXELFORMATDESCRIPTOR));
			formatDescriptor.nSize = sizeof(PIXELFORMATDESCRIPTOR);
			formatDescriptor.nVersion = 1;
			
			formatDescriptor.dwFlags = PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_GENERIC_ACCELERATED | PFD_DOUBLEBUFFER | PFD_SWAP_EXCHANGE;
			if (!(displayFlags & kDisplayFullscreen)) formatDescriptor.dwFlags |= PFD_SUPPORT_COMPOSITION;
			
			formatDescriptor.iPixelType = PFD_TYPE_RGBA;
			formatDescriptor.cColorBits = 24;
			formatDescriptor.cAlphaBits = 8;
			formatDescriptor.cDepthBits = 0;
			formatDescriptor.cStencilBits = 0;
			formatDescriptor.iLayerType = PFD_MAIN_PLANE;
			
			InitializeWglExtensions(&formatDescriptor);
			
			deviceContext = GetDC(TheDisplayMgr->GetDisplayWindow());
			
			if (capabilities.windowSystemExtensionFlag[kWindowSystemExtensionPixelFormat])
			{
				UINT	formatCount;
				
				static const int formatAttributes[] =
				{
					WGL_SUPPORT_OPENGL_ARB, true,
					WGL_DRAW_TO_WINDOW_ARB, true,
					WGL_DOUBLE_BUFFER_ARB, true,
					WGL_ACCELERATION_ARB, WGL_FULL_ACCELERATION_ARB,
					WGL_PIXEL_TYPE_ARB, WGL_TYPE_RGBA_ARB,
					WGL_COLOR_BITS_ARB, 24,
					WGL_ALPHA_BITS_ARB, 8,
					WGL_DEPTH_BITS_ARB, 0,
					WGL_STENCIL_BITS_ARB, 0,
					0, 0
				};
				
				if ((!wglChoosePixelFormatARB(deviceContext, formatAttributes, nullptr, 1, &pixelFormat, &formatCount)) || (formatCount == 0))
				{
					ReleaseDC(TheDisplayMgr->GetDisplayWindow(), deviceContext);
					return (kGraphicsNoHardware);
				}
			}
			else
			{
				pixelFormat = ChoosePixelFormat(deviceContext, &formatDescriptor);
				if (pixelFormat == 0)
				{
					ReleaseDC(TheDisplayMgr->GetDisplayWindow(), deviceContext);
					return (kGraphicsNoHardware);
				}
			}
			
			if (!SetPixelFormat(deviceContext, pixelFormat, &formatDescriptor))
			{
				ReleaseDC(TheDisplayMgr->GetDisplayWindow(), deviceContext);
				return (kGraphicsFormatFailed);
			}
			
			openglContext = nullptr;
			
			if (capabilities.windowSystemExtensionFlag[kWindowSystemExtensionCreateContext])
			{
				static const int contextAttributes[] =
				{
					WGL_CONTEXT_MAJOR_VERSION_ARB, 3,
					WGL_CONTEXT_MINOR_VERSION_ARB, 2,
					WGL_CONTEXT_PROFILE_MASK_ARB, WGL_CONTEXT_COMPATIBILITY_PROFILE_BIT_ARB,
					
					#if C4CREATE_DEBUG_CONTEXT
					
						WGL_CONTEXT_FLAGS_ARB, WGL_CONTEXT_DEBUG_BIT_ARB,
					
					#endif
					
					0, 0
				};
				
				openglContext = wglCreateContextAttribsARB(deviceContext, nullptr, contextAttributes);
			}
			
			if (!openglContext)
			{
				openglContext = wglCreateContext(deviceContext);
				
				if (!openglContext)
				{
					ReleaseDC(TheDisplayMgr->GetDisplayWindow(), deviceContext);
					return (kGraphicsContextFailed);
				}
			}
			
			if (!wglMakeCurrent(deviceContext, openglContext))
			{
				wglDeleteContext(openglContext);
				ReleaseDC(TheDisplayMgr->GetDisplayWindow(), deviceContext);
				return (kGraphicsContextFailed);
			}
			
			if (capabilities.windowSystemExtensionFlag[kWindowSystemExtensionSwapControl])
			{
				int32 swapInterval = ((displayFlags & kDisplayRefreshSync) != 0);
				if ((capabilities.windowSystemExtensionFlag[kWindowSystemExtensionSwapControlTear]) && (displayFlags & kDisplaySyncTear)) swapInterval = -swapInterval;
				wglSwapIntervalEXT(swapInterval);
			}
			
			if (wglGetExtensionsStringARB) windowSystemExtensionsString = wglGetExtensionsStringARB(deviceContext);
		
		#elif C4MACOS
		
			static GLint formatAttributes[13] =
			{
				AGL_MINIMUM_POLICY,
				AGL_ACCELERATED,
				AGL_DOUBLEBUFFER,
				AGL_NO_RECOVERY,
				AGL_RGBA,
				AGL_PIXEL_SIZE, 32,
				AGL_DEPTH_SIZE, 0,
				AGL_STENCIL_SIZE, 0,
				AGL_NONE,
				AGL_NONE
			};
			
			GLboolean	result;
			
			formatAttributes[11] = (displayFlags & kDisplayFullscreen) ? AGL_FULLSCREEN : AGL_NONE;
			
			GDHandle device = TheDisplayMgr->GetDisplayDevice();
			AGLPixelFormat format = aglChoosePixelFormat(&device, 1, formatAttributes);
			if (!format) return (kGraphicsNoHardware);
			
			openglContext = aglCreateContext(format, 0);
			aglDestroyPixelFormat(format);
			
			if (!openglContext) return (kGraphicsFormatFailed);
			aglSetCurrentContext(openglContext);
			
			if (displayFlags & kDisplayFullscreen) result = aglSetFullScreen(openglContext, 0, 0, 0, 0);
			else result = aglSetDrawable(openglContext, TheDisplayMgr->GetDisplayPort());
			if (!result)
			{
				aglDestroyContext(openglContext);
				return (kGraphicsContextFailed);
			}
			
			openglBundle = TheEngine->GetOpenGLBundle();
			
			GLint swapInterval = ((displayFlags & kDisplayRefreshSync) != 0);
			aglSetInteger(openglContext, AGL_SWAP_INTERVAL, &swapInterval);
		
		#elif C4LINUX
		
			static int visualAttributes[] =
			{
				GLX_DOUBLEBUFFER,
				GLX_RGBA,
				GLX_DEPTH_SIZE, 0,
				GLX_STENCIL_SIZE, 0,
				None
			};
			
			XSetWindowAttributes	windowAttributes;
			
			openglDisplay = TheEngine->GetEngineDisplay();
			
			XVisualInfo *visualInfo = glXChooseVisual(openglDisplay, DefaultScreen(openglDisplay), visualAttributes);
			if (!visualInfo) return (kGraphicsFormatFailed);
			
			::Window engineWindow = TheEngine->GetEngineWindow();
			openglColormap = XCreateColormap(openglDisplay, engineWindow, visualInfo->visual, AllocNone);
			
			windowAttributes.override_redirect = true;
			windowAttributes.colormap = openglColormap;
			openglWindow = XCreateWindow(openglDisplay, engineWindow, 0, 0, displayWidth, displayHeight, 0, visualInfo->depth, InputOutput, visualInfo->visual, CWOverrideRedirect | CWColormap, &windowAttributes);
			
			openglContext = glXCreateContext(openglDisplay, visualInfo, nullptr, true);
			XFree(visualInfo);
			
			if (!openglContext)
			{
				XDestroyWindow(openglDisplay, openglWindow);
				XFreeColormap(openglDisplay, openglColormap);
				return (kGraphicsContextFailed);
			}
			
			if (!glXMakeCurrent(openglDisplay, openglWindow, openglContext))
			{
				XDestroyWindow(openglDisplay, openglWindow);
				XFreeColormap(openglDisplay, openglColormap);
				return (kGraphicsContextFailed);
			}
			
			XMapWindow(openglDisplay, openglWindow);
			
			InitializeGlxExtensions();
			
			if (capabilities.windowSystemExtensionFlag[kWindowSystemExtensionSwapControl])
			{
				int32 swapInterval = ((displayFlags & kDisplayRefreshSync) != 0);
				if ((capabilities.windowSystemExtensionFlag[kWindowSystemExtensionSwapControlTear]) && (displayFlags & kDisplaySyncTear)) swapInterval = -swapInterval;
				glXSwapIntervalEXT(openglDisplay, openglWindow, swapInterval);
			}
		
		#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

		#endif //]
		
		DetermineHardwareType();
	
	#endif
	
	return (kEngineOkay);
}

void GraphicsMgr::TerminateGraphicsContext(void)
{
	#if !C4SERVER
	
		#if C4WINDOWS
		
			wglMakeCurrent(nullptr, nullptr);
			wglDeleteContext(openglContext);
			ReleaseDC(TheDisplayMgr->GetDisplayWindow(), deviceContext);
		
		#elif C4MACOS
		
			aglSetDrawable(openglContext, 0);
			aglDestroyContext(openglContext);
		
		#elif C4LINUX
		
			glXMakeCurrent(openglDisplay, None, nullptr);
			glXDestroyContext(openglDisplay, openglContext);
			
			XDestroyWindow(openglDisplay, openglWindow);
			XFreeColormap(openglDisplay, openglColormap);
		
		#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

		#endif //]
	
	#endif
	
	Render::Terminate();
}

void GraphicsMgr::DetermineHardwareType(void)
{
	#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#else //]
	
		capabilities.hardwareType = kHardwareUnknown;
		capabilities.hardwareSpeed = 0;
		
		#if !C4SERVER
		
			const char *vendor = GetOpenGLVendor();
			if (Text::FindText(vendor, "NVIDIA") >= 0)
			{
				capabilities.hardwareType = kHardwareNV;
			}
			else if ((Text::FindText(vendor, "ATI") >= 0) || (Text::FindText(vendor, "AMD") >= 0))
			{
				capabilities.hardwareType = kHardwareATI;
				
				#if C4LINUX
				
					// [HACK] GL_ARB_texture_array broken in AMD driver.
					extensionData[kExtensionTextureArray].enabled = false;
				
				#endif
			}
			
			const char *renderer = GetOpenGLRenderer();
			for (machine a = 0;; a++)
			{
				unsigned_int32 c = renderer[a];
				if (c == 0) break;
				
				if (c - '0' < 10U)
				{
					c = renderer[a + 1];
					if ((c - '0' < 10U) && ((unsigned_int32) renderer[a + 2] - '0' < 10U))
					{
						if (c >= '5') capabilities.hardwareSpeed = (c < '8') ? 1 : 2;
						break;
					}
				}
			}
		
		#endif
	
	#endif
}

#if C4WINDOWS

	void GraphicsMgr::InitializeWglExtensions(PIXELFORMATDESCRIPTOR *formatDescriptor)
	{
		static const wchar_t name[] = L"wgl";
		
		WNDCLASSEXW		windowClass;
		
		HINSTANCE instance = TheEngine->GetInstance();
		
		windowClass.cbSize = sizeof(WNDCLASSEXW);
		windowClass.style = CS_NOCLOSE | CS_OWNDC;
		windowClass.lpfnWndProc = &WglWindowProc;
		windowClass.cbClsExtra = 0;
		windowClass.cbWndExtra = 0;
		windowClass.hInstance = instance;
		windowClass.hIcon = nullptr;
		windowClass.hCursor = nullptr;
		windowClass.hbrBackground = nullptr;
		windowClass.lpszMenuName = nullptr;
		windowClass.lpszClassName = name;
		windowClass.hIconSm = nullptr;
		
		RegisterClassExW(&windowClass);
		HWND window = CreateWindowExW(0, name, name, WS_POPUP, 0, 0, 32, 32, nullptr, nullptr, instance, formatDescriptor);
		DestroyWindow(window);
		UnregisterClassW(name, instance);
	}
	
	LRESULT CALLBACK GraphicsMgr::WglWindowProc(HWND window, UINT message, WPARAM wparam, LPARAM lparam)
	{
		if (message == WM_CREATE)
		{
			TheGraphicsMgr->windowSystemExtensionsString = nullptr;
			bool *windowSystemExtensionFlag = TheGraphicsMgr->capabilities.windowSystemExtensionFlag;
			for (machine a = 0; a < kWindowSystemExtensionCount; a++) windowSystemExtensionFlag[a] = false;
			
			const CREATESTRUCTW *createStruct = (CREATESTRUCTW *) lparam;
			const PIXELFORMATDESCRIPTOR *formatDescriptor = (PIXELFORMATDESCRIPTOR *) createStruct->lpCreateParams;
			
			HDC deviceContext = GetDC(window);
			int pixelFormat = ChoosePixelFormat(deviceContext, formatDescriptor);
			if (pixelFormat != 0)
			{
				if (SetPixelFormat(deviceContext, pixelFormat, formatDescriptor))
				{
					HGLRC openglContext = wglCreateContext(deviceContext);
					if (wglMakeCurrent(deviceContext, openglContext))
					{
						GLGETEXTFUNC(wglGetExtensionsStringARB);
						if (wglGetExtensionsStringARB)
						{
							const char *string = wglGetExtensionsStringARB(deviceContext);
							
							const WindowSystemExtensionData *data = windowSystemExtensionData;
							for (machine a = 0; a < kWindowSystemExtensionCount; a++)
							{
								windowSystemExtensionFlag[a] = ((data->enabled) && (Text::FindText(string, data->name) >= 0));
								data++;
							}
							
							if (windowSystemExtensionFlag[kWindowSystemExtensionCreateContext]) GLGETEXTFUNC(wglCreateContextAttribsARB);
							if (windowSystemExtensionFlag[kWindowSystemExtensionPixelFormat]) GLGETEXTFUNC(wglChoosePixelFormatARB);
							if (windowSystemExtensionFlag[kWindowSystemExtensionSwapControl]) GLGETEXTFUNC(wglSwapIntervalEXT);
						}
						
						wglMakeCurrent(nullptr, nullptr);
					}
					
					wglDeleteContext(openglContext);
				}
			}
			
			ReleaseDC(window, deviceContext);
			return (0);
		}
		
		return (DefWindowProcW(window, message, wparam, lparam));
	}

#elif C4LINUX

	void GraphicsMgr::InitializeGlxExtensions(void)
	{
		const char *string = glXQueryExtensionsString(openglDisplay, DefaultScreen(openglDisplay));
		windowSystemExtensionsString = string;
		
		bool *windowSystemExtensionFlag = capabilities.windowSystemExtensionFlag;
		const WindowSystemExtensionData *data = windowSystemExtensionData;
		for (machine a = 0; a < kWindowSystemExtensionCount; a++)
		{
			windowSystemExtensionFlag[a] = ((data->enabled) && (Text::FindText(string, data->name) >= 0));
			data++;
		}
		
		if (windowSystemExtensionFlag[kWindowSystemExtensionSwapControl]) GLGETEXTFUNC(glXSwapIntervalEXT);
	}

#endif

bool GraphicsMgr::InitializeOpenglExtensions(void)
{
	#if C4OPENGL
	
		const char *string = reinterpret_cast<const char *>(glGetString(GL_VERSION));
		
		int32 major = 0;
		for (;;)
		{
			unsigned_int32 c = (unsigned_int32) *string - 48U;
			if (c > 10U) break;
			major = major * 10 + c;
			string++;
		}
		
		int32 minor = 0;
		if (*string == '.')
		{
			for (;;)
			{
				unsigned_int32 c = (unsigned_int32) *++string - 48U;
				if (c > 10U) break;
				minor = minor * 10 + c;
			}
		}
		
		unsigned_int32 version = (major << 8) | (minor << 4);
		capabilities.openglVersion = version;
		
		if (version < 0x0200) return (false);
		
		glGetIntegerv(GL_MAX_TEXTURE_SIZE, reinterpret_cast<GLint *>(&capabilities.maxTextureSize));
		glGetIntegerv(GL_MAX_CUBE_MAP_TEXTURE_SIZE, reinterpret_cast<GLint *>(&capabilities.maxCubeTextureSize));
		glGetFloatv(GL_MAX_TEXTURE_LOD_BIAS, &capabilities.maxTextureLodBias);
		
		string = reinterpret_cast<const char *>(glGetString(GL_EXTENSIONS));
		extensionsString = string;
		
		bool *extensionFlag = capabilities.extensionFlag;
		for (machine a = 0; a < kGraphicsExtensionCount; a++) extensionFlag[a] = false;
		bool requiredExtensions = true;
		
		const GraphicsExtensionData *data = extensionData;
		for (machine a = 0; a < kGraphicsExtensionCount; a++)
		{
			bool flag = false;
			if (data->enabled)
			{
				flag = (version >= data->version);
				if (!flag)
				{
					const char *name1 = data->name1;
					const char *name2 = data->name2;
					if ((Text::FindText(string, name1) >= 0) || ((name2) && (Text::FindText(string, name2) >= 0))) flag = true;
					else if (data->required) requiredExtensions = false;
				}
			}
			
			extensionFlag[a] = flag;
			data++;
		}
		
		#if C4WINDOWS || C4LINUX
		
			C4::InitializeOpenglExtensions(&capabilities);
		
		#elif C4MACOS || C4IOS
		
			C4::InitializeOpenglExtensions(&capabilities, openglBundle);
		
		#endif
	
		if ((!requiredExtensions) || (capabilities.maxTextureCoordCount < 8) || (capabilities.maxTextureImageCount < 8)) return (false);
		
		bool *capabilityFlag = capabilities.capabilityFlag;
		for (machine a = 0; a < kGraphicsCapabilityCount; a++) capabilityFlag[a] = false;
		
		if (extensionFlag[kExtensionTextureCompressionS3TC])
		{
			capabilityFlag[kCapabilityTextureCompressionS3TC] = true;
		}
		
		if ((extensionFlag[kExtensionFramebufferSRGB]) && (extensionFlag[kExtensionTextureSRGB]))
		{
			capabilityFlag[kCapabilityFramebufferSRGB] = true;
		}
		
		if ((extensionFlag[kExtensionColorBufferFloat]) && (extensionFlag[kExtensionHalfFloatPixel]))
		{
			capabilityFlag[kCapabilityFramebufferFloat] = true;
		}
		
		if ((extensionFlag[kExtensionShaderBufferLoad]) && (extensionFlag[kExtensionVertexBufferUnifiedMemory]))
		{
			capabilityFlag[kCapabilityUnifiedMemory] = true;
		}
		
		if (extensionFlag[kExtensionOcclusionQuery])
		{
			GLint	count;
			
			glGetQueryiv(GL_SAMPLES_PASSED, GL_QUERY_COUNTER_BITS, &count);
			if (count >= 24) capabilityFlag[kCapabilityOcclusionQuery] = true;
		}
		
		if (extensionFlag[kExtensionTextureArray])
		{
			glGetIntegerv(GL_MAX_ARRAY_TEXTURE_LAYERS, reinterpret_cast<GLint *>(&capabilities.maxArrayTextureLayers));
			
			if (extensionFlag[kExtensionGpuProgram4])
			{
				capabilityFlag[kCapabilityTextureArray] = true;
				capabilityFlag[kCapabilityProgramTextureArray] = true;
			}
			
			if (extensionFlag[kExtensionGpuShader4])
			{
				capabilityFlag[kCapabilityTextureArray] = true;
				capabilityFlag[kCapabilityShaderTextureArray] = true;
			}
		}
		
		#if C4MACOS
		
			if (extensionFlag[kExtensionDepthBoundsTest])
			{
				// DBT is broken under Mac OS 10.5
				
				if (((TheEngine->GetSystemVersion() >> 4) & 0x0F) < 6) extensionFlag[kExtensionDepthBoundsTest] = false;
			}
		
		#endif
	
	#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#endif //]
	
	return (true);
}

void GraphicsMgr::InitializeProcessGrid(void)
{
	float w = 2.0F / (float) kProcessGridWidth;
	float h = 2.0F / (float) kProcessGridHeight;
	
	Point2D *vertex = processGridVertex;
	
	for (machine j = 0; j < kProcessGridHeight; j++)
	{
		float y = (float) j * h - 1.0F;
		
		for (machine i = 0; i < kProcessGridWidth; i++)
		{
			vertex->Set((float) i * w - 1.0F, y);
			vertex++;
		}
		
		vertex->Set(1.0F, y);
		vertex++;
	}
	
	for (machine i = 0; i < kProcessGridWidth; i++)
	{
		vertex->Set((float) i * w - 1.0F, 1.0F);
		vertex++;
	}
	
	vertex->Set(1.0F, 1.0F);
}

unsigned_int32 GraphicsMgr::InitializeVariables(void)
{
	unsigned_int32 flags = 0;
	unsigned_int32 disableFlags = kRenderOptionMotionBlur;
	
	unsigned_int32 speed = capabilities.hardwareSpeed;
	if (speed < 2)
	{
		disableFlags = kRenderOptionHorizonMapping | kRenderOptionStructureEffects | kRenderOptionDistortion | kRenderOptionGlowBloom;
		if (speed < 1) disableFlags |= kRenderOptionNormalizeBumps | kRenderOptionParallaxMapping | kRenderOptionTerrainBumps;
	}
	
	Variable *normalize = TheEngine->InitVariable("renderNormalizeBumps", (disableFlags & kRenderOptionNormalizeBumps) ? "0" : "1", kVariablePermanent);
	if (normalize->GetIntegerValue() != 0) flags |= kRenderOptionNormalizeBumps;
	normalize->AddObserver(&renderNormalizeBumpsObserver);
	
	Variable *parallax = TheEngine->InitVariable("renderParallaxMapping", (disableFlags & kRenderOptionParallaxMapping) ? "0" : "1", kVariablePermanent);
	if (parallax->GetIntegerValue() != 0) flags |= kRenderOptionParallaxMapping;
	parallax->AddObserver(&renderParallaxMappingObserver);
	
	Variable *horizon = TheEngine->InitVariable("renderHorizonMapping", (disableFlags & kRenderOptionHorizonMapping) ? "0" : "1", kVariablePermanent);
	if (horizon->GetIntegerValue() != 0) flags |= kRenderOptionHorizonMapping;
	horizon->AddObserver(&renderHorizonMappingObserver);
	
	Variable *terrain = TheEngine->InitVariable("renderTerrainBumps", (disableFlags & kRenderOptionTerrainBumps) ? "0" : "1", kVariablePermanent);
	if (terrain->GetIntegerValue() != 0) flags |= kRenderOptionTerrainBumps;
	terrain->AddObserver(&renderTerrainBumpsObserver);
	
	TheEngine->InitVariable("textureDetailLevel", "1", kVariablePermanent, &textureDetailLevelObserver);
	TheEngine->InitVariable("paletteDetailLevel", "0", kVariablePermanent, &paletteDetailLevelObserver);
	
	Variable *anisotropy = TheEngine->InitVariable("textureAnisotropy", (speed >= 2) ? "4" : "1", kVariablePermanent);
	textureFilterAnisotropy = anisotropy->GetIntegerValue();
	anisotropy->AddObserver(&textureAnisotropyObserver);
	
	Variable *structure = TheEngine->InitVariable("renderStructureEffects", (disableFlags & kRenderOptionStructureEffects) ? "0" : "1", kVariablePermanent);
	if (structure->GetIntegerValue() != 0) flags |= kRenderOptionStructureEffects;
	structure->AddObserver(&renderStructureEffectsObserver);
	
	TheEngine->InitVariable("postBrightness", "1.0", kVariablePermanent, &postBrightnessObserver);
	
	Variable *motionBlur = TheEngine->InitVariable("postMotionBlur", "0", kVariablePermanent);
	if (motionBlur->GetIntegerValue() != 0) flags |= kRenderOptionMotionBlur;
	motionBlur->AddObserver(&postMotionBlurObserver);
	
	Variable *distortion = TheEngine->InitVariable("postDistortion", (disableFlags & kRenderOptionDistortion) ? "0" : "1", kVariablePermanent);
	if (distortion->GetIntegerValue() != 0) flags |= kRenderOptionDistortion;
	distortion->AddObserver(&postDistortionObserver);
	
	Variable *glowBloom = TheEngine->InitVariable("postGlowBloom", (disableFlags & kRenderOptionGlowBloom) ? "0" : "1", kVariablePermanent);
	if (glowBloom->GetIntegerValue() != 0) flags |= kRenderOptionGlowBloom;
	glowBloom->AddObserver(&postGlowBloomObserver);
	
	return (flags);
}

void GraphicsMgr::HandleTextureDetailLevelEvent(Variable *variable)
{
	textureDetailLevel = MaxZero(Min(variable->GetIntegerValue(), 2));
	currentGraphicsState |= kGraphicsReactivateTextures;
	Texture::DeactivateAll();
}

void GraphicsMgr::HandlePaletteDetailLevelEvent(Variable *variable)
{
	paletteDetailLevel = MaxZero(Min(variable->GetIntegerValue(), 2));
	currentGraphicsState |= kGraphicsReactivateTextures;
	Texture::DeactivateAll();
}

void GraphicsMgr::HandleTextureAnisotropyEvent(Variable *variable)
{
	textureFilterAnisotropy = variable->GetIntegerValue();
	currentGraphicsState |= kGraphicsReactivateTextures;
	Texture::DeactivateAll();
	
	if (shadowFrameBuffer) shadowFrameBuffer->SetTextureAnisotropy(textureFilterAnisotropy);
}

void GraphicsMgr::HandleRenderNormalizeBumpsEvent(Variable *variable)
{
	unsigned_int32 flags = renderOptionFlags;
	
	if (variable->GetIntegerValue() != 0) flags |= kRenderOptionNormalizeBumps;
	else flags &= ~kRenderOptionNormalizeBumps;
	
	renderOptionFlags = flags;
	ResetShaders();
}

void GraphicsMgr::HandleRenderParallaxMappingEvent(Variable *variable)
{
	unsigned_int32 flags = renderOptionFlags;
	
	if (variable->GetIntegerValue() != 0) flags |= kRenderOptionParallaxMapping;
	else flags &= ~kRenderOptionParallaxMapping;
	
	renderOptionFlags = flags;
	ResetShaders();
}

void GraphicsMgr::HandleRenderHorizonMappingEvent(Variable *variable)
{
	unsigned_int32 flags = renderOptionFlags;
	
	if (variable->GetIntegerValue() != 0) flags |= kRenderOptionHorizonMapping;
	else flags &= ~kRenderOptionHorizonMapping;
	
	renderOptionFlags = flags;
	ResetShaders();
}

void GraphicsMgr::HandleRenderTerrainBumpsEvent(Variable *variable)
{
	unsigned_int32 flags = renderOptionFlags;
	
	if (variable->GetIntegerValue() != 0) flags |= kRenderOptionTerrainBumps;
	else flags &= ~kRenderOptionTerrainBumps;
	
	renderOptionFlags = flags;
	ResetShaders();
}

void GraphicsMgr::HandleRenderStructureEffectsEvent(Variable *variable)
{
	unsigned_int32 flags = renderOptionFlags;
	
	if (variable->GetIntegerValue() != 0) flags |= kRenderOptionStructureEffects;
	else flags &= ~kRenderOptionStructureEffects;
	
	renderOptionFlags = flags;
	ResetShaders();
}

void GraphicsMgr::HandlePostBrightnessEvent(Variable *variable)
{
	brightnessMultiplier = variable->GetFloatValue();
}

void GraphicsMgr::HandlePostMotionBlurEvent(Variable *variable)
{
	unsigned_int32 flags = renderOptionFlags;
	
	if (variable->GetIntegerValue() != 0) flags |= kRenderOptionMotionBlur;
	else flags &= ~kRenderOptionMotionBlur;
	
	renderOptionFlags = flags;
}

void GraphicsMgr::HandlePostDistortionEvent(Variable *variable)
{
	unsigned_int32 flags = renderOptionFlags;
	
	if (variable->GetIntegerValue() != 0) flags |= kRenderOptionDistortion;
	else flags &= ~kRenderOptionDistortion;
	
	renderOptionFlags = flags;
}

void GraphicsMgr::HandlePostGlowBloomEvent(Variable *variable)
{
	unsigned_int32 flags = renderOptionFlags;
	
	if (variable->GetIntegerValue() != 0) flags |= kRenderOptionGlowBloom;
	else flags &= ~kRenderOptionGlowBloom;
	
	renderOptionFlags = flags;
}

void GraphicsMgr::SetShaderTime(float time, float delta)
{
	Render::SetVertexProgramParameter4f(kVertexParamShaderTime, time, delta, 0.0F, 0.0F);
	Render::SetFragmentProgramParameter4f(kFragmentParamShaderTime, time * kInverseShaderTimePeriod, 0.0F, 0.0F, 0.0F);
}

void GraphicsMgr::SetImpostorDepthParams(float scale, float offset, float tangent)
{
	Render::SetVertexProgramParameter4f(kVertexParamImpostorDepth, scale, offset, tangent, 0.0F);
}

void GraphicsMgr::ResetShaders(void)
{
	ShaderData::Purge();
	VertexProgram::Flush();
	FragmentProgram::Flush();
}

void GraphicsMgr::InvalidateVertexBuffer(const VertexBuffer *buffer)
{
	for (machine array = 0; array < kMaxShaderArrayCount; array++)
	{
		if (currentVertexBuffer[array] == buffer) currentVertexBuffer[array] = nullptr;
	}
}

void GraphicsMgr::BeginRendering(void)
{
	Render::BeginRendering();
	
	unsigned_int32 graphicsState = currentGraphicsState;
	currentGraphicsState = graphicsState & ~kGraphicsReactivateTextures;
	
	if (graphicsState & kGraphicsReactivateTextures) Texture::ReactivateAll();
	
	for (machine a = 0; a < kGraphicsCounterCount; a++) graphicsCounter[a] = 0;
	
	if ((diagnosticFlags & kDiagnosticTimer) && (capabilities.extensionFlag[kExtensionTimerQuery]))
	{
		Render::BeginQuery(&timerQuery[frameCount & 3], Render::kQueryTimeElapsed);
	}
	
	Render::SetColor4ub(0, 0, 0, 0);
}

void GraphicsMgr::EndRendering(void)
{
	if ((diagnosticFlags & kDiagnosticTimer) && (capabilities.extensionFlag[kExtensionTimerQuery]))
	{
		Render::EndQuery(&timerQuery[frameCount & 3], Render::kQueryTimeElapsed);
		frameCount++;
	}
	
	#if !C4SERVER
	
		#if C4WINDOWS
		
			SwapBuffers(deviceContext);
		
		#elif C4MACOS
		
			aglSwapBuffers(openglContext);
		
		#elif C4LINUX
		
			glXSwapBuffers(openglDisplay, openglWindow);
		
		#endif
	
	#endif
	
	Render::EndRendering();
	
	#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#endif //]
}

unsigned_int64 GraphicsMgr::GetRenderingTime(void)
{
	unsigned_int32 flags = diagnosticFlags;
	if (flags & kDiagnosticTimer)
	{
		diagnosticFlags = flags & ~kDiagnosticTimer;
		if (capabilities.extensionFlag[kExtensionTimerQuery]) return (Render::GetQueryTimeElapsed(&timerQuery[(frameCount + 1) & 3]));
	}
	
	return (0);
}

void GraphicsMgr::SetFinalColorTransform(const ColorRGBA& scale, const ColorRGBA& bias)
{
	finalColorScale[0] = scale;
	finalColorBias = bias;
	colorTransformFlags = 0;
}

void GraphicsMgr::SetFinalColorTransform(const ColorRGBA& red, const ColorRGBA& green, const ColorRGBA& blue, const ColorRGBA& bias)
{
	finalColorScale[0] = red;
	finalColorScale[1] = green;
	finalColorScale[2] = blue;
	finalColorBias = bias;
	colorTransformFlags = kPostColorMatrix;
}

void GraphicsMgr::SetPostProcessingProgram(unsigned_int32 postFlags)
{
	static Link<FragmentProgram>		postProgram[kPostProgramCount];
	
	postFlags |= colorTransformFlags;
	if (postFlags & kPostColorMatrix)
	{
		ColorRGBA red = finalColorScale[0] * brightnessMultiplier;
		ColorRGBA green = finalColorScale[1] * brightnessMultiplier;
		ColorRGBA blue = finalColorScale[2] * brightnessMultiplier;
		Render::SetFragmentProgramParameter4fv(kFragmentParamConstant0, &red.red);
		Render::SetFragmentProgramParameter4fv(kFragmentParamConstant1, &green.red);
		Render::SetFragmentProgramParameter4fv(kFragmentParamConstant2, &blue.red);
	}
	else
	{
		ColorRGBA scale = finalColorScale[0] * brightnessMultiplier;
		Render::SetFragmentProgramParameter4fv(kFragmentParamConstant0, &scale.red);
	}
	
	ColorRGBA bias = finalColorBias * brightnessMultiplier;
	Render::SetFragmentProgramParameter4fv(kFragmentParamConstant3, &bias.red);
	
	FragmentProgram *fragmentProgram = postProgram[postFlags];
	if (!fragmentProgram)
	{
		Process			*colorProcess;
		Process			*positionProcess;
		ShaderGraph		shaderGraph;
		
		Process *transformProcess = new TransformPostProcess((postFlags & kPostColorMatrix) != 0);
		shaderGraph.AddElement(transformProcess);
		
		if (postFlags & kPostMotionBlur) colorProcess = new MotionBlurPostProcess((postFlags & kPostMotionBlurGradient) != 0);
		else colorProcess = new ColorPostProcess;
		shaderGraph.AddElement(colorProcess);
		
		if (postFlags & kPostDistortion) positionProcess = new DistortPostProcess;
		else positionProcess = new FragmentPositionProcess;
		shaderGraph.AddElement(positionProcess);
		
		new Route(positionProcess, colorProcess, 0);
		
		if (postFlags & kPostGlowBloom)
		{
			Process *glowProcess = new GlowPostProcess;
			shaderGraph.AddElement(glowProcess);
			
			new Route(colorProcess, glowProcess, 0);
			new Route(glowProcess, transformProcess, 0);
		}
		else
		{
			new Route(colorProcess, transformProcess, 0);
		}
		
		fragmentProgram = ShaderAttribute::CompilePostShader(&shaderGraph);
		postProgram[postFlags] = fragmentProgram;
	}
	
	Render::SetFragmentProgram(fragmentProgram);
}

void GraphicsMgr::SetDisplayRenderTarget(void)
{
	#if !C4SERVER
	
		static const float triangleVertex[6] = {-1.0F, -3.0F, 3.0F, 1.0F, -1.0F, 1.0F};
		
		unsigned_int32 graphicsState = currentGraphicsState;
		currentGraphicsState = graphicsState & ~(kGraphicsMotionBlurAvail | kGraphicsDistortionAvail | kGraphicsGlowBloomAvail);
		if (!(renderOptionFlags & kRenderOptionGlowBloom)) graphicsState &= ~kGraphicsGlowBloomAvail;
		
		if (multisampleFrameBuffer)
		{
			Render::SetDrawFrameBuffer(normalFrameBuffer);
			Render::SetReadFrameBuffer(multisampleFrameBuffer);
			
			normalFrameBuffer->SetColorRenderTexture(normalFrameBuffer->GetRenderTargetTexture(kRenderTargetPrimary));
			
			int32 left = cameraRect.left;
			int32 top = cameraRect.top;
			int32 right = cameraRect.right;
			int32 bottom = cameraRect.bottom;
			Render::BlitFrameBuffer(left, bottom, right, top, left, bottom, right, top, Render::kBlitFilterBilinear);
			
			normalFrameBuffer->ResetColorRenderTexture();
		}
		
		SetBlendState(BlendState(kBlendOne, kBlendZero, kBlendOne, kBlendZero));
		SetRenderState(kRenderDepthInhibit);
		SetMaterialState(nullptr, currentMaterialState & kMaterialTwoSided);
		ResetArrayState();
		
		Render::ResetAttributeVertexBuffer();
		
		currentVertexBuffer[kShaderArrayPosition0] = nullptr;
		shaderArrayPointer[kShaderArrayPosition0] = nullptr;
		
		const Render::TextureObject *mainTexture = normalFrameBuffer->GetRenderTargetTexture(kRenderTargetPrimary);
		Render::BindTexture(0, mainTexture);
		
		if (graphicsState & kGraphicsGlowBloomAvail)
		{
			static Link<VertexProgram>		glowVertexProgram;
			static Link<FragmentProgram>	glowFragmentProgram;
			
			Point2D		quadVertex[4];
			
			float y = -4.0F / (float) viewportRect.Height();
			
			#if C4OPENGL
			
				quadVertex[0].Set(-1.0F, -1.0F);
				quadVertex[1].Set(0.0F, -1.0F);
				quadVertex[2].Set(0.0F, y);
				quadVertex[3].Set(-1.0F, y);
			
			#else
			
				quadVertex[0].Set(-1.0F, -y);
				quadVertex[1].Set(0.0F, -y);
				quadVertex[2].Set(0.0F, 1.0F);
				quadVertex[3].Set(-1.0F, 1.0F);
			
			#endif
			
			SetRenderTarget(kRenderTargetGlowBloom);
			SetLocalVertexProgram(&glowVertexProgram, &VertexProgram::extractGlowTransform);
			
			FragmentProgram *fragmentProgram = glowFragmentProgram;
			if (!fragmentProgram)
			{
				ShaderGraph		shaderGraph;
				
				shaderGraph.AddElement(new ExtractPostProcess);
				fragmentProgram = ShaderAttribute::CompilePostShader(&shaderGraph);
				glowFragmentProgram = fragmentProgram;
			}
			
			Render::SetFragmentProgram(fragmentProgram);
			
			Render::SetVertexArray(2, Render::kVertexFloat, 8, quadVertex);
			Render::DrawArrays(Render::kPrimitiveQuads, 0, 4);
		}
		
		Render::ResetFrameBuffer();
		
		if (graphicsState & (kGraphicsMotionBlurAvail | kGraphicsDistortionAvail | kGraphicsGlowBloomAvail))
		{
			static Link<VertexProgram>		postVertexProgram;
			
			unsigned_int32 postFlags = 0;
			if (graphicsState & kGraphicsMotionBlurAvail) postFlags |= kPostMotionBlur;
			if (graphicsState & kGraphicsDistortionAvail) postFlags |= kPostDistortion;
			if (graphicsState & kGraphicsGlowBloomAvail) postFlags |= kPostGlowBloom;
			
			const Render::TextureObject *structureTexture = normalFrameBuffer->GetRenderTargetTexture(kRenderTargetStructure);
			const Render::TextureObject *distortionTexture = normalFrameBuffer->GetRenderTargetTexture(kRenderTargetDistortion);
			const Render::TextureObject *glowBloomTexture = normalFrameBuffer->GetRenderTargetTexture(kRenderTargetGlowBloom);
			
			if (postFlags & kPostDistortion) Render::BindTexture(2, distortionTexture);
			if (postFlags & kPostGlowBloom) Render::BindTexture(3, glowBloomTexture);
			
			SetLocalVertexProgram(&postVertexProgram, &VertexProgram::postProcessTransform);
			
			if (postFlags & kPostMotionBlur)
			{
				Render::BindTexture(1, structureTexture);
				Render::ResetIndexVertexBuffer();
				
				int32 basicCount = 0;
				int32 gradientCount = 0;
				Quad *basicQuad = processGridQuad;
				Quad *gradientQuad = basicQuad + kProcessGridWidth * kProcessGridHeight;
				
				const bool *flag = motionGridFlag;
				machine k = 0;
				
				for (machine j = 0; j < kProcessGridHeight; j++)
				{
					for (machine i = 0; i < kProcessGridWidth; i++)
					{
						if (!*flag)
						{
							basicQuad->Set(k, k + 1, k + kProcessGridWidth + 2, k + kProcessGridWidth + 1);
							basicQuad++;
							basicCount++;
						}
						else
						{
							gradientQuad--;
							gradientQuad->Set(k, k + 1, k + kProcessGridWidth + 2, k + kProcessGridWidth + 1);
							gradientCount++;
						}
						
						flag++;
						k++;
					}
					
					k++;
				}
				
				Render::SetVertexArray(2, Render::kVertexFloat, 8, processGridVertex);
				
				if (basicCount != 0)
				{
					SetPostProcessingProgram(postFlags);
					Render::DrawElements(Render::kPrimitiveQuads, (kProcessGridWidth + 1) * (kProcessGridHeight + 1), basicCount * 4, processGridQuad);
				}
				
				if (gradientCount != 0)
				{
					SetPostProcessingProgram(postFlags | kPostMotionBlurGradient);
					Render::DrawElements(Render::kPrimitiveQuads, (kProcessGridWidth + 1) * (kProcessGridHeight + 1), gradientCount * 4, gradientQuad);
				}
				
				structureTexture->Unbind(1);
				
				#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

				#endif //]
			}
			else
			{
				SetPostProcessingProgram(postFlags);
				Render::SetVertexArray(2, Render::kVertexFloat, 8, triangleVertex);
				Render::DrawArrays(Render::kPrimitiveTriangles, 0, 3);
			}
			
			if (postFlags & kPostDistortion) distortionTexture->Unbind(2);
			if (postFlags & kPostGlowBloom) glowBloomTexture->Unbind(3);
		}
		else
		{
			static Link<VertexProgram>		copyVertexProgram;
			
			SetLocalVertexProgram(&copyVertexProgram, &VertexProgram::nullTransform);
			SetPostProcessingProgram(0);
			
			Render::SetVertexArray(2, Render::kVertexFloat, 8, triangleVertex);
			Render::DrawArrays(Render::kPrimitiveTriangles, 0, 3);
		}
		
		mainTexture->Unbind(0);
		currentRenderTargetType = kRenderTargetDisplay;
	
	#endif
}

void GraphicsMgr::SetRenderTarget(RenderTargetType type)
{
	#if !C4SERVER
	
		RenderTargetType prev = currentRenderTargetType;
		if (prev != type)
		{
			if (type == kRenderTargetDisplay)
			{
				SetDisplayRenderTarget();
			}
			else
			{
				currentRenderTargetType = type;
				if ((type == kRenderTargetPrimary) && (multisampleFrameBuffer))
				{
					Render::SetFrameBuffer(multisampleFrameBuffer);
				}
				else
				{
					Render::SetFrameBuffer(normalFrameBuffer);
					normalFrameBuffer->SetColorRenderTexture(normalFrameBuffer->GetRenderTargetTexture(type));
				}
			}
		}
	
	#endif
}

void GraphicsMgr::CopyRenderTarget(Texture *texture, const Rect& rect)
{
	#if !C4SERVER
	
		if ((multisampleFrameBuffer) && (currentRenderTargetType == kRenderTargetPrimary))
		{
			Render::SetDrawFrameBuffer(&genericFrameBuffer);
			genericFrameBuffer.SetColorRenderTexture(texture);
			
			int32 w = rect.Width();
			int32 h = rect.Height();
			int32 left = rect.left;
			int32 bottom = renderTargetHeight - rect.bottom;
			Render::CopyFrameBuffer(left, bottom, left + w, bottom + h, 0, 0, w, h, Render::kBlitFilterPoint);
			
			genericFrameBuffer.ResetColorRenderTexture();
			Render::SetDrawFrameBuffer(multisampleFrameBuffer);
		}
		else
		{
			texture->BlitImageRect(rect.left, renderTargetHeight - rect.bottom, 0, 0, rect.Width(), rect.Height());
		}
	
	#endif
}

void GraphicsMgr::SetOrtho(const Rect& rect, float orthoLeft, float orthoRight, float orthoTop, float orthoBottom, float nearDepth, float farDepth)
{
	currentNearDepth = nearDepth;
	currentFrustumFlags = 0;
	
	cameraPosition4D = -cameraTransformable->GetWorldTransform()[2];
	
	Matrix4D& matrix = cameraProjectionMatrix;
	matrix(0,1) = matrix(0,2) = matrix(1,0) = matrix(1,2) = matrix(2,0) = matrix(2,1) = matrix(3,0) = matrix(3,1) = matrix(3,2) = 0.0F;
	matrix(3,3) = 1.0F;
	
	float dx = 1.0F / (orthoRight - orthoLeft);
	float dy = 1.0F / (orthoBottom - orthoTop);
	float dz = 1.0F / (farDepth - nearDepth);
	
	matrix(0,0) = 2.0F * dx;
	matrix(0,3) = -(orthoRight + orthoLeft) * dx;
	matrix(1,1) = 2.0F * dy;
	matrix(1,3) = -(orthoBottom + orthoTop) * dy;
	matrix(2,2) = -2.0F * dz;
	matrix(2,3) = -(farDepth + nearDepth) * dz;
	
	currentProjectionMatrix = matrix;
	depthOffsetConstant = 0.0F;
	
	currentGraphicsState &= ~(kGraphicsScissorMask | kGraphicsDepthBoundsAvail | kGraphicsObliqueFrustum);
	currentRenderState &= ~kRenderDepthOffset;
	disabledRenderState = kRenderDepthOffset;
	
	int32 left = rect.left;
	int32 right = rect.right;
	int32 bottom = renderTargetHeight - rect.bottom;
	int32 top = renderTargetHeight - rect.top;
	int32 width = right - left;
	int32 height = top - bottom;
	
	viewportRect.Set(left, top, right, bottom);
	cameraRect.Set(left, top, right, bottom);
	renderTargetOffsetSize = (float) width;
	
	Render::SetVertexProgramParameter4f(kVertexParamViewportTransform, (float) width, (float) height, (float) left, (float) bottom);
	Render::SetViewport(left, bottom, width, height);
	
	if (currentGraphicsState & kGraphicsClipEnabled)
	{
		left = Max(left, clipRect.left);
		right = Min(right, clipRect.right);
		bottom = Max(bottom, renderTargetHeight - clipRect.bottom);
		top = Min(top, renderTargetHeight - clipRect.top);
		
		right = Max(left, right);
		top = Max(bottom, top);
		
		width = right - left;
		height = top - bottom;
		
		scissorRect.Set(left, top, right, bottom);
	}
	else
	{
		scissorRect = cameraRect;
	}
	
	Render::SetScissor(left, bottom, width, height);
}

void GraphicsMgr::SetFrustum(const Rect& rect, float focalLength, float aspectRatio, float nearDepth, float farDepth, unsigned_int32 flags)
{
	currentNearDepth = nearDepth;
	currentFrustumFlags = flags;
	
	cameraPosition4D = cameraTransformable->GetWorldPosition();
	
	Matrix4D& matrix = cameraProjectionMatrix;
	matrix(0,1) = matrix(0,2) = matrix(0,3) = matrix(1,0) = matrix(1,2) = matrix(1,3) = matrix(2,0) = matrix(2,1) = matrix(3,0) = matrix(3,1) = matrix(3,3) = 0.0F;
	matrix(3,2) = -1.0F;
	
	matrix(0,0) = focalLength;
	matrix(1,1) = focalLength / aspectRatio;
	
	unsigned_int32 graphicsState = currentGraphicsState & ~(kGraphicsScissorMask | kGraphicsDepthBoundsAvail | kGraphicsObliqueFrustum);
	if (capabilities.extensionFlag[kExtensionDepthBoundsTest]) graphicsState |= kGraphicsDepthBoundsAvail;
	currentGraphicsState = graphicsState;
	
	if (flags & kFrustumInfinite)
	{
		matrix(2,2) = kFrustumEpsilon - 1.0F;
		matrix(2,3) = nearDepth * (kFrustumEpsilon - 2.0F);
	}
	else
	{
		float d = -1.0F / (farDepth - nearDepth);
		float k = 2.0F * farDepth * nearDepth;
		
		matrix(2,2) = (farDepth + nearDepth) * d;
		matrix(2,3) = k * d;
	}
	
	standardProjectionMatrix = matrix;
	currentProjectionMatrix = matrix;
	depthOffsetConstant = -matrix(2,3) / matrix(2,2);
	
	currentRenderState &= ~kRenderDepthOffset;
	disabledRenderState = 0;
	
	int32 left = rect.left;
	int32 right = rect.right;
	int32 bottom = renderTargetHeight - rect.bottom;
	int32 top = renderTargetHeight - rect.top;
	int32 width = right - left;
	int32 height = top - bottom;
	
	viewportRect.Set(left, top, right, bottom);
	cameraRect.Set(left, top, right, bottom);
	renderTargetOffsetSize = (float) width;
	
	Render::SetVertexProgramParameter4f(kVertexParamViewportTransform, (float) width, (float) height, (float) left, (float) bottom);
	Render::SetViewport(left, bottom, width, height);
	
	if (currentGraphicsState & kGraphicsClipEnabled)
	{
		left = Max(left, clipRect.left);
		right = Min(right, clipRect.right);
		bottom = Max(bottom, renderTargetHeight - clipRect.bottom);
		top = Min(top, renderTargetHeight - clipRect.top);
		
		right = Max(left, right);
		top = Max(bottom, top);
		
		width = right - left;
		height = top - bottom;
		
		scissorRect.Set(left, top, right, bottom);
	}
	else
	{
		scissorRect = cameraRect;
	}
	
	Render::SetScissor(left, bottom, width, height);
}

void GraphicsMgr::SetSubfrustum(const Rect& rect, const ProjectionRect& frustumBoundary, float focalLength, float aspectRatio, float nearDepth, float farDepth, unsigned_int32 flags, const Antivector4D& worldClipPlane)
{
	currentNearDepth = nearDepth;
	currentFrustumFlags = flags;
	
	cameraPosition4D = cameraTransformable->GetWorldPosition();
	
	Matrix4D& matrix = cameraProjectionMatrix;
	matrix(0,1) = matrix(0,2) = matrix(0,3) = matrix(1,0) = matrix(1,2) = matrix(1,3) = matrix(2,0) = matrix(2,1) = matrix(3,0) = matrix(3,1) = matrix(3,3) = 0.0F;
	matrix(3,2) = -1.0F;
	
	matrix(0,0) = focalLength;
	matrix(1,1) = focalLength / aspectRatio;
	
	unsigned_int32 graphicsState = currentGraphicsState & ~(kGraphicsScissorMask | kGraphicsDepthBoundsAvail | kGraphicsObliqueFrustum);
	
	if (flags & kFrustumInfinite)
	{
		matrix(2,2) = kFrustumEpsilon - 1.0F;
		matrix(2,3) = nearDepth * (kFrustumEpsilon - 2.0F);
	}
	else
	{
		float d = -1.0F / (farDepth - nearDepth);
		float k = 2.0F * farDepth * nearDepth;
		
		matrix(2,2) = (farDepth + nearDepth) * d;
		matrix(2,3) = k * d;
	}
	
	standardProjectionMatrix = matrix;
	
	if (flags & kFrustumOblique)
	{
		Antivector4D clipPlane = worldClipPlane * Inverse(cameraSpaceTransform);
		if (clipPlane.w < 0.0F)
		{
			float qpx = (clipPlane.x < 0.0F) ? -1.0F : 1.0F;
			float qpy = (clipPlane.y < 0.0F) ? -1.0F : 1.0F;
			
			float qx = (qpx + matrix(0,2)) / matrix(0,0);
			float qy = (qpy + matrix(1,2)) / matrix(1,1);
			float qw = (1.0F + matrix(2,2)) / matrix(2,3);
			
			float scale = 2.0F / (clipPlane.x * qx + clipPlane.y * qy - clipPlane.z + clipPlane.w * qw);
			
			matrix(2,0) = scale * clipPlane.x;
			matrix(2,1) = scale * clipPlane.y;
			matrix(2,2) = scale * clipPlane.z + 1.0F;
			matrix(2,3) = scale * clipPlane.w;
			
			graphicsState |= kGraphicsObliqueFrustum;
		}
	}
	else
	{
		if (capabilities.extensionFlag[kExtensionDepthBoundsTest]) graphicsState |= kGraphicsDepthBoundsAvail;
	}
	
	currentGraphicsState = graphicsState;
	currentProjectionMatrix = matrix;
	depthOffsetConstant = -matrix(2,3) / matrix(2,2);
	
	currentRenderState &= ~kRenderDepthOffset;
	disabledRenderState = 0;
	
	int32 left = rect.left;
	int32 right = rect.right;
	int32 bottom = renderTargetHeight - rect.bottom;
	int32 top = renderTargetHeight - rect.top;
	int32 width = right - left;
	int32 height = top - bottom;
	
	viewportRect.Set(left, top, right, bottom);
	renderTargetOffsetSize = (float) width;
	
	Render::SetVertexProgramParameter4f(kVertexParamViewportTransform, (float) width, (float) height, (float) left, (float) bottom);
	Render::SetViewport(left, bottom, width, height);
	
	float x1 = frustumBoundary.left * 0.5F + 0.5F;
	float x2 = frustumBoundary.right * 0.5F + 0.5F;
	float y1 = frustumBoundary.bottom * 0.5F + 0.5F;
	float y2 = frustumBoundary.top * 0.5F + 0.5F;
	
	float w = (float) width * 0.5F;
	float h = (float) height * 0.5F;
	
	int32 cameraLeft = left + (int32) (x1 * w * 2.0F);
	int32 cameraRight = left + (int32) (x2 * w * 2.0F + 0.5F);
	int32 cameraBottom = bottom + (int32) (y1 * h * 2.0F);
	int32 cameraTop = bottom + (int32) (y2 * h * 2.0F + 0.5F);
	
	cameraRect.Set(cameraLeft, cameraTop, cameraRight, cameraBottom);
	
	if (currentGraphicsState & kGraphicsClipEnabled)
	{
		left = Max(left, clipRect.left);
		right = Min(right, clipRect.right);
		bottom = Max(bottom, renderTargetHeight - clipRect.bottom);
		top = Min(top, renderTargetHeight - clipRect.top);
		
		right = Max(left, right);
		top = Max(bottom, top);
		
		scissorRect.Set(left, top, right, bottom);
		Render::SetScissor(left, bottom, right - left, top - bottom);
	}
	else
	{
		scissorRect = cameraRect;
		Render::SetScissor(cameraLeft, cameraBottom, cameraRight - cameraLeft, cameraTop - cameraBottom);
	}
}

void GraphicsMgr::SetCamera(const CameraObject *camera, const Transformable *transformable, unsigned_int32 clearMask, bool reset)
{
	cameraObject = camera;
	cameraTransformable = transformable;
	
	const Transform4D& worldTransform = (transformable) ? transformable->GetWorldTransform() : K::identity_4D;
	const Vector3D& rightDirection = worldTransform[0];
	const Vector3D& downDirection = worldTransform[1];
	const Vector3D& viewDirection = worldTransform[2];
	
	if (camera->GetCameraType() != kCameraOrtho)
	{
		const FrustumCameraObject *object = static_cast<const FrustumCameraObject *>(camera);
		const Rect& rect = object->GetViewRect();
		float viewWidth = (float) rect.Width();
		
		float focal = object->GetFocalLength();
		float factor = focal * viewWidth;
		Render::SetVertexProgramParameter4f(kVertexParamRadiusPointFactor, factor, 0.0F, 0.0F, 0.0F);
		
		factor = 1.0F / factor;
		float d = -(viewDirection * worldTransform.GetTranslation());
		Render::SetVertexProgramParameter4f(kVertexParamPointCameraPlane, viewDirection.x * factor, viewDirection.y * factor, viewDirection.z * factor, d * factor);
		
		focal = 1.0F / focal;
		distortionPlane.Set(viewDirection.x * focal, viewDirection.y * focal, viewDirection.z * focal, d * focal);
		
		occlusionAreaNormalizer = 1.0F / (viewWidth * (float) rect.Height());
	}
	
	cameraSpaceTransform(0,0) = rightDirection.x;
	cameraSpaceTransform(0,1) = rightDirection.y;
	cameraSpaceTransform(0,2) = rightDirection.z;
	cameraSpaceTransform(1,0) = -downDirection.x;
	cameraSpaceTransform(1,1) = -downDirection.y;
	cameraSpaceTransform(1,2) = -downDirection.z;
	cameraSpaceTransform(2,0) = -viewDirection.x;
	cameraSpaceTransform(2,1) = -viewDirection.y;
	cameraSpaceTransform(2,2) = -viewDirection.z;
	
	const Vector3D& worldOffset = worldTransform[3];
	cameraSpaceTransform(0,3) = -(cameraSpaceTransform.GetRow(0) ^ worldOffset);
	cameraSpaceTransform(1,3) = -(cameraSpaceTransform.GetRow(1) ^ worldOffset);
	cameraSpaceTransform(2,3) = -(cameraSpaceTransform.GetRow(2) ^ worldOffset);
	
	camera->Activate();
	unsigned_int32 graphicsState = currentGraphicsState;
	
	if (Determinant(worldTransform) > 0.0F)
	{
		if (!(graphicsState & kGraphicsFrontFaceCCW))
		{
			graphicsState |= kGraphicsFrontFaceCCW;
			Render::SetFrontFace(Render::kFrontCCW);
		}
	}
	else
	{
		if (graphicsState & kGraphicsFrontFaceCCW)
		{
			graphicsState &= ~kGraphicsFrontFaceCCW;
			Render::SetFrontFace(Render::kFrontCW);
		}
	}
	
	unsigned_int32 clearFlags = camera->GetClearFlags() & clearMask;
	if (clearFlags != 0)
	{
		unsigned_int32 bufferMask = 0;
		unsigned_int32 renderState = currentRenderState;
		
		if (clearFlags & kClearColorBuffer)
		{
			bufferMask |= Render::kClearBufferColor;
			const ColorRGBA& color = camera->GetClearColor();
			Render::SetClearColor(color.red, color.green, color.blue, color.alpha);
			
			if (renderState & kRenderColorInhibit)
			{
				Render::SetColorMask(true, true, true, true);
				renderState &= ~kRenderColorInhibit;
			}
		}
		
		if (clearFlags & kClearDepthBuffer)
		{
			bufferMask |= Render::kClearBufferDepth;
			
			if (renderState & kRenderDepthInhibit)
			{
				Render::SetDepthMask(true);
				renderState &= ~kRenderDepthInhibit;
			}
		}
		
		if (clearFlags & kClearStencilBuffer)
		{
			bufferMask |= Render::kClearBufferStencil;
			
			if (!(graphicsState & kGraphicsRenderShadowMap))
			{
				graphicsState |= kGraphicsStencilClear;
				graphicsCounter[kGraphicsCounterStencilClears]++;
			}
		}
		
		currentRenderState = renderState;
		Render::Clear(bufferMask);
	}
	
	currentGraphicsState = graphicsState;
	
	if (graphicsState & kGraphicsRenderShadowMap)
	{
		float cp = Magnitude(viewDirection.GetVector2D());
		float elevation = Atan(-viewDirection.z, cp) * K::degrees;
		
		float scale = 1.0F / (cp * (camera->GetFarDepth() - camera->GetNearDepth()));
		
		if (elevation < 30.5F)
		{
			float t = FmaxZero(elevation - 14.5F) * 0.0625F;
			Render::SetFragmentProgramParameter4f(kFragmentParamImpostorShadowBlend, 1.0F - t, t, 0.0F, 0.0F);
			Render::SetFragmentProgramParameter4f(kFragmentParamImpostorShadowScale, scale * (1.0F - t), scale * t, 0.0F, 0.0F);
		}
		else if (elevation < 45.5F)
		{
			float t = FmaxZero(elevation - 29.5F) * 0.0625F;
			Render::SetFragmentProgramParameter4f(kFragmentParamImpostorShadowBlend, 0.0F, 1.0F - t, t, 0.0F);
			Render::SetFragmentProgramParameter4f(kFragmentParamImpostorShadowScale, 0.0F, scale * (1.0F - t), scale * t, 0.0F);
		}
		else if (elevation < 60.5F)
		{
			float t = FmaxZero(elevation - 44.5F) * 0.0625F;
			Render::SetFragmentProgramParameter4f(kFragmentParamImpostorShadowBlend, 0.0F, 0.0F, 1.0F - t, t);
			Render::SetFragmentProgramParameter4f(kFragmentParamImpostorShadowScale, 0.0F, 0.0F, scale * (1.0F - t), scale * t);
		}
		else
		{
			Render::SetFragmentProgramParameter4f(kFragmentParamImpostorShadowBlend, 0.0F, 0.0F, 0.0F, 1.0F);
			Render::SetFragmentProgramParameter4f(kFragmentParamImpostorShadowScale, 0.0F, 0.0F, 0.0F, scale);
		}
	}
	else
	{
		const Point3D& position = worldTransform.GetTranslation();
		directCameraPosition = position;
		
		Render::SetVertexProgramParameter4f(kVertexParamImpostorCameraPosition, position.x, position.y, position.z, 1.0F);
	}
	
	if (reset)
	{
		fogSpaceObject = nullptr;
		fogSpaceTransformable = nullptr;
		
		currentShaderVariant = kShaderVariantNormal;
		SetAmbient();
	}
}

void GraphicsMgr::SetFogSpace(const FogSpaceObject *fogSpace, const Transformable *transformable)
{
	fogSpaceObject = fogSpace;
	fogSpaceTransformable = transformable;
	
	if (fogSpace)
	{
		float	f1, f2;
		
		const Transform4D& m = transformable->GetInverseWorldTransform();
		const MatrixRow4D& plane = m.GetRow(2);
		worldFogPlane = plane;
		
		float F_dot_C = plane ^ cameraTransformable->GetWorldPosition();
		if (F_dot_C > 0.0F)
		{
			f1 = 0.0F;
			f2 = 1.0F;
		}
		else
		{
			f1 = 1.0F;
			f2 = -1.0F;
		}
		
		float density = fogSpace->GetFogDensity();
		Render::SetVertexProgramParameter4f(kVertexParamFogParams, F_dot_C, f1, f2, density * K::one_over_ln_2);
		Render::SetFragmentProgramParameter4f(kFragmentParamFogParams, f1, 0.0F, 0.0F, 0.0F);
		Render::SetFragmentProgramParameter4fv(kFragmentParamFogColor, &fogSpace->GetFogColor().red);
		
		currentShaderVariant = (fogSpace->GetFogFunction() == kFogFunctionLinear) ? kShaderVariantLinearFog : kShaderVariantConstantFog;
	}
	else
	{
		currentShaderVariant = kShaderVariantNormal;
	}
}

void GraphicsMgr::SetAmbient(void)
{
	if (lightObject)
	{
		lightObject = nullptr;
		shadowFrameBuffer->GetRenderTargetTexture()->SetCompareMode(Render::kTextureCompareNone);
	}
	
	lightTransformable = nullptr;
	geometryTransformable = nullptr;
	
	unsigned_int32 oldGraphicsState = currentGraphicsState;
	unsigned_int32 newGraphicsState = oldGraphicsState & ~(kGraphicsAmbientLessEqual | kGraphicsScissorMask | kGraphicsDepthBoundsMask | kGraphicsRenderLight);
	
	currentGraphicsState = newGraphicsState;
	unsigned_int32 changed = (newGraphicsState ^ oldGraphicsState) & (kGraphicsRenderLight | kGraphicsScissorMask | kGraphicsDepthBoundsMask);
	if (changed != 0)
	{
		if (changed & kGraphicsRenderLight) Render::DisableStencilTest();
		if (changed & kGraphicsDepthBoundsMask) Render::DisableDepthBoundsTest();
		
		if (changed & kGraphicsScissorMask)
		{
			int32 left = scissorRect.left;
			int32 bottom = scissorRect.bottom;
			Render::SetScissor(left, bottom, scissorRect.right - left, scissorRect.top - bottom);
		}
	}
	
	currentShaderType = kShaderAmbient;
}

void GraphicsMgr::BeginClip(const Rect& rect)
{
	currentGraphicsState |= kGraphicsClipEnabled;
	clipRect = rect;
	
	int32 left = Max(cameraRect.left, rect.left);
	int32 bottom = Max(cameraRect.bottom, viewportRect.top - rect.bottom);
	int32 right = Min(cameraRect.right, rect.right);
	int32 top = Min(cameraRect.top, viewportRect.top - rect.top);
	
	scissorRect.Set(left, top, right, bottom);
	Render::SetScissor(left, bottom, right - left, top - bottom);
}

void GraphicsMgr::EndClip(void)
{
	currentGraphicsState &= ~kGraphicsClipEnabled;
	scissorRect = cameraRect;
	
	int32 left = cameraRect.left;
	int32 bottom = cameraRect.bottom;
	Render::SetScissor(left, bottom, cameraRect.right - left, cameraRect.top - bottom);
}

bool GraphicsMgr::SetLight(const LightObject *light, const Transformable *transformable, const LightShadowData *shadowData)
{
	ShaderType	shaderType;
	
	lightObject = light;
	lightTransformable = transformable;
	
	geometryTransformable = nullptr;
	
	const ColorRGB& lightColor = light->GetLightColor();
	Render::SetFragmentProgramParameter4f(kFragmentParamLightColor, lightColor.red, lightColor.green, lightColor.blue, 1.0F);
	
	unsigned_int32 oldGraphicsState = currentGraphicsState;
	unsigned_int32 newGraphicsState = (oldGraphicsState | kGraphicsRenderLight) & ~(kGraphicsScissorMask | kGraphicsDepthBoundsMask);
	
	LightType lightType = light->GetLightType();
	if (light->GetBaseLightType() != kLightInfinite)
	{
		switch (lightType)
		{
			case kLightPoint:
				
				shaderType = kShaderPointLight;
				break;
			
			case kLightCube:
				
				shaderType = kShaderCubeLight;
				Render::BindTexture(kTextureUnitLightProjection, static_cast<const CubeLightObject *>(light)->GetShadowMap());
				break;
			
			case kLightSpot:
				
				shaderType = kShaderSpotLight;
				Render::BindTexture(kTextureUnitLightProjection, static_cast<const SpotLightObject *>(light)->GetShadowMap());
				break;
		}
		
		const PointLightObject *pointLight = static_cast<const PointLightObject *>(light);
		float r = pointLight->GetLightRange();
		float s = 1.0F / r;
		
		Render::SetVertexProgramParameter4f(kVertexParamLightRange, r, 0.0F, 0.0F, s);
		
		if (cameraObject->GetCameraType() != kCameraOrtho)
		{
			ProjectionRect		projectionRect;
			
			const FrustumCameraObject *frustumCamera = static_cast<const FrustumCameraObject *>(cameraObject);
			Point3D center = cameraSpaceTransform * transformable->GetWorldPosition();
			
			ProjectionResult result = frustumCamera->ProjectSphere(center, r, &projectionRect);
			if (result == kProjectionEmpty) return (false);
			
			if (result == kProjectionPartial)
			{
				const Rect& viewRect = frustumCamera->GetViewRect();
				
				float viewLeft = (float) viewRect.left;
				float viewBottom = (float) (renderTargetHeight - viewRect.bottom);
				float viewWidth = (float) viewRect.Width() * 0.5F;
				float viewHeight = (float) viewRect.Height() * 0.5F;
				
				int32 scissorLeft = Max(scissorRect.left, (int32) (viewLeft + viewWidth * (projectionRect.left + 1.0F)));
				int32 scissorRight = Min(scissorRect.right, (int32) (viewLeft + viewWidth * (projectionRect.right + 1.0F)));
				int32 scissorBottom = Max(scissorRect.bottom, (int32) (viewBottom + viewHeight * (projectionRect.bottom + 1.0F)));
				int32 scissorTop = Min(scissorRect.top, (int32) (viewBottom + viewHeight * (projectionRect.top + 1.0F)));
				
				if ((scissorRight <= scissorLeft) || (scissorTop <= scissorBottom)) return (false);
				
				newGraphicsState |= kGraphicsLightScissor;
				lightRect.Set(scissorLeft, scissorTop, scissorRight, scissorBottom);
				Render::SetScissor(scissorLeft, scissorBottom, scissorRight - scissorLeft, scissorTop - scissorBottom);
				
				lightVertex[0].Set(projectionRect.left, projectionRect.bottom);
				lightVertex[1].Set(projectionRect.right, projectionRect.bottom);
				lightVertex[2].Set(projectionRect.right, projectionRect.top);
				lightVertex[3].Set(projectionRect.left, projectionRect.top);
			}
			else
			{
				lightRect = viewportRect;
				
				lightVertex[0].Set(-1.0F, -1.0F);
				lightVertex[1].Set(1.0F, -1.0F);
				lightVertex[2].Set(1.0F, 1.0F);
				lightVertex[3].Set(-1.0F, 1.0F);
			}
			
			float n = -currentNearDepth;
			float z1 = Fmin(center.z + r, n);
			float z2 = Fmin(center.z - r, n);
			
			float p33 = standardProjectionMatrix(2,2);
			float p34 = standardProjectionMatrix(2,3);
			float dmin = -0.5F * (p33 * z1 + p34) / z1 + 0.5F;
			float dmax = -0.5F * (p33 * z2 + p34) / z2 + 0.5F;
			
			lightDepthBounds.Set(dmin, dmax);
			
			if (newGraphicsState & kGraphicsDepthBoundsAvail)
			{
				Render::SetDepthBounds(dmin, dmax);
				newGraphicsState |= kGraphicsLightDepthBounds;
			}
		}
	}
	else
	{
		switch (lightType)
		{
			case kLightInfinite:
			{
				shaderType = kShaderInfiniteLight;
				break;
			}
			
			case kLightDepth:
			{
				shaderType = kShaderDepthLight;
				lightShadowData = shadowData;
				
				Render::TextureObject *shadowTexture = shadowFrameBuffer->GetRenderTargetTexture();
				shadowTexture->SetCompareMode(Render::kTextureCompareReference);
				Render::BindTexture(kTextureUnitLightProjection, shadowTexture);
				
				float w = (float) dynamicShadowMapSize;
				float h = (float) (dynamicShadowMapSize * kMaxShadowSectionCount);
				float dx = 1.5F / w;
				float dy = 1.5F / h;
				
				Render::SetFragmentProgramParameter4f(kFragmentParamShadowSample1, -0.125F * dx, -0.375F * dy, 0.375F * dx, -0.125F * dy);
				Render::SetFragmentProgramParameter4f(kFragmentParamShadowSample2, 0.125F * dx, 0.375F * dy, -0.375F * dx, 0.125F * dy);
				
				const Transform4D& shadowTransform = transformable->GetInverseWorldTransform();
				const Vector3D& cameraView = cameraTransformable->GetWorldTransform()[2];
				float f = InverseSqrt(cameraView.x * cameraView.x + cameraView.y * cameraView.y);
				Vector3D shadowView = shadowTransform[0] * (cameraView.x * f) + shadowTransform[1] * (cameraView.y * f);
				
				#if !C4PLAYSTATION3
				
					Render::SetFragmentProgramParameter4f(kFragmentParamShadowViewDirection, shadowView.x * -shadowData->inverseShadowSize.x, shadowView.y * -kInverseMaxShadowSectionCount * shadowData->inverseShadowSize.y, shadowView.z * -shadowData->inverseShadowSize.z, 0.0F);
				
				#else
				
					Render::SetFragmentProgramParameter4f(kFragmentParamShadowViewDirection, shadowView.x * -shadowData->inverseShadowSize.x, shadowView.y * kInverseMaxShadowSectionCount * shadowData->inverseShadowSize.y, shadowView.z * -shadowData->inverseShadowSize.z, 0.0F);
				
				#endif
				
				break;
			}
			
			case kLightLandscape:
			{
				shaderType = kShaderLandscapeLight;
				lightShadowData = shadowData;
				
				Render::TextureObject *shadowTexture = shadowFrameBuffer->GetRenderTargetTexture();
				shadowTexture->SetCompareMode(Render::kTextureCompareReference);
				Render::BindTexture(kTextureUnitLightProjection, shadowTexture);
				
				float w = (float) dynamicShadowMapSize;
				float h = (float) (dynamicShadowMapSize * kMaxShadowSectionCount);
				float dx = 1.5F / w;
				float dy = 1.5F / h;
				
				Render::SetFragmentProgramParameter4f(kFragmentParamShadowSample1, -0.125F * dx, -0.375F * dy, 0.375F * dx, -0.125F * dy);
				Render::SetFragmentProgramParameter4f(kFragmentParamShadowSample2, 0.125F * dx, 0.375F * dy, -0.375F * dx, 0.125F * dy);
				
				const Transform4D& shadowTransform = transformable->GetInverseWorldTransform();
				const Vector3D& cameraView = cameraTransformable->GetWorldTransform()[2];
				float f = InverseSqrt(cameraView.x * cameraView.x + cameraView.y * cameraView.y);
				Vector3D shadowView = shadowTransform[0] * (cameraView.x * f) + shadowTransform[1] * (cameraView.y * f);
				
				#if !C4PLAYSTATION3
				
					Render::SetFragmentProgramParameter4f(kFragmentParamShadowViewDirection, shadowView.x * -shadowData->inverseShadowSize.x, shadowView.y * -kInverseMaxShadowSectionCount * shadowData->inverseShadowSize.y, shadowView.z * -shadowData->inverseShadowSize.z, 0.0F);
				
				#else
				
					Render::SetFragmentProgramParameter4f(kFragmentParamShadowViewDirection, shadowView.x * -shadowData->inverseShadowSize.x, shadowView.y * kInverseMaxShadowSectionCount * shadowData->inverseShadowSize.y, shadowView.z * -shadowData->inverseShadowSize.z, 0.0F);
				
				#endif
				
				Vector3D scale1(shadowData[1].inverseShadowSize.x * shadowData[0].shadowSize.x, shadowData[1].inverseShadowSize.y * shadowData[0].shadowSize.y, shadowData[1].inverseShadowSize.z * shadowData[0].shadowSize.z);
				Vector3D scale2(shadowData[2].inverseShadowSize.x * shadowData[0].shadowSize.x, shadowData[2].inverseShadowSize.y * shadowData[0].shadowSize.y, shadowData[2].inverseShadowSize.z * shadowData[0].shadowSize.z);
				Vector3D scale3(shadowData[3].inverseShadowSize.x * shadowData[0].shadowSize.x, shadowData[3].inverseShadowSize.y * shadowData[0].shadowSize.y, shadowData[3].inverseShadowSize.z * shadowData[0].shadowSize.z);
				
				Render::SetFragmentProgramParameter4f(kFragmentParamShadowMapScale1, scale1.x, scale1.y, scale1.z, 0.0F);
				Render::SetFragmentProgramParameter4f(kFragmentParamShadowMapScale2, scale2.x, scale2.y, scale2.z, 0.0F);
				Render::SetFragmentProgramParameter4f(kFragmentParamShadowMapScale3, scale3.x, scale3.y, scale3.z, 0.0F);
				
				#if !C4PLAYSTATION3
				
					Render::SetFragmentProgramParameter4f(kFragmentParamShadowMapOffset1,
							shadowData[1].shadowPosition.x * shadowData[1].inverseShadowSize.x + 0.5F - scale1.x * (shadowData[0].shadowPosition.x * shadowData[0].inverseShadowSize.x + 0.5F),
							(shadowData[1].shadowPosition.y * shadowData[1].inverseShadowSize.y + 1.5F - scale1.y * (shadowData[0].shadowPosition.y * shadowData[0].inverseShadowSize.y + 0.5F)) * kInverseMaxShadowSectionCount,
							shadowData[1].shadowPosition.z * shadowData[1].inverseShadowSize.z - scale1.z * (shadowData[0].shadowPosition.z * shadowData[0].inverseShadowSize.z), 0.0F);
					Render::SetFragmentProgramParameter4f(kFragmentParamShadowMapOffset2,
							shadowData[2].shadowPosition.x * shadowData[2].inverseShadowSize.x + 0.5F - scale2.x * (shadowData[0].shadowPosition.x * shadowData[0].inverseShadowSize.x + 0.5F),
							(shadowData[2].shadowPosition.y * shadowData[2].inverseShadowSize.y + 2.5F - scale2.y * (shadowData[0].shadowPosition.y * shadowData[0].inverseShadowSize.y + 0.5F)) * kInverseMaxShadowSectionCount,
							shadowData[2].shadowPosition.z * shadowData[2].inverseShadowSize.z - scale2.z * (shadowData[0].shadowPosition.z * shadowData[0].inverseShadowSize.z), 0.0F);
					Render::SetFragmentProgramParameter4f(kFragmentParamShadowMapOffset3,
							shadowData[3].shadowPosition.x * shadowData[3].inverseShadowSize.x + 0.5F - scale3.x * (shadowData[0].shadowPosition.x * shadowData[0].inverseShadowSize.x + 0.5F),
							(shadowData[3].shadowPosition.y * shadowData[3].inverseShadowSize.y + 3.5F - scale3.y * (shadowData[0].shadowPosition.y * shadowData[0].inverseShadowSize.y + 0.5F)) * kInverseMaxShadowSectionCount,
							shadowData[3].shadowPosition.z * shadowData[3].inverseShadowSize.z - scale3.z * (shadowData[0].shadowPosition.z * shadowData[0].inverseShadowSize.z), 0.0F);
				
				#else
				
					Render::SetFragmentProgramParameter4f(kFragmentParamShadowMapOffset1,
							shadowData[1].shadowPosition.x * shadowData[1].inverseShadowSize.x + 0.5F - scale1.x * (shadowData[0].shadowPosition.x * shadowData[0].inverseShadowSize.x + 0.5F),
							(shadowData[1].shadowPosition.y * -shadowData[1].inverseShadowSize.y + 2.5F - scale1.y * (shadowData[0].shadowPosition.y * -shadowData[0].inverseShadowSize.y + 3.5F)) * kInverseMaxShadowSectionCount,
							shadowData[1].shadowPosition.z * shadowData[1].inverseShadowSize.z - scale1.z * (shadowData[0].shadowPosition.z * shadowData[0].inverseShadowSize.z), 0.0F);
					Render::SetFragmentProgramParameter4f(kFragmentParamShadowMapOffset2,
							shadowData[2].shadowPosition.x * shadowData[2].inverseShadowSize.x + 0.5F - scale2.x * (shadowData[0].shadowPosition.x * shadowData[0].inverseShadowSize.x + 0.5F),
							(shadowData[2].shadowPosition.y * -shadowData[2].inverseShadowSize.y + 1.5F - scale2.y * (shadowData[0].shadowPosition.y * -shadowData[0].inverseShadowSize.y + 3.5F)) * kInverseMaxShadowSectionCount,
							shadowData[2].shadowPosition.z * shadowData[2].inverseShadowSize.z - scale2.z * (shadowData[0].shadowPosition.z * shadowData[0].inverseShadowSize.z), 0.0F);
					Render::SetFragmentProgramParameter4f(kFragmentParamShadowMapOffset3,
							shadowData[3].shadowPosition.x * shadowData[3].inverseShadowSize.x + 0.5F - scale3.x * (shadowData[0].shadowPosition.x * shadowData[0].inverseShadowSize.x + 0.5F),
							(shadowData[3].shadowPosition.y * -shadowData[3].inverseShadowSize.y + 0.5F - scale3.y * (shadowData[0].shadowPosition.y * -shadowData[0].inverseShadowSize.y + 3.5F)) * kInverseMaxShadowSectionCount,
							shadowData[3].shadowPosition.z * shadowData[3].inverseShadowSize.z - scale3.z * (shadowData[0].shadowPosition.z * shadowData[0].inverseShadowSize.z), 0.0F);
				
				#endif
				
				break;
			}
		}
		
		lightRect = viewportRect;
		lightDepthBounds.Set(0.0F, 1.0F);
	}
	
	currentShaderType = shaderType;
	currentGraphicsState = newGraphicsState;
	
	unsigned_int32 changed = (newGraphicsState ^ oldGraphicsState) & (kGraphicsRenderLight | kGraphicsScissorMask | kGraphicsDepthBoundsMask);
	if (changed != 0)
	{
		if (changed & kGraphicsRenderLight) Render::EnableStencilTest();
		
		if (changed & kGraphicsDepthBoundsMask)
		{
			if (newGraphicsState & kGraphicsDepthBoundsMask) Render::EnableDepthBoundsTest();
			else Render::DisableDepthBoundsTest();
		}
		
		if (changed & kGraphicsScissorMask)
		{
			if (!(newGraphicsState & kGraphicsScissorMask))
			{
				int32 left = scissorRect.left;
				int32 bottom = scissorRect.bottom;
				Render::SetScissor(left, bottom, scissorRect.right - left, scissorRect.top - bottom);
			}
		}
	}
	
	return (true);
}

const Vector4D& GraphicsMgr::SetGeometryTransformable(const Transformable *transformable)
{
	geometryTransformable = transformable;
	
	if (lightObject->GetBaseLightType() == kLightInfinite)
	{
		const Vector3D& direction = lightTransformable->GetWorldTransform()[2];
		geometryLightPosition = (transformable) ? transformable->GetInverseWorldTransform() * direction : direction;
	}
	else
	{
		const Point3D& position = lightTransformable->GetWorldPosition();
		geometryLightPosition = (transformable) ? transformable->GetInverseWorldTransform() * position : position;
	}
	
	Render::SetVertexProgramParameter4fv(kVertexParamLightPosition, &geometryLightPosition.x);
	return (geometryLightPosition);
}

template <int32 index> void GraphicsMgr::GroupRenderSublist(List<Renderable> *list, unsigned_int32 bit, List<Renderable> *final)
{
	List<Renderable>	oddList;
	
	machine_address evenAccum = 0;
	
	Renderable *renderable = list->First();
	while (renderable)
	{
		Renderable *next = renderable->Next();
		
		machine_address key = renderable->GetGroupKey(index);
		if (key & bit) oddList.Append(renderable);
		else evenAccum |= key;
		
		renderable = next;
	}
	
	if ((evenAccum & ~(bit - 1)) == 0)
	{
		if (list != final)
		{
			for (;;)
			{
				renderable = list->First();
				if (!renderable) break;
				final->Append(renderable);
			}
		}
	}
	else
	{
		GroupRenderSublist<index>(list, bit << 1, final);
	}
	
	if (oddList.First()) GroupRenderSublist<index>(&oddList, bit << 1, final);
}

void GraphicsMgr::GroupAmbientRenderList(List<Renderable> *renderList)
{
	GroupRenderSublist<kGroupKeyAmbient>(renderList, 0x10, renderList);
}

void GraphicsMgr::GroupLightRenderList(List<Renderable> *renderList)
{
	GroupRenderSublist<kGroupKeyLight>(renderList, 0x10, renderList);
}

void GraphicsMgr::SortRenderSublist(List<Renderable> *list, float zmin, float zmax, List<Renderable> *final)
{
	float avg = (zmin + zmax) * 0.5F;
	
	Renderable *renderable = list->Last();
	if ((list->First() == renderable) || (!(zmax - zmin > Fabs(avg) * 0.001F)))
	{
		while (renderable)
		{
			Renderable *prev = renderable->Previous();
			final->Prepend(renderable);
			renderable = prev;
		}
	}
	else
	{
		List<Renderable>	farList;
		
		float zminFar = zmax;
		float zmaxNear = zmin;
		
		renderable = list->First();
		do
		{
			Renderable *next = renderable->Next();
			float z = renderable->GetTransparentDepth();
			if (z < avg)
			{
				zmaxNear = Fmax(zmaxNear, z);
			}
			else
			{
				zminFar = Fmin(zminFar, z);
				farList.Append(renderable);
			}
			
			renderable = next;
		} while (renderable);
		
		SortRenderSublist(list, zmin, zmaxNear, final);
		SortRenderSublist(&farList, zminFar, zmax, final);
	}
}

void GraphicsMgr::SortRenderList(List<Renderable> *renderList)
{
	Renderable *renderable = renderList->First();
	if (renderable)
	{
		List<Renderable>	transparentList;
		List<Renderable>	attachedList;
		
		const Vector3D& direction = cameraTransformable->GetWorldTransform()[2];
		
		float zmin = K::infinity;
		float zmax = K::minus_infinity;
		
		do
		{
			Renderable *next = renderable->Next();
			
			if (renderable->GetTransparentAttachment())
			{
				attachedList.Append(renderable);
			}
			else
			{
				const Point3D *position = renderable->GetTransparentPosition();
				if (position)
				{
					float z = direction * *position;
					zmin = Fmin(zmin, z);
					zmax = Fmax(zmax, z);
					
					renderable->SetTransparentDepth(z);
					transparentList.Append(renderable);
				}
			}
			
			renderable = next;
		} while (renderable);
		
		if (!transparentList.Empty()) SortRenderSublist(&transparentList, zmin, zmax, renderList);
		
		renderable = attachedList.First();
		while (renderable)
		{
			Renderable *next = renderable->Next();
			
			Renderable *attachment = renderable->GetTransparentAttachment();
			if (attachment->GetOwningList() == renderList) renderList->InsertBefore(renderable, attachment);
			else renderList->Append(renderable);
			
			renderable = next;
		}
	}
}

void GraphicsMgr::SetModelviewMatrix(const Transform4D& matrix)
{
	Matrix4D& mvp = currentMVPMatrix;
	mvp = currentProjectionMatrix * matrix;
	
	Render::SetVertexProgramParameter4f(kVertexParamMatrixMVP, mvp(0,0), mvp(0,1), mvp(0,2), mvp(0,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixMVP + 1, mvp(1,0), mvp(1,1), mvp(1,2), mvp(1,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixMVP + 2, mvp(2,0), mvp(2,1), mvp(2,2), mvp(2,3));
	Render::SetVertexProgramParameter4f(kVertexParamMatrixMVP + 3, mvp(3,0), mvp(3,1), mvp(3,2), mvp(3,3));
}

void GraphicsMgr::SetLocalVertexProgram(Link<VertexProgram> *programLink, const VertexSnippet *snippet)
{
	VertexProgram *vertexProgram = *programLink;
	if (!vertexProgram)
	{
		VertexAssembly assembly(ShaderAttribute::signatureStorage);
		assembly.AddSnippet(snippet);
		
		vertexProgram = VertexProgram::New(&assembly);
		*programLink = vertexProgram;
	}
	
	Render::SetVertexProgram(vertexProgram);
}

void GraphicsMgr::SetLocalVertexProgram(Link<VertexProgram> *programLink, int32 snippetCount, const VertexSnippet *const *snippet)
{
	VertexProgram *vertexProgram = *programLink;
	if (!vertexProgram)
	{
		VertexAssembly assembly(ShaderAttribute::signatureStorage);
		for (machine a = 0; a < snippetCount; a++) assembly.AddSnippet(snippet[a]);
		
		vertexProgram = VertexProgram::New(&assembly);
		*programLink = vertexProgram;
	}
	
	Render::SetVertexProgram(vertexProgram);
}

void GraphicsMgr::SetLocalFragmentProgram(int32 programIndex)
{
	static Link<FragmentProgram>	localProgram[kLocalProgramCount];
	
	FragmentProgram *fragmentProgram = localProgram[programIndex];
	if (!fragmentProgram)
	{
		static const char *const programSource[kLocalProgramCount] =
		{
			FragmentProgram::copyLightColor, FragmentProgram::copyVertexColor
		};
		
		fragmentProgram = new FragmentProgram(programSource[programIndex]);
		localProgram[programIndex] = fragmentProgram;
	}
	
	Render::SetFragmentProgram(fragmentProgram);
}

void GraphicsMgr::SetBlendState(unsigned_int32 newBlendState)
{
	unsigned_int32 oldBlendState = currentBlendState;
	if (newBlendState != oldBlendState)
	{
		static const unsigned_int32 blendFactor[kBlendFactorCount] =
		{
			Render::kBlendZero, Render::kBlendOne,
			Render::kBlendSrcColor, Render::kBlendDstColor, Render::kBlendConstColor,
			Render::kBlendSrcAlpha, Render::kBlendDstAlpha, Render::kBlendConstAlpha,
			Render::kBlendInvSrcColor, Render::kBlendInvDstColor, Render::kBlendInvConstColor,
			Render::kBlendInvSrcAlpha, Render::kBlendInvDstAlpha, Render::kBlendInvConstAlpha
		};
		
		currentBlendState = newBlendState;
		
		unsigned_int32 sourceRGB = blendFactor[GetBlendSource(newBlendState)];
		unsigned_int32 destRGB = blendFactor[GetBlendDest(newBlendState)];
		unsigned_int32 sourceAlpha = blendFactor[GetBlendSourceAlpha(newBlendState)];
		unsigned_int32 destAlpha = blendFactor[GetBlendDestAlpha(newBlendState)];
		
		Render::SetBlendFunc(sourceRGB, destRGB, sourceAlpha, destAlpha);
	}
}

void GraphicsMgr::SetRenderState(unsigned_int32 newRenderState)
{
	unsigned_int32 oldRenderState = currentRenderState;
	unsigned_int32 changed = newRenderState ^ oldRenderState;
	if (changed != 0)
	{
		currentRenderState = newRenderState;
		
		if (changed & kRenderDepthTest)
		{
			if (newRenderState & kRenderDepthTest) Render::EnableDepthTest();
			else Render::DisableDepthTest();
		}
		
		if (changed & kRenderColorInhibit)
		{
			if (newRenderState & kRenderColorInhibit) Render::SetColorMask(false, false, false, false);
			else Render::SetColorMask(true, true, true, true);
		}
		
		if (changed & kRenderDepthInhibit)
		{
			if (newRenderState & kRenderDepthInhibit) Render::SetDepthMask(false);
			else Render::SetDepthMask(true);
		}
		
		if ((changed & kRenderDepthOffset) && (!(newRenderState & kRenderDepthOffset)))
		{
			currentProjectionMatrix = cameraProjectionMatrix;
		}
		
		if (changed & kRenderAlphaTest)
		{
			if (newRenderState & kRenderAlphaTest) Render::EnableAlphaTest();
			else Render::DisableAlphaTest();
		}
		
		if (changed & kRenderLineSmooth)
		{
			if (newRenderState & kRenderLineSmooth) Render::EnableLineSmooth();
			else Render::DisableLineSmooth();
		}
		
		if (changed & kRenderWireframe)
		{
			if (newRenderState & kRenderWireframe) Render::SetPolygonMode(Render::kPolygonModeLine);
			else Render::SetPolygonMode(Render::kPolygonModeFill);
		}
		
		if (changed & kRenderStencilTest)
		{
			if (newRenderState & kRenderStencilTest) Render::SetStencilFunc(Render::kStencilEqual, 0, ~0);
			else Render::SetStencilFunc(Render::kStencilAlways, 0, ~0);
		}
	}
}

void GraphicsMgr::SetMaterialState(const RenderSegment *segment, unsigned_int32 newMaterialState)
{
	newMaterialState &= kMaterialShaderStateMask;
	unsigned_int32 oldMaterialState = currentMaterialState;
	unsigned_int32 changed = newMaterialState ^ oldMaterialState;
	if (changed != 0)
	{
		currentMaterialState = newMaterialState;
		
		if (changed & kMaterialTwoSided)
		{
			if (newMaterialState & kMaterialTwoSided) Render::DisableCullFace();
			else Render::EnableCullFace();
		}
		
		if (changed & kMaterialAlphaCoverage)
		{
			if (newMaterialState & kMaterialAlphaCoverage) Render::EnableAlphaCoverage();
			else Render::DisableAlphaCoverage();
		}
		
		if (changed & kMaterialSampleShading)
		{
			if (newMaterialState & kMaterialSampleShading) Render::EnableSampleShading();
			else Render::DisableSampleShading();
		}
	}
	
	if (newMaterialState & (kMaterialEmissionGlow | kMaterialSpecularBloom)) currentGraphicsState |= kGraphicsGlowBloomAvail;
}

void GraphicsMgr::ResetArrayState(int32 texcoordPreserveCount)
{
	unsigned_int32 arrayState = currentArrayState;
	
	if (arrayState & (1 << kShaderArrayPosition1)) Render::DisableAttribArray(1);
	if (arrayState & (1 << kShaderArrayPrevious)) Render::DisableAttribArray(7);
	if (arrayState & (1 << kShaderArrayNormal)) Render::DisableNormalArray();
	if (arrayState & (1 << kShaderArrayTangent)) Render::DisableAttribArray(6);
	
	if (arrayState & (1 << kShaderArrayColor0)) Render::DisableColorArray(0);
	if (arrayState & (1 << kShaderArrayColor1)) Render::DisableColorArray(1);
	if (arrayState & (1 << kShaderArrayColor2)) Render::DisableColorArray(2);
	
	arrayState &= ~((1 << kShaderArrayPosition1) | (1 << kShaderArrayPrevious) | (1 << kShaderArrayNormal) | (1 << kShaderArrayTangent) | (1 << kShaderArrayColor0) | (1 << kShaderArrayColor1) | (1 << kShaderArrayColor2));
	
	for (machine a = kShaderArrayTexture0 + texcoordPreserveCount; a < kMaxShaderArrayCount; a++)
	{
		unsigned_int32 bit = 1 << a;
		if (arrayState & bit)
		{
			arrayState &= ~bit;
			Render::DisableTexcoordArray(a - kShaderArrayTexture0);
		}
	}
	
	currentArrayState = arrayState;
}

void GraphicsMgr::SetVertexArray(const ShaderData *shaderData)
{
	const VertexBuffer *vertexBuffer = shaderData->vertexBuffer[kShaderArrayPosition0];
	if (vertexBuffer)
	{
		const float *array = ArrayOffsetToPtr(*shaderData->shaderOffset[kShaderArrayPosition0]);
		
		if (vertexBuffer != currentVertexBuffer[kShaderArrayPosition0]) currentVertexBuffer[kShaderArrayPosition0] = vertexBuffer;
		else if (array == shaderArrayPointer[kShaderArrayPosition0]) return;
		
		Render::SetAttributeVertexBuffer(vertexBuffer);
		
		shaderArrayPointer[kShaderArrayPosition0] = array;
		Render::SetVertexArray(shaderData->componentCount[kShaderArrayPosition0], Render::kVertexFloat, vertexBuffer->GetVertexStride(), array);
	}
	else
	{
		const float *array = *shaderData->shaderArray[kShaderArrayPosition0];
		
		if (currentVertexBuffer[kShaderArrayPosition0]) currentVertexBuffer[kShaderArrayPosition0] = nullptr;
		else if (array == shaderArrayPointer[kShaderArrayPosition0]) return;
		
		Render::ResetAttributeVertexBuffer();
		
		shaderArrayPointer[kShaderArrayPosition0] = array;
		int32 size = shaderData->componentCount[kShaderArrayPosition0];
		Render::SetVertexArray(size, Render::kVertexFloat, size * 4, array);
	}
}

void GraphicsMgr::SetPosition1Array(const ShaderData *shaderData)
{
	unsigned_int32 arrayState = currentArrayState;
	
	const VertexBuffer *vertexBuffer = shaderData->vertexBuffer[kShaderArrayPosition1];
	if (vertexBuffer)
	{
		if (!(arrayState & (1 << kShaderArrayPosition1)))
		{
			currentArrayState = arrayState | (1 << kShaderArrayPosition1);
			Render::EnableAttribArray(1);
		}
		
		const float *array = ArrayOffsetToPtr(*shaderData->shaderOffset[kShaderArrayPosition1]);
		
		if (vertexBuffer != currentVertexBuffer[kShaderArrayPosition1]) currentVertexBuffer[kShaderArrayPosition1] = vertexBuffer;
		else if (array == shaderArrayPointer[kShaderArrayPosition1]) return;
		
		Render::SetAttributeVertexBuffer(vertexBuffer);
		
		shaderArrayPointer[kShaderArrayPosition1] = array;
		Render::SetAttribArray(1, shaderData->componentCount[kShaderArrayPosition1], Render::kVertexFloat, vertexBuffer->GetVertexStride(), array);
	}
	else
	{
		const float *const *arrayPtr = shaderData->shaderArray[kShaderArrayPosition1];
		if (arrayPtr)
		{
			if (!(arrayState & (1 << kShaderArrayPosition1)))
			{
				currentArrayState = arrayState | (1 << kShaderArrayPosition1);
				Render::EnableAttribArray(1);
			}
			
			const float *array = *arrayPtr;
			
			if (currentVertexBuffer[kShaderArrayPosition1]) currentVertexBuffer[kShaderArrayPosition1] = nullptr;
			else if (array == shaderArrayPointer[kShaderArrayPosition1]) return;
			
			Render::ResetAttributeVertexBuffer();
			
			shaderArrayPointer[kShaderArrayPosition1] = array;
			int32 size = shaderData->componentCount[kShaderArrayPosition1];
			Render::SetAttribArray(1, size, Render::kVertexFloat, size * 4, array);
		}
		else
		{
			if (arrayState & (1 << kShaderArrayPosition1))
			{
				currentArrayState = arrayState & ~(1 << kShaderArrayPosition1);
				Render::DisableAttribArray(1);
			}
		}
	}
}

void GraphicsMgr::SetPreviousArray(const ShaderData *shaderData)
{
	unsigned_int32 arrayState = currentArrayState;
	
	const VertexBuffer *vertexBuffer = shaderData->vertexBuffer[kShaderArrayPrevious];
	if (vertexBuffer)
	{
		if (!(arrayState & (1 << kShaderArrayPrevious)))
		{
			currentArrayState = arrayState | (1 << kShaderArrayPrevious);
			Render::EnableAttribArray(7);
		}
		
		const float *array = ArrayOffsetToPtr(*shaderData->shaderOffset[kShaderArrayPrevious]);
		
		if (vertexBuffer != currentVertexBuffer[kShaderArrayPrevious]) currentVertexBuffer[kShaderArrayPrevious] = vertexBuffer;
		else if (array == shaderArrayPointer[kShaderArrayPrevious]) return;
		
		Render::SetAttributeVertexBuffer(vertexBuffer);
		
		shaderArrayPointer[kShaderArrayPrevious] = array;
		Render::SetAttribArray(7, shaderData->componentCount[kShaderArrayPrevious], Render::kVertexFloat, vertexBuffer->GetVertexStride(), array);
	}
	else
	{
		const float *const *arrayPtr = shaderData->shaderArray[kShaderArrayPrevious];
		if (arrayPtr)
		{
			if (!(arrayState & (1 << kShaderArrayPrevious)))
			{
				currentArrayState = arrayState | (1 << kShaderArrayPrevious);
				Render::EnableAttribArray(7);
			}
			
			const float *array = *arrayPtr;
			
			if (currentVertexBuffer[kShaderArrayPrevious]) currentVertexBuffer[kShaderArrayPrevious] = nullptr;
			else if (array == shaderArrayPointer[kShaderArrayPrevious]) return;
			
			Render::ResetAttributeVertexBuffer();
			
			shaderArrayPointer[kShaderArrayPrevious] = array;
			int32 size = shaderData->componentCount[kShaderArrayPrevious];
			Render::SetAttribArray(7, size, Render::kVertexFloat, size * 4, array);
		}
		else
		{
			if (arrayState & (1 << kShaderArrayPrevious))
			{
				currentArrayState = arrayState & ~(1 << kShaderArrayPrevious);
				Render::DisableAttribArray(7);
			}
		}
	}
}

void GraphicsMgr::SetNormalArray(const ShaderData *shaderData)
{
	unsigned_int32 arrayState = currentArrayState;
	
	const VertexBuffer *vertexBuffer = shaderData->vertexBuffer[kShaderArrayNormal];
	if (vertexBuffer)
	{
		if (!(arrayState & (1 << kShaderArrayNormal)))
		{
			currentArrayState = arrayState | (1 << kShaderArrayNormal);
			Render::EnableNormalArray();
		}
		
		const float *array = ArrayOffsetToPtr(*shaderData->shaderOffset[kShaderArrayNormal]);
		
		if (vertexBuffer != currentVertexBuffer[kShaderArrayNormal]) currentVertexBuffer[kShaderArrayNormal] = vertexBuffer;
		else if (array == shaderArrayPointer[kShaderArrayNormal]) return;
		
		Render::SetAttributeVertexBuffer(vertexBuffer);
		
		shaderArrayPointer[kShaderArrayNormal] = array;
		Render::SetNormalArray(shaderData->componentCount[kShaderArrayNormal], Render::kVertexFloat, vertexBuffer->GetVertexStride(), array);
	}
	else
	{
		const float *const *arrayPtr = shaderData->shaderArray[kShaderArrayNormal];
		if (arrayPtr)
		{
			if (!(arrayState & (1 << kShaderArrayNormal)))
			{
				currentArrayState = arrayState | (1 << kShaderArrayNormal);
				Render::EnableNormalArray();
			}
			
			const float *array = *arrayPtr;
			
			if (currentVertexBuffer[kShaderArrayNormal]) currentVertexBuffer[kShaderArrayNormal] = nullptr;
			else if (array == shaderArrayPointer[kShaderArrayNormal]) return;
			
			Render::ResetAttributeVertexBuffer();
			
			shaderArrayPointer[kShaderArrayNormal] = array;
			int32 size = shaderData->componentCount[kShaderArrayNormal];
			Render::SetNormalArray(size, Render::kVertexFloat, size * 4, array);
		}
		else
		{
			if (arrayState & (1 << kShaderArrayNormal))
			{
				currentArrayState = arrayState & ~(1 << kShaderArrayNormal);
				Render::DisableNormalArray();
			}
		}
	}
}

void GraphicsMgr::SetTangentArray(const ShaderData *shaderData)
{
	unsigned_int32 arrayState = currentArrayState;
	
	const VertexBuffer *vertexBuffer = shaderData->vertexBuffer[kShaderArrayTangent];
	if (vertexBuffer)
	{
		if (!(arrayState & (1 << kShaderArrayTangent)))
		{
			currentArrayState = arrayState | (1 << kShaderArrayTangent);
			Render::EnableAttribArray(6);
		}
		
		const float *array = ArrayOffsetToPtr(*shaderData->shaderOffset[kShaderArrayTangent]);
		
		if (vertexBuffer != currentVertexBuffer[kShaderArrayTangent]) currentVertexBuffer[kShaderArrayTangent] = vertexBuffer;
		else if (array == shaderArrayPointer[kShaderArrayTangent]) return;
		
		Render::SetAttributeVertexBuffer(vertexBuffer);
		
		shaderArrayPointer[kShaderArrayTangent] = array;
		Render::SetAttribArray(6, shaderData->componentCount[kShaderArrayTangent], Render::kVertexFloat, vertexBuffer->GetVertexStride(), array);
	}
	else
	{
		const float *const *arrayPtr = shaderData->shaderArray[kShaderArrayTangent];
		if (arrayPtr)
		{
			if (!(arrayState & (1 << kShaderArrayTangent)))
			{
				currentArrayState = arrayState | (1 << kShaderArrayTangent);
				Render::EnableAttribArray(6);
			}
			
			const float *array = *arrayPtr;
			
			if (currentVertexBuffer[kShaderArrayTangent]) currentVertexBuffer[kShaderArrayTangent] = nullptr;
			else if (array == shaderArrayPointer[kShaderArrayTangent]) return;
			
			Render::ResetAttributeVertexBuffer();
			
			shaderArrayPointer[kShaderArrayTangent] = array;
			int32 size = shaderData->componentCount[kShaderArrayTangent];
			Render::SetAttribArray(6, size, Render::kVertexFloat, size * 4, array);
		}
		else
		{
			if (arrayState & (1 << kShaderArrayTangent))
			{
				currentArrayState = arrayState & ~(1 << kShaderArrayTangent);
				Render::DisableAttribArray(6);
			}
		}
	}
}

void GraphicsMgr::SetOffsetArray(const ShaderData *shaderData)
{
	unsigned_int32 arrayState = currentArrayState;
	
	const VertexBuffer *vertexBuffer = shaderData->vertexBuffer[kShaderArrayOffset];
	if (vertexBuffer)
	{
		if (!(arrayState & (1 << kShaderArrayOffset)))
		{
			currentArrayState = arrayState | (1 << kShaderArrayOffset);
			Render::EnableAttribArray(6);
		}
		
		const float *array = ArrayOffsetToPtr(*shaderData->shaderOffset[kShaderArrayOffset]);
		
		if (vertexBuffer != currentVertexBuffer[kShaderArrayOffset]) currentVertexBuffer[kShaderArrayOffset] = vertexBuffer;
		else if (array == shaderArrayPointer[kShaderArrayOffset]) return;
		
		Render::SetAttributeVertexBuffer(vertexBuffer);
		
		shaderArrayPointer[kShaderArrayOffset] = array;
		Render::SetAttribArray(6, shaderData->componentCount[kShaderArrayOffset], Render::kVertexFloat, vertexBuffer->GetVertexStride(), array);
	}
	else
	{
		const float *const *arrayPtr = shaderData->shaderArray[kShaderArrayOffset];
		if (arrayPtr)
		{
			if (!(arrayState & (1 << kShaderArrayOffset)))
			{
				currentArrayState = arrayState | (1 << kShaderArrayOffset);
				Render::EnableAttribArray(6);
			}
			
			const float *array = *arrayPtr;
			
			if (currentVertexBuffer[kShaderArrayOffset]) currentVertexBuffer[kShaderArrayOffset] = nullptr;
			else if (array == shaderArrayPointer[kShaderArrayOffset]) return;
			
			Render::ResetAttributeVertexBuffer();
			
			shaderArrayPointer[kShaderArrayOffset] = array;
			int32 size = shaderData->componentCount[kShaderArrayOffset];
			Render::SetAttribArray(6, size, Render::kVertexFloat, size * 4, array);
		}
		else
		{
			if (arrayState & (1 << kShaderArrayOffset))
			{
				currentArrayState = arrayState & ~(1 << kShaderArrayOffset);
				Render::DisableAttribArray(6);
			}
		}
	}
}

void GraphicsMgr::SetColorArray(const ShaderData *shaderData)
{
	unsigned_int32 arrayState = currentArrayState;
	
	const VertexBuffer *vertexBuffer = shaderData->vertexBuffer[kShaderArrayColor0];
	if (vertexBuffer)
	{
		if (!(arrayState & (1 << kShaderArrayColor0)))
		{
			currentArrayState = arrayState | (1 << kShaderArrayColor0);
			Render::EnableColorArray(0);
		}
		
		const float *array = ArrayOffsetToPtr(*shaderData->shaderOffset[kShaderArrayColor0]);
		
		if (vertexBuffer != currentVertexBuffer[kShaderArrayColor0]) currentVertexBuffer[kShaderArrayColor0] = vertexBuffer;
		else if (array == shaderArrayPointer[kShaderArrayColor0]) return;
		
		Render::SetAttributeVertexBuffer(vertexBuffer);
		
		shaderArrayPointer[kShaderArrayColor0] = array;
		
		int32 size = shaderData->componentCount[kShaderArrayColor0];
		if (size > 1) Render::SetColorArray(0, size, Render::kVertexFloat, vertexBuffer->GetVertexStride(), array);
		else Render::SetColorArray(0, 4, Render::kVertexUnsignedByte, vertexBuffer->GetVertexStride(), array);
	}
	else
	{
		const float *const *arrayPtr = shaderData->shaderArray[kShaderArrayColor0];
		if (arrayPtr)
		{
			if (!(arrayState & (1 << kShaderArrayColor0)))
			{
				currentArrayState = arrayState | (1 << kShaderArrayColor0);
				Render::EnableColorArray(0);
			}
			
			const float *array = *arrayPtr;
			
			if (currentVertexBuffer[kShaderArrayColor0]) currentVertexBuffer[kShaderArrayColor0] = nullptr;
			else if (array == shaderArrayPointer[kShaderArrayColor0]) return;
			
			Render::ResetAttributeVertexBuffer();
			
			shaderArrayPointer[kShaderArrayColor0] = array;
			int32 size = shaderData->componentCount[kShaderArrayColor0];
			if (size > 1) Render::SetColorArray(0, size, Render::kVertexFloat, size * 4, array);
			else Render::SetColorArray(0, 4, Render::kVertexUnsignedByte, 4, array);
		}
		else
		{
			if (arrayState & (1 << kShaderArrayColor0))
			{
				currentArrayState = arrayState & ~(1 << kShaderArrayColor0);
				Render::DisableColorArray(0);
			}
		}
	}
}

void GraphicsMgr::SetAuxColorArray(const ShaderData *shaderData, int32 index)
{
	unsigned_int32 color = index - kShaderArrayColor0;
	unsigned_int32 arrayBit = 1 << index;
	unsigned_int32 arrayState = currentArrayState;
	
	const VertexBuffer *vertexBuffer = shaderData->vertexBuffer[index];
	if (vertexBuffer)
	{
		if (!(arrayState & arrayBit))
		{
			currentArrayState = arrayState | arrayBit;
			Render::EnableColorArray(color);
		}
		
		const float *array = ArrayOffsetToPtr(*shaderData->shaderOffset[index]);
		
		if (vertexBuffer != currentVertexBuffer[index]) currentVertexBuffer[index] = vertexBuffer;
		else if (array == shaderArrayPointer[index]) return;
		
		Render::SetAttributeVertexBuffer(vertexBuffer);
		
		shaderArrayPointer[index] = array;
		Render::SetColorArray(color, 4, Render::kVertexUnsignedByte, vertexBuffer->GetVertexStride(), array);
	}
	else
	{
		const float *const *arrayPtr = shaderData->shaderArray[index];
		if (arrayPtr)
		{
			if (!(arrayState & arrayBit))
			{
				currentArrayState = arrayState | arrayBit;
				Render::EnableColorArray(color);
			}
			
			const float *array = *arrayPtr;
			
			if (currentVertexBuffer[index]) currentVertexBuffer[index] = nullptr;
			else if (array == shaderArrayPointer[index]) return;
			
			Render::ResetAttributeVertexBuffer();
			
			shaderArrayPointer[index] = array;
			Render::SetColorArray(color, 4, Render::kVertexUnsignedByte, 4, array);
		}
		else
		{
			if (arrayState & arrayBit)
			{
				currentArrayState = arrayState & ~arrayBit;
				Render::DisableColorArray(color);
			}
		}
	}
}

void GraphicsMgr::SetTexcoordArray(const ShaderData *shaderData, int32 index)
{
	unsigned_int32 coord = index - kShaderArrayTexture0;
	unsigned_int32 arrayBit = 1 << index;
	unsigned_int32 arrayState = currentArrayState;
	
	const VertexBuffer *vertexBuffer = shaderData->vertexBuffer[index];
	if (vertexBuffer)
	{
		if (!(arrayState & arrayBit))
		{
			currentArrayState = arrayState | arrayBit;
			Render::EnableTexcoordArray(coord);
		}
		
		const float *array = ArrayOffsetToPtr(*shaderData->shaderOffset[index]);
		
		if (vertexBuffer != currentVertexBuffer[index]) currentVertexBuffer[index] = vertexBuffer;
		else if (array == shaderArrayPointer[index]) return;
		
		Render::SetAttributeVertexBuffer(vertexBuffer);
		
		shaderArrayPointer[index] = array;
		Render::SetTexcoordArray(coord, shaderData->componentCount[index], Render::kVertexFloat, vertexBuffer->GetVertexStride(), array);
	}
	else
	{
		const float *const *arrayPtr = shaderData->shaderArray[index];
		if (arrayPtr)
		{
			if (!(arrayState & arrayBit))
			{
				currentArrayState = arrayState | arrayBit;
				Render::EnableTexcoordArray(coord);
			}
			
			const float *array = *arrayPtr;
			
			if (currentVertexBuffer[index]) currentVertexBuffer[index] = nullptr;
			else if (array == shaderArrayPointer[index]) return;
			
			Render::ResetAttributeVertexBuffer();
			
			shaderArrayPointer[index] = array;
			int32 size = shaderData->componentCount[index];
			Render::SetTexcoordArray(coord, size, Render::kVertexFloat, size * 4, array);
		}
		else
		{
			if (arrayState & arrayBit)
			{
				currentArrayState = arrayState & ~arrayBit;
				Render::DisableTexcoordArray(coord);
			}
		}
	}
}

void GraphicsMgr::DrawRenderList(const List<Renderable> *renderList)
{
	#if !C4SERVER
	
		Renderable *renderable = renderList->First();
		if (renderable)
		{
			unsigned_int32 graphicsState = currentGraphicsState;
			
			unsigned_int32 extraState = 0;
			if (lightObject)
			{
				extraState = kRenderDepthInhibit;
				unsigned_int32 lightFlags = lightObject->GetLightFlags();
				if ((!(lightFlags & kLightShadowInhibit)) && (graphicsState & kGraphicsStencilValid)) extraState |= kRenderStencilTest;
			
				if (graphicsState & kGraphicsDepthTestLess)
				{
					currentGraphicsState = graphicsState & ~kGraphicsDepthTestLess;
					Render::SetDepthFunc(Render::kDepthLessEqual);
				}
			}
			else
			{
				if (!(graphicsState & kGraphicsAmbientLessEqual))
				{
					if (!(graphicsState & kGraphicsDepthTestLess))
					{
						currentGraphicsState = graphicsState | kGraphicsDepthTestLess;
						Render::SetDepthFunc(Render::kDepthLess);
					}
				}
				else
				{
					if (graphicsState & kGraphicsDepthTestLess)
					{
						currentGraphicsState = graphicsState & ~kGraphicsDepthTestLess;
						Render::SetDepthFunc(Render::kDepthLessEqual);
					}
				}
			}
			
			do
			{
				unsigned_int32 newRenderState = renderable->GetRenderState() & ~disabledRenderState;
				SetRenderState(newRenderState | extraState);
				
				unsigned_int32 renderableFlags = renderable->GetRenderableFlags();
				const Transformable *transformable = renderable->GetTransformable();
				if (!(renderableFlags & kRenderableCameraTransformInhibit))
				{
					if (newRenderState & kRenderDepthOffset)
					{
						float z = Fmin(cameraSpaceTransform.GetRow(2) ^ renderable->GetDepthOffsetPoint(), -cameraObject->GetNearDepth());
						float delta = renderable->GetDepthOffsetDelta();
						float epsilon = depthOffsetConstant * delta / (z * (z + delta));
						epsilon = Fmax(Fabs(epsilon), 4.8e-7F) * NonzeroFsgn(epsilon);
						
						currentProjectionMatrix = cameraProjectionMatrix;
						currentProjectionMatrix(2,2) *= 1.0F + epsilon;
					}
					
					if (transformable)
						SetModelviewMatrix(cameraSpaceTransform * transformable->GetWorldTransform());
					else 
						SetModelviewMatrix(cameraSpaceTransform);
				}
				else
				{
					if (transformable)
						SetModelviewMatrix(transformable->GetWorldTransform());
					else 
						SetModelviewMatrix(K::identity_4D);
				}
				
				ShaderType shaderType = currentShaderType;
				if (shaderType == kShaderAmbient) shaderType = renderable->GetAmbientEnvironment()->ambientShaderType;
				ShaderVariant variant = (renderableFlags & (kRenderableFogInhibit | kRenderableUnfog)) ? kShaderVariantNormal : currentShaderVariant;
				
				RenderSegment *segment = renderable->GetFirstRenderSegment();
				do
				{
					const ShaderData *shaderData = segment->GetShaderData(shaderType, renderable->GetShaderDetailLevel());
					if ((!shaderData) || ((shaderData->variantMask & (1 << variant)) == 0))
					{
						shaderData = segment->InitShaderData(renderable, shaderType, variant);
					}
					
					if (!shaderData->programData[variant].vertexProgram) continue;
					
					SetBlendState(shaderData->blendState);
					SetMaterialState(segment, shaderData->materialState);
					
					Render::SetVertexProgram(shaderData->programData[variant].vertexProgram);
					Render::SetFragmentProgram(shaderData->programData[variant].fragmentProgram);
					
					SetVertexArray(shaderData);
					SetPosition1Array(shaderData);
					SetNormalArray(shaderData);
					SetTangentArray(shaderData);
					SetColorArray(shaderData);
					SetAuxColorArray(shaderData, kShaderArrayColor1);
					SetAuxColorArray(shaderData, kShaderArrayColor2);

					for (machine a = kShaderArrayTexture0; a < kMaxShaderArrayCount; a++)
						SetTexcoordArray(shaderData, a);
					
					int32 stateFuncCount = shaderData->shaderStateDataCount;
					for (machine a = 0; a < stateFuncCount; a++)
						(*shaderData->shaderStateData[a].stateFunc)(renderable, shaderData->shaderStateData[a].stateCookie);
					
					if (variant != kShaderVariantNormal)
						(*shaderData->fogStateFunc)(renderable, nullptr);
					
					int32 unitCount = shaderData->textureUnitCount;
					const Render::TextureObject *const *textureObject = shaderData->textureObject;
					for (machine unit = 0; unit < unitCount; unit++)
					{
						const Render::TextureObject *texture = textureObject[unit];
						if (texture) Render::BindTexture(unit, texture);
					}
					
					int32 vertexCount = renderable->GetVertexCount();
					graphicsCounter[kGraphicsCounterDirectVertices] += vertexCount;
					graphicsCounter[kGraphicsCounterDirectCommands]++;
					
					switch (renderable->GetRenderType())
					{
						case kRenderPoints:
							
							Render::EnablePointSprite();
							Render::DrawArrays(Render::kPrimitivePoints, 0, vertexCount);
							Render::DisablePointSprite();
							break;
						
						case kRenderLines:
							
							Render::DrawArrays(Render::kPrimitiveLines, 0, vertexCount);
							break;
						
						case kRenderLineStrip:
							
							Render::DrawArrays(Render::kPrimitiveLineStrip, 0, vertexCount);
							break;
						
						case kRenderLineLoop:
							
							Render::DrawArrays(Render::kPrimitiveLineLoop, 0, vertexCount);
							break;
						
						case kRenderIndexedLines:
						{
							const VertexBuffer *indexBuffer = shaderData->indexBuffer;
							if (indexBuffer)
							{
								Render::SetIndexVertexBuffer(indexBuffer);
								
								const unsigned_int16 *ptr = IndexArrayOffsetToPtr(renderable->GetFaceOffset()) + segment->GetFaceStart() * 2;
								Render::DrawElements(Render::kPrimitiveLines, vertexCount, segment->GetFaceCount() * 2, ptr);
							}
							else
							{
								Render::ResetIndexVertexBuffer();
								
								const unsigned_int16 *ptr = renderable->GetFaceArray() + segment->GetFaceStart() * 2;
								Render::DrawElements(Render::kPrimitiveLines, vertexCount, segment->GetFaceCount() * 2, ptr);
							}
							
							break;
						}
						
						case kRenderTriangles:
							
							graphicsCounter[kGraphicsCounterDirectPrimitives] += vertexCount / 3;
							Render::DrawArrays(Render::kPrimitiveTriangles, 0, vertexCount);
							break;
						
						case kRenderTriangleStrip:
							
							graphicsCounter[kGraphicsCounterDirectPrimitives] += vertexCount - 2;
							Render::DrawArrays(Render::kPrimitiveTriangleStrip, 0, vertexCount);
							break;
						
						case kRenderIndexedTriangles:
						{
							int32 triangleCount = segment->GetFaceCount();
							graphicsCounter[kGraphicsCounterDirectPrimitives] += triangleCount;
							
							const VertexBuffer *indexBuffer = shaderData->indexBuffer;
							if (indexBuffer)
							{
								Render::SetIndexVertexBuffer(indexBuffer);
								
								const unsigned_int16 *ptr = IndexArrayOffsetToPtr(renderable->GetFaceOffset()) + segment->GetFaceStart() * 3;
								Render::DrawElements(Render::kPrimitiveTriangles, vertexCount, triangleCount * 3, ptr);
							}
							else
							{
								Render::ResetIndexVertexBuffer();
								
								const unsigned_int16 *ptr = renderable->GetFaceArray() + segment->GetFaceStart() * 3;
								Render::DrawElements(Render::kPrimitiveTriangles, vertexCount, triangleCount * 3, ptr);
							}
							
							break;
						}
						
						case kRenderQuads:
							
							graphicsCounter[kGraphicsCounterDirectPrimitives] += vertexCount >> 2;
							Render::DrawArrays(Render::kPrimitiveQuads, 0, vertexCount);
							break;
						
						case kRenderMultiIndexedTriangles:
							
							Render::SetIndexVertexBuffer(shaderData->indexBuffer);
							Render::MultiDrawElements(Render::kPrimitiveTriangles, segment->GetMultiCountArray(), reinterpret_cast<const void *const *>(segment->GetMultiOffsetArray()), segment->GetMultiRenderCount());
							break;
						
						case kRenderMaskedMultiIndexedTriangles:
						{
							static const void		*index[33];
							static unsigned_int32	count[33];
							
							Render::SetIndexVertexBuffer(shaderData->indexBuffer);
							
							unsigned_int32 size = 0;
							const unsigned_int16 *ptr = IndexArrayOffsetToPtr(renderable->GetFaceOffset());
							
							int32 triangleCount = segment->GetFaceCount();
							if (triangleCount != 0)
							{
								graphicsCounter[kGraphicsCounterDirectPrimitives] += triangleCount;
								
								index[0] = ptr;
								count[0] = triangleCount * 3;
								size = 1;
							}
							
							unsigned_int32 mask = segment->GetMultiRenderMask();
							const int32 *data = segment->GetMultiRenderData();
							while (mask != 0)
							{
								if (mask & 1)
								{
									triangleCount = data[1];
									graphicsCounter[kGraphicsCounterDirectPrimitives] += triangleCount;
									
									index[size] = ptr + data[0] * 3;
									count[size] = triangleCount * 3;
									size++;
								}
								
								mask >>= 1;
								data += 2;
							}
							
							Render::MultiDrawElements(Render::kPrimitiveTriangles, count, index, size);
							break;
						}
					}
				} while ((segment = segment->GetNextRenderSegment()) != nullptr);
				
				if (currentOcclusionQuery)
				{
					Render::EndQuery(currentOcclusionQuery, Render::kQuerySamplesPassed);
					
					occlusionQueryList.Append(currentOcclusionQuery);
					currentOcclusionQuery = nullptr;
				}
				
				renderable = renderable->Next();
			} while (renderable);
			
			#if C4DIAGNOSTICS
			
				if ((diagnosticFlags & (kDiagnosticWireframe | kDiagnosticNormals | kDiagnosticTangents)) && (!lightObject))
				{
					if (diagnosticFlags & kDiagnosticWireframe) DrawWireframe((diagnosticFlags & kDiagnosticDepthTest) ? kWireframeDepthTest : 0, renderList);
					if (diagnosticFlags & kDiagnosticNormals) DrawVectors(kArrayNormal, renderList);
					if (diagnosticFlags & kDiagnosticTangents) DrawVectors(kArrayTangent, renderList);
				}
			
			#endif
		}
	
	#endif
}

bool GraphicsMgr::BeginStructureRendering(const Transform4D& previousCameraWorldTransform, unsigned_int32 structureFlags, float velocityScale)
{
	if (currentRenderTargetType != kRenderTargetPrimary)
		return (false);

	if ((renderOptionFlags & (kRenderOptionStructureEffects | kRenderOptionMotionBlur)) == 0)
		return (false);

	if ((GetRenderTargetMask() & (1 << kRenderTargetStructure)) == 0)
		return (false);
	
	if (!(renderOptionFlags & kRenderOptionMotionBlur))
		structureFlags &= ~kStructureRenderVelocity;
	
	previousCameraSpaceTransform(3,0) = previousCameraSpaceTransform(3,1) = previousCameraSpaceTransform(3,2) = 0.0F;
	previousCameraSpaceTransform(3,3) = 1.0F;
	
	const Vector3D& rightDirection = previousCameraWorldTransform[0];
	const Vector3D& downDirection = previousCameraWorldTransform[1];
	const Vector3D& viewDirection = previousCameraWorldTransform[2];
	
	previousCameraSpaceTransform(0,0) = rightDirection.x;
	previousCameraSpaceTransform(0,1) = rightDirection.y;
	previousCameraSpaceTransform(0,2) = rightDirection.z;
	previousCameraSpaceTransform(1,0) = -downDirection.x;
	previousCameraSpaceTransform(1,1) = -downDirection.y;
	previousCameraSpaceTransform(1,2) = -downDirection.z;
	previousCameraSpaceTransform(2,0) = -viewDirection.x;
	previousCameraSpaceTransform(2,1) = -viewDirection.y;
	previousCameraSpaceTransform(2,2) = -viewDirection.z;
	
	const Vector3D& previousCameraWorldOffset = previousCameraWorldTransform[3];
	previousCameraSpaceTransform(0,3) = -(previousCameraSpaceTransform.GetRow(0) ^ previousCameraWorldOffset);
	previousCameraSpaceTransform(1,3) = -(previousCameraSpaceTransform.GetRow(1) ^ previousCameraWorldOffset);
	previousCameraSpaceTransform(2,3) = -(previousCameraSpaceTransform.GetRow(2) ^ previousCameraWorldOffset);
	
	float red = 0.0F;
	float green = 0.0F;
	unsigned_int32 graphicsState = currentGraphicsState;
	
	if ((structureFlags & (kStructureZeroBackgroundVelocity | kStructureRenderVelocity)) == kStructureRenderVelocity)
	{
		graphicsState |= kGraphicsMotionBlurAvail;
		
		Vector4D v = currentProjectionMatrix * (cameraSpaceTransform * viewDirection);
		float w = (float) (viewportRect.right - viewportRect.left) * 0.5F;
		float h = (float) (viewportRect.top - viewportRect.bottom) * 0.5F;
		float f = kVelocityMultiplier / v.w;
		red = v.x * f * w;
		green = v.y * f * h;
		
		float m = 1.0F / Fmax(Fabs(red), Fabs(green), 1.0F);
		red = Fmin(FmaxZero(red * m), 1.0F);
		green = Fmin(FmaxZero(green * m), 1.0F);
	}
	
	normalFrameBuffer->GetRenderTargetTexture(kRenderTargetStructure)->UnbindAll();
	SetRenderTarget(kRenderTargetStructure);
	
	ResetArrayState();
	Render::DisableBlend();
	
	if (multisampleFrameBuffer)
	{
		if (structureFlags & kStructureClearBuffer)
		{
			#if !C4PLAYSTATION3
			
				SetRenderState(kRenderDepthTest);
				Render::SetClearColor(red, green, 65504.0F, 0.0F);
				Render::Clear(Render::kClearBufferColor | Render::kClearBufferDepth | Render::kClearBufferStencil);
			
			#else //[ PS3

			// -- PlayStation 3 code hidden --

			#endif //]
		}
		else
		{
			SetRenderState(kRenderDepthTest);
			Render::Clear(Render::kClearBufferDepth | Render::kClearBufferStencil);
		}
	}
	else
	{
		if (structureFlags & kStructureClearBuffer)
		{
			#if !C4PLAYSTATION3
			
				Render::SetClearColor(red, green, 65504.0F, 0.0F);
				Render::Clear(Render::kClearBufferColor);
			
			#else //[ PS3

			// -- PlayStation 3 code hidden --

			#endif //]
		}
		
		graphicsState |= kGraphicsAmbientLessEqual;
	}
	
	if (!(graphicsState & kGraphicsDepthTestLess))
	{
		graphicsState |= kGraphicsDepthTestLess;
		Render::SetDepthFunc(Render::kDepthLess);
	}
	
	currentGraphicsState = graphicsState;
	currentStructureFlags = structureFlags;
	
	float scale = velocityScale * kVelocityMultiplier;
	
	#if !C4PLAYSTATION3
	
		Render::SetFragmentProgramParameter4f(kFragmentParamVelocityScale, scale, scale, 0.0F, 0.0F);
	
	#else
	
		Render::SetFragmentProgramParameter4f(kFragmentParamVelocityScale, scale, -scale, 0.0F, 0.0F);
	
	#endif
	
	MemoryMgr::ClearMemory(motionGridFlag, kProcessGridWidth * kProcessGridHeight);
	return (true);
}

void GraphicsMgr::EndStructureRendering(void)
{
	unsigned_int32 arrayState = currentArrayState;
	if (arrayState & (1 << kShaderArrayPrevious))
	{
		currentArrayState = arrayState & ~(1 << kShaderArrayPrevious);
		Render::DisableAttribArray(7);
	}
	
	Render::EnableBlend();
	SetRenderTarget(kRenderTargetPrimary);
}

void GraphicsMgr::DrawStructureList(const List<Renderable> *renderList)
{
	#if !C4SERVER
	
		if (!(currentStructureFlags & kStructureRenderVelocity))
		{
			DrawStructureDepthList(renderList);
			return;
		}
		
		for (Renderable *renderable = renderList->First(); renderable; renderable = renderable->Next())
		{
			Transform4D		mv, *modelview;
			Matrix4D		mvp1;
			
			unsigned_int32 flags = renderable->GetRenderableFlags();
			if (flags & kRenderableStructureBufferInhibit) continue;
			
			const Transformable *transformable = renderable->GetTransformable();
			if (transformable)
			{
				mv = cameraSpaceTransform * transformable->GetWorldTransform();
				modelview = &mv;
			}
			else
			{
				modelview = &cameraSpaceTransform;
			}
			
			if (flags & kRenderableMotionBlurGradient)
			{
				static const char edgeVertexIndex[24] =
				{
					0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 0, 4, 1, 5, 2, 6, 3, 7
				};
				
				Vector4D	vertex[8];
				float 		xmin, xmax, ymin, ymax;
				
				Matrix4D transform = standardProjectionMatrix * *modelview;
				const Box3D *box = renderable->GetMotionBlurBox();
				
				vertex[0] = transform * Point3D(box->min.x, box->min.y, box->min.z);
				vertex[1] = transform * Point3D(box->max.x, box->min.y, box->min.z);
				vertex[2] = transform * Point3D(box->max.x, box->max.y, box->min.z);
				vertex[3] = transform * Point3D(box->min.x, box->max.y, box->min.z);
				vertex[4] = transform * Point3D(box->min.x, box->min.y, box->max.z);
				vertex[5] = transform * Point3D(box->max.x, box->min.y, box->max.z);
				vertex[6] = transform * Point3D(box->max.x, box->max.y, box->max.z);
				vertex[7] = transform * Point3D(box->min.x, box->max.y, box->max.z);
				
				bool visible = false;
				const char *edge = edgeVertexIndex;
				for (machine a = 0; a < 12; a++, edge += 2)
				{
					Vector4D p1 = vertex[edge[0]];
					Vector4D p2 = vertex[edge[1]];
					
					if (p1.z < -p1.w)
					{
						if (p2.z < -p2.w) continue;
						
						Vector4D dp = p1 - p2;
						p1 -= dp * ((p1.z + p1.w) / (dp.w + dp.z));
					}
					else if (p2.z < -p2.w)
					{
						Vector4D dp = p2 - p1;
						p2 -= dp * ((p2.z + p2.w) / (dp.w + dp.z));
					}
					
					float f1 = 1.0F / p1.w;
					float f2 = 1.0F / p2.w;
					float x1 = p1.x * f1;
					float x2 = p2.x * f2;
					float y1 = p1.y * f1;
					float y2 = p2.y * f2;
					
					if (!visible)
					{
						visible = true;
						
						if (x1 < x2)
						{
							xmin = x1;
							xmax = x2;
						}
						else
						{
							xmin = x2;
							xmax = x1;
						}
						
						if (y1 < y2)
						{
							ymin = y1;
							ymax = y2;
						}
						else
						{
							ymin = y2;
							ymax = y1;
						}
					}
					else
					{
						if (x1 < xmin) xmin = x1;
						else if (x1 > xmax) xmax = x1;
						if (x2 < xmin) xmin = x2;
						else if (x2 > xmax) xmax = x2;
						
						if (y1 < ymin) ymin = y1;
						else if (y1 > ymax) ymax = y1;
						if (y2 < ymin) ymin = y2;
						else if (y2 > ymax) ymax = y2;
					}
				}
				
				if (!visible) continue;
				
				int32 left = MaxZero((int32) ((xmin * 0.5F + motionBlurBoxLeftOffset) * (float) kProcessGridWidth));
				int32 right = Min((int32) ((xmax * 0.5F + motionBlurBoxRightOffset) * (float) kProcessGridWidth), kProcessGridWidth - 1);
				int32 bottom = MaxZero((int32) ((ymin * 0.5F + motionBlurBoxBottomOffset) * (float) kProcessGridHeight));
				int32 top = Min((int32) ((ymax * 0.5F + motionBlurBoxTopOffset) * (float) kProcessGridHeight), kProcessGridHeight - 1);
				
				for (machine j = bottom; j <= top; j++)
				{
					machine k = j * kProcessGridWidth;
					for (machine i = left; i <= right; i++) motionGridFlag[k + i] = true;
				}
			}
			
			unsigned_int32 newRenderState = kRenderDepthTest | (renderable->GetRenderState() & (kRenderDepthInhibit | kRenderDepthOffset));
			SetRenderState(newRenderState);
			
			if (newRenderState & kRenderDepthOffset)
			{
				float z = Fmin(cameraSpaceTransform.GetRow(2) ^ renderable->GetDepthOffsetPoint(), -cameraObject->GetNearDepth());
				float delta = renderable->GetDepthOffsetDelta();
				float epsilon = depthOffsetConstant * delta / (z * (z + delta));
				epsilon = Fmax(Fabs(epsilon), 4.8e-7F) * NonzeroFsgn(epsilon);
				
				currentProjectionMatrix = cameraProjectionMatrix;
				currentProjectionMatrix(2,2) *= 1.0F + epsilon;
			}
			
			SetModelviewMatrix(*modelview);
			
			if (transformable)
			{
				const Transform4D *previousWorldTransform = renderable->GetPreviousWorldTransformPointer();
				if (previousWorldTransform) mvp1 = currentProjectionMatrix * (previousCameraSpaceTransform * *previousWorldTransform);
				else mvp1 = currentProjectionMatrix * (previousCameraSpaceTransform * transformable->GetWorldTransform());
			}
			else
			{
				mvp1 = currentProjectionMatrix * previousCameraSpaceTransform;
			}
			
			float l = (float) viewportRect.left;
			float b = (float) viewportRect.bottom;
			float w = ((float) viewportRect.right - l) * 0.5F;
			float h = ((float) viewportRect.top - b) * 0.5F;
			l += w;
			b += h;
			
			Render::SetVertexProgramParameter4f(kVertexParamMatrixVelocityA, mvp1(0,0) * w + mvp1(3,0) * l, mvp1(0,1) * w + mvp1(3,1) * l, mvp1(0,2) * w + mvp1(3,2) * l, mvp1(0,3) * w + mvp1(3,3) * l);
			Render::SetVertexProgramParameter4f(kVertexParamMatrixVelocityA + 1, mvp1(1,0) * h + mvp1(3,0) * b, mvp1(1,1) * h + mvp1(3,1) * b, mvp1(1,2) * h + mvp1(3,2) * b, mvp1(1,3) * h + mvp1(3,3) * b);
			Render::SetVertexProgramParameter4f(kVertexParamMatrixVelocityA + 2, (mvp1(2,0) + mvp1(3,0)) * 0.5F, (mvp1(2,1) + mvp1(3,1)) * 0.5F, (mvp1(2,2) + mvp1(3,2)) * 0.5F, (mvp1(2,3) + mvp1(3,3)) * 0.5F);
			Render::SetVertexProgramParameter4f(kVertexParamMatrixVelocityA + 3, mvp1(3,0), mvp1(3,1), mvp1(3,2), mvp1(3,3));
			
			const Matrix4D& mvp2 = currentMVPMatrix;
			Render::SetVertexProgramParameter4f(kVertexParamMatrixVelocityB, mvp2(0,0) * w + mvp2(3,0) * l, mvp2(0,1) * w + mvp2(3,1) * l, mvp2(0,2) * w + mvp2(3,2) * l, mvp2(0,3) * w + mvp2(3,3) * l);
			Render::SetVertexProgramParameter4f(kVertexParamMatrixVelocityB + 1, mvp2(1,0) * h + mvp2(3,0) * b, mvp2(1,1) * h + mvp2(3,1) * b, mvp2(1,2) * h + mvp2(3,2) * b, mvp2(1,3) * h + mvp2(3,3) * b);
			Render::SetVertexProgramParameter4f(kVertexParamMatrixVelocityB + 2, (mvp2(2,0) + mvp2(3,0)) * 0.5F, (mvp2(2,1) + mvp2(3,1)) * 0.5F, (mvp2(2,2) + mvp2(3,2)) * 0.5F, (mvp2(2,3) + mvp2(3,3)) * 0.5F);
			Render::SetVertexProgramParameter4f(kVertexParamMatrixVelocityB + 3, mvp2(3,0), mvp2(3,1), mvp2(3,2), mvp2(3,3));
			
			RenderSegment *segment = renderable->GetFirstRenderSegment();
			do
			{
				const ShaderData *shaderData = segment->GetShaderData(kShaderStructure, renderable->GetShaderDetailLevel());
				if (!shaderData) shaderData = segment->InitShaderData(renderable, kShaderStructure);
				
				if (!shaderData->programData[kShaderVariantNormal].vertexProgram) continue;
				
				SetMaterialState(segment, shaderData->materialState);
				
				Render::SetVertexProgram(shaderData->programData[kShaderVariantNormal].vertexProgram);
				Render::SetFragmentProgram(shaderData->programData[kShaderVariantNormal].fragmentProgram);
				
				SetVertexArray(shaderData);
				SetPosition1Array(shaderData);
				SetPreviousArray(shaderData);
				SetNormalArray(shaderData);
				SetOffsetArray(shaderData);
				SetColorArray(shaderData);
				SetAuxColorArray(shaderData, kShaderArrayColor1);
				SetAuxColorArray(shaderData, kShaderArrayColor2);
				for (machine a = kShaderArrayTexture0; a < kMaxShaderArrayCount; a++) SetTexcoordArray(shaderData, a);
				
				int32 stateFuncCount = shaderData->shaderStateDataCount;
				for (machine a = 0; a < stateFuncCount; a++) (*shaderData->shaderStateData[a].stateFunc)(renderable, shaderData->shaderStateData[a].stateCookie);
				
				int32 unitCount = shaderData->textureUnitCount;
				const Render::TextureObject *const *textureObject = shaderData->textureObject;
				for (machine unit = 0; unit < unitCount; unit++)
				{
					const Render::TextureObject *texture = textureObject[unit];
					if (texture) Render::BindTexture(unit, texture);
				}
				
				int32 vertexCount = renderable->GetVertexCount();
				graphicsCounter[kGraphicsCounterVelocityVertices] += vertexCount;
				graphicsCounter[kGraphicsCounterVelocityCommands]++;
				
				switch (renderable->GetRenderType())
				{
					case kRenderTriangles:
						
						graphicsCounter[kGraphicsCounterVelocityPrimitives] += vertexCount / 3;
						Render::DrawArrays(Render::kPrimitiveTriangles, 0, vertexCount);
						break;
					
					case kRenderTriangleStrip:
						
						graphicsCounter[kGraphicsCounterVelocityPrimitives] += vertexCount - 2;
						Render::DrawArrays(Render::kPrimitiveTriangleStrip, 0, vertexCount);
						break;
					
					case kRenderIndexedTriangles:
					{
						int32 triangleCount = segment->GetFaceCount();
						graphicsCounter[kGraphicsCounterVelocityPrimitives] += triangleCount;
						
						const VertexBuffer *indexBuffer = shaderData->indexBuffer;
						if (indexBuffer)
						{
							Render::SetIndexVertexBuffer(indexBuffer);
							
							const unsigned_int16 *ptr = IndexArrayOffsetToPtr(renderable->GetFaceOffset()) + segment->GetFaceStart() * 3;
							Render::DrawElements(Render::kPrimitiveTriangles, vertexCount, triangleCount * 3, ptr);
						}
						else
						{
							Render::ResetIndexVertexBuffer();
							
							const unsigned_int16 *ptr = renderable->GetFaceArray() + segment->GetFaceStart() * 3;
							Render::DrawElements(Render::kPrimitiveTriangles, vertexCount, triangleCount * 3, ptr);
						}
						
						break;
					}
					
					case kRenderQuads:
						
						graphicsCounter[kGraphicsCounterVelocityPrimitives] += vertexCount >> 2;
						Render::DrawArrays(Render::kPrimitiveQuads, 0, vertexCount);
						break;
					
					case kRenderMultiIndexedTriangles:
						
						Render::SetIndexVertexBuffer(shaderData->indexBuffer);
						Render::MultiDrawElements(Render::kPrimitiveTriangles, segment->GetMultiCountArray(), reinterpret_cast<const void *const *>(segment->GetMultiOffsetArray()), segment->GetMultiRenderCount());
						break;
					
					case kRenderMaskedMultiIndexedTriangles:
					{
						static const void		*index[33];
						static unsigned_int32	count[33];
						
						Render::SetIndexVertexBuffer(shaderData->indexBuffer);
						
						unsigned_int32 size = 0;
						const unsigned_int16 *ptr = IndexArrayOffsetToPtr(renderable->GetFaceOffset());
						
						int32 triangleCount = segment->GetFaceCount();
						if (triangleCount != 0)
						{
							graphicsCounter[kGraphicsCounterVelocityPrimitives] += triangleCount;
							
							index[0] = ptr;
							count[0] = triangleCount * 3;
							size = 1;
						}
						
						unsigned_int32 mask = segment->GetMultiRenderMask();
						const int32 *data = segment->GetMultiRenderData();
						while (mask != 0)
						{
							if (mask & 1)
							{
								triangleCount = data[1];
								graphicsCounter[kGraphicsCounterVelocityPrimitives] += triangleCount;
								
								index[size] = ptr + data[0] * 3;
								count[size] = triangleCount * 3;
								size++;
							}
							
							mask >>= 1;
							data += 2;
						}
						
						Render::MultiDrawElements(Render::kPrimitiveTriangles, count, index, size);
						break;
					}
				}
			} while ((segment = segment->GetNextRenderSegment()) != nullptr);
		}
	
	#endif
}

void GraphicsMgr::DrawStructureDepthList(const List<Renderable> *renderList)
{
	#if !C4SERVER
	
		for (Renderable *renderable = renderList->First(); renderable; renderable = renderable->Next())
		{
			Transform4D		mv;
			Transform4D		*modelview;
			
			if (renderable->GetRenderableFlags() & kRenderableStructureBufferInhibit) continue;
			
			const Transformable *transformable = renderable->GetTransformable();
			if (transformable)
			{
				mv = cameraSpaceTransform * transformable->GetWorldTransform();
				modelview = &mv;
			}
			else
			{
				modelview = &cameraSpaceTransform;
			}
			
			unsigned_int32 newRenderState = kRenderDepthTest | (renderable->GetRenderState() & (kRenderDepthInhibit | kRenderDepthOffset));
			SetRenderState(newRenderState);
			
			if (newRenderState & kRenderDepthOffset)
			{
				float z = Fmin(cameraSpaceTransform.GetRow(2) ^ renderable->GetDepthOffsetPoint(), -cameraObject->GetNearDepth());
				float delta = renderable->GetDepthOffsetDelta();
				float epsilon = depthOffsetConstant * delta / (z * (z + delta));
				epsilon = Fmax(Fabs(epsilon), 4.8e-7F) * NonzeroFsgn(epsilon);
				
				currentProjectionMatrix = cameraProjectionMatrix;
				currentProjectionMatrix(2,2) *= 1.0F + epsilon;
			}
			
			SetModelviewMatrix(*modelview);
			
			RenderSegment *segment = renderable->GetFirstRenderSegment();
			do
			{
				const ShaderData *shaderData = segment->GetShaderData(kShaderStructure, renderable->GetShaderDetailLevel());
				if (!shaderData) shaderData = segment->InitShaderData(renderable, kShaderStructure);
				
				if (!shaderData->programData[kShaderVariantNormal].vertexProgram) continue;
				
				SetMaterialState(segment, shaderData->materialState);
				
				Render::SetVertexProgram(shaderData->programData[kShaderVariantNormal].vertexProgram);
				Render::SetFragmentProgram(shaderData->programData[kShaderVariantNormal].fragmentProgram);
				
				SetVertexArray(shaderData);
				SetPosition1Array(shaderData);
				SetPreviousArray(shaderData);
				SetNormalArray(shaderData);
				SetOffsetArray(shaderData);
				SetColorArray(shaderData);
				SetAuxColorArray(shaderData, kShaderArrayColor1);
				SetAuxColorArray(shaderData, kShaderArrayColor2);
				for (machine a = kShaderArrayTexture0; a < kMaxShaderArrayCount; a++) SetTexcoordArray(shaderData, a);
				
				int32 stateFuncCount = shaderData->shaderStateDataCount;
				for (machine a = 0; a < stateFuncCount; a++) (*shaderData->shaderStateData[a].stateFunc)(renderable, shaderData->shaderStateData[a].stateCookie);
				
				int32 unitCount = shaderData->textureUnitCount;
				const Render::TextureObject *const *textureObject = shaderData->textureObject;
				for (machine unit = 0; unit < unitCount; unit++)
				{
					const Render::TextureObject *texture = textureObject[unit];
					if (texture) Render::BindTexture(unit, texture);
				}
				
				int32 vertexCount = renderable->GetVertexCount();
				graphicsCounter[kGraphicsCounterVelocityVertices] += vertexCount;
				graphicsCounter[kGraphicsCounterVelocityCommands]++;
				
				switch (renderable->GetRenderType())
				{
					case kRenderTriangles:
						
						graphicsCounter[kGraphicsCounterVelocityPrimitives] += vertexCount / 3;
						Render::DrawArrays(Render::kPrimitiveTriangles, 0, vertexCount);
						break;
					
					case kRenderTriangleStrip:
						
						graphicsCounter[kGraphicsCounterVelocityPrimitives] += vertexCount - 2;
						Render::DrawArrays(Render::kPrimitiveTriangleStrip, 0, vertexCount);
						break;
					
					case kRenderIndexedTriangles:
					{
						int32 triangleCount = segment->GetFaceCount();
						graphicsCounter[kGraphicsCounterVelocityPrimitives] += triangleCount;
						
						const VertexBuffer *indexBuffer = shaderData->indexBuffer;
						if (indexBuffer)
						{
							Render::SetIndexVertexBuffer(indexBuffer);
							
							const unsigned_int16 *ptr = IndexArrayOffsetToPtr(renderable->GetFaceOffset()) + segment->GetFaceStart() * 3;
							Render::DrawElements(Render::kPrimitiveTriangles, vertexCount, triangleCount * 3, ptr);
						}
						else
						{
							Render::ResetIndexVertexBuffer();
							
							const unsigned_int16 *ptr = renderable->GetFaceArray() + segment->GetFaceStart() * 3;
							Render::DrawElements(Render::kPrimitiveTriangles, vertexCount, triangleCount * 3, ptr);
						}
						
						break;
					}
					
					case kRenderQuads:
						
						graphicsCounter[kGraphicsCounterVelocityPrimitives] += vertexCount >> 2;
						Render::DrawArrays(Render::kPrimitiveQuads, 0, vertexCount);
						break;
					
					case kRenderMultiIndexedTriangles:
						
						Render::SetIndexVertexBuffer(shaderData->indexBuffer);
						Render::MultiDrawElements(Render::kPrimitiveTriangles, segment->GetMultiCountArray(), reinterpret_cast<const void *const *>(segment->GetMultiOffsetArray()), segment->GetMultiRenderCount());
						break;
					
					case kRenderMaskedMultiIndexedTriangles:
					{
						static const void		*index[33];
						static unsigned_int32	count[33];
						
						Render::SetIndexVertexBuffer(shaderData->indexBuffer);
						
						unsigned_int32 size = 0;
						const unsigned_int16 *ptr = IndexArrayOffsetToPtr(renderable->GetFaceOffset());
						
						int32 triangleCount = segment->GetFaceCount();
						if (triangleCount != 0)
						{
							graphicsCounter[kGraphicsCounterVelocityPrimitives] += triangleCount;
							
							index[0] = ptr;
							count[0] = triangleCount * 3;
							size = 1;
						}
						
						unsigned_int32 mask = segment->GetMultiRenderMask();
						const int32 *data = segment->GetMultiRenderData();
						while (mask != 0)
						{
							if (mask & 1)
							{
								triangleCount = data[1];
								graphicsCounter[kGraphicsCounterVelocityPrimitives] += triangleCount;
								
								index[size] = ptr + data[0] * 3;
								count[size] = triangleCount * 3;
								size++;
							}
							
							mask >>= 1;
							data += 2;
						}
						
						Render::MultiDrawElements(Render::kPrimitiveTriangles, count, index, size);
						break;
					}
				}
			} while ((segment = segment->GetNextRenderSegment()) != nullptr);
		}
	
	#endif
}

bool GraphicsMgr::BeginDistortionRendering(void)
{
	if ((renderOptionFlags & kRenderOptionDistortion) && (currentRenderTargetType == kRenderTargetPrimary))
	{
		SetRenderTarget(kRenderTargetDistortion);
		SetRenderState(kRenderDepthTest | kRenderDepthInhibit);
		
		Render::SetClearColor(0.0F, 0.0F, 0.0F, 0.0F);
		Render::Clear(Render::kClearBufferColor);
		
		SetBlendState(kBlendAccumulate | kBlendAlphaAccumulate);
		
		unsigned_int32 graphicsState = currentGraphicsState;
		if (!(graphicsState & kGraphicsDepthTestLess))
		{
			currentGraphicsState = graphicsState | kGraphicsDepthTestLess;
			Render::SetDepthFunc(Render::kDepthLess);
		}
		
		ResetArrayState();
		
		currentGraphicsState |= kGraphicsDistortionAvail;
		return (true);
	}
	
	return (false);
}

void GraphicsMgr::EndDistortionRendering(void)
{
	SetRenderTarget(kRenderTargetPrimary);
}

void GraphicsMgr::DrawDistortionList(const List<Renderable> *renderList)
{
	#if !C4SERVER
	
		for (Renderable *renderable = renderList->First(); renderable; renderable = renderable->Next())
		{
			unsigned_int32 newRenderState = renderable->GetRenderState();
			SetRenderState(newRenderState);
			
			unsigned_int32 renderableFlags = renderable->GetRenderableFlags();
			const Transformable *transformable = renderable->GetTransformable();
			if (!(renderableFlags & kRenderableCameraTransformInhibit))
			{
				if (newRenderState & kRenderDepthOffset)
				{
					float z = Fmin(cameraSpaceTransform.GetRow(2) ^ renderable->GetDepthOffsetPoint(), -cameraObject->GetNearDepth());
					float delta = renderable->GetDepthOffsetDelta();
					float epsilon = depthOffsetConstant * delta / (z * (z + delta));
					epsilon = Fmax(Fabs(epsilon), 4.8e-7F) * NonzeroFsgn(epsilon);
					
					currentProjectionMatrix = cameraProjectionMatrix;
					currentProjectionMatrix(2,2) *= 1.0F + epsilon;
				}
				
				if (transformable) SetModelviewMatrix(cameraSpaceTransform * transformable->GetWorldTransform());
				else SetModelviewMatrix(cameraSpaceTransform);
			}
			else
			{
				if (transformable) SetModelviewMatrix(transformable->GetWorldTransform());
				else SetModelviewMatrix(K::identity_4D);
			}
			
			RenderSegment *segment = renderable->GetFirstRenderSegment();
			do
			{
				const ShaderData *shaderData = segment->GetShaderData(kShaderAmbient, renderable->GetShaderDetailLevel());
				if ((!shaderData) || ((shaderData->variantMask & (1 << kShaderVariantNormal)) == 0))
				{
					shaderData = segment->InitShaderData(renderable, kShaderAmbient, kShaderVariantNormal);
				}
				
				if (!shaderData->programData[kShaderVariantNormal].vertexProgram) continue;
				
				SetMaterialState(nullptr, shaderData->materialState & kMaterialTwoSided);
				
				Render::SetVertexProgram(shaderData->programData[kShaderVariantNormal].vertexProgram);
				Render::SetFragmentProgram(shaderData->programData[kShaderVariantNormal].fragmentProgram);
				
				SetVertexArray(shaderData);
				SetNormalArray(shaderData);
				SetTangentArray(shaderData);
				SetColorArray(shaderData);
				for (machine a = kShaderArrayTexture0; a < kMaxShaderArrayCount; a++) SetTexcoordArray(shaderData, a);
				
				int32 stateFuncCount = shaderData->shaderStateDataCount;
				for (machine a = 0; a < stateFuncCount; a++) (*shaderData->shaderStateData[a].stateFunc)(renderable, shaderData->shaderStateData[a].stateCookie);
				
				int32 unitCount = shaderData->textureUnitCount;
				const Render::TextureObject *const *textureObject = shaderData->textureObject;
				for (machine unit = 0; unit < unitCount; unit++)
				{
					const Render::TextureObject *texture = textureObject[unit];
					if (texture) Render::BindTexture(unit, texture);
				}
				
				int32 vertexCount = renderable->GetVertexCount();
				graphicsCounter[kGraphicsCounterDistortionVertices] += vertexCount;
				graphicsCounter[kGraphicsCounterDistortionCommands]++;
				
				switch (renderable->GetRenderType())
				{
					case kRenderTriangles:
						
						graphicsCounter[kGraphicsCounterDistortionPrimitives] += vertexCount / 3;
						Render::DrawArrays(Render::kPrimitiveTriangles, 0, vertexCount);
						break;
					
					case kRenderTriangleStrip:
						
						graphicsCounter[kGraphicsCounterDistortionPrimitives] += vertexCount - 2;
						Render::DrawArrays(Render::kPrimitiveTriangleStrip, 0, vertexCount);
						break;
					
					case kRenderIndexedTriangles:
					{
						int32 triangleCount = segment->GetFaceCount();
						graphicsCounter[kGraphicsCounterDistortionPrimitives] += triangleCount;
						
						const VertexBuffer *indexBuffer = shaderData->indexBuffer;
						if (indexBuffer)
						{
							Render::SetIndexVertexBuffer(indexBuffer);
							
							const unsigned_int16 *ptr = IndexArrayOffsetToPtr(renderable->GetFaceOffset()) + segment->GetFaceStart() * 3;
							Render::DrawElements(Render::kPrimitiveTriangles, vertexCount, triangleCount * 3, ptr);
						}
						else
						{
							Render::ResetIndexVertexBuffer();
							
							const unsigned_int16 *ptr = renderable->GetFaceArray() + segment->GetFaceStart() * 3;
							Render::DrawElements(Render::kPrimitiveTriangles, vertexCount, triangleCount * 3, ptr);
						}
						
						break;
					}
					
					case kRenderQuads:
						
						graphicsCounter[kGraphicsCounterDistortionPrimitives] += vertexCount >> 2;
						Render::DrawArrays(Render::kPrimitiveQuads, 0, vertexCount);
						break;
				}
			} while ((segment = segment->GetNextRenderSegment()) != nullptr);
		}
	
	#endif
}

bool GraphicsMgr::ActivateShadowBounds(const StencilShadow *shadow)
{
	const Polyhedron *polyhedron = shadow->GetShadowPolyhedron();
	int32 edgeCount = polyhedron->edgeCount;
	if (edgeCount != 0)
	{
		float 	xmin, xmax, ymin, ymax, zmin, zmax;
		
		Matrix4D transform = standardProjectionMatrix * cameraSpaceTransform;
		
		const Point3D *vertex = polyhedron->vertex;
		const Edge *edge = polyhedron->edge;
		
		bool visible = false;
		for (machine a = 0; a < edgeCount; a++, edge++)
		{
			Vector4D p1 = transform * vertex[edge->vertexIndex[0]];
			Vector4D p2 = transform * vertex[edge->vertexIndex[1]];
			
			if (p1.z < -p1.w)
			{
				if (p2.z < -p2.w) continue;
				
				Vector4D dp = p1 - p2;
				p1 -= dp * ((p1.z + p1.w) / (dp.w + dp.z));
			}
			else if (p2.z < -p2.w)
			{
				Vector4D dp = p2 - p1;
				p2 -= dp * ((p2.z + p2.w) / (dp.w + dp.z));
			}
			
			float f1 = 0.5F / p1.w;
			float f2 = 0.5F / p2.w;
			float x1 = p1.x * f1;
			float x2 = p2.x * f2;
			float y1 = p1.y * f1;
			float y2 = p2.y * f2;
			float z1 = p1.z * f1;
			float z2 = p2.z * f2;
			
			if (!visible)
			{
				visible = true;
				
				if (x1 < x2)
				{
					xmin = x1;
					xmax = x2;
				}
				else
				{
					xmin = x2;
					xmax = x1;
				}
				
				if (y1 < y2)
				{
					ymin = y1;
					ymax = y2;
				}
				else
				{
					ymin = y2;
					ymax = y1;
				}
				
				if (z1 < z2)
				{
					zmin = z1;
					zmax = z2;
				}
				else
				{
					zmin = z2;
					zmax = z1;
				}
			}
			else
			{
				if (x1 < xmin) xmin = x1;
				else if (x1 > xmax) xmax = x1;
				if (x2 < xmin) xmin = x2;
				else if (x2 > xmax) xmax = x2;
				
				if (y1 < ymin) ymin = y1;
				else if (y1 > ymax) ymax = y1;
				if (y2 < ymin) ymin = y2;
				else if (y2 > ymax) ymax = y2;
				
				if (z1 < zmin) zmin = z1;
				else if (z1 > zmax) zmax = z1;
				if (z2 < zmin) zmin = z2;
				else if (z2 > zmax) zmax = z2;
			}
		}
		
		if (visible)
		{
			float viewLeft = (float) viewportRect.left;
			float viewWidth = (float) viewportRect.Width();
			shadowRect.left = Max((int32) (viewLeft + viewWidth * (xmin + 0.5F)), lightRect.left);
			shadowRect.right = Min((int32) (viewLeft + viewWidth * (xmax + 0.5F)), lightRect.right);
			
			float viewBottom = (float) viewportRect.bottom;
			float viewHeight = (float) viewportRect.Height();
			shadowRect.bottom = Max((int32) (viewBottom - viewHeight * (ymin + 0.5F)), lightRect.bottom);
			shadowRect.top = Min((int32) (viewBottom - viewHeight * (ymax + 0.5F)), lightRect.top);
			
			if ((shadowRect.left < shadowRect.right) && (shadowRect.bottom < shadowRect.top))
			{
				unsigned_int32 graphicsState = currentGraphicsState;
				if (graphicsState & kGraphicsObliqueFrustum)
				{
					currentGraphicsState = graphicsState | kGraphicsUpdateShadowScissor;
					
					#if C4DIAGNOSTICS
					
						if (diagnosticFlags & kDiagnosticShadowBounds) DrawShadowBoundsDiagnostic(shadow);
					
					#endif
					
					return (true);
				}
				
				zmin = Fmax(zmin + 0.5F, lightDepthBounds.min);
				zmax = Fmin(zmax + 0.5F, lightDepthBounds.max);
				
				if (zmin < zmax)
				{
					graphicsState |= kGraphicsUpdateShadowScissor;
					
					if (graphicsState & kGraphicsDepthBoundsAvail)
					{
						if (!(graphicsState & kGraphicsDepthBoundsMask)) Render::EnableDepthBoundsTest();
						graphicsState |= kGraphicsShadowDepthBounds;
						Render::SetDepthBounds(zmin, zmax);
					}
					
					currentGraphicsState = graphicsState;
					
					#if C4DIAGNOSTICS
					
						if (diagnosticFlags & kDiagnosticShadowBounds) DrawShadowBoundsDiagnostic(shadow);
					
					#endif
					
					return (true);
				}
			}
		}
	}
	else
	{
		DeactivateShadowBounds();
		return (true);
	}
	
	return (false);
}

void GraphicsMgr::DeactivateShadowBounds(void)
{
	unsigned_int32 graphicsState = currentGraphicsState;
	
	if (graphicsState & kGraphicsShadowScissor)
	{
		graphicsState &= ~kGraphicsShadowScissor;
		const Rect& rect = (graphicsState & kGraphicsLightScissor) ? lightRect : scissorRect;
		Render::SetScissor(rect.left, rect.bottom, rect.Width(), -rect.Height());
	}
	
	if (graphicsState & kGraphicsShadowDepthBounds)
	{
		float	dmin, dmax;
		
		graphicsState &= ~kGraphicsShadowDepthBounds;
		
		if (graphicsState & kGraphicsLightDepthBounds)
		{
			dmin = lightDepthBounds.min;
			dmax = lightDepthBounds.max;
		}
		else
		{
			dmin = 0.0F;
			dmax = 1.0F;
		}
		
		Render::SetDepthBounds(dmin, dmax);
	}
	
	currentGraphicsState = graphicsState;
}

#if C4DIAGNOSTICS

	void GraphicsMgr::DrawShadowBoundsDiagnostic(const StencilShadow *shadow)
	{
		static const VertexSnippet *diagnosticTransform[2] =
		{
			&VertexProgram::modelviewProjectTransform, &VertexProgram::outputPrimaryColor
		};
		
		static Link<VertexProgram>	diagnosticProgram;
		
		SetLocalVertexProgram(&diagnosticProgram, 2, diagnosticTransform);
		SetBlendState(kBlendReplace | kBlendAlphaPreserve);
		
		SetModelviewMatrix(cameraSpaceTransform);
		
		int32 left = scissorRect.left;
		int32 bottom = scissorRect.bottom;
		Render::SetScissor(left, bottom, scissorRect.right - left, scissorRect.top - bottom);
		
		Render::DisableDepthTest();
		if (currentGraphicsState & kGraphicsDepthBoundsMask) Render::DisableDepthBoundsTest();
		
		Render::SetPolygonMode(Render::kPolygonModeLine);
		Render::SetColorMask(true, true, true, true);
		
		Render::SetColor4ub(255, 0, 0, 0);
		Render::Begin(Render::kPrimitiveLines);
		
		const Polyhedron *polyhedron = shadow->GetShadowPolyhedron();
		const Point3D *vertex = polyhedron->vertex;
		const Edge *edge = polyhedron->edge;
		
		int32 edgeCount = polyhedron->edgeCount;
		for (machine a = 0; a < edgeCount; a++)
		{
			Render::SetVertex3fv(&vertex[edge->vertexIndex[0]].x);
			Render::SetVertex3fv(&vertex[edge->vertexIndex[1]].x);
			edge++;
		}
		
		Render::End();
		Render::SetColor4ub(0, 0, 0, 0);
		
		Render::EnableDepthTest();
		if (currentGraphicsState & kGraphicsDepthBoundsMask) Render::EnableDepthBoundsTest();
		
		Render::SetPolygonMode(Render::kPolygonModeFill);
		Render::SetColorMask(false, false, false, false);
	}

#endif

void GraphicsMgr::BeginStencilShadow(void)
{
	unsigned_int32 graphicsState = (currentGraphicsState | kGraphicsRenderStencil) & ~kGraphicsStencilValid;
	if (graphicsState & kGraphicsStencilClear) graphicsState |= kGraphicsStencilValid;
	
	if (!(graphicsState & kGraphicsDepthTestLess))
	{
		graphicsState |= kGraphicsDepthTestLess;
		Render::SetDepthFunc(Render::kDepthLess);
	}
	
	currentGraphicsState = graphicsState;
	
	SetMaterialState(nullptr, kMaterialTwoSided);
	SetRenderState(kRenderDepthTest | kRenderColorInhibit | kRenderDepthInhibit);
	
	ResetArrayState();
	currentVertexBuffer[kShaderArrayPosition0] = nullptr;
	shaderArrayPointer[kShaderArrayPosition0] = nullptr;
	
	Render::ResetAttributeVertexBuffer();
	Render::ResetIndexVertexBuffer();
		
	SetLocalFragmentProgram(kLocalProgramCopyVertexColor);
}

void GraphicsMgr::EndStencilShadow(void)
{
	DeactivateShadowBounds();
	
	if (currentStencilMode != kStencilNone)
	{
		currentStencilMode = kStencilNone;
		Render::SetStencilOp(Render::kStencilKeep, Render::kStencilKeep, Render::kStencilKeep);
	}
	
	currentGraphicsState &= ~(kGraphicsUpdateShadowScissor | kGraphicsRenderStencil);
}

void GraphicsMgr::DrawStencilShadow(const StencilShadow *shadow, StencilType type, StencilMode mode)
{
	#if !C4SERVER
	
		static const VertexSnippet *shadowTransform[kStencilTypeCount] =
		{
			&VertexProgram::shadowInfiniteExtrusionTransform,
			&VertexProgram::shadowPointExtrusionTransform,
			&VertexProgram::shadowEndcapProjectionTransform,
			&VertexProgram::modelviewProjectTransform
		};
		
		static Link<VertexProgram>	stencilProgram[kStencilTypeCount];
		
		unsigned_int32 graphicsState = currentGraphicsState;
		if (!(graphicsState & kGraphicsStencilValid))
		{
			graphicsState |= kGraphicsStencilValid;
			graphicsCounter[kGraphicsCounterStencilClears]++;
			
			Render::Clear(Render::kClearBufferStencil);
		}
		
		if (graphicsState & kGraphicsUpdateShadowScissor)
		{
			graphicsState |= kGraphicsShadowScissor;
			Render::SetScissor(shadowRect.left, shadowRect.bottom, shadowRect.Width(), -shadowRect.Height());
		}
		
		const Transformable *transformable = geometryTransformable;
		if (transformable) SetModelviewMatrix(cameraSpaceTransform * transformable->GetWorldTransform());
		else SetModelviewMatrix(cameraSpaceTransform);
		
		SetLocalVertexProgram(&stencilProgram[type], shadowTransform[type]);
		
		if (mode == kStencilPass)
		{
			if (currentStencilMode != kStencilPass)
			{
				currentStencilMode = kStencilPass;
				Render::SetBackStencilOp(Render::kStencilKeep, Render::kStencilKeep, Render::kStencilDecrWrap);
				Render::SetFrontStencilOp(Render::kStencilKeep, Render::kStencilKeep, Render::kStencilIncrWrap);
			}
			
			graphicsCounter[kGraphicsCounterStencilCommands]++;
			
			switch (type)
			{
				case kStencilInfiniteExtrusion:
				{
					int32 triangleCount = shadow->GetExtrusionEdgeCount();
					int32 vertexCount = triangleCount * 3;
					graphicsCounter[kGraphicsCounterStencilVertices] += vertexCount;
					graphicsCounter[kGraphicsCounterStencilPrimitives] += triangleCount;
					
					Render::SetVertexArray(4, Render::kVertexFloat, 16, shadow->GetExtrusionVertexArray());
					Render::DrawArrays(Render::kPrimitiveTriangles, 0, vertexCount);
					break;
				}
				
				case kStencilPointExtrusion:
				{
					int32 quadCount = shadow->GetExtrusionEdgeCount();
					int32 vertexCount = quadCount * 4;
					graphicsCounter[kGraphicsCounterStencilVertices] += vertexCount;
					graphicsCounter[kGraphicsCounterStencilPrimitives] += quadCount;
					
					Render::SetVertexArray(4, Render::kVertexFloat, 16, shadow->GetExtrusionVertexArray());
					Render::DrawArrays(Render::kPrimitiveQuads, 0, vertexCount);
					break;
				}
			}
		}
		else if (mode == kStencilFail)
		{
			if (currentStencilMode != kStencilFail)
			{
				currentStencilMode = kStencilFail;
				Render::SetBackStencilOp(Render::kStencilKeep, Render::kStencilIncrWrap, Render::kStencilKeep);
				Render::SetFrontStencilOp(Render::kStencilKeep, Render::kStencilDecrWrap, Render::kStencilKeep);
			}
			
			graphicsCounter[kGraphicsCounterStencilCommands]++;
			
			switch (type)
			{
				case kStencilInfiniteExtrusion:
				{
					int32 triangleCount = shadow->GetExtrusionEdgeCount();
					int32 vertexCount = triangleCount * 3;
					graphicsCounter[kGraphicsCounterStencilVertices] += vertexCount;
					graphicsCounter[kGraphicsCounterStencilPrimitives] += triangleCount;
					
					Render::SetVertexArray(4, Render::kVertexFloat, 16, shadow->GetExtrusionVertexArray());
					Render::DrawArrays(Render::kPrimitiveTriangles, 0, vertexCount);
					break;
				}
				
				case kStencilPointExtrusion:
				{
					int32 quadCount = shadow->GetExtrusionEdgeCount();
					int32 vertexCount = quadCount * 4;
					graphicsCounter[kGraphicsCounterStencilVertices] += vertexCount;
					graphicsCounter[kGraphicsCounterStencilPrimitives] += quadCount;
					
					Render::SetVertexArray(4, Render::kVertexFloat, 16, shadow->GetExtrusionVertexArray());
					Render::DrawArrays(Render::kPrimitiveQuads, 0, vertexCount);
					break;
				}
				
				case kStencilEndcapProjection:
				{
					int32 triangleCount = shadow->GetBackEndcapTriangleCount();
					int32 vertexCount = shadow->GetGeometryVertexCount();
					graphicsCounter[kGraphicsCounterStencilVertices] += vertexCount;
					graphicsCounter[kGraphicsCounterStencilPrimitives] += triangleCount;
					
					Render::SetVertexArray(3, Render::kVertexFloat, 12, shadow->GetGeometryVertexArray());
					Render::DrawElements(Render::kPrimitiveTriangles, vertexCount, triangleCount * 3, shadow->GetBackEndcapTriangleArray());
					break;
				}
				
				case kStencilEndcapIdentity:
				{
					int32 triangleCount = shadow->GetFrontEndcapTriangleCount();
					int32 vertexCount = shadow->GetGeometryVertexCount();
					graphicsCounter[kGraphicsCounterStencilVertices] += vertexCount;
					graphicsCounter[kGraphicsCounterStencilPrimitives] += triangleCount;
					
					Render::SetVertexArray(3, Render::kVertexFloat, 12, shadow->GetGeometryVertexArray());
					Render::DrawElements(Render::kPrimitiveTriangles, vertexCount, triangleCount * 3, shadow->GetFrontEndcapTriangleArray());
					break;
				}
			}
		}
		else
		{
			graphicsCounter[kGraphicsCounterStencilCommands]++;
			
			int32 triangleCount = shadow->GetBackEndcapTriangleCount();
			int32 vertexCount = shadow->GetGeometryVertexCount();
			graphicsCounter[kGraphicsCounterStencilVertices] += vertexCount;
			graphicsCounter[kGraphicsCounterStencilPrimitives] += triangleCount;
			
			if (currentStencilMode != kStencilDark)
			{
				currentStencilMode = kStencilDark;
				
				Render::SetBackStencilOp(Render::kStencilKeep, Render::kStencilKeep, Render::kStencilKeep);
				Render::SetFrontStencilOp(Render::kStencilKeep, Render::kStencilKeep, Render::kStencilDecrWrap);
			}
			
			Render::SetDepthFunc(Render::kDepthLessEqual);
			Render::SetVertexArray(3, Render::kVertexFloat, 12, shadow->GetGeometryVertexArray());
			Render::DrawElements(Render::kPrimitiveTriangles, vertexCount, triangleCount * 3, shadow->GetBackEndcapTriangleArray());
			Render::SetDepthFunc(Render::kDepthLess);
		}
		
		currentGraphicsState = graphicsState & ~(kGraphicsStencilClear | kGraphicsUpdateShadowScissor);
		
		#if C4DIAGNOSTICS
		
			if (diagnosticFlags & kDiagnosticShadows) DrawStencilDiagnostic(shadow, type, mode);
		
		#endif
	
	#endif
}

#if C4DIAGNOSTICS

	void GraphicsMgr::DrawStencilDiagnostic(const StencilShadow *shadow, StencilType type, StencilMode mode)
	{
		static const VertexSnippet *diagnosticTransform[kStencilTypeCount][2] =
		{
			{&VertexProgram::shadowInfiniteExtrusionTransform, &VertexProgram::outputPrimaryColor},
			{&VertexProgram::shadowPointExtrusionTransform, &VertexProgram::outputPrimaryColor},
			{&VertexProgram::shadowEndcapProjectionTransform, &VertexProgram::outputPrimaryColor},
			{&VertexProgram::modelviewProjectTransform, &VertexProgram::outputPrimaryColor}
		};
		
		static Link<VertexProgram>	diagnosticProgram[kStencilTypeCount];
		
		SetLocalVertexProgram(&diagnosticProgram[type], 2, diagnosticTransform[type]);
		SetBlendState(kBlendAccumulate | kBlendAlphaPreserve);
		
		Render::DisableDepthTest();
		Render::SetPolygonMode(Render::kPolygonModeLine);
		Render::SetColorMask(true, true, true, true);
		
		currentStencilMode = kStencilNone;
		Render::SetStencilOp(Render::kStencilKeep, Render::kStencilKeep, Render::kStencilKeep);
		
		switch (type)
		{
			case kStencilInfiniteExtrusion:
				
				Render::SetColor4ub(16, 32, 4, 0);
				Render::DrawArrays(Render::kPrimitiveTriangles, 0, shadow->GetExtrusionEdgeCount() * 3);
				break;
			
			case kStencilPointExtrusion:
				
				Render::SetColor4ub(16, 32, 4, 0);
				Render::DrawArrays(Render::kPrimitiveQuads, 0, shadow->GetExtrusionEdgeCount() * 4);
				break;
			
			case kStencilEndcapProjection:
				
				Render::SetColor4ub(32, 16, 4, 0);
				Render::DrawElements(Render::kPrimitiveTriangles, shadow->GetGeometryVertexCount(), shadow->GetBackEndcapTriangleCount() * 3, shadow->GetBackEndcapTriangleArray());
				break;
			
			case kStencilEndcapIdentity:
				
				if (mode != kStencilDark)
				{
					Render::SetColor4ub(24, 4, 40, 0);
					Render::DrawElements(Render::kPrimitiveTriangles, shadow->GetGeometryVertexCount(), shadow->GetFrontEndcapTriangleCount() * 3, shadow->GetFrontEndcapTriangleArray());
				}
				else
				{
					Render::SetColor4ub(4, 24, 48, 0);
					Render::DrawElements(Render::kPrimitiveTriangles, shadow->GetGeometryVertexCount(), shadow->GetFrontEndcapTriangleCount() * 3, shadow->GetBackEndcapTriangleArray());
				}
				
				break;
		}
		
		Render::EnableDepthTest();
		Render::SetPolygonMode(Render::kPolygonModeFill);
		Render::SetColorMask(false, false, false, false);
		Render::SetColor4ub(0, 0, 0, 0);
	}

#endif

void GraphicsMgr::BeginShadowMap(void)
{
	savedCameraObject = cameraObject;
	savedCameraTransformable = cameraTransformable;
	
	currentGraphicsState = (currentGraphicsState | kGraphicsRenderShadowMap) & ~kGraphicsScissorMask;
	SetAmbient();
	
	shadowFrameBuffer->GetRenderTargetTexture()->Unbind(kTextureUnitLightProjection);
	Render::SetFrameBuffer(shadowFrameBuffer);
	
	SetBlendState(kBlendReplace);
	SetRenderState((currentRenderState & kRenderDepthTest) | kRenderColorInhibit);
	ResetArrayState(1);
	
	renderTargetHeight = dynamicShadowMapSize * kMaxShadowSectionCount;
	
	Render::SetScissor(0, 0, dynamicShadowMapSize, renderTargetHeight);
	Render::Clear(Render::kClearBufferDepth | Render::kClearBufferStencil);
	
	Render::SetPolygonOffset(1.0F, 1.0F);
	Render::EnablePolygonFillOffset();
	if (capabilities.extensionFlag[kExtensionDepthClamp]) Render::EnableDepthClamp();
}

void GraphicsMgr::EndShadowMap(void)
{
	if ((currentRenderTargetType == kRenderTargetPrimary) && (multisampleFrameBuffer)) Render::SetFrameBuffer(multisampleFrameBuffer);
	else Render::SetFrameBuffer(normalFrameBuffer);
	
	Render::DisablePolygonFillOffset();
	if (capabilities.extensionFlag[kExtensionDepthClamp]) Render::DisableDepthClamp();
	
	currentGraphicsState &= ~kGraphicsRenderShadowMap;
	renderTargetHeight = TheDisplayMgr->GetDisplayHeight();
	
	SetCamera(savedCameraObject, savedCameraTransformable, 0, false);
}

void GraphicsMgr::DrawShadowMapList(const List<Renderable> *renderList)
{
	#if !C4SERVER
	
		for (Renderable *renderable = renderList->First(); renderable; renderable = renderable->Next())
		{
			SetRenderState((renderable->GetRenderState() & kRenderDepthTest) | (kRenderColorInhibit | kRenderDepthOffset));
			
			const Transformable *transformable = renderable->GetTransformable();
			if (transformable) SetModelviewMatrix(cameraSpaceTransform * transformable->GetWorldTransform());
			else SetModelviewMatrix(cameraSpaceTransform);
			
			RenderSegment *segment = renderable->GetFirstRenderSegment();
			do
			{
				const ShaderData *shaderData = segment->GetShaderData(kShaderShadowMap, renderable->GetShaderDetailLevel());
				if (!shaderData) shaderData = segment->InitShaderData(renderable, kShaderShadowMap);
				if (!shaderData->programData[kShaderVariantNormal].vertexProgram) continue;
				
				SetMaterialState(nullptr, shaderData->materialState & kMaterialTwoSided);
				
				Render::SetVertexProgram(shaderData->programData[kShaderVariantNormal].vertexProgram);
				Render::SetFragmentProgram(shaderData->programData[kShaderVariantNormal].fragmentProgram);
				
				SetVertexArray(shaderData);
				SetPosition1Array(shaderData);
				SetOffsetArray(shaderData);
				SetColorArray(shaderData);
				SetAuxColorArray(shaderData, kShaderArrayColor1);
				SetAuxColorArray(shaderData, kShaderArrayColor2);
				for (machine a = kShaderArrayTexture0; a < kMaxShaderArrayCount; a++) SetTexcoordArray(shaderData, a);
				
				int32 stateFuncCount = shaderData->shaderStateDataCount;
				for (machine a = 0; a < stateFuncCount; a++) (*shaderData->shaderStateData[a].stateFunc)(renderable, shaderData->shaderStateData[a].stateCookie);
				
				int32 unitCount = shaderData->textureUnitCount;
				const Render::TextureObject *const *textureObject = shaderData->textureObject;
				for (machine unit = 0; unit < unitCount; unit++)
				{
					const Render::TextureObject *texture = textureObject[unit];
					if (texture) Render::BindTexture(unit, texture);
				}
				
				int32 vertexCount = renderable->GetVertexCount();
				graphicsCounter[kGraphicsCounterShadowVertices] += vertexCount;
				graphicsCounter[kGraphicsCounterShadowCommands]++;
				
				switch (renderable->GetRenderType())
				{
					case kRenderTriangles:
						
						graphicsCounter[kGraphicsCounterShadowPrimitives] += vertexCount / 3;
						Render::DrawArrays(Render::kPrimitiveTriangles, 0, vertexCount);
						break;
					
					case kRenderTriangleStrip:
						
						graphicsCounter[kGraphicsCounterShadowPrimitives] += vertexCount - 2;
						Render::DrawArrays(Render::kPrimitiveTriangleStrip, 0, vertexCount);
						break;
					
					case kRenderIndexedTriangles:
					{
						int32 triangleCount = segment->GetFaceCount();
						graphicsCounter[kGraphicsCounterShadowPrimitives] += triangleCount;
						
						const VertexBuffer *indexBuffer = shaderData->indexBuffer;
						if (indexBuffer)
						{
							Render::SetIndexVertexBuffer(indexBuffer);
							
							const unsigned_int16 *ptr = IndexArrayOffsetToPtr(renderable->GetFaceOffset()) + segment->GetFaceStart() * 3;
							Render::DrawElements(Render::kPrimitiveTriangles, vertexCount, triangleCount * 3, ptr);
						}
						else
						{
							Render::ResetIndexVertexBuffer();
							
							const unsigned_int16 *ptr = renderable->GetFaceArray() + segment->GetFaceStart() * 3;
							Render::DrawElements(Render::kPrimitiveTriangles, vertexCount, triangleCount * 3, ptr);
						}
						
						break;
					}
					
					case kRenderQuads:
						
						graphicsCounter[kGraphicsCounterShadowPrimitives] += vertexCount >> 2;
						Render::DrawArrays(Render::kPrimitiveQuads, 0, vertexCount);
						break;
					
					case kRenderMultiIndexedTriangles:
						
						Render::SetIndexVertexBuffer(shaderData->indexBuffer);
						Render::MultiDrawElements(Render::kPrimitiveTriangles, segment->GetMultiCountArray(), reinterpret_cast<const void *const *>(segment->GetMultiOffsetArray()), segment->GetMultiRenderCount());
						break;
					
					case kRenderMaskedMultiIndexedTriangles:
					{
						static const void		*index[33];
						static unsigned_int32	count[33];
						
						Render::SetIndexVertexBuffer(shaderData->indexBuffer);
						
						unsigned_int32 size = 0;
						const unsigned_int16 *ptr = IndexArrayOffsetToPtr(renderable->GetFaceOffset());
						
						int32 triangleCount = segment->GetFaceCount();
						if (triangleCount != 0)
						{
							graphicsCounter[kGraphicsCounterShadowPrimitives] += triangleCount;
							
							index[0] = ptr;
							count[0] = triangleCount * 3;
							size = 1;
						}
						
						unsigned_int32 mask = segment->GetMultiRenderMask();
						const int32 *data = segment->GetMultiRenderData();
						while (mask != 0)
						{
							if (mask & 1)
							{
								triangleCount = data[1];
								graphicsCounter[kGraphicsCounterShadowPrimitives] += triangleCount;
								
								index[size] = ptr + data[0] * 3;
								count[size] = triangleCount * 3;
								size++;
							}
							
							mask >>= 1;
							data += 2;
						}
						
						Render::MultiDrawElements(Render::kPrimitiveTriangles, count, index, size);
						break;
					}
				}
			} while ((segment = segment->GetNextRenderSegment()) != nullptr);
		}
	
	#endif
}

void GraphicsMgr::DrawWireframe(unsigned_int32 flags, const List<Renderable> *renderList)
{
	#if !C4SERVER
	
		unsigned_int32 state = (currentRenderState & kRenderDepthInhibit) | kRenderWireframe;
		
		if (flags & kWireframeDepthTest)
		{
			state |= kRenderDepthTest;
			Render::EnablePolygonLineOffset();
			Render::SetPolygonOffset(0.0F, -2.0F);
		}
		
		SetRenderState(state);
		SetBlendState(kBlendReplace);
		
		unsigned_int32 newMaterialState = currentMaterialState & kMaterialTwoSided;
		SetMaterialState(nullptr, newMaterialState);
		
		ResetArrayState();
		
		SetLocalFragmentProgram(kLocalProgramCopyLightColor);
		Render::SetFragmentProgramParameter4fv(kFragmentParamLightColor, &K::white.red);
		
		for (Renderable *renderable = renderList->First(); renderable; renderable = renderable->Next())
		{
			if (renderable->GetRenderType() >= kRenderTriangles)
			{
				const Transformable *transformable = renderable->GetTransformable();
				if (!(renderable->GetRenderableFlags() & kRenderableCameraTransformInhibit))
				{
					if (transformable) SetModelviewMatrix(cameraSpaceTransform * transformable->GetWorldTransform());
					else SetModelviewMatrix(cameraSpaceTransform);
				}
				else
				{
					if (transformable) SetModelviewMatrix(transformable->GetWorldTransform());
					else SetModelviewMatrix(K::identity_4D);
				}
				
				if (flags & kWireframeColor)
				{
					const ColorRGBA *wireColor = renderable->GetWireframeColorPointer();
					Render::SetFragmentProgramParameter4fv(kFragmentParamLightColor, (wireColor) ? &wireColor->red : &K::white.red);
				}
				
				RenderSegment *segment = renderable->GetFirstRenderSegment();
				do
				{
					const ShaderData *shaderData = segment->GetShaderData(kShaderShadowMap, renderable->GetShaderDetailLevel());
					if ((!shaderData) || ((shaderData->variantMask & (1 << kShaderVariantNormal)) == 0))
					{
						shaderData = segment->InitShaderData(renderable, kShaderShadowMap);
						SetLocalFragmentProgram(kLocalProgramCopyLightColor);
					}
					
					if (!shaderData->programData[kShaderVariantNormal].vertexProgram) continue;
					
					if ((shaderData->materialState & kMaterialTwoSided) || (flags & kWireframeTwoSided))
					{
						if (!(newMaterialState & kMaterialTwoSided))
						{
							newMaterialState |= kMaterialTwoSided;
							Render::DisableCullFace();
						}
					}
					else
					{
						if (newMaterialState & kMaterialTwoSided)
						{
							newMaterialState &= ~kMaterialTwoSided;
							Render::EnableCullFace();
						}
					}
					
					if ((shaderData->vertexBuffer[kShaderArrayPosition0]) || (shaderData->shaderArray[kShaderArrayPosition0]))
					{
						Render::SetVertexProgram(shaderData->programData[kShaderVariantNormal].vertexProgram);
						
						SetVertexArray(shaderData);
						SetPosition1Array(shaderData);
						SetNormalArray(shaderData);
						SetAuxColorArray(shaderData, kShaderArrayColor2);
						SetOffsetArray(shaderData);
						
						int32 stateFuncCount = shaderData->shaderStateDataCount;
						for (machine a = 0; a < stateFuncCount; a++) (*shaderData->shaderStateData[a].stateFunc)(renderable, shaderData->shaderStateData[a].stateCookie);
						
						int32 vertexCount = renderable->GetVertexCount();
						switch (renderable->GetRenderType())
						{
							case kRenderTriangles:
								
								Render::DrawArrays(Render::kPrimitiveTriangles, 0, vertexCount);
								break;
							
							case kRenderTriangleStrip:
								
								Render::DrawArrays(Render::kPrimitiveTriangleStrip, 0, vertexCount);
								break;
							
							case kRenderIndexedTriangles:
							{
								const VertexBuffer *indexBuffer = shaderData->indexBuffer;
								if (indexBuffer)
								{
									Render::SetIndexVertexBuffer(indexBuffer);
									
									const unsigned_int16 *ptr = IndexArrayOffsetToPtr(renderable->GetFaceOffset()) + segment->GetFaceStart() * 3;
									Render::DrawElements(Render::kPrimitiveTriangles, vertexCount, segment->GetFaceCount() * 3, ptr);
								}
								else
								{
									Render::ResetIndexVertexBuffer();
									
									const unsigned_int16 *ptr = renderable->GetFaceArray() + segment->GetFaceStart() * 3;
									Render::DrawElements(Render::kPrimitiveTriangles, vertexCount, segment->GetFaceCount() * 3, ptr);
								}
								
								break;
							}
							
							case kRenderQuads:
								
								Render::DrawArrays(Render::kPrimitiveQuads, 0, vertexCount);
								break;
							
							case kRenderMultiIndexedTriangles:
								
								Render::SetIndexVertexBuffer(shaderData->indexBuffer);
								Render::MultiDrawElements(Render::kPrimitiveTriangles, segment->GetMultiCountArray(), reinterpret_cast<const void *const *>(segment->GetMultiOffsetArray()), segment->GetMultiRenderCount());
								break;
							
							case kRenderMaskedMultiIndexedTriangles:
							{
								static const void		*index[33];
								static unsigned_int32	count[33];
								
								Render::SetIndexVertexBuffer(shaderData->indexBuffer);
								
								unsigned_int32 size = 0;
								const unsigned_int16 *ptr = IndexArrayOffsetToPtr(renderable->GetFaceOffset());
								
								int32 triangleCount = segment->GetFaceCount();
								if (triangleCount != 0)
								{
									index[0] = ptr;
									count[0] = triangleCount * 3;
									size = 1;
								}
								
								unsigned_int32 mask = segment->GetMultiRenderMask();
								const int32 *data = segment->GetMultiRenderData();
								while (mask != 0)
								{
									if (mask & 1)
									{
										triangleCount = data[1];
										
										index[size] = ptr + data[0] * 3;
										count[size] = triangleCount * 3;
										size++;
									}
									
									mask >>= 1;
									data += 2;
								}
								
								Render::MultiDrawElements(Render::kPrimitiveTriangles, count, index, size);
								break;
							}
						}
					}
				} while ((segment = segment->GetNextRenderSegment()) != nullptr);
			}
		}
		
		currentMaterialState = newMaterialState;
		if (flags & kWireframeDepthTest) Render::DisablePolygonLineOffset();
	
	#endif
}

void GraphicsMgr::DrawVectors(int32 array, const List<Renderable> *renderList)
{
	#if !C4SERVER
	
		static const VertexSnippet *drawVectorSnippets[2] =
		{
			&VertexProgram::modelviewProjectTransform,
			&VertexProgram::outputPrimaryColor
		};
		
		static Link<VertexProgram>	vectorVertexProgram;
		
		SetBlendState(kBlendReplace);
		SetRenderState(currentRenderState & kRenderDepthInhibit);
		SetMaterialState(nullptr, currentMaterialState & kMaterialTwoSided);
		
		Render::SetColor4ub(255, 255, 255, 255);
		SetLocalVertexProgram(&vectorVertexProgram, 2, drawVectorSnippets);
		
		SetLocalFragmentProgram(kLocalProgramCopyLightColor);
		Render::SetFragmentProgramParameter4fv(kFragmentParamLightColor, &K::white.red);
		
		Renderable *renderable = renderList->First();
		while (renderable)
		{
			if (renderable->GetRenderType() >= kRenderTriangles)
			{
				const Point3D *vtx = renderable->GetAttributeArray<Point3D>(kArrayVertex);
				const float *vec = renderable->GetAttributeArray(array);
				if ((vtx) && (vec))
				{
					int32 componentCount = renderable->GetComponentCount(array);
					if (componentCount >= 3)
					{
						const Transformable *transformable = renderable->GetTransformable();
						if (!(renderable->GetRenderableFlags() & kRenderableCameraTransformInhibit))
						{
							if (transformable) SetModelviewMatrix(cameraSpaceTransform * transformable->GetWorldTransform());
							else SetModelviewMatrix(cameraSpaceTransform);
						}
						else
						{
							if (transformable) SetModelviewMatrix(transformable->GetWorldTransform());
							else SetModelviewMatrix(K::identity_4D);
						}
						
						Render::Begin(Render::kPrimitiveLines);
						
						int32 vertexCount = renderable->GetVertexCount();
						for (machine a = 0; a < vertexCount; a++)
						{
							Render::SetVertex3fv(&vtx->x);
							
							float x = vec[0];
							float y = vec[1];
							float z = vec[2];
							float r = InverseSqrt(x * x + y * y + z * z) * 0.0625F;
							Render::SetVertex3f(vtx->x + x * r, vtx->y + y * r, vtx->z + z * r);
							
							vtx++;
							vec += componentCount;
						}
						
						Render::End();
					}
				}
			}
			
			renderable = renderable->Next();
		}
		
		Render::SetColor4ub(0, 0, 0, 0);
	
	#endif
}

void GraphicsMgr::ProcessOcclusionQueries(void)
{
	List<Renderable>	renderList;
	
	float normalizer = occlusionAreaNormalizer;
	if ((multisampleFrameBuffer) && (currentRenderTargetType == kRenderTargetPrimary)) normalizer *= multisampleFrameBuffer->GetSampleDivider();
	
	for (;;)
	{
		OcclusionQuery *query = occlusionQueryList.First();
		if (!query) break;
		
		unsigned_int32 sampleCount = Render::GetQuerySamplesPassed(query);
		if (sampleCount != 0)
		{
			query->unoccludedArea = (float) sampleCount * normalizer;
			(*query->renderProc)(query, &renderList, query->renderCookie);
		}
		
		OcclusionQuery::occlusionQueryList.Append(query);
	}
	
	if (!renderList.Empty())
	{
		DrawRenderList(&renderList);
		renderList.RemoveAll();
	}
}

void GraphicsMgr::ReadImageBuffer(const Rect& rect, Color4C *image, int32 rowPixels, const Integer2D& p)
{
	#if C4OPENGL
	
		image += p.y * rowPixels + p.x;
		glPixelStorei(GL_PACK_ROW_LENGTH, rowPixels);
		glReadPixels(rect.left, TheDisplayMgr->GetDisplayHeight() - rect.bottom, rect.Width(), rect.Height(), GL_RGBA, GL_UNSIGNED_BYTE, image);
	
	#endif
}

void GraphicsMgr::ReadDepthBuffer(const Rect& rect, unsigned_int16 *image, int32 rowPixels, const Integer2D& p)
{
	#if C4OPENGL
	
		image += p.y * rowPixels + p.x;
		glPixelStorei(GL_PACK_ROW_LENGTH, rowPixels);
		glReadPixels(rect.left, TheDisplayMgr->GetDisplayHeight() - rect.bottom, rect.Width(), rect.Height(), GL_DEPTH_COMPONENT, GL_UNSIGNED_SHORT, image);
	
	#endif
}

void GraphicsMgr::ReadDepthBuffer(const Rect& rect, unsigned_int32 *image, int32 rowPixels, const Integer2D& p)
{
	#if C4OPENGL
	
		image += p.y * rowPixels + p.x;
		glPixelStorei(GL_PACK_ROW_LENGTH, rowPixels);
		glReadPixels(rect.left, TheDisplayMgr->GetDisplayHeight() - rect.bottom, rect.Width(), rect.Height(), GL_DEPTH_COMPONENT, GL_UNSIGNED_INT, image);
	
	#endif
}

// ZYURVUR
