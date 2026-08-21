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


#ifndef C4ShaderEditor_h
#define C4ShaderEditor_h


#include "C4Shaders.h"
#include "C4Primitives.h"
#include "C4Viewports.h"
#include "C4Configuration.h"


namespace C4
{
	enum
	{
		kShaderEditorModeTool,
		kShaderEditorModeProcess,
		kShaderEditorModeSection,
		kShaderEditorModeRoute,
		kShaderEditorModeCount
	};
	
	
	enum
	{
		kShaderToolGraphSelect = -1,
		kShaderToolProcessMove,
		kShaderToolViewportScroll,
		kShaderToolViewportZoom,
		kShaderToolCount
	};
	
	
	enum
	{
		kShaderMenuUndo,
		kShaderMenuCut,
		kShaderMenuCopy,
		kShaderMenuPaste,
		kShaderMenuClear,
		kShaderMenuDuplicate,
		
		kShaderMenuGetInfo,
		kShaderMenuToggleDetailLevel,
		kShaderMenuShowAmbientShader,
		kShaderMenuShowLightShader,
		
		kShaderMenuItemCount
	};
	
	
	enum
	{
		kProcessWidgetSelected			= 1 << 0,
		kProcessWidgetTempSelected		= 1 << 1
	};
	
	
	enum
	{
		kRouteWidgetSelected			= 1 << 0
	};
	
	
	enum
	{
		kShaderSectionWidgetSelected	= 1 << 0
	};
	
	
	enum
	{
		kShaderEditorModified			= 1 << 0,
		kShaderEditorUndoPending		= 1 << 1,
		kShaderEditorUpdateMenus		= 1 << 2,
		kShaderEditorUpdateGraph		= 1 << 3,
		kShaderEditorUpdateGrid			= 1 << 4
	};
	
	
	enum
	{
		kWidgetProcess					= 'proc',
		kWidgetRoute					= 'rout',
		kWidgetShaderSection			= 'shsc'
	};
	
	
	class ProcessWidget;
	class RouteWidget;
	class ShaderSectionWidget;
	class ShaderEditor;
	class PrimitiveGeometry;
	class Dialog;
	
	
	class ProcessReference : public ListElement<ProcessReference>
	{
		private:
			
			ProcessWidget	*reference;
		
		public: 
			
			ProcessReference(ProcessWidget *widget) 
			{ 
				reference = widget; 
			}
			 
			ProcessWidget *GetProcessWidget(void) const
			{
				return (reference);
			} 
	};
	
	
	class RouteReference : public ListElement<RouteReference> 
	{
		private:
			
			RouteWidget		*reference;
		
		public:
			
			RouteReference(RouteWidget *widget)
			{
				reference = widget;
			}
			
			RouteWidget *GetRouteWidget(void) const
			{
				return (reference);
			}
	};
	
	
	class ShaderSectionReference : public ListElement<ShaderSectionReference>
	{
		private:
			
			ShaderSectionWidget		*reference;
		
		public:
			
			ShaderSectionReference(ShaderSectionWidget *widget)
			{
				reference = widget;
			}
			
			ShaderSectionWidget *GetSectionWidget(void) const
			{
				return (reference);
			}
	};
	
	
	class ShaderUndoData : public ListElement<ShaderUndoData>
	{
		private:
			
			bool	coupledFlag;
		
		protected:
			
			ShaderUndoData();
		
		public:
			
			virtual ~ShaderUndoData();
			
			bool Coupled(void) const
			{
				return (coupledFlag);
			}
			
			void SetCoupledFlag(bool flag)
			{
				coupledFlag = flag;
			}
			
			virtual void Undo(ShaderEditor *shaderEditor) = 0;
	};
	
	
	class CreateShaderUndoData : public ShaderUndoData
	{
		private:
			
			List<ProcessReference>			createdProcessList;
			List<RouteReference>			createdRouteList;
			List<ShaderSectionReference>	createdSectionList;
		
		public:
			
			CreateShaderUndoData(ProcessWidget *process);
			CreateShaderUndoData(RouteWidget *route);
			CreateShaderUndoData(ShaderSectionWidget *section);
			CreateShaderUndoData(const List<ProcessWidget> *processList, const List<RouteWidget> *routeList, const List<ShaderSectionWidget> *sectionList);
			~CreateShaderUndoData();
			
