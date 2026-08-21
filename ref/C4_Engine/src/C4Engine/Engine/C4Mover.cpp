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


#include "C4Simulation.h"
#include "C4Mover.h"
#include "C4Configuration.h"


using namespace C4;


MoverController::MoverController() : RigidBodyController(kControllerMover)
{
	moverState = 0;
	moverSpeed = 1.0F;
	
	beginningConnectorKey = "begin";
	endingConnectorKey = "end";
}

MoverController::MoverController(const MoverController& moverController) : RigidBodyController(moverController)
{
	moverState = 0;
	moverSpeed = moverController.moverSpeed;
	beginningConnectorKey = moverController.beginningConnectorKey;
	endingConnectorKey = moverController.endingConnectorKey;
}

MoverController::~MoverController()
{
}

Controller *MoverController::Replicate(void) const
{
	return (new MoverController(*this));
}

void MoverController::RegisterFunctions(ControllerRegistration *registration)
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	static FunctionReg<MoveToBeginningFunction> moveToBeginningRegistration(registration, kFunctionMoveToBeginning, table->GetString(StringID('CTRL', kControllerMover, kFunctionMoveToBeginning)), kFunctionRemote | kFunctionJournaled);
	static FunctionReg<MoveToEndingFunction> moveToEndingRegistration(registration, kFunctionMoveToEnding, table->GetString(StringID('CTRL', kControllerMover, kFunctionMoveToEnding)), kFunctionRemote | kFunctionJournaled);
	static FunctionReg<StopMoverFunction> stopMoverRegistration(registration, kFunctionStopMover, table->GetString(StringID('CTRL', kControllerMover, kFunctionStopMover)), kFunctionRemote | kFunctionJournaled);
}

void MoverController::Pack(Packer& data, unsigned_int32 packFlags) const
{
	RigidBodyController::Pack(data, packFlags);
	
	data << ChunkHeader('STAT', 4);
	data << moverState;
	
	data << ChunkHeader('SPED', 4);
	data << moverSpeed;
	
	PackHandle handle = data.BeginChunk('BCON');
	data << beginningConnectorKey;
	data.EndChunk(handle);
	
	handle = data.BeginChunk('ECON');
	data << endingConnectorKey;
	data.EndChunk(handle);
	
	if (moverState & kMoverInitialized)
	{
		data << ChunkHeader('IMAS', 4);
		data << moverInverseMass;
		
		data << ChunkHeader('OFST', sizeof(Vector3D));
		data << moverOffset;
		
		data << ChunkHeader('POSI', sizeof(Point3D) * 2);
		data << beginningPosition;
		data << endingPosition;
	}
	
	data << TerminatorChunk;
}

void MoverController::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	RigidBodyController::Unpack(data, unpackFlags);
	UnpackChunkList<MoverController>(data, unpackFlags);
}

bool MoverController::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'STAT':
			
			data >> moverState;
			return (true);
		
		case 'SPED':
			
			data >> moverSpeed;
			return (true);
		
		case 'BCON':
			
			data >> beginningConnectorKey;
			return (true);
		 
		case 'ECON':
			 
			data >> endingConnectorKey; 
			return (true); 
		
		#if C4LEGACY 
		
			case 'BEGN':
			{
				Type	key; 
				
				data >> key;
				beginningConnectorKey = Text::TypeToString(key);
				return (true); 
			}
			
			case 'ENDG':
			{
				Type	key;
				
				data >> key;
				endingConnectorKey = Text::TypeToString(key);
				return (true);
			}
		
		#endif
		
		case 'IMAS':
			
			data >> moverInverseMass;
			return (true);
		
		case 'OFST':
			
			data >> moverOffset;
			return (true);
		
		case 'POSI':
			
			data >> beginningPosition;
			data >> endingPosition;
			return (true);
	}
	
	return (false);
}

int32 MoverController::GetSettingCount(void) const
{
	return (3);
}

