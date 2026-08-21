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


#include "C4World.h"
#include "C4WorldEditor.h"
#include "C4EditorSupport.h"


using namespace C4;


namespace
{
	static const char *const axesImageName[6] =
	{
		"WorldEditor/axes/AxesTop",
		"WorldEditor/axes/AxesBottom",
		"WorldEditor/axes/AxesFront",
		"WorldEditor/axes/AxesBack",
		"WorldEditor/axes/AxesRight",
		"WorldEditor/axes/AxesLeft"
	};
}


const unsigned_int32 EditorViewport::viewportIdentifier[8] =
{
	'TOP ', 'BTTM', 'FRNT', 'BACK', 'RGHT', 'LEFT', 'PERS', 'GRPH'
};


EditorViewport::EditorViewport(EditorViewportType type, Editor *editor, int32 mode, ViewportWidget *widget) :
		borderWidget(Vector2D(1.0F, 1.0F)),
		titleWidget(Vector2D(128.0F, 16.0F), TheWorldEditor->GetStringTable()->GetString(StringID('VPRT', viewportIdentifier[mode])), "font/Gui")
{
	editorViewportType = type;
	
	worldEditor = editor;
	viewportWidget = widget;
	
	toolRenderable = nullptr;
}

EditorViewport::~EditorViewport()
{
}

void EditorViewport::Postconstruct(void)
{
	viewportWidget->SetTrackTaskProc(&HandleViewportTrack, this);
	
	viewportWidget->AddSubnode(&borderWidget);
	borderWidget.Disable();
	
	titleWidget.SetWidgetPosition(Point3D(4.0F, 2.0F, 0.0F));
	titleWidget.SetWidgetColor(ColorRGBA(1.0F, 1.0F, 0.5F, 1.0F));
	viewportWidget->AddSubnode(&titleWidget);
	titleWidget.Disable();
}

void EditorViewport::Predestruct(void)
{
	borderWidget.UpdatableTree<Widget>::Detach();
	titleWidget.UpdatableTree<Widget>::Detach();
}

void EditorViewport::SetViewportPosition(const Point3D& position, const Vector2D& size)
{
	viewportWidget->SetWidgetPosition(position);
	viewportWidget->SetWidgetSize(size);
	viewportWidget->Invalidate();
	
	if (viewportWidget->Visible()) viewportWidget->AllocateTexture();
	
	borderWidget.SetWidgetSize(size);
}

void EditorViewport::Invalidate(void)
{
	viewportWidget->InvalidateTexture();
}

void EditorViewport::ShowViewportInfo(void)
{
	titleWidget.Show();
}

void EditorViewport::HideViewportInfo(void)
{
	titleWidget.Hide();
}

void EditorViewport::Hover(const Point3D& position)
{
}

void EditorViewport::HandleViewportTrack(const Point3D& position, ViewportWidget *viewport, void *cookie)
{
	EditorViewport *editorViewport = static_cast<EditorViewport *>(cookie);
	Editor *editor = editorViewport->GetEditor();
	
	EditorTool *tool = editor->GetTrackingTool();
	if (tool) tool->TrackTool(editor, editor->CalculateTrackData(viewport, position));
}
 
void EditorViewport::RenderNode(const Node *root, const ManipulatorRenderData *renderData)
{ 
	EditorManipulator *manipulator = Editor::GetManipulator(root); 
	const BoundingSphere *sphere = manipulator->GetTreeSphere(); 
	if (sphere)
	{ 
		const Camera *camera = viewportWidget->GetViewportCamera();
		if (camera->SphereVisible(sphere->GetCenter(), sphere->GetRadius()))
		{
			unsigned_int32 state = manipulator->GetManipulatorState(); 
			if ((state & (kManipulatorHidden | kManipulatorForceRender)) != kManipulatorHidden)
			{
				sphere = manipulator->GetNodeSphere();
				if ((sphere) && (camera->SphereVisible(sphere->GetCenter(), sphere->GetRadius()))) 
				{
					if (root->GetNodeType() == kNodeGeometry)
					{
						Controller *controller = root->GetController();
						if ((controller) && (controller->GetControllerFlags() & kControllerUpdate)) controller->Update();
					}
					
					int32 index = (state & (kManipulatorSelected | kManipulatorHilited)) ? 0 : 1;
					manipulator->Render(&renderData[index]);
				}
			}
			
			const Node *node = root->GetFirstSubnode();
			while (node)
			{
				RenderNode(node, renderData);
				node = node->Next();
			}
		}
	}
}


OrthoEditorViewport::OrthoEditorViewport(Editor *editor, const Vector2D& scale, int32 mode) :
		EditorViewport(kEditorViewportOrtho, editor, mode, &orthoViewport),
		orthoViewport(Vector2D(1.0F, 1.0F), scale),
		axesWidget(Vector2D(32.0F, 32.0F), axesImageName[mode])
{
	Postconstruct();
	
	orthoViewport.SetMouseEventProc(&HandleViewportMouseEvent, this);
	orthoViewport.SetRenderProc(&RenderViewport, this);
	
	orthoViewport.AddSubnode(&axesWidget);
	axesWidget.Disable();
	
	OrthoCameraObject *object = orthoViewport.GetViewportCamera()->GetObject();
	object->SetClearFlags(kClearColorBuffer | kClearDepthBuffer | kClearStencilBuffer);
	object->SetClearColor(ColorRGBA(0.0F, 0.0F, 0.0F, 1.0F));
	object->SetNearDepth(-131072.0F);
	object->SetFarDepth(131072.0F);
	
	gridValidFlag = false;
	viewportGrid.SetTransformable(&gridTransformable);
}

