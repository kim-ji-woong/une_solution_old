#include "stdafx.h"
#include "VertexLink.h"
#include "Line2D.h"

using namespace VectorGraphics;

namespace SpaceMaker
{
	template<class Type1, class Type2>
	bool FindMap(std::map<Type1, Type2> m, Type1 key, Type2& value)
	{
		std::map<Type1, Type2>::iterator iter = m.find(key);

		if (iter == m.end())
			return false;

		value = iter->second;
		return true;
	}

	VertexLink::Node::Node(void)
	{
	}

	VertexLink::Node::Node(const Vertex2D& vPos)
	{
		m_pos = vPos;
	}

	void VertexLink::Node::AddLink(VertexLink::Node* node)
	{
		int nNodeCount = (int)m_linkedNodes.size();

		for (int i = 0; i < nNodeCount; i++)
		{
			Node* link = m_linkedNodes[i];

			if (link == node)
				return;
		}

		m_linkedNodes.push_back(node);
	}

	VertexLink::VertexLink()
	{
	}

	VertexLink::~VertexLink()
	{
		int nNodeCount = (int)m_nodes.size();

		for (int i = 0; i < nNodeCount; i++)
		{
			delete m_nodes[i];
		}

		m_nodes.clear();

		for (std::map<Line2D*, std::pair<Node*, Node*>>::iterator iter = m_dicLineVertex.begin(); iter != m_dicLineVertex.end(); iter++)
		{
			delete iter->first;
		}

		m_dicLineVertex.clear();
	}

	__int64 VertexToLong(const Vertex2D& vertex)
	{
		__int64 x = (__int64)(vertex.x + 0.5);
		__int64 y = (__int64)(vertex.y + 0.5);

		__int64 key = ((x << 32) | y);
		return key;
	}

	extern int IntersectLineToLine(Line2D& rLine1, Line2D& rLine2, Vertex2D& rVertex1, Vertex2D& rVertex2);

	static const double _HALF_PI = 1.57079632679489661923;
	static const double _PI = 3.14159265358979323846;

	static double GetAngle(const Vertex2D& v1, const Vertex2D& vCenter, const Vertex2D& v2)
	{
		// 코사인 제2법칙
		// C²= A²+ B²- 2ABcosΘ
		double a = vCenter.GetDistance(v1);
		double b = v2.GetDistance(vCenter);
		double c = v2.GetDistance(v1);

		double cosData = (a * a + b * b - c * c) / 2 / a / b;
		if (cosData < -1.0) cosData = -1.0;
		else if (cosData > 1.0) cosData = 1.0;

		return acos(cosData);
	}

	static double GetDistance(const Vertex2D& rVertex, const Vertex2D& vBegin, const Vertex2D& vEnd)
	{
		double dTolerance = 0.001;

		double a = vBegin.GetDistance(rVertex);
		double b = vBegin.GetDistance(vEnd);
		double c = vEnd.GetDistance(rVertex);

		if (a <= dTolerance || c <= dTolerance)
			return 0.0;
		if (b <= dTolerance)
			return a;

		double dCos = (a * a + b * b - c * c) / 2 / a / b;
		Vertex2D vertex = vBegin.GetLinearVertex(vEnd, dCos * a);
		double dLen = vertex.GetDistance(rVertex);

		double dAngle1 = GetAngle(rVertex, vBegin, vEnd);
		double dAngle2 = GetAngle(rVertex, vEnd, vBegin);

		if (dAngle1 <= _HALF_PI && dAngle2 <= _HALF_PI)
			return dLen;

		return a > c ? c : a;
	}

	bool IsInclude(const Vertex2D& rVertex, const Vertex2D& vBegin, const Vertex2D& vEnd)
	{
		double dLen = GetDistance(rVertex, vBegin, vEnd);
		if (dLen <= 0.1)
			return true;

		return false;
	}

