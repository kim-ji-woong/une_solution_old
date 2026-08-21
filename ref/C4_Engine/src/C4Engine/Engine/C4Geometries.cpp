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
#include "C4Primitives.h"
#include "C4Terrain.h"
#include "C4Water.h"
#include "C4Configuration.h"


using namespace C4;


const char C4::kConnectorKeyPaint[] = "%Paint";


Geometry::Geometry(GeometryType type) :
		RenderableNode(kNodeGeometry, kRenderIndexedTriangles, kRenderDepthTest),
		stencilData(this)
{
	geometryType = type;
	
	minGeometryDetailLevel = 0;
	perspectiveExclusionMask = 0;
	
	shadowStamp = 0xFFFFFFFF;
	queryThreadFlags = 0;
	
	materialCount = 1;
	materialObject = nullptr;
	
	RenderSegment *segment = GetFirstRenderSegment();
	segment->SetMaterialObjectPointer(&materialObject);
	segmentStorage = nullptr;
}

Geometry::Geometry(const Geometry& geometry) :
		RenderableNode(geometry),
		stencilData(this)
{
	geometryType = geometry.geometryType;
	
	minGeometryDetailLevel = geometry.minGeometryDetailLevel;
	perspectiveExclusionMask = geometry.perspectiveExclusionMask;
	
	shadowStamp = 0xFFFFFFFF;
	queryThreadFlags = 0;
	
	materialCount = 1;
	materialObject = geometry.materialObject;
	if (materialObject) materialObject->Retain();
	
	RenderSegment *segment = GetFirstRenderSegment();
	segment->SetMaterialObjectPointer(&materialObject);
	segmentStorage = nullptr;
	
	int32 count = geometry.materialCount;
	if (count > 1)
	{
		SetMaterialCount(count);
		
		count--;
		for (machine a = 0; a < count; a++)
		{
			MaterialObject *object = geometry.GetMaterialObjectTable()[a];
			if (object)
			{
				object->Retain();
				GetMaterialObjectTable()[a] = object;
			}
		}
	}
}

Geometry::~Geometry()
{
	for (machine a = 0; a < kMaxStaticStencilVolumeCount; a++)
	{
		StencilVolume *stencilVolume = staticStencilVolume[a];
		delete stencilVolume;
	}
	
	ReleaseSegmentStorage();
	if (materialObject) materialObject->Release();
}

Geometry *Geometry::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kGeometryMesh:
			
			return (new MeshGeometry);
		
		case kGeometryPrimitive:
			
			return (PrimitiveGeometry::Construct(++data, unpackFlags));
		
		case kGeometryTerrain:
			
			#if C4LEGACY
			
				if (data.GetVersion() < 23) return (new TerrainGeometry);
			
			#endif 
			
			if ((++data).GetType() == 0) return (new TerrainGeometry); 
			return (new TerrainLevelGeometry); 
		 
		case kGeometryWater:
			 
			return (new WaterGeometry);
		
		case kGeometryHorizonWater:
			 
			return (new HorizonWaterGeometry);
	}
	
	return (nullptr); 
}

void Geometry::PackType(Packer& data) const
{
	RenderableNode::PackType(data);
	data << geometryType;
}

void Geometry::Prepack(List<Object> *linkList) const
{
	RenderableNode::Prepack(linkList);
	if (materialObject) linkList->Append(materialObject);
	
	int32 count = materialCount;
	if (count > 1)
	{
		count--;
		for (machine a = 0; a < count; a++)
		{
			MaterialObject *object = GetMaterialObjectTable()[a];
			if (object) linkList->Append(object);
		}
	}
}

void Geometry::Pack(Packer& data, unsigned_int32 packFlags) const
{
	RenderableNode::Pack(data, packFlags);
	
	if (!(packFlags & kPackSettings))
	{
		data << ChunkHeader('MAT0', 4);
		
		const MaterialObject *object = materialObject;
		int32 objectIndex = (object) ? object->GetObjectIndex() : -1;
		data << objectIndex;
		
		int32 count = materialCount;
		if (count > 1)
		{
			data << ChunkHeader('MCNT', 4);
			data << count;
			
			count--;
			for (machine a = 0; a < count; a++)
			{
				object = GetMaterialObjectTable()[a];
				if (object)
				{
					data << ChunkHeader('MATL', 8);
					data << (int32) a;
					data << object->GetObjectIndex();
				}
			}
		}
	}
	
	if (perspectiveExclusionMask != 0)
	{
		data << ChunkHeader('EXCL', 4);
		data << perspectiveExclusionMask;
	}
	
	data << TerminatorChunk;
}

