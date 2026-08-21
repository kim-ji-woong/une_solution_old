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
#include "C4Manipulator.h"
#include "C4Configuration.h"


using namespace C4;


const char C4::kConnectorKeyZone[] = "%Zone";


PortalObject::PortalObject(PortalType type) : Object(kObjectPortal)
{
	portalType = type;
	portalFlags = 0;
	perspectiveExclusionMask = 0;
	
	vertexCount = 0;
}

PortalObject::PortalObject(PortalType type, const Vector2D& size) : Object(kObjectPortal)
{
	portalType = type;
	portalFlags = 0;
	perspectiveExclusionMask = 0;
	
	SetPortalSize(size);
}

PortalObject::~PortalObject()
{
}

PortalObject *PortalObject::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kPortalDirect:
			
			return (new DirectPortalObject);
		
		case kPortalRemote:
			
			return (new RemotePortalObject);
		
		case kPortalCamera:
			
			return (new CameraPortalObject);
		
		case kPortalOcclusion:
			
			return (new OcclusionPortalObject);
	}
	
	return (nullptr);
}

void PortalObject::PackType(Packer& data) const
{
	Object::PackType(data);
	data << portalType;
}

void PortalObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	data << ChunkHeader('DATA', 4);
	data << portalFlags;
	
	if (perspectiveExclusionMask != 0)
	{
		data << ChunkHeader('EXCL', 4);
		data << perspectiveExclusionMask;
	}
	
	data << ChunkHeader('VERT', 4 + vertexCount * sizeof(Point3D));
	data << vertexCount;
	data.WriteArray(vertexCount, portalVertex);
	
	data << TerminatorChunk;
}

void PortalObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<PortalObject>(data, unpackFlags);
}

bool PortalObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'DATA':
			
			data >> portalFlags;
			return (true);
		
		case 'EXCL':
			
			data >> perspectiveExclusionMask;
			return (true);
		
		case 'VERT':
			 
			data >> vertexCount;
			data.ReadArray(vertexCount, portalVertex); 
			return (true); 
	} 
	
	return (false); 
}

void *PortalObject::BeginSettingsUnpack(void)
{ 
	perspectiveExclusionMask = 0;
	return (nullptr);
}
 
int32 PortalObject::GetCategoryCount(void) const
{
	return (1);
}

Type PortalObject::GetCategoryType(int32 index, const char **title) const
{
	if (index == 0)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kObjectPortal));
		return (kObjectPortal);
	}
	
	return (0);
}

int32 PortalObject::GetCategorySettingCount(Type category) const
{
	if (category == kObjectPortal) return (6);
	return (0);
}

Setting *PortalObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kObjectPortal)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID(kObjectPortal, 'EXCL'));
			return (new HeadingSetting('EXCL', title));
		}
		
		if (index == 1)
		{
			const char *title = table->GetString(StringID(kObjectPortal, 'EXCL', 'DRCT'));
			return (new BooleanSetting('DRCT', ((perspectiveExclusionMask & kPerspectiveDirect) != 0), title));
		}
		
		if (index == 2)
		{
			const char *title = table->GetString(StringID(kObjectPortal, 'EXCL', 'REFL'));
			return (new BooleanSetting('REFL', ((perspectiveExclusionMask & kPerspectiveReflection) != 0), title));
		}
		
		if (index == 3)
		{
			const char *title = table->GetString(StringID(kObjectPortal, 'EXCL', 'REFR'));
			return (new BooleanSetting('REFR', ((perspectiveExclusionMask & kPerspectiveRefraction) != 0), title));
		}
		
		if (index == 4)
		{
			const char *title = table->GetString(StringID(kObjectPortal, 'EXCL', 'CAMR'));
			return (new BooleanSetting('CAMR', ((perspectiveExclusionMask & kPerspectiveCameraWidget) != 0), title));
		}
		
		if (index == 5)
		{
			const char *title = table->GetString(StringID(kObjectPortal, 'EXCL', 'RPRT'));
			return (new BooleanSetting('RPRT', ((perspectiveExclusionMask & kPerspectiveRemotePortal) != 0), title));
		}
	}
	
	return (nullptr);
}

void PortalObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kObjectPortal)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'DRCT')
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

void PortalObject::SetPortalSize(const Vector2D& size)
{
	vertexCount = 4;
	
	float x = size.x;
	float y = size.y;
	
	portalVertex[0].Set(0.0F, 0.0F, 0.0F);
	portalVertex[1].Set(x, 0.0F, 0.0F);
	portalVertex[2].Set(x, y, 0.0F);
	portalVertex[3].Set(0.0F, y, 0.0F);
}


DirectPortalObject::DirectPortalObject() : PortalObject(kPortalDirect)
{
}

DirectPortalObject::DirectPortalObject(const Vector2D& size) : PortalObject(kPortalDirect, size)
{
}

DirectPortalObject::~DirectPortalObject()
{
}

int32 DirectPortalObject::GetCategorySettingCount(Type category) const
{
	int32 count = PortalObject::GetCategorySettingCount(category);
	if (category == kObjectPortal) count += 5;
	return (count);
}

Setting *DirectPortalObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kObjectPortal)
	{
		int32 count = PortalObject::GetCategorySettingCount(kObjectPortal);
		if (index >= count)
		{
			const StringTable *table = TheInterfaceMgr->GetStringTable();
			
			if (index == count)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalDirect));
				return (new HeadingSetting('PDIR', title));
			}
			
			if (index == count + 1)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalDirect, 'LITE'));
				return (new BooleanSetting('LITE', ((GetPortalFlags() & kPortalLightInhibit) != 0), title));
			}
			
			if (index == count + 2)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalDirect, 'STAT'));
				return (new BooleanSetting('STAT', ((GetPortalFlags() & kPortalStaticLightInhibit) != 0), title));
			}
			
			if (index == count + 3)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalDirect, 'SHAD'));
				return (new BooleanSetting('SHAD', ((GetPortalFlags() & kPortalShadowMapInhibit) != 0), title));
			}
			
			if (index == count + 4)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalDirect, 'NFOG'));
				return (new BooleanSetting('NFOG', ((GetPortalFlags() & kPortalFogInhibit) != 0), title));
			}
			
			return (nullptr);
		}
	}
	
	return (PortalObject::GetCategorySetting(category, index, flags));
}

void DirectPortalObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kObjectPortal)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'LITE')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) SetPortalFlags(GetPortalFlags() | kPortalLightInhibit);
			else SetPortalFlags(GetPortalFlags() & ~kPortalLightInhibit);
		}
		else if (identifier == 'STAT')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) SetPortalFlags(GetPortalFlags() | kPortalStaticLightInhibit);
			else SetPortalFlags(GetPortalFlags() & ~kPortalStaticLightInhibit);
		}
		else if (identifier == 'SHAD')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) SetPortalFlags(GetPortalFlags() | kPortalShadowMapInhibit);
			else SetPortalFlags(GetPortalFlags() & ~kPortalShadowMapInhibit);
		}
		else if (identifier == 'NFOG')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) SetPortalFlags(GetPortalFlags() | kPortalFogInhibit);
			else SetPortalFlags(GetPortalFlags() & ~kPortalFogInhibit);
		}
		else
		{
			PortalObject::SetCategorySetting(kObjectPortal, setting);
		}
	}
}


RemotePortalObject::RemotePortalObject() : PortalObject(kPortalRemote)
{
	portalBuffer = kPortalBufferPrimary;
	portalClearColor.Set(0.0F, 0.0F, 0.0F, 0.0F);
	portalPlaneOffset = 0.0F;
	
	targetLocatorType = 0;
	
	minDetailLevel = 0;
	detailLevelBias = 0.0F;
	
	focalLengthMultiplier = 1.0F;
}

