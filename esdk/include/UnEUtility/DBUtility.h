#pragma once
#include <string>
#include <vector>

namespace UnE
{
	namespace Utility
	{
		class DBUtility
		{
		public:
			static bool InsertToUpdateString(std::wstring strInsertQuery, std::wstring& strUpdateQuery, int nPrimaryKeyIndex);
			static bool InsertToUpdateFile(std::wstring strInsertFilePath, std::wstring strUpdateFilePath, int nPrimaryKeyIndex);
			static std::wstring GetErrorMessage() { return m_strError; }

		private:
			static bool GetTokens(std::wstring strInsertQuery, std::wstring& strTableName, std::vector<std::wstring>& rVecFields, std::vector<std::wstring>& rVecValues);
			static bool ParseValue(const wchar_t* strValues, std::vector<std::wstring>& rVecValues);

		private:
			static std::wstring m_strError;
		};
	}
}
