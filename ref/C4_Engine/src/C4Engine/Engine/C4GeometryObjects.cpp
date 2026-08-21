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


#include "C4GeometryObjects.h"
#include "C4Primitives.h"
#include "C4Topology.h"
#include "C4Hull.h"
#include "C4Terrain.h"
#include "C4Water.h"
#include "C4Models.h"
#include "C4Configuration.h"


using namespace C4;


namespace
{
	enum
	{
		kBuildGeometryLevelWeld			= 1 << 0,
		kBuildGeometryLevelOptimize		= 1 << 1,
		kBuildGeometryLevelColor		= 1 << 2
	};
}


namespace C4
{	
	struct BooleanEdge : public ListElement<BooleanEdge>
	{
		unsigned_int32	validFlags;
		Point3D			endpoint[2];
		
		BooleanEdge(int32 index, const Point3D& p);
		~BooleanEdge();
	};
	
	struct BooleanLoop : public ListElement<BooleanLoop>
	{
		int32		vertexCount;
		bool		*reflex;
		bool		*active;
		Point3D		*vertex;
		
		BooleanLoop(int32 count);
		~BooleanLoop();
		
		int32 GetActiveVertexCount(void) const;
		
		int32 GetNextActiveVertex(int32 index) const;
		int32 GetPrevActiveVertex(int32 index) const;
		
		bool ClassifyVertex(int32 index, const Vector3D& normal);
		int32 GetDecompStart(int32 *finish) const;
	};
}


void C4::Reverse(CollisionOctree *octree)
{
	Reverse(&octree->collisionBounds);
	Reverse(&octree->elementCount);
	Reverse(&octree->offsetAlign);
	
	int32 count = octree->elementCount;
	unsigned_int16 *index = octree->GetIndexArray();
	for (machine a = 0; a < count; a++) Reverse(&index[a]);
	
	for (machine a = 0; a < 8; a++)
	{
		Reverse(&octree->subnodeOffset[a]);
		if (octree->subnodeOffset[a] != 0) Reverse(octree->GetSubnode(a));
	}
}


BooleanEdge::BooleanEdge(int32 index, const Point3D& p)
{
	validFlags = 1 << index;
	endpoint[index] = p;
}

BooleanEdge::~BooleanEdge()
{
}


BooleanLoop::BooleanLoop(int32 count)
{
	vertexCount = count;
	
	reflex = new bool[count * (2 + sizeof(Point3D))];
	active = reflex + count;
	vertex = reinterpret_cast<Point3D *>(active + count);
	
	for (machine a = 0; a < count; a++) active[a] = true;
}

BooleanLoop::~BooleanLoop()
{
	delete[] reflex;
}

int32 BooleanLoop::GetActiveVertexCount(void) const 
{
	int32 count = 0; 
	for (machine a = 0; a < vertexCount; a++) count += active[a]; 
	return (count); 
}
 
int32 BooleanLoop::GetNextActiveVertex(int32 index) const
{
	for (;;)
	{ 
		if (++index == vertexCount) index = 0;
		if (active[index]) return (index);
	}
} 

int32 BooleanLoop::GetPrevActiveVertex(int32 index) const
{
	for (;;)
	{
		if (--index == -1) index = vertexCount - 1;
		if (active[index]) return (index);
	}
}

bool BooleanLoop::ClassifyVertex(int32 index, const Vector3D& normal)
{
	int32 next = GetNextActiveVertex(index);
	int32 prev = GetPrevActiveVertex(index);
	
	const Point3D& p0 = vertex[prev];
	const Point3D& p1 = vertex[index];
	const Point3D& p2 = vertex[next];
	
	bool b = (normal % (p1 - p0) * (p2 - p1) < 0.0F);
	reflex[index] = b;
	return (b);
}

int32 BooleanLoop::GetDecompStart(int32 *finish) const
{
	for (machine start = 0;; start++)
	{
		for (machine a = start; a < vertexCount; a++)
		{
			if ((active[a]) && (reflex[a]))
			{
				start = a;
				goto found;
			}
		}
		
		break;
		
		found:
		int32 next = GetNextActiveVertex(start);
		if (!reflex[next])
		{
			*finish = next;
			return (start);
		}
	}
	
	return (-1);
}


GeometryObject::GeometryObject(GeometryType type) :
		Object(kObjectGeometry),
		staticVertexBuffer(kVertexBufferAttribute | kVertexBufferStatic),
		indexBuffer(kVertexBufferIndex | kVertexBufferStatic),
		staticVertexBufferObserver(this, &GeometryObject::FillStaticVertexBuffer),
		indexBufferObserver(this, &GeometryObject::FillIndexBuffer)
{
	geometryType = type;
	Initialize();
	
	geometryLevelCount = 0;
	geometryLevel = nullptr;
}

GeometryObject::GeometryObject(GeometryType type, int32 levelCount) :
		Object(kObjectGeometry),
		staticVertexBuffer(kVertexBufferAttribute | kVertexBufferStatic),
		indexBuffer(kVertexBufferIndex | kVertexBufferStatic),
		staticVertexBufferObserver(this, &GeometryObject::FillStaticVertexBuffer),
		indexBufferObserver(this, &GeometryObject::FillIndexBuffer)
{
	geometryType = type;
	Initialize();
	
	geometryLevelCount = levelCount;
	geometryLevel = new GeometryLevel[levelCount];
}

GeometryObject::~GeometryObject()
{
	if (!(geometryObjectFlags & kGeometryObjectStaticSurfaces)) delete[] surfaceData;
	
	delete[] convexHullIndexArray;
	delete[] reinterpret_cast<char *>(collisionOctree);
	delete[] geometryLevel;
}

void GeometryObject::Initialize(void)
{
	geometryFlags = 0;
	geometryEffectFlags = 0;
	
	geometryDetailBias = 0.0F;
	shaderDetailBias = 0.0F;
	
	geometryObjectFlags = 0;
	
	collisionExclusionMask = kCollisionSoundPath;
	collisionLevel = 0;
	
	surfaceCount = 0;
	surfaceData = nullptr;
	
	collisionOctree = nullptr;
	convexHullIndexArray = nullptr;
}

int32 GeometryObject::Release(void)
{
	if ((geometryObjectFlags & kGeometryObjectPrototype) && (GetReferenceCount() == 2)) ResetVertexBuffers();
	return (Object::Release());
}

GeometryObject *GeometryObject::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kGeometryMesh:
			
			return (new MeshGeometryObject);
		
		case kGeometryPrimitive:
			
			return (PrimitiveGeometryObject::Construct(++data, unpackFlags));
		
		case kGeometryTerrain:
			
			#if C4LEGACY
			
				if (data.GetVersion() < 24) return (new TerrainGeometryObject);
			
			#endif
			
			if ((++data).GetType() == 0) return (new TerrainGeometryObject);
			return (new TerrainLevelGeometryObject);
		
		case kGeometryWater:
			
			return (new WaterGeometryObject);
		
		case kGeometryHorizonWater:
			
			return (new HorizonWaterGeometryObject);
	}
	
	return (nullptr);
}

void GeometryObject::PackType(Packer& data) const
{
	Object::PackType(data);
	data << geometryType;
}

void GeometryObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	data << ChunkHeader('FLAG', 4);
	data << geometryFlags;
	
	data << ChunkHeader('EFLG', 4);
	data << geometryEffectFlags;
	
	if (geometryDetailBias != 0.0F)
	{
		data << ChunkHeader('BIAS', 4);
		data << geometryDetailBias;
	}
	
	if (shaderDetailBias != 0.0F)
	{
		data << ChunkHeader('SHDB', 4);
		data << shaderDetailBias;
	}
	
	data << ChunkHeader('CDAT', 8);
	data << collisionExclusionMask;
	data << collisionLevel;
	
	PackHandle handle = data.BeginChunk('GLEV');
	data << geometryLevelCount;
	for (machine a = 0; a < geometryLevelCount; a++) geometryLevel[a].Pack(data, packFlags);
	data.EndChunk(handle);
	
	if (surfaceData)
	{
		data << ChunkHeader('SURF', sizeof(SurfaceData) * surfaceCount + 4);
		data << surfaceCount;
		for (machine a = 0; a < surfaceCount; a++) data << surfaceData[a];
	}
	
	if (collisionOctree)
	{
		data << ChunkHeader('CTRE', 4 + collisionOctreeSize);
		data << collisionOctreeSize;
		data.WriteData(collisionOctree, collisionOctreeSize);
	}
	
	if (convexHullIndexArray)
	{
		int32 size = ((convexHullVertexCount + 1) & ~1) * 2;
		data << ChunkHeader('HULL', 4 + size);
		data << convexHullVertexCount;
		data.WriteData(convexHullIndexArray, size);
	}
	
	data << TerminatorChunk;
}

void GeometryObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<GeometryObject>(data, unpackFlags);
}

#if C4LEGACY

	static void FixBoundsCoords(CollisionOctree *octree)
	{
		float xmax = octree->collisionBounds.min.y;
		float ymin = octree->collisionBounds.min.z;
		float ymax = octree->collisionBounds.max.x;
		float zmin = octree->collisionBounds.max.y;
		octree->collisionBounds.min.y = ymin;
		octree->collisionBounds.min.z = zmin;
		octree->collisionBounds.max.x = xmax;
		octree->collisionBounds.max.y = ymax;
		
		for (machine a = 0; a < 8; a++)
		{
			if (octree->subnodeOffset[a] != 0) FixBoundsCoords(octree->GetSubnode(a));
		}
	}

#endif

bool GeometryObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'FLAG':
			
			data >> geometryFlags;
			return (true);
		
		case 'EFLG':
			
			data >> geometryEffectFlags;
			return (true);
		
		case 'BIAS':
			
			data >> geometryDetailBias;
			return (true);
		
		case 'SHDB':
			
			data >> shaderDetailBias;
			return (true);
		
		case 'CDAT':
			
			data >> collisionExclusionMask;
			data >> collisionLevel;
			return (true);
		
		case 'GLEV':
			
			data >> geometryLevelCount;
			if (geometryLevelCount != 0)
			{
				geometryLevel = new GeometryLevel[geometryLevelCount];
				for (machine a = 0; a < geometryLevelCount; a++) geometryLevel[a].Unpack(data, unpackFlags);
			}
			
			return (true);
		
		case 'SURF':
		{
			int32	count;
			
			data >> count;
			
			if (!(geometryObjectFlags & kGeometryObjectStaticSurfaces))
			{
				if (!(unpackFlags & kUnpackEditor)) break;
				
				surfaceCount = count;
				if (count != 0) surfaceData = new SurfaceData[surfaceCount];
			}
			
			for (machine a = 0; a < count; a++) data >> surfaceData[a];
			return (true);
		}
		
		case 'CTRE':
			
			data >> collisionOctreeSize;
			collisionOctree = reinterpret_cast<CollisionOctree *>(new char[collisionOctreeSize]);
			data.ReadData(collisionOctree, collisionOctreeSize);
			if (data.GetEndian() != 1) Reverse(collisionOctree);
			
			return (true);
		
		case 'HULL':
		{
			data >> convexHullVertexCount;
			int32 count = (convexHullVertexCount + 1) & ~1;
			convexHullIndexArray = new unsigned_int16[count];
			data.ReadArray(count, convexHullIndexArray);
			return (true);
		}
		
		#if C4LEGACY
		
			case 'COLL':
			{
				unsigned_int32	edgeOctreeSize, vertexOctreeSize;
				
				data >> collisionOctreeSize;
				data >> edgeOctreeSize;
				data >> vertexOctreeSize;
				
				if (collisionOctreeSize != 0)
				{
					collisionOctree = reinterpret_cast<CollisionOctree *>(new char[collisionOctreeSize]);
					data.ReadData(collisionOctree, collisionOctreeSize);
					if (data.GetEndian() != 1) Reverse(collisionOctree);
					
					if (data.GetVersion() < 28) FixBoundsCoords(collisionOctree);
				}
				
				return (false);		// Skip unused data
			}
		
		#endif
	}
	
	return (false);
}

void *GeometryObject::BeginSettingsUnpack(void)
{
	geometryDetailBias = 0.0F;
	shaderDetailBias = 0.0F;
	
	if (!(geometryObjectFlags & kGeometryObjectStaticSurfaces))
	{
		delete[] surfaceData;
		surfaceData = nullptr;
	}
	
	delete[] convexHullIndexArray;
	convexHullIndexArray = nullptr;
	
	delete[] reinterpret_cast<char *>(collisionOctree);
	collisionOctree = nullptr;
	
	delete[] geometryLevel;
	geometryLevel = nullptr;
	
	return (nullptr);
}

int32 GeometryObject::GetCategoryCount(void) const
{
	return (2);
}

Type GeometryObject::GetCategoryType(int32 index, const char **title) const
{
	if (index == 0)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kObjectGeometry));
		return (kObjectGeometry);
	}
	
	if (index == 1)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID('COLL'));
		return ('COLL');
	}
	
	return (0);
}

int32 GeometryObject::GetCategorySettingCount(Type category) const
{
	if (category == kObjectGeometry) return (17);
	if (category == 'COLL') return (14);
	return (0);
}

