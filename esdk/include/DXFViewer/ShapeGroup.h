#pragma once
#include "Shape.h"

namespace DXFViewer
{
	ref class ShapeGroupOption;

	public ref class ShapeGroup : Shape
	{
	public:
		enum class DrawType { IMAGE = 0, SHAPE, NONE };

	public:
		ShapeGroup(void);
		ShapeGroup(ShapeGroupOption^ option);
		virtual ~ShapeGroup(void);

	public:
		virtual bool Draw(System::Drawing::Graphics^ g, bool bDrawText) override;
		// (x,y)만큼 객체를 옮긴다.
		virtual void Move(double x, double y) override;
		virtual ShapeType GetShapeType() override;
		virtual DXFViewer::Shape^ Clone() override;
		// Selectable이 false이면 HitTest 검사가 무조건 실패한다.
		virtual bool HitTest(double x, double y) override;
		virtual bool CheckClipBounds(System::Drawing::Graphics^ g, UnE::Geometry::Vertex2D^ vClipTL, UnE::Geometry::Vertex2D^ vClipBR) override;

	public:
		property UnE::Geometry::Vertex2D^ Position
		{
			virtual UnE::Geometry::Vertex2D^ get() override { return m_vPos; }
			void set(UnE::Geometry::Vertex2D^ value) { m_vPos = value; }
		}

		virtual  property UnE::Geometry::Vertex2D^ BoundaryTL
		{
			UnE::Geometry::Vertex2D^ get() override
			{
				if (m_arrShapes != nullptr)
				{
					double x, y;
					bool isFirst = true;

					for each (DXFViewer::Shape^ shape in m_arrShapes)
					{
						UnE::Geometry::Vertex2D^ vTL = shape->BoundaryTL;
						UnE::Geometry::Vertex2D^ vBR = shape->BoundaryBR;

						if (isFirst)
						{
							x = vTL->x;
							y = vTL->y;
						}
						else
						{
							if (x > vTL->x)
								x = vTL->x;
							if (y < vTL->y)
								y = vTL->y;
						}

						if (x > vBR->x)
							x = vBR->x;
						if (y < vBR->y)
							y = vBR->y;
					}

					return gcnew UnE::Geometry::Vertex2D(x, y);
				}
				
				return nullptr;
			}
		}

		virtual  property UnE::Geometry::Vertex2D^ BoundaryBR
		{
			UnE::Geometry::Vertex2D^ get() override
			{
				if (m_arrShapes != nullptr)
				{
					double x, y;
					bool isFirst = true;

					for each (DXFViewer::Shape^ shape in m_arrShapes)
					{
						UnE::Geometry::Vertex2D^ vTL = shape->BoundaryTL;
						UnE::Geometry::Vertex2D^ vBR = shape->BoundaryBR;

						if (isFirst)
						{
							x = vTL->x;
							y = vTL->y;
						}
						else
						{
							if (x < vTL->x)
								x = vTL->x;
							if (y > vTL->y)
								y = vTL->y;
						}

						if (x < vBR->x)
							x = vBR->x;
						if (y > vBR->y)
							y = vBR->y;
					}

					return gcnew UnE::Geometry::Vertex2D(x, y);
				}

				return nullptr;
			}
		}

	public:
		void AddShape(DXFViewer::Shape^ shape);
		void RemoveShape(DXFViewer::Shape^ shape);
		void RemoveShape(int nIndex);
		void Clear();
		int GetShapeCount();
		DXFViewer::Shape^ GetShape(int nIndex);

	protected:
		bool DrawGDI(System::Drawing::Graphics^ g,bool bDrawText);
		bool DrawGL();

	public:
		property DrawType DrawingType
		{
			DrawType get() { return m_drawType; }
			void set(DrawType value) { m_drawType = value; }
		}

		property System::Drawing::Image^ Image
		{
			System::Drawing::Image^ get() { return m_img; }
			void set(System::Drawing::Image^ value) { m_img = value; }
		}

		property DXFViewer::Shape^ Shape
		{
			DXFViewer::Shape^ get() { return m_shape; }
			void set(DXFViewer::Shape^ value) { m_shape = value; }
		}

	private:
		bool GetImageSize(float% rWidth, float% rHeight);
		
	protected:
		System::Collections::ArrayList^ m_arrShapes;
		DrawType m_drawType;
		UnE::Geometry::Vertex2D^ m_vPos;
		System::Drawing::SizeF m_imgSize;

		// m_img 또는 m_shape 둘 중의 하나가 그려진다.
		System::Drawing::Image^ m_img;
		DXFViewer::Shape^ m_shape;
		float m_fImageScale;
	};

	public ref class ShapeGroupOption
	{
	public:
		ShapeGroupOption();
		ShapeGroupOption(System::Drawing::Image^ img);
		ShapeGroupOption(System::Drawing::Image^ img, float fWidth, float fHeight);
		ShapeGroupOption(DXFViewer::Shape^ shape);

	public:
		property ShapeGroup::DrawType DrawingType
		{
			ShapeGroup::DrawType get() { return m_drawType; }
			void set(ShapeGroup::DrawType value) { m_drawType = value; }
		}

		property System::Drawing::Image^ Image
		{
			System::Drawing::Image^ get() { return m_img; }
			void set(System::Drawing::Image^ value) { m_img = value; }
		}

		property DXFViewer::Shape^ Shape
		{
			DXFViewer::Shape^ get() { return m_shape; }
			void set(DXFViewer::Shape^ value) { m_shape = value; }
		}

		property System::Drawing::SizeF ImageSize
		{
			System::Drawing::SizeF get() { return m_imgSize; }
			void set(System::Drawing::SizeF value) { m_imgSize = value; }
		}

	private:
		System::Drawing::Image^ m_img;
		DXFViewer::Shape^ m_shape;
		System::Drawing::SizeF m_imgSize;
		ShapeGroup::DrawType m_drawType;
	};
}
