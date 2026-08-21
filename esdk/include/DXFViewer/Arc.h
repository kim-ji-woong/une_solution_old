#pragma once
#include "Shape.h"

// 3차원 공간에서 원을 그리기 위해서는 원이 지나는 세 점이 필요하다.
// 그러나, CAD에서는 이러한 방식으로 3차원 공간상에 원을 그리는 방식을
// 지원하지 않고, 원이 존재하는 평면을 가상의 XY 평면(OCS 좌표계)로 놓은 다음
// 그 평면에서의 좌표값을 사용하여 원을 그린다.(중점과 반지름)

// DXF에는 Arc의 BeginAngle과 ArcAngle만 존재하며, Arc의 방향은 기입하지 않는다.
// Arc는 항상 시계 방향으로만 그려진다.

namespace DXFViewer
{
	ref class EditBox;

	public ref class Arc : Shape
	{
	public:
		Arc(void);
		virtual ~Arc(void);
		
	public:
		virtual bool Draw(System::Drawing::Graphics^ g, bool bDrawText) override;
		// (x,y)만큼 객체를 옮긴다.
		virtual void Move(double x, double y) override;
		virtual ShapeType GetShapeType() override;
		virtual Shape^ Clone() override;
		// Selectable이 false이면 HitTest 검사가 무조건 실패한다.
		virtual bool HitTest(double x, double y) override;
		virtual bool CheckClipBounds(System::Drawing::Graphics^ g, UnE::Geometry::Vertex2D^ vClipTL, UnE::Geometry::Vertex2D^ vClipBR) override;

	protected:
		bool DrawGDI(System::Drawing::Graphics^ g);
		bool DrawGL();

	public:
		property UnE::Geometry::Vertex2D^ Position
		{
			virtual UnE::Geometry::Vertex2D^ get() override { return m_vCenter; }
		}

		virtual property UnE::Geometry::Vertex2D^ BoundaryTL
		{
			UnE::Geometry::Vertex2D^ get() override
			{
				double x = m_vCenter->x - m_dRadius;
				double y = m_vCenter->y + m_dRadius;
				return gcnew UnE::Geometry::Vertex2D(x, y);
			}
		}

		virtual property UnE::Geometry::Vertex2D^ BoundaryBR
		{
			UnE::Geometry::Vertex2D^ get() override
			{
				double x = m_vCenter->x + m_dRadius;
				double y = m_vCenter->y - m_dRadius;
				return gcnew UnE::Geometry::Vertex2D(x, y);
			}
		}

	public:
		// Degree
		property double BeginAngle
		{
			double get() { return m_dBeginAngle; }
			void set(double value) { m_dBeginAngle = value; }
		}

		// Degree
		property double ArcAngle
		{
			double get() { return m_dArcAngle; }
			void set(double value) { m_dArcAngle = value; }
		}

		property double Radius
		{
			double get() { return m_dRadius; }
			void set(double value) { m_dRadius = value; }
		}

		property UnE::Geometry::Vertex2D^ Center
		{
			UnE::Geometry::Vertex2D^ get() { return m_vCenter; }
			void set(UnE::Geometry::Vertex2D^ value) { m_vCenter = value; }
		}

		property bool IsCircle
		{
			bool get() { return m_isCircle; }
			void set(bool value) { m_isCircle = value; }
		}

	private:
		void Draw(System::Drawing::Graphics^ g, System::Drawing::Pen^ pen);

	private:
		UnE::Geometry::Vertex2D^ m_vCenter;
		// Degree
		double m_dBeginAngle, m_dArcAngle;
		double m_dRadius;
		bool m_isCircle;
	};
}
