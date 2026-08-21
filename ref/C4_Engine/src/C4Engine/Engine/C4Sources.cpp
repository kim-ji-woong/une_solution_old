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


#include "C4World.h"
#include "C4Configuration.h"


using namespace C4;


SoundData::SoundData(const char *name)
{
	soundName = name;
}

SoundData::~SoundData()
{
}


SourceObject::SourceObject(SourceType type) : Object(kObjectSource)
{
	sourceType = type;
	sourceFlags = kSourceInitialPlay;
	
	initialSourceVolume = 1.0F;
	initialSourceFrequency = 1.0F;
	
	soundGroupType = 0;
}

SourceObject::SourceObject(SourceType type, const char *name) : Object(kObjectSource)
{
	sourceType = type;
	sourceFlags = kSourceInitialPlay;
	
	initialSourceVolume = 1.0F;
	initialSourceFrequency = 1.0F;
	
	soundGroupType = 0;
	
	if (name) soundList.Append(new SoundData(name));
}

SourceObject::~SourceObject()
{
}

SourceObject *SourceObject::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kSourceAmbient:
			
			return (new AmbientSourceObject);
		
		case kSourceOmni:
			
			return (new OmniSourceObject);
		
		case kSourceDirected:
			
			return (new DirectedSourceObject);
	}
	
	return (nullptr);
}

void SourceObject::PackType(Packer& data) const
{
	Object::PackType(data);
	data << sourceType;
}

void SourceObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	data << ChunkHeader('FLAG', 4);
	data << sourceFlags;
	
	data << ChunkHeader('VOLU', 4);
	data << initialSourceVolume;
	
	data << ChunkHeader('FREQ', 4);
	data << initialSourceFrequency;
	
	data << ChunkHeader('GRUP', 4);
	data << soundGroupType;
	
	const SoundData *sound = soundList.First();
	while (sound)
	{
		PackHandle handle = data.BeginChunk('SOND');
		data << sound->GetSoundName();
		data.EndChunk(handle);
		
		sound = sound->Next();
	}
	
	data << TerminatorChunk;
}

void SourceObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<SourceObject>(data, unpackFlags);
} 

bool SourceObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags) 
{ 
	switch (chunkHeader->chunkType) 
	{
		case 'FLAG': 
		
		#if C4LEGACY
		
			case 'DATA': 
		
		#endif
			
			data >> sourceFlags; 
			return (true);
		
		case 'VOLU':
			
			data >> initialSourceVolume;
			return (true);
		
		case 'FREQ':
			
			data >> initialSourceFrequency;
			return (true);
		
		case 'GRUP':
		
			data >> soundGroupType;
			return (true);
		
		case 'SOND':
		{
			ResourceName	name;
			
			data >> name;
			soundList.Append(new SoundData(name));
			return (true);
		}
	}
	
	return (false);
}

void *SourceObject::BeginSettingsUnpack(void)
{
	soundList.Purge();
	return (nullptr);
}

int32 SourceObject::GetCategoryCount(void) const
{
	return (1);
}

Type SourceObject::GetCategoryType(int32 index, const char **title) const
{
	if (index == 0)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kObjectSource));
		return (kObjectSource);
	}
	
	return (0);
}

int32 SourceObject::GetCategorySettingCount(Type category) const
{
	if (category == kObjectSource) return (9);
	return (0);
}

Setting *SourceObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kObjectSource)
	{
		if (flags & kConfigurationScript) return (nullptr);
		
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID(kObjectSource, 'SORC'));
			return (new HeadingSetting(kObjectSource, title));
		}
		
		if (index == 1)
		{
			const char *picker = table->GetString(StringID(kObjectSource, 'SORC', 'PICK'));
			
			if (GetSourceType() != kSourceAmbient)
			{
				const SoundData *data = GetFirstSound();
				const char *name = (data) ? data->GetSoundName() : nullptr;
				
				const char *title = table->GetString(StringID(kObjectSource, 'SORC', 'SNAM'));
				return (new ResourceSetting('SNAM', name, title, picker, SoundResource::GetDescriptor()));
			}
			
			const char *title = table->GetString(StringID(kObjectSource, 'SORC', 'LIST'));
			MultiResourceSetting *setting = new MultiResourceSetting('LIST', title, picker, SoundResource::GetDescriptor());
			
			const SoundData *data = GetFirstSound();
			while (data)
			{
				setting->AddResourceName(data->GetSoundName());
				data = data->Next();
			}
			
			return (setting);
		}
		
		if (index == 2)
		{
			const char *title = table->GetString(StringID(kObjectSource, 'SORC', 'STRM'));
			return (new BooleanSetting('STRM', ((sourceFlags & kSourceStream) != 0), title));
		}
		
		if (index == 3)
		{
			const char *title = table->GetString(StringID(kObjectSource, 'SORC', 'PLAY'));
			return (new BooleanSetting('PLAY', ((sourceFlags & kSourceInitialPlay) != 0), title));
		}
		
		if (index == 4)
		{
			const char *title = table->GetString(StringID(kObjectSource, 'SORC', 'LOOP'));
			return (new BooleanSetting('LOOP', ((sourceFlags & kSourceLoop) != 0), title));
		}
		
		if (index == 5)
		{
			const char *title = table->GetString(StringID(kObjectSource, 'SORC', 'RAND'));
			return (new BooleanSetting('RAND', ((sourceFlags & kSourceRandom) != 0), title));
		}
		
		if (index == 6)
		{
			int32 selection = 0;
			int32 count = 1;
			
			SoundGroupType type = soundGroupType;
			const SoundGroup *group = TheSoundMgr->GetFirstSoundGroup();
			while (group)
			{
				if (group->GetSoundGroupType() == type) selection = count;
				
				count++;
				group = group->Next();
			}
			
			const char *title = table->GetString(StringID(kObjectSource, 'SORC', 'GRUP'));
			MenuSetting *menu = new MenuSetting('GRUP', selection, title, count);
			
			menu->SetMenuItemString(0, table->GetString(StringID(kObjectSource, 'SORC', 'GRUP', 'DFLT')));
			
			count = 1;
			group = TheSoundMgr->GetFirstSoundGroup();
			while (group)
			{
				menu->SetMenuItemString(count, group->GetSoundGroupName());
				
				count++;
				group = group->Next();
			}
			
			return (menu);
		}
		
		if (index == 7)
		{
			const char *title = table->GetString(StringID(kObjectSource, 'SORC', 'VOLU'));
			return (new IntegerSetting('VOLU', (int32) (initialSourceVolume * 100.0 + 0.5F), title, 1, 100, 1));
		}
		
		if (index == 8)
		{
			const char *title = table->GetString(StringID(kObjectSource, 'SORC', 'FREQ'));
			return (new FloatSetting('FREQ', initialSourceFrequency, title, 0.25F, 4.0F, 0.05F));
		}
	}
	
	return (nullptr);
}

void SourceObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kObjectSource)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'SNAM')
		{
			soundList.Purge();
			
			const char *name = static_cast<const ResourceSetting *>(setting)->GetResourceName();
			if (name[0] != 0) soundList.Append(new SoundData(name));
		}
		else if (identifier == 'LIST')
		{
			soundList.Purge();
			
			const MultiResourceSetting *multiResourceSetting = static_cast<const MultiResourceSetting *>(setting);
			int32 count = multiResourceSetting->GetResourceCount();
			for (machine a = 0; a < count; a++)
			{
				ResourceName name(multiResourceSetting->GetResourceName(a));
				if (name[0] != 0) soundList.Append(new SoundData(name));
			}
		}
		else if (identifier == 'STRM')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) sourceFlags |= kSourceStream;
			else sourceFlags &= ~kSourceStream;
		}
		else if (identifier == 'PLAY')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) sourceFlags |= kSourceInitialPlay;
			else sourceFlags &= ~kSourceInitialPlay;
		}
		else if (identifier == 'LOOP')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) sourceFlags |= kSourceLoop;
			else sourceFlags &= ~kSourceLoop;
		}
		else if (identifier == 'RAND')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) sourceFlags |= kSourceRandom;
			else sourceFlags &= ~kSourceRandom;
		}
		else if (identifier == 'GRUP')
		{
			int32 selection = static_cast<const MenuSetting *>(setting)->GetMenuSelection();
			if (selection == 0)
			{
				soundGroupType = 0;
			}
			else
			{
				int32 count = 1;
				const SoundGroup *group = TheSoundMgr->GetFirstSoundGroup();
				while (group)
				{
					if (count == selection)
					{
						soundGroupType = group->GetSoundGroupType();
						break;
					}
					
					count++;
					group = group->Next();
				}
			}
		}
		else if (identifier == 'VOLU')
		{
			int32 value = static_cast<const IntegerSetting *>(setting)->GetIntegerValue();
			initialSourceVolume = (float) value * 0.01F;
		}
		else if (identifier == 'FREQ')
		{
			initialSourceFrequency = static_cast<const FloatSetting *>(setting)->GetFloatValue();
		}
	}
}


AmbientSourceObject::AmbientSourceObject() : SourceObject(kSourceAmbient)
{
	fadeTime = 0;
	loopIndex = 0;
}

AmbientSourceObject::AmbientSourceObject(const char *name) : SourceObject(kSourceAmbient, name)
{
	fadeTime = 0;
	loopIndex = 0;
}

AmbientSourceObject::~AmbientSourceObject()
{
}

void AmbientSourceObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	SourceObject::Pack(data, packFlags);
	
	data << ChunkHeader('FADE', 4);
	data << fadeTime;
	
	data << ChunkHeader('LIDX', 4);
	data << loopIndex;
	
	data << TerminatorChunk;
}

void AmbientSourceObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	SourceObject::Unpack(data, unpackFlags);
	UnpackChunkList<AmbientSourceObject>(data, unpackFlags);
}

bool AmbientSourceObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'FADE':
			
			data >> fadeTime;
			return (true);
		
		case 'LIDX':
			
			data >> loopIndex;
			return (true);
	}
	
	return (false);
}

int32 AmbientSourceObject::GetCategorySettingCount(Type category) const
{
	int32 count = SourceObject::GetCategorySettingCount(category);
	if (category == kObjectSource) count += 2;
	return (count);
}

Setting *AmbientSourceObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kObjectSource)
	{
		int32 count = SourceObject::GetCategorySettingCount(kObjectSource);
		if (index >= count)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const StringTable *table = TheInterfaceMgr->GetStringTable();
			
			if (index == count)
			{
				const char *title = table->GetString(StringID(kObjectSource, 'SORC', 'FADE'));
				return (new TextSetting('FADE', (float) fadeTime * 0.001F, title));
			}
			
			if (index == count + 1)
			{
				const char *title = table->GetString(StringID(kObjectSource, 'SORC', 'LIDX'));
				return (new TextSetting('LIDX', Text::IntegerToString(loopIndex), title, 2, &EditTextWidget::NumberFilter));
			}
			
			return (nullptr);
		}
	}
	
	return (SourceObject::GetCategorySetting(category, index, flags));
}

void AmbientSourceObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kObjectSource)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'FADE')
		{
			const char *text = static_cast<const TextSetting *>(setting)->GetText();
			fadeTime = MaxZero((int32) (Text::StringToFloat(text) * 1000.0F));
		}
		else if (identifier == 'LIDX')
		{
			const char *text = static_cast<const TextSetting *>(setting)->GetText();
			loopIndex = Text::StringToInteger(text);
		}
		else
		{
			SourceObject::SetCategorySetting(kObjectSource, setting);
		}
	}
}


OmniSourceObject::OmniSourceObject() : SourceObject(kSourceOmni)
{
	Initialize();
}

OmniSourceObject::OmniSourceObject(SourceType type) : SourceObject(type)
{
	Initialize();
}

OmniSourceObject::OmniSourceObject(SourceType type, const char *name, float range) : SourceObject(type, name)
{
	sourceRange = range;
	Initialize();
}

OmniSourceObject::OmniSourceObject(const char *name, float range) : SourceObject(kSourceOmni, name)
{
	sourceRange = range;
	Initialize();
}

OmniSourceObject::~OmniSourceObject()
{
}

void OmniSourceObject::Initialize(void)
{
	SetSourceFlags(GetSourceFlags() | (kSourceDopplerShift | kSourceDistanceDelay | kSourceReverb));
	
	reflectionVolume = 1.0F;
	reflectionHFVolume = 1.0F;
}

void OmniSourceObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	SourceObject::Pack(data, packFlags);
	
	data << ChunkHeader('RANG', 4);
	data << sourceRange;
	
	data << ChunkHeader('REFL', 8);
	data << reflectionVolume;
	data << reflectionHFVolume;
	
	data << TerminatorChunk;
}

void OmniSourceObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	SourceObject::Unpack(data, unpackFlags);
	UnpackChunkList<OmniSourceObject>(data, unpackFlags);
}

bool OmniSourceObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'RANG':
			
			data >> sourceRange;
			return (true);
		
		case 'REFL':
			
			data >> reflectionVolume;
			data >> reflectionHFVolume;
			return (true);
	}
	
	return (false);
}

int32 OmniSourceObject::GetCategorySettingCount(Type category) const
{
	int32 count = SourceObject::GetCategorySettingCount(category);
	if (category == kObjectSource) count += 8;
	return (count);
}

Setting *OmniSourceObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kObjectSource)
	{
		int32 count = SourceObject::GetCategorySettingCount(kObjectSource);
		if (index >= count)
		{
			const StringTable *table = TheInterfaceMgr->GetStringTable();
			
			if (index == count)
			{
				const char *title = table->GetString(StringID(kObjectSource, 'SPAT'));
				return (new HeadingSetting('SPAT', title));
			}
			
			if (index == count + 1)
			{
				const char *title = table->GetString(StringID(kObjectSource, 'SPAT', 'DPLR'));
				return (new BooleanSetting('DPLR', ((GetSourceFlags() & kSourceDopplerShift) != 0), title));
			}
			
			if (index == count + 2)
			{
				const char *title = table->GetString(StringID(kObjectSource, 'SPAT', 'DIST'));
				return (new BooleanSetting('DIST', ((GetSourceFlags() & kSourceDistanceDelay) != 0), title));
			}
			
			if (index == count + 3)
			{
				const char *title = table->GetString(StringID(kObjectSource, 'SPAT', 'RVRB'));
				return (new BooleanSetting('RVRB', ((GetSourceFlags() & kSourceReverb) != 0), title));
			}
			
			if (index == count + 4)
			{
				const char *title = table->GetString(StringID(kObjectSource, 'SPAT', 'OBST'));
				return (new BooleanSetting('OBST', ((GetSourceFlags() & kSourceObstruction) != 0), title));
			}
			
			if (index == count + 5)
			{
				if (flags & kConfigurationScript) return (nullptr);
				
				const char *title = table->GetString(StringID(kObjectSource, 'SPAT', 'EXTN'));
				return (new BooleanSetting('EXTN', ((GetSourceFlags() & kSourceExternalZone) != 0), title));
			}
			
			if (index == count + 6)
			{
				const char *title = table->GetString(StringID(kObjectSource, 'SPAT', 'REFL'));
				return (new IntegerSetting('REFL', (int32) (reflectionVolume * 100.0 + 0.5F), title, 0, 100, 1));
			}
			
			if (index == count + 7)
			{
				const char *title = table->GetString(StringID(kObjectSource, 'SPAT', 'REFH'));
				return (new IntegerSetting('REFH', (int32) (reflectionHFVolume * 100.0 + 0.5F), title, 0, 100, 1));
			}
			
			return (nullptr);
		}
	}
	
	return (SourceObject::GetCategorySetting(category, index, flags));
}

void OmniSourceObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kObjectSource)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'DPLR')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) SetSourceFlags(GetSourceFlags() | kSourceDopplerShift);
			else SetSourceFlags(GetSourceFlags() & ~kSourceDopplerShift);
		}
		else if (identifier == 'DIST')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) SetSourceFlags(GetSourceFlags() | kSourceDistanceDelay);
			else SetSourceFlags(GetSourceFlags() & ~kSourceDistanceDelay);
		}
		else if (identifier == 'RVRB')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) SetSourceFlags(GetSourceFlags() | kSourceReverb);
			else SetSourceFlags(GetSourceFlags() & ~kSourceReverb);
		}
		else if (identifier == 'OBST')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) SetSourceFlags(GetSourceFlags() | kSourceObstruction);
			else SetSourceFlags(GetSourceFlags() & ~kSourceObstruction);
		}
		else if (identifier == 'EXTN')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) SetSourceFlags(GetSourceFlags() | kSourceExternalZone);
			else SetSourceFlags(GetSourceFlags() & ~kSourceExternalZone);
		}
		else if (identifier == 'REFL')
		{
			reflectionVolume = (float) static_cast<const IntegerSetting *>(setting)->GetIntegerValue() * 0.01F;
		}
		else if (identifier == 'REFH')
		{
			reflectionHFVolume = (float) static_cast<const IntegerSetting *>(setting)->GetIntegerValue() * 0.01F;
		}
		else
		{
			SourceObject::SetCategorySetting(kObjectSource, setting);
		}
	}
}

