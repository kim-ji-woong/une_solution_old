using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPMonitoringSystem
{
    public partial class FormLeftScenario : Form
    {
        private FormMain m_frmMain = null;

        public FormLeftScenario(FormMain main)
        {
            InitializeComponent();
            m_frmMain = main;
        }

        public void AddGridRowScenario(string strPath)
        {
            int nRowIndex = 0;

            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();

            cell.Value = strPath;
            gridRow.Cells.Add(cell);

            if (dataGridScenario.Rows.Count == 0)
            {
                dataGridScenario.Rows.Add(gridRow);
                nRowIndex = cell.RowIndex;

                SOPDisasterSystem.FormRightSituation frmSituation = m_frmMain.GetMonitor2().GetSituation();
                if (frmSituation == null) return;
                frmSituation.AddScenarioTab(strPath);
            }
            else
            {
                bool isSame = false;
                foreach (DataGridViewRow row in dataGridScenario.Rows)
                {
                    if (strPath == row.Cells[0].Value.ToString())
                    {
                        isSame = true;
                        nRowIndex = row.Index;
                        cell = row.Cells[0];
                        break;
                    }
                }
                if(!isSame)
                {
                    dataGridScenario.Rows.Add(gridRow);
                    nRowIndex = cell.RowIndex;

                    SOPDisasterSystem.FormRightSituation frmSituation = m_frmMain.GetMonitor2().GetSituation();
                    if (frmSituation == null) return;
                    frmSituation.AddScenarioTab(strPath);
                }
            }

            //nRowIndex = dataGridScenario.;
            SetFontStyle(cell);
            dataGridScenario.Rows[nRowIndex].Cells[0].Selected = true;
        }

        // Return 값 : 삭제된 행의 Index
        //             삭제되지 않을 경우 -1을 리턴
        public int DeleteGridRowScenario(string strPath)
        {
            int nDeletedIndex = -1;
            int nRowCount = dataGridScenario.Rows.Count;

            for (int i=0;i<nRowCount;i++)
            //foreach (DataGridViewRow row in dataGridScenario.Rows)
            {
                DataGridViewRow row = dataGridScenario.Rows[i];

                if (strPath == row.Cells[0].Value.ToString())
                {
                    dataGridScenario.Rows.Remove(row);
                    nDeletedIndex = i;

                    SOPDisasterSystem.FormRightSituation frmSituation = m_frmMain.GetMonitor2().GetSituation();
                    if (frmSituation == null) break;
                    frmSituation.DeleteScenarioTab(strPath, nDeletedIndex);

                    break;
                }
            }

            foreach (DataGridViewCell cell in dataGridScenario.SelectedCells)
            {
                SetFontStyle(cell);
            }

            return nDeletedIndex;
        }

        public void SetFontStyle(DataGridViewCell cell)
        {
            foreach (DataGridViewRow row in dataGridScenario.Rows)
            {
                row.Cells[0].Style.ForeColor = Color.Black;
                row.Cells[0].Style.Font = new Font("Tahoma", 8, FontStyle.Regular);
            }

            cell.Style.ForeColor = Color.Red;
            cell.Style.Font = new Font("Tahoma", 8, FontStyle.Bold);
        }

        private void dataGridScenario_CellLClick()
        {
            foreach (DataGridViewCell cell in dataGridScenario.SelectedCells)
            {
                SetFontStyle(cell);

                TreeNode node = m_frmMain.GetDisaster().FindNode(cell.Value.ToString());
                m_frmMain.GetProcess().SetCurrentNode(node, true);
            }
        }

        private void dataGridScenario_CellRClick(int x, int y)
        {
            rButtonMenu.Show(dataGridScenario, new Point(x, y));
        }

        private void dataGridScenario_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                dataGridScenario_CellLClick();
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                dataGridScenario_CellRClick(e.X, e.Y);
        }

        public DataGridView GetGridView()
        {
            return dataGridScenario;
        }

        public int IndexOf(string strFullName)
        {
            int nRowCount = dataGridScenario.RowCount;

            for (int i=0;i<nRowCount;i++)
            {
                if (strFullName == (string)dataGridScenario.Rows[i].Cells[0].Value)
                    return i;
            }

            return -1;
        }

        private void deleteMenuItem_Click(object sender, EventArgs e)
        {
            bool isFirst = true;
            ArrayList arrRemove = new ArrayList();

            foreach (DataGridViewCell cell in dataGridScenario.SelectedCells)
            {
                TreeNode node = m_frmMain.GetDisaster().FindNode(cell.Value.ToString());
                TimeLine.PROCESS_STATUS status = m_frmMain.GetProcess().GetSOPStatus(node);

                if (isFirst)
                {
                    if (status != TimeLine.PROCESS_STATUS.NO_WORKED && status != TimeLine.PROCESS_STATUS.COMPLETE)
                    {
                        DialogResult result = MessageBox.Show("프로세스가 실행중입니다.\r\n중단시키고 리스트에서 삭제할까요?", "프로세스 종료", MessageBoxButtons.YesNo);

                        if (result == DialogResult.No)
                            return;
                    }

                    isFirst = false;
                }

                arrRemove.Add(cell.RowIndex);
            }

            arrRemove.Sort();

            int nRemoveCount = arrRemove.Count;

            for (int i = nRemoveCount - 1; i >= 0; i--)
            {
                int nRowIndex = (int)arrRemove[i];
                DataGridViewCell cell = dataGridScenario.Rows[nRowIndex].Cells[0];

                TreeNode node = m_frmMain.GetDisaster().FindNode(cell.Value.ToString());
                m_frmMain.GetProcess().RemoveSOP(node);

                dataGridScenario.Rows.RemoveAt(nRowIndex);
            }

            if (nRemoveCount > 0)
            {
                if (dataGridScenario.SelectedCells.Count > 0)
                {
                    DataGridViewCell cell = dataGridScenario.SelectedCells[0];
                    TreeNode node = m_frmMain.GetDisaster().FindNode(cell.Value.ToString());

                    if (node != null)
                        m_frmMain.GetProcess().SetCurrentNode(node, false);
                }
                else
                {
                    TreeNode node = m_frmMain.GetDisaster().GetSelectedNode();

                    string strFullPath;
                    int nDepth = m_frmMain.GetDisaster().GetNodeText(node, out strFullPath);

                    m_frmMain.OnSelectedSOP(nDepth, strFullPath, node);
                }

                m_frmMain.GetProcess().Refresh();
            }
        }

        private void dataGridScenario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteMenuItem_Click(null, null);
            }
        }
    }
}
