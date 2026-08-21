#include "stdafx.h"
#include "Shape.h"
#include "Vertex2D.h"

namespace VectorGraphics
{
	Shape::Shape()
	{
		m_pLayer = 0;
	}


	Shape::~Shape()
	{
	}

	void Shape::DrawVertex(const Vertex2D& rVertex)
	{
		glVertex2f((float)rVertex.x, (float)rVertex.y);
	}

	bool Shape::HitTest(const Vertex2D& vPos)
	{
		return false;
	}

	bool Shape::HitTestIfPOI(const Vertex2D& vPos)
	{
		return false;
	}

	bool Shape::HitTestIfNotPOI(const Vertex2D& vPos)
	{
		return false;
	}

	void Shape::SetLayer(Layer* pLayer)
	{
		m_pLayer = pLayer;
	}

	Layer* Shape::GetLayer()
	{
		return m_pLayer;
	}
}
