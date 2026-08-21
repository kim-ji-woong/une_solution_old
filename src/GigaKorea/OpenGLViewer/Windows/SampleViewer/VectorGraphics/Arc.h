#pragma once
#include "Shape.h"
#include "Vertex2D.h"
#include <list>

namespace VectorGraphics
{
	class __declspec(dllexport) Arc : public Shape
	{
	public:
		Arc();
		Arc(const Vertex2D& vCenter, double dRadius, double dBeginAngle, double dArcAngle, bool isClockwise);
		virtual ~Arc();

	public:
		void Draw();

	public:
		void SetArc(const Vertex2D& vCenter, double dRadius, double dBeginAngle, double dArcAngle, bool isClockwise);
		const std::list<Vertex2D>& GetVertices();

	private:
		Vertex2D m_vCenter;
		double m_dRadius, m_dBeginAngle, m_dArcAngle;
		bool m_isClockwise;
		std::list<Vertex2D> m_vertices;
	};
}
