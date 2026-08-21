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
#include "C4Application.h"
#include "C4ToolWindows.h"
#include "C4EditorCommands.h"
#include "C4EditorSupport.h"
#include "C4GeometryManipulators.h"
#include "C4InstanceManipulators.h"
#include "C4WorldEditor.h"
#include "C4ScriptEditor.h"
#include "C4PanelEditor.h"


using namespace C4;


void Editor::HandleCloseMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	Close();
}

void Editor::HandleSaveWorldMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	if (resourceName[0] == 0) HandleSaveWorldAsMenuItem(nullptr, nullptr);
	else SaveWorld();
}

void Editor::HandleSaveWorldAsMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	FilePicker *picker = new WorldSavePicker;
	picker->SetCompletionProc(&SavePickerProc, this);
	AddSubwindow(picker);
}

void Editor::HandleSaveAndPlayWorldMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	if (resourceName[0] == 0)
	{
		HandleSaveWorldAsMenuItem(nullptr, nullptr);
	}
	else
	{
		if ((!(editorState & kEditorWorldUnsaved)) || (SaveWorld()))
		{
			ResourceName name(resourceName);
			Close();
			
			TheInterfaceMgr->GetStrip()->Hide();
			TheConsoleWindow->Close();
			
			TheWorldEditor->SetPlayedWorldName(name);
			TheApplication->LoadWorld(name);
		}
	}
}

void Editor::HandleImportSceneMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	SceneImportPicker *picker = new SceneImportPicker(nullptr, kSceneImportGeometry);
	picker->SetCompletionProc(&SceneImportPickerProc, this);
	AddSubwindow(picker);
}

void Editor::HandleExportSceneMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	SceneExportPicker *picker = new SceneExportPicker(nullptr);
	picker->SetCompletionProc(&SceneExportPickerProc, this);
	AddSubwindow(picker);
}

void Editor::HandleLoadModelResourceMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	const char *title = TheWorldEditor->GetStringTable()->GetString(StringID('IMDL'));
	FilePicker *picker = new FilePicker('WMDL', title, TheResourceMgr->GetGenericCatalog(), ModelResource::GetDescriptor());
	picker->SetCompletionProc(&ModelLoadPickerProc, this);
	AddSubwindow(picker);
}

void Editor::HandleSaveModelResourceMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	const char *title = TheWorldEditor->GetStringTable()->GetString(StringID('EMDL'));
	FilePicker *picker = new FilePicker('WMDL', title, TheResourceMgr->GetGenericCatalog(), ModelResource::GetDescriptor(), nullptr, kFilePickerSave);
	picker->SetCompletionProc(&ModelSavePickerProc, this);
	AddSubwindow(picker);
	
	const char *name = resourceName;
	int32 directoryLength = Text::GetDirectoryPathLength(name);
	picker->SetFileName(&name[directoryLength]);
}

void Editor::HandleUndoMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	for (;;)
	{
		UndoData *undoData = undoList.Last();
		if (!undoData) break;
		
		bool coupled = undoData->Coupled();
		undoData->Undo(this);
		delete undoData;
		
		if (!coupled) break;
	} 
	
	if (undoList.Empty()) editorMenuItem[kEditorMenuUndo]->Disable(); 
	 
	editorState |= kEditorRedrawViewports | kEditorUpdateConditionalItems; 
}
 
void Editor::HandleCutMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	const Node *targetZone = GetTargetZone();
	 
	NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode(); 
		if (node == targetZone)
		{
			SetTargetZone(GetRootNode());
			break;
		}
		
		reference = reference->Next();
	}
	
	HandleCopyMenuItem(nullptr, nullptr);
	HandleClearMenuItem(nullptr, nullptr);
}

void Editor::HandleCopyMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	delete editorClipboard;
	
	Node *root = GetRootNode();
	InfiniteZone *zone = static_cast<InfiniteZone *>(root);
	Object *auxiliaryObject = zone->GetAuxiliaryObject();
	auxiliaryObject->Retain();
	zone->SetAuxiliaryObject(nullptr);
	
	Manipulator *manipulator = root->GetManipulator();
	unsigned_int32 state = manipulator->GetManipulatorState();
	manipulator->SetManipulatorState(state | kManipulatorSelected);
	
	editorClipboard = new Package(nullptr);
	root->PackTree(editorClipboard, kPackSelected | kPackEditor);
	
	zone->SetAuxiliaryObject(auxiliaryObject);
	auxiliaryObject->Release();
	
	manipulator->SetManipulatorState(state);
	
	editorMenuItem[kEditorMenuPaste]->Enable();
}

void Editor::HandlePasteMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	Paste(GetTargetZone());
}

void Editor::HandlePasteSubnodesMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	Paste(GetFirstSelectedNode()->GetNode());
}

void Editor::HandleClearMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	NodeReference *reference = GetFirstSelectedNode();
	if (reference)
	{
		if ((reference->GetNode() != GetRootNode()) || (reference->Next()))
		{
			do
			{
				Node *root = reference->GetNode()->GetSuperNode();
				if ((root) && (root->GetNodeType() == kNodeGroup))
				{
					const Manipulator *manipulator = root->GetManipulator();
					if ((!manipulator->Selected()) && (EntireGroupSelected(root))) SelectNode(root);
				}
				
				reference = reference->Next();
				
			} while (reference);
			
			AddUndoData(new DeleteUndoData(GetSelectionList()));
			
			reference = GetFirstSelectedNode();
			do
			{
				NodeReference *next = reference->Next();
				DeleteNode(reference->GetNode(), true);
				reference = next;
			} while (reference);
		}
	}
}

void Editor::HandleSelectAllMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	SelectAll(GetRootNode());
}

void Editor::HandleSelectSuperNodeMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	NodeReference *reference = GetLastSelectedNode();
	while (reference)
	{
		Node *super = reference->GetNode()->GetSuperNode();
		if (super) SelectNode(super);
		reference = reference->Previous();
	}
}

void Editor::HandleSelectSubtreeMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode()->GetFirstSubnode();
		while (node)
		{
			SelectNode(node);
			node = node->Next();
		}
		
		reference = reference->Next();
	}
}

void Editor::HandleSelectAllMaskMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	UnselectAll();
	SelectAllMask(GetRootNode());
}

void Editor::HandleSelectMaterialMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	UnselectAll();
	SelectWithMaterial();
}

void Editor::HandleLockSelectionMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	editorState |= kEditorSelectionLocked | kEditorUpdateConditionalItems;
}

void Editor::HandleUnlockSelectionMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	editorState = (editorState & ~kEditorSelectionLocked) | kEditorUpdateConditionalItems;
}

void Editor::HandleDuplicateMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	HandleCopyMenuItem(nullptr, nullptr);
	HandlePasteMenuItem(nullptr, nullptr);
}

void Editor::HandleCloneMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	List<NodeReference>		cloneList;
	
	Zone *targetZone = GetTargetZone();
	
	NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		const Node *node = reference->GetNode();
		if (!GetManipulator(node)->PredecessorSelected())
		{
			Node *clone = node->Clone();
			cloneList.Append(new NodeReference(clone));
		}
		
		reference = reference->Next();
	}
	
	UnselectAll();
	
	reference = cloneList.First();
	while (reference)
	{
		Node *clone = reference->GetNode();
		targetZone->AddSubnode(clone);
		
		EditorManipulator::Install(this, clone);
		GetManipulator(clone)->InvalidateGraph();
		
		clone->Preprocess();
		SelectNode(clone);
		
		Node *node = clone->GetFirstSubnode();
		while (node)
		{
			SelectNode(node);
			node = clone->GetNextNode(node);
		}
		
		reference = reference->Next();
	}
	
	AddUndoData(new PasteUndoData(&cloneList));
}

void Editor::HandleCopyTransformMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	const NodeReference *reference = GetGizmoTarget();
	if (reference) transformClipboard = reference->GetNode()->GetNodeTransform();
}

void Editor::HandlePasteTransformMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	const NodeReference *reference = GetGizmoTarget();
	if (reference)
	{
		Node *node = reference->GetNode();
		AddUndoData(new MoveUndoData(node));
		
		node->SetNodeTransform(transformClipboard);
		node->Invalidate();
		
		if (node->GetNodeType() == kNodeGeometry)
		{
			node->Update();
			RebuildGeometry(static_cast<Geometry *>(node));
		}
	}
}

void Editor::HandleNodeInfoMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	if (!selectionList.Empty()) AddSubwindow(new NodeInfoWindow(this));
}

void Editor::HandleEditControllerMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	NodeReference *reference = GetFirstSelectedNode();
	if (reference)
	{
		const Node *node = reference->GetNode();
		Controller *controller = node->GetController();
		if (controller)
		{
			ControllerType type = controller->GetControllerType();
			if (type == kControllerScript)
			{
				ScriptController *scriptController = static_cast<ScriptController *>(controller);
				ScriptObject *object = scriptController->GetScriptObject();
				if (!object)
				{
					object = new ScriptObject;
					scriptController->SetScriptObject(object);
					object->Release();
				}
				
				AddSubwindow(new ScriptEditor(node, object));
				SetWorldUnsavedFlag();
			}
			else if (type == kControllerPanel)
			{
				AddSubwindow(new PanelEditor(static_cast<const PanelEffect *>(node)));
				SetWorldUnsavedFlag();
			}
		}
	}
}

