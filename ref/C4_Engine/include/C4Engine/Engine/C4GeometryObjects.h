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


#ifndef C4GeometryObjects_h
#define C4GeometryObjects_h


//# \component	World Manager
//# \prefix		WorldMgr/

//# \import		C4Node.h


#include "C4Objects.h"
#include "C4Bounding.h"
#include "C4GeometryLevel.h"
#include "C4Node.h"


namespace C4
{
	typedef Type	GeometryType;
	typedef Type	PrimitiveType;
	
	
	enum
	{
		kObjectGeometry			= 'GEOM'
	};
	
	
	enum
	{
		kGeometryMesh			= 'MESH'
	};
	
	
	//# \enum	GeometryFlags
	
	enum
	{
		kGeometryInvisible				= 1 << 1,		//## Geometry is invisible (but can still participate in collision detection).
		kGeometryAmbientOnly			= 1 << 2,		//## Geometry is rendered only in ambient light.
		kGeometryRenderShadowMap		= 1 << 4,		//## Geometry participates in shadow map generation.
		kGeometryCubeLightInhibit		= 1 << 6,		//## Render with point light shaders when illuminated by a cube light.
		kGeometryShadowInhibit			= 1 << 11,		//## Geometry does not cast stencil shadows.
		kGeometryMarkingInhibit			= 1 << 13,		//## Surface markings are not be applied to the geometry.
		kGeometryFogInhibit				= 1 << 14,		//## Fog is not applied to the geometry.
		kGeometryShaderDetailEnable		= 1 << 18,		//## Multiple shader detail levels are enabled.
		kGeometryDynamic				= 1 << 19,		//## Shadow volumes should not be cached because this geometry is dynamic.
		kGeometryMotionBlurInhibit		= 1 << 23,		//## Geometry does not get rendered with motion blur.
		kGeometryRemotePortal			= 1 << 24,		//## Geometry covers a remote portal and should be rendered first when visible.
		kGeometryRenderEffectPass		= 1 << 25,		//## Geometry is rendered after lighting in the effect pass.
		kGeometryRenderDecal			= 1 << 26,		//## Geometry is rendered with depth offset for decaling.
		kGeometryConvexHull				= 1 << 27,		//## The geometry's convex hull is used for collision detection.
		kGeometryTwoSidedPlaneArray		= 1 << 29,
		
		kGeometryModelExportFlags		= kGeometryDynamic,
		kGeometrySkinnedModelFlags		= kGeometryModelExportFlags | kGeometryMarkingInhibit
	};
	
	
	//# \enum	GeometryEffectFlags
	
	enum
	{
		kGeometryEffectShader			= 1 << 0,
		kGeometryEffectAccumulate		= 1 << 1		//## Use an additive blending mode instead of alpha interpolation.
	};
	
	
	enum
	{
		kGeometryObjectPrototype		= 1 << 0,
		kGeometryObjectPreprocessed		= 1 << 1,
		kGeometryObjectStaticSurfaces	= 1 << 2,
		kGeometryObjectConvexPrimitive	= 1 << 3
	};
	
	
	enum BooleanOperation
	{
		kBooleanUnion,
		kBooleanIntersection
	};
	
	
	class Zone;
	class Geometry;
	class Manipulator;
	struct BooleanLoop;
	
	
	struct GeometryHitData
	{
		Point3D			position;
		Vector3D		normal;
		float			param;
		unsigned_int32	triangleIndex;
	};
	
	
	struct CollisionOctree
	{
		Box3D				collisionBounds; 
		unsigned_int16		subnodeOffset[8];
		unsigned_int16		elementCount; 
		unsigned_int16		offsetAlign; 
		 
		CollisionOctree *GetSubnode(int32 index)
		{ 
			return (reinterpret_cast<CollisionOctree *>(reinterpret_cast<char *>(this) + subnodeOffset[index] * offsetAlign));
		}
		
