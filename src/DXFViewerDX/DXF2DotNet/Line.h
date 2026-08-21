#pragma once
#include "Shape.h"

namespace DXFDotNet
{
	ref class UnE::Geometry::Vertex2D;

	public ref class Line abstract : Shape
	{
	public:
		Line(void);
		virtual ~Line(void);
		Line(Line^ rhs);
		Line(UnE::Geometry::Vertex2D^ vBegin, UnE::Geometry::Vertex2D^ vEnd);


		virtual Line^ CreateLine() = 0;
		//virtual bool Draw(System::Drawing::Graphics^ g, bool bDrawText) override = 0;

	public:
		
		// (x,y)만큼 객체를 옮긴다.
		virtual void Move(double x, double y) override;
		virtual ShapeType GetShapeType() override;
		virtual Shape^ Clone() override;
		// Selectable이 false이면 HitTest 검사가 무조건 실패한다.
		virtual bool HitTest(double x, double y) override;
		
	public:
		property UnE::Geometry::Vertex2D^ Position
		{
			virtual UnE::Geometry::Vertex2D^ get() override
			{
				return (m_vBegin + m_vEnd) / 2;
			}
		}

		virtual property UnE::Geometry::Vertex2D^ BoundaryTL
		{
			 UnE::Geometry::Vertex2D^ get() override
			{
				double x = m_vBegin->x;
				if (x > m_vEnd->x)
					x = m_vEnd->x;

				double y = m_vEnd->y;
				if (x < m_vEnd->y)
					y = m_vEnd->y;

				return gcnew UnE::Geometry::Vertex2D(x, y);
			}
		}

		virtual property UnE::Geometry::Vertex2D^ BoundaryBR
		{
			UnE::Geometry::Vertex2D^ get() override
			{
				double x = m_vBegin->x;
				if (x < m_vEnd->x)
					x = m_vEnd->x;

				double y = m_vEnd->y;
				if (y > m_vEnd->y)
					y = m_vEnd->y;

				return gcnew UnE::Geometry::Vertex2D(x, y);
			}
		}

		property UnE::Geometry::Vertex2D^ Begin
		{
			UnE::Geometry::Vertex2D^ get() { return m_vBegin; }
			void set(UnE::Geometry::Vertex2D^ value)
			{
				m_vBegin = value; 
			}
		}

		property UnE::Geometry::Vertex2D^ End
		{
			UnE::Geometry::Vertex2D^ get() { return m_vEnd; }
			void set(UnE::Geometry::Vertex2D^ value)
			{
				m_vEnd = value; 
			}
		}

	protected:
		UnE::Geometry::Vertex2D^ m_vBegin;
		UnE::Geometry::Vertex2D^ m_vEnd;
		UnE::Geometry::Vertex2F^ m_vSelectedBegin;
		UnE::Geometry::Vertex2F^ m_vSelectedEnd;

		// for GDI
		array<System::Drawing::PointF>^ m_arrPoint;
	
	};
}
