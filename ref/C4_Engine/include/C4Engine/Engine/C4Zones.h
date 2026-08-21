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


#ifndef C4Zones_h
#define C4Zones_h


//# \component	World Manager
//# \prefix		WorldMgr/


#include "C4Portals.h"
#include "C4Regions.h"
#include "C4Lights.h"
#include "C4Sources.h"
#include "C4Effects.h"
#include "C4Instances.h"
#include "C4Models.h"
#include "C4Markers.h"
#include "C4Triggers.h"
#include "C4Physics.h"
#include "C4Fields.h"


namespace C4
{
	typedef Type	ZoneType;
	
	
	enum
	{
		kObjectZone			= 'ZONE'
	};
	
	
	//# \enum	ZoneType
	
	enum
	{
		kZoneInfinite		= 'INFT',		//## Infinite zone that covers all space.
		kZoneBox			= 'BOX ',		//## Box-shaped zone.
		kZoneCylinder		= 'CYLD',		//## Cylinder-shaped zone.
		kZoneDome			= 'DOME',		//## Dome-shaped zone.
		kZonePolygon		= 'POLY'		//## Zone whose base is a convex polygon.
	};
	
	
	enum
	{
		kMaxZoneVertexCount		= 8
	};
	
	
	//# \enum	ZoneFlags
	
	enum
	{
		kZoneRenderSkybox	= 1 << 0,
		kZoneTransition		= 1 << 1
	};
	
	
	C4API extern const char kConnectorKeyFog[];
	C4API extern const char kConnectorKeyAcoustics[];
	C4API extern const char kConnectorKeyAmbient[];
	
	
	class Texture;
	
	
	//# \class	ZoneObject	Encapsulates data for a zone.
	//
	//# The $ZoneObject$ class encapsulates data for a zone.
	//
	//# \def	class ZoneObject : public Object
	//
	//# \ctor	ZoneObject(ZoneType type);
	//
	//# The constructor has protected access. The $ZoneObject$ class can only exist as the base class for a more specific type of zone.
	//
	//# \param	type	The zone type.
	//
	//# \desc
	//#
	//
	//# \table	ZoneType
	//
	//# \base	Object		A $ZoneObject$ is an object that can be shared by multiple zone nodes.
	//
	//# \also	$@Zone@$
	//# \also	$@PortalObject@$
	
	
	//# \function	ZoneObject::GetZoneType		Returns the type of a zone.
	//
	//# \proto	ZoneType GetZoneType(void) const;
	//
	//# \desc
	//# The $GetZoneType$ function returns the type of a zone, which can be one of the following values.
	//
	//# \table	ZoneType
	
	
	//# \function	ZoneObject::GetAmbientLight		Returns the color of the ambient light for a zone.
	// 
	//# \proto	const ColorRGBA& GetAmbientLight(void) const;
	// 
	//# \desc 
	//# The $GetAmbientLight$ function returns the color of the ambient light used in a zone. 
	//# The alpha component of the color is not used.
	//# 
	//# The initial ambient light color is (0.25, 0.25, 0.25, 1.0).
	//
	//# \also	$@ZoneObject::SetAmbientLight@$
	 
	
	//# \function	ZoneObject::SetAmbientLight		Sets the color of the ambient light for a zone.
	//
	//# \proto	void SetAmbientLight(const ColorRGBA& ambient); 
	//
	//# \param	ambient		The new ambient light color.
	//
	//# \desc
	//# The $SetAmbientLight$ function sets the color of the ambient light used in a zone to that
	//# specified by the $ambient$ paramater. The alpha component of the color is not used and should be set to 1.0.
	//#
	//# The initial ambient light color is (0.25, 0.25, 0.25, 1.0).
	//
	//# \also	$@ZoneObject::GetAmbientLight@$
	
	
	class ZoneObject : public Object
	{
		friend class WorldMgr;
		
		private:
			
			ZoneType				zoneType;
			unsigned_int32			zoneFlags;
			
			ColorRGBA				ambientLight;
			
			Texture					*environmentMap;
			ResourceName			environmentName;
			
