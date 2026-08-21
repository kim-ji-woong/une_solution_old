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


#include "C4Sound.h"
#include "C4Engine.h"
#include "C4Packing.h"
#include "C4Time.h"


using namespace C4;


namespace
{
	#if C4GAMECONSOLE
	
		enum
		{
			kSoundOutputSampleRate	= 48000,
			kMaxFeedbackDelay		= 5103
		};
	
	#else
	
		enum
		{
			kSoundOutputSampleRate	= 44100,
			kMaxFeedbackDelay		= 4689
		};
	
	#endif
	
	
	const float kMaxFeedbackTimeDelay = (float) kMaxFeedbackDelay * 1000.0F / (float) kSoundOutputSampleRate;
	const int32 kMaxFeedbackMixCount = (int32) PositiveCeil(kMaxReverbDecayTime / kMaxFeedbackTimeDelay);
	
	
	enum
	{
		kMixFractionSize	= 10,
		kMixFractionMax		= (1 << kMixFractionSize) - 1
	};
	
	
	const float kMixFractionMultiplier = (float) (1 << kMixFractionSize);
	const float kMixFractionReciprocal = 1.0F / kMixFractionMultiplier;
	const float kMinDistanceDelayPlayFrame = -(float) (1 << (30 - kMixFractionSize));
	
	
	const int16 adpcmTable[16] =
	{
		0x00E6, 0x00E6, 0x00E6, 0x00E6, 0x0133, 0x0199, 0x0200, 0x0266,
		0x0300, 0x0266, 0x0200, 0x0199, 0x0133, 0x00E6, 0x00E6, 0x00E6
	};
	
	
	struct RIFFChunkHeader
	{
		unsigned_int32	type;
		unsigned_int32	size;
	};
}


SoundMgr *C4::TheSoundMgr = nullptr;


namespace C4
{
	template <> SoundMgr Manager<SoundMgr>::managerObject(0);
	template <> SoundMgr **Manager<SoundMgr>::managerPointer = &TheSoundMgr;
	
	template <> Heap Memory<Sound>::heap("Sound", MemoryMgr::CalculatePoolSize(kMaxSoundCount, sizeof(Sound)), kHeapMutexless);
	template class Memory<Sound>;
}


ResourceDescriptor SoundResource::descriptor("wav", 0, 1048576, "C4/missing");


void C4::Reverse(WaveHeader *header)
{
	Reverse(&header->format);
	Reverse(&header->numChannels);
	Reverse(&header->sampleRate);
	Reverse(&header->bytesPerSec);
	Reverse(&header->blockAlign);
	Reverse(&header->bitsPerSample);
}


void C4::Reverse(PredictorCoefficient *coefficient)
{
	Reverse(&coefficient->c1);
	Reverse(&coefficient->c2);
}


void C4::Reverse(ADPCMWaveHeader *header)
{
	Reverse(static_cast<WaveHeader *>(header));
	Reverse(&header->extraSize);
	
	Reverse(&header->blockFrameCount);
	Reverse(&header->coefficientCount); 
	
	int32 count = Min(header->coefficientCount, 32); 
	for (machine a = 0; a < count; a++) Reverse(&header->coefficient[a]); 
} 

 
SoundResource::SoundResource(const char *name, ResourceCatalog *catalog) : Resource<SoundResource>(name, catalog)
{
}
 
SoundResource::~SoundResource()
{
}
 
void SoundResource::Preprocess(void)
{
	char *chunkData = static_cast<char *>(GetData());
	
	unsigned_int32 position = 12;
	unsigned_int32 resourceSize = GetSize() - sizeof(RIFFChunkHeader);
	while (position < resourceSize)
	{
		RIFFChunkHeader *chunkHeader = reinterpret_cast<RIFFChunkHeader *>(&chunkData[position]);
		position += sizeof(ChunkHeader);
		
		#if C4LITTLEENDIAN
		
			Reverse(&chunkHeader->type);
		
		#endif
		
		#if C4BIGENDIAN
		
			Reverse(&chunkHeader->size);
		
		#endif
		
		if (chunkHeader->type == 'fmt ')
		{
			WaveHeader *header = reinterpret_cast<WaveHeader *>(chunkHeader + 1);
			waveHeader = header;
			
			#if C4BIGENDIAN
			
				Reverse(header);
			
			#endif
		}
		else if (chunkHeader->type == 'data')
		{
			sampleData = reinterpret_cast<Sample *>(chunkHeader + 1);
			sampleCount = chunkHeader->size / sizeof(Sample);
		}
		
		position += chunkHeader->size;
	}
}

ResourceResult SoundResource::LoadWaveHeader(ResourceLoader *loader, WaveHeader *waveHeader) const
{
	unsigned_int32 size = loader->GetDataSize();
	
	unsigned_int32 position = 12;
	while (position < size)
	{
		RIFFChunkHeader		chunkHeader;
		
		ResourceResult result = loader->Read(&chunkHeader, position, sizeof(RIFFChunkHeader));
		if (result != kResourceOkay) return (result);
		
		position += sizeof(RIFFChunkHeader);
		
		#if C4LITTLEENDIAN
		
			Reverse(&chunkHeader.type);
		
		#endif
		
		#if C4BIGENDIAN
		
			Reverse(&chunkHeader.size);
		
		#endif
		
		if (chunkHeader.type == 'fmt ')
		{
			result = loader->Read(waveHeader, position, sizeof(WaveHeader));
			if (result != kResourceOkay) return (result);
			
			#if C4BIGENDIAN
			
				Reverse(waveHeader);
			
			#endif
			
			break;
		}
		
		position += chunkHeader.size;
	}
	
	return (kResourceOkay);
}

bool SoundResource::DetermineStreaming(const char *name)
{
	bool streaming = false;
	
	SoundResource *resource = Get(name, kResourceDeferLoad);
	if (resource)
	{
		ResourceLoader		loader;
		WaveHeader			header;
		
		if (resource->OpenLoader(&loader) == kResourceOkay)
		{
			if (resource->LoadWaveHeader(&loader, &header) == kResourceOkay)
			{
				streaming = (header.format == WAVE_FORMAT_ADPCM);
			}
		}
		
		resource->Release();
	}
	
	return (streaming);
}


SoundLoader::SoundLoader()
{
	soundResource = nullptr;
}

SoundLoader::~SoundLoader()
{
	if (soundResource)
	{
		soundResource->CloseLoader(this);
		soundResource->Release();
	}
}

SoundResult SoundLoader::Open(const char *name)
{
	soundResource = SoundResource::Get(name, kResourceDeferLoad);
	if (!soundResource) return (kSoundLoadFailed);
	
	if (soundResource->OpenLoader(this) != kResourceOkay)
	{
		soundResource->Release();
		soundResource = nullptr;
		return (kSoundLoadFailed);
	}
	
	frameCount = 0;
	waveDataSize = 0;
	
	bool adpcmFlag = false;
	
	unsigned_int32 position = 12;
	unsigned_int32 dataSize = GetDataSize();
	while (position < dataSize)
	{
		RIFFChunkHeader		chunkHeader;
		
		Read(&chunkHeader, position, sizeof(RIFFChunkHeader));
		position += sizeof(RIFFChunkHeader);
		
		#if C4LITTLEENDIAN
		
			Reverse(&chunkHeader.type);
		
		#endif
		
		#if C4BIGENDIAN
		
			Reverse(&chunkHeader.size);
		
		#endif
		
		if (chunkHeader.type == 'fmt ')
		{
			adpcmFlag = true;
			Read(&adpcmHeader, position, Min(chunkHeader.size, sizeof(ADPCMWaveHeader)));
			
			#if C4BIGENDIAN
			
				Reverse(&adpcmHeader);
			
			#endif
			
			if (adpcmHeader.format != WAVE_FORMAT_ADPCM) return (kSoundFormatInvalid);
		}
		else if (chunkHeader.type == 'fact')
		{
			Read(&frameCount, position, 4);
			
			#if C4BIGENDIAN
			
				Reverse(&frameCount);
			
			#endif
		}
		else if (chunkHeader.type == 'data')
		{
			startPosition = position;
			waveDataSize = chunkHeader.size;
		}
		
		position += chunkHeader.size;
	}
	
	if (adpcmFlag)
	{
		int32 blockSize = (((adpcmHeader.blockFrameCount - 2) >> 1) + 7) * adpcmHeader.numChannels;
		int32 count = waveDataSize / blockSize * adpcmHeader.blockFrameCount;
		
		if (frameCount == 0) frameCount = count;
		else frameCount = Min(frameCount, count);
	}
	
	return (kSoundOkay);
}


SoundStreamer::SoundStreamer()
{
	streamerState = 0;
	workBuffer = nullptr;
}

SoundStreamer::~SoundStreamer()
{
	delete[] workBuffer;
}

void SoundStreamer::AllocateStreamMemory(unsigned_int32 workSize, unsigned_int32 streamSize)
{
	delete[] workBuffer;
	
	workBufferSize = workSize;
	streamBufferSize = streamSize;
	
	unsigned_int32 size = sizeof(StreamBufferHeader) + ((streamSize + 3) & ~3);
	workBuffer = new char[workSize + size * 2];
	streamBuffer[0] = reinterpret_cast<StreamBufferHeader *>(workBuffer + workSize);
	streamBuffer[1] = reinterpret_cast<StreamBufferHeader *>(workBuffer + workSize + size);
}

int32 SoundStreamer::GetTotalFrameCount(void)
{
	return (0);
}

SoundResult SoundStreamer::StartStreamComponent(int32 index)
{
	return (StartStream());
}


WaveStreamer::WaveStreamer()
{
	currentSoundLoader = nullptr;
}

WaveStreamer::~WaveStreamer()
{
}

int32 WaveStreamer::DecompressMonoBlock(const char *input, Sample *output, int32 frameCount, const PredictorCoefficient *coefficient)
{
	int32 predictor = input[0];
	int32 c1 = coefficient[predictor].c1;
	int32 c2 = coefficient[predictor].c2;
	
	int32 delta = (input[2] << 8) | reinterpret_cast<const unsigned_int8 *>(input)[1];
	int32 sample1 = (input[4] << 8) | reinterpret_cast<const unsigned_int8 *>(input)[3];
	int32 sample2 = (input[6] << 8) | reinterpret_cast<const unsigned_int8 *>(input)[5];
	
	WriteLittleEndianS16(output, (Sample) sample2);
	WriteLittleEndianS16(++output, (Sample) sample1);
	
	input += 7;
	
	for (machine a = frameCount - 2; a > 0; a--)
	{
		int32 byte = *input++;
		int32 code = byte >> 4;
		
		int32 prediction = (sample1 * c1 + sample2 * c2) >> 8;
		sample2 = sample1;
		sample1 = Min(Max(code * delta + prediction, -32768), 32767);
		WriteLittleEndianS16(++output, (Sample) sample1);
		
		delta = Max((adpcmTable[code & 0x0F] * delta) >> 8, 16);
		
		if (--a <= 0) break;
		code = byte << 28 >> 28;
		
		prediction = (sample1 * c1 + sample2 * c2) >> 8;
		sample2 = sample1;
		sample1 = Min(Max(code * delta + prediction, -32768), 32767);
		WriteLittleEndianS16(++output, (Sample) sample1);
		
		delta = Max((adpcmTable[code & 0x0F] * delta) >> 8, 16);
	}
	
	return (frameCount);
}

int32 WaveStreamer::DecompressStereoBlock(const char *input, Sample *output, int32 frameCount, const PredictorCoefficient *coefficient)
{
	int32 predictorLeft = input[0];
	int32 predictorRight = input[1];
	int32 c1Left = coefficient[predictorLeft].c1;
	int32 c2Left = coefficient[predictorLeft].c2;
	int32 c1Right = coefficient[predictorRight].c1;
	int32 c2Right = coefficient[predictorRight].c2;
	
	int32 deltaLeft = (input[3] << 8) | reinterpret_cast<const unsigned_int8 *>(input)[2];
	int32 deltaRight = (input[5] << 8) | reinterpret_cast<const unsigned_int8 *>(input)[4];
	int32 sample1Left = (input[7] << 8) | reinterpret_cast<const unsigned_int8 *>(input)[6];
	int32 sample1Right = (input[9] << 8) | reinterpret_cast<const unsigned_int8 *>(input)[8];
	int32 sample2Left = (input[11] << 8) | reinterpret_cast<const unsigned_int8 *>(input)[10];
	int32 sample2Right = (input[13] << 8) | reinterpret_cast<const unsigned_int8 *>(input)[12];
	
	WriteLittleEndianS16(output, (Sample) sample2Left);
	WriteLittleEndianS16(++output, (Sample) sample2Right);
	WriteLittleEndianS16(++output, (Sample) sample1Left);
	WriteLittleEndianS16(++output, (Sample) sample1Right);
	
	input += 14;
	
	for (machine a = frameCount - 2; a > 0; a--)
	{
		int32 byte = *input++;
		int32 codeLeft = byte >> 4;
		int32 codeRight = byte << 28 >> 28;
		
		int32 prediction = (sample1Left * c1Left + sample2Left * c2Left) >> 8;
		sample2Left = sample1Left;
		sample1Left = Min(Max(codeLeft * deltaLeft + prediction, -32768), 32767);
		WriteLittleEndianS16(++output, (Sample) sample1Left);
		
		prediction = (sample1Right * c1Right + sample2Right * c2Right) >> 8;
		sample2Right = sample1Right;
		sample1Right = Min(Max(codeRight * deltaRight + prediction, -32768), 32767);
		WriteLittleEndianS16(++output, (Sample) sample1Right);
		
		deltaLeft = Max((adpcmTable[codeLeft & 0x0F] * deltaLeft) >> 8, 16);
		deltaRight = Max((adpcmTable[codeRight & 0x0F] * deltaRight) >> 8, 16);
	}
	
	return (frameCount);
}

