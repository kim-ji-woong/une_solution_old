#pragma once

namespace DXFDotNet
{
	ref class DXFControl;

	public ref class LineType
	{
	public:
		LineType(DXFControl^ ctrl);
		virtual ~LineType(void);
		LineType(DXFControl^ ctrl, System::Drawing::Drawing2D::DashStyle lineStyle, int nLineWidth);

	public:
		System::Drawing::Pen^ GetPen();
		
		void SetLineType(System::Drawing::Drawing2D::DashStyle lineStyle, int nLineWidth);

		System::Drawing::Drawing2D::DashStyle GetLineStyle();
		int GetLineWidth();

	protected:
		// LineType별 Pen
		// Key : Line Style(상위 4바이트) + Line Width(하위 4바이트)
		static System::Collections::Generic::Dictionary<__int64, System::Drawing::Pen^>^ m_dicPens = gcnew System::Collections::Generic::Dictionary<__int64, System::Drawing::Pen^>();;

	public:
		property System::String^ LineTypeName
		{
			System::String^ get() { return m_strLineTypeName; }
			void set(System::String^ value) { m_strLineTypeName = value; }
		}

	protected:
		System::Drawing::Drawing2D::DashStyle m_lineStyle;
		int m_nLineWidth;
		System::String^ m_strLineTypeName;
		System::Drawing::Pen^ m_pen;
		DXFControl^ m_ctrl;
	};
}
