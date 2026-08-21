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
#include "C4Primitives.h"
#include "C4Terrain.h"
#include "C4WorldEditor.h"
#include "C4EditorSupport.h"
#include "C4InstanceManipulators.h"


using namespace C4;


UndoData::UndoData(UndoType type)
{
	undoType = type;
	coupledFlag = false;
}

UndoData::~UndoData()
{
}


CreateUndoData::CreateUndoData() : UndoData(kUndoCreate)
{
}

CreateUndoData::CreateUndoData(Node *node) : UndoData(kUndoCreate)
{
	AddNode(node);
}

CreateUndoData::CreateUndoData(const List<NodeReference> *referenceList) : UndoData(kUndoCreate)
{
	const NodeReference *reference = referenceList->First();
	while (reference)
	{
		AddNode(reference->GetNode());
		reference = reference->Next();
	}
}

CreateUndoData::~CreateUndoData()
{
}

void CreateUndoData::Undo(Editor *editor)
{
	const NodeReference *created = createdList.First();
	while (created)
	{
		editor->DeleteSubtree(created->GetNode());
		created = created->Next();
	}
}


MoveUndoData::MoveUndoData(Node *node) : UndoData(kUndoMove)
{
	movedList.Append(new NodeTransformReference(node));
}

MoveUndoData::MoveUndoData(const List<NodeReference> *referenceList) : UndoData(kUndoMove)
{
	for (const NodeReference *reference = referenceList->First(); reference; reference = reference->Next())
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeMarker)
		{
			Marker *marker = static_cast<Marker *>(node);
			if (marker->GetMarkerType() == kMarkerPath)
			{
				movedList.Append(new PathReference(static_cast<PathMarker *>(marker)));
				continue;
			}
		}
		
		movedList.Append(new NodeTransformReference(node));
	}
}

MoveUndoData::~MoveUndoData()
{
}

MoveUndoData::PathReference::PathReference(PathMarker *marker) :
		NodeTransformReference(marker),
		path(*marker->GetPath())
{
}

void MoveUndoData::Undo(Editor *editor)
{
	const NodeReference *reference = movedList.First();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeMarker)
		{
			Marker *marker = static_cast<Marker *>(node);
			if (marker->GetMarkerType() == kMarkerPath)
			{
				PathMarker *pathMarker = static_cast<PathMarker *>(marker); 
				*pathMarker->GetPath() = *static_cast<const PathReference *>(reference)->GetPath();
			} 
		} 
		 
		node->SetNodeTransform(static_cast<const NodeTransformReference *>(reference)->GetTransform());
		Editor::GetManipulator(node)->InvalidateNode(); 
		
		reference = reference->Next();
	}
	 
	editor->RegenerateTexcoords(&movedList);
}

 
SizeUndoData::SizeUndoData(Node *node) : UndoData(kUndoSize)
{
	sizeNode = node;
	sizeCount = node->GetObject()->GetObjectSize(objectSize);
}

SizeUndoData::~SizeUndoData()
{
}

void SizeUndoData::Undo(Editor *editor)
{
	Editor::GetManipulator(sizeNode)->HandleSizeUpdate(sizeCount, objectSize);
}


ResizeUndoData::ResizeUndoData(Node *node) : UndoData(kUndoResize)
{
	AddNode(node);
}

ResizeUndoData::ResizeUndoData(const List<NodeReference> *referenceList) : UndoData(kUndoResize)
{
	const NodeReference *reference = referenceList->First();
	while (reference)
	{
		AddNode(reference->GetNode());
		reference = reference->Next();
	}
}

ResizeUndoData::~ResizeUndoData()
{
}

ResizeUndoData::ResizedReference::ResizedReference(Node *node)
{
	reference = node;
	transform = node->GetNodeTransform();
	
	const Object *object = node->GetObject();
	if (object) object->GetObjectSize(objectSize);
}

ResizeUndoData::ResizedGeometryReference::ResizedGeometryReference(Geometry *geometry) : ResizedReference(geometry)
{
	const GeometryObject *object = geometry->GetObject();
	int32 levelCount = object->GetGeometryLevelCount();
	
	geometryLevel = new GeometryLevel[levelCount];
	for (machine a = 0; a < levelCount; a++) geometryLevel[a].CopyGeometryLevel(object->GetGeometryLevel(a));
}

ResizeUndoData::ResizedGeometryReference::~ResizedGeometryReference()
{
	delete[] geometryLevel;
}

ResizeUndoData::ResizedMeshReference::ResizedMeshReference(MeshGeometry *mesh) : ResizedGeometryReference(mesh)
{
	const MeshGeometryObject *object = mesh->GetObject();
	boundingSphere = *object->GetBoundingSphere();
	boundingBox = object->GetBoundingBox();
}

ResizeUndoData::ResizedPortalReference::ResizedPortalReference(Portal *portal) : ResizedReference(portal)
{
	const PortalObject *object = portal->GetObject();
	const Point3D *vertex = object->GetVertexArray();
	
	int32 count = object->GetVertexCount();
	for (machine a = 0; a < count; a++) portalVertex[a] = vertex[a];
}

ResizeUndoData::ResizedPolygonZoneReference::ResizedPolygonZoneReference(PolygonZone *polygon) : ResizedReference(polygon)
{
	const PolygonZoneObject *object = polygon->GetObject();
	const Point3D *vertex = object->GetVertexArray();
	
	int32 count = object->GetVertexCount();
	for (machine a = 0; a < count; a++) zoneVertex[a] = vertex[a];
}

ResizeUndoData::AffectedReference::AffectedReference(Node *node)
{
	reference = node;
	position = node->GetNodePosition();
}

