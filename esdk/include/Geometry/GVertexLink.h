#pragma once
#include "GeometryAPI.h"
#include "GVertex.h"

#ifndef DOTNET
#include <vector>
#include <map>
#endif

namespace UnE
{
	namespace Geometry
	{
		_DECLARE_CLASS(Line2D);

		GEOMETRY_EXPORT_CLASS(VertexLink)
		{
		public:
#ifdef DOTNET
			ref class Node
#else
			class Node
#endif
			{
			public:
				Node();
				Node(REF_CONST(Vertex2D) vPos);

			public:
				void AddLink(POINTER(Node) node);

#ifdef DOTNET
				property Vertex2D^ Position
				{
					Vertex2D^ get() { return m_pos; }
					void set(Vertex2D^ value) { m_pos = value; }
				}

				property System::Collections::Generic::List<Node^>^ LinkedNodes
				{
					System::Collections::Generic::List<Node^>^ get() { return m_linkedNodes; }
				}
#else
				Vertex2D& GetPosition()
				{
					return m_pos;
				}

				void SetPosition(const Vertex2D& rPos)
				{
					m_pos = rPos;
				}

				std::vector<Node*>& GetLinkedNodes()
				{
					return m_linkedNodes;
				}
#endif

			private:
				INSTANCE(Vertex2D) m_pos;
				INSTANCE(STD_VECTOR(POINTER(Node))) m_linkedNodes;
			};

		public:
			VertexLink(void);
			virtual ~VertexLink(void);

		public:
#ifdef DOTNET
			property System::Collections::Generic::List<Node^>^ Nodes
			{
				System::Collections::Generic::List<Node^>^ get()
				{
					return m_nodes;
				}
			}
#else
			std::vector<Node*>& Nodes()
			{
				return m_nodes;
			}
#endif

		public:
			void AddLine(POINTER(Line2D) line);
			// 다른 노드와 연결되지 않은 노드들을 모두 없앤다.
			void RemoveSingleNodes();

#ifdef DOTNET
			System::Collections::Generic::List<Line2D^>^ GetLines();
#else
			int GetLines(std::vector<Line2D>& lines);
#endif

		private:
			bool IsSameVertex(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2);
			bool IsSame(double len);
			void AddLineNode(POINTER(Line2D) line);
			void RemoveSingleNode(POINTER(Node) node);
			void SplitLine(POINTER(Line2D) line, REF_CONST(Vertex2D) vertex, REF(STD_VECTOR(POINTER(Node))) linearNodes);
			POINTER(Node) GetLinearNode(REF_CONST(Vertex2D) vertex, REF(STD_VECTOR(POINTER(Node))) linearNodes);
			// vNew보다 앞에 있는 Vertex들을 모두 지우고, vNew를 시작점으로 한다.
			void SetLinearVertexBegin(REF(STD_VECTOR(INSTANCE(Vertex2D))) linearVertices, REF_CONST(Vertex2D) vNew);
			// vNew보다 뒤에 있는 Vertex들을 모두 지우고, vNew를 끝점으로 한다.
			void SetLinearVertexEnd(REF(STD_VECTOR(INSTANCE(Vertex2D))) linearVertices, REF_CONST(Vertex2D) vNew);
			// vNew를 linearVertices에 시작점과 가까운 순으로 정렬하여 삽입한다.
			void AddLinearVertex(REF(STD_VECTOR(INSTANCE(Vertex2D))) linearVertices, REF_CONST(Vertex2D) vNew);
			POINTER(Node) FindNode(REF_CONST(Vertex2D) vPos);

#ifdef DOTNET
			Node^ FindRecycleNode(Vertex2D^ vPos, System::Collections::Generic::Dictionary<NormalString, Node^>^ recycleNodes);
			//Node^ FindRecycleNode(Vertex2D^ vPos, System::Collections::Generic::Dictionary<Vertex2D^, Node^>^ recycleNodes);
#else
			Node* FindRecycleNode(const Vertex2D& vPos, std::map<NormalString, Node*>& recycleNodes);
			//Node* FindRecycleNode(const Vertex2D& vPos, std::vector<std::pair<Vertex2D, Node*>>& recycleNodes);
#endif

		private:
			INSTANCE(STD_VECTOR(POINTER(Node))) m_nodes;

#ifdef DOTNET
			System::Collections::Generic::Dictionary<Line2D^, System::Collections::Generic::KeyValuePair<Node^, Node^>>^ m_dicLineVertex = gcnew System::Collections::Generic::Dictionary<Line2D^, System::Collections::Generic::KeyValuePair<Node^, Node^>>();
#else
			std::map<Line2D*, std::pair<Node*, Node*>> m_dicLineVertex;
#endif
		};
	}
}
