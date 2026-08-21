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
#include "C4Skybox.h"
#include "C4Water.h"
#include "C4Cloth.h"
#include "C4Panels.h"
#include "C4Scripts.h"
#include "C4Mover.h"
#include "C4Models.h"
#include "C4Shaders.h"


using namespace C4;


namespace C4
{
	template class Constructable<Controller>;
	template class Registrable<Controller, ControllerRegistration>;
}


FunctionRegistration::FunctionRegistration(ControllerRegistration *reg, FunctionType type, const char *name, unsigned_int32 flags)
{
	functionType = type;
	functionFlags = flags;
	functionName = name;
	
	reg->functionMap.Insert(this);
}

FunctionRegistration::~FunctionRegistration()
{
}


Function::Function(FunctionType funcType, ControllerType contType)
{
	functionType = funcType;
	controllerType = contType;
}

Function::Function(const Function& function)
{
	functionType = function.functionType;
	controllerType = function.controllerType;
}

Function::~Function()
{
}

void Function::Pack(Packer& data, unsigned_int32 packFlags) const
{
}

void Function::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
}

void Function::Compress(Compressor& data) const
{
}

bool Function::Decompress(Decompressor& data)
{
	return (true);
}

bool Function::OverridesFunction(const Function *function) const
{
	return (false);
}

void Function::Preprocess(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
}

void Function::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	CallCompletionProc();
}

void Function::Resume(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	Execute(controller, method, state);
}


ControllerRegistration::ControllerRegistration(ControllerType type, const char *name) : Registration<Controller, ControllerRegistration>(type)
{
	controllerName = name;
}

ControllerRegistration::~ControllerRegistration()
{
	functionMap.RemoveAll();
}

Function *ControllerRegistration::ConstructFunction(FunctionType type) const
{
	FunctionRegistration *reg = functionMap.Find(type);
	if (reg) return (reg->Construct()); 
	return (nullptr);
} 
 
 
Controller::Controller(ControllerType type)
{ 
	controllerType = type;
	baseControllerType = kControllerGeneric;
	
	controllerIndex = kControllerUnassigned; 
	controllerFlags = (type == kControllerGeneric) ? kControllerAsleep : 0;
	
	targetNode = nullptr;
} 

Controller::Controller(const Controller& controller)
{
	controllerType = controller.controllerType;
	baseControllerType = controller.baseControllerType;
	
	controllerIndex = kControllerUnassigned;
	controllerFlags = controller.controllerFlags & ~kControllerUpdate;
	
	targetNode = nullptr;
}

Controller::~Controller()
{
	Node *node = targetNode;
	if (node) node->SetController(nullptr);
}

Controller *Controller::Replicate(void) const
{
	return (new Controller(*this));
}

Controller *Controller::New(ControllerType type)
{
	Type	data[2];
	
	data[0] = type;
	data[1] = 0;
	
	Unpacker unpacker(data);
	return (Construct(unpacker));
}

bool Controller::ValidNode(const Node *node)
{
	return (true);
}

void Controller::RegisterStandardControllers(void)
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	static ControllerReg<Controller> genericRegistration(kControllerGeneric, table->GetString(StringID('CTRL', 'GNRC')));
	static ControllerReg<AnimationController> animationRegistration(kControllerAnimation, table->GetString(StringID('CTRL', kControllerAnimation)));
	static ControllerReg<SkinController> skinRegistration(kControllerSkin, table->GetString(StringID('CTRL', kControllerSkin)));
	static ControllerReg<ScriptController> scriptRegistration(kControllerScript, table->GetString(StringID('CTRL', kControllerScript)));
	static ControllerReg<PanelController> panelRegistration(kControllerPanel, table->GetString(StringID('CTRL', kControllerPanel)));
	static ControllerReg<RigidBodyController> rigidBodyRegistration(kControllerRigidBody, table->GetString(StringID('CTRL', kControllerRigidBody)));
	static ControllerReg<MoverController> moverRegistration(kControllerMover, table->GetString(StringID('CTRL', kControllerMover)));
	static ControllerReg<PhysicsController> physicsRegistration(kControllerPhysics, table->GetString(StringID('CTRL', kControllerPhysics)));
	static ControllerReg<ClothController> clothRegistration(kControllerCloth, table->GetString(StringID('CTRL', kControllerCloth)));
	static ControllerReg<WaterController> waterRegistration(kControllerWater, table->GetString(StringID('CTRL', kControllerWater)));
	
	AnimationController::RegisterFunctions(&animationRegistration);
	PanelController::RegisterFunctions(&panelRegistration);
	MoverController::RegisterFunctions(&moverRegistration);
	PhysicsController::RegisterFunctions(&physicsRegistration);
	WaterController::RegisterFunctions(&waterRegistration);
}

