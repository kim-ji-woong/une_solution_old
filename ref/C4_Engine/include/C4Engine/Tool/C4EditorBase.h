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


#ifndef C4EditorBase_h
#define C4EditorBase_h


//# \component	World Editor
//# \prefix		WorldEditor/


#include "C4Interface.h"


namespace C4
{
	#ifdef C4EDITOR
	
		#define C4EDITORAPI C4MODULEEXPORT
	
	#else
	
		#define C4EDITORAPI C4MODULEIMPORT
	
	#endif
	
	
	typedef Type	EditorEventType;
	typedef Type	EditorViewportType;
	
	
	enum
	{
		kMaxEditorToolBookCount		= 4
	};
	
	
	enum
	{
		kInvalidTriangleIndex		= 0xFFFFFFFF
	};
	
	
	enum
	{
		kEditorViewportTopLeft,
		kEditorViewportTopRight,
		kEditorViewportBottomLeft,
		kEditorViewportBottomRight,
		kEditorViewportLeft,
		kEditorViewportRight,
		kEditorViewportTop,
		kEditorViewportBottom,
		kEditorViewportCount
	};
	
	
	//# \enum	EditorEventType
	
	enum
	{
		kEditorEventSelectionUpdated			= 'SLCT',		//## The selection has been changed in some way.
		kEditorEventNodeCreated					= 'NODE',		//## A new node has been created and added to the world.
		kEditorEventGizmoMoved					= 'GZMO',		//## The gizmo has been moved to a different node.
		kEditorEventGizmoTargetInvalidated		= 'GINV',		//## The gizmo target node has been invalidated.
		kEditorEventGizmoTargetModified			= 'GMOD',		//## The gizmo target node, or one of its associated objects, has been modified in some way.
		kEditorEventMaterialSelected			= 'MSEL',		//## The material selection has changed.
		kEditorEventMaterialModified			= 'MATL',		//## The selected material has been modified.
		kEditorEventTexcoordModified			= 'TEXC',		//## The texture coordinates of a geometry have been modified.
		kEditorEventNodeInfoModified			= 'INFO',		//## Settings for the selected nodes have been modified.
		kEditorEventNodesPasted					= 'PAST'		//## New nodes have been pasted into the scene and are now selected.
	};
	
	
	class Node;
	class Camera;
	class Editor;
	class EditorGizmo;
	class MaterialObject;
	
	
	class EditorEvent
	{
		private:
			
			EditorEventType		eventType;
		
		public:
			
			C4EDITORAPI EditorEvent(EditorEventType type);
			C4EDITORAPI virtual ~EditorEvent();
			
			EditorEventType GetEventType(void) const
			{
				return (eventType);
			}
	};
	
	
	class NodeEditorEvent : public EditorEvent
	{
		private:
			
			Node		*eventNode;
		 
		public:
			 
			C4EDITORAPI NodeEditorEvent(EditorEventType type, Node *node); 
			C4EDITORAPI ~NodeEditorEvent(); 
			
			Node *GetEventNode(void) const 
			{
				return (eventNode);
			}
	}; 
	
	
	class GizmoEditorEvent : public EditorEvent
	{ 
		private:
			
			Node		*eventGizmoTarget;
		
		public:
			
			C4EDITORAPI GizmoEditorEvent(EditorEventType type, Node *gizmoTarget);
			C4EDITORAPI ~GizmoEditorEvent();
			
			Node *GetEventGizmoTarget(void) const
			{
				return (eventGizmoTarget);
			}
	};
	
	
	class MaterialEditorEvent : public EditorEvent
	{
		private:
			
			MaterialObject		*eventMaterialObject;
		
		public:
			
			C4EDITORAPI MaterialEditorEvent(EditorEventType type, MaterialObject *materialObject);
			C4EDITORAPI ~MaterialEditorEvent();
			
			MaterialObject *GetEventMaterialObject(void) const
			{
				return (eventMaterialObject);
			}
	};
	
	
	struct PickData
	{
		float				rayParam;
		int32				pickIndex[2];
		Point3D				pickPoint;
		Vector3D			pickNormal;
		unsigned_int32		triangleIndex;
	};
	
	
	struct ManipulatorHandleData
	{
		unsigned_int32		handleFlags;
		int32				oppositeIndex;
	};
	
	
	struct ManipulatorViewportData
	{
		int32				viewportIndex;
		EditorViewportType	viewportType;
		
		float				viewportScale;
		unsigned_int32		editorFlags;
		
		Camera				*viewportCamera;
	};
	
	
	struct ManipulatorRenderData : ManipulatorViewportData
	{
		List<Renderable>	*geometryList;
		List<Renderable>	*manipulatorList;
		List<Renderable>	*gizmoList;
		List<Renderable>	*connectorList;
		List<Renderable>	*handleList;
	};
	
	
	struct ManipulatorResizeData
	{
		unsigned_int32		resizeFlags;
		unsigned_int32		handleFlags;
		int32				handleIndex;
		
		Vector3D			resizeDelta;
		mutable Vector3D	positionOffset;
	};
	
	
	struct EditorTrackData : ManipulatorViewportData
	{
		Ray						worldRay;
		
		Point2D					currentPosition;
		Point2D					previousPosition;
		Point2D					anchorPosition;
		Point2D					snappedCurrentPosition;
		Point2D					snappedPreviousPosition;
		Point2D					snappedAnchorPosition;
		
		Point2D					currentViewportPosition;
		Point2D					previousViewportPosition;
		Point2D					anchorViewportPosition;
		
		Node					*currentPickNode;
		Point3D					currentPickPoint;
		Vector3D				currentPickNormal;
		
		unsigned_int32			mouseEventFlags;
		unsigned_int32			currentModifierKeys;
		unsigned_int32			previousModifierKeys;
		
		int32					trackType;
		Node					*trackNode;
		Node					*superNode;
		Transform4D				originalTransform;
		Vector2D				currentSize;
		
		EditorGizmo				*gizmo;
		int32					gizmoIndex;
		
		ManipulatorResizeData	resizeData;
	};
}


#endif

// ZYURVUR
