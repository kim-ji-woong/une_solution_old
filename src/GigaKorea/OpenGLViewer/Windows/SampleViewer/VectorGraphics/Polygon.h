#pragma once
#include "Shape.h"
#include "Vertex2D.h"
#include <list>

namespace VectorGraphics
{
	class __declspec(dllexport) Polygon : public Shape
	{
	public:
		enum DrawMode { Boundary = 0, Fill };

	public:
		Polygon();
		virtual ~Polygon();

	public:
		void Draw();
		bool HitTest(const Vertex2D& vPos);
		bool HitTestIfNotPOI(const Vertex2D& vPos);

	public:
		void AddVertex(const Vertex2D& rVertex);
		int GetVertexCount();
		bool GetVertex(int nIndex, Vertex2D* pVertex);
		void RemoveAt(int nIndex);
		void Clear();

		void SetDrawingMode(DrawMode mode);
		DrawMode GetDrawingMode();

		void Done();

	private:
		std::list<Vertex2D> m_vertices;
		unsigned int* m_arrIndices;
		float* m_arrCoords;
		int m_nIndexCount, m_nVertexCount;
		DrawMode m_mode;
	};
}
