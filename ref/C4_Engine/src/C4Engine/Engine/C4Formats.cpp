//=============================================================
//
// C4 Engine version 2.10 beta
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


#include "C4Formats.h"


using namespace C4;


namespace
{
	bool TargaFilter(const char *name)
	{
		int32 len = Text::GetTextLength(name);
		return (Text::CompareTextCaseless(&name[MaxZero(len - 4)], ".tga"));
	}

	int32 CountConstantRGBAValues(const Color4C *image, int32 limit)
	{
		const Color4C& p = image[0];

		int32 count = 1;
		while (count < limit)
		{
			if (image[count] != p) break;
			count++;
		}

		return (count);
	}

	int32 CountVaryingRGBAValues(const Color4C *image, int32 limit)
	{
		if (limit < 3) return (limit);

		Color4C p = image[1];

		int32 count = 2;
		while (count < limit)
		{
			const Color4C& q = image[count];
			if (q == p) break;
			p = q;
			count++;
		}

		return (count - 1);
	}

	int32 CountConstantRGBValues(const Color4C *image, int32 limit)
	{
		unsigned_int32 p = image[0].GetPackedRGBColor();

		int32 count = 1;
		while (count < limit)
		{
			if (image[count].GetPackedRGBColor() != p) break;
			count++;
		}

		return (count);
	}

	int32 CountVaryingRGBValues(const Color4C *image, int32 limit)
	{
		if (limit < 3) return (limit);

		unsigned_int32 p = image[1].GetPackedRGBColor();

		int32 count = 2;
		while (count < limit)
		{
			unsigned_int32 q = image[count].GetPackedRGBColor();
			if (q == p) break;
			p = q;
			count++;
		}

		return (count - 1);
	}
}


ResourceDescriptor TargaResource::descriptor("tga");


TargaResource::TargaResource(const char *name, ResourceCatalog *catalog) : Resource<TargaResource>(name, catalog)
{
}

TargaResource::~TargaResource()
{
}

void TargaResource::Preprocess(void)
{
	#if C4BIGENDIAN

		TargaHeader *header = static_cast<TargaHeader *>(GetData());

		Reverse(&header->xOffset);
		Reverse(&header->yOffset);
		Reverse(&header->width);
		Reverse(&header->height);
 
	#endif
} 
 
 
TargaImageImportPlugin::TargaImageImportPlugin()
{ 
}

TargaImageImportPlugin::~TargaImageImportPlugin()
{ 
}

const char *TargaImageImportPlugin::GetImageTypeName(void) const
{ 
	return ("Targa");
}

const ResourceDescriptor *TargaImageImportPlugin::GetImageResourceDescriptor(void) const
{
	return (TargaResource::GetDescriptor());
}

ImagePlugin::ImageFileFilterProc *TargaImageImportPlugin::GetImageFileFilterProc(void) const
{
	return (&TargaFilter);
}

void TargaImageImportPlugin::CopyTarga_L8(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		unsigned_int32 value = data[0];
		image[a].Set(value, value, value, 255);
		data++;
	}
}

void TargaImageImportPlugin::CopyTarga_RGB16(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		unsigned_int32 p = ReadLittleEndianU16(reinterpret_cast<const unsigned_int16 *>(data));
		unsigned_int32 red = p >> 10;
		unsigned_int32 green = (p >> 5) & 31;
		unsigned_int32 blue = p & 31;

		red = (red << 3) | (red >> 2);
		green = (green << 3) | (green >> 2);
		blue = (blue << 3) | (blue >> 2);

		image[a].Set(red, green, blue, 255);
		data += 2;
	}
}

void TargaImageImportPlugin::CopyTarga_RGB24(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		image[a].Set(data[2], data[1], data[0], 255);
		data += 3;
	}
}

void TargaImageImportPlugin::CopyTarga_RGBA32(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount; a++)
	{
		image[a].Set(data[2], data[1], data[0], data[3]);
		data += 4;
	}
}

