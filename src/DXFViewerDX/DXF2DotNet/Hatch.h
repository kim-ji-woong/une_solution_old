#pragma once
#include "Shape.h"

namespace DXFDotNet
{
	ref class UnE::Geometry::Vertex2D;

	public ref class Hatch abstract : Shape
	{
	public:
		Hatch(void);
		virtual ~Hatch(void);
		virtual Hatch^ CreateHatch(void) = 0;

	public:
		//virtual bool Draw(System::Drawing::Graphics^ g, bool bDrawText) override = 0;
		// (x,y)만큼 객체를 옮긴다.
		virtual void Move(double x, double y) override;
		virtual ShapeType GetShapeType() override;
		virtual Shape^ Clone() override;
		// Selectable이 false이면 HitTest 검사가 무조건 실패한다.
		virtual bool HitTest(double x, double y) override;
		
	public:
		virtual void SetVertex(System::Collections::ArrayList^ arrVertices);
		virtual bool UpdatePoint(int nIndex, float x, float y);
		virtual bool UpdatePoint(bool bRefresh){ return true; };

		void SetPointSize(int nPointCount);
		int GetPointSize();
		bool GetPoint(int nIndex, [System::Runtime::InteropServices::OutAttribute] float% x, [System::Runtime::InteropServices::OutAttribute] float% y);


	public:
		property UnE::Geometry::Vertex2D^ Position
		{
			virtual UnE::Geometry::Vertex2D^ get() override
			{
				return gcnew UnE::Geometry::Vertex2D(m_ptCenter.X, m_ptCenter.Y);
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

		property System::Drawing::PointF Center
		{
			System::Drawing::PointF get() { return m_ptCenter; }
			void set(System::Drawing::PointF value) { m_ptCenter = value; }
		}

		property UnE::Geometry::Polygon^ Polygon
		{
			UnE::Geometry::Polygon^ get() { return m_polygon; }
		}

	protected:
		array<System::Drawing::PointF>^ m_arrPoint;
		// HitTest를 위한 Polygon
		UnE::Geometry::Polygon^ m_polygon;
		System::Drawing::SolidBrush^ m_brush;
		System::Drawing::PointF m_ptCenter;

		// 영역
		UnE::Geometry::Vertex2D^ m_vTL;
		UnE::Geometry::Vertex2D^ m_vBR;
		bool m_isInitArea;

	};
}
