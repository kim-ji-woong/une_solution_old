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


#ifndef C4GeometryLevel_h
#define C4GeometryLevel_h


//# \component	World Manager
//# \prefix		WorldMgr/


#include "C4Renderable.h"
#include "C4Packing.h"


namespace C4
{
	typedef Type	TextureAlignMode;
	
	
	enum
	{
		kArraySharedIndex			= 16,
		kArrayNodeHash				= 17,
		kArraySurfaceIndex			= 18,
		kArrayHandedness			= 19,
		kArrayFace					= 20,
		kArrayInverseBindTransform	= 21,
		kArraySegment				= 22,
		kArrayEdge					= 23,
		kArrayPlane					= 24,
		kArrayPlaneIndex			= 25,
		kMaxGeometryArrayCount		= 26
	};
	
	
	enum GeometryArrayType
	{
		kGeometryFloat,
		kGeometryByte,
		kGeometryShort,
		kGeometryLong,
		kGeometryTriangle,
		kGeometryQuad,
		kGeometrySegment,
		kGeometryEdge
	};
	
	
	enum
	{
		kMaxGeometryTexcoordCount		= 2
	};
	
	
	enum
	{
		kTextureAlignNatural			= 'NATL',
		kTextureAlignObjectPlane		= 'OPLN',
		kTextureAlignWorldPlane			= 'WPLN',
		kTextureAlignGlobalObjectPlane	= 'GOPL',
		kTextureAlignModeCount			= 4
	};
	
	
	enum
	{
		kSurfaceValidNormals			= 1 << 0,
		kSurfaceValidTangents			= 1 << 1,
		kSurfaceValidColors				= 1 << 2
	};
	
	
	class GeometryObject;
	
	
	struct TextureAlignData
	{
		TextureAlignMode	alignMode;
		Antivector4D		alignPlane;
	};
	
	void Reverse(TextureAlignData *data);
	
	
	struct SurfaceData
	{
		unsigned_int16		surfaceFlags;
		unsigned_int16		materialIndex;
		TextureAlignData	textureAlignData[2];
	};
	
	void Reverse(SurfaceData *data);
	
	
	//# \struct	ArrayDescriptor		Contains information about a geometrical array.
	//
	//# The $ArrayDescriptor$ structure contains information about a geometrical array.
	//
	//# \def	struct ArrayDescriptor
	//
	//# \data	ArrayDescriptor
	
	
	//# \member		ArrayDescriptor 
	
	struct ArrayDescriptor 
	{ 
		int32		identifier;			//## The array identifier. 
		int32		elementCount;		//## The number of elements in the array.
		int16		elementSize;		//## The size of each element in the array, in bytes. 
		int16		componentCount;		//## The number of vector components used by each element in the array.
	};
	
	void Reverse(ArrayDescriptor *desc); 
	
	
	struct ArrayBundle
	{ 
		ArrayDescriptor		descriptor;
		void				*pointer;
		
		unsigned_int32 GetArraySize(void) const
		{
			return (descriptor.elementCount * descriptor.elementSize);
		}
	};
	
	
	struct SegmentData
	{
		unsigned_int32		materialIndex;
		int32				faceStart;
		int32				faceCount;
	};
	
	void Reverse(SegmentData *sd);
	
	
	struct BoneWeight
	{
		int32			boneIndex;
		float			weight;
	};
	
	void Reverse(BoneWeight *bw);
	
	
	struct WeightedVertex
	{
		int32			boneCount;
		BoneWeight		boneWeight[1];
		
		unsigned_int32 GetSize(void) const
		{
			return (sizeof(WeightedVertex) + sizeof(BoneWeight) * (boneCount - 1));
		}
	};
	
	
	struct SkinData
	{
		int32					boneCount;
		const unsigned_int32	*nodeHashArray;
		const Transform4D		*inverseBindTransformArray;
		const WeightedVertex	*const *weightDataTable;
	};
	
	
	class C4_API GeometryVertex : public ListElement<GeometryVertex>, public Memory<GeometryVertex>
	{
		public:
			