void Geometry::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	RenderableNode::Unpack(data, unpackFlags);
	UnpackChunkList<Geometry>(data, unpackFlags);
}

bool Geometry::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'MAT0':
		
		#if C4LEGACY
		
			case 'DATA':
		
		#endif
		
		{
			int32	objectIndex;
			
			data >> objectIndex;
			if (objectIndex >= 0) data.AddObjectLink(objectIndex, &MaterialObjectLinkProc, &materialObject);
			return (true);
		}
		
		case 'MCNT':
		{
			int32	count;
			
			data >> count;
			SetMaterialCount(count);
			return (true);
		}
		
		case 'MATL':
		{
			int32	index;
			int32	objectIndex;
			
			data >> index;
			data >> objectIndex;
			data.AddObjectLink(objectIndex, &MaterialObjectLinkProc, &GetMaterialObjectTable()[index]);
			return (true);
		}
		
		case 'EXCL':
			
			data >> perspectiveExclusionMask;
			return (true);
	}
	
	return (false);
}

void *Geometry::BeginSettingsUnpack(void)
{
	perspectiveExclusionMask = 0;
	return (RenderableNode::BeginSettingsUnpack());
}

void Geometry::MaterialObjectLinkProc(Object *object, void *cookie)
{
	*static_cast<MaterialObject **>(cookie) = static_cast<MaterialObject *>(object);
	object->Retain();
}

int32 Geometry::GetCategorySettingCount(Type category) const
{
	int32 count = RenderableNode::GetCategorySettingCount(category);
	if (category == 'NODE') count += 13;
	return (count);
}

Setting *Geometry::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == 'NODE')
	{
		int32 count = RenderableNode::GetCategorySettingCount('NODE');
		if (index >= count)
		{
			const StringTable *table = TheInterfaceMgr->GetStringTable();
			
			if (index == count)
			{
				const char *title = table->GetString(StringID('NODE', 'GMEX'));
				return (new HeadingSetting('GMEX', title));
			}
			
			if (index == count + 1)
			{
				const char *title = table->GetString(StringID('NODE', 'EXCL', 'DRCT'));
				return (new BooleanSetting('GDRC', ((perspectiveExclusionMask & kPerspectiveDirect) != 0), title));
			}
			
			if (index == count + 2)
			{
				const char *title = table->GetString(StringID('NODE', 'EXCL', 'REFL'));
				return (new BooleanSetting('GRFL', ((perspectiveExclusionMask & kPerspectiveReflection) != 0), title));
			}
			
			if (index == count + 3)
			{
				const char *title = table->GetString(StringID('NODE', 'EXCL', 'REFR'));
				return (new BooleanSetting('GRFR', ((perspectiveExclusionMask & kPerspectiveRefraction) != 0), title));
			}
			
			if (index == count + 4)
			{
				const char *title = table->GetString(StringID('NODE', 'EXCL', 'CAMR'));
				return (new BooleanSetting('GCAM', ((perspectiveExclusionMask & kPerspectiveCameraWidget) != 0), title));
			}
			
			if (index == count + 5)
			{
				const char *title = table->GetString(StringID('NODE', 'EXCL', 'RPRT'));
				return (new BooleanSetting('GRPT', ((perspectiveExclusionMask & kPerspectiveRemotePortal) != 0), title));
			}
			
			if (index == count + 6)
			{
				const char *title = table->GetString(StringID('NODE', 'EXCL', 'AMBT'));
				return (new BooleanSetting('GAMB', ((perspectiveExclusionMask & kPerspectiveAmbientSpace) != 0), title));
			}
			
			if (index == count + 7)
			{
				const char *title = table->GetString(StringID('NODE', 'SHEX'));
				return (new HeadingSetting('SHEX', title));
			}
			
			if (index == count + 8)
			{
				const char *title = table->GetString(StringID('NODE', 'EXCL', 'DRCT'));
				return (new BooleanSetting('SDRC', ((perspectiveExclusionMask & kShadowPerspectiveDirect) != 0), title));
			}
			
			if (index == count + 9)
			{
				const char *title = table->GetString(StringID('NODE', 'EXCL', 'REFL'));
				return (new BooleanSetting('SRFL', ((perspectiveExclusionMask & kShadowPerspectiveReflection) != 0), title));
			}
			
			if (index == count + 10)
			{
				const char *title = table->GetString(StringID('NODE', 'EXCL', 'REFR'));
				return (new BooleanSetting('SRFR', ((perspectiveExclusionMask & kShadowPerspectiveRefraction) != 0), title));
			}
			
			if (index == count + 11)
			{
				const char *title = table->GetString(StringID('NODE', 'EXCL', 'CAMR'));
				return (new BooleanSetting('SCAM', ((perspectiveExclusionMask & kShadowPerspectiveCameraWidget) != 0), title));
			}
			
			if (index == count + 12)
			{
				const char *title = table->GetString(StringID('NODE', 'EXCL', 'RPRT'));
				return (new BooleanSetting('SRPT', ((perspectiveExclusionMask & kShadowPerspectiveRemotePortal) != 0), title));
			}
			
			return (nullptr);
		}
	}
	
	return (RenderableNode::GetCategorySetting(category, index, flags));
}

