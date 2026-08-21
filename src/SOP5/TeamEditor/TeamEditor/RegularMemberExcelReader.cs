using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace TeamEditor
{
    public class RegularMemberExcelReader : RegularMemberReader
    {
        private Excel.Application m_app = null;
        private Excel.Workbook m_workBook = null;

        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);

        public bool OpenExcelFile(string strPath)
        {
            List<string> sheetNames = GetSheetNames(strPath);

            if (sheetNames == null || sheetNames.Count == 0)
            {
                CloseExcel();
                return false;
            }

            string strSheetName = "";

            if (sheetNames.Count == 1)
                strSheetName = sheetNames[0];
            else
            {
                Popup.FormExcelSheet frm = new Popup.FormExcelSheet(sheetNames);

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    strSheetName = frm.SheetName;
                }
                else
                {
                    CloseExcel();
                    return false;
                }
            }

            Cursor oldCursor = FormMain.Instance.Cursor;
            FormMain.Instance.Enabled = false;

            FormMain.Instance.Cursor = Cursors.WaitCursor;
            LoadExcel(strSheetName);

            FormMain.Instance.Enabled = true;
            FormMain.Instance.Cursor = oldCursor;

            return true;
        }

        private bool LoadExcel(string strSheetName)
        {
            // sheets 생성
            Excel.Sheets sheets = m_workBook.Sheets;

            // 작업할 Sheet 선택
            Excel.Worksheet workSheet = sheets[strSheetName];
            bool result = ReadWorkSheet(workSheet);

            Marshal.ReleaseComObject(workSheet);
            CloseExcel();
            return result;
        }

        private bool ReadWorkSheet(Excel.Worksheet workSheet)
        {
            Popup.FormProcessingExcel frm = new Popup.FormProcessingExcel(0);
            frm.Show(FormMain.Instance);

            List<string[]> tokenList = WorkSheetToList(frm, workSheet);

            if (tokenList == null)
            {
                frm.Close();
                return false;
            }

            int nRowCount = tokenList.Count;
            frm.RowCount = nRowCount;

            // Key : Column Index
            // Value : 해당 Column에 대한 실제 File 내 Column Index
            Dictionary<COLUMN_HEADER, int> dicIndices = null;
            Dictionary<JOB_POSITION, int> dicJobPositionID = ReadJobPositions();
            // 사번 중복체크용
            Dictionary<string, string> dicMemberID = new Dictionary<string, string>();
            // 휴대전화번호 중복체크용
            Dictionary<string, string> dicPhoneNumber = new Dictionary<string, string>();

            int nIndexCount = Enum.GetValues(typeof(COLUMN_HEADER)).Length;

            for (int i = 0; i < nRowCount; i++)
            {
                string[] tokens = tokenList[i];
                
                if (dicIndices == null)
                    dicIndices = FindColumnHeader(tokens, nIndexCount);
                else
                    ReadRegularMember(tokens, dicIndices, nIndexCount, dicJobPositionID, dicMemberID, dicPhoneNumber);

                frm.SetRowCount(i);
            }

            frm.SetRowCount(nRowCount);
            frm.Close();

            return true;

            /*if (workSheet == null)
                return false;

            int nColumnCount = workSheet.UsedRange.Columns.Count;
            int nRowCount = workSheet.UsedRange.Rows.Count;

            if (nColumnCount == 0 || nRowCount == 0)
                return false;

            Popup.FormProcessingExcel frm = new Popup.FormProcessingExcel(nRowCount);
            frm.Show(FormMain.Instance);

            // Key : Column Index
            // Value : 해당 Column에 대한 실제 File 내 Column Index
            Dictionary<COLUMN_HEADER, int> dicIndices = null;
            Dictionary<JOB_POSITION, int> dicJobPositionID = ReadJobPositions();

            int nIndexCount = Enum.GetValues(typeof(COLUMN_HEADER)).Length;

            for (int i = 0; i < nRowCount; i++)
            {
                string[] tokens = new string[nColumnCount];
                bool isEmpty = true;

                for (int j = 0; j < nColumnCount; j++)
                {
                    object obj = workSheet.UsedRange.Cells[i + 1, j + 1].Value2;
                    string strValue = ExcelString(obj).Trim();
                    tokens[j] = strValue;

                    if (strValue.Length > 0)
                        isEmpty = false;
                }

                if (isEmpty)
                    continue;

                if (dicIndices == null)
                    dicIndices = FindColumnHeader(tokens, nIndexCount);
                else
                    ReadRegularMember(tokens, dicIndices, nIndexCount, dicJobPositionID);

                System.Diagnostics.Trace.WriteLine(string.Format("Call SetRowCount : ({0}, {1})", i, nRowCount));
                frm.SetRowCount(i);
            }

            System.Diagnostics.Trace.WriteLine("Call SetRowCount : Last");
            frm.SetRowCount(nRowCount);
            frm.Close();

            Marshal.ReleaseComObject(workSheet);
            return true;*/
        }

        private List<string[]> WorkSheetToList(Popup.FormProcessingExcel frm, Excel.Worksheet workSheet)
        {
            if (workSheet == null)
                return null;

            int nColumnCount = workSheet.UsedRange.Columns.Count;
            int nRowCount = workSheet.UsedRange.Rows.Count;

            if (nColumnCount == 0 || nRowCount == 0)
                return null;

            List<string[]> tokenList = new List<string[]>();

            int nIndexCount = Enum.GetValues(typeof(COLUMN_HEADER)).Length;

            for (int i = 0; i < nRowCount; i++)
            {
                frm.WaitUntilBeginning();

                string[] tokens = new string[nColumnCount];
                bool isEmpty = true;

                for (int j = 0; j < nColumnCount; j++)
                {
                    object obj = workSheet.UsedRange.Cells[i + 1, j + 1].Value2;
                    string strValue = ExcelString(obj).Trim();
                    tokens[j] = strValue;

                    if (strValue.Length > 0)
                        isEmpty = false;
                }

                if (isEmpty)
                    continue;

                tokenList.Add(tokens);
            }

            Marshal.ReleaseComObject(workSheet);
            return tokenList;
        }

        private string ExcelString(object obj)
        {
            if (obj == null)
                return "";

            return obj.ToString();
        }

        private List<string> GetSheetNames(string strPath)
        {
            try
            {
                // Excel 프로세스 생성
                m_app = new Excel.Application();

                // 읽기전용 열기
                m_workBook = m_app.Workbooks.Open(strPath, 0, true, 5, Type.Missing, Type.Missing, false, Type.Missing, Type.Missing, true, false, Type.Missing, false, false, false);

                // sheets 생성
                Excel.Sheets sheets = m_workBook.Sheets;
                List<string> sheetNames = new List<string>();

                foreach (Excel.Worksheet sheet in sheets)
                {
                    sheetNames.Add(sheet.Name);
                }

                return sheetNames;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                CloseExcel();
            }

            return null;
        }

        private void CloseExcel()
        {
            if (m_workBook != null)
            {
                Marshal.ReleaseComObject(m_workBook.Sheets);

                m_workBook.Close(false);
                Marshal.ReleaseComObject(m_workBook);
                m_workBook = null;

                if (m_app != null)
                {
                    Marshal.ReleaseComObject(m_app.Workbooks);
                    //m_app.Application.Quit();
                    m_app.Quit();
                    Marshal.ReleaseComObject(m_app);

                    /*System.Diagnostics.Process process = GetExcelProcess();

                    if (process != null)
                        process.Kill();*/

                    m_app = null;

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }

        private System.Diagnostics.Process GetExcelProcess()
        {
            int id;
            GetWindowThreadProcessId(m_app.Hwnd, out id);
            return System.Diagnostics.Process.GetProcessById(id);
        }
    }
}
