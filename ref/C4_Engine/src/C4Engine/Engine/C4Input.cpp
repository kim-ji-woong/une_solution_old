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


#include "C4Input.h"
#include "C4Engine.h"

#if C4XINPUT && C4FASTBUILD

	#include "C4XInput.h"

#endif

#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

#endif //]


using namespace C4;


namespace
{
	enum
	{
		kInputQueueSize		= 1024
	};
	
	
	#if C4XINPUT
	
		const char *xboxButtonName[kXboxButtonCount] =
		{
			"Up", "Down", "Left", "Right", "Start", "Back", "Left Thumb", "Right Thumb", "Left Shoulder", "Right Shoulder", "A", "B", "X", "Y"
		};
		
		const char *xboxTriggerName[kXboxTriggerCount] =
		{
			"Left Trigger", "Right Trigger"
		};
		
		const char *xboxAxisName[kXboxAxisCount] =
		{
			"Left Thumb X Axis", "Left Thumb Y Axis", "Right Thumb X Axis", "Right Thumb Y Axis"
		};
	
	#elif C4MACOS
	
		const char *keyButtonName[0xE8] =
		{
			"0x00", "0x01", "0x02", "0x03", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L",
			"M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "1", "2",
			"3", "4", "5", "6", "7", "8", "9", "0", "Return", "Escape", "Delete", "Tab", "Space", "-", "=", "[",
			"]", "\\", "Pound", ";", "'", "`", ",", ".", "/", "Caps Lock", "F1", "F2", "F3", "F4", "F5", "F6",
			"F7", "F8", "F9", "F10", "F11", "F12", "Print", "Scroll Lock", "Pause", "Help", "Home", "Page Up", "Del", "End", "Page Down", "Right",
			"Left", "Down", "Up", "Clear", "Pad /", "Pad *", "Pad -", "Pad +", "Enter", "Pad 1", "Pad 2", "Pad 3", "Pad 4", "Pad 5", "Pad 6", "Pad 7",
			"Pad 8", "Pad 9", "Pad 0", "Pad .", "Slash", "Appl", "Power", "Pad =", "F13", "F14", "F15", "F16", "F17", "F18", "F19", "F20",
			"F21", "F22", "F23", "F24", "Exec", "Help", "Menu", "Select", "Stop", "Again", "Undo", "Cut", "Copy", "Paste", "Find", "Mute",
			"Vol Up", "Vol Down", "Caps-Lock", "Num-Lock", "Scroll-Lock", "Pad ,", "Pad =", "Inter1", "Inter2", "Inter3", "Inter4", "Inter5", "Inter6", "Inter7", "Inter8", "Inter9",
			"Lang1", "Lang2", "Lang3", "Lang4", "Lang5", "Lang6", "Lang7", "Lang8", "Lang9", "Erase", "SysReq", "Cancel", "Clear", "Prior", "Ret", "Separator",
			"Out", "Oper", "Clear/Again", "CrSel", "ExCel", "0xA5", "0xA6", "0xA7", "0xA8", "0xA9", "0xAA", "0xAB", "0xAC", "0xAD", "0xAE", "0xAF",
			"0xB0", "0xB1", "0xB2", "0xB3", "0xB4", "0xB5", "0xB6", "0xB7", "0xB8", "0xB9", "0xBA", "0xBB", "0xBC", "0xBD", "0xBE", "0xBF",
			"0xC0", "0xC1", "0xC2", "0xC3", "0xC4", "0xC5", "0xC6", "0xC7", "0xC8", "0xC9", "0xCA", "0xCB", "0xCC", "0xCD", "0xCE", "0xCF",
			"0xD0", "0xD1", "0xD2", "0xD3", "0xD4", "0xD5", "0xD6", "0xD7", "0xD8", "0xD9", "0xDA", "0xDB", "0xDC", "0xDD", "0xDE", "0xDF",
			"Left Control", "Left Shift", "Left Option", "Left Command", "Right Control", "Right Shift", "Right Option", "Right Command"
		};
		
		const char *linearAxisName[3] =
		{
			"X-Axis ", "Y-Axis ", "Z-Axis "
		};
		
		const char *rotationAxisName[3] =
		{
			"RX-Axis ", "RY-Axis ", "RZ-Axis "
		};
		
		const char *deltaAxisName[3] =
		{
			"X-Delta ", "Y-Delta ", "Z-Delta "
		};
	
	#elif C4LINUX
	
		const char *keySymbolName[0x0101] =
		{
			"", "", "", "", "", "", "", "", "Backspace", "Tab", "", "Clear", "", "Enter", "", "",
			"", "", "", "Pause", "Scroll Lock", "Sys Req", "", "", "", "", "", "Escape", "", "", "", "",
			"", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
			"", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
			"", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
			"Home", "Left", "Up", "Right", "Down", "Page Up", "Page Down", "End", "Home", "", "", "", "", "", "", "",
			"Select", "Print", "Exec", "Insert", "", "Undo", "Redo", "Menu", "Find", "Cancel", "Help", "Break", "", "", "", "",
			"", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "",
			"Pad Space", "", "", "", "", "", "", "", "", "Pad Tab", "", "", "", "Pad Enter", "", "",
			"", "Pad F1", "Pad F2", "Pad F3", "Pad F4", "Pad 7", "Pad 4", "Pad 8", "Pad 6", "Pad 2", "Pad 9", "Pad 3", "Pad 1", "Pad 5", "Pad 0", "Pad .",
			"", "", "", "", "", "", "", "", "", "", "Pad *", "Pad +", "Pad ,", "Pad -", "Pad .", "Pad /",
			"Pad 0", "Pad 1", "Pad 2", "Pad 3", "Pad 4", "Pad 5", "Pad 6", "Pad 7", "Pad 8", "Pad 9", "", "", "", "Pad =", "F1", "F2",
			"F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "F13", "F14", "F15", "F16", "F17", "F18",
			"F19", "F20", "F21", "F22", "F23", "F24", "F25", "F26", "F27", "F28", "F29", "F30", "F31", "F32", "F33", "F34",
			"F35", "Left Shift", "Right Shift", "Left Control", "Right Control", "Caps Lock", "Shift Lock", "Left Meta", "Right Meta", "Left Alt", "Right Alt", "Left Super", "Right Super", "Left Hyper", "Right Hyper", "",
			"", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "Delete", "Space"
		};
	 
	#elif C4PLAYSTATION3 //[ PS3
 
			// -- PlayStation 3 code hidden -- 
 
	#endif //]
} 


InputMgr *C4::TheInputMgr = nullptr;
 

namespace C4
{
	template <> InputMgr Manager<InputMgr>::managerObject(0); 
	template <> InputMgr **Manager<InputMgr>::managerPointer = &TheInputMgr;
	
	template <> Heap Memory<InputDevice>::heap("InputDevice", 8192, kHeapMutexless);
	template class Memory<InputDevice>;
}


InputMode InputMgr::inputMode = kInputInactive;
InputMode InputMgr::internalInputMode = kInputInactive;


Action::Action(ActionType type)
{
	actionType = type;
	actionFlags = 0;
	activeCount = 0;
}

Action::~Action()
{
}

void Action::Begin(void)
{
}

void Action::End(void)
{
}

void Action::Move(int32 value)
{
}

void Action::Update(float value)
{
}


ConsoleAction::ConsoleAction() : Action(kActionConsole)
{
	SetActionFlags(kActionImmutable | kActionPersistent);
}

ConsoleAction::~ConsoleAction()
{
}

void ConsoleAction::Begin(void)
{
	InputMgr::KeyProc *proc = TheInputMgr->GetConsoleProc();
	if (proc) (*proc)(TheInputMgr->GetConsoleCookie());
}


EscapeAction::EscapeAction() : Action(kActionEscape)
{
	SetActionFlags(kActionImmutable | kActionPersistent);
}

EscapeAction::~EscapeAction()
{
}

void EscapeAction::Begin(void)
{
	InputMgr::KeyProc *proc = TheInputMgr->GetEscapeProc();
	if (proc) (*proc)(TheInputMgr->GetEscapeCookie());
}


MouseAction::MouseAction(ActionType type) : Action(type)
{
	SetActionFlags(kActionImmutable | kActionPersistent);
}

MouseAction::~MouseAction()
{
}

void MouseAction::Update(float value)
{
	if (GetActionType() == kActionMouseX) TheInputMgr->mouseDeltaX -= value;
	else TheInputMgr->mouseDeltaY += value;
}


CommandAction::CommandAction(const char *cmd) : Action(0)
{
	command = cmd;
	SetActionFlags(kActionPersistent);
}

CommandAction::~CommandAction()
{
}

void CommandAction::Begin(void)
{
	TheEngine->ExecuteText(command);
}


#if C4WINDOWS

	InputControl::InputControl(InputControlType type, InputDevice *device, const DIDEVICEOBJECTINSTANCEA *instance)
	{
		controlType = type;
		
		owningDevice = device;
		controlName = instance->tszName;
		
		dataGuid = instance->guidType;
		dataType = instance->dwType;
		dataFlags = instance->dwFlags;
	}

#endif

InputControl::InputControl(InputControlType type, InputDevice *device, const char *name)
{
	controlType = type;
	
	owningDevice = device;
	controlName = name;
	
	#if C4MACOS
	
		controlCookie = 0;
		controlActive = false;
	
	#endif
}

InputControl::InputControl(InputDevice *device)
{
	controlType = kInputGroup;
	
	owningDevice = device;
	controlName[0] = 0;
	
	#if C4MACOS
	
		controlCookie = 0;
		controlActive = false;
	
	#endif
}

InputControl::~InputControl()
{
}

void InputControl::SetControlAction(Action *action)
{
	Action *prevAction = controlAction;
	if ((prevAction) && (prevAction->GetActionType() == 0)) delete prevAction;
	
	controlAction = action;
}

#if C4MACOS

	void InputControl::Activate(IOHIDQueueInterface **deviceQueue)
	{
		if ((!controlActive) && (controlType != kInputGroup))
		{
			controlActive = true;
			
			#if !C4SERVER
			
				(**deviceQueue).addElement(deviceQueue, controlCookie, 0);
			
			#endif
		}
	}
	
	void InputControl::Deactivate(IOHIDQueueInterface **deviceQueue)
	{
		if (controlActive)
		{
			controlActive = false;
			
			#if !C4SERVER
			
				(**deviceQueue).removeElement(deviceQueue, controlCookie);
			
			#endif
		}
	}

