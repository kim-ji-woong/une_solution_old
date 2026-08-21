using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.Common;
using Oracle.DataAccess.Client;
using System.IO;
using UserCtrls;
using System.Diagnostics;

namespace UBMLViewer
{
    public class OracleManager
    {
		private int nCgPointID = 1;
		private int nParcelID = 1;

		private DbConnection m_Connection = null;
        private bool m_isConnection = false;
       
        // 개발모드일 경우 실제 Oracle DB가 아닌 Excel 파일에서 데이터를 얻어온다.
        private bool m_devMode = false;

        private static char[] m_arrTrim = new char[] { ' ', '\t', '\r', '\n' };


		string ProviderName = "Oracle.DataAccess.Client";
		DbProviderFactory factory = null;
		
        public OracleManager(string strID, string strPW, string strDataSource)
        {
            if (!m_devMode)
            {
				factory = DbProviderFactories.GetFactory(ProviderName);
				m_Connection = factory.CreateConnection();
				m_Connection.ConnectionString = string.Format("User Id={0}; Password={1}; Data Source={2};",
                                                                 strID,
                                                                 strPW,
                                                                 strDataSource);
            }
        }

        public bool OpenConnection()
        {
            if (m_devMode)
                return true;

            if (m_isConnection)
                return true;

            try
            {
                m_Connection.Open();
                m_isConnection = true;
                return true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }

            return false;
        }

        public void CloseConnection()
        {
            if (m_devMode)
                return;

            if (!m_isConnection)
                return;

            m_Connection.Close();
            m_isConnection = false;
        }   

        private void TrimString(ref string str)
        {
            str = str.TrimStart(m_arrTrim);
            str = str.TrimEnd(m_arrTrim);
        }

		private void CreateTable()
		{
			string szSql1 = "CREATE TABLE LX_CG_POINT (point_id NUMBER PRIMARY KEY, name VARCHAR2(32), parent VARCHAR2(32),point MDSYS.sdo_point_type);";
			string szSql2 = "CREATE TABLE LX_PARCEL (parcel_id NUMBER PRIMARY KEY,name VARCHAR2(32),parent NUMBER,boundary MDSYS.SDO_GEOMETRY);";
			string szSql3 = "INSERT INTO user_sdo_geom_metadata VALUES ('LX_PARCEL','boundary',MDSYS.SDO_DIM_ARRAY(MDSYS.SDO_DIM_ELEMENT('X', -1000000, 1000000, 0.001), MDSYS.SDO_DIM_ELEMENT('Y', -1000000, 1000000, 0.001)), NULL);";
		}
		
		public void InsertData(CgPoint point)
		{
			//return;

			if (point == null)
				return;

			string szName = point.PointName;
			if( szName == null)
				szName = "";

			string szParentName = point.ParentName;
			if( szParentName == null)
				szParentName = "";

			string szSql2 = string.Format("INSERT INTO LX_CG_POINT VALUES( {0}, '{1}', '{2}', MDSYS.SDO_POINT_TYPE( {3}, {4}, {5}))",
				nCgPointID++, szName, szParentName, point.XCoordinate, point.YCoordinate, point.ZCoordinate);

			try
			{
				DbCommand cmd = m_Connection.CreateCommand();
				cmd.CommandText = szSql2;
				cmd.ExecuteScalar();

			}
			catch (System.Exception ex)
			{
				Debug.WriteLine(szSql2);
				Debug.WriteLine(ex.Message);
				Debug.WriteLine(ex.StackTrace);
			}
		}

		public void InsertData(Parcel parcel)
		{
			//return;

			if (parcel == null)
				return;

			int nParentID = nParcelID++;
			bool bFirst = true;

			string szName = parcel.Name;
			if( szName == null)
				szName = "";
			string szSql2 = string.Format("INSERT INTO LX_PARCEL VALUES({0},'{1}',{2}, null)", nParentID, szName, -1);


			DbCommand cmd = null;
			try
			{
				cmd = m_Connection.CreateCommand();
				cmd.CommandText = szSql2;
				cmd.ExecuteScalar();

			}
			catch (System.Exception ex)
			{
				Debug.WriteLine(szSql2);
				Debug.WriteLine(ex.Message);
				Debug.WriteLine(ex.StackTrace);
			}


			IEnumerator enParcel = parcel.GetElementEnumerator();
			while (enParcel.MoveNext())
			{
				string szSql3 = "";
				try
				{
					GeometryElement element = (GeometryElement)enParcel.Current;
					if (element.DrawElementType == GeometryElement.ElementType.LINESTRING)
					{
						int nID = nParcelID++;
						int nPid = nParcelID;
						szName = "Parcel" + nID.ToString();

						szSql3 = string.Format("INSERT INTO LX_PARCEL VALUES({0},'{1}',{2}, " +
							"MDSYS.SDO_GEOMETRY (2003,NULL,NULL,MDSYS.SDO_ELEM_INFO_ARRAY(1,1003,1, 19, 2003,1),MDSYS.SDO_ORDINATE_ARRAY(", nID, szName, nPid);

						int nCount = element.GetPointsCount();
						for (int i = 0; i < nCount; i++)
						{
							if (i != 0)
							{
								szSql3 += ",";
							}

							CgPoint point = element.GetPointAt(i);
							if (point != null)
								szSql3 += string.Format("{0},{1}", point.XCoordinate, point.YCoordinate);
						}
						szSql3 += ")))";

						cmd.CommandText = szSql3;
						cmd.ExecuteScalar();
					}
					

				}
				catch (System.Exception ex)
				{
					Debug.WriteLine(szSql3);
					Debug.WriteLine(ex.Message);
					Debug.WriteLine(ex.StackTrace);
				}
			}
			
		}
    }
}
