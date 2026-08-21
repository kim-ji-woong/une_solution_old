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


#include "C4ZoneManipulators.h"
#include "C4VolumeManipulators.h"
#include "C4WorldEditor.h"


using namespace C4;


namespace
{
	const ConstColorRGBA kZoneSelectedColor = {1.0F, 1.0F, 0.25F, 1.0F};
	const ConstColorRGBA kZoneUnselectedColor = {0.5F, 0.5F, 0.0F, 1.0F};
	const ConstColorRGBA kTargetSelectedColor = {0.0F, 1.0F, 0.5F, 1.0F};
	const ConstColorRGBA kTargetUnselectedColor = {0.0F, 0.5F, 0.25F, 1.0F};
	
	
	const TextureHeader zoneTextureHeader =
	{
		kTexture2D,
		kTextureForceHighQuality,
		kTextureSemanticEmission,
		kTextureSemanticNone,
		kTextureL8,
		8, 8, 1,
		{kTextureClamp, kTextureRepeat, kTextureRepeat},
		4
	};
	
	
	const unsigned_int8 zoneTextureImage[85] =
	{
		0x00, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00,
		0x00, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00,
		0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00,
		0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00,
		0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0x00,
		0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00,
		0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00,
		0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00,
		0x00, 0xFF, 0x00, 0x00,
		0x00, 0x00, 0x00, 0x00,
		0x00, 0x00, 0xFF, 0x00,
		0x00, 0xFF, 0xFF, 0x00,
		0xFF, 0x00,
		0x00, 0xFF,
		0x80
	};
}


const ConstPoint3D DomeZoneManipulator::domeVertex[128] =
{
	{0.0F, 1.0F, 0.0F}, {0.0F, 1.0F, 0.0F}, {0.0F, 0.9238795F, 0.3826833F}, {0.0F, 0.9238795F, 0.3826833F},
	{0.0F, 0.9238795F, 0.3826833F}, {0.0F, 0.9238795F, 0.3826833F}, {0.0F, 0.7071067F, 0.7071067F}, {0.0F, 0.7071067F, 0.7071067F},
	{0.0F, 0.7071067F, 0.7071067F}, {0.0F, 0.7071067F, 0.7071067F}, {0.0F, 0.3826833F, 0.9238795F}, {0.0F, 0.3826833F, 0.9238795F},
	{0.0F, 0.3826833F, 0.9238795F}, {0.0F, 0.3826833F, 0.9238795F}, {0.0F, 0.0F, 1.0F}, {0.0F, 0.0F, 1.0F},
	{0.0F, 0.0F, 1.0F}, {0.0F, 0.0F, 1.0F}, {0.0F, -0.3826833F, 0.9238795F}, {0.0F, -0.3826833F, 0.9238795F},
	{0.0F, -0.3826833F, 0.9238795F}, {0.0F, -0.3826833F, 0.9238795F}, {0.0F, -0.7071067F, 0.7071067F}, {0.0F, -0.7071067F, 0.7071067F},
	{0.0F, -0.7071067F, 0.7071067F}, {0.0F, -0.7071067F, 0.7071067F}, {0.0F, -0.9238795F, 0.3826833F}, {0.0F, -0.9238795F, 0.3826833F},
	{0.0F, -0.9238795F, 0.3826833F}, {0.0F, -0.9238795F, 0.3826833F}, {0.0F, -1.0F, 0.0F}, {0.0F, -1.0F, 0.0F},
	
	{-1.0F, 0.0F, 0.0F}, {-1.0F, 0.0F, 0.0F}, {-0.9238795F, 0.0F, 0.3826833F}, {-0.9238795F, 0.0F, 0.3826833F},
	{-0.9238795F, 0.0F, 0.3826833F}, {-0.9238795F, 0.0F, 0.3826833F}, {-0.7071067F, 0.0F, 0.7071067F}, {-0.7071067F, 0.0F, 0.7071067F},
	{-0.7071067F, 0.0F, 0.7071067F}, {-0.7071067F, 0.0F, 0.7071067F}, {-0.3826833F, 0.0F, 0.9238795F}, {-0.3826833F, 0.0F, 0.9238795F},
	{-0.3826833F, 0.0F, 0.9238795F}, {-0.3826833F, 0.0F, 0.9238795F}, {0.0F, 0.0F, 1.0F}, {0.0F, 0.0F, 1.0F},
	{0.0F, 0.0F, 1.0F}, {0.0F, 0.0F, 1.0F}, {0.3826833F, 0.0F, 0.9238795F}, {0.3826833F, 0.0F, 0.9238795F},
	{0.3826833F, 0.0F, 0.9238795F}, {0.3826833F, 0.0F, 0.9238795F}, {0.7071067F, 0.0F, 0.7071067F}, {0.7071067F, 0.0F, 0.7071067F},
	{0.7071067F, 0.0F, 0.7071067F}, {0.7071067F, 0.0F, 0.7071067F}, {0.9238795F, 0.0F, 0.3826833F}, {0.9238795F, 0.0F, 0.3826833F},
	{0.9238795F, 0.0F, 0.3826833F}, {0.9238795F, 0.0F, 0.3826833F}, {1.0F, 0.0F, 0.0F}, {1.0F, 0.0F, 0.0F},
	
	{1.0F, 0.0F, 0.0F}, {1.0F, 0.0F, 0.0F}, {0.9238795F, 0.3826833F, 0.0F}, {0.9238795F, 0.3826833F, 0.0F},
	{0.9238795F, 0.3826833F, 0.0F}, {0.9238795F, 0.3826833F, 0.0F}, {0.7071067F, 0.7071067F, 0.0F}, {0.7071067F, 0.7071067F, 0.0F},
	{0.7071067F, 0.7071067F, 0.0F}, {0.7071067F, 0.7071067F, 0.0F}, {0.3826833F, 0.9238795F, 0.0F}, {0.3826833F, 0.9238795F, 0.0F},
	{0.3826833F, 0.9238795F, 0.0F}, {0.3826833F, 0.9238795F, 0.0F}, {0.0F, 1.0F, 0.0F}, {0.0F, 1.0F, 0.0F},
	{0.0F, 1.0F, 0.0F}, {0.0F, 1.0F, 0.0F}, {-0.3826833F, 0.9238795F, 0.0F}, {-0.3826833F, 0.9238795F, 0.0F},
	{-0.3826833F, 0.9238795F, 0.0F}, {-0.3826833F, 0.9238795F, 0.0F}, {-0.7071067F, 0.7071067F, 0.0F}, {-0.7071067F, 0.7071067F, 0.0F},
	{-0.7071067F, 0.7071067F, 0.0F}, {-0.7071067F, 0.7071067F, 0.0F}, {-0.9238795F, 0.3826833F, 0.0F}, {-0.9238795F, 0.3826833F, 0.0F},
	{-0.9238795F, 0.3826833F, 0.0F}, {-0.9238795F, 0.3826833F, 0.0F}, {-1.0F, 0.0F, 0.0F}, {-1.0F, 0.0F, 0.0F},
	{-1.0F, 0.0F, 0.0F}, {-1.0F, 0.0F, 0.0F}, {-0.9238795F, -0.3826833F, 0.0F}, {-0.9238795F, -0.3826833F, 0.0F},
	{-0.9238795F, -0.3826833F, 0.0F}, {-0.9238795F, -0.3826833F, 0.0F}, {-0.7071067F, -0.7071067F, 0.0F}, {-0.7071067F, -0.7071067F, 0.0F},
	{-0.7071067F, -0.7071067F, 0.0F}, {-0.7071067F, -0.7071067F, 0.0F}, {-0.3826833F, -0.9238795F, 0.0F}, {-0.3826833F, -0.9238795F, 0.0F},
	{-0.3826833F, -0.9238795F, 0.0F}, {-0.3826833F, -0.9238795F, 0.0F}, {0.0F, -1.0F, 0.0F}, {0.0F, -1.0F, 0.0F},
	{0.0F, -1.0F, 0.0F}, {0.0F, -1.0F, 0.0F}, {0.3826833F, -0.9238795F, 0.0F}, {0.3826833F, -0.9238795F, 0.0F},
	{0.3826833F, -0.9238795F, 0.0F}, {0.3826833F, -0.9238795F, 0.0F}, {0.7071067F, -0.7071067F, 0.0F}, {0.7071067F, -0.7071067F, 0.0F},
	{0.7071067F, -0.7071067F, 0.0F}, {0.7071067F, -0.7071067F, 0.0F}, {0.9238795F, -0.3826833F, 0.0F}, {0.9238795F, -0.3826833F, 0.0F},
	{0.9238795F, -0.3826833F, 0.0F}, {0.9238795F, -0.3826833F, 0.0F}, {1.0F, 0.0F, 0.0F}, {1.0F, 0.0F, 0.0F}
};

