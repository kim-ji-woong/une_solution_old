#include "StdAfx.h"
#include "GPolygonBuilder.h"
#include "GLine.h"
#include "GVertex.h"
#include "GMath.h"
#include "GPolygon.h"

#include <float.h>

BEGIN_NS(UnE)
BEGIN_NS(Geometry)

#ifdef DOTNET
using namespace System::Collections::Generic;
#endif

PolygonBuilder::PolygonBuilder(void)
{
}

PolygonBuilder::~PolygonBuilder(void)
{
#ifndef DOTNET
	/*for (std::vector<Line2D*>::iterator iter = m_lines.begin(); iter != m_lines.end(); iter++)
	{
		delete *iter;
	}

	m_lines.clear();*/
#endif
}

void PolygonBuilder::AddLine(REF_CONST(UnE::Geometry::Vertex2D) vBegin, REF_CONST(Vertex2D) vEnd)
{
	INSTANCE(Line2D) line = dnonlynew Line2D(vBegin, vEnd);

#ifdef DOTNET
	m_lines->Add(line);
#else
	m_lines.push_back(line);
#endif
}

#ifdef DOTNET
// Polygon 생성 과정에서 기존에 입력된 Line들이 쪼개어지게 되는데
// 최종적으로 쪼개어진 모든 Line들의 집합을 lines에 담는다.
List<Polygon^>^ PolygonBuilder::MakePolygon(OUT List<Line2D^>^% lines)
#else
// Polygon 생성 과정에서 기존에 입력된 Line들이 쪼개어지게 되는데
// 최종적으로 쪼개어진 모든 Line들의 집합을 lines에 담는다.
void PolygonBuilder::MakePolygon(int& rPolygonCount, Polygon*& polygons, int& rLineCount, Line2D*& lines)
#endif
{
#ifdef DOTNET
	lines = nullptr;
	List<Polygon^>^ polygons = gcnew List<Polygon^>();
#endif

#ifdef DOTNET
	int nLineCount = m_lines->Count;

	if (nLineCount <= 2)
		return polygons;
#else
	rPolygonCount = rLineCount = 0;
	polygons = 0;
	lines = 0;

	int nLineCount = (int)m_lines.size();

	if (nLineCount <= 2)
		return;
#endif

	INSTANCE(VertexLink) link = dnonlynew VertexLink();

	for (int i=0;i<nLineCount;i++)
	{
		REF(Line2D) line = m_lines[i];
		OF(link, AddLine(POINTER_ADDR(line)));
	}

#ifdef DOTNET
	lines = link->GetLines();
#else
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

#endif

	OF(link, RemoveSingleNodes());

#ifdef DOTNET
	MakePolygon(link->Nodes, polygons);
	return polygons;
#else
	std::vector<Polygon> vecPolygons;
	MakePolygon(link.Nodes(), vecPolygons);

	rPolygonCount = (int)vecPolygons.size();

	if (rPolygonCount == 0)
		polygons = 0;
	else
	{
		polygons = new Polygon[rPolygonCount];

		for (int i = 0; i < rPolygonCount; i++)
		{
			polygons[i] = vecPolygons[i];
		}
	}
#endif
}

void PolygonBuilder::MakePolygon(REF(STD_VECTOR(POINTER(VertexLink::Node))) nodes, REF(STD_VECTOR(INSTANCE(Polygon))) polygons)
{
	INSTANCE(STD_VECTOR(POINTER(STD_VECTOR(POINTER(VertexLink::Node))))) polygonNodesList = dnonlynew STD_VECTOR(POINTER(STD_VECTOR(POINTER(VertexLink::Node))))();

#ifdef DOTNET
	int nNodeCount = nodes->Count;
#else
	int nNodeCount = (int)nodes.size();
#endif

	for (int i=0;i<nNodeCount;i++)
	{
		POINTER(VertexLink::Node) node = nodes[i];
		MakePolygon(node, polygonNodesList);
	}

#ifdef DOTNET
	int nPolygonNodeCount = polygonNodesList->Count;
#else
	int nPolygonNodeCount = (int)polygonNodesList.size();
#endif

	for (int i=0;i<nPolygonNodeCount;i++)
	{
		POINTER(STD_VECTOR(POINTER(VertexLink::Node))) polygonNodes = polygonNodesList[i];
		INSTANCE(Polygon) polygon = dnonlynew Polygon();

#ifdef DOTNET
		int nPolygonCount = polygonNodes->Count;
#else
		int nPolygonCount = (int)polygonNodes->size();
#endif

		for (int i=0;i<nPolygonCount;i++)
		{
			POINTER(VertexLink::Node) node = POINTER_VALUE(polygonNodes)[i];

#ifdef DOTNET
			polygon->AddVertex(node->Position);
#else
			polygon.AddVertex(node->GetPosition());
#endif
		}

#ifdef DOTNET
		polygons->Add(polygon);
#else
		polygons.push_back(polygon);
#endif
	}

#ifndef DOTNET
	for (std::vector<std::vector<VertexLink::Node*>*>::iterator iter = polygonNodesList.begin(); iter != polygonNodesList.end(); iter++)
	{
		delete *iter;
	}
#endif
}

