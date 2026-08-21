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


#include "C4Skybox.h"
#include "C4Regions.h"
#include "C4Configuration.h"


using namespace C4;


Skybox::Skybox(unsigned_int32 flags) :
		Node(kNodeSkybox),
		faceRenderable(kRenderTriangleStrip, kRenderDepthTest | kRenderDepthInhibit)
{
	skyboxFlags = flags;
	texcoordAdjustment = 0.0F;
	
	materialObject = nullptr;
	skyboxStorage = nullptr;
}

Skybox::Skybox(const Skybox& skybox) :
		Node(skybox),
		faceRenderable(kRenderTriangleStrip, kRenderDepthTest | kRenderDepthInhibit)
{
	skyboxFlags = skybox.skyboxFlags;
	texcoordAdjustment = skybox.texcoordAdjustment;
	
	materialObject = skybox.materialObject;
	if (materialObject) materialObject->Retain();
	
	skyboxStorage = nullptr;
}

Skybox::~Skybox()
{
	delete[] skyboxStorage;
	
	if (materialObject) materialObject->Release();
}

Node *Skybox::Replicate(void) const
{
	return (new Skybox(*this));
}

void Skybox::Prepack(List<Object> *linkList) const
{
	Node::Prepack(linkList);
	if (materialObject) linkList->Append(materialObject);
}

void Skybox::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Node::Pack(data, packFlags);
	
	data << ChunkHeader('FLAG', 4);
	data << skyboxFlags;
	
	data << ChunkHeader('TEXC', 4);
	data << texcoordAdjustment;
	
	if ((materialObject) && (!(packFlags & kPackSettings)))
	{
		data << ChunkHeader('MATL', 4);
		data << materialObject->GetObjectIndex();
	}
	
	for (machine a = 0; a < 6; a++)
	{
		const ResourceName& name = faceTextureMap[a].GetTextureName();
		if (name[0] != 0)
		{
			PackHandle handle = data.BeginChunk('FACE');
			data << int32(a);
			data << name;
			data.EndChunk(handle);
		}
	}
	
	data << TerminatorChunk;
}

void Skybox::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Node::Unpack(data, unpackFlags);
	UnpackChunkList<Skybox>(data, unpackFlags);
}

bool Skybox::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'FLAG':
			
			data >> skyboxFlags;
			return (true);
		
		case 'TEXC':
			
			data >> texcoordAdjustment;
			return (true);
		
		case 'MATL': 
		{
			int32	objectIndex; 
			 
			data >> objectIndex; 
			data.AddObjectLink(objectIndex, &MaterialObjectLinkProc, this);
			return (true); 
		}
		
		case 'FACE':
		{ 
			int32			index;
			ResourceName	name;
			
			data >> index; 
			data >> name;
			faceTextureMap[index].SetTexture(name);
			return (true);
		}
		
		#if C4LEGACY
		
			case 'DATA':
			{
				int32	subdiv;
				
				data >> subdiv;
				data >> skyboxFlags;
				return (true);
			}
		
		#endif
	}
	
	return (false);
}

void *Skybox::BeginSettingsUnpack(void)
{
	for (machine a = 0; a < 6; a++) faceTextureMap[a].SetTexture(static_cast<const char *>(nullptr));
	return (Node::BeginSettingsUnpack());
}

void Skybox::MaterialObjectLinkProc(Object *object, void *cookie)
{
	Skybox *skybox = static_cast<Skybox *>(cookie);
	skybox->SetMaterialObject(static_cast<MaterialObject *>(object));
}

int32 Skybox::GetCategoryCount(void) const
{
	return (Node::GetCategoryCount() + 1);
}

Type Skybox::GetCategoryType(int32 index, const char **title) const
{
	int32 count = Node::GetCategoryCount();
	if (index == count)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kNodeSkybox));
		return (kNodeSkybox);
	}
	
	return (Node::GetCategoryType(index, title));
}

int32 Skybox::GetCategorySettingCount(Type category) const
{
	if (category == kNodeSkybox) return (14);
	return (Node::GetCategorySettingCount(category));
}