#endif

void InputControl::HandleNormalEvent(int32 value)
{
}

bool InputControl::HandleConfigEvent(int32 value)
{
	return (false);
}


#if C4WINDOWS

	ButtonControl::ButtonControl(InputControlType type, InputDevice *device, const DIDEVICEOBJECTINSTANCEA *instance) : InputControl(type, device, instance)
	{
		dataMask = 0x80;
	}

#endif

ButtonControl::ButtonControl(InputControlType type, InputDevice *device, const char *name) : InputControl(type, device, name)
{
	dataMask = 0xFFFFFFFF;
}

ButtonControl::~ButtonControl()
{
}

void ButtonControl::HandleNormalEvent(int32 value)
{
	Action *action = GetControlAction();
	if (action)
	{
		if ((value & dataMask) != 0)
		{
			SetActiveAction(action);
			int32 k = action->GetActiveCount();
			action->SetActiveCount(k + 1);
			if (k == 0) action->Begin();
		}
		else
		{
			SetActiveAction(nullptr);
			int32 k = action->GetActiveCount();
			action->SetActiveCount(MaxZero(k - 1));
			if (k == 1) action->End();
		}
	}
}

bool ButtonControl::HandleConfigEvent(int32 value)
{
	if ((value & dataMask) != 0) 
	{
		TheInputMgr->CallConfigProc(this, 1.0F);
		return (true);
	}
	
	return (false);
}


#if C4WINDOWS

	KeyButtonControl::KeyButtonControl(InputDevice *device, const DIDEVICEOBJECTINSTANCEA *instance) : ButtonControl(kInputKeyButton, device, instance)
	{
	}

#endif

KeyButtonControl::KeyButtonControl(InputDevice *device, const char *name) : ButtonControl(kInputKeyButton, device, name)
{
}

KeyButtonControl::~KeyButtonControl()
{
}


#if C4WINDOWS

	GenericButtonControl::GenericButtonControl(InputDevice *device, const DIDEVICEOBJECTINSTANCEA *instance) : ButtonControl(kInputGenericButton, device, instance)
	{
	}

#endif

GenericButtonControl::GenericButtonControl(InputDevice *device, const char *name) : ButtonControl(kInputGenericButton, device, name)
{
}

GenericButtonControl::~GenericButtonControl()
{
}


#if C4WINDOWS

	AxisControl::AxisControl(InputControlType type, InputDevice *device, InputAxis axis, const DIDEVICEOBJECTINSTANCEA *instance) : InputControl(type, device, instance)
	{
		controlAxis = axis;
	}

#endif

AxisControl::AxisControl(InputControlType type, InputDevice *device, InputAxis axis, const char *name) : InputControl(type, device, name)
{
	controlAxis = axis;
}

AxisControl::~AxisControl()
{
}

void AxisControl::SetRange(float vmin, float vmax, float dead)
{
	float center = (vmin + vmax) * 0.5F;
	centerValue = center;
	
	minValue = vmin - center;
	maxValue = vmax - center;
	deadZone = dead;
	normalizer = 1.0F / (maxValue - dead);
}

void AxisControl::HandleNormalEvent(int32 value)
{
	Action *action = GetControlAction();
	if (action)
	{
		float v = (float) value - centerValue;
		if (Fabs(v) <= deadZone) action->Update(0.0F);
		else if (v > 0.0F) action->Update((Fmin(v, maxValue) - deadZone) * normalizer);
		else action->Update((Fmax(v, minValue) + deadZone) * normalizer);
	}
}

bool AxisControl::HandleConfigEvent(int32 value)
{
	float v = (float) value - centerValue;
	if (Fabs(v) > deadZone)
	{
		if (v > 0.0F) v = (Fmin(v, maxValue) - deadZone) * normalizer;
		else v = (Fmax(v, minValue) + deadZone) * normalizer;
		
		TheInputMgr->CallConfigProc(this, v);
		return (true);
	}
	
	return (false);
}


#if C4WINDOWS

	LinearAxisControl::LinearAxisControl(InputDevice *device, InputAxis axis, const DIDEVICEOBJECTINSTANCEA *instance) : AxisControl(kInputLinearAxis, device, axis, instance)
	{
	}

#endif

LinearAxisControl::LinearAxisControl(InputDevice *device, InputAxis axis, const char *name) : AxisControl(kInputLinearAxis, device, axis, name)
{
}

LinearAxisControl::~LinearAxisControl()
{
}


#if C4WINDOWS

	RotationAxisControl::RotationAxisControl(InputDevice *device, InputAxis axis, const DIDEVICEOBJECTINSTANCEA *instance) : AxisControl(kInputRotationAxis, device, axis, instance)
	{
	}

#endif

RotationAxisControl::RotationAxisControl(InputDevice *device, InputAxis axis, const char *name) : AxisControl(kInputRotationAxis, device, axis, name)
{
}

RotationAxisControl::~RotationAxisControl()
{
}


#if C4WINDOWS

	DeltaAxisControl::DeltaAxisControl(InputDevice *device, InputAxis axis, const DIDEVICEOBJECTINSTANCEA *instance) : AxisControl(kInputDeltaAxis, device, axis, instance)
	{
	}

#endif

DeltaAxisControl::DeltaAxisControl(InputDevice *device, InputAxis axis, const char *name) : AxisControl(kInputDeltaAxis, device, axis, name)
{
}

DeltaAxisControl::~DeltaAxisControl()
{
}

void DeltaAxisControl::HandleNormalEvent(int32 value)
{
	Action *action = GetControlAction();
	if (action)
	{
		if (value != 0) action->Update((float) value);
	}
}

bool DeltaAxisControl::HandleConfigEvent(int32 value)
{
	if ((GetOwningDevice()->GetDeviceType() != kInputMouse) || (GetControlAxis() == kInputAxisZ))
	{
		if (value != 0)
		{
			TheInputMgr->CallConfigProc(this, (float) value);
			return (true);
		}
	}
	
	return (false);
}


#if C4WINDOWS

	SliderControl::SliderControl(InputDevice *device, const DIDEVICEOBJECTINSTANCEA *instance) : InputControl(kInputSlider, device, instance)
	{
	}

#endif

SliderControl::SliderControl(InputDevice *device, const char *name) : InputControl(kInputSlider, device, name)
{
}

SliderControl::~SliderControl()
{
}

void SliderControl::SetRange(float vmin, float vmax, float thresh)
{
	maxValue = vmax;
	threshold = vmin + thresh;
	normalizer = 1.0F / (vmax - threshold);
}

void SliderControl::HandleNormalEvent(int32 value)
{
	Action *action = GetControlAction();
	if (action)
	{
		float v = (float) value;
		if (v <= threshold) action->Update(0.0F);
		else action->Update((Fmin(v, maxValue) - threshold) * normalizer);
	}
}

bool SliderControl::HandleConfigEvent(int32 value)
{
	float v = (float) value;
	if (v > threshold)
	{
		TheInputMgr->CallConfigProc(this, (Fmin(v, maxValue) - threshold) * normalizer);
		return (true);
	}
	
	return (false);
}


#if C4WINDOWS

	DirectionalControl::DirectionalControl(InputDevice *device, const DIDEVICEOBJECTINSTANCEA *instance) : InputControl(kInputDirectional, device, instance)
	{
		divider = 4500;
	}

#endif

DirectionalControl::DirectionalControl(InputDevice *device, const char *name) : InputControl(kInputDirectional, device, name)
{
	divider = 1;
}

DirectionalControl::~DirectionalControl()
{
}

void DirectionalControl::HandleNormalEvent(int32 value)
{
	Action *action = GetControlAction();
	if (action)
	{
		if ((value & 0xFFFF) == 0xFFFF) action->Move(-1);
		else action->Move(value / divider);
	}
}

bool DirectionalControl::HandleConfigEvent(int32 value)
{
	if ((value & 0xFFFF) != 0xFFFF)
	{
		TheInputMgr->CallConfigProc(this, (float) (value / divider));
		return (true);
	}
	
	return (false);
}


InputFeedback::InputFeedback(const char *name)
{
	feedbackName = name;
}

InputFeedback::~InputFeedback()
{
}


#if C4XINPUT || C4WINDOWS

	InputDevice::InputDevice(InputDeviceType type, const char *name) : controlTree(this)
	{
		deviceType = type;
		deviceName = name;
		deviceActive = false;
	}
	
	InputDevice::~InputDevice()
	{
	}