const ConstVector4D DomeZoneManipulator::domeTangent[128] =
{
	{0.0F, -0.1950902F, 0.9807852F, -1.0F}, {0.0F, -0.1950902F, 0.9807852F, 1.0F}, {0.0F, -0.1950902F, 0.9807852F, 1.0F}, {0.0F, -0.1950902F, 0.9807852F, -1.0F},
	{0.0F, -0.5555702F, 0.8314696F, -1.0F}, {0.0F, -0.5555702F, 0.8314696F, 1.0F}, {0.0F, -0.5555702F, 0.8314696F, 1.0F}, {0.0F, -0.5555702F, 0.8314696F, -1.0F},
	{0.0F, -0.8314696F, 0.5555702F, -1.0F}, {0.0F, -0.8314696F, 0.5555702F, 1.0F}, {0.0F, -0.8314696F, 0.5555702F, 1.0F}, {0.0F, -0.8314696F, 0.5555702F, -1.0F},
	{0.0F, -0.9807851F, 0.1950902F, -1.0F}, {0.0F, -0.9807851F, 0.1950902F, 1.0F}, {0.0F, -0.9807851F, 0.1950902F, 1.0F}, {0.0F, -0.9807851F, 0.1950902F, -1.0F},
	{0.0F, -0.9807851F, -0.1950902F, -1.0F}, {0.0F, -0.9807851F, -0.1950902F, 1.0F}, {0.0F, -0.9807851F, -0.1950902F, 1.0F}, {0.0F, -0.9807851F, -0.1950902F, -1.0F},
	{0.0F, -0.8314696F, -0.5555702F, -1.0F}, {0.0F, -0.8314696F, -0.5555702F, 1.0F}, {0.0F, -0.8314696F, -0.5555702F, 1.0F}, {0.0F, -0.8314696F, -0.5555702F, -1.0F},
	{0.0F, -0.5555702F, -0.8314696F, -1.0F}, {0.0F, -0.5555702F, -0.8314696F, 1.0F}, {0.0F, -0.5555702F, -0.8314696F, 1.0F}, {0.0F, -0.5555702F, -0.8314696F, -1.0F},
	{0.0F, -0.1950902F, -0.9807852F, -1.0F}, {0.0F, -0.1950902F, -0.9807852F, 1.0F}, {0.0F, -0.1950902F, -0.9807852F, 1.0F}, {0.0F, -0.1950902F, -0.9807852F, -1.0F},
	
	{0.1950902F, 0.0F, 0.9807852F, -1.0F}, {0.1950902F, 0.0F, 0.9807852F, 1.0F}, {0.1950902F, 0.0F, 0.9807852F, 1.0F}, {0.1950902F, 0.0F, 0.9807852F, -1.0F},
	{0.5555702F, 0.0F, 0.8314696F, -1.0F}, {0.5555702F, 0.0F, 0.8314696F, 1.0F}, {0.5555702F, 0.0F, 0.8314696F, 1.0F}, {0.5555702F, 0.0F, 0.8314696F, -1.0F},
	{0.8314696F, 0.0F, 0.5555702F, -1.0F}, {0.8314696F, 0.0F, 0.5555702F, 1.0F}, {0.8314696F, 0.0F, 0.5555702F, 1.0F}, {0.8314696F, 0.0F, 0.5555702F, -1.0F}, 
	{0.9807851F, 0.0F, 0.1950902F, -1.0F}, {0.9807851F, 0.0F, 0.1950902F, 1.0F}, {0.9807851F, 0.0F, 0.1950902F, 1.0F}, {0.9807851F, 0.0F, 0.1950902F, -1.0F},
	{0.9807851F, 0.0F, -0.1950902F, -1.0F}, {0.9807851F, 0.0F, -0.1950902F, 1.0F}, {0.9807851F, 0.0F, -0.1950902F, 1.0F}, {0.9807851F, 0.0F, -0.1950902F, -1.0F}, 
	{0.8314696F, 0.0F, -0.5555702F, -1.0F}, {0.8314696F, 0.0F, -0.5555702F, 1.0F}, {0.8314696F, 0.0F, -0.5555702F, 1.0F}, {0.8314696F, 0.0F, -0.5555702F, -1.0F}, 
	{0.5555702F, 0.0F, -0.8314696F, -1.0F}, {0.5555702F, 0.0F, -0.8314696F, 1.0F}, {0.5555702F, 0.0F, -0.8314696F, 1.0F}, {0.5555702F, 0.0F, -0.8314696F, -1.0F}, 
	{0.1950902F, 0.0F, -0.9807852F, -1.0F}, {0.1950902F, 0.0F, -0.9807852F, 1.0F}, {0.1950902F, 0.0F, -0.9807852F, 1.0F}, {0.1950902F, 0.0F, -0.9807852F, -1.0F},
	 
	{-0.1950902F, 0.9807852F, 0.0F, -1.0F}, {-0.1950902F, 0.9807852F, 0.0F, 1.0F}, {-0.1950902F, 0.9807852F, 0.0F, 1.0F}, {-0.1950902F, 0.9807852F, 0.0F, -1.0F},
	{-0.5555702F, 0.8314696F, 0.0F, -1.0F}, {-0.5555702F, 0.8314696F, 0.0F, 1.0F}, {-0.5555702F, 0.8314696F, 0.0F, 1.0F}, {-0.5555702F, 0.8314696F, 0.0F, -1.0F},
	{-0.8314696F, 0.5555702F, 0.0F, -1.0F}, {-0.8314696F, 0.5555702F, 0.0F, 1.0F}, {-0.8314696F, 0.5555702F, 0.0F, 1.0F}, {-0.8314696F, 0.5555702F, 0.0F, -1.0F},
	{-0.9807851F, 0.1950902F, 0.0F, -1.0F}, {-0.9807851F, 0.1950902F, 0.0F, 1.0F}, {-0.9807851F, 0.1950902F, 0.0F, 1.0F}, {-0.9807851F, 0.1950902F, 0.0F, -1.0F}, 
	{-0.9807851F, -0.1950902F, 0.0F, -1.0F}, {-0.9807851F, -0.1950902F, 0.0F, 1.0F}, {-0.9807851F, -0.1950902F, 0.0F, 1.0F}, {-0.9807851F, -0.1950902F, 0.0F, -1.0F},
	{-0.8314696F, -0.5555702F, 0.0F, -1.0F}, {-0.8314696F, -0.5555702F, 0.0F, 1.0F}, {-0.8314696F, -0.5555702F, 0.0F, 1.0F}, {-0.8314696F, -0.5555702F, 0.0F, -1.0F},
	{-0.5555702F, -0.8314696F, 0.0F, -1.0F}, {-0.5555702F, -0.8314696F, 0.0F, 1.0F}, {-0.5555702F, -0.8314696F, 0.0F, 1.0F}, {-0.5555702F, -0.8314696F, 0.0F, -1.0F},
	{-0.1950902F, -0.9807852F, 0.0F, -1.0F}, {-0.1950902F, -0.9807852F, 0.0F, 1.0F}, {-0.1950902F, -0.9807852F, 0.0F, 1.0F}, {-0.1950902F, -0.9807852F, 0.0F, -1.0F}, 
	{0.1950902F, -0.9807852F, 0.0F, -1.0F}, {0.1950902F, -0.9807852F, 0.0F, 1.0F}, {0.1950902F, -0.9807852F, 0.0F, 1.0F}, {0.1950902F, -0.9807852F, 0.0F, -1.0F},
	{0.5555702F, -0.8314696F, 0.0F, -1.0F}, {0.5555702F, -0.8314696F, 0.0F, 1.0F}, {0.5555702F, -0.8314696F, 0.0F, 1.0F}, {0.5555702F, -0.8314696F, 0.0F, -1.0F},
	{0.8314696F, -0.5555702F, 0.0F, -1.0F}, {0.8314696F, -0.5555702F, 0.0F, 1.0F}, {0.8314696F, -0.5555702F, 0.0F, 1.0F}, {0.8314696F, -0.5555702F, 0.0F, -1.0F},
	{0.9807851F, -0.1950902F, 0.0F, -1.0F}, {0.9807851F, -0.1950902F, 0.0F, 1.0F}, {0.9807851F, -0.1950902F, 0.0F, 1.0F}, {0.9807851F, -0.1950902F, 0.0F, -1.0F},
	{0.9807851F, 0.1950902F, 0.0F, -1.0F}, {0.9807851F, 0.1950902F, 0.0F, 1.0F}, {0.9807851F, 0.1950902F, 0.0F, 1.0F}, {0.9807851F, 0.1950902F, 0.0F, -1.0F},
	{0.8314696F, 0.5555702F, 0.0F, -1.0F}, {0.8314696F, 0.5555702F, 0.0F, 1.0F}, {0.8314696F, 0.5555702F, 0.0F, 1.0F}, {0.8314696F, 0.5555702F, 0.0F, -1.0F},
	{0.5555702F, 0.8314696F, 0.0F, -1.0F}, {0.5555702F, 0.8314696F, 0.0F, 1.0F}, {0.5555702F, 0.8314696F, 0.0F, 1.0F}, {0.5555702F, 0.8314696F, 0.0F, -1.0F},
	{0.1950902F, 0.9807852F, 0.0F, -1.0F}, {0.1950902F, 0.9807852F, 0.0F, 1.0F}, {0.1950902F, 0.9807852F, 0.0F, 1.0F}, {0.1950902F, 0.9807852F, 0.0F, -1.0F}
};


