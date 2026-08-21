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


#include "C4ExtrasPlugin.h"


using namespace C4;


ResourceDescriptor FieldResource::descriptor("fld");


FieldResource::FieldResource(const char *name, ResourceCatalog *catalog) : Resource<FieldResource>(name, catalog)
{
}

FieldResource::~FieldResource()
{
}

void FieldResource::Preprocess(void)
{
	int32 *data = static_cast<int32 *>(GetData());
	if (data[0] != 1)
	{
		Reverse(&data[1]);
		
		int32 count = GetFieldCount();
		FieldData *fieldData = reinterpret_cast<FieldData *>(&data[2]);
		for (machine a = 0; a < count; a++)
		{
			Reverse(&fieldData->rightAscension);
			Reverse(&fieldData->declination);
			Reverse(&fieldData->magnitude);
			fieldData++;
		}
	}
}


StarField::StarField() :
		InfinitePointParticleSystem(kParticleSystemStarField, &particlePool),
		particlePool(kMaxParticleCount, particleArray)
{
	SetVisibilityProc(&AlwaysVisible);
	SetOcclusionProc(&NeverOccluded);
}

StarField::StarField(const StarField& starField) :
		InfinitePointParticleSystem(starField, &particlePool),
		particlePool(kMaxParticleCount, particleArray)
{
	SetVisibilityProc(&AlwaysVisible);
	SetOcclusionProc(&NeverOccluded);
}

StarField::~StarField()
{
}

Node *StarField::Replicate(void) const
{
	return (new StarField(*this));
}

void StarField::Preprocess(void)
{
	SetParticleSystemFlags(kParticleSystemPointSprite | kParticleSystemNonpersistent);
	InfinitePointParticleSystem::Preprocess();
	
	if (!GetManipulator())
	{
		FieldResource *resource = FieldResource::Get("star/Earth");
		if (resource)
		{
			const FieldData *fieldData = resource->GetFieldData();
			
			int32 fieldCount = Min(resource->GetFieldCount(), kMaxParticleCount);
			for (machine a = 0; a < fieldCount; a++)
			{
				Particle *particle = particlePool.NewParticle();
				
				particle->emitTime = 0;
				particle->lifeTime = 0x7FFFFFFF;
				particle->orientation = 0;
				
				float bright = 1.0F - fieldData->magnitude * 0.2F;
				float radius = bright * 0.6F;
				float color = 1.0F;
				if (radius < 0.25F) color = Fmax(color * radius * 4.0F, 0.15F);
				
				particle->radius = Fmax(radius, 0.25F) * 0.01F;
				particle->color.Set(color, color, color, 1.0F);
				
				Vector2D t = CosSin(fieldData->rightAscension * K::pi_over_12);
				Vector2D u = CosSin((90.0F - fieldData->declination) * K::radians);
				particle->position.Set(t.x * u.y, t.y * u.y, u.x);
				
				AddParticle(particle);
				fieldData++;
			}
			
			resource->Release();
		}
	} 
}
 
void StarField::AnimateParticles(void) 
{ 
}

// ZYURVUR