			static ZoneObject *Construct(Unpacker& data, unsigned_int32 unpackFlags);
		
		protected:
			
			ZoneObject(ZoneType type);
			~ZoneObject();
		
		public:
			
			ZoneType GetZoneType(void) const
			{
				return (zoneType);
			}
			
			unsigned_int32 GetZoneFlags(void) const
			{
				return (zoneFlags);
			}
			
			void SetZoneFlags(unsigned_int32 flags)
			{
				zoneFlags = flags;
			}
			
			const ColorRGBA& GetAmbientLight(void) const
			{
				return (ambientLight);
			}
			
			void SetAmbientLight(const ColorRGBA& ambient)
			{
				ambientLight = ambient;
			}
			
			Texture *const& GetEnvironmentMap(void) const
			{
				return (environmentMap);
			}
			
			const ResourceName& GetEnvironmentName(void) const
			{
				return (environmentName);
			}
			
			void PackType(Packer& data) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetCategoryCount(void) const;
			Type GetCategoryType(int32 index, const char **title) const;
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
			
			C4API void SetEnvironmentMap(const char *name);
			
			virtual bool ExteriorSphere(const Point3D& center, float radius) const = 0;
			virtual bool InteriorSphere(const Point3D& center, float radius) const = 0;
			virtual bool ExteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const = 0;
			virtual bool InteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const = 0;
			virtual bool IntersectRay(const Ray *ray, float *param) const = 0;
			
			virtual bool CalculateOppositePlane(const Vector3D& direction, Antivector4D *plane) const;
	};
	
	
	//# \class	InfiniteZoneObject	Encapsulates data for an infinite zone.
	//
	//# The $InfiniteZoneObject$ class encapsulates data for data for an infinite zone.
	//
	//# \def	class InfiniteZoneObject : public ZoneObject
	//
	//# \ctor	InfiniteZoneObject();
	//
	//# \desc
	//
	//# \base	ZoneObject	An $InfiniteZoneObject$ is a specific type of $ZoneObject$.
	//
	//# \also	$@InfiniteZone@$
	
	
	class InfiniteZoneObject : public ZoneObject
	{
		private:
			
			Box3D		zoneBox;
			
			~InfiniteZoneObject();
		
		public:
			
			C4API InfiniteZoneObject();
			C4API InfiniteZoneObject(const Box3D& box);
			
			const Box3D& GetZoneBox(void) const
			{
				return (zoneBox);
			}
			
			void SetZoneBox(const Box3D& box)
			{
				zoneBox = box;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetObjectSize(float *size) const;
			void SetObjectSize(const float *size);
			
			bool ExteriorSphere(const Point3D& center, float radius) const;
			bool InteriorSphere(const Point3D& center, float radius) const;
			bool ExteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const;
			bool InteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const;
			bool IntersectRay(const Ray *ray, float *param) const;
	};
	
	
	//# \class	BoxZoneObject	Encapsulates data for a box zone.
	//
	//# The $BoxZoneObject$ class encapsulates data for data for a box zone.
	//
	//# \def	class BoxZoneObject : public ZoneObject
	//
	//# \ctor	BoxZoneObject(const Vector3D& size);
	//
	//# \param	size	The size of the box.
	//
	//# \desc
	//
	//# \base	ZoneObject	A $BoxZoneObject$ is a specific type of $ZoneObject$.
	//
	//# \also	$@BoxZone@$
	
	
	class BoxZoneObject : public ZoneObject
	{
		friend class ZoneObject;
		
		private:
			
			Vector3D	boxSize;
			
			BoxZoneObject();
			~BoxZoneObject();
		
		public:
			
			BoxZoneObject(const Vector3D& size);
			
			const Vector3D& GetBoxSize(void) const
			{
				return (boxSize);
			}
			
