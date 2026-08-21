#include "stdafx.h"
#include "UnEUtility/StringManager.h"

// º¤ÅÍ°ö
Utility::Vertex3D CrossProduct(const Utility::Vertex3D& v1, const Utility::Vertex3D& v2);

BEGIN_NS(DXF)
BEGIN_NS(ENTITIES)

MText::MText(void)
{
	Init();
}

MText::~MText(void)
{
}

void MText::Init()
{
	m_strSubClassName = L"AcDbMText";
	m_strEntityType	  = L"MTEXT";
	m_strStyleName	  = L"STANDARD";
	m_strData		  = L"";
	m_dAngle		  = 0.0;

	m_dAreaWidth = m_dAreaHeight = 0.0;
}

void MText::Write(Utility::FileManager* pMgr)
{
	Entity::Write(pMgr);

	AddLine(pMgr,100,L"%s",m_strSubClassName.data());
	AddLine(pMgr,10,L"%lf",m_dInsertionPoint[0]);
	AddLine(pMgr,20,L"%lf",m_dInsertionPoint[1]);
	AddLine(pMgr,30,L"%lf",m_dInsertionPoint[2]);
	AddLine(pMgr,40,L"%lf",m_dHeight);
	AddLine(pMgr,41,L"%lf",m_dAreaWidth);
	AddLine(pMgr,71,L"%d",m_nAttachmentPoint);
	AddLine(pMgr,72,L"%d",m_nDrawingDirection);
	AddLine(pMgr,1,L"%s",m_strData.data());

	AddLine(pMgr,7,L"%s",m_strStyleName.data());

	AddLine(pMgr,11,L"%lf",m_vecAxis[0].m_pt[0]);
	AddLine(pMgr,21,L"%lf",m_vecAxis[0].m_pt[1]);
	AddLine(pMgr,31,L"%lf",m_vecAxis[0].m_pt[2]);

	AddLine(pMgr,7,L"%s",m_strStyleName.data());
	AddLine(pMgr,73,L"1");
	AddLine(pMgr,44,L"%lf",m_dLineSpace);

	AddLine(pMgr,1001,L"ACAD");
	AddLine(pMgr,1000,L"MTEXTBEGIN");
	AddLine(pMgr,1070,L"1");
	AddLine(pMgr,1070,L"44");
	AddLine(pMgr,1040,L"%lf",m_dAreaHeight);
	AddLine(pMgr,1070,L"74");
	AddLine(pMgr,1070,L"0");
	AddLine(pMgr,1000,L"MTEXTEND");
}

bool MText::ReadDatai(int nCode, int nData)
{
	bool bResult = __super::ReadDatai(nCode,nData);
	if (bResult) return bResult;

	switch (nCode)
	{
	case 71:
		m_nAttachmentPoint = nData;
		return true;

	case 72:
		m_nDrawingDirection = nData;
		return true;
	}

	return false;
}

bool MText::ReadDatad(int nCode, double dData)
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
		m_dArrTemp[2] = dData;
		memcpy(m_dInsertionPoint,m_dArrTemp,sizeof(double)*3);
		return true;

	case 11:
		m_dArrTemp[0] = dData;
		return true;

	case 21:
		m_dArrTemp[1] = dData;
		return true;

	case 31:
		m_dArrTemp[2] = dData;
		SetXAxisVector(m_dArrTemp[0], m_dArrTemp[1], m_dArrTemp[2]);
		return true;

	case 40:
		m_dHeight = dData;
		return true;

	case 41:
		m_dAreaWidth = dData;
		return true;

	case 44:
		m_dLineSpace = dData;
		return true;

	case 50:
		m_dAngle = dData;
		return true;

	case 1040:
		m_dAreaHeight = dData;
		return true;

	case 210:
		m_dArrTemp[0] = dData;
		return true;

	case 220:
		m_dArrTemp[1] = dData;
		return true;

	case 230:
		m_dArrTemp[2] = dData;
		memcpy(m_vNormal.m_pt,m_dArrTemp,sizeof(double)*3);
		SetZAxisVector();
		return true;
	}

	return false;
}

// XÃà¹æÇâ º¤ÅÍÀÇ ¼¼ ÁÂÇ¥°ª
void MText::SetXAxisVector(double x, double y, double z)
{
	m_vecAxis[0].m_pt[0] = x;
	m_vecAxis[0].m_pt[1] = y;
	m_vecAxis[0].m_pt[2] = z;

	SetZAxisVector();
}

