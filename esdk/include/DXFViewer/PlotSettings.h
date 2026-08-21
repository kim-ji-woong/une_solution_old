#pragma once
#include "Shape.h"

namespace DXFViewer
{
	public ref class PlotSettings
	{
	public:
		enum class PlotPaperUnits { INCHES = 0, MILLIMETERS = 1, PIXELS = 2, TYPE_COUNT };

	public:
		PlotSettings();
		~PlotSettings();

	public:
		property PlotPaperUnits PlotPaperUnit
		{
			PlotPaperUnits get() { return m_plotPaperUnits; }
			void set(PlotPaperUnits value) { m_plotPaperUnits = value; }
		}

	public:
		// dNumerator : 분자
		// dDenominator : 분모
		void SetPrintScale(double dNumerator, double dDenominator);
		bool GetPrintScale([System::Runtime::InteropServices::OutAttribute] double% dNumerator, [System::Runtime::InteropServices::OutAttribute] double% dDenominator);
		
	private:
		double m_dPrintScaleNumerator;
		double m_dPrintScaleDenominator;
		PlotPaperUnits m_plotPaperUnits;
	};
}