void Controller::PackType(Packer& data) const
{
	data << controllerType;
}

void Controller::Pack(Packer& data, unsigned_int32 packFlags) const
{
	data << ChunkHeader('INDX', 4);
	data << controllerIndex;
	
	data << ChunkHeader('FLAG', 4);
	data << int32(controllerFlags & ~kControllerUpdate);
	
	data << TerminatorChunk;
}

void Controller::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<Controller>(data, unpackFlags);
	
	if (unpackFlags & kUnpackNonpersistent) controllerIndex = kControllerUnassigned;
}

bool Controller::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'INDX':
			
			data >> controllerIndex;
			return (true);
		
		case 'FLAG':
			
			data >> controllerFlags;
			return (true);
	}
	
	return (false);
}

void Controller::SetTargetNode(Node *node)
{
	if (targetNode != node)
	{
		if (!node)
		{
			World *world = targetNode->GetWorld();
			if (world) world->RemoveController(this);
		}
		
		targetNode = node;
	}
}

void Controller::Preprocess(void)
{
	World *world = targetNode->GetWorld();
	if ((world) && (!targetNode->GetManipulator()))
	{
		if (controllerType == kControllerGeneric) controllerFlags |= kControllerMoveInhibit;
		world->AddController(this);
	}
}

void Controller::Neutralize(void)
{
	World *world = targetNode->GetWorld();
	if (world) world->RemoveController(this);
	
	controllerIndex = kControllerUnassigned;
}

ControllerMessage *Controller::ConstructMessage(ControllerMessageType type) const
{
	switch (type)
	{
		case kControllerMessageSetting:
			
			return (new SettingMessage(GetControllerIndex()));
		
		case kControllerMessageFunction:
			
			return (new FunctionMessage(GetControllerIndex()));
		
		case kControllerMessageWake:
		case kControllerMessageSleep:
			
			return (new WakeSleepMessage(type, GetControllerIndex()));
		
		case kControllerMessageEnableNode:
		case kControllerMessageDisableNode:
			
			return (new NodeEnableDisableMessage(type, GetControllerIndex()));
		
		case kControllerMessageDeleteNode:
			
			return (new DeleteNodeMessage(GetControllerIndex()));
		
		case kControllerMessageEnableInteractivity:
		case kControllerMessageDisableInteractivity:
			
			return (new NodeInteractivityMessage(type, GetControllerIndex()));
		
		case kControllerMessageShowGeometry:
		case kControllerMessageHideGeometry:
			
			return (new GeometryVisibilityMessage(type, GetControllerIndex()));
		
		case kControllerMessagePlaySource:
		case kControllerMessageStopSource:
			
			return (new SourcePlayStopMessage(type, GetControllerIndex()));
		
		case kControllerMessageMaterialColor:
			
			return (new MaterialColorMessage(GetControllerIndex()));
		
		case kControllerMessageShaderParameter:
			
			return (new ShaderParameterMessage(GetControllerIndex()));
	}
	
	return (nullptr);
}

void Controller::ReceiveMessage(const ControllerMessage *message)
{
}

void Controller::SendInitialStateMessages(Player *player) const
{
}

void Controller::EnterWorld(Zone *zone, const Point3D& zonePosition)
{
}

void Controller::ChangeZones(Zone *zone, const Transform4D& transform)
{
}

void Controller::Wake(void)
{
	controllerFlags &= ~kControllerAsleep;
	
	World *world = targetNode->GetWorld();
	if (world) world->WakeController(this);
}

void Controller::Sleep(void)
{
	controllerFlags |= kControllerAsleep;
	World::SleepController(this);
}

void Controller::Move(void)
{
}

void Controller::StopMotion(void)
{
}

void Controller::Update(void)
{
	controllerFlags &= ~kControllerUpdate;
}

void Controller::SetDetailLevel(int32 level)
{
	Invalidate();
}

void Controller::Activate(Node *trigger, Node *activator)
{
}

void Controller::Deactivate(Node *trigger)
{
}

void Controller::HandleInteractionEvent(InteractionEventType type, const Point3D *position, Node *activator)
{
}


