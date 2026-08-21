#include "StdAfx.h"
#include "GVertexLink.h"
#include "GLine.h"
#include "GMath.h"

#ifndef DOTNET
#include <string>
#endif

BEGIN_NS(UnE)
BEGIN_NS(Geometry)

#ifdef DOTNET
using namespace System::Collections::Generic;
#endif

#ifndef DOTNET
template<class Type1, class Type2>
bool FindMap(std::map<Type1, Type2> m, Type1 key, Type2& value)
{
	std::map<Type1, Type2>::iterator iter = m.find(key);

	if (iter == m.end())
		return false;

	value = iter->second;
	return true;
}
#endif

VertexLink::Node::Node(void)
{
#ifdef DOTNET
	m_pos = nullptr;
	m_linkedNodes = gcnew System::Collections::Generic::List<Node^>();
#endif
}

VertexLink::Node::Node(REF_CONST(Vertex2D) vPos)
{
	m_pos = vPos;

#ifdef DOTNET
	m_linkedNodes = gcnew System::Collections::Generic::List<Node^>();
#endif
}

void VertexLink::Node::AddLink(POINTER(VertexLink::Node) node)
{
#ifdef DOTNET
	int nNodeCount = m_linkedNodes->Count;
#else
	int nNodeCount = (int)m_linkedNodes.size();
#endif

	for (int i = 0; i < nNodeCount; i++)
	{
		POINTER(Node) link = m_linkedNodes[i];

		if (link == node)
			return;
	}

#ifdef DOTNET
	m_linkedNodes->Add(node);
#else
	m_linkedNodes.push_back(node);
#endif
}

VertexLink::VertexLink()
{
#ifdef DOTNET
	m_nodes = gcnew System::Collections::Generic::List<Node^>();
#endif
}

VertexLink::~VertexLink()
{
#ifndef DOTNET
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
#endif
}

NormalString VertexToString(REF_CONST(Vertex2D) vertex)
{
	__int64 x = (__int64)(OF(vertex, x) + 0.5);
	__int64 y = (__int64)(OF(vertex, y) + 0.5);

#ifdef DOTNET
	return x.ToString() + "," + y.ToString();
#else
	char str[256];
	sprintf_s(str, "%I64d,%I64d", x, y);
	return str;
#endif
}

/*__int64 VertexToLong(REF_CONST(Vertex2D) vertex)
{
	__int64 x = (__int64)(OF(vertex, x) + 0.5);
	__int64 y = (__int64)(OF(vertex, y) + 0.5);

	__int64 key = ((x << 32) | y);
	return key;
}*/