			void Undo(ShaderEditor *shaderEditor);
	};
	
	
	class DeleteShaderUndoData : public ShaderUndoData
	{
		private:
			
			List<ProcessWidget>			deletedProcessList;
			List<RouteWidget>			deletedRouteList;
			List<ShaderSectionWidget>	deletedSectionList;
		
		public:
			
			DeleteShaderUndoData(List<ProcessWidget> *processList, List<RouteWidget> *routeList, List<ShaderSectionWidget> *sectionList);
			~DeleteShaderUndoData();
			
			void Undo(ShaderEditor *shaderEditor);
	};
	
	
	class MoveShaderUndoData : public ShaderUndoData
	{
		private:
			
			class MovedProcessReference : public ProcessReference
			{
				private:
					
					Point2D		position;
				
				public:
					
					MovedProcessReference(ProcessWidget *widget);
					
					const Point2D& GetPosition(void) const
					{
						return (position);
					}
			};
			
			class MovedSectionReference : public ShaderSectionReference
			{
				private:
					
					Point2D		position;
				
				public:
					
					MovedSectionReference(ShaderSectionWidget *widget);
					
					const Point2D& GetPosition(void) const
					{
						return (position);
					}
			};
			
			List<ProcessReference>			movedProcessList;
			List<ShaderSectionReference>	movedSectionList;
		
		public:
			
			MoveShaderUndoData(const List<ProcessWidget> *processList, const List<ShaderSectionWidget> *sectionList);
			~MoveShaderUndoData();
			
			void Undo(ShaderEditor *shaderEditor);
	};
	
	
	class ResizeShaderUndoData : public ShaderUndoData
	{
		private:
			
			ShaderSectionWidget		*sectionWidget;
			float					sectionWidth;
			float					sectionHeight;
		
		public:
			
			ResizeShaderUndoData(ShaderSectionWidget *widget);
			~ResizeShaderUndoData();
			
			void Undo(ShaderEditor *shaderEditor);
	};
	
	
	class RouteShaderUndoData : public ShaderUndoData
	{
		private:
			
			class DetailedReference : public RouteReference
			{
				private:
					
					unsigned_int32		flags;
				
				public:
					
					DetailedReference(RouteWidget *widget);
					
					unsigned_int32 GetFlags(void) const
					{
						return (flags);
					}
			};
			
			List<RouteReference>		routeList;
		
		public:
			
			RouteShaderUndoData(const List<RouteWidget> *selectionList);
			~RouteShaderUndoData();
			
			void Undo(ShaderEditor *shaderEditor);
	};
	
	
	class ProcessWidget : public TextWidget, public ListElement<ProcessWidget>
	{
		private:
			
			ShaderEditor				*shaderEditor;
			Process						*shaderProcess;
			const ProcessRegistration	*processRegistration;
			
			unsigned_int32				processWidgetState;
			float						viewportScale;
			
			Point3D						originalPosition;
			float						sortPosition;
			
			TextWidget					*commentText;
			TextWidget					*valueText;
			QuadWidget					*colorBox;
			ImageWidget					*textureBox;
			
			TextWidget					*portText[kMaxProcessPortCount];
			
			Point2D						processVertex[16];
			ColorRGBA					processColor[16];
			
			Point2D						portVertex[kMaxProcessPortCount * 12];
			ColorRGBA					portColor[kMaxProcessPortCount * 12];
			Point2D						portTexcoord[kMaxProcessPortCount * 12];
			
			Renderable					processRenderable;
			
			List<Attribute>				backgroundAttributeList;
			DiffuseAttribute			backgroundDiffuseAttribute;
			TextureMapAttribute			backgroundTextureMap;
			Renderable					backgroundRenderable;
			
			List<Attribute>				outputAttributeList;
			DiffuseAttribute			outputDiffuseAttribute;
			TextureMapAttribute			outputTextureMap;
			Renderable					outputRenderable;
			
			List<Attribute>				portAttributeList;
			TextureMapAttribute			portTextureMap;
			Renderable					portRenderable;
			
