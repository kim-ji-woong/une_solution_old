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
#include "C4Paint.h"
#include "C4Configuration.h"


using namespace C4;


SpaceObject::SpaceObject(SpaceType type, Volume *volume) :
		Object(kObjectSpace),
		VolumeObject(volume)
{
	spaceType = type;
}

SpaceObject::~SpaceObject()
{
}

SpaceObject *SpaceObject::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kSpaceFog:
			
			return (new FogSpaceObject);
		
		case kSpaceShadow:
			
			return (new ShadowSpaceObject);
		
		case kSpaceAmbient:
			
			return (new AmbientSpaceObject);
		
		case kSpaceAcoustics:
			
			return (new AcousticsSpaceObject);
		
		case kSpaceOcclusion:
			
			return (new OcclusionSpaceObject);
		
		case kSpacePaint:
			
			return (new PaintSpaceObject);
	}
	
	return (nullptr);
}

void SpaceObject::PackType(Packer& data) const
{
	Object::PackType(data);
	data << spaceType;
}

void SpaceObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	PackVolume(data, packFlags);
}

void SpaceObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackVolume(data, unpackFlags);
}

int32 SpaceObject::GetObjectSize(float *size) const
{
	return (GetVolumeObjectSize(size));
}

void SpaceObject::SetObjectSize(const float *size)
{
	SetVolumeObjectSize(size);
}


FogSpaceObject::FogSpaceObject() : SpaceObject(kSpaceFog, this)
{
	fogFlags = kFogOcclusionInhibit;
	fogFunction = kFogFunctionConstant;
	perspectiveExclusionMask = 0;
}

FogSpaceObject::FogSpaceObject(const Vector2D& size) :
		SpaceObject(kSpaceFog, this),
		PlateVolume(size)
{
	fogFlags = kFogOcclusionInhibit;
	fogColor.Set(1.0F, 1.0F, 1.0F, 1.0F);
	fogDensity = 0.05F;
	fogFunction = kFogFunctionConstant;
	perspectiveExclusionMask = 0;
}

FogSpaceObject::~FogSpaceObject()
{
}

void FogSpaceObject::Pack(Packer& data, unsigned_int32 packFlags) const
{ 
	SpaceObject::Pack(data, packFlags);
	 
	data << ChunkHeader('FLAG', 4); 
	data << fogFlags; 
	
	data << ChunkHeader('PARM', sizeof(ColorRGBA) + 4); 
	data << fogColor;
	data << fogDensity;
	
	data << ChunkHeader('FUNC', 4); 
	data << fogFunction;
	
	if (perspectiveExclusionMask != 0)
	{ 
		data << ChunkHeader('EXCL', 4);
		data << perspectiveExclusionMask;
	}
	
	data << TerminatorChunk;
}

void FogSpaceObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	SpaceObject::Unpack(data, unpackFlags);
	UnpackChunkList<FogSpaceObject>(data, unpackFlags);
}

bool FogSpaceObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'FLAG':
			
			data >> fogFlags;
			return (true);
		
		case 'PARM':
			
			data >> fogColor;
			data >> fogDensity;
			return (true);
		
		case 'FUNC':
			
			data >> fogFunction;
			return (true);
		
		case 'EXCL':
			
			data >> perspectiveExclusionMask;
			return (true);
	}
	
	return (false);
}

void *FogSpaceObject::BeginSettingsUnpack(void)
{
	perspectiveExclusionMask = 0;
	return (SpaceObject::BeginSettingsUnpack());
}

int32 FogSpaceObject::GetCategoryCount(void) const
{
	return (1);
}

Type FogSpaceObject::GetCategoryType(int32 index, const char **title) const
{
	if (index == 0)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kSpaceFog));
		return (kSpaceFog);
	}
	
	return (0);
}

int32 FogSpaceObject::GetCategorySettingCount(Type category) const
{
	if (category == kSpaceFog) return (11);
	return (0);
}

