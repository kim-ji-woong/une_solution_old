#pragma once
#include "Shape.h"

namespace DXFViewer
{
	ref class UnE::Geometry::Vertex2D;
	ref class UnE::Geometry::Arc2D;
	ref class UnE::Geometry::EArc2D;
	ref class UnE::Geometry::Line2D;

	public ref class Hatch : Shape
	{
	public:
		ref class PathItem
		{
		public:
			enum class DrawType { None = 0, Line, Arc, EArc };

		private:
			UnE::Geometry::Line2D^ m_line = nullptr;
			UnE::Geometry::Arc2D^ m_arc = nullptr;
			UnE::Geometry::EArc2D^ m_earc = nullptr;
			DrawType m_drawType = DrawType::None;

		public:
			property UnE::Geometry::Line2D^ Line
			{
				UnE::Geometry::Line2D^ get()
				{
					return m_line;
				}
			}

			property UnE::Geometry::Arc2D^ Arc
			{
				UnE::Geometry::Arc2D^ get()
				{
					return m_arc;
				}
			}

			property UnE::Geometry::EArc2D^ EArc
			{
				UnE::Geometry::EArc2D^ get()
				{
					return m_earc;
				}
			}

			property DrawType DrawingType
			{
				DrawType get()
				{
					return m_drawType;
				}
			}

		public:
			void SetLine(UnE::Geometry::Vertex2D^ vBegin, UnE::Geometry::Vertex2D^ vEnd);
			void SetLine(UnE::Geometry::Line2D^ line);
			void SetArc(UnE::Geometry::Arc2D^ arc);
			void SetEArc(UnE::Geometry::EArc2D^ earc);
		};

	public:
		Hatch(void);
		virtual ~Hatch(void);
		
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
		void AddLine(UnE::Geometry::Vertex2D^ vBegin, UnE::Geometry::Vertex2D^ vEnd);
		void AddArc(UnE::Geometry::Arc2D^ arc);
		void AddEArc(UnE::Geometry::EArc2D^ earc);
		/*void SetVertex(System::Collections::ArrayList^ arrVertices);
		bool UpdatePoint(int nIndex, float x, float y);
		void SetPointSize(int nPointCount);
		int GetPointSize();
		bool GetPoint(int nIndex, [System::Runtime::InteropServices::OutAttribute] float% x, [System::Runtime::InteropServices::OutAttribute] float% y);*/

#pragma region OpenGL 함수들
		/*void CalcGLBuffer();
		float* GetVertexArray();
		unsigned int* GetIndexArray();
		int GetIndexArrayCount();*/
#pragma endregion

		void MakePath(double x, double y);

	protected:
		bool DrawGDI(System::Drawing::Graphics^ g);
		bool DrawGL();
		//void ClearGLBuffer();
		void AddPath(System::Drawing::Drawing2D::GraphicsPath^ path, PathItem^ item, double x, double y);

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

	public:
		property System::Drawing::PointF Center
		{
			System::Drawing::PointF get() { return m_ptCenter; }
			void set(System::Drawing::PointF value) { m_ptCenter = value; }
		}

		property System::Collections::Generic::List<PathItem^>^ PathItems
		{
			System::Collections::Generic::List<PathItem^>^ get() { return m_pathItems; }
		}

		/*property UnE::Geometry::Polygon^ Polygon
		{
			UnE::Geometry::Polygon^ get() { return m_polygon; }
		}*/

	private:
		//array<System::Drawing::PointF>^ m_arrPoint;
		// HitTest를 위한 Polygon
		UnE::Geometry::Polygon^ m_polygon;
		System::Drawing::SolidBrush^ m_brush;
		System::Drawing::PointF m_ptCenter;
		System::Drawing::Drawing2D::GraphicsPath^ m_path = nullptr;

		// 영역
		UnE::Geometry::Vertex2D^ m_vTL;
		UnE::Geometry::Vertex2D^ m_vBR;
		System::Collections::Generic::List<PathItem^>^ m_pathItems = gcnew System::Collections::Generic::List<PathItem^>();
		bool m_isInitArea;
		double m_dMoveX, m_dMoveY;

#pragma region OpenGL을 위한 변수들
		/*float* m_arrPointGL;
		unsigned int* m_arrIndex;
		int m_nIndexCount;*/
#pragma endregion
	};
}
