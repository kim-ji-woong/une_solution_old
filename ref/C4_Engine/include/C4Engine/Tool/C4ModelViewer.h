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


#ifndef C4ModelViewer_h
#define C4ModelViewer_h


#include "C4Viewports.h"
#include "C4Animation.h"
#include "C4EditorPlugins.h"


namespace C4
{
	enum
	{
		kModelToolOrbit,
		kModelToolLight,
		kModelToolHand,
		kModelToolGlass,
		kModelToolFree,
		kModelToolCount
	};
	
	
	enum
	{
		kModelDiagnosticDarkness,
		kModelDiagnosticWireframe,
		kModelDiagnosticNormals,
		kModelDiagnosticTangents,
		kModelDiagnosticSkeleton,
		kModelDiagnosticShadows,
		kModelDiagnosticMotionBlur,
		kModelDiagnosticCount
	};
	
	
	enum
	{
		kCueMenuInsertCue,
		kCueMenuDeleteCue,
		kCueMenuDeleteAllCues,
		kCueMenuGetCueInfo,
		kCueMenuItemCount
	};
	
	
	enum
	{
		kModelViewerModified		= 1 << 0,
		kModelViewerUpdateMenus		= 1 << 1
	};
	
	
	enum
	{
		kWidgetLimit				= 'LIMT',
		kWidgetCue					= 'CUE '
	};
	
	
	class Light;
	class World;
	class Model;
	class ModelWindow;
	
	
	class LimitWidget : public RenderableWidget
	{
		private:
			
			int32			limitValue;
			int32			maxLimitValue;
			
			int32			minLimitPosition;
			int32			maxLimitPosition;
			
			float			dragPosition;
			
			Point3D			limitVertex[8];
			ColorRGBA		limitColor[8];
			Point2D			limitTexcoord[8];
			
			int32 GetPositionValue(float x) const;
		
		public:
			
			LimitWidget(const Vector2D& size);
			~LimitWidget();
			
			int32 GetValue(void) const
			{
				return (limitValue);
			}
			
			int32 GetMaxValue(void) const
			{
				return (maxLimitValue);
			}
			
			int32 GetMinLimitPosition(void) const
			{
				return (minLimitPosition);
			} 
			
			int32 GetMaxLimitPosition(void) const 
			{ 
				return (maxLimitPosition); 
			}
			 
			void SetLimitRange(int32 min, int32 max)
			{
				minLimitPosition = min;
				maxLimitPosition = max; 
			}
			
			void SetValue(int32 value);
			void SetMaxValue(int32 maxValue); 
			
			WidgetPart TestPosition(const Point3D& position) const;
			float GetIndicatorPosition(void) const;
			
			void Preprocess(void);
			void Build(void);
			
			void HandleMouseEvent(const PanelMouseEventData *eventData);
	};
	
	
	class CueWidget : public TextWidget, public ListElement<CueWidget>
	{
		private:
			
			CueType			cueType;
			
			int32			cueValue;
			int32			maxCueValue;
			
			float			dragPosition;
			
			Renderable		cueRenderable;
			
			Point3D			cueVertex[8];
			ColorRGBA		cueColor[8];
			Point2D			cueTexcoord[8];
			
			int32 GetPositionValue(float x) const;
		
		public:
			
			CueWidget();
			CueWidget(CueType type, int32 value, int32 maxValue);
			~CueWidget();
			
			using ListElement<CueWidget>::Previous;
			using ListElement<CueWidget>::Next;
			
			CueType GetCueType(void) const
			{
				return (cueType);
			}
			
			int32 GetValue(void) const
			{
				return (cueValue);
			}
			
			int32 GetMaxValue(void) const
			{
				return (maxCueValue);
			}
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void SetValue(int32 value);
			void SetMaxValue(int32 maxValue);
			
			WidgetPart TestPosition(const Point3D& position) const;
			float GetIndicatorPosition(void) const;
			
			void Preprocess(void);
			void Build(void);
			void Render(List<Renderable> *renderList);
			
			void HandleMouseEvent(const PanelMouseEventData *eventData);
	};
	
	
	class CueInfoWindow : public Window
	{
		private:
			