Setting *Skybox::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kNodeSkybox)
	{
		if (flags & kConfigurationScript) return (nullptr);
		
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID(kNodeSkybox, 'SKYB'));
			return (new HeadingSetting(kNodeSkybox, title));
		}
		
		if (index == 1)
		{
			const char *title = table->GetString(StringID(kNodeSkybox, 'SKYB', 'HORZ'));
			return (new BooleanSetting('HORZ', ((skyboxFlags & kSkyboxHorizon) != 0), title));
		}
		
		if (index == 2)
		{
			const char *title = table->GetString(StringID(kNodeSkybox, 'SKYB', 'SMER'));
			return (new BooleanSetting('SMER', ((skyboxFlags & kSkyboxSmearTexture) != 0), title));
		}
		
		if (index == 3)
		{
			const char *title = table->GetString(StringID(kNodeSkybox, 'SKYB', 'TEXC'));
			return (new TextSetting('TEXC', texcoordAdjustment, title));
		}
		
		if (index == 4)
		{
			const char *title = table->GetString(StringID(kNodeSkybox, 'SKYB', 'GLOW'));
			return (new BooleanSetting('GLOW', ((skyboxFlags & kSkyboxGlowEnable) != 0), title));
		}
		
		if (index == 5)
		{
			const char *title = table->GetString(StringID(kNodeSkybox, 'SKYB', 'NFOG'));
			return (new BooleanSetting('NFOG', ((skyboxFlags & kSkyboxFogInhibit) != 0), title));
		}
		
		if (index == 6)
		{
			const char *title = table->GetString(StringID(kNodeSkybox, 'TXTR'));
			return (new HeadingSetting('TXTR', title));
		}
		
		if ((index >= 7) && (index <= 12))
		{
			index -= 7;
			Type identifier = 'FAC0' + index;
			
			const char *title = table->GetString(StringID(kNodeSkybox, 'TXTR', identifier));
			const char *picker = table->GetString(StringID(kNodeSkybox, 'TXTR', 'PICK'));
			return (new ResourceSetting(identifier, GetFaceTextureName(index), title, picker, TextureResource::GetDescriptor()));
		}
		
		return (nullptr);
	}
	
	return (Node::GetCategorySetting(category, index, flags));
}

void Skybox::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kNodeSkybox)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'HORZ')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) skyboxFlags |= kSkyboxHorizon;
			else skyboxFlags &= ~kSkyboxHorizon;
		}
		else if (identifier == 'SMER')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) skyboxFlags |= kSkyboxSmearTexture;
			else skyboxFlags &= ~kSkyboxSmearTexture;
		}
		else if (identifier == 'TEXC')
		{
			texcoordAdjustment = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
		}
		else if (identifier == 'GLOW')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) skyboxFlags |= kSkyboxGlowEnable;
			else skyboxFlags &= ~kSkyboxGlowEnable;
		}
		else if (identifier == 'NFOG')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) skyboxFlags |= kSkyboxFogInhibit;
			else skyboxFlags &= ~kSkyboxFogInhibit;
		}
		else if ((identifier >= 'FAC0') && (identifier <= 'FAC5'))
		{
			const char *name = static_cast<const ResourceSetting *>(setting)->GetResourceName();
			SetFaceTextureName(identifier - 'FAC0', name);
		}
	}
	else
	{
		Node::SetCategorySetting(category, setting);
	}
}

void Skybox::SetMaterialObject(MaterialObject *object)
{
	if (materialObject != object)
	{
		if (materialObject) materialObject->Release();
		if (object) object->Retain();
		materialObject = object;
	}
}

void Skybox::InvalidateShaderData(void)
{
	for (machine a = 0; a < 6; a++) faceRenderable[a].InvalidateShaderData();
}

