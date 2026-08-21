#include "Stdafx.h"
#include "Line.h"
#include "Block.h"
#include "Arc.h"
#include "EArc.h"
#include "Hatch.h"
#include "Layer.h"
#include "LineType.h"
#include "DXFControl.h"
#include "ShapeGroup.h"
#include "Text.h"
#include "PolyLine.h"
#include "EntityFactory.h"


namespace DXFDotNet
{
	EntityFactory::EntityFactory(void)
	{
	}

	EntityFactory::~EntityFactory(void)
	{
	}

	// Create Line
	Line^ EntityFactory::CreateLine()
	{
		return nullptr;
	}

	Line^ EntityFactory::CreateLine(Line^ rhs)
	{
		return nullptr;
	}

	Line^ EntityFactory::CreateLine(UnE::Geometry::Vertex2D^ vBegin, UnE::Geometry::Vertex2D^ vEnd)
	{
		return nullptr;
	}

	// Create Block
	Block^ EntityFactory::CreateBlock(DXFControl^ ctrl)
	{
		return nullptr;
	}

	// Create Arc
	Arc^ EntityFactory::CreateArc(void)
	{
		return nullptr;
	}

	// Create EArc
	EArc^ EntityFactory::CreateEArc(void)
	{
		return nullptr;
	}

	// Create Hatch
	Hatch^ EntityFactory::CreateHatch(void)
	{
		return nullptr;
	}

	// Create Layer
	Layer^ EntityFactory::CreateLayer(IShapeOwner^ owner)
	{
		return nullptr;
	}

	Layer^ EntityFactory::CreateLayer(IShapeOwner^ owner, LineType^ lineType)
	{
		return nullptr;
	}

	// Create ShapeGroup
	ShapeGroup^ EntityFactory::CreateShapeGroup()
	{
		return nullptr;
	}

	ShapeGroup^ EntityFactory::CreateShapeGroup(ShapeGroupOption^ option)
	{
		return nullptr;
	}

	// Create Text
	Text^ EntityFactory::CreateText()
	{
		return nullptr;
	}

	PolyLine^ EntityFactory::CreatePolyLine()
	{
		return nullptr;
	}
}
