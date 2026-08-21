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


#ifndef C4EditorCommands_h
#define C4EditorCommands_h


#include "C4Threads.h"
#include "C4EditorBase.h"


namespace C4
{
	enum
	{
		kMaxCombineGeometryCount	= 4
	};
	
	
	enum
	{
		kEditorMenuUndo,
		kEditorMenuCut,
		kEditorMenuCopy,
		kEditorMenuPaste,
		kEditorMenuPasteSubnodes,
		kEditorMenuClear,
		kEditorMenuSelectSubtree,
		kEditorMenuSelectSuperNode,
		kEditorMenuLockSelection,
		kEditorMenuUnlockSelection,
		kEditorMenuDuplicate,
		kEditorMenuClone,
		kEditorMenuCopyTransform,
		kEditorMenuPasteTransform,
		
		kEditorMenuGetInfo,
		kEditorMenuEditController,
		kEditorMenuGroup,
		kEditorMenuResetTransform,
		kEditorMenuAlignToGrid,
		kEditorMenuSetTargetZone,
		kEditorMenuMoveToTargetZone,
		kEditorMenuConnectNode,
		kEditorMenuUnconnectNode,
		kEditorMenuAutoConnectPortal,
		kEditorMenuConnectRootZone,
		kEditorMenuSelectConnectedNode,
		kEditorMenuMoveViewportCameraToNode,
		kEditorMenuOpenInstancedWorld,
		
		kEditorMenuRebuildGeometry,
		kEditorMenuRebuildWithNewPath,
		kEditorMenuRecalculateNormals,
		kEditorMenuBakeTransformIntoVertices,
		kEditorMenuRepositionMeshOrigin,
		kEditorMenuSetMaterial,
		kEditorMenuRemoveMaterial,
		kEditorMenuCombineDetailLevels,
		kEditorMenuSeparateDetailLevels,
		kEditorMenuConvertToGenericMesh,
		kEditorMenuMergeGeometry,
		kEditorMenuInvertGeometry,
		kEditorMenuIntersectGeometry,
		kEditorMenuUnionGeometry,
		kEditorMenuGenerateAmbientOcclusion,
		kEditorMenuRemoveAmbientOcclusion,
		
		kEditorMenuHideSelected,
		kEditorMenuShowBackfaces,
		kEditorMenuExpandWorlds,
		kEditorMenuExpandModels,
		kEditorMenuRenderLighting,
		kEditorMenuDrawFromCenter,
		kEditorMenuCapGeometry,
		
		kEditorMenuItemCount
	};
	
	
	struct PickData;
	
	
	class MeshOriginWindow : public Window
	{
		private:
			
			Editor					*worldEditor;
			
			PushButtonWidget		*okayButton;
			PushButtonWidget		*cancelButton;
			
			RadioWidget				*radioButton[3][3];
			
			void CommitSettings(void) const;
		
		public:
			
			MeshOriginWindow(Editor *editor);
			~MeshOriginWindow();
			
			void Preprocess(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData); 
	};
	 
	 
	class GenerateAmbientOcclusionWindow : public Window 
	{
		private: 
			
			class AmbientOcclusionJob : public Job
			{
				private: 
					
					GenerateAmbientOcclusionWindow		*jobWindow;
				
				public: 
					
					AmbientOcclusionJob(GenerateAmbientOcclusionWindow *window, ExecuteProc *execProc, void *cookie);
					
					GenerateAmbientOcclusionWindow *GetJobWindow(void) const
					{
						return (jobWindow);
					}
			};
			
			Editor					*worldEditor;
			float					blockageMultiplier;
			
			int32					jobCount;
			Job						**jobTable;
			
			Lock					jobLock;
			
			PushButtonWidget		*startButton;
			PushButtonWidget		*cancelButton;
			EditTextWidget			*intensityBox;
			TextWidget				*inputText;
			
			PushButtonWidget		*stopButton;
			ProgressWidget			*progressBar;
			BorderWidget			*borderWidget;
			TextWidget				*messageText;
			
			void StartJob(void);
			
			static bool DetectCollision(const Node *root, Ray *ray, PickData *pickData);
			static void GenerateAmbientOcclusionJob(Job *job, void *cookie);
		
		public:
			
			GenerateAmbientOcclusionWindow(Editor *editor);
			~GenerateAmbientOcclusionWindow();
			
			void Preprocess(void);
			void Move(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
}


#endif

// ZYURVUR