SoundResult WaveStreamer::AddComponent(const char *name)
{
	SoundLoader *soundLoader = new SoundLoader;
	SoundResult result = soundLoader->Open(name);
	if (result != kSoundOkay)
	{
		delete soundLoader;
		return (result);
	}
	
	soundLoaderList.Append(soundLoader);
	
	if (!soundLoader->Previous())
	{
		const ADPCMWaveHeader *header = soundLoader->GetADPCMWaveHeader();
		
		int32 channelCount = header->numChannels;
		SetChannelCount(channelCount);
		SetSampleRate(header->sampleRate);
		
		blockFrameCount = header->blockFrameCount;
		compressedBlockSize = (((blockFrameCount - 2) >> 1) + 7) * channelCount;
		decompressedBlockSize = blockFrameCount * channelCount * sizeof(Sample);
		bufferBlockCount = kStreamBufferSize / decompressedBlockSize;
		decompressor = (channelCount == 1) ? &DecompressMonoBlock : &DecompressStereoBlock;
		
		AllocateStreamMemory(compressedBlockSize * bufferBlockCount, decompressedBlockSize * bufferBlockCount);
	}
	
	return (kSoundOkay);
}

int32 WaveStreamer::GetTotalFrameCount(void)
{
	int32 count = 0;
	
	SoundLoader *loader = soundLoaderList.First();
	while (loader)
	{
		count += loader->GetFrameCount();
		loader = loader->Next();
	}
	
	return (count);
}

SoundResult WaveStreamer::StartStream(void)
{
	currentSoundLoader = soundLoaderList.First();
	if (currentSoundLoader)
	{
		streamPosition = currentSoundLoader->GetStartPosition();
		frameNumber = 0;
		return (kSoundOkay);
	}
	
	return (kSoundPlayFailed);
}

SoundResult WaveStreamer::StartStreamComponent(int32 index)
{
	currentSoundLoader = soundLoaderList[index];
	if (currentSoundLoader)
	{
		streamPosition = currentSoundLoader->GetStartPosition();
		frameNumber = 0;
		return (kSoundOkay);
	}
	
	return (kSoundPlayFailed);
}

bool WaveStreamer::FillBuffer(unsigned_int32 bufferSize, Sample *buffer, int32 *count)
{
	bool result = true;
	int32 totalCount = 0;
	
	unsigned_int32 outputPosition = 0;
	for (;;)
	{
		int32 frameCount = currentSoundLoader->GetFrameCount();
		int32 blockCount = bufferSize / decompressedBlockSize;
		
		char *workBuffer = GetWorkBuffer();
		unsigned_int32 remainingSize = currentSoundLoader->GetWaveDataSize() + currentSoundLoader->GetStartPosition() - streamPosition;
		unsigned_int32 size = Min(compressedBlockSize * blockCount, remainingSize);
		currentSoundLoader->Read(workBuffer, streamPosition, size);
		streamPosition += size;
		
		const PredictorCoefficient *coefficient = currentSoundLoader->GetADPCMWaveHeader()->coefficient;
		
		unsigned_int32 inputPosition = 0;
		for (machine a = 0; a < blockCount; a++)
		{
			int32 count = (*decompressor)(&workBuffer[inputPosition], buffer + outputPosition / sizeof(Sample), Min(blockFrameCount, frameCount - frameNumber), coefficient);
			totalCount += count;
			
			outputPosition += decompressedBlockSize;
			if ((frameNumber += count) >= frameCount) break;
			
			inputPosition += compressedBlockSize;
		}
		
		if (frameNumber < frameCount) break;
		
		SoundLoader *loader = currentSoundLoader->Next();
		if (!loader)
		{
			result = false;
			break;
		}
		
		currentSoundLoader = loader;
		streamPosition = loader->GetStartPosition();
		bufferSize -= outputPosition;
		frameNumber = 0;
	}
	
	*count = totalCount;
	return (result);
}


SoundGroup::SoundGroup(SoundGroupType type, const char *name)
{
	soundGroupType = type;
	soundGroupName = name;
	soundGroupVolume = 1.0F;
}

SoundGroup::~SoundGroup()
{
}

void SoundGroup::SetVolume(float volume)
{
	soundGroupVolume = volume;
	
	Sound *sound = TheSoundMgr->loadedSoundList.First();
	while (sound)
	{
		sound->updateFlags |= kSoundUpdateVolume;
		sound = sound->Next();
	}
}


SoundRoom::SoundRoom(const Vector3D& size)
{
	tableIndex = -1;
	maxRoomMixCount = 0;
	
	roomSize = size;
	reflectionVolume = 0.0F;
	reflectionHFVolume = 1.0F;
	mediumHFAbsorption = 1.0F;
	reverbVolume = 0.0F;
	
	roomVolume[0] = 0.0F;
	roomVolume[1] = 0.0F;
}

SoundRoom::~SoundRoom()
{
}

void SoundRoom::Release(void)
{
	TheSoundMgr->releasedRoomList.Append(this);
}

void SoundRoom::SetReverbDecayTime(float time)
{
	if (time != 0.0F)
	{
		float count = time / kMaxFeedbackTimeDelay;
		reverbVolume = Pow(kMixFractionReciprocal, 1.0F / count);
		maxRoomMixCount = Min((int32) PositiveCeil(count), kMaxFeedbackMixCount);
	}
	else
	{
		reverbVolume = 0.0F;
		maxRoomMixCount = 0;
	}
}

void SoundRoom::SetRoomPosition(const Point3D& position)
{
	roomPosition = position;
	
	Point3D p = TheSoundMgr->GetListenerTransformable()->GetInverseWorldTransform() * position;
	float inverseDistance = InverseMag(p);
	float m = Fmin(inverseDistance, 1.0F);
	
	float d = p.x * inverseDistance;
	if (d > 0.0F)
	{
		roomVolume[0] = m * (1.0F - d);
		roomVolume[1] = m;
	}
	else
	{
		roomVolume[0] = m;
		roomVolume[1] = m * (1.0F + d);
	}
}


Sound::Sound()
{
	soundResource = nullptr;
	soundStreamer = nullptr;
	
	soundFlags = kSoundPersistent;
	soundState = kSoundUnloaded;
	tableIndex = -1;
	releaseFlag = false;
	
	startTime = 0;
	loopCount = 0;
	loopIndex = 0;
	loopProc = nullptr;
	
	fadeFlags = 0;
	fadeProc = nullptr;
	
	soundProperty[kSoundVolume] = 1.0F;
	soundProperty[kSoundDirectVolume] = 1.0F;
	soundProperty[kSoundDirectHFVolume] = 1.0F;
	soundProperty[kSoundReflectionVolume] = 1.0F;
	soundProperty[kSoundReflectionHFVolume] = 1.0F;
	soundProperty[kSoundOuterConeVolume] = 0.0F;
	soundProperty[kSoundOuterConeHFVolume] = 1.0F;
	soundProperty[kSoundMinAttenDistance] = 0.0F;
	soundProperty[kSoundMaxAttenDistance] = 16.0F;
	soundProperty[kSoundInnerConeCosine] = 1.0F;
	soundProperty[kSoundOuterConeCosine] = 0.0F;
	soundProperty[kSoundFrequency] = 1.0F;
	
	soundTransformable = nullptr;
	soundVelocity.Set(0.0F, 0.0F, 0.0F);
	soundPathCount = 0;
	
	soundGroup = TheSoundMgr->GetDefaultSoundGroup();
}

Sound::~Sound()
{
	if (soundResource) soundResource->Release();
	delete soundStreamer;
}

void Sound::Release(void)
{
	if (tableIndex < 0)
	{
		delete this;
	}
	else
	{
		mixRelease = false;
		streamRelease = (soundStreamer == nullptr);
		releaseFlag = true;
	}
}

SoundResult Sound::Load(const char *name)
{
	delete soundStreamer;
	soundStreamer = nullptr;
	
	soundResource = SoundResource::Get(name);
	if (!soundResource) return (kSoundLoadFailed);
	
	const WaveHeader *header = soundResource->GetWaveHeader();
	if ((header->format != WAVE_FORMAT_PCM) || (header->bitsPerSample != 16))
	{
		soundResource->Release();
		return (kSoundFormatInvalid);
	}
	
	channelCount = header->numChannels;
	sampleRate = header->sampleRate;
	sampleFrequency = sampleRate / (float) kSoundOutputSampleRate;
	
	soundSampleData = soundResource->GetSampleData();
	soundFrameCount = soundResource->GetSampleCount() / channelCount;
	if (soundFrameCount >= 0x00200000) return (kSoundTooLarge);
	
	soundState = kSoundStopped;
	soundFlags &= ~kSoundPersistent;
	
	TheSoundMgr->loadedSoundList.Append(this);
	return (kSoundOkay);
}

SoundResult Sound::Stream(SoundStreamer *streamer)
{
	delete soundStreamer;
	soundStreamer = streamer;
	
	soundState = kSoundStopped;
	soundFlags |= kSoundPersistent;
	
	TheSoundMgr->loadedSoundList.Append(this);
	return (kSoundOkay);
}

void Sound::FillStreamBuffer(SoundStreamer *streamer, StreamBufferHeader *buffer)
{
	buffer->frameCount = 0;
	buffer->loopFrame = 0x7FFFFFFF;
	buffer->finalFlag = false;
	
	unsigned_int32 bufferSize = streamer->GetStreamBufferSize();
	Sample *sampleData = buffer->GetSampleData();
	
	for (;;)
	{
		int32	frameCount;
		
		bool result = soundStreamer->FillBuffer(bufferSize, sampleData, &frameCount);
		buffer->frameCount += frameCount;
		
		if (!result)
		{
			if (loopCount > 0) loopCount--;
			if (loopCount != 0)
			{
				if (soundStreamer->StartStreamComponent(loopIndex) != kSoundOkay) soundStreamer->StartStreamComponent(0);
				
				buffer->loopFrame = buffer->frameCount;
				bufferSize -= frameCount * channelCount * sizeof(Sample);
				sampleData += frameCount * channelCount;
				if (bufferSize != 0) continue;
			}
			else
			{
				buffer->finalFlag = true;
			}
		}
		
		break;
	}
	
	buffer->readyFlag = true;
}

SoundResult Sound::Play(void)
{
	SoundResult result = kSoundOkay;
	
	#if !C4SERVER
	
		if (soundState < kSoundPlaying)
		{
			if (soundState != kSoundDelaying) fadeFlags = 0;
			
			mixFlag = false;
			loopFlag = false;
			pauseCount = 0;
			playFrame = 0;
			
			if (soundStreamer)
			{
				result = soundStreamer->StartStream();
				if (result != kSoundOkay) return (result);
				
				channelCount = soundStreamer->GetChannelCount();
				sampleRate = soundStreamer->GetSampleRate();
				sampleFrequency = sampleRate / (float) kSoundOutputSampleRate;
				
				playBuffer = 0;
				FillStreamBuffer(soundStreamer, soundStreamer->GetStreamBuffer(0));
				FillStreamBuffer(soundStreamer, soundStreamer->GetStreamBuffer(1));
				
				if (channelCount == 1)
				{
					soundMixData.sampleTableIndex = 0;
					float sample = (float) ReadLittleEndianS16(soundStreamer->GetStreamBuffer(0)->GetSampleData());
					soundMixData.sampleTableSum = sample * (float) kSampleHistoryCount;
					for (machine a = 0; a < kSampleHistoryCount; a++) soundMixData.sampleTable[a] = sample;
				}
			}
			else
			{
				if (startTime != 0) playFrame = Min(startTime * sampleRate / 1000, soundFrameCount - 1);
				
				if (channelCount == 1)
				{
					soundMixData.sampleTableIndex = 0;
					float sample = (float) ReadLittleEndianS16(&soundSampleData[playFrame]);
					soundMixData.sampleTableSum = sample * (float) kSampleHistoryCount;
					for (machine a = 0; a < kSampleHistoryCount; a++) soundMixData.sampleTable[a] = sample;
					
					if (((soundFlags & (kSoundSpatialized | kSoundDistanceDelay)) == (kSoundSpatialized | kSoundDistanceDelay)) && (playFrame == 0))
					{
						float distance = Magnitude(soundTransformable->GetWorldPosition() - TheSoundMgr->GetListenerTransformable()->GetWorldPosition());
						playFrame = (int32) Fmax(distance / TheSoundMgr->GetGlobalSoundSpeed() * sampleFrequency * -(float) kSoundOutputSampleRate, kMinDistanceDelayPlayFrame);
					}
				}
			}
			
			updateFlags = 0;
			
			UpdateVolume();
			soundMixData.directVolumeCurrent[0] = soundMixData.directVolumeFinal[0];
			soundMixData.directVolumeCurrent[1] = soundMixData.directVolumeFinal[1];
			soundMixData.reflectionVolumeCurrent = soundMixData.reflectionVolumeFinal;
			
			UpdateFrequency();
			soundMixData.frequencyCurrent = soundMixData.frequencyFinal;
			
			UpdateReflections();
			
			if (tableIndex < 0)
			{
				int32 index = TheSoundMgr->AddSound(this);
				if (index >= 0)
				{
					tableIndex = index;
					soundState = kSoundPlaying;
				}
				else
				{
					soundState = kSoundCompleted;
					result = kSoundPlayFailed;
				}
			}
			else
			{
				soundState = kSoundPlaying;
			}
		}
	
	#else
	
		soundState = kSoundCompleted;
	
	#endif
	
	return (result);
}

