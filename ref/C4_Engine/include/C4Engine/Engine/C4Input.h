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


#ifndef C4Input_h
#define C4Input_h


//# \component	Input Manager
//# \prefix		InputMgr/


#include "C4Variables.h"

#if C4WINDOWS && C4FASTBUILD

	#include "C4DirectInput.h"

#endif

namespace C4
{
	//# \tree	InputControl
	//
	//# \node	ButtonControl
	//# \sub
	//#		\node	GenericButtonControl
	//#		\node	KeyButtonControl
	//# \end
	//# \node	AxisControl
	//# \sub
	//#		\node	LinearAxisControl
	//#		\node	RotationAxisControl
	//#		\node	DeltaAxisControl
	//# \end
	//#	\node	SliderControl
	//# \node	DirectionalControl
	
	
	typedef EngineResult InputResult;
	
	typedef unsigned_int32 InputMode;
	
	typedef Type	InputDeviceType;
	typedef Type	InputControlType;
	typedef Type	ActionType;
	
	
	enum
	{
		kInputOkay				= kEngineOkay,
		kInputInitFailed		= (kManagerInput << 16) | 0x0001
	};
	
	
	enum
	{
		kMaxInputDeviceNameLength		= 255,
		kMaxInputControlNameLength		= 127,
		kMaxInputFeedbackNameLength		= 127
	};
	
	
	typedef String<kMaxInputDeviceNameLength>		InputDeviceName;
	typedef String<kMaxInputControlNameLength>		InputControlName;
	typedef String<kMaxInputFeedbackNameLength>		InputFeedbackName;
	
	
	#if C4LINUX
	
		enum
		{
			kKeyboardRawCodeBase		= 8,
			kKeyboardRawCodeCount		= 248
		};
	
	#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#endif //]
	
	
	#if C4XINPUT
	
		enum
		{
			kXboxButtonCount			= 14,
			kXboxTriggerCount			= 2,
			kXboxAxisCount				= 4
		};
	
	#endif
	
	
	//# \enum	InputDeviceType
	
	enum
	{
		kInputMouse				= 'MOUS',		//## A mouse device.
		kInputKeyboard			= 'KYBD',		//## A keyboard device.
		kInputXbox				= 'XBOX',		//## An Xbox 360 controller.
		
		#if !C4GAMECONSOLE
		
			kInputJoystick		= 'JSTK'		//## A joystick or other device. 
		
		#elif C4PLAYSTATION3 //[ PS3 
 
			// -- PlayStation 3 code hidden -- 

		#endif //] 
	};
	
	
	//# \enum	InputControlType 
	
	enum
	{
		kInputGroup				= 0,			//## A group of input controls. 
		kInputKeyButton			= 'KEYB',		//## A key button control.
		kInputGenericButton		= 'BTTN',		//## A generic button control.
		kInputLinearAxis		= 'AXIS',		//## A linear axis control.
		kInputRotationAxis		= 'RTAX',		//## A rotation axis control.
		kInputDeltaAxis			= 'DELT',		//## A delta axis control.
		kInputSlider			= 'SLID',		//## A slider control.
		kInputDirectional		= 'DPAD'		//## A directional pad control.
	};
	
	
	enum
	{
		kActionConsole			= 'CONS',
		kActionEscape			= 'ESCP',
		kActionMouseX			= 'MOSX',
		kActionMouseY			= 'MOSY'
	};
	
	
	enum
	{
		kActionImmutable		= 1 << 0,
		kActionPersistent		= 1 << 1
	};
	
	
	//# \enum	MouseFlags
	
	enum
	{
		kMouseInverted			= 1 << 0,		//## Movement along the vertical axis of the mouse is negated.
		kMouseSmooth			= 1 << 1		//## Mouse movements are smoothed over multiple input events.
	};
	
	
	//# \enum	InputMode
	
