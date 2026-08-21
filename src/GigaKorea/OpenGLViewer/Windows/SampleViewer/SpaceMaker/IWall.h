#pragma once
#include "Vertex2D.h"

namespace SpaceMaker
{
	class ISpace;

	class IWall
	{
	public:
		enum GridType { Unknown = -1, Line = 0, Arc, EArc };

	public:
		IWall();

	public:
		virtual const VectorGraphics::Vertex2D& GetBegin() = 0;
		virtual const VectorGraphics::Vertex2D& GetEnd() = 0;
		virtual GridType GetGridType() = 0;
		virtual double GetThick() = 0;
		virtual int GetLinkedSpaceCount() = 0;
		virtual ISpace* GetLinkedSpace(int nIndex) = 0;
		virtual void AddBoundaryVertex(const VectorGraphics::Vertex2D& vertex) = 0;
	};
}