void ResizeUndoData::AddNode(Node *node)
{
	NodeType nodeType = node->GetNodeType();
	if (nodeType != kNodeGroup)
	{
		if (nodeType != kNodeTerrainBlock)
		{
			if (nodeType == kNodeGeometry)
			{
				Geometry *geometry = static_cast<Geometry *>(node);
				if (geometry->GetGeometryType() == kGeometryMesh) resizedList.Append(new ResizedMeshReference(static_cast<MeshGeometry *>(geometry)));
				else resizedList.Append(new ResizedGeometryReference(geometry));
			}
			else if (nodeType == kNodePortal)
			{
				resizedList.Append(new ResizedPortalReference(static_cast<Portal *>(node)));
			}
			else if ((nodeType == kNodeZone) && (static_cast<Zone *>(node)->GetZoneType() == kZonePolygon))
			{
				resizedList.Append(new ResizedPolygonZoneReference(static_cast<PolygonZone *>(node)));
			}
			else
			{
				resizedList.Append(new ResizedReference(node));
			}
			
			node = node->GetFirstSubnode();
			while (node)
			{
				affectedList.Append(new AffectedReference(node));
				node = node->Next();
			}
		}
		else
		{
			resizedList.Append(new ResizedReference(node));
			
			Node *subnode = node->GetFirstSubnode();
			while (subnode)
			{
				if (subnode->GetNodeType() == kNodeGeometry)
				{
					Geometry *geometry = static_cast<Geometry *>(subnode);
					if (geometry->GetGeometryType() == kGeometryTerrain) resizedList.Append(new ResizedGeometryReference(geometry));
				}
				
				subnode = node->GetNextNode(subnode);
			}
		}
	}
}

void ResizeUndoData::Undo(Editor *editor)
{
	const AffectedReference *affected = affectedList.First();
	while (affected)
	{
		Node *node = affected->GetNode();
		node->SetNodePosition(affected->GetPosition());
		node->Invalidate();
		
		affected = affected->Next();
	}
	
	const ResizedReference *resized = resizedList.First();
	while (resized)
	{
		Node *node = resized->GetNode();
		node->SetNodeTransform(resized->GetTransform());
		
		NodeType nodeType = node->GetNodeType();
		if (nodeType == kNodeGeometry)
		{
			const ResizedGeometryReference *resizedGeometry = static_cast<const ResizedGeometryReference *>(resized);
			Geometry *geometry = static_cast<Geometry *>(node);
			GeometryObject *object = geometry->GetObject();
			
			int32 levelCount = object->GetGeometryLevelCount();
			for (machine a = 0; a < levelCount; a++) object->GetGeometryLevel(a)->CopyGeometryLevel(resizedGeometry->GetGeometryLevel(a));
			
			if (geometry->GetGeometryType() == kGeometryMesh)
			{
				const ResizedMeshReference *resizedMesh = static_cast<const ResizedMeshReference *>(resizedGeometry);
				MeshGeometryObject *meshObject = static_cast<MeshGeometryObject *>(object);
				
				meshObject->SetBoundingSphere(resizedMesh->GetBoundingSphere());
				meshObject->SetBoundingBox(resizedMesh->GetBoundingBox());
			}
			else
			{
				object->SetObjectSize(resized->GetObjectSize());
			}
			
			object->BuildCollisionData();
			editor->InvalidateGeometry(geometry);
		}
		else if (nodeType == kNodePortal)
		{
			const Point3D *portalVertex = static_cast<const ResizedPortalReference *>(resized)->GetVertexArray();
			
			PortalObject *portalObject = static_cast<Portal *>(node)->GetObject();
			Point3D *vertex = portalObject->GetVertexArray();
			
			int32 count = portalObject->GetVertexCount();
			for (machine a = 0; a < count; a++) vertex[a] = portalVertex[a];
		}
		else if (nodeType == kNodeZone)
		{
			Object *object = node->GetObject();
			object->SetObjectSize(resized->GetObjectSize());
			
			if (static_cast<Zone *>(node)->GetZoneType() == kZonePolygon)
			{
				const Point3D *zoneVertex = static_cast<const ResizedPolygonZoneReference *>(resized)->GetVertexArray();
				
				PolygonZoneObject *polygonObject = static_cast<PolygonZoneObject *>(object);
				Point3D *vertex = polygonObject->GetVertexArray();
				
				int32 count = polygonObject->GetVertexCount();
				for (machine a = 0; a < count; a++) vertex[a] = zoneVertex[a];
			}
		}
		else
		{
			Object *object = node->GetObject();
			if (object) object->SetObjectSize(resized->GetObjectSize());
			
			if (nodeType == kNodeEffect) static_cast<Effect *>(node)->UpdateEffectGeometry();
		}
		
		node->Invalidate();
		
		resized = resized->Next();
	}
}


PasteUndoData::PasteUndoData(const List<NodeReference> *referenceList) : UndoData(kUndoPaste)
{
	const NodeReference *reference = referenceList->First();
	while (reference)
	{
		pastedList.Append(new NodeReference(reference->GetNode()));
		reference = reference->Next();
	}
}

PasteUndoData::~PasteUndoData()
{
}

void PasteUndoData::Undo(Editor *editor)
{
	const NodeReference *pasted = pastedList.First();
	while (pasted)
	{
		editor->DeleteNode(pasted->GetNode());
		pasted = pasted->Next();
	}
}


DeleteUndoData::DeleteUndoData(const List<NodeReference> *referenceList) : UndoData(kUndoDelete)
{
	const NodeReference *reference = referenceList->First();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetSuperNode())
		{
			affectedList.Append(new AffectedReference(node, true));
			
			node = node->GetFirstSubnode();
			while (node)
			{
				if ((!(node->GetNodeFlags() & kNodeNonpersistent)) && (!node->GetManipulator()->Selected()))
				{
					affectedList.Append(new AffectedReference(node, false));
				}
				
				node = node->Next();
			}
		}
		
		reference = reference->Next();
	}
}

DeleteUndoData::~DeleteUndoData()
{
	AffectedReference *affected = affectedList.First();
	while (affected)
	{
		if (affected->GetDeletedFlag()) delete affected->GetNode();
		affected = affected->Next();
	}
}