Setting *FogSpaceObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kSpaceFog)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID(kSpaceFog, 'FOG '));
			return (new HeadingSetting(kSpaceFog, title));
		}
		
		if (index == 1)
		{
			const char *title = table->GetString(StringID(kSpaceFog, 'FOG ', 'COLR'));
			const char *picker = table->GetString(StringID(kSpaceFog, 'FOG ', 'PICK'));
			return (new ColorSetting('COLR', fogColor, title, picker));
		}
		
		if (index == 2)
		{
			const char *title = table->GetString(StringID(kSpaceFog, 'FOG ', 'DENS'));
			return (new TextSetting('DENS', fogDensity, title));
		}
		
		if (index == 3)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kSpaceFog, 'FOG ', 'FUNC'));
			MenuSetting *menu = new MenuSetting('FUNC', (fogFunction != kFogFunctionConstant), title, 2);
			
			menu->SetMenuItemString(0, table->GetString(StringID(kSpaceFog, 'FOG ', 'FUNC', kFogFunctionConstant)));
			menu->SetMenuItemString(1, table->GetString(StringID(kSpaceFog, 'FOG ', 'FUNC', kFogFunctionLinear)));
			
			return (menu);
		}
		
		if (index == 4)
		{
			const char *title = table->GetString(StringID(kSpaceFog, 'FOG ', 'OCCL'));
			return (new BooleanSetting('OCCL', ((fogFlags & kFogOcclusionInhibit) == 0), title));
		}
		
		if (index == 5)
		{
			const char *title = table->GetString(StringID(kSpaceFog, 'EXCL'));
			return (new HeadingSetting('EXCL', title));
		}
		
		if (index == 6)
		{
			const char *title = table->GetString(StringID(kSpaceFog, 'EXCL', 'DRCT'));
			return (new BooleanSetting('DRCT', ((perspectiveExclusionMask & kPerspectiveDirect) != 0), title));
		}
		
		if (index == 7)
		{
			const char *title = table->GetString(StringID(kSpaceFog, 'EXCL', 'REFL'));
			return (new BooleanSetting('REFL', ((perspectiveExclusionMask & kPerspectiveReflection) != 0), title));
		}
		
		if (index == 8)
		{
			const char *title = table->GetString(StringID(kSpaceFog, 'EXCL', 'REFR'));
			return (new BooleanSetting('REFR', ((perspectiveExclusionMask & kPerspectiveRefraction) != 0), title));
		}
		
		if (index == 9)
		{
			const char *title = table->GetString(StringID(kSpaceFog, 'EXCL', 'CAMR'));
			return (new BooleanSetting('CAMR', ((perspectiveExclusionMask & kPerspectiveCameraWidget) != 0), title));
		}
		
		if (index == 10)
		{
			const char *title = table->GetString(StringID(kSpaceFog, 'EXCL', 'RPRT'));
			return (new BooleanSetting('RPRT', ((perspectiveExclusionMask & kPerspectiveRemotePortal) != 0), title));
		}
	}
	
	return (nullptr);
}

void FogSpaceObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kSpaceFog)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'COLR')
		{
			fogColor = static_cast<const ColorSetting *>(setting)->GetColor();
		}
		else if (identifier == 'DENS')
		{
			fogDensity = Fmax(Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText()), 1.0e-10F);
		}
		else if (identifier == 'FUNC')
		{
			fogFunction = (static_cast<const MenuSetting *>(setting)->GetMenuSelection() == 0) ? kFogFunctionConstant : kFogFunctionLinear;
		}
		else if (identifier == 'OCCL')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) fogFlags &= ~kFogOcclusionInhibit;
			else fogFlags |= kFogOcclusionInhibit;
		}
		else if (identifier == 'DRCT')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) perspectiveExclusionMask |= kPerspectiveDirect;
			else perspectiveExclusionMask &= ~kPerspectiveDirect;
		}
		else if (identifier == 'REFL')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) perspectiveExclusionMask |= kPerspectiveReflection;
			else perspectiveExclusionMask &= ~kPerspectiveReflection;
		}
		else if (identifier == 'REFR')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) perspectiveExclusionMask |= kPerspectiveRefraction;
			else perspectiveExclusionMask &= ~kPerspectiveRefraction;
		}
		else if (identifier == 'CAMR')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) perspectiveExclusionMask |= kPerspectiveCameraWidget;
			else perspectiveExclusionMask &= ~kPerspectiveCameraWidget;
		}
		else if (identifier == 'RPRT')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) perspectiveExclusionMask |= kPerspectiveRemotePortal;
			else perspectiveExclusionMask &= ~kPerspectiveRemotePortal;
		}
	}
}

