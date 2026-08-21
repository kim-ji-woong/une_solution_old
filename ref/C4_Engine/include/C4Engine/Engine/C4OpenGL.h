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


#ifndef C4OpenGL_h
#define C4OpenGL_h


#include "C4Memory.h"


namespace C4
{
	#if C4WINDOWS
	
		#define OPENGLAPI APIENTRY
	
	#else
	
		#define OPENGLAPI
	
	#endif
	
	
	#ifdef C4OpenGL_cpp
	
		#if C4WINDOWS || C4LINUX
			
			void *GetCoreFuncAddress(bool core, const char *coreName, const char *extName);
			
			#define GLCOREFUNC(type, name, params, version, string) type (OPENGLAPI *name)params = nullptr; inline void GetCoreFuncAddress_##name(unsigned_int32 ver) {*(void **) &name = GetCoreFuncAddress((ver >= version), #name, string);}
		
		#elif C4MACOS
		
			void *GetCoreFuncAddress(CFBundleRef bundle, bool core, const char *coreName, const char *extName);
			
			#define GLCOREFUNC(type, name, params, version, string) type (*name)params = nullptr; inline void GetCoreFuncAddress_##name(CFBundleRef bundle, unsigned_int32 ver) {*(void **) &name = GetCoreFuncAddress(bundle, (ver >= version), #name, string);}
		
		#endif
		
		#define GLEXTFUNC(type, name, params) type (OPENGLAPI *name)params = nullptr;
	
	#else
	
		#define GLCOREFUNC(type, name, params, version, string) extern type (OPENGLAPI *name)params;
		#define GLEXTFUNC(type, name, params) extern type (OPENGLAPI *name)params;
	
	#endif
	
	
	#if C4WINDOWS && C4FASTBUILD
	
		// -------------------------------------------------------------------
		//
		// Core OpenGL
		//
		// -------------------------------------------------------------------
		
		typedef unsigned int	GLenum;
		typedef unsigned char	GLboolean;
		typedef unsigned int	GLbitfield;
		typedef signed char		GLbyte;
		typedef short			GLshort;
		typedef int				GLint;
		typedef int				GLsizei;
		typedef unsigned char	GLubyte;
		typedef unsigned short	GLushort;
		typedef unsigned int	GLuint;
		typedef float			GLfloat;
		typedef float			GLclampf;
		typedef double			GLdouble;
		typedef double			GLclampd;
		
		#define GL_NONE													0
		#define GL_NO_ERROR												0
		#define GL_ZERO													0
		#define GL_ONE													1
		#define GL_POINTS												0x0000
		#define GL_LINES												0x0001
		#define GL_LINE_LOOP											0x0002
		#define GL_LINE_STRIP											0x0003
		#define GL_TRIANGLES											0x0004
		#define GL_TRIANGLE_STRIP										0x0005
		#define GL_TRIANGLE_FAN											0x0006
		#define GL_QUADS												0x0007
		#define GL_QUAD_STRIP											0x0008
		#define GL_POLYGON												0x0009
		#define GL_NEVER												0x0200
		#define GL_LESS													0x0201
		#define GL_EQUAL												0x0202
		#define GL_LEQUAL												0x0203
		#define GL_GREATER												0x0204
		#define GL_NOTEQUAL												0x0205
		#define GL_GEQUAL												0x0206
		#define GL_ALWAYS												0x0207
		#define GL_SRC_COLOR											0x0300
		#define GL_ONE_MINUS_SRC_COLOR									0x0301
		#define GL_SRC_ALPHA											0x0302
		#define GL_ONE_MINUS_SRC_ALPHA									0x0303
		#define GL_DST_ALPHA											0x0304
		#define GL_ONE_MINUS_DST_ALPHA									0x0305
		#define GL_DST_COLOR											0x0306
		#define GL_ONE_MINUS_DST_COLOR									0x0307
		#define GL_SRC_ALPHA_SATURATE									0x0308
		#define GL_FRONT_LEFT											0x0400
		#define GL_FRONT_RIGHT											0x0401 
		#define GL_BACK_LEFT											0x0402
		#define GL_BACK_RIGHT											0x0403 
		#define GL_FRONT												0x0404 
		#define GL_BACK													0x0405 
		#define GL_LEFT													0x0406
		#define GL_RIGHT												0x0407 
		#define GL_FRONT_AND_BACK										0x0408
		#define GL_CW													0x0900
		#define GL_CCW													0x0901
		#define GL_LINE_SMOOTH											0x0B20 
		#define GL_CULL_FACE											0x0B44
		#define GL_DEPTH_TEST											0x0B71
		#define GL_STENCIL_TEST											0x0B90
		#define GL_ALPHA_TEST											0x0BC0 
		#define GL_BLEND												0x0BE2
		#define GL_SCISSOR_TEST											0x0C11
		#define GL_UNPACK_ROW_LENGTH									0x0CF2
		#define GL_UNPACK_ALIGNMENT										0x0CF5
		#define GL_PACK_ROW_LENGTH										0x0D02
		#define GL_MAX_TEXTURE_SIZE										0x0D33
		#define GL_MAX_VIEWPORT_DIMS									0x0D3A
		#define GL_TEXTURE_2D											0x0DE1
		#define GL_TEXTURE_BORDER_COLOR									0x1004
		#define GL_BYTE													0x1400
		#define GL_UNSIGNED_BYTE										0x1401
		#define GL_SHORT												0x1402
		#define GL_UNSIGNED_SHORT										0x1403
		#define GL_INT													0x1404
		#define GL_UNSIGNED_INT											0x1405
		#define GL_FLOAT												0x1406
		#define GL_INVERT												0x150A
		#define GL_DEPTH_COMPONENT										0x1902
		#define GL_RGBA													0x1908
		#define GL_LUMINANCE											0x1909
		#define GL_LUMINANCE_ALPHA										0x190A
		#define GL_POINT												0x1B00
		#define GL_LINE													0x1B01
		#define GL_FILL													0x1B02
		#define GL_KEEP													0x1E00
		#define GL_REPLACE												0x1E01
		#define GL_INCR													0x1E02
		#define GL_DECR													0x1E03
		#define GL_VENDOR												0x1F00
		#define GL_RENDERER												0x1F01
		#define GL_VERSION												0x1F02
		#define GL_EXTENSIONS											0x1F03
		#define GL_NEAREST												0x2600
		#define GL_LINEAR												0x2601
		#define GL_NEAREST_MIPMAP_NEAREST								0x2700
		#define GL_LINEAR_MIPMAP_NEAREST								0x2701
		#define GL_NEAREST_MIPMAP_LINEAR								0x2702
		#define GL_LINEAR_MIPMAP_LINEAR									0x2703
		#define GL_TEXTURE_MAG_FILTER									0x2800
		#define GL_TEXTURE_MIN_FILTER									0x2801
		#define GL_TEXTURE_WRAP_S										0x2802
		#define GL_TEXTURE_WRAP_T										0x2803
		#define GL_CLAMP												0x2900
		#define GL_REPEAT												0x2901
		#define GL_POLYGON_OFFSET_POINT									0x2A01
		#define GL_POLYGON_OFFSET_LINE									0x2A02
		#define GL_POLYGON_OFFSET_FILL									0x8037
		#define GL_LUMINANCE8											0x8040
		#define GL_LUMINANCE8_ALPHA8									0x8045
		#define GL_INTENSITY											0x8049
		#define GL_INTENSITY8											0x804B
		#define GL_INTENSITY16											0x804D
		#define GL_RGB8													0x8051
		#define GL_RGBA8												0x8058	
		#define GL_DEPTH_BUFFER_BIT										0x00000100
		#define GL_STENCIL_BUFFER_BIT									0x00000400
		#define GL_COLOR_BUFFER_BIT										0x00004000
		
		extern "C"
		{
			WINGDIAPI void APIENTRY glAlphaFunc(GLenum, GLclampf);
			WINGDIAPI void APIENTRY glBegin(GLenum);
			WINGDIAPI void APIENTRY glBindTexture(GLenum, GLuint);
			WINGDIAPI void APIENTRY glBlendFunc(GLenum, GLenum);
			WINGDIAPI void APIENTRY glClear(GLbitfield);
			WINGDIAPI void APIENTRY glClearColor(GLclampf, GLclampf, GLclampf, GLclampf);
			WINGDIAPI void APIENTRY glClearDepth(GLclampd);
			WINGDIAPI void APIENTRY glClearStencil(GLint);
			WINGDIAPI void APIENTRY glColorMask(GLboolean, GLboolean, GLboolean, GLboolean);
			WINGDIAPI void APIENTRY glCopyTexSubImage2D(GLenum, GLint, GLint, GLint, GLint, GLint, GLsizei, GLsizei);
			WINGDIAPI void APIENTRY glCullFace(GLenum);
			WINGDIAPI void APIENTRY glDeleteTextures(GLsizei, const GLuint *);
			WINGDIAPI void APIENTRY glDepthFunc(GLenum);
			WINGDIAPI void APIENTRY glDepthMask(GLboolean);
			WINGDIAPI void APIENTRY glDepthRange(GLclampd, GLclampd);
			WINGDIAPI void APIENTRY glDisable(GLenum);
			WINGDIAPI void APIENTRY glDisableClientState(GLenum);
			WINGDIAPI void APIENTRY glDrawArrays(GLenum, GLint, GLsizei);
			WINGDIAPI void APIENTRY glDrawBuffer(GLenum);
			WINGDIAPI void APIENTRY glDrawElements(GLenum, GLsizei, GLenum, const void *);
			WINGDIAPI void APIENTRY glEnable(GLenum);
			WINGDIAPI void APIENTRY glEnableClientState(GLenum);
			WINGDIAPI void APIENTRY glEnd(void);
			WINGDIAPI void APIENTRY glFinish(void);
			WINGDIAPI void APIENTRY glFlush(void);
			WINGDIAPI void APIENTRY glFrontFace(GLenum);
			WINGDIAPI void APIENTRY glFrustum(GLdouble, GLdouble, GLdouble, GLdouble, GLdouble, GLdouble);
			WINGDIAPI void APIENTRY glGenTextures(GLsizei, GLuint *);
			WINGDIAPI GLenum APIENTRY glGetError(void);
			WINGDIAPI void APIENTRY glGetFloatv(GLenum, GLfloat *);
			WINGDIAPI void APIENTRY glGetIntegerv(GLenum, GLint *);
			WINGDIAPI const GLubyte *APIENTRY glGetString(GLenum);
			WINGDIAPI void APIENTRY glPixelStorei(GLenum, GLint);
			WINGDIAPI void APIENTRY glPointSize(GLfloat);
			WINGDIAPI void APIENTRY glPolygonMode(GLenum, GLenum);
			WINGDIAPI void APIENTRY glPolygonOffset(GLfloat, GLfloat);
			WINGDIAPI void APIENTRY glReadBuffer(GLenum);
			WINGDIAPI void APIENTRY glReadPixels(GLint, GLint, GLsizei, GLsizei, GLenum, GLenum, void *);
			WINGDIAPI void APIENTRY glScissor(GLint, GLint, GLsizei, GLsizei);
			WINGDIAPI void APIENTRY glStencilFunc(GLenum, GLint, GLuint);
			WINGDIAPI void APIENTRY glStencilMask(GLuint);
			WINGDIAPI void APIENTRY glStencilOp(GLenum, GLenum, GLenum);
			WINGDIAPI void APIENTRY glTexEnvi(GLenum, GLenum, GLint);
			WINGDIAPI void APIENTRY glTexImage2D(GLenum, GLint, GLint, GLsizei, GLsizei, GLint, GLenum, GLenum, const void *);
			WINGDIAPI void APIENTRY glTexParameterf(GLenum, GLenum, GLfloat);
			WINGDIAPI void APIENTRY glTexParameterfv(GLenum, GLenum, const GLfloat *);
			WINGDIAPI void APIENTRY glTexParameteri(GLenum, GLenum, GLint);
			WINGDIAPI void APIENTRY glTexParameteriv(GLenum, GLenum, const GLint *);
			WINGDIAPI void APIENTRY glTexSubImage2D(GLenum, GLint, GLint, GLint, GLsizei, GLsizei, GLenum, GLenum, const void *);
			WINGDIAPI void APIENTRY glViewport(GLint, GLint, GLsizei, GLsizei);	
		}
	