void Sound::Stop(void)
{
	fadeFlags = 0;
	
	if (soundFlags & kSoundPersistent)
	{
		if (tableIndex < 0) soundState = kSoundStopped;
		else soundState = kSoundStopping;
	}
	else
	{
		Release();
	}
}

void Sound::Delay(int32 time)
{
	delayTime = time;
	playFrame = 0;
	fadeFlags = 0;
	soundState = kSoundDelaying;
}

void Sound::Pause(void)
{
	if (soundState == kSoundPlaying)
	{
		if (++pauseCount == 1) soundState = kSoundPaused;
	}
	else if (soundState == kSoundDelaying)
	{
		if (++pauseCount == 1) soundState = kSoundDelayPaused;
	}
}

void Sound::Resume(void)
{
	if (soundState == kSoundPaused)
	{
		if (--pauseCount == 0) soundState = kSoundPlaying;
	}
	else if (soundState == kSoundDelayPaused)
	{
		soundState = kSoundDelaying;
	}
}

void Sound::Fade(float targetVolume, int32 fadeTime, bool endStop)
{
	if ((soundState >= kSoundDelaying) && (soundState <= kSoundPlaying))
	{
		if (fadeTime == 0)
		{
			SetSoundProperty(kSoundVolume, targetVolume);
			if (fadeProc) (*fadeProc)(this, fadeCookie);
			if (endStop) Stop();
		}
		else
		{
			unsigned_int32 flags = kSoundFadeActive;
			if (endStop) flags |= kSoundFadeEndStop;
			fadeFlags = flags;
			
			fadeVolume = targetVolume;
			fadeDelta = (targetVolume - soundProperty[kSoundVolume]) / (float) fadeTime;
		}
	}
}

inline float Sound::CalculateVolume(float distance) const
{
	float minAttenDistance = soundProperty[kSoundMinAttenDistance];
	float maxAttenDistance = soundProperty[kSoundMaxAttenDistance];
	float d = FmaxZero(Fmin(distance, maxAttenDistance) - minAttenDistance) / (maxAttenDistance - minAttenDistance);
	
	d -= 1.0F;
	d *= d;
	return (d * d);
}

void Sound::UpdateVolume(void)
{
	float masterVolume = soundProperty[kSoundVolume] * TheSoundMgr->GetMasterVolume();
	const SoundGroup *group = soundGroup;
	if (group) masterVolume *= group->GetVolume();
	
	if (soundFlags & kSoundSpatialized)
	{
		float	directHFVolume;
		float	speakerVolume[2];
		
		const SoundRoom *room = soundRoom;
		const Transformable *listenerTransformable = TheSoundMgr->GetListenerTransformable();
		
		int32 pathCount = soundPathCount;
		if (pathCount > 0)
		{
			directHFVolume = (room) ? 0.0F : 1.0F;
			speakerVolume[0] = 0.0F;
			speakerVolume[1] = 0.0F;
			
			machine pathIndex = 0;
			do
			{
				const SoundPathData *data = &soundPathData[pathIndex];
				
				Point3D position = listenerTransformable->GetInverseWorldTransform() * *data->soundPosition;
				float squaredDistance = SquaredMag(position);
				float inverseDistance = InverseSqrt(squaredDistance);
				
				float distance = squaredDistance * inverseDistance + data->soundPathLength;
				float volume = CalculateVolume(distance);
				
				float d = position.x * inverseDistance;
				if (d > 0.0F)
				{
					speakerVolume[0] = Fmax(speakerVolume[0], FmaxZero(1.0F - d) * volume);
					speakerVolume[1] = Fmax(speakerVolume[1], volume);
				}
				else
				{
					speakerVolume[0] = Fmax(speakerVolume[0], volume);
					speakerVolume[1] = Fmax(speakerVolume[1], FmaxZero(1.0F + d) * volume);
				}
				
				if (room) directHFVolume = Fmax(directHFVolume, Fmin(Pow(room->GetMediumHFAbsorption(), distance), 1.0F));
				
			} while (++pathIndex < pathCount);
		}
		else
		{
			Point3D position = listenerTransformable->GetInverseWorldTransform() * soundTransformable->GetWorldPosition();
			
			float r2 = Fmax(SquaredMag(position), K::min_float);
			float inverseDistance = InverseSqrt(r2);
			float distance = r2 * inverseDistance;
			float volume = CalculateVolume(distance);
			
			float d = position.x * inverseDistance;
			if (d > 0.0F)
			{
				speakerVolume[0] = FmaxZero(1.0F - d) * volume;
				speakerVolume[1] = volume;
			}
			else
			{
				speakerVolume[0] = volume;
				speakerVolume[1] = FmaxZero(1.0F + d) * volume;
			}
			
			directHFVolume = (room) ? Fmin(Pow(room->GetMediumHFAbsorption(), distance), 1.0F) : 1.0F;
		}
		
		float directVolume = masterVolume * soundProperty[kSoundDirectVolume];
		
		if (soundFlags & kSoundCones)
		{
			Point3D conePosition = soundTransformable->GetInverseWorldTransform() * listenerTransformable->GetWorldPosition();
			float cosine = conePosition.z * InverseMag(conePosition);
			
			float inner = soundProperty[kSoundInnerConeCosine];
			if (cosine < inner)
			{
				float outer = soundProperty[kSoundOuterConeCosine];
				if (cosine < outer)
				{
					directVolume *= soundProperty[kSoundOuterConeVolume];
					directHFVolume *= soundProperty[kSoundOuterConeHFVolume];
				}
				else
				{
					float t = (inner - cosine) / (inner - outer);
					directVolume *= (soundProperty[kSoundOuterConeVolume] - 1.0F) * t + 1.0F;
					directHFVolume *= (soundProperty[kSoundOuterConeHFVolume] - 1.0F) * t + 1.0F;
				}
			}
		}
		
		soundMixData.directVolumeFinal[0] = directVolume * speakerVolume[0];
		soundMixData.directVolumeFinal[1] = directVolume * speakerVolume[1];
		soundMixData.directHFVolume = directHFVolume * soundProperty[kSoundDirectHFVolume];
		
		soundMixData.reflectionVolumeFinal = masterVolume * soundProperty[kSoundReflectionVolume];
		soundMixData.reflectionHFVolume = soundProperty[kSoundReflectionHFVolume];
	}
	else
	{
		soundMixData.directVolumeFinal[0] = masterVolume;
		soundMixData.directVolumeFinal[1] = masterVolume;
	}
}

void Sound::UpdateFrequency(void)
{
	float frequency = soundProperty[kSoundFrequency] * sampleFrequency;
	if ((soundFlags & (kSoundSpatialized | kSoundDopplerShift)) == (kSoundSpatialized | kSoundDopplerShift))
	{
		const Transformable *listenerTransformable = TheSoundMgr->GetListenerTransformable();
		if ((listenerTransformable) && (soundTransformable))
		{
			Vector3D dp = soundTransformable->GetWorldPosition() - listenerTransformable->GetWorldPosition();
			dp.Normalize();
			
			float c = TheSoundMgr->GetGlobalSoundSpeed();
			float doppler = (c + dp * TheSoundMgr->GetListenerVelocity()) / (c + dp * soundVelocity);
			frequency *= Clamp(doppler, 0.25F, 4.0F);
		}
	}
	
	soundMixData.frequencyFinal = frequency;
}

void Sound::UpdateReflections(void)
{
	soundMixData.reflectionData[0].soundRoom = nullptr;
	soundMixData.reflectionData[1].soundRoom = nullptr;
	
	SoundRoom *primaryRoom = soundRoom;
	if (primaryRoom)
	{
		static const float reflectionDistance[kSoundReflectionCount] =
		{
			0.25F, 0.1625F, 0.2075F, 0.3075F, 0.3925F, 0.4725F
		};
		
		soundMixData.reflectionData[0].soundRoom = primaryRoom;
		
		const Vector3D& size1 = primaryRoom->GetRoomSize();
		float x2 = size1.x * size1.x;
		float y2 = size1.y * size1.y;
		float z2 = size1.z * size1.z;
		float d = Sqrt(x2 + y2 + z2) * 1.5F;
		
		float unitDistanceFrameCount = TheSoundMgr->unitDistanceFrameCount;
		float rv = primaryRoom->GetReflectionVolume();
		float rhfv = primaryRoom->GetReflectionHFVolume();
		
		SoundMixData::ReflectionData *data = &soundMixData.reflectionData[0];
		for (machine a = 0; a < kSoundReflectionCount; a++)
		{
			float distance = d * reflectionDistance[a];
			data->reflectionDelay[a] = Min((int32) (distance * unitDistanceFrameCount), kRingBufferFrameCount - 1);
			data->reflectionVolume[a] = CalculateVolume(distance) * rv;
			data->reflectionHFVolume[a] = rhfv;
		}
		
		SoundRoom *secondaryRoom = primaryRoom->GetOutputRoom();
		if (secondaryRoom)
		{
			soundMixData.reflectionData[1].soundRoom = secondaryRoom;
			
			const Vector3D& size2 = secondaryRoom->GetRoomSize();
			x2 = size2.x * size2.x;
			y2 = size2.y * size2.y;
			z2 = size2.z * size2.z;
			d = Sqrt(x2 + y2 + z2) * 0.5F;
			
			rv = secondaryRoom->GetReflectionVolume();
			rhfv = secondaryRoom->GetReflectionHFVolume();
			float primaryDistance = Magnitude(primaryRoom->GetRoomPosition() - soundTransformable->GetWorldPosition());
			
			data = &soundMixData.reflectionData[1];
			for (machine a = 0; a < kSoundReflectionCount; a++)
			{
				float distance = primaryDistance + d * reflectionDistance[a];
				data->reflectionDelay[a] = Min((int32) (distance * unitDistanceFrameCount), kRingBufferFrameCount - 1);
				data->reflectionVolume[a] = CalculateVolume(distance) * rv;
				data->reflectionHFVolume[a] = rhfv;
			}
		}
	}
}

int32 Sound::GetDuration(void) const
{
	int32 count = (soundStreamer) ? soundStreamer->GetTotalFrameCount() : soundFrameCount;
	return ((int32) ((float) count * 1000.0F / (sampleFrequency * (float) kSoundOutputSampleRate)));
}


SoundMgr::SoundMgr(int) : soundReverbObserver(this, &SoundMgr::HandleSoundReverbEvent)
{
}

SoundMgr::~SoundMgr()
{
}

