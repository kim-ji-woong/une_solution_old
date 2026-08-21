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


#include "C4ExtrasPlugin.h"
#include "C4Configuration.h"
#include "C4World.h"


using namespace C4;


RotationController::RotationController() : Controller(kControllerRotation)
{
	rotationFlags = 0;
	
	rotationAngle = 0.0F;
	rotationSpeed = 0.0F;
	targetSpeed = 0.0F;
	acceleration = 0.0F;
	
	startAngle = 0.0F;
	finishAngle = 0.25F;
}

RotationController::RotationController(const RotationController& rotationController) : Controller(rotationController)
{
	rotationFlags = rotationController.rotationFlags & ~kRotationInitialized;
	
	rotationAngle = 0.0F;
	targetSpeed = 0.0F;
	acceleration = 0.0F;
	
	if (rotationController.acceleration == 0.0F) rotationSpeed = rotationController.rotationSpeed;
	else rotationSpeed = rotationController.targetSpeed;
	
	startAngle = rotationController.startAngle;
	finishAngle = rotationController.finishAngle;
}

RotationController::~RotationController()
{
}

Controller *RotationController::Replicate(void) const
{
	return (new RotationController(*this));
}

void RotationController::RegisterFunctions(ControllerRegistration *registration)
{
	const StringTable *table = TheExtrasPlugin->GetStringTable();
	
	static FunctionReg<ChangeRotationFunction> changeRotationRegistration(registration, kFunctionChangeRotation, table->GetString(StringID('CTRL', kControllerRotation, 'CROT')));
	static FunctionReg<SetRotationStateFunction> setRotationStateRegistration(registration, kFunctionSetRotationState, table->GetString(StringID('CTRL', kControllerRotation, 'STAT')));
}

void RotationController::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Controller::Pack(data, packFlags);
	
	data << ChunkHeader('flag', 4);
	data << rotationFlags;
	
	data << ChunkHeader('stat', 16);
	data << rotationAngle;
	data << rotationSpeed;
	data << targetSpeed;
	data << acceleration;
	
	if (rotationFlags & kRotationInitialized)
	{
		data << ChunkHeader('xfrm', sizeof(Transform4D));
		data << originalTransform;
	}
	
	if (rotationFlags & kRotationRestricted)
	{
		data << ChunkHeader('angl', 8);
		data << startAngle;
		data << finishAngle;
	}
	
	data << TerminatorChunk;
}

void RotationController::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Controller::Unpack(data, unpackFlags);
	UnpackChunkList<RotationController>(data, unpackFlags);
}

bool RotationController::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'flag':
			
			data >> rotationFlags;
			return (true);
		
		case 'stat':
			
			data >> rotationAngle;
			data >> rotationSpeed;
			data >> targetSpeed; 
			data >> acceleration;
			return (true); 
		 
		case 'xfrm': 
			
			data >> originalTransform; 
			return (true);
		
		case 'angl':
			 
			data >> startAngle;
			data >> finishAngle;
			return (true);
	} 
	
	return (false);
}

int32 RotationController::GetSettingCount(void) const
{
	return (6);
}

Setting *RotationController::GetSetting(int32 index) const
{
	const StringTable *table = TheExtrasPlugin->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRotation, 'SPED'));
		return (new TextSetting('SPED', rotationSpeed * 1000.0F, title));
	}
	
	if (index == 1)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRotation, 'RSTC'));
		return (new BooleanSetting('RSTC', ((rotationFlags & kRotationRestricted) != 0), title));
	}
	
	if (index == 2)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRotation, 'ANG1'));
		return (new TextSetting('ANG1', startAngle, title));
	}
	
	if (index == 3)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRotation, 'ANG2'));
		return (new TextSetting('ANG2', finishAngle, title));
	}
	
	if (index == 4)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRotation, 'DSAB'));
		return (new BooleanSetting('DSAB', ((rotationFlags & kRotationDisabled) != 0), title));
	}
	
	if (index == 5)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRotation, 'RVRS'));
		return (new BooleanSetting('RVRS', ((rotationFlags & kRotationReverse) != 0), title));
	}
	
	return (nullptr);
}

