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


#include "C4Movies.h"
#include "C4World.h"


using namespace C4;


namespace
{
	enum
	{
		kMovieCompressionNone			= 0,
		kMovieCompressionGeneral		= 1
	};


	enum
	{
		kAudioIndependent				= 0,
		kAudioLeftDifference			= 1,
		kAudioRightDifference			= 2,
		kAudioAverageDifference			= 3,
		kAudioChannelDependencyCount	= 4
	};


	enum
	{
		kRecordStageReady				= 0,
		kRecordStageTransferring		= 1,
		kRecordStageReading				= 2,
		kRecordStageCompleted			= 3
	};


	enum
	{
		kMaxRecordFileSize				= 0x60000000
	};


	const float alignas(128) blockTransformMatrix[8][8] =
	{
		{0.353553F,   0.490393F,   0.461940F,   0.415735F,   0.353553F,   0.277785F,   0.191342F,   0.0975452F},
		{0.353553F,   0.415735F,   0.191342F,  -0.0975452F, -0.353553F,	 -0.490393F,  -0.461940F,  -0.277785F},
		{0.353553F,   0.277785F,  -0.191342F,  -0.490393F,  -0.353553F,   0.0975452F,  0.461940F,   0.415735F},
		{0.353553F,   0.0975452F, -0.461940F,  -0.277785F,   0.353553F,   0.415735F,  -0.191342F,  -0.490393F},
		{0.353553F,  -0.0975452F, -0.461940F,   0.277785F,   0.353553F,  -0.415735F,  -0.191342F,   0.490393F},
		{0.353553F,  -0.277785F,  -0.191342F,   0.490393F,  -0.353553F,  -0.0975452F,  0.461940F,  -0.415735F},
		{0.353553F,  -0.415735F,   0.191342F,   0.0975452F, -0.353553F,   0.490393F,  -0.461940F,   0.277785F},
		{0.353553F,  -0.490393F,   0.461940F,  -0.415735F,   0.353553F,  -0.277785F,   0.191342F,  -0.0975452F}
	};

	const float alignas(128) blockInverseTransformMatrix[8][8] =
	{
		{0.353553F,   0.353553F,   0.353553F,   0.353553F,   0.353553F,   0.353553F,   0.353553F,   0.353553F},
		{0.490393F,   0.415735F,   0.277785F,   0.0975452F, -0.0975452F, -0.277785F,  -0.415735F,  -0.490393F},
		{0.461940F,   0.191342F,  -0.191342F,  -0.461940F,  -0.461940F,  -0.191342F,   0.191342F,   0.461940F},
		{0.415735F,  -0.0975452F, -0.490393F,  -0.277785F,   0.277785F,   0.490393F,   0.0975452F, -0.415735F},
		{0.353553F,  -0.353553F,  -0.353553F,   0.353553F,   0.353553F,  -0.353553F,  -0.353553F,   0.353553F},
		{0.277785F,  -0.490393F,   0.0975452F,  0.415735F,  -0.415735F,  -0.0975452F,  0.490393F,  -0.277785F},
		{0.191342F,  -0.461940F,   0.461940F,  -0.191342F,  -0.191342F,   0.461940F,  -0.461940F,   0.191342F},
		{0.0975452F, -0.277785F,   0.415735F,  -0.490393F,   0.490393F,  -0.415735F,   0.277785F,  -0.0975452F}
	};

	const float alignas(128) luminanceQuantizeMatrix[8][8] =
	{
		{16.0F,  11.0F,  10.0F,  16.0F,  24.0F,  40.0F,  51.0F,  61.0F},
		{12.0F,  12.0F,  14.0F,  19.0F,  26.0F,  58.0F,  60.0F,  55.0F},
		{14.0F,  13.0F,  16.0F,  24.0F,  40.0F,  57.0F,  69.0F,  56.0F},
		{14.0F,  17.0F,  22.0F,  29.0F,  51.0F,  87.0F,  80.0F,  62.0F},
		{18.0F,  22.0F,  37.0F,  56.0F,  68.0F, 109.0F, 103.0F,  77.0F},
		{24.0F,  35.0F,  55.0F,  64.0F,  81.0F, 104.0F, 113.0F,  92.0F},
		{49.0F,  64.0F,  78.0F,  87.0F, 103.0F, 121.0F, 120.0F, 101.0F},
		{72.0F,  92.0F,  95.0F,  98.0F, 112.0F, 100.0F, 103.0F,  99.0F}
	};

	const float alignas(128) chrominanceQuantizeMatrix[8][8] =
	{
		{17.0F,  18.0F,  24.0F,  47.0F,  99.0F,  99.0F,  99.0F,  99.0F},
		{18.0F,  21.0F,  26.0F,  66.0F,  99.0F,  99.0F,  99.0F,  99.0F},
		{24.0F,  26.0F,  56.0F,  99.0F,  99.0F,  99.0F,  99.0F,  99.0F},
		{47.0F,  66.0F,  99.0F,  99.0F,  99.0F,  99.0F,  99.0F,  99.0F},
		{99.0F,  99.0F,  99.0F,  99.0F,  99.0F,  99.0F,  99.0F,  99.0F},
		{99.0F,  99.0F,  99.0F,  99.0F,  99.0F,  99.0F,  99.0F,  99.0F},
		{99.0F,  99.0F,  99.0F,  99.0F,  99.0F,  99.0F,  99.0F,  99.0F},
		{99.0F,  99.0F,  99.0F,  99.0F,  99.0F,  99.0F,  99.0F,  99.0F}
	};

	const unsigned_int8 alignas(64) blockLinearizeTable[64] =
	{
		0x00, 0x08, 0x01, 0x10, 0x09, 0x02, 0x18, 0x11,
		0x0A, 0x03, 0x20, 0x19, 0x12, 0x0B, 0x04, 0x28,
		0x21, 0x1A, 0x13, 0x0C, 0x05, 0x30, 0x29, 0x22,
		0x1B, 0x14, 0x0D, 0x06, 0x38, 0x31, 0x2A, 0x23,
		0x1C, 0x15, 0x0E, 0x07, 0x39, 0x32, 0x2B, 0x24,
		0x1D, 0x16, 0x0F, 0x3A, 0x33, 0x2C, 0x25, 0x1E,
		0x17, 0x3B, 0x34, 0x2D, 0x26, 0x1F, 0x3C, 0x35,
		0x2E, 0x27, 0x3D, 0x36, 0x2F, 0x3E, 0x37, 0x3F
	};

 
	struct AudioLeftDifferenceCombiner
	{ 
		static void Output(Sample sample, Sample left, Sample *restrict audio) 
		{ 
			WriteLittleEndianS16(&audio[0], left);
			WriteLittleEndianS16(&audio[1], (Sample) (left + sample)); 
		}
	};

	struct AudioRightDifferenceCombiner 
	{
		static void Output(Sample sample, Sample right, Sample *restrict audio)
		{
			WriteLittleEndianS16(&audio[0], (Sample) (right - sample)); 
			WriteLittleEndianS16(&audio[1], right);
		}
	};

	struct AudioAverageDifferenceCombiner
	{
		static void Output(Sample sample, Sample average, Sample *restrict audio)
		{
			WriteLittleEndianS16(&audio[0], (Sample) (average - (sample >> 1)));
			WriteLittleEndianS16(&audio[1], (Sample) (average + ((sample + 1) >> 1)));
		}
	};


	void ReverseData(MovieVideoChannelData *data)
	{
		Reverse(&data->videoChannelFlags);
		Reverse(&data->quantizationScale);
		Reverse(&data->channelBoundingRect);

		Reverse(&data->flatCodeOffset);
		Reverse(&data->flatCodeSize);
		Reverse(&data->flatDataSize);

		Reverse(&data->wlenCodeOffset);
		Reverse(&data->wlenCodeSize);
		Reverse(&data->wlenDataSize);

		Reverse(&data->wavyCodeOffset);
		Reverse(&data->wavyCodeSize);
		Reverse(&data->wavyDataSize);
	}

	void ReverseHeader(MovieVideoFrameHeader *header)
	{
		ReverseData(&header->luminanceData);
		ReverseData(&header->chrominanceData);
	}

	void ReverseHeader(MovieTrackHeader *header)
	{
		Reverse(&header->movieTrackType);
		Reverse(&header->movieTrackOffset);

		MovieTrackType type = header->movieTrackType;
		if (type == kMovieTrackVideo)
		{
			MovieVideoTrackHeader *trackHeader = const_cast<MovieVideoTrackHeader *>(header->GetVideoTrackHeader());

			Reverse(&trackHeader->videoTrackFlags);
			Reverse(&trackHeader->videoFrameCount);

			Reverse(&trackHeader->videoFrameSize);
			Reverse(&trackHeader->videoFrameTime);
			Reverse(&trackHeader->posterFrameIndex);

			Reverse(&trackHeader->maxFrameCodeSize);
			Reverse(&trackHeader->maxFrameDataSize);

			int32 count = trackHeader->videoFrameCount;
			MovieVideoFrameData *data = trackHeader->videoFrameData;
			for (machine a = 0; a < count; a++)
			{
				Reverse(&data->videoFrameOffset);
				Reverse(&data->videoFrameSize);
				Reverse(&data->videoBaseFrameIndex);
				data++;
			}
		}
		else if (type == kMovieTrackAudio)
		{
			MovieAudioTrackHeader *trackHeader = const_cast<MovieAudioTrackHeader *>(header->GetAudioTrackHeader());

			Reverse(&trackHeader->audioTrackFlags);
			Reverse(&trackHeader->audioBlockCount);
			Reverse(&trackHeader->blockFrameCount);

			Reverse(&trackHeader->audioFrameCount);
			Reverse(&trackHeader->audioChannelCount);
			Reverse(&trackHeader->audioSampleDepth);
			Reverse(&trackHeader->audioSampleFrequency);

			Reverse(&trackHeader->maxBlockCodeSize);
			Reverse(&trackHeader->maxBlockDataSize);

			int32 count = trackHeader->audioBlockCount;
			MovieAudioBlockData *data = trackHeader->audioBlockData;
			for (machine a = 0; a < count; a++)
			{
				Reverse(&data->audioBlockOffset);
				Reverse(&data->audioBlockSize);
				data++;
			}
		}
	}

	void ReverseData(MovieAudioChannelData *data)
	{
		Reverse(&data->audioChannelFlags);
		Reverse(&data->audioCodeOffset);
		Reverse(&data->audioCodeSize);
		Reverse(&data->audioDataSize);
	}

	void ReverseHeader(MovieAudioBlockHeader *header, int32 channelCount)
	{
		Reverse(&header->audioBlockFlags);
		for (machine a = 0; a < channelCount; a++) ReverseData(&header->audioChannelData[a]);
	}
}


MovieMgr *C4::TheMovieMgr = nullptr;


namespace C4
{
	template <> MovieMgr Manager<MovieMgr>::managerObject(0);
	template <> MovieMgr **Manager<MovieMgr>::managerPointer = &TheMovieMgr;

	template <> const char *const Manager<MovieMgr>::resultString[] =
	{
		nullptr,
		"Movie load failed",
		"Movie video track missing",
		"Bad movie image dimensions",
		"Inconsistent movie image dimensions",
		"Bad movie audio format",
		"Movie import cancelled"
	};

	template <> const unsigned_int32 Manager<MovieMgr>::resultIdentifier[] =
	{
		0, 'LOAD', 'VDEO', 'ISIZ', 'ICON', 'ADIO', 'CANC'
	};

	template class Manager<MovieMgr>;
}


ResourceDescriptor MovieResource::descriptor("mvi");


MovieResource::MovieResource(const char *name, ResourceCatalog *catalog) : Resource<MovieResource>(name, catalog)
{
}

MovieResource::~MovieResource()
{
}

void MovieResource::Preprocess(void)
{
	MovieResourceHeader *resourceHeader = static_cast<MovieResourceHeader *>(GetData());
	if (resourceHeader->endian != 1)
	{
		Reverse(&resourceHeader->headerDataSize);
		Reverse(&resourceHeader->movieTrackCount);

		int32 count = resourceHeader->movieTrackCount;
		MovieTrackHeader *movieTrackHeader = reinterpret_cast<MovieTrackHeader *>(resourceHeader + 1);
		for (machine a = 0; a < count; a++) ReverseHeader(&movieTrackHeader[a]);
	}
}

ResourceResult MovieResource::LoadHeaderData(ResourceLoader *loader, MovieResourceHeader *resourceHeader, MovieTrackHeader **movieTrackHeader) const
{
	ResourceResult result = loader->Read(resourceHeader, 0, sizeof(MovieResourceHeader));
	if (result != kResourceOkay) return (result);

	int32 endian = resourceHeader->endian;
	if (endian != 1)
	{
		Reverse(&resourceHeader->headerDataSize);
		Reverse(&resourceHeader->movieTrackCount);
	}

	unsigned_int32 size = resourceHeader->headerDataSize;
	char *storage = new char[size];

	result = loader->Read(storage, sizeof(MovieResourceHeader), size);
	if (result != kResourceOkay)
	{
		delete[] storage;
		return (result);
	}

	MovieTrackHeader *header = reinterpret_cast<MovieTrackHeader *>(storage);
	if (endian != 1)
	{
		int32 count = resourceHeader->movieTrackCount;
		for (machine a = 0; a < count; a++) ReverseHeader(&header[a]);
	}

	*movieTrackHeader = header;
	return (kResourceOkay);
}

ResourceResult MovieResource::LoadVideoFrameData(ResourceLoader *loader, MovieResourceHeader *resourceHeader, const MovieVideoTrackHeader *videoTrackHeader, int32 frameIndex, MovieVideoFrameHeader *videoFrameHeader) const
{
	const MovieVideoFrameData *frameData = &videoTrackHeader->videoFrameData[frameIndex];
	ResourceResult result = loader->Read(videoFrameHeader, frameData->videoFrameOffset, frameData->videoFrameSize);
	if (result != kResourceOkay) return (result);

	if (resourceHeader->endian != 1)
	{
		ReverseHeader(videoFrameHeader);
		if (videoTrackHeader->videoTrackFlags & kMovieVideoAlphaChannel) ReverseData(&static_cast<MovieVideoAlphaFrameHeader *>(videoFrameHeader)->alphaData);
	}
	
	return (kResourceOkay);
}

ResourceResult MovieResource::LoadAudioBlockData(ResourceLoader *loader, MovieResourceHeader *resourceHeader, const MovieAudioTrackHeader *audioTrackHeader, int32 blockIndex, MovieAudioBlockHeader *audioBlockHeader) const
{
	const MovieAudioBlockData *blockData = &audioTrackHeader->audioBlockData[blockIndex];
	ResourceResult result = loader->Read(audioBlockHeader, blockData->audioBlockOffset, blockData->audioBlockSize);
	if (result != kResourceOkay) return (result);

	if (resourceHeader->endian != 1) ReverseHeader(audioBlockHeader, audioTrackHeader->audioChannelCount);
	return (kResourceOkay);
}

void MovieResource::ReleaseHeaderData(MovieTrackHeader *movieTrackHeader)
{
	delete[] reinterpret_cast<char *>(movieTrackHeader);
}


VideoCompressor::VideoCompressor(const MovieVideoTrackHeader *videoTrackHeader)
{
	videoTrackFlags = videoTrackHeader->videoTrackFlags;
	videoFrameSize = videoTrackHeader->videoFrameSize;

	baseImageIndex = 0;

	unsigned_int32 pixelCount = videoFrameSize.x * videoFrameSize.y;
	unsigned_int32 blockDataSize = (videoFrameSize.x >> 3) * 130;

	unsigned_int32 flatLuminCodeSize = ((pixelCount >> 5) + 15) & ~15;
	unsigned_int32 wlenLuminCodeSize = ((pixelCount >> 6) + 15) & ~15;
	unsigned_int32 wavyLuminCodeSize = pixelCount * 2;
	unsigned_int32 luminCodeSize = flatLuminCodeSize + wlenLuminCodeSize + wavyLuminCodeSize;

	unsigned_int32 flatChromCodeSize = wlenLuminCodeSize;
	unsigned_int32 wlenChromCodeSize = ((pixelCount >> 7) + 15) & ~15;
	unsigned_int32 wavyChromCodeSize = pixelCount;
	unsigned_int32 chromCodeSize = flatChromCodeSize + wlenChromCodeSize + wavyChromCodeSize;

	unsigned_int32 colorImageSize = pixelCount + (pixelCount >> 1);
	unsigned_int32 totalImageSize = colorImageSize;

	unsigned_int32 colorCodeSize = luminCodeSize + chromCodeSize;
	unsigned_int32 totalCodeSize = colorCodeSize;

	if (videoTrackFlags & kMovieVideoAlphaChannel)
	{
		totalImageSize += pixelCount;
		totalCodeSize += luminCodeSize;
	}

	compressorStorage = new char[totalImageSize * 3 + totalCodeSize * 3 + blockDataSize];

	luminanceImage[0] = reinterpret_cast<int8 *>(compressorStorage);
	chrominanceImage[0] = reinterpret_cast<int8 *>(compressorStorage + pixelCount);
	alphaImage[0] = reinterpret_cast<int8 *>(compressorStorage + colorImageSize);
	luminanceImage[1] = reinterpret_cast<int8 *>(compressorStorage + totalImageSize);
	chrominanceImage[1] = reinterpret_cast<int8 *>(compressorStorage + (totalImageSize + pixelCount));
	alphaImage[1] = reinterpret_cast<int8 *>(compressorStorage + (totalImageSize + colorImageSize));
	luminanceImage[2] = reinterpret_cast<int8 *>(compressorStorage + totalImageSize * 2);
	chrominanceImage[2] = reinterpret_cast<int8 *>(compressorStorage + (totalImageSize * 2 + pixelCount));
	alphaImage[2] = reinterpret_cast<int8 *>(compressorStorage + (totalImageSize * 2 + colorImageSize));

	totalImageSize *= 3;

	flatLuminCode = reinterpret_cast<unsigned_int8 *>(compressorStorage + totalImageSize);
	wlenLuminCode = reinterpret_cast<unsigned_int8 *>(compressorStorage + (totalImageSize + flatLuminCodeSize));
	wavyLuminCode = reinterpret_cast<unsigned_int8 *>(compressorStorage + (totalImageSize + flatLuminCodeSize + wlenLuminCodeSize));

	flatChromCode = reinterpret_cast<unsigned_int8 *>(compressorStorage + (totalImageSize + luminCodeSize));
	wlenChromCode = reinterpret_cast<unsigned_int8 *>(compressorStorage + (totalImageSize + luminCodeSize + flatChromCodeSize));
	wavyChromCode = reinterpret_cast<unsigned_int8 *>(compressorStorage + (totalImageSize + luminCodeSize + flatChromCodeSize + wlenChromCodeSize));

	flatAlphaCode = reinterpret_cast<unsigned_int8 *>(compressorStorage + (totalImageSize + colorCodeSize));
	wlenAlphaCode = reinterpret_cast<unsigned_int8 *>(compressorStorage + (totalImageSize + colorCodeSize + flatLuminCodeSize));
	wavyAlphaCode = reinterpret_cast<unsigned_int8 *>(compressorStorage + (totalImageSize + colorCodeSize + flatLuminCodeSize + wlenLuminCodeSize));

	compressedCode[0] = reinterpret_cast<unsigned_int8 *>(compressorStorage + (totalImageSize + totalCodeSize));
	compressedCode[1] = reinterpret_cast<unsigned_int8 *>(compressorStorage + (totalImageSize + totalCodeSize * 2));

	blockData = reinterpret_cast<int16 *>(compressorStorage + (totalImageSize + totalCodeSize * 3));
}

VideoCompressor::~VideoCompressor()
{
	delete[] compressorStorage;
}

void VideoCompressor::FindLuminanceBounds(const int8 *image, const Integer2D& frameSize, Rect *bounds)
{
	int32 width = frameSize.x;
	int32 height = frameSize.y;
	int32 top = height;

	const int8 *row = image;
	for (machine j = 0; j < height; j++)
	{
		unsigned_int32 data = 0;
		for (machine i = 0; i < width; i++) data |= row[i] + 128;

		if (data != 0)
		{
			top = j;
			break;
		}

		row += width;
	}

	if (top < height)
	{
		int32 bottom = height;

		row = image + (height - 1) * width;
		for (machine j = height; j > 0; j--)
		{
			unsigned_int32 data = 0;
			for (machine i = width - 1; i >= 0; i--) data |= row[i] + 128;

			if (data != 0)
			{
				bottom = j;
				break;
			}

			row -= width;
		}

		int32 left = width;
		int32 right = 0;

		row = image + top * width;
		for (machine j = top; j < bottom; j++)
		{
			for (machine i = 0; i < left; i++)
			{
				if (row[i] != -128)
				{
					left = i;
					break;
				}
			}

			for (machine i = width - 1; i >= right; i--)
			{
				if (row[i] != -128)
				{
					right = i + 1;
					break;
				}
			}

			row += width;
		}

		bounds->Set(left & ~7, top & ~7, (right + 7) & ~7, (bottom + 7) & ~7);
	}
	else
	{
		bounds->Set(0, height, width, height);
	}
}

void VideoCompressor::FindChrominanceBounds(const int8 *image, const Integer2D& frameSize, Rect *bounds)
{
	int32 width = frameSize.x >> 2;
	int32 height = frameSize.y >> 1;
	int32 top = height;

	const unsigned_int32 *row = reinterpret_cast<const unsigned_int32 *>(image);
	for (machine j = 0; j < height; j++)
	{
		unsigned_int32 data = 0;
		for (machine i = 0; i < width; i++) data |= row[i];

		if (data != 0)
		{
			top = j;
			break;
		}

		row += width;
	}

	if (top < height)
	{
		int32 bottom = height;

		row = reinterpret_cast<const unsigned_int32 *>(image) + (height - 1) * width;
		for (machine j = height; j > 0; j--)
		{
			unsigned_int32 data = 0;
			for (machine i = width - 1; i >= 0; i--) data |= row[i];

			if (data != 0)
			{
				bottom = j;
				break;
			}

			row -= width;
		}

		int32 left = width;
		int32 right = 0;

		row = reinterpret_cast<const unsigned_int32 *>(image) + top * width;
		for (machine j = top; j < bottom; j++)
		{
			for (machine i = 0; i < left; i++)
			{
				if (row[i] != 0)
				{
					left = i;
					break;
				}
			}

			for (machine i = width - 1; i >= right; i--)
			{
				if (row[i] != 0)
				{
					right = i + 1;
					break;
				}
			}

			row += width;
		}

		bounds->Set((left << 1) & ~7, top & ~7, ((right << 1) + 7) & ~7, (bottom + 7) & ~7);
	}
	else
	{
		bounds->Set(0, height, width << 1, height);
	}
}

void VideoCompressor::FindDeltaLuminanceBounds(const int8 *image, const int8 *baseImage, const Integer2D& frameSize, Rect *bounds)
{
	// This function ignores differences in the least significant bit.

	int32 width = frameSize.x;
	int32 height = frameSize.y;
	int32 top = height;

	const int8 *row = image;
	const int8 *baseRow = baseImage;

	for (machine j = 0; j < height; j++)
	{
		unsigned_int32 data = 0;
		for (machine i = 0; i < width; i++) data |= row[i] ^ baseRow[i];

		if ((data & ~1) != 0)
		{
			top = j;
			break;
		}

		row += width;
		baseRow += width;
	}

	if (top < height)
	{
		int32 bottom = height;

		int32 offset = (height - 1) * width;
		row = image + offset;
		baseRow = baseImage + offset;

		for (machine j = height; j > 0; j--)
		{
			unsigned_int32 data = 0;
			for (machine i = width - 1; i >= 0; i--) data |= row[i] ^ baseRow[i];

			if ((data & ~1) != 0)
			{
				bottom = j;
				break;
			}

			row -= width;
			baseRow -= width;
		}

		int32 left = width;
		int32 right = 0;

		offset = top * width;
		row = image + offset;
		baseRow = baseImage + offset;

		for (machine j = top; j < bottom; j++)
		{
			for (machine i = 0; i < left; i++)
			{
				if (((row[i] ^ baseRow[i]) & ~1) != 0)
				{
					left = i;
					break;
				}
			}

			for (machine i = width - 1; i >= right; i--)
			{
				if (((row[i] ^ baseRow[i]) & ~1) != 0)
				{
					right = i + 1;
					break;
				}
			}

			row += width;
			baseRow += width;
		}

		bounds->Set(left & ~7, top & ~7, (right + 7) & ~7, (bottom + 7) & ~7);
	}
	else
	{
		bounds->Set(0, height, width, height);
	}
}

