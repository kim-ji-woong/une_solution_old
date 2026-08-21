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


#ifndef C4Terrain_h
#define C4Terrain_h


//# \component	World Manager
//# \prefix		WorldMgr/


#include "C4Voxels.h"
#include "C4Geometries.h"


namespace C4
{
	enum
	{
		kTerrainLogDimension	= 4,
		kTerrainDimension		= 1 << kTerrainLogDimension
	};
	
	
	enum
	{
		kMaxTerrainMaterialCount		= 255,
		kTerrainMaterialUsageTableSize	= 256,
		kDeadTerrainMaterialIndex		= 0xFF
	};
	
	
	enum
	{
		kTerrainChannelDensity,
		kTerrainChannelBlend,
		kTerrainChannelMaterial,
		kTerrainChannelCount
	};
	
	
	enum
	{
		kObjectTerrainBlock		= 'TBLK'
	};
	
	
	enum
	{
		kNodeTerrainBlock		= 'TBLK'
	};
	
	
	enum
	{
		kGeometryTerrain		= 'TERR'
	};
	
	
	class TerrainGeometry;
	
	
	struct TerrainBorderRenderData
	{
		int32				baseTriangleCount;
		unsigned_int32		multiRenderMask;
		int32				multiRenderData[12];
	};
	
	
	struct TerrainMaterial
	{
		Color3C		primaryMaterial;
		Color3C		secondaryMaterial;
	};
	
	
	class VoxelBox : public Range<Integer3D>
	{
		public:
			
			VoxelBox() {}
			
			VoxelBox(const Integer3D& pmin, const Integer3D& pmax) : Range<Integer3D>(pmin, pmax)
			{
			}
	};
	
	
	class VoxelMap
	{
		private:
			
			Voxel				*voxelBase;
			
			union
			{
				Voxel			*voxelOrigin;
				UnsignedVoxel	*unsignedVoxelOrigin;
			};
			
			int32				rowSize;
			int32				deckSize;
			
			VoxelBox			blockBounds; 
			VoxelBox			voxelBounds;
		 
		public: 
			 
			C4API VoxelMap(const Integer3D& size, const Integer3D& origin, const VoxelBox& blockBox, const VoxelBox& voxelBox);
			C4API ~VoxelMap(); 
			
			Voxel *GetVoxelDataBase(void) const
			{
				return (voxelBase); 
			}
			
			Voxel *GetVoxelData(void) const
			{ 
				return (voxelOrigin);
			}
			
			UnsignedVoxel *GetUnsignedVoxelData(void) const
			{
				return (unsignedVoxelOrigin);
			}
			
			int32 GetRowSize(void) const
			{
				return (rowSize);
			}
			
			int32 GetDeckSize(void) const
			{
				return (deckSize);
			}
			
			const VoxelBox& GetBlockBounds(void) const
			{
				return (blockBounds);
			}
			
			const VoxelBox& GetVoxelBounds(void) const
			{
				return (voxelBounds);
			}
			
			Voxel GetVoxel(int32 x, int32 y, int32 z) const
			{
				x = Min(Max(x, voxelBounds.min.x), voxelBounds.max.x);
				y = Min(Max(y, voxelBounds.min.y), voxelBounds.max.y);
				z = Min(Max(z, voxelBounds.min.z), voxelBounds.max.z);
				return (voxelOrigin[z * deckSize + y * rowSize + x]);
			}
			
			UnsignedVoxel GetUnsignedVoxel(int32 x, int32 y, int32 z) const
			{
				x = Min(Max(x, voxelBounds.min.x), voxelBounds.max.x);
				y = Min(Max(y, voxelBounds.min.y), voxelBounds.max.y);
				z = Min(Max(z, voxelBounds.min.z), voxelBounds.max.z);
				return (unsignedVoxelOrigin[z * deckSize + y * rowSize + x]);
			}
			
			void SetVoxel(int32 x, int32 y, int32 z, Voxel v) const
			{
				voxelOrigin[z * deckSize + y * rowSize + x] = v;
			}
			
			void SetUnsignedVoxel(int32 x, int32 y, int32 z, UnsignedVoxel v) const
			{
				unsignedVoxelOrigin[z * deckSize + y * rowSize + x] = v;
			}
			
			C4API Voxel GetFilteredVoxel(int32 x, int32 y, int32 z) const;
			C4API Voxel GetMinHorizontalVoxel(int32 x, int32 y, int32 z, Fixed bias) const;
			C4API Voxel GetMaxHorizontalVoxel(int32 x, int32 y, int32 z, Fixed bias) const;
			C4API void SetUnsignedVoxelOctet(int32 x, int32 y, int32 z, UnsignedVoxel v) const;
			