ZoneManipulator::ZoneManipulator(Zone *zone) :
		EditorManipulator(zone, "WorldEditor/zone/Box"),
		zoneMaterial(new MaterialObject),
		zoneDiffuseColor(kZoneUnselectedColor, kAttributeMutable),
		zoneTextureMap(&zoneTextureHeader, zoneTextureImage),
		zoneRenderable(kRenderQuads, kRenderDepthTest)
{
	zoneMaterial->AddAttribute(&zoneDiffuseColor);
	zoneMaterial->AddAttribute(&zoneTextureMap);
	zoneMaterial->SetTexcoordOffset(0, Vector2D(0.0F, 1.0F));
	
	zoneRenderable.SetMaterialObjectPointer(&zoneMaterial);
	zoneRenderable.SetAmbientBlendState(BlendState(kBlendOne, kBlendInvSourceAlpha));
	zoneRenderable.SetShaderFlags(kShaderAmbientEffect | kShaderVertexPolyboard | kShaderLinearPolyboard | kShaderOrthoPolyboard | kShaderScaleVertex);
	zoneRenderable.SetRenderParameterPointer(&zoneSizeVector);
	zoneRenderable.SetTransformable(zone);
	
	zoneSizeVector.Set(1.0F, 1.0F, 1.0F, 1.0F);
}

