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


#include "C4FlashController.h"
#include "C4Lights.h"


using namespace C4;


FlashController::FlashController() : Controller(kControllerFlash)
{
}

FlashController::FlashController(const ColorRGB& color, float init, int32 duration) :
		Controller(kControllerFlash),
		lightInterpolator(init, (2.0F - init) / (float) duration, kInterpolatorForward | kInterpolatorOscillate)
{
	lightColor = color;
}

FlashController::FlashController(const FlashController& flashController) :
		Controller(flashController),
		lightInterpolator(flashController.lightInterpolator.GetValue(), flashController.lightInterpolator.GetRate(), kInterpolatorForward | kInterpolatorOscillate)
{
	lightColor = flashController.lightColor;
}

FlashController::~FlashController()
{
}

Controller *FlashController::Replicate(void) const
{
	return (new FlashController(*this));
}

void FlashController::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Controller::Pack(data, packFlags);
	
	data << ChunkHeader('COLR', sizeof(ColorRGB));
	data << lightColor;
	
	PackHandle handle = data.BeginChunk('FLSH');
	lightInterpolator.Pack(data, packFlags);
	data.EndChunk(handle);
	
	data << TerminatorChunk;
}

void FlashController::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Controller::Unpack(data, unpackFlags);
	UnpackChunkList<FlashController>(data, unpackFlags);
}

bool FlashController::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'COLR':
			
			data >> lightColor;
			return (true);
		
		case 'FLSH':
			
			lightInterpolator.Unpack(data, unpackFlags);
			return (true);
	}
	
	return (false);
}

void FlashController::Move(void)
{
	const Light *light = static_cast<Light *>(GetTargetNode());
	
	float value = lightInterpolator.UpdateValue();
	if (lightInterpolator.GetMode() != kInterpolatorStop)
	{
		light->GetObject()->SetLightColor(lightColor * value);
	}
	else
	{
		delete light;
	}
}

// ZYURVUR
