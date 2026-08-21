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


#include "C4GeometryManipulators.h"
#include "C4CameraManipulators.h"
#include "C4LightManipulators.h"
#include "C4SourceManipulators.h"
#include "C4ZoneManipulators.h"
#include "C4PortalManipulators.h"
#include "C4SpaceManipulators.h"
#include "C4MarkerManipulators.h"
#include "C4TriggerManipulators.h"
#include "C4EffectManipulators.h"
#include "C4EmitterManipulators.h"
#include "C4InstanceManipulators.h"
#include "C4ModelManipulators.h"
#include "C4PhysicsManipulators.h"
#include "C4TerrainTools.h"
#include "C4WaterTools.h"
#include "C4WorldEditor.h"
#include "C4EditorSupport.h"


using namespace C4;


namespace
{
	const float kGizmoThreshold = 0.95F;
	
	
	const ConstColorRGBA kDefaultHandleColor = {0.25F, 1.0F, 0.25F, 0.75F};
	const ConstColorRGBA kUnselectedMarkerColor = {0.5F, 0.5F, 0.5F, 1.0F};
	const ConstColorRGBA kConnectorBackgroundColor = {0.125F, 0.125F, 0.125F, 1.0F};
	const ConstColorRGBA kConnectorLineColor = {1.0F, 0.75F, 0.25F, 1.0F};
	const ConstColorRGBA kGroupOutlineColor = {0.0F, 0.5F, 0.25F, 1.0F};
	
	
	const TextureHeader boxEdgeTextureHeader =
	{
		kTexture2D,
		kTextureForceHighQuality,
		kTextureSemanticDiffuse,
		kTextureSemanticTransparency,
		kTextureLA8,
		8, 1, 1,
		{kTextureClamp, kTextureRepeat, kTextureClamp},
		1
	};
	
	
	const TextureHeader boxHiliteTextureHeader =
	{
		kTexture2D,
		kTextureForceHighQuality,
		kTextureSemanticDiffuse,
		kTextureSemanticTransparency,
		kTextureL8,
		8, 1, 1,
		{kTextureClamp, kTextureRepeat, kTextureClamp},
		1
	};
	
	
	const unsigned_int8 boxEdgeTextureImage[16] =
	{
		0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00
	};
	
	
	const unsigned_int8 boxHiliteTextureImage[8] =
	{
		0x00, 0x55, 0xAA, 0xFF, 0xFF, 0xAA, 0x55, 0x00
	};
}


const ConstPoint3D EditorGizmo::gizmoVertex[32] =
{
	{0.0F, 0.0F, 0.0F}, {0.0F, 0.0F, 0.0F}, {40.0F, 0.0F, 0.0F}, {40.0F, 0.0F, 0.0F},
	{40.0F, 0.0F, 0.0F}, {40.0F, 0.0F, 0.0F}, {56.0F, 0.0F, 0.0F}, {56.0F, 0.0F, 0.0F},
	{0.0F, 0.0F, 0.0F}, {0.0F, 0.0F, 0.0F}, {0.0F, 40.0F, 0.0F}, {0.0F, 40.0F, 0.0F},
	{0.0F, 40.0F, 0.0F}, {0.0F, 40.0F, 0.0F}, {0.0F, 56.0F, 0.0F}, {0.0F, 56.0F, 0.0F},
	{0.0F, 0.0F, 0.0F}, {0.0F, 0.0F, 0.0F}, {0.0F, 0.0F, 40.0F}, {0.0F, 0.0F, 40.0F},
	{0.0F, 0.0F, 40.0F}, {0.0F, 0.0F, 40.0F}, {0.0F, 0.0F, 56.0F}, {0.0F, 0.0F, 56.0F},
	{0.0F, 0.0F, 0.0F}, {0.0F, 0.0F, 0.0F}, {40.0F, 0.0F, 0.0F}, {40.0F, 0.0F, 0.0F},
	{40.0F, 0.0F, 0.0F}, {40.0F, 0.0F, 0.0F}, {56.0F, 0.0F, 0.0F}, {56.0F, 0.0F, 0.0F}
};

const ConstVector4D EditorGizmo::gizmoTangent[32] =
{
	{1.0F, 0.0F, 0.0F, -10.0F}, {1.0F, 0.0F, 0.0F, 10.0F}, {1.0F, 0.0F, 0.0F, 10.0F}, {1.0F, 0.0F, 0.0F, -10.0F},
	{1.0F, 0.0F, 0.0F, -10.0F}, {1.0F, 0.0F, 0.0F, 10.0F}, {1.0F, 0.0F, 0.0F, 10.0F}, {1.0F, 0.0F, 0.0F, -10.0F},
	{0.0F, 1.0F, 0.0F, -10.0F}, {0.0F, 1.0F, 0.0F, 10.0F}, {0.0F, 1.0F, 0.0F, 10.0F}, {0.0F, 1.0F, 0.0F, -10.0F},
	{0.0F, 1.0F, 0.0F, -10.0F}, {0.0F, 1.0F, 0.0F, 10.0F}, {0.0F, 1.0F, 0.0F, 10.0F}, {0.0F, 1.0F, 0.0F, -10.0F},
	{0.0F, 0.0F, 1.0F, -10.0F}, {0.0F, 0.0F, 1.0F, 10.0F}, {0.0F, 0.0F, 1.0F, 10.0F}, {0.0F, 0.0F, 1.0F, -10.0F},
	{0.0F, 0.0F, 1.0F, -10.0F}, {0.0F, 0.0F, 1.0F, 10.0F}, {0.0F, 0.0F, 1.0F, 10.0F}, {0.0F, 0.0F, 1.0F, -10.0F},
	{1.0F, 0.0F, 0.0F, -10.0F}, {1.0F, 0.0F, 0.0F, 10.0F}, {1.0F, 0.0F, 0.0F, 10.0F}, {1.0F, 0.0F, 0.0F, -10.0F},
	{1.0F, 0.0F, 0.0F, -10.0F}, {1.0F, 0.0F, 0.0F, 10.0F}, {1.0F, 0.0F, 0.0F, 10.0F}, {1.0F, 0.0F, 0.0F, -10.0F}
};

const ConstPoint2D EditorGizmo::gizmoTexcoord[32] =
{
	{0.0F, 0.0F}, {0.0F, 1.0F}, {0.625F, 1.0F}, {0.625F, 0.0F},
	{0.625F, 0.0F}, {0.625F, 1.0F}, {0.9921875F, 1.0F}, {0.9921875F, 0.0F}, 
	{0.0F, 0.0F}, {0.0F, 1.0F}, {0.625F, 1.0F}, {0.625F, 0.0F},
	{0.625F, 0.0F}, {0.625F, 1.0F}, {0.9921875F, 1.0F}, {0.9921875F, 0.0F}, 
	{0.0F, 0.0F}, {0.0F, 1.0F}, {0.625F, 1.0F}, {0.625F, 0.0F}, 
	{0.625F, 0.0F}, {0.625F, 1.0F}, {0.9921875F, 1.0F}, {0.9921875F, 0.0F}, 
	{0.0F, 0.0F}, {0.0F, 1.0F}, {0.625F, 1.0F}, {0.625F, 0.0F},
	{0.625F, 0.0F}, {0.625F, 1.0F}, {0.9921875F, 1.0F}, {0.9921875F, 0.0F} 
};


const ConstPoint3D EditorManipulator::markerVertex[4] = 
{
	{0.0F, 0.0F, 0.0F}, {0.0F, 0.0F, 0.0F}, {0.0F, 0.0F, 0.0F}, {0.0F, 0.0F, 0.0F}
};
 
const ConstPoint2D EditorManipulator::markerTexcoord[4] =
{
	{0.0F, 1.0F}, {0.0F, 0.0F}, {1.0F, 0.0F}, {1.0F, 1.0F}
};

const ConstVector2D EditorManipulator::markerBillboard[4] =
{
	{-36.0F, -36.0F}, {-36.0F, 12.0F}, {12.0F, 12.0F}, {12.0F, -36.0F}
};

const ConstPoint2D EditorManipulator::iconTexcoord[4] =
{
	{0.0F, 1.0F}, {0.0F, 0.0F}, {1.0F, 0.0F}, {1.0F, 1.0F}
};

const ConstVector2D EditorManipulator::iconBillboard[4] =
{
	{-34.5F, -34.5F}, {-34.5F, -13.5F}, {-13.5F, -13.5F}, {-13.5F, -34.5F}
};

const ConstVector2D EditorManipulator::handleBillboard[kMaxManipulatorHandleCount * 4] =
{
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F},
	{-3.0F, -3.0F}, {-3.0F, 3.0F}, {3.0F, 3.0F}, {3.0F, -3.0F}
};

