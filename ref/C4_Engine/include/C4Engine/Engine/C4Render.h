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


#ifndef C4Render_h
#define C4Render_h


#include "C4Packing.h"


#if C4OPENGL

	#include "C4OpenGL.h"

#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

#endif //]


namespace C4
{
	struct C4_API Render
	{
		enum
		{
			kMaxTextureUnitCount		= 16,
			kMaxFragmentParamCount		= 29
		};
		
		
		enum
		{
			kTextureTarget2D,
			kTextureTarget3D,
			kTextureTargetRectangle,
			kTextureTargetCube,
			kTextureTargetArray2D,
			kTextureTargetCount
		};
		
		
		enum
		{
			kTextureRGBX8,
			kTextureRGBA8,
			kTextureBGRX8,
			kTextureBGRA8,
			kTextureXRGB8,
			kTextureARGB8,
			kTextureL8,
			kTextureLA8,
			kTextureI8,
			kTextureI16,
			kTextureDepth16,
			kTextureDepth24,
			kTextureBC1,
			kTextureBC3,
			kTextureRenderBufferRGB8,
			kTextureRenderBufferRGBA8,
			kTextureRenderBufferRGBA16F,
			kTextureFormatCount
		};


		enum
		{
			kVertexBufferTargetAttribute,
			kVertexBufferTargetIndex
		};


		enum
		{
			kVertexBufferUsageStatic,
			kVertexBufferUsageDynamic
		};
		
		
		typedef void Decompressor(const unsigned_int8 *, unsigned_int32, void *);
		
		
		struct TextureImageData
		{
			const void		*image;
			unsigned_int32	size;
			Decompressor	*decompressor;
		};
		
		
		static void Initialize(void);
		static void Terminate(void);
		
		
		#if C4OPENGL || C4SERVER
		
			enum
			{
				kAlphaNever						= GL_NEVER,
				kAlphaLess						= GL_LESS,
				kAlphaEqual						= GL_EQUAL,
				kAlphaLessEqual					= GL_LEQUAL,
				kAlphaGreater					= GL_GREATER,
				kAlphaNotEqual					= GL_NOTEQUAL, 
				kAlphaGreaterEqual				= GL_GEQUAL,
				kAlphaAlways					= GL_ALWAYS 
			}; 
			 
			
			enum 
			{
				kDepthNever						= GL_NEVER,
				kDepthLess						= GL_LESS,
				kDepthEqual						= GL_EQUAL, 
				kDepthLessEqual					= GL_LEQUAL,
				kDepthGreater					= GL_GREATER,
				kDepthNotEqual					= GL_NOTEQUAL,
				kDepthGreaterEqual				= GL_GEQUAL, 
				kDepthAlways					= GL_ALWAYS
			};
			
			
			enum
			{
				kStencilNever					= GL_NEVER,
				kStencilLess					= GL_LESS,
				kStencilEqual					= GL_EQUAL,
				kStencilLessEqual				= GL_LEQUAL,
				kStencilGreater					= GL_GREATER,
				kStencilNotEqual				= GL_NOTEQUAL,
				kStencilGreaterEqual			= GL_GEQUAL,
				kStencilAlways					= GL_ALWAYS
			};
			
			
			enum
			{
				kStencilZero					= GL_ZERO,
				kStencilInvert					= GL_INVERT,
				kStencilKeep					= GL_KEEP,
				kStencilReplace					= GL_REPLACE,
				kStencilIncr					= GL_INCR,
				kStencilDecr					= GL_DECR,
				kStencilIncrWrap				= GL_INCR_WRAP,
				kStencilDecrWrap				= GL_DECR_WRAP
			};
			
			
			enum
			{
				kBlendZero						= GL_ZERO,
				kBlendOne						= GL_ONE,
				kBlendSrcColor					= GL_SRC_COLOR,
				kBlendInvSrcColor				= GL_ONE_MINUS_SRC_COLOR,
				kBlendSrcAlpha					= GL_SRC_ALPHA,
				kBlendInvSrcAlpha				= GL_ONE_MINUS_SRC_ALPHA,
				kBlendDstAlpha					= GL_DST_ALPHA,
				kBlendInvDstAlpha				= GL_ONE_MINUS_DST_ALPHA,
				kBlendDstColor					= GL_DST_COLOR,
				kBlendInvDstColor				= GL_ONE_MINUS_DST_COLOR,
				kBlendConstColor				= GL_CONSTANT_COLOR,
				kBlendInvConstColor				= GL_ONE_MINUS_CONSTANT_COLOR,
				kBlendConstAlpha				= GL_CONSTANT_ALPHA,
				kBlendInvConstAlpha				= GL_ONE_MINUS_CONSTANT_ALPHA
			};
			
			
			enum
			{
				kBlendEquationAdd				= GL_FUNC_ADD,
				kBlendEquationMin				= GL_MIN,
				kBlendEquationMax				= GL_MAX,
				kBlendEquationSubtract			= GL_FUNC_SUBTRACT,
				kBlendEquationReverseSubtract	= GL_FUNC_REVERSE_SUBTRACT
			};
			
			
			enum
			{
				kCullFront						= GL_FRONT,
				kCullBack						= GL_BACK,
				kCullFrontAndBack				= GL_FRONT_AND_BACK
			};
			
			
			enum
			{
				kFrontCW						= GL_CW,
				kFrontCCW						= GL_CCW
			};
			
			
			enum
			{
				kPolygonModePoint				= GL_POINT,
				kPolygonModeLine				= GL_LINE,
				kPolygonModeFill				= GL_FILL
			};
			
			
			enum
			{
				kShadowNever					= GL_NEVER,
				kShadowLess						= GL_LESS,
				kShadowEqual					= GL_EQUAL,
				kShadowLessEqual				= GL_LEQUAL,
				kShadowGreater					= GL_GREATER,
				kShadowNotEqual					= GL_NOTEQUAL,
				kShadowGreaterEqual				= GL_GEQUAL,
				kShadowAlways					= GL_ALWAYS
			};
			
			
			enum
			{
				kTextureCompareNone				= GL_NONE,
				kTextureCompareReference		= GL_COMPARE_REF_TO_TEXTURE
			};
			
			
			enum
			{
				kClearBufferColor				= GL_COLOR_BUFFER_BIT,
				kClearBufferDepth				= GL_DEPTH_BUFFER_BIT,
				kClearBufferStencil				= GL_STENCIL_BUFFER_BIT
			};
			
			
			enum
			{
				kPrimitivePoints				= GL_POINTS,
				kPrimitiveLines					= GL_LINES,
				kPrimitiveLineLoop				= GL_LINE_LOOP,
				kPrimitiveLineStrip				= GL_LINE_STRIP,
				kPrimitiveTriangles				= GL_TRIANGLES,
				kPrimitiveTriangleStrip			= GL_TRIANGLE_STRIP,
				kPrimitiveTriangleFan			= GL_TRIANGLE_FAN,
				kPrimitiveQuads					= GL_QUADS,
				kPrimitiveQuadStrip				= GL_QUAD_STRIP,
				kPrimitivePolygon				= GL_POLYGON
			};
			
			
			enum
			{
				kVertexFloat					= GL_FLOAT,
				kVertexUnsignedByte				= GL_UNSIGNED_BYTE,
				kVertexSignedShort				= GL_SHORT
			};
			
			
			enum
			{
				kRenderBufferRGBA8				= GL_RGBA8,
				kRenderBufferSRGBA8				= GL_SRGB8_ALPHA8,
				kRenderBufferRGBA16F			= GL_RGBA16F,
				kRenderBufferDepth				= GL_DEPTH_COMPONENT24,
				kRenderBufferDepthStencil		= GL_DEPTH24_STENCIL8
			};
			
			
			enum
			{
				kWrapRepeat						= GL_REPEAT,
				kWrapMirrorRepeat				= GL_MIRRORED_REPEAT,
				kWrapClampToEdge				= GL_CLAMP_TO_EDGE,
				kWrapClampToBorder				= GL_CLAMP_TO_BORDER,
				kWrapClamp						= GL_CLAMP,
				kWrapMirrorClampToEdge			= GL_MIRROR_CLAMP_TO_EDGE_EXT,
				kWrapMirrorClampToBorder		= GL_MIRROR_CLAMP_TO_BORDER_EXT,
				kWrapMirrorClamp				= GL_MIRROR_CLAMP_EXT
			};
			
			
			enum
			{
				kFilterNearest					= GL_NEAREST,
				kFilterLinear					= GL_LINEAR,
				kFilterNearestMipmapNearest		= GL_NEAREST_MIPMAP_NEAREST,
				kFilterLinearMipmapNearest		= GL_LINEAR_MIPMAP_NEAREST,
				kFilterNearestMipmapLinear		= GL_NEAREST_MIPMAP_LINEAR,
				kFilterLinearMipmapLinear		= GL_LINEAR_MIPMAP_LINEAR
			};
			
			
			enum
			{
				kQuerySamplesPassed				= GL_SAMPLES_PASSED,
				kQueryTimeElapsed				= GL_TIME_ELAPSED
			};
			
			
			enum
			{
				kBlitFilterPoint				= GL_NEAREST,
				kBlitFilterBilinear				= GL_LINEAR
			};
		