DeleteUndoData::AffectedReference::AffectedReference(Node *node, bool deleted)
{
	reference = node;
	superNode = node->GetSuperNode();
	nodeTransform = node->GetNodeTransform();
	
	deletedFlag = deleted;
	if (deleted)
	{
		for (;;)
		{
			Link<Node> *link = node->GetFirstLink();
			if (!link) break;
			
			linkArray.AddElement(link);
			*link = nullptr;
		}
		
		const Hub *hub = node->GetHub();
		if (hub)
		{
			Connector *connector = hub->GetFirstOutgoingEdge();
			while (connector)
			{
				outgoingConnectorArray.AddElement(OutgoingConnectorData(connector));
				connector = connector->GetNextOutgoingEdge();
			}
			
			connector = hub->GetFirstIncomingEdge();
			while (connector)
			{
				incomingConnectorArray.AddElement(IncomingConnectorData(connector));
				connector = connector->GetNextIncomingEdge();
			}
		}
	}
}

DeleteUndoData::AffectedReference::~AffectedReference()
{
}

void DeleteUndoData::Undo(Editor *editor)
{
	const AffectedReference *affected = affectedList.First();
	while (affected)
	{
		Node *node = affected->GetNode();
		if (!affected->GetDeletedFlag()) node->Neutralize();
		
		node->SetNodeTransform(affected->GetNodeTransform());
		
		Node *super = affected->GetSuperNode();
		super->AddSubnode(node);
		
		const Array<Link<Node> *>& linkArray = affected->GetLinkArray();
		int32 count = linkArray.GetElementCount();
		for (machine a = 0; a < count; a++) *linkArray[a] = node;
		
		const Array<OutgoingConnectorData>& outgoingConnectorArray = affected->GetOutgoingConnectorArray();
		count = outgoingConnectorArray.GetElementCount();
		for (machine a = 0; a < count; a++)
		{
			const OutgoingConnectorData *data = &outgoingConnectorArray[a];
			data->outgoingConnector->SetConnectorTarget(data->targetNode);
		}
		
		Editor::GetManipulator(node)->HandleConnectorUpdate();
		
		const Array<IncomingConnectorData>& incomingConnectorArray = affected->GetIncomingConnectorArray();
		count = incomingConnectorArray.GetElementCount();
		for (machine a = 0; a < count; a++)
		{
			const IncomingConnectorData *data = &incomingConnectorArray[a];
			const ConnectorKey& key = data->connectorKey;
			
			Node *connectorNode = data->connectorNode;
			if (!connectorNode->SetConnectedNode(key, node)) connectorNode->AddConnector(key, node);
			
			Editor::GetManipulator(connectorNode)->HandleConnectorUpdate();
		}
		
		Editor::GetManipulator(node)->InvalidateGraph();
		
		affected = affected->Next();
	}
	
	affected = affectedList.First();
	while (affected)
	{
		if (affected->GetDeletedFlag())
		{
			Node *node = affected->GetNode();
			node->Preprocess();
			
			Editor::GetManipulator(node)->HandleUndelete();
		}
		
		affected = affected->Next();
	}
	
	affectedList.Purge();
	
	const NodeReference *gizmoTarget = editor->GetGizmoTarget();
	if (gizmoTarget) editor->PostEvent(GizmoEditorEvent(kEditorEventGizmoTargetModified, gizmoTarget->GetNode()));
}


GroupUndoData::GroupUndoData() : UndoData(kUndoGroup)
{
}

GroupUndoData::~GroupUndoData()
{
}

void GroupUndoData::Undo(Editor *editor)
{
	const NodeReference *reference = groupList.First();
	while (reference)
	{
		Node *group = reference->GetNode();
		Node *superNode = group->GetSuperNode();
		
		for (;;)
		{
			Node *node = group->GetFirstSubnode();
			if (!node) break;
			
			superNode->AddSubnode(node);
			static_cast<EditorManipulator *>(node->GetManipulator())->InvalidateGraph();
		}
		
		editor->DeleteNode(group);
		reference = reference->Next();
	}
	
	editor->GetRootNode()->Update();
}


ConnectUndoData::ConnectUndoData(const List<EditorManipulator> *manipulatorList) : UndoData(kUndoConnect)
{
	EditorManipulator *manipulator = manipulatorList->First();
	while (manipulator)
	{
		connectedList.Append(new ConnectedReference(manipulator->GetTargetNode()));
		manipulator = manipulator->Next();
	}
}

ConnectUndoData::ConnectUndoData(const List<NodeReference> *referenceList) : UndoData(kUndoConnect)
{
	const NodeReference *reference = referenceList->First();
	while (reference)
	{
		connectedList.Append(new ConnectedReference(reference->GetNode()));
		reference = reference->Next();
	}
}

ConnectUndoData::~ConnectUndoData()
{
}

ConnectUndoData::ConnectedReference::ConnectedReference(Node *node)
{
	reference = node;
	
	const EditorManipulator *manipulator = Editor::GetManipulator(node);
	int32 count = manipulator->GetConnectorCount();
	
	for (machine a = 0; a < count; a++) connectorArray.AddElement(ConnectorData(a, manipulator->GetConnectorTarget(a)));
}

ConnectUndoData::ConnectedReference::~ConnectedReference()
{
}

void ConnectUndoData::Undo(Editor *editor)
{
	const ConnectedReference *reference = connectedList.First();
	while (reference)
	{
		Node *node = reference->GetNode();
		EditorManipulator *manipulator = Editor::GetManipulator(node);
		
		const Array<ConnectorData>& connectorArray = reference->GetConnectorArray();
		int32 count = connectorArray.GetElementCount();
		for (machine a = 0; a < count; a++)
		{
			const ConnectorData *data = &connectorArray[a];
			manipulator->SetConnectorTarget(data->connectorIndex, data->targetNode);
		}
		
		node->ProcessInternalConnectors();
		node->Invalidate();
		
		reference = reference->Next();
	}
}


