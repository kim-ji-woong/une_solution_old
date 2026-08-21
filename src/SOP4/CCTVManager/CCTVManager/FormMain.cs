using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using System.Collections;

namespace CCTVManager
{
    public partial class FormMain : Form
    {
        private WebDBManager m_dbMgr = new WebDBManager();

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            InitColumns();
            LoadCCTVs();
        }

        private void InitColumns()
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void LoadCCTVs()
        {
            dataGridView1.Rows.Clear();

            string strSQL = "select ID, CameraName, IPAddr, Port, PositionName, Type, UserID, Password from CCTV";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
            {
                MessageBox.Show("Server에 연결할 수 없습니다.");
                return;
            }

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-7;i+=8)
            {
                VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strCameraName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strIP = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<int> nPort = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strPositionName = WebDBManager.GetStringField(arrResult[i + 4]);
                string strType = WebDBManager.GetStringField(arrResult[i + 5]);
                string strUserID = WebDBManager.GetStringField(arrResult[i + 6]);
                string strPW = WebDBManager.GetStringField(arrResult[i + 7]);

                if (nID == null || nPort == null || strCameraName == null || strIP == null ||
                    strPositionName == null || strType == null || strUserID == null || strPW == null)
                    continue;

                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = nID.Data.ToString();
                row.Cells.Add(cell);
                cell.ReadOnly = true;
                cell.Style.BackColor = Color.LightGray;

                cell = new DataGridViewTextBoxCell();
                cell.Value = strCameraName;
                row.Cells.Add(cell);
                cell.ReadOnly = true;
                cell.Style.BackColor = Color.LightGray;

                cell = new DataGridViewTextBoxCell();
                cell.Value = strIP;
                row.Cells.Add(cell);
                cell.ReadOnly = true;
                cell.Style.BackColor = Color.LightGray;

                cell = new DataGridViewTextBoxCell();
                cell.Value = nPort.Data.ToString();
                row.Cells.Add(cell);
                cell.ReadOnly = true;
                cell.Style.BackColor = Color.LightGray;

                cell = new DataGridViewTextBoxCell();
                cell.Value = strPositionName;
                row.Cells.Add(cell);
                cell.ReadOnly = true;
                cell.Style.BackColor = Color.LightGray;

                cell = new DataGridViewTextBoxCell();
                cell.Value = strType;
                row.Cells.Add(cell);
                cell.ReadOnly = true;
                cell.Style.BackColor = Color.LightGray;

                cell = new DataGridViewTextBoxCell();
                cell.Value = strUserID;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = strPW;
                row.Cells.Add(cell);

                dataGridView1.Rows.Add(row);
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 7)
            {
                if (e.Value != null)
                {
                    e.Value = new string('*', e.Value.ToString().Length);
                }

            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            bool passEmpty = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string strID = row.Cells[0].Value.ToString();
                string strCameraName = row.Cells[1].Value.ToString();
                string strIP = row.Cells[2].Value.ToString();
                string strPort = row.Cells[3].Value.ToString();
                string strUserID = row.Cells[6].Value == null ? "" : row.Cells[6].Value.ToString();
                string strPW = row.Cells[7].Value == null ? "" : row.Cells[7].Value.ToString();

                if (passEmpty == false && (strUserID.Length == 0 || strPW.Length == 0))
                {
                    dataGridView1.ClearSelection();

                    if (strUserID.Length == 0)
                    {
                        dataGridView1.CurrentCell = row.Cells[6];
                        row.Cells[6].Selected = true;
                    }
                    else
                    {
                        dataGridView1.CurrentCell = row.Cells[7];
                        row.Cells[7].Selected = true;
                    }

                    string strCCTVInfo = string.Format("{0}. {1}({2}, {3})", strID, strCameraName, strIP, strPort);

                    if (MessageBox.Show("ID나 비밀번호가 비어있는 CCTV가 존재합니다.\r\n이대로 계속 진행할까요?\r\n" + strCCTVInfo, "경고", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                        return;
                    else
                        passEmpty = true;
                }
            }

            this.Cursor = Cursors.WaitCursor;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string strID = row.Cells[0].Value.ToString();
                string strUserID = row.Cells[6].Value.ToString();
                string strPW = row.Cells[7].Value.ToString();

                string strSQL = string.Format("update CCTV set UserID = '{0}', Password = '{1}' where ID = {2}",
                    strUserID, strPW, strID);

                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                {
                    System.Diagnostics.Trace.WriteLine("SQL error");
                    continue;
                }
            }

            this.Cursor = Cursors.Arrow;
            MessageBox.Show("CCTV 정보 변경작업이 완료되었습니다.\r\n변경된 CCTV 정보를 적용하시려면 스마트 재난관리 시스템을 재시작하여 주시기 바랍니다.");
        }
    }
}
