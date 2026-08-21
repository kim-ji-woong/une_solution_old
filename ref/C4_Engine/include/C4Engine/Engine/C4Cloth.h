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


#ifndef C4Cloth_h
#define C4Cloth_h


//# \component	Physics Manager
//# \prefix		PhysicsMgr/


#include "C4Primitives.h"
#include "C4Controller.h"


namespace C4
{
	enum
	{
		kControllerCloth	= 'CLTH'
	};
	
	
	enum
	{
		kPrimitiveCloth		= 'CLTH'
	};
	
	
	enum
	{
		kMaxClothSize		= 44
	};
	
	
	enum
	{
		kClothLowerLeftCorner		= 1 << 0,
		kClothLowerRightCorner		= 1 << 1,
		kClothUpperRightCorner		= 1 << 2,
		kClothUpperLeftCorner		= 1 << 3,
		kClothBottomEdge			= 1 << 4,
		kClothRightEdge				= 1 << 5,
		kClothTopEdge				= 1 << 6,
		kClothLeftEdge				= 1 << 7
	};
	
	
	C4API extern const char kConnectorKeyWind[];
	
	
	class PhysicsController;
	class Field;
		
	
	//# \class	ClothGeometryObject		Encapsulates data pertaining to a cloth primitive.
	//
	//# The $ClothGeometryObject$ class encapsulates data pertaining to a cloth primitive.
	//
	//# \def	class ClothGeometryObject : public PrimitiveGeometryObject
	//
	//# \ctor	ClothGeometryObject(const Vector2D& size, int32 width, int32 height);
	//
	//# \param	size	The size of the cloth.
	//# \param	width	The width of the cloth lattice.
	//# \param	height	The height of the cloth lattice.
	//
	//# \desc
	//
	//# \base	WorldMgr/PrimitiveGeometryObject	A $ClothGeometryObject$ is an object that can be shared by multiple cloth geometry nodes.
	//
	//# \also	$@ClothGeometry@$
	//# \also	$@ClothController@$
	
	
	//# \function	ClothGeometryObject::GetClothSize		Returns the cloth size.
	//
	//# \proto	const Vector2D& GetClothSize(void) const;
	//
	//# \desc
	//
	//# \also	$@ClothGeometryObject::SetClothSize@$
	
	
	//# \function	ClothGeometryObject::SetClothSize		Sets the cloth size.
	//
	//# \proto	void SetClothSize(const Vector2D& size);
	//
	//# \param	size	The new cloth size.
	//
	//# \desc
	//
	//# \also	$@ClothGeometryObject::GetClothSize@$
	
	
	class ClothGeometryObject : public PrimitiveGeometryObject
	{
		friend class PrimitiveGeometryObject;
		
		private:
			
			Vector2D		clothSize;
			
			unsigned_int32	flexibilityFlags;
			float			*clothFlexibility; 
			
			SurfaceData		staticSurfaceData[2]; 
			 
			ClothGeometryObject(); 
			~ClothGeometryObject();
			 
			void UpdateFlexibility(void);
		
		public:
			 
			ClothGeometryObject(const Vector2D& size, int32 width, int32 height);
			
			const Vector2D& GetClothSize(void) const
			{ 
				return (clothSize);
			}
			
			void SetClothSize(const Vector2D& size)
			{
				clothSize = size;
			}
			
			int32 GetFieldWidth(void) const
			{
				return (GetMaxSubdivX() + 1);
			}
			
			int32 GetFieldHeight(void) const
			{
				return (GetMaxSubdivY() + 1);
			}
			
			unsigned_int32 GetFlexibilityFlags(void) const
			{
				return (flexibilityFlags);
			}
			
			void SetFlexibilityFlags(unsigned_int32 flags)
			{
				flexibilityFlags = flags;
				UpdateFlexibility();
			}
			
			float *GetClothFlexibility(void) const
			{
				return (clothFlexibility);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
			
			int32 GetObjectSize(float *size) const;
			void SetObjectSize(const float *size);
			
			void Build(Geometry *geometry);
			
			C4API void SetFieldSize(int32 width, int32 height);
	};
	
	
	//# \class	ClothGeometry		Represents a cloth primitive node in a world.
	//
	//# The $ClothGeometry$ class represents a cloth primitive node in a world.
	//
	//# \def	class ClothGeometry : public PrimitiveGeometry
	//
	//# \ctor	ClothGeometry(const Vector2D& size, int32 width, int32 height);
	//
	//# \param	size	The size of the cloth.
	//# \param	width	The width of the cloth lattice.
	//# \param	height	The height of the cloth lattice.
	//
	//# \desc
	//
	//# \base	WorldMgr/PrimitiveGeometry		A cloth is a specific type of primitive geometry.
	//
	//# \also	$@ClothGeometryObject@$
	//# \also	$@ClothController@$
	
	
	class ClothGeometry : public PrimitiveGeometry
	{
		friend class PrimitiveGeometry;
		
		private:
			
			const Point3D	*clothCenter;
			
