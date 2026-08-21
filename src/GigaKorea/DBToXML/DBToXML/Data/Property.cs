using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using DBUtility2;

namespace DBToXML.Data
{
    public class Property
    {
        private string m_strGroupName = "";
        private string m_strName = "";
        private string m_strValue = "";
        private string m_strDescription = null;

        public string GroupName
        {
            get { return m_strGroupName; }
            set { m_strGroupName = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public static Dictionary<int, List<Property>> ReadDB(WebDBManager dbMgr, string strGroupName, string strTableName, string strIDField, string strCondition = null)
        {
            string strSQL = string.Format("Select {0}, PropertyName, PropertyValue, Description from {1}", strIDField, strTableName);

            if (strCondition != null)
            {
                strSQL += " where " + strCondition;
            }

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            Dictionary<int, List<Property>> dicProperties = new Dictionary<int, List<Property>>();

            if (arrResult == null)
                return dicProperties;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strValue = WebDBManager.GetStringField(arrResult[i + 2]);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 3]);

                if (id == null || strName == null || strValue == null)
                    continue;

                Property property = new Property();

                property.m_strGroupName = strGroupName;
                property.m_strName = strName;
                property.m_strValue = strValue;
                property.m_strDescription = strDescription;

                List<Property> properties = null;

                if (dicProperties.TryGetValue(id.Data, out properties) == false)
                {
                    properties = new List<Property>();
                    dicProperties[id.Data] = properties;
                }

                properties.Add(property);
            }

            return dicProperties;
        }

        public static List<Property> ReadDB(WebDBManager dbMgr, string strGroupName, string strTableName, string strIDField, int nID)
        {
            string strSQL = string.Format("Select PropertyName, PropertyValue, Description from {0} where {1} = {2}", strTableName, strIDField, nID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            List<Property> properties = new List<Property>();

            if (arrResult == null)
                return properties;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                string strName = WebDBManager.GetStringField(arrResult[i]);
                string strValue = WebDBManager.GetStringField(arrResult[i + 1]);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 2]);

                if (strName == null || strValue == null)
                    continue;

                Property property = new Property();

                property.m_strGroupName = strGroupName;
                property.m_strName = strName;
                property.m_strValue = strValue;
                property.m_strDescription = strDescription;

                properties.Add(property);
            }

            return properties;
        }
    }
}