void VideoCompressor::FindDeltaChrominanceBounds(const int8 *image, const int8 *baseImage, const Integer2D& frameSize, Rect *bounds)
{
	// This function ignores differences in the least significant bit.

	int32 width = frameSize.x >> 2;
	int32 height = frameSize.y >> 1;
	int32 top = height;

	const unsigned_int32 *row = reinterpret_cast<const unsigned_int32 *>(image);
	const unsigned_int32 *baseRow = reinterpret_cast<const unsigned_int32 *>(baseImage);

	for (machine j = 0; j < height; j++)
	{
		unsigned_int32 data = 0;
		for (machine i = 0; i < width; i++) data |= row[i] ^ baseRow[i];

		if ((data & ~1) != 0)
		{
			top = j;
			break;
		}

		row += width;
		baseRow += width;
	}

	if (top < height)
	{
		int32 bottom = height;

		int32 offset = (height - 1) * width;
		row = reinterpret_cast<const unsigned_int32 *>(image) + offset;
		baseRow = reinterpret_cast<const unsigned_int32 *>(baseImage) + offset;

		for (machine j = height; j > 0; j--)
		{
			unsigned_int32 data = 0;
			for (machine i = width - 1; i >= 0; i--) data |= row[i] ^ baseRow[i];

			if ((data & ~1) != 0)
			{
				bottom = j;
				break;
			}

			row -= width;
			baseRow -= width;
		}

		int32 left = width;
		int32 right = 0;

		offset = top * width;
		row = reinterpret_cast<const unsigned_int32 *>(image) + offset;
		baseRow = reinterpret_cast<const unsigned_int32 *>(baseImage) + offset;

		for (machine j = top; j < bottom; j++)
		{
			for (machine i = 0; i < left; i++)
			{
				if (((row[i] ^ baseRow[i]) & ~1) != 0)
				{
					left = i;
					break;
				}
			}

			for (machine i = width - 1; i >= right; i--)
			{
				if (((row[i] ^ baseRow[i]) & ~1) != 0)
				{
					right = i + 1;
					break;
				}
			}

			row += width;
			baseRow += width;
		}

		bounds->Set((left << 1) & ~7, top & ~7, ((right << 1) + 7) & ~7, (bottom + 7) & ~7);
	}
	else
	{
		bounds->Set(0, height, width << 1, height);
	}
}

void VideoCompressor::LoadLuminanceBlock(const int8 *input, float (*restrict output)[8], int32 rowLength)
{
	#if C4SIMD

		for (machine j = 0; j < 8; j++)
		{
			vec_int16 lum = SimdInt8UnpackA(SimdInt8LoadUnaligned(input));

			reinterpret_cast<vec_float *>(output[j])[0] = SimdInt32ConvertFloat(SimdInt16UnpackA(lum));
			reinterpret_cast<vec_float *>(output[j])[1] = SimdInt32ConvertFloat(SimdInt16UnpackB(lum));

			input += rowLength;
		}

	#else

		for (machine j = 0; j < 8; j++)
		{
			for (machine i = 0; i < 8; i++) output[j][i] = (float) input[i];

			input += rowLength;
		}

	#endif
}

void VideoCompressor::LoadDeltaLuminanceBlock(const int8 *input, const int8 *base, float (*restrict output)[8], int32 rowLength)
{
	#if C4SIMD

		for (machine j = 0; j < 8; j++)
		{
			vec_int16 lum = SimdInt8UnpackA(SimdInt8LoadUnaligned(input));
			vec_int16 baseLum = SimdInt8UnpackA(SimdInt8LoadUnaligned(base));

			reinterpret_cast<vec_float *>(output[j])[0] = SimdInt32ConvertFloat(SimdInt32Sub(SimdInt16UnpackA(lum), SimdInt16UnpackA(baseLum)));
			reinterpret_cast<vec_float *>(output[j])[1] = SimdInt32ConvertFloat(SimdInt32Sub(SimdInt16UnpackB(lum), SimdInt16UnpackB(baseLum)));

			input += rowLength;
			base += rowLength;
		}

	#else

		for (machine j = 0; j < 8; j++)
		{
			for (machine i = 0; i < 8; i++) output[j][i] = (float) (input[i] - base[i]);

			input += rowLength;
			base += rowLength;
		}

	#endif
}

void VideoCompressor::TransformQuantizeLuminanceBlock(const float (& input)[8][8], int16 *restrict output, const float (& quantizeMatrix)[8][8])
{
	#if C4SIMD

		float alignas(16)	lumBlock1[8][8];
		float alignas(16)	lumBlock2[8][8];

		const vec_float *transform = reinterpret_cast<const vec_float *>(blockTransformMatrix);

		for (machine j = 0; j < 8; j++)
		{
			vec_float l1 = reinterpret_cast<const vec_float *>(input[j])[0];
			vec_float l2 = reinterpret_cast<const vec_float *>(input[j])[1];

			vec_float m = SimdSmearX(l1);
			vec_float y1 = SimdMul(m, transform[0]);
			vec_float y2 = SimdMul(m, transform[1]);

			m = SimdSmearY(l1);
			y1 = SimdMadd(m, transform[2], y1);
			y2 = SimdMadd(m, transform[3], y2);

			m = SimdSmearZ(l1);
			y1 = SimdMadd(m, transform[4], y1);
			y2 = SimdMadd(m, transform[5], y2);

			m = SimdSmearW(l1);
			y1 = SimdMadd(m, transform[6], y1);
			y2 = SimdMadd(m, transform[7], y2);

			m = SimdSmearX(l2);
			y1 = SimdMadd(m, transform[8], y1);
			y2 = SimdMadd(m, transform[9], y2);

			m = SimdSmearY(l2);
			y1 = SimdMadd(m, transform[10], y1);
			y2 = SimdMadd(m, transform[11], y2);

			m = SimdSmearZ(l2);
			y1 = SimdMadd(m, transform[12], y1);
			y2 = SimdMadd(m, transform[13], y2);

			m = SimdSmearW(l2);
			y1 = SimdMadd(m, transform[14], y1);
			y2 = SimdMadd(m, transform[15], y2);

			reinterpret_cast<vec_float *>(lumBlock1[j])[0] = y1;
			reinterpret_cast<vec_float *>(lumBlock1[j])[1] = y2;
		}

		for (machine i = 0; i < 8; i++)
		{
			const float *lum = lumBlock1[0] + i;

			vec_float m = SimdLoadSmearScalar(lum, 0);
			vec_float y1 = SimdMul(m, transform[0]);
			vec_float y2 = SimdMul(m, transform[1]);

			m = SimdLoadSmearScalar(lum, 8);
			y1 = SimdMadd(m, transform[2], y1);
			y2 = SimdMadd(m, transform[3], y2);

			m = SimdLoadSmearScalar(lum, 16);
			y1 = SimdMadd(m, transform[4], y1);
			y2 = SimdMadd(m, transform[5], y2);

			m = SimdLoadSmearScalar(lum, 24);
			y1 = SimdMadd(m, transform[6], y1);
			y2 = SimdMadd(m, transform[7], y2);

			m = SimdLoadSmearScalar(lum, 32);
			y1 = SimdMadd(m, transform[8], y1);
			y2 = SimdMadd(m, transform[9], y2);

			m = SimdLoadSmearScalar(lum, 40);
			y1 = SimdMadd(m, transform[10], y1);
			y2 = SimdMadd(m, transform[11], y2);

			m = SimdLoadSmearScalar(lum, 48);
			y1 = SimdMadd(m, transform[12], y1);
			y2 = SimdMadd(m, transform[13], y2);

			m = SimdLoadSmearScalar(lum, 56);
			y1 = SimdMadd(m, transform[14], y1);
			y2 = SimdMadd(m, transform[15], y2);

			float *dst = &lumBlock2[0][i];
			SimdStoreX(y1, dst, 0);
			SimdStoreY(y1, dst, 8);
			SimdStoreZ(y1, dst, 16);
			SimdStoreW(y1, dst, 24);
			SimdStoreX(y2, dst, 32);
			SimdStoreY(y2, dst, 40);
			SimdStoreZ(y2, dst, 48);
			SimdStoreW(y2, dst, 56);
		}

		const vec_float *quantize = reinterpret_cast<const vec_float *>(quantizeMatrix);
		vec_float offset = SimdLoadConstant<0x3F000000>();

		for (machine j = 0; j < 8; j++)
		{
			const vec_float *lum = reinterpret_cast<vec_float *>(lumBlock2[j]);
			vec_int16 y1 = SimdInt32PackSaturate(SimdConvertInt32(SimdFloor(SimdMadd(lum[0], quantize[0], offset))));
			vec_int16 y2 = SimdInt32PackSaturate(SimdConvertInt32(SimdFloor(SimdMadd(lum[1], quantize[1], offset))));

			SimdInt32StoreX((vec_int32) y1, reinterpret_cast<int32 *>(output), 0);
			SimdInt32StoreY((vec_int32) y1, reinterpret_cast<int32 *>(output), 1);
			SimdInt32StoreX((vec_int32) y2, reinterpret_cast<int32 *>(output), 2);
			SimdInt32StoreY((vec_int32) y2, reinterpret_cast<int32 *>(output), 3);

			output += 8;
			quantize += 2;
		}

	#else

		float		lumBlock1[8][8];
		float		lumBlock2[8][8];

		const float (& transform)[8][8] = blockTransformMatrix;

		for (machine j = 0; j < 8; j++)
		{
			const float *lum = input[j];
			for (machine i = 0; i < 8; i++)
			{
				float y = lum[0] * transform[0][i];
				for (machine k = 1; k < 8; k++) y += lum[k] * transform[k][i];
				lumBlock1[j][i] = y;
			}
		}

		for (machine i = 0; i < 8; i++)
		{
			const float *lum = lumBlock1[0] + i;
			for (machine j = 0; j < 8; j++)
			{
				float y = lum[0] * transform[0][j];
				for (machine k = 1; k < 8; k++) y += lum[k * 8] * transform[k][j];
				lumBlock2[j][i] = y;
			}
		}

		for (machine j = 0; j < 8; j++)
		{
			const float *lum = lumBlock2[j];
			for (machine i = 0; i < 8; i++)
			{
				output[j * 8 + i] = (int16) Floor(lum[i] * quantizeMatrix[j][i] + 0.5F);
			}
		}

	#endif
}

void VideoCompressor::StoreLuminanceBlock(const float (& input)[8][8], int8 *restrict output, int32 rowLength)
{
	#if C4SIMD

		vec_float offset = SimdLoadConstant<0x3F000000>();

		for (machine j = 0; j < 8; j++)
		{
			const vec_float *lum = reinterpret_cast<const vec_float *>(input[j]);
			vec_int8 y1 = SimdInt16PackSaturate(SimdInt32PackSaturate(SimdConvertInt32(SimdAdd(lum[0], offset))));
			vec_int8 y2 = SimdInt16PackSaturate(SimdInt32PackSaturate(SimdConvertInt32(SimdAdd(lum[1], offset))));

			SimdInt32StoreX((vec_int32) y1, reinterpret_cast<int32 *>(output), 0);
			SimdInt32StoreX((vec_int32) y2, reinterpret_cast<int32 *>(output), 1);

			output += rowLength;
		}

	#else

		for (machine j = 0; j < 8; j++)
		{
			const float *lum = input[j];
			for (machine i = 0; i < 8; i++)
			{
				output[i] = Min(Max((int32) (lum[i] + 0.5F), -128), 127);
			}

			output += rowLength;
		}

	#endif
}

void VideoCompressor::LoadChrominanceBlock(const int8 *input, float (*restrict blueOutput)[8], float (*restrict redOutput)[8], int32 rowLength)
{
	#if C4SIMD

		for (machine j = 0; j < 8; j++)
		{
			vec_int8 chrom = *reinterpret_cast<const vec_int8 *>(input);
			vec_int16 c1 = SimdInt16Deinterleave(SimdInt8UnpackA(chrom));
			vec_int16 c2 = SimdInt16Deinterleave(SimdInt8UnpackB(chrom));

			reinterpret_cast<vec_float *>(blueOutput[j])[0] = SimdInt32ConvertFloat(SimdInt16UnpackA(c1));
			reinterpret_cast<vec_float *>(blueOutput[j])[1] = SimdInt32ConvertFloat(SimdInt16UnpackA(c2));
			reinterpret_cast<vec_float *>(redOutput[j])[0] = SimdInt32ConvertFloat(SimdInt16UnpackB(c1));
			reinterpret_cast<vec_float *>(redOutput[j])[1] = SimdInt32ConvertFloat(SimdInt16UnpackB(c2));

			input += rowLength * 2;
		}

	#else

		for (machine j = 0; j < 8; j++)
		{
			for (machine i = 0; i < 8; i++)
			{
				blueOutput[j][i] = (float) input[i * 2];
				redOutput[j][i] = (float) input[i * 2 + 1];
			}

			input += rowLength * 2;
		}

	#endif
}

void VideoCompressor::LoadDeltaChrominanceBlock(const int8 *input, const int8 *base, float (*restrict blueOutput)[8], float (*restrict redOutput)[8], int32 rowLength)
{
	#if C4SIMD

		for (machine j = 0; j < 8; j++)
		{
			vec_int8 chrom = *reinterpret_cast<const vec_int8 *>(input);
			vec_int16 c1 = SimdInt16Deinterleave(SimdInt8UnpackA(chrom));
			vec_int16 c2 = SimdInt16Deinterleave(SimdInt8UnpackB(chrom));

			vec_int8 baseChrom = *reinterpret_cast<const vec_int8 *>(base);
			vec_int16 bc1 = SimdInt16Deinterleave(SimdInt8UnpackA(baseChrom));
			vec_int16 bc2 = SimdInt16Deinterleave(SimdInt8UnpackB(baseChrom));

			reinterpret_cast<vec_float *>(blueOutput[j])[0] = SimdInt32ConvertFloat(SimdInt32Sub(SimdInt16UnpackA(c1), SimdInt16UnpackA(bc1)));
			reinterpret_cast<vec_float *>(blueOutput[j])[1] = SimdInt32ConvertFloat(SimdInt32Sub(SimdInt16UnpackA(c2), SimdInt16UnpackA(bc2)));
			reinterpret_cast<vec_float *>(redOutput[j])[0] = SimdInt32ConvertFloat(SimdInt32Sub(SimdInt16UnpackB(c1), SimdInt16UnpackB(bc1)));
			reinterpret_cast<vec_float *>(redOutput[j])[1] = SimdInt32ConvertFloat(SimdInt32Sub(SimdInt16UnpackB(c2), SimdInt16UnpackB(bc2)));

			input += rowLength * 2;
			base += rowLength * 2;
		}

	#else

		for (machine j = 0; j < 8; j++)
		{
			for (machine i = 0; i < 8; i++)
			{
				blueOutput[j][i] = (float) (input[i * 2] - base[i * 2]);
				redOutput[j][i] = (float) (input[i * 2 + 1] - base[i * 2 + 1]);
			}

			input += rowLength * 2;
			base += rowLength * 2;
		}

	#endif
}

void VideoCompressor::TransformQuantizeChrominanceBlock(const float (& blueInput)[8][8], const float (& redInput)[8][8], int16 *restrict blueOutput, int16 *restrict redOutput, const float (& quantizeMatrix)[8][8])
{
	#if C4SIMD

		float alignas(16)	blueBlock1[8][8];
		float alignas(16)	blueBlock2[8][8];
		float alignas(16)	redBlock1[8][8];
		float alignas(16)	redBlock2[8][8];

		const vec_float *transform = reinterpret_cast<const vec_float *>(blockTransformMatrix);

		for (machine j = 0; j < 8; j++)
		{
			vec_float u1 = reinterpret_cast<const vec_float *>(blueInput[j])[0];
			vec_float u2 = reinterpret_cast<const vec_float *>(blueInput[j])[1];
			vec_float v1 = reinterpret_cast<const vec_float *>(redInput[j])[0];
			vec_float v2 = reinterpret_cast<const vec_float *>(redInput[j])[1];

			vec_float p = SimdSmearX(u1);
			vec_float q = SimdSmearX(v1);
			vec_float b1 = SimdMul(p, transform[0]);
			vec_float b2 = SimdMul(p, transform[1]);
			vec_float r1 = SimdMul(q, transform[0]);
			vec_float r2 = SimdMul(q, transform[1]);

			p = SimdSmearY(u1);
			q = SimdSmearY(v1);
			b1 = SimdMadd(p, transform[2], b1);
			b2 = SimdMadd(p, transform[3], b2);
			r1 = SimdMadd(q, transform[2], r1);
			r2 = SimdMadd(q, transform[3], r2);

			p = SimdSmearZ(u1);
			q = SimdSmearZ(v1);
			b1 = SimdMadd(p, transform[4], b1);
			b2 = SimdMadd(p, transform[5], b2);
			r1 = SimdMadd(q, transform[4], r1);
			r2 = SimdMadd(q, transform[5], r2);

			p = SimdSmearW(u1);
			q = SimdSmearW(v1);
			b1 = SimdMadd(p, transform[6], b1);
			b2 = SimdMadd(p, transform[7], b2);
			r1 = SimdMadd(q, transform[6], r1);
			r2 = SimdMadd(q, transform[7], r2);

			p = SimdSmearX(u2);
			q = SimdSmearX(v2);
			b1 = SimdMadd(p, transform[8], b1);
			b2 = SimdMadd(p, transform[9], b2);
			r1 = SimdMadd(q, transform[8], r1);
			r2 = SimdMadd(q, transform[9], r2);

			p = SimdSmearY(u2);
			q = SimdSmearY(v2);
			b1 = SimdMadd(p, transform[10], b1);
			b2 = SimdMadd(p, transform[11], b2);
			r1 = SimdMadd(q, transform[10], r1);
			r2 = SimdMadd(q, transform[11], r2);

			p = SimdSmearZ(u2);
			q = SimdSmearZ(v2);
			b1 = SimdMadd(p, transform[12], b1);
			b2 = SimdMadd(p, transform[13], b2);
			r1 = SimdMadd(q, transform[12], r1);
			r2 = SimdMadd(q, transform[13], r2);

			p = SimdSmearW(u2);
			q = SimdSmearW(v2);
			b1 = SimdMadd(p, transform[14], b1);
			b2 = SimdMadd(p, transform[15], b2);
			r1 = SimdMadd(q, transform[14], r1);
			r2 = SimdMadd(q, transform[15], r2);

			reinterpret_cast<vec_float *>(blueBlock1[j])[0] = b1;
			reinterpret_cast<vec_float *>(blueBlock1[j])[1] = b2;
			reinterpret_cast<vec_float *>(redBlock1[j])[0] = r1;
			reinterpret_cast<vec_float *>(redBlock1[j])[1] = r2;
		}

		for (machine i = 0; i < 8; i++)
		{
			const float *blue = blueBlock1[0] + i;
			const float *red = redBlock1[0] + i;

			vec_float p = SimdLoadSmearScalar(blue, 0);
			vec_float q = SimdLoadSmearScalar(red, 0);
			vec_float b1 = SimdMul(p, transform[0]);
			vec_float b2 = SimdMul(p, transform[1]);
			vec_float r1 = SimdMul(q, transform[0]);
			vec_float r2 = SimdMul(q, transform[1]);

			p = SimdLoadSmearScalar(blue, 8);
			q = SimdLoadSmearScalar(red, 8);
			b1 = SimdMadd(p, transform[2], b1);
			b2 = SimdMadd(p, transform[3], b2);
			r1 = SimdMadd(q, transform[2], r1);
			r2 = SimdMadd(q, transform[3], r2);

			p = SimdLoadSmearScalar(blue, 16);
			q = SimdLoadSmearScalar(red, 16);
			b1 = SimdMadd(p, transform[4], b1);
			b2 = SimdMadd(p, transform[5], b2);
			r1 = SimdMadd(q, transform[4], r1);
			r2 = SimdMadd(q, transform[5], r2);

			p = SimdLoadSmearScalar(blue, 24);
			q = SimdLoadSmearScalar(red, 24);
			b1 = SimdMadd(p, transform[6], b1);
			b2 = SimdMadd(p, transform[7], b2);
			r1 = SimdMadd(q, transform[6], r1);
			r2 = SimdMadd(q, transform[7], r2);

			p = SimdLoadSmearScalar(blue, 32);
			q = SimdLoadSmearScalar(red, 32);
			b1 = SimdMadd(p, transform[8], b1);
			b2 = SimdMadd(p, transform[9], b2);
			r1 = SimdMadd(q, transform[8], r1);
			r2 = SimdMadd(q, transform[9], r2);

			p = SimdLoadSmearScalar(blue, 40);
			q = SimdLoadSmearScalar(red, 40);
			b1 = SimdMadd(p, transform[10], b1);
			b2 = SimdMadd(p, transform[11], b2);
			r1 = SimdMadd(q, transform[10], r1);
			r2 = SimdMadd(q, transform[11], r2);

			p = SimdLoadSmearScalar(blue, 48);
			q = SimdLoadSmearScalar(red, 48);
			b1 = SimdMadd(p, transform[12], b1);
			b2 = SimdMadd(p, transform[13], b2);
			r1 = SimdMadd(q, transform[12], r1);
			r2 = SimdMadd(q, transform[13], r2);

			p = SimdLoadSmearScalar(blue, 56);
			q = SimdLoadSmearScalar(red, 56);
			b1 = SimdMadd(p, transform[14], b1);
			b2 = SimdMadd(p, transform[15], b2);
			r1 = SimdMadd(q, transform[14], r1);
			r2 = SimdMadd(q, transform[15], r2);

			float *dst = &blueBlock2[0][i];
			SimdStoreX(b1, dst, 0);
			SimdStoreY(b1, dst, 8);
			SimdStoreZ(b1, dst, 16);
			SimdStoreW(b1, dst, 24);
			SimdStoreX(b2, dst, 32);
			SimdStoreY(b2, dst, 40);
			SimdStoreZ(b2, dst, 48);
			SimdStoreW(b2, dst, 56);

			dst = &redBlock2[0][i];
			SimdStoreX(r1, dst, 0);
			SimdStoreY(r1, dst, 8);
			SimdStoreZ(r1, dst, 16);
			SimdStoreW(r1, dst, 24);
			SimdStoreX(r2, dst, 32);
			SimdStoreY(r2, dst, 40);
			SimdStoreZ(r2, dst, 48);
			SimdStoreW(r2, dst, 56);
		}

		const vec_float *quantize = reinterpret_cast<const vec_float *>(quantizeMatrix);
		vec_float offset = SimdLoadConstant<0x3F000000>();

		for (machine j = 0; j < 8; j++)
		{
			const vec_float *blue = reinterpret_cast<vec_float *>(blueBlock2[j]);
			vec_int16 b1 = SimdInt32PackSaturate(SimdConvertInt32(SimdFloor(SimdMadd(blue[0], quantize[0], offset))));
			vec_int16 b2 = SimdInt32PackSaturate(SimdConvertInt32(SimdFloor(SimdMadd(blue[1], quantize[1], offset))));

			SimdInt32StoreX((vec_int32) b1, reinterpret_cast<int32 *>(blueOutput), 0);
			SimdInt32StoreY((vec_int32) b1, reinterpret_cast<int32 *>(blueOutput), 1);
			SimdInt32StoreX((vec_int32) b2, reinterpret_cast<int32 *>(blueOutput), 2);
			SimdInt32StoreY((vec_int32) b2, reinterpret_cast<int32 *>(blueOutput), 3);

			const vec_float *red = reinterpret_cast<vec_float *>(redBlock2[j]);
			vec_int16 r1 = SimdInt32PackSaturate(SimdConvertInt32(SimdFloor(SimdMadd(red[0], quantize[0], offset))));
			vec_int16 r2 = SimdInt32PackSaturate(SimdConvertInt32(SimdFloor(SimdMadd(red[1], quantize[1], offset))));

			SimdInt32StoreX((vec_int32) r1, reinterpret_cast<int32 *>(redOutput), 0);
			SimdInt32StoreY((vec_int32) r1, reinterpret_cast<int32 *>(redOutput), 1);
			SimdInt32StoreX((vec_int32) r2, reinterpret_cast<int32 *>(redOutput), 2);
			SimdInt32StoreY((vec_int32) r2, reinterpret_cast<int32 *>(redOutput), 3);

			blueOutput += 8;
			redOutput += 8;
			quantize += 2;
		}

	#else

		float		blueBlock1[8][8];
		float		blueBlock2[8][8];
		float		redBlock1[8][8];
		float		redBlock2[8][8];

		const float (& transform)[8][8] = blockTransformMatrix;

		for (machine j = 0; j < 8; j++)
		{
			const float *blue = blueInput[j];
			const float *red = redInput[j];

			for (machine i = 0; i < 8; i++)
			{
				float b = blue[0] * transform[0][i];
				float r = red[0] * transform[0][i];

				for (machine k = 1; k < 8; k++)
				{
					b += blue[k] * transform[k][i];
					r += red[k] * transform[k][i];
				}

				blueBlock1[j][i] = b;
				redBlock1[j][i] = r;
			}
		}

		for (machine i = 0; i < 8; i++)
		{
			const float *blue = blueBlock1[0] + i;
			const float *red = redBlock1[0] + i;

			for (machine j = 0; j < 8; j++)
			{
				float b = blue[0] * transform[0][j];
				float r = red[0] * transform[0][j];

				for (machine k = 1; k < 8; k++)
				{
					b += blue[k * 8] * transform[k][j];
					r += red[k * 8] * transform[k][j];
				}

				blueBlock2[j][i] = b;
				redBlock2[j][i] = r;
			}
		}

		for (machine j = 0; j < 8; j++)
		{
			const float *blue = blueBlock2[j];
			const float *red = redBlock2[j];

			for (machine i = 0; i < 8; i++)
			{
				blueOutput[j * 8 + i] = (int16) Floor(blue[i] * quantizeMatrix[j][i] + 0.5F);
				redOutput[j * 8 + i] = (int16) Floor((float) red[i] * quantizeMatrix[j][i] + 0.5F);
			}
		}

	#endif
}

