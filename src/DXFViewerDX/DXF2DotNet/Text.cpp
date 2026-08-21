#include "StdAfx.h"
#include "Text.h"
#include "Layer.h"
#include "IShapeOwner.h"

#include <string>

#include <msclr\marshal_cppstd.h>

using namespace UnE::Geometry;

BEGIN_NS(DXFDotNet)

Text::Text(void)
{
	m_strText = L"";
	m_stringFormat = gcnew System::Drawing::StringFormat();
	m_brush = gcnew System::Drawing::SolidBrush(System::Drawing::Color::Black);
	m_font = gcnew System::Drawing::Font(gcnew System::Drawing::FontFamily(System::Drawing::Text::GenericFontFamilies::Serif), 10.0f);
	m_dTextAngle = 0.0;

	//m_strTextGL = new wchar_t[1];
	//m_strTextGL[0] = 0;

	//SetFontName();

	m_vBoundaryTL = gcnew UnE::Geometry::Vertex2D();
	m_vBoundaryBR = gcnew UnE::Geometry::Vertex2D();

	// 이전에 CalcBoundary() 호출시 사용했던 값들과 차이가 있는지 비교하기 위한 값들...
	m_dPrevTextAngle = -1.0;
	m_strPrevText = L"";
	m_nPrevFontSize = -1;
}

Text::~Text(void)
{
	//if (m_strFontName != 0)
	//	delete[] m_strFontName;

	//if (m_strTextGL != 0)
	//	delete[] m_strTextGL;
}

// (x,y)만큼 객체를 옮긴다.
void Text::Move(double x, double y)
{
	m_ptPos.X += (float)x;
	m_ptPos.Y += (float)y;
}

Shape::ShapeType Text::GetShapeType()
{
	return ShapeType::TEXT;
}

Shape^ Text::Clone()
{
	Text^ text = CreateText();
	text->CopyFrom(this);

	if (this->m_strText == nullptr)
	{
		text->m_strText = nullptr;
		//text->m_strTextGL = 0;
	}
	else
	{
		text->m_strText = gcnew System::String(this->m_strText);
		//text->m_strTextGL = ToWcharArray(text->m_strText);
	}

	if (this->m_stringFormat == nullptr)
		text->m_stringFormat = nullptr;
	else
		text->m_stringFormat = (System::Drawing::StringFormat^)this->m_stringFormat->Clone();//gcnew System::Drawing::StringFormat(this->m_stringFormat);

	text->m_ptPos = this->m_ptPos;

	if (this->m_font == nullptr)
		text->m_font = nullptr;
	else
	{
		text->m_font = (System::Drawing::Font^)this->m_font->Clone();
	}

	if (this->m_brush == nullptr)
		text->m_brush = nullptr;
	else
		text->m_brush = (System::Drawing::SolidBrush^)this->m_brush->Clone();

	text->m_dTextAngle = this->m_dTextAngle;

	return text;
}

// Selectable이 false이면 HitTest 검사가 무조건 실패한다.
bool Text::HitTest(double x, double y)
{
	if (!Selectable)
		return false;
	
	// Text의 HitTest는 일단 나중에 구현...
	return false;
}

void Text::SetPosition(UnE::Geometry::Vertex2D^ value)
{
	m_ptPos.X = (float)value->x;
	m_ptPos.Y = (float)value->y;
}



END_NS
