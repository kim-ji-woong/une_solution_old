#include "StdAfx.h"
#include "CgPoint.h"
#include "UnEUtility/StringManager.h"
#include "Parcel.h"

BEGIN_NS(UnE)
BEGIN_NS(LX)

CgPoint::CgPoint(void)
{
	m_strName = L"";
	m_survType = BOUNDARY;
	m_stateType = EXISTING;
	m_strOID = L"";
}

CgPoint::~CgPoint(void)
{
}

void CgPoint::SetName(std::wstring strName)
{
	m_strName = strName;
}

void CgPoint::SetOID(std::wstring strOID)
{
	m_strOID = strOID;
}

void CgPoint::SetSurveyType(CgPoint::SurveyType type)
{
	m_survType = type;
}

void CgPoint::SetSurveyType(wchar_t* strType)
{
	if (!_wcsicmp(L"monument", strType))
		m_survType = MONUMENT;
	else if (!_wcsicmp(L"control", strType))
		m_survType = CONTROL;
	else if (!_wcsicmp(L"sideshot", strType))
		m_survType = SIDESHOT;
	else if (!_wcsicmp(L"boundary", strType))
		m_survType = BOUNDARY;
	else if (!_wcsicmp(L"natural boundary", strType))
		m_survType = NATURAL_BOUNDARY;
	else if (!_wcsicmp(L"travers", strType))
		m_survType = TRAVERSE;
	else if (!_wcsicmp(L"reference", strType))
		m_survType = REFRERENCE;
	else if (!_wcsicmp(L"administrative", strType))
		m_survType = ADMINISTRATIVE;
}

void CgPoint::SetStateType(CgPoint::StateType type)
{
	m_stateType = type;
}

void CgPoint::SetStateType(wchar_t* strType)
{
	if (!_wcsicmp(L"abandoned", strType))
		m_stateType = ABANDONED;
	else if (!_wcsicmp(L"destroyed", strType))
		m_stateType = DESTROYED;
	else if (!_wcsicmp(L"existing", strType))
		m_stateType = EXISTING;
	else if (!_wcsicmp(L"proposed", strType))
		m_stateType = PROPOSED;
}

void CgPoint::SetVertex(Geometry::Vertex3D vertex)
{
	m_vertex = vertex;
}

std::wstring CgPoint::GetName() const
{
	return m_strName;
}

std::wstring CgPoint::GetOID() const
{
	return m_strOID;
}

CgPoint::SurveyType CgPoint::GetSurveyType() const
{
	return m_survType;
}

CgPoint::StateType CgPoint::GetStateType() const
{
	return m_stateType;
}

const Geometry::Vertex3D& CgPoint::GetVertex() const
{
	return m_vertex;
}

bool CgPoint::ReadPoint3D(wchar_t* strPoints)
{
	unsigned int nBeginIndex = 0, nEndIndex;
	int nIndex = 0;
	double* pData = 0;

	Geometry::Vertex3D vertex;

	while (CoordGeom::GetPoint3DNextIndex(strPoints, nBeginIndex, nEndIndex))
	{
		if (nIndex == 0)
			pData = &vertex.x;
		else if (nIndex == 1)
			pData = &vertex.y;
		else
			pData = &vertex.z;

		if (!Utility::StringManager::StrToDouble(strPoints, pData, nBeginIndex, nEndIndex))
			return false;

		nIndex++;
	}

	if (nIndex != 3)
		return false;

	SetVertex(vertex);
	return true;
}

CgPoints::CgPoints()
{
	m_pRefCount = new int;
	*m_pRefCount = 1;
}

CgPoints::~CgPoints()
{
	*m_pRefCount -= 1;

	if (*m_pRefCount <= 0)
	{
		FreeMemory();
		delete m_pRefCount;
	}
}

CgPoints::CgPoints(const CgPoints& rhs)
{
	Copy(rhs);
	*m_pRefCount += 1;
}

void CgPoints::operator= (const CgPoints& rhs)
{
	// 같은 메모리를 공유하고 있는지 검사
	bool bSame = false;
	if (m_pRefCount == rhs.m_pRefCount) bSame = true;

	if (!bSame)
	{
		*m_pRefCount -= 1;
		if (*m_pRefCount <= 0) 
		{
			FreeMemory();
			delete m_pRefCount;
		}
	}

	Copy(rhs);
	if (!bSame) *m_pRefCount += 1;
}

void CgPoints::AddPoint(CgPoint* pPoint)
{
	if (pPoint != 0)
		m_vecPoints.push_back(pPoint);
}

bool CgPoints::InsertPoint(int nIndex, CgPoint* pPoint)
{
	if (nIndex < 0 || nIndex > GetPointCount() || pPoint == 0)
		return false;

	std::vector<CgPoint*>::iterator iter = m_vecPoints.begin() + nIndex;
	m_vecPoints.insert(iter, pPoint);
	return true;
}

int CgPoints::GetPointCount() const
{
	return (int)m_vecPoints.size();
}

const CgPoint* CgPoints::GetPoint(int nIndex) const
{
	if (nIndex < 0 || nIndex >= GetPointCount())
		return 0;

	return m_vecPoints[nIndex];
}

bool CgPoints::RemovePoint(int nIndex)
{
	if (nIndex < 0 || nIndex >= GetPointCount())
		return false;

	m_vecPoints.erase(m_vecPoints.begin() + nIndex);
	return true;
}

void CgPoints::RemoveAllPoint(bool freeMemory)
{
	if (freeMemory)
		FreeMemory();
	else
		m_vecPoints.clear();
}

void CgPoints::FreeMemory()
{
	int nPointCount = GetPointCount();

	for (int i=0;i<nPointCount;i++)
	{
		delete m_vecPoints[i];
	}

	m_vecPoints.clear();
}

void CgPoints::Copy(const CgPoints& rhs)
{
	m_vecPoints.insert(m_vecPoints.begin(), rhs.m_vecPoints.begin(), rhs.m_vecPoints.end());

	m_pRefCount = rhs.m_pRefCount;
}

END_NS
END_NS