void VideoCompressor::StoreChrominanceBlock(const float (& blueInput)[8][8], const float (& redInput)[8][8], int8 *restrict output, int32 rowLength)
{
	#if C4SIMD

		vec_float offset = SimdLoadConstant<0x3F000000>();

		for (machine j = 0; j < 8; j++)
		{
			const vec_float *blue = reinterpret_cast<const vec_float *>(blueInput[j]);
			vec_int8 b1 = SimdInt16PackSaturate(SimdInt32PackSaturate(SimdConvertInt32(SimdAdd(blue[0], offset))));
			vec_int8 b2 = SimdInt16PackSaturate(SimdInt32PackSaturate(SimdConvertInt32(SimdAdd(blue[1], offset))));

			const vec_float *red = reinterpret_cast<const vec_float *>(redInput[j]);
			vec_int8 r1 = SimdInt16PackSaturate(SimdInt32PackSaturate(SimdConvertInt32(SimdAdd(red[0], offset))));
			vec_int8 r2 = SimdInt16PackSaturate(SimdInt32PackSaturate(SimdConvertInt32(SimdAdd(red[1], offset))));

			vec_int8 m1 = SimdInt8MergeA(b1, r1);
			vec_int8 m2 = SimdInt8MergeA(b2, r2);

			SimdInt32StoreX((vec_int32) m1, reinterpret_cast<int32 *>(output), 0);
			SimdInt32StoreY((vec_int32) m1, reinterpret_cast<int32 *>(output), 1);
			SimdInt32StoreX((vec_int32) m2, reinterpret_cast<int32 *>(output), 2);
			SimdInt32StoreY((vec_int32) m2, reinterpret_cast<int32 *>(output), 3);

			output += rowLength * 2;
		}

	#else

		for (machine j = 0; j < 8; j++)
		{
			const float *blue = blueInput[j];
			const float *red = redInput[j];

			for (machine i = 0; i < 8; i++)
			{
				output[i * 2] = Min(Max((int32) (blue[i] + 0.5F), -128), 127);
				output[i * 2 + 1] = Min(Max((int32) (red[i] + 0.5F), -128), 127);
			}

			output += rowLength * 2;
		}

	#endif
}

unsigned_int32 VideoCompressor::EncodeFlatRow(unsigned_int32 count, int16 *restrict data, unsigned_int8 *restrict code)
{
	int32 minDelta = 0;
	int32 maxDelta = 0;

	for (machine a = count - 1; a > 0; a--)
	{
		int32 delta = data[a] - data[a - 1];
		data[a] = (int16) delta;

		minDelta = Min(minDelta, delta);
		maxDelta = Max(maxDelta, delta);
	}

	unsigned_int32 first = data[0] & 0x0FFF;
	code[0] = (unsigned_int8) first;

	if ((minDelta >= -128) && (maxDelta <= 127))
	{
		code[1] = (unsigned_int8) (first >> 8);
		code++;

		for (unsigned_machine a = 1; a < count; a++) code[a] = (unsigned_int8) data[a];

		if ((count & 1) == 0)
		{
			code[count] = 0;
			count++;
		}

		return (count + 1);
	}

	code[1] = (unsigned_int8) ((first | 0x1000) >> 8);
	code += 2;

	for (unsigned_machine a = 1; a < count; a++)
	{
		int32 delta = data[a];
		code[0] = (unsigned_int8) delta;
		code[1] = (unsigned_int8) (delta >> 8);
		code += 2;
	}

	return (count * 2);
}

unsigned_int32 VideoCompressor::EncodeWavyBlock(const int16 *data, unsigned_int8 *restrict length, unsigned_int8 *restrict code)
{
	machine count = 0;
	int32 minValue = 0;
	int32 maxValue = 0;

	for (machine a = 63; a > 0; a--)
	{
		int32 value = data[blockLinearizeTable[a]];
		if (value != 0)
		{
			count = a;
			minValue = value;
			maxValue = value;
			break;
		}
	}

	for (machine a = count - 1; a > 0; a--)
	{
		int32 value = data[blockLinearizeTable[a]];
		minValue = Min(minValue, value);
		maxValue = Max(maxValue, value);
	}

	if ((minValue >= -2) && (maxValue <= 1))
	{
		*length = (unsigned_int8) count;

		machine k = count & ~3;
		for (machine a = 1; a <= k; a += 4)
		{
			unsigned_int32 value = data[blockLinearizeTable[a]] & 0x03;
			value |= (data[blockLinearizeTable[a + 1]] << 2) & 0x0C;
			value |= (data[blockLinearizeTable[a + 2]] << 4) & 0x30;
			value |= (data[blockLinearizeTable[a + 3]] << 6) & 0xC0;
			*code++ = (unsigned_int8) value;
		}

		if (count & 3)
		{
			unsigned_int32 value = data[blockLinearizeTable[count]] & 0x03;
			for (machine a = count - 1; a > k; a--)
			{
				value = (value << 2) | (data[blockLinearizeTable[a]] & 0x03);
			}

			*code = (unsigned_int8) value;
		}

		return ((unsigned_int32) ((count + 3) >> 2));
	}

	if ((minValue >= -8) && (maxValue <= 7))
	{
		*length = (unsigned_int8) (count | 0x40);

		machine k = count & ~1;
		for (machine a = 1; a <= k; a += 2)
		{
			unsigned_int32 value = data[blockLinearizeTable[a]] & 0x0F;
			value |= (data[blockLinearizeTable[a + 1]] << 4) & 0xF0;
			*code++ = (unsigned_int8) value;
		}

		if (count & 1)
		{
			*code = (unsigned_int8) data[blockLinearizeTable[count]] & 0x0F;
		}

		return ((unsigned_int32) ((count + 1) >> 1));
	}

	if ((minValue >= -128) && (maxValue <= 127))
	{
		*length = (unsigned_int8) (count | 0x80);

		for (machine a = 1; a <= count; a++)
		{
			*code++ = (unsigned_int8) data[blockLinearizeTable[a]];
		}

		return ((unsigned_int32) count);
	}

	*length = (unsigned_int8) (count | 0xC0);

	machine k = count & ~1;
	for (machine a = 1; a <= k; a += 2)
	{
		int32 value1 = Min(Max(data[blockLinearizeTable[a]], -2048), 2047);
		int32 value2 = Min(Max(data[blockLinearizeTable[a + 1]], -2048), 2047);
		code[0] = (unsigned_int8) value1;
		code[1] = (unsigned_int8) value2;
		code[2] = (unsigned_int8) (((value1 >> 8) & 0x0F) | ((value2 >> 4) & 0xF0));
		code += 3;
	}

	if (count & 1)
	{
		int32 value = Min(Max(data[blockLinearizeTable[count]], -2048), 2047);
		code[0] = (unsigned_int8) value;
		code[1] = (unsigned_int8) (value >> 8);
	}

	return ((unsigned_int32) ((count * 3 + 1) >> 1));
}

unsigned_int32 VideoCompressor::EncodeWavyRow(unsigned_int32 count, const int16 *data, unsigned_int8 *restrict length, unsigned_int8 *restrict code)
{
	const unsigned_int8 *start = code;

	for (unsigned_machine a = 0; a < count; a++)
	{
		code += EncodeWavyBlock(data, length, code);
		data += 64;
		length++;
	}

	return ((unsigned_int32) (code - start));
}

unsigned_int32 VideoCompressor::CompressLuminance(MovieVideoFrameHeader *videoFrameHeader, float quantScale, unsigned_int32 flags, int32 index)
{
	float alignas(16)	quantizeMatrix[8][8];
	float alignas(16)	dequantizeMatrix[8][8];
	float alignas(16)	lumBlock[8][8];

	bool delta = ((flags & kMovieVideoCompressDeltaFrame) != 0);

	Rect *bounds = &videoFrameHeader->luminanceData.channelBoundingRect;
	if (delta) FindDeltaLuminanceBounds(luminanceImage[2], luminanceImage[baseImageIndex], videoFrameSize, bounds);
	else FindLuminanceBounds(luminanceImage[index], videoFrameSize, bounds);

	videoFrameHeader->luminanceData.quantizationScale = quantScale;

	float f = 1.0F / quantScale;
	for (machine j = 0; j < 8; j++)
	{
		for (machine i = 0; i < 8; i++)
		{
			float m = luminanceQuantizeMatrix[j][i];
			quantizeMatrix[j][i] = f / m;
			dequantizeMatrix[j][i] = m * quantScale;
		}
	}

	int8 *image = luminanceImage[index];
	int32 width = videoFrameSize.x;
	int32 offset = bounds->top * width + bounds->left;
	image += offset;

	int32 blockCountX = bounds->Width() >> 3;
	int32 blockCountY = bounds->Height() >> 3;

	int16 *flatData = blockData + blockCountX * 64;

	unsigned_int8 *restrict flatCode = flatLuminCode;
	unsigned_int8 *restrict wlenCode = wlenLuminCode;
	unsigned_int8 *restrict wavyCode = wavyLuminCode;

	if (delta)
	{
		const int8 *base = luminanceImage[baseImageIndex] + offset;

		for (machine j = 0; j < blockCountY; j++)
		{
			for (machine i = 0; i < blockCountX; i++)
			{
				LoadDeltaLuminanceBlock(image + i * 8, base + i * 8, lumBlock, width);
				TransformQuantizeLuminanceBlock(lumBlock, blockData + i * 64, quantizeMatrix);
				flatData[i] = blockData[i * 64];
			}

			flatCode += EncodeFlatRow(blockCountX, flatData, flatCode);
			wavyCode += EncodeWavyRow(blockCountX, blockData, wlenCode, wavyCode);
			wlenCode += blockCountX;

			image += width * 8;
			base += width * 8;
		}
	}
	else
	{
		for (machine j = 0; j < blockCountY; j++)
		{
			for (machine i = 0; i < blockCountX; i++)
			{
				LoadLuminanceBlock(image + i * 8, lumBlock, width);
				TransformQuantizeLuminanceBlock(lumBlock, blockData + i * 64, quantizeMatrix);
				flatData[i] = blockData[i * 64];

				VideoDecompressor::DequantizeInverseTransformLuminanceBlock(blockData + i * 64, lumBlock, dequantizeMatrix);
				StoreLuminanceBlock(lumBlock, image + i * 8, width);
			}

			flatCode += EncodeFlatRow(blockCountX, flatData, flatCode);
			wavyCode += EncodeWavyRow(blockCountX, blockData, wlenCode, wavyCode);
			wlenCode += blockCountX;

			image += width * 8;
		}
	}

	unsigned_int32 flatDataSize = flatCode - flatLuminCode;
	unsigned_int32 wlenDataSize = blockCountX * blockCountY;
	unsigned_int32 wavyDataSize = wavyCode - wavyLuminCode;

	unsigned_int8 flatCompression = kMovieCompressionGeneral;
	unsigned_int8 wlenCompression = kMovieCompressionGeneral;
	unsigned_int8 wavyCompression = kMovieCompressionGeneral;

	unsigned_int8 *code = compressedCode[delta];

	unsigned_int32 flatCodeSize = Comp::CompressData(flatLuminCode, flatDataSize, code);
	if (flatCodeSize == 0)
	{
		MemoryMgr::CopyMemory(flatLuminCode, code, flatDataSize);
		flatCompression = kMovieCompressionNone;
		flatCodeSize = flatDataSize;
	}

	unsigned_int32 wlenCodeSize = Comp::CompressData(wlenLuminCode, wlenDataSize, code + flatCodeSize);
	if (wlenCodeSize == 0)
	{
		MemoryMgr::CopyMemory(wlenLuminCode, code + flatCodeSize, wlenDataSize);
		wlenCompression = kMovieCompressionNone;
		wlenCodeSize = wlenDataSize;
	}

	unsigned_int32 wavyCodeSize = Comp::CompressData(wavyLuminCode, wavyDataSize, code + (flatCodeSize + wlenCodeSize));
	if (wavyCodeSize == 0)
	{
		MemoryMgr::CopyMemory(wavyLuminCode, code + (flatCodeSize + wlenCodeSize), wavyDataSize);
		wavyCompression = kMovieCompressionNone;
		wavyCodeSize = wavyDataSize;
	}

	videoFrameHeader->luminanceData.videoChannelFlags = flatCompression | (wlenCompression << 4) | (wavyCompression << 8);

	videoFrameHeader->luminanceData.flatCodeOffset = sizeof(MovieVideoFrameHeader);
	videoFrameHeader->luminanceData.flatCodeSize = flatCodeSize;
	videoFrameHeader->luminanceData.flatDataSize = flatDataSize;

	videoFrameHeader->luminanceData.wlenCodeOffset = sizeof(MovieVideoFrameHeader) + flatCodeSize;
	videoFrameHeader->luminanceData.wlenCodeSize = wlenCodeSize;
	videoFrameHeader->luminanceData.wlenDataSize = wlenDataSize;

	videoFrameHeader->luminanceData.wavyCodeOffset = sizeof(MovieVideoFrameHeader) + flatCodeSize + wlenCodeSize;
	videoFrameHeader->luminanceData.wavyCodeSize = wavyCodeSize;
	videoFrameHeader->luminanceData.wavyDataSize = wavyDataSize;

	return (flatCodeSize + wlenCodeSize + wavyCodeSize);
}

unsigned_int32 VideoCompressor::CompressChrominance(MovieVideoFrameHeader *videoFrameHeader, float quantScale, unsigned_int32 flags, int32 index, unsigned_int32 codeOffset)
{
	float alignas(16)	quantizeMatrix[8][8];
	float alignas(16)	dequantizeMatrix[8][8];
	float alignas(16)	blueBlock[8][8];
	float alignas(16)	redBlock[8][8];

	bool delta = ((flags & kMovieVideoCompressDeltaFrame) != 0);

	Rect *bounds = &videoFrameHeader->chrominanceData.channelBoundingRect;
	if (delta) FindDeltaChrominanceBounds(chrominanceImage[2], chrominanceImage[baseImageIndex], videoFrameSize, bounds);
	else FindChrominanceBounds(chrominanceImage[index], videoFrameSize, bounds);

	videoFrameHeader->chrominanceData.quantizationScale = quantScale;

	float f = 1.0F / quantScale;
	for (machine j = 0; j < 8; j++)
	{
		for (machine i = 0; i < 8; i++)
		{
			float m = chrominanceQuantizeMatrix[j][i];
			quantizeMatrix[j][i] = f / m;
			dequantizeMatrix[j][i] = m * quantScale;
		}
	}

	int8 *image = chrominanceImage[index];
	int32 width = videoFrameSize.x >> 1;
	int32 offset = (bounds->top * width + bounds->left) * 2;
	image += offset;

	int32 blockCountX = bounds->Width() >> 3;
	int32 blockCountY = bounds->Height() >> 3;

	int16 *blueBlockData = blockData;
	int16 *redBlockData = blueBlockData + blockCountX * 64;
	int16 *blueFlatData = redBlockData + blockCountX * 64;
	int16 *redFlatData = blueFlatData + blockCountX;

	unsigned_int8 *restrict flatCode = flatChromCode;
	unsigned_int8 *restrict wlenCode = wlenChromCode;
	unsigned_int8 *restrict wavyCode = wavyChromCode;

	if (delta)
	{
		const int8 *base = chrominanceImage[baseImageIndex] + offset;

		for (machine j = 0; j < blockCountY; j++)
		{
			for (machine i = 0; i < blockCountX; i++)
			{
				LoadDeltaChrominanceBlock(image + i * 16, base + i * 16, blueBlock, redBlock, width);
				TransformQuantizeChrominanceBlock(blueBlock, redBlock, blueBlockData + i * 64, redBlockData + i * 64, quantizeMatrix);
				blueFlatData[i] = blueBlockData[i * 64];
				redFlatData[i] = redBlockData[i * 64];
			}

			flatCode += EncodeFlatRow(blockCountX, blueFlatData, flatCode);
			flatCode += EncodeFlatRow(blockCountX, redFlatData, flatCode);

			wavyCode += EncodeWavyRow(blockCountX, blueBlockData, wlenCode, wavyCode);
			wlenCode += blockCountX;

			wavyCode += EncodeWavyRow(blockCountX, redBlockData, wlenCode, wavyCode);
			wlenCode += blockCountX;

			image += width * 16;
			base += width * 16;
		}
	}
	else
	{
		for (machine j = 0; j < blockCountY; j++)
		{
			for (machine i = 0; i < blockCountX; i++)
			{
				LoadChrominanceBlock(image + i * 16, blueBlock, redBlock, width);
				TransformQuantizeChrominanceBlock(blueBlock, redBlock, blueBlockData + i * 64, redBlockData + i * 64, quantizeMatrix);
				blueFlatData[i] = blueBlockData[i * 64];
				redFlatData[i] = redBlockData[i * 64];

				VideoDecompressor::DequantizeInverseTransformChrominanceBlock(blueBlockData + i * 64, redBlockData + i * 64, blueBlock, redBlock, dequantizeMatrix);
				StoreChrominanceBlock(blueBlock, redBlock, image + i * 16, width);
			}

			flatCode += EncodeFlatRow(blockCountX, blueFlatData, flatCode);
			flatCode += EncodeFlatRow(blockCountX, redFlatData, flatCode);

			wavyCode += EncodeWavyRow(blockCountX, blueBlockData, wlenCode, wavyCode);
			wlenCode += blockCountX;

			wavyCode += EncodeWavyRow(blockCountX, redBlockData, wlenCode, wavyCode);
			wlenCode += blockCountX;

			image += width * 16;
		}
	}

	unsigned_int32 flatDataSize = flatCode - flatChromCode;
	unsigned_int32 wlenDataSize = blockCountX * blockCountY * 2;
	unsigned_int32 wavyDataSize = wavyCode - wavyChromCode;

	unsigned_int8 flatCompression = kMovieCompressionGeneral;
	unsigned_int8 wlenCompression = kMovieCompressionGeneral;
	unsigned_int8 wavyCompression = kMovieCompressionGeneral;

	unsigned_int8 *code = compressedCode[delta] + codeOffset;

	unsigned_int32 flatCodeSize = Comp::CompressData(flatChromCode, flatDataSize, code);
	if (flatCodeSize == 0)
	{
		MemoryMgr::CopyMemory(flatChromCode, code, flatDataSize);
		flatCompression = kMovieCompressionNone;
		flatCodeSize = flatDataSize;
	}

	unsigned_int32 wlenCodeSize = Comp::CompressData(wlenChromCode, wlenDataSize, code + flatCodeSize);
	if (wlenCodeSize == 0)
	{
		MemoryMgr::CopyMemory(wlenChromCode, code + flatCodeSize, wlenDataSize);
		wlenCompression = kMovieCompressionNone;
		wlenCodeSize = wlenDataSize;
	}

	unsigned_int32 wavyCodeSize = Comp::CompressData(wavyChromCode, wavyDataSize, code + (flatCodeSize + wlenCodeSize));
	if (wavyCodeSize == 0)
	{
		MemoryMgr::CopyMemory(wavyChromCode, code + (flatCodeSize + wlenCodeSize), wavyDataSize);
		wavyCompression = kMovieCompressionNone;
		wavyCodeSize = wavyDataSize;
	}

	videoFrameHeader->chrominanceData.videoChannelFlags = flatCompression | (wlenCompression << 4) | (wavyCompression << 8);

	unsigned_int32 codeBase = sizeof(MovieVideoFrameHeader) - sizeof(MovieVideoChannelData) + codeOffset;

	videoFrameHeader->chrominanceData.flatCodeOffset = codeBase;
	videoFrameHeader->chrominanceData.flatCodeSize = flatCodeSize;
	videoFrameHeader->chrominanceData.flatDataSize = flatDataSize;

	videoFrameHeader->chrominanceData.wlenCodeOffset = codeBase + flatCodeSize;
	videoFrameHeader->chrominanceData.wlenCodeSize = wlenCodeSize;
	videoFrameHeader->chrominanceData.wlenDataSize = wlenDataSize;

	videoFrameHeader->chrominanceData.wavyCodeOffset = codeBase + flatCodeSize + wlenCodeSize;
	videoFrameHeader->chrominanceData.wavyCodeSize = wavyCodeSize;
	videoFrameHeader->chrominanceData.wavyDataSize = wavyDataSize;

	return (flatCodeSize + wlenCodeSize + wavyCodeSize);
}

unsigned_int32 VideoCompressor::CompressAlpha(MovieVideoAlphaFrameHeader *videoFrameHeader, float quantScale, unsigned_int32 flags, int32 index, unsigned_int32 codeOffset)
{
	float alignas(16)	quantizeMatrix[8][8];
	float alignas(16)	dequantizeMatrix[8][8];
	float alignas(16)	alphaBlock[8][8];

	bool delta = ((flags & kMovieVideoCompressDeltaFrame) != 0);

	Rect *bounds = &videoFrameHeader->alphaData.channelBoundingRect;
	if (delta) FindDeltaLuminanceBounds(alphaImage[2], alphaImage[baseImageIndex], videoFrameSize, bounds);
	else FindLuminanceBounds(alphaImage[index], videoFrameSize, bounds);

	videoFrameHeader->alphaData.quantizationScale = quantScale;

	float f = 1.0F / quantScale;
	for (machine j = 0; j < 8; j++)
	{
		for (machine i = 0; i < 8; i++)
		{
			float m = luminanceQuantizeMatrix[j][i];
			quantizeMatrix[j][i] = f / m;
			dequantizeMatrix[j][i] = m * quantScale;
		}
	}

	int8 *image = alphaImage[index];
	int32 width = videoFrameSize.x;
	int32 offset = bounds->top * width + bounds->left;
	image += offset;

	int32 blockCountX = bounds->Width() >> 3;
	int32 blockCountY = bounds->Height() >> 3;

	int16 *flatData = blockData + blockCountX * 64;

	unsigned_int8 *restrict flatCode = flatAlphaCode;
	unsigned_int8 *restrict wlenCode = wlenAlphaCode;
	unsigned_int8 *restrict wavyCode = wavyAlphaCode;

	if (delta)
	{
		const int8 *base = alphaImage[baseImageIndex] + offset;

		for (machine j = 0; j < blockCountY; j++)
		{
			for (machine i = 0; i < blockCountX; i++)
			{
				LoadDeltaLuminanceBlock(image + i * 8, base + i * 8, alphaBlock, width);
				TransformQuantizeLuminanceBlock(alphaBlock, blockData + i * 64, quantizeMatrix);
				flatData[i] = blockData[i * 64];
			}

			flatCode += EncodeFlatRow(blockCountX, flatData, flatCode);
			wavyCode += EncodeWavyRow(blockCountX, blockData, wlenCode, wavyCode);
			wlenCode += blockCountX;

			image += width * 8;
			base += width * 8;
		}
	}
	else
	{
		for (machine j = 0; j < blockCountY; j++)
		{
			for (machine i = 0; i < blockCountX; i++)
			{
				LoadLuminanceBlock(image + i * 8, alphaBlock, width);
				TransformQuantizeLuminanceBlock(alphaBlock, blockData + i * 64, quantizeMatrix);
				flatData[i] = blockData[i * 64];

				VideoDecompressor::DequantizeInverseTransformLuminanceBlock(blockData + i * 64, alphaBlock, dequantizeMatrix);
				StoreLuminanceBlock(alphaBlock, image + i * 8, width);
			}

			flatCode += EncodeFlatRow(blockCountX, flatData, flatCode);
			wavyCode += EncodeWavyRow(blockCountX, blockData, wlenCode, wavyCode);
			wlenCode += blockCountX;

			image += width * 8;
		}
	}

	unsigned_int32 flatDataSize = flatCode - flatAlphaCode;
	unsigned_int32 wlenDataSize = blockCountX * blockCountY;
	unsigned_int32 wavyDataSize = wavyCode - wavyAlphaCode;

	unsigned_int8 flatCompression = kMovieCompressionGeneral;
	unsigned_int8 wlenCompression = kMovieCompressionGeneral;
	unsigned_int8 wavyCompression = kMovieCompressionGeneral;

	unsigned_int8 *code = compressedCode[delta] + codeOffset;

	unsigned_int32 flatCodeSize = Comp::CompressData(flatAlphaCode, flatDataSize, code);
	if (flatCodeSize == 0)
	{
		MemoryMgr::CopyMemory(flatAlphaCode, code, flatDataSize);
		flatCompression = kMovieCompressionNone;
		flatCodeSize = flatDataSize;
	}

	unsigned_int32 wlenCodeSize = Comp::CompressData(wlenAlphaCode, wlenDataSize, code + flatCodeSize);
	if (wlenCodeSize == 0)
	{
		MemoryMgr::CopyMemory(wlenAlphaCode, code + flatCodeSize, wlenDataSize);
		wlenCompression = kMovieCompressionNone;
		wlenCodeSize = wlenDataSize;
	}

	unsigned_int32 wavyCodeSize = Comp::CompressData(wavyAlphaCode, wavyDataSize, code + (flatCodeSize + wlenCodeSize));
	if (wavyCodeSize == 0)
	{
		MemoryMgr::CopyMemory(wavyAlphaCode, code + (flatCodeSize + wlenCodeSize), wavyDataSize);
		wavyCompression = kMovieCompressionNone;
		wavyCodeSize = wavyDataSize;
	}

	videoFrameHeader->alphaData.videoChannelFlags = flatCompression | (wlenCompression << 4) | (wavyCompression << 8);

	unsigned_int32 codeBase = sizeof(MovieVideoAlphaFrameHeader) - sizeof(MovieVideoChannelData) * 2 + codeOffset;

	videoFrameHeader->alphaData.flatCodeOffset = codeBase;
	videoFrameHeader->alphaData.flatCodeSize = flatCodeSize;
	videoFrameHeader->alphaData.flatDataSize = flatDataSize;

	videoFrameHeader->alphaData.wlenCodeOffset = codeBase + flatCodeSize;
	videoFrameHeader->alphaData.wlenCodeSize = wlenCodeSize;
	videoFrameHeader->alphaData.wlenDataSize = wlenDataSize;

	videoFrameHeader->alphaData.wavyCodeOffset = codeBase + flatCodeSize + wlenCodeSize;
	videoFrameHeader->alphaData.wavyCodeSize = wavyCodeSize;
	videoFrameHeader->alphaData.wavyDataSize = wavyDataSize;

	return (flatCodeSize + wlenCodeSize + wavyCodeSize);
}