ZoneManipulator::~ZoneManipulator()
{
}

ZoneManipulator *ZoneManipulator::Construct(Zone *zone)
{
	switch (zone->GetZoneType())
	{
		case kZoneInfinite:
			
			return (new InfiniteZoneManipulator(static_cast<InfiniteZone *>(zone)));
		
		case kZoneBox:
			
			return (new BoxZoneManipulator(static_cast<BoxZone *>(zone)));
		
		case kZoneCylinder:
			
			return (new CylinderZoneManipulator(static_cast<CylinderZone *>(zone)));
		
		case kZoneDome:
			
			return (new DomeZoneManipulator(static_cast<DomeZone *>(zone)));
		
		case kZonePolygon:
			
			return (new PolygonZoneManipulator(static_cast<PolygonZone *>(zone)));
	}
	
	return (new ZoneManipulator(zone));
}

const char *ZoneManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', kNodeZone, GetTargetNode()->GetZoneType())));
}

void ZoneManipulator::SetTarget(bool target)
{
	unsigned_int32 state = GetManipulatorState();
	if (target) state |= kManipulatorTarget;
	else state &= ~kManipulatorTarget;
	SetManipulatorState(state);
	
	if (Selected())
	{
		if (target) zoneDiffuseColor.SetDiffuseColor(kTargetSelectedColor);
		else zoneDiffuseColor.SetDiffuseColor(kZoneSelectedColor);
	}
	else
	{
		if (target) zoneDiffuseColor.SetDiffuseColor(kTargetUnselectedColor);
		else zoneDiffuseColor.SetDiffuseColor(kZoneUnselectedColor);
	}
}

void ZoneManipulator::Invalidate(void)
{
	EditorManipulator::Invalidate();
	
	Zone *zone = GetTargetNode();
	zone->InvalidateLightRegions();
	zone->ProcessTransitions();
}

void ZoneManipulator::Select(void)
{
	EditorManipulator::Select();
	
	if (GetManipulatorState() & kManipulatorTarget) zoneDiffuseColor.SetDiffuseColor(kTargetSelectedColor);
	else zoneDiffuseColor.SetDiffuseColor(kZoneSelectedColor);
}

void ZoneManipulator::Unselect(void)
{
	EditorManipulator::Unselect();
	
	if (GetManipulatorState() & kManipulatorTarget) zoneDiffuseColor.SetDiffuseColor(kTargetUnselectedColor);
	else zoneDiffuseColor.SetDiffuseColor(kZoneUnselectedColor);
}

void ZoneManipulator::HandleDelete(bool undoable)
{
	EditorManipulator::HandleDelete(undoable);
	
	Editor *editor = GetEditor();
	if (editor->GetTargetZone() == GetTargetNode()) editor->SetTargetZone(nullptr);
}

bool ZoneManipulator::Pick(const Ray *ray, PickData *data) const
{
	if (zoneRenderable.AttributeArrayEnabled(kArrayVertex))
	{
		const Point3D *vertex = zoneRenderable.GetAttributeArray<Point3D>(kArrayVertex);
		
		float r = (ray->radius != 0.0F) ? ray->radius : Editor::kFrustumRenderScale;
		float r2 = r * r * 16.0F;
		
		int32 vertexCount = zoneRenderable.GetVertexCount();
		const Vector3D& size = zoneSizeVector.GetVector3D();
		
		for (machine a = 0; a < vertexCount; a += 4)
		{
			if (PickLineSegment(ray, vertex[0] & size, vertex[2] & size, r2, &data->rayParam)) return (true);
			vertex += 4;
		}
	}
	
	return (false);
}

bool ZoneManipulator::RegionPick(const Region *region) const
{
	if (zoneRenderable.AttributeArrayEnabled(kArrayVertex))
	{
		const Point3D *vertex = zoneRenderable.GetAttributeArray<Point3D>(kArrayVertex);
		
		int32 vertexCount = zoneRenderable.GetVertexCount();
		const Vector3D& size = zoneSizeVector.GetVector3D();
		
		for (machine a = 0; a < vertexCount; a += 4)
		{
			if (RegionPickLineSegment(region, vertex[0] & size, vertex[2] & size)) return (true);
			vertex += 4;
		}
	}
	
	return (false);
}

void ZoneManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated))
	{
		if (zoneRenderable.AttributeArrayEnabled(kArrayVertex))
		{
			const Point3D *vertex = zoneRenderable.GetAttributeArray<Point3D>(kArrayVertex);
			Point2D *texcoord = const_cast<Point2D *>(zoneRenderable.GetAttributeArray<Point2D>(kArrayTexture0));
			
			int32 vertexCount = zoneRenderable.GetVertexCount();
			const Vector3D& size = zoneSizeVector.GetVector3D();
			
			for (machine a = 0; a < vertexCount; a += 4)
			{
				float dp = Magnitude((vertex[2] & size) - (vertex[0] & size));
				
				texcoord[0].Set(0.0F, 0.0F);
				texcoord[1].Set(1.0F, 0.0F);
				texcoord[2].Set(1.0F, dp);
				texcoord[3].Set(0.0F, dp);
				
				vertex += 4;
				texcoord += 4;
			}
		}
	}
	
	EditorManipulator::Update();
}

void ZoneManipulator::Render(const ManipulatorRenderData *renderData)
{
	List<Renderable> *renderList = renderData->manipulatorList;
	if ((renderList) && (zoneRenderable.AttributeArrayEnabled(kArrayVertex)))
	{
		float scale = renderData->viewportScale;
		zoneSizeVector.w = scale * 3.0F;
		zoneMaterial->SetTexcoordScale(0, Vector2D(1.0F, 0.125F / scale));
		renderList->Append(&zoneRenderable);
	}
	
	EditorManipulator::Render(renderData);
}


InfiniteZoneManipulator::InfiniteZoneManipulator(InfiniteZone *infinite) : ZoneManipulator(infinite)
{
	SetManipulatorState(kManipulatorHidden);
	SetManipulatorFlags(kManipulatorLockedTransform);
	
	Renderable *zoneRenderable = GetZoneRenderable();
	zoneRenderable->SetVertexCount(48);
	zoneRenderable->SetAttributeArray(kArrayVertex, boxVertex);
	zoneRenderable->SetAttributeArray(kArrayTangent, &BoxVolumeManipulator::outlineTangent[0]);
	zoneRenderable->SetAttributeArray(kArrayTexture0, boxTexcoord);
}