ReparentUndoData::ReparentUndoData() : UndoData(kUndoReparent)
{
}

ReparentUndoData::ReparentUndoData(const List<NodeReference> *referenceList) : UndoData(kUndoReparent)
{
	const NodeReference *reference = referenceList->First();
	while (reference)
	{
		AddNode(reference->GetNode());
		reference = reference->Next();
	}
}

ReparentUndoData::~ReparentUndoData()
{
}

ReparentUndoData::MovedReference::MovedReference(Node *node)
{
	reference = node;
	superNode = node->GetSuperNode();
	owningZone = node->GetOwningZone();
	transform = node->GetNodeTransform();
}

void ReparentUndoData::AddNode(Node *node)
{
	movedList.Append(new MovedReference(node));
}

void ReparentUndoData::Undo(Editor *editor)
{
	const MovedReference *moved = movedList.Last();
	while (moved)
	{
		Node *node = moved->GetNode();
		Zone *oldZone = node->GetOwningZone();
		Zone *newZone = moved->GetOwningZone();
		
		if (newZone != oldZone) node->Neutralize();
		
		node->SetNodeTransform(moved->GetTransform());
		moved->GetSuperNode()->AddSubnode(node);
		
		if (newZone != oldZone) node->Preprocess();
		
		node->Invalidate();
		static_cast<EditorManipulator *>(node->GetManipulator())->InvalidateGraph();
		
		moved = moved->Previous();
	}
	
	editor->GetRootNode()->Update();
}


ZoneVertexUndoData::ZoneVertexUndoData(PolygonZone *polygon) : UndoData(kUndoZoneVertex)
{
	zoneNode = polygon;
	
	const PolygonZoneObject *object = polygon->GetObject();
	const Point3D *vertex = object->GetVertexArray();
	
	int32 count = object->GetVertexCount();
	zoneVertexCount = count;
	
	for (machine a = 0; a < count; a++) zoneVertex[a] = vertex[a];
}

ZoneVertexUndoData::~ZoneVertexUndoData()
{
}

void ZoneVertexUndoData::Undo(Editor *editor)
{
	PolygonZoneObject *object = zoneNode->GetObject();
	Point3D *vertex = object->GetVertexArray();
	
	int32 count = zoneVertexCount;
	object->SetVertexCount(count);
	
	for (machine a = 0; a < count; a++) vertex[a] = zoneVertex[a];
	zoneNode->Invalidate();
}


PortalVertexUndoData::PortalVertexUndoData(Portal *portal) : UndoData(kUndoPortalVertex)
{
	portalNode = portal;
	
	const PortalObject *object = portal->GetObject();
	const Point3D *vertex = object->GetVertexArray();
	
	int32 count = object->GetVertexCount();
	portalVertexCount = count;
	
	for (machine a = 0; a < count; a++) portalVertex[a] = vertex[a];
}

PortalVertexUndoData::~PortalVertexUndoData()
{
}

void PortalVertexUndoData::Undo(Editor *editor)
{
	PortalObject *object = portalNode->GetObject();
	Point3D *vertex = object->GetVertexArray();
	
	int32 count = portalVertexCount;
	object->SetVertexCount(count);
	
	for (machine a = 0; a < count; a++) vertex[a] = portalVertex[a];
	portalNode->Invalidate();
}


MaterialUndoData::MaterialUndoData(const List<NodeReference> *referenceList) : UndoData(kUndoMaterial)
{
	const NodeReference *reference = referenceList->First();
	while (reference)
	{
		Node *node = reference->GetNode();
		NodeType type = node->GetNodeType();
		
		if (type == kNodeGeometry)
		{
			geometryList.Append(new GeometryReference(static_cast<Geometry *>(node)));
		}
		else if (type == kNodeSkybox)
		{
			skyboxList.Append(new SkyboxReference(static_cast<Skybox *>(node)));
		}
		else if (type == kNodeImpostor)
		{
			impostorList.Append(new ImpostorReference(static_cast<Impostor *>(node)));
		}
		else if (type == kNodeEffect)
		{
			Effect *effect = static_cast<Effect *>(node);
			if (effect->GetEffectType() == kEffectParticleSystem)
			{
				particleSystemList.Append(new ParticleSystemReference(static_cast<ParticleSystem *>(effect)));
			}
		}
		else if (type == kNodeInstance)
		{
			Instance *instance = static_cast<Instance *>(node);
			
			Modifier *modifier = instance->GetFirstModifier();
			while (modifier)
			{
				if (modifier->GetModifierType() == kModifierReplaceMaterial)
				{
					replaceMaterialModifierList.Append(new ReplaceMaterialModifierReference(instance, static_cast<ReplaceMaterialModifier *>(modifier)));
				}
				
				modifier = modifier->Next();
			}
		}
		
		reference = reference->Next();
	}
}

MaterialUndoData::~MaterialUndoData()
{
}

MaterialUndoData::GeometryReference::GeometryReference(Geometry *geometry)
{
	reference = geometry;
	
	materialCount = geometry->GetMaterialCount();
	
	const GeometryObject *object = geometry->GetObject();
	int32 surfaceCount = object->GetSurfaceCount();
	
	materialStorage = new char[materialCount * sizeof(MaterialObject *) + object->GetSurfaceCount() * 4];
	materialObject = reinterpret_cast<MaterialObject **>(materialStorage);
	materialIndex = reinterpret_cast<unsigned_int32 *>(materialObject + materialCount);
	
	for (machine a = 0; a < materialCount; a++)
	{
		MaterialObject *material = geometry->GetMaterialObject(a);
		materialObject[a] = material;
		if (material) material->Retain();
	}
	
	for (machine a = 0; a < surfaceCount; a++) materialIndex[a] = object->GetSurfaceData(a)->materialIndex;
}

MaterialUndoData::GeometryReference::~GeometryReference()
{
	for (machine a = 0; a < materialCount; a++)
	{
		MaterialObject *material = materialObject[a];
		if (material) material->Release();	
	}
	
	delete[] materialStorage;
}

