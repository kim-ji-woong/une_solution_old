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


#ifndef C4ScriptEditor_h
#define C4ScriptEditor_h


#include "C4Viewports.h"
#include "C4Expressions.h"


namespace C4
{
	enum
	{
		kScriptEditorModeTool,
		kScriptEditorModeMethod,
		kScriptEditorModeSection,
		kScriptEditorModeFiber,
		kScriptEditorModeCount
	};
	
	
	enum
	{
		kScriptToolGraphSelect = -1,
		kScriptToolMethodMove,
		kScriptToolViewportScroll,
		kScriptToolViewportZoom,
		kScriptToolCount
	};
	
	
	enum
	{
		kScriptMenuUndo,
		kScriptMenuCut,
		kScriptMenuCopy,
		kScriptMenuPaste,
		kScriptMenuClear,
		kScriptMenuDuplicate,
		
		kScriptMenuGetInfo,
		kScriptMenuCycleFiberCondition,
		
		kScriptMenuItemCount
	};
	
	
	enum
	{
		kMethodColorNormal,
		kMethodColorInitial,
		kMethodColorDead,
		kMethodColorError,
		kMethodColorCount
	};
	
	
	enum
	{
		kMethodWidgetSelected			= 1 << 0,
		kMethodWidgetTempSelected		= 1 << 1
	};
	
	
	enum
	{
		kFiberWidgetSelected			= 1 << 0
	};
	
	
	enum
	{
		kScriptSectionWidgetSelected	= 1 << 0
	};
	
	
	enum
	{
		kScriptEditorModified			= 1 << 0,
		kScriptEditorUndoPending		= 1 << 1,
		kScriptEditorUpdateMenus		= 1 << 2,
		kScriptEditorUpdateGraph		= 1 << 3,
		kScriptEditorUpdateGrid			= 1 << 4
	};
	
	
	enum
	{
		kWidgetMethod				= 'meth',
		kWidgetFiber				= 'fibr',
		kWidgetScriptSection		= 'scsc'
	};
	
	
	class MethodWidget;
	class FiberWidget;
	class ScriptSectionWidget;
	class ScriptEditor;
	class Dialog;
	
	
	class MethodReference : public ListElement<MethodReference>
	{ 
		private:
			 
			MethodWidget	*reference; 
		 
		public:
			 
			MethodReference(MethodWidget *widget)
			{
				reference = widget;
			} 
			
			MethodWidget *GetMethodWidget(void) const
			{
				return (reference); 
			}
	};
	
	
	class FiberReference : public ListElement<FiberReference>
	{
		private:
			
			FiberWidget	*reference;
		
		public:
			
			FiberReference(FiberWidget *widget)
			{
				reference = widget;
			}
			
			FiberWidget *GetFiberWidget(void) const
			{
				return (reference);
			}
	};
	
	
	class ScriptSectionReference : public ListElement<ScriptSectionReference>
	{
		private:
			
			ScriptSectionWidget	*reference;
		
		public:
			
			ScriptSectionReference(ScriptSectionWidget *widget)
			{
				reference = widget;
			}
			
			ScriptSectionWidget *GetSectionElement(void) const
			{
				return (reference);
			}
	};
	
	
	class ScriptUndoData : public ListElement<ScriptUndoData>
	{
		protected:
			
			ScriptUndoData();
		
		public:
			
			virtual ~ScriptUndoData();
			
			virtual void Undo(ScriptEditor *scriptEditor) = 0;
	};
	
	
	class CreateScriptUndoData : public ScriptUndoData
	{
		private:
			
			List<MethodReference>			createdMethodList;
			List<FiberReference>			createdFiberList;
			List<ScriptSectionReference>	createdSectionList;
		
		public:
			
			CreateScriptUndoData(MethodWidget *method);
			CreateScriptUndoData(FiberWidget *fiber);
			CreateScriptUndoData(ScriptSectionWidget *section);
			CreateScriptUndoData(const List<MethodWidget> *methodList, const List<FiberWidget> *fiberList, const List<ScriptSectionWidget> *sectionList);
			~CreateScriptUndoData();
			
			void Undo(ScriptEditor *scriptEditor);
	};
	
	
	class DeleteScriptUndoData : public ScriptUndoData
	{
		private:
			