void TargaImageImportPlugin::DecompressTarga_L8(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount;)
	{
		unsigned_int8 d = *data++;
		int32 count = (d & 0x7F) + 1;

		if ((d & 0x80) != 0)
		{
			Color4C		c;

			unsigned_int32 value = data[0];
			c.Set(value, value, value, 255);
			do
			{
				image[a++] = c;
			} while (--count > 0);

			data++;
		}
		else
		{
			do
			{
				unsigned_int32 value = data[0];
				image[a++].Set(value, value, value, 255);
				data++;

			} while (--count > 0);
		}
	}
}

void TargaImageImportPlugin::DecompressTarga_RGB16(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount;)
	{
		unsigned_int8 d = *data++;
		int32 count = (d & 0x7F) + 1;

		if ((d & 0x80) != 0)
		{
			Color4C		c;

			unsigned_int32 p = data[0] | (data[1] << 8);
			data += 2;

			unsigned_int32 red = p >> 10;
			unsigned_int32 green = (p >> 5) & 31;
			unsigned_int32 blue = p & 31;

			red = (red << 3) | (red >> 2);
			green = (green << 3) | (green >> 2);
			blue = (blue << 3) | (blue >> 2);

			c.Set(red, green, blue, 255);
			do
			{
				image[a++] = c;
			} while (--count > 0);
		}
		else
		{
			do
			{
				unsigned_int32 p = data[0] | (data[1] << 8);
				data += 2;

				unsigned_int32 red = p >> 10;
				unsigned_int32 green = (p >> 5) & 31;
				unsigned_int32 blue = p & 31;

				red = (red << 3) | (red >> 2);
				green = (green << 3) | (green >> 2);
				blue = (blue << 3) | (blue >> 2);

				image[a++].Set(red, green, blue, 255);
			} while (--count > 0);
		}
	}
}

void TargaImageImportPlugin::DecompressTarga_RGB24(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount;)
	{
		unsigned_int8 d = *data++;
		int32 count = (d & 0x7F) + 1;

		if ((d & 0x80) != 0)
		{
			Color4C		c;

			c.Set(data[2], data[1], data[0], 255);
			do
			{
				image[a++] = c;
			} while (--count > 0);

			data += 3;
		}
		else
		{
			do
			{
				image[a++].Set(data[2], data[1], data[0], 255);
				data += 3;

			} while (--count > 0);
		}
	}
}

void TargaImageImportPlugin::DecompressTarga_RGBA32(const unsigned_int8 *data, Color4C *restrict image, int32 pixelCount)
{
	for (machine a = 0; a < pixelCount;)
	{
		unsigned_int8 d = *data++;
		int32 count = (d & 0x7F) + 1;

		if ((d & 0x80) != 0)
		{
			Color4C		c;

			c.Set(data[2], data[1], data[0], data[3]);
			do
			{
				image[a++] = c;
			} while (--count > 0);

			data += 4;
		}
		else
		{
			do
			{
				image[a++].Set(data[2], data[1], data[0], data[3]);
				data += 4;

			} while (--count > 0);
		}
	}
}

ImageFormat TargaImageImportPlugin::GetImageFormat(void) const
{
	return (kImageFormatRGBA);
}

EngineResult TargaImageImportPlugin::GetImageFileInfo(const char *name, Integer2D *size, unsigned_int32 *flags, int32 *count)
{
	File	file;

	EngineResult result = file.Open(name);
	if (result == kFileOkay)
	{
		TargaHeader		header;

		result = file.Read(&header, sizeof(TargaHeader));
		if (result == kFileOkay)
		{
			int32 pixelDepth = header.pixelDepth;
			int32 imageType = header.imageType;

			if ((header.colorMapType == 0) && ((pixelDepth == 8) || (pixelDepth == 16) || (pixelDepth == 24) || (pixelDepth == 32)) && ((imageType == 2) || (imageType == 3) || (imageType == 10) || (imageType == 11)))
			{
				int32 width = ReadLittleEndianS16(&header.width);
				int32 height = ReadLittleEndianS16(&header.height);
				size->Set(width, height);

				if (flags) *flags = (pixelDepth == 32) ? kImageImportAlpha : 0;
				if (count) *count = 1;
			}
			else
			{
				result = kPluginBadImageFormat;
			}
		}
	}

	return (result);
}

