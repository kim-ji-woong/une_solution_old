using System;

namespace UserCtrls
{
	/// <summary>
	/// Summary description for CoordRange.
	/// </summary>
	public class CoordRange
	{
		double lowx;
		double lowy;
		double lowz;

		double extx;
		double exty;
		double extz;


		bool bIsXFirst = true;
		bool bIsYFirst = true;
		bool bIsZFirst = true;

		public CoordRange()
		{
			lowx = 0;
			lowy = 0;
			lowz = 0;

			extx = 0;
			exty = 0;
			extz = 0;
		}

		public bool IsInRange(double x, double y, double z = 0.0)
		{
			if(x >= lowx && x <= extx)
			{
				if(y >= lowy && y <= exty)
				{
					if( z >= lowz && z <= extz)
						return true;
					return false;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		public void ExpandRange(double dPercent)
		{
			double xdiff = this.extx - this.lowx;
			double ydiff = this.exty - this.lowy;
			double zdiff = this.extz - this.lowz;

			double xIncr = xdiff * (dPercent / 100.0);
			double yIncr = ydiff * (dPercent / 100.0);
			double zIncr = zdiff * (dPercent / 100.0);

			lowx -= xIncr;
			lowy -= yIncr;
			lowz -= zIncr;

			extx += xIncr;
			exty += yIncr;
			extz += zIncr;
		}

		public void CheckPointRange(ref CgPoint pt)
		{
			CheckXCoordinate(pt.XCoordinate);
			CheckYCoordinate(pt.YCoordinate);
			CheckYCoordinate(pt.ZCoordinate);
		}
		public void CheckRangeCoordinates(CoordRange range)
		{
			CheckXCoordinate(range.OriginX);
			CheckYCoordinate(range.OriginY);
			CheckZCoordinate(range.OriginZ);

			CheckXCoordinate(range.ExtentsX);
			CheckYCoordinate(range.ExtentsY);
			CheckZCoordinate(range.ExtentsZ);
		}

		public void CheckCoordinates(double x, double y, double z = 0.0)
		{
			CheckXCoordinate(x);
			CheckYCoordinate(y);
			CheckYCoordinate(z);
		}
		public void CheckXCoordinate(double dX)
		{
			if(bIsXFirst)
			{
				lowx = dX;
				extx = dX;

				bIsXFirst = false;
			}
			else
			{
				if(dX < lowx)
				{
					lowx = dX;
				}
				else if(dX > extx)
				{
					extx = dX;
				}
			}
		}

		public void CheckYCoordinate(double dY)
		{
			if(bIsYFirst)
			{
				lowy = dY;
				exty = dY;

				bIsYFirst = false;
			}
			else
			{
				if(dY < lowy)
				{
					lowy = dY;
				}
				else if(dY > exty)
				{
					exty = dY;
				}
			}
		}

		public void CheckZCoordinate(double dZ)
		{
			if (bIsZFirst)
			{
				lowz = dZ;
				extz = dZ;

				bIsZFirst = false;
			}
			else
			{
				if (dZ < lowz)
				{
					lowz = dZ;
				}
				else if (dZ > extz)
				{
					extz = dZ;
				}
			}
		}

		#region
		public double OriginX
		{
			get
			{
				return this.lowx;
			}
		}
		public double OriginY
		{
			get
			{
				return this.lowy;
			}
		}
		public double OriginZ
		{
			get
			{
				return this.lowz;
			}
		}
		public double ExtentsX
		{
			get
			{
				return this.extx;
			}
		}
		public double ExtentsY
		{
			get
			{
				return this.exty;
			}
		}
		public double ExtentsZ
		{
			get
			{
				return this.extz;
			}
		}
		#endregion
	}
}
