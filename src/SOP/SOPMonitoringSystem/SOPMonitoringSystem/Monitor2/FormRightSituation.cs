using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Data.SqlClient;
using Microsoft.Win32;

namespace SOPDisasterSystem
{
    public partial class FormRightSituation : Form
    {
        private FormMain m_frmMain = null;

        private int m_nCount = 0;

        DataGridView m_dataGrid = new DataGridView();

        ArrayList m_arrEquipment = new ArrayList();

        public FormRightSituation(FormMain main)
        {
            InitializeComponent();
            m_frmMain = main;

            GetCalener();
            InitGrid();
            //SetColumnColor();
            m_arrEquipment = m_frmMain.GetBuildingList();

            tabCtrlSystem.Controls.Remove(tabSensor);
            tabCtrlSystem.Controls.Remove(tabCCTV);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            userControl.DigitText = SOPMonitoringSystem.FlexTimer.Now.ToString("HH:mm:ss");//DateTime.Now.ToString("HH:mm:ss");
        }

        private void GetCalener()
        {
            string[] strMonth = new string[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
            string strToday;
            string /*strMonth = "",*/ strDay = "";
            
            string strWeek = DateTime.Today.DayOfWeek.ToString();
            int nMonth = int.Parse(DateTime.Today.ToString("MM"));
            strDay = DateTime.Today.ToString("dd");

            string strTemp = strWeek.Remove(3, strWeek.Length-3).ToUpper();

            //textBox1.Font = new System.Drawing.Font("Orbit-B BT", 22);
            textBox1.ForeColor = Color.Goldenrod;

            strToday = strTemp + " " + strMonth[nMonth-1] + " " + strDay;
            textBox1.Text = strToday;
        }

        private void InitGrid()
        {
            string[] strValue = new string[] { "재난종류", "발동SOP", "재난위치" };

            for (int i = 0; i < strValue.Length; i++)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();

                cell.Value = strValue[i];
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "";
                gridRow.Cells.Add(cell);

                dataGridSenario.Rows.Add(gridRow);
            }
        }

        private void SetColumnColor()
        {
            for (int i = 0; i < dataGridSenario.RowCount; i++)
            {
                dataGridSenario.Columns[0].DefaultCellStyle.BackColor = Color.AliceBlue;
                dataGridSenario.Columns[1].DefaultCellStyle.BackColor = Color.BlanchedAlmond;
            }
        }
        public void SetEquipmentTree(string strBuilding)
        {
            treeEquipment.Nodes.Clear();
            
            string [] str = strBuilding.Split('\\');
            
            int nTemp = -100;
            string strFloor = "";
            int i = 0;
            foreach (SOPMonitoringSystem.Data_EquipmentInfo dataEquip in m_arrEquipment)
            {
                if (str[0] == dataEquip.GroupName && str[1] == dataEquip.BuildingName)
                {
                    if (nTemp != dataEquip.FloorIndex)
                    {
                        nTemp = dataEquip.FloorIndex;

                        if (nTemp < 0)
                        {
                            strFloor = "지하" + dataEquip.FloorIndex + "층";
                        }
                        else
                        {
                            int nIndex = dataEquip.FloorIndex + 1;
                            strFloor = nIndex.ToString() + "층";
                        }

                        treeEquipment.Nodes.Add(strFloor);
                        i++;
                    }
                    treeEquipment.Nodes[i - 1].Nodes.Add(dataEquip.EquipID);
                }
            }

            treeEquipment.ExpandAll();
        }

        public void GridViewClearSelection()
        {
            dataGridSenario.ClearSelection();
        }

        private void btnEquipment_Click(object sender, EventArgs e)
        {
            
        }

        private void btnCCTV_Click(object sender, EventArgs e)
        {

        }

        private void btnSensor_Click(object sender, EventArgs e)
        {

        }

        public void AddScenarioTab(string strData)
        {
            string[] strValue = strData.Split('/');

            if (m_nCount == 0)
            {
                m_nCount++;
                tabPage.Text = strValue[1];
                AddGridData(dataGridSenario, strData);
            }
            else
            {
                m_nCount++;
                tabPage = new TabPage();

                tabPage.Location = new System.Drawing.Point(4, 22);
                tabPage.Name = "tabPage";
                tabPage.Padding = new System.Windows.Forms.Padding(3);
                tabPage.Size = new System.Drawing.Size(224, 141);
                tabPage.Text = "SOP";
                tabPage.UseVisualStyleBackColor = true;

                tabCtrlScenario.Controls.Add(tabPage);

                NewScenario(tabPage);
                tabPage.Text = strValue[1];
                
                AddGridData(m_dataGrid, strData);
            }
        }
        