void Editor::HandleGroupMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	List<NodeReference>		referenceList;
	
	NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetSuperNode()->GetNodeType() != kNodeGroup) referenceList.Append(new NodeReference(node));
		reference = reference->Next();
	}
	
	GroupUndoData *undoData = new GroupUndoData;
	AddUndoData(undoData);
	
	for (;;)
	{
		reference = referenceList.First();
		if (!reference) break;
		
		Node *groupNode = new Node;
		EditorManipulator::Install(this, groupNode);
		groupNode->Preprocess();
		
		Node *superNode = reference->GetNode()->GetSuperNode();
		do
		{
			NodeReference *next = reference->Next();
			
			Node *node = reference->GetNode();
			if (node->GetSuperNode() == superNode)
			{
				groupNode->AddSubnode(node);
				delete reference;
			}
			
			reference = next;
		} while (reference);
		
		superNode->AddSubnode(groupNode);
		undoData->AddGroup(groupNode);
		
		GetManipulator(groupNode)->InvalidateGraph();
	}
}

void Editor::HandleResetTransformMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	AddUndoData(new MoveUndoData(GetSelectionList()));
	
	NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetSuperNode())
		{
			node->SetNodeTransform(K::identity_4D);
			node->Invalidate();
		}
		
		reference = reference->Next();
	}
	
	rootNode->Update();
	
	reference = GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeGeometry) RebuildGeometry(static_cast<Geometry *>(node));
		
		reference = reference->Next();
	}
}

void Editor::HandleAlignToGridMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	AddUndoData(new MoveUndoData(GetSelectionList()));
	
	float spacing = GetEditorObject()->GetGridLineSpacing();
	float inverse = 1.0F / spacing;
	
	NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		const Node *super = node->GetSuperNode();
		if (super)
		{
			Point3D p = node->GetWorldPosition();
			p.x = Floor(p.x * inverse + 0.5F) * spacing;
			p.y = Floor(p.y * inverse + 0.5F) * spacing;
			p.z = Floor(p.z * inverse + 0.5F) * spacing;
			
			node->SetNodePosition(super->GetInverseWorldTransform() * p);
			node->Invalidate();
		}
		
		reference = reference->Next();
	}
	
	rootNode->Update();
	
	reference = GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeGeometry) RebuildGeometry(static_cast<Geometry *>(node));
		
		reference = reference->Next();
	}
}

void Editor::HandleSetTargetZoneMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeZone)
		{
			SetTargetZone(static_cast<Zone *>(node));
			break;
		}
		
		reference = reference->Next();
	}
}

void Editor::HandleMoveToTargetZoneMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	ReparentSelectedNodes(GetTargetZone());
}

void Editor::HandleConnectNodeMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	NodeReference *reference = GetFirstSelectedNode();
	if (reference)
	{
		AddUndoData(new ConnectUndoData(&selectedConnectorList));
		bool connection = false;
		
		Node *target = reference->GetNode();
		EditorManipulator *manipulator = selectedConnectorList.First();
		while (manipulator)
		{
			Node *node = manipulator->GetTargetNode();
			if (node != target)
			{
				if (manipulator->SetConnectorSelectionTarget(target))
				{
					connection = true;
					node->Invalidate();
				}
			}
			
			manipulator = manipulator->Next();
		}
		
		if (!connection) DeleteLastUndoData();
	}
	
	editorState |= kEditorUpdateConditionalItems;
}

void Editor::HandleUnconnectNodeMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	AddUndoData(new ConnectUndoData(&selectedConnectorList));
	
	EditorManipulator *manipulator = selectedConnectorList.First();
	while (manipulator)
	{
		manipulator->SetConnectorSelectionTarget(nullptr);
		manipulator->GetTargetNode()->Invalidate();
		manipulator = manipulator->Next();
	}
	
	editorState |= kEditorUpdateConditionalItems;
}

void Editor::HandleAutoConnectPortalMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	AddUndoData(new ConnectUndoData(GetSelectionList()));
	
	NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodePortal)
		{
			Portal *portal = static_cast<Portal *>(node);
			if (portal->GetPortalType() != kPortalOcclusion)
			{
				const Point3D& center = portal->GetBoundingSphere()->GetCenter();
				Point3D position = center - portal->GetWorldTransform()[2] * 0.125F;
				portal->SetConnectedZone(GetEditorWorld()->FindZone(position));
				Editor::GetManipulator(portal)->HandleConnectorUpdate();
			}
		}
		
		reference = reference->Next();
	}
	
	editorState |= kEditorUpdateConditionalItems;
}

void Editor::HandleConnectRootZoneMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	AddUndoData(new ConnectUndoData(&selectedConnectorList));
	bool undo = false;
	
	EditorManipulator *manipulator = selectedConnectorList.First();
	while (manipulator)
	{
		if (manipulator->GetTargetNode() != rootNode) undo |= manipulator->SetConnectorSelectionTarget(rootNode);
		manipulator = manipulator->Next();
	}
	
	if (!undo) DeleteLastUndoData();
	editorState |= kEditorUpdateConditionalItems;
}

void Editor::HandleSelectConnectedNodeMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	UnselectAll();
	
	const EditorManipulator *manipulator = selectedConnectorList.First();
	while (manipulator)
	{
		Node *node = manipulator->GetConnectorSelectionTarget();
		if (node) SelectNode(node);
		
		manipulator = manipulator->Next();
	}
}

void Editor::HandleMoveViewportCameraToNodeMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	const NodeReference *reference = GetFirstSelectedNode();
	if (reference)
	{
		const Transform4D& transform = reference->GetNode()->GetWorldTransform();
		const Vector3D& view = transform[2];
		const Point3D& position = transform.GetTranslation();
		
		float azm = ((view.x != 0.0F) || (view.y != 0.0F)) ? Atan(view.y, view.x) : 0.0F;
		float alt = Atan(view.z, Sqrt(view.x * view.x + view.y * view.y));
		if (alt < -1.45F) alt = -1.45F;
		else if (alt > 1.45F) alt = 1.45F;
		
		for (machine a = 0; a < kEditorViewportCount; a++)
		{
			ViewportWidget *viewport = GetViewport(a)->GetViewportWidget();
			if (viewport->GetWidgetType() == kWidgetFrustumViewport) static_cast<FrustumViewportWidget *>(viewport)->SetCameraTransform(azm, alt, position);
		}
	}
}

void Editor::HandleOpenInstancedWorldMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	const NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		const Node *node = reference->GetNode();
		
		if (node->GetNodeType() == kNodeInstance)
		{
			const ResourceName& name = static_cast<const Instance *>(node)->GetWorldName();
			if (name[0] != 0) Open(name);
		}
		
		reference = reference->Next();
	}
}

void Editor::HandleRebuildGeometryMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	AddUndoData(new GeometryUndoData(GetSelectionList()));
	
	const NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeGeometry) RebuildGeometry(static_cast<Geometry *>(node));
		reference = reference->Next();
	}
}

void Editor::HandleRebuildWithNewPathMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	UndoData *geometryUndoData = nullptr;
	UndoData *effectUndoData = nullptr;
	
	const NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		
		NodeType type = node->GetNodeType();
		if (type == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			if (geometry->GetGeometryType() == kGeometryPrimitive)
			{
				PrimitiveGeometry *primitive = static_cast<PrimitiveGeometry *>(geometry);
				if (primitive->PathPrimitive())
				{
					PathPrimitiveGeometry *pathPrimitive = static_cast<PathPrimitiveGeometry *>(primitive);
					const PathMarker *marker = pathPrimitive->GetConnectedPathMarker();
					if (marker)
					{
						if (!geometryUndoData)
						{
							geometryUndoData = new GeometryUndoData(GetSelectionList(), kGeometryPrimitive);
							if (effectUndoData) geometryUndoData->SetCoupledFlag(true);
							AddUndoData(geometryUndoData);
						}
						
						pathPrimitive->GetObject()->SetPrimitivePath(marker->GetPath());
						RebuildGeometry(pathPrimitive);
					}
				}
			}
		}
		else if (type == kNodeEffect)
		{
			Effect *effect = static_cast<Effect *>(node);
			if (effect->GetEffectType() == kEffectTube)
			{
				TubeEffect *tube = static_cast<TubeEffect *>(effect);
				const PathMarker *marker = tube->GetConnectedPathMarker();
				if (marker)
				{
					if (!effectUndoData)
					{
						effectUndoData = new TubeEffectUndoData(GetSelectionList());
						if (geometryUndoData) effectUndoData->SetCoupledFlag(true);
						AddUndoData(effectUndoData);
					}
					
					TubeEffectObject *object = tube->GetObject();
					object->SetTubePath(marker->GetPath());
					object->Build();
					
					tube->Invalidate();
					tube->Neutralize();
					tube->Preprocess();
				}
			}
		}
		
		reference = reference->Next();
	}
}

void Editor::HandleRecalculateNormalsMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	AddUndoData(new GeometryUndoData(GetSelectionList()));
	
	const NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeGeometry)
		{
			bool tangentsBuilt = false;
			
			Geometry *geometry = static_cast<Geometry *>(node);
			const GeometryObject *object = geometry->GetObject();
			
			int32 levelCount = object->GetGeometryLevelCount();
			for (machine a = 0; a < levelCount; a++)
			{
				GeometryLevel *level = object->GetGeometryLevel(a);
				if (level->GetArray(kArrayNormal))
				{
					level->CalculateNormalArray();
					if (level->GetArray(kArrayTangent))
					{
						if (level->GetArrayDescriptor(kArrayTangent)->componentCount == 4)
						{
							level->CalculateTangentArray();
						}
						else
						{
							GeometryLevel	tempLevel;
							
							tempLevel.CopyGeometryLevel(level);
							level->BuildTangentArray(&tempLevel);
							tangentsBuilt = true;
						}
					}
				}
				else
				{
					GeometryLevel	tempLevel;
					
					tempLevel.BuildNormalArray(level);
					level->BuildTangentArray(&tempLevel);
					tangentsBuilt = true;
				}
			}
			
			if (tangentsBuilt) InvalidateGeometry(geometry);
		}
		
		reference = reference->Next();
	}
}

void Editor::HandleBakeTransformIntoVerticesMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	AddUndoData(new GeometryUndoData(GetSelectionList(), kGeometryMesh));
	
	const NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		
		if (node->GetNodeType() == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			if (geometry->GetGeometryType() == kGeometryMesh)
			{
				const Transform4D& transform = geometry->GetNodeTransform();
				GeometryObject *object = geometry->GetObject();
				
				int32 levelCount = object->GetGeometryLevelCount();
				for (machine a = 0; a < levelCount; a++)
				{
					GeometryLevel *level = object->GetGeometryLevel(a);
					level->TransformGeometryLevel(transform);
				}
				
				static_cast<MeshGeometryObject *>(object)->UpdateBounds();
				object->BuildCollisionData();
				
				Node *subnode = geometry->GetFirstSubnode();
				while (subnode)
				{
					subnode->SetNodeTransform(transform * subnode->GetNodeTransform());
					subnode = subnode->Next();
				}
				
				geometry->SetNodeTransform(K::identity_4D);
				InvalidateGeometry(geometry);
			}
		}
		
		reference = reference->Next();
	}
}

void Editor::HandleRepositionMeshOriginMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	AddSubwindow(new MeshOriginWindow(this));
}

void Editor::HandleSetMaterialMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	AddUndoData(new MaterialUndoData(GetSelectionList()));
	
	MaterialObject *materialObject = GetSelectedMaterial()->GetMaterialObject();
	
	const NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			GeometryManipulator *manipulator = static_cast<GeometryManipulator *>(geometry->GetManipulator());
			
			GeometryObject *object = geometry->GetObject();
			if (object->GetReferenceCount() == 1)
			{
				int32 materialCount = geometry->GetMaterialCount();
				
				if (manipulator->GetSelectionType() == kEditorSelectionSurface)
				{
					int32	materialIndex;
					
					for (machine a = 0; a < materialCount; a++)
					{
						if (geometry->GetMaterialObject(a) == materialObject)
						{
							materialIndex = a;
							goto found;
						}
					}
					
					materialIndex = materialCount;
					geometry->SetMaterialCount(++materialCount);
					geometry->SetMaterialObject(materialIndex, materialObject);
					
					found:
					int32 surfaceCount = object->GetSurfaceCount();
					if (surfaceCount != 0)
					{
						for (machine a = 0; a < surfaceCount; a++)
						{
							if (manipulator->SurfaceSelected(a)) object->GetSurfaceData(a)->materialIndex = materialIndex;
						}
					}
					else
					{
						geometry->SetMaterialObject(0, materialObject);
					}
				}
				else
				{
					manipulator->SetMaterial(materialObject);
				}
				
				geometry->OptimizeMaterials();
				object->BuildCollisionData();
			}
			else
			{
				if (manipulator->GetSelectionType() == kEditorSelectionSurface)
				{
					int32 surfaceCount = object->GetSurfaceCount();
					if (surfaceCount != 0)
					{
						for (machine a = 0; a < surfaceCount; a++)
						{
							if (manipulator->SurfaceSelected(a)) geometry->SetMaterialObject(object->GetSurfaceData(a)->materialIndex, materialObject);
						}
					}
					else
					{
						geometry->SetMaterialObject(0, materialObject);
					}
				}
				else
				{
					manipulator->SetMaterial(materialObject);
				}
			}
			
			InvalidateGeometry(geometry);
		}
		else
		{
			Editor::GetManipulator(node)->SetMaterial(materialObject);
		}
		
		reference = reference->Next();
	}
}

void Editor::HandleRemoveMaterialMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	AddUndoData(new MaterialUndoData(GetSelectionList()));
	
	const NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		Editor::GetManipulator(reference->GetNode())->RemoveMaterial();
		reference = reference->Next();
	}
}

void Editor::HandleCombineDetailLevelsMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	Array<Geometry *> combineArray(kMaxCombineGeometryCount);
	
	int32 combineCount = 0;
	const NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			int32 vertexCount = geometry->GetObject()->GetGeometryLevel(0)->GetVertexCount();
			
			int32 index = combineCount;
			for (machine a = 0; a < combineCount; a++)
			{
				int32 count = combineArray[a]->GetObject()->GetGeometryLevel(0)->GetVertexCount();
				if (count == vertexCount) goto next;
				if (count < vertexCount)
				{
					index = a;
					break;
				}
			}
			
			combineArray.InsertElement(index, geometry);
			if (++combineCount == kMaxCombineGeometryCount) break;
		}
		
		next:
		reference = reference->Next();
	}
	
	if (combineCount > 1)
	{
		Geometry *primaryGeometry = combineArray[0];
		GeometryObject *primaryObject = primaryGeometry->GetObject();
		GeometryLevel *primaryGeometryLevel = primaryObject->GetGeometryLevel(0);
		
		int32 boneCount = 0;
		bool skin = (primaryGeometryLevel->GetWeightData() != nullptr);
		if (skin)
		{
			bool match = true;
			
			const ArrayBundle *bundle = primaryGeometryLevel->GetArrayBundle(kArrayInverseBindTransform);
			if (bundle) boneCount = bundle->descriptor.elementCount;
			
			for (machine a = 1; a < combineCount; a++)
			{
				const GeometryLevel *level = combineArray[a]->GetObject()->GetGeometryLevel(0);
				if (level->GetWeightData())
				{
					const ArrayBundle *bundle = level->GetArrayBundle(kArrayInverseBindTransform);
					if ((!bundle) || (bundle->descriptor.elementCount != boneCount))
					{
						match = false;
						break;
					}
				}
				else
				{
					match = false;
					break;
				}
			}
			
			if (!match)
			{
				const StringTable *table = TheWorldEditor->GetStringTable();
				DisplayError(table->GetString(StringID('ERRR', 'COMB')));
				return;
			}
		}
		
		List<NodeReference>		deletedList;
		GeometryLevel			tempLevel[kMaxCombineGeometryCount];
		
		AddUndoData(new GeometryUndoData(combineArray[0]));
		
		for (machine a = 1; a < combineCount; a++) deletedList.Append(new NodeReference(combineArray[a]));
		DeleteUndoData *undoData = new DeleteUndoData(&deletedList);
		undoData->SetCoupledFlag(true);
		AddUndoData(undoData);
		
		const Transform4D& primaryTransform = primaryGeometry->GetInverseWorldTransform();
		
		tempLevel[0].CopyGeometryLevel(primaryGeometryLevel);
		for (machine a = 1; a < combineCount; a++)
		{
			Geometry *geometry = combineArray[a];
			tempLevel[a].CopyGeometryLevel(geometry->GetObject()->GetGeometryLevel(0));
			tempLevel[a].TransformGeometryLevel(primaryTransform * geometry->GetWorldTransform());
		}
		
		int32 primarySurfaceCount = primaryObject->GetSurfaceCount();
		
		for (machine a = 1; a < combineCount; a++)
		{
			Geometry *geometry = combineArray[a];
			
			unsigned_int16 *surfaceIndex = tempLevel[a].GetArray<unsigned_int16>(kArraySurfaceIndex);
			if (surfaceIndex)
			{
				const GeometryObject *object = geometry->GetObject();
				
				int32 surfaceCount = object->GetSurfaceCount();
				if (surfaceCount != 0)
				{
					unsigned_int32 *surfaceRemapTable = new unsigned_int32[surfaceCount];
					
					for (machine b = 0; b < surfaceCount; b++)
					{
						unsigned_int32 remap = MaxZero(Min(b, primarySurfaceCount - 1));
						
						const MaterialObject *materialObject = geometry->GetMaterialObject(object->GetSurfaceData(b)->materialIndex);
						if (primaryGeometry->GetMaterialObject(primaryObject->GetSurfaceData(remap)->materialIndex) != materialObject)
						{
							for (machine c = 0; c < primarySurfaceCount; c++)
							{
								if (primaryGeometry->GetMaterialObject(primaryObject->GetSurfaceData(c)->materialIndex) == materialObject)
								{
									remap = c;
									break;
								}
							}
						}
						
						surfaceRemapTable[b] = remap;
					}
					
					int32 vertexCount = tempLevel[a].GetVertexCount();
					for (machine b = 0; b < vertexCount; b++) surfaceIndex[b] = (unsigned_int16) surfaceRemapTable[surfaceIndex[b]];
					
					delete[] surfaceRemapTable;
				}
				else
				{
					int32 vertexCount = tempLevel[a].GetVertexCount();
					for (machine b = 0; b < vertexCount; b++) surfaceIndex[b] = (unsigned_int16) MaxZero(Min(surfaceIndex[b], primarySurfaceCount - 1));
				}
			}
			
			DeleteNode(geometry, true);
		}
		
		primaryObject->SetGeometryLevelCount(combineCount);
		for (machine a = 0; a < combineCount; a++) primaryObject->GetGeometryLevel(a)->BuildSegmentArray(&tempLevel[a], primarySurfaceCount, primaryObject->GetSurfaceData());
		
		InvalidateGeometry(primaryGeometry);
	}
}

void Editor::HandleSeparateDetailLevelsMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	List<NodeReference>		geometryList;
	
	const NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			if (geometry->GetObject()->GetGeometryLevelCount() > 1) geometryList.Append(new NodeReference(geometry));
		}
		
		reference = reference->Next();
	}
	
	if (!geometryList.Empty())
	{
		List<NodeReference>		createdList;
		
		AddUndoData(new GeometryUndoData(&geometryList));
		
		Buffer buffer(kPackageDefaultSize);
		
		const NodeReference *reference = geometryList.First();
		while (reference)
		{
			GeometryLevel	tempLevel;
			
			Geometry *primaryGeometry = static_cast<Geometry *>(reference->GetNode());
			GeometryObject *primaryObject = primaryGeometry->GetObject();
			int32 levelCount = primaryObject->GetGeometryLevelCount();
			Node *super = primaryGeometry->GetSuperNode();
			
			for (machine a = 1; a < levelCount; a++)
			{
				Geometry *geometry = static_cast<Geometry *>(primaryGeometry->Replicate());
				GeometryObject *object = geometry->GetObject();
				
				Package package(buffer, kPackageDefaultSize);
				Packer packer(&package);
				
				object->PackType(packer);
				object->Pack(packer, kPackEditor);
				
				Unpacker unpacker(package.GetStorage(), 1, kWorldVersion);
				object = static_cast<GeometryObject *>(Object::Construct(unpacker, kUnpackEditor));
				object->Unpack(++unpacker, kUnpackEditor);
				
				geometry->SetObject(object);
				object->Release();
				
				tempLevel.CopyGeometryLevel(object->GetGeometryLevel(a));
				object->SetGeometryLevelCount(1);
				object->GetGeometryLevel(0)->CopyGeometryLevel(&tempLevel);
				object->BuildCollisionData();
				
				super->AddSubnode(geometry);
				EditorManipulator::Install(this, geometry);
				GetManipulator(geometry)->InvalidateGraph();
				geometry->Preprocess();
				
				createdList.Append(new NodeReference(geometry));
			}
			
			tempLevel.CopyGeometryLevel(primaryObject->GetGeometryLevel(0));
			primaryObject->SetGeometryLevelCount(1);
			primaryObject->GetGeometryLevel(0)->CopyGeometryLevel(&tempLevel);
			primaryObject->BuildCollisionData();
			
			InvalidateGeometry(primaryGeometry);
			
			reference = reference->Next();
		}
		
		CreateUndoData *undoData = new CreateUndoData(&createdList);
		undoData->SetCoupledFlag(true);
		AddUndoData(undoData);
	}
}

void Editor::HandleConvertToGenericMeshMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	CreateUndoData *createUndoData = new CreateUndoData;
	ReparentUndoData *reparentUndoData = new ReparentUndoData;
	
	const NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		const Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeGeometry)
		{
			const Geometry *geometry = static_cast<const Geometry *>(node);
			MeshGeometry *mesh = new MeshGeometry(geometry);
			mesh->SetNodeTransform(geometry->GetNodeTransform());
			
			const Property *property = geometry->GetFirstProperty();
			while (property)
			{
				Property *clone = property->Clone();
				if (clone) mesh->AddProperty(clone);
				
				property = property->Next();
			}
			
			EditorManipulator::Install(this, mesh);
			geometry->GetSuperNode()->AddNewSubnode(mesh);
			createUndoData->AddNode(mesh);
			
			for (;;)
			{
				Node *subnode = geometry->GetFirstSubnode();
				if (!subnode) break;
				
				reparentUndoData->AddNode(subnode);
				mesh->AddNewSubnode(subnode);
			}
			
			GetManipulator(mesh)->InvalidateGraph();
		}
		
		reference = reference->Next();
	}
	
	const List<NodeReference> *createdList = createUndoData->GetCreatedList();
	if (!createdList->Empty())
	{
		AddUndoData(createUndoData);
		
		reparentUndoData->SetCoupledFlag(true);
		AddUndoData(reparentUndoData);
		
		UnselectNonGeometryNodes();
		HandleClearMenuItem(nullptr, nullptr);
		undoList.Last()->SetCoupledFlag(true);
	}
	else
	{
		delete reparentUndoData;
		delete createUndoData;
	}
}

void Editor::HandleMergeGeometryMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	Array<const Geometry *> geometryArray(8);
	
	int32 vertexCount = 0;
	const NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		const Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeGeometry)
		{
			const Geometry *geometry = static_cast<const Geometry *>(node);
			vertexCount += geometry->GetObject()->GetGeometryLevel(0)->GetVertexCount();
			geometryArray.AddElement(geometry);
		}
		
		reference = reference->Next();
	}
	
	int32 count = geometryArray.GetElementCount();
	if (count != 0)
	{
		if (vertexCount < 65535)
		{
			UnselectNonGeometryNodes();
			
			MeshGeometry *mesh = new MeshGeometry(count, geometryArray, geometryArray[0]);
			mesh->SetNodeTransform(geometryArray[0]->GetNodeTransform());
			
			Node *super = geometryArray[0]->GetSuperNode();
			while (super->GetManipulator()->Selected()) super = super->GetSuperNode();
			
			HandleClearMenuItem(nullptr, nullptr);
			
			EditorManipulator::Install(this, mesh);
			super->AddNewSubnode(mesh);
			
			CreateUndoData *undoData = new CreateUndoData(mesh);
			undoData->SetCoupledFlag(true);
			AddUndoData(undoData);
			
			GetManipulator(mesh)->InvalidateGraph();
		}
		else
		{
			const StringTable *table = TheWorldEditor->GetStringTable();
			DisplayError(table->GetString(StringID('ERRR', 'MERG')));
		}
	}
}

void Editor::HandleInvertGeometryMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	AddUndoData(new GeometryUndoData(GetSelectionList()));
	
	const NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			GeometryObject *object = geometry->GetObject();
			
			int32 levelCount = object->GetGeometryLevelCount();
			for (machine a = 0; a < levelCount; a++) object->GetGeometryLevel(a)->InvertGeometryLevel();
			
			if (object->GetGeometryType() == kGeometryPrimitive)
			{
				PrimitiveGeometryObject *primitiveObject = static_cast<PrimitiveGeometryObject *>(object);
				primitiveObject->SetPrimitiveFlags(primitiveObject->GetPrimitiveFlags() ^ kPrimitiveInvert);
			}
			
			InvalidateGeometry(geometry);
			static_cast<GeometryManipulator *>(GetManipulator(geometry))->UpdateSurfaceSelection();
		}
		
		reference = reference->Next();
	}
}

void Editor::HandleBooleanGeometryMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	const Geometry *geometry1 = nullptr;
	const Geometry *geometry2 = nullptr;
	
	const NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		const Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeGeometry)
		{
			if (!geometry1)
			{
				geometry1 = static_cast<const Geometry *>(node);
			}
			else
			{
				geometry2 = static_cast<const Geometry *>(node);
				break;
			}
		}
		
		reference = reference->Next();
	}
	
	if (geometry2)
	{
		BooleanOperation operation = (menuItem == editorMenuItem[kEditorMenuIntersectGeometry]) ? kBooleanIntersection : kBooleanUnion;
		MeshGeometry *mesh = new MeshGeometry(operation, geometry1, geometry2);
		
		if (mesh->GetObject()->GetGeometryLevel(0)->GetVertexCount() != 0)
		{
			mesh->SetNodeTransform(geometry1->GetNodeTransform());
			
			EditorManipulator::Install(this, mesh);
			geometry1->GetSuperNode()->AddNewSubnode(mesh);
			
			GetManipulator(mesh)->InvalidateGraph();
		}
		else
		{
			delete mesh;
			mesh = nullptr;
		}
		
		HandleClearMenuItem(nullptr, nullptr);
		
		if (mesh)
		{
			CreateUndoData *undoData = new CreateUndoData(mesh);
			undoData->SetCoupledFlag(true);
			AddUndoData(undoData);
		}
	}
}