const ConstPoint3D EditorManipulator::manipulatorBoxVertex[kManipulatorBoxVertexCount] =
{
	{0.0F, 0.0F, 0.0F}, {0.0F, 0.0F, 0.0F}, {0.0F, 0.0F, 0.0F}, {0.0F, 0.0F, 0.0F}, {0.0F, 0.0F, 0.0F}, {0.0F, 0.0F, 0.0F}, {0.0F, 0.0F, 0.0F}, {0.0F, 0.0F, 0.0F},
	{1.0F, 0.0F, 0.0F}, {1.0F, 0.0F, 0.0F}, {1.0F, 0.0F, 0.0F}, {1.0F, 0.0F, 0.0F}, {1.0F, 0.0F, 0.0F}, {1.0F, 0.0F, 0.0F}, {1.0F, 0.0F, 0.0F}, {1.0F, 0.0F, 0.0F},
	{1.0F, 1.0F, 0.0F}, {1.0F, 1.0F, 0.0F}, {1.0F, 1.0F, 0.0F}, {1.0F, 1.0F, 0.0F}, {1.0F, 1.0F, 0.0F}, {1.0F, 1.0F, 0.0F}, {1.0F, 1.0F, 0.0F}, {1.0F, 1.0F, 0.0F},
	{0.0F, 1.0F, 0.0F}, {0.0F, 1.0F, 0.0F}, {0.0F, 1.0F, 0.0F}, {0.0F, 1.0F, 0.0F}, {0.0F, 1.0F, 0.0F}, {0.0F, 1.0F, 0.0F}, {0.0F, 1.0F, 0.0F}, {0.0F, 1.0F, 0.0F},
	{0.0F, 0.0F, 1.0F}, {0.0F, 0.0F, 1.0F}, {0.0F, 0.0F, 1.0F}, {0.0F, 0.0F, 1.0F}, {0.0F, 0.0F, 1.0F}, {0.0F, 0.0F, 1.0F}, {0.0F, 0.0F, 1.0F}, {0.0F, 0.0F, 1.0F},
	{1.0F, 0.0F, 1.0F}, {1.0F, 0.0F, 1.0F}, {1.0F, 0.0F, 1.0F}, {1.0F, 0.0F, 1.0F}, {1.0F, 0.0F, 1.0F}, {1.0F, 0.0F, 1.0F}, {1.0F, 0.0F, 1.0F}, {1.0F, 0.0F, 1.0F},
	{1.0F, 1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {1.0F, 1.0F, 1.0F},
	{0.0F, 1.0F, 1.0F}, {0.0F, 1.0F, 1.0F}, {0.0F, 1.0F, 1.0F}, {0.0F, 1.0F, 1.0F}, {0.0F, 1.0F, 1.0F}, {0.0F, 1.0F, 1.0F}, {0.0F, 1.0F, 1.0F}, {0.0F, 1.0F, 1.0F}
};

const ConstPoint3D EditorManipulator::manipulatorCenterBoxVertex[kManipulatorBoxVertexCount] =
{
	{-1.0F, -1.0F, -1.0F}, {-1.0F, -1.0F, -1.0F}, {-1.0F, -1.0F, -1.0F}, {-1.0F, -1.0F, -1.0F}, {-1.0F, -1.0F, -1.0F}, {-1.0F, -1.0F, -1.0F}, {-1.0F, -1.0F, -1.0F}, {-1.0F, -1.0F, -1.0F},
	{1.0F, -1.0F, -1.0F}, {1.0F, -1.0F, -1.0F}, {1.0F, -1.0F, -1.0F}, {1.0F, -1.0F, -1.0F}, {1.0F, -1.0F, -1.0F}, {1.0F, -1.0F, -1.0F}, {1.0F, -1.0F, -1.0F}, {1.0F, -1.0F, -1.0F},
	{1.0F, 1.0F, -1.0F}, {1.0F, 1.0F, -1.0F}, {1.0F, 1.0F, -1.0F}, {1.0F, 1.0F, -1.0F}, {1.0F, 1.0F, -1.0F}, {1.0F, 1.0F, -1.0F}, {1.0F, 1.0F, -1.0F}, {1.0F, 1.0F, -1.0F},
	{-1.0F, 1.0F, -1.0F}, {-1.0F, 1.0F, -1.0F}, {-1.0F, 1.0F, -1.0F}, {-1.0F, 1.0F, -1.0F}, {-1.0F, 1.0F, -1.0F}, {-1.0F, 1.0F, -1.0F}, {-1.0F, 1.0F, -1.0F}, {-1.0F, 1.0F, -1.0F},
	{-1.0F, -1.0F, 1.0F}, {-1.0F, -1.0F, 1.0F}, {-1.0F, -1.0F, 1.0F}, {-1.0F, -1.0F, 1.0F}, {-1.0F, -1.0F, 1.0F}, {-1.0F, -1.0F, 1.0F}, {-1.0F, -1.0F, 1.0F}, {-1.0F, -1.0F, 1.0F},
	{1.0F, -1.0F, 1.0F}, {1.0F, -1.0F, 1.0F}, {1.0F, -1.0F, 1.0F}, {1.0F, -1.0F, 1.0F}, {1.0F, -1.0F, 1.0F}, {1.0F, -1.0F, 1.0F}, {1.0F, -1.0F, 1.0F}, {1.0F, -1.0F, 1.0F},
	{1.0F, 1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {1.0F, 1.0F, 1.0F},
	{-1.0F, 1.0F, 1.0F}, {-1.0F, 1.0F, 1.0F}, {-1.0F, 1.0F, 1.0F}, {-1.0F, 1.0F, 1.0F}, {-1.0F, 1.0F, 1.0F}, {-1.0F, 1.0F, 1.0F}, {-1.0F, 1.0F, 1.0F}, {-1.0F, 1.0F, 1.0F}
};

const ConstVector3D EditorManipulator::manipulatorBoxOffset[kManipulatorBoxVertexCount] =
{
	{-1.0F, -1.0F, -1.0F}, {1.0F, -1.0F, -1.0F}, {1.0F, 1.0F, -1.0F}, {-1.0F, 1.0F, -1.0F}, {-1.0F, -1.0F, 1.0F}, {1.0F, -1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {-1.0F, 1.0F, 1.0F},
	{-1.0F, -1.0F, -1.0F}, {1.0F, -1.0F, -1.0F}, {1.0F, 1.0F, -1.0F}, {-1.0F, 1.0F, -1.0F}, {-1.0F, -1.0F, 1.0F}, {1.0F, -1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {-1.0F, 1.0F, 1.0F},
	{-1.0F, -1.0F, -1.0F}, {1.0F, -1.0F, -1.0F}, {1.0F, 1.0F, -1.0F}, {-1.0F, 1.0F, -1.0F}, {-1.0F, -1.0F, 1.0F}, {1.0F, -1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {-1.0F, 1.0F, 1.0F},
	{-1.0F, -1.0F, -1.0F}, {1.0F, -1.0F, -1.0F}, {1.0F, 1.0F, -1.0F}, {-1.0F, 1.0F, -1.0F}, {-1.0F, -1.0F, 1.0F}, {1.0F, -1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {-1.0F, 1.0F, 1.0F},
	{-1.0F, -1.0F, -1.0F}, {1.0F, -1.0F, -1.0F}, {1.0F, 1.0F, -1.0F}, {-1.0F, 1.0F, -1.0F}, {-1.0F, -1.0F, 1.0F}, {1.0F, -1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {-1.0F, 1.0F, 1.0F},
	{-1.0F, -1.0F, -1.0F}, {1.0F, -1.0F, -1.0F}, {1.0F, 1.0F, -1.0F}, {-1.0F, 1.0F, -1.0F}, {-1.0F, -1.0F, 1.0F}, {1.0F, -1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {-1.0F, 1.0F, 1.0F},
	{-1.0F, -1.0F, -1.0F}, {1.0F, -1.0F, -1.0F}, {1.0F, 1.0F, -1.0F}, {-1.0F, 1.0F, -1.0F}, {-1.0F, -1.0F, 1.0F}, {1.0F, -1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {-1.0F, 1.0F, 1.0F},
	{-1.0F, -1.0F, -1.0F}, {1.0F, -1.0F, -1.0F}, {1.0F, 1.0F, -1.0F}, {-1.0F, 1.0F, -1.0F}, {-1.0F, -1.0F, 1.0F}, {1.0F, -1.0F, 1.0F}, {1.0F, 1.0F, 1.0F}, {-1.0F, 1.0F, 1.0F}
};

const Triangle EditorManipulator::manipulatorBoxTriangle[kManipulatorBoxTriangleCount] =
{
	{{ 1,  8,  5}}, {{ 5,  8, 12}}, {{ 5, 12,  6}}, {{ 6, 12, 15}}, {{ 6, 15,  2}}, {{ 2, 15, 11}}, {{ 2, 11,  1}}, {{ 1, 11,  8}},
	{{10, 17, 14}}, {{14, 17, 21}}, {{14, 21, 15}}, {{15, 21, 20}}, {{15, 20, 11}}, {{11, 20, 16}}, {{11, 16, 10}}, {{10, 16, 17}},
	{{19, 26, 23}}, {{23, 26, 30}}, {{23, 30, 20}}, {{20, 30, 29}}, {{20, 29, 16}}, {{16, 29, 25}}, {{16, 25, 19}}, {{19, 25, 26}},
	{{24,  3, 28}}, {{28,  3,  7}}, {{28,  7, 29}}, {{29,  7,  6}}, {{29,  6, 25}}, {{25,  6,  2}}, {{25,  2, 24}}, {{24,  2,  3}},
	{{33, 40, 37}}, {{37, 40, 44}}, {{37, 44, 38}}, {{38, 44, 47}}, {{38, 47, 34}}, {{34, 47, 43}}, {{34, 43, 33}}, {{33, 43, 40}},
	{{42, 49, 46}}, {{46, 49, 53}}, {{46, 53, 47}}, {{47, 53, 52}}, {{47, 52, 43}}, {{43, 52, 48}}, {{43, 48, 42}}, {{42, 48, 49}},
	{{51, 58, 55}}, {{55, 58, 62}}, {{55, 62, 52}}, {{52, 62, 61}}, {{52, 61, 48}}, {{48, 61, 57}}, {{48, 57, 51}}, {{51, 57, 58}},
	{{56, 35, 60}}, {{60, 35, 39}}, {{60, 39, 61}}, {{61, 39, 38}}, {{61, 38, 57}}, {{57, 38, 34}}, {{57, 34, 56}}, {{56, 34, 35}},
	{{ 6, 34,  5}}, {{ 5, 34, 33}}, {{ 5, 33,  4}}, {{ 4, 33, 32}}, {{ 4, 32,  7}}, {{ 7, 32, 35}}, {{ 7, 35,  6}}, {{ 6, 35, 34}},
	{{14, 42, 13}}, {{13, 42, 41}}, {{13, 41, 12}}, {{12, 41, 40}}, {{12, 40, 15}}, {{15, 40, 43}}, {{15, 43, 14}}, {{14, 43, 42}},
	{{22, 50, 21}}, {{21, 50, 49}}, {{21, 49, 20}}, {{20, 49, 48}}, {{20, 48, 23}}, {{23, 48, 51}}, {{23, 51, 22}}, {{22, 51, 50}},
	{{30, 58, 29}}, {{29, 58, 57}}, {{29, 57, 28}}, {{28, 57, 56}}, {{28, 56, 31}}, {{31, 56, 59}}, {{31, 59, 30}}, {{30, 59, 58}},
	{{ 0,  1,  4}}, {{ 1,  5,  4}}, {{ 3,  2,  0}}, {{ 2,  1,  0}}, {{ 3,  0,  7}}, {{ 0,  4,  7}},
	{{ 9, 10, 13}}, {{10, 14, 13}}, {{ 8, 11,  9}}, {{11, 10,  9}}, {{ 8,  9, 12}}, {{ 9, 13, 12}},
	{{18, 19, 22}}, {{19, 23, 22}}, {{17, 16, 18}}, {{16, 19, 18}}, {{17, 18, 21}}, {{18, 22, 21}},
	{{27, 24, 31}}, {{24, 28, 31}}, {{26, 25, 27}}, {{25, 24, 27}}, {{26, 27, 30}}, {{27, 31, 30}},
	{{32, 33, 36}}, {{33, 37, 36}}, {{39, 36, 38}}, {{38, 36, 37}}, {{35, 32, 39}}, {{32, 36, 39}},
	{{41, 42, 45}}, {{42, 46, 45}}, {{44, 45, 47}}, {{47, 45, 46}}, {{40, 41, 44}}, {{41, 45, 44}},
	{{50, 51, 54}}, {{51, 55, 54}}, {{53, 54, 52}}, {{52, 54, 55}}, {{49, 50, 53}}, {{50, 54, 53}},
	{{59, 56, 63}}, {{56, 60, 63}}, {{62, 63, 61}}, {{61, 63, 60}}, {{58, 59, 62}}, {{59, 63, 62}}
};


EditorGizmo::EditorGizmo(const EditorManipulator *manipulator) :
		gizmoTextureMap("WorldEditor/arrow"),
		gizmoRenderable(kRenderQuads),
		boxDiffuseColor(ColorRGBA(1.0F, 1.0F, 0.5F, 1.0F)),
		boxTextureMap(&boxEdgeTextureHeader, boxEdgeTextureImage),
		boxRenderable(kRenderQuads),
		faceRenderable(kRenderQuads),
		edgeDiffuseColor(kAttributeMutable),
		edgeTextureMap(&boxHiliteTextureHeader, boxHiliteTextureImage),
		edgeRenderable(kRenderQuads),
		handleRenderable(kRenderQuads)
{
	gizmoManipulator = manipulator;
	hiliteEdgeIndex = -1;
	
	gizmoAttributeList.Append(&gizmoTextureMap);
	gizmoRenderable.SetMaterialAttributeList(&gizmoAttributeList);
	gizmoRenderable.SetAmbientBlendState(kBlendInterpolate);
	gizmoRenderable.SetShaderFlags(kShaderAmbientEffect | kShaderVertexPolyboard | kShaderLinearPolyboard | kShaderOrthoPolyboard | kShaderScaleVertex);
	gizmoRenderable.SetRenderParameterPointer(&gizmoScaleVector);
	gizmoRenderable.SetTransformable(manipulator->GetTargetNode());
	
	for (machine a = 0; a < 8; a++) gizmoColor[a].Set(1.0F, 0.0F, 0.0F);
	for (machine a = 8; a < 16; a++) gizmoColor[a].Set(0.0F, 1.0F, 0.0F);
	for (machine a = 16; a < 24; a++) gizmoColor[a].Set(0.0F, 0.375F, 1.0F);
	for (machine a = 24; a < 32; a++) gizmoColor[a].Set(1.0F, 0.0F, 0.0F);
	
	boxAttributeList.Append(&boxDiffuseColor);
	boxAttributeList.Append(&boxTextureMap);
	boxRenderable.SetMaterialAttributeList(&boxAttributeList);
	boxRenderable.SetAmbientBlendState(kBlendInterpolate);
	boxRenderable.SetShaderFlags(kShaderAmbientEffect | kShaderVertexPolyboard | kShaderLinearPolyboard);
	boxRenderable.SetTransformable(manipulator->GetTargetNode());
	boxRenderable.SetVertexCount(48);
	boxRenderable.SetAttributeArray(kArrayVertex, boxVertex);
	boxRenderable.SetAttributeArray(kArrayTangent, boxTangent);
	boxRenderable.SetAttributeArray(kArrayTexture0, boxTexcoord);
	
	for (machine a = 0; a < 48; a += 4)
	{
		boxTexcoord[a].Set(0.0F, 0.0F);
		boxTexcoord[a + 1].Set(1.0F, 0.0F);
		boxTexcoord[a + 2].Set(1.0F, 1.0F);
		boxTexcoord[a + 3].Set(0.0F, 1.0F);
	}
	
	faceRenderable.SetAmbientBlendState(kBlendAccumulate);
	faceRenderable.SetShaderFlags(kShaderAmbientEffect);
	faceRenderable.SetTransformable(manipulator->GetTargetNode());
	faceRenderable.SetVertexCount(24);
	faceRenderable.SetAttributeArray(kArrayVertex, faceVertex);
	faceRenderable.SetAttributeArray(kArrayColor0, faceColor);
	
	for (machine a = 0; a < 24; a++) faceColor[a].Set(0.0625F, 0.0625F, 0.0625F);
	
	edgeAttributeList.Append(&edgeDiffuseColor);
	edgeAttributeList.Append(&edgeTextureMap);
	edgeRenderable.SetMaterialAttributeList(&edgeAttributeList);
	edgeRenderable.SetAmbientBlendState(kBlendAccumulate);
	edgeRenderable.SetShaderFlags(kShaderAmbientEffect | kShaderVertexPolyboard | kShaderLinearPolyboard);
	edgeRenderable.SetTransformable(manipulator->GetTargetNode());
	edgeRenderable.SetVertexCount(4);
	edgeRenderable.SetAttributeArray(kArrayVertex, edgeVertex);
	edgeRenderable.SetAttributeArray(kArrayTangent, edgeTangent);
	edgeRenderable.SetAttributeArray(kArrayTexture0, edgeTexcoord);
	
	edgeTexcoord[0].Set(0.0F, 0.0F);
	edgeTexcoord[1].Set(1.0F, 0.0F);
	edgeTexcoord[2].Set(1.0F, 1.0F);
	edgeTexcoord[3].Set(0.0F, 1.0F);
	
	handleRenderable.SetAmbientBlendState(kBlendInterpolate);
	handleRenderable.SetShaderFlags(kShaderAmbientEffect | kShaderVertexBillboard | kShaderScaleVertex);
	handleRenderable.SetRenderParameterPointer(&gizmoScaleVector);
	handleRenderable.SetAttributeArray(kArrayBillboard, &EditorManipulator::handleBillboard[0]);
	handleRenderable.SetTransformable(manipulator->GetTargetNode());
	handleRenderable.SetVertexCount(12);
	handleRenderable.SetAttributeArray(kArrayVertex, &handleVertex[0]);
	handleRenderable.SetAttributeArray(kArrayColor0, &handleColor[0]);
	
	for (machine a = 0; a < 4; a++) handleColor[a].Set(1.0F, 0.0F, 0.0F);
	for (machine a = 4; a < 8; a++) handleColor[a].Set(0.0F, 1.0F, 0.0F);
	for (machine a = 8; a < 12; a++) handleColor[a].Set(0.0F, 0.375F, 1.0F);
}

EditorGizmo::~EditorGizmo()
{
}

void EditorGizmo::HiliteMovers(unsigned_int32 mask)
{
	if (mask & 1)
	{
		for (machine a = 4; a < 8; a++) gizmoColor[a].Set(1.0F, 1.0F, 1.0F);
		for (machine a = 28; a < 32; a++) gizmoColor[a].Set(1.0F, 1.0F, 1.0F);
	}
	else
	{
		for (machine a = 4; a < 8; a++) gizmoColor[a].Set(1.0F, 0.0F, 0.0F);
		for (machine a = 28; a < 32; a++) gizmoColor[a].Set(1.0F, 0.0F, 0.0F);
	}
	
	if (mask & 2)
	{
		for (machine a = 12; a < 16; a++) gizmoColor[a].Set(1.0F, 1.0F, 1.0F);
	}
	else
	{
		for (machine a = 12; a < 16; a++) gizmoColor[a].Set(0.0F, 1.0F, 0.0F);
	}
	
	if (mask & 4)
	{
		for (machine a = 20; a < 24; a++) gizmoColor[a].Set(1.0F, 1.0F, 1.0F);
	}
	else
	{
		for (machine a = 20; a < 24; a++) gizmoColor[a].Set(0.0F, 0.375F, 1.0F);
	}
}

void EditorGizmo::HiliteRotators(unsigned_int32 mask)
{
	if (mask & 1)
	{
		for (machine a = 0; a < 4; a++) handleColor[a].Set(1.0F, 1.0F, 1.0F);
	}
	else
	{
		for (machine a = 0; a < 4; a++) handleColor[a].Set(1.0F, 0.0F, 0.0F);
	}
	
	if (mask & 2)
	{
		for (machine a = 4; a < 8; a++) handleColor[a].Set(1.0F, 1.0F, 1.0F);
	}
	else
	{
		for (machine a = 4; a < 8; a++) handleColor[a].Set(0.0F, 1.0F, 0.0F);
	}
	
	if (mask & 4)
	{
		for (machine a = 8; a < 12; a++) handleColor[a].Set(1.0F, 1.0F, 1.0F);
	}
	else
	{
		for (machine a = 8; a < 12; a++) handleColor[a].Set(0.0F, 0.375F, 1.0F);
	}
}

int32 EditorGizmo::PickMover(const ManipulatorViewportData *viewportData, const Ray *ray) const
{
	Vector3D viewDirection = GetTransformable()->GetInverseWorldTransform() * viewportData->viewportCamera->GetNodeTransform()[2];
	
	const Transform4D& transform = gizmoRenderable.GetTransformable()->GetInverseWorldTransform();
	Point3D p = transform * ray->origin;
	Vector3D v = transform * ray->direction;
	
	float scale = viewportData->viewportScale;
	float handleRadius = scale * 10.0F;
	float handlePosition = scale * 48.0F;
	
	float r2 = handleRadius * handleRadius;
	float x = p.x - handlePosition;
	float y = p.y - handlePosition;
	float z = p.z - handlePosition;
	
	float a = v * v;
	float b0 = x * v.x + p.y * v.y + p.z * v.z;
	float c0 = x * x + p.y * p.y + p.z * p.z - r2;
	float b1 = p.x * v.x + y * v.y + p.z * v.z;
	float c1 = p.x * p.x + y * y + p.z * p.z - r2;
	float b2 = p.x * v.x + p.y * v.y + z * v.z;
	float c2 = p.x * p.x + p.y * p.y + z * z - r2;
	
	float d0 = b0 * b0 - a * c0;
	float d1 = b1 * b1 - a * c1;
	float d2 = b2 * b2 - a * c2;
	
	int32 index = -1;
	float t = K::infinity;
	
	if ((d0 > K::min_float) && (Fabs(viewDirection.x) <= kGizmoThreshold))
	{
		float u = (-b0 - Sqrt(d0)) / a;
		if (u < t)
		{
			t = u;
			index = 0;
		}
	}
	
	if ((d1 > K::min_float) && (Fabs(viewDirection.y) <= kGizmoThreshold))
	{
		float u = (-b1 - Sqrt(d1)) / a;
		if (u < t)
		{
			t = u;
			index = 1;
		}
	}
	
	if ((d2 > K::min_float) && (Fabs(viewDirection.z) <= kGizmoThreshold))
	{
		float u = (-b2 - Sqrt(d2)) / a;
		if (u < t) index = 2;
	}
	
	return (index);
}

int32 EditorGizmo::PickRotator(const ManipulatorViewportData *viewportData, const Ray *ray) const
{
	const Transform4D& transform = gizmoRenderable.GetTransformable()->GetInverseWorldTransform();
	Point3D p = transform * ray->origin;
	Vector3D v = transform * ray->direction;
	
	float scale = viewportData->viewportScale;
	float handleRadius = scale * 3.0F;
	float handlePosition = scale * 24.0F;
	
	float r2 = handleRadius * handleRadius;
	float x = p.x - handlePosition;
	float y = p.y - handlePosition;
	float z = p.z - handlePosition;
	
	float a = v * v;
	float b0 = p.x * v.x + y * v.y + z * v.z;
	float c0 = p.x * p.x + y * y + z * z - r2;
	float b1 = x * v.x + p.y * v.y + z * v.z;
	float c1 = x * x + p.y * p.y + z * z - r2;
	float b2 = x * v.x + y * v.y + p.z * v.z;
	float c2 = x * x + y * y + p.z * p.z - r2;
	
	float d0 = b0 * b0 - a * c0;
	float d1 = b1 * b1 - a * c1;
	float d2 = b2 * b2 - a * c2;
	
	int32 index = -1;
	float t = K::infinity;
	
	if (d0 > K::min_float)
	{
		float u = (-b0 - Sqrt(d0)) / a;
		if (u < t)
		{
			t = u;
			index = 0;
		}
	}
	
	if (d1 > K::min_float)
	{
		float u = (-b1 - Sqrt(d1)) / a;
		if (u < t)
		{
			t = u;
			index = 1;
		}
	}
	
	if (d2 > K::min_float)
	{
		float u = (-b2 - Sqrt(d2)) / a;
		if (u < t) index = 2;
	}
	
	return (index);
}

void EditorGizmo::HiliteFace(int32 face, float intensity)
{
	for (machine a = 0; a < 24; a++) faceColor[a].Set(0.0625F, 0.0625F, 0.0625F);
	
	if (face >= 0)
	{
		intensity *= 0.125F; 
		ColorRGB *color = faceColor + face * 4;
		for (machine a = 0; a < 4; a++) color[a].Set(intensity, intensity, intensity);
	}
}

void EditorGizmo::HiliteEdge(int32 edge, float intensity)
{
	hiliteEdgeIndex = edge;
	if (edge >= 0)
	{
		intensity *= 0.25F;
		edgeDiffuseColor.SetDiffuseColor(ColorRGBA(intensity, intensity, intensity, 1.0F));
	}
}

int32 EditorGizmo::PickFace(const Ray *ray, Point3D *point) const
{
	const Transform4D& transform = GetTransformable()->GetInverseWorldTransform();
	const Point3D& position = transform * ray->origin;
	const Vector3D& direction = transform * ray->direction;
	const Box3D& box = gizmoBox;
	
	if ((position.x > box.max.x) && (direction.x < 0.0F))
	{
		float t = (box.max.x - position.x) / direction.x;
		float y = position.y + t * direction.y;
		float z = position.z + t * direction.z;
		
		if ((y > box.min.y) && (y < box.max.y) && (z > box.min.z) && (z < box.max.z))
		{
			if (point) *point = ray->origin + ray->direction * t;
			return (0);
		}
	}
	
	if ((position.x < box.min.x) && (direction.x > 0.0F))
	{
		float t = (box.min.x - position.x) / direction.x;
		float y = position.y + t * direction.y;
		float z = position.z + t * direction.z;
		
		if ((y > box.min.y) && (y < box.max.y) && (z > box.min.z) && (z < box.max.z))
		{
			if (point) *point = ray->origin + ray->direction * t;
			return (1);
		}
	}
	
	if ((position.y > box.max.y) && (direction.y < 0.0F))
	{
		float t = (box.max.y - position.y) / direction.y;
		float x = position.x + t * direction.x;
		float z = position.z + t * direction.z;
		
		if ((x > box.min.x) && (x < box.max.x) && (z > box.min.z) && (z < box.max.z))
		{
			if (point) *point = ray->origin + ray->direction * t;
			return (2);
		}
	}
	
	if ((position.y < box.min.y) && (direction.y > 0.0F))
	{
		float t = (box.min.y - position.y) / direction.y;
		float x = position.x + t * direction.x;
		float z = position.z + t * direction.z;
		
		if ((x > box.min.x) && (x < box.max.x) && (z > box.min.z) && (z < box.max.z))
		{
			if (point) *point = ray->origin + ray->direction * t;
			return (3);
		}
	}
	
	if ((position.z > box.max.z) && (direction.z < 0.0F))
	{
		float t = (box.max.z - position.z) / direction.z;
		float x = position.x + t * direction.x;
		float y = position.y + t * direction.y;
		
		if ((x > box.min.x) && (x < box.max.x) && (y > box.min.y) && (y < box.max.y))
		{
			if (point) *point = ray->origin + ray->direction * t;
			return (4);
		}
	}
	
	if ((position.z < box.min.z) && (direction.z > 0.0F))
	{
		float t = (box.min.z - position.z) / direction.z;
		float x = position.x + t * direction.x;
		float y = position.y + t * direction.y;
		
		if ((x > box.min.x) && (x < box.max.x) && (y > box.min.y) && (y < box.max.y))
		{
			if (point) *point = ray->origin + ray->direction * t;
			return (5);
		}
	}
	
	return (-1);
}

int32 EditorGizmo::PickEdge(const Ray *ray, Point3D *point) const
{
	const Transform4D& transform = GetTransformable()->GetInverseWorldTransform();
	Bivector4D rayLine(transform * ray->origin, transform * ray->direction);
	
	const Box3D& box = gizmoBox;
	Vector3D size = box.GetSize();
	float width = Fmin(size.x, size.y, size.z) * 0.0625F;
	
	for (machine a = 0; a < 4; a++)
	{
		float y = box[a & 1].y;
		float z = box[(a >> 1) & 1].z;
		
		Bivector4D edgeLine(Point3D(box.min.x, y, z), Point3D(box.max.x, y, z));
		Antivector3D normal = rayLine.GetTangent() % edgeLine.GetTangent();
		
		float t = SquaredMag(normal);
		if (t > K::min_float)
		{
			float d = Fabs((rayLine ^ edgeLine) * InverseSqrt(t));
			if (d < width)
			{
				Antivector4D plane = edgeLine ^ normal;
				Vector4D p = rayLine ^ plane;
				if (Fabs(p.w) > K::min_float)
				{
					float w = 1.0F / p.w;
					float x = p.x * w;
					if ((x > box.min.x) && (x < box.max.x))
					{
						if (point) *point = GetTransformable()->GetWorldTransform() * Point3D(x, p.y * w, p.z * w);
						return (a);
					}
				}
			}
		}
	}
	
	for (machine a = 0; a < 4; a++)
	{
		float x = box[a & 1].x;
		float z = box[(a >> 1) & 1].z;
		
		Bivector4D edgeLine(Point3D(x, box.min.y, z), Point3D(x, box.max.y, z));
		Antivector3D normal = rayLine.GetTangent() % edgeLine.GetTangent();
		
		float t = SquaredMag(normal);
		if (t > K::min_float)
		{
			float d = Fabs((rayLine ^ edgeLine) * InverseSqrt(t));
			if (d < width)
			{
				Antivector4D plane = edgeLine ^ normal;
				Vector4D p = rayLine ^ plane;
				if (Fabs(p.w) > K::min_float)
				{
					float w = 1.0F / p.w;
					float y = p.y * w;
					if ((y > box.min.y) && (y < box.max.y))
					{
						if (point) *point = GetTransformable()->GetWorldTransform() * Point3D(p.x * w, y, p.z * w);
						return (a + 4);
					}
				}
			}
		}
	}
	
	for (machine a = 0; a < 4; a++)
	{
		float x = box[a & 1].x;
		float y = box[(a >> 1) & 1].y;
		
		Bivector4D edgeLine(Point3D(x, y, box.min.z), Point3D(x, y, box.max.z));
		Antivector3D normal = rayLine.GetTangent() % edgeLine.GetTangent();
		
		float t = SquaredMag(normal);
		if (t > K::min_float)
		{
			float d = Fabs((rayLine ^ edgeLine) * InverseSqrt(t));
			if (d < width)
			{
				Antivector4D plane = edgeLine ^ normal;
				Vector4D p = rayLine ^ plane;
				if (Fabs(p.w) > K::min_float)
				{
					float w = 1.0F / p.w;
					float z = p.z * w;
					if ((z > box.min.z) && (z < box.max.z))
					{
						if (point) *point = GetTransformable()->GetWorldTransform() * Point3D(p.x * w, p.y * w, z);
						return (a + 8);
					}
				}
			}
		}
	}
	
	return (-1);
}

void EditorGizmo::Render(const ManipulatorRenderData *renderData)
{
	float scale = renderData->viewportScale;
	gizmoScaleVector.Set(scale, scale, scale, scale);
	
	Vector3D viewDirection = GetTransformable()->GetInverseWorldTransform() * renderData->viewportCamera->GetNodeTransform()[2];
	
	List<Renderable> *renderList = renderData->gizmoList;
	if (renderList)
	{
		if (Fabs(viewDirection.x) > kGizmoThreshold)
		{
			gizmoRenderable.SetVertexCount(16);
			gizmoRenderable.SetAttributeArray(kArrayVertex, &gizmoVertex[8]);
			gizmoRenderable.SetAttributeArray(kArrayColor0, &gizmoColor[8]);
			gizmoRenderable.SetAttributeArray(kArrayTangent, &gizmoTangent[8]);
			gizmoRenderable.SetAttributeArray(kArrayTexture0, &gizmoTexcoord[8]);
		}
		else if (Fabs(viewDirection.y) > kGizmoThreshold)
		{
			gizmoRenderable.SetVertexCount(16);
			gizmoRenderable.SetAttributeArray(kArrayVertex, &gizmoVertex[16]);
			gizmoRenderable.SetAttributeArray(kArrayColor0, &gizmoColor[16]);
			gizmoRenderable.SetAttributeArray(kArrayTangent, &gizmoTangent[16]);
			gizmoRenderable.SetAttributeArray(kArrayTexture0, &gizmoTexcoord[16]);
		}
		else
		{
			gizmoRenderable.SetVertexCount((Fabs(viewDirection.z) > kGizmoThreshold) ? 16 : 24);
			gizmoRenderable.SetAttributeArray(kArrayVertex, &gizmoVertex[0]);
			gizmoRenderable.SetAttributeArray(kArrayColor0, &gizmoColor[0]);
			gizmoRenderable.SetAttributeArray(kArrayTangent, &gizmoTangent[0]);
			gizmoRenderable.SetAttributeArray(kArrayTexture0, &gizmoTexcoord[0]);
		}
		
		renderList->Append(&gizmoRenderable);
		
		if (renderData->viewportType == kEditorViewportFrustum) RenderBox(renderData);
	}
	
	renderList = renderData->handleList;
	if (renderList)
	{
		float scale = renderData->viewportScale * 24.0F;
		
		handleVertex[0].Set(0.0F, scale, scale);
		handleVertex[1].Set(0.0F, scale, scale);
		handleVertex[2].Set(0.0F, scale, scale);
		handleVertex[3].Set(0.0F, scale, scale);
		handleVertex[4].Set(scale, 0.0F, scale);
		handleVertex[5].Set(scale, 0.0F, scale);
		handleVertex[6].Set(scale, 0.0F, scale);
		handleVertex[7].Set(scale, 0.0F, scale);
		handleVertex[8].Set(scale, scale, 0.0F);
		handleVertex[9].Set(scale, scale, 0.0F);
		handleVertex[10].Set(scale, scale, 0.0F);
		handleVertex[11].Set(scale, scale, 0.0F);
		
		renderList->Append(&handleRenderable);
	}
}

void EditorGizmo::RenderBox(const ManipulatorRenderData *renderData)
{
	float	scale[8];
	
	Box3D& box = gizmoBox;
	box = gizmoManipulator->CalculateNodeBoundingBox();
	EditorManipulator::AdjustBoundingBox(&box);
	
	Transform4D transform = renderData->viewportCamera->GetInverseWorldTransform() * gizmoManipulator->GetTargetNode()->GetWorldTransform();
	const MatrixRow4D& row = transform.GetRow(2);
	
	scale[0] = row ^ box.min;
	scale[1] = row ^ Point3D(box.max.x, box.min.y, box.min.z);
	scale[2] = row ^ Point3D(box.min.x, box.max.y, box.min.z);
	scale[3] = row ^ Point3D(box.max.x, box.max.y, box.min.z);
	scale[4] = row ^ Point3D(box.min.x, box.min.y, box.max.z);
	scale[5] = row ^ Point3D(box.max.x, box.min.y, box.max.z);
	scale[6] = row ^ Point3D(box.min.x, box.max.y, box.max.z);
	scale[7] = row ^ box.max;
	
	float t = renderData->viewportScale * 0.125F;
	for (machine a = 0; a < 8; a++) scale[a] = Fmax(scale[a], 0.5F) * t;
	
	boxVertex[0].Set(box.min.x, box.min.y, box.min.z);
	boxVertex[1].Set(box.min.x, box.min.y, box.min.z);
	boxVertex[2].Set(box.max.x, box.min.y, box.min.z);
	boxVertex[3].Set(box.max.x, box.min.y, box.min.z);
	
	boxVertex[4].Set(box.min.x, box.max.y, box.min.z);
	boxVertex[5].Set(box.min.x, box.max.y, box.min.z);
	boxVertex[6].Set(box.max.x, box.max.y, box.min.z);
	boxVertex[7].Set(box.max.x, box.max.y, box.min.z);
	
	boxVertex[8].Set(box.min.x, box.min.y, box.max.z);
	boxVertex[9].Set(box.min.x, box.min.y, box.max.z);
	boxVertex[10].Set(box.max.x, box.min.y, box.max.z);
	boxVertex[11].Set(box.max.x, box.min.y, box.max.z);
	
	boxVertex[12].Set(box.min.x, box.max.y, box.max.z);
	boxVertex[13].Set(box.min.x, box.max.y, box.max.z);
	boxVertex[14].Set(box.max.x, box.max.y, box.max.z);
	boxVertex[15].Set(box.max.x, box.max.y, box.max.z);
	
	boxVertex[16].Set(box.min.x, box.min.y, box.min.z);
	boxVertex[17].Set(box.min.x, box.min.y, box.min.z);
	boxVertex[18].Set(box.min.x, box.max.y, box.min.z);
	boxVertex[19].Set(box.min.x, box.max.y, box.min.z);
	
	boxVertex[20].Set(box.max.x, box.min.y, box.min.z);
	boxVertex[21].Set(box.max.x, box.min.y, box.min.z);
	boxVertex[22].Set(box.max.x, box.max.y, box.min.z);
	boxVertex[23].Set(box.max.x, box.max.y, box.min.z);
	
	boxVertex[24].Set(box.min.x, box.min.y, box.max.z);
	boxVertex[25].Set(box.min.x, box.min.y, box.max.z);
	boxVertex[26].Set(box.min.x, box.max.y, box.max.z);
	boxVertex[27].Set(box.min.x, box.max.y, box.max.z);
	
	boxVertex[28].Set(box.max.x, box.min.y, box.max.z);
	boxVertex[29].Set(box.max.x, box.min.y, box.max.z);
	boxVertex[30].Set(box.max.x, box.max.y, box.max.z);
	boxVertex[31].Set(box.max.x, box.max.y, box.max.z);
	
	boxVertex[32].Set(box.min.x, box.min.y, box.min.z);
	boxVertex[33].Set(box.min.x, box.min.y, box.min.z);
	boxVertex[34].Set(box.min.x, box.min.y, box.max.z);
	boxVertex[35].Set(box.min.x, box.min.y, box.max.z);
	
	boxVertex[36].Set(box.max.x, box.min.y, box.min.z);
	boxVertex[37].Set(box.max.x, box.min.y, box.min.z);
	boxVertex[38].Set(box.max.x, box.min.y, box.max.z);
	boxVertex[39].Set(box.max.x, box.min.y, box.max.z);
	
	boxVertex[40].Set(box.min.x, box.max.y, box.min.z);
	boxVertex[41].Set(box.min.x, box.max.y, box.min.z);
	boxVertex[42].Set(box.min.x, box.max.y, box.max.z);
	boxVertex[43].Set(box.min.x, box.max.y, box.max.z);
	
	boxVertex[44].Set(box.max.x, box.max.y, box.min.z);
	boxVertex[45].Set(box.max.x, box.max.y, box.min.z);
	boxVertex[46].Set(box.max.x, box.max.y, box.max.z);
	boxVertex[47].Set(box.max.x, box.max.y, box.max.z);
	
	boxTangent[0].Set(1.0F, 0.0F, 0.0F, -scale[0]);
	boxTangent[1].Set(1.0F, 0.0F, 0.0F, scale[0]);
	boxTangent[2].Set(1.0F, 0.0F, 0.0F, scale[1]);
	boxTangent[3].Set(1.0F, 0.0F, 0.0F, -scale[1]);
	
	boxTangent[4].Set(1.0F, 0.0F, 0.0F, -scale[2]);
	boxTangent[5].Set(1.0F, 0.0F, 0.0F, scale[2]);
	boxTangent[6].Set(1.0F, 0.0F, 0.0F, scale[3]);
	boxTangent[7].Set(1.0F, 0.0F, 0.0F, -scale[3]);
	
	boxTangent[8].Set(1.0F, 0.0F, 0.0F, -scale[4]);
	boxTangent[9].Set(1.0F, 0.0F, 0.0F, scale[4]);
	boxTangent[10].Set(1.0F, 0.0F, 0.0F, scale[5]);
	boxTangent[11].Set(1.0F, 0.0F, 0.0F, -scale[5]);
	
	boxTangent[12].Set(1.0F, 0.0F, 0.0F, -scale[6]);
	boxTangent[13].Set(1.0F, 0.0F, 0.0F, scale[6]);
	boxTangent[14].Set(1.0F, 0.0F, 0.0F, scale[7]);
	boxTangent[15].Set(1.0F, 0.0F, 0.0F, -scale[7]);
	
	boxTangent[16].Set(0.0F, 1.0F, 0.0F, -scale[0]);
	boxTangent[17].Set(0.0F, 1.0F, 0.0F, scale[0]);
	boxTangent[18].Set(0.0F, 1.0F, 0.0F, scale[2]);
	boxTangent[19].Set(0.0F, 1.0F, 0.0F, -scale[2]);
	
	boxTangent[20].Set(0.0F, 1.0F, 0.0F, -scale[1]);
	boxTangent[21].Set(0.0F, 1.0F, 0.0F, scale[1]);
	boxTangent[22].Set(0.0F, 1.0F, 0.0F, scale[3]);
	boxTangent[23].Set(0.0F, 1.0F, 0.0F, -scale[3]);
	
	boxTangent[24].Set(0.0F, 1.0F, 0.0F, -scale[4]);
	boxTangent[25].Set(0.0F, 1.0F, 0.0F, scale[4]);
	boxTangent[26].Set(0.0F, 1.0F, 0.0F, scale[6]);
	boxTangent[27].Set(0.0F, 1.0F, 0.0F, -scale[6]);
	
	boxTangent[28].Set(0.0F, 1.0F, 0.0F, -scale[5]);
	boxTangent[29].Set(0.0F, 1.0F, 0.0F, scale[5]);
	boxTangent[30].Set(0.0F, 1.0F, 0.0F, scale[7]);
	boxTangent[31].Set(0.0F, 1.0F, 0.0F, -scale[7]);
	
	boxTangent[32].Set(0.0F, 0.0F, 1.0F, -scale[0]);
	boxTangent[33].Set(0.0F, 0.0F, 1.0F, scale[0]);
	boxTangent[34].Set(0.0F, 0.0F, 1.0F, scale[4]);
	boxTangent[35].Set(0.0F, 0.0F, 1.0F, -scale[4]);
	
	boxTangent[36].Set(0.0F, 0.0F, 1.0F, -scale[1]);
	boxTangent[37].Set(0.0F, 0.0F, 1.0F, scale[1]);
	boxTangent[38].Set(0.0F, 0.0F, 1.0F, scale[5]);
	boxTangent[39].Set(0.0F, 0.0F, 1.0F, -scale[5]);
	
	boxTangent[40].Set(0.0F, 0.0F, 1.0F, -scale[2]);
	boxTangent[41].Set(0.0F, 0.0F, 1.0F, scale[2]);
	boxTangent[42].Set(0.0F, 0.0F, 1.0F, scale[6]);
	boxTangent[43].Set(0.0F, 0.0F, 1.0F, -scale[6]);
	
	boxTangent[44].Set(0.0F, 0.0F, 1.0F, -scale[3]);
	boxTangent[45].Set(0.0F, 0.0F, 1.0F, scale[3]);
	boxTangent[46].Set(0.0F, 0.0F, 1.0F, scale[7]);
	boxTangent[47].Set(0.0F, 0.0F, 1.0F, -scale[7]);
	
	faceVertex[0].Set(box.max.x, box.min.y, box.min.z);
	faceVertex[1].Set(box.max.x, box.max.y, box.min.z);
	faceVertex[2].Set(box.max.x, box.max.y, box.max.z);
	faceVertex[3].Set(box.max.x, box.min.y, box.max.z);
	
	faceVertex[4].Set(box.min.x, box.max.y, box.min.z);
	faceVertex[5].Set(box.min.x, box.min.y, box.min.z);
	faceVertex[6].Set(box.min.x, box.min.y, box.max.z);
	faceVertex[7].Set(box.min.x, box.max.y, box.max.z);
	
	faceVertex[8].Set(box.max.x, box.max.y, box.min.z);
	faceVertex[9].Set(box.min.x, box.max.y, box.min.z);
	faceVertex[10].Set(box.min.x, box.max.y, box.max.z);
	faceVertex[11].Set(box.max.x, box.max.y, box.max.z);
	
	faceVertex[12].Set(box.min.x, box.min.y, box.min.z);
	faceVertex[13].Set(box.max.x, box.min.y, box.min.z);
	faceVertex[14].Set(box.max.x, box.min.y, box.max.z);
	faceVertex[15].Set(box.min.x, box.min.y, box.max.z);
	
	faceVertex[16].Set(box.min.x, box.min.y, box.max.z);
	faceVertex[17].Set(box.max.x, box.min.y, box.max.z);
	faceVertex[18].Set(box.max.x, box.max.y, box.max.z);
	faceVertex[19].Set(box.min.x, box.max.y, box.max.z);
	
	faceVertex[20].Set(box.min.x, box.max.y, box.min.z);
	faceVertex[21].Set(box.max.x, box.max.y, box.min.z);
	faceVertex[22].Set(box.max.x, box.min.y, box.min.z);
	faceVertex[23].Set(box.min.x, box.min.y, box.min.z);
	
	renderData->gizmoList->Append(&boxRenderable);
	renderData->gizmoList->Append(&faceRenderable);
	
	int32 index = hiliteEdgeIndex;
	if (index >= 0)
	{
		if (index < 4)
		{
			float y = box[index & 1].y;
			float z = box[(index >> 1) & 1].z;
			edgeVertex[0].Set(box.min.x, y, z);
			edgeVertex[1].Set(box.min.x, y, z);
			edgeVertex[2].Set(box.max.x, y, z);
			edgeVertex[3].Set(box.max.x, y, z);
			
			index *= 2;
			float s1 = scale[index];
			float s2 = scale[index + 1];
			edgeTangent[0].Set(1.0F, 0.0F, 0.0F, -s1 * 5.0F);
			edgeTangent[1].Set(1.0F, 0.0F, 0.0F, s1 * 5.0F);
			edgeTangent[2].Set(1.0F, 0.0F, 0.0F, s2 * 5.0F);
			edgeTangent[3].Set(1.0F, 0.0F, 0.0F, -s2 * 5.0F);
		}
		else if (index < 8)
		{
			index -= 4;
			float x = box[index & 1].x;
			float z = box[(index >> 1) & 1].z;
			edgeVertex[0].Set(x, box.min.y, z);
			edgeVertex[1].Set(x, box.min.y, z);
			edgeVertex[2].Set(x, box.max.y, z);
			edgeVertex[3].Set(x, box.max.y, z);
			
			index += index & 2;
			float s1 = scale[index];
			float s2 = scale[index + 2];
			edgeTangent[0].Set(0.0F, 1.0F, 0.0F, -s1 * 5.0F);
			edgeTangent[1].Set(0.0F, 1.0F, 0.0F, s1 * 5.0F);
			edgeTangent[2].Set(0.0F, 1.0F, 0.0F, s2 * 5.0F);
			edgeTangent[3].Set(0.0F, 1.0F, 0.0F, -s2 * 5.0F);
		}
		else
		{
			index -= 8;
			float x = box[index & 1].x;
			float y = box[(index >> 1) & 1].y;
			edgeVertex[0].Set(x, y, box.min.z);
			edgeVertex[1].Set(x, y, box.min.z);
			edgeVertex[2].Set(x, y, box.max.z);
			edgeVertex[3].Set(x, y, box.max.z);
			
			float s1 = scale[index];
			float s2 = scale[index + 4];
			edgeTangent[0].Set(0.0F, 0.0F, 1.0F, -s1 * 5.0F);
			edgeTangent[1].Set(0.0F, 0.0F, 1.0F, s1 * 5.0F);
			edgeTangent[2].Set(0.0F, 0.0F, 1.0F, s2 * 5.0F);
			edgeTangent[3].Set(0.0F, 0.0F, 1.0F, -s2 * 5.0F);
		}
		
		renderData->gizmoList->Append(&edgeRenderable);
	}
}


EditorConnector::EditorConnector(const EditorManipulator *manipulator, Connector *connector, int32 index) :
		lineWidget1(Vector2D(32.0F, 1.0F), kLineDotted1, K::white),
		lineWidget2(Vector2D(24.0F, 1.0F), kLineDotted1, K::white),
		backgroundWidget(Vector2D(96.0F, 18.0F), kConnectorBackgroundColor),
		borderWidget(Vector2D(96.0F, 18.0F), kLineSolid, K::white),
		textWidget(Vector2D(96.0F, 18.0F), connector->GetConnectorKey(), "font/Page"),
		lineDiffuseColor(kConnectorLineColor, kAttributeMutable),
		lineTextureMap("WorldEditor/arrow"),
		lineRenderable(kRenderTriangleStrip)
{
	connectorObject = connector;
	connectorNode = manipulator->GetTargetNode();
	connectorIndex = index;
	
	lineWidget1.SetWidgetPosition(Point3D(0.0F, 9.0F, 0.0F));
	lineWidget2.SetWidgetTransform(K::minus_y_unit, K::x_unit, K::z_unit, Point3D(0.0F, 10.0F, 0.0F));
	
	backgroundWidget.SetWidgetPosition(Point3D(32.0F, 0.0F, 0.0F));
	borderWidget.SetWidgetPosition(Point3D(32.0F, 0.0F, 0.0F));
	
	textWidget.SetWidgetColor(K::white);
	textWidget.SetTextAlignment(kTextAlignCenter);
	textWidget.SetWidgetPosition(Point3D(32.0F, 3.0F, 0.0F));
	
	groupWidget.AddSubnode(&lineWidget1);
	groupWidget.AddSubnode(&lineWidget2);
	groupWidget.AddSubnode(&backgroundWidget);
	groupWidget.AddSubnode(&borderWidget);
	groupWidget.AddSubnode(&textWidget);
	groupWidget.Preprocess();
	
	lineAttributeList.Append(&lineDiffuseColor);
	lineAttributeList.Append(&lineTextureMap);
	lineRenderable.SetMaterialAttributeList(&lineAttributeList);
	lineRenderable.SetAmbientBlendState(kBlendInterpolate);
	lineRenderable.SetShaderFlags(kShaderAmbientEffect | kShaderVertexPolyboard | kShaderLinearPolyboard | kShaderOrthoPolyboard);
	lineRenderable.SetRenderParameterPointer(&manipulator->GetManipulatorScaleVector());
	lineRenderable.SetVertexCount(66);
	lineRenderable.SetAttributeArray(kArrayVertex, lineVertex);
	lineRenderable.SetAttributeArray(kArrayTangent, lineTangent);
	lineRenderable.SetAttributeArray(kArrayTexture0, lineTexcoord);
}

EditorConnector::~EditorConnector()
{
}

void EditorConnector::Select(void)
{
	ColorRGBA color = TheInterfaceMgr->GetInterfaceColor(kInterfaceColorHilite);
	backgroundWidget.SetVertexColor(1, color);
	backgroundWidget.SetVertexColor(3, color);
	
	color.red *= 0.75F;
	color.green *= 0.75F;
	color.blue *= 0.75F;
	backgroundWidget.SetVertexColor(0, color);
	backgroundWidget.SetVertexColor(2, color);
	
	lineDiffuseColor.SetDiffuseColor(K::white);
}

void EditorConnector::Unselect(void)
{
	backgroundWidget.SetWidgetColor(kConnectorBackgroundColor);
	lineDiffuseColor.SetDiffuseColor(kConnectorLineColor);
}

bool EditorConnector::Pick(const ManipulatorViewportData *viewportData, const Ray *ray) const
{
	float scale = viewportData->viewportScale;
	const Transform4D& cameraTransform = viewportData->viewportCamera->GetNodeTransform();
	Vector3D p = ray->origin - GetConnectorPosition(cameraTransform, scale);
	
	scale = 1.0F / scale;
	float x = p * cameraTransform[0] * scale;
	float y = p * cameraTransform[1] * scale;
	
	return ((x > 32.0F) && (x < 128.0F) && (y > 0.0F) && (y < 18.0F));
}

Point3D EditorConnector::GetConnectorPosition(const Transform4D& cameraTransform, float scale) const
{
	Vector3D position = connectorNode->GetWorldPosition() - cameraTransform.GetTranslation();
	position.x = Floor(position.x / scale);
	position.y = Floor(position.y / scale);
	position.z = Floor(position.z / scale);
	
	position += cameraTransform[1] * ((float) connectorIndex * 24.0F + 13.0F);
	return (cameraTransform.GetTranslation() + position * scale);
}

void EditorConnector::RenderBox(const ManipulatorViewportData *viewportData, List<Renderable> *renderList)
{
	float scale = viewportData->viewportScale;
	const Transform4D& cameraTransform = viewportData->viewportCamera->GetNodeTransform();
	
	groupWidget.SetWidgetTransform(cameraTransform[0] * scale, cameraTransform[1] * scale, cameraTransform[2] * scale, GetConnectorPosition(cameraTransform, scale));
	groupWidget.Invalidate();
	groupWidget.Update();
	
	groupWidget.RenderTree(renderList);
}

void EditorConnector::RenderLine(const ManipulatorViewportData *viewportData, List<Renderable> *renderList)
{
	const Node *target = GetConnectorTarget();
	if (target)
	{
		float scale = viewportData->viewportScale;
		const Transform4D& cameraTransform = viewportData->viewportCamera->GetNodeTransform();
		
		Point3D p1 = GetConnectorPosition(cameraTransform, scale) + cameraTransform[0] * (scale * 128.0F) + cameraTransform[1] * (scale * 9.0F);
		Point3D p2 = p1 + cameraTransform[0] * (scale * 256.0F);
		Point3D texpoint = p1;
		
		const EditorManipulator *targetManipulator = Editor::GetManipulator(target);
		const BoundingSphere *sphere = targetManipulator->GetNodeSphere();
		const Point3D& p3 = ((sphere) && (!(targetManipulator->GetManipulatorState() & kManipulatorShowIcon))) ? sphere->GetCenter() : target->GetWorldPosition();
		
		lineVertex[0] = p1;
		lineVertex[1] = p1;
		lineVertex[64] = p3;
		lineVertex[65] = p3;
		
		Vector3D tangent = (p2 - p1).Normalize();
		float radius = scale * 6.0F;
		
		lineTangent[0].Set(tangent, -radius);
		lineTangent[1].Set(tangent, radius);
		lineTexcoord[0].Set(0.0F, 0.0F);
		lineTexcoord[1].Set(0.0F, 1.0F);
		
		float u = 0.03125F;
		float dtex = 0.03125F / scale;
		float texcoord = 0.0F;
		
		for (machine a = 2; a < 66; a += 2)
		{
			float v = 1.0F - u;
			float u2 = u * u;
			float v2 = v * v;
			
			Point3D p = p1 * v2 + p2 * (u * v * 2.0F) + p3 * u2;
			Vector3D t = p1 * -v + p2 * (1.0F - u * 2.0F) + p3 * u;
			t.Normalize();
			
			lineVertex[a] = p;
			lineVertex[a + 1] = p;
			
			lineTangent[a].Set(t, -radius);
			lineTangent[a + 1].Set(t, radius);
			
			texcoord += Magnitude(p - texpoint) * dtex;
			texpoint = p;
			
			lineTexcoord[a].Set(texcoord, 0.0F);
			lineTexcoord[a + 1].Set(texcoord, 1.0F);
			
			u += 0.03125F;
		}
		
		texcoord = PositiveCeil(texcoord) / texcoord;
		for (machine a = 2; a < 66; a += 2)
		{
			float s = lineTexcoord[a].x * texcoord;
			lineTexcoord[a].x = s;
			lineTexcoord[a + 1].x = s;
		}
		
		renderList->Append(&lineRenderable);
	}
}


ManipulatorWidget::ManipulatorWidget(EditorManipulator *manipulator) :
		RenderableWidget(kWidgetManipulator, kRenderQuads, Vector2D(kGraphBoxWidth, kGraphBoxHeight)),
		diffuseAttribute(K::black)
{
	editorManipulator = manipulator;
	viewportScale = 1.0F;
}

ManipulatorWidget::~ManipulatorWidget()
{
}

void ManipulatorWidget::Preprocess(void)
{
	RenderableWidget::Preprocess();
	
	SetAttributeArray(kArrayVertex, manipulatorVertex);
	
	attributeList.Append(&diffuseAttribute);
	SetMaterialAttributeList(&attributeList);
}

void ManipulatorWidget::Build(void)
{
	float scale = viewportScale;
	
	manipulatorVertex[0].Set(-scale, -scale, 0.0F);
	manipulatorVertex[1].Set(-scale, 0.0F, 0.0F);
	manipulatorVertex[2].Set(kGraphBoxWidth + scale, 0.0F, 0.0F);
	manipulatorVertex[3].Set(kGraphBoxWidth + scale, -scale, 0.0F);
	
	manipulatorVertex[4].Set(-scale, kGraphBoxHeight, 0.0F);
	manipulatorVertex[5].Set(-scale, kGraphBoxHeight + scale, 0.0F);
	manipulatorVertex[6].Set(kGraphBoxWidth + scale, kGraphBoxHeight + scale, 0.0F);
	manipulatorVertex[7].Set(kGraphBoxWidth + scale, kGraphBoxHeight, 0.0F);
	
	manipulatorVertex[8].Set(-scale, 0.0F, 0.0F);
	manipulatorVertex[9].Set(-scale, kGraphBoxHeight, 0.0F);
	manipulatorVertex[10].Set(0.0F, kGraphBoxHeight, 0.0F);
	manipulatorVertex[11].Set(0.0F, 0.0F, 0.0F);
	
	manipulatorVertex[12].Set(kGraphBoxWidth, 0.0F, 0.0F);
	manipulatorVertex[13].Set(kGraphBoxWidth, kGraphBoxHeight, 0.0F);
	manipulatorVertex[14].Set(kGraphBoxWidth + scale, kGraphBoxHeight, 0.0F);
	manipulatorVertex[15].Set(kGraphBoxWidth + scale, 0.0F, 0.0F);
	
	int32 count = 16;
	const Node *node = editorManipulator->GetTargetNode();
	
	if (node->GetFirstSubnode())
	{
		count = 20;
		manipulatorVertex[16].Set(kGraphBoxWidth + 1.0F, 7.0F, 0.0F);
		manipulatorVertex[17].Set(kGraphBoxWidth + 1.0F, 7.0F + scale, 0.0F);
		manipulatorVertex[18].Set(kGraphBoxWidth + 10.0F, 7.0F + scale, 0.0F);
		manipulatorVertex[19].Set(kGraphBoxWidth + 10.0F, 7.0F, 0.0F);
	}
	
	if (node->GetSuperNode())
	{
		const Node *previous = node->Previous();
		if (previous)
		{
			manipulatorVertex[count].Set(-14.0F, 7.0F, 0.0F);
			manipulatorVertex[count + 1].Set(-14.0F, 7.0F + scale, 0.0F);
			manipulatorVertex[count + 2].Set(-1.0F, 7.0F + scale, 0.0F);
			manipulatorVertex[count + 3].Set(-1.0F, 7.0F, 0.0F);
			
			float h = static_cast<EditorManipulator *>(previous->GetManipulator())->GetGraphHeight();
			float y = (previous->Previous()) ? 8.0F - h : 13.0F - h;
			
			manipulatorVertex[count + 4].Set(-15.0F, y, 0.0F);
			manipulatorVertex[count + 5].Set(-15.0F, 8.0F, 0.0F);
			manipulatorVertex[count + 6].Set(scale - 15.0F, 8.0F, 0.0F);
			manipulatorVertex[count + 7].Set(scale - 15.0F, y, 0.0F);
			count += 8;
		}
		else
		{
			manipulatorVertex[count].Set(-9.0F, 7.0F, 0.0F);
			manipulatorVertex[count + 1].Set(-9.0F, 7.0F + scale, 0.0F);
			manipulatorVertex[count + 2].Set(-1.0F, 7.0F + scale, 0.0F);
			manipulatorVertex[count + 3].Set(-1.0F, 7.0F, 0.0F);
			count += 4;
		}
	}
	
	SetVertexCount(count);
}


EditorManipulator::EditorManipulator(Node *node, const char *iconName) :
		Manipulator(node),
		iconTextureMap(iconName),
		iconRenderable(kRenderQuads, kRenderDepthTest | kRenderDepthInhibit),
		markerDiffuseColor(kUnselectedMarkerColor, kAttributeMutable),
		markerTextureMap("WorldEditor/marker"),
		markerRenderable(kRenderQuads, kRenderDepthTest | kRenderDepthInhibit | kRenderAlphaTest),
		handleRenderable(kRenderQuads),
		connectorTextureMap("WorldEditor/connector"),
		connectorRenderable(kRenderQuads),
		graphBackground(Vector2D(kGraphBoxWidth + 8.0F, kGraphBoxHeight + 8.0F), "WorldEditor/graph"),
		graphImage(Vector2D(16.0F, 16.0F)),
		graphText(Vector2D(kGraphBoxWidth - 21.0F, kGraphBoxHeight), nullptr, "font/Normal"),
		graphBorder(this),
		graphCollapseButton(Vector2D(13.0F, 13.0F), Point2D(0.5625F, 0.6875F), Point2D(0.6875F, 0.8125F)),
		graphCollapseObserver(this, &EditorManipulator::HandleGraphCollapseEvent)
{
	manipulatorFlags = 0;
	
	worldEditor = nullptr;
	editorGizmo = nullptr;
	
	selectionType = kEditorSelectionObject;
	
	nodeSpherePointer = nullptr;
	treeSpherePointer = nullptr;
	
	handleCount = 0;
	connectorCount = 0;
	connectorStorage = nullptr;
	
	iconAttributeList.Append(&iconTextureMap);
	iconRenderable.SetMaterialAttributeList(&iconAttributeList);
	iconRenderable.SetAmbientBlendState(kBlendInterpolate);
	iconRenderable.SetShaderFlags(kShaderAmbientEffect | kShaderVertexBillboard | kShaderScaleVertex);
	iconRenderable.SetRenderParameterPointer(&manipulatorScaleVector);
	iconRenderable.SetVertexCount(4);
	iconRenderable.SetAttributeArray(kArrayVertex, &markerVertex[0]);
	iconRenderable.SetAttributeArray(kArrayTexture0, &iconTexcoord[0]);
	iconRenderable.SetAttributeArray(kArrayBillboard, &iconBillboard[0]);
	iconRenderable.SetTransformable(node);
	
	markerAttributeList.Append(&markerDiffuseColor);
	markerAttributeList.Append(&markerTextureMap);
	markerRenderable.SetMaterialAttributeList(&markerAttributeList);
	markerRenderable.SetAmbientBlendState(kBlendInterpolate);
	markerRenderable.SetShaderFlags(kShaderAmbientEffect | kShaderVertexBillboard | kShaderScaleVertex);
	markerRenderable.SetRenderParameterPointer(&manipulatorScaleVector);
	markerRenderable.SetVertexCount(4);
	markerRenderable.SetAttributeArray(kArrayVertex, &markerVertex[0]);
	markerRenderable.SetAttributeArray(kArrayTexture0, &markerTexcoord[0]);
	markerRenderable.SetAttributeArray(kArrayBillboard, &markerBillboard[0]);
	markerRenderable.SetTransformable(node);
	
	handleRenderable.SetAmbientBlendState(kBlendInterpolate);
	handleRenderable.SetShaderFlags(kShaderAmbientEffect | kShaderVertexBillboard | kShaderScaleVertex);
	handleRenderable.SetRenderParameterPointer(&manipulatorScaleVector);
	handleRenderable.SetAttributeArray(kArrayVertex, handleVertex);
	handleRenderable.SetAttributeArray(kArrayBillboard, &handleBillboard[0]);
	handleRenderable.SetTransformable(node);
	
	connectorAttributeList.Append(&connectorTextureMap);
	connectorRenderable.SetMaterialAttributeList(&connectorAttributeList);
	connectorRenderable.SetAmbientBlendState(kBlendInterpolate);
	connectorRenderable.SetShaderFlags(kShaderAmbientEffect | kShaderVertexBillboard | kShaderScaleVertex);
	connectorRenderable.SetRenderParameterPointer(&manipulatorScaleVector);
	connectorRenderable.SetTransformable(node);
	
	graphBackground.SetQuadOffset(Vector2D(-4.0F, -1.0F));
	
	graphText.SetWidgetPosition(Point3D(21.0F, 2.0F, 0.0F));
	graphText.SetTextFlags(kTextUnformatted | kTextClipped);
	
	graphCollapseButton.SetWidgetPosition(Point3D(120.0F, 2.0F, 0.0F));
	graphCollapseButton.SetWidgetColor(ColorRGBA(1.0F, 1.0F, 0.5F, 1.0F));
	graphCollapseButton.SetObserver(&graphCollapseObserver);
	
	graphBackground.AddSubnode(&graphImage);
	graphBackground.AddSubnode(&graphText);
	graphBackground.AddSubnode(&graphBorder);
	graphBackground.AddSubnode(&graphCollapseButton);
	graphBackground.Preprocess();
}

EditorManipulator::~EditorManipulator()
{
	ReleaseConnectorStorage();
	delete editorGizmo;
}

Manipulator *EditorManipulator::Construct(Node *node, unsigned_int32 flags)
{
	switch (node->GetNodeType())
	{
		case kNodeGroup:
			
			return (new GroupManipulator(node));
		
		case kNodeCamera:
			
			return (CameraManipulator::Construct(static_cast<Camera *>(node)));
		
		case kNodeLight:
			
			return (LightManipulator::Construct(static_cast<Light *>(node)));
		
		case kNodeSource:
			
			return (SourceManipulator::Construct(static_cast<Source *>(node)));
		
		case kNodeGeometry:
			
			return (GeometryManipulator::Construct(static_cast<Geometry *>(node)));
		
		case kNodeInstance:
			
			return (new InstanceManipulator(static_cast<Instance *>(node)));
		
		case kNodeModel:
			
			return (new ModelManipulator(static_cast<Model *>(node)));
		
		case kNodeBone:
			
			return (new BoneManipulator(static_cast<Bone *>(node)));
		
		case kNodeMarker:
			
			return (MarkerManipulator::Construct(static_cast<Marker *>(node)));
		
		case kNodeTrigger:
			
			return (TriggerManipulator::Construct(static_cast<Trigger *>(node)));
		
		case kNodeEffect:
			
			return (EffectManipulator::Construct(static_cast<Effect *>(node)));
		
		case kNodeEmitter:
			
			return (EmitterManipulator::Construct(static_cast<Emitter *>(node)));
		
		case kNodeSpace:
			
			return (SpaceManipulator::Construct(static_cast<Space *>(node)));
		
		case kNodePortal:
			
			return (PortalManipulator::Construct(static_cast<Portal *>(node)));
		
		case kNodeZone:
			
			return (ZoneManipulator::Construct(static_cast<Zone *>(node)));
		
		case kNodeShape:
			
			return (ShapeManipulator::Construct(static_cast<Shape *>(node)));
		
		case kNodeJoint:
			
			return (JointManipulator::Construct(static_cast<Joint *>(node)));
		
		case kNodeField:
			
			return (FieldManipulator::Construct(static_cast<Field *>(node)));
		
		case kNodePhysics:
			
			return (new PhysicsNodeManipulator(static_cast<PhysicsNode *>(node)));
		
		case kNodeSkybox:
			
			return (new SkyboxManipulator(static_cast<Skybox *>(node)));
		
		case kNodeImpostor:
			
			return (new ImpostorManipulator(static_cast<Impostor *>(node)));
		
		case kNodeTerrainBlock:
		
		#if C4LEGACY
		
			case 'BLCK':
		
		#endif
			
			return (new TerrainBlockManipulator(static_cast<TerrainBlock *>(node)));
		
		case kNodeWaterBlock:
			
			return (new WaterBlockManipulator(static_cast<WaterBlock *>(node)));
	}
	
	return (nullptr);
}

void EditorManipulator::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Manipulator::Pack(data, packFlags);
	
	if (graphCollapseButton.GetWidgetState() & kWidgetCollapsed)
	{
		data << ChunkHeader('CLPS', 0);
	}
	
	data << TerminatorChunk;
}

void EditorManipulator::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Manipulator::Unpack(data, unpackFlags);
	UnpackChunkList<EditorManipulator>(data, unpackFlags);
}

bool EditorManipulator::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'CLPS':
			
			graphCollapseButton.SetWidgetState(graphCollapseButton.GetWidgetState() | kWidgetCollapsed);
			return (true);
	}
	
	return (false);
}

const char *EditorManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', 'NODE')));
}

void EditorManipulator::Preprocess(void)
{
	Manipulator::Preprocess();
	
	AllocateConnectorStorage();
	
	UpdateGraphColor();
	graphImage.SetTexture(0, GetIconName());
	
	if (GetTargetNode()->GetNodeFlags() & kNodeNonpersistent) SetManipulatorState(GetManipulatorState() & ~kManipulatorShowIcon);
}

void EditorManipulator::Invalidate(void)
{
	EditorManipulator *manipulator = this;
	for (;;)
	{
		manipulator->SetManipulatorState(manipulator->GetManipulatorState() & ~kManipulatorUpdated);
		
		Node *super = manipulator->GetTargetNode()->GetSuperNode();
		if (!super) break;
		
		manipulator = static_cast<EditorManipulator *>(super->GetManipulator());
		if ((!manipulator) || (!(manipulator->GetManipulatorState() & kManipulatorUpdated))) break;
	}
	
	if (GetManipulatorState() & kManipulatorShowGizmo)
	{
		worldEditor->PostEvent(GizmoEditorEvent(kEditorEventGizmoTargetInvalidated, GetTargetNode()));
	}
}

void EditorManipulator::InvalidateGraph(void)
{
	SetManipulatorState(GetManipulatorState() & ~kManipulatorGraphValid);
	graphBackground.Invalidate();
	
	Node *node = GetTargetNode();
	
	Node *subnode = node->GetFirstSubnode();
	if (subnode)
	{
		EditorManipulator *manipulator = static_cast<EditorManipulator *>(subnode->GetManipulator());
		if (manipulator->GetManipulatorState() & kManipulatorGraphValid) manipulator->InvalidateGraph();
	}
	
	Node *next = node->Next();
	if (next)
	{
		EditorManipulator *manipulator = static_cast<EditorManipulator *>(next->GetManipulator());
		if (manipulator->GetManipulatorState() & kManipulatorGraphValid) manipulator->InvalidateGraph();
	}
	
	Node *super = node->GetSuperNode();
	if (super)
	{
		EditorManipulator *manipulator = static_cast<EditorManipulator *>(super->GetManipulator());
		if (manipulator->GetManipulatorState() & kManipulatorGraphValid) manipulator->InvalidateGraph();
	}
}

void EditorManipulator::InvalidateNode(void)
{
	GetTargetNode()->Invalidate();
}

void EditorManipulator::EnableGizmo(void)
{
	SetManipulatorState(GetManipulatorState() | kManipulatorShowGizmo);
	
	if (!editorGizmo) editorGizmo = new EditorGizmo(this);
}

void EditorManipulator::DisableGizmo(void)
{
	SetManipulatorState(GetManipulatorState() & ~kManipulatorShowGizmo);
	
	delete editorGizmo;
	editorGizmo = nullptr;
}

void EditorManipulator::Update(void)
{
	unsigned_int32 state = GetManipulatorState();
	if (!(state & kManipulatorUpdated))
	{
		Point3D		handlePosition[kMaxManipulatorHandleCount];
		
		SetManipulatorState(state | kManipulatorUpdated);
		const Node *node = GetTargetNode();
		
		const Node *subnode = node->GetFirstSubnode();
		while (subnode)
		{
			EditorManipulator *manipulator = static_cast<EditorManipulator *>(subnode->GetManipulator());
			manipulator->Update();
			
			subnode = subnode->Next();
		}
		
		nodeSpherePointer = nullptr;
		treeSpherePointer = nullptr;
		
		bool icon = ((state & kManipulatorShowIcon) != 0);
		
		if (CalculateNodeSphere(&nodeSphere))
		{
			nodeSphere.SetCenter(node->GetWorldTransform() * nodeSphere.GetCenter());
			nodeSpherePointer = &nodeSphere;
			
			if (icon)
			{
				BoundingSphere sphere(node->GetWorldPosition(), Editor::kFrustumRenderScale * 12.0F);
				nodeSphere.Union(&sphere);
			}
			
			treeSphere = nodeSphere;
			treeSpherePointer = &treeSphere;
		}
		else if (icon)
		{
			nodeSphere.SetCenter(node->GetWorldPosition());
			nodeSphere.SetRadius(Editor::kFrustumRenderScale * 12.0F);
			nodeSpherePointer = &nodeSphere;
			
			treeSphere = nodeSphere;
			treeSpherePointer = &treeSphere;
		}
		else
		{
			nodeSphere.SetCenter(node->GetWorldPosition());
			nodeSphere.SetRadius(Editor::kFrustumRenderScale);
			nodeSpherePointer = &nodeSphere;
			
			treeSphere = nodeSphere;
			treeSpherePointer = &treeSphere;
		}
		
		int32 count = GetHandleTable(handlePosition);
		handleRenderable.SetVertexCount(count * 4);
		handleCount = count;
		
		Point3D *vertex = handleVertex;
		for (machine a = 0; a < count; a++)
		{
			const Point3D& p = handlePosition[a];
			vertex[0] = p;
			vertex[1] = p;
			vertex[2] = p;
			vertex[3] = p;
			vertex += 4;
		}
		
		subnode = GetTargetNode()->GetFirstSubnode();
		while (subnode)
		{
			EditorManipulator *manipulator = static_cast<EditorManipulator *>(subnode->GetManipulator());
			
			const BoundingSphere *sphere = manipulator->GetTreeSphere();
			if (sphere)
			{
				if (treeSpherePointer)
				{
					treeSphere.Union(sphere);
				}
				else
				{
					treeSphere = *sphere;
					nodeSpherePointer = &treeSphere;
					treeSpherePointer = &treeSphere;
				}
			}
			
			subnode = subnode->Next();
		}
	}
}

void EditorManipulator::UpdateGraph(void)
{
	unsigned_int32 state = GetManipulatorState();
	if (!(state & kManipulatorGraphValid))
	{
		SetManipulatorState(state | kManipulatorGraphValid);
		const Node *node = GetTargetNode();
		
		const Node *previous = node->Previous();
		if (previous)
		{
			const EditorManipulator *manipulator = static_cast<EditorManipulator *>(previous->GetManipulator());
			const Point3D& position = manipulator->GetGraphPosition();
			graphBackground.SetWidgetPosition(Point3D(position.x, position.y + manipulator->GetGraphHeight(), 0.0F));
		}
		else
		{
			const Node *super = node->GetSuperNode();
			if (super)
			{
				const EditorManipulator *manipulator = static_cast<EditorManipulator *>(super->GetManipulator());
				const Point3D& position = manipulator->GetGraphPosition();
				graphBackground.SetWidgetPosition(Point3D(position.x + kGraphBoxWidth + 29.0F, position.y, 0.0F));
			}
			else
			{
				graphBackground.SetWidgetPosition(Point3D(0.0F, 0.0F, 0.0F));
			}
		}
		
		const char *name = node->GetNodeName();
		graphText.SetText((name) ? name : GetDefaultNodeName());
		
		const Node *subnode = node->GetFirstSubnode();
		if (subnode)
		{
			float width = 0.0F;
			float height = 0.0F;
			do
			{
				EditorManipulator *manipulator = static_cast<EditorManipulator *>(subnode->GetManipulator());
				manipulator->UpdateGraph();
				
				width = Fmax(width, manipulator->GetGraphWidth());
				height += manipulator->GetGraphHeight();
				
				subnode = subnode->Next();
			} while (subnode);
			
			if (!(graphCollapseButton.GetWidgetState() & kWidgetCollapsed))
			{
				graphWidth = width + kGraphBoxWidth + 29.0F;
				graphHeight = height;
			}
			else
			{
				graphWidth = kGraphBoxWidth + 29.0F;
				graphHeight = kGraphBoxHeight + 12.0F;
			}
			
			graphCollapseButton.Show();
		}
		else
		{
			graphWidth = kGraphBoxWidth;
			const Node *next = node->Next();
			if ((next) && (!next->GetFirstSubnode())) graphHeight = kGraphBoxHeight + 8.0F;
			else graphHeight = kGraphBoxHeight + 12.0F;
			
			graphCollapseButton.Hide();
		}
		
		graphBackground.Update();
	}
}

void EditorManipulator::UpdateGraphColor(void)
{
	if (!(GetTargetNode()->GetNodeFlags() & kNodeNonpersistent))
	{
		if (!Selected())
		{
			if (!Hidden())
			{
				graphBackground.SetWidgetColor(ColorRGBA(1.0F, 1.0F, 1.0F, 1.0F));
			}
			else
			{
				graphBackground.SetWidgetColor(ColorRGBA(0.5F, 0.5F, 0.5F, 1.0F));
			}
		}
		else
		{
			graphBackground.SetWidgetColor(TheInterfaceMgr->GetInterfaceColor(kInterfaceColorHilite));
		}
	}
	else
	{
		graphBackground.SetWidgetColor(ColorRGBA(1.0F, 0.5F, 0.5F, 1.0F));
	}
}

void EditorManipulator::HandleGraphCollapseEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		InvalidateGraph();
		widget->SetWidgetState(widget->GetWidgetState() ^ kWidgetCollapsed);
	}
}

bool EditorManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	if (!GetTargetNode()->CalculateBoundingSphere(sphere))
	{
		sphere->SetCenter(Zero3D);
		sphere->SetRadius(0.0F);
	}
	
	return (true);
}

bool EditorManipulator::PickLineSegment(const Ray *ray, const Point3D& p1, const Point3D& p2, float r2, float *param)
{
	float	u1, u2;
	
	Vector3D dp = p2 - p1;
	if (Math::CalculateNearestParameters(ray->origin, ray->direction, p1, dp, &u1, &u2))
	{
		if ((u1 > ray->tmin) && (u1 < ray->tmax) && (u2 > 0.0F) && (u2 < 1.0F))
		{
			if (SquaredMag(ray->origin + ray->direction * u1 - p1 - dp * u2) < r2)
			{
				*param = u1;
				return (true);
			}
		}
	}
	
	return (false);
}

bool EditorManipulator::RegionPickLineSegment(const Region *region, const Point3D& p1, const Point3D& p2) const
{
	const Transform4D& worldTransform = GetTargetNode()->GetWorldTransform();
	return (region->CylinderVisible(worldTransform * p1, worldTransform * p2, 0.0F));
}

void EditorManipulator::Show(void)
{
	SetManipulatorState(GetManipulatorState() & ~kManipulatorHidden);
	UpdateGraphColor();
}

void EditorManipulator::Hide(void)
{
	SetManipulatorState(GetManipulatorState() | kManipulatorHidden);
	UpdateGraphColor();
}

bool EditorManipulator::PredecessorSelected(void) const
{
	const Node *node = GetTargetNode();
	for (;;)
	{
		node = node->GetSuperNode();
		if (!node) break;
		
		if (node->GetManipulator()->Selected()) return (true);
	}
	
	return (false);
}

void EditorManipulator::Select(void)
{
	Show();
	
	SetManipulatorState(GetManipulatorState() | kManipulatorSelected);
	selectionType = kEditorSelectionObject;
	
	markerDiffuseColor.SetDiffuseColor(K::white);
	UpdateGraphColor();
}

void EditorManipulator::Unselect(void)
{
	SetManipulatorState(GetManipulatorState() & ~(kManipulatorSelected | kManipulatorTempSelected));
	selectionType = kEditorSelectionObject;
	
	markerDiffuseColor.SetDiffuseColor(kUnselectedMarkerColor);
	UpdateGraphColor();
}

void EditorManipulator::HandleDelete(bool undoable)
{
	SetManipulatorState(GetManipulatorState() | kManipulatorDeleted);
	UnselectConnector();
}

void EditorManipulator::HandleUndelete(void)
{
	SetManipulatorState(GetManipulatorState() & ~kManipulatorDeleted);
}

void EditorManipulator::HandleSizeUpdate(int32 count, const float *size)
{
	Node *node = GetTargetNode();
	Object *object = node->GetObject();
	if (object)
	{
		float	objectSize[kMaxObjectSizeCount];
		
		for (machine a = 0; a < count; a++) objectSize[a] = Fmax(size[a], kSizeEpsilon);
		
		object->SetObjectSize(objectSize);
		worldEditor->InvalidateNode(node);
	}
}

void EditorManipulator::HandleSettingsUpdate(void)
{
	GetTargetNode()->Invalidate();
	InvalidateGraph();
}

void EditorManipulator::HandleConnectorUpdate(void)
{
	GetTargetNode()->ProcessInternalConnectors();
}

bool EditorManipulator::MaterialSettable(void) const
{
	return (false);
}

bool EditorManipulator::MaterialRemovable(void) const
{
	return (false);
}

const MaterialObject *EditorManipulator::PickupMaterial(void) const
{
	return (nullptr);
}

void EditorManipulator::SetMaterial(MaterialObject *materialObject)
{
}

void EditorManipulator::RemoveMaterial(void)
{
}

Box3D EditorManipulator::CalculateNodeBoundingBox(void) const
{
	return (Box3D(Zero3D, Zero3D));
}

Box3D EditorManipulator::CalculateWorldBoundingBox(void) const
{
	return (Transform(CalculateNodeBoundingBox(), GetTargetNode()->GetWorldTransform()));
}

void EditorManipulator::AdjustBoundingBox(Box3D *box)
{
	Vector3D size = box->GetSize();
	
	if (size.x < 0.25F)
	{
		float x = (box->min.x + box->max.x) * 0.5F;
		box->min.x = x - 0.125F;
		box->max.x = x + 0.125F;
	}
	
	if (size.y < 0.25F)
	{
		float y = (box->min.y + box->max.y) * 0.5F;
		box->min.y = y - 0.125F;
		box->max.y = y + 0.125F;
	}
	
	if (size.z < 0.25F)
	{
		float z = (box->min.z + box->max.z) * 0.5F;
		box->min.z = z - 0.125F;
		box->max.z = z + 0.125F;
	}
	
	float expand = Fmax(size.x, size.y, size.z) * 0.03125F;
	box->min -= Vector3D(expand, expand, expand);
	box->max += Vector3D(expand, expand, expand);
}

bool EditorManipulator::Pick(const Ray *ray, PickData *data) const
{
	if (GetManipulatorState() & kManipulatorShowIcon)
	{
		float	t2;
		
		float r = ray->radius * 11.0F;
		if (r == 0.0F) r = Editor::kFrustumRenderScale * 12.0F;
		
		return (Math::IntersectRayAndSphere(ray, Zero3D, r, &data->rayParam, &t2));
	}
	
	return (false);
}

bool EditorManipulator::RegionPick(const Region *region) const
{
	return (region->SphereVisible(GetTargetNode()->GetWorldPosition(), 0.0F));
}

int32 EditorManipulator::GetHandleTable(Point3D *handle) const
{
	return (0);
}

void EditorManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	handleData->handleFlags = 0;
	handleData->oppositeIndex = kHandleOrigin;
}