RemotePortalObject::RemotePortalObject(const Vector2D& size) : PortalObject(kPortalRemote, size)
{
	portalBuffer = kPortalBufferPrimary;
	portalClearColor.Set(0.0F, 0.0F, 0.0F, 0.0F);
	portalPlaneOffset = 0.0F;
	
	targetLocatorType = 0;
	
	minDetailLevel = 0;
	detailLevelBias = 0.0F;
	
	focalLengthMultiplier = 1.0F;
}

RemotePortalObject::~RemotePortalObject()
{
}

void RemotePortalObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	PortalObject::Pack(data, packFlags);
	
	data << ChunkHeader('BUFF', 4);
	data << portalBuffer;
	
	data << ChunkHeader('CLER', sizeof(ColorRGBA));
	data << portalClearColor;
	
	data << ChunkHeader('OFST', 4);
	data << portalPlaneOffset;
	
	data << ChunkHeader('LOCA', 4);
	data << targetLocatorType;
	
	data << ChunkHeader('MLEV', 4);
	data << minDetailLevel;
	
	data << ChunkHeader('BIAS', 4);
	data << detailLevelBias;
	
	data << ChunkHeader('FMUL', 4);
	data << focalLengthMultiplier;
	
	data << TerminatorChunk;
}

void RemotePortalObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	PortalObject::Unpack(data, unpackFlags);
	UnpackChunkList<RemotePortalObject>(data, unpackFlags);
}

bool RemotePortalObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'BUFF':
			
			data >> portalBuffer;
			return (true);
		
		case 'CLER':
			
			data >> portalClearColor;
			return (true);
		
		case 'OFST':
			
			data >> portalPlaneOffset;
			return (true);
		
		case 'LOCA':
			
			data >> targetLocatorType;
			return (true);
		
		case 'MLEV':
			
			data >> minDetailLevel;
			return (true);
		
		case 'BIAS':
			
			data >> detailLevelBias;
			return (true);
		
		case 'FMUL':
			
			data >> focalLengthMultiplier;
			return (true);
	}
	
	return (false);
}

int32 RemotePortalObject::GetCategorySettingCount(Type category) const
{
	int32 count = PortalObject::GetCategorySettingCount(category);
	if (category == kObjectPortal) count += 14;
	return (count);
}

