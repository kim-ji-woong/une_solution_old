using System;
using System.Collections;

namespace AlarmWatcher
{
    public class DBManager
    {
        private string m_strErrorMessage = "";
        private string m_strHost = "", m_strID = "", m_strPW = "", m_strDBName = "";

        public string ErrorMessage
        {
            get { return m_strErrorMessage; }
        }

        public DBManager(string strHost, string strID, string strPW, string strDBName)
        {
            m_strHost = strHost;
            m_strID = strID;
            m_strPW = strPW;
            m_strDBName = strDBName;
        }

        public ArrayList RunQuery(string strSQL)
        {
            m_strErrorMessage = "";
            SqlServerManager mgr = new SqlServerManager(m_strHost, m_strID, m_strPW, m_strDBName);

            if (mgr.Connect())
            {
                ArrayList arrResult = mgr.GetResultData(strSQL);
                mgr.Close();

                if (arrResult == null)
                    m_strErrorMessage = mgr.ErrorMessage;

                return arrResult;
            }
            else
                m_strErrorMessage = mgr.ErrorMessage;

            return null;
        }

        static public VariousData<int> GetIntField(string dataSrc)
        {
            if (dataSrc == null || dataSrc.StartsWith("!") == false)
                return null;

            string strValue = dataSrc.Substring(1);

            if (string.Compare(strValue, "true", true) == 0)
                return new VariousData<int>(1);
            else if (string.Compare(strValue, "false", true) == 0)
                return new VariousData<int>(0);

            int num;

            if (int.TryParse(strValue, out num))
                return new VariousData<int>(num);

            return null;
        }

        static public VariousData<float> GetFloatField(string dataSrc)
        {
            if (dataSrc == null || dataSrc.StartsWith("!") == false)
                return null;

            float num;

            if (float.TryParse(dataSrc.Substring(1), out num))
                return new VariousData<float>(num);

            return null;
        }

        static public VariousData<DateTime> GetDateTimeField(object dataSrc)
        {
            if (dataSrc == null)
                return null;

            string strValue = dataSrc.ToString();

            if (strValue.StartsWith("!") == false)
                return null;

            strValue = strValue.Substring(1);

            try
            {
                DateTime time = Convert.ToDateTime(strValue);
                return new VariousData<DateTime>(time);
            }
            catch (Exception)
            {
            }

            return null;
        }

        // 문자열 앞뒤의 빈문자들을 제거한다.
        static public string GetStringField(object dataSrc)
        {
            if (dataSrc == null)
                return null;

            string strValue = dataSrc.ToString();

            if (strValue.StartsWith("!") == false)
                return null;

            strValue = (string)dataSrc;
            strValue = strValue.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
            strValue = strValue.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

            // (char)5, 6, 7, 8은 DB 입력시 '\t', '\n', '\r', '\''이 임시로 바뀌어 들어간 값이므로, 다시 '\n'으로 되돌려 준다.

            strValue = strValue.Replace((char)6, '\n');
            strValue = strValue.Replace((char)7, '\r');
            strValue = strValue.Replace((char)8, '\'');

            strValue = strValue.Substring(1).Trim();
            return strValue;
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
