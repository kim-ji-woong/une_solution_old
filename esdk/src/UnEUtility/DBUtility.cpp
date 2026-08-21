#include "StdAfx.h"
#include "DBUtility.h"
#include "StringManager.h"
#include <fstream>
#include "FileManager.h"

BEGIN_NS(UnE)
BEGIN_NS(Utility)

std::wstring DBUtility::m_strError = L"";

// strToken의 시작과 끝부분에 ','나 ')', '(' 가 있으면 제거한다.
// rMode(Bit Flag) : 1 => '('로 시작되는 구문
//                   2 => ')'로 끝나는 구문
static wchar_t* ParseToken(wchar_t* strToken, int& rMode)
{
	rMode = 0;

	size_t nLen = wcslen(strToken);
	size_t nBeginIndex = nLen;

	for (size_t i=0;i<nLen;i++)
	{
		if (strToken[i] == '(')
			rMode = 1;

		if (strToken[i] != ',' && strToken[i] != ')' && strToken[i] != '(')
		{
			nBeginIndex = i;
			break;
		}
	}

	for (size_t i=nLen-1;i>=nBeginIndex;i--)
	{
		if (strToken[i] == ')')
			rMode |= 2;

		if (strToken[i] != ',' && strToken[i] != ')' && strToken[i] != '(')
		{
			strToken[i+1] = 0;
			break;
		}
	}

	return &strToken[nBeginIndex];
}

bool DBUtility::GetTokens(std::wstring strInsertQuery, std::wstring& strTableName, std::vector<std::wstring>& rVecFields, std::vector<std::wstring>& rVecValues)
{
	strInsertQuery = StringManager::ParseString(strInsertQuery.c_str());

	wchar_t strToken[1024];
	int nMode;
	int nCount = 0;
	bool inParenthesis = false;
	bool isValueTime = false;

	strTableName = L"";

	while (true)
	{
		strInsertQuery = StringManager::GetToken(strInsertQuery.c_str(), strToken);

		if (strInsertQuery.length() == 0 && wcslen(strToken) == 0)
			break;

		wchar_t* str = ParseToken(strToken, nMode);

		if (wcslen(str) > 0)
		{
			if (nCount == 0)
			{
				if (_wcsicmp(str, L"Insert") != 0)
				{
					m_strError = L"Insert 구문이 아닙니다.";
					return false;
				}
			}
			else if (nCount == 1)
			{
				if (_wcsicmp(str, L"into") != 0)
					strTableName = str;
			}
			else if (nCount == 2 && strTableName.length() == 0)
			{
				strTableName = str;
			}
			else
			{
				if ((nMode & 1) == 1)
					inParenthesis = true;

				if (inParenthesis)
				{
					if (isValueTime)
						rVecValues.push_back(str);
					else
						rVecFields.push_back(str);
				}
				else if (_wcsicmp(str, L"values") == 0)
				{
					isValueTime = true;
					
					if (!ParseValue(strInsertQuery.c_str(), rVecValues))
						return false;
					else
						break;
				}

				if ((nMode & 2) == 2)
					inParenthesis = false;
			}

			nCount++;

			if (strInsertQuery.length() == 0)
				break;
		}
	}

	if (strTableName.length() == 0)
	{
		m_strError = L"Table 이름이 존재하지 않습니다.";
		return false;
	}
	else if (rVecValues.size() != rVecFields.size())
	{
		m_strError = L"Field 개수와 Value 개수가 일치하지 않습니다.";
		return false;
	}
	else if (rVecValues.size() == 0)
	{
		m_strError = L"Value가 존재하지 않습니다.";
		return false;
	}

	return true;
}

bool DBUtility::ParseValue(const wchar_t* strValues, std::vector<std::wstring>& rVecValues)
{
	int nLen = (int)wcslen(strValues) - 1;
	int nBeginIndex = -1;
	int nQuotationCount = 0;

	for (int i=nLen-1;i>=0;i--)
	{
		if (strValues[i] != ' ' && strValues[i] != '\t')
		{
			nLen = i + 1;
			break;
		}
	}

	wchar_t strValue[10240];

	// 괄호 부분은 제외
	for (int i=1;i<nLen;i++)
	{
		if (nBeginIndex < 0)
		{
			if (strValues[i] != ' ' && strValues[i] != '\t')
			{
				nBeginIndex = i;
				nQuotationCount = 0;
			}

			if (strValues[i] == ',')
			{
				m_strError = L"비어있는 Field가 존재합니다.";
				return false;
			}
		}

		if (nBeginIndex >= 0)
		{
			if (strValues[i] == '\'')
			{
				if (nQuotationCount == 0)
					nQuotationCount++;
				else if (nQuotationCount == 1)
				{
					if (i < nLen - 1 && strValues[i + 1] == '\'')
						i++;
					else
						nQuotationCount++;
				}
				else if (nQuotationCount == 2)
				{
					m_strError = L"잘못된 구문이 존재합니다.";
					return false;
				}
			}
			else if (strValues[i] == ',')
			{
				if (nQuotationCount == 0 || nQuotationCount == 2)
				{
					int len = i - nBeginIndex;
					memcpy(strValue, &strValues[nBeginIndex], sizeof(wchar_t) * len);
					strValue[len] = 0;

					nBeginIndex = -1;

					rVecValues.push_back(strValue);
				}
			}
		}
	}

	if (nBeginIndex < 0)
	{
		m_strError = L"비어있는 Field가 존재합니다.";
		return false;
	}
	else if (nQuotationCount != 0 && nQuotationCount != 2)
	{
		m_strError = L"잘못된 구문이 존재합니다.";
		return false;
	}
	else
	{
		int len = nLen - nBeginIndex;
		memcpy(strValue, &strValues[nBeginIndex], sizeof(wchar_t) * len);
		strValue[len] = 0;

		rVecValues.push_back(strValue);
	}

	return true;
}