Setting *RemotePortalObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kObjectPortal)
	{
		int32 count = PortalObject::GetCategorySettingCount(kObjectPortal);
		if (index >= count)
		{
			const StringTable *table = TheInterfaceMgr->GetStringTable();
			
			if (index == count)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalRemote));
				return (new HeadingSetting('PREM', title));
			}
			
			if (index == count + 1)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalRemote, 'NSKY'));
				return (new BooleanSetting('NSKY', ((GetPortalFlags() & kPortalSkyboxInhibit) != 0), title));
			}
			
			if (index == count + 2)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalRemote, 'DIST'));
				return (new BooleanSetting('DIST', ((GetPortalFlags() & kPortalDistant) != 0), title));
			}
			
			if (index == count + 3)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalRemote, 'RLIT'));
				return (new BooleanSetting('RLIT', ((GetPortalFlags() & kPortalAllowRemoteLight) != 0), title));
			}
			
			if (index == count + 4)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalRemote, 'OBLQ'));
				return (new BooleanSetting('OBLQ', ((GetPortalFlags() & kPortalObliqueFrustum) != 0), title));
			}
			
			if (index == count + 5)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalRemote, 'RCUR'));
				return (new BooleanSetting('RCUR', ((GetPortalFlags() & kPortalRecursive) != 0), title));
			}
			
			if (index == count + 6)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalRemote, 'BUFF'));
				
				int32 selection = 0;
				if (portalBuffer == kPortalBufferReflection) selection = 1;
				else if (portalBuffer == kPortalBufferRefraction) selection = 2;
				MenuSetting *menu = new MenuSetting('BUFF', selection, title, 3);
				
				menu->SetMenuItemString(0, table->GetString(StringID(kObjectPortal, kPortalRemote, 'BUFF', 'PRIM')));
				menu->SetMenuItemString(1, table->GetString(StringID(kObjectPortal, kPortalRemote, 'BUFF', 'REFL')));
				menu->SetMenuItemString(2, table->GetString(StringID(kObjectPortal, kPortalRemote, 'BUFF', 'REFR')));
				
				return (menu);
			}
			
			if (index == count + 7)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalRemote, 'SPRT'));
				return (new BooleanSetting('SPRT', ((GetPortalFlags() & kPortalSeparateShadowMap) != 0), title));
			}
			
			if (index == count + 8)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalRemote, 'CLER'));
				const char *picker = table->GetString(StringID(kObjectPortal, kPortalRemote, 'CPCK'));
				return (new CheckColorSetting('CLER', ((GetPortalFlags() & kPortalOverrideClearColor) != 0), portalClearColor, title, picker));
			}
			
			if (index == count + 9)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalRemote, 'OFST'));
				return (new TextSetting('OFST', portalPlaneOffset, title));
			}
			
			if (index == count + 10)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalRemote, 'LOCA'));
				return (new TextSetting('LOCA', Text::TypeToString(targetLocatorType), title, 4));
			}
			
			if (index == count + 11)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalRemote, 'MLEV'));
				return (new IntegerSetting('MLEV', minDetailLevel, title, 0, 3, 1));
			}
			
			if (index == count + 12)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalRemote, 'BIAS'));
				return (new TextSetting('BIAS', detailLevelBias, title));
			}
			
			if (index == count + 13)
			{
				const char *title = table->GetString(StringID(kObjectPortal, kPortalRemote, 'FMUL'));
				return (new TextSetting('FMUL', focalLengthMultiplier, title));
			}
			
			return (nullptr);
		}
	}
	
	return (PortalObject::GetCategorySetting(category, index, flags));
}

void RemotePortalObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kObjectPortal)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'NSKY')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) SetPortalFlags(GetPortalFlags() | kPortalSkyboxInhibit);
			else SetPortalFlags(GetPortalFlags() & ~kPortalSkyboxInhibit);
		}
		else if (identifier == 'DIST')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) SetPortalFlags(GetPortalFlags() | kPortalDistant);
			else SetPortalFlags(GetPortalFlags() & ~kPortalDistant);
		}
		else if (identifier == 'RLIT')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) SetPortalFlags(GetPortalFlags() | kPortalAllowRemoteLight);
			else SetPortalFlags(GetPortalFlags() & ~kPortalAllowRemoteLight);
		}
		else if (identifier == 'OBLQ')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) SetPortalFlags(GetPortalFlags() | kPortalObliqueFrustum);
			else SetPortalFlags(GetPortalFlags() & ~kPortalObliqueFrustum);
		}
		else if (identifier == 'RCUR')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) SetPortalFlags(GetPortalFlags() | kPortalRecursive);
			else SetPortalFlags(GetPortalFlags() & ~kPortalRecursive);
		}
		else if (identifier == 'BUFF')
		{
			int32 selection = static_cast<const MenuSetting *>(setting)->GetMenuSelection();
			if (selection == 1) portalBuffer = kPortalBufferReflection;
			else if (selection == 2) portalBuffer = kPortalBufferRefraction;
			else portalBuffer = kPortalBufferPrimary;
		}
		else if (identifier == 'SPRT')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) SetPortalFlags(GetPortalFlags() | kPortalSeparateShadowMap);
			else SetPortalFlags(GetPortalFlags() & ~kPortalSeparateShadowMap);
		}
		else if (identifier == 'CLER')
		{
			const CheckColorSetting *checkColorSetting = static_cast<const CheckColorSetting *>(setting);
			if (checkColorSetting->GetCheckValue() != 0)
			{
				SetPortalFlags(GetPortalFlags() | kPortalOverrideClearColor);
				portalClearColor = checkColorSetting->GetColor();
			}
			else
			{
				SetPortalFlags(GetPortalFlags() & ~kPortalOverrideClearColor);
			}
		}
		else if (identifier == 'OFST')
		{
			portalPlaneOffset = FmaxZero(Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText()));
		}
		else if (identifier == 'LOCA')
		{
			targetLocatorType = Text::StringToType(static_cast<const TextSetting *>(setting)->GetText());
		}
		else if (identifier == 'MLEV')
		{
			minDetailLevel = static_cast<const IntegerSetting *>(setting)->GetIntegerValue();
		}
		else if (identifier == 'BIAS')
		{
			detailLevelBias = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
		}
		else if (identifier == 'FMUL')
		{
			focalLengthMultiplier = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
		}
		else
		{
			PortalObject::SetCategorySetting(kObjectPortal, setting);
		}
	}
}