#elif C4MACOS

	InputDevice::InputDevice(InputDeviceType type, io_object_t object, CFMutableDictionaryRef properties) : controlTree(this)
	{
		deviceType = type;
		deviceActive = false;
		
		pluginInterface = nullptr;
		deviceInterface = nullptr;
		deviceQueue = nullptr;
		
		#if !C4SERVER
		
			const void *productValue = CFDictionaryGetValue(properties, CFSTR(kIOHIDProductKey));
			if (productValue)
			{
				SInt32		score;
				
				CFStringGetCString((CFStringRef) productValue, deviceName, kMaxInputDeviceNameLength + 1, CFStringGetSystemEncoding());
				
				if (IOCreatePlugInInterfaceForService(object, kIOHIDDeviceUserClientTypeID, kIOCFPlugInInterfaceID, &pluginInterface, &score) == kIOReturnSuccess)
				{
					if ((**pluginInterface).QueryInterface(pluginInterface, CFUUIDGetUUIDBytes(kIOHIDDeviceInterfaceID), (void **) &deviceInterface) == S_OK)
					{
						(**deviceInterface).open(deviceInterface, 0);
						deviceQueue = (**deviceInterface).allocQueue(deviceInterface);
						if ((deviceQueue) && ((**deviceQueue).create(deviceQueue, 0, kInputQueueSize) == kIOReturnSuccess))
						{
							BuildControlTree(&controlTree, properties);
						}
					}
				}
			}
		
		#endif
	}
	
	InputDevice::~InputDevice()
	{
		#if !C4SERVER
		
			if (deviceQueue)
			{
				(**deviceQueue).dispose(deviceQueue);
				(**deviceQueue).Release(deviceQueue);
			}
			
			if (deviceInterface)
			{
				(**deviceInterface).close(deviceInterface);
				(**deviceInterface).Release(deviceInterface);
			}
			
			if (pluginInterface) IODestroyPlugInInterface(pluginInterface);
		
		#endif
	}
	
	void InputDevice::BuildControlTree(InputControl *root, CFMutableDictionaryRef dictionary)
	{
		#if !C4SERVER
		
			const void *elementValue = CFDictionaryGetValue(dictionary, CFSTR(kIOHIDElementKey));
			if ((elementValue) && (CFGetTypeID(elementValue) == CFArrayGetTypeID()))
			{
				CFArrayRef elementArray = (CFArrayRef) elementValue;
				
				CFIndex count = CFArrayGetCount(elementArray);
				for (CFIndex index = 0; index < count; index++)
				{
					const void *value = CFArrayGetValueAtIndex(elementArray, index);
					if ((value) && (CFGetTypeID(value) == CFDictionaryGetTypeID()))
					{
						CFMutableDictionaryRef properties = (CFMutableDictionaryRef) value;
						
						const void *elementTypeValue = CFDictionaryGetValue(properties, CFSTR(kIOHIDElementTypeKey));
						if (elementTypeValue)
						{
							int32	elementType;
							
							CFNumberGetValue((CFNumberRef) elementTypeValue, kCFNumberLongType, &elementType);
							switch (elementType)
							{
								case kIOHIDElementTypeCollection:
								{
									InputControl *group = new InputControl(this);
									root->AddSubnode(group);
									
									BuildControlTree(group, properties);
									break;
								}
								
								case kIOHIDElementTypeInput_Misc:
								case kIOHIDElementTypeInput_Button:
								case kIOHIDElementTypeInput_Axis:
								case kIOHIDElementTypeInput_ScanCodes:
								{
									const void *cookieValue = CFDictionaryGetValue(properties, CFSTR(kIOHIDElementCookieKey));
									if (cookieValue)
									{
										int32	cookie;
										
										CFNumberGetValue((CFNumberRef) cookieValue, kCFNumberLongType, &cookie);
										
										const void *usagePageValue = CFDictionaryGetValue(properties, CFSTR(kIOHIDElementUsagePageKey));
										const void *usageValue = CFDictionaryGetValue(properties, CFSTR(kIOHIDElementUsageKey));
										if ((usagePageValue) && (usageValue))
										{
											int32	usagePage;
											int32	usage;
											
											CFNumberGetValue((CFNumberRef) usagePageValue, kCFNumberLongType, &usagePage);
											CFNumberGetValue((CFNumberRef) usageValue, kCFNumberLongType, &usage);
											
											InputControl *control = nullptr;
											if (usagePage == kHIDPage_GenericDesktop)
											{
												switch (usage)
												{
													case kHIDUsage_GD_X:
													case kHIDUsage_GD_Y:
													case kHIDUsage_GD_Z:
													{
														int32 relative = false;
														const void *relativeValue = CFDictionaryGetValue(properties, CFSTR(kIOHIDElementIsRelativeKey));
														if (relativeValue) relative = CFBooleanGetValue((CFBooleanRef) relativeValue);
														
														InputAxis axis = static_cast<InputAxis>(usage - kHIDUsage_GD_X);
														
														if ((deviceType == kInputMouse) || (relative))
														{
															control = new DeltaAxisControl(this, axis, InputControlName(deltaAxisName[axis]) + cookie);
															
															if (usage == kHIDUsage_GD_X) control->SetControlAction(TheInputMgr->FindAction(kActionMouseX));
															else if (usage == kHIDUsage_GD_Y) control->SetControlAction(TheInputMgr->FindAction(kActionMouseY));
														}
														else
														{
															control = new LinearAxisControl(this, axis, InputControlName(linearAxisName[axis]) + cookie);
														}
														
														break;
													}
													
													case kHIDUsage_GD_Wheel:
														
														control = new DeltaAxisControl(this, kInputAxisZ, InputControlName(deltaAxisName[kInputAxisZ]) + cookie);
														break;
													
													case kHIDUsage_GD_Rx:
													case kHIDUsage_GD_Ry:
													case kHIDUsage_GD_Rz:
													{
														InputAxis axis = static_cast<InputAxis>(usage - kHIDUsage_GD_Rx);
														control = new RotationAxisControl(this, axis, InputControlName(rotationAxisName[axis]) + cookie);
														break;
													}
													
													case kHIDUsage_GD_Slider:
														
														control = new SliderControl(this, InputControlName("Slider ") + cookie);
														break;
													
													case kHIDUsage_GD_Hatswitch:
														
														control = new DirectionalControl(this, InputControlName("Directional ") + cookie);
														break;
												}
											}
											else if (usagePage == kHIDPage_KeyboardOrKeypad)
											{
												bool valid = ((unsigned_int32) (usage - 0xE0) < 8U);
												if (valid)
												{
													const void *arrayValue = CFDictionaryGetValue(properties, CFSTR(kIOHIDElementIsArrayKey));
													if ((arrayValue) && (CFBooleanGetValue((CFBooleanRef) arrayValue))) valid = false;
												}
												else
												{
													valid = ((unsigned_int32) (usage - 4) < 100U);
												}
												
												if (valid)
												{
													control = new KeyButtonControl(this, keyButtonName[usage]);
													
													if (usage == kHIDUsage_KeyboardGraveAccentAndTilde) control->SetControlAction(TheInputMgr->FindAction(kActionConsole));
													else if (usage == kHIDUsage_KeyboardEscape) control->SetControlAction(TheInputMgr->FindAction(kActionEscape));
												}
											}
											else if (usagePage == kHIDPage_Button)
											{
												control = new GenericButtonControl(this, InputControlName("Button ") += usage);
											}
											
											if (control)
											{
												control->controlCookie = (IOHIDElementCookie) cookie;
												
												InputControlType type = control->GetControlType();
												if ((type == kInputLinearAxis) || (type == kInputRotationAxis))
												{
													int32 imin = 0x80000000;
													int32 imax = 0x7FFFFFFF;
													
													const void *minValue = CFDictionaryGetValue(properties, CFSTR(kIOHIDElementMinKey));
													if (minValue) CFNumberGetValue((CFNumberRef) minValue, kCFNumberLongType, &imin);
													
													const void *maxValue = CFDictionaryGetValue(properties, CFSTR(kIOHIDElementMaxKey));
													if (maxValue) CFNumberGetValue((CFNumberRef) maxValue, kCFNumberLongType, &imax);
													
													float vmin = (float) imin;
													float vmax = (float) imax;
													static_cast<AxisControl *>(control)->SetRange(vmin, vmax, (vmax - vmin) * 0.25F);
												}
												else if (type == kInputSlider)
												{
													int32 imin = 0;
													int32 imax = 0x7FFFFFFF;
													
													const void *minValue = CFDictionaryGetValue(properties, CFSTR(kIOHIDElementMinKey));
													if (minValue) CFNumberGetValue((CFNumberRef) minValue, kCFNumberLongType, &imin);
													
													const void *maxValue = CFDictionaryGetValue(properties, CFSTR(kIOHIDElementMaxKey));
													if (maxValue) CFNumberGetValue((CFNumberRef) maxValue, kCFNumberLongType, &imax);
													
													float vmin = (float) imin;
													float vmax = (float) imax;
													static_cast<SliderControl *>(control)->SetRange(vmin, vmax, (vmax - vmin) * 0.0625F);
												}
												
												root->AddSubnode(control);
											}
										}
									}
									
									break;
								}
							}
						}
					}
				}
			}
		
		#endif
	}

#elif C4LINUX

	InputDevice::InputDevice(InputDeviceType type, const char *name) : controlTree(this)
	{
		deviceType = type;
		deviceName = name;
		deviceActive = false;
	}
	
	InputDevice::~InputDevice()
	{
	}

#elif C4PLAYSTATION3
	
	InputDevice::InputDevice(InputDeviceType type, unsigned_int32 number, const char *name) : controlTree(this)
	{
		deviceType = type;
		deviceActive = false;
		
		deviceNumber = number;
		deviceName = name;
	}
	
	InputDevice::~InputDevice()
	{
	}

#endif

InputControl *InputDevice::FindControl(const char *name) const
{
	InputControl *control = GetFirstControl();
	while (control)
	{
		if (Text::CompareTextCaseless(control->GetControlName(), name)) break;
		control = GetNextControl(control);
	}
	
	return (control);
}

void InputDevice::ResetActions(void) const
{
	InputControl *control = GetFirstControl();
	while (control)
	{
		Action *action = control->GetControlAction();
		if ((action) && (action->GetActiveCount() > 0))
		{
			action->SetActiveCount(0);
			action->End();
		}
		
		control = GetNextControl(control);
	}
}

void InputDevice::Activate(void)
{
	deviceActive = true;
	
	#if C4MACOS
	
		InputControl *control = GetFirstControl();
		while (control)
		{
			control->Activate(deviceQueue);
			control = GetNextControl(control);
		}
		
		#if !C4SERVER
		
			(**deviceQueue).start(deviceQueue);
		
		#endif
	
	#endif
}

void InputDevice::Deactivate(void)
{
	deviceActive = false;
	
	#if C4MACOS && !C4SERVER
	
		(**deviceQueue).stop(deviceQueue);
	
	#endif
	
	InputControl *control = GetFirstControl();
	while (control)
	{
		Action *action = control->activeAction;
		if (action)
		{
			control->activeAction = nullptr;
			int32 k = action->GetActiveCount();
			action->SetActiveCount(MaxZero(k - 1));
			if (k == 1) action->End();
		}
		
		#if C4MACOS
		
			control->Deactivate(deviceQueue);
		
		#endif
		
		control = GetNextControl(control);
	}
}

