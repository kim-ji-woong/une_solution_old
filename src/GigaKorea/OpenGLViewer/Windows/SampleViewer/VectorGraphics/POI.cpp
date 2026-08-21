#include "stdafx.h"
#include "POI.h"
#include <algorithm>
#include "VectorCtrl.h"
#include "Layer.h"

namespace VectorGraphics
{
	POIText::POIText()
	{
		m_strText = L"";
		m_dAngle = 0.0;
		m_fFontSize = 10.0f;
		m_strFontName = L"맑은 고딕";
	}

	POIText::POIText(const std::wstring& strText, const Vertex2D& vPos, float fFontSize, double dAngle)
	{
		m_strText = strText;
		m_vPos = vPos;
		m_dAngle = dAngle;
		m_fFontSize = fFontSize;
		m_strFontName = L"맑은 고딕";
	}

	const std::wstring& POIText::GetText()
	{
		return m_strText;
	}

	const Vertex2D& POIText::GetPosition()
	{
		return m_vPos;
	}

	void POIText::SetPosition(const Vertex2D& vPos)
	{
		m_vPos = vPos;
	}

	double POIText::GetAngle()
	{
		return m_dAngle;
	}

	float POIText::GetFontSize()
	{
		return m_fFontSize;
	}

	void POIText::SetFontName(const std::wstring& strFontName)
	{
		m_strFontName = strFontName;
	}

	int GetLineCount(const wchar_t* str)
	{
		int nLineCount = 0;

		for (int i = 0; str[i] != 0; i++)
		{
			if (str[i] == '\n')
				nLineCount++;
		}

		return nLineCount;
	}

	void POIText::Draw(Layer* pLayer, const Vertex2D& vPos)
	{
		if (pLayer)
		{
			VectorCtrl* pCtrl = pLayer->GetControl();

			if (pCtrl)
			{
				double dViewportWeight = pCtrl->GetViewportWeight();

				int x, y;
				pCtrl->GlobalToScreen(vPos + m_vPos, &x, &y);

				int nFontSize = (int)(m_fFontSize / dViewportWeight);

				int nLineCount = GetLineCount(m_strText.c_str());
				y += nFontSize * nLineCount;

				HDC hdc = pCtrl->GetHDC();

				int nEscapement = (int)(m_dAngle * 10);
				HFONT hFont = CreateFont(nFontSize, 0, nEscapement,
					0, 0, 0, 0, 0, HANGUL_CHARSET, 3, 2, 1, VARIABLE_PITCH | FF_ROMAN, m_strFontName.data());
				HGDIOBJ hOldFont = SelectObject(hdc, hFont);

				SetTextColor(hdc, pLayer->GetColor());
				SetBkMode(hdc, TRANSPARENT);
				TextOut(hdc, x, y, m_strText.c_str(), m_strText.length());
				
				SelectObject(hdc, hOldFont);
				DeleteObject(hFont);
			}
		}
	}

	POIIcon::POIIcon()
	{
		m_initialize = false;
	}

	static void DeleteVertexList(VertexList* pVertexList)
	{
		delete pVertexList;
	}

	POIIcon::~POIIcon()
	{
		std::for_each(m_boundaryEdges.begin(), m_boundaryEdges.end(), DeleteVertexList);
		std::for_each(m_fillEdges.begin(), m_fillEdges.end(), DeleteVertexList);

		for (std::list<unsigned int*>::iterator iter = m_indices.begin(); iter != m_indices.end(); iter++)
		{
			unsigned int* arrIndices = *iter;
			delete[] arrIndices;
		}

		for (std::list<float*>::iterator iter = m_coords.begin(); iter != m_coords.end(); iter++)
		{
			float* arrCoords = *iter;
			delete[] arrCoords;
		}
	}

	void POIIcon::SetBoundary(VertexList* pVertexList)
	{
		if (pVertexList->Vertices.begin() == pVertexList->Vertices.end())
			return;

		if (m_initialize == false)
		{
			const Vertex2D& vertex = *pVertexList->Vertices.begin();
			m_vTL.x = m_vBR.x = vertex.x;
			m_vTL.y = m_vBR.y = vertex.y;

			m_initialize = true;
		}

		for (std::list<Vertex2D>::iterator iter = pVertexList->Vertices.begin(); iter != pVertexList->Vertices.end(); iter++)
		{
			const Vertex2D& rVertex = *iter;

			if (m_vTL.x > rVertex.x)
				m_vTL.x = rVertex.x;
			if (m_vTL.y < rVertex.y)
				m_vTL.y = rVertex.y;

			if (m_vBR.x < rVertex.x)
				m_vBR.x = rVertex.x;
			if (m_vBR.y > rVertex.y)
				m_vBR.y = rVertex.y;
		}
	}