void EditorManipulator::BeginResize(const ManipulatorResizeData *resizeData)
{
	const Node *node = GetTargetNode();
	const Object *object = node->GetObject();
	if (object) object->GetObjectSize(originalSize);
	originalPosition = node->GetNodePosition();
}

bool EditorManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	return (false);
}

void EditorManipulator::AllocateConnectorStorage(void)
{
	ReleaseConnectorStorage();
	
	connectorCount = 0;
	
	const Hub *hub = GetTargetNode()->GetHub();
	if (hub)
	{
		int32 count = hub->GetOutgoingEdgeCount();
		connectorCount = count;
		if (count != 0)
		{
			connectorStorage = new char[sizeof(EditorConnector) * count];
			editorConnector = reinterpret_cast<EditorConnector *>(connectorStorage);
			
			Connector *connector = hub->GetFirstOutgoingEdge();
			for (machine a = 0; a < count; a++)
			{
				new(&editorConnector[a]) EditorConnector(this, connector, a);
				connector = connector->GetNextOutgoingEdge();
			}
		}
	}
}

void EditorManipulator::ReleaseConnectorStorage(void)
{
	if (connectorStorage)
	{
		for (machine index = connectorCount - 1; index >= 0; index--) editorConnector[index].~EditorConnector();
		delete[] connectorStorage;
		connectorStorage = nullptr;
	}
}

