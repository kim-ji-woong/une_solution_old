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


#ifndef C4EditorSupport_h
#define C4EditorSupport_h


#include "C4EditorPages.h"
#include "C4EditorManipulators.h"
#include "C4Configuration.h"
#include "C4FilePicker.h"


namespace C4
{
	enum
	{
		kObjectEditor	= 'EDIT'
	};
	
	
	enum
	{
		kEditorShowGridlines		= 1 << 0,
		kEditorShowViewportInfo		= 1 << 1,
		kEditorSnapToGrid			= 1 << 2,
		kEditorShowBackfaces		= 1 << 5,
		kEditorExpandWorlds			= 1 << 6,
		kEditorRenderLighting		= 1 << 7,
		kEditorDrawFromCenter		= 1 << 8,
		kEditorExpandModels			= 1 << 9,
		kEditorCapGeometry			= 1 << 16
	};
	
	
	enum
	{
		kViewportModeTop,
		kViewportModeBottom,
		kViewportModeFront,
		kViewportModeBack,
		kViewportModeRight,
		kViewportModeLeft,
		kViewportModeOrthoCount,
		
		kViewportModeFrustum = kViewportModeOrthoCount,
		kViewportModeGraph,
		kViewportModeCount
	};
	
	
	enum
	{
		kEditorTrackNode	= 0,
		kEditorTrackVertex	= 1
	};
	
	
	enum
	{
		kMeshOriginMin		= 0,
		kMeshOriginCenter	= 1,
		kMeshOriginMax		= 2
	};
	
	
	class ViewportsPage;
	class MaterialContainer;
	
	
	//# \class	EditorObject	Encapsulates information about the editor state of a world.
	//
	//# \def	class EditorObject : public Object
	//
	//# \ctor	EditorObject();
	//
	//# \desc
	//# The $Editor$ class encapsulates information about the editor state of a world.
	//
	//# \base	WorldMgr/Object		An editor object is a special object that is attached to the root zone.
	
	
	//# \function	EditorObject::AddEditorPage		Adds an editor page instance to an editor window.
	//
	//# \proto	void AddEditorPage(EditorPage *page);
	//
	//# \param	page	The editor page to add to the editor window.
	//
	//# \desc
	//# The $AddEditorPage$ function adds the editor page specified by the $page$ parameter to the set of pages
	//# available for an editor window. This function is normally called from within the $@EditorPlugin::Initialize@$ function.
	//
	//# \also	$@EditorPage@$
	
	
	class EditorObject : public Object
	{
		private:
			
			unsigned_int32				editorFlags;
			unsigned_int32				selectionMask;
			unsigned_int32				bookLocation;
			
			float						gridLineSpacing;
			int32						majorLineInterval; 
			ColorRGB					gridColor;
			 
			float						snapAngle; 
			float						symbolSize; 
			float						cameraSpeed;
			 
			Zone						*targetZone;
			
			int32						currentViewportLayout;
			int32						previousViewportLayout; 
			int32						fullViewportIndex;
			
			int32						viewportMode[kEditorViewportCount];
			Transform4D					viewportTransform[kEditorViewportCount]; 
			Vector2D					viewportData[kEditorViewportCount];
			
			unsigned_int32				meshOriginSettings;
			
			List<MaterialContainer>		materialList;
			MaterialContainer			*selectedMaterial;
			
			List<EditorPage>			pageList;
			ViewportsPage				*viewportsPage;
			PaintPage					*paintPage;
			
			~EditorObject();
			
			static void TargetZoneLinkProc(Node *node, void *cookie);
			
			void ConfirmMaterialContainer(MaterialObject *object);
			static void ReplaceMaterial(MaterialObject *oldMaterial, MaterialObject *newMaterial, Node *root);
		
		public:
			
			EditorObject();
			
			unsigned_int32 GetEditorFlags(void) const
			{
				return (editorFlags);
			}
			
			void SetEditorFlags(unsigned_int32 flags)
			{
				editorFlags = flags;
			}
			
			unsigned_int32 GetSelectionMask(void) const
			{
				return (selectionMask);
			}
			
			void SetSelectionMask(unsigned_int32 mask)
			{
				selectionMask = mask;
			}
			
			unsigned_int32 GetBookLocation(void) const
			{
				return (bookLocation);
			}
			
			void SetBookLocation(unsigned_int32 location)
			{
				bookLocation = location;
			}
			
			float GetGridLineSpacing(void) const
			{
				return (gridLineSpacing);
			}
			