	enum
	{
		kInputInactive			= 0,															//## No input is processed by the Input Manager. This flag should be specified by itself to disable the Input Manager.
		kInputMouseActive		= 1 << 0,														//## Mouse input is processed by the Input Manager.
		kInputKeyboardActive	= 1 << 1,														//## Keyboard input is processed by the Input Manager.
		kInputGameActive		= 1 << 2,														//## Joystick and game controller input is processed by the Input Manager.
		kInputAllActive			= kInputMouseActive | kInputKeyboardActive | kInputGameActive,	//## All input is processed by the Input Manager. Specifying this flag is equivalent to specifying $kInputMouseActive$, $kInputKeyboardActive$, and $kInputGameActive$.
		kInputConfiguration		= 0xFFFFFFFF
	};
	
	
	enum InputAxis
	{
		kInputAxisNone = -1,
		kInputAxisX,
		kInputAxisY,
		kInputAxisZ
	};
	
	
	class InputDevice;
	
	
	//# \class	Action		Represents an action that can be triggered by an input control.
	//
	//# The $Action$ class represents an action that can be triggered by an input control.
	//
	//# \def	class Action : public LinkTarget<Action>, public ListElement<Action>, public Memory<InputDevice>
	//
	//# \ctor	explicit Action(ActionType type);
	//
	//# The constructor has protected access. An $Action$ class can only exist as the base class for a more specific type of action.
	//
	//# \param	type	The type of the action.
	//
	//# \desc
	//# An application typically creates several $Action$ objects representing the various actions that a player
	//# can perform in a game. Examples of actions are forward and backward movements and firing a weapon. Once an
	//# action object is created, it must be registered with the Input Manager by calling the $@InputMgr::AddAction@$
	//# function.
	//#
	//# An action object is assigned to an input control by calling the $@InputControl::SetControlAction@$ function.
	//# One action object may be assigned to multiple input controls. After an action object is assigned to an input
	//# control, it can receive calls to its virtual member functions in response to user input to that input control.
	//#
	//# Each action must have a unique type. An application can choose any 32-bit identifier that does not consist entirely
	//# of uppercase letters and numbers to pass in the $type$ paramater of the $Action$ constructor.
	//
	//# \base	Utilities/LinkTarget<Action>	Used internally by the Input Manager.
	//# \base	Utilities/ListElement<Action>	Used internally by the Input Manager.
	//# \base	MemoryMgr/Memory<InputDevice>	Actions are allocated in a dedicated heap.
	//
	//# \also	$@InputControl@$
	//# \also	$@InputMgr::AddAction@$
	//# \also	$@InputMgr::RemoveAction@$
	
	
	//# \function	Action::GetActionType		Returns the type of action.
	//
	//# \proto	ActionType GetActionType(void) const;
	//
	//# \desc
	//# The $GetActionType$ function returns the action type that was established when the action object was constructed.
	
	
	//# \function	Action::Begin		Called when an associated input control is activated.
	//
	//# \proto	virtual void Begin(void);
	//
	//# \desc
	//# The $Begin$ function is called when an input control to which the action object is assigned is activated by the user.
	//# For example, when a key is pressed, a mouse button is clicked, or a button on a joystick is pressed, the $Begin$
	//# function is called for any action object assigned to the control. When the user releases the control, the
	//# $@Action::End@$ function is called for the action object.
	//
	//# \also	$@Action::End@$
	//# \also	$@Action::Move@$
	//# \also	$@Action::Update@$
	
	
	//# \function	Action::End			Called when an associated input control is deactivated.
	//
	//# \proto	virtual void End(void);
	//
	//# \desc
	//# The $End$ function is called when an input control to which the action object is assigned is deactivated by the user.
	//# For example, when a key, mouse button, or joystick button is released, the $End$ function is called for any action
	//# object assigned to the control. The $@Action::Begin@$ function would have previously been called for the action object
	//# at the time that the user activated the control.
	//
	//# \also	$@Action::Begin@$
	//# \also	$@Action::Move@$
	//# \also	$@Action::Update@$
	
	
	//# \function	Action::Move		Called when an associated input control has new directional data.
	//
	//# \proto	virtual void Move(int32 value);
	//
	//# \param	value	The new control data.
	//
	//# \desc
	//# The $Move$ function is called when a directional pad input control to which the action object is assigned is manipulated
	//# by the user. If the user lets go of the directional pad (allowing it to return to the center position), then the $value$
	//# parameter is &minus;1. Otherwise, the $value$ parameter is in the range [0,&nbsp;7], where 0 corresponds to straight up,
	//# and higher values represent directions in 45-degree increments moving clockwise. For instance, 1 corresponds to
	//# halfway between right and up, 2 means directly to the right, and 6 means directly to the left.
	//
	//# \also	$@Action::Begin@$
	//# \also	$@Action::End@$
	//# \also	$@Action::Update@$
	
	
	//# \function	Action::Update		Called when an associated input control has new axis data.
	//
	//# \proto	virtual void Update(float value);
	//
	//# \param	value	The new control data.
	//
	//# \desc
	//# The $Update$ function is called when the position of an analog axis control to which the action object is assigned
	//# is changed by the user. For an absolute axis, the $value$ parameter is in the range [&minus;1.0F,&nbsp;1.0F], where
	//# 0.0F corresponds to the center position. For a relative axis (such as a mouse axis), the $value$ parameter
	//# represents the relative displacement without normalization to any predefined range. For a slider axis, the
	//# value parameter is in the range [0.0F,&nbsp;1.0F].
	//
	//# \also	$@Action::Begin@$
	//# \also	$@Action::End@$
	//# \also	$@Action::Move@$
	
	
	class C4_API Action : public LinkTarget<Action>, public ListElement<Action>, public Memory<InputDevice>
	{
		private:
			
			ActionType			actionType;
			unsigned_int32		actionFlags;
			
			int32				activeCount;
		
		protected:
			
			explicit Action(ActionType type);
		
		public:
			
			virtual ~Action();
			
			ActionType GetActionType(void) const
			{
				return (actionType);
			}
			
			unsigned_int32 GetActionFlags(void) const
			{
				return (actionFlags);
			}
			
			void SetActionFlags(unsigned_int32 flags)
			{
				actionFlags = flags;
			}
			
			int32 GetActiveCount(void) const
			{
				return (activeCount);
			}
			
			void SetActiveCount(int32 count)
			{
				activeCount = count;
			}
			
			virtual void Begin(void);
			virtual void End(void);
			virtual void Move(int32 value);
			virtual void Update(float value);
	};
	
	
	class C4_API ConsoleAction : public Action
	{
		public:
			
			ConsoleAction();
			~ConsoleAction();
			
			void Begin(void);
	};
	
	
	class C4_API EscapeAction : public Action
	{
		public:
			
			EscapeAction();
			~EscapeAction();
			
			void Begin(void);
	};
	
	
	class C4_API MouseAction : public Action
	{
		public:
			
			MouseAction(ActionType type);
			~MouseAction();
			
			void Update(float value);
	};
	
	
	class C4_API CommandAction : public Action
	{
		private:
			
			String<kMaxVariableValueLength>		command;
		
		public:
			
			CommandAction(const char *cmd);
			~CommandAction();
			
			const char *GetCommand(void) const
			{
				return (command);
			}
			