EngineResult SoundMgr::Construct(void)
{
	soundOptionFlags = kSoundReverb;
	
	masterVolume = 1.0F;
	SetGlobalSoundSpeed(343.0F);
	
	listenerTransformable = nullptr;
	listenerVelocity.Set(0.0F, 0.0F, 0.0F);
	listenerRoom = nullptr;
	
	ringBufferSliceIndex = 0;
	
	char *bufferStorage = new char[kRingBufferFrameCount * sizeof(StereoMixFrame) + kMaxRoomCount * sizeof(RoomMixBuffer)];
	MemoryMgr::ClearMemory(bufferStorage, kRingBufferFrameCount * sizeof(StereoMixFrame));
	stereoRingBuffer = reinterpret_cast<StereoMixFrame *>(bufferStorage);
	
	RoomMixBuffer *buffer = reinterpret_cast<RoomMixBuffer *>(stereoRingBuffer + kRingBufferFrameCount);
	for (machine a = 0; a < kMaxRoomCount; a++)
	{
		roomMixBuffer[a] = &buffer[a];
		activeRoomTable[a] = nullptr;
	}
	
	for (machine a = 0; a < kMaxSoundCount; a++) activeSoundTable[a] = nullptr;
	
	TheEngine->InitVariable("soundReverb", "1", kVariablePermanent, &soundReverbObserver);
	
	#if !C4SERVER
	
		#if C4XAUDIO
		
			WAVEFORMATEX	format;
			
			if (FAILED(XAudio2Create( &xaudioObject, XAUDIO2_DEBUG_ENGINE)))
			{
				delete[] bufferStorage;
				return (kSoundInitFailed);
			}
			XAUDIO2_DEBUG_CONFIGURATION debugSet;
			debugSet.TraceMask =XAUDIO2_LOG_ERRORS ;

			debugSet.LogFileline = TRUE;
			debugSet.LogFunctionName = TRUE;
			debugSet.LogThreadID = TRUE;
			debugSet.LogTiming = TRUE;
			xaudioObject->SetDebugConfiguration(&debugSet);
			
			if (FAILED(xaudioObject->CreateMasteringVoice(&masteringVoice, 2, kSoundOutputSampleRate)))
			{
				xaudioObject->Release();
				delete[] bufferStorage;
				return (kSoundInitFailed);
			}
			
			voiceCallback.soundMgr = this;
			
			format.wFormatTag = WAVE_FORMAT_PCM;
			format.nChannels = 2;
			format.nSamplesPerSec = kSoundOutputSampleRate;
			format.nAvgBytesPerSec = kSoundOutputSampleRate * 4;
			format.nBlockAlign = 4;
			format.wBitsPerSample = 16;
			format.cbSize = 0;
			
			if (FAILED(xaudioObject->CreateSourceVoice(&sourceVoice, &format, XAUDIO2_VOICE_NOPITCH, 1.0F, &voiceCallback)))
			{
				masteringVoice->DestroyVoice();
				xaudioObject->Release();
				delete[] bufferStorage;
				return (kSoundInitFailed);
			}
			
			playBuffer[0] = new OutputSample[kOutputBufferFrameCount * 4];
			playBuffer[1] = playBuffer[0] + kOutputBufferFrameCount * 2;
			MemoryMgr::ClearMemory(playBuffer[0], kOutputBufferFrameCount * 4 * sizeof(OutputSample));
			
			soundSignal = new Signal(3);
			soundThread = new Thread(&SoundThread, this, 0, soundSignal);
			soundThread->SetThreadPriority(kThreadPriorityCritical);
			
			sourceBuffer[0].Flags = 0;
			sourceBuffer[0].AudioBytes = kStereoOutputBufferSize;
			sourceBuffer[0].pAudioData = reinterpret_cast<BYTE *>(playBuffer[0]);
			sourceBuffer[0].PlayBegin = 0;
			sourceBuffer[0].PlayLength = 0;
			sourceBuffer[0].LoopBegin = XAUDIO2_NO_LOOP_REGION;
			sourceBuffer[0].LoopLength = 0;
			sourceBuffer[0].LoopCount = 0;
			sourceBuffer[0].pContext = &sourceBuffer[0];
			
			sourceBuffer[1].Flags = 0;
			sourceBuffer[1].AudioBytes = kStereoOutputBufferSize;
			sourceBuffer[1].pAudioData = reinterpret_cast<BYTE *>(playBuffer[1]);
			sourceBuffer[1].PlayBegin = 0;
			sourceBuffer[1].PlayLength = 0;
			sourceBuffer[1].LoopBegin = XAUDIO2_NO_LOOP_REGION;
			sourceBuffer[1].LoopLength = 0;
			sourceBuffer[1].LoopCount = 0;
			sourceBuffer[1].pContext = &sourceBuffer[1];
			
			sourceVoice->SubmitSourceBuffer(&sourceBuffer[0]);
			sourceVoice->SubmitSourceBuffer(&sourceBuffer[1]);
			sourceVoice->Start(0);

		#elif C4OPENAL // add openal

	/*
	WAVEFORMATEX	format;
	if (FAILED(XAudio2Create( &xaudioObject, XAUDIO2_DEBUG_ENGINE)))
	{
	delete[] bufferStorage;
	return (kSoundInitFailed);
	}

	XAUDIO2_DEBUG_CONFIGURATION debugSet;
	debugSet.TraceMask =XAUDIO2_LOG_ERRORS ;

	debugSet.LogFileline = TRUE;
	debugSet.LogFunctionName = TRUE;
	debugSet.LogThreadID = TRUE;
	debugSet.LogTiming = TRUE;
	xaudioObject->SetDebugConfiguration(&debugSet);

	if (FAILED(xaudioObject->CreateMasteringVoice(&masteringVoice, 2, kSoundOutputSampleRate)))
	{
	xaudioObject->Release();
	delete[] bufferStorage;
	return (kSoundInitFailed);
	}

	voiceCallback.soundMgr = this;

	format.wFormatTag = WAVE_FORMAT_PCM;
	format.nChannels = 2;
	format.nSamplesPerSec = kSoundOutputSampleRate;
	format.nAvgBytesPerSec = kSoundOutputSampleRate * 4;
	format.nBlockAlign = 4;
	format.wBitsPerSample = 16;
	format.cbSize = 0;

	if (FAILED(xaudioObject->CreateSourceVoice(&sourceVoice, &format, XAUDIO2_VOICE_NOPITCH, 1.0F, &voiceCallback)))
	{
	masteringVoice->DestroyVoice();
	xaudioObject->Release();
	delete[] bufferStorage;
	return (kSoundInitFailed);
	}

	playBuffer[0] = new OutputSample[kOutputBufferFrameCount * 4];
	playBuffer[1] = playBuffer[0] + kOutputBufferFrameCount * 2;
	MemoryMgr::ClearMemory(playBuffer[0], kOutputBufferFrameCount * 4 * sizeof(OutputSample));

	soundSignal = new Signal(3);
	soundThread = new Thread(&SoundThread, this, 0, soundSignal);
	soundThread->SetThreadPriority(kThreadPriorityCritical);

	sourceBuffer[0].Flags = 0;
	sourceBuffer[0].AudioBytes = kStereoOutputBufferSize;
	sourceBuffer[0].pAudioData = reinterpret_cast<BYTE *>(playBuffer[0]);
	sourceBuffer[0].PlayBegin = 0;
	sourceBuffer[0].PlayLength = 0;
	sourceBuffer[0].LoopBegin = XAUDIO2_NO_LOOP_REGION;
	sourceBuffer[0].LoopLength = 0;
	sourceBuffer[0].LoopCount = 0;
	sourceBuffer[0].pContext = &sourceBuffer[0];

	sourceBuffer[1].Flags = 0;
	sourceBuffer[1].AudioBytes = kStereoOutputBufferSize;
	sourceBuffer[1].pAudioData = reinterpret_cast<BYTE *>(playBuffer[1]);
	sourceBuffer[1].PlayBegin = 0;
	sourceBuffer[1].PlayLength = 0;
	sourceBuffer[1].LoopBegin = XAUDIO2_NO_LOOP_REGION;
	sourceBuffer[1].LoopLength = 0;
	sourceBuffer[1].LoopCount = 0;
	sourceBuffer[1].pContext = &sourceBuffer[1];

	sourceVoice->SubmitSourceBuffer(&sourceBuffer[0]);
	sourceVoice->SubmitSourceBuffer(&sourceBuffer[1]);
	sourceVoice->Start(0);*/

		#endif



		
		streamSignal = new Signal(2);
		streamThread = new Thread(&StreamThread, this, 0, streamSignal);
	
	#endif
	
	return (kSoundOkay);
}

void SoundMgr::Destruct(void)
{
	#if !C4SERVER
	
		delete streamThread;
		delete streamSignal;
		
		#if C4XAUDIO
		
			delete soundThread;
			delete soundSignal;
			
			sourceVoice->Stop(0);
			delete[] playBuffer[0];
			
			sourceVoice->DestroyVoice();
			masteringVoice->DestroyVoice();
			xaudioObject->Release();
		
		#elif C4MACOS
		
			AUGraphStop(audioGraph);
			AudioOutputUnitStop(audioUnit);
			
			delete[] playBuffer[0];
			
			AUGraphUninitialize(audioGraph);
			AUGraphClose(audioGraph);
			DisposeAUGraph(audioGraph);
		
		#elif C4LINUX
		
			snd_pcm_drop(soundHandle);
			
			soundExitFlag = true;
			delete soundThread;
			
			delete[] playBuffer;
			
			snd_pcm_close(soundHandle);
		
		#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

		#endif //]
	
	#endif
	
	delete[] reinterpret_cast<char *>(stereoRingBuffer);
	
	releasedRoomList.Purge();
	loadedSoundList.Purge();
	
	TheResourceMgr->FlushCache(SoundResource::GetDescriptor());
}

#if C4XAUDIO && !C4SERVER

	void SoundMgr::VoiceCallback::OnVoiceProcessingPassStart(UINT32)
	{
	}
	
	void SoundMgr::VoiceCallback::OnVoiceProcessingPassEnd(void)
	{
	}
	
	void SoundMgr::VoiceCallback::OnStreamEnd(void)
	{
	}
	
	void SoundMgr::VoiceCallback::OnBufferStart(void *context)
	{
	}
	
	void SoundMgr::VoiceCallback::OnBufferEnd(void *context)
	{
		soundMgr->soundSignal->Trigger((context == &soundMgr->sourceBuffer[0]) ? 1 : 2);
	}
	
	void SoundMgr::VoiceCallback::OnLoopEnd(void *context)
	{
	}
	
	void SoundMgr::VoiceCallback::OnVoiceError(void *context, HRESULT)
	{
	}

#endif

void SoundMgr::HandleSoundReverbEvent(Variable *variable)
{
	unsigned_int32 flags = soundOptionFlags;
	
	if (variable->GetIntegerValue() != 0) flags |= kSoundOptionReverb;
	else flags &= ~kSoundOptionReverb;
	
	soundOptionFlags = flags;
}

int32 SoundMgr::AddSound(Sound *sound)
{
	for (machine a = 0; a < kMaxSoundCount; a++) if (!activeSoundTable[a])
	{
		activeSoundTable[a] = sound;
		return (a);
	}
	
	return (-1);
}

int32 SoundMgr::MixStereoSamples_Mono_Constant(SoundMixData *mixData, const Sample *input, int32 inputFrameCount, int32& inputOffset, int32 outputFrameCount, int32 outputOffset)
{
	float directVolumeLeft = mixData->directVolumeCurrent[0];
	float directVolumeRight = mixData->directVolumeCurrent[1];
	
	Fixed ds = (int32) (mixData->frequencyFinal * kMixFractionMultiplier);
	Fixed offset = inputOffset << kMixFractionSize;
	
	StereoMixFrame *output = stereoRingBuffer;
	
	int32 count = 0;
	do
	{
		int32 sample1 = ReadLittleEndianS16(&input[offset >> kMixFractionSize]);
		int32 sample2 = ReadLittleEndianS16(&input[Min((offset >> kMixFractionSize) + 1, inputFrameCount - 1)]);
		
		Fixed param = offset & kMixFractionMax;
		float sample = (float) (sample1 + (((sample2 - sample1) * param) >> kMixFractionSize));
		
		float leftSample = sample * directVolumeLeft;
		float rightSample = sample * directVolumeRight;
		
		output[outputOffset].left += leftSample;
		output[outputOffset].right += rightSample;
		
		directVolumeLeft += mixData->directVolumeDelta[0];
		directVolumeRight += mixData->directVolumeDelta[1];
		
		count++;
		offset += ds;
		if ((offset >> kMixFractionSize) >= inputFrameCount) break;
		
		outputOffset++;
		
	} while (count < outputFrameCount);
	
	mixData->directVolumeCurrent[0] = directVolumeLeft;
	mixData->directVolumeCurrent[1] = directVolumeRight;
	
	inputOffset = offset >> kMixFractionSize;
	return (count);
}

int32 SoundMgr::MixStereoSamples_Mono_Variable(SoundMixData *mixData, const Sample *input, int32 inputFrameCount, int32& inputOffset, int32 outputFrameCount, int32 outputOffset)
{
	float directVolumeLeft = mixData->directVolumeCurrent[0];
	float directVolumeRight = mixData->directVolumeCurrent[1];
	
	float ds = mixData->frequencyCurrent * kMixFractionMultiplier;
	float alpha = mixData->frequencyAlpha;
	Fixed offset = inputOffset << kMixFractionSize;
	
	StereoMixFrame *output = stereoRingBuffer;
	
	int32 count = 0;
	do
	{
		int32 sample1 = ReadLittleEndianS16(&input[offset >> kMixFractionSize]);
		int32 sample2 = ReadLittleEndianS16(&input[Min((offset >> kMixFractionSize) + 1, inputFrameCount - 1)]);
		
		Fixed param = offset & kMixFractionMax;
		float sample = (float) (sample1 + (((sample2 - sample1) * param) >> kMixFractionSize));
		
		float leftSample = sample * directVolumeLeft;
		float rightSample = sample * directVolumeRight;
		
		output[outputOffset].left += leftSample;
		output[outputOffset].right += rightSample;
		
		directVolumeLeft += mixData->directVolumeDelta[0];
		directVolumeRight += mixData->directVolumeDelta[1];
		
		count++;
		offset += (int32) ds;
		if ((offset >> kMixFractionSize) >= inputFrameCount) break;
		
		ds *= alpha;
		outputOffset++;
		
	} while (count < outputFrameCount);
	
	mixData->directVolumeCurrent[0] = directVolumeLeft;
	mixData->directVolumeCurrent[1] = directVolumeRight;
	mixData->frequencyCurrent = ds * kMixFractionReciprocal;
	
	inputOffset = offset >> kMixFractionSize;
	return (count);
}