void PolygonBuilder::MakePolygon(POINTER(VertexLink::Node) node, REF(STD_VECTOR(POINTER(STD_VECTOR(POINTER(VertexLink::Node))))) polygonNodesList)
{
#ifdef DOTNET
	int nLinkCount = node->LinkedNodes->Count;
	List<VertexLink::Node^>^ linkedNodes = node->LinkedNodes;
#else
	int nLinkCount = (int)node->GetLinkedNodes().size();
	std::vector<VertexLink::Node*>& linkedNodes = node->GetLinkedNodes();
#endif

	if (nLinkCount == 0)
		return;

	for (int i = 0; i < nLinkCount; i++)
	{
		POINTER(VertexLink::Node) link = linkedNodes[i];
		POINTER(STD_VECTOR(POINTER(VertexLink::Node))) polygonNodes = MakePolygon(node, link);

		if (polygonNodes != NULL_PTR)
		{
			if (CheckDuplicate(POINTER_VALUE(polygonNodes), polygonNodesList))
			{
#ifdef DOTNET
				polygonNodesList->Add(polygonNodes);
#else
				polygonNodesList.push_back(polygonNodes);
#endif
			}
		}
	}
}

bool PolygonBuilder::CheckDuplicate(REF(STD_VECTOR(POINTER(VertexLink::Node))) polygonNodes, REF(STD_VECTOR(POINTER(STD_VECTOR(POINTER(VertexLink::Node))))) polygonNodesList)
{
#ifdef DOTNET
	int nPolygonNodeCount = polygonNodesList->Count;
#else
	int nPolygonNodeCount = (int)polygonNodesList.size();
#endif

	for (int i=0;i<nPolygonNodeCount;i++)
	{
		POINTER(STD_VECTOR(POINTER(VertexLink::Node))) nodeList = polygonNodesList[i];

		if (IsSamePolygonNodeList(polygonNodes, POINTER_VALUE(nodeList)))
			return false;
	}

	return true;
}

bool PolygonBuilder::IsSamePolygonNodeList(REF(STD_VECTOR(POINTER(VertexLink::Node))) nodes1, REF(STD_VECTOR(POINTER(VertexLink::Node))) nodes2)
{
#ifdef DOTNET
	int nCount1 = nodes1->Count;
	int nCount2 = nodes2->Count;
#else
	int nCount1 = (int)nodes1.size();
	int nCount2 = (int)nodes2.size();
#endif

	if (nCount1 != nCount2)
		return false;

	int nBeginIndex = -1;
	POINTER(VertexLink::Node) firstNode = nodes1[0];

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

		POINTER(VertexLink::Node) node1 = nodes1[i];
		POINTER(VertexLink::Node) node2 = nodes2[j];

		if (node1 != node2)
			return false;
	}

	return true;
}

POINTER(STD_VECTOR(POINTER(VertexLink::Node))) PolygonBuilder::MakePolygon(POINTER(VertexLink::Node) begin, POINTER(VertexLink::Node) next)
{
	POINTER(STD_VECTOR(POINTER(VertexLink::Node))) nodes = geonew STD_VECTOR(POINTER(VertexLink::Node))();

#ifdef DOTNET
	nodes->Add(begin);
	nodes->Add(next);
#else
	nodes->push_back(begin);
	nodes->push_back(next);
#endif

	POINTER(VertexLink::Node) prev = begin;
	POINTER(VertexLink::Node) node = next;

	double dTotalAngle = 0.0, dAngle = 0.0;
	next = GetNextNode(node, prev, dAngle);

	while (next != NULL_PTR)
	{
		dTotalAngle += dAngle;

#ifdef DOTNET
		int nNodeCount = nodes->Count;
#else
		int nNodeCount = (int)nodes->size();
#endif

		if (next == begin)
		{
			// Polygon 회전방향이 반시계 방향이면 무시한다.
			if (nNodeCount < 3 || dTotalAngle < 0.0)
				return NULL_PTR;
			else
				return nodes;
		}
		else
		{
			// 이미 존재하는 노드를 만날 경우 Polygon은 성립할 수 없다.
#ifdef DOTNET
			if (nodes->Contains(next))
				return nullptr;
			else
				nodes->Add(next);
#else
			if (std::find(nodes->begin(), nodes->end(), next) != nodes->end())
				return 0;
			else
				nodes->push_back(next);
#endif
		}

		prev = node;
		node = next;
		next = GetNextNode(node, prev, dAngle);
	}

	return NULL_PTR;
}

POINTER(VertexLink::Node) PolygonBuilder::GetNextNode(POINTER(VertexLink::Node) node, POINTER(VertexLink::Node) prev, OUT CBR(double) dAngle)
{
	dAngle = 0.0;
	double theta = 0.0;
	POINTER(VertexLink::Node) right = NULL_PTR;

#ifdef DOTNET
	int nLinkCount = node->LinkedNodes->Count;
	List<VertexLink::Node^>^ linkedNodes = node->LinkedNodes;
#else
	int nLinkCount = (int)node->GetLinkedNodes().size();
	std::vector<VertexLink::Node*>& linkedNodes = node->GetLinkedNodes();
#endif

	for (int i=0;i<nLinkCount;i++)
	{
		POINTER(VertexLink::Node) link = linkedNodes[i];

		if (link == prev)
			continue;

#ifdef DOTNET
		Vertex2D^ linkPosition = link->Position;
		Vertex2D^ nodePosition = node->Position;
		Vertex2D^ prevPosition = prev->Position;
#else
		Vertex2D& linkPosition = link->GetPosition();
		Vertex2D& nodePosition = node->GetPosition();
		Vertex2D& prevPosition = prev->GetPosition();
#endif

		if (UnE::Geometry::Math::IsRightSideFromLine(linkPosition, nodePosition, prevPosition) != 0)
		{
			theta = UnE::Geometry::Math::PI() - UnE::Geometry::Math::GetAngle(linkPosition, nodePosition, prevPosition);
		}
		else
		{
			theta = UnE::Geometry::Math::GetAngle(linkPosition, nodePosition, prevPosition) - UnE::Geometry::Math::PI();
		}

		if (right == NULL_PTR || theta > dAngle)
		{
			dAngle = theta;
			right = link;
		}
	}

	return right;
}

END_NS
END_NS
