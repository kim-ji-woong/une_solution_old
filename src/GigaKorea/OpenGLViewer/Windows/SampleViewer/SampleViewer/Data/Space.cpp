#include "stdafx.h"
#include "Space.h"
#include "Wall.h"
#include "IWall.h"

using namespace VectorGraphics;
using namespace SpaceMaker;

namespace FireSafetyManager
{
	Space::Space()
	{
		m_nID = -1;
		m_strName = L"";
		m_pFirstWall = 0;
	}

	Space::Space(int nID, const std::wstring& strName)
	{
		m_nID = nID;
		m_strName = strName;
		m_pFirstWall = 0;
	}

	int Space::GetID()
	{
		return m_nID;
	}

	void Space::SetID(int nID)
	{
		m_nID = nID;
	}

	void Space::SetName(const std::wstring& strName)
	{
		m_strName = strName;
	}

	void Space::AddWall(Wall* pWall)
	{
		if (m_walls.size() == 0)
			m_pFirstWall = pWall;

		m_walls.push_back(pWall);
		pWall->AddLinkedSpace(this);
	}

	int Space::GetWallCount()
	{
		return (int)m_walls.size();
	}

	IWall* Space::GetWall(int nIndex)
	{
		if (nIndex < 0 || nIndex >= GetWallCount())
			return 0;

		std::list<Wall*>::iterator iter = m_walls.begin();

		for (int i = 0; i < nIndex; i++)
		{
			iter++;
		}

		return *iter;
	}

	void Space::AddBoundaryVertex(const Vertex2D& vertex)
	{
		m_boundaries.push_back(vertex);
	}

	int Space::GetBoundaryVertexCount()
	{
		return (int)m_boundaries.size();
	}

	Vertex2D* Space::GetBoundaryVertex(int nIndex)
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