bool DBUtility::InsertToUpdateString(std::wstring strInsertQuery, std::wstring& strUpdateQuery, int nPrimaryKeyIndex)
{
	m_strError = L"";

	std::wstring strTableName;
	std::vector<std::wstring> vecFields, vecValues;

	if (!GetTokens(strInsertQuery, strTableName, vecFields, vecValues))
		return false;

	int nFieldCount = (int)vecFields.size();

	if (nPrimaryKeyIndex >= nFieldCount)
	{
		m_strError = L"PrimaryKey Index가 Field 개수를 벗어납니다.";
		return false;
	}

	strUpdateQuery = L"Update ";
	strUpdateQuery += strTableName + L" Set ";

	std::wstring strSet = L"";
	
	for (int i=0;i<nFieldCount;i++)
	{
		const std::wstring& strField = vecFields[i];
		const std::wstring& strValue = vecValues[i];

		if (strSet.length() == 0)
			strSet += strField + L" = " + strValue;
		else
		{
			strSet += L", ";
			strSet += strField + L" = " + strValue;
		}
	}

	strUpdateQuery += strSet + L" where " + vecFields[nPrimaryKeyIndex] + L" = " + vecValues[nPrimaryKeyIndex];
	return true;
}

bool DBUtility::InsertToUpdateFile(std::wstring strInsertFilePath, std::wstring strUpdateFilePath, int nPrimaryKeyIndex)
{
	m_strError = L"";

	//std::wifstream fin(strInsertFilePath);
	FileManager fin;

	//if (!fin.is_open())
	if (!fin.Open((wchar_t*)strInsertFilePath.c_str(), GENERIC_READ))
	{
		m_strError = L"\"";
		m_strError += strInsertFilePath + L"\" 존재하지 않는 파일입니다.";
		return false;
	}

	//std::wofstream fout(strUpdateFilePath);
	FileManager fout;

	//if (!fout.is_open())
	if (!fout.Open((wchar_t*)strUpdateFilePath.c_str(), GENERIC_WRITE))
	{
		//fin.close();
		fin.Close();

		m_strError = L"\"";
		m_strError += strInsertFilePath + L"\" 파일을 생성할 수 없습니다.";
	}

	wchar_t strLine[10240];
	int nLine = 0, nReadBytes = 0;
	std::wstring strUpdateQuery;

	//while (fin.getline(strLine, 10240))
	while (fin.ReadLine(strLine, 10240, &nReadBytes))
	{
		nLine++;
		std::wstring strInsertQuery = StringManager::ParseString(strLine);

		if (strInsertQuery.length() == 0)
		{
			fout.Write(strLine, 0, FILE_CURRENT);
			fout.Write(L"\r\n", 0, FILE_CURRENT);
			//fout << strLine << std::endl;
			continue;
		}

		if (!InsertToUpdateString(strLine, strUpdateQuery, nPrimaryKeyIndex))
		{
			// Insert 이외의 구문은 그냥 무시
			if (m_strError == L"Insert 구문이 아닙니다.")
			{
				//fout << strLine << std::endl;
				fout.Write(strLine, 0, FILE_CURRENT);
				continue;
			}

			fin.Close();
			fout.Close();
			//swprintf_s(strLine, L"Line : %d\r\n%S", nLine, m_strError.c_str());
			//m_strError = strLine;
			swprintf_s(strLine, L"Line : %d\r\n", nLine);
			m_strError = std::wstring(strLine) + m_strError;
			return false;
		}

		//fout << strUpdateQuery << std::endl;
		fout.Write((wchar_t*)strUpdateQuery.c_str(), 0, FILE_CURRENT);
		fout.Write(L"\r\n", 0, FILE_CURRENT);
	}

	fin.Close();
	fout.Close();
	return true;
}

END_NS
END_NS