			ModelWindow				*modelWindow;
			CueWidget				*cueWidget;
			
			PushButtonWidget		*okayButton;
			PushButtonWidget		*cancelButton;
			
			ConfigurationWidget		*configurationWidget;
		
		public:
			
			CueInfoWindow(ModelWindow *window, CueWidget *cue = nullptr);
			~CueInfoWindow();
			
			void Preprocess(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
	
	
	class AnimationPicker : public SceneImportPicker
	{
		private:
			
			CheckWidget			*anchorXYBox;
			CheckWidget			*anchorZBox;
			CheckWidget			*freezeRootBox;
			CheckWidget			*preserveMissingBox;
			PopupMenuWidget		*rotationMenu;
		
		public:
			
			AnimationPicker(const char *subdir);
			~AnimationPicker();
			
			bool GetAnchorXYFlag(void) const
			{
				return (anchorXYBox->GetValue() != 0);
			}
			
			bool GetAnchorZFlag(void) const
			{
				return (anchorZBox->GetValue() != 0);
			}
			
			bool GetFreezeRootFlag(void) const
			{
				return (freezeRootBox->GetValue() != 0);
			}
			
			bool GetPreserveMissingFlag(void) const
			{
				return (preserveMissingBox->GetValue() != 0);
			}
			
			int32 GetRotationIndex(void) const
			{
				return (rotationMenu->GetSelection());
			}
			
			void Preprocess(void);
	};
	
	
	class ModelPage : public Page
	{
		private:
			
			ModelWindow		*modelWindow;
		
		protected:
			
			ModelPage(ModelWindow *window, const char *panelName);
		
		public:
			
			~ModelPage();
			
			ModelWindow *GetModelWindow(void) const
			{
				return (modelWindow);
			}
	};
	
	
	class ModelInfoPage : public ModelPage
	{
		public:
			
			ModelInfoPage(ModelWindow *window);
			~ModelInfoPage();
			
			void Preprocess(void);
	};
	
	
	class ModelAnimationPage : public ModelPage
	{
		private:
			
			IconButtonWidget		*playButton;
			IconButtonWidget		*stopButton;
			
			CheckWidget				*loopBox;
			CheckWidget				*oscillateBox;
			CheckWidget				*reverseBox;
			
			ListWidget				*animationList;
			PushButtonWidget		*importButton;
			
			WidgetObserver<ModelAnimationPage>		playButtonObserver;
			WidgetObserver<ModelAnimationPage>		stopButtonObserver;
			
			WidgetObserver<ModelAnimationPage>		loopBoxObserver;
			WidgetObserver<ModelAnimationPage>		oscillateBoxObserver;
			WidgetObserver<ModelAnimationPage>		reverseBoxObserver;
			
			WidgetObserver<ModelAnimationPage>		animationListObserver;
			WidgetObserver<ModelAnimationPage>		importButtonObserver;
			
			ResourcePath GetModelDirectory(void) const;
			void BuildAnimationList(void);
			
			static void AnimationPickerProc(FilePicker *picker, void *cookie);
			static void AnimationCompleteProc(Interpolator *interpolator, void *cookie);
			
			void HandlePlayButtonEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleStopButtonEvent(Widget *widget, const WidgetEventData *eventData);
			
			void HandleLoopBoxEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleOscillateBoxEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleReverseBoxEvent(Widget *widget, const WidgetEventData *eventData);
			
			void HandleAnimationListEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleImportButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			ModelAnimationPage(ModelWindow *window);
			~ModelAnimationPage();
			
			void StopAnimation(void)
			{
				stopButton->Activate();
			}
			
			void Preprocess(void);
	};
	
	
	class ModelDisplayPage : public ModelPage
	{
		private:
			
			TextWidget				*biasText;
			CheckWidget				*diagnosticBox[kModelDiagnosticCount];
			
			WidgetObserver<ModelDisplayPage>	biasSliderObserver;
			
			void HandleBiasSliderEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			ModelDisplayPage(ModelWindow *window);
			~ModelDisplayPage();
			
			bool GetDiagnosticFlag(int32 index) const
			{
				return (diagnosticBox[index]->GetValue() != 0);
			}
			
			void Preprocess(void);
	};
	
	
	class ModelWindow : public Window, public ListElement<ModelWindow>
	{
		private:
			
			ResourceName					resourceName;
			ResourceName					animationName;
			ResourceLocation				resourceLocation;
			
			unsigned_int32					viewerState;
			
			Vector2D						viewportSize;
			FrustumViewportWidget			*modelViewport;
			BorderWidget					*viewportBorder;
			
			Widget							*frameLabel;
			Widget							*cuesLabel;
			Widget							*cuesLine;
			Widget							*cuesGroup;
			
			SliderWidget					*frameSlider;
			LimitWidget						*beginLimit;
			LimitWidget						*endLimit;
			TextWidget						*frameText;
			TextWidget						*beginText;
			TextWidget						*endText;
			
			IconButtonWidget				*toolButton[kModelToolCount];
			
			MenuBarWidget					*menuBar;
			PulldownMenuWidget				*modelMenu;
			PulldownMenuWidget				*cueMenu;
			MenuItemWidget					*saveAnimationItem;
			MenuItemWidget					*cueMenuItem[kCueMenuItemCount];
			
			BookWidget						*bookWidget;
			ModelInfoPage					*infoPage;
			ModelAnimationPage				*animationPage;
			ModelDisplayPage				*displayPage;
			
			List<CueWidget>					cueWidgetList;
			WidgetObserver<ModelWindow>		cueWidgetObserver;
			
			World							*environmentWorld;
			Zone							*zoneNode;
			Light							*lightNode;
			Model							*modelNode;
			FrameAnimator					*frameAnimator;
			
			int32							currentTool;
			int32							trackTool;
			bool							toolTracking;
			Point3D							previousPosition;
			
			float							lightAzimuth;
			float							lightAltitude;
			
			float							freeCameraSpeed;
			unsigned_int32					freeCameraFlags;
			
			static List<ModelWindow>		windowList;
			
			void PositionWidgets(void);
			void PositionCueWidget(CueWidget *cueWidget);
			void UpdateLimitText(void);
			
			void BuildMenus(void);
			
			void HandleCloseMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleSaveAnimationMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleSaveModelMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleInsertCueMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleDeleteCueMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleDeleteAllCuesMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleGetCueInfoMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			
			static void EnvironmentPickerProc(FilePicker *picker, void *cookie);
			void HandleSelectEnvironmentMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void SetEnvironmentWorld(const char *name);
			
			void HandleCueWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			
			int32 GetActiveTool(void) const;
			void TrackGlassTool(ViewportWidget *viewport, float dy);
			
			static void ViewportHandleMouseEvent(const MouseEventData *eventData, ViewportWidget *viewport, void *cookie);
			static void ViewportTrackTask(const Point3D& position, ViewportWidget *viewport, void *cookie);
			static void ViewportRender(List<Renderable> *renderList, ViewportWidget *viewport, void *cookie);
		
		public:
			
			ModelWindow(const char *name, const ModelResource *resource, const ResourceLocation *location);
			~ModelWindow();
			
			static void PurgeWindowList(void)
			{
				windowList.Purge();
			}
			
			const char *GetResourceName(void) const
			{
				return (resourceName);
			}
			
			const ResourceLocation *GetResourceLocation(void) const
			{
				return (&resourceLocation);
			}
			
			void SetModifiedFlag(void)
			{
				viewerState |= kModelViewerModified;
			}
			
			void SetMenuUpdateFlag(void)
			{
				viewerState |= kModelViewerUpdateMenus;
			}
			
			Model *GetModel(void) const
			{
				return (modelNode);
			}
			
			FrameAnimator *GetFrameAnimator(void) const
			{
				return (frameAnimator);
			}
			
			bool GetDiagnosticFlag(int32 index) const
			{
				return (displayPage->GetDiagnosticFlag(index));
			}
			
			static ResourceResult Open(const char *name);
			
			void SetWidgetSize(const Vector2D& size);
			void Preprocess(void);
			void Move(void);
			
			void EnterBackground(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			
			void ActivateAnimation(const char *name);
			int32 GetAnimationFrame(void) const;
			void AddCue(CueWidget *cueWidget);
	};
}


#endif

// ZYURVUR
