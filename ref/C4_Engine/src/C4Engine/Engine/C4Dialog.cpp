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


#include "C4Dialog.h"


using namespace C4;


Dialog::Dialog(const Vector2D& size, const char *title, const char *okayText, const char *cancelText, const char *ignoreText) :
		Window(size, title, kWindowBackground | kWindowCenter),
		okayButton(Vector2D(72.0F, 24.0F), okayText, "font/Heading"),
		cancelButton(Vector2D(72.0F, 24.0F), cancelText, "font/Heading"),
		ignoreButton(Vector2D(72.0F, 24.0F), ignoreText, "font/Heading")
{
	okayButton.SetPushButtonFlags(kPushButtonPrimary);
	okayButton.SetWidgetPosition(Point3D(size.x - 84.0F, size.y - 32.0F, 0.0F));
	AddSubnode(&okayButton);
	
	if (cancelText)
	{
		cancelButton.SetWidgetPosition(Point3D(size.x - 172.0F, size.y - 32.0F, 0.0F));
		AddSubnode(&cancelButton);
	}
	
	if (ignoreText)
	{
		ignoreButton.SetWidgetPosition(Point3D(12.0F, size.y - 32.0F, 0.0F));
		AddSubnode(&ignoreButton);
	}
	
	dialogStatus = kDialogOkay;
	ignoreKeyCode = 0;
}

Dialog::~Dialog()
{
}

bool Dialog::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyDown)
	{
		unsigned_int32 code = eventData->keyCode;
		
		if (code == kKeyCodeReturn)
		{
			okayButton.Activate();
			return (true);
		}
		else if (code == kKeyCodeEscape)
		{
			if (cancelButton.GetSuperNode()) cancelButton.Activate();
			else okayButton.Activate();
			return (true);
		}
		else if ((code != 0) && (code == ignoreKeyCode))
		{
			ignoreButton.Activate();
			return (true);
		}
	}
	
	return (Window::HandleKeyboardEvent(eventData));
}

void Dialog::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		if (widget == &okayButton)
		{
			TheInterfaceMgr->RemoveWidget(this);
			dialogStatus = kDialogOkay;
			CallCompletionProc();
			Close();
		}
		else if (widget == &cancelButton)
		{
			TheInterfaceMgr->RemoveWidget(this);
			dialogStatus = kDialogCancel;
			CallCompletionProc();
			Close();
		}
		else if (widget == &ignoreButton)
		{
			TheInterfaceMgr->RemoveWidget(this);
			dialogStatus = kDialogIgnore;
			CallCompletionProc();
			Close();
		}
	}
}

// ZYURVUR