OrthoEditorViewport::~OrthoEditorViewport()
{
	Predestruct();
}

void OrthoEditorViewport::SetViewportPosition(const Point3D& position, const Vector2D& size)
{
	EditorViewport::SetViewportPosition(position, size);
	
	axesWidget.SetWidgetPosition(Point3D(2.0F, size.y - 34.0F, 0.0F));
	gridValidFlag = false;
}

void OrthoEditorViewport::Invalidate(void)
{
	EditorViewport::Invalidate();
	gridValidFlag = false;
}

void OrthoEditorViewport::ShowViewportInfo(void)
{
	EditorViewport::ShowViewportInfo();
	axesWidget.Show();
}

void OrthoEditorViewport::HideViewportInfo(void)
{
	EditorViewport::HideViewportInfo();
	axesWidget.Hide();
}

void OrthoEditorViewport::HandleViewportMouseEvent(const MouseEventData *eventData, ViewportWidget *viewport, void *cookie)
{
	OrthoEditorViewport *editorViewport = static_cast<OrthoEditorViewport *>(cookie);
	Editor *editor = editorViewport->GetEditor();
	
	EventType eventType = eventData->eventType;
	if ((eventType == kEventMouseDown) || (eventType == kEventMiddleMouseDown) || (eventType == kEventRightMouseDown))
	{
		editor->SetFocusWidget(nullptr);
		
		if (!(editor->GetEditorState() & kEditorWaitUpdate))
		{
			if (eventType == kEventRightMouseDown)
			{
				editor->ActivateViewportMenu(viewport->GetViewportIndex(), eventData->mousePosition);
				return;
			}
			
			unsigned_int32 modifierKeys = InterfaceMgr::GetModifierKeys();
			EditorTool *tool = editor->GetCurrentTool();
			
			if ((eventType == kEventMiddleMouseDown) || (modifierKeys & kModifierKeyOption))
			{
				tool = editor->GetStandardTool(kEditorToolViewportScroll);
			}
			else if (modifierKeys & kModifierKeyCommand)
			{
				tool = editor->GetStandardTool(kEditorToolOrbitCamera);
			}
			
			EditorTrackData *trackData = editor->GetTrackData();
			
			trackData->viewportIndex = viewport->GetViewportIndex();
			trackData->viewportType = kEditorViewportOrtho;
			trackData->viewportScale = static_cast<OrthoViewportWidget *>(viewport)->GetOrthoScale().x;
			trackData->editorFlags = editor->GetEditorObject()->GetEditorFlags();
			trackData->viewportCamera = viewport->GetViewportCamera();
			
			trackData->currentPosition.Set(0.0F, 0.0F);
			trackData->snappedCurrentPosition.Set(0.0F, 0.0F);
			trackData->currentViewportPosition.Set(0.0F, 0.0F);
			trackData->currentPickPoint.Set(0.0F, 0.0F, 0.0F);
			trackData->currentPickNormal.Set(0.0F, 0.0F, 1.0F);
			trackData->mouseEventFlags = eventData->eventFlags;
			trackData->currentModifierKeys = modifierKeys;
			
			editor->CalculateTrackData(viewport, eventData->mousePosition);
			trackData->anchorPosition = trackData->currentPosition;
			trackData->snappedAnchorPosition = trackData->snappedCurrentPosition;
			trackData->anchorViewportPosition = trackData->currentViewportPosition;
			trackData->currentSize.Set(0.0F, 0.0F);
			
			editor->ResetScrollFraction();
			trackData->trackType = kEditorTrackNode;
			trackData->trackNode = nullptr;
			trackData->superNode = nullptr;
			trackData->gizmo = nullptr;
			
			if (tool->BeginTool(editor, trackData)) editor->SetTrackingTool(tool);
		}
	}
	else if ((eventType == kEventMouseUp) || (eventType == kEventMiddleMouseUp) || (eventType == kEventRightMouseUp))
	{
		EditorTool *tool = editor->GetTrackingTool();
		if (tool)
		{
			tool->EndTool(editor, editor->CalculateTrackData(viewport, eventData->mousePosition));
			
			editor->SetTrackingTool(nullptr);
			editor->ClearPickFilterProc();
		}
	}
	else if (eventType == kEventMouseWheel)
	{
		EditorTrackData		trackData;
		
		trackData.viewportIndex = viewport->GetViewportIndex();
		trackData.viewportType = kEditorViewportOrtho;
		trackData.viewportScale = static_cast<OrthoViewportWidget *>(viewport)->GetOrthoScale().x;
		trackData.viewportCamera = viewport->GetViewportCamera();
		
		trackData.previousViewportPosition.Set(0.0F, 0.0F);
		trackData.currentViewportPosition.Set(0.0F, eventData->mousePosition.y * -0.04F);
		
		editor->GetStandardTool(kEditorToolViewportZoom)->TrackTool(editor, &trackData);
	}
}

