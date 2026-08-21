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


#ifndef C4EmitterManipulators_h
#define C4EmitterManipulators_h


#include "C4EditorManipulators.h"
#include "C4VolumeManipulators.h"
#include "C4Emitters.h"


namespace C4
{
	class EmitterManipulator : public EditorManipulator
	{
		friend class EditorManipulator;
		
		private:
			
			VolumeManipulator	*volumeManipulator;
			
			static Manipulator *Construct(Emitter *emitter);
		
		protected:
			
			EmitterManipulator(Emitter *emitter, VolumeManipulator *volume);
		
		public:
			
			~EmitterManipulator();
			
			Emitter *GetTargetNode(void) const
			{
				return (static_cast<Emitter *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
			
			void Select(void);
			void Unselect(void);
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool RegionPick(const Region *region) const;
			
			void Render(const ManipulatorRenderData *renderData);
	};
	
	
	class BoxEmitterManipulator : public EmitterManipulator, public BoxVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			BoxEmitterManipulator(BoxEmitter *box);
			~BoxEmitterManipulator();
			
			BoxEmitter *GetTargetNode(void) const
			{
				return (static_cast<BoxEmitter *>(EditorManipulator::GetTargetNode()));
			}
			
			BoxEmitterObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class CylinderEmitterManipulator : public EmitterManipulator, public CylinderVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			CylinderEmitterManipulator(CylinderEmitter *cylinder);
			~CylinderEmitterManipulator();
			
			CylinderEmitter *GetTargetNode(void) const
			{
				return (static_cast<CylinderEmitter *>(EditorManipulator::GetTargetNode()));
			}
			
			CylinderEmitterObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const; 
			
			bool Resize(const ManipulatorResizeData *resizeData); 
			void Update(void); 
	}; 
	
	 
	class SphereEmitterManipulator : public EmitterManipulator, public SphereVolumeManipulator
	{
		private:
			 
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const; 
		
		public:
			
			SphereEmitterManipulator(SphereEmitter *sphere);
			~SphereEmitterManipulator();
			
			SphereEmitter *GetTargetNode(void) const
			{
				return (static_cast<SphereEmitter *>(EditorManipulator::GetTargetNode()));
			}
			
			SphereEmitterObject *GetObject(void) const
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
