#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(OBJECTS)

ACDBPlaceHolder::ACDBPlaceHolder(int nHandle, int nSoftPointer, ObjectManager* pMgr)
	: Object(pMgr)
{
	m_strEntityType = L"ACDBPLACEHOLDER";
	m_nHandle = nHandle;
	m_nSoftPointer = nSoftPointer;

	DXFData data[6];
	SetDXFData(0,L"ACDBPLACEHOLDER",&data[0]);
	SetDXFData(5,&m_nHandle,&data[1]);
	SetDXFData(102,L"{ACAD_REACTORS",&data[2]);
	SetDXFData(330,&m_nSoftPointer,&data[3]);
	SetDXFData(102,L"}",&data[4]);
	SetDXFData(330,&m_nSoftPointer,&data[5]);

	for (int i=0;i<6;i++) m_list.push_back(data[i]);
}

ACDBPlaceHolder::~ACDBPlaceHolder(void)
{
}

END_NS
END_NS
