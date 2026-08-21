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


#ifndef C4Controller_h
#define C4Controller_h


//# \component	Controller System
//# \prefix		Controller/


#include "C4Construction.h"
#include "C4Configurable.h"
#include "C4Attributes.h"
#include "C4Messages.h"
#include "C4Time.h"


namespace C4
{
	//# \tree	Controller
	//
	//# \node	PhysicsMgr/RigidBodyController
	//# \sub
	//#		\node	PhysicsMgr/CharacterController
	//# \end
	//# \node	PhysicsMgr/PhysicsController
	//# \node	ScriptController
	//# \sub
	//#		\node	EffectMgr/PanelController
	//# \end
	//# \node	WorldMgr/SkinController
	//# \node	WorldMgr/AnimationController
	//#	\node	PhysicsMgr/ClothController
	//#	\node	PhysicsMgr/WaterController
	//#	\node	ExtrasPlugin/RotationController
	//# \node	ExtrasPlugin/FlashController
	//#	\node	MoviesPlugin/MovieController
	
	
	typedef Type	FunctionType;
	typedef Type	ControllerType;
	
	
	enum
	{
		kControllerUnassigned	= -1
	};
	
	
	enum
	{
		kControllerGeneric		= 0
	};
	
	
	//# \enum	FunctionFlags
	
	enum
	{
		kFunctionRemote			= 1 << 0,		//## The function executes on remote machines instead of only on the server.
		kFunctionJournaled		= 1 << 1,		//## The message used to execute a remote function should be journaled.
		kFunctionOutputValue	= 1 << 2		//## The function generates an output value that can be stored in a script variable.
	};
	
	
	//# \enum	ControllerFlags
	
	enum
	{
		kControllerUpdate		= 1 << 0,		//## The controller needs to be updated (read-only flag).
		kControllerAsleep		= 1 << 1,		//## The controller is asleep, and thus its $Move$ function is not called (read-only flag).
		kControllerLocal		= 1 << 2,		//## The controller operates autonomously and does receive messages from remote machines.
		kControllerMoveInhibit	= 1 << 3		//## The controller's $Move$ function is never called, even if the controller is awake.
	};
	
	
	//# \enum	InteractionEventType
	
	enum InteractionEventType
	{
		kInteractionEventEngage,				//## The user has begun looking at an interactive object.
		kInteractionEventDisengage,				//## The user has stopped looking at an interactive object.
		kInteractionEventActivate,				//## The user has explicitly activated an interactive object (e.g., with a mouse click).
		kInteractionEventDeactivate,			//## The user has released the input control used to activate an interactive object.
		kInteractionEventTrack					//## The user has changed the interaction position.
	};
	
	
	class Function;
	class FunctionMethod;
	class ScriptState;
	class Controller;
	class ControllerRegistration;
	class Zone;
	
	
	//# \class	FunctionRegistration	Contains information about an application-defined controller function.
	//
	//# The $FunctionRegistration$ class contains information about an application-defined controller function.
	//
	//# \def	class FunctionRegistration : public MapElement<FunctionRegistration>
	//
	//# \ctor	FunctionRegistration(ControllerRegistration *reg, FunctionType type, const char *name, unsigned_int32 flags = 0);
	// 
	//# \param	reg		A pointer to the registration object for the controller to which the function pertains.
	//# \param	type	The function type. 
	//# \param	name	The function name. 
	//# \param	flags	The function flags. 
	//
	//# \desc 
	//# The $FunctionRegistration$ class is abstract and serves as the common base class for the template class
	//# $@FunctionReg@$. A custom function is registered with the engine by instantiating an object of type
	//# $FunctionReg<classType>$, where $classType$ is the type of the function subclass being registered.
	//#  
	//# The $flags$ parameter can be a combination (through logical OR) of the following values.
	//
	//# \table	FunctionFlags
	// 
	//# \base	Utilities/MapElement<FunctionRegistration>	Registration objects are stored in a map container by the engine.
	//
	//# \also	$@FunctionReg@$
	//# \also	$@Function@$
	//# \also	$@Controller@$
	//# \also	$@ControllerRegistration@$
	
	
	//# \function	FunctionRegistration::GetFunctionType		Returns the registered function type.
	//
	//# \proto	FunctionType GetFunctionType(void) const;
	//
	//# \desc
	//# The $GetKey$ function returns the function type for a particular function registration.
	//# The function type is established when the function registration is constructed.
	//
	//# \also	$@FunctionRegistration::GetFunctionName@$
	
	
	//# \function	FunctionRegistration::GetFunctionName		Returns the human-readable function name.
	//
	//# \proto	const char *GetFunctionName(void) const;
	//
	//# \desc
	//# The $GetFunctionName$ function returns the human-readable function name for a particular function registration.
	//# The function name is established when the function registration is constructed.
	//
	//# \also	$@FunctionRegistration::GetFunctionType@$
	
	
	class C4_API FunctionRegistration : public MapElement<FunctionRegistration>
	{
		private:
			