EngineResult TargaImageImportPlugin::ImportImageFile(const char *name, void **image, Integer2D *size, unsigned_int32 *flags, int32 index)
{
	File	file;

	EngineResult result = file.Open(name);
	if (result == kFileOkay)
	{
		unsigned_int64 dataSize = file.GetSize();
		char *storage = new char[dataSize];

		result = file.Read(storage, dataSize);
		if (result == kFileOkay)
		{
			const TargaHeader *header = reinterpret_cast<TargaHeader *>(storage);
			int32 pixelDepth = header->pixelDepth;
			int32 imageType = header->imageType;

			if ((header->colorMapType == 0) && ((pixelDepth == 8) || (pixelDepth == 16) || (pixelDepth == 24) || (pixelDepth == 32)) && ((imageType == 2) || (imageType == 3) || (imageType == 10) || (imageType == 11)))
			{
				int32 width = ReadLittleEndianS16(&header->width);
				int32 height = ReadLittleEndianS16(&header->height);
				size->Set(width, height);

				int32 pixelCount = width * height;
				Color4C *pixel = new Color4C[pixelCount];
				*image = pixel;

				if (flags) *flags = (pixelDepth == 32) ? kImageImportAlpha : 0;

				const unsigned_int8 *data = header->GetPixelData();
				if ((imageType & 8) == 0)
				{
					if (pixelDepth == 8) CopyTarga_L8(data, pixel, pixelCount);
					else if (pixelDepth == 16) CopyTarga_RGB16(data, pixel, pixelCount);
					else if (pixelDepth == 24) CopyTarga_RGB24(data, pixel, pixelCount);
					else CopyTarga_RGBA32(data, pixel, pixelCount);
				}
				else
				{
					if (pixelDepth == 8) DecompressTarga_L8(data, pixel, pixelCount);
					else if (pixelDepth == 16) DecompressTarga_RGB16(data, pixel, pixelCount);
					else if (pixelDepth == 24) DecompressTarga_RGB24(data, pixel, pixelCount);
					else DecompressTarga_RGBA32(data, pixel, pixelCount);
				}

				if ((header->imageDescriptor & 0x20) != 0)
				{
					int32 h = height >> 1;
					for (machine j = 0; j < h; j++)
					{
						Color4C *top = pixel + j * width;
						Color4C *bottom = pixel + (height - j - 1) * width;
						for (machine i = 0; i < width; i++)
						{
							Color4C c = top[i];
							top[i] = bottom[i];
							bottom[i] = c;
						}
					}
				}
			}
			else
			{
				result = kPluginBadImageFormat;
			}
		}

		delete[] storage;
	}

	return (result);
}

void TargaImageImportPlugin::ReleaseImageData(void *image)
{
	delete[] static_cast<Color4C *>(image);
}


TargaImageExportPlugin::TargaImageExportPlugin()
{
}

TargaImageExportPlugin::~TargaImageExportPlugin()
{
}

const char *TargaImageExportPlugin::GetImageTypeName(void) const
{
	return ("Targa");
}

const ResourceDescriptor *TargaImageExportPlugin::GetImageResourceDescriptor(void) const
{
	return (TargaResource::GetDescriptor());
}

ImagePlugin::ImageFileFilterProc *TargaImageExportPlugin::GetImageFileFilterProc(void) const
{
	return (&TargaFilter);
}

unsigned_int32 TargaImageExportPlugin::CompressTarga_BGRA(const Color4C *image, unsigned_int8 *restrict data, int32 pixelCount)
{
	unsigned_int32 compressedSize = 0;
	unsigned_int32 maxSize = pixelCount * sizeof(Color4C);

	for (machine a = 0; a < pixelCount;)
	{
		int32 limit = Min(pixelCount - a, 128);
		int32 count = CountConstantRGBAValues(image + a, limit);
		if (count > 1)
		{
			compressedSize += 5;
			if (compressedSize >= maxSize) return (0);

			*data++ = (unsigned_int8) ((count - 1) | 0x80);

			const Color4C& pixel = image[a];
			data[0] = (unsigned_int8) pixel.GetRed();
			data[1] = (unsigned_int8) pixel.GetGreen();
			data[2] = (unsigned_int8) pixel.GetBlue();
			data[3] = (unsigned_int8) pixel.GetAlpha();
			data += 4;
		}
		else
		{
			count = CountVaryingRGBAValues(image + a, limit);

			compressedSize += count * 4 + 1;
			if (compressedSize >= maxSize) return (0);

			*data++ = (unsigned_int8) (count - 1);

			for (machine b = 0; b < count; b++)
			{
				const Color4C& pixel = image[a + b];
				data[0] = (unsigned_int8) pixel.GetRed();
				data[1] = (unsigned_int8) pixel.GetGreen();
				data[2] = (unsigned_int8) pixel.GetBlue();
				data[3] = (unsigned_int8) pixel.GetAlpha();
				data += 4;
			}
		}

		a += count;
	}

	return (compressedSize);
}