Setting *GeometryObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (category == kObjectGeometry)
	{
		if (index == 0)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectGeometry, 'REND'));
			return (new HeadingSetting('REND', title));
		}
		
		if (index == 1)
		{
			const char *title = table->GetString(StringID(kObjectGeometry, 'REND', 'IVIS'));
			return (new BooleanSetting('IVIS', ((geometryFlags & kGeometryInvisible) != 0), title));
		}
		
		if (index == 2)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectGeometry, 'REND', 'CUBE'));
			return (new BooleanSetting('CUBE', ((geometryFlags & kGeometryCubeLightInhibit) != 0), title));
		}
		
		if (index == 3)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectGeometry, 'REND', 'NFOG'));
			return (new BooleanSetting('NFOG', ((geometryFlags & kGeometryFogInhibit) != 0), title));
		}
		
		if (index == 4)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectGeometry, 'REND', 'AMBT'));
			return (new BooleanSetting('AMBT', ((geometryFlags & kGeometryAmbientOnly) != 0), title));
		}
		
		if (index == 5)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectGeometry, 'REND', 'EFCT'));
			return (new BooleanSetting('EFCT', ((geometryFlags & kGeometryRenderEffectPass) != 0), title));
		}
		
		if (index == 6)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectGeometry, 'REND', 'DCAL'));
			return (new BooleanSetting('DCAL', ((geometryFlags & kGeometryRenderDecal) != 0), title));
		}
		
		if (index == 7)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectGeometry, 'REND', 'RMOT'));
			return (new BooleanSetting('RMOT', ((geometryFlags & kGeometryRemotePortal) != 0), title));
		}
		
		if (index == 8)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectGeometry, 'REND', 'BLUR'));
			return (new BooleanSetting('BLUR', ((geometryFlags & kGeometryMotionBlurInhibit) != 0), title));
		}
		
		if (index == 9)
		{
			const char *title = table->GetString(StringID(kObjectGeometry, 'REND', 'NMRK'));
			return (new BooleanSetting('NMRK', ((geometryFlags & kGeometryMarkingInhibit) != 0), title));
		}
		
		if (index == 10)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectGeometry, 'DETL'));
			return (new HeadingSetting('DETL', title));
		}
		
		if (index == 11)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectGeometry, 'DETL', 'SDET'));
			return (new BooleanSetting('SDET', ((geometryFlags & kGeometryShaderDetailEnable) != 0), title));
		}
		
		if (index == 12)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectGeometry, 'DETL', 'SHDB'));
			return (new TextSetting('SHDB', shaderDetailBias, title));
		}
		
		if (index == 13)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectGeometry, 'DETL', 'BIAS'));
			return (new TextSetting('BIAS', geometryDetailBias, title));
		}
		
		if (index == 14)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectGeometry, 'GSHD'));
			return (new HeadingSetting('GSHD', title));
		}
		
		if (index == 15)
		{
			if ((geometryType != kGeometryTerrain) && (geometryType != kGeometryWater))
			{
				const char *title = table->GetString(StringID(kObjectGeometry, 'GSHD', 'STEN'));
				return (new BooleanSetting('STEN', ((geometryFlags & kGeometryShadowInhibit) == 0), title));
			}
			
			return (nullptr);
		}
		
		if (index == 16)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectGeometry, 'GSHD', 'SHDM'));
			return (new BooleanSetting('SHDM', ((geometryFlags & kGeometryRenderShadowMap) != 0), title));
		}
	}
	else if (category == 'COLL')
	{
		if (index == 0)
		{
			const char *title = table->GetString(StringID('COLL', 'COLL'));
			return (new HeadingSetting('COLL', title));
		}
		
		if (index == 1)
		{
			const char *title = table->GetString(StringID('COLL', 'COLL', 'DTCT'));
			return (new BooleanSetting('DTCT', (collisionExclusionMask != kCollisionExcludeAll), title));
		}
		
		if (index == 2)
		{
			if ((geometryObjectFlags & kGeometryObjectConvexPrimitive) == 0)
			{
				const char *title = table->GetString(StringID('COLL', 'COLL', 'HULL'));
				return (new BooleanSetting('HULL', ((geometryFlags & kGeometryConvexHull) != 0), title));
			}
		}
		
		if (index == 3)
		{
			const char *title = table->GetString(StringID('COLL', 'COLL', 'LEVL'));
			return (new IntegerSetting('LEVL', collisionLevel, title, 0, 3, 1));
		}
		
		if (index == 4)
		{
			const char *title = table->GetString(StringID('COLL', 'CFLG'));
			return (new HeadingSetting('CFLG', title));
		}
		
		if (index == 5)
		{
			const char *title = table->GetString(StringID('COLL', 'CFLG', 'BODY'));
			return (new BooleanSetting('BODY', ((collisionExclusionMask & kCollisionRigidBody) == 0), title));
		}
		
		if (index == 6)
		{
			const char *title = table->GetString(StringID('COLL', 'CFLG', 'CHAR'));
			return (new BooleanSetting('CHAR', ((collisionExclusionMask & kCollisionCharacter) == 0), title));
		}
		
		if (index == 7)
		{
			const char *title = table->GetString(StringID('COLL', 'CFLG', 'PROJ'));
			return (new BooleanSetting('PROJ', ((collisionExclusionMask & kCollisionProjectile) == 0), title));
		}
		
		if (index == 8)
		{
			const char *title = table->GetString(StringID('COLL', 'CFLG', 'VHCL'));
			return (new BooleanSetting('VHCL', ((collisionExclusionMask & kCollisionVehicle) == 0), title));
		}
		
		if (index == 9)
		{
			const char *title = table->GetString(StringID('COLL', 'CFLG', 'CAMR'));
			return (new BooleanSetting('CAMR', ((collisionExclusionMask & kCollisionCamera) == 0), title));
		}
		
		if (index == 10)
		{
			const char *title = table->GetString(StringID('COLL', 'CFLG', 'INTR'));
			return (new BooleanSetting('INTR', ((collisionExclusionMask & kCollisionInteraction) == 0), title));
		}
		
		if (index == 11)
		{
			const char *title = table->GetString(StringID('COLL', 'OFLG'));
			return (new HeadingSetting('OFLG', title));
		}
		
		if (index == 12)
		{
			const char *title = table->GetString(StringID('COLL', 'OFLG', 'SITE'));
			return (new BooleanSetting('SITE', ((collisionExclusionMask & kCollisionSightPath) == 0), title));
		}
		
		if (index == 13)
		{
			const char *title = table->GetString(StringID('COLL', 'OFLG', 'SOND'));
			return (new BooleanSetting('SOND', ((collisionExclusionMask & kCollisionSoundPath) == 0), title));
		}
	}
	
	return (nullptr);
}

void GeometryObject::SetCategorySetting(Type category, const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (category == kObjectGeometry)
	{
		if (identifier == 'IVIS')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) geometryFlags |= kGeometryInvisible;
			else geometryFlags &= ~kGeometryInvisible;
		}
		else if (identifier == 'CUBE')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) geometryFlags |= kGeometryCubeLightInhibit;
			else geometryFlags &= ~kGeometryCubeLightInhibit;
		}
		else if (identifier == 'NFOG')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) geometryFlags |= kGeometryFogInhibit;
			else geometryFlags &= ~kGeometryFogInhibit;
		}
		else if (identifier == 'AMBT')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) geometryFlags |= kGeometryAmbientOnly;
			else geometryFlags &= ~kGeometryAmbientOnly;
		}
		else if (identifier == 'EFCT')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) geometryFlags |= kGeometryRenderEffectPass;
			else geometryFlags &= ~kGeometryRenderEffectPass;
		}
		else if (identifier == 'DCAL')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) geometryFlags |= kGeometryRenderDecal;
			else geometryFlags &= ~kGeometryRenderDecal;
		}
		else if (identifier == 'RMOT')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) geometryFlags |= kGeometryRemotePortal;
			else geometryFlags &= ~kGeometryRemotePortal;
		}
		else if (identifier == 'BLUR')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) geometryFlags |= kGeometryMotionBlurInhibit;
			else geometryFlags &= ~kGeometryMotionBlurInhibit;
		}
		else if (identifier == 'NMRK')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) geometryFlags |= kGeometryMarkingInhibit;
			else geometryFlags &= ~kGeometryMarkingInhibit;
		}
		else if (identifier == 'SDET')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) geometryFlags |= kGeometryShaderDetailEnable;
			else geometryFlags &= ~kGeometryShaderDetailEnable;
		}
		else if (identifier == 'SHDB')
		{
			shaderDetailBias = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
		}
		else if (identifier == 'BIAS')
		{
			geometryDetailBias = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
		}
		else if (identifier == 'STEN')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) geometryFlags &= ~kGeometryShadowInhibit;
			else geometryFlags |= kGeometryShadowInhibit;
		}
		else if (identifier == 'SHDM')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) geometryFlags |= kGeometryRenderShadowMap;
			else geometryFlags &= ~kGeometryRenderShadowMap;
		}
	}
	else if (category == 'COLL')
	{
		if (identifier == 'DTCT')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) collisionExclusionMask &= kCollisionCharacter | kCollisionProjectile | kCollisionVehicle | kCollisionCamera | kCollisionInteraction | kCollisionSightPath | kCollisionSoundPath;
			else collisionExclusionMask = kCollisionExcludeAll;
		}
		else if (identifier == 'HULL')
		{
			if ((geometryObjectFlags & kGeometryObjectConvexPrimitive) == 0)
			{
				bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
				if (b) geometryFlags |= kGeometryConvexHull;
				else geometryFlags &= ~kGeometryConvexHull;
			}
		}
		else if (identifier == 'LEVL')
		{
			SetCollisionLevel(static_cast<const IntegerSetting *>(setting)->GetIntegerValue());
		}
		else if (identifier == 'BODY')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (!b) collisionExclusionMask |= kCollisionRigidBody;
			else if (collisionExclusionMask != kCollisionExcludeAll) collisionExclusionMask &= ~kCollisionRigidBody;
		}
		else if (identifier == 'CHAR')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (!b) collisionExclusionMask |= kCollisionCharacter;
			else if (collisionExclusionMask != kCollisionExcludeAll) collisionExclusionMask &= ~kCollisionCharacter;
		}
		else if (identifier == 'PROJ')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (!b) collisionExclusionMask |= kCollisionProjectile;
			else if (collisionExclusionMask != kCollisionExcludeAll) collisionExclusionMask &= ~kCollisionProjectile;
		}
		else if (identifier == 'VHCL')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (!b) collisionExclusionMask |= kCollisionVehicle;
			else if (collisionExclusionMask != kCollisionExcludeAll) collisionExclusionMask &= ~kCollisionVehicle;
		}
		else if (identifier == 'CAMR')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (!b) collisionExclusionMask |= kCollisionCamera;
			else if (collisionExclusionMask != kCollisionExcludeAll) collisionExclusionMask &= ~kCollisionCamera;
		}
		else if (identifier == 'INTR')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (!b) collisionExclusionMask |= kCollisionInteraction;
			else if (collisionExclusionMask != kCollisionExcludeAll) collisionExclusionMask &= ~kCollisionInteraction;
		}
		else if (identifier == 'SITE')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (!b) collisionExclusionMask |= kCollisionSightPath;
			else if (collisionExclusionMask != kCollisionExcludeAll) collisionExclusionMask &= ~kCollisionSightPath;
		}
		else if (identifier == 'SOND')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (!b) collisionExclusionMask |= kCollisionSoundPath;
			else if (collisionExclusionMask != kCollisionExcludeAll) collisionExclusionMask &= ~kCollisionSoundPath;
		}
	}
}

void GeometryObject::Preprocess(unsigned_int32 dynamicFlags)
{
	unsigned_int32 objectFlags = geometryObjectFlags;
	if (!(objectFlags & kGeometryObjectPreprocessed))
	{
		geometryObjectFlags = objectFlags | kGeometryObjectPreprocessed;
		dynamicArrayFlags = dynamicFlags;
		
		unsigned_int32 bufferSize = 0;
		unsigned_int32 maxVertexSize = 0;
		
		GeometryLevel *level = geometryLevel;
		int32 levelCount = geometryLevelCount;
		for (machine a = 0; a < levelCount; a++)
		{
			unsigned_int32 vertexSize = 0;
			
			const unsigned_int8 *arrayIndex = level->GetAttributeArrayIndex();
			int32 arrayCount = level->GetAttributeArrayCount();
			for (machine b = 0; b < arrayCount; b++)
			{
				int32 index = arrayIndex[b];
				if ((dynamicFlags & (1 << index)) == 0)
				{
					level->attributeOffset[b] = bufferSize + vertexSize;
					vertexSize += level->GetArrayDescriptor(index)->elementSize;
				}
			}
			
			bufferSize += level->GetVertexCount() * vertexSize;
			maxVertexSize = Max(maxVertexSize, vertexSize);
			level++;
		}
		
		staticVertexBuffer.Initialize(bufferSize, maxVertexSize, &staticVertexBufferObserver);
		
		level = geometryLevel;
		if (level->GetArray(kArrayFace))
		{
			unsigned_int32 indexSize = 0;
			for (machine a = 0; a < levelCount; a++)
			{
				level->faceOffset = indexSize;
				const ArrayDescriptor *desc = level->GetArrayDescriptor(kArrayFace);
				indexSize += desc->elementCount * desc->elementSize;
				level++;
			}
			
			indexBuffer.Initialize(indexSize, 0, &indexBufferObserver);
		}
	}
}

void GeometryObject::Neutralize(void)
{
	geometryObjectFlags &= ~kGeometryObjectPreprocessed;
}

void GeometryObject::ResetVertexBuffers(void)
{
	indexBuffer.Deactivate();
	staticVertexBuffer.Deactivate();
	
	geometryObjectFlags &= ~kGeometryObjectPreprocessed;
}

