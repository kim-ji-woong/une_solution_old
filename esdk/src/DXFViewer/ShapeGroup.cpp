#include "StdAfx.h"
#include "ShapeGroup.h"
#include "IPainter.h"
#include "EditBox.h"

using namespace System::Collections;
using namespace UnE::Geometry;

BEGIN_NS(DXFViewer)

ShapeGroup::ShapeGroup(void)
{
	m_arrShapes = gcnew ArrayList();
	m_drawType = DrawType::NONE;

	m_img = nullptr;
	m_shape = nullptr;
	m_vPos = gcnew UnE::Geometry::Vertex2D();

	m_imgSize.Width = -1.0f;
	m_imgSize.Height = -1.0f;
	m_fImageScale = 100.0f;
}

ShapeGroup::ShapeGroup(ShapeGroupOption^ option)
{
	m_arrShapes = gcnew ArrayList();

	if (option != nullptr)
	{
		m_drawType = option->DrawingType;

		m_img = option->Image;
		m_shape = option->Shape;
		m_imgSize = option->ImageSize;
	}
	else
	{
		m_drawType = DrawType::NONE;

		m_img = nullptr;
		m_shape = nullptr;
		m_imgSize.Width = -1.0f;
		m_imgSize.Height = -1.0f;
	}

	m_vPos = gcnew UnE::Geometry::Vertex2D();
	m_fImageScale = 100.0f;
}

ShapeGroup::~ShapeGroup(void)
{
}

bool ShapeGroup::GetImageSize(float% rWidth, float% rHeight)
{
	if (m_img != nullptr)
	{
		if (m_imgSize.Width < 0 || m_imgSize.Height < 0)
		{
			rWidth = m_img->Width * m_fImageScale;
			rHeight = m_img->Height * m_fImageScale;
		}
		else
		{
			rWidth = m_imgSize.Width * m_fImageScale;
			rHeight = m_imgSize.Height * m_fImageScale;
		}

		if (m_pOwner->DownToTop())
			rHeight = -rHeight;

		return true;
	}

	return false;
}

bool ShapeGroup::Draw(System::Drawing::Graphics^ g, bool bDrawText)
{
	if (m_pOwner->Renderer == IPainter::RendererType::GDI_PLUS)
	{
		return DrawGDI(g, bDrawText);
	}
	else if (m_pOwner->Renderer == IPainter::RendererType::OPEN_GL)
	{
		return DrawGL();
	}

	return false;
}

bool ShapeGroup::DrawGDI(System::Drawing::Graphics^ g, bool bDrawText)
{
	if (m_drawType == DrawType::IMAGE)
	{
		if (m_img != nullptr)
		{
			float fWidth = 0, fHeight = 0;
			GetImageSize(fWidth, fHeight);

			g->DrawImage(m_img, (float)m_vPos->x, (float)m_vPos->y, fWidth, fHeight);

			if (Selectable && Selected)
				m_editBox->Draw(g, (float)(m_vPos->x + fWidth / 2), (float)(m_vPos->y + fHeight / 2));

			return true;
		}
	}
	else if (m_drawType == DrawType::SHAPE)
	{
		if (m_shape != nullptr)
		{
			m_shape->Selectable = this->Selectable;
			m_shape->Selected = this->Selected;
			m_shape->Draw(g, bDrawText);

			return true;
		}
	}

	return false;
}

// (x,y)만큼 객체를 옮긴다.
void ShapeGroup::Move(double x, double y)
{
	m_vPos->x += x;
	m_vPos->y += y;
}

Shape::ShapeType ShapeGroup::GetShapeType()
{
	return ShapeType::GROUP;
}

DXFViewer::Shape^ ShapeGroup::Clone()
{
	ShapeGroup^ group = gcnew ShapeGroup();
	group->CopyFrom(this);

	group->m_vPos->x = this->m_vPos->x;
	group->m_vPos->y = this->m_vPos->y;

	group->m_drawType = this->m_drawType;

	group->m_img = this->m_img;
	group->m_shape = this->m_shape;

	for each (DXFViewer::Shape^ shape in this->m_arrShapes)
	{
		group->AddShape(shape);
	}

	return group;
}