			Vector3D CalculateNormal(const Integer3D& coord) const;
			Vector3D CalculateNormal(const Integer3D& coord1, const Integer3D& coord2, Fixed t, Fixed u) const;
	};
	
	
	class TerrainStorage
	{
		protected:
			
			Integer3D		blockSize;
			char			**voxelData[kTerrainChannelCount];
			
			static Heap		terrainHeap;
			
			TerrainStorage();
			TerrainStorage(const Integer3D& size);
			
			void ClearBlock(void);
			void SetNewBlockSize(const Integer3D& size);
			void CopyNewBlock(const TerrainStorage *storage);
		
		public:
			
			TerrainStorage(const TerrainStorage& terrainStorage);
			~TerrainStorage();
			
			const Integer3D& GetBlockSize(void) const
			{
				return (blockSize);
			}
			
			C4API void SetBlockSize(const Integer3D& size);
			C4API void CopyBlock(const TerrainStorage *storage);
			
			C4API static void DeleteVoxelData(char *data);
	};
	
	
	class TerrainBlockObject : public Object
	{
		friend class WorldMgr;
		
		private:
			
			float		voxelScale;
			
			TerrainBlockObject();
			~TerrainBlockObject();
		
		public:
			
			TerrainBlockObject(float scale);
			
			float GetVoxelScale(void) const
			{
				return (voxelScale);
			}
			
			void SetVoxelScale(float scale)
			{
				voxelScale = scale;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetObjectSize(float *size) const;
			void SetObjectSize(const float *size);
	};
	
	
	class TerrainBlock : public Node, public TerrainStorage
	{
		friend class Node;
		
		private:
			
			bool				materialUsage[kTerrainMaterialUsageTableSize];
			TerrainMaterial		terrainMaterial[kMaxTerrainMaterialCount];
			
			TerrainBlock();
			TerrainBlock(const TerrainBlock& terrainBlock);
			
			Node *Replicate(void) const override;
			
			bool CalculateBoundingBox(Box3D *box) const override;
			
			static int32 CountConstantValues(const Voxel *voxel, int32 rowSize, int32 deckSize, int32 i, int32 j, int32 k);
			static int32 CountVaryingValues(const Voxel *voxel, int32 rowSize, int32 deckSize, int32 i, int32 j, int32 k);
			
			static unsigned_int32 CompressVoxelData(const Voxel *voxel, int32 rowSize, int32 deckSize, char *data);
			static void DecompressVoxelData(const char *data, Voxel *voxel, int32 rowSize, int32 deckSize);
			
			C4API static TerrainGeometry *FindTerrainGeometry(const Node *root, const Integer3D& coord, int32 level);
			static Node *FindTerrainSuperNode(Node *root, const Integer3D& coord, int32 level);
		
		public:
			
			C4API TerrainBlock(const Integer3D& size, float scale, const TerrainMaterial *material);
			C4API ~TerrainBlock();
			
			TerrainBlockObject *GetObject(void) const
			{
				return (static_cast<TerrainBlockObject *>(Node::GetObject()));
			}
			
			Vector3D GetBlockBoxSize(void) const
			{
				float scale = GetObject()->GetVoxelScale();
				return (Vector3D((float) (blockSize.x * kTerrainDimension) * scale, (float) (blockSize.y * kTerrainDimension) * scale, (float) (blockSize.z * kTerrainDimension) * scale));
			}
			
			const TerrainMaterial *GetTerrainMaterial(int32 index) const
			{
				return (&terrainMaterial[index]);
			}
			
			TerrainGeometry *FindTerrainGeometry(const Integer3D& coord, int32 level = 0) const
			{
				return (FindTerrainGeometry(this, coord, level));
			}
			