CameraPortalObject::CameraPortalObject() : PortalObject(kPortalCamera)
{
	minDetailLevel = 0;
	detailLevelBias = 0.0F;
}

CameraPortalObject::CameraPortalObject(const Vector2D& size, int32 width, int32 height) : PortalObject(kPortalCamera, size)
{
	viewportWidth = width;
	viewportHeight = height;
	
	minDetailLevel = 0;
	detailLevelBias = 0.0F;
}

CameraPortalObject::~CameraPortalObject()
{
}

void CameraPortalObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	PortalObject::Pack(data, packFlags);
	
	data << ChunkHeader('VPRT', 8);
	data << viewportWidth;
	data << viewportHeight;
	
	data << ChunkHeader('MLEV', 4);
	data << minDetailLevel;
	
	data << ChunkHeader('BIAS', 4);
	data << detailLevelBias;
	
	data << TerminatorChunk;
}

void CameraPortalObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	PortalObject::Unpack(data, unpackFlags);
	UnpackChunkList<CameraPortalObject>(data, unpackFlags);
}

bool CameraPortalObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'VPRT':
			
			data >> viewportWidth;
			data >> viewportHeight;
			return (true);
		
		case 'MLEV':
			
			data >> minDetailLevel;
			return (true);
		
		case 'BIAS':
			
			data >> detailLevelBias;
			return (true);
	}
	
	return (false);
}


OcclusionPortalObject::OcclusionPortalObject() : PortalObject(kPortalOcclusion)
{
}

OcclusionPortalObject::OcclusionPortalObject(const Vector2D& size) : PortalObject(kPortalOcclusion, size)
{
}

OcclusionPortalObject::~OcclusionPortalObject()
{
}


Portal::Portal(PortalType type) : Node(kNodePortal)
{
	portalType = type;
	connectedZone = nullptr;
	
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdatePostTransform);
}

Portal::Portal(const Portal& portal) : Node(portal)
{
	portalType = portal.portalType;
	connectedZone = nullptr;
	
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdatePostTransform);
}

Portal::~Portal()
{
}

Portal *Portal::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kPortalDirect:
			
			return (new DirectPortal);
		
		case kPortalRemote:
			
			return (new RemotePortal);
		
		case kPortalCamera:
			
			return (new CameraPortal);
		
		case kPortalOcclusion:
			
			return (new OcclusionPortal);
	}
	
	return (nullptr);
}

void Portal::PackType(Packer& data) const
{
	Node::PackType(data);
	data << portalType;
}

void Portal::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Node::Pack(data, packFlags);
	
	data << TerminatorChunk;
}

void Portal::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Node::Unpack(data, unpackFlags);
	UnpackChunkList<Portal>(data, unpackFlags);
}

bool Portal::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	#if C4LEGACY
	
		if (chunkHeader->chunkType == 'LINK')
		{
			int32	zoneIndex;
			
			data >> zoneIndex;
			data.AddNodeLink(zoneIndex, &ZoneLinkProc, this);
			return (true);
		}
	
	#endif
	
	return (false);
}

