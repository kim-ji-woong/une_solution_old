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


using namespace C4;


ResourceDescriptor TextureResource::descriptor("tex", 0, 0, "C4/missing");


int32 Texture::totalTextureCount = 0;
unsigned_int32 Texture::totalTextureMemory = 0;

List<Texture> Texture::textureList;
Map<Texture> Texture::textureHeaderMap;


namespace C4
{
	struct TextureFormatData
	{
		TextureFormat			engineFormat;
		
		unsigned_int32			renderFormatAlpha;
		unsigned_int32			renderFormat;
		unsigned_int32			pixelSize;
		
		Render::Decompressor	*decompressorAlpha;
		Render::Decompressor	*decompressor;
	};
}


namespace
{
	const TextureFormatData textureFormatDataTable[kTextureFormatCount] =
	{
		{kTextureRGBA8,		Render::kTextureRGBA8,			Render::kTextureRGBX8,		4,	&Image::DecompressImageRLE_RGBA32,	&Image::DecompressImageRLE_RGBA32},
		{kTextureBGRA8,		Render::kTextureBGRA8,			Render::kTextureBGRX8,		4,	&Image::DecompressImageRLE_RGBA32,	&Image::DecompressImageRLE_RGBA32},
		{kTextureARGB8,		Render::kTextureARGB8,			Render::kTextureXRGB8,		4,	&Image::DecompressImageRLE_RGBA32,	&Image::DecompressImageRLE_RGBA32},
		{kTextureLA8,		Render::kTextureLA8,			Render::kTextureLA8,		2,	&Image::DecompressImageRLE_LA16,	&Image::DecompressImageRLE_LA16},
		{kTextureL8,		Render::kTextureL8,				Render::kTextureL8,			1,	&Image::DecompressImageRLE_L8,		&Image::DecompressImageRLE_L8},
		{kTextureI8,		Render::kTextureI8,				Render::kTextureI8,			1,	&Image::DecompressImageRLE_L8,		&Image::DecompressImageRLE_L8},
		{kTextureBC13,		Render::kTextureBC3,			Render::kTextureBC1,		0,	&Image::DecompressImageRLE_BC3,		&Image::DecompressImageRLE_BC1},
		{kTextureDEPTH16,	Render::kTextureDepth16,		Render::kTextureDepth16,	2,	&Image::DecompressImageRLE_DEPTH16,	&Image::DecompressImageRLE_DEPTH16},
		{kTextureDEPTH24,	Render::kTextureDepth24,		Render::kTextureDepth24,	4,	nullptr,							nullptr}
	};
	
	
	void ReverseHeader(TextureHeader *header, const TextureHeader *previousHeader = nullptr)
	{
		Reverse(&header->textureType);
		Reverse(&header->textureFlags);
		
		Reverse(&header->colorSemantic);
		Reverse(&header->alphaSemantic);
		Reverse(&header->imageFormat);
		
		Reverse(&header->imageWidth);
		Reverse(&header->imageHeight);
		Reverse(&header->imageDepth);
		
		Reverse(&header->wrapMode[0]);
		Reverse(&header->wrapMode[1]);
		Reverse(&header->wrapMode[2]);
		
		Reverse(&header->mipmapCount);
		Reverse(&header->mipmapDataOffset);
		
		Reverse(&header->auxiliaryDataSize);
		Reverse(&header->auxiliaryDataOffset);
		
		int32 offset = header->mipmapDataOffset;
		if (offset != 0)
		{
			int32 count = header->mipmapCount;
			if (header->textureType == kTextureCube) count *= 6;
			
			TextureMipmapData *data = header->GetMipmapData();
			for (machine a = 0; a < count; a++)
			{
				Reverse(&data->imageOffset);
				Reverse(&data->imageSize);
				Reverse(&data->chainSize);
				Reverse(&data->compressionType);
				data++;
			}
		}
		
		offset = header->auxiliaryDataOffset;
		if (offset != 0)
		{
			C4::ChunkHeader *chunk = header->GetAuxiliaryData();
			if ((!previousHeader) || (chunk != previousHeader->GetAuxiliaryData()))
			{
				unsigned_int32 size = header->auxiliaryDataSize;
				while (size > 0)
				{
					Reverse(chunk);
					switch (chunk->chunkType)
					{
						case kTextureChunkParallaxScale: 
							
							Reverse(reinterpret_cast<float *>(chunk + 1)); 
							break; 
						 
						case kTextureChunkImageCenter:
							 
							Reverse(reinterpret_cast<Point2D *>(chunk + 1));
							break;
						
						case kTextureChunkPaletteSize: 
						{
							int32 *size = reinterpret_cast<int32 *>(chunk + 1);
							Reverse(&size[0]);
							Reverse(&size[1]); 
							break;
						}
						
						case kTextureChunkImpostorSize:
						{
							float *size = reinterpret_cast<float *>(chunk + 1);
							Reverse(&size[0]);
							Reverse(&size[1]);
							break;
						}
						
						case kTextureChunkImpostorClipData:
						{
							float *size = reinterpret_cast<float *>(chunk + 1);
							Reverse(&size[0]);
							Reverse(&size[1]);
							Reverse(&size[2]);
							Reverse(&size[3]);
							break;
						}
						
						case kTextureChunkReferenceList:
						{
							int32 *count = reinterpret_cast<int32 *>(chunk + 1);
							Reverse(count);
							break;
						}
					}
					
					size -= sizeof(C4::ChunkHeader) + chunk->chunkSize;
					chunk = chunk->GetNextChunk();
				}
			}
		}
	}
}


