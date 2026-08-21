//=============================================================
//
// C4 Engine version 2.9.1
// Copyright 1999-2012, by Terathon Software LLC
//
// This file is part of the C4 Engine and is provided under the
// terms of the license agreement entered by the registed user.
//
// Unauthorized redistribution of source code is strictly
// prohibited. Violators will be prosecuted.
//
//=============================================================


#include "C4ExtraEffects.h"
#include "C4World.h"


using namespace C4;


const ConstPoint2D ShockwaveEffect::shockTexcoord[130] =
{
	{1.0F, 0.5F}, {0.5F, 0.5F}, {0.9975923F, 0.5490084F}, {0.5F, 0.5F}, {0.9903925F, 0.5975451F}, {0.5F, 0.5F}, {0.9784702F, 0.6451423F}, {0.5F, 0.5F},
	{0.9619396F, 0.6913417F}, {0.5F, 0.5F}, {0.9409606F, 0.7356983F}, {0.5F, 0.5F}, {0.9157347F, 0.777785F}, {0.5F, 0.5F}, {0.8865052F, 0.8171966F}, {0.5F, 0.5F},
	{0.8535534F, 0.8535534F}, {0.5F, 0.5F}, {0.8171966F, 0.8865052F}, {0.5F, 0.5F}, {0.777785F, 0.9157347F}, {0.5F, 0.5F}, {0.7356983F, 0.9409606F}, {0.5F, 0.5F},
	{0.6913416F, 0.9619396F}, {0.5F, 0.5F}, {0.6451423F, 0.9784702F}, {0.5F, 0.5F}, {0.5975451F, 0.9903926F}, {0.5F, 0.5F}, {0.5490084F, 0.9975923F}, {0.5F, 0.5F},
	{0.5F, 1.0F}, {0.5F, 0.5F}, {0.4509913F, 0.9975923F}, {0.5F, 0.5F}, {0.4024548F, 0.9903926F}, {0.5F, 0.5F}, {0.3548576F, 0.9784702F}, {0.5F, 0.5F},
	{0.3086582F, 0.9619396F}, {0.5F, 0.5F}, {0.2643016F, 0.9409606F}, {0.5F, 0.5F}, {0.2222148F, 0.9157347F}, {0.5F, 0.5F}, {0.1828032F, 0.8865052F}, {0.5F, 0.5F},
	{0.1464465F, 0.8535534F}, {0.5F, 0.5F}, {0.1134947F, 0.8171966F}, {0.5F, 0.5F}, {0.0842651F, 0.777785F}, {0.5F, 0.5F}, {0.0590393F, 0.7356983F}, {0.5F, 0.5F},
	{0.0380601F, 0.6913417F}, {0.5F, 0.5F}, {0.0215297F, 0.6451423F}, {0.5F, 0.5F}, {0.0096073F, 0.5975451F}, {0.5F, 0.5F}, {0.0024075F, 0.5490084F}, {0.5F, 0.5F},
	{0.0F, 0.5F}, {0.5F, 0.5F}, {0.0024075F, 0.4509913F}, {0.5F, 0.5F}, {0.0096073F, 0.4024548F}, {0.5F, 0.5F}, {0.0215297F, 0.3548576F}, {0.5F, 0.5F},
	{0.0380601F, 0.3086582F}, {0.5F, 0.5F}, {0.0590393F, 0.2643015F}, {0.5F, 0.5F}, {0.0842651F, 0.2222148F}, {0.5F, 0.5F}, {0.1134947F, 0.1828032F}, {0.5F, 0.5F},
	{0.1464465F, 0.1464465F}, {0.5F, 0.5F}, {0.1828032F, 0.1134947F}, {0.5F, 0.5F}, {0.2222148F, 0.0842651F}, {0.5F, 0.5F}, {0.2643016F, 0.0590393F}, {0.5F, 0.5F},
	{0.3086582F, 0.0380601F}, {0.5F, 0.5F}, {0.3548576F, 0.0215297F}, {0.5F, 0.5F}, {0.4024548F, 0.0096073F}, {0.5F, 0.5F}, {0.4509913F, 0.0024075F}, {0.5F, 0.5F},
	{0.5F, 0.0F}, {0.5F, 0.5F}, {0.5490084F, 0.0024075F}, {0.5F, 0.5F}, {0.5975451F, 0.0096073F}, {0.5F, 0.5F}, {0.6451423F, 0.0215297F}, {0.5F, 0.5F},
	{0.6913416F, 0.0380601F}, {0.5F, 0.5F}, {0.7356983F, 0.0590393F}, {0.5F, 0.5F}, {0.777785F, 0.0842651F}, {0.5F, 0.5F}, {0.8171966F, 0.1134947F}, {0.5F, 0.5F},
	{0.8535534F, 0.1464465F}, {0.5F, 0.5F}, {0.8865052F, 0.1828032F}, {0.5F, 0.5F}, {0.9157347F, 0.2222148F}, {0.5F, 0.5F}, {0.9409606F, 0.2643015F}, {0.5F, 0.5F},
	{0.9619396F, 0.3086582F}, {0.5F, 0.5F}, {0.9784702F, 0.3548576F}, {0.5F, 0.5F}, {0.9903925F, 0.4024548F}, {0.5F, 0.5F}, {0.9975923F, 0.4509913F}, {0.5F, 0.5F},
	{1.0F, 0.5F}, {0.5F, 0.5F},
};


