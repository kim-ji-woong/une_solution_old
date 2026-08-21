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


#ifndef C4Geometries_h
#define C4Geometries_h


//# \component	World Manager
//# \prefix		WorldMgr/

//# \import		C4World.h


#include "C4GeometryObjects.h"
#include "C4Shadows.h"
#include "C4Spaces.h"


namespace C4
{
	extern const char kConnectorKeyPaint[];
	
	
	//# \class	Geometry	Represents a geometry node in a world.
	//
	//# The $Geometry$ class represents a geometry node in a world.
	//
	//# \def	class Geometry : public RenderableNode, public ListElement<Geometry>
	//
	//# \ctor	Geometry(GeometryType type);
	//
	//# The constructor has protected access. A $Geometry$ class can only exist as the base class for another class.
	//
	//# \desc
	//# The $Geometry$ class serves as the base class for all geometrical nodes in the world. The geometrical information
	//# itself is stored in the associated $@GeometryObject@$ class and $@GeometryLevel@$ class.
	//#
	//# A geometry node can be either a primitive geometry type, a generic mesh, or a chunk of voxel terrain.
	//# See the <a href="Node_tree.html">node class hierarchy</a> for a diagram showing the relationships among these types.
	//
	//# \base	RenderableNode						A $Geometry$ node is a renderable scene graph node.
	//# \base	Utilities/ListElement<Geometry>		Used internally by the World Manager.
	//
	//# \also	$@GeometryObject@$
	//# \also	$@GeometryLevel@$
	//# \also	$@GraphicsMgr/MaterialObject@$
	
	
	//# \function	Geometry::GetGeometryType		Returns the geometry type.
	//
	//# \proto	GeometryType GetGeometryType(void) const;
	//
	//# \desc
	//# The $GetGeometryType$ function returns the geometry type. It can be one of the following values.
	//
	//# \value	kGeometryMesh			A generic mesh.
	//# \value	kGeometryPrimitive		A primitive geometry.
	//# \value	kGeometryTerrain		A chunk of voxel terrain.


	//# \function	Geometry::GetMaterialCount		Returns the number of material slots.
	//
	//# \proto	int32 GetMaterialCount(void) const;
	//
	//# \desc
	//# The $GetMaterialCount$ function returns the number of material slots allocated for a geometry.
	//# The return value is always at least 1.
	//
	//# \also	$@Geometry::SetMaterialCount@$
	//# \also	$@Geometry::GetMaterialObject@$
	//# \also	$@Geometry::SetMaterialObject@$
	
	
	//# \function	Geometry::SetMaterialCount		Sets the number of material slots.
	//
	//# \proto	void SetMaterialCount(int32 count);
	//
	//# \param	count	The new number of material slots. This cannot be less than 1.
	//
	//# \desc
	//# The $SetMaterialCount$ function sets the number of material slots allocated for a geometry to the
	//# number specified by the $count$ parameter. If the new number of material slots is less than its
	//# previous value, then any material objects assigned to slots with indexes greater than or equal to
	//# $count$ are released.
	//
	//# \also	$@Geometry::GetMaterialCount@$
	//# \also	$@Geometry::GetMaterialObject@$
	//# \also	$@Geometry::SetMaterialObject@$
	
	
	//# \function	Geometry::GetMaterialObject		Returns a material object.
	//
	//# \proto	MaterialObject *GetMaterialObject(unsigned_int32 index) const;
	//
	//# \param	index		The index of the material slot from which to retrieve a material object.
	//
	//# \desc
	//# The $GetMaterialObject$ function returns one of the material objects assigned to a geometry node.
	//# For a geometry having <i>n</i> materials, the $index$ parameter should be an integer between 0 and
	//# <i>n</i>&nbsp;&minus;&nbsp;1. If no material object has been assigned for the specified index, then
	//# this function returns $nullptr$.
	//# 
	//# The number of material slots can be determined using the $@Geometry::GetMaterialCount@$ function.
	//
	//# \also	$@GraphicsMgr/MaterialObject@$ 
	//# \also	$@Geometry::SetMaterialObject@$
	//# \also	$@Geometry::GetMaterialCount@$ 
	//# \also	$@Geometry::SetMaterialCount@$ 
	 
	
	//# \function	Geometry::SetMaterialObject		Sets a material object. 
	//
	//# \proto	void SetMaterialObject(unsigned_int32 index, MaterialObject *object);
	//
	//# \param	index		The index of the material slot to which a material object is to be assigned. 
	//# \param	object		The new material object. This can be $nullptr$.
	//
	//# \desc
	//# The $SetMaterialObject$ function assigns the material object specified by the $object$ parameter 
	//# to a geometry node in the material slot specified by the $index$ parameter. If $object$ is $nullptr$,
	//# then the geometry node does not have a material in the specified material slot after this function is
	//# called. Otherwise, the reference count of the material object is incremented, and the new material
	//# object is assigned to the geometry node. The reference count of any material object previously
	//# assigned to the geometry node in the same slot is decremented, and the old material object is
	//# deleted if its reference count reaches zero.
	//# 
	//# The number of material slots can be determined using the $@Geometry::GetMaterialCount@$ function.
	//
	//# \also	$@GraphicsMgr/MaterialObject@$
	//# \also	$@Geometry::GetMaterialObject@$
	//# \also	$@Geometry::GetMaterialCount@$
	//# \also	$@Geometry::SetMaterialCount@$
	
	
	//# \function	Geometry::GetPerspectiveExclusionMask		Returns the perspective exclusion mask.
	//
	//# \proto	unsigned_int32 GetPerspectiveExclusionMask(void) const;
	//
	//# \desc
	//# The $GetPerspectiveExclusionMask$ function returns the perspective exclusion mask that determines from
	//# what camera perspectives the geometry is visible. The mask can be a combination (through logical OR)
	//# of the following values.
	//
	//# \table	PerspectiveMask
	//
	//# For any bits that are set in the perspective exclusion mask, the geometry is not rendered for cameras
	//# having the matching perspective. The initial value of the mask is 0, meaning that the geometry is
	//# rendered from all camera perspectives.
	//
	//# \also	$@Geometry::SetPerspectiveExclusionMask@$
	
	
	//# \function	Geometry::SetPerspectiveExclusionMask		Sets the perspective exclusion mask.
	//
	//# \proto	void SetPerspectiveExclusionMask(unsigned_int32 mask);
	//
	//# \param	mask	The new perspective exclusion mask.
	//
	//# \desc
	//# The $SetPerspectiveExclusionMask$ function sets the perspective exclusion mask that determines from
	//# what camera perspectives the geometry is visible to the value specified by the $mask$ parameter.
	//# The mask can be a combination (through logical OR) of the following values.
	//
	//# \table	PerspectiveMask
	//
	//# For any bits that are set in the perspective exclusion mask, the geometry is not rendered for cameras
	//# having the matching perspective. The initial value of the mask is 0, meaning that the geometry is
	//# rendered from all camera perspectives.
	//
	//# \also	$@Geometry::GetPerspectiveExclusionMask@$
	
	
	class C4_API Geometry : public RenderableNode, public ListElement<Geometry>
	{
		friend class Node;
		