InfiniteZoneManipulator::~InfiniteZoneManipulator()
{
}

bool InfiniteZoneManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	const Box3D& box = GetObject()->GetZoneBox();
	sphere->SetCenter(box.GetCenter());
	sphere->SetRadius(Magnitude(box.GetSize()) * 0.5F);
	return (true);
}

Box3D InfiniteZoneManipulator::CalculateNodeBoundingBox(void) const
{
	return (GetObject()->GetZoneBox());
}

void InfiniteZoneManipulator::HandleSizeUpdate(int32 count, const float *size)
{
	float	objectSize[6];
	
	for (machine a = 0; a < 3; a++)
	{
		float f = size[a];
		objectSize[a] = f;
		objectSize[a + 3] = Fmax(size[a + 3], f + 1.0F);
	}
	
	Node *node = GetTargetNode();
	node->GetObject()->SetObjectSize(objectSize);
	GetEditor()->InvalidateNode(node);
}

int32 InfiniteZoneManipulator::GetHandleTable(Point3D *handle) const
{
	const Box3D& box = GetObject()->GetZoneBox();
	Vector3D center = box.GetCenter();
	
	handle[0] = box.min;
	handle[1].Set(center.x, box.min.y, box.min.z);
	handle[2].Set(box.max.x, box.min.y, box.min.z);
	handle[3].Set(box.max.x, center.y, box.min.z);
	handle[4].Set(box.max.x, box.max.y, box.min.z);
	handle[5].Set(center.x, box.max.y, box.min.z);
	handle[6].Set(box.min.x, box.max.y, box.min.z);
	handle[7].Set(box.min.x, center.y, box.min.z);
	
	handle[8].Set(box.min.x, box.min.y, box.max.z);
	handle[9].Set(center.x, box.min.y, box.max.z);
	handle[10].Set(box.max.x, box.min.y, box.max.z);
	handle[11].Set(box.max.x, center.y, box.max.z);
	handle[12] = box.max;
	handle[13].Set(center.x, box.max.y, box.max.z);
	handle[14].Set(box.min.x, box.max.y, box.max.z);
	handle[15].Set(box.min.x, center.y, box.max.z);
	
	handle[16].Set(box.min.x, box.min.y, center.z);
	handle[17].Set(box.max.x, box.min.y, center.z);
	handle[18].Set(box.max.x, box.max.y, center.z);
	handle[19].Set(box.min.x, box.max.y, center.z);
	
	return (20);
}

void InfiniteZoneManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	BoxVolumeManipulator::GetHandleData(index, handleData);
}

void InfiniteZoneManipulator::BeginResize(const ManipulatorResizeData *resizeData)
{
	EditorManipulator::BeginResize(resizeData);
	originalZoneBox = GetObject()->GetZoneBox();
}

bool InfiniteZoneManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	Box3D box = originalZoneBox;
	
	float dx = resizeData->resizeDelta.x;
	float dy = resizeData->resizeDelta.y;
	float dz = resizeData->resizeDelta.z;
	
	unsigned_int32 handleFlags = resizeData->handleFlags;
	
	if (handleFlags & kManipulatorHandlePositiveX)
	{
		box.max.x = Fmax(box.max.x + dx, box.min.x + 1.0F);
	}
	else if (handleFlags & kManipulatorHandleNegativeX)
	{
		box.min.x = Fmin(box.min.x + dx, box.max.x - 1.0F);
	}
	
	if (handleFlags & kManipulatorHandlePositiveY)
	{
		box.max.y = Fmax(box.max.y + dy, box.min.y + 1.0F);
	}
	else if (handleFlags & kManipulatorHandleNegativeY)
	{
		box.min.y = Fmin(box.min.y + dy, box.max.y - 1.0F);
	}
	
	if (handleFlags & kManipulatorHandlePositiveZ)
	{
		box.max.z = Fmax(box.max.z + dz, box.min.z + 1.0F);
	}
	else if (handleFlags & kManipulatorHandleNegativeZ)
	{
		box.min.z = Fmin(box.min.z + dz, box.max.z - 1.0F);
	}
	
	GetObject()->SetZoneBox(box);
	return (false);
}

void InfiniteZoneManipulator::Update(void)
{
	unsigned_int32 state = GetManipulatorState();
	if (!(state & kManipulatorUpdated))
	{
		const Box3D& box = GetObject()->GetZoneBox();
		Vector3D size = box.GetSize();
		
		const Point3D *outlineVertex = &BoxVolumeManipulator::outlineVertex[0];
		for (machine a = 0; a < 48; a++) boxVertex[a] = box.min + (outlineVertex[a] & size);
	}
	
	ZoneManipulator::Update();
}


BoxZoneManipulator::BoxZoneManipulator(BoxZone *box) : ZoneManipulator(box)
{
	Renderable *zoneRenderable = GetZoneRenderable();
	zoneRenderable->SetVertexCount(48);
	zoneRenderable->SetAttributeArray(kArrayVertex, &BoxVolumeManipulator::outlineVertex[0]);
	zoneRenderable->SetAttributeArray(kArrayTangent, &BoxVolumeManipulator::outlineTangent[0]);
	zoneRenderable->SetAttributeArray(kArrayTexture0, boxTexcoord);
}

BoxZoneManipulator::~BoxZoneManipulator()
{
}

bool BoxZoneManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	BoxVolumeManipulator::CalculateVolumeSphere(GetObject()->GetBoxSize(), sphere);
	return (true);
}

Box3D BoxZoneManipulator::CalculateNodeBoundingBox(void) const
{
	const Vector3D& size = GetObject()->GetBoxSize();
	return (Box3D(Zero3D, Zero3D + size));
}

int32 BoxZoneManipulator::GetHandleTable(Point3D *handle) const
{
	return (BoxVolumeManipulator::GetHandleTable(GetObject()->GetBoxSize(), handle));
}

void BoxZoneManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	BoxVolumeManipulator::GetHandleData(index, handleData);
}

bool BoxZoneManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	BoxZoneObject *object = GetObject();
	
	Vector3D newSize = object->GetBoxSize();
	const Vector3D *oldSize = reinterpret_cast<const Vector3D *>(GetOriginalSize());
	bool move = BoxVolumeManipulator::Resize(resizeData, *oldSize, newSize);
	
	object->SetBoxSize(newSize);
	return (move);
}

void BoxZoneManipulator::Update(void)
{
	const Vector3D& size = GetObject()->GetBoxSize();
	SetZoneSize(size.x, size.y, size.z);
	
	ZoneManipulator::Update();
}


