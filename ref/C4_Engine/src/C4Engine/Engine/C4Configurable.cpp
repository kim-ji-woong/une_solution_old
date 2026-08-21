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


#include "C4Configurable.h"


using namespace C4;


int32 Configurable::GetSettingCount(void) const
{
	return (0);
}

Setting *Configurable::GetSetting(int32 index) const
{
	return (nullptr);
}

void Configurable::SetSetting(const Setting *value)
{
}

int32 Configurable::GetCategoryCount(void) const
{
	return (0);
}

Type Configurable::GetCategoryType(int32 index, const char **title) const
{
	return (0);
}

int32 Configurable::GetCategorySettingCount(Type category) const
{
	return (0);
}

Setting *Configurable::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	return (nullptr);
}

void Configurable::SetCategorySetting(Type category, const Setting *setting)
{
}

void *Configurable::BeginSettings(void)
{
	return (nullptr);
}

void Configurable::EndSettings(void *cookie)
{
}

// ZYURVUR
