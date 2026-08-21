//=============================================================
//
// C4 Engine version 2.9.1
// Copyright 1999-2012, by Terathon Software LLC
//
// This copy is licensed to the following:
//
//     Registered user: Soo Ki Kim
//     Maximum number of users: 1
//     License #C4T0035002
//
// License is granted under terms of the license agreement
// entered by the registed user.
//
// Unauthorized redistribution of source code is strictly
// prohibited. Violators will be prosecuted.
//
//=============================================================


#include "C4World.h"
#include "C4Manipulator.h"
#include "C4Emitters.h"
#include "C4Terrain.h"
#include "C4Water.h"
#include "C4Skybox.h"
#include "C4Configuration.h"


using namespace C4;


namespace C4
{
	struct ConnectorData
	{
		ConnectorKey	connectorKey;
		Node			*targetNode;
		
		ConnectorData(const ConnectorKey& key, Node *node)
		{
			connectorKey = key;
			targetNode = node;
		}
	};
	
	
	template class Constructable<Node>;
}


void C4::Reverse(WorldHeader *wh)
{
	Reverse(&wh->controllerCount);
	Reverse(&wh->objectCount);
	Reverse(&wh->nodeCount);
	Reverse(&wh->offsetCount);
}


Node::Node(NodeType type) : Site(-1)
{
	nodeType = type;
	nodeFlags = 0;
	nodeHash = 0;
	nodeStamp = 0xFFFFFFFF;
	
	nodeWorld = nullptr;
	nodeManipulator = nullptr;
	nodeController = nullptr;
	nodeObject = nullptr;
	nodeHub = nullptr;
	propertyObject = nullptr;
	
	nodeTransform.SetIdentity();
	previousWorldTransform(3,3) = 0.0F;
	
	boundingSpherePointer = nullptr;
	
	visibilityProc = &SphereVisible;
	occlusionProc = &SphereOccluded;
}

Node::Node(const Node& node) : Site(-1)
{
	nodeType = node.nodeType;
	nodeFlags = node.nodeFlags;
	nodeHash = node.nodeHash;
	nodeStamp = 0xFFFFFFFF;
	
	nodeWorld = nullptr;
	nodeManipulator = nullptr;
	nodeController = nullptr;
	nodeObject = nullptr;
	nodeHub = nullptr;
	
	Object *object = node.nodeObject;
	if (object)
	{
		if (!(nodeFlags & kNodeUnsharedObject))
		{
			share:
			nodeObject = object;
			object->Retain();
		}
		else
		{
			Object *clone = object->Clone();
			if (!clone) goto share;
			nodeObject = clone;
		}
	}
	
	const Controller *controller = node.nodeController;
	if (controller) SetController(controller->Clone());
	
	const Hub *hub = node.nodeHub;
	if (hub)
	{
		nodeHub = new Hub(this);
		
		const Connector *connector = hub->GetFirstOutgoingEdge(); 
		while (connector)
		{ 
			new Connector(nodeHub, *connector); 
			connector = connector->GetNextOutgoingEdge(); 
		}
	} 
	
	const Property *property = node.GetFirstProperty();
	while (property)
	{ 
		Property *clone = property->Clone();
		if (clone) AddProperty(clone);
		
		property = property->Next(); 
	}
	
	propertyObject = node.propertyObject;
	if (propertyObject) propertyObject->Retain();
	
	nodeTransform = node.nodeTransform;
	previousWorldTransform(3,3) = 0.0F;
	
	boundingSpherePointer = nullptr;
	
	visibilityProc = &SphereVisible;
	occlusionProc = &SphereOccluded;
}

Node::~Node()
{
	Zone *zone = GetOwningZone();
	if (zone) ExitZone(zone);
	
	if (propertyObject)
		propertyObject->Release();
	
	if( nodeController)
		delete nodeController;

	if( nodeManipulator )
		delete nodeManipulator;
	
	delete nodeHub;

	if (nodeObject)
		nodeObject->Release();
}

Node *Node::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kNodeGroup:
			
			return (new Node);
		
		case kNodeCamera:
			
			return (Camera::Construct(++data, unpackFlags));
		
		case kNodeLight:
			
			return (Light::Construct(++data, unpackFlags));
		
		case kNodeSource:
			
			return (Source::Construct(++data, unpackFlags));
		
		case kNodeGeometry:
			
			return (Geometry::Construct(++data, unpackFlags));
		
		case kNodeInstance:
			
			return (new Instance);
		
		case kNodeModel:
			
			return (Model::Construct(++data, unpackFlags));
		
		case kNodeBone:
			
			return (new Bone);
		
		case kNodeMarker:
			
			#if C4LEGACY
			
				++data;
				if (data.GetType() == 'RFER') return (new Instance);
				return (Marker::Construct(data, unpackFlags));
			
			#else
			
				return (Marker::Construct(++data, unpackFlags));
			
			#endif
		
		case kNodeTrigger:
			
			return (Trigger::Construct(++data, unpackFlags));
		
		case kNodeEffect:
			
			return (Effect::Construct(++data, unpackFlags));
		
		case kNodeEmitter:
			
			return (Emitter::Construct(++data, unpackFlags));
		
		case kNodeSpace:
			
			return (Space::Construct(++data, unpackFlags));
			
		case kNodePortal:
			
			return (Portal::Construct(++data, unpackFlags));
		
		case kNodeZone:
			
			return (Zone::Construct(++data, unpackFlags));
		
		case kNodeShape:
			
			return (Shape::Construct(++data, unpackFlags));
		
		case kNodeJoint:
			
			return (Joint::Construct(++data, unpackFlags));
		
		case kNodeField:
			
			return (Field::Construct(++data, unpackFlags));
		
		case kNodePhysics:
			
			return (new PhysicsNode);
		
		case kNodeSkybox:
			
			return (new Skybox);
		
		case kNodeImpostor:
			
			return (new Impostor);
		
		case kNodeTerrainBlock:
		
		#if C4LEGACY
		
			case 'BLCK':
		
		#endif
			
			return (new TerrainBlock);
		
		case kNodeWaterBlock:
			
			return (new WaterBlock);
	}
	
	return (Constructable<Node>::Construct(data, unpackFlags));
}

Node *Node::Replicate(void) const
{
	return (new Node(*this));
}

bool Node::DefaultCloneFilter(const Node *node, void *cookie)
{
	return (!(node->GetNodeFlags() & kNodeCloneInhibit));
}

Node *Node::CloneNode(CloneFilterProc *filterProc, void *filterCookie) const
{
	Node *clone = Replicate();
	
	Node *subnode = GetFirstSubnode();
	while (subnode)
	{
		if ((*filterProc)(subnode, filterCookie))
		{
			Node *subclone = subnode->CloneNode(filterProc, filterCookie);
			if (subclone) clone->AddSubnode(subclone);
		}
		
		subnode = subnode->Next();
	}
	
	return (clone);
}

Node *Node::CloneNode(const Node *root, Node **nodeTable, Array<ConnectorCloneData> *connectorArray, CloneFilterProc *filterProc, void *filterCookie) const
{
	Node *clone = Replicate();
	nodeTable[nodeIndex] = clone;
	
	if (nodeHub)
	{
		const Connector *nodeConnector = nodeHub->GetFirstOutgoingEdge();
		if (nodeConnector)
		{
			Connector *cloneConnector = clone->nodeHub->GetFirstOutgoingEdge();
			do
			{
				const Node *target = nodeConnector->GetConnectorTarget();
				if ((target) && ((root == target) || (root->Successor(target))) && ((*filterProc)(target, filterCookie)))
				{
					ConnectorCloneData *data = connectorArray->AddElement();
					data->connector = cloneConnector;
					data->linkIndex = target->GetNodeIndex();
				}
				
				nodeConnector = nodeConnector->GetNextOutgoingEdge();
				cloneConnector = cloneConnector->GetNextOutgoingEdge();
			} while (nodeConnector);
		}
	}
	
	Node *subnode = GetFirstSubnode();
	while (subnode)
	{
		if ((*filterProc)(subnode, filterCookie))
		{
			Node *subclone = subnode->CloneNode(root, nodeTable, connectorArray, filterProc, filterCookie);
			if (subclone) clone->AddSubnode(subclone);
		}
		
		subnode = subnode->Next();
	}
	
	return (clone);
}