void Editor::HandleGenerateAmbientOcclusionMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	AddSubwindow(new GenerateAmbientOcclusionWindow(this));
}

void Editor::HandleRemoveAmbientOcclusionMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	AddUndoData(new GeometryUndoData(GetSelectionList()));
	
	const NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if ((node->GetNodeType() == kNodeGeometry) && (node->GetObject()->GetReferenceCount() == 1))
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			const GeometryObject *object = geometry->GetObject();
			
			int32 levelCount = object->GetGeometryLevelCount();
			for (machine level = 0; level < levelCount; level++)
			{
				GeometryLevel *geometryLevel = object->GetGeometryLevel(level);
				Color4C *restrict color = geometryLevel->GetArray<Color4C>(kArrayColor0);
				if (color)
				{
					if (object->GetGeometryType() == kGeometryTerrain)
					{
						int32 vertexCount = geometryLevel->GetVertexCount();
						for (machine a = 0; a < vertexCount; a++) color[a].SetAlpha(255);
					}
					else
					{
						ArrayDescriptor		desc;
						GeometryLevel		tempLevel;
						
						tempLevel.CopyGeometryLevel(geometryLevel);
						
						desc.identifier = kArrayColor0;
						desc.elementCount = 0;
						desc.elementSize = 4;
						desc.componentCount = 1;
						
						geometryLevel->AllocateStorage(&tempLevel, 1, &desc);
						geometry->SetAttributeArray(kArrayColor0, (float *) nullptr);
					}
					
					InvalidateGeometry(geometry);
				}
			}
		}
		
		reference = reference->Next();
	}
}

void Editor::HandleShowAllMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	Node *root = GetRootNode();
	
	Node *node = root->GetNextNode(root);
	while (node)
	{
		ShowNode(node);
		node = root->GetNextNode(node);
	}
}

void Editor::HandleHideSelectedMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	const NodeReference *reference = GetFirstSelectedNode();
	while (reference)
	{
		const NodeReference *next = reference->Next();
		HideNode(reference->GetNode());
		reference = next;
	}
}

void Editor::HandleShowAllInTargetZoneMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	Zone *zone = GetTargetZone();
	
	Node *node = zone->GetNextNode(zone);
	while (node)
	{
		if (node->GetOwningZone() == zone) ShowNode(node);
		node = zone->GetNextNode(node);
	}
}

void Editor::HandleHideNonTargetZonesMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	Zone *zone = GetTargetZone();
	Node *root = GetRootNode();
	
	Node *node = root->GetNextNode(root);
	while (node)
	{
		if ((node->GetOwningZone() != zone) && (node != zone)) HideNode(node);
		node = root->GetNextNode(node);
	}
}

void Editor::HandleShowBackfacesMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	editorState |= kEditorRedrawViewports;
	
	unsigned_int32 editorFlags = editorObject->GetEditorFlags() ^ kEditorShowBackfaces;
	editorObject->SetEditorFlags(editorFlags);
	
	if (editorFlags & kEditorShowBackfaces)
	{
		flagButton[kEditorFlagBackfaces]->SetValue(1);
		static_cast<MenuItemWidget *>(menuItem)->ShowBullet();
	}
	else
	{
		flagButton[kEditorFlagBackfaces]->SetValue(0);
		static_cast<MenuItemWidget *>(menuItem)->HideBullet();
	}
}

void Editor::HandleExpandWorldsMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	unsigned_int32 editorFlags = editorObject->GetEditorFlags() ^ kEditorExpandWorlds;
	editorObject->SetEditorFlags(editorFlags);
	
	if (editorFlags & kEditorExpandWorlds)
	{
		flagButton[kEditorFlagExpandWorlds]->SetValue(1);
		static_cast<MenuItemWidget *>(menuItem)->ShowBullet();
		ExpandAllWorlds();
	}
	else
	{
		flagButton[kEditorFlagExpandWorlds]->SetValue(0);
		static_cast<MenuItemWidget *>(menuItem)->HideBullet();
		CollapseAllWorlds();
	}
}

void Editor::HandleExpandModelsMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	unsigned_int32 editorFlags = editorObject->GetEditorFlags() ^ kEditorExpandModels;
	editorObject->SetEditorFlags(editorFlags);
	
	if (editorFlags & kEditorExpandModels)
	{
		flagButton[kEditorFlagExpandModels]->SetValue(1);
		static_cast<MenuItemWidget *>(menuItem)->ShowBullet();
		ExpandAllModels();
	}
	else
	{
		flagButton[kEditorFlagExpandModels]->SetValue(0);
		static_cast<MenuItemWidget *>(menuItem)->HideBullet();
		CollapseAllModels();
	}
}

void Editor::HandleRenderLightingMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	editorState |= kEditorRedrawViewports;
	
	unsigned_int32 editorFlags = editorObject->GetEditorFlags() ^ kEditorRenderLighting;
	editorObject->SetEditorFlags(editorFlags);
	
	if (editorFlags & kEditorRenderLighting)
	{
		flagButton[kEditorFlagLighting]->SetValue(1);
		static_cast<MenuItemWidget *>(menuItem)->ShowBullet();
	}
	else
	{
		flagButton[kEditorFlagLighting]->SetValue(0);
		static_cast<MenuItemWidget *>(menuItem)->HideBullet();
	}
	
	Node *root = GetRootNode();
	Node *node = root;
	for (;;)
	{
		node = root->GetNextNode(node);
		if (!node) break;
		
		if (node->GetNodeType() == kNodeGeometry) static_cast<Geometry *>(node)->InvalidateShaderData();
	}
}

void Editor::HandleDrawFromCenterMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	unsigned_int32 editorFlags = editorObject->GetEditorFlags() ^ kEditorDrawFromCenter;
	editorObject->SetEditorFlags(editorFlags);
	
	if (editorFlags & kEditorDrawFromCenter)
	{
		flagButton[kEditorFlagCenter]->SetValue(1);
		static_cast<MenuItemWidget *>(menuItem)->ShowBullet();
	}
	else
	{
		flagButton[kEditorFlagCenter]->SetValue(0);
		static_cast<MenuItemWidget *>(menuItem)->HideBullet();
	}
}

void Editor::HandleCapGeometryMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	unsigned_int32 editorFlags = editorObject->GetEditorFlags() ^ kEditorCapGeometry;
	editorObject->SetEditorFlags(editorFlags);
	
	if (editorFlags & kEditorCapGeometry)
	{
		flagButton[kEditorFlagCap]->SetValue(1);
		static_cast<MenuItemWidget *>(menuItem)->ShowBullet();
	}
	else
	{
		flagButton[kEditorFlagCap]->SetValue(0);
		static_cast<MenuItemWidget *>(menuItem)->HideBullet();
	}
}

void Editor::HandleShowViewportMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	for (machine a = 0; a < kEditorViewportCount; a++)
	{
		if (menuItem == showViewportItem[a])
		{
			ShowViewport(a);
			break;
		}
	}
}

void Editor::HandleEditorSettingsMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	AddSubwindow(new EditorSettingsWindow(this));
}

void Editor::HandleShowAllPagesMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	for (machine a = 0; a < kMaxEditorToolBookCount; a++)
	{
		BookWidget *book = bookWidget[a];
		if (book->Visible())
		{
			EditorPage *page = editorObject->GetFirstEditorPage();
			while (page)
			{
				if (!page->Visible())
				{
					book->AppendPage(page);
					page->Show();
					page->Expand();
				}
				
				page = page->ListElement<EditorPage>::Next();
			}
			
			book->OrganizePages();
			break;
		}
	}
}

void Editor::HandleChangeViewportMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	int32 index = viewportMenuIndex;
	
	int32 selection = static_cast<MenuItemWidget *>(menuItem)->ListElement<MenuItemWidget>::GetListIndex();
	if (selection != editorObject->GetViewportMode(index))
	{
		editorObject->SetViewportMode(index, selection);
		ConstructViewport(index);
		UpdateViewportStructures();
	}
}