			List<MethodWidget>			deletedMethodList;
			List<FiberWidget>			deletedFiberList;
			List<ScriptSectionWidget>	deletedSectionList;
		
		public:
			
			DeleteScriptUndoData(List<MethodWidget> *methodList, List<FiberWidget> *fiberList, List<ScriptSectionWidget> *sectionList);
			~DeleteScriptUndoData();
			
			void Undo(ScriptEditor *scriptEditor);
	};
	
	
	class MoveScriptUndoData : public ScriptUndoData
	{
		private:
			
			class MovedMethodReference : public MethodReference
			{
				private:
					
					Point2D		position;
				
				public:
					
					MovedMethodReference(MethodWidget *widget);
					
					const Point2D& GetPosition(void) const
					{
						return (position);
					}
			};
			
			class MovedSectionReference : public ScriptSectionReference
			{
				private:
					
					Point2D		position;
				
				public:
					
					MovedSectionReference(ScriptSectionWidget *widget);
					
					const Point2D& GetPosition(void) const
					{
						return (position);
					}
			};
			
			List<MethodReference>			movedMethodList;
			List<ScriptSectionReference>	movedSectionList;
		
		public:
			
			MoveScriptUndoData(const List<MethodWidget> *methodList, const List<ScriptSectionWidget> *sectionList);
			~MoveScriptUndoData();
			
			void Undo(ScriptEditor *scriptEditor);
	};
	
	
	class ResizeScriptUndoData : public ScriptUndoData
	{
		private:
			
			ScriptSectionWidget		*sectionWidget;
			float						sectionWidth;
			float						sectionHeight;
		
		public:
			
			ResizeScriptUndoData(ScriptSectionWidget *widget);
			~ResizeScriptUndoData();
			
			void Undo(ScriptEditor *shaderEditor);
	};
	
	
	class FiberScriptUndoData : public ScriptUndoData
	{
		private:
			
			class CycledReference : public FiberReference
			{
				private:
					
					unsigned_int32		flags;
				
				public:
					
					CycledReference(FiberWidget *widget);
					
					unsigned_int32 GetFlags(void) const
					{
						return (flags);
					}
			};
			
			List<FiberReference>		fiberList;
		
		public:
			
			FiberScriptUndoData(const List<FiberWidget> *selectionList);
			~FiberScriptUndoData();
			
			void Undo(ScriptEditor *scriptEditor);
	};
	
	
	class MethodWidget : public TextWidget, public ListElement<MethodWidget>
	{
		private:
			
			ScriptEditor				*scriptEditor;
			Method						*scriptMethod;
			const MethodRegistration	*methodRegistration;
			
			unsigned_int32				methodWidgetState;
			float						viewportScale;
			
			Point3D						originalPosition;
			float						sortPosition;
			
			Renderable					methodRenderable;
			
			List<Attribute>				backgroundAttributeList;
			DiffuseAttribute			backgroundDiffuseAttribute;
			TextureMapAttribute			backgroundTextureMap;
			Renderable					backgroundRenderable;
			
			List<Attribute>				outputAttributeList;
			DiffuseAttribute			outputDiffuseAttribute;
			TextureMapAttribute			outputTextureMap;
			Renderable					outputRenderable;
			
			Point2D						methodVertex[16];
			ColorRGBA					methodColor[16];
			
			static const Triangle		methodTriangle[16];
			static const ConstPoint2D	backgroundVertex[4];
			static const ConstPoint2D	backgroundTexcoord[4];
			static const ConstPoint2D	outputVertex[4];
			static const ConstPoint2D	outputTexcoord[4];
			
			bool CalculateBoundingBox(Box2D *box) const override;
		
		public:
			
			MethodWidget(ScriptEditor *editor, Method *method, const MethodRegistration *registration);
			~MethodWidget();
			
			using ListElement<MethodWidget>::Previous;
			using ListElement<MethodWidget>::Next;
			
			Method *GetScriptMethod(void) const
			{
				return (scriptMethod);
			}
			
			const MethodRegistration *GetMethodRegistration(void) const
			{
				return (methodRegistration);
			}
			
			unsigned_int32 GetMethodWidgetState(void) const
			{
				return (methodWidgetState);
			}
			
			void SetMethodWidgetState(unsigned_int32 state)
			{
				methodWidgetState = state;
			}
			