		#endif
		
		
		#if C4SERVER
		
			struct TextureObject
			{
				public:
					
					void Construct(unsigned_int32 index) {}
					void Destruct(void) {}
					
					unsigned_int32 GetTextureTargetIndex(void) const
					{
						return (kTextureTarget2D);
					}
					
					void Bind(unsigned_int32 unit) const {}
					void Unbind(unsigned_int32 unit) const {}
					void UnbindAll(void) const {}
					
					void SetSWrapMode(unsigned_int32 mode) {}
					void SetTWrapMode(unsigned_int32 mode) {}
					void SetRWrapMode(unsigned_int32 mode) {}
					void SetCompareFunc(unsigned_int32 func) {}
					void SetCompareMode(unsigned_int32 mode) {}
					void SetMinLod(unsigned_int32 lod) {}
					void SetMaxLod(unsigned_int32 lod) {}
					void SetLodBias(float bias) {}
					void SetMinFilterMode(unsigned_int32 mode) {}
					void SetMagFilterMode(unsigned_int32 mode) {}
					void SetMaxAnisotropy(float anisotropy) {}
					void SetBorderColor(const ColorRGBA& color) {}
					
					unsigned_int32 SetImage2D(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, int32 count, const TextureImageData *imageData) {return (0);}
					unsigned_int32 SetImage3D(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, unsigned_int32 depth, int32 count, const TextureImageData *imageData) {return (0);}
					unsigned_int32 SetImageCube(unsigned_int32 format, unsigned_int32 width, int32 count, const TextureImageData *imageData) {return (0);}
					unsigned_int32 SetImageRect(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, unsigned_int32 rowLength, const TextureImageData *imageData) {return (0);}
					unsigned_int32 SetImageArray2D(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, unsigned_int32 depth, int32 count, const TextureImageData *imageData) {return (0);}
					unsigned_int32 SetCompressedImage2D(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, int32 count, const TextureImageData *imageData) {return (0);}
					unsigned_int32 SetCompressedImageCube(unsigned_int32 format, unsigned_int32 width, int32 count, const TextureImageData *imageData) {return (0);}
					unsigned_int32 SetCompressedImageArray2D(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, unsigned_int32 depth, int32 count, const TextureImageData *imageData) {return (0);}
					
					unsigned_int32 AllocateStorage2D(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, bool renderBuffer) {return (0);}
					unsigned_int32 AllocateStorageRect(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, bool renderBuffer) {return (0);}
					
					void UpdateImage2D(unsigned_int32 x, unsigned_int32 y, unsigned_int32 width, unsigned_int32 height, unsigned_int32 rowLength, const void *image) const {}
					void UpdateImageRect(unsigned_int32 x, unsigned_int32 y, unsigned_int32 width, unsigned_int32 height, unsigned_int32 rowLength, const void *image) const {}
					
					void BlitImageRect(unsigned_int32 srcX, unsigned_int32 srcY, unsigned_int32 dstX, unsigned_int32 dstY, unsigned_int32 width, unsigned_int32 height) const {}
			};
			
			
			struct RenderBufferObject
			{
				public:
					
					void Construct(void) {}
					void Destruct(void) {}
					
					void AllocateStorage(unsigned_int32 width, unsigned_int32 height, unsigned_int32 format) {}
					void AllocateMultisampleStorage(unsigned_int32 width, unsigned_int32 height, unsigned_int32 sampleCount, unsigned_int32 format) {}
			};
			
			
			struct FrameBufferObject
			{
				public:
					
					void Construct(void) {}
					void Destruct(void) {}
					
					void SetColorRenderBuffer(const RenderBufferObject *renderBuffer) {}
					void SetDepthStencilRenderBuffer(const RenderBufferObject *renderBuffer) {}
					
					void SetColorRenderTexture(const TextureObject *renderTexture) {}
					void ResetColorRenderTexture(void) {}
					
					void SetDepthRenderTexture(const TextureObject *renderTexture) {}
					void ResetDepthRenderTexture(void) {}
			};
			
			
			struct VertexBufferObject
			{
				public:
					
					VertexBufferObject(unsigned_int32 target, unsigned_int32 usage)
					{
					}
					
					void Construct(void)
					{
					}
					
					void Destruct(void)
					{
					}
					