			void SetBoxSize(const Vector3D& size)
			{
				boxSize = size;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetObjectSize(float *size) const;
			void SetObjectSize(const float *size);
			
			bool ExteriorSphere(const Point3D& center, float radius) const;
			bool InteriorSphere(const Point3D& center, float radius) const;
			bool ExteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const;
			bool InteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const;
			bool IntersectRay(const Ray *ray, float *param) const;
			
			bool CalculateOppositePlane(const Vector3D& direction, Antivector4D *plane) const;
	};
	
	
	//# \class	CylinderZoneObject	Encapsulates data for a cylinder zone.
	//
	//# The $CylinderZoneObject$ class encapsulates data for data for a cylinder zone.
	//
	//# \def	class CylinderZoneObject : public ZoneObject
	//
	//# \ctor	CylinderZoneObject(const Vector2D& size, float height);
	//
	//# \param	size	The size of the cylinder's base.
	//# \param	height	The height of the cylinder.
	//
	//# \desc
	//
	//# \base	ZoneObject	A $CylinderZoneObject$ is a specific type of $ZoneObject$.
	//
	//# \also	$@CylinderZone@$
	
	
	class CylinderZoneObject : public ZoneObject
	{
		friend class ZoneObject;
		
		private:
			
			Vector2D	cylinderSize;
			float		cylinderHeight;
			float		ratioXY;
			
			CylinderZoneObject();
			~CylinderZoneObject();
		
		public:
			
			CylinderZoneObject(const Vector2D& size, float height);
			
			const Vector2D& GetCylinderSize(void) const
			{
				return (cylinderSize);
			}
			
			void SetCylinderSize(const Vector2D& size)
			{
				cylinderSize = size;
				ratioXY = size.x / size.y;
			}
			
			float GetCylinderHeight(void) const
			{
				return (cylinderHeight);
			}
			
			void SetCylinderHeight(float height)
			{
				cylinderHeight = height;
			}
			
			float GetRatioXY(void) const
			{
				return (ratioXY);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetObjectSize(float *size) const;
			void SetObjectSize(const float *size);
			
			bool ExteriorSphere(const Point3D& center, float radius) const;
			bool InteriorSphere(const Point3D& center, float radius) const;
			bool ExteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const;
			bool InteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const;
			bool IntersectRay(const Ray *ray, float *param) const;
			
			bool CalculateOppositePlane(const Vector3D& direction, Antivector4D *plane) const;
	};
	
	
	//# \class	DomeZoneObject	Encapsulates data for a dome zone.
	//
	//# The $DomeZoneObject$ class encapsulates data for data for a dome zone.
	//
	//# \def	class DomeZoneObject : public ZoneObject
	//
	//# \ctor	DomeZoneObject(const Vector3D& size);
	//
	//# \param	size	The size of the dome.
	//
	//# \desc
	//
	//# \base	ZoneObject	A $DomeZoneObject$ is a specific type of $ZoneObject$.
	//
	//# \also	$@DomeZone@$
	
	
	class DomeZoneObject : public ZoneObject
	{
		friend class ZoneObject;
		
		private:
			
			Vector3D	domeSize;
			float		ratioXY;
			float		ratioXZ;
			
			DomeZoneObject();
			~DomeZoneObject();
		
		public:
			
			DomeZoneObject(const Vector3D& size);
			
			const Vector3D& GetDomeSize(void) const
			{
				return (domeSize);
			}
			
			void SetDomeSize(const Vector3D& size)
			{
				domeSize = size;
				ratioXY = size.x / size.y;
				ratioXZ = size.x / size.z;
			}
			
			float GetRatioXY(void) const
			{
				return (ratioXY);
			}
			
			float GetRatioXZ(void) const
			{
				return (ratioXZ);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetObjectSize(float *size) const;
			void SetObjectSize(const float *size);
			
			bool ExteriorSphere(const Point3D& center, float radius) const;
			bool InteriorSphere(const Point3D& center, float radius) const;
			bool ExteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const;
			bool InteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const;
			bool IntersectRay(const Ray *ray, float *param) const;
			
			bool CalculateOppositePlane(const Vector3D& direction, Antivector4D *plane) const;
	};
	
	
	//# \class	PolygonZoneObject	Encapsulates data for a polygon zone.
	//
	//# The $PolygonZoneObject$ class encapsulates data for data for a polygon zone.
	//
	//# \def	class PolygonZoneObject : public ZoneObject
	//
	//# \ctor	PolygonZoneObject(const Vector2D& size, float height);
	//
	//# \param	size	The dimensions of a rectangular base for the zone.
	//# \param	height	The height of the zone.
	//
	//# \desc
	//
	//# \base	ZoneObject	A $PolygonZoneObject$ is a specific type of $ZoneObject$.
	//
	//# \also	$@PolygonZone@$
	
	
	class PolygonZoneObject : public ZoneObject
	{
		friend class ZoneObject;
		