			Point3D			position;
			Vector3D		normal;
			Vector4D		tangent;
			ColorRGBA		color;
			Point2D			texcoord[kMaxGeometryTexcoordCount];
			int32			skinIndex;
			
			GeometryVertex();
			~GeometryVertex();
	};
	
	
	class C4_API GeometryPolygon : public ListElement<GeometryPolygon>, public Memory<GeometryVertex>
	{
		public:
			
			List<GeometryVertex>	vertexList;
			
			GeometryPolygon();
			~GeometryPolygon();
	};
	
	
	class C4_API GeometrySurface : public ListElement<GeometrySurface>, public Memory<GeometryVertex>
	{
		public:
			
			unsigned_int32			surfaceFlags;
			int32					texcoordCount;
			
			List<GeometryPolygon>	polygonList;
			
			GeometrySurface(unsigned_int32 flags = 0);
			~GeometrySurface();
	};
	
	
	class C4_API GeometryOctree : public Octree<GeometryOctree>, public Memory<GeometryOctree>
	{
		private:
			
			Point3D					octreeCenter;
			Vector3D				octreeSize;
			Array<unsigned_int32>	indexArray;
			
			int32 ClassifyPoint(const Vector3D& p) const;
			int32 ClassifySphere(const Point3D& p, float r) const;
		
		public:
			
			GeometryOctree();
			GeometryOctree(const Box3D& bounds);
			GeometryOctree(const Point3D& center, const Vector3D& size);
			GeometryOctree(const GeometryOctree *octree, int32 subnodeIndex);
			~GeometryOctree();
			
			const Point3D& GetCenter(void) const
			{
				return (octreeCenter);
			}
			
			void SetCenter(const Point3D& center)
			{
				octreeCenter = center;
			}
			
			const Vector3D& GetSize(void) const
			{
				return (octreeSize);
			}
			
			void SetSize(const Vector3D& size)
			{
				octreeSize = size;
			}
			
			int32 GetIndexCount(void) const
			{
				return (indexArray.GetElementCount());
			}
			
			const unsigned_int32 *GetIndexArray(void) const
			{
				return (indexArray);
			}
			
			void AddIndex(unsigned_int32 index)
			{
				indexArray.AddElement(index);
			}
			
			void Purge(void);
			
