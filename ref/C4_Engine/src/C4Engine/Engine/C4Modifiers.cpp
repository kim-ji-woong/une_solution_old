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


namespace C4
{
	template class Registrable<Modifier, ModifierRegistration>;
}


ModifierRegistration::ModifierRegistration(ModifierType type, const char *name) : Registration<Modifier, ModifierRegistration>(type)
{
	modifierName = name;
}

ModifierRegistration::~ModifierRegistration()
{
}


Modifier::Modifier(ModifierType type)
{
	modifierType = type;
}

Modifier::Modifier(const Modifier& modifier)
{
	modifierType = modifier.modifierType;
}

Modifier::~Modifier()
{
}

Modifier *Modifier::Replicate(void) const
{
	return (nullptr);
}

Modifier *Modifier::New(ModifierType type)
{
	Type	data[2];
	
	data[0] = type;
	data[1] = 0;
	
	Unpacker unpacker(data);
	return (Construct(unpacker));
}

bool Modifier::ValidInstance(const Instance *instance)
{
	return (true);
}

void Modifier::RegisterStandardModifiers(void)
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	static ModifierReg<AugmentInstanceModifier> augmentInstanceRegistration(kModifierAugmentInstance, table->GetString(StringID('MDFR', kModifierAugmentInstance)));
	static ModifierReg<WakeControllerModifier> wakeControllerRegistration(kModifierWakeController, table->GetString(StringID('MDFR', kModifierWakeController)));
	static ModifierReg<SleepControllerModifier> sleepControllerRegistration(kModifierSleepController, table->GetString(StringID('MDFR', kModifierSleepController)));
	static ModifierReg<ConnectInstanceModifier> connectInstanceRegistration(kModifierConnectInstance, table->GetString(StringID('MDFR', kModifierConnectInstance)));
	static ModifierReg<MoveConnectorModifier> moveConnectorRegistration(kModifierMoveConnector, table->GetString(StringID('MDFR', kModifierMoveConnector)));
	static ModifierReg<DeleteNodeModifier> deleteNodeRegistration(kModifierDeleteNode, table->GetString(StringID('MDFR', kModifierDeleteNode)));
	static ModifierReg<ReplaceMaterialModifier> replaceMaterialRegistration(kModifierReplaceMaterial, table->GetString(StringID('MDFR', kModifierReplaceMaterial)));
	static ModifierReg<RemovePhysicsModifier> removePhysicsRegistration(kModifierRemovePhysics, table->GetString(StringID('MDFR', kModifierRemovePhysics)));
	static ModifierReg<RemoveLightsModifier> removeLightssRegistration(kModifierRemoveLights, table->GetString(StringID('MDFR', kModifierRemoveLights)));
	static ModifierReg<RemoveSourcesModifier> removeSourcesRegistration(kModifierRemoveSources, table->GetString(StringID('MDFR', kModifierRemoveSources)));
}

void Modifier::PackType(Packer& data) const
{
	data << modifierType;
}

void Modifier::Apply(World *world, Instance *instance)
{
}

bool Modifier::KeepNode(const Node *node) const
{
	return (true);
}


AugmentInstanceModifier::AugmentInstanceModifier() : Modifier(kModifierAugmentInstance)
{
	worldName[0] = 0;
}

AugmentInstanceModifier::AugmentInstanceModifier(const char *name) : Modifier(kModifierAugmentInstance)
{
	worldName = name;
}

AugmentInstanceModifier::AugmentInstanceModifier(const AugmentInstanceModifier& augmentInstanceModifier) : Modifier(augmentInstanceModifier)
{ 
	worldName = augmentInstanceModifier.worldName;
} 
 
AugmentInstanceModifier::~AugmentInstanceModifier() 
{
} 

Modifier *AugmentInstanceModifier::Replicate(void) const
{
	return (new AugmentInstanceModifier(*this)); 
}

