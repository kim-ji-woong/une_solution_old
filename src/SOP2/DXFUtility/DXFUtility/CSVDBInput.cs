using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Windows.Forms;
using System.Collections;

namespace DXFUtility
{
    public class CSVDBInput
    {
        private WebDBManager m_dbMgr = null;
        private string m_strCSVFolder = "";
        private string m_strEquipDataFile = "";
        private Dictionary<int, FireEquipmentDBData> m_dicFEDBData = new Dictionary<int, FireEquipmentDBData>();
        private Dictionary<int, FireEquipmentDBData> m_dicFAnHDDBData = new Dictionary<int, FireEquipmentDBData>();

        private Dictionary<int, string> m_dicFETag = new Dictionary<int, string>();
        private Dictionary<int, string> m_dicFAnHDTag = new Dictionary<int, string>();
        private Dictionary<int, string> m_dicFEFilePath = new Dictionary<int, string>();
        private Dictionary<int, string> m_dicFAnHDPath = new Dictionary<int, string>();

        public CSVDBInput(string strFolder, string strEquipDataFile, WebDBManager dbMgr)
        {
            m_strCSVFolder = strFolder;
            m_strEquipDataFile = strEquipDataFile;
            m_dbMgr = dbMgr;
        }

        private void CheckDuplicate()
        {
            string strSQL = "Select RFIDTag, EquipID from FireEquipmentTemp where RFIDTag is not null";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            Dictionary<int, string> dicRFID = new Dictionary<int,string>();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                string strRFID = m_dbMgr.GetStringField(arrResult[i], "");
                int nEquipID = m_dbMgr.GetIntField(arrResult[i + 1].ToString(), -1);

                if (nEquipID < 0)
                    continue;

                dicRFID[nEquipID] = strRFID;
            }

            strSQL = "Select RFIDTag, EquipID from FireEquipment where RFIDTag is not null";

            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount2 = arrResult.Count;
            KeyValuePair<int, string> pair = new KeyValuePair<int,string>();

            for (int i = 0; i < nResultCount2 - 1; i += 2)
            {
                string strRFID = m_dbMgr.GetStringField(arrResult[i], "");
                int nEquipID = m_dbMgr.GetIntField(arrResult[i + 1].ToString(), -1);

                if (nEquipID < 0)
                    continue;

                 bool find = FindRFIDTag(dicRFID, strRFID, ref pair);

                if (find)
                    continue;
            }
        }

        private bool FindRFIDTag(Dictionary<int, string> dicRFID, string strRFID, ref KeyValuePair<int, string> pair)
        {
            foreach (KeyValuePair<int, string> _pair in dicRFID)
            {
                if (_pair.Value == strRFID)
                {
                    pair = _pair;
                    return true;
                }
            }

            return false;
        }

        public bool Run()
        {
            // FireEquipment와 FireEquipmentTemp에 중복된 RFID가 존재하는지 검사
            /*CheckDuplicate();
            return true;*/
            ClearTable();

            if (!ReadEquipDataFile())
                return false;

            if (!ReadCSVFiles())
                return false;

            if (!InsertDB())
                return false;

            return true;
        }

        private bool InsertDB()
        {
            if (!InsertDB(m_dicFEDBData, 0))
                return false;

            if (!InsertDB(m_dicFAnHDDBData, m_dicFEDBData.Count))
                return false;

            // FireEquipment Table에 이미 저장되어 있는 Tag 정보들도 업데이트 시킨다.
            if (!UpdateRegacy())
                return false;

            return true;
        }