void OrthoEditorViewport::RenderViewport(List<Renderable> *renderList, ViewportWidget *viewport, void *cookie)
{
	ManipulatorRenderData	renderData[2];
	List<Renderable>		geometryList[2];
	List<Renderable>		manipulatorList[2];
	List<Renderable>		gizmoList;
	List<Renderable>		connectorList;
	List<Renderable>		handleList;
	
	OrthoEditorViewport *editorViewport = static_cast<OrthoEditorViewport *>(cookie);
	Editor *editor = editorViewport->GetEditor();
	
	const EditorObject *object = editor->GetEditorObject();
	unsigned_int32 editorFlags = object->GetEditorFlags();
	
	if (editorFlags & kEditorShowGridlines) editorViewport->RenderGrid(object);
	
	unsigned_int32 wireFlags = kWireframeColor;
	if (editorFlags & kEditorShowBackfaces) wireFlags |= kWireframeTwoSided;
	
	unsigned_int32 renderFlags = editor->GetRenderFlags();
	bool renderConnectors = (((renderFlags & kEditorRenderConnectors) != 0) || (!editor->GetSelectedConnectorList()->Empty()));
	bool renderHandles = ((renderFlags & kEditorRenderHandles) != 0);
	
	int32 index = viewport->GetViewportIndex();
	float scale = static_cast<OrthoViewportWidget *>(viewport)->GetOrthoScale().x;
	
	renderData[0].viewportIndex = index;
	renderData[0].viewportType = kEditorViewportOrtho;
	renderData[0].viewportScale = scale;
	renderData[0].editorFlags = editorFlags;
	renderData[0].viewportCamera = viewport->GetViewportCamera();
	renderData[0].geometryList = &geometryList[0];
	renderData[0].manipulatorList = &manipulatorList[0];
	renderData[0].gizmoList = &gizmoList;
	renderData[0].connectorList = (renderConnectors) ? &connectorList : nullptr;
	renderData[0].handleList = (renderHandles) ? &handleList : nullptr;
	
	renderData[1].viewportIndex = index;
	renderData[1].viewportType = kEditorViewportOrtho;
	renderData[1].viewportScale = scale;
	renderData[1].viewportCamera = viewport->GetViewportCamera();
	renderData[1].editorFlags = editorFlags;
	renderData[1].geometryList = &geometryList[1];
	renderData[1].manipulatorList = &manipulatorList[1];
	renderData[1].gizmoList = nullptr;
	renderData[1].connectorList = renderData[0].connectorList;
	renderData[1].handleList = nullptr;
	
	editorViewport->RenderNode(editor->GetRootNode(), renderData);
	editor->GetEditorWorld()->FinishWorldBatch();
	
	TheGraphicsMgr->DrawWireframe(wireFlags, &geometryList[1]);
	geometryList[1].RemoveAll();
	
	TheGraphicsMgr->DrawRenderList(&manipulatorList[1]);
	manipulatorList[1].RemoveAll();
	
	TheGraphicsMgr->DrawWireframe(wireFlags, &geometryList[0]);
	geometryList[0].RemoveAll();
	
	TheGraphicsMgr->DrawRenderList(&manipulatorList[0]);
	manipulatorList[0].RemoveAll();
	
	TheGraphicsMgr->DrawRenderList(&gizmoList);
	gizmoList.RemoveAll();
	
	if (renderConnectors)
	{
		TheGraphicsMgr->DrawRenderList(&connectorList);
		connectorList.RemoveAll();
	}
	
	if (renderHandles)
	{
		TheGraphicsMgr->DrawRenderList(&handleList);
		handleList.RemoveAll();
	}
	
	Renderable *renderable = editorViewport->GetToolRenderable();
	if (renderable)
	{
		editorViewport->SetToolRenderable(nullptr);
		renderList->Append(renderable);
	}
}

void OrthoEditorViewport::RenderGrid(const EditorObject *editorObject)
{
	List<Renderable>	renderList;
	
	if (!gridValidFlag)
	{
		gridValidFlag = true;
		
		viewportGrid.SetGridLineSpacing(editorObject->GetGridLineSpacing());
		viewportGrid.SetMajorLineInterval(editorObject->GetMajorLineInterval());
		
		const ColorRGB& color = editorObject->GetGridColor();
		viewportGrid.SetAxisLineColor(color * 0.75F);
		viewportGrid.SetMajorLineColor(color * 0.375F);
		viewportGrid.SetMinorLineColor(color * 0.25F);
		
		const OrthoCamera *camera = orthoViewport.GetViewportCamera();
		const Transform4D& transform = camera->GetNodeTransform();
		gridTransformable.SetWorldTransform(transform(0,0), transform(0,1), transform(0,2), 0.0F, transform(1,0), transform(1,1), transform(1,2), 0.0F, transform(2,0), transform(2,1), transform(2,2), 0.0F);
		
		Vector3D position = gridTransformable.GetInverseWorldTransform() * static_cast<const Vector3D&>(camera->GetNodePosition());
		const OrthoCameraObject *cameraObject = camera->GetObject();
		float xmin = cameraObject->GetOrthoRectLeft() + position.x;
		float xmax = cameraObject->GetOrthoRectRight() + position.x;
		float ymin = cameraObject->GetOrthoRectTop() + position.y;
		float ymax = cameraObject->GetOrthoRectBottom() + position.y;
		
		viewportGrid.Build(Point2D(xmin, ymin), Point2D(xmax, ymax), orthoViewport.GetOrthoScale().x);
	}
	
	renderList.Append(&viewportGrid);
	TheGraphicsMgr->DrawRenderList(&renderList);
	renderList.Remove(&viewportGrid);
}