Node *Node::Clone(CloneFilterProc *filterProc, void *filterCookie) const
{
	nodeIndex = 0;
	int32 count = 1;
	bool connectorFlag = ((nodeHub) && (nodeHub->HasOutgoingConnection()));
	
	Node *subnode = GetFirstSubnode();
	while (subnode)
	{
		if ((*filterProc)(subnode, filterCookie))
		{
			subnode->nodeIndex = count++;
			const Hub *hub = subnode->nodeHub;
			connectorFlag |= ((hub) && (hub->HasOutgoingConnection()));
			
			subnode = GetNextNode(subnode);
		}
		else
		{
			subnode = GetNextLevelNode(subnode);
		}
	}
	
	if (!connectorFlag) return (CloneNode(filterProc, filterCookie));
	
	Array<ConnectorCloneData> connectorArray(16);
	Node **nodeTable = new Node *[count];
	
	Node *clone = CloneNode(this, nodeTable, &connectorArray, filterProc, filterCookie);
	
	const ConnectorCloneData *data = connectorArray;
	count = connectorArray.GetElementCount();
	for (machine a = 0; a < count; a++)
	{
		data->connector->SetConnectorTarget(nodeTable[data->linkIndex]);
		data++;
	}
	
	delete[] nodeTable;
	return (clone);
}

void Node::CloneSubtree(Node *root) const
{
	int32 count = 0;
	bool connectorFlag = false;
	
	Node *subnode = GetFirstSubnode();
	while (subnode)
	{
		if (DefaultCloneFilter(subnode))
		{
			subnode->nodeIndex = count++;
			const Hub *hub = subnode->nodeHub;
			connectorFlag |= ((hub) && (hub->HasOutgoingConnection()));
			
			subnode = GetNextNode(subnode);
		}
		else
		{
			subnode = GetNextLevelNode(subnode);
		}
	}
	
	if (!connectorFlag)
	{
		subnode = GetFirstSubnode();
		while (subnode)
		{
			if (DefaultCloneFilter(subnode)) root->AddSubnode(subnode->CloneNode());
			subnode = subnode->Next();
		}
	}
	else
	{
		Array<ConnectorCloneData> connectorArray(16);
		Node **nodeTable = new Node *[count];
		
		subnode = GetFirstSubnode();
		while (subnode)
		{
			if (DefaultCloneFilter(subnode)) root->AddSubnode(subnode->CloneNode(this, nodeTable, &connectorArray));
			subnode = subnode->Next();
		}
		
		const ConnectorCloneData *data = connectorArray;
		count = connectorArray.GetElementCount();
		for (machine a = 0; a < count; a++)
		{
			data->connector->SetConnectorTarget(nodeTable[data->linkIndex]);
			data++;
		}
		
		delete[] nodeTable;
	}
}

void Node::Invalidate(void)
{
	UpdatableTree<Node>::Invalidate();
	if (nodeManipulator) nodeManipulator->Invalidate();
	
	Node *node = this;
	for (;;)
	{
		const Bond *bond = node->GetFirstIncomingEdge();
		if (!bond) break;
		
		Site *site = bond->GetStartElement();
		if (site->GetCellIndex() >= 0) break;
		
		node = static_cast<Node *>(site);
		if ((node->GetNodeFlags() & (kNodeVisibilitySite | kNodeIsolatedVisibility)) != kNodeVisibilitySite) break;
		
		node->SetCurrentUpdateFlags(node->GetCurrentUpdateFlags() | kUpdatePostTransform);
		node->PropagateUpdateFlags(kUpdatePostTransform);
	}
}

bool Node::LinkedNodePackable(unsigned_int32 packFlags) const
{
	if (packFlags & kPackSettings) return (false);
	return ((!(packFlags & kPackSelected)) || (GetManipulator()->Selected()));
}

void Node::PackType(Packer& data) const
{
	data << nodeType;
}

void Node::Prepack(List<Object> *linkList) const
{
	if (nodeObject) linkList->Append(nodeObject);
	if (nodeController) nodeController->Prepack(linkList);
	
	const Property *property = propertyMap.First();
	while (property)
	{
		if (!(property->GetPropertyFlags() & kPropertyNonpersistent)) property->Prepack(linkList);
		property = property->Next();
	}
	
	if (propertyObject) linkList->Append(propertyObject);
}

void Node::Pack(Packer& data, unsigned_int32 packFlags) const
{
	data << ChunkHeader('FLAG', 4);
	data << (unsigned_int32) (nodeFlags & kNodeFlagsMask);
	
	data << ChunkHeader('XFRM', sizeof(Transform4D));
	data << nodeTransform;
	
	if (nodeHash != 0)
	{
		data << ChunkHeader('HASH', 4);
		data << nodeHash;
	}
	
	if (nodeController)
	{
		PackHandle handle = data.BeginChunk('CTRL');
		nodeController->PackType(data);
		nodeController->Pack(data, packFlags);
		data.EndChunk(handle);
	}
	
	if (nodeHub)
	{
		const Connector *connector = nodeHub->GetFirstOutgoingEdge();
		while (connector)
		{
			PackHandle handle = data.BeginChunk('CNNC');
			data << connector->GetConnectorKey();
			
			int32 nodeIndex = -1;
			const Node *node = connector->GetConnectorTarget();
			if ((node) && (node->LinkedNodePackable(packFlags)))
			{
				if ((node->GetNodeFlags() & kNodeNonpersistent) && (connector->GetConnectorFlags() & kConnectorSaveFinishPersistent))
				{
					const Node *super = node->GetSuperNode();
					while (super)
					{
						if (!(super->GetNodeFlags() & kNodeNonpersistent))
						{
							node = super;
							break;
						}
						
						super = super->GetSuperNode();
					}
				}
				
				nodeIndex = node->GetNodeIndex();
			}
			
			data << nodeIndex;
			data.EndChunk(handle);
			
			connector = connector->GetNextOutgoingEdge();
		}
	}
	
	const Property *property = propertyMap.First();
	while (property)
	{
		if (!(property->GetPropertyFlags() & kPropertyNonpersistent))
		{
			PackHandle handle = data.BeginChunk('PROP');
			property->PackType(data);
			property->Pack(data, packFlags);
			data.EndChunk(handle);
		}
		
		property = property->Next();
	}
	
	if (!(packFlags & kPackSettings))
	{
		if (propertyObject)
		{
			data << ChunkHeader('POBJ', 4);
			data << propertyObject->GetObjectIndex();
		}
		
		if ((nodeManipulator) && (packFlags & kPackEditor))
		{
			PackHandle handle = data.BeginChunk('MTOR');
			nodeManipulator->Pack(data, packFlags);
			data.EndChunk(handle);
		}
		
		int32 superIndex = -1;
		const Node *super = GetSuperNode();
		
		if (packFlags & kPackSelected)
		{
			while (super)
			{
				if (super->GetManipulator()->Selected())
				{
					superIndex = super->nodeIndex;
					break;
				}
				
				super = super->GetSuperNode();
			}
		}
		else if (super)
		{
			superIndex = super->nodeIndex;
		}
		
		data << ChunkHeader('INDX', 8);
		data << superIndex;
		
		int32 objectIndex = (nodeObject) ? nodeObject->GetObjectIndex() : -1;
		data << objectIndex;
	}
	
	data << TerminatorChunk;
}