Setting *MoverController::GetSetting(int32 index) const
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerMover, 'SPED'));
		return (new TextSetting('SPED', moverSpeed, title));
	}
	
	if (index == 1)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerMover, 'BCON'));
		return (new TextSetting('BCON', beginningConnectorKey, title, kMaxConnectorKeyLength, &Connector::ConnectorKeyFilter));
	}
	
	if (index == 2)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerMover, 'ECON'));
		return (new TextSetting('ECON', endingConnectorKey, title, kMaxConnectorKeyLength, &Connector::ConnectorKeyFilter));
	}
	
	return (nullptr);
}

void MoverController::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'SPED')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		moverSpeed = FmaxZero(Text::StringToFloat(text));
	}
	else if (identifier == 'BCON')
	{
		beginningConnectorKey = static_cast<const TextSetting *>(setting)->GetText();
	}
	else if (identifier == 'ECON')
	{
		endingConnectorKey = static_cast<const TextSetting *>(setting)->GetText();
	}
}

void MoverController::Preprocess(void)
{
	SetRigidBodyFlags(kRigidBodyFixedOrientation);
	RigidBodyController::Preprocess();
	
	const Node *node = GetTargetNode();
	if (!node->GetManipulator())
	{
		unsigned_int32 state = moverState;
		if (!(state & kMoverInitialized))
		{
			Node *begin = node->GetConnectedNode(beginningConnectorKey);
			Node *end = node->GetConnectedNode(endingConnectorKey);
			if ((begin) && (end))
			{
				moverState = state | kMoverInitialized;
				
				beginningPosition = begin->GetWorldPosition();
				endingPosition = end->GetWorldPosition();
				moverOffset = GetFinalWorldPosition() - beginningPosition;
				
				moverInverseMass = GetInverseBodyMass();
				SetInverseBodyMass(0.0F);
			}
		}
		
		if (moverState & kMoverInitialized) movementDirection = (endingPosition - beginningPosition).Normalize();
		
		SetGravityMultiplier(0.0F);
		SetFrictionCoefficient(0.0F);
	}
}

void MoverController::Move(void)
{
	unsigned_int32 state = moverState;
	if (state & kMoverActive)
	{
		Point3D position = GetFinalWorldPosition() - moverOffset;
		float speed = GetLinearVelocity() * movementDirection;
		
		if (!(state & kMoverReverse))
		{
			if (movementDirection * (position - endingPosition) < kContactEpsilon)
			{
				SetExternalForce(movementDirection * ((moverSpeed - speed) * 20.0F));
			}
			else
			{
				const Transform4D& transform = GetTargetNode()->GetSuperNode()->GetInverseWorldTransform();
				SetRigidBodyPosition(transform * (endingPosition + moverOffset));
				Stop();
				
				PostEvent(kMoverEventReachedEnding);
			}
		}
		else
		{
			if (movementDirection * (position - beginningPosition) > -kContactEpsilon)
			{
				SetExternalForce(movementDirection * ((moverSpeed + speed) * -20.0F));
			}
			else
			{
				const Transform4D& transform = GetTargetNode()->GetSuperNode()->GetInverseWorldTransform();
				SetRigidBodyPosition(transform * (beginningPosition + moverOffset));
				Stop();
				
				PostEvent(kMoverEventReachedBeginning);
			}
		}
	}
}

void MoverController::MoveToBeginning(void)
{
	unsigned_int32 state = moverState;
	if (state & kMoverInitialized)
	{
		moverState = state | (kMoverActive | kMoverReverse);
		
		SetInverseBodyMass(moverInverseMass);
		SetRigidBodyFlags(GetRigidBodyFlags() | kRigidBodyKeepAwake);
		if (RigidBodyAsleep()) Wake();
	}
}

void MoverController::MoveToEnding(void)
{
	unsigned_int32 state = moverState;
	if (state & kMoverInitialized)
	{
		moverState = (state & ~kMoverReverse) | kMoverActive;
		
		SetInverseBodyMass(moverInverseMass);
		SetRigidBodyFlags(GetRigidBodyFlags() | kRigidBodyKeepAwake);
		if (RigidBodyAsleep()) Wake();
	}
}