void EditorManipulator::UpdateConnectors(void)
{
	AllocateConnectorStorage();
	SetManipulatorState(GetManipulatorState() & ~kManipulatorConnectorSelected);
	Detach();
}

void EditorManipulator::SelectConnector(int32 index, bool toggle)
{
	unsigned_int32 state = GetManipulatorState();
	if (state & kManipulatorConnectorSelected)
	{
		if (connectorSelection == index)
		{
			if (toggle)
			{
				editorConnector[index].Unselect();
				SetManipulatorState(state & ~kManipulatorConnectorSelected);
			}
			
			return;
		}
		
		editorConnector[connectorSelection].Unselect();
	}
	else
	{
		SetManipulatorState(state | kManipulatorConnectorSelected);
	}
	
	connectorSelection = index;
	editorConnector[index].Select();
}

void EditorManipulator::UnselectConnector(void)
{
	unsigned_int32 state = GetManipulatorState();
	if (state & kManipulatorConnectorSelected)
	{
		SetManipulatorState(state & ~kManipulatorConnectorSelected);
		editorConnector[connectorSelection].Unselect();
		Detach();
	}
}

bool EditorManipulator::SetConnectorTarget(int32 index, Node *target)
{
	Node *node = GetTargetNode();
	const Hub *hub = node->GetHub();
	if (hub)
	{
		Connector *connector = hub->GetOutgoingEdge(index);
		if (connector)
		{
			if (target)
			{
				if (node->ValidConnectedNode(connector->GetConnectorKey(), target))
				{
					connector->SetConnectorTarget(target);
					HandleConnectorUpdate();
					return (true);
				}
			}
			else
			{
				connector->SetConnectorTarget(nullptr);
				HandleConnectorUpdate();
				return (true);
			}
		}
	}
	
	return (false);
}