unsigned_int32 VideoCompressor::CompressFrame(const void *image, ImageFormat format, MovieVideoFrameHeader *videoFrameHeader, float quantScale, unsigned_int32 flags)
{
	int32	index;

	if ((flags & kMovieVideoCompressDeltaFrame) != 0)
	{
		index = 2;
	}
	else if ((flags & kMovieVideoCompressBaseFrame) != 0)
	{
		index = 0;
		baseImageIndex = 0;
	}
	else
	{
		index = baseImageIndex ^ 1;
	}

	if (format != kImageFormatYCbCr)
	{
		const Color4C *color = static_cast<const Color4C *>(image);
		MovieMgr::ExtractLuminanceImage(videoFrameSize, color, luminanceImage[index]);
		MovieMgr::ExtractChrominanceImage(videoFrameSize, color, chrominanceImage[index]);
		if (videoTrackFlags & kMovieVideoAlphaChannel) MovieMgr::ExtractAlphaImage(videoFrameSize, color, alphaImage[index]);
	}
	else
	{
		int32 pixelCount = videoFrameSize.x * videoFrameSize.y;
		const int8 *luminance = static_cast<const int8 *>(image);
		const int8 *chrominance = luminance + pixelCount;

		MemoryMgr::CopyMemory(luminance, luminanceImage[index], pixelCount);
		MemoryMgr::CopyMemory(chrominance, chrominanceImage[index], pixelCount >> 1);
		if (videoTrackFlags & kMovieVideoAlphaChannel) MemoryMgr::FillMemory(alphaImage[index], pixelCount, 0x7F);
	}

	unsigned_int32 size = CompressLuminance(videoFrameHeader, quantScale, flags, index);
	size += CompressChrominance(videoFrameHeader, quantScale, flags, index, size);

	if (videoTrackFlags & kMovieVideoAlphaChannel)
	{
		videoFrameHeader->luminanceData.flatCodeOffset += sizeof(MovieVideoChannelData);
		videoFrameHeader->luminanceData.wlenCodeOffset += sizeof(MovieVideoChannelData);
		videoFrameHeader->luminanceData.wavyCodeOffset += sizeof(MovieVideoChannelData);

		videoFrameHeader->chrominanceData.flatCodeOffset += sizeof(MovieVideoChannelData);
		videoFrameHeader->chrominanceData.wlenCodeOffset += sizeof(MovieVideoChannelData);
		videoFrameHeader->chrominanceData.wavyCodeOffset += sizeof(MovieVideoChannelData);

		size += CompressAlpha(static_cast<MovieVideoAlphaFrameHeader *>(videoFrameHeader), quantScale, flags, index, size);
	}

	return (size);
}


VideoDecompressor::VideoDecompressor(const MovieVideoTrackHeader *videoTrackHeader)
{
	videoFrameSize = videoTrackHeader->videoFrameSize;

	int32 width = videoFrameSize.x;
	int32 height = videoFrameSize.y;
	unsigned_int32 pixelCount = width * height;
	unsigned_int32 imageSize = (pixelCount + (pixelCount >> 1)) * 2;
	if (videoTrackHeader->videoTrackFlags & kMovieVideoAlphaChannel) imageSize += pixelCount * 2;

	unsigned_int32 blockDataSize = (width >> 3) * 130;
	decompressorStorage = new char[imageSize + videoTrackHeader->maxFrameCodeSize + videoTrackHeader->maxFrameDataSize + blockDataSize];

	luminanceImage[0] = reinterpret_cast<Color1C *>(decompressorStorage);
	luminanceImage[1] = reinterpret_cast<Color1C *>(decompressorStorage + pixelCount);
	chrominanceImage[0] = reinterpret_cast<Color2C *>(decompressorStorage + pixelCount * 2);
	chrominanceImage[1] = reinterpret_cast<Color2C *>(decompressorStorage + (pixelCount * 2 + (pixelCount >> 1)));
	alphaImage[0] = reinterpret_cast<Color1C *>(decompressorStorage + (pixelCount * 2 + (pixelCount >> 1) * 2));
	alphaImage[1] = reinterpret_cast<Color1C *>(decompressorStorage + (pixelCount * 3 + (pixelCount >> 1) * 2));

	videoFrameHeader = reinterpret_cast<MovieVideoFrameHeader *>(decompressorStorage + imageSize);
	videoFrameData = reinterpret_cast<unsigned_int8 *>(decompressorStorage + (imageSize + videoTrackHeader->maxFrameCodeSize));

	blockData = reinterpret_cast<int16 *>(videoFrameData + videoTrackHeader->maxFrameDataSize);
}

VideoDecompressor::~VideoDecompressor()
{
	delete[] decompressorStorage;
}

void VideoDecompressor::DequantizeInverseTransformLuminanceBlock(const int16 *input, float (*restrict output)[8], const float (& dequantizeMatrix)[8][8])
{
	#if C4SIMD

		float alignas(16)	lumBlock[8][8];

		const vec_float *transform = reinterpret_cast<const vec_float *>(blockInverseTransformMatrix);
		const vec_float *dequantize = reinterpret_cast<const vec_float *>(dequantizeMatrix);

		for (machine j = 0; j < 8; j++)
		{
			vec_int16 lum = *reinterpret_cast<const vec_int16 *>(input);
			vec_float l1 = SimdMul(SimdInt32ConvertFloat(SimdInt16UnpackA(lum)), dequantize[0]);
			vec_float l2 = SimdMul(SimdInt32ConvertFloat(SimdInt16UnpackB(lum)), dequantize[1]);

			input += 8;
			dequantize += 2;

			vec_float m = SimdSmearX(l1);
			vec_float y1 = SimdMul(m, transform[0]);
			vec_float y2 = SimdMul(m, transform[1]);

			m = SimdSmearY(l1);
			y1 = SimdMadd(m, transform[2], y1);
			y2 = SimdMadd(m, transform[3], y2);

			m = SimdSmearZ(l1);
			y1 = SimdMadd(m, transform[4], y1);
			y2 = SimdMadd(m, transform[5], y2);

			m = SimdSmearW(l1);
			y1 = SimdMadd(m, transform[6], y1);
			y2 = SimdMadd(m, transform[7], y2);

			m = SimdSmearX(l2);
			y1 = SimdMadd(m, transform[8], y1);
			y2 = SimdMadd(m, transform[9], y2);

			m = SimdSmearY(l2);
			y1 = SimdMadd(m, transform[10], y1);
			y2 = SimdMadd(m, transform[11], y2);

			m = SimdSmearZ(l2);
			y1 = SimdMadd(m, transform[12], y1);
			y2 = SimdMadd(m, transform[13], y2);

			m = SimdSmearW(l2);
			y1 = SimdMadd(m, transform[14], y1);
			y2 = SimdMadd(m, transform[15], y2);

			reinterpret_cast<vec_float *>(lumBlock[j])[0] = y1;
			reinterpret_cast<vec_float *>(lumBlock[j])[1] = y2;
		}

		for (machine i = 0; i < 8; i++)
		{
			const float *lum = lumBlock[0] + i;

			vec_float m = SimdLoadSmearScalar(lum, 0);
			vec_float y1 = SimdMul(m, transform[0]);
			vec_float y2 = SimdMul(m, transform[1]);

			m = SimdLoadSmearScalar(lum, 8);
			y1 = SimdMadd(m, transform[2], y1);
			y2 = SimdMadd(m, transform[3], y2);

			m = SimdLoadSmearScalar(lum, 16);
			y1 = SimdMadd(m, transform[4], y1);
			y2 = SimdMadd(m, transform[5], y2);

			m = SimdLoadSmearScalar(lum, 24);
			y1 = SimdMadd(m, transform[6], y1);
			y2 = SimdMadd(m, transform[7], y2);

			m = SimdLoadSmearScalar(lum, 32);
			y1 = SimdMadd(m, transform[8], y1);
			y2 = SimdMadd(m, transform[9], y2);

			m = SimdLoadSmearScalar(lum, 40);
			y1 = SimdMadd(m, transform[10], y1);
			y2 = SimdMadd(m, transform[11], y2);

			m = SimdLoadSmearScalar(lum, 48);
			y1 = SimdMadd(m, transform[12], y1);
			y2 = SimdMadd(m, transform[13], y2);

			m = SimdLoadSmearScalar(lum, 56);
			y1 = SimdMadd(m, transform[14], y1);
			y2 = SimdMadd(m, transform[15], y2);

			float *dst = &output[0][i];
			SimdStoreX(y1, dst, 0);
			SimdStoreY(y1, dst, 8);
			SimdStoreZ(y1, dst, 16);
			SimdStoreW(y1, dst, 24);
			SimdStoreX(y2, dst, 32);
			SimdStoreY(y2, dst, 40);
			SimdStoreZ(y2, dst, 48);
			SimdStoreW(y2, dst, 56);
		}

	#else

		float		lumBlock1[8][8];
		float		lumBlock2[8][8];

		const float (& transform)[8][8] = blockInverseTransformMatrix;

		for (machine j = 0; j < 8; j++)
		{
			const int16 *lum = input + j * 8;
			for (machine i = 0; i < 8; i++) lumBlock1[j][i] = (float) lum[i] * dequantizeMatrix[j][i];
		}

		for (machine j = 0; j < 8; j++)
		{
			const float *lum = lumBlock1[j];
			for (machine i = 0; i < 8; i++)
			{
				float y = lum[0] * transform[0][i];
				for (machine k = 1; k < 8; k++) y += lum[k] * transform[k][i];
				lumBlock2[j][i] = y;
			}
		}

		for (machine i = 0; i < 8; i++)
		{
			const float *lum = lumBlock2[0] + i;
			for (machine j = 0; j < 8; j++)
			{
				float y = lum[0] * transform[0][j];
				for (machine k = 1; k < 8; k++) y += lum[k * 8] * transform[k][j];
				output[j][i] = y;
			}
		}

	#endif
}

void VideoDecompressor::StoreLuminanceBlock(const float (& input)[8][8], Color1C *restrict output, int32 rowLength)
{
	#if C4SIMD

		vec_float offset = SimdLoadConstant<0x43008000>();

		for (machine j = 0; j < 8; j++)
		{
			const vec_float *lum = reinterpret_cast<const vec_float *>(input[j]);
			vec_unsigned_int8 y1 = SimdInt16PackUnsignedSaturate(SimdInt32PackSaturate(SimdConvertInt32(SimdAdd(lum[0], offset))));
			vec_unsigned_int8 y2 = SimdInt16PackUnsignedSaturate(SimdInt32PackSaturate(SimdConvertInt32(SimdAdd(lum[1], offset))));

			SimdInt32StoreX((vec_int32) y1, reinterpret_cast<int32 *>(output), 0);
			SimdInt32StoreX((vec_int32) y2, reinterpret_cast<int32 *>(output), 1);

			output += rowLength;
		}

	#else

		for (machine j = 0; j < 8; j++)
		{
			const float *lum = input[j];
			for (machine i = 0; i < 8; i++)
			{
				output[i] = (Color1C) Min(MaxZero((int32) (lum[i] + 128.5F)), 255);
			}

			output += rowLength;
		}

	#endif
}

void VideoDecompressor::StoreDeltaLuminanceBlock(const float (& input)[8][8], const Color1C *base, Color1C *restrict output, int32 rowLength)
{
	#if C4SIMD

		vec_float offset = SimdLoadConstant<0x3F000000>();

		for (machine j = 0; j < 8; j++)
		{
			vec_unsigned_int16 baseLum = SimdUnsignedInt8UnpackA(SimdUnsignedInt8LoadUnaligned(base));
			vec_int32 b1 = (vec_int32) SimdUnsignedInt16UnpackA(baseLum);
			vec_int32 b2 = (vec_int32) SimdUnsignedInt16UnpackB(baseLum);

			const vec_float *lum = reinterpret_cast<const vec_float *>(input[j]);
			vec_unsigned_int8 y1 = SimdInt16PackUnsignedSaturate(SimdInt32PackSaturate(SimdInt32Add(SimdConvertInt32(SimdAdd(lum[0], offset)), b1)));
			vec_unsigned_int8 y2 = SimdInt16PackUnsignedSaturate(SimdInt32PackSaturate(SimdInt32Add(SimdConvertInt32(SimdAdd(lum[1], offset)), b2)));

			SimdInt32StoreX((vec_int32) y1, reinterpret_cast<int32 *>(output), 0);
			SimdInt32StoreX((vec_int32) y2, reinterpret_cast<int32 *>(output), 1);

			base += rowLength;
			output += rowLength;
		}

	#else

		for (machine j = 0; j < 8; j++)
		{
			const float *lum = input[j];
			for (machine i = 0; i < 8; i++)
			{
				output[i] = (Color1C) Min(MaxZero((int32) (lum[i] + 0.5F) + base[i]), 255);
			}

			base += rowLength;
			output += rowLength;
		}

	#endif
}

void VideoDecompressor::DequantizeInverseTransformChrominanceBlock(const int16 *blueInput, const int16 *redInput, float (*restrict blueOutput)[8], float (*restrict redOutput)[8], const float (& dequantizeMatrix)[8][8])
{
	#if C4SIMD

		float alignas(16)	blueBlock[8][8];
		float alignas(16)	redBlock[8][8];

		const vec_float *transform = reinterpret_cast<const vec_float *>(blockInverseTransformMatrix);
		const vec_float *dequantize = reinterpret_cast<const vec_float *>(dequantizeMatrix);

		for (machine j = 0; j < 8; j++)
		{
			vec_int16 blue = *reinterpret_cast<const vec_int16 *>(blueInput);
			vec_int16 red = *reinterpret_cast<const vec_int16 *>(redInput);
			vec_float u1 = SimdMul(SimdInt32ConvertFloat(SimdInt16UnpackA(blue)), dequantize[0]);
			vec_float u2 = SimdMul(SimdInt32ConvertFloat(SimdInt16UnpackB(blue)), dequantize[1]);
			vec_float v1 = SimdMul(SimdInt32ConvertFloat(SimdInt16UnpackA(red)), dequantize[0]);
			vec_float v2 = SimdMul(SimdInt32ConvertFloat(SimdInt16UnpackB(red)), dequantize[1]);

			blueInput += 8;
			redInput += 8;
			dequantize += 2;

			vec_float p = SimdSmearX(u1);
			vec_float q = SimdSmearX(v1);
			vec_float b1 = SimdMul(p, transform[0]);
			vec_float b2 = SimdMul(p, transform[1]);
			vec_float r1 = SimdMul(q, transform[0]);
			vec_float r2 = SimdMul(q, transform[1]);

			p = SimdSmearY(u1);
			q = SimdSmearY(v1);
			b1 = SimdMadd(p, transform[2], b1);
			b2 = SimdMadd(p, transform[3], b2);
			r1 = SimdMadd(q, transform[2], r1);
			r2 = SimdMadd(q, transform[3], r2);

			p = SimdSmearZ(u1);
			q = SimdSmearZ(v1);
			b1 = SimdMadd(p, transform[4], b1);
			b2 = SimdMadd(p, transform[5], b2);
			r1 = SimdMadd(q, transform[4], r1);
			r2 = SimdMadd(q, transform[5], r2);

			p = SimdSmearW(u1);
			q = SimdSmearW(v1);
			b1 = SimdMadd(p, transform[6], b1);
			b2 = SimdMadd(p, transform[7], b2);
			r1 = SimdMadd(q, transform[6], r1);
			r2 = SimdMadd(q, transform[7], r2);

			p = SimdSmearX(u2);
			q = SimdSmearX(v2);
			b1 = SimdMadd(p, transform[8], b1);
			b2 = SimdMadd(p, transform[9], b2);
			r1 = SimdMadd(q, transform[8], r1);
			r2 = SimdMadd(q, transform[9], r2);

			p = SimdSmearY(u2);
			q = SimdSmearY(v2);
			b1 = SimdMadd(p, transform[10], b1);
			b2 = SimdMadd(p, transform[11], b2);
			r1 = SimdMadd(q, transform[10], r1);
			r2 = SimdMadd(q, transform[11], r2);

			p = SimdSmearZ(u2);
			q = SimdSmearZ(v2);
			b1 = SimdMadd(p, transform[12], b1);
			b2 = SimdMadd(p, transform[13], b2);
			r1 = SimdMadd(q, transform[12], r1);
			r2 = SimdMadd(q, transform[13], r2);

			p = SimdSmearW(u2);
			q = SimdSmearW(v2);
			b1 = SimdMadd(p, transform[14], b1);
			b2 = SimdMadd(p, transform[15], b2);
			r1 = SimdMadd(q, transform[14], r1);
			r2 = SimdMadd(q, transform[15], r2);

			reinterpret_cast<vec_float *>(blueBlock[j])[0] = b1;
			reinterpret_cast<vec_float *>(blueBlock[j])[1] = b2;
			reinterpret_cast<vec_float *>(redBlock[j])[0] = r1;
			reinterpret_cast<vec_float *>(redBlock[j])[1] = r2;
		}

		for (machine i = 0; i < 8; i++)
		{
			const float *blue = blueBlock[0] + i;
			const float *red = redBlock[0] + i;

			vec_float p = SimdLoadSmearScalar(blue, 0);
			vec_float q = SimdLoadSmearScalar(red, 0);
			vec_float b1 = SimdMul(p, transform[0]);
			vec_float b2 = SimdMul(p, transform[1]);
			vec_float r1 = SimdMul(q, transform[0]);
			vec_float r2 = SimdMul(q, transform[1]);

			p = SimdLoadSmearScalar(blue, 8);
			q = SimdLoadSmearScalar(red, 8);
			b1 = SimdMadd(p, transform[2], b1);
			b2 = SimdMadd(p, transform[3], b2);
			r1 = SimdMadd(q, transform[2], r1);
			r2 = SimdMadd(q, transform[3], r2);

			p = SimdLoadSmearScalar(blue, 16);
			q = SimdLoadSmearScalar(red, 16);
			b1 = SimdMadd(p, transform[4], b1);
			b2 = SimdMadd(p, transform[5], b2);
			r1 = SimdMadd(q, transform[4], r1);
			r2 = SimdMadd(q, transform[5], r2);

			p = SimdLoadSmearScalar(blue, 24);
			q = SimdLoadSmearScalar(red, 24);
			b1 = SimdMadd(p, transform[6], b1);
			b2 = SimdMadd(p, transform[7], b2);
			r1 = SimdMadd(q, transform[6], r1);
			r2 = SimdMadd(q, transform[7], r2);

			p = SimdLoadSmearScalar(blue, 32);
			q = SimdLoadSmearScalar(red, 32);
			b1 = SimdMadd(p, transform[8], b1);
			b2 = SimdMadd(p, transform[9], b2);
			r1 = SimdMadd(q, transform[8], r1);
			r2 = SimdMadd(q, transform[9], r2);

			p = SimdLoadSmearScalar(blue, 40);
			q = SimdLoadSmearScalar(red, 40);
			b1 = SimdMadd(p, transform[10], b1);
			b2 = SimdMadd(p, transform[11], b2);
			r1 = SimdMadd(q, transform[10], r1);
			r2 = SimdMadd(q, transform[11], r2);

			p = SimdLoadSmearScalar(blue, 48);
			q = SimdLoadSmearScalar(red, 48);
			b1 = SimdMadd(p, transform[12], b1);
			b2 = SimdMadd(p, transform[13], b2);
			r1 = SimdMadd(q, transform[12], r1);
			r2 = SimdMadd(q, transform[13], r2);

			p = SimdLoadSmearScalar(blue, 56);
			q = SimdLoadSmearScalar(red, 56);
			b1 = SimdMadd(p, transform[14], b1);
			b2 = SimdMadd(p, transform[15], b2);
			r1 = SimdMadd(q, transform[14], r1);
			r2 = SimdMadd(q, transform[15], r2);

			float *dst = &blueOutput[0][i];
			SimdStoreX(b1, dst, 0);
			SimdStoreY(b1, dst, 8);
			SimdStoreZ(b1, dst, 16);
			SimdStoreW(b1, dst, 24);
			SimdStoreX(b2, dst, 32);
			SimdStoreY(b2, dst, 40);
			SimdStoreZ(b2, dst, 48);
			SimdStoreW(b2, dst, 56);

			dst = &redOutput[0][i];
			SimdStoreX(r1, dst, 0);
			SimdStoreY(r1, dst, 8);
			SimdStoreZ(r1, dst, 16);
			SimdStoreW(r1, dst, 24);
			SimdStoreX(r2, dst, 32);
			SimdStoreY(r2, dst, 40);
			SimdStoreZ(r2, dst, 48);
			SimdStoreW(r2, dst, 56);
		}

	#else

		float		blueBlock1[8][8];
		float		blueBlock2[8][8];
		float		redBlock1[8][8];
		float		redBlock2[8][8];

		const float (& transform)[8][8] = blockInverseTransformMatrix;

		for (machine j = 0; j < 8; j++)
		{
			const int16 *blue = blueInput + j * 8;
			const int16 *red = redInput + j * 8;

			for (machine i = 0; i < 8; i++)
			{
				blueBlock1[j][i] = (float) blue[i] * dequantizeMatrix[j][i];
				redBlock1[j][i] = (float) red[i] * dequantizeMatrix[j][i];
			}
		}

		for (machine j = 0; j < 8; j++)
		{
			const float *blue = blueBlock1[j];
			const float *red = redBlock1[j];

			for (machine i = 0; i < 8; i++)
			{
				float b = blue[0] * transform[0][i];
				float r = red[0] * transform[0][i];

				for (machine k = 1; k < 8; k++)
				{
					b += blue[k] * transform[k][i];
					r += red[k] * transform[k][i];
				}

				blueBlock2[j][i] = b;
				redBlock2[j][i] = r;
			}
		}

		for (machine i = 0; i < 8; i++)
		{
			const float *blue = blueBlock2[0] + i;
			const float *red = redBlock2[0] + i;

			for (machine j = 0; j < 8; j++)
			{
				float b = blue[0] * transform[0][j];
				float r = red[0] * transform[0][j];

				for (machine k = 1; k < 8; k++)
				{
					b += blue[k * 8] * transform[k][j];
					r += red[k * 8] * transform[k][j];
				}

				blueOutput[j][i] = b;
				redOutput[j][i] = r;
			}
		}

	#endif
}

void VideoDecompressor::StoreChrominanceBlock(const float (& blueInput)[8][8], const float (& redInput)[8][8], Color2C *restrict output, int32 rowLength)
{
	#if C4SIMD

		vec_float offset = SimdLoadConstant<0x43008000>();

		for (machine j = 0; j < 8; j++)
		{
			const vec_float *blue = reinterpret_cast<const vec_float *>(blueInput[j]);
			vec_unsigned_int8 b1 = SimdInt16PackUnsignedSaturate(SimdInt32PackSaturate(SimdConvertInt32(SimdAdd(blue[0], offset))));
			vec_unsigned_int8 b2 = SimdInt16PackUnsignedSaturate(SimdInt32PackSaturate(SimdConvertInt32(SimdAdd(blue[1], offset))));

			const vec_float *red = reinterpret_cast<const vec_float *>(redInput[j]);
			vec_unsigned_int8 r1 = SimdInt16PackUnsignedSaturate(SimdInt32PackSaturate(SimdConvertInt32(SimdAdd(red[0], offset))));
			vec_unsigned_int8 r2 = SimdInt16PackUnsignedSaturate(SimdInt32PackSaturate(SimdConvertInt32(SimdAdd(red[1], offset))));

			vec_unsigned_int8 m1 = SimdUnsignedInt8MergeA(b1, r1);
			vec_unsigned_int8 m2 = SimdUnsignedInt8MergeA(b2, r2);

			SimdInt32StoreX((vec_int32) m1, reinterpret_cast<int32 *>(output), 0);
			SimdInt32StoreY((vec_int32) m1, reinterpret_cast<int32 *>(output), 1);
			SimdInt32StoreX((vec_int32) m2, reinterpret_cast<int32 *>(output), 2);
			SimdInt32StoreY((vec_int32) m2, reinterpret_cast<int32 *>(output), 3);

			output += rowLength;
		}

	#else

		for (machine j = 0; j < 8; j++)
		{
			const float *blue = blueInput[j];
			const float *red = redInput[j];

			for (machine i = 0; i < 8; i++)
			{
				output[i].Set(Min(MaxZero((int32) (blue[i] + 128.5F)), 255), Min(MaxZero((int32) (red[i] + 128.5F)), 255));
			}

			output += rowLength;
		}

	#endif
}