			static const Triangle		processTriangle[16];
			static const ConstPoint2D	backgroundVertex[4];
			static const ConstPoint2D	backgroundTexcoord[4];
			static const ConstPoint2D	outputVertex[4];
			static const ConstPoint2D	outputTexcoord[4];
			static const Triangle		portBoxTriangle[kMaxProcessPortCount * 10];
			
			bool CalculateBoundingBox(Box2D *box) const override;
		
		public:
			
			ProcessWidget(ShaderEditor *editor, Process *process, const ProcessRegistration *registration);
			~ProcessWidget();
			
			using ListElement<ProcessWidget>::Previous;
			using ListElement<ProcessWidget>::Next;
			
			Process *GetShaderProcess(void) const
			{
				return (shaderProcess);
			}
			
			unsigned_int32 GetProcessWidgetState(void) const
			{
				return (processWidgetState);
			}
			
			void SetProcessWidgetState(unsigned_int32 state)
			{
				processWidgetState = state;
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
			void UpdatePortColor(int32 port, bool hilite);
			void UpdateContent(void);
			
			void Select(unsigned_int32 state);
			void Unselect(void);
			
			WidgetPart TestPosition(const Point3D& position) const;
			
			void Build(void);
			void Render(List<Renderable> *renderList);
	};
	
	
	class RouteWidget : public RenderableWidget, public ListElement<RouteWidget>
	{
		private:
			
			ShaderEditor				*shaderEditor;
			Route						*shaderRoute;
			
			unsigned_int32				routeWidgetState;
			float						viewportScale;
			
			TextWidget					*swizzleText;
			
			Point2D						routeVertex[70];
			Vector4D					routeTangent[70];
			Point2D						routeTexcoord[70];
			
			Vector4D					selectionTangent[70];
			Point2D						selectionTexcoord[70];
			
			List<Attribute>				routeAttributeList;
			DiffuseAttribute			routeDiffuseAttribute;
			TextureMapAttribute			routeTextureMapAttribute;
			
			List<Attribute>				selectionAttributeList;
			DiffuseAttribute			selectionDiffuseAttribute;
			TextureMapAttribute			selectionTextureMapAttribute;
			Renderable					selectionRenderable;
			
			Renderable					swizzleRenderable;
			
			Point2D						swizzleVertex[12];
			static const ConstColorRGB	swizzleColor[12];
			static const Triangle		swizzleTriangle[10];
			
			bool CalculateBoundingBox(Box2D *box) const override;
		
		public:
			
			RouteWidget(ShaderEditor *editor, Route *route);
			~RouteWidget();
			
			using ListElement<RouteWidget>::Previous;
			using ListElement<RouteWidget>::Next;
			
			Route *GetShaderRoute(void) const
			{
				return (shaderRoute);
			}
			
			void Rebuild(void)
			{
				SetBuildFlag();
				Invalidate();
			}
			
			unsigned_int32 GetRouteWidgetState(void) const
			{
				return (routeWidgetState);
			}
			
			void SetRouteWidgetState(unsigned_int32 state)
			{
				routeWidgetState = state;
			}
			
			void SetViewportScale(float scale)
			{
				viewportScale = scale;
				SetBuildFlag();
			}
			
			void UpdateContent(void);
			
			void Select(void);
			void Unselect(void);
			
			WidgetPart TestPosition(const Point3D& position) const;
			
			void Build(void);
			void Render(List<Renderable> *renderList);
	};
	
	
	class ShaderSectionWidget : public TextWidget, public ListElement<ShaderSectionWidget>
	{
		private:
			
			ShaderEditor			*shaderEditor;
			SectionProcess			*sectionProcess;
			
			unsigned_int32			sectionWidgetState;
			float					viewportScale;
			
			Point3D					originalPosition;
			
			Renderable				sectionRenderable;
			
			Point2D					sectionVertex[27];
			ColorRGBA				sectionColor[27];
			static const Triangle	sectionTriangle[21];
		
		public:
			
			ShaderSectionWidget(ShaderEditor *editor, SectionProcess *process);
			~ShaderSectionWidget();
			
			using ListElement<ShaderSectionWidget>::Previous;
			using ListElement<ShaderSectionWidget>::Next;
			
			SectionProcess *GetSectionProcess(void) const
			{
				return (sectionProcess);
			}
			
			unsigned_int32 GetSectionWidgetState(void) const
			{
				return (sectionWidgetState);
			}
			