CylinderZoneManipulator::CylinderZoneManipulator(CylinderZone *cylinder) : ZoneManipulator(cylinder)
{
	Renderable *zoneRenderable = GetZoneRenderable();
	zoneRenderable->SetVertexCount(144);
	zoneRenderable->SetAttributeArray(kArrayVertex, &CylinderVolumeManipulator::outlineVertex[0]);
	zoneRenderable->SetAttributeArray(kArrayTangent, &CylinderVolumeManipulator::outlineTangent[0]);
	zoneRenderable->SetAttributeArray(kArrayTexture0, cylinderTexcoord);
}

CylinderZoneManipulator::~CylinderZoneManipulator()
{
}

bool CylinderZoneManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	const CylinderZoneObject *object = GetObject();
	CylinderVolumeManipulator::CalculateVolumeSphere(object->GetCylinderSize(), object->GetCylinderHeight(), sphere);
	return (true);
}

Box3D CylinderZoneManipulator::CalculateNodeBoundingBox(void) const
{
	const CylinderZoneObject *object = GetObject();
	const Vector2D& size = object->GetCylinderSize();
	float height = object->GetCylinderHeight();
	return (Box3D(Point3D(-size, 0.0F), Point3D(size, height)));
}

int32 CylinderZoneManipulator::GetHandleTable(Point3D *handle) const
{
	const CylinderZoneObject *object = GetObject();
	return (CylinderVolumeManipulator::GetHandleTable(GetObject()->GetCylinderSize(), object->GetCylinderHeight(), handle));
}

void CylinderZoneManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	CylinderVolumeManipulator::GetHandleData(index, handleData);
}

bool CylinderZoneManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	CylinderZoneObject *object = GetObject();
	
	Vector2D newSize = object->GetCylinderSize();
	float newHeight = object->GetCylinderHeight();
	const Vector2D *oldSize = reinterpret_cast<const Vector2D *>(GetOriginalSize());
	float oldHeight = GetOriginalSize()[2];
	bool move = CylinderVolumeManipulator::Resize(resizeData, *oldSize, oldHeight, newSize, newHeight);
	
	object->SetCylinderSize(newSize);
	object->SetCylinderHeight(newHeight);
	return (move);
}

void CylinderZoneManipulator::Update(void)
{
	CylinderZoneObject *object = GetObject();
	const Vector2D& size = object->GetCylinderSize();
	SetZoneSize(size.x, size.y, object->GetCylinderHeight());
	
	ZoneManipulator::Update();
}


DomeZoneManipulator::DomeZoneManipulator(DomeZone *dome) : ZoneManipulator(dome)
{
	Renderable *zoneRenderable = GetZoneRenderable();
	zoneRenderable->SetVertexCount(128);
	zoneRenderable->SetAttributeArray(kArrayVertex, &domeVertex[0]);
	zoneRenderable->SetAttributeArray(kArrayTangent, &domeTangent[0]);
	zoneRenderable->SetAttributeArray(kArrayTexture0, domeTexcoord);
}

DomeZoneManipulator::~DomeZoneManipulator()
{
}

bool DomeZoneManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	const Vector3D& domeSize = GetObject()->GetDomeSize();
	
	sphere->SetCenter(Zero3D);
	sphere->SetRadius(Fmax(domeSize.x, domeSize.y, domeSize.z));
	return (true);
}

Box3D DomeZoneManipulator::CalculateNodeBoundingBox(void) const
{
	const DomeZoneObject *object = GetObject();
	const Vector3D& size = object->GetDomeSize();
	return (Box3D(Point3D(-size.x, -size.y, 0.0F), Zero3D + size));
}

int32 DomeZoneManipulator::GetHandleTable(Point3D *handle) const
{
	const Vector3D& domeSize = GetObject()->GetDomeSize();
	float x = domeSize.x;
	float y = domeSize.y;
	float z = domeSize.z;
	
	handle[0].Set(-x, -y, 0.0F);
	handle[1].Set(0.0F, -y, 0.0F);
	handle[2].Set(x, -y, 0.0F);
	handle[3].Set(x, 0.0F, 0.0F);
	handle[4].Set(x, y, 0.0F);
	handle[5].Set(0.0F, y, 0.0F);
	handle[6].Set(-x, y, 0.0F);
	handle[7].Set(-x, 0.0F, 0.0F);
	
	handle[8].Set(-x, -y, z);
	handle[9].Set(0.0F, -y, z);
	handle[10].Set(x, -y, z);
	handle[11].Set(x, 0.0F, z);
	handle[12].Set(x, y, z);
	handle[13].Set(0.0F, y, z);
	handle[14].Set(-x, y, z);
	handle[15].Set(-x, 0.0F, z);
	
	return (16);
}

void DomeZoneManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	static const unsigned_int32 handleFlags[16] =
	{
		kManipulatorHandleNegativeX | kManipulatorHandleNegativeY | kManipulatorHandleNegativeZ,
		kManipulatorHandleNegativeY | kManipulatorHandleNegativeZ,
		kManipulatorHandlePositiveX | kManipulatorHandleNegativeY | kManipulatorHandleNegativeZ,
		kManipulatorHandlePositiveX | kManipulatorHandleNegativeZ,
		kManipulatorHandlePositiveX | kManipulatorHandlePositiveY | kManipulatorHandleNegativeZ,
		kManipulatorHandlePositiveY | kManipulatorHandleNegativeZ,
		kManipulatorHandleNegativeX | kManipulatorHandlePositiveY | kManipulatorHandleNegativeZ,
		kManipulatorHandleNegativeX | kManipulatorHandleNegativeZ,
		kManipulatorHandleNegativeX | kManipulatorHandleNegativeY | kManipulatorHandlePositiveZ,
		kManipulatorHandleNegativeY | kManipulatorHandlePositiveZ,
		kManipulatorHandlePositiveX | kManipulatorHandleNegativeY | kManipulatorHandlePositiveZ,
		kManipulatorHandlePositiveX | kManipulatorHandlePositiveZ,
		kManipulatorHandlePositiveX | kManipulatorHandlePositiveY | kManipulatorHandlePositiveZ,
		kManipulatorHandlePositiveY | kManipulatorHandlePositiveZ,
		kManipulatorHandleNegativeX | kManipulatorHandlePositiveY | kManipulatorHandlePositiveZ,
		kManipulatorHandleNegativeX | kManipulatorHandlePositiveZ
	};
	
	handleData->handleFlags = handleFlags[index];
	handleData->oppositeIndex = index ^ 12;
}