	void VertexLink::AddLine(Line2D* line)
	{
		int nNodeCount = (int)m_nodes.size();

		if (nNodeCount == 0)
		{
			AddLineNode(line);
		}
		else
		{
			Vertex2D v1;
			Vertex2D v2;

			std::map<Line2D*, Vertex2D> dicSplitLines;
			// Key : 좌표의 소숫점 첫째자리에서 반올림 한 이후 정수로 변환하여 상위 4바이트는 x, 하위 4바이트는 y값으로 만든다.
			std::map<__int64, Node*> recycleNode;
			
			std::vector<Line2D*> removeLines;

			// line이 몇개의 버텍스로 쪼개어지는가를 기록하기위한 List
			std::vector<Vertex2D> linearVertices;

			linearVertices.push_back(line->GetVertex(true));
			linearVertices.push_back(line->GetVertex(false));

			for (std::map<Line2D*, std::pair<Node*, Node*>>::iterator iter = m_dicLineVertex.begin(); iter != m_dicLineVertex.end(); iter++)
			{
				Line2D* key = iter->first;
				Node* firstNode = iter->second.first;
				Node* secondNode = iter->second.second;
				Vertex2D& firstNodePosition = firstNode->GetPosition();
				Vertex2D& secondNodePosition = secondNode->GetPosition();

				int nResult = IntersectLineToLine(*key, *line, v1, v2);

				// 한점에서 만날 경우
				if (nResult == 1)
				{
					bool sameBegin = IsSameVertex(v1, firstNodePosition);
					bool sameEnd = IsSameVertex(v1, secondNodePosition);

					if (sameBegin == false && sameEnd == false)
					{
						// pair.Key는 v1에 의하여 쪼개어진다.
						dicSplitLines[key] = v1;
					}
					else if (sameBegin)
					{
						recycleNode[VertexToLong(v1)] = firstNode;
					}
					else if (sameEnd)
					{
						recycleNode[VertexToLong(v1)] = secondNode;
					}

					AddLinearVertex(linearVertices, v1);
				}
				// 두점에서 만날 경우
				else if (nResult == 2)
				{
					// v1이 pair.Key 내에 포함되어 있는가?
					bool v1Include = IsInclude(v1, key->GetVertex(true), key->GetVertex(false));
					// v2가 pair.Key 내에 포함되어 있는가?
					bool v2Include = IsInclude(v2, key->GetVertex(true), key->GetVertex(false));

					if (v1Include && !v2Include)
					{
						if (v2.GetDistance(firstNodePosition) < v2.GetDistance(secondNodePosition))
							SetLinearVertexBegin(linearVertices, firstNodePosition);
						else
							SetLinearVertexBegin(linearVertices, secondNodePosition);
					}
					else if (!v1Include && v2Include)
					{
						if (v1.GetDistance(firstNodePosition) < v1.GetDistance(secondNodePosition))
							SetLinearVertexEnd(linearVertices, firstNodePosition);
						else
							SetLinearVertexEnd(linearVertices, secondNodePosition);
					}
					else if (v1Include && v2Include)
					{
						// line이 pair.Key내에 완전히 포함되어 있는 경우
						//return;
						continue;
					}
					else
					{
						// pair.Key가 line내에 완전히 속해있는 경우
						removeLines.push_back(key);

						AddLinearVertex(linearVertices, key->GetVertex(true));
						AddLinearVertex(linearVertices, key->GetVertex(false));

						recycleNode[VertexToLong(key->GetVertex(true))] = firstNode;
						recycleNode[VertexToLong(key->GetVertex(false))] = secondNode;
					}
				}
			}

			for (std::vector<Line2D*>::iterator iter = removeLines.begin(); iter != removeLines.end(); iter++)
			{
				m_dicLineVertex.erase(*iter);
				delete *iter;
			}

			Node* prevNode = 0;
			std::vector<Node*> linearNodes;

			int nVertexCount = (int)linearVertices.size();

			for (int i = 0; i<nVertexCount; i++)
			{
				const Vertex2D& vPos = linearVertices[i];
				Node* node = FindRecycleNode(vPos, recycleNode);

				if (node == 0)
				{
					node = FindNode(vPos);

					if (node == 0)
					{
						node = new Node(vPos);
						m_nodes.push_back(node);
					}
				}

				linearNodes.push_back(node);

				if (prevNode != 0)
				{
					Vertex2D& prevNodePosition = prevNode->GetPosition();
					Vertex2D& nodePosition = node->GetPosition();

					prevNode->AddLink(node);
					node->AddLink(prevNode);

					m_dicLineVertex[new Line2D(prevNodePosition, nodePosition)] = std::pair<Node*, Node*>(prevNode, node);
				}

				prevNode = node;
			}

			for (std::map<Line2D*, Vertex2D>::iterator iter = dicSplitLines.begin(); iter != dicSplitLines.end(); iter++)
			{
				SplitLine(iter->first, iter->second, linearNodes);
			}

		}
	}

