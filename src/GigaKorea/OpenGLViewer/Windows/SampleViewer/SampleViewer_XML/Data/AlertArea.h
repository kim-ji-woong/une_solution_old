#pragma once
#include <list>
#include <string>
#include "Vertex2D.h"

namespace FireSafetyManager
{
	class AlertArea
	{
	public:
		AlertArea();

	public:
		void SetID(int nID);
		int GetID();
		
		void AddBoundaryVertex(const VectorGraphics::Vertex2D& vertex);
		int GetBoundaryVertexCount();
		VectorGraphics::Vertex2D* GetBoundaryVertex(int nIndex);

		void SetName(const std::wstring& strName);

	private:
		std::list<VectorGraphics::Vertex2D> m_boundaries;
		std::wstring m_strName;
		int m_nID;
	};
}
