#include "stdafx.h"
#include "Polygon.h"
#include <algorithm>
#include "poly2tri.h"
#include <vector>

namespace VectorGraphics
{
	Polygon::Polygon()
	{
		m_arrIndices = 0;
		m_arrCoords = 0;
		m_nIndexCount = m_nVertexCount = 0;
		m_mode = DrawMode::Fill;
	}


	Polygon::~Polygon()
	{
		delete[] m_arrIndices;
		delete[] m_arrCoords;
	}

	void Polygon::Draw()
	{
		if (m_nIndexCount >= 3)
		{
			if (m_mode == DrawMode::Fill)
			{
				glBegin(GL_TRIANGLES);

				for (int i = 0; i < m_nIndexCount; i++)
				{
					glVertex2f(m_arrCoords[m_arrIndices[i] * 2], m_arrCoords[m_arrIndices[i] * 2 + 1]);
				}

				glEnd();
			}
			else if (m_mode == DrawMode::Boundary)
			{
				glBegin(GL_LINE_STRIP);

				for (int i = 0; i < m_nVertexCount; i++)
				{
					glVertex2f(m_arrCoords[i * 2], m_arrCoords[i * 2 + 1]);
				}

				glEnd();
			}
		}
	}

	void Polygon::AddVertex(const Vertex2D& rVertex)
	{
		m_vertices.push_back(rVertex);
	}

	int Polygon::GetVertexCount()
	{
		return (int)m_vertices.size();
	}

	bool Polygon::GetVertex(int nIndex, Vertex2D* pVertex)
	{
		if (nIndex < 0 || nIndex >= GetVertexCount())
			return false;

		std::list<Vertex2D>::iterator iter = m_vertices.begin();

		for (int i = 0; i < nIndex; i++)
		{
			iter++;
		}

		*pVertex = *iter;
		return true;
	}

	void Polygon::RemoveAt(int nIndex)
	{
		std::list<Vertex2D>::iterator iter = m_vertices.begin();

		for (int i = 0; i < nIndex; i++)
		{
			if (iter == m_vertices.end())
				return;

			iter++;
		}

		if (iter != m_vertices.end())
			m_vertices.erase(iter);
	}

	void Polygon::Clear()
	{
		m_vertices.clear();
	}

	unsigned int FindIndex(p2t::Point* point, p2t::Point* arrPoints, unsigned int nVertexCount)
	{
		for (unsigned int i = 0; i < nVertexCount; i++)
		{
			if (&arrPoints[i] == point)
				return i;
		}

		return -1;
	}

	bool SetIndices(unsigned int* indices, p2t::Point* arrPoints, unsigned int nVertexCount, p2t::Triangle* pTriangle)
	{
		for (int i = 0; i < 3; i++)
		{
			unsigned int nIndex = FindIndex(pTriangle->GetPoint(i), arrPoints, nVertexCount);

			if (nIndex < 0)
				return false;

			indices[i] = nIndex;
		}

		return true;
	}

	bool Triangulate(std::list<Vertex2D>& vertices, unsigned int*& arrIndices, float*& arrCoords, int& rIndexCount)
	{
		unsigned int nVertexCount = (unsigned int)vertices.size();

		if (nVertexCount >= 3)
		{
			delete[] arrIndices;
			delete[] arrCoords;

			p2t::Point* arrPoints = new p2t::Point[nVertexCount];
			// 시작점을 한번더 그릴수 있도록 하기 위하여 2만큼 더 추가
			arrCoords = new float[(nVertexCount + 1) * 2];
			std::list<Vertex2D>::iterator iter = vertices.begin();
			std::vector<p2t::Point*> vecPoints;

			for (unsigned int i = 0; i < nVertexCount; i++)
			{
				arrPoints[i].x = iter->x;
				arrPoints[i].y = iter->y;
				arrCoords[i * 2] = (float)iter->x;
				arrCoords[i * 2 + 1] = (float)iter->y;

				iter++;

				vecPoints.push_back(&arrPoints[i]);
			}

			iter = vertices.end();
			iter--;

			const Vertex2D& vFirst = *vertices.begin();
			const Vertex2D& vLast = *iter;

			if (vFirst.GetDistance(vLast) < 0.001)
			{
				vecPoints.pop_back();
			}

			p2t::CDT cdt(vecPoints);
			bool result = cdt.Triangulate();

			if (result == false)
			{
				delete[] arrPoints;
				delete[] arrCoords;
				arrIndices = 0;
				arrCoords = 0;
				rIndexCount = 0;

				return true;
			}

			std::vector<p2t::Triangle*> vecTriangles = cdt.GetTriangles();
			int nTriangleCount = (int)vecTriangles.size();

			if (nTriangleCount == 0)
			{
				delete[] arrPoints;
				delete[] arrCoords;
				arrIndices = 0;
				arrCoords = 0;
				rIndexCount = 0;

				return true;
			}

			rIndexCount = nTriangleCount * 3;
			arrIndices = new unsigned int[rIndexCount];

			for (int i = 0; i < nTriangleCount; i++)
			{
				p2t::Triangle* pTriangle = vecTriangles[i];
				SetIndices(&arrIndices[i * 3], arrPoints, nVertexCount, pTriangle);
			}

			delete[] arrPoints;

			vecTriangles.clear();
			vecPoints.clear();
			return true;
		}

		return false;
	}