float FogSpaceObject::GetOcclusionValue(void) const
{
	// This function returns the minimum distance at which an object would be completely fogged
	// in a fog space using a constant density function. The distance d solves the equation
	//
	//		f = Exp(-rho * d),
	//
	// where rho is the density and f is the fraction of light that makes it through to the camera.
	// When f = 1/256, it represents the minimum nonzero value that a color channel can attain.
	// We let f increase to 1/128 for bright fog colors because two bits of precision is virtually
	// impossible to see when it's washed out by the fog.

	return (Log(256.0F - Fmax(fogColor.red, fogColor.green, fogColor.blue) * 128.0F) / fogDensity);
}


ShadowSpaceObject::ShadowSpaceObject() : SpaceObject(kSpaceShadow, this)
{
}

ShadowSpaceObject::ShadowSpaceObject(const Vector3D& size) :
		SpaceObject(kSpaceShadow, this),
		BoxVolume(size)
{
}

ShadowSpaceObject::~ShadowSpaceObject()
{
}


AmbientSpaceObject::AmbientSpaceObject() : SpaceObject(kSpaceAmbient, this)
{
	ambientSpaceFlags = kAmbientSpaceGenerator;
	samplingRadius = 5.0F;
	occlusionExponent = 1.0F;
	minAmbientValue = 0.0F;
	
	ambientMap[0] = nullptr;
	ambientMap[1] = nullptr;
}

AmbientSpaceObject::AmbientSpaceObject(const Vector3D& size, int32 x, int32 y, int32 z, const char *name) :
		SpaceObject(kSpaceAmbient, this),
		BoxVolume(size)
{
	ambientSpaceFlags = kAmbientSpaceGenerator;
	samplingRadius = 5.0F;
	occlusionExponent = 1.0F;
	minAmbientValue = 0.0F;
	
	textureSize[0] = x;
	textureSize[1] = y;
	textureSize[2] = z;
	
	ambientMap[0] = nullptr;
	ambientMap[1] = nullptr;
	SetAmbientMap(name);
}

AmbientSpaceObject::~AmbientSpaceObject()
{
	if (ambientMap[1]) ambientMap[1]->Release();
	if (ambientMap[0]) ambientMap[0]->Release();
}

void AmbientSpaceObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	SpaceObject::Pack(data, packFlags);
	
	PackHandle handle = data.BeginChunk('AMBT');
	data << ambientName;
	data.EndChunk(handle);
	
	data << ChunkHeader('FLAG', 4);
	data << ambientSpaceFlags;
	
	data << ChunkHeader('SRAD', 4);
	data << samplingRadius;
	
	data << ChunkHeader('OEXP', 4);
	data << occlusionExponent;
	
	data << ChunkHeader('MINV', 4);
	data << minAmbientValue;
	
	data << ChunkHeader('TXTR', 12);
	data << textureSize[0];
	data << textureSize[1];
	data << textureSize[2];
	
	data << TerminatorChunk;
}

void AmbientSpaceObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	SpaceObject::Unpack(data, unpackFlags);
	UnpackChunkList<AmbientSpaceObject>(data, unpackFlags);
}

bool AmbientSpaceObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'AMBT':
			
			data >> ambientName;
			SetAmbientMap(ambientName);
			return (true);
		
		case 'FLAG':
			
			data >> ambientSpaceFlags;
			return (true);
		
		case 'SRAD':
			
			data >> samplingRadius;
			return (true);
		
		case 'OEXP':
			
			data >> occlusionExponent;
			return (true);
		
		case 'MINV':
			
			data >> minAmbientValue;
			return (true);
		
		case 'TXTR':
			
			data >> textureSize[0];
			data >> textureSize[1];
			data >> textureSize[2];
			return (true);
	}
	
	return (false);
}

void *AmbientSpaceObject::BeginSettingsUnpack(void)
{
	if (ambientMap[1])
	{
		ambientMap[1]->Release();
		ambientMap[1] = nullptr;
	}
	
	if (ambientMap[0])
	{
		ambientMap[0]->Release();
		ambientMap[0] = nullptr;
	}
	
	return (SpaceObject::BeginSettingsUnpack());
}

int32 AmbientSpaceObject::GetCategoryCount(void) const
{
	return (1);
}

Type AmbientSpaceObject::GetCategoryType(int32 index, const char **title) const
{
	if (index == 0)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kSpaceAmbient));
		return (kSpaceAmbient);
	}
	
	return (0);
}