void Geometry::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == 'NODE')
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'GDRC')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) perspectiveExclusionMask |= kPerspectiveDirect;
			else perspectiveExclusionMask &= ~kPerspectiveDirect;
		}
		else if (identifier == 'GRFL')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) perspectiveExclusionMask |= kPerspectiveReflection;
			else perspectiveExclusionMask &= ~kPerspectiveReflection;
		}
		else if (identifier == 'GRFR')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) perspectiveExclusionMask |= kPerspectiveRefraction;
			else perspectiveExclusionMask &= ~kPerspectiveRefraction;
		}
		else if (identifier == 'GCAM')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) perspectiveExclusionMask |= kPerspectiveCameraWidget;
			else perspectiveExclusionMask &= ~kPerspectiveCameraWidget;
		}
		else if (identifier == 'GRPT')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) perspectiveExclusionMask |= kPerspectiveRemotePortal;
			else perspectiveExclusionMask &= ~kPerspectiveRemotePortal;
		}
		else if (identifier == 'GAMB')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) perspectiveExclusionMask |= kPerspectiveAmbientSpace;
			else perspectiveExclusionMask &= ~kPerspectiveAmbientSpace;
		}
		else if (identifier == 'SDRC')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) perspectiveExclusionMask |= kShadowPerspectiveDirect;
			else perspectiveExclusionMask &= ~kShadowPerspectiveDirect;
		}
		else if (identifier == 'SRFL')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) perspectiveExclusionMask |= kShadowPerspectiveReflection;
			else perspectiveExclusionMask &= ~kShadowPerspectiveReflection;
		}
		else if (identifier == 'SRFR')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) perspectiveExclusionMask |= kShadowPerspectiveRefraction;
			else perspectiveExclusionMask &= ~kShadowPerspectiveRefraction;
		}
		else if (identifier == 'SCAM')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) perspectiveExclusionMask |= kShadowPerspectiveCameraWidget;
			else perspectiveExclusionMask &= ~kShadowPerspectiveCameraWidget;
		}
		else if (identifier == 'SRPT')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) perspectiveExclusionMask |= kShadowPerspectiveRemotePortal;
			else perspectiveExclusionMask &= ~kShadowPerspectiveRemotePortal;
		}
		else
		{
			RenderableNode::SetCategorySetting('NODE', setting);
		}
	}
	else
	{
		RenderableNode::SetCategorySetting(category, setting);
	}
}

int32 Geometry::GetInternalConnectorCount(void) const
{
	return (1);
}

const char *Geometry::GetInternalConnectorKey(int32 index) const
{
	if (index == 0) return (kConnectorKeyPaint);
	return (nullptr);
}