	#endif
	
	
	typedef char				GLchar;
	typedef GLuint				GLhandle;
	typedef ptrdiff_t			GLintptr;
	typedef ptrdiff_t			GLsizeiptr;
	typedef int64				GLint64;
	typedef unsigned_int64		GLuint64;
	
	
	// -------------------------------------------------------------------
	//
	// OpenGL 1.2 Core Features
	//
	// -------------------------------------------------------------------
	
	// bgra
	#define GL_BGR													0x80E0
	#define GL_BGRA													0x80E1
	
	// blend_color
	#define GL_CONSTANT_COLOR										0x8001
	#define GL_ONE_MINUS_CONSTANT_COLOR								0x8002
	#define GL_CONSTANT_ALPHA										0x8003
	#define GL_ONE_MINUS_CONSTANT_ALPHA								0x8004
	#define GL_BLEND_COLOR											0x8005
	
	GLCOREFUNC(void, glBlendColor, (GLclampf, GLclampf, GLclampf, GLclampf), 0x0120, "glBlendColorEXT")
	
	// blend_minmax
	#define GL_FUNC_ADD												0x8006
	#define GL_MIN													0x8007
	#define GL_MAX													0x8008
	#define GL_BLEND_EQUATION										0x8009
	
	GLCOREFUNC(void, glBlendEquation, (GLenum), 0x0120, "glBlendEquationEXT")
	
	// blend_subtract
	#define GL_FUNC_SUBTRACT										0x800A
	#define GL_FUNC_REVERSE_SUBTRACT								0x800B
	
	// draw_range_elements
	GLCOREFUNC(void, glDrawRangeElements, (GLenum, GLuint, GLuint, GLsizei, GLenum, const void *), 0x0120, "glDrawRangeElementsEXT")
	
	// packed_pixels
	#define GL_UNSIGNED_BYTE_3_3_2									0x8032
	#define GL_UNSIGNED_SHORT_4_4_4_4								0x8033
	#define GL_UNSIGNED_SHORT_5_5_5_1								0x8034
	#define GL_UNSIGNED_INT_8_8_8_8									0x8035
	#define GL_UNSIGNED_INT_10_10_10_2								0x8036
	#define GL_UNSIGNED_BYTE_2_3_3_REV								0x8362
	#define GL_UNSIGNED_SHORT_5_6_5									0x8363
	#define GL_UNSIGNED_SHORT_5_6_5_REV								0x8364
	#define GL_UNSIGNED_SHORT_4_4_4_4_REV							0x8365
	#define GL_UNSIGNED_SHORT_1_5_5_5_REV							0x8366
	#define GL_UNSIGNED_INT_8_8_8_8_REV								0x8367
	#define GL_UNSIGNED_INT_2_10_10_10_REV							0x8368
	
	// texture3D
	#define GL_PACK_SKIP_IMAGES										0x806B
	#define GL_PACK_IMAGE_HEIGHT									0x806C
	#define GL_UNPACK_SKIP_IMAGES									0x806D
	#define GL_UNPACK_IMAGE_HEIGHT									0x806E
	#define GL_TEXTURE_3D											0x806F
	#define GL_PROXY_TEXTURE_3D										0x8070
	#define GL_TEXTURE_DEPTH										0x8071
	#define GL_TEXTURE_WRAP_R										0x8072
	#define GL_MAX_3D_TEXTURE_SIZE									0x8073
	
	GLCOREFUNC(void, glTexImage3D, (GLenum, GLint, GLenum, GLsizei, GLsizei, GLsizei, GLint, GLenum, GLenum, const void *), 0x0120, "glTexImage3DEXT")
	GLCOREFUNC(void, glTexSubImage3D, (GLenum, GLint, GLint, GLint, GLint, GLsizei, GLsizei, GLsizei, GLenum, GLenum, const void *), 0x0120, "glTexSubImage3DEXT")
	GLCOREFUNC(void, glCopyTexSubImage3D, (GLenum, GLint, GLint, GLint, GLint, GLint, GLint, GLsizei, GLsizei), 0x0120, "glCopyTexSubImage3DEXT")
	
	// texture_edge_clamp
	#define GL_CLAMP_TO_EDGE										0x812F
	
	// texture_lod
	#define GL_TEXTURE_MIN_LOD										0x813A
	#define GL_TEXTURE_MAX_LOD										0x813B
	#define GL_TEXTURE_BASE_LEVEL									0x813C
	#define GL_TEXTURE_MAX_LEVEL									0x813D
	
	
	// -------------------------------------------------------------------
	//
	// OpenGL 1.3 Core Features
	//
	// -------------------------------------------------------------------
	
	// texture_border_clamp
	#define GL_CLAMP_TO_BORDER										0x812D
	
	// multisample
	#define GL_MULTISAMPLE											0x809D
	#define GL_SAMPLE_ALPHA_TO_COVERAGE								0x809E
	#define GL_SAMPLE_ALPHA_TO_ONE									0x809F
	#define GL_SAMPLE_COVERAGE										0x80A0
	#define GL_SAMPLE_BUFFERS										0x80A8
	#define GL_SAMPLES												0x80A9
	#define GL_SAMPLE_COVERAGE_VALUE								0x80AA
	#define GL_SAMPLE_COVERAGE_INVERT								0x80AB
	#define GL_MULTISAMPLE_BIT										0x20000000
	
	GLCOREFUNC(void, glSampleCoverage, (GLclampf, GLboolean), 0x0130, "glSampleCoverageARB")
	
	// multitexture
	#define GL_TEXTURE0												0x84C0
	#define GL_TEXTURE1												0x84C1
	#define GL_TEXTURE2												0x84C2
	#define GL_TEXTURE3												0x84C3
	#define GL_TEXTURE4												0x84C4
	#define GL_TEXTURE5												0x84C5
	#define GL_TEXTURE6												0x84C6
	#define GL_TEXTURE7												0x84C7
	#define GL_TEXTURE8												0x84C8
	#define GL_TEXTURE9												0x84C9
	#define GL_TEXTURE10											0x84CA
	#define GL_TEXTURE11											0x84CB
	#define GL_TEXTURE12											0x84CC
	#define GL_TEXTURE13											0x84CD
	#define GL_TEXTURE14											0x84CE
	#define GL_TEXTURE15											0x84CF
	#define GL_TEXTURE16											0x84D0
	#define GL_TEXTURE17											0x84D1
	#define GL_TEXTURE18											0x84D2
	#define GL_TEXTURE19											0x84D3
	#define GL_TEXTURE20											0x84D4
	#define GL_TEXTURE21											0x84D5
	#define GL_TEXTURE22											0x84D6
	#define GL_TEXTURE23											0x84D7
	#define GL_TEXTURE24											0x84D8
	#define GL_TEXTURE25											0x84D9
	#define GL_TEXTURE26											0x84DA
	#define GL_TEXTURE27											0x84DB
	#define GL_TEXTURE28											0x84DC
	#define GL_TEXTURE29											0x84DD
	#define GL_TEXTURE30											0x84DE
	#define GL_TEXTURE31											0x84DF
	#define GL_ACTIVE_TEXTURE										0x84E0
	#define GL_CLIENT_ACTIVE_TEXTURE								0x84E1
	#define GL_MAX_TEXTURE_UNITS									0x84E2
	
	GLCOREFUNC(void, glActiveTexture, (GLenum), 0x0130, "glActiveTextureARB")
	GLCOREFUNC(void, glClientActiveTexture, (GLenum), 0x0130, "glClientActiveTextureARB")
	
	// texture_compression
	#define GL_COMPRESSED_ALPHA										0x84E9
	#define GL_COMPRESSED_LUMINANCE									0x84EA
	#define GL_COMPRESSED_LUMINANCE_ALPHA							0x84EB
	#define GL_COMPRESSED_INTENSITY									0x84EC
	#define GL_COMPRESSED_RGB										0x84ED
	#define GL_COMPRESSED_RGBA										0x84EE
	#define GL_TEXTURE_COMPRESSION_HINT								0x84EF
	#define GL_TEXTURE_COMPRESSED_IMAGE_SIZE						0x86A0
	#define GL_TEXTURE_COMPRESSED									0x86A1
	#define GL_NUM_COMPRESSED_TEXTURE_FORMATS						0x86A2
	#define GL_COMPRESSED_TEXTURE_FORMATS							0x86A3
	
	GLCOREFUNC(void, glCompressedTexImage3D, (GLenum, GLint, GLenum, GLsizei, GLsizei, GLsizei, GLint, GLsizei, const void *), 0x0130, "glCompressedTexImage3DARB")
	GLCOREFUNC(void, glCompressedTexImage2D, (GLenum, GLint, GLenum, GLsizei, GLsizei, GLint, GLsizei, const void *), 0x0130, "glCompressedTexImage2DARB")
	GLCOREFUNC(void, glCompressedTexImage1D, (GLenum, GLint, GLenum, GLsizei, GLint, GLsizei, const void *), 0x0130, "glCompressedTexImage1DARB")
	GLCOREFUNC(void, glCompressedTexSubImage3D, (GLenum, GLint, GLint, GLint, GLint, GLsizei, GLsizei, GLsizei, GLenum, GLsizei, const void *), 0x0130, "glCompressedTexSubImage3DARB")
	GLCOREFUNC(void, glCompressedTexSubImage2D, (GLenum, GLint, GLint, GLint, GLsizei, GLsizei, GLenum, GLsizei, const void *), 0x0130, "glCompressedTexSubImage2DARB")
	GLCOREFUNC(void, glCompressedTexSubImage1D, (GLenum, GLint, GLint, GLsizei, GLenum, GLsizei, const void *), 0x0130, "glCompressedTexSubImage1DARB")
	
	// texture_cube_map
	#define GL_NORMAL_MAP											0x8511
	#define GL_REFLECTION_MAP										0x8512
	#define GL_TEXTURE_CUBE_MAP										0x8513
	#define GL_TEXTURE_CUBE_MAP_POSITIVE_X							0x8515
	#define GL_TEXTURE_CUBE_MAP_NEGATIVE_X							0x8516
	#define GL_TEXTURE_CUBE_MAP_POSITIVE_Y							0x8517
	#define GL_TEXTURE_CUBE_MAP_NEGATIVE_Y							0x8518
	#define GL_TEXTURE_CUBE_MAP_POSITIVE_Z							0x8519
	#define GL_TEXTURE_CUBE_MAP_NEGATIVE_Z							0x851A
	#define GL_PROXY_TEXTURE_CUBE_MAP								0x851B 
	#define GL_MAX_CUBE_MAP_TEXTURE_SIZE							0x851C
	
	
	// -------------------------------------------------------------------
	//
	// OpenGL 1.4 Core Features
	//
	// -------------------------------------------------------------------
	
	// blend_func_separate
	#define GL_BLEND_DST_RGB										0x80C8
	#define GL_BLEND_SRC_RGB										0x80C9
	#define GL_BLEND_DST_ALPHA										0x80CA
	#define GL_BLEND_SRC_ALPHA										0x80CB
	
	GLCOREFUNC(void, glBlendFuncSeparate, (GLenum, GLenum, GLenum, GLenum), 0x0140, "glBlendFuncSeparateEXT")
	
	// depth_texture
	#define GL_DEPTH_COMPONENT16									0x81A5
	#define GL_DEPTH_COMPONENT24									0x81A6
	#define GL_DEPTH_COMPONENT32									0x81A7
	#define GL_TEXTURE_DEPTH_SIZE									0x884A
	#define GL_DEPTH_TEXTURE_MODE									0x884B
	
	// multi_draw_arrays
	GLCOREFUNC(void, glMultiDrawArrays, (GLenum, const GLint *, const GLsizei *, GLsizei), 0x0140, "glMultiDrawArraysEXT")
	GLCOREFUNC(void, glMultiDrawElements, (GLenum, const GLsizei *, GLenum, const void *const *, GLsizei), 0x0140, "glMultiDrawElementsEXT")
	
	// shadow
	#define GL_TEXTURE_COMPARE_MODE									0x884C
	#define GL_TEXTURE_COMPARE_FUNC									0x884D
	#define GL_COMPARE_REF_TO_TEXTURE								0x884E
	
	// stencil_wrap
	#define GL_INCR_WRAP											0x8507
	#define GL_DECR_WRAP											0x8508
	
	// texture_lod_bias
	#define GL_TEXTURE_FILTER_CONTROL								0x8500
	#define GL_TEXTURE_LOD_BIAS										0x8501
	#define GL_MAX_TEXTURE_LOD_BIAS									0x84FD
	
