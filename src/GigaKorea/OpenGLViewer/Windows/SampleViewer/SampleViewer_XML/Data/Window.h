#pragma once
#include "Vertex2D.h"
#include <vector>

namespace VectorGraphics
{
	class VertexList;
}

namespace FireSafetyManager
{
	class Wall;

	class Window
	{
	public:
		Window();

	public:
		int GetID();
		const VectorGraphics::Vertex2D& GetPosition();
		double GetWidth();
		double GetHeight();
		double GetThick();
		double GetElevation();
		Wall* GetWall();

		void SetID(int nID);
		void SetPosition(const VectorGraphics::Vertex2D& vPos);
		void SetWidth(double dWidth);
		void SetHeight(double dHeight);
		void SetThick(double dThick);
		void SetElevation(double dElevation);
		void SetWall(Wall* pWall);

		int CalcBoundary(std::vector<VectorGraphics::VertexList*>& edges);
		
	private:
		int m_nID;
		VectorGraphics::Vertex2D m_vPos;
		double m_dWidth;
		double m_dHeight;
		double m_dElevation;
		double m_dThick;
		Wall* m_pWall;
	};
}