int32 AmbientSpaceObject::GetCategorySettingCount(Type category) const
{
	if (category == kSpaceAmbient) return (9);
	return (0);
}

Setting *AmbientSpaceObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kSpaceAmbient)
	{
		if (flags & kConfigurationScript) return (nullptr);
		
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID(kSpaceAmbient, 'AMBT'));
			return (new HeadingSetting(kSpaceAmbient, title));
		}
		
		if (index == 1)
		{
			const char *title = table->GetString(StringID(kSpaceAmbient, 'AMBT', 'GENR'));
			return (new BooleanSetting('GENR', ((ambientSpaceFlags & kAmbientSpaceGenerator) != 0), title));
		}
		
		if (index == 2)
		{
			const char *title = table->GetString(StringID(kSpaceAmbient, 'AMBT', 'SRAD'));
			return (new TextSetting('SRAD', samplingRadius, title));
		}
		
		if (index == 3)
		{
			const char *title = table->GetString(StringID(kSpaceAmbient, 'AMBT', 'OEXP'));
			return (new FloatSetting('OEXP', occlusionExponent, title, 1.0F, 4.0F, 0.1F));
		}
		
		if (index == 4)
		{
			const char *title = table->GetString(StringID(kSpaceAmbient, 'AMBT', 'MINV'));
			return (new FloatSetting('MINV', minAmbientValue, title, 0.0F, 1.0F, 0.01F));
		}
		
		if (index == 5)
		{
			const char *title = table->GetString(StringID(kSpaceAmbient, 'AMBT', 'WIDE'));
			return (new PowerTwoSetting('WIDE', textureSize[0], title, 2, kMaxAmbientSpaceSize));
		}
		
		if (index == 6)
		{
			const char *title = table->GetString(StringID(kSpaceAmbient, 'AMBT', 'HIGH'));
			return (new PowerTwoSetting('HIGH', textureSize[1], title, 2, kMaxAmbientSpaceSize));
		}
		
		if (index == 7)
		{
			const char *title = table->GetString(StringID(kSpaceAmbient, 'AMBT', 'DEEP'));
			return (new PowerTwoSetting('DEEP', textureSize[2], title, 2, kMaxAmbientSpaceSize));
		}
		
		if (index == 8)
		{
			const char *title = table->GetString(StringID(kSpaceAmbient, 'AMBT', 'TNAM'));
			const char *picker = table->GetString(StringID(kSpaceAmbient, 'AMBT', 'TPCK'));
			return (new ResourceSetting('TNAM', ambientName, title, picker, TextureResource::GetDescriptor()));
		}
	}
	
	return (nullptr);
}

void AmbientSpaceObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kSpaceAmbient)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'GENR')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) ambientSpaceFlags |= kAmbientSpaceGenerator;
			else ambientSpaceFlags &= ~kAmbientSpaceGenerator;
		}
		else if (identifier == 'SRAD')
		{
			samplingRadius = Fmax(Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText()), 0.5F);
		}
		else if (identifier == 'OEXP')
		{
			occlusionExponent = static_cast<const FloatSetting *>(setting)->GetFloatValue();
		}
		else if (identifier == 'MINV')
		{
			minAmbientValue = static_cast<const FloatSetting *>(setting)->GetFloatValue();
		}
		else if (identifier == 'WIDE')
		{
			textureSize[0] = static_cast<const PowerTwoSetting *>(setting)->GetIntegerValue();
		}
		else if (identifier == 'HIGH')
		{
			textureSize[1] = static_cast<const PowerTwoSetting *>(setting)->GetIntegerValue();
		}
		else if (identifier == 'DEEP')
		{
			textureSize[2] = static_cast<const PowerTwoSetting *>(setting)->GetIntegerValue();
		}
		else if (identifier == 'TNAM')
		{
			ambientName = static_cast<const ResourceSetting *>(setting)->GetResourceName();
		}
	}
}

void AmbientSpaceObject::SetAmbientMap(const char *name)
{
	Texture *texture1 = ambientMap[0];
	Texture *texture2 = ambientMap[1];
	
	ambientName = name;
	for (machine a = 0; a < 2; a++) ambientMap[a] = Texture::Get(name, a);
	
	if (texture2) texture2->Release();
	if (texture1) texture1->Release();
}