bool Geometry::ValidConnectedNode(const ConnectorKey& key, const Node *node) const
{
	if (key == kConnectorKeyPaint)
	{
		if (node->GetNodeType() == kNodeSpace) return (static_cast<const Space *>(node)->GetSpaceType() == kSpacePaint);
		return (false);
	}
	
	return (Node::ValidConnectedNode(key, node));
}

PaintSpace *Geometry::GetConnectedPaintSpace(void) const
{
	Node *node = GetConnectedNode(kConnectorKeyPaint);
	if (node) return (static_cast<PaintSpace *>(node));
	return (nullptr);
}

void Geometry::SetConnectedPaintSpace(PaintSpace *space)
{
	if (space)
	{
		SetPaintEnvironment(space->GetPaintEnvironment());
		
		Hub *hub = GetHub();
		if (hub)
		{
			Connector *connector = hub->FindOutgoingConnector(kConnectorKeyPaint);
			if (connector)
			{
				connector->SetConnectorTarget(space);
				return;
			}
		}
		
		AddConnector(kConnectorKeyPaint, space);
	}
	else
	{
		SetNullPaintEnvironment();
		RemoveConnector(kConnectorKeyPaint);
	}
}

bool Geometry::AlphaTestMaterial(void) const
{
	const MaterialObject *material = materialObject;
	if (material)
	{
		unsigned_int32 flags = material->GetMaterialFlags();
		
		int32 count = materialCount - 1;
		const MaterialObject *const *materialTable = GetMaterialObjectTable();
		for (machine a = 0; a < count; a++)
		{
			material = materialTable[a];
			if (material) flags |= material->GetMaterialFlags();
		}
		
		return ((flags & kMaterialAlphaTest) != 0);
	}
	
	return (false);
}

void Geometry::Preprocess(void)
{
	SetTransformable(this);
	SetPreviousWorldTransformPointer(&GetPreviousWorldTransform());
	
	SetVisibilityProc(&BoxVisible);
	SetOcclusionProc(&BoxOccluded);
	
	int32 renderStage = AlphaTestMaterial() ? kRenderStageAlphaTest : kRenderStageDefault;
	
	GeometryObject *object = GetObject();
	unsigned_int32 geometryFlags = object->GetGeometryFlags();
	unsigned_int32 shaderFlags = (geometryFlags & kGeometryCubeLightInhibit) ? kShaderCubeLightInhibit : 0;
	unsigned_int32 renderState = kRenderDepthTest;
	unsigned_int32 blendState = kBlendReplace;
	
	if (geometryFlags & kGeometryRemotePortal)
	{
		renderStage = kRenderStageCover;
		blendState = kBlendInterpolate;
	}
	else if (geometryFlags & kGeometryRenderEffectPass)
	{
		renderStage = kRenderStageEffectTransparent;
		renderState |= kRenderDepthInhibit;
		
		SetTransparentPosition(&GetWorldPosition());
		
		unsigned_int32 geometryEffectFlags = object->GetGeometryEffectFlags();
		if (geometryEffectFlags & kGeometryEffectShader) shaderFlags |= kShaderAmbientEffect;
		blendState = (geometryEffectFlags & kGeometryEffectAccumulate) ? kBlendAccumulate | kBlendAlphaPreserve : kBlendInterpolate | kBlendAlphaPreserve;
	}
	
	if (geometryFlags & kGeometryRenderDecal)
	{
		renderStage = (geometryFlags & kGeometryRenderEffectPass) ? kRenderStageEffectOpaque : kRenderStageDecal;
		renderState |= kRenderDepthOffset;
		SetDepthOffset(0.0078125F, GetBoundingSphereCenterPointer());
	}
	
	geometryRenderStage = renderStage;
	SetRenderState(renderState);
	SetShaderFlags(shaderFlags);
	SetAmbientBlendState(blendState);
	
	unsigned_int32 renderableFlags = GetRenderableFlags();
	if (geometryFlags & kGeometryMotionBlurInhibit) renderableFlags |= kRenderableStructureVelocityZero;
	if (geometryFlags & kGeometryFogInhibit) renderableFlags |= kRenderableFogInhibit;
	SetRenderableFlags(renderableFlags);
	
	const PaintSpace *paintSpace = GetConnectedPaintSpace();
	if (paintSpace) SetPaintEnvironment(paintSpace->GetPaintEnvironment());
	
	for (machine a = 0; a < kMaxGeometryArrayCount; a++) arrayBundle[a] = nullptr;
	
	RenderableNode::Preprocess();
	object->Preprocess(GetDynamicArrayFlags());
	
	BondVisibility();
	
	SetVertexBuffer(kVertexBufferStaticArray, object->GetStaticVertexBuffer());
	SetVertexBuffer(kVertexBufferIndexArray, object->GetIndexBuffer());
	
	shadowFrontArray = nullptr;
	stencilData.Deactivate();
	
	SetDetailLevel(0);
}

