using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOPManager
{
	public class DataUtil
	{
		public static bool CheckChangedData(string strValue1, string strValue2)
		{
			if (strValue1 != strValue2)
			{
				UndoRedoManager.Instance.SaveSnapshot();
				return true;
			}
			return false;
		}

		public static bool CheckChangedData(int nValue1, int nValue2)
		{
			if (nValue1 != nValue2)
			{
				UndoRedoManager.Instance.SaveSnapshot();
				return true;
			}
			return false;
		}

		public static bool CheckChangedData(bool bValue1, bool bValue2)
		{
			if (bValue1 != bValue2)
			{
				UndoRedoManager.Instance.SaveSnapshot();
				return true;
			}
			return false;
		}

		public static bool CheckChangedData(object obj1, object obj2)
		{
			if (obj1 == null && obj2 != null)
			{
				UndoRedoManager.Instance.SaveSnapshot();
				return true;
			}

			if (obj1 != null && obj2 == null)
			{
				UndoRedoManager.Instance.SaveSnapshot();
				return true;
			}

			if (obj1.GetType() == obj2.GetType())
			{
				if (obj1 != obj2)
				{
					UndoRedoManager.Instance.SaveSnapshot();
					return true;
				}
			}
			return false;

		}
	}
}