FrustumEditorViewport::FrustumEditorViewport(Editor *editor) :
		EditorViewport(kEditorViewportFrustum, editor, kViewportModeFrustum, &frustumViewport),
		frustumViewport(Vector2D(1.0F, 1.0F), 2.0F)
{
	Postconstruct();
	
	frustumViewport.SetMouseEventProc(&HandleViewportMouseEvent, this);
	frustumViewport.SetTrackTaskProc(&HandleViewportTrack, this);
	frustumViewport.SetRenderProc(&RenderViewport, this);
	frustumViewport.SetOverlayProc(&RenderOverlay, this);
	
	FrustumCameraObject *object = frustumViewport.GetViewportCamera()->GetObject();
	object->SetFrustumFlags(kFrustumInfinite);
	object->SetNearDepth(0.1F);
	object->SetFarDepth(1000.0F);
	
	viewportTrackMode = kViewportTrackNone;
	
	multiaxisTranslationRate.Set(0.0F, 0.0F, 0.0F);
	multiaxisRotationRate.Set(0.0F, 0.0F, 0.0F);
}

FrustumEditorViewport::~FrustumEditorViewport()
{
	Predestruct();
}

void FrustumEditorViewport::Hover(const Point3D& position)
{
	const Editor *editor = GetEditor();
	
	const NodeReference *reference = editor->GetGizmoTarget();
	if (reference)
	{
		Ray		ray;
		
		const EditorManipulator *manipulator = Editor::GetManipulator(reference->GetNode());
		if (!(manipulator->GetManipulatorFlags() & kManipulatorLockedTransform))
		{
			EditorGizmo *gizmo = manipulator->GetGizmo();
			int32 gizmoFace = -1;
			int32 gizmoEdge = -1;
			
			if ((!editor->GetTrackingTool()) && (InterfaceMgr::GetModifierKeys() == 0))
			{
				const EditorTool *tool = editor->GetCurrentTool();
				const EditorTool *moveTool = editor->GetStandardTool(kEditorToolNodeMove);
				const EditorTool *rotateTool = editor->GetStandardTool(kEditorToolNodeRotate);
				
				if ((tool == moveTool) || (tool == rotateTool))
				{
					float x = position.x / frustumViewport.GetWidgetSize().x;
					float y = position.y / frustumViewport.GetWidgetSize().y;
					
					const Camera *camera = frustumViewport.GetViewportCamera();
					camera->CastRay(x, y, &ray);
					
					const Transform4D& cameraTransform = camera->GetNodeTransform();
					ray.origin = cameraTransform * ray.origin;
					ray.direction = (cameraTransform * ray.direction).Normalize();
					
					if (tool == moveTool) gizmoFace = gizmo->PickFace(&ray);
					else gizmoEdge = gizmo->PickEdge(&ray);
				}
			}
			
			if (viewportTrackMode != kViewportTrackMoveGizmo) gizmo->HiliteFace(gizmoFace);
			if (viewportTrackMode != kViewportTrackRotateGizmo) gizmo->HiliteEdge(gizmoEdge);
		}
	}
}