int32 OmniSourceObject::GetObjectSize(float *size) const
{
	size[0] = sourceRange;
	return (1);
}

void OmniSourceObject::SetObjectSize(const float *size)
{
	sourceRange = size[0];
}


DirectedSourceObject::DirectedSourceObject() : OmniSourceObject(kSourceDirected)
{
	outerConeVolume = 0.0F;
	outerConeHFVolume = 1.0F;
}

DirectedSourceObject::DirectedSourceObject(const char *name, float range, float apex) : OmniSourceObject(kSourceDirected, name, range)
{
	apexTangent = apex;
	
	outerConeVolume = 0.0F;
	outerConeHFVolume = 1.0F;
}

DirectedSourceObject::~DirectedSourceObject()
{
}

void DirectedSourceObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	OmniSourceObject::Pack(data, packFlags);
	
	data << ChunkHeader('APEX', 4);
	data << apexTangent;
	
	data << ChunkHeader('CONE', 8);
	data << outerConeVolume;
	data << outerConeHFVolume;
	
	data << TerminatorChunk;
}

void DirectedSourceObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	OmniSourceObject::Unpack(data, unpackFlags);
	UnpackChunkList<DirectedSourceObject>(data, unpackFlags);
}

bool DirectedSourceObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'APEX':
			
			data >> apexTangent;
			return (true);
		
		case 'CONE':
			
			data >> outerConeVolume;
			data >> outerConeHFVolume;
			return (true);
	}
	
	return (false);
}

int32 DirectedSourceObject::GetCategorySettingCount(Type category) const
{
	int32 count = OmniSourceObject::GetCategorySettingCount(category);
	if (category == kObjectSource) count += 2;
	return (count);
}

Setting *DirectedSourceObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kObjectSource)
	{
		int32 count = OmniSourceObject::GetCategorySettingCount(kObjectSource);
		if (index >= count)
		{
			const StringTable *table = TheInterfaceMgr->GetStringTable();
			
			if (index == count)
			{
				const char *title = table->GetString(StringID(kObjectSource, 'SPAT', 'CONV'));
				return (new IntegerSetting('CONV', (int32) (outerConeVolume * 100.0 + 0.5F), title, 0, 100, 1));
			}
			
			if (index == count + 1)
			{
				const char *title = table->GetString(StringID(kObjectSource, 'SPAT', 'CONH'));
				return (new IntegerSetting('CONH', (int32) (outerConeHFVolume * 100.0 + 0.5F), title, 0, 100, 1));
			}
			
			return (nullptr);
		}
	}
	
	return (OmniSourceObject::GetCategorySetting(category, index, flags));
}

void DirectedSourceObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kObjectSource)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'CONV')
		{
			outerConeVolume = (float) static_cast<const IntegerSetting *>(setting)->GetIntegerValue() * 0.01F;
		}
		else if (identifier == 'CONH')
		{
			outerConeHFVolume = (float) static_cast<const IntegerSetting *>(setting)->GetIntegerValue() * 0.01F;
		}
		else
		{
			OmniSourceObject::SetCategorySetting(kObjectSource, setting);
		}
	}
}

int32 DirectedSourceObject::GetObjectSize(float *size) const
{
	size[0] = GetSourceRange();
	size[1] = apexTangent;
	return (2);
}

void DirectedSourceObject::SetObjectSize(const float *size)
{
	SetSourceRange(size[0]);
	apexTangent = size[1];
}


Source::Source(SourceType type) :
		Node(kNodeSource),
		playTask(&PlayTask, this)
{
	sourceType = type;
	sourceState = 0;
	
	sourceVolume = 1.0F;
	sourceFrequency = 1.0F;
	
	Initialize();
}

Source::Source(SourceType type, bool persistent) :
		Node(kNodeSource),
		playTask(&PlayTask, this)
{
	sourceType = type;
	sourceState = (persistent) ? kSourcePersistent : 0;
	
	sourceVolume = 1.0F;
	sourceFrequency = 1.0F;
	
	Initialize();
}

Source::Source(const Source& source) :
		Node(source),
		playTask(&PlayTask, this)
{
	sourceType = source.sourceType;
	sourceState = source.sourceState & kSourcePersistent;
	
	sourceVolume = source.sourceVolume;
	sourceFrequency = source.sourceFrequency;
	
	Initialize();
}

Source::~Source()
{
	if (sourceSound) sourceSound->Release();
	delete rootRegion;
}

void Source::Initialize(void)
{
	sourceLifeTime = 0;
	
	rootRegion = nullptr;
	sourceSound = nullptr;
	
	SetActiveUpdateFlags(GetActiveUpdateFlags() & ~kUpdateBoundingSphere);
}

Source *Source::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kSourceAmbient:
			
			return (new AmbientSource);
		
		case kSourceOmni:
			
			return (new OmniSource);
		
		case kSourceDirected:
			
			return (new DirectedSource);
	}
	
	return (nullptr);
}

void Source::PackType(Packer& data) const
{
	Node::PackType(data);
	data << sourceType;
}