			FunctionType	functionType;
			unsigned_int32	functionFlags;
			const char		*functionName;
		
		protected:
			
			FunctionRegistration(ControllerRegistration *reg, FunctionType type, const char *name, unsigned_int32 flags = 0);
		
		public:
			
			typedef FunctionType KeyType;
			
			~FunctionRegistration();
			
			KeyType GetKey(void) const
			{
				return (functionType);
			}
			
			FunctionType GetFunctionType(void) const
			{
				return (functionType);
			}
			
			unsigned_int32 GetFunctionFlags(void) const
			{
				return (functionFlags);
			}
			
			const char *GetFunctionName(void) const
			{
				return (functionName);
			}
			
			virtual Function *Construct(void) const = 0;
	};
	
	
	//# \class	FunctionReg		Represents a custom function type.
	//
	//# The $FunctionReg$ class represents a custom function type.
	//
	//# \def	template <class classType> class FunctionReg : public FunctionRegistration
	//
	//# \tparam	classType	The custom function class.
	//
	//# \ctor	FunctionReg(ControllerRegistration *reg, FunctionType type, const char *name, unsigned_int32 flags = 0);
	//
	//# \param	reg		A pointer to the registration object for the controller to which the function pertains.
	//# \param	type	The function type.
	//# \param	name	The function name.
	//# \param	flags	The function flags.
	//
	//# \desc
	//# The $FunctionReg$ template class is used to advertise the existence of a custom function type for a particular.
	//# type of controller. The World Manager uses a function registration to construct a custom function, and the
	//# Script Editor displays a list of register functions for the type of controller that each method operates on.
	//# The act of instantiating a $FunctionReg$ object automatically registers the corresponding function
	//# type. The function type is unregistered when the $FunctionReg$ object is destroyed.
	//# 
	//# The $flags$ parameter can be a combination (through logical OR) of the following values.
	//
	//# \table	FunctionFlags
	//
	//# No more than one function registration should be created for each distinct function type.
	//
	//# \base	FunctionRegistration	All specific function registration classes share the common base class $FunctionRegistration$.
	//
	//# \also	$@Function@$
	//# \also	$@Controller@$
	
	
	template <class classType> class FunctionReg : public FunctionRegistration
	{
		public:
			
			FunctionReg(ControllerRegistration *reg, FunctionType type, const char *name, unsigned_int32 flags = 0) : FunctionRegistration(reg, type, name, flags)
			{
			}
			
			Function *Construct(void) const
			{
				return (new classType);
			}
	};
	
	
	//# \class	Function	The base class for all controller function objects.
	//
	//# Every function object that is exposed by a controller is a subclass of the $Function$ class.
	//
	//# \def	class Function : public Completable<Function>, public Packable, public Configurable
	//
	//# \ctor	Function(FunctionType funcType, ControllerType contType);
	//
	//# \param	funcType	The function type.
	//# \param	contType	The controller type to which the function applies.
	//
	//# \desc
	//# The $Function$ class is the base class for all controller function objects. A $Function$ subclass represents
	//# a specific function, associated with a particular type of controller, that can be called from a script.
	//
	//# \base	Utilities/Completable<Property>			A function calls its completion procedure when it has finished.
	//# \base	ResourceMgr/Packable					Function objects can be packed for storage in resources.
	//# \base	InterfaceMgr/Configurable				Function objects can define configurable parameters that are exposed
	//#													as user interface widgets in the Script Editor.
	//
	//# \also	$@FunctionReg@$
	//# \also	$@Controller@$
	
	
	//# \function	Function::GetFunctionType		Returns the function type.
	//
	//# \proto	FunctionType GetFunctionType(void) const;
	//
	//# \desc
	//# The $GetFunctionType$ function returns the function type.
	//
	//# \also	$@FunctionReg@$
	
	
	class C4_API Function : public Completable<Function>, public Packable, public Configurable
	{
		private:
			
			FunctionType		functionType;
			ControllerType		controllerType;
			
			virtual Function *Replicate(void) const = 0;
		
		protected:
			