void GeometryObject::FillStaticVertexBuffer(VertexBuffer *vertexBuffer)
{
	unsigned_int32 *restrict buffer = static_cast<unsigned_int32 *>(vertexBuffer->BeginUpdate());
	
	int32 levelCount = geometryLevelCount;
	const GeometryLevel *level = geometryLevel;
	unsigned_int32 dynamicFlags = dynamicArrayFlags;
	
	for (machine a = 0; a < levelCount; a++)
	{
		unsigned_int8			componentCount[kMaxAttributeArrayCount];
		const unsigned_int32	*staticArray[kMaxAttributeArrayCount];
		
		int32 staticCount = 0;
		
		int32 arrayCount = level->GetAttributeArrayCount();
		const unsigned_int8 *arrayIndex = level->GetAttributeArrayIndex();
		for (machine b = 0; b < arrayCount; b++)
		{
			int32 index = arrayIndex[b];
			if ((dynamicFlags & (1 << index)) == 0)
			{
				const ArrayBundle *bundle = level->GetArrayBundle(index);
				componentCount[staticCount] = (unsigned_int8) bundle->descriptor.componentCount;
				staticArray[staticCount] = static_cast<unsigned_int32 *>(bundle->pointer);
				staticCount++;
			}
		}
		
		int32 vertexCount = level->GetVertexCount();
		for (machine b = 0; b < vertexCount; b++)
		{
			for (machine c = 0; c < staticCount; c++)
			{
				int32 count = componentCount[c];
				const unsigned_int32 *source = staticArray[c] + b * count;
				for (machine d = 0; d < count; d++) buffer[d] = source[d];
				buffer += count;
			}
		}
		
		level++;
	}
	
	vertexBuffer->EndUpdate();
}

void GeometryObject::FillIndexBuffer(VertexBuffer *indexBuffer)
{
	const GeometryLevel *level = geometryLevel;
	int32 levelCount = geometryLevelCount;
	for (machine a = 0; a < levelCount; a++)
	{
		const ArrayBundle *bundle = level->GetArrayBundle(kArrayFace);
		unsigned_int32 size = bundle->descriptor.elementCount * bundle->descriptor.elementSize;
		indexBuffer->UpdateBuffer(level->GetFaceOffset(), size, bundle->pointer);
		level++;
	}
}

void GeometryObject::SetGeometryLevelCount(int32 levelCount)
{
	delete[] geometryLevel;
	
	geometryLevelCount = levelCount;
	if (levelCount != 0) geometryLevel = new GeometryLevel[levelCount];
	else geometryLevel = nullptr;
	
	delete[] reinterpret_cast<char *>(collisionOctree);
	collisionOctree = nullptr;
	
	delete[] convexHullIndexArray;
	convexHullIndexArray = nullptr;
	
	collisionLevel = Min(collisionLevel, levelCount - 1);
}

void GeometryObject::SetSurfaceCount(int32 count)
{
	delete[] surfaceData;
	
	surfaceCount = count;
	if (count != 0) surfaceData = new SurfaceData[count];
	else surfaceData = nullptr;
}

void GeometryObject::SetStaticSurfaceData(int32 count, SurfaceData *data, bool init)
{
	surfaceCount = count;
	surfaceData = data;
	
	geometryObjectFlags |= kGeometryObjectStaticSurfaces;
	
	if (init)
	{
		for (machine a = 0; a < count; a++)
		{
			data->surfaceFlags = 0;
			data->materialIndex = 0;
			data++;
		}
	}
}

unsigned_int32 GeometryObject::GetCompressedOctreeSize(const GeometryOctree *geometryOctree)
{
	unsigned_int32 size = sizeof(CollisionOctree) + ((geometryOctree->GetIndexCount() + 1) & ~1) * 2;
	for (machine a = 0; a < 8; a++)
	{
		const GeometryOctree *node = geometryOctree->GetSubnode(a);
		if (node) size += GetCompressedOctreeSize(node);
	}
	
	return (size);
}

char *GeometryObject::CompressOctree(const GeometryOctree *geometryOctree, CollisionOctree *collisionOctree)
{
	const Point3D& center = geometryOctree->GetCenter();
	const Vector3D& size = geometryOctree->GetSize();
	
	collisionOctree->collisionBounds.min = center - size;
	collisionOctree->collisionBounds.max = center + size;
	
	int32 count = geometryOctree->GetIndexCount();
	collisionOctree->elementCount = (unsigned_int16) count;
	collisionOctree->offsetAlign = 4;
	
	const unsigned_int32 *indexArray = geometryOctree->GetIndexArray();
	unsigned_int16 *compressedArray = collisionOctree->GetIndexArray();
	for (machine a = 0; a < count; a++) compressedArray[a] = indexArray[a];
	if ((count & 1) != 0) compressedArray[count] = 0;
	
	char *nodeBase = reinterpret_cast<char *>(collisionOctree);
	char *subnodeBase = nodeBase + sizeof(CollisionOctree) + ((count + 1) & ~1) * 2;
	
	for (machine a = 0; a < 8; a++)
	{
		const GeometryOctree *node = geometryOctree->GetSubnode(a);
		if (node)
		{
			unsigned_int32 offset = (subnodeBase - nodeBase) / 4;
			if (offset < 65536)
			{
				collisionOctree->subnodeOffset[a] = (unsigned_int16) offset;
				subnodeBase = CompressOctree(node, reinterpret_cast<CollisionOctree *>(subnodeBase));
			}
			else
			{
				collisionOctree->subnodeOffset[a] = 0;
			}
		}
		else
		{
			collisionOctree->subnodeOffset[a] = 0;
		}
	}
	
	return (subnodeBase);
}

GeometryOctree *GeometryObject::BuildCollisionOctree(const GeometryLevel *level, const Box3D& boundingBox)
{
	GeometryOctree *octree = new GeometryOctree(boundingBox);
	
	const Point3D *vertexArray = level->GetArray<Point3D>(kArrayVertex);
	const Triangle *triangle = level->GetArray<Triangle>(kArrayFace);
	int32 triangleCount = level->GetFaceCount();
	
	for (machine index = 0; index < triangleCount; index++)
	{
		const Point3D& p1 = vertexArray[triangle->index[0]];
		const Point3D& p2 = vertexArray[triangle->index[1]];
		const Point3D& p3 = vertexArray[triangle->index[2]];
		
		octree->FindNodeContainingTriangle(p1, p2, p3)->AddIndex(index);
		triangle++;
	}
	
	return (octree);
}

void GeometryObject::BuildCollisionData(void)
{
	delete[] reinterpret_cast<char *>(collisionOctree);
	collisionOctree = nullptr;
	
	delete[] convexHullIndexArray;
	convexHullIndexArray = nullptr;
	
	if (collisionExclusionMask != kCollisionExcludeAll)
	{
		Box3D	boundingBox;
		
		const GeometryLevel *level = &geometryLevel[collisionLevel];
		const Point3D *vertex = level->GetArray<Point3D>(kArrayVertex);
		if ((!vertex) || (level->GetWeightData())) return;
		
		int32 vertexCount = level->GetVertexCount();
		boundingBox.Calculate(vertexCount, vertex);
		
		GeometryOctree *triangleRoot = BuildCollisionOctree(level, boundingBox);
		collisionOctreeSize = GetCompressedOctreeSize(triangleRoot);
		
		if (collisionOctreeSize != 0)
		{
			collisionOctree = reinterpret_cast<CollisionOctree *>(new char[collisionOctreeSize]);
			CompressOctree(triangleRoot, collisionOctree);
		}
		
		delete triangleRoot;
		
		if (geometryFlags & kGeometryConvexHull)
		{
			convexHullIndexArray = new unsigned_int16[(vertexCount + 1) & ~1];
			int32 count = Math::ComputeConvexHull(vertexCount, vertex, boundingBox, convexHullIndexArray);
			
			convexHullVertexCount = count;
			if ((count & 1) != 0) convexHullIndexArray[count - 1] = 0;
		}
	}
}

void GeometryObject::ScaleCollisionOctree(CollisionOctree *octree, float factor)
{
	octree->collisionBounds.Scale(factor);
	
	for (machine a = 0; a < 8; a++)
	{
		if (octree->subnodeOffset[a]) ScaleCollisionOctree(octree->GetSubnode(a), factor);
	}
}

void GeometryObject::ScaleCollisionData(float factor)
{
	if (collisionOctree) ScaleCollisionOctree(collisionOctree, factor);
}

void GeometryObject::OffsetCollisionOctree(CollisionOctree *octree, const Vector3D& dv)
{
	octree->collisionBounds.Offset(dv);
	
	for (machine a = 0; a < 8; a++)
	{
		if (octree->subnodeOffset[a]) OffsetCollisionOctree(octree->GetSubnode(a), dv);
	}
}

void GeometryObject::OffsetCollisionData(const Vector3D& dv)
{
	if (collisionOctree) OffsetCollisionOctree(collisionOctree, dv);
}

const Point3D& GeometryObject::CalculateConvexHullSupportPoint(const Point3D *vertex, const Vector3D& direction) const
{
	int32 vertexCount = convexHullVertexCount;
	const unsigned_int16 *hullIndex = convexHullIndexArray;
	
	unsigned_int32 maxIndex = hullIndex[0];
	
	#if C4SIMD
	
		float4 d = SimdLoadUnaligned(&direction.x);
		float4 maxDistance = SimdDot3D(SimdLoadUnaligned(&vertex[maxIndex].x), d);
		
		for (machine a = 1; a < vertexCount; a++)
		{
			unsigned_int32 i = hullIndex[a];
			float4 f = SimdDot3D(SimdLoadUnaligned(&vertex[i].x), d);
			if (SimdCmpgtScalar(f, maxDistance))
			{
				maxIndex = i;
				maxDistance = f;
			}
		}
	
	#else
	
		float maxDistance = vertex[maxIndex] * direction;
		
		for (machine a = 1; a < vertexCount; a++)
		{
			unsigned_int32 i = hullIndex[a];
			float f = vertex[i] * direction;
			if (f > maxDistance)
			{
				maxIndex = i;
				maxDistance = f;
			}
		}
	
	#endif
	
	return (vertex[maxIndex]);
}

void GeometryObject::CalculateConvexHullSupportPointArray(const Point3D *vertex, int32 count, const Vector3D *direction, Point3D *support) const
{
	int32 vertexCount = convexHullVertexCount;
	const unsigned_int16 *hullIndex = convexHullIndexArray;
	
	for (machine k = 0; k < count; k++)
	{
		#if C4SIMD
		
			float4 d = SimdLoadUnaligned(&direction[k].x);
			float4 maxVertex = SimdLoadUnaligned(&vertex[hullIndex[0]].x);
			float4 maxDistance = SimdDot3D(maxVertex, d);
			
			for (machine a = 1; a < vertexCount; a++)
			{
				float4 v = SimdLoadUnaligned(&vertex[hullIndex[a]].x);
				float4 f = SimdDot3D(v, d);
				float4 mask = SimdSmearX(SimdMaskCmpgt(f, maxDistance));
				maxVertex = SimdSelect(maxVertex, v, mask);
				maxDistance = SimdSelect(maxDistance, f, mask);
			}
			
			SimdStore3D(maxVertex, &support[k].x);
		
		#else
		
			const Vector3D& d = direction[k];
			unsigned_int32 maxIndex = hullIndex[0];
			float maxDistance = vertex[maxIndex] * d;
			
			for (machine a = 1; a < vertexCount; a++)
			{
				unsigned_int32 i = hullIndex[a];
				float f = vertex[i] * d;
				if (f > maxDistance)
				{
					maxIndex = i;
					maxDistance = f;
				}
			}
			
			support[k] = vertex[maxIndex];
		
		#endif
	}
}

Point3D GeometryObject::GetInitialPrimitiveSupportPoint(void) const
{
	return (Point3D(0.0F, 0.0F, 0.0F));
}

Point3D GeometryObject::CalculatePrimitiveSupportPoint(const Vector3D& direction) const
{
	return (Point3D(0.0F, 0.0F, 0.0F));
}

void GeometryObject::CalculatePrimitiveSupportPointArray(int32 count, const Vector3D *direction, Point3D *support) const
{
}

int32 GeometryObject::GetMaxCollisionLevel(void) const
{
	return (geometryLevelCount - 1);
}