			void SetSectionWidgetState(unsigned_int32 state)
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
	
	
	class PreviewWidget : public FrustumViewportWidget
	{
		private:
			
			float					cameraDistance;
			
			bool					trackFlag;
			Point3D					previousPosition;
			
			World					*previewWorld;
			Zone					*previewZone;
			Light					*previewLight;
			PrimitiveGeometry		*previewGeometry;
			
			MaterialObject			*previewMaterial;
			
			void SetCameraAngles(float azm, float alt);
			
			static void ViewportHandleMouseEvent(const MouseEventData *eventData, ViewportWidget *viewport, void *cookie);
			static void ViewportRender(List<Renderable> *renderList, ViewportWidget *viewport, void *cookie);
		
		public:
			
			PreviewWidget(const Vector2D& size, MaterialObject *materialObject);
			~PreviewWidget();
			
			const PrimitiveGeometry *GetPreviewGeometry(void) const
			{
				return (previewGeometry);
			}
			
			void Preprocess(void);
			
			void SetMaterial(MaterialObject *materialObject);
			void SetPreviewGeometry(PrimitiveType type);
			
			void UpdatePreview(void);
	};
	
	
	class ShaderPage : public Page
	{
		private:
			
			ShaderEditor	*shaderEditor;
		
		protected:
			
			ShaderPage(ShaderEditor *editor, const char *panelName);
		
		public:
			
			~ShaderPage();
			
			ShaderEditor *GetShaderEditor(void) const
			{
				return (shaderEditor);
			}
	};
	
	
	class ShaderProcessesPage : public ShaderPage
	{
		private:
			
			enum
			{
				kProcessPaneCount = 4
			};
			
			class ToolWidget : public TextWidget
			{
				public:
					
					const ProcessRegistration	*processRegistration;
					
					ToolWidget(const Vector2D& size, const ProcessRegistration *registration);
					~ToolWidget();
					
					static String<127> GetProcessName(const ProcessRegistration *registration);
			};
			
			MultipaneWidget							*multipaneWidget;
			ListWidget								*listWidget[kProcessPaneCount];
			
			WidgetObserver<ShaderProcessesPage>		multipaneWidgetObserver;
			WidgetObserver<ShaderProcessesPage>		listWidgetObserver;
			
			void HandleMultipaneWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			void HandleListWidgetEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			ShaderProcessesPage(ShaderEditor *editor);
			~ShaderProcessesPage();
			
			void Preprocess(void);
			
			void SetShaderGraph(int32 shader);
			void UnselectProcessTool(void);
	};
	
	
	class ShaderPreviewPage : public ShaderPage
	{
		private:
			
			PrimitiveType						initPrimitiveType;
			
			MaterialObject						*previewMaterial;
			ShaderAttribute						*previewAttribute;
			
			PreviewWidget						*previewWidget;
			PopupMenuWidget						*menuWidget;
			
			WidgetObserver<ShaderPreviewPage>	menuWidgetObserver;
			
			void HandleMenuWidgetEvent(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			ShaderPreviewPage(ShaderEditor *editor, PrimitiveType primitiveType);
			~ShaderPreviewPage();
			
			void Preprocess(void);
			
			void UpdatePreviewMaterial(const ShaderGraph *shaderGraph, ShaderResult *result);
			void SetPreviewGeometry(PrimitiveType type);
	};
	
	
	class ProcessInfoWindow : public Window
	{
		private:
			
			ShaderEditor			*shaderEditor;
			ProcessWidget			*processWidget;
			
			PushButtonWidget		*okayButton;
			PushButtonWidget		*cancelButton;
			
			ConfigurationWidget		*configurationWidget;
		
		public:
			
			ProcessInfoWindow(ShaderEditor *editor);
			~ProcessInfoWindow();
			
