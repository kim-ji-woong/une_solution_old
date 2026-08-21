using System;
using System.Collections;
using UserCtrls;

namespace UBMLViewer
{
	/// <summary>
	/// Summary description for PointManager.
	/// </summary>
	public class PointManager
	{
		Hashtable cgPoints = new Hashtable();
		Hashtable grPoints = new Hashtable();

		public PointManager()
		{
		}

		public void ClearAll()
		{
			cgPoints = new Hashtable();
			grPoints = new Hashtable();
		}

		public void AddCgPoint(CgPoint drawPoint)
		{
			try
			{
				cgPoints.Add(drawPoint.PointName, drawPoint);
			}
			catch(Exception err)
			{
				string sError = err.Message;
			}
		}

		public CgPoint GetCgPoint(string sName)
		{
			return (CgPoint) cgPoints[sName];
		}

		public void AddGrPoint(CgPoint drawPoint)
		{
			try
			{
				grPoints.Add(drawPoint.ParentName + "." + drawPoint.PointName, drawPoint);
			}
			catch(Exception err)
			{
				string sError = err.Message;
			}
		}

		public CgPoint GetGrPoint(string sName)
		{
			return (CgPoint) grPoints[sName];
		}
	}
}