TextureResource::TextureResource(const char *name, ResourceCatalog *catalog) : Resource<TextureResource>(name, catalog)
{
	for (machine a = 0; a < kMaxResourceTextureCount; a++) owningTexture[a] = nullptr;
}

TextureResource::~TextureResource()
{
}

void TextureResource::Preprocess(void)
{
	TextureResourceHeader *resourceHeader = static_cast<TextureResourceHeader *>(GetData());
	if (resourceHeader->endian != 1)
	{
		Reverse(&resourceHeader->headerDataSize);
		Reverse(&resourceHeader->textureCount);
		
		TextureHeader *textureHeader = reinterpret_cast<TextureHeader *>(resourceHeader + 1);
		ReverseHeader(&textureHeader[0]);
		int32 count = resourceHeader->textureCount;
		for (machine a = 1; a < count; a++) ReverseHeader(&textureHeader[a], &textureHeader[a - 1]);
	}
}

ResourceResult TextureResource::LoadHeaderData(ResourceLoader *loader, TextureResourceHeader *resourceHeader, TextureHeader **textureHeader) const
{
	ResourceResult result = loader->Read(resourceHeader, 0, sizeof(TextureResourceHeader));
	if (result != kResourceOkay) return (result);
	
	int32 endian = resourceHeader->endian;
	if (endian != 1)
	{
		Reverse(&resourceHeader->headerDataSize);
		Reverse(&resourceHeader->textureCount);
	}
	
	unsigned_int32 size = resourceHeader->headerDataSize;
	char *storage = new char[size];
	
	result = loader->Read(storage, sizeof(TextureResourceHeader), size);
	if (result != kResourceOkay)
	{
		delete[] storage;
		return (result);
	}
	
	TextureHeader *header = reinterpret_cast<TextureHeader *>(storage);
	if (endian != 1)
	{
		ReverseHeader(&header[0]);
		int32 count = resourceHeader->textureCount;
		for (machine a = 1; a < count; a++) ReverseHeader(&header[a], &header[a - 1]);
	}
	
	*textureHeader = header;
	return (kResourceOkay);
}

ResourceResult TextureResource::LoadImageData(ResourceLoader *loader, const TextureResourceHeader *resourceHeader, const TextureHeader *textureHeader, int32 index, int32 level, void **imageData) const
{
	const TextureMipmapData *mipmapData = textureHeader->GetMipmapData() + level;
	unsigned_int32 size = mipmapData->chainSize;
	char *storage = new char[size];
	
	unsigned_int32 start = sizeof(TextureResourceHeader) + index * sizeof(TextureHeader) + textureHeader->mipmapDataOffset + level * sizeof(TextureMipmapData) + mipmapData->imageOffset;
	ResourceResult result = loader->Read(storage, start, size);
	if (result != kResourceOkay)
	{
		delete[] storage;
		return (result);
	}
	
	if ((resourceHeader->endian != 1) && (textureHeader->imageFormat == kTextureDEPTH16))
	{
		int32 count = size / 2;
		unsigned_int16 *depth = reinterpret_cast<unsigned_int16 *>(storage);
		for (machine a = 0; a < count; a++)
		{
			Reverse(depth);
			depth++;
		}
	}
	
	*imageData = storage;
	return (kResourceOkay);
}

void TextureResource::ReleaseHeaderData(TextureHeader *textureHeader)
{
	delete[] reinterpret_cast<char *>(textureHeader);
}

void TextureResource::ReleaseImageData(void *imageData)
{
	delete[] static_cast<char *>(imageData);
}


Texture::Texture(TextureResource *resource, int32 index)
{
	textureResource = resource;
	textureIndex = index;
	
	resource->SetOwningTexture(index, this);
	
	textureInfo.textureHeader = nullptr;
	textureInfo.imagePointer = nullptr;
	
	activeFlag = false;
	impostorClipFlag = false;
}

Texture::Texture(const TextureHeader *header, const void *image)
{
	textureResource = nullptr;
	textureIndex = 0;
	
	textureInfo.textureHeader = header;
	textureInfo.imagePointer = image;
	
	activeFlag = false;
	impostorClipFlag = false;
}

Texture::~Texture()
{
	Deactivate();
	
	TextureResource *resource = textureResource;
	if (resource)
	{
		resource->SetOwningTexture(textureIndex, nullptr);
		resource->Release();
	}
}

unsigned_int32 Texture::GetTextureTarget(void)
{
	const GraphicsCapabilities *capabilities = TheGraphicsMgr->GetCapabilities();
	
	unsigned_int32 target = Render::kTextureTarget2D;
	int32 maxTextureSize = capabilities->maxTextureSize;
	int32 biggestDimension = Max(textureWidth, textureHeight);
	
	switch (textureType)
	{
		case kTexture3D:
			
			target = Render::kTextureTarget3D;
			maxTextureSize = capabilities->max3DTextureSize;
			biggestDimension = Max(biggestDimension, textureDepth);
			break;
		
		case kTextureCube:
			
			target = Render::kTextureTargetCube;
			maxTextureSize = capabilities->maxCubeTextureSize;
			break;
		
		case kTextureRectangle:
			
			target = Render::kTextureTargetRectangle;
			break;
		
		case kTextureArray2D:
			
			target = Render::kTextureTargetArray2D;
			break;
	}
	
	int32 skipCount = 0;
	unsigned_int32 flags = textureFlags;
	if (!(flags & kTextureForceHighQuality))
	{
		skipCount = TheGraphicsMgr->GetTextureDetailLevel();
		if (flags & kTextureImagePalette)
		{
			int32 size = (GetTextureTargetIndex() == Render::kTextureTarget2D) ? 4096 : 1024;
			if (biggestDimension >= size) skipCount = Max(skipCount, TheGraphicsMgr->GetPaletteDetailLevel());
		}
		
		biggestDimension >>= skipCount;
	}
	
	while (biggestDimension > maxTextureSize)
	{
		biggestDimension >>= 1;
		skipCount++;
	}
	
	baseMipmapLevel = Min(skipCount, mipmapLevelCount - 1);
	return (target);
}

