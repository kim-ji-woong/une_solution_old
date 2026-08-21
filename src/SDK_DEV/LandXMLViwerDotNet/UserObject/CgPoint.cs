using System;

namespace UserCtrls
{
	/// <summary>
	/// Summary description for CgPoint.
	/// </summary>
	public class CgPoint
	{
		string strName = "";
		string strParent = "";
		
		double xcoord;
		double ycoord;
		double zcoord;


		public enum PointType
		{
			NONE,
			COGO,
			P,
			PVI
		}
		private PointType pType = PointType.NONE;

		public enum DTMType
		{
			NONE,
			RANDOM
		}
		private DTMType dtmType = DTMType.NONE;

		public CgPoint()
		{
		}
		public CgPoint(double x, double y, double z = 0.0)
		{
			this.xcoord = x;
			this.ycoord = y;
			this.zcoord = z;
		}
		public CgPoint(string sname, double x, double y, double z = 0.0)
		{
			this.strName = sname;
			this.xcoord = x;
			this.ycoord = y;
			this.zcoord = z;
		}

		#region Properties
		public string ParentName
		{
			get
			{
				return this.strParent;
			}
			set
			{
				this.strParent = value;
			}
		}
		public string PointName
		{
			get
			{
				return this.strName;
			}
			set
			{
				this.strName = value;
			}
		}
		public double XCoordinate
		{
			get
			{
				return this.xcoord;
			}
			set
			{
				this.xcoord = value;
			}
		}
		public double YCoordinate
		{
			get
			{
				return this.ycoord;
			}
			set
			{
				this.ycoord = value;
			}
		}
		public double ZCoordinate
		{
			get
			{
				return this.zcoord;
			}
			set
			{
				this.zcoord = value;
			}
		}
		public PointType PType
		{
			get
			{
				return this.pType;
			}
			set
			{
				this.pType = value;
			}
		}
		#endregion
	}
}
