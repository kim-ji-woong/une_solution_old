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


#include "C4Scripts.h"
#include "C4World.h"


using namespace C4;


namespace C4
{
	template <> Heap Memory<ScriptState>::heap("Script", 65536, kHeapMutexless);
	template class Memory<ScriptState>;
}


ScriptObject::ScriptObject() : Object(kObjectScript)
{
}

ScriptObject::ScriptObject(const ScriptObject *object) : Object(kObjectScript)
{
	CloneScript(&object->scriptGraph, &scriptGraph);
	
	const Value *value = object->GetFirstValue();
	while (value)
	{
		valueMap.Insert(value->Clone());
		value = value->Next();
	}
}

ScriptObject::~ScriptObject()
{
}

void ScriptObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	PackHandle handle = data.BeginChunk('SCPT');
	PackScript(&scriptGraph, data, packFlags);
	data.EndChunk(handle);
	
	const Value *value = valueMap.First();
	while (value)
	{
		PackHandle handle = data.BeginChunk('VALU');
		value->PackType(data);
		value->Pack(data, packFlags);
		data.EndChunk(handle);
		
		value = value->Next();
	}
	
	data << TerminatorChunk;
}

void ScriptObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<ScriptObject>(data, unpackFlags);
}

bool ScriptObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'SCPT':
		{
			Method **methodTable = UnpackScript(&scriptGraph, data, unpackFlags);
			delete[] methodTable;
			return (true);
		}
		
		case 'VALU':
		{
			Value *value = Value::Construct(data);
			if (value)
			{
				value->Unpack(++data, unpackFlags);
				valueMap.Insert(value);
				return (true);
			}
			
			break;
		}
	}
	
	return (false);
}

void *ScriptObject::BeginSettingsUnpack(void)
{
	scriptGraph.Purge();
	valueMap.Purge();
	return (nullptr);
}

void ScriptObject::PackScript(const ScriptGraph *graph, Packer& data, unsigned_int32 packFlags)
{
	int32 methodCount = 0;
	int32 fiberCount = 0;
	
	const Method *method = graph->GetFirstElement();
	while (method)
	{ 
		if ((method->GetMethodType() != kMethodSection) || (packFlags & kPackEditor))
		{ 
			fiberCount += method->GetIncomingEdgeCount(); 
			method->methodIndex = methodCount; 
			methodCount++;
		} 
		
		method = method->GetNextElement();
	}
	 
	data << methodCount;
	data << fiberCount;
	
	method = graph->GetFirstElement(); 
	while (method)
	{
		if ((method->GetMethodType() != kMethodSection) || (packFlags & kPackEditor))
		{
			PackHandle handle = data.BeginSection();
			method->PackType(data);
			method->Pack(data, packFlags);
			data.EndSection(handle);
		}
		
		method = method->GetNextElement();
	}
	
	method = graph->GetFirstElement();
	while (method)
	{
		const Fiber *fiber = method->GetFirstIncomingEdge();
		while (fiber)
		{
			data << fiber->GetStartElement()->methodIndex;
			data << fiber->GetFinishElement()->methodIndex;
			
			PackHandle handle = data.BeginSection();
			fiber->Pack(data, packFlags);
			data.EndSection(handle);
			
			fiber = fiber->GetNextIncomingEdge();
		}
		
		method = method->GetNextElement();
	}
}

Method **ScriptObject::UnpackScript(ScriptGraph *graph, Unpacker& data, unsigned_int32 unpackFlags)
{
	int32		methodCount;
	int32		fiberCount;
	Method		**methodTable;
	
	data >> methodCount;
	data >> fiberCount;
	
	methodTable = new Method *[methodCount];
	
	for (machine a = 0; a < methodCount; a++)
	{
		unsigned_int32	size;
		
		data >> size;
		data.SetMark();
		
		if ((data.GetType() != kMethodSection) || (unpackFlags & kUnpackEditor))
		{
			Method *method = Method::Construct(data, unpackFlags);
			if (method)
			{
				method->Unpack(++data, unpackFlags);
				methodTable[a] = method;
				graph->AddElement(method);
				continue;
			}
		}
		
		data.Skip(size);
		methodTable[a] = nullptr;
	}
	
	for (machine a = 0; a < fiberCount; a++)
	{
		unsigned_int32	startIndex;
		unsigned_int32	finishIndex;
		unsigned_int32	size;
		
		data >> startIndex;
		data >> finishIndex;
		
		data >> size;
		data.SetMark();
		
		Method *startNode = methodTable[startIndex];
		Method *finishNode = methodTable[finishIndex];
		if ((startNode) && (finishNode))
		{
			Fiber *fiber = new Fiber(startNode, finishNode);
			fiber->Unpack(data, unpackFlags);
		}
		else
		{
			data.Skip(size);
		}
	}
	
	return (methodTable);
}

