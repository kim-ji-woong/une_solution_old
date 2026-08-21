#include "stdafx.h"
#include "UPrintDocument.h"

namespace DXFViewer
{	

	UPrintDocument::UPrintDocument()
	{
		dOffsetX = 10.0f;
		dOffsetY = 10.0f;
		mUnit = LengthUnit::mm;
		mDrawingSize = gcnew System::Drawing::Size();
		m_bLandscape = false;
		m_bUpsideDown = false;
		m_bFitToPage = true;
		m_bPrintOnPaperCenter = true;
		unitLength = 1.0;
		length = 1.0f;

		m_bRectMode = false;
	}

}