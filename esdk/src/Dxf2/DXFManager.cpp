#include "stdafx.h"
#include "UnEUtility/StringManager.h"

BEGIN_NS(DXF)

//static wchar_t g_strError[256];

inline wchar_t* UndefinedCode(wchar_t* strError, int nLine, int nCode)
{
	int nRemainder = nCode % 10;

	if (nRemainder == 2 || nRemainder == 4 || nRemainder == 5 || nRemainder == 9)
	{
        swprintf_s(strError, 256, L"Line %d : %d는 정의되지 않은 Code입니다.",nLine,nCode);
	}
	else
	{
		swprintf_s(strError, 256, L"Line %d : %d은 정의되지 않은 Code입니다.", nLine, nCode);
	}

	return strError;
}

inline wchar_t* CodeError(wchar_t* strError, int nLine)
{
	swprintf_s(strError, 256, L"Line %d : 정수 형태의 Code가 존재하여야만 합니다.", nLine);
	return strError;
}

inline wchar_t* DataError(wchar_t* strError, int nCode, int nLine, wchar_t* strType)
{
	int nRemainder = nCode % 10;

	if (nRemainder == 2 || nRemainder == 4 || nRemainder == 5 || nRemainder == 9)
	{
		swprintf_s(strError, 256, L"Line %d : Code 번호 %d는 %s 형태의 값을 가져야만 합니다.", nLine, nCode, strType);
	}
	else
	{
		swprintf_s(strError, 256, L"Line %d : Code 번호 %d은 %s 형태의 값을 가져야만 합니다.", nLine, nCode, strType);
	}

	return strError;
}

DXFManager::DXFManager(void)
{
	m_pBlkMgr = 0;
	m_pClsMgr = 0;
	m_pEntMgr = 0;
	m_pHdrMgr = 0;
	m_pObjMgr = 0;
	m_pTblMgr = 0;

	m_n16BitHandle = 1;
	m_n32BitHandle = 1;

	ClearError();
}

DXFManager::~DXFManager(void)
{
}

void DXFManager::SetBlockManager(BLOCKS::BlockManager* pBlkMgr)
{
	if (m_pBlkMgr == pBlkMgr)
		return;

	if (m_isFirstBlkMgr)
	{
		delete m_pBlkMgr;
		m_isFirstBlkMgr = false;
	}

	m_pBlkMgr = pBlkMgr;
	m_pBlkMgr->SetOwner(this);
}

void DXFManager::SetClassManager(CLASSES::ClassManager* pClsMgr)
{
	m_pClsMgr = pClsMgr;
	m_pClsMgr->SetOwner(this);
}

void DXFManager::SetEntityManager(ENTITIES::EntityManager* pEntMgr)
{
	m_pEntMgr = pEntMgr;
	m_pEntMgr->SetOwner(this);
}

void DXFManager::SetHeaderManager(HEADER::CHeader* pHdrMgr)
{
	m_pHdrMgr = pHdrMgr;
	m_pHdrMgr->SetOwner(this);
}

void DXFManager::SetObjectManager(OBJECTS::ObjectManager* pObjMgr)
{
	m_pObjMgr = pObjMgr;
	m_pObjMgr->SetOwner(this);
}

void DXFManager::SetTableManager(TABLES::TableManager* pTblMgr)
{
	m_pTblMgr = pTblMgr;
	m_pTblMgr->SetOwner(this);
}

wchar_t* Trim(wchar_t* str)
{
	wchar_t strNull[1];
	strNull[0] = 0;

	if (str == 0)
		return strNull;

	int nBeginIndex = -1, nLastIndex = -1;

	for (int i = 0; str[i]; i++)
	{
		if (str[i] != ' ' && str[i] != '\t' && str[i] != '\r' && str[i] != '\n')
		{
			if (nBeginIndex < 0)
				nBeginIndex = i;

			nLastIndex = i;
		}
	}

	if (nBeginIndex < 0 || nLastIndex < nBeginIndex)
		return strNull;

	str[nLastIndex + 1] = 0;
	return &str[nBeginIndex];
}

