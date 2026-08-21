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


#include "C4Water.h"
#include "C4EditorTools.h"
#include "C4EditorSupport.h"
#include "C4WorldEditor.h"
#include "C4GeometryManipulators.h"
#include "C4MarkerManipulators.h"


using namespace C4;


namespace
{
	const float reparentColor[80] =
	{
		0.5F, 0.5F, 0.5F, 0.75F,
		0.5F, 0.5F, 0.5F, 0.75F,
		0.5F, 0.5F, 0.5F, 0.75F,
		0.5F, 0.5F, 0.5F, 0.75F,
		0.0F, 0.0F, 0.0F, 0.75F,
		0.0F, 0.0F, 0.0F, 0.75F,
		0.0F, 0.0F, 0.0F, 0.75F,
		0.0F, 0.0F, 0.0F, 0.75F,
		0.0F, 0.0F, 0.0F, 0.75F,
		0.0F, 0.0F, 0.0F, 0.75F,
		0.0F, 0.0F, 0.0F, 0.75F,
		0.0F, 0.0F, 0.0F, 0.75F,
		0.0F, 0.0F, 0.0F, 1.0F,
		0.0F, 0.0F, 0.0F, 1.0F,
		0.0F, 0.0F, 0.0F, 1.0F,
		0.0F, 0.0F, 0.0F, 1.0F,
		0.0F, 0.0F, 0.0F, 1.0F,
		0.0F, 0.0F, 0.0F, 1.0F,
		0.0F, 0.0F, 0.0F, 1.0F,
		0.0F, 0.0F, 0.0F, 1.0F
	};
	
	
	const Triangle reparentTriangle[18] =
	{
		{{12, 15, 16}},
		{{16, 15, 19}},
		{{13, 12, 17}},
		{{17, 12, 16}},
		{{14, 13, 18}},
		{{18, 13, 17}},
		{{15, 14, 19}},
		{{19, 14, 18}},
		
		{{ 0,  1,  2}},
		{{ 0,  2,  3}},
		{{ 4,  7,  8}},
		{{ 8,  7, 11}},
		{{ 5,  4,  9}},
		{{ 9,  4,  8}},
		{{ 6,  5, 10}},
		{{10,  5,  9}},
		{{ 7,  6, 11}},
		{{11,  6, 10}}
	};
}


EditorTool::EditorTool()
{
}

EditorTool::~EditorTool()
{
}

void EditorTool::Engage(Editor *editor, void *cookie)
{
}

void EditorTool::Disengage(Editor *editor, void *cookie)
{
}

bool EditorTool::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	return (false);
}

bool EditorTool::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	return (false);
}

bool EditorTool::EndTool(Editor *editor, EditorTrackData *trackData)
{
	return (false);
}


StandardEditorTool::StandardEditorTool(IconButtonWidget *widget) : toolObserver(this, &StandardEditorTool::HandleToolButtonEvent)
{
	toolButton = widget;
	widget->SetObserver(&toolObserver);
}

StandardEditorTool::~StandardEditorTool() 
{
} 
 
void StandardEditorTool::HandleToolButtonEvent(Widget *widget, const WidgetEventData *eventData) 
{
	Editor *editor = static_cast<Editor *>(widget->GetOwningWindow()); 
	editor->SetCurrentTool(this);
	editor->SetFocusWidget(nullptr);
}
 
void StandardEditorTool::Engage(Editor *editor, void *cookie)
{
	toolButton->SetValue(1);
} 

void StandardEditorTool::Disengage(Editor *editor, void *cookie)
{
	toolButton->SetValue(0);
}

bool StandardEditorTool::SelectNode(Editor *editor, EditorTrackData *trackData)
{
	PickData	pickData;
	
	Node *node = nullptr;
	bool getInfo = false;
	
	if (trackData->viewportType != kEditorViewportGraph)
	{
		node = editor->PickNode(trackData, &pickData, 0U);
	}
	else
	{
		Widget *widget = nullptr;
		node = Editor::GetManipulator(editor->GetRootNode())->PickGraphNode(trackData, &trackData->worldRay, &widget);
		if ((!node) && (widget))
		{
			widget->Activate();
			editor->GetViewport(trackData->viewportIndex)->GetViewportWidget()->InvalidateTexture();
			return (true);
		}
		
		getInfo = ((trackData->mouseEventFlags & kMouseDoubleClick) != 0);
		pickData.pickIndex[0] = -1;
	}
	
	if (!(editor->GetEditorState() & kEditorSelectionLocked))
	{
		bool shift = ((trackData->currentModifierKeys & kModifierKeyShift) != 0);
		if (node)
		{
			trackData->trackNode = node;
			trackData->originalTransform = node->GetWorldTransform();
			
			if (node->GetNodeType() == kNodeMarker)
			{
				Marker *marker = static_cast<Marker *>(node);
				if (marker->GetMarkerType() == kMarkerPath)
				{
					int32 pointIndex1 = pickData.pickIndex[0];
					int32 pointIndex2 = pickData.pickIndex[1];
					if (pointIndex1 >= 0)
					{
						PathManipulator *manipulator = static_cast<PathManipulator *>(marker->GetManipulator());
						
						bool tangentFlag = ((trackData->currentModifierKeys & kModifierKeyCommand) != 0);
						if (shift)
						{
							if (manipulator->ControlPointSelected(pointIndex1))
							{
								manipulator->UnselectControlPoint(pointIndex1, tangentFlag);
								manipulator->UnselectControlPoint(pointIndex2, tangentFlag);
							}
							else
							{
								manipulator->SelectControlPoint(pointIndex1, tangentFlag);
								manipulator->SelectControlPoint(pointIndex2, tangentFlag);
							}
						}
						else
						{
							if (!manipulator->ControlPointSelected(pointIndex1))
							{
								editor->UnselectAll();
								editor->SelectNode(marker);
								manipulator->SelectControlPoint(pointIndex1, tangentFlag);
								manipulator->SelectControlPoint(pointIndex2, tangentFlag);
							}
						}
						
						trackData->trackType = kEditorTrackVertex;
						return (false);
					}
				}
			}
			
			if (node->GetManipulator()->Selected())
			{
				if (shift)
				{
					trackData->trackNode = nullptr;
					editor->UnselectNode(node);
					getInfo = false;
				}
			}
			else
			{
				if (!shift) editor->UnselectAll();
				editor->SelectNode(node);
			}
			
			if (getInfo) editor->OpenNodeInfo();
		}
		else
		{
			if (!shift) editor->UnselectAll();
		}
	}
	else
	{
		if ((node) && (node->GetManipulator()->Selected()))
		{
			trackData->trackNode = node;
			trackData->originalTransform = node->GetWorldTransform();
		}
	}
	
	return (false);
}


