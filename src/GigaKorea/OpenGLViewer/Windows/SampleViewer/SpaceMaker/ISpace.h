#pragma once

namespace VectorGraphics
{
	class Vertex2D;
}

namespace SpaceMaker
{
	class IWall;

	class ISpace
	{
	public:
		ISpace();
		~ISpace();

	public:
		virtual int GetWallCount() = 0;
		virtual IWall* GetWall(int nIndex) = 0;
		virtual void AddBoundaryVertex(const VectorGraphics::Vertex2D& vertex) = 0;
	};
}
