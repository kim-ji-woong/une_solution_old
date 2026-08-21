#pragma once
#include "Shape.h"

namespace DXFViewer
{
	ref class UnE::Geometry::Vertex2D;

	public ref class PolyLine : Shape
	{
	public:
		PolyLine(void);
		virtual ~PolyLine(void);
		
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
		void DrawLines(System::Drawing::Pen^ pen, System::Drawing::Graphics^ g);

	public:
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

	private:
		void ResetSelectedVertex(int nPointCount);
		void CheckClosed(int nPointCount);

	private:
		// for GDI
		array<System::Drawing::PointF>^ m_arrPoint;
		// for OpenGL
		float* m_arrPointGL;
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
