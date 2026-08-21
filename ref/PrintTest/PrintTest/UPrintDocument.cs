using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Drawing.Printing
{
	public class UPrintDocument : PrintDocument
	{
		public enum LengthUnit
		{
			mm = 1,
			inch = 2,
			none = 3
		}

		protected double dOffsetX = 1.0f;
		public double OffsetX
		{
			get { return dOffsetX; }
			set { dOffsetX = value; }
		}

		protected double dOffsetY = 1.0f;
		public double OffsetY
		{
			get { return dOffsetY; }
			set { dOffsetY = value; }
		}

		protected LengthUnit mUnit = LengthUnit.mm;
		public LengthUnit LengthOfUnit
		{
			get { return mUnit; }
			set { mUnit = value; }
		}

		protected double length = 1.0f;
		public double Length
		{
			get { return length; }
			set { length = value; }
		}

		protected double unitLength = 1.0;
		public double UnitValue
		{
			get { return unitLength; }
			set { unitLength = value; }
		}

		protected bool m_bPrintOnPaperCenter = false;
		public bool PrintOnCenter
		{
			get { return m_bPrintOnPaperCenter; }
			set { m_bPrintOnPaperCenter = value; }
		}

		protected bool m_bFitToPage = false;
		public bool FitToPage
		{
			get { return m_bFitToPage; }
			set { m_bFitToPage = value; }
		}

		protected bool m_bUpsideDown = false;
		public bool UpsideDown
		{
			get { return m_bUpsideDown; }
			set { m_bUpsideDown = value; }
		}

		protected bool m_bLandscape = false;
		public bool Landscape
		{
			get { return m_bLandscape; }
			set { m_bLandscape = value; }
		}

		protected Size mDrawingSize = new Size();
		public Size DrawingSize
		{
			get { return mDrawingSize; }
			set { mDrawingSize = value; }
		}


		
	}
}