	VertexLink::Node* VertexLink::FindRecycleNode(const Vertex2D& vPos, std::map<__int64, VertexLink::Node*>& recycleNodes)
	{
		__int64 key = VertexToLong(vPos);
		std::map<__int64, VertexLink::Node*>::iterator iter = recycleNodes.find(key);

		if (iter != recycleNodes.end())
			return iter->second;

		return 0;
	}
	
	void VertexLink::SplitLine(Line2D* line, const Vertex2D& vertex, std::vector<Node*>& linearNodes)
	{
		std::pair<Node*, Node*> pair;

		if (FindMap<Line2D*, std::pair<Node*, Node*>>(m_dicLineVertex, line, pair))
		{
			Line2D* lineBegin = new Line2D(pair.first->GetPosition(), vertex);
			Line2D* lineEnd = new Line2D(vertex, pair.second->GetPosition());

			Node* newNode = GetLinearNode(vertex, linearNodes);

			if (newNode != 0)
			{
				pair.first->AddLink(newNode);
				pair.second->AddLink(newNode);
				newNode->AddLink(pair.first);
				newNode->AddLink(pair.second);

				m_dicLineVertex[lineBegin] = std::pair<Node*, Node*>(pair.first, newNode);
				m_dicLineVertex[lineEnd] = std::pair<Node*, Node*>(newNode, pair.second);

				if (newNode != pair.first && newNode != pair.second)
				{
					pair.first->GetLinkedNodes().erase(std::find(pair.first->GetLinkedNodes().begin(), pair.first->GetLinkedNodes().end(), pair.second));
					pair.second->GetLinkedNodes().erase(std::find(pair.second->GetLinkedNodes().begin(), pair.second->GetLinkedNodes().end(), pair.first));
				}
			}

			m_dicLineVertex.erase(line);
		}
	}

	VertexLink::Node* VertexLink::GetLinearNode(const Vertex2D& vertex, std::vector<VertexLink::Node*>& linearNodes)
	{
		for (std::vector<Node*>::iterator iter = linearNodes.begin(); iter != linearNodes.end(); iter++)
		{
			Node* node = *iter;
			Vertex2D& nodePosition = node->GetPosition();

			if (IsSameVertex(vertex, nodePosition))
			{
				return node;
			}
		}

		return 0;
	}

	// vNew보다 앞에 있는 Vertex들을 모두 지우고, vNew를 시작점으로 한다.
	void VertexLink::SetLinearVertexBegin(std::vector<Vertex2D>& linearVertices, const Vertex2D& vNew)
	{
		int nVertexCount = linearVertices.size();
		double dPrevLen = 0.0;

		for (int i = 0; i < nVertexCount; i++)
		{
			Vertex2D& vertex = linearVertices[i];

			double dLen = vertex.GetDistance(vNew);

			if (IsSame(dLen))
			{
				for (int j = 0; j < i; j++)
				{
					linearVertices.erase(linearVertices.begin());
				}

				return;
			}

			if (i == 0)
				dPrevLen = dLen;
			else
			{
				double dLen2 = vertex.GetDistance(linearVertices[i - 1]);

				if (dPrevLen < dLen2)
				{
					for (int j = 0; j < i; j++)
						linearVertices.erase(linearVertices.begin());

					linearVertices.insert(linearVertices.begin(), vNew);
					return;
				}
				else
					dPrevLen = dLen;
			}
		}
	}

	// vNew보다 뒤에 있는 Vertex들을 모두 지우고, vNew를 끝점으로 한다.
	void VertexLink::SetLinearVertexEnd(std::vector<Vertex2D>& linearVertices, const Vertex2D& vNew)
	{
		int nVertexCount = linearVertices.size();

		double dPrevLen = 0.0;

		for (int i = nVertexCount - 1; i >= 0; i--)
		{
			Vertex2D& vertex = linearVertices[i];

			double dLen = vertex.GetDistance(vNew);

			if (IsSame(dLen))
			{
				for (int j = i + 1; j < nVertexCount; j++)
				{
					linearVertices.erase(linearVertices.begin() + i + 1);
				}

				return;
			}

			if (i == nVertexCount - 1)
				dPrevLen = dLen;
			else
			{
				double dLen2 = vertex.GetDistance(linearVertices[i + 1]);

				if (dPrevLen < dLen2)
				{
					for (int j = i + 1; j < nVertexCount; j++)
						linearVertices.erase(linearVertices.begin() + i + 1);

					linearVertices.push_back(vNew);
					return;
				}
				else
					dPrevLen = dLen;
			}
		}
	}