void VertexLink::AddLine(POINTER(Line2D) line)
{
#ifdef DOTNET
	int nNodeCount = m_nodes->Count;
#else
	int nNodeCount = (int)m_nodes.size();
#endif

	if (nNodeCount == 0)
	{
		AddLineNode(line);
	}
	else
	{
		INSTANCE(Vertex2D) v1;
		INSTANCE(Vertex2D) v2;
		Line2D::LineType resultType;

#ifdef DOTNET
		Dictionary<Line2D^, Vertex2D^>^ dicSplitLines = gcnew Dictionary<Line2D^, Vertex2D^>();
		// Key : 좌표의 소숫점 첫째자리에서 반올림 한 이후 정수로 변환하여 상위 4바이트는 x, 하위 4바이트는 y값으로 만든다.
		Dictionary<NormalString, Node^>^ recycleNode = gcnew Dictionary<NormalString, Node^>();
		//Dictionary<Vertex2D^, Node^>^ recycleNode = gcnew Dictionary<Vertex2D^, Node^>();
#else
		std::map<Line2D*, Vertex2D> dicSplitLines;
		// Key : 좌표의 소숫점 첫째자리에서 반올림 한 이후 정수로 변환하여 상위 4바이트는 x, 하위 4바이트는 y값으로 만든다.
		std::map<NormalString, Node*> recycleNode;
		//std::vector<std::pair<Vertex2D, Node*>> recycleNode;
#endif
		INSTANCE(STD_VECTOR(POINTER(Line2D))) removeLines = dnonlynew STD_VECTOR(POINTER(Line2D))();

		// line이 몇개의 버텍스로 쪼개어지는가를 기록하기위한 List
		INSTANCE(STD_VECTOR(INSTANCE(Vertex2D))) linearVertices = dnonlynew STD_VECTOR(INSTANCE(Vertex2D))();

#ifdef DOTNET
		linearVertices->Add(line->GetVertex(true));
		linearVertices->Add(line->GetVertex(false));
#else
		linearVertices.push_back(line->GetVertex(true));
		linearVertices.push_back(line->GetVertex(false));
#endif

#ifdef DOTNET
		for each (KeyValuePair<Line2D^, KeyValuePair<Node^, Node^>>^ pair in m_dicLineVertex)
		{
			Line2D^ key = pair->Key;
			Node^ firstNode = pair->Value.Key;
			Node^ secondNode = pair->Value.Value;
			Vertex2D^ firstNodePosition = firstNode->Position;
			Vertex2D^ secondNodePosition = secondNode->Position;
#else
		for (std::map<Line2D*, std::pair<Node*, Node*>>::iterator iter = m_dicLineVertex.begin();iter != m_dicLineVertex.end();iter++)
		{
			Line2D* key = iter->first;
			Node* firstNode = iter->second.first;
			Node* secondNode = iter->second.second;
			Vertex2D& firstNodePosition = firstNode->GetPosition();
			Vertex2D& secondNodePosition = secondNode->GetPosition();
#endif
			int nResult = key->IntersectLine(POINTER_VALUE(line), v1, v2, resultType);

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
					recycleNode[VertexToString(v1)] = firstNode;
					//recycleNode[VertexToLong(v1)] = firstNode;
				}
				else if (sameEnd)
				{
					recycleNode[VertexToString(v1)] = secondNode;
					//recycleNode[VertexToLong(v1)] = secondNode;
				}

				AddLinearVertex(linearVertices, v1);
			}
			// 두점에서 만날 경우
			else if (nResult == 2)
			{
				// v1이 pair.Key 내에 포함되어 있는가?
				bool v1Include = key->IsInclude(v1);
				// v2가 pair.Key 내에 포함되어 있는가?
				bool v2Include = key->IsInclude(v2);

				if (v1Include && !v2Include)
				{
					if (OF(v2, GetDistance(firstNodePosition)) < OF(v2, GetDistance(secondNodePosition)))
						SetLinearVertexBegin(linearVertices, firstNodePosition);
					else
						SetLinearVertexBegin(linearVertices, secondNodePosition);
				}
				else if (!v1Include && v2Include)
				{
					if (OF(v1, GetDistance(firstNodePosition)) < OF(v1, GetDistance(secondNodePosition)))
						SetLinearVertexEnd(linearVertices, firstNodePosition);
					else
						SetLinearVertexEnd(linearVertices, secondNodePosition);
				}
				else if (v1Include && v2Include)
				{
					if (OF(key->GetVertex(true), GetDistance(key->GetVertex(false))) > OF(line->GetVertex(true), GetDistance(line->GetVertex(false))))
					{
						// line이 pair.Key내에 완전히 포함되어 있는 경우
						continue;
					}
					else
					{
#ifdef DOTNET
						// pair.Key가 line내에 완전히 속해있는 경우
						removeLines->Add(pair->Key);
#else
						// pair.Key가 line내에 완전히 속해있는 경우
						removeLines.push_back(key);
#endif

						AddLinearVertex(linearVertices, key->GetVertex(true));
						AddLinearVertex(linearVertices, key->GetVertex(false));

						recycleNode[VertexToString(key->GetVertex(true))] = firstNode;
						recycleNode[VertexToString(key->GetVertex(false))] = secondNode;
					}
				}
				/*else
				{
#ifdef DOTNET
					// pair.Key가 line내에 완전히 속해있는 경우
					removeLines->Add(pair->Key);
#else
					// pair.Key가 line내에 완전히 속해있는 경우
					removeLines.push_back(key);
#endif

					AddLinearVertex(linearVertices, key->GetVertex(true));
					AddLinearVertex(linearVertices, key->GetVertex(false));

					recycleNode[VertexToString(key->GetVertex(true))] = firstNode;
					recycleNode[VertexToString(key->GetVertex(false))] = secondNode;
					//recycleNode[VertexToLong(key->GetVertex(true))] = firstNode;
					//recycleNode[VertexToLong(key->GetVertex(false))] = secondNode;
				}*/
			}
		}

#ifdef DOTNET
		for each(Line2D^ removeLine in removeLines)
		{
			m_dicLineVertex->Remove(removeLine);
		}
#else
		for (std::vector<Line2D*>::iterator iter = removeLines.begin(); iter != removeLines.end(); iter++)
		{
			m_dicLineVertex.erase(*iter);
			delete *iter;
		}
#endif

		POINTER(Node) prevNode = NULL_PTR;
		INSTANCE(STD_VECTOR(POINTER(Node))) linearNodes = dnonlynew STD_VECTOR(POINTER(Node))();

#ifdef DOTNET
		int nVertexCount = linearVertices->Count;
#else
		int nVertexCount = (int)linearVertices.size();
#endif

		for (int i=0;i<nVertexCount;i++)
		{
			REF_CONST(Vertex2D) vPos = linearVertices[i];
			POINTER(Node) node = FindRecycleNode(vPos, recycleNode);

			if (node == NULL_PTR)
			{
				node = FindNode(vPos);

				if (node == NULL_PTR)
				{
					node = geonew Node(vPos);
#ifdef DOTNET
					m_nodes->Add(node);
#else
					m_nodes.push_back(node);
#endif
				}
			}

#ifdef DOTNET
			linearNodes->Add(node);
#else
			linearNodes.push_back(node);
#endif

			if (prevNode != NULL_PTR)
			{
#ifdef DOTNET
				Vertex2D^ prevNodePosition = prevNode->Position;
				Vertex2D^ nodePosition = node->Position;
#else
				Vertex2D& prevNodePosition = prevNode->GetPosition();
				Vertex2D& nodePosition = node->GetPosition();
#endif

				prevNode->AddLink(node);
				node->AddLink(prevNode);

				m_dicLineVertex[geonew Line2D(prevNodePosition, nodePosition)] = STD_PAIR(POINTER(Node), POINTER(Node))(prevNode, node);
			}

			prevNode = node;
		}

#ifdef DOTNET
		for each(KeyValuePair<Line2D^, Vertex2D^>^ pair in dicSplitLines)
		{
			SplitLine(pair->Key, pair->Value, linearNodes);
		}
#else
		for(std::map<Line2D*, Vertex2D>::iterator iter = dicSplitLines.begin(); iter != dicSplitLines.end(); iter++)
		{
			SplitLine(iter->first, iter->second, linearNodes);
		}
#endif
	}
}