NodeSelectTool::NodeSelectTool(IconButtonWidget *widget) : StandardEditorTool(widget)
{
}

NodeSelectTool::~NodeSelectTool()
{
}

bool NodeSelectTool::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	SelectNode(editor, trackData);
	return (false);
}


NodeMoveTool::NodeMoveTool(IconButtonWidget *widget) :
		StandardEditorTool(widget),
		reparentRenderable(kRenderIndexedTriangles)
{
	reparentRenderable.SetVertexCount(20);
	reparentRenderable.SetAmbientBlendState(kBlendInterpolate);
	reparentRenderable.SetAttributeArray(kArrayVertex, reparentVertex);
	reparentRenderable.SetAttributeArray(kArrayColor0, reparentColor, 4);
}

NodeMoveTool::~NodeMoveTool()
{
}

void NodeMoveTool::CalculateReparentVertices(const Point2D& position)
{
	float x = position.x - 24.0F;
	float y = position.y - 6.0F;
	
	reparentVertex[0].Set(x, y, 0.0F);
	reparentVertex[1].Set(x, y + 12.0F, 0.0F);
	reparentVertex[2].Set(x + 48.0F, y + 12.0F, 0.0F);
	reparentVertex[3].Set(x + 48.0F, y, 0.0F);
	reparentVertex[4].Set(x, y, 0.0F);
	reparentVertex[5].Set(x, y + 12.0F, 0.0F);
	reparentVertex[6].Set(x + 48.0F, y + 12.0F, 0.0F);
	reparentVertex[7].Set(x + 48.0F, y, 0.0F);
	reparentVertex[8].Set(x - 1.0F, y - 1.0F, 0.0F);
	reparentVertex[9].Set(x - 1.0F, y + 13.0F, 0.0F);
	reparentVertex[10].Set(x + 49.0F, y + 13.0F, 0.0F);
	reparentVertex[11].Set(x + 49.0F, y - 1.0F, 0.0F);
}

EditorManipulator *NodeMoveTool::GetReparentNode(Editor *editor, EditorTrackData *trackData)
{
	const Node *node = Editor::GetManipulator(editor->GetRootNode())->PickGraphNode(trackData, &trackData->worldRay);
	if (node)
	{
		NodeType type = node->GetNodeType();
		if ((type == kNodeTerrainBlock) || (type == kNodeWaterBlock)) return (nullptr);
		
		if (type == kNodeGeometry)
		{
			GeometryType geometryType = static_cast<const Geometry *>(node)->GetGeometryType();
			if ((geometryType == kGeometryTerrain) || (geometryType == kGeometryWater) || (geometryType == kGeometryHorizonWater)) return (nullptr);
		}
		
		if ((type == kNodeInstance) || (type == kNodeModel)) return (nullptr);
		
		EditorManipulator *manipulator = Editor::GetManipulator(node);
		if (!manipulator->Selected()) return (manipulator);
	}
	
	return (nullptr);
}

bool NodeMoveTool::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	Node *node = editor->PickGizmoMover(trackData);
	if (node)
	{
		if (Editor::GetManipulator(node)->GetManipulatorFlags() & kManipulatorLockedTransform) return (false);
		
		trackData->trackNode = node;
		trackData->originalTransform = node->GetWorldTransform();
		trackData->gizmo->HiliteMovers(1 << trackData->gizmoIndex);
		
		undoDataFlag = false;
		editor->InvalidateAllViewports();
		return (true);
	}
	
	bool collapse = SelectNode(editor, trackData);
	
	if (trackData->viewportType != kEditorViewportFrustum)
	{
		node = trackData->trackNode;
		if ((node) && (!(Editor::GetManipulator(node)->GetManipulatorFlags() & kManipulatorLockedTransform)))
		{
			if (trackData->viewportType != kEditorViewportGraph)
			{
				undoDataFlag = false;
				return (true);
			}
			else
			{
				if ((!collapse) && (editor->GetFirstSelectedNode()))
				{
					reparentRenderable.SetFaceCount(10);
					reparentRenderable.SetTriangleArray(reparentTriangle + 8);
					
					CalculateReparentVertices(trackData->anchorPosition);
					
					EditorViewport *viewport = editor->GetViewport(trackData->viewportIndex);
					viewport->SetToolRenderable(&reparentRenderable);
					viewport->GetViewportWidget()->InvalidateTexture();
					return (true);
				}
			}
		}
	}
	
	return (false);
}