        public void DeleteScenarioTab(string strData, int nIndex)
        {
            m_nCount--;
            if(tabCtrlScenario.TabCount > 1)
                tabCtrlScenario.Controls.RemoveAt(nIndex);

        }

        public void AddGridData(DataGridView dataGrid, string strData)
        {
            string[] strValue = strData.Split('/');

            dataGrid.Rows[0].Cells[1].Value = strValue[0];
            dataGrid.Rows[1].Cells[1].Value = strValue[1] + "/" + strValue[2];
            dataGrid.Rows[2].Cells[1].Value = "건물ID,건물이름";
        }
        
        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 텝 추가 후 Pane 생성 및 Data 출력 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 

        // Panel 생성
        private void NewScenario(TabPage tabPage)
        {
            Panel panel = new Panel();
            panel.BackColor = System.Drawing.Color.MistyRose;
            NewLabel2(panel);
            NewGrid(panel);
            NewLabel1(panel);
            panel.Dock = System.Windows.Forms.DockStyle.Fill;
            panel.Location = new System.Drawing.Point(3, 3);
            panel.Name = "panel";
            panel.Size = new System.Drawing.Size(218, 135);
            panel.ResumeLayout(false);
            panel.PerformLayout();

            tabPage.Controls.Add(panel);
        }

        // label 생성
        private void NewLabel1(Panel panel)
        {
            Label label = new Label();
            label.AutoSize = true;
            label.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            label.Location = new System.Drawing.Point(21, 4);
            label.Name = "label1";
            label.Size = new System.Drawing.Size(176, 19);
            label.Text = "위기관리 활동단계";

            panel.Controls.Add(label);
        }

        // label2 생성
        private void NewLabel2(Panel panel)
        {
            Label label = new Label();
            label.AutoSize = true;
            label.Font = new System.Drawing.Font("굴림", 21.75F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(129)));
            label.ForeColor = System.Drawing.Color.DodgerBlue;
            label.Location = new System.Drawing.Point(73, 30);
            label.Name = "label";
            label.Size = new System.Drawing.Size(73, 29);
            label.Text = "관심";

            panel.Controls.Add(label);
        }

        // grid 생성 및 초기 column 입력
        private void NewGrid(Panel panel)
        {
            // 
            // Column2
            // 
            DataGridViewTextBoxColumn column2 = new DataGridViewTextBoxColumn();
            column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            column2.HeaderText = "column2";
            column2.Name = "column2";
            column2.ReadOnly = true;
            // 
            // Column1
            // 
            DataGridViewTextBoxColumn column1 = new DataGridViewTextBoxColumn();
            column1.Frozen = true;
            column1.HeaderText = "column1";
            column1.Name = "column1";
            column1.ReadOnly = true;
            column1.Width = 70;

            DataGridView dataGrid = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(dataGrid)).BeginInit();

            dataGrid.AllowUserToAddRows = false;
            dataGrid.AllowUserToDeleteRows = false;
            dataGrid.AllowUserToResizeColumns = false;
            dataGrid.AllowUserToResizeRows = false;
            dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGrid.ColumnHeadersVisible = false;
            dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {column1,column2});
            dataGrid.Dock = System.Windows.Forms.DockStyle.Bottom;
            dataGrid.Enabled = false;
            dataGrid.Location = new System.Drawing.Point(0, 63);
            dataGrid.MultiSelect = false;
            dataGrid.Name = "dataGridSenario";
            dataGrid.ReadOnly = true;
            dataGrid.RowHeadersVisible = false;
            dataGrid.RowTemplate.Height = 23;
            dataGrid.Size = new System.Drawing.Size(218, 72);

            panel.Controls.Add(dataGrid);

            ((System.ComponentModel.ISupportInitialize)(dataGrid)).EndInit();

            string[] strValue = new string[] { "재난종류", "발동SOP", "재난위치" };

            for (int i = 0; i < strValue.Length; i++)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();

                cell.Value = strValue[i];
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "";
                gridRow.Cells.Add(cell);

                dataGrid.Rows.Add(gridRow);
            }

            m_dataGrid = dataGrid;
        }

    }
}