#ifdef DOTNET
VertexLink::Node^ VertexLink::FindRecycleNode(Vertex2D^ vPos, Dictionary<NormalString, VertexLink::Node^>^ recycleNodes)
{
	NormalString key = VertexToString(vPos);
	Node^ node;

	if (recycleNodes->TryGetValue(key, node))
		return node;
	
	return nullptr;
}
/*VertexLink::Node^ VertexLink::FindRecycleNode(Vertex2D^ vPos, Dictionary<Vertex2D^, VertexLink::Node^>^ recycleNodes)
{
	for each (KeyValuePair<Vertex2D^, Node^>^ pair in recycleNodes)
	{
		if (IsSameVertex(vPos, pair->Key))
			return pair->Value;
	}

	return nullptr;
}*/
#else
VertexLink::Node* VertexLink::FindRecycleNode(const Vertex2D& vPos, std::map<NormalString, VertexLink::Node*>& recycleNodes)
{
	NormalString key = VertexToString(vPos);
	std::map<NormalString, VertexLink::Node*>::iterator iter = recycleNodes.find(key);

	if (iter != recycleNodes.end())
		return iter->second;

	return 0;
}
/*VertexLink::Node* VertexLink::FindRecycleNode(const Vertex2D& vPos, std::vector<std::pair<Vertex2D, VertexLink::Node*>>& recycleNodes)
{
	for (std::vector<std::pair<Vertex2D, VertexLink::Node*>>::iterator iter = recycleNodes.begin(); iter != recycleNodes.end(); iter++)
	{
		if (IsSameVertex(vPos, iter->first))
			return iter->second;
	}

	return 0;
}*/
#endif