unsigned_int32 TargaImageExportPlugin::CompressTarga_BGR(const Color4C *image, unsigned_int8 *restrict data, int32 pixelCount)
{
	unsigned_int32 compressedSize = 0;
	unsigned_int32 maxSize = pixelCount * 3;

	for (machine a = 0; a < pixelCount;)
	{
		int32 limit = Min(pixelCount - a, 128);
		int32 count = CountConstantRGBValues(image + a, limit);
		if (count > 1)
		{
			compressedSize += 4;
			if (compressedSize >= maxSize) return (0);

			*data++ = (unsigned_int8) ((count - 1) | 0x80);

			const Color4C& pixel = image[a];
			data[0] = (unsigned_int8) pixel.GetRed();
			data[1] = (unsigned_int8) pixel.GetGreen();
			data[2] = (unsigned_int8) pixel.GetBlue();
			data += 3;
		}
		else
		{
			count = CountVaryingRGBValues(image + a, limit);

			compressedSize += count * 3 + 1;
			if (compressedSize >= maxSize) return (0);

			*data++ = (unsigned_int8) (count - 1);

			for (machine b = 0; b < count; b++)
			{
				const Color4C& pixel = image[a + b];
				data[0] = (unsigned_int8) pixel.GetRed();
				data[1] = (unsigned_int8) pixel.GetGreen();
				data[2] = (unsigned_int8) pixel.GetBlue();
				data += 3;
			}
		}

		a += count;
	}

	return (compressedSize);
}

unsigned_int32 TargaImageExportPlugin::CompressTarga_RGBA(const Color4C *image, unsigned_int8 *restrict data, int32 pixelCount)
{
	unsigned_int32 compressedSize = 0;
	unsigned_int32 maxSize = pixelCount * sizeof(Color4C);

	for (machine a = 0; a < pixelCount;)
	{
		int32 limit = Min(pixelCount - a, 128);
		int32 count = CountConstantRGBAValues(image + a, limit);
		if (count > 1)
		{
			compressedSize += 5;
			if (compressedSize >= maxSize) return (0);

			*data++ = (unsigned_int8) ((count - 1) | 0x80);

			const Color4C& pixel = image[a];
			data[0] = (unsigned_int8) pixel.GetBlue();
			data[1] = (unsigned_int8) pixel.GetGreen();
			data[2] = (unsigned_int8) pixel.GetRed();
			data[3] = (unsigned_int8) pixel.GetAlpha();
			data += 4;
		}
		else
		{
			count = CountVaryingRGBAValues(image + a, limit);

			compressedSize += count * 4 + 1;
			if (compressedSize >= maxSize) return (0);

			*data++ = (unsigned_int8) (count - 1);

			for (machine b = 0; b < count; b++)
			{
				const Color4C& pixel = image[a + b];
				data[0] = (unsigned_int8) pixel.GetBlue();
				data[1] = (unsigned_int8) pixel.GetGreen();
				data[2] = (unsigned_int8) pixel.GetRed();
				data[3] = (unsigned_int8) pixel.GetAlpha();
				data += 4;
			}
		}

		a += count;
	}

	return (compressedSize);
}

