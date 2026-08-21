#pragma once
#include "Shape.h"

// DXF에는 EArc의 BeginAngle과 EArcAngle만 존재하며, EArc의 방향은 기입하지 않는다.
// EArc는 항상 시계 방향으로만 그려진다.

namespace DXFDotNet
{
	public ref class EArc abstract : Shape
	{
	public:
		EArc(void);
		virtual ~EArc(void);
		
		// 내부에서 생성용
		virtual EArc^ CreateEArc(void) = 0;
	public:
		//virtual bool Draw(System::Drawing::Graphics^ g, bool bDrawText) override = 0;
		// (x,y)만큼 객체를 옮긴다.
		virtual void Move(double x, double y) override;
		virtual ShapeType GetShapeType() override;
		virtual Shape^ Clone() override;
		// Selectable이 false이면 HitTest 검사가 무조건 실패한다.
		virtual bool HitTest(double x, double y) override;
		

	public:
		property UnE::Geometry::Vertex2D^ Position
		{
			virtual UnE::Geometry::Vertex2D^ get() override { return m_vTL; }
		}

		virtual property UnE::Geometry::Vertex2D^ BoundaryTL
		{
			UnE::Geometry::Vertex2D^ get() override
			{
				if (m_vTL == nullptr || m_vBL == nullptr || m_vBR == nullptr)
					return nullptr;

				double x = m_vTL->x;

				if (x > m_vBL->x)
					x = m_vBL->x;
				if (x > m_vBR->x)
					x = m_vBR->x;

				double y = m_vTL->y;

				if (y < m_vBL->y)
					y = m_vBL->y;
				if (y < m_vBR->y)
					y = m_vBR->y;

				return gcnew UnE::Geometry::Vertex2D(x, y);
			}
		}

		virtual property UnE::Geometry::Vertex2D^ BoundaryBR
		{
			UnE::Geometry::Vertex2D^ get() override
			{
				if (m_vTL == nullptr || m_vBL == nullptr || m_vBR == nullptr)
					return nullptr;

				double x = m_vTL->x;

				if (x < m_vBL->x)
					x = m_vBL->x;
				if (x < m_vBR->x)
					x = m_vBR->x;

				double y = m_vTL->y;

				if (y > m_vBL->y)
					y = m_vBL->y;
				if (y > m_vBR->y)
					y = m_vBR->y;

				return gcnew UnE::Geometry::Vertex2D(x, y);
			}
		}

	public:
		property double Width
		{
			double get() { return m_dWidth; }
			void set(double value) { m_dWidth = value; }
		}

		property double Height
		{
			double get() { return m_dHeight; }
			void set(double value) { m_dHeight = value; }
		}

		// 타원을 이루는 사각영역의 왼쪽 상단 모서리
		property UnE::Geometry::Vertex2D^ TopLeft
		{
			UnE::Geometry::Vertex2D^ get() { return m_vTL; }
			void set(UnE::Geometry::Vertex2D^ value) { m_vTL = value; }
		}

		// 타원을 이루는 사각영역의 왼쪽 하단 모서리
		property UnE::Geometry::Vertex2D^ BottomLeft
		{
			UnE::Geometry::Vertex2D^ get() { return m_vBL; }
			void set(UnE::Geometry::Vertex2D^ value) { m_vBL = value; }
		}

		// 타원을 이루는 사각영역의 오른쪽 하단 모서리
		property UnE::Geometry::Vertex2D^ BottomRight
		{
			UnE::Geometry::Vertex2D^ get() { return m_vBR; }
			void set(UnE::Geometry::Vertex2D^ value) { m_vBR = value; }
		}

		property bool IsEllipse
		{
			bool get() { return m_isEllipse; }
			void set(bool value) { m_isEllipse = value; }
		}

		// Degree
		property double BeginAngle
		{
			double get() { return m_dBeginAngle; }
			void set(double value) { m_dBeginAngle = value; }
		}

		// Degree
		property double EArcAngle
		{
			double get() { return m_dEArcAngle; }
			void set(double value) { m_dEArcAngle = value; }
		}

		// 타원을 이루는 사각영역의 X축이 반시계 방향으로 얼마나 회전하였는가?
		// degree
		property double XAxisAngle
		{
			double get() { return m_dXAxisAngle; }
			void set(double value) { m_dXAxisAngle = value; }
		}

	
	protected:
		UnE::Geometry::Vertex2D^ m_vTL;
		UnE::Geometry::Vertex2D^ m_vBL;
		UnE::Geometry::Vertex2D^ m_vBR;
		double m_dWidth, m_dHeight;
		bool m_isEllipse;
		// degree
		double m_dBeginAngle, m_dEArcAngle;
		// 타원을 이루는 사각영역의 X축이 반시계 방향으로 얼마나 회전하였는가?
		// degree
		double m_dXAxisAngle;
	};
}