#if C4LEGACY

	void Portal::ZoneLinkProc(Node *node, void *cookie)
	{
		Portal *portal = static_cast<Portal *>(cookie);
		portal->SetConnectedZone(static_cast<Zone *>(node));
	}

#endif

void Portal::CalculatePostTransform(void)
{
	const Transform4D& inverse = GetInverseWorldTransform();
	worldPlane.Set(inverse(2,0), inverse(2,1), inverse(2,2), inverse(2,3));
	
	const PortalObject *object = GetObject();
	int32 vertexCount = object->GetVertexCount();
	const Point3D *vertex = object->GetVertexArray();
	
	const Transform4D& transform = GetWorldTransform();
	for (machine a = 0; a < vertexCount; a++) worldVertex[a] = transform * vertex[a];
	
	const Point3D *v1 = &worldVertex[vertexCount - 1];
	for (machine a = 0; a < vertexCount; a++)
	{
		const Point3D *v2 = &worldVertex[a];
		
		Bivector4D& edge = worldEdgeLine[a];
		edge = *v1 ^ *v2;
		edge.Standardize();
		
		Vector3D& inward = worldInwardDirection[a];
		inward = worldPlane.GetAntivector3D() % edge.GetTangent();
		inward.Normalize();
		
		v1 = v2;
	}
}

bool Portal::CalculateBoundingBox(Box3D *box) const
{
	const PortalObject *object = GetObject();
	box->Calculate(object->GetVertexCount(), object->GetVertexArray());
	return (true);
}

bool Portal::CalculateBoundingSphere(BoundingSphere *sphere) const
{
	const PortalObject *object = GetObject();
	
	int32 count = object->GetVertexCount();
	if (count > 2)
	{
		const Point3D *vertex = object->GetVertexArray();
		
		Point3D p(0.0F, 0.0F, 0.0F);
		for (machine a = 0; a < count; a++) p += vertex[a];
		p /= (float) count;
		
		float r = 0.0F;
		for (machine a = 0; a < count; a++) r = Fmax(r, SquaredMag(vertex[a] - p));
		
		sphere->SetCenter(p);
		sphere->SetRadius(Sqrt(r));
		return (true);
	}
	
	return (false);
}

int32 Portal::GetInternalConnectorCount(void) const
{
	return (1);
}

const char *Portal::GetInternalConnectorKey(int32 index) const
{
	if (index == 0) return (kConnectorKeyZone);
	return (nullptr);
}

void Portal::ProcessInternalConnectors(void)
{
	connectedZone = static_cast<Zone *>(GetConnectedNode(kConnectorKeyZone));
}

bool Portal::ValidConnectedNode(const ConnectorKey& key, const Node *node) const
{
	if (key == kConnectorKeyZone) return (node->GetNodeType() == kNodeZone);
	return (Node::ValidConnectedNode(key, node));
}

void Portal::SetConnectedZone(Zone *zone)
{
	connectedZone = zone;
	
	if (zone)
	{
		Hub *hub = GetHub();
		if (hub)
		{
			Connector *connector = hub->FindOutgoingConnector(kConnectorKeyZone);
			if (connector)
			{
				connector->SetConnectorTarget(zone);
				return;
			}
		}
		
		AddConnector(kConnectorKeyZone, zone);
	}
	else
	{
		RemoveConnector(kConnectorKeyZone);
	}
}

void Portal::Neutralize(void)
{
	ListElement<Portal>::Detach();
	Node::Neutralize();
}

void Portal::EnterZone(Zone *zone)
{
	if (portalType != kPortalOcclusion) zone->AddPortal(this);
	else zone->AddOcclusionPortal(this);
}