			void SetViewportScale(float scale)
			{
				viewportScale = scale;
				SetBuildFlag();
			}
			
			const Point3D& GetOriginalPosition(void) const
			{
				return (originalPosition);
			}
			
			void SaveOriginalPosition(void)
			{
				originalPosition = GetWidgetPosition();
			}
			
			float GetSortPosition(void) const
			{
				return (sortPosition);
			}
			
			void SetSortPosition(float position)
			{
				sortPosition = position;
			}
			
			void UpdateOutputColor(bool hilite);
			void UpdateColor(int32 index);
			void UpdateText(void);
			
			void Select(unsigned_int32 state);
			void Unselect(void);
			
			WidgetPart TestPosition(const Point3D& position) const;
			
			void Build(void);
			void Render(List<Renderable> *renderList);
	};
	
	
	class FiberWidget : public RenderableWidget, public ListElement<FiberWidget>
	{
		private:
			
			ScriptEditor			*scriptEditor;
			Fiber					*scriptFiber;
			
			unsigned_int32			fiberWidgetState;
			
			Point2D					fiberVertex[70];
			Vector4D				fiberTangent[70];
			Point2D					fiberTexcoord[70];
			Vector4D				selectionTangent[70];
			Point2D					selectionTexcoord[70];
			
			List<Attribute>			fiberAttributeList;
			DiffuseAttribute		fiberDiffuseAttribute;
			TextureMapAttribute		fiberTextureMapAttribute;
			
			List<Attribute>			selectionAttributeList;
			DiffuseAttribute		selectionDiffuseAttribute;
			TextureMapAttribute		selectionTextureMapAttribute;
			Renderable				selectionRenderable;
			
			bool CalculateBoundingBox(Box2D *box) const override;
		
		public:
			
			FiberWidget(ScriptEditor *editor, Fiber *fiber);
			~FiberWidget();
			
			using ListElement<FiberWidget>::Previous;
			using ListElement<FiberWidget>::Next;
			
			Fiber *GetScriptFiber(void) const
			{
				return (scriptFiber);
			}
			
			void Rebuild(void)
			{
				SetBuildFlag();
			}
			
			unsigned_int32 GetFiberWidgetState(void) const
			{
				return (fiberWidgetState);
			}
			
			void SetFiberWidgetState(unsigned_int32 state)
			{
				fiberWidgetState = state;
			}
			
			void UpdateColor(void);
			
			void Select(void);
			void Unselect(void);
			
			WidgetPart TestPosition(const Point3D& position) const;
			
			void Build(void);
			void Render(List<Renderable> *renderList);
	};
	
	
	class ScriptSectionWidget : public TextWidget, public ListElement<ScriptSectionWidget>
	{
		private:
			
			ScriptEditor			*scriptEditor;
			SectionMethod			*sectionMethod;
			
			unsigned_int32			sectionWidgetState;
			float					viewportScale;
			
			Point3D					originalPosition;
			
			Renderable				sectionRenderable;
			
			Point2D					sectionVertex[27];
			ColorRGBA				sectionColor[27];
			static const Triangle	sectionTriangle[21];
		
		public:
			
			ScriptSectionWidget(ScriptEditor *editor, SectionMethod *method);
			~ScriptSectionWidget();
			
			using ListElement<ScriptSectionWidget>::Previous;
			using ListElement<ScriptSectionWidget>::Next;
			
			SectionMethod *GetSectionMethod(void) const
			{
				return (sectionMethod);
			}
			
			unsigned_int32 GetSectionElementState(void) const
			{
				return (sectionWidgetState);
			}
			
			void SetSectionElementState(unsigned_int32 state)
			{
				sectionWidgetState = state;
			}
			
			void SetViewportScale(float scale)
			{
				viewportScale = scale;
				SetBuildFlag();
			}
			
			const Point3D& GetOriginalPosition(void) const
			{
				return (originalPosition);
			}
			
			void SaveOriginalPosition(void)
			{
				originalPosition = GetWidgetPosition();
			}
			
			void UpdateContent(void);
			void UpdateColor(void);
			
			void Select(void);
			void Unselect(void);
			
			WidgetPart TestPosition(const Point3D& position) const;
			
			void Build(void);
			void Render(List<Renderable> *renderList);
	};
	
	
	class ScriptPage : public Page
	{
		private:
			