					bool AllocateStorage(unsigned_int32 size)
					{
						return (false);
					}
					
					void *BeginUpdate(void)
					{
						return (nullptr);
					}
					
					bool EndUpdate(void)
					{
						return (true);
					}
					
					void UpdateBuffer(unsigned_int32 offset, unsigned_int32 size, const void *data)
					{
					}
			};
			
			
			struct QueryObject
			{
				public:
					
					void Construct(void) {}
					void Destruct(void) {}
			};
			
			
			struct VertexProgramObject
			{
				public:
					
					void Construct(void) {}
					void Destruct(void) {}
					
					void SetSourceCode(const char *text, unsigned_int32 size) {}
			};
			
			
			struct FragmentProgramObject
			{
				public:
					
					void Construct(bool programFlag = true) {}
					void Destruct(void) {}
					
					void SetDeadFragmentTextureEnableMask(unsigned_int32 mask) {}
					void SetSourceCode(const char *text, unsigned_int32 size) {}
			};
			
			
			struct GeometryProgramObject
			{
				public:
					
					void Construct(bool programFlag = true) {}
					void Destruct(void) {}
					
					void SetSourceCode(const char *text, unsigned_int32 size) {}
			};
			
			
			struct RenderState
			{
			};
			
			
			private:
				
				static RenderState		renderState;
			
			public:
				
				static void BeginRendering(void) {}
				static void EndRendering(void) {}
				static void SetColorMask(bool r, bool g, bool b, bool a) {}
				static void EnableAlphaTest(void) {}
				static void DisableAlphaTest(void) {}
				static void SetAlphaFunc(unsigned_int32 func, float ref) {}
				static void EnableAlphaCoverage(void) {}
				static void DisableAlphaCoverage(void) {}
				static void EnableSampleShading(void) {}
				static void DisableSampleShading(void) {}
				static void SetMinSampleShading(float value) {}
				static void EnableDepthTest(void) {}
				static void DisableDepthTest(void) {}
				static void SetDepthFunc(unsigned_int32 func) {}
				static void SetDepthMask(bool mask) {}
				static void EnableDepthClamp(void) {}
				static void DisableDepthClamp(void) {}
				static void EnableDepthBoundsTest(void) {}
				static void DisableDepthBoundsTest(void) {}
				static void SetDepthBounds(float zmin, float zmax) {}
				static void EnableStencilTest(void) {}
				static void DisableStencilTest(void) {}
				static void SetStencilFunc(unsigned_int32 func, unsigned_int32 ref, unsigned_int32 mask) {}
				static void SetStencilOp(unsigned_int32 fail, unsigned_int32 zfail, unsigned_int32 zpass) {}
				static void SetFrontStencilOp(unsigned_int32 fail, unsigned_int32 zfail, unsigned_int32 zpass) {}
				static void SetBackStencilOp(unsigned_int32 fail, unsigned_int32 zfail, unsigned_int32 zpass) {}
				static void SetStencilMask(unsigned_int32 mask) {}
				static void EnableBlend(void) {}
				static void DisableBlend(void) {}
				static void SetBlendFunc(unsigned_int32 srcFunc, unsigned_int32 dstFunc) {}
				static void SetBlendFunc(unsigned_int32 srcRgbFunc, unsigned_int32 dstRgbFunc, unsigned_int32 srcAlphaFunc, unsigned_int32 dstAlphaFunc) {}
				static void SetBlendEquation(unsigned_int32 equation) {}
				static void SetBlendColor(float r, float g, float b, float a) {}
				static void EnableCullFace(void) {}
				static void DisableCullFace(void) {}
				static void SetCullFace(unsigned_int32 face) {}
				static void SetFrontFace(unsigned_int32 front) {}
				static void SetPolygonMode(unsigned_int32 mode) {}
				static void SetPointSize(float size) {}
				static void EnablePointSprite(void) {}
				static void DisablePointSprite(void) {}
				static void EnableLineSmooth(void) {}
				static void DisableLineSmooth(void) {}
				static void EnablePolygonLineOffset(void) {}
				static void DisablePolygonLineOffset(void) {}
				static void EnablePolygonFillOffset(void) {}
				static void DisablePolygonFillOffset(void) {}
				static void SetPolygonOffset(float slope, float bias) {}
				static void EnableFrameBufferSRGB(void) {}
				static void DisableFrameBufferSRGB(void) {}
				static void SetClearColor(float r, float g, float b, float a) {}
				static void Clear(unsigned_int32 mask) {}
				static void Begin(unsigned_int32 prim) {}
				static void End(void) {}
				static void SetVertex3f(float x, float y, float z) {}
				static void SetVertex3fv(const float *v) {}
				static void SetColor4ub(unsigned_int32 r, unsigned_int32 g, unsigned_int32 b, unsigned_int32 a) {}
				static void SetColor4fv(const float *v) {}
				static void EnableNormalArray(void) {}
				static void DisableNormalArray(void) {}
				static void EnableColorArray(unsigned_int32 color) {}
				static void DisableColorArray(unsigned_int32 color) {}
				static void EnableTexcoordArray(unsigned_int32 coord) {}
				static void DisableTexcoordArray(unsigned_int32 coord) {}
				static void EnableAttribArray(unsigned_int32 index) {}
				static void DisableAttribArray(unsigned_int32 index) {}
				static void SetVertexArray(int32 size, int32 type, int32 stride, const void *ptr) {}
				static void SetNormalArray(int32 size, int32 type, int32 stride, const void *ptr) {}
				static void SetColorArray(unsigned_int32 color, int32 size, int32 type, int32 stride, const void *ptr) {}
				static void SetTexcoordArray(unsigned_int32 coord, int32 size, int32 type, int32 stride, const void *ptr) {}
				static void SetAttribArray(unsigned_int32 index, int32 size, int32 type, int32 stride, const void *ptr) {}
				static void SetViewport(int32 x, int32 y, int32 width, int32 height) {}
				static void SetScissor(int32 x, int32 y, int32 width, int32 height) {}
				static void SetVertexProgramParameter4f(unsigned_int32 index, float x, float y, float z, float w) {}
				static void SetVertexProgramParameter4fv(unsigned_int32 index, const float *v) {}
				static void SetFragmentProgramParameter4f(unsigned_int32 index, float x, float y, float z, float w) {}
				static void SetFragmentProgramParameter4fv(unsigned_int32 index, const float *v) {}
				static void BindTexture(unsigned_int32 unit, const TextureObject *texture) {}
				static void ResetFrameBuffer(void) {}
				static void SetFrameBuffer(const FrameBufferObject *frameBuffer) {}
				static void SetDrawFrameBuffer(const FrameBufferObject *frameBuffer) {}
				static void SetReadFrameBuffer(const FrameBufferObject *frameBuffer) {}
				static void BlitFrameBuffer(unsigned_int32 srcX1, unsigned_int32 srcY1, unsigned_int32 srcX2, unsigned_int32 srcY2, unsigned_int32 dstX1, unsigned_int32 dstY1, unsigned_int32 dstX2, unsigned_int32 dstY2, unsigned_int32 filter) {}
				static void CopyFrameBuffer(unsigned_int32 srcX1, unsigned_int32 srcY1, unsigned_int32 srcX2, unsigned_int32 srcY2, unsigned_int32 dstX1, unsigned_int32 dstY1, unsigned_int32 dstX2, unsigned_int32 dstY2, unsigned_int32 filter) {}
				static void ResetAttributeVertexBuffer(void) {}
				static void SetAttributeVertexBuffer(const VertexBufferObject *vertexBuffer) {}
				static void ResetIndexVertexBuffer(void) {}
				static void SetIndexVertexBuffer(const VertexBufferObject *vertexBuffer) {}
				static void BeginQuery(QueryObject *query, unsigned_int32 type) {}
				static void EndQuery(QueryObject *query, unsigned_int32 type) {}
				static void BeginConditionalRender(QueryObject *query) {}
				static void EndConditionalRender(void) {}
				
