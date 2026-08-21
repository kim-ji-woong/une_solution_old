using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace DBToXML.Data
{
    public class Material
    {
        private string m_strID = "";
        private string m_strTypeName = "Type1";
        private string m_strMaterialName = "";

        public const string MaterialIDTag = "component";

        public string ID
        {
            get { return m_strID; }
        }

        public string TypeName
        {
            get { return m_strTypeName; }
            set { m_strTypeName = value; }
        }

        public string MaterialName
        {
            get { return m_strMaterialName; }
            set { m_strMaterialName = value; }
        }

        public static Material ReadMaterial(int nMaterialID, WebDBManager dbMgr)
        {
            string strSQL = "Select TypeName, ComponentName from Component where ID = " + nMaterialID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                string strTypeName = WebDBManager.GetStringField(arrResult[i]);
                string strMaterialName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (strTypeName == null || strMaterialName == null)
                    continue;

                Material material = new Material();

                material.m_strID = MaterialIDTag + nMaterialID.ToString();
                material.m_strTypeName = strTypeName;
                material.m_strMaterialName = strMaterialName;

                return material;
            }

            return null;
        }
    }
}
