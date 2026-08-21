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
#include "C4Instances.h"
#include "C4Configuration.h"


using namespace C4;


Instance::Instance() : Node(kNodeInstance)
{
}

Instance::Instance(const char *name) : Node(kNodeInstance)
{
	worldName = name;
}

Instance::Instance(const Instance& instance) : Node(instance)
{
	worldName = instance.worldName;
	
	const Modifier *modifier = instance.GetFirstModifier();
	while (modifier)
	{
		Modifier *clone = modifier->Clone();
		if (clone) AddModifier(clone);
		
		modifier = modifier->Next();
	}
}

Instance::~Instance()
{
}

Node *Instance::Replicate(void) const
{
	return (new Instance(*this));
}

void Instance::Prepack(List<Object> *linkList) const
{
	Node::Prepack(linkList);
	
	const Modifier *modifier = modifierList.First();
	while (modifier)
	{
		modifier->Prepack(linkList);
		modifier = modifier->Next();
	}
}

void Instance::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Node::Pack(data, packFlags);
	
	PackHandle handle = data.BeginChunk('WRLD');
	data << worldName;
	data.EndChunk(handle);
	
	const Modifier *modifier = modifierList.First();
	while (modifier)
	{
		PackHandle handle = data.BeginChunk('MDFR');
		modifier->PackType(data);
		modifier->Pack(data, packFlags);
		data.EndChunk(handle);
		
		modifier = modifier->Next();
	}
	
	data << TerminatorChunk;
}

void Instance::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Node::Unpack(data, unpackFlags);
	UnpackChunkList<Instance>(data, unpackFlags);
}

bool Instance::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'WRLD':
			
			data >> worldName;
			return (true);
		
		case 'MDFR':
		{
			Modifier *modifier = Modifier::Construct(data, unpackFlags);
			if (modifier)
			{
				modifier->Unpack(++data, unpackFlags);
				modifierList.Append(modifier);
				return (true);
			}
			
			break;
		}
	} 
	
	return (false); 
} 
 
void *Instance::BeginSettingsUnpack(void)
{ 
	modifierList.Purge();
	return (Node::BeginSettingsUnpack());
}
 
int32 Instance::GetCategoryCount(void) const
{
	return (Node::GetCategoryCount() + 1);
} 

Type Instance::GetCategoryType(int32 index, const char **title) const
{
	int32 count = Node::GetCategoryCount();
	if (index == count)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kNodeInstance));
		return (kNodeInstance);
	}
	
	return (Node::GetCategoryType(index, title));
}

int32 Instance::GetCategorySettingCount(Type category) const
{
	if (category == kNodeInstance) return (2);
	return (Node::GetCategorySettingCount(category));
}

Setting *Instance::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kNodeInstance)
	{
		if (flags & kConfigurationScript) return (nullptr);
		
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID(kNodeInstance, 'INST'));
			return (new HeadingSetting('INST', title));
		}
		
		if (index == 1)
		{
			const char *title = table->GetString(StringID(kNodeInstance, 'INST', 'WRLD'));
			const char *picker = table->GetString(StringID(kNodeInstance, 'INST', 'PICK'));
			return (new ResourceSetting('WRLD', worldName, title, picker, WorldResource::GetDescriptor()));
		}
		
		return (nullptr);
	}
	
	return (Node::GetCategorySetting(category, index, flags));
}

void Instance::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kNodeInstance)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'WRLD')
		{
			worldName = static_cast<const ResourceSetting *>(setting)->GetResourceName();
		}
	}
	else
	{
		Node::SetCategorySetting(category, setting);
	}
}

int32 Instance::GetInternalConnectorCount(void) const
{
	return (1);
}

const char *Instance::GetInternalConnectorKey(int32 index) const
{
	if (index == 0) return (kConnectorKeyShadow);
	return (nullptr);
}

bool Instance::ValidConnectedNode(const ConnectorKey& key, const Node *node) const
{
	if (key == kConnectorKeyShadow)
	{
		if (node->GetNodeType() == kNodeSpace) return (static_cast<const Space *>(node)->GetSpaceType() == kSpaceShadow);
		return (false);
	}
	
	return (Node::ValidConnectedNode(key, node));
}