	void POIIcon::SetBoundary(POIText& text)
	{
		const Vertex2D& vCenter = text.GetPosition();
		float fFontSize = text.GetFontSize();
		double dTextLength = fFontSize * text.GetText().length();

		double tlX = vCenter.x - dTextLength / 2;
		double tlY = vCenter.y + fFontSize / 2;
		double brX = vCenter.x + dTextLength / 2;
		double brY = vCenter.y - fFontSize / 2;

		if (m_initialize == false)
		{
			m_vTL.x = tlX;
			m_vTL.y = tlY;
			m_vBR.x = brX;
			m_vBR.y = brY;

			m_initialize = true;
		}
		else
		{
			if (m_vTL.x > tlX)
				m_vTL.x = tlX;
			if (m_vTL.y < tlY)
				m_vTL.y = tlY;

			if (m_vBR.x < brX)
				m_vBR.x = brX;
			if (m_vBR.y > brY)
				m_vBR.y = brY;
		}
	}

	void POIIcon::AddBoundaryEdge(VertexList* pVertexList)
	{
		if (pVertexList != 0)
		{
			SetBoundary(pVertexList);
			m_boundaryEdges.push_back(pVertexList);
		}
	}

	extern bool Triangulate(std::list<Vertex2D>& vertices, unsigned int*& arrIndices, float*& arrCoords, int& rIndexCount);

	void POIIcon::AddFillEdge(VertexList* pVertexList)
	{
		unsigned int* arrIndices = 0;
		float* arrCoords = 0;
		int nIndexCount = 0;

		if (Triangulate(pVertexList->Vertices, arrIndices, arrCoords, nIndexCount))
		{
			SetBoundary(pVertexList);
			m_fillEdges.push_back(pVertexList);

			m_indices.push_back(arrIndices);
			m_coords.push_back(arrCoords);
			m_indexCounts.push_back(nIndexCount);
		}
	}

	void POIIcon::AddText(const POIText& text)
	{
		SetBoundary((POIText&)text);
		m_texts.push_back(text);
	}

	static void MoveVertexList(std::list<VertexList*>& edges, double x, double y)
	{
		for (std::list<VertexList*>::iterator iter = edges.begin(); iter != edges.end(); iter++)
		{
			VertexList* polygon = *iter;

			for (std::list<Vertex2D>::iterator iter2 = polygon->Vertices.begin(); iter2 != polygon->Vertices.end(); iter2++)
			{
				Vertex2D& rVertex = *iter2;
				rVertex.x += x;
				rVertex.y += y;
			}
		}
	}

	static void MoveVertexList(std::list<VertexList*>& edges, std::list<float*>& arrCoords, double x, double y)
	{
		std::list<float*>::iterator iterCoords = arrCoords.begin();

		for (std::list<VertexList*>::iterator iter = edges.begin(); iter != edges.end(); iter++)
		{
			VertexList* polygon = *iter;

			for (std::list<Vertex2D>::iterator iter2 = polygon->Vertices.begin(); iter2 != polygon->Vertices.end(); iter2++)
			{
				Vertex2D& rVertex = *iter2;
				rVertex.x += x;
				rVertex.y += y;
			}

			float* arr = *iterCoords;
			int nVertexCount = (int)polygon->Vertices.size();

			for (int i = 0; i <= nVertexCount; i++)
			{
				arr[i * 2] += (float)x;
				arr[i * 2 + 1] += (float)y;
			}

			iterCoords++;
		}
	}

	static void MoveTextList(std::list<POIText>& texts, double x, double y)
	{
		for (std::list<POIText>::iterator iter = texts.begin(); iter != texts.end(); iter++)
		{
			POIText& text = *iter;
			const Vertex2D& vPos = text.GetPosition();
			text.SetPosition(Vertex2D(vPos.x + x, vPos.y + y));
		}
	}

	void POIIcon::Done()
	{
		if (m_vTL.x != 0.0 || m_vBR.y != 0.0)
		{
			double dMoveX = -m_vTL.x;
			double dMoveY = -m_vBR.y;

			MoveVertexList(m_boundaryEdges, dMoveX, dMoveY);
			MoveVertexList(m_fillEdges, m_coords, dMoveX, dMoveY);
			MoveTextList(m_texts, dMoveX, dMoveY);

			m_vTL.x += dMoveX;
			m_vTL.y += dMoveY;
			m_vBR.x += dMoveX;
			m_vBR.y += dMoveY;
		}
	}

	int POIIcon::GetBoundaryEdgeCount()
	{
		return (int)m_boundaryEdges.size();
	}

	VertexList* POIIcon::GetBoundaryEdge(int nIndex)
	{
		if (nIndex < 0 || nIndex >= GetBoundaryEdgeCount())
			return 0;

		std::list<VertexList*>::iterator iter = m_boundaryEdges.begin();

		for (int i = 0; i < nIndex; i++)
		{
			iter++;
		}

		return *iter;
	}

	int POIIcon::GetFillEdgeCount()
	{
		return (int)m_fillEdges.size();
	}

	VertexList* POIIcon::GetFillEdge(int nIndex)
	{
		if (nIndex < 0 || nIndex >= GetFillEdgeCount())
			return 0;

		std::list<VertexList*>::iterator iter = m_fillEdges.begin();

		for (int i = 0; i < nIndex; i++)
		{
			iter++;
		}

		return *iter;
	}

	int POIIcon::GetTextCount()
	{
		return (int)m_texts.size();
	}