	// texture_mirrored_repeat
	#define GL_MIRRORED_REPEAT										0x8370
	
	
	// -------------------------------------------------------------------
	//
	// OpenGL 1.5 Core Features
	//
	// -------------------------------------------------------------------
	
	// occlusion_query
	#define GL_SAMPLES_PASSED										0x8914
	#define GL_QUERY_COUNTER_BITS									0x8864
	#define GL_CURRENT_QUERY										0x8865
	#define GL_QUERY_RESULT											0x8866
	#define GL_QUERY_RESULT_AVAILABLE								0x8867
	
	GLCOREFUNC(void, glGenQueries, (GLsizei, GLuint *), 0x0150, "glGenQueriesARB")
	GLCOREFUNC(void, glDeleteQueries, (GLsizei, const GLuint *), 0x0150, "glDeleteQueriesARB")
	GLCOREFUNC(void, glBeginQuery, (GLenum, GLuint), 0x0150, "glBeginQueryARB")
	GLCOREFUNC(void, glEndQuery, (GLenum), 0x0150, "glEndQueryARB")
	GLCOREFUNC(void, glGetQueryiv, (GLenum, GLenum, GLint *), 0x0150, "glGetQueryivARB")
	GLCOREFUNC(void, glGetQueryObjectiv, (GLuint, GLenum, GLint *), 0x0150, "glGetQueryObjectivARB")
	GLCOREFUNC(void, glGetQueryObjectuiv, (GLuint, GLenum, GLuint *), 0x0150, "glGetQueryObjectuivARB")
	
	// vertex_buffer_object
	#define GL_ARRAY_BUFFER											0x8892
	#define GL_ELEMENT_ARRAY_BUFFER									0x8893
	#define GL_STREAM_DRAW											0x88E0
	#define GL_STREAM_READ											0x88E1
	#define GL_STREAM_COPY											0x88E2
	#define GL_STATIC_DRAW											0x88E4
	#define GL_STATIC_READ											0x88E5
	#define GL_STATIC_COPY											0x88E6
	#define GL_DYNAMIC_DRAW											0x88E8
	#define GL_DYNAMIC_READ											0x88E9
	#define GL_DYNAMIC_COPY											0x88EA
	#define GL_READ_ONLY											0x88B8
	#define GL_WRITE_ONLY											0x88B9
	#define GL_READ_WRITE											0x88BA
	
	GLCOREFUNC(void, glBindBuffer, (GLenum, GLuint), 0x0150, "glBindBufferARB")
	GLCOREFUNC(void, glDeleteBuffers, (GLsizei, const GLuint *), 0x0150, "glDeleteBuffersARB")
	GLCOREFUNC(void, glGenBuffers, (GLsizei, GLuint *), 0x0150, "glGenBuffersARB")
	GLCOREFUNC(void, glBufferData, (GLenum, GLsizeiptr, const void *, GLenum), 0x0150, "glBufferDataARB")
	GLCOREFUNC(void, glBufferSubData, (GLenum, GLintptr, GLsizeiptr, const void *), 0x0150, "glBufferSubDataARB")
	GLCOREFUNC(void *, glMapBuffer, (GLenum, GLenum), 0x0150, "glMapBufferARB")
	GLCOREFUNC(GLboolean, glUnmapBuffer, (GLenum), 0x0150, "glUnmapBufferARB")
	
	
	// -------------------------------------------------------------------
	//
	// OpenGL 2.0 Core Features
	//
	// -------------------------------------------------------------------
	
	// fragment_shader
	#define GL_FRAGMENT_SHADER										0x8B30
	#define GL_MAX_FRAGMENT_UNIFORM_COMPONENTS						0x8B49
	#define GL_FRAGMENT_SHADER_DERIVATIVE_HINT						0x8B8B
	
	// point_parameters
	GLCOREFUNC(void, glPointParameterf, (GLenum, GLfloat), 0x0200, nullptr)
	GLCOREFUNC(void, glPointParameterfv, (GLenum, const GLfloat *), 0x0200, nullptr)
	GLCOREFUNC(void, glPointParameteri, (GLenum, GLint), 0x0200, nullptr)
	GLCOREFUNC(void, glPointParameteriv, (GLenum, const GLint *), 0x0200, nullptr)
	
	// point_sprite
	#define GL_POINT_SPRITE											0x8861
	#define GL_COORD_REPLACE										0x8862
	#define GL_POINT_SPRITE_COORD_ORIGIN							0x8CA0
	#define GL_LOWER_LEFT											0x8CA1
	#define GL_UPPER_LEFT											0x8CA2
	
	// separate_stencil
	#define GL_STENCIL_BACK_FUNC									0x8800
	#define GL_STENCIL_BACK_FAIL									0x8801
	#define GL_STENCIL_BACK_PASS_DEPTH_FAIL							0x8802
	#define GL_STENCIL_BACK_PASS_DEPTH_PASS							0x8803
	
	GLCOREFUNC(void, glStencilOpSeparate, (GLenum, GLenum, GLenum, GLenum), 0x0200, "glStencilOpSeparateATI")
	GLCOREFUNC(void, glStencilFuncSeparate, (GLenum, GLenum, GLint, GLuint), 0x0200, "glStencilFuncSeparateATI")
	
	// shader_objects
	#define GL_PROGRAM_OBJECT										0x8B40
	#define GL_OBJECT_TYPE											0x8B4E
	#define GL_OBJECT_SUBTYPE										0x8B4F
	#define GL_OBJECT_DELETE_STATUS									0x8B80
	#define GL_OBJECT_COMPILE_STATUS								0x8B81
	#define GL_OBJECT_LINK_STATUS									0x8B82
	#define GL_OBJECT_VALIDATE_STATUS								0x8B83
	#define GL_OBJECT_INFO_LOG_LENGTH								0x8B84
	#define GL_OBJECT_ATTACHED_OBJECTS								0x8B85
	#define GL_OBJECT_ACTIVE_UNIFORMS								0x8B86
	#define GL_OBJECT_ACTIVE_UNIFORM_MAX_LENGTH						0x8B87
	#define GL_OBJECT_SHADER_SOURCE_LENGTH							0x8B88
	#define GL_SHADER_OBJECT										0x8B48
	#define GL_FLOAT_VEC2											0x8B50
	#define GL_FLOAT_VEC3											0x8B51
	#define GL_FLOAT_VEC4											0x8B52
	#define GL_INT_VEC2												0x8B53
	#define GL_INT_VEC3												0x8B54
	#define GL_INT_VEC4												0x8B55
	#define GL_BOOL													0x8B56
	#define GL_BOOL_VEC2											0x8B57
	#define GL_BOOL_VEC3											0x8B58
	#define GL_BOOL_VEC4											0x8B59
	#define GL_FLOAT_MAT2											0x8B5A
	#define GL_FLOAT_MAT3											0x8B5B
	#define GL_FLOAT_MAT4											0x8B5C
	#define GL_SAMPLER_1D											0x8B5D
	#define GL_SAMPLER_2D											0x8B5E
	#define GL_SAMPLER_3D											0x8B5F
	#define GL_SAMPLER_CUBE											0x8B60
	#define GL_SAMPLER_1D_SHADOW									0x8B61
	#define GL_SAMPLER_2D_SHADOW									0x8B62
	#define GL_SAMPLER_2D_RECT										0x8B63
	#define GL_SAMPLER_2D_RECT_SHADOW								0x8B64
	#define GL_SHADING_LANGUAGE_VERSION								0x8B8C
	
	GLCOREFUNC(void, glDeleteObject, (GLhandle), 0x0200, "glDeleteObjectARB")
	GLCOREFUNC(GLhandle, glGetHandle, (GLenum), 0x0200, "glGetHandleARB")
	GLCOREFUNC(void, glDetachObject, (GLhandle, GLhandle), 0x0200, "glDetachObjectARB")
	GLCOREFUNC(GLhandle, glCreateShaderObject, (GLenum), 0x0200, "glCreateShaderObjectARB")
	GLCOREFUNC(void, glShaderSource, (GLhandle, GLsizei, const GLchar **, const GLint *), 0x0200, "glShaderSourceARB")
	GLCOREFUNC(void, glCompileShader, (GLhandle), 0x0200, "glCompileShaderARB")
	GLCOREFUNC(GLhandle, glCreateProgramObject, (void), 0x0200, "glCreateProgramObjectARB")
	GLCOREFUNC(void, glAttachObject, (GLhandle, GLhandle), 0x0200, "glAttachObjectARB")
	GLCOREFUNC(void, glLinkProgram, (GLhandle), 0x0200, "glLinkProgramARB")
	GLCOREFUNC(void, glUseProgramObject, (GLhandle), 0x0200, "glUseProgramObjectARB")
	GLCOREFUNC(void, glValidateProgram, (GLhandle), 0x0200, "glValidateProgramARB")
	GLCOREFUNC(void, glUniform1f, (GLint, GLfloat ), 0x0200, "glUniform1fARB")
	GLCOREFUNC(void, glUniform2f, (GLint, GLfloat, GLfloat), 0x0200, "glUniform2fARB")
	GLCOREFUNC(void, glUniform3f, (GLint, GLfloat, GLfloat, GLfloat), 0x0200, "glUniform3fARB")
	GLCOREFUNC(void, glUniform4f, (GLint, GLfloat, GLfloat, GLfloat, GLfloat), 0x0200, "glUniform4fARB")
	GLCOREFUNC(void, glUniform1i, (GLint, GLint), 0x0200, "glUniform1iARB")
	GLCOREFUNC(void, glUniform2i, (GLint, GLint, GLint), 0x0200, "glUniform2iARB")
	GLCOREFUNC(void, glUniform3i, (GLint, GLint, GLint, GLint), 0x0200, "glUniform3iARB")
	GLCOREFUNC(void, glUniform4i, (GLint, GLint, GLint, GLint, GLint), 0x0200, "glUniform4iARB")
	GLCOREFUNC(void, glUniform1fv, (GLint, GLsizei, const GLfloat *), 0x0200, "glUniform1fvARB")
	GLCOREFUNC(void, glUniform2fv, (GLint, GLsizei, const GLfloat *), 0x0200, "glUniform2fvARB")
	GLCOREFUNC(void, glUniform3fv, (GLint, GLsizei, const GLfloat *), 0x0200, "glUniform3fvARB")
	GLCOREFUNC(void, glUniform4fv, (GLint, GLsizei, const GLfloat *), 0x0200, "glUniform4fvARB")
	GLCOREFUNC(void, glUniform1iv, (GLint, GLsizei, const GLint *), 0x0200, "glUniform1ivARB")
	GLCOREFUNC(void, glUniform2iv, (GLint, GLsizei, const GLint *), 0x0200, "glUniform2ivARB")
	GLCOREFUNC(void, glUniform3iv, (GLint, GLsizei, const GLint *), 0x0200, "glUniform3ivARB")
	GLCOREFUNC(void, glUniform4iv, (GLint, GLsizei, const GLint *), 0x0200, "glUniform4ivARB")
	GLCOREFUNC(void, glUniformMatrix2fv, (GLint, GLsizei, GLboolean, const GLfloat *), 0x0200, "glUniformMatrix2fvARB")
	GLCOREFUNC(void, glUniformMatrix3fv, (GLint, GLsizei, GLboolean, const GLfloat *), 0x0200, "glUniformMatrix3fvARB")
	GLCOREFUNC(void, glUniformMatrix4fv, (GLint, GLsizei, GLboolean, const GLfloat *), 0x0200, "glUniformMatrix4fvARB")
	GLCOREFUNC(void, glGetObjectParameterfv, (GLhandle, GLenum, GLfloat *), 0x0200, "glGetObjectParameterfvARB")
	GLCOREFUNC(void, glGetObjectParameteriv, (GLhandle, GLenum, GLint *), 0x0200, "glGetObjectParameterivARB")
	GLCOREFUNC(void, glGetInfoLog, (GLhandle, GLsizei, GLsizei *, GLchar *), 0x0200, "glGetInfoLogARB")
	GLCOREFUNC(void, glGetAttachedObjects, (GLhandle, GLsizei, GLsizei *, GLhandle *), 0x0200, "glGetAttachedObjectsARB")
	GLCOREFUNC(GLint, glGetUniformLocation, (GLhandle, const GLchar *), 0x0200, "glGetUniformLocationARB")
	GLCOREFUNC(void, glGetActiveUniform, (GLhandle, GLuint, GLsizei, GLsizei *, GLint *, GLenum *, GLchar *), 0x0200, "glGetActiveUniformARB")
	
	
	// -------------------------------------------------------------------
	//
	// OpenGL 2.1 Core Features
	//
	// -------------------------------------------------------------------
	
