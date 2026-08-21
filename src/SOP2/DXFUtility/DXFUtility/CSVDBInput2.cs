using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace DXFUtility
{
    public class CSVDBInput2
    {
        private WebDBManager m_dbMgr = null;
        //private string m_strFilePath = "";
        private string m_strFolderPath = "";

        public CSVDBInput2(string strFolderPath, WebDBManager dbMgr)
        {
            //m_strFilePath = strFilePath;
            m_strFolderPath = strFolderPath;
            m_dbMgr = dbMgr;
        }

        public bool Run()
        {
            string[] arrFiles = System.IO.Directory.GetFiles(m_strFolderPath);

            foreach (string strFilePath in arrFiles)
            {
                int nDotIndex = strFilePath.LastIndexOf('.');
                string strExt = strFilePath.Substring(nDotIndex + 1);

                if (string.Compare(strExt, "txt", true) != 0)
                    continue;

                // Key : "EquipID-EquipType"
                // Value : FireEquipment DB ID
                Dictionary<string, int> dicEquipmentID = new Dictionary<string, int>();

                if (!ReadEquipmentID(dicEquipmentID, strFilePath))
                    return false;

                if (!UpdateEquipmentData(dicEquipmentID))
                {
                    MessageBox.Show("실패하였습니다.");
                    return false;
                }
            }

            MessageBox.Show("성공하였습니다.");
            return true;
        }

        private string GetDateTimeDBString(string strDateTime)
        {

            try
            {
                DateTime dt = Convert.ToDateTime(strDateTime);
            }
            catch (Exception)
            {
                return "NULL";
            }

            return "'" + strDateTime + "'";
        }

        private bool UpdateEquipmentData(Dictionary<string, int> dicEquipmentID)
        {
            string strSQL = "select RFIDTag, EquipID, RFIDTagID, EquipType, EquipSubType, CreateDate, Duration, Description from FireEquipmentTemp where RFIDTag is not null";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                string strRFIDTag = m_dbMgr.GetStringField(arrResult[i], "");
                string strEquipID = m_dbMgr.GetStringField(arrResult[i + 1], "");
                string strRFIDTagID = m_dbMgr.GetStringField(arrResult[i + 2], "");
                int nEquipType = m_dbMgr.GetIntField(arrResult[i + 3].ToString(), -1);
                int nEquipSubType = m_dbMgr.GetIntField(arrResult[i + 4].ToString(), -1);
                string strCreateTime = m_dbMgr.GetStringField(arrResult[i + 5], "");
                int nDuration = m_dbMgr.GetIntField(arrResult[i + 6].ToString(), -1);
                string strDescription = m_dbMgr.GetStringField(arrResult[i + 7], "");

                if (strRFIDTag.Length == 0 || strRFIDTag == "null")
                    continue;

                if (strEquipID.Length == 0 || strEquipID == "null")
                    continue;

                if (nEquipType < 0)
                    continue;

                string strKey = strEquipID + "-" + nEquipType.ToString();

                if (dicEquipmentID.ContainsKey(strKey))
                {
                    int nID = dicEquipmentID[strKey];

                    string strSQL2 = "select id from FireEquipment where id = " + nID.ToString();
                    ArrayList arrResult2 = m_dbMgr.GetResultData(strSQL2, 0);

                    if (arrResult2 == null || arrResult2.Count == 0)
                        continue;

                    CheckQuotation(ref strDescription);
                    CheckQuotation(ref strRFIDTagID);

                    string strDBCreate = GetDateTimeDBString(strCreateTime);
                    string strDBDesc = strDescription.Length == 0 || strDescription == "null" ? "NULL" : "'" + strDescription + "'";
                    string strDBRFIDTagID = strRFIDTagID.Length == 0 || strRFIDTagID == "null" ? "NULL" : "'" + strRFIDTagID + "'";

                    strSQL2 = string.Format("Update FireEquipment set RFIDTag = '{0}', EquipID = '{1}', RFIDTagID = {2}, EquipType = {3}, EquipSubType = {4}, CreateDate = {5}, Duration = {6}, Description = {7} where id = {8}",
                        strRFIDTag, strEquipID, strDBRFIDTagID, nEquipType, nEquipSubType, strDBCreate, nDuration < 0 ? "NULL" : nDuration.ToString(), strDBDesc, nID);

                    if (m_dbMgr.GetResultData(strSQL2, 0) == null)
                        return false;
                }
                else
                    continue;
            }

            return true;
        }

        // 문자열의 가운데에 ['] 가 있으면 ['']으로 바꿔준다.
        private void CheckQuotation(ref string strField)
        {
            int nBeginIndex = 0;
            int nIndex = strField.IndexOf('\'', nBeginIndex);

            ArrayList arrQuotation = new ArrayList();

            while (nIndex >= 0)
            {
                arrQuotation.Add(nIndex);
                nBeginIndex = nIndex + 1;
                nIndex = strField.IndexOf('\'', nBeginIndex);
            }

            int nArrSize = arrQuotation.Count;

            for (int i = nArrSize - 1; i >= 0; i--)
            {
                int nQuotationIndex = (int)arrQuotation[i];
                strField = strField.Insert(nQuotationIndex, "'");
            }
        }

        private bool ReadEquipmentID(Dictionary<string, int> dicEquipmentID, string strFilePath)
        {
            StreamReader reader = new StreamReader(strFilePath, Encoding.UTF8);

            char[] arrTrims = new char[] { ' ', '\t', '\r', '\n' };
            char delim = ',';

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();

                int nIndex1 = strLine.IndexOf(delim);
                int nIndex2 = strLine.LastIndexOf(delim);

                if (nIndex1 < 0 || nIndex2 < 0)
                    break;

                string strID = strLine.Substring(0, nIndex1);
                string strEquipID = strLine.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                string strEquipType = strLine.Substring(nIndex2 + 1);

                strID = strID.TrimStart(arrTrims);
                strID = strID.TrimEnd(arrTrims);
                strEquipID = strEquipID.TrimStart(arrTrims);
                strEquipID = strEquipID.TrimEnd(arrTrims);
                strEquipType = strEquipType.TrimStart(arrTrims);
                strEquipType = strEquipType.TrimEnd(arrTrims);

                try
                {
                    int nID = int.Parse(strID);
                    int nEquipType = int.Parse(strEquipType);
                    int nEquipID = int.Parse(strEquipID);

                    if (nEquipID < 0)
                        continue;

                    string strKey = strEquipID + "-" + strEquipType;
                    dicEquipmentID[strKey] = nID;
                }
                catch (Exception)
                {
                    continue;
                }
            }

            reader.Close();
            return true;
        }
    }
}