bool EditorManipulator::PickConnector(const ManipulatorViewportData *viewportData, const Ray *ray, PickData *pickData) const
{
	int32 count = connectorCount;
	for (machine a = count - 1; a >= 0; a--)
	{
		if (editorConnector[a].Pick(viewportData, ray))
		{
			pickData->pickIndex[0] = a;
			return (true);
		}
	}
	
	return (false);
}

Box2D EditorManipulator::GetGraphBox(void) const
{
	const Point2D& p = GetGraphPosition().GetPoint2D();
	return (Box2D(p, Point2D(p.x + kGraphBoxWidth, p.y + 16.0F)));
}

void EditorManipulator::ExpandSubgraph(void)
{
	unsigned_int32 state = graphCollapseButton.GetWidgetState();
	if (state & kWidgetCollapsed)
	{
		InvalidateGraph();
		graphCollapseButton.SetWidgetState(state & ~kWidgetCollapsed);
	}
}

void EditorManipulator::CollapseSubgraph(void)
{
	unsigned_int32 state = graphCollapseButton.GetWidgetState();
	if (!(state & kWidgetCollapsed))
	{
		InvalidateGraph();
		graphCollapseButton.SetWidgetState(state | kWidgetCollapsed);
	}
}

Node *EditorManipulator::PickGraphNode(const ManipulatorViewportData *viewportData, const Ray *ray, Widget **widget)
{
	const Point3D& position = GetGraphPosition();
	float x = ray->origin.x - position.x;
	float y = ray->origin.y - position.y;
	
	if ((y > -1.0F) && (y < graphHeight) && (x > -1.0F) && (x < graphWidth + 29.0F))
	{
		Node *node = GetTargetNode();
		if ((x > -1.0F) && (x < kGraphBoxWidth + 1.0F) && (y < 17.0F)) return (node);
		
		if (widget)
		{
			if ((graphCollapseButton.Visible()) && (graphCollapseButton.GetBoundingBox()->Contains(ray->origin.GetPoint2D())))
			{
				*widget = &graphCollapseButton;
				return (nullptr);
			}
		}
		
		const Node *subnode = node->GetFirstSubnode();
		while (subnode)
		{
			Node *pick = static_cast<EditorManipulator *>(subnode->GetManipulator())->PickGraphNode(viewportData, ray, widget);
			if (pick) return (pick);
			
			subnode = subnode->Next();
		}
	}
	
	return (nullptr);
}