void Geometry::Neutralize(void)
{
	GeometryObject *object = GetObject();
	if (object) object->Neutralize();
	
	SetDynamicArrayFlags(0);
	SetNullPaintEnvironment();
	
	RenderableNode::Neutralize();
}

void Geometry::EnterZone(Zone *zone)
{
	const AmbientEnvironment *environment = zone->GetAmbientEnvironment();
	if (*environment->environmentMap) InvalidateAmbientShaderData();
	SetAmbientEnvironment(environment);
}

void Geometry::ReleaseSegmentStorage(void)
{
	if (segmentStorage)
	{
		int32 count = materialCount - 1;
		
		MaterialObject **table = GetMaterialObjectTable();
		for (machine a = 0; a < count; a++)
		{
			MaterialObject *object = table[a];
			if (object) object->Release();
		}
		
		RenderSegment *segment = GetRenderSegmentTable();
		for (machine a = count - 1; a >= 0; a--) segment[a].~RenderSegment();
		
		delete[] segmentStorage;
		segmentStorage = nullptr;
	}
}

void Geometry::SetMaterialCount(int32 count)
{
	if (count <= 1)
	{
		ReleaseSegmentStorage();
		materialCount = 1;
	}
	else
	{
		count--;
		char *newStorage = new char[count * (sizeof(MaterialObject *) + sizeof(RenderSegment))];
		
		MaterialObject **newTable = reinterpret_cast<MaterialObject **>(newStorage);
		MaterialObject *const *oldTable = GetMaterialObjectTable();
		
		int32 transferCount = Min(count, materialCount - 1);
		for (machine a = 0; a < transferCount; a++)
		{
			MaterialObject *object = oldTable[a];
			if (object) object->Retain();
			newTable[a] = object;
		}
		
		for (machine a = transferCount; a < count; a++) newTable[a] = nullptr;
		
		RenderSegment *segment = reinterpret_cast<RenderSegment *>(newTable + count);
		for (machine a = 0; a < count; a++)
		{
			new(&segment[a]) RenderSegment;
			segment[a].SetMaterialObjectPointer(&newTable[a]);
		}
		
		ReleaseSegmentStorage();
		segmentStorage = newStorage;
		materialCount = count + 1;
	}
}

void Geometry::SetMaterialObject(unsigned_int32 index, MaterialObject *object)
{
	MaterialObject **pointer = (index == 0) ? &materialObject : &GetMaterialObjectTable()[index - 1];
	
	MaterialObject *prevObject = *pointer;
	if (prevObject != object)
	{
		if (prevObject) prevObject->Release();
		if (object) object->Retain();
		*pointer = object;
	}
}

void Geometry::OptimizeMaterials(void)
{
	int32 *remap = new int32[materialCount];
	for (machine a = 0; a < materialCount; a++) remap[a] = -1;
	
	const GeometryObject *object = GetObject();
	int32 surfaceCount = object->GetSurfaceCount();
	for (machine a = 0; a < surfaceCount; a++)
	{
		int32 index = object->GetSurfaceData(a)->materialIndex;
		if (index < materialCount) remap[index] = 0;
	}
	
	int32 newCount = 0;
	for (machine a = 0; a < materialCount; a++)
	{
		if (remap[a] == 0)
		{
			MaterialObject *material = GetMaterialObject(a);
			for (machine b = 0; b < newCount; b++)
			{
				if (GetMaterialObject(b) == material)
				{
					remap[a] = b;
					goto next;
				}
			}
			
			SetMaterialObject(newCount, material);
			remap[a] = newCount++;
		}
		
		next:;
	}
	
	SetMaterialCount(newCount);
	GetFirstRenderSegment()->InvalidateShaderData();
	
	for (machine a = 0; a < surfaceCount; a++)
	{
		SurfaceData *surfaceData = object->GetSurfaceData(a);
		int32 index = remap[surfaceData->materialIndex];
		surfaceData->materialIndex = (unsigned_int16) ((index >= 0) ? index : 0);
	}
	
	int32 levelCount = object->GetGeometryLevelCount();
	for (machine a = 0; a < levelCount; a++)
	{
		GeometryLevel	tempLevel;
		
		GeometryLevel *geometryLevel = object->GetGeometryLevel(a);
		tempLevel.CopyGeometryLevel(geometryLevel);
		geometryLevel->BuildSegmentArray(&tempLevel, surfaceCount, object->GetSurfaceData());
	}
	
	delete[] remap;
	
	SetDetailLevel(0);
}