	// pixel_buffer_object
	#define GL_PIXEL_PACK_BUFFER									0x88EB
	#define GL_PIXEL_UNPACK_BUFFER									0x88EC
	
	// texture_sRGB
	#define GL_SRGB													0x8C40
	#define GL_SRGB8												0x8C41
	#define GL_SRGB_ALPHA											0x8C42
	#define GL_SRGB8_ALPHA8											0x8C43
	#define GL_SLUMINANCE_ALPHA										0x8C44
	#define GL_SLUMINANCE8_ALPHA8									0x8C45
	#define GL_SLUMINANCE											0x8C46
	#define GL_SLUMINANCE8											0x8C47
	#define GL_COMPRESSED_SRGB_S3TC_DXT1_EXT						0x8C4C
	#define GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT1_EXT					0x8C4D
	#define GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT3_EXT					0x8C4E
	#define GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT5_EXT					0x8C4F
	
	
	// -------------------------------------------------------------------
	//
	// OpenGL 3.0 Core Features
	//
	// -------------------------------------------------------------------
	
	// conditional_render
	#define GL_QUERY_WAIT											0x8E13
	#define GL_QUERY_NO_WAIT										0x8E14
	#define GL_QUERY_BY_REGION_WAIT									0x8E15
	#define GL_QUERY_BY_REGION_NO_WAIT								0x8E16
	
	GLCOREFUNC(void, glBeginConditionalRender, (GLuint, GLenum), 0x0300, "glBeginConditionalRenderNV")
	GLCOREFUNC(void, glEndConditionalRender, (void), 0x0300, "glEndConditionalRenderNV")
	
	// framebuffer_blit
	#define GL_READ_FRAMEBUFFER										0x8CA8
	#define GL_DRAW_FRAMEBUFFER										0x8CA9
	
	GLCOREFUNC(void, glBlitFramebuffer, (GLint, GLint, GLint, GLint, GLint, GLint, GLint, GLint, GLbitfield, GLenum), 0x0300, "glBlitFramebufferEXT")
	
	// framebuffer_object
	#define GL_FRAMEBUFFER											0x8D40
	#define GL_RENDERBUFFER											0x8D41
	#define GL_STENCIL_INDEX1										0x8D46
	#define GL_STENCIL_INDEX4										0x8D47
	#define GL_STENCIL_INDEX8										0x8D48
	#define GL_STENCIL_INDEX16										0x8D49
	#define GL_RENDERBUFFER_WIDTH									0x8D42
	#define GL_RENDERBUFFER_HEIGHT									0x8D43
	#define GL_RENDERBUFFER_INTERNAL_FORMAT							0x8D44
	#define GL_FRAMEBUFFER_ATTACHMENT_OBJECT_TYPE					0x8CD0
	#define GL_FRAMEBUFFER_ATTACHMENT_OBJECT_NAME					0x8CD1
	#define GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_LEVEL					0x8CD2
	#define GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_CUBE_MAP_FACE			0x8CD3
	#define GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_3D_ZOFFSET			0x8CD4
	#define GL_COLOR_ATTACHMENT0									0x8CE0
	#define GL_COLOR_ATTACHMENT1									0x8CE1
	#define GL_COLOR_ATTACHMENT2									0x8CE2
	#define GL_COLOR_ATTACHMENT3									0x8CE3
	#define GL_COLOR_ATTACHMENT4									0x8CE4
	#define GL_COLOR_ATTACHMENT5									0x8CE5
	#define GL_COLOR_ATTACHMENT6									0x8CE6
	#define GL_COLOR_ATTACHMENT7									0x8CE7
	#define GL_COLOR_ATTACHMENT8									0x8CE8
	#define GL_COLOR_ATTACHMENT9									0x8CE9
	#define GL_COLOR_ATTACHMENT10									0x8CEA
	#define GL_COLOR_ATTACHMENT11									0x8CEB
	#define GL_COLOR_ATTACHMENT12									0x8CEC
	#define GL_COLOR_ATTACHMENT13									0x8CED
	#define GL_COLOR_ATTACHMENT14									0x8CEE
	#define GL_COLOR_ATTACHMENT15									0x8CEF
	#define GL_DEPTH_ATTACHMENT										0x8D00
	#define GL_STENCIL_ATTACHMENT									0x8D20
	#define GL_FRAMEBUFFER_COMPLETE									0x8CD5
	#define GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT					0x8CD6
	#define GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT			0x8CD7
	#define GL_FRAMEBUFFER_INCOMPLETE_DIMENSIONS					0x8CD9
	#define GL_FRAMEBUFFER_INCOMPLETE_FORMATS						0x8CDA
	#define GL_FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER					0x8CDB
	#define GL_FRAMEBUFFER_INCOMPLETE_READ_BUFFER					0x8CDC
	#define GL_FRAMEBUFFER_UNSUPPORTED								0x8CDD
	#define GL_FRAMEBUFFER_STATUS_ERROR								0x8CDE
	#define GL_MAX_COLOR_ATTACHMENTS								0x8CDF
	#define GL_MAX_RENDERBUFFER_SIZE								0x84E8
	#define GL_INVALID_FRAMEBUFFER_OPERATION						0x0506
	
	GLCOREFUNC(void, glBindRenderbuffer, (GLenum, GLuint), 0x0300, "glBindRenderbufferEXT")
	GLCOREFUNC(void, glDeleteRenderbuffers, (GLsizei, const GLuint *), 0x0300, "glDeleteRenderbuffersEXT")
	GLCOREFUNC(void, glGenRenderbuffers, (GLsizei, GLuint *), 0x0300, "glGenRenderbuffersEXT")
	GLCOREFUNC(void, glRenderbufferStorage, (GLenum, GLenum, GLsizei, GLsizei), 0x0300, "glRenderbufferStorageEXT")
	GLCOREFUNC(void, glGetRenderbufferParameteriv, (GLenum, GLenum, GLint *), 0x0300, "glGetRenderbufferParameterivEXT")
	GLCOREFUNC(void, glBindFramebuffer, (GLenum, GLuint), 0x0300, "glBindFramebufferEXT")
	GLCOREFUNC(void, glDeleteFramebuffers, (GLsizei, const GLuint *), 0x0300, "glDeleteFramebuffersEXT")
	GLCOREFUNC(void, glGenFramebuffers, (GLsizei, GLuint *), 0x0300, "glGenFramebuffersEXT")
	GLCOREFUNC(GLenum, glCheckFramebufferStatus, (GLenum), 0x0300, "glCheckFramebufferStatusEXT")
	GLCOREFUNC(void, glFramebufferTexture1D, (GLenum, GLenum, GLenum, GLuint, GLint), 0x0300, "glFramebufferTexture1DEXT")
	GLCOREFUNC(void, glFramebufferTexture2D, (GLenum, GLenum, GLenum, GLuint, GLint), 0x0300, "glFramebufferTexture2DEXT")
	GLCOREFUNC(void, glFramebufferTexture3D, (GLenum, GLenum, GLenum, GLuint, GLint, GLint), 0x0300, "glFramebufferTexture3DEXT")
	GLCOREFUNC(void, glFramebufferRenderbuffer, (GLenum, GLenum, GLenum, GLuint), 0x0300, "glFramebufferRenderbufferEXT")
	GLCOREFUNC(void, glGenerateMipmap, (GLenum), 0x0300, "glGenerateMipmapEXT")
	
	// framebuffer_multisample
	#define GL_RENDERBUFFER_SAMPLES									0x8CAB
	#define GL_FRAMEBUFFER_INCOMPLETE_MULTISAMPLE					0x8D56
	#define GL_MAX_SAMPLES											0x8D57
	
	GLCOREFUNC(void, glRenderbufferStorageMultisample, (GLenum, GLsizei, GLenum, GLsizei, GLsizei), 0x0300, "glRenderbufferStorageMultisampleEXT")
	
	// framebuffer_sRGB
	#define GL_FRAMEBUFFER_SRGB										0x8DB9
	#define GL_FRAMEBUFFER_SRGB_CAPABLE								0x8DBA
	
	// packed_depth_stencil
	#define GL_DEPTH_STENCIL										0x84F9
	#define GL_UNSIGNED_INT_24_8									0x84FA
	#define GL_DEPTH24_STENCIL8										0x88F0
	#define GL_TEXTURE_STENCIL_SIZE									0x88F1
	
	// half_float_pixel
	#define GL_HALF_FLOAT											0x140B
	
	// texture_array
	#define GL_TEXTURE_1D_ARRAY										0x8C18
	#define GL_PROXY_TEXTURE_1D_ARRAY								0x8C19
	#define GL_TEXTURE_2D_ARRAY										0x8C1A
	#define GL_PROXY_TEXTURE_2D_ARRAY								0x8C1B
	#define GL_MAX_ARRAY_TEXTURE_LAYERS								0x88FF
	#define GL_COMPARE_REF_DEPTH_TO_TEXTURE							0x884E
	#define GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_LAYER					0x8CD4
	
	GLCOREFUNC(void, glFramebufferTexture, (GLenum, GLenum, GLuint, GLint), 0x0300, "glFramebufferTextureEXT")
	GLCOREFUNC(void, glFramebufferTextureLayer, (GLenum, GLenum, GLuint, GLint, GLint), 0x0300, "glFramebufferTextureLayerEXT")
	
	// texture_compression_rgtc
	#define GL_COMPRESSED_RED_RGTC1									0x8DBB
	#define GL_COMPRESSED_SIGNED_RED_RGTC1							0x8DBC
	#define GL_COMPRESSED_RED_GREEN_RGTC2							0x8DBD
	#define GL_COMPRESSED_SIGNED_RED_GREEN_RGTC2					0x8DBE
	
	// texture_float
	#define GL_TEXTURE_RED_TYPE										0x8C10
	#define GL_TEXTURE_GREEN_TYPE									0x8C11
	#define GL_TEXTURE_BLUE_TYPE									0x8C12
	#define GL_TEXTURE_ALPHA_TYPE									0x8C13
	#define GL_TEXTURE_LUMINANCE_TYPE								0x8C14
	#define GL_TEXTURE_INTENSITY_TYPE								0x8C15
	#define GL_TEXTURE_DEPTH_TYPE									0x8C16
	#define GL_UNSIGNED_NORMALIZED									0x8C17
	#define GL_RGBA32F												0x8814
	#define GL_RGB32F												0x8815
	#define GL_ALPHA32F												0x8816
	#define GL_INTENSITY32F											0x8817
	#define GL_LUMINANCE32F											0x8818
	#define GL_LUMINANCE_ALPHA32F									0x8819
	#define GL_RGBA16F												0x881A
	#define GL_RGB16F												0x881B
	#define GL_ALPHA16F												0x881C
	#define GL_INTENSITY16F											0x881D
	#define GL_LUMINANCE16F											0x881E
	#define GL_LUMINANCE_ALPHA16F									0x881F
	
	
	// -------------------------------------------------------------------
	//
	// OpenGL 3.1 Core Features
	//
	// -------------------------------------------------------------------
	
	// texture_rectangle
	#define GL_TEXTURE_RECTANGLE									0x84F5
	#define GL_MAX_RECTANGLE_TEXTURE_SIZE							0x84F8
	