        // FireEquipment Table에 이미 저장되어 있는 Tag 정보들도 업데이트 시킨다.
        private bool UpdateRegacy()
        {
            string strSQL = "select RFIDTag, EquipID, EquipType from FireEquipment where RFIDTag is not null";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                string strRFIDTag = m_dbMgr.GetStringField(arrResult[i], "");
                string strEquipID = m_dbMgr.GetStringField(arrResult[i + 1], "");
                int nEquipType = m_dbMgr.GetIntField(arrResult[i + 2].ToString(), -1);

                string strSQL2 = string.Format("select RFIDTag from FireEquipmentTemp where EquipID = '{0}' and EquipType = {1}", strEquipID, nEquipType);
                ArrayList arrResult2 = m_dbMgr.GetResultData(strSQL2, 0);

                if (arrResult2 == null)
                    return false;

                if (arrResult2.Count > 0)
                {
                    string strRFIDTag2 = m_dbMgr.GetStringField(arrResult2[0], "");

                    if (strRFIDTag2 != "" && strRFIDTag2 != "null" && strRFIDTag2 != strRFIDTag)
                        return false;
                }

                strSQL2 = string.Format("Update FireEquipmentTemp set RFIDTag = '{0}' where EquipID = '{1}'",
                    strRFIDTag, strEquipID);

                if (m_dbMgr.GetResultData(strSQL2, 0) == null)
                    return false;
            }