void Node::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<Node>(data, unpackFlags);
	if (unpackFlags & kUnpackNonpersistent) nodeFlags |= kNodeNonpersistent;
}

bool Node::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'FLAG':
			
			data >> nodeFlags;
			return (true);
		
		case 'XFRM':
			
			data >> nodeTransform;
			return (true);
		
		case 'HASH':
			
			data >> nodeHash;
			return (true);
		
		case 'CTRL':
		{
			Controller *controller = Controller::Construct(data, unpackFlags);
			if (controller)
			{
				controller->targetNode = this;
				controller->Unpack(++data, unpackFlags);
				SetController(controller);
				return (true);
			}
			
			break;
		}
		
		case 'CNNC':
		{
			ConnectorKey	key;
			int32			index;
			
			data >> key;
			data >> index;
			
			if (!nodeHub) new Hub(this);
			Connector *connector = new Connector(nodeHub, key);
			data.AddNodeLink(index, &ConnectorLinkProc, connector);
			return (true);
		}
		
		#if C4LEGACY
		
			case 'CNCT':
			{
				ConnectorKey	key;
				NodeType		type;
				Type			subtype;
				int32			index;
				
				data >> key;
				data >> type;
				data >> subtype;
				data >> index;
				
				if (!nodeHub) new Hub(this);
				Connector *connector = new Connector(nodeHub, key);
				data.AddNodeLink(index, &ConnectorLinkProc, connector);
				return (true);
			}
			
			case 'CONN':
			{
				Type		key;
				NodeType	type;
				Type		subtype;
				int32		index;
				
				data >> key;
				data >> type;
				data >> subtype;
				data >> index;
				
				if (!nodeHub) new Hub(this);
				Connector *connector = new Connector(nodeHub, Text::TypeToString(key));
				data.AddNodeLink(index, &ConnectorLinkProc, connector);
				return (true);
			}
		
		#endif
		
		case 'PROP':
		{
			Property *property = Property::Construct(data, unpackFlags);
			if (property)
			{
				property->Unpack(++data, unpackFlags);
				propertyMap.Insert(property);
				
				#if C4LEGACY
				
					if ((property->GetPropertyType() == kPropertyName) && (nodeHash == 0))
					{
						nodeHash = Text::GetTextHash(static_cast<NameProperty *>(property)->GetNodeName());
					}
				
				#endif
				
				return (true);
			}
			
			break;
		}
		
		case 'POBJ':
		{
			int32	index;
			
			data >> index;
			data.AddObjectLink(index, &PropertyObjectLinkProc, this);
			return (true);
		}
		
		case 'MTOR':
			
			if (unpackFlags & kUnpackEditor)
			{
				Manipulator *manipulator = Manipulator::Construct(this);
				if (manipulator)
				{
					manipulator->Unpack(data, unpackFlags);
					nodeManipulator = manipulator;
					return (true);
				}
			}
			
			break;
		
		#if C4LEGACY
		
			case 'DATA':
				
				data >> superIndex;
				data >> objectIndex;
				data >> nodeFlags;
				data >> nodeTransform;
				return (true);
		
		#endif
		
		case 'INDX':
			
			data >> superIndex;
			data >> objectIndex;
			return (true);
	}
	
	return (false);
}

void *Node::BeginSettingsUnpack(void)
{
	nodeHash = 0;
	
	delete nodeController;
	nodeController = nullptr;
	
	Array<ConnectorData> *array = nullptr;
	if (nodeHub)
	{
		array = new Array<ConnectorData>(4);
		
		Connector *connector = nodeHub->GetFirstOutgoingEdge();
		while (connector)
		{
			Connector *next = connector->GetNextOutgoingEdge();
			
			Node *node = connector->GetConnectorTarget();
			if (node) array->AddElement(ConnectorData(connector->GetConnectorKey(), node));
			delete connector;
			
			connector = next;
		}
	}
	
	propertyMap.Purge();
	return (array);
}

void Node::EndSettingsUnpack(void *cookie)
{
	Array<ConnectorData> *array = static_cast<Array<ConnectorData> *>(cookie);
	if (array)
	{
		if (nodeHub)
		{
			int32 count = array->GetElementCount();
			for (machine a = 0; a < count; a++)
			{
				const ConnectorData *data = &(*array)[a];
				
				Connector *connector = nodeHub->GetFirstOutgoingEdge();
				while (connector)
				{
					if (connector->GetConnectorKey() == data->connectorKey)
					{
						if (connector->GetFinishElement() == nodeHub) connector->SetConnectorTarget(data->targetNode);
						break;
					}
					
					connector = connector->GetNextOutgoingEdge();
				}
			}
		}
		
		delete array;
	}
}

void Node::ConnectorLinkProc(Node *node, void *cookie)
{
	static_cast<Connector *>(cookie)->SetConnectorTarget(node);
}

void Node::PropertyObjectLinkProc(Object *object, void *cookie)
{
	static_cast<Node *>(cookie)->propertyObject = static_cast<PropertyObject *>(object);
	object->Retain();
}

int32 Node::GetCategoryCount(void) const
{
	return (GetSuperNode() ? 1 : 0);
}

Type Node::GetCategoryType(int32 index, const char **title) const
{
	if (index == 0)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID('NODE'));
		return ('NODE');
	}
	
	return (0);
}

int32 Node::GetCategorySettingCount(Type category) const
{
	if (category == 'NODE') return (5);
	return (0);
}

Setting *Node::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == 'NODE')
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID('NODE', 'NODE'));
			return (new HeadingSetting('NODE', title));
		}
		
		if (index == 1)
		{
			const char *name = nullptr;
			const Property *property = GetProperty(kPropertyName);
			if (property) name = static_cast<const NameProperty *>(property)->GetNodeName();
			
			const char *title = table->GetString(StringID('NODE', 'NODE', 'NAME'));
			return (new TextSetting('NAME', name, title, 127));
		}
		
		if (index == 2)
		{
			const char *title = table->GetString(StringID('NODE', 'NODE', 'DSBL'));
			return (new BooleanSetting('DSBL', ((nodeFlags & kNodeDisabled) != 0), title));
		}
		
		if (index == 3)
		{
			const char *title = table->GetString(StringID('NODE', 'NODE', 'DENO'));
			return (new BooleanSetting('DENO', ((nodeFlags & kNodeDirectEnableOnly) != 0), title));
		}
		
		if (index == 4)
		{
			const char *title = table->GetString(StringID('NODE', 'NODE', 'ANIM'));
			return (new BooleanSetting('ANIM', ((nodeFlags & kNodeAnimateInhibit) != 0), title));
		}
	}
	
	return (nullptr);
}

void Node::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == 'NODE')
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'NAME')
		{
			const char *name = static_cast<const TextSetting *>(setting)->GetText();
			Property *property = GetProperty(kPropertyName);
			
			if (name[0] != 0)
			{
				if (property) static_cast<NameProperty *>(property)->SetNodeName(name);
				else AddProperty(new NameProperty(name));
				
				nodeHash = Text::GetTextHash(name);
			}
			else
			{
				delete property;
				nodeHash = 0;
			}
		}
		else if (identifier == 'DSBL')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) nodeFlags |= kNodeDisabled;
			else nodeFlags &= ~kNodeDisabled;
		}
		else if (identifier == 'DENO')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) nodeFlags |= kNodeDirectEnableOnly;
			else nodeFlags &= ~kNodeDirectEnableOnly;
		}
		else if (identifier == 'ANIM')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) nodeFlags |= kNodeAnimateInhibit;
			else nodeFlags &= ~kNodeAnimateInhibit;
		}
	}
}