void FrustumEditorViewport::HandleViewportMouseEvent(const MouseEventData *eventData, ViewportWidget *viewport, void *cookie)
{
	FrustumEditorViewport *editorViewport = static_cast<FrustumEditorViewport *>(cookie);
	Editor *editor = editorViewport->GetEditor();
	
	EventType eventType = eventData->eventType;
	if ((eventType == kEventMouseDown) || (eventType == kEventMiddleMouseDown) || (eventType == kEventRightMouseDown))
	{
		editor->SetFocusWidget(nullptr);
		editorViewport->viewportTrackMode = kViewportTrackNone;
		
		if (!(editor->GetEditorState() & kEditorWaitUpdate))
		{
			unsigned_int32 modifierKeys = InterfaceMgr::GetModifierKeys();
			EditorTool *tool = editor->GetCurrentTool();
			
			if (eventType == kEventRightMouseDown)
			{
				if (modifierKeys & kModifierKeyCommand)
				{
					editor->ActivateViewportMenu(viewport->GetViewportIndex(), eventData->mousePosition);
					return;
				}
				
				tool = editor->GetStandardTool(kEditorToolFreeCamera);
			}
			else if ((eventType == kEventMiddleMouseDown) || (modifierKeys & kModifierKeyOption))
			{
				tool = editor->GetStandardTool(kEditorToolViewportScroll);
			}
			else if (InterfaceMgr::GetCommandKey())
			{
				tool = editor->GetStandardTool(kEditorToolOrbitCamera);
			}
			
			EditorTrackData *trackData = editor->GetTrackData();
			
			trackData->viewportIndex = viewport->GetViewportIndex();
			trackData->viewportType = kEditorViewportFrustum;
			trackData->viewportScale = Editor::kFrustumRenderScale;
			trackData->editorFlags = editor->GetEditorObject()->GetEditorFlags();
			trackData->viewportCamera = viewport->GetViewportCamera();
			
			trackData->currentPosition.Set(0.0F, 0.0F);
			trackData->snappedCurrentPosition.Set(0.0F, 0.0F);
			trackData->currentViewportPosition.Set(0.0F, 0.0F);
			trackData->currentPickPoint.Set(0.0F, 0.0F, 0.0F);
			trackData->currentPickNormal.Set(0.0F, 0.0F, 1.0F);
			trackData->mouseEventFlags = eventData->eventFlags;
			trackData->currentModifierKeys = modifierKeys;
			
			editor->CalculateTrackData(viewport, eventData->mousePosition);
			trackData->anchorPosition = trackData->currentPosition;
			trackData->snappedAnchorPosition = trackData->snappedCurrentPosition;
			trackData->anchorViewportPosition = trackData->currentViewportPosition;
			trackData->currentSize.Set(0.0F, 0.0F);
			
			editor->ResetScrollFraction();
			trackData->trackType = kEditorTrackNode;
			trackData->trackNode = nullptr;
			trackData->superNode = nullptr;
			trackData->gizmo = nullptr;
			
			if (!(modifierKeys & kModifierKeyShift))
			{
				const NodeReference *reference = editor->GetGizmoTarget();
				if (reference)
				{
					const EditorTool *moveTool = editor->GetStandardTool(kEditorToolNodeMove);
					const EditorTool *rotateTool = editor->GetStandardTool(kEditorToolNodeRotate);
					if ((tool == moveTool) || (tool == rotateTool))
					{
						Node *node = reference->GetNode();
						const EditorManipulator *manipulator = Editor::GetManipulator(node);
						if (!(manipulator->GetManipulatorFlags() & kManipulatorLockedTransform))
						{
							EditorGizmo *gizmo = manipulator->GetGizmo();
							if (tool == moveTool)
							{
								int32 gizmoFace = gizmo->PickFace(&trackData->worldRay, &editorViewport->trackAnchor);
								if (gizmoFace >= 0)
								{
									gizmo->HiliteFace(gizmoFace, 2.0F);
									gizmoFace >>= 1;
									
									editorViewport->viewportTrackMode = kViewportTrackMoveGizmo;
									editorViewport->trackUndoData = nullptr;
									editorViewport->trackDirection = node->GetWorldTransform()[gizmoFace];
									
									trackData->trackNode = node;
									trackData->originalTransform = node->GetWorldTransform();
									trackData->gizmoIndex = gizmoFace;
									return;
								}
							}
							else
							{
								int32 gizmoEdge = gizmo->PickEdge(&trackData->worldRay, &editorViewport->trackAnchor);
								if (gizmoEdge >= 0)
								{
									gizmo->HiliteEdge(gizmoEdge, 2.0F);
									gizmoEdge >>= 2;
									
									editorViewport->viewportTrackMode = kViewportTrackRotateGizmo;
									editorViewport->trackUndoData = nullptr;
									editorViewport->trackDirection = node->GetWorldTransform()[gizmoEdge];
									editorViewport->trackCenter = node->GetWorldTransform() * gizmo->GetGizmoBox().GetCenter();
									
									trackData->trackNode = node;
									trackData->originalTransform = node->GetWorldTransform();
									trackData->gizmoIndex = gizmoEdge;
									return;
								}
							}
						}
					}
				}
			}
			
			if (tool->BeginTool(editor, trackData))
			{
				editor->SetTrackingTool(tool);
				editorViewport->viewportTrackMode = kViewportTrackUseTool;
			}
		}
	}
	else if ((eventType == kEventMouseUp) || (eventType == kEventMiddleMouseUp) || (eventType == kEventRightMouseUp))
	{
		if (editorViewport->viewportTrackMode != kViewportTrackNone)
		{
			editorViewport->viewportTrackMode = kViewportTrackNone;
			
			EditorTool *tool = editor->GetTrackingTool();
			if (tool)
			{
				tool->EndTool(editor, editor->CalculateTrackData(viewport, eventData->mousePosition));
				
				editor->SetTrackingTool(nullptr);
				editor->ClearPickFilterProc();
			}
		}
	}
	else if (eventType == kEventMouseWheel)
	{
		EditorTrackData		trackData;
		
		trackData.viewportIndex = viewport->GetViewportIndex();
		trackData.viewportType = kEditorViewportFrustum;
		trackData.viewportScale = 1.0F;
		trackData.viewportCamera = viewport->GetViewportCamera();
		
		trackData.previousViewportPosition.Set(0.0F, 0.0F);
		trackData.currentViewportPosition.Set(0.0F, eventData->mousePosition.y * -0.04F);
		
		editor->GetStandardTool(kEditorToolViewportZoom)->TrackTool(editor, &trackData);
	}
	else if ((eventType == kEventMultiaxisMouseTranslation) || (eventType == kEventMultiaxisMouseRotation))
	{
		if (eventType == kEventMultiaxisMouseTranslation)
		{
			editorViewport->multiaxisTranslationRate = eventData->mousePosition;
			if (InterfaceMgr::GetShiftKey()) editorViewport->multiaxisTranslationRate *= 5.0F;
		}
		else if (eventType == kEventMultiaxisMouseRotation)
		{
			editorViewport->multiaxisRotationRate = eventData->mousePosition;
		}
	}
}