void EditorManipulator::SelectGraphNodes(float left, float right, float top, float bottom, unsigned_int32 state)
{
	const Point3D& position = GetGraphPosition();
	if ((position.y < bottom) && (position.y + graphHeight > top) && (position.x < right) && (position.x + graphWidth > left))
	{
		Node *node = GetTargetNode();
		if ((position.x + kGraphBoxWidth > left) && (position.y + 16.0F > top))
		{
			worldEditor->SelectNode(node);
			SetManipulatorState(GetManipulatorState() | state);
		}
		
		const Node *subnode = node->GetFirstSubnode();
		while (subnode)
		{
			static_cast<EditorManipulator *>(subnode->GetManipulator())->SelectGraphNodes(left, right, top, bottom, state);
			subnode = subnode->Next();
		}
	}
}

void EditorManipulator::HiliteSubtree(void)
{
	Node *root = GetTargetNode();
	Node *node = root->GetFirstSubnode();
	while (node)
	{
		EditorManipulator *manipulator = Editor::GetManipulator(node);
		manipulator->SetManipulatorState(manipulator->GetManipulatorState() | kManipulatorHilited);
		node = root->GetNextNode(node);
	}
}

void EditorManipulator::UnhiliteSubtree(void)
{
	Node *root = GetTargetNode();
	Node *node = root->GetFirstSubnode();
	while (node)
	{
		EditorManipulator *manipulator = Editor::GetManipulator(node);
		manipulator->SetManipulatorState(manipulator->GetManipulatorState() & ~kManipulatorHilited);
		node = root->GetNextNode(node);
	}
}