	void Polygon::Done()
	{
		if (Triangulate(m_vertices, m_arrIndices, m_arrCoords, m_nIndexCount))
		{
			m_nVertexCount = (int)m_vertices.size();

			if (m_nVertexCount > 0)
			{
				std::list<Vertex2D>::iterator iter = m_vertices.end();
				iter--;

				const Vertex2D& vBegin = *m_vertices.begin();
				const Vertex2D& vLast = *iter;

				// Boundary 모드로 그릴때는 시작점과 끝점이 서로 만나야 한다.
				if (vBegin.GetDistance(vLast) > 0.1)
				{
					m_arrCoords[m_nVertexCount * 2] = vBegin.x;
					m_arrCoords[m_nVertexCount * 2 + 1] = vBegin.y;
					m_nVertexCount++;
				}
			}
		}
		else
			m_nVertexCount = 0;
		/*unsigned int nVertexCount = (unsigned int)m_vertices.size();

		if (nVertexCount >= 3)
		{
			delete[] m_arrIndices;
			delete[] m_arrCoords;

			p2t::Point* arrPoints = new p2t::Point[nVertexCount];
			m_arrCoords = new float[nVertexCount * 2];
			std::list<Vertex2D>::iterator iter = m_vertices.begin();
			std::vector<p2t::Point*> vecPoints;

			for (unsigned int i = 0; i < nVertexCount; i++)
			{
				arrPoints[i].x = iter->x;
				arrPoints[i].y = iter->y;
				m_arrCoords[i * 2] = (float)iter->x;
				m_arrCoords[i * 2 + 1] = (float)iter->y;

				iter++;

				vecPoints.push_back(&arrPoints[i]);
			}

			p2t::CDT cdt(vecPoints);
			cdt.Triangulate();

			std::vector<p2t::Triangle*> vecTriangles = cdt.GetTriangles();
			int nTriangleCount = (int)vecTriangles.size();

			if (nTriangleCount == 0)
			{
				delete[] m_arrCoords;
				m_arrIndices = 0;
				m_arrCoords = 0;
				m_nIndexCount = 0;

				return;
			}

			m_nIndexCount = nTriangleCount * 3;
			m_arrIndices = new unsigned int[m_nIndexCount];

			for (int i = 0; i < nTriangleCount; i++)
			{
				p2t::Triangle* pTriangle = vecTriangles[i];
				SetIndices(&m_arrIndices[i * 3], arrPoints, nVertexCount, pTriangle);
			}

			delete[] arrPoints;

			vecTriangles.clear();
			vecPoints.clear();
		}*/
	}

	static double GetAngle(const Vertex2D& v1, Vertex2D& vCenter, Vertex2D& v2)
	{
		// 코사인 제2법칙
		// C²= A²+ B²- 2ABcosΘ
		double a = vCenter.GetDistance(v1);
		double b = v2.GetDistance(vCenter);
		double c = v2.GetDistance(v1);

		double cosData = (a * a + b * b - c * c) / 2 / a / b;
		if (cosData < -1.0) cosData = -1.0;
		else if (cosData > 1.0) cosData = 1.0;

		return acos(cosData);
	}

	static const double _HALF_PI = 1.57079632679489661923;
	static const double _PI = 3.14159265358979323846;

	static double GetDistance(const Vertex2D& rVertex, Vertex2D& vBegin, Vertex2D& vEnd)
	{
		double dTolerance = 0.001;

		double a = vBegin.GetDistance(rVertex);
		double b = vBegin.GetDistance(vEnd);
		double c = vEnd.GetDistance(rVertex);

		if (a <= dTolerance || c <= dTolerance)
			return 0.0;
		if (b <= dTolerance)
			return a;

		double dCos = (a * a + b * b - c * c) / 2 / a / b;
		Vertex2D vertex = vBegin.GetLinearVertex(vEnd, dCos * a);
		double dLen = vertex.GetDistance(rVertex);

		double dAngle1 = GetAngle(rVertex, vBegin, vEnd);
		double dAngle2 = GetAngle(rVertex, vEnd, vBegin);

		if (dAngle1 <= _HALF_PI && dAngle2 <= _HALF_PI)
			return dLen;

		return a > c ? c : a;
	}

