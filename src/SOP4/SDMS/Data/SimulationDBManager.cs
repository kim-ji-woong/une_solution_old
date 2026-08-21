using System;
using System.Collections;
using System.Data.SQLite;
using System.Linq;

namespace SDMS
{
	public class SimulationDBManager : DBUtility.WebDBManager, IDisposable
	{
		private SQLiteConnection m_connection = null;

		public SimulationDBManager(int nSiteID)
            : base(nSiteID)
		{
			MakeConnection();
		}

        public new void Dispose()
        {
            if(m_connection != null)
            {
                m_connection.Dispose();
            }
        }

		public void CloseLocalDB()
		{
			if (m_connection != null)
			{
				m_connection.Close();
                
				m_connection = null;
			}
		}

		private void MakeConnection()
		{
			string str = new string(new char[] { '9', '4', '4', '9', '9', '6', '6', 'A', 'b' });
			string strConnection = "Data Source=" + System.Windows.Forms.Application.StartupPath + "\\SOPSimulation.db3;Password=" + str;

			try
			{
				m_connection = new SQLiteConnection(strConnection);
				m_connection.Open();
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine(e.Message);
				m_connection = null;
			}
		}

		public override ArrayList GetResultData(string strSQLQuery, int nTransaction, string szDBName = null)
		{
			string strSQLLower = strSQLQuery.ToLower();

			// MS-SQL 문법을 SQLite 문법으로 변환
			ToSQLiteQuery(ref strSQLLower, ref strSQLQuery);

			return GetResultData(strSQLLower, strSQLQuery);
		}

		private void ToSQLiteQuery(ref string strSQLLower, ref string strSQLOrigin)
		{
			while (true)
			{
				string strTag = "dateadd";
				int nIndex = strSQLLower.IndexOf(strTag);

				if (nIndex < 0)
					break;

				int nFullLength = strSQLLower.Length;
				int nBeginIndex = -1, nEndIndex = -1, nOpenCount = 0;

				for (int i = nIndex + strTag.Length; i < nFullLength; i++)
				{
					char ch = strSQLLower.ElementAt(i);

					if (ch == '(')
					{
						if (nBeginIndex < 0)
							nBeginIndex = i;
						nOpenCount++;
					}
					else if (ch == ')')
					{
						nOpenCount--;

						if (nBeginIndex >= 0 && nOpenCount == 0)
						{
							nEndIndex = i;
							break;
						}
					}
				}

				if (nBeginIndex < 0 || nEndIndex < 0)
					break;

				string strDateAdd = strSQLLower.Substring(nIndex, nEndIndex - nIndex + 1);

				if (!ToDateTime(ref strDateAdd, nBeginIndex - nIndex))
					break;

				strSQLLower = strSQLLower.Substring(0, nIndex) + strDateAdd + strSQLLower.Substring(nEndIndex + 1);
				strSQLOrigin = strSQLOrigin.Substring(0, nIndex) + strDateAdd + strSQLOrigin.Substring(nEndIndex + 1);
			}

			while (true)
			{
				string strTag = "getdate()";
				int nTagLen = strTag.Length;

				int nIndex = strSQLLower.IndexOf(strTag);

				if (nIndex < 0)
					break;

				strSQLLower = strSQLLower.Substring(0, nIndex) + "datetime('now')" + strSQLLower.Substring(nIndex + nTagLen);
				strSQLOrigin = strSQLOrigin.Substring(0, nIndex) + "datetime('now')" + strSQLOrigin.Substring(nIndex + nTagLen);
			}
		}

		private bool ToDateTime(ref string strDateAdd, int nBeginIndex)
		{
			int nIndex1 = strDateAdd.IndexOf(',');
			int nIndex2 = strDateAdd.LastIndexOf(',');

			if (nIndex1 < 0 || nIndex2 < 0)
				return false;

			string strNumber = strDateAdd.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
			strNumber = strNumber.Trim();

			if (strNumber.Length == 0)
				return false;

			char ch = strNumber.ElementAt(0);

			if (ch != '+' && ch != '-')
				strNumber = "+" + strNumber;

			string strUnit = strDateAdd.Substring(nBeginIndex + 1, nIndex1 - nBeginIndex - 1);
			strUnit = strUnit.Trim();

			strDateAdd = "datetime('now', '" + strNumber + " " + strUnit + "')";
			return true;
		}

		private ArrayList GetResultData(string strSQLLower, string strSQLQuery)
		{
			ArrayList arrResult = new ArrayList();

			try
			{
				SQLiteCommand command = new SQLiteCommand(strSQLQuery, m_connection);

				if (strSQLLower.StartsWith("select "))
				{
					SQLiteDataReader reader = command.ExecuteReader();
					int nFieldCount = reader.FieldCount;

					while (reader.Read())
					{
						for (int i = 0; i < nFieldCount; i++)
						{
							if (reader.IsDBNull(i))
								arrResult.Add("");
							else
								arrResult.Add(reader[i].ToString());
						}
					}

					reader.Close();
				}
				else
					command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				System.Diagnostics.Trace.WriteLine(e.Message);
				return null;
			}

			return arrResult;
		}
	}
}