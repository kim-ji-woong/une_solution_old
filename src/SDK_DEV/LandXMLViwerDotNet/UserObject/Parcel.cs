using System;
using System.Collections;

namespace UserCtrls
{
	/// <summary>
	/// Summary description for Parcel.
	/// </summary>
	public class Parcel
	{
		private string strId;
		private string strType;
		private bool bShow = true;
		private bool bInclude = true;

		private CgPoint centerPoint;
		private CoordRange range = new CoordRange();
		private ArrayList elements = new ArrayList();

		public Parcel()
		{
		}

		public int AddDrawElement(GeometryElement ele)
		{
			CoordRange rng = ele.CoordinateRange;
			this.range.CheckRangeCoordinates(rng);

			return elements.Add(ele);
		}

		public IEnumerator GetElementEnumerator()
		{
			return elements.GetEnumerator();
		}

		#region Properties
		public CgPoint CenterPoint
		{
			get
			{
				return this.centerPoint;
			}
			set
			{
				this.centerPoint = value;
			}
		}
		public string GeomType
		{
			get
			{
				return this.strType;
			}
			set
			{
				this.strType = value;
			}
		}
		public string Name
		{
			get
			{
				return this.strId;
			}
			set
			{
				this.strId = value;
			}
		}
		public CoordRange ObjectRange
		{
			get
			{
				return this.range;
			}
		}

		public bool Show
		{
			get
			{
				return this.bShow;
			}
			set
			{
				this.bShow = value;
			}
		}

		public bool Include
		{
			get
			{
				return this.bInclude;
			}
			set
			{
				this.bInclude = value;
			}
		}
		#endregion
	}
}