		private:
			
			float		polygonHeight;
			
			int32		vertexCount;
			Point3D		polygonVertex[kMaxZoneVertexCount];
			
			PolygonZoneObject();
			~PolygonZoneObject();
		
		public:
			
			PolygonZoneObject(const Vector2D& size, float height);
			
			float GetPolygonHeight(void) const
			{
				return (polygonHeight);
			}
			
			void SetPolygonHeight(float height)
			{
				polygonHeight = height;
			}
			
			int32 GetVertexCount(void) const
			{
				return (vertexCount);
			}
			
			void SetVertexCount(int32 count)
			{
				vertexCount = count;
			}
			
			Point3D *GetVertexArray(void)
			{
				return (polygonVertex);
			}
			
			const Point3D *GetVertexArray(void) const
			{
				return (polygonVertex);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			C4API void SetPolygonSize(const Vector2D& size, float height);
			
			int32 GetObjectSize(float *size) const;
			void SetObjectSize(const float *size);
			
			bool ExteriorSphere(const Point3D& center, float radius) const;
			bool InteriorSphere(const Point3D& center, float radius) const;
			bool ExteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const;
			bool InteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const;
			bool IntersectRay(const Ray *ray, float *param) const;
			
			bool CalculateOppositePlane(const Vector3D& direction, Antivector4D *plane) const;
	};
	
	
	//# \class	Zone	Represents a zone node in a world.
	//
	//# The $Zone$ class represents a zone node in a world.
	//
	//# \def	class Zone : public Node, public ListElement<Zone>
	//
	//# \ctor	Zone(ZoneType type);
	//
	//# The constructor has protected access. The $Zone$ class can only exist as the base class for a more specific type of zone.
	//
	//# \param	type	The zone type. See below for a list of possible types.
	//
	//# \desc
	//#
	//
	//# \table	ZoneType
	//
	//# \base	Node							A $Zone$ node is a scene graph node.
	//# \base	Utilities/ListElement<Zone>		Each zone has a list of its subzones.
	//
	//# \also	$@ZoneObject@$
	//# \also	$@Portal@$
	
	
	//# \function	Zone::GetZoneType		Returns the type of a zone.
	//
	//# \proto	ZoneType GetZoneType(void) const;
	//
	//# \desc
	//# The $GetZoneType$ function returns the type of a zone, which can be one of the following values.
	//
	//# \table	ZoneType
	
	
	class Zone : public Node, public ListElement<Zone>
	{
		friend class Node;
		
		private:
			
			ZoneType					zoneType;
			
			unsigned_int32				exclusionMask;
			Zone						*transitionMapping;
			
			FogSpace					*connectedFogSpace;
			AcousticsSpace				*connectedAcousticsSpace;
			
			AmbientEnvironment			ambientEnvironment;
			
			CellGraph					visibilityGraph;
			CellGraphSite				triggerGraph;
			CellGraphSite				fieldGraph;
			
			Site						zoneSite;
			Site						effectSite;
			
			List<Zone>					subzoneList;
			List<Portal>				portalList;
			List<Portal>				occlusionPortalList;
			List<OcclusionSpace>		occlusionSpaceList;
			List<FogSpace>				fogSpaceList;
			List<Marker>				markerList;
			List<Instance>				instanceList;
			
			Link<Node>					physicsNodeLink;
			
			List<Region>				cameraRegionList;
			List<Region>				lightRegionList;
			List<Region>				sourceRegionList;
			