void Source::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Node::Pack(data, packFlags);
	
	unsigned_int32 state = sourceState;
	if (packFlags & kPackEditor) state &= kSourcePersistent;
	else state &= ~kSourceEngaged;
	
	data << ChunkHeader('STAT', 4);
	data << state;
	
	data << ChunkHeader('LIFE', 4);
	data << sourceLifeTime;
	
	data << ChunkHeader('VOLU', 4);
	data << sourceVolume;
	
	data << ChunkHeader('FREQ', 4);
	data << sourceFrequency;
	
	data << TerminatorChunk;
}

void Source::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Node::Unpack(data, unpackFlags);
	UnpackChunkList<Source>(data, unpackFlags);
}

bool Source::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'STAT':
			
			data >> sourceState;
			return (true);
		
		case 'LIFE':
			
			data >> sourceLifeTime;
			return (true);
		
		#if C4LEGACY
		
			case 'DATA':
				
				data >> sourceState;
				data >> sourceLifeTime;
				return (true);
		
		#endif
		
		case 'VOLU':
			
			data >> sourceVolume;
			return (true);
		
		case 'FREQ':
			
			data >> sourceFrequency;
			return (true);
	}
	
	return (false);
}

void Source::Preprocess(void)
{
	Node::Preprocess();
	
	if (!GetManipulator())
	{
		const SourceObject *object = GetObject();
		
		unsigned_int32 state = sourceState;
		if (!(state & kSourceInitialized))
		{
			sourceState = state | kSourceInitialized;
			
			sourceVolume = object->GetInitialSourceVolume();
			sourceFrequency = object->GetInitialSourceFrequency();
		}
		
		if ((state & (kSourcePlaying | kSourceStopped)) == 0)
		{
			if ((state & kSourcePlaying) || (object->GetSourceFlags() & kSourceInitialPlay)) TheTimeMgr->AddTask(&playTask);
		}
	}
}

void Source::Neutralize(void)
{
	if (!GetManipulator()) Stop();
	Node::Neutralize();
}

void Source::LoadSound(void)
{
	const SourceObject *object = GetObject();
	unsigned_int32 flags = object->GetSourceFlags();
	
	sourceSound = new Sound;
	
	const SoundData *soundData = object->GetFirstSound();
	if (soundData)
	{
		if (!(flags & kSourceStream))
		{
			sourceSound->Load(soundData->GetSoundName());
		}
		else
		{
			WaveStreamer *streamer = new WaveStreamer;
			if (sourceSound->Stream(streamer) == kSoundOkay)
			{
				do
				{
					if (streamer->AddComponent(soundData->GetSoundName()) != kSoundOkay) break;
					soundData = soundData->Next();
				} while (soundData);
			}
		}
	}
	
	sourceSound->SetSoundFlags(kSoundPersistent);
	sourceSound->SetTransformable(this);
	sourceSound->SetCompletionProc(&SoundComplete, this);
	if (flags & kSourceLoop) sourceSound->SetLoopCount(kSoundLoopInfinite);
	
	sourceDuration = sourceSound->GetDuration();
	if (sourceLifeTime <= 0) sourceLifeTime = sourceDuration;
}

void Source::Move(void)
{
	if (!(GetObject()->GetSourceFlags() & kSourceLoop))
	{
		if (sourceSound)
		{
			int32 time = sourceLifeTime - TheTimeMgr->GetSystemDeltaTime();
			sourceLifeTime = time;
			
			if (time <= 0)
			{
				unsigned_int32 state = sourceState;
				if (!(state & kSourceEngaged))
				{
					sourceState = (state & ~kSourcePlaying) | kSourceStopped;
					CallCompletionProc();
					
					if (!(state & kSourcePersistent)) delete this;
				}
			}
		}
		else
		{
			LoadSound();
		}
	}
}

void Source::SoundComplete(Sound *sound, void *cookie)
{
	Source *source = static_cast<Source *>(cookie);
	
	unsigned_int32 state = source->sourceState;
	if (state & kSourcePersistent)
	{
		source->ResetSound();
		source->CallCompletionProc();
	}
	else
	{
		source->CallCompletionProc();
		delete source;
	}
}

void Source::InitializeSound(Sound *sound)
{
	sound->SetSoundProperty(kSoundVolume, sourceVolume);
	sound->SetSoundProperty(kSoundFrequency, sourceFrequency);
	
	SoundGroupType type = GetObject()->GetSoundGroupType();
	if (type != 0) sound->SetSoundGroup(TheSoundMgr->FindSoundGroup(type));
}

void Source::ResetSound(void)
{
	sourceState = (sourceState & ~(kSourcePlaying | kSourceEngaged)) | kSourceStopped;
	SetActiveUpdateFlags(GetActiveUpdateFlags() & ~kUpdatePostBounding);
	
	ListElement<Source>::Detach();
	
	delete rootRegion;
	rootRegion = nullptr;
	
	if (sourceSound)
	{
		sourceSound->Release();
		sourceSound = nullptr;
	}
	
	sourceLifeTime = 0;
}

void Source::PlayTask(DeferredTask *task, void *cookie)
{
	static_cast<Source *>(cookie)->Play();
}

void Source::Play(void)
{
	unsigned_int32 state = sourceState;
	if (!(state & kSourcePlaying))
	{
		sourceState = (state | kSourcePlaying) & ~kSourceStopped;
		if (sourceSound) sourceLifeTime = sourceDuration;
	}
}

void Source::Stop(void)
{
	if (sourceState & kSourceEngaged) Disengage();
	ResetSound();
}

