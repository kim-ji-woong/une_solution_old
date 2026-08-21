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


#ifndef C4Regions_h
#define C4Regions_h


//# \component	World Manager
//# \prefix		WorldMgr/


#include "C4Geometries.h"
#include "C4Impostors.h"
#include "C4Effects.h"


namespace C4
{
	const float kMinPortalClipDistance = 0.2F;
	
	
	enum
	{
		kMaxRegionNonlateralPlaneCount	= 3,
		kMaxPortalVertexCount			= 12,
		kMaxRegionPlaneCount			= kMaxPortalVertexCount + kMaxRegionNonlateralPlaneCount
	};
	
	
	enum
	{
		kLightRegionBoundaryCalculated	= 1 << 0,
		kLightRegionShadowsRendered		= 1 << 1
	};
	
	
	class Light;
	class Camera;
	class Source;
	class Portal;
	
	
	//# \class	Region		Represents a convex region of space.
	//
	//# The $Region$ class represents a convex region of space.
	//
	//# \def	class Region : public Tree<Region>, public Memory<Region>
	//
	//# \ctor	Region();
	//
	//# \desc
	//# 
	//
	//# \base	Utilities/Tree<Region>		Used internally by the World Manager.
	//# \base	MemoryMgr/Memory<Region>	Regions are allocated in a dedicated heap.
	
	
	//# \function	Region::PolygonVisible		Determines whether a polygon is visible in a region.
	//
	//# \proto	bool PolygonVisible(int32 vertexCount, const Point3D *vertex) const;
	//
	//# \param	vertexCount		The number of vertices in the polygon.
	//# \param	vertex			A pointer to an array containing the polygon's world-space vertex positions.
	//
	//# \desc
	//# 
	//
	//# \also	$@Region::SphereVisible@$
	//# \also	$@Region::EllipsoidVisible@$
	//# \also	$@Region::BoxVisible@$
	//# \also	$@Region::CylinderVisible@$
	
	
	//# \function	Region::SphereVisible		Determines whether a sphere is visible in a region.
	//
	//# \proto	bool SphereVisible(const Point3D& center, float radius) const;
	//
	//# \param	center		The world-space center of the sphere.
	//# \param	radius		The radius of the sphere.
	//
	//# \desc
	//# 
	//
	//# \also	$@Region::PolygonVisible@$
	//# \also	$@Region::EllipsoidVisible@$
	//# \also	$@Region::BoxVisible@$
	//# \also	$@Region::CylinderVisible@$
	
	
	//# \function	Region::EllipsoidVisible		Determines whether an ellipsoid is visible in a region.
	//
	//# \proto	bool EllipsoidVisible(const Point3D& center, const Vector3D *axis) const;
	//
	//# \param	center		The world-space center of the ellipsoid.
	//# \param	axis		A pointer to an array containing the three semi-axis vectors of the ellipsoid.
	//
	//# \desc
	//# 
	//
	//# \also	$@Region::PolygonVisible@$
	//# \also	$@Region::SphereVisible@$
	//# \also	$@Region::BoxVisible@$
	//# \also	$@Region::CylinderVisible@$
	
	 
	//# \function	Region::BoxVisible		Determines whether a box is visible in a region.
	// 
	//# \proto	bool BoxVisible(const Point3D& center, const Vector3D *axis) const; 
	// 
	//# \param	center		The world-space center of the box.
	//# \param	axis		A pointer to an array containing the three semi-axis vectors of the box. 
	//
	//# \desc
	//# 
	// 
	//# \also	$@Region::PolygonVisible@$
	//# \also	$@Region::SphereVisible@$
	//# \also	$@Region::EllipsoidVisible@$
	//# \also	$@Region::CylinderVisible@$ 
	
	
	//# \function	Region::CylinderVisible		Determines whether a cylinder is visible in a region.
	//
	//# \proto	bool CylinderVisible(const Point3D& p1, const Point3D& p2, float radius) const;
	//
	//# \param	p1			The world-space center of one end of the cylinder.
	//# \param	p2			The world-space center of the other end of the cylinder.
	//# \param	radius		The radius of the cylinder.
	//
	//# \desc
	//# 
	//
	//# \also	$@Region::PolygonVisible@$
	//# \also	$@Region::SphereVisible@$
	//# \also	$@Region::EllipsoidVisible@$
	//# \also	$@Region::BoxVisible@$
	
	
	//# \div
	//# \function	Region::PolygonOccluded		Determines whether a polygon is occluded in a region.
	//
	//# \proto	bool PolygonOccluded(int32 vertexCount, const Point3D *vertex) const;
	//
	//# \param	vertexCount		The number of vertices in the polygon.
	//# \param	vertex			A pointer to an array containing the polygon's world-space vertex positions.
	//
	//# \desc
	//# 
	//
	//# \also	$@Region::SphereOccluded@$
	//# \also	$@Region::EllipsoidOccluded@$
	//# \also	$@Region::BoxOccluded@$
	//# \also	$@Region::CylinderOccluded@$
	
	
	//# \function	Region::SphereOccluded		Determines whether a sphere is occluded in a region.
	//
	//# \proto	bool SphereOccluded(const Point3D& center, float radius) const;
	//
	//# \param	center		The world-space center of the sphere.
	//# \param	radius		The radius of the sphere.
	//
	//# \desc
	//# 
	//
	//# \also	$@Region::PolygonOccluded@$
	//# \also	$@Region::EllipsoidOccluded@$
	//# \also	$@Region::BoxOccluded@$
	//# \also	$@Region::CylinderOccluded@$
	
	
	//# \function	Region::EllipsoidOccluded		Determines whether an ellipsoid is occluded in a region.
	//
	//# \proto	bool EllipsoidOccluded(const Point3D& center, const Vector3D *axis) const;
	//
	//# \param	center		The world-space center of the ellipsoid.
	//# \param	axis		A pointer to an array containing the three semi-axis vectors of the ellipsoid.
	//
	//# \desc
	//# 
	//
	//# \also	$@Region::PolygonOccluded@$
	//# \also	$@Region::SphereOccluded@$
	//# \also	$@Region::BoxOccluded@$
	//# \also	$@Region::CylinderOccluded@$
	
	
	//# \function	Region::BoxOccluded		Determines whether a box is occluded in a region.
	//
	//# \proto	bool BoxOccluded(const Point3D& center, const Vector3D *axis) const;
	//
	//# \param	center		The world-space center of the box.
	//# \param	axis		A pointer to an array containing the three semi-axis vectors of the box.
	//
	//# \desc
	//# 
	//
	//# \also	$@Region::PolygonOccluded@$
	//# \also	$@Region::SphereOccluded@$
	//# \also	$@Region::EllipsoidOccluded@$
	//# \also	$@Region::CylinderOccluded@$
	
	
	//# \function	Region::CylinderOccluded		Determines whether a cylinder is occluded in a region.
	//
	//# \proto	bool CylinderOccluded(const Point3D& p1, const Point3D& p2, float radius) const;
	//
	//# \param	p1			The world-space center of one end of the cylinder.
	//# \param	p2			The world-space center of the other end of the cylinder.
	//# \param	radius		The radius of the cylinder.
	//
	//# \desc
	//# 
	//
	//# \also	$@Region::PolygonOccluded@$
	//# \also	$@Region::SphereOccluded@$
	//# \also	$@Region::EllipsoidOccluded@$
	//# \also	$@Region::BoxOccluded@$
	
	
	class Region : public ListElement<Region>, public Memory<Region>
	{
		private:
			
