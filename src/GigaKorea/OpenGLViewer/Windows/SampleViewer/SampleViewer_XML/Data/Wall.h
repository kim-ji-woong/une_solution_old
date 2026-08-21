#pragma once
#include <string>
#include <list>
#include "IWall.h"

namespace VectorGraphics
{
	class Vertex2D;
	class Arc;
	class EArc;
}

namespace SpaceMaker
{
	class ISpace;
}

namespace FireSafetyManager
{
	class Component;
	class Line2D;
	class Door;
	class Window;
	class Space;
	
	class Wall : public SpaceMaker::IWall
	{
	public:
		Wall();
		Wall(int nID, double dThick, double dHeight, Component* pComponent, Line2D* pLine);
		Wall(int nID, double dThick, double dHeight, Component* pComponent, VectorGraphics::Arc* pArc);
		Wall(int nID, double dThick, double dHeight, Component* pComponent, VectorGraphics::EArc* pEArc);
		virtual ~Wall();

	public:
		static GridType ToGridType(int nGridType);

	public:
		int GetID();
		const VectorGraphics::Vertex2D& GetBegin();
		const VectorGraphics::Vertex2D& GetEnd();
		GridType GetGridType();

		void AddDoor(Door* pDoor);
		void AddWindow(Window* pWindow);

		int GetDoorCount();
		Door* GetDoor(int nIndex);
		int GetWindowCount();
		Window* GetWindow(int nIndex);

		double GetThick();

		void AddLinkedSpace(Space* pSpace);
		int GetLinkedSpaceCount();
		SpaceMaker::ISpace* GetLinkedSpace(int nIndex);

		void AddBoundaryVertex(const VectorGraphics::Vertex2D& vertex);
		int GetBoundaryVertexCount();
		VectorGraphics::Vertex2D* GetBoundaryVertex(int nIndex);
		
	private:
		int m_nID;
		double m_dThick;
		double m_dHeight;
		Component* m_pComponent;

		Line2D* m_pLine;
		VectorGraphics::Arc* m_pArc;
		VectorGraphics::EArc* m_pEArc;
		GridType m_gridType;
		std::list<VectorGraphics::Vertex2D> m_boundaries;

		std::list<Door*> m_doors;
		std::list<Window*> m_windows;
		std::list<Space*> m_linkedSpaces;
	};
}
