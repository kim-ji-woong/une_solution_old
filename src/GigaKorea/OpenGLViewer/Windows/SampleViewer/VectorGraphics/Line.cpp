#include "stdafx.h"
#include "Line.h"

namespace VectorGraphics
{
	Line::Line()
	{
	}

	Line::Line(const Vertex2D& vBegin, const Vertex2D& vEnd)
	{
		m_vBegin.x = vBegin.x;
		m_vBegin.y = vBegin.y;
		m_vEnd.x = vEnd.x;
		m_vEnd.y = vEnd.y;
	}

	Line::~Line()
	{
	}

	void Line::SetVertex(const Vertex2D& vertex, bool isBegin)
	{
		if (isBegin)
		{
			m_vBegin.x = vertex.x;
			m_vBegin.y = vertex.y;
		}
		else
		{
			m_vEnd.x = vertex.x;
			m_vEnd.y = vertex.y;
		}
	}

	const Vertex2D& Line::GetVertex(bool isBegin)
	{
		if (isBegin)
			return m_vBegin;

		return m_vEnd;
	}

	void Line::Draw()
	{
		glBegin(GL_LINES);
		glVertex2f((float)m_vBegin.x, (float)m_vBegin.y);
		glVertex2f((float)m_vEnd.x, (float)m_vEnd.y);
		glEnd();
	}
}