void Node::StopMotion(void)
{
	previousWorldTransform(3,3) = 0.0F;
	if (nodeController) nodeController->StopMotion();
	
	Node *subnode = GetFirstSubnode();
	while (subnode)
	{
		subnode->StopMotion();
		subnode = subnode->Next();
	}
}

void Node::Update(void)
{
	UpdateTransform();
	UpdatePostTransform();
	UpdateBoundingSphere();
	UpdateVisibility();
	UpdatePostBounding();
}

void Node::InitTransform(void)
{
	Box3D	box;
	
	CalculateWorldTransform();
	if (CalculateBoundingBox(&box)) SetWorldBoundingBox(Transform(box, GetWorldTransform()));
	
	Node *node = GetFirstSubnode();
	while (node)
	{
		node->InitTransform();
		node = node->Next();
	}
}

void Node::UpdateTransform(void)
{
	unsigned_int32 flags = GetCurrentUpdateFlags();
	if (flags & kUpdateTransform)
	{
		Box3D	box;
		
		SetCurrentUpdateFlags(flags & ~kUpdateTransform);
		
		CalculateWorldTransform();
		if (CalculateBoundingBox(&box)) SetWorldBoundingBox(Transform(box, GetWorldTransform()));
	}
	
	flags = GetSubtreeUpdateFlags();
	if (flags & kUpdateTransform)
	{
		SetSubtreeUpdateFlags(flags & ~kUpdateTransform);
		
		Node *node = GetFirstSubnode();
		while (node)
		{
			node->UpdateTransform();
			node = node->Next();
		}
	}
}

void Node::UpdatePostTransform(void)
{
	unsigned_int32 flags = GetSubtreeUpdateFlags();
	if (flags & kUpdatePostTransform)
	{
		SetSubtreeUpdateFlags(flags & ~kUpdatePostTransform);
		
		Node *node = GetFirstSubnode();
		while (node)
		{
			node->UpdatePostTransform();
			node = node->Next();
		}
	}
	
	flags = GetCurrentUpdateFlags();
	if (flags & kUpdatePostTransform)
	{
		SetCurrentUpdateFlags(flags & ~kUpdatePostTransform);
		CalculatePostTransform();
	}
}

void Node::UpdateBoundingSphere(void)
{
	BoundingSphere *spherePointer = nullptr;
	
	unsigned_int32 flags = GetCurrentUpdateFlags();
	if (flags & kUpdateBoundingSphere)
	{
		SetCurrentUpdateFlags(flags & ~kUpdateBoundingSphere);
		
		if (CalculateBoundingSphere(&boundingSphere))
		{
			boundingSphere.SetCenter(GetWorldTransform() * boundingSphere.GetCenter());
			spherePointer = &boundingSphere;
		}
	}
	else
	{
		spherePointer = boundingSpherePointer;
	}
	
	flags = GetSubtreeUpdateFlags();
	if (flags & kUpdateBoundingSphere)
	{
		SetSubtreeUpdateFlags(flags & ~kUpdateBoundingSphere);
		
		Node *node = GetFirstSubnode();
		while (node)
		{
			node->UpdateBoundingSphere();
			
			const BoundingSphere *sphere = node->GetBoundingSphere();
			if (sphere)
			{
				if (spherePointer)
				{
					spherePointer->Union(sphere);
				}
				else
				{
					boundingSphere = *sphere;
					spherePointer = &boundingSphere;
				}
			}
			
			node = node->Next();
		}
	}
	
	boundingSpherePointer = spherePointer;
}

void Node::UpdateVisibility(void)
{
	unsigned_int32 flags = GetCurrentUpdateFlags();
	if (flags & kUpdateVisibility)
	{
		SetCurrentUpdateFlags(flags & ~kUpdateVisibility);
		CalculateVisibility();
	}
	else
	{
		flags = GetSubtreeUpdateFlags();
		if (flags & kUpdateVisibility)
		{
			SetSubtreeUpdateFlags(flags & ~kUpdateVisibility);
			
			Node *node = GetFirstSubnode();
			while (node)
			{
				node->UpdateVisibility();
				node = node->Next();
			}
		}
	}
}

void Node::UpdatePostBounding(void)
{
	unsigned_int32 flags = GetCurrentUpdateFlags();
	if (flags & kUpdatePostBounding)
	{
		SetCurrentUpdateFlags(flags & ~kUpdatePostBounding);
		CalculatePostBounding();
	}
	
	flags = GetSubtreeUpdateFlags();
	if (flags & kUpdatePostBounding)
	{
		SetSubtreeUpdateFlags(flags & ~kUpdatePostBounding);
		
		Node *node = GetFirstSubnode();
		while (node)
		{
			node->UpdatePostBounding();
			node = node->Next();
		}
	}
}

void Node::CalculateWorldTransform(void)
{
	const Node *super = GetSuperNode();
	
	if (previousWorldTransform(3,3) != 0.0F)
	{
		previousWorldTransform = GetWorldTransform();
		
		if (super) SetWorldTransform(super->GetWorldTransform() * nodeTransform);
		else SetWorldTransform(nodeTransform);
	}
	else
	{
		if (super) SetWorldTransform(super->GetWorldTransform() * nodeTransform);
		else SetWorldTransform(nodeTransform);
		
		previousWorldTransform = GetWorldTransform();
	}
}

void Node::CalculatePostTransform(void)
{
	const Bond *bond = GetFirstOutgoingEdge();
	if (bond)
	{
		Box3D box = bond->GetFinishElement()->GetWorldBoundingBox();
		for (;;)
		{
			bond = bond->GetNextOutgoingEdge();
			if (!bond) break;
			
			box.Union(bond->GetFinishElement()->GetWorldBoundingBox());
		}
		
		SetWorldBoundingBox(box);
	}
}

bool Node::CalculateBoundingBox(Box3D *box) const
{
	return (false);
}

bool Node::CalculateBoundingSphere(BoundingSphere *sphere) const
{
	return (false);
}

void Node::CalculateVisibility(void)
{
	if (boundingSpherePointer)
	{
		Zone *oldZone = GetOwningZone();
		if (!oldZone->GetFirstSubzone())
		{
			if (oldZone->GetObject()->InteriorSphere(oldZone->GetInverseWorldTransform() * boundingSpherePointer->GetCenter(), boundingSpherePointer->GetRadius())) return;
		}
		
		Zone *newZone = nodeWorld->FindZone(GetWorldPosition());
		if (newZone != oldZone)
		{
			SetNodeTransform(newZone->GetInverseWorldTransform() * GetWorldTransform());
			if (nodeController) nodeController->ChangeZones(newZone, newZone->GetInverseWorldTransform() * GetSuperNode()->GetWorldTransform());
			newZone->Tree<Node>::AddSubnode(this);
			
			Node *node = this;
			do
			{
				node->ExitZone(oldZone);
				node->EnterZone(newZone);
				
				node = GetNextNode(node);
			} while (node);
			
			if (!FindIncomingEdge(newZone)) new Bond(newZone, this);
		}
		
		Bond *bond = GetFirstIncomingEdge();
		if (bond)
		{
			const Point3D& center = boundingSpherePointer->GetCenter();
			float radius = boundingSpherePointer->GetRadius();
			
			const Zone *zone = GetOwningZone();
			if (zone->GetObject()->InteriorSphere(zone->GetInverseWorldTransform() * center, radius))
			{
				do
				{
					Bond *next = bond->GetNextIncomingEdge();
					if (bond->GetStartElement() != zone) delete bond;
					bond = next;
				} while (bond);
			}
			else
			{
				do
				{
					Bond *next = bond->GetNextIncomingEdge();
					
					zone = static_cast<Zone *>(bond->GetStartElement());
					if (zone->GetObject()->ExteriorSphere(zone->GetInverseWorldTransform() * center, radius)) delete bond;
					
					bond = next;
				} while (bond);
			}
		}
	}
}