			int32			planeCount;
			Antivector4D	regionPlane[kMaxRegionPlaneCount];
		
		public:
			
			C4API Region();
			C4API virtual ~Region();
			
			int32 GetPlaneCount(void) const
			{
				return (planeCount);
			}
			
			void SetPlaneCount(int32 count)
			{
				Assert(count <= kMaxRegionPlaneCount);
				planeCount = count;
			}
			
			Antivector4D *GetPlaneArray(void)
			{
				return (regionPlane);
			}
			
			const Antivector4D *GetPlaneArray(void) const
			{
				return (regionPlane);
			}
			
			C4API bool BoxVisible(const Box3D& box) const;
			C4API bool BoxVisible(const Point3D& center, const Vector3D *axis) const;
			C4API bool BoxVisible(const Point3D& center, const Vector3D& size) const;
			C4API bool DirectionVisible(const Vector3D& center, float radius) const;
			C4API bool PolygonVisible(int32 vertexCount, const Point3D *vertex) const;
			C4API bool SphereVisible(const Point3D& center, float radius) const;
			C4API bool EllipsoidVisible(const Point3D& center, const Vector3D *axis) const;
			C4API bool CylinderVisible(const Point3D& p1, const Point3D& p2, float radius) const;
			
			C4API bool BoxOccluded(const Box3D& box) const;
			C4API bool BoxOccluded(const Point3D& center, const Vector3D *axis) const;
			C4API bool BoxOccluded(const Point3D& center, const Vector3D& size) const;
			C4API bool DirectionOccluded(const Vector3D& center, float radius) const;
			C4API bool PolygonOccluded(int32 vertexCount, const Point3D *vertex) const;
			C4API bool SphereOccluded(const Point3D& center, float radius) const;
			C4API bool EllipsoidOccluded(const Point3D& center, const Vector3D *axis) const;
			C4API bool CylinderOccluded(const Point3D& p1, const Point3D& p2, float radius) const;
	};
	
	
	template <class type> class RegionReference : public Reference<type>, public Memory<Region>
	{
		public:
			
			RegionReference(type *region) : Reference<type>(region) {}
			~RegionReference() {}
	};
	
	
	class ShadowRegion : public Region
	{
		private:
			
