#pragma once
#include <list>

namespace SpaceMaker
{
	class IWall;
	class ISpace;

	class Manager
	{
	public:
		Manager();
		virtual ~Manager();

	public:
		void AddWall(IWall* pWall);
		void AddSpace(ISpace* pSpace);
		bool Calc();

	private:
		bool MakeOutsideWallLine(void* arg);

	private:
		std::list<IWall*> m_walls;
		std::list<ISpace*> m_spaces;
	};
}