			void Begin(void);
	};
	
	
	//# \class	InputControl	Encapsulates an individual input device control.
	//
	//# \def	class InputControl : public Tree<InputControl>, public Memory<InputDevice>
	//
	//# \desc
	//# The $InputControl$ class represents an individual input control for an input device.
	//# An input control can have one of the following types.
	//
	//# \table	InputControlType
	//
	//# Events for an input control are communicated to the application through $@Action@$ objects.
	//# An action is assigned to an input control using the $@InputControl::SetControlAction@$ function.
	//
	//# \base	Utilities/Tree<InputMgr>		Input controls are stored in a tree structure.
	//# \base	MemoryMgr/Memory<InputDevice>	Input controls are allocated in a dedicated heap.
	//
	//# \also	$@Action@$
	//# \also	$@InputDevice@$
	
	
	//# \function	InputControl::GetControlType		Returns the type of an input control.
	//
	//# \proto	InputControlType GetControlType(void) const;
	//
	//# \desc
	//# The $GetControlType$ function returns the type of an input control, which can be one of
	//# the following values.
	//
	//# \table	InputControlType
	//
	//# \also	$@InputControl::GetControlName@$
	
	
	//# \function	InputControl::GetControlName		Returns the name of an input control.
	//
	//# \proto	const char *GetControlName(void) const;
	//
	//# \desc
	//# The $GetControlName$ function returns a pointer to the name of an input control.
	//
	//# \also	$@InputControl::GetControlType@$
	
	
	//# \function	InputControl::GetControlAction		Returns the action assigned to an input control.
	//
	//# \proto	Action *GetControlAction(void) const;
	//
	//# \desc
	//# The $GetControlAction$ function returns a pointer to the $@Action@$ object assigned to an input control.
	//# If no action is assigned to a control, then the return value is $nullptr$.
	//
	//# \also	$@InputControl::SetControlAction@$
	//# \also	$@Action@$
	
	
	//# \function	InputControl::SetControlAction		Assigns an action to an input control.
	//
	//# \proto	void SetControlAction(Action *action);
	//
	//# \param	action		The action to assign to the input control. This can be $nullptr$ to remove any
	//#						previously assigned action.
	//
	//# \desc
	//# The $SetControlAction$ function assigns an $@Action@$ object to an input control. Once an action
	//# has been assigned, its member functions are called whenever the input control processes an event.
	//
	//# \also	$@InputControl::GetControlAction@$
	//# \also	$@Action@$
	
	
	class C4_API InputControl : public Tree<InputControl>, public Memory<InputDevice>
	{
		friend class InputDevice;
		friend class JoystickDevice;
		
		#if C4WINDOWS
		
			friend class DirectInputDevice;
		
		#endif
		
		private:
			
			InputControlType			controlType;
			
			InputDevice					*owningDevice;
			Link<Action>				controlAction;
			Link<Action>				activeAction;
			
			InputControlName			controlName;
			
			#if C4WINDOWS
			
				GUID					dataGuid;
				DWORD					dataType;
				DWORD					dataFlags;
				DWORD					dataOffset;
			
			#elif C4MACOS
			
				IOHIDElementCookie		controlCookie;
				bool					controlActive;
			
			#endif
		
		protected:
			
			#if C4WINDOWS
			
				InputControl(InputControlType type, InputDevice *device, const DIDEVICEOBJECTINSTANCEA *instance);
			
			#endif
			
			InputControl(InputControlType type, InputDevice *device, const char *name);
			
			void SetActiveAction(Action *action)
			{
				activeAction = action;
			}
		
		public:
			
			InputControl(InputDevice *device);
			virtual ~InputControl();
			
			InputControlType GetControlType(void) const
			{
				return (controlType);
			}
			
			const char *GetControlName(void) const
			{
				return (controlName);
			}
			
			InputDevice *GetOwningDevice(void) const
			{
				return (owningDevice);
			}
			
			Action *GetControlAction(void) const
			{
				return (controlAction);
			}
			
			void SetControlAction(Action *action);
			
			#if C4MACOS
			
				void Activate(IOHIDQueueInterface **deviceQueue);
				void Deactivate(IOHIDQueueInterface **deviceQueue);
			
			#endif
			
			virtual void HandleNormalEvent(int32 value);
			virtual bool HandleConfigEvent(int32 value);
	};
	
	
	//# \class	ButtonControl	Encapsulates a button input control.
	//
	//# \def	class ButtonControl : public InputControl
	//
	//# \desc
	//# The $ButtonControl$ class is the base class for all input controls that are buttons.
	//#
	//# If an action is assigned to a $ButtonControl$ object, an internal counter for the action is incremented when the button is pressed,
	//# and it is decremented when the button is released. When the counter is incremented to 1, the action's $@Action::Begin@$ function
	//# is called, and when the counter is decremented to 0, the action's $@Action::End@$ function is called. This mechanism allows an
	//# action to be assigned to multiple buttons without redundant events being reported.
	//
	//# \base	InputControl	A $ButtonControl$ is a specific type of $InputControl$.
	//
	//# \also	$@Action::Begin@$
	//# \also	$@Action::End@$
	
	
	class C4_API ButtonControl : public InputControl
	{
		private:
			
			unsigned_int32	dataMask;
		
		protected:
			
			#if C4WINDOWS
			
				ButtonControl(InputControlType type, InputDevice *device, const DIDEVICEOBJECTINSTANCEA *instance);
			
			#endif
			
			ButtonControl(InputControlType type, InputDevice *device, const char *name);
		
		public:
			
			~ButtonControl();
			
			void HandleNormalEvent(int32 value);
			bool HandleConfigEvent(int32 value);
	};
	
	
	//# \class	KeyButtonControl		Encapsulates a key button input control.
	//
	//# \def	class KeyButtonControl : public ButtonControl
	//
	//# \desc
	//# The $KeyButtonControl$ class represents a single key on a keyboard device. If an action is assigned
	//# to a $KeyButtonControl$ object, then events are reported as described for the $@ButtonControl@$ base class.
	//
	//# \base	ButtonControl	A $KeyButtonControl$ is a specific type of $ButtonControl$.
	
	
	class C4_API KeyButtonControl : public ButtonControl
	{
		public:
			
			#if C4WINDOWS
			
				KeyButtonControl(InputDevice *device, const DIDEVICEOBJECTINSTANCEA *instance);
			
			#endif
			
			KeyButtonControl(InputDevice *device, const char *name);
			~KeyButtonControl();
	};
	
	
	//# \class	GenericButtonControl		Encapsulates a generic button input control.
	//
	//# \def	class GenericButtonControl : public ButtonControl
	//
	//# \desc
	//# The $GenericButtonControl$ class represents any button on an input device that is not a key on a keyboard.
	//# If an action is assigned to a $GenericButtonControl$ object, then events are reported as described for the
	//# $@ButtonControl@$ base class.
	//
	//# \base	ButtonControl	A $GenericButtonControl$ is a specific type of $ButtonControl$.
	
	
	class C4_API GenericButtonControl : public ButtonControl
	{
		public:
			
			#if C4WINDOWS
			
				GenericButtonControl(InputDevice *device, const DIDEVICEOBJECTINSTANCEA *instance);
			
			#endif
			