	// uniform_buffer_object
	#define GL_UNIFORM_BUFFER										0x8A11
	#define GL_UNIFORM_BUFFER_START									0x8A29
	#define GL_UNIFORM_BUFFER_SIZE									0x8A2A
	#define GL_MAX_VERTEX_UNIFORM_BLOCKS							0x8A2B
	#define GL_MAX_GEOMETRY_UNIFORM_BLOCKS							0x8A2C
	#define GL_MAX_FRAGMENT_UNIFORM_BLOCKS							0x8A2D
	#define GL_MAX_COMBINED_UNIFORM_BLOCKS							0x8A2E
	#define GL_MAX_UNIFORM_BUFFER_BINDINGS							0x8A2F
	#define GL_MAX_UNIFORM_BLOCK_SIZE								0x8A30
	#define GL_MAX_COMBINED_VERTEX_UNIFORM_COMPONENTS				0x8A31
	#define GL_MAX_COMBINED_GEOMETRY_UNIFORM_COMPONENTS				0x8A32
	#define GL_MAX_COMBINED_FRAGMENT_UNIFORM_COMPONENTS				0x8A33
	#define GL_UNIFORM_BUFFER_OFFSET_ALIGNMENT						0x8A34
	#define GL_ACTIVE_UNIFORM_BLOCK_MAX_NAME_LENGTH					0x8A35
	#define GL_ACTIVE_UNIFORM_BLOCKS								0x8A36
	#define GL_UNIFORM_TYPE											0x8A37
	#define GL_UNIFORM_SIZE											0x8A38
	#define GL_UNIFORM_NAME_LENGTH									0x8A39
	#define GL_UNIFORM_BLOCK_INDEX									0x8A3A
	#define GL_UNIFORM_OFFSET										0x8A3B
	#define GL_UNIFORM_ARRAY_STRIDE									0x8A3C
	#define GL_UNIFORM_MATRIX_STRIDE								0x8A3D
	#define GL_UNIFORM_IS_ROW_MAJOR									0x8A3E
	#define GL_UNIFORM_BLOCK_DATA_SIZE								0x8A40
	#define GL_UNIFORM_BLOCK_NAME_LENGTH							0x8A41
	#define GL_UNIFORM_BLOCK_ACTIVE_UNIFORMS						0x8A42
	#define GL_UNIFORM_BLOCK_ACTIVE_UNIFORM_INDICES					0x8A43
	#define GL_UNIFORM_BLOCK_REFERENCED_BY_VERTEX_SHADER			0x8A44
	#define GL_UNIFORM_BLOCK_REFERENCED_BY_GEOMETRY_SHADER			0x8A45
	#define GL_UNIFORM_BLOCK_REFERENCED_BY_FRAGMENT_SHADER			0x8A46
	#define GL_INVALID_INDEX										0xFFFFFFFFU
	
	GLCOREFUNC(void, glGetUniformIndices, (GLuint, GLsizei, const char **, GLuint *), 0x0310, "glGetUniformIndicesARB")
	GLCOREFUNC(void, glGetActiveUniformsiv, (GLuint, GLsizei, const GLuint *, GLenum, GLint *), 0x0310, "glGetActiveUniformsivARB")
	GLCOREFUNC(void, glGetActiveUniformName, (GLuint, GLuint, GLsizei, GLsizei *, char *), 0x0310, "glGetActiveUniformNameARB")
	GLCOREFUNC(GLuint, glGetUniformBlockIndex, (GLuint, const char *), 0x0310, "glGetUniformBlockIndexARB")
	GLCOREFUNC(void, glGetActiveUniformBlockiv, (GLuint, GLuint, GLenum, GLint *), 0x0310, "glGetActiveUniformBlockivARB")
	GLCOREFUNC(void, glGetActiveUniformBlockName, (GLuint, GLuint, GLsizei, GLsizei *, char *), 0x0310, "glGetActiveUniformBlockNameARB")
	GLCOREFUNC(void, glBindBufferRange, (GLenum, GLuint, GLuint, GLintptr, GLsizeiptr), 0x0310, "glBindBufferRangeARB")
	GLCOREFUNC(void, glBindBufferBase, (GLenum, GLuint, GLuint), 0x0310, "glBindBufferBaseARB")
	GLCOREFUNC(void, glUniformBlockBinding, (GLuint, GLuint, GLuint), 0x0310, "glUniformBlockBindingARB")

	
	// -------------------------------------------------------------------
	//
	// OpenGL 3.2 Core Features
	//
	// -------------------------------------------------------------------
	
	// depth_clamp
	#define GL_DEPTH_CLAMP											0x864F
	
	// seamless_cube_map
	#define GL_TEXTURE_CUBE_MAP_SEAMLESS							0x884F
	
	
	// -------------------------------------------------------------------
	//
	// OpenGL 3.3 Core Features
	//
	// -------------------------------------------------------------------
	
	// instanced_arrays
	#define GL_VERTEX_ATTRIB_ARRAY_DIVISOR							0x88FE
	
	GLCOREFUNC(void, glDrawElementsInstanced, (GLenum, GLsizei, GLenum, const void *, GLsizei), 0x0330, "glDrawElementsInstancedARB")
	GLCOREFUNC(void, glVertexAttribDivisor, (GLuint, GLuint), 0x0330, "glVertexAttribDivisorARB")
	
	// timer_query
	#define GL_TIME_ELAPSED											0x88BF
	#define GL_TIMESTAMP											0x8E28
	
	GLCOREFUNC(void, glQueryCounter, (GLuint, GLenum), 0x0330, "glQueryCounter")
	GLCOREFUNC(void, glGetQueryObjecti64v, (GLuint, GLenum, GLint64 *), 0x0330, "glGetQueryObjecti64v")
	GLCOREFUNC(void, glGetQueryObjectui64v, (GLuint, GLenum, GLuint64 *), 0x0330, "glGetQueryObjectui64v")
	
	
	// -------------------------------------------------------------------
	//
	// OpenGL 4.0 Core Features
	//
	// -------------------------------------------------------------------
	
	// sample_shading
	#define GL_SAMPLE_SHADING										0x8C36
	#define GL_MIN_SAMPLE_SHADING_VALUE								0x8C37
	
	GLCOREFUNC(void, glMinSampleShading, (GLclampf), 0x0400, "glMinSampleShadingARB")
	
	// tessellation_shader
	#define GL_PATCHES												14
	#define GL_PATCH_VERTICES										0x8E72
	#define GL_PATCH_DEFAULT_INNER_LEVEL							0x8E73
	#define GL_PATCH_DEFAULT_OUTER_LEVEL							0x8E74
	#define GL_TESS_CONTROL_OUTPUT_VERTICES							0x8E75
	#define GL_TESS_GEN_MODE										0x8E76
	#define GL_TESS_GEN_SPACING										0x8E77
	#define GL_TESS_GEN_VERTEX_ORDER								0x8E78
	#define GL_TESS_GEN_POINT_MODE									0x8E79
	#define GL_ISOLINES												0x8E7A
	#define GL_FRACTIONAL_ODD										0x8E7B
	#define GL_FRACTIONAL_EVEN										0x8E7C
	#define GL_MAX_PATCH_VERTICES									0x8E7D
	#define GL_MAX_TESS_GEN_LEVEL									0x8E7E
	#define GL_MAX_TESS_CONTROL_UNIFORM_COMPONENTS					0x8E7F
	#define GL_MAX_TESS_EVALUATION_UNIFORM_COMPONENTS				0x8E80
	#define GL_MAX_TESS_CONTROL_TEXTURE_IMAGE_UNITS					0x8E81
	#define GL_MAX_TESS_EVALUATION_TEXTURE_IMAGE_UNITS				0x8E82
	#define GL_MAX_TESS_CONTROL_OUTPUT_COMPONENTS					0x8E83
	#define GL_MAX_TESS_PATCH_COMPONENTS							0x8E84
	#define GL_MAX_TESS_CONTROL_TOTAL_OUTPUT_COMPONENTS				0x8E85
	#define GL_MAX_TESS_EVALUATION_OUTPUT_COMPONENTS				0x8E86
	#define GL_MAX_TESS_CONTROL_UNIFORM_BLOCKS						0x8E89
	#define GL_MAX_TESS_EVALUATION_UNIFORM_BLOCKS					0x8E8A
	#define GL_MAX_TESS_CONTROL_INPUT_COMPONENTS					0x886C
	#define GL_MAX_TESS_EVALUATION_INPUT_COMPONENTS					0x886D
	#define GL_MAX_COMBINED_TESS_CONTROL_UNIFORM_COMPONENTS			0x8E1E
	#define GL_MAX_COMBINED_TESS_EVALUATION_UNIFORM_COMPONENTS		0x8E1F
	#define GL_UNIFORM_BLOCK_REFERENCED_BY_TESS_CONTROL_SHADER		0x84F0
	#define GL_UNIFORM_BLOCK_REFERENCED_BY_TESS_EVALUATION_SHADER	0x84F1
	#define GL_TESS_EVALUATION_SHADER								0x8E87
	#define GL_TESS_CONTROL_SHADER									0x8E88
	
	GLCOREFUNC(void, glPatchParameteri, (GLenum, GLint), 0x0400, "glPatchParameteriARB")
	GLCOREFUNC(void, glPatchParameterfv, (GLenum, const GLfloat *), 0x0400, "glPatchParameterfvARB")
	
	
	// -------------------------------------------------------------------
	//
	// OpenGL 4.1 Core Features
	//
	// -------------------------------------------------------------------
	
	// separate_shader_objects
	#define GL_VERTEX_SHADER_BIT									0x00000001
	#define GL_FRAGMENT_SHADER_BIT									0x00000002
	#define GL_GEOMETRY_SHADER_BIT									0x00000004
	#define GL_TESS_CONTROL_SHADER_BIT								0x00000008
	#define GL_TESS_EVALUATION_SHADER_BIT							0x00000010
	#define GL_ALL_SHADER_BITS										0xFFFFFFFF
	#define GL_PROGRAM_SEPARABLE									0x8258
	#define GL_ACTIVE_PROGRAM										0x8259
	
	GLCOREFUNC(void, glUseProgramStages, (GLuint, GLbitfield, GLuint), 0x0410, "glUseProgramStagesARB")
	GLCOREFUNC(void, glActiveShaderProgram, (GLuint, GLuint), 0x0410, "glActiveShaderProgramARB")
	GLCOREFUNC(GLuint, glCreateShaderProgramv, (GLenum, GLsizei, const GLchar **), 0x0410, "glCreateShaderProgramvARB")
	GLCOREFUNC(void, glBindProgramPipeline, (GLuint), 0x0410, "glBindProgramPipelineARB")
	GLCOREFUNC(void, glDeleteProgramPipelines, (GLsizei, const GLuint *), 0x0410, "glDeleteProgramPipelinesARB")
	GLCOREFUNC(void, glGenProgramPipelines, (GLsizei, GLuint *), 0x0410, "glGenProgramPipelinesARB")
	GLCOREFUNC(GLboolean, glIsProgramPipeline, (GLuint), 0x0410, "glIsProgramPipelineARB")
	GLCOREFUNC(void, glProgramParameteri, (GLuint, GLenum, GLint), 0x0410, "glProgramParameteriARB")
	GLCOREFUNC(void, glGetProgramPipelineiv, (GLuint, GLenum, GLint *), 0x0410, "glGetProgramPipelineivARB")
	GLCOREFUNC(void, glProgramUniform1i, (GLuint, GLint, GLint), 0x0410, "glProgramUniform1iARB")
	GLCOREFUNC(void, glProgramUniform2i, (GLuint, GLint, GLint, GLint), 0x0410, "glProgramUniform2iARB")
	GLCOREFUNC(void, glProgramUniform3i, (GLuint, GLint, GLint, GLint, GLint), 0x0410, "glProgramUniform3iARB")
	GLCOREFUNC(void, glProgramUniform4i, (GLuint, GLint, GLint, GLint, GLint, GLint), 0x0410, "glProgramUniform4iARB")
	GLCOREFUNC(void, glProgramUniform1ui, (GLuint, GLint, GLuint), 0x0410, "glProgramUniform1uiARB")
	GLCOREFUNC(void, glProgramUniform2ui, (GLuint, GLint, GLuint, GLuint), 0x0410, "glProgramUniform2uiARB")
	GLCOREFUNC(void, glProgramUniform3ui, (GLuint, GLint, GLuint, GLuint, GLuint), 0x0410, "glProgramUniform3uiARB")
	GLCOREFUNC(void, glProgramUniform4ui, (GLuint, GLint,  GLuint, GLuint, GLuint, GLuint), 0x0410, "glProgramUniform4uiARB")
	GLCOREFUNC(void, glProgramUniform1f, (GLuint, GLint, GLfloat), 0x0410, "glProgramUniform1fARB")
	GLCOREFUNC(void, glProgramUniform2f, (GLuint, GLint, GLfloat, GLfloat), 0x0410, "glProgramUniform2fARB")
	GLCOREFUNC(void, glProgramUniform3f, (GLuint, GLint, GLfloat, GLfloat, GLfloat), 0x0410, "glProgramUniform3fARB")
	GLCOREFUNC(void, glProgramUniform4f, (GLuint, GLint, GLfloat, GLfloat, GLfloat, GLfloat), 0x0410, "glProgramUniform4fARB")
	GLCOREFUNC(void, glProgramUniform1iv, (GLuint, GLint, GLsizei, const GLint *), 0x0410, "glProgramUniform1ivARB")
	GLCOREFUNC(void, glProgramUniform2iv, (GLuint, GLint, GLsizei, const GLint *), 0x0410, "glProgramUniform2ivARB")
	GLCOREFUNC(void, glProgramUniform3iv, (GLuint, GLint, GLsizei, const GLint *), 0x0410, "glProgramUniform3ivARB")
	GLCOREFUNC(void, glProgramUniform4iv, (GLuint, GLint, GLsizei, const GLint *), 0x0410, "glProgramUniform4ivARB")
	GLCOREFUNC(void, glProgramUniform1uiv, (GLuint, GLint, GLsizei, const GLuint *), 0x0410, "glProgramUniform1uivARB")
	GLCOREFUNC(void, glProgramUniform2uiv, (GLuint, GLint, GLsizei, const GLuint *), 0x0410, "glProgramUniform2uivARB")
	GLCOREFUNC(void, glProgramUniform3uiv, (GLuint, GLint, GLsizei, const GLuint *), 0x0410, "glProgramUniform3uivARB")
	GLCOREFUNC(void, glProgramUniform4uiv, (GLuint, GLint, GLsizei, const GLuint *), 0x0410, "glProgramUniform4uivARB")
	GLCOREFUNC(void, glProgramUniform1fv, (GLuint, GLint, GLsizei, const GLfloat *), 0x0410, "glProgramUniform1fvARB")
	GLCOREFUNC(void, glProgramUniform2fv, (GLuint, GLint, GLsizei, const GLfloat *), 0x0410, "glProgramUniform2fvARB")
	GLCOREFUNC(void, glProgramUniform3fv, (GLuint, GLint, GLsizei, const GLfloat *), 0x0410, "glProgramUniform3fvARB")
	GLCOREFUNC(void, glProgramUniform4fv, (GLuint, GLint, GLsizei, const GLfloat *), 0x0410, "glProgramUniform4fvARB")
	GLCOREFUNC(void, glValidateProgramPipeline, (GLuint), 0x0410, "glValidateProgramPipelineARB")
	GLCOREFUNC(void, glGetProgramPipelineInfoLog, (GLuint, GLsizei, GLsizei *, GLchar *), 0x0410, "glGetProgramPipelineInfoLogARB")
	