const MaterialObject *Geometry::GetTriangleMaterial(int32 triangleIndex) const
{
	const RenderSegment *segment = GetFirstRenderSegment();
	do
	{
		int32 start = segment->GetFaceStart();
		int32 count = segment->GetFaceCount();
		if ((unsigned_int32) (triangleIndex - start) < (unsigned_int32) count)
		{
			const MaterialObject *const *object = segment->GetMaterialObjectPointer();
			if (object) return (*object);
			break;
		}
		
		segment = segment->GetNextRenderSegment();
	} while (segment);
	
	return (nullptr);
}

void Geometry::SetDetailLevel(int32 level)
{
	geometryDetailLevel = level;
	
	const GeometryObject *object = GetObject();
	if (object)
	{
		unsigned_int32 dynamicFlags = GetDynamicArrayFlags();
		const GeometryLevel *geometryLevel = object->GetGeometryLevel(level);
		
		int32 vertexCount = geometryLevel->GetVertexCount();
		SetVertexCount(vertexCount);
		
		const unsigned_int8 *arrayIndex = geometryLevel->GetAttributeArrayIndex();
		int32 arrayCount = geometryLevel->GetAttributeArrayCount();
		for (machine a = 0; a < arrayCount; a++)
		{
			int32 index = arrayIndex[a];
			if ((dynamicFlags & (1 << index)) == 0)
			{
				arrayBundle[index] = geometryLevel->GetArrayBundle(index);
				
				const float *array = geometryLevel->GetArray<float>(index);
				SetAttributeArray(index, array, geometryLevel->GetArrayDescriptor(index)->componentCount);
				SetAttributeOffset(index, geometryLevel->GetAttributeOffset(a));
			}
		}
		
		for (machine a = kMaxAttributeArrayCount; a < kMaxGeometryArrayCount; a++)
		{
			const ArrayBundle *bundle = geometryLevel->GetArrayBundle(a);
			if (bundle->pointer) arrayBundle[a] = bundle;
		}
		
		const unsigned_int16 *face = geometryLevel->GetArray<unsigned_int16>(kArrayFace);
		if (face)
		{
			SetFaceArray(face);
			SetFaceOffset(geometryLevel->GetFaceOffset());
		}
		
		RenderSegment *segment = GetFirstRenderSegment();
		const SegmentData *data = geometryLevel->GetArray<SegmentData>(kArraySegment);
		if (!data)
		{
			segment->SetNextRenderSegment(nullptr);
			segment->SetFaceRange(0, geometryLevel->GetFaceCount());
		}
		else
		{
			segment->SetFaceRange(data->faceStart, data->faceCount);
			RenderSegment *prevSegment = segment;
			
			RenderSegment *segmentTable = GetRenderSegmentTable();
			
			int32 segmentCount = geometryLevel->GetArrayDescriptor(kArraySegment)->elementCount - 1;
			for (machine a = 0; a < segmentCount; a++)
			{
				data++;
				
				segment = &segmentTable[data->materialIndex - 1];
				segment->SetFaceRange(data->faceStart, data->faceCount);
				
				prevSegment->SetNextRenderSegment(segment);
				prevSegment = segment;
			}
			
			prevSegment->SetNextRenderSegment(nullptr);
		}
		
		Controller *controller = GetController();
		if (controller) controller->SetDetailLevel(level);
	}
}

