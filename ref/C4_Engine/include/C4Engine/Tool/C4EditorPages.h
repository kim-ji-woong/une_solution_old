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


#ifndef C4EditorPages_h
#define C4EditorPages_h


//# \component	World Editor
//# \prefix		WorldEditor/


#include "C4WorldEditor.h"
#include "C4Paint.h"


namespace C4
{
	typedef Type	PageType;
	
	
	const float kEditorPageWidth = 163.0F;
	
	
	enum
	{
		kEditorPageGeometries		= 'GEOM',
		kEditorPageCameras			= 'CAMR',
		kEditorPageLights			= 'LITE',
		kEditorPageSources			= 'SORC',
		kEditorPageZones			= 'ZONE',
		kEditorPagePortals			= 'PORT',
		kEditorPageSpaces			= 'SPAC',
		kEditorPageMarkers			= 'MARK',
		kEditorPageTriggers			= 'TRIG',
		kEditorPageEffects			= 'EFCT',
		kEditorPageParticles		= 'PART',
		kEditorPagePhysics			= 'PHYS',
		kEditorPageModels			= 'MODL',
		kEditorPageWorlds			= 'WRLD',
		kEditorPageSkyboxes			= 'SKYB',
		kEditorPageImpostors		= 'IPST',
		kEditorPagePaths			= 'PATH',
		kEditorPagePlacement		= 'PLAC',
		kEditorPageMaterial			= 'MATL',
		kEditorPageSelectionMask	= 'MASK',
		kEditorPageVisibility		= 'VISI',
		kEditorPageViewports		= 'VIEW',
		kEditorPageTransform		= 'XFRM',
		kEditorPageTextureMapping	= 'TXTR',
		kEditorPageInfo				= 'INFO',
		kEditorPageGrid				= 'GRID',
		kEditorPageFind				= 'FIND',
		kEditorPagePaint			= 'PANT'
	};
	
	
	enum
	{
		kEditorNodeGeometry,
		kEditorNodeCamera,
		kEditorNodeLight,
		kEditorNodeSource,
		kEditorNodeZone,
		kEditorNodePortal,
		kEditorNodeSpace,
		kEditorNodeMarker,
		kEditorNodeTrigger,
		kEditorNodeEffect,
		kEditorNodeModel,
		kEditorNodeGroup,
		kEditorNodeCount,
		kEditorNonpersistentMask	= 1 << 31
	};
	
	
	enum
	{
		kEditorLayout1,
		kEditorLayout4,
		kEditorLayout2H,
		kEditorLayout2V,
		kEditorLayout3L,
		kEditorLayout3R,
		kEditorLayout3T,
		kEditorLayout3B,
		kEditorLayoutCount
	};
	
	
	class ParticleSystemRegistration;
	class ModelRegistration;
	class MaterialBox;
	class MaterialContainer;
	class MaterialWindow;
	class EditorObject;
	class ColorPicker;
	class PathMarker;
	struct EditorToolData;
	
	
	class TypeWidget : public TextWidget
	{
		private:
			
			Type		itemType; 
		
		public: 
			 
			C4EDITORAPI TypeWidget(const Vector2D& size, const char *text, Type type); 
			C4EDITORAPI ~TypeWidget();
			 
			Type GetItemType(void) const
			{
				return (itemType);
			} 
	};
	
	
	//# \class	EditorPage		Represents a World Editor tool page. 
	//
	//# The $EditorPage$ class represents a World Editor tool page.
	//
	//# \def	class EditorPage : public Page, public Packable, public ListElement<EditorPage>, public MapElement<EditorPage>
	//
	//# \ctor	EditorPage(PageType type, const char *panelName);
	//
	//# The constructor has protected access. Only instances of $EditorPage$ subclasses can be created.
	//
	//# \param	type		The type of the page.
	//# \param	panelName	The name of the panel resource to load for the page.
	//
	//# \desc
	//# The $EditorPage$ class is the base class for all World Editor tool pages.
	//
	//# \privbase	Page								The $EditorPage$ class is an extension of a general tool page.
	//# \base		Utilities/ListElement<EditorPage>	Used internally by the World Editor.
	//# \base		Utilities/MapElement<EditorPage>	Used internally by the World Editor.
	
	
	class EditorPage : public Page, public ListElement<EditorPage>, public MapElement<EditorPage>
	{
		friend class EditorObject;
		