	// get_program_binary
	#define GL_PROGRAM_BINARY_RETRIEVABLE_HINT						0x8257
	#define GL_PROGRAM_BINARY_LENGTH								0x8741
	#define GL_NUM_PROGRAM_BINARY_FORMATS							0x87FE
	#define GL_PROGRAM_BINARY_FORMATS								0x87FF
	
	GLCOREFUNC(void, glGetProgramBinary, (GLuint, GLsizei, GLsizei *, GLenum *, void *), 0x0410, "glGetProgramBinaryARB")
	GLCOREFUNC(void, glProgramBinary, (GLuint, GLenum, const void *, GLsizei), 0x0410, "glProgramBinaryARB")
	
	
	// -------------------------------------------------------------------
	//
	// OpenGL Extensions
	//
	// -------------------------------------------------------------------
	
	#if C4DEBUG
	
		// GL_ARB_debug_context
		#define GL_DEBUG_OUTPUT_SYNCHRONOUS_ARB                     0x8242
		#define GL_MAX_DEBUG_MESSAGE_LENGTH_ARB                     0x9143
		#define GL_MAX_DEBUG_LOGGED_MESSAGES_ARB                    0x9144
		#define GL_DEBUG_LOGGED_MESSAGES_ARB                        0x9145
		#define GL_DEBUG_NEXT_LOGGED_MESSAGE_LENGTH_ARB             0x8243
		#define GL_DEBUG_CALLBACK_FUNCTION_ARB                      0x8244
		#define GL_DEBUG_CALLBACK_USER_PARAM_ARB                    0x8245
		#define GL_DEBUG_SOURCE_API_ARB                             0x8246
		#define GL_DEBUG_SOURCE_WINDOW_SYSTEM_ARB                   0x8247
		#define GL_DEBUG_SOURCE_SHADER_COMPILER_ARB                 0x8248
		#define GL_DEBUG_SOURCE_THIRD_PARTY_ARB                     0x8249
		#define GL_DEBUG_SOURCE_APPLICATION_ARB                     0x824A
		#define GL_DEBUG_SOURCE_OTHER_ARB                           0x824B
		#define GL_DEBUG_TYPE_ERROR_ARB                             0x824C
		#define GL_DEBUG_TYPE_DEPRECATED_BEHAVIOR_ARB               0x824D
		#define GL_DEBUG_TYPE_UNDEFINED_BEHAVIOR_ARB                0x824E
		#define GL_DEBUG_TYPE_PORTABILITY_ARB                       0x824F
		#define GL_DEBUG_TYPE_PERFORMANCE_ARB                       0x8250
		#define GL_DEBUG_TYPE_OTHER_ARB                             0x8251
		#define GL_DEBUG_SEVERITY_HIGH_ARB                          0x9146
		#define GL_DEBUG_SEVERITY_MEDIUM_ARB                        0x9147
		#define GL_DEBUG_SEVERITY_LOW_ARB                           0x9148
		
		typedef void (OPENGLAPI *GLdebugProcARB)(GLenum, GLenum, GLuint, GLenum, GLsizei, const GLchar *, void *);
		
		GLEXTFUNC(void, glDebugMessageControlARB, (GLenum, GLenum, GLenum, GLsizei, const GLuint *, GLboolean))
		GLEXTFUNC(void, glDebugMessageInsertARB, (GLenum, GLenum, GLuint, GLenum, GLsizei, const GLchar *))
		GLEXTFUNC(void, glDebugMessageCallbackARB, (GLdebugProcARB, void *))
		GLEXTFUNC(GLuint, glGetDebugMessageLogARB, (GLuint, GLsizei, GLenum *, GLenum *, GLuint *, GLenum *, GLsizei *, GLchar *))
		GLEXTFUNC(void, glGetPointerv, (GLenum, void **))

	#endif
	
	// GL_ARB_fragment_program
	#define GL_FRAGMENT_PROGRAM_ARB									0x8804
	#define GL_PROGRAM_ALU_INSTRUCTIONS_ARB							0x8805
	#define GL_PROGRAM_TEX_INSTRUCTIONS_ARB							0x8806
	#define GL_PROGRAM_TEX_INDIRECTIONS_ARB							0x8807
	#define GL_PROGRAM_NATIVE_ALU_INSTRUCTIONS_ARB					0x8808
	#define GL_PROGRAM_NATIVE_TEX_INSTRUCTIONS_ARB					0x8809
	#define GL_PROGRAM_NATIVE_TEX_INDIRECTIONS_ARB					0x880A
	#define GL_MAX_PROGRAM_ALU_INSTRUCTIONS_ARB						0x880B
	#define GL_MAX_PROGRAM_TEX_INSTRUCTIONS_ARB						0x880C
	#define GL_MAX_PROGRAM_TEX_INDIRECTIONS_ARB						0x880D
	#define GL_MAX_PROGRAM_NATIVE_ALU_INSTRUCTIONS_ARB				0x880E
	#define GL_MAX_PROGRAM_NATIVE_TEX_INSTRUCTIONS_ARB				0x880F
	#define GL_MAX_PROGRAM_NATIVE_TEX_INDIRECTIONS_ARB				0x8810
	#define GL_MAX_TEXTURE_COORDS									0x8871
	#define GL_MAX_TEXTURE_IMAGE_UNITS								0x8872
	
	// GL_ARB_vertex_program
	#define GL_VERTEX_PROGRAM_ARB									0x8620
	#define GL_VERTEX_PROGRAM_POINT_SIZE							0x8642
	#define GL_VERTEX_PROGRAM_TWO_SIDE								0x8643
	#define GL_COLOR_SUM_ARB										0x8458
	#define GL_PROGRAM_FORMAT_ASCII_ARB								0x8875
	#define GL_VERTEX_ATTRIB_ARRAY_ENABLED							0x8622
	#define GL_VERTEX_ATTRIB_ARRAY_SIZE								0x8623
	#define GL_VERTEX_ATTRIB_ARRAY_STRIDE							0x8624
	#define GL_VERTEX_ATTRIB_ARRAY_TYPE								0x8625
	#define GL_VERTEX_ATTRIB_ARRAY_NORMALIZED						0x886A
	#define GL_CURRENT_VERTEX_ATTRIB								0x8626
	#define GL_VERTEX_ATTRIB_ARRAY_POINTER							0x8645
	#define GL_PROGRAM_LENGTH_ARB									0x8627
	#define GL_PROGRAM_FORMAT_ARB									0x8876
	#define GL_PROGRAM_INSTRUCTIONS_ARB								0x88A0
	#define GL_MAX_PROGRAM_INSTRUCTIONS_ARB							0x88A1
	#define GL_PROGRAM_NATIVE_INSTRUCTIONS_ARB						0x88A2
	#define GL_MAX_PROGRAM_NATIVE_INSTRUCTIONS_ARB					0x88A3
	#define GL_PROGRAM_TEMPORARIES_ARB								0x88A4
	#define GL_MAX_PROGRAM_TEMPORARIES_ARB							0x88A5
	#define GL_PROGRAM_NATIVE_TEMPORARIES_ARB						0x88A6
	#define GL_MAX_PROGRAM_NATIVE_TEMPORARIES_ARB					0x88A7
	#define GL_PROGRAM_PARAMETERS_ARB								0x88A8
	#define GL_MAX_PROGRAM_PARAMETERS_ARB							0x88A9
	#define GL_PROGRAM_NATIVE_PARAMETERS_ARB						0x88AA
	#define GL_MAX_PROGRAM_NATIVE_PARAMETERS_ARB					0x88AB
	#define GL_PROGRAM_ATTRIBS_ARB									0x88AC
	#define GL_MAX_PROGRAM_ATTRIBS_ARB								0x88AD
	#define GL_PROGRAM_NATIVE_ATTRIBS_ARB							0x88AE
	#define GL_MAX_PROGRAM_NATIVE_ATTRIBS_ARB						0x88AF
	#define GL_PROGRAM_ADDRESS_REGISTERS_ARB						0x88B0
	#define GL_MAX_PROGRAM_ADDRESS_REGISTERS_ARB					0x88B1
	#define GL_PROGRAM_NATIVE_ADDRESS_REGISTERS_ARB					0x88B2
	#define GL_MAX_PROGRAM_NATIVE_ADDRESS_REGISTERS_ARB				0x88B3
	#define GL_MAX_PROGRAM_LOCAL_PARAMETERS_ARB						0x88B4
	#define GL_MAX_PROGRAM_ENV_PARAMETERS_ARB						0x88B5
	#define GL_PROGRAM_UNDER_NATIVE_LIMITS_ARB						0x88B6
	#define GL_PROGRAM_STRING_ARB									0x8628
	#define GL_PROGRAM_ERROR_POSITION_ARB							0x864B
	#define GL_CURRENT_MATRIX_ARB									0x8641
	#define GL_TRANSPOSE_CURRENT_MATRIX_ARB							0x88B7
	#define GL_CURRENT_MATRIX_STACK_DEPTH_ARB						0x8640
	#define GL_MAX_VERTEX_ATTRIBS									0x8869
	#define GL_MAX_PROGRAM_MATRICES_ARB								0x862F
	#define GL_MAX_PROGRAM_MATRIX_STACK_DEPTH_ARB					0x862E
	#define GL_PROGRAM_ERROR_STRING_ARB								0x8874
	
