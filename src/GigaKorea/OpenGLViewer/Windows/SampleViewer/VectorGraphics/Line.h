#pragma once
#include "Shape.h"
#include "Vertex2D.h"

namespace VectorGraphics
{
	class __declspec(dllexport) Line : public Shape
	{
	public:
		Line();
		Line(const Vertex2D& vBegin, const Vertex2D& vEnd);
		virtual ~Line();

	public:
		void Draw();

	public:
		void SetVertex(const Vertex2D& vertex, bool isBegin);
		const Vertex2D& GetVertex(bool isBegin);

	private:
		Vertex2D m_vBegin, m_vEnd;
	};
}