void Node::CalculatePostBounding(void)
{
}

bool Node::AlwaysVisible(const Node *node, const Region *region)
{
	return (true);
}

bool Node::NeverOccluded(const Node *node, const Region *region)
{
	return (false);
}

bool Node::BoxVisible(const Node *node, const Region *region)
{
	return (region->BoxVisible(node->GetWorldBoundingBox()));
}

bool Node::BoxOccluded(const Node *node, const Region *region)
{
	const Box3D& box = node->GetWorldBoundingBox();
	do
	{
		if (region->BoxOccluded(box)) return (true);
		region = region->Next();
	} while (region);
	
	return (false);
}

bool Node::SphereVisible(const Node *node, const Region *region)
{
	const BoundingSphere *sphere = node->GetBoundingSphere();
	return ((sphere) && (region->SphereVisible(sphere->GetCenter(), sphere->GetRadius())));
}

bool Node::SphereOccluded(const Node *node, const Region *region)
{
	const BoundingSphere *sphere = node->GetBoundingSphere();
	const Point3D& center = sphere->GetCenter();
	float radius = sphere->GetRadius();
	
	do
	{
		if (region->SphereOccluded(center, radius)) return (true);
		region = region->Next();
	} while (region);
	
	return (false);
}

void Node::Enable(void)
{
	nodeFlags &= ~kNodeDisabled;
	
	Node *subnode = GetFirstSubnode();
	while (subnode)
	{
		unsigned_int32 flags = subnode->nodeFlags;
		if (!(flags & kNodeDirectEnableOnly)) subnode->nodeFlags = flags & ~kNodeDisabled;
		
		subnode = GetNextNode(subnode);
	}
}

void Node::Disable(void)
{
	nodeFlags |= kNodeDisabled;
	
	Node *subnode = GetFirstSubnode();
	while (subnode)
	{
		unsigned_int32 flags = subnode->nodeFlags;
		if (!(flags & kNodeDirectEnableOnly)) subnode->nodeFlags = flags | kNodeDisabled;
		
		subnode = GetNextNode(subnode);
	}
}

void Node::SetPersistent(void)
{
	nodeFlags &= ~kNodeNonpersistent;
	
	Node *subnode = GetFirstSubnode();
	while (subnode)
	{
		subnode->nodeFlags &= ~kNodeNonpersistent;
		subnode = GetNextNode(subnode);
	}
}

void Node::SetNonpersistent(void)
{
	nodeFlags |= kNodeNonpersistent;
	
	Node *subnode = GetFirstSubnode();
	while (subnode)
	{
		subnode->nodeFlags |= kNodeNonpersistent;
		subnode = GetNextNode(subnode);
	}
}

void Node::SetObject(Object *object)
{
	if (nodeObject != object)
	{
		if (nodeObject) nodeObject->Release();
		if (object) object->Retain();
		nodeObject = object;
	}
}

void Node::SetController(Controller *controller)
{
	if (nodeController != controller)
	{
		if (nodeController) nodeController->SetTargetNode(nullptr);
		if (controller) controller->SetTargetNode(this);
		nodeController = controller;
	}
}

Node *Node::GetConnectedNode(const char *key) const
{
	const Hub *hub = nodeHub;
	if (hub)
	{
		const Connector *connector = hub->GetFirstOutgoingEdge();
		while (connector)
		{
			if (connector->GetConnectorKey() == key)
			{
				const Hub *finish = connector->GetFinishElement();
				if (finish != hub) return (finish->GetNode());
				break;
			}
			
			connector = connector->GetNextOutgoingEdge();
		}
	}
	
	return (nullptr);
}

bool Node::SetConnectedNode(const char *key, Node *node) const
{
	const Hub *hub = nodeHub;
	if (hub)
	{
		Connector *connector = hub->GetFirstOutgoingEdge();
		while (connector)
		{
			if (connector->GetConnectorKey() == key)
			{
				connector->SetConnectorTarget(node);
				return (true);
			}
			
			connector = connector->GetNextOutgoingEdge();
		}
	}
	
	return (false);
}

void Node::AddConnector(const char *key, Node *node)
{
	if (!nodeHub) new Hub(this);
	
	if (node)
	{
		Hub *finish = node->nodeHub;
		if (!finish) finish = new Hub(node);
		
		new Connector(nodeHub, finish, key);
	}
	else
	{
		new Connector(nodeHub, key);
	}
}

bool Node::RemoveConnector(const char *key)
{
	if (nodeHub)
	{
		Connector *connector = nodeHub->FindOutgoingConnector(key);
		if (connector)
		{
			delete connector;
			if (nodeHub->Isolated()) delete nodeHub;
			return (true);
		}
	}
	
	return (false);
}

int32 Node::GetInternalConnectorCount(void) const
{
	return (0);
}

const char *Node::GetInternalConnectorKey(int32 index) const
{
	return (nullptr);
}

void Node::ProcessInternalConnectors(void)
{
}

bool Node::ValidConnectedNode(const ConnectorKey& key, const Node *node) const
{
	return (true);
}

void Node::SetPropertyObject(PropertyObject *object)
{
	PropertyObject *prevObject = propertyObject;
	if (prevObject != object)
	{
		if (prevObject) prevObject->Release();
		if (object) object->Retain();
		propertyObject = object;
	}
}

const char *Node::GetNodeName(void) const
{
	const Property *property = GetProperty(kPropertyName);
	if (property) return (static_cast<const NameProperty *>(property)->GetNodeName());
	return (nullptr);
}

void Node::SetNodeName(const char *name)
{
	Property *property = GetProperty(kPropertyName);
	if (property) static_cast<NameProperty *>(property)->SetNodeName(name);
	else AddProperty(new NameProperty(name));
	
	nodeHash = Text::GetTextHash(name);
}

void Node::BondVisibility(void)
{
	unsigned_int32 flags = nodeFlags;
	if ((flags & (kNodeVisibilitySite | kNodeIsolatedVisibility)) == kNodeVisibilitySite) CalculatePostTransform();
	
	Node *super = GetSuperNode();
	while (super)
	{
		unsigned_int32 superFlags = super->GetNodeFlags();
		if (superFlags & kNodeVisibilitySite)
		{
			new Bond(super, this);
			break;
		}
		
		if (super->GetNodeType() == kNodeZone)
		{
			if (!(flags & kNodeDynamicVisibility)) static_cast<Zone *>(super)->AddSite(this);
			else new Bond(super, this);
			
			break;
		}
		
		if (superFlags & kNodeExternalVisibility) break;
		
		super = super->GetSuperNode();
	}
}

void Node::BreakVisibility(void)
{
	PurgeIncomingEdges();
}

