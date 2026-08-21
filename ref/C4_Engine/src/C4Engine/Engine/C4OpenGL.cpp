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


#define C4OpenGL_cpp


#include "C4Graphics.h"

#if C4MACOS

	#include "C4Engine.h"

#endif


using namespace C4;


#if C4WINDOWS

	void *C4::GetCoreFuncAddress(bool core, const char *coreName, const char *extName)
	{
		if (core)
		{
			void *address = wglGetProcAddress(coreName);
			if (address) return (address);
		}
		
		return (wglGetProcAddress(extName));
	}

#elif C4LINUX

	void *C4::GetCoreFuncAddress(bool core, const char *coreName, const char *extName)
	{
		if (core)
		{
			void *address = (void *) glXGetProcAddress(reinterpret_cast<const GLubyte *>(coreName));
			if (address) return (address);
		}
		
		return ((void *) glXGetProcAddress(reinterpret_cast<const GLubyte *>(extName)));
	}

#elif C4MACOS

	void *C4::GetCoreFuncAddress(CFBundleRef bundle, bool core, const char *coreName, const char *extName)
	{
		if (core)
		{
			void *address = Engine::GetBundleFunctionAddress(bundle, coreName);
			if (address) return (address);
		}
		
		return (Engine::GetBundleFunctionAddress(bundle, extName));
	}

#endif


#if C4WINDOWS || C4LINUX

	void C4::InitializeOpenglExtensions(GraphicsCapabilities *capabilities)

#elif C4MACOS || C4IOS

	void C4::InitializeOpenglExtensions(GraphicsCapabilities *capabilities, CFBundleRef openglBundle)

#endif