void ScriptObject::CloneScript(const ScriptGraph *sourceGraph, ScriptGraph *destinGraph)
{
	const Method *method = sourceGraph->GetFirstElement();
	while (method)
	{
		Method *clone = method->Clone();
		method->cloneMethod = clone;
		destinGraph->AddElement(clone);
		
		method = method->GetNextElement();
	}
	
	method = sourceGraph->GetFirstElement();
	while (method)
	{
		Method *clone = method->cloneMethod;
		
		const Fiber *fiber = method->GetFirstIncomingEdge();
		while (fiber)
		{
			new Fiber(*fiber, fiber->GetStartElement()->cloneMethod, clone);
			fiber = fiber->GetNextIncomingEdge();
		}
		
		method = method->GetNextElement();
	}
}


ScriptState::ScriptState(ScriptController *controller)
{
	scriptController = controller;
	scriptObject = nullptr;
}

ScriptState::ScriptState(ScriptController *controller, ScriptObject *object)
{
	scriptController = controller;
	scriptObject = object;
	object->Retain();
	
	scriptTime = 0.0F;
	
	ScriptObject::CloneScript(object->GetScriptGraph(), &scriptGraph);
	
	const Value *value = object->GetFirstValue();
	while (value)
	{
		if (value->GetValueScope() == kValueScopeScript) scriptValueMap.Insert(value->Clone());
		value = value->Next();
	}
}

ScriptState::~ScriptState()
{
	if (scriptObject) scriptObject->Release();
}

void ScriptState::Prepack(List<Object> *linkList) const
{
	if (scriptObject) linkList->Append(scriptObject);
}

void ScriptState::Pack(Packer& data, unsigned_int32 packFlags) const
{
	if (scriptObject)
	{
		data << ChunkHeader('OBJC', 4);
		data << scriptObject->GetObjectIndex();
	}
	
	data << ChunkHeader('TIME', 4);
	data << scriptTime;
	
	int32 loopCount = loopList.GetElementCount();
	int32 readyCount = readyList.GetElementCount();
	int32 executingCount = executingList.GetElementCount();
	int32 completeCount = completeList.GetElementCount();
	int32 totalCount = loopCount + readyCount + executingCount + completeCount;
	
	PackHandle handle = data.BeginChunk('SCPT');
	ScriptObject::PackScript(&scriptGraph, data, packFlags);
	
	data << loopCount;
	data << readyCount;
	data << executingCount;
	data << completeCount;
	
	int32 *indexTable = new int32[totalCount];
	int32 entry = 0;
	
	const Reference<Method> *reference = loopList.First();
	while (reference)
	{
		indexTable[entry++] = reference->GetTarget()->GetMethodIndex();
		reference = reference->Next();
	}
	
	reference = readyList.First();
	while (reference)
	{
		indexTable[entry++] = reference->GetTarget()->GetMethodIndex();
		reference = reference->Next();
	}
	
	reference = executingList.First();
	while (reference)
	{
		indexTable[entry++] = reference->GetTarget()->GetMethodIndex();
		reference = reference->Next();
	}
	
	reference = completeList.First();
	while (reference)
	{
		indexTable[entry++] = reference->GetTarget()->GetMethodIndex();
		reference = reference->Next();
	}
	
	data.WriteArray(totalCount, indexTable);
	data.EndChunk(handle);
	delete[] indexTable;
	
	const Node *trigger = triggerNodeLink;
	if ((trigger) && (trigger->LinkedNodePackable(packFlags)))
	{
		data << ChunkHeader('TRIG', 4);
		data << trigger->GetNodeIndex();
	}
	
	const Node *activator = activatorNodeLink;
	if ((activator) && (!(activator->GetNodeFlags() & kNodeNonpersistent)) && (activator->LinkedNodePackable(packFlags)))
	{
		data << ChunkHeader('ACTV', 4);
		data << activator->GetNodeIndex();
	}
	
	const Value *value = scriptValueMap.First();
	while (value)
	{
		PackHandle handle = data.BeginChunk('VALU');
		value->PackType(data);
		value->Pack(data, packFlags);
		data.EndChunk(handle);
		
		value = value->Next();
	}
	
	data << TerminatorChunk;
}