				static unsigned_int32 GetQuerySamplesPassed(const QueryObject *query)
				{
					return (0);
				}
				
				static unsigned_int64 GetQueryTimeElapsed(const QueryObject *query)
				{
					return (0);
				}
				
				static void SetVertexProgram(const VertexProgramObject *vertexProgram) {}
				static void SetFragmentProgram(const FragmentProgramObject *fragmentProgram) {}
				static void SetGeometryProgram(const GeometryProgramObject *geometryProgram) {}
				static void BindFragmentShader(const FragmentProgramObject *fragmentProgram) {}
				static void UnbindFragmentShader(void) {}
				static void SetFragmentShaderTextureUnit(const FragmentProgramObject *fragmentProgram, const char *name, int32 unit) {}
				static void DrawArrays(unsigned_int32 prim, unsigned_int32 start, unsigned_int32 count) {}
				static void DrawElements(unsigned_int32 prim, unsigned_int32 range, unsigned_int32 count, const void *index) {}
				static void MultiDrawArrays(unsigned_int32 prim, const unsigned_int32 *startArray, const unsigned_int32 *countArray, unsigned_int32 size) {}
				static void MultiDrawElements(unsigned_int32 prim, const unsigned_int32 *countArray, const void *const *indexArray, unsigned_int32 size) {}
		
		#elif C4OPENGL
		
			struct TextureObject
			{
				private:
					
					unsigned_int16		targetIndex;
					unsigned_int16		openglTarget;
					unsigned_int32		formatIndex;
					GLuint				identifier;
				
				public:
					
					void Construct(unsigned_int32 index);
					void Destruct(void);
					
					unsigned_int32 GetTextureTargetIndex(void) const
					{
						return (targetIndex);
					}
					
					unsigned_int32 GetOpenGLTextureTarget(void) const
					{
						return (openglTarget);
					}
					
					unsigned_int32 GetTextureIdentifier(void) const
					{
						return (identifier);
					}
					
					void SetSWrapMode(unsigned_int32 mode)
					{
						glTextureParameteriEXT(identifier, openglTarget, GL_TEXTURE_WRAP_S, mode);
					}
					
					void SetTWrapMode(unsigned_int32 mode)
					{
						glTextureParameteriEXT(identifier, openglTarget, GL_TEXTURE_WRAP_T, mode);
					}
					
					void SetRWrapMode(unsigned_int32 mode)
					{
						glTextureParameteriEXT(identifier, openglTarget, GL_TEXTURE_WRAP_R, mode);
					}
					
					void SetCompareFunc(unsigned_int32 func)
					{
						glTextureParameteriEXT(identifier, openglTarget, GL_DEPTH_TEXTURE_MODE, GL_INTENSITY);
						glTextureParameteriEXT(identifier, openglTarget, GL_TEXTURE_COMPARE_FUNC, func);
					}
					
					void SetCompareMode(unsigned_int32 mode)
					{
						glTextureParameteriEXT(identifier, openglTarget, GL_TEXTURE_COMPARE_MODE, mode);
					}
					
					void SetMinLod(unsigned_int32 lod)
					{
						glTextureParameteriEXT(identifier, openglTarget, GL_TEXTURE_BASE_LEVEL, lod);
					}
					
					void SetMaxLod(unsigned_int32 lod)
					{
						glTextureParameteriEXT(identifier, openglTarget, GL_TEXTURE_MAX_LEVEL, lod);
					}
					
					void SetLodBias(float bias)
					{
						glTextureParameterfEXT(identifier, openglTarget, GL_TEXTURE_LOD_BIAS, bias);
					}
					
					void SetMinFilterMode(unsigned_int32 mode)
					{
						glTextureParameteriEXT(identifier, openglTarget, GL_TEXTURE_MIN_FILTER, mode);
					}
					
					void SetMagFilterMode(unsigned_int32 mode)
					{
						glTextureParameteriEXT(identifier, openglTarget, GL_TEXTURE_MAG_FILTER, mode);
					}
					
					void SetMaxAnisotropy(float anisotropy)
					{
						glTextureParameterfEXT(identifier, openglTarget, GL_TEXTURE_MAX_ANISOTROPY_EXT, anisotropy);
					}
					
					void SetBorderColor(const ColorRGBA& color)
					{
						glTextureParameterfvEXT(identifier, openglTarget, GL_TEXTURE_BORDER_COLOR, &color.red);
					}
					
					void Bind(unsigned_int32 unit) const;
					void Unbind(unsigned_int32 unit) const;
					void UnbindAll(void) const;
					
					unsigned_int32 SetImage2D(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, int32 count, const TextureImageData *imageData);
					unsigned_int32 SetImage3D(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, unsigned_int32 depth, int32 count, const TextureImageData *imageData);
					unsigned_int32 SetImageCube(unsigned_int32 format, unsigned_int32 width, int32 count, const TextureImageData *imageData);
					unsigned_int32 SetImageRect(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, unsigned_int32 rowLength, const TextureImageData *imageData);
					unsigned_int32 SetImageArray2D(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, unsigned_int32 depth, int32 count, const TextureImageData *imageData);
					unsigned_int32 SetCompressedImage2D(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, int32 count, const TextureImageData *imageData);
					unsigned_int32 SetCompressedImageCube(unsigned_int32 format, unsigned_int32 width, int32 count, const TextureImageData *imageData);
					unsigned_int32 SetCompressedImageArray2D(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, unsigned_int32 depth, int32 count, const TextureImageData *imageData);
					