void AugmentInstanceModifier::Pack(Packer& data, unsigned_int32 packFlags) const
{ 
	Modifier::Pack(data, packFlags);
	
	PackHandle handle = data.BeginChunk('WRLD');
	data << worldName;
	data.EndChunk(handle);
	
	data << TerminatorChunk;
}

void AugmentInstanceModifier::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Modifier::Unpack(data, unpackFlags);
	UnpackChunkList<AugmentInstanceModifier>(data, unpackFlags);
}

bool AugmentInstanceModifier::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'WRLD':
			
			data >> worldName;
			return (true);
	}
	
	return (false);
}

int32 AugmentInstanceModifier::GetSettingCount(void) const
{
	return (1);
}

Setting *AugmentInstanceModifier::GetSetting(int32 index) const
{
	if (index == 0)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		const char *title = table->GetString(StringID('MDFR', kModifierAugmentInstance, 'WRLD'));
		const char *picker = table->GetString(StringID('MDFR', kModifierAugmentInstance, 'PICK'));
		return (new ResourceSetting('WRLD', worldName, title, picker, WorldResource::GetDescriptor()));
	}
	
	return (nullptr);
}

void AugmentInstanceModifier::SetSetting(const Setting *setting)
{
	if (setting->GetSettingIdentifier() == 'WRLD')
	{
		worldName = static_cast<const ResourceSetting *>(setting)->GetResourceName();
	}
}

void AugmentInstanceModifier::Apply(World *world, Instance *instance)
{
	Node *worldRoot = world->NewInstancedWorld(worldName);
	if (worldRoot)
	{
		Node *root = instance;
		Node *node = instance->GetFirstSubnode();
		while (node)
		{
			const Controller *controller = node->GetController();
			if ((controller) && (controller->GetBaseControllerType() == kControllerRigidBody))
			{
				root = node;
				break;
			}
			
			node = node->Next();
		}
		
		for (;;)
		{
			Node *node = worldRoot->GetFirstSubnode();
			if (!node) break;
			root->AddSubnode(node);
		}
		
		delete worldRoot;
	}
}


WakeControllerModifier::WakeControllerModifier() : Modifier(kModifierWakeController)
{
}

WakeControllerModifier::WakeControllerModifier(const WakeControllerModifier& wakeControllerModifier) : Modifier(wakeControllerModifier)
{
}

WakeControllerModifier::~WakeControllerModifier()
{
}

Modifier *WakeControllerModifier::Replicate(void) const
{
	return (new WakeControllerModifier(*this));
}

void WakeControllerModifier::Apply(World *world, Instance *instance)
{
	const Node *node = instance->GetFirstSubnode();
	while (node)
	{
		Controller *controller = node->GetController();
		if (controller) controller->SetControllerFlags(controller->GetControllerFlags() & ~kControllerAsleep);
		
		node = node->Next();
	}
}


SleepControllerModifier::SleepControllerModifier() : Modifier(kModifierSleepController)
{
}

SleepControllerModifier::SleepControllerModifier(const SleepControllerModifier& sleepControllerModifier) : Modifier(sleepControllerModifier)
{
}

SleepControllerModifier::~SleepControllerModifier()
{
}

Modifier *SleepControllerModifier::Replicate(void) const
{
	return (new SleepControllerModifier(*this));
}

void SleepControllerModifier::Apply(World *world, Instance *instance)
{
	const Node *node = instance->GetFirstSubnode();
	while (node)
	{
		Controller *controller = node->GetController();
		if (controller) controller->SetControllerFlags(controller->GetControllerFlags() | kControllerAsleep);
		
		node = node->Next();
	}
}


ConnectInstanceModifier::ConnectInstanceModifier() : Modifier(kModifierConnectInstance)
{
	connectorKey[0] = 0;
	targetNodeName[0] = 0;
}

ConnectInstanceModifier::ConnectInstanceModifier(const char *key, const char *name) : Modifier(kModifierConnectInstance)
{
	connectorKey = key;
	targetNodeName = name;
}