			GenericButtonControl(InputDevice *device, const char *name);
			~GenericButtonControl();
	};
	
	
	//# \class	AxisControl		Encapsulates an axis input control.
	//
	//# \def	class AxisControl : public InputControl
	//
	//# \desc
	//# The $AxisControl$ class is the base class for all input controls that are based on a two-way axis.
	//#
	//# If an action is assigned to an $AxisControl$ object, then the action's $@Action::Update@$ function is called
	//# when the position of the axis control changes. For an absolute axis, the value passed to the $Update$ function
	//# is normalized to the range [&minus;1.0F,&nbsp;1.0F], where 0.0F corresponds to the center position. For a relative
	//# axis (such as a mouse axis), the $value$ parameter represents the relative displacement without normalization to
	//# any predefined range.
	//
	//# \base	InputControl	A $AxisControl$ is a specific type of $InputControl$.
	//
	//# \also	$@Action::Update@$
	
	
	class C4_API AxisControl : public InputControl
	{
		private:
			
			InputAxis		controlAxis;
			
			float			minValue;
			float			maxValue;
			float			centerValue;
			float			deadZone;
			float			normalizer;
		
		protected:
			
			#if C4WINDOWS
			
				AxisControl(InputControlType type, InputDevice *device, InputAxis axis, const DIDEVICEOBJECTINSTANCEA *instance);
			
			#endif
			
			AxisControl(InputControlType type, InputDevice *device, InputAxis axis, const char *name);
		
		public:
			
			~AxisControl();
			
			InputAxis GetControlAxis(void) const
			{
				return (controlAxis);
			}
			
			void SetRange(float vmin, float vmax, float dead);
			
			void HandleNormalEvent(int32 value);
			bool HandleConfigEvent(int32 value);
	};
	
	
	//# \class	LinearAxisControl		Encapsulates a linear axis input control.
	//
	//# \def	class LinearAxisControl : public AxisControl
	//
	//# \desc
	//# The $LinearAxisControl$ class represents an input control that uses a linear axis.
	//# If an action is assigned to a $LinearAxisControl$ object, then events are reported as described for
	//# the $AxisControl$ base class.
	//
	//# \base	AxisControl		A $LinearAxisControl$ is a specific type of $AxisControl$.
	
	
	class C4_API LinearAxisControl : public AxisControl
	{
		public:
			
			#if C4WINDOWS
			
				LinearAxisControl(InputDevice *device, InputAxis axis, const DIDEVICEOBJECTINSTANCEA *instance);
			
			#endif
			
			LinearAxisControl(InputDevice *device, InputAxis axis, const char *name);
			~LinearAxisControl();
	};
	
	
	//# \class	RotationAxisControl		Encapsulates a rotation axis input control.
	//
	//# \def	class RotationAxisControl : public AxisControl
	//
	//# \desc
	//# The $RotationAxisControl$ class represents an input control that uses a rotational axis.
	//# If an action is assigned to a $RotationAxisControl$ object, then events are reported as described for
	//# the $AxisControl$ base class.
	//
	//# \base	AxisControl		A $RotationAxisControl$ is a specific type of $AxisControl$.
	
	
	class C4_API RotationAxisControl : public AxisControl
	{
		public:
			
			#if C4WINDOWS
			
				RotationAxisControl(InputDevice *device, InputAxis axis, const DIDEVICEOBJECTINSTANCEA *instance);
			
			#endif
			
			RotationAxisControl(InputDevice *device, InputAxis axis, const char *name);
			~RotationAxisControl();
	};
	
	
	//# \class	DeltaAxisControl		Encapsulates a delta axis input control.
	//
	//# \def	class DeltaAxisControl : public AxisControl
	//
	//# \desc
	//# The $DeltaAxisControl$ class represents an input control that uses a relative linear axis.
	//# If an action is assigned to a $DeltaAxisControl$ object, then events are reported as described for
	//# the $AxisControl$ base class.
	//
	//# \base	AxisControl		A $DeltaAxisControl$ is a specific type of $AxisControl$.
	
	
	class C4_API DeltaAxisControl : public AxisControl
	{
		public:
			
			#if C4WINDOWS
			
				DeltaAxisControl(InputDevice *device, InputAxis axis, const DIDEVICEOBJECTINSTANCEA *instance);
			
			#endif
			
			DeltaAxisControl(InputDevice *device, InputAxis axis, const char *name);
			~DeltaAxisControl();
			
			void HandleNormalEvent(int32 value);
			bool HandleConfigEvent(int32 value);
	};
	
	
	//# \class	SliderControl		Encapsulates a slider input control.
	//
	//# \def	class SliderControl : public InputControl
	//
	//# \desc
	//# The $SliderControl$ class represents an input control that uses a one-way axis, or slider.
	//#
	//# If an action is assigned to a $SliderControl$ object, then the action's $@Action::Update@$ function is called
	//# when the position of the slider changes. The value passed to the $Update$ function is normalized to the range [0.0F,&nbsp;1.0F].
	//
	//# \base	InputControl	A $SliderControl$ is a specific type of $InputControl$.
	//
	//# \also	$@Action::Update@$
	
	
	class C4_API SliderControl : public InputControl
	{
		private:
			
			float			maxValue;
			float			threshold;
			float			normalizer;
		
		public:
			
			#if C4WINDOWS
			
				SliderControl(InputDevice *device, const DIDEVICEOBJECTINSTANCEA *instance);
			
			#endif
			
			SliderControl(InputDevice *device, const char *name);
			~SliderControl();
			
			void SetRange(float vmin, float vmax, float thresh);
			
			void HandleNormalEvent(int32 value);
			bool HandleConfigEvent(int32 value);
	};
	
	
	//# \class	DirectionalControl		Encapsulates a directional pad input control.
	//
	//# \def	class DirectionalControl : public InputControl
	//
	//# \desc
	//# The $DirectionalControl$ class represents an input control that is a directional pad.
	//#
	//# If an action is assigned to a $DirectionalControl$ object, then the action's $@Action::Move@$ function is called
	//# when the state of the directional pad changes. If the user lets go of the directional pad (allowing it to return to
	//# the center position), then the value passed to the $Move$ function is &minus;1. Otherwise, the value is in the range
	//# [0,&nbsp;7], where 0 corresponds to straight up, and higher values represent directions in 45-degree increments moving
	//# clockwise. For instance, 1 corresponds to halfway between right and up, 2 means directly to the right, and 6 means directly to the left.
	//
	//# \base	InputControl	A $DirectionalControl$ is a specific type of $InputControl$.
	//
	//# \also	$@Action::Move@$
	
	
	class C4_API DirectionalControl : public InputControl
	{
		private:
			
