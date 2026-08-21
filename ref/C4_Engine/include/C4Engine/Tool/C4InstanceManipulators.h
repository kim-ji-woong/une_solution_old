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


#ifndef C4InstanceManipulators_h
#define C4InstanceManipulators_h


#include "C4EditorManipulators.h"
#include "C4Instances.h"


namespace C4
{
	class InstanceManipulator : public EditorManipulator
	{
		public:
			
			InstanceManipulator(Instance *instance);
			~InstanceManipulator();
			
			Instance *GetTargetNode(void) const
			{
				return (static_cast<Instance *>(EditorManipulator::GetTargetNode()));
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
			
			bool MaterialSettable(void) const;
			void SetMaterial(MaterialObject *materialObject);
			
			Box3D CalculateNodeBoundingBox(void) const;
			Box3D CalculateWorldBoundingBox(void) const;
			
			void ExpandWorld(void);
			void CollapseWorld(void);
	};
}


#endif

// ZYURVUR