StencilData *Geometry::GetStencilData(void)
{
	if (!stencilData.shadowStorage) shadowFrontArray = stencilData.Activate(true);
	return (&stencilData);
}

Link<StencilVolume> *Geometry::GetStaticStencilVolume(const Light *light)
{
	Link<StencilVolume> *empty = nullptr;
	
	for (machine a = 0; a < kMaxStaticStencilVolumeCount; a++)
	{
		Link<StencilVolume> *link = &staticStencilVolume[a];
		StencilVolume *stencilVolume = *link;
		if (stencilVolume)
		{
			if (stencilVolume->GetLight() == light) return (link);
		}
		else
		{
			empty = link;
		}
	}
	
	return (empty);
}

void Geometry::InvalidateStaticShadowVolumes(void)
{
	for (machine a = 0; a < kMaxStaticStencilVolumeCount; a++) delete staticStencilVolume[a].GetTarget();
}

void Geometry::CalculateInfiniteShadowFrontArray(const Vector3D& lightDirection)
{
	const ArrayBundle *planeBundle = GetArrayBundle(kArrayPlane);
	const Antivector4D *plane = static_cast<Antivector4D *>(planeBundle->pointer);
	bool *front = GetShadowFrontArray();
	
	int32 planeCount = planeBundle->descriptor.elementCount;
	for (machine a = 0; a < planeCount; a++) front[a] = ((plane[a] ^ lightDirection) > 0.0F);
}

void Geometry::CalculatePointShadowFrontArray(const Point3D& lightPosition)
{
	const ArrayBundle *planeBundle = GetArrayBundle(kArrayPlane);
	const Antivector4D *plane = static_cast<Antivector4D *>(planeBundle->pointer);
	bool *front = GetShadowFrontArray();
	
	int32 planeCount = planeBundle->descriptor.elementCount;
	for (machine a = 0; a < planeCount; a++) front[a] = ((plane[a] ^ lightPosition) > 0.0F);
}


MeshGeometry::MeshGeometry() : Geometry(kGeometryMesh)
{
}

MeshGeometry::MeshGeometry(const Geometry *geometry) : Geometry(kGeometryMesh)
{
	int32 materialCount = geometry->GetMaterialCount();
	SetMaterialCount(materialCount);
	for (machine a = 0; a < materialCount; a++) SetMaterialObject(a, geometry->GetMaterialObject(a));
	
	SetNewObject(new MeshGeometryObject(geometry));
}

MeshGeometry::MeshGeometry(int32 levelCount, const List<GeometrySurface> *const *surfaceList, MaterialObject *const *materialArray, const SkinData *const *skinData) : Geometry(kGeometryMesh)
{
	Array<int32> materialIndexArray(8);
	Array<MaterialObject *> materialObjectArray(8);
	
	int32 surfaceCount = 0;
	int32 materialCount = 0;
	
	const GeometrySurface *surface = surfaceList[0]->First();
	while (surface)
	{
		MaterialObject *object = materialArray[surfaceCount];
		
		int32 index = materialObjectArray.FindElement(object);
		if (index == -1)
		{
			materialObjectArray.AddElement(object);
			index = materialCount++;
		}
		
		materialIndexArray.AddElement(index);
		
		surfaceCount++;
		surface = surface->Next();
	}
	
	SetMaterialCount(materialCount);
	for (machine a = 0; a < materialCount; a++) SetMaterialObject(a, materialObjectArray[a]);
	
	SetNewObject(new MeshGeometryObject(levelCount, surfaceList, surfaceCount, materialIndexArray, skinData));
}

MeshGeometry::MeshGeometry(int32 geometryCount, const Geometry *const *geometryArray, const Transformable *transformable) : Geometry(kGeometryMesh)
{
	Array<MaterialObject *> materialArray(8);
	for (machine a = 0; a < geometryCount; a++)
	{
		const Geometry *geometry = geometryArray[a];
		int32 count = geometry->GetMaterialCount();
		for (machine b = 0; b < count; b++)
		{
			MaterialObject *object = geometry->GetMaterialObject(b);
			if (materialArray.FindElement(object) == -1) materialArray.AddElement(object);
		}
	}
	
	int32 materialCount = materialArray.GetElementCount();
	SetMaterialCount(materialCount);
	for (machine a = 0; a < materialCount; a++) SetMaterialObject(a, materialArray[a]);
	
	SetNewObject(new MeshGeometryObject(geometryCount, geometryArray, materialArray, transformable));
}