AcousticsSpaceObject::AcousticsSpaceObject() : SpaceObject(kSpaceAcoustics, this)
{
}

AcousticsSpaceObject::AcousticsSpaceObject(const Vector3D& size) :
		SpaceObject(kSpaceAcoustics, this),
		BoxVolume(size)
{
	reflectionVolume = 1.0F;
	reflectionHFVolume = 0.5F;
	reverbDecayTime = 500.0F;
	mediumHFAbsorption = 1.0F;
}

AcousticsSpaceObject::~AcousticsSpaceObject()
{
}

void AcousticsSpaceObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	SpaceObject::Pack(data, packFlags);
	
	data << ChunkHeader('REFL', 8);
	data << reflectionVolume;
	data << reflectionHFVolume;
	
	data << ChunkHeader('RVRB', 4);
	data << reverbDecayTime;
	
	data << ChunkHeader('ABSP', 4);
	data << mediumHFAbsorption;
	
	data << TerminatorChunk;
}

void AcousticsSpaceObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	SpaceObject::Unpack(data, unpackFlags);
	UnpackChunkList<AcousticsSpaceObject>(data, unpackFlags);
}

bool AcousticsSpaceObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'REFL':
			
			data >> reflectionVolume;
			data >> reflectionHFVolume;
			return (true);
		
		case 'RVRB':
			
			data >> reverbDecayTime;
			return (true);
		
		case 'ABSP':
			
			data >> mediumHFAbsorption;
			return (true);
	}
	
	return (false);
}

int32 AcousticsSpaceObject::GetCategoryCount(void) const
{
	return (1);
}

Type AcousticsSpaceObject::GetCategoryType(int32 index, const char **title) const
{
	if (index == 0)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kSpaceAcoustics));
		return (kSpaceAcoustics);
	}
	
	return (0);
}

int32 AcousticsSpaceObject::GetCategorySettingCount(Type category) const
{
	if (category == kSpaceAcoustics) return (5);
	return (0);
}

Setting *AcousticsSpaceObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kSpaceAcoustics)
	{
		if (flags & kConfigurationScript) return (nullptr);
		
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID(kSpaceAcoustics, 'ACST'));
			return (new HeadingSetting(kSpaceAcoustics, title));
		}
		
		if (index == 1)
		{
			const char *title = table->GetString(StringID(kSpaceAcoustics, 'ACST', 'REFV'));
			return (new IntegerSetting('REFV', (int32) (reflectionVolume * 100.0 + 0.5F), title, 0, 200, 1));
		}
		
		if (index == 2)
		{
			const char *title = table->GetString(StringID(kSpaceAcoustics, 'ACST', 'REFH'));
			return (new IntegerSetting('REFH', (int32) (reflectionHFVolume * 100.0 + 0.5F), title, 0, 100, 1));
		}
		
		if (index == 3)
		{
			const char *title = table->GetString(StringID(kSpaceAcoustics, 'ACST', 'RVBT'));
			return (new FloatSetting('RVBT', reverbDecayTime * 0.001F, title, 0.0F, kMaxReverbDecayTime * 0.001F, 0.01F));
		}
		
		if (index == 4)
		{
			const char *title = table->GetString(StringID(kSpaceAcoustics, 'ACST', 'ABSP'));
			return (new IntegerSetting('ABSP', (int32) ((1.0F - mediumHFAbsorption) * 1000.0F + 0.5F), title, 0, 100, 1));
		}
	}
	
	return (nullptr);
}

void AcousticsSpaceObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kSpaceAcoustics)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'REFV')
		{
			reflectionVolume = (float) static_cast<const IntegerSetting *>(setting)->GetIntegerValue() * 0.01F;
		}
		else if (identifier == 'REFH')
		{
			reflectionHFVolume = (float) static_cast<const IntegerSetting *>(setting)->GetIntegerValue() * 0.01F;
		}
		else if (identifier == 'RVBT')
		{
			reverbDecayTime = static_cast<const FloatSetting *>(setting)->GetFloatValue() * 1000.0F;
		}
		else if (identifier == 'ABSP')
		{
			mediumHFAbsorption = 1.0F - (float) static_cast<const IntegerSetting *>(setting)->GetIntegerValue() * 0.001F;
		}
	}
}