bool DXFManager::OpenFile(wchar_t* strPath)
{
	if (strPath == 0) 
	{
		m_strError = L"DXF 파일 경로가 설정되지 않았습니다.";
		return false;
	}

	ClearError();

	try
	{
		Utility::FileManager mgr;
		if (!mgr.Open(strPath,GENERIC_READ)) throw L"파일을 열 수 없거나 잘못된 경로입니다.";

		if (m_pBlkMgr == 0) throw L"BLOCKS Data가 설정되지 않았습니다.";
		if (m_pClsMgr == 0) throw L"CLASSES Data가 설정되지 않았습니다.";
		if (m_pEntMgr == 0) throw L"ENTITIES Data가 설정되지 않았습니다.";
		if (m_pHdrMgr == 0) throw L"HEADER Data가 설정되지 않았습니다.";
		if (m_pObjMgr == 0) throw L"OBJECTS Data가 설정되지 않았습니다.";
		if (m_pTblMgr == 0) throw L"TABLES Data가 설정되지 않았습니다.";

		wchar_t buf[3072];
		int nSize, nLine = 0, nCode, nData;
		double dData;
		bool bNewSection = false;

		SectionManager* pMgr = 0;

		while (mgr.ReadLine(buf,3072,&nSize))
		{
			nLine++;
			//std::wstring _str = UnE::Utility::StringManager::ParseString(buf);
			//const wchar_t* str = _str.data();
			const wchar_t* str = (const wchar_t*)Trim(buf);

			if ((nLine % 2) == 0)
			{
				int nRange = GetCodeRange(nCode);

				if (nRange < 0) throw UndefinedCode(m_strErrorBuf, nLine, nCode);
				else if (nRange == 0)
				{
					if (!UnE::Utility::StringManager::StrToInt(str, &nData)) throw DataError(m_strErrorBuf, nCode, nLine, L"정수");
					if (pMgr) pMgr->ReadDatai(nCode,nData);
				}
				else if (nRange == 1)
				{
					if (!UnE::Utility::StringManager::HexToInt(str, &nData))
					{
						if (pMgr != 0 && pMgr->ReadStringHandle())
						{
							pMgr->ReadDatas(nCode, (wchar_t*)str);
							continue;
						}
						else
							throw DataError(m_strErrorBuf, nCode, nLine, L"16진수 형태의 문자열");
					}

					if (pMgr) pMgr->ReadDatai(nCode,nData);
				}
				else if (nRange == 2)
				{
					if (!UnE::Utility::StringManager::StrToDouble(str, &dData)) throw DataError(m_strErrorBuf, nCode, nLine, L"실수");
					if (pMgr) pMgr->ReadDatad(nCode,dData);
				}
				else
				{
					if (nCode == 0 && !wcscmp(str, L"SECTION")) bNewSection = true;
					else
					{
						if (bNewSection && nCode == 2)
						{
							if (!wcscmp(str, L"HEADER")) pMgr = m_pHdrMgr;
							else if (!wcscmp(str, L"CLASSES")) pMgr = m_pClsMgr;
							else if (!wcscmp(str, L"TABLES")) pMgr = m_pTblMgr;
							else if (!wcscmp(str, L"BLOCKS")) pMgr = m_pBlkMgr;
							else if (!wcscmp(str, L"ENTITIES")) pMgr = m_pEntMgr;
							else if (!wcscmp(str, L"OBJECTS")) pMgr = m_pObjMgr;
							else 
							{
								pMgr = 0;
								continue;
							}

							bNewSection = false;
							pMgr->Clear();
						}
						else
						{
							if (pMgr) pMgr->ReadDatas(nCode,(wchar_t*)str);
						}
					}
				}
			}
			else
			{
				if (!UnE::Utility::StringManager::StrToInt(str, &nCode)) throw CodeError(m_strErrorBuf, nLine);
			}
		}

		// MText 임시 객체들을 삭제한다.
		m_pEntMgr->RemoveTempMText(m_pBlkMgr);
	}
	catch (wchar_t* strError)
	{
		m_strError = strError;
		return false;
	}

	return true;
}

bool DXFManager::SaveFile(wchar_t* strPath)
{
	if (strPath == 0) 
	{
		m_strError = L"DXF 파일 경로가 설정되지 않았습니다.";
		return false;
	}

	ClearError();

	try
	{
		if (m_pBlkMgr == 0) throw L"BLOCKS Data가 설정되지 않았습니다.";
		if (m_pClsMgr == 0) throw L"CLASSES Data가 설정되지 않았습니다.";
		if (m_pEntMgr == 0) throw L"ENTITIES Data가 설정되지 않았습니다.";
		if (m_pHdrMgr == 0) throw L"HEADER Data가 설정되지 않았습니다.";
		if (m_pObjMgr == 0) throw L"OBJECTS Data가 설정되지 않았습니다.";
		if (m_pTblMgr == 0) throw L"TABLES Data가 설정되지 않았습니다.";

		Utility::FileManager mgr;
		if (!mgr.Open(strPath,GENERIC_WRITE)) throw L"파일을 생성할 수 없거나 잘못된 경로입니다.";

		m_pHdrMgr->UpdateNextHandle();

		m_pHdrMgr->Write(&mgr);
		m_pClsMgr->Write(&mgr);
		m_pTblMgr->Write(&mgr);
		m_pBlkMgr->Write(&mgr);
		m_pEntMgr->Write(&mgr);
		m_pObjMgr->Write(&mgr);

		AddLine(&mgr,0,L"EOF");
		//mgr.Close();
	}
	catch (wchar_t* strError)
	{
		m_strError = strError;
		return false;
	}

	return true;
}

wchar_t* DXFManager::GetErrorMessage()
{
	return (wchar_t*)m_strError.data();
}

void DXFManager::ClearError()
{
	m_strError = L"";
}

short DXFManager::Get16BitHandle()
{
	return m_n16BitHandle++;
}

int DXFManager::Get32BitHandle()
{
	return m_n32BitHandle++;
}

END_NS