Point3D Portal::CalculateClosestBoundaryPoint(const Point3D& p) const
{
	Vector3D	direction[kMaxPortalVertexCount];
	bool		exterior[kMaxPortalVertexCount];
	
	int32 vertexCount = GetObject()->GetVertexCount();
	
	int32 exteriorCount = 0;
	for (machine a = 0; a < vertexCount; a++)
	{
		direction[a] = p - worldVertex[a];
		float d = direction[a] * worldInwardDirection[a];
		bool b = (d < 0.0F);
		exterior[a] = b;
		exteriorCount += b;
	}
	
	if (exteriorCount != 0)
	{
		const Vector3D *v1 = &direction[vertexCount - 1];
		float d0 = *v1 * worldEdgeLine[vertexCount - 1].GetTangent();
		
		for (machine a = 0; a < vertexCount; a++)
		{
			const Vector3D *v2 = &direction[a];
			const Vector3D& tangent = worldEdgeLine[a].GetTangent();
			float d1 = *v1 * tangent;
			
			if (exterior[a])
			{
				float d2 = *v2 * tangent;
				
				if (d1 > 0.0F)
				{
					if (d2 < 0.0F)
					{
						return (p - *v1 + ProjectOnto(*v1, tangent));
					}
				}
				else
				{
					if (!(d0 < 0.0F)) return (p - *v1);
				}
			}
			
			v1 = v2;
			d0 = d1;
		}
	}
	
	return (p - worldPlane.GetAntivector3D() * (worldPlane ^ p));
}

bool Portal::CalculateClosestBoundaryPoint(const Bivector4D& line, Point3D *result) const
{
	float	edgeProduct[kMaxPortalVertexCount];
	
	int32 vertexCount = GetObject()->GetVertexCount();
	
	int32 interiorCount = 0;
	for (machine a = 0; a < vertexCount; a++)
	{
		float d = line ^ worldEdgeLine[a];
		edgeProduct[a] = d;
		interiorCount += (d < 0.0F);
	}
	
	if (interiorCount == vertexCount) return (false);
	
	float closestDistance = K::infinity;
	const Point3D *v1 = &worldVertex[vertexCount - 1];
	for (machine a = 0; a < vertexCount; a++)
	{
		const Point3D *v2 = &worldVertex[a];
		
		float d = *v2 * line.GetTangent();
		float d2 = SquaredMag(*v2 - line.GetSupport()) - d * d;
		if (d2 < closestDistance)
		{
			closestDistance = d2;
			*result = *v2;
		}
		
		d = edgeProduct[a];
		if (!(d < 0.0F))
		{
			const Bivector4D& edgeLine = worldEdgeLine[a];
			const Vector3D& tangent = edgeLine.GetTangent();
			
			Antivector3D normal = line.GetTangent() ^ tangent;
			Point3D p = (line ^ normal ^ edgeLine).ProjectPoint3D();
			
			if (((p - *v1) * tangent > 0.0F) && ((p - *v2) * tangent < 0.0F))
			{
				d *= InverseMag(normal);
				d *= d;
				
				if (d < closestDistance)
				{
					closestDistance = d;
					*result = p;
				}
			}
		}
		
		v1 = v2;
	}
	
	return (true);
}


DirectPortal::DirectPortal() : Portal(kPortalDirect)
{
}

DirectPortal::DirectPortal(const Vector2D& size) : Portal(kPortalDirect)
{
	SetNewObject(new DirectPortalObject(size));
}

DirectPortal::DirectPortal(const DirectPortal& directPortal) : Portal(directPortal)
{
}

DirectPortal::~DirectPortal()
{
}

Node *DirectPortal::Replicate(void) const
{
	return (new DirectPortal(*this));
}


RemotePortal::RemotePortal() : Portal(kPortalRemote)
{
	previousCameraWorldTransform(3,3) = 0.0F;
}

RemotePortal::RemotePortal(const Vector2D& size) : Portal(kPortalRemote)
{
	previousCameraWorldTransform(3,3) = 0.0F;
	
	SetNewObject(new RemotePortalObject(size));
}

RemotePortal::RemotePortal(const RemotePortal& remotePortal) : Portal(remotePortal)
{
	previousCameraWorldTransform(3,3) = 0.0F;
}

RemotePortal::~RemotePortal()
{
}

Node *RemotePortal::Replicate(void) const
{
	return (new RemotePortal(*this));
}