OcclusionSpaceObject::OcclusionSpaceObject() : SpaceObject(kSpaceOcclusion, this)
{
}

OcclusionSpaceObject::OcclusionSpaceObject(const Vector3D& size) :
		SpaceObject(kSpaceOcclusion, this),
		BoxVolume(size)
{
}

OcclusionSpaceObject::~OcclusionSpaceObject()
{
}


PaintSpaceObject::PaintSpaceObject() : SpaceObject(kSpacePaint, this)
{
	paintImage = nullptr;
	paintTexture = nullptr;
	
	preprocessCount = 0;
}

PaintSpaceObject::PaintSpaceObject(const Vector3D& size, const Integer2D& resolution, int32 count) :
		SpaceObject(kSpacePaint, this),
		BoxVolume(size)
{
	imageDesc.paintResolution = resolution;
	imageDesc.channelCount = count;
	
	paintImage = nullptr;
	paintTexture = nullptr;
	
	preprocessCount = 0;
}

PaintSpaceObject::~PaintSpaceObject()
{
	if (paintTexture) paintTexture->Release();
	delete[] paintImage;
}

void PaintSpaceObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	SpaceObject::Pack(data, packFlags);
	
	data << ChunkHeader('RESO', sizeof(Integer2D));
	data << imageDesc.paintResolution;
	
	data << ChunkHeader('CHAN', 4);
	data << imageDesc.channelCount;
	
	if (paintImage)
	{
		unsigned_int32 dataSize = imageDesc.paintResolution.x * imageDesc.paintResolution.y * imageDesc.channelCount;
		unsigned_int8 *compressedData = new unsigned_int8[dataSize];
		
		unsigned_int32 compressedSize = Comp::CompressData(paintImage, dataSize, compressedData);
		if (compressedSize != 0)
		{
			data << ChunkHeader('ICMP', 4 + compressedSize);
			
			data << compressedSize;
			data.WriteData(compressedData, compressedSize);
		}
		else
		{
			data << ChunkHeader('IRAW', dataSize);
			data.WriteData(paintImage, dataSize);
		}
		
		delete[] compressedData;
	}
	
	data << TerminatorChunk;
}

void PaintSpaceObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	SpaceObject::Unpack(data, unpackFlags);
	UnpackChunkList<PaintSpaceObject>(data, unpackFlags);
}

bool PaintSpaceObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'RESO':
			
			data >> imageDesc.paintResolution;
			return (true);
		
		case 'CHAN':
			
			data >> imageDesc.channelCount;
			return (true);
		
		case 'ICMP':
		{
			unsigned_int32	compressedSize;
			
			data >> compressedSize;
			unsigned_int8 *compressedData = new unsigned_int8[compressedSize];
			data.ReadData(compressedData, compressedSize);
			
			paintImage = new unsigned_int8[imageDesc.paintResolution.x * imageDesc.paintResolution.y * imageDesc.channelCount];
			Comp::DecompressData(compressedData, compressedSize, paintImage);
			
			delete[] compressedData;
			return (true);
		}
		
		case 'IRAW':
		{
			unsigned_int32 dataSize = imageDesc.paintResolution.x * imageDesc.paintResolution.y * imageDesc.channelCount;
			paintImage = new unsigned_int8[dataSize];
			data.ReadData(paintImage, dataSize);
			return (true);
		}
	}
	
	return (false);
}

void *PaintSpaceObject::BeginSettingsUnpack(void)
{
	delete[] paintImage;
	paintImage = nullptr;
	
	if (paintTexture)
	{
		paintTexture->Release();
		paintTexture = nullptr;
	}
	
	return (SpaceObject::BeginSettingsUnpack());
}

int32 PaintSpaceObject::GetCategoryCount(void) const
{
	return (1);
}

Type PaintSpaceObject::GetCategoryType(int32 index, const char **title) const
{
	if (index == 0)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kSpacePaint));
		return (kSpacePaint);
	}
	
	return (0);
}

int32 PaintSpaceObject::GetCategorySettingCount(Type category) const
{
	if (category == kSpacePaint) return (4);
	return (0);
}

