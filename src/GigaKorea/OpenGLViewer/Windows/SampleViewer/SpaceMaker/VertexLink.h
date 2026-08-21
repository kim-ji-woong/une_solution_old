#pragma once
#include "Vertex2D.h"
#include <vector>
#include <map>

namespace SpaceMaker
{
	class Line2D;

	class VertexLink
	{
	public:
		class Node
		{
		public:
			Node();
			Node(const VectorGraphics::Vertex2D& vPos);

		public:
			void AddLink(Node* node);

			VectorGraphics::Vertex2D& GetPosition()
			{
				return m_pos;
			}

			void SetPosition(const VectorGraphics::Vertex2D& rPos)
			{
				m_pos = rPos;
			}

			std::vector<Node*>& GetLinkedNodes()
			{
				return m_linkedNodes;
			}

		private:
			VectorGraphics::Vertex2D m_pos;
			std::vector<Node*> m_linkedNodes;
		};

	public:
		VertexLink(void);
		virtual ~VertexLink(void);

	public:
		std::vector<Node*>& Nodes()
		{
			return m_nodes;
		}

	public:
		void AddLine(Line2D* line);
		// 다른 노드와 연결되지 않은 노드들을 모두 없앤다.
		void RemoveSingleNodes();
		int GetLines(std::vector<Line2D>& lines);

	private:
		bool IsSameVertex(const VectorGraphics::Vertex2D& v1, const VectorGraphics::Vertex2D& v2);
		bool IsSame(double len);
		void AddLineNode(Line2D* line);
		void RemoveSingleNode(Node* node);
		void SplitLine(Line2D* line, const VectorGraphics::Vertex2D& vertex, std::vector<Node*>& linearNodes);
		Node* GetLinearNode(const VectorGraphics::Vertex2D& vertex, std::vector<Node*>& linearNodes);
		// vNew보다 앞에 있는 Vertex들을 모두 지우고, vNew를 시작점으로 한다.
		void SetLinearVertexBegin(std::vector<VectorGraphics::Vertex2D>& linearVertices, const VectorGraphics::Vertex2D& vNew);
		// vNew보다 뒤에 있는 Vertex들을 모두 지우고, vNew를 끝점으로 한다.
		void SetLinearVertexEnd(std::vector<VectorGraphics::Vertex2D>& linearVertices, const VectorGraphics::Vertex2D& vNew);
		// vNew를 linearVertices에 시작점과 가까운 순으로 정렬하여 삽입한다.
		void AddLinearVertex(std::vector<VectorGraphics::Vertex2D>& linearVertices, const VectorGraphics::Vertex2D& vNew);
		Node* FindNode(const VectorGraphics::Vertex2D& vPos);

		Node* FindRecycleNode(const VectorGraphics::Vertex2D& vPos, std::map<__int64, Node*>& recycleNodes);

	private:
		std::vector<Node*> m_nodes;
		std::map<Line2D*, std::pair<Node*, Node*>> m_dicLineVertex;
	};
}