void MText::SetZAxisVector()
{
	m_vecAxis[2] = m_vNormal;
	m_vecAxis[1] = ::CrossProduct(m_vecAxis[2], m_vecAxis[0]);
}

bool MText::ReadDatas(int nCode, wchar_t* strData)
{
	bool bResult = __super::ReadDatas(nCode,strData);
	if (bResult) return bResult;

	switch (nCode)
	{
	case 1:
		SetText(strData);
		//m_strData = strData;
		return true;

	case 7:
		m_strStyleName = strData;
		return true;

	case 1005:
		{
			int nHandle = 0;
			
			if (UnE::Utility::StringManager::HexToInt(strData, &nHandle))
				m_pMgr->AddTempMText(this, nHandle);
		}
		return true;
	}

	return false;
}

static std::wstring GetTextToken(wchar_t* strData, int& nBeginIndex, int len)
{
	if (nBeginIndex + 1 >= len || strData[nBeginIndex] != '\\' || strData[nBeginIndex + 1] != 'f')
	{
		nBeginIndex = len;
		return L"";
	}

	int nIndex = -1;

	for (int i = nBeginIndex; i < len; i++)
	{
		if (nIndex < 0)
		{
			if (strData[i] == ';')
				nIndex = i + 1;
		}
		else
		{
			if (i == len - 1)
			{
				if (nIndex >= 0)
				{
					nBeginIndex = len;
					wchar_t ch = strData[i];
					strData[i] = 0;

					std::wstring str = &strData[nIndex];
					strData[i] = ch;
					return str;
				}
			}
			else
			{
				if (strData[i] == '\\' && strData[i + 1] == 'f')
				{
					nBeginIndex = i;
					strData[i] = 0;

					std::wstring str = &strData[nIndex];
					strData[i] = '\\';
					return str;
				}
			}
		}
	}

	nBeginIndex = len;
	return L"";
}

static std::wstring GetToken(wchar_t* strData, int nBeginIndex, int nEndIndex)
{
	std::wstring str = L"";

	for (int i = nBeginIndex; i <= nEndIndex; i++)
	{
		str += strData[i];
	}

	return str;
}

static void SplitText(wchar_t* strData, std::list<std::wstring>& tokens)
{
	int nBeginIndex = 0;
	bool beginBlock = false;
	int len = (int)wcslen(strData);

	for (int i = 0; i < len; i++)
	{
		if (beginBlock == false)
		{
			if (i >= 2 && strData[i] == 'f' && strData[i - 1] == '\\' && strData[i - 2] == '{')
			{
				if (i - 2 > nBeginIndex)
				{
					std::wstring str = GetToken(strData, nBeginIndex, i - 3);
					tokens.push_back(str);
				}

				nBeginIndex = i - 2;
				beginBlock = true;
			}
		}
		else
		{
			if (i > nBeginIndex + 2 && strData[i] == '}' && strData[i] != '\\')
			{
				std::wstring str = GetToken(strData, nBeginIndex, i);
				tokens.push_back(str);

				nBeginIndex = i + 1;
				beginBlock = false;
			}
		}
	}

	if (nBeginIndex < len)
	{
		std::wstring str = GetToken(strData, nBeginIndex, len - 1);
		tokens.push_back(str);
	}
}

static std::wstring TokenToString(wchar_t* strData)
{
	std::wstring strResult;

	int len = (int)wcslen(strData);

	if (len < 4)
	{
		return strData;
	}

	if (strData[0] == '{' && strData[1] == '\\' && strData[2] == 'f' && strData[len - 1] == '}')
	{
		strResult = L"";
		int nBeginIndex = 1;

		while (nBeginIndex < len)
		{
			std::wstring str = GetTextToken(strData, nBeginIndex, len);
			strResult += str;
		}
	}
	else
		strResult = strData;

	return strResult;
}