ConnectInstanceModifier::ConnectInstanceModifier(const ConnectInstanceModifier& connectInstanceModifier) : Modifier(connectInstanceModifier)
{
	connectorKey = connectInstanceModifier.connectorKey;
	targetNodeName = connectInstanceModifier.targetNodeName;
}

ConnectInstanceModifier::~ConnectInstanceModifier()
{
}

Modifier *ConnectInstanceModifier::Replicate(void) const
{
	return (new ConnectInstanceModifier(*this));
}

void ConnectInstanceModifier::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Modifier::Pack(data, packFlags);
	
	PackHandle handle = data.BeginChunk('CONN');
	data << connectorKey;
	data.EndChunk(handle);
	
	handle = data.BeginChunk('NAME');
	data << targetNodeName;
	data.EndChunk(handle);
	
	data << TerminatorChunk;
}

void ConnectInstanceModifier::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Modifier::Unpack(data, unpackFlags);
	UnpackChunkList<ConnectInstanceModifier>(data, unpackFlags);
}

bool ConnectInstanceModifier::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'CONN':
			
			data >> connectorKey;
			return (true);
		
		#if C4LEGACY
		
			case 'CKEY':
			{
				Type	key;
				
				data >> key;
				connectorKey = Text::TypeToString(key);
				return (true);
			}
		
		#endif
		
		case 'NAME':
			
			data >> targetNodeName;
			return (true);
	}
	
	return (false);
}

int32 ConnectInstanceModifier::GetSettingCount(void) const
{
	return (2);
}

Setting *ConnectInstanceModifier::GetSetting(int32 index) const
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('MDFR', kModifierConnectInstance, 'CONN'));
		return (new TextSetting('CONN', connectorKey, title, kMaxConnectorKeyLength, &Connector::ConnectorKeyFilter));
	}
	
	if (index == 1)
	{
		const char *title = table->GetString(StringID('MDFR', kModifierConnectInstance, 'NAME'));
		return (new TextSetting('NAME', targetNodeName, title, kMaxModifierNodeNameLength));
	}
	
	return (nullptr);
}

void ConnectInstanceModifier::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'CONN')
	{
		connectorKey = static_cast<const TextSetting *>(setting)->GetText();
	}
	else if (identifier == 'NAME')
	{
		targetNodeName = static_cast<const TextSetting *>(setting)->GetText();
	}
}

void ConnectInstanceModifier::Apply(World *world, Instance *instance)
{
	if (!instance->GetManipulator())
	{
		const Hub *hub = instance->GetHub();
		if (hub)
		{
			unsigned_int32 hash = Text::GetTextHash(targetNodeName);
			
			Connector *connector = hub->GetFirstOutgoingEdge();
			while (connector)
			{
				if (connector->GetConnectorKey() == connectorKey)
				{
					Node *node = instance->GetFirstSubnode();
					while (node)
					{
						if (node->GetNodeHash() == hash)
						{
							connector->SetConnectorTarget(node);
							break;
						}
						
						node = instance->GetNextNode(node);
					}
					
					break;
				}
				
				connector = connector->GetNextOutgoingEdge();
			}
		}
	}
}


MoveConnectorModifier::MoveConnectorModifier() : Modifier(kModifierMoveConnector)
{
	connectorKey[0] = 0;
	targetNodeName[0] = 0;
}

MoveConnectorModifier::MoveConnectorModifier(const char *key, const char *name) : Modifier(kModifierMoveConnector)
{
	connectorKey = key;
	targetNodeName = name;
}

MoveConnectorModifier::MoveConnectorModifier(const MoveConnectorModifier& moveConnectorModifier) : Modifier(moveConnectorModifier)
{
	connectorKey = moveConnectorModifier.connectorKey;
	targetNodeName = moveConnectorModifier.targetNodeName;
}

MoveConnectorModifier::~MoveConnectorModifier()
{
}

Modifier *MoveConnectorModifier::Replicate(void) const
{
	return (new MoveConnectorModifier(*this));
}