void Editor::HandleFrameAllMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	EditorViewport *viewport = editorViewport[viewportMenuIndex];
	ViewportWidget *viewportWidget = viewport->GetViewportWidget();
	viewport->Invalidate();
	
	EditorViewportType viewportType = viewport->GetEditorViewportType();
	if (viewportType == kEditorViewportGraph)
	{
		const EditorManipulator *manipulator = GetManipulator(GetRootNode());
		float width = manipulator->GetGraphWidth();
		float height = manipulator->GetGraphHeight();
		
		OrthoViewportWidget *orthoViewportWidget = static_cast<OrthoViewportWidget *>(viewportWidget);
		orthoViewportWidget->GetViewportCamera()->SetNodePosition(Point3D(PositiveFloor(width * 0.5F), PositiveFloor(height * 0.5F), 0.0F));
		
		float scale = Fmax((width + 32.0F) / orthoViewportWidget->GetWidgetSize().x, (height + 32.0F) / orthoViewportWidget->GetWidgetSize().y, 1.0F);
		orthoViewportWidget->SetOrthoScale(Vector2D(scale, scale));
	}
	else
	{
		const BoundingSphere *sphere = GetManipulator(GetRootNode())->GetTreeSphere();
		if (sphere)
		{
			const Point3D& center = sphere->GetCenter();
			float radius = sphere->GetRadius();
			
			if (viewportType == kEditorViewportOrtho)
			{
				OrthoViewportWidget *orthoViewportWidget = static_cast<OrthoViewportWidget *>(viewportWidget);
				OrthoCamera *camera = orthoViewportWidget->GetViewportCamera();
				
				camera->SetNodePosition(center - ProjectOnto(center, camera->GetNodeTransform()[2]));
				float scale = Fmax(radius / orthoViewportWidget->GetWidgetSize().x, radius / orthoViewportWidget->GetWidgetSize().y) * 2.0F;
				orthoViewportWidget->SetOrthoScale(Vector2D(scale, scale));
			}
			else if (viewportType == kEditorViewportFrustum)
			{
				FrustumViewportWidget *frustumViewportWidget = static_cast<FrustumViewportWidget *>(viewportWidget);
				FrustumCamera *camera = frustumViewportWidget->GetViewportCamera();
				
				float focal = camera->GetObject()->GetFocalLength();
				camera->SetNodePosition(center - camera->GetNodeTransform()[2] * (focal * radius));
			}
		}
	}
}

void Editor::HandleFrameSelectionMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	EditorViewport *viewport = editorViewport[viewportMenuIndex];
	ViewportWidget *viewportWidget = viewport->GetViewportWidget();
	viewport->Invalidate();
	
	EditorViewportType viewportType = viewport->GetEditorViewportType();
	if (viewportType == kEditorViewportGraph)
	{
		const NodeReference *reference = GetFirstSelectedNode();
		if (reference)
		{
			do
			{
				const Node *super = reference->GetNode()->GetSuperNode();
				while (super)
				{
					GetManipulator(super)->ExpandSubgraph();
					super = super->GetSuperNode();
				}
				
				reference = reference->Next();
			} while (reference);
			
			GetManipulator(GetRootNode())->UpdateGraph();
			
			reference = GetFirstSelectedNode();
			const EditorManipulator *manipulator = GetManipulator(reference->GetNode());
			const Point3D *position = &manipulator->GetGraphPosition();
			float xmin = position->x;
			float ymin = position->y;
			float xmax = xmin + kGraphBoxWidth;
			float ymax = ymin + kGraphBoxHeight;
			
			for (;;)
			{
				reference = reference->Next();
				if (!reference) break;
				
				manipulator = GetManipulator(reference->GetNode());
				position = &manipulator->GetGraphPosition();
				xmin = Fmin(xmin, position->x);
				ymin = Fmin(ymin, position->y);
				xmax = Fmax(xmax, position->x + kGraphBoxWidth);
				ymax = Fmax(ymax, position->y + kGraphBoxHeight);
			}
			
			OrthoViewportWidget *orthoViewportWidget = static_cast<OrthoViewportWidget *>(viewportWidget);
			orthoViewportWidget->GetViewportCamera()->SetNodePosition(Point3D(Floor((xmin + xmax) * 0.5F), Floor((ymin + ymax) * 0.5F), 0.0F));
			
			float scale = Fmax((xmax - xmin + 32.0F) / orthoViewportWidget->GetWidgetSize().x, (ymax - ymin + 32.0F) / orthoViewportWidget->GetWidgetSize().y, 1.0F);
			orthoViewportWidget->SetOrthoScale(Vector2D(scale, scale));
		}
	}
	else
	{
		const NodeReference *reference = GetFirstSelectedNode();
		if (reference)
		{
			Box3D box = GetManipulator(reference->GetNode())->CalculateWorldBoundingBox();
			for (;;)
			{
				reference = reference->Next();
				if (!reference) break;
				
				box.Union(GetManipulator(reference->GetNode())->CalculateWorldBoundingBox());
			}
			
			EditorManipulator::AdjustBoundingBox(&box);
			
			Point3D center = box.GetCenter();
			Vector3D size = box.GetSize();
			
			if (viewportType == kEditorViewportOrtho)
			{
				OrthoViewportWidget *orthoViewportWidget = static_cast<OrthoViewportWidget *>(viewportWidget);
				OrthoCamera *camera = orthoViewportWidget->GetViewportCamera();
				
				camera->SetNodePosition(center - ProjectOnto(center, camera->GetNodeTransform()[2]));
				
				float x = Magnitude(ProjectOnto(size, camera->GetNodeTransform()[0]));
				float y = Magnitude(ProjectOnto(size, camera->GetNodeTransform()[1]));
				
				float scale = Fmax(x / orthoViewportWidget->GetWidgetSize().x, y / orthoViewportWidget->GetWidgetSize().y);
				orthoViewportWidget->SetOrthoScale(Vector2D(scale, scale));
			}
			else if (viewportType == kEditorViewportFrustum)
			{
				FrustumViewportWidget *frustumViewportWidget = static_cast<FrustumViewportWidget *>(viewportWidget);
				FrustumCamera *camera = frustumViewportWidget->GetViewportCamera();
				
				float x = Magnitude(ProjectOnto(size, camera->GetNodeTransform()[0]));
				float y = Magnitude(ProjectOnto(size, camera->GetNodeTransform()[1]));
				
				float focal = camera->GetObject()->GetFocalLength();
				camera->SetNodePosition(center - camera->GetNodeTransform()[2] * (focal * Fmax(x, y) * 0.5F));
			}
		}
	}
}

void Editor::SavePickerProc(FilePicker *picker, void *cookie)
{
	ResourceName name(picker->GetFileName());
	name[Text::GetResourceNameLength(name)] = 0;
	int32 len = Text::GetPrefixDirectoryLength(name);
	
	Editor *editor = static_cast<Editor *>(cookie);
	editor->resourceName = &name[len];
	editor->resourceLocation.GetPath().Set(name, len - 1);
	editor->SaveWorld(static_cast<WorldSavePicker *>(picker)->GetStripFlag());
	
	ResourcePath title(editor->resourceName);
	editor->SetWindowTitle(title += WorldResource::GetDescriptor()->GetExtension());
	editor->SetStripTitle(&title[Text::GetDirectoryPathLength(title)]);
}

void Editor::SceneImportPickerProc(FilePicker *picker, void *cookie)
{
	GeometryImportData	importData;
	
	const SceneImportPicker *importPicker = static_cast<SceneImportPicker *>(picker);
	
	ResourceName name(importPicker->GetFileName());
	name[Text::GetResourceNameLength(name)] = 0;
	
	importData.importFlags = 0;
	importPicker->GetSceneImportPlugin()->ImportGeometry(static_cast<Editor *>(cookie), name, &importData);
}

void Editor::SceneExportPickerProc(FilePicker *picker, void *cookie)
{
	const SceneExportPicker *exportPicker = static_cast<SceneExportPicker *>(picker);
	
	ResourceName name(exportPicker->GetFileName());
	name[Text::GetResourceNameLength(name)] = 0;
	
	exportPicker->GetSceneExportPlugin()->ExportScene(name, static_cast<Editor *>(cookie)->GetEditorWorld());
}

void Editor::ModelLoadPickerProc(FilePicker *picker, void *cookie)
{
	Editor *editor = static_cast<Editor *>(cookie);
	editor->UnselectAll();
	
	Model *model = Model::New(picker->GetResourceName(), kModelUnknown, kUnpackEditor);
	if (model)
	{
		Node *node = model->GetFirstSubnode();
		while (node)
		{
			node->SetNodeFlags(node->GetNodeFlags() & ~kNodeNonpersistent);
			node = model->GetNextNode(node);
		}
		
		Zone *zone = editor->GetTargetZone();
		
		node = model->GetFirstSubnode();
		Node *subnode = node;
		while (subnode)
		{
			Node *next = subnode->Next();
			
			EditorManipulator::Install(editor, subnode);
			zone->AddSubnode(subnode);
			GetManipulator(subnode)->InvalidateGraph();
			
			subnode = next;
		}
		
		delete model;
		editor->editorState |= kEditorWorldUnsaved | kEditorRedrawViewports;
		
		while (node)
		{
			node->Preprocess();
			node = node->Next();
		}
	}
}

