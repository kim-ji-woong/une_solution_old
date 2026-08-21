#include "stdafx.h"
#include "PolygonBuilder.h"

using namespace VectorGraphics;

namespace SpaceMaker
{
	PolygonBuilder::PolygonBuilder(void)
	{
	}

	PolygonBuilder::~PolygonBuilder(void)
	{
	}

	void PolygonBuilder::AddLine(const Vertex2D& vBegin, const Vertex2D& vEnd)
	{
		Line2D line(vBegin, vEnd);
		m_lines.push_back(line);
	}

	// Polygon 생성 과정에서 기존에 입력된 Line들이 쪼개어지게 되는데
	// 최종적으로 쪼개어진 모든 Line들의 집합을 lines에 담는다.
	void PolygonBuilder::MakePolygon(int& rPolygonCount, VertexList*& polygons, int& rLineCount, Line2D*& lines)
	{
		rPolygonCount = rLineCount = 0;
		polygons = 0;
		lines = 0;

		int nLineCount = (int)m_lines.size();

		if (nLineCount <= 2)
			return;

		VertexLink link;

		for (int i = 0; i<nLineCount; i++)
		{
			Line2D& line = m_lines[i];
			link.AddLine(&line);
		}

		std::vector<Line2D> vecLines;
		link.GetLines(vecLines);

		rLineCount = (int)vecLines.size();

		if (rLineCount == 0)
			lines = 0;
		else
		{
			lines = new Line2D[rLineCount];

			for (int i = 0; i < rLineCount; i++)
			{
				lines[i] = vecLines[i];
			}
		}

		link.RemoveSingleNodes();

		std::vector<VertexList> vecPolygons;
		MakePolygon(link.Nodes(), vecPolygons);

		rPolygonCount = (int)vecPolygons.size();

		if (rPolygonCount == 0)
			polygons = 0;
		else
		{
			polygons = new VertexList[rPolygonCount];

			for (int i = 0; i < rPolygonCount; i++)
			{
				polygons[i] = vecPolygons[i];
			}
		}
	}

	void PolygonBuilder::MakePolygon(std::vector<VertexLink::Node*>& nodes, std::vector<VertexList>& polygons)
	{
		std::vector<std::vector<VertexLink::Node*>*> polygonNodesList;

		int nNodeCount = (int)nodes.size();

		for (int i = 0; i<nNodeCount; i++)
		{
			VertexLink::Node* node = nodes[i];
			MakePolygon(node, polygonNodesList);
		}

		int nPolygonNodeCount = (int)polygonNodesList.size();

		for (int i = 0; i<nPolygonNodeCount; i++)
		{
			std::vector<VertexLink::Node*>* polygonNodes = polygonNodesList[i];
			VertexList polygon;

			int nPolygonCount = (int)polygonNodes->size();

			for (int i = 0; i<nPolygonCount; i++)
			{
				VertexLink::Node* node = (*polygonNodes)[i];
				polygon.Vertices.push_back(node->GetPosition());
			}

			polygons.push_back(polygon);
		}

		for (std::vector<std::vector<VertexLink::Node*>*>::iterator iter = polygonNodesList.begin(); iter != polygonNodesList.end(); iter++)
		{
			delete *iter;
		}
	}

	void PolygonBuilder::MakePolygon(VertexLink::Node* node, std::vector<std::vector<VertexLink::Node*>*>& polygonNodesList)
	{
		int nLinkCount = (int)node->GetLinkedNodes().size();
		std::vector<VertexLink::Node*>& linkedNodes = node->GetLinkedNodes();

		if (nLinkCount == 0)
			return;

		for (int i = 0; i < nLinkCount; i++)
		{
			VertexLink::Node* link = linkedNodes[i];
			std::vector<VertexLink::Node*>* polygonNodes = MakePolygon(node, link);

			if (polygonNodes != 0)
			{
				if (CheckDuplicate(*polygonNodes, polygonNodesList))
				{
					polygonNodesList.push_back(polygonNodes);
				}
			}
		}
	}

	bool PolygonBuilder::CheckDuplicate(std::vector<VertexLink::Node*>& polygonNodes, std::vector<std::vector<VertexLink::Node*>*>& polygonNodesList)
	{
		int nPolygonNodeCount = (int)polygonNodesList.size();

		for (int i = 0; i<nPolygonNodeCount; i++)
		{
			std::vector<VertexLink::Node*>* nodeList = polygonNodesList[i];

			if (IsSamePolygonNodeList(polygonNodes, *nodeList))
				return false;
		}

		return true;
	}