void VertexLink::SplitLine(POINTER(Line2D) line, REF_CONST(Vertex2D) vertex, REF(STD_VECTOR(POINTER(Node))) linearNodes)
{
	STD_PAIR(POINTER(Node), POINTER(Node)) pair;

#ifdef DOTNET
	if (m_dicLineVertex->TryGetValue(line, pair))
#else
	if (FindMap<Line2D*, std::pair<Node*, Node*>>(m_dicLineVertex, line, pair))
#endif
	{
#ifdef DOTNET
		Line2D^ lineBegin = gcnew Line2D(pair.Key->Position, vertex);
		Line2D^ lineEnd = gcnew Line2D(vertex, pair.Value->Position);
#else
		Line2D* lineBegin = new Line2D(pair.first->GetPosition(), vertex);
		Line2D* lineEnd = new Line2D(vertex, pair.second->GetPosition());
#endif
		POINTER(Node) newNode = GetLinearNode(vertex, linearNodes);

		if (newNode != NULL_PTR)
		{
#ifdef DOTNET
			pair.Key->AddLink(newNode);
			pair.Value->AddLink(newNode);
			newNode->AddLink(pair.Key);
			newNode->AddLink(pair.Value);

			m_dicLineVertex[lineBegin] = KeyValuePair<Node^, Node^>(pair.Key, newNode);
			m_dicLineVertex[lineEnd] = KeyValuePair<Node^, Node^>(newNode, pair.Value);

			if (newNode != pair.Key && newNode != pair.Value)
			{
				pair.Key->LinkedNodes->Remove(pair.Value);
				pair.Value->LinkedNodes->Remove(pair.Key);
			}
#else
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
#endif
		}

#ifdef DOTNET
		m_dicLineVertex->Remove(line);
#else
		m_dicLineVertex.erase(line);
#endif
	}
}

