#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

Ellipse::Ellipse(void)
{
	Init();
}

Ellipse::~Ellipse(void)
{
}

void Ellipse::Init()
{
	m_strSubClassName = L"AcDbEllipse";
	m_strEntityType	  = L"ELLIPSE";
	m_vNormal.m_pt[0] = 0.0;		// 값 확인할 것
	m_vNormal.m_pt[1] = 0.0;
	m_vNormal.m_pt[2] = 1.0;
	m_dParameterBegin = 0.0;
	m_dParameterEnd	  = 6.28318530717958647692;
}

void Ellipse::Write(Utility::FileManager* pMgr)
{
	Entity::Write(pMgr);

	AddLine(pMgr,100,L"%s",m_strSubClassName.data());
	if (m_fLineWidth != 0.0f) AddLine(pMgr,39,L"%d",(int)m_fLineWidth);
	AddLine(pMgr,10,L"%lf",m_dArrCoordCenter[0]);
	AddLine(pMgr,20,L"%lf",m_dArrCoordCenter[1]);
	AddLine(pMgr,30,L"%lf",m_dArrCoordCenter[2]);
	AddLine(pMgr,11,L"%lf",m_dArrCoordLongAxis[0]);
	AddLine(pMgr,21,L"%lf",m_dArrCoordLongAxis[1]);
	AddLine(pMgr,31,L"%lf",m_dArrCoordLongAxis[2]);

	if (m_vNormal.m_pt[0] != 0.0 || m_vNormal.m_pt[1] != 0.0 || m_vNormal.m_pt[2] != 1.0)
	{
		AddLine(pMgr,210,L"%lf",m_vNormal.m_pt[0]);
		AddLine(pMgr,220,L"%lf",m_vNormal.m_pt[1]);
		AddLine(pMgr,230,L"%lf",m_vNormal.m_pt[2]);
	}

	AddLine(pMgr,40,L"%lf",m_dRatio);
	AddLine(pMgr,41,L"%lf",m_dParameterBegin);
	AddLine(pMgr,42,L"%lf",m_dParameterEnd);
}

// dArrCoordLongAxis : 타원의 장축 끝점이 타원의 중점으로 부터 얼만큼 떨어져 있나를 나타냄
// dRatio : 단축대 장축의 비율
void Ellipse::SetEllipse(double dArrCoordCenter[3], double dArrCoordLongAxis[3], double dRatio)
{
	int nSize = sizeof(double) * 3;
	memcpy(m_dArrCoordCenter,dArrCoordCenter,nSize);
	memcpy(m_dArrCoordLongAxis,dArrCoordLongAxis,nSize);
	m_dRatio = dRatio;
}

// 시작각도와 끝 각도를 설정(Radian)
void Ellipse::SetAngle(double dBeginAngle, double dEndAngle)
{
	m_dParameterBegin = dBeginAngle;
	m_dParameterEnd	  = dEndAngle;
}

void Ellipse::GetLongAxisCoord(double* pX, double* pY, double* pZ)
{
	*pX = m_dArrCoordLongAxis[0];
	*pY = m_dArrCoordLongAxis[1];
	*pZ = m_dArrCoordLongAxis[2];
}

// 단축대 장축의 비율(short / long)
double Ellipse::GetRatio()
{
	return m_dRatio;
}

void Ellipse::GetParameter(double* pBeginAngle, double* pEndAngle)
{
	*pBeginAngle = m_dParameterBegin;
	*pEndAngle	 = m_dParameterEnd;
}

bool Ellipse::ReadDatai(int nCode, int nData)
{
	return __super::ReadDatai(nCode,nData);
}

bool Ellipse::ReadDatad(int nCode, double dData)
{
	bool bResult = __super::ReadDatad(nCode,dData);
	if (bResult) return bResult;

	switch (nCode)
	{
	case 10:
		m_dArrCoordCenter[0] = dData;
		return true;

	case 20:
		m_dArrCoordCenter[1] = dData;
		return true;

	case 30:
		m_dArrCoordCenter[2] = dData;
		return true;

	case 11:
		m_dArrCoordLongAxis[0] = dData;
		return true;

	case 21:
		m_dArrCoordLongAxis[1] = dData;
		return true;

	case 31:
		m_dArrCoordLongAxis[2] = dData;
		return true;

	case 210:
		m_vNormal.m_pt[0] = dData;
		return true;

	case 220:
		m_vNormal.m_pt[1] = dData;
		return true;

	case 230:
		m_vNormal.m_pt[2] = dData;
		return true;

	case 40:
		m_dRatio = dData;
		return true;

	case 41:
		m_dParameterBegin = dData;
		return true;

	case 42:
		m_dParameterEnd = dData;
		return true;
	}

	return false;
}

bool Ellipse::ReadDatas(int nCode, wchar_t* strData)
{
	bool bResult = __super::ReadDatas(nCode,strData);
	if (bResult) return bResult;

	return false;
}

END_NS
END_NS