void RotationController::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'SPED')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		rotationSpeed = Text::StringToFloat(text) * 0.001F;
	}
	else if (identifier == 'RSTC')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) rotationFlags |= kRotationRestricted;
		else rotationFlags &= ~kRotationRestricted;
	}
	else if (identifier == 'ANG1')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		startAngle = Text::StringToFloat(text);
	}
	else if (identifier == 'ANG2')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		finishAngle = Text::StringToFloat(text);
	}
	else if (identifier == 'DSAB')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) rotationFlags |= kRotationDisabled;
		else rotationFlags &= ~kRotationDisabled;
	}
	else if (identifier == 'RVRS')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) rotationFlags |= kRotationReverse;
		else rotationFlags &= ~kRotationReverse;
	}
}

ControllerMessage *RotationController::ConstructMessage(ControllerMessageType type) const
{
	switch (type)
	{
		case kRotationMessageUpdate:
			
			return (new UpdateRotationMessage(GetControllerIndex()));
		
		case kRotationMessageState:
			
			return (new RotationStateMessage(GetControllerIndex()));
	}
	
	return (Controller::ConstructMessage(type));
}

void RotationController::ReceiveMessage(const ControllerMessage *message)
{
	switch (message->GetControllerMessageType())
	{
		case kRotationMessageUpdate:
		{
			const UpdateRotationMessage *m = static_cast<const UpdateRotationMessage *>(message);
			
			rotationAngle = m->GetRotationAngle();
			
			float time = m->GetAccelerationTime();
			if (time > K::min_float)
			{
				targetSpeed = m->GetRotationSpeed();
				acceleration = (targetSpeed - rotationSpeed) * 0.001F / time;
			}
			else
			{
				rotationSpeed = m->GetRotationSpeed();
				acceleration = 0.0F;
			}
			
			break;
		}
		
		case kRotationMessageState:
		{
			const RotationStateMessage *m = static_cast<const RotationStateMessage *>(message);
			
			rotationAngle = m->GetRotationAngle();
			rotationSpeed = m->GetRotationSpeed();
			rotationFlags = (rotationFlags & kRotationInitialized) | (m->GetRotationFlags() | kRotationRestricted);
			break;
		}
		
		default:
			
			Controller::ReceiveMessage(message);
			break;
	}
}

void RotationController::SendInitialStateMessages(Player *player) const
{
	unsigned_int32 flags = rotationFlags;
	if (flags & kRotationRestricted) player->SendMessage(RotationStateMessage(GetControllerIndex(), rotationAngle, rotationSpeed, flags));
	player->SendMessage(UpdateRotationMessage(GetControllerIndex(), rotationAngle, (acceleration == 0.0F) ? rotationSpeed : targetSpeed, 0.0F));
}

void RotationController::Preprocess(void)
{
	Controller::Preprocess();
	
	centerPosition = nullptr;
	
	Node *node = GetTargetNode();
	if (!node->GetManipulator())
	{
		unsigned_int32 flags = rotationFlags;
		if (!(flags & kRotationInitialized))
		{
			rotationFlags = flags | kRotationInitialized;
			originalTransform = node->GetNodeTransform();
		}
		
		const Node *centerNode = node->GetConnectedNode("CENT");
		if (centerNode)
		{
			centerPosition = &centerNode->GetWorldPosition();
			
			const Node *axisNode = node->GetConnectedNode("AXIS");
			if (axisNode) rotationAxis = (axisNode->GetWorldPosition() - centerNode->GetWorldPosition()).Normalize();
			else rotationAxis.Set(0.0F, 0.0F, 1.0F);
			
			if ((flags & (kRotationInitialized | kRotationDisabled | kRotationRestricted)) == (kRotationDisabled | kRotationRestricted))
			{
				UpdateRotationAngle((flags & kRotationReverse) ? finishAngle : startAngle);
				node->StopMotion();
			}
		}
		
		Node *subnode = node;
		do
		{
			if (subnode->GetNodeType() == kNodeGeometry)
			{
				GeometryObject *object = static_cast<Geometry *>(subnode)->GetObject();
				object->SetGeometryFlags(object->GetGeometryFlags() | kGeometryDynamic);
			}
			
			subnode = node->GetNextNode(subnode);
		} while (subnode);
	}
}