Setting *PaintSpaceObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kSpacePaint)
	{
		if (flags & kConfigurationScript) return (nullptr);
		
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID(kSpacePaint, 'PANT'));
			return (new HeadingSetting(kSpacePaint, title));
		}
		
		if (index == 1)
		{
			const char *title = table->GetString(StringID(kSpacePaint, 'PANT', 'XRES'));
			return (new PowerTwoSetting('XRES', imageDesc.paintResolution.x, title, kPaintMinResolution, kPaintMaxResolution));
		}
		
		if (index == 2)
		{
			const char *title = table->GetString(StringID(kSpacePaint, 'PANT', 'YRES'));
			return (new PowerTwoSetting('YRES', imageDesc.paintResolution.y, title, kPaintMinResolution, kPaintMaxResolution));
		}
		
		if (index == 3)
		{
			const char *title = table->GetString(StringID(kSpacePaint, 'PANT', 'CHAN'));
			return (new PowerTwoSetting('CHAN', imageDesc.channelCount, title, 1, 4));
		}
	}
	
	return (nullptr);
}

void PaintSpaceObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kSpacePaint)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'XRES')
		{
			imageDesc.paintResolution.x = static_cast<const PowerTwoSetting *>(setting)->GetIntegerValue();
		}
		else if (identifier == 'YRES')
		{
			imageDesc.paintResolution.y = static_cast<const PowerTwoSetting *>(setting)->GetIntegerValue();
		}
		else if (identifier == 'CHAN')
		{
			imageDesc.channelCount = static_cast<const PowerTwoSetting *>(setting)->GetIntegerValue();
		}
	}
}

void *PaintSpaceObject::BeginSettings(void)
{
	PaintImageDesc *desc = new PaintImageDesc;
	desc->paintResolution = imageDesc.paintResolution;
	desc->channelCount = imageDesc.channelCount;
	return (desc);
}

void PaintSpaceObject::EndSettings(void *cookie)
{
	PaintImageDesc *desc = static_cast<PaintImageDesc *>(cookie);
	
	if ((imageDesc.paintResolution != desc->paintResolution) || (imageDesc.channelCount != desc->channelCount))
	{
		if (paintImage)
		{
			delete[] paintImage;
			
			int32 pixelCount = imageDesc.paintResolution.x * imageDesc.paintResolution.y;
			paintImage = new unsigned_int8[pixelCount * imageDesc.channelCount];
			MemoryMgr::ClearMemory(paintImage, pixelCount * imageDesc.channelCount);
			
			if (paintTexture)
			{
				paintTexture->Release();
				CreatePaintTexture();
			}
		}
	}
	
	delete desc;
}

void PaintSpaceObject::CreatePaintTexture(void)
{
	textureHeader.textureType = kTexture2D;
	textureHeader.textureFlags = 0;
	textureHeader.colorSemantic = kTextureSemanticData;
	textureHeader.alphaSemantic = kTextureSemanticData;
	
	if (imageDesc.channelCount == 4) textureHeader.imageFormat = kTextureRGBA8;
	else if (imageDesc.channelCount == 2) textureHeader.imageFormat = kTextureLA8;
	else textureHeader.imageFormat = kTextureI8;
	
	textureHeader.imageWidth = imageDesc.paintResolution.x;
	textureHeader.imageHeight = imageDesc.paintResolution.y;
	textureHeader.imageDepth = 1;
	textureHeader.wrapMode[0] = kTextureClamp;
	textureHeader.wrapMode[1] = kTextureClamp;
	textureHeader.wrapMode[2] = kTextureClamp;
	textureHeader.mipmapCount = 1;
	textureHeader.mipmapDataOffset = 0;
	textureHeader.auxiliaryDataSize = 0;
	textureHeader.auxiliaryDataOffset = 0;
	
	paintTexture = Texture::Get(&textureHeader, paintImage);
}

void PaintSpaceObject::Preprocess(void)
{
	preprocessCount++;
	
	if (!paintTexture)
	{
		if (!paintImage)
		{
			int32 pixelCount = imageDesc.paintResolution.x * imageDesc.paintResolution.y;
			paintImage = new unsigned_int8[pixelCount * imageDesc.channelCount];
			MemoryMgr::ClearMemory(paintImage, pixelCount * imageDesc.channelCount);
		}
		
		CreatePaintTexture();
	}
}

void PaintSpaceObject::Neutralize(void)
{
	if (--preprocessCount == 0)
	{
		if (paintTexture)
		{
			paintTexture->Release();
			paintTexture = nullptr;
		}
	}
}

// ZYURVUR