{
	const bool *extensionFlag = capabilities->extensionFlag;
	unsigned_int32 version = capabilities->openglVersion;
	
	GLGETEXTFUNC(glPointParameterf);
	GLGETEXTFUNC(glPointParameterfv);
	GLGETEXTFUNC(glPointParameteri);
	GLGETEXTFUNC(glPointParameteriv);
	
	GLGETCOREFUNC(glStencilFuncSeparate);
	GLGETCOREFUNC(glStencilOpSeparate);
	
	if (extensionFlag[kExtensionBlendColor])
	{
		GLGETCOREFUNC(glBlendColor);
	}
	
	if ((extensionFlag[kExtensionBlendMinmax]) || (extensionFlag[kExtensionBlendSubtract]))
	{
		GLGETCOREFUNC(glBlendEquation);
	}
	
	if (extensionFlag[kExtensionDrawRangeElements])
	{
		GLGETCOREFUNC(glDrawRangeElements);
	}
	
	if (extensionFlag[kExtensionTexture3D])
	{
		GLGETCOREFUNC(glTexImage3D);
		GLGETCOREFUNC(glTexSubImage3D);
		GLGETCOREFUNC(glCopyTexSubImage3D);
		
		glGetIntegerv(GL_MAX_3D_TEXTURE_SIZE, (GLint *) &capabilities->max3DTextureSize);
	} 
	
	if (extensionFlag[kExtensionMultisample]) 
	{ 
		GLGETCOREFUNC(glSampleCoverage); 
	}
	 
	if (extensionFlag[kExtensionMultitexture])
	{
		GLGETCOREFUNC(glActiveTexture);
		GLGETCOREFUNC(glClientActiveTexture); 
	}
	
	if (extensionFlag[kExtensionTextureCompression])
	{ 
		GLGETCOREFUNC(glCompressedTexImage3D);
		GLGETCOREFUNC(glCompressedTexImage2D);
		GLGETCOREFUNC(glCompressedTexImage1D);
		GLGETCOREFUNC(glCompressedTexSubImage3D);
		GLGETCOREFUNC(glCompressedTexSubImage2D);
		GLGETCOREFUNC(glCompressedTexSubImage1D);
	}
	
	if (extensionFlag[kExtensionBlendFuncSeparate])
	{
		GLGETCOREFUNC(glBlendFuncSeparate);
	}
	
	if (extensionFlag[kExtensionMultiDrawArrays])
	{
		GLGETCOREFUNC(glMultiDrawArrays);
		GLGETCOREFUNC(glMultiDrawElements);
	}
	
	if (extensionFlag[kExtensionOcclusionQuery])
	{
		GLGETCOREFUNC(glGenQueries);
		GLGETCOREFUNC(glDeleteQueries);
		GLGETCOREFUNC(glBeginQuery);
		GLGETCOREFUNC(glEndQuery);
		GLGETCOREFUNC(glGetQueryiv);
		GLGETCOREFUNC(glGetQueryObjectiv);
		GLGETCOREFUNC(glGetQueryObjectuiv);
	}
	
	if (extensionFlag[kExtensionVertexBufferObject])
	{
		GLGETCOREFUNC(glBindBuffer);
		GLGETCOREFUNC(glDeleteBuffers);
		GLGETCOREFUNC(glGenBuffers);
		GLGETCOREFUNC(glBufferData);
		GLGETCOREFUNC(glBufferSubData);
		GLGETCOREFUNC(glMapBuffer);
		GLGETCOREFUNC(glUnmapBuffer);
	}
	
	if (extensionFlag[kExtensionShaderObjects])
	{
		GLGETCOREFUNC(glDeleteObject);
		GLGETCOREFUNC(glGetHandle);
		GLGETCOREFUNC(glDetachObject);
		GLGETCOREFUNC(glCreateShaderObject);
		GLGETCOREFUNC(glShaderSource);
		GLGETCOREFUNC(glCompileShader);
		GLGETCOREFUNC(glCreateProgramObject);
		GLGETCOREFUNC(glAttachObject);
		GLGETCOREFUNC(glLinkProgram);
		GLGETCOREFUNC(glUseProgramObject);
		GLGETCOREFUNC(glValidateProgram);
		GLGETCOREFUNC(glUniform1f);
		GLGETCOREFUNC(glUniform2f);
		GLGETCOREFUNC(glUniform3f);
		GLGETCOREFUNC(glUniform4f);
		GLGETCOREFUNC(glUniform1i);
		GLGETCOREFUNC(glUniform2i);
		GLGETCOREFUNC(glUniform3i);
		GLGETCOREFUNC(glUniform4i);
		GLGETCOREFUNC(glUniform1fv);
		GLGETCOREFUNC(glUniform2fv);
		GLGETCOREFUNC(glUniform3fv);
		GLGETCOREFUNC(glUniform4fv);
		GLGETCOREFUNC(glUniform1iv);
		GLGETCOREFUNC(glUniform2iv);
		GLGETCOREFUNC(glUniform3iv);
		GLGETCOREFUNC(glUniform4iv);
		GLGETCOREFUNC(glUniformMatrix2fv);
		GLGETCOREFUNC(glUniformMatrix3fv);
		GLGETCOREFUNC(glUniformMatrix4fv);
		GLGETCOREFUNC(glGetObjectParameterfv);
		GLGETCOREFUNC(glGetObjectParameteriv);
		GLGETCOREFUNC(glGetInfoLog);
		GLGETCOREFUNC(glGetAttachedObjects);
		GLGETCOREFUNC(glGetUniformLocation);
		GLGETCOREFUNC(glGetActiveUniform);
	}
	
	if (extensionFlag[kExtensionFramebufferBlit])
	{
		GLGETCOREFUNC(glBlitFramebuffer);
	}
	
	if (extensionFlag[kExtensionFramebufferObject])
	{
		GLGETCOREFUNC(glBindRenderbuffer);
		GLGETCOREFUNC(glDeleteRenderbuffers);
		GLGETCOREFUNC(glGenRenderbuffers);
		GLGETCOREFUNC(glRenderbufferStorage);
		GLGETCOREFUNC(glGetRenderbufferParameteriv);
		GLGETCOREFUNC(glBindFramebuffer);
		GLGETCOREFUNC(glDeleteFramebuffers);
		GLGETCOREFUNC(glGenFramebuffers);
		GLGETCOREFUNC(glCheckFramebufferStatus);
		GLGETCOREFUNC(glFramebufferTexture1D);
		GLGETCOREFUNC(glFramebufferTexture2D);
		GLGETCOREFUNC(glFramebufferTexture3D);
		GLGETCOREFUNC(glFramebufferRenderbuffer);
		GLGETCOREFUNC(glGenerateMipmap);
		
		glGetIntegerv(GL_MAX_COLOR_ATTACHMENTS, (GLint *) &capabilities->maxColorAttachments);
		glGetIntegerv(GL_MAX_RENDERBUFFER_SIZE, (GLint *) &capabilities->maxRenderbufferSize);
	}
	
	if (extensionFlag[kExtensionFramebufferMultisample])
	{
		GLGETCOREFUNC(glRenderbufferStorageMultisample);
		
		glGetIntegerv(GL_MAX_SAMPLES, (GLint *) &capabilities->maxMultisampleSamples);
	}
	
	if (extensionFlag[kExtensionTextureArray])
	{
		GLGETCOREFUNC(glFramebufferTexture);
		GLGETCOREFUNC(glFramebufferTextureLayer);
	}
	
	if (extensionFlag[kExtensionUniformBufferObject])
	{
		GLGETCOREFUNC(glGetUniformIndices);
		GLGETCOREFUNC(glGetActiveUniformsiv);
		GLGETCOREFUNC(glGetActiveUniformName);
		GLGETCOREFUNC(glGetUniformBlockIndex);
		GLGETCOREFUNC(glGetActiveUniformBlockiv);
		GLGETCOREFUNC(glGetActiveUniformBlockName);
		GLGETCOREFUNC(glBindBufferRange);
		GLGETCOREFUNC(glBindBufferBase);
		GLGETCOREFUNC(glUniformBlockBinding);
	}
	
	if (extensionFlag[kExtensionInstancedArrays])
	{
		GLGETCOREFUNC(glDrawElementsInstanced);
		GLGETCOREFUNC(glVertexAttribDivisor);
	}
	
	if (extensionFlag[kExtensionTimerQuery])
	{
		GLGETCOREFUNC(glGetQueryObjecti64v);
		GLGETCOREFUNC(glGetQueryObjectui64v);
	}
	
	if (extensionFlag[kExtensionSampleShading])
	{
		GLGETCOREFUNC(glMinSampleShading);
	}
	
	if (extensionFlag[kExtensionTessellationShader])
	{
		GLGETCOREFUNC(glPatchParameteri);
		GLGETCOREFUNC(glPatchParameterfv);
	}
	
	if (extensionFlag[kExtensionSeparateShaderObjects])
	{
		GLGETCOREFUNC(glUseProgramStages);
		GLGETCOREFUNC(glActiveShaderProgram);
		GLGETCOREFUNC(glCreateShaderProgramv);
		GLGETCOREFUNC(glBindProgramPipeline);
		GLGETCOREFUNC(glDeleteProgramPipelines);
		GLGETCOREFUNC(glGenProgramPipelines);
		GLGETCOREFUNC(glIsProgramPipeline);
		GLGETCOREFUNC(glProgramParameteri);
		GLGETCOREFUNC(glGetProgramPipelineiv);
		GLGETCOREFUNC(glProgramUniform1i);
		GLGETCOREFUNC(glProgramUniform2i);
		GLGETCOREFUNC(glProgramUniform3i);
		GLGETCOREFUNC(glProgramUniform4i);
		GLGETCOREFUNC(glProgramUniform1ui);
		GLGETCOREFUNC(glProgramUniform2ui);
		GLGETCOREFUNC(glProgramUniform3ui);
		GLGETCOREFUNC(glProgramUniform4ui);
		GLGETCOREFUNC(glProgramUniform1f);
		GLGETCOREFUNC(glProgramUniform2f);
		GLGETCOREFUNC(glProgramUniform3f);
		GLGETCOREFUNC(glProgramUniform4f);
		GLGETCOREFUNC(glProgramUniform1iv);
		GLGETCOREFUNC(glProgramUniform2iv);
		GLGETCOREFUNC(glProgramUniform3iv);
		GLGETCOREFUNC(glProgramUniform4iv);
		GLGETCOREFUNC(glProgramUniform1uiv);
		GLGETCOREFUNC(glProgramUniform2uiv);
		GLGETCOREFUNC(glProgramUniform3uiv);
		GLGETCOREFUNC(glProgramUniform4uiv);
		GLGETCOREFUNC(glProgramUniform1fv);
		GLGETCOREFUNC(glProgramUniform2fv);
		GLGETCOREFUNC(glProgramUniform3fv);
		GLGETCOREFUNC(glProgramUniform4fv);
		GLGETCOREFUNC(glValidateProgramPipeline);
		GLGETCOREFUNC(glGetProgramPipelineInfoLog);
	}
	
	if (extensionFlag[kExtensionGetProgramBinary])
	{
		GLGETCOREFUNC(glGetProgramBinary);
		GLGETCOREFUNC(glProgramBinary);
	}
	
	if (extensionFlag[kExtensionFragmentProgram])
	{
		GLGETEXTFUNC(glProgramStringARB);
		GLGETEXTFUNC(glBindProgramARB);
		GLGETEXTFUNC(glDeleteProgramsARB);
		GLGETEXTFUNC(glGenProgramsARB);
		GLGETEXTFUNC(glProgramEnvParameter4fARB);
		GLGETEXTFUNC(glProgramEnvParameter4fvARB);
		GLGETEXTFUNC(glProgramLocalParameter4fARB);
		GLGETEXTFUNC(glProgramLocalParameter4fvARB);
		GLGETEXTFUNC(glGetProgramivARB);
		
		glGetIntegerv(GL_MAX_TEXTURE_COORDS, (GLint *) &capabilities->maxTextureCoordCount);
		glGetIntegerv(GL_MAX_TEXTURE_IMAGE_UNITS, (GLint *) &capabilities->maxTextureImageCount);
		glGetProgramivARB(GL_FRAGMENT_PROGRAM_ARB, GL_MAX_PROGRAM_NATIVE_ALU_INSTRUCTIONS_ARB, (GLint *) &capabilities->maxFragmentProgramALUInstructionCount);
		glGetProgramivARB(GL_FRAGMENT_PROGRAM_ARB, GL_MAX_PROGRAM_NATIVE_TEX_INSTRUCTIONS_ARB, (GLint *) &capabilities->maxFragmentProgramTEXInstructionCount);
		glGetProgramivARB(GL_FRAGMENT_PROGRAM_ARB, GL_MAX_PROGRAM_NATIVE_TEX_INDIRECTIONS_ARB, (GLint *) &capabilities->maxFragmentProgramTEXIndirectionCount);
		glGetProgramivARB(GL_FRAGMENT_PROGRAM_ARB, GL_MAX_PROGRAM_NATIVE_TEMPORARIES_ARB, (GLint *) &capabilities->maxFragmentProgramTemporaryCount);
		glGetProgramivARB(GL_FRAGMENT_PROGRAM_ARB, GL_MAX_PROGRAM_NATIVE_PARAMETERS_ARB, (GLint *) &capabilities->maxFragmentProgramParameterCount);
	}
	
	#if C4DEBUG
	
		if (extensionFlag[kExtensionDebugOutput])
		{
			GLGETEXTFUNC(glDebugMessageControlARB);
			GLGETEXTFUNC(glDebugMessageInsertARB);
			GLGETEXTFUNC(glDebugMessageCallbackARB);
			GLGETEXTFUNC(glGetDebugMessageLogARB);
			GLGETEXTFUNC(glGetPointerv);
		}
	
	#endif
	
	if (extensionFlag[kExtensionVertexProgram])
	{
		GLGETEXTFUNC(glVertexAttrib1fARB);
		GLGETEXTFUNC(glVertexAttrib2fARB);
		GLGETEXTFUNC(glVertexAttrib3fARB);
		GLGETEXTFUNC(glVertexAttrib4fARB);
		GLGETEXTFUNC(glVertexAttrib4NubARB);
		GLGETEXTFUNC(glVertexAttrib1fvARB);
		GLGETEXTFUNC(glVertexAttrib2fvARB);
		GLGETEXTFUNC(glVertexAttrib3fvARB);
		GLGETEXTFUNC(glVertexAttrib4ubvARB);
		GLGETEXTFUNC(glVertexAttrib4fvARB);
		GLGETEXTFUNC(glVertexAttrib4NubvARB);
		GLGETEXTFUNC(glVertexAttribPointerARB);
		GLGETEXTFUNC(glEnableVertexAttribArrayARB);
		GLGETEXTFUNC(glDisableVertexAttribArrayARB);
		GLGETEXTFUNC(glProgramStringARB);
		GLGETEXTFUNC(glBindProgramARB);
		GLGETEXTFUNC(glDeleteProgramsARB);
		GLGETEXTFUNC(glGenProgramsARB);
		GLGETEXTFUNC(glProgramEnvParameter4fARB);
		GLGETEXTFUNC(glProgramEnvParameter4fvARB);
		GLGETEXTFUNC(glProgramLocalParameter4fARB);
		GLGETEXTFUNC(glProgramLocalParameter4fvARB);
		GLGETEXTFUNC(glGetProgramivARB);
		
		glGetProgramivARB(GL_VERTEX_PROGRAM_ARB, GL_MAX_PROGRAM_NATIVE_INSTRUCTIONS_ARB, (GLint *) &capabilities->maxVertexProgramInstructionCount);
		glGetProgramivARB(GL_VERTEX_PROGRAM_ARB, GL_MAX_PROGRAM_NATIVE_TEMPORARIES_ARB, (GLint *) &capabilities->maxVertexProgramTemporaryCount);
		glGetProgramivARB(GL_VERTEX_PROGRAM_ARB, GL_MAX_PROGRAM_NATIVE_ATTRIBS_ARB, (GLint *) &capabilities->maxVertexProgramAttributeCount);
		glGetProgramivARB(GL_VERTEX_PROGRAM_ARB, GL_MAX_PROGRAM_NATIVE_PARAMETERS_ARB, (GLint *) &capabilities->maxVertexProgramParameterCount);
		glGetProgramivARB(GL_VERTEX_PROGRAM_ARB, GL_MAX_PROGRAM_NATIVE_ADDRESS_REGISTERS_ARB, (GLint *) &capabilities->maxVertexProgramAddressRegisterCount);
	}
	
	if (extensionFlag[kExtensionDepthBoundsTest])
	{
		GLGETEXTFUNC(glDepthBoundsEXT);
	}
	
	if (extensionFlag[kExtensionDirectStateAccess])
	{
		GLGETEXTFUNC(glEnableClientStateIndexedEXT);
		GLGETEXTFUNC(glDisableClientStateIndexedEXT);
		GLGETEXTFUNC(glBindMultiTextureEXT);
		GLGETEXTFUNC(glTextureParameteriEXT);
		GLGETEXTFUNC(glTextureParameterivEXT);
		GLGETEXTFUNC(glTextureParameterfEXT);
		GLGETEXTFUNC(glTextureParameterfvEXT);
		GLGETEXTFUNC(glMultiTexParameteriEXT);
		GLGETEXTFUNC(glMultiTexParameterivEXT);
		GLGETEXTFUNC(glMultiTexParameterfEXT);
		GLGETEXTFUNC(glMultiTexParameterfvEXT);
		GLGETEXTFUNC(glTextureImage2DEXT);
		GLGETEXTFUNC(glTextureSubImage2DEXT);
		GLGETEXTFUNC(glTextureImage3DEXT);
		GLGETEXTFUNC(glCompressedTextureImage2DEXT);
		GLGETEXTFUNC(glCompressedTextureImage3DEXT);
		GLGETEXTFUNC(glCopyTextureSubImage2DEXT);
		GLGETEXTFUNC(glNamedProgramStringEXT);
		GLGETEXTFUNC(glNamedBufferDataEXT);
		GLGETEXTFUNC(glNamedBufferSubDataEXT);
		GLGETEXTFUNC(glMapNamedBufferEXT);
		GLGETEXTFUNC(glUnmapNamedBufferEXT);
		GLGETEXTFUNC(glProgramUniform4fEXT);
		GLGETEXTFUNC(glProgramUniform4fvEXT);
		GLGETEXTFUNC(glNamedRenderbufferStorageEXT);
		GLGETEXTFUNC(glNamedRenderbufferStorageMultisampleEXT);
		GLGETEXTFUNC(glNamedRenderbufferStorageMultisampleCoverageEXT);
		GLGETEXTFUNC(glCheckNamedFramebufferStatusEXT);
		GLGETEXTFUNC(glNamedFramebufferTexture2DEXT);
		GLGETEXTFUNC(glNamedFramebufferTexture3DEXT);
		GLGETEXTFUNC(glNamedFramebufferRenderbufferEXT);
		GLGETEXTFUNC(glFramebufferDrawBufferEXT);
		GLGETEXTFUNC(glFramebufferDrawBuffersEXT);
		GLGETEXTFUNC(glFramebufferReadBufferEXT);
		GLGETEXTFUNC(glNamedFramebufferTextureEXT);
		GLGETEXTFUNC(glNamedFramebufferTextureLayerEXT);
		GLGETEXTFUNC(glNamedFramebufferTextureFaceEXT);
		GLGETEXTFUNC(glTextureRenderbufferEXT);
	}
	else
	{
		#if !C4SERVER
		
			glBindMultiTextureEXT = &Render::BindMultiTexture;
			glTextureParameteriEXT = &Render::TextureParameteri;
			glTextureParameterivEXT = &Render::TextureParameteriv;
			glTextureParameterfEXT = &Render::TextureParameterf;
			glTextureParameterfvEXT = &Render::TextureParameterfv;
			glMultiTexParameteriEXT = &Render::MultiTexParameteri;
			glMultiTexParameterivEXT = &Render::MultiTexParameteriv;
			glMultiTexParameterfEXT = &Render::MultiTexParameterf;
			glMultiTexParameterfvEXT = &Render::MultiTexParameterfv;
			glTextureImage2DEXT = &Render::TextureImage2D;
			glTextureSubImage2DEXT = &Render::TextureSubImage2D;
			glTextureImage3DEXT = &Render::TextureImage3D;
			glCompressedTextureImage2DEXT = &Render::CompressedTextureImage2D;
			glCompressedTextureImage3DEXT = &Render::CompressedTextureImage3D;
			glCopyTextureSubImage2DEXT = &Render::CopyTextureSubImage2D;
			glNamedBufferDataEXT = &Render::NamedBufferData;
			glNamedBufferSubDataEXT = &Render::NamedBufferSubData;
			glMapNamedBufferEXT = &Render::MapNamedBuffer;
			glUnmapNamedBufferEXT = &Render::UnmapNamedBuffer;
		
		#endif
	}
	
	if (extensionFlag[kExtensionTextureFilterAnisotropic])
	{
		glGetFloatv(GL_MAX_TEXTURE_MAX_ANISOTROPY_EXT, (GLfloat *) &capabilities->maxTextureAnisotropy);
	}
	
	if (extensionFlag[kExtensionConditionalRender])
	{
		GLGETEXTFUNC(glBeginConditionalRender);
		GLGETEXTFUNC(glEndConditionalRender);
	}
	
	if (extensionFlag[kExtensionExplicitMultisample])
	{
		GLGETEXTFUNC(glGetBooleanIndexedvEXT);
		GLGETEXTFUNC(glGetIntegerIndexedvEXT);
		GLGETEXTFUNC(glGetMultisamplefvNV);
		GLGETEXTFUNC(glSampleMaskIndexedNV);
		GLGETEXTFUNC(glTexRenderbufferNV);
	}
	
	if (extensionFlag[kExtensionFramebufferMultisampleCoverage])
	{
		GLGETEXTFUNC(glRenderbufferStorageMultisampleCoverageNV);
	}
	
	if (extensionFlag[kExtensionGpuProgram4])
	{
		GLGETEXTFUNC(glProgramLocalParameterI4iNV);
		GLGETEXTFUNC(glProgramLocalParameterI4ivNV);
		GLGETEXTFUNC(glProgramLocalParameterI4uiNV);
		GLGETEXTFUNC(glProgramLocalParameterI4uivNV);
		GLGETEXTFUNC(glProgramVertexLimitNV);
		GLGETEXTFUNC(glVertexAttribI1iNV);
		GLGETEXTFUNC(glVertexAttribI2iNV);
		GLGETEXTFUNC(glVertexAttribI3iNV);
		GLGETEXTFUNC(glVertexAttribI4iNV);
		GLGETEXTFUNC(glVertexAttribI1uiNV);
		GLGETEXTFUNC(glVertexAttribI2uiNV);
		GLGETEXTFUNC(glVertexAttribI3uiNV);
		GLGETEXTFUNC(glVertexAttribI4uiNV);
		GLGETEXTFUNC(glVertexAttribI1ivNV);
		GLGETEXTFUNC(glVertexAttribI2ivNV);
		GLGETEXTFUNC(glVertexAttribI3ivNV);
		GLGETEXTFUNC(glVertexAttribI4ivNV);
		GLGETEXTFUNC(glVertexAttribI1uivNV);
		GLGETEXTFUNC(glVertexAttribI2uivNV);
		GLGETEXTFUNC(glVertexAttribI3uivNV);
		GLGETEXTFUNC(glVertexAttribI4uivNV);
		GLGETEXTFUNC(glVertexAttribI4bvNV);
		GLGETEXTFUNC(glVertexAttribI4svNV);
		GLGETEXTFUNC(glVertexAttribI4ubvNV);
		GLGETEXTFUNC(glVertexAttribI4usvNV);
		GLGETEXTFUNC(glVertexAttribIPointerNV);
		
		glGetProgramivARB(GL_GEOMETRY_PROGRAM_NV, GL_MAX_PROGRAM_OUTPUT_VERTICES_NV, (GLint *) &capabilities->maxGeometryProgramOutputVertexCount);
		glGetProgramivARB(GL_GEOMETRY_PROGRAM_NV, GL_MAX_PROGRAM_TOTAL_OUTPUT_COMPONENTS_NV, (GLint *) &capabilities->maxGeometryProgramOutputComponentCount);
		glGetIntegerv(GL_MAX_GEOMETRY_TEXTURE_IMAGE_UNITS_NV, (GLint *) &capabilities->maxGeometryProgramTextureImageCount);
		
		glGetProgramivARB(GL_VERTEX_PROGRAM_ARB, GL_MAX_PROGRAM_RESULT_COMPONENTS_NV, (GLint *) &capabilities->maxVertexProgramResultComponentCount);
		glGetProgramivARB(GL_GEOMETRY_PROGRAM_NV, GL_MAX_PROGRAM_RESULT_COMPONENTS_NV, (GLint *) &capabilities->maxGeometryProgramResultComponentCount);
	}
	
	if (extensionFlag[kExtensionShaderBufferLoad])
	{
		GLGETEXTFUNC(glMakeNamedBufferResidentNV);
		GLGETEXTFUNC(glMakeNamedBufferNonResidentNV);
		GLGETEXTFUNC(glGetNamedBufferParameterui64vNV);
		GLGETEXTFUNC(glProgramUniformui64NV);
		GLGETEXTFUNC(glProgramUniformui64vNV);
	}
	
	if (extensionFlag[kExtensionTransformFeedback])
	{
		GLGETEXTFUNC(glTransformFeedbackAttribsNV);
		GLGETEXTFUNC(glTransformFeedbackVaryingsNV);
		GLGETEXTFUNC(glBeginTransformFeedbackNV);
		GLGETEXTFUNC(glEndTransformFeedbackNV);
		GLGETEXTFUNC(glGetVaryingLocationNV);
		GLGETEXTFUNC(glGetActiveVaryingNV);
		GLGETEXTFUNC(glActiveVaryingNV);
		
		glGetIntegerv(GL_MAX_TRANSFORM_FEEDBACK_INTERLEAVED_COMPONENTS, (GLint *) &capabilities->maxTransformFeedbackInterleavedComponents);
		glGetIntegerv(GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_COMPONENTS, (GLint *) &capabilities->maxTransformFeedbackSeparateComponents);
		glGetIntegerv(GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_ATTRIBS, (GLint *) &capabilities->maxTransformFeedbackSeparateAttribs);
	}
	
	if (extensionFlag[kExtensionVertexBufferUnifiedMemory])
	{
		GLGETEXTFUNC(glBufferAddressRangeNV);
		GLGETEXTFUNC(glVertexAttribFormatNV);
		GLGETEXTFUNC(glVertexAttribIFormatNV);
		GLGETEXTFUNC(glGetIntegerui64i_vNV);
	}
}

// ZYURVUR
