#include "StdAfx.h"
#include "PlotSettings.h"

BEGIN_NS(DXFViewer)

PlotSettings::PlotSettings(void)
{
	m_dPrintScaleNumerator = m_dPrintScaleDenominator = 0.0;
	m_plotPaperUnits = PlotPaperUnits::MILLIMETERS;
}

PlotSettings::~PlotSettings(void)
{
}

// dNumerator : 분자
// dDenominator : 분모
void PlotSettings::SetPrintScale(double dNumerator, double dDenominator)
{
	m_dPrintScaleNumerator = dNumerator;
	m_dPrintScaleDenominator = dDenominator;
}

bool PlotSettings::GetPrintScale([System::Runtime::InteropServices::OutAttribute] double% dNumerator, [System::Runtime::InteropServices::OutAttribute] double% dDenominator)
{
	dNumerator = dDenominator = 0.0;

	if (m_dPrintScaleNumerator == 0.0 || m_dPrintScaleDenominator == 0.0)
		return false;

	dNumerator = m_dPrintScaleNumerator;
	dDenominator = m_dPrintScaleDenominator;
	return true;
}

END_NS