int32 SoundMgr::MixStereoSamples_Mono_Constant_Dry(SoundMixData *mixData, const Sample *input, int32 inputFrameCount, int32& inputOffset, int32 outputFrameCount, int32 outputOffset)
{
	float directVolumeLeft = mixData->directVolumeCurrent[0];
	float directVolumeRight = mixData->directVolumeCurrent[1];
	
	Fixed ds = (int32) (mixData->frequencyFinal * kMixFractionMultiplier);
	Fixed offset = inputOffset << kMixFractionSize;
	
	int32 count = 0;
	if (offset < 0)
	{
		count = Min(-offset / ds, outputFrameCount);
		
		directVolumeLeft += mixData->directVolumeDelta[0] * count;
		directVolumeRight += mixData->directVolumeDelta[1] * count;
		
		if (count == outputFrameCount)
		{
			mixData->directVolumeCurrent[0] = directVolumeLeft;
			mixData->directVolumeCurrent[1] = directVolumeRight;
			
			inputOffset = (offset + ds * outputFrameCount) >> kMixFractionSize;
			return (outputFrameCount);
		}
		else
		{
			offset = 0;
			outputOffset += count;
		}
	}
	
	StereoMixFrame *output = stereoRingBuffer;
	
	unsigned_int32 index = mixData->sampleTableIndex;
	do
	{
		int32 sample1 = ReadLittleEndianS16(&input[offset >> kMixFractionSize]);
		int32 sample2 = ReadLittleEndianS16(&input[Min((offset >> kMixFractionSize) + 1, inputFrameCount - 1)]);
		
		Fixed param = offset & kMixFractionMax;
		float sample = (float) (sample1 + (((sample2 - sample1) * param) >> kMixFractionSize));
		
		float *tableSample = &mixData->sampleTable[index];
		index = (index + 1) & (kSampleHistoryCount - 1);
		
		float sum = mixData->sampleTableSum - *tableSample + sample;
		mixData->sampleTableSum = sum;
		*tableSample = sample;
		
		sum *= 1.0F / (float) kSampleHistoryCount;
		sample = sum + (sample - sum) * mixData->directHFVolume;
		
		float leftSample = sample * directVolumeLeft;
		float rightSample = sample * directVolumeRight;
		
		output[outputOffset].left += leftSample;
		output[outputOffset].right += rightSample;
		
		directVolumeLeft += mixData->directVolumeDelta[0];
		directVolumeRight += mixData->directVolumeDelta[1];
		
		count++;
		offset += ds;
		if ((offset >> kMixFractionSize) >= inputFrameCount) break;
		
		outputOffset++;
		
	} while (count < outputFrameCount);
	
	mixData->directVolumeCurrent[0] = directVolumeLeft;
	mixData->directVolumeCurrent[1] = directVolumeRight;
	
	mixData->sampleTableIndex = index;
	inputOffset = offset >> kMixFractionSize;
	return (count);
}

int32 SoundMgr::MixStereoSamples_Mono_Constant_Wet(SoundMixData *mixData, const Sample *input, int32 inputFrameCount, int32& inputOffset, int32 outputFrameCount, int32 outputOffset)
{
	float directVolumeLeft = mixData->directVolumeCurrent[0];
	float directVolumeRight = mixData->directVolumeCurrent[1];
	float reflectionVolume = mixData->reflectionVolumeCurrent;
	
	Fixed ds = (int32) (mixData->frequencyFinal * kMixFractionMultiplier);
	Fixed offset = inputOffset << kMixFractionSize;
	
	int32 count = 0;
	if (offset < 0)
	{
		count = Min(-offset / ds, outputFrameCount);
		
		directVolumeLeft += mixData->directVolumeDelta[0] * count;
		directVolumeRight += mixData->directVolumeDelta[1] * count;
		reflectionVolume += mixData->reflectionVolumeDelta * count;
		
		if (count == outputFrameCount)
		{
			mixData->directVolumeCurrent[0] = directVolumeLeft;
			mixData->directVolumeCurrent[1] = directVolumeRight;
			mixData->reflectionVolumeCurrent = reflectionVolume;
			
			inputOffset = (offset + ds * outputFrameCount) >> kMixFractionSize;
			return (outputFrameCount);
		}
		else
		{
			offset = 0;
			outputOffset += count;
		}
	}
	
	StereoMixFrame *output = stereoRingBuffer;
	StereoMixFrame *reverb = mixData->reflectionData[0].roomMixBuffer->reverbBuffer;
	
	unsigned_int32 index = mixData->sampleTableIndex;
	
	if (!mixData->reflectionData[1].roomMixBuffer)
	{
		do
		{
			int32 sample1 = ReadLittleEndianS16(&input[offset >> kMixFractionSize]);
			int32 sample2 = ReadLittleEndianS16(&input[Min((offset >> kMixFractionSize) + 1, inputFrameCount - 1)]);
			
			Fixed param = offset & kMixFractionMax;
			float sample = (float) (sample1 + (((sample2 - sample1) * param) >> kMixFractionSize));
			
			float *tableSample = &mixData->sampleTable[index];
			index = (index + 1) & (kSampleHistoryCount - 1);
			
			float sum = mixData->sampleTableSum - *tableSample + sample;
			mixData->sampleTableSum = sum;
			*tableSample = sample;
			
			sum *= 1.0F / (float) kSampleHistoryCount;
			sample = sum + (sample - sum) * mixData->directHFVolume;
			
			float leftSample = sample * directVolumeLeft;
			float rightSample = sample * directVolumeRight;
			
			output[outputOffset].left += leftSample;
			output[outputOffset].right += rightSample;
			
			sample *= reflectionVolume;
			for (machine a = 0; a < kSoundReflectionCount; a++)
			{
				float t = mixData->reflectionData[0].reflectionHFVolume[a] * mixData->reflectionHFVolume;
				float s = sum + (sample - sum) * t;
				
				unsigned_int32 reflectOffset = (outputOffset + mixData->reflectionData[0].reflectionDelay[a]) & (kRingBufferFrameCount - 1);
				reverb[reflectOffset].left += s * mixData->reflectionData[0].reflectionVolume[a];
				reverb[reflectOffset].right += s * mixData->reflectionData[0].reflectionVolume[a];
			}
			
			directVolumeLeft += mixData->directVolumeDelta[0];
			directVolumeRight += mixData->directVolumeDelta[1];
			reflectionVolume += mixData->reflectionVolumeDelta;
			
			count++;
			offset += ds;
			if ((offset >> kMixFractionSize) >= inputFrameCount) break;
			
			outputOffset++;
			
		} while (count < outputFrameCount);
	}
	else
	{
		StereoMixFrame *reverb2 = mixData->reflectionData[1].roomMixBuffer->reverbBuffer;
		
		do
		{
			int32 sample1 = ReadLittleEndianS16(&input[offset >> kMixFractionSize]);
			int32 sample2 = ReadLittleEndianS16(&input[Min((offset >> kMixFractionSize) + 1, inputFrameCount - 1)]);
			
			Fixed param = offset & kMixFractionMax;
			float sample = (float) (sample1 + (((sample2 - sample1) * param) >> kMixFractionSize));
			
			float *tableSample = &mixData->sampleTable[index];
			index = (index + 1) & (kSampleHistoryCount - 1);
			
			float sum = mixData->sampleTableSum - *tableSample + sample;
			mixData->sampleTableSum = sum;
			*tableSample = sample;
			
			sum *= 1.0F / (float) kSampleHistoryCount;
			sample = sum + (sample - sum) * mixData->directHFVolume;
			
			float leftSample = sample * directVolumeLeft;
			float rightSample = sample * directVolumeRight;
			
			output[outputOffset].left += leftSample;
			output[outputOffset].right += rightSample;
			
			sample *= reflectionVolume;
			for (machine a = 0; a < kSoundReflectionCount; a++)
			{
				float t = mixData->reflectionData[0].reflectionHFVolume[a] * mixData->reflectionHFVolume;
				float s = sum + (sample - sum) * t;
				
				unsigned_int32 reflectOffset = (outputOffset + mixData->reflectionData[0].reflectionDelay[a]) & (kRingBufferFrameCount - 1);
				reverb[reflectOffset].left += s * mixData->reflectionData[0].reflectionVolume[a];
				reverb[reflectOffset].right += s * mixData->reflectionData[0].reflectionVolume[a];
				
				t = mixData->reflectionData[1].reflectionHFVolume[a] * mixData->reflectionHFVolume;
				s = sum + (sample - sum) * t;
				
				reflectOffset = (outputOffset + mixData->reflectionData[1].reflectionDelay[a]) & (kRingBufferFrameCount - 1);
				reverb2[reflectOffset].left += s * mixData->reflectionData[1].reflectionVolume[a];
				reverb2[reflectOffset].right += s * mixData->reflectionData[1].reflectionVolume[a];
			}
			
			directVolumeLeft += mixData->directVolumeDelta[0];
			directVolumeRight += mixData->directVolumeDelta[1];
			reflectionVolume += mixData->reflectionVolumeDelta;
			
			count++;
			offset += ds;
			if ((offset >> kMixFractionSize) >= inputFrameCount) break;
			
			outputOffset++;
			
		} while (count < outputFrameCount);
	}
	
	mixData->directVolumeCurrent[0] = directVolumeLeft;
	mixData->directVolumeCurrent[1] = directVolumeRight;
	mixData->reflectionVolumeCurrent = reflectionVolume;
	
	mixData->sampleTableIndex = index;
	inputOffset = offset >> kMixFractionSize;
	return (count);
}

int32 SoundMgr::MixStereoSamples_Mono_Variable_Dry(SoundMixData *mixData, const Sample *input, int32 inputFrameCount, int32& inputOffset, int32 outputFrameCount, int32 outputOffset)
{
	float directVolumeLeft = mixData->directVolumeCurrent[0];
	float directVolumeRight = mixData->directVolumeCurrent[1];
	
	float ds = mixData->frequencyCurrent * kMixFractionMultiplier;
	float alpha = mixData->frequencyAlpha;
	Fixed offset = inputOffset << kMixFractionSize;
	
	int32 count = 0;
	if (offset < 0)
	{
		do
		{
			directVolumeLeft += mixData->directVolumeDelta[0];
			directVolumeRight += mixData->directVolumeDelta[1];
			
			count++;
			offset += (int32) ds;
			ds *= alpha;
			outputOffset++;
			
			if (offset > 0) break;
			
		} while (count < outputFrameCount);
	}
	
	StereoMixFrame *output = stereoRingBuffer;
	
	unsigned_int32 index = mixData->sampleTableIndex;
	while (count < outputFrameCount)
	{
		int32 sample1 = ReadLittleEndianS16(&input[offset >> kMixFractionSize]);
		int32 sample2 = ReadLittleEndianS16(&input[Min((offset >> kMixFractionSize) + 1, inputFrameCount - 1)]);
		
		Fixed param = offset & kMixFractionMax;
		float sample = (float) (sample1 + (((sample2 - sample1) * param) >> kMixFractionSize));
		
		float *tableSample = &mixData->sampleTable[index];
		index = (index + 1) & (kSampleHistoryCount - 1);
		
		float sum = mixData->sampleTableSum - *tableSample + sample;
		mixData->sampleTableSum = sum;
		*tableSample = sample;
		
		sum *= 1.0F / (float) kSampleHistoryCount;
		sample = sum + (sample - sum) * mixData->directHFVolume;
		
		float leftSample = sample * directVolumeLeft;
		float rightSample = sample * directVolumeRight;
		
		output[outputOffset].left += leftSample;
		output[outputOffset].right += rightSample;
		
		directVolumeLeft += mixData->directVolumeDelta[0];
		directVolumeRight += mixData->directVolumeDelta[1];
		
		count++;
		offset += (int32) ds;
		if ((offset >> kMixFractionSize) >= inputFrameCount) break;
		
		ds *= alpha;
		outputOffset++;
	}
	
	mixData->directVolumeCurrent[0] = directVolumeLeft;
	mixData->directVolumeCurrent[1] = directVolumeRight;
	mixData->frequencyCurrent = ds * kMixFractionReciprocal;
	
	mixData->sampleTableIndex = index;
	inputOffset = offset >> kMixFractionSize;
	return (count);
}