			void SetGridLineSpacing(float spacing)
			{
				gridLineSpacing = spacing;
			}
			
			int32 GetMajorLineInterval(void) const
			{
				return (majorLineInterval);
			}
			
			void SetMajorLineInterval(int32 interval)
			{
				majorLineInterval = interval;
			}
			
			const ColorRGB& GetGridColor(void) const
			{
				return (gridColor);
			}
			
			void SetGridColor(const ColorRGB& color)
			{
				gridColor = color;
			}
			
			float GetSnapAngle(void) const
			{
				return (snapAngle);
			}
			
			void SetSnapAngle(float angle)
			{
				snapAngle = angle;
			}
			
			float GetSymbolSize(void) const
			{
				return (symbolSize);
			}
			
			void SetSymbolSize(float size)
			{
				symbolSize = size;
			}
			
			float GetCameraSpeed(void) const
			{
				return (cameraSpeed);
			}
			
			void SetCameraSpeed(float speed)
			{
				cameraSpeed = speed;
			}
			
			Zone *GetTargetZone(void) const
			{
				return (targetZone);
			}
			
			void SetTargetZone(Zone *zone)
			{
				targetZone = zone;
			}
			
			int32 GetCurrentViewportLayout(void) const
			{
				return (currentViewportLayout);
			}
			
			int32 GetPreviousViewportLayout(void) const
			{
				return (previousViewportLayout);
			}
			
			void SetPreviousViewportLayout(int32 layout)
			{
				previousViewportLayout = layout;
			}
			
			int32 GetFullViewportIndex(void) const
			{
				return (fullViewportIndex);
			}
			
			void SetFullViewportIndex(int32 index)
			{
				fullViewportIndex = index;
			}
			
			int32 GetViewportMode(int32 index) const
			{
				return (viewportMode[index]);
			}
			
			const Transform4D& GetViewportTransform(int32 index) const
			{
				return (viewportTransform[index]);
			}
			
			void SetViewportTransform(int32 index, const Transform4D& transform)
			{
				viewportTransform[index] = transform;
			}
			
			const Vector2D& GetViewportData(int32 index) const
			{
				return (viewportData[index]);
			}
			
			void SetViewportData(int32 index, const Vector2D& data)
			{
				viewportData[index] = data;
			}
			
			unsigned_int32 GetMeshOriginSettings(void) const
			{
				return (meshOriginSettings);
			}
			
			void SetMeshOriginSettings(unsigned_int32 settings)
			{
				meshOriginSettings = settings;
			}
			
			List<MaterialContainer> *GetMaterialList(void)
			{
				return (&materialList);
			}
			
			MaterialContainer *GetSelectedMaterial(void) const
			{
				return (selectedMaterial);
			}
			
			void SelectMaterial(MaterialContainer *material)
			{
				selectedMaterial = material;
			}
			
			EditorPage *GetFirstEditorPage(void) const
			{
				return (pageList.First());
			}
			
			PaintPage *GetPaintPage(void) const
			{
				return (paintPage);
			}
			
			void Prepack(List<Object> *linkList) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void Preprocess(Editor *editor);
			
			void SetCurrentViewportLayout(int32 layout);
			void SetViewportMode(int32 index, int32 mode);
			
			List<MaterialContainer> *UpdateMaterialList(const Node *root);
			MaterialContainer *FindMaterialContainer(const MaterialObject *materialObject) const;
			void CleanupMaterials(Node *root);
			
			C4EDITORAPI MaterialObject *FindMatchingMaterial(const MaterialObject *materialObject) const;
			C4EDITORAPI MaterialObject *FindNamedMaterial(const char *name) const;
			
			C4EDITORAPI void AddEditorPage(EditorPage *page);
	};
	
	
	class WorldSavePicker : public FilePicker
	{
		private:
			
			CheckWidget		*stripBox;
		
		public:
			
			WorldSavePicker();
			~WorldSavePicker();
			
			bool GetStripFlag(void) const
			{
				return (stripBox->GetValue() != 0);
			}
			
			void Preprocess(void);
	};
	
	
	class EditorSettingsWindow : public Window, public LinkTarget<EditorSettingsWindow>
	{
		private:
			
			Editor					*worldEditor;
			
			PushButtonWidget		*okayButton;
			PushButtonWidget		*cancelButton;
			
			ConfigurationWidget		*configurationWidget;
		
		public:
			
			EditorSettingsWindow(Editor *editor);
			~EditorSettingsWindow();
			
			void Preprocess(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
}


#endif

// ZYURVUR