void RemotePortal::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Portal::Pack(data, packFlags);
	
	data << ChunkHeader('RMOT', sizeof(Transform4D));
	data << remoteTransform;
	
	data << TerminatorChunk;
}

void RemotePortal::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Portal::Unpack(data, unpackFlags);
	UnpackChunkList<RemotePortal>(data, unpackFlags);
}

bool RemotePortal::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'RMOT':
			
			data >> remoteTransform;
			return (true);
	}
	
	return (false);
}

void RemotePortal::Preprocess(void)
{
	Portal::Preprocess();
	
	const RemotePortalObject *object = GetObject();
	if (object->GetTargetLocatorType() == 0)
	{
		remoteTransform.SetIdentity();
		if (object->GetPortalBuffer() != kPortalBufferRefraction) remoteTransform(2,2) = -1.0F;
	}
}


CameraPortal::CameraPortal() : Portal(kPortalCamera)
{
	cameraTexture = nullptr;
	renderSizeProc = nullptr;
}

CameraPortal::CameraPortal(const Vector2D& size, int32 width, int32 height) : Portal(kPortalCamera)
{
	SetNewObject(new CameraPortalObject(size, width, height));
	
	cameraTexture = nullptr;
	renderSizeProc = nullptr;
}

CameraPortal::CameraPortal(const CameraPortal& cameraPortal) : Portal(cameraPortal)
{
	cameraTexture = nullptr;
	renderSizeProc = nullptr;
}

CameraPortal::~CameraPortal()
{
	if (cameraTexture) cameraTexture->Release();
}

Node *CameraPortal::Replicate(void) const
{
	return (new CameraPortal(*this));
}

void CameraPortal::SetCameraTexture(Texture *texture)
{
	if (texture != cameraTexture)
	{
		if (cameraTexture) cameraTexture->Release();
		
		texture->Retain();
		cameraTexture = texture;
	}
}


OcclusionPortal::OcclusionPortal() : Portal(kPortalOcclusion)
{
}

OcclusionPortal::OcclusionPortal(const Vector2D& size) : Portal(kPortalOcclusion)
{
	SetNewObject(new OcclusionPortalObject(size));
}

OcclusionPortal::OcclusionPortal(const OcclusionPortal& occlusionPortal) : Portal(occlusionPortal)
{
}

OcclusionPortal::~OcclusionPortal()
{
}

Node *OcclusionPortal::Replicate(void) const
{
	return (new OcclusionPortal(*this));
}

CameraRegion *OcclusionPortal::CalculateFrustumOcclusionRegion(const FrustumCamera *camera, Zone *zone) const
{
	const Point3D& cameraPosition = camera->GetWorldPosition();
	const Vector3D& viewDirection = camera->GetWorldTransform()[2];
	
	const Antivector4D& portalPlane = GetWorldPlane();
	float distance = portalPlane ^ camera->GetWorldPosition();
	if ((distance > 0.0F) && ((portalPlane ^ viewDirection) < camera->GetSineHalfField()))
	{
		Point3D		tempVertex[2][kMaxPortalVertexCount];
		
		int32 vertexCount = GetObject()->GetVertexCount();
		const Point3D *vertex = GetWorldVertexArray();
		
		for (machine a = 0; a < 4; a++)
		{
			int8	location[kMaxPortalVertexCount];
			
			Point3D *result = tempVertex[a & 1];
			Antivector4D plane(camera->GetFrustumPlaneNormal(a), cameraPosition);
			vertexCount = Math::ClipPolygonAgainstPlane(vertexCount, vertex, plane, location, result);
			if (vertexCount == 0) return (nullptr);
			vertex = result;
		}
		
		Antivector4D plane = -portalPlane;
		CameraRegion *region = new CameraRegion(camera, zone);
		region->SetFrustumOcclusionPortalPlanes(vertexCount, vertex, 1, &plane);
		return (region);
	}
	
	return (nullptr);
}

// ZYURVUR