SettingMessage::SettingMessage(int32 controllerIndex) : ControllerMessage(Controller::kControllerMessageSetting, controllerIndex)
{
	messageSetting = nullptr;
}

SettingMessage::SettingMessage(int32 controllerIndex, Type category, const Setting *setting) : ControllerMessage(Controller::kControllerMessageSetting, controllerIndex)
{
	settingCategory = category;
	messageSetting = setting->Clone();
}

SettingMessage::~SettingMessage()
{
	delete messageSetting;
}

void SettingMessage::Compress(Compressor& data) const
{
	ControllerMessage::Compress(data);
	
	data << messageSetting->GetSettingType();
	data << settingCategory;
	
	messageSetting->Compress(data);
}

bool SettingMessage::Decompress(Decompressor& data)
{
	if (ControllerMessage::Decompress(data))
	{
		Type	unpackData[2];
		
		data >> unpackData[0];
		unpackData[1] = 0;
		
		Unpacker unpacker(unpackData);
		Setting *setting = Setting::Construct(unpacker);
		if (setting)
		{
			data >> settingCategory;
			
			setting->Decompress(data);
			messageSetting = setting;
			return (true);
		}
	}
	
	return (false);
}

bool SettingMessage::HandleControllerMessage(Controller *controller) const
{
	Node *node = controller->GetTargetNode();
	
	Object *object = node->GetObject();
	object->SetCategorySetting(settingCategory, messageSetting);
	object->SetModifiedFlag();
	
	node->ProcessObjectSettings();
	return (true);
}

bool SettingMessage::OverridesMessage(const ControllerMessage *message) const
{
	if (message->GetControllerMessageType() == Controller::kControllerMessageSetting)
	{
		const Setting *setting = static_cast<const SettingMessage *>(message)->GetSetting();
		return (setting->GetSettingIdentifier() == messageSetting->GetSettingIdentifier());
	}
	
	return (false);
}


FunctionMessage::FunctionMessage(int32 controllerIndex) : ControllerMessage(Controller::kControllerMessageFunction, controllerIndex)
{
	messageFunction = nullptr;
}

FunctionMessage::FunctionMessage(int32 controllerIndex, const Function *function) : ControllerMessage(Controller::kControllerMessageFunction, controllerIndex)
{
	messageFunction = function->Clone();
	messageFunction->SetCompletionProc(function->GetCompletionProc(), function->GetCompletionCookie());
}

FunctionMessage::~FunctionMessage()
{
	delete messageFunction;
}

void FunctionMessage::Compress(Compressor& data) const
{
	ControllerMessage::Compress(data);
	
	data << messageFunction->GetFunctionType();
	messageFunction->Compress(data);
}

bool FunctionMessage::Decompress(Decompressor& data)
{
	if (ControllerMessage::Decompress(data))
	{
		const Controller *controller = TheWorldMgr->GetWorld()->GetController(GetControllerIndex());
		const ControllerRegistration *registration = Controller::FindRegistration(controller->GetControllerType());
		if (registration)
		{
			FunctionType	functionType;
			
			data >> functionType;
			
			Function *function = registration->ConstructFunction(functionType);
			if (function)
			{
				function->Decompress(data);
				messageFunction = function;
				return (true);
			}
		}
	}
	
	return (false);
}

bool FunctionMessage::HandleControllerMessage(Controller *controller) const
{
	messageFunction->Execute(controller, nullptr, nullptr);
	return (true);
}

bool FunctionMessage::OverridesMessage(const ControllerMessage *message) const
{
	if (message->GetControllerMessageType() == Controller::kControllerMessageFunction)
	{
		const Function *function = static_cast<const FunctionMessage *>(message)->GetFunction();
		return (messageFunction->OverridesFunction(function));
	}
	
	return (false);
}


WakeSleepMessage::WakeSleepMessage(ControllerMessageType type, int32 controllerIndex) : ControllerMessage(type, controllerIndex)
{
}

WakeSleepMessage::~WakeSleepMessage()
{
}

bool WakeSleepMessage::HandleControllerMessage(Controller *controller) const
{
	if (GetControllerMessageType() == Controller::kControllerMessageWake) controller->Wake();
	else controller->Sleep();
	
	return (true);
}

bool WakeSleepMessage::OverridesMessage(const ControllerMessage *message) const
{
	ControllerMessageType type = message->GetControllerMessageType();
	return ((type == Controller::kControllerMessageWake) || (type == Controller::kControllerMessageSleep));
}