	// vNew를 linearVertices에 시작점과 가까운 순으로 정렬하여 삽입한다.
	void VertexLink::AddLinearVertex(std::vector<Vertex2D>& linearVertices, const Vertex2D& vNew)
	{
		int nVertexCount = linearVertices.size();
		double dPrevLen = 0.0;

		for (int i = 0; i < nVertexCount; i++)
		{
			Vertex2D& vertex = linearVertices[i];
			double dLen = vertex.GetDistance(vNew);

			if (IsSame(dLen))
				return;

			if (i == 0)
				dPrevLen = dLen;
			else
			{
				double dLen2 = vertex.GetDistance(linearVertices[i - 1]);

				if (dPrevLen < dLen2)
				{
					linearVertices.insert(linearVertices.begin() + i, vNew);
					return;
				}
				else
					dPrevLen = dLen;
			}
		}
	}

	bool VertexLink::IsSameVertex(const Vertex2D& v1, const Vertex2D& v2)
	{
		double len = v1.GetDistance(v2);
		return IsSame(len);
	}

	bool VertexLink::IsSame(double len)
	{
		return len < 0.1;
		//return len < Math::HALF_TOLERANCE();
	}

	VertexLink::Node* VertexLink::FindNode(const Vertex2D& vPos)
	{
		for (std::vector<Node*>::iterator iter = m_nodes.begin(); iter != m_nodes.end(); iter++)
		{
			Node* node = *iter;

			if (node->GetPosition().GetDistance(vPos) < 0.001)
				return node;
		}

		return 0;
	}

	void VertexLink::AddLineNode(Line2D* line)
	{
		Node* begin = new Node(line->GetVertex(true));
		Node* end = new Node(line->GetVertex(false));

		begin->GetLinkedNodes().push_back(end);
		end->GetLinkedNodes().push_back(begin);

		m_nodes.push_back(begin);
		m_nodes.push_back(end);

		Line2D* newLine = new Line2D(line->GetVertex(true), line->GetVertex(false));
		m_dicLineVertex[newLine] = std::pair<Node*, Node*>(begin, end);
	}

	int VertexLink::GetLines(std::vector<Line2D>& lines)
	{
		for (std::map< Line2D*, std::pair<Node*, Node*>>::iterator iter = m_dicLineVertex.begin(); iter != m_dicLineVertex.end(); iter++)
		{
			lines.push_back(*iter->first);
		}

		return (int)lines.size();
	}

	// 다른 노드와 연결되지 않은 노드들을 모두 없앤다.
	void VertexLink::RemoveSingleNodes()
	{
		std::vector<Node*> singleNodes;

		for (std::vector<Node*>::iterator iter = m_nodes.begin(); iter != m_nodes.end(); iter++)
		{
			Node* node = *iter;

			if ((int)node->GetLinkedNodes().size() <= 1)
				singleNodes.push_back(node);
		}

		for (std::vector<Node*>::iterator iter = singleNodes.begin(); iter != singleNodes.end(); iter++)
		{
			std::vector<Node*>::iterator remove = std::find(m_nodes.begin(), m_nodes.end(), *iter);

			if (remove != m_nodes.end())
			{
				m_nodes.erase(remove);
				RemoveSingleNode(*iter);
			}
		}
	}

	template<class T>
	void RemoveVector(std::vector<T>& vec, T value)
	{
		for (std::vector<T>::iterator iter = vec.begin(); iter != vec.end(); iter++)
		{
			if (*iter == value)
			{
				vec.erase(iter);
				break;
			}
		}
	}

	void VertexLink::RemoveSingleNode(Node* node)
	{
		for (int i = 0; i < (int)node->GetLinkedNodes().size(); i++)
		//for (std::vector<Node*>::iterator iter = node->GetLinkedNodes().begin(); iter != node->GetLinkedNodes().end(); iter++)
		{
			Node* link = node->GetLinkedNodes()[i];
			//Node* link = *iter;
			RemoveVector<Node*>(link->GetLinkedNodes(), node);

			if ((int)link->GetLinkedNodes().size() <= 1)
			{
				RemoveVector<Node*>(m_nodes, link);
				RemoveSingleNode(link);
			}
		}

		node->GetLinkedNodes().clear();
		//delete node;
	}
}
