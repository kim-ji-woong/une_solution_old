using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UserCtrls;

namespace UBMLViewer
{
	public class Utils
	{
		public Utils()
		{
		}

		public static CgPoint GetSectionPoint(string str)
		{
			CgPoint dp = new CgPoint();
			char[] spaces = { ' ' };
			string strNormal = NormalizeString(str);
			string[] strCoords = strNormal.Split(spaces);
			double x = ConvertStringToDouble(strCoords[0]);
			double y = ConvertStringToDouble(strCoords[1]);

			dp.XCoordinate = x;
			dp.YCoordinate = y;

			return dp;
		}
		public static ArrayList GetDrawPoint3D(string str)
		{
			char[] spaces = { ' ' };
			string strNormal = NormalizeString(str);
			string[] strCoords = strNormal.Split(spaces);

			ArrayList arResult = new ArrayList();
			for (int i = 0; i < strCoords.Length; i += 3)
			{
				double y = ConvertStringToDouble(strCoords[i]);
				double x = ConvertStringToDouble(strCoords[i + 1]);
				double z = ConvertStringToDouble(strCoords[i + 2]);

				CgPoint dp = new CgPoint();
				dp.XCoordinate = x;
				dp.YCoordinate = y;
				dp.ZCoordinate = z;
				arResult.Add(dp);
			}
			return arResult;
		}

		public static CgPoint GetDrawPoint(string str)
		{
			CgPoint dp = new CgPoint();
			char[] spaces = { ' ' };
			string strNormal = NormalizeString(str);
			string[] strCoords = strNormal.Split(spaces);
			double x = ConvertStringToDouble(strCoords[1]);
			double y = ConvertStringToDouble(strCoords[0]);

			dp.XCoordinate = x;
			dp.YCoordinate = y;

			return dp;
		}

		public static string NormalizeString(string str)
		{
			string strTrimmed = str.Trim();
			strTrimmed = strTrimmed.Replace("\n", " ");
			strTrimmed = strTrimmed.Replace("\r", "");
			strTrimmed = strTrimmed.Replace("\t", "");

			return strTrimmed;
		}

		public static double ConvertStringToDouble(string str)
		{
			try
			{
				return double.Parse(str);
			}
			catch
			{
				return 0;
			}
		}
	}
}