bool GeometryObject::ClipSegmentToCollisionBounds(const Box3D& bounds, float radius, Point3D& p1, Point3D& p2)
{
	radius += 1.0e-3F;
	
	float dx = p2.x - p1.x;
	float xmin = bounds.min.x - radius;
	if (p1.x < xmin)
	{
		if (p2.x < xmin) return (false);
		if (Fabs(dx) > K::min_float)
		{
			float t = (xmin - p1.x) / dx;
			
			p1.x = xmin;
			dx = p2.x - xmin;
			
			p1.y += t * (p2.y - p1.y);
			p1.z += t * (p2.z - p1.z);
		}
	}
	else if (p2.x < xmin)
	{
		if (Fabs(dx) > K::min_float)
		{
			float t = (p2.x - xmin) / dx;
			
			p2.x = xmin;
			dx = xmin - p1.x;
			
			p2.y += t * (p1.y - p2.y);
			p2.z += t * (p1.z - p2.z);
		}
	}
	
	float xmax = bounds.max.x + radius;
	if (p1.x > xmax)
	{
		if (p2.x > xmax) return (false);
		if (Fabs(dx) > K::min_float)
		{
			float t = (xmax - p1.x) / dx;
			
			p1.x = xmax;
			p1.y += t * (p2.y - p1.y);
			p1.z += t * (p2.z - p1.z);
		}
	}
	else if (p2.x > xmax)
	{
		if (Fabs(dx) > K::min_float)
		{
			float t = (p2.x - xmax) / dx;
			
			p2.x = xmax;
			p2.y += t * (p1.y - p2.y);
			p2.z += t * (p1.z - p2.z);
		}
	}
	
	float dy = p2.y - p1.y;
	float ymin = bounds.min.y - radius;
	if (p1.y < ymin)
	{
		if (p2.y < ymin) return (false);
		if (Fabs(dy) > K::min_float)
		{
			float t = (ymin - p1.y) / dy;
			
			p1.y = ymin;
			dy = p2.y - ymin;
			
			p1.x += t * (p2.x - p1.x);
			p1.z += t * (p2.z - p1.z);
		}
	}
	else if (p2.y < ymin)
	{
		if (Fabs(dy) > K::min_float)
		{
			float t = (p2.y - ymin) / dy;
			
			p2.y = ymin;
			dy = ymin - p1.y;
			
			p2.x += t * (p1.x - p2.x);
			p2.z += t * (p1.z - p2.z);
		}
	}
	
	float ymax = bounds.max.y + radius;
	if (p1.y > ymax)
	{
		if (p2.y > ymax) return (false);
		if (Fabs(dy) > K::min_float)
		{
			float t = (ymax - p1.y) / dy;
			
			p1.y = ymax;
			p1.x += t * (p2.x - p1.x);
			p1.z += t * (p2.z - p1.z);
		}
	}
	else if (p2.y > ymax)
	{
		if (Fabs(dy) > K::min_float)
		{
			float t = (p2.y - ymax) / dy;
			
			p2.y = ymax;
			p2.x += t * (p1.x - p2.x);
			p2.z += t * (p1.z - p2.z);
		}
	}
	
	float dz = p2.z - p1.z;
	float zmin = bounds.min.z - radius;
	if (p1.z < zmin)
	{
		if (p2.z < zmin) return (false);
		if (Fabs(dz) > K::min_float)
		{
			float t = (zmin - p1.z) / dz;
			
			p1.z = zmin;
			dz = p2.z - zmin;
			
			p1.x += t * (p2.x - p1.x);
			p1.y += t * (p2.y - p1.y);
		}
	}
	else if (p2.z < zmin)
	{
		if (Fabs(dz) > K::min_float)
		{
			float t = (p2.z - zmin) / dz;
			
			p2.z = zmin;
			dz = zmin - p1.z;
			
			p2.x += t * (p1.x - p2.x);
			p2.y += t * (p1.y - p2.y);
		}
	}
	
	float zmax = bounds.max.z + radius;
	if (p1.z > zmax)
	{
		if (p2.z > zmax) return (false);
		if (Fabs(dz) > K::min_float)
		{
			float t = (zmax - p1.z) / dz;
			
			p1.z = zmax;
			p1.x += t * (p2.x - p1.x);
			p1.y += t * (p2.y - p1.y);
		}
	}
	else if (p2.z > zmax)
	{
		if (Fabs(dz) > K::min_float)
		{
			float t = (p2.z - zmax) / dz;
			
			p2.z = zmax;
			p2.x += t * (p1.x - p2.x);
			p2.y += t * (p1.y - p2.y);
		}
	}
	
	return (true);
}

bool GeometryObject::DetectSegmentIntersection(const CollisionOctree *octree, const GeometryLevel *level, const Bivector4D& segmentLine, const Point3D& p1, const Point3D& p2, GeometryHitData *geometryHitData)
{
	bool result = false;
	float smax = 1.0F;
	
	const Point3D *vertexArray = level->GetArray<Point3D>(kArrayVertex);
	const Triangle *triangleArray = level->GetArray<Triangle>(kArrayFace);
	
	int32 count = octree->elementCount;
	const unsigned_int16 *triangleIndex = octree->GetIndexArray();
	for (machine i = 0; i < count; i++)
	{
		unsigned_int32 index = triangleIndex[i];
		const Triangle *triangle = &triangleArray[index];
		const Point3D& v1 = vertexArray[triangle->index[0]];
		const Point3D& v2 = vertexArray[triangle->index[1]];
		const Point3D& v3 = vertexArray[triangle->index[2]];
		
		Bivector4D edgeLine1(v1, v2);
		Antivector4D plane = edgeLine1 ^ v3;
		plane.Standardize();
		
		float d1 = plane ^ p1;
		float d2 = plane ^ p2;
		
		if ((!(d1 < 0.0F)) && (d2 < 0.0F))
		{
			Bivector4D edgeLine2(v2, v3);
			Bivector4D edgeLine3(v3, v1);
		
			if ((!((segmentLine ^ edgeLine1) > 0.0F)) && (!((segmentLine ^ edgeLine2) > 0.0F)) && (!((segmentLine ^ edgeLine3) > 0.0F)))
			{
				float s = d1 / (d1 - d2);
				if (s < smax)
				{
					smax = s;
					result = true;
					
					geometryHitData->position = p1 + (p2 - p1) * s;
					geometryHitData->normal = plane.GetAntivector3D();
					geometryHitData->triangleIndex = index;
				}
			}
		}
	}
	
	const Point3D *intersectPoint = (result) ? &geometryHitData->position : &p2;
	
	for (machine a = 0; a < 8; a++)
	{
		if (octree->subnodeOffset[a] != 0)
		{
			Point3D q1 = p1;
			Point3D q2 = *intersectPoint;
			
			const CollisionOctree *suboctree = octree->GetSubnode(a);
			if (ClipSegmentToCollisionBounds(suboctree->collisionBounds, 0.0F, q1, q2))
			{
				if (DetectSegmentIntersection(suboctree, level, segmentLine, q1, q2, geometryHitData))
				{
					intersectPoint = &geometryHitData->position;
					result = true;
				}
			}
		}
	}
	
	return (result);
}

bool GeometryObject::DetectSegmentEdgeIntersection(const Bivector4D& segmentLine, const Bivector4D& edgeLine, const Point3D& p1, const Vector3D& v1, float r2, float& smax, GeometryHitData *geometryHitData)
{
	const Vector3D& dp = segmentLine.GetTangent();
	
	float w = segmentLine ^ edgeLine;
	Vector3D u = dp % edgeLine.GetTangent();
	if (w * w < u * u * r2)
	{
		float e2 = SquaredMag(edgeLine.GetTangent());
		Vector3D t = edgeLine.GetTangent() * InverseSqrt(e2);
		
		Point3D p0 = p1 - v1;
		float p0_dot_t = p0 * t;
		float dp_dot_t = dp * t;
		
		float b = p0 * dp - p0_dot_t * dp_dot_t;
		if (b < 0.0F)
		{
			float a = dp * dp - dp_dot_t * dp_dot_t;
			float c = p0 * p0 - p0_dot_t * p0_dot_t - r2;
			float D = b * b - a * c;
			if (D > 0.0F)
			{
				float s = (-b - Sqrt(D)) / a;
				if ((s > 0.0F) && (s < smax))
				{
					Point3D p = p0 + dp * s;
					float h = p * t;
					if ((h > 0.0F) && (h * h < e2))
					{
						smax = s;
						geometryHitData->position = p + v1;
						geometryHitData->normal = p - ProjectOnto(p, t);
						return (true);
					}
				}
			}
		}
	}
	
	return (false);
}

bool GeometryObject::DetectSegmentVertexIntersection(const Bivector4D& segmentLine, const Point3D& p1, const Vector3D& v1, float r2, float a, float ainv, float& smax, GeometryHitData *geometryHitData)
{
	const Vector3D& dp = segmentLine.GetTangent();
	Point3D p0 = p1 - v1;
	
	float b = p0 * dp;
	if (b < 0.0F)
	{
		float c = p0 * p0 - r2;
		float D = b * b - a * c;
		if (D > 0.0F)
		{
			float s = (-b - Sqrt(D)) * ainv;
			if ((s > 0.0F) && (s < smax))
			{
				smax = s;
				geometryHitData->position = p1 + dp * s;
				geometryHitData->normal = geometryHitData->position - v1;
				return (true);
			}
		}
	}
	
	return (false);
}

bool GeometryObject::DetectSegmentIntersection(const CollisionOctree *octree, const GeometryLevel *level, const Point3D& p1, const Point3D& p2, float radius, GeometryHitData *geometryHitData)
{
	bool result = false;
	float smax = 1.0F;
	
	const Point3D *vertexArray = level->GetArray<Point3D>(kArrayVertex);
	const Triangle *triangleArray = level->GetArray<Triangle>(kArrayFace);
	
	Bivector4D segmentLine(p1, p2);
	
	int32 count = octree->elementCount;
	const unsigned_int16 *triangleIndex = octree->GetIndexArray();
	for (machine i = 0; i < count; i++)
	{
		unsigned_int32 index = triangleIndex[i];
		const Triangle *triangle = &triangleArray[index];
		const Point3D& v1 = vertexArray[triangle->index[0]];
		const Point3D& v2 = vertexArray[triangle->index[1]];
		const Point3D& v3 = vertexArray[triangle->index[2]];
		
		Bivector4D edgeLine1(v1, v2);
		Antivector4D plane = edgeLine1 ^ v3;
		plane.Standardize();
		
		float d1 = plane ^ p1;
		float d2 = plane ^ p2;
		
		if ((Fmin(d1, d2) < radius) && (Fmax(d1, d2) > -radius))
		{
			Bivector4D edgeLine2(v2, v3);
			Bivector4D edgeLine3(v3, v1);
			
			if (!(d1 < radius))
			{
				Bivector4D line = Translate(segmentLine, plane.GetAntivector3D() * -radius);
				if ((!((line ^ edgeLine1) > 0.0F)) && (!((line ^ edgeLine2) > 0.0F)) && (!((line ^ edgeLine3) > 0.0F)))
				{
					float s = (d1 - radius) / (d1 - d2);
					if (s < smax)
					{
						smax = s;
						result = true;
						
						geometryHitData->position = p1 + segmentLine.GetTangent() * s;
						geometryHitData->normal = plane.GetAntivector3D();
						geometryHitData->triangleIndex = index;
					}
				}
			}
			
			float r2 = radius * radius;
			
			if (DetectSegmentEdgeIntersection(segmentLine, edgeLine1, p1, v1, r2, smax, geometryHitData))
			{
				result = true;
				geometryHitData->triangleIndex = index;
			}
			
			if (DetectSegmentEdgeIntersection(segmentLine, edgeLine2, p1, v2, r2, smax, geometryHitData))
			{
				result = true;
				geometryHitData->triangleIndex = index;
			}
			
			if (DetectSegmentEdgeIntersection(segmentLine, edgeLine3, p1, v3, r2, smax, geometryHitData))
			{
				result = true;
				geometryHitData->triangleIndex = index;
			}
			
			const Vector3D& dp = segmentLine.GetTangent();
			float a = dp * dp;
			float ainv = 1.0F / a;
			
			if (DetectSegmentVertexIntersection(segmentLine, p1, v1, r2, a, ainv, smax, geometryHitData))
			{
				result = true;
				geometryHitData->triangleIndex = index;
			}
			
			if (DetectSegmentVertexIntersection(segmentLine, p1, v2, r2, a, ainv, smax, geometryHitData))
			{
				result = true;
				geometryHitData->triangleIndex = index;
			}
			
			if (DetectSegmentVertexIntersection(segmentLine, p1, v3, r2, a, ainv, smax, geometryHitData))
			{
				result = true;
				geometryHitData->triangleIndex = index;
			}
		}
	}
	
	const Point3D *intersectPoint = (result) ? &geometryHitData->position : &p2;
	
	for (machine a = 0; a < 8; a++)
	{
		if (octree->subnodeOffset[a] != 0)
		{
			Point3D q1 = p1;
			Point3D q2 = *intersectPoint;
			
			const CollisionOctree *suboctree = octree->GetSubnode(a);
			if (ClipSegmentToCollisionBounds(suboctree->collisionBounds, radius, q1, q2))
			{
				if (DetectSegmentIntersection(suboctree, level, q1, q2, radius, geometryHitData))
				{
					intersectPoint = &geometryHitData->position;
					result = true;
				}
			}
		}
	}
	
	return (result);
}

bool GeometryObject::DetectCollision(const Point3D& p1, const Point3D& p2, float radius, GeometryHitData *geometryHitData) const
{
	if (collisionOctree)
	{
		Point3D q1 = p1;
		Point3D q2 = p2;
		
		if (ClipSegmentToCollisionBounds(collisionOctree->collisionBounds, radius, q1, q2))
		{
			bool	result;
			
			const GeometryLevel *level = &geometryLevel[collisionLevel];
			
			if (radius == 0.0F) result = DetectSegmentIntersection(collisionOctree, level, Bivector4D(q1, q2), q1, q2, geometryHitData);
			else result = DetectSegmentIntersection(collisionOctree, level, q1, q2, radius, geometryHitData);
			
			if (result)
			{
				Vector3D dp = p2 - p1;
				geometryHitData->param = (geometryHitData->position - p1) * dp / (dp * dp);
				return (true);
			}
		}
	}
	
	return (false);
}

bool GeometryObject::ExteriorSphere(const Point3D& center, float radius) const
{
	return (false);
}

bool GeometryObject::ExteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const
{
	return (false);
}


MeshGeometryObject::MeshGeometryObject() : GeometryObject(kGeometryMesh)
{
}

MeshGeometryObject::MeshGeometryObject(const Geometry *geometry) : GeometryObject(kGeometryMesh)
{
	const GeometryObject *object = geometry->GetObject();
	SetGeometryFlags(object->GetGeometryFlags());
	
	int32 surfaceCount = object->GetSurfaceCount();
	if (surfaceCount != 0)
	{
		SetSurfaceCount(surfaceCount);
		for (machine a = 0; a < surfaceCount; a++) *GetSurfaceData(a) = *object->GetSurfaceData(a);
	}
	
	int32 levelCount = object->GetGeometryLevelCount();
	SetGeometryLevelCount(levelCount);
	
	for (machine a = 0; a < levelCount; a++)
	{
		GeometryLevel	tempLevel[2];
		
		GeometryLevel *level = object->GetGeometryLevel(a);
		if (level->GetArray<Vector3D>(kArrayNormal))
		{
			if (level->GetArray<Point2D>(kArrayTexture0)) tempLevel[0].CopyGeometryLevel(level);
			else tempLevel[0].BuildTexcoordArray(level, geometry, this);
		}
		else
		{
			if (level->GetArray<Point2D>(kArrayTexture0))
			{
				tempLevel[0].BuildNormalArray(level);
			}
			else
			{
				tempLevel[1].BuildTexcoordArray(level, geometry, this);
				tempLevel[0].BuildNormalArray(&tempLevel[1]);
			}
		}
		
		GetGeometryLevel(a)->BuildTangentArray(&tempLevel[0]);
	}
	
	unsigned_int32 mask = object->GetCollisionExclusionMask();
	SetCollisionExclusionMask(mask);
	
	UpdateBounds();
	BuildCollisionData();
}