void Node::TransferVisibility(void)
{
	Node *super = GetSuperNode();
	while (super)
	{
		unsigned_int32 flags = super->GetNodeFlags();
		if (flags & kNodeVisibilitySite)
		{
			for (;;)
			{
				Bond *bond = GetFirstOutgoingEdge();
				if (!bond) break;
				
				bond->SetStartElement(super);
			}
			
			break;
		}
		
		if (super->GetNodeType() == kNodeZone)
		{
			Zone *zone = static_cast<Zone *>(super);
			for (;;)
			{
				Bond *bond = GetFirstOutgoingEdge();
				if (!bond) break;
				
				Site *site = bond->GetFinishElement();
				delete bond;
				
				if (!(static_cast<Node *>(site)->GetNodeFlags() & kNodeDynamicVisibility)) zone->AddSite(site);
				else new Bond(zone, site);
			}
			
			break;
		}
		
		if (flags & kNodeExternalVisibility) break;
		
		super = super->GetSuperNode();
	}
}

void Node::Preprocess(void)
{
	Node *super = GetSuperNode();
	if (super) nodeWorld = super->GetWorld();
	
	ProcessInternalConnectors();
	
	if (nodeController) nodeController->Preprocess();
	if (nodeManipulator) nodeManipulator->Preprocess();
	
	Zone *zone = GetOwningZone();
	
	unsigned_int32 flags = nodeFlags;
	if (flags & kNodeVisibilitySite)
	{
		if (!(flags & kNodeIsolatedVisibility)) SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdatePostTransform);
		SetVisibilityProc(&BoxVisible);
		SetOcclusionProc(&BoxOccluded);
	}
	else if (flags & kNodeExternalVisibility)
	{
		if (zone) new Bond(zone, this);
	}
	
	if (zone) EnterZone(zone);
	
	Node *subnode = GetFirstSubnode();
	while (subnode)
	{
		subnode->Preprocess();
		subnode = subnode->Next();
	}
}

void Node::Neutralize(void)
{
	Node *subnode = GetLastSubnode();
	while (subnode)
	{
		subnode->Neutralize();
		subnode = subnode->Previous();
	}
	
	Zone *zone = GetOwningZone();
	if (zone) ExitZone(zone);
	
	if (nodeManipulator) nodeManipulator->Neutralize();
	if (nodeController) nodeController->Neutralize();
	
	BreakVisibility();
	SetActiveUpdateFlags(GetActiveUpdateFlags() & ~kUpdatePostTransform);
	
	nodeWorld = nullptr;
}

void Node::ProcessObjectSettings(void)
{
}

void Node::EnterZone(Zone *zone)
{
}

void Node::ExitZone(Zone *zone)
{
}

C4::Zone *Node::GetOwningZone(void) const
{
	Node *super = GetSuperNode();
	while (super)
	{
		if (super->GetNodeType() == kNodeZone) break;
		super = super->GetSuperNode();
	}
	
	return (static_cast<Zone *>(super));
}

void Node::PrepackNodeObjects(List<Object> *linkList) const
{
	List<Object>	list;
	
	Prepack(&list);
	for (;;)
	{
		Object *object = list.First();
		if (!object) break;
		
		object->Prepack(&list);
		linkList->Append(object);
	}
}

FileResult Node::PackTree(File *file, unsigned_int32 packFlags) const
{
	int32			fileHeader[2];
	List<Object>	objectList;
	
	fileHeader[0] = 1;
	fileHeader[1] = kWorldVersion;
	
	FileResult result = file->Write(fileHeader, 8);
	if (result != kFileOkay) return (result);
	
	int32 controllerCount = 0;
	int32 nodeCount = 0;
	
	const Node *node = this;
	do
	{
		if (!(node->GetNodeFlags() & kNodeNonpersistent))
		{
			node->nodeIndex = nodeCount++;
			node->PrepackNodeObjects(&objectList);
			
			Controller *controller = node->GetController();
			if (controller)
			{
				if (packFlags & kPackInitialize)
				{
					controller->SetControllerIndex(kControllerUnassigned);
					if (!(controller->GetControllerFlags() & kControllerLocal)) controllerCount++;
				}
				else
				{
					controllerCount = Max(controller->GetControllerIndex() + 1, controllerCount);
				}
			}
			
			node = GetNextNode(node);
		}
		else
		{
			node->nodeIndex = -1;
			
			Node *subnode = node->GetFirstSubnode();
			while (subnode)
			{
				subnode->nodeIndex = -1;
				subnode = node->GetNextNode(subnode);
			}
			
			node = GetNextLevelNode(node);
		}
	} while (node);
	
	int32 objectCount = 0;
	Object *object = objectList.First();
	while (object)
	{
		object->SetObjectIndex(objectCount);
		objectCount++;
		
		object = object->Next();
	}
	
	file->Write(&controllerCount, 4);
	file->Write(&objectCount, 4);
	file->Write(&nodeCount, 4);
	
	int32 offsetCount = objectCount + 1;
	file->Write(&offsetCount, 4);
	
	unsigned_int32 *objectOffset = new unsigned_int32[offsetCount];
	MemoryMgr::ClearMemory(objectOffset, offsetCount * 4);
	result = file->Write(objectOffset, offsetCount * 4);
	
	if (result != kFileOkay)
	{
		delete[] objectOffset;
		objectList.RemoveAll();
		return (result);
	}
	
	Buffer buffer(kPackageDefaultSize);
	
	unsigned_int32 offset = 24 + offsetCount * 4;
	objectOffset[0] = offset;
	
	for (machine a = 0; a < objectCount; a++)
	{
		object = objectList.First();
		
		Package package(buffer, kPackageDefaultSize);
		Packer packer(&package);
		
		PackHandle handle = packer.BeginSection();
		object->PackType(packer);
		object->Pack(packer, packFlags);
		packer.EndSection(handle);
		
		unsigned_int32 size = package.GetSize();
		offset += size;
		objectOffset[a + 1] = offset;
		
		result = file->Write(package.GetStorage(), size);
		if (result != kFileOkay) break;
		
		objectList.Remove(object);
	}
	
	if (result != kFileOkay)
	{
		delete[] objectOffset;
		objectList.RemoveAll();
		return (result);
	}
	
	Node *super = GetSuperNode();
	if (super) super->nodeIndex = -1;
	
	node = this;
	do
	{
		if (!(node->GetNodeFlags() & kNodeNonpersistent))
		{
			Package package(buffer, kPackageDefaultSize);
			Packer packer(&package);
			
			PackHandle handle = packer.BeginSection();
			node->PackType(packer);
			node->Pack(packer, packFlags);
			packer.EndSection(handle);
			
			result = file->Write(package.GetStorage(), package.GetSize());
			if (result != kFileOkay) break;
			
			node = GetNextNode(node);
		}
		else
		{
			node = GetNextLevelNode(node);
		}
	} while (node);
	
	if (result == kFileOkay)
	{
		file->SetPosition(24);
		result = file->Write(objectOffset, offsetCount * 4);
	}
	
	delete[] objectOffset;
	return (result);
}

