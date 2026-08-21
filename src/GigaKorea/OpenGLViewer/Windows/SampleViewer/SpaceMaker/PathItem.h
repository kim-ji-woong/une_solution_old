#pragma once
#include <vector>

namespace VectorGraphics
{
	class Vertex2D;
}

namespace SpaceMaker
{
	class IWall;
	class Line2D;

	class PathItem
	{
	public:
		PathItem();
		virtual ~PathItem();

	public:
		void SetLine(Line2D* line, VectorGraphics::Vertex2D* vBegin);
		bool GetVertex(VectorGraphics::Vertex2D& vBegin, VectorGraphics::Vertex2D& vEnd);
		PathItem* Offset(double offset, bool isClockwise);
		void InnerToCenter();
		void SetWall(IWall* pWall);
		IWall* GetWall();

	public:
		static int CalcIntersection(PathItem* item1, PathItem* item2, std::vector<PathItem*>& items, int nItem1Index);

	private:
		static int CalcIntersectionLineToLine(PathItem* item1, PathItem* item2);

	private:
		// m_innerXXX : 교차점 계산에 의하여 잘려진 결과 선형
		// m_XXX : 원래 선형
		Line2D* m_pLine;
		Line2D* m_pInnerLine;
		// 교차점 계산결과 이 선형은 사용하지 않게될 경우 m_innerPass는 true가 된다.
		bool m_innerPass = false;
		IWall* m_pWall;
	};
}