		private:
			
			enum
			{
				kMaxStaticStencilVolumeCount = 2
			};
			
			GeometryType			geometryType;
			
			int32					geometryDetailLevel;
			int32					minGeometryDetailLevel;
			
			unsigned_int32			perspectiveExclusionMask;
			int32					geometryRenderStage;
			
			unsigned_int32			shadowStamp;
			volatile int32			queryThreadFlags;
			
			int32					materialCount;
			MaterialObject			*materialObject;
			char					*segmentStorage;
			
			const ArrayBundle		*arrayBundle[kMaxGeometryArrayCount];
			bool					*shadowFrontArray;
			
			StencilData				stencilData;
			Link<StencilVolume>		staticStencilVolume[kMaxStaticStencilVolumeCount];
			
			MaterialObject **GetMaterialObjectTable(void) const
			{
				return (reinterpret_cast<MaterialObject **>(segmentStorage));
			}
			
			RenderSegment *GetRenderSegmentTable(void) const
			{
				return (reinterpret_cast<RenderSegment *>(GetMaterialObjectTable() + (materialCount - 1)));
			}
			
			static Geometry *Construct(Unpacker& data, unsigned_int32 unpackFlags);
			static void MaterialObjectLinkProc(Object *object, void *cookie);
			
			void ReleaseSegmentStorage(void);
			
			bool AlphaTestMaterial(void) const;
		
		protected:
			
			Geometry(GeometryType type);
			Geometry(const Geometry& geometry);
		
		public:
			
			virtual ~Geometry();
			
			using ListElement<Geometry>::Previous;
			using ListElement<Geometry>::Next;
			
			GeometryType GetGeometryType(void) const
			{
				return (geometryType);
			}
			
			GeometryObject *GetObject(void) const
			{
				return (static_cast<GeometryObject *>(Node::GetObject()));
			}
			
			int32 GetDetailLevel(void) const
			{
				return (geometryDetailLevel);
			}
			
			int32 GetMinDetailLevel(void) const
			{
				return (minGeometryDetailLevel);
			}
			
			void SetMinDetailLevel(int32 level)
			{
				minGeometryDetailLevel = level;
			}
			
			unsigned_int32 GetPerspectiveExclusionMask(void) const
			{
				return (perspectiveExclusionMask);
			}
			