void FrustumEditorViewport::HandleViewportTrack(const Point3D& position, ViewportWidget *viewport, void *cookie)
{
	FrustumEditorViewport *editorViewport = static_cast<FrustumEditorViewport *>(cookie);
	int32 mode = editorViewport->viewportTrackMode;
	
	if (mode == kViewportTrackUseTool)
	{
		EditorViewport *cookie = editorViewport;
		EditorViewport::HandleViewportTrack(position, viewport, cookie);
	}
	else if (mode == kViewportTrackMoveGizmo)
	{
		float	t1, t2;
		
		Editor *editor = editorViewport->GetEditor();
		const EditorTrackData *trackData = editor->CalculateTrackData(viewport, position);
		editor->AutoScroll(trackData);
		
		if (Math::CalculateNearestParameters(trackData->worldRay.origin, trackData->worldRay.direction, editorViewport->trackAnchor, editorViewport->trackDirection, &t1, &t2))
		{
			Node *trackNode = trackData->trackNode;
			const Node *trackSuper = trackNode->GetSuperNode();
			const Transform4D& worldTransform = trackSuper->GetWorldTransform();
			
			Point3D p = trackData->originalTransform.GetTranslation();
			Point3D q = editor->SnapToGrid(worldTransform * (trackSuper->GetInverseWorldTransform() * p + trackNode->GetNodeTransform()[trackData->gizmoIndex] * t2));
			Vector3D translation = q - p;
			
			if (SquaredMag(translation) > K::min_float)
			{
				MoveUndoData *undoData = editorViewport->trackUndoData;
				if (!undoData)
				{
					undoData = new MoveUndoData(editor->GetSelectionList());
					editorViewport->trackUndoData = undoData;
					editor->AddUndoData(undoData);
				}
				
				const List<NodeReference> *nodeList = undoData->GetNodeList();
				const Node *rootNode = editor->GetRootNode();
				
				const NodeReference *reference = nodeList->First();
				while (reference)
				{
					Node *node = reference->GetNode();
					EditorManipulator *manipulator = Editor::GetManipulator(node);
					if (!(manipulator->GetManipulatorFlags() & kManipulatorLockedTransform))
					{
						Node *super = node->GetSuperNode();
						if ((super) && ((super == rootNode) || (!manipulator->PredecessorSelected())))
						{
							const Transform4D& originalTransform = static_cast<const NodeTransformReference *>(reference)->GetTransform();
							node->SetNodePosition(originalTransform.GetTranslation() + super->GetInverseWorldTransform() * translation);
							manipulator->InvalidateNode();
						}
					}
					
					reference = reference->Next();
				}
				
				editor->RegenerateTexcoords(nodeList);
			}
		}
	}
	else if (mode == kViewportTrackRotateGizmo)
	{
		Editor *editor = editorViewport->GetEditor();
		const EditorTrackData *trackData = editor->CalculateTrackData(viewport, position);
		editor->AutoScroll(trackData);
		
		Antivector4D plane(editorViewport->trackDirection, editorViewport->trackAnchor);
		Vector4D position = Bivector4D(trackData->worldRay.origin, trackData->worldRay.direction) ^ plane;
		if (Fabs(position.w) > K::min_float)
		{
			Transform4D		rotation;
			
			const Vector3D& axis = editorViewport->trackDirection;
			const Point3D& center = editorViewport->trackCenter;
			
			Vector3D d1 = editorViewport->trackAnchor - center;
			Vector3D d2 = position.GetPoint3D() / position.w - center;
			d1 = (d1 - ProjectOnto(d1, axis)).Normalize();
			d2 = (d2 - ProjectOnto(d2, axis)).Normalize();
			
			float angle = Acos(d1 * d2);
			if ((d1 % d2) * axis < 0.0F) angle = -angle;
			
			if (trackData->currentModifierKeys & kModifierKeyShift)
			{
				float snap = editor->GetEditorObject()->GetSnapAngle();
				angle = Floor(angle / snap + 0.5F) * snap;
			}
			
			rotation.SetRotationAboutAxis(angle, axis);
			rotation.SetTranslation(center - rotation * center.GetVector3D());
			
			MoveUndoData *undoData = editorViewport->trackUndoData;
			if (!undoData)
			{
				undoData = new MoveUndoData(editor->GetSelectionList());
				editorViewport->trackUndoData = undoData;
				editor->AddUndoData(undoData);
			}
			
			const List<NodeReference> *nodeList = undoData->GetNodeList();
			const Node *rootNode = editor->GetRootNode();
			
			const NodeReference *reference = nodeList->First();
			while (reference)
			{
				Node *node = reference->GetNode();
				EditorManipulator *manipulator = Editor::GetManipulator(node);
				if (!(manipulator->GetManipulatorFlags() & kManipulatorLockedTransform))
				{
					Node *super = node->GetSuperNode();
					if ((super) && ((super == rootNode) || (!manipulator->PredecessorSelected())))
					{
						const Transform4D& originalTransform = static_cast<const NodeTransformReference *>(reference)->GetTransform();
						node->SetNodeTransform(super->GetInverseWorldTransform() * (rotation * (super->GetWorldTransform() * originalTransform)));
						manipulator->InvalidateNode();
					}
				}
				
				reference = reference->Next();
			}
			
			editor->RegenerateTexcoords(nodeList);
		}
	}
}

