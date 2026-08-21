#pragma once
#include "Shape.h"

namespace DXFDotNet
{
	ref class UnE::Geometry::Vertex2D;

	public ref class Text abstract : Shape
	{
	public:
		Text(void);
		virtual ~Text(void);
		virtual Text^ CreateText() = 0;
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
				return gcnew UnE::Geometry::Vertex2D(m_ptPos.X, m_ptPos.Y);
			}
		}

		virtual property UnE::Geometry::Vertex2D^ BoundaryTL
		{
			UnE::Geometry::Vertex2D^ get() override
			{
				return gcnew UnE::Geometry::Vertex2D(m_ptPos.X, m_ptPos.Y);
			}
		}

		virtual property UnE::Geometry::Vertex2D^ BoundaryBR
		{
			UnE::Geometry::Vertex2D^ get() override
			{
				return gcnew UnE::Geometry::Vertex2D(m_ptPos.X, m_ptPos.Y);
			}
		}

	public:
		void SetPosition(UnE::Geometry::Vertex2D^ value);

		property System::String^ Title
		{
			System::String^ get() { return m_strText; }
			void set(System::String^ value)
			{
				m_strText = value;

				//if (m_strTextGL != 0)
				//	delete[] m_strTextGL;

				//if (m_strText == nullptr)
				//	m_strTextGL = 0;
				//else
				//	m_strTextGL = ToWcharArray(m_strText);
			}
		}

		// 세로 정렬(Near : 위쪽 정렬, Center : 가운데, Far : 아래쪽 정렬)
		property System::Drawing::StringAlignment VerticalAlignment
		{
			System::Drawing::StringAlignment get() { return m_stringFormat->LineAlignment; }
			void set(System::Drawing::StringAlignment value) { m_stringFormat->LineAlignment = value; }
		}

		// 가로 정렬(Near : 왼쪽 정렬, Center : 가운데, Far : 오른쪽 정렬)
		property System::Drawing::StringAlignment HorizontalAlignment
		{
			System::Drawing::StringAlignment get() { return m_stringFormat->Alignment; }
			void set(System::Drawing::StringAlignment value) { m_stringFormat->Alignment = value; }
		}

		/*property System::Drawing::PointF Position
		{
			System::Drawing::PointF get() { return m_ptPos; }
			void set(System::Drawing::PointF value) { m_ptPos = value; }
		}*/

		property System::Drawing::Font^ Font
		{
			System::Drawing::Font^ get() { return m_font; }
			void set(System::Drawing::Font^ value)
			{
				m_font = value;
			}
		}

		property System::Drawing::SolidBrush^ Brush
		{
			System::Drawing::SolidBrush^ get() { return m_brush; }
			void set(System::Drawing::SolidBrush^ value) { m_brush = value; }
		}

		// Degree
		property double Angle
		{
			double get() { return m_dTextAngle; }
			void set(double value) { m_dTextAngle = value; }
		}

	protected:
		System::String^ m_strText;
		System::Drawing::StringFormat^ m_stringFormat;
		System::Drawing::PointF m_ptPos;
		System::Drawing::Font^ m_font;
		System::Drawing::SolidBrush^ m_brush;
		// Degree
		double m_dTextAngle;

		UnE::Geometry::Vertex2D^ m_vBoundaryTL;
		UnE::Geometry::Vertex2D^ m_vBoundaryBR;

		// 이전에 CalcBoundary() 호출시 사용했던 값들과 차이가 있는지 비교하기 위한 값들...
		double m_dPrevTextAngle;
		System::Drawing::PointF m_ptPrevPos;
		System::String^ m_strPrevText;
		int m_nPrevFontSize;

	};
}