			static Zone *Construct(Unpacker& data, unsigned_int32 unpackFlags);
			
			#if C4LEGACY
			
				static void FogSpaceLinkProc(Node *node, void *cookie);
				static void AcousticsSpaceLinkProc(Node *node, void *cookie);
				static void AmbientSpaceLinkProc(Node *node, void *cookie);
			
			#endif
			
			static void PhysicsNodeLinkProc(Node *node, void *cookie);
			
			void AddTransition(Zone *zone);
		
		protected:
			
			Zone(ZoneType type);
			Zone(const Zone& zone);
		
		public:
			
			virtual ~Zone();
			
			using ListElement<Zone>::Previous;
			using ListElement<Zone>::Next;
			
			ZoneType GetZoneType(void) const
			{
				return (zoneType);
			}
			
			ZoneObject *GetObject(void) const
			{
				return (static_cast<ZoneObject *>(Node::GetObject()));
			}
			
			unsigned_int32 GetExclusionMask(void) const
			{
				return (exclusionMask);
			}
			
			void SetExclusionMask(unsigned_int32 mask)
			{
				exclusionMask = mask;
			}
			
			Zone *GetTransitionMapping(void) const
			{
				return (transitionMapping);
			}
			
			FogSpace *GetConnectedFogSpace(void) const
			{
				return (connectedFogSpace);
			}
			
			AcousticsSpace *GetConnectedAcousticsSpace(void) const
			{
				return (connectedAcousticsSpace);
			}
			
			const AmbientEnvironment *GetAmbientEnvironment(void) const
			{
				return (&ambientEnvironment);
			}
			
			void AddSite(Site *site)
			{
				visibilityGraph.AddSite(site);
			}
			
			Site *GetZoneSite(void)
			{
				return (&zoneSite);
			}
			
			const Site *GetZoneSite(void) const
			{
				return (&zoneSite);
			}
			
			Site *GetEffectSite(void)
			{
				return (&effectSite);
			}
			
			const Site *GetEffectSite(void) const
			{
				return (&effectSite);
			}
			
			static Zone *GetEffectSiteZone(Site *site)
			{
				return (GetEnclosingStruct(site, Zone, effectSite));
			}
			
			Zone *GetFirstSubzone(void) const
			{
				return (subzoneList.First());
			}
			
			void AddSubzone(Zone *zone)
			{
				subzoneList.Append(zone);
			}
			
			Portal *GetFirstPortal(void) const
			{
				return (portalList.First());
			}
			
			void AddPortal(Portal *portal)
			{
				portalList.Append(portal);
			}
			
			Portal *GetFirstOcclusionPortal(void) const
			{
				return (occlusionPortalList.First());
			}
			
			void AddOcclusionPortal(Portal *portal)
			{
				occlusionPortalList.Append(portal);
			}
			
			OcclusionSpace *GetFirstOcclusionSpace(void) const
			{
				return (occlusionSpaceList.First());
			}
			
			void AddOcclusionSpace(OcclusionSpace *space)
			{
				occlusionSpaceList.Append(space);
			}
			
			FogSpace *GetFirstFogSpace(void) const
			{
				return (fogSpaceList.First());
			}
			
			void AddFogSpace(FogSpace *fogSpace)
			{
				fogSpaceList.Append(fogSpace);
			}
			
			Site *GetTriggerSite(void)
			{
				return (&triggerGraph);
			}
			
			const Site *GetTriggerSite(void) const
			{
				return (&triggerGraph);
			}
			
			void AddTrigger(Trigger *trigger)
			{
				triggerGraph.AddSite(trigger);
			}
			
			void RemoveTrigger(Trigger *trigger)
			{
				triggerGraph.RemoveSite(trigger);
			}
			
			Site *GetFieldSite(void)
			{
				return (&fieldGraph);
			}
			
			void AddField(Field *field)
			{
				fieldGraph.AddSite(field);
			}
			
			void RemoveField(Field *field)
			{
				fieldGraph.RemoveSite(field);
			}
			
			Marker *GetFirstMarker(void) const
			{
				return (markerList.First());
			}
			