void FrustumEditorViewport::RenderViewport(List<Renderable> *renderList, ViewportWidget *viewport, void *cookie)
{
	ManipulatorRenderData	renderData[2];
	List<Renderable>		manipulatorList[2];
	List<Renderable>		geometryList[2];
	
	FrustumEditorViewport *editorViewport = static_cast<FrustumEditorViewport *>(cookie);
	Editor *editor = editorViewport->GetEditor();
	
	const EditorObject *object = editor->GetEditorObject();
	unsigned_int32 editorFlags = object->GetEditorFlags();
	
	int32 index = viewport->GetViewportIndex();
	
	renderData[0].viewportIndex = index;
	renderData[0].viewportType = kEditorViewportFrustum;
	renderData[0].viewportScale = Editor::kFrustumRenderScale;
	renderData[0].editorFlags = editorFlags;
	renderData[0].viewportCamera = viewport->GetViewportCamera();
	renderData[0].geometryList = &geometryList[0];
	renderData[0].manipulatorList = &manipulatorList[0];
	renderData[0].gizmoList = nullptr;
	renderData[0].connectorList = nullptr;
	renderData[0].handleList = nullptr;
	
	renderData[1].viewportIndex = index;
	renderData[1].viewportType = kEditorViewportFrustum;
	renderData[1].viewportScale = Editor::kFrustumRenderScale;
	renderData[1].editorFlags = editorFlags;
	renderData[1].viewportCamera = viewport->GetViewportCamera();
	renderData[1].geometryList = &geometryList[1];
	renderData[1].manipulatorList = &manipulatorList[1];
	renderData[1].gizmoList = nullptr;
	renderData[1].connectorList = nullptr;
	renderData[1].handleList = nullptr;
	
	World *world = editor->GetEditorWorld();
	unsigned_int32 worldFlags = world->GetWorldFlags();
	unsigned_int32 targetMask = TheGraphicsMgr->GetTargetDisableMask();
	
	if (!(editorFlags & kEditorRenderLighting))
	{
		world->SetWorldFlags(worldFlags | kWorldAmbientOnly);
		
		TheGraphicsMgr->SetTargetDisableMask(targetMask | ((1 << kRenderTargetReflection) | (1 << kRenderTargetRefraction)));
		TheGraphicsMgr->SetAmbientMode(kAmbientBright);
	}
	
	FrustumCamera *camera = static_cast<FrustumViewportWidget *>(viewport)->GetViewportCamera();
	world->SetCamera(camera);
	world->Update();
	
	world->SetRenderSize((int32) viewport->GetWidgetSize().x, (int32) viewport->GetWidgetSize().y);
	world->BeginRendering();
	world->Render();
	
	editorViewport->RenderNode(editor->GetRootNode(), renderData);
	
	unsigned_int32 wireFlags = kWireframeColor;
	if (editor->GetEditorObject()->GetEditorFlags() & kEditorShowBackfaces) wireFlags |= kWireframeTwoSided;
	
	TheGraphicsMgr->DrawRenderList(&manipulatorList[1]);
	TheGraphicsMgr->DrawWireframe(wireFlags, &geometryList[0]);
	TheGraphicsMgr->DrawRenderList(&manipulatorList[0]);
	
	world->EndRendering();
	camera->GetObject()->SetClearFlags(0);
	
	if (!(editorFlags & kEditorRenderLighting))
	{
		world->SetWorldFlags(worldFlags);
		
		TheGraphicsMgr->SetTargetDisableMask(targetMask);
		TheGraphicsMgr->SetAmbientMode(kAmbientNormal);
	}
	
	geometryList[1].RemoveAll();
	manipulatorList[1].RemoveAll();
	geometryList[0].RemoveAll();
	manipulatorList[0].RemoveAll();
}

void FrustumEditorViewport::RenderOverlay(List<Renderable> *renderList, ViewportWidget *viewport, void *cookie)
{
	FrustumEditorViewport *editorViewport = static_cast<FrustumEditorViewport *>(cookie);
	Editor *editor = editorViewport->GetEditor();
	
	const NodeReference *reference = editor->GetGizmoTarget();
	if (reference)
	{
		ManipulatorRenderData	renderData;
		
		renderData.viewportIndex = viewport->GetViewportIndex();
		renderData.viewportType = kEditorViewportFrustum;
		renderData.viewportScale = Editor::kFrustumRenderScale * 1280.0F / viewport->GetWidgetSize().x;
		renderData.editorFlags = editor->GetEditorObject()->GetEditorFlags();
		renderData.viewportCamera = viewport->GetViewportCamera();
		renderData.geometryList = nullptr;
		renderData.manipulatorList = nullptr;
		renderData.gizmoList = renderList;
		renderData.connectorList = nullptr;
		renderData.handleList = nullptr;
		
		Editor::GetManipulator(reference->GetNode())->GetGizmo()->Render(&renderData);
	}
}


GraphEditorViewport::GraphEditorViewport(Editor *editor, const Vector2D& scale) :
		EditorViewport(kEditorViewportGraph, editor, kViewportModeGraph, &graphViewport),
		graphViewport(Vector2D(1.0F, 1.0F), scale)
{
	Postconstruct();
	
	graphViewport.SetMouseEventProc(&HandleViewportMouseEvent, this);
	graphViewport.SetRenderProc(&RenderViewport, this);
	
	OrthoCameraObject *object = graphViewport.GetViewportCamera()->GetObject();
	object->SetClearFlags(kClearColorBuffer | kClearDepthBuffer | kClearStencilBuffer);
	object->SetClearColor(TheWorldEditor->GetSceneGraphColor());
	object->SetNearDepth(-1.0F);
	object->SetFarDepth(1.0F);
}

GraphEditorViewport::~GraphEditorViewport()
{
	Predestruct();
}