		private:
			
			PageType			pageType;
			PageType			prevPageType;
			
			Editor				*worldEditor;
			
			unsigned_int32		pageState;
			int32				bookIndex;
			
			MenuItemWidget		*menuItem;
		
		protected:
			
			C4EDITORAPI EditorPage(PageType type, const char *panelName);
			
			void InitialShow(void)
			{
				pageState &= ~kWidgetHidden;
			}
		
		public:
			
			typedef ConstCharKey KeyType;
			
			C4EDITORAPI virtual ~EditorPage();
			
			PageType GetPageType(void) const
			{
				return (pageType);
			}
			
			KeyType GetKey(void) const
			{
				return (GetPageTitle());
			}
			
			Editor *GetEditor(void) const
			{
				return (worldEditor);
			}
			
			void SetEditor(Editor *editor)
			{
				worldEditor = editor;
			}
			
			int32 GetBookIndex(void) const
			{
				return (bookIndex);
			}
			
			void SetMenuItem(MenuItemWidget *widget)
			{
				menuItem = widget;
			}
			
			C4EDITORAPI void Pack(Packer& data, unsigned_int32 packFlags) const;
			C4EDITORAPI void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			C4EDITORAPI void SetWidgetState(unsigned_int32 state);
			C4EDITORAPI void Preprocess(void);

			void HandlePageMenuItem(Widget *menuItem, const WidgetEventData *eventData);
	};
	
	
	class GeometriesPage : public EditorPage, public EditorTool
	{
		private:
			
			enum
			{
				kEditorGeometryPlate,
				kEditorGeometryDisk,
				kEditorGeometryHole,
				kEditorGeometryAnnulus,
				kEditorGeometryBox,
				kEditorGeometryPyramid,
				kEditorGeometryCylinder,
				kEditorGeometryCone,
				kEditorGeometrySphere,
				kEditorGeometryDome,
				kEditorGeometryTorus,
				kEditorGeometryTruncatedCone,
				kEditorGeometryTube,
				kEditorGeometryExtrusion,
				kEditorGeometryRevolution,
				kEditorGeometryCloth,
				kEditorGeometryCount
			};
			
			int32								currentTool;

			IconButtonWidget					*geometryButton[kEditorGeometryCount];
			WidgetObserver<GeometriesPage>		geometryButtonObserver;
			
