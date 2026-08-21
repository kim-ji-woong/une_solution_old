#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

Text::Text(void)
{
	Init();
}

Text::~Text(void)
{
}

void Text::Init()
{
	m_strSubClassName = L"AcDbText";
	m_strEntityType	  = L"TEXT";
	m_strStyleName	  = L"STANDARD";
	m_strData		  = L"";
	m_vNormal.m_pt[0] = 0.0;
	m_vNormal.m_pt[1] = 0.0;
	m_vNormal.m_pt[2] = 1.0;
	m_nHorizonJust		= 0;
	m_nVerticalJust		= 0;
	m_dAngle			= 0.0;
}

void Text::Write(Utility::FileManager* pMgr)
{
	Entity::Write(pMgr);

	AddLine(pMgr,100,L"%s",m_strSubClassName.data());
	if (m_fLineWidth != 0.0f) AddLine(pMgr,39,L"%d",(int)m_fLineWidth);
	AddLine(pMgr,10,L"%lf",m_dFirstAlignPoint[0]);
	AddLine(pMgr,20,L"%lf",m_dFirstAlignPoint[1]);
	AddLine(pMgr,30,L"%lf",m_dFirstAlignPoint[2]);
	AddLine(pMgr,40,L"%lf",m_dHeight);

	if (m_vNormal.m_pt[0] != 0.0 || m_vNormal.m_pt[1] != 0.0 || m_vNormal.m_pt[2] != 1.0)
	{
		AddLine(pMgr,210,L"%lf",m_vNormal.m_pt[0]);
		AddLine(pMgr,220,L"%lf",m_vNormal.m_pt[1]);
		AddLine(pMgr,230,L"%lf",m_vNormal.m_pt[2]);
	}

	AddLine(pMgr,1,L"%s",m_strData.data());
	AddLine(pMgr,7,L"%s",m_strStyleName.data());
	AddLine(pMgr,72,L"%d",m_nHorizonJust);

	if (m_nHorizonJust != 0 || m_nVerticalJust != 0)
	{
		AddLine(pMgr,11,L"%lf",m_dFirstAlignPoint[0]);
		AddLine(pMgr,21,L"%lf",m_dFirstAlignPoint[1]);
		AddLine(pMgr,31,L"%lf",m_dFirstAlignPoint[2]);
	}

	AddLine(pMgr,100,L"%s",m_strSubClassName.data());
	AddLine(pMgr,73,L"%d",m_nVerticalJust);
}

wchar_t* Text::GetString()
{
	return (wchar_t*)m_strData.data();
}

void Text::GetJustification(int* pHorizon, int* pVertical)
{
	*pHorizon = m_nHorizonJust;
	*pVertical= m_nVerticalJust;
}

wchar_t* Text::GetStyleName()
{
	return (wchar_t*)m_strStyleName.data();
}

void Text::SetString(wchar_t* strData)
{
	m_strData = strData;
}

void Text::SetStyleName(wchar_t* strStyleName)
{
	m_strStyleName = strStyleName;
}

void Text::SetHorizontalJustification(int nHorizon)
{
	m_nHorizonJust = nHorizon;
}

void Text::SetVerticalJustification(int nVertical)
{
	m_nVerticalJust = nVertical;
}

void Text::SetHeight(double dHeight)
{
	m_dHeight = dHeight;
}

double Text::GetHeight()
{
	return m_dHeight;
}

void Text::SetFirstAlignPoint(double dX, double dY, double dZ)
{
	m_dFirstAlignPoint[0] = dX;
	m_dFirstAlignPoint[1] = dY;
	m_dFirstAlignPoint[2] = dZ;
}

void Text::GetFirstAlignPoint(double* pX, double* pY, double* pZ)
{
	*pX = m_dFirstAlignPoint[0];
	*pY = m_dFirstAlignPoint[1];
	*pZ = m_dFirstAlignPoint[2];
}

void Text::SetSecondAlignPoint(double dX, double dY, double dZ)
{
	m_dSecondAlignPoint[0] = dX;
	m_dSecondAlignPoint[1] = dY;
	m_dSecondAlignPoint[2] = dZ;
}

void Text::GetSecondAlignPoint(double* pX, double* pY, double* pZ)
{
	*pX = m_dSecondAlignPoint[0];
	*pY = m_dSecondAlignPoint[1];
	*pZ = m_dSecondAlignPoint[2];
}

void Text::SetNormalVector(double dAxisX, double dAxisY, double dAxisZ)
{
	m_vNormal.m_pt[0] = dAxisX;
	m_vNormal.m_pt[1] = dAxisY;
	m_vNormal.m_pt[2] = dAxisZ;
}

void Text::GetNormalVector(double* pX, double* pY, double* pZ)
{
	*pX = m_vNormal.m_pt[0];
	*pY = m_vNormal.m_pt[1];
	*pZ = m_vNormal.m_pt[2];
}

bool Text::ReadDatai(int nCode, int nData)
{
	bool bResult = __super::ReadDatai(nCode,nData);
	if (bResult) return bResult;

	switch (nCode)
	{
	case 72:
		m_nHorizonJust = nData;
		return true;

	case 73:
		m_nVerticalJust = nData;
		return true;
	}

	return false;
}

bool Text::ReadDatad(int nCode, double dData)
{
	bool bResult = __super::ReadDatad(nCode,dData);
	if (bResult) return bResult;

	switch (nCode)
	{
	case 10:
		m_dArrTemp[0] = dData;
		return true;

	case 20:
		m_dArrTemp[1] = dData;
		return true;

	case 30:
		SetFirstAlignPoint(m_dArrTemp[0],m_dArrTemp[1],dData);
		return true;

	case 40:
		SetHeight(dData);
		return true;

	case 50:
		m_dAngle = dData;
		return true;

	case 11:
		m_dArrTemp[0] = dData;
		return true;

	case 21:
		m_dArrTemp[1] = dData;
		return true;

	case 31:
		SetSecondAlignPoint(m_dArrTemp[0],m_dArrTemp[1],dData);
		return true;

	case 210:
		m_dArrTemp[0] = dData;
		return true;

	case 220:
		m_dArrTemp[1] = dData;
		return true;

	case 230:
		SetNormalVector(m_dArrTemp[0],m_dArrTemp[1],dData);
		return true;
	}

	return false;
}

bool Text::ReadDatas(int nCode, wchar_t* strData)
{
	bool bResult = __super::ReadDatas(nCode,strData);
	if (bResult) return bResult;

	switch (nCode)
	{
	case 1:
		m_strData = strData;
		return true;

	case 7:
		m_strStyleName = strData;
		return true;
	}

	return false;
}

// Degree
double Text::GetTextAngle() const
{
	return m_dAngle;
}

END_NS
END_NS
