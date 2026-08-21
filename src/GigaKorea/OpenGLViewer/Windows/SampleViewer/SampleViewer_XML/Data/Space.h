#pragma once
#include <string>
#include <list>
#include "Vertex2D.h"
#include "ISpace.h"

namespace SpaceMaker
{
	class IWall;
}

namespace FireSafetyManager
{
	class Wall;

	class Space : public SpaceMaker::ISpace
	{
	public:
		Space();
		Space(int nID, const std::wstring& strName);

	public:
		int GetID();

		void SetID(int nID);
		void SetName(const std::wstring& strName);
		std::wstring GetName();

		void AddWall(Wall* pWall);
		int GetWallCount();
		SpaceMaker::IWall* GetWall(int nIndex);

		void AddBoundaryVertex(const VectorGraphics::Vertex2D& vertex);
		int GetBoundaryVertexCount();
		VectorGraphics::Vertex2D* GetBoundaryVertex(int nIndex);

	/*private:
		bool CheckComplete(Wall* pFirst, Wall* pLast);
		void CalcBoundaries();*/
		
	private:
		int m_nID;
		std::wstring m_strName;
		std::list<Wall*> m_walls;
		std::list<VectorGraphics::Vertex2D> m_boundaries;
		Wall* m_pFirstWall;
	};
}