			void Preprocess(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
	
	
	class RouteInfoWindow : public Window
	{
		private:
			
			ShaderEditor			*shaderEditor;
			RouteWidget				*routeWidget;
			
			PushButtonWidget		*okayButton;
			PushButtonWidget		*cancelButton;
			
			ConfigurationWidget		*configurationWidget;
		
		public:
			
			RouteInfoWindow(ShaderEditor *editor);
			~RouteInfoWindow();
			
			void Preprocess(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
	
	
	class ShaderSectionInfoWindow : public Window
	{
		private:
			
			ShaderEditor			*shaderEditor;
			ShaderSectionWidget		*sectionWidget;
			
			PushButtonWidget		*okayButton;
			PushButtonWidget		*cancelButton;
			
			ConfigurationWidget		*configurationWidget;
		
		public:
			
			ShaderSectionInfoWindow(ShaderEditor *editor);
			~ShaderSectionInfoWindow();
			
			void Preprocess(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
	
	
	class ShaderEditor : public Window, public Completable<ShaderEditor>
	{
		private:
			
			MaterialObject					*materialObject;
			PrimitiveType					initPrimitiveType;
			
			ShaderGraph						shaderGraph[kShaderGraphCount];
			
			unsigned_int32					editorState;
			
			int32							currentShader;
			int32							currentMode;
			int32							currentTool;
			const ProcessRegistration		*currentProcessReg;
			
			List<ShaderUndoData>			undoList[kShaderGraphCount];
			
			int32							trackingMode;
			int32							trackingTool;
			bool							toolTracking;
			bool							boxSelectFlag;
			Point3D							previousPoint;
			Point3D							previousPosition;
			Point3D							anchorPoint;
			Point3D							anchorPosition;
			
			ProcessWidget					*routeStartProcess;
			ProcessWidget					*routeFinishProcess;
			int32							routePort;
			
			ShaderSectionWidget				*sectionTrackWidget;
			
			Widget							*graphRoot[kShaderGraphCount];
			Widget							*sectionRoot[kShaderGraphCount];
			
			List<ProcessWidget>				processWidgetList[kShaderGraphCount];
			List<RouteWidget>				routeWidgetList[kShaderGraphCount];
			List<ShaderSectionWidget>		sectionWidgetList[kShaderGraphCount];
			
			List<ProcessWidget>				selectedProcessList[kShaderGraphCount];
			List<RouteWidget>				selectedRouteList[kShaderGraphCount];
			List<ShaderSectionWidget>		selectedSectionList[kShaderGraphCount];
			
			PushButtonWidget				*okayButton;
			PushButtonWidget				*cancelButton;
			
			MultipaneWidget					*shaderMultipaneWidget;
			OrthoViewportWidget				*shaderViewport[kShaderGraphCount];
			
			IconButtonWidget				*toolButton[kShaderToolCount];
			IconButtonWidget				*sectionButton;
			
			TextWidget						*statusMessage[kShaderGraphCount];
			
			MenuBarWidget					*menuBar;
			PulldownMenuWidget				*editMenu;
			PulldownMenuWidget				*shaderMenu;
			MenuItemWidget					*shaderMenuItem[kShaderMenuItemCount];
			
			BookWidget						*bookWidget;
			ShaderProcessesPage				*processesPage;
			ShaderPreviewPage				*previewPage;

			WidgetObserver<ShaderEditor>	toolButtonObserver;
			
			Grid							viewportGrid;
			DragRect						dragRect;
			
			List<Attribute>					routeAttributeList;
			DiffuseAttribute				routeDiffuseColor;
			TextureMapAttribute				routeTextureMap;
			Renderable						routeRenderable;
			Point2D							routeVertex[4];
			Vector4D						routeTangent[4];
			Point2D							routeTexcoord[4];
			
			static ShaderGraph				editorClipboard;
			
			bool ProcessSelected(const ProcessWidget *widget) const
			{
				return (selectedProcessList[currentShader].Member(widget));
			}
			
			bool RouteSelected(const RouteWidget *widget) const
			{
				return (selectedRouteList[currentShader].Member(widget));
			}
			
			bool SectionSelected(const ShaderSectionWidget *widget) const
			{
				return (selectedSectionList[currentShader].Member(widget));
			}
			
			void TranslateAmbientAttributes(void);
			void TranslateLightAttributes(void);
			
			void PositionWidgets(void);
			void BuildMenus(void);
			
			void BuildShaderGraph(int32 index);
			void UpdateShaderGraph(int32 index);
			
			void UnselectCurrentTool(void);
			void UpdateViewportScale(float scale);
			
			void SelectProcess(ProcessWidget *widget, unsigned_int32 state = 0);
			void UnselectProcess(ProcessWidget *widget);
			void SelectRoute(RouteWidget *widget);
			void UnselectRoute(RouteWidget *widget);
			void SelectSection(ShaderSectionWidget *widget);
			void UnselectSection(ShaderSectionWidget *widget);
			void SelectAll(void);
			void UnselectAll(void);
			void UnselectAllTemp(void);
			
			void AddUndoData(ShaderUndoData *data);
			void RemoveUndoData(ShaderUndoData *data);
			
			void HandleUndoMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleCutMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleCopyMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandlePasteMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleClearMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleSelectAllMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleDuplicateMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			
			void HandleGetInfoMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleToggleDetailLevelMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			void HandleShowViewportMenuItem(Widget *menuItem, const WidgetEventData *eventData);
			
			Point3D ViewportToGraphPosition(const Point3D& p) const;
			static Point3D AlignPositionToGrid(const Point3D& p);
			void ShowGraphPosition(float x, float y);
			void AutoScroll(const Point3D& p);
			
			ProcessWidget *FindProcessWidget(const Point3D& position) const;
			ProcessWidget *FindProcessPort(const Point3D& position, int32 *port) const;
			RouteWidget *GetRouteWidget(const Route *route) const;
			
			void SortSelectedProcessSublist(List<ProcessWidget> *list, float dmin, float dmax);
			void SortSelectedProcessList(float dx, float dy);
			
			static bool BoxIntersectsProcessWidget(const Point3D& p1, const Point3D& p2, const ProcessWidget *widget);
			bool ProcessBoxIntersectsAnyProcessWidget(float x, float y, const ProcessWidget *exclude = nullptr);
			
			void BeginTool(const Point3D& p, unsigned_int32 eventFlags);
			void TrackTool(const Point3D& p);
			void EndTool(const Point3D& p);
			
			void BeginSection(const Point3D& p);
			void TrackSection(const Point3D& p);
			void EndSection(const Point3D& p);
			
			void TrackRoute(const Point3D& p);
			void EndRoute(const Point3D& p);
			
			void CreateProcess(const Point3D& p);
			
			static void ViewportHandleMouseEvent(const MouseEventData *eventData, ViewportWidget *viewport, void *cookie);
			static void ViewportTrackTask(const Point3D& position, ViewportWidget *viewport, void *cookie);
			static void ViewportRender(List<Renderable> *renderList, ViewportWidget *viewport, void *cookie);
			
			void HandleToolButtonEvent(Widget *widget, const WidgetEventData *eventData);
			
			static void ConfirmationDialogComplete(Dialog *dialog, void *cookie);
		
		public:
			
			ShaderEditor(MaterialObject *material, PrimitiveType primitiveType = kPrimitiveSphere);
			~ShaderEditor();
			
			MaterialObject *GetMaterialObject(void) const
			{
				return (materialObject);
			}
			
			ProcessWidget *GetFirstSelectedProcess(void) const
			{
				return (selectedProcessList[currentShader].First());
			}
			
			RouteWidget *GetFirstSelectedRoute(void) const
			{
				return (selectedRouteList[currentShader].First());
			}
			
			ShaderSectionWidget *GetFirstSelectedSection(void) const
			{
				return (selectedSectionList[currentShader].First());
			}
			
			void SetModifiedFlag(void)
			{
				editorState |= kShaderEditorModified | kShaderEditorUpdateGraph;
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
			
			void ReattachProcess(ProcessWidget *widget);
			void ReattachRoute(RouteWidget *widget);
			void ReattachSection(ShaderSectionWidget *widget);
			
			void RebuildRouteWidgets(const Process *process);
			
			void DeleteProcess(ProcessWidget *processWidget, List<ProcessWidget> *deletedProcessList = nullptr, List<RouteWidget> *deletedRouteList = nullptr);
			void DeleteRoute(RouteWidget *routeWidget, List<RouteWidget> *deletedRouteList = nullptr);
			void DeleteSection(ShaderSectionWidget *sectionWidget, List<ShaderSectionWidget> *deletedSectionList = nullptr);
			
			void SelectDefaultTool(void);
			void SelectProcessTool(const ProcessRegistration *registration);
	};
}


#endif

// ZYURVUR