MaterialUndoData::SkyboxReference::SkyboxReference(Skybox *skybox)
{
	reference = skybox;
	
	materialObject = skybox->GetMaterialObject();
	if (materialObject) materialObject->Retain();
}

MaterialUndoData::SkyboxReference::~SkyboxReference()
{
	if (materialObject) materialObject->Release();
}

MaterialUndoData::ImpostorReference::ImpostorReference(Impostor *impostor)
{
	reference = impostor;
	
	materialObject = impostor->GetMaterialObject();
	if (materialObject) materialObject->Retain();
}

MaterialUndoData::ImpostorReference::~ImpostorReference()
{
	if (materialObject) materialObject->Release();
}

MaterialUndoData::ParticleSystemReference::ParticleSystemReference(ParticleSystem *particleSystem)
{
	reference = particleSystem;
	
	materialObject = particleSystem->GetMaterialObject();
	if (materialObject) materialObject->Retain();
}

MaterialUndoData::ParticleSystemReference::~ParticleSystemReference()
{
	if (materialObject) materialObject->Release();
}

MaterialUndoData::ReplaceMaterialModifierReference::ReplaceMaterialModifierReference(Instance *node, ReplaceMaterialModifier *replaceMaterialModifier)
{
	instance = node;
	reference = replaceMaterialModifier;
	
	materialObject = replaceMaterialModifier->GetMaterialObject();
	if (materialObject) materialObject->Retain();
}

MaterialUndoData::ReplaceMaterialModifierReference::~ReplaceMaterialModifierReference()
{
	if (materialObject) materialObject->Release();
}

void MaterialUndoData::Undo(Editor *editor)
{
	const GeometryReference *geometryReference = geometryList.First();
	while (geometryReference)
	{
		Geometry *geometry = geometryReference->GetGeometry();
		GeometryObject *object = geometry->GetObject();
		int32 materialCount = geometryReference->GetMaterialCount();
		
		if (object->GetReferenceCount() == 1)
		{
			geometry->SetMaterialCount(materialCount);
			for (machine a = 0; a < materialCount; a++) geometry->SetMaterialObject(a, geometryReference->GetMaterialObject(a));
			
			int32 surfaceCount = object->GetSurfaceCount();
			for (machine a = 0; a < surfaceCount; a++) object->GetSurfaceData(a)->materialIndex = geometryReference->GetMaterialIndex(a);
			
			geometry->OptimizeMaterials();
		}
		else
		{
			for (machine a = 0; a < materialCount; a++) geometry->SetMaterialObject(a, geometryReference->GetMaterialObject(a));
			geometry->InvalidateShaderData();
		}
		
		geometryReference = geometryReference->Next();
	}
	
	const SkyboxReference *skyboxReference = skyboxList.First();
	while (skyboxReference)
	{
		Skybox *skybox = skyboxReference->GetSkybox();
		skybox->SetMaterialObject(skyboxReference->GetMaterialObject());
		skybox->InvalidateShaderData();
		
		skyboxReference = skyboxReference->Next();
	}
	
	const ImpostorReference *impostorReference = impostorList.First();
	while (impostorReference)
	{
		Impostor *impostor = impostorReference->GetImpostor();
		impostor->SetMaterialObject(impostorReference->GetMaterialObject());
		
		impostorReference = impostorReference->Next();
	}
	
	const ParticleSystemReference *particleSystemReference = particleSystemList.First();
	while (particleSystemReference)
	{
		ParticleSystem *particleSystem = particleSystemReference->GetParticleSystem();
		particleSystem->SetMaterialObject(particleSystemReference->GetMaterialObject());
		
		particleSystemReference = particleSystemReference->Next();
	}
	
	const ReplaceMaterialModifierReference *replaceMaterialModifierReference = replaceMaterialModifierList.First();
	while (replaceMaterialModifierReference)
	{
		ReplaceMaterialModifier *replaceMaterialModifier = replaceMaterialModifierReference->GetReplaceMaterialModifier();
		replaceMaterialModifier->SetMaterialObject(replaceMaterialModifierReference->GetMaterialObject());
		
		if (editor->GetEditorObject()->GetEditorFlags() & kEditorExpandWorlds)
		{
			InstanceManipulator *manipulator = static_cast<InstanceManipulator *>(Editor::GetManipulator(replaceMaterialModifierReference->GetInstance()));
			manipulator->CollapseWorld();
			manipulator->ExpandWorld();
			editor->InvalidateAllViewports();
		}
		
		replaceMaterialModifierReference = replaceMaterialModifierReference->Next();
	}
}


GeometryUndoData::GeometryUndoData(Geometry *geometry) : UndoData(kUndoGeometry)
{
	AddGeometry(geometry);
}

GeometryUndoData::GeometryUndoData(const List<NodeReference> *referenceList, GeometryType filter) : UndoData(kUndoGeometry)
{
	const NodeReference *reference = referenceList->First();
	while (reference)
	{
		Node *node = reference->GetNode();
		
		if (node->GetNodeType() == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			if ((filter == 0) || (geometry->GetGeometryType() == filter)) AddGeometry(geometry);
		}
		
		reference = reference->Next();
	}
}

GeometryUndoData::~GeometryUndoData()
{
}

GeometryUndoData::GeometryReference::GeometryReference(Geometry *geometry)
{
	reference = geometry;
	transform = geometry->GetNodeTransform();
	
	const GeometryObject *object = geometry->GetObject();
	if (object->GetGeometryType() == kGeometryPrimitive) primitiveFlags = static_cast<const PrimitiveGeometryObject *>(object)->GetPrimitiveFlags();
	
	int32 levelCount = object->GetGeometryLevelCount();
	geometryLevelCount = levelCount;
	collisionLevel = object->GetCollisionLevel();
	
	geometryLevel = new GeometryLevel[levelCount];
	for (machine a = 0; a < levelCount; a++) geometryLevel[a].CopyGeometryLevel(object->GetGeometryLevel(a));
}

