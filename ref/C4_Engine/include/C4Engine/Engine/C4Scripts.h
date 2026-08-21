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


#ifndef C4Scripts_h
#define C4Scripts_h


//# \component	Controller System
//# \prefix		Controller/


#include "C4Methods.h"


namespace C4
{
	//# \enum	ScriptFlags
	
	enum
	{
		kScriptInitialExecute		= 1 << 0,		//## Script executes immediately upon world load.
		kScriptContinuousExecute	= 1 << 1,		//## Script always restarts when it finishes running.
		kScriptReentrant			= 1 << 2,		//## Multiple instances of the script can run simultaneously.
		kScriptUniqueActivators		= 1 << 3		//## Each instance of a running script must have a unique activator.
	};
	
	
	enum
	{
		kObjectScript		= 'SCPT'
	};
	
	
	enum
	{
		kControllerScript	= 'SCPT'
	};
	
	
	class ScriptController;
	
	
	class ScriptObject : public Object
	{
		private:
			
			ScriptGraph		scriptGraph;
			Map<Value>		valueMap;
			
			~ScriptObject();
		
		public:
			
			C4API ScriptObject();
			C4API ScriptObject(const ScriptObject *object);
			
			ScriptGraph *GetScriptGraph(void)
			{
				return (&scriptGraph);
			}
			
			const ScriptGraph *GetScriptGraph(void) const
			{
				return (&scriptGraph);
			}
			
			void SetScriptGraph(const ScriptGraph *graph)
			{
				scriptGraph.Purge();
				CloneScript(graph, &scriptGraph);
			}
			
			Value *GetFirstValue(void) const
			{
				return (valueMap.First());
			}
			
			bool AddValue(Value *value)
			{
				return (valueMap.Insert(value));
			}
			
			Value *GetValue(const char *name) const
			{
				return (valueMap.Find(name));
			}
			
			void PurgeValues(void)
			{
				valueMap.Purge();
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			static void PackScript(const ScriptGraph *graph, Packer& data, unsigned_int32 packFlags);
			static Method **UnpackScript(ScriptGraph *graph, Unpacker& data, unsigned_int32 unpackFlags);
			
			C4API static void CloneScript(const ScriptGraph *sourceGraph, ScriptGraph *destinGraph);
	};
	
	
	//# \class	ScriptState		Represents the state of a running script. 
	//
	//# The $ScriptState$ class represents the state of a running script. 
	// 
	//# \def	class ScriptState : public ListElement<ScriptState>, public Packable, public Memory<ScriptState> 
	//
	//# \ctor	ScriptState(); 
	//
	//# \desc
	//# The $ScriptState$ class represents the current state of a running script. A $ScriptState$ object is passed
	//# to the $@Method::Execute@$ and $@Method::Resume@$ functions so that the implementations of those functions 
	//# can pass them to other functions that require the script state.
	//
	//# \base	Utilities/ListElement<ScriptState>		Used internally by a script.
	//# \base	ResourceMgr/Packable					Script state can be packed for storage in resources. 
	//# \base	MemoryMgr/Memory<ScriptState>			Script state objects are stored in a dedicated heap.
	//
	//# \also	$@Method::Execute@$
	//# \also	$@Method::Resume@$
	
	
	//# \function	ScriptState::GetTriggerNode		Returns the trigger node for a script.
	//
	//# \proto	Node *GetTriggerNode(void) const;
	//
	//# \desc
	//# The $GetTriggerNode$ function returns the trigger node that was activated, causing the particular script instance
	//# represented by a $ScriptState$ object to run. If the script was not started by a trigger node being activated, then there
	//# is no trigger node, and the return value is $nullptr$.
	//#
	//# Note that if a particular method targets the trigger node, then it will be returned by the $@Method::GetTargetNode@$ function.
	//
	//# \also	$@ScriptState::GetActivatorNode@$
	//# \also	$@Method::GetTargetNode@$
	
	
	//# \function	ScriptState::GetActivatorNode		Returns the activator node for a script.
	//
	//# \proto	Node *GetActivatorNode(void) const;
	//
	//# \desc
	//# The $GetActivatorNode$ function returns the node that activated the trigger node, causing the particular script instance
	//# represented by a $ScriptState$ object to run. If the script was not started by a trigger node being activated, then there
	//# is no activator node, and the return value is $nullptr$.
	//#
	//# Note that if a particular method targets the trigger node, then it will be returned by the $@Method::GetTargetNode@$ function.
	//
	//# \also	$@ScriptState::GetTriggerNode@$
	//# \also	$@Method::GetTargetNode@$
	
	
	//# \function	ScriptState::GetScriptTime		Returns the time elapsed since a script began running.
	//
	//# \proto	float GetScriptTime(void) const;
	//
	//# \desc
	//# The $GetScriptTime$ function returns the time elapsed, as a floating-point value measured in seconds, since a script
	//# began running. The script time is updated once per frame, so all methods executed during the same frame observe the
	//# same script time. During the first frame in which a script is running, the script time is guaranteed to be exactly 0.0.
	
	
	//# \function	ScriptState::GetValue		Returns a script variable.
	//
	//# \proto	Value *GetValue(const char *name) const;
	//
	//# \param	name	The name of the value to retrieve.
	//
	//# \desc
	//# The $GetValue$ function returns the script variable specified by the $name$ parameter. If no variable by that name exists,
	//# then the return value is $nullptr$.
	//
	//# \also	$@Value@$
	
	
	class ScriptState : public ListElement<ScriptState>, public Packable, public Memory<ScriptState>
	{
		private:
			