// Selectable이 false이면 HitTest 검사가 무조건 실패한다.
bool ShapeGroup::HitTest(double x, double y)
{
	if (!Selectable)
		return false;

	if (m_drawType == DrawType::IMAGE)
	{
		float fWidth = 0, fHeight = 0;
		if (!GetImageSize(fWidth, fHeight))
			return false;

		if (fHeight < 0.0f)
			fHeight = -fHeight;

		/*Vertex2D^ vOrigin = m_pOwner->ScreenToGlobal(0, 0);
		Vertex2D^ v2 = m_pOwner->ScreenToGlobal((int)fWidth, (int)fHeight);

		double dWidth = System::Math::Abs(v2->x - vOrigin->x);
		double dHeight = System::Math::Abs(v2->y - vOrigin->y);

		if (x >= m_vPos->x && x <= m_vPos->x + dWidth &&
			y >= m_vPos->y && y <= m_vPos->y + dHeight)
			return true;*/
		if (x >= m_vPos->x && x <= m_vPos->x + fWidth &&
			y >= m_vPos->y - fHeight && y <= m_vPos->y)
			return true;
	}
	else if (m_drawType == DrawType::SHAPE)
	{
		if (m_shape == nullptr)
			return false;

		return m_shape->HitTest(x, y);
	}

	return false;
}

void ShapeGroup::AddShape(DXFViewer::Shape^ shape)
{
	m_arrShapes->Add(shape);
}

void ShapeGroup::RemoveShape(DXFViewer::Shape^ shape)
{
	m_arrShapes->Remove(shape);
}

void ShapeGroup::RemoveShape(int nIndex)
{
	if (nIndex >= 0 && m_arrShapes->Count > nIndex)
		m_arrShapes->RemoveAt(nIndex);
}

void ShapeGroup::Clear()
{
	m_arrShapes->Clear();
}

int ShapeGroup::GetShapeCount()
{
	return m_arrShapes->Count;
}

DXFViewer::Shape^ ShapeGroup::GetShape(int nIndex)
{
	if (nIndex >= 0 && m_arrShapes->Count > nIndex)
		return (DXFViewer::Shape^)m_arrShapes[nIndex];

	return nullptr;
}

bool ShapeGroup::CheckClipBounds(System::Drawing::Graphics^ g, UnE::Geometry::Vertex2D^ vClipTL, UnE::Geometry::Vertex2D^ vClipBR)
{
	UnE::Geometry::Vertex2D^ vTL = this->BoundaryTL;
	UnE::Geometry::Vertex2D^ vBR = this->BoundaryBR;

	if (vTL == nullptr || vBR == nullptr)
		return false;

	return CheckClipBounds(vClipTL, vClipBR, vTL, vBR);
}

ShapeGroupOption::ShapeGroupOption()
{
	m_img = nullptr;
	m_shape = nullptr;
	m_imgSize.Width = -1.0f;
	m_imgSize.Height = -1.0f;
	m_drawType = ShapeGroup::DrawType::NONE;
}

ShapeGroupOption::ShapeGroupOption(System::Drawing::Image^ img)
{
	m_img = img;
	m_shape = nullptr;
	m_imgSize.Width = -1.0f;
	m_imgSize.Height = -1.0f;
	m_drawType = ShapeGroup::DrawType::IMAGE;
}

ShapeGroupOption::ShapeGroupOption(System::Drawing::Image^ img, float fWidth, float fHeight)
{
	m_img = img;
	m_shape = nullptr;
	m_imgSize.Width = fWidth;
	m_imgSize.Height = fHeight;
	m_drawType = ShapeGroup::DrawType::IMAGE;
}

ShapeGroupOption::ShapeGroupOption(DXFViewer::Shape^ shape)
{
	m_img = nullptr;
	m_shape = shape;
	m_imgSize.Width = -1.0f;
	m_imgSize.Height = -1.0f;
	m_drawType = ShapeGroup::DrawType::SHAPE;
}

END_NS