	POIText* POIIcon::GetText(int nIndex)
	{
		if (nIndex < 0 || nIndex >= GetTextCount())
			return 0;

		std::list<POIText>::iterator iter = m_texts.begin();

		for (int i = 0; i < nIndex; i++)
		{
			iter++;
		}

		POIText& rText = *iter;
		return &rText;
	}

	const Vertex2D& POIIcon::GetTL()
	{
		return m_vTL;
	}

	const Vertex2D& POIIcon::GetBR()
	{
		return m_vBR;
	}

	static void DrawVertex(const Vertex2D& rVertex)
	{
		glVertex2f((float)rVertex.x, (float)rVertex.y);
	}

	static void DrawBoundary(VertexList* pVertexList)
	{
		glBegin(GL_LINE_STRIP);

		std::for_each(pVertexList->Vertices.begin(), pVertexList->Vertices.end(), DrawVertex);

		glEnd();
	}

	static void DrawPolygon(unsigned int* arrIndices, float* arrCoords, int nIndexCount)
	{
		if (nIndexCount >= 3)
		{
			glBegin(GL_TRIANGLES);

			for (int i = 0; i < nIndexCount; i++)
			{
				glVertex2f(arrCoords[arrIndices[i] * 2], arrCoords[arrIndices[i] * 2 + 1]);
			}

			glEnd();
		}
	}

	void POIIcon::Draw(Layer* pLayer, const Vertex2D& vPos)
	{
		for (std::list<VertexList*>::iterator iter = m_boundaryEdges.begin(); iter != m_boundaryEdges.end(); iter++)
		{
			DrawBoundary(*iter);
		}

		std::list<unsigned int*>::iterator iterIndex = m_indices.begin();
		std::list<float*>::iterator iterCoord = m_coords.begin();
		std::list<int>::iterator iterIndexCount = m_indexCounts.begin();

		int nPolygonCount = m_fillEdges.size();

		for (int i = 0; i < nPolygonCount;i++)
		{
			if (iterIndex == m_indices.end() || iterCoord == m_coords.end() || iterIndexCount == m_indexCounts.end())
				break;

			unsigned int* arrIndices = *iterIndex;
			float* arrCoords = *iterCoord;
			int nIndexCount = *iterIndexCount;

			DrawPolygon(arrIndices, arrCoords, nIndexCount);

			iterIndex++;
			iterCoord++;
			iterIndexCount++;
		}

		for (std::list<POIText>::iterator iter = m_texts.begin(); iter != m_texts.end(); iter++)
		{
			POIText& rText = *iter;
			rText.Draw(pLayer, vPos);
		}
	}

	POI::POI()
	{
		m_pIcon = 0;
		m_setPosition = false;
	}


	POI::~POI()
	{
	}

	// OpenGL은 현재위치를 POI의 좌측 하단으로 잡고 그리기 때문에
	// m_vPos를 POI의 가운데로 놓기 위해서는 기준점을 더 왼쪽 아래로 이동시켜야 한다.
	static Vertex2D SetTranslateVertex(POIIcon* pIcon, const Vertex2D& vPos)
	{
		double dWidth = pIcon->GetBR().x - pIcon->GetTL().x;
		double dHeight = pIcon->GetTL().y - pIcon->GetBR().y;
		return Vertex2D(vPos.x - dWidth / 2, vPos.y - dHeight / 2);
	}

	void POI::SetPosition(const Vertex2D& vPos)
	{
		m_vPos = vPos;
		m_setPosition = true;

		if (m_pIcon != 0)
		{
			m_vTrans = SetTranslateVertex(m_pIcon, m_vPos);
		}
	}

	void POI::SetIcon(POIIcon* pIcon)
	{
		m_pIcon = pIcon;

		if (m_setPosition && m_pIcon != 0)
		{
			m_vTrans = SetTranslateVertex(m_pIcon, m_vPos);
		}
	}

	const Vertex2D& POI::GetPosition()
	{
		return m_vPos;
	}

	POIIcon* POI::GetIcon()
	{
		return m_pIcon;
	}

	void POI::Draw()
	{
		if (m_pIcon != 0)
		{
			glPushMatrix();
				glTranslatef((float)m_vTrans.x, (float)m_vTrans.y, 0.0f);
				m_pIcon->Draw(GetLayer(), m_vTrans);
			glPopMatrix();
		}
	}

	bool POI::HitTest(const Vertex2D& vPos)
	{
		if (m_pIcon == 0)
			return false;

		double dWidth = m_pIcon->GetBR().x - m_pIcon->GetTL().x;
		double dHeight = m_pIcon->GetTL().y - m_pIcon->GetBR().y;

		double tlX = m_vPos.x - dWidth / 2;
		double tlY = m_vPos.y + dHeight / 2;
		double brX = m_vPos.x + dWidth / 2;
		double brY = m_vPos.y - dHeight / 2;

		if (vPos.x >= tlX && vPos.x <= brX && vPos.y >= brY && vPos.y <= tlY)
			return true;

		return false;
	}

	bool POI::HitTestIfPOI(const Vertex2D& vPos)
	{
		return HitTest(vPos);
	}
}
