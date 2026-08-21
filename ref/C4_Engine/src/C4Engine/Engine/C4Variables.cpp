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


#include "C4Variables.h"


using namespace C4;


Variable::Variable(const char *name, unsigned_int32 flags, ObserverType *observer)
{
	variableFlags = flags;
	variableName = name;
	variableValue[0] = 0;

	if (observer) AddObserver(observer);
}

Variable::~Variable()
{
}

void Variable::SetValue(const char *value)
{
	variableValue = value;
	PostEvent();
}

void Variable::SetIntegerValue(int32 value)
{
	variableValue = value;
	PostEvent();
}

void Variable::SetFloatValue(float value)
{
	variableValue = value;
	PostEvent();
}

// ZYURVUR