MeshGeometry::MeshGeometry(BooleanOperation operation, const Geometry *geometry1, const Geometry *geometry2) : Geometry(kGeometryMesh)
{
	Array<MaterialObject *> materialArray(8);
	
	int32 count = geometry1->GetMaterialCount();
	for (machine a = 0; a < count; a++)
	{
		MaterialObject *object = geometry1->GetMaterialObject(a);
		if (materialArray.FindElement(object) == -1) materialArray.AddElement(object);
	}
	
	count = geometry2->GetMaterialCount();
	for (machine a = 0; a < count; a++)
	{
		MaterialObject *object = geometry2->GetMaterialObject(a);
		if (materialArray.FindElement(object) == -1) materialArray.AddElement(object);
	}
	
	int32 materialCount = materialArray.GetElementCount();
	SetMaterialCount(materialCount);
	for (machine a = 0; a < materialCount; a++) SetMaterialObject(a, materialArray[a]);
	
	SetNewObject(new MeshGeometryObject(operation, geometry1, geometry2, materialArray));
	OptimizeMaterials();
}

MeshGeometry::MeshGeometry(const MeshGeometry& meshGeometry) : Geometry(meshGeometry)
{
}

MeshGeometry::~MeshGeometry()
{
}
			
Node *MeshGeometry::Replicate(void) const
{
	return (new MeshGeometry(*this));
}

void MeshGeometry::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Geometry::Pack(data, packFlags);
	
	data << TerminatorChunk;
}

void MeshGeometry::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Geometry::Unpack(data, unpackFlags);
	UnpackChunkList<MeshGeometry>(data, unpackFlags);
}

bool MeshGeometry::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	return (false);
}

void MeshGeometry::CalculatePostTransform(void)
{
	Geometry::CalculatePostTransform();
	(*postTransformProc)(this);
}

void MeshGeometry::CalculateOrientedBoundingBox(MeshGeometry *meshGeometry)
{
	const Box3D& box = meshGeometry->GetObject()->GetBoundingBox();
	const Transform4D& transform = meshGeometry->GetWorldTransform();
	meshGeometry->worldCenter = transform * box.GetCenter();
	
	const Vector3D& size = box.GetSize();
	meshGeometry->worldAxis[0] = transform[0] * (size.x * 0.5F);
	meshGeometry->worldAxis[1] = transform[1] * (size.y * 0.5F);
	meshGeometry->worldAxis[2] = transform[2] * (size.z * 0.5F);
}

bool MeshGeometry::CalculateBoundingBox(Box3D *box) const
{
	*box = GetObject()->GetBoundingBox();
	return (true);
}

bool MeshGeometry::CalculateBoundingSphere(BoundingSphere *sphere) const
{
	*sphere = *GetObject()->GetBoundingSphere();
	return (true);
}

void MeshGeometry::Preprocess(void)
{
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdatePostTransform);
	postTransformProc = &CalculateOrientedBoundingBox;
	
	SetVisibilityProc(&BoxVisible);
	SetOcclusionProc(&BoxOccluded);
	
	SetMotionBlurBox(&GetObject()->GetBoundingBox());
	
	// Call Geometry::Preprocess() last to allow controllers to override the post-transform proc,
	// the visibility and occlusion procs, or the motion blur box.
	
	Geometry::Preprocess();
}

bool MeshGeometry::BoxVisible(const Node *node, const Region *region)
{
	const MeshGeometry *geometry = static_cast<const MeshGeometry *>(node);
	return (region->BoxVisible(geometry->worldCenter, geometry->worldAxis));
}

bool MeshGeometry::BoxOccluded(const Node *node, const Region *region)
{
	const MeshGeometry *geometry = static_cast<const MeshGeometry *>(node);
	const Point3D& center = geometry->worldCenter;
	const Vector3D *axis = geometry->worldAxis;
	
	do
	{
		if (region->BoxOccluded(center, axis)) return (true);
		region = region->Next();
	} while (region);
	
	return (false);
}

// ZYURVUR