	bool PolygonBuilder::IsSamePolygonNodeList(std::vector<VertexLink::Node*>& nodes1, std::vector<VertexLink::Node*>& nodes2)
	{
		int nCount1 = (int)nodes1.size();
		int nCount2 = (int)nodes2.size();

		if (nCount1 != nCount2)
			return false;

		int nBeginIndex = -1;
		VertexLink::Node* firstNode = nodes1[0];

		for (int i = 0; i < nCount2; i++)
		{
			if (firstNode == nodes2[i])
			{
				nBeginIndex = i;
				break;
			}
		}

		if (nBeginIndex < 0)
			return false;

		for (int i = 1, j = nBeginIndex + 1; i < nCount1; i++, j++)
		{
			if (j >= nCount2)
				j = 0;

			VertexLink::Node* node1 = nodes1[i];
			VertexLink::Node* node2 = nodes2[j];

			if (node1 != node2)
				return false;
		}

		return true;
	}

	std::vector<VertexLink::Node*>* PolygonBuilder::MakePolygon(VertexLink::Node* begin, VertexLink::Node* next)
	{
		std::vector<VertexLink::Node*>* nodes = new std::vector<VertexLink::Node*>();

		nodes->push_back(begin);
		nodes->push_back(next);

		VertexLink::Node* prev = begin;
		VertexLink::Node* node = next;

		double dTotalAngle = 0.0, dAngle = 0.0;
		next = GetNextNode(node, prev, dAngle);

		while (next != 0)
		{
			dTotalAngle += dAngle;

			int nNodeCount = (int)nodes->size();

			if (next == begin)
			{
				// Polygon 회전방향이 반시계 방향이면 무시한다.
				if (nNodeCount < 3 || dTotalAngle < 0.0)
					return 0;
				else
					return nodes;
			}
			else
			{
				// 이미 존재하는 노드를 만날 경우 Polygon은 성립할 수 없다.
				if (std::find(nodes->begin(), nodes->end(), next) != nodes->end())
					return 0;
				else
					nodes->push_back(next);
			}

			prev = node;
			node = next;
			next = GetNextNode(node, prev, dAngle);
		}

		return 0;
	}

	extern double GetAngle(const Vertex2D& v1, const Vertex2D& vCenter, const Vertex2D& v2);
	
	static const double _HALF_PI = 1.57079632679489661923;
	static const double _PI = 3.14159265358979323846;

	int IsRightSideFromLine(const Vertex2D& rVertex, const Vertex2D& vBegin, const Vertex2D& vEnd)
	{
		Vertex2D vR = vBegin.GetRightVertex(vEnd, 100.0);
		double dAngle1 = GetAngle(vEnd, vBegin, rVertex);
		double dAngle2 = GetAngle(rVertex, vBegin, vR);

		Vertex2D v;

		if (dAngle1 < _HALF_PI)
		{
			double dLen = vBegin.GetDistance(rVertex);
			v = vBegin.GetLinearVertex(vEnd, dLen);
		}
		else
		{
			double dLen = vEnd.GetDistance(rVertex);
			v = vEnd.GetLinearVertex(vBegin, dLen);
		}

		if (v.GetDistance(rVertex) <= 0.001)
			return -1;

		if (dAngle2 < _HALF_PI)
			return 1;

		return 0;
	}

	VertexLink::Node* PolygonBuilder::GetNextNode(VertexLink::Node* node, VertexLink::Node* prev, double& dAngle)
	{
		dAngle = 0.0;
		double theta = 0.0;
		VertexLink::Node* right = 0;

		int nLinkCount = (int)node->GetLinkedNodes().size();
		std::vector<VertexLink::Node*>& linkedNodes = node->GetLinkedNodes();

		for (int i = 0; i<nLinkCount; i++)
		{
			VertexLink::Node* link = linkedNodes[i];

			if (link == prev)
				continue;

			Vertex2D& linkPosition = link->GetPosition();
			Vertex2D& nodePosition = node->GetPosition();
			Vertex2D& prevPosition = prev->GetPosition();

			if (IsRightSideFromLine(linkPosition, nodePosition, prevPosition) != 0)
			{
				theta = _PI - GetAngle(linkPosition, nodePosition, prevPosition);
			}
			else
			{
				theta = GetAngle(linkPosition, nodePosition, prevPosition) - _PI;
			}

			if (right == 0 || theta > dAngle)
			{
				dAngle = theta;
				right = link;
			} 
		}

		return right;
	}
}
