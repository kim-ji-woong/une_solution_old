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


#ifndef C4ExtrasPlugin_h
#define C4ExtrasPlugin_h


#include "C4Plugins.h"
#include "C4ExtraEffects.h"
#include "C4RotationController.h"
#include "C4StarField.h"


#ifdef C4EXTRAS

	extern "C"
	{
		C4MODULEEXPORT C4::Plugin *ConstructPlugin(void);
	}

#endif


namespace C4
{
	class ExtrasPlugin : public Plugin, public Singleton<ExtrasPlugin>
	{
		private:
			
			StringTable							stringTable;
			
			Constructor<Controller>				controllerConstructor;
			ControllerReg<RotationController>	rotationControllerReg;
			EffectReg<ShockwaveEffect>			shockwaveEffectReg;
			EffectReg<ShellEffect>				shellEffectReg;
			ParticleSystemReg<StarField>		starFieldReg;
			
			static Controller *ConstructController(Unpacker& data, unsigned_int32 unpackFlags);
		
		public:
			
			ExtrasPlugin();
			~ExtrasPlugin();
			
			const StringTable *GetStringTable(void) const
			{
				return (&stringTable);
			}
	};
	
	
	extern ExtrasPlugin *TheExtrasPlugin;
}


#endif

// ZYURVUR