void ScriptState::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<ScriptState>(data, unpackFlags);
}

bool ScriptState::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'OBJC':
		{
			int32	index;
			
			data >> index;
			data.AddObjectLink(index, &ScriptObjectLinkProc, this);
			return (true);
		}
		
		case 'TIME':
			
			data >> scriptTime;
			return (true);
		
		case 'SCPT':
		{
			int32	loopCount;
			int32	readyCount;
			int32	executingCount;
			int32	completeCount;
			
			Method **methodTable = ScriptObject::UnpackScript(&scriptGraph, data, unpackFlags);
			
			data >> loopCount;
			data >> readyCount;
			data >> executingCount;
			data >> completeCount;
			
			int32 totalCount = loopCount + readyCount + executingCount + completeCount;
			int32 *indexTable = new int32[totalCount];
			data.ReadArray(totalCount, indexTable);
			
			int32 *index = indexTable;
			for (machine a = 0; a < loopCount; a++) loopList.Append(methodTable[*index++]->GetMethodReference());
			for (machine a = 0; a < readyCount; a++) readyList.Append(methodTable[*index++]->GetMethodReference());
			for (machine a = 0; a < executingCount; a++) executingList.Append(methodTable[*index++]->GetMethodReference());
			for (machine a = 0; a < completeCount; a++) completeList.Append(methodTable[*index++]->GetMethodReference());
			
			delete[] indexTable;
			delete[] methodTable;
			return (true);
		}
		
		case 'TRIG':
		{
			int32	nodeIndex;
			
			data >> nodeIndex;
			data.AddNodeLink(nodeIndex, &TriggerLinkProc, this);
			return (true);
		}
		
		case 'ACTV':
		{
			int32	nodeIndex;
			
			data >> nodeIndex;
			data.AddNodeLink(nodeIndex, &ActivatorLinkProc, this);
			return (true);
		}
		
		case 'VALU':
		{
			Value *value = Value::Construct(data);
			if (value)
			{
				value->Unpack(++data, unpackFlags);
				scriptValueMap.Insert(value);
				return (true);
			}
			
			break;
		}
	}
	
	return (false);
}

void *ScriptState::BeginSettingsUnpack(void)
{
	scriptGraph.Purge();
	scriptValueMap.Purge();
	return (nullptr);
}

void ScriptState::ScriptObjectLinkProc(Object *object, void *cookie)
{
	static_cast<ScriptState *>(cookie)->scriptObject = static_cast<ScriptObject *>(object);
	object->Retain();
}

void ScriptState::TriggerLinkProc(Node *node, void *cookie)
{
	ScriptState *state = static_cast<ScriptState *>(cookie);
	state->triggerNodeLink = node;
}

void ScriptState::ActivatorLinkProc(Node *node, void *cookie)
{
	ScriptState *state = static_cast<ScriptState *>(cookie);
	state->activatorNodeLink = node;
}

Value *ScriptState::GetValue(const char *name) const
{
	if (name[0] != 0)
	{
		Value *value = scriptValueMap.Find(name);
		if (value) return (value);
		
		return (scriptObject->GetValue(name));
	}
	
	return (nullptr);
}

void ScriptState::ExecuteMethod(Method *method, bool dead)
{
	if (!dead)
	{
		Fiber *fiber = method->GetFirstIncomingEdge();
		while (fiber)
		{
			fiber->SetFiberState(0);
			fiber = fiber->GetNextIncomingEdge();
		}
		
		method->SetCompletionProc(&MethodComplete, this);
		executingList.Append(method->GetMethodReference());
		
		method->SetMethodState(kMethodResult);
		method->ProcessInputValues(this);
		method->Execute(this);
	}
	else
	{
		method->SetMethodState(kMethodDead);
		completeList.Append(method->GetMethodReference());
	}
}

void ScriptState::MethodComplete(Method *method, void *cookie)
{
	ScriptState *state = static_cast<ScriptState *>(cookie);
	state->completeList.Append(method->GetMethodReference());
}

void ScriptState::Preprocess(void)
{
	Method *method = scriptGraph.GetFirstElement();
	while (method)
	{
		method->Preprocess(this);
		method = method->GetNextElement();
	}
}

