#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

PolyLine::PolyLine(bool bLWPoly)
: m_bLWPoly(bLWPoly)
{
	Init();
}

PolyLine::~PolyLine(void)
{
}

void PolyLine::Init()
{
	m_strSubClassName = L"AcDbPolyline";
	if (m_bLWPoly) m_strEntityType	  = L"LWPOLYLINE";
	else m_strEntityType = L"POLYLINE";
	m_nPointSize	  = 0;
	m_vNormal.m_pt[0] = 0.0;		// 값 확인할 것
	m_vNormal.m_pt[1] = 0.0;
	m_vNormal.m_pt[2] = 1.0;
	m_bClosed = false;
	m_dConstantWidth = 0.0;
	m_bReadVertex = false;
	m_bSetHandle = false;
	m_strVertexLayerName = L"";
	m_readClosed = false;
}

void PolyLine::ReadVertex(bool bVertex)
{
	m_bReadVertex = bVertex;
}

void PolyLine::SetClosed(bool bClosed)
{
	m_bClosed = bClosed;
}

bool PolyLine::GetClosed()
{
	return m_bClosed;
}

void PolyLine::AddPoint(Utility::Vertex2D pt)
{
	m_nPointSize++;
	m_list.push_back(Utility::Vertex3D(pt.m_pt[0],pt.m_pt[1],0.0));
}

void PolyLine::AddPoint(double dX, double dY)
{
	m_nPointSize++;
	m_list.push_back(Utility::Vertex3D(dX,dY,0.0));
}

int PolyLine::GetPointSize() const
{
	return (int)m_list.size();
}

void PolyLine::Write(Utility::FileManager* pMgr)
{
	Entity::Write(pMgr);
	int nFlag = m_bClosed ? 1 : 0;

	// Line Data 삽입
	AddLine(pMgr,100,L"%s",m_strSubClassName.data());
	if (m_fLineWidth != 0.0f) AddLine(pMgr,39,L"%d",(int)m_fLineWidth);
	AddLine(pMgr,90,L"%d",m_nPointSize);
	AddLine(pMgr,70,L"%d",nFlag);
	AddLine(pMgr,43,L"%lf",m_dConstantWidth);

	std::list<Utility::Vertex3D>::const_iterator p = m_list.begin();
	while (p != m_list.end())
	{
		const Utility::Vertex3D& pt = *p;
		AddLine(pMgr,10,L"%lf",pt.m_pt[0]);
		AddLine(pMgr,20,L"%lf",pt.m_pt[1]);
		++p;
	}
}

// pID : 좌표 정보가 담긴 링크드 리스트 노드의 포인터
// Return 값 : true(좌표를 얻어오는데 성공)
//             false(더 이상 얻어올 좌표가 없다.)
bool PolyLine::GetPoint(void*& pID, double* pX, double* pY, double* pBulge)
{
	//static std::list<Utility::Vertex3D>::iterator p;
	std::list<Utility::Vertex3D>::iterator& p = m_vertexIter;

	if (pID == 0) p = m_list.begin();
	else 
	{
		p = *(std::list<Utility::Vertex3D>::iterator*)pID;
	}

	if (p != m_list.end())
	{
		Utility::Vertex3D pt = *p;
		p++;
		pID = &p;

		*pX = pt.m_pt[0];
		*pY = pt.m_pt[1];
		*pBulge = pt.m_pt[2];

		return true;
	}

	return false;
}

bool PolyLine::ReadDatai(int nCode, int nData)
{
	bool bResult = __super::ReadDatai(nCode,nData);
	if (bResult) return bResult;

	switch (nCode)
	{
	case 70:
		if (!m_readClosed)
		{
			m_bClosed = (nData & 1) == 1;
			m_readClosed = true;
			/*if (nData == 0) m_bClosed = false;
			else m_bClosed = true;*/
		}
		return true;

	case 90:
		m_nPointSize = nData;
		return true;
	}

	return false;
}

double PolyLine::GetConstantWidth() const
{
	return m_dConstantWidth;
}

void PolyLine::SetConstantWidth(double dWidth)
{
	if (dWidth >= 0.0) m_dConstantWidth = dWidth;
}

bool PolyLine::ReadDatad(int nCode, double dData)
{
	bool bResult = __super::ReadDatad(nCode,dData);
	if (bResult) return bResult;

	switch (nCode)
	{
	case 10:
		m_dTemp = dData;
		return true;

	case 20:
		if (m_bLWPoly)
		{
			m_list.push_back(Utility::Vertex3D(m_dTemp,dData,0.0));
		}
		else
		{
			if (m_bReadVertex) m_list.push_back(Utility::Vertex3D(m_dTemp,dData,0.0));
		}
		m_bReadVertex = false;
		return true;

	case 42:
		if (m_list.size() > 0)
		{
			std::list<Utility::Vertex3D>::iterator pIter = m_list.end();pIter--;
			pIter->m_pt[2] = dData;
			return true;
		}
		else return false;

	case 43:
		m_dConstantWidth = dData;
		return true;
	}

	return false;
}

bool PolyLine::ReadDatas(int nCode, wchar_t* strData)
{
	if (m_bLWPoly)
	{
ReadPolyLineString:
		bool bResult = __super::ReadDatas(nCode,strData);
		if (bResult) return bResult;
	}
	else
	{
		if (nCode == 0 && !_wcsicmp(strData,L"VERTEX")) m_bReadVertex = true;
		if (nCode == 8)
		{
			if (m_bReadVertex)
			{
				if (m_strVertexLayerName.size() == 0) m_strVertexLayerName = strData;
				else
				{
					if (m_strVertexLayerName != strData) m_bReadVertex = false;
				}

				return true;
			}
		}

		goto ReadPolyLineString;
	}

	return false;
}

void PolyLine::SetHandle(int nHandle)
{
	if (!m_bSetHandle)
	{
		__super::SetHandle(nHandle);
		m_bSetHandle = true;
	}
}

END_NS
END_NS