void Instance::ExtractNodes(Node *rigidBodyNode, unsigned_int32 worldFlags)
{
	if (!(worldFlags & kWorldRestore))
	{
		Property *property = GetFirstProperty();
		while (property)
		{
			Property *next = property->Next();
			rigidBodyNode->AddProperty(property);
			property = next;
		}
		
		const Hub *hub = GetHub();
		if (hub)
		{
			Connector *connector = hub->GetFirstIncomingEdge();
			while (connector)
			{
				Connector *next = connector->GetNextIncomingEdge();
				connector->SetConnectorTarget(rigidBodyNode);
				connector = next;
			}
		}
		
		Transform4D transform = GetNodeTransform();
		Node *super = GetSuperNode();
		for (;;)
		{
			if (super->GetNodeType() == kNodeZone) break;
			transform = super->GetNodeTransform() * transform;
			super = super->GetSuperNode();
		}
		
		Node *node = GetFirstSubnode();
		do
		{
			Node *next = node->Next();
			
			node->SetNodeTransform(transform * node->GetNodeTransform());
			super->AddSubnode(node);
			node->SetPersistent();
			
			node = next;
		} while (node);
	}
	
	delete this;
}

bool Instance::ModifierCloneFilter(const Node *node, void *cookie)
{
	const Instance *instance = static_cast<Instance *>(cookie);
	
	const Modifier *modifier = instance->modifierList.First();
	do
	{
		if (!modifier->KeepNode(node)) return (false);
		modifier = modifier->Next();
	} while (modifier);
	
	return (true);
}

bool Instance::Expand(World *world)
{
	if (!GetFirstSubnode())
	{
		Modifier *modifier = modifierList.First();
		Node::CloneFilterProc *filterProc = (modifier) ? &ModifierCloneFilter : &Node::DefaultCloneFilter;
		
		Node *instanceRoot = world->NewInstancedWorld(worldName, filterProc, this);
		if (instanceRoot)
		{
			for (;;)
			{
				Node *node = instanceRoot->GetFirstSubnode();
				if (!node) break;
				AddSubnode(node);
			}
			
			if (!GetManipulator())
			{
				const Hub *instanceHub = instanceRoot->GetHub();
				if (instanceHub)
				{
					Node *node = GetFirstSubnode();
					while (node)
					{
						const Hub *hub = node->GetHub();
						if (hub)
						{
							Connector *connector = hub->GetFirstOutgoingEdge();
							while (connector)
							{
								const Hub *finish = connector->GetFinishElement();
								if (finish == instanceHub)
								{
									if ((node->GetNodeType() == kNodePortal) && (connector->GetConnectorKey() == kConnectorKeyZone))
									{
										static_cast<Portal *>(node)->SetConnectedZone(GetOwningZone());
									}
									else
									{
										connector->SetConnectorTarget(this);
									}
								}
								
								connector = connector->GetNextOutgoingEdge();
							}
						}
						
						node = GetNextNode(node);
					}
				}
			}
			
			delete instanceRoot;
			
			while (modifier)
			{
				modifier->Apply(world, this);
				modifier = modifier->Next();
			}
			
			if (!Enabled()) Disable();
			
			if (!GetManipulator())
			{
				Node *node = GetFirstSubnode();
				while (node)
				{
					const Controller *controller = node->GetController();
					if ((controller) && (controller->GetBaseControllerType() == kControllerRigidBody))
					{
						ExtractNodes(node, world->GetWorldFlags());
						return (false);
					}
					
					node = node->Next();
				}
			}
		}
	}
	
	return (true);
}

void Instance::Collapse(void)
{
	PurgeSubtree();
}

void Instance::Preprocess(void)
{
	SetNodeFlags(GetNodeFlags() | kNodeVisibilitySite);
	Node::Preprocess();
	
	if (GetFirstOutgoingEdge()) BondVisibility();
}

void Instance::Neutralize(void)
{
	ListElement<Instance>::Detach();
	Node::Neutralize();
}

void Instance::EnterZone(Zone *zone)
{
	zone->AddInstance(this);
}

// ZYURVUR
