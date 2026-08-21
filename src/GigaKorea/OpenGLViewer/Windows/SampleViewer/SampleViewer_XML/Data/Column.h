#pragma once
#include "Vertex2D.h"

namespace FireSafetyManager
{
	class Column
	{
	public:
		enum ColumnType { RectType = 0, CircleType };

	public:
		Column();

	public:
		void SetID(int nID);
		int GetID();
		void SetRectType(VectorGraphics::Vertex2D vTL, VectorGraphics::Vertex2D vBL, VectorGraphics::Vertex2D vBR);
		void GetRect(VectorGraphics::Vertex2D& vTL, VectorGraphics::Vertex2D& vBL, VectorGraphics::Vertex2D& vBR);
		void SetCircleType(VectorGraphics::Vertex2D vCenter, double dRadius);
		void GetCircle(VectorGraphics::Vertex2D& vCenter, double& dRadius);
		ColumnType GetColumnType();

	private:
		VectorGraphics::Vertex2D m_vTL, m_vBL, m_vBR;
		VectorGraphics::Vertex2D m_vCenter;
		double m_dRadius;
		ColumnType m_type;
		int m_nID;
	};
}
