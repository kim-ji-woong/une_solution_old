#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(OBJECTS)

std::wstring Layout::m_strSubClassName = L"AcDbPlotSettings";

Layout::Layout(ObjectManager* pMgr)
	: Object(pMgr)
{
	m_strLayoutName = L"";
	m_nOrder = 0;
	m_nBlockHandle = 0;

	Init();
}

Layout::Layout(wchar_t* strLayoutName, int nOrder, int nBlockHandle, ObjectManager* pMgr)
	: Object(pMgr)
{
	m_strLayoutName = strLayoutName;
	m_nOrder		= nOrder;
	m_nBlockHandle  = nBlockHandle;

	Init();
}

Layout::~Layout(void)
{
}

void Layout::Init()
{
	m_strEntityType = L"LAYOUT";
	m_pPlotSettings = 0;

	m_readNumerator = m_readDenominator = false;

	AddData(0,(wchar_t*)m_strEntityType.data());
	AddData(5,&m_nHandle);
}

wchar_t* Layout::GetSubClassName()
{
	return (wchar_t*)m_strSubClassName.data();
}

void Layout::AddPlotSettings(PlotSettings* pPlot)
{
	m_pPlotSettings = pPlot;
}

PlotSettings* Layout::GetPlotSettings()
{
	return m_pPlotSettings;
}

void Layout::SetData()
{
	int nFlag = 1;
	double dMinimumLimitsX = 0.0, dMinimumLimitsY = 0.0;
	double dMaximumLimitsX = 0.0, dMaximumLimitsY = 0.0;
	double dInsertBasePosX = 0.0, dInsertBasePosY = 0.0, dInsertBasePosZ = 0.0;
	double dMinimumExtentsX = 0.0, dMinimumExtentsY = 0.0, dMinimumExtentsZ = 0.0;
	double dMaximumExtentsX = 0.0, dMaximumExtentsY = 0.0, dMaximumExtentsZ = 0.0;
	double dElevation = 0.0;
	double dUCSOriginX = 0.0, dUCSOriginY = 0.0, dUCSOriginZ = 0.0;
	Utility::Vertex3D vUCSAxisX(1.0,0.0,0.0), vUCSAxisY(0.0,1.0,1.0);
	int nOrthoType = 0;

	AddData(102,L"{ACAD_REACTORS");
	AddData(330,&m_nDictionaryHandle);
	AddData(102,L"}");
	AddData(330,&m_nDictionaryHandle);
	AddData(100,(wchar_t*)m_strSubClassName.data());
	AddData(1,L"");

	if (m_pPlotSettings)
	{
		std::list<DXFData>::const_iterator p = m_pPlotSettings->m_list.begin();
		int nCount = 0;

		while (p != m_pPlotSettings->m_list.end())
		{
			if (nCount++ > 3)
			{
				DXFData data = *p;
				AddData(data);
			}

			++p;
		}
	}

	AddData(100,L"AcDbLayout");
	AddData(1,(wchar_t*)m_strLayoutName.data());
	AddData(70,&nFlag);
	AddData(71,&m_nOrder);
	AddData(10,&dMinimumLimitsX);
	AddData(20,&dMinimumLimitsY);
	AddData(11,&dMaximumLimitsX);
	AddData(21,&dMaximumLimitsY);
	AddData(12,&dInsertBasePosX);
	AddData(22,&dInsertBasePosY);
	AddData(32,&dInsertBasePosZ);
	AddData(14,&dMinimumExtentsX);
	AddData(24,&dMinimumExtentsY);
	AddData(34,&dMinimumExtentsZ);
	AddData(15,&dMaximumExtentsX);
	AddData(25,&dMaximumExtentsY);
	AddData(35,&dMaximumExtentsZ);
	AddData(146,&dElevation);
	AddData(13,&dUCSOriginX);
	AddData(23,&dUCSOriginY);
	AddData(33,&dUCSOriginZ);
	AddData(16,&vUCSAxisX.m_pt[0]);
	AddData(26,&vUCSAxisX.m_pt[1]);
	AddData(36,&vUCSAxisX.m_pt[2]);
	AddData(17,&vUCSAxisY.m_pt[0]);
	AddData(27,&vUCSAxisY.m_pt[1]);
	AddData(37,&vUCSAxisY.m_pt[2]);
	AddData(76,&nOrthoType);
	AddData(330,&m_nBlockHandle);
}

int Layout::GetBlockHandle()
{
	return m_nBlockHandle;
}

wchar_t* Layout::GetLayoutName()
{
	return (wchar_t*)m_strLayoutName.data();
}

void Layout::ReadDatai(int nCode, int nData)
{
	if (nCode == 5)
		m_nBlockHandle = nData;
	else if (nCode == 72)
	{
		if (m_pPlotSettings)
			m_pPlotSettings->SetPaperUnits(nData);
	}
}

void Layout::ReadDatad(int nCode, double dData)
{
	if (nCode == 142)
	{
		m_dNumerator = dData;
		m_readNumerator = true;

		if (m_readDenominator && m_pPlotSettings)
			m_pPlotSettings->SetPrintScale(m_dNumerator, m_dDenominator);
	}
	else if (nCode == 143)
	{
		m_dDenominator = dData;
		m_readDenominator = true;

		if (m_readDenominator && m_pPlotSettings)
			m_pPlotSettings->SetPrintScale(m_dNumerator, m_dDenominator);
	}
}

void Layout::ReadDatas(int nCode, wchar_t* strData)
{
	if (nCode == 100)
	{
		if (!wcscmp(strData, L"AcDbPlotSettings"))
			m_pPlotSettings = new PlotSettings(m_pMgr);
	}
	else if (nCode == 1)
	{
		m_strLayoutName = strData;
	}
}

END_NS
END_NS