	static bool IsInclude(const Vertex2D& rVertex, Vertex2D& vBegin, Vertex2D& vEnd)
	{
		double dLen = GetDistance(rVertex, vBegin, vEnd);
		if (dLen <= 0.1)
			return true;

		return false;
	}

	static bool GetXFromLine(Vertex2D& rBegin, Vertex2D& rEnd, double y, double* pX, double dTolerance)
	{
		if (rBegin.y == rEnd.y)
		{
			if (y == rBegin.y)
			{
				*pX = (rBegin.x + rEnd.x) / 2;
				return true;
			}
			else
				return false;
		}

		*pX = (rEnd.x - rBegin.x) / (rEnd.y - rBegin.y) * (y - rBegin.y) + rBegin.x;

		if (rBegin.x < rEnd.x)
		{
			if (*pX < rBegin.x - dTolerance || *pX > rEnd.x + dTolerance)
				return false;
		}
		else
		{
			if (*pX < rEnd.x - dTolerance || *pX > rBegin.x + dTolerance)
				return false;
		}

		if (rBegin.y < rEnd.y)
		{
			if (y < rBegin.y - dTolerance || y > rEnd.y + dTolerance)
				return false;
		}
		else
		{
			if (y < rEnd.y - dTolerance || y > rBegin.y + dTolerance)
				return false;
		}

		return true;
	}

	static void CheckPointCount(Vertex2D& rLineBegin, Vertex2D& rLineEnd, float y, int& rCount, double dTolerance)
	{
		float dMaxY, dMinY;

		if (rLineBegin.y < rLineEnd.y)
		{
			dMinY = rLineBegin.y;
			dMaxY = rLineEnd.y;
		}
		else
		{
			dMinY = rLineEnd.y;
			dMaxY = rLineBegin.y;
		}

		// y가 rLineBegin과 rLineEnd 사이에 있거나, y가 둘 중 최소점과 일치하는 경우 rCount를 증가시킨다.
		// y가 둘 중 최대점과 일치하는 경우 rCount를 증가시키지 않는다.
		if (y < dMaxY - dTolerance && y >= dMinY - dTolerance)
		{
			rCount++;
		}
	}

	bool Polygon::HitTest(const Vertex2D& vPos)
	{
		int nVertexCount = GetVertexCount();
		if (nVertexCount < 3)
			return false;

		// 시작점과 끝점이 같은지 검사한다.
		std::list<Vertex2D>::iterator iter = m_vertices.end();
		iter--;

		Vertex2D& rBeginVertex = *m_vertices.begin();
		Vertex2D& rEndVertex = *iter;

		if (rBeginVertex.GetDistance(rEndVertex) < 0.1)
			nVertexCount--;

		if (nVertexCount < 3)
			return false;

		double dTolerance = 0.001;
		double x;
		int nCount = 0;
		Vertex2D* pPrev = &rEndVertex;

		for (iter = m_vertices.begin(); iter != m_vertices.end(); iter++)
		{
			Vertex2D& rVertex = *iter;
			
			if (IsInclude(vPos, *pPrev, rVertex))
				return true;
			else
			{
				double diff = pPrev->y - rVertex.y;

				if (diff < 0)
					diff = -diff;

				// X축과 평행한 선분은 계산하지 않는다.
				if (diff > dTolerance)
				{
					if (GetXFromLine(*pPrev, rVertex, vPos.y, &x, dTolerance))
					{
						diff = x - vPos.x;
						if (diff < 0.0)
							diff = -diff;

						if (diff <= dTolerance)
							return true;
						else if (x > vPos.x)
						{
							CheckPointCount(*pPrev, rVertex, vPos.y, nCount, dTolerance);
						}
					}
				}
			}

			pPrev = &rVertex;
		}

		if (nCount % 2 == 0)
			return false;

		return true;
	}

	bool Polygon::HitTestIfNotPOI(const Vertex2D& vPos)
	{
		return HitTest(vPos);
	}

	void Polygon::SetDrawingMode(DrawMode mode)
	{
		m_mode = mode;
	}

	Polygon::DrawMode Polygon::GetDrawingMode()
	{
		return m_mode;
	}
}