MeshGeometryObject::MeshGeometryObject(int32 levelCount, const List<GeometrySurface> *const *surfaceListTable, int32 surfaceCount, const Array<int32>& materialIndexArray, const SkinData *const *skinDataTable) : GeometryObject(kGeometryMesh)
{
	SetSurfaceCount(surfaceCount);
	for (machine a = 0; a < surfaceCount; a++)
	{
		SurfaceData *data = GetSurfaceData(a);
		
		data->surfaceFlags = 0;
		data->materialIndex = materialIndexArray[a];
		
		data->textureAlignData[0].alignMode = kTextureAlignNatural;
		data->textureAlignData[0].alignPlane.Set(1.0F, 0.0F, 0.0F, 0.0F);
		data->textureAlignData[1].alignMode = kTextureAlignNatural;
		data->textureAlignData[1].alignPlane.Set(0.0F, 1.0F, 0.0F, 0.0F);
	}
	
	SetGeometryLevelCount(levelCount);
	for (machine level = 0; level < levelCount; level++)
	{
		BuildGeometryLevel(level, 0, surfaceListTable[level], materialIndexArray.GetElementCount(), (skinDataTable) ? skinDataTable[level] : nullptr);
	}
	
	UpdateBounds();
	BuildCollisionData();
}

MeshGeometryObject::MeshGeometryObject(int32 geometryCount, const Geometry *const *geometryArray, const Array<MaterialObject *>& materialArray, const Transformable *transformable) : GeometryObject(kGeometryMesh)
{
	ArrayDescriptor		desc[7];
	
	const GeometryObject *object = geometryArray[0]->GetObject();
	int32 levelCount = object->GetGeometryLevelCount();
	unsigned_int32 flags = object->GetGeometryFlags();
	unsigned_int32 mask = object->GetCollisionExclusionMask();
	
	int32 boneCount = 0;
	const GeometryLevel *geometryLevel = object->GetGeometryLevel(0);
	bool skinFlag = (geometryLevel->GetWeightData() != nullptr);
	if (skinFlag)
	{
		const ArrayDescriptor *descriptor = geometryLevel->GetArrayDescriptor(kArrayInverseBindTransform);
		if (descriptor) boneCount = descriptor->elementCount;
		else skinFlag = false;
	}
	
	for (machine a = 1; a < geometryCount; a++)
	{
		object = geometryArray[a]->GetObject();
		levelCount = Max(levelCount, object->GetGeometryLevelCount());
		flags &= object->GetGeometryFlags();
		mask &= object->GetCollisionExclusionMask();
		
		if (skinFlag)
		{
			const ArrayDescriptor *descriptor = geometryLevel->GetArrayDescriptor(kArrayInverseBindTransform);
			if ((!descriptor) || (descriptor->elementCount != boneCount)) skinFlag = false;
		}
	}
	
	SetGeometryFlags(flags);
	SetCollisionExclusionMask(mask);
	
	SetGeometryLevelCount(levelCount);
	GeometryLevel *finalLevel = new GeometryLevel[levelCount];
	
	for (machine level = 0; level < levelCount; level++)
	{
		GeometryLevel	outputLevel[2];
		
		int32 vertCount = 0;
		int32 faceCount = 0;
		int32 surfCount = 0;
		unsigned_int32 weightSize = 0;
		
		for (machine a = 0; a < geometryCount; a++)
		{
			object = geometryArray[a]->GetObject();
			surfCount += Max(object->GetSurfaceCount(), 1);
			
			const GeometryLevel *inputLevel = object->GetGeometryLevel(Min(level, object->GetGeometryLevelCount() - 1));
			vertCount += inputLevel->GetVertexCount();
			faceCount += inputLevel->GetFaceCount();
			
			if (skinFlag) weightSize += inputLevel->GetWeightDataSize();
		}
		
		if (level == 0) SetSurfaceCount(surfCount);
		
		desc[0].identifier = kArrayVertex;
		desc[0].elementCount = vertCount;
		desc[0].elementSize = sizeof(Point3D);
		desc[0].componentCount = 3;
		
		desc[1].identifier = kArrayNormal;
		desc[1].elementCount = vertCount;
		desc[1].elementSize = sizeof(Vector3D);
		desc[1].componentCount = 3;
		
		desc[2].identifier = kArrayTexture0;
		desc[2].elementCount = vertCount;
		desc[2].elementSize = sizeof(Point2D);
		desc[2].componentCount = 2;
		
		desc[3].identifier = kArraySurfaceIndex;
		desc[3].elementCount = vertCount;
		desc[3].elementSize = 2;
		desc[3].componentCount = 1;
		
		desc[4].identifier = kArrayFace;
		desc[4].elementCount = faceCount;
		desc[4].elementSize = sizeof(Triangle);
		desc[4].componentCount = 1;
		
		int32 arrayCount = 5;
		if (skinFlag)
		{
			arrayCount = 7;
			
			desc[5].identifier = kArrayNodeHash;
			desc[5].elementCount = boneCount;
			desc[5].elementSize = 4;
			desc[5].componentCount = 1;
			
			desc[6].identifier = kArrayInverseBindTransform;
			desc[6].elementCount = boneCount;
			desc[6].elementSize = sizeof(Transform4D);
			desc[6].componentCount = 16;
		}
		
		outputLevel[0].AllocateStorage(vertCount, arrayCount, desc, weightSize);
		
		int32 vertOffset = 0;
		int32 faceOffset = 0;
		int32 surfOffset = 0;
		unsigned_int32 weightOffset = 0;
		
		for (machine a = 0; a < geometryCount; a++)
		{
			const Geometry *geometry = geometryArray[a];
			object = geometry->GetObject();
			
			const GeometryLevel *inputLevel = object->GetGeometryLevel(Min(level, object->GetGeometryLevelCount() - 1));
			vertCount = inputLevel->GetVertexCount();
			
			const Point3D *inputVertex = inputLevel->GetArray<Point3D>(kArrayVertex);
			const Vector3D *inputNormal = inputLevel->GetArray<Vector3D>(kArrayNormal);
			Point3D *outputVertex = outputLevel[0].GetArray<Point3D>(kArrayVertex) + vertOffset;
			Vector3D *outputNormal = outputLevel[0].GetArray<Vector3D>(kArrayNormal) + vertOffset;
			
			if (geometry == transformable)
			{
				MemoryMgr::CopyMemory(inputVertex, outputVertex, vertCount * sizeof(Point3D));
				MemoryMgr::CopyMemory(inputNormal, outputNormal, vertCount * sizeof(Vector3D));
			}
			else
			{
				Transform4D transform = transformable->GetInverseWorldTransform() * geometry->GetWorldTransform();
				Transform4D inverse = geometry->GetInverseWorldTransform() * transformable->GetWorldTransform();
				
				for (machine b = 0; b < vertCount; b++)
				{
					outputVertex[b] = transform * inputVertex[b];
					outputNormal[b] = inputNormal[b] * inverse;
				}
			}
			
			const Point2D *inputTexcoord = inputLevel->GetArray<Point2D>(kArrayTexture0);
			Point2D *outputTexcoord = outputLevel[0].GetArray<Point2D>(kArrayTexture0) + vertOffset;
			if (inputTexcoord) MemoryMgr::CopyMemory(inputTexcoord, outputTexcoord, vertCount * sizeof(Point2D));
			else MemoryMgr::ClearMemory(outputTexcoord, vertCount * sizeof(Point2D));
			
			const unsigned_int16 *inputSurfaceIndex = inputLevel->GetArray<unsigned_int16>(kArraySurfaceIndex);
			unsigned_int16 *outputSurfaceIndex = outputLevel[0].GetArray<unsigned_int16>(kArraySurfaceIndex) + vertOffset;
			
			if (inputSurfaceIndex)
			{
				for (machine b = 0; b < vertCount; b++) outputSurfaceIndex[b] = inputSurfaceIndex[b] + surfOffset;
			}
			else
			{
				for (machine b = 0; b < vertCount; b++) outputSurfaceIndex[b] = surfOffset;
			}
			
			const Triangle *inputTriangle = inputLevel->GetArray<Triangle>(kArrayFace);
			Triangle *outputTriangle = outputLevel[0].GetArray<Triangle>(kArrayFace) + faceOffset;
			
			faceCount = inputLevel->GetFaceCount();
			for (machine b = 0; b < faceCount; b++)
			{
				outputTriangle[b].Set(inputTriangle[b].index[0] + vertOffset, inputTriangle[b].index[1] + vertOffset, inputTriangle[b].index[2] + vertOffset);
			}
			
			vertOffset += vertCount;
			faceOffset += faceCount;
			surfOffset += Max(object->GetSurfaceCount(), 1);
			
			if (skinFlag)
			{
				if (a == 0)
				{
					const unsigned_int32 *inputNodeHash = inputLevel->GetArray<unsigned_int32>(kArrayNodeHash);
					unsigned_int32 *outputNodeHash = outputLevel[0].GetArray<unsigned_int32>(kArrayNodeHash);
					if (inputNodeHash) MemoryMgr::CopyMemory(inputNodeHash, outputNodeHash, boneCount * 4);
					else MemoryMgr::ClearMemory(outputNodeHash, boneCount * 4);
					
					const Transform4D *inputTransform = inputLevel->GetArray<Transform4D>(kArrayInverseBindTransform);
					Transform4D *outputTransform = outputLevel[0].GetArray<Transform4D>(kArrayInverseBindTransform);
					if (inputTransform) MemoryMgr::CopyMemory(inputTransform, outputTransform, boneCount * sizeof(Transform4D));
					else MemoryMgr::ClearMemory(outputTransform, boneCount * sizeof(Transform4D));
				}
				
				weightSize = inputLevel->GetWeightDataSize();
				MemoryMgr::CopyMemory(inputLevel->GetWeightData(), reinterpret_cast<char *>(outputLevel[0].GetWeightData()) + weightOffset, weightSize);
				
				weightOffset += weightSize;
			}
		}
		
		outputLevel[0].WeldGeometryLevel(0.001F);
		
		if ((GetGeometryFlags() & kGeometryShadowInhibit) == 0)
		{
			outputLevel[1].BuildTangentArray(&outputLevel[0]);
			outputLevel[0].BuildPlaneArray(&outputLevel[1]);
			finalLevel[level].BuildEdgeArray(&outputLevel[0]);
		}
		else
		{
			finalLevel[level].BuildTangentArray(&outputLevel[0]);
		}
	}
	
	int32 surfOffset = 0;
	for (machine a = 0; a < geometryCount; a++)
	{
		const Geometry *geometry = geometryArray[a];
		object = geometry->GetObject();
		
		int32 surfCount = object->GetSurfaceCount();
		if (surfCount != 0)
		{
			for (machine b = 0; b < surfCount; b++)
			{
				SurfaceData *outputSurfaceData = GetSurfaceData(surfOffset++);
				
				const SurfaceData *inputSurfaceData = object->GetSurfaceData(b);
				outputSurfaceData->surfaceFlags = inputSurfaceData->surfaceFlags;
				outputSurfaceData->materialIndex = MaxZero(materialArray.FindElement(geometry->GetMaterialObject(inputSurfaceData->materialIndex)));
				
				if (geometry == transformable)
				{
					outputSurfaceData->textureAlignData[0] = inputSurfaceData->textureAlignData[0];
					outputSurfaceData->textureAlignData[1] = inputSurfaceData->textureAlignData[1];
				}
				else
				{
					Transform4D inverseTransform = geometry->GetInverseWorldTransform() * transformable->GetWorldTransform();
					for (machine c = 0; c < 2; c++)
					{
						TextureAlignMode mode = inputSurfaceData->textureAlignData[c].alignMode;
						const Antivector4D& plane = inputSurfaceData->textureAlignData[c].alignPlane;
						
						outputSurfaceData->textureAlignData[c].alignMode = mode;
						if ((mode == kTextureAlignObjectPlane) || (mode == kTextureAlignGlobalObjectPlane))
						{
							outputSurfaceData->textureAlignData[c].alignPlane = plane * inverseTransform;
						}
						else
						{
							outputSurfaceData->textureAlignData[c].alignPlane = plane;
						}
					}
				}
			}
		}
		else
		{
			SurfaceData *outputSurfaceData = GetSurfaceData(surfOffset++);
			outputSurfaceData->surfaceFlags = 0;
			outputSurfaceData->materialIndex = MaxZero(materialArray.FindElement(geometry->GetMaterialObject(0)));
			
			outputSurfaceData->textureAlignData[0].alignMode = kTextureAlignNatural;
			outputSurfaceData->textureAlignData[0].alignPlane.Set(1.0F, 0.0F, 0.0F, 0.0F);
			outputSurfaceData->textureAlignData[1].alignMode = kTextureAlignNatural;
			outputSurfaceData->textureAlignData[1].alignPlane.Set(0.0F, 1.0F, 0.0F, 0.0F);
		}
	}
	
	for (machine level = 0; level < levelCount; level++) GetGeometryLevel(level)->BuildSegmentArray(&finalLevel[level], GetSurfaceCount(), GetSurfaceData());
	delete[] finalLevel;
	
	UpdateBounds();
	BuildCollisionData();
}