		const CollisionOctree *GetSubnode(int32 index) const 
		{
			return (reinterpret_cast<const CollisionOctree *>(reinterpret_cast<const char *>(this) + subnodeOffset[index] * offsetAlign));
		}
		 
		unsigned_int16 *GetIndexArray(void)
		{
			return (reinterpret_cast<unsigned_int16 *>(this + 1));
		}
		
		const unsigned_int16 *GetIndexArray(void) const
		{
			return (reinterpret_cast<const unsigned_int16 *>(this + 1));
		}
	};
	
	void Reverse(CollisionOctree *octree);
	
	
	//# \class	GeometryObject	Encapsulates data for a geometry.
	//
	//# The $GeometryObject$ class encapsulates data for a geometry.
	//
	//# \def	class GeometryObject : public Object
	//
	//# \ctor	GeometryObject(GeometryType type);
	//
	//# The constructor has protected access. The $GeometryObject$ class can only exist as the base class for a more specific type of geometry.
	//
	//# \param	type	The geometry type.
	//
	//# \desc
	//# The $GeometryObject$ class all of the geometric information pertaining to a geometry. Each geometry object has
	//# one or more levels of detail, and the geometric data (such as vertex and triangle arrays) for each level is stored
	//# in a $@GeometryLevel@$ object.
	//
	//# \base	Object		A $GeometryObject$ is an object that can be shared by multiple geometry nodes.
	//
	//# \also	$@GeometryLevel@$
	//# \also	$@Geometry@$
	
	
	//# \function	GeometryObject::GetGeometryType		Returns the geometry type.
	//
	//# \proto	GeometryType GetGeometryType(void) const;
	//
	//# \desc
	//# The $GetGeometryType$ function returns the geometry type.
	
	
	//# \function	GeometryObject::GetGeometryFlags		Returns the geometry flags.
	//
	//# \proto	unsigned_int32 GetGeometryFlags(void) const;
	//
	//# \desc
	//# The $GetGeometryFlags$ function returns the geometry flags, which can be a combination (through logical OR) of the
	//# following bit flags.
	//
	//# \table	GeometryFlags
	//
	//# By default, none of the geometry flags are set.
	//
	//# \also	$@GeometryObject::SetGeometryFlags@$
	
	
	//# \function	GeometryObject::SetGeometryFlags		Sets the geometry flags.
	//
	//# \proto	void SetGeometryFlags(unsigned_int32 flags);
	//
	//# \param	flags	The new geometry flags.
	//
	//# \desc
	//# The $SetGeometryFlags$ function sets the geometry flags. The $flags$ parameter may be any
	//# combination of the following bit flags.
	//
	//# \table	GeometryFlags
	//
	//# By default, none of the geometry flags are set.
	//
	//# \also	$@GeometryObject::GetGeometryFlags@$
	
	
	//# \function	GeometryObject::GetGeometryLevelCount		Returns the number of detail levels.
	//
	//# \proto	int32 GetGeometryLevelCount(void) const;
	//
	//# \desc
	//# The $GetGeometryLevelCount$ function returns the number of detail levels belonging to a geometry object.
	//# The geometric information for a particular detail level can be retrieved using the $@GeometryObject::GetGeometryLevel@$ function.
	//
	//# \also	$@GeometryObject::GetGeometryLevel@$
	
	
	//# \function	GeometryObject::GetGeometryLevel		Returns a specific geometric level of detail.
	//
	//# \proto	GeometryLevel *GetGeometryLevel(int32 level) const;
	//
	//# \param	level	The detail level to retrieve.
	//
	//# \desc
	//# The $GetGeometryLevel$ function returns the a pointer to the $@GeometryLevel@$ object for the detail level
	//# specified by the $level$ parameter. The number of detail levels can be determined by calling the
	//# $@GeometryObject::GetGeometryLevelCount@$ function. The $level$ parameter must be in the range
	//# [0,&nbsp;<i>n</i>&nbsp;&minus;&nbsp;1], where <i>n</i> is the number of detail levels.
	//
	//# \also	$@GeometryObject::GetGeometryLevelCount@$
	//# \also	$@GeometryLevel@$
	
	
	//# \function	GeometryObject::GetCollisionLevel		Returns the index of the detail level used for collision detection.
	//
	//# \proto	int32 GetCollisionLevel(void) const;
	//
	//# \desc
	//# The $GetCollisionLevel$ function returns the index of the detail level used for collision detection.
	//
	//# \also	$@GeometryObject::SetCollisionLevel@$
	//# \also	$@GeometryObject::GetCollisionExclusionMask@$
	//# \also	$@GeometryObject::SetCollisionExclusionMask@$
	//# \also	$@World::DetectCollision@$
	//# \also	$@World::QueryCollision@$
	
	
	//# \function	GeometryObject::SetCollisionLevel		Sets the index of the detail level used for collision detection.
	//
	//# \proto	void SetCollisionLevel(int32 level) const;
	//
	//# \param	level	The detail level index.
	//
	//# \desc
	//# The $SetCollisionLevel$ function sets the index of the detail level used for collision detection. The $level$
	//# parameter should be in the range [0,&nbsp;<i>n</i>&nbsp;&minus;&nbsp;1], where <i>n</i> is the number of detail levels.
	//# If the $level$ parameter is greater than or equal to <i>n</i>, then the collision level is set to <i>n</i>&nbsp;&minus;&nbsp;1.
	//
	//# \also	$@GeometryObject::GetCollisionLevel@$
	//# \also	$@GeometryObject::GetCollisionExclusionMask@$
	//# \also	$@GeometryObject::SetCollisionExclusionMask@$
	//# \also	$@World::DetectCollision@$
	//# \also	$@World::QueryCollision@$
	
	
	//# \function	GeometryObject::GetCollisionExclusionMask		Returns the collision exclusion mask.
	//
	//# \proto	unsigned_int32 GetCollisionExclusionMask(void) const;
	//
	//# \desc
	//# The $GetCollisionExclusionMask$ function returns the collision exclusion mask, which may be a combination
	//# (through logical OR) of the following bit flags.
	//
	//# \table	CollisionKind
	//
	//# The collision exclusion mask is used to invalidate collisions with rigid bodies having specific collision kinds.
	//# The mask is also used to invalidate collisions that are detected by the $@World::DetectCollision@$
	//# and $@World::QueryCollision@$ functions.
	//
	//# \also	$@GeometryObject::SetCollisionExclusionMask@$
	//# \also	$@PhysicsMgr/RigidBodyController::GetCollisionKind@$
	//# \also	$@PhysicsMgr/RigidBodyController::SetCollisionKind@$
	//# \also	$@PhysicsMgr/RigidBodyController::ValidGeometryCollision@$
	//# \also	$@World::DetectCollision@$
	//# \also	$@World::QueryCollision@$
	
	
	//# \function	GeometryObject::SetCollisionExclusionMask		Sets the collision exclusion mask.
	//
	//# \proto	void SetCollisionExclusionMask(unsigned_int32 mask);
	//
	//# \param	mask	The new collision exclusion mask.
	//
	//# \desc
	//# The $SetCollisionExclusionMask$ function sets the collision mask. The $mask$ parameter may be a
	//# combination (through logical OR) of the following bit flags.
	//
	//# \table	CollisionKind
	//
	//# The collision exclusion mask is used to invalidate collisions with rigid bodies having specific collision kinds.
	//# The mask is also used to invalidate collisions that are detected by the $@World::DetectCollision@$
	//# and $@World::QueryCollision@$ functions.
	//
	//# \also	$@GeometryObject::GetCollisionExclusionMask@$
	//# \also	$@PhysicsMgr/RigidBodyController::GetCollisionKind@$
	//# \also	$@PhysicsMgr/RigidBodyController::SetCollisionKind@$
	//# \also	$@PhysicsMgr/RigidBodyController::ValidGeometryCollision@$
	//# \also	$@World::DetectCollision@$
	//# \also	$@World::QueryCollision@$
	
	
	class C4_API GeometryObject : public Object
	{
		friend class WorldMgr;
		