bool InputDevice::ProcessEvents(InputMode mode)
{
	#if C4MACOS && !C4SERVER
	
		static const AbsoluteTime zero = {0, 0};
		
		if (mode != kInputConfiguration)
		{
			for (;;)
			{
				IOHIDEventStruct	event;
				
				if ((**deviceQueue).getNextEvent(deviceQueue, &event, zero, 0) != kIOReturnSuccess) break;
				
				IOHIDElementCookie cookie = event.elementCookie;
				
				InputControl *control = GetFirstControl();
				while (control)
				{
					if (control->controlCookie == cookie)
					{
						control->HandleNormalEvent(event.value);
						break;
					}
					
					control = GetNextControl(control);
				}
			}
		}
		else
		{
			IOHIDEventStruct	event;
			
			if ((**deviceQueue).getNextEvent(deviceQueue, &event, zero, 0) == kIOReturnSuccess)
			{
				IOHIDElementCookie cookie = event.elementCookie;
				
				InputControl *control = GetFirstControl();
				while (control)
				{
					if (control->controlCookie == cookie)
					{
						if (control->HandleConfigEvent(event.value)) return (true);
						break;
					}
					
					control = GetNextControl(control);
				}
			}
		}
	
	#endif
	
	return (false);
}


#if C4WINDOWS

	DirectInputDevice::DirectInputDevice(InputDeviceType type, IDirectInput8A *directInput, const DIDEVICEINSTANCEA *instance) : InputDevice(type, instance->tszProductName)
	{
		
		
		controlCount = 0;
		controlTableSize = 0;
		controlTable = nullptr;

#if !C4SERVER

		directInput->CreateDevice(instance->guidInstance, &deviceInstance, nullptr);

#endif
	}
	
	DirectInputDevice::~DirectInputDevice()
	{
		delete[] controlTable;
		
		#if !C4SERVER
		
			deviceInstance->Release();
		
		#endif
	}
	
	#if !C4SERVER
	
		BOOL CALLBACK DirectInputDevice::EnumObjectsCallback(const DIDEVICEOBJECTINSTANCEA *instance, void *cookie)
		{
			DirectInputDevice *inputDevice = static_cast<DirectInputDevice *>(cookie);
			InputControl *controlTree = &inputDevice->controlTree;
			
			InputControl *control = nullptr;
			DWORD objectType = DIDFT_GETTYPE(instance->dwType);
			
			bool button = false;
			
			if (IsEqualGUID(instance->guidType, GUID_XAxis))
			{
				if ((objectType & DIDFT_RELAXIS) != 0)
				{
					control = new DeltaAxisControl(inputDevice, kInputAxisX, instance);
					if (inputDevice->GetDeviceType() == kInputMouse) control->SetControlAction(TheInputMgr->FindAction(kActionMouseX));
				}
				else
				{
					control = new LinearAxisControl(inputDevice, kInputAxisX, instance);
				}
			}
			else if (IsEqualGUID(instance->guidType, GUID_YAxis))
			{
				if ((objectType & DIDFT_RELAXIS) != 0)
				{
					control = new DeltaAxisControl(inputDevice, kInputAxisY, instance);
					if (inputDevice->GetDeviceType() == kInputMouse) control->SetControlAction(TheInputMgr->FindAction(kActionMouseY));
				}
				else
				{
					control = new LinearAxisControl(inputDevice, kInputAxisY, instance);
				}
			}
			else if (IsEqualGUID(instance->guidType, GUID_ZAxis))
			{
				if ((objectType & DIDFT_RELAXIS) != 0) control = new DeltaAxisControl(inputDevice, kInputAxisZ, instance);
				else control = new LinearAxisControl(inputDevice, kInputAxisZ, instance);
			}
			else if (IsEqualGUID(instance->guidType, GUID_RxAxis))
			{
				control = new RotationAxisControl(inputDevice, kInputAxisX, instance);
			}
			else if (IsEqualGUID(instance->guidType, GUID_RyAxis))
			{
				control = new RotationAxisControl(inputDevice, kInputAxisY, instance);
			}
			else if (IsEqualGUID(instance->guidType, GUID_RzAxis))
			{
				control = new RotationAxisControl(inputDevice, kInputAxisZ, instance);
			}
			else if (IsEqualGUID(instance->guidType, GUID_Slider))
			{
				control = new SliderControl(inputDevice, instance);
			}
			else if (IsEqualGUID(instance->guidType, GUID_Button))
			{
				control = new GenericButtonControl(inputDevice, instance);
				button = true;
			}
			else if (IsEqualGUID(instance->guidType, GUID_Key))
			{
				control = new KeyButtonControl(inputDevice, instance);
				button = true;
				
				if ((instance->dwOfs == DIK_GRAVE) || (control->GetControlName()[0] == '`')) control->SetControlAction(TheInputMgr->FindAction(kActionConsole));
				else if (instance->dwOfs == DIK_ESCAPE) control->SetControlAction(TheInputMgr->FindAction(kActionEscape));
			}
			else if (IsEqualGUID(instance->guidType, GUID_POV))
			{
				control = new DirectionalControl(inputDevice, instance);
			}
			
			if (control)
			{
				InputControlType type = control->GetControlType();
				if ((type == kInputLinearAxis) || (type == kInputRotationAxis))
				{
					DIPROPRANGE		range;
					DIPROPDWORD		dead;
					
					range.diph.dwSize = sizeof(DIPROPRANGE);
					range.diph.dwHeaderSize = sizeof(DIPROPHEADER);
					range.diph.dwObj = instance->dwType;
					range.diph.dwHow = DIPH_BYID;
					inputDevice->deviceInstance->GetProperty(DIPROP_RANGE, &range.diph);
					
					dead.diph.dwSize = sizeof(DIPROPDWORD);
					dead.diph.dwHeaderSize = sizeof(DIPROPHEADER);
					dead.diph.dwObj = instance->dwType;
					dead.diph.dwHow = DIPH_BYID;
					HRESULT hr = inputDevice->deviceInstance->GetProperty(DIPROP_DEADZONE, &dead.diph);
					
					float d = (hr == DI_OK) ? Fmax((float) dead.dwData * 0.00005F, 0.25F) : 0.25F;
					
					float vmin = (float) range.lMin;
					float vmax = (float) range.lMax;
					static_cast<AxisControl *>(control)->SetRange(vmin, vmax, (vmax - vmin) * d);
				}
				else if (type == kInputSlider)
				{
					DIPROPRANGE		range;
					
					range.diph.dwSize = sizeof(DIPROPRANGE);
					range.diph.dwHeaderSize = sizeof(DIPROPHEADER);
					range.diph.dwObj = instance->dwType;
					range.diph.dwHow = DIPH_BYID;
					inputDevice->deviceInstance->GetProperty(DIPROP_RANGE, &range.diph);
					
					float vmin = (float) range.lMin;
					float vmax = (float) range.lMax;
					static_cast<SliderControl *>(control)->SetRange(vmin, vmax, (vmax - vmin) * 0.0625F);
				}
				
				unsigned_int32 size = inputDevice->controlTableSize;
				if (button) inputDevice->controlTableSize = size + 1;
				else inputDevice->controlTableSize = ((size + 3) & ~3) + 4;
				
				controlTree->AddSubnode(control);
				inputDevice->controlCount++;
			}
			
			return (DIENUM_CONTINUE);
		}
		
		void DirectInputDevice::BuildDataFormat(void)
		{
			DIDATAFORMAT	dataFormat;
			
			unsigned_int32 tableSize = (controlTableSize + 3) & ~3;
			
			dataFormat.dwSize = sizeof(DIDATAFORMAT);
			dataFormat.dwObjSize = sizeof(DIOBJECTDATAFORMAT);
			
			InputDeviceType type = GetDeviceType();
			if (type == kInputMouse) dataFormat.dwFlags = DIDF_RELAXIS;
			else if (type == kInputJoystick) dataFormat.dwFlags = DIDF_ABSAXIS;
			else dataFormat.dwFlags = 0;
			
			dataFormat.dwDataSize = tableSize;
			dataFormat.dwNumObjs = controlCount;
			
			DIOBJECTDATAFORMAT *objectDataFormat = new DIOBJECTDATAFORMAT[controlCount];
			dataFormat.rgodf = objectDataFormat;
			
			controlTable = new InputControl *[tableSize];
			MemoryMgr::ClearMemory(controlTable, sizeof(InputControl *) * tableSize);
			
			int32 count = 0;
			DWORD offset = 0;
			
			InputControl *control = controlTree.GetFirstSubnode();
			while (control)
			{
				InputControlType type = control->GetControlType();
				bool button = ((type == kInputKeyButton) || (type == kInputGenericButton));
				if (!button) offset = (offset + 3) & ~3;
				
				controlTable[offset] = control;
				control->dataOffset = offset;
				
				DIOBJECTDATAFORMAT *format = &objectDataFormat[count];
				format->pguid = &control->dataGuid;
				format->dwOfs = offset;
				format->dwType = control->dataType;
				format->dwFlags = control->dataFlags;
				
				offset += (button) ? 1 : 4;
				count++;
				
				control = control->Next();
			}
			
			deviceInstance->SetDataFormat(&dataFormat);
			
			delete[] objectDataFormat;
		}
	
	#endif
	
	void DirectInputDevice::Activate(void)
	{
		InputDevice::Activate();
		
		#if !C4SERVER
		
			deviceInstance->Acquire();
			
			DWORD count = INFINITE;
			deviceInstance->GetDeviceData(sizeof(DIDEVICEOBJECTDATA), nullptr, &count, 0);
		
		#endif
	}
	
	void DirectInputDevice::Deactivate(void)
	{
		#if !C4SERVER
		
			deviceInstance->Unacquire();
		
		#endif
		
		InputDevice::Deactivate();
	}
	
	bool DirectInputDevice::ProcessEvents(InputMode mode)
	{
		#if !C4SERVER
		
			static DIDEVICEOBJECTDATA	data[kInputQueueSize];
			
			if (mode != kInputConfiguration)
			{
				DWORD count = kInputQueueSize;
				HRESULT hr = deviceInstance->GetDeviceData(sizeof(DIDEVICEOBJECTDATA), data, &count, 0);
				if (hr == DI_OK)
				{
					for (unsigned_machine a = 0; a < count; a++)
					{
						InputControl *control = controlTable[data[a].dwOfs];
						if (control) control->HandleNormalEvent(data[a].dwData);
					}
				}
				else if ((hr == DIERR_NOTACQUIRED) || (hr == DIERR_INPUTLOST))
				{
					deviceInstance->Acquire();
					ResetActions();
				}
			}
			else
			{
				DWORD count = 1;
				HRESULT hr = deviceInstance->GetDeviceData(sizeof(DIDEVICEOBJECTDATA), data, &count, 0);
				if (((hr == DI_OK) || (hr == DI_BUFFEROVERFLOW)) && (count != 0))
				{
					InputControl *control = controlTable[data[0].dwOfs];
					if (control)
					{
						if (control->HandleConfigEvent(data[0].dwData)) return (true);
					}
				}
				else if ((hr == DIERR_NOTACQUIRED) || (hr == DIERR_INPUTLOST))
				{
					deviceInstance->Acquire();
				}
			}
		
		#endif
		
		return (false);
	}

