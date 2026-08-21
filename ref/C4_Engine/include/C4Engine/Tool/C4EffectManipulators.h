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


#ifndef C4EffectManipulators_h
#define C4EffectManipulators_h


#include "C4EditorManipulators.h"
#include "C4VolumeManipulators.h"
#include "C4Particles.h"
#include "C4Panels.h"


namespace C4
{
	class EffectManipulator : public EditorManipulator
	{
		friend class EditorManipulator;
		
		private:
			
			VolumeManipulator	*volumeManipulator;
			
			static Manipulator *Construct(Effect *effect);
		
		protected:
			
			EffectManipulator(Effect *effect, VolumeManipulator *volume, const char *iconName);
		
		public:
			
			~EffectManipulator();
			
			Effect *GetTargetNode(void) const
			{
				return (static_cast<Effect *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
			
			void Invalidate(void);
			
			void Select(void);
			void Unselect(void);
			
			void HandleSizeUpdate(int32 count, const float *size);
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool RegionPick(const Region *region) const;
			
			void Render(const ManipulatorRenderData *renderData);
	};
	
	
	class ParticleSystemManipulator : public EditorManipulator
	{
		public:
			
			ParticleSystemManipulator(ParticleSystem *particleSystem);
			~ParticleSystemManipulator();
			
			ParticleSystem *GetTargetNode(void) const
			{
				return (static_cast<ParticleSystem *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
			
			void Preprocess(void);
			
			bool MaterialSettable(void) const;
			bool MaterialRemovable(void) const;
			const MaterialObject *PickupMaterial(void) const;
			void SetMaterial(MaterialObject *materialObject);
			void RemoveMaterial(void);
	};
	
	
	class QuadEffectManipulator : public EffectManipulator, public SphereVolumeManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			QuadEffectManipulator(QuadEffect *quadEffect);
			~QuadEffectManipulator();
			
			QuadEffect *GetTargetNode(void) const
			{
				return (static_cast<QuadEffect *>(EditorManipulator::GetTargetNode()));
			}
			
			QuadEffectObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	}; 
	
	 
	class FlareEffectManipulator : public EffectManipulator, public SphereVolumeManipulator 
	{ 
		private:
			 
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public: 
			
			FlareEffectManipulator(FlareEffect *flareEffect);
			~FlareEffectManipulator();
			 
			FlareEffect *GetTargetNode(void) const
			{
				return (static_cast<FlareEffect *>(EditorManipulator::GetTargetNode()));
			}
			
			FlareEffectObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class BeamEffectManipulator : public EffectManipulator, public CylinderVolumeManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			BeamEffectManipulator(BeamEffect *beamEffect);
			~BeamEffectManipulator();
			
			BeamEffect *GetTargetNode(void) const
			{
				return (static_cast<BeamEffect *>(EditorManipulator::GetTargetNode()));
			}
			
			BeamEffectObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class TubeEffectManipulator : public EffectManipulator, public DiskVolumeManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			TubeEffectManipulator(TubeEffect *tubeEffect);
			~TubeEffectManipulator();
			
			TubeEffect *GetTargetNode(void) const
			{
				return (static_cast<TubeEffect *>(EditorManipulator::GetTargetNode()));
			}
			
			TubeEffectObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class FireEffectManipulator : public EffectManipulator, public CylinderVolumeManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			FireEffectManipulator(FireEffect *fireEffect);
			~FireEffectManipulator();
			
			FireEffect *GetTargetNode(void) const
			{
				return (static_cast<FireEffect *>(EditorManipulator::GetTargetNode()));
			}
			
			FireEffectObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class PanelEffectManipulator : public EffectManipulator, public PlateVolumeManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			PanelEffectManipulator(PanelEffect *panelEffect);
			~PanelEffectManipulator();
			
			PanelEffect *GetTargetNode(void) const
			{
				return (static_cast<PanelEffect *>(EditorManipulator::GetTargetNode()));
			}
			
			PanelEffectObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
}


#endif

// ZYURVUR