GeometryUndoData::GeometryReference::~GeometryReference()
{
	delete[] geometryLevel;
}

GeometryUndoData::TerrainReference::TerrainReference(TerrainGeometry *terrain) : GeometryReference(terrain)
{
	const TerrainGeometryObject *object = terrain->GetObject();
	if (object->GetDetailLevel() != 0) static_cast<const TerrainLevelGeometryObject *>(object)->SaveBorderRenderData(&borderRenderData);
}

GeometryUndoData::MovedReference::MovedReference(Node *node)
{
	reference = node;
	transform = node->GetNodeTransform();
}

void GeometryUndoData::AddGeometry(Geometry *geometry)
{
	if (geometry->GetGeometryType() != kGeometryTerrain) geometryList.Append(new GeometryReference(geometry));
	else geometryList.Append(new TerrainReference(static_cast<TerrainGeometry *>(geometry)));
	
	Node *subnode = geometry->GetFirstSubnode();
	while (subnode)
	{
		movedList.Append(new MovedReference(subnode));
		subnode = subnode->Next();
	}
}

void GeometryUndoData::Undo(Editor *editor)
{
	const MovedReference *moved = movedList.First();
	while (moved)
	{
		moved->GetNode()->SetNodeTransform(moved->GetTransform());
		moved = moved->Next();
	}
	
	const GeometryReference *reference = geometryList.First();
	while (reference)
	{
		Geometry *geometry = reference->GetGeometry();
		geometry->SetNodeTransform(reference->GetTransform());
		
		GeometryObject *object = geometry->GetObject();
		GeometryType type = object->GetGeometryType();
		if (type == kGeometryPrimitive)
		{
			static_cast<PrimitiveGeometryObject *>(object)->SetPrimitiveFlags(reference->GetPrimitiveFlags());
		}
		else if (type == kGeometryTerrain)
		{
			TerrainGeometryObject *terrainObject = static_cast<TerrainGeometryObject *>(object);
			if (terrainObject->GetDetailLevel() != 0)
			{
				const TerrainReference *terrainReference = static_cast<const TerrainReference *>(reference);
				static_cast<TerrainLevelGeometryObject *>(terrainObject)->RestoreBorderRenderData(terrainReference->GetBorderRenderData());
			}
		}
		
		int32 levelCount = reference->GetGeometryLevelCount();
		object->SetGeometryLevelCount(levelCount);
		for (machine a = 0; a < levelCount; a++) object->GetGeometryLevel(a)->CopyGeometryLevel(reference->GetGeometryLevel(a));
		
		if (type == kGeometryMesh) static_cast<MeshGeometryObject *>(object)->UpdateBounds();
		
		object->SetCollisionLevel(reference->GetCollisionLevel());
		object->BuildCollisionData();
		
		editor->InvalidateGeometry(geometry);
		
		reference = reference->Next();
	}
}


TextureUndoData::TextureUndoData(Geometry *geometry) : UndoData(kUndoGeometry)
{
	geometryList.Append(new GeometryReference(geometry));
}

TextureUndoData::TextureUndoData(const List<NodeReference> *referenceList) : UndoData(kUndoGeometry)
{
	const NodeReference *reference = referenceList->First();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeGeometry) geometryList.Append(new GeometryReference(static_cast<Geometry *>(node)));
		
		reference = reference->Next();
	}
}

TextureUndoData::~TextureUndoData()
{
}

TextureUndoData::GeometryReference::GeometryReference(Geometry *geometry)
{
	reference = geometry;
	
	const GeometryObject *object = geometry->GetObject();
	int32 levelCount = object->GetGeometryLevelCount();
	
	int32 texcoordCount = 0;
	for (machine a = 0; a < levelCount; a++)
	{
		const GeometryLevel *level = object->GetGeometryLevel(a);
		if (level->GetArray<Point2D>(kArrayTexture0)) texcoordCount += level->GetVertexCount();
	}
	
	int32 surfaceCount = object->GetSurfaceCount();
	
	textureStorage = new char[texcoordCount * sizeof(Point2D) + surfaceCount * sizeof(TextureAlignData) * 2];
	texcoordArray = reinterpret_cast<Point2D *>(textureStorage);
	textureAlignData = reinterpret_cast<TextureAlignData *>(texcoordArray + texcoordCount);
	
	texcoordCount = 0;
	for (machine a = 0; a < levelCount; a++)
	{
		const GeometryLevel *level = object->GetGeometryLevel(a);
		const Point2D *texcoord = level->GetArray<Point2D>(kArrayTexture0);
		if (texcoord)
		{
			int32 vertexCount = level->GetVertexCount();
			MemoryMgr::CopyMemory(texcoord, &texcoordArray[texcoordCount], vertexCount * sizeof(Point2D));
			texcoordCount += vertexCount;
		}
	}
	
	for (machine a = 0; a < surfaceCount; a++)
	{
		const SurfaceData *data = object->GetSurfaceData(a);
		textureAlignData[a * 2] = data->textureAlignData[0];
		textureAlignData[a * 2 + 1] = data->textureAlignData[1];
	}
}

TextureUndoData::GeometryReference::~GeometryReference()
{
	delete[] textureStorage;
}