		private:
			
			GeometryType							geometryType;
			
			unsigned_int32							geometryFlags;
			unsigned_int32							geometryEffectFlags;
			
			float									geometryDetailBias;
			float									shaderDetailBias;
			
			unsigned_int16							geometryObjectFlags;
			unsigned_int16							dynamicArrayFlags;
			
			unsigned_int32							collisionExclusionMask;
			int32									collisionLevel;
			
			int32									geometryLevelCount;
			GeometryLevel							*geometryLevel;
			
			int32									surfaceCount;
			SurfaceData								*surfaceData;
			
			CollisionOctree							*collisionOctree;
			unsigned_int32							collisionOctreeSize;
			
			int32									convexHullVertexCount;
			unsigned_int16							*convexHullIndexArray;
			
			VertexBuffer							staticVertexBuffer;
			VertexBuffer							indexBuffer;

			VertexBufferObserver<GeometryObject>	staticVertexBufferObserver;
			VertexBufferObserver<GeometryObject>	indexBufferObserver;
			
			void Initialize(void);
			
			static GeometryObject *Construct(Unpacker& data, unsigned_int32 unpackFlags);
			
			void FillStaticVertexBuffer(VertexBuffer *vertexBuffer);
			void FillIndexBuffer(VertexBuffer *indexBuffer);
			