			void AddMarker(Marker *marker)
			{
				markerList.Append(marker);
			}
			
			Instance *GetFirstInstance(void) const
			{
				return (instanceList.First());
			}
			
			void AddInstance(Instance *instance)
			{
				instanceList.Append(instance);
			}
			
			PhysicsNode *GetPhysicsNode(void) const
			{
				return (static_cast<PhysicsNode *>(physicsNodeLink.GetTarget()));
			}
			
			void SetPhysicsNode(PhysicsNode *physicsNode)
			{
				physicsNodeLink = physicsNode;
			}
			
			CameraRegion *GetFirstCameraRegion(void) const
			{
				return (static_cast<CameraRegion *>(cameraRegionList.First()));
			}
			
			void AddCameraRegion(CameraRegion *cameraRegion)
			{
				cameraRegionList.Append(cameraRegion);
			}
			
			LightRegion *GetFirstLightRegion(void) const
			{
				return (static_cast<LightRegion *>(lightRegionList.First()));
			}
			
			void AddLightRegion(LightRegion *lightRegion)
			{
				lightRegionList.Append(lightRegion);
			}
			
			SourceRegion *GetFirstSourceRegion(void) const
			{
				return (static_cast<SourceRegion *>(sourceRegionList.First()));
			}
			
			void AddSourceRegion(SourceRegion *sourceRegion)
			{
				sourceRegionList.Append(sourceRegion);
			}
			
			void PackType(Packer& data) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetInternalConnectorCount(void) const;
			const char *GetInternalConnectorKey(int32 index) const;
			void ProcessInternalConnectors(void);
			bool ValidConnectedNode(const ConnectorKey& key, const Node *node) const;
			C4API void SetConnectedFogSpace(FogSpace *fogSpace);
			C4API void SetConnectedAcousticsSpace(AcousticsSpace *acousticsSpace);
			C4API AmbientSpace *GetConnectedAmbientSpace(void) const;
			C4API void SetConnectedAmbientSpace(AmbientSpace *ambientSpace);
			
			void Preprocess(void);
			void Neutralize(void);
			void EnterZone(Zone *zone);
			
			C4API void InvalidateLightRegions(void) const;
			C4API void InvalidateSourceRegions(void) const;
			
			C4API void ProcessTransitions(void);
	};
	
	
	//# \class	InfiniteZone	Represents an infinite zone node in a world.
	//
	//# The $InfiniteZone$ class represents an infinite zone node in a world.
	//
	//# \def	class InfiniteZone : public Zone
	//
	//# \ctor	InfiniteZone();
	//
	//# \desc
	//#
	//
	//# \base	Zone		An $InfiniteZone$ node is a specific type of $Zone$.
	//
	//# \also	$@InfiniteZoneObject@$
	
	
	class InfiniteZone : public Zone
	{
		private:
			
			Object			*auxiliaryObject;
			
			InfiniteZone(const InfiniteZone& infiniteZone);
			
			Node *Replicate(void) const override;
			
			static void AuxiliaryObjectLinkProc(Object *object, void *cookie);
			
			bool CalculateBoundingBox(Box3D *box) const override;
		
		public:
			
			C4API InfiniteZone();
			C4API ~InfiniteZone();
			
			InfiniteZoneObject *GetObject(void) const
			{
				return (static_cast<InfiniteZoneObject *>(Node::GetObject()));
			}
			
			Object *GetAuxiliaryObject(void) const
			{
				return (auxiliaryObject);
			}
			
			void Prepack(List<Object> *linkList) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			C4API void SetAuxiliaryObject(Object *object);
			
			void Preprocess(void);
	};
	
	
	//# \class	BoxZone		Represents a box zone node in a world.
	//
	//# The $BoxZone$ class represents a box zone node in a world.
	//
	//# \def	class BoxZone : public Zone
	//
	//# \ctor	BoxZone(const Vector3D& size);
	//
	//# \param	size	The size of the box.
	//
	//# \desc
	//#
	//
	//# \base	Zone		A $BoxZone$ node is a specific type of $Zone$.
	//
	//# \also	$@BoxZoneObject@$
	
	
	class BoxZone : public Zone
	{
		friend class Zone;
		