int32 SoundMgr::MixStereoSamples_Mono_Variable_Wet(SoundMixData *mixData, const Sample *input, int32 inputFrameCount, int32& inputOffset, int32 outputFrameCount, int32 outputOffset)
{
	float directVolumeLeft = mixData->directVolumeCurrent[0];
	float directVolumeRight = mixData->directVolumeCurrent[1];
	float reflectionVolume = mixData->reflectionVolumeCurrent;
	
	float ds = mixData->frequencyCurrent * kMixFractionMultiplier;
	float alpha = mixData->frequencyAlpha;
	Fixed offset = inputOffset << kMixFractionSize;
	
	int32 count = 0;
	if (offset < 0)
	{
		do
		{
			directVolumeLeft += mixData->directVolumeDelta[0];
			directVolumeRight += mixData->directVolumeDelta[1];
			reflectionVolume += mixData->reflectionVolumeDelta;
			
			count++;
			offset += (int32) ds;
			ds *= alpha;
			outputOffset++;
			
			if (offset > 0) break;
			
		} while (count < outputFrameCount);
	}
	
	StereoMixFrame *output = stereoRingBuffer;
	StereoMixFrame *reverb = mixData->reflectionData[0].roomMixBuffer->reverbBuffer;
	
	unsigned_int32 index = mixData->sampleTableIndex;
	
	if (!mixData->reflectionData[1].roomMixBuffer)
	{
		while (count < outputFrameCount)
		{
			int32 sample1 = ReadLittleEndianS16(&input[offset >> kMixFractionSize]);
			int32 sample2 = ReadLittleEndianS16(&input[Min((offset >> kMixFractionSize) + 1, inputFrameCount - 1)]);
			
			Fixed param = offset & kMixFractionMax;
			float sample = (float) (sample1 + (((sample2 - sample1) * param) >> kMixFractionSize));
			
			float *tableSample = &mixData->sampleTable[index];
			index = (index + 1) & (kSampleHistoryCount - 1);
			
			float sum = mixData->sampleTableSum - *tableSample + sample;
			mixData->sampleTableSum = sum;
			*tableSample = sample;
			
			sum *= 1.0F / (float) kSampleHistoryCount;
			sample = sum + (sample - sum) * mixData->directHFVolume;
			
			float leftSample = sample * directVolumeLeft;
			float rightSample = sample * directVolumeRight;
			
			output[outputOffset].left += leftSample;
			output[outputOffset].right += rightSample;
			
			sample *= reflectionVolume;
			for (machine a = 0; a < kSoundReflectionCount; a++)
			{
				float t = mixData->reflectionData[0].reflectionHFVolume[a] * mixData->reflectionHFVolume;
				float s = sum + (sample - sum) * t;
				
				unsigned_int32 reflectOffset = (outputOffset + mixData->reflectionData[0].reflectionDelay[a]) & (kRingBufferFrameCount - 1);
				reverb[reflectOffset].left += s * mixData->reflectionData[0].reflectionVolume[a];
				reverb[reflectOffset].right += s * mixData->reflectionData[0].reflectionVolume[a];
			}
			
			directVolumeLeft += mixData->directVolumeDelta[0];
			directVolumeRight += mixData->directVolumeDelta[1];
			reflectionVolume += mixData->reflectionVolumeDelta;
			
			count++;
			offset += (int32) ds;
			if ((offset >> kMixFractionSize) >= inputFrameCount) break;
			
			ds *= alpha;
			outputOffset++;
		}
	}
	else
	{
		StereoMixFrame *reverb2 = mixData->reflectionData[1].roomMixBuffer->reverbBuffer;
		
		while (count < outputFrameCount)
		{
			int32 sample1 = ReadLittleEndianS16(&input[offset >> kMixFractionSize]);
			int32 sample2 = ReadLittleEndianS16(&input[Min((offset >> kMixFractionSize) + 1, inputFrameCount - 1)]);
			
			Fixed param = offset & kMixFractionMax;
			float sample = (float) (sample1 + (((sample2 - sample1) * param) >> kMixFractionSize));
			
			float *tableSample = &mixData->sampleTable[index];
			index = (index + 1) & (kSampleHistoryCount - 1);
			
			float sum = mixData->sampleTableSum - *tableSample + sample;
			mixData->sampleTableSum = sum;
			*tableSample = sample;
			
			sum *= 1.0F / (float) kSampleHistoryCount;
			sample = sum + (sample - sum) * mixData->directHFVolume;
			
			float leftSample = sample * directVolumeLeft;
			float rightSample = sample * directVolumeRight;
			
			output[outputOffset].left += leftSample;
			output[outputOffset].right += rightSample;
			
			sample *= reflectionVolume;
			for (machine a = 0; a < kSoundReflectionCount; a++)
			{
				float t = mixData->reflectionData[0].reflectionHFVolume[a] * mixData->reflectionHFVolume;
				float s = sum + (sample - sum) * t;
				
				unsigned_int32 reflectOffset = (outputOffset + mixData->reflectionData[0].reflectionDelay[a]) & (kRingBufferFrameCount - 1);
				reverb[reflectOffset].left += s * mixData->reflectionData[0].reflectionVolume[a];
				reverb[reflectOffset].right += s * mixData->reflectionData[0].reflectionVolume[a];
				
				t = mixData->reflectionData[1].reflectionHFVolume[a] * mixData->reflectionHFVolume;
				s = sum + (sample - sum) * t;
				
				reflectOffset = (outputOffset + mixData->reflectionData[1].reflectionDelay[a]) & (kRingBufferFrameCount - 1);
				reverb2[reflectOffset].left += s * mixData->reflectionData[1].reflectionVolume[a];
				reverb2[reflectOffset].right += s * mixData->reflectionData[1].reflectionVolume[a];
			}
			
			directVolumeLeft += mixData->directVolumeDelta[0];
			directVolumeRight += mixData->directVolumeDelta[1];
			reflectionVolume += mixData->reflectionVolumeDelta;
			
			count++;
			offset += (int32) ds;
			if ((offset >> kMixFractionSize) >= inputFrameCount) break;
			
			ds *= alpha;
			outputOffset++;
		}
	}
	
	mixData->directVolumeCurrent[0] = directVolumeLeft;
	mixData->directVolumeCurrent[1] = directVolumeRight;
	mixData->reflectionVolumeCurrent = reflectionVolume;
	mixData->frequencyCurrent = ds * kMixFractionReciprocal;
	
	mixData->sampleTableIndex = index;
	inputOffset = offset >> kMixFractionSize;
	return (count);
}

int32 SoundMgr::MixStereoSamples_Stereo_Constant(SoundMixData *mixData, const Sample *input, int32 inputFrameCount, int32& inputOffset, int32 outputFrameCount, int32 outputOffset)
{
	float directVolumeLeft = mixData->directVolumeCurrent[0];
	float directVolumeRight = mixData->directVolumeCurrent[1];
	
	Fixed ds = (int32) (mixData->frequencyFinal * kMixFractionMultiplier);
	Fixed offset = inputOffset << kMixFractionSize;
	
	StereoMixFrame *output = stereoRingBuffer;
	
	int32 count = 0;
	do
	{
		const Sample *source = &input[(offset >> (kMixFractionSize - 1)) & ~1];
		float leftSample = (float) ReadLittleEndianS16(&source[0]);
		float rightSample = (float) ReadLittleEndianS16(&source[1]);
		
		output[outputOffset].left += leftSample * directVolumeLeft;
		output[outputOffset].right += rightSample * directVolumeRight;
		
		directVolumeLeft += mixData->directVolumeDelta[0];
		directVolumeRight += mixData->directVolumeDelta[1];
		
		count++;
		offset += ds;
		if ((offset >> kMixFractionSize) >= inputFrameCount) break;
		
		outputOffset++;
		
	} while (count < outputFrameCount);
	
	mixData->directVolumeCurrent[0] = directVolumeLeft;
	mixData->directVolumeCurrent[1] = directVolumeRight;
	
	inputOffset = offset >> kMixFractionSize;
	return (count);
}

int32 SoundMgr::MixStereoSamples_Stereo_Variable(SoundMixData *mixData, const Sample *input, int32 inputFrameCount, int32& inputOffset, int32 outputFrameCount, int32 outputOffset)
{
	float directVolumeLeft = mixData->directVolumeCurrent[0];
	float directVolumeRight = mixData->directVolumeCurrent[1];
	
	float ds = mixData->frequencyCurrent * kMixFractionMultiplier;
	float alpha = mixData->frequencyAlpha;
	Fixed offset = inputOffset << kMixFractionSize;
	
	StereoMixFrame *output = stereoRingBuffer;
	
	int32 count = 0;
	do
	{
		const Sample *source = &input[(offset >> (kMixFractionSize - 1)) & ~1];
		float leftSample = (float) ReadLittleEndianS16(&source[0]);
		float rightSample = (float) ReadLittleEndianS16(&source[1]);
		
		output[outputOffset].left += leftSample * directVolumeLeft;
		output[outputOffset].right += rightSample * directVolumeRight;
		directVolumeLeft += mixData->directVolumeDelta[0];
		directVolumeRight += mixData->directVolumeDelta[1];
		
		count++;
		offset += (int32) ds;
		if ((offset >> kMixFractionSize) >= inputFrameCount) break;
		
		ds *= alpha;
		outputOffset++;
		
	} while (count < outputFrameCount);
	
	mixData->directVolumeCurrent[0] = directVolumeLeft;
	mixData->directVolumeCurrent[1] = directVolumeRight;
	mixData->frequencyCurrent = ds * kMixFractionReciprocal;
	
	inputOffset = offset >> kMixFractionSize;
	return (count);
}

void SoundMgr::MixSoundMono(Sound *sound, int32 outputOffset)
{
	const Sample *input = sound->soundSampleData;
	int32 inputFrameCount = sound->soundFrameCount;
	int32 inputOffset = sound->playFrame;
	
	int32 outputFrameCount = kOutputBufferFrameCount;
	
	float ratio = sound->soundMixData.frequencyFinal / sound->soundMixData.frequencyCurrent;
	if (Fabs(ratio - 1.0F) < 0.015625F)
	{
		MixProc mixProc = &SoundMgr::MixStereoSamples_Mono_Constant;
		if (sound->GetSoundFlags() & kSoundSpatialized)
		{
			mixProc = (sound->soundMixData.reflectionData[0].roomMixBuffer) ? &SoundMgr::MixStereoSamples_Mono_Constant_Wet : &SoundMgr::MixStereoSamples_Mono_Constant_Dry;
		}
		
		for (;;)
		{
			int32 mixFrameCount = (this->*mixProc)(&sound->soundMixData, input, inputFrameCount, inputOffset, outputFrameCount, outputOffset);
			
			outputOffset += mixFrameCount;
			outputFrameCount -= mixFrameCount;
			
			if (inputOffset >= inputFrameCount)
			{
				if (sound->loopCount > 0) sound->loopCount--;
				if (sound->loopCount != 0)
				{
					inputOffset = 0;
					sound->loopFlag = true;
					continue;
				}
			}
			
			break;
		}
	}
	else
	{
		sound->soundMixData.frequencyAlpha = Pow(ratio, 1.0F / (float) kOutputBufferFrameCount);
		
		MixProc mixProc = &SoundMgr::MixStereoSamples_Mono_Variable;
		if (sound->GetSoundFlags() & kSoundSpatialized)
		{
			mixProc = (sound->soundMixData.reflectionData[0].roomMixBuffer) ? &SoundMgr::MixStereoSamples_Mono_Variable_Wet : &SoundMgr::MixStereoSamples_Mono_Variable_Dry;
		}
		
		for (;;)
		{
			int32 mixFrameCount = (this->*mixProc)(&sound->soundMixData, input, inputFrameCount, inputOffset, outputFrameCount, outputOffset);
			
			outputOffset += mixFrameCount;
			outputFrameCount -= mixFrameCount;
			
			if (inputOffset >= inputFrameCount)
			{
				if (sound->loopCount > 0) sound->loopCount--;
				if (sound->loopCount != 0)
				{
					inputOffset = 0;
					sound->loopFlag = true;
					continue;
				}
			}
			
			break;
		}
	}
	
	sound->playFrame = inputOffset;
}

void SoundMgr::MixSoundStereo(Sound *sound, int32 outputOffset)
{
	const Sample *input = sound->soundSampleData;
	int32 inputFrameCount = sound->soundFrameCount;
	int32 inputOffset = sound->playFrame;
	
	int32 outputFrameCount = kOutputBufferFrameCount;
	
	float ratio = sound->soundMixData.frequencyFinal / sound->soundMixData.frequencyCurrent;
	if (Fabs(ratio - 1.0F) < 0.015625F)
	{
		for (;;)
		{
			int32 mixFrameCount = MixStereoSamples_Stereo_Constant(&sound->soundMixData, input, inputFrameCount, inputOffset, outputFrameCount, outputOffset);
			
			outputOffset += mixFrameCount;
			outputFrameCount -= mixFrameCount;
			
			if (inputOffset >= inputFrameCount)
			{
				if (sound->loopCount > 0) sound->loopCount--;
				if (sound->loopCount != 0)
				{
					inputOffset = 0;
					sound->loopFlag = true;
					continue;
				}
			}
			
			break;
		}
	}
	else
	{
		sound->soundMixData.frequencyAlpha = Pow(ratio, 1.0F / (float) kOutputBufferFrameCount);
		
		for (;;)
		{
			int32 mixFrameCount = MixStereoSamples_Stereo_Variable(&sound->soundMixData, input, inputFrameCount, inputOffset, outputFrameCount, outputOffset);
			
			outputOffset += mixFrameCount;
			outputFrameCount -= mixFrameCount;
			
			if (inputOffset >= inputFrameCount)
			{
				if (sound->loopCount > 0) sound->loopCount--;
				if (sound->loopCount != 0)
				{
					inputOffset = 0;
					sound->loopFlag = true;
					continue;
				}
			}
			
			break;
		}
	}
	
	sound->playFrame = inputOffset;
}