bool NodeMoveTool::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	if (trackData->viewportType != kEditorViewportGraph)
	{
		if (trackData->currentModifierKeys & kModifierKeyShift)
		{
			float dx = trackData->currentPosition.x - trackData->anchorPosition.x;
			float dy = trackData->currentPosition.y - trackData->anchorPosition.y;
			
			if (Fabs(dy) > Fabs(dx)) trackData->currentPosition.x = trackData->anchorPosition.x;
			else trackData->currentPosition.y = trackData->anchorPosition.y;
		}
		
		if (trackData->currentPosition != trackData->previousPosition)
		{
			if (!undoDataFlag)
			{
				undoDataFlag = true;
				editor->AddUndoData(new MoveUndoData(editor->GetSelectionList()));
			}
			
			Vector3D delta = Editor::GetWorldSpaceDirection(trackData, trackData->currentPosition - trackData->anchorPosition);
			const Point3D& originalPosition = trackData->originalTransform.GetTranslation();
			delta = editor->SnapToGrid(originalPosition + delta) - trackData->trackNode->GetWorldPosition();
			
			const EditorGizmo *gizmo = trackData->gizmo;
			if (gizmo)
			{
				const Transform4D& transform = gizmo->GetTransformable()->GetWorldTransform();
				delta = ProjectOnto(delta, transform[trackData->gizmoIndex]);
			}
			
			const NodeReference *reference = editor->GetFirstSelectedNode();
			while (reference)
			{
				Node *node = reference->GetNode();
				
				if (trackData->trackType == kEditorTrackVertex)
				{
					if (node->GetNodeType() == kNodeMarker)
					{
						const Marker *marker = static_cast<Marker *>(node);
						if (marker->GetMarkerType() == kMarkerPath)
						{
							EditorManipulator *manipulator = Editor::GetManipulator(node);
							if (manipulator->GetSelectionType() == kEditorSelectionVertex)
							{
								Vector3D dp = Editor::GetWorldSpaceDirection(trackData, trackData->snappedCurrentPosition - trackData->snappedPreviousPosition);
								static_cast<PathManipulator *>(manipulator)->MoveSelectedControlPoints(node->GetInverseWorldTransform() * dp, ((trackData->currentModifierKeys & kModifierKeyCommand) == 0));
							}
						}
					}
				}
				else
				{
					EditorManipulator *manipulator = Editor::GetManipulator(node);
					if (!(manipulator->GetManipulatorFlags() & kManipulatorLockedTransform))
					{
						Node *super = node->GetSuperNode();
						if ((super) && ((super == editor->GetRootNode()) || (!manipulator->PredecessorSelected())))
						{
							node->SetNodePosition(node->GetNodeTransform() * (Zero3D + node->GetInverseWorldTransform() * delta));
							manipulator->InvalidateNode();
						}
					}
				}
				
				reference = reference->Next();
			}
			
			editor->RegenerateTexcoords(editor->GetSelectionList());
		}
	}
	else
	{
		int32 count = 10;
		const Triangle *triangle = reparentTriangle + 8;
		
		EditorManipulator *manipulator = GetReparentNode(editor, trackData);
		if (manipulator)
		{
			Box2D box = manipulator->GetGraphBox();
			
			reparentVertex[12].Set(box.min.x - 3.0F, box.min.y - 3.0F, 0.0F);
			reparentVertex[13].Set(box.min.x - 3.0F, box.max.y + 3.0F, 0.0F);
			reparentVertex[14].Set(box.max.x + 3.0F, box.max.y + 3.0F, 0.0F);
			reparentVertex[15].Set(box.max.x + 3.0F, box.min.y - 3.0F, 0.0F);
			reparentVertex[16].Set(box.min.x - 5.0F, box.min.y - 5.0F, 0.0F);
			reparentVertex[17].Set(box.min.x - 5.0F, box.max.y + 5.0F, 0.0F);
			reparentVertex[18].Set(box.max.x + 5.0F, box.max.y + 5.0F, 0.0F);
			reparentVertex[19].Set(box.max.x + 5.0F, box.min.y - 5.0F, 0.0F);
			
			count = 18;
			triangle = reparentTriangle;
		}
		
		reparentRenderable.SetFaceCount(count);
		reparentRenderable.SetTriangleArray(triangle);
		
		CalculateReparentVertices(trackData->currentPosition);
		
		EditorViewport *viewport = editor->GetViewport(trackData->viewportIndex);
		viewport->SetToolRenderable(&reparentRenderable);
		viewport->GetViewportWidget()->InvalidateTexture();
	}
	
	return (true);
}

bool NodeMoveTool::EndTool(Editor *editor, EditorTrackData *trackData)
{
	EditorGizmo *gizmo = trackData->gizmo;
	if (gizmo) gizmo->HiliteMovers(0);
	
	if (trackData->viewportType != kEditorViewportGraph)
	{
		editor->InvalidateAllViewports();
		return (NodeMoveTool::TrackTool(editor, trackData));
	}
	
	EditorManipulator *manipulator = GetReparentNode(editor, trackData);
	if (manipulator) editor->ReparentSelectedNodes(manipulator->GetTargetNode());
	else editor->GetViewport(trackData->viewportIndex)->GetViewportWidget()->InvalidateTexture();
	
	return (true);
}


NodeRotateTool::NodeRotateTool(IconButtonWidget *widget) : StandardEditorTool(widget)
{
}

NodeRotateTool::~NodeRotateTool()
{
}

void NodeRotateTool::Engage(Editor *editor, void *cookie)
{
	StandardEditorTool::Engage(editor, cookie);
	
	editor->SetRenderFlags(editor->GetRenderFlags() | kEditorRenderHandles);
}

void NodeRotateTool::Disengage(Editor *editor, void *cookie)
{
	StandardEditorTool::Disengage(editor, cookie);
	
	editor->SetRenderFlags(editor->GetRenderFlags() & ~kEditorRenderHandles);
}