void GraphEditorViewport::HandleViewportMouseEvent(const MouseEventData *eventData, ViewportWidget *viewport, void *cookie)
{
	GraphEditorViewport *editorViewport = static_cast<GraphEditorViewport *>(cookie);
	Editor *editor = editorViewport->GetEditor();
	
	EventType eventType = eventData->eventType;
	if ((eventType == kEventMouseDown) || (eventType == kEventMiddleMouseDown) || (eventType == kEventRightMouseDown))
	{
		editor->SetFocusWidget(nullptr);
		
		if (!(editor->GetEditorState() & kEditorWaitUpdate))
		{
			if (eventType == kEventRightMouseDown)
			{
				editor->ActivateViewportMenu(viewport->GetViewportIndex(), eventData->mousePosition);
				return;
			}
			
			unsigned_int32 modifierKeys = InterfaceMgr::GetModifierKeys();
			EditorTool *tool = editor->GetCurrentTool();
			
			if ((eventType == kEventMiddleMouseDown) || (modifierKeys & kModifierKeyOption))
			{
				tool = editor->GetStandardTool(kEditorToolViewportScroll);
			}
			else if (modifierKeys & kModifierKeyCommand)
			{
				tool = editor->GetStandardTool(kEditorToolOrbitCamera);
			}
			
			EditorTrackData *trackData = editor->GetTrackData();
			
			trackData->viewportIndex = viewport->GetViewportIndex();
			trackData->viewportType = kEditorViewportGraph;
			trackData->viewportScale = static_cast<OrthoViewportWidget *>(viewport)->GetOrthoScale().x;
			trackData->editorFlags = editor->GetEditorObject()->GetEditorFlags();
			trackData->viewportCamera = viewport->GetViewportCamera();
			
			trackData->currentPosition.Set(0.0F, 0.0F);
			trackData->snappedCurrentPosition.Set(0.0F, 0.0F);
			trackData->currentViewportPosition.Set(0.0F, 0.0F);
			trackData->currentPickPoint.Set(0.0F, 0.0F, 0.0F);
			trackData->currentPickNormal.Set(0.0F, 0.0F, 1.0F);
			trackData->mouseEventFlags = eventData->eventFlags;
			trackData->currentModifierKeys = modifierKeys;
			
			editor->CalculateTrackData(viewport, eventData->mousePosition);
			trackData->anchorPosition = trackData->currentPosition;
			trackData->snappedAnchorPosition = trackData->snappedCurrentPosition;
			trackData->anchorViewportPosition = trackData->currentViewportPosition;
			trackData->currentSize.Set(0.0F, 0.0F);
			
			editor->ResetScrollFraction();
			trackData->trackType = kEditorTrackNode;
			trackData->trackNode = nullptr;
			trackData->superNode = nullptr;
			trackData->gizmo = nullptr;
			
			Node *node = Editor::GetManipulator(editor->GetRootNode())->PickGraphNode(trackData, &trackData->worldRay);
			if ((node) && (!(Editor::GetManipulator(node)->GetManipulatorFlags() & kManipulatorLockedSubtree))) trackData->superNode = node;
			
			if (tool->BeginTool(editor, trackData)) editor->SetTrackingTool(tool);
		}
	}
	else if ((eventType == kEventMouseUp) || (eventType == kEventMiddleMouseUp) || (eventType == kEventRightMouseUp))
	{
		EditorTool *tool = editor->GetTrackingTool();
		if (tool)
		{
			tool->EndTool(editor, editor->CalculateTrackData(viewport, eventData->mousePosition));
			
			editor->SetTrackingTool(nullptr);
			editor->ClearPickFilterProc();
		}
	}
	else if (eventType == kEventMouseWheel)
	{
		EditorTrackData		trackData;
		
		trackData.viewportIndex = viewport->GetViewportIndex();
		trackData.viewportType = kEditorViewportGraph;
		trackData.viewportScale = static_cast<OrthoViewportWidget *>(viewport)->GetOrthoScale().x;
		trackData.viewportCamera = viewport->GetViewportCamera();
		
		trackData.previousViewportPosition.Set(0.0F, 0.0F);
		trackData.currentViewportPosition.Set(0.0F, eventData->mousePosition.y * -0.04F);
		
		editor->GetStandardTool(kEditorToolViewportZoom)->TrackTool(editor, &trackData);
	}
}

void GraphEditorViewport::RenderViewport(List<Renderable> *renderList, ViewportWidget *viewport, void *cookie)
{
	ManipulatorViewportData		viewportData;
	
	GraphEditorViewport *editorViewport = static_cast<GraphEditorViewport *>(cookie);
	Editor *editor = editorViewport->GetEditor();
	
	EditorManipulator *manipulator = Editor::GetManipulator(editor->GetRootNode());
	manipulator->UpdateGraph();
	
	int32 index = viewport->GetViewportIndex();
	
	viewportData.viewportIndex = index;
	viewportData.viewportType = kEditorViewportGraph;
	viewportData.viewportScale = static_cast<OrthoViewportWidget *>(viewport)->GetOrthoScale().x;
	viewportData.viewportCamera = viewport->GetViewportCamera();
	
	manipulator->RenderGraph(&viewportData, renderList);
	
	Renderable *renderable = editorViewport->GetToolRenderable();
	if (renderable)
	{
		editorViewport->SetToolRenderable(nullptr);
		renderList->Append(renderable);
	}
}

// ZYURVUR
