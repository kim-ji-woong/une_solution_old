#include "stdafx.h"
#include "Line2D.h"

using namespace VectorGraphics;

namespace FireSafetyManager
{
	Line2D::Line2D()
	{
	}

	Line2D::Line2D(const Vertex2D& vBegin, const Vertex2D& vEnd)
	{
		m_vBegin = vBegin;
		m_vEnd = vEnd;
	}

	const Vertex2D& Line2D::GetVertex(bool isBegin)
	{
		return isBegin ? m_vBegin : m_vEnd;
	}

	void Line2D::SetVertex(const VectorGraphics::Vertex2D& rVertex, bool isBegin)
	{
		if (isBegin)
			m_vBegin = rVertex;
		else
			m_vEnd = rVertex;
	}
}