void EditorManipulator::Render(const ManipulatorRenderData *renderData)
{
	float scale = renderData->viewportScale;
	manipulatorScaleVector.Set(scale, scale, scale, scale);
	
	unsigned_int32 state = GetManipulatorState();
	bool showConnectors = ((renderData->connectorList) && (state & (kManipulatorSelected | kManipulatorConnectorSelected)) && (connectorCount != 0));
	
	if ((state & kManipulatorShowIcon) || (showConnectors))
	{
		List<Renderable> *renderList = renderData->manipulatorList;
		if (renderList)
		{
			renderList->Append(&markerRenderable);
			renderList->Append(&iconRenderable);
		}
	}
	
	if (state & kManipulatorSelected)
	{
		List<Renderable> *renderList = renderData->handleList;
		if ((renderList) && (handleCount != 0)) renderList->Append(&handleRenderable);
	}
	
	if (showConnectors)
	{
		List<Renderable> *renderList = renderData->connectorList;
		for (machine a = 0; a < connectorCount; a++) editorConnector[a].RenderLine(renderData, renderList);
		for (machine a = 0; a < connectorCount; a++) editorConnector[a].RenderBox(renderData, renderList);
	}
	
	if (state & kManipulatorShowGizmo) editorGizmo->Render(renderData);
}