					unsigned_int32 AllocateStorage2D(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, bool renderBuffer);
					unsigned_int32 AllocateStorageRect(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, bool renderBuffer);
					
					void UpdateImage2D(unsigned_int32 x, unsigned_int32 y, unsigned_int32 width, unsigned_int32 height, unsigned_int32 rowLength, const void *image) const;
					void UpdateImageRect(unsigned_int32 x, unsigned_int32 y, unsigned_int32 width, unsigned_int32 height, unsigned_int32 rowLength, const void *image) const;
					
					void BlitImageRect(unsigned_int32 srcX, unsigned_int32 srcY, unsigned_int32 dstX, unsigned_int32 dstY, unsigned_int32 width, unsigned_int32 height) const
					{
						glCopyTextureSubImage2DEXT(identifier, GL_TEXTURE_RECTANGLE, 0, dstX, dstY, srcX, srcY, width, height);
					}
			};
			
			
			struct RenderBufferObject
			{
				private:
					
					GLuint				identifier;
				
				public:
					
					void Construct(void);
					void Destruct(void);
					
					unsigned_int32 GetRenderBufferIdentifier(void) const
					{
						return (identifier);
					}
					
					void AllocateStorage(unsigned_int32 width, unsigned_int32 height, unsigned_int32 format);
					void AllocateMultisampleStorage(unsigned_int32 width, unsigned_int32 height, unsigned_int32 sampleCount, unsigned_int32 format);
			};
			
			
			struct C4_API FrameBufferObject
			{
				friend struct Render;
				
				private:
					
					GLuint					identifier;
					
					const TextureObject		*currentColorTexture;
					const TextureObject		*currentDepthTexture;
					
					void Bind(void) const
					{
						if (renderState.drawFrameBuffer != this)
						{
							renderState.drawFrameBuffer = this;
							renderState.readFrameBuffer = this;
							glBindFramebuffer(GL_FRAMEBUFFER, identifier);
						}
					}
				
				public:
					
					void Construct(void);
					void Destruct(void);
					
					unsigned_int32 GetFrameBufferIdentifier(void) const
					{
						return (identifier);
					}
					
					void SetColorRenderBuffer(const RenderBufferObject *renderBuffer);
					void SetDepthStencilRenderBuffer(const RenderBufferObject *renderBuffer);
					
					void SetColorRenderTexture(const TextureObject *renderTexture);
					void ResetColorRenderTexture(void);
					
					void SetDepthRenderTexture(const TextureObject *renderTexture);
					void ResetDepthRenderTexture(void);
			};
			
			
			struct VertexBufferObject
			{
				friend struct Render;
				
				private:
					
					unsigned_int16		bufferTarget;
					unsigned_int16		bufferUsage;
					
					unsigned_int16		openglTarget;
					unsigned_int16		openglUsage;
					GLuint				identifier;
					
					void BindAttributeBuffer(void) const
					{
						if (renderState.attributeVertexBuffer != identifier)
						{
							renderState.attributeVertexBuffer = identifier;
							glBindBuffer(GL_ARRAY_BUFFER, identifier);
						}
					}
					
					void BindIndexBuffer(void) const
					{
						if (renderState.indexVertexBuffer != identifier)
						{
							renderState.indexVertexBuffer = identifier;
							glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, identifier);
						}
					}
					
					void Bind(void) const
					{
						if (bufferTarget == kVertexBufferTargetAttribute) BindAttributeBuffer();
						else BindIndexBuffer();
					}
					
					void Unbind(void) const
					{
						if (bufferTarget == kVertexBufferTargetAttribute)
						{
							renderState.attributeVertexBuffer = 0;
							glBindBuffer(GL_ARRAY_BUFFER, 0);
						}
						else
						{
							renderState.indexVertexBuffer = 0;
							glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
						}
					}
				
				public:
					
					VertexBufferObject(unsigned_int32 target, unsigned_int32 usage)
					{
						bufferTarget = (unsigned_int16) target;
						bufferUsage = (unsigned_int16) usage;
					}
					
					void Construct(void);
					void Destruct(void);
					
					unsigned_int32 GetVertexBufferIdentifier(void) const
					{
						return (identifier);
					}
					
					void *BeginUpdate(void)
					{
						return (glMapNamedBufferEXT(identifier, GL_WRITE_ONLY));
					}

					bool EndUpdate(void)
					{
						return (glUnmapNamedBufferEXT(identifier));
					}
					
					void UpdateBuffer(unsigned_int32 offset, unsigned_int32 size, const void *data)
					{
						glNamedBufferSubDataEXT(identifier, offset, size, data);
					}
					
					bool AllocateStorage(unsigned_int32 size);
			};
			
			
			struct QueryObject
			{
				private:
					
					GLuint		identifier;
				
				public:
					
					void Construct(void);
					void Destruct(void);
					
					unsigned_int32 GetQueryIdentifier(void) const
					{
						return (identifier);
					}
			};
			
			
			struct VertexProgramObject
			{
				private:
					
					GLuint		identifier;
				
				public:
					
					void Construct(void);
					void Destruct(void);
					
					unsigned_int32 GetVertexProgramIdentifier(void) const
					{
						return (identifier);
					}
					
					void SetSourceCode(const char *text, unsigned_int32 size);
			};
			
			
			struct FragmentProgramObject
			{
				private:
					
					GLuint		identifier;
					GLhandle	programHandle;
					GLhandle	shaderHandle;
					
					int32		uniformLocation;
				
				public:
					
					void Construct(bool programFlag = true);
					void Destruct(void);
					
					unsigned_int32 GetFragmentProgramIdentifier(void) const
					{
						return (identifier);
					}
					
					GLhandle GetFragmentShaderHandle(void) const
					{
						return (programHandle);
					}
					
					int32 GetUniformLocation(void) const
					{
						return (uniformLocation);
					}
					
					int32 QueryUniformLocation(const char *name) const
					{
						return (glGetUniformLocation(programHandle, name));
					}
					
					void SetDeadFragmentTextureEnableMask(unsigned_int32 mask)
					{
					}
					
					void SetSourceCode(const char *text, unsigned_int32 size);
			};
			
			
			struct GeometryProgramObject
			{
				private:
					
					GLuint		identifier;
				
				public:
					
					void Construct(void);
					void Destruct(void);
					
					unsigned_int32 GetGeometryProgramIdentifier(void) const
					{
						return (identifier);
					}
					
					void SetSourceCode(const char *text, unsigned_int32 size);
			};
			
			
			struct RenderState
			{
				unsigned_int32					imageUnit;
				GLuint							texture[kMaxTextureUnitCount][kTextureTargetCount];
				
				const FrameBufferObject			*drawFrameBuffer;
				const FrameBufferObject			*readFrameBuffer;
				
				GLuint							attributeVertexBuffer;
				GLuint							indexVertexBuffer;
				
