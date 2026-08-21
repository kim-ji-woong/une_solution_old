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


#include "C4LightObjects.h"
#include "C4Configuration.h"


using namespace C4;


LightObject::LightObject(LightType type, LightType base) : Object(kObjectLight)
{
	lightType = type;
	baseLightType = base;
	
	minDetailLevel = 0;
}

LightObject::LightObject(LightType type, LightType base, const ColorRGB& color) : Object(kObjectLight)
{
	lightType = type;
	baseLightType = base;
	
	lightColor = color;
	lightFlags = 0;
	minDetailLevel = 0;
}

LightObject::LightObject(const LightObject& lightObject) : Object(kObjectLight)
{
	lightType = lightObject.lightType;
	baseLightType = lightObject.baseLightType;
	
	lightColor = lightObject.lightColor;
	lightFlags = lightObject.lightFlags;
	minDetailLevel = lightObject.minDetailLevel;
}

LightObject::~LightObject()
{
}

LightObject *LightObject::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kLightInfinite:
			
			return (new InfiniteLightObject);
		
		case kLightDepth:
			
			return (new DepthLightObject);
		
		case kLightLandscape:
			
			return (new LandscapeLightObject);
		
		case kLightPoint:
			
			return (new PointLightObject);
		
		case kLightCube:
			
			return (new CubeLightObject);
		
		case kLightSpot:
			
			return (new SpotLightObject);
	}
	
	return (nullptr);
}

void LightObject::PackType(Packer& data) const
{
	Object::PackType(data);
	data << lightType;
}

void LightObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	data << ChunkHeader('COLR', sizeof(ColorRGB));
	data << lightColor;
	
	data << ChunkHeader('FLAG', 4);
	data << lightFlags;
	
	data << ChunkHeader('MDET', 4);
	data << minDetailLevel;
	
	data << TerminatorChunk;
}

void LightObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<LightObject>(data, unpackFlags);
}

bool LightObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'COLR':
		 
		#if C4LEGACY
		 
			case 'DIFF': 
		 
		#endif
			 
			data >> lightColor;
			return (true);
		
		case 'FLAG': 
			
			data >> lightFlags;
			return (true);
		 
		case 'MDET':
			
			data >> minDetailLevel;
			return (true);
	}
	
	return (false);
}

int32 LightObject::GetCategoryCount(void) const
{
	return (1);
}

Type LightObject::GetCategoryType(int32 index, const char **title) const
{
	if (index == 0)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kObjectLight));
		return (kObjectLight);
	}
	
	return (0);
}

int32 LightObject::GetCategorySettingCount(Type category) const
{
	if (category == kObjectLight) return (9);
	return (0);
}

Setting *LightObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kObjectLight)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID(kObjectLight, 'ILMN'));
			return (new HeadingSetting('ILMN', title));
		}
		
		if (index == 1)
		{
			const char *title = table->GetString(StringID(kObjectLight, 'ILMN', 'COLR'));
			const char *picker = table->GetString(StringID(kObjectLight, 'ILMN', 'PICK'));
			
			ColorRGB color = lightColor;
			float bright = Fmax(color.red, color.green, color.blue);
			if (bright > 1.0F) color /= bright;
			
			return (new ColorSetting('COLR', color, title, picker));
		}
		
		if (index == 2)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectLight, 'ILMN', 'SCAL'));
			float bright = Fmax(lightColor.red, lightColor.green, lightColor.blue);
			return (new TextSetting('SCAL', Fmax(bright, 1.0F), title));
		}
		
		if (index == 3)
		{
			const char *title = table->GetString(StringID(kObjectLight, 'FLAG'));
			return (new HeadingSetting('FLAG', title));
		}
		
		if (index == 4)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectLight, 'FLAG', 'STAT'));
			return (new BooleanSetting('STAT', ((lightFlags & kLightStatic) != 0), title));
		}
		
		if (index == 5)
		{
			const char *title = table->GetString(StringID(kObjectLight, 'FLAG', 'NSHD'));
			return (new BooleanSetting('NSHD', ((lightFlags & kLightShadowInhibit) != 0), title));
		}
		
		if (index == 6)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectLight, 'FLAG', 'EXTN'));
			return (new BooleanSetting('EXTN', ((lightFlags & kLightExternalZone) != 0), title));
		}
		
		if (index == 7)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectLight, 'FLAG', 'ISPC'));
			return (new BooleanSetting('ISPC', ((lightFlags & kLightInstanceShadowSpace) != 0), title));
		}
		
		if (index == 8)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectLight, 'FLAG', 'MDET'));
			return (new IntegerSetting('MDET', minDetailLevel, title, 0, 2, 1));
		}
	}
	
	return (nullptr);
}

void LightObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kObjectLight)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'COLR')
		{
			lightColor = static_cast<const ColorSetting *>(setting)->GetColor();
		}
		else if (identifier == 'SCAL')
		{
			lightColor *= Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
		}
		else if (identifier == 'STAT')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			unsigned_int32 flags = lightFlags & ~kLightConfined;
			if (b) lightFlags = flags | kLightStatic;
			else lightFlags = flags & ~kLightStatic;
		}
		else if (identifier == 'NSHD')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) lightFlags |= kLightShadowInhibit;
			else lightFlags &= ~kLightShadowInhibit;
		}
		else if (identifier == 'EXTN')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) lightFlags |= kLightExternalZone;
			else lightFlags &= ~kLightExternalZone;
		}
		else if (identifier == 'ISPC')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) lightFlags |= kLightInstanceShadowSpace;
			else lightFlags &= ~kLightInstanceShadowSpace;
		}
		else if (identifier == 'MDET')
		{
			minDetailLevel = static_cast<const IntegerSetting *>(setting)->GetIntegerValue();
		}
	}
}


InfiniteLightObject::InfiniteLightObject() : LightObject(kLightInfinite, kLightInfinite)
{
}

InfiniteLightObject::InfiniteLightObject(LightType type) : LightObject(type, kLightInfinite)
{
}

InfiniteLightObject::InfiniteLightObject(LightType type, const ColorRGB& color) : LightObject(type, kLightInfinite, color)
{
}

InfiniteLightObject::InfiniteLightObject(const ColorRGB& color) : LightObject(kLightInfinite, kLightInfinite, color)
{
}

InfiniteLightObject::InfiniteLightObject(const InfiniteLightObject& infiniteLightObject) : LightObject(infiniteLightObject)
{
}

InfiniteLightObject::~InfiniteLightObject()
{
}

Object *InfiniteLightObject::Replicate(void) const
{
	return (new InfiniteLightObject(*this));
}


DepthLightObject::DepthLightObject() : InfiniteLightObject(kLightDepth)
{
}

DepthLightObject::DepthLightObject(LightType type) : InfiniteLightObject(type)
{
}

DepthLightObject::DepthLightObject(LightType type, const ColorRGB& color) : InfiniteLightObject(type, color)
{
}

DepthLightObject::DepthLightObject(const ColorRGB& color) : InfiniteLightObject(kLightDepth, color)
{
}

DepthLightObject::DepthLightObject(const DepthLightObject& depthLightObject) : InfiniteLightObject(depthLightObject)
{
}

DepthLightObject::~DepthLightObject()
{
}

Object *DepthLightObject::Replicate(void) const
{
	return (new DepthLightObject(*this));
}


LandscapeLightObject::LandscapeLightObject() : DepthLightObject(kLightLandscape)
{
	sectionRange[0].Set(0.0F, 100.0F);
	sectionRange[1].Set(60.0F, 200.0F);
	sectionRange[2].Set(160.0F, 350.0F);
	sectionRange[3].Set(310.0F, 600.0F);
}

LandscapeLightObject::LandscapeLightObject(const ColorRGB& color) : DepthLightObject(kLightLandscape, color)
{
	sectionRange[0].Set(0.0F, 100.0F);
	sectionRange[1].Set(60.0F, 200.0F);
	sectionRange[2].Set(160.0F, 350.0F);
	sectionRange[3].Set(310.0F, 600.0F);
}

LandscapeLightObject::LandscapeLightObject(const LandscapeLightObject& landscapeLightObject) : DepthLightObject(landscapeLightObject)
{
	for (machine a = 0; a < kMaxShadowSectionCount; a++) sectionRange[a] = landscapeLightObject.sectionRange[a];
}

LandscapeLightObject::~LandscapeLightObject()
{
}

Object *LandscapeLightObject::Replicate(void) const
{
	return (new LandscapeLightObject(*this));
}

void LandscapeLightObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	DepthLightObject::Pack(data, packFlags);
	
	for (machine a = 0; a < kMaxShadowSectionCount; a++)
	{
		data << ChunkHeader('SEC1' + a, 8);
		data << sectionRange[a];
	}
	
	data << TerminatorChunk;
}

void LandscapeLightObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	DepthLightObject::Unpack(data, unpackFlags);
	UnpackChunkList<LandscapeLightObject>(data, unpackFlags);
}

