#pragma once

#define SPIN_CONTROL_VAR(Name)	m_spin ## Name
#define EDIT_CONTROL_VAR(Name)	m_edit ## Name
#define InitSpinControls(Name)	InitSpinControl(MIN_SPIN_VAR(Name),MAX_SPIN_VAR(Name),CUR_SPIN_VAR(Name),SPIN_CONTROL_VAR(Name),(int)(EDIT_CONTROL_VAR(Name).GetData()))
#define OnDeltaposSpinControls(Name)	{\
	if (EDIT_CONTROL_VAR(Name).GetClassType() == 1/* Double */) OnDeltaposSpinControlDouble(MIN_SPIN_VAR(Name),MAX_SPIN_VAR(Name),CUR_SPIN_VAR(Name),SPIN_CONTROL_VAR(Name),EDIT_CONTROL_VAR(Name));	\
	else if (EDIT_CONTROL_VAR(Name).GetClassType() == 0 /* Int */) OnDeltaposSpinControlInt(MIN_SPIN_VAR(Name),MAX_SPIN_VAR(Name),CUR_SPIN_VAR(Name),SPIN_CONTROL_VAR(Name),EDIT_CONTROL_VAR(Name));	}
//if (!strcmp(typeid(EDIT_CONTROL_VAR(Name)).name(),"class CEditDouble")) OnDeltaposSpinControlDouble(MIN_SPIN_VAR(Name),MAX_SPIN_VAR(Name),CUR_SPIN_VAR(Name),SPIN_CONTROL_VAR(Name),EDIT_CONTROL_VAR(Name));	\
//else if (!strcmp(typeid(EDIT_CONTROL_VAR(Name)).name(),"class CEditInt")) OnDeltaposSpinControlInt(MIN_SPIN_VAR(Name),MAX_SPIN_VAR(Name),CUR_SPIN_VAR(Name),SPIN_CONTROL_VAR(Name),EDIT_CONTROL_VAR(Name));	}

// Functions for Spin Control, Edit Control
// 2008/6/4 [kjw]
void InitSpinControl(int& rMin, int& rMax, int& rCurrent, CSpinButtonCtrl& rSpin, int nCurrentData);
void OnDeltaposSpinControlInt(int& rMin, int& rMax, int& rCurrent, CSpinButtonCtrl& rSpin, CEdit& rEdit);
void OnDeltaposSpinControlDouble(int& rMin, int& rMax, int& rCurrent, CSpinButtonCtrl& rSpin, CEdit& rEdit);
