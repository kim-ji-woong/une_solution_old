#include "stdafx.h"
#include "Wall.h"
#include "Line2D.h"
#include "Vertex2D.h"
#include "Space.h"
#include "Door.h"
#include "Window.h"

using namespace VectorGraphics;
using namespace SpaceMaker;

namespace FireSafetyManager
{
	Wall::Wall()
	{
		m_nID = -1;
		m_dThick = 0.0;
		m_dHeight = 0.0;
		m_pComponent = 0;
		m_pLine = 0;
		m_gridType = GridType::Unknown;
		m_pArc = 0;
		m_pEArc = 0;
	}

	Wall::Wall(int nID, double dThick, double dHeight, Component* pComponent, Line2D* pLine)
	{
		m_nID = nID;
		m_dThick = dThick;
		m_dHeight = dHeight;
		m_pComponent = pComponent;
		m_pLine = pLine;
		m_pArc = 0;
		m_pEArc = 0;
		m_gridType = GridType::Line;
	}

	Wall::Wall(int nID, double dThick, double dHeight, Component* pComponent, VectorGraphics::Arc* pArc)
	{
		m_nID = nID;
		m_dThick = dThick;
		m_dHeight = dHeight;
		m_pComponent = pComponent;
		m_pLine = 0;
		m_pArc = pArc;
		m_pEArc = 0;
		m_gridType = GridType::Arc;
	}

	Wall::Wall(int nID, double dThick, double dHeight, Component* pComponent, VectorGraphics::EArc* pEArc)
	{
		m_nID = nID;
		m_dThick = dThick;
		m_dHeight = dHeight;
		m_pComponent = pComponent;
		m_pLine = 0;
		m_pArc = 0;
		m_pEArc = pEArc;
		m_gridType = GridType::Arc;
	}

	Wall::~Wall()
	{
		if (m_pLine)
			delete m_pLine;
	}

	Wall::GridType Wall::ToGridType(int nGridType)
	{
		if (nGridType == (int)GridType::Line)
			return GridType::Line;
		else if (nGridType == (int)GridType::Arc)
			return GridType::Arc;
		else if (nGridType == (int)GridType::EArc)
			return GridType::EArc;

		return GridType::Unknown;
	}

	int Wall::GetID()
	{
		return m_nID;
	}

	const Vertex2D& Wall::GetBegin()
	{
		if (m_gridType == GridType::Line)
		{
			if (m_pLine != 0)
				return m_pLine->GetVertex(true);
		}

		return Vertex2D();
	}

	const Vertex2D& Wall::GetEnd()
	{
		if (m_gridType == GridType::Line)
		{
			if (m_pLine != 0)
				return m_pLine->GetVertex(false);
		}

		return Vertex2D();
	}

	IWall::GridType Wall::GetGridType()
	{
		return m_gridType;
	}

	void Wall::AddDoor(Door* pDoor)
	{
		m_doors.push_back(pDoor);
		pDoor->SetWall(this);
	}

	void Wall::AddWindow(Window* pWindow)
	{
		m_windows.push_back(pWindow);
		pWindow->SetWall(this);
	}

	double Wall::GetThick()
	{
		return m_dThick;
	}

	void Wall::AddLinkedSpace(Space* pSpace)
	{
		if (std::find(m_linkedSpaces.begin(), m_linkedSpaces.end(), pSpace) == m_linkedSpaces.end())
			m_linkedSpaces.push_back(pSpace);
	}

	int Wall::GetLinkedSpaceCount()
	{
		return (int)m_linkedSpaces.size();
	}

	ISpace* Wall::GetLinkedSpace(int nIndex)
	{
		if (nIndex < 0 || nIndex >= GetLinkedSpaceCount())
			return 0;

		std::list<Space*>::iterator iter = m_linkedSpaces.begin();

		for (int i = 0; i < nIndex; i++)
		{
			iter++;
		}

		return *iter;
	}

	void Wall::AddBoundaryVertex(const Vertex2D& vertex)
	{
		m_boundaries.push_back(vertex);
	}

	int Wall::GetBoundaryVertexCount()
	{
		return (int)m_boundaries.size();
	}

	Vertex2D* Wall::GetBoundaryVertex(int nIndex)
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

	int Wall::GetDoorCount()
	{
		return (int)m_doors.size();
	}

	Door* Wall::GetDoor(int nIndex)
	{
		if (nIndex < 0 || nIndex >= GetDoorCount())
			return 0;

		std::list<Door*>::iterator iter = m_doors.begin();

		for (int i = 0; i < nIndex; i++)
		{
			iter++;
		}

		return *iter;
	}

	int Wall::GetWindowCount()
	{
		return (int)m_windows.size();
	}

	Window* Wall::GetWindow(int nIndex)
	{
		if (nIndex < 0 || nIndex >= GetWindowCount())
			return 0;

		std::list<Window*>::iterator iter = m_windows.begin();

		for (int i = 0; i < nIndex; i++)
		{
			iter++;
		}

		return *iter;
	}
}