			static unsigned_int32 GetCompressedOctreeSize(const GeometryOctree *geometryOctree);
			static char *CompressOctree(const GeometryOctree *geometryOctree, CollisionOctree *collisionOctree);
			
			GeometryOctree *BuildCollisionOctree(const GeometryLevel *level, const Box3D& boundingBox);
			
			static bool ClipSegmentToCollisionBounds(const Box3D& bounds, float radius, Point3D& p1, Point3D& p2);
			static bool DetectSegmentIntersection(const CollisionOctree *octree, const GeometryLevel *level, const Bivector4D& segmentLine, const Point3D& p1, const Point3D& p2, GeometryHitData *geometryHitData);
			static bool DetectSegmentEdgeIntersection(const Bivector4D& segmentLine, const Bivector4D& edgeLine, const Point3D& p1, const Vector3D& v1, float r2, float& smax, GeometryHitData *geometryHitData);
			static bool DetectSegmentVertexIntersection(const Bivector4D& segmentLine, const Point3D& p1, const Vector3D& v1, float r2, float a, float ainv, float& smax, GeometryHitData *geometryHitData);
			static bool DetectSegmentIntersection(const CollisionOctree *octree, const GeometryLevel *level, const Point3D& p1, const Point3D& p2, float radius, GeometryHitData *geometryHitData);
			
			static void ScaleCollisionOctree(CollisionOctree *octree, float factor);
			static void OffsetCollisionOctree(CollisionOctree *octree, const Vector3D& dv);
		
		protected:
			
			GeometryObject(GeometryType type);
			~GeometryObject();

			void SetConvexPrimitiveFlag(void)
			{
				geometryObjectFlags |= kGeometryObjectConvexPrimitive;
			}

			void ClearConvexPrimitiveFlag(void)
			{
				geometryObjectFlags &= ~kGeometryObjectConvexPrimitive;
			}
			
			void SetStaticSurfaceData(int32 count, SurfaceData *data, bool init = false);
			
			void ResetVertexBuffers(void);
		
		public:
			
			GeometryObject(GeometryType type, int32 levelCount);
			
			GeometryType GetGeometryType(void) const
			{
				return (geometryType);
			}
			
			unsigned_int32 GetGeometryFlags(void) const
			{
				return (geometryFlags);
			}
			
