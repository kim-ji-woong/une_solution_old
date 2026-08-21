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


#ifndef C4Skybox_h
#define C4Skybox_h


//# \component	World Manager
//# \prefix		WorldMgr/


#include "C4Attributes.h"
#include "C4Cameras.h"


namespace C4
{
	//# \enum	SkyboxFlags
	
	enum
	{
		kSkyboxHorizon			= 1 << 0,		//## The skybox is rendered only above the horizon.
		kSkyboxGlowEnable		= 1 << 1,		//## Glow is enabled for the skybox, and the glow intensity is stored in the alpha channel.
		kSkyboxFogInhibit		= 1 << 2,		//## Fog is not applied to the skybox.
		kSkyboxSmearTexture		= 1 << 3		//## The skybox texture is only mapped to the area above the horizon.
	};
	
	
	class Region;
	
	
	//# \class	Skybox		Represents a skybox node in a world.
	//
	//# The $Skybox$ class represents a skybox node in a world.
	//
	//# \def	class Skybox : public Node
	//
	//# \ctor	Skybox(unsigned_int32 flags = 0);
	//
	//# \param	flags		The skybox flags.
	//
	//# \desc
	//#
	//
	//# \table	MarkerType
	//
	//# \base	Node		A $Skybox$ node is a scene graph node.
	
	
	//# \function	Skybox::GetSkyboxFlags		Returns the skybox flags.
	//
	//# \proto	unsigned_int32 GetSkyboxFlags(void) const;
	//
	//# \desc
	//# The $GetSkyboxFlags$ function returns the skybox flags, which can be a combination (through logical OR) of the following values.
	//
	//# \table	SkyboxFlags
	//
	//# \also	$@Skybox::SetSkyboxFlags@$
	
	
	//# \function	Skybox::SetSkyboxFlags		Returns the skybox flags.
	//
	//# \proto	void SetSkyboxFlags(unsigned_int32 flags);
	//
	//# \param	flags	The new skybox flags.
	//
	//# \desc
	//# The $GetSkyboxFlags$ function sets the skybox flags, which can be a combination (through logical OR) of the following values.
	//
	//# \table	SkyboxFlags
	//
	//# \also	$@Skybox::GetSkyboxFlags@$
	
	
	class Skybox : public Node
	{
		friend class Node;
		
		private:
			
			typedef ClassArray2<Renderable, 6, RenderType, unsigned_int32>	RenderableArray;
			
			unsigned_int32			skyboxFlags;
			float					texcoordAdjustment;
			
			MaterialObject			*materialObject;
			
			char					*skyboxStorage;
			bool					faceRenderFlag[6];
			
			RenderableArray			faceRenderable;
			List<Attribute>			attributeList[6];
			TextureMapAttribute		faceTextureMap[6];
			
			Skybox(const Skybox& skybox);
			
			Node *Replicate(void) const override;
			
			static void MaterialObjectLinkProc(Object *object, void *cookie);
		
		public:
			
			C4API Skybox(unsigned_int32 flags = 0);
			C4API ~Skybox(); 
			
			unsigned_int32 GetSkyboxFlags(void) const 
			{ 
				return (skyboxFlags); 
			}
			 
			void SetSkyboxFlags(unsigned_int32 flags)
			{
				skyboxFlags = flags;
			} 
			
			MaterialObject *GetMaterialObject(void) const
			{
				return (materialObject); 
			}
			
			const char *GetFaceTextureName(int32 index) const
			{
				return (faceTextureMap[index].GetTextureName());
			}
			
			void SetFaceTextureName(int32 index, const char *name)
			{
				faceTextureMap[index].SetTexture(name);
			}
			
			void Prepack(List<Object> *linkList) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetCategoryCount(void) const;
			Type GetCategoryType(int32 index, const char **title) const;
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
			
			C4API void SetMaterialObject(MaterialObject *object);
			C4API void InvalidateShaderData(void);
			
			void Preprocess(void);
			void Neutralize(void);
			
			bool Render(const FrustumCamera *camera, List<Renderable> *renderList);
	};
}


#endif

// ZYURVUR