#endif


#if C4WINDOWS

	MouseDevice::MouseDevice(IDirectInput8A *directInput, const DIDEVICEINSTANCEA *instance) : StandardInputDevice(kInputMouse, directInput, instance)
	{
		#if !C4SERVER
		
			DIPROPDWORD		prop;
			
			/*deviceInstance->SetCooperativeLevel(TheEngine->GetWindow(), DISCL_NONEXCLUSIVE | DISCL_BACKGROUND);
			deviceInstance->EnumObjects(&EnumObjectsCallback, static_cast<DirectInputDevice *>(this), DIDFT_BUTTON | DIDFT_RELAXIS);
			BuildDataFormat();
			
			prop.diph.dwSize = sizeof(DIPROPDWORD);
			prop.diph.dwHeaderSize = sizeof(DIPROPHEADER);
			prop.diph.dwObj = 0;
			prop.diph.dwHow = DIPH_DEVICE;
			prop.dwData = kInputQueueSize;
			deviceInstance->SetProperty(DIPROP_BUFFERSIZE, &prop.diph);
		*/
		#endif
	}
	
	MouseDevice::~MouseDevice()
	{
		if (DeviceActive())
			Deactivate();
	}
	
	void MouseDevice::Activate(void)
	{
		StandardInputDevice::Activate();
		
		if (GetCapture())
			ReleaseCapture();
	}

	bool MouseDevice::ProcessEvents(InputMode mode)
	{
		WINDOWPLACEMENT pl;
		::GetWindowPlacement(TheEngine->GetWindow(), &pl);

		POINT pt;
		static POINT prevPt;
		::GetCursorPos(&pt);
		
		                                      
		::MapWindowPoints(NULL, TheEngine->GetWindow(),  &pt, 1);

		int nWidth = pl.rcNormalPosition.right - pl.rcNormalPosition.left;
		int nHeight = pl.rcNormalPosition.bottom - pl.rcNormalPosition.top;

		if( (0 <= pt.x && nWidth > pt.x) && (0 <= pt.y && nHeight > pt.y))
		{
			TheInputMgr->mouseDeltaX = -(pt.x - prevPt.x);
			TheInputMgr->mouseDeltaY = (pt.y - prevPt.y);
		}
		else
		{
			TheInputMgr->mouseDeltaX = 0;
			TheInputMgr->mouseDeltaY = 0;
		}
		prevPt = pt;

		return (false);
	}	

#elif C4MACOS

	MouseDevice::MouseDevice(io_object_t object, CFMutableDictionaryRef properties) : StandardInputDevice(kInputMouse, object, properties)
	{
	}
	
	MouseDevice::~MouseDevice()
	{
		if (DeviceActive()) Deactivate();
	}

#elif C4LINUX

	MouseDevice::MouseDevice() :
			StandardInputDevice(kInputMouse, "Mouse"),
			leftButton(this, "Left Mouse"),
			middleButton(this, "Middle Mouse"),
			rightButton(this, "Right Mouse"),
			horizontalAxis(this, kInputAxisX, "X Axis"),
			verticalAxis(this, kInputAxisY, "Y Axis"),
			wheelAxis(this, kInputAxisZ, "Wheel")
	{
		controlTree.AddSubnode(&leftButton);
		controlTree.AddSubnode(&middleButton);
		controlTree.AddSubnode(&rightButton);
		controlTree.AddSubnode(&horizontalAxis);
		controlTree.AddSubnode(&verticalAxis);
		controlTree.AddSubnode(&wheelAxis);
		
		horizontalAxis.SetControlAction(TheInputMgr->FindAction(kActionMouseX));
		verticalAxis.SetControlAction(TheInputMgr->FindAction(kActionMouseY));
	}
	
	MouseDevice::~MouseDevice()
	{
		if (DeviceActive()) Deactivate();
	}
	
	void MouseDevice::Activate(void)
	{
		::Window		rootWindow;
		::Window		childWindow;
		int				rootX, rootY;
		int				windowX, windowY;
		unsigned int	mask;
		
		StandardInputDevice::Activate();
		
		::Display *display = TheEngine->GetEngineDisplay();
		XChangePointerControl(display, true, true, 1, 1, 0);
		
		XQueryPointer(display, TheEngine->GetEngineWindow(), &rootWindow, &childWindow, &rootX, &rootY, &windowX, &windowY, &mask);
		originalPosition.Set(windowX, windowY);
		currentPosition.Set(windowX, windowY);
	}
	
	void MouseDevice::Deactivate(void)
	{
		XChangePointerControl(TheEngine->GetEngineDisplay(), true, true, -1, -1, -1);
		
		StandardInputDevice::Deactivate();
	}
	
	bool MouseDevice::ProcessEvents(InputMode mode)
	{
		if (mode != kInputConfiguration)
		{
			int32 x = currentPosition.x;
			int32 y = currentPosition.y;
			int32 dx = x - originalPosition.x;
			int32 dy = y - originalPosition.y;
			
			if ((dx | dy) != 0)
			{
				if (dx != 0) horizontalAxis.HandleNormalEvent(dx);
				if (dy != 0) verticalAxis.HandleNormalEvent(dy);
				
				currentPosition = originalPosition;
				XWarpPointer(TheEngine->GetEngineDisplay(), None, TheEngine->GetEngineWindow(), 0, 0, 0, 0, originalPosition.x, originalPosition.y);
			}
		}
		
		return (false);
	}
	
	void MouseDevice::HandleMouseButtonEvent(InputMode mode, const XButtonEvent *event)
	{
		if (mode != kInputConfiguration)
		{
			if (mode & kInputMouseActive)
			{
				if (event->type == ButtonPress)
				{
					switch (event->button)
					{
						case Button1:
							
							leftButton.HandleNormalEvent(1);
							break;
						
						case Button2:
							
							middleButton.HandleNormalEvent(1);
							break;
						
						case Button3:
							
							rightButton.HandleNormalEvent(1);
							break;
						
						case Button4:
							
							wheelAxis.HandleNormalEvent(1);
							break;
						
						case Button5:
							
							wheelAxis.HandleNormalEvent(-1);
							break;
					}
				}
				else
				{
					switch (event->button)
					{
						case Button1:
							
							leftButton.HandleNormalEvent(0);
							break;
						
						case Button2:
							
							middleButton.HandleNormalEvent(0);
							break;
						
						case Button3:
							
							rightButton.HandleNormalEvent(0);
							break;
					}
				}
			}
			else
			{
				switch (event->button)
				{
					case Button1:
					{
						Integer2D point(event->x, event->y);
						if (event->type == ButtonPress) TheEngine->HandleMouseEvent(kEventMouseDown, point);
						else TheEngine->HandleMouseEvent(kEventMouseUp, point);
						break;
					}
					
					case Button2:
					{
						Integer2D point(event->x, event->y);
						if (event->type == ButtonPress) TheEngine->HandleMouseEvent(kEventMiddleMouseDown, point);
						else TheEngine->HandleMouseEvent(kEventMiddleMouseUp, point);
						break;
					}
					
					case Button3:
					{
						Integer2D point(event->x, event->y);
						if (event->type == ButtonPress) TheEngine->HandleMouseEvent(kEventRightMouseDown, point);
						else TheEngine->HandleMouseEvent(kEventRightMouseUp, point);
						break;
					}
					
					case Button4:
						
						if (event->type == ButtonPress) TheEngine->HandleMouseEvent(kEventMouseWheel, Integer2D(0, 1));
						break;
					
					case Button5:
						
						if (event->type == ButtonPress) TheEngine->HandleMouseEvent(kEventMouseWheel, Integer2D(0, -1));
						break;
				}
			}
		}
		else
		{
			if (event->type == ButtonPress)
			{
				switch (event->button)
				{
					case Button1:
						
						leftButton.HandleConfigEvent(1);
						break;
					
					case Button2:
						
						middleButton.HandleConfigEvent(1);
						break;
					
					case Button3:
						
						rightButton.HandleConfigEvent(1);
						break;
					
					case Button4:
						
						wheelAxis.HandleConfigEvent(1);
						break;
					
					case Button5:
						
						wheelAxis.HandleConfigEvent(-1);
						break;
				}
			}
		}
	}
	
	void MouseDevice::HandleMouseMotionEvent(InputMode mode, const XMotionEvent *event)
	{
		if (mode != kInputConfiguration)
		{
			if (mode & kInputMouseActive)
			{
				int32 x = event->x;
				int32 y = event->y;
				if (((x - originalPosition.x) | (y - originalPosition.y)) != 0) currentPosition.Set(x, y);
			}
			else
			{
				TheEngine->HandleMouseEvent(kEventMouseMoved, Integer2D(event->x, event->y));
			}
		}
	}

#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

#endif //]

void MouseDevice::SetInputMode(InputMode mode)
{
	if (mode & kInputMouseActive)
	{
		if (!DeviceActive()) MouseDevice::Activate();
	}
	else
	{
		if (DeviceActive())
		{
			MouseDevice::Deactivate();
			Engine::ResetMouseButtonMask();
		}
	}
	
	#if C4MACOS
	
		if ((mode & kInputMouseActive) && (mode != kInputConfiguration))
		{
			CGAssociateMouseAndMouseCursorPosition(false);
		}
		else
		{
			CGAssociateMouseAndMouseCursorPosition(true);
		}
	
	#endif
}


