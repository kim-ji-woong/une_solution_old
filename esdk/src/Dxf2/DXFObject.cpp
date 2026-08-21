#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(OBJECTS)

Object::Object(ObjectManager* pMgr)
{
	m_pMgr = pMgr;

	if (m_pMgr != 0)
	{
		DXFManager* pDXFMgr = m_pMgr->GetOwner();

		if (pDXFMgr != 0)
		{
			m_nHandle = pDXFMgr->Get32BitHandle();
		}
	}

	//m_nHandle = Get32BitHandle();
	m_nDictionaryHandle = 0;
}

Object::~Object(void)
{
}

void Object::Write(Utility::FileManager* pMgr)
{
	std::list<DXFData>::const_iterator p = m_list.begin();
	while (p != m_list.end())
	{
		DXFData data = *p;
		WriteDXFData(pMgr,&data);
		++p;
	}
}

void Object::AddData(DXFData& rData)
{
	m_list.push_back(rData);
}

void Object::AddData(int nCode, void* pData)
{
	DXFData data;
	SetDXFData(nCode,pData,&data);
	m_list.push_back(data);
}

wchar_t* Object::GetEntityType()
{
	return (wchar_t*)m_strEntityType.data();
}

int Object::GetHandle()
{
	return m_nHandle;
}

int Object::GetDictionaryHandle()
{
	return m_nDictionaryHandle;
}

void Object::SetDictionaryHandle(int nDictionaryHandle)
{
	m_nDictionaryHandle = nDictionaryHandle;
}

END_NS
END_NS