const TextureFormatData *Texture::GetTextureFormatData(TextureFormat imageFormat)
{
	const TextureFormatData *formatData = textureFormatDataTable;
	for (machine a = 0; a < kTextureFormatCount; a++)
	{
		if (formatData->engineFormat == imageFormat) break;
		formatData++;
	}
	
	return (formatData);
}

Render::Decompressor *Texture::GetDecompressor(const TextureHeader *textureHeader, const TextureMipmapData *mipmapData)
{
	CompressionType type = mipmapData->compressionType;
	if (type == kTextureCompressionGeneral)
	{
		return (&Comp::DecompressData);
	}
	else if (type == kTextureCompressionRunLength)
	{
		const TextureFormatData *formatData = GetTextureFormatData(textureHeader->imageFormat);
		return ((textureHeader->alphaSemantic != kTextureSemanticNone) ? formatData->decompressorAlpha : formatData->decompressor);
	}
	
	return (nullptr);
}

unsigned_int32 Texture::GetTextureWrapMode(TextureWrap mode)
{
	switch (mode)
	{
		case kTextureClamp:
			
			return (Render::kWrapClampToEdge);
		
		case kTextureClampBorder:
			
			return (Render::kWrapClampToBorder);
		
		case kTextureMirrorRepeat:
			
			return (Render::kWrapMirrorRepeat);
		
		case kTextureMirrorClamp:
			
			return (Render::kWrapMirrorClampToEdge);
		
		case kTextureMirrorClampBorder:
			
			return (Render::kWrapMirrorClampToBorder);
	}
	
	return (Render::kWrapRepeat);
}

void Texture::SetTextureParameters(const TextureHeader *header)
{
	SetSWrapMode(GetTextureWrapMode(header->wrapMode[0]));
	SetTWrapMode(GetTextureWrapMode(header->wrapMode[1]));
	if (GetTextureTargetIndex() == Render::kTextureTarget3D) SetRWrapMode(GetTextureWrapMode(header->wrapMode[2]));
	
	const GraphicsCapabilities *capabilities = TheGraphicsMgr->GetCapabilities();
	const bool *extensionFlag = capabilities->extensionFlag;
	
	unsigned_int32 flags = textureFlags;
	bool palette2D = ((flags & kTextureImagePalette) && (GetTextureTargetIndex() == Render::kTextureTarget2D));
	
	if (palette2D)
	{
		int32 x = Cntlz(textureWidth >> baseMipmapLevel);
		const unsigned_int32 *size = GetPaletteSize();
		SetLodBias((float) (Cntlz(Min(size[0], size[1])) - x - 1));
		SetMaxLod(mipmapLevelCount - baseMipmapLevel - 1);
	}
	
	if (flags & kTextureFilterInhibit)
	{
		SetMinFilterMode((mipmapLevelCount > 1) ? Render::kFilterNearestMipmapNearest : Render::kFilterNearest);
		SetMagFilterMode(Render::kFilterNearest);
	}
	else
	{
		SetMinFilterMode((mipmapLevelCount > 1) ? Render::kFilterLinearMipmapLinear : Render::kFilterLinear);
		SetMagFilterMode(Render::kFilterLinear);
		
		if ((extensionFlag[kExtensionTextureFilterAnisotropic]) && (!(flags & kTextureAnisotropicFilterInhibit)))
		{
			if ((!palette2D) && (GetTextureTargetIndex() != Render::kTextureTarget3D))
			{
				SetMaxAnisotropy(Fmin((float) TheGraphicsMgr->GetTextureFilterAnisotropy(), capabilities->maxTextureAnisotropy));
			}
		}
	}
}