void Node::PackTree(Package *package, unsigned_int32 packFlags) const
{
	List<Object>	objectList;
	
	Packer packer(package);
	
	int32 endian = 1;
	packer << endian;
	
	int32 version = kWorldVersion;
	packer << version;
	
	bool select = ((packFlags & kPackSelected) != 0);
	
	int32 controllerCount = 0;
	int32 nodeCount = 0;
	
	const Node *node = this;
	do
	{
		if (!(node->GetNodeFlags() & kNodeNonpersistent))
		{
			bool include = !select;
			if (!include)
			{
				const Manipulator *manipulator = node->GetManipulator();
				if ((manipulator) && (manipulator->Selected())) include = true;
			}
			
			if (include)
			{
				node->nodeIndex = nodeCount++;
				node->PrepackNodeObjects(&objectList);
				
				Controller *controller = node->GetController();
				if (controller)
				{
					if (packFlags & kPackInitialize)
					{
						controller->SetControllerIndex(kControllerUnassigned);
						if (!(controller->GetControllerFlags() & kControllerLocal)) controllerCount++;
					}
					else
					{
						controllerCount = Max(controller->GetControllerIndex() + 1, controllerCount);
					}
				}
			}
			else
			{
				node->nodeIndex = -1;
			}
			
			node = GetNextNode(node);
		}
		else
		{
			node->nodeIndex = -1;
			
			Node *subnode = node->GetFirstSubnode();
			while (subnode)
			{
				subnode->nodeIndex = -1;
				subnode = node->GetNextNode(subnode);
			}
			
			node = GetNextLevelNode(node);
		}
	} while (node);
	
	int32 objectCount = 0;
	Object *object = objectList.First();
	while (object)
	{
		object->SetObjectIndex(objectCount);
		objectCount++;
		
		object = object->Next();
	}
	
	packer << controllerCount;
	packer << objectCount;
	packer << nodeCount;
	
	int32 offsetCount = 0;
	packer << offsetCount;
	
	for (;;)
	{
		object = objectList.First();
		if (!object) break;
		
		PackHandle handle = packer.BeginSection();
		object->PackType(packer);
		object->Pack(packer, packFlags);
		packer.EndSection(handle);
		
		objectList.Remove(object);
	}
	
	Node *super = GetSuperNode();
	if (super) super->nodeIndex = -1;
	
	node = this;
	do
	{
		if (!(node->GetNodeFlags() & kNodeNonpersistent))
		{
			bool include = !select;
			if (!include)
			{
				const Manipulator *manipulator = node->GetManipulator();
				if ((manipulator) && (manipulator->Selected())) include = true;
			}
			
			if (include)
			{
				PackHandle handle = packer.BeginSection();
				node->PackType(packer);
				node->Pack(packer, packFlags);
				packer.EndSection(handle);
			}
			
			node = GetNextNode(node);
		}
		else
		{
			node = GetNextLevelNode(node);
		}
	} while (node);
}

Object **Node::LoadOriginalObjects(const ResourceName& name, World *previousWorld, int32 newObjectCount, int32 *originalObjectCount, int32 *totalObjectCount)
{
	ResourceLoader		loader;
	WorldHeader			worldHeader;
	int32				*offsetTable;
	
	WorldResource *resource = WorldResource::Get(name, kResourceDeferLoad);
	
	ResourceResult result = resource->OpenLoader(&loader);
	if (result != kResourceOkay)
	{
		resource->Release();
		return (nullptr);
	}
	
	result = resource->LoadObjectOffsetTable(&loader, &worldHeader, &offsetTable);
	if (result != kResourceOkay)
	{
		resource->Release();
		return (nullptr);
	}
	
	int32 objectCount = worldHeader.objectCount;
	int32 totalCount = Max(objectCount, newObjectCount);
	*originalObjectCount = objectCount;
	*totalObjectCount = totalCount;
	
	Object **objectTable = new Object *[totalCount];
	MemoryMgr::ClearMemory(objectTable, totalCount * sizeof(Object *));
	
	if ((previousWorld) && (Text::CompareTextCaseless(previousWorld->GetWorldName(), name)))
	{
		List<Object>	objectList;
		
		Node *root = previousWorld->GetRootNode();
		const Node *node = root;
		do
		{
			if (!(node->GetNodeFlags() & kNodeNonpersistent))
			{
				node->PrepackNodeObjects(&objectList);
				node = root->GetNextNode(node);
			}
			else
			{
				node = root->GetNextLevelNode(node);
			}
		} while (node);
		
		for (;;)
		{
			Object *object = objectList.First();
			if (!object) break;
			
			int32 index = object->GetObjectIndex();
			if ((index >= 0) && (!object->GetModifiedFlag()))
			{
				object->Retain();
				objectTable[index] = object;
			}
			
			objectList.Remove(object);
		}
		
		delete previousWorld;
		
		for (machine a = 0; a < objectCount; a++)
		{
			if (!objectTable[a])
			{
				char			*objectData;
				unsigned_int32	size;
				
				if (resource->LoadObject(&loader, (int32) a, offsetTable, &objectData) == kResourceOkay)
				{
					Unpacker unpacker(objectData, worldHeader.endian, worldHeader.version);
					
					unpacker >> size;
					Object *object = Object::Construct(unpacker);
					if (object)
					{
						object->Unpack(++unpacker, 0);
						object->SetObjectIndex((int32) a);
						objectTable[a] = object;
					}
					
					delete[] objectData;
				}
			}
		}
	}
	else
	{
		char	*objectData;
		
		delete previousWorld;
		
		if (resource->LoadAllObjects(&loader, &worldHeader, offsetTable, &objectData) == kResourceOkay)
		{
			Unpacker unpacker(objectData, worldHeader.endian, worldHeader.version);
			
			for (machine a = 0; a < objectCount; a++)
			{
				unsigned_int32	size;
				
				unpacker >> size;
				unpacker.SetMark();
				
				Object *object = Object::Construct(unpacker);
				if (object)
				{
					object->Unpack(++unpacker, 0);
					object->SetObjectIndex((int32) a);
					objectTable[a] = object;
				}
				else
				{
					unpacker.Skip(size);
					objectTable[a] = nullptr;
				}
			}
			
			delete[] objectData;
		}
	}
	
	delete[] offsetTable;
	resource->Release();
	
	return (objectTable);
}

Node *Node::LoadNodeTable(Unpacker& unpacker, unsigned_int32 unpackFlags, int32 nodeCount, int32 objectCount, Object **objectTable)
{
	Node *root = nullptr;
	Node **nodeTable = new Node *[nodeCount];
	for (machine a = 0; a < nodeCount; a++)
	{
		unsigned_int32	size;
		
		unpacker >> size;
		unpacker.SetMark();
		
		Node *node = Construct(unpacker, unpackFlags);
		if (node)
		{
			node->Unpack(++unpacker, unpackFlags);
			nodeTable[a] = node;
		}
		else
		{
			unpacker.Skip(size);
			nodeTable[a] = nullptr;
		}
	}
	
	for (machine a = 0; a < nodeCount; a++)
	{
		Node *node = nodeTable[a];
		if (node)
		{
			int32 superIndex = node->superIndex;
			if (superIndex >= 0)
			{
				Node *super = nodeTable[superIndex];
				if (super)
				{
					super->AddSubnode(node);
				}
				else
				{
					delete node;
					nodeTable[a] = nullptr;
					continue;
				}
			}
			else
			{
				root = node;
			}
			
			int32 objectIndex = node->objectIndex;
			if (objectIndex >= 0)
			{
				Object *object = objectTable[objectIndex];
				if (object)
				{
					node->SetObject(object);
				}
				else if (node != root)
				{
					delete node;
					nodeTable[a] = nullptr;
				}
			}
		}
	}
	
	ObjectLink *objectLink = unpacker.GetFirstObjectLink();
	while (objectLink)
	{
		int32 index = objectLink->GetObjectIndex();
		objectLink->CallLinkProc((index >= 0) ? objectTable[index] : nullptr);
		objectLink = objectLink->Next();
	}
	
	NodeLink *nodeLink = unpacker.GetFirstNodeLink();
	while (nodeLink)
	{
		int32 index = nodeLink->GetNodeIndex();
		nodeLink->CallLinkProc((index >= 0) ? nodeTable[index] : nullptr);
		nodeLink = nodeLink->Next();
	}
	
	for (machine a = objectCount - 1; a >= 0; a--)
	{
		Object *object = objectTable[a];
		if (object) object->Release();
	}
	
	delete[] nodeTable;
	delete[] objectTable;
	
	return (root);
}