	GLEXTFUNC(void, glVertexAttrib1fARB, (GLuint, GLfloat))
	GLEXTFUNC(void, glVertexAttrib2fARB, (GLuint, GLfloat, GLfloat))
	GLEXTFUNC(void, glVertexAttrib3fARB, (GLuint, GLfloat, GLfloat, GLfloat))
	GLEXTFUNC(void, glVertexAttrib4fARB, (GLuint, GLfloat, GLfloat, GLfloat, GLfloat))
	GLEXTFUNC(void, glVertexAttrib4NubARB, (GLuint, GLubyte, GLubyte, GLubyte, GLubyte))
	GLEXTFUNC(void, glVertexAttrib1fvARB, (GLuint, const GLfloat *))
	GLEXTFUNC(void, glVertexAttrib2fvARB, (GLuint, const GLfloat *))
	GLEXTFUNC(void, glVertexAttrib3fvARB, (GLuint, const GLfloat *))
	GLEXTFUNC(void, glVertexAttrib4ubvARB, (GLuint, const GLubyte *))
	GLEXTFUNC(void, glVertexAttrib4fvARB, (GLuint, const GLfloat *))
	GLEXTFUNC(void, glVertexAttrib4NubvARB, (GLuint, const GLubyte *))
	GLEXTFUNC(void, glVertexAttribPointerARB, (GLuint, GLint, GLenum, GLboolean, GLsizei, const void *))
	GLEXTFUNC(void, glEnableVertexAttribArrayARB, (GLuint))
	GLEXTFUNC(void, glDisableVertexAttribArrayARB, (GLuint))
	GLEXTFUNC(void, glProgramStringARB, (GLenum, GLenum, GLsizei, const void *))
	GLEXTFUNC(void, glBindProgramARB, (GLenum, GLuint))
	GLEXTFUNC(void, glDeleteProgramsARB, (GLsizei, const GLuint *))
	GLEXTFUNC(void, glGenProgramsARB, (GLsizei, GLuint *))
	GLEXTFUNC(void, glProgramEnvParameter4fARB, (GLenum, GLuint, GLfloat, GLfloat, GLfloat, GLfloat))
	GLEXTFUNC(void, glProgramEnvParameter4fvARB, (GLenum, GLuint, const GLfloat *))
	GLEXTFUNC(void, glProgramLocalParameter4fARB, (GLenum, GLuint, GLfloat, GLfloat, GLfloat, GLfloat))
	GLEXTFUNC(void, glProgramLocalParameter4fvARB, (GLenum, GLuint, const GLfloat *))
	GLEXTFUNC(void, glGetProgramivARB, (GLenum, GLenum, GLint *))
	
	// GL_EXT_depth_bounds_test
	#define GL_DEPTH_BOUNDS_TEST_EXT								0x8890
	#define GL_DEPTH_BOUNDS_EXT										0x8891
	
	GLEXTFUNC(void, glDepthBoundsEXT, (GLclampd, GLclampd))
	
	// GL_EXT_direct_state_access
	GLEXTFUNC(void, glEnableClientStateIndexedEXT, (GLenum, GLuint))
	GLEXTFUNC(void, glDisableClientStateIndexedEXT, (GLenum, GLuint))
	GLEXTFUNC(void, glBindMultiTextureEXT, (GLenum, GLenum, GLuint))
	GLEXTFUNC(void, glTextureParameteriEXT, (GLuint, GLenum, GLenum, GLint))
	GLEXTFUNC(void, glTextureParameterivEXT, (GLuint, GLenum, GLenum, const GLint *))
	GLEXTFUNC(void, glTextureParameterfEXT, (GLuint, GLenum, GLenum, GLfloat))
	GLEXTFUNC(void, glTextureParameterfvEXT, (GLuint, GLenum, GLenum, const GLfloat *))
	GLEXTFUNC(void, glMultiTexParameteriEXT, (GLenum, GLenum, GLenum, GLint))
	GLEXTFUNC(void, glMultiTexParameterivEXT, (GLenum, GLenum, GLenum, const GLint *))
	GLEXTFUNC(void, glMultiTexParameterfEXT, (GLenum, GLenum, GLenum, GLfloat))
	GLEXTFUNC(void, glMultiTexParameterfvEXT, (GLenum, GLenum, GLenum, const GLfloat *))
	GLEXTFUNC(void, glTextureImage2DEXT, (GLuint, GLenum, GLint, GLint, GLsizei, GLsizei, GLint, GLenum, GLenum, const void *))
	GLEXTFUNC(void, glTextureSubImage2DEXT, (GLuint, GLenum, GLint, GLint, GLint, GLsizei, GLsizei, GLenum, GLenum, const void *))
	GLEXTFUNC(void, glTextureImage3DEXT, (GLuint, GLenum, GLint, GLint, GLsizei, GLsizei, GLsizei, GLint, GLenum, GLenum, const void *))
	GLEXTFUNC(void, glCompressedTextureImage2DEXT, (GLuint, GLenum, GLint, GLenum, GLsizei, GLsizei, GLint, GLsizei, const void *))
	GLEXTFUNC(void, glCompressedTextureImage3DEXT, (GLuint, GLenum, GLint, GLenum, GLsizei, GLsizei, GLsizei, GLint, GLsizei, const void *))
	GLEXTFUNC(void, glCopyTextureSubImage2DEXT, (GLuint, GLenum, GLint, GLint, GLint, GLint, GLint, GLsizei, GLsizei))
	GLEXTFUNC(void, glNamedProgramStringEXT, (GLuint, GLenum, GLenum, GLsizei, const void *))
	GLEXTFUNC(void, glNamedBufferDataEXT, (GLuint, GLsizeiptr, const void *, GLenum))
	GLEXTFUNC(void, glNamedBufferSubDataEXT, (GLuint, GLintptr, GLsizeiptr, const void *))
	GLEXTFUNC(void *, glMapNamedBufferEXT, (GLuint, GLenum))
	GLEXTFUNC(GLboolean, glUnmapNamedBufferEXT, (GLuint))
	GLEXTFUNC(void, glProgramUniform4fEXT, (GLuint, GLint, GLfloat, GLfloat, GLfloat, GLfloat))
	GLEXTFUNC(void, glProgramUniform4fvEXT, (GLuint, GLint, GLsizei, const GLfloat *))
	GLEXTFUNC(void, glNamedRenderbufferStorageEXT, (GLuint, GLenum, GLsizei, GLsizei))
	GLEXTFUNC(void, glNamedRenderbufferStorageMultisampleEXT, (GLuint, GLsizei, GLenum, GLsizei, GLsizei))
	GLEXTFUNC(void, glNamedRenderbufferStorageMultisampleCoverageEXT, (GLuint, GLsizei, GLsizei, GLenum, GLsizei, GLsizei))
	GLEXTFUNC(GLenum, glCheckNamedFramebufferStatusEXT, (GLuint, GLenum))
	GLEXTFUNC(void, glNamedFramebufferTexture2DEXT, (GLuint, GLenum, GLenum, GLuint, GLint))
	GLEXTFUNC(void, glNamedFramebufferTexture3DEXT, (GLuint, GLenum, GLenum, GLuint, GLint, GLint))
	GLEXTFUNC(void, glNamedFramebufferRenderbufferEXT, (GLuint, GLenum, GLenum, GLuint))
	GLEXTFUNC(void, glFramebufferDrawBufferEXT, (GLuint, GLenum))
	GLEXTFUNC(void, glFramebufferDrawBuffersEXT, (GLuint, GLsizei, const GLenum *))
	GLEXTFUNC(void, glFramebufferReadBufferEXT, (GLuint, GLenum))
	GLEXTFUNC(void, glNamedFramebufferTextureEXT, (GLuint, GLenum, GLuint, GLint))
	GLEXTFUNC(void, glNamedFramebufferTextureLayerEXT, (GLuint, GLenum, GLuint, GLint, GLint))
	GLEXTFUNC(void, glNamedFramebufferTextureFaceEXT, (GLuint, GLenum, GLuint, GLint, GLenum))
	GLEXTFUNC(void, glTextureRenderbufferEXT, (GLuint, GLenum, GLuint))
	
	// GL_EXT_texture_compression_s3tc
	#define GL_COMPRESSED_RGB_S3TC_DXT1_EXT							0x83F0
	#define GL_COMPRESSED_RGBA_S3TC_DXT1_EXT						0x83F1
	#define GL_COMPRESSED_RGBA_S3TC_DXT3_EXT						0x83F2
	#define GL_COMPRESSED_RGBA_S3TC_DXT5_EXT						0x83F3
	
	// GL_EXT_texture_filter_anisotropic
	#define GL_TEXTURE_MAX_ANISOTROPY_EXT							0x84FE
	#define GL_MAX_TEXTURE_MAX_ANISOTROPY_EXT						0x84FF
	
	// GL_EXT_texture_mirror_clamp
	#define GL_MIRROR_CLAMP_EXT										0x8742
	#define GL_MIRROR_CLAMP_TO_EDGE_EXT								0x8743
	#define GL_MIRROR_CLAMP_TO_BORDER_EXT							0x8912
	
	// GL_NV_explicit_multisample
	#define GL_SAMPLE_POSITION_NV									0x8E50
	#define GL_SAMPLE_MASK_NV										0x8E51
	#define GL_SAMPLE_MASK_VALUE_NV									0x8E52
	#define GL_MAX_SAMPLE_MASK_WORDS_NV								0x8E59
	#define GL_TEXTURE_RENDERBUFFER_NV								0x8E55
	#define GL_SAMPLER_RENDERBUFFER_NV								0x8E56
	#define GL_INT_SAMPLER_RENDERBUFFER_NV							0x8E57
	#define GL_UNSIGNED_INT_SAMPLER_RENDERBUFFER_NV					0x8E58
	
	GLEXTFUNC(void, glGetBooleanIndexedvEXT, (GLenum, GLuint, GLboolean *))
	GLEXTFUNC(void, glGetIntegerIndexedvEXT, (GLenum, GLuint, GLint *))
	GLEXTFUNC(void, glGetMultisamplefvNV, (GLenum, GLuint, GLfloat *))
	GLEXTFUNC(void, glSampleMaskIndexedNV, (GLuint, GLbitfield))
	GLEXTFUNC(void, glTexRenderbufferNV, (GLenum, GLuint))
	
	// GL_NV_fragment_program2
	#define GL_MAX_PROGRAM_EXEC_INSTRUCTIONS_NV						0x88F4
	#define GL_MAX_PROGRAM_CALL_DEPTH_NV							0x88F5
	#define GL_MAX_PROGRAM_IF_DEPTH_NV								0x88F6
	#define GL_MAX_PROGRAM_LOOP_DEPTH_NV							0x88F7
	#define GL_MAX_PROGRAM_LOOP_COUNT_NV							0x88F8
	
	// GL_NV_framebuffer_multisample_coverage
	#define GL_RENDERBUFFER_COVERAGE_SAMPLES_NV						0x8CAB
	#define GL_RENDERBUFFER_COLOR_SAMPLES_NV						0x8E10
	#define GL_MAX_MULTISAMPLE_COVERAGE_MODES_NV					0x8E11
	#define GL_MULTISAMPLE_COVERAGE_MODES_NV						0x8E12
	
	GLEXTFUNC(void, glRenderbufferStorageMultisampleCoverageNV, (GLenum, GLsizei, GLsizei, GLenum, GLsizei, GLsizei))
	
	// GL_NV_gpu_program4
	#define GL_VERTEX_ATTRIB_ARRAY_INTEGER_NV						0x88FD
	#define GL_MIN_PROGRAM_TEXEL_OFFSET_NV							0x8904
	#define GL_MAX_PROGRAM_TEXEL_OFFSET_NV							0x8905
	#define GL_PROGRAM_ATTRIB_COMPONENTS_NV							0x8906
	#define GL_PROGRAM_RESULT_COMPONENTS_NV							0x8907
	#define GL_MAX_PROGRAM_ATTRIB_COMPONENTS_NV						0x8908
	#define GL_MAX_PROGRAM_RESULT_COMPONENTS_NV						0x8909
	#define GL_GEOMETRY_PROGRAM_NV									0x8C26
	#define GL_MAX_PROGRAM_OUTPUT_VERTICES_NV						0x8C27
	#define GL_MAX_PROGRAM_TOTAL_OUTPUT_COMPONENTS_NV				0x8C28
	#define GL_MAX_GEOMETRY_TEXTURE_IMAGE_UNITS_NV					0x8C29
	#define GL_MAX_PROGRAM_GENERIC_ATTRIBS_NV						0x8DA5
	#define GL_MAX_PROGRAM_GENERIC_RESULTS_NV						0x8DA6
	#define GL_FRAMEBUFFER_ATTACHMENT_LAYERED_NV					0x8DA7
	#define GL_FRAMEBUFFER_INCOMPLETE_LAYER_TARGETS_NV				0x8DA8
	#define GL_FRAMEBUFFER_INCOMPLETE_LAYER_COUNT_NV				0x8DA9
	#define GL_PROGRAM_VERTEX_LIMIT_NV								0x8DAA
	#define GL_LINES_ADJACENCY										10
	#define GL_LINE_STRIP_ADJACENCY									11
	#define GL_TRIANGLES_ADJACENCY									12
	#define GL_TRIANGLE_STRIP_ADJACENCY								13
	