			ScriptController			*scriptController;
			ScriptObject				*scriptObject;
			
			Link<Node>					triggerNodeLink;
			Link<Node>					activatorNodeLink;

			float						scriptTime;
			
			List<Reference<Method> >	loopList;
			List<Reference<Method> >	readyList;
			List<Reference<Method> >	executingList;
			List<Reference<Method> >	completeList;
			
			ScriptGraph					scriptGraph;
			Map<Value>					scriptValueMap;
			
			static void ScriptObjectLinkProc(Object *object, void *cookie);
			static void TriggerLinkProc(Node *node, void *cookie);
			static void ActivatorLinkProc(Node *node, void *cookie);
			
			void ExecuteMethod(Method *method, bool dead = false);
			static void MethodComplete(Method *method, void *cookie);
			
			void StartScript(void);
		
		public:
			
			ScriptState(ScriptController *controller);
			ScriptState(ScriptController *controller, ScriptObject *object);
			~ScriptState();
			
			const ScriptController *GetScriptController(void) const
			{
				return (scriptController);
			}
			
			Node *GetTriggerNode(void) const
			{
				return (triggerNodeLink);
			}
			
			Node *GetActivatorNode(void) const
			{
				return (activatorNodeLink);
			}

			float GetScriptTime(void) const
			{
				return (scriptTime);
			}
			
			void Prepack(List<Object> *linkList) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			Node *GetScriptControllerTarget(void) const;
			
			C4API Value *GetValue(const char *name) const;
			
			void Preprocess(void);
			
			void ExecuteScript(Node *trigger, Node *activator);
			void ResumeScript(void);
			
			void ScriptTask(void);
	};
	
	
	//# \class	ScriptController		Manages a scripted node in a world.
	//
	//# The $ScriptController$ class manages a scripted node in a world.
	//
	//# \def	class ScriptController : public Controller
	//
	//# \ctor	ScriptController();
	//
	//# \desc
	//# The $ScriptController$ class manages a scripted node in a world.
	//
	//# \base	Controller		The $ScriptController$ class is a specialized controller.
	//
	//# \also	$@Method@$
	//# \also	$@ScriptState@$
	
	
	class ScriptController : public Controller
	{
		private:
			
			unsigned_int32		scriptFlags;
			ScriptObject		*scriptObject;
			
			List<ScriptState>	executeList;
			List<ScriptState>	resumeList;
			
			DeferredTask		initialExecuteTask;
			DeferredTask		initialResumeTask;
			
			Controller *Replicate(void) const override;
			
			static void ScriptObjectLinkProc(Object *object, void *cookie);
			
			static void InitialExecuteTask(DeferredTask *event, void *cookie);
			static void InitialResumeTask(DeferredTask *event, void *cookie);
		
		protected:
			
			C4API ScriptController(ControllerType type);
			C4API ScriptController(const ScriptController& scriptController);
		
		public:
			
			C4API ScriptController();
			C4API ~ScriptController();
			
			unsigned_int32 GetScriptFlags(void) const
			{
				return (scriptFlags);
			}
			
			void SetScriptFlags(unsigned_int32 flags)
			{
				scriptFlags = flags;
			}
			
			ScriptObject *GetScriptObject(void) const
			{
				return (scriptObject);
			}
			
			void Terminate(void)
			{
				executeList.Purge();
			}
			
			C4API void Prepack(List<Object> *linkList) const;
			C4API void Pack(Packer& data, unsigned_int32 packFlags) const;
			C4API void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			C4API int32 GetSettingCount(void) const;
			C4API Setting *GetSetting(int32 index) const;
			C4API void SetSetting(const Setting *setting);
			
			C4API void SetScriptObject(ScriptObject *object);
			C4API void ExecuteScript(ScriptObject *object, Node *trigger = nullptr, Node *activator = nullptr);
			
			C4API void Preprocess(void);
			C4API void Move(void);
			
			C4API void Activate(Node *trigger, Node *activator);
			
			C4API void HandleInteractionEvent(InteractionEventType type, const Point3D *position, Node *activator);
	};
	
	
	inline Node *ScriptState::GetScriptControllerTarget(void) const
	{
		return (scriptController->GetTargetNode());
	}
}


#endif

// ZYURVUR