bool NodeRotateTool::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	Node *node = editor->PickGizmoRotator(trackData);
	if (node)
	{
		if (Editor::GetManipulator(node)->GetManipulatorFlags() & kManipulatorLockedTransform) return (false);
		
		trackData->trackNode = node;
		rotationCenter = node->GetWorldPosition();
		trackData->gizmo->HiliteRotators(1 << trackData->gizmoIndex);
		
		const Vector3D& axis = trackData->gizmo->GetTransformable()->GetWorldTransform()[trackData->gizmoIndex];
		negateAngle = (axis * trackData->viewportCamera->GetWorldTransform()[2] < 0.0F);
		
		undoDataFlag = false;
		accumAngle = 0.0F;
		
		editor->InvalidateAllViewports();
		return (true);
	}
	
	if (trackData->viewportType == kEditorViewportOrtho)
	{
		int32	handleIndex;
		
		node = editor->PickHandle(trackData, &handleIndex);
		if (node)
		{
			ManipulatorHandleData	handleData;
			
			const EditorManipulator *manipulator = Editor::GetManipulator(node);
			if (manipulator->GetManipulatorFlags() & kManipulatorLockedTransform) return (false);
			
			if (editor->GetEditorObject()->GetEditorFlags() & kEditorDrawFromCenter)
			{
				rotationCenter = manipulator->GetNodeSphere()->GetCenter();
			}
			else
			{
				manipulator->GetHandleData(handleIndex, &handleData);
				
				int32 index = handleData.oppositeIndex;
				if (index == kHandleOrigin) rotationCenter = node->GetWorldPosition();
				else rotationCenter = node->GetWorldTransform() * manipulator->GetHandlePosition(index);
			}
			
			undoDataFlag = false;
			accumAngle = 0.0F;
			
			editor->InvalidateAllViewports();
			return (true);
		}
	}
	
	SelectNode(editor, trackData);
	return (false);
}

bool NodeRotateTool::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	const Camera *camera = trackData->viewportCamera;
	Vector3D center = camera->GetInverseWorldTransform() * (rotationCenter + camera->GetWorldPosition());
	
	Vector2D v1(trackData->anchorPosition.x - center.x, trackData->anchorPosition.y - center.y);
	Vector2D v2(trackData->currentPosition.x - center.x, trackData->currentPosition.y - center.y);
	
	float angle = Acos(v1 * v2 * InverseSqrt(SquaredMag(v1) * SquaredMag(v2)));
	if (v1.x * v2.y - v1.y * v2.x < 0.0F) angle = -angle;
	
	if (trackData->currentModifierKeys & kModifierKeyShift)
	{
		float snap = editor->GetEditorObject()->GetSnapAngle();
		angle = Floor(angle / snap + 0.5F) * snap;
	}
	
	float accum = accumAngle;
	if (angle != accum)
	{
		Matrix3D	rotation;
		
		if (!undoDataFlag)
		{
			undoDataFlag = true;
			editor->AddUndoData(new MoveUndoData(editor->GetSelectionList()));
		}
		
		accumAngle = angle;
		float delta = angle - accum;
		
		const EditorGizmo *gizmo = trackData->gizmo;
		if (gizmo)
		{
			if (negateAngle) delta = -delta;
			rotation.SetRotationAboutAxis(delta, gizmo->GetTransformable()->GetWorldTransform()[trackData->gizmoIndex]);
		}
		else
		{
			rotation.SetRotationAboutAxis(delta, camera->GetWorldTransform()[2]);
		}
		
		Transform4D centralRotation(rotation, rotationCenter - rotation * rotationCenter);
		
		const NodeReference *reference = editor->GetFirstSelectedNode();
		while (reference)
		{
			Node *node = reference->GetNode();
			EditorManipulator *manipulator = Editor::GetManipulator(node);
			if (!(manipulator->GetManipulatorFlags() & kManipulatorLockedTransform))
			{
				Node *super = node->GetSuperNode();
				if ((super) && ((super == editor->GetRootNode()) || (!manipulator->PredecessorSelected())))
				{
					Transform4D transform = node->GetNodeTransform() * node->GetInverseWorldTransform() * centralRotation * node->GetWorldTransform();
					node->SetNodeTransform(transform.Normalize());
					manipulator->InvalidateNode();
				}
			}
			
			reference = reference->Next();
		}
		
		editor->RegenerateTexcoords(editor->GetSelectionList());
	}
	
	return (true);
}

bool NodeRotateTool::EndTool(Editor *editor, EditorTrackData *trackData)
{
	EditorGizmo *gizmo = trackData->gizmo;
	if (gizmo) gizmo->HiliteRotators(0);
	
	editor->InvalidateAllViewports();
	return (NodeRotateTool::TrackTool(editor, trackData));
}


NodeScaleTool::NodeScaleTool(IconButtonWidget *widget) : StandardEditorTool(widget)
{
}

NodeScaleTool::~NodeScaleTool()
{
}

void NodeScaleTool::Engage(Editor *editor, void *cookie)
{
	StandardEditorTool::Engage(editor, cookie);
	
	editor->SetRenderFlags(editor->GetRenderFlags() | kEditorRenderHandles);
}

void NodeScaleTool::Disengage(Editor *editor, void *cookie)
{
	StandardEditorTool::Disengage(editor, cookie);
	
	editor->SetRenderFlags(editor->GetRenderFlags() & ~kEditorRenderHandles);
}

