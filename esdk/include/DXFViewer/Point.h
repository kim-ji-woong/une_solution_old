#pragma once
#include "Shape.h"

namespace DXFViewer
{
	ref class UnE::Geometry::Vertex2D;

	public ref class Point : Shape
	{
	public:
		Point(void);
		virtual ~Point(void);
		Point(Point^ rhs);
		Point(double x, double y);

	public:
		virtual bool Draw(System::Drawing::Graphics^ g, bool bDrawText) override;
		// (x,y)만큼 객체를 옮긴다.
		virtual void Move(double x, double y) override;
		virtual ShapeType GetShapeType() override;
		virtual Shape^ Clone() override;
		// Selectable이 false이면 HitTest 검사가 무조건 실패한다.
		virtual bool HitTest(double x, double y) override;
		virtual bool CheckClipBounds(System::Drawing::Graphics^ g, UnE::Geometry::Vertex2D^ vClipTL, UnE::Geometry::Vertex2D^ vClipBR) override;

	public:
		property UnE::Geometry::Vertex2D^ Position
		{
			virtual UnE::Geometry::Vertex2D^ get() override
			{
				return m_vertex;
			}
		}

		virtual property UnE::Geometry::Vertex2D^ BoundaryTL
		{
			 UnE::Geometry::Vertex2D^ get() override
			{
				return m_vertex;
			}
		}

		virtual property UnE::Geometry::Vertex2D^ BoundaryBR
		{
			UnE::Geometry::Vertex2D^ get() override
			{
				return m_vertex;
			}
		}

	public:
		property UnE::Geometry::Vertex2D^ Vertex
		{
			UnE::Geometry::Vertex2D^ get() { return m_vertex; }
			void set(UnE::Geometry::Vertex2D^ value)
			{
				m_vertex = value; 
				m_point->X = (float)m_vertex->x;
				m_point->Y = (float)m_vertex->y;
			}
		}

	protected:
		bool DrawGDI(System::Drawing::Graphics^ g);
		bool DrawGL();

	private:
		UnE::Geometry::Vertex2D^ m_vertex;
		UnE::Geometry::Vertex2F^ m_vSelected;

		// for GDI
		System::Drawing::PointF^ m_point;
		// for OpenGL
		//float* m_arrPointGL;
	};
}
