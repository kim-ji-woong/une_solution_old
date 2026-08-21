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


#ifndef C4SourceManipulators_h
#define C4SourceManipulators_h


#include "C4EditorManipulators.h"
#include "C4VolumeManipulators.h"
#include "C4Sources.h"


namespace C4
{
	class SourceManipulator : public EditorManipulator
	{
		friend class EditorManipulator;
		
		private:
			
			VolumeManipulator	*volumeManipulator;
			
			static Manipulator *Construct(Source *source);
		
		public:
			
			SourceManipulator(Source *source, VolumeManipulator *volume, const char *iconName);
			~SourceManipulator();
			
			Source *GetTargetNode(void) const
			{
				return (static_cast<Source *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
			
			void Preprocess(void);
			
			void Select(void);
			void Unselect(void);
	};
	
	
	class OmniSourceManipulator : public SourceManipulator, public SphereVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			OmniSourceManipulator(OmniSource *omniSource);
			~OmniSourceManipulator();
			
			OmniSource *GetTargetNode(void) const
			{
				return (static_cast<OmniSource *>(EditorManipulator::GetTargetNode()));
			}
			
			OmniSourceObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			
			void Update(void);
			void Render(const ManipulatorRenderData *renderData);
	};
	
	
	class DirectedSourceManipulator : public SourceManipulator, public ProjectionVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			DirectedSourceManipulator(DirectedSource *directedSource);
			~DirectedSourceManipulator();
			
			DirectedSource *GetTargetNode(void) const
			{
				return (static_cast<DirectedSource *>(EditorManipulator::GetTargetNode()));
			}
			
			DirectedSourceObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			
			void Update(void); 
			void Render(const ManipulatorRenderData *renderData);
	}; 
} 
 

#endif 

// ZYURVUR
