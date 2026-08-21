#pragma once
#include "Shape.h"

namespace DXFDotNet
{
	ref class UnE::Geometry::Vertex2D;

	public ref class PolyLine abstract : Shape
	{
	public:
		PolyLine(void);
		virtual ~PolyLine(void);

		virtual PolyLine^ CreatePolyLine() = 0;
		
		//virtual bool Draw(System::Drawing::Graphics^ g, bool bDrawText) override = 0;
		// (x,y)만큼 객체를 옮긴다.
		virtual void Move(double x, double y) override;
		virtual ShapeType GetShapeType() override;
		virtual Shape^ Clone() override;
		// Selectable이 false이면 HitTest 검사가 무조건 실패한다.
		virtual bool HitTest(double x, double y) override;

		property UnE::Geometry::Vertex2D^ Position
		{
			virtual UnE::Geometry::Vertex2D^ get() override
			{
				return (m_vTL + m_vBR) / 2;
			}
		}

		virtual property UnE::Geometry::Vertex2D^ BoundaryTL
		{
			UnE::Geometry::Vertex2D^ get() override
			{
				return m_vTL;
			}
		}

		virtual property UnE::Geometry::Vertex2D^ BoundaryBR
		{
			UnE::Geometry::Vertex2D^ get() override
			{
				return m_vBR;
			}
		}

		property bool IsClosed
		{
			bool get() { return m_isClosed; }
		}

	public:
		void SetVertex(System::Collections::ArrayList^ arrVertices);
		bool UpdatePoint(int nIndex, float x, float y);
		void SetPointSize(int nPointCount);

		int GetVertexSize();
		System::Drawing::PointF GetVertex(int nIndex);

		UnE::Geometry::Polygon^ GetPolygon();

	protected:
		void ResetSelectedVertex(int nPointCount);
		void CheckClosed(int nPointCount);

	protected:
		// for GDI
		array<System::Drawing::PointF>^ m_arrPoint;
		// HitTest를 위한 Polygon
		UnE::Geometry::Polygon^ m_polygon;

		// 영역
		UnE::Geometry::Vertex2D^ m_vTL;
		UnE::Geometry::Vertex2D^ m_vBR;
		bool m_isInitArea;

		UnE::Geometry::Vertex2F^ m_vSelectedBegin;
		UnE::Geometry::Vertex2F^ m_vSelectedEnd;

		bool m_isClosed;

		System::Drawing::SolidBrush^ m_brushPolygon;
	};
}
