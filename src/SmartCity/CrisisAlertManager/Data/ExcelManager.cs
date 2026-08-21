using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

using Excel = Microsoft.Office.Interop.Excel;

namespace CrisisAlertManager.Data
{
    class ExcelManager
    {
        Application m_ExcelApp = null;
        Workbook m_workbook = null;
        Worksheet m_worksheet = null;

        public ExcelManager()
        {
            
        }

        public string SaveDataReportExcel(Dictionary<int, DataReport> dicDataReport)
        {
            string currentPath = System.IO.Directory.GetCurrentDirectory();
            string strPath = currentPath + @"\" + CommonString.Report_Data_Kor + @".xlsx";

            m_ExcelApp = new Application();
            m_workbook = m_ExcelApp.Workbooks.Add();
            m_worksheet = m_workbook.Worksheets.Item[1];
            m_worksheet.Name = CommonString.Report_Data_Kor;

            // 목록 셀 설정
            SetHeadCell(m_worksheet);

            int nRow = 1;

            foreach (KeyValuePair<int, DataReport> item in dicDataReport)
            {
                nRow++;
                DataReport data = item.Value;
                string strType = TransFacilityType(data.FacilityType);

                m_worksheet.Cells[nRow, 1] = nRow - 1;
                m_worksheet.Cells[nRow, 2] = strType;
                m_worksheet.Cells[nRow, 3] = data.OccurTime.ToString("yyyy년 MM월 dd일 hh시 mm분 ss초");
                m_worksheet.Cells[nRow, 4] = data.DataName;
                m_worksheet.Cells[nRow, 5] = data.OriginData;
                m_worksheet.Cells[nRow, 6] = data.NewData;
            }

            // 데이터 셀 설정
            SetDataCell(m_worksheet, nRow);

            int nCount = 1;

            strPath = CheckFilePath(strPath);

            m_workbook.SaveAs(strPath);
            m_workbook.Close();

            return strPath;
        }

        public string SaveAlertReportExcel(Dictionary<int, AlertReport> dicAlertReport)
        {
            string currentPath = System.IO.Directory.GetCurrentDirectory();
            string strPath = currentPath + @"\" + CommonString.Report_Alert_Kor + @".xlsx";

            m_ExcelApp = new Application();
            m_workbook = m_ExcelApp.Workbooks.Add();
            m_worksheet = m_workbook.Worksheets.Item[1];
            m_worksheet.Name = CommonString.Report_Alert_Kor;

            // 목록 셀 설정
            SetHeadCell(m_worksheet);

            int nRow = 1;

            foreach (KeyValuePair<int, AlertReport> item in dicAlertReport)
            {
                nRow++;
                AlertReport data = item.Value;
                string strType = TransFacilityType(data.FacilityType);

                m_worksheet.Cells[nRow, 1] = nRow - 1;
                m_worksheet.Cells[nRow, 2] = strType;
                m_worksheet.Cells[nRow, 3] = data.OccurTime.ToString("yyyy년 MM월 dd일 hh시 mm분 ss초");
                m_worksheet.Cells[nRow, 4] = data.DataName;
                m_worksheet.Cells[nRow, 5] = data.OriginData;
                m_worksheet.Cells[nRow, 6] = data.NewData;
            }

            // 데이터 셀 설정
            SetDataCell(m_worksheet, nRow);

            int nCount = 1;

            strPath = CheckFilePath(strPath);

            m_workbook.SaveAs(strPath);
            m_workbook.Close();

            return strPath;
        }

        private string TransFacilityType(FacilityType facilityType)
        {
            string strFacilityType = "";

            if (facilityType == FacilityType.FIRE_SENSOR)
                strFacilityType = CommonString.FacilityType_Fire_Kor;
            else if (facilityType == FacilityType.FLOOD_SENSOR)
                strFacilityType = CommonString.FacilityType_Flood_Kor;
            else if (facilityType == FacilityType.HEAT_SENSOR)
                strFacilityType = CommonString.FacilityType_Heat_Kor;
            else if (facilityType == FacilityType.COLLAPSE_SENSOR)
                strFacilityType = CommonString.FacilityType_Collapse_Kor;

            return strFacilityType;
        }