			int32	divider;
		
		public:
			
			#if C4WINDOWS
			
				DirectionalControl(InputDevice *device, const DIDEVICEOBJECTINSTANCEA *instance);
			
			#endif
			
			DirectionalControl(InputDevice *device, const char *name);
			~DirectionalControl();
			
			void HandleNormalEvent(int32 value);
			bool HandleConfigEvent(int32 value);
	};
	
	
	class C4_API InputFeedback : public ListElement<InputFeedback>, public Memory<InputDevice>
	{
		private:
			
			InputFeedbackName		feedbackName;
		
		public:
			
			InputFeedback(const char *name);
			~InputFeedback();
			
			const char *GetFeedbackName(void) const
			{
				return (feedbackName);
			}
	};
	
	
	//# \class	InputDevice		Encapsulates an individual input device.
	//
	//# \def	class InputDevice : public ListElement<InputDevice>, public Memory<InputDevice>
	//
	//# \desc
	//#
	//
	//# \base	Utilities/ListElement<InputDevice>	Used internally by the Input Manager.
	//# \base	MemoryMgr/Memory<InputDevice>		Input devices are allocated in a dedicated heap.
	//
	//# \also	$@InputControl@$
	
	
	//# \function	InputDevice::GetDeviceType		Returns the type of an input device.
	//
	//# \proto	InputDeviceType GetDeviceType(void) const;
	//
	//# \desc
	//#
	//# \table	InputDeviceType
	
	
	//# \function	InputDevice::GetDeviceName		Returns the name of an input device.
	//
	//# \proto	const char *GetDeviceName(void) const;
	//
	//# \desc
	//
	
	
	//# \function	InputDevice::GetFirstControl	Returns the first control belonging to an input device.
	//
	//# \proto	InputControl *GetFirstControl(void) const;
	//
	//# \desc
	//
	//# \also	$@InputDevice::GetNextControl@$
	//# \also	$@InputDevice::FindControl@$
	//# \also	$@InputControl@$
	
	
	//# \function	InputDevice::GetNextControl		Returns the next control in an input device's control tree.
	//
	//# \proto	InputControl *GetNextControl(const InputControl *control) const;
	//
	//# \param	control		The most recently visited control.
	//
	//# \desc
	//
	//# \also	$@InputDevice::GetFirstControl@$
	//# \also	$@InputDevice::FindControl@$
	//# \also	$@InputControl@$
	
	
	//# \function	InputDevice::FindControl		Returns the control having a given name.
	//
	//# \proto	InputControl *FindControl(const char *name) const;
	//
	//# \param	name	The name of the control to find.
	//
	//# \desc
	//
	//# \also	$@InputDevice::GetFirstControl@$
	//# \also	$@InputDevice::GetNextControl@$
	//# \also	$@InputControl@$
	
	
	class C4_API InputDevice : public ListElement<InputDevice>, public Memory<InputDevice>
	{
		private:
			
			InputDeviceType					deviceType;
			bool							deviceActive;
		
		protected:
		
			InputControl					controlTree;
			List<InputFeedback>				feedbackList;
			
			#if C4MACOS
			
				IOCFPlugInInterface			**pluginInterface;
				IOHIDDeviceInterface		**deviceInterface;
				IOHIDQueueInterface			**deviceQueue;
			
			#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

			#endif //]
			
			InputDeviceName					deviceName;
			
			#if C4XINPUT || C4WINDOWS || C4LINUX
			
				InputDevice(InputDeviceType type, const char *name);
			
			#elif C4MACOS
			
				InputDevice(InputDeviceType type, io_object_t object, CFMutableDictionaryRef properties);
				
				void BuildControlTree(InputControl *root, CFMutableDictionaryRef dictionary);
			
			#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

			#endif //]
			
			virtual void Activate(void);
			virtual void Deactivate(void);
		
		public:
			
			virtual ~InputDevice();
			
			InputDeviceType GetDeviceType(void) const
			{
				return (deviceType);
			}
			
			bool DeviceActive(void) const
			{
				return (deviceActive);
			}
			
			#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

			#endif //]
			
			const char *GetDeviceName(void) const
			{
				return (deviceName);
			}
			
			InputControl *GetFirstControl(void) const
			{
				return (controlTree.GetNextNode(&controlTree));
			}
			
			InputControl *GetNextControl(const InputControl *control) const
			{
				return (controlTree.GetNextNode(control));
			}
			
			InputFeedback *GetFirstFeedback(void) const
			{
				return (feedbackList.First());
			}
			
			InputControl *FindControl(const char *name) const;
			
			void ResetActions(void) const;
			
			virtual void SetInputMode(InputMode mode) = 0;
			virtual bool ProcessEvents(InputMode mode);
	};
	
	
	#if C4WINDOWS
	
		class C4_API  DirectInputDevice : public InputDevice
		{
			protected:
				
				IDirectInputDevice8A		*deviceInstance;
				
				int32						controlCount;
				unsigned_int32				controlTableSize;
				InputControl				**controlTable;
				
				DirectInputDevice(InputDeviceType type, IDirectInput8A *directInput, const DIDEVICEINSTANCEA *instance);
				
				#if !C4SERVER
				
					static BOOL CALLBACK EnumObjectsCallback(const DIDEVICEOBJECTINSTANCEA *instance, void *cookie);
					
					void BuildDataFormat(void);
				
				#endif
				
				void Activate(void);
				void Deactivate(void);
			
			public:
				
				~DirectInputDevice();
				
				virtual bool ProcessEvents(InputMode mode);
		};
		
		typedef DirectInputDevice StandardInputDevice;
	
	#else
	
		typedef InputDevice StandardInputDevice;
	
	#endif
	
	
	class C4_API MouseDevice : public StandardInputDevice
	{
		private:
			
			#if C4WINDOWS
			
				void Activate(void);
			
			#elif C4LINUX
			
				Integer2D				originalPosition;
				Integer2D				currentPosition;
				
