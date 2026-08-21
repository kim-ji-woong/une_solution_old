#include "stdafx.h"
#include "Polyline.h"
#include <algorithm>

namespace VectorGraphics
{
	Polyline::Polyline()
	{
		m_isClosed = false;
	}

	Polyline::~Polyline()
	{

	}

	void Polyline::Draw()
	{
		glBegin(GL_LINE_STRIP);

		std::for_each(m_vertices.begin(), m_vertices.end(), DrawVertex);

		if (m_isClosed && m_vertices.size() > 0)
		{
			DrawVertex(*m_vertices.begin());
		}

		glEnd();
	}

	void Polyline::AddVertex(const Vertex2D& rVertex)
	{
		m_vertices.push_back(rVertex);
	}

	int Polyline::GetVertexCount()
	{
		return (int)m_vertices.size();
	}

	bool Polyline::GetVertex(int nIndex, Vertex2D* pVertex)
	{
		if (nIndex < 0 || nIndex >= GetVertexCount())
			return false;

		std::list<Vertex2D>::iterator iter = m_vertices.begin();

		for (int i = 0; i < nIndex; i++)
		{
			iter++;
		}

		*pVertex = *iter;
		return true;
	}

	void Polyline::RemoveAt(int nIndex)
	{
		std::list<Vertex2D>::iterator iter = m_vertices.begin();

		for (int i = 0; i < nIndex; i++)
		{
			if (iter == m_vertices.end())
				return;

			iter++;
		}

		if (iter != m_vertices.end())
			m_vertices.erase(iter);
	}

	void Polyline::Clear()
	{
		m_vertices.clear();
	}

	bool Polyline::IsClosed()
	{
		return m_isClosed;
	}

	void Polyline::SetClosed(bool closed)
	{
		m_isClosed = closed;
	}
}
