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


#include "C4Render.h"

#if C4DEBUG

	#include "C4Engine.h"

#endif

#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

#endif //]


using namespace C4;


Render::RenderState Render::renderState;


#if C4SERVER

	void Render::Initialize(void)
	{
	}
	
	void Render::Terminate(void)
	{
	}

#elif C4OPENGL

	namespace
	{
		enum
		{
			kUpdateFragmentProgramParameters	= 1 << 0
		};
		
		
		struct TextureFormatData
		{
			GLenum			internalFormat;
			GLenum			pixelFormat;
			GLenum			pixelType;
			unsigned_int32	pixelSize;
		};
		
		static const TextureFormatData textureFormatData[Render::kTextureFormatCount] =
		{
			{GL_RGB8, GL_RGBA, GL_UNSIGNED_BYTE, 4},								// kTextureRGBX8
			{GL_RGBA8, GL_RGBA, GL_UNSIGNED_BYTE, 4},								// kTextureRGBA8
			{GL_RGB8, GL_BGRA, GL_UNSIGNED_BYTE, 4},								// kTextureBGRX8
			{GL_RGBA8, GL_BGRA, GL_UNSIGNED_BYTE, 4},								// kTextureBGRA8
			{GL_RGB8, GL_RGBA, GL_UNSIGNED_INT_8_8_8_8_REV, 4},						// kTextureXRGB8
			{GL_RGBA8, GL_RGBA, GL_UNSIGNED_INT_8_8_8_8_REV, 4},					// kTextureARGB8
			{GL_LUMINANCE8, GL_LUMINANCE, GL_UNSIGNED_BYTE, 1},						// kTextureL8
			{GL_LUMINANCE8_ALPHA8, GL_LUMINANCE_ALPHA, GL_UNSIGNED_BYTE, 2},		// kTextureLA8
			{GL_INTENSITY8, GL_LUMINANCE, GL_UNSIGNED_BYTE, 1},						// kTextureI8
			{GL_INTENSITY16, GL_LUMINANCE, GL_UNSIGNED_SHORT, 2},					// kTextureI16
			{GL_DEPTH_COMPONENT, GL_DEPTH_COMPONENT, GL_UNSIGNED_SHORT, 2},			// kTextureDepth16
			{GL_DEPTH_COMPONENT24, GL_DEPTH_COMPONENT, GL_UNSIGNED_INT, 4},			// kTextureDepth24
			{GL_COMPRESSED_RGB_S3TC_DXT1_EXT, GL_RGBA, GL_UNSIGNED_BYTE, 8},		// kTextureBC1
			{GL_COMPRESSED_RGBA_S3TC_DXT5_EXT, GL_RGBA, GL_UNSIGNED_BYTE, 16},		// kTextureBC3
			{GL_RGB8, GL_RGBA, GL_UNSIGNED_BYTE, 4},								// kTextureRenderBufferRGB8
			{GL_RGBA8, GL_RGBA, GL_UNSIGNED_BYTE, 4},								// kTextureRenderBufferRGBA8
			{GL_RGBA16F, GL_RGBA, GL_HALF_FLOAT, 8}									// kTextureRenderBufferRGBA16F
		};
	}
	
	
	void Render::TextureObject::Construct(unsigned_int32 index)
	{
		static const unsigned_int16 target[kTextureTargetCount] =
		{
			GL_TEXTURE_2D, GL_TEXTURE_3D, GL_TEXTURE_RECTANGLE, GL_TEXTURE_CUBE_MAP, GL_TEXTURE_2D_ARRAY
		};
		
		targetIndex = index;
		openglTarget = target[index];
		
		glGenTextures(1, &identifier);
	}
	
	void Render::TextureObject::Destruct(void)
	{
		glDeleteTextures(1, &identifier);
		
		for (unsigned_machine unit = 0; unit < kMaxTextureUnitCount; unit++)
		{
			if (renderState.texture[unit][targetIndex] == identifier) renderState.texture[unit][targetIndex] = 0;
		}
	}
	
	void Render::TextureObject::Bind(unsigned_int32 unit) const
	{
		GLuint *object = &renderState.texture[unit][targetIndex];
		if (*object != identifier)
		{
			*object = identifier;
			glBindMultiTextureEXT(GL_TEXTURE0 + unit, openglTarget, identifier); 
		}
	} 
	 
	void Render::TextureObject::Unbind(unsigned_int32 unit) const 
	{
		GLuint *object = &renderState.texture[unit][targetIndex]; 
		if (*object == identifier)
		{
			*object = 0;
			glBindMultiTextureEXT(GL_TEXTURE0 + unit, openglTarget, 0); 
		}
	}
	
	void Render::TextureObject::UnbindAll(void) const 
	{
		for (machine a = 0; a < kMaxTextureUnitCount; a++)
		{
			GLuint *object = &renderState.texture[a][targetIndex];
			if (*object == identifier)
			{
				*object = 0;
				glBindMultiTextureEXT(GL_TEXTURE0 + a, openglTarget, 0);
			}
		}
	}
	
	unsigned_int32 Render::TextureObject::SetImage2D(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, int32 count, const TextureImageData *imageData)
	{
		glPixelStorei(GL_UNPACK_ROW_LENGTH, 0);
		
		formatIndex = format;
		const TextureFormatData *formatData = &textureFormatData[format];
		
		char *storage = nullptr;
		unsigned_int32 storageSize = 0;
		
		for (machine level = 0; level < count; level++)
		{
			unsigned_int32 pixelCount = width * height;
			unsigned_int32 size = pixelCount * formatData->pixelSize;
			storageSize += size;
			
			const void *image = imageData->image;
			if (imageData->decompressor)
			{
				if (!storage) storage = new char[size];
				(*imageData->decompressor)(static_cast<const unsigned_int8 *>(image), imageData->size, storage);
				image = storage;
			}
			
			glTextureImage2DEXT(identifier, GL_TEXTURE_2D, level, formatData->internalFormat, width, height, 0, formatData->pixelFormat, formatData->pixelType, image);
			
			width = Max(width >> 1, 1);
			height = Max(height >> 1, 1);
			
			imageData++;
		}
		
		delete[] storage;
		return (storageSize);
	}
	
	unsigned_int32 Render::TextureObject::SetImage3D(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, unsigned_int32 depth, int32 count, const TextureImageData *imageData)
	{
		glPixelStorei(GL_UNPACK_ROW_LENGTH, 0);
		
		formatIndex = format;
		const TextureFormatData *formatData = &textureFormatData[format];
		
		char *storage = nullptr;
		unsigned_int32 storageSize = 0;
		
		for (machine level = 0; level < count; level++)
		{
			unsigned_int32 pixelCount = width * height * depth;
			unsigned_int32 size = pixelCount * formatData->pixelSize;
			storageSize += size;
			
			const void *image = imageData->image;
			if (imageData->decompressor)
			{
				if (!storage) storage = new char[size];
				(*imageData->decompressor)(static_cast<const unsigned_int8 *>(image), imageData->size, storage);
				image = storage;
			}
			
			glTextureImage3DEXT(identifier, GL_TEXTURE_3D, level, formatData->internalFormat, width, height, depth, 0, formatData->pixelFormat, formatData->pixelType, image);
			
			width = Max(width >> 1, 1);
			height = Max(height >> 1, 1);
			depth = Max(depth >> 1, 1);
			
			imageData++;
		}
		
		delete[] storage;
		return (storageSize);
	}
	
	unsigned_int32 Render::TextureObject::SetImageCube(unsigned_int32 format, unsigned_int32 width, int32 count, const TextureImageData *imageData)
	{
		glPixelStorei(GL_UNPACK_ROW_LENGTH, 0);
		
		formatIndex = format;
		const TextureFormatData *formatData = &textureFormatData[format];
		
		char *storage = nullptr;
		unsigned_int32 storageSize = 0;
		
		for (machine level = 0; level < count; level++)
		{
			for (machine component = 0; component < 6; component++)
			{
				unsigned_int32 pixelCount = width * width;
				unsigned_int32 size = pixelCount * formatData->pixelSize;
				storageSize += size;
				
				const void *image = imageData->image;
				if (imageData->decompressor)
				{
					if (!storage) storage = new char[size];
					(*imageData->decompressor)(static_cast<const unsigned_int8 *>(image), imageData->size, storage);
					image = storage;
				}
				
				glTextureImage2DEXT(identifier, GL_TEXTURE_CUBE_MAP_POSITIVE_X + component, level, formatData->internalFormat, width, width, 0, formatData->pixelFormat, formatData->pixelType, image);
				imageData++;
			}
			
			width >>= 1;
		}
		
		delete[] storage;
		return (storageSize);
	}
	
	unsigned_int32 Render::TextureObject::SetImageRect(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, unsigned_int32 rowLength, const TextureImageData *imageData)
	{
		formatIndex = format;
		const TextureFormatData *formatData = &textureFormatData[format];
		
		char *storage = nullptr;
		
		int32 pixelCount = width * height;
		unsigned_int32 storageSize = pixelCount * formatData->pixelSize;
		
		const void *image = imageData->image;
		if (imageData->decompressor)
		{
			if (!storage) storage = new char[storageSize];
			(*imageData->decompressor)(static_cast<const unsigned_int8 *>(image), imageData->size, storage);
			image = storage;
		}
		
		glPixelStorei(GL_UNPACK_ROW_LENGTH, rowLength);
		glTextureImage2DEXT(identifier, GL_TEXTURE_RECTANGLE, 0, formatData->internalFormat, width, height, 0, formatData->pixelFormat, formatData->pixelType, image);
		
		delete[] storage;
		return (storageSize);
	}
	
	unsigned_int32 Render::TextureObject::SetImageArray2D(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, unsigned_int32 depth, int32 count, const TextureImageData *imageData)
	{
		glPixelStorei(GL_UNPACK_ROW_LENGTH, 0);
		
		formatIndex = format;
		const TextureFormatData *formatData = &textureFormatData[format];
		
		char *storage = nullptr;
		unsigned_int32 storageSize = 0;
		
		for (machine level = 0; level < count; level++)
		{
			unsigned_int32 pixelCount = width * height * depth;
			unsigned_int32 size = pixelCount * formatData->pixelSize;
			storageSize += size;
			
			const void *image = imageData->image;
			if (imageData->decompressor)
			{
				if (!storage) storage = new char[size];
				(*imageData->decompressor)(static_cast<const unsigned_int8 *>(image), imageData->size, storage);
				image = storage;
			}
			
			glTextureImage3DEXT(identifier, GL_TEXTURE_2D_ARRAY, level, formatData->internalFormat, width, height, depth, 0, formatData->pixelFormat, formatData->pixelType, image);
			
			width = Max(width >> 1, 1);
			height = Max(height >> 1, 1);
			
			imageData++;
		}
		
		delete[] storage;
		return (storageSize);
	}
	
	unsigned_int32 Render::TextureObject::SetCompressedImage2D(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, int32 count, const TextureImageData *imageData)
	{
		formatIndex = format;
		const TextureFormatData *formatData = &textureFormatData[format];
		
		char *storage = nullptr;
		unsigned_int32 storageSize = 0;
		
		for (machine level = 0; level < count; level++)
		{
			const void *image = imageData->image;
			unsigned_int32 size = imageData->size;
			
			if (imageData->decompressor)
			{
				unsigned_int32 blockCount = ((width + 3) / 4) * ((height + 3) / 4);
				size = blockCount * formatData->pixelSize;
				if (!storage) storage = new char[size];
				(*imageData->decompressor)(static_cast<const unsigned_int8 *>(image), imageData->size, storage);
				image = storage;
			}
			
			storageSize += size;
			glCompressedTextureImage2DEXT(identifier, GL_TEXTURE_2D, level, formatData->internalFormat, width, height, 0, size, image);
			
			width = Max(width >> 1, 1);
			height = Max(height >> 1, 1);
			
			imageData++;
		}
		
		delete[] storage;
		return (storageSize);
	}
	
	unsigned_int32 Render::TextureObject::SetCompressedImageCube(unsigned_int32 format, unsigned_int32 width, int32 count, const TextureImageData *imageData)
	{
		formatIndex = format;
		const TextureFormatData *formatData = &textureFormatData[format];
		
		char *storage = nullptr;
		unsigned_int32 storageSize = 0;
		
		for (machine level = 0; level < count; level++)
		{
			for (machine component = 0; component < 6; component++)
			{
				const void *image = imageData->image;
				unsigned_int32 size = imageData->size;
				
				if (imageData->decompressor)
				{
					unsigned_int32 w = (width + 3) / 4;
					unsigned_int32 blockCount = w * w;
					size = blockCount * formatData->pixelSize;
					if (!storage) storage = new char[size];
					(*imageData->decompressor)(static_cast<const unsigned_int8 *>(image), imageData->size, storage);
					image = storage;
				}
				
				storageSize += size;
				glCompressedTextureImage2DEXT(identifier, GL_TEXTURE_CUBE_MAP_POSITIVE_X + component, level, formatData->internalFormat, width, width, 0, size, image);
				
				imageData++;
			}
			
			width >>= 1;
		}
		
		delete[] storage;
		return (storageSize);
	}
	
	unsigned_int32 Render::TextureObject::SetCompressedImageArray2D(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, unsigned_int32 depth, int32 count, const TextureImageData *imageData)
	{
		formatIndex = format;
		const TextureFormatData *formatData = &textureFormatData[format];
		
		char *storage = nullptr;
		unsigned_int32 storageSize = 0;
		
		for (machine level = 0; level < count; level++)
		{
			const void *image = imageData->image;
			unsigned_int32 size = imageData->size;
			
			if (imageData->decompressor)
			{
				unsigned_int32 blockCount = ((width + 3) / 4) * ((height + 3) / 4) * depth;
				size = blockCount * formatData->pixelSize;
				if (!storage) storage = new char[size];
				(*imageData->decompressor)(static_cast<const unsigned_int8 *>(image), imageData->size, storage);
				image = storage;
			}
			
			storageSize += size;
			glCompressedTextureImage3DEXT(identifier, GL_TEXTURE_2D_ARRAY, level, formatData->internalFormat, width, height, depth, 0, size, image);
			
			width = Max(width >> 1, 1);
			height = Max(height >> 1, 1);
			
			imageData++;
		}
		
		delete[] storage;
		return (storageSize);
	}
	
	unsigned_int32 Render::TextureObject::AllocateStorage2D(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, bool renderBuffer)
	{
		formatIndex = format;
		const TextureFormatData *formatData = &textureFormatData[format];
		
		SetMinFilterMode(Render::kFilterLinear);
		SetMagFilterMode(Render::kFilterLinear);
		
		glTextureImage2DEXT(identifier, GL_TEXTURE_2D, 0, formatData->internalFormat, width, height, 0, formatData->pixelFormat, formatData->pixelType, nullptr);
		Unbind(0);
		
		return (width * height * formatData->pixelSize);
	}
	
	unsigned_int32 Render::TextureObject::AllocateStorageRect(unsigned_int32 format, unsigned_int32 width, unsigned_int32 height, bool renderBuffer)
	{
		formatIndex = format;
		const TextureFormatData *formatData = &textureFormatData[format];
		
		SetMinFilterMode(Render::kFilterLinear);
		SetMagFilterMode(Render::kFilterLinear);
		
		glTextureImage2DEXT(identifier, GL_TEXTURE_RECTANGLE, 0, formatData->internalFormat, width, height, 0, formatData->pixelFormat, formatData->pixelType, nullptr);
		Unbind(0);
		
		return (width * height * formatData->pixelSize);
	}
	
	void Render::TextureObject::UpdateImage2D(unsigned_int32 x, unsigned_int32 y, unsigned_int32 width, unsigned_int32 height, unsigned_int32 rowLength, const void *image) const
	{
		glPixelStorei(GL_UNPACK_ROW_LENGTH, rowLength);
		
		const TextureFormatData *formatData = &textureFormatData[formatIndex];
		image = static_cast<const char *>(image) + (rowLength * y + x) * formatData->pixelSize;
		glTextureSubImage2DEXT(identifier, GL_TEXTURE_2D, 0, x, y, width, height, formatData->pixelFormat, formatData->pixelType, image);
	}
	
	void Render::TextureObject::UpdateImageRect(unsigned_int32 x, unsigned_int32 y, unsigned_int32 width, unsigned_int32 height, unsigned_int32 rowLength, const void *image) const
	{
		glPixelStorei(GL_UNPACK_ROW_LENGTH, rowLength);
		
		const TextureFormatData *formatData = &textureFormatData[formatIndex];
		image = static_cast<const char *>(image) + (rowLength * y + x) * formatData->pixelSize;
		glTextureSubImage2DEXT(identifier, GL_TEXTURE_RECTANGLE, 0, x, y, width, height, formatData->pixelFormat, formatData->pixelType, image);
	}
	
	
	void Render::RenderBufferObject::Construct(void)
	{
		glGenRenderbuffers(1, &identifier);
	}
	
	void Render::RenderBufferObject::Destruct(void)
	{
		glDeleteRenderbuffers(1, &identifier);
	}
	
	void Render::RenderBufferObject::AllocateStorage(unsigned_int32 width, unsigned_int32 height, unsigned_int32 format)
	{
		glBindRenderbuffer(GL_RENDERBUFFER, identifier);
		glRenderbufferStorage(GL_RENDERBUFFER, format, width, height);
		glBindRenderbuffer(GL_RENDERBUFFER, 0);
	}
	
	void Render::RenderBufferObject::AllocateMultisampleStorage(unsigned_int32 width, unsigned_int32 height, unsigned_int32 sampleCount, unsigned_int32 format)
	{
		glBindRenderbuffer(GL_RENDERBUFFER, identifier);
		glRenderbufferStorageMultisample(GL_RENDERBUFFER, sampleCount, format, width, height);
		glBindRenderbuffer(GL_RENDERBUFFER, 0);
	}
	
	
	void Render::FrameBufferObject::Construct(void)
	{
		glGenFramebuffers(1, &identifier);
		
		currentColorTexture = nullptr;
		currentDepthTexture = nullptr;
	}
	
	void Render::FrameBufferObject::Destruct(void)
	{
		glDeleteFramebuffers(1, &identifier);
	}
	
	void Render::FrameBufferObject::SetColorRenderBuffer(const RenderBufferObject *renderBuffer)
	{
		Bind();
		glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_RENDERBUFFER, renderBuffer->GetRenderBufferIdentifier());
	}
	
	void Render::FrameBufferObject::SetDepthStencilRenderBuffer(const RenderBufferObject *renderBuffer)
	{
		Bind();
		GLuint bufferIdentifier = renderBuffer->GetRenderBufferIdentifier();
		glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, bufferIdentifier);
		glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_STENCIL_ATTACHMENT, GL_RENDERBUFFER, bufferIdentifier);
	}
	
	void Render::FrameBufferObject::SetColorRenderTexture(const TextureObject *renderTexture)
	{
		if (currentColorTexture != renderTexture)
		{
			currentColorTexture = renderTexture;
			
			Bind();
			glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_RECTANGLE, renderTexture->GetTextureIdentifier(), 0);
		}
	}
	
	void Render::FrameBufferObject::ResetColorRenderTexture(void)
	{
		if (currentColorTexture)
		{
			currentColorTexture = nullptr;
			
			Bind();
			glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_RECTANGLE, 0, 0);
		}
	}
	
	void Render::FrameBufferObject::SetDepthRenderTexture(const TextureObject *renderTexture)
	{
		if (currentDepthTexture != renderTexture)
		{
			currentDepthTexture = renderTexture;
			
			Bind();
			glFramebufferTexture2D(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_TEXTURE_2D, renderTexture->GetTextureIdentifier(), 0);
			
			glDrawBuffer(GL_NONE);
			glReadBuffer(GL_NONE);
		}
	}
	
	void Render::FrameBufferObject::ResetDepthRenderTexture(void)
	{
		if (currentDepthTexture)
		{
			currentDepthTexture = nullptr;
			
			Bind();
			glFramebufferTexture2D(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_TEXTURE_2D, 0, 0);
		}
	}
	
	
	void Render::VertexBufferObject::Construct(void)
	{
		openglTarget = (bufferTarget == kVertexBufferTargetAttribute) ? GL_ARRAY_BUFFER : GL_ELEMENT_ARRAY_BUFFER;
		openglUsage = (bufferUsage == kVertexBufferUsageStatic) ? GL_STATIC_DRAW : GL_DYNAMIC_DRAW;
		
		glGenBuffers(1, &identifier);
	}
	
	void Render::VertexBufferObject::Destruct(void)
	{
		glDeleteBuffers(1, &identifier);
		
		if (renderState.attributeVertexBuffer == identifier) renderState.attributeVertexBuffer = 0;
		else if (renderState.indexVertexBuffer == identifier) renderState.indexVertexBuffer = 0;
	}
	
	bool Render::VertexBufferObject::AllocateStorage(unsigned_int32 size)
	{
		glGetError();
		glNamedBufferDataEXT(identifier, size, nullptr, openglUsage);
		return (glGetError() == GL_NO_ERROR);
	}
	
	
	void Render::QueryObject::Construct(void)
	{
		glGenQueries(1, &identifier);
	}
	
	void Render::QueryObject::Destruct(void)
	{
		glDeleteQueries(1, &identifier);
	}
	
	
	void Render::VertexProgramObject::Construct(void)
	{
		glGenProgramsARB(1, &identifier);
	}
	
	void Render::VertexProgramObject::Destruct(void)
	{
		glDeleteProgramsARB(1, &identifier);
		
		if (renderState.vertexProgram == this) renderState.vertexProgram = nullptr;
	}
	
	void Render::VertexProgramObject::SetSourceCode(const char *text, unsigned_int32 size)
	{
		renderState.vertexProgram = this;
		glBindProgramARB(GL_VERTEX_PROGRAM_ARB, identifier);
		
		#if C4DEBUG
		
			glGetError();
		
		#endif
		
		glProgramStringARB(GL_VERTEX_PROGRAM_ARB, GL_PROGRAM_FORMAT_ASCII_ARB, size, text);
		
		#if C4DEBUG
		
			if (glGetError() != GL_NO_ERROR)
			{
				Engine::Report("Vertex program error logged", kReportError);
				Engine::Report("Vertex program error\r\n", kReportLog | kReportError);
				
				const char *string = reinterpret_cast<const char *>(glGetString(GL_PROGRAM_ERROR_STRING_ARB));
				if (string) Engine::Report(string, kReportLog | kReportError | kReportFormatted);
				Engine::LogSource(text);
			}
			else
			{
				GLint	underLimits;
				
				glGetProgramivARB(GL_VERTEX_PROGRAM_ARB, GL_PROGRAM_UNDER_NATIVE_LIMITS_ARB, &underLimits);
				if (!underLimits)
				{
					Engine::Report("Vertex program resource overflow logged", kReportError);
					Engine::Report("Vertex program resource overflow\r\n", kReportLog | kReportError);
					Engine::LogSource(text);
				}
			}
		
		#endif
	}
	
	
	void Render::FragmentProgramObject::Construct(bool programFlag)
	{
		if (programFlag)
		{
			glGenProgramsARB(1, &identifier);
		}
		else
		{
			identifier = 0;
			uniformLocation = -1;
			
			programHandle = glCreateProgramObject();
			shaderHandle = glCreateShaderObject(GL_FRAGMENT_SHADER);
			glAttachObject(programHandle, shaderHandle);
		}
	}
	
	void Render::FragmentProgramObject::Destruct(void)
	{
		if (identifier != 0)
		{
			glDeleteProgramsARB(1, &identifier);
			
			if (renderState.fragmentProgram == this) renderState.fragmentProgram = nullptr;
		}
		else
		{
			if (renderState.fragmentShader == this)
			{
				renderState.fragmentShader = nullptr;
				glUseProgramObject(0);
			}
			
			glDeleteObject(shaderHandle);
			glDeleteObject(programHandle);
		}
	}
	
	void Render::FragmentProgramObject::SetSourceCode(const char *text, unsigned_int32 size)
	{
		if (identifier != 0)
		{
			renderState.fragmentProgram = this;
			glBindProgramARB(GL_FRAGMENT_PROGRAM_ARB, identifier);
			
			#if C4DEBUG
			
				glGetError();
			
			#endif
			
			glProgramStringARB(GL_FRAGMENT_PROGRAM_ARB, GL_PROGRAM_FORMAT_ASCII_ARB, size, text);
			
			#if C4DEBUG
			
				if (glGetError() != GL_NO_ERROR)
				{
					Engine::Report("Fragment program error logged", kReportError);
					Engine::Report("Fragment program error\r\n", kReportLog | kReportError);
					
					const char *string = reinterpret_cast<const char *>(glGetString(GL_PROGRAM_ERROR_STRING_ARB));
					if (string) Engine::Report(string, kReportLog | kReportError | kReportFormatted);
					Engine::LogSource(text);
				}
				else
				{
					GLint	underLimits;
					
					glGetProgramivARB(GL_FRAGMENT_PROGRAM_ARB, GL_PROGRAM_UNDER_NATIVE_LIMITS_ARB, &underLimits);
					if (!underLimits)
					{
						Engine::Report("Fragment program resource overflow logged", kReportError);
						Engine::Report("Fragment program resource overflow\r\n", kReportLog | kReportError);
						Engine::LogSource(text);
					}
				}
			
			#endif
		}
		else
		{
			glShaderSource(shaderHandle, 1, &text, reinterpret_cast<GLint *>(&size));
			glCompileShader(shaderHandle);
			glLinkProgram(programHandle);
			
			uniformLocation = QueryUniformLocation("param");
			
			#if C4DEBUG
			
				GLint	status;
				char	string[256];
				
				glGetObjectParameteriv(programHandle, GL_OBJECT_LINK_STATUS, &status);
				if (!status)
				{
					Engine::Report("Fragment shader error logged", kReportError);
					Engine::Report("Fragment shader error\r\n", kReportLog | kReportError);
					
					glGetInfoLog(shaderHandle, 255, nullptr, string);
					Engine::Report(string, kReportLog | kReportError | kReportFormatted);
					
					Engine::LogSource(text);
				}
			
			#endif
		}
	}
	
	
	void Render::GeometryProgramObject::Construct(void)
	{
		glGenProgramsARB(1, &identifier);
	}
	
	void Render::GeometryProgramObject::Destruct(void)
	{
		glDeleteProgramsARB(1, &identifier);
		
		if (renderState.geometryProgram == this) renderState.geometryProgram = nullptr;
	}
	
	void Render::GeometryProgramObject::SetSourceCode(const char *text, unsigned_int32 size)
	{
		renderState.geometryProgram = this;
		glBindProgramARB(GL_GEOMETRY_PROGRAM_NV, identifier);
		
		#if C4DEBUG
		
			glGetError();
		
		#endif
		
		glProgramStringARB(GL_GEOMETRY_PROGRAM_NV, GL_PROGRAM_FORMAT_ASCII_ARB, size, text);
		
		#if C4DEBUG
		
			if (glGetError() != GL_NO_ERROR)
			{
				Engine::Report("Geometry program error logged", kReportError);
				Engine::Report("Geometry program error\r\n", kReportLog | kReportError);
				
				const char *string = reinterpret_cast<const char *>(glGetString(GL_PROGRAM_ERROR_STRING_ARB));
				if (string) Engine::Report(string, kReportLog | kReportError | kReportFormatted);
				Engine::LogSource(text);
			}
		
		#endif
	}
	
	
	void Render::Initialize(void)
	{
		renderState.imageUnit = 0;
		for (machine unit = 0; unit < kMaxTextureUnitCount; unit++)
		{
			for (machine target = 0; target < kTextureTargetCount; target++) renderState.texture[unit][target] = 0;
		}
		
		renderState.drawFrameBuffer = nullptr;
		renderState.readFrameBuffer = nullptr;
		
		renderState.attributeVertexBuffer = 0;
		renderState.indexVertexBuffer = 0;
		
		renderState.vertexProgram = nullptr;
		renderState.fragmentProgram = nullptr;
		renderState.fragmentShader = nullptr;
		renderState.geometryProgram = nullptr;
		
		renderState.updateFlags = 0;
		
		for (machine a = 0; a < kMaxFragmentParamCount; a++) renderState.fragmentProgramParam[a].Set(0.0F, 0.0F, 0.0F, 0.0F);
	}
	
	void Render::Terminate(void)
	{
	}
	
	void Render::SetVertexProgram(const VertexProgramObject *vertexProgram)
	{
		if (renderState.vertexProgram != vertexProgram)
		{
			renderState.vertexProgram = vertexProgram;
			glBindProgramARB(GL_VERTEX_PROGRAM_ARB, vertexProgram->GetVertexProgramIdentifier());
		}
	}
	
	void Render::SetFragmentProgram(const FragmentProgramObject *fragmentProgram)
	{
		unsigned_int32 identifier = fragmentProgram->GetFragmentProgramIdentifier();
		if (identifier != 0)
		{
			if (renderState.fragmentProgram != fragmentProgram)
			{
				renderState.fragmentProgram = fragmentProgram;
				glBindProgramARB(GL_FRAGMENT_PROGRAM_ARB, identifier);
			}
			
			if (renderState.fragmentShader)
			{
				renderState.updateFlags &= ~kUpdateFragmentProgramParameters;
				
				renderState.fragmentShader = nullptr;
				glUseProgramObject(0);
			}
		}
		else
		{
			if (renderState.fragmentShader != fragmentProgram)
			{
				renderState.updateFlags |= kUpdateFragmentProgramParameters;
				
				renderState.fragmentShader = fragmentProgram;
				glUseProgramObject(fragmentProgram->GetFragmentShaderHandle());
			}
		}
	}
	
	void Render::SetGeometryProgram(const GeometryProgramObject *geometryProgram)
	{
		if (renderState.geometryProgram != geometryProgram)
		{
			renderState.geometryProgram = geometryProgram;
			glBindProgramARB(GL_GEOMETRY_PROGRAM_NV, geometryProgram->GetGeometryProgramIdentifier());
		}
	}
	
	void Render::SetFragmentProgramParameter4f(unsigned_int32 index, float x, float y, float z, float w)
	{
		renderState.updateFlags |= kUpdateFragmentProgramParameters;
		
		renderState.fragmentProgramParam[index].Set(x, y, z, w);
		glProgramEnvParameter4fARB(GL_FRAGMENT_PROGRAM_ARB, index, x, y, z, w);
	}
	
	void Render::SetFragmentProgramParameter4fv(unsigned_int32 index, const float *v)
	{
		renderState.updateFlags |= kUpdateFragmentProgramParameters;
		
		renderState.fragmentProgramParam[index].Set(v[0], v[1], v[2], v[3]);
		glProgramEnvParameter4fvARB(GL_FRAGMENT_PROGRAM_ARB, index, v);
	}
	
	void Render::UpdateFragmentProgramParameters(void)
	{
		unsigned long flags = renderState.updateFlags;
		if (flags & kUpdateFragmentProgramParameters)
		{
			renderState.updateFlags = flags & ~kUpdateFragmentProgramParameters;
			
			const FragmentProgramObject *shader = renderState.fragmentShader;
			if (shader)
			{
				int32 location = shader->GetUniformLocation();
				if (location >= 0) glUniform4fv(location, kMaxFragmentParamCount, &renderState.fragmentProgramParam[0].x);
			}
		}
	}
	
	void Render::BindTextureUnit0(GLuint texture, GLenum target)
	{
		if (renderState.imageUnit != 0)
		{
			renderState.imageUnit = 0;
			glActiveTexture(GL_TEXTURE0);
		}
		
		machine targetIndex = 0;
		targetIndex += ((int32) (GL_TEXTURE_2D - target) >> 31) & 1;
		targetIndex += ((int32) (GL_TEXTURE_3D - target) >> 31) & 1;
		targetIndex += ((int32) (GL_TEXTURE_RECTANGLE - target) >> 31) & 1;
		targetIndex += ((int32) (GL_TEXTURE_CUBE_MAP - target) >> 31) & 1;		// kTextureTargetCount - 1
		
		GLuint *object = &renderState.texture[0][targetIndex];
		if (*object != texture)
		{
			*object = texture;
			glBindTexture(target, texture);
		}
	}
	
	void Render::BindMultiTexture(GLenum texunit, GLenum target, GLuint texture)
	{
		int32 unitIndex = texunit - GL_TEXTURE0;
		if (renderState.imageUnit != unitIndex)
		{
			renderState.imageUnit = unitIndex;
			glActiveTexture(texunit);
		}
		
		glBindTexture(target, texture);
	}
	
	void Render::TextureParameteri(GLuint texture, GLenum target, GLenum pname, GLint param)
	{
		BindTextureUnit0(texture, target);
		glTexParameteri(target, pname, param);
	}
	
	void Render::TextureParameteriv(GLuint texture, GLenum target, GLenum pname, const GLint *param)
	{
		BindTextureUnit0(texture, target);
		glTexParameteriv(target, pname, param);
	}
	
	void Render::TextureParameterf(GLuint texture, GLenum target, GLenum pname, GLfloat param)
	{
		BindTextureUnit0(texture, target);
		glTexParameterf(target, pname, param);
	}
	
	void Render::TextureParameterfv(GLuint texture, GLenum target, GLenum pname, const GLfloat *param)
	{
		BindTextureUnit0(texture, target);
		glTexParameterfv(target, pname, param);
	}
	
	void Render::MultiTexParameteri(GLenum texunit, GLenum target, GLenum pname, GLint param)
	{
		int32 unitIndex = texunit - GL_TEXTURE0;
		if (renderState.imageUnit != unitIndex)
		{
			renderState.imageUnit = unitIndex;
			glActiveTexture(texunit);
		}
		
		glTexParameteri(target, pname, param);
	}
	
	void Render::MultiTexParameteriv(GLenum texunit, GLenum target, GLenum pname, const GLint *param)
	{
		int32 unitIndex = texunit - GL_TEXTURE0;
		if (renderState.imageUnit != unitIndex)
		{
			renderState.imageUnit = unitIndex;
			glActiveTexture(texunit);
		}
		
		glTexParameteriv(target, pname, param);
	}
	
	void Render::MultiTexParameterf(GLenum texunit, GLenum target, GLenum pname, GLfloat param)
	{
		int32 unitIndex = texunit - GL_TEXTURE0;
		if (renderState.imageUnit != unitIndex)
		{
			renderState.imageUnit = unitIndex;
			glActiveTexture(texunit);
		}
		
		glTexParameterf(target, pname, param);
	}
	
	void Render::MultiTexParameterfv(GLenum texunit, GLenum target, GLenum pname, const GLfloat *param)
	{
		int32 unitIndex = texunit - GL_TEXTURE0;
		if (renderState.imageUnit != unitIndex)
		{
			renderState.imageUnit = unitIndex;
			glActiveTexture(texunit);
		}
		
		glTexParameterfv(target, pname, param);
	}
	
	void Render::TextureImage2D(GLuint texture, GLenum target, GLint level, GLint internalformat, GLsizei width, GLsizei height, GLint border, GLenum format, GLenum type, const void *pixels)
	{
		// Taking the minimum with GL_TEXTURE_CUBE_MAP remaps all of the pseudo-targets for the
		// individual cube faces to GL_TEXTURE_CUBE_MAP since their enums have greater values.
		
		BindTextureUnit0(texture, Min(target, GL_TEXTURE_CUBE_MAP));
		glTexImage2D(target, level, internalformat, width, height, border, format, type, pixels);
	}
	
	void Render::TextureSubImage2D(GLuint texture, GLenum target, GLint level, GLint xoffset, GLint yoffset, GLsizei width, GLsizei height, GLenum format, GLenum type, const void *pixels)
	{
		BindTextureUnit0(texture, Min(target, GL_TEXTURE_CUBE_MAP));
		glTexSubImage2D(target, level, xoffset, yoffset, width, height, format, type, pixels);
	}
	
	void Render::TextureImage3D(GLuint texture, GLenum target, GLint level, GLint internalformat, GLsizei width, GLsizei height, GLsizei depth, GLint border, GLenum format, GLenum type, const void *pixels)
	{
		BindTextureUnit0(texture, target);
		glTexImage3D(target, level, internalformat, width, height, depth, border, format, type, pixels);
	}
	
	void Render::CompressedTextureImage2D(GLuint texture, GLenum target, GLint level, GLenum internalformat, GLsizei width, GLsizei height, GLint border, GLsizei imageSize, const void *data)
	{
		BindTextureUnit0(texture, Min(target, GL_TEXTURE_CUBE_MAP));
		glCompressedTexImage2D(target, level, internalformat, width, height, border, imageSize, data);
	}
	
	void Render::CompressedTextureImage3D(GLuint texture, GLenum target, GLint level, GLenum internalformat, GLsizei width, GLsizei height, GLsizei depth, GLint border, GLsizei imageSize, const void *data)
	{
		BindTextureUnit0(texture, target);
		glCompressedTexImage3D(target, level, internalformat, width, height, depth, border, imageSize, data);
	}
	
	void Render::CopyTextureSubImage2D(GLuint texture, GLenum target, GLint level, GLint xoffset, GLint yoffset, GLint x, GLint y, GLsizei width, GLsizei height)
	{
		BindTextureUnit0(texture, Min(target, GL_TEXTURE_CUBE_MAP));
		glCopyTexSubImage2D(target, level, xoffset, yoffset, x, y, width, height);
	}
	
	void Render::NamedBufferData(GLuint buffer, GLsizeiptr size, const void *data, GLenum usage)
	{
		if (renderState.attributeVertexBuffer != buffer)
		{
			renderState.attributeVertexBuffer = buffer;
			glBindBuffer(GL_ARRAY_BUFFER, buffer);
		}
		
		glBufferData(GL_ARRAY_BUFFER, size, data, usage);
	}
	
	void Render::NamedBufferSubData(GLuint buffer, GLintptr offset, GLsizeiptr size, const void *data)
	{
		GLuint boundBuffer = renderState.attributeVertexBuffer;
		renderState.attributeVertexBuffer = 0;
		if (boundBuffer != buffer) glBindBuffer(GL_ARRAY_BUFFER, buffer);
		
		glBufferSubData(GL_ARRAY_BUFFER, offset, size, data);
		glBindBuffer(GL_ARRAY_BUFFER, 0);
	}
	
	void *Render::MapNamedBuffer(GLuint buffer, GLenum access)
	{
		if (renderState.attributeVertexBuffer != buffer)
		{
			renderState.attributeVertexBuffer = buffer;
			glBindBuffer(GL_ARRAY_BUFFER, buffer);
		}
		
		return (glMapBuffer(GL_ARRAY_BUFFER, GL_WRITE_ONLY));
	}
	
	GLboolean Render::UnmapNamedBuffer(GLuint buffer)
	{
		GLuint boundBuffer = renderState.attributeVertexBuffer;
		renderState.attributeVertexBuffer = 0;
		if (boundBuffer != buffer) glBindBuffer(GL_ARRAY_BUFFER, buffer);
		
		GLboolean result = glUnmapBuffer(GL_ARRAY_BUFFER);
		glBindBuffer(GL_ARRAY_BUFFER, 0);
		return (result);
	}

#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

#endif //]

// ZYURVUR