NodeEnableDisableMessage::NodeEnableDisableMessage(ControllerMessageType type, int32 controllerIndex) : ControllerMessage(type, controllerIndex)
{
}

NodeEnableDisableMessage::~NodeEnableDisableMessage()
{
}

bool NodeEnableDisableMessage::HandleControllerMessage(Controller *controller) const
{
	Node *node = controller->GetTargetNode();
	
	if (GetControllerMessageType() == Controller::kControllerMessageEnableNode) node->Enable();
	else node->Disable();
	
	return (true);
}

bool NodeEnableDisableMessage::OverridesMessage(const ControllerMessage *message) const
{
	ControllerMessageType type = message->GetControllerMessageType();
	return ((type == Controller::kControllerMessageEnableNode) || (type == Controller::kControllerMessageDisableNode));
}


DeleteNodeMessage::DeleteNodeMessage(int32 controllerIndex) : ControllerMessage(Controller::kControllerMessageDeleteNode, controllerIndex, kMessageDestroyer)
{
}

DeleteNodeMessage::~DeleteNodeMessage()
{
}

bool DeleteNodeMessage::HandleControllerMessage(Controller *controller) const
{
	delete controller->GetTargetNode();
	return (true);
}


NodeInteractivityMessage::NodeInteractivityMessage(ControllerMessageType type, int32 controllerIndex) : ControllerMessage(type, controllerIndex)
{
}

NodeInteractivityMessage::~NodeInteractivityMessage()
{
}

bool NodeInteractivityMessage::HandleControllerMessage(Controller *controller) const
{
	Property *property = controller->GetTargetNode()->GetProperty(kPropertyInteraction);
	if (property)
	{
		unsigned_int32 flags = property->GetPropertyFlags();
		
		if (GetControllerMessageType() == Controller::kControllerMessageEnableInteractivity) flags &= ~kPropertyDisabled;
		else flags |= kPropertyDisabled;
		
		property->SetPropertyFlags(flags);
	}
	
	return (true);
}

bool NodeInteractivityMessage::OverridesMessage(const ControllerMessage *message) const
{
	ControllerMessageType type = message->GetControllerMessageType();
	return ((type == Controller::kControllerMessageEnableInteractivity) || (type == Controller::kControllerMessageDisableInteractivity));
}


GeometryVisibilityMessage::GeometryVisibilityMessage(ControllerMessageType type, int32 controllerIndex) : ControllerMessage(type, controllerIndex)
{
}

GeometryVisibilityMessage::~GeometryVisibilityMessage()
{
}

bool GeometryVisibilityMessage::HandleControllerMessage(Controller *controller) const
{
	ControllerMessageType type = GetControllerMessageType();
	
	Node *root = controller->GetTargetNode();
	Node *node = root;
	do
	{
		if (node->GetNodeType() == kNodeGeometry)
		{
			GeometryObject *object = static_cast<Geometry *>(node)->GetObject();
			unsigned_int32 flags = object->GetGeometryFlags();
			
			if (type == Controller::kControllerMessageShowGeometry) flags &= ~kGeometryInvisible;
			else flags |= kGeometryInvisible;
			
			object->SetGeometryFlags(flags);
			object->SetModifiedFlag();
		}
		
		node = root->GetNextNode(node);
	} while (node);
	
	return (true);
}

bool GeometryVisibilityMessage::OverridesMessage(const ControllerMessage *message) const
{
	ControllerMessageType type = message->GetControllerMessageType();
	return ((type == Controller::kControllerMessageShowGeometry) || (type == Controller::kControllerMessageHideGeometry));
}


SourcePlayStopMessage::SourcePlayStopMessage(ControllerMessageType type, int32 controllerIndex) : ControllerMessage(type, controllerIndex)
{
}

SourcePlayStopMessage::~SourcePlayStopMessage()
{
}

bool SourcePlayStopMessage::HandleControllerMessage(Controller *controller) const
{
	Node *node = controller->GetTargetNode();
	if (node->GetNodeType() == kNodeSource)
	{
		Source *source = static_cast<Source *>(node);
		
		if (GetControllerMessageType() == Controller::kControllerMessagePlaySource) source->Play();
		else source->Stop();
	}
	
	return (true);
}