				const VertexProgramObject		*vertexProgram;
				const FragmentProgramObject		*fragmentProgram;
				const FragmentProgramObject		*fragmentShader;
				const GeometryProgramObject		*geometryProgram;
				
				unsigned_int32					updateFlags;
				
				Vector4D						fragmentProgramParam[kMaxFragmentParamCount];
			};
			
			
			private:
				
				static RenderState		renderState;
				
				static void UpdateFragmentProgramParameters(void);
				static void BindTextureUnit0(GLuint texture, GLenum target);
			
			public:
				
				static void BeginRendering(void)
				{
				}
				
				static void EndRendering(void)
				{
				}
				
				static void SetColorMask(bool r, bool g, bool b, bool a)
				{
					glColorMask(r, g, b, a);
				}
				
				static void EnableAlphaTest(void)
				{
					glEnable(GL_ALPHA_TEST);
				}
				
				static void DisableAlphaTest(void)
				{
					glDisable(GL_ALPHA_TEST);
				}
				
				static void SetAlphaFunc(unsigned_int32 func, float ref)
				{
					glAlphaFunc(func, ref);
				}
				
				static void EnableAlphaCoverage(void)
				{
					glEnable(GL_SAMPLE_ALPHA_TO_COVERAGE);
				}
				
				static void DisableAlphaCoverage(void)
				{
					glDisable(GL_SAMPLE_ALPHA_TO_COVERAGE);
				}
				
				static void EnableSampleShading(void)
				{
					glEnable(GL_SAMPLE_SHADING);
				}
				
				static void DisableSampleShading(void)
				{
					glDisable(GL_SAMPLE_SHADING);
				}
				
				static void SetMinSampleShading(float value)
				{
					glMinSampleShading(value);
				}
				
				static void EnableDepthTest(void)
				{
					glEnable(GL_DEPTH_TEST);
				}
				
				static void DisableDepthTest(void)
				{
					glDisable(GL_DEPTH_TEST);
				}
				
				static void SetDepthFunc(unsigned_int32 func)
				{
					glDepthFunc(func);
				}
				
				static void SetDepthMask(bool mask)
				{
					glDepthMask(mask);
				}
				
				static void EnableDepthClamp(void)
				{
					glEnable(GL_DEPTH_CLAMP);
				}
				
				static void DisableDepthClamp(void)
				{
					glDisable(GL_DEPTH_CLAMP);
				}
				
				static void EnableDepthBoundsTest(void)
				{
					glEnable(GL_DEPTH_BOUNDS_TEST_EXT);
				}
				
				static void DisableDepthBoundsTest(void)
				{
					glDisable(GL_DEPTH_BOUNDS_TEST_EXT);
				}
				
				static void SetDepthBounds(float zmin, float zmax)
				{
					glDepthBoundsEXT(zmin, zmax);
				}
				
				static void EnableStencilTest(void)
				{
					glEnable(GL_STENCIL_TEST);
				}
				
				static void DisableStencilTest(void)
				{
					glDisable(GL_STENCIL_TEST);
				}
				
				static void SetStencilFunc(unsigned_int32 func, unsigned_int32 ref, unsigned_int32 mask)
				{
					glStencilFunc(func, ref, mask);
				}
				
				static void SetStencilOp(unsigned_int32 fail, unsigned_int32 zfail, unsigned_int32 zpass)
				{
					glStencilOp(fail, zfail, zpass);
				}
				
				static void SetFrontStencilOp(unsigned_int32 fail, unsigned_int32 zfail, unsigned_int32 zpass)
				{
					glStencilOpSeparate(GL_FRONT, fail, zfail, zpass);
				}
				
				static void SetBackStencilOp(unsigned_int32 fail, unsigned_int32 zfail, unsigned_int32 zpass)
				{
					glStencilOpSeparate(GL_BACK, fail, zfail, zpass);
				}
				
				static void SetStencilMask(unsigned_int32 mask)
				{
					glStencilMask(mask);
				}
				
				static void EnableBlend(void)
				{
					glEnable(GL_BLEND);
				}
				
				static void DisableBlend(void)
				{
					glDisable(GL_BLEND);
				}
				
				static void SetBlendFunc(unsigned_int32 srcFunc, unsigned_int32 dstFunc)
				{
					glBlendFunc(srcFunc, dstFunc);
				}
				
				static void SetBlendFunc(unsigned_int32 srcRgbFunc, unsigned_int32 dstRgbFunc, unsigned_int32 srcAlphaFunc, unsigned_int32 dstAlphaFunc)
				{
					glBlendFuncSeparate(srcRgbFunc, dstRgbFunc, srcAlphaFunc, dstAlphaFunc);
				}
				
				static void SetBlendEquation(unsigned_int32 equation)
				{
					glBlendEquation(equation);
				}
				
				static void SetBlendColor(float r, float g, float b, float a)
				{
					glBlendColor(r, g, b, a);
				}
				
				static void EnableCullFace(void)
				{
					glEnable(GL_CULL_FACE);
				}
				
				static void DisableCullFace(void)
				{
					glDisable(GL_CULL_FACE);
				}
				
				static void SetCullFace(unsigned_int32 face)
				{
					glCullFace(face);
				}
				
				static void SetFrontFace(unsigned_int32 front)
				{
					glFrontFace(front);
				}
				
				static void SetPolygonMode(unsigned_int32 mode)
				{
					glPolygonMode(GL_FRONT_AND_BACK, mode);
				}
				
				static void SetPointSize(float size)
				{
					glPointSize(size);
				}
				
				static void EnablePointSprite(void)
				{
					glEnable(GL_POINT_SPRITE);
					glEnable(GL_VERTEX_PROGRAM_POINT_SIZE);
				}
				
				static void DisablePointSprite(void)
				{
					glDisable(GL_POINT_SPRITE);
					glDisable(GL_VERTEX_PROGRAM_POINT_SIZE);
				}
				
				static void EnableLineSmooth(void)
				{
					glEnable(GL_LINE_SMOOTH);
				}
				
				static void DisableLineSmooth(void)
				{
					glDisable(GL_LINE_SMOOTH);
				}
				
				static void EnablePolygonLineOffset(void)
				{
					glEnable(GL_POLYGON_OFFSET_LINE);
				}
				
				static void DisablePolygonLineOffset(void)
				{
					glDisable(GL_POLYGON_OFFSET_LINE);
				}
				
				static void EnablePolygonFillOffset(void)
				{
					glEnable(GL_POLYGON_OFFSET_FILL);
				}
				
				static void DisablePolygonFillOffset(void)
				{
					glDisable(GL_POLYGON_OFFSET_FILL);
				}
				
				static void SetPolygonOffset(float slope, float bias)
				{
					glPolygonOffset(slope, bias);
				}
				
