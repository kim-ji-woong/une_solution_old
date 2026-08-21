#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(OBJECTS)

std::wstring MLineStyle::m_strSubClassName = L"AcDbMlineStyle";

MLineStyle::MLineStyle(wchar_t* strStyleName, ObjectManager* pMgr)
	: Object(pMgr)
{
	m_strStyleName = strStyleName;
	Init();
}

MLineStyle::~MLineStyle(void)
{
}

wchar_t* MLineStyle::GetSubClassName()
{
	return (wchar_t*)m_strSubClassName.data();
}

wchar_t* MLineStyle::GetStyleName()
{
	return (wchar_t*)m_strStyleName.data();
}

void MLineStyle::Init()
{
	m_strEntityType = L"MLINESTYLE";
	m_nFillColor = 256;
	m_dBeginAngle = 90.0;
	m_dEndAngle   = 90.0;
	m_nElementSize = 0;

	AddElement(0.5,256,L"BYLAYER");
	AddElement(-0.5,256,L"BYLAYER");

	AddData(0,(wchar_t*)m_strEntityType.data());
	AddData(5,&m_nHandle);
}

// nColor : ACI(AutoCAD Color Index)¿¡ µû¸§
void MLineStyle::SetFillColor(int nColor)
{
	m_nFillColor = nColor;
}

void MLineStyle::SetAngle(double dBeginAngle, double dEndAngle)
{
	m_dBeginAngle = dBeginAngle;
	m_dEndAngle   = dEndAngle;
}

void MLineStyle::AddElement(double dOffset, int nColor, wchar_t* strLineType)
{
	Element ele;
	ele.m_dOffset = dOffset;
	ele.m_nColor  = nColor;
	ele.m_strLineType = strLineType;

	m_nElementSize++;
	m_listElement.push_back(ele);
}

void MLineStyle::SetData()
{
	int nFlag = 0;

	AddData(102,L"{ACAD_REACTORS");
	AddData(330,&m_nDictionaryHandle);
	AddData(102,L"}");
	AddData(330,&m_nDictionaryHandle);
	AddData(100,(wchar_t*)m_strSubClassName.data());
	AddData(2,(wchar_t*)m_strStyleName.data());
	AddData(70,&nFlag);
	AddData(3,"");
	AddData(62,&m_nFillColor);
	AddData(51,&m_dBeginAngle);
	AddData(52,&m_dEndAngle);
	AddData(71,&m_nElementSize);

	std::list<Element>::const_iterator p = m_listElement.begin();
	while (p != m_listElement.end())
	{
		Element ele = *p;
		AddData(49,&ele.m_dOffset);
		//AddData(256,&ele.m_nColor);
		AddData(6,(wchar_t*)ele.m_strLineType.data());
		++p;
	}
}

END_NS
END_NS
