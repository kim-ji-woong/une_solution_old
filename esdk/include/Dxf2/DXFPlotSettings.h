#pragma once
#include "DXFObject.h"

namespace DXF
{
	namespace OBJECTS
	{
		class ObjectManager;
		class Layout;

		enum PlotLayoutFlag {PlotViewportBorders = 1,
							ShowPlotStyles = 2,
							PlotCentered = 4,
							PlotHidden = 8,
							UseStandardScale = 16,
							PlotPlotStyles = 32,
							ScaleLineweights = 64,
							PrintLineweights = 128,
							DrawViewportsFirst = 512,
							ModelType = 1024,
							UpdatePaper = 2048,
							ZoomToPaperOnUpdate = 4096,
							Initializing = 8192,
							PrevPlotInit = 16384
							};

		enum PlotPaperUnits {INCHES = 0, MILLIMETERS = 1, PIXELS = 2};

		enum PlotRotation {NoRotation = 0, _90DegreeCounterClockWise = 1,
							UpsideDown = 2, _90DegreeClockWise = 3};

		enum PlotType {LastScreenDisplay = 0,
						DrawingExtents = 1,
						DrawingLimits = 2,
						ViewSpecifiedByCode6 = 3,
						WindowSpecified = 4,
						LayoutInformation = 5
						};

		class PlotSettings : public Object
		{
		public:
			PlotSettings(ObjectManager* pMgr);
			virtual ~PlotSettings(void);
			friend class Layout;

		public:
			void SetData();
			void SetPlotViewName(wchar_t* strViewName);
			void SetPageSetupName(wchar_t* strPageSetupName);
			void SetDevicePath(wchar_t* strDevicePath);
			void SetRotation(int nRotation);
			void SetPlotType(int nType);
			void SetCurrentStyleSheet(wchar_t* strCurrentStyleSheet);
			void SetLayoutFlag(int nFlag);
			void SetScaleType(int nType);
			void SetFloatingPointScale(double dScale);
			void SetPaperImageOrigin(double dOriginX, double dOriginY);
			void SetPaperUnits(int nUnits);
			void SetPaperSize(double dWidth, double dHeight);
			void SetMargin(double dLeft, double dRight, double dTop, double dBottom);
			void SetOrigin(double dX, double dY);
			void SetWindowArea(double dBottomLeftX, double dBottomLeftY, double dTopRightX, double dTopRightY);
			// dScaleNumerator : 분자
			// dScaleDenominator : 분모
			void SetPrintScale(double dScaleNumerator, double dScaleDenominator);

			// dScaleNumerator : 분자
			// dScaleDenominator : 분모
			void GetPrintScale(double* pScaleNumerator, double* pScaleDenominator);
			int GetPaperUnits() const;

		protected:
			void Init();

		protected:
			int m_nHandle;
			int m_nSoftPointer;
			std::wstring m_strPageSetupName;
			std::wstring m_strDevicePath;
			double m_dPaperWidth, m_dPaperHeight;
			int m_nPlotLayoutFlag;
			std::wstring m_strPlotViewName;
			int m_nPlotRotation;
			int m_nPlotType;
			std::wstring	m_strCurrentStyleSheet;
			int m_nStandardScaleType;
			double m_dFloatingPointScale;
			double m_dPaperImageOriginX;
			double m_dPaperImageOriginY;
			int m_nPaperUnits;
			double m_dLeftMargin;
			double m_dBottomMargin;
			double m_dRightMargin;
			double m_dTopMargin;
			double m_dPlotOriginX;
			double m_dPlotOriginY;
			double m_dPlotWindowAreaBL[2];
			double m_dPlotWindowAreaTR[2];
			double m_dPrintScaleNumerator;
			double m_dPrintScaleDenominator;
		};
	}
}