void Skybox::Preprocess(void)
{
	Node::Preprocess();
	
	delete[] skyboxStorage;
	skyboxStorage = nullptr;
	
	int32 vertexCount = 0;
	
	unsigned_int32 shaderFlags = kShaderVertexInfinite;
	unsigned_int32 blendState = kBlendReplace;
	
	unsigned_int32 renderableFlags = kRenderableStructureBufferInhibit;
	if (skyboxFlags & kSkyboxFogInhibit) renderableFlags |= kRenderableFogInhibit;
	
	unsigned_int32 materialState = (skyboxFlags & kSkyboxGlowEnable) ? kMaterialEmissionGlow : 0;
	
	for (machine a = 0; a < 6; a++)
	{
		faceRenderFlag[a] = false;
		
		if (faceTextureMap[a].GetTextureName()[0] != 0)
		{
			faceRenderFlag[a] = true;
			vertexCount += 4;
			
			Renderable *renderable = &faceRenderable[a];
			renderable->SetRenderableFlags(renderableFlags);
			renderable->SetShaderFlags(shaderFlags);
			renderable->SetAmbientBlendState(blendState);
			renderable->SetTransformable(this);
			renderable->SetVertexCount(4);
			
			attributeList[a].Append(&faceTextureMap[a]);
			renderable->SetMaterialAttributeList(&attributeList[a]);
			renderable->SetMaterialObjectPointer(&materialObject);
			renderable->GetFirstRenderSegment()->SetMaterialState(materialState);
		}
	}
	
	unsigned_int32 size = vertexCount * (sizeof(Vector3D) + sizeof(Point2D));
	if (size != 0)
	{
		Point3D		*vertexArray[6];
		Point2D		*texcoordArray[6];
		
		char *data = new char[size];
		skyboxStorage = data;
		
		for (machine a = 0; a < 6; a++)
		{
			if (faceTextureMap[a].GetTextureName()[0] != 0)
			{
				Renderable *renderable = &faceRenderable[a];
				
				Point3D *vertex = reinterpret_cast<Point3D *>(data);
				Point2D *texcoord = reinterpret_cast<Point2D *>(vertex + 4);
				
				vertexArray[a] = vertex;
				texcoordArray[a] = texcoord;
				
				renderable->SetAttributeArray(kArrayVertex, vertex);
				renderable->SetAttributeArray(kArrayTexture0, texcoord);
				
				data = reinterpret_cast<char *>(texcoord + 4);
			}
		}
		
		float bottomPosition = (skyboxFlags & kSkyboxHorizon) ? 0.0F : -1.0F;
		float bottomTexcoord = (skyboxFlags & kSkyboxSmearTexture) ? texcoordAdjustment - 1.0F : texcoordAdjustment;
		
		if (faceRenderFlag[0])
		{
			Point3D *vertex = vertexArray[0];
			Point2D *texcoord = texcoordArray[0];
			
			vertex[0].Set(1.0F, 1.0F, bottomPosition);
			vertex[1].Set(1.0F, -1.0F, bottomPosition);
			vertex[2].Set(1.0F, 1.0F, 1.0F);
			vertex[3].Set(1.0F, -1.0F, 1.0F);
			
			texcoord[0].Set(0.0F, bottomTexcoord);
			texcoord[1].Set(1.0F, bottomTexcoord);
			texcoord[2].Set(0.0F, 1.0F);
			texcoord[3].Set(1.0F, 1.0F);
		}
			
		if (faceRenderFlag[1])
		{
			Point3D *vertex = vertexArray[1];
			Point2D *texcoord = texcoordArray[1];
			
			vertex[0].Set(-1.0F, -1.0F, bottomPosition);
			vertex[1].Set(-1.0F, 1.0F, bottomPosition);
			vertex[2].Set(-1.0F, -1.0F, 1.0F);
			vertex[3].Set(-1.0F, 1.0F, 1.0F);
			
			texcoord[0].Set(0.0F, bottomTexcoord);
			texcoord[1].Set(1.0F, bottomTexcoord);
			texcoord[2].Set(0.0F, 1.0F);
			texcoord[3].Set(1.0F, 1.0F);
		}
			
		if (faceRenderFlag[2])
		{
			Point3D *vertex = vertexArray[2];
			Point2D *texcoord = texcoordArray[2];
			
			vertex[0].Set(-1.0F, 1.0F, bottomPosition);
			vertex[1].Set(1.0F, 1.0F, bottomPosition);
			vertex[2].Set(-1.0F, 1.0F, 1.0F);
			vertex[3].Set(1.0F, 1.0F, 1.0F);
			
			texcoord[0].Set(0.0F, bottomTexcoord);
			texcoord[1].Set(1.0F, bottomTexcoord);
			texcoord[2].Set(0.0F, 1.0F);
			texcoord[3].Set(1.0F, 1.0F);
		}
			
		if (faceRenderFlag[3])
		{
			Point3D *vertex = vertexArray[3];
			Point2D *texcoord = texcoordArray[3];
			
			vertex[0].Set(1.0F, -1.0F, bottomPosition);
			vertex[1].Set(-1.0F, -1.0F, bottomPosition);
			vertex[2].Set(1.0F, -1.0F, 1.0F);
			vertex[3].Set(-1.0F, -1.0F, 1.0F);
			
			texcoord[0].Set(0.0F, bottomTexcoord);
			texcoord[1].Set(1.0F, bottomTexcoord);
			texcoord[2].Set(0.0F, 1.0F);
			texcoord[3].Set(1.0F, 1.0F);
		}
			
		if (faceRenderFlag[4])
		{
			Point3D *vertex = vertexArray[4];
			Point2D *texcoord = texcoordArray[4];
			
			vertex[0].Set(1.0F, -1.0F, 1.0F);
			vertex[1].Set(-1.0F, -1.0F, 1.0F);
			vertex[2].Set(1.0F, 1.0F, 1.0F);
			vertex[3].Set(-1.0F, 1.0F, 1.0F);
			
			texcoord[0].Set(0.0F, 0.0F);
			texcoord[1].Set(1.0F, 0.0F);
			texcoord[2].Set(0.0F, 1.0F);
			texcoord[3].Set(1.0F, 1.0F);
		}
			
		if (faceRenderFlag[5])
		{
			Point3D *vertex = vertexArray[5];
			Point2D *texcoord = texcoordArray[5];
			
			vertex[0].Set(1.0F, 1.0F, -1.0F);
			vertex[1].Set(-1.0F, 1.0F, -1.0F);
			vertex[2].Set(1.0F, -1.0F, -1.0F);
			vertex[3].Set(-1.0F, -1.0F, -1.0F);
			
			texcoord[0].Set(0.0F, 0.0F);
			texcoord[1].Set(1.0F, 0.0F);
			texcoord[2].Set(0.0F, 1.0F);
			texcoord[3].Set(1.0F, 1.0F);
		}
	}
}