unsigned_int32 TargaImageExportPlugin::CompressTarga_RGB(const Color4C *image, unsigned_int8 *restrict data, int32 pixelCount)
{
	unsigned_int32 compressedSize = 0;
	unsigned_int32 maxSize = pixelCount * 3;

	for (machine a = 0; a < pixelCount;)
	{
		int32 limit = Min(pixelCount - a, 128);
		int32 count = CountConstantRGBValues(image + a, limit);
		if (count > 1)
		{
			compressedSize += 4;
			if (compressedSize >= maxSize) return (0);

			*data++ = (unsigned_int8) ((count - 1) | 0x80);

			const Color4C& pixel = image[a];
			data[0] = (unsigned_int8) pixel.GetBlue();
			data[1] = (unsigned_int8) pixel.GetGreen();
			data[2] = (unsigned_int8) pixel.GetRed();
			data += 3;
		}
		else
		{
			count = CountVaryingRGBValues(image + a, limit);

			compressedSize += count * 3 + 1;
			if (compressedSize >= maxSize) return (0);

			*data++ = (unsigned_int8) (count - 1);

			for (machine b = 0; b < count; b++)
			{
				const Color4C& pixel = image[a + b];
				data[0] = (unsigned_int8) pixel.GetBlue();
				data[1] = (unsigned_int8) pixel.GetGreen();
				data[2] = (unsigned_int8) pixel.GetRed();
				data += 3;
			}
		}

		a += count;
	}

	return (compressedSize);
}

EngineResult TargaImageExportPlugin::ExportImageFile(const char *name, const Color4C *image, const Integer2D& size, unsigned_int32 flags)
{
	File	file;

	EngineResult result = file.Open(name, kFileCreate);
	if (result == kFileOkay)
	{
		TargaHeader		header;
		unsigned_int32	compressedSize;

		header.idLength = 0;
		header.colorMapType = 0;
		header.colorMapStartL = 0;
		header.colorMapStartH = 0;
		header.colorMapLengthL = 0;
		header.colorMapLengthH = 0;
		header.colorMapDepth = 0;
		header.xOffset = 0;
		header.yOffset = 0;
		header.width = (int16) size.x;
		header.height = (int16) size.y;
		header.imageDescriptor = 8;

		#if C4BIGENDIAN

			Reverse(&header.width);
			Reverse(&header.height);

		#endif

		int32 pixelCount = size.x * size.y;
		unsigned_int8 *compressedData = nullptr;
		
		if ((flags & kImageExportUncompressed) == 0) compressedData = new unsigned_int8[pixelCount * sizeof(Color4C)];

		if (flags & kImageExportAlpha)
		{
			header.pixelDepth = 32;

			if (((flags & kImageExportUncompressed) == 0) && ((compressedSize = ((flags & kImageExportBGRA) ? CompressTarga_BGRA : CompressTarga_RGBA)(image, compressedData, pixelCount)) != 0))
			{
				header.imageType = 10;
				file.Write(&header, sizeof(TargaHeader));
				result = file.Write(compressedData, compressedSize);
			}
			else
			{
				header.imageType = 2;
				file.Write(&header, sizeof(TargaHeader));

				if (flags & kImageExportBGRA)
				{
					result = file.Write(image, pixelCount * sizeof(Color4C));
				}
				else
				{
					const Color4C *input = image;
					unsigned_int8 *output = compressedData;
					for (machine a = 0; a < pixelCount; a++)
					{
						output[0] = (unsigned_int8) input->GetBlue();
						output[1] = (unsigned_int8) input->GetGreen();
						output[2] = (unsigned_int8) input->GetRed();
						output[3] = (unsigned_int8) input->GetAlpha();

						input++;
						output += 4;
					}

					result = file.Write(compressedData, pixelCount * sizeof(Color4C));
				}
			}
		}
		else
		{
			header.pixelDepth = 24;

			if (((flags & kImageExportUncompressed) == 0) && ((compressedSize = ((flags & kImageExportBGRA) ? CompressTarga_BGR : CompressTarga_RGB)(image, compressedData, pixelCount)) != 0))
			{
				header.imageType = 10;
				file.Write(&header, sizeof(TargaHeader));
				result = file.Write(compressedData, compressedSize);
			}
			else
			{
				header.imageType = 2;
				file.Write(&header, sizeof(TargaHeader));

				const Color4C *input = image;
				unsigned_int8 *output = compressedData;

				if (flags & kImageExportBGRA)
				{
					for (machine a = 0; a < pixelCount; a++)
					{
						output[0] = (unsigned_int8) input->GetRed();
						output[1] = (unsigned_int8) input->GetGreen();
						output[2] = (unsigned_int8) input->GetBlue();

						input++;
						output += 3;
					}
				}
				else
				{
					for (machine a = 0; a < pixelCount; a++)
					{
						output[0] = (unsigned_int8) input->GetBlue();
						output[1] = (unsigned_int8) input->GetGreen();
						output[2] = (unsigned_int8) input->GetRed();

						input++;
						output += 3;
					}
				}

				result = file.Write(compressedData, pixelCount * 3);
			}
		}

		delete[] compressedData;
	}

	return (result);
}


