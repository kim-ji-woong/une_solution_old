#include "stdafx.h"
#include "Component.h"

namespace FireSafetyManager
{
	Component::Component()
	{
		m_nID = -1;
		m_strTypeName = L"";
		m_strComponentName = L"";
	}

	Component::Component(int nID, const std::wstring& strTypeName, const std::wstring& strComponentName)
	{
		m_nID = nID;
		m_strTypeName = strTypeName;
		m_strComponentName = strComponentName;
	}

	int Component::GetID()
	{
		return m_nID;
	}

	const std::wstring& Component::GetTypeName()
	{
		return m_strTypeName;
	}

	const std::wstring& Component::GetComponentName()
	{
		return m_strComponentName;
	}
}
