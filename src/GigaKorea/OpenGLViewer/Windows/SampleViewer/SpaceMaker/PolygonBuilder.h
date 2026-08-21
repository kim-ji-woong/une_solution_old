#pragma once
#include "Line2D.h"
#include "POI.h"
#include "VertexLink.h"

namespace SpaceMaker
{
	class PolygonBuilder
	{
	public:
		PolygonBuilder();
		virtual ~PolygonBuilder();

	public:
		void AddLine(const VectorGraphics::Vertex2D& vBegin, const VectorGraphics::Vertex2D& vEnd);

		// Polygon 생성 과정에서 기존에 입력된 Line들이 쪼개어지게 되는데
		// 최종적으로 쪼개어진 모든 Line들의 집합을 lines에 담는다.
		void MakePolygon(int& rPolygonCount, VectorGraphics::VertexList*& polygons, int& rLineCount, Line2D*& lines);

	private:
		VertexLink::Node* GetNextNode(VertexLink::Node* node, VertexLink::Node* prev, double& dAngle);
		void MakePolygon(std::vector<VertexLink::Node*>& nodes, std::vector<VectorGraphics::VertexList>& polygons);
		void MakePolygon(VertexLink::Node* node, std::vector<std::vector<VertexLink::Node*>*>& polygonNodesList);
		bool CheckDuplicate(std::vector<VertexLink::Node*>& polygonNodes, std::vector<std::vector<VertexLink::Node*>*>& polygonNodesList);
		bool IsSamePolygonNodeList(std::vector<VertexLink::Node*>& nodes1, std::vector<VertexLink::Node*>& nodes2);
		std::vector<VertexLink::Node*>* MakePolygon(VertexLink::Node* begin, VertexLink::Node* next);

	private:
		std::vector<Line2D> m_lines;
	};
}