const void *Texture::ProcessAuxiliaryData(const TextureHeader *header)
{
	const void *pointerTextureData = nullptr;
	
	if (header->auxiliaryDataOffset != 0)
	{
		const ChunkHeader *chunk = header->GetAuxiliaryData();
		unsigned_int32 size = header->auxiliaryDataSize;
		
		while (size > 0)
		{
			switch (chunk->chunkType)
			{
				case kTextureChunkParallaxScale:
				{
					float scale = *reinterpret_cast<const float *>(chunk + 1);
					floatTextureData[0] = scale / (float) textureWidth;
					floatTextureData[1] = scale / (float) textureHeight;
					break;
				}
				
				case kTextureChunkImageCenter:
				{
					const float *center = reinterpret_cast<const float *>(chunk + 1);
					floatTextureData[0] = center[0];
					floatTextureData[1] = center[1];
					break;
				}
				
				case kTextureChunkPaletteSize:
				{
					const unsigned_int32 *size = reinterpret_cast<const unsigned_int32 *>(chunk + 1);
					integerTextureData[0] = size[0];
					integerTextureData[1] = size[1];
					break;
				}
				
				case kTextureChunkImpostorSize:
				{
					const float *size = reinterpret_cast<const float *>(chunk + 1);
					floatTextureData[0] = size[0];
					floatTextureData[1] = size[1];
					break;
				}
				
				case kTextureChunkImpostorClipData:
				{
					const float *data = reinterpret_cast<const float *>(chunk + 1);
					floatTextureData[2] = data[0];
					floatTextureData[3] = data[1];
					floatTextureData[4] = data[2];
					floatTextureData[5] = data[3];
					impostorClipFlag = true;
					break;
				}
				
				case kTextureChunkReferenceList:
				{
					const int32 *pointer = reinterpret_cast<const int32 *>(chunk + 1);
					pointerTextureData = pointer;
					int32 count = *pointer;
					
					if (TheGraphicsMgr->GetCapabilities()->capabilityFlag[kCapabilityTextureArray])
					{
						textureType = kTextureArray2D;
						textureDepth = count;
						
						if (count <= 9)
						{
							integerTextureData[0] = 3;
							integerTextureData[1] = 3;
						}
						else if (count <= 18)
						{
							integerTextureData[0] = 6;
							integerTextureData[1] = 3;
						}
						else
						{
							integerTextureData[0] = 6;
							integerTextureData[1] = 6;
						}
					}
					else
					{
						if (count <= 9)
						{
							integerTextureData[0] = 3;
							integerTextureData[1] = 3;
							
							textureWidth *= 4;
							textureHeight *= 4;
						}
						else if (count <= 18)
						{
							integerTextureData[0] = 6;
							integerTextureData[1] = 3;
							
							textureWidth *= 8;
							textureHeight *= 4;
						}
						else
						{
							integerTextureData[0] = 6;
							integerTextureData[1] = 6;
							
							textureWidth *= 8;
							textureHeight *= 8;
						}
						
						// Reduce mipmap count by 3 for 1/8 border, and by 2 for 4x4 compressed block size.
						
						mipmapLevelCount -= 5;
					}
					
					textureFlags |= kTextureImagePalette;
					break;
				}
			}
			
			size -= sizeof(ChunkHeader) + chunk->chunkSize;
			chunk = chunk->GetNextChunk();
		}
	}
	
	return (pointerTextureData);
}

void Texture::Activate(const TextureHeader *header, const void *image)
{
	Render::TextureImageData	textureImageData[60];
	
	textureType = header->textureType;
	textureFlags = header->textureFlags;
	textureWidth = header->imageWidth;
	textureHeight = header->imageHeight;
	textureDepth = header->imageDepth;
	
	baseMipmapLevel = 0;
	mipmapLevelCount = header->mipmapCount;
	
	alphaSemantic = header->alphaSemantic;
	imageFormat = header->imageFormat;
	
	Construct(GetTextureTarget());
	SetTextureParameters(header);
	
	bool alpha = (header->alphaSemantic != kTextureSemanticNone);
	const TextureFormatData *formatData = GetTextureFormatData(header->imageFormat);
	unsigned_int32 renderFormat = (alpha) ? formatData->renderFormatAlpha : formatData->renderFormat;
	
	unsigned_int32 flags = header->textureFlags;
	if (flags & kTextureRenderTarget)
	{
		renderFormat = (alpha) ? Render::kTextureRenderBufferRGBA8 : Render::kTextureRenderBufferRGB8;
	}
	else if ((!image) && (!(flags & kTextureExternalStorage)))
	{
		if (header->mipmapDataOffset == 0) image = header + 1;
		else image = header->GetMipmapData()->GetMipmapImage();
	}
	
	unsigned_int32 memorySize = 0;
	
	switch (textureType)
	{
		case kTexture2D:
		{
			unsigned_int32 width = textureWidth;
			unsigned_int32 height = textureHeight;
			
			int32 mipmapCount = mipmapLevelCount;
			for (machine level = 0; level < mipmapCount; level++)
			{
				unsigned_int32 size = width * height * formatData->pixelSize;
				textureImageData[level].image = image;
				textureImageData[level].size = size;
				textureImageData[level].decompressor = nullptr;
				image = static_cast<const char *>(image) + size;
				
				width = Max(width >> 1, 1);
				height = Max(height >> 1, 1);
			}
			
			memorySize = SetImage2D(renderFormat, textureWidth, textureHeight, mipmapCount, textureImageData);
			break;
		}
		
		case kTexture3D:
		{
			unsigned_int32 width = textureWidth;
			unsigned_int32 height = textureHeight;
			unsigned_int32 depth = textureDepth;
			
			int32 mipmapCount = mipmapLevelCount;
			for (machine level = 0; level < mipmapCount; level++)
			{
				unsigned_int32 size = width * height * depth * formatData->pixelSize;
				textureImageData[level].image = image;
				textureImageData[level].size = size;
				textureImageData[level].decompressor = nullptr;
				image = static_cast<const char *>(image) + size;
				
				width = Max(width >> 1, 1);
				height = Max(height >> 1, 1);
				depth = Max(depth >> 1, 1);
			}
			
			memorySize = SetImage3D(renderFormat, textureWidth, textureHeight, textureDepth, mipmapCount, textureImageData);
			break;
		}
		
		case kTextureCube:
		{
			unsigned_int32 width = textureWidth;
			
			int32 mipmapCount = mipmapLevelCount;
			for (machine level = 0; level < mipmapCount; level++)
			{
				for (machine component = 0; component < 6; component++)
				{
					unsigned_int32 size = width * width * formatData->pixelSize;
					textureImageData[level * 6 + component].image = image;
					textureImageData[level * 6 + component].size = size;
					textureImageData[level * 6 + component].decompressor = nullptr;
					image = static_cast<const char *>(image) + size;
				}
				
				width = Max(width >> 1, 1);
			}
			
			memorySize = SetImageCube(renderFormat, textureWidth, mipmapCount, textureImageData);
			break;
		}
		
		case kTextureRectangle:
			
			if (image)
			{
				unsigned_int32 width = textureWidth;
				unsigned_int32 height = textureHeight;
				
				textureImageData[0].image = image;
				textureImageData[0].size = width * height * formatData->pixelSize;
				textureImageData[0].decompressor = nullptr;
				
				memorySize = SetImageRect(renderFormat, width, height, width, &textureImageData[0]);
			}
			else
			{
				memorySize = AllocateStorageRect(renderFormat, textureWidth, textureHeight, false);
			}
			
			break;
	}
	
	textureMemorySize = memorySize;
	totalTextureMemory += memorySize;
}