void SoundMgr::MixSoundStreamMono(Sound *sound, int32 outputOffset)
{
	int32 bufferIndex = sound->playBuffer;
	StreamBufferHeader *header = sound->GetSoundStreamer()->GetStreamBuffer(bufferIndex);
	
	if (header->readyFlag)
	{
		const Sample *input = header->GetSampleData();
		int32 inputFrameCount = header->frameCount;
		int32 inputOffset = sound->playFrame;
		
		int32 outputFrameCount = kOutputBufferFrameCount;
		
		float ratio = sound->soundMixData.frequencyFinal / sound->soundMixData.frequencyCurrent;
		if (Fabs(ratio - 1.0F) < 0.015625F)
		{
			MixProc mixProc = &SoundMgr::MixStereoSamples_Mono_Constant;
			if (sound->GetSoundFlags() & kSoundSpatialized)
			{
				mixProc = (sound->soundMixData.reflectionData[0].roomMixBuffer) ? &SoundMgr::MixStereoSamples_Mono_Constant_Wet : &SoundMgr::MixStereoSamples_Mono_Constant_Dry;
			}
			
			do
			{
				int32 mixFrameCount = (this->*mixProc)(&sound->soundMixData, input, inputFrameCount, inputOffset, outputFrameCount, outputOffset);
				
				outputOffset += mixFrameCount;
				outputFrameCount -= mixFrameCount;
				
				if (inputOffset >= header->loopFrame)
				{
					sound->loopFlag = true;
					header->loopFrame = 0x7FFFFFFF;
				}
				
				if (inputOffset >= inputFrameCount)
				{
					if (header->finalFlag)
					{
						sound->soundState = kSoundCompleted;
						break;
					}
					
					header->readyFlag = false;
					bufferIndex = 1 - bufferIndex;
					inputOffset = 0;
					
					header = sound->GetSoundStreamer()->GetStreamBuffer(bufferIndex);
					if (!header->readyFlag) break;
					
					input = header->GetSampleData();
					inputFrameCount = header->frameCount;
				}
			} while (outputFrameCount > 0);
		}
		else
		{
			sound->soundMixData.frequencyAlpha = Pow(ratio, 1.0F / (float) kOutputBufferFrameCount);
			
			MixProc mixProc = &SoundMgr::MixStereoSamples_Mono_Variable;
			if (sound->GetSoundFlags() & kSoundSpatialized)
			{
				mixProc = (sound->soundMixData.reflectionData[0].roomMixBuffer) ? &SoundMgr::MixStereoSamples_Mono_Variable_Wet : &SoundMgr::MixStereoSamples_Mono_Variable_Dry;
			}
			
			do
			{
				int32 mixFrameCount = (this->*mixProc)(&sound->soundMixData, input, inputFrameCount, inputOffset, outputFrameCount, outputOffset);
				
				outputOffset += mixFrameCount;
				outputFrameCount -= mixFrameCount;
				
				if (inputOffset >= header->loopFrame)
				{
					sound->loopFlag = true;
					header->loopFrame = 0x7FFFFFFF;
				}
				
				if (inputOffset >= inputFrameCount)
				{
					if (header->finalFlag)
					{
						sound->soundState = kSoundCompleted;
						break;
					}
					
					header->readyFlag = false;
					bufferIndex = 1 - bufferIndex;
					inputOffset = 0;
					
					header = sound->GetSoundStreamer()->GetStreamBuffer(bufferIndex);
					if (!header->readyFlag) break;
					
					input = header->GetSampleData();
					inputFrameCount = header->frameCount;
				}
			} while (outputFrameCount > 0);
		}
		
		sound->playFrame = inputOffset;
		sound->playBuffer = bufferIndex;
	}
}

void SoundMgr::MixSoundStreamStereo(Sound *sound, int32 outputOffset)
{
	int32 bufferIndex = sound->playBuffer;
	StreamBufferHeader *header = sound->GetSoundStreamer()->GetStreamBuffer(bufferIndex);
	
	if (header->readyFlag)
	{
		const Sample *input = header->GetSampleData();
		int32 inputFrameCount = header->frameCount;
		int32 inputOffset = sound->playFrame;
		
		unsigned_int32 outputFrameCount = kOutputBufferFrameCount;
		
		float ratio = sound->soundMixData.frequencyFinal / sound->soundMixData.frequencyCurrent;
		if (Fabs(ratio - 1.0F) < 0.015625F)
		{
			do
			{
				int32 mixFrameCount = MixStereoSamples_Stereo_Constant(&sound->soundMixData, input, inputFrameCount, inputOffset, outputFrameCount, outputOffset);
				
				outputOffset += mixFrameCount;
				outputFrameCount -= mixFrameCount;
				
				if (inputOffset >= header->loopFrame)
				{
					sound->loopFlag = true;
					header->loopFrame = 0x7FFFFFFF;
				}
				
				if (inputOffset >= inputFrameCount)
				{
					if (header->finalFlag)
					{
						sound->soundState = kSoundCompleted;
						break;
					}
					
					header->readyFlag = false;
					bufferIndex = 1 - bufferIndex;
					inputOffset = 0;
					
					header = sound->GetSoundStreamer()->GetStreamBuffer(bufferIndex);
					if (!header->readyFlag) break;
					
					input = header->GetSampleData();
					inputFrameCount = header->frameCount;
				}
			} while (outputFrameCount > 0);
		}
		else
		{
			sound->soundMixData.frequencyAlpha = Pow(ratio, 1.0F / (float) kOutputBufferFrameCount);
			
			do
			{
				int32 mixFrameCount = MixStereoSamples_Stereo_Variable(&sound->soundMixData, input, inputFrameCount, inputOffset, outputFrameCount, outputOffset);
				
				outputOffset += mixFrameCount;
				outputFrameCount -= mixFrameCount;
				
				if (inputOffset >= header->loopFrame)
				{
					sound->loopFlag = true;
					header->loopFrame = 0x7FFFFFFF;
				}
				
				if (inputOffset >= inputFrameCount)
				{
					if (header->finalFlag)
					{
						sound->soundState = kSoundCompleted;
						break;
					}
					
					header->readyFlag = false;
					bufferIndex = 1 - bufferIndex;
					inputOffset = 0;
					
					header = sound->GetSoundStreamer()->GetStreamBuffer(bufferIndex);
					if (!header->readyFlag) break;
					
					input = header->GetSampleData();
					inputFrameCount = header->frameCount;
				}
			} while (outputFrameCount > 0);
		}
		
		sound->playFrame = inputOffset;
		sound->playBuffer = bufferIndex;
	}
}

void SoundMgr::MixRoomEffects(const SoundRoom *soundRoom, RoomMixBuffer *roomMixBuffer, int32 outputOffset)
{
	static const int32 kFeedbackOffset[kRoomFeedbackBufferCount] =
	{
		kMaxFeedbackDelay * 35 / 100, kMaxFeedbackDelay * 63 / 100, kMaxFeedbackDelay * 85 / 100, kMaxFeedbackDelay
	};
	
	StereoMixFrame *output = stereoRingBuffer;
	StereoMixFrame *reverb = roomMixBuffer->reverbBuffer;
	float reverbVolume = soundRoom->reverbVolume;
	
	float leftVolume = soundRoom->roomVolume[0];
	float rightVolume = soundRoom->roomVolume[1];
	
	int32 count = 0;
	do
	{
		float leftSample = reverb[outputOffset].left;
		float rightSample = reverb[outputOffset].right;
		
		for (machine a = 0; a < kRoomFeedbackBufferCount; a++)
		{
			StereoMixFrame *input = roomMixBuffer->feedbackBuffer[a];
			leftSample = input[outputOffset].left - leftSample;
			rightSample = input[outputOffset].right - rightSample;
			
			StereoMixFrame *feedback = &input[(outputOffset + kFeedbackOffset[a]) & (kRingBufferFrameCount - 1)];
			feedback->left += reverb[outputOffset].left + input[outputOffset].left * reverbVolume;
			feedback->right += reverb[outputOffset].right + input[outputOffset].right * reverbVolume;
			
			input[outputOffset].left = 0.0F;
			input[outputOffset].right = 0.0F;
		}
		
		output[outputOffset].left += leftSample * leftVolume;
		output[outputOffset].right += rightSample * rightVolume;
		reverb[outputOffset].left = 0.0F;
		reverb[outputOffset].right = 0.0F;
		
		outputOffset++;
	} while (++count < kOutputBufferFrameCount);
}

void SoundMgr::AllocateListenerRoomMixBuffer(SoundRoom *soundRoom)
{
	int32 roomIndex = -1;
	int32 minMixCount = 0x7FFF;
	for (machine a = 0; a < kMaxRoomCount; a++)
	{
		const SoundRoom *room = activeRoomTable[a];
		if (!room)
		{
			roomIndex = a;
			break;
		}
		
		int32 count = room->roomMixCount;
		if (count < minMixCount)
		{
			minMixCount = count;
			roomIndex = a;
		}
	}
	
	SoundRoom *room = activeRoomTable[roomIndex];
	if (room) room->tableIndex = -1;
	
	MemoryMgr::ClearMemory(roomMixBuffer[roomIndex], sizeof(RoomMixBuffer));
	activeRoomTable[roomIndex] = soundRoom;
	soundRoom->tableIndex = roomIndex;
	soundRoom->roomMixCount = 0;
}

int32 SoundMgr::AllocateSoundRoomMixBuffer(SoundRoom *soundRoom)
{
	if (!soundRoom->GetOwningList())
	{
		int32 index = soundRoom->tableIndex;
		if (index >= 0) return (index);
		
		for (machine a = 0; a < kMaxRoomCount; a++)
		{
			const SoundRoom *room = activeRoomTable[a];
			if (!room)
			{
				MemoryMgr::ClearMemory(roomMixBuffer[a], sizeof(RoomMixBuffer));
				activeRoomTable[a] = soundRoom;
				soundRoom->tableIndex = a;
				return (a);
			}
		}
	}
	
	return (-1);
}