			Node *FindTerrainSuperNode(const Integer3D& coord, int32 level = 0)
			{
				return (FindTerrainSuperNode(this, coord, level));
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			void Preprocess(void);
			
			C4API void ResizeBlock(const Integer3D& size, const Integer3D& offset);
			
			C4API void LoadVoxels(int32 channel, const char *const *channelData, const Integer3D& coord, Voxel *voxel, int32 rowSize, int32 deckSize) const;
			C4API void StoreVoxels(int32 channel, const Integer3D& coord, const Voxel *voxel, int32 rowSize, int32 deckSize);
			
			VoxelMap *OpenBuildVoxelMap(int32 channel, const Integer3D& coord, int32 level = 0) const;
			static void CloseBuildVoxelMap(VoxelMap *voxelMap);
			
			C4API VoxelMap *OpenVoxelMap(int32 channel, const Integer3D& vmin, const Integer3D& vmax) const;
			C4API void CloseVoxelMap(int32 channel, VoxelMap *voxelMap);
			
			C4API char *SaveVoxelData(int32 channel, const Integer3D& coord) const;
			C4API void RestoreVoxelData(int32 channel, const Integer3D& coord, char *data);
			
			C4API void SetBlockToHorizontalPlane(int32 planeZ);
			C4API void SetBlockToHeightField(const unsigned_int8 *heightField, int32 heightScale, int32 heightOffset);
			C4API void SetBlockToHeightField(const unsigned_int16 *heightField, int32 heightScale, int32 heightOffset);
			
			C4API void AddTerrainGeometry(TerrainGeometry *terrainGeometry);
			C4API void ProcessStructure(void);
			
			C4API int32 GetTerrainMaterialIndex(const TerrainMaterial *material);
			C4API void OptimizeTerrainMaterials(void);
	};
	
	
	class TerrainGeometryObject : public GeometryObject
	{
		friend class GeometryObject;
		
		private:
			
			Integer3D		geometryCoord;
			int32			detailLevel;
			
			Box3D			boundingBox;
			Vector4D		texcoordParameter;
			
			SurfaceData		staticSurfaceData[1];
		
		protected:
			
			enum
			{
				kMaxTerrainTriangleCount	= kTerrainDimension * kTerrainDimension * 32,
				kMaxTerrainVertexCount		= kMaxTerrainTriangleCount
			};
			
			typedef unsigned_int16 VoxelStorage[4];
			typedef VoxelStorage DeckStorage[kTerrainDimension][kTerrainDimension];
			
			struct BuildVertex
			{
				Fixed3D					position0;
				Fixed3D					position1;
				Vector3D				normal;
				Color4C					border;
				
				UnsignedVoxel			blend;
				UnsignedVoxel			material;
				unsigned_int8			statusFlags;
				
				mutable unsigned_int16	nextIndex;
				unsigned_int16			remapIndex;
			};
			
			struct BuildTriangle
			{
				Triangle				triangle;
				bool					inclusionFlag;
			};
			
			struct BuildStorage
			{
				BuildVertex				vertexStorage[kMaxTerrainVertexCount];
				BuildTriangle			triangleStorage[kMaxTerrainTriangleCount];
				DeckStorage 			deckStorage[2];
			};
			
			TerrainGeometryObject();
			TerrainGeometryObject(const Integer3D& coord, int32 level);
			~TerrainGeometryObject();
			
			void SetTexcoordParameter(float x, float y, float z, float w)
			{
				texcoordParameter.Set(x, y, z, w);
			}
			
			static unsigned_int16 AddSharedVertex(const BuildVertex *sharedVertex, UnsignedVoxel material, BuildVertex *vertexStorage, int32& globalVertexCount);
		
		public:
			
			TerrainGeometryObject(const Integer3D& coord);
			
			const Integer3D& GetGeometryCoord(void) const
			{
				return (geometryCoord);
			}
			
			void SetGeometryCoord(const Integer3D& coord)
			{
				geometryCoord = coord;
			}
			
			int32 GetDetailLevel(void) const
			{
				return (detailLevel);
			}
			
			Box3D& GetBoundingBox(void)
			{
				return (boundingBox);
			}
			
			const Box3D& GetBoundingBox(void) const
			{
				return (boundingBox);
			}
			
			const Vector4D *GetTexcoordParameter(void) const
			{
				return (&texcoordParameter);
			}
			
			void PackType(Packer& data) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			bool ExteriorSphere(const Point3D& center, float radius) const;
			bool ExteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const;
			
			virtual void Build(Geometry *geometry);
	};
	
	
	class TerrainLevelGeometryObject : public TerrainGeometryObject
	{
		friend class GeometryObject;
		friend class TerrainGeometryObject;
		
		private:
			
			typedef unsigned_int16 BorderVoxelStorage[10];
			typedef BorderVoxelStorage RowStorage[kTerrainDimension];
			
			struct LevelBuildStorage : BuildStorage
			{
				RowStorage	 	rowStorage[2];
			};
			
			TerrainBorderRenderData		borderRenderData;
			
			TerrainLevelGeometryObject();
			~TerrainLevelGeometryObject();
			