void Editor::ModelSavePickerProc(FilePicker *picker, void *cookie)
{
	File			file;
	ResourcePath	path;
	
	Editor *editor = static_cast<Editor *>(cookie);
	
	Node *root = editor->GetRootNode();
	root->InvalidateNodeIndex();
	
	Node *group = root->GetFirstSubnode();
	if ((group) && (group->GetNodeType() == kNodeGroup) && (!group->Next()))
	{
		root = group;
		root->InvalidateNodeIndex();
	}
	
	bool newModel = false;
	Model *model = nullptr;
	
	Node *node = root->GetFirstSubnode();
	while (node)
	{
		if (node->GetNodeType() == kNodeModel)
		{
			model = static_cast<Model *>(node);
			break;
		}
		
		node = node->Next();
	}
	
	if (!model)
	{
		newModel = true;
		model = new Model;
		
		for (;;)
		{
			Node *node = root->GetFirstSubnode();
			if (!node) break;
			
			model->AddSubnode(node);
		}
	}
	
	node = model;
	for (;;)
	{
		node = model->GetNextNode(node);
		if (!node) break;
		
		if (node->GetNodeType() == kNodeGeometry)
		{
			GeometryObject *object = static_cast<Geometry *>(node)->GetObject();
			object->SetGeometryFlags(object->GetGeometryFlags() | kGeometryModelExportFlags);
		}
	}
	
	ResourceName name(picker->GetFileName());
	name[Text::GetResourceNameLength(name)] = 0;
	
	TheResourceMgr->GetGenericCatalog()->GetResourcePath(ModelResource::GetDescriptor(), name, &path);
	TheResourceMgr->CreateDirectoryPath(path);
	
	if (file.Open(path, kFileCreate) == kFileOkay)
	{
		model->PackTree(&file, kPackInitialize);
		file.Close();
	}
	
	int32 start = Text::GetPrefixDirectoryLength(name);
	ModelRegistration *registration = Model::GetFirstRegistration();
	while (registration)
	{
		if (Text::CompareText(&name[start], registration->GetResourceName()))
		{
			registration->Reload();
			break;
		}
		
		registration = registration->Next();
	}
	
	if (newModel)
	{
		for (;;)
		{
			Node *node = model->GetFirstSubnode();
			if (!node) break;
			
			root->AddSubnode(node);
		}
		
		delete model;
	}
}


MeshOriginWindow::MeshOriginWindow(Editor *editor) : Window("WorldEditor/MeshOrigin")
{
	worldEditor = editor;
}

MeshOriginWindow::~MeshOriginWindow()
{
}

void MeshOriginWindow::Preprocess(void)
{
	Window::Preprocess();
	
	okayButton = static_cast<PushButtonWidget *>(FindWidget("OK"));
	cancelButton = static_cast<PushButtonWidget *>(FindWidget("Cancel"));
	
	unsigned_int32 settings = worldEditor->GetEditorObject()->GetMeshOriginSettings();
	
	radioButton[0][0] = static_cast<RadioWidget *>(FindWidget("Xmin"));
	radioButton[0][1] = static_cast<RadioWidget *>(FindWidget("Xcen"));
	radioButton[0][2] = static_cast<RadioWidget *>(FindWidget("Xmax"));
	radioButton[0][settings & 0xFF]->SetValue(1);
	
	radioButton[1][0] = static_cast<RadioWidget *>(FindWidget("Ymin"));
	radioButton[1][1] = static_cast<RadioWidget *>(FindWidget("Ycen"));
	radioButton[1][2] = static_cast<RadioWidget *>(FindWidget("Ymax"));
	radioButton[1][(settings >> 8) & 0xFF]->SetValue(1);
	
	radioButton[2][0] = static_cast<RadioWidget *>(FindWidget("Zmin"));
	radioButton[2][1] = static_cast<RadioWidget *>(FindWidget("Zcen"));
	radioButton[2][2] = static_cast<RadioWidget *>(FindWidget("Zmax"));
	radioButton[2][(settings >> 16) & 0xFF]->SetValue(1);
}

void MeshOriginWindow::CommitSettings(void) const
{
	unsigned_int32 settings = 0;
	for (machine a = 0; a < 3; a++)
	{
		for (machine b = 0; b < 3; b++)
		{
			if (radioButton[a][b]->GetValue() != 0)
			{
				settings |= b << (a * 8);
				break;
			}
		}
	}
	
	Editor *editor = worldEditor;
	editor->GetEditorObject()->SetMeshOriginSettings(settings);
	
	editor->AddUndoData(new GeometryUndoData(editor->GetSelectionList(), kGeometryMesh));
	
	const NodeReference *reference = editor->GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		
		if (node->GetNodeType() == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			if (geometry->GetGeometryType() == kGeometryMesh)
			{
				MeshGeometry *mesh = static_cast<MeshGeometry *>(geometry);
				MeshGeometryObject *object = mesh->GetObject();
				
				const Box3D& bounds = object->GetBoundingBox();
				
				unsigned_int32 i = settings & 0xFF;
				unsigned_int32 j = (settings >> 8) & 0xFF;
				unsigned_int32 k = (settings >> 16) & 0xFF;
				float x = (i == kMeshOriginMin) ? bounds.min.x : ((i == kMeshOriginMax) ? bounds.max.x : (bounds.min.x + bounds.max.x) * 0.5F);
				float y = (j == kMeshOriginMin) ? bounds.min.y : ((j == kMeshOriginMax) ? bounds.max.y : (bounds.min.y + bounds.max.y) * 0.5F);
				float z = (k == kMeshOriginMin) ? bounds.min.z : ((k == kMeshOriginMax) ? bounds.max.z : (bounds.min.z + bounds.max.z) * 0.5F);
				
				Vector3D dp(-x, -y, -z);
				mesh->SetNodePosition(mesh->GetNodePosition() - dp);
				
				int32 levelCount = object->GetGeometryLevelCount();
				for (machine a = 0; a < levelCount; a++)
				{
					GeometryLevel *level = object->GetGeometryLevel(a);
					level->TranslateGeometryLevel(dp);
				}
				
				object->UpdateBounds();
				object->BuildCollisionData();
				
				editor->InvalidateGeometry(mesh);
			}
		}
		
		reference = reference->Next();
	}
}

bool MeshOriginWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyDown)
	{
		unsigned_int32 code = eventData->keyCode;
		
		if (code == kKeyCodeEscape)
		{
			cancelButton->Activate();
			return (true);
		}
		else if (code == kKeyCodeReturn)
		{
			okayButton->Activate();
			return (true);
		}
	}
	
	return (Window::HandleKeyboardEvent(eventData));
}

void MeshOriginWindow::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		if (widget == okayButton)
		{
			CommitSettings();
			Close();
		}
		else if (widget == cancelButton)
		{
			Close();
		}
	}
}


GenerateAmbientOcclusionWindow::GenerateAmbientOcclusionWindow(Editor *editor) : Window("WorldEditor/AmbientOcclusion")
{
	worldEditor = editor;
	
	jobCount = 0;
	jobTable = nullptr;
}

GenerateAmbientOcclusionWindow::~GenerateAmbientOcclusionWindow()
{
	int32 count = jobCount;
	for (machine a = count - 1; a >= 0; a--) delete jobTable[a];
	delete[] jobTable;
	
	const NodeReference *reference = worldEditor->GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			worldEditor->InvalidateGeometry(geometry);
		}
		
		reference = reference->Next();
	}
}

GenerateAmbientOcclusionWindow::AmbientOcclusionJob::AmbientOcclusionJob(GenerateAmbientOcclusionWindow *window, ExecuteProc *execProc, void *cookie) : Job(execProc, cookie)
{
	jobWindow = window;
}

void GenerateAmbientOcclusionWindow::Preprocess(void)
{
	Window::Preprocess();
	
	startButton = static_cast<PushButtonWidget *>(FindWidget("Start"));
	cancelButton = static_cast<PushButtonWidget *>(FindWidget("Cancel"));
	intensityBox = static_cast<EditTextWidget *>(FindWidget("Intensity"));
	inputText = static_cast<TextWidget *>(FindWidget("Input"));
	
	stopButton = static_cast<PushButtonWidget *>(FindWidget("Stop"));
	progressBar = static_cast<ProgressWidget *>(FindWidget("Progress"));
	borderWidget = static_cast<BorderWidget *>(FindWidget("Border"));
	messageText = static_cast<TextWidget *>(FindWidget("Message"));
	
	SetFocusWidget(intensityBox);
}

void GenerateAmbientOcclusionWindow::Move(void)
{
	Window::Move();
	
	int32 count = jobCount;
	if (count > 0)
	{
		int32 progress = 0;
		
		for (machine a = 0; a < count; a++) progress += jobTable[a]->Complete();
		progressBar->SetValue(progress);
		
		if (progress == count) Close();
	}
}