void ScriptState::ExecuteScript(Node *trigger, Node *activator)
{
	triggerNodeLink = trigger;
	activatorNodeLink = activator;
	
	Preprocess();
	StartScript();
}

void ScriptState::StartScript(void)
{
	Method *method = scriptGraph.GetFirstElement();
	while (method)
	{
		if (!method->GetFirstIncomingEdge()) ExecuteMethod(method);
		method = method->GetNextElement();
	}
}

void ScriptState::ResumeScript(void)
{
	Preprocess();
	
	Reference<Method> *reference = executingList.First();
	while (reference)
	{
		Reference<Method> *next = reference->Next();
		
		Method *method = reference->GetTarget();
		method->SetCompletionProc(&MethodComplete, this);
		method->Resume(this);
		
		reference = next;
	}
}

void ScriptState::ScriptTask(void)
{
	Reference<Method> *reference = loopList.Last();
	while (reference)
	{
		Reference<Method> *previous = reference->Previous();
		readyList.Prepend(reference);
		reference = previous;
	}
	
	for (;;)
	{
		Reference<Method> *reference = readyList.First();
		while (reference)
		{
			Reference<Method> *next = reference->Next();
			
			bool exec = true;
			bool dead = true;
			
			Method *method = reference->GetTarget();
			const Fiber *fiber = method->GetFirstIncomingEdge();
			while (fiber)
			{
				bool looping = ((fiber->GetFiberFlags() & kFiberLooping) != 0);
				unsigned_int32 state = fiber->GetFiberState();
				
				if (state & kFiberReady)
				{
					if (looping)
					{
						exec = true;
						dead = false;
						break;
					}
					else
					{
						if (!(state & kFiberDead)) dead = false;
					}
				}
				else
				{
					if (!looping) exec = false;
				}
				
				fiber = fiber->GetNextIncomingEdge();
			}
			
			if (exec) ExecuteMethod(method, dead);
			
			reference = next;
		}
		
		reference = completeList.First();
		if (!reference) break;
		
		do
		{
			Reference<Method> *next = reference->Next();
			
			completeList.Remove(reference);
			
			Method *method = reference->GetTarget();
			unsigned_int32 methodState = method->GetMethodState();
			
			if (!(methodState & kMethodDead))
			{
				bool result = ((methodState & kMethodResult) != 0);
				bool loop = false;
				
				// Examine all looping fibers first to determine if a loop is followed.
				
				Fiber *fiber = method->GetFirstOutgoingEdge();
				while (fiber)
				{
					unsigned_int32 fiberFlags = fiber->GetFiberFlags();
					if (fiberFlags & kFiberLooping)
					{
						Reference<Method> *successor = fiber->GetFinishElement()->GetMethodReference();
						if ((!successor->GetOwningList()) || (readyList.Member(successor)) || (loopList.Member(successor)))
						{
							if ((fiberFlags & (kFiberConditionTrue | kFiberConditionFalse)) != 0)
							{
								bool dead = (result) ? !(fiberFlags & kFiberConditionTrue) : !(fiberFlags & kFiberConditionFalse);
								if (!dead)
								{
									loop = true;
									fiber->SetFiberState(kFiberReady);
									if (!successor->GetOwningList()) loopList.Append(successor);
								}
							}
							else
							{
								loop = true;
								fiber->SetFiberState(kFiberReady);
								if (!successor->GetOwningList()) loopList.Append(successor);
							}
						}
					}
					
					fiber = fiber->GetNextOutgoingEdge();
				}
				
				// Now loop through all non-looping fibers, but don't follow conditional fibers
				// if the condition is not satisfied and any loop was followed.
				
				fiber = method->GetFirstOutgoingEdge();
				while (fiber)
				{
					unsigned_int32 fiberFlags = fiber->GetFiberFlags();
					if (!(fiberFlags & kFiberLooping))
					{
						Reference<Method> *successor = fiber->GetFinishElement()->GetMethodReference();
						if ((!successor->GetOwningList()) || (readyList.Member(successor)) || (loopList.Member(successor)))
						{
							if ((fiberFlags & (kFiberConditionTrue | kFiberConditionFalse)) != 0)
							{
								bool dead = (result) ? !(fiberFlags & kFiberConditionTrue) : !(fiberFlags & kFiberConditionFalse);
								if (!dead)
								{
									fiber->SetFiberState(kFiberReady);
									if (!successor->GetOwningList()) readyList.Append(successor);
								}
								else if (!loop)
								{
									fiber->SetFiberState(kFiberReady | kFiberDead);
									if (!successor->GetOwningList()) readyList.Append(successor);
								}
							}
							else
							{
								fiber->SetFiberState(kFiberReady);
								if (!successor->GetOwningList()) readyList.Append(successor);
							}
						}
					}
					
					fiber = fiber->GetNextOutgoingEdge();
				}
			}
			else
			{
				Fiber *fiber = method->GetFirstOutgoingEdge();
				while (fiber)
				{
					if (!(fiber->GetFiberFlags() & kFiberLooping))
					{
						Reference<Method> *successor = fiber->GetFinishElement()->GetMethodReference();
						if ((!successor->GetOwningList()) || (readyList.Member(successor)) || (loopList.Member(successor)))
						{
							fiber->SetFiberState(kFiberReady | kFiberDead);
							if (!successor->GetOwningList()) readyList.Append(successor);
						}
					}
					
					fiber = fiber->GetNextOutgoingEdge();
				}
			}
			
			reference = next;
		} while (reference);
	}

	scriptTime += TheTimeMgr->GetFloatDeltaTime();
	
	if ((executingList.Empty()) && (readyList.Empty()) && (loopList.Empty()))
	{
		if (scriptController->GetScriptFlags() & kScriptContinuousExecute) StartScript();
		else delete this;
	}
}