bool NodeScaleTool::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (trackData->viewportType == kEditorViewportOrtho)
	{
		int32	handleIndex;
		
		Node *node = editor->PickHandle(trackData, &handleIndex);
		if (node)
		{
			ManipulatorHandleData	handleData;
			
			const EditorManipulator *manipulator = Editor::GetManipulator(node);
			if ((manipulator->GetManipulatorFlags() & kManipulatorLockedTransform) && (node->GetSuperNode())) return (false);
			
			manipulator->GetHandleData(handleIndex, &handleData);
			trackData->resizeData.resizeFlags = (editor->GetEditorObject()->GetEditorFlags() & kEditorDrawFromCenter) ? kManipulatorResizeCenter : 0;
			trackData->resizeData.handleFlags = handleData.handleFlags;
			trackData->resizeData.handleIndex = handleIndex;
			
			NodeReference *reference = editor->GetFirstSelectedNode();
			while (reference)
			{
				Editor::GetManipulator(reference->GetNode())->BeginResize(&trackData->resizeData);
				reference = reference->Next();
			}
			
			undoDataFlag = false;
			return (true);
		}
	}
	
	SelectNode(editor, trackData);
	return (false);
}

bool NodeScaleTool::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	bool currentShift = ((trackData->currentModifierKeys & kModifierKeyShift) != 0);
	bool previousShift = ((trackData->previousModifierKeys & kModifierKeyShift) != 0);
	
	if ((trackData->currentPosition != trackData->previousPosition) || (currentShift != previousShift))
	{
		if (!undoDataFlag)
		{
			undoDataFlag = true;
			editor->AddUndoData(new ResizeUndoData(editor->GetSelectionList()));
		}
		
		if (currentShift) trackData->resizeData.resizeFlags |= kManipulatorResizeConstrain;
		else trackData->resizeData.resizeFlags &= ~kManipulatorResizeConstrain;
		
		Vector3D delta = Editor::GetWorldSpaceDirection(trackData, trackData->snappedCurrentPosition - trackData->snappedAnchorPosition);
		
		const NodeReference *reference = editor->GetFirstSelectedNode();
		while (reference)
		{
			Node *node = reference->GetNode();
			if (node->GetNodeType() != kNodeGroup)
			{
				EditorManipulator *manipulator = Editor::GetManipulator(node);
				if ((!(manipulator->GetManipulatorFlags() & kManipulatorLockedTransform)) || (!node->GetSuperNode()))
				{
					trackData->resizeData.resizeDelta = node->GetInverseWorldTransform() * delta;
					trackData->resizeData.positionOffset.Set(0.0F, 0.0F, 0.0F);
					
					bool move = manipulator->Resize(&trackData->resizeData);
					node->Invalidate();
					
					if (move)
					{
						Point3D compensator = node->GetNodePosition();
						node->SetNodePosition(manipulator->GetOriginalPosition() + node->GetNodeTransform() * trackData->resizeData.positionOffset);
						
						Node *subnode = node->GetFirstSubnode();
						if (subnode)
						{
							compensator = Inverse(node->GetNodeTransform()) * compensator;
							do
							{
								subnode->SetNodePosition(compensator + subnode->GetNodePosition());
								subnode->Invalidate();
								
								subnode = subnode->Next();
							} while (subnode);
						}
					}
				}
			}
			
			reference = reference->Next();
		}
		
		editor->RebuildGeometry(editor->GetSelectionList());
	}
	
	return (true);
}

bool NodeScaleTool::EndTool(Editor *editor, EditorTrackData *trackData)
{
	return (NodeScaleTool::TrackTool(editor, trackData));
}


ConnectTool::ConnectTool(IconButtonWidget *widget) : StandardEditorTool(widget)
{
}

ConnectTool::~ConnectTool()
{
}

void ConnectTool::Engage(Editor *editor, void *cookie)
{
	StandardEditorTool::Engage(editor, cookie);
	
	editor->SetRenderFlags(editor->GetRenderFlags() | kEditorRenderConnectors);
}

void ConnectTool::Disengage(Editor *editor, void *cookie)
{
	StandardEditorTool::Disengage(editor, cookie);
	
	editor->UnselectAllConnectors();
	editor->SetRenderFlags(editor->GetRenderFlags() & ~kEditorRenderConnectors);
}

bool ConnectTool::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (trackData->viewportType == kEditorViewportOrtho)
	{
		int32	index;
		
		Node *node = editor->PickConnector(trackData, &index);
		if (node)
		{
			bool shift = ((trackData->currentModifierKeys & kModifierKeyShift) != 0);
			if (!shift) editor->UnselectAllConnectors(node);
			
			editor->SelectConnector(node, index, shift);
			return (false);
		}
	}
	
	SelectNode(editor, trackData);
	return (false);
}


SurfaceSelectTool::SurfaceSelectTool(IconButtonWidget *widget) : StandardEditorTool(widget)
{
}

SurfaceSelectTool::~SurfaceSelectTool()
{
}

void SurfaceSelectTool::Engage(Editor *editor, void *cookie)
{
	StandardEditorTool::Engage(editor, cookie);
}

void SurfaceSelectTool::Disengage(Editor *editor, void *cookie)
{
	StandardEditorTool::Disengage(editor, cookie);
}