#if C4WINDOWS

	KeyboardDevice::KeyboardDevice(IDirectInput8A *directInput, const DIDEVICEINSTANCEA *instance) : StandardInputDevice(kInputKeyboard, directInput, instance)
	{
		#if !C4SERVER
		
			DIPROPDWORD		prop;
			
			deviceInstance->SetCooperativeLevel(TheEngine->GetWindow(), DISCL_FOREGROUND | DISCL_NONEXCLUSIVE | DISCL_NOWINKEY);
			deviceInstance->EnumObjects(&EnumObjectsCallback, static_cast<DirectInputDevice *>(this), DIDFT_BUTTON);
			BuildDataFormat();
			
			prop.diph.dwSize = sizeof(DIPROPDWORD);
			prop.diph.dwHeaderSize = sizeof(DIPROPHEADER);
			prop.diph.dwObj = 0;
			prop.diph.dwHow = DIPH_DEVICE;
			prop.dwData = kInputQueueSize;
			deviceInstance->SetProperty(DIPROP_BUFFERSIZE, &prop.diph);
		
		#endif
	}
	
	KeyboardDevice::~KeyboardDevice()
	{
		if (DeviceActive()) Deactivate();
	}

#elif C4MACOS

	KeyboardDevice::KeyboardDevice(io_object_t object, CFMutableDictionaryRef properties) : StandardInputDevice(kInputKeyboard, object, properties)
	{
	}
	
	KeyboardDevice::~KeyboardDevice()
	{
		if (DeviceActive()) Deactivate();
	}

#elif C4LINUX

	KeyboardDevice::KeyboardDevice() : StandardInputDevice(kInputKeyboard, "Keyboard")
	{
		char ascii[2] = {0, 0};
		
		::Display *display = TheEngine->GetEngineDisplay();
		for (machine a = 0; a < kKeyboardRawCodeCount; a++)
		{
			keyButton[a] = nullptr;
			
			KeySym symbol = XKeycodeToKeysym(display, a + kKeyboardRawCodeBase, 0);
			if (symbol != NoSymbol)
			{
				const char *name = "";
				
				if ((unsigned_int32) (symbol - 0x20) < 0x5FU)
				{
					if (symbol != 0x20)
					{
						name = ascii;
						if ((unsigned_int32) (symbol - 'a') < 26U) ascii[0] = (char) (symbol - 32);
						else ascii[0] = (char) symbol;
					}
					else
					{
						name = keySymbolName[0x0100];
					}
				}
				else if ((unsigned_int32) (symbol - 0xFF00) < 0x0100U)
				{
					name = keySymbolName[symbol & 0x00FF];
				}
				
				if (name[0] == 0)
				{
					name = XKeysymToString(symbol);
					if (!name) continue;
				}
				
				KeyButtonControl *button = new KeyButtonControl(this, name);
				controlTree.AddSubnode(button);
				keyButton[a] = button;
				
				if (symbol == XK_grave) button->SetControlAction(TheInputMgr->FindAction(kActionConsole));
				else if (symbol == XK_Escape) button->SetControlAction(TheInputMgr->FindAction(kActionEscape));
			}
		}
	}
	
	KeyboardDevice::~KeyboardDevice()
	{
		if (DeviceActive()) Deactivate();
	}
	
	void KeyboardDevice::HandleKeyboardEvent(InputMode mode, const XKeyEvent *event)
	{
		if (mode != kInputConfiguration)
		{
			if (mode & kInputKeyboardActive)
			{
				unsigned_int32 code = event->keycode - kKeyboardRawCodeBase;
				if (code < kKeyboardRawCodeCount)
				{
					KeyButtonControl *control = keyButton[code];
					if (control) control->HandleNormalEvent(event->type == KeyPress);
				}
			}
			else
			{
				EventType	eventType;
				char		string[4];
				KeySym		symbol;
				
				if (event->type == KeyPress)
				{
					eventType = ((event->state & ControlMask) == 0) ? kEventKeyDown : kEventKeyCommand;
				}
				else
				{
					if ((event->state & ControlMask) != 0) return;
					eventType = kEventKeyUp;
				}
				
				unsigned_int32 modifierKeys = ((event->state & ShiftMask) != 0) ? kModifierKeyShift : 0;
				
				XLookupString(const_cast<XKeyEvent *>(event), string, 4, &symbol, nullptr);
				unsigned_int32 code = 0;
				
				if ((unsigned_int32) symbol - 0x0020 < 0x005FU)
				{
					if (eventType != kEventKeyCommand)
					{
						code = string[0];
					}
					else
					{
						code = symbol;
						if (code - 'a' < 26U) code -= 32;
					}
				}
				else if ((unsigned_int32) symbol - 0xFF08 < 0x00F8U)
				{
					switch (symbol)
					{
						case XK_BackSpace:
							code = kKeyCodeBackspace;
							break;
						case XK_Tab:
							code = kKeyCodeTab;
							break;
						case XK_Return:
							code = kKeyCodeReturn;
							break;
						case XK_Escape:
							code = kKeyCodeEscape;
							break;
						case XK_Home:
							code = kKeyCodeHome;
							break;
						case XK_Left:
							code = kKeyCodeLeftArrow;
							break;
						case XK_Up:
							code = kKeyCodeUpArrow;
							break;
						case XK_Right:
							code = kKeyCodeRightArrow;
							break;
						case XK_Down:
							code = kKeyCodeDownArrow;
							break;
						case XK_Page_Up:
							code = kKeyCodePageUp;
							break;
						case XK_Page_Down:
							code = kKeyCodePageDown;
							break;
						case XK_End:
							code = kKeyCodeEnd;
							break;
						case XK_Delete:
							code = kKeyCodeDelete;
							break;
					}
				}
				
				TheEngine->HandleKeyboardEvent(eventType, code, modifierKeys);
			}
		}
		else
		{
			unsigned_int32 code = event->keycode - kKeyboardRawCodeBase;
			if (code < kKeyboardRawCodeCount)
			{
				KeyButtonControl *control = keyButton[code];
				if (control) control->HandleConfigEvent(1);
			}
		}
	}

#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

#endif //]

void KeyboardDevice::SetInputMode(InputMode mode)
{
	if (mode & kInputKeyboardActive)
	{
		if (!DeviceActive())
		{
			KeyboardDevice::Activate();
			
			#if C4LINUX
			
				XAutoRepeatOff(TheEngine->GetEngineDisplay());
			
			#endif
		}
	}
	else
	{
		if (DeviceActive())
		{
			KeyboardDevice::Deactivate();
			
			#if C4LINUX
			
				XAutoRepeatOn(TheEngine->GetEngineDisplay());
			
			#endif
		}
	}
}


#if C4WINDOWS

	JoystickDevice::JoystickDevice(IDirectInput8A *directInput, const DIDEVICEINSTANCEA *instance) : StandardInputDevice(kInputJoystick, directInput, instance)
	{
		#if !C4SERVER
		
			DIPROPDWORD		prop;
			
			deviceInstance->SetCooperativeLevel(TheEngine->GetWindow(), DISCL_FOREGROUND | DISCL_EXCLUSIVE);
			
			prop.diph.dwSize = sizeof(DIPROPDWORD);
			prop.diph.dwHeaderSize = sizeof(DIPROPHEADER);
			prop.diph.dwObj = 0;
			prop.diph.dwHow = DIPH_DEVICE;
			prop.dwData = kInputQueueSize;
			deviceInstance->SetProperty(DIPROP_BUFFERSIZE, &prop.diph);
			
			prop.diph.dwSize = sizeof(DIPROPDWORD);
			prop.diph.dwHeaderSize = sizeof(DIPROPHEADER);
			prop.diph.dwObj = 0;
			prop.diph.dwHow = DIPH_DEVICE;
			prop.dwData = DIPROPAXISMODE_ABS;
			deviceInstance->SetProperty(DIPROP_AXISMODE, &prop.diph);
			
			deviceInstance->EnumObjects(&EnumObjectsCallback, static_cast<DirectInputDevice *>(this), DIDFT_ALL);
			BuildDataFormat();
			
			deviceInstance->EnumEffects(&EnumEffectsCallback, this, DIEFT_CONSTANTFORCE | DIEFT_CUSTOMFORCE);
		
		#endif
	}
	
	JoystickDevice::~JoystickDevice()
	{
		if (DeviceActive()) Deactivate();
	}
	
	BOOL CALLBACK JoystickDevice::EnumEffectsCallback(const DIEFFECTINFOA *effect, void *cookie)
	{
		JoystickDevice *joystickDevice = static_cast<JoystickDevice *>(cookie);
		joystickDevice->feedbackList.Append(new InputFeedback(effect->tszName));
		
		return (DIENUM_CONTINUE);
	}
	
	bool JoystickDevice::ProcessEvents(InputMode mode)
	{
		#if !C4SERVER
		
			deviceInstance->Poll();
		
		#endif
		
		return (StandardInputDevice::ProcessEvents(mode));
	}

#elif C4MACOS

	JoystickDevice::JoystickDevice(io_object_t object, CFMutableDictionaryRef properties) : StandardInputDevice(kInputJoystick, object, properties)
	{
	}
	
	JoystickDevice::~JoystickDevice()
	{
		if (DeviceActive()) Deactivate();
	}

#endif

#if !C4GAMECONSOLE

	void JoystickDevice::SetInputMode(InputMode mode)
	{
		if (mode & kInputGameActive)
		{
			if (!DeviceActive()) JoystickDevice::Activate();
		}
		else
		{
			if (DeviceActive()) JoystickDevice::Deactivate();
		}
	}

#endif