			ScriptEditor	*scriptEditor;
		
		protected:
			
			ScriptPage(ScriptEditor *editor, const char *panelName);
		
		public:
			
			~ScriptPage();
			
			ScriptEditor *GetScriptEditor(void) const
			{
				return (scriptEditor);
			}
	};
	
	
	class ScriptMethodsPage : public ScriptPage
	{
		private:
			
			enum
			{
				kMethodPaneCount = 3
			};
			
			class ToolWidget : public TextWidget
			{
				public:
					
					const MethodRegistration	*methodRegistration;
					
					ToolWidget(const Vector2D& size, const MethodRegistration *registration);
					~ToolWidget();
					
					static String<127> GetMethodName(const MethodRegistration *registration);
			};
			
			MultipaneWidget							*multipaneWidget;
			WidgetObserver<ScriptMethodsPage>		multipaneWidgetObserver;
			
			ListWidget								*listWidget[kMethodPaneCount];
			WidgetObserver<ScriptMethodsPage>		listWidgetObserver;
			
			void HandleMultipaneWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleListWidgetEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			ScriptMethodsPage(ScriptEditor *editor);
			~ScriptMethodsPage();
			
			void Preprocess(void);
			
			void UnselectMethodTool(void);
	};
	
	
	class ScriptVariablesPage : public ScriptPage
	{
		private:
			
			class VariableWidget : public TextWidget
			{
				public:
					
					Value	*variableValue;
					
					VariableWidget(const Vector2D& size, Value *value);
					~VariableWidget();
			};
			
			ListWidget								*listWidget;
			WidgetObserver<ScriptVariablesPage>		listWidgetObserver;
			
			PushButtonWidget						*addButton;
			PushButtonWidget						*deleteButton;
			WidgetObserver<ScriptVariablesPage>		addButtonObserver;
			WidgetObserver<ScriptVariablesPage>		deleteButtonObserver;
			
			void HandleListWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleAddButtonEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleDeleteButtonEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			ScriptVariablesPage(ScriptEditor *editor);
			~ScriptVariablesPage();
			
			void Preprocess(void);
			
			void BuildVariableList(void);
	};
	
	
	class MethodInfoWindow : public Window
	{
		private:
			
			struct SettingData
			{
				Type			categoryType;
				List<Setting>	settingList;
			};
			
			class TargetWidget : public TextWidget
			{
				public:
					
					ConnectorKey	connectorKey;
					
					TargetWidget(const Vector2D& size, const char *text, const char *font, const char *key);
					~TargetWidget();
			};
			
			ScriptEditor					*scriptEditor;
			MethodWidget					*methodWidget;
			const Node						*controllerTarget;
			
			const ControllerRegistration	*controllerRegistration;
			
			int32							functionCount;
			Function						*currentFunction;
			Function						**functionTable;
			
			int32							categoryCount;
			SettingData						*currentSettingData;
			SettingData						**settingDataTable;
			const Object					*settingObject;
			
			PushButtonWidget				*okayButton;
			PushButtonWidget				*cancelButton;
			
			ConfigurationWidget				*configurationWidget;
			
			ListWidget						*targetList;
			ListWidget						*auxiliaryList;
			PushButtonWidget				*clearButton;
			
			EditTextWidget					*expressionBox;
			
			EditTextWidget					*outputBox;
			TextWidget						*outputText;
			
			const Node *GetTargetNode(void) const;
			
			void BuildTargetList(const Method *method);
			
			void UpdateFunctionList(FunctionMethod *method);
			void SelectFunction(FunctionMethod *method, int32 index, bool commit = true, bool final = false);
			
			void UpdateCategoryList(SettingMethod *method);
			void SelectCategory(SettingMethod *method, int32 index, bool commit = true, bool final = false);
		
		public:
			
			MethodInfoWindow(ScriptEditor *editor);
			~MethodInfoWindow();
			