ScriptController::ScriptController() :
		Controller(kControllerScript),
		initialExecuteTask(&InitialExecuteTask, this),
		initialResumeTask(&InitialResumeTask, this)
{
	scriptFlags = 0;
	scriptObject = nullptr;
}

ScriptController::ScriptController(ControllerType type) :
		Controller(type),
		initialExecuteTask(&InitialExecuteTask, this),
		initialResumeTask(&InitialResumeTask, this)
{
	scriptFlags = 0;
	scriptObject = nullptr;
}

ScriptController::ScriptController(const ScriptController& scriptController) :
		Controller(scriptController),
		initialExecuteTask(&InitialExecuteTask, this),
		initialResumeTask(&InitialResumeTask, this)
{
	scriptFlags = scriptController.scriptFlags;
	scriptObject = scriptController.scriptObject;
	if (scriptObject) scriptObject->Retain();
}

ScriptController::~ScriptController()
{
	if (scriptObject) scriptObject->Release();
}

Controller *ScriptController::Replicate(void) const
{
	return (new ScriptController(*this));
}

void ScriptController::Prepack(List<Object> *linkList) const
{
	Controller::Prepack(linkList);
	
	if (scriptObject) linkList->Append(scriptObject);
	
	const ScriptState *state = executeList.First();
	while (state)
	{
		state->Prepack(linkList);
		state = state->Next();
	}
}

void ScriptController::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Controller::Pack(data, packFlags);
	
	data << ChunkHeader('FLAG', 4);
	data << scriptFlags;
	
	if (!(packFlags & kPackSettings))
	{
		if (scriptObject)
		{
			data << ChunkHeader('OBJC', 4);
			data << scriptObject->GetObjectIndex();
		}
		
		const ScriptState *state = executeList.First();
		while (state)
		{
			PackHandle handle = data.BeginChunk('EXEC');
			state->Pack(data, packFlags);
			data.EndChunk(handle);
			
			state = state->Next();
		}
	}
	
	data << TerminatorChunk;
}

void ScriptController::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Controller::Unpack(data, unpackFlags);
	UnpackChunkList<ScriptController>(data, unpackFlags);
}
			
bool ScriptController::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'FLAG':
		{
			data >> scriptFlags;
			return (true);
		}
		
		case 'OBJC':
		{
			int32	objectIndex;
			
			data >> objectIndex;
			data.AddObjectLink(objectIndex, &ScriptObjectLinkProc, this);
			return (true);
		}
		
		case 'EXEC':
		{
			ScriptState *state = new ScriptState(this);
			state->Unpack(data, unpackFlags);
			resumeList.Append(state);
			return (true);
		}
	}
	
	return (false);
}

void ScriptController::ScriptObjectLinkProc(Object *object, void *cookie)
{
	ScriptController *scriptController = static_cast<ScriptController *>(cookie);
	scriptController->scriptObject = static_cast<ScriptObject *>(object);
	object->Retain();
}

int32 ScriptController::GetSettingCount(void) const
{
	return (4);
}

