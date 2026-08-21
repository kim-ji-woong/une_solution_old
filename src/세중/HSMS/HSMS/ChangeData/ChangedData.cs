using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections;

namespace HSMS
{
    public abstract class ChangedData
    {
        public abstract bool Update(DBConn conn);
        public abstract void AddToManager(IChangedDataManager mgr);
        
        public const int UPDATE = 1;
        public const int DELETE = 2;
        public const int INSERT = 3;

        // Update시 Network으로 전송될 데이터를 저장할 공간
        protected ArrayList m_arrDatas = null;
        public ArrayList Datas
        {
            get { return m_arrDatas; }
            set { m_arrDatas = value; }
        }

        public int SQLType
        {
            get;
            set;
        }

        protected void AddQueryString(ref string strSQL, string strValue)
        {
            if (strSQL.Length == 0)
                strSQL = strValue;
            else
                strSQL += ", " + strValue;
        }


        //최대 id_key값 찾기
        public int FindMaxID(DBConn conn, string strTableName)
        {
            SqlConnection connect = conn.Connect();
            int nCount = 0;
            string strCount = "";

            string SQLMaxID = "select max(ID) from " + strTableName;
            SqlDataReader rd = conn.ExecuteReader(SQLMaxID, connect);
            if (rd.Read())
            {
                if (rd.IsDBNull(0))
                {
                    nCount = 1;
                }
                else
                {
                    strCount = rd[0].ToString();
                    nCount = Convert.ToInt32(strCount);
                    nCount++;
                }
            }
            rd.Close();
            connect.Close();

            return nCount;
        }
    }

    // struct와 같이 null이 허용되지 않는 데이터를 위한 Wrapper 클래스
    public class VariousData<DataType>
    {
        private DataType data;

        public DataType Data
        {
            get { return data; }
            set { data = value; }
        }

        public VariousData()
        {
        }

        public VariousData(DataType data)
        {
            this.data = data;
        }
    }
}