			void Preprocess(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
	
	
	class ScriptSectionInfoWindow : public Window
	{
		private:
			
			ScriptEditor			*scriptEditor;
			ScriptSectionWidget		*sectionWidget;
			
			PushButtonWidget		*okayButton;
			PushButtonWidget		*cancelButton;
			
			ConfigurationWidget		*configurationWidget;
		
		public:
			
			ScriptSectionInfoWindow(ScriptEditor *editor);
			~ScriptSectionInfoWindow();
			
			void Preprocess(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
	
	
	class VariableInfoWindow : public Window
	{
		private:
			
			ScriptEditor			*scriptEditor;
			Value					*originalValue;
			Value					*currentValue;
			
			PushButtonWidget		*okayButton;
			PushButtonWidget		*cancelButton;
			
			ConfigurationWidget							*configurationWidget;
			ConfigurationObserver<VariableInfoWindow>	configurationObserver;
			
			void HandleConfigurationEvent(SettingInterface *settingInterface);
		
		public:
			
			VariableInfoWindow(ScriptEditor *editor, Value *value = nullptr);
			~VariableInfoWindow();
			
			void Preprocess(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
	
	
	class ScriptEditor : public Window, public Completable<ScriptEditor>
	{
		private:
			
			const Node						*targetNode;
			ScriptObject					*scriptObject;
			ScriptGraph						scriptGraph;
			Map<Value>						scriptValueMap;
			
			unsigned_int32					editorState;
			
			int32							currentMode;
			int32							currentTool;
			const MethodRegistration		*currentMethodReg;
			
			List<ScriptUndoData>			undoList;
			
			int32							trackingMode;
			int32							trackingTool;
			bool							toolTracking;
			bool							boxSelectFlag;
			Point3D							previousPoint;
			Point3D							previousPosition;
			Point3D							anchorPoint;
			Point3D							anchorPosition;
			
			MethodWidget					*fiberStartMethod;
			MethodWidget					*fiberFinishMethod;
			
			ScriptSectionWidget				*sectionTrackWidget;
			
			Widget							*graphRoot;
			Widget							*sectionRoot;
			
			List<MethodWidget>				methodWidgetList;
			List<FiberWidget>				fiberWidgetList;
			List<ScriptSectionWidget>		sectionWidgetList;
			
			List<MethodWidget>				selectedMethodList;
			List<FiberWidget>				selectedFiberList;
			List<ScriptSectionWidget>		selectedSectionList;
			
			PushButtonWidget				*okayButton;
			PushButtonWidget				*cancelButton;
			
			OrthoViewportWidget				*scriptViewport;
			BorderWidget					*viewportBorder;
			
			IconButtonWidget				*toolButton[kScriptToolCount];
			IconButtonWidget				*sectionButton;
			
			MenuBarWidget					*menuBar;
			PulldownMenuWidget				*editMenu;
			PulldownMenuWidget				*scriptMenu;
			MenuItemWidget					*scriptMenuItem[kScriptMenuItemCount];
			
			BookWidget						*bookWidget;
			ScriptMethodsPage				*methodsPage;
			ScriptVariablesPage				*variablesPage;
			
			WidgetObserver<ScriptEditor>	toolButtonObserver;
			
			Grid							viewportGrid;
			DragRect						dragRect;
			
			List<Attribute>					fiberAttributeList;
			DiffuseAttribute				fiberDiffuseColor;
			TextureMapAttribute				fiberTextureMap;
			Renderable						fiberRenderable;
			Point2D							fiberVertex[4];
			Vector4D						fiberTangent[4];
			Point2D							fiberTexcoord[4];
			
			static ScriptGraph				editorClipboard;
			
			bool MethodSelected(const MethodWidget *widget) const
			{
				return (selectedMethodList.Member(widget));
			}
			
			bool FiberSelected(const FiberWidget *widget) const
			{
				return (selectedFiberList.Member(widget));
			}
			
			bool SectionSelected(const ScriptSectionWidget *widget) const
			{
				return (selectedSectionList.Member(widget));
			}
			
			void PositionWidgets(void);
			void BuildMenus(void);
			
			void BuildScriptGraph(void);
			void UpdateScriptGraph(void);
			
			static void TraverseScriptGraph(Method *method, int32 depth);
			static bool DetectMethodError(const Method *method);
			
			void UnselectCurrentTool(void);
			void UpdateViewportScale(float scale);
			
			void SelectMethod(MethodWidget *widget, unsigned_int32 state = 0);
			void UnselectMethod(MethodWidget *widget);
			void SelectFiber(FiberWidget *widget);
			void UnselectFiber(FiberWidget *widget);
			void SelectSection(ScriptSectionWidget *widget);
			void UnselectSection(ScriptSectionWidget *widget);
			void SelectAll(void);
			void UnselectAll(void);
			void UnselectAllTemp(void);
			
			void AddUndoData(ScriptUndoData *data);
			void RemoveUndoData(ScriptUndoData *data);
			
			void HandleUndoMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleCutMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleCopyMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandlePasteMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleClearMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleSelectAllMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleDuplicateMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			
			void HandleGetInfoMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleCycleFiberConditionMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			
			Point3D ViewportToGraphPosition(const Point3D& p) const;
			static Point3D AlignPositionToGrid(const Point3D& p);
			void ShowGraphPosition(float x, float y);
			void AutoScroll(const Point3D& p);
			
			MethodWidget *FindMethodWidget(const Point3D& position) const;
			
			void SortSelectedMethodSublist(List<MethodWidget> *list, float dmin, float dmax);
			void SortSelectedMethodList(float dx, float dy);
			
			static bool BoxIntersectsMethodWidget(const Point3D& p1, const Point3D& p2, const MethodWidget *widget);
			bool MethodBoxIntersectsAnyMethodWidget(float x, float y, const MethodWidget *exclude = nullptr);
			
			void BeginTool(const Point3D& p, unsigned_int32 eventFlags);
			void TrackTool(const Point3D& p);
			void EndTool(const Point3D& p);
			
			void BeginSection(const Point3D& p);
			void TrackSection(const Point3D& p);
			void EndSection(const Point3D& p);
			
			void BeginFiber(const Point3D& p);
			void TrackFiber(const Point3D& p);
			void EndFiber(const Point3D& p);
			
			void CreateMethod(const Point3D& p);
			
			static void ViewportHandleMouseEvent(const MouseEventData *eventData, ViewportWidget *viewport, void *cookie);
			static void ViewportTrackTask(const Point3D& position, ViewportWidget *viewport, void *cookie);
			static void ViewportRender(List<Renderable> *renderList, ViewportWidget *viewport, void *cookie);
			
			void HandleToolButtonEvent(Widget *widget, const WidgetEventData *eventData);
			
			static void ConfirmationDialogComplete(Dialog *dialog, void *cookie);
		
		public:
			
			ScriptEditor(const Node *target, ScriptObject *object);
			~ScriptEditor();
			
			const Node *GetTargetNode(void) const
			{
				return (targetNode);
			}
			
			ScriptObject *GetScriptObject(void) const
			{
				return (scriptObject);
			}
			
			ScriptGraph *GetScriptGraph(void)
			{
				return (&scriptGraph);
			}
			
			MethodWidget *GetFirstSelectedMethod(void) const
			{
				return (selectedMethodList.First());
			}
			
			ScriptSectionWidget *GetFirstSelectedSection(void) const
			{
				return (selectedSectionList.First());
			}
			
			void AddEditorState(unsigned_int32 state)
			{
				editorState |= state;
			}
			
			Map<Value> *GetValueMap(void)
			{
				return (&scriptValueMap);
			}
			
			void AddValue(Value *value)
			{
				scriptValueMap.Insert(value);
				variablesPage->BuildVariableList();
				editorState |= kScriptEditorModified;
			}
			
			Value *FindValue(const char *name) const
			{
				return (scriptValueMap.Find(name));
			}
			
			static void ReleaseClipboard(void)
			{
				editorClipboard.Purge();
			}
			
			void SetWidgetSize(const Vector2D& size);
			void Preprocess(void);
			void Move(void);
			
			void EnterForeground(void);
			void EnterBackground(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			
			void ReattachMethod(MethodWidget *widget);
			void ReattachFiber(FiberWidget *widget);
			void ReattachSection(ScriptSectionWidget *widget);
			
			void RebuildFiberWidgets(const Method *method);
			
			void DeleteMethod(MethodWidget *methodWidget, List<MethodWidget> *deletedMethodList = nullptr, List<FiberWidget> *deletedFiberList = nullptr);
			void DeleteFiber(FiberWidget *fiberWidget, List<FiberWidget> *deletedFiberList = nullptr);
			void DeleteSection(ScriptSectionWidget *sectionWidget, List<ScriptSectionWidget> *deletedSectionList = nullptr);
			
			void SelectDefaultTool(void);
			void SelectMethodTool(const MethodRegistration *registration);
	};
}


#endif

// ZYURVUR