#if C4XINPUT

	XboxDevice::XboxDevice(int32 index) : InputDevice(kInputXbox, "Xbox 360 Controller")
	{
		deviceIndex = index;
		packetNumber = 0xFFFFFFFF;
		
		char *storage = controlStorage;
		for (machine a = 0; a < kXboxButtonCount; a++)
		{
			GenericButtonControl *control = new(storage) GenericButtonControl(this, xboxButtonName[a]);
			buttonControl[a] = control;
			
			controlTree.AddSubnode(control);
			buttonState[a] = false;
			
			storage += sizeof(GenericButtonControl);
		}
		
		for (machine a = 0; a < kXboxTriggerCount; a++)
		{
			SliderControl *control = new(storage) SliderControl(this, xboxTriggerName[a]);
			triggerControl[a] = control;
			
			control->SetRange(0.0F, 255.0F, (float) XINPUT_GAMEPAD_TRIGGER_THRESHOLD);
			
			controlTree.AddSubnode(control);
			triggerState[a] = 0;
			
			storage += sizeof(SliderControl);
		}
		
		for (machine a = 0; a < kXboxAxisCount; a++)
		{
			LinearAxisControl *control = new(storage) LinearAxisControl(this, static_cast<InputAxis>(a & 1), xboxAxisName[a]);
			axisControl[a] = control;
			
			control->SetRange(-32768.0F, 32767.0F, (a < 2) ? (float) XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE : (float) XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE);
			
			controlTree.AddSubnode(control);
			axisState[a] = 0;
			
			storage += sizeof(LinearAxisControl);
		}
	}
	
	XboxDevice::~XboxDevice()
	{
		if (DeviceActive()) Deactivate();
		
		for (machine a = kXboxAxisCount - 1; a >= 0; a--) axisControl[a]->~LinearAxisControl();
		for (machine a = kXboxTriggerCount - 1; a >= 0; a--) triggerControl[a]->~SliderControl();
		for (machine a = kXboxButtonCount - 1; a >= 0; a--) buttonControl[a]->~GenericButtonControl();
	}
	
	void XboxDevice::SetInputMode(InputMode mode)
	{
		if (mode & kInputGameActive)
		{
			if (!DeviceActive()) XboxDevice::Activate();
		}
		else
		{
			if (DeviceActive()) XboxDevice::Deactivate();
		}
	}
	
	bool XboxDevice::ProcessEvents(InputMode mode)
	{
		XINPUT_STATE	inputState;
		
		if (XInputGetState(deviceIndex, &inputState) == ERROR_SUCCESS)
		{
			unsigned_int32 num = inputState.dwPacketNumber;
			if (packetNumber != num)
			{
				packetNumber = num;
				
				if (mode != kInputConfiguration)
				{
					unsigned_int32 state = inputState.Gamepad.wButtons;
					for (machine a = 0; a < 10; a++)
					{
						unsigned_int32 b = state & 1;
						if (b ^ buttonState[a])
						{
							buttonState[a] = (b != 0);
							buttonControl[a]->HandleNormalEvent(b);
						}
						
						state >>= 1;
					}
					
					state >>= 2;
					
					for (machine a = 10; a < kXboxButtonCount; a++)
					{
						unsigned_int32 b = state & 1;
						if (b ^ buttonState[a])
						{
							buttonState[a] = (b != 0);
							buttonControl[a]->HandleNormalEvent(b);
						}
						
						state >>= 1;
					}
					
					for (machine a = 0; a < kXboxTriggerCount; a++)
					{
						unsigned_int8 v = (&inputState.Gamepad.bLeftTrigger)[a];
						if (v != triggerState[a])
						{
							triggerState[a] = v;
							triggerControl[a]->HandleNormalEvent(v);
						}
					}
					
					for (machine a = 0; a < kXboxAxisCount; a++)
					{
						int16 v = (&inputState.Gamepad.sThumbLX)[a];
						if (v != axisState[a])
						{
							axisState[a] = v;
							axisControl[a]->HandleNormalEvent(v);
						}
					}
				}
				else
				{
					unsigned_int32 state = inputState.Gamepad.wButtons;
					for (machine a = 0; a < 10; a++)
					{
						unsigned_int32 b = state & 1;
						if ((b != 0) && (buttonControl[a]->HandleConfigEvent(1))) return (true);
						
						state >>= 1;
					}
					
					state >>= 2;
					
					for (machine a = 10; a < kXboxButtonCount; a++)
					{
						unsigned_int32 b = state & 1;
						if ((b != 0) && (buttonControl[a]->HandleConfigEvent(1))) return (true);
						
						state >>= 1;
					}
					
					for (machine a = 0; a < kXboxTriggerCount; a++)
					{
						unsigned_int8 v = (&inputState.Gamepad.bLeftTrigger)[a];
						if (v != triggerState[a])
						{
							triggerState[a] = v;
							if (triggerControl[a]->HandleConfigEvent(v)) return (true);
						}
					}
					
					for (machine a = 0; a < kXboxAxisCount; a++)
					{
						int16 v = (&inputState.Gamepad.sThumbLX)[a];
						if (v != axisState[a])
						{
							axisState[a] = v;
							if (axisControl[a]->HandleConfigEvent(v)) return (true);
						}
					}
				}
			}
		}
		
		return (false);
	}

#endif


#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

#endif //]


InputMgr::InputMgr(int) :
		mouseXAction(kActionMouseX),
		mouseYAction(kActionMouseY),
		sensitivityObserver(this, &InputMgr::HandleSensitivityEvent),
		invertMouseObserver(this, &InputMgr::HandleInvertMouseEvent),
		smoothMouseObserver(this, &InputMgr::HandleSmoothMouseEvent)
{
}

InputMgr::~InputMgr()
{
}

EngineResult InputMgr::Construct(void)
{
	AddAction(&consoleAction);
	AddAction(&escapeAction);
	AddAction(&mouseXAction);
	AddAction(&mouseYAction);
	
	#if C4WINDOWS && !C4SERVER
	
		xinputDeviceCount = 0;
		
		IWbemLocator *wbemLocator = nullptr;
		IWbemServices *wbemServices = nullptr;
		IEnumWbemClassObject *deviceEnumerator = nullptr;
		
		if (SUCCEEDED(CoCreateInstance(__uuidof(WbemLocator), nullptr, CLSCTX_INPROC_SERVER, __uuidof(IWbemLocator), (void **) &wbemLocator)))
		{
			BSTR namespaceString = SysAllocString(L"\\\\.\\root\\cimv2");
			BSTR classString = SysAllocString(L"Win32_PNPEntity");
			BSTR idString = SysAllocString(L"DeviceID");
			
			if (SUCCEEDED(wbemLocator->ConnectServer(namespaceString, nullptr, nullptr, nullptr, 0, nullptr, nullptr, &wbemServices)))
			{
				CoSetProxyBlanket(wbemServices, RPC_C_AUTHN_WINNT, RPC_C_AUTHZ_NONE, nullptr, RPC_C_AUTHN_LEVEL_CALL, RPC_C_IMP_LEVEL_IMPERSONATE, nullptr, EOAC_NONE);
				
				if (SUCCEEDED(wbemServices->CreateInstanceEnum(classString, WBEM_FLAG_RETURN_IMMEDIATELY | WBEM_FLAG_FORWARD_ONLY, nullptr, &deviceEnumerator)))
				{
					for (;;)
					{
						ULONG				count;
						IWbemClassObject	*device[16];
						
						if ((FAILED(deviceEnumerator->Next(1000, 16, device, &count))) || (count == 0)) break;
						
						for (unsigned_machine a = 0; a < count; a++)
						{
							VARIANT		variant;
							
							if (SUCCEEDED(device[a]->Get(idString, 0, &variant, nullptr, nullptr)))
							{
								if (variant.vt == VT_BSTR)
								{
									const wchar_t *id = variant.bstrVal;
									if ((id) && (FindDeviceSubstring(id, L"IG_")))
									{
										unsigned_int32 guid = 0;
										
										const wchar_t *pid = FindDeviceSubstring(id, L"PID_");
										if (pid) guid = ReadDeviceHexString(pid);
										
										const wchar_t *vid = FindDeviceSubstring(id, L"VID_");
										if (vid) guid = (guid << 16) | ReadDeviceHexString(vid);
										
										if (guid != 0)
										{
											int32 deviceCount = xinputDeviceCount;
											if (deviceCount < kMaxXinputDeviceCount)
											{
												xinputDeviceGuid[deviceCount] = guid;
												xinputDeviceCount = deviceCount + 1;
											}
										}
									}
								}
								
								VariantClear(&variant);
							}
							
							device[a]->Release();
						}
					}
				}
			}
			
			SysFreeString(idString);
			SysFreeString(classString);
			SysFreeString(namespaceString);
		}
		
		InputResult result = kInputOkay;
		if (FAILED(DirectInput8Create(TheEngine->GetInstance(), DIRECTINPUT_VERSION, IID_IDirectInput8A, (void **) &directInput, nullptr)))
			result = kInputInitFailed;
		else 
			directInput->EnumDevices(DI8DEVCLASS_ALL, &EnumDevicesCallback, this, DIEDFL_ATTACHEDONLY);
		
		if (wbemLocator)
		{
			if (wbemServices)
			{
				if (deviceEnumerator) deviceEnumerator->Release();
				wbemServices->Release();
			}
			
			wbemLocator->Release();
		}
		
		if (result != kInputOkay)
			return (result);
		
	#elif C4MACOS && !C4SERVER
		
		io_iterator_t	iterator;
		
		CFMutableDictionaryRef dictionary = IOServiceMatching(kIOHIDDeviceKey);
		if ((!dictionary) || (IOServiceGetMatchingServices(kIOMasterPortDefault, dictionary, &iterator) != kIOReturnSuccess)) return (kInputInitFailed);
		
		for (;;)
		{
			CFMutableDictionaryRef	properties;
			
			io_object_t object = IOIteratorNext(iterator);
			if (!object) break;
			
			if (IORegistryEntryCreateCFProperties(object, &properties, kCFAllocatorDefault, kNilOptions) == KERN_SUCCESS)
			{
				const void *usagePageValue = CFDictionaryGetValue(properties, CFSTR(kIOHIDPrimaryUsagePageKey));
				const void *usageValue = CFDictionaryGetValue(properties, CFSTR(kIOHIDPrimaryUsageKey));
				if ((usagePageValue) && (usageValue))
				{
					int32	usagePage;
					int32	usage;
					
					CFNumberGetValue((CFNumberRef) usagePageValue, kCFNumberLongType, &usagePage);
					CFNumberGetValue((CFNumberRef) usageValue, kCFNumberLongType, &usage);
					
					if (usagePage == kHIDPage_GenericDesktop)
					{
						if ((usage == kHIDUsage_GD_Pointer) || (usage == kHIDUsage_GD_Mouse)) deviceList.Append(new MouseDevice(object, properties));
						else if ((usage == kHIDUsage_GD_Keyboard) || (usage == kHIDUsage_GD_Keypad)) deviceList.Append(new KeyboardDevice(object, properties));
						else if ((usage == kHIDUsage_GD_Joystick) || (usage == kHIDUsage_GD_GamePad)) deviceList.Append(new JoystickDevice(object, properties));
					}
				}
				
				CFRelease(properties);
			}
			
			IOObjectRelease(object);
		}
		
		IOObjectRelease(iterator);
	
	#elif C4LINUX
	
		mouseDevice = new MouseDevice;
		deviceList.Append(mouseDevice);
		
		keyboardDevice = new KeyboardDevice;
		deviceList.Append(keyboardDevice);
	
	#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#endif //]
	
	#if C4WINDOWS && !C4SERVER
	
		/*for (machine a = 0; a < 4; a++)
		{
			XINPUT_CAPABILITIES		capabilities;
			
			if (XInputGetCapabilities(a, XINPUT_FLAG_GAMEPAD, &capabilities) == ERROR_SUCCESS)
			{
				XboxDevice *device = new XboxDevice(a);
				deviceList.Append(device);
			}
		}*/
	
	#endif
	
	InputDevice *device = deviceList.First();
	while (device)
	{
		InputDevice *next = device->Next();
		//if (!device->GetFirstControl()) delete device;
		device = next;
	}
	
	consoleProc = nullptr;
	escapeProc = nullptr;
	configProc = nullptr;
	
	mouseFlags = 0;
	
	TheEngine->InitVariable("sensitivity", "15", kVariablePermanent, &sensitivityObserver);
	TheEngine->InitVariable("invertMouse", "0", kVariablePermanent, &invertMouseObserver);
	TheEngine->InitVariable("smoothMouse", "1", kVariablePermanent, &smoothMouseObserver);
	
	UpdateLog();
	return (kInputOkay);
}