void VideoDecompressor::StoreDeltaChrominanceBlock(const float (& blueInput)[8][8], const float (& redInput)[8][8], const Color2C *base, Color2C *restrict output, int32 rowLength)
{
	#if C4SIMD

		vec_float offset = SimdLoadConstant<0x3F000000>();

		for (machine j = 0; j < 8; j++)
		{
			vec_unsigned_int8 baseChrom = *reinterpret_cast<const vec_unsigned_int8 *>(base);
			vec_int16 c1 = SimdInt16Deinterleave((vec_int16) SimdUnsignedInt8UnpackA(baseChrom));
			vec_int16 c2 = SimdInt16Deinterleave((vec_int16) SimdUnsignedInt8UnpackB(baseChrom));

			const vec_float *blue = reinterpret_cast<const vec_float *>(blueInput[j]);
			vec_unsigned_int8 b1 = SimdInt16PackUnsignedSaturate(SimdInt32PackSaturate(SimdInt32Add(SimdConvertInt32(SimdAdd(blue[0], offset)), SimdInt16UnpackA(c1))));
			vec_unsigned_int8 b2 = SimdInt16PackUnsignedSaturate(SimdInt32PackSaturate(SimdInt32Add(SimdConvertInt32(SimdAdd(blue[1], offset)), SimdInt16UnpackA(c2))));

			const vec_float *red = reinterpret_cast<const vec_float *>(redInput[j]);
			vec_unsigned_int8 r1 = SimdInt16PackUnsignedSaturate(SimdInt32PackSaturate(SimdInt32Add(SimdConvertInt32(SimdAdd(red[0], offset)), SimdInt16UnpackB(c1))));
			vec_unsigned_int8 r2 = SimdInt16PackUnsignedSaturate(SimdInt32PackSaturate(SimdInt32Add(SimdConvertInt32(SimdAdd(red[1], offset)), SimdInt16UnpackB(c2))));

			vec_unsigned_int8 m1 = SimdUnsignedInt8MergeA(b1, r1);
			vec_unsigned_int8 m2 = SimdUnsignedInt8MergeA(b2, r2);

			SimdInt32StoreX((vec_int32) m1, reinterpret_cast<int32 *>(output), 0);
			SimdInt32StoreY((vec_int32) m1, reinterpret_cast<int32 *>(output), 1);
			SimdInt32StoreX((vec_int32) m2, reinterpret_cast<int32 *>(output), 2);
			SimdInt32StoreY((vec_int32) m2, reinterpret_cast<int32 *>(output), 3);

			base += rowLength;
			output += rowLength;
		}

	#else

		for (machine j = 0; j < 8; j++)
		{
			const float *blue = blueInput[j];
			const float *red = redInput[j];

			for (machine i = 0; i < 8; i++)
			{
				output[i].Set(Min(MaxZero((int32) (blue[i] + 0.5F) + base[i].GetLum()), 255), Min(MaxZero((int32) (red[i] + 0.5F) + base[i].GetAlpha()), 255));
			}

			base += rowLength;
			output += rowLength;
		}

	#endif
}

unsigned_int32 VideoDecompressor::DecodeFlatRow(unsigned_int32 count, const unsigned_int8 *code, int16 *restrict data)
{
	int32 previous = ReadLittleEndianS16(reinterpret_cast<const int16 *>(code));
	int32 flags = (previous & 0xF000);
	previous = previous << 20 >> 20;
	data[0] = (int16) previous;

	if (flags == 0)
	{
		const int8 *delta = reinterpret_cast<const int8 *>(code + 1);
		for (unsigned_machine a = 1; a < count; a++)
		{
			int32 value = previous + delta[a];
			data[a] = (int16) value;
			previous = value;
		}

		return ((count + 2) & ~1);
	}

	const int16 *delta = reinterpret_cast<const int16 *>(code + 2);
	for (unsigned_machine a = 1; a < count; a++)
	{
		int32 value = previous + ReadLittleEndianS16(delta);
		data[a] = (int16) value;
		previous = value;
		delta++;
	}

	return (count * 2);
}

unsigned_int32 VideoDecompressor::DecodeWavyBlock(unsigned_int32 length, const unsigned_int8 *code, int16 *restrict data)
{
	#if C4SIMD

		vec_int32 zero = SimdInt32GetZero();
		for (machine a = 0; a < 8; a++) reinterpret_cast<vec_int32 *>(data)[a] = zero;

	#else

		for (machine a = 1; a < 64; a++) data[a] = 0;

	#endif

	unsigned_int32 type = length >> 6;
	machine count = length & 0x3F;

	if (type == 0)
	{
		machine k = count & ~3;
		for (machine a = 1; a <= k; a += 4)
		{
			int32 value = *code++;
			data[blockLinearizeTable[a]] = (int16) (value << 30 >> 30);
			data[blockLinearizeTable[a + 1]] = (int16) (value << 28 >> 30);
			data[blockLinearizeTable[a + 2]] = (int16) (value << 26 >> 30);
			data[blockLinearizeTable[a + 3]] = (int16) (value << 24 >> 30);
		}

		if (count & 3)
		{
			int32 value = *code;
			for (machine a = k + 1; a <= count; a++)
			{
				data[blockLinearizeTable[a]] = (int16) (value << 30 >> 30);
				value >>= 2;
			}
		}

		return ((unsigned_int32) ((count + 3) >> 2));
	}

	if (type == 1)
	{
		machine k = count & ~1;
		for (machine a = 1; a <= k; a += 2)
		{
			int32 value = *code++;
			data[blockLinearizeTable[a]] = (int16) (value << 28 >> 28);
			data[blockLinearizeTable[a + 1]] = (int16) (value << 24 >> 28);
		}

		if (count & 1)
		{
			int32 value = *code;
			data[blockLinearizeTable[count]] = (int16) (value << 28 >> 28);
		}

		return ((unsigned_int32) ((count + 1) >> 1));
	}

	if (type == 2)
	{
		for (machine a = 1; a <= count; a++)
		{
			data[blockLinearizeTable[a]] = (int8) *code++;
		}

		return ((unsigned_int32) count);
	}

	machine k = count & ~1;
	for (machine a = 1; a <= k; a += 2)
	{
		int32 c = code[2];
		data[blockLinearizeTable[a]] = (int16) (code[0] | (c << 28 >> 20));
		data[blockLinearizeTable[a + 1]] = (int16) (code[1] | ((c << 24 >> 20) & ~0xFF));
		code += 3;
	}

	if (count & 1)
	{
		data[blockLinearizeTable[count]] = (int16) (code[0] | (code[1] << 8));
	}

	return ((unsigned_int32) ((count * 3 + 1) >> 1));
}

unsigned_int32 VideoDecompressor::DecodeWavyRow(unsigned_int32 count, const unsigned_int8 *length, const unsigned_int8 *code, int16 *restrict data)
{
	const unsigned_int8 *start = code;

	for (unsigned_machine a = 0; a < count; a++)
	{
		code += DecodeWavyBlock(length[a], code, data);
		data += 64;
	}

	return ((unsigned_int32) (code - start));
}

void VideoDecompressor::ClearLuminance(Color1C *restrict image, int32 rowLength, const Rect& bounds)
{
	#if C4SIMD

		int32 left = bounds.left & ~15;
		int32 right = (bounds.right + 15) & ~15;

		int32 width = right - left;
		int32 height = bounds.Height();

		if (Min(width, height) > 0)
		{
			vec_int8 *restrict lumin = reinterpret_cast<vec_int8 *>(image + (bounds.top * rowLength + left));
			vec_int8 zero = SimdInt8GetZero();

			rowLength >>= 4;
			width >>= 4;

			machine j = 0;
			do
			{
				machine i = 0;
				do
				{
					lumin[i] = zero;
				} while (++i < width);

				lumin += rowLength;
			} while (++j < height);
		}

	#else

		int32 width = bounds.Width();
		int32 height = bounds.Height();

		if (Min(width, height) > 0)
		{
			image += bounds.top * rowLength + bounds.left;

			machine j = 0;
			do
			{
				machine i = 0;
				do
				{
					image[i] = 0;
				} while (++i < width);

				image += rowLength;
			} while (++j < height);
		}

	#endif
}

void VideoDecompressor::ClearChrominance(Color2C *restrict image, int32 rowLength, const Rect& bounds)
{
	int32 width = bounds.Width();
	int32 height = bounds.Height();

	if (Min(width, height) > 0)
	{
		#if C4SIMD

			vec_int8 *restrict chrom = reinterpret_cast<vec_int8 *>(image + (bounds.top * rowLength + bounds.left));
			vec_int8 infinity = SimdInt8GetInfinity();

			rowLength >>= 3;
			width >>= 3;

			machine j = 0;
			do
			{
				machine i = 0;
				do
				{
					chrom[i] = infinity;
				} while (++i < width);

				chrom += rowLength;
			} while (++j < height);

		#else

			image += bounds.top * rowLength + bounds.left;

			machine j = 0;
			do
			{
				machine i = 0;
				do
				{
					image[i].SetPackedColor(0x8080);
				} while (++i < width);

				image += rowLength;
			} while (++j < height);

		#endif
	}
}

void VideoDecompressor::CopyLuminance(const Color1C *base, Color1C *restrict image, int32 rowLength, const Rect& bounds)
{
	#if C4SIMD

		int32 left = bounds.left & ~15;
		int32 right = (bounds.right + 15) & ~15;

		int32 width = right - left;
		int32 height = bounds.Height();

		if (Min(width, height) > 0)
		{
			int32 offset = bounds.top * rowLength + left;
			const vec_int8 *src = reinterpret_cast<const vec_int8 *>(base + offset);
			vec_int8 *restrict dst = reinterpret_cast<vec_int8 *>(image + offset);

			rowLength >>= 4;
			width >>= 4;

			machine j = 0;
			do
			{
				machine i = 0;
				do
				{
					dst[i] = src[i];
				} while (++i < width);

				src += rowLength;
				dst += rowLength;
			} while (++j < height);
		}

	#else

		int32 width = bounds.Width();
		int32 height = bounds.Height();

		if (Min(width, height) > 0)
		{
			int32 offset = bounds.top * rowLength + bounds.left;
			base += offset;
			image += offset;

			machine j = 0;
			do
			{
				machine i = 0;
				do
				{
					image[i] = base[i];
				} while (++i < width);

				base += rowLength;
				image += rowLength;
			} while (++j < height);
		}

	#endif
}

void VideoDecompressor::CopyChrominance(const Color2C *base, Color2C *restrict image, int32 rowLength, const Rect& bounds)
{
	int32 width = bounds.Width();
	int32 height = bounds.Height();

	if (Min(width, height) > 0)
	{
		int32 offset = bounds.top * rowLength + bounds.left;

		#if C4SIMD

			const vec_int8 *src = reinterpret_cast<const vec_int8 *>(base + offset);
			vec_int8 *restrict dst = reinterpret_cast<vec_int8 *>(image + offset);

			rowLength >>= 3;
			width >>= 3;

			machine j = 0;
			do
			{
				machine i = 0;
				do
				{
					dst[i] = src[i];
				} while (++i < width);

				src += rowLength;
				dst += rowLength;
			} while (++j < height);

		#else

			base += offset;
			image += offset;

			machine j = 0;
			do
			{
				machine i = 0;
				do
				{
					image[i] = base[i];
				} while (++i < width);

				base += rowLength;
				image += rowLength;
			} while (++j < height);

		#endif
	}
}

void VideoDecompressor::DecompressLuminance(bool delta)
{
	float alignas(16)	dequantizeMatrix[8][8];
	float alignas(16)	lumBlock[8][8];
	const Color1C		*base;

	const MovieVideoChannelData *luminanceData = &videoFrameHeader->luminanceData;

	const unsigned_int8 *flatCode = videoFrameData;
	const unsigned_int8 *wlenCode = flatCode + luminanceData->flatDataSize;
	const unsigned_int8 *wavyCode = wlenCode + luminanceData->wlenDataSize;

	unsigned_int32 flags = luminanceData->videoChannelFlags;

	if ((flags & 0x000F) == kMovieCompressionGeneral) Comp::DecompressData(luminanceData->GetFlatCode(), luminanceData->flatCodeSize, const_cast<unsigned_int8 *>(flatCode));
	else flatCode = luminanceData->GetFlatCode();

	if (((flags >> 4) & 0x000F) == kMovieCompressionGeneral) Comp::DecompressData(luminanceData->GetWlenCode(), luminanceData->wlenCodeSize, const_cast<unsigned_int8 *>(wlenCode));
	else wlenCode = luminanceData->GetWlenCode();

	if (((flags >> 8) & 0x000F) == kMovieCompressionGeneral) Comp::DecompressData(luminanceData->GetWavyCode(), luminanceData->wavyCodeSize, const_cast<unsigned_int8 *>(wavyCode));
	else wavyCode = luminanceData->GetWavyCode();

	int32 width = videoFrameSize.x;
	int32 height = videoFrameSize.y;

	Color1C *restrict image = luminanceImage[delta];
	const Rect& bounds = luminanceData->channelBoundingRect;
	int32 offset = bounds.top * width + bounds.left;

	if (delta)
	{
		base = luminanceImage[0];
		CopyLuminance(base, image, width, Rect(0, 0, width, bounds.top));
		CopyLuminance(base, image, width, Rect(0, bounds.top, bounds.left, bounds.bottom));
		CopyLuminance(base, image, width, Rect(bounds.right, bounds.top, width, bounds.bottom));
		CopyLuminance(base, image, width, Rect(0, bounds.top, width, height));
		base += offset;
	}
	else
	{
		ClearLuminance(image, width, Rect(0, 0, width, bounds.top));
		ClearLuminance(image, width, Rect(0, bounds.top, bounds.left, bounds.bottom));
		ClearLuminance(image, width, Rect(bounds.right, bounds.top, width, bounds.bottom));
		ClearLuminance(image, width, Rect(0, bounds.top, width, height));
	}

	image += offset;

	float scale = luminanceData->quantizationScale;
	for (machine j = 0; j < 8; j++)
	{
		for (machine i = 0; i < 8; i++) dequantizeMatrix[j][i] = luminanceQuantizeMatrix[j][i] * scale;
	}

	int32 blockCountX = bounds.Width() >> 3;
	int32 blockCountY = bounds.Height() >> 3;

	int16 *flatData = blockData + blockCountX * 64;

	if (delta)
	{
		for (machine j = 0; j < blockCountY; j++)
		{
			flatCode += DecodeFlatRow(blockCountX, flatCode, flatData);
			wavyCode += DecodeWavyRow(blockCountX, wlenCode, wavyCode, blockData);
			wlenCode += blockCountX;

			for (machine i = 0; i < blockCountX; i++)
			{
				blockData[i * 64] = flatData[i];
				DequantizeInverseTransformLuminanceBlock(blockData + i * 64, lumBlock, dequantizeMatrix);
				StoreDeltaLuminanceBlock(lumBlock, base + i * 8, image + i * 8, width);
			}

			base += width * 8;
			image += width * 8;
		}
	}
	else
	{
		for (machine j = 0; j < blockCountY; j++)
		{
			flatCode += DecodeFlatRow(blockCountX, flatCode, flatData);
			wavyCode += DecodeWavyRow(blockCountX, wlenCode, wavyCode, blockData);
			wlenCode += blockCountX;

			for (machine i = 0; i < blockCountX; i++)
			{
				blockData[i * 64] = flatData[i];
				DequantizeInverseTransformLuminanceBlock(blockData + i * 64, lumBlock, dequantizeMatrix);
				StoreLuminanceBlock(lumBlock, image + i * 8, width);
			}

			image += width * 8;
		}
	}
}

void VideoDecompressor::DecompressChrominance(bool delta)
{
	float alignas(16)	dequantizeMatrix[8][8];
	float alignas(16)	blueBlock[8][8];
	float alignas(16)	redBlock[8][8];
	const Color2C		*base;

	const MovieVideoChannelData *chrominanceData = &videoFrameHeader->chrominanceData;

	const unsigned_int8 *flatCode = videoFrameData;
	const unsigned_int8 *wlenCode = flatCode + chrominanceData->flatDataSize;
	const unsigned_int8 *wavyCode = wlenCode + chrominanceData->wlenDataSize;

	unsigned_int32 flags = chrominanceData->videoChannelFlags;

	if ((flags & 0x000F) == kMovieCompressionGeneral) Comp::DecompressData(chrominanceData->GetFlatCode(), chrominanceData->flatCodeSize, const_cast<unsigned_int8 *>(flatCode));
	else flatCode = chrominanceData->GetFlatCode();

	if (((flags >> 4) & 0x000F) == kMovieCompressionGeneral) Comp::DecompressData(chrominanceData->GetWlenCode(), chrominanceData->wlenCodeSize, const_cast<unsigned_int8 *>(wlenCode));
	else wlenCode = chrominanceData->GetWlenCode();

	if (((flags >> 8) & 0x000F) == kMovieCompressionGeneral) Comp::DecompressData(chrominanceData->GetWavyCode(), chrominanceData->wavyCodeSize, const_cast<unsigned_int8 *>(wavyCode));
	else wavyCode = chrominanceData->GetWavyCode();

	int32 width = videoFrameSize.x >> 1;
	int32 height = videoFrameSize.y >> 1;

	Color2C *restrict image = chrominanceImage[delta];
	const Rect& bounds = chrominanceData->channelBoundingRect;
	int32 offset = bounds.top * width + bounds.left;

	if (delta)
	{
		base = chrominanceImage[0];
		CopyChrominance(base, image, width, Rect(0, 0, width, bounds.top));
		CopyChrominance(base, image, width, Rect(0, bounds.top, bounds.left, bounds.bottom));
		CopyChrominance(base, image, width, Rect(bounds.right, bounds.top, width, bounds.bottom));
		CopyChrominance(base, image, width, Rect(0, bounds.top, width, height));
		base += offset;
	}
	else
	{
		ClearChrominance(image, width, Rect(0, 0, width, bounds.top));
		ClearChrominance(image, width, Rect(0, bounds.top, bounds.left, bounds.bottom));
		ClearChrominance(image, width, Rect(bounds.right, bounds.top, width, bounds.bottom));
		ClearChrominance(image, width, Rect(0, bounds.top, width, height));
	}

	image += offset;

	float scale = chrominanceData->quantizationScale;
	for (machine j = 0; j < 8; j++)
	{
		for (machine i = 0; i < 8; i++) dequantizeMatrix[j][i] = chrominanceQuantizeMatrix[j][i] * scale;
	}

	int32 blockCountX = bounds.Width() >> 3;
	int32 blockCountY = bounds.Height() >> 3;

	int16 *blueBlockData = blockData;
	int16 *redBlockData = blueBlockData + blockCountX * 64;
	int16 *blueFlatData = redBlockData + blockCountX * 64;
	int16 *redFlatData = blueFlatData + blockCountX;

	if (delta)
	{
		for (machine j = 0; j < blockCountY; j++)
		{
			flatCode += DecodeFlatRow(blockCountX, flatCode, blueFlatData);
			flatCode += DecodeFlatRow(blockCountX, flatCode, redFlatData);

			wavyCode += DecodeWavyRow(blockCountX, wlenCode, wavyCode, blueBlockData);
			wlenCode += blockCountX;

			wavyCode += DecodeWavyRow(blockCountX, wlenCode, wavyCode, redBlockData);
			wlenCode += blockCountX;

			for (machine i = 0; i < blockCountX; i++)
			{
				blueBlockData[i * 64] = blueFlatData[i];
				redBlockData[i * 64] = redFlatData[i];
				DequantizeInverseTransformChrominanceBlock(blueBlockData + i * 64, redBlockData + i * 64, blueBlock, redBlock, dequantizeMatrix);
				StoreDeltaChrominanceBlock(blueBlock, redBlock, base + i * 8, image + i * 8, width);
			}

			base += width * 8;
			image += width * 8;
		}
	}
	else
	{
		for (machine j = 0; j < blockCountY; j++)
		{
			flatCode += DecodeFlatRow(blockCountX, flatCode, blueFlatData);
			flatCode += DecodeFlatRow(blockCountX, flatCode, redFlatData);

			wavyCode += DecodeWavyRow(blockCountX, wlenCode, wavyCode, blueBlockData);
			wlenCode += blockCountX;

			wavyCode += DecodeWavyRow(blockCountX, wlenCode, wavyCode, redBlockData);
			wlenCode += blockCountX;

			for (machine i = 0; i < blockCountX; i++)
			{
				blueBlockData[i * 64] = blueFlatData[i];
				redBlockData[i * 64] = redFlatData[i];
				DequantizeInverseTransformChrominanceBlock(blueBlockData + i * 64, redBlockData + i * 64, blueBlock, redBlock, dequantizeMatrix);
				StoreChrominanceBlock(blueBlock, redBlock, image + i * 8, width);
			}

			image += width * 8;
		}
	}
}

void VideoDecompressor::DecompressAlpha(bool delta)
{
	float alignas(16)	dequantizeMatrix[8][8];
	float alignas(16)	alphaBlock[8][8];
	const Color1C		*base;

	const MovieVideoChannelData *alphaData = &static_cast<MovieVideoAlphaFrameHeader *>(videoFrameHeader)->alphaData;

	const unsigned_int8 *flatCode = videoFrameData;
	const unsigned_int8 *wlenCode = flatCode + alphaData->flatDataSize;
	const unsigned_int8 *wavyCode = wlenCode + alphaData->wlenDataSize;

	unsigned_int32 flags = alphaData->videoChannelFlags;

	if ((flags & 0x000F) == kMovieCompressionGeneral) Comp::DecompressData(alphaData->GetFlatCode(), alphaData->flatCodeSize, const_cast<unsigned_int8 *>(flatCode));
	else flatCode = alphaData->GetFlatCode();

	if (((flags >> 4) & 0x000F) == kMovieCompressionGeneral) Comp::DecompressData(alphaData->GetWlenCode(), alphaData->wlenCodeSize, const_cast<unsigned_int8 *>(wlenCode));
	else wlenCode = alphaData->GetWlenCode();

	if (((flags >> 8) & 0x000F) == kMovieCompressionGeneral) Comp::DecompressData(alphaData->GetWavyCode(), alphaData->wavyCodeSize, const_cast<unsigned_int8 *>(wavyCode));
	else wavyCode = alphaData->GetWavyCode();

	int32 width = videoFrameSize.x;
	int32 height = videoFrameSize.y;

	Color1C *restrict image = alphaImage[delta];
	const Rect& bounds = alphaData->channelBoundingRect;
	int32 offset = bounds.top * width + bounds.left;

	if (delta)
	{
		base = alphaImage[0];
		CopyLuminance(base, image, width, Rect(0, 0, width, bounds.top));
		CopyLuminance(base, image, width, Rect(0, bounds.top, bounds.left, bounds.bottom));
		CopyLuminance(base, image, width, Rect(bounds.right, bounds.top, width, bounds.bottom));
		CopyLuminance(base, image, width, Rect(0, bounds.top, width, height));
		base += offset;
	}
	else
	{
		ClearLuminance(image, width, Rect(0, 0, width, bounds.top));
		ClearLuminance(image, width, Rect(0, bounds.top, bounds.left, bounds.bottom));
		ClearLuminance(image, width, Rect(bounds.right, bounds.top, width, bounds.bottom));
		ClearLuminance(image, width, Rect(0, bounds.top, width, height));
	}

	image += offset;

	float scale = alphaData->quantizationScale;
	for (machine j = 0; j < 8; j++)
	{
		for (machine i = 0; i < 8; i++) dequantizeMatrix[j][i] = luminanceQuantizeMatrix[j][i] * scale;
	}

	int32 blockCountX = bounds.Width() >> 3;
	int32 blockCountY = bounds.Height() >> 3;

	int16 *flatData = blockData + blockCountX * 64;

	if (delta)
	{
		for (machine j = 0; j < blockCountY; j++)
		{
			flatCode += DecodeFlatRow(blockCountX, flatCode, flatData);
			wavyCode += DecodeWavyRow(blockCountX, wlenCode, wavyCode, blockData);
			wlenCode += blockCountX;

			for (machine i = 0; i < blockCountX; i++)
			{
				blockData[i * 64] = flatData[i];
				DequantizeInverseTransformLuminanceBlock(blockData + i * 64, alphaBlock, dequantizeMatrix);
				StoreDeltaLuminanceBlock(alphaBlock, base + i * 8, image + i * 8, width);
			}

			base += width * 8;
			image += width * 8;
		}
	}
	else
	{
		for (machine j = 0; j < blockCountY; j++)
		{
			flatCode += DecodeFlatRow(blockCountX, flatCode, flatData);
			wavyCode += DecodeWavyRow(blockCountX, wlenCode, wavyCode, blockData);
			wlenCode += blockCountX;

			for (machine i = 0; i < blockCountX; i++)
			{
				blockData[i * 64] = flatData[i];
				DequantizeInverseTransformLuminanceBlock(blockData + i * 64, alphaBlock, dequantizeMatrix);
				StoreLuminanceBlock(alphaBlock, image + i * 8, width);
			}

			image += width * 8;
		}
	}
}


AudioCompressor::AudioCompressor(const MovieAudioTrackHeader *audioTrackHeader)
{
	channelCount = audioTrackHeader->audioChannelCount;

	int32 frameCount = audioTrackHeader->blockFrameCount;
	unsigned_int32 blockCodeSize = frameCount * sizeof(Sample);

	if (channelCount == 1)
	{
		compressorStorage = new char[blockCodeSize * 3];
		audioChannelCode[0] = reinterpret_cast<unsigned_int8 *>(compressorStorage);
		compressedCode = audioChannelCode[0] + blockCodeSize * 2;
	}
	else
	{
		compressorStorage = new char[blockCodeSize * 14];

		audioChannelData[0] = reinterpret_cast<Sample *>(compressorStorage);
		audioChannelData[1] = audioChannelData[0] + frameCount;
		audioDifferenceData = audioChannelData[1] + frameCount;
		audioAverageData = audioDifferenceData + frameCount;

		audioChannelCode[0] = reinterpret_cast<unsigned_int8 *>(audioAverageData + frameCount);
		audioChannelCode[1] = audioChannelCode[0] + blockCodeSize * 2;
		audioDifferenceCode = audioChannelCode[1] + blockCodeSize * 2;
		audioAverageCode = audioDifferenceCode + blockCodeSize * 2;

		compressedCode = audioAverageCode + blockCodeSize * 2;
	}
}

AudioCompressor::~AudioCompressor()
{
	delete[] compressorStorage;
}

