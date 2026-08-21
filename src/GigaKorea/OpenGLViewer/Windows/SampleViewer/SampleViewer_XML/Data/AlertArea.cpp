#include "stdafx.h"
#include "AlertArea.h"

using namespace VectorGraphics;

namespace FireSafetyManager
{
	AlertArea::AlertArea()
	{
		m_nID = 0;
		m_strName = L"";
	}

	void AlertArea::SetID(int nID)
	{
		m_nID = nID;
	}

	int AlertArea::GetID()
	{
		return m_nID;
	}

	void AlertArea::SetName(const std::wstring& strName)
	{
		m_strName = strName;
	}

	void AlertArea::AddBoundaryVertex(const Vertex2D& vertex)
	{
		m_boundaries.push_back(vertex);
	}

	int AlertArea::GetBoundaryVertexCount()
	{
		return (int)m_boundaries.size();
	}

	Vertex2D* AlertArea::GetBoundaryVertex(int nIndex)
	{
		if (nIndex < 0 || nIndex >= GetBoundaryVertexCount())
			return 0;

		std::list<Vertex2D>::iterator iter = m_boundaries.begin();

		for (int i = 0; i < nIndex; i++)
		{
			iter++;
		}

		Vertex2D& rVertex = *iter;
		return &rVertex;
	}
}