bool DomeZoneManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	DomeZoneObject *object = GetObject();
	Vector3D newDomeSize = object->GetDomeSize();
	const Vector3D *oldDomeSize = reinterpret_cast<const Vector3D *>(GetOriginalSize());
	
	float dx = resizeData->resizeDelta.x;
	float dy = resizeData->resizeDelta.y;
	float dz = resizeData->resizeDelta.z;
	
	unsigned_int32 handleFlags = resizeData->handleFlags;
	
	if (resizeData->resizeFlags & kManipulatorResizeCenter)
	{
		if (handleFlags & kManipulatorHandleNonzeroX)
		{
			if (handleFlags & kManipulatorHandleNegativeX) dx = -dx;
			newDomeSize.x = Fmax(oldDomeSize->x + dx, kSizeEpsilon);
		}
		
		if (handleFlags & kManipulatorHandleNonzeroY)
		{
			if (handleFlags & kManipulatorHandleNegativeY) dy = -dy;
			newDomeSize.y = Fmax(oldDomeSize->y + dy, kSizeEpsilon);
		}
	}
	else
	{
		if (handleFlags & kManipulatorHandleNonzeroX)
		{
			dx *= 0.5F;
			
			if (handleFlags & kManipulatorHandleNegativeX)
			{
				newDomeSize.x = Fmax(oldDomeSize->x - dx, kSizeEpsilon);
				resizeData->positionOffset.x = oldDomeSize->x - newDomeSize.x;
			}
			else
			{
				newDomeSize.x = Fmax(oldDomeSize->x + dx, kSizeEpsilon);
				resizeData->positionOffset.x = newDomeSize.x - oldDomeSize->x;
			}
		}
		
		if (handleFlags & kManipulatorHandleNonzeroY)
		{
			dy *= 0.5F;
			
			if (handleFlags & kManipulatorHandleNegativeY)
			{
				newDomeSize.y = Fmax(oldDomeSize->y - dy, kSizeEpsilon);
				resizeData->positionOffset.y = oldDomeSize->y - newDomeSize.y;
			}
			else
			{
				newDomeSize.y = Fmax(oldDomeSize->y + dy, kSizeEpsilon);
				resizeData->positionOffset.y = newDomeSize.y - oldDomeSize->y;
			}
		}
	}
	
	if (handleFlags & kManipulatorHandlePositiveZ)
	{
		newDomeSize.z = Fmax(oldDomeSize->z + dz, kSizeEpsilon);
	}
	else if (handleFlags & kManipulatorHandleNegativeZ)
	{
		newDomeSize.z = Fmax(oldDomeSize->z - dz, kSizeEpsilon);
		resizeData->positionOffset.z = oldDomeSize->z - newDomeSize.z;
	}
	
	object->SetDomeSize(newDomeSize);
	return (true);
}

void DomeZoneManipulator::Update(void)
{
	const Vector3D& size = GetObject()->GetDomeSize();
	SetZoneSize(size.x, size.y, size.z);
	
	ZoneManipulator::Update();
}


PolygonZoneManipulator::PolygonZoneManipulator(PolygonZone *polygon) : ZoneManipulator(polygon)
{
	Renderable *zoneRenderable = GetZoneRenderable();
	zoneRenderable->SetAttributeArray(kArrayVertex, polygonVertex);
	zoneRenderable->SetAttributeArray(kArrayTangent, polygonTangent);
	zoneRenderable->SetAttributeArray(kArrayTexture0, polygonTexcoord);
	
	SetZoneSize(1.0F, 1.0F, 1.0F);
}

PolygonZoneManipulator::~PolygonZoneManipulator()
{
}

bool PolygonZoneManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	const PolygonZoneObject *object = GetObject();
	
	const Point3D *vertex = object->GetVertexArray();
	float xmin = vertex->x;
	float ymin = vertex->y;
	float xmax = xmin;
	float ymax = ymin;
	
	int32 vertexCount = object->GetVertexCount();
	for (machine a = 1; a < vertexCount; a++)
	{
		const Point3D& p = vertex[a];
		float x = p.x;
		float y = p.y;
		
		xmin = Fmin(xmin, x);
		xmax = Fmax(xmax, x);
		ymin = Fmin(ymin, y);
		ymax = Fmax(ymax, y);
	}
	
	xmin *= 0.5F;
	ymin *= 0.5F;
	xmax *= 0.5F;
	ymax *= 0.5F;
	
	float h = object->GetPolygonHeight() * 0.5F;
	sphere->SetCenter(xmin + xmax, ymin + ymax, h);
	
	float x = xmax - xmin;
	float y = ymax - ymin;
	sphere->SetRadius(Sqrt(x * x + y * y + h * h));
	return (true);
}

Box3D PolygonZoneManipulator::CalculateNodeBoundingBox(void) const
{
	const PolygonZoneObject *object = GetObject();
	int32 vertexCount = object->GetVertexCount();
	const Point3D *vertex = object->GetVertexArray();
	
	Box3D box(vertex[0], vertex[0]);
	for (machine a = 1; a < vertexCount; a++) box.Union(vertex[a]);
	
	box.max.z = object->GetPolygonHeight();
	return (box);
}

int32 PolygonZoneManipulator::GetHandleTable(Point3D *handle) const
{
	const PolygonZoneObject *object = GetObject();
	int32 vertexCount = object->GetVertexCount();
	const Point3D *vertex = object->GetVertexArray();
	
	float x = 0.0F;
	float y = 0.0F;
	
	for (machine a = 0; a < vertexCount; a++)
	{
		handle[a] = vertex[a];
		x += vertex[a].x;
		y += vertex[a].y;
	}
	
	float f = 1.0F / (float) vertexCount;
	handle[vertexCount].Set(x * f, y * f, object->GetPolygonHeight());
	return (vertexCount + 1);
}

void PolygonZoneManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	const PolygonZoneObject *object = GetObject();
	int32 vertexCount = object->GetVertexCount();
	if (index < vertexCount)
	{
		const Point3D *vertex = object->GetVertexArray();
		
		int32 opposite = index;
		float distance = 0.0F;
		
		const Point3D& p = vertex[index];
		for (machine a = 0; a < vertexCount; a++) if (a != index)
		{
			float d = SquaredMag(vertex[a] - p);
			if (d > distance)
			{
				distance = d;
				opposite = (int32) a;
			}
		}
		
		handleData->handleFlags = 0;
		handleData->oppositeIndex = opposite;
	}
	else
	{
		handleData->handleFlags = kManipulatorHandlePositiveZ;
		handleData->oppositeIndex = kHandleOrigin;
	}
}