MeshGeometryObject::MeshGeometryObject(BooleanOperation operation, const Geometry *geometry1, const Geometry *geometry2, const Array<MaterialObject *>& materialArray) : GeometryObject(kGeometryMesh)
{
	Array<SurfaceData> surfaceData(8);
	
	const GeometryObject *object1 = geometry1->GetObject();
	const GeometryObject *object2 = geometry2->GetObject();
	
	SetGeometryFlags(object1->GetGeometryFlags() & object2->GetGeometryFlags());
	int32 levelCount = Max(object1->GetGeometryLevelCount(), object2->GetGeometryLevelCount());
	SetGeometryLevelCount(levelCount);
	
	for (machine level = 0; level < levelCount; level++)
	{
		GeometryLevel			tempLevel[2];
		List<GeometrySurface>	resultList;
		
		tempLevel[0].CopyGeometryLevel(object1->GetGeometryLevel(Min(level, object1->GetGeometryLevelCount() - 1)));
		tempLevel[1].CopyGeometryLevel(object2->GetGeometryLevel(Min(level, object2->GetGeometryLevelCount() - 1)));
		tempLevel[1].TransformGeometryLevel(geometry1->GetInverseWorldTransform() * geometry2->GetWorldTransform());
		
		if (operation == kBooleanUnion) tempLevel[1].InvertGeometryLevel();
		
		Array<SurfaceData> *surfaceDataArray = (level == 0) ? &surfaceData : nullptr;
		unsigned_int32 buildFlags = IntersectMeshes(&tempLevel[0], &tempLevel[1], &resultList, geometry1, surfaceDataArray, materialArray);
		int32 primarySurfaceCount = resultList.GetElementCount();
		
		if (operation == kBooleanUnion)
		{
			tempLevel[0].InvertGeometryLevel();
			tempLevel[1].InvertGeometryLevel();
		}
		
		buildFlags |= IntersectMeshes(&tempLevel[1], &tempLevel[0], &resultList, geometry2, surfaceDataArray, materialArray);
		
		if (level == 0)
		{
			int32 surfaceCount = surfaceData.GetElementCount();
			if (surfaceCount != 0)
			{
				Transform4D inverseTransform = geometry2->GetInverseWorldTransform() * geometry1->GetWorldTransform();
				
				SetSurfaceCount(surfaceCount);
				for (machine a = 0; a < surfaceCount; a++)
				{
					const SurfaceData *inputSurfaceData = &surfaceData[a];
					SurfaceData *outputSurfaceData = GetSurfaceData(a);
					
					if (a < primarySurfaceCount)
					{
						*GetSurfaceData(a) = *inputSurfaceData;
					}
					else
					{
						outputSurfaceData->surfaceFlags = inputSurfaceData->surfaceFlags;
						outputSurfaceData->materialIndex = inputSurfaceData->materialIndex;
						
						for (machine b = 0; b < 2; b++)
						{
							TextureAlignMode mode = inputSurfaceData->textureAlignData[b].alignMode;
							const Antivector4D& plane = inputSurfaceData->textureAlignData[b].alignPlane;
							
							outputSurfaceData->textureAlignData[b].alignMode = mode;
							if ((mode == kTextureAlignObjectPlane) || (mode == kTextureAlignGlobalObjectPlane))
							{
								outputSurfaceData->textureAlignData[b].alignPlane = plane * inverseTransform;
							}
							else
							{
								outputSurfaceData->textureAlignData[b].alignPlane = plane;
							}
						}
					}
				}
			}
		}
		
		BuildGeometryLevel(level, buildFlags | (kBuildGeometryLevelWeld | kBuildGeometryLevelOptimize), &resultList, materialArray.GetElementCount());
	}
	
	UpdateBounds();
	BuildCollisionData();
}

MeshGeometryObject::~MeshGeometryObject()
{
}

void MeshGeometryObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	GeometryObject::Pack(data, packFlags);
	
	data << ChunkHeader('BSPH', sizeof(Point3D) + 4);
	data << boundingSphere.GetCenter();
	data << boundingSphere.GetRadius();
	
	data << ChunkHeader('BBOX', sizeof(Box3D));
	data << boundingBox;
	
	data << TerminatorChunk;
}

void MeshGeometryObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	GeometryObject::Unpack(data, unpackFlags);
	
	#if C4LEGACY
	
		if (data.GetVersion() <= 41)
		{
			Point3D		center;
			float		radius;
			
			data >> center;
			data >> radius;
			
			boundingSphere.SetCenter(center);
			boundingSphere.SetRadius(radius);
		}
	
	#endif
	
	UnpackChunkList<MeshGeometryObject>(data, unpackFlags);
}

bool MeshGeometryObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'BSPH':
		{
			Point3D		center;
			float		radius;
			
			data >> center;
			data >> radius;
			
			boundingSphere.SetCenter(center);
			boundingSphere.SetRadius(radius);
			return (true);
		}
		
		case 'BBOX':
			
			data >> boundingBox;
			return (true);
	}
	
	return (false);
}

int32 MeshGeometryObject::GetObjectSize(float *size) const
{
	Vector3D boundsSize = boundingBox.max - boundingBox.min;
	size[0] = boundsSize.x;
	size[1] = boundsSize.y;
	size[2] = boundsSize.z;
	return (3);
}

void MeshGeometryObject::SetObjectSize(const float *size)
{
	Vector3D scale = boundingBox.max - boundingBox.min;
	scale.x = (scale.x > 0.0F) ? size[0] / scale.x : 1.0F;
	scale.y = (scale.y > 0.0F) ? size[1] / scale.y : 1.0F;
	scale.z = (scale.z > 0.0F) ? size[2] / scale.z : 1.0F;
	
	int32 levelCount = GetGeometryLevelCount();
	for (machine a = 0; a < levelCount; a++) GetGeometryLevel(a)->ScaleGeometryLevel(scale);
	
	boundingBox.Scale(scale);
	SetBoundingSphere((boundingBox.min + boundingBox.max) * 0.5F, Magnitude(boundingBox.max - boundingBox.min) * K::sqrt_2_over_2);
}

void MeshGeometryObject::BuildGeometryLevel(int32 level, unsigned_int32 flags, const List<GeometrySurface> *surfaceList, int32 materialCount, const SkinData *skinData)
{
	ArrayDescriptor		desc[kMaxGeometryTexcoordCount + 7];
	Point2D				*texcoord[kMaxGeometryTexcoordCount];
	GeometryLevel		outputLevel[2];
	
	int32 vertCount = 0;
	int32 faceCount = 0;
	int32 texcoordCount = 1;
	unsigned_int32 surfaceFlags = ~0;
	
	const GeometrySurface *surface = surfaceList->First();
	while (surface)
	{
		texcoordCount = Max(texcoordCount, surface->texcoordCount);
		surfaceFlags &= surface->surfaceFlags;
		
		int32 count = surface->polygonList.GetElementCount();
		vertCount += count * 3;
		faceCount += count;
		
		surface = surface->Next();
	}
	
	desc[0].identifier = kArrayVertex;
	desc[0].elementCount = vertCount;
	desc[0].elementSize = sizeof(Point3D);
	desc[0].componentCount = 3;
	
	int32 arrayCount = 1;
	if (surfaceFlags & kSurfaceValidNormals)
	{
		desc[1].identifier = kArrayNormal;
		desc[1].elementCount = vertCount;
		desc[1].elementSize = sizeof(Vector3D);
		desc[1].componentCount = 3;
		
		arrayCount = 2;
	}
	
	if (surfaceFlags & kSurfaceValidTangents)
	{
		desc[arrayCount].identifier = kArrayTangent;
		desc[arrayCount].elementCount = vertCount;
		desc[arrayCount].elementSize = sizeof(Vector4D);
		desc[arrayCount].componentCount = 4;
		arrayCount++;
	}
	
	if ((flags & kBuildGeometryLevelColor) || (surfaceFlags & kSurfaceValidColors))
	{
		desc[arrayCount].identifier = kArrayColor0;
		desc[arrayCount].elementCount = vertCount;
		desc[arrayCount].elementSize = 4;
		desc[arrayCount].componentCount = 1;
		arrayCount++;
	}
	
	for (machine a = 0; a < texcoordCount; a++)
	{
		desc[arrayCount].identifier = kArrayTexture0 + a;
		desc[arrayCount].elementCount = vertCount;
		desc[arrayCount].elementSize = sizeof(Point2D);
		desc[arrayCount].componentCount = 2;
		arrayCount++;
	}
	
	int32 surfaceCount = GetSurfaceCount();
	if (surfaceCount > 1)
	{
		desc[arrayCount].identifier = kArraySurfaceIndex;
		desc[arrayCount].elementCount = vertCount;
		desc[arrayCount].elementSize = 2;
		desc[arrayCount].componentCount = 1;
		arrayCount++;
	}
	
	unsigned_int32 weightDataSize = 0;
	if (skinData)
	{
		desc[arrayCount].identifier = kArrayNodeHash;
		desc[arrayCount].elementCount = skinData->boneCount;
		desc[arrayCount].elementSize = 4;
		desc[arrayCount].componentCount = 1;
		arrayCount++;
		
		desc[arrayCount].identifier = kArrayInverseBindTransform;
		desc[arrayCount].elementCount = skinData->boneCount;
		desc[arrayCount].elementSize = sizeof(Transform4D);
		desc[arrayCount].componentCount = 16;
		arrayCount++;
		
		surface = surfaceList->First();
		while (surface)
		{
			const GeometryPolygon *polygon = surface->polygonList.First();
			while (polygon)
			{
				const GeometryVertex *v = polygon->vertexList.First();
				for (machine a = 0; a < 3; a++)
				{
					weightDataSize += skinData->weightDataTable[v->skinIndex]->GetSize();
					v = v->Next();
				}
				
				polygon = polygon->Next();
			}
			
			surface = surface->Next();
		}
	}
	
	outputLevel[0].AllocateStorage(vertCount, arrayCount, desc, weightDataSize);
	
	int32 vertIndex = 0;
	int32 faceIndex = 0;
	int32 surfIndex = 0;
	
	Point3D *vertex = outputLevel[0].GetArray<Point3D>(kArrayVertex);
	Vector3D *normal = outputLevel[0].GetArray<Vector3D>(kArrayNormal);
	Vector4D *tangent = outputLevel[0].GetArray<Vector4D>(kArrayTangent);
	Color4C *color = outputLevel[0].GetArray<Color4C>(kArrayColor0);
	for (machine a = 0; a < texcoordCount; a++) texcoord[a] = outputLevel[0].GetArray<Point2D>(kArrayTexture0 + a);
	unsigned_int16 *surfaceIndex = outputLevel[0].GetArray<unsigned_int16>(kArraySurfaceIndex);
	WeightedVertex *weightData = outputLevel[0].GetWeightData();
	
	surface = surfaceList->First();
	while (surface)
	{
		const GeometryPolygon *polygon = surface->polygonList.First();
		while (polygon)
		{
			const GeometryVertex	*v[3];
			
			v[0] = polygon->vertexList.First();
			v[1] = v[0]->Next();
			v[2] = v[1]->Next();
			
			vertex[vertIndex] = v[0]->position;
			vertex[vertIndex + 1] = v[1]->position;
			vertex[vertIndex + 2] = v[2]->position;
			
			if (surfaceFlags & kSurfaceValidNormals)
			{
				normal[vertIndex] = v[0]->normal;
				normal[vertIndex + 1] = v[1]->normal;
				normal[vertIndex + 2] = v[2]->normal;
			}
			
			if (surfaceFlags & kSurfaceValidTangents)
			{
				tangent[vertIndex] = v[0]->tangent;
				tangent[vertIndex + 1] = v[1]->tangent;
				tangent[vertIndex + 2] = v[2]->tangent;
			}
			
			if (color)
			{
				for (machine a = 0; a < 3; a++)
				{
					const ColorRGBA& c = v[a]->color;
					color[vertIndex + a].Set((unsigned_int32) (c.red * 255.0F), (unsigned_int32) (c.green * 255.0F), (unsigned_int32) (c.blue * 255.0F), (unsigned_int32) (c.alpha * 255.0F));
				}
			}
			
			for (machine a = 0; a < texcoordCount; a++)
			{
				texcoord[a][vertIndex] = v[0]->texcoord[a];
				texcoord[a][vertIndex + 1] = v[1]->texcoord[a];
				texcoord[a][vertIndex + 2] = v[2]->texcoord[a];
			}
			
			if (surfaceCount > 1)
			{
				surfaceIndex[vertIndex] = surfIndex;
				surfaceIndex[vertIndex + 1] = surfIndex;
				surfaceIndex[vertIndex + 2] = surfIndex;
			}
			
			if (skinData)
			{
				for (machine a = 0; a < 3; a++)
				{
					const WeightedVertex *wv = skinData->weightDataTable[v[a]->skinIndex];
					
					int32 boneCount = wv->boneCount;
					weightData->boneCount = boneCount;
					
					BoneWeight *boneWeight = weightData->boneWeight;
					const BoneWeight *bw = wv->boneWeight;
					for (machine b = 0; b < boneCount; b++)
					{
						*boneWeight = *bw;
						boneWeight++;
						bw++;
					}
					
					weightData = reinterpret_cast<WeightedVertex *>(boneWeight);
				}
			}
			
			vertIndex += 3;
			faceIndex++;
			
			polygon = polygon->Next();
		}
		
		surfIndex++;
		surface = surface->Next();
	}
	
	if (skinData)
	{
		int32 boneCount = skinData->boneCount;
		MemoryMgr::CopyMemory(skinData->nodeHashArray, outputLevel[0].GetArray(kArrayNodeHash), boneCount * 4);
		MemoryMgr::CopyMemory(skinData->inverseBindTransformArray, outputLevel[0].GetArray(kArrayInverseBindTransform), boneCount * sizeof(Transform4D));
	}
	
	if (flags & kBuildGeometryLevelWeld) outputLevel[0].WeldGeometryLevel(0.001F);
	if (flags & kBuildGeometryLevelOptimize) outputLevel[0].MendGeometryLevel(0.001F, 0.001F, 0.001F);
	outputLevel[1].UnifyGeometryLevel(&outputLevel[0]);
	
	unsigned_int32 x = 1;
	if ((flags & kBuildGeometryLevelOptimize) && (surfIndex > 1))
	{
		outputLevel[0].SimplifyBoundaryEdges(&outputLevel[1]);
		x = 0;
	}
	
	if (!(surfaceFlags & kSurfaceValidNormals))
	{
		outputLevel[x ^ 1].BuildNormalArray(&outputLevel[x]);
		x ^= 1;
	}
	
	if ((GetGeometryFlags() & kGeometryShadowInhibit) == 0)
	{
		outputLevel[x ^ 1].BuildTangentArray(&outputLevel[x]);
		x ^= 1;
		
		if (!skinData)
		{
			outputLevel[x ^ 1].BuildPlaneArray(&outputLevel[x]);
			x ^= 1;
		}
		
		if (materialCount > 1)
		{
			outputLevel[x ^ 1].BuildEdgeArray(&outputLevel[x]);
			GetGeometryLevel(level)->BuildSegmentArray(&outputLevel[x ^ 1], surfaceCount, GetSurfaceData());
		}
		else
		{
			GetGeometryLevel(level)->BuildEdgeArray(&outputLevel[x]);
		}
	}
	else
	{
		if (materialCount > 1)
		{
			outputLevel[x ^ 1].BuildTangentArray(&outputLevel[x]);
			GetGeometryLevel(level)->BuildSegmentArray(&outputLevel[x ^ 1], surfaceCount, GetSurfaceData());
		}
		else
		{
			GetGeometryLevel(level)->BuildTangentArray(&outputLevel[x]);
		}
	}
}

