#pragma once
#include "Shape.h"
#include "Vertex2D.h"
#include <list>

namespace VectorGraphics
{
	class __declspec(dllexport) Polyline : public Shape
	{
	public:
		Polyline();
		virtual ~Polyline();

	public:
		void Draw();

	public:
		void AddVertex(const Vertex2D& rVertex);
		int GetVertexCount();
		bool GetVertex(int nIndex, Vertex2D* pVertex);
		void RemoveAt(int nIndex);
		void Clear();
		bool IsClosed();
		void SetClosed(bool closed);

	private:
		std::list<Vertex2D> m_vertices;
		bool m_isClosed;
	};
}