void SoundMgr::MixSounds(OutputSample *output)
{
	bool reverbFlag = ((soundOptionFlags & kSoundOptionReverb) != 0);
	if (reverbFlag)
	{
		SoundRoom *primaryRoom = listenerRoom;
		if ((primaryRoom) && (primaryRoom->tableIndex < 0)) AllocateListenerRoomMixBuffer(primaryRoom);
	}
	
	int32 sliceIndex = ringBufferSliceIndex;
	int32 outputOffset = kOutputBufferFrameCount * sliceIndex;
	ringBufferSliceIndex = (sliceIndex + 1) & (kRingBufferSliceCount - 1);
	
	for (machine index = 0; index < kMaxSoundCount; index++)
	{
		Sound *sound = activeSoundTable[index];
		if (sound)
		{
			if (!sound->releaseFlag)
			{
				if (sound->soundState == kSoundPlaying)
				{
					sound->soundMixData.directVolumeDelta[0] = (sound->soundMixData.directVolumeFinal[0] - sound->soundMixData.directVolumeCurrent[0]) / kOutputBufferFrameCount;
					sound->soundMixData.directVolumeDelta[1] = (sound->soundMixData.directVolumeFinal[1] - sound->soundMixData.directVolumeCurrent[1]) / kOutputBufferFrameCount;
					
					unsigned_int32 flags = sound->GetSoundFlags();
					if ((flags & kSoundSpatialized) && (sound->channelCount == 1))
					{
						sound->soundMixData.reflectionVolumeDelta = (sound->soundMixData.reflectionVolumeFinal - sound->soundMixData.reflectionVolumeCurrent) / kOutputBufferFrameCount;
						
						sound->soundMixData.reflectionData[0].roomMixBuffer = nullptr;
						sound->soundMixData.reflectionData[1].roomMixBuffer = nullptr;
						
						if ((flags & kSoundReverb) && (reverbFlag))
						{
							SoundRoom *soundRoom = sound->soundMixData.reflectionData[0].soundRoom;
							if (soundRoom)
							{
								int32 roomIndex = AllocateSoundRoomMixBuffer(soundRoom);
								if (roomIndex >= 0)
								{
									sound->soundMixData.reflectionData[0].roomMixBuffer = roomMixBuffer[roomIndex];
									soundRoom->roomMixCount = soundRoom->maxRoomMixCount;
									
									soundRoom = sound->soundMixData.reflectionData[1].soundRoom;
									if (soundRoom)
									{
										roomIndex = AllocateSoundRoomMixBuffer(soundRoom);
										if (roomIndex >= 0)
										{
											sound->soundMixData.reflectionData[1].roomMixBuffer = roomMixBuffer[roomIndex];
											soundRoom->roomMixCount = soundRoom->maxRoomMixCount;
										}
									}
								}
							}
						}
						
						const SoundStreamer *streamer = sound->soundStreamer;
						if (!streamer)
						{
							MixSoundMono(sound, outputOffset);
							if (sound->playFrame >= sound->soundFrameCount) sound->soundState = kSoundCompleted;
						}
						else
						{
							if (!streamer->Paused()) MixSoundStreamMono(sound, outputOffset);
						}
						
						sound->soundMixData.reflectionVolumeCurrent = sound->soundMixData.reflectionVolumeFinal;
					}
					else
					{
						const SoundStreamer *streamer = sound->soundStreamer;
						if (!streamer)
						{
							if (sound->channelCount == 1) MixSoundMono(sound, outputOffset);
							else MixSoundStereo(sound, outputOffset);
							
							if (sound->playFrame >= sound->soundFrameCount) sound->soundState = kSoundCompleted;
						}
						else
						{
							if (!streamer->Paused())
							{
								if (sound->channelCount == 1) MixSoundStreamMono(sound, outputOffset);
								else MixSoundStreamStereo(sound, outputOffset);
							}
						}
					}
					
					sound->soundMixData.directVolumeCurrent[0] = sound->soundMixData.directVolumeFinal[0];
					sound->soundMixData.directVolumeCurrent[1] = sound->soundMixData.directVolumeFinal[1];
					sound->soundMixData.frequencyCurrent = sound->soundMixData.frequencyFinal;
					
					sound->mixFlag = true;
				}
				else if (sound->soundState == kSoundStopping)
				{
					sound->soundState = kSoundStopped;
				}
			}
			else
			{
				sound->mixRelease = true;
			}
		}
	}
	
	for (machine a = 0; a < kMaxRoomCount; a++)
	{
		SoundRoom *room = activeRoomTable[a];
		if (room)
		{
			MixRoomEffects(room, roomMixBuffer[a], outputOffset);
			
			if (--room->roomMixCount < 0)
			{
				room->tableIndex = -1;
				activeRoomTable[a] = nullptr;
			}
		}
	}
	
	StereoMixFrame *ringBufferSlice = stereoRingBuffer + outputOffset;
	
	#if C4MACOS || C4PLAYSTATION3
	
		output--;
		float *input = &ringBufferSlice->left - 1;
		for (machine a = 0; a < kOutputBufferFrameCount * 2; a++)
		{
			*++output = *++input * 3.0517578125e-5F;
			*input = 0.0F;
		}
	
	#else
	
		float *input = &ringBufferSlice->left;
		for (machine a = 0; a < kOutputBufferFrameCount * 2; a++)
		{
			int32 sample = (int32) *input;
			sample = Min(sample, 32767);
			sample = Max(sample, -32768);
			*output++ = (Sample) sample;
			*input++ = 0.0F;
		}
	
	#endif
}

#if !C4SERVER

	#if C4XAUDIO
	
		void SoundMgr::SoundThread(const Thread *thread, void *cookie)
		{
			SoundMgr *soundMgr = static_cast<SoundMgr *>(cookie);
			
			for (;;)
			{
				int32 index = soundMgr->soundSignal->Wait(kSignalForever);
				if (index == 0) break;
				
				index--;
				soundMgr->MixSounds(soundMgr->playBuffer[index]);
				soundMgr->sourceVoice->SubmitSourceBuffer(&soundMgr->sourceBuffer[index]);
			}
		}
	
	#elif C4MACOS
	
		OSStatus SoundMgr::SoundCallback(void *inRefCon, AudioUnitRenderActionFlags *ioActionFlags, const AudioTimeStamp *inTimeStamp, UInt32 inBusNumber, UInt32 inNumberFrames, AudioBufferList *ioData)
		{
			if (inNumberFrames > kOutputBufferFrameCount) return (kAudioUnitErr_TooManyFramesToProcess);
			
			SoundMgr *soundMgr = static_cast<SoundMgr *>(inRefCon);
			
			unsigned_int32 beginFrame = soundMgr->playFrameIndex;
			unsigned_int32 endFrame = beginFrame + inNumberFrames;
			
			if (beginFrame <= kOutputBufferFrameCount)
			{
				if (endFrame > kOutputBufferFrameCount) soundMgr->MixSounds(soundMgr->playBuffer[1]);
				MemoryMgr::CopyMemory(soundMgr->playBuffer[0] + beginFrame * 2, ioData->mBuffers[0].mData, (endFrame - beginFrame) * 2 * sizeof(OutputSample));
			}
			else
			{
				if (endFrame > kOutputBufferFrameCount * 2)
				{
					soundMgr->MixSounds(soundMgr->playBuffer[0]);
					unsigned_int32 sampleCount = (kOutputBufferFrameCount * 2 - beginFrame) * 2;
					OutputSample *output = static_cast<OutputSample *>(ioData->mBuffers[0].mData);
					MemoryMgr::CopyMemory(soundMgr->playBuffer[0] + beginFrame * 2, output, sampleCount * sizeof(OutputSample));
					
					endFrame -= kOutputBufferFrameCount * 2;
					MemoryMgr::CopyMemory(soundMgr->playBuffer[0], output + sampleCount, endFrame * 2 * sizeof(OutputSample));
				}
				else
				{
					MemoryMgr::CopyMemory(soundMgr->playBuffer[0] + beginFrame * 2, ioData->mBuffers[0].mData, (endFrame - beginFrame) * 2 * sizeof(OutputSample));
				}
			}
			
			soundMgr->playFrameIndex = endFrame;
			return (noErr);
		}
	
	#elif C4LINUX
	
		void SoundMgr::SoundThread(const Thread *thread, void *cookie)
		{
			SoundMgr *soundMgr = static_cast<SoundMgr *>(cookie);
			
			for (;;)
			{
				snd_pcm_wait(soundMgr->soundHandle, -1);
				if (soundMgr->soundExitFlag) break;
				
				soundMgr->MixSounds(soundMgr->playBuffer);
				
				int result = snd_pcm_writei(soundMgr->soundHandle, soundMgr->playBuffer, kOutputBufferFrameCount);
				if (result < 0)
				{
					if (soundMgr->soundExitFlag) break;
					snd_pcm_recover(soundMgr->soundHandle, result, true);
				}
			}
		}
	
	#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#endif //]

#endif

void SoundMgr::SetMasterVolume(float volume)
{
	masterVolume = volume;
	
	Sound *sound = loadedSoundList.First();
	while (sound)
	{
		sound->updateFlags |= kSoundUpdateVolume;
		sound = sound->Next();
	}
}

void SoundMgr::StopAllSounds(void)
{
	Sound *sound = loadedSoundList.First();
	while (sound)
	{
		SoundState state = sound->GetSoundState();
		if ((state >= kSoundDelaying) && (state <= kSoundPaused)) sound->Stop();
		sound = sound->Next();
	}
}

void SoundMgr::PauseAllSounds(void)
{
	Sound *sound = loadedSoundList.First();
	while (sound)
	{
		sound->Pause();
		sound = sound->Next();
	}
}

void SoundMgr::ResumeAllSounds(void)
{
	Sound *sound = loadedSoundList.First();
	while (sound)
	{
		sound->Resume();
		sound = sound->Next();
	}
}

void SoundMgr::SetGlobalSoundSpeed(float speed)
{
	globalSoundSpeed = speed;
	unitDistanceFrameCount = (float) kSoundOutputSampleRate / speed;
}

void SoundMgr::SetListenerRoom(SoundRoom *room)
{
	listenerRoom = room;
	if (room)
	{
		room->outputRoom = nullptr;
		room->roomVolume[0] = 1.0F;
		room->roomVolume[1] = 1.0F;
	}
}

void SoundMgr::StreamThread(const Thread *thread, void *cookie)
{
	SoundMgr *soundMgr = static_cast<SoundMgr *>(cookie);
	
	for (;;)
	{
		for (machine a = 0; a < kMaxSoundCount; a++)
		{
			Sound *sound = soundMgr->activeSoundTable[a];
			if (sound)
			{
				if (!sound->releaseFlag)
				{
					if ((sound->soundState == kSoundPlaying) && (sound->Streaming()))
					{
						SoundStreamer *streamer = sound->GetSoundStreamer();
						StreamBufferHeader *buffer1 = streamer->GetStreamBuffer(0);
						StreamBufferHeader *buffer2 = streamer->GetStreamBuffer(1);
						if ((!buffer1->readyFlag) && (!buffer2->finalFlag)) sound->FillStreamBuffer(streamer, buffer1);
						if ((!buffer2->readyFlag) && (!buffer1->finalFlag)) sound->FillStreamBuffer(streamer, buffer2);
					}
				}
				else
				{
					sound->streamRelease = true;
				}
			}
		}
		
		if (soundMgr->streamSignal->Wait() == 0) break;
	}
}

void SoundMgr::SoundTask(void)
{
	int32 dt = TheTimeMgr->GetSystemDeltaTime();
	float fdt = TheTimeMgr->GetSystemFloatDeltaTime();
	
	Sound *sound = loadedSoundList.First();
	while (sound)
	{
		Sound *next = sound->Next();
		
		if (!sound->releaseFlag)
		{
			switch (sound->soundState)
			{
				case kSoundStopped:
				{
					int32 index = sound->tableIndex;
					if (index >= 0)
					{
						activeSoundTable[index] = nullptr;
						sound->tableIndex = -1;
					}
					
					break;
				}
				
				case kSoundDelaying:
				
					if ((sound->delayTime -= dt) <= 0) sound->Play();
					break;
					
				case kSoundPlaying:
				{
					if (sound->Streaming())
					{
						const SoundStreamer *streamer = sound->GetSoundStreamer();
						StreamBufferHeader *buffer1 = streamer->GetStreamBuffer(0);
						StreamBufferHeader *buffer2 = streamer->GetStreamBuffer(1);
						if ((!buffer1->readyFlag) || (!buffer2->readyFlag)) streamSignal->Trigger(1);
					}
					
					if (sound->loopFlag)
					{
						sound->loopFlag = false;
						if (sound->loopProc) (*sound->loopProc)(sound, sound->loopCookie);
					}
					
					bool spatialized = ((sound->GetSoundFlags() & kSoundSpatialized) != 0);
					bool updateVolume = ((sound->updateFlags & kSoundUpdateVolume) != 0) | spatialized;
					
					unsigned_int32 fadeFlags = sound->fadeFlags;
					if (fadeFlags & kSoundFadeActive)
					{
						updateVolume = true;
						if (!(fadeFlags & kSoundFadeEnding))
						{
							float volume = sound->soundProperty[kSoundVolume] + fdt * sound->fadeDelta;
							if ((sound->fadeDelta >= 0.0F) ? (volume >= sound->fadeVolume) : (volume <= sound->fadeVolume))
							{
								volume = sound->fadeVolume;
								sound->fadeFlags = fadeFlags | kSoundFadeEnding;
								sound->mixFlag = false;
							}
							
							sound->soundProperty[kSoundVolume] = volume;
						}
						else if (sound->mixFlag)
						{
							sound->fadeFlags = 0;
							if (sound->fadeProc) (*sound->fadeProc)(sound, sound->fadeCookie);
							if (fadeFlags & kSoundFadeEndStop)
							{
								sound->Stop();
								break;
							}
						}
					}
					
					if (updateVolume) sound->UpdateVolume();
					if ((spatialized) || (sound->updateFlags & kSoundUpdateFrequency)) sound->UpdateFrequency();
					if (sound->updateFlags & kSoundUpdateReflections) sound->UpdateReflections();
					
					sound->updateFlags = 0;
					break;
				}
				
				case kSoundCompleted:
				{
					sound->soundState = kSoundStopped;
					
					int32 index = sound->tableIndex;
					if (index >= 0)
					{
						activeSoundTable[index] = nullptr;
						sound->tableIndex = -1;
					}
					
					bool persistent = ((sound->soundFlags & kSoundPersistent) != 0);
					sound->CallCompletionProc();
					if (!persistent) delete sound;
					break;
				}
			}
		}
		else
		{
			if (sound->streamRelease)
			{
				if (sound->mixRelease)
				{
					int32 index = sound->tableIndex;
					if (index >= 0) activeSoundTable[index] = nullptr;
					delete sound;
				}
			}
			else
			{
				streamSignal->Trigger(1);
			}
		}
		
		sound = next;
	}
	
	SoundRoom *room = releasedRoomList.First();
	while (room)
	{
		SoundRoom *next = room->Next();
		if (room->tableIndex < 0) delete room;
		room = next;
	}
}

// ZYURVUR