bool SurfaceSelectTool::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (trackData->viewportType != kEditorViewportGraph)
	{
		PickData	pickData;
		
		bool shift = ((trackData->currentModifierKeys & kModifierKeyShift) != 0);
		Node *node = editor->PickNode(trackData, &pickData, 1 << kEditorNodeGeometry);
		if ((node) && (pickData.triangleIndex != kInvalidTriangleIndex))
		{
			const GeometryObject *object = static_cast<Geometry *>(node)->GetObject();
			const GeometryLevel *level = object->GetGeometryLevel(0);
			const Triangle *triangle = level->GetArray<Triangle>(kArrayFace);
			const unsigned_int16 *surfaceIndex = level->GetArray<unsigned_int16>(kArraySurfaceIndex);
			int32 selectIndex = (surfaceIndex) ? surfaceIndex[triangle[pickData.triangleIndex].index[0]] : 0;
			
			GeometryManipulator *manipulator = static_cast<GeometryManipulator *>(node->GetManipulator());
			
			if (shift)
			{
				if (!manipulator->Selected()) editor->SelectNode(node);
				
				if (manipulator->SurfaceSelected(selectIndex))
				{
					manipulator->UnselectSurface(selectIndex);
					if (manipulator->GetSelectionType() == kEditorSelectionObject) editor->UnselectNode(node);
				}
				else
				{
					manipulator->SelectSurface(selectIndex);
				}
				
				editor->InvalidateSelection();
			}
			else
			{
				editor->UnselectAll();
				editor->SelectNode(node);
				manipulator->SelectSurface(selectIndex);
			}
		}
		else
		{
			if (!shift) editor->UnselectAll();
		}
	}
	
	return (false);
}


ViewportScrollTool::ViewportScrollTool(IconButtonWidget *widget) : StandardEditorTool(widget)
{
}

ViewportScrollTool::~ViewportScrollTool()
{
}

void ViewportScrollTool::Engage(Editor *editor, void *cookie)
{
	StandardEditorTool::Engage(editor, cookie);
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorHand));
}

bool ViewportScrollTool::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	initalCameraPosition = trackData->viewportCamera->GetNodePosition();
	
	previousCursor = editor->GetCurrentCursor();
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorDrag));
	return (true);
}

bool ViewportScrollTool::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	float dx = trackData->anchorViewportPosition.x - trackData->currentViewportPosition.x;
	float dy = trackData->anchorViewportPosition.y - trackData->currentViewportPosition.y;
	
	if ((dx != 0.0F) || (dy != 0.0F))
	{
		Camera *camera = trackData->viewportCamera;
		const Vector3D& right = camera->GetNodeTransform()[0];
		const Vector3D& down = camera->GetNodeTransform()[1];
		
		if (camera->GetCameraType() == kCameraOrtho)
		{
			const OrthoCameraObject *object = static_cast<OrthoCameraObject *>(camera->GetObject());
			dx *= object->GetOrthoRectRight() - object->GetOrthoRectLeft();
			dy *= object->GetOrthoRectBottom() - object->GetOrthoRectTop();
			
			if (trackData->viewportType == kEditorViewportGraph)
			{
				float x = Floor(initalCameraPosition.x + right.x * dx + down.x * dy);
				float y = Floor(initalCameraPosition.y + right.y * dx + down.y * dy);
				camera->SetNodePosition(Point3D(x, y, initalCameraPosition.z));
			}
			else
			{
				Vector3D offset = right * dx + down * dy;
				camera->SetNodePosition(initalCameraPosition + offset);
			}
		}
		else
		{
			Vector3D offset = right * (dx * 8.0F) + down * (dy * 8.0F);
			camera->SetNodePosition(initalCameraPosition + offset);
		}
		
		editor->InvalidateViewport(trackData->viewportIndex);
	}
	
	return (true);
}

bool ViewportScrollTool::EndTool(Editor *editor, EditorTrackData *trackData)
{
	editor->SetCurrentCursor(previousCursor);
	return (true);
}


ViewportZoomTool::ViewportZoomTool(IconButtonWidget *widget) : StandardEditorTool(widget)
{
}

ViewportZoomTool::~ViewportZoomTool()
{
}

void ViewportZoomTool::Engage(Editor *editor, void *cookie)
{
	StandardEditorTool::Engage(editor, cookie);
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorGlass));
}

bool ViewportZoomTool::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	return (true);
}

bool ViewportZoomTool::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	float dy = trackData->previousViewportPosition.y - trackData->currentViewportPosition.y;
	if (dy != 0.0F)
	{
		EditorViewport *viewport = editor->GetViewport(trackData->viewportIndex);
		ViewportWidget *viewportWidget = viewport->GetViewportWidget();
		Camera *camera = trackData->viewportCamera;
		
		if (camera->GetCameraType() == kCameraOrtho)
		{
			float scale = trackData->viewportScale * Exp(dy * -4.0F);
			if (trackData->viewportType == kEditorViewportGraph) scale = Fmax(scale, 1.0F);
			trackData->viewportScale = scale;
			
			static_cast<OrthoViewportWidget *>(viewportWidget)->SetOrthoScale(Vector2D(scale, scale));
		}
		else
		{
			const Vector3D& view = camera->GetNodeTransform()[2];
			camera->SetNodePosition(camera->GetNodePosition() + view * (dy * 16.0F));
		}
		
		viewport->Invalidate();
	}
	
	return (true);
}


DragRectTool::DragRectTool(IconButtonWidget *widget, const ColorRGBA& color) :
		StandardEditorTool(widget),
		dragRect(color)
{
}

DragRectTool::~DragRectTool()
{
}

bool DragRectTool::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (trackData->viewportType != kEditorViewportFrustum)
	{
		const Point2D& anchor = trackData->anchorPosition;
		const Transform4D& transform = trackData->viewportCamera->GetNodeTransform();
		dragRect.Build(anchor, anchor, transform[0], transform[1], trackData->viewportScale);
		
		EditorViewport *viewport = editor->GetViewport(trackData->viewportIndex);
		viewport->SetToolRenderable(&dragRect);
		viewport->GetViewportWidget()->InvalidateTexture();
		return (true);
	}
	
	return (false);
}