			void SetGeometryFlags(unsigned_int32 flags)
			{
				geometryFlags = flags;
			}
			
			unsigned_int32 GetGeometryEffectFlags(void) const
			{
				return (geometryEffectFlags);
			}
			
			void SetGeometryEffectFlags(unsigned_int32 flags)
			{
				geometryEffectFlags = flags;
			}
			
			float GetGeometryDetailBias(void) const
			{
				return (geometryDetailBias);
			}
			
			void SetGeometryDetailBias(float bias)
			{
				geometryDetailBias = bias;
			}
			
			float GetShaderDetailBias(void) const
			{
				return (shaderDetailBias);
			}
			
			void SetShaderDetailBias(float bias)
			{
				shaderDetailBias = bias;
			}
			
			void SetPrototypeFlag(void)
			{
				geometryObjectFlags |= kGeometryObjectPrototype;
			}

			bool GetConvexPrimitiveFlag(void) const
			{
				return ((geometryObjectFlags & kGeometryObjectConvexPrimitive) != 0);
			}
			
			const VertexBuffer *GetStaticVertexBuffer(void) const
			{
				return (&staticVertexBuffer);
			}
			
			const VertexBuffer *GetIndexBuffer(void) const
			{
				return (&indexBuffer);
			}
			
			unsigned_int32 GetCollisionExclusionMask(void) const
			{
				return (collisionExclusionMask);
			}
			
			void SetCollisionExclusionMask(unsigned_int32 mask)
			{
				collisionExclusionMask = mask;
			}
			
			int32 GetCollisionLevel(void) const
			{
				return (collisionLevel);
			}
			
			void SetCollisionLevel(int32 level)
			{
				collisionLevel = Min(level, GetMaxCollisionLevel());
			}
			
			int32 GetGeometryLevelCount(void) const
			{
				return (geometryLevelCount);
			}
			
			GeometryLevel *GetGeometryLevel(int32 level) const
			{
				return (&geometryLevel[level]);
			}
			
			int32 GetSurfaceCount(void) const
			{
				return (surfaceCount);
			}
			
			SurfaceData *GetSurfaceData(int32 index = 0) const
			{
				return (&surfaceData[index]);
			}
			
			const CollisionOctree *GetCollisionOctree(void) const
			{
				return (collisionOctree);
			}
			
			const Point3D *GetConvexHullVertexArray(void) const
			{
				return (geometryLevel[collisionLevel].GetArray<Point3D>(kArrayVertex));
			}
			
			Point3D GetInitialConvexHullSupportPoint(const Point3D *vertex) const
			{
				return (vertex[convexHullIndexArray[0]]);
			}
			
			int32 Release(void);
			
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
			
			virtual void Preprocess(unsigned_int32 dynamicFlags);
			virtual void Neutralize(void);
			
			void SetGeometryLevelCount(int32 levelCount);
			void SetSurfaceCount(int32 count);
			
			void BuildCollisionData(void);
			void ScaleCollisionData(float factor);
			void OffsetCollisionData(const Vector3D& dv);
			
			const Point3D& CalculateConvexHullSupportPoint(const Point3D *vertex, const Vector3D& direction) const;
			void CalculateConvexHullSupportPointArray(const Point3D *vertex, int32 count, const Vector3D *direction, Point3D *support) const;
			
			virtual Point3D GetInitialPrimitiveSupportPoint(void) const;
			virtual Point3D CalculatePrimitiveSupportPoint(const Vector3D& direction) const;
			virtual void CalculatePrimitiveSupportPointArray(int32 count, const Vector3D *direction, Point3D *support) const;
			
			virtual int32 GetMaxCollisionLevel(void) const;
			virtual bool DetectCollision(const Point3D& p1, const Point3D& p2, float radius, GeometryHitData *geometryHitData) const;
			