bool SourcePlayStopMessage::OverridesMessage(const ControllerMessage *message) const
{
	ControllerMessageType type = message->GetControllerMessageType();
	return ((type == Controller::kControllerMessagePlaySource) || (type == Controller::kControllerMessageStopSource));
}


MaterialColorMessage::MaterialColorMessage(int32 controllerIndex) : ControllerMessage(Controller::kControllerMessageMaterialColor, controllerIndex)
{
}

MaterialColorMessage::MaterialColorMessage(int32 controllerIndex, AttributeType type, const ColorRGBA& color) : ControllerMessage(Controller::kControllerMessageMaterialColor, controllerIndex)
{
	attributeType = type;
	materialColor = color;
}

MaterialColorMessage::~MaterialColorMessage()
{
}

void MaterialColorMessage::Compress(Compressor& data) const
{
	ControllerMessage::Compress(data);
	
	data << attributeType;
	data << materialColor;
}

bool MaterialColorMessage::Decompress(Decompressor& data)
{
	if (ControllerMessage::Decompress(data))
	{
		data >> attributeType;
		data >> materialColor;
		return (true);
	}
	
	return (false);
}

bool MaterialColorMessage::HandleControllerMessage(Controller *controller) const
{
	Node *node = controller->GetTargetNode();
	
	NodeType type = node->GetNodeType();
	if (type == kNodeGeometry)
	{
		const Geometry *geometry = static_cast<Geometry *>(node);
		
		int32 count = geometry->GetMaterialCount();
		for (machine a = 0; a < count; a++)
		{
			MaterialObject *object = geometry->GetMaterialObject(a);
			if (object)
			{
				Attribute *attribute = object->FindAttribute(attributeType);
				if (attribute)
				{
					attribute->SetAttributeColor(materialColor);
					object->SetModifiedFlag();
				}
			}
		}
	}
	else if (type == kNodeSkybox)
	{
		const Skybox *skybox = static_cast<Skybox *>(node);
		
		MaterialObject *object = skybox->GetMaterialObject();
		if (object)
		{
			Attribute *attribute = object->FindAttribute(attributeType);
			if (attribute)
			{
				attribute->SetAttributeColor(materialColor);
				object->SetModifiedFlag();
			}
		}
	}
	
	return (true);
}

bool MaterialColorMessage::OverridesMessage(const ControllerMessage *message) const
{
	return (message->GetControllerMessageType() == Controller::kControllerMessageMaterialColor);
}


ShaderParameterMessage::ShaderParameterMessage(int32 controllerIndex) : ControllerMessage(Controller::kControllerMessageShaderParameter, controllerIndex)
{
}

ShaderParameterMessage::ShaderParameterMessage(int32 controllerIndex, int32 slot, const Vector4D& param) : ControllerMessage(Controller::kControllerMessageShaderParameter, controllerIndex)
{
	parameterSlot = slot;
	parameterValue = param;
}

ShaderParameterMessage::~ShaderParameterMessage()
{
}

void ShaderParameterMessage::Compress(Compressor& data) const
{
	ControllerMessage::Compress(data);
	
	data << parameterSlot;
	data << parameterValue;
}

bool ShaderParameterMessage::Decompress(Decompressor& data)
{
	if (ControllerMessage::Decompress(data))
	{
		data >> parameterSlot;
		data >> parameterValue;
		return (true);
	}
	
	return (false);
}

bool ShaderParameterMessage::HandleControllerMessage(Controller *controller) const
{
	Node *node = controller->GetTargetNode();
	
	if (node->GetNodeType() == kNodeGeometry)
	{
		const Geometry *geometry = static_cast<Geometry *>(node);
		
		int32 count = geometry->GetMaterialCount();
		for (machine a = 0; a < count; a++)
		{
			MaterialObject *object = geometry->GetMaterialObject(a);
			if (object)
			{
				Attribute *attribute = object->GetFirstAttribute();
				if ((attribute) && (attribute->GetAttributeType() == kAttributeShader))
				{
					static_cast<ShaderAttribute *>(attribute)->SetParameterValue(parameterSlot, parameterValue);
					object->SetModifiedFlag();
				}
			}
		}
	}
	
	return (true);
}

bool ShaderParameterMessage::OverridesMessage(const ControllerMessage *message) const
{
	if (message->GetControllerMessageType() != Controller::kControllerMessageShaderParameter) return (false);
	return (static_cast<const ShaderParameterMessage *>(message)->GetParameterSlot() == parameterSlot);
}

// ZYURVUR
