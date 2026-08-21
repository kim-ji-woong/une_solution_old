#pragma once
#include "IShapeOwner.h"

namespace DXFDotNet
{
	ref class UnE::Geometry::Vertex2D;
	ref class Line;
	ref class Block;
	ref class Arc;
	ref class EArc;
	ref class Hatch;
	ref class Layer;
	ref class LineType;
	ref class DXFControl;
	ref class ShapeGroup;
	ref class ShapeGroupOption;
	ref class Text;
	ref class PolyLine;

	public ref class EntityFactory
	{
	public:
		EntityFactory(void);
		virtual ~EntityFactory(void);	

		// Create Line
		virtual Line^ CreateLine();
		virtual Line^ CreateLine(Line^ rhs);
		virtual Line^ CreateLine(UnE::Geometry::Vertex2D^ vBegin, UnE::Geometry::Vertex2D^ vEnd);
		
		// Create Block
		virtual Block^ CreateBlock(DXFControl^ ctrl);
	
		// Create Arc
		virtual Arc^ CreateArc(void);

		// Create EArc
		virtual EArc^ CreateEArc(void);

		// Create Hatch
		virtual Hatch^ CreateHatch(void);

		// Create Layer
		virtual Layer^ CreateLayer(IShapeOwner^ owner);
		virtual Layer^ CreateLayer(IShapeOwner^ owner, LineType^ lineType);
		
		// Create ShapeGroup
		virtual ShapeGroup^ CreateShapeGroup();
		virtual ShapeGroup^ CreateShapeGroup(ShapeGroupOption^ option);

		// Create Text
		virtual Text^ CreateText();

		virtual PolyLine^ CreatePolyLine();
	};
}