ResourceResult Texture::Activate(void)
{
	if (activeFlag) return (kResourceOkay);
	
	if (textureInfo.textureHeader)
	{
		Activate(textureInfo.textureHeader, textureInfo.imagePointer);
	}
	else
	{
		ResourceLoader				loader;
		void						*imageData;
		TextureHeader				*textureHeader;
		TextureResourceHeader		resourceHeader;
		Render::TextureImageData	textureImageData[66];
		
		ResourceResult result = textureResource->OpenLoader(&loader);
		if (result != kResourceOkay) return (result);
		
		result = textureResource->LoadHeaderData(&loader, &resourceHeader, &textureHeader);
		if (result != kResourceOkay) return (result);
		
		int32 textureLoadIndex = Min(textureIndex, resourceHeader.textureCount - 1);
		if (!TheGraphicsMgr->GetCapabilities()->capabilityFlag[kCapabilityTextureArray])
		{
			while (textureHeader[textureLoadIndex].textureType == kTextureArray2D) textureLoadIndex++;
		}
		
		textureHeader += textureLoadIndex;
		
		textureType = textureHeader->textureType;
		textureFlags = textureHeader->textureFlags;
		textureWidth = textureHeader->imageWidth;
		textureHeight = textureHeader->imageHeight;
		textureDepth = textureHeader->imageDepth;
		
		baseMipmapLevel = 0;
		mipmapLevelCount = textureHeader->mipmapCount;
		
		alphaSemantic = textureHeader->alphaSemantic;
		imageFormat = textureHeader->imageFormat;
		
		floatTextureData[0] = 0.0F;
		floatTextureData[1] = 0.0F;
		integerTextureData[0] = 0;
		integerTextureData[1] = 0;
		
		const void *pointerTextureData = ProcessAuxiliaryData(textureHeader);
		
		Construct(GetTextureTarget());
		SetTextureParameters(textureHeader);
		
		const TextureFormatData *formatData = GetTextureFormatData(textureHeader->imageFormat);
		unsigned_int32 renderFormat = (textureHeader->alphaSemantic != kTextureSemanticNone) ? formatData->renderFormatAlpha : formatData->renderFormat;
		
		unsigned_int32 memorySize = 0;
		
		if (!(textureFlags & kTextureReferenceList))
		{
			int32 componentCount = (textureType != kTextureCube) ? 1 : 6;
			int32 mipmapDataSkipCount = baseMipmapLevel * componentCount;
			
			result = textureResource->LoadImageData(&loader, &resourceHeader, textureHeader, textureLoadIndex, mipmapDataSkipCount, &imageData);
			if (result != kResourceOkay)
			{
				TextureResource::ReleaseHeaderData(textureHeader);
				return (result);
			}
			
			const char *image = static_cast<char *>(imageData);
			int32 mipmapCount = mipmapLevelCount - baseMipmapLevel;
			const TextureMipmapData *mipmapData = textureHeader->GetMipmapData() + mipmapDataSkipCount;
			
			switch (textureType)
			{
				case kTexture2D:
				{
					for (machine level = 0; level < mipmapCount; level++)
					{
						unsigned_int32 size = mipmapData->imageSize;
						textureImageData[level].image = image;
						textureImageData[level].size = size;
						textureImageData[level].decompressor = GetDecompressor(textureHeader, mipmapData);
						image += size;
						mipmapData++;
					}
					
					unsigned_int32 width = textureWidth >> baseMipmapLevel;
					unsigned_int32 height = textureHeight >> baseMipmapLevel;
					
					if (formatData->engineFormat == kTextureBC13) memorySize = SetCompressedImage2D(renderFormat, width, height, mipmapCount, textureImageData);
					else memorySize = SetImage2D(renderFormat, width, height, mipmapCount, textureImageData);
					break;
				}
				
				case kTexture3D:
				{
					for (machine level = 0; level < mipmapCount; level++)
					{
						unsigned_int32 size = mipmapData->imageSize;
						textureImageData[level].image = image;
						textureImageData[level].size = size;
						textureImageData[level].decompressor = GetDecompressor(textureHeader, mipmapData);
						image += size;
						mipmapData++;
					}
					
					unsigned_int32 width = textureWidth >> baseMipmapLevel;
					unsigned_int32 height = textureHeight >> baseMipmapLevel;
					unsigned_int32 depth = textureDepth >> baseMipmapLevel;
					
					memorySize = SetImage3D(renderFormat, width, height, depth, mipmapCount, textureImageData);
					break;
				}
				
				case kTextureCube:
				{
					int32 i = 0;
					for (machine level = 0; level < mipmapCount; level++)
					{
						for (machine component = 0; component < 6; component++)
						{
							unsigned_int32 size = mipmapData->imageSize;
							textureImageData[i].image = image;
							textureImageData[i].size = size;
							textureImageData[i].decompressor = GetDecompressor(textureHeader, mipmapData);
							image += size;
							mipmapData++;
							i++;
						}
					}
					
					unsigned_int32 width = textureWidth >> baseMipmapLevel;
					
					if (formatData->engineFormat == kTextureBC13) memorySize = SetCompressedImageCube(renderFormat, width, mipmapCount, textureImageData);
					else memorySize = SetImageCube(renderFormat, width, mipmapCount, textureImageData);
					break;
				}
				
				case kTextureRectangle:
				{
					unsigned_int32 width = textureWidth;
					unsigned_int32 height = textureHeight;
					
					textureImageData[0].image = image;
					textureImageData[0].size = mipmapData->imageSize;
					textureImageData[0].decompressor = GetDecompressor(textureHeader, mipmapData);
					
					memorySize = SetImageRect(renderFormat, width, height, width, &textureImageData[0]);
					break;
				}
				
				#if !C4PLAYSTATION3
				
					case kTextureArray2D:
					{
						for (machine level = 0; level < mipmapCount; level++)
						{
							unsigned_int32 size = mipmapData->imageSize;
							textureImageData[level].image = image;
							textureImageData[level].size = size;
							textureImageData[level].decompressor = GetDecompressor(textureHeader, mipmapData);
							image += size;
							mipmapData++;
						}
						
						unsigned_int32 width = textureWidth >> baseMipmapLevel;
						unsigned_int32 height = textureHeight >> baseMipmapLevel;
						unsigned_int32 depth = textureDepth;
						
						if (formatData->engineFormat == kTextureBC13) memorySize = SetCompressedImageArray2D(renderFormat, width, height, depth, mipmapCount, textureImageData);
						else memorySize = SetImageArray2D(renderFormat, width, height, depth, mipmapCount, textureImageData);
						break;
					}
				
				#endif
			}
			
			TextureResource::ReleaseImageData(imageData);
		}
		else
		{
			const int32 *pointer = static_cast<const int32 *>(pointerTextureData);
			const ResourceName *textureName = reinterpret_cast<const ResourceName *>(pointer + 1);
			
			#if !C4PLAYSTATION3
			
				if (textureType == kTextureArray2D)
				{
					unsigned_int32 width = textureHeader->imageWidth >> baseMipmapLevel;
					unsigned_int32 height = textureHeader->imageHeight >> baseMipmapLevel;
					
					int32 storageSize = Image::CalculateBlockMipmapChainSize2D(width, height, mipmapLevelCount - baseMipmapLevel);
					if (alphaSemantic == kTextureSemanticNone) storageSize >>= 1;
					
					int32 entryCount = textureDepth;
					storageSize *= entryCount;
					unsigned_int8 *storage = new unsigned_int8[storageSize];
					
					for (machine a = 0; a < entryCount; a++)
					{
						result = LoadReferencedArrayImage(textureName[a], storage, a, entryCount);
						if (result != kResourceOkay) break;
					}
					
					unsigned_int32 blockSize = (alphaSemantic == kTextureSemanticNone) ? 8 : 16;
					int32 mipmapCount = mipmapLevelCount - baseMipmapLevel;
					unsigned_int8 *image = storage;
					
					for (machine level = 0; level < mipmapCount; level++)
					{
						int32 blockCount = ((width + 3) >> 2) * ((height + 3) >> 2);
						unsigned_int32 size = blockCount * blockSize * entryCount;
						
						textureImageData[level].image = image;
						textureImageData[level].size = size;
						textureImageData[level].decompressor = nullptr;
						
						width = Max(width >> 1, 1);
						height = Max(height >> 1, 1);
						image += size;
					}
					
					width = textureWidth >> baseMipmapLevel;
					height = textureHeight >> baseMipmapLevel;
					memorySize = SetCompressedImageArray2D(renderFormat, width, height, entryCount, mipmapCount, textureImageData);
					
					delete[] storage;
				}
				else
				{
			
			#endif
			
					unsigned_int32 width = textureWidth >> baseMipmapLevel;
					unsigned_int32 height = textureHeight >> baseMipmapLevel;
					
					int32 storageSize = Image::CalculateBlockMipmapChainSize2D(width, height, mipmapLevelCount - baseMipmapLevel);
					if (alphaSemantic == kTextureSemanticNone) storageSize >>= 1;
					unsigned_int8 *storage = new unsigned_int8[storageSize];
					
					int32 entryCount = *pointer;
					unsigned_int32 paletteWidth = GetPaletteSize()[0];
					int32 x = 0;
					int32 y = 0;
					
					for (machine a = 0; a < entryCount; a++)
					{
						result = LoadReferencedPaletteImage(textureName[a], storage, x, y);
						if (result != kResourceOkay) break;
						
						if (++x == paletteWidth)
						{
							x = 0;
							y++;
						}
					}
					
					unsigned_int32 blockSize = (alphaSemantic == kTextureSemanticNone) ? 8 : 16;
					int32 mipmapCount = mipmapLevelCount - baseMipmapLevel;
					unsigned_int8 *image = storage;
					
					for (machine level = 0; level < mipmapCount; level++)
					{
						int32 blockCount = ((width + 3) >> 2) * ((height + 3) >> 2);
						unsigned_int32 size = blockCount * blockSize;
						
						textureImageData[level].image = image;
						textureImageData[level].size = size;
						textureImageData[level].decompressor = nullptr;
						
						width = Max(width >> 1, 1);
						height = Max(height >> 1, 1);
						image += size;
					}
					
					width = textureWidth >> baseMipmapLevel;
					height = textureHeight >> baseMipmapLevel;
					memorySize = SetCompressedImage2D(renderFormat, width, height, mipmapCount, textureImageData);
					
					delete[] storage;
			
			#if !C4PLAYSTATION3
			
				}
			
			#endif
		}
		
		TextureResource::ReleaseHeaderData(textureHeader - textureLoadIndex);
		
		textureMemorySize = memorySize;
		totalTextureMemory += memorySize;
	}
	
	activeFlag = true;
	totalTextureCount++;
	
	if (!GetOwningList()) textureList.Append(this);
	return (kResourceOkay);
}

