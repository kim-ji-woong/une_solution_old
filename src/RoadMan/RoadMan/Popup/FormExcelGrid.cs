using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace RoadMan
{
    public partial class FormExcelGrid : Form
    {
        private string m_strExcelFile = "";
        private Excel.Application m_app = null;
        private Excel.Workbook m_workBook = null;
        private IExcelGridManager m_mgr = null;

        public FormExcelGrid(string strPath, IExcelGridManager mgr)
        {
            m_strExcelFile = strPath;
            m_mgr = mgr;

            InitializeComponent();
        }

        private void FormExcelGrid_Load(object sender, EventArgs e)
        {
            Cursor oldCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
            List<string> sheetNames = GetSheetNames();
            this.Cursor = oldCursor;

            if (sheetNames != null)
            {
                string strSheetName = "";

                if (sheetNames.Count == 1)
                    strSheetName = sheetNames[0];
                else
                {
                    FormSheetNames frm = new FormSheetNames(sheetNames);
					DialogFormFrame frameSheet = new DialogFormFrame(frm);

                    if (frameSheet.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        strSheetName = frm.TargetSheetName;
                    }
                    else
                    {
                        CloseExcel();
                        this.Visible = false;

                        // 안전한 종료처리를 위하여 1초후 종료시킨다.
                        timer1.Start();
                        return;
                    }
                }

                this.Cursor = Cursors.WaitCursor;
                LoadExcel(strSheetName);
                this.Cursor = oldCursor;
            }
            else
            {
                CloseExcel();

                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Close();
            }
        }

        private List<string> GetSheetNames()
        {
            try
            {
                // Excel 프로세스 생성
                m_app = new Excel.Application();

                // 읽기전용 열기
                m_workBook = m_app.Workbooks.Open(m_strExcelFile, 0, true, 5, Type.Missing, Type.Missing, false, Type.Missing, Type.Missing, true, false, Type.Missing, false, false, false);

                // sheets 생성
                Excel.Sheets sheets = m_workBook.Sheets;
                List<string> sheetNames = new List<string>();

                foreach (Excel.Worksheet sheet in sheets)
                {
                    sheetNames.Add(sheet.Name);
                }

                return sheetNames;
            }
            catch (Exception/* e*/)
            {
                UnE.Utility.UMessageBox.Show(this, "Excel Sheet 열기 오류가 발생하였습니다.", "열기 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //MessageBox.Show(e.Message);
                CloseExcel();
            }

            return null;
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
                    m_app.Application.Quit();
                    m_app.Quit();
                    Marshal.ReleaseComObject(m_app);

                    m_app = null;

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }

        private bool ReadWorkSheet(Excel.Worksheet workSheet)
        {
            if (workSheet == null)
                return false;

            int nColumnCount = workSheet.UsedRange.Columns.Count;
            int nRowCount = workSheet.UsedRange.Rows.Count;

            if (nColumnCount == 0 || nRowCount == 0)
                return false;

            for (int i=0;i<nColumnCount;i++)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Width = 100;
                dataGridView1.Columns.Add(column);
            }

            for (int i=0;i<nRowCount;i++)
            {
                DataGridViewRow row = new DataGridViewRow();

                for (int j = 0; j < nColumnCount; j++)
                {
                    object obj = workSheet.UsedRange.Cells[i + 1, j + 1].Value2;
                    string strValue = ExcelString(obj);
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    cell.Value = strValue;
                    row.Cells.Add(cell);
                }

                dataGridView1.Rows.Add(row);
            }

            return true;
        }

        private string ExcelString(object obj)
        {
            if (obj == null)
                return "";

            return obj.ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (m_mgr == null || m_mgr.ExcelGridLinker == null)
                return;

            DataGridViewSelectedCellCollection cellsTarget = m_mgr.ExcelGridLinker.GetPastePositionCells();
            //DataGridViewSelectedCellCollection cellsTarget = m_frm.GetSelectedCells();

            if (cellsTarget.Count == 0)
            {
				UnE.Utility.UMessageBox.Show(this, "붙여넣기할 대상이 정해지지 않았습니다.\r\n붙여넣을 곳의 셀을 선택해주세요.", "붙여넣기", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //MessageBox.Show("붙여넣기할 대상이 정해지지 않았습니다.\r\n붙여넣을 곳의 셀을 선택해주세요.");
                return;
            }

            DataGridViewSelectedCellCollection cellsSource = dataGridView1.SelectedCells;

            if (cellsSource.Count == 0)
            {
				string szMsg = "복사할 셀이 선택되지 않았습니다.\r\n복사할 셀을 선택해주세요.";
                UnE.Utility.UMessageBox.Show(this, szMsg, "Cell 복사 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //MessageBox.Show("복사할 셀이 선택되지 않았습니다.\r\n복사할 셀을 선택해주세요.");
                return;
            }

            m_mgr.ExcelGridLinker.PasteCells(cellsSource);
            //m_frm.PasteCells(cellsSource);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