bool Source::Engage(void)
{
	unsigned_int32 flags = GetObject()->GetSourceFlags();
	if ((sourceLifeTime > 0) || (flags & kSourceLoop))
	{
		sourceState |= kSourceEngaged;
		if (!sourceSound) LoadSound();
		
		if (flags & kSourceLoop)
		{
			if (flags & kSourceRandom) sourceSound->SetStartTime(Math::Random(sourceDuration));
		}
		else
		{
			sourceSound->SetStartTime(sourceDuration - sourceLifeTime);
		}
		
		InitializeSound(sourceSound);
		sourceSound->Play();
		return (true);
	}
	
	return (false);
}

void Source::Disengage(void)
{
	sourceState &= ~kSourceEngaged;
	
	if (GetObject()->GetSourceFlags() & kSourceLoop)
	{
		sourceSound->Release();
		sourceSound = nullptr;
	}
	else
	{
		sourceSound->Stop();
	}
}

void Source::SetSourceVolume(float volume)
{
	sourceVolume = volume;
	if (sourceSound) sourceSound->SetSoundProperty(kSoundVolume, volume);
}

void Source::SetSourceFrequency(float frequency)
{
	sourceFrequency = frequency;
	if (sourceSound) sourceSound->SetSoundProperty(kSoundFrequency, frequency);
}


AmbientSource::AmbientSource() : Source(kSourceAmbient)
{
}

AmbientSource::AmbientSource(const char *name, bool persistent) : Source(kSourceAmbient, persistent)
{
	SetNewObject(new AmbientSourceObject(name));
}

AmbientSource::AmbientSource(const AmbientSource& ambientSource) : Source(ambientSource)
{
}

AmbientSource::~AmbientSource()
{
}

Node *AmbientSource::Replicate(void) const
{
	return (new AmbientSource(*this));
}

void AmbientSource::Preprocess(void)
{
	Source::Preprocess();
	
	if (!GetManipulator())
	{
		if ((sourceState & (kSourcePlaying | kSourceStopped | kSourceEngaged)) == kSourcePlaying) Play();
	}
}

void AmbientSource::InitializeSound(Sound *sound)
{
	Source::InitializeSound(sound);
	
	sound->SetSoundProperty(kSoundVolume, (GetObject()->GetFadeTime() == 0) ? GetSourceVolume() : 0.0F);
}

void AmbientSource::Play(void)
{
	Source::Play();
	Engage();
}

bool AmbientSource::Engage(void)
{
	if (Source::Engage())
	{
		const AmbientSourceObject *object = GetObject();
		
		int32 fadeTime = object->GetFadeTime();
		if (fadeTime != 0) sourceSound->Fade(GetSourceVolume(), fadeTime);
		
		sourceSound->SetLoopIndex(object->GetLoopIndex());
		return (true);
	}
	
	return (false);
}

void AmbientSource::FadeOut(int32 time)
{
	if (sourceSound) sourceSound->Fade(0.0F, time, true);
	
	sourceState |= kSourceStopped;
}

Sound *AmbientSource::ExtractSound(void)
{
	Sound *sound = sourceSound;
	if (sound)
	{
		sound->SetCompletionProc(nullptr);
		sourceSound = nullptr;
		ResetSound();
		return (sound);
	}
	
	return (nullptr);
}


OmniSource::OmniSource() : Source(kSourceOmni)
{
	sourceVelocity.Set(0.0F, 0.0F, 0.0F);
}

OmniSource::OmniSource(SourceType type) : Source(type)
{
	sourceVelocity.Set(0.0F, 0.0F, 0.0F);
}

OmniSource::OmniSource(SourceType type, bool persistent) : Source(type, persistent)
{
	sourceVelocity.Set(0.0F, 0.0F, 0.0F);
}

OmniSource::OmniSource(const char *name, float range, bool persistent) : Source(kSourceOmni, persistent)
{
	SetNewObject(new OmniSourceObject(name, range));
	
	sourceVelocity.Set(0.0F, 0.0F, 0.0F);
}

OmniSource::OmniSource(const OmniSource& omniSource) : Source(omniSource)
{
	sourceVelocity = omniSource.sourceVelocity;
}

OmniSource::~OmniSource()
{
}

Node *OmniSource::Replicate(void) const
{
	return (new OmniSource(*this));
}

void OmniSource::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Source::Pack(data, packFlags);
	
	data << ChunkHeader('VELO', sizeof(Vector3D));
	data << sourceVelocity;
	
	data << TerminatorChunk;
}

void OmniSource::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Source::Unpack(data, unpackFlags);
	UnpackChunkList<OmniSource>(data, unpackFlags);
}

bool OmniSource::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'VELO':
			
			data >> sourceVelocity;
			return (true);
	}
	
	return (false);
}

void OmniSource::Preprocess(void)
{
	sourceRoom = nullptr;
	sourceObstruction = nullptr;
	playRegionCount = 0;
	
	Source::Preprocess();
	
	if (!GetManipulator())
	{
		sourceRange = GetObject()->GetSourceRange();
		if (Playing()) SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdatePostBounding);
	}
}

void OmniSource::EnterZone(Zone *zone)
{
	Source::EnterZone(zone);
	
	const AcousticsSpace *acousticsSpace = zone->GetConnectedAcousticsSpace();
	sourceRoom = (acousticsSpace) ? acousticsSpace->GetSoundRoom() : nullptr;
	if (sourceSound) sourceSound->SetSoundRoom(sourceRoom);
}