void Texture::Deactivate(void)
{
	if (activeFlag)
	{
		activeFlag = false;
		
		totalTextureCount--;
		totalTextureMemory -= textureMemorySize;
		
		Destruct();
	}
}

template <typename type> void Texture::CopyPixelImage(const type *source, int32 sourceRowSize, int32 sx, int32 sy, type *restrict destin, int32 destinRowSize, int32 dx, int32 dy, int32 width, int32 height)
{
	source += sy * sourceRowSize + sx;
	destin += dy * destinRowSize + dx;
	
	for (machine j = 0; j < height; j++)
	{
		for (machine i = 0; i < width; i++) destin[i] = source[i];
		
		source += sourceRowSize;
		destin += destinRowSize;
	}
}

template <typename type> void Texture::CreatePaletteEntry(const type *source, int32 width, type *restrict destin, int32 destinRowSize, int32 x, int32 y)
{
	int32 border = width >> 3;
	int32 inner = width - border;
	
	int32 entryWidth = width + border * 2;
	destin += y * entryWidth * destinRowSize + x * entryWidth;
	
	CopyPixelImage(source, width, inner, inner, destin, destinRowSize, 0, 0, border, border);
	CopyPixelImage(source, width, 0, inner, destin, destinRowSize, border, 0, width, border);
	CopyPixelImage(source, width, 0, inner, destin, destinRowSize, width + border, 0, border, border);
	
	CopyPixelImage(source, width, inner, 0, destin, destinRowSize, 0, border, border, width);
	CopyPixelImage(source, width, 0, 0, destin, destinRowSize, border, border, width, width);
	CopyPixelImage(source, width, 0, 0, destin, destinRowSize, width + border, border, border, width);
	
	CopyPixelImage(source, width, inner, 0, destin, destinRowSize, 0, width + border, border, border);
	CopyPixelImage(source, width, 0, 0, destin, destinRowSize, border, width + border, width, border);
	CopyPixelImage(source, width, 0, 0, destin, destinRowSize, width + border, width + border, border, border);
}

