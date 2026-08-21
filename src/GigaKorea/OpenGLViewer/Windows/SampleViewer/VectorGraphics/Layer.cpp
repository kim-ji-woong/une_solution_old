#include "stdafx.h"
#include "Layer.h"
#include <algorithm>
#include "Shape.h"

namespace VectorGraphics
{
	Layer::Layer()
	{
		m_visible = true;
		m_color = RGB(255, 255, 255);
		m_pCtrl = 0;
		m_fLineThick = 1.0f;
	}

	Layer::Layer(std::wstring name)
	{
		m_visible = true;
		m_color = RGB(255, 255, 255);
		m_strLayerName = name;
		m_pCtrl = 0;
		m_fLineThick = 1.0f;
	}

	void DeleteShape(Shape* pShape)
	{
		delete pShape;
	}

	Layer::~Layer()
	{
		std::for_each(m_listShapes.begin(), m_listShapes.end(), DeleteShape);
		m_listShapes.clear();
	}

	void Layer::AddShape(Shape* pShape)
	{
		if (pShape)
		{
			m_listShapes.push_back(pShape);
			pShape->SetLayer(this);
		}
	}

	void Layer::RemoveShape(Shape* pShape)
	{
		if (pShape)
		{
			m_listShapes.remove(pShape);
			pShape->SetLayer(0);
		}
	}

	int Layer::GetShapeCount()
	{
		return (int)m_listShapes.size();
	}

	Shape* Layer::GetShape(int nIndex)
	{
		if (nIndex < 0 || nIndex >= GetShapeCount())
			return 0;

		std::list<Shape*>::iterator iter = m_listShapes.begin();

		for (int i = 0; i < nIndex; i++)
		{
			iter++;
		}

		return *iter;
	}

	void Layer::Clear()
	{
		for (std::list<Shape*>::iterator iter = m_listShapes.begin(); iter != m_listShapes.end(); iter++)
		{
			Shape* pShape = *iter;
			pShape->SetLayer(0);
		}

		m_listShapes.clear();
	}

	void DrawShape(Shape* pShape)
	{
		pShape->Draw();
	}

	bool Layer::Draw()
	{
		if (m_visible)
		{
			glColor3f(GetRValue(m_color) / 255.0f, GetGValue(m_color) / 255.0f, GetBValue(m_color) / 255.0f);
			glLineStipple(1, 0xffff);
			glLineWidth(m_fLineThick);

			std::for_each(m_listShapes.begin(), m_listShapes.end(), DrawShape);
		}

		return m_visible;
	}

	void Layer::SetColor(COLORREF color)
	{
		m_color = color;
	}

	COLORREF Layer::GetColor()
	{
		return m_color;
	}

	void Layer::SetVisible(bool visible)
	{
		m_visible = visible;
	}

	bool Layer::GetVisible()
	{
		return m_visible;
	}

	Shape* Layer::HitTest(const Vertex2D& vPos)
	{
		for (std::list<Shape*>::iterator iter = m_listShapes.begin(); iter != m_listShapes.end(); iter++)
		{
			Shape* pShape = *iter;
			
			if (pShape->HitTest(vPos))
				return pShape;
		}

		return 0;
	}

	Shape* Layer::HitTestPOI(const Vertex2D& vPos)
	{
		for (std::list<Shape*>::iterator iter = m_listShapes.begin(); iter != m_listShapes.end(); iter++)
		{
			Shape* pShape = *iter;

			if (pShape->HitTestIfPOI(vPos))
				return pShape;
		}

		return 0;
	}

	Shape* Layer::HitTestExceptPOI(const Vertex2D& vPos)
	{
		for (std::list<Shape*>::iterator iter = m_listShapes.begin(); iter != m_listShapes.end(); iter++)
		{
			Shape* pShape = *iter;

			if (pShape->HitTestIfNotPOI(vPos))
				return pShape;
		}

		return 0;
	}

	void Layer::SetLayerName(std::wstring name)
	{
		m_strLayerName = name;
	}

	std::wstring Layer::GetLayerName()
	{
		return m_strLayerName;
	}

	void Layer::SetControl(VectorCtrl* pCtrl)
	{
		m_pCtrl = pCtrl;
	}

	VectorCtrl* Layer::GetControl()
	{
		return m_pCtrl;
	}

	void Layer::SetLineThick(float fLineThick)
	{
		m_fLineThick = fLineThick;
	}

	float Layer::GetLineThick()
	{
		return m_fLineThick;
	}
}