			GeometryOctree *FindNodeContainingPoint(const Vector3D& p, int32 maxDepth = 12);
			GeometryOctree *FindNodeContainingSphere(const Point3D& p, float r, int32 maxDepth = 12);
			GeometryOctree *FindNodeContainingEdge(const Vector3D& p1, const Vector3D& p2, int32 maxDepth = 12);
			GeometryOctree *FindNodeContainingTriangle(const Vector3D& p1, const Vector3D& p2, const Vector3D& p3, int32 maxDepth = 12);
	};
	
	
	//# \class	GeometryLevel	Encapsulates data for a single geometrical level of detail.
	//
	//# The $GeometryObject$ class encapsulates data for a single geometrical level of detail.
	//
	//# \def	class GeometryLevel : public Packable
	//
	//# \ctor	GeometryLevel();
	//
	//# \desc
	//
	//# \base	ResourceMgr/Packable	Geometry levels can be packed for storage in resources.
	//
	//# \also	$@GeometryObject@$
	
	
	//# \function	GeometryLevel::GetVertexCount		Returns the number of vertices in a geometry level.
	//
	//# \proto	int32 GetVertexCount(void) const;
	//
	//# \desc
	//# The $GetVertexCount$ function returns the number of vertices in a geometry level.
	//
	//# \also	$@GeometryLevel::GetFaceCount@$
	
	
	//# \function	GeometryLevel::GetFaceCount		Returns the number of faces in a geometry level.
	//
	//# \proto	int32 GetFaceCount(void) const;
	//
	//# \desc
	//# The $GetFaceCount$ function returns the number of faces (triangles) in a geometry level. This is equivalent
	//# to the number of elements in the array with identifier $kArrayFace$.
	//
	//# \also	$@GeometryLevel::GetVertexCount@$
	
	
	//# \function	GeometryLevel::GetArray		Returns a pointer to one of the geometrical arrays stored in a geometry level.
	//
	//# \proto	void *GetArray(int32 index) const;
	//# \proto	template <typename type> type *GetArray<float>(int32 index) const;
	//
	//# \desc
	//# The $GetArray$ function and associated template function return a pointer to one of the geometrical arrays stored in a geometry level.
	//# Each of the functions returns a pointer representing the same address, but with a different type. Most arrays can
	//# only contain data of one type, but some can have different types for different geometries.
	//
	//# \also	$@GeometryLevel::GetArrayDescriptor@$
	
	
	//# \function	GeometryLevel::GetArrayDescriptor	Returns the descriptor for one of the geometrical arrays stored in a geometry level.
	//
	//# \proto	const ArrayDescriptor *GetArrayDescriptor(int32 index) const;
	//
	//# \desc
	//# The $GetArrayDescriptor$ function returns the descriptor for one of the geometrical arrays stored in a geometry level.
	//# See the $@ArrayDescriptor@$ structure.
	//
	//# \also	$@ArrayDescriptor@$
	//# \also	$@GeometryLevel::GetArray@$
	
	
	//# \function	GeometryLevel::AllocateStorage		Allocates memory for the contents of a geometry level.
	//
	//# \proto	void AllocateStorage(int32 vertCount, int32 arrayCount, const ArrayDescriptor *arrayDesc,
	//# \proto2	unsigned_int32 weightSize = 0);
	//# \proto	void AllocateStorage(const GeometryLevel *inputLevel, int32 arrayCount, const ArrayDescriptor *arrayDesc,
	//# \proto2	unsigned_int32 weightSize = 0);
	//
	//# \param	vertCount		The new vertex count.
	//# \param	arrayCount		The number of arrays to allocate space for.
	//# \param	arrayDesc		A pointer to an array (of size $arrayCount$) of array descriptors.
	//# \param	weightSize		The size of the weighting data, in bytes.
	//# \param	inputLevel		The input geometry level from which data is copied for arrays not being replaced. This may not be the same object for which $AllocateStorage$ is called.
	//
	//# \desc
	//# The $AllocateStorage$ function allocates space for all of the geometrical data stored in a geometry level.
	//# There are two variants of this function, and both take an array of $@ArrayDescriptor@$ records describing
	//# what types of arrays memory needs to be allocated for. The $arrayDesc$ parameter should point to an array
	//# having the size specified by the $arrayCount$ parameter.
	//# 
	//# If the function taking the $vertCount$ parameter is called, then memory is only allocated for the arrays
	//# specified by the $arrayDesc$ array. Any data previously existing in the geometry level is deleted, and the
	//# newly allocated space is uninitialized.
	//# 
	//# If the function taking the $inputLevel$ parameter is called, then memory is allocated for the arrays specified
	//# by the $arrayDesc$ array in addition to any other arrays existing in the geometry level specified by $inputLevel$.
	//# In this case, the new (or replaced) arrays specified by the $arrayDesc$ array are uninitialized, but data for all
	//# other arrays is copied from the input geometry level.
	//
	//# \also	$@ArrayDescriptor@$
	
	
	//# \function	GeometryLevel::CopyGeometryLevel	Copies the contents of a geometry level.
	//
	//# \proto	void CopyGeometryLevel(const GeometryLevel *geometryLevel, unsigned_int32 exclusionMask = 0);
	//
	//# \param	geometryLevel	The input geometry level.
	//# \param	exclusionMask	A bit mask indicating which arrays should be excluded from the copy.
	//
	//# \desc
	//# The $CopyGeometryLevel$ function copies the contents of the geometry level specified by the $geometryLevel$
	//# parameter to the geometry level for which this function is called (the output level). The previous contents
	//# of the output geometry level are deleted.
	//#
	//# If the $exclusionMask$ parameter is not zero, then the position of the set bits correspond to the indexes
	//# of arrays that are excluded from the copy. For example, to prevent the edge array from being copied, the
	//# $exclusionMask$ parameter should be $1 << kArrayEdge$.
	//
	//# \also	$@GeometryLevel::CopyRigidGeometryLevel@$
	
	
	//# \function	GeometryLevel::CopyRigidGeometryLevel	Copies the contents of a geometry level and removes skinning data.
	//
	//# \proto	void CopyRigidGeometryLevel(const GeometryLevel *geometryLevel, unsigned_int32 exclusionMask = 0);
	//
	//# \param	geometryLevel	The input geometry level.
	//# \param	exclusionMask	A bit mask indicating which arrays should be excluded from the copy.
	//
	//# \desc
	//# The $CopyRigidGeometryLevel$ function copies the contents of the geometry level specified by the $geometryLevel$
	//# parameter to the geometry level for which this function is called (the output level). The previous contents
	//# of the output geometry level are deleted.
	//# 
	//# If the input geometry level contains skinning data, then it is removed during the copy operation. The removed
	//# skinning data consists of the vertex weighting data, the bone hash array, and the inverse bind transform array.
	//#
	//# If the $exclusionMask$ parameter is not zero, then the position of the set bits correspond to the indexes
	//# of arrays that are excluded from the copy. For example, to prevent the edge array from being copied, the
	//# $exclusionMask$ parameter should be $1 << kArrayEdge$. The bone hash array and inverse bind transform array
	//# are always excluded regardless of the value of the $exclusionMask$ parameter.
	//
	//# \also	$@GeometryLevel::CopyGeometryLevel@$
	
	
	//# \function	GeometryLevel::TransformGeometryLevel	Transforms the geometrical data in a geometry level.
	//
	//# \proto	void TransformGeometryLevel(const Transform4D& transform);
	//
	//# \param	transform	The transform to apply to the geometry level.
	//
	//# \desc
	//# The $TransformGeometryLevel$ function transforms the contents of a geometry level by the matrix given by the
	//# $transform$ parameter. This operation affects the vertex array, normal array, and plane array. Any existing
	//# tangent array must be recalculated after this operation is applied.
	//
	//# \also	$@GeometryLevel::TranslateGeometryLevel@$
	//# \also	$@GeometryLevel::ScaleGeometryLevel@$
	
	
	//# \function	GeometryLevel::TranslateGeometryLevel	Translates the geometrical data in a geometry level.
	//
	//# \proto	void TranslateGeometryLevel(const Vector3D& translation);
	//
	//# \param	translation		The translation to apply to the geometry level.
	//
	//# \desc
	//# The $TranslateGeometryLevel$ function translates the contents of a geometry level by the offset vector given by
	//# the $translation$ parameter. This operation affects the vertex array and plane array.
	//
	//# \also	$@GeometryLevel::TransformGeometryLevel@$
	//# \also	$@GeometryLevel::ScaleGeometryLevel@$
	
	
	//# \function	GeometryLevel::ScaleGeometryLevel	Scales the geometrical data in a geometry level.
	//
	//# \proto	void ScaleGeometryLevel(const Vector3D& scale);
	//
	//# \param	scale		The scale to apply to the geometry level.
	//
	//# \desc
	//# The $ScaleGeometryLevel$ function scales the contents of a geometry level by the scaling vector given by
	//# the $scale$ parameter. This operation affects the vertex array, normal array, and plane array. Any existing
	//# tangent array must be recalculated after this operation is applied.
	//
	//# \also	$@GeometryLevel::TransformGeometryLevel@$
	//# \also	$@GeometryLevel::TranslateGeometryLevel@$
	
	
	//# \function	GeometryLevel::InvertGeometryLevel		Inverts the geometrical data in a geometry level.
	//
	//# \proto	void InvertGeometryLevel(void);
	//
	//# \desc
	//# The $InvertGeometryLevel$ function inverts the contents of a geometry level. This operation affects the
	//# normal array, tangent array, face array, edge array, and plane array.
	
	
	//# \function	GeometryLevel::WeldGeometryLevel	Welds the surfaces a geometry level together.
	//
	//# \proto	void WeldGeometryLevel(float epsilon);
	//
	//# \param	epsilon		The distance below which vertices are forced to coincide.
	//
	//# \desc
	//# The $WeldGeometryLevel$ function searches for pairs of vertices that are within the distance $epsilon$
	//# of each other, but belong to different surfaces. When such a pair is found, one of the vertices is moved
	//# so that it coincides exactly with the other vertex in the pair. This operation welds the boundaries between
	//# surfaces together.
	//
	//# \also	$@GeometryLevel::MendGeometryLevel@$
	//# \also	$@GeometryLevel::UnifyGeometryLevel@$
	
	
	//# \function	GeometryLevel::MendGeometryLevel	Mends the vertices in each surface of a geometry level.
	//
	//# \proto	void MendGeometryLevel(float vertexEpsilon, float normalEpsilon, float texcoordEpsilon);
	//
	//# \param	vertexEpsilon		The vertex position difference threshold.
	//# \param	normalEpsilon		The normal vector difference threshold.
	//# \param	texcoordEpsilon		The texture coordinate difference threshold.
	//
	//# \desc
	//# The $MendGeometryLevel$ function searches for pairs of vertices in the same surface that are within the
	//# distance $vertexEpsilon$ of each other, have normal vectors whose dot product is less than $1.0 - normalEpsilon$,
	//# and whose texture coordinates are each within $texcoordEpsilon$ of each other. When such a pair is found, one
	//# of the vertices is changed so that it has exactly the same position, normal, and texture coordinates as the
	//# other vertex. If there is no normal array, then only position and texture coordinates are considered.
	//#
	//# If the input geometry level has a 4D tangent array, then two vertices are considered to be distinct if the <i>w</i> coordinates of the
	//# tangents are not the same or the 3D dot product between the tangents is not positive.
	//# 
	//# The mending operation is ordinarily followed by a call to the $@GeometryLevel::UnifyGeometryLevel@$ function
	//# to remove duplicate vertices.
	//
	//# \also	$@GeometryLevel::WeldGeometryLevel@$
	//# \also	$@GeometryLevel::UnifyGeometryLevel@$
	
	
	//# \function	GeometryLevel::UnifyGeometryLevel	Unifies duplicate vertices in a geometry level.
	//
	//# \proto	void UnifyGeometryLevel(const GeometryLevel *inputLevel);
	//
	//# \param	geometryLevel	The input geometry level.
	//
	//# \desc
	//# The $UnifyGeometryLevel$ function searches for pairs of vertices in the same surface that have identical
	//# positions, normal vectors, colors, and texture coordinates. Duplicates are removed, and index data in the face array
	//# is remapped. If the normal array or color array does not exist, then the unification proceeds without considering normals and/or colors.
	//#
	//# If the input geometry level has a 4D tangent array, then two vertices are considered to be distinct if the <i>w</i> coordinates of the
	//# tangents are not the same or the 3D dot product between the tangents is not positive. When the $UnifyGeometryLevel$ function returns,
	//# the geometry level no longer has a tangent array, so the $@GeometryLevel::BuildTangentArray@$ function must be called to generate
	//# a new tangent array for the unified geometry level.
	//
	//# \also	$@GeometryLevel::WeldGeometryLevel@$
	//# \also	$@GeometryLevel::MendGeometryLevel@$
	//# \also	$@GeometryLevel::BuildTangentArray@$
	
	
	//# \function	GeometryLevel::BuildNormalArray		Builds the array of normal vectors for a geometry level.
	//
	//# \proto	void BuildNormalArray(const GeometryLevel *inputLevel);
	//
	//# \param	geometryLevel	The input geometry level.
	//
	//# \desc
	//# The $BuildNormalArray$ function adds a normal array to a geometry level. The vertex and face information
	//# in the input geometry level is used to a calculate normal vector for each vertex. All other array data
	//# from the input geometry level is copied to the output geometry level.
	//
	//# \also	$@GeometryLevel::BuildTangentArray@$
	//# \also	$@GeometryLevel::BuildPlaneArray@$
	//# \also	$@GeometryLevel::BuildEdgeArray@$
	
	
	//# \function	GeometryLevel::BuildTangentArray		Builds the array of tangent vectors for a geometry level.
	//
	//# \proto	void BuildTangentArray(const GeometryLevel *inputLevel);
	//
	//# \param	geometryLevel	The input geometry level.
	//
	//# \desc
	//# The $BuildTangentArray$ function adds a tangent array to a geometry level. The vertex, normal, texture coordinate,
	//# and face information in the input geometry level is used to a calculate tangent vector and handedness for each
	//# vertex. All other array data from the input geometry level is copied to the output geometry level.
	//
	//# \also	$@GeometryLevel::BuildNormalArray@$
	//# \also	$@GeometryLevel::BuildPlaneArray@$
	//# \also	$@GeometryLevel::BuildEdgeArray@$
	
	
	//# \function	GeometryLevel::BuildPlaneArray		Builds the array of unique planes for a geometry level.
	//
	//# \proto	void BuildPlaneArray(const GeometryLevel *inputLevel);
	//
	//# \param	geometryLevel	The input geometry level.
	//
	//# \desc
	//# The $BuildPlaneArray$ function adds a plane array and a plane index array to a geometry level.
	//# The vertex and face information in the input geometry level is used to determine the set of unique planes
	//# for a geometry level, and these are stored in the plane array. The plane index array maps each face to the
	//# plane that it lies in. All other array data from the input geometry level is copied to the output geometry level.
	//
	//# \also	$@GeometryLevel::BuildEdgeArray@$
	//# \also	$@GeometryLevel::BuildNormalArray@$
	//# \also	$@GeometryLevel::BuildTangentArray@$
	
	
	//# \function	GeometryLevel::BuildEdgeArray		Builds the array of edges for a geometry level.
	//
	//# \proto	void BuildEdgeArray(const GeometryLevel *inputLevel);
	//
	//# \param	geometryLevel	The input geometry level.
	//
	//# \desc
	//# The $BuildEdgeArray$ function adds an edge array to a geometry level. The vertex and face information in the
	//# input geometry level is used to determine the set of all edges for a geometry level, and these are stored in the
	//# edge array. All other array data from the input geometry level is copied to the output geometry level.
	//
	//# \also	$@GeometryLevel::BuildPlaneArray@$
	//# \also	$@GeometryLevel::BuildNormalArray@$
	//# \also	$@GeometryLevel::BuildTangentArray@$
	
	
	class C4_API GeometryLevel : public Packable
	{
		friend class GeometryObject;
		
