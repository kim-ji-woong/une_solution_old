#pragma once
#include <string>

namespace FireSafetyManager
{
	class Component
	{
	public:
		Component();
		Component(int nID, const std::wstring& strTypeName, const std::wstring& strComponentName);

	public:
		int GetID();
		const std::wstring& GetTypeName();
		const std::wstring& GetComponentName();
		
	private:
		int m_nID;
		std::wstring m_strTypeName;
		std::wstring m_strComponentName;
	};
}