void MoveConnectorModifier::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Modifier::Pack(data, packFlags);
	
	PackHandle handle = data.BeginChunk('CONN');
	data << connectorKey;
	data.EndChunk(handle);
	
	handle = data.BeginChunk('NAME');
	data << targetNodeName;
	data.EndChunk(handle);
	
	data << TerminatorChunk;
}

void MoveConnectorModifier::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Modifier::Unpack(data, unpackFlags);
	UnpackChunkList<MoveConnectorModifier>(data, unpackFlags);
}

bool MoveConnectorModifier::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'CONN':
			
			data >> connectorKey;
			return (true);
		
		case 'NAME':
			
			data >> targetNodeName;
			return (true);
	}
	
	return (false);
}

int32 MoveConnectorModifier::GetSettingCount(void) const
{
	return (2);
}

Setting *MoveConnectorModifier::GetSetting(int32 index) const
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('MDFR', kModifierMoveConnector, 'CONN'));
		return (new TextSetting('CONN', connectorKey, title, kMaxConnectorKeyLength, &Connector::ConnectorKeyFilter));
	}
	
	if (index == 1)
	{
		const char *title = table->GetString(StringID('MDFR', kModifierMoveConnector, 'NAME'));
		return (new TextSetting('NAME', targetNodeName, title, kMaxModifierNodeNameLength));
	}
	
	return (nullptr);
}

void MoveConnectorModifier::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'CONN')
	{
		connectorKey = static_cast<const TextSetting *>(setting)->GetText();
	}
	else if (identifier == 'NAME')
	{
		targetNodeName = static_cast<const TextSetting *>(setting)->GetText();
	}
}

void MoveConnectorModifier::Apply(World *world, Instance *instance)
{
	if (!instance->GetManipulator())
	{
		const Hub *hub = instance->GetHub();
		if (hub)
		{
			unsigned_int32 hash = Text::GetTextHash(targetNodeName);
			
			Connector *connector = hub->GetFirstIncomingEdge();
			while (connector)
			{
				if (connector->GetConnectorKey() == connectorKey)
				{
					Node *node = instance->GetFirstSubnode();
					while (node)
					{
						if (node->GetNodeHash() == hash)
						{
							connector->SetConnectorTarget(node);
							connector->SetConnectorFlags(kConnectorSaveFinishPersistent);
							break;
						}
						
						node = instance->GetNextNode(node);
					}
					
					break;
				}
				
				connector = connector->GetNextIncomingEdge();
			}
		}
	}
}


DeleteNodeModifier::DeleteNodeModifier() : Modifier(kModifierDeleteNode)
{
	nodeHash = 0;
	nodeName[0] = 0;
}

DeleteNodeModifier::DeleteNodeModifier(const char *name) : Modifier(kModifierDeleteNode)
{
	nodeName = name;
	nodeHash = Text::GetTextHash(name);
}

DeleteNodeModifier::DeleteNodeModifier(const DeleteNodeModifier& deleteNodeModifier) : Modifier(deleteNodeModifier)
{
	nodeName = deleteNodeModifier.nodeName;
	nodeHash = Text::GetTextHash(nodeName);
}

DeleteNodeModifier::~DeleteNodeModifier()
{
}

Modifier *DeleteNodeModifier::Replicate(void) const
{
	return (new DeleteNodeModifier(*this));
}

void DeleteNodeModifier::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Modifier::Pack(data, packFlags);
	
	PackHandle handle = data.BeginChunk('NAME');
	data << nodeName;
	data.EndChunk(handle);
	
	data << TerminatorChunk;
}

void DeleteNodeModifier::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Modifier::Unpack(data, unpackFlags);
	UnpackChunkList<DeleteNodeModifier>(data, unpackFlags);
}

bool DeleteNodeModifier::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'NAME':
			
			data >> nodeName;
			nodeHash = Text::GetTextHash(nodeName);
			return (true);
	}
	
	return (false);
}