void OmniSource::CalculatePostBounding(void)
{
	Zone	*zone;
	
	delete rootRegion;
	rootRegion = nullptr;
	
	unsigned_int32 sourceFlags = GetObject()->GetSourceFlags();
	
	if (!(sourceFlags & kSourceExternalZone)) zone = GetOwningZone();
	else zone = GetWorld()->FindZone(GetWorldPosition());
	
	if (zone)
	{
		SourceRegion *region = new SourceRegion(this, zone);
		zone->AddSourceRegion(region);
		
		rootRegion = region;
		CalculatePermeation(region);
	}
	
	if ((sourceState & kSourceEngaged) && (sourceFlags & kSourceObstruction)) DetectObstruction();
}

void OmniSource::CalculatePermeation(SourceRegion *region)
{
	Zone *zone = region->GetZone();
	zone->SetExclusionMask(1);
	
	const Portal *portal = zone->GetFirstPortal();
	while (portal)
	{
		if (portal->GetPortalType() == kPortalDirect)
		{
			if ((portal->Enabled()) || ((!region->GetSuperNode()) && (zone->GetObject()->GetZoneFlags() & kZoneTransition)))
			{
				Zone *connectedZone = portal->GetConnectedZone();
				if ((connectedZone) && (connectedZone->GetExclusionMask() == 0))
				{
					const Point3D& permeatedPosition = region->GetPermeatedPosition();
					Point3D portalPosition = portal->CalculateClosestBoundaryPoint(permeatedPosition);
					
					float length = Magnitude(portalPosition - permeatedPosition) + region->GetPermeatedPathLength();
					if (length < sourceRange)
					{
						SourceRegion *sourceRegion = connectedZone->GetFirstSourceRegion();
						while (sourceRegion)
						{
							if (sourceRegion->GetSource() == this) break;
							sourceRegion = sourceRegion->GetNextSourceRegion();
						}
						
						if (sourceRegion)
						{
							if (length < sourceRegion->GetPermeatedPathLength())
							{
								sourceRegion->SetPermeatedPortal(portal, portalPosition, length);
								
								region->AddSubnode(sourceRegion);
								CalculatePermeation(sourceRegion);
							}
						}
						else
						{
							SourceRegion *newRegion = new SourceRegion(this, connectedZone);
							newRegion->SetPermeatedPortal(portal, portalPosition, length);
							connectedZone->AddSourceRegion(newRegion);
							
							region->AddSubnode(newRegion);
							CalculatePermeation(newRegion);
						}
					}
				}
			}
		}
		
		portal = portal->Next();
	}
	
	const Bond *bond = zone->GetZoneSite()->GetFirstOutgoingEdge();
	while (bond)
	{
		Zone *bondZone = static_cast<Zone *>(bond->GetFinishElement());
		if (bondZone->GetExclusionMask() == 0)
		{
			SourceRegion *sourceRegion = bondZone->GetFirstSourceRegion();
			while (sourceRegion)
			{
				if (sourceRegion->GetSource() == this) break;
				sourceRegion = sourceRegion->GetNextSourceRegion();
			}
			
			if (sourceRegion)
			{
				float length = region->GetPermeatedPathLength();
				if (length < sourceRegion->GetPermeatedPathLength())
				{
					sourceRegion->SetPermeatedPortal(region->GetPermeatedPortal(), region->GetPermeatedPosition(), length);
					sourceRegion->SetPrimaryRegion(region);
					
					region->AddSubnode(sourceRegion);
					CalculatePermeation(sourceRegion);
				}
			}
			else
			{
				SourceRegion *newRegion = new SourceRegion(this, bondZone, region);
				bondZone->AddSourceRegion(newRegion);
				
				region->AddSubnode(newRegion);
				CalculatePermeation(newRegion);
			}
		}
		
		bond = bond->GetNextOutgoingEdge();
	}
	
	zone->SetExclusionMask(0);
}

void OmniSource::DetectObstruction(void)
{
	const AcousticsProperty *property = GetWorld()->DetectObstruction(GetWorldPosition());
	if (sourceObstruction != property)
	{
		sourceObstruction = property;
		
		const OmniSourceObject *object = GetObject();
		if (property)
		{
			sourceSound->SetSoundProperty(kSoundDirectVolume, property->GetDirectVolume());
			sourceSound->SetSoundProperty(kSoundDirectHFVolume, property->GetDirectHFVolume());
			sourceSound->SetSoundProperty(kSoundReflectionVolume, object->GetReflectionVolume() * property->GetReflectionVolume());
			sourceSound->SetSoundProperty(kSoundReflectionHFVolume, object->GetReflectionHFVolume() * property->GetReflectionHFVolume());
		}
		else
		{
			sourceSound->SetSoundProperty(kSoundDirectVolume, 1.0F);
			sourceSound->SetSoundProperty(kSoundDirectHFVolume, 1.0F);
			sourceSound->SetSoundProperty(kSoundReflectionVolume, object->GetReflectionVolume());
			sourceSound->SetSoundProperty(kSoundReflectionHFVolume, object->GetReflectionHFVolume());
		}
	}
}

void OmniSource::InitializeSound(Sound *sound)
{
	Source::InitializeSound(sound);
	
	const OmniSourceObject *object = GetObject();
	unsigned_int32 flags = object->GetSourceFlags();
	
	unsigned_int32 soundFlags = sound->GetSoundFlags() & ~(kSoundDopplerShift | kSoundDistanceDelay | kSoundReverb);
	if (flags & kSourceDopplerShift) soundFlags |= kSoundDopplerShift;
	if (flags & kSourceDistanceDelay) soundFlags |= kSoundDistanceDelay;
	if (flags & kSourceReverb) soundFlags |= kSoundReverb;
	sound->SetSoundFlags(soundFlags | kSoundSpatialized);
	
	sound->SetSoundProperty(kSoundDirectVolume, 1.0F);
	sound->SetSoundProperty(kSoundDirectHFVolume, 1.0F);
	sound->SetSoundProperty(kSoundReflectionVolume, object->GetReflectionVolume());
	sound->SetSoundProperty(kSoundReflectionHFVolume, object->GetReflectionHFVolume());
	sound->SetSoundProperty(kSoundMaxAttenDistance, object->GetSourceRange());
	
	sound->SetVelocity(sourceVelocity);
	sound->SetSoundRoom(sourceRoom);
	
	EndUpdate();
	if (flags & kSourceObstruction) DetectObstruction();
}