POINTER(VertexLink::Node) VertexLink::GetLinearNode(REF_CONST(Vertex2D) vertex, REF(STD_VECTOR(POINTER(VertexLink::Node))) linearNodes)
{
#ifdef DOTNET
	for each (Node^ node in linearNodes)
	{
		Vertex2D^ nodePosition = node->Position;
#else
	for (std::vector<Node*>::iterator iter = linearNodes.begin(); iter != linearNodes.end(); iter++)
	{
		Node* node = *iter;
		Vertex2D& nodePosition = node->GetPosition();
#endif
		if (IsSameVertex(vertex, nodePosition))
		{
			return node;
		}
	}

	return NULL_PTR;
}

// vNew보다 앞에 있는 Vertex들을 모두 지우고, vNew를 시작점으로 한다.
void VertexLink::SetLinearVertexBegin(REF(STD_VECTOR(INSTANCE(Vertex2D))) linearVertices, REF_CONST(Vertex2D) vNew)
{
#ifdef DOTNET
	int nVertexCount = linearVertices->Count;
#else
	int nVertexCount = linearVertices.size();
#endif

	double dPrevLen = 0.0;

	for (int i = 0; i < nVertexCount; i++)
	{
		REF(Vertex2D) vertex = linearVertices[i];

		double dLen = OF(vertex, GetDistance(vNew));

		if (IsSame(dLen))
		{
			for (int j = 0; j < i; j++)
			{
#ifdef DOTNET
				linearVertices->RemoveAt(0);
#else
				linearVertices.erase(linearVertices.begin());
#endif
			}

			return;
		}

		if (i == 0)
			dPrevLen = dLen;
		else
		{
			double dLen2 = OF(vertex, GetDistance(linearVertices[i - 1]));

			if (dPrevLen < dLen2)
			{
#ifdef DOTNET
				for (int j = 0; j < i; j++)
					linearVertices->RemoveAt(0);

				linearVertices->Insert(0, vNew);
#else
				for (int j = 0; j < i; j++)
					linearVertices.erase(linearVertices.begin());

				linearVertices.insert(linearVertices.begin(), vNew);
#endif
				return;
			}
			else
				dPrevLen = dLen;
		}
	}
}

// vNew보다 뒤에 있는 Vertex들을 모두 지우고, vNew를 끝점으로 한다.
void VertexLink::SetLinearVertexEnd(REF(STD_VECTOR(INSTANCE(Vertex2D))) linearVertices, REF_CONST(Vertex2D) vNew)
{
#ifdef DOTNET
	int nVertexCount = linearVertices->Count;
#else
	int nVertexCount = linearVertices.size();
#endif
	double dPrevLen = 0.0;

	for (int i = nVertexCount - 1; i >= 0; i--)
	{
		REF(Vertex2D) vertex = linearVertices[i];

		double dLen = OF(vertex, GetDistance(vNew));

		if (IsSame(dLen))
		{
			for (int j = i + 1; j < nVertexCount; j++)
			{
#ifdef DOTNET
				linearVertices->RemoveAt(i + 1);
#else
				linearVertices.erase(linearVertices.begin() + i + 1);
#endif
			}

			return;
		}

		if (i == nVertexCount - 1)
			dPrevLen = dLen;
		else
		{
			double dLen2 = OF(vertex, GetDistance(linearVertices[i + 1]));

			if (dPrevLen < dLen2)
			{
#ifdef DOTNET
				for (int j = i + 1; j < nVertexCount; j++)
					linearVertices->RemoveAt(i + 1);

				linearVertices->Add(vNew);
#else
				for (int j = i + 1; j < nVertexCount; j++)
					linearVertices.erase(linearVertices.begin() + i + 1);

				linearVertices.push_back(vNew);
#endif
				return;
			}
			else
				dPrevLen = dLen;
		}
	}
}

// vNew를 linearVertices에 시작점과 가까운 순으로 정렬하여 삽입한다.
void VertexLink::AddLinearVertex(REF(STD_VECTOR(INSTANCE(Vertex2D))) linearVertices, REF_CONST(Vertex2D) vNew)
{
#ifdef DOTNET
	int nVertexCount = linearVertices->Count;
#else
	int nVertexCount = linearVertices.size();
#endif
	double dPrevLen = 0.0;

	for (int i = 0; i < nVertexCount; i++)
	{
		REF(Vertex2D) vertex = linearVertices[i];

		double dLen = OF(vertex, GetDistance(vNew));

		if (IsSame(dLen))
			return;

		if (i == 0)
			dPrevLen = dLen;
		else
		{
			double dLen2 = OF(vertex, GetDistance(linearVertices[i - 1]));

			if (dPrevLen < dLen2)
			{
#ifdef DOTNET
				linearVertices->Insert(i, vNew);
#else
				linearVertices.insert(linearVertices.begin() + i, vNew);
#endif
				return;
			}
			else
				dPrevLen = dLen;
		}
	}
}

bool VertexLink::IsSameVertex(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2)
{
	double len = OF(v1, GetDistance(v2));
	return IsSame(len);
}

bool VertexLink::IsSame(double len)
{
	return len < 0.1;
	//return len < Math::HALF_TOLERANCE();
}

POINTER(VertexLink::Node) VertexLink::FindNode(REF_CONST(Vertex2D) vPos)
{
#ifdef DOTNET
	for each (Node^ node in m_nodes)
	{
		if (node->Position->GetDistance(vPos) < 0.1)
		//if (node->Position->GetDistance(vPos) < Math::HALF_TOLERANCE())
			return node;
	}
#else
	for (std::vector<Node*>::iterator iter = m_nodes.begin();iter != m_nodes.end(); iter++)
	{
		Node* node = *iter;

		if (node->GetPosition().GetDistance(vPos) < Math::HALF_TOLERANCE())
			return node;
	}
#endif

	return NULL_PTR;
}

void VertexLink::AddLineNode(POINTER(Line2D) line)
{
	POINTER(Node) begin = geonew Node(line->GetVertex(true));
	POINTER(Node) end = geonew Node(line->GetVertex(false));

#ifdef DOTNET
	begin->LinkedNodes->Add(end);
	end->LinkedNodes->Add(begin);

	m_nodes->Add(begin);
	m_nodes->Add(end);
	m_dicLineVertex[line] = KeyValuePair<Node^, Node^>(begin, end);
#else
	begin->GetLinkedNodes().push_back(end);
	end->GetLinkedNodes().push_back(begin);

	m_nodes.push_back(begin);
	m_nodes.push_back(end);

	Line2D* newLine = new Line2D(line->GetVertex(true), line->GetVertex(false));
	m_dicLineVertex[newLine] = std::pair<Node*, Node*>(begin, end);
#endif
}

#ifdef DOTNET
List<Line2D^>^ VertexLink::GetLines()
{
	List<Line2D^>^ lines = gcnew List<Line2D^>();

	for each (KeyValuePair<Line2D^, KeyValuePair<Node^, Node^>>^ pair in m_dicLineVertex)
	{
		lines->Add(pair->Key);
	}

	return lines;
	//return m_dicLineVertex->Keys->ToList();
}
#else
int VertexLink::GetLines(std::vector<Line2D>& lines)
{
	for (std::map< Line2D*, std::pair<Node*, Node*>>::iterator iter = m_dicLineVertex.begin();iter != m_dicLineVertex.end();iter++)
	{
		lines.push_back(*iter->first);
	}

	return (int)lines.size();
}
#endif

// 다른 노드와 연결되지 않은 노드들을 모두 없앤다.
void VertexLink::RemoveSingleNodes()
{
	INSTANCE(STD_VECTOR(POINTER(Node))) singleNodes = dnonlynew STD_VECTOR(POINTER(Node))();

#ifdef DOTNET
	for each (Node^ node in m_nodes)
	{
		if (node->LinkedNodes->Count <= 1)
			singleNodes->Add(node);
	}

	for each (Node^ node in singleNodes)
	{
		m_nodes->Remove(node);
		RemoveSingleNode(node);
	}
#else
	for (std::vector<Node*>::iterator iter = m_nodes.begin(); iter != m_nodes.end(); iter++)
	{
		Node* node = *iter;

		if ((int)node->GetLinkedNodes().size() <= 1)
			singleNodes.push_back(node);
	}

	for (std::vector<Node*>::iterator iter = singleNodes.begin();iter != singleNodes.end(); iter++)
	{
		m_nodes.erase(std::find(m_nodes.begin(), m_nodes.end(), *iter));
		RemoveSingleNode(*iter);
	}
#endif
}

#ifndef DOTNET
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
#endif

void VertexLink::RemoveSingleNode(POINTER(Node) node)
{
#ifdef DOTNET
	for (int i=0;i<node->LinkedNodes->Count;i++)
	//for each (Node^ link in node->LinkedNodes)
	{
		Node^ link = node->LinkedNodes[i];

		link->LinkedNodes->Remove(node);

		if (link->LinkedNodes->Count <= 1)
		{
			m_nodes->Remove(link);
			RemoveSingleNode(link);
		}
	}

	node->LinkedNodes->Clear();
#else
	for (int i=0;i<(int)node->GetLinkedNodes().size();i++)
	//for (std::vector<Node*>::iterator iter = node->GetLinkedNodes().begin(); iter != node->GetLinkedNodes().end();iter++)
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
#endif
}

END_NS
END_NS