        private string CheckFilePath(string strPath)
        {
            int nChkPath = strPath.LastIndexOf(@"\") + 1;
            int nExtension = strPath.LastIndexOf(".");
            int nTitle = nExtension - nChkPath;

            string strExtension = strPath.Substring(nExtension);
            string strTitle = strPath.Substring(nChkPath, nTitle);
            string strFilePath = strPath.Substring(0, nChkPath);

            string strNewPath = strFilePath + strTitle + strExtension;

            int nCount = 1;

            while (File.Exists(strNewPath))
            {
                strNewPath = strFilePath + strTitle + "(" + nCount.ToString() + ")" + strExtension;
                nCount++;
            }

            return strNewPath;
        }

        private void SetHeadCell(Worksheet worksheet)
        {
            // 행 목록
            worksheet.Cells[1, 1] = CommonString.Report_No;
            worksheet.Cells[1, 2] = CommonString.Report_Type;
            worksheet.Cells[1, 3] = CommonString.Report_Time;
            worksheet.Cells[1, 4] = CommonString.Report_DataName;
            worksheet.Cells[1, 5] = CommonString.Report_OriginData;
            worksheet.Cells[1, 6] = CommonString.Report_NewData;

            // 컬럼 속성 설정
            Excel.Range range = (Excel.Range)worksheet.get_Range("A1", "F1");
            range.Font.Bold = true;         // 볼드 설정
            range.RowHeight = 20;           // 셀 높이 설정
            range.Borders.LineStyle = 1;    // 테두리 설정
            range.HorizontalAlignment = 3;  // 정렬 설정

            // 셀 가로 사이즈 설정
            range = (Excel.Range)worksheet.get_Range("A1");
            range.ColumnWidth = 6;
            range = (Excel.Range)worksheet.get_Range("B1");
            range.ColumnWidth = 7;
            range = (Excel.Range)worksheet.get_Range("C1");
            range.ColumnWidth = 34;
            range = (Excel.Range)worksheet.get_Range("D1", "F1");
            range.ColumnWidth = 24;
        }

        

        private void SetDataCell(Worksheet worksheet, int nRow)
        {
            if (nRow == 1)
                return;

            // 데이터 컬럼 속성 설정
            Excel.Range range = (Excel.Range)m_worksheet.get_Range("A2", "F" + nRow);
            range.Borders.LineStyle = 1;
            range.HorizontalAlignment = 3;
        }

        

        public string SaveSMSReportExcel(Dictionary<int, SMSReport> dicSMSReport)
        {
            string currentPath = System.IO.Directory.GetCurrentDirectory();
            string strPath = currentPath + @"\" + CommonString.Report_SMS_Kor + @".xlsx";

            m_ExcelApp = new Application();
            m_workbook = m_ExcelApp.Workbooks.Add();
            m_worksheet = m_workbook.Worksheets.Item[1];
            m_worksheet.Name = CommonString.Report_SMS_Kor;

            // 목록 셀 설정
            SetSMSHeadCell(m_worksheet);

            int nRow = 1;

            foreach (KeyValuePair<int, SMSReport> item in dicSMSReport)
            {
                nRow++;
                SMSReport data = item.Value;
                string strType = TransFacilityType(data.FacilityType);

                m_worksheet.Cells[nRow, 1] = nRow - 1;
                m_worksheet.Cells[nRow, 2] = strType;
                m_worksheet.Cells[nRow, 3] = data.OccurTime.ToString("yyyy년 MM월 dd일 hh시 mm분 ss초");
                m_worksheet.Cells[nRow, 4] = data.Message;
                m_worksheet.Cells[nRow, 5] = data.Managers;
            }

            // 데이터 셀 설정
            SetSMSDataCell(m_worksheet, nRow);

            int nCount = 1;

            strPath = CheckFilePath(strPath);

            m_workbook.SaveAs(strPath);
            m_workbook.Close();

            return strPath;
        }

        private void SetSMSHeadCell(Worksheet worksheet)
        {
            // 행 목록
            worksheet.Cells[1, 1] = CommonString.Report_No;
            worksheet.Cells[1, 2] = CommonString.Report_Type;
            worksheet.Cells[1, 3] = CommonString.Report_Time;
            worksheet.Cells[1, 4] = CommonString.Report_Message;
            worksheet.Cells[1, 5] = CommonString.Report_Manager;


            // 컬럼 속성 설정
            Excel.Range range = (Excel.Range)worksheet.get_Range("A1", "E1");
            range.Font.Bold = true;         // 볼드 설정
            range.RowHeight = 20;           // 셀 높이 설정
            range.Borders.LineStyle = 1;    // 테두리 설정
            range.HorizontalAlignment = 3;  // 정렬 설정

            // 셀 가로 사이즈 설정
            range = (Excel.Range)worksheet.get_Range("A1");
            range.ColumnWidth = 6;
            range = (Excel.Range)worksheet.get_Range("B1");
            range.ColumnWidth = 7;
            range = (Excel.Range)worksheet.get_Range("C1");
            range.ColumnWidth = 34;
            range = (Excel.Range)worksheet.get_Range("D1", "E1");
            range.ColumnWidth = 34;
        }

        private void SetSMSDataCell(Worksheet worksheet, int nRow)
        {
            if (nRow == 1)
                return;

            // 데이터 컬럼 속성 설정
            Excel.Range range = (Excel.Range)m_worksheet.get_Range("A2", "E" + nRow);
            range.Borders.LineStyle = 1;
            range.HorizontalAlignment = 3;
        }
    }
}
