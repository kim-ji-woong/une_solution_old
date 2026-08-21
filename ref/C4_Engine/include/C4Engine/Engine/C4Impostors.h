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


#ifndef C4Impostors_h
#define C4Impostors_h


//# \component	World Manager
//# \prefix		WorldMgr/


#include "C4Node.h"
#include "C4LightObjects.h"


namespace C4
{
	class ImpostorSystem;
	
	
	class ImpostorProperty : public Property
	{
		friend class PropertyReg<ImpostorProperty>;
		
		private:
			
			int32				textureWidth;
			int32				textureHeight;
			
			Type				textureUsage;
			unsigned_int32		impostorFlags;
			
			ResourceName		textureName;
			
			ImpostorProperty();
			ImpostorProperty(const ImpostorProperty& impostorProperty);
			
			Property *Replicate(void) const override;
		
		public:
			
			enum
			{
				kImpostorAlphaChannel		= 1 << 0,
				kImpostorKeepBottom			= 1 << 8,
				kImpostorKeepTop			= 1 << 9
			};
			
			enum
			{
				kImpostorColorMap			= 'COLR',
				kImpostorNormalMap			= 'NRML',
				kImpostorShadowMap			= 'SHAD'
			};
			
			C4API ImpostorProperty(int32 width, int32 height, const char *name);
			C4API ~ImpostorProperty();
			
			int32 GetTextureWidth(void) const
			{
				return (textureWidth);
			}
			
			int32 GetTextureHeight(void) const
			{
				return (textureHeight);
			}
			
			void SetTextureSize(int32 width, int32 height)
			{
				textureWidth = width;
				textureHeight = height;
			}
			
			Type GetTextureUsage(void) const
			{
				return (textureUsage);
			}
			
			void SetTextureUsage(Type usage)
			{
				textureUsage = usage;
			}
			
			unsigned_int32 GetImpostorFlags(void) const
			{
				return (impostorFlags);
			}
			
			void SetImpostorFlags(unsigned_int32 flags)
			{
				impostorFlags = flags;
			}
			
			const ResourceName& GetTextureName(void) const
			{
				return (textureName);
			}
			
			void SetTextureName(const char *name)
			{
				textureName = name;
			}
			
			static bool ValidNode(const Node *node); 
			
			void Pack(Packer& data, unsigned_int32 packFlags) const; 
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags); 
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags); 
			
			int32 GetSettingCount(void) const; 
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
	};
	 
	
	//# \class	Impostor		Represents an impostor node in a world.
	//
	//# The $Impostor$ class represents an impostor node in a world. 
	//
	//# \def	class Impostor : public Node, public ListElement<Impostor>
	//
	//# \ctor	Impostor();
	//
	//# \desc
	//#
	//
	//# \base	Node								An $Impostor$ is a scene graph node.
	//# \base	Utilities/ListElement<Impostor>		Used internally by the World Manager.
	//
	//# \wiki	Impostors
	
	
	class Impostor : public Node, public ListElement<Impostor>
	{
		friend class ImpostorSystem;
		
		private:
			
			MaterialObject		*materialObject;
			
			float				renderDistance;
			float				transitionLength;
			
			float				squaredRenderDistance;
			float				squaredGeometryDistance;
			
			Vector2D			impostorSize;
			Vector4D			renderParameter[2];
			
			ImpostorSystem		*impostorSystem;
			unsigned_int32		impostorIndex;
			
			Impostor(const Impostor& impostor);
			
			Node *Replicate(void) const override;
			
			void CalculateWorldTransform(void) override;
			
			static void MaterialObjectLinkProc(Object *object, void *cookie);
		
		public:
			
			C4API Impostor();
			C4API ~Impostor();
			
			using ListElement<Impostor>::Previous;
			using ListElement<Impostor>::Next;
			
			MaterialObject *GetMaterialObject(void) const
			{
				return (materialObject);
			}
			
			float GetRenderDistance(void) const
			{
				return (renderDistance);
			}
			
			void SetRenderDistance(float distance)
			{
				renderDistance = distance;
			}
			
			float GetTransitionLength(void) const
			{
				return (transitionLength);
			}
			
			void SetTransitionLength(float length)
			{
				transitionLength = length;
			}
			
			float GetSquaredRenderDistance(void) const
			{
				return (squaredRenderDistance);
			}
			
			float GetSquaredGeometryDistance(void) const
			{
				return (squaredGeometryDistance);
			}
			
			void Prepack(List<Object> *linkList) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetCategoryCount(void) const;
			Type GetCategoryType(int32 index, const char **title) const;
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
			
			void Invalidate(void);
			
			void Preprocess(void);
			void Neutralize(void);
			
			C4API void SetMaterialObject(MaterialObject *object);
			
			void Render(void) const;
	};


	class ImpostorSystem : public MapElement<ImpostorSystem>
	{
		friend class Impostor;
		
		private:
			
			struct ImpostorVertex
			{
				Point3D		position;
				Vector3D	direction;
				Point2D		texcoord;
			};
			
			MaterialObject							*materialObject;
			List<Impostor>							impostorList;
			
			Vector4D								renderParameter;
			
			int32									geometryTriangleCount;
			float									geometryRadius[2];
			float									geometryHeight[2];
			
			bool									buildFlag;
			int32									impostorRenderCount;
			Triangle								*impostorTriangle;
			void									(ImpostorSystem::*impostorRenderProc)(const Impostor *);
			
			VertexBuffer							attributeVertexBuffer;
			VertexBufferObserver<ImpostorSystem>	attributeVertexBufferObserver;
			
			VertexBuffer							indexVertexBuffer;
			Renderable								impostorRenderable;
			
			void FillAttributeVertexBuffer(VertexBuffer *vertexBuffer);
			
			void RenderImpostor1(const Impostor *impostor);
			void RenderImpostor2(const Impostor *impostor);
			void RenderImpostor3(const Impostor *impostor);
		
		public:
			
			typedef const MaterialObject *KeyType;
			
			ImpostorSystem(MaterialObject *material, const float *geometryData);
			~ImpostorSystem();
			
			using MapElement<ImpostorSystem>::Previous;
			using MapElement<ImpostorSystem>::Next;
			
			KeyType GetKey(void) const
			{
				return (materialObject);
			}
			
			const MaterialObject *GetMaterialObject(void) const
			{
				return (materialObject);
			}
			
			void AddImpostor(Impostor *impostor)
			{
				impostorList.Append(impostor);
				buildFlag = true;
			}
			
			void RemoveImpostor(Impostor *impostor);
			
			void Build(void);
			void RenderSystem(List<Renderable> *renderList);
	};
	
	
	inline void Impostor::Render(void) const
	{
		(impostorSystem->*impostorSystem->impostorRenderProc)(this);
	}
}


#endif

// ZYURVUR