unsigned_int32 MeshGeometryObject::IntersectMeshes(const GeometryLevel *targetLevel, const GeometryLevel *auxLevel, List<GeometrySurface> *resultList, const Geometry *targetGeometry, Array<SurfaceData> *surfaceDataArray, const Array<MaterialObject *>& materialArray)
{
	Point3D		polygonVertex[3];
	Vector3D	polygonNormal[3];
	ColorRGBA	polygonColor[3];
	Point2D		polygonTexcoord[3];
	
	float auxVolume = auxLevel->CalculateVolume();
	
	int32 faceCount = targetLevel->GetFaceCount();
	const Triangle *triangle = targetLevel->GetArray<Triangle>(kArrayFace);
	
	const Point3D *vertex = targetLevel->GetArray<Point3D>(kArrayVertex);
	const Vector3D *normal = targetLevel->GetArray<Vector3D>(kArrayNormal);
	const Point2D *texcoord = targetLevel->GetArray<Point2D>(kArrayTexture0);
	
	const Color4C *color = nullptr;
	unsigned_int32 buildFlags = 0;
	
	const ArrayBundle *colorBundle = targetLevel->GetArrayBundle(kArrayColor0);
	if ((colorBundle) && (colorBundle->descriptor.componentCount == 1))
	{
		color = targetLevel->GetArray<Color4C>(kArrayColor0);
		buildFlags = kBuildGeometryLevelColor;
	}
	else
	{
		for (machine a = 0; a < 3; a++) polygonColor[a].Set(0.0F, 0.0F, 0.0F, 0.0F);
	}
	
	const unsigned_int16 *surfaceIndex = targetLevel->GetArray<unsigned_int16>(kArraySurfaceIndex);
	if (surfaceIndex)
	{
		for (machine faceIndex = 0; faceIndex < faceCount;)
		{
			unsigned_int32 inputSurfaceIndex = surfaceIndex[triangle->index[0]];
			
			GeometrySurface *surface = new GeometrySurface;
			surface->surfaceFlags = kSurfaceValidNormals;
			resultList->Append(surface);
			
			for (; faceIndex < faceCount; faceIndex++)
			{
				if (surfaceIndex[triangle->index[0]] != inputSurfaceIndex) break;
				
				if (color)
				{
					for (machine a = 0; a < 3; a++)
					{
						unsigned_int32 i = triangle->index[a];
						polygonVertex[a] = vertex[i];
						polygonNormal[a] = normal[i];
						polygonTexcoord[a] = texcoord[i];
						
						const Color4C& c = color[i];
						polygonColor[a].Set((float) c.GetRed() * K::one_over_255, (float) c.GetGreen() * K::one_over_255, (float) c.GetBlue() * K::one_over_255, (float) c.GetAlpha() * K::one_over_255);
					}
				}
				else
				{
					for (machine a = 0; a < 3; a++)
					{
						unsigned_int32 i = triangle->index[a];
						polygonVertex[a] = vertex[i];
						polygonNormal[a] = normal[i];
						polygonTexcoord[a] = texcoord[i];
					}
				}
				
				IntersectPolygonAndMesh(polygonVertex, polygonNormal, polygonColor, polygonTexcoord, auxLevel, auxVolume, &surface->polygonList);
				triangle++;
			}
			
			if (surfaceDataArray)
			{
				int32 surfaceCount = surfaceDataArray->GetElementCount();
				surfaceDataArray->SetElementCount(surfaceCount + 1);
				
				SurfaceData *outputSurfaceData = &(*surfaceDataArray)[surfaceCount];
				const SurfaceData *inputSurfaceData = targetGeometry->GetObject()->GetSurfaceData(inputSurfaceIndex);
				
				outputSurfaceData->surfaceFlags = inputSurfaceData->surfaceFlags;
				outputSurfaceData->materialIndex = MaxZero(materialArray.FindElement(targetGeometry->GetMaterialObject(inputSurfaceData->materialIndex)));
				
				outputSurfaceData->textureAlignData[0] = inputSurfaceData->textureAlignData[0];
				outputSurfaceData->textureAlignData[1] = inputSurfaceData->textureAlignData[1];
			}
		}
	}
	else
	{
		GeometrySurface *surface = new GeometrySurface;
		surface->surfaceFlags = kSurfaceValidNormals;
		resultList->Append(surface);
		
		for (machine faceIndex = 0; faceIndex < faceCount; faceIndex++)
		{
			if (color)
			{
				for (machine a = 0; a < 3; a++)
				{
					unsigned_int32 i = triangle->index[a];
					polygonVertex[a] = vertex[i];
					polygonNormal[a] = normal[i];
					polygonTexcoord[a] = texcoord[i];
					
					const Color4C& c = color[i];
					polygonColor[a].Set((float) c.GetRed() * K::one_over_255, (float) c.GetGreen() * K::one_over_255, (float) c.GetBlue() * K::one_over_255, (float) c.GetAlpha() * K::one_over_255);
				}
			}
			else
			{
				for (machine a = 0; a < 3; a++)
				{
					unsigned_int32 i = triangle->index[a];
					polygonVertex[a] = vertex[i];
					polygonNormal[a] = normal[i];
					polygonTexcoord[a] = texcoord[i];
				}
			}
			
			IntersectPolygonAndMesh(polygonVertex, polygonNormal, polygonColor, polygonTexcoord, auxLevel, auxVolume, &surface->polygonList);
			triangle++;
		}
		
		if (surfaceDataArray)
		{
			int32 surfaceCount = surfaceDataArray->GetElementCount();
			surfaceDataArray->SetElementCount(surfaceCount + 1);
			
			SurfaceData *outputSurfaceData = &(*surfaceDataArray)[surfaceCount];
			outputSurfaceData->surfaceFlags = 0;
			outputSurfaceData->materialIndex = MaxZero(materialArray.FindElement(targetGeometry->GetMaterialObject(0)));
			
			outputSurfaceData->textureAlignData[0].alignMode = kTextureAlignNatural;
			outputSurfaceData->textureAlignData[0].alignPlane.Set(1.0F, 0.0F, 0.0F, 0.0F);
			outputSurfaceData->textureAlignData[1].alignMode = kTextureAlignNatural;
			outputSurfaceData->textureAlignData[1].alignPlane.Set(0.0F, 1.0F, 0.0F, 0.0F);
		}
	}
	
	return (buildFlags);
}

void MeshGeometryObject::IntersectPolygonAndMesh(const Point3D *polygonVertex, const Vector3D *polygonNormal, const ColorRGBA *polygonColor, const Point2D *polygonTexcoord, const GeometryLevel *geometryLevel, float geometryVolume, List<GeometryPolygon> *resultList)
{
	List<BooleanLoop>		positiveLoopList;
	List<BooleanLoop>		negativeLoopList;
	List<GeometryPolygon>	inputList[2];
	int32					vertexCount[2];
	const Point3D			*vertexTable[2];
	
	Antivector4D plane(polygonVertex[0], polygonVertex[1], polygonVertex[2]);
	plane.Standardize();
	ConstructBooleanLoops(plane, geometryLevel, &positiveLoopList, &negativeLoopList);
	
	int32 positiveLoopCount = positiveLoopList.GetElementCount();
	int32 negativeLoopCount = negativeLoopList.GetElementCount();
	
	if (positiveLoopCount + negativeLoopCount == 0)
	{
		if (geometryVolume < 0.0F)
		{
			GeometryPolygon *gp = new GeometryPolygon;
			resultList->Append(gp);
			
			for (machine a = 0; a < 3; a++)
			{
				GeometryVertex *gv = new GeometryVertex;
				gp->vertexList.Append(gv);
				
				gv->position = polygonVertex[a];
				gv->normal = polygonNormal[a];
				gv->color = polygonColor[a];
				gv->texcoord[0] = polygonTexcoord[a];
			}
		}
		
		return;
	}
	
	if (positiveLoopCount != 0)
	{
		int32 maxLoopVertexCount = 0;
		const BooleanLoop *loop = positiveLoopList.First();
		do
		{
			maxLoopVertexCount = Max(maxLoopVertexCount, loop->vertexCount);
			loop = loop->Next();
		} while (loop);
		
		int32 resultCount = 3 + maxLoopVertexCount;
		
		Buffer buffer(resultCount * (sizeof(Point3D) + sizeof(Vector3D) + sizeof(ColorRGBA) + sizeof(Point2D)) + (resultCount - 2) * sizeof(Triangle));
		Point3D *resultVertex = static_cast<Point3D *>(*buffer);
		Vector3D *resultNormal = reinterpret_cast<Vector3D *>(resultVertex + resultCount);
		ColorRGBA *resultColor = reinterpret_cast<ColorRGBA *>(resultNormal + resultCount);
		Point2D *resultTexcoord = reinterpret_cast<Point2D *>(resultColor + resultCount);
		Triangle *resultTriangle = reinterpret_cast<Triangle *>(resultTexcoord + resultCount);
		
		vertexCount[0] = 3;
		vertexTable[0] = polygonVertex;
		
		loop = positiveLoopList.First();
		do
		{
			vertexCount[1] = loop->vertexCount;
			vertexTable[1] = loop->vertex;
			
			Math::IntersectConvexPolygons(vertexCount, vertexTable, plane.GetAntivector3D(), &resultCount, resultVertex);
			if (resultCount >= 3)
			{
				CalculatePolygonAttributes(polygonVertex, polygonNormal, polygonColor, polygonTexcoord, resultCount, resultVertex, resultNormal, resultColor, resultTexcoord);
				int32 triangleCount = Math::TriangulatePolygon(resultCount, resultVertex, plane.GetAntivector3D(), resultTriangle);
				
				for (machine a = 0; a < triangleCount; a++)
				{
					const Triangle *t = &resultTriangle[a];
					
					GeometryPolygon *gp = new GeometryPolygon;
					for (machine b = 0; b < 3; b++)
					{
						int32 index = t->index[b];
						
						GeometryVertex *gv = new GeometryVertex;
						gv->position = resultVertex[index];
						gv->normal = resultNormal[index];
						gv->color = resultColor[index];
						gv->texcoord[0] = resultTexcoord[index];
						gp->vertexList.Append(gv);
					}
					
					inputList[0].Append(gp);
				}
			}
			
			loop = loop->Next();
		} while (loop);
	}
	
	int32 parity = 0;
	
	if (negativeLoopCount != 0)
	{
		if (geometryVolume < 0.0F)
		{
			for (;;)
			{
				GeometryPolygon *gp = inputList[0].First();
				if (!gp) break;
				
				resultList->Append(gp);
			}
			
			GeometryPolygon *gp = new GeometryPolygon;
			inputList[0].Append(gp);
			
			for (machine a = 0; a < 3; a++)
			{
				GeometryVertex *gv = new GeometryVertex;
				gp->vertexList.Append(gv);
				
				gv->position = polygonVertex[a];
				gv->normal = polygonNormal[a];
				gv->color = polygonColor[a];
				gv->texcoord[0] = polygonTexcoord[a];
			}
		}
		
		const BooleanLoop *loop = negativeLoopList.First();
		do
		{
			int32 subtractCount = loop->vertexCount;
			vertexCount[1] = subtractCount;
			vertexTable[1] = loop->vertex;
			
			for (;;)
			{
				GeometryPolygon *inputPolygon = inputList[parity].First();
				if (!inputPolygon) break;
				
				int32 positiveCount = inputPolygon->vertexList.GetElementCount();
				int32 triangleCount = positiveCount + subtractCount;
				int32 resultCount = triangleCount + subtractCount;
				
				Buffer buffer((positiveCount + resultCount) * sizeof(Point3D) + resultCount * (sizeof(Vector3D) + sizeof(ColorRGBA) + sizeof(Point2D)) + triangleCount * sizeof(Triangle));
				Point3D *positiveVertex = static_cast<Point3D *>(*buffer);
				Point3D *resultVertex = positiveVertex + positiveCount;
				Vector3D *resultNormal = reinterpret_cast<Vector3D *>(resultVertex + resultCount);
				ColorRGBA *resultColor = reinterpret_cast<ColorRGBA *>(resultNormal + resultCount);
				Point2D *resultTexcoord = reinterpret_cast<Point2D *>(resultColor + resultCount);
				Triangle *resultTriangle = reinterpret_cast<Triangle *>(resultTexcoord + resultCount);
				
				const GeometryVertex *gv = inputPolygon->vertexList.First();
				for (machine a = 0; a < positiveCount; a++)
				{
					positiveVertex[a] = gv->position;
					gv = gv->Next();
				}
				
				vertexCount[0] = positiveCount;
				vertexTable[0] = positiveVertex;
				
				if (Math::SubtractConvexPolygons(vertexCount, vertexTable, plane.GetAntivector3D(), &resultCount, &triangleCount, resultVertex, resultTriangle))
				{
					CalculatePolygonAttributes(polygonVertex, polygonNormal, polygonColor, polygonTexcoord, resultCount, resultVertex, resultNormal, resultColor, resultTexcoord);
					
					for (machine a = 0; a < triangleCount; a++)
					{
						const Triangle *t = &resultTriangle[a];
						
						GeometryPolygon *gp = new GeometryPolygon;
						for (machine b = 0; b < 3; b++)
						{
							int32 index = t->index[b];
							
							GeometryVertex *gv = new GeometryVertex;
							gv->position = resultVertex[index];
							gv->normal = resultNormal[index];
							gv->color = resultColor[index];
							gv->texcoord[0] = resultTexcoord[index];
							gp->vertexList.Append(gv);
						}
						
						inputList[parity ^ 1].Append(gp);
					}
					
					delete inputPolygon;
				}
				else
				{
					inputList[parity ^ 1].Append(inputPolygon);
				}
			}
			
			parity ^= 1;
			loop = loop->Next();
		} while (loop);
	}
	
	for (;;)
	{
		GeometryPolygon *gp = inputList[parity].First();
		if (!gp) break;
		
		resultList->Append(gp);
	}
}