			unsigned_int32 GetShadowPerspectiveExclusionMask(void) const
			{
				return (perspectiveExclusionMask >> 16);
			}
			
			void SetPerspectiveExclusionMask(unsigned_int32 mask)
			{
				perspectiveExclusionMask = mask;
			}
			
			int32 GetGeometryRenderStage(void) const
			{
				return (geometryRenderStage);
			}
			
			unsigned_int32 GetShadowStamp(void) const
			{
				return (shadowStamp);
			}
			
			void SetShadowStamp(unsigned_int32 stamp)
			{
				shadowStamp = stamp;
			}
			
			volatile int32 *GetQueryThreadFlags(void)
			{
				return (&queryThreadFlags);
			}
			
			int32 GetMaterialCount(void) const
			{
				return (materialCount);
			}
			
			MaterialObject *GetMaterialObject(unsigned_int32 index) const
			{
				return ((index == 0) ? materialObject : GetMaterialObjectTable()[index - 1]);
			}
			
			const ArrayBundle *GetArrayBundle(int32 index) const
			{
				return (arrayBundle[index]);
			}
			
			void SetArrayBundle(int32 index, ArrayBundle *bundle)
			{
				arrayBundle[index] = bundle;
			}
			
			bool *GetShadowFrontArray(void) const
			{
				return (shadowFrontArray);
			}
			
			void PackType(Packer& data) const;
			void Prepack(List<Object> *linkList) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
			
			int32 GetInternalConnectorCount(void) const;
			const char *GetInternalConnectorKey(int32 index) const;
			bool ValidConnectedNode(const ConnectorKey& key, const Node *node) const;
			PaintSpace *GetConnectedPaintSpace(void) const;
			void SetConnectedPaintSpace(PaintSpace *space);
			
			void Preprocess(void);
			void Neutralize(void);
			
			void EnterZone(Zone *zone);
			
			void SetMaterialCount(int32 count);
			void SetMaterialObject(unsigned_int32 index, MaterialObject *object);
			void OptimizeMaterials(void);
			
			const MaterialObject *GetTriangleMaterial(int32 triangleIndex) const;
			
			void SetDetailLevel(int32 level);
			
			StencilData *GetStencilData(void);
			Link<StencilVolume> *GetStaticStencilVolume(const Light *light);
			void InvalidateStaticShadowVolumes(void);
			
			virtual void CalculateInfiniteShadowFrontArray(const Vector3D& lightDirection);
			virtual void CalculatePointShadowFrontArray(const Point3D& lightPosition);
	};
	
	
	//# \class	MeshGeometry		Represents a mesh geometry node in a world.
	//
	//# The $MeshGeometry$ class represents a mesh geometry node in a world.
	//
	//# \def	class MeshGeometry : public Geometry
	//
	//# \ctor	MeshGeometry(const Geometry *geometry);
	//
	//# \param	geometry	A pointer to another geometry node that is copied into the mesh geometry.
	//
	//# \desc
	//# The $MeshGeometry$ class represents a mesh geometry node in the world.
	//
	//# \base	Geometry		A mesh geometry node is a specific type of geometry.
	//
	//# \also	$@MeshGeometryObject@$
	
	
	class C4_API MeshGeometry : public Geometry
	{
		friend class Geometry;
		
		private:
			
			typedef void PostTransformProc(MeshGeometry *);
			
			PostTransformProc	*postTransformProc;
			
			Point3D				worldCenter;
			Vector3D			worldAxis[3];
			
			MeshGeometry(const MeshGeometry& meshGeometry);
			
			Node *Replicate(void) const override;
			
			static void CalculateOrientedBoundingBox(MeshGeometry *meshGeometry);
			
			static bool BoxVisible(const Node *node, const Region *region);
			static bool BoxOccluded(const Node *node, const Region *region);
			
			void CalculatePostTransform(void) override;
			bool CalculateBoundingBox(Box3D *box) const override;
			bool CalculateBoundingSphere(BoundingSphere *sphere) const override;
		
		public:
			
			MeshGeometry();
			MeshGeometry(const Geometry *geometry);
			MeshGeometry(int32 levelCount, const List<GeometrySurface> *const *surfaceList, MaterialObject *const *materialArray, const SkinData *const *skinData = nullptr);
			MeshGeometry(int32 geometryCount, const Geometry *const *geometryArray, const Transformable *transformable);
			MeshGeometry(BooleanOperation operation, const Geometry *geometry1, const Geometry *geometry2);
			~MeshGeometry();
			
			MeshGeometryObject *GetObject(void) const
			{
				return (static_cast<MeshGeometryObject *>(Node::GetObject()));
			}
			
			void SetPostTransformProc(PostTransformProc *proc)
			{
				postTransformProc = proc;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			void Preprocess(void);
	};
}


#endif

// ZYURVUR