Setting *ScriptController::GetSetting(int32 index) const
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerScript, 'INIT'));
		return (new BooleanSetting('INIT', ((scriptFlags & kScriptInitialExecute) != 0), title));
	}
	
	if (index == 1)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerScript, 'CONT'));
		return (new BooleanSetting('CONT', ((scriptFlags & kScriptContinuousExecute) != 0), title));
	}
	
	if (index == 2)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerScript, 'RENT'));
		return (new BooleanSetting('RENT', ((scriptFlags & kScriptReentrant) != 0), title));
	}
	
	if (index == 3)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerScript, 'UNIQ'));
		return (new BooleanSetting('UNIQ', ((scriptFlags & kScriptUniqueActivators) != 0), title));
	}
	
	return (nullptr);
}

void ScriptController::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'INIT')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) scriptFlags |= kScriptInitialExecute;
		else scriptFlags &= ~kScriptInitialExecute;
	}
	else if (identifier == 'CONT')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) scriptFlags |= kScriptContinuousExecute;
		else scriptFlags &= ~kScriptContinuousExecute;
	}
	else if (identifier == 'RENT')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) scriptFlags |= kScriptReentrant;
		else scriptFlags &= ~kScriptReentrant;
	}
	else if (identifier == 'UNIQ')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) scriptFlags |= kScriptUniqueActivators;
		else scriptFlags &= ~kScriptUniqueActivators;
	}
}

void ScriptController::SetScriptObject(ScriptObject *object)
{
	if (scriptObject != object)
	{
		if (scriptObject) scriptObject->Release();
		if (object) object->Retain();
		scriptObject = object;
	}
}

void ScriptController::ExecuteScript(ScriptObject *object, Node *trigger, Node *activator)
{
	unsigned_int32 flags = scriptFlags;
	if ((flags & kScriptReentrant) || (executeList.Empty()))
	{
		if (flags & kScriptUniqueActivators)
		{
			const ScriptState *state = executeList.First();
			while (state)
			{
				if (state->GetActivatorNode() == activator) return;
				state = state->Next();
			}
		}
		
		Wake();
		
		ScriptState *state = new ScriptState(this, object);
		executeList.Append(state);
		
		state->ExecuteScript(trigger, activator);
	}
}

void ScriptController::InitialExecuteTask(DeferredTask *task, void *cookie)
{
	ScriptController *scriptController = static_cast<ScriptController *>(cookie);
	ScriptObject *object = scriptController->scriptObject;
	if (object)
	{
		scriptController->scriptFlags &= ~kScriptInitialExecute;
		scriptController->ExecuteScript(object);
	}
}

void ScriptController::InitialResumeTask(DeferredTask *task, void *cookie)
{
	ScriptController *scriptController = static_cast<ScriptController *>(cookie);
	scriptController->Wake();
	
	ScriptState *state = scriptController->resumeList.First();
	while (state)
	{
		ScriptState *next = state->Next();
		
		scriptController->executeList.Append(state);
		state->ResumeScript();
		
		state = next;
	}
}

void ScriptController::Preprocess(void)
{
	if (GetControllerType() == kControllerScript) SetControllerFlags(kControllerAsleep);
	
	Controller::Preprocess();
	
	if ((!GetTargetNode()->GetManipulator()) && (TheMessageMgr->Server()))
	{
		if (scriptFlags & kScriptInitialExecute)
		{
			if (scriptObject) TheTimeMgr->AddTask(&initialExecuteTask);
		}
		else if (!resumeList.Empty())
		{
			TheTimeMgr->AddTask(&initialResumeTask);
		}
	}
}

void ScriptController::Move(void)
{
	ScriptState *state = executeList.First();
	if (state)
	{
		do
		{
			ScriptState *next = state->Next();
			
			GetTargetNode()->GetWorld()->IncrementWorldCounter(kWorldCounterRunningScript);
			state->ScriptTask();
			
			state = next;
		} while (state);
	}
	else
	{
		if (GetControllerType() == kControllerScript) Sleep();
	}
}

void ScriptController::Activate(Node *trigger, Node *activator)
{
	ScriptObject *object = scriptObject;
	if ((object) && (TheMessageMgr->Server())) ExecuteScript(object, trigger, activator);
}

void ScriptController::HandleInteractionEvent(InteractionEventType type, const Point3D *position, Node *activator)
{
	if (type == kInteractionEventActivate) Activate(nullptr, activator);
}

// ZYURVUR