void OmniSource::ResetSound(void)
{
	Source::ResetSound();
	
	playRegionCount = 0;
}

void OmniSource::Play(void)
{
	Source::Play();
	
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdatePostBounding);
	GetWorld()->AddPlayingSource(this);
}

void OmniSource::Disengage(void)
{
	playRegionCount = 0;
	Source::Disengage();
}

void OmniSource::BeginUpdate(void)
{
	sourceState &= ~kSourceAudible;
	playRegionCount = 0;
}

void OmniSource::EndUpdate(void)
{
	int32 count = playRegionCount;
	sourceSound->SetSoundPathCount(count);
	
	for (machine a = 0; a < count; a++)
	{
		const SourceRegion *region = playRegion[a];
		sourceSound->SetSoundPathData(a, &region->GetAudiblePosition(), region->GetAudiblePathLength());
	}
}

void OmniSource::AddPlayRegion(SourceRegion *region, const Point3D& listenerPosition)
{
	region = region->GetPrimaryRegion();
	const Portal *portal = region->GetPermeatedPortal();
	if (portal)
	{
		int32 count = playRegionCount;
		if (count < kMaxSoundPathCount)
		{
			playRegion[count] = region;
			playRegionCount = count + 1;
			
			SourceRegion *baseRegion = region;
			baseRegion->SetAudibleSubregion(nullptr);
			
			const Point3D *p1 = &listenerPosition;
			float length = 0.0F;
			
			SourceRegion *superRegion = baseRegion->GetSuperNode()->GetPrimaryRegion();
			for (;;)
			{
				Point3D		q;
				
				SourceRegion *nextRegion = superRegion->GetSuperNode();
				if (nextRegion)
				{
					const Point3D& p2 = superRegion->GetPermeatedPosition();
					if (portal->CalculateClosestBoundaryPoint((p2 ^ *p1).Standardize(), &q))
					{
						length += Magnitude(q - *p1);
						baseRegion->SetAudiblePosition(q, length);
						superRegion->SetAudibleSubregion(baseRegion);
						
						p1 = &baseRegion->GetAudiblePosition();
						baseRegion = superRegion;
					}
				}
				else
				{
					const Point3D& p2 = GetWorldPosition();
					if (portal->CalculateClosestBoundaryPoint((p2 ^ *p1).Standardize(), &q))
					{
						length += Magnitude(q - *p1);
						baseRegion->SetAudiblePosition(q, length);
						length += Magnitude(p2 - q);
					}
					else
					{
						length += Magnitude(p2 - *p1);
						baseRegion->SetAudiblePosition(p2, length);
					}
					
					superRegion->SetAudibleSubregion(baseRegion);
					break;
				}
				
				portal = superRegion->GetPermeatedPortal();
				superRegion = nextRegion->GetPrimaryRegion();
			}
			
			SourceRegion *subregion = superRegion->GetAudibleSubregion();
			do
			{
				subregion->InvertAudiblePathLength(length);
				subregion = subregion->GetAudibleSubregion();
			} while (subregion);
		}
		
		const AcousticsSpace *prevSpace = region->GetZone()->GetConnectedAcousticsSpace();
		const SourceRegion *prevRegion = region;
		for (;;)
		{
			region = region->GetSuperNode();
			if (!region) break;
			
			region = region->GetPrimaryRegion();
			
			const AcousticsSpace *space = region->GetZone()->GetConnectedAcousticsSpace();
			if ((space) && (space != prevSpace))
			{
				SoundRoom *room = space->GetSoundRoom();
				room->SetOutputRoom((prevSpace) ? prevSpace->GetSoundRoom() : nullptr);
				room->SetRoomPosition(prevRegion->GetAudiblePosition());
				
				prevSpace = space;
				prevRegion = region;
			}
		}
	}
}

void OmniSource::SetSourceVelocity(const Vector3D& velocity)
{
	sourceVelocity = velocity;
	if (sourceSound) sourceSound->SetVelocity(velocity);
}


DirectedSource::DirectedSource() : OmniSource(kSourceDirected)
{
}

DirectedSource::DirectedSource(const char *name, float range, float apex, bool persistent) : OmniSource(kSourceDirected, persistent)
{
	SetNewObject(new DirectedSourceObject(name, range, apex));
}

DirectedSource::DirectedSource(const DirectedSource& directedSource) : OmniSource(directedSource)
{
}

DirectedSource::~DirectedSource()
{
}

Node *DirectedSource::Replicate(void) const
{
	return (new DirectedSource(*this));
}

void DirectedSource::InitializeSound(Sound *sound)
{
	OmniSource::InitializeSound(sound);
	sound->SetSoundFlags(sound->GetSoundFlags() | kSoundCones);
	
	const DirectedSourceObject *object = GetObject();
	float f = object->GetApexTangent();
	
	sound->SetSoundProperty(kSoundOuterConeCosine, f * InverseSqrt(f * f + 1.0F));
	sound->SetSoundProperty(kSoundOuterConeVolume, object->GetOuterConeVolume());
	sound->SetSoundProperty(kSoundOuterConeHFVolume, object->GetOuterConeHFVolume());
}

// ZYURVUR
