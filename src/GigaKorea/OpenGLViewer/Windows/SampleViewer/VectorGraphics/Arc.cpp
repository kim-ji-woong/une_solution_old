#include "stdafx.h"
#include "Arc.h"

namespace VectorGraphics
{
	Arc::Arc()
	{
		m_dRadius = 0.0;
		m_dBeginAngle = 0.0;
		m_dArcAngle = 0.0;
		m_isClockwise = true;
	}

	Arc::Arc(const Vertex2D& vCenter, double dRadius, double dBeginAngle, double dArcAngle, bool isClockwise)
	{
		SetArc(vCenter, dRadius, dBeginAngle, dArcAngle, isClockwise);
	}

	Arc::~Arc()
	{
	}

	void Arc::SetArc(const Vertex2D& vCenter, double dRadius, double dBeginAngle, double dArcAngle, bool isClockwise)
	{
		m_vCenter = vCenter;
		m_dRadius = dRadius;
		m_dBeginAngle = dBeginAngle;
		m_dArcAngle = dArcAngle;
		m_isClockwise = isClockwise;

		Vertex2D vBegin;
		Vertex2D vEnd;

		vBegin.x = m_vCenter.x + m_dRadius * cos(m_dBeginAngle);
		vBegin.y = m_vCenter.y + m_dRadius * sin(m_dBeginAngle);

		if (m_isClockwise)
		{
			vEnd.x = m_vCenter.x + m_dRadius * cos(m_dBeginAngle - m_dArcAngle);
			vEnd.y = m_vCenter.y + m_dRadius * sin(m_dBeginAngle - m_dArcAngle);
		}
		else
		{
			vEnd.x = m_vCenter.x + m_dRadius * cos(m_dBeginAngle + m_dArcAngle);
			vEnd.y = m_vCenter.y + m_dRadius * sin(m_dBeginAngle + m_dArcAngle);
		}

		int nSlice = 100;
		double theta = m_dArcAngle / nSlice;

		m_vertices.clear();
		m_vertices.push_back(vBegin);
		
		for (int i = 1; i < nSlice; i++)
		{
			double angle = isClockwise ? m_dBeginAngle - theta * i : m_dBeginAngle + theta * i;
			double x = m_vCenter.x + m_dRadius * cos(angle);
			double y = m_vCenter.y + m_dRadius * sin(angle);

			m_vertices.push_back(Vertex2D(x, y));
		}

		m_vertices.push_back(vEnd);
	}

	const std::list<Vertex2D>& Arc::GetVertices()
	{
		return m_vertices;
	}

	void Arc::Draw()
	{
		glBegin(GL_LINES);

		for (std::list<Vertex2D>::iterator iter = m_vertices.begin(); iter != m_vertices.end(); iter++)
		{
			glVertex2f((float)iter->x, (float)iter->y);
		}

		glEnd();
	}
}
