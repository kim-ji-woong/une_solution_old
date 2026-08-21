using System;
using System.Collections;

namespace UserCtrls
{
	/// <summary>
	/// Summary description for GeometryElement.
	/// </summary>
	public class GeometryElement
	{
		string id;
		long ndx;

		bool bShowElement = true;

		CoordRange range;

		CgPoint startPoint;
		CgPoint endPoint;
		CgPoint centerPoint;

		double dRadius = 0;
		bool bIsCW = false;

		ArrayList pointsList = new ArrayList();

		public enum ElementType
		{
			NONE,
			POINT,
			XPOINT,
			MONUMENT,
			LINE,
			LINESTRING,
			ARC,
			SPIRAL
		}

		private ElementType eleType = ElementType.NONE;

		public GeometryElement()
		{
			range = new CoordRange();
		}

		#region Points methods
		public int AddDrawPoint(CgPoint pt)
		{
			range.CheckPointRange(ref pt);
			return pointsList.Add(pt);
		}

		public CgPoint GetPointAt(int i)
		{
			return (CgPoint) pointsList[i];
		}

		public int GetPointsCount()
		{
			return pointsList.Count;
		}
		#endregion

		#region Query into methods
		public bool HasID()
		{
			if(this.id != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion

		#region Properties
		public double Radius
		{
			get
			{
				return this.dRadius;
			}
			set
			{
				this.dRadius = value;
			}
		}
		public bool IsCW
		{
			get
			{
				return this.bIsCW;
			}
			set
			{
				this.bIsCW = value;
			}
		}
		public CoordRange CoordinateRange
		{
			get
			{
				return this.range;
			}
			set
			{
				this.range = value;
			}
		}
		public string ID
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}
		public long Index
		{
			get
			{
				return this.ndx;
			}
			set
			{
				this.ndx = value;
			}
		}
		public ElementType DrawElementType
		{
			get
			{
				return this.eleType;
			}
			set
			{
				this.eleType = value;
			}
		}
		public CgPoint StartPoint
		{
			get
			{
				return this.startPoint;
			}
			set
			{
				this.startPoint = value;
				range.CheckPointRange(ref startPoint);
			}
		}
		public CgPoint CenterPoint
		{
			get
			{
				return this.centerPoint;
			}
			set
			{
				this.centerPoint = value;
				range.CheckPointRange(ref centerPoint);
			}
		}
		public CgPoint EndPoint
		{
			get
			{
				return this.endPoint;
			}
			set
			{
				this.endPoint = value;
				range.CheckPointRange(ref endPoint);
			}
		}

		public bool ShowElement
		{
			get
			{
				return this.bShowElement;
			}
			set
			{
				this.bShowElement = value;
			}
		}
		#endregion
	}
}
