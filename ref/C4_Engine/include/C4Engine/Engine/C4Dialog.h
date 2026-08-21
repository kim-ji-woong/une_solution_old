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


#ifndef C4Dialog_h
#define C4Dialog_h


//# \component	Interface Manager
//# \prefix		InterfaceMgr/


#include "C4Interface.h"


namespace C4
{
	//# \enum	DialogStatus
	
	enum
	{
		kDialogOkay,		//## The okay button was triggered.
		kDialogCancel,		//## The cancel button was triggered.
		kDialogIgnore		//## The ignore button was triggered.
	};
	
	
	//# \class	Dialog		Encapsulates a basic dialog box.
	//
	//# A $Dialog$ class encapsulates a basic dialog box.
	//
	//# \def	class Dialog : public Window, public Completable<Dialog>
	//
	//# \ctor	Dialog(const Vector2D& size, const char *title, const char *okayText,
	//# \ctor2	const char *cancelText = nullptr, const char *ignoreText = nullptr);
	//
	//# \param	size		The size of the dialog window.
	//# \param	title		The dialog window's displayed title.
	//# \param	okayText	The text to be displayed in the okay button. This should not be $nullptr$.
	//# \param	cancelText	The text to be displayed in the cancel button. This can be $nullptr$.
	//# \param	ignoreText	The text to be displayed in the ignore button. This can be $nullptr$.
	//
	//# \desc
	//# The $Dialog$ class provides a convenient way to display a basic dialog box. Between one and three buttons,
	//# having the semantic meanings &ldquo;okay&rdquo;, &ldquo;cancel&rdquo;, and &ldquo;ignore&rdquo;, are displayed
	//# at the bottom of the dialog box. Other interface widgets can be added by creating them separately and then
	//# calling $@Utilities/Tree::AddSubnode@$ to add them to the window.
	//# 
	//# The $okayText$ parameter specifies the text that is displayed in the okay button. The okay button
	//# is triggered if the user presses the Enter key (or the Return key on the Mac). If there is no cancel button,
	//# then the Escape key also triggers the okay button.
	//# 
	//# If the $cancelText$ parameter is not $nullptr$, then a cancel button is displayed with the text it specifies.
	//# The cancel button is triggered if the user presses the Escape key.
	//# 
	//# If the $ignoreText$ parameter is not $nullptr$, then a button is displayed on the left side of the window
	//# with the text it specifies. No keys trigger the ignore button.
	//# 
	//# Once a dialog box has been created, it can be displayed by calling the $@InterfaceMgr::AddWidget@$ function.
	//# If a dialog box is added to an existing window using the $@Window::AddSubwindow@$ function, then it becomes
	//# a modal dialog for that window.
	//# 
	//# When the user clicks a button or presses a key that triggers a button, the dialog box is dismissed.
	//# Before the $Dialog$ object destroys itself, it records which button was triggered and calls its completion
	//# procedure. The completion procedure can call the $@Dialog::GetDialogStatus@$ function to determine which
	//# button was triggered to dismiss the dialog.
	//
	//# \base	Window							The $Dialog$ class is a specific type of window.
	//# \base	Utilities/Completable<Dialog>	The completion procedure is called when the dialog is dismissed.
	
	
	//# \function	Dialog::GetDialogStatus		Returns the status of a dialog box.
	//
	//# \proto	int32 GetDialogStatus(void) const;
	//
	//# \desc
	//# The $GetDialogStatus$ function returns the status of a dialog box that has been dismissed. It should be
	//# called from within the dialog box's completion procedure to determine how the dialog was dismissed.
	//# The return value is one of the following constants.
	//
	//# \table	DialogStatus
	//
	//# The status of a dialog box is undefined before it is dismissed by the user.
	
	
	class Dialog : public Window, public Completable<Dialog>
	{
		private:
			
			int32				dialogStatus;
			unsigned_int32		ignoreKeyCode;
			
			PushButtonWidget	okayButton;
			PushButtonWidget	cancelButton;
			PushButtonWidget	ignoreButton;
		
		public:
			
			C4API Dialog(const Vector2D& size, const char *title, const char *okayText, const char *cancelText = nullptr, const char *ignoreText = nullptr);
			C4API ~Dialog();
			
			int32 GetDialogStatus(void) const
			{
				return (dialogStatus);
			}
			 
			unsigned_int32 GetIgnoreKeyCode(void) const
			{ 
				return (ignoreKeyCode); 
			} 
			
			void SetIgnoreKeyCode(unsigned_int32 code) 
			{
				ignoreKeyCode = code;
			}
			 
			PushButtonWidget *GetOkayButton(void)
			{
				return (&okayButton);
			} 
			
			PushButtonWidget *GetCancelButton(void)
			{
				return (&cancelButton);
			}
			
			PushButtonWidget *GetIgnoreButton(void)
			{
				return (&ignoreButton);
			}
			
			C4API bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			C4API void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
}


#endif

// ZYURVUR