				static void EnableFrameBufferSRGB(void)
				{
					glEnable(GL_FRAMEBUFFER_SRGB);
				}
				
				static void DisableFrameBufferSRGB(void)
				{
					glDisable(GL_FRAMEBUFFER_SRGB);
				}
				
				static void SetClearColor(float r, float g, float b, float a)
				{
					glClearColor(r, g, b, a);
				}
				
				static void Clear(unsigned_int32 mask)
				{
					glClear(mask);
				}
				
				static void Begin(unsigned_int32 prim)
				{
					glBegin(prim);
				}
				
				static void End(void)
				{
					glEnd();
				}
				
				static void SetVertex3f(float x, float y, float z)
				{
					glVertexAttrib3fARB(0, x, y, z);
				}
				
				static void SetVertex3fv(const float *v)
				{
					glVertexAttrib3fvARB(0, v);
				}
				
				static void SetColor4ub(unsigned_int32 r, unsigned_int32 g, unsigned_int32 b, unsigned_int32 a)
				{
					glVertexAttrib4NubARB(3, r, g, b, a);
				}
				
				static void SetColor4fv(const float *v)
				{
					glVertexAttrib4fvARB(3, v);
				}
				
				static void EnableNormalArray(void)
				{
					glEnableVertexAttribArrayARB(2);
				}
				
				static void DisableNormalArray(void)
				{
					glDisableVertexAttribArrayARB(2);
				}
				
				static void EnableColorArray(unsigned_int32 color)
				{
					glEnableVertexAttribArrayARB(color + 3);
				}
				
				static void DisableColorArray(unsigned_int32 color)
				{
					glDisableVertexAttribArrayARB(color + 3);
				}
				
				static void EnableTexcoordArray(unsigned_int32 coord)
				{
					glEnableVertexAttribArrayARB(coord + 8);
				}
				
				static void DisableTexcoordArray(unsigned_int32 coord)
				{
					glDisableVertexAttribArrayARB(coord + 8);
				}
				
				static void EnableAttribArray(unsigned_int32 index)
				{
					glEnableVertexAttribArrayARB(index);
				}
				
				static void DisableAttribArray(unsigned_int32 index)
				{
					glDisableVertexAttribArrayARB(index);
				}
				
				static void SetVertexArray(int32 size, int32 type, int32 stride, const void *ptr)
				{
					glVertexAttribPointerARB(0, size, type, false, stride, ptr);
				}
				
				static void SetNormalArray(int32 size, int32 type, int32 stride, const void *ptr)
				{
					glVertexAttribPointerARB(2, size, type, false, stride, ptr);
				}
				
				static void SetColorArray(unsigned_int32 color, int32 size, int32 type, int32 stride, const void *ptr)
				{
					glVertexAttribPointerARB(color + 3, size, type, true, stride, ptr);
				}
				
				static void SetTexcoordArray(unsigned_int32 coord, int32 size, int32 type, int32 stride, const void *ptr)
				{
					glVertexAttribPointerARB(coord + 8, size, type, false, stride, ptr);
				}
				
				static void SetAttribArray(unsigned_int32 index, int32 size, int32 type, int32 stride, const void *ptr)
				{
					glVertexAttribPointerARB(index, size, type, false, stride, ptr);
				}
				
				static void SetViewport(int32 x, int32 y, int32 width, int32 height)
				{
					glViewport(x, y, width, height);
				}
				
				static void SetScissor(int32 x, int32 y, int32 width, int32 height)
				{
					glScissor(x, y, width, height);
				}
				
				static void SetVertexProgramParameter4f(unsigned_int32 index, float x, float y, float z, float w)
				{
					glProgramEnvParameter4fARB(GL_VERTEX_PROGRAM_ARB, index, x, y, z, w);
				}
				
				static void SetVertexProgramParameter4fv(unsigned_int32 index, const float *v)
				{
					glProgramEnvParameter4fvARB(GL_VERTEX_PROGRAM_ARB, index, v);
				}
				
				static void SetFragmentProgramParameter4f(unsigned_int32 index, float x, float y, float z, float w);
				static void SetFragmentProgramParameter4fv(unsigned_int32 index, const float *v);
				
				static void BindTexture(unsigned_int32 unit, const TextureObject *texture)
				{
					texture->Bind(unit);
				}
				
				static void ResetFrameBuffer(void)
				{
					renderState.drawFrameBuffer = nullptr;
					renderState.readFrameBuffer = nullptr;
					glBindFramebuffer(GL_FRAMEBUFFER, 0);
				}
				
				static void SetFrameBuffer(const FrameBufferObject *frameBuffer)
				{
					frameBuffer->Bind();
				}
				
				static void SetDrawFrameBuffer(const FrameBufferObject *frameBuffer)
				{
					if (renderState.drawFrameBuffer != frameBuffer)
					{
						renderState.drawFrameBuffer = frameBuffer;
						glBindFramebuffer(GL_DRAW_FRAMEBUFFER, frameBuffer->GetFrameBufferIdentifier());
					}
				}
				
				static void SetReadFrameBuffer(const FrameBufferObject *frameBuffer)
				{
					if (renderState.readFrameBuffer != frameBuffer)
					{
						renderState.readFrameBuffer = frameBuffer;
						glBindFramebuffer(GL_READ_FRAMEBUFFER, frameBuffer->GetFrameBufferIdentifier());
					}
				}
				
				static void BlitFrameBuffer(unsigned_int32 srcX1, unsigned_int32 srcY1, unsigned_int32 srcX2, unsigned_int32 srcY2, unsigned_int32 dstX1, unsigned_int32 dstY1, unsigned_int32 dstX2, unsigned_int32 dstY2, unsigned_int32 filter)
				{
					glBlitFramebuffer(srcX1, srcY1, srcX2, srcY2, dstX1, dstY1, dstX2, dstY2, GL_COLOR_BUFFER_BIT, filter);
				}
				
				static void CopyFrameBuffer(unsigned_int32 srcX1, unsigned_int32 srcY1, unsigned_int32 srcX2, unsigned_int32 srcY2, unsigned_int32 dstX1, unsigned_int32 dstY1, unsigned_int32 dstX2, unsigned_int32 dstY2, unsigned_int32 filter)
				{
					glBlitFramebuffer(srcX1, srcY1, srcX2, srcY2, dstX1, dstY1, dstX2, dstY2, GL_COLOR_BUFFER_BIT, filter);
				}
				
				static void ResetAttributeVertexBuffer(void)
				{
					if (renderState.attributeVertexBuffer != 0)
					{
						renderState.attributeVertexBuffer = 0;
						glBindBuffer(GL_ARRAY_BUFFER, 0);
					}
				}
				