            return true;
        }

        private bool InsertDB(Dictionary<int, FireEquipmentDBData> dicFireEquipment, int nIndex)
        {
            string strFormat = "Insert into FireEquipmentTemp (ID, RFIDTag, EquipID, RFIDTagID, DxfObjID, EquipType, EquipSubType, ZoneID, x, y, z, CreateDate, Duration, Description) ";
            strFormat += "values ({0}, {1}, {2}, {3}, NULL, {4}, {5}, {6}, 0.0, 0.0, 0.0, {7}, {8}, {9})";

            foreach (KeyValuePair<int, FireEquipmentDBData> pair in dicFireEquipment)
            {
                FireEquipmentDBData data = pair.Value;

                string strSQL = string.Format(strFormat, ++nIndex, data.RFIDTag, data.EquipID, data.RFIDTagID, data.EquipType, data.EquipSubType, data.ZoneID, data.CreateDate, data.Duration, data.Description);
                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                {
                    return false;
                }
            }

            return true;
        }

        private bool ReadCSVFiles()
        {
            string[] arrFiles = Directory.GetFiles(m_strCSVFolder);

            foreach (string strFilePath in arrFiles)
            {
                int nDotIndex = strFilePath.LastIndexOf('.');
                if (nDotIndex < 0)
                    continue;

                string strExt = strFilePath.Substring(nDotIndex + 1);
                if (string.Compare(strExt, "csv", true) != 0)
                    continue;

                if (!ReadCSVFile(strFilePath))
                    return false;
            }

            return true;
        }

        private bool ReadCSVFile(string strFilePath)
        {
            StreamReader reader = new StreamReader(strFilePath, Encoding.UTF8);

            // 제목 줄
            reader.ReadLine();

            char[] arrTrims = new char[] { ' ', '\t', '\r', '\n' };
            char delim = ',';

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();
                int nIndex1 = strLine.IndexOf(delim);
                int nIndex2 = strLine.LastIndexOf(delim);

                if (nIndex1 < 0 || nIndex2 < 0)
                    break;

                string strRFIDTag = strLine.Substring(0, nIndex1);
                string strEquipID = strLine.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                string strEquipType = strLine.Substring(nIndex2 + 1);

                strRFIDTag = strRFIDTag.TrimStart(arrTrims);
                strRFIDTag = strRFIDTag.TrimEnd(arrTrims);
                strEquipID = strEquipID.TrimStart(arrTrims);
                strEquipID = strEquipID.TrimEnd(arrTrims);
                strEquipType = strEquipType.TrimStart(arrTrims);
                strEquipType = strEquipType.TrimEnd(arrTrims);

                try
                {
                    int nEquipType;

                    if (!int.TryParse(strEquipType, out nEquipType))
                        nEquipType = 1;

                    int nEquipID = int.Parse(strEquipID);

                    if (nEquipType == 1)
                    {
                        if (!AddCSVData(reader, nEquipID, nEquipType, strRFIDTag, strFilePath, m_dicFETag, m_dicFEDBData, m_dicFEFilePath))
                            return false;
                    }
                    else
                    {
                        if (!AddCSVData(reader, nEquipID, nEquipType, strRFIDTag, strFilePath, m_dicFAnHDTag, m_dicFAnHDDBData, m_dicFAnHDPath))
                            return false;
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }

            reader.Close();
            return true;
        }

        private bool AddCSVData(StreamReader reader, int nEquipID, int nEquipType, string strRFIDTag, string strFilePath, Dictionary<int, string> dicRFIDTag, Dictionary<int, FireEquipmentDBData> dicEquipmentDBData, Dictionary<int, string> dicFilePath)
        {
            if (dicRFIDTag.ContainsKey(nEquipID))
            {
                string strOldFilePath = dicFilePath[nEquipID];
                int nSlashIndex = strOldFilePath.LastIndexOf('\\');
                strOldFilePath = strOldFilePath.Substring(nSlashIndex + 1);
                MessageBox.Show(string.Format("{0}, 설비 번호 {1}, {2}에 이미 동일한 설비 번호가 등록되어 있습니다.", strFilePath, nEquipID, strOldFilePath));
                //reader.Close();
                //return false;
            }

            foreach (KeyValuePair<int, string> pair in dicRFIDTag)
            {
                if (pair.Value == strRFIDTag)
                {
                    string strOldFilePath = dicFilePath[nEquipID];
                    int nSlashIndex = strOldFilePath.LastIndexOf('\\');
                    strOldFilePath = strOldFilePath.Substring(nSlashIndex + 1);
                    MessageBox.Show(string.Format("{0}, 설비 번호 {1}, {2}에 이미 동일한 RFID Tag가 등록되어 있습니다.", strFilePath, nEquipID, strOldFilePath));
                    //reader.Close();
                    //return false;
                }
            }

            dicFilePath[nEquipID] = strFilePath;

            if (!dicEquipmentDBData.ContainsKey(nEquipID))
            {
                MessageBox.Show(string.Format("{0}, 설비 번호 {1}, 목록에 없는 설비번호 입니다.", strFilePath, nEquipID));
                reader.Close();
                return false;
            }

            dicRFIDTag[nEquipID] = strRFIDTag;

            FireEquipmentDBData data = dicEquipmentDBData[nEquipID];
            data.EquipType = nEquipType;
            data.RFIDTag = "'" + strRFIDTag + "'";

            return true;
        }

        private void ClearTable()
        {
            string strSQL = "delete from FireEquipmentTemp";
            m_dbMgr.GetResultData(strSQL, 0);
        }

        private bool ReadEquipDataFile()
        {
            // Excel 프로세스 생성
            Excel.Application app = new Excel.Application();

            // 읽기전용 열기
            Excel.Workbook workBook = app.Workbooks.Open(m_strEquipDataFile, 0, true, 5, Type.Missing, Type.Missing, false, Type.Missing, Type.Missing, true, false, Type.Missing, false, false, false);

            // sheets 생성
            Excel.Sheets sheets = workBook.Sheets;
            
            // 작업할 Sheet 선택
            Excel.Worksheet workSheet = sheets["소화기정보 취합_1~6호기, 사무동, 부속건물"];
            ReadFESheet(workSheet);

            workSheet = sheets["소화전_발신기 정보"];
            ReadFAnHDSheet(workSheet);

            workBook.Close(false);
            // workBook을 null로 초기화하지 않으면 Excel 프로세스가 종료되지 않음
            workBook = null;

            app.Quit();

            return true;
        }

        private void ReadFAnHDSheet(Excel.Worksheet workSheet)
        {
            if (workSheet == null)
                return;

            for (int nRowIndex = 5; ; nRowIndex++)
            {
                string strRange = string.Format("A{0}:M{0}", nRowIndex);
                Excel.Range cellRange = workSheet.Range[strRange];

                string strID = ExcelString(cellRange.Value2[1, 1]);
                string strRFIDTagID = ExcelString(cellRange.Value2[1, 4]);
                string strPosition = ExcelString(cellRange.Value2[1, 11]);

                // '\''은 jsp 쿼리 처리시 문제가 될수 있으므로 (char)8로 바꿔서 DB에 저장한다.
                strRFIDTagID = strRFIDTagID.Replace('\'', (char)8);
                strPosition = strPosition.Replace('\'', (char)8);

                try
                {
                    int nID = int.Parse(strID);
                    AddFAnHDData(nID, strRFIDTagID, strPosition);
                }
                catch (Exception)
                {
                    break;
                }
            }
        }

        private void AddFAnHDData(int nEquipID, string strRFIDTagID, string strPosition)
        {
            FireEquipmentDBData data = new FireEquipmentDBData();
            data.EquipID = "'" + nEquipID.ToString() + "'";
            data.Description = strPosition == "NULL" ? strPosition : "'" + strPosition + "'";
            data.RFIDTagID = strRFIDTagID == "NULL" ? strRFIDTagID : "'" + strRFIDTagID + "'";

            m_dicFAnHDDBData[nEquipID] = data;
        }

        private int GetFEOption(Excel.Range cellRange)
        {
            for (int i = 1; i <= 21; i++)
            {
                if (cellRange.Value2[1, 11 + i] != null)
                    return i;
            }

            return -1;
        }

        private void ReadFESheet(Excel.Worksheet workSheet)
        {
            if (workSheet == null)
                return;

            for (int nRowIndex = 5; ; nRowIndex++)
            {
                string strRange = string.Format("A{0}:AH{0}", nRowIndex);
                Excel.Range cellRange = workSheet.Range[strRange];

                string strID = ExcelString(cellRange.Value2[1, 1]);
                string strRFIDTagID = ExcelString(cellRange.Value2[1, 3]);
                string strPosition = ExcelString(cellRange.Value2[1, 7]);
                int nFEOption = GetFEOption(cellRange);
                string strDate = ExcelString(cellRange.Value2[1, 33]);

                // '\''은 jsp 쿼리 처리시 문제가 될수 있으므로 (char)8로 바꿔서 DB에 저장한다.
                strRFIDTagID = strRFIDTagID.Replace('\'', (char)8);
                strPosition = strPosition.Replace('\'', (char)8);

                try
                {
                    int nID = int.Parse(strID);
                    AddFEData(nID, strRFIDTagID, strPosition, nFEOption, strDate);
                }
                catch (Exception)
                {
                    break;
                }
            }
        }

        private string GetDateTimeString(string strDate)
        {
            int nLen = strDate.Length;
            if (nLen == 0)
                return "";

            int num = 0;
            int nYear = -1, nMonth = -1, nDay = -1;

            for (int i = 0; i < nLen; i++)
            {
                char ch = strDate[i];

                if (ch >= '0' && ch <= '9')
                {
                    num = num * 10 + ch - '0';
                }
                else
                {
                    if (num > 0)
                    {
                        if (nYear < 0)
                            nYear = num;
                        else if (nMonth < 0)
                            nMonth = num;
                        else if (nDay < 0)
                            nDay = num;

                        num = 0;
                    }
                }
            }

            if (num > 0)
            {
                if (nYear < 0)
                    nYear = num;
                else if (nMonth < 0)
                    nMonth = num;
                else if (nDay < 0)
                    nDay = num;

                num = 0;
            }

            if (nYear > 0 && nYear < 100)
                nYear += 2000;

            if (nYear < 0)
                return "NULL";
            else if (nMonth < 0)
                return string.Format("{0}-01-01 00:00:00", nYear);
            else if (nDay < 0)
                return string.Format("{0}-{1}-01 00:00:00", nYear, nMonth);

            return string.Format("{0}-{1}-{2} 00:00:00", nYear, nMonth, nDay);
        }

        private void AddFEData(int nEquipID, string strRFIDTagID, string strPosition, int nFEOption, string strDate)
        {
            string strFEOption = nFEOption < 0 ? "NULL" : nFEOption.ToString();
            string strCreateTime = GetDateTimeString(strDate);

            FireEquipmentDBData data = new FireEquipmentDBData();
            data.EquipID = "'" + nEquipID.ToString() + "'";
            data.CreateDate = strCreateTime == "NULL" ? strCreateTime : "'" + strCreateTime + "'";
            data.Description = strPosition == "NULL" ? strPosition : "'" + strPosition + "'";
            data.EquipSubType = strFEOption;
            data.EquipType = 1;
            data.RFIDTagID = strRFIDTagID == "NULL" ? strRFIDTagID : "'" + strRFIDTagID + "'";

            m_dicFEDBData[nEquipID] = data;
        }

        private string ExcelString(object obj)
        {
            if (obj == null)
                return "";

            return obj.ToString();
        }
    }
}
