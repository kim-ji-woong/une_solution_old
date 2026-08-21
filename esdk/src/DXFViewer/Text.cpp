#include "StdAfx.h"
#include "Text.h"
#include "Layer.h"
#include "IPainter.h"

#include <string>

#include <msclr\marshal_cppstd.h>

using namespace UnE::Geometry;

BEGIN_NS(DXFViewer)

Text::Text(void)
{
	m_strText = L"";
	m_stringFormat = gcnew System::Drawing::StringFormat();
	m_brush = gcnew System::Drawing::SolidBrush(System::Drawing::Color::Black);
	m_font = gcnew System::Drawing::Font(gcnew System::Drawing::FontFamily(System::Drawing::Text::GenericFontFamilies::Serif), 10.0f);
	m_dTextAngle = 0.0;

	m_strTextGL = new wchar_t[1];
	m_strTextGL[0] = 0;

	SetFontName();

	m_vBoundaryTL = gcnew UnE::Geometry::Vertex2D();
	m_vBoundaryBR = gcnew UnE::Geometry::Vertex2D();

	// 이전에 CalcBoundary() 호출시 사용했던 값들과 차이가 있는지 비교하기 위한 값들...
	m_dPrevTextAngle = -1.0;
	m_strPrevText = L"";
	m_nPrevFontSize = -1;
}

Text::~Text(void)
{
	if (m_strFontName != 0)
		delete[] m_strFontName;

	if (m_strTextGL != 0)
		delete[] m_strTextGL;
}

// (x,y)만큼 객체를 옮긴다.
void Text::Move(double x, double y)
{
	m_ptPos.X += (float)x;
	m_ptPos.Y += (float)y;
}

bool Text::Draw(System::Drawing::Graphics^ g, bool bDrawText)
{

	if (m_pOwner->Renderer == IPainter::RendererType::GDI_PLUS)
	{
		if (bDrawText == false)
			return true;

		return DrawGDI(g);
	}
	else if (m_pOwner->Renderer == IPainter::RendererType::OPEN_GL)
	{
		return DrawGL();
	}

	return false;
}

bool Text::DrawGDI(System::Drawing::Graphics^ g)
{
	m_brush->Color = GetColor();

	float m11 = 0.0f, m12 = 0.0f, m21 = 0.0f, m22 = 0.0f, dx = 0.0f, dy = 0.0f;	
	if (m_dTextAngle != 0.0)
	{
		System::Drawing::Drawing2D::Matrix^ matrix = g->Transform;
		m11 = matrix->Elements[0];
		m12 = matrix->Elements[1];
		m21 = matrix->Elements[2];
		m22 = matrix->Elements[3];
		dx = matrix->Elements[4];
		dy = matrix->Elements[5];

		g->TranslateTransform(m_ptPos.X, m_ptPos.Y);
		g->RotateTransform((float)m_dTextAngle);
		g->TranslateTransform(-m_ptPos.X, -m_ptPos.Y);
	}

	if (m_pOwnLayer->Owner->DownToTop())
	{
		// 윈도우 좌표계와 AutoCAD 좌표계는 세로 방향이 반대이므로 그대로 그리면 글자 모양이 뒤집힌다.
		// 이를 방지하기 위하여 세로축 방향을 다시 뒤집은 다음, Y좌표를 음수로 두어 글자가 뒤집히지 않은채
		// 원래 위치에서 표시되도록 한다.
		g->ScaleTransform(1.0f, -1.0f);
		m_ptPos.Y = -m_ptPos.Y;			
	
		// Edit by skkim 2015.02.25
		// 현재 Y축 Scale값을 가져온다.
		float x1 = g->Transform->Elements[3];
		// 폰트의 길이와 Y축의 곱이 실제 픽셀당 거리
		float h = x1 * m_font->Height;
		// 1 픽셀미만이면 의미없으므로 Cutoff를 1로 한다.
		// 자간이 좁아지면 Graphics에서 예외가 발생하므로 작은값은 피한다.
		if (h > 1.f || h < -1.f)
		{
			// CheckClipBounds()에서 처리하므로 다시 검사하지 않는다.
			/*// 드로우영역에 포함되지 않으면 리턴한다.
			if (g->Clip->GetBounds(g).Contains(m_ptPos.X, m_ptPos.Y))*/
			{
				try
				{
					g->DrawString(m_strText, m_font, m_brush, m_ptPos);
				}
				catch (System::InvalidOperationException^ )
				{
					//System::Diagnostics::Trace::WriteLine(ex->Message);
					//System::Diagnostics::Trace::WriteLine(ex->StackTrace);
				}
				catch (System::Runtime::InteropServices::ExternalException^ )
				{
					//System::Diagnostics::Trace::WriteLine(e->Message);
					//System::Diagnostics::Trace::WriteLine(e->StackTrace);
					//System::Diagnostics::Trace::WriteLine("H Value : " + h);
				}
			}
		}		
		m_ptPos.Y = -m_ptPos.Y;
		g->ScaleTransform(1.0f, -1.0f);
	}
	else
	{
		if (g->Clip->GetBounds(g).Contains(m_ptPos.X, m_ptPos.Y))
		{
			try
			{
				g->DrawString(m_strText, m_font, m_brush, m_ptPos);// , m_stringFormat);
			}
			catch (System::Runtime::InteropServices::ExternalException^ e)
			{
				//System::Diagnostics::Trace::WriteLine(e->Message);
				//System::Diagnostics::Trace::WriteLine(e->StackTrace);
			}
		}
	}

	if (m_dTextAngle != 0.0)
	{
		g->Transform = gcnew System::Drawing::Drawing2D::Matrix(m11, m12, m21, m22, dx, dy);		
	}

	return true;
}