				static void SetAttributeVertexBuffer(const VertexBufferObject *vertexBuffer)
				{
					vertexBuffer->BindAttributeBuffer();
				}
				
				static void ResetIndexVertexBuffer(void)
				{
					if (renderState.indexVertexBuffer != 0)
					{
						renderState.indexVertexBuffer = 0;
						glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
					}
				}
				
				static void SetIndexVertexBuffer(const VertexBufferObject *vertexBuffer)
				{
					vertexBuffer->BindIndexBuffer();
				}
				
				static void BeginQuery(QueryObject *query, unsigned_int32 type)
				{
					glBeginQuery(type, query->GetQueryIdentifier());
				}
				
				static void EndQuery(QueryObject *query, unsigned_int32 type)
				{
					glEndQuery(type);
				}
				
				static void BeginConditionalRender(QueryObject *query)
				{
					glBeginConditionalRender(query->GetQueryIdentifier(), GL_QUERY_BY_REGION_NO_WAIT);
				}
				
				static void EndConditionalRender(void)
				{
					glEndConditionalRender();
				}
				
				static unsigned_int32 GetQuerySamplesPassed(const QueryObject *query)
				{
					GLuint			result;
					
					glGetQueryObjectuiv(query->GetQueryIdentifier(), GL_QUERY_RESULT, &result);
					return (result);
				}
				
				static unsigned_int64 GetQueryTimeElapsed(const QueryObject *query)
				{
					GLuint64		result;
					
					glGetQueryObjectui64v(query->GetQueryIdentifier(), GL_QUERY_RESULT, &result);
					return (result);
				}
				
				static void SetVertexProgram(const VertexProgramObject *vertexProgram);
				static void SetFragmentProgram(const FragmentProgramObject *fragmentProgram);
				static void SetGeometryProgram(const GeometryProgramObject *geometryProgram);
				
				static void BindFragmentShader(const FragmentProgramObject *fragmentProgram)
				{
					glUseProgramObject(fragmentProgram->GetFragmentShaderHandle());
				}
				
				static void UnbindFragmentShader(void)
				{
					renderState.fragmentShader = nullptr;
					glUseProgramObject(0);
				}
				
				static void SetFragmentShaderTextureUnit(const FragmentProgramObject *fragmentProgram, const char *name, int32 unit)
				{
					glUniform1i(fragmentProgram->QueryUniformLocation(name), unit);
				}
				
				static void DrawArrays(unsigned_int32 prim, unsigned_int32 start, unsigned_int32 count)
				{
					UpdateFragmentProgramParameters();
					glDrawArrays(prim, start, count);
				}
				
				static void DrawElements(unsigned_int32 prim, unsigned_int32 range, unsigned_int32 count, const void *index)
				{
					UpdateFragmentProgramParameters();
					glDrawRangeElements(prim, 0, range - 1, count, GL_UNSIGNED_SHORT, index);
				}
				
				static void MultiDrawArrays(unsigned_int32 prim, const unsigned_int32 *startArray, const unsigned_int32 *countArray, unsigned_int32 size)
				{
					UpdateFragmentProgramParameters();
					glMultiDrawArrays(prim, reinterpret_cast<const GLint *>(startArray), reinterpret_cast<const GLsizei *>(countArray), size);
				}
				
				static void MultiDrawElements(unsigned_int32 prim, const unsigned_int32 *countArray, const void *const *indexArray, unsigned_int32 size)
				{
					UpdateFragmentProgramParameters();
					glMultiDrawElements(prim, reinterpret_cast<const GLsizei *>(countArray), GL_UNSIGNED_SHORT, indexArray, size);
				}
				
				// If GL_EXT_direct_state_access is not available, then the DSA functions used by the engine
				// get remapped to the following functions in the Render namespace.

				#if C4WINDOWS
				
					#define OPENGLCALL APIENTRY
				
				#else
				
					#define OPENGLCALL
				
				#endif
				
				static void OPENGLCALL BindMultiTexture(GLenum texunit, GLenum target, GLuint texture);
				static void OPENGLCALL TextureParameteri(GLuint texture, GLenum target, GLenum pname, GLint param);
				static void OPENGLCALL TextureParameteriv(GLuint texture, GLenum target, GLenum pname, const GLint *param);
				static void OPENGLCALL TextureParameterf(GLuint texture, GLenum target, GLenum pname, GLfloat param);
				static void OPENGLCALL TextureParameterfv(GLuint texture, GLenum target, GLenum pname, const GLfloat *param);
				static void OPENGLCALL MultiTexParameteri(GLenum texunit, GLenum target, GLenum pname, GLint param);
				static void OPENGLCALL MultiTexParameteriv(GLenum texunit, GLenum target, GLenum pname, const GLint *param);
				static void OPENGLCALL MultiTexParameterf(GLenum texunit, GLenum target, GLenum pname, GLfloat param);
				static void OPENGLCALL MultiTexParameterfv(GLenum texunit, GLenum target, GLenum pname, const GLfloat *param);
				static void OPENGLCALL TextureImage2D(GLuint texture, GLenum target, GLint level, GLint internalformat, GLsizei width, GLsizei height, GLint border, GLenum format, GLenum type, const void *pixels);
				static void OPENGLCALL TextureSubImage2D(GLuint texture, GLenum target, GLint level, GLint xoffset, GLint yoffset, GLsizei width, GLsizei height, GLenum format, GLenum type, const void *pixels);
				static void OPENGLCALL TextureImage3D(GLuint texture, GLenum target, GLint level, GLint internalformat, GLsizei width, GLsizei height, GLsizei depth, GLint border, GLenum format, GLenum type, const void *pixels);
				static void OPENGLCALL CompressedTextureImage2D(GLuint texture, GLenum target, GLint level, GLenum internalformat, GLsizei width, GLsizei height, GLint border, GLsizei imageSize, const void *data);
				static void OPENGLCALL CompressedTextureImage3D(GLuint texture, GLenum target, GLint level, GLenum internalformat, GLsizei width, GLsizei height, GLsizei depth, GLint border, GLsizei imageSize, const void *data);
				static void OPENGLCALL CopyTextureSubImage2D(GLuint texture, GLenum target, GLint level, GLint xoffset, GLint yoffset, GLint x, GLint y, GLsizei width, GLsizei height);
				static void OPENGLCALL NamedBufferData(GLuint buffer, GLsizeiptr size, const void *data, GLenum usage);
				static void OPENGLCALL NamedBufferSubData(GLuint buffer, GLintptr offset, GLsizeiptr size, const void *data);
				static void *OPENGLCALL MapNamedBuffer(GLuint buffer, GLenum access);
				static GLboolean OPENGLCALL UnmapNamedBuffer(GLuint buffer);
		
		#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

		#endif //]
	};
}


#endif

// ZYURVUR