			virtual bool ExteriorSphere(const Point3D& center, float radius) const;
			virtual bool ExteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const;
	};
	
	
	//# \class	MeshGeometryObject	Encapsulates data for a mesh geometry.
	//
	//# The $MeshGeometryObject$ class encapsulates data for a mesh geometry.
	//
	//# \def	class MeshGeometryObject : public GeometryObject
	//
	//# \ctor	MeshGeometryObject(const Geometry *geometry);
	//
	//# \param	geometry	A pointer to another geometry node that is copied into the mesh geometry..
	//
	//# \desc
	//
	//# \base	GeometryObject		A $MeshGeometryObject$ is an object that can be shared by multiple mesh geometry nodes.
	//
	//# \also	$@MeshGeometry@$
	
	
	class C4_API MeshGeometryObject : public GeometryObject
	{
		friend class GeometryObject;
		
		private:
			
			BoundingSphere		boundingSphere;
			Box3D				boundingBox;
			
			~MeshGeometryObject();
			
			void BuildGeometryLevel(int32 level, unsigned_int32 flags, const List<GeometrySurface> *surfaceList, int32 materialCount, const SkinData *skinData = nullptr);
			
			static unsigned_int32 IntersectMeshes(const GeometryLevel *targetLevel, const GeometryLevel *auxLevel, List<GeometrySurface> *resultList, const Geometry *targetGeometry, Array<SurfaceData> *surfaceDataArray, const Array<MaterialObject *>& materialArray);
			static void IntersectPolygonAndMesh(const Point3D *polygonVertex, const Vector3D *polygonNormal, const ColorRGBA *polygonColor, const Point2D *polygonTexcoord, const GeometryLevel *geometryLevel, float geometryVolume, List<GeometryPolygon> *resultList);
			static void ConstructBooleanLoops(const Antivector4D& plane, const GeometryLevel *geometryLevel, List<BooleanLoop> *positiveList, List<BooleanLoop> *negativeList);
			static void ConvexDecomposeLoop(const Vector3D& normal, BooleanLoop *inputLoop, List<BooleanLoop> *outputList);
			static void CalculatePolygonAttributes(const Point3D *polygonVertex, const Vector3D *polygonNormal, const ColorRGBA *polygonColor, const Point2D *polygonTexcoord, int32 vertexCount, const Point3D *vertex, Vector3D *normal, ColorRGBA *color, Point2D *texcoord);
		
		public:
			
			MeshGeometryObject();
			MeshGeometryObject(const Geometry *geometry);
			MeshGeometryObject(int32 levelCount, const List<GeometrySurface> *const *surfaceListTable, int32 surfaceCount, const Array<int32>& materialIndexArray, const SkinData *const *skinDataTable = nullptr);
			MeshGeometryObject(int32 geometryCount, const Geometry *const *geometryArray, const Array<MaterialObject *>& materialArray, const Transformable *transformable);
			MeshGeometryObject(BooleanOperation operation, const Geometry *geometry1, const Geometry *geometry2, const Array<MaterialObject *>& materialArray);
			
			BoundingSphere *GetBoundingSphere(void)
			{
				return (&boundingSphere);
			}
			
			const BoundingSphere *GetBoundingSphere(void) const
			{
				return (&boundingSphere);
			}
			
			void SetBoundingSphere(const BoundingSphere *sphere)
			{
				boundingSphere = *sphere;
			}
			
			void SetBoundingSphere(const Point3D& center, float radius)
			{
				boundingSphere.SetCenter(center);
				boundingSphere.SetRadius(radius);
			}
			
			const Box3D& GetBoundingBox(void) const
			{
				return (boundingBox);
			}
			
			void SetBoundingBox(const Box3D& box)
			{
				boundingBox = box;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetObjectSize(float *size) const;
			void SetObjectSize(const float *size);
			
			bool ExteriorSphere(const Point3D& center, float radius) const;
			bool ExteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const;
			
			void UpdateBounds(void);
			void Rebuild(const Geometry *geometry);
	};
}


#endif

// ZYURVUR