void RotationController::UpdateRotationAngle(float angle)
{
	Matrix3D	rotator;
	
	Node *node = GetTargetNode();
	const Transform4D& inverseTransform = node->GetSuperNode()->GetInverseWorldTransform();
	
	rotationAngle = angle;
	rotator.SetRotationAboutAxis(angle * K::two_pi, inverseTransform * rotationAxis);
	
	Point3D center = inverseTransform * *centerPosition;
	Transform4D transform(rotator, center - rotator * center);
	
	node->SetNodeTransform(transform * originalTransform);
	node->Invalidate();
}

void RotationController::Move(void)
{
	unsigned_int32 flags = rotationFlags;
	if ((!(flags & kRotationDisabled)) && (centerPosition))
	{
		float dt = TheTimeMgr->GetFloatDeltaTime();
		float angle = rotationAngle;
		
		if (flags & kRotationRestricted)
		{
			float speed = rotationSpeed;
			if (!(flags & kRotationReverse))
			{
				angle += speed * dt;
				
				if (speed > 0.0F)
				{
					if (angle > finishAngle)
					{
						angle = finishAngle;
						rotationFlags = flags | (kRotationDisabled | kRotationReverse);
						GetTargetNode()->StopMotion();
					}
				}
				else
				{
					if (angle < finishAngle)
					{
						angle = finishAngle;
						rotationFlags = flags | (kRotationDisabled | kRotationReverse);
						GetTargetNode()->StopMotion();
					}
				}
			}
			else
			{
				angle -= speed * dt;
				
				if (speed > 0.0F)
				{
					if (angle < startAngle)
					{
						angle = startAngle;
						rotationFlags = (flags | kRotationDisabled) & ~kRotationReverse;
						GetTargetNode()->StopMotion();
					}
				}
				else
				{
					if (angle > startAngle)
					{
						angle = startAngle;
						rotationFlags = (flags | kRotationDisabled) & ~kRotationReverse;
						GetTargetNode()->StopMotion();
					}
				}
			}
		}
		else
		{
			if (acceleration != 0.0F)
			{
				if (acceleration > 0.0F)
				{
					rotationSpeed += acceleration * dt;
					if (rotationSpeed > targetSpeed)
					{
						rotationSpeed = targetSpeed;
						acceleration = 0.0F;
					}
				}
				else
				{
					rotationSpeed += acceleration * dt;
					if (rotationSpeed < targetSpeed)
					{
						rotationSpeed = targetSpeed;
						acceleration = 0.0F;
					}
				}
			}
			
			angle += rotationSpeed * dt;
			if (angle > 1.0F) angle -= 1.0F;
			else if (angle < -1.0F) angle += 1.0F;
		}
		
		UpdateRotationAngle(angle);
	}
}

void RotationController::Activate(Node *trigger, Node *activator)
{
	rotationFlags &= ~kRotationDisabled;
}


ChangeRotationFunction::ChangeRotationFunction() : Function(kFunctionChangeRotation, kControllerRotation)
{
	rotationSpeed = 0.0F;
	accelerationTime = 0.0F;
}

ChangeRotationFunction::ChangeRotationFunction(const ChangeRotationFunction& changeRotationFunction) : Function(changeRotationFunction)
{
	rotationSpeed = changeRotationFunction.rotationSpeed;
	accelerationTime = changeRotationFunction.accelerationTime;
}

ChangeRotationFunction::~ChangeRotationFunction()
{
}

Function *ChangeRotationFunction::Replicate(void) const
{
	return (new ChangeRotationFunction(*this));
}

void ChangeRotationFunction::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Function::Pack(data, packFlags);
	
	data << rotationSpeed;
	data << accelerationTime;
}

void ChangeRotationFunction::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Function::Unpack(data, unpackFlags);
	
	data >> rotationSpeed;
	data >> accelerationTime;
}

int32 ChangeRotationFunction::GetSettingCount(void) const
{
	return (2);
}

Setting *ChangeRotationFunction::GetSetting(int32 index) const
{
	const StringTable *table = TheExtrasPlugin->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRotation, 'CROT', 'SPED'));
		return (new TextSetting('SPED', rotationSpeed * 1000.0F, title));
	}
	
	if (index == 1)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRotation, 'CROT', 'TIME'));
		return (new TextSetting('TIME', accelerationTime, title));
	}
	
	return (nullptr);
}

void ChangeRotationFunction::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'SPED')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		rotationSpeed = Text::StringToFloat(text) * 0.001F;
	}
	else if (identifier == 'TIME')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		accelerationTime = Text::StringToFloat(text);
	}
}

void ChangeRotationFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	RotationController *rotator = static_cast<RotationController *>(controller);
	TheMessageMgr->SendMessageAll(UpdateRotationMessage(rotator->GetControllerIndex(), rotator->GetRotationAngle(), rotationSpeed, accelerationTime));
	CallCompletionProc();
}


SetRotationStateFunction::SetRotationStateFunction() : Function(kFunctionSetRotationState, kControllerRotation)
{
	rotationFlags = 0;
}

SetRotationStateFunction::SetRotationStateFunction(const SetRotationStateFunction& setRotationStateFunction) : Function(setRotationStateFunction)
{
	rotationFlags = setRotationStateFunction.rotationFlags & ~kRotationInitialized;
}

SetRotationStateFunction::~SetRotationStateFunction()
{
}

Function *SetRotationStateFunction::Replicate(void) const
{
	return (new SetRotationStateFunction(*this));
}

void SetRotationStateFunction::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Function::Pack(data, packFlags);
	
	data << rotationFlags;
}

void SetRotationStateFunction::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Function::Unpack(data, unpackFlags);
	
	data >> rotationFlags;
}

int32 SetRotationStateFunction::GetSettingCount(void) const
{
	return (2);
}

Setting *SetRotationStateFunction::GetSetting(int32 index) const
{
	const StringTable *table = TheExtrasPlugin->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRotation, 'STAT', 'DSAB'));
		return (new BooleanSetting('DSAB', ((rotationFlags & kRotationDisabled) != 0), title));
	}
	
	if (index == 1)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRotation, 'STAT', 'RVRS'));
		return (new BooleanSetting('RVRS', ((rotationFlags & kRotationReverse) != 0), title));
	}
	
	return (nullptr);
}

void SetRotationStateFunction::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'DSAB')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) rotationFlags |= kRotationDisabled;
		else rotationFlags &= ~kRotationDisabled;
	}
	else if (identifier == 'RVRS')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) rotationFlags |= kRotationReverse;
		else rotationFlags &= ~kRotationReverse;
	}
}

void SetRotationStateFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	RotationController *rotator = static_cast<RotationController *>(controller);
	TheMessageMgr->SendMessageAll(RotationStateMessage(rotator->GetControllerIndex(), rotator->GetRotationAngle(), rotator->GetRotationSpeed(), rotationFlags));
	CallCompletionProc();
}


UpdateRotationMessage::UpdateRotationMessage(int32 controllerIndex) : ControllerMessage(RotationController::kRotationMessageUpdate, controllerIndex)
{
}

UpdateRotationMessage::UpdateRotationMessage(int32 controllerIndex, float angle, float speed, float time) : ControllerMessage(RotationController::kRotationMessageUpdate, controllerIndex)
{
	rotationAngle = angle;
	rotationSpeed = speed;
	accelerationTime = time;
}

UpdateRotationMessage::~UpdateRotationMessage()
{
}

void UpdateRotationMessage::Compress(Compressor& data) const
{
	ControllerMessage::Compress(data);
	
	data << rotationAngle;
	data << rotationSpeed;
	data << accelerationTime;
}

bool UpdateRotationMessage::Decompress(Decompressor& data)
{
	if (ControllerMessage::Decompress(data))
	{
		data >> rotationAngle;
		data >> rotationSpeed;
		data >> accelerationTime;
		return (true);
	}
	
	return (false);
}


RotationStateMessage::RotationStateMessage(int32 controllerIndex) : ControllerMessage(RotationController::kRotationMessageState, controllerIndex)
{
}

RotationStateMessage::RotationStateMessage(int32 controllerIndex, float angle, float speed, unsigned_int32 flags) : ControllerMessage(RotationController::kRotationMessageState, controllerIndex)
{
	rotationAngle = angle;
	rotationSpeed = speed;
	rotationFlags = flags;
}

RotationStateMessage::~RotationStateMessage()
{
}

void RotationStateMessage::Compress(Compressor& data) const
{
	ControllerMessage::Compress(data);
	
	data << rotationAngle;
	data << rotationSpeed;
	data << rotationFlags;
}

bool RotationStateMessage::Decompress(Decompressor& data)
{
	if (ControllerMessage::Decompress(data))
	{
		data >> rotationAngle;
		data >> rotationSpeed;
		data >> rotationFlags;
		return (true);
	}
	
	return (false);
}

// ZYURVUR