void PolygonZoneManipulator::BeginResize(const ManipulatorResizeData *resizeData)
{
	EditorManipulator::BeginResize(resizeData);
	
	const PolygonZoneObject *object = GetObject();
	const Point3D *vertex = object->GetVertexArray();
	originalVertexPosition = vertex[resizeData->handleIndex];
}

Point3D PolygonZoneManipulator::ConstrainVertex(const Point3D& original, const Point3D& current, const Point3D& v1, const Point3D& v2)
{
	float x = current.x;
	float y = current.y;
	
	Vector2D dv(v1.x - v2.x, v1.y - v2.y);
	if ((x - v2.x) * dv.y - (y - v2.y) * dv.x < 0.0F)
	{
		float dx = x - original.x;
		float dy = y - original.y;
		
		Vector3D plane(dv.y, -dv.x, dv.x * v1.y - dv.y * v1.x);
		float t = -(plane.x * original.x + plane.y * original.y + plane.z) / (plane.x * dx + plane.y * dy);
		x = original.x + t * dx;
		y = original.y + t * dy;
	}
	
	return (Point3D(x, y, 0.0F));
}

bool PolygonZoneManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	PolygonZoneObject *object = GetObject();
	
	if (resizeData->handleFlags == 0)
	{
		Zone *zone = GetTargetNode();
		Point3D p = zone->GetWorldTransform() * (originalVertexPosition + resizeData->resizeDelta);
		p = zone->GetInverseWorldTransform() * GetEditor()->SnapToGrid(p);
		
		Point3D *vertex = object->GetVertexArray();
		
		int32 count = object->GetVertexCount();
		int32 index = resizeData->handleIndex;
		
		const Point3D *v1 = &vertex[(index != count - 1) ? index + 1 : 0];
		const Point3D *v2 = &vertex[(index != 0) ? index - 1 : count - 1];
		p = ConstrainVertex(originalVertexPosition, p, *v1, *v2);
		
		if (count > 3)
		{
			const Point3D *v3 = &vertex[(index < count - 2) ? index + 2 : index + 2 - count];
			p = ConstrainVertex(originalVertexPosition, p, *v1, *v3);
			
			v3 = &vertex[(index > 1) ? index - 2 : index - 2 + count];
			p = ConstrainVertex(originalVertexPosition, p, *v3, *v2);
		}
		
		vertex[index] = p;
	}
	else
	{
		float oldPolygonHeight = GetOriginalSize()[0];
		float dz = resizeData->resizeDelta.z;
		object->SetPolygonHeight(Fmax(oldPolygonHeight + dz, kSizeEpsilon));
	}
	
	return (false);
}

bool PolygonZoneManipulator::Pick(const Ray *ray, PickData *data) const
{
	const Renderable *zoneRenderable = GetZoneRenderable();
	if (zoneRenderable->AttributeArrayEnabled(kArrayVertex))
	{
		const Point3D *vertex = GetZoneRenderable()->GetAttributeArray<Point3D>(kArrayVertex);
		
		float r = (ray->radius != 0.0F) ? ray->radius : Editor::kFrustumRenderScale;
		float r2 = r * r * 16.0F;
		
		int32 vertexCount = GetZoneRenderable()->GetVertexCount();
		const Vector3D& size = GetZoneSize().GetVector3D();
		
		for (machine a = 0; a < vertexCount; a += 4)
		{
			if (PickLineSegment(ray, vertex[0] & size, vertex[2] & size, r2, &data->rayParam))
			{
				a >>= 2;
				int32 index = -1;
				if (a % 3 != 2) index = a / 3;
				
				data->pickIndex[0] = index;
				data->pickPoint = ray->origin + data->rayParam * ray->direction;
				return (true);
			}
			
			vertex += 4;
		}
	}
	
	return (false);
}

void PolygonZoneManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated))
	{
		PolygonZoneObject *object = GetObject();
		Renderable *zoneRenderable = GetZoneRenderable();
		
		int32 count = object->GetVertexCount();
		zoneRenderable->SetVertexCount(count * 12);
		const Point3D *vertex = object->GetVertexArray();
		float height = object->GetPolygonHeight();
		
		Point3D *zoneVertex = const_cast<Point3D *>(zoneRenderable->GetAttributeArray<Point3D>(kArrayVertex));
		Vector4D *zoneTangent = const_cast<Vector4D *>(zoneRenderable->GetAttributeArray<Vector4D>(kArrayTangent));
		
		const Point3D *p1 = &vertex[count - 1];
		for (machine a = 0; a < count; a++)
		{
			const Point3D *p2 = &vertex[a];
			Vector3D tangent = (*p2 - *p1).Normalize();
			
			zoneVertex[0].Set(p1->x, p1->y, 0.0F);
			zoneVertex[1].Set(p1->x, p1->y, 0.0F);
			zoneVertex[2].Set(p2->x, p2->y, 0.0F);
			zoneVertex[3].Set(p2->x, p2->y, 0.0F);
			zoneVertex[4].Set(p1->x, p1->y, height);
			zoneVertex[5].Set(p1->x, p1->y, height);
			zoneVertex[6].Set(p2->x, p2->y, height);
			zoneVertex[7].Set(p2->x, p2->y, height);
			zoneVertex[8].Set(p1->x, p1->y, 0.0F);
			zoneVertex[9].Set(p1->x, p1->y, 0.0F);
			zoneVertex[10].Set(p1->x, p1->y, height);
			zoneVertex[11].Set(p1->x, p1->y, height);
			
			zoneTangent[0].Set(tangent, -1.0F);
			zoneTangent[1].Set(tangent, 1.0F);
			zoneTangent[2].Set(tangent, 1.0F);
			zoneTangent[3].Set(tangent, -1.0F);
			zoneTangent[4].Set(tangent, -1.0F);
			zoneTangent[5].Set(tangent, 1.0F);
			zoneTangent[6].Set(tangent, 1.0F);
			zoneTangent[7].Set(tangent, -1.0F);
			zoneTangent[8].Set(0.0F, 0.0F, 1.0F, -1.0F);
			zoneTangent[9].Set(0.0F, 0.0F, 1.0F, 1.0F);
			zoneTangent[10].Set(0.0F, 0.0F, 1.0F, 1.0F);
			zoneTangent[11].Set(0.0F, 0.0F, 1.0F, -1.0F);
			
			zoneVertex += 12;
			zoneTangent += 12;
			p1 = p2;
		}
	}
	
	ZoneManipulator::Update();
}

// ZYURVUR