ShockwaveEffect::ShockwaveEffect() : Effect(kEffectShockwave, kRenderTriangleStrip, kRenderDepthTest | kRenderDepthInhibit | kRenderDepthOffset)
{
}

ShockwaveEffect::ShockwaveEffect(const char *textureName, float radius, float width, float speed) :
		Effect(kEffectShockwave, kRenderTriangleStrip, kRenderDepthTest | kRenderDepthInhibit | kRenderDepthOffset),
		textureMap(textureName)
{
	width *= 0.5F;
	shockRadius = width;
	maxShockRadius = radius;
	shockWidth = width;
	shockSpeed = speed;
}

ShockwaveEffect::ShockwaveEffect(const ShockwaveEffect& shockwaveEffect) :
		Effect(shockwaveEffect),
		textureMap(shockwaveEffect.textureMap)
{
	shockRadius = shockwaveEffect.shockWidth;
	maxShockRadius = shockwaveEffect.maxShockRadius;
	shockWidth = shockwaveEffect.shockWidth;
	shockSpeed = shockwaveEffect.shockSpeed;
}

ShockwaveEffect::~ShockwaveEffect()
{
}

Node *ShockwaveEffect::Replicate(void) const
{
	return (new ShockwaveEffect(*this));
}

void ShockwaveEffect::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Effect::Pack(data, packFlags);
	
	data << ChunkHeader('RADI', 8);
	data << shockRadius;
	data << maxShockRadius;
	
	data << ChunkHeader('WIDE', 4);
	data << shockWidth;
	
	data << ChunkHeader('SPED', 4);
	data << shockSpeed;
	
	PackHandle handle = data.BeginChunk('TNAM');
	data << textureMap.GetTextureName();
	data.EndChunk(handle);
	
	data << TerminatorChunk;
}

void ShockwaveEffect::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Effect::Unpack(data, unpackFlags);
	UnpackChunkList<ShockwaveEffect>(data, unpackFlags);
}

bool ShockwaveEffect::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'RADI':
			
			data >> shockRadius;
			data >> maxShockRadius;
			return (true);
		
		case 'WIDE':
			 
			data >> shockWidth;
			return (true); 
		 
		case 'SPED': 
			
			data >> shockSpeed; 
			return (true);
		
		case 'TNAM':
		{ 
			ResourceName	textureName;
			
			data >> textureName;
			textureMap.SetTexture(textureName); 
			return (true);
		}
	}
	
	return (false);
}

bool ShockwaveEffect::CalculateBoundingSphere(BoundingSphere *sphere) const
{
	sphere->SetCenter(Point3D(0.0F, 0.0F, 0.0F));
	sphere->SetRadius(maxShockRadius);
	return (true);
}

void ShockwaveEffect::Preprocess(void)
{
	Effect::Preprocess();
	
	SetTransformable(this);
	SetDistortionState();
	SetDepthOffset(4.0F, &GetWorldPosition());
	
	attributeList.Append(&textureMap);
	SetMaterialAttributeList(&attributeList);
	SetShaderFlags(GetShaderFlags() | kShaderVertexBillboard);
	
	SetVertexCount(130);
	SetAttributeArray(kArrayVertex, shockVertex);
	SetAttributeArray(kArrayTexture0, &shockTexcoord[0]);
}