			ClothGeometry();
			ClothGeometry(const ClothGeometry& clothGeometry);
			
			Node *Replicate(void) const override;
			
			bool CalculateBoundingBox(Box3D *box) const override;
			bool CalculateBoundingSphere(BoundingSphere *sphere) const override;
		
		public:
			
			C4API ClothGeometry(const Vector2D& size, int32 width, int32 height);
			C4API ~ClothGeometry();
			
			ClothGeometryObject *GetObject(void) const
			{
				return (static_cast<ClothGeometryObject *>(Node::GetObject()));
			}
			
			int32 GetInternalConnectorCount(void) const;
			const char *GetInternalConnectorKey(int32 index) const;
			bool ValidConnectedNode(const ConnectorKey& key, const Node *node) const;
			
			void Preprocess(void);
			
			void CalculateInfiniteShadowFrontArray(const Vector3D& lightDirection);
			void CalculatePointShadowFrontArray(const Point3D& lightPosition);
	};
	
	
	//# \class	ClothController		Manages a dynamic cloth surface.
	//
	//# The $ClothController$ class manages a dynamic cloth surface.
	//
	//# \def	class ClothController : public Controller
	//
	//# \ctor	ClothController();
	//
	//# \desc
	//# 
	//
	//# \base	Controller/Controller		A $ClothController$ is a specific type of controller.
	
	
	class ClothController : public Controller
	{
		private:
			
			struct DynamicVertex
			{
				Point3D			vertex;
				Vector3D		velocity;
				Vector3D		normal;
				Vector4D		tangent;
			};
			
			struct SpringData
			{
				unsigned_int16		massIndex1;
				unsigned_int16		massIndex2;
			};
			
			int32					massCount;
			int32					connectCount;
			int32					shearCount;
			int32					bendCount;
			
			float					viscosityConstant;
			float					connectConstant;
			float					shearConstant;
			float					bendConstant;
			
			int32					clothTime;
			int32					updateTime;
			
			ArrayBundle				vertexBundle;
			ArrayBundle				velocityBundle;
			ArrayBundle				normalBundle;
			ArrayBundle				tangentBundle;
			ArrayBundle				planeBundle;
			
			char					*fieldStorage;
			char					*springStorage;
			char					*vertexStorage;
			
			Point3D					*clothPosition[2];
			Vector3D				*clothBitangent;
			
			#if C4SIMD
			
				float4				*clothForce;
			
			#else
			
				Vector3D			*clothForce;
			
			#endif
			
			SpringData				*connectSpring;
			SpringData				*shearSpring;
			SpringData				*bendSpring;
			
			float					connectDistance;
			float					shearDistance;
			float					bendDistance;
			
			PhysicsController		*physicsController;
			const Field				*windForceField;
			float					gravityMultiplier;
			
			BatchJob				clothUpdateJob;

			VertexBuffer							dynamicVertexBuffer;
			VertexBufferObserver<ClothController>	dynamicVertexBufferObserver;

			ClothController(const ClothController& clothController);
			
			Controller *Replicate(void) const override;
			
			void AllocateFieldStorage(void);
			void AllocateSpringStorage(void);
			
			Vector3D CalculateGravityForce(void) const;
			
			void ApplyCellWind(Site *site, Vector3D& wind, unsigned_int32 fieldStamp) const;
			Vector3D CalculateWindForce(void) const;
			
			static void ClothUpdateJob(Job *job, void *cookie);
			static void FinalizeUpdate(Job *job, void *cookie);
			
			void FillDynamicVertexBuffer(VertexBuffer *vertexBuffer);
		
		public:
			
			ClothController();
			~ClothController();
			
			ClothGeometry *GetTargetNode(void) const
			{
				return (static_cast<ClothGeometry *>(Controller::GetTargetNode()));
			}
			
			float GetViscosityConstant(void) const
			{
				return (viscosityConstant);
			}
			
			void SetViscosityConstant(float viscosity)
			{
				viscosityConstant = viscosity;
			}
			
			float GetConnectConstant(void) const
			{
				return (connectConstant);
			}
			
			void SetConnectConstant(float connect)
			{
				connectConstant = connect;
			}
			
			float GetShearConstant(void) const
			{
				return (shearConstant);
			}
			
			void SetShearConstant(float shear)
			{
				shearConstant = shear;
			}
			
			float GetBendConstant(void) const
			{
				return (bendConstant);
			}
			
			void SetBendConstant(float bend)
			{
				bendConstant = bend;
			}
			
			const Point3D *GetClothPosition(void) const
			{
				return (clothPosition[0]);
			}
			
			float GetGravityMultiplier(void) const
			{
				return (gravityMultiplier);
			}
			
			void SetGravityMultiplier(float multiplier)
			{
				gravityMultiplier = multiplier;
			}
			
			static bool ValidNode(const Node *node);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void Preprocess(void);
			void Neutralize(void);
			void Move(void);
			void Update(void);
			
			void SetDetailLevel(int32 level);
	};
}


#endif

// ZYURVUR