unsigned_int32 AudioCompressor::EncodeDeltaRun(const Sample *sample, int32 count, int32 level, unsigned_int8 *restrict code)
{
	const unsigned_int8 *start = code;

	int32 remain = count;
	do
	{
		count = Min(remain, 4112);
		remain -= count;

		if (count < 17)
		{
			*code++ = (unsigned_int8) ((level << 4) | (count - 1));
		}
		else
		{
			machine k = count - 17;
			code[0] = (unsigned_int8) (0x80 | (level << 4) | (k >> 8));
			code[1] = (unsigned_int8) k;
			code += 2;
		}

		if (level == 0)
		{
			machine k = count & ~3;
			for (machine a = 0; a < k; a += 4)
			{
				int32 sample0 = sample[a - 1];
				int32 sample1 = sample[a];
				int32 sample2 = sample[a + 1];
				int32 sample3 = sample[a + 2];
				int32 sample4 = sample[a + 3];

				unsigned_int32 value = (sample1 - sample0) & 0x03;
				value |= ((sample2 - sample1) << 2) & 0x0C;
				value |= ((sample3 - sample2) << 4) & 0x30;
				value |= ((sample4 - sample3) << 6) & 0xC0;
				*code++ = (unsigned_int8) value;
			}

			if (count & 3)
			{
				unsigned_int32 value = (sample[count - 1] - sample[count - 2]) & 0x03;
				for (machine a = count - 2; a >= k; a--)
				{
					value = (value << 2) | ((sample[a] - sample[a - 1]) & 0x03);
				}

				*code++ = (unsigned_int8) value;
			}
		}
		else if (level == 1)
		{
			machine k = count & ~1;
			for (machine a = 0; a < k; a += 2)
			{
				int32 sample0 = sample[a - 1];
				int32 sample1 = sample[a];
				int32 sample2 = sample[a + 1];

				unsigned_int32 value = (sample1 - sample0) & 0x0F;
				value |= ((sample2 - sample1) << 4) & 0xF0;
				*code++ = (unsigned_int8) value;
			}

			if (count & 1)
			{
				*code++ = (unsigned_int8) (sample[count - 1] - sample[count - 2]);
			}
		}
		else if (level == 2)
		{
			machine k = count & ~3;
			for (machine a = 0; a < k; a += 4)
			{
				int32 sample0 = sample[a - 1];
				int32 sample1 = sample[a];
				int32 sample2 = sample[a + 1];
				int32 sample3 = sample[a + 2];
				int32 sample4 = sample[a + 3];

				unsigned_int32 value1 = ((sample1 - sample0) << 2) & 0xFC;
				value1 |= ((sample2 - sample1) >> 4) & 0x03;
				unsigned_int32 value2 = ((sample2 - sample1) << 4) & 0xF0;
				value2 |= ((sample3 - sample2) >> 2) & 0x0F;
				unsigned_int32 value3 = ((sample3 - sample2) << 6) & 0xC0;
				value3 |= (sample4 - sample3) & 0x3F;

				code[0] = (unsigned_int8) value1;
				code[1] = (unsigned_int8) value2;
				code[2] = (unsigned_int8) value3;
				code += 3;
			}

			if (count & 3)
			{
				for (machine a = k; a < count; a++)
				{
					*code++ = (unsigned_int8) (sample[a] - sample[a - 1]);
				}
			}
		}
		else if (level == 3)
		{
			int32 sample0 = sample[-1];
			for (machine a = 0; a < count; a++)
			{
				int32 sample1 = sample[a];
				*code++ = (unsigned_int8) (sample1 - sample0);
				sample0 = sample1;
			}
		}
		else if (level == 4)
		{
			machine k = count & ~1;
			for (machine a = 0; a < k; a += 2)
			{
				int32 sample0 = sample[a - 1];
				int32 sample1 = sample[a];
				int32 sample2 = sample[a + 1];

				int32 value1 = sample1 - sample0;
				int32 value2 = sample2 - sample1;
				code[0] = (unsigned_int8) value1;
				code[1] = (unsigned_int8) value2;
				code[2] = (unsigned_int8) (((value1 >> 8) & 0x0F) | ((value2 >> 4) & 0xF0));
				code += 3;
			}

			if (count & 1)
			{
				int32 value = sample[count - 1];
				code[0] = (unsigned_int8) value;
				code[1] = (unsigned_int8) (value >> 8);
				code += 2;
			}
		}
		else
		{
			for (machine a = 0; a < count; a++)
			{
				int32 value = sample[a];
				code[0] = (unsigned_int8) value;
				code[1] = (unsigned_int8) (value >> 8);
				code += 2;
			}
		}

		sample += count;
	} while (remain > 0);

	return (code - start);
}

unsigned_int32 AudioCompressor::CompressAudioChannel(const Sample *audio, int32 frameCount, unsigned_int8 *restrict code)
{
	enum
	{
		kLevelCount = 6
	};

	static const int8 minTransitionCount[kLevelCount - 1][kLevelCount] =
	{
		{ 7, 0, 0, 0, 0, 0},
		{10, 6, 0, 0, 0, 0},
		{ 3, 4, 8, 0, 0, 0},
		{ 2, 2, 4, 4, 0, 0},
		{ 2, 2, 2, 2, 3, 0}
	};

	int32 sample1 = audio[0];
	code[0] = (unsigned_int8) sample1;
	code[1] = (unsigned_int8) (sample1 >> 8);
	unsigned_int32 codeSize = 2;

	unsigned_int32 frameStart = 1;
	int32 level = kLevelCount - 1;
	int32 levelCount[kLevelCount] = {0};

	for (machine a = 1; a < frameCount; a++)
	{
		int32 sample2 = audio[a];
		int32 delta = sample2 - sample1;
		sample1 = sample2;

		int32 k = kLevelCount - 1;
		if ((unsigned_int32) (delta + 2) < 4U) k = 0;
		else if ((unsigned_int32) (delta + 8) < 16U) k = 1;
		else if ((unsigned_int32) (delta + 32) < 64U) k = 2;
		else if ((unsigned_int32) (delta + 128) < 256U) k = 3;
		else if ((unsigned_int32) (delta + 2048) < 4096U) k = 4;

		if (k > level)
		{
			if (levelCount[level] >= minTransitionCount[k - 1][level])
			{
				codeSize += EncodeDeltaRun(&audio[frameStart], a - frameStart, level, &code[codeSize]);

				level = k;
				frameStart = (unsigned_int32) a;
				for (machine i = 0; i < k; i++) levelCount[i] = 0;
				for (machine i = k; i < kLevelCount; i++) levelCount[i] = 1;
				continue;
			}

			level = k;
		}
		else if ((k < level) && (levelCount[k] >= minTransitionCount[level - 1][k]))
		{
			codeSize += EncodeDeltaRun(&audio[frameStart], a - frameStart, level, &code[codeSize]);

			level = k;
			frameStart = (unsigned_int32) a;
			for (machine i = 0; i < k; i++) levelCount[i] = 0;
			for (machine i = k; i < kLevelCount; i++) levelCount[i] = 1;
			continue;
		}

		for (machine i = 0; i < k; i++) levelCount[i] = 0;
		for (machine i = k; i < kLevelCount; i++) levelCount[i]++;
	}

	return (codeSize + EncodeDeltaRun(&audio[frameStart], frameCount - frameStart, level, &code[codeSize]));
}

unsigned_int32 AudioCompressor::CompressBlock(const Sample *audio, int32 frameCount, MovieAudioBlockHeader *audioBlockHeader)
{
	if (channelCount == 1)
	{
		unsigned_int32 dataSize = CompressAudioChannel(audio, frameCount, audioChannelCode[0]);

		unsigned_int8 compression = kMovieCompressionGeneral;
		unsigned_int32 codeSize = Comp::CompressData(audioChannelCode[0], dataSize, compressedCode);
		if (codeSize == 0)
		{
			MemoryMgr::CopyMemory(audioChannelCode[0], compressedCode, dataSize);
			compression = kMovieCompressionNone;

			codeSize = (dataSize + 3) & ~3;
			for (unsigned_machine a = dataSize; a < codeSize; a++) compressedCode[a] = 0;
		}

		audioBlockHeader->audioBlockFlags = 0;
		audioBlockHeader->audioChannelData[0].audioChannelFlags = compression;
		audioBlockHeader->audioChannelData[0].audioCodeOffset = sizeof(MovieAudioChannelData);
		audioBlockHeader->audioChannelData[0].audioCodeSize = codeSize;
		audioBlockHeader->audioChannelData[0].audioDataSize = dataSize;

		return (codeSize);
	}

	for (machine a = 0; a < frameCount; a++)
	{
		Sample left = audio[0];
		Sample right = audio[1];
		audioChannelData[0][a] = left;
		audioChannelData[1][a] = right;
		audioDifferenceData[a] = (Sample) (right - left);
		audioAverageData[a] = (Sample) ((left + right) >> 1);
		audio += 2;
	}

	unsigned_int32 leftDataSize = CompressAudioChannel(audioChannelData[0], frameCount, audioChannelCode[0]);
	unsigned_int32 rightDataSize = CompressAudioChannel(audioChannelData[1], frameCount, audioChannelCode[1]);
	unsigned_int32 differenceDataSize = CompressAudioChannel(audioDifferenceData, frameCount, audioDifferenceCode);
	unsigned_int32 averageDataSize = CompressAudioChannel(audioAverageData, frameCount, audioAverageCode);

	unsigned_int32 independentDataSize = leftDataSize + rightDataSize;
	unsigned_int32 leftDifferenceDataSize = leftDataSize + differenceDataSize;
	unsigned_int32 rightDifferenceDataSize = rightDataSize + differenceDataSize;
	unsigned_int32 averageDifferenceDataSize = averageDataSize + differenceDataSize;

	unsigned_int32 audioBlockFlags = kAudioIndependent;
	const unsigned_int8 *channelData1 = audioChannelCode[0];
	const unsigned_int8 *channelData2 = audioChannelCode[1];
	unsigned_int32 dataSize1 = leftDataSize;
	unsigned_int32 dataSize2 = rightDataSize;

	if ((leftDifferenceDataSize < independentDataSize) && (leftDifferenceDataSize <= rightDifferenceDataSize) && (leftDifferenceDataSize <= averageDifferenceDataSize))
	{
		audioBlockFlags = kAudioLeftDifference;

		channelData2 = audioDifferenceCode;
		dataSize2 = differenceDataSize;
	}
	else if ((rightDifferenceDataSize < independentDataSize) && (rightDifferenceDataSize <= averageDifferenceDataSize))
	{
		audioBlockFlags = kAudioRightDifference;

		channelData1 = audioChannelCode[1];
		dataSize1 = rightDataSize;

		channelData2 = audioDifferenceCode;
		dataSize2 = differenceDataSize;
	}
	else if (averageDifferenceDataSize < independentDataSize)
	{
		audioBlockFlags = kAudioAverageDifference;

		channelData1 = audioAverageCode;
		dataSize1 = averageDifferenceDataSize;

		channelData2 = audioDifferenceCode;
		dataSize2 = differenceDataSize;
	}

	unsigned_int8 compression1 = kMovieCompressionGeneral;
	unsigned_int8 *code = compressedCode;

	unsigned_int32 codeSize1 = Comp::CompressData(channelData1, dataSize1, code);
	if (codeSize1 == 0)
	{
		MemoryMgr::CopyMemory(channelData1, code, dataSize1);
		compression1 = kMovieCompressionNone;

		codeSize1 = (dataSize1 + 3) & ~3;
		for (unsigned_machine a = dataSize1; a < codeSize1; a++) code[a] = 0;
	}

	unsigned_int8 compression2 = kMovieCompressionGeneral;
	code += codeSize1;

	unsigned_int32 codeSize2 = Comp::CompressData(channelData2, dataSize2, code);
	if (codeSize2 == 0)
	{
		MemoryMgr::CopyMemory(channelData2, code, dataSize2);
		compression2 = kMovieCompressionNone;

		codeSize2 = (dataSize2 + 3) & ~3;
		for (unsigned_machine a = dataSize2; a < codeSize2; a++) code[a] = 0;
	}

	audioBlockHeader->audioBlockFlags = audioBlockFlags;
	audioBlockHeader->audioChannelData[0].audioChannelFlags = compression1;
	audioBlockHeader->audioChannelData[0].audioCodeOffset = sizeof(MovieAudioChannelData) * 2;
	audioBlockHeader->audioChannelData[0].audioCodeSize = codeSize1;
	audioBlockHeader->audioChannelData[0].audioDataSize = dataSize1;

	audioBlockHeader->audioChannelData[1].audioChannelFlags = compression2;
	audioBlockHeader->audioChannelData[1].audioCodeOffset = sizeof(MovieAudioChannelData) + codeSize1;
	audioBlockHeader->audioChannelData[1].audioCodeSize = codeSize2;
	audioBlockHeader->audioChannelData[1].audioDataSize = dataSize2;

	return (codeSize1 + codeSize2);
}


AudioDecompressor::AudioDecompressor(const MovieAudioTrackHeader *audioTrackHeader)
{
	channelCount = audioTrackHeader->audioChannelCount;

	if (channelCount == 1)
	{
		decompressorStorage = new char[audioTrackHeader->maxBlockCodeSize + audioTrackHeader->maxBlockDataSize];
		audioBlockHeader = reinterpret_cast<MovieAudioBlockHeader *>(decompressorStorage);
		audioBlockData = reinterpret_cast<unsigned_int8 *>(decompressorStorage + audioTrackHeader->maxBlockCodeSize);
	}
	else
	{
		unsigned_int32 maxBlockDataSize = audioTrackHeader->maxBlockDataSize;
		decompressorStorage = new char[audioTrackHeader->maxBlockCodeSize + maxBlockDataSize + audioTrackHeader->blockFrameCount * sizeof(Sample)];

		audioBlockHeader = reinterpret_cast<MovieAudioBlockHeader *>(decompressorStorage);
		audioBlockData = reinterpret_cast<unsigned_int8 *>(decompressorStorage + audioTrackHeader->maxBlockCodeSize);
		audioSampleBuffer = reinterpret_cast<Sample *>(audioBlockData + maxBlockDataSize);
	}
}

AudioDecompressor::~AudioDecompressor()
{
	delete[] decompressorStorage;
}

template <int stride> void AudioDecompressor::DecompressAudioMono(const unsigned_int8 *code, int32 sampleCount, Sample *restrict audio)
{
	Sample sample = code[0] | (reinterpret_cast<const int8 *>(code)[1] << 8);
	code += 2;

	WriteLittleEndianS16(&audio[0], sample);
	audio += stride;
	sampleCount--;

	while (sampleCount > 0)
	{
		machine k = *code++;
		int32 level = (k >> 4) & 0x07;
		int32 count = (k & 0x0F) + 1;
		if (k & 0x80)
		{
			k = *code++;
			count = ((count << 8) | k) - 239;
		}

		switch (level)
		{
			case 0:
			{
				k = count & ~3;
				for (machine a = 0; a < k; a += 4)
				{
					int32 value = *code++;
					int32 delta1 = value << 30 >> 30;
					int32 delta2 = value << 28 >> 30;
					int32 delta3 = value << 26 >> 30;
					int32 delta4 = value << 24 >> 30;

					sample = (Sample) (sample + delta1);
					WriteLittleEndianS16(&audio[0], sample);
					sample = (Sample) (sample + delta2);
					WriteLittleEndianS16(&audio[stride], sample);
					sample = (Sample) (sample + delta3);
					WriteLittleEndianS16(&audio[stride * 2], sample);
					sample = (Sample) (sample + delta4);
					WriteLittleEndianS16(&audio[stride * 3], sample);
					audio += stride * 4;
				}

				if (count & 3)
				{
					int32 value = *code++;
					for (machine a = k; a < count; a++)
					{
						int32 delta = value << 30 >> 30;
						value >>= 2;

						sample = (Sample) (sample + delta);
						WriteLittleEndianS16(&audio[0], sample);
						audio += stride;
					}
				}

				break;
			}

			case 1:
			{
				k = count & ~1;
				for (machine a = 0; a < k; a += 2)
				{
					int32 value = *code++;
					int32 delta1 = value << 28 >> 28;
					int32 delta2 = value << 24 >> 28;

					sample = (Sample) (sample + delta1);
					WriteLittleEndianS16(&audio[0], sample);
					sample = (Sample) (sample + delta2);
					WriteLittleEndianS16(&audio[stride], sample);
					audio += stride * 2;
				}

				if (count & 1)
				{
					int32 delta = *reinterpret_cast<const int8 *>(code++);
					sample = (Sample) (sample + delta);
					WriteLittleEndianS16(&audio[0], sample);
					audio += stride;
				}

				break;
			}

			case 2:
			{
				k = count & ~3;
				for (machine a = 0; a < k; a += 4)
				{
					int32 value1 = code[0];
					int32 value2 = code[1];
					int32 value3 = code[2];
					code += 3;

					int32 delta1 = value1 << 24 >> 26;
					int32 delta2 = ((value1 << 30) | (value2 << 22)) >> 26;
					int32 delta3 = ((value2 << 28) | (value3 << 20)) >> 26;
					int32 delta4 = value3 << 26 >> 26;

					sample = (Sample) (sample + delta1);
					WriteLittleEndianS16(&audio[0], sample);
					sample = (Sample) (sample + delta2);
					WriteLittleEndianS16(&audio[stride], sample);
					sample = (Sample) (sample + delta3);
					WriteLittleEndianS16(&audio[stride * 2], sample);
					sample = (Sample) (sample + delta4);
					WriteLittleEndianS16(&audio[stride * 3], sample);
					audio += stride * 4;
				}

				for (machine a = k; a < count; a++)
				{
					int32 delta = *reinterpret_cast<const int8 *>(code++);
					sample = (Sample) (sample + delta);
					WriteLittleEndianS16(&audio[0], sample);
					audio += stride;
				}

				break;
			}

			case 3:
			{
				for (machine a = 0; a < count; a++)
				{
					int32 delta = reinterpret_cast<const int8 *>(code)[a];
					sample = (Sample) (sample + delta);
					WriteLittleEndianS16(&audio[0], sample);
					audio += stride;
				}

				code += count;
				break;
			}

			case 4:
			{
				k = count & ~1;
				for (machine a = 0; a < k; a += 2)
				{
					int32 value1 = code[0];
					int32 value2 = code[1];
					int32 value3 = code[2];
					code += 3;

					int32 delta1 = value1 | ((value3 << 28 >> 20) & ~0xFF);
					int32 delta2 = value2 | ((value3 << 24 >> 20) & ~0xFF);

					sample = (Sample) (sample + delta1);
					WriteLittleEndianS16(&audio[0], sample);
					sample = (Sample) (sample + delta2);
					WriteLittleEndianS16(&audio[stride], sample);
					audio += stride * 2;
				}

				if (count & 1)
				{
					sample = (Sample) (code[0] | (reinterpret_cast<const int8 *>(code)[1] << 8));
					WriteLittleEndianS16(&audio[0], sample);
					audio += stride;
					code += 2;
				}

				break;
			}

			default:
			{
				for (machine a = 0; a < count; a++)
				{
					sample = (Sample) (code[0] | (reinterpret_cast<const int8 *>(code)[1] << 8));
					WriteLittleEndianS16(&audio[0], sample);
					audio += stride;
					code += 2;
				}

				break;
			}
		}

		sampleCount -= count;
	}
}

template <class combiner> void AudioDecompressor::DecompressAudioStereo(const unsigned_int8 *code, int32 sampleCount, const Sample *channel, Sample *restrict audio)
{
	Sample sample = code[0] | (reinterpret_cast<const int8 *>(code)[1] << 8);
	code += 2;

	combiner::Output(sample, ReadLittleEndianS16(&channel[0]), audio);
	channel++;
	audio += 2;
	sampleCount--;

	while (sampleCount > 0)
	{
		machine k = *code++;
		int32 level = (k >> 4) & 0x07;
		int32 count = (k & 0x0F) + 1;
		if (k & 0x80)
		{
			k = *code++;
			count = ((count << 8) | k) - 239;
		}

		switch (level)
		{
			case 0:
			{
				k = count & ~3;
				for (machine a = 0; a < k; a += 4)
				{
					int32 value = *code++;
					int32 delta1 = value << 30 >> 30;
					int32 delta2 = value << 28 >> 30;
					int32 delta3 = value << 26 >> 30;
					int32 delta4 = value << 24 >> 30;

					sample = (Sample) (sample + delta1);
					combiner::Output(sample, ReadLittleEndianS16(&channel[0]), audio);
					sample = (Sample) (sample + delta2);
					combiner::Output(sample, ReadLittleEndianS16(&channel[1]), audio + 2);
					sample = (Sample) (sample + delta3);
					combiner::Output(sample, ReadLittleEndianS16(&channel[2]), audio + 4);
					sample = (Sample) (sample + delta4);
					combiner::Output(sample, ReadLittleEndianS16(&channel[3]), audio + 6);
					channel += 4;
					audio += 8;
				}

				if (count & 3)
				{
					int32 value = *code++;
					for (machine a = k; a < count; a++)
					{
						int32 delta = value << 30 >> 30;
						value >>= 2;

						sample = (Sample) (sample + delta);
						combiner::Output(sample, ReadLittleEndianS16(&channel[0]), audio);
						channel++;
						audio += 2;
					}
				}

				break;
			}

			case 1:
			{
				k = count & ~1;
				for (machine a = 0; a < k; a += 2)
				{
					int32 value = *code++;
					int32 delta1 = value << 28 >> 28;
					int32 delta2 = value << 24 >> 28;

					sample = (Sample) (sample + delta1);
					combiner::Output(sample, ReadLittleEndianS16(&channel[0]), audio);
					sample = (Sample) (sample + delta2);
					combiner::Output(sample, ReadLittleEndianS16(&channel[1]), audio + 2);
					channel += 2;
					audio += 4;
				}

				if (count & 1)
				{
					int32 delta = *reinterpret_cast<const int8 *>(code++);
					sample = (Sample) (sample + delta);
					combiner::Output(sample, ReadLittleEndianS16(&channel[0]), audio);
					channel++;
					audio += 2;
				}

				break;
			}

			case 2:
			{
				k = count & ~3;
				for (machine a = 0; a < k; a += 4)
				{
					int32 value1 = code[0];
					int32 value2 = code[1];
					int32 value3 = code[2];
					code += 3;

					int32 delta1 = value1 << 24 >> 26;
					int32 delta2 = ((value1 << 30) | (value2 << 22)) >> 26;
					int32 delta3 = ((value2 << 28) | (value3 << 20)) >> 26;
					int32 delta4 = value3 << 26 >> 26;

					sample = (Sample) (sample + delta1);
					combiner::Output(sample, ReadLittleEndianS16(&channel[0]), audio);
					sample = (Sample) (sample + delta2);
					combiner::Output(sample, ReadLittleEndianS16(&channel[1]), audio + 2);
					sample = (Sample) (sample + delta3);
					combiner::Output(sample, ReadLittleEndianS16(&channel[2]), audio + 4);
					sample = (Sample) (sample + delta4);
					combiner::Output(sample, ReadLittleEndianS16(&channel[3]), audio + 6);
					channel += 4;
					audio += 8;
				}

				for (machine a = k; a < count; a++)
				{
					int32 delta = *reinterpret_cast<const int8 *>(code++);
					sample = (Sample) (sample + delta);
					combiner::Output(sample, ReadLittleEndianS16(&channel[0]), audio);
					channel++;
					audio += 2;
				}

				break;
			}

			case 3:
			{
				for (machine a = 0; a < count; a++)
				{
					int32 delta = reinterpret_cast<const int8 *>(code)[a];
					sample = (Sample) (sample + delta);
					combiner::Output(sample, ReadLittleEndianS16(&channel[0]), audio);
					channel++;
					audio += 2;
				}

				code += count;
				break;
			}

			case 4:
			{
				k = count & ~1;
				for (machine a = 0; a < k; a += 2)
				{
					int32 value1 = code[0];
					int32 value2 = code[1];
					int32 value3 = code[2];
					code += 3;

					int32 delta1 = value1 | ((value3 << 28 >> 20) & ~0xFF);
					int32 delta2 = value2 | ((value3 << 24 >> 20) & ~0xFF);

					sample = (Sample) (sample + delta1);
					combiner::Output(sample, ReadLittleEndianS16(&channel[0]), audio);
					sample = (Sample) (sample + delta2);
					combiner::Output(sample, ReadLittleEndianS16(&channel[1]), audio + 2);
					channel += 2;
					audio += 4;
				}

				if (count & 1)
				{
					sample = (Sample) (code[0] | (reinterpret_cast<const int8 *>(code)[1] << 8));
					combiner::Output(sample, ReadLittleEndianS16(&channel[0]), audio);
					channel++;
					audio += 2;
					code += 2;
				}

				break;
			}

			default:
			{
				for (machine a = 0; a < count; a++)
				{
					sample = (Sample) (code[0] | (reinterpret_cast<const int8 *>(code)[1] << 8));
					combiner::Output(sample, ReadLittleEndianS16(&channel[0]), audio);
					channel++;
					audio += 2;
					code += 2;
				}

				break;
			}
		}

		sampleCount -= count;
	}
}

void AudioDecompressor::DecompressBlock(Sample *audio, int32 frameCount)
{
	const MovieAudioChannelData *channelData = audioBlockHeader->audioChannelData;

	if (channelCount == 1)
	{
		const unsigned_int8 *code = channelData[0].GetAudioCode();
		const unsigned_int8 *data = audioBlockData;

		if (channelData[0].audioChannelFlags == kMovieCompressionGeneral) Comp::DecompressData(code, channelData[0].audioCodeSize, const_cast<unsigned_int8 *>(data));
		else data = code;

		DecompressAudioMono<1>(data, frameCount, audio);
	}
	else
	{
		const unsigned_int8 *code1 = channelData[0].GetAudioCode();
		const unsigned_int8 *code2 = channelData[1].GetAudioCode();

		const unsigned_int8 *data1 = audioBlockData;
		const unsigned_int8 *data2 = data1 + channelData[0].audioDataSize;

		if (channelData[0].audioChannelFlags == kMovieCompressionGeneral) Comp::DecompressData(code1, channelData[0].audioCodeSize, const_cast<unsigned_int8 *>(data1));
		else data1 = code1;

		if (channelData[1].audioChannelFlags == kMovieCompressionGeneral) Comp::DecompressData(code2, channelData[1].audioCodeSize, const_cast<unsigned_int8 *>(data2));
		else data2 = code2;

		unsigned_int32 flags = audioBlockHeader->audioBlockFlags;
		if (flags == kAudioIndependent)
		{
			DecompressAudioMono<2>(data1, frameCount, audio);
			DecompressAudioMono<2>(data2, frameCount, audio + 1);
		}
		else
		{
			static void (*const decompressor[kAudioChannelDependencyCount - 1])(const unsigned_int8 *, int32, const Sample *, Sample *) =
			{
				&DecompressAudioStereo<AudioLeftDifferenceCombiner>, &DecompressAudioStereo<AudioRightDifferenceCombiner>, &DecompressAudioStereo<AudioAverageDifferenceCombiner>
			};

			DecompressAudioMono<1>(data1, frameCount, audioSampleBuffer);
			(*decompressor[flags - 1])(data2, frameCount, audioSampleBuffer, audio);
		}
	}
}