void ShockwaveEffect::Move(void)
{
	shockRadius += TheTimeMgr->GetFloatDeltaTime() * shockSpeed;
	if (shockRadius > maxShockRadius) delete this;
}

void ShockwaveEffect::Render(const Camera *camera, List<Renderable> *effectList)
{
	const ConstVector2D *trig = Math::GetTrigTable();
	
	float radius = shockRadius;
	float r1 = radius - shockWidth;
	float r2 = radius + shockWidth;
	
	for (machine a = 0; a < 64; a++)
	{
		const Vector2D& cs = trig[a * 4];
		shockVertex[a * 2] = cs * r2;
		shockVertex[a * 2 + 1] = cs * r1;
	}
	
	shockVertex[128] = shockVertex[0];
	shockVertex[129] = shockVertex[1];
	
	Effect::Render(camera, effectList);
}


ShellEffect::ShellEffect() :
		Effect(kEffectShell, kRenderIndexedTriangles, kRenderDepthTest | kRenderDepthInhibit),
		diffuseAttribute(kAttributeMutable)
{
	shellGeometry = nullptr;
	
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdatePostBounding);
}

ShellEffect::ShellEffect(Geometry *geometry, float size, const ColorRGBA& color) :
		Effect(kEffectShell, kRenderIndexedTriangles, kRenderDepthTest | kRenderDepthInhibit),
		diffuseAttribute(color, kAttributeMutable)
{
	shellGeometry = geometry;
	scaleVector.Set(size, 0.0F, 0.0F, 0.0F);
	
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdatePostBounding);
}

ShellEffect::ShellEffect(const ShellEffect& shellEffect) :
		Effect(shellEffect),
		diffuseAttribute(shellEffect.diffuseAttribute.GetDiffuseColor())
{
	shellGeometry = shellEffect.shellGeometry;
	scaleVector = shellEffect.scaleVector;
	
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdatePostBounding);
}

ShellEffect::~ShellEffect()
{
}

Node *ShellEffect::Replicate(void) const
{
	return (new ShellEffect(*this));
}

void ShellEffect::Preprocess(void)
{
	Effect::Preprocess();
	
	if (shellGeometry)
	{
		SetAmbientBlendState(kBlendAccumulate);
		SetShaderFlags(kShaderAmbientEffect | kShaderNormalExpandVertex | (shellGeometry->GetShaderFlags() & kShaderNormalizeBasisVectors));
		SetRenderParameterPointer(&scaleVector);
		
		SetTransformable(shellGeometry->GetTransformable());
	}
	
	attributeList.Append(&diffuseAttribute);
	SetMaterialAttributeList(&attributeList);
}

void ShellEffect::CalculatePostBounding(void)
{
	if (shellGeometry)
	{
		const BoundingSphere *sphere = shellGeometry->GetBoundingSphere();
		if (sphere) SetBoundingSphere(sphere->GetCenter(), sphere->GetRadius() + scaleVector.x);
	}
	
	Effect::CalculatePostBounding();
}

void ShellEffect::Render(const Camera *camera, List<Renderable> *effectList)
{
	if (shellGeometry)
	{
		GetWorld()->UpdateGeometry(shellGeometry);
		
		SetDynamicArrayFlags(shellGeometry->GetDynamicArrayFlags());
		SetVertexBuffer(kVertexBufferStaticArray, shellGeometry->GetVertexBuffer(kVertexBufferStaticArray));
		SetVertexBuffer(kVertexBufferDynamicArray, shellGeometry->GetVertexBuffer(kVertexBufferDynamicArray));
		SetVertexBuffer(kVertexBufferIndexArray, shellGeometry->GetVertexBuffer(kVertexBufferIndexArray));
		
		SetVertexCount(shellGeometry->GetVertexCount());
		SetAttributeOffset(kArrayVertex, shellGeometry->GetAttributeOffset(kArrayVertex), shellGeometry->GetComponentCount(kArrayVertex));
		SetAttributeOffset(kArrayNormal, shellGeometry->GetAttributeOffset(kArrayNormal), shellGeometry->GetComponentCount(kArrayNormal));
		
		SetFaceArray(shellGeometry->GetFaceArray());
		SetFaceOffset(shellGeometry->GetFaceOffset());
		GetFirstRenderSegment()->SetFaceRange(0, shellGeometry->GetFaceCount());
		
		effectList[kEffectListOpaque].Append(this);
	}
}

// ZYURVUR