void TextureUndoData::Undo(Editor *editor)
{
	const NodeReference *gizmoTarget = editor->GetGizmoTarget();
	const Node *gizmoNode = (gizmoTarget) ? gizmoTarget->GetNode() : nullptr;
	
	const GeometryReference *reference = geometryList.First();
	while (reference)
	{
		Geometry *geometry = reference->GetGeometry();
		GeometryObject *object = geometry->GetObject();
		int32 levelCount = object->GetGeometryLevelCount();
		
		int32 texcoordCount = 0;
		const Point2D *texcoordArray = reference->GetTexcoordArray();
		
		for (machine a = 0; a < levelCount; a++)
		{
			GeometryLevel *level = object->GetGeometryLevel(a);
			Point2D *texcoord = level->GetArray<Point2D>(kArrayTexture0);
			if (texcoord)
			{
				int32 vertexCount = level->GetVertexCount();
				MemoryMgr::CopyMemory(&texcoordArray[texcoordCount], texcoord, vertexCount * sizeof(Point2D));
				texcoordCount += vertexCount;
				
				level->CalculateTangentArray();
			}
		}
		
		geometry->Neutralize();
		geometry->Preprocess();
		
		int32 surfaceCount = object->GetSurfaceCount();
		const TextureAlignData *textureAlignData = reference->GetTextureAlignData();
		
		for (machine a = 0; a < surfaceCount; a++)
		{
			SurfaceData *data = object->GetSurfaceData(a);
			data->textureAlignData[0] = textureAlignData[a * 2];
			data->textureAlignData[1] = textureAlignData[a * 2 + 1];
		}
		
		editor->PostEvent(NodeEditorEvent(kEditorEventTexcoordModified, geometry));
		if (geometry == gizmoNode) editor->PostEvent(GizmoEditorEvent(kEditorEventGizmoTargetModified, geometry));
		
		reference = reference->Next();
	}
}


PaintUndoData::PaintUndoData(const PaintSpaceObject *object, const Painter *painter) : UndoData(kUndoPaint)
{
	paintSpaceObject = object;
	paintBounds = painter->GetPaintBounds();
	
	undoImage = painter->CreateUndoImage(paintBounds);
}

PaintUndoData::~PaintUndoData()
{
	Painter::ReleaseUndoImage(undoImage);
}

void PaintUndoData::Undo(Editor *editor)
{
	Painter::ApplyUndoImage(paintSpaceObject, paintBounds, undoImage);
	paintSpaceObject->GetPaintTexture()->Update(paintBounds);
	editor->InvalidateFrustumViewports();
}


PathUndoData::PathUndoData(PathMarker *marker) : UndoData(kUndoPath)
{
	pathList.Append(new PathReference(marker));
}

PathUndoData::PathUndoData(const List<NodeReference> *referenceList) : UndoData(kUndoPath)
{
	const NodeReference *reference = referenceList->First();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeMarker)
		{
			Marker *marker = static_cast<Marker *>(node);
			if (marker->GetMarkerType() == kMarkerPath) pathList.Append(new PathReference(static_cast<PathMarker *>(marker)));
		}
		
		reference = reference->Next();
	}
}

PathUndoData::~PathUndoData()
{
}

PathUndoData::PathReference::PathReference(PathMarker *marker) : path(*marker->GetPath())
{
	reference = marker;
}

PathUndoData::PathReference::~PathReference()
{
}

void PathUndoData::Undo(Editor *editor)
{
	const PathReference *reference = pathList.First();
	while (reference)
	{
		PathMarker *marker = reference->GetPathMarker();
		*marker->GetPath() = *reference->GetPath();
		marker->Invalidate();
		
		reference = reference->Next();
	}
}


TubeEffectUndoData::TubeEffectUndoData(const List<NodeReference> *referenceList) : UndoData(kUndoTubeEffect)
{
	const NodeReference *reference = referenceList->First();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeEffect)
		{
			Effect *effect = static_cast<Effect *>(node);
			if (effect->GetEffectType() == kEffectTube) tubeList.Append(new TubeReference(static_cast<TubeEffect *>(effect)));
		}
		
		reference = reference->Next();
	}
}

TubeEffectUndoData::~TubeEffectUndoData()
{
}

TubeEffectUndoData::TubeReference::TubeReference(TubeEffect *tube) : path(*tube->GetObject()->GetTubePath())
{
	reference = tube;
}

TubeEffectUndoData::TubeReference::~TubeReference()
{
}

void TubeEffectUndoData::Undo(Editor *editor)
{
	const TubeReference *reference = tubeList.First();
	while (reference)
	{
		TubeEffect *tube = reference->GetTubeEffect();
		TubeEffectObject *object = tube->GetObject();
		object->SetTubePath(reference->GetPath());
		object->Build();
		
		tube->Invalidate();
		tube->Neutralize();
		tube->Preprocess();
		
		reference = reference->Next();
	}
}


ReplaceWorldUndoData::ReplaceWorldUndoData(const List<NodeReference> *referenceList) : UndoData(kUndoReplaceWorld)
{
	const NodeReference *reference = referenceList->First();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeInstance)
		{
			worldList.Append(new WorldReference(static_cast<Instance *>(node)));
		}
		
		reference = reference->Next();
	}
}

ReplaceWorldUndoData::~ReplaceWorldUndoData()
{
}

ReplaceWorldUndoData::WorldReference::WorldReference(Instance *instance)
{
	reference = instance;
	worldName = instance->GetWorldName();
}

ReplaceWorldUndoData::WorldReference::~WorldReference()
{
}

void ReplaceWorldUndoData::Undo(Editor *editor)
{
	const WorldReference *reference = worldList.First();
	while (reference)
	{
		Instance *instance = reference->GetInstance();
		instance->Collapse();
		instance->SetWorldName(reference->GetWorldName());
		editor->ExpandWorld(instance);
		
		reference = reference->Next();
	}
}


AssociatePaintSpaceUndoData::AssociatePaintSpaceUndoData(const List<NodeReference> *referenceList) : UndoData(kUndoAssociatePaintSpace)
{
	const NodeReference *reference = referenceList->First();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeGeometry)
		{
			geometryList.Append(new GeometryReference(static_cast<Geometry *>(node)));
		}
		
		reference = reference->Next();
	}
}

AssociatePaintSpaceUndoData::~AssociatePaintSpaceUndoData()
{
}

AssociatePaintSpaceUndoData::GeometryReference::GeometryReference(Geometry *geometry)
{
	reference = geometry;
	paintSpace = geometry->GetConnectedPaintSpace();
}