			void HandleGeometryButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			GeometriesPage();
			~GeometriesPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class CamerasPage : public EditorPage, public EditorTool
	{
		private:
			
			enum
			{
				kEditorCameraFrustum,
				kEditorCameraCount
			};
			
			int32							currentTool;

			IconButtonWidget				*cameraButton[kEditorCameraCount];
			WidgetObserver<CamerasPage>		cameraButtonObserver;
			
			void HandleCameraButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			CamerasPage();
			~CamerasPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class LightsPage : public EditorPage, public EditorTool
	{
		private:
			
			enum
			{
				kEditorLightPoint,
				kEditorLightCube,
				kEditorLightSpot,
				kEditorLightInfinite,
				kEditorLightDepth,
				kEditorLightLandscape,
				kEditorLightCount
			};
			
			int32							currentTool;

			IconButtonWidget				*lightButton[kEditorLightCount];
			WidgetObserver<LightsPage>		lightButtonObserver;
			
			void HandleLightButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			LightsPage();
			~LightsPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class SourcesPage : public EditorPage, public EditorTool
	{
		private:
			
			enum
			{
				kEditorSourceAmbient,
				kEditorSourceOmni,
				kEditorSourceDirected,
				kEditorSourceCount
			};
			
			int32							currentTool;

			IconButtonWidget				*sourceButton[kEditorSourceCount];
			WidgetObserver<SourcesPage>		sourceButtonObserver;
			
			void HandleSourceButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			SourcesPage();
			~SourcesPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class ZonesPage : public EditorPage, public EditorTool
	{
		private:
			
			enum
			{
				kEditorZoneBox,
				kEditorZoneCylinder,
				kEditorZoneDome,
				kEditorZonePolygon,
				kEditorZoneCount
			};
			
			enum
			{
				kEditorZoneModeDraw,
				kEditorZoneModeTool
			};
			
			enum
			{
				kEditorZoneToolInsert,
				kEditorZoneToolRemove,
				kEditorZoneToolCount
			};
			
			int32						currentMode;
			int32						currentTool;

			IconButtonWidget			*zoneButton[kEditorZoneCount];
			IconButtonWidget			*toolButton[kEditorZoneToolCount];
			
			WidgetObserver<ZonesPage>	zoneButtonObserver;
			
			void HandleZoneButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			ZonesPage();
			~ZonesPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class PortalsPage : public EditorPage, public EditorTool
	{
		private:
			
			enum
			{
				kEditorPortalDirect,
				kEditorPortalRemote,
				kEditorPortalOcclusion,
				kEditorPortalCount
			};
			
			enum
			{
				kEditorPortalModeDraw,
				kEditorPortalModeTool
			};
			
			enum
			{
				kEditorPortalToolInsert,
				kEditorPortalToolRemove,
				kEditorPortalToolCount
			};
			
			int32							currentMode;
			int32							currentTool;

			IconButtonWidget				*portalButton[kEditorPortalCount];
			IconButtonWidget				*toolButton[kEditorPortalToolCount];
			
			WidgetObserver<PortalsPage>		portalButtonObserver;
			
			void HandlePortalButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			PortalsPage();
			~PortalsPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class SpacesPage : public EditorPage, public EditorTool
	{
		private:
			
			enum
			{
				kEditorSpaceFog,
				kEditorSpaceShadow,
				kEditorSpaceAmbient,
				kEditorSpaceAcoustics,
				kEditorSpaceOcclusion,
				kEditorSpacePaint,
				kEditorSpaceCount
			};
			
			int32							currentTool;

			IconButtonWidget				*spaceButton[kEditorSpaceCount];
			WidgetObserver<SpacesPage>		spaceButtonObserver;
			
			void HandleSpaceButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			SpacesPage();
			~SpacesPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class MarkersPage : public EditorPage, public EditorTool
	{
		private:
			
			enum
			{
				kEditorMarkerLocator,
				kEditorMarkerConnection,
				kEditorMarkerCube,
				kEditorMarkerCount
			};
			
			int32							currentTool;
			
			ListWidget						*locatorList;
			IconButtonWidget				*markerButton[kEditorMarkerCount];
			
			WidgetObserver<MarkersPage>		markerButtonObserver;
			
			void HandleMarkerButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			MarkersPage();
			~MarkersPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class TriggersPage : public EditorPage, public EditorTool
	{
		private:
			
			enum
			{
				kEditorTriggerBox,
				kEditorTriggerCylinder,
				kEditorTriggerSphere,
				kEditorTriggerCount
			};
			
			int32							currentTool;

			IconButtonWidget				*triggerButton[kEditorTriggerCount];
			WidgetObserver<TriggersPage>	triggerButtonObserver;
			
			void HandleTriggerButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			TriggersPage();
			~TriggersPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class EffectsPage : public EditorPage, public EditorTool
	{
		private:
			
			enum
			{
				kEditorEffectQuad,
				kEditorEffectFlare,
				kEditorEffectBeam,
				kEditorEffectTube,
				kEditorEffectFire,
				kEditorEffectPanel,
				kEditorEffectCount
			};
			
			int32							currentTool;
			
			IconButtonWidget				*effectButton[kEditorEffectCount];
			WidgetObserver<EffectsPage>		effectButtonObserver;
			
			void HandleEffectButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			EffectsPage();
			~EffectsPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class ParticlesPage : public EditorPage, public EditorTool
	{
		private:
			
			enum
			{
				kEditorEmitterBox,
				kEditorEmitterCylinder,
				kEditorEmitterSphere,
				kEditorEmitterCount
			};
			
			int32							currentTool;
			
			ListWidget						*particleSystemList;
			IconButtonWidget				*emitterButton[kEditorEmitterCount];
			
			WidgetObserver<ParticlesPage>	emitterButtonObserver;
			
			void HandleEmitterButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			ParticlesPage();
			~ParticlesPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class PhysicsPage : public EditorPage, public EditorTool
	{
		private:
			
			enum
			{
				kEditorShapeBox,
				kEditorShapePyramid,
				kEditorShapeCylinder,
				kEditorShapeCone,
				kEditorShapeSphere,
				kEditorShapeDome,
				kEditorShapeCapsule,
				kEditorShapeTruncatedPyramid,
				kEditorShapeTruncatedCone,
				kEditorShapeTruncatedDome,
				kEditorShapeCount
			};
			
			enum
			{
				kEditorJointSpherical,
				kEditorJointUniversal,
				kEditorJointDiscal,
				kEditorJointRevolute,
				kEditorJointCylindrical,
				kEditorJointPrismatic,
				kEditorJointCount
			};
			
			enum
			{
				kEditorFieldBox,
				kEditorFieldCylinder,
				kEditorFieldSphere,
				kEditorFieldCount
			};
			
			enum
			{
				kEditorPhysicsModePhysics,
				kEditorPhysicsModeShape,
				kEditorPhysicsModeJoint,
				kEditorPhysicsModeField
			};
			
			int32							currentMode;
			int32							currentTool;
			
			IconButtonWidget				*physicsButton;
			IconButtonWidget				*shapeButton[kEditorShapeCount];
			IconButtonWidget				*jointButton[kEditorJointCount];
			IconButtonWidget				*fieldButton[kEditorFieldCount];
			
			WidgetObserver<PhysicsPage>		physicsButtonObserver;
			
			void HandlePhysicsButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			PhysicsPage();
			~PhysicsPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class ModelsPage : public EditorPage, public EditorTool
	{
		private:
			
			int32							currentTool;
			
			IconButtonWidget				*modelButton;
			ListWidget						*modelList;
			
			WidgetObserver<ModelsPage>		modelButtonObserver;
			
			void HandleModelButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			ModelsPage();
			~ModelsPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class WorldsPage : public EditorPage, public EditorTool
	{
		private:
			
			enum
			{
				kWorldMenuSelectAll,
				kWorldMenuSelectSome,
				kWorldMenuReplaceSelected,
				kWorldMenuItemCount
			};
			
			class WorldWidget : public TextWidget, public MapElement<WorldWidget>
			{
				public:
					
					typedef StringKey KeyType;
					
					WorldWidget(const char *text);
					~WorldWidget();
					
					KeyType GetKey(void) const
					{
						return (GetText());
					}
			};
			
			class SelectSomeWindow : public Window, public Completable<SelectSomeWindow>
			{
				private:
					
					unsigned_int32			selectPercentage;
					
					PushButtonWidget		*okayButton;
					PushButtonWidget		*cancelButton;
					EditTextWidget			*percentBox;
				
				public:
					
					SelectSomeWindow(unsigned_int32 percentage);
					~SelectSomeWindow();
					
					unsigned_int32 GetSelectPercentage(void) const
					{
						return (selectPercentage);
					}
					
					void Preprocess(void);
					
					bool HandleKeyboardEvent(const KeyboardEventData *eventData);
					void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			};
			
			int32							currentTool;
			Map<WorldWidget>				worldWidgetMap;
			
			unsigned_int32					selectPercentage;
			
			IconButtonWidget				*worldButton;
			ListWidget						*worldList;
			
			IconButtonWidget				*menuButton;
			MenuItemWidget					*worldMenuItem[kWorldMenuItemCount];
			List<MenuItemWidget>			worldMenuItemList;
			
			EditorObserver<WorldsPage>		editorObserver;
			WidgetObserver<WorldsPage>		worldButtonObserver;
			WidgetObserver<WorldsPage>		worldListObserver;
			WidgetObserver<WorldsPage>		menuButtonObserver;
			
			void AddWorldWidget(const char *text);
			void AddZoneWorlds(const Zone *zone);
			void BuildWorldList(void);
			
			static void SelectAllZoneWorlds(Editor *editor, const Zone *zone, const char *worldName);
			static int32 GatherZoneWorlds(Editor *editor, const Zone *zone, const char *worldName, List<NodeReference> *worldList);
			
			void HandleCleanupMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData);
			void HandleSelectAllMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData);
			void HandleSelectSomeMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData);
			static void SelectSomeComplete(SelectSomeWindow *window, void *cookie);
			void HandleReplaceSelectedMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData);
			
			void HandleEditorEvent(Editor *editor, const EditorEvent& event);
			void HandleWorldButtonEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleWorldListEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleMenuButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			WorldsPage();
			~WorldsPage();
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class SkyboxesPage : public EditorPage, public EditorTool
	{
		private:
			
			int32							currentTool;
			
			IconButtonWidget				*skyboxButton;
			WidgetObserver<SkyboxesPage>	skyboxButtonObserver;
			
			void HandleSkyboxButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			SkyboxesPage();
			~SkyboxesPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class ImpostorsPage : public EditorPage, public EditorTool
	{
		private:
			
			int32							currentTool;
			
			IconButtonWidget				*impostorButton;
			WidgetObserver<ImpostorsPage>	impostorButtonObserver;
			
			void HandleImpostorButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			ImpostorsPage();
			~ImpostorsPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class PathsPage : public EditorPage, public EditorTool
	{
		private:
			
			enum
			{
				kEditorPathLinear,
				kEditorPathElliptical,
				kEditorPathBezier,
				kEditorPathCount
			};
			
			int32							currentTool;
			PathMarker						*targetPath;
			
			IconButtonWidget				*pathButton[kEditorPathCount];
			WidgetObserver<PathsPage>		pathButtonObserver;
			
			void HandlePathButtonEvent(Widget *widget, const WidgetEventData *eventData);
			
			static bool SnapToBeginning(const PathMarker *marker, const EditorTrackData *trackData);
		
		public:
			
			PathsPage();
			~PathsPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class PlacementPage : public EditorPage
	{
		private:
			
			int32							enableValue;
			int32							rotateValue;
			int32							tangentValue;
			int32							sinkValue;
			float							radiusValue;
			
			CheckWidget						*enableWidget;
			CheckWidget						*rotateWidget;
			CheckWidget						*tangentWidget;
			CheckWidget						*sinkWidget;
			EditTextWidget					*radiusWidget;
			PushButtonWidget				*applyButton;
			
			WidgetObserver<PlacementPage>	placementWidgetObserver;
			WidgetObserver<PlacementPage>	applyButtonObserver;
			
			void HandlePlacementWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleApplyButtonEvent(Widget *widget, const WidgetEventData *eventData);
			
			void ApplyRotation(const Editor *editor, Transform4D *transform) const;
			void ApplyTangent(const Editor *editor, Transform4D *transform, const Vector3D *normal = nullptr) const;
			void ApplySink(const Editor *editor, Transform4D *transform) const;
			
			static void ModifyPlacement(const Editor *editor, Transform4D *transform, const Vector3D& normal, void *cookie);
		
		public:
			
			PlacementPage();
			~PlacementPage();
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			void Preprocess(void);
	};
	
	
	class MaterialPage : public EditorPage, public EditorTool
	{
		private:
			
			enum
			{
				kEditorMaterialToolPickup,
				kEditorMaterialToolCount
			};
			
			int32							currentTool;
			
			IconButtonWidget				*toolButton[kEditorMaterialToolCount];
			MaterialWidget					*materialWidget;
			
			EditorObserver<MaterialPage>	editorObserver;
			WidgetObserver<MaterialPage>	toolButtonObserver;
			WidgetObserver<MaterialPage>	materialWidgetObserver;
			
			void HandleEditorEvent(Editor *editor, const EditorEvent& event);
			void HandleToolButtonEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleMaterialWidgetEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			MaterialPage();
			~MaterialPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class SelectionMaskPage : public EditorPage
	{
		private:
			
			IconButtonWidget					*maskButton[kEditorNodeCount];
			
			EditorObserver<SelectionMaskPage>	editorObserver;
			WidgetObserver<SelectionMaskPage>	maskButtonObserver;

			void HandleEditorEvent(Editor *editor, const EditorEvent& event);
			void HandleMaskButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			SelectionMaskPage();
			~SelectionMaskPage();
			
			void Preprocess(void);
	};
	
	
	class VisibilityPage : public EditorPage
	{
		private:
			
			IconButtonWidget					*showButton[kEditorNodeCount];
			IconButtonWidget					*hideButton[kEditorNodeCount];
			
			WidgetObserver<VisibilityPage>		visibilityButtonObserver;
			
			void HandleVisibilityButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			VisibilityPage();
			~VisibilityPage();
			
			void Preprocess(void);
	};
	
	
	class ViewportsPage : public EditorPage
	{
		private:
			
			IconButtonWidget				*layoutButton[kEditorLayoutCount];
			SliderWidget					*cameraSlider;
			
			WidgetObserver<ViewportsPage>	layoutButtonObserver;
			WidgetObserver<ViewportsPage>	cameraSliderObserver;
			
			void HandleLayoutButtonEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleCameraSliderEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			ViewportsPage();
			~ViewportsPage();
			
			void Preprocess(void);
			
			void SetViewportLayout(int32 layout);
			void SetCameraSpeed(float speed);
	};
	
	
	class TransformPage : public EditorPage
	{
		private:
			
			UndoType						undoType;
			bool							updateFlag;
			
			EditTextWidget					*positionTextWidget[3];
			EditTextWidget					*rotationTextWidget[3];
			EditTextWidget					*sizeTextWidget[kMaxObjectSizeCount];
			
			EditorObserver<TransformPage>	editorObserver;
			WidgetObserver<TransformPage>	positionTextObserver;
			WidgetObserver<TransformPage>	rotationTextObserver;
			WidgetObserver<TransformPage>	sizeTextObserver;
			
			void ClearTransform(void);
			void UpdateTransform(const Node *node);
			
			void HandleEditorEvent(Editor *editor, const EditorEvent& event);
			void HandlePositionTextEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleRotationTextEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleSizeTextEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			TransformPage();
			~TransformPage();
			
			void Preprocess(void);
	};
	
	
	class TextureMappingPage : public EditorPage, public EditorTool
	{
		private:
			
			enum
			{
				kEditorTextureToolOffset,
				kEditorTextureToolRotate,
				kEditorTextureToolScale,
				kEditorTextureToolCount
			};
			
			bool									undoDataFlag;
			int32									textureUndoType;
			
			Geometry								*targetGeometry;
			unsigned_int32							targetSurfaceIndex;
			
			int32									currentTool;
			IconButtonWidget						*toolButton[kEditorTextureToolCount];
			
			EditTextWidget							*offsetTextWidget[2];
			EditTextWidget							*scaleTextWidget[2];
			EditTextWidget							*rotationTextWidget;
			CheckWidget								*reflectionCheckWidget;
			PopupMenuWidget							*modePopupMenu[2];
			PopupMenuWidget							*planePopupMenu;
			
			EditorObserver<TextureMappingPage>		editorObserver;
			WidgetObserver<TextureMappingPage>		toolButtonObserver;
			WidgetObserver<TextureMappingPage>		offsetTextObserver;
			WidgetObserver<TextureMappingPage>		scaleTextObserver;
			WidgetObserver<TextureMappingPage>		rotationTextObserver;
			WidgetObserver<TextureMappingPage>		reflectionBoxObserver;
			WidgetObserver<TextureMappingPage>		modeMenuObserver;
			
			void UpdateTextureAlignData(void);
			
			void HandleEditorEvent(Editor *editor, const EditorEvent& event);
			void HandleToolButtonEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleOffsetTextEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleScaleTextEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleRotationTextEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleReflectionBoxEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleModeMenuEvent(Widget *widget, const WidgetEventData *eventData);
			
			static void OffsetTexcoords(const GeometryObject *object, unsigned_int32 index, const Vector2D& offset);
			static void RotateTexcoords(const GeometryObject *object, unsigned_int32 index, const Transform4D& rotation);
			static void ScaleTexcoords(const GeometryObject *object, unsigned_int32 index, const Vector2D& scale);
		
		public:
			
			TextureMappingPage();
			~TextureMappingPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
	};
	
	
	class InfoPage : public EditorPage
	{
		private:
			
			enum
			{
				kNodeInfoName,
				kNodeInfoController,
				kNodeInfoConnectors,
				kNodeInfoProperties,
				kNodeInfoInstances,
				kNodeInfoCount
			};
			
			enum
			{
				kGeometryInfoVertices,
				kGeometryInfoFaces,
				kGeometryInfoSurfaces,
				kGeometryInfoMaterials,
				kGeometryInfoLevels,
				kGeometryInfoCount
			};
			
			ImageWidget					*nodeIconWidget;
			
			Widget						*geometryGroup;
			Widget						*worldGroup;
			Widget						*typeGroup;
			
			TextWidget					*nodeWidget[kNodeInfoCount];
			TextWidget					*geometryWidget[kGeometryInfoCount];
			TextWidget					*worldWidget;
			TextWidget					*typeWidget;
			
			EditorObserver<InfoPage>	editorObserver;
			
			void HandleEditorEvent(Editor *editor, const EditorEvent& event);
			
			void ClearInfo(void);
			void UpdateInfo(const Node *node);
		
		public:
			
			InfoPage();
			~InfoPage();
			
			void Preprocess(void);
	};
	
	
	class GridPage : public EditorPage
	{
		private:
			
			enum
			{
				kEditorGridButtonShow,
				kEditorGridButtonSnap,
				kEditorGridButtonAxes,
				kEditorGridButtonHalve,
				kEditorGridButtonDouble,
				kEditorGridButtonCount
			};
			
			IconButtonWidget			*gridButton[kEditorGridButtonCount];
			EditTextWidget				*gridSpacingWidget;
			EditTextWidget				*majorLineWidget;
			ColorWidget					*gridColorWidget;
			
			WidgetObserver<GridPage>	gridButtonObserver;
			WidgetObserver<GridPage>	gridTextObserver;
			WidgetObserver<GridPage>	gridColorObserver;
			
			void HandleGridButtonEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleGridTextEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleGridColorEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			GridPage();
			~GridPage();
			
			void Preprocess(void);
	};
	
	
	class FindPage : public EditorPage
	{
		private:
			
			enum
			{
				kNodeTypeCount = 23
			};
			
			EditTextWidget				*nameWidget;
			PopupMenuWidget				*typeWidget;
			PopupMenuWidget				*controllerWidget;
			
			WidgetObserver<FindPage>	findAllButtonObserver;
			WidgetObserver<FindPage>	findNextButtonObserver;
			WidgetObserver<FindPage>	findPreviousButtonObserver;
			
			ControllerType				*controllerTypeTable;
			static NodeType				nodeTypeTable[kNodeTypeCount];
			
			bool MatchingNode(const Node *node) const;
			
			void HandleFindAllButtonEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleFindNextButtonEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleFindPreviousButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			FindPage();
			~FindPage();
			
			void Preprocess(void);
	};
	
	
	class PaintPage : public EditorPage, public EditorTool
	{
		private:
			
			enum
			{
				kEditorPaintToolBrush,
				kEditorPaintToolCount
			};
			
			enum
			{
				kPaintMenuAssociatePaintSpace,
				kPaintMenuDissociatePaintSpace,
				kPaintMenuSelectAssociatedPaintSpaces,
				kPaintMenuSelectAssociatedGeometries,
				kPaintMenuItemCount
			};
			
			int32							currentTool;
			PaintSpace						*targetPaintSpace;
			Painter							*painter;
			
			PaintState						paintState;
			Point2D							previousPosition;
			
			bool							channelMask[4];
			ColorRGBA						brushColor;
			
			int32							invertValue;
			int32							stylusValue;
			int32							strengthValue;
			int32							radiusValue;
			int32							fuzzyValue;
			
			IconButtonWidget				*toolButton[kEditorPaintToolCount];
			IconButtonWidget				*channelButton[4];
			ColorWidget						*colorWidget;
			CheckWidget						*invertWidget;
			CheckWidget						*stylusWidget;
			ImageWidget						*imageWidget;
			
			SliderWidget					*strengthSlider;
			TextWidget						*strengthText;
			
			SliderWidget					*radiusSlider;
			TextWidget						*radiusText;
			
			SliderWidget					*fuzzySlider;
			TextWidget						*fuzzyText;
			
			IconButtonWidget				*menuButton;
			MenuItemWidget					*paintMenuItem[kPaintMenuItemCount];
			List<MenuItemWidget>			paintMenuItemList;
			
			EditorObserver<PaintPage>		editorObserver;
			WidgetObserver<PaintPage>		toolButtonObserver;
			WidgetObserver<PaintPage>		channelButtonObserver;
			WidgetObserver<PaintPage>		colorObserver;
			WidgetObserver<PaintPage>		checkObserver;
			WidgetObserver<PaintPage>		sliderObserver;
			WidgetObserver<PaintPage>		menuButtonObserver;
			
			float GetBrushStrength(void) const
			{
				return ((float) strengthValue * 0.01F);
			}
			
			float GetBrushRadius(void) const
			{
				return ((float) (radiusValue + 1));
			}
			
			float GetBrushFuzziness(void) const
			{
				return ((float) fuzzyValue * 0.01F);
			}
			
			void UpdateSlider(SliderWidget *widget);
			void UpdateImage(void);
			
			void HandleEditorEvent(Editor *editor, const EditorEvent& event);
			void HandleToolButtonEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleChannelButtonEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleColorEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleCheckEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleSliderEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleMenuButtonEvent(Widget *widget, const WidgetEventData *eventData);
			
			void HandleAssociatePaintSpaceMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData);
			void HandleDissociatePaintSpaceMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData);
			void HandleSelectAssociatedPaintSpacesMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData);
			void HandleSelectAssociatedGeometriesMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData);
			
			static bool PaintPickFilter(const Node *node, const PickData *pickData, void *cookie);
		
		public:
			
			PaintPage();
			~PaintPage();
			
			PaintSpace *GetTargetPaintSpace(void) const
			{
				return (targetPaintSpace);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
			
			void SetTargetPaintSpace(PaintSpace *paintSpace);
	};
}


#endif

// ZYURVUR
