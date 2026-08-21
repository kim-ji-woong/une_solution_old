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


#ifndef C4WaterTools_h
#define C4WaterTools_h


#include "C4GeometryManipulators.h"
#include "C4EditorPages.h"
#include "C4Water.h"


namespace C4
{
	enum
	{
		kEditorPageWater		= 'WATR'
	};
	
	
	class WaterResource : public Resource<WaterResource>
	{
		friend class Resource<WaterResource>;
		
		private:
			
			static ResourceDescriptor	descriptor;
			
			~WaterResource();
		
		public:
			
			WaterResource(const char *name, ResourceCatalog *catalog);
			
			void Preprocess(void);
	};
	
	
	class WaterBlockManipulator : public EditorManipulator
	{
		private:
			
			Vector4D				blockSizeVector;
			List<Attribute>			blockAttributeList;
			DiffuseAttribute		blockDiffuseColor;
			Renderable				blockRenderable;
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			WaterBlockManipulator(WaterBlock *block);
			~WaterBlockManipulator();
			
			WaterBlock *GetTargetNode(void) const
			{
				return (static_cast<WaterBlock *>(EditorManipulator::GetTargetNode()));
			}
			
			WaterBlockObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			const char *GetDefaultNodeName(void) const;
			
			void Select(void);
			void Unselect(void);
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool RegionPick(const Region *region) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			
			void Render(const ManipulatorRenderData *renderData);
	};
	
	
	class WaterGeometryManipulator : public GeometryManipulator
	{
		public:
			
			WaterGeometryManipulator(WaterGeometry *water);
			~WaterGeometryManipulator();
			
			WaterGeometry *GetTargetNode(void) const
			{
				return (static_cast<WaterGeometry *>(EditorManipulator::GetTargetNode()));
			}
			
			WaterGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			const char *GetDefaultNodeName(void) const;
	};
	
	
	class HorizonWaterGeometryManipulator : public GeometryManipulator 
	{
		public: 
			 
			HorizonWaterGeometryManipulator(HorizonWaterGeometry *water); 
			~HorizonWaterGeometryManipulator();
			 
			HorizonWaterGeometry *GetTargetNode(void) const
			{
				return (static_cast<HorizonWaterGeometry *>(EditorManipulator::GetTargetNode()));
			} 
			
			HorizonWaterGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject()); 
			}
			
			const char *GetDefaultNodeName(void) const;
	};
	
	
	class WaterPage : public EditorPage, public EditorTool
	{
		private:
			
			enum
			{
				kWaterMenuGenerateLandHeight,
				kWaterMenuRemoveLandHeight,
				kWaterMenuRebuildWaterBlock,
				kWaterMenuImportWaveData,
				kWaterMenuRemoveWaveData,
				kWaterMenuItemCount
			};
			
			int32						currentTool;
			
			IconButtonWidget			*waterButton;
			
			IconButtonWidget			*menuButton;
			MenuItemWidget				*waterMenuItem[kWaterMenuItemCount];
			List<MenuItemWidget>		waterMenuItemList;
			
			WidgetObserver<WaterPage>	waterButtonObserver;
			WidgetObserver<WaterPage>	menuButtonObserver;
			
			void HandleWaterButtonEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleMenuButtonEvent(Widget *widget, const WidgetEventData *eventData);
			
			void HandleGenerateLandHeightMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData);
			void HandleRemoveLandHeightMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData);
			void HandleRebuildWaterBlockMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData);
			void HandleImportWaveDataMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData);
			void HandleRemoveWaveDataMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData);
			
			static void WaterPicked(FilePicker *picker, void *cookie);
		
		public:
			
			WaterPage();
			~WaterPage();
			
			void Preprocess(void);
			
			void Engage(Editor *editor, void *cookie);
			void Disengage(Editor *editor, void *cookie);
			
			bool BeginTool(Editor *editor, EditorTrackData *trackData);
			bool TrackTool(Editor *editor, EditorTrackData *trackData);
			bool EndTool(Editor *editor, EditorTrackData *trackData);
			
			static void ExportWater(const World *world, const ResourceName& resourceName);
	};
	
	
	class WaterRebuildUndoData : public UndoData
	{
		private:
			
			WaterBlock		*blockNode;
			Integer2D		blockSize;
			float			waterFieldScale;
			float			waterHorizonDistance;
		
		public:
			
			WaterRebuildUndoData(WaterBlock *block);
			~WaterRebuildUndoData();
			
			void Undo(Editor *editor);
	};
	
	
	class WaterBuilder : public Configurable
	{
		private:
			
			Integer2D		waterSize;
			
			float			waterHorizonDistance;
			bool			horizonFlag[4];
		
		public:
			
			WaterBuilder(const WaterBlock *block);
			~WaterBuilder();
			
			const bool *GetHorizonFlagArray(void) const
			{
				return (horizonFlag);
			}
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void BuildWater(Job *job, WaterBlock *block);
	};
	
	
	class BuildWaterWindow : public Window
	{
		private:
			
			Editor					*worldEditor;
			WaterBlock				*blockNode;
			
			WaterBuilder			*waterBuilder;
			
			PushButtonWidget		*buildButton;
			PushButtonWidget		*cancelButton;
			
			ConfigurationWidget		*configurationWidget;
		
		public:
			
			BuildWaterWindow(Editor *editor, WaterBlock *block);
			~BuildWaterWindow();
			
			void Preprocess(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
	
	
	class WaterProgressWindow : public Window
	{
		private:
			
			Editor					*worldEditor;
			WaterBlock				*blockNode;
			WaterBuilder			*waterBuilder;
			
			Job						waterJob;
			volatile bool			buildSuccess;
			List<Geometry>			geometryList;
			
			PushButtonWidget		*stopButton;
			ProgressWidget			*progressBar;
			
			static void WaterJob(Job *job, void *cookie);
		
		public:
			
			WaterProgressWindow(Editor *editor, WaterBlock *block, WaterBuilder *builder);
			~WaterProgressWindow();
			
			void Preprocess(void);
			void Move(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
	
	
	class GenerateLandElevationWindow : public Window
	{
		private:
			
			class LandElevationJob : public Job
			{
				private:
					
					GenerateLandElevationWindow		*jobWindow;
				
				public:
					
					LandElevationJob(GenerateLandElevationWindow *window, ExecuteProc *execProc, void *cookie);
					
					GenerateLandElevationWindow *GetJobWindow(void) const
					{
						return (jobWindow);
					}
			};
			
			Editor					*worldEditor;
			
			int32					jobCount;
			Job						**jobTable;
			
			Lock					jobLock;
			
			PushButtonWidget		*stopButton;
			ProgressWidget			*progressBar;
			
			void StartJob(void);
			
			static bool DetectCollision(const Node *root, Ray *ray, PickData *pickData);
			static void GenerateLandElevationJob(Job *job, void *cookie);
		
		public:
			
			GenerateLandElevationWindow(Editor *editor);
			~GenerateLandElevationWindow();
			
			static bool ValidGeometry(const Geometry *geometry);
			
			void Preprocess(void);
			void Move(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
}


#endif

// ZYURVUR
