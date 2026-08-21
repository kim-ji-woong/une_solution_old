#include "StdAfx.h"
#include "Shape.h"
#include "LineType.h"
#include "Layer.h"
#include "Block.h"
#include "DXFControl.h"
#include "ColorExtension.h"

BEGIN_NS(DXFDotNet)

Shape::Shape(void)
{
	m_lineTypeOption = ControlType::BYLAYER;
	//m_lineType = gcnew LineType();
	m_lineType = nullptr;

	m_colorOption = ControlType::BYLAYER;
	m_color = System::Drawing::Color::Black;

	m_pOwnLayer = nullptr;
	m_pOwnBlock = nullptr;
	m_isSelectable = false;
	m_isSelected = false;
	m_isVisible = true;
	m_selectedShowingType = SelectedShowingType::EDIT_BOX;

	m_tag = nullptr;

	m_nID = -1;
}

Shape::~Shape(void)
{
}

void Shape::SetLayer(Layer^ pLayer)
{
	m_pOwnLayer = pLayer;
}

void Shape::SetBlock(Block^ pBlock)
{
	m_pOwnBlock = pBlock;
}

Layer^ Shape::GetLayer()
{
	return m_pOwnLayer;
}

Block^ Shape::GetBlock()
{
	return m_pOwnBlock;
}

void Shape::SetColorOption(ControlType opt)
{
	m_colorOption = opt;
}

// Return 값 : true이면 byLayer, false이면 Layer 사용 안함.
Shape::ControlType Shape::GetColorOption()
{
	return m_colorOption;
}

// ByLayer 옵션이 false일 경우 사용될 Color
void Shape::SetOwnColor(System::Drawing::Color color)
{
	m_color = color;
}

System::Drawing::Color Shape::GetOwnColor()
{
	return m_color;
}

// ByLayer 옵션이 true이면 Layer Color
//                false이면 m_color가 리턴된다.
System::Drawing::Color Shape::GetColor()
{
	/*if (m_pOwner != nullptr)
	{
		System::Drawing::Color backColor = m_pOwner->GetBackColor();
		if (m_colorOption == ControlType::BYLAYER && m_pOwnLayer != nullptr)
		{
			if (backColor.ToArgb() == m_pOwnLayer->LineColor.ToArgb())
			{
				return ColorExtension::GetContrast(m_pOwnLayer->LineColor, true);
			}
			else
			{
				return m_pOwnLayer->LineColor;
			}
		}
		else if (m_colorOption == ControlType::BYBLOCK && m_pOwnBlock != nullptr)
		{
			if (backColor.ToArgb() == m_pOwnBlock->LineColor.ToArgb())
			{
				return ColorExtension::GetContrast(m_pOwnBlock->LineColor, true);
			}
			else

			{
				return m_pOwnBlock->LineColor;
			}
		}
		
		if (backColor.ToArgb() == m_color.ToArgb())
		{
			return ColorExtension::GetContrast(m_color, true);
		}
	}*/
	

	if (m_colorOption == ControlType::BYLAYER && m_pOwnLayer != nullptr)
	{
		return m_pOwnLayer->LineColor;
	}

	else if (m_colorOption == ControlType::BYBLOCK && m_pOwnBlock != nullptr)
		return m_pOwnBlock->LineColor;
	//else
	return m_color;
}

void Shape::SetLineTypeOption(ControlType opt)
{
	m_lineTypeOption = opt;
}

Shape::ControlType Shape::GetLineTypeOption()
{
	return m_lineTypeOption;
}

void Shape::SetOwnLineType(LineType^ lineType)
{
	m_lineType = lineType;
}

LineType^ Shape::GetOwnLineType()
{
	return m_lineType;
}

// ByLayer 옵션이면 Layer LineType
// ByBlock 옵션이면 Block LineType
// ByOwn 옵션이면 m_lineType을 리턴한다.
LineType^ Shape::GetLineType()
{
	if (m_colorOption == ControlType::BYLAYER)
		return m_pOwnLayer->GetLineType();
	else if (m_colorOption == ControlType::BYBLOCK)
		return m_pOwnBlock->GetLineType();
	//else
		return m_lineType;
}

void Shape::SetOwner(IShapeOwner^ owner)
{
	if (owner != nullptr && m_lineType == nullptr)
		m_lineType = gcnew LineType((DXFControl^)owner);

	m_pOwner = owner;
}

IShapeOwner^ Shape::GetOwner()
{
	return m_pOwner;
}

void Shape::CopyFrom(Shape^ shape)
{
	this->m_colorOption = shape->m_colorOption;
	this->m_color = shape->m_color;
	this->m_lineTypeOption = shape->m_lineTypeOption;
	this->m_lineType = shape->m_lineType;

	this->m_nID = shape->m_nID;
}


bool Shape::VertexInRect(double x, double y, double left, double right, double top, double bottom)
{
	return x >= left && x <= right && y >= bottom && y <= top;
}

bool Shape::CheckClipBounds(UnE::Geometry::Vertex2D^ vClipTL, UnE::Geometry::Vertex2D^ vClipBR, UnE::Geometry::Vertex2D^ vTL, UnE::Geometry::Vertex2D^ vBR)
{
	System::Drawing::RectangleF rectClip((float)vClipTL->x, (float)vClipBR->y, (float)(vClipBR->x - vClipTL->x), (float)(vClipTL->y - vClipBR->y));
	System::Drawing::RectangleF rectTarget((float)vTL->x, (float)vBR->y, (float)(vBR->x - vTL->x), (float)(vTL->y - vBR->y));

	bool result = rectClip.IntersectsWith(rectTarget);
	return result;
}
// vTL, vBR로 이루어진 사각형이 Cliping 영역내에 포함되는가?
/*bool Shape::CheckClipBounds(System::Drawing::Graphics^ g, UnE::Geometry::Vertex2D^ vTL, UnE::Geometry::Vertex2D^ vBR)
{
	double left = g->ClipBounds.X;
	double right = g->ClipBounds.Right;
	double bottom = g->ClipBounds.Y;
	double top = g->ClipBounds.Y + g->ClipBounds.Height;

	if (VertexInRect(vTL->x, vTL->y, left, right, top, bottom))
		return true;
	if (VertexInRect(vTL->x, vBR->y, left, right, top, bottom))
		return true;
	if (VertexInRect(vBR->x, vBR->y, left, right, top, bottom))
		return true;
	if (VertexInRect(vBR->x, vTL->y, left, right, top, bottom))
		return true;

	if (VertexInRect(left, top, vTL->x, vBR->x, vTL->y, vBR->y))
		return true;

	return false;
}*/


/*bool Shape::IntersectLine(UnE::Geometry::Line2D^ line1, UnE::Geometry::Line2D^ line2)
{
	UnE::Geometry::Vertex2D^ v1 = nullptr;
	UnE::Geometry::Vertex2D^ v2 = nullptr;
	UnE::Geometry::Line2D::LineType lineType;

	if (line1->IntersectLine(line2, v1, v2, lineType) > 0)
		return true;

	return false;
}*/

END_NS
