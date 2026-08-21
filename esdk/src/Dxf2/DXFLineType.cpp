#include "StdAfx.h"

BEGIN_NS(PenWorld)

LineType::LineType(void)
{
	/*char strTemp[32];
	sprintf(strTemp,"Unnamed Line");
	m_strTypeName = strTemp;*/
	swprintf_s(m_strTypeName,L"Unnamed Line");

	m_fThick = 1.0f;
	m_nStyle = 0xffff;
	m_nFactor= 1;
	//m_color = RGB(255,255,255);
	m_nRefCount = 0;
}

LineType::~LineType(void)
{
}

void LineType::SetTypeName(wchar_t* strTypeName)
{
	//m_strTypeName = strTypeName;
	wcscpy_s(m_strTypeName, 256, strTypeName);
}

wchar_t* LineType::GetTypeName()
{
	//return (char*)m_strTypeName.data();
	return m_strTypeName;
}

void LineType::SetStyle(unsigned short nStyle)
{
	m_nStyle = nStyle;
}

void LineType::SetThick(float fThick)
{
	m_fThick = fThick;
}

void LineType::SetFactor(int nFactor)
{
	m_nFactor = nFactor;
}

float LineType::GetThick()
{
	return m_fThick;
}

int LineType::GetStyle()
{
	return m_nStyle;
}

int LineType::GetFactor()
{
	return m_nFactor;
}

void LineType::AddRef()
{
	m_nRefCount++;
}

void LineType::SetZeroCount()
{
	m_nRefCount = 0;
}

int LineType::GetRefCount() const
{
	return m_nRefCount;
}

END_NS