void Skybox::Neutralize(void)
{
	delete[] skyboxStorage;
	skyboxStorage = nullptr;
	
	Node::Neutralize();
}

bool Skybox::Render(const FrustumCamera *camera, List<Renderable> *renderList)
{
	Vector3D direction = GetInverseWorldTransform() * camera->GetWorldTransform()[2];
	float sineHalfField = camera->GetSineHalfField();
	bool render = false;
	
	if ((faceRenderFlag[0]) && (direction.x > -sineHalfField))
	{
		renderList->Append(&faceRenderable[0]);
		render = true;
	}
	
	if ((faceRenderFlag[1]) && (direction.x < sineHalfField))
	{
		renderList->Append(&faceRenderable[1]);
		render = true;
	}
	
	if ((faceRenderFlag[2]) && (direction.y > -sineHalfField))
	{
		renderList->Append(&faceRenderable[2]);
		render = true;
	}
	
	if ((faceRenderFlag[3]) && (direction.y < sineHalfField))
	{
		renderList->Append(&faceRenderable[3]);
		render = true;
	}
	
	if ((faceRenderFlag[4]) && (direction.z > -sineHalfField))
	{
		renderList->Append(&faceRenderable[4]);
		render = true;
	}
	
	if ((faceRenderFlag[5]) && (direction.z < sineHalfField))
	{
		renderList->Append(&faceRenderable[5]);
		render = true;
	}
	
	return (render);
}

// ZYURVUR