C4::Movie::Movie(bool conduit)
{
	movieState = kMovieStopped;
	movieDuration = 0;

	movieTime = 0;
	movieLoop = 0;

	conduitFlag = conduit;
	movieVolume = 1.0F;

	movieRenderFlag = false;
	movieUpdateFlag = false;

	movieResource = nullptr;
	videoTrackHeader = nullptr;
	audioTrackHeader = nullptr;
	movieSound = nullptr;
}

C4::Movie::~Movie()
{
	while (movieRenderFlag) Thread::Yield();

	if (movieResource)
	{
		if (audioTrackHeader)
		{
			if (movieSound) movieSound->Release();
			audioDecompressor->~AudioDecompressor();
		}

		if (videoTrackHeader)
		{
			if (alphaTexture) alphaTexture->Release();
			chrominanceTexture->Release();
			luminanceTexture->Release();

			videoDecompressor->~VideoDecompressor();
		}

		if (movieTrackHeader) MovieResource::ReleaseHeaderData(movieTrackHeader);

		movieResource->CloseLoader(&movieLoader);
		movieResource->Release();
	}
}

MovieResult C4::Movie::Load(const char *name)
{
	if (movieResource)
	{
		if (audioTrackHeader)
		{
			if (movieSound)
			{
				movieSound->Release();
				movieSound = nullptr;
			}

			ReleaseStreamMemory();

			audioDecompressor->~AudioDecompressor();
			audioTrackHeader = nullptr;
		}

		if (videoTrackHeader)
		{
			if (alphaTexture) alphaTexture->Release();
			chrominanceTexture->Release();
			luminanceTexture->Release();

			videoDecompressor->~VideoDecompressor();
			videoTrackHeader = nullptr;
		}

		if (movieTrackHeader) MovieResource::ReleaseHeaderData(movieTrackHeader);

		movieResource->CloseLoader(&movieLoader);
		movieResource->Release();
		movieResource = nullptr;
	}

	movieState = kMovieStopped;
	movieDuration = 0;
	movieTime = 0;

	MovieResource *resource = MovieResource::Get(name, kResourceDeferLoad);
	if (!resource) return (kMovieLoadFailed);

	if (resource->OpenLoader(&movieLoader) != kResourceOkay)
	{
		resource->Release();
		return (kMovieLoadFailed);
	}

	if (resource->LoadHeaderData(&movieLoader, &movieResourceHeader, &movieTrackHeader) != kResourceOkay)
	{
		resource->CloseLoader(&movieLoader);
		resource->Release();
		return (kMovieLoadFailed);
	}

	int32 trackCount = movieResourceHeader.movieTrackCount;
	const MovieTrackHeader *track = movieTrackHeader;
	for (machine a = 0; a < trackCount; a++)
	{
		MovieTrackType type = track->movieTrackType;
		if (type == kMovieTrackVideo)
		{
			videoTrackHeader = track->GetVideoTrackHeader();
			new(videoDecompressor) VideoDecompressor(videoTrackHeader);

			movieDuration = Max(movieDuration, videoTrackHeader->videoFrameTime * videoTrackHeader->videoFrameCount);

			videoFrameIndex = -1;
			baseFrameIndex = -1;
		}
		else if (type == kMovieTrackAudio)
		{
			audioTrackHeader = track->GetAudioTrackHeader();
			new(audioDecompressor) AudioDecompressor(audioTrackHeader);

			unsigned_int32 streamSize = audioTrackHeader->blockFrameCount * audioTrackHeader->audioChannelCount * sizeof(Sample);
			AllocateStreamMemory(streamSize, streamSize);

			int64 audioDuration = int64(audioTrackHeader->audioFrameCount) * kMovieTicksPerSecond / audioTrackHeader->audioSampleFrequency;
			movieDuration = Max(movieDuration, (int32) audioDuration);

			SetStreamChannelCount(audioTrackHeader->audioChannelCount);
			SetStreamSampleRate(audioTrackHeader->audioSampleFrequency);
		}

		track++;
	}

	if (!videoTrackHeader)
	{
		resource->CloseLoader(&movieLoader);
		resource->Release();
		return (kMovieVideoTrackMissing);
	}

	int32 width = videoTrackHeader->videoFrameSize.x;
	int32 height = videoTrackHeader->videoFrameSize.y;

	luminanceTextureHeader.textureType = kTextureRectangle;
	luminanceTextureHeader.textureFlags = 0;
	luminanceTextureHeader.colorSemantic = kTextureSemanticLuminance;
	luminanceTextureHeader.alphaSemantic = kTextureSemanticNone;
	luminanceTextureHeader.imageFormat = kTextureL8;
	luminanceTextureHeader.imageWidth = width;
	luminanceTextureHeader.imageHeight = height;
	luminanceTextureHeader.imageDepth = 1;
	luminanceTextureHeader.wrapMode[0] = kTextureClamp;
	luminanceTextureHeader.wrapMode[1] = kTextureClamp;
	luminanceTextureHeader.wrapMode[2] = kTextureClamp;
	luminanceTextureHeader.mipmapCount = 1;
	luminanceTextureHeader.mipmapDataOffset = 0;
	luminanceTextureHeader.auxiliaryDataSize = 0;
	luminanceTextureHeader.auxiliaryDataOffset = 0;

	chrominanceTextureHeader.textureType = kTextureRectangle;
	chrominanceTextureHeader.textureFlags = 0;
	chrominanceTextureHeader.colorSemantic = kTextureSemanticChrominance;
	chrominanceTextureHeader.alphaSemantic = kTextureSemanticChrominance;
	chrominanceTextureHeader.imageFormat = kTextureLA8;
	chrominanceTextureHeader.imageWidth = width >> 1;
	chrominanceTextureHeader.imageHeight = height >> 1;
	chrominanceTextureHeader.imageDepth = 1;
	chrominanceTextureHeader.wrapMode[0] = kTextureClamp;
	chrominanceTextureHeader.wrapMode[1] = kTextureClamp;
	chrominanceTextureHeader.wrapMode[2] = kTextureClamp;
	chrominanceTextureHeader.mipmapCount = 1;
	chrominanceTextureHeader.mipmapDataOffset = 0;
	chrominanceTextureHeader.auxiliaryDataSize = 0;
	chrominanceTextureHeader.auxiliaryDataOffset = 0;

	luminanceTexture = Texture::Get(&luminanceTextureHeader, videoDecompressor->GetLuminanceImage());
	chrominanceTexture = Texture::Get(&chrominanceTextureHeader, videoDecompressor->GetChrominanceImage());

	if (videoTrackHeader->videoTrackFlags & kMovieVideoAlphaChannel)
	{
		alphaTextureHeader.textureType = kTextureRectangle;
		alphaTextureHeader.textureFlags = 0;
		alphaTextureHeader.colorSemantic = kTextureSemanticTransparency;
		alphaTextureHeader.alphaSemantic = kTextureSemanticTransparency;
		alphaTextureHeader.imageFormat = kTextureI8;
		alphaTextureHeader.imageWidth = width;
		alphaTextureHeader.imageHeight = height;
		alphaTextureHeader.imageDepth = 1;
		alphaTextureHeader.wrapMode[0] = kTextureClamp;
		alphaTextureHeader.wrapMode[1] = kTextureClamp;
		alphaTextureHeader.wrapMode[2] = kTextureClamp;
		alphaTextureHeader.mipmapCount = 1;
		alphaTextureHeader.mipmapDataOffset = 0;
		alphaTextureHeader.auxiliaryDataSize = 0;
		alphaTextureHeader.auxiliaryDataOffset = 0;

		alphaTexture = Texture::Get(&alphaTextureHeader, videoDecompressor->GetAlphaImage());
	}
	else
	{
		alphaTexture = nullptr;
	}

	movieResource = resource;
	return (kMovieOkay);
}

void C4::Movie::Play(void)
{
	if ((videoTrackHeader) && (movieState == kMovieStopped))
	{
		movieState = kMoviePlaying;

		if ((audioTrackHeader) && (!conduitFlag))
		{
			movieSound = new Sound;
			movieSound->Stream(this, true);

			movieSound->SetSoundProperty(kSoundVolume, movieVolume);
			movieSound->Play();
		}
	}
}

void C4::Movie::Stop(void)
{
	if (movieState == kMoviePlaying)
	{
		movieState = kMovieStopped;

		if (movieSound)
		{
			movieSound->Release();
			movieSound = nullptr;
		}
	}
}

MovieResult C4::Movie::RenderVideoFrame(void)
{
	int32 frameIndex = videoFrameIndex;
	const MovieVideoFrameData *frameData = &videoTrackHeader->videoFrameData[frameIndex];

	int32 baseIndex = frameData->videoBaseFrameIndex;
	if (baseFrameIndex != baseIndex)
	{
		ResourceResult result = movieResource->LoadVideoFrameData(&movieLoader, &movieResourceHeader, videoTrackHeader, baseIndex, videoDecompressor->GetVideoFrameHeader());
		if (result != kResourceOkay) return (kMovieLoadFailed);

		videoDecompressor->DecompressLuminance(false);
		videoDecompressor->DecompressChrominance(false);
		if (videoTrackHeader->videoTrackFlags & kMovieVideoAlphaChannel) videoDecompressor->DecompressAlpha(false);

		baseFrameIndex = baseIndex;
	}

	int32 width = videoTrackHeader->videoFrameSize.x;
	int32 height = videoTrackHeader->videoFrameSize.y;

	if (frameIndex == baseIndex)
	{
		luminanceTexture->SetImagePointerOffset(0);
		chrominanceTexture->SetImagePointerOffset(0);
		if (videoTrackHeader->videoTrackFlags & kMovieVideoAlphaChannel) alphaTexture->SetImagePointerOffset(0);
	}
	else
	{
		ResourceResult result = movieResource->LoadVideoFrameData(&movieLoader, &movieResourceHeader, videoTrackHeader, frameIndex, videoDecompressor->GetVideoFrameHeader());
		if (result != kResourceOkay) return (kMovieLoadFailed);

		videoDecompressor->DecompressLuminance(true);
		videoDecompressor->DecompressChrominance(true);

		int32 pixelCount = width * height;
		luminanceTexture->SetImagePointerOffset(pixelCount);
		chrominanceTexture->SetImagePointerOffset(pixelCount >> 1);

		if (videoTrackHeader->videoTrackFlags & kMovieVideoAlphaChannel)
		{
			videoDecompressor->DecompressAlpha(true);
			alphaTexture->SetImagePointerOffset(pixelCount);
		}
	}

	return (kResourceOkay);
}

void C4::Movie::UpdateVideoFrame(void)
{
	Rect rect(0, 0, videoTrackHeader->videoFrameSize.x, videoTrackHeader->videoFrameSize.y);
	luminanceTexture->Update(rect);

	chrominanceTexture->Update(Rect(0, 0, rect.right >> 1, rect.bottom >> 1));

	if (videoTrackHeader->videoTrackFlags & kMovieVideoAlphaChannel) alphaTexture->Update(rect);
}

bool C4::Movie::Update(void)
{
	if (movieUpdateFlag)
	{
		movieUpdateFlag = false;
		UpdateVideoFrame();
	}

	if (!movieRenderFlag)
	{
		int32 frameIndex = Min(movieTime / videoTrackHeader->videoFrameTime, videoTrackHeader->videoFrameCount - 1);
		int32 previousIndex = videoFrameIndex;
		if (frameIndex != previousIndex)
		{
			videoFrameIndex = frameIndex;
			if (previousIndex >= 0)
			{
				movieRenderFlag = true;
				TheMovieMgr->SubmitMovieRenderTask(this);
			}
			else
			{
				RenderVideoFrame();
				UpdateVideoFrame();
			}
		}
	}

	bool result = false;

	if (movieState == kMoviePlaying)
	{
		MovieTime time = movieTime + TheTimeMgr->GetDeltaTime() * kMovieTicksPerMillisecond;
		MovieTime duration = GetMovieDuration();
		if (time < duration)
		{
			movieTime = time;
			result = true;
		}
		else
		{
			if (movieLoop == 0)
			{
				movieState = kMovieStopped;
				movieTime = duration;
				CallCompletionProc();
			}
			else
			{
				movieTime = time % duration;
			}
		}
	}

	return (result);
}

int32 C4::Movie::GetStreamFrameCount(void)
{
	return (audioTrackHeader->audioFrameCount);
}

SoundResult C4::Movie::StartStream(void)
{
	int32 frequency = audioTrackHeader->audioSampleFrequency;
	audioFrameIndex = (int32) (int64(movieTime) * frequency / kMovieTicksPerSecond);
	return (kSoundOkay);
}

bool C4::Movie::FillBuffer(unsigned_int32 bufferSize, Sample *buffer, int32 *count)
{
	int32 frameIndex = audioFrameIndex;
	int32 blockFrameCount = audioTrackHeader->blockFrameCount;
	int32 blockIndex = frameIndex / blockFrameCount;

	if (movieResource->LoadAudioBlockData(&movieLoader, &movieResourceHeader, audioTrackHeader, blockIndex, audioDecompressor->GetAudioBlockHeader()) != kResourceOkay)
	{
		*count = 0;
		return (false);
	}

	int32 audioFrameCount = audioTrackHeader->audioFrameCount;
	int32 channelCount = audioTrackHeader->audioChannelCount;
	unsigned_int32 frameSize = channelCount * sizeof(Sample);

	int32 blockStartFrameIndex = blockIndex * blockFrameCount;
	int32 internalFrameIndex = frameIndex - blockStartFrameIndex;

	int32 frameCount = Min(blockStartFrameIndex + blockFrameCount, audioFrameCount) - (blockStartFrameIndex + internalFrameIndex);
	frameCount = Min(frameCount, bufferSize / frameSize);
	audioFrameIndex = frameIndex + frameCount;
	*count = frameCount;

	if (internalFrameIndex == 0)
	{
		audioDecompressor->DecompressBlock(buffer, frameCount);
	}
	else
	{
		Sample *workBuffer = reinterpret_cast<Sample *>(GetWorkBuffer());
		audioDecompressor->DecompressBlock(workBuffer, internalFrameIndex + frameCount);
		MemoryMgr::CopyMemory(workBuffer + internalFrameIndex * channelCount, buffer, frameCount * frameSize);
	}

	if (audioFrameIndex < audioFrameCount) return (true);

	if (movieLoop)
	{
		audioFrameIndex = 0;
		return (true);
	}

	return (false);
}

Sound *Movie::LoadSound(Source *source)
{
	Sound *sound = new Sound;
	sound->Stream(this, true);
	return (sound);
}

void Movie::SetSoundVolume(Sound *sound, float volume)
{
	sound->SetSoundProperty(kSoundVolume, movieVolume * volume);
}


MovieProcess::MovieProcess(Texture *luminance, Texture *chrominance, Texture *alpha) : Process(kProcessMovie)
{
	luminanceTexture = luminance;
	chrominanceTexture = chrominance;
	alphaTexture = alpha;
}

MovieProcess::MovieProcess(const MovieProcess& movieProcess) : Process(movieProcess)
{
	luminanceTexture = movieProcess.luminanceTexture;
	chrominanceTexture = movieProcess.chrominanceTexture;
	alphaTexture = movieProcess.alphaTexture;
}

MovieProcess::~MovieProcess()
{
}

Process *MovieProcess::Replicate(void) const
{
	return (new MovieProcess(*this));
}

int32 MovieProcess::GetPortCount(void) const
{
	return (1);
}

const char *MovieProcess::GetPortName(int32 index) const
{
	return ("TEXC");
}

void MovieProcess::GenerateProcessData(const ShaderCompileData *compileData, ProcessData *data) const
{
	data->registerCount = 1;
	data->inputSize[0] = 2;

	data->textureObject[0] = luminanceTexture;
	data->textureObject[1] = chrominanceTexture;

	if (alphaTexture)
	{
		data->textureCount = 3;
		data->outputSize = 4;

		data->textureObject[2] = alphaTexture;
	}
	else
	{
		data->textureCount = 2;
		data->outputSize = 3;
	}
}

int32 MovieProcess::GenerateProgramCode(const ShaderCompileData *compileData, const char **programCode) const
{
	static const char code[] =
	{
		"MUL		temp.xy, %0, 0.5;\n"
		"TEX		temp.xw, temp, %IMG1, RECT;\n"
		"MOV		temp.y, temp.w;\n"
		"TEX		temp.z, %0, %IMG0, RECT;\n"
		"DPH_SAT	#.x, temp, {0.0, 1.402, 1.0, -0.701};\n"
		"DPH_SAT	#.y, temp, {-0.344136, -0.714136, 1.0, 0.529136};\n"
		"DPH_SAT	#.z, temp, {1.772, 0.0, 1.0, -0.886};\n"
	};

	static const char alphaCode[] =
	{
		"TEX		#.w, %0, %IMG2, RECT;\n"
	};

	programCode[0] = code;

	if (alphaTexture)
	{
		programCode[1] = alphaCode;
		return (2);
	}

	return (1);
}

int32 MovieProcess::GenerateShaderCode(const ShaderCompileData *compileData, const char **shaderCode) const
{
	static const char code[] =
	{
		#if C4OPENGL

			"temp.xy = texture2DRect(%IMG1, %0 * 0.5).xw;\n"
			"temp.z = texture2DRect(%IMG0, %0).z;\n"
			"#.x = clamp(dot(temp.xyz, vec3(0.0, 1.402, 1.0)) - 0.701, 0.0, 1.0);\n"
			"#.y = clamp(dot(temp.xyz, vec3(-0.344136, -0.714136, 1.0)) + 0.529136, 0.0, 1.0);\n"
			"#.z = clamp(dot(temp.xyz, vec3(1.772, 0.0, 1.0)) - 0.886, 0.0, 1.0);\n"

		#else

			"temp.xy = texRECT(%IMG1, %0 * 0.5).xw;\n"
			"temp.z = texRECT(%IMG0, %0).z;\n"
			"#.x = saturate(dot(temp.xyz, float3(0.0, 1.402, 1.0)) - 0.701);\n"
			"#.y = saturate(dot(temp.xyz, float3(-0.344136, -0.714136, 1.0)) + 0.529136);\n"
			"#.z = saturate(dot(temp.xyz, float3(1.772, 0.0, 1.0)) - 0.886);\n"

		#endif
	};

	static const char alphaCode[] =
	{
		#if C4OPENGL

			"#.w = texRECT(%IMG2, %0).w;\n"

		#else

			"#.w = texture2DRect(%IMG2, %0).w;\n"

		#endif
	};

	shaderCode[0] = code;

	if (alphaTexture)
	{
		shaderCode[1] = alphaCode;
		return (2);
	}

	return (1);
}


MovieWidget::MovieWidget() :
		QuadWidget(kWidgetMovie),
		playTask(&PlayTask, this)
{
	movieStartTime = 0;
	sourceConnectorKey[0] = 0;

	movieObject = nullptr;
}

MovieWidget::MovieWidget(const Vector2D& size, const char *name) :
		QuadWidget(kWidgetMovie, size),
		playTask(&PlayTask, this)
{
	movieName = name;
	movieFlags = 0;
	movieBlendState = kBlendInterpolate;
	movieStartTime = 0;
	sourceConnectorKey[0] = 0;

	movieObject = nullptr;
}

MovieWidget::MovieWidget(const MovieWidget& movieWidget) :
		QuadWidget(movieWidget),
		playTask(&PlayTask, this)
{
	movieName = movieWidget.movieName;
	movieFlags = movieWidget.movieFlags;
	movieBlendState = movieWidget.movieBlendState;
	movieStartTime = movieWidget.movieStartTime;
	sourceConnectorKey = movieWidget.sourceConnectorKey;

	movieObject = nullptr;
}

MovieWidget::~MovieWidget()
{
	Node *node = sourceNode;
	if (node)
	{
		Source *source = static_cast<Source *>(node);
		source->Stop();
		source->SetSoundConduit(nullptr);
	}

	delete movieObject;
}

Widget *MovieWidget::Replicate(void) const
{
	return (new MovieWidget(*this));
}

void MovieWidget::RegisterFunctions(ControllerRegistration *registration)
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();

	static FunctionReg<PlayMovieWidgetFunction> playMovieWidgetRegistration(registration, kFunctionPlayMovieWidget, table->GetString(StringID('CTRL', kControllerPanel, kFunctionPlayMovieWidget)), kFunctionRemote | kFunctionJournaled);
	static FunctionReg<StopMovieWidgetFunction> stopMovieWidgetRegistration(registration, kFunctionStopMovieWidget, table->GetString(StringID('CTRL', kControllerPanel, kFunctionStopMovieWidget)), kFunctionRemote | kFunctionJournaled);
	static FunctionReg<SetMovieWidgetTimeFunction> getMovieWidgetTimeRegistration(registration, kFunctionGetMovieWidgetTime, table->GetString(StringID('CTRL', kControllerPanel, kFunctionGetMovieWidgetTime)), kFunctionOutputValue);
	static FunctionReg<SetMovieWidgetTimeFunction> setMovieWidgetTimeRegistration(registration, kFunctionSetMovieWidgetTime, table->GetString(StringID('CTRL', kControllerPanel, kFunctionSetMovieWidgetTime)), kFunctionRemote);
}

void MovieWidget::Pack(Packer& data, unsigned_int32 packFlags) const
{
	QuadWidget::Pack(data, packFlags);

	PackHandle handle = data.BeginChunk('NAME');
	data << movieName;
	data.EndChunk(handle);

	unsigned_int32 flags = movieFlags;
	if ((movieObject) && (movieObject->GetMovieState() == kMovieStopped)) flags &= ~kMovieInitialPlay;
	data << ChunkHeader('FLAG', 4);
	data << flags;

	data << ChunkHeader('BLND', 4);
	data << movieBlendState;

	if (movieObject)
	{
		unsigned_int32 time = movieObject->GetMovieTime();
		data << ChunkHeader('STRT', 4);
		data << time;
	}

	if (sourceConnectorKey[0] != 0)
	{
		PackHandle handle = data.BeginChunk('SCON');
		data << sourceConnectorKey;
		data.EndChunk(handle);
	}

	data << TerminatorChunk;
}

void MovieWidget::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	QuadWidget::Unpack(data, unpackFlags);
	UnpackChunkList<MovieWidget>(data, unpackFlags);
}

bool MovieWidget::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'NAME':

			data >> movieName;
			return (true);

		case 'FLAG':

			data >> movieFlags;
			return (true);

		case 'BLND':

			data >> movieBlendState;
			return (true);

		case 'STRT':

			data >> movieStartTime;
			return (true);

		case 'SCON':

			data >> sourceConnectorKey;
			return (true);
	}

	return (false);
}

void *MovieWidget::BeginSettingsUnpack(void)
{
	movieStartTime = 0;
	sourceConnectorKey[0] = 0;
	return (nullptr);
}

int32 MovieWidget::GetSettingCount(void) const
{
	return (QuadWidget::GetSettingCount() + 6);
}

Setting *MovieWidget::GetSetting(int32 index) const
{
	int32 count = QuadWidget::GetSettingCount();
	if (index < count) return (QuadWidget::GetSetting(index));

	const StringTable *table = TheInterfaceMgr->GetStringTable();

	if (index == count)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetMovie, 'SETT'));
		return (new HeadingSetting(kWidgetMovie, title));
	}

	if (index == count + 1)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetMovie, 'NAME'));
		const char *picker = table->GetString(StringID('WDGT', kWidgetMovie, 'PICK'));
		return (new ResourceSetting('MNAM', movieName, title, picker, MovieResource::GetDescriptor()));
	}

	if (index == count + 2)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetMovie, 'PLAY'));
		return (new BooleanSetting('PLAY', ((movieFlags & kMovieInitialPlay) != 0), title));
	}

	if (index == count + 3)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetMovie, 'LOOP'));
		return (new BooleanSetting('LOOP', ((movieFlags & kMovieLoop) != 0), title));
	}

	if (index == count + 4)
	{
		int32 selection = 0;
		if (movieBlendState == BlendState(kBlendSourceAlpha, kBlendOne)) selection = 1;
		else if (movieBlendState == kBlendInterpolate) selection = 2;
		else if (movieBlendState == BlendState(kBlendOne, kBlendInvSourceAlpha)) selection = 3;
		else if (movieBlendState == kBlendReplace) selection = 4;

		const char *title = table->GetString(StringID('WDGT', kWidgetMovie, 'BLND'));
		MenuSetting *menu = new MenuSetting('BLND', selection, title, 5);

		menu->SetMenuItemString(0, table->GetString(StringID('WDGT', kWidgetMovie, 'BLND', 'ADD ')));
		menu->SetMenuItemString(1, table->GetString(StringID('WDGT', kWidgetMovie, 'BLND', 'ADDA')));
		menu->SetMenuItemString(2, table->GetString(StringID('WDGT', kWidgetMovie, 'BLND', 'TERP')));
		menu->SetMenuItemString(3, table->GetString(StringID('WDGT', kWidgetMovie, 'BLND', 'PREM')));
		menu->SetMenuItemString(4, table->GetString(StringID('WDGT', kWidgetMovie, 'BLND', 'REPL')));

		return (menu);
	}

	if (index == count + 5)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetMovie, 'SCON'));
		return (new TextSetting('SCON', sourceConnectorKey, title, kMaxConnectorKeyLength, &Connector::ConnectorKeyFilter));
	}

	return (nullptr);
}