void MText::SetText(wchar_t* strData)
{
	MText* pFirstText = m_pMgr->FindFirstMText(this);

	int len = (int)wcslen(strData);

	if (len < 4)
	{
		m_strData = strData;

		if (pFirstText != 0)
		{
			pFirstText->m_strData += m_strData;
		}

		return;
	}

	std::list<std::wstring> tokens;
	SplitText(strData, tokens);

	m_strData = L"";

	for (std::list<std::wstring>::iterator iter = tokens.begin(); iter != tokens.end(); iter++)
	{
		std::wstring& str = *iter;
		m_strData += TokenToString((wchar_t*)str.c_str());
	}

	/*if (strData[0] == '{' && strData[1] == '\\' && strData[2] == 'f' && strData[len - 1] == '}')
	{
		m_strData = L"";
		int nBeginIndex = 1;

		while (nBeginIndex < len)
		{
			std::wstring str = GetTextToken(strData, nBeginIndex, len);
			m_strData += str;
		}
	}
	else
		m_strData = strData;*/

	if (pFirstText != 0)
	{
		pFirstText->m_strData += m_strData;
	}
}

void MText::GetAttachment(int* pHorizon, int* pVertical)
{
	switch (m_nAttachmentPoint)
	{
	case 1:		// Top Left
		*pHorizon  = 0;
		*pVertical = 0;
		break;

	case 2:		// Top Center
		*pHorizon  = 1;
		*pVertical = 0;
		break;

	case 3:		// Top Right
		*pHorizon  = 2;
		*pVertical = 0;
		break;

	case 4:		// Middle Left
		*pHorizon  = 0;
		*pVertical = 1;
		break;

	case 5:		// Middle Center
		*pHorizon  = 1;
		*pVertical = 1;
		break;

	case 6:		// Middle Right
		*pHorizon  = 2;
		*pVertical = 1;
		break;

	case 7:		// Bottom Left
		*pHorizon  = 0;
		*pVertical = 2;
		break;

	case 8:		// Bottom Center
		*pHorizon  = 1;
		*pVertical = 2;
		break;

	case 9:		// Bottom Right
		*pHorizon  = 2;
		*pVertical = 2;
		break;
	}
}

void MText::SetAttachment(int nAttachmentPoint)
{
	m_nAttachmentPoint = nAttachmentPoint;
}

void MText::SetDrawingDirection(int nDrawingDirection)
{
	m_nDrawingDirection = nDrawingDirection;
}

int MText::GetDrawingDirection()
{
	return m_nDrawingDirection;
}

void MText::SetInsertionPoint(double x, double y, double z)
{
	m_dInsertionPoint[0] = x;
	m_dInsertionPoint[1] = y;
	m_dInsertionPoint[2] = z;
}

void MText::GetInsertionPoint(double* pX, double* pY, double* pZ)
{
	*pX = m_dInsertionPoint[0];
	*pY = m_dInsertionPoint[1];
	*pZ = m_dInsertionPoint[2];
}

void MText::SetArea(double dAreaWidth, double dAreaHeight)
{
	m_dAreaWidth  = dAreaWidth;
	m_dAreaHeight = dAreaHeight;
}

void MText::GetArea(double* pAreaWidth, double* pAreaHeight)
{
	*pAreaWidth  = m_dAreaWidth;
	*pAreaHeight = m_dAreaHeight;
}

void MText::SetHeight(double dHeight)
{
	m_dHeight = dHeight;
}

double MText::GetHeight()
{
	return m_dHeight;
}

void MText::SetNormalVector(double dAxisX, double dAxisY, double dAxisZ)
{
	m_vNormal.m_pt[0] = dAxisX;
	m_vNormal.m_pt[1] = dAxisY;
	m_vNormal.m_pt[2] = dAxisZ;
}

void MText::GetNormalVector(double* pX, double* pY, double* pZ)
{
	*pX = m_vNormal.m_pt[0];
	*pY = m_vNormal.m_pt[1];
	*pZ = m_vNormal.m_pt[2];
}

wchar_t* MText::GetString()
{
	return (wchar_t*)m_strData.data();
}

void MText::SetString(wchar_t* strData)
{
	m_strData = strData;
}

void MText::SetLineSpace(double dLineSpace)
{
	m_dLineSpace = dLineSpace;
}

double MText::GetLineSpace()
{
	return m_dLineSpace;
}

// Radian
double MText::GetTextAngle() const
{
	return m_dAngle;
}

void MText::SetStyleName(const wchar_t* strStyleName)
{
	m_strStyleName = strStyleName;
}

const wchar_t* MText::GetStyleName()
{
	return m_strStyleName.data();
}

END_NS
END_NS