			Function(FunctionType funcType, ControllerType contType);
			Function(const Function& function);
		
		public:
			
			~Function();
			
			FunctionType GetFunctionType(void) const
			{
				return (functionType);
			}
			
			ControllerType GetControllerType(void) const
			{
				return (controllerType);
			}
			
			Function *Clone(void) const
			{
				return (Replicate());
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			virtual void Compress(Compressor& data) const;
			virtual bool Decompress(Decompressor& data);
			
			virtual bool OverridesFunction(const Function *function) const;
			
			virtual void Preprocess(Controller *controller, FunctionMethod *method, const ScriptState *state);
			virtual void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
			virtual void Resume(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	//# \class	ControllerRegistration		Manages internal registration information for a custom controller type.
	//
	//# The $ControllerRegistration$ class manages internal registration information for a custom controller type.
	//
	//# \def	class ControllerRegistration : public Registration<Controller, ControllerRegistration>
	//
	//# \ctor	ControllerRegistration(ControllerType type, const char *name);
	//
	//# \param	type	The controller type.
	//# \param	name	The controller name.
	//
	//# \desc
	//# The $ControllerRegistration$ class is abstract and serves as the common base class for the template class
	//# $@ControllerReg@$. A custom controller is registered with the engine by instantiating an object of type
	//# $ControllerReg<classType>$, where $classType$ is the type of the controller subclass being registered.
	//
	//# \base	System/Registration<Controller, ControllerRegistration>		A controller registration is a specific type of registration object.
	//
	//# \also	$@ControllerReg@$
	//# \also	$@Controller@$
	
	
	//# \function	ControllerRegistration::GetControllerType		Returns the registered controller type.
	//
	//# \proto	ControllerType GetControllerType(void) const;
	//
	//# \desc
	//# The $GetControllerType$ function returns the controller type for a particular controller registration.
	//# The controller type is established when the controller registration is constructed.
	//
	//# \also	$@ControllerRegistration::GetControllerName@$
	
	
	//# \function	ControllerRegistration::GetControllerName		Returns the human-readable controller name.
	//
	//# \proto	const char *GetControllerName(void) const;
	//
	//# \desc
	//# The $GetControllerName$ function returns the human-readable controller name for a particular controller registration.
	//# The controller name is established when the controller registration is constructed.
	//
	//# \also	$@ControllerRegistration::GetControllerType@$
	
	
	//# \function	ControllerRegistration::GetFirstFunctionRegistration	Returns the first function registration.
	//
	//# \proto	FunctionRegistration *GetFirstFunctionRegistration(void);
	//
	//# \desc
	//# The $GetFirstFunctionRegistration$ function returns a pointer to the registration object corresponding
	//# to the first function registration for the controller type represented by the controller registration object.
	//# The entire list of registrations can be iterated by calling the $@Utilities/MapElement::Next@$ function on
	//# the returned object and continuing until $nullptr$ is returned.
	//
	//# \also	$@ControllerRegistration::FindFunctionRegistration@$
	
	
	//# \function	ControllerRegistration::FindFunctionRegistration		Returns a specific function registration.
	//
	//# \proto	FunctionRegistration *FindFunctionRegistration(Type type);
	//
	//# \param	type	The type of the subclass.
	//
	//# \desc
	//# The $FindFunctionRegistration$ function returns a pointer to the registration object corresponding
	//# to the function type specified by the $type$ parameter for the controller type represented by the
	//# controller registration object. If no such registration exists, then the return value is $nullptr$.
	//
	//# \also	$@ControllerRegistration::GetFirstFunctionRegistration@$
	
	
	class ControllerRegistration : public Registration<Controller, ControllerRegistration>
	{
		friend class FunctionRegistration;
		
		private:
			
			const char					*controllerName;
			
			Map<FunctionRegistration>	functionMap;
		
		protected:
			
			C4API ControllerRegistration(ControllerType type, const char *name);
		
		public:
			
			C4API ~ControllerRegistration();
			
			ControllerType GetControllerType(void) const
			{
				return (GetRegistrableType());
			}
			
			const char *GetControllerName(void) const
			{
				return (controllerName);
			}
			
			FunctionRegistration *GetFirstFunctionRegistration(void) const
			{
				return (functionMap.First());
			}
			
			FunctionRegistration *FindFunctionRegistration(FunctionType type) const
			{
				return (functionMap.Find(type));
			}
			
			FunctionRegistration *GetFunctionRegistration(int32 index) const
			{
				return (functionMap[index]);
			}
			
			int32 GetFunctionRegistrationCount(void) const
			{
				return (functionMap.GetElementCount());
			}
			
			C4API Function *ConstructFunction(FunctionType type) const;
			
			virtual bool ValidNode(const Node *node) const = 0;
	};
	
	
	//# \class	ControllerReg	 Represents a custom controller type.
	//
	//# The $ControllerReg$ class represents a custom controller type.
	//
	//# \def	template <class classType> class ControllerReg : public ControllerRegistration
	//
	//# \tparam	classType	The custom controller class.
	//
	//# \ctor	ControllerReg(ControllerType type, const char *name);
	//
	//# \param	type	The controller type.
	//# \param	name	The controller name.
	//
	//# \desc
	//# The $ControllerReg$ template class is used to advertise the existence of a custom controller type.
	//# The World Manager uses a controller registration to construct a custom controller, and the World Editor
	//# examines a controller registration to determine what type of node a custom controller can be assigned to.
	//# The act of instantiating a $ControllerReg$ object automatically registers the corresponding controller
	//# type. The controller type is unregistered when the $ControllerReg$ object is destroyed.
	//# 
	//# No more than one controller registration should be created for each distinct controller type.
	//
	//# \base	ControllerRegistration	All specific controller registration classes share the common base class $ControllerRegistration$.
	//
	//# \also	$@Controller@$
	
	
	template <class classType> class ControllerReg : public ControllerRegistration
	{
		public:
			
			ControllerReg(ControllerType type, const char *name) : ControllerRegistration(type, name)
			{
			}
			
			Controller *Construct(void) const
			{
				return (new classType);
			}
			
			bool ValidNode(const Node *node) const
			{
				return ((GetControllerName()) && (classType::ValidNode(node)));
			}
	};
	
	
	//# \class	Controller		Manages a dynamic node in a world.
	//
	//# The $Controller$ class manages a dynamic node in a world.
	//
	//# \def	class Controller : public ListElement<Controller>, public Packable,
	//# \def2	public Configurable, public Registrable<Controller, ControllerRegistration>
	//
	//# \ctor	Controller(ControllerType type = kControllerGeneric);
	//
	//# \param	type		The controller type.
	//
	//# \desc
	//# The $Controller$ class is the general mechanism through which dynamic nodes are managed in a world.
	//# Any node that moves for almost any reason is controlled by a specialized subclass of the $Controller$ class.
	//# The $Controller$ class also serves as the point of communication for nodes that need to be synchronized in
	//# a multiplayer environment.
	//
	//# \base	Utilities/ListElement<Controller>						Used internally by the World Manager.
	//# \base	ResourceMgr/Packable									Controllers can be packed for storage in resources.
	//# \base	InterfaceMgr/Configurable								Controllers can define configurable parameters that are exposed
	//#																	as user interface widgets in the World Editor.
	//# \base	System/Registrable<Controller, ControllerRegistration>	Custom controller types can be registered with the engine.
	//
	//# \also	$@ControllerReg@$
	
	
	//# \function	Controller::New		Constructs a new controller of a particular type.
	//
	//# \proto	static Controller *New(ControllerType type);
	//
	//# \param	type	The type of controller to construct.
	//
	//# \desc
	//# The $New$ function constructs a new controller having the type specified by the $type$ parameter.
	//
	//# \also	$@ControllerReg@$
	
	
	//# \function	Controller::GetControllerType		Returns the controller type.
	//
	//# \proto	ControllerType GetControllerType(void) const;
	//
	//# \desc
	//# The $GetControllerType$ function returns the controller type.
	//
	//# \also	$@ControllerReg@$
	
	
	//# \function	Controller::GetControllerIndex		Returns the World Manager controller index.
	//
	//# \proto	int32 GetControllerIndex(void) const;
	//
	//# \desc
	//# The $GetControllerIndex$ function returns the controller index that is assigned by the World Manager.
	//# Every controller is assigned a unique index by the server machine in a multiplayer game so that a
	//# particular controller can be identified on every machine in the game. A controller index is
	//# passed to the constructor of any $@MessageMgr/ControllerMessage@$ object in order to specify the
	//# message's destination.
	//
	//# \also	$@ControllerReg@$
	//# \also	$@MessageMgr/ControllerMessage@$
	
	
	//# \function	Controller::GetTargetNode		Returns the node to which a controller is attached.
	//
	//# \proto	Node *GetTargetNode(void) const;
	//
	//# \desc
	//# The $GetTargetNode$ function returns the node to which a controller is attached.
	//# A controller is attached to a node using the $@WorldMgr/Node::SetController@$ function.
	//
	//# \also	$@WorldMgr/Node::SetController@$
	
	
	//# \function	Controller::ValidNode		Returns a boolean value indicating whether the controller can be assigned to a particular node.
	//
	//# \proto	static bool ValidNode(const Node *node);
	//
	//# \desc
	//# The $ValidNode$ function should be redefined by controller subclasses. Its implementation should examine the
	//# node pointed to by the $node$ parameter and return $true$ if the controller type can be used with the node.
	//# If the controller type cannot be used, the $ValidNode$ function should return $false$. If the $ValidNode$
	//# function is not redefined for a registered subclass of the $Controller$ class, then that controller type
	//# can be assigned to any node.
	
	
	//# \function	Controller::Activate		Called when a controller is activated by some kind of trigger.
	//
	//# \proto	virtual void Activate(Node *trigger, Node *activator = nullptr);
	//
	//# \param	trigger		A pointer to the node that caused the controller to be activated. This can be $nullptr$.
	//# \param	activator	The node that activated the trigger. This can be $nullptr$.
	//
	//# \desc
	//# The $Activate$ function is called when some kind of trigger in the world causes a controller to be activated.
	//# This function can be called because a $@WorldMgr/Trigger@$ node was activated, or it can be called by a script.
	//# A controller subclass may perform whatever action is appropriate in response to the activation.
	//#
	//# The default implementation performs no action, so any override of the $Activate$ function does not need to
	//# call the base class counterpart.
	//
	//# \also	$@Controller::Deactivate@$
	//# \also	$@WorldMgr/World::ActivateTriggers@$
	//# \also	$@WorldMgr/Trigger@$
	
	
	//# \function	Controller::Deactivate		Called when the Trigger node that activated a controller is deactivated.
	//
	//# \proto	virtual void Deactivate(Node *trigger);
	//
	//# \param	trigger		A pointer to the node that caused the controller to be activated. This can be $nullptr$.
	//
	//# \desc
	//# The $Deactivate$ function is called when a $@WorldMgr/Trigger@$ node that previously activated the controller
	//# becomes deactivated. A controller subclass may perform whatever action is appropriate in response to the deactivation.
	//#
	//# The default implementation performs no action, so any override of the $Deactivate$ function does not need to
	//# call the base class counterpart.
	//
	//# \also	$@Controller::Activate@$
	//# \also	$@WorldMgr/World::ActivateTriggers@$
	//# \also	$@WorldMgr/Trigger@$
	
	
	//# \div
	//# \function	Controller::Preprocess		Performs any preprocessing that a controller needs to do before being used in a world.
	//
	//# \proto	virtual void Preprocess(void);
	//
	//# \desc
	//# The $Preprocess$ function is called when the node to which a controller is attached is preprocessed.
	//# A controller subclass may perform whatever action is necessary to initialize the controller.
	//# 
	//# Any override of the $Preprocess$ function should always call the base class counterpart,
	//# or the controller will not function correctly.
	//
	//# \special
	//# It is often the case that a controller will not want to perform certain initialization while it is being preprocessed inside
	//# the World Editor, but only when a game is actually being played. A controller can determine whether it is running inside the
	//# World Editor by calling the $GetManipulator$ function for its target node. If the return value is not $nullptr$, then the
	//# controller is running inside the World Editor. If the return value is $nullptr$, then the world to which the target node
	//# belongs is actually being played.
	//
	//# \also	$@WorldMgr/Node::Preprocess@$
	
	
	//# \function	Controller::Sleep		Puts a controller to sleep.
	//
	//# \proto	virtual void Sleep(void);
	//
	//# \desc
	//# The $Sleep$ function puts a controller to sleep so that it receives no processing time. The $@Controller::Move@$ function
	//# is not called for controllers that are currently in the sleeping state. A controller can be returned to the non-sleeping
	//# state by calling the $@Controller::Wake@$ function.
	//# 
	//# Any override of the $Sleep$ function should always call the base class counterpart.
	//
	//# \also	$@Controller::Wake@$
	//# \also	$@Controller::Move@$
	
	
	//# \function	Controller::Wake		Wakes a sleeping controller.
	//
	//# \proto	virtual void Wake(void);
	//
	//# \desc
	//# The $Wake$ function wakes a controller that was previously placed in the sleeping state by the $@Controller::Sleep@$ function.
	//# The $@Controller::Move@$ function is called only for controllers that are currently in the non-sleeping state.
	//# 
	//# Any override of the $Wake$ function should always call the base class counterpart.
	//
	//# \also	$@Controller::Sleep@$
	//# \also	$@Controller::Move@$
	
	
	//# \function	Controller::Move		Performs any per-frame movement or processing that a controller needs to do.
	//
	//# \proto	virtual void Move(void);
	//
	//# \desc
	//# The $Move$ function is called once per frame for all non-sleeping controllers in a world.
	//# A controller subclass may perform whatever action is appropriate to move its target node.
	//#
	//# The default implementation performs no action, so any override of the $Move$ function does not need to
	//# call the base class counterpart.
	//
	//# \also	$@Controller::Sleep@$
	//# \also	$@Controller::Wake@$
	
	
	//# \function	Controller::Update		Performs any processing that must be done before the node to which a controller is attached is rendered.
	//
	//# \proto	virtual void Update(void);
	//
	//# \desc
	//# The $Update$ function is called when a controller has been invalidated and its target node is about to be
	//# rendered. This gives the controller an opportunity to perform any calculations that could be deferred until
	//# its target node actually became visible.
	//# 
	//# Any override of the $Update$ function should always call the base class counterpart to clear the update flag.
	//
	//# \also	$@Controller::Invalidate@$ 
	
	
	//# \function	Controller::SetDetailLevel		Called when the detail level for the target node changes.
	//
	//# \proto	virtual void SetDetailLevel(int32 level);
	//
	//# \param	level	The new detail level.
	//
	//# \desc
	//# The $SetDetailLevel$ function is called when a controller is attached to a geometry node, and the level of
	//# detail for that geometry node changes. The new level of detail is given by the $level$ parameter.
	//# 
	//# Any override of the $SetDetailLevel$ function should always call the base class counterpart.
	
	
	//# \function	Controller::Invalidate		Indicates that a controller needs to be updated.
	//
	//# \proto	void Invalidate(void);
	//
	//# \desc
	//# The $Invalidate$ function should be called to indicate that a controller should be updated before the next
	//# time it is rendered. When a controller's target node is visible and the controller has been invalidated,
	//# the $@Controller::Update@$ function is called by the World Manager.
	//
	//# \also	$@Controller::Update@$
	
	
	//# \div
	//# \function	Controller::ConstructMessage		Called to construct a cotroller-defined message.
	//
	//# \proto	virtual ControllerMessage *ConstructMessage(ControllerMessageType type) const;
	//
	//# \param	type	The controller-specific type of the message to construct.
	//
	//# \desc
	//# The $ConstructMessage$ function is called when the Message Manager needs to construct a network message
	//# for a particular controller. The overriding implementation should examine the $type$ parameter and
	//# return a newly constructed instance of the appropriate message class. If the type is not recognized,
	//# then the base class counterpart should be called to construct the message.
	//
	//# \also	$@Controller::ReceiveMessage@$
	//# \also	$@MessageMgr/ControllerMessage@$
	
	
	//# \function	Controller::ReceiveMessage		Called to process a controller-defined message.
	//
	//# \proto	virtual void ReceiveMessage(const ControllerMessage *message);
	//
	//# \param	message		The message to process.
	//
	//# \desc
	//# The $ReceiveMessage$ function is called when the Message Manager successfully receives a network message
	//# for a particular controller. The overriding implementation should examine the type of the message and
	//# process it if the type is recognized. Otherwise, the base class counterpart should be called to process
	//# the message.
	//
	//# \also	$@Controller::ConstructMessage@$
	//# \also	$@MessageMgr/ControllerMessage@$
	
	
	//# \function	Controller::SendInitialStateMessages	Called to send messages containing the controller's state to a new player.
	//
	//# \proto	virtual void SendInitialStateMessages(Player *player) const;
	//
	//# \param	player		The player to which initial state messages should be sent.
	//
	//# \desc
	//# The $SendInitialStateMessages$ function is called for every controller in a world when a new client
	//# machine joins a multiplayer game. This function should send any messages necessary to synchronize the
	//# initial state of the controller on the new client by calling the $@MessageMgr/Player::SendMessage@$
	//# for the player specified by the $player$ parameter.
	//# 
	//# This function is called on the server machine after the $kPlayerInitialized$ event has been sent to
	//# the $@System/Application::HandlePlayerEvent@$ function and before the message journal is transmitted
	//# to a new client machine.
	//
	//# \also	$@MessageMgr/Player::SendMessage@$
	
	
	//# \function	Controller::HandleInteractionEvent		Called to handle an event for an interactive node.
	//
	//# \proto	virtual void HandleInteractionEvent(InteractionEventType type, const Point3D *position, Node *activator = nullptr);
	//
	//# \param	type		The event type. See the list of possible events below.
	//# \param	position	The position, in the target node's local coordinate system, where the event occurred.
	//# \param	activator	The node that caused the event to occur.
	//
	//# \desc
	//# The $HandleInteractionEvent$ function is called when an event needs to be handled for an interactive node.
	//# The $type$ parameter specifies the type of event that occurred and can be one of the following values.
	//
	//# \table	InteractionEventType
	
	
	class C4_API Controller : public ListElement<Controller>, public Packable, public Configurable, public Registrable<Controller, ControllerRegistration>
	{
		friend class Node;
		
		private:
			
			ControllerType				controllerType;
			ControllerType				baseControllerType;
			
			int32						controllerIndex;
			unsigned_int32				controllerFlags;
			
			Node						*targetNode;
			
			List<ControllerMessage>		journaledMessageList;
			
			virtual Controller *Replicate(void) const;
			
			void SetTargetNode(Node *node);
		
		protected:
			
			Controller(const Controller& controller);
			
			void SetBaseControllerType(ControllerType type)
			{
				baseControllerType = type;
			}
		
		public:
			
			enum
			{
				kControllerMessageSetting				= 255,
				kControllerMessageFunction				= 254,
				kControllerMessageWake					= 253,
				kControllerMessageSleep					= 252,
				kControllerMessageEnableNode			= 251,
				kControllerMessageDisableNode			= 250,
				kControllerMessageDeleteNode			= 249,
				kControllerMessageEnableInteractivity	= 248,
				kControllerMessageDisableInteractivity	= 247,
				kControllerMessageShowGeometry			= 246,
				kControllerMessageHideGeometry			= 245,
				kControllerMessagePlaySource			= 244,
				kControllerMessageStopSource			= 243,
				kControllerMessageMaterialColor			= 242,
				kControllerMessageShaderParameter		= 241
			};
			
			Controller(ControllerType type = kControllerGeneric);
			virtual ~Controller();
			
			ControllerType GetControllerType(void) const
			{
				return (controllerType);
			}
			
			ControllerType GetBaseControllerType(void) const
			{
				return (baseControllerType);
			}
			
			int32 GetControllerIndex(void) const
			{
				return (controllerIndex);
			}
			
			void SetControllerIndex(int32 index)
			{
				controllerIndex = index;
			}
			
			unsigned_int32 GetControllerFlags(void) const
			{
				return (controllerFlags);
			}
			
			void SetControllerFlags(unsigned_int32 flags)
			{
				controllerFlags = flags;
			}
			
			bool Asleep(void) const
			{
				return ((controllerFlags & kControllerAsleep) != 0);
			}
			
			Node *GetTargetNode(void) const
			{
				return (targetNode);
			}
			
			void Invalidate(void)
			{
				controllerFlags |= kControllerUpdate;
			}
			
			ControllerMessage *GetFirstJournaledMessage(void) const
			{
				return (journaledMessageList.First());
			}
			
			void AddJournaledMessage(ControllerMessage *message)
			{
				journaledMessageList.Append(message);
			}
			
			Controller *Clone(void) const
			{
				return (Replicate());
			}
			
			static Controller *New(ControllerType type);
			
			static bool ValidNode(const Node *node);
			static void RegisterStandardControllers(void);
			
			void PackType(Packer& data) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			virtual void Preprocess(void);
			virtual void Neutralize(void);
			
			virtual ControllerMessage *ConstructMessage(ControllerMessageType type) const;
			virtual void ReceiveMessage(const ControllerMessage *message);
			virtual void SendInitialStateMessages(Player *player) const;
			
			virtual void EnterWorld(Zone *zone, const Point3D& zonePosition);
			virtual void ChangeZones(Zone *zone, const Transform4D& transform);
			
			virtual void Wake(void);
			virtual void Sleep(void);
			
			virtual void Move(void);
			virtual void StopMotion(void);
			virtual void Update(void);
			
			virtual void SetDetailLevel(int32 level);
			
			virtual void Activate(Node *trigger, Node *activator = nullptr);
			virtual void Deactivate(Node *trigger);
			
			virtual void HandleInteractionEvent(InteractionEventType type, const Point3D *position, Node *activator = nullptr);
	};
	
	
	class C4_API SettingMessage : public ControllerMessage
	{
		friend class Controller;
		
		private:	
			
			Type		settingCategory;
			Setting		*messageSetting;
			
			SettingMessage(int32 controllerIndex);
		
		public:
			
			SettingMessage(int32 controllerIndex, Type category, const Setting *setting);
			~SettingMessage();
			
			Type GetSettingCategory(void) const
			{
				return (settingCategory);
			}
			
			const Setting *GetSetting(void) const
			{
				return (messageSetting);
			}
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			bool HandleControllerMessage(Controller *controller) const;
			bool OverridesMessage(const ControllerMessage *message) const;
	};
	
	
	class C4_API FunctionMessage : public ControllerMessage
	{
		friend class Controller;
		
		private:	
			
			Function	*messageFunction;
			
			FunctionMessage(int32 controllerIndex);
		
		public:
			
			FunctionMessage(int32 controllerIndex, const Function *function);
			~FunctionMessage();
			
			Function *GetFunction(void) const
			{
				return (messageFunction);
			}
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			bool HandleControllerMessage(Controller *controller) const;
			bool OverridesMessage(const ControllerMessage *message) const;
	};
	
	
	class C4_API WakeSleepMessage : public ControllerMessage
	{
		public:
			
			WakeSleepMessage(ControllerMessageType type, int32 controllerIndex);
			~WakeSleepMessage();
			
			bool HandleControllerMessage(Controller *controller) const;
			bool OverridesMessage(const ControllerMessage *message) const;
	};
	
	
	class C4_API NodeEnableDisableMessage : public ControllerMessage
	{
		public:
			
			NodeEnableDisableMessage(ControllerMessageType type, int32 controllerIndex);
			~NodeEnableDisableMessage();
			
			bool HandleControllerMessage(Controller *controller) const;
			bool OverridesMessage(const ControllerMessage *message) const;
	};
	
	
	class C4_API DeleteNodeMessage : public ControllerMessage
	{
		public:
			
			DeleteNodeMessage(int32 controllerIndex);
			~DeleteNodeMessage();
			
			bool HandleControllerMessage(Controller *controller) const;
	};
	
	
	class C4_API NodeInteractivityMessage : public ControllerMessage
	{
		public:
			
			NodeInteractivityMessage(ControllerMessageType type, int32 controllerIndex);
			~NodeInteractivityMessage();
			
			bool HandleControllerMessage(Controller *controller) const;
			bool OverridesMessage(const ControllerMessage *message) const;
	};
	
	
	class C4_API GeometryVisibilityMessage : public ControllerMessage
	{
		public:
			
			GeometryVisibilityMessage(ControllerMessageType type, int32 controllerIndex);
			~GeometryVisibilityMessage();
			
			bool HandleControllerMessage(Controller *controller) const;
			bool OverridesMessage(const ControllerMessage *message) const;
	};
	
	
	class C4_API SourcePlayStopMessage : public ControllerMessage
	{
		public:
			
			SourcePlayStopMessage(ControllerMessageType type, int32 controllerIndex);
			~SourcePlayStopMessage();
			
			bool HandleControllerMessage(Controller *controller) const;
			bool OverridesMessage(const ControllerMessage *message) const;
	};
	
	
	class C4_API MaterialColorMessage : public ControllerMessage
	{
		friend class Controller;
		
		private:
			
			AttributeType	attributeType;
			ColorRGBA		materialColor;
			
			MaterialColorMessage(int32 controllerIndex);
		
		public:
			
			MaterialColorMessage(int32 controllerIndex, AttributeType type, const ColorRGBA& color);
			~MaterialColorMessage();
			
			AttributeType GetAttributeType(void) const
			{
				return (attributeType);
			}
			
			const ColorRGBA& GetMaterialColor(void) const
			{
				return (materialColor);
			}
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			bool HandleControllerMessage(Controller *controller) const;
			bool OverridesMessage(const ControllerMessage *message) const;
	};
	
	
	class C4_API ShaderParameterMessage : public ControllerMessage
	{
		friend class Controller;
		
		private:
			
			int32			parameterSlot;
			Vector4D		parameterValue;
			
			ShaderParameterMessage(int32 controllerIndex);
		
		public:
			
			ShaderParameterMessage(int32 controllerIndex, int32 slot, const Vector4D& param);
			~ShaderParameterMessage();
			
			int32 GetParameterSlot(void) const
			{
				return (parameterSlot);
			}
			
			const Vector4D& GetParameterValue(void) const
			{
				return (parameterValue);
			}
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			bool HandleControllerMessage(Controller *controller) const;
			bool OverridesMessage(const ControllerMessage *message) const;
	};
}


#endif

// ZYURVUR