ResourceResult Texture::LoadReferencedArrayImage(const char *name, unsigned_int8 *finalImage, int32 entryIndex, int32 entryCount) const
{
	ResourceLoader				loader;
	void						*imageData;
	TextureHeader				*textureHeader;
	TextureResourceHeader		resourceHeader;
	
	TextureResource *resource = TextureResource::Get(name, kResourceDeferLoad);
	ResourceResult result = resource->OpenLoader(&loader);
	if (result == kResourceOkay)
	{
		result = textureResource->LoadHeaderData(&loader, &resourceHeader, &textureHeader);
		if (result == kResourceOkay)
		{
			result = textureResource->LoadImageData(&loader, &resourceHeader, textureHeader, 0, baseMipmapLevel, &imageData);
			if (result == kResourceOkay)
			{
				const unsigned_int8 *image = static_cast<unsigned_int8 *>(imageData);
				int32 mipmapCount = textureHeader->mipmapCount - baseMipmapLevel;
				const TextureMipmapData *mipmapData = textureHeader->GetMipmapData() + baseMipmapLevel;
				
				int32 width = textureHeader->imageWidth >> baseMipmapLevel;
				int32 height = textureHeader->imageHeight >> baseMipmapLevel;
				unsigned_int32 blockSize = (alphaSemantic == kTextureSemanticNone) ? 8 : 16;
				
				for (machine level = 0; level < mipmapCount; level++)
				{
					int32 blockCount = ((width + 3) >> 2) * ((height + 3) >> 2);
					finalImage += blockCount * blockSize * entryIndex;
					
					unsigned_int32 size = mipmapData->imageSize;
					Render::Decompressor *decompressor = GetDecompressor(textureHeader, mipmapData);
					
					if (decompressor) (*decompressor)(image, size, finalImage);
					else MemoryMgr::CopyMemory(image, finalImage, size);
					
					width = Max(width >> 1, 1);
					height = Max(height >> 1, 1);
					mipmapData++;
					
					image += size;
					finalImage += blockCount * blockSize * (entryCount - entryIndex);
				}
				
				TextureResource::ReleaseImageData(imageData);
			}
			
			TextureResource::ReleaseHeaderData(textureHeader);
		}
	}
	
	resource->Release();
	return (result);
}