			const Light		*regionLight;
		
		public:
			
			ShadowRegion();
			~ShadowRegion();
			
			const Light *GetLight(void) const
			{
				return (regionLight);
			}
			
			void SetLight(const Light *light)
			{
				regionLight = light;
			}
	};
	
	
	class ZoneRegion : public Region
	{
		private:
			
			Zone		*regionZone;
		
		public:
			
			C4API ZoneRegion(Zone *zone = nullptr);
			C4API ZoneRegion(Zone *zone, const ZoneRegion *region);
			C4API ~ZoneRegion();
			
			Zone *GetZone(void) const
			{
				return (regionZone);
			}
	};
	
	
	class CameraRegion : public ZoneRegion, public Tree<CameraRegion>
	{
		private:
			
			const Camera			*regionCamera;
			
			List<Geometry>			geometryList;
			List<Effect>			effectList;
			Array<Impostor *>		impostorArray;
			
			int32					nonlateralPlaneCount;
			int32					portalExcludePlaneCount;
			unsigned_int32			shadowRegionFlags;
			
			ShadowRegion			shadowRegion;
		
		public:
			
			CameraRegion(const Camera *camera, Zone *zone = nullptr);
			CameraRegion(const Camera *camera, Zone *zone, const CameraRegion *region);
			~CameraRegion();
			
			CameraRegion *GetPreviousCameraRegion(void) const
			{
				return (static_cast<CameraRegion *>(ListElement<Region>::Previous()));
			}
			
			CameraRegion *GetNextCameraRegion(void) const
			{
				return (static_cast<CameraRegion *>(ListElement<Region>::Next()));
			}
			
			const Camera *GetCamera(void) const
			{
				return (regionCamera);
			}
			
			Geometry *GetFirstGeometry(void) const
			{
				return (geometryList.First());
			}
			
			void AddGeometry(Geometry *geometry)
			{
				geometryList.Append(geometry);
			}
			
			Effect *GetFirstEffect(void) const
			{
				return (effectList.First());
			}
			
			void AddEffect(Effect *effect)
			{
				effectList.Append(effect);
			}
			
			int32 GetImpostorCount(void) const
			{
				return (impostorArray.GetElementCount());
			}
			
			Impostor *GetImpostor(int32 index) const
			{
				return (impostorArray[index]);
			}
			
			void AddImpostor(Impostor *impostor)
			{
				impostorArray.AddElement(impostor);
			}
			
			int32 GetNonlateralPlaneCount(void) const
			{
				return (nonlateralPlaneCount);
			}
			
			void SetNonlateralPlaneCount(int32 count)
			{
				nonlateralPlaneCount = count;
			}
			
			int32 GetPortalExcludePlaneCount(void) const
			{
				return (portalExcludePlaneCount);
			}
			
			void SetPortalExcludePlaneCount(int32 count)
			{
				portalExcludePlaneCount = count;
			}
			
			unsigned_int32 GetShadowRegionFlags(void) const
			{
				return (shadowRegionFlags);
			}
			
			void SetShadowRegionFlags(unsigned_int32 flags)
			{
				shadowRegionFlags = flags;
			}
			
			ShadowRegion *GetShadowRegion(void)
			{
				return (&shadowRegion);
			}
			
			void SetFrustumPortalPlanes(int32 vertexCount, const Point3D *vertex, const Antivector4D& portalPlane);
			void SetFrustumOcclusionPortalPlanes(int32 vertexCount, const Point3D *vertex, int32 frontPlaneCount, const Antivector4D *frontPlane);
			void SetOrthoPortalPlanes(int32 vertexCount, const Point3D *vertex, const Antivector4D& portalPlane);
			void SetOrthoPortalPlanes(int32 vertexCount, const Point3D *vertex, const CameraRegion *rootRegion);
			
			bool AddOppositePlane(void);
			
			bool ContainsInfiniteLight(const Vector3D& direction) const;
			bool ContainsPointLight(const Point3D& position) const;
	};
	
	
	class LightRegion : public ZoneRegion, public Tree<LightRegion>
	{
		private:
			
			unsigned_int32		lightRegionFlags;
			
			Light				*regionLight;
			const Portal		*illuminatedPortal;
			
			int32				boundaryPolygonCount;
			int32				boundaryPolygonVertexCount[kMaxPortalVertexCount + 2];
			Point3D				boundaryPolygonVertexArray[kMaxPortalVertexCount * 6];
		
		public:
			
			LightRegion(Light *light, Zone *zone);
			LightRegion(Light *light, Zone *zone, const LightRegion *region);
			~LightRegion();
			
			LightRegion *GetPreviousLightRegion(void) const
			{
				return (static_cast<LightRegion *>(ListElement<Region>::Previous()));
			}
			