void MeshGeometryObject::ConstructBooleanLoops(const Antivector4D& plane, const GeometryLevel *geometryLevel, List<BooleanLoop> *positiveList, List<BooleanLoop> *negativeList)
{
	List<BooleanEdge>	edgeList;
	
	int32 faceCount = geometryLevel->GetFaceCount();
	const Triangle *triangle = geometryLevel->GetArray<Triangle>(kArrayFace);
	const Point3D *vertex = geometryLevel->GetArray<Point3D>(kArrayVertex);
	
	for (machine a = 0; a < faceCount; a++)
	{
		BooleanEdge *edge = nullptr;
		
		unsigned_int32 index1 = triangle->index[2];
		for (machine b = 0; b < 3; b++)
		{
			unsigned_int32 index2 = triangle->index[b];
			
			const Point3D& p1 = vertex[index1];
			const Point3D& p2 = vertex[index2];
			
			float d1 = plane ^ p1;
			float d2 = plane ^ p2;
			
			int32 c1 = (d1 > 0.0F) ? 1 : -1;
			int32 c2 = (d2 > 0.0F) ? 1 : -1;
			
			if (c1 > c2)
			{
				float t = d2 / (d1 - d2);
				Point3D q = p2 - (p1 - p2) * t;
				
				if (edge)
				{
					if ((edge->validFlags & 1) == 0)
					{
						edge->validFlags = 3;
						edge->endpoint[0] = q;
						break;
					}
				}
				else
				{
					edge = new BooleanEdge(0, q);
					edgeList.Append(edge);
				}
			}
			else if (c2 > c1)
			{
				float t = d1 / (d2 - d1);
				Point3D q = p1 - (p2 - p1) * t;
				
				if (edge)
				{
					if ((edge->validFlags & 2) == 0)
					{
						edge->validFlags = 3;
						edge->endpoint[1] = q;
						break;
					}
				}
				else
				{
					edge = new BooleanEdge(1, q);
					edgeList.Append(edge);
				}
			}
			
			index1 = index2;
		}
		
		if (edge)
		{
			if (edge->validFlags != 3)
			{
				delete edge;
			}
			else
			{
				const BooleanEdge *be = edgeList.First();
				while (be != edge)
				{
					float dp1 = SquaredMag(be->endpoint[0] - edge->endpoint[0]);
					float dp2 = SquaredMag(be->endpoint[1] - edge->endpoint[1]);
					if ((dp1 < kWeldEpsilonSquared) && (dp2 < kWeldEpsilonSquared))
					{
						delete edge;
						break;
					}
					
					be = be->Next();
				}
			}
		}
		
		triangle++;
	}
	
	for (;;)
	{
		List<BooleanEdge>	loopEdgeList;
		
		BooleanEdge *edge = edgeList.First();
		if (!edge) break;
		
		loopEdgeList.Append(edge);
		int32 edgeCount = 1;
		
		for (;;)
		{
			BooleanEdge *next = edgeList.First();
			while (next)
			{
				if (SquaredMag(next->endpoint[0] - edge->endpoint[1]) < kWeldEpsilonSquared)
				{
					edge = next;
					loopEdgeList.Append(next);
					edgeCount++;
					break;
				}
				
				next = next->Next();
			}
			
			if (!next) break;
		}
		
		if (edgeCount < 3) continue;
		
		BooleanEdge *e3 = loopEdgeList.First();
		BooleanEdge *e2 = loopEdgeList.Last();
		BooleanEdge *e1 = e2->Previous();
		do
		{
			Vector3D dp1 = e2->endpoint[0] - e1->endpoint[0];
			Vector3D dp2 = e3->endpoint[0] - e2->endpoint[0];
			if (dp1 * dp2 * InverseMag(dp1) * InverseMag(dp2) > kCollinearEdgeEpsilon)
			{
				delete e2;
				edgeCount--;
			}
			else
			{
				e1 = e2;
			}
			
			e2 = e3;
			e3 = e3->Next();
		} while (e3);
		
		if (edgeCount < 3) continue;
		
		BooleanLoop *loop = new BooleanLoop(edgeCount);
		
		edge = loopEdgeList.First();
		for (machine a = 0; a < edgeCount; a++)
		{
			loop->vertex[a] = edge->endpoint[0];
			edge = edge->Next();
		}
		
		const Vector3D& normal = plane.GetAntivector3D();
		if (Math::GetPolygonArea(edgeCount, loop->vertex, normal) >= 0.0F) ConvexDecomposeLoop(normal, loop, positiveList);
		else ConvexDecomposeLoop(-normal, loop, negativeList);
	}
}

void MeshGeometryObject::ConvexDecomposeLoop(const Vector3D& normal, BooleanLoop *inputLoop, List<BooleanLoop> *outputList)
{
	int32 reflexCount = 0;
	int32 vertexCount = inputLoop->vertexCount;
	for (machine a = 0; a < vertexCount; a++) reflexCount += inputLoop->ClassifyVertex(a, normal);
	
	if (reflexCount == 0)
	{
		outputList->Append(inputLoop);
		return;
	}
	
	const Point3D *vertex = inputLoop->vertex;
	
	for (;;)
	{
		int32	finish;
		
		int32 start = inputLoop->GetDecompStart(&finish);
		if (start == -1)
		{
			int32 outputCount = inputLoop->GetActiveVertexCount();
			BooleanLoop *outputLoop = new BooleanLoop(outputCount);
			outputList->Append(outputLoop);
			
			outputCount = 0;
			for (machine a = 0; a < vertexCount; a++)
			{
				if (inputLoop->active[a]) outputLoop->vertex[outputCount++] = vertex[a];
			}
			
			break;
		}
		
		int32 startNext = finish;
		int32 outputCount = 2;
		
		for (;;)
		{
			int32 finishPrev = finish;
			finish = inputLoop->GetNextActiveVertex(finish);
			if (inputLoop->reflex[finish])
			{
				outputCount++;
				break;
			}
			
			const Point3D& p0 = vertex[finishPrev];
			const Point3D& p1 = vertex[finish];
			const Point3D& p2 = vertex[start];
			const Point3D& p3 = vertex[startNext];
			
			if ((normal % (p1 - p0) * (p2 - p1) < 0.0F) || (normal % (p2 - p1) * (p3 - p2) < 0.0F))
			{
				finish = finishPrev;
				break;
			}
			
			outputCount++;
		}
		
		if (outputCount >= 3)
		{
			BooleanLoop *outputLoop = new BooleanLoop(outputCount);
			outputList->Append(outputLoop);
			
			outputCount = 0;
			for (machine a = start;;)
			{
				outputLoop->vertex[outputCount++] = vertex[a];
				if (a == finish) break;
				
				if (a != start) inputLoop->active[a] = false;
				a = inputLoop->GetNextActiveVertex(a);
			}
		}
		else
		{
			inputLoop->active[finish] = false;
		}
		
		inputLoop->ClassifyVertex(start, normal);
		inputLoop->ClassifyVertex(finish, normal);
	}
	
	delete inputLoop;
}

void MeshGeometryObject::CalculatePolygonAttributes(const Point3D *polygonVertex, const Vector3D *polygonNormal, const ColorRGBA *polygonColor, const Point2D *polygonTexcoord, int32 vertexCount, const Point3D *vertex, Vector3D *normal, ColorRGBA *color, Point2D *texcoord)
{
	for (machine a = 0; a < vertexCount; a++)
	{
		float	w1, w2, w3;
		
		const Point3D& p = vertex[a];
		for (machine b = 0; b < 3; b++)
		{
			if (polygonVertex[b] == p)
			{
				normal[a] = polygonNormal[b];
				color[a] = polygonColor[b];
				texcoord[a] = polygonTexcoord[b];
				goto next;
			}
		}
		
		Math::CalculateBarycentricCoordinates(polygonVertex[0], polygonVertex[1], polygonVertex[2], p, &w1, &w2, &w3);
		
		normal[a] = (polygonNormal[0] * w1 + polygonNormal[1] * w2 + polygonNormal[2] * w3).Normalize();
		color[a] = polygonColor[0] * w1 + polygonColor[1] * w2 + polygonColor[2] * w3;
		texcoord[a] = polygonTexcoord[0] * w1 + polygonTexcoord[1] * w2 + polygonTexcoord[2] * w3;
		
		next:;
	}
}

bool MeshGeometryObject::ExteriorSphere(const Point3D& center, float radius) const
{
	return (boundingBox.ExteriorSphere(center, radius));
}

bool MeshGeometryObject::ExteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const
{
	return (boundingBox.ExteriorSweptSphere(p1, p2, radius));
}

void MeshGeometryObject::UpdateBounds(void)
{
	const GeometryLevel *geometryLevel = GetGeometryLevel(0);
	int32 vertexCount = geometryLevel->GetVertexCount();
	const Point3D *vertex = geometryLevel->GetArray<Point3D>(kArrayVertex);
	
	BoundingSphere *sphere = GetBoundingSphere();
	sphere->Calculate(vertexCount, vertex);
	boundingBox.Calculate(vertexCount, vertex);
}

void MeshGeometryObject::Rebuild(const Geometry *geometry)
{
	const Controller *controller = geometry->GetController();
	bool skin = ((controller) && (controller->GetControllerType() == kControllerSkin));
	
	int32 levelCount = GetGeometryLevelCount();
	for (machine level = 0; level < levelCount; level++)
	{
		GeometryLevel	tempLevel[2];
		
		GeometryLevel *geometryLevel = GetGeometryLevel(level);
		if ((!skin) && (geometryLevel->GetWeightData()))
		{
			if ((GetGeometryFlags() & kGeometryShadowInhibit) != 0)
			{
				tempLevel[0].CopyRigidGeometryLevel(geometryLevel, (1 << kArrayEdge) | (1 << kArrayPlane) | (1 << kArrayPlaneIndex));
				geometryLevel->CopyRigidGeometryLevel(&tempLevel[0]);
			}
			else
			{
				if ((!geometryLevel->GetArray(kArrayEdge)) || (!geometryLevel->GetArray(kArrayPlane)) || (!geometryLevel->GetArray(kArrayPlaneIndex)))
				{
					tempLevel[0].CopyRigidGeometryLevel(geometryLevel, (1 << kArrayEdge) | (1 << kArrayPlane) | (1 << kArrayPlaneIndex));
					tempLevel[1].BuildPlaneArray(&tempLevel[0]);
					geometryLevel->BuildEdgeArray(&tempLevel[1]);
				}
				else
				{
					tempLevel[0].CopyRigidGeometryLevel(geometryLevel);
					geometryLevel->CopyRigidGeometryLevel(&tempLevel[0]);
				}
			}
		}
		else
		{
			if ((GetGeometryFlags() & kGeometryShadowInhibit) != 0)
			{
				if ((geometryLevel->GetArray(kArrayEdge)) || (geometryLevel->GetArray(kArrayPlane)) || (geometryLevel->GetArray(kArrayPlaneIndex)))
				{
					tempLevel[0].CopyGeometryLevel(geometryLevel, (1 << kArrayEdge) | (1 << kArrayPlane) | (1 << kArrayPlaneIndex));
					geometryLevel->CopyGeometryLevel(&tempLevel[0]);
				}
			}
			else
			{
				if (skin)
				{
					if (!geometryLevel->GetArray(kArrayEdge))
					{
						tempLevel[0].CopyGeometryLevel(geometryLevel, (1 << kArrayEdge) | (1 << kArrayPlane) | (1 << kArrayPlaneIndex));
						geometryLevel->BuildEdgeArray(&tempLevel[0]);
					}
				}
				else
				{
					if ((!geometryLevel->GetArray(kArrayEdge)) || (!geometryLevel->GetArray(kArrayPlane)) || (!geometryLevel->GetArray(kArrayPlaneIndex)))
					{
						tempLevel[0].CopyGeometryLevel(geometryLevel, (1 << kArrayEdge) | (1 << kArrayPlane) | (1 << kArrayPlaneIndex));
						tempLevel[1].BuildPlaneArray(&tempLevel[0]);
						geometryLevel->BuildEdgeArray(&tempLevel[1]);
					}
				}
			}
		}
		
		geometryLevel->CalculateTangentArray();
	}
}

// ZYURVUR