void InputMgr::Destruct(void)
{
	inputMode = kInputInactive;
	
	RemoveAction(&mouseYAction);
	RemoveAction(&mouseXAction);
	RemoveAction(&escapeAction);
	RemoveAction(&consoleAction);
	
	actionList.Purge();
	deviceList.Purge();
	
	#if C4WINDOWS
	
		#if !C4SERVER
		
			directInput->Release();
		
		#endif
	
	#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#endif //]
}

#if C4WINDOWS && !C4SERVER

	const wchar_t *InputMgr::FindDeviceSubstring(const wchar_t *string, const wchar_t *substring)
	{
		int32 subposition = 0;
		int32 subchar = substring[0];
		
		for (;;)
		{
			int32 c = *string++;
			if (c == 0) break;
			
			if (c == subchar)
			{
				subchar = substring[++subposition];
				if (subchar == 0) return (string);
			}
			else if (subposition != 0)
			{
				subposition = 0;
				subchar = substring[0];
				continue;
			}
		}
		
		return (nullptr);
	}
	
	unsigned_int32 InputMgr::ReadDeviceHexString(const wchar_t *string)
	{
		unsigned_int32 id = 0;
		
		for (machine a = 0; a < 4; a++)
		{
			unsigned_int32 c = string[a];
			if (c == 0) break;
			
			id <<= 4;
			if ((c -= 48) < 10U) id |= c;
			else if (((c -= 17) < 6U) || ((c -= 32) < 6U)) id |= c + 10;
		}
		
		return (id);
	}
	
	BOOL CALLBACK InputMgr::EnumDevicesCallback(const DIDEVICEINSTANCEA *instance, void *cookie)
	{
		InputMgr *inputMgr = static_cast<InputMgr *>(cookie);
		
		DWORD type = GET_DIDEVICE_TYPE(instance->dwDevType);
		if (type == DI8DEVTYPE_MOUSE)
		{
			inputMgr->deviceList.Append(new MouseDevice(inputMgr->directInput, instance));
		}
		else if (type == DI8DEVTYPE_KEYBOARD)
		{
			inputMgr->deviceList.Append(new KeyboardDevice(inputMgr->directInput, instance));
		}
		else
		{
			unsigned_int32 guid = instance->guidProduct.Data1;
			
			int32 count = inputMgr->xinputDeviceCount;
			for (machine a = 0; a < count; a++)
			{
				if (inputMgr->xinputDeviceGuid[a] == guid) return (DIENUM_CONTINUE);
			}
			
			inputMgr->deviceList.Append(new JoystickDevice(inputMgr->directInput, instance));
		}
		
		return (DIENUM_CONTINUE);
	}

#endif

void InputMgr::HandleSensitivityEvent(Variable *variable)
{
	SetMouseSensitivity(Min(Max(variable->GetIntegerValue(), 1), 100));
}

void InputMgr::HandleInvertMouseEvent(Variable *variable)
{
	unsigned_int32 flags = mouseFlags;

	if (variable->GetIntegerValue() != 0) flags |= kMouseInverted;
	else flags &= ~kMouseInverted;
	
	SetMouseFlags(flags);
}

void InputMgr::HandleSmoothMouseEvent(Variable *variable)
{
	unsigned_int32 flags = mouseFlags;

	if (variable->GetIntegerValue() != 0) flags |= kMouseSmooth;
	else flags &= ~kMouseSmooth;
	
	SetMouseFlags(flags);
}

void InputMgr::UpdateLog(void) const
{
	Engine::Report("Input Manager", kReportLog | kReportHeading);
	Engine::Report("<table cellspacing=\"0\" cellpadding=\"0\">\r\n", kReportLog);
	
	const InputDevice *device = GetFirstDevice();
	while (device)
	{
		Engine::Report("<tr><th>", kReportLog);
		Engine::Report(device->GetDeviceName(), kReportLog);
		
		switch (device->GetDeviceType())
		{
			case kInputMouse:
				
				Engine::Report("<br/><span style=\"font-weight: normal;\">(Mouse)</span>", kReportLog);
				break;
			
			case kInputKeyboard:
				
				Engine::Report("<br/><span style=\"font-weight: normal;\">(Keyboard)</span>", kReportLog);
				break;
		}
		
		Engine::Report("</th><td><div style=\"height: 128px; overflow: auto;\">\r\n", kReportLog);
		
		const InputControl *control = device->GetFirstControl();
		while (control)
		{
			if (control->GetControlType() != kInputGroup)
			{
				Engine::Report(control->GetControlName(), kReportLog);
				Engine::Report("<br/>\r\n", kReportLog);
			}
			
			control = device->GetNextControl(control);
		}
		
		Engine::Report("</div></td></tr>\r\n", kReportLog);
		
		device = device->Next();
	}
	
	Engine::Report("</table>\r\n", kReportLog);
}

InputDevice *InputMgr::FindDevice(const char *name) const
{
	InputDevice *device = deviceList.First();
	while (device)
	{
		if (Text::CompareTextCaseless(device->GetDeviceName(), name)) break;
		device = device->Next();
	}
	
	return (device);
}

InputDevice *InputMgr::FindDevice(InputDeviceType type) const
{
	InputDevice *device = deviceList.First();
	while (device)
	{
		if (device->GetDeviceType() == type) break;
		device = device->Next();
	}
	
	return (device);
}

Action *InputMgr::FindAction(ActionType type) const
{
	Action *action = actionList.First();
	while (action)
	{
		if (action->GetActionType() == type) break;
		action = action->Next();
	}
	
	return (action);
}

void InputMgr::SetInputMode(InputMode mode)
{
	#if !C4SERVER
	
		if (inputMode != mode)
		{
			internalInputMode = inputMode;
			inputMode = mode;
			
			InputDevice *device = deviceList.First();
			while (device)
			{
				device->SetInputMode(mode);
				device = device->Next();
			}
			
			prevMouseDeltaX = 0.0F;
			prevMouseDeltaY = 0.0F;
		}
	
	#endif
}

void InputMgr::SetMouseSensitivity(int32 sensitivity)
{
	mouseSensitivity = sensitivity;

	deltaXMultiplier = (float) mouseSensitivity * 2.0e-4F;
	deltaYMultiplier = (mouseFlags & kMouseInverted) ? deltaXMultiplier : -deltaXMultiplier;
}

void InputMgr::SetMouseFlags(unsigned_int32 flags)
{
	mouseFlags = flags;
	
	float m = deltaXMultiplier;
	deltaYMultiplier = (flags & kMouseInverted) ? m : -m;
}

InputControl *InputMgr::GetActionControl(const Action *action, int32 index)
{
	InputDevice *device = deviceList.First();
	while (device)
	{
		InputControl *control = device->GetFirstControl();
		while (control)
		{
			if (control->GetControlAction() == action)
			{
				if (index == 0) return (control);
				index--;
			}
			
			control = device->GetNextControl(control);
		}
		
		device = device->Next();
	}
	
	return (nullptr);
}

void InputMgr::ClearAllControlActions(void)
{
	InputDevice *device = deviceList.First();
	while (device)
	{
		InputControl *control = device->GetFirstControl();
		while (control)
		{
			Action *action = control->GetControlAction();
			if ((action) && (!(action->GetActionFlags() & kActionPersistent))) control->SetControlAction(nullptr);
			
			control = device->GetNextControl(control);
		}
		
		device = device->Next();
	}
}

void InputMgr::ResetAllActions(void)
{
	Action *action = actionList.First();
	while (action)
	{
		if (action->GetActiveCount() > 0)
		{
			action->SetActiveCount(0);
			action->End();
		}
		
		action = action->Next();
	}
}

void InputMgr::InputTask(void)
{
	mouseDeltaX = 0.0F;
	mouseDeltaY = 0.0F;
	
	InputMode mode = inputMode;
	internalInputMode = mode;
	
	if (mode != kInputInactive)
	{
		InputDevice *device = deviceList.First();
		while (device)
		{
			if ((device->DeviceActive()) && (device->ProcessEvents(mode)))
				break;
			device = device->Next();
		}
		
		if (mouseFlags & kMouseSmooth)
		{
			float x = mouseDeltaX * 0.5F;
			float y = mouseDeltaY * 0.5F;
			mouseDeltaX = prevMouseDeltaX + x;
			mouseDeltaY = prevMouseDeltaY + y;
			prevMouseDeltaX = x;
			prevMouseDeltaY = y;
		}
	}
	
	#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#endif //]
}

// ZYURVUR