bool DragRectTool::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	const Point2D& anchor = trackData->anchorPosition;
	const Point2D& position = trackData->currentPosition;
	const Transform4D& transform = trackData->viewportCamera->GetNodeTransform();
	dragRect.Build(anchor, position, transform[0], transform[1], trackData->viewportScale);
	
	EditorViewport *viewport = editor->GetViewport(trackData->viewportIndex);
	viewport->SetToolRenderable(&dragRect);
	viewport->GetViewportWidget()->InvalidateTexture();
	return (true);
}

bool DragRectTool::EndTool(Editor *editor, EditorTrackData *trackData)
{
	editor->InvalidateViewport(trackData->viewportIndex);
	return (true);
}


BoxSelectTool::BoxSelectTool(IconButtonWidget *widget) : DragRectTool(widget, ColorRGBA(0.5F, 0.5F, 0.5F, 1.0F))
{
}

BoxSelectTool::~BoxSelectTool()
{
}

void BoxSelectTool::UnselectAllTemp(Editor *editor)
{
	NodeReference *reference = editor->GetFirstSelectedNode();
	while (reference)
	{
		NodeReference *next = reference->Next();
		
		Node *node = reference->GetNode();
		if (node->GetManipulator()->GetManipulatorState() & kManipulatorTempSelected) editor->UnselectNode(node);
		
		reference = next;
	}
}

bool BoxSelectTool::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (DragRectTool::BeginTool(editor, trackData))
	{
		if ((trackData->currentModifierKeys & kModifierKeyShift) == 0) editor->UnselectAll();
		return (true);
	}
	
	SelectNode(editor, trackData);
	return (false);
}

bool BoxSelectTool::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	DragRectTool::TrackTool(editor, trackData);
	
	UnselectAllTemp(editor);
	
	const Point2D& position = trackData->currentPosition;
	const Point2D& anchor = trackData->anchorPosition;
	
	if ((position.x != anchor.x) || (position.y != anchor.y))
	{
		Point3D p1 = Editor::GetWorldSpacePosition(trackData, anchor);
		Point3D p3 = Editor::GetWorldSpacePosition(trackData, position);
		
		Node *rootNode = editor->GetRootNode();
		
		if (trackData->viewportType == kEditorViewportOrtho)
		{
			Region	region;
			
			float sx = (position.x > anchor.x) ? 1.0F : -1.0F;
			float sy = (position.y > anchor.y) ? 1.0F : -1.0F;
			
			const Transform4D& transform = trackData->viewportCamera->GetNodeTransform();
			Antivector4D *plane = region.GetPlaneArray();
			plane[0].Set(transform[0] * sx, p1);
			plane[1].Set(transform[1] * sy, p1);
			plane[2].Set(-transform[0] * sx, p3);
			plane[3].Set(-transform[1] * sy, p3);
			region.SetPlaneCount(4);
			
			unsigned_int32 mask = editor->GetEditorObject()->GetSelectionMask();
			
			Node *node = rootNode->GetFirstSubnode();
			while (node)
			{
				const EditorManipulator *manipulator = Editor::GetManipulator(node);
				const BoundingSphere *sphere = manipulator->GetTreeSphere();
				
				if ((sphere) && (region.SphereVisible(sphere->GetCenter(), sphere->GetRadius())))
				{
					if ((!(manipulator->GetManipulatorState() & (kManipulatorSelected | kManipulatorHidden))) && (editor->NodeSelectable(node, mask)))
					{
						if (manipulator->RegionPick(&region))
						{
							editor->SelectNode(node);
							
							Manipulator *manipulator = node->GetManipulator();
							manipulator->SetManipulatorState(manipulator->GetManipulatorState() | kManipulatorTempSelected);
						}
					}
					
					node = rootNode->GetNextNode(node);
				}
				else
				{
					node = rootNode->GetNextLevelNode(node);
				}
			}
		}
		else
		{
			float left = p1.x;
			float right = p3.x;
			if (left > right)
			{
				float t = left;
				left = right;
				right = t;
			}
			
			float top = p1.y;
			float bottom = p3.y;
			if (top > bottom)
			{
				float t = top;
				top = bottom;
				bottom = t;
			}
			
			Editor::GetManipulator(rootNode)->SelectGraphNodes(left, right, top, bottom, kManipulatorTempSelected);
		}
	}
	
	return (true);
}

bool BoxSelectTool::EndTool(Editor *editor, EditorTrackData *trackData)
{
	TrackTool(editor, trackData);
	
	NodeReference *reference = editor->GetFirstSelectedNode();
	while (reference)
	{
		Manipulator *manipulator = reference->GetNode()->GetManipulator();
		manipulator->SetManipulatorState(manipulator->GetManipulatorState() & ~kManipulatorTempSelected);
		
		reference = reference->Next();
	}
	
	editor->GetViewport(trackData->viewportIndex)->SetToolRenderable(nullptr);
	return (DragRectTool::EndTool(editor, trackData));
}


ViewportBoxZoomTool::ViewportBoxZoomTool(IconButtonWidget *widget) : DragRectTool(widget, ColorRGBA(0.5F, 0.5F, 0.75F, 1.0F))
{
}

ViewportBoxZoomTool::~ViewportBoxZoomTool()
{
}

void ViewportBoxZoomTool::Engage(Editor *editor, void *cookie)
{
	DragRectTool::Engage(editor, cookie);
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorGlass));
}

