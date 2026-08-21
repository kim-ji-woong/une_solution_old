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


#ifndef C4FlashController_h
#define C4FlashController_h


//# \component	Extras Plugin
//# \prefix		ExtrasPlugin/


#include "C4ExtrasBase.h"
#include "C4Controller.h"


namespace C4
{
	enum
	{
		kControllerFlash		= 'flsh'
	};
	
	
	//# \class	FlashController		Manages a light that flashes momentarily.
	//
	//# The $FlashController$ class manages a light that flashes momentarily.
	//
	//# \def	class FlashController : public Controller
	//
	//# \ctor	FlashController(const ColorRGB& color, float init, int32 duration);
	//
	//# \param	color		The brightest color that the light will attain.
	//# \param	init		The initial intensity of the light (in the range 0.0 to 1.0).
	//# \param	duration	The duration of the flash, in milliseconds.
	//
	//# \desc
	//# 
	//
	//# \base	Controller/Controller		A $FlashController$ is a specific type of controller.
	//
	//# \also	$@Math/ColorRGB@$
	
	
	class FlashController : public Controller
	{
		friend class ExtrasPlugin;
		
		private:
			
			ColorRGB		lightColor;
			Interpolator	lightInterpolator;
			
			FlashController();
			FlashController(const FlashController& flashController);
			
			Controller *Replicate(void) const override;
		
		public:
			
			C4EXTRASAPI FlashController(const ColorRGB& color, float init, int32 duration);
			C4EXTRASAPI ~FlashController();
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			void Move(void);
	};
}


#endif

// ZYURVUR