int32 DeleteNodeModifier::GetSettingCount(void) const
{
	return (1);
}

Setting *DeleteNodeModifier::GetSetting(int32 index) const
{
	if (index == 0)
	{
		const char *title = TheInterfaceMgr->GetStringTable()->GetString(StringID('MDFR', kModifierDeleteNode, 'NAME'));
		return (new TextSetting('NAME', nodeName, title, kMaxModifierNodeNameLength));
	}
	
	return (nullptr);
}

void DeleteNodeModifier::SetSetting(const Setting *setting)
{
	if (setting->GetSettingIdentifier() == 'NAME')
	{
		nodeName = static_cast<const TextSetting *>(setting)->GetText();
		nodeHash = Text::GetTextHash(nodeName);
	}
}

bool DeleteNodeModifier::KeepNode(const Node *node) const
{
	return (node->GetNodeHash() != nodeHash);
}


ReplaceMaterialModifier::ReplaceMaterialModifier() : Modifier(kModifierReplaceMaterial)
{
	nodeHash = 0;
	nodeName[0] = 0;
	
	materialObject = nullptr;
}

ReplaceMaterialModifier::ReplaceMaterialModifier(const char *name) : Modifier(kModifierReplaceMaterial)
{
	nodeName = name;
	nodeHash = Text::GetTextHash(name);
	
	materialObject = nullptr;
}

ReplaceMaterialModifier::ReplaceMaterialModifier(const ReplaceMaterialModifier& replaceMaterialModifier) : Modifier(replaceMaterialModifier)
{
	nodeName = replaceMaterialModifier.nodeName;
	nodeHash = Text::GetTextHash(nodeName);
	
	materialObject = replaceMaterialModifier.materialObject;
	if (materialObject) materialObject->Retain();
}

ReplaceMaterialModifier::~ReplaceMaterialModifier()
{
	if (materialObject) materialObject->Release();
}

Modifier *ReplaceMaterialModifier::Replicate(void) const
{
	return (new ReplaceMaterialModifier(*this));
}

void ReplaceMaterialModifier::Prepack(List<Object> *linkList) const
{
	if (materialObject) linkList->Append(materialObject);
}

void ReplaceMaterialModifier::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Modifier::Pack(data, packFlags);
	
	if (nodeName[0] != 0)
	{
		PackHandle handle = data.BeginChunk('NAME');
		data << nodeName;
		data.EndChunk(handle);
	}
	
	if ((materialObject) && (!(packFlags & kPackSettings)))
	{
		data << ChunkHeader('MATL', 4);
		data << materialObject->GetObjectIndex();
	}
	
	data << TerminatorChunk;
}

void ReplaceMaterialModifier::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Modifier::Unpack(data, unpackFlags);
	UnpackChunkList<ReplaceMaterialModifier>(data, unpackFlags);
}

bool ReplaceMaterialModifier::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'NAME':
			
			data >> nodeName;
			nodeHash = Text::GetTextHash(nodeName);
			return (true);
		
		case 'MATL':
		{
			int32	objectIndex;
			
			data >> objectIndex;
			data.AddObjectLink(objectIndex, &MaterialObjectLinkProc, this);
			return (true);
		}
	}
	
	return (false);
}

void *ReplaceMaterialModifier::BeginSettingsUnpack(void)
{
	nodeName[0] = 0;
	return (Modifier::BeginSettingsUnpack());
}

void ReplaceMaterialModifier::MaterialObjectLinkProc(Object *object, void *cookie)
{
	ReplaceMaterialModifier *replaceMaterialModifier = static_cast<ReplaceMaterialModifier *>(cookie);
	replaceMaterialModifier->SetMaterialObject(static_cast<MaterialObject *>(object));
}

int32 ReplaceMaterialModifier::GetSettingCount(void) const
{
	return (1);
}