void EditorManipulator::RenderGraph(const ManipulatorViewportData *viewportData, List<Renderable> *renderList)
{
	const Node *node = GetTargetNode();
	const Node *previous = node->Previous();
	
	const Point3D& cameraPosition = viewportData->viewportCamera->GetNodePosition();
	const Vector3D& position = GetGraphPosition() - cameraPosition;
	
	float left = position.x - 29.0F;
	float right = position.x + graphWidth;
	float top = (previous) ? Editor::GetManipulator(previous)->GetGraphPosition().y - cameraPosition.y : position.y - 1.0F;
	float bottom = position.y + graphHeight;
	
	const OrthoCameraObject *cameraObject = static_cast<OrthoCamera *>(viewportData->viewportCamera)->GetObject();
	if ((top < cameraObject->GetOrthoRectBottom()) && (bottom > cameraObject->GetOrthoRectTop()) && (left < cameraObject->GetOrthoRectRight()) && (right > cameraObject->GetOrthoRectLeft()))
	{
		graphBorder.SetViewportScale(viewportData->viewportScale);
		graphBackground.RenderTree(renderList);
		
		if (!(graphCollapseButton.GetWidgetState() & kWidgetCollapsed))
		{
			const Node *subnode = node->GetFirstSubnode();
			while (subnode)
			{
				static_cast<EditorManipulator *>(subnode->GetManipulator())->RenderGraph(viewportData, renderList);
				subnode = subnode->Next();
			}
		}
	}
}

void EditorManipulator::Install(Editor *editor, Node *root, bool recursive)
{
	EditorManipulator *manipulator = Editor::GetManipulator(root);
	if (!manipulator)
	{
		manipulator = static_cast<EditorManipulator *>(Manipulator::Construct(root));
		if (!manipulator) manipulator = new EditorManipulator(root, "WorldEditor/node/Node");
		
		root->SetManipulator(manipulator);
		manipulator->Invalidate();
	}
	else
	{
		unsigned_int32 state = manipulator->GetManipulatorState() & ~kManipulatorSelected;
		manipulator->SetManipulatorState(state);
	}
	
	manipulator->worldEditor = editor;
	
	if (recursive)
	{
		Node *node = root->GetFirstSubnode();
		while (node)
		{
			Install(editor, node);
			node = node->Next();
		}
	}
}


GroupManipulator::GroupManipulator(Node *node) :
		EditorManipulator(node, "WorldEditor/node/Group"),
		groupDiffuseColor(kGroupOutlineColor, kAttributeMutable),
		groupTextureMap(&VolumeManipulator::outlineTextureHeader, VolumeManipulator::outlineTextureImage),
		groupRenderable(kRenderQuads, kRenderDepthTest | kRenderDepthInhibit)
{
	groupSizeVector.GetVector3D().Set(1.0F, 1.0F, 1.0F);
	
	groupAttributeList.Append(&groupDiffuseColor);
	groupAttributeList.Append(&groupTextureMap);
	groupRenderable.SetMaterialAttributeList(&groupAttributeList);
	groupRenderable.SetAmbientBlendState(kBlendInterpolate);
	groupRenderable.SetShaderFlags(kShaderAmbientEffect | kShaderVertexPolyboard | kShaderLinearPolyboard | kShaderOrthoPolyboard | kShaderScaleVertex);
	groupRenderable.SetRenderParameterPointer(&groupSizeVector);
	groupRenderable.SetTransformable(node);
	groupRenderable.SetVertexCount(48);
	groupRenderable.SetAttributeArray(kArrayVertex, groupVertex);
	groupRenderable.SetAttributeArray(kArrayTangent, &BoxVolumeManipulator::outlineTangent[0]);
	groupRenderable.SetAttributeArray(kArrayTexture0, &VolumeManipulator::outlineTexcoord[0]);
}

GroupManipulator::~GroupManipulator()
{
}

const char *GroupManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', 'GRUP')));
}

void GroupManipulator::Select(void)
{
	EditorManipulator::Select();
	groupDiffuseColor.SetDiffuseColor(ColorRGBA(1.0F, 1.0F, 1.0F, 1.0F));
}

void GroupManipulator::Unselect(void)
{
	EditorManipulator::Unselect();
	groupDiffuseColor.SetDiffuseColor(kGroupOutlineColor);
}

bool GroupManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	const Node *node = GetTargetNode();
	const BoundingSphere *boundingSphere = node->GetBoundingSphere();
	if (boundingSphere)
	{
		sphere->SetCenter(node->GetInverseWorldTransform() * boundingSphere->GetCenter());
		sphere->SetRadius(boundingSphere->GetRadius() * K::sqrt_3);
	}
	else
	{
		sphere->SetCenter(Zero3D);
		sphere->SetRadius(K::sqrt_3_over_2);
	}
	
	return (true);
}

bool GroupManipulator::Pick(const Ray *ray, PickData *data) const
{
	float r = (ray->radius != 0.0F) ? ray->radius : Editor::kFrustumRenderScale;
	float r2 = r * r * 4.0F;
	
	const Point3D *vertex = groupVertex;
	for (machine a = 0; a < 48; a += 4)
	{
		if (PickLineSegment(ray, vertex[0], vertex[2], r2, &data->rayParam)) return (true);
		vertex += 4;
	}
	
	return (false);
}

bool GroupManipulator::RegionPick(const Region *region) const
{
	const Point3D *vertex = groupVertex;
	for (machine a = 0; a < 48; a += 4)
	{
		if (RegionPickLineSegment(region, vertex[0], vertex[2])) return (true);
		vertex += 4;
	}
	
	return (false);
}

void GroupManipulator::Update(void)
{
	EditorManipulator::Update();
	
	Point3D center(0.0F, 0.0F, 0.0F);
	float radius = 0.5F;
	
	const Node *node = GetTargetNode();
	const BoundingSphere *boundingSphere = node->GetBoundingSphere();
	if (boundingSphere)
	{
		center = node->GetInverseWorldTransform() * boundingSphere->GetCenter();
		radius = boundingSphere->GetRadius();
	}
	else
	{
		boundingSphere = Editor::GetManipulator(node)->GetTreeSphere();
		if (boundingSphere)
		{
			center = node->GetInverseWorldTransform() * boundingSphere->GetCenter();
			radius = boundingSphere->GetRadius();
		}
	}
	
	Vector3D dp(radius, radius, radius);
	radius *= 2.0F;
	
	const Point3D *vertex = &BoxVolumeManipulator::outlineVertex[0];
	for (machine a = 0; a < 48; a++) groupVertex[a] = center + vertex[a] * radius - dp;
}

void GroupManipulator::Render(const ManipulatorRenderData *renderData)
{
	List<Renderable> *renderList = renderData->manipulatorList;
	if (renderList)
	{
		groupSizeVector.w = renderData->viewportScale;
		renderList->Append(&groupRenderable);
	}
	
	EditorManipulator::Render(renderData);
}


SkyboxManipulator::SkyboxManipulator(Skybox *skybox) : EditorManipulator(skybox, "WorldEditor/skybox/Skybox")
{
}

SkyboxManipulator::~SkyboxManipulator()
{
}

const char *SkyboxManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', kNodeSkybox, kNodeSkybox)));
}

void SkyboxManipulator::Preprocess(void)
{
	SetManipulatorState(GetManipulatorState() | kManipulatorShowIcon);
	EditorManipulator::Preprocess();
	
	GetEditor()->SetProcessPropertiesFlag();
}

void SkyboxManipulator::HandleDelete(bool undoable)
{
	EditorManipulator::HandleDelete(undoable);
	GetEditor()->SetProcessPropertiesFlag();
}

void SkyboxManipulator::HandleUndelete(void)
{
	EditorManipulator::HandleUndelete();
	GetEditor()->SetProcessPropertiesFlag();
}

void SkyboxManipulator::HandleSettingsUpdate(void)
{
	EditorManipulator::HandleSettingsUpdate();
	GetTargetNode()->InvalidateShaderData();
}

bool SkyboxManipulator::MaterialSettable(void) const
{
	return (true);
}

bool SkyboxManipulator::MaterialRemovable(void) const
{
	return (true);
}

const MaterialObject *SkyboxManipulator::PickupMaterial(void) const
{
	return (GetTargetNode()->GetMaterialObject());
}

void SkyboxManipulator::SetMaterial(MaterialObject *materialObject)
{
	Skybox *skybox = GetTargetNode();
	skybox->SetMaterialObject(materialObject);
	skybox->InvalidateShaderData();
}

void SkyboxManipulator::RemoveMaterial(void)
{
	Skybox *skybox = GetTargetNode();
	skybox->SetMaterialObject(nullptr);
	skybox->InvalidateShaderData();
}


ImpostorManipulator::ImpostorManipulator(Impostor *impostor) : EditorManipulator(impostor, "WorldEditor/impostor/Impostor")
{
}

ImpostorManipulator::~ImpostorManipulator()
{
}

const char *ImpostorManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', kNodeImpostor, kNodeImpostor)));
}

void ImpostorManipulator::Preprocess(void)
{
	EditorManipulator::Preprocess();
	SetManipulatorState(GetManipulatorState() | kManipulatorShowIcon);
}

bool ImpostorManipulator::MaterialSettable(void) const
{
	return (true);
}

const MaterialObject *ImpostorManipulator::PickupMaterial(void) const
{
	return (GetTargetNode()->GetMaterialObject());
}

void ImpostorManipulator::SetMaterial(MaterialObject *materialObject)
{
	Impostor *impostor = GetTargetNode();
	impostor->SetMaterialObject(materialObject);
}

// ZYURVUR
