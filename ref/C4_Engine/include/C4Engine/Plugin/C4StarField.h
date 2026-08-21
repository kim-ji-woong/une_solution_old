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


#ifndef C4StarField_h
#define C4StarField_h


#include "C4ExtrasBase.h"
#include "C4Particles.h"


namespace C4
{
	enum
	{
		kParticleSystemStarField	= 'star'
	};
	
	
	struct FieldData
	{
		float		rightAscension;
		float		declination;
		float		magnitude;
	};
	
	
	class FieldResource : public Resource<FieldResource>
	{
		friend class Resource<FieldResource>;
		
		private:
			
			static ResourceDescriptor	descriptor;
			
			~FieldResource();
			
			void Preprocess(void);
		
		public:
			
			FieldResource(const char *name, ResourceCatalog *catalog);
			
			int32 GetFieldCount(void) const
			{
				return (static_cast<const int32 *>(GetData())[1]);
			}
			
			const FieldData *GetFieldData(void) const
			{
				return (reinterpret_cast<const FieldData *>(&static_cast<const int32 *>(GetData())[2]));
			}
	};
	
	
	class C4EXTRASAPI StarField : public InfinitePointParticleSystem
	{
		private:
			
			enum
			{
				kMaxParticleCount = 12000
			};
			
			ParticlePool<>		particlePool;
			Particle			particleArray[kMaxParticleCount];
			
			StarField(const StarField& starField);
			
			Node *Replicate(void) const override;
		
		public:
			
			StarField();
			~StarField();
			
			void Preprocess(void);
			void AnimateParticles(void);
	};
}


#endif

// ZYURVUR