				GenericButtonControl	leftButton;
				GenericButtonControl	middleButton;
				GenericButtonControl	rightButton;
				
				DeltaAxisControl		horizontalAxis;
				DeltaAxisControl		verticalAxis;
				DeltaAxisControl		wheelAxis;
				
				void Activate(void);
				void Deactivate(void);
			
			#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

			#endif //]
		
		public:
			
			#if C4WINDOWS
			
				MouseDevice(IDirectInput8A *directInput, const DIDEVICEINSTANCEA *instance);
			
			#elif C4MACOS
			
				MouseDevice(io_object_t object, CFMutableDictionaryRef properties);
			
			#elif C4LINUX
			
				MouseDevice();
				
				bool ProcessEvents(InputMode mode);
				
				void HandleMouseButtonEvent(InputMode mode, const XButtonEvent *event);
				void HandleMouseMotionEvent(InputMode mode, const XMotionEvent *event);
			
			#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

			#endif //]
			
			~MouseDevice();
			
			bool ProcessEvents(InputMode mode);
			void SetInputMode(InputMode mode);
	};
	
	
	class C4_API KeyboardDevice : public StandardInputDevice
	{
		private:
			
			#if C4LINUX
			
				KeyButtonControl	*keyButton[kKeyboardRawCodeCount];
			
			#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

			#endif //]
		
		public:
			
			#if C4WINDOWS
			
				KeyboardDevice(IDirectInput8A *directInput, const DIDEVICEINSTANCEA *instance);
			
			#elif C4MACOS
			
				KeyboardDevice(io_object_t object, CFMutableDictionaryRef properties);
			
			#elif C4LINUX
			
				KeyboardDevice();
				
				void HandleKeyboardEvent(InputMode mode, const XKeyEvent *event);
			
			#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

			#endif //]
			
			~KeyboardDevice();
			
			void SetInputMode(InputMode mode);
	};
	
	
	#if !C4GAMECONSOLE
	
		class C4_API JoystickDevice : public StandardInputDevice
		{
			private:
				
				#if C4WINDOWS
				
					static BOOL CALLBACK EnumEffectsCallback(const DIEFFECTINFOA *effect, void *cookie);
				
				#endif
				
			public:
				
				#if C4WINDOWS
				
					JoystickDevice(IDirectInput8A *directInput, const DIDEVICEINSTANCEA *instance);
					
					bool ProcessEvents(InputMode mode);
				
				#elif C4MACOS
				
					JoystickDevice(io_object_t object, CFMutableDictionaryRef properties);
				
				#endif
				
				~JoystickDevice();
				
				void SetInputMode(InputMode mode);
		};
	
	#endif
	
	
	#if C4XINPUT
	
		class XboxDevice : public InputDevice
		{
			private:
				
				int32					deviceIndex;
				unsigned_int32			packetNumber;
				
				bool					buttonState[kXboxButtonCount];
				unsigned_int8			triggerState[kXboxTriggerCount];
				int16					axisState[kXboxAxisCount];
				
				GenericButtonControl	*buttonControl[kXboxButtonCount];
				SliderControl			*triggerControl[kXboxTriggerCount];
				LinearAxisControl		*axisControl[kXboxAxisCount];
				
				char					controlStorage[sizeof(GenericButtonControl) * kXboxButtonCount + sizeof(SliderControl) * kXboxTriggerCount + sizeof(LinearAxisControl) * kXboxAxisCount];
			
			public:
				
				XboxDevice(int32 index);
				~XboxDevice();
				
				int32 GetDeviceIndex(void) const
				{
					return (deviceIndex);
				}
				
				void SetInputMode(InputMode mode);
				bool ProcessEvents(InputMode mode);
		};
	