void GenerateAmbientOcclusionWindow::StartJob(void)
{
	blockageMultiplier = Text::StringToFloat(intensityBox->GetText()) * 12.055888F;		// 12.055888F = 255.0F / normalization
	
	startButton->Hide();
	cancelButton->Hide();
	intensityBox->Hide();
	inputText->Hide();
	stopButton->Show();
	progressBar->Show();
	borderWidget->Show();
	messageText->Show();
	
	worldEditor->AddUndoData(new GeometryUndoData(worldEditor->GetSelectionList()));
	
	int32 count = 0;
	const NodeReference *reference = worldEditor->GetFirstSelectedNode();
	while (reference)
	{
		const Node *node = reference->GetNode();
		if ((node->GetNodeType() == kNodeGeometry) && (node->GetObject()->GetReferenceCount() == 1)) count++;
		reference = reference->Next();
	}
	
	jobCount = count;
	jobTable = new Job *[count];
	
	count = 0;
	reference = worldEditor->GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if ((node->GetNodeType() == kNodeGeometry) && (node->GetObject()->GetReferenceCount() == 1))
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			Job *job = new AmbientOcclusionJob(this, &GenerateAmbientOcclusionJob, geometry);
			jobTable[count++] = job;
			TheJobMgr->SubmitJob(job);
		}
		
		reference = reference->Next();
	}
	
	progressBar->SetMaxValue(count);
}

bool GenerateAmbientOcclusionWindow::DetectCollision(const Node *root, Ray *ray, PickData *pickData)
{
	bool result = false;
	
	if (root->Enabled())
	{
		float	t1, t2;
		
		EditorManipulator *manipulator = Editor::GetManipulator(root);
		const BoundingSphere *sphere = manipulator->GetTreeSphere();
		
		if ((sphere) && (Math::IntersectRayAndSphere(ray, sphere->GetCenter(), sphere->GetRadius(), &t1, &t2)))
		{
			if ((root->GetNodeType() == kNodeGeometry) && (!(static_cast<const Geometry *>(root)->GetObject()->GetGeometryFlags() & kGeometryInvisible)))
			{
				sphere = manipulator->GetNodeSphere();
				if ((sphere) && (Math::IntersectRayAndSphere(ray, sphere->GetCenter(), sphere->GetRadius(), &t1, &t2)))
				{
					Ray		nodeRay;
					
					const Transform4D& transform = root->GetInverseWorldTransform();
					nodeRay.origin = transform * ray->origin;
					nodeRay.direction = transform * ray->direction;
					nodeRay.radius = 0.0F;
					nodeRay.tmin = Fmax(ray->tmin, t1);
					nodeRay.tmax = Fmin(ray->tmax, t2);
					
					if (manipulator->Pick(&nodeRay, pickData))
					{
						pickData->pickPoint = root->GetWorldTransform() * pickData->pickPoint;
						pickData->pickNormal = pickData->pickNormal * transform;
						
						ray->tmax = pickData->rayParam;
						result = true;
					}
				}
			}
			
			Node *node = root->GetFirstSubnode();
			while (node)
			{
				result |= DetectCollision(node, ray, pickData);
				node = node->Next();
			}
		}
	}
	
	return (result);
}

void GenerateAmbientOcclusionWindow::GenerateAmbientOcclusionJob(Job *job, void *cookie)
{
	static const float directionTable[35][3] =
	{
		{0.9238795F, 0.0F, 0.3826833F},
		{0.8535532F, 0.3535534F, 0.3826833F},
		{0.6532814F, 0.6532814F, 0.3826833F},
		{0.3535532F, 0.8535532F, 0.3826833F},
		{0.0F, 0.9238795F, 0.3826833F},
		{-0.3535534F, 0.8535532F, 0.3826833F},
		{-0.6532814F, 0.6532814F, 0.3826833F},
		{-0.8535534F, 0.3535531F, 0.3826833F},
		{-0.9238795F, 0.0F, 0.3826833F},
		{-0.8535532F, -0.3535532F, 0.3826833F},
		{-0.6532813F, -0.6532815F, 0.3826833F},
		{-0.353553F, -0.8535534F, 0.3826833F},
		{0.0F, -0.9238795F, 0.3826833F},
		{0.3535535F, -0.8535532F, 0.3826833F},
		{0.6532816F, -0.6532812F, 0.3826833F},
		{0.8535534F, -0.3535532F, 0.3826833F},
		{0.7071067F, 0.0F, 0.7071067F},
		{0.6123723F, 0.3535532F, 0.7071067F},
		{0.3535532F, 0.6123723F, 0.7071067F},
		{0.0F, 0.7071067F, 0.7071067F},
		{-0.3535534F, 0.6123723F, 0.7071067F},
		{-0.6123723F, 0.3535534F, 0.7071067F},
		{-0.7071067F, 0.0F, 0.7071067F},
		{-0.6123722F, -0.3535534F, 0.7071067F},
		{-0.3535532F, -0.6123723F, 0.7071067F},
		{0.0F, -0.7071067F, 0.7071067F},
		{0.3535532F, -0.6123723F, 0.7071067F},
		{0.6123725F, -0.3535531F, 0.7071067F},
		{0.3826833F, 0.0F, 0.9238795F},
		{0.1913416F, 0.3314135F, 0.9238795F},
		{-0.1913416F, 0.3314135F, 0.9238795F},
		{-0.3826833F, 0.0F, 0.9238795F},
		{-0.1913416F, -0.3314135F, 0.9238795F},
		{0.1913416F, -0.3314135F, 0.9238795F},
		{0.0F, 0.0F, 1.0F}
	};
	
	Ray		ray;
	
	GenerateAmbientOcclusionWindow *window = static_cast<AmbientOcclusionJob *>(job)->GetJobWindow();
	const Node *rootNode = window->worldEditor->GetRootNode();
	float multiplier = window->blockageMultiplier;
	
	ray.radius = 0.0F;
	ray.tmin = 0.0F;
	
	const Geometry *geometry = static_cast<Geometry *>(cookie);
	const GeometryObject *object = geometry->GetObject();
	
	int32 levelCount = object->GetGeometryLevelCount();
	for (machine level = 0; level < levelCount; level++)
	{
		GeometryLevel *geometryLevel = object->GetGeometryLevel(level);
		int32 vertexCount = geometryLevel->GetVertexCount();
		
		Color4C *restrict color = geometryLevel->GetArray<Color4C>(kArrayColor0);
		if (!color)
		{
			ArrayDescriptor		desc;
			GeometryLevel		tempLevel;
			
			window->jobLock.AcquireExclusive();
			
			tempLevel.CopyGeometryLevel(geometryLevel);
			
			desc.identifier = kArrayColor0;
			desc.elementCount = vertexCount;
			desc.elementSize = 4;
			desc.componentCount = 1;
			
			geometryLevel->AllocateStorage(&tempLevel, 1, &desc);
			color = geometryLevel->GetArray<Color4C>(kArrayColor0);
			MemoryMgr::ClearMemory(color, vertexCount * sizeof(Color4C));
			
			window->jobLock.ReleaseExclusive();
		}
		
		window->jobLock.AcquireShared();
		
		const Point3D *vertex = geometryLevel->GetArray<Point3D>(kArrayPosition0);
		const Vector3D *normal = geometryLevel->GetArray<Vector3D>(kArrayNormal);
		
		float radius = 12.0F;
		
		for (machine a = 0; a < vertexCount; a++)
		{
			Vector3D nrml = normal[a] * geometry->GetInverseWorldTransform();
			Vector3D tang = Math::CreateUnitPerpendicular(nrml);
			Vector3D btng = nrml % tang;
			
			ray.origin = geometry->GetWorldTransform() * vertex[a] + nrml * 0.03125F;
			float blockage = 0.0F;
			
			for (machine b = 0; b < 35; b++)
			{
				PickData	pickData;
				
				const float *d = directionTable[b];
				ray.direction = (tang * d[0] + btng * d[1] + nrml * d[2]) * radius;
				ray.tmax = 1.0F;
				
				if (DetectCollision(rootNode, &ray, &pickData))
				{
					float t = ray.tmax * ray.tmax;
					float m = (1.0F - t * t) * d[2];
					blockage += m;
				}
			}
			
			color[a].SetAlpha(Min(MaxZero((int32) (255.0F - blockage * multiplier)), 255));
		}
		
		window->jobLock.ReleaseShared();
	}
}

bool GenerateAmbientOcclusionWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyDown)
	{
		unsigned_int32 code = eventData->keyCode;
		
		if (code == kKeyCodeReturn)
		{
			if (startButton->Visible()) startButton->Activate();
			return (true);
		}
		else if (code == kKeyCodeEscape)
		{
			if (cancelButton->Visible()) cancelButton->Activate();
			else stopButton->Activate();
			return (true);
		}
	}
	
	return (Window::HandleKeyboardEvent(eventData));
}

void GenerateAmbientOcclusionWindow::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		if (widget == startButton)
		{
			StartJob();
		}
		else if (widget == cancelButton)
		{
			Close();
		}
		else if (widget == stopButton)
		{
			TheJobMgr->CancelJobArray(jobCount, jobTable);
			Close();
		}
	}
}

// ZYURVUR
