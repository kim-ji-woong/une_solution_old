#pragma once

namespace UnE
{
	namespace Geometry
	{
		ref class Vertex2D;
	}
}

namespace DXFViewer
{
	ref class Shape;
	ref class LineType;
	ref class DXFControl;

	public ref class Block
	{
	public:
		Block(DXFControl^ ctrl);
		virtual ~Block(void);

	public:
		void Add(Shape^ pObj);
		bool Remove(Shape^ pObj);
		void RemoveAll();

		void SetLineType(LineType^ lineType);
		LineType^ GetLineType();

	public:
		property System::Collections::ArrayList^ Shapes
		{
			System::Collections::ArrayList^ get() { return m_listObj; }
		}

		property bool Hidden
		{
			bool get() { return m_isHidden; }
			void set(bool value) { m_isHidden = value; }
		}

		property bool Lock
		{
			bool get() { return m_isLock; }
			void set(bool value) { m_isLock = value; }
		}

		property UnE::Geometry::Vertex2D^ OriginVertex
		{
			UnE::Geometry::Vertex2D^ get() { return m_vOrigin; }
			void set(UnE::Geometry::Vertex2D^ vertex) { m_vOrigin = vertex; }
		}

		property System::Drawing::Color LineColor
		{
			System::Drawing::Color get() { return m_color; }
			void set(System::Drawing::Color value) { m_color = value; }
		}

		property System::String^ Name
		{
			System::String^ get() { return m_strBlockName; }
			void set(System::String^ value) { m_strBlockName = value; }
		}

	protected:
		System::Collections::ArrayList^ m_listObj;

	protected:
		bool m_isHidden;		// 숨김 기능
		bool m_isLock;		// 잠금 기능(true이면 삭제할 수 없다.)
		UnE::Geometry::Vertex2D^ m_vOrigin;
		LineType^ m_lineType;
		System::Drawing::Color m_color;
		System::String^ m_strBlockName;
		DXFControl^ m_ctrl;
	};
}
