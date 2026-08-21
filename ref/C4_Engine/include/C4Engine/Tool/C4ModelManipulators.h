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


#ifndef C4ModelManipulators_h
#define C4ModelManipulators_h


#include "C4EditorManipulators.h"
#include "C4Models.h"


namespace C4
{
	class BoneManipulator : public EditorManipulator
	{
		private:
			
			List<Attribute>			boneAttributeList;
			DiffuseAttribute		boneDiffuseColor;
			Renderable				boneRenderable;
			Point3D					boneVertex[2];
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
		
		public:
			
			BoneManipulator(Bone *bone);
			~BoneManipulator();
			
			Bone *GetTargetNode(void) const
			{
				return (static_cast<Bone *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
			
			void InvalidateNode(void);
			void Render(const ManipulatorRenderData *renderData);
	};
	
	
	class ModelManipulator : public EditorManipulator
	{
		public:
			
			ModelManipulator(Model *model);
			~ModelManipulator();
			
			Model *GetTargetNode(void) const
			{
				return (static_cast<Model *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
			
			void Preprocess(void);
			
			void Show(void);
			void Hide(void);
			
			void Select(void);
			void Unselect(void);
			
			void HandleDelete(bool undoable);
			void HandleUndelete(void);
			void HandleSettingsUpdate(void);
			
			Box3D CalculateNodeBoundingBox(void) const;
			Box3D CalculateWorldBoundingBox(void) const;
			
			void ExpandModel(void);
			void CollapseModel(void);
	};
}


#endif

// ZYURVUR