bool LandscapeLightObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	unsigned_int32 type = chunkHeader->chunkType;
	unsigned_int32 index = type - 'SEC1';
	if (index < kMaxShadowSectionCount)
	{
		data >> sectionRange[index];
		return (true);
	}
	
	return (false);
}

int32 LandscapeLightObject::GetCategorySettingCount(Type category) const
{
	int32 count = DepthLightObject::GetCategorySettingCount(category);
	if (category == kObjectLight) count += 8;
	return (count);
}

Setting *LandscapeLightObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kObjectLight)
	{
		int32 count = DepthLightObject::GetCategorySettingCount(kObjectLight);
		if (index >= count)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const StringTable *table = TheInterfaceMgr->GetStringTable();
			
			if (index == count)
			{
				const char *title = table->GetString(StringID(kObjectLight, kLightLandscape));
				return (new HeadingSetting(kLightLandscape, title));
			}
			
			if (index == count + 1)
			{
				const char *title = table->GetString(StringID(kObjectLight, kLightLandscape, 'SEC1'));
				return (new TextSetting('SEC1', sectionRange[0].max, title));
			}
			
			if (index == count + 2)
			{
				const char *title = table->GetString(StringID(kObjectLight, kLightLandscape, 'SEC2'));
				return (new TextSetting('SEC2', sectionRange[1].max, title));
			}
			
			if (index == count + 3)
			{
				const char *title = table->GetString(StringID(kObjectLight, kLightLandscape, 'SEC3'));
				return (new TextSetting('SEC3', sectionRange[2].max, title));
			}
			
			if (index == count + 4)
			{
				const char *title = table->GetString(StringID(kObjectLight, kLightLandscape, 'SEC4'));
				return (new TextSetting('SEC4', sectionRange[3].max, title));
			}
			
			if (index == count + 5)
			{
				const char *title = table->GetString(StringID(kObjectLight, kLightLandscape, 'TRN1'));
				return (new TextSetting('TRN1', sectionRange[0].max - sectionRange[1].min, title));
			}
			
			if (index == count + 6)
			{
				const char *title = table->GetString(StringID(kObjectLight, kLightLandscape, 'TRN2'));
				return (new TextSetting('TRN2', sectionRange[1].max - sectionRange[2].min, title));
			}
			
			if (index == count + 7)
			{
				const char *title = table->GetString(StringID(kObjectLight, kLightLandscape, 'TRN3'));
				return (new TextSetting('TRN3', sectionRange[2].max - sectionRange[3].min, title));
			}
			
			return (nullptr);
		}
	}
	
	return (DepthLightObject::GetCategorySetting(category, index, flags));
}

void LandscapeLightObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kObjectLight)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'SEC1')
		{
			float value = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
			sectionRange[0].max = Fmax(value, 2.0F);
		}
		else if (identifier == 'SEC2')
		{
			float value = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
			sectionRange[1].max = Fmax(value, sectionRange[0].max + 2.0F);
		}
		else if (identifier == 'SEC3')
		{
			float value = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
			sectionRange[2].max = Fmax(value, sectionRange[1].max + 2.0F);
		}
		else if (identifier == 'SEC4')
		{
			float value = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
			sectionRange[3].max = Fmax(value, sectionRange[2].max + 2.0F);
		}
		else if (identifier == 'TRN1')
		{
			float value = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
			sectionRange[1].min = Fmax(sectionRange[0].max - Fmax(value, 1.0F), sectionRange[0].min);
		}
		else if (identifier == 'TRN2')
		{
			float value = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
			sectionRange[2].min = Fmax(sectionRange[1].max - Fmax(value, 1.0F), sectionRange[1].min);
		}
		else if (identifier == 'TRN3')
		{
			float value = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
			sectionRange[3].min = Fmax(sectionRange[2].max - Fmax(value, 1.0F), sectionRange[2].min);
		}
		else
		{
			DepthLightObject::SetCategorySetting(kObjectLight, setting);
		}
	}
	else
	{
		DepthLightObject::SetCategorySetting(category, setting);
	}
}


PointLightObject::PointLightObject() : LightObject(kLightPoint, kLightPoint)
{
	confinementRadius = 0.0F;
}

PointLightObject::PointLightObject(LightType type) : LightObject(type, kLightPoint)
{
	confinementRadius = 0.0F;
}

PointLightObject::PointLightObject(LightType type, const ColorRGB& color, float range) : LightObject(type, kLightPoint, color)
{
	lightRange = range;
	confinementRadius = 0.0F;
}

