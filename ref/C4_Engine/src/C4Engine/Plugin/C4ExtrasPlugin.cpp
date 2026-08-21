//=============================================================
//
// C4 Engine version 2.9.1
// Copyright 1999-2012, by Terathon Software LLC
//
// This file is part of the C4 Engine and is provided under the
// terms of the license agreement entered by the registed user.
//
// Unauthorized redistribution of source code is strictly
// prohibited. Violators will be prosecuted.
//
//=============================================================


#include "C4ExtrasPlugin.h"
#include "C4FlashController.h"


using namespace C4;


ExtrasPlugin *C4::TheExtrasPlugin = nullptr;


C4::Plugin *ConstructPlugin(void)
{
	return (new ExtrasPlugin);
}


ExtrasPlugin::ExtrasPlugin() :
		Singleton<ExtrasPlugin>(TheExtrasPlugin),
		stringTable("Extras/strings"),
		
		controllerConstructor(&ConstructController),
		rotationControllerReg(kControllerRotation, stringTable.GetString(StringID('CTRL', kControllerRotation))),
		shockwaveEffectReg(kEffectShockwave, stringTable.GetString(StringID('EFCT', kEffectShockwave))),
		shellEffectReg(kEffectShell, stringTable.GetString(StringID('EFCT', kEffectShell))),
		starFieldReg(kParticleSystemStarField, stringTable.GetString(StringID('PART', kParticleSystemStarField)))
{
	Controller::InstallConstructor(&controllerConstructor);
	RotationController::RegisterFunctions(&rotationControllerReg);
}

ExtrasPlugin::~ExtrasPlugin()
{
}

Controller *ExtrasPlugin::ConstructController(Unpacker& data, unsigned_int32 unpackFlags)
{
	if (data.GetType() == kControllerFlash) return (new FlashController);
	return (nullptr);
}

// ZYURVUR
