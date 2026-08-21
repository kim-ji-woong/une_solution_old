#include "stdafx.h"

BEGIN_NS(DXF)
BEGIN_NS(OBJECTS)

PlotSettings::PlotSettings(ObjectManager* pMgr)
: Object(pMgr)
{
	Init();
}

PlotSettings::~PlotSettings(void)
{
}

void PlotSettings::SetPageSetupName(wchar_t* strPageSetupName)
{
	m_strPageSetupName = strPageSetupName;
}

void PlotSettings::SetDevicePath(wchar_t* strDevicePath)
{
	m_strDevicePath = strDevicePath;
}

void PlotSettings::Init()
{
	if (m_pMgr != 0)
	{
		DXFManager* pDXFMgr = m_pMgr->GetOwner();

		if (pDXFMgr != 0)
		{
			m_nHandle = pDXFMgr->Get32BitHandle();
			m_nSoftPointer = pDXFMgr->Get32BitHandle();
		}
	}

	m_strEntityType = L"PLOTSETTINGS";
	//m_nHandle = Get32BitHandle();
	//m_nSoftPointer = Get32BitHandle();
	m_strPageSetupName = L"";
	m_strDevicePath = L"";
	SetPaperSize(0.0,0.0);
	SetPlotViewName(L"");
	SetMargin(0.0,0.0,0.0,0.0);
	SetOrigin(0.0,0.0);
	SetWindowArea(0.0,0.0,0.0,0.0);
	SetPrintScale(1.0,1.0);
	SetLayoutFlag(UseStandardScale | PlotPlotStyles | PrintLineweights | DrawViewportsFirst);
	SetPaperUnits(INCHES);
	SetRotation(NoRotation);
	SetPlotType(LayoutInformation);
	SetCurrentStyleSheet(L"");
	SetScaleType(16);
	SetFloatingPointScale(1.0);
	SetPaperImageOrigin(0.0,0.0);
}

void PlotSettings::SetPlotViewName(wchar_t* strViewName)
{
	m_strPlotViewName = strViewName;
}

void PlotSettings::SetRotation(int nRotation)
{
	m_nPlotRotation = nRotation;
}

void PlotSettings::SetPlotType(int nType)
{
	m_nPlotType = nType;
}

void PlotSettings::SetCurrentStyleSheet(wchar_t* strCurrentStyleSheet)
{
	m_strCurrentStyleSheet = strCurrentStyleSheet;
}

void PlotSettings::SetLayoutFlag(int nFlag)
{
	m_nPlotLayoutFlag = nFlag;
}

void PlotSettings::SetScaleType(int nType)
{
	m_nStandardScaleType = nType;
}

void PlotSettings::SetFloatingPointScale(double dScale)
{
	m_dFloatingPointScale = dScale;
}

void PlotSettings::SetPaperImageOrigin(double dOriginX, double dOriginY)
{
	m_dPaperImageOriginX = dOriginX;
	m_dPaperImageOriginY = dOriginY;
}

void PlotSettings::SetPaperUnits(int nUnits)
{
	m_nPaperUnits = nUnits;
}

void PlotSettings::SetPaperSize(double dWidth, double dHeight)
{
	m_dPaperWidth = dWidth;
	m_dPaperHeight = dHeight;
}

void PlotSettings::SetMargin(double dLeft, double dRight, double dTop, double dBottom)
{
	m_dLeftMargin = dLeft;
	m_dBottomMargin = dBottom;
	m_dRightMargin = dRight;
	m_dTopMargin = dTop;
}

void PlotSettings::SetOrigin(double dX, double dY)
{
	m_dPlotOriginX = dX;
	m_dPlotOriginY = dY;
}