		private:
			
			BoxZone();
			BoxZone(const BoxZone& boxZone);
			
			Node *Replicate(void) const override;
			
			bool CalculateBoundingBox(Box3D *box) const override;
		
		public:
			
			C4API BoxZone(const Vector3D& size);
			C4API ~BoxZone();
			
			BoxZoneObject *GetObject(void) const
			{
				return (static_cast<BoxZoneObject *>(Node::GetObject()));
			}
			
			void Preprocess(void);
	};
	
	
	//# \class	CylinderZone		Represents a cylinder zone node in a world.
	//
	//# The $CylinderZone$ class represents a cylinder zone node in a world.
	//
	//# \def	class CylinderZone : public Zone
	//
	//# \ctor	CylinderZone(const Vector2D& size, float height);
	//
	//# \param	size	The size of the cylinder's base.
	//# \param	height	The height of the cylinder.
	//
	//# \desc
	//#
	//
	//# \base	Zone		A $CylinderZone$ node is a specific type of $Zone$.
	//
	//# \also	$@CylinderZoneObject@$
	
	
	class CylinderZone : public Zone
	{
		friend class Zone;
		
		private:
			
			CylinderZone();
			CylinderZone(const CylinderZone& cylinderZone);
			
			Node *Replicate(void) const override;
			
			bool CalculateBoundingBox(Box3D *box) const override;
		
		public:
			
			C4API CylinderZone(const Vector2D& size, float height);
			C4API ~CylinderZone();
			
			CylinderZoneObject *GetObject(void) const
			{
				return (static_cast<CylinderZoneObject *>(Node::GetObject()));
			}
			
			void Preprocess(void);
	};
	
	
	//# \class	DomeZone		Represents a dome zone node in a world.
	//
	//# The $DomeZone$ class represents a dome zone node in a world.
	//
	//# \def	class DomeZone : public Zone
	//
	//# \ctor	DomeZone(const Vector3D& size);
	//
	//# \param	size	The size of the dome.
	//
	//# \desc
	//#
	//
	//# \base	Zone		A $DomeZone$ node is a specific type of $Zone$.
	//
	//# \also	$@DomeZoneObject@$
	
	
	class DomeZone : public Zone
	{
		friend class Zone;
		
		private:
			
			DomeZone();
			DomeZone(const DomeZone& domeZone);
			
			Node *Replicate(void) const override;
			
			bool CalculateBoundingBox(Box3D *box) const override;
		
		public:
			
			C4API DomeZone(const Vector3D& size);
			C4API ~DomeZone();
			
			DomeZoneObject *GetObject(void) const
			{
				return (static_cast<DomeZoneObject *>(Node::GetObject()));
			}
			
			void Preprocess(void);
	};
	
	
	//# \class	PolygonZone		Represents a polygon zone node in a world.
	//
	//# The $PolygonZone$ class represents a polygon zone node in a world.
	//
	//# \def	class PolygonZone : public Zone
	//
	//# \ctor	PolygonZone(const Vector2D& size, float height);
	//
	//# \param	size	The dimensions of a rectangular base for the zone.
	//# \param	height	The height of the zone.
	//
	//# \desc
	//#
	//
	//# \base	Zone		A $PolygonZone$ node is a specific type of $Zone$.
	//
	//# \also	$@PolygonZoneObject@$
	
	
	class PolygonZone : public Zone
	{
		friend class Zone;
		
		private:
			
			PolygonZone();
			PolygonZone(const PolygonZone& polygonZone);
			
			Node *Replicate(void) const override;
			
			bool CalculateBoundingBox(Box3D *box) const override;
		
		public:
			
			C4API PolygonZone(const Vector2D& size, float height);
			C4API ~PolygonZone();
			
			PolygonZoneObject *GetObject(void) const
			{
				return (static_cast<PolygonZoneObject *>(Node::GetObject()));
			}
			
			void Preprocess(void);
	};
}


#endif

// ZYURVUR