			LightRegion *GetNextLightRegion(void) const
			{
				return (static_cast<LightRegion *>(ListElement<Region>::Next()));
			}
			
			unsigned_int32 GetLightRegionFlags(void) const
			{
				return (lightRegionFlags);
			}
			
			void SetLightRegionFlags(unsigned_int32 flags)
			{
				lightRegionFlags = flags;
			}
			
			Light *GetLight(void) const
			{
				return (regionLight);
			}
			
			const Portal *GetIlluminatedPortal(void) const
			{
				return (illuminatedPortal);
			}
			
			void SetIlluminatedPortal(const Portal *portal)
			{
				illuminatedPortal = portal;
			}
			
			int32 GetBoundaryPolygonCount(void) const
			{
				return (boundaryPolygonCount);
			}
			
			void SetBoundaryPolygonCount(int32 count)
			{
				boundaryPolygonCount = count;
			}
			
			int32 GetBoundaryPolygonVertexCount(int32 index) const
			{
				return (boundaryPolygonVertexCount[index]);
			}
			
			void SetBoundaryPolygonVertexCount(int32 index, int32 count)
			{
				boundaryPolygonVertexCount[index] = count;
			}
			
			Point3D *GetBoundaryPolygonVertexArray(void)
			{
				return (boundaryPolygonVertexArray);
			}
			
			const Point3D *GetBoundaryPolygonVertexArray(void) const
			{
				return (boundaryPolygonVertexArray);
			}
	};
	
	
	class SourceRegion : public ZoneRegion, public Tree<SourceRegion>
	{
		private:
			
			Source				*regionSource;
			SourceRegion		*primaryRegion;
			
			const Portal		*permeatedPortal;
			Point3D				permeatedPosition;
			float				permeatedPathLength;
			
			SourceRegion		*audibleSubregion;
			Point3D				audiblePosition;
			float				audiblePathLength;
		
		public:
			
			SourceRegion(Source *source, Zone *zone);
			SourceRegion(Source *source, Zone *zone, SourceRegion *region);
			~SourceRegion();
			
			SourceRegion *GetPreviousSourceRegion(void) const
			{
				return (static_cast<SourceRegion *>(ListElement<Region>::Previous()));
			}
			
			SourceRegion *GetNextSourceRegion(void) const
			{
				return (static_cast<SourceRegion *>(ListElement<Region>::Next()));
			}
			
			Source *GetSource(void) const
			{
				return (regionSource);
			}
			
			SourceRegion *GetPrimaryRegion(void) const
			{
				return (primaryRegion);
			}
			
			void SetPrimaryRegion(SourceRegion *region)
			{
				primaryRegion = region;
			}
			
			const Portal *GetPermeatedPortal(void) const
			{
				return (permeatedPortal);
			}
			
			void SetPermeatedPortal(const Portal *portal, const Point3D& position, float length)
			{
				permeatedPortal = portal;
				permeatedPosition = position;
				permeatedPathLength = length;
			}
			
			const Point3D& GetPermeatedPosition(void) const
			{
				return (permeatedPosition);
			}
			
			float GetPermeatedPathLength(void) const
			{
				return (permeatedPathLength);
			}
			
			SourceRegion *GetAudibleSubregion(void) const
			{
				return (audibleSubregion);
			}
			
			void SetAudibleSubregion(SourceRegion *region)
			{
				audibleSubregion = region;
			}
			
			const Point3D& GetAudiblePosition(void) const
			{
				return (audiblePosition);
			}
			
			float GetAudiblePathLength(void) const
			{
				return (audiblePathLength);
			}
			
			void SetAudiblePosition(const Point3D& position, float length)
			{
				audiblePosition = position;
				audiblePathLength = length;
			}
			
			void InvertAudiblePathLength(float length)
			{
				audiblePathLength = length - audiblePathLength;
			}
	};
	
	
	#if C4DIAGNOSTICS
	
		class RegionRenderable : public Renderable
		{
			private:
				
				List<Attribute>		attributeList;
				DiffuseAttribute	diffuseColor;
				ColorRGBA			wireframeColor;
				
				Point3D				vertexArray[kMaxRegionPlaneCount * (kMaxRegionPlaneCount + 3)];
				Triangle			triangleArray[kMaxRegionPlaneCount * (kMaxRegionPlaneCount + 1)];
			
			public:
				
				RegionRenderable(const Region *region, const Point3D& referencePoint, float size);
				~RegionRenderable();
				
				const ColorRGBA& GetRegionColor(void) const
				{
					return (diffuseColor.GetDiffuseColor());
				}
				
				void SetRegionColor(const ColorRGBA& color)
				{
					diffuseColor.SetDiffuseColor(color);
				}
		};
	
	#endif
}


#endif

// ZYURVUR