void PlotSettings::SetWindowArea(double dBottomLeftX, double dBottomLeftY, double dTopRightX, double dTopRightY)
{
	m_dPlotWindowAreaBL[0] = dBottomLeftX;
	m_dPlotWindowAreaBL[1] = dBottomLeftY;
	m_dPlotWindowAreaTR[0] = dTopRightX;
	m_dPlotWindowAreaTR[1] = dTopRightY;
}

// dScaleNumerator : 분자
// dScaleDenominator : 분모
void PlotSettings::SetPrintScale(double dScaleNumerator, double dScaleDenominator)
{
	m_dPrintScaleNumerator = dScaleNumerator;
	m_dPrintScaleDenominator = dScaleDenominator;
}

void PlotSettings::SetData()
{
	wchar_t strPaperSize[32];

	if (m_dPaperWidth == 0.0 && m_dPaperHeight == 0.0)
	{
		wcscpy_s(strPaperSize, 32, L"");
	}
	else
	{
		swprintf_s(strPaperSize,L"ISO_A4_(%.2lf_x_%.2lf_MM)",m_dPaperWidth,m_dPaperHeight);
	}

	DXFData data[31];

	SetDXFData(0,L"PLOTSETTINGS",&data[0]);
	SetDXFData(5,&m_nHandle,&data[1]);
	SetDXFData(330,&m_nSoftPointer,&data[2]);
	SetDXFData(100,L"AcDbPlotSettings",&data[3]);
	SetDXFData(1,(wchar_t*)m_strPageSetupName.data(),&data[4]);
	SetDXFData(2,(wchar_t*)m_strDevicePath.data(),&data[5]);
	SetDXFData(4,strPaperSize,&data[6]);
	SetDXFData(6,(wchar_t*)m_strPlotViewName.data(),&data[7]);
	SetDXFData(40,&m_dLeftMargin,&data[8]);
	SetDXFData(41,&m_dBottomMargin,&data[9]);
	SetDXFData(42,&m_dRightMargin,&data[10]);
	SetDXFData(43,&m_dTopMargin,&data[11]);
	SetDXFData(44,&m_dPaperWidth,&data[12]);
	SetDXFData(45,&m_dPaperHeight,&data[13]);
	SetDXFData(46,&m_dPlotOriginX,&data[14]);
	SetDXFData(47,&m_dPlotOriginY,&data[15]);
	SetDXFData(48,&m_dPlotWindowAreaBL[0],&data[16]);
	SetDXFData(49,&m_dPlotWindowAreaBL[1],&data[17]);
	SetDXFData(140,&m_dPlotWindowAreaTR[0],&data[18]);
	SetDXFData(141,&m_dPlotWindowAreaTR[1],&data[19]);
	SetDXFData(142,&m_dPrintScaleNumerator,&data[20]);
	SetDXFData(143,&m_dPrintScaleDenominator,&data[21]);
	SetDXFData(70,&m_nPlotLayoutFlag,&data[22]);
	SetDXFData(72,&m_nPaperUnits,&data[23]);
	SetDXFData(73,&m_nPlotRotation,&data[24]);
	SetDXFData(74,&m_nPlotType,&data[25]);
	SetDXFData(7,(char*)m_strCurrentStyleSheet.data(),&data[26]);
	SetDXFData(75,&m_nStandardScaleType,&data[27]);
	SetDXFData(147,&m_dFloatingPointScale,&data[28]);
	SetDXFData(148,&m_dPaperImageOriginX,&data[29]);
	SetDXFData(149,&m_dPaperImageOriginY,&data[30]);

	for (int i=0;i<31;i++) m_list.push_back(data[i]);
}

// dScaleNumerator : 분자
// dScaleDenominator : 분모
void PlotSettings::GetPrintScale(double* pScaleNumerator, double* pScaleDenominator)
{
	*pScaleNumerator = m_dPrintScaleNumerator;
	*pScaleDenominator = m_dPrintScaleDenominator;
}

int PlotSettings::GetPaperUnits() const
{
	return m_nPaperUnits;
}

END_NS
END_NS