void MoverController::Stop(void)
{
	moverState &= ~kMoverActive;
	
	SetInverseBodyMass(0.0F);
	SetExternalForce(Zero3D);
	SetLinearVelocity(Zero3D);
	SetRigidBodyFlags(GetRigidBodyFlags() & ~kRigidBodyKeepAwake);
	
	PurgeOutgoingEdges();
	PurgeIncomingEdges();
	Sleep();
}


MoveToBeginningFunction::MoveToBeginningFunction() :
		Function(kFunctionMoveToBeginning, kControllerMover),
		Observer<MoveToBeginningFunction, MoverController>(this, &MoveToBeginningFunction::HandleEvent)
{
}

MoveToBeginningFunction::MoveToBeginningFunction(const MoveToBeginningFunction& moveToBeginningFunction) :
		Function(moveToBeginningFunction),
		Observer<MoveToBeginningFunction, MoverController>(this, &MoveToBeginningFunction::HandleEvent)
{
}

MoveToBeginningFunction::~MoveToBeginningFunction()
{
}

Function *MoveToBeginningFunction::Replicate(void) const
{
	return (new MoveToBeginningFunction(*this));
}

bool MoveToBeginningFunction::OverridesFunction(const Function *function) const
{
	FunctionType type = function->GetFunctionType();
	return ((type == kFunctionMoveToBeginning) || (type == kFunctionMoveToEnding) || (type == kFunctionStopMover));
}

void MoveToBeginningFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	MoverController *mover = static_cast<MoverController *>(controller);
	mover->MoveToBeginning();
	mover->AddObserver(this);
}

void MoveToBeginningFunction::HandleEvent(MoverController *mover, MoverController::ObservableEventType event)
{
	mover->RemoveObserver(this);
	CallCompletionProc();
}


MoveToEndingFunction::MoveToEndingFunction() :
		Function(kFunctionMoveToEnding, kControllerMover),
		Observer<MoveToEndingFunction, MoverController>(this, &MoveToEndingFunction::HandleEvent)
{
}

MoveToEndingFunction::MoveToEndingFunction(const MoveToEndingFunction& moveToEndingFunction) :
		Function(moveToEndingFunction),
		Observer<MoveToEndingFunction, MoverController>(this, &MoveToEndingFunction::HandleEvent)
{
}

MoveToEndingFunction::~MoveToEndingFunction()
{
}

Function *MoveToEndingFunction::Replicate(void) const
{
	return (new MoveToEndingFunction(*this));
}

bool MoveToEndingFunction::OverridesFunction(const Function *function) const
{
	FunctionType type = function->GetFunctionType();
	return ((type == kFunctionMoveToBeginning) || (type == kFunctionMoveToEnding) || (type == kFunctionStopMover));
}

void MoveToEndingFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	MoverController *mover = static_cast<MoverController *>(controller);
	mover->MoveToEnding();
	mover->AddObserver(this);
}

void MoveToEndingFunction::HandleEvent(MoverController *mover, MoverController::ObservableEventType event)
{
	mover->RemoveObserver(this);
	CallCompletionProc();
}


StopMoverFunction::StopMoverFunction() : Function(kFunctionStopMover, kControllerMover)
{
}

StopMoverFunction::StopMoverFunction(const StopMoverFunction& stopMoverFunction) : Function(stopMoverFunction)
{
}

StopMoverFunction::~StopMoverFunction()
{
}

Function *StopMoverFunction::Replicate(void) const
{
	return (new StopMoverFunction(*this));
}

bool StopMoverFunction::OverridesFunction(const Function *function) const
{
	FunctionType type = function->GetFunctionType();
	return ((type == kFunctionMoveToBeginning) || (type == kFunctionMoveToEnding) || (type == kFunctionStopMover));
}

void StopMoverFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	static_cast<MoverController *>(controller)->Stop();
	CallCompletionProc();
}

// ZYURVUR
