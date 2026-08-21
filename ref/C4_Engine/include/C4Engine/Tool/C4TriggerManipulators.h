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


#ifndef C4TriggerManipulators_h
#define C4TriggerManipulators_h


#include "C4EditorManipulators.h"
#include "C4VolumeManipulators.h"
#include "C4Triggers.h"


namespace C4
{
	class TriggerManipulator : public EditorManipulator
	{
		friend class EditorManipulator;
		
		private:
			
			VolumeManipulator	*volumeManipulator;
			
			static Manipulator *Construct(Trigger *trigger);
		
		protected:
			
			TriggerManipulator(Trigger *trigger, VolumeManipulator *volume);
		
		public:
			
			~TriggerManipulator();
			
			Trigger *GetTargetNode(void) const
			{
				return (static_cast<Trigger *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
			
			void Select(void);
			void Unselect(void);
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool RegionPick(const Region *region) const;
			
			void Render(const ManipulatorRenderData *renderData);
	};
	
	
	class BoxTriggerManipulator : public TriggerManipulator, public BoxVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			BoxTriggerManipulator(BoxTrigger *box);
			~BoxTriggerManipulator();
			
			BoxTrigger *GetTargetNode(void) const
			{
				return (static_cast<BoxTrigger *>(EditorManipulator::GetTargetNode()));
			}
			
			BoxTriggerObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class CylinderTriggerManipulator : public TriggerManipulator, public CylinderVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			CylinderTriggerManipulator(CylinderTrigger *cylinder);
			~CylinderTriggerManipulator();
			
			CylinderTrigger *GetTargetNode(void) const
			{
				return (static_cast<CylinderTrigger *>(EditorManipulator::GetTargetNode()));
			}
			
			CylinderTriggerObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const; 
			
			bool Resize(const ManipulatorResizeData *resizeData); 
			void Update(void); 
	}; 
	
	 
	class SphereTriggerManipulator : public TriggerManipulator, public SphereVolumeManipulator
	{
		private:
			 
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const; 
		
		public:
			
			SphereTriggerManipulator(SphereTrigger *sphere);
			~SphereTriggerManipulator();
			
			SphereTrigger *GetTargetNode(void) const
			{
				return (static_cast<SphereTrigger *>(EditorManipulator::GetTargetNode()));
			}
			
			SphereTriggerObject *GetObject(void) const
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