namespace
{
	bool SequenceFilter(const char *name)
	{
		int32 len = Text::GetTextLength(name);
		return (Text::CompareTextCaseless(&name[MaxZero(len - 4)], ".seq"));
	}

	void ReverseHeader(SequenceHeader *header)
	{
		Reverse(&header->headerSize);
		Reverse(&header->imageWidth);
		Reverse(&header->imageHeight);
	}
}


ResourceDescriptor SequenceResource::descriptor("seq");


SequenceResource::SequenceResource(const char *name, ResourceCatalog *catalog) : Resource<SequenceResource>(name, catalog)
{
}

SequenceResource::~SequenceResource()
{
}

void SequenceResource::Preprocess(void)
{
	SequenceHeader *header = static_cast<SequenceHeader *>(GetData());
	if (header->endian != 1) ReverseHeader(header);
}


SequenceImageImportPlugin::SequenceImageImportPlugin()
{
}

SequenceImageImportPlugin::~SequenceImageImportPlugin()
{
}

const char *SequenceImageImportPlugin::GetImageTypeName(void) const
{
	return ("Sequence");
}

const ResourceDescriptor *SequenceImageImportPlugin::GetImageResourceDescriptor(void) const
{
	return (SequenceResource::GetDescriptor());
}

ImageImportPlugin::ImageFileFilterProc *SequenceImageImportPlugin::GetImageFileFilterProc(void) const
{
	return (&SequenceFilter);
}

ImageFormat SequenceImageImportPlugin::GetImageFormat(void) const
{
	return (kImageFormatYCbCr);
}

EngineResult SequenceImageImportPlugin::GetImageFileInfo(const char *name, Integer2D *size, unsigned_int32 *flags, int32 *count)
{
	File	file;

	EngineResult result = file.Open(name);
	if (result == kFileOkay)
	{
		SequenceHeader		header;

		result = file.Read(&header, sizeof(SequenceHeader));
		if (result == kFileOkay)
		{
			if (header.endian != 1) ReverseHeader(&header);

			int32 width = header.imageWidth;
			int32 height = header.imageHeight;
			size->Set(width, height);

			if (flags) *flags = 0;
			if (count)
			{
				int32 pixelCount = width * height;
				unsigned_int32 imageSize = pixelCount + (pixelCount >> 1);
				*count = (file.GetSize() - header.headerSize) / imageSize;
			}
		}
	}

	return (result);
}

EngineResult SequenceImageImportPlugin::ImportImageFile(const char *name, void **image, Integer2D *size, unsigned_int32 *flags, int32 index)
{
	File	file;

	EngineResult result = file.Open(name);
	if (result == kFileOkay)
	{
		SequenceHeader		header;

		result = file.Read(&header, sizeof(SequenceHeader));
		if (result == kFileOkay)
		{
			if (header.endian != 1) ReverseHeader(&header);

			int32 width = header.imageWidth;
			int32 height = header.imageHeight;
			size->Set(width, height);

			if (flags) *flags = 0;

			int32 pixelCount = width * height;
			unsigned_int32 imageSize = pixelCount + (pixelCount >> 1);

			int32 imageCount = (file.GetSize() - header.headerSize) / imageSize;
			if ((index >= 0) && (index < imageCount))
			{
				int8 *pixel = new int8[imageSize];
				*image = pixel;

				file.SetPosition(int64(header.headerSize) + int64(imageSize) * index);
				result = file.Read(pixel, imageSize);
			}
			else
			{
				result = kPluginBadImageIndex;
			}
		}
	}

	return (result);
}

void SequenceImageImportPlugin::ReleaseImageData(void *image)
{
	delete[] static_cast<int8 *>(image);
}

// ZYURVUR