		private:
			
			char					*levelStorage;
			ArrayBundle				arrayBundle[kMaxGeometryArrayCount];
			
			int32					vertexCount;
			int32					attributeArrayCount;
			unsigned_int8			attributeArrayIndex[kMaxAttributeArrayCount];
			
			unsigned_int32			faceOffset;
			unsigned_int32			attributeOffset[kMaxAttributeArrayCount];
			
			unsigned_int32			weightDataSize;
			WeightedVertex			*weightData;
			
			static GeometryArrayType GetArrayType(const ArrayDescriptor *desc, int32 *count);
			
			static bool TangentsSimilar(const Vector4D& t1, const Vector4D& t2);
			static bool GetTexcoordTransform(const TextureAlignData *alignData, const Transformable *transformable, Antivector4D *plane);
		
		public:
			
			GeometryLevel();
			~GeometryLevel();
			
			const ArrayBundle *GetArrayBundle(int32 index) const
			{
				return (&arrayBundle[index]);
			}
			
			const ArrayDescriptor *GetArrayDescriptor(int32 index) const
			{
				return (&arrayBundle[index].descriptor);
			}
			
			void *GetArray(int32 index) const
			{
				return (arrayBundle[index].pointer);
			}
			
			template <typename type> type *GetArray(int32 index) const
			{
				return (static_cast<type *>(arrayBundle[index].pointer));
			}
			