AssociatePaintSpaceUndoData::GeometryReference::~GeometryReference()
{
}

void AssociatePaintSpaceUndoData::Undo(Editor *editor)
{
	const GeometryReference *reference = geometryList.First();
	while (reference)
	{
		Geometry *geometry = reference->GetGeometry();
		geometry->SetConnectedPaintSpace(reference->GetPaintSpace());
		
		reference = reference->Next();
	}
}


NodeInfoUndoData::NodeInfoUndoData(const List<NodeReference> *referenceList) : UndoData(kUndoNodeInfo)
{
	const NodeReference *reference = referenceList->First();
	while (reference)
	{
		nodeList.Append(new NodeInfoReference(reference->GetNode()));
		reference = reference->Next();
	}
}

NodeInfoUndoData::~NodeInfoUndoData()
{
}

NodeInfoUndoData::NodeInfoReference::NodeInfoReference(Node *node) :
		nodePackage(nullptr),
		objectPackage(nullptr),
		propertyPackage(nullptr)
{
	reference = node;
	
	Packer nodePacker(&nodePackage);
	node->Pack(nodePacker, kPackEditor | kPackSettings);
	
	const Object *object = node->GetObject();
	if (object)
	{
		Packer objectPacker(&objectPackage);
		object->Pack(objectPacker, kPackEditor | kPackSettings);
	}
	
	const PropertyObject *propertyObject = node->GetPropertyObject();
	if (propertyObject)
	{
		Packer propertyPacker(&propertyPackage);
		propertyObject->Pack(propertyPacker, kPackEditor | kPackSettings);
		
		const Node *root = node->GetRootNode();
		Node *subnode = root->GetFirstSubnode();
		while (subnode)
		{
			const PropertyObject *object = subnode->GetPropertyObject();
			if ((object == propertyObject) && (subnode != node)) propertyObjectList.Append(new NodeReference(subnode));
			subnode = root->GetNextNode(subnode);
		}
	}
	
	const Hub *hub = node->GetHub();
	if (hub)
	{
		Connector *connector = hub->GetFirstOutgoingEdge();
		while (connector)
		{
			connectorArray.AddElement(ConnectorData(connector));
			connector = connector->GetNextOutgoingEdge();
		}
	}
}

NodeInfoUndoData::NodeInfoReference::~NodeInfoReference()
{
}

void NodeInfoUndoData::Undo(Editor *editor)
{
	const NodeReference *gizmoReference = editor->GetGizmoTarget();
	Node *gizmoTarget = (gizmoReference) ? gizmoReference->GetNode() : nullptr;
	bool gizmoEvent = false;
	
	const NodeInfoReference *reference = nodeList.First();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node == gizmoTarget) gizmoEvent = true;
		
		EditorManipulator *manipulator = Editor::GetManipulator(node);
		manipulator->UnselectConnector();
		
		node->Neutralize();
		
		void *cookie = node->BeginSettingsUnpack();
		Unpacker nodeUnpacker(reference->GetNodePackage()->GetStorage(), 1, kWorldVersion);
		node->Unpack(nodeUnpacker, kUnpackEditor);
		node->EndSettingsUnpack(cookie);
		
		Object *object = node->GetObject();
		if (object)
		{
			cookie = object->BeginSettingsUnpack();
			Unpacker objectUnpacker(reference->GetObjectPackage()->GetStorage(), 1, kWorldVersion);
			object->Unpack(objectUnpacker, kUnpackEditor);
			object->EndSettingsUnpack(cookie);
		}
		
		PropertyObject *propertyObject = node->GetPropertyObject();
		const Package *propertyPackage = reference->GetPropertyPackage();
		if (propertyPackage->GetSize() != 0)
		{
			if (propertyObject)
			{
				cookie = propertyObject->BeginSettingsUnpack();
				Unpacker propertyUnpacker(propertyPackage->GetStorage(), 1, kWorldVersion);
				propertyObject->Unpack(propertyUnpacker, kUnpackEditor);
				propertyObject->EndSettingsUnpack(cookie);
			}
			else
			{
				propertyObject = new PropertyObject;
				node->SetPropertyObject(propertyObject);
				propertyObject->Release();
				
				const NodeReference *propertyReference = reference->GetFirstPropertyObjectNode();
				while (propertyReference)
				{
					propertyReference->GetNode()->SetPropertyObject(propertyObject);
					propertyReference = propertyReference->Next();
				}
				
				Unpacker propertyUnpacker(propertyPackage->GetStorage(), 1, kWorldVersion);
				propertyObject->Unpack(propertyUnpacker, kUnpackEditor);
			}
		}
		else
		{
			if (propertyObject)
			{
				if (propertyObject->GetReferenceCount() == 1)
				{
					node->SetPropertyObject(nullptr);
				}
				else
				{
					Node *root = editor->GetRootNode();
					Node *subnode = root->GetFirstSubnode();
					while (subnode)
					{
						PropertyObject *object = subnode->GetPropertyObject();
						if (object == propertyObject) subnode->SetPropertyObject(nullptr);
						subnode = root->GetNextNode(subnode);
					}
				}
			}
		}
		
		Hub *hub = node->GetHub();
		if (hub)
		{
			const Array<ConnectorData>& connectorArray = reference->GetConnectorArray();
			int32 count = connectorArray.GetElementCount();
			for (machine a = 0; a < count; a++)
			{
				const ConnectorData *data = &connectorArray[a];
				const ConnectorKey& key = data->connectorKey;
				if (!node->GetConnectedNode(key)) node->SetConnectedNode(key, data->targetNode);
			}
		}
		
		node->Preprocess();
		manipulator->HandleSettingsUpdate();
		
		reference = reference->Next();
	}
	
	if (gizmoEvent) editor->PostEvent(GizmoEditorEvent(kEditorEventGizmoTargetModified, gizmoTarget));
	editor->PostEvent(EditorEvent(kEditorEventNodeInfoModified));
}

// ZYURVUR
