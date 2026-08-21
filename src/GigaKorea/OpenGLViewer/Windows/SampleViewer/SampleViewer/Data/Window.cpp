#include "stdafx.h"
#include "Window.h"
#include "POI.h"
#include "Wall.h"

using namespace VectorGraphics;

namespace FireSafetyManager
{
	Window::Window()
	{
		m_nID = -1;
		m_dThick = 10.0;
	}

	int Window::GetID()
	{
		return m_nID;
	}

	const VectorGraphics::Vertex2D& Window::GetPosition()
	{
		return m_vPos;
	}

	double Window::GetWidth()
	{
		return m_dWidth;
	}

	double Window::GetHeight()
	{
		return m_dHeight;
	}

	double Window::GetThick()
	{
		return m_dThick;
	}

	double Window::GetElevation()
	{
		return m_dElevation;
	}

	Wall* Window::GetWall()
	{
		return m_pWall;
	}

	void Window::SetID(int nID)
	{
		m_nID = nID;
	}

	void Window::SetPosition(const VectorGraphics::Vertex2D& vPos)
	{
		m_vPos = vPos;
	}

	void Window::SetWidth(double dWidth)
	{
		m_dWidth = dWidth;
	}

	void Window::SetHeight(double dHeight)
	{
		m_dHeight = dHeight;
	}

	void Window::SetThick(double dThick)
	{
		m_dThick = dThick;
	}

	void Window::SetElevation(double dElevation)
	{
		m_dElevation = dElevation;
	}

	void Window::SetWall(Wall* pWall)
	{
		m_pWall = pWall;
	}

	int Window::CalcBoundary(std::vector<VectorGraphics::VertexList*>& edges)
	{
		const Vertex2D& vBegin = m_pWall->GetBegin();
		const Vertex2D& vEnd = m_pWall->GetEnd();

		double len1 = m_vPos.GetDistance(vBegin);
		double len2 = m_vPos.GetDistance(vEnd);
		Vertex2D vB;

		if (len1 > len2)
			vB = m_vPos.GetLinearVertex(vBegin, m_dWidth / 2);
		else
			vB = m_vPos.GetLinearVertex(vEnd, m_dWidth / 2);

		Vertex2D vE = m_vPos * 2 - vB;

		Vertex2D v1 = vB.GetRightVertex(vE, m_dThick / 2);
		Vertex2D v2 = vB * 2 - v1;
		Vertex2D v3 = m_vPos * 2 - v1;
		Vertex2D v4 = m_vPos * 2 - v2;

		VertexList* path = new VertexList();

		path->Vertices.push_back(v1);
		path->Vertices.push_back(v2);
		path->Vertices.push_back(v3);
		path->Vertices.push_back(v4);

		edges.push_back(path);
		return (int)edges.size();
	}
}
