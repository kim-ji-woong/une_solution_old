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


#ifndef C4SpaceManipulators_h
#define C4SpaceManipulators_h


#include "C4EditorManipulators.h"
#include "C4VolumeManipulators.h"
#include "C4EditorUndo.h"
#include "C4Spaces.h"


namespace C4
{
	class SpaceManipulator : public EditorManipulator
	{
		friend class EditorManipulator;
		
		private:
			
			VolumeManipulator	*volumeManipulator;
			
			static Manipulator *Construct(Space *space);
		
		protected:
			
			SpaceManipulator(Space *space, VolumeManipulator *volume, const char *iconName);
		
		public:
			
			~SpaceManipulator();
			
			Space *GetTargetNode(void) const
			{
				return (static_cast<Space *>(EditorManipulator::GetTargetNode()));
			}
			
			SpaceObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			const char *GetDefaultNodeName(void) const;
			
			void Select(void);
			void Unselect(void);
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool RegionPick(const Region *region) const;
			
			void Render(const ManipulatorRenderData *renderData);
	};
	
	
	class FogSpaceManipulator : public SpaceManipulator, public PlateVolumeManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			FogSpaceManipulator(FogSpace *fog);
			~FogSpaceManipulator();
			
			FogSpace *GetTargetNode(void) const
			{
				return (static_cast<FogSpace *>(EditorManipulator::GetTargetNode()));
			}
			
			FogSpaceObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class ShadowSpaceManipulator : public SpaceManipulator, public BoxVolumeManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			ShadowSpaceManipulator(ShadowSpace *shadow);
			~ShadowSpaceManipulator();
			
			ShadowSpace *GetTargetNode(void) const
			{
				return (static_cast<ShadowSpace *>(EditorManipulator::GetTargetNode()));
			}
			
			ShadowSpaceObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			} 
			
			Box3D CalculateNodeBoundingBox(void) const; 
			 
			bool Resize(const ManipulatorResizeData *resizeData); 
			void Update(void);
	}; 
	
	
	class AmbientSpaceManipulator : public SpaceManipulator, public BoxVolumeManipulator
	{ 
		private:
			
			List<Attribute>		gridAttributeList;
			DiffuseAttribute	gridDiffuseColor; 
			Renderable			gridRenderable;
			
			Point3D				gridVertex[(AmbientSpaceObject::kMaxAmbientSpaceSize - 2) * 12];
			Line				gridLine[(AmbientSpaceObject::kMaxAmbientSpaceSize - 2) * 12];
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			AmbientSpaceManipulator(AmbientSpace *ambient);
			~AmbientSpaceManipulator();
			
			AmbientSpace *GetTargetNode(void) const
			{
				return (static_cast<AmbientSpace *>(EditorManipulator::GetTargetNode()));
			}
			
			AmbientSpaceObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
			
			void Render(const ManipulatorRenderData *renderData);
	};
	
	
	class AcousticsSpaceManipulator : public SpaceManipulator, public BoxVolumeManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			AcousticsSpaceManipulator(AcousticsSpace *acoustics);
			~AcousticsSpaceManipulator();
			
			AcousticsSpace *GetTargetNode(void) const
			{
				return (static_cast<AcousticsSpace *>(EditorManipulator::GetTargetNode()));
			}
			
			AcousticsSpaceObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			
			void Update(void);
	};
	
	
	class OcclusionSpaceManipulator : public SpaceManipulator, public BoxVolumeManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			OcclusionSpaceManipulator(OcclusionSpace *occlusion);
			~OcclusionSpaceManipulator();
			
			OcclusionSpace *GetTargetNode(void) const
			{
				return (static_cast<OcclusionSpace *>(EditorManipulator::GetTargetNode()));
			}
			
			OcclusionSpaceObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			
			void Update(void);
	};
	
	
	class PaintSpaceManipulator : public SpaceManipulator, public BoxVolumeManipulator
	{
		private:
			
			List<NodeReference>		undoGeometryList;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			PaintSpaceManipulator(PaintSpace *shadow);
			~PaintSpaceManipulator();
			
			PaintSpace *GetTargetNode(void) const
			{
				return (static_cast<PaintSpace *>(EditorManipulator::GetTargetNode()));
			}
			
			PaintSpaceObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			
			void Update(void);
			
			void HandleDelete(bool undoable);
			void HandleUndelete(void);
			void HandleSettingsUpdate(void);
	};
}


#endif

// ZYURVUR