Node *Node::UnpackTree(const void *data, unsigned_int32 unpackFlags)
{
	int32	controllerCount;
	int32	objectCount;
	int32	nodeCount;
	int32	offsetCount;
	
	const int32 *format = static_cast<const int32 *>(data);
	Unpacker unpacker(format + 2, format[0], format[1]);
	
	unpacker >> controllerCount;
	unpacker >> objectCount;
	unpacker >> nodeCount;
	
	unpacker >> offsetCount;
	unpacker += offsetCount * 4;
	
	Object **objectTable = new Object *[objectCount];
	for (machine a = 0; a < objectCount; a++)
	{
		unsigned_int32	size;
		
		unpacker >> size;
		unpacker.SetMark();
		
		Object *object = Object::Construct(unpacker, unpackFlags);
		if (object)
		{
			object->Unpack(++unpacker, unpackFlags);
			if ((unpackFlags & (kUnpackNonpersistent | kUnpackExternal)) == 0) object->SetObjectIndex((int32) a);
			objectTable[a] = object;
		}
		else
		{
			unpacker.Skip(size);
			objectTable[a] = nullptr;
		}
	}
	
	return (LoadNodeTable(unpacker, unpackFlags, nodeCount, objectCount, objectTable));
}

FileResult Node::PackDeltaTree(File *file, const ResourceName& originalName) const
{
	int32			fileHeader[7];
	List<Object>	objectList;
	
	int32 controllerCount = 0;
	int32 nodeCount = 0;
	
	const Node *node = this;
	do
	{
		if (!(node->GetNodeFlags() & kNodeNonpersistent))
		{
			node->nodeIndex = nodeCount++;
			node->PrepackNodeObjects(&objectList);
			
			Controller *controller = node->GetController();
			if (controller) controllerCount = Max(controller->GetControllerIndex() + 1, controllerCount);
			
			node = GetNextNode(node);
		}
		else
		{
			node->nodeIndex = -1;
			
			Node *subnode = node->GetFirstSubnode();
			while (subnode)
			{
				subnode->nodeIndex = -1;
				subnode = node->GetNextNode(subnode);
			}
			
			node = GetNextLevelNode(node);
		}
	} while (node);
	
	int32 objectCount = 0;
	int32 modifiedCount = 0;
	
	Object *object = objectList.First();
	while (object)
	{
		int32 index = object->GetObjectIndex();
		if (index >= 0)
		{
			objectCount = Max(index + 1, objectCount);
			if (object->GetModifiedFlag()) modifiedCount++;
		}
		
		object = object->Next();
	}
	
	int32 originalCount = objectCount;
	
	object = objectList.First();
	while (object)
	{
		if (object->GetObjectIndex() < 0)
		{
			object->SetObjectIndex(objectCount);
			object->SetModifiedFlag();
			objectCount++;
			modifiedCount++;
		}
		
		object = object->Next();
	}
	
	ResourceName name(originalName);
	unsigned_int32 nameLength = name.Length();
	unsigned_int32 nameSize = (nameLength + 4) & ~3;
	for (unsigned_machine a = nameLength + 1; a < nameSize; a++) name[int32(a)] = 0;
	
	fileHeader[0] = 1;
	fileHeader[1] = kWorldVersion;
	fileHeader[2] = controllerCount;
	fileHeader[3] = objectCount;
	fileHeader[4] = modifiedCount;
	fileHeader[5] = nodeCount;
	fileHeader[6] = nameSize;
	
	FileResult result = file->Write(fileHeader, 28);
	if (result == kFileOkay) result = file->Write(&name, nameSize);
	
	Buffer buffer(kPackageDefaultSize);
	
	if (result == kFileOkay)
	{
		object = objectList.First();
		while (object)
		{
			if (object->GetModifiedFlag())
			{
				int32 index = object->GetObjectIndex();
				file->Write(&index, 4);
				
				Package package(buffer, kPackageDefaultSize);
				Packer packer(&package);
				
				PackHandle handle = packer.BeginSection();
				object->PackType(packer);
				object->Pack(packer, 0);
				packer.EndSection(handle);
				
				result = file->Write(package.GetStorage(), package.GetSize());
				if (result != kFileOkay) break;
			}
			
			object = object->Next();
		}
	}
	
	if (result == kFileOkay)
	{
		Node *super = GetSuperNode();
		if (super) super->nodeIndex = -1;
		
		node = this;
		do
		{
			if (!(node->GetNodeFlags() & kNodeNonpersistent))
			{
				Package package(buffer, kPackageDefaultSize);
				Packer packer(&package);
				
				PackHandle handle = packer.BeginSection();
				node->PackType(packer);
				node->Pack(packer, 0);
				packer.EndSection(handle);
				
				result = file->Write(package.GetStorage(), package.GetSize());
				if (result != kFileOkay) break;
				
				node = GetNextNode(node);
			}
			else
			{
				node = GetNextLevelNode(node);
			}
		} while (node);
	}
	
	for (;;)
	{
		object = objectList.First();
		if (!object) break;
		
		if (object->GetObjectIndex() >= originalCount) object->SetObjectIndex(-1);
		
		objectList.Remove(object);
	}
	
	return (result);
}

Node *Node::UnpackDeltaTree(const void *data, ResourceName& originalName, World *previousWorld)
{
	int32	originalObjectCount;
	int32	totalObjectCount;
	int32	controllerCount;
	int32	objectCount;
	int32	modifiedCount;
	int32	nodeCount;
	
	const int32 *format = static_cast<const int32 *>(data);
	Unpacker unpacker(format + 2, format[0], format[1]);
	
	unpacker >> controllerCount;
	unpacker >> objectCount;
	unpacker >> modifiedCount;
	unpacker >> nodeCount;
	
	unpacker >> originalName;
	Object **objectTable = LoadOriginalObjects(originalName, previousWorld, objectCount, &originalObjectCount, &totalObjectCount);
	if (!objectTable) return (nullptr);
	
	for (machine a = 0; a < modifiedCount; a++)
	{
		int32			index;
		unsigned_int32	size;
		
		unpacker >> index;
		if (index < originalObjectCount)
		{
			Object *originalObject = objectTable[index];
			if (originalObject) originalObject->Release();
		}
		
		unpacker >> size;
		unpacker.SetMark();
		
		Object *object = Object::Construct(unpacker);
		if (object)
		{
			object->Unpack(++unpacker, 0);
			object->SetObjectIndex(index);
			object->SetModifiedFlag();
			objectTable[index] = object;
		}
		else
		{
			unpacker.Skip(size);
			objectTable[index] = nullptr;
		}
	}
	
	return (LoadNodeTable(unpacker, 0, nodeCount, totalObjectCount, objectTable));
}


RenderableNode::RenderableNode(NodeType type, RenderType renderType, unsigned_int32 renderState) :
		Node(type),
		Renderable(renderType, renderState)
{
}

RenderableNode::RenderableNode(const RenderableNode& renderableNode) :
		Node(renderableNode),
		Renderable(renderableNode.GetRenderType(), renderableNode.GetRenderState())
{
}

RenderableNode::~RenderableNode()
{
}

void RenderableNode::CalculatePostTransform(void)
{
	const Bond *bond = GetFirstOutgoingEdge();
	if (bond)
	{
		Box3D box = Union(GetWorldBoundingBox(), bond->GetFinishElement()->GetWorldBoundingBox());
		for (;;)
		{
			bond = bond->GetNextOutgoingEdge();
			if (!bond) break;
			
			box.Union(bond->GetFinishElement()->GetWorldBoundingBox());
		}
		
		SetWorldBoundingBox(box);
	}
}

void RenderableNode::Neutralize(void)
{
	InvalidateShaderData();
	Node::Neutralize();
}

// ZYURVUR
