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


#ifndef C4EditorTools_h
#define C4EditorTools_h


#include "C4Viewports.h"
#include "C4EditorBase.h"


namespace C4
{
	enum
	{
		kEditorToolNodeBoxSelect,
		kEditorToolNodeSelect,
		kEditorToolNodeMove,
		kEditorToolNodeRotate,
		kEditorToolNodeResize,
		kEditorToolConnect,
		kEditorToolSurfaceSelect,
		kEditorToolViewportScroll,
		kEditorToolViewportZoom,
		kEditorToolViewportBoxZoom,
		kEditorToolOrbitCamera,
		kEditorToolFreeCamera,
		kEditorToolCount
	};
	
	
	enum
	{
		kFreeCameraForward		= 1 << 0,
		kFreeCameraBackward		= 1 << 1,
		kFreeCameraLeft			= 1 << 2,
		kFreeCameraRight		= 1 << 3,
		kFreeCameraUp			= 1 << 4,
		kFreeCameraDown			= 1 << 5
	};
	
	
	class Editor;
	class EditorManipulator;
	struct EditorTrackData;
	
	
	class EditorTool
	{
		protected:
			
			C4EDITORAPI EditorTool();
		
		public:
			
			C4EDITORAPI virtual ~EditorTool();
			
			C4EDITORAPI virtual void Engage(Editor *editor, void *cookie = nullptr);
			C4EDITORAPI virtual void Disengage(Editor *editor, void *cookie = nullptr);
			
			C4EDITORAPI virtual bool BeginTool(Editor *editor, EditorTrackData *trackData);
			C4EDITORAPI virtual bool TrackTool(Editor *editor, EditorTrackData *trackData);
			C4EDITORAPI virtual bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class StandardEditorTool : public EditorTool
	{
		private:
			
			IconButtonWidget						*toolButton;
			WidgetObserver<StandardEditorTool>		toolObserver;
			
			void HandleToolButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		protected:
			
			StandardEditorTool(IconButtonWidget *widget);
			
			static bool SelectNode(Editor *editor, EditorTrackData *trackData);
		
		public:
			
			~StandardEditorTool();
			
			void Engage(Editor *editor, void *cookie = nullptr);
			void Disengage(Editor *editor, void *cookie = nullptr);
	};
	
	
	class NodeSelectTool : public StandardEditorTool
	{
		public:
			
			NodeSelectTool(IconButtonWidget *widget);
			~NodeSelectTool();
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class NodeMoveTool : public StandardEditorTool
	{
		private:
			 
			bool			undoDataFlag;
			 
			Renderable		reparentRenderable; 
			Point3D			reparentVertex[20]; 
			
			void CalculateReparentVertices(const Point2D& position); 
			EditorManipulator *GetReparentNode(Editor *editor, EditorTrackData *trackData);
		
		public:
			 
			NodeMoveTool(IconButtonWidget *widget);
			~NodeMoveTool();
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData); 
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class NodeRotateTool : public StandardEditorTool
	{
		private:
			
			bool		undoDataFlag;
			bool		negateAngle;
			
			Point3D		rotationCenter;
			float		accumAngle;
		
		public:
			
			NodeRotateTool(IconButtonWidget *widget);
			~NodeRotateTool();
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class NodeScaleTool : public StandardEditorTool
	{
		private:
			
			bool		undoDataFlag;
		
		public:
			
			NodeScaleTool(IconButtonWidget *widget);
			~NodeScaleTool();
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class ConnectTool : public StandardEditorTool
	{
		public:
			
			ConnectTool(IconButtonWidget *widget);
			~ConnectTool();
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class SurfaceSelectTool : public StandardEditorTool
	{
		public:
			
			SurfaceSelectTool(IconButtonWidget *widget);
			~SurfaceSelectTool();
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class ViewportScrollTool : public StandardEditorTool
	{
		private:
			
			Point3D		initalCameraPosition;
			Cursor		*previousCursor;
		
		public:
			
			ViewportScrollTool(IconButtonWidget *widget);
			~ViewportScrollTool();
			
			void Engage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class ViewportZoomTool : public StandardEditorTool
	{
		public:
			
			ViewportZoomTool(IconButtonWidget *widget);
			~ViewportZoomTool();
			
			void Engage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class DragRectTool : public StandardEditorTool
	{
		protected:
			
			DragRect		dragRect;
			
			DragRectTool(IconButtonWidget *widget, const ColorRGBA& color);
		
		public:
			
			~DragRectTool();
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class BoxSelectTool : public DragRectTool
	{
		private:
			
			static void UnselectAllTemp(Editor *editor);
		
		public:
			
			BoxSelectTool(IconButtonWidget *widget);
			~BoxSelectTool();
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class ViewportBoxZoomTool : public DragRectTool
	{
		public:
			
			ViewportBoxZoomTool(IconButtonWidget *widget);
			~ViewportBoxZoomTool();
			
			void Engage(Editor *editor, void *cookie);
			
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class OrbitCameraTool : public StandardEditorTool
	{
		private:
			
			Point3D		orbitCenter;
		
		public:
			
			OrbitCameraTool(IconButtonWidget *widget);
			~OrbitCameraTool();
			
			void Engage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class FreeCameraTool : public StandardEditorTool
	{
		private:
			
			float				cameraSpeed;
			unsigned_int32		cameraFlags;
			
			Cursor				*previousCursor;
		
		public:
			
			FreeCameraTool(IconButtonWidget *widget);
			~FreeCameraTool();
			
			unsigned_int32 GetCameraFlags(void) const
			{
				return (cameraFlags);
			}
			
			void SetCameraFlags(unsigned_int32 flags)
			{
				cameraFlags = flags;
			}
			
			void Engage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
}


#endif

// ZYURVUR
