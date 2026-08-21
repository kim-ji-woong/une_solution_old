#pragma once

namespace DXFDotNet
{
	public enum class LengthUnit : int
	{
		mm = 1,
		inch = 2,
		none = 3
	};

	public ref class UPrintDocument : public System::Drawing::Printing::PrintDocument
	{

	public:
		

		UPrintDocument();

		property double OffsetX
		{
			double get() { return dOffsetX; }
			void set(double value) { dOffsetX = value; }
		}
				
		property double OffsetY
		{
			double get() { return dOffsetY; }
			void set(double value) { dOffsetY = value; }
		}
				
		property LengthUnit LengthOfUnit
		{					
			LengthUnit get() { return mUnit; }
			void set(LengthUnit value) { mUnit = value; }
		}
		
		property double Length
		{
			double get(){ return length; }
			void set(double value){ length = value; }
		}

		
		property double UnitValue
		{
			double get(){ return unitLength; }
			void set(double value){ unitLength = value; }
		}

		
		property bool PrintOnCenter
		{
			bool get(){ return m_bPrintOnPaperCenter; }
			void set(bool value){ m_bPrintOnPaperCenter = value; }
		}

		
		property bool FitToPage
		{
				bool get(){ return m_bFitToPage; }
			void set(bool value){ m_bFitToPage = value; }
		}

		
		property bool UpsideDown
		{
			bool get(){ return m_bUpsideDown; }
			void set(bool value){ m_bUpsideDown = value; }
		}

		
		property bool Landscape
		{
			bool get() { return m_bLandscape; }
			void set(bool value){ m_bLandscape = value; }
		}

		
		property System::Drawing::Size^ DrawingSize
		{
			System::Drawing::Size^ get() { return mDrawingSize; }
			void set(System::Drawing::Size^ value) { mDrawingSize = value; }
		}


		property System::Drawing::Rectangle^ DrawingRectSize
		{
			System::Drawing::Rectangle^ get() { return mDrawingRect; }
			void set(System::Drawing::Rectangle^ value) { mDrawingRect = value; }
		}

		property bool WindowPrintMode
		{
			bool get() { return m_bRectMode; }
			void set(bool value){ m_bRectMode = value; }
		}


	protected:
		double dOffsetX;
		double dOffsetY;
		LengthUnit mUnit;
		System::Drawing::Size^ mDrawingSize;
		System::Drawing::Rectangle^ mDrawingRect;
		bool m_bLandscape;
		bool m_bUpsideDown;
		bool m_bFitToPage;
		bool m_bPrintOnPaperCenter;
		double unitLength;
		double length;

		bool m_bRectMode;
	};

}