	#endif
	
	
	#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#endif //]
	
	
	//# \class	InputMgr	The Input Manager class.
	//
	//# \def	class InputMgr : public Manager<InputMgr>
	//
	//# \desc
	//# The $InputMgr$ class encapsulates the input device functionality of the C4 Engine.
	//# The single instance of the Input Manager is constructed during an application's initialization
	//# and destroyed at termination.
	//# 
	//# The Input Manager's member functions are accessed through the global pointer $TheInputMgr$.
	//
	//# \also	$@InputDevice@$
	//# \also	$@InputControl@$
	//# \also	$@Action@$
	
	
	//# \function	InputMgr::GetFirstDevice	Returns the first input device in the Input Manager's device list.
	//
	//# \proto	InputDevice *GetFirstDevice(void) const;
	//
	//# \desc
	//
	//# \also	$@InputMgr::FindDevice@$
	
	
	//# \function	InputMgr::FindDevice		Returns the input device having a given name or type.
	//
	//# \proto	InputDevice *FindDevice(const char *name) const;
	//# \proto	InputDevice *FindDevice(InputDeviceType type) const;
	//
	//# \param	name	The name of the device to find.
	//# \param	type	The type of the device to find.
	//
	//# \desc
	//
	//# \also	$@InputMgr::GetFirstDevice@$
	
	
	//# \function	InputMgr::AddAction			Registers an action object with the Input Manager.
	//
	//# \proto	void AddAction(Action *action);
	//
	//# \param	action		The action object to register.
	//
	//# \desc
	//# The $AddAction$ function registers an $@Action@$ object with the Input Manager. An action is
	//# normally registered by an application at the time that it is initialized.
	//
	//# \also	$@InputMgr::RemoveAction@$
	//# \also	$@Action@$
	//# \also	$@InputControl@$
	
	
	//# \function	InputMgr::RemoveAction		Unregisters an action object with the Input Manager.
	//
	//# \proto	void RemoveAction(Action *action);
	//
	//# \param	action		The action object to unregister.
	//
	//# \desc
	//# The $RemoveAction$ function unregisters an $@Action@$ object that was previously registered
	//# with the Input Manager. When an action is unregistered, any input controls to which the action
	//# was assigned have their actions reset to $nullptr$.
	//#
	//# An action is automatically unregistered if it is destroyed.
	//
	//# \also	$@InputMgr::AddAction@$
	//# \also	$@Action@$
	//# \also	$@InputControl@$
	
	
	//# \function	InputMgr::SetInputMode		Sets the current mode in which the Input Manager processes input events.
	//
	//# \proto	void SetInputMode(InputMode mode);
	//
	//# \param	mode	The new input mode.
	//
	//# \desc
	//# The $SetInputMode$ function sets the current mode in which the Input Manager processes input events
	//# from various types of input devices. The $mode$ parameter can be a combination (through logical OR)
	//# of the following values.
	//
	//# \table	InputMode
	//
	//# If the new input mode specifies that events are to be processed by the Input Manager for a particular
	//# type of device, then events for that type of device are sent to input actions and are not captured by
	//# the Interface Manager.
	//#
	//# The default input mode is $kInputInactive$ when the Input Manager is initialized.
	//
	//# \also	$@InterfaceMgr/InterfaceMgr::SetInputManagementMode@$
	
	
	//# \div
	//# \function	InputMgr::GetMouseSensitivity		Returns the global mouse sensitivity.
	//
	//# \proto	int32 GetMouseSensitivity(void) const;
	//
	//# \desc
	//# The $GetMouseSensitivity$ function returns the global mouse sensitivity. This value is used as a multiplier
	//# for the raw mouse movement, and it is applied to the values returned by the $@InputMgr::GetMouseDeltaX@$
	//# and $@InputMgr::GetMouseDeltaY@$ functions. The default value of the sensitivity is 25.
	//
	//# \also	$@InputMgr::SetMouseSensitivity@$
	//# \also	$@InputMgr::GetMouseFlags@$
	//# \also	$@InputMgr::SetMouseFlags@$
	//# \also	$@InputMgr::GetMouseDeltaX@$
	//# \also	$@InputMgr::GetMouseDeltaY@$
	
	
	//# \function	InputMgr::SetMouseSensitivity		Sets the global mouse sensitivity.
	//
	//# \proto	void SetMouseSensitivity(int32 sensitivity);
	//
	//# \param	sensitivity		The new mouse sensitivity.
	//
	//# \desc
	//# The $SetMouseSensitivity$ function sets the global mouse sensitivity. This value is used as a multiplier
	//# for the raw mouse movement, and it is applied to the values returned by the $@InputMgr::GetMouseDeltaX@$
	//# and $@InputMgr::GetMouseDeltaY@$ functions. The default value of the sensitivity is 25.
	//#
	//# To change the value of the mouse sensitivity persistently over multiple runs of the engine, change the
	//# value of the $sensitivity$ system variable as follows.
	//
	//# \code	TheEngine->GetVariable("sensitivity")->SetIntegerValue(sensitivity);
	//
	//# \also	$@InputMgr::GetMouseSensitivity@$
	//# \also	$@InputMgr::GetMouseFlags@$
	//# \also	$@InputMgr::SetMouseFlags@$
	//# \also	$@InputMgr::GetMouseDeltaX@$
	//# \also	$@InputMgr::GetMouseDeltaY@$
	
	
	//# \function	InputMgr::GetMouseFlags		Returns the mouse input flags.
	//
	//# \proto	unsigned_int32 GetMouseFlags(void) const;
	//
	//# \desc
	//# The $GetMouseFlags$ function returns the mouse input flags, which can be a combination (through logical OR)
	//# of the following values.
	//
	//# \table	MouseFlags
	//
	//# The default value of the mouse flags is $kMouseSmooth$.
	//
	//# \also	$@InputMgr::SetMouseFlags@$
	//# \also	$@InputMgr::GetMouseSensitivity@$
	//# \also	$@InputMgr::SetMouseSensitivity@$
	//# \also	$@InputMgr::GetMouseDeltaX@$
	//# \also	$@InputMgr::GetMouseDeltaY@$
	
	
	//# \function	InputMgr::SetMouseFlags		Sets the mouse input flags.
	//
	//# \proto	void SetMouseFlags(unsigned_int32 flags);
	//
	//# \param	flags	The new mouse flags.
	//
	//# \desc
	//# The $SetMouseFlags$ function sets the mouse input flags to the value specified by the $flags$ parameter,
	//# which can be a combination (through logical OR) of the following values.
	//
	//# \table	MouseFlags
	//
	//# The default value of the mouse flags is $kMouseSmooth$.
	//#
	//# To change the value of the mouse flags persistently over multiple runs of the engine, change the
	//# values of the $invertMouse$ and $smoothMouse$ system variables as follows.
	//
	//# \code	TheEngine->GetVariable("invertMouse")->SetIntegerValue(invert);
	//# \code	TheEngine->GetVariable("smoothMouse")->SetIntegerValue(smooth);
	//
	//# \also	$@InputMgr::GetMouseFlags@$
	//# \also	$@InputMgr::GetMouseSensitivity@$
	//# \also	$@InputMgr::SetMouseSensitivity@$
	//# \also	$@InputMgr::GetMouseDeltaX@$
	//# \also	$@InputMgr::GetMouseDeltaY@$
	
	
	//# \function	InputMgr::GetMouseDeltaX		Returns the mouse movement delta on the <i>x</i> axis.
	//
	//# \proto	float GetMouseDeltaX(void) const;
	//
	//# \desc
	//# The $GetMouseDeltaX$ function returns the mouse movement delta on the <i>x</i> axis. The delta value is
	//# expressed in generic units that are scaled to be equivalent across all mouse devices and then multiplied
	//# by the current mouse sensitivity.
	//#
	//# The mouse movement delta is updated by the Input Manager only when the current input mode includes
	//# the $kInputMouseActive$ flag.
	//
	//# \also	$@InputMgr::GetMouseDeltaY@$
	//# \also	$@InputMgr::GetMouseSensitivity@$
	//# \also	$@InputMgr::SetMouseSensitivity@$
	//# \also	$@InputMgr::GetMouseFlags@$
	//# \also	$@InputMgr::SetMouseFlags@$
	
	
	//# \function	InputMgr::GetMouseDeltaY		Returns the mouse movement delta on the <i>y</i> axis.
	//
	//# \proto	float GetMouseDeltaY(void) const;
	//
	//# \desc
	//# The $GetMouseDeltaY$ function returns the mouse movement delta on the <i>y</i> axis. The delta value is
	//# expressed in generic units that are scaled to be equivalent across all mouse devices and then multiplied
	//# by the current mouse sensitivity. If the $kMouseInverted$ mouse flag is currently set, then the delta value
	//# is negated before it is returned.
	//#
	//# The mouse movement delta is updated by the Input Manager only when the current input mode includes
	//# the $kInputMouseActive$ flag.
	//
	//# \also	$@InputMgr::GetMouseDeltaX@$
	//# \also	$@InputMgr::GetMouseSensitivity@$
	//# \also	$@InputMgr::SetMouseSensitivity@$
	//# \also	$@InputMgr::GetMouseFlags@$
	//# \also	$@InputMgr::SetMouseFlags@$
	
	
	class C4_API InputMgr : public Manager<InputMgr>
	{
		friend class MouseAction;
		