			int32 GetVertexCount(void) const
			{
				return (vertexCount);
			}
			
			int32 GetFaceCount(void) const
			{
				return (arrayBundle[kArrayFace].descriptor.elementCount);
			}
			
			int32 GetAttributeArrayCount(void) const
			{
				return (attributeArrayCount);
			}
			
			const unsigned_int8 *GetAttributeArrayIndex(void) const
			{
				return (attributeArrayIndex);
			}
			
			unsigned_int32 GetFaceOffset(void) const
			{
				return (faceOffset);
			}
			
			unsigned_int32 GetAttributeOffset(int32 index) const
			{
				return (attributeOffset[index]);
			}
			
			unsigned_int32 GetWeightDataSize(void) const
			{
				return (weightDataSize);
			}
			
			WeightedVertex *GetWeightData(void) const
			{
				return (weightData);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			void AllocateStorage(int32 vertCount, int32 arrayCount, const ArrayDescriptor *arrayDesc, unsigned_int32 weightSize = 0);
			void AllocateStorage(const GeometryLevel *inputLevel, int32 arrayCount, const ArrayDescriptor *arrayDesc, unsigned_int32 weightSize = 0);
			
			void CopyGeometryLevel(const GeometryLevel *geometryLevel, unsigned_int32 exclusionMask = 0);
			void CopyRigidGeometryLevel(const GeometryLevel *geometryLevel, unsigned_int32 exclusionMask = 0);
			
			void TransformGeometryLevel(const Transform4D& transform);
			void TranslateGeometryLevel(const Vector3D& translation);
			void ScaleGeometryLevel(const Vector3D& scale);
			void InvertGeometryLevel(void);
			
			void WeldGeometryLevel(float epsilon);
			void MendGeometryLevel(float vertexEpsilon, float normalEpsilon, float texcoordEpsilon);
			void UnifyGeometryLevel(const GeometryLevel *inputLevel);
			
			bool GenerateTexcoords(const Transformable *transformable, const GeometryObject *object);
			bool TransformTexcoords(const Transformable *transformable, const GeometryObject *object);
			
			void CalculateEdgeArray(const unsigned_int32 *remapTable);
			
			void BuildNormalArray(const GeometryLevel *inputLevel);
			void CalculateNormalArray(const GeometryLevel *inputLevel = nullptr);
			
			void BuildTangentArray(const GeometryLevel *inputLevel);
			void CalculateTangentArray(const GeometryLevel *inputLevel = nullptr);
			
			void BuildPlaneArray(const GeometryLevel *inputLevel);
			void BuildEdgeArray(const GeometryLevel *inputLevel);
			void BuildSegmentArray(const GeometryLevel *inputLevel, int32 surfaceCount, const SurfaceData *surfaceData);
			void BuildTexcoordArray(const GeometryLevel *inputLevel, const Transformable *transformable, const GeometryObject *object);
			
			void SimplifyBoundaryEdges(const GeometryLevel *inputLevel);
			void OptimizeMesh(const GeometryLevel *inputLevel, float collapseThreshold, int32 baseTriangleCount = -1);
			
			float CalculateVolume(void) const;
	};
}


#endif

// ZYURVUR