			static void CalculateSecondaryPosition(int32 level, BuildVertex *buildVertex);
			static void FindSurfaceCrossingEdge(const VoxelMap *densityMap, const VoxelMap *blendMap, int32 level, Integer3D& coord0, Integer3D& coord1, int32& d0, int32& d1, UnsignedVoxel& b0, UnsignedVoxel& b1);
			static bool NontrivialCell(const VoxelMap *densityMap, machine x, machine y, machine z, machine dv);
			static bool FindBestMaterial(const VoxelMap *densityMap, const VoxelMap *materialMap, machine x, machine y, machine z, machine dv, UnsignedVoxel *material);
			static int32 ChooseTriangulation(int32 cellClass, const VoxelMap *densityMap, machine x, machine y, machine z, machine dv, const unsigned_int16 *globalVertexIndex, const BuildVertex *buildVertex);
			static unsigned_int16 AddSharedVertex(const BuildVertex *sharedVertex, UnsignedVoxel material, BuildVertex *vertexStorage, int32& globalVertexCount);
			
			void BuildMipBorder(int32 face, const VoxelMap *densityMap, const VoxelMap *blendMap, const VoxelMap *materialMap, int32& globalVertexCount, int32& globalTriangleCount, LevelBuildStorage *buildStorage);
		
		public:
			
			TerrainLevelGeometryObject(const Integer3D& coord, int32 level);
			
			int32 GetBaseTriangleCount(void) const
			{
				return (borderRenderData.baseTriangleCount);
			}
			
			unsigned_int32 GetMultiRenderMask(void) const
			{
				return (borderRenderData.multiRenderMask);
			}
			
			const int32 *GetMultiRenderData(void) const
			{
				return (borderRenderData.multiRenderData);
			}
			
			void PackType(Packer& data) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			C4API void SaveBorderRenderData(TerrainBorderRenderData *data) const;
			C4API void RestoreBorderRenderData(const TerrainBorderRenderData *data);
			C4API void OffsetBorderRenderData(int32 delta);
			
			void Build(Geometry *geometry);
	};
	
	
	class TerrainGeometry : public Geometry
	{
		friend class Geometry;
		
		private:
			
			TerrainBlock		*blockNode;
			
			Point3D				worldCenter;
			float				renderDistance;
			
			Node *Replicate(void) const override;
			
			void CalculatePostTransform(void) override;
			bool CalculateBoundingBox(Box3D *box) const override;
			bool CalculateBoundingSphere(BoundingSphere *sphere) const override;
		
		protected:
			
			TerrainGeometry();
			TerrainGeometry(TerrainBlock *block, const Integer3D& coord, int32 level);
			TerrainGeometry(const TerrainGeometry& terrainGeometry);
		
		public:
			
			C4API TerrainGeometry(TerrainBlock *block, const Integer3D& coord);
			C4API ~TerrainGeometry();
			
			TerrainGeometryObject *GetObject(void) const
			{
				return (static_cast<TerrainGeometryObject *>(Node::GetObject()));
			}
			
			TerrainBlock *GetBlockNode(void) const
			{
				return (blockNode);
			}
			
			const Point3D& GetWorldCenter(void) const
			{
				return (worldCenter);
			}
			
			float GetRenderDistance(void) const
			{
				return (renderDistance);
			}
			
			void PackType(Packer& data) const;
			
			void Preprocess(void);
	};
	
	
	class TerrainLevelGeometry : public TerrainGeometry, public ListElement<TerrainLevelGeometry>
	{
		friend class Geometry;
		friend class TerrainGeometry;
		
		private:
			
			Vector3D		defaultBorderState[2];
			Vector4D		terrainBorderState[2];
			
			Link<Node>		superTerrain;
			Link<Node>		neighborTerrain[6];
			
			Box3D			terrainBounds;
			
			TerrainLevelGeometry();
			TerrainLevelGeometry(const TerrainLevelGeometry& terrainLevelGeometry);
			
			Node *Replicate(void) const override;
			
			static void TerrainLinkProc(Node *node, void *cookie);
			
			bool CalculateBoundingBox(Box3D *box) const override;
			
			float GetRenderStatus(void) const;
		
		public:
			
			C4API TerrainLevelGeometry(TerrainBlock *block, const Integer3D& coord, int32 level);
			C4API ~TerrainLevelGeometry();
			
			using ListElement<TerrainLevelGeometry>::Previous;
			using ListElement<TerrainLevelGeometry>::Next;
			
			TerrainLevelGeometryObject *GetObject(void) const
			{
				return (static_cast<TerrainLevelGeometryObject *>(Node::GetObject()));
			}
			
			bool Rendering(void) const
			{
				return (ListElement<TerrainLevelGeometry>::GetOwningList() != nullptr);
			}
			
			Node *GetNeighborTerrain(int32 index) const
			{
				return (neighborTerrain[index]);
			}
			
			void PackType(Packer& data) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			void Preprocess(void);
			
			C4API void ProcessStructure(void);
			void UpdateBorderState(void);
	};
}


#endif

// ZYURVUR