void Text::CalcBoundary(System::Drawing::Graphics^ g)
{
	if (m_strText == m_strPrevText && m_dTextAngle == m_dPrevTextAngle && m_ptPos == m_ptPrevPos && m_nPrevFontSize == m_font->Height)
		return;

	m_strPrevText = m_strText;
	m_dPrevTextAngle = m_dTextAngle;
	m_ptPrevPos = m_ptPos;
	m_nPrevFontSize = m_font->Height;

	System::Drawing::SizeF sizeText = g->MeasureString(m_strText, m_font);

	double dAngle = UnE::Geometry::Math::DegToRad(m_dTextAngle);
	double dSin = System::Math::Cos(dAngle);
	double dCos = System::Math::Sin(dAngle);

	double x1 = m_ptPos.X;
	double y1 = m_ptPos.Y;
	double x2 = x1 + sizeText.Width * dSin;
	double y2 = y1 + sizeText.Width * dCos;
	double x3 = x2 + sizeText.Height * dCos;
	double y3 = y2 - sizeText.Height * dSin;
	double x4 = x3 + x1 - x2;
	double y4 = y3 + y1 - y2;

	m_vBoundaryTL->x = m_vBoundaryBR->x = x1;
	m_vBoundaryTL->y = m_vBoundaryBR->y = y1;

	if (m_vBoundaryTL->x > x2)
		m_vBoundaryTL->x = x2;
	if (m_vBoundaryTL->x > x3)
		m_vBoundaryTL->x = x3;
	if (m_vBoundaryTL->x > x4)
		m_vBoundaryTL->x = x4;

	if (m_vBoundaryTL->y < y2)
		m_vBoundaryTL->y = y2;
	if (m_vBoundaryTL->y < y3)
		m_vBoundaryTL->y = y3;
	if (m_vBoundaryTL->y < y4)
		m_vBoundaryTL->y = y4;

	if (m_vBoundaryBR->x < x2)
		m_vBoundaryBR->x = x2;
	if (m_vBoundaryBR->x < x3)
		m_vBoundaryBR->x = x3;
	if (m_vBoundaryBR->x < x4)
		m_vBoundaryBR->x = x4;

	if (m_vBoundaryBR->y > y2)
		m_vBoundaryBR->y = y2;
	if (m_vBoundaryBR->y > y3)
		m_vBoundaryBR->y = y3;
	if (m_vBoundaryBR->y > y4)
		m_vBoundaryBR->y = y4;
}

Shape::ShapeType Text::GetShapeType()
{
	return ShapeType::TEXT;
}

Shape^ Text::Clone()
{
	Text^ text = gcnew Text();
	text->CopyFrom(this);

	if (this->m_strText == nullptr)
	{
		text->m_strText = nullptr;
		text->m_strTextGL = 0;
	}
	else
	{
		text->m_strText = gcnew System::String(this->m_strText);
		text->m_strTextGL = ToWcharArray(text->m_strText);
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
		text->SetFontName();
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

	if (x >= m_vBoundaryTL->x && x <= m_vBoundaryBR->x &&
		y >= m_vBoundaryBR->y && y <= m_vBoundaryTL->y)
		return true;

	return false;
	
	// Text의 HitTest는 일단 나중에 구현...
	//return false;
}

void Text::SetPosition(UnE::Geometry::Vertex2D^ value)
{
	m_ptPos.X = (float)value->x;
	m_ptPos.Y = (float)value->y;
}

void Text::SetFontName()
{
	if (m_strFontName != 0)
		delete[] m_strFontName;

	m_strFontName = ToWcharArray(m_font->FontFamily->Name);
}

bool Text::CheckClipBounds(System::Drawing::Graphics^ g, UnE::Geometry::Vertex2D^ vClipTL, UnE::Geometry::Vertex2D^ vClipBR)
{
	CalcBoundary(g);
	return __super::CheckClipBounds(vClipTL, vClipBR, m_vBoundaryTL, m_vBoundaryBR);
}

END_NS