ResourceResult Texture::LoadReferencedPaletteImage(const char *name, unsigned_int8 *finalImage, int32 entryX, int32 entryY) const
{
	ResourceLoader				loader;
	void						*imageData;
	TextureHeader				*textureHeader;
	TextureResourceHeader		resourceHeader;
	
	TextureResource *resource = TextureResource::Get(name, kResourceDeferLoad);
	ResourceResult result = resource->OpenLoader(&loader);
	if (result == kResourceOkay)
	{
		result = textureResource->LoadHeaderData(&loader, &resourceHeader, &textureHeader);
		if (result == kResourceOkay)
		{
			result = textureResource->LoadImageData(&loader, &resourceHeader, textureHeader, 0, baseMipmapLevel, &imageData);
			if (result == kResourceOkay)
			{
				const unsigned_int8 *image = static_cast<unsigned_int8 *>(imageData);
				int32 mipmapCount = mipmapLevelCount - baseMipmapLevel;
				const TextureMipmapData *mipmapData = textureHeader->GetMipmapData() + baseMipmapLevel;
				
				int32 width = textureHeader->imageWidth >> baseMipmapLevel;
				int32 height = textureHeader->imageHeight >> baseMipmapLevel;
				unsigned_int32 blockSize = (alphaSemantic == kTextureSemanticNone) ? 8 : 16;
				
				int32 blockCount = (width * height) >> 4;
				unsigned_int8 *mipmapImage = new unsigned_int8[blockCount * blockSize];
				
				for (machine level = 0; level < mipmapCount; level++)
				{
					unsigned_int32 size = mipmapData->imageSize;
					Render::Decompressor *decompressor = GetDecompressor(textureHeader, mipmapData);
					
					const unsigned_int8 *sourceImage = image;
					if (decompressor)
					{
						(*decompressor)(image, size, mipmapImage);
						sourceImage = mipmapImage;
					}
					
					int32 w = (width + 3) >> 2;
					int32 rowSize = Max((textureWidth >> 2) >> (baseMipmapLevel + level), 1);
					
					if (alphaSemantic == kTextureSemanticNone)
					{
						const BC1Block *source = reinterpret_cast<const BC1Block *>(sourceImage);
						BC1Block *destin = reinterpret_cast<BC1Block *>(finalImage);
						CreatePaletteEntry(source, w, destin, rowSize, entryX, entryY);
					}
					else
					{
						const BC3Block *source = reinterpret_cast<const BC3Block *>(sourceImage);
						BC3Block *destin = reinterpret_cast<BC3Block *>(finalImage);
						CreatePaletteEntry(source, w, destin, rowSize, entryX, entryY);
					}
					
					width = Max(width >> 1, 1);
					height = Max(height >> 1, 1);
					mipmapData++;
					
					image += size;
					finalImage += rowSize * Max((textureHeight >> 2) >> (baseMipmapLevel + level), 1) * blockSize;
				}
				
				delete[] mipmapImage;
				TextureResource::ReleaseImageData(imageData);
			}
			
			TextureResource::ReleaseHeaderData(textureHeader);
		}
	}
	
	resource->Release();
	return (result);
}

Texture *Texture::Get(const char *name, int32 index)
{
	TextureResource *resource = TextureResource::Get(name, kResourceDeferLoad);
	
	Texture *texture = resource->GetOwningTexture(index);
	if (texture)
	{
		texture->Retain();
		resource->Release();
		return (texture);
	}
	
	texture = new Texture(resource, index);
	if (texture->Activate() == kResourceOkay) return (texture);
	
	delete texture;
	return (nullptr);
}

Texture *Texture::Get(const TextureHeader *header, const void *image)
{
	TextureInfo		info;
	
	info.textureHeader = header;
	info.imagePointer = image;
	
	Texture *texture = textureHeaderMap.Find(info);
	if (texture)
	{
		texture->Retain();
		return (texture);
	}
	
	texture = new Texture(header, image);
	if ((header->textureFlags & kTextureDynamic) || (texture->Activate() == kResourceOkay))
	{
		textureHeaderMap.Insert(texture);
		return (texture);
	}
	
	delete texture;
	return (nullptr);
}

void Texture::Update(const C4::Rect& rect)
{
	if (GetOwningList())
	{
		const Color4C *image = static_cast<const Color4C *>(textureInfo.imagePointer);
		if (!image) image = static_cast<const Color4C *>(textureInfo.textureHeader->GetMipmapData()->GetMipmapImage());
		
		unsigned_int32 pitch = textureInfo.textureHeader->imageWidth;
		
		if (textureType == kTextureRectangle) UpdateImageRect(rect.left, rect.top, rect.Width(), rect.Height(), pitch, image);
		else UpdateImage2D(rect.left, rect.top, rect.Width(), rect.Height(), pitch, image);
	}
	else
	{
		Activate();
	}
}

void Texture::Update(const Rect& rect, int32 pitch, const Color4C *image)
{
	if (GetOwningList())
	{
		if (textureType == kTextureRectangle) UpdateImageRect(rect.left, rect.top, rect.Width(), rect.Height(), pitch, image);
		else UpdateImage2D(rect.left, rect.top, rect.Width(), rect.Height(), pitch, image);
	}
	else
	{
		Activate();
	}
}

void Texture::DeactivateAll(void)
{
	Texture *texture = textureList.First();
	while (texture)
	{
		texture->Deactivate();
		texture = texture->ListElement<Texture>::Next();
	}
}

void Texture::ReactivateAll(void)
{
	Texture *texture = textureList.First();
	while (texture)
	{
		texture->Activate();
		texture = texture->ListElement<Texture>::Next();
	}
}

void Texture::Reload(const char *name)
{
	Texture *texture = textureList.Last();
	while (texture)
	{
		TextureResource *resource = texture->textureResource;
		if ((resource) && (resource->GetName() == name))
		{
			texture->Deactivate();
			texture->Activate();
			
			GraphicsMgr::ResetShaders();
			break;
		}
		
		texture = texture->ListElement<Texture>::Previous();
	}
}

// ZYURVUR
