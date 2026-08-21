#pragma once
#include "Vertex2D.h"

namespace FireSafetyManager
{
	class Line2D
	{
	public:
		Line2D();
		Line2D(const VectorGraphics::Vertex2D& vBegin, const VectorGraphics::Vertex2D& vEnd);

	public:
		const VectorGraphics::Vertex2D& GetVertex(bool isBegin);
		void SetVertex(const VectorGraphics::Vertex2D& rVertex, bool isBegin);
		
	private:
		VectorGraphics::Vertex2D m_vBegin;
		VectorGraphics::Vertex2D m_vEnd;
	};
}