bool ViewportBoxZoomTool::EndTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	float x = Fabs(trackData->currentViewportPosition.x - trackData->anchorViewportPosition.x);
	float y = Fabs(trackData->currentViewportPosition.y - trackData->anchorViewportPosition.y);
	float scale = Fmax(x, y) * trackData->viewportScale;
	
	if (scale != 0.0F)
	{
		Point3D p = Editor::GetWorldSpacePosition(trackData, (trackData->anchorPosition + trackData->currentPosition) * 0.5F);
		
		if (trackData->viewportType == kEditorViewportGraph)
		{
			scale = Fmax(scale, 1.0F);
			p.x = Floor(p.x);
			p.y = Floor(p.y);
		}
		
		trackData->viewportCamera->SetNodePosition(p);
		
		ViewportWidget *viewport = editor->GetViewport(trackData->viewportIndex)->GetViewportWidget();
		static_cast<OrthoViewportWidget *>(viewport)->SetOrthoScale(Vector2D(scale, scale));
	}
	
	return (DragRectTool::EndTool(editor, trackData));
}


OrbitCameraTool::OrbitCameraTool(IconButtonWidget *widget) : StandardEditorTool(widget)
{
}

OrbitCameraTool::~OrbitCameraTool()
{
}

void OrbitCameraTool::Engage(Editor *editor, void *cookie)
{
	StandardEditorTool::Engage(editor, cookie);
}

bool OrbitCameraTool::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (trackData->viewportCamera->GetCameraType() == kCameraFrustum)
	{
		const NodeReference *reference = editor->GetGizmoTarget();
		if (reference)
		{
			Box3D box = Editor::GetManipulator(reference->GetNode())->CalculateWorldBoundingBox();
			orbitCenter = box.GetCenter();
		}
		else
		{
			orbitCenter.Set(0.0F, 0.0F, 0.0F);
		}
		
		return (true);
	}
	
	return (false);
}

bool OrbitCameraTool::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	FrustumViewportWidget *viewport = static_cast<FrustumViewportWidget *>(editor->GetViewport(trackData->viewportIndex)->GetViewportWidget());
	
	float dx = (trackData->previousViewportPosition.x - trackData->currentViewportPosition.x) * 8.0F;
	float dy = InterfaceMgr::GetShiftKey() ? 0.0F : (trackData->previousViewportPosition.y - trackData->currentViewportPosition.y) * 8.0F;
	
	float azm = viewport->GetCameraAzimuth() + dx;
	if (azm < -K::pi) azm += K::two_pi;
	else if (azm > K::pi) azm -= K::two_pi;
	
	float alt0 = viewport->GetCameraAltitude();
	float alt = alt0 + dy;
	if (alt < -1.45F) alt = -1.45F;
	else if (alt > 1.45F) alt = 1.45F;
	
	const Camera *camera = trackData->viewportCamera;
	const Vector3D& right = camera->GetNodeTransform()[0];
	
	Vector3D p = camera->GetNodePosition() - orbitCenter;
	Matrix3D m = Quaternion().SetRotationAboutAxis(alt - alt0, right).GetRotationMatrix();
	p = Matrix3D().SetRotationAboutZ(dx) * (m * p);
	
	viewport->SetCameraTransform(azm, alt, orbitCenter + p);
	return (true);
}


FreeCameraTool::FreeCameraTool(IconButtonWidget *widget) : StandardEditorTool(widget)
{
}

FreeCameraTool::~FreeCameraTool()
{
}

void FreeCameraTool::Engage(Editor *editor, void *cookie)
{
	StandardEditorTool::Engage(editor, cookie);
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorFree));
}

bool FreeCameraTool::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (trackData->viewportCamera->GetCameraType() == kCameraFrustum)
	{
		cameraSpeed = 0.0F;
		cameraFlags = 0;
		
		previousCursor = editor->GetCurrentCursor();
		editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorFree));
		return (true);
	}
	
	return (false);
}

bool FreeCameraTool::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	FrustumViewportWidget *viewport = static_cast<FrustumViewportWidget *>(editor->GetViewport(trackData->viewportIndex)->GetViewportWidget());
	
	float dx = trackData->previousViewportPosition.x - trackData->currentViewportPosition.x;
	float dy = trackData->previousViewportPosition.y - trackData->currentViewportPosition.y;
	
	float azm = viewport->GetCameraAzimuth() + dx * 8.0F;
	if (azm < -K::pi) azm += K::two_pi;
	else if (azm > K::pi) azm -= K::two_pi;
	
	float alt = viewport->GetCameraAltitude() + dy * 8.0F;
	if (alt < -1.45F) alt = -1.45F;
	else if (alt > 1.45F) alt = 1.45F;
	
	const Camera *camera = trackData->viewportCamera;
	Point3D position = camera->GetNodePosition();
	
	unsigned_int32 flags = cameraFlags;
	if (flags != 0)
	{
		float t = TheTimeMgr->GetSystemFloatDeltaTime();
		cameraSpeed = Fmin(cameraSpeed + t * 5.0e-4F, editor->GetEditorObject()->GetCameraSpeed());
		t *= cameraSpeed;
		
		if (InterfaceMgr::GetShiftKey()) t *= 5.0F;
		
		const Vector3D& view = camera->GetNodeTransform()[2];
		const Vector3D& right = camera->GetNodeTransform()[0];
		
		if (flags & kFreeCameraForward) position += view * t;
		if (flags & kFreeCameraBackward) position -= view * t;
		if (flags & kFreeCameraRight) position += right * t;
		if (flags & kFreeCameraLeft) position -= right * t;
		if (flags & kFreeCameraUp) position.z += t;
		if (flags & kFreeCameraDown) position.z -= t;
	}
	else
	{
		cameraSpeed = 0.0F;
	}
	
	viewport->SetCameraTransform(azm, alt, position);
	return (true);
}

bool FreeCameraTool::EndTool(Editor *editor, EditorTrackData *trackData)
{
	editor->SetCurrentCursor(previousCursor);
	return (true);
}

// ZYURVUR
