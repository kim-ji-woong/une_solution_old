#pragma once
#include "Shape.h"

namespace DXFViewer
{
	ref class UnE::Geometry::Vertex2D;

	public ref class Line : Shape
	{
	public:
		Line(void);
		virtual ~Line(void);
		Line(Line^ rhs);
		Line(UnE::Geometry::Vertex2D^ vBegin, UnE::Geometry::Vertex2D^ vEnd);

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

	public:
		property UnE::Geometry::Vertex2D^ Begin
		{
			UnE::Geometry::Vertex2D^ get() { return m_vBegin; }
			void set(UnE::Geometry::Vertex2D^ value)
			{
				m_vBegin = value; 
				m_arrPoint[0].X = /*m_arrPointGL[0] = */(float)m_vBegin->x;
				m_arrPoint[0].Y = /*m_arrPointGL[1] = */(float)m_vBegin->y;
			}
		}

		property UnE::Geometry::Vertex2D^ End
		{
			UnE::Geometry::Vertex2D^ get() { return m_vEnd; }
			void set(UnE::Geometry::Vertex2D^ value)
			{
				m_vEnd = value; 
				m_arrPoint[1].X = /*m_arrPointGL[2] = */(float)m_vEnd->x;
				m_arrPoint[1].Y = /*m_arrPointGL[3] = */(float)m_vEnd->y;
			}
		}

	protected:
		bool DrawGDI(System::Drawing::Graphics^ g);
		bool DrawGL();
		void DrawLines(System::Drawing::Pen^ pen, System::Drawing::Graphics^ g);

	private:
		UnE::Geometry::Vertex2D^ m_vBegin;
		UnE::Geometry::Vertex2D^ m_vEnd;
		UnE::Geometry::Vertex2F^ m_vSelectedBegin;
		UnE::Geometry::Vertex2F^ m_vSelectedEnd;

		// for GDI
		array<System::Drawing::PointF>^ m_arrPoint;
		// for OpenGL
		//float* m_arrPointGL;
	};
}