	GLEXTFUNC(void, glProgramLocalParameterI4iNV, (GLenum, GLuint, GLint, GLint, GLint, GLint))
	GLEXTFUNC(void, glProgramLocalParameterI4ivNV, (GLenum, GLuint, const GLint *))
	GLEXTFUNC(void, glProgramLocalParameterI4uiNV, (GLenum, GLuint, GLuint, GLuint, GLuint, GLuint))
	GLEXTFUNC(void, glProgramLocalParameterI4uivNV, (GLenum, GLuint, const GLuint *))
	GLEXTFUNC(void, glProgramVertexLimitNV, (GLenum, GLint))
	GLEXTFUNC(void, glVertexAttribI1iNV, (GLuint, GLint))
	GLEXTFUNC(void, glVertexAttribI2iNV, (GLuint, GLint, GLint))
	GLEXTFUNC(void, glVertexAttribI3iNV, (GLuint, GLint, GLint, GLint))
	GLEXTFUNC(void, glVertexAttribI4iNV, (GLuint, GLint, GLint, GLint, GLint))
	GLEXTFUNC(void, glVertexAttribI1uiNV, (GLuint, GLuint))
	GLEXTFUNC(void, glVertexAttribI2uiNV, (GLuint, GLuint, GLuint))
	GLEXTFUNC(void, glVertexAttribI3uiNV, (GLuint, GLuint, GLuint, GLuint))
	GLEXTFUNC(void, glVertexAttribI4uiNV, (GLuint, GLuint, GLuint, GLuint, GLuint))
	GLEXTFUNC(void, glVertexAttribI1ivNV, (GLuint, const GLint *))
	GLEXTFUNC(void, glVertexAttribI2ivNV, (GLuint, const GLint *))
	GLEXTFUNC(void, glVertexAttribI3ivNV, (GLuint, const GLint *))
	GLEXTFUNC(void, glVertexAttribI4ivNV, (GLuint, const GLint *))
	GLEXTFUNC(void, glVertexAttribI1uivNV, (GLuint, const GLuint *))
	GLEXTFUNC(void, glVertexAttribI2uivNV, (GLuint, const GLuint *))
	GLEXTFUNC(void, glVertexAttribI3uivNV, (GLuint, const GLuint *))
	GLEXTFUNC(void, glVertexAttribI4uivNV, (GLuint, const GLuint *))
	GLEXTFUNC(void, glVertexAttribI4bvNV, (GLuint, const GLbyte *))
	GLEXTFUNC(void, glVertexAttribI4svNV, (GLuint, const GLshort *))
	GLEXTFUNC(void, glVertexAttribI4ubvNV, (GLuint, const GLubyte *))
	GLEXTFUNC(void, glVertexAttribI4usvNV, (GLuint, const GLushort *))
	GLEXTFUNC(void, glVertexAttribIPointerNV, (GLuint, GLint, GLenum, GLsizei, const void *))
	
	// GL_NV_shader_buffer_load
	#define GL_BUFFER_GPU_ADDRESS_NV								0x8F1D
	#define GL_GPU_ADDRESS_NV										0x8F34
	#define GL_MAX_SHADER_BUFFER_ADDRESS_NV							0x8F35
	
	GLEXTFUNC(void, glMakeNamedBufferResidentNV, (GLuint, GLenum))
	GLEXTFUNC(void, glMakeNamedBufferNonResidentNV, (GLuint))
	GLEXTFUNC(void, glGetNamedBufferParameterui64vNV, (GLuint, GLenum, GLuint64 *))
	GLEXTFUNC(void, glProgramUniformui64NV, (GLuint, GLint, GLuint64))
	GLEXTFUNC(void, glProgramUniformui64vNV, (GLuint, GLint, GLsizei, const GLuint64 *))
	
	// GL_NV_transform_feedback
	#define GL_TRANSFORM_FEEDBACK_BUFFER							0x8C8E
	#define GL_TRANSFORM_FEEDBACK_BUFFER_START						0x8C84
	#define GL_TRANSFORM_FEEDBACK_BUFFER_SIZE						0x8C85
	#define GL_TRANSFORM_FEEDBACK_RECORD_NV							0x8C86
	#define GL_INTERLEAVED_ATTRIBS									0x8C8C
	#define GL_SEPARATE_ATTRIBS										0x8C8D
	#define GL_PRIMITIVES_GENERATED									0x8C87
	#define GL_TRANSFORM_FEEDBACK_PRIMITIVES_WRITTEN				0x8C88
	#define GL_RASTERIZER_DISCARD									0x8C89
	#define GL_MAX_TRANSFORM_FEEDBACK_INTERLEAVED_COMPONENTS		0x8C8A
	#define GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_ATTRIBS				0x8C8B
	#define GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_COMPONENTS			0x8C80
	#define GL_TRANSFORM_FEEDBACK_ATTRIBS_NV						0x8C7E
	#define GL_ACTIVE_VARYINGS_NV									0x8C81
	#define GL_ACTIVE_VARYING_MAX_LENGTH							0x8C82
	#define GL_TRANSFORM_FEEDBACK_VARYINGS							0x8C83
	#define GL_TRANSFORM_FEEDBACK_BUFFER_MODE						0x8C7F
	#define GL_BACK_PRIMARY_COLOR_NV								0x8C77
	#define GL_BACK_SECONDARY_COLOR_NV								0x8C78
	#define GL_TEXTURE_COORD_NV										0x8C79
	#define GL_CLIP_DISTANCE_NV										0x8C7A
	#define GL_VERTEX_ID_NV											0x8C7B
	#define GL_PRIMITIVE_ID	_NV										0x8C7C
	#define GL_GENERIC_ATTRIB_NV									0x8C7D
	#define GL_LAYER_NV												0x8DAA
	
	GLEXTFUNC(void, glTransformFeedbackAttribsNV, (GLsizei, const GLint *, GLenum))
	GLEXTFUNC(void, glTransformFeedbackVaryingsNV, (GLuint, GLsizei, const GLint *, GLenum))
	GLEXTFUNC(void, glBeginTransformFeedbackNV, (GLenum))
	GLEXTFUNC(void, glEndTransformFeedbackNV, (void))
	GLEXTFUNC(GLint, glGetVaryingLocationNV, (GLuint, const GLchar *))
	GLEXTFUNC(void, glGetActiveVaryingNV, (GLuint, GLuint, GLsizei, GLsizei *, GLsizei *, GLenum *, GLchar *))
	GLEXTFUNC(void, glActiveVaryingNV, (GLuint, const GLchar *))
	
	// GL_NV_vertex_buffer_unified_memory
	#define GL_VERTEX_ATTRIB_ARRAY_UNIFIED_NV						0x8F1E
	#define GL_ELEMENT_ARRAY_UNIFIED_NV								0x8F1F
	#define GL_VERTEX_ATTRIB_ARRAY_ADDRESS_NV						0x8F20
	#define GL_ELEMENT_ARRAY_ADDRESS_NV								0x8F29
	
	GLEXTFUNC(void, glBufferAddressRangeNV, (GLenum, GLuint, GLuint64, GLsizeiptr))
	GLEXTFUNC(void, glVertexAttribFormatNV, (GLuint, GLint, GLenum, GLboolean, GLsizei))
	GLEXTFUNC(void, glVertexAttribIFormatNV, (GLuint, GLint, GLenum, GLsizei))
	GLEXTFUNC(void, glGetIntegerui64i_vNV, (GLenum, GLuint, GLuint64 *))
	
	
	#if C4WINDOWS
	
		// WGL_ARB_extensions_string
		GLEXTFUNC(const char *, wglGetExtensionsStringARB, (HDC))
		
		// WGL_ARB_create_context
		#define WGL_CONTEXT_MAJOR_VERSION_ARB							0x2091
		#define WGL_CONTEXT_MINOR_VERSION_ARB							0x2092
		#define WGL_CONTEXT_LAYER_PLANE_ARB								0x2093
		#define WGL_CONTEXT_FLAGS_ARB									0x2094
		#define WGL_CONTEXT_PROFILE_MASK_ARB							0x9126
		#define WGL_CONTEXT_DEBUG_BIT_ARB								0x0001
		#define WGL_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB					0x0002
		#define WGL_CONTEXT_CORE_PROFILE_BIT_ARB						0x00000001
		#define WGL_CONTEXT_COMPATIBILITY_PROFILE_BIT_ARB				0x00000002
		
		GLEXTFUNC(HGLRC, wglCreateContextAttribsARB, (HDC, HGLRC, const int *))
		
		// WGL_ARB_pixel_format
		#define WGL_NUMBER_PIXEL_FORMATS_ARB							0x2000
		#define WGL_DRAW_TO_WINDOW_ARB									0x2001
		#define WGL_DRAW_TO_BITMAP_ARB									0x2002
		#define WGL_ACCELERATION_ARB									0x2003
		#define WGL_SWAP_METHOD_ARB										0x2007
		#define WGL_SUPPORT_GDI_ARB										0x200F
		#define WGL_SUPPORT_OPENGL_ARB									0x2010
		#define WGL_DOUBLE_BUFFER_ARB									0x2011
		#define WGL_STEREO_ARB											0x2012
		#define WGL_PIXEL_TYPE_ARB										0x2013
		#define WGL_COLOR_BITS_ARB										0x2014
		#define WGL_RED_BITS_ARB										0x2015
		#define WGL_RED_SHIFT_ARB										0x2016
		#define WGL_GREEN_BITS_ARB										0x2017
		#define WGL_GREEN_SHIFT_ARB										0x2018
		#define WGL_BLUE_BITS_ARB										0x2019
		#define WGL_BLUE_SHIFT_ARB										0x201A
		#define WGL_ALPHA_BITS_ARB										0x201B
		#define WGL_ALPHA_SHIFT_ARB										0x201C
		#define WGL_DEPTH_BITS_ARB										0x2022
		#define WGL_STENCIL_BITS_ARB									0x2023
		#define WGL_GENERIC_ACCELERATION_ARB							0x2026
		#define WGL_FULL_ACCELERATION_ARB								0x2027
		#define WGL_SWAP_EXCHANGE_ARB									0x2028
		#define WGL_SWAP_COPY_ARB										0x2029
		#define WGL_SWAP_UNDEFINED_ARB									0x202A
		#define WGL_TYPE_RGBA_ARB										0x202B
		
		GLEXTFUNC(BOOL, wglChoosePixelFormatARB, (HDC, const int *, const FLOAT *, UINT, int *, UINT *))
		
		// WGL_ARB_framebuffer_sRGB
		#define WGL_FRAMEBUFFER_SRGB_CAPABLE_ARB						0x20A9
		
		// WGL_EXT_swap_control
		GLEXTFUNC(BOOL, wglSwapIntervalEXT, (int))
	
	#elif C4LINUX
	
		// GLX_EXT_swap_control
		GLEXTFUNC(void, glXSwapIntervalEXT, (::Display *, GLXDrawable, int))
	
	#endif
	
	
	struct GraphicsCapabilities;
	
	
	#if C4WINDOWS
	
		#define GLGETCOREFUNC(name) GetCoreFuncAddress_##name(version)
		#define GLGETEXTFUNC(name) *reinterpret_cast<PROC *>(&name) = wglGetProcAddress(#name)
		
		void InitializeOpenglExtensions(GraphicsCapabilities *capabilities);
	
	#elif C4LINUX
	
		#define GLGETCOREFUNC(name) GetCoreFuncAddress_##name(version)
		#define GLGETEXTFUNC(name) *reinterpret_cast<void (**)()>(&name) = glXGetProcAddress(reinterpret_cast<const GLubyte *>(#name))
		
		void InitializeOpenglExtensions(GraphicsCapabilities *capabilities);
	
	#elif C4MACOS || C4IOS
	
		#define GLGETCOREFUNC(name) GetCoreFuncAddress_##name(openglBundle, version)
		#define GLGETEXTFUNC(name) *reinterpret_cast<void **>(&name) = Engine::GetBundleFunctionAddress(openglBundle, #name)
		
		void InitializeOpenglExtensions(GraphicsCapabilities *capabilities, CFBundleRef openglBundle);
	
	#endif
}


#endif

// ZYURVUR
