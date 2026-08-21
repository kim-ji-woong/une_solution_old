#include "stdafx.h"
#include "Column.h"

using namespace VectorGraphics;

namespace FireSafetyManager
{
	Column::Column()
	{
		m_nID = 0;
		m_dRadius = 0;
		m_type = ColumnType::RectType;
	}

	void Column::SetID(int nID)
	{
		m_nID = nID;
	}

	int Column::GetID()
	{
		return m_nID;
	}

	void Column::SetRectType(Vertex2D vTL, Vertex2D vBL, Vertex2D vBR)
	{
		m_vTL = vTL;
		m_vBL = vBL;
		m_vBR = vBR;
		m_type = ColumnType::RectType;
	}

	void Column::GetRect(VectorGraphics::Vertex2D& vTL, VectorGraphics::Vertex2D& vBL, VectorGraphics::Vertex2D& vBR)
	{
		vTL = m_vTL;
		vBL = m_vBL;
		vBR = m_vBR;
	}

	void Column::SetCircleType(Vertex2D vCenter, double dRadius)
	{
		m_vCenter = vCenter;
		m_dRadius = dRadius;
		m_type = ColumnType::CircleType;
	}

	void Column::GetCircle(VectorGraphics::Vertex2D& vCenter, double& dRadius)
	{
		vCenter = m_vCenter;
		dRadius = m_dRadius;
	}

	Column::ColumnType Column::GetColumnType()
	{
		return m_type;
	}
}