void MovieWidget::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();

	if (identifier == 'MNAM')
	{
		movieName = static_cast<const ResourceSetting *>(setting)->GetResourceName();
	}
	else if (identifier == 'PLAY')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) movieFlags |= kMovieInitialPlay;
		else movieFlags &= ~kMovieInitialPlay;
	}
	else if (identifier == 'LOOP')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) movieFlags |= kMovieLoop;
		else movieFlags &= ~kMovieLoop;
	}
	else if (identifier == 'BLND')
	{
		static const unsigned_int32 blendTable[5] = 
		{
			kBlendAccumulate, BlendState(kBlendSourceAlpha, kBlendOne), kBlendInterpolate, BlendState(kBlendOne, kBlendInvSourceAlpha), kBlendReplace
		};

		movieBlendState = blendTable[static_cast<const MenuSetting *>(setting)->GetMenuSelection()];
	}
	else if (identifier == 'SCON')
	{
		sourceConnectorKey = static_cast<const TextSetting *>(setting)->GetText();
	}
	else
	{
		QuadWidget::SetSetting(setting);
	}
}

void MovieWidget::Preprocess(void)
{
	QuadWidget::Preprocess();

	InvalidateShaderData();
	attributeList.RemoveAll();

	ShaderGraph *graph = shaderAttribute.GetShaderGraph(kShaderGraphAmbient);
	graph->Purge();

	RootWidget *root = GetRootWidget();
	if (root) root->RemoveMovingWidget(this);

	delete movieObject;
	movieObject = nullptr;
	sourceNode = nullptr;

	PanelController *controller = GetPanelController();
	if (!GetManipulator())
	{
		if ((!controller) || (!controller->GetTargetNode()->GetManipulator()))
		{
			if (movieName[0] != 0)
			{
				Movie *movie = new Movie(controller != nullptr);
				if (movie->Load(movieName) == kMovieOkay) movieObject = movie;
				else delete movie;
			}
		}
	}

	SetAttributeArray(kArrayTexture0, movieTexcoord);
	SetAmbientBlendState(kBlendReplace);

	attributeList.Append(&shaderAttribute);
	SetMaterialAttributeList(&attributeList);

	if (movieObject)
	{
		const Integer2D& size = movieObject->GetVideoFrameSize();
		float width = (float) size.x;
		float height = (float) size.y;

		movieTexcoord[0].Set(0.0F, height);
		movieTexcoord[1].Set(0.0F, 0.0F);
		movieTexcoord[2].Set(width, height);
		movieTexcoord[3].Set(width, 0.0F);

		movieProcess = new MovieProcess(movieObject->GetLuminanceTexture(), movieObject->GetChrominanceTexture(), movieObject->GetAlphaTexture());
		graph->AddElement(movieProcess);

		Texcoord0Process *texcoordProcess = new Texcoord0Process;
		graph->AddElement(texcoordProcess);

		AmbientOutputProcess *ambientOutputProcess = new AmbientOutputProcess;
		graph->AddElement(ambientOutputProcess);

		new Route(texcoordProcess, movieProcess, 0);
		new Route(movieProcess, ambientOutputProcess, 0);

		if (movieObject->GetAlphaTexture())
		{
			SetAmbientBlendState(movieBlendState);

			AlphaOutputProcess *alphaOutputProcess = new AlphaOutputProcess;
			graph->AddElement(alphaOutputProcess);

			(new Route(movieProcess, alphaOutputProcess, 0))->SetRouteSwizzle('aaaa');
		}

		if (root)
		{
			root->AddMovingWidget(this);

			if ((controller) && (sourceConnectorKey[0] != 0))
			{
				Node *node = controller->GetTargetNode()->GetConnectedNode(sourceConnectorKey);
				if ((node) && (node->GetNodeType() == kNodeSource))
				{
					sourceNode = node;
					static_cast<Source *>(node)->SetSoundConduit(movieObject);
				}
			}
		}

		movieObject->SetMovieTime(movieStartTime);

		unsigned_int32 flags = movieFlags;
		if (flags & kMovieLoop)
		{
			movieObject->SetMovieLoop(true);

			const Node *node = sourceNode;
			if (node)
			{
				SourceObject *sourceObject = static_cast<const Source *>(node)->GetObject();
				sourceObject->SetSourceFlags((sourceObject->GetSourceFlags() | kSourceLoop) & ~kSourceInitialPlay);
			}
		}
		else
		{
			const Node *node = sourceNode;
			if (node)
			{
				SourceObject *sourceObject = static_cast<const Source *>(node)->GetObject();
				sourceObject->SetSourceFlags(sourceObject->GetSourceFlags() & ~(kSourceInitialPlay | kSourceLoop));
			}
		}

		if (flags & kMovieInitialPlay)
		{
			if (controller) controller->GetTargetNode()->GetWorld()->AddDeferredTask(&playTask);
			else movieObject->Play();
		}
	}
	else
	{
		TextureMapProcess *textureMapProcess = new TextureMapProcess;
		textureMapProcess->SetTexture("C4/checker");
		graph->AddElement(textureMapProcess);

		Texcoord0Process *texcoordProcess = new Texcoord0Process;
		graph->AddElement(texcoordProcess);

		AmbientOutputProcess *ambientOutputProcess = new AmbientOutputProcess;
		graph->AddElement(ambientOutputProcess);

		new Route(texcoordProcess, textureMapProcess, 0);
		new Route(textureMapProcess, ambientOutputProcess, 0);
	}
}

void MovieWidget::PlayTask(DeferredTask *task, void *cookie)
{
	static_cast<MovieWidget *>(cookie)->PlayMovie();
}

void MovieWidget::ExtendAnimationTime(void)
{
	PanelController *controller = GetPanelController();
	if (controller)
	{
		Movie *movie = movieObject;
		MovieTime time = Max(movie->GetMovieDuration() - movie->GetMovieTime(), kMovieTicksPerSecond);
		controller->ExtendAnimationTime((float) time * (kMovieFloatSecondsPerTick * 1000.0F));
	}
}

void MovieWidget::Move(void)
{
	QuadWidget::Move();

	Movie *movie = movieObject;
	if ((movie) && (movie->Update())) ExtendAnimationTime();
}

void MovieWidget::Build(void)
{
	if (!movieObject)
	{
		const Vector2D& size = GetWidgetSize();
		float width = (float) size.x * 0.03125F;
		float height = (float) size.y * 0.03125F;

		movieTexcoord[0].Set(0.0F, height);
		movieTexcoord[1].Set(0.0F, 0.0F);
		movieTexcoord[2].Set(width, height);
		movieTexcoord[3].Set(width, 0.0F);
	}

	QuadWidget::Build();
}

void MovieWidget::HandleMouseEvent(const PanelMouseEventData *eventData)
{
	if (eventData->eventType == kEventMouseDown) Activate();
}

void MovieWidget::PlayMovie(void)
{
	Movie *movie = movieObject;
	if (movie)
	{
		movie->Play();
		ExtendAnimationTime();

		Node *node = sourceNode;
		if (node) static_cast<Source *>(node)->Play();
	}
}

void MovieWidget::StopMovie(void)
{
	Movie *movie = movieObject;
	if (movie)
	{
		movie->Stop();

		Node *node = sourceNode;
		if (node) static_cast<Source *>(node)->Stop();
	}
}


PlayMovieWidgetFunction::PlayMovieWidgetFunction() : WidgetFunction(kFunctionPlayMovieWidget)
{
}

PlayMovieWidgetFunction::PlayMovieWidgetFunction(const PlayMovieWidgetFunction& playMovieWidgetFunction) : WidgetFunction(playMovieWidgetFunction)
{
}

PlayMovieWidgetFunction::~PlayMovieWidgetFunction()
{
}

Function *PlayMovieWidgetFunction::Replicate(void) const
{
	return (new PlayMovieWidgetFunction(*this));
}

bool PlayMovieWidgetFunction::OverridesFunction(const Function *function) const
{
	FunctionType type = function->GetFunctionType();
	if ((type == kFunctionPlayMovieWidget) || (type == kFunctionStopMovieWidget))
	{
		return (static_cast<const WidgetFunction *>(function)->GetWidgetKey() == GetWidgetKey());
	}

	return (false);
}

void PlayMovieWidgetFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	const Panel *root = static_cast<PanelController *>(controller)->GetRootWidget();
	Widget *widget = root->FindWidget(GetWidgetKey());
	while (widget)
	{
		if (widget->GetWidgetType() == kWidgetMovie)
		{
			static_cast<MovieWidget *>(widget)->PlayMovie();
		}

		widget = widget->GetNextWidgetWithSameKey();
	}

	CallCompletionProc();
}


StopMovieWidgetFunction::StopMovieWidgetFunction() : WidgetFunction(kFunctionStopMovieWidget)
{
}

StopMovieWidgetFunction::StopMovieWidgetFunction(const StopMovieWidgetFunction& stopMovieWidgetFunction) : WidgetFunction(stopMovieWidgetFunction)
{
}

StopMovieWidgetFunction::~StopMovieWidgetFunction()
{
}

Function *StopMovieWidgetFunction::Replicate(void) const
{
	return (new StopMovieWidgetFunction(*this));
}

bool StopMovieWidgetFunction::OverridesFunction(const Function *function) const
{
	FunctionType type = function->GetFunctionType();
	if ((type == kFunctionPlayMovieWidget) || (type == kFunctionStopMovieWidget))
	{
		return (static_cast<const WidgetFunction *>(function)->GetWidgetKey() == GetWidgetKey());
	}

	return (false);
}

void StopMovieWidgetFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	const Panel *root = static_cast<PanelController *>(controller)->GetRootWidget();
	Widget *widget = root->FindWidget(GetWidgetKey());
	while (widget)
	{
		if (widget->GetWidgetType() == kWidgetMovie)
		{
			static_cast<MovieWidget *>(widget)->StopMovie();
		}

		widget = widget->GetNextWidgetWithSameKey();
	}

	CallCompletionProc();
}


GetMovieWidgetTimeFunction::GetMovieWidgetTimeFunction() : WidgetFunction(kFunctionGetMovieWidgetTime)
{
}

GetMovieWidgetTimeFunction::GetMovieWidgetTimeFunction(const GetMovieWidgetTimeFunction& getMovieWidgetTimeFunction) : WidgetFunction(getMovieWidgetTimeFunction)
{
}

GetMovieWidgetTimeFunction::~GetMovieWidgetTimeFunction()
{
}

Function *GetMovieWidgetTimeFunction::Replicate(void) const
{
	return (new GetMovieWidgetTimeFunction(*this));
}

void GetMovieWidgetTimeFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	const Panel *root = static_cast<PanelController *>(controller)->GetRootWidget();
	const Widget *widget = root->FindWidget(GetWidgetKey());
	if (widget)
	{
		if (widget->GetWidgetType() == kWidgetMovie)
		{
			Movie *movie = static_cast<const MovieWidget *>(widget)->GetMovieObject();
			if (movie) method->SetOutputValue(state, movie->GetMovieTime() * kMovieFloatSecondsPerTick);
		}
	}

	CallCompletionProc();
}


SetMovieWidgetTimeFunction::SetMovieWidgetTimeFunction(float time) : WidgetFunction(kFunctionSetMovieWidgetTime)
{
	movieTime = time;
}

SetMovieWidgetTimeFunction::SetMovieWidgetTimeFunction(const SetMovieWidgetTimeFunction& setMovieWidgetTimeFunction) : WidgetFunction(setMovieWidgetTimeFunction)
{
	movieTime = setMovieWidgetTimeFunction.movieTime;
}

SetMovieWidgetTimeFunction::~SetMovieWidgetTimeFunction()
{
}

Function *SetMovieWidgetTimeFunction::Replicate(void) const
{
	return (new SetMovieWidgetTimeFunction(*this));
}

void SetMovieWidgetTimeFunction::Pack(Packer& data, unsigned_int32 packFlags) const
{
	WidgetFunction::Pack(data, packFlags);

	data << movieTime;
}

void SetMovieWidgetTimeFunction::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	WidgetFunction::Unpack(data, unpackFlags);

	data >> movieTime;
}

void SetMovieWidgetTimeFunction::Compress(Compressor& data) const
{
	WidgetFunction::Compress(data);

	data << movieTime;
}

bool SetMovieWidgetTimeFunction::Decompress(Decompressor& data)
{
	if (WidgetFunction::Decompress(data))
	{
		data >> movieTime;
		return (true);
	}

	return (false);
}

int32 SetMovieWidgetTimeFunction::GetSettingCount(void) const
{
	return (WidgetFunction::GetSettingCount() + 1);
}

Setting *SetMovieWidgetTimeFunction::GetSetting(int32 index) const
{
	int32 count = WidgetFunction::GetSettingCount();
	if (index < count) return (WidgetFunction::GetSetting(index));

	if (index == count)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		const char *title = table->GetString(StringID('CTRL', kControllerPanel, kFunctionSetMovieWidgetTime, 'TIME'));
		return (new TextSetting('TIME', movieTime, title));
	}

	return (nullptr);
}

void SetMovieWidgetTimeFunction::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();

	if (identifier == 'TIME')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		movieTime = FmaxZero(Text::StringToFloat(text));
	}
	else
	{
		WidgetFunction::SetSetting(setting);
	}
}

void SetMovieWidgetTimeFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	const Panel *root = static_cast<PanelController *>(controller)->GetRootWidget();
	Widget *widget = root->FindWidget(GetWidgetKey());
	while (widget)
	{
		if (widget->GetWidgetType() == kWidgetMovie)
		{
			Movie *movie = static_cast<MovieWidget *>(widget)->GetMovieObject();
			if (movie) movie->SetMovieTime((MovieTime) (movieTime * kMovieFloatTicksPerSecond));
		}

		widget = widget->GetNextWidgetWithSameKey();
	}

	CallCompletionProc();
}


MovieMgr::MovieMgr(int)
{
}

MovieMgr::~MovieMgr()
{
}

EngineResult MovieMgr::Construct(void)
{
	#if C4RECORDABLE

		recordFrameCount = -1;

	#endif

	new(movieSignal) Signal(2);
	new(movieThread) Thread(&MovieThread, this, 0, movieSignal);

	return (kMovieOkay);
}

void MovieMgr::Destruct(void)
{
	StopRecording();

	movieThread->~Thread();
	movieSignal->~Signal();
}

void MovieMgr::ExtractLuminanceImage(const Integer2D& size, const Color4C *color, int8 *restrict luminance)
{
	int32 pixelCount = size.x * size.y;
	for (machine a = 0; a < pixelCount; a++)
	{
		const Color4C& c = color[a];
		luminance[a] = (int8) (((c.GetRed() * 0x4C8B + c.GetGreen() * 0x9646 + c.GetBlue() * 0x1D2F) >> 16) - 128);
	}
}

void MovieMgr::ExtractChrominanceImage(const Integer2D& size, const Color4C *color, int8 *restrict chrominance, bool reverse)
{
	int32 width = size.x;
	int32 height = size.y;

	if (!reverse)
	{
		for (machine j = 0; j < height; j += 2)
		{
			for (machine i = 0; i < width; i += 2)
			{
				const Color4C *c = &color[i];
				int32 r = c[0].GetRed() + c[1].GetRed() + c[width].GetRed() + c[width + 1].GetRed();
				int32 g = c[0].GetGreen() + c[1].GetGreen() + c[width].GetGreen() + c[width + 1].GetGreen();
				int32 b = c[0].GetBlue() + c[1].GetBlue() + c[width].GetBlue() + c[width + 1].GetBlue();
				chrominance[0] = (int8) ((b * 0x8000 - g * 0x54CE - r * 0x2B32) >> 18);
				chrominance[1] = (int8) ((r * 0x8000 - g * 0x6B2F - b * 0x14D1) >> 18);
				chrominance += 2;
			}

			color += width * 2;
		}
	}
	else
	{
		for (machine j = 0; j < height; j += 2)
		{
			for (machine i = 0; i < width; i += 2)
			{
				const Color4C *c = &color[i];
				int32 b = c[0].GetRed() + c[1].GetRed() + c[width].GetRed() + c[width + 1].GetRed();
				int32 g = c[0].GetGreen() + c[1].GetGreen() + c[width].GetGreen() + c[width + 1].GetGreen();
				int32 r = c[0].GetBlue() + c[1].GetBlue() + c[width].GetBlue() + c[width + 1].GetBlue();
				chrominance[0] = (int8) ((b * 0x8000 - g * 0x54CE - r * 0x2B32) >> 18);
				chrominance[1] = (int8) ((r * 0x8000 - g * 0x6B2F - b * 0x14D1) >> 18);
				chrominance += 2;
			}

			color += width * 2;
		}
	}
}

void MovieMgr::ExtractAlphaImage(const Integer2D& size, const Color4C *color, int8 *restrict alpha)
{
	int32 pixelCount = size.x * size.y;
	for (machine a = 0; a < pixelCount; a++) alpha[a] = (int8) (color[a].GetAlpha() - 128);
}

void MovieMgr::MovieThread(const Thread *thread, void *cookie)
{
	MovieMgr *movieMgr = static_cast<MovieMgr *>(cookie);

	for (;;)
	{
		int32 index = movieMgr->movieSignal->Wait();
		if (index == 0) break;

		movieMgr->renderMutex.Acquire();

		for (;;)
		{
			Movie *movie = movieMgr->renderList.First();
			if (!movie) break;

			movieMgr->renderMutex.Release();

			if (movie->RenderVideoFrame() == kMovieOkay) movie->movieUpdateFlag = true;

			movieMgr->renderMutex.Acquire();
			movieMgr->renderList.Remove(movie);

			movie->movieRenderFlag = false;
		}

		movieMgr->renderMutex.Release();
	}
}

void MovieMgr::SubmitMovieRenderTask(Movie *movie)
{
	renderMutex.Acquire();
	renderList.Append(movie);
	renderMutex.Release();

	movieSignal->Trigger(1);
}

EngineResult MovieMgr::StartRecording(int32 rate, const char *name)
{
	#if C4RECORDABLE

		if (recordFrameCount < 0)
		{
			recordFrameCount = 0;
			recordFileCount = 0;

			int32 width = TheDisplayMgr->GetDisplayWidth();
			int32 height = TheDisplayMgr->GetDisplayHeight();
			recordFrameSize.Set(width, height);

			recordTime = 0;
			recordFrameDuration = kMovieTicksPerSecond / rate;

			recordSequenceHeader.endian = 1;
			recordSequenceHeader.headerSize = sizeof(SequenceHeader);
			recordSequenceHeader.imageWidth = width;
			recordSequenceHeader.imageHeight = height;

			recordFileName = name;
			FileMgr::CreateDirectoryPath(name);

			new(recordFile) File;
			FileResult result = NewRecordFile();
			if (result == kFileOkay)
			{
				int32 pixelCount = width * height;
				unsigned_int32 bufferSize = pixelCount + (pixelCount >> 1);
				recordLuminanceBuffer = new int8[bufferSize];
				recordChrominanceBuffer = recordLuminanceBuffer + pixelCount;

				for (machine a = 0; a < kMovieRecordBufferCount; a++)
				{
					recordBuffer[a].Construct();
					recordBuffer[a].AllocateStorage(width * height * sizeof(Color4C));
					recordStage[a] = kRecordStageReady;
					recordExtractFlag[a] = false;
				}

				recordFrameIndex = 0;

				new(recordSignal) Signal(2);
				new(recordThread) Thread(&RecordThread, this, 0, recordSignal);
			}
			else
			{
				recordFile->~File();
				return (result);
			}
		}

	#endif

	return (kEngineOkay);
}

void MovieMgr::StopRecording(void)
{
	#if C4RECORDABLE

		if (recordFrameCount >= 0)
		{
			recordFrameCount = -1;

			recordThread->~Thread();
			recordSignal->~Signal();

			for (machine a = kMovieRecordBufferCount - 1; a >= 0; a--)
			{
				if (recordStage[a] == kRecordStageCompleted) recordBuffer[a].EndRead();
				recordBuffer[a].Destruct();
			}

			delete[] recordLuminanceBuffer;

			recordFile->SetSize(recordFile->GetPosition());
			recordFile->~File();
		}

	#endif
}

#if C4RECORDABLE

	void MovieMgr::RecordTask(void)
	{
		if (recordFrameCount >= 0)
		{
			for (machine a = 0; a < kMovieRecordBufferCount; a++)
			{
				if (recordExtractFlag[a])
				{
					recordExtractFlag[a] = false;
					recordBuffer[a].EndRead();
				}

				if (recordStage[a] == kRecordStageCompleted)
				{
					recordStage[a] = kRecordStageReady;
				}
			}

			MovieTime time = recordTime + TheTimeMgr->GetSystemDeltaTime() * kMovieTicksPerMillisecond;
			MovieTime frameDuration = recordFrameDuration;

			if (time >= frameDuration)
			{
				int32 index = recordFrameCount;
				machine a = index & kMovieRecordBufferMask;
				if (recordStage[a] == kRecordStageReady)
				{
					time -= frameDuration;

					recordStage[a] = kRecordStageTransferring;
					recordFrameCount = index + 1;

					Render::PixelBufferObject *buffer = &recordBuffer[a];
					buffer->Bind();

					int32 width = TheDisplayMgr->GetDisplayWidth();
					int32 height = TheDisplayMgr->GetDisplayHeight();

					glPixelStorei(GL_PACK_ROW_LENGTH, 0);
					glReadPixels(0, 0, width, height, GL_BGRA, GL_UNSIGNED_BYTE, nullptr);
					buffer->Unbind();
				}

				index -= kMovieRecordBufferCount / 2;
				if (index >= 0)
				{
					a = index & kMovieRecordBufferMask;
					if (recordStage[a] == kRecordStageTransferring)
					{
						recordStage[a] = kRecordStageReading;
						recordImage[a] = static_cast<const Color4C *>(recordBuffer[a].BeginRead());
						recordSignal->Trigger(1);
					}
				}
			}

			recordTime = time;
		}
	}

	FileResult MovieMgr::NewRecordFile(void)
	{
		String<kMaxFileNameLength> path(recordFileName);
		(path += ++recordFileCount) += SequenceResource::GetDescriptor()->GetExtension();

		FileResult result = recordFile->Open(path, kFileCreate);
		if (result == kFileOkay)
		{
			recordFile->SetSize(kMaxRecordFileSize + sizeof(SequenceHeader));
			recordFile->SetPosition(0);

			recordFile->Write(&recordSequenceHeader, sizeof(SequenceHeader));
		}

		recordFileSize = 0;
		return (result);
	}

	void MovieMgr::RecordThread(const Thread *thread, void *cookie)
	{
		MovieMgr *movieMgr = static_cast<MovieMgr *>(cookie);

		for (;;)
		{
			int32 index = movieMgr->recordSignal->Wait();
			if (index == 0) break;

			index = movieMgr->recordFrameIndex;
			for (;;)
			{
				machine a = index & kMovieRecordBufferMask;
				if (movieMgr->recordStage[a] != kRecordStageReading) break;
				
				if (movieMgr->recordFileSize >= kMaxRecordFileSize)
				{
					movieMgr->recordFile->SetSize(movieMgr->recordFile->GetPosition());
					movieMgr->recordFile->Close();

					movieMgr->NewRecordFile();
				}

				ExtractLuminanceImage(movieMgr->recordFrameSize, movieMgr->recordImage[a], movieMgr->recordLuminanceBuffer);
				ExtractChrominanceImage(movieMgr->recordFrameSize, movieMgr->recordImage[a], movieMgr->recordChrominanceBuffer, true);
				movieMgr->recordExtractFlag[a] = true;

				int32 pixelCount = movieMgr->recordFrameSize.x * movieMgr->recordFrameSize.y;
				unsigned_int32 bufferSize = pixelCount + (pixelCount >> 1);

				movieMgr->recordFileSize += bufferSize;
				movieMgr->recordFile->Write(movieMgr->recordLuminanceBuffer, bufferSize);

				movieMgr->recordStage[a] = kRecordStageCompleted;
				index++;
			}

			movieMgr->recordFrameIndex = index;
		}
	}

#endif

String<15> MovieMgr::FormatMovieTime(MovieTime time)
{
	String<15>	string;

	unsigned_int32 tenths = time / (kMovieTicksPerSecond / 10);

	unsigned_int32 seconds = tenths / 10;
	tenths -= seconds * 10;

	unsigned_int32 minutes = seconds / 60;
	seconds -= minutes * 60;

	unsigned_int32 hours = minutes / 60;
	minutes -= hours * 60;

	unsigned_int32 h = hours / 10;
	string[0] = Min(h, 9) + '0';
	string[1] = hours - h * 10 + '0';
	string[2] = ':';

	unsigned_int32 m = minutes / 10;
	string[3] = m + '0';
	string[4] = minutes - m * 10 + '0';
	string[5] = ':';

	unsigned_int32 s = seconds / 10;
	string[6] = s + '0';
	string[7] = seconds - s * 10 + '0';
	string[8] = '.';
	string[9] = tenths + '0';

	string[10] = 0;
	return (string);
}

// ZYURVUR