PointLightObject::PointLightObject(const ColorRGB& color, float range) : LightObject(kLightPoint, kLightPoint, color)
{
	lightRange = range;
	confinementRadius = 0.0F;
}

PointLightObject::PointLightObject(const PointLightObject& pointLightObject) : LightObject(pointLightObject)
{
	lightRange = pointLightObject.lightRange;
	confinementRadius = pointLightObject.confinementRadius;
}

PointLightObject::~PointLightObject()
{
}

Object *PointLightObject::Replicate(void) const
{
	return (new PointLightObject(*this));
}

void PointLightObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	LightObject::Pack(data, packFlags);
	
	data << ChunkHeader('RANG', 4);
	data << lightRange;
	
	data << ChunkHeader('CRAD', 4);
	data << confinementRadius;
	
	data << TerminatorChunk;
}

void PointLightObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	LightObject::Unpack(data, unpackFlags);
	UnpackChunkList<PointLightObject>(data, unpackFlags);
}

bool PointLightObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'RANG':
			
			data >> lightRange;
			return (true);
		
		case 'CRAD':
			
			data >> confinementRadius;
			return (true);
	}
	
	return (false);
}

int32 PointLightObject::GetObjectSize(float *size) const
{
	size[0] = lightRange;
	return (1);
}

void PointLightObject::SetObjectSize(const float *size)
{
	lightRange = size[0];
}


ShadowLightObject::ShadowLightObject(LightType type) : PointLightObject(type)
{
	shadowMap = nullptr;
}

ShadowLightObject::ShadowLightObject(LightType type, const ColorRGB& color, float range, const char *name) : PointLightObject(type, color, range)
{
	shadowMap = nullptr;
	
	textureSize = 128;
	textureFormat = kTextureI8;
	SetShadowMap(name);
}

ShadowLightObject::ShadowLightObject(const ShadowLightObject& shadowLightObject) : PointLightObject(shadowLightObject)
{
	shadowMap = nullptr;
	
	textureSize = shadowLightObject.textureSize;
	textureFormat = shadowLightObject.textureFormat;
	SetShadowMap(shadowLightObject.shadowName);
}

ShadowLightObject::~ShadowLightObject()
{
	if (shadowMap) shadowMap->Release();
}

void ShadowLightObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	PointLightObject::Pack(data, packFlags);
	
	PackHandle handle = data.BeginChunk('SHAD');
	data << shadowName;
	data.EndChunk(handle);
	
	data << ChunkHeader('SIZE', 4);
	data << textureSize;
	
	data << ChunkHeader('FORM', 4);
	data << textureFormat;
	
	data << TerminatorChunk;
}

void ShadowLightObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	PointLightObject::Unpack(data, unpackFlags);
	UnpackChunkList<ShadowLightObject>(data, unpackFlags);
}

bool ShadowLightObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'SHAD':
			
			data >> shadowName;
			SetShadowMap(shadowName);
			return (true);
		
		case 'SIZE':
			
			data >> textureSize;
			return (true);
		
		case 'FORM':
			
			data >> textureFormat;
			return (true);
	}
	
	return (false);
}

void *ShadowLightObject::BeginSettingsUnpack(void)
{
	if (shadowMap)
	{
		shadowMap->Release();
		shadowMap = nullptr;
	}
	
	return (PointLightObject::BeginSettingsUnpack());
}

int32 ShadowLightObject::GetCategorySettingCount(Type category) const
{
	int32 count = PointLightObject::GetCategorySettingCount(category);
	if (category == kObjectLight) count += 5;
	return (count);
}

Setting *ShadowLightObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kObjectLight)
	{
		int32 count = PointLightObject::GetCategorySettingCount(kObjectLight);
		if (index >= count)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const StringTable *table = TheInterfaceMgr->GetStringTable();
			
			if (index == count)
			{
				const char *title = table->GetString(StringID(kObjectLight, 'SHAD'));
				return (new HeadingSetting('SHAD', title));
			}
			
			if (index == count + 1)
			{
				const char *title = table->GetString(StringID(kObjectLight, 'SHAD', 'GENR'));
				return (new BooleanSetting('GENR', ((GetLightFlags() & kLightGenerator) != 0), title));
			}
			
			if (index == count + 2)
			{
				const char *title = table->GetString(StringID(kObjectLight, 'SHAD', 'SIZE'));
				return (new PowerTwoSetting('SIZE', textureSize, title, 16, 1024));
			}
			
			if (index == count + 3)
			{
				const char *title = table->GetString(StringID(kObjectLight, 'SHAD', 'FORM'));
				MenuSetting *menu = new MenuSetting('FORM', (textureFormat != kTextureRGBA8), title, 2);
				
				menu->SetMenuItemString(0, table->GetString(StringID(kObjectLight, 'SHAD', 'FORM', kTextureRGBA8)));
				menu->SetMenuItemString(1, table->GetString(StringID(kObjectLight, 'SHAD', 'FORM', kTextureI8)));
				
				return (menu);
			}
			
			if (index == count + 4)
			{
				const char *title = table->GetString(StringID(kObjectLight, 'SHAD', 'TNAM'));
				const char *picker = table->GetString(StringID(kObjectLight, 'SHAD', 'PICK'));
				return (new ResourceSetting('TNAM', shadowName, title, picker, TextureResource::GetDescriptor()));
			}
			
			return (nullptr);
		}
	}
	
	return (PointLightObject::GetCategorySetting(category, index, flags));
}

void ShadowLightObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kObjectLight)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'GENR')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) SetLightFlags(GetLightFlags() | kLightGenerator);
			else SetLightFlags(GetLightFlags() & ~kLightGenerator);
		}
		else if (identifier == 'SIZE')
		{
			textureSize = static_cast<const PowerTwoSetting *>(setting)->GetIntegerValue();
		}
		else if (identifier == 'FORM')
		{
			textureFormat = (static_cast<const MenuSetting *>(setting)->GetMenuSelection() == 0) ? kTextureRGBA8 : kTextureI8;
		}
		else if (identifier == 'TNAM')
		{
			SetShadowMap(static_cast<const ResourceSetting *>(setting)->GetResourceName());
		}
		else
		{
			PointLightObject::SetCategorySetting(kObjectLight, setting);
		}
	}
}

void ShadowLightObject::SetShadowMap(const char *name)
{
	Texture *texture = shadowMap;
	
	shadowName = name;
	shadowMap = Texture::Get(name);
	if (!shadowMap) shadowMap = Texture::Get((GetLightType() == kLightCube) ? "C4/cube" : "C4/spot");
	
	if (texture) texture->Release();
}


CubeLightObject::CubeLightObject() : ShadowLightObject(kLightCube)
{
}

CubeLightObject::CubeLightObject(const ColorRGB& color, float range, const char *name) : ShadowLightObject(kLightCube, color, range, name)
{
}

CubeLightObject::CubeLightObject(const CubeLightObject& cubeLightObject) : ShadowLightObject(cubeLightObject)
{
}

CubeLightObject::~CubeLightObject()
{
}

Object *CubeLightObject::Replicate(void) const
{
	return (new CubeLightObject(*this));
}


SpotLightObject::SpotLightObject() : ShadowLightObject(kLightSpot)
{
}

SpotLightObject::SpotLightObject(const ColorRGB& color, float range, float apex, const char *name) : ShadowLightObject(kLightSpot, color, range, name)
{
	apexTangent = apex;
	CalculateAspectRatio();
}

SpotLightObject::SpotLightObject(const SpotLightObject& spotLightObject) : ShadowLightObject(spotLightObject)
{
	apexTangent = spotLightObject.apexTangent;
	CalculateAspectRatio();
}

SpotLightObject::~SpotLightObject()
{
}

Object *SpotLightObject::Replicate(void) const
{
	return (new SpotLightObject(*this));
}

void SpotLightObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	ShadowLightObject::Pack(data, packFlags);
	
	data << ChunkHeader('APEX', 4);
	data << apexTangent;
	
	data << TerminatorChunk;
}

void SpotLightObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	ShadowLightObject::Unpack(data, unpackFlags);
	UnpackChunkList<SpotLightObject>(data, unpackFlags);
}

bool SpotLightObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'APEX':
			
			data >> apexTangent;
			return (true);
	}
	
	return (false);
}

int32 SpotLightObject::GetObjectSize(float *size) const
{
	size[0] = GetLightRange();
	size[1] = apexTangent;
	return (2);
}

void SpotLightObject::SetObjectSize(const float *size)
{
	SetLightRange(size[0]);
	apexTangent = size[1];
}

void SpotLightObject::CalculateAspectRatio(void)
{
	const Texture *texture = GetShadowMap();
	aspectRatio = (float) texture->GetTextureWidth() / (float) texture->GetTextureHeight();
}

void SpotLightObject::SetShadowMap(const char *name)
{
	ShadowLightObject::SetShadowMap(name);
	CalculateAspectRatio();
}

// ZYURVUR
