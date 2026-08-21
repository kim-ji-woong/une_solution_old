#pragma once

namespace UnE
{
	namespace Geometry
	{
		ref class Vertex2D;
		ref class Line2D;
	}
}

namespace DXFViewer
{
	interface class IPainter;
	ref class Layer;
	ref class Block;
	ref class LineType;
	ref class EditBox;

	public ref class Shape abstract
	{
	public:
		enum class ShapeType {LINE = 0, POLYLINE, ARC, EARC, HATCH, TEXT, GROUP, POINT, NONE };
		enum class ControlType {BYLAYER = 0, BYBLOCK, BYOWN };
		enum class SelectedShowingType { EDIT_BOX = 0, BRIGHT_EFFECT, DRAW_POLYGON, NONE };

	public:
		Shape(void);
		virtual ~Shape(void);

	public:
		virtual bool Draw(System::Drawing::Graphics^ g, bool bDrawText) = 0;
		// (x,y)만큼 객체를 옮긴다.
		virtual void Move(double x, double y) = 0;
		virtual ShapeType GetShapeType() = 0;
		virtual Shape^ Clone() = 0;
		// Selectable이 false이면 HitTest 검사가 무조건 실패한다.
		virtual bool HitTest(double x, double y) = 0;
		// Object가 Cliping 영역내에 존재하는가?
		virtual bool CheckClipBounds(System::Drawing::Graphics^ g, UnE::Geometry::Vertex2D^ vClipTL, UnE::Geometry::Vertex2D^ vClipBR) = 0;

	public:
		void SetLayer(Layer^ pLayer);
		void SetBlock(Block^ pBlock);
		Layer^ GetLayer();
		Block^ GetBlock();
		void SetColorOption(ControlType opt);
		ControlType GetColorOption();
		// ByLayer 옵션이 false일 경우 사용될 Color
		void SetOwnColor(System::Drawing::Color color);
		System::Drawing::Color GetOwnColor();
		// ByLayer 옵션이면 Layer Color
		// ByBlock 옵션이면 Block Color
		// ByOwn 옵션이면 m_color를 리턴한다.
		System::Drawing::Color GetColor();
		void SetLineTypeOption(ControlType opt);
		ControlType GetLineTypeOption();
		void SetOwnLineType(LineType^ lineType);
		LineType^ GetOwnLineType();
		// ByLayer 옵션이면 Layer LineType
		// ByBlock 옵션이면 Block LineType
		// ByOwn 옵션이면 m_lineType을 리턴한다.
		LineType^ GetLineType();
		void SetOwner(IPainter^ owner);
		IPainter^ GetOwner();
		// Line 두께를 화면 Scale에 상관없이 항상 일정하게 유지한다.
		// Return 값 : 화면에 표현하고자 하였던 원래 Pen의 두께
		static float SetScalePenWidth(System::Drawing::Pen^ pen, System::Drawing::Graphics^ g);

	public:
		virtual property UnE::Geometry::Vertex2D^ Position
		{
			UnE::Geometry::Vertex2D^ get() = 0;
		}

		virtual property UnE::Geometry::Vertex2D^ BoundaryTL
		{
			UnE::Geometry::Vertex2D^ get() = 0;
		}

		virtual property UnE::Geometry::Vertex2D^ BoundaryBR
		{
			UnE::Geometry::Vertex2D^ get() = 0;
		}
		
	public:
		property bool Selectable
		{
			bool get() { return m_isSelectable; }
			void set(bool value) { m_isSelectable = value; }
		}

		property bool Selected
		{
			bool get() { return m_isSelected; }
			void set(bool value) { m_isSelected = value; }
		}

		property SelectedShowingType SelectedShowing
		{
			SelectedShowingType get() { return m_selectedShowingType; }
			void set(SelectedShowingType value) { m_selectedShowingType = value; }
		}

		property bool Visible
		{
			bool get() { return m_isVisible; }
			void set(bool value) { m_isVisible = value; }
		}

		property System::Object^ Tag
		{
			System::Object^ get() { return m_tag; }
			void set(System::Object^ value) { m_tag = value; }
		}

		property int ID
		{
			int get() { return m_nID; }
			void set(int value) { m_nID = value; }
		}

	protected:
		// vTL, vBR로 이루어진 사각형이 Cliping 영역내에 포함되는가?
		virtual bool CheckClipBounds(UnE::Geometry::Vertex2D^ vClipTL, UnE::Geometry::Vertex2D^ vClipBR, UnE::Geometry::Vertex2D^ vTL, UnE::Geometry::Vertex2D^ vBR);

	protected:
		void CopyFrom(Shape^ shape);
		bool VertexInRect(double x, double y, double left, double right, double top, double bottom);
		//bool IntersectLine(UnE::Geometry::Line2D^ line1, UnE::Geometry::Line2D^ line2);

	protected:
		ControlType m_colorOption;
		System::Drawing::Color m_color;
		ControlType m_lineTypeOption;
		LineType^ m_lineType;
		Layer^ m_pOwnLayer;
		Block^ m_pOwnBlock;
		System::Object^ m_tag;
		
		IPainter^ m_pOwner;

		bool m_isSelectable;
		bool m_isSelected;
		bool m_isVisible;
		SelectedShowingType m_selectedShowingType;

		EditBox^ m_editBox;
		int m_nID;
	};
}