		#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

		#endif //]
		
		public:
			
			typedef void KeyProc(void *);
			typedef void ConfigProc(InputControl *, float, void *);
		
		private:
			
			#if C4WINDOWS
			
				enum
				{
					kMaxXinputDeviceCount	= 16
				};
				
				LPDIRECTINPUT8A				directInput;
				
				int32						xinputDeviceCount;
				unsigned_int32				xinputDeviceGuid[kMaxXinputDeviceCount];
				
				#if !C4SERVER
				
					static const wchar_t *FindDeviceSubstring(const wchar_t *string, const wchar_t *substring);
					static unsigned_int32 ReadDeviceHexString(const wchar_t *string);
					
					static BOOL CALLBACK EnumDevicesCallback(const DIDEVICEINSTANCEA *instance, void *cookie);
				
				#endif
			
			#endif
			
			static InputMode			inputMode;
			static InputMode				internalInputMode;
			
			#if C4LINUX || C4PLAYSTATION3
			
				MouseDevice					*mouseDevice;
				KeyboardDevice				*keyboardDevice;
			
			#endif
			
			List<InputDevice>				deviceList;
			List<Action>					actionList;
			
			ConsoleAction					consoleAction;
			EscapeAction					escapeAction;
			MouseAction						mouseXAction;
			MouseAction						mouseYAction;
			
			KeyProc							*consoleProc;
			void							*consoleCookie;
			
			KeyProc							*escapeProc;
			void							*escapeCookie;
			
			ConfigProc						*configProc;
			void							*configCookie;
			
			#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

			#endif //]
			
			int32							mouseSensitivity;
			unsigned_int32					mouseFlags;
	public:
			float							deltaXMultiplier;
			float							deltaYMultiplier;
			
			float							prevMouseDeltaX;
			float							prevMouseDeltaY;
			
			float							mouseDeltaX;
			float							mouseDeltaY;
	private:
			VariableObserver<InputMgr>		sensitivityObserver;
			VariableObserver<InputMgr>		invertMouseObserver;
			VariableObserver<InputMgr>		smoothMouseObserver;
			
			void HandleSensitivityEvent(Variable *variable);
			void HandleInvertMouseEvent(Variable *variable);
			void HandleSmoothMouseEvent(Variable *variable);
			
			void UpdateLog(void) const;
			
		public:
			
			InputMgr(int);
			~InputMgr();
			
			EngineResult Construct(void);
			void Destruct(void);
			
			static InputMode GetInputMode(void)
			{
				return (inputMode);
			}
			
			static InputMode GetInternalInputMode(void)
			{
				return (internalInputMode);
			}
			
			InputDevice *GetFirstDevice(void) const
			{
				return (deviceList.First());
			}
			
			Action *GetFirstAction(void) const
			{
				return (actionList.First());
			}
			
			void AddAction(Action *action)
			{
				actionList.Append(action);
			}
			
			void RemoveAction(Action *action)
			{
				action->BreakAllLinks();
				actionList.Remove(action);
			}
			
			KeyProc *GetConsoleProc(void) const
			{
				return (consoleProc);
			}
			
			void *GetConsoleCookie(void) const
			{
				return (consoleCookie);
			}
			
			void SetConsoleProc(KeyProc *proc, void *cookie = nullptr)
			{
				consoleProc = proc;
				consoleCookie = cookie;
			}
			
			KeyProc *GetEscapeProc(void) const
			{
				return (escapeProc);
			}
			
			void *GetEscapeCookie(void) const
			{
				return (escapeCookie);
			}
			
			void SetEscapeProc(KeyProc *proc, void *cookie = nullptr)
			{
				escapeProc = proc;
				escapeCookie = cookie;
			}
			
			void SetConfigProc(ConfigProc *proc, void *cookie = nullptr)
			{
				configProc = proc;
				configCookie = cookie;
			}
			
			void CallConfigProc(InputControl *control, float value)
			{
				if (configProc) (*configProc)(control, value, configCookie);
			}
			
			int32 GetMouseSensitivity(void) const
			{
				return (mouseSensitivity);
			}
			
			unsigned_int32 GetMouseFlags(void) const
			{
				return (mouseFlags);
			}
			
			float GetMouseDeltaX(void) const
			{
				return (mouseDeltaX * deltaXMultiplier);
			}
			
			float GetMouseDeltaY(void) const
			{
				return (mouseDeltaY * deltaYMultiplier);
			}
			
			#if C4LINUX
			
				void HandleMouseButtonEvent(const XButtonEvent *event)
				{
					mouseDevice->HandleMouseButtonEvent(inputMode, event);
				}
				
				void HandleMouseMotionEvent(const XMotionEvent *event)
				{
					mouseDevice->HandleMouseMotionEvent(inputMode, event);
				}
				
				void HandleKeyboardEvent(const XKeyEvent *event)
				{
					keyboardDevice->HandleKeyboardEvent(inputMode, event);
				}
			
			#endif
			
			InputDevice *FindDevice(const char *name) const;
			InputDevice *FindDevice(InputDeviceType type) const;
			Action *FindAction(ActionType type) const;
			
			void SetInputMode(InputMode mode);
			void SetMouseSensitivity(int32 sensitivity);
			void SetMouseFlags(unsigned_int32 flags);
			
			InputControl *GetActionControl(const Action *action, int32 index = 0);
			void ClearAllControlActions(void);
			void ResetAllActions(void);
			
			void InputTask(void);
	};
	
	
	C4_API extern InputMgr *TheInputMgr;
}


#endif

// ZYURVUR
