#pragma once
#include "Shape.h"
#include "Vertex2D.h"
#include <list>

namespace VectorGraphics
{
	class __declspec(dllexport) EArc : public Shape
	{
	public:
		EArc();
		EArc(const Vertex2D& vTL, const Vertex2D& vBL, const Vertex2D& vBR, double dBeginAngle, double dEArcAngle, bool isClockwise);
		virtual ~EArc();

	public:
		void Draw();

	public:
		void SetEArc(const Vertex2D& vTL, const Vertex2D& vBL, const Vertex2D& vBR, double dBeginAngle, double dEArcAngle, bool isClockwise);
		const std::list<Vertex2D>& GetVertices();

	private:
		Vertex2D m_vTL, m_vBL, m_vBR;
		double m_dBeginAngle, m_dEArcAngle;
		bool m_isClockwise;
		std::list<Vertex2D> m_vertices;
	};
}