Setting *ReplaceMaterialModifier::GetSetting(int32 index) const
{
	if (index == 0)
	{
		const char *title = TheInterfaceMgr->GetStringTable()->GetString(StringID('MDFR', kModifierReplaceMaterial, 'NAME'));
		return (new TextSetting('NAME', nodeName, title, kMaxModifierNodeNameLength));
	}
	
	return (nullptr);
}

void ReplaceMaterialModifier::SetSetting(const Setting *setting)
{
	if (setting->GetSettingIdentifier() == 'NAME')
	{
		nodeName = static_cast<const TextSetting *>(setting)->GetText();
		nodeHash = Text::GetTextHash(nodeName);
	}
}

void ReplaceMaterialModifier::SetMaterialObject(MaterialObject *object)
{
	if (materialObject != object)
	{
		if (materialObject) materialObject->Release();
		if (object) object->Retain();
		materialObject = object;
	}
}

void ReplaceMaterialModifier::Apply(World *world, Instance *instance)
{
	if (materialObject)
	{
		if (nodeHash == 0)
		{
			Node *node = instance->GetFirstSubnode();
			while (node)
			{
				if (node->GetNodeType() == kNodeGeometry)
				{
					Geometry *geometry = static_cast<Geometry *>(node);
					geometry->SetMaterialObject(0, materialObject);
				}
				
				node = instance->GetNextNode(node);
			}
		}
		else
		{
			Node *node = instance->GetFirstSubnode();
			while (node)
			{
				if (node->GetNodeHash() == nodeHash)
				{
					if (node->GetNodeType() == kNodeGeometry)
					{
						Geometry *geometry = static_cast<Geometry *>(node);
						geometry->SetMaterialObject(0, materialObject);
					}
				}
				
				node = instance->GetNextNode(node);
			}
		}
	}
}


RemovePhysicsModifier::RemovePhysicsModifier() : Modifier(kModifierRemovePhysics)
{
}

RemovePhysicsModifier::RemovePhysicsModifier(const RemovePhysicsModifier& removePhysicsModifier) : Modifier(removePhysicsModifier)
{
}

RemovePhysicsModifier::~RemovePhysicsModifier()
{
}

Modifier *RemovePhysicsModifier::Replicate(void) const
{
	return (new RemovePhysicsModifier(*this));
}

void RemovePhysicsModifier::Apply(World *world, Instance *instance)
{
	const Node *node = instance->GetFirstSubnode();
	while (node)
	{
		const Controller *controller = node->GetController();
		if ((controller) && (controller->GetBaseControllerType() == kControllerRigidBody)) delete controller;
		
		node = node->Next();
	}
}

bool RemovePhysicsModifier::KeepNode(const Node *node) const
{
	NodeType type = node->GetNodeType();
	return ((type != kNodeShape) && (type != kNodeJoint));
}


RemoveLightsModifier::RemoveLightsModifier() : Modifier(kModifierRemoveLights)
{
}

RemoveLightsModifier::RemoveLightsModifier(const RemoveLightsModifier& removeLightsModifier) : Modifier(removeLightsModifier)
{
}

RemoveLightsModifier::~RemoveLightsModifier()
{
}

Modifier *RemoveLightsModifier::Replicate(void) const
{
	return (new RemoveLightsModifier(*this));
}

bool RemoveLightsModifier::KeepNode(const Node *node) const
{
	return (node->GetNodeType() != kNodeLight);
}


RemoveSourcesModifier::RemoveSourcesModifier() : Modifier(kModifierRemoveSources)
{
}

RemoveSourcesModifier::RemoveSourcesModifier(const RemoveSourcesModifier& removeSourcesModifier) : Modifier(removeSourcesModifier)
{
}

RemoveSourcesModifier::~RemoveSourcesModifier()
{
}

Modifier *RemoveSourcesModifier::Replicate(void) const
{
	return (new RemoveSourcesModifier(*this));
}

bool RemoveSourcesModifier::KeepNode(const Node *node) const
{
	return (node->GetNodeType() != kNodeSource);
}

// ZYURVUR
