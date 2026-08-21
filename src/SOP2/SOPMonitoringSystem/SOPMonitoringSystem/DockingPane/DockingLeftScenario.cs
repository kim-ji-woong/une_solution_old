using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using XtremeShortcutBar;
using System.IO;

namespace SOPMonitoringSystem
{
    public partial class DockingLeftScenario : Form
    {
        private BarLevelTree m_barTree = null;
        private BarPage m_barPage = null;
        
        private bool m_isAllStop = false;

        private bool m_isLoadComponentHistory = false;

        public bool FinishLoadingComponentHistory
        {
            get { return m_isLoadComponentHistory; }
        }

        private ArrayList m_arrLoadHistory = new ArrayList();
        public ArrayList ArrLoadHistory
        {
            get { return m_arrLoadHistory; }
            set { m_arrLoadHistory = value; }
        }
        // 초기 로딩시 DB에 저장된 History를 불러와서 SOP Log 창에 생성된 Log의 개수
        private int m_nInitHistoryLogCount = 0;

        private ArrayList m_arrHistory = new ArrayList();
        public ArrayList ArrHistory
        {
            get { return m_arrHistory; }
            set { m_arrHistory = value; }
        }

        public bool bDeleteMenuEnable = false;
        public bool DeleteMenuEnable
        {
            get { return bDeleteMenuEnable; }
            set
            {
                bDeleteMenuEnable = value;
                if (bDeleteMenuEnable == true)
                {
                    deleteMenuItem.Enabled = true;
                }
                else
                {
                    deleteMenuItem.Enabled = false;
                }
            }
        }

        public int ScenarioCount
        {
            get { return dataGridScenario.Rows.Count; }
        }

        public DockingLeftScenario()
        {
            InitializeComponent();

            CreateShortcutBar();
            InitImage();
            //deleteMenuItem.Enabled = !Sections.WorkFlowManager.Instance.DeleteComplete;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (this.Visible)
            {
                FormMain.Instance.FrmMain2.ApplyWindow(this.Handle.ToInt32());

                m_barPage.VisibleChange(e);
                dataGridScenario.BackgroundColor = Color.White;
                dataGridScenario.RowsDefaultCellStyle.SelectionBackColor = Color.White;
                dataGridScenario.RowsDefaultCellStyle.SelectionForeColor = Color.Red;
                    
            }
        }

        public void EnableControl(bool enabled)
        {
            this.dataGridScenario.Enabled = enabled;
            this.GetBarLevelTree().Enabled = enabled;
        }

        public void DeleteOptionChanged(object sender, DeleteOptionChangeEventArgs e)
        {
            deleteMenuItem.Enabled = !Sections.WorkFlowManager.Instance.DeleteComplete;

            int nTargetActionStep = -1;
            ArrayList activeList = new ArrayList();
            activeList.AddRange(Sections.TabPageManager.Instance.GetAliveList(true));
            activeList.AddRange(Sections.TabPageManager.Instance.GetAliveList(false));
            foreach (int id in activeList)
            {
                nTargetActionStep = id;
                if (nTargetActionStep == -1)
                    continue;
                Sections.SectionTabPage page = (Sections.SectionTabPage)Sections.TabPageManager.Instance.GetPage(nTargetActionStep, true);
                if (page != null)
                {
                    if (page.State == Sections.TabPageState.NOUSE)
                    {
                        foreach (DataGridViewRow row in dataGridScenario.Rows)
                        {
                            bool deleterow = false;
                            
                            int nActionStepID = (int)row.Cells[3].Tag;
                            if (nTargetActionStep == nActionStepID)
                            {
                                deleterow = true;
                            }
                            
                            if (deleterow)
                            {
                                dataGridScenario.Rows.Remove(row);
                                Sections.TabPageManager.Instance.RemovePage(nTargetActionStep, true);
                                Sections.WorkFlowManager.Instance.Remove(nTargetActionStep, true);
                            }
                        }
                    }
                }
                page = (Sections.SectionTabPage)Sections.TabPageManager.Instance.GetPage(nTargetActionStep, false);
                if (page != null)
                {
                    if (page.State == Sections.TabPageState.NOUSE)
                    {
                        foreach (DataGridViewRow row in dataGridScenario.Rows)
                        {
                            bool deleterow = false;
                            
                            int nActionStepID = (int)row.Cells[3].Tag;
                            if (nTargetActionStep == nActionStepID)
                            {
                                deleterow = true;
                            }
                            
                            if (deleterow)
                            {
                                dataGridScenario.Rows.Remove(row);
                                Sections.TabPageManager.Instance.RemovePage(nTargetActionStep, false);
                                Sections.WorkFlowManager.Instance.Remove(nTargetActionStep, false);
                            }
                        }
                    }
                }
            }
        }

        private void CreateShortcutBar()
        {
            m_barTree = new BarLevelTree();
            m_barPage = new BarPage();

            ShortcutBarItem ItemTree = axShortcutBar.AddItem(ID.ID_SHORTCUT_LEVELTREE, "재난 Tree", m_barTree.Handle.ToInt32());
            ShortcutBarItem ItemPage = axShortcutBar.AddItem(ID.ID_SHORTCUT_PAGE, "페이지", m_barPage.Handle.ToInt32());

            axShortcutBar.Selected = ItemTree;
            axShortcutBar.ExpandedLinesCount = 2;

//             m_nActiopnStepID = LastActionStepID();
//             //tabPage1.Tag = ++m_nActiopnStepID;
//             ++m_nActiopnStepID;
//             m_arrTabPage.Add(tabPage1);
// 
//             m_propertiesLevel.GetLevelProperties(tabPage1);
// 
//             Data_ActionStep data = new Data_ActionStep();
//             data.StepName = tabPage1.Text;
//             //data.ParentStepID = (int)tabPage1.Tag;
//             data.ParentStepID = -1;
//             m_propertiesLevel.LevelProperties.Add(data);

        }

        //////////////////////////////////////////////////////////////////////////
        public BarLevelTree GetBarLevelTree()
        {
            return m_barTree;
        }

        public BarPage GetBarPage()
        {
            return m_barPage;
        }
                
        public void DeleteRow(int nActionStepID)
        {
            foreach (DataGridViewRow row in dataGridScenario.Rows)
            {
                if (nActionStepID == (int)(row.Cells[3].Tag))
                {
                    lock (m_arrLoadHistory)
                    {
                        int nActionStepHistoryID = (int)row.Tag;
                        m_arrLoadHistory.Remove(nActionStepHistoryID);
                        dataGridScenario.Rows.Remove(row);
                    }

                    break;
                }
            }
        }

        private Image m_imgReal = null, m_imgTrain = null, m_imgRegister = null, m_imgNonRegister = null, m_imgWeekday = null, m_imgWeekend = null;
        private void InitImage()
        {
            Bitmap bmpLayer = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.layer_mode);

            ImageList imgListLayer = new ImageList();
            imgListLayer.ImageSize = new Size(16, 16);
            imgListLayer.Images.AddStrip(bmpLayer);

            m_imgReal = imgListLayer.Images[0];
            m_imgTrain = imgListLayer.Images[1];
            m_imgRegister = imgListLayer.Images[2];
            m_imgNonRegister = imgListLayer.Images[3];
            m_imgWeekday = imgListLayer.Images[4];
            m_imgWeekend = imgListLayer.Images[5];
        }

        public int FindRowIndex(int nActionStepID, bool isReal)
        {
            int nRowCount = dataGridScenario.Rows.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                DataGridViewRow row = dataGridScenario.Rows[i];

                if ((bool)row.Cells[0].Tag != isReal)
                    continue;

                if ((int)row.Cells[3].Tag == nActionStepID)
                    return i;
            }

            return -1;
        }

        public void SelectRow(int nRowIndex)
        {
            int nRowCount = dataGridScenario.Rows.Count;

            if (nRowIndex >= nRowCount)
                return;

            dataGridScenario.Rows[nRowIndex].Selected = true;
        }

        public void AddGridRowScenario(string strPath, int nActionStepID, bool bReal, int nActionStepHistoryID)
        {
            bool bRegular = FormMain.Instance.IsRegular;
            bool bNormal = FormMain.Instance.IsNormal;
            AddGridRowScenario(strPath, nActionStepID, bReal, bRegular, bNormal, nActionStepHistoryID);
        }

        public void AddGridRowScenario(string strPath, int nActionStepID, bool bReal, bool bRegular, bool bNormal, int nActionStepHistoryID)
        {
            int nRowIndex = 0;

            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            
            cell = new DataGridViewImageCell();
            cell.Value = bReal ? m_imgReal : m_imgTrain;
            cell.Tag = bReal;
            gridRow.Cells.Add(cell);
            
            cell = new DataGridViewImageCell();
            cell.Value = bRegular ? m_imgRegister : m_imgNonRegister;
            cell.Tag = bRegular;
            gridRow.Cells.Add(cell);
            
            cell = new DataGridViewImageCell();
            cell.Value = bNormal ? m_imgWeekday : m_imgWeekend;
            cell.Tag = bNormal;
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();

			strPath = strPath.Replace((char)0x06, '/');
            cell.Value = strPath;
            if (bReal == false)
                cell.Value = strPath + "(훈련모드)";

            cell.Tag = nActionStepID;
            gridRow.Cells.Add(cell);
            gridRow.Tag = nActionStepHistoryID;

            lock (m_arrLoadHistory)
            {
                m_arrLoadHistory.Add(nActionStepHistoryID);
            }

            if (dataGridScenario.Rows.Count == 0)
            {
                dataGridScenario.Rows.Add(gridRow);
                nRowIndex = cell.RowIndex;

                //SOPDisasterSystem.FormRightSituation frmSituation = m_frmMain.GetMonitor2().GetSituation();
                //if (frmSituation == null) return;
                //frmSituation.AddScenarioTab(strPath);
            }
            else
            {
                bool isSame = false;
                foreach (DataGridViewRow row in dataGridScenario.Rows)
                {
                    if (((string)cell.Value == (string)row.Cells[3].Value) && ((int)cell.Tag) == (int)row.Cells[3].Tag)
                    {
                        isSame = true;
                        nRowIndex = row.Index;
                        cell = row.Cells[3];
                        break;
                    }
                }
                if (!isSame)
                {
                    dataGridScenario.Rows.Add(gridRow);
                    nRowIndex = cell.RowIndex;

                    //SOPDisasterSystem.FormRightSituation frmSituation = m_frmMain.GetMonitor2().GetSituation();
                    //if (frmSituation == null) return;
                    //frmSituation.AddScenarioTab(strPath);
                }
            }
            SetFontStyle(cell);
            dataGridScenario.Rows[nRowIndex].Selected = true;
        }

        // Return 값 : 삭제된 행의 Index
        //             삭제되지 않을 경우 -1을 리턴
        public int DeleteGridRowScenario(string strPath)
        {
            int nDeletedIndex = -1;
            int nRowCount = dataGridScenario.Rows.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                DataGridViewRow row = dataGridScenario.Rows[i];

                if (strPath == row.Cells[3].Value.ToString())
                {
                    int nActionStepHistoryID = (int)row.Tag;

                    lock (m_arrLoadHistory)
                    {
                        m_arrLoadHistory.Remove(nActionStepHistoryID);
                    }

                    dataGridScenario.Rows.Remove(row);
                    nDeletedIndex = i;
                    //SOPDisasterSystem.FormRightSituation frmSituation = m_frmMain.GetMonitor2().GetSituation();
                    //if (frmSituation == null) break;
                    //frmSituation.DeleteScenarioTab(strPath, nDeletedIndex);

                    if (dataGridScenario.SelectedRows.Count > 0)
                    {
                        DataGridViewRow rowSelected = dataGridScenario.SelectedRows[0];
                        FormMain.Instance.WriteCurrentActionStepID((int)rowSelected.Cells[3].Tag, (bool)rowSelected.Cells[0].Tag);
                    }
                    else
                    {
                        if (dataGridScenario.Rows.Count > 0)
                        {
                            int nIndex = dataGridScenario.Rows.Count - 1;
                            SelectRow(nIndex);

                            DataGridViewRow rowSelected = dataGridScenario.SelectedRows[nIndex];
                            FormMain.Instance.WriteCurrentActionStepID((int)rowSelected.Cells[3].Tag, (bool)rowSelected.Cells[0].Tag);
                        }
                    }

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
                row.Cells[3].Style.ForeColor = Color.Black;
                row.Cells[3].Style.Font = new Font("Tahoma", 8, FontStyle.Regular);
            }

            cell.Style.ForeColor = Color.Red;
            cell.Style.Font = new Font("Tahoma", 8, FontStyle.Bold);
        }

        private bool IsVirtualMode(string name)
        {
            if( name == null)
                return false;
            if (name.IndexOf("훈련모드") != -1)
                return true;
            return false;
        }

        private void dataGridScenario_CellLClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            if (e.RowIndex < 0)
                return;
            
            DataGridViewCell cell = (grid.Rows[e.RowIndex].Cells[3]);
            SetSelectedGridRow(cell, e.Button, grid.Rows[e.RowIndex]);

            //int nActionStepID = (int)cell.Tag;
            //VersionInfo ainfo = FormMain.Instance.SOPManager.GetActionStepVersionInfo(nActionStepID);
            //ActionStepInfo info = FormMain.Instance.SOPManager.GetActionStepInfo(nActionStepID);
            //FormMain.Instance.ChangeMode(ainfo, info);

            //string szText = cell.Value.ToString();
            //bool bVirutalMode = IsVirtualMode(szText);
            //FormMain.Instance.VirtualMode(bVirutalMode);            

            //BarLevelTree tree = GetBarLevelTree();
            //if (tree != null)
            //{
            //    TreeNode node = tree.FindActionStepNode(nActionStepID);
            //    if (node != null)
            //    {
            //        if (tree.TreeView.SelectedNode != null)
            //            tree.TreeView.SelectedNode.ForeColor = Color.Black;
            //        tree.SelectNode(node);
            //        tree.SelectSop(node);
            //        node.ForeColor = Color.Red;
            //    }
            //}
            //SetFontStyle(cell);
        }
        private int gx = 0;
        private int gy = 0;
        private void dataGridScenario_CellRClick(int x, int y)
        {
            rButtonMenu.Show(dataGridScenario, new Point(gx, gy));
        }

        private void dataGridScenario_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                dataGridScenario_CellLClick(sender, e);

            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                // exclude header
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    SetSelectedGridRow(dataGridScenario.Rows[e.RowIndex].Cells[3], e.Button, dataGridScenario.Rows[e.RowIndex]);
                    dataGridScenario.Rows[e.RowIndex].Selected = true;
                    
                    dataGridScenario_CellRClick(e.X, e.Y);
                }
            }            
            
        }

        private void SetSelectedGridRow(DataGridViewCell cell, MouseButtons mouse, DataGridViewRow row)
        {
            int nActionStepID = (int)cell.Tag;
            string strValue = cell.Value.ToString();

            VersionInfo ainfo = FormMain.Instance.SOPManager.GetActionStepVersionInfo(nActionStepID);
            ActionStepInfo info = FormMain.Instance.SOPManager.GetActionStepInfo(nActionStepID);
            FormMain.Instance.ChangeMode(ainfo, info, (bool)row.Cells[0].Tag);

            bool bVirutalMode = IsVirtualMode(strValue);
            FormMain.Instance.VirtualMode(bVirutalMode);       

            BarLevelTree tree = GetBarLevelTree();
            if (tree != null)
            {
                TreeNode node = tree.FindActionStepNode(nActionStepID);
                if (node != null)
                {
                    if (tree.TreeView.SelectedNode != null)
                        tree.TreeView.SelectedNode.ForeColor = Color.Black;

                    bool isRealMode = (bool)row.Cells[0].Tag;

                    if (PageBackstageHome.IsWorkingMode(info.ActionStepID, isRealMode))
                    {
                        if (FormMain.Instance.HasControl == true)
                        {
                            // 현재 화면에 나타나고 있는 ActionStep을 기록한다.
                            FormMain.Instance.WriteCurrentActionStepID(nActionStepID, isRealMode);
                        }                 
                    }
                    tree.SelectNode(node);
                    tree.SelectSop(node);
                    node.ForeColor = Color.Red;

                    tree.IgnoreSelect = false;
                }
            }
            SetFontStyle(cell);

            if (mouse == MouseButtons.Right)
            {
                Sections.WorkFlowState state = FormMain.Instance.CheckWorkflow(nActionStepID, bVirutalMode);
                toolStripMenuDisable(state);
            }
        }

        public void SetSelectedGridRow()
        {
            int nRow = dataGridScenario.RowCount;
            if (nRow != 0)
            {
                int nSelectRow = nRow - 1;
                SetSelectedGridRow(dataGridScenario.Rows[nSelectRow].Cells[3], System.Windows.Forms.MouseButtons.Left, dataGridScenario.Rows[nSelectRow]);
            }
        }

        public void SelectedGridRow(int nActionStepID, bool isRealMode)
        {
            dataGridScenario.ClearSelection();

            foreach (DataGridViewRow row in dataGridScenario.Rows)
            {
                //row.Selected = false;
                if (nActionStepID == (int)row.Cells[3].Tag && isRealMode == (bool)row.Cells[0].Tag)
                {
                    row.Selected = true;
                    SetSelectedGridRow(row.Cells[3], System.Windows.Forms.MouseButtons.Left, row);

                    SetFontStyle(row.Cells[3]);
                    break;
                }
            }
        }

        // isRegular : 등록된 버전인가?
        // isNormal : 평일 버전인가?
        // Return 값 : 현재 실행중인 SOP의 FullPath
        //             현재 실행중인 것이 없을 경우 빈 문자열을 리턴
        public string GetCurrentSOPInfo(out int nActionStepID, out bool isReal, out bool isRegular, out bool isNormal)
        {
            if (dataGridScenario.SelectedRows == null || dataGridScenario.SelectedRows.Count == 0)
            {
                nActionStepID = -1;
                isReal = isRegular = isNormal = true;
                return "";
            }

            DataGridViewRow row = dataGridScenario.SelectedRows[0];

            isReal = (bool)row.Cells[0].Tag;
            isRegular = (bool)row.Cells[1].Tag;
            isNormal = (bool)row.Cells[2].Tag;
            nActionStepID = (int)row.Cells[3].Tag;

            return (string)row.Cells[3].Value;
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

        private void dataGridScenario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteMenuItem_Click(null, null);
            }
        }

        private void deleteMenuItem_Click(object sender, EventArgs e)
        {
            int nTargetActionStep = -1;
            string szName = "";
            bool isTargetRealMode = true;

            foreach (DataGridViewRow row in dataGridScenario.SelectedRows)
            {
                nTargetActionStep = (int)row.Cells[3].Tag;
                szName = row.Cells[3].Value.ToString();
                isTargetRealMode = (bool)row.Cells[0].Tag;
                break;
            }
            

            if (nTargetActionStep == -1)
                return;

            bool bReal = ((szName.IndexOf("훈련모드") == -1 )? true : false) ;

            Sections.SectionTabPage page = (Sections.SectionTabPage)Sections.TabPageManager.Instance.GetPage(nTargetActionStep, bReal);
            RemoveTabPage(page);
            /*if (page != null)
            {
                if (page.State == Sections.TabPageState.NOUSE)
                {
                    foreach (DataGridViewRow row in dataGridScenario.Rows)
                    {
                        bool deleterow = false;
                        //foreach (DataGridViewCell cell in row.Cells)
                        {
                            int nActionStepID = (int)row.Cells[3].Tag;
                            if (nTargetActionStep == nActionStepID && isTargetRealMode == (bool)row.Cells[0].Tag)
                            {
                                deleterow = true;
                                //break;
                            }
                        }

                        if (deleterow == true)
                        {

                            dataGridScenario.Rows.Remove(row);                            
                            BarLevelTree tree = GetBarLevelTree();
                            if (tree != null)
                            {
                                tree.ResetSelect();
                                tree.UnSelectedNode();                            
                            }                    
                            Sections.TabPageManager.Instance.RemovePage(nTargetActionStep, bReal);
                            Sections.WorkFlowManager.Instance.Remove(nTargetActionStep, bReal);

                            FormMain.Instance.GetPageHome().ClearComponentContents(nTargetActionStep, bReal);

                            FormMain.Instance.GetPageHome().RemoveTabPage(page);
                            FormMain.Instance.GetPageHome().PanelArray.Clear();
                            dataGridScenario.Update();
                            break;
                        }
                    }
                }
            }*/

            AfterRemoveTabPage();
        }




        public bool RemoveTabPage(Sections.SectionTabPage page)
        {
            if (page != null)
            {
                int nTargetActionStep = page.ActionStepID;
                bool isTargetRealMode = !page.VirtualMode;

                if (page.State == Sections.TabPageState.NOUSE)
                {
                    foreach (DataGridViewRow row in dataGridScenario.Rows)
                    {
                        bool deleterow = false;
                        //foreach (DataGridViewCell cell in row.Cells)
                        {
                            int nActionStepID = (int)row.Cells[3].Tag;
                            if (nTargetActionStep == nActionStepID && isTargetRealMode == (bool)row.Cells[0].Tag)
                            {
                                deleterow = true;
                                //break;
                            }
                        }

                        if (deleterow == true)
                        {
                            int nActionStepHistoryID = (int)row.Tag;

                            lock (m_arrLoadHistory)
                            {
                                m_arrLoadHistory.Remove(nActionStepHistoryID);
                            }

                            dataGridScenario.Rows.Remove(row);
                            BarLevelTree tree = GetBarLevelTree();
                            if (tree != null)
                            {
                                tree.ResetSelect();
                                tree.UnSelectedNode();
                            }
                            Sections.TabPageManager.Instance.RemovePage(nTargetActionStep, isTargetRealMode);
                            Sections.WorkFlowManager.Instance.Remove(nTargetActionStep, isTargetRealMode);

                            FormMain.Instance.GetPageHome().ClearComponentContents(nTargetActionStep, isTargetRealMode);

                            FormMain.Instance.GetPageHome().RemoveTabPage(page);
                            FormMain.Instance.GetPageHome().PanelArray.Clear();
                            dataGridScenario.Update();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void AfterRemoveTabPage()
        {
            if (dataGridScenario.Rows.Count == 0)
            {
                FormMain.Instance.GetPageHome().TabControls.TabPages.Clear();
                FormMain.Instance.GetPageHome().TabControls.SelectedTab = null;
                //FormMain.Instance.GetPageHome().TabControls.Visible = false;
                FormMain.Instance.GetPageHome().panel.Visible = false;
                FormMain.Instance.GetPageHome().SetBackgroundImage(false);
                FormMain.Instance.WaitWorkflow();
                FormMain.Instance.GetPageHome().ClearProcess();

                BarLevelTree tree = GetBarLevelTree();
                if (tree != null)
                {
                    tree.ResetSelect();
                    tree.UnSelectedNode();
                }
            }
            else
            {
                if (dataGridScenario.SelectedRows.Count == 0)
                {
                    dataGridScenario.Rows[0].Selected = true;
                }

				DataGridViewRow workingRow = null;
                foreach (DataGridViewRow row in dataGridScenario.SelectedRows)
                {
                    int nActionStepID = (int)row.Cells[3].Tag;
                    bool isRealMode = (bool)row.Cells[0].Tag;

                    if (!PageBackstageHome.IsWorkingMode(nActionStepID, isRealMode))
                        continue;

					workingRow = row;
				}

				if( workingRow == null)
				{
					workingRow = dataGridScenario.Rows[0];
				}
				
				if( workingRow != null)
				{
					int nActionStepID = (int)workingRow.Cells[3].Tag;
					bool isRealMode = (bool)workingRow.Cells[0].Tag;
                    VersionInfo ainfo = FormMain.Instance.SOPManager.GetActionStepVersionInfo(nActionStepID);
                    ActionStepInfo info = FormMain.Instance.SOPManager.GetActionStepInfo(nActionStepID);
					FormMain.Instance.ChangeMode(ainfo, info, (bool)workingRow.Cells[0].Tag);

					string szText = workingRow.Cells[3].Value.ToString();
                    bool bVirutalMode = IsVirtualMode(szText);
                    FormMain.Instance.VirtualMode(bVirutalMode);

                    BarLevelTree tree = GetBarLevelTree();
                    if (tree != null)
                    {
                        TreeNode node = tree.FindActionStepNode(nActionStepID);
                        if (node != null)
                        {
                            if (tree.TreeView.SelectedNode != null)
                                tree.TreeView.SelectedNode.ForeColor = Color.Black;
                            tree.SelectNode(node);
                            tree.SelectSop(node);
                            node.ForeColor = Color.Red;

                            FormMain.Instance.WriteCurrentActionStepID(nActionStepID, isRealMode);

							SetFontStyle(workingRow.Cells[3]);
                        }
                    }
					SetFontStyle(workingRow.Cells[3]);
                }
            }
        }

        private void dataGridScenario_MouseDown(object sender, MouseEventArgs e)
        {            
            gx = e.X;
            gy = e.Y;     
        }

        private string GetActionStepPath(ArrayList arrActionSteps, int nActionStepID)
        {
            string strPath = "";

            foreach (ActionStepInfo actionStep in arrActionSteps)
            {
                if (actionStep.ActionStepID == nActionStepID)
                {
                    strPath = actionStep.ActionStepName;

                    if (actionStep.ParentStepID > 0)
                    {
                        string strParentPath = GetActionStepPath(arrActionSteps, actionStep.ParentStepID);
                        if (strParentPath.Length > 0)
							strPath = strParentPath + '/' + strPath;
                    }

                    return strPath;
                }
            }

            return strPath;
        }

        // 실행중인 ActionStepHistory 목록을 얻어온다.
        // Key : 양수이면 실제모드의 ActionStepID
        //       음수이면 훈련모드의 ActionStepID
        private bool LoadActionStepHistory(WebDBManager dbMgr, Dictionary<int, Data_ActionStepHistory> dicActionStepHistories)
        {
            string strSQL = "select id, ActionStepID, RealMode, BeginTime, DetectTime from ActionStepHistory where EndTime is null and CancelTime is null";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            DateTime dtDefault = new DateTime();

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nActionStepID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                bool isRealMode = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0) == 0 ? false : true;
                DateTime dtBegin = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                DateTime dtDetect = WebDBManager.GetDateTimeField(arrResult[i + 4], dtDefault);

                if (nID < 0 || nActionStepID < 0)
                    continue;

                if (!isRealMode)
                    nActionStepID = -nActionStepID;

                if (dicActionStepHistories.ContainsKey(nActionStepID))
                {
                    Data_ActionStepHistory history = dicActionStepHistories[nActionStepID];

                    if (history.BeginTime < dtBegin)
                    {
                        history.ID = nID;
                        history.ActionStepID = System.Math.Abs(nActionStepID);
                        history.BeginTime = dtBegin;
                        history.DetectTime = dtDetect;
                        history.RealMode = isRealMode;
                    }
                }
                else
                {
                    Data_ActionStepHistory history = new Data_ActionStepHistory();

                    history.ID = nID;
                    history.ActionStepID = System.Math.Abs(nActionStepID);
                    history.BeginTime = dtBegin;
                    history.DetectTime = dtDetect;
                    history.RealMode = isRealMode;

                    dicActionStepHistories[nActionStepID] = history;
                }
            }

            return true;
        }

        // 기존의 LoadHistory(...)가 너무 많은 SubQuery로 인하여 DB 부하가 많은 관계로 SubQuery를 사용하지 않고 
        // dicActionStepHistories를 사용하는 버전으로 변경
        private bool LoadHistory(WebDBManager dbMgr, Dictionary<string, DisasterInfo> dicData, Dictionary<int, Data_ActionStepHistory> dicActionStepHistories, bool isRealMode, bool isRegular, bool isNormal)
        {
            // ActionStep ID, Disaster
            Dictionary<int, DisasterInfo> dicDisaster = new Dictionary<int, DisasterInfo>();
            // Disaster, Disaster Full Path
            Dictionary<DisasterInfo, string> dicDisasterFullPath = new Dictionary<DisasterInfo, string>();
            ArrayList arrActionStepHistories = new ArrayList();
            ArrayList arrResult = new ArrayList();

            foreach (KeyValuePair<string, DisasterInfo> pair in dicData)
            {
                DisasterInfo disaster = pair.Value;
                dicDisasterFullPath[disaster] = pair.Key;

                foreach (ActionStepInfo actionStep in disaster.ActionSteps)
                {
                    dicDisaster[actionStep.ActionStepID] = disaster;

                    int nKey = isRealMode ? actionStep.ActionStepID : -actionStep.ActionStepID;

                    if (dicActionStepHistories.ContainsKey(nKey))
                    {
                        Data_ActionStepHistory history = dicActionStepHistories[nKey];
                        arrActionStepHistories.Add(history);

                        arrResult.Add(history.ID);
                        arrResult.Add(history.ActionStepID);
                        arrResult.Add(history.BeginTime);
                    }
                }
            }

            if (arrActionStepHistories.Count == 0)
                return true;

            m_arrHistory.Add(arrResult);

            string strActionStepIDs = "";
            ArrayList arrHistoryID = new ArrayList();
            ArrayList arrActionStepID = new ArrayList();
            ArrayList arrBeginTime = new ArrayList();
            ArrayList arrDetectTime = new ArrayList();
            ArrayList arrDisaster = new ArrayList();

            foreach (Data_ActionStepHistory history in arrActionStepHistories)
            {
                if (!dicDisaster.ContainsKey(history.ActionStepID))
                    continue;

                DisasterInfo disaster = dicDisaster[history.ActionStepID];

                if (!dicDisasterFullPath.ContainsKey(disaster))
                    continue;

                //string strDisasterPath = dicDisasterFullPath[disaster];
                //string strActionStepPath = GetActionStepPath(disaster.ActionSteps, nActionStepID);

                //if (strActionStepPath.Length == 0)
                //  continue;

                if (strActionStepIDs.Length == 0)
                    strActionStepIDs = history.ActionStepID.ToString();
                else
                    strActionStepIDs += ", " + history.ActionStepID.ToString();

                arrHistoryID.Add(history.ID);
                arrActionStepID.Add(history.ActionStepID);
                arrBeginTime.Add(history.BeginTime);
                arrDetectTime.Add(history.DetectTime);
                arrDisaster.Add(disaster);

                History.HistoryManager.Instance.AddHistoryDisasterPosition(history.ID, history.ActionStepID, isRealMode);
                History.HistoryManager.Instance.SetActionStepHistory(isRealMode ? history.ActionStepID : -history.ActionStepID, history.ID);

                // 1. ActionStep내의 StepMember List 받아오기
                // 2. Tab에 Panel 추가하기
                // 3. Panel에 Component 추가하기
                // 4. Component별 상태정보 넣기

                // => 새로 만들것   //LoadActionStepComponent(nID, nActionStepID, disaster, dtBegin, isRealMode);
                //AddGridRowScenario(strDisasterPath + "/" + strActionStepPath, nActionStepID, isRealMode);
            }

            //FileWrite(arrHistoryID.ToString());
            if (!LoadActionStepPanel(dbMgr, strActionStepIDs, arrHistoryID, arrActionStepID, arrBeginTime, arrDetectTime, arrDisaster, isRealMode, dicDisaster, dicDisasterFullPath, isRegular, isNormal))
                return false;

            int nActionStepCount = arrActionStepID.Count;

            for (int i = 0; i < nActionStepCount; i++)
            {
                FormMain.Instance.SOPManager.SetActionStepHistoryID((int)arrActionStepID[i], isRealMode, (int)arrHistoryID[i]);
            }

            return true;
        }

        /*private bool LoadHistory(WebDBManager dbMgr, Dictionary<string, DisasterInfo> dicData, bool isRealMode, bool isRegular, bool isNormal)
        {
            // ActionStep ID, Disaster
            Dictionary<int, DisasterInfo> dicDisaster = new Dictionary<int, DisasterInfo>();
            // Disaster, Disaster Full Path
            Dictionary<DisasterInfo, string> dicDisasterFullPath = new Dictionary<DisasterInfo, string>();

            bool isFirst = true;
            string strSQL = "select id, ActionStepID, BeginTime from ActionStepHistory where EndTime is null and CancelTime is null and id in (";

            foreach (KeyValuePair<string, DisasterInfo> pair in dicData)
            {
                DisasterInfo disaster = pair.Value;
                dicDisasterFullPath[disaster] = pair.Key;

                foreach (ActionStepInfo actionStep in disaster.ActionSteps)
                {
                    dicDisaster[actionStep.ActionStepID] = disaster;

                    string strSubSQL = string.Format("(select max(id) from ActionStepHistory where BeginTime = (select max(BeginTime) from ActionStepHistory where ActionStepID = {0} and RealMode = {1}))",
                        actionStep.ActionStepID, isRealMode ? 1 : 0);

                    if (isFirst)
                        isFirst = false;
                    else
                        strSubSQL = ", " + strSubSQL;

                    strSQL += strSubSQL;
                }
            }

            if (isFirst)
                return true;

            strSQL += ")";
            
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            m_arrLoadHistory = arrResult;
            if (arrResult == null) return false;

            int nResultCount = arrResult.Count;
            if(nResultCount > 0)
                m_arrHistory.Add(arrResult);

            BarLevelTree tree = FormMain.Instance.GetPageHome().GetDockScenario().GetBarLevelTree();

            DateTime dtDefault = new DateTime();

            string strActionStepIDs = "";
            ArrayList arrHistoryID = new ArrayList();
            ArrayList arrActionStepID = new ArrayList();
            ArrayList arrBeginTime = new ArrayList();
            ArrayList arrDisaster = new ArrayList();

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nActionStepID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                DateTime dtBegin = WebDBManager.GetDateTimeField(arrResult[i + 2], dtDefault);
            
                if (!dicDisaster.ContainsKey(nActionStepID))
                    continue;

                DisasterInfo disaster = dicDisaster[nActionStepID];

                if (!dicDisasterFullPath.ContainsKey(disaster))
                    continue;

                //string strDisasterPath = dicDisasterFullPath[disaster];
                //string strActionStepPath = GetActionStepPath(disaster.ActionSteps, nActionStepID);

                //if (strActionStepPath.Length == 0)
                //  continue;

                if (strActionStepIDs.Length == 0)
                    strActionStepIDs = nActionStepID.ToString();
                else
                    strActionStepIDs += ", " + nActionStepID.ToString();

                arrHistoryID.Add(nID);
                arrActionStepID.Add(nActionStepID);
                arrBeginTime.Add(dtBegin);
                arrDisaster.Add(disaster);

                History.HistoryManager.Instance.AddHistoryDisasterPosition(nID, nActionStepID, isRealMode);
                History.HistoryManager.Instance.SetActionStepHistory(isRealMode ? nActionStepID : -nActionStepID, nID);

                // 1. ActionStep내의 StepMember List 받아오기
                // 2. Tab에 Panel 추가하기
                // 3. Panel에 Component 추가하기
                // 4. Component별 상태정보 넣기

             // => 새로 만들것   //LoadActionStepComponent(nID, nActionStepID, disaster, dtBegin, isRealMode);
                //AddGridRowScenario(strDisasterPath + "/" + strActionStepPath, nActionStepID, isRealMode);
            }

            //FileWrite(arrHistoryID.ToString());
            if (!LoadActionStepPanel(dbMgr, strActionStepIDs, arrHistoryID, arrActionStepID, arrBeginTime, arrDisaster, isRealMode, dicDisaster, dicDisasterFullPath, isRegular, isNormal))
                return false;

            int nActionStepCount = arrActionStepID.Count;

            for (int i = 0; i < nActionStepCount; i++)
            {
                FormMain.Instance.SOPManager.SetActionStepHistoryID((int)arrActionStepID[i], isRealMode, (int)arrHistoryID[i]);
            }

            return true;
        }*/

        public ArrayList GetRunActionStepHistory()
        {
            ArrayList arrHistory = FormMain.Instance.GetPageHome().GetDockScenario().ArrHistory;
            foreach (ArrayList arr in arrHistory)
            {
                if (arr.Count > 0)
                    return arr;
            }
            return null;
        }

        private ArrayList FindStepMemberList(int nActionStepID, Dictionary<ActionStepInfo, ArrayList> dicStepMember)
        {
            foreach (KeyValuePair<ActionStepInfo, ArrayList> pair in dicStepMember)
            {
                if (pair.Key.ActionStepID == nActionStepID)
                    return pair.Value;
            }

            return null;
        }

        // dicStepMember : ActionStepInfo, StepMemberData List
        private bool LoadActionSteps(WebDBManager dbMgr, string strSQL, ArrayList arrActionStepID, ArrayList arrDisaster, Dictionary<ActionStepInfo, ArrayList> dicStepMember)
        {
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return false;

            int nResultCount = arrResult.Count;

            int nPrevActionStepID = -2;
            int nIndex = -1;

            ArrayList arrStepMember = null;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nTeamID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");
                int nLevelNo = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nTeamType = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nActionStepID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);

                if (nActionStepID != nPrevActionStepID)
                {
                    arrStepMember = FindStepMemberList(nActionStepID, dicStepMember);

                    if (arrStepMember == null)
                    {
                        nIndex = arrActionStepID.IndexOf(nActionStepID);
                        if (nIndex < 0)
                            continue;

                        DisasterInfo disaster = (DisasterInfo)arrDisaster[nIndex];
                        ActionStepInfo actionStep = disaster.FindActionStep(nActionStepID);
                        if (actionStep == null)
                            continue;

                        arrStepMember = new ArrayList();
                        dicStepMember[actionStep] = arrStepMember;
                    }

                    nPrevActionStepID = nActionStepID;
                }

                StepMemberData data = new StepMemberData(strTeamName, nTeamID, nTeamType, nID, nLevelNo);
                arrStepMember.Add(data);
            }

            return true;
        }

        public bool LoadActionStepPanel(WebDBManager dbMgr, string strActionstepIDs, ArrayList arrActionStepHistoryID, ArrayList arrActionStepID, ArrayList arrActionStepBeginTime, ArrayList arrActionStepDetectTime, ArrayList arrDisaster, bool isRealMode, Dictionary<int, DisasterInfo> dicDisaster, Dictionary<DisasterInfo, string> dicDisasterFullPath, bool isRegular, bool isNormal)
        {
            if (strActionstepIDs.Length == 0)
                return true;

            // ActionStepInfo, StepMemberData List
            Dictionary<ActionStepInfo, ArrayList> dicStepMember = new Dictionary<ActionStepInfo, ArrayList>();

            string strSQL = string.Format("select sm.ID, sm.TeamID, tt.TeamName, tt.LevelNo, sm.TeamType, sm.ActionStepID from StepMember as sm, TemporaryNormalTeam as tt where sm.TeamType = 0 and sm.TeamID = tt.ID and sm.ActionStepID in ({0}) order by ActionStepID",
                strActionstepIDs);
            if (!LoadActionSteps(dbMgr, strSQL, arrActionStepID, arrDisaster, dicStepMember))
                return false;

            strSQL = string.Format("select sm.ID, sm.TeamID, tt.TeamName, tt.LevelNo, sm.TeamType, sm.ActionStepID from StepMember as sm, TemporaryEmergencyTeam as tt where sm.TeamType = 1 and sm.TeamID = tt.ID and sm.ActionStepID in ({0}) order by ActionStepID",
                strActionstepIDs);
            if (!LoadActionSteps(dbMgr, strSQL, arrActionStepID, arrDisaster, dicStepMember))
                return false;

            strSQL = string.Format("select sm.ID, sm.TeamID, tt.TeamName, sm.TeamType, sm.ActionStepID from StepMember as sm, ExternalTeam as tt where sm.TeamType = 2 and sm.TeamID = tt.ID and sm.ActionStepID in ({0}) order by ActionStepID",
                strActionstepIDs);
            if (!LoadActionSteps(dbMgr, strSQL, arrActionStepID, arrDisaster, dicStepMember))
                return false;

            strSQL = string.Format("select sm.ID, sm.TeamID, tt.TeamName, sm.TeamType, sm.ActionStepID from StepMember as sm, UserDefinedTeam as tt where sm.TeamType = 3 and sm.TeamID = tt.ID and sm.ActionStepID in ({0}) order by ActionStepID",
                strActionstepIDs);
            if (!LoadActionSteps(dbMgr, strSQL, arrActionStepID, arrDisaster, dicStepMember))
                return false;

            strSQL = string.Format("select sm.ID, sm.TeamID, tt.TeamName, sm.TeamType, sm.ActionStepID from StepMember as sm, RegularTeam as tt where sm.TeamType = 4 and sm.TeamID = tt.ID and sm.ActionStepID in ({0}) order by ActionStepID",
                strActionstepIDs);
            if (!LoadActionSteps(dbMgr, strSQL, arrActionStepID, arrDisaster, dicStepMember))
                return false;

            PageBackstageHome pageHome = FormMain.Instance.GetPageHome();
            IOManager ioMgr = new IOManager();

            // TeamID, Team Name
            Dictionary<int, string> dicNormal = null;
            Dictionary<int, string> dicEmergency = null;
            Dictionary<int, string> dicUserDefined = null;
            Dictionary<int, Sections.ExternalTeamData> dicExternal = IOManager.ReadExternalTeamList(dbMgr);
            Dictionary<int, string> dicRegular = null;

            foreach (KeyValuePair<ActionStepInfo, ArrayList> pair in dicStepMember)
            {
                Sections.SectionData.ClearIDList();

                ActionStepInfo actionStep = pair.Key;
                ArrayList arrStepMember = pair.Value;

                Sections.SectionTabPage tabPage = (Sections.SectionTabPage)pageHome.AddTabPage(actionStep.ToData_ActionStep(), isRealMode);

                if (tabPage != null)
                {
                    int nActionStepIndex = arrActionStepID.IndexOf(actionStep.ActionStepID);

                    if (nActionStepIndex >= 0)
                    {
                        int nActionStepHistoryID = (int)arrActionStepHistoryID[nActionStepIndex];
                        tabPage.ActionStepHistoryID = nActionStepHistoryID;
                    }
                }

                int nIndex = arrActionStepID.IndexOf(actionStep.ActionStepID);
                if (nIndex >= 0)
                {
                    // ActionStep 시작 정보를 Log 창에 표시
                    //History.HistoryManager.Instance.AddActionStepHistory(actionStep.ActionStepID, isRealMode, Sections.WorkFlowState.RUN, actionStep.BeginTime, true);
                    History.HistoryManager.Instance.AddActionStepHistory(actionStep.ActionStepID, isRealMode, Sections.WorkFlowState.RUN, (DateTime)arrActionStepBeginTime[nIndex], true);
                    m_nInitHistoryLogCount++;
                }
               
                if (tabPage == null)
                    continue;

                Sections.WorkFlow work = Sections.WorkFlowManager.Instance.Get(tabPage.ActionStepID, !tabPage.VirtualMode);
                if (work != null)
                {
                    work.State = Sections.WorkFlowState.RUN;
                }

                // 방금 추가된 Tab 이외의 탭들은 제거한다.(화면에서만 사라지고 실제로는 제거되지 않음)
                int nCount = pageHome.TabControls.Controls.Count;
                for (int i = 1; i < nCount; i++)
                {
                    pageHome.RemoveTabPage((TabPage)pageHome.TabControls.Controls[0]);
                    pageHome.GetTabPage().RemoveAt(0);
                    //pageHome.TabControls.Controls.RemoveAt(0);
                }

                pageHome.TabControls.SelectedTab = tabPage;
                Sections.TabPageManager.Instance.AddPage(tabPage, isRealMode);
                if (tabPage.CreateNew == true)
                {
                    ArrayList arrPanels = pageHome.AddPane(arrStepMember, actionStep.ActionStepID, tabPage);

                    if (!ioMgr.LoadNewPanelComponent(dbMgr, arrPanels, arrStepMember, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular))
                        return false;

                    if (!LoadComponentHistory(dbMgr, arrPanels, arrActionStepHistoryID, arrActionStepID, arrActionStepBeginTime, arrActionStepDetectTime, arrDisaster, isRealMode, dicDisaster, dicDisasterFullPath, isRegular, isNormal))
                    {
                        return false;
                    }
                }
                tabPage.CreateNew = false;
                tabPage.VirtualMode = !isRealMode;
            }
            //pageHome.GetComponentContents();
            return true;
        }

        private Sections.Section FindSection(string strComponentID, ArrayList arrSections)
        {
            foreach (Sections.Section section in arrSections)
            {
                if (section.Data.ComponentID == strComponentID)
                    return section;
            }

            return null;
        }

        private void SetSectionProcessButtonStatus(ProcessButtonManager mgr, Sections.State state, int nProcessDirections, Sections.ProcessDirection processDirection)
        {
            Sections.Arrow.ArrowPosition arrowPosition = Sections.Arrow.ArrowPosition.NONE;

            if (processDirection == Sections.ProcessDirection.TOP)
                arrowPosition = Sections.Arrow.ArrowPosition.TOP;
            else if (processDirection == Sections.ProcessDirection.BOTTOM)
                arrowPosition = Sections.Arrow.ArrowPosition.BOTTOM;
            else if (processDirection == Sections.ProcessDirection.LEFT)
                arrowPosition = Sections.Arrow.ArrowPosition.LEFT;
            else if (processDirection == Sections.ProcessDirection.RIGHT)
                arrowPosition = Sections.Arrow.ArrowPosition.RIGHT;

            ProcessButton btn = mgr.FindButton(arrowPosition);
            if (btn == null)
                return;

            ProcessButton.ButtonStatus btnStatus = ProcessButton.ButtonStatus.UNKNOWN;

            if (state == Sections.State.DONE)
                btnStatus = ProcessButton.ButtonStatus.DONE;
            else if (state == Sections.State.INPUT || state == Sections.State.NORMAL)
                btnStatus = ProcessButton.ButtonStatus.WAIT;
            else if (state == Sections.State.RUN)
                btnStatus = ProcessButton.ButtonStatus.WAIT;
            else if (state == Sections.State.SKIP)
                btnStatus = ProcessButton.ButtonStatus.CANCEL;

            if ((nProcessDirections & (int)processDirection) == (int)processDirection)
                btn.Status = btnStatus;
            else
                btn.Status = ProcessButton.ButtonStatus.WAIT;
        }

        private void SetSectionProcessButtons(Sections.Section section, Sections.State state, int nProcessDirections, Sections.WorkFlow workFlow)
        {
            if (section.AdditionalPainter == null)
                return;
            //if (state != Sections.State.DONE || section.AdditionalPainter == null)
            //    return;

            ProcessButtonManager mgr = (ProcessButtonManager)section.AdditionalPainter;

            if (state != Sections.State.DONE)
            {
                //Sections.SectionState sectionState = FormMain.Instance.CurrentWork.FindState(section);
                Sections.SectionState sectionState = workFlow.FindState(section);
                mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.WAIT, null, sectionState);
                return;
            }

            SetSectionProcessButtonStatus(mgr, state, nProcessDirections, Sections.ProcessDirection.TOP);
            SetSectionProcessButtonStatus(mgr, state, nProcessDirections, Sections.ProcessDirection.BOTTOM);
            SetSectionProcessButtonStatus(mgr, state, nProcessDirections, Sections.ProcessDirection.LEFT);
            SetSectionProcessButtonStatus(mgr, state, nProcessDirections, Sections.ProcessDirection.RIGHT);

            // Decision의 경우 특정 버튼이 완료 상태면 나머지 버튼들은 모두 사용안함 상태로 만든다.
            if (section.GetComponentType() == Sections.Section.ComponentType.DECISION)
            {
                ProcessButton btn = GetCompletedProcessButton(mgr);

                if (btn != null)
                    mgr.SetAllButtonsStatus(ProcessButton.ButtonStatus.NOT_USE, btn);
            }
        }

        private ProcessButton GetCompletedProcessButton(ProcessButtonManager mgr)
        {
            ProcessButton btn = mgr.FindButton(Sections.Arrow.ArrowPosition.TOP);
            
            if (btn != null && btn.Status == ProcessButton.ButtonStatus.DONE)
                return btn;

            btn = mgr.FindButton(Sections.Arrow.ArrowPosition.BOTTOM);
            
            if (btn != null && btn.Status == ProcessButton.ButtonStatus.DONE)
                return btn;

            btn = mgr.FindButton(Sections.Arrow.ArrowPosition.RIGHT);

            if (btn != null && btn.Status == ProcessButton.ButtonStatus.DONE)
                return btn;

            btn = mgr.FindButton(Sections.Arrow.ArrowPosition.LEFT);

            if (btn != null && btn.Status == ProcessButton.ButtonStatus.DONE)
                return btn;

            return null;
        }

        private void AddSOPSectionLog(int nActionStepID, Sections.Section section, int nComponentHistoryID, bool isRealMode, int nStatus, int nProcessDirections, ArrayList arrSections, string strTask, DateTime time, string strDescription, bool showBoard, int nCheckedNotify1, int nCheckedNotify2, Sections.WorkFlow workFlow)
        {
            Sections.State state;

            if (nStatus == 1)
                state = Sections.State.NORMAL;
            else if (nStatus == 2)
                state = Sections.State.RUN;
            else if (nStatus == 3)
                state = Sections.State.DONE;
            else if (nStatus == 5)
                state = Sections.State.SKIP;
            else
            {
                // 입력대기는 SOP Log 창에 표시하지 않는다.
                return;
            }

            History.HistoryManager.Instance.SetLastComponentHistory(nActionStepID, nComponentHistoryID);

            SetSectionProcessButtons(section, state, nProcessDirections, workFlow);
            Sections.Section.ComponentType type = section.GetComponentType();

            Sections.SectionState sectionState = Sections.WorkFlowManager.Instance.Find(section, isRealMode);

            if (type == Sections.Section.ComponentType.ENDPOINT || type == Sections.Section.ComponentType.PROCESS ||
                type == Sections.Section.ComponentType.TRANSSOP || type == Sections.Section.ComponentType.LINK)
            {
                History.HistoryManager.Instance.AddSectionHistory(section, nComponentHistoryID, state, nProcessDirections, true, time, showBoard, nCheckedNotify1, nCheckedNotify2);
                m_nInitHistoryLogCount++;
            }
            else if (type == Sections.Section.ComponentType.DECISION)
            {
                Sections.Section nextSection = strDescription.Length == 0 ? null : FindSection(strDescription, arrSections);
                History.HistoryManager.Instance.AddDecisionHistory((Sections.SectionDecision)section, state, nProcessDirections, nextSection, true, time, showBoard);
                m_nInitHistoryLogCount++;
            }
            else if (type == Sections.Section.ComponentType.INTERNAL)
            {
                Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;
                bool usePopupMessage = data.UsePopupMessage;
                bool useSMS = data.UseMobileApp;
                bool useBroadcast = data.UseBroadcast;

                History.HistoryManager.Instance.AddInternalHistory((Sections.SectionInternal)section, state, nProcessDirections, usePopupMessage, useSMS, useBroadcast, true, time, showBoard, nCheckedNotify1);
                m_nInitHistoryLogCount++;
            }
            else if (type == Sections.Section.ComponentType.EXTERNAL)
            {
                Sections.SectionDataExternal data = (Sections.SectionDataExternal)section.Data;
                //bool useSMS = strTask.Contains("메시지");
                //bool useFax = strTask.Contains("Fax");
                bool useSMS = data.UseSMS;
                bool useFax = data.UseFax;
                History.HistoryManager.Instance.AddExternalHistory((Sections.SectionExternal)section, state, nProcessDirections, useSMS, useFax, true, time, showBoard, nCheckedNotify1, nCheckedNotify2);
                m_nInitHistoryLogCount++;
            }
            else if (type == Sections.Section.ComponentType.TRANSMISSION)
            {
                //bool usePopupMessage = strTask.Contains("Popup");
                //bool useSMS = strTask.Contains("메시지");
                //bool useBroadcast = strTask.Contains("방송");
                //bool useExSMS = strTask.Contains("메시지") && strTask.Contains("외부");
                //bool useExFax = strTask.Contains("Fax");

                Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)section.Data;
                bool usePopupMessage = data.DataInternal.UsePopupMessage;
                bool useSMS = data.DataInternal.UseMobileApp;
                bool useBroadcast = data.DataInternal.UseBroadcast;
                bool useExSMS = data.DataExternal.UseSMS;
                bool useExFax = data.DataExternal.UseFax;
                History.HistoryManager.Instance.AddTransmissionHistory((Sections.SectionTransmission)section, state, nProcessDirections, usePopupMessage, useSMS, useBroadcast, useExSMS, useExFax, true, time, showBoard, nCheckedNotify1, nCheckedNotify2);
                m_nInitHistoryLogCount++;
            }
        }

        public void AddSOPSectionLog(int nActionStepID, ArrayList arrComponentHistoryID, ArrayList arrSections, ArrayList arrStatus, ArrayList arrProcessDirections, ArrayList arrTask, ArrayList arrTime, ArrayList arrDescription, ArrayList arrShowBoard, ArrayList arrCheckedNotify1, ArrayList arrCheckedNotify2, bool isRealMode, Sections.WorkFlow workFlow)
        {
            int nSectionCount = arrSections.Count;
            
            for (int i=0;i<nSectionCount;i++)
            {
                Sections.Section section = (Sections.Section)arrSections[i];

                int nComponentHistoryID = (int)arrComponentHistoryID[i];
                int nStatus = (int)arrStatus[i];
                int nProcessDirections = (int)arrProcessDirections[i];
                string strDescription = (string)arrDescription[i];
                string strTask = (string)arrTask[i];
                DateTime time = (DateTime)arrTime[i];
                bool showBoard = (bool)arrShowBoard[i];
                int nCheckedNotify1 = (int)arrCheckedNotify1[i];
                int nCheckedNotify2 = (int)arrCheckedNotify2[i];

                AddSOPSectionLog(nActionStepID, section, nComponentHistoryID, isRealMode, nStatus, nProcessDirections, arrSections, strTask, time, strDescription, showBoard, nCheckedNotify1, nCheckedNotify2, workFlow);
            }
        }

        private string GetActionStepPath(int nActionStepID, ArrayList arrActionSteps)
        {
            foreach (ActionStepInfo actionStep in arrActionSteps)
            {
                if (actionStep.ActionStepID == nActionStepID)
                {
                    if (actionStep.ParentStepID >= 0)
                        return GetActionStepPath(actionStep.ParentStepID, arrActionSteps) + "/" + actionStep.ActionStepName;
                    else
                        return actionStep.ActionStepName;
                }
            }

            return "";
        }

        private string GetActionStepFullPath(int nActionStepID, Dictionary<int, DisasterInfo> dicDisaster, Dictionary<DisasterInfo, string> dicDisasterFullPath)
        {
            if (dicDisaster.ContainsKey(nActionStepID))
            {
                DisasterInfo disaster = dicDisaster[nActionStepID];

                if (dicDisasterFullPath.ContainsKey(disaster))
                {
                    string strDisasterFullPath = dicDisasterFullPath[disaster];
                    return strDisasterFullPath + "/" + GetActionStepPath(nActionStepID, disaster.ActionSteps);
                }
            }

            return "";
        }

        private bool LoadComponentHistory(WebDBManager dbMgr, ArrayList arrPanels, ArrayList arrActionStepHistoryID, ArrayList arrActionStepID, ArrayList arrActionStepBeginTime, ArrayList arrActionStepDetectTime, ArrayList arrDisaster, bool isRealMode, Dictionary<int, DisasterInfo> dicDisaster, Dictionary<DisasterInfo, string> dicDisasterFullPath, bool isRegular, bool isNormal)
        {
            if (arrPanels.Count == 0)
                return true;

            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)arrPanels[0];
            int nIndex = arrActionStepID.IndexOf(panel.ActionStepID);

            if (nIndex < 0)
                return false;

            int nActionStepHistoryID = (int)arrActionStepHistoryID[nIndex];
            DateTime dtBegin = (DateTime)arrActionStepBeginTime[nIndex];
            DisasterInfo disaster = (DisasterInfo)arrDisaster[nIndex];

            string strSQL = string.Format("select ID, ComponentID, ComponentType, Time, Status, Task, CompleteCount, CheckedNotify1, CheckedNotify2, Description, ShowBoard from ComponentHistory where ActionStepHistoryID = {0}",
                nActionStepHistoryID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return false;

            int nResultCount = arrResult.Count;
            DateTime dtDefault = new DateTime();

            // Section, Section Status
            Dictionary<Sections.Section, int> dicSectionStatus = new Dictionary<Sections.Section, int>();
            //ArrayList arrSections = new ArrayList();
            //ArrayList arrSectionStatus = new ArrayList();
            ArrayList arrSections4Log = new ArrayList();
            ArrayList arrSectionStatus4Log = new ArrayList();
            ArrayList arrSectionProcessDirections4Log = new ArrayList();
            ArrayList arrDescription = new ArrayList();
            ArrayList arrTask = new ArrayList();
            ArrayList arrTime = new ArrayList();
            // 상황판에 보여줄 것인가?
            ArrayList arrShowBoard = new ArrayList();
            ArrayList arrComponentHistoryID = new ArrayList();
            ArrayList arrCheckedNotify1 = new ArrayList();
            ArrayList arrCheckedNotify2 = new ArrayList();

            ArrayList arrAllSections = GetAllPanelSections(arrPanels);

            for (int i = 0; i < nResultCount - 10; i += 11)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nComponentID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nComponentType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                DateTime time = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                int nStatus = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                string strTask = WebDBManager.GetStringField(arrResult[i + 5].ToString(), "");
                int nCompleteCount = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                int nCheckedNotify1 = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                int nCheckedNotify2 = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 9].ToString(), "");
                bool showBoard = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 0) == 0 ? false : true;

                Sections.Section section = FindSection(nComponentID, nComponentType, arrAllSections);
                if (section == null)
                    continue;

                section.CompleteCount = nCompleteCount;

                dicSectionStatus[section] = nStatus;

                int nDirections = nStatus >> 16;
                nStatus = nStatus & 0x0000ffff;

                /*int nSectionIndex = arrSections.IndexOf(section);

                // 같은 Section에 대하여 여러 상태 정보가 있을 경우 마지막 정보만 기억시킨다.
                if (nSectionIndex >= 0)
                {
                    arrSections.RemoveAt(nSectionIndex);
                    arrSectionStatus.RemoveAt(nSectionIndex);
                }

                arrSections.Add(section);
                arrSectionStatus.Add(nStatus);*/

                // SOP Log창 기록을 위한 List
                arrComponentHistoryID.Add(nID);
                arrSections4Log.Add(section);
                arrSectionStatus4Log.Add(nStatus);
                arrSectionProcessDirections4Log.Add(nDirections);
                arrDescription.Add(strDescription);
                arrTask.Add(strTask);
                arrTime.Add(time);
                arrShowBoard.Add(showBoard);
                arrCheckedNotify1.Add(nCheckedNotify1);
                arrCheckedNotify2.Add(nCheckedNotify2);
            }


            if (Sections.WorkFlowManager.Instance.Exist(panel.ActionStepID, isRealMode))
                Sections.WorkFlowManager.Instance.Remove(panel.ActionStepID, isRealMode);
            Sections.WorkFlow workFlow = Sections.WorkFlowManager.Instance.Add(panel.ActionStepID, arrAllSections, isRealMode);

            if (workFlow == null)
                return false;

            int nArrIndex = arrActionStepID.IndexOf(panel.ActionStepID);

            if (nArrIndex >= 0)
            {
                DateTime dtDetect = (DateTime)arrActionStepDetectTime[nArrIndex];
                workFlow.DetectTime = dtDetect;
            }

            AddSOPSectionLog(panel.ActionStepID, arrComponentHistoryID, arrSections4Log, arrSectionStatus4Log, arrSectionProcessDirections4Log, arrTask, arrTime, arrDescription, arrShowBoard, arrCheckedNotify1, arrCheckedNotify2, isRealMode, workFlow);

            BarLevelTree tree = FormMain.Instance.GetPageHome().GetDockScenario().GetBarLevelTree();
            TreeNode node = tree.FindActionStepNode(panel.ActionStepID);
            string szPath = node == null ? GetActionStepFullPath(panel.ActionStepID, dicDisaster, dicDisasterFullPath) : node.FullPath;
            bool bHasPos = true;
			if (szPath.IndexOf("자연재해") != -1 || szPath.IndexOf("태풍") != -1)
            {
                bHasPos = false;
            }
            string sopName = szPath.Substring(szPath.IndexOf("\\") + 1);


            workFlow.HasPosition = bHasPos;
            
            if (bHasPos == true)
            {        
                
                HistoryDiasterPosition pos = History.HistoryManager.Instance.FindHistoryDisasterPosition(panel.ActionStepID, isRealMode);
                if (pos != null)
                {
                    workFlow.LastPosition = pos;
                    workFlow.Position = workFlow.LastPosition.PoistionName;
                }                
            }

            workFlow.SOPName = sopName;

            workFlow.State = Sections.WorkFlowState.RUN;

            FormMain.Instance.SetCurrentWorkflow(workFlow);

            int nSectionCount = arrAllSections.Count;

            for (int i = 0; i < nSectionCount; i++)
            {
                Sections.Section section = (Sections.Section)arrAllSections[i];

                // add by skkim : 2013-01-07 링크 노드 상태 세팅 제외
                if( section.GetComponentType() == Sections.Section.ComponentType.LINK)
                    continue;

                int nStatus = dicSectionStatus.ContainsKey(section) ? dicSectionStatus[section] : 1/*대기상태*/;
                int nDirection = nStatus >> 16;

                nStatus = nStatus & 0x0000ffff;

                // changed by skkim : 2013-01-07 링크노드 상태 세팅 제외
                //Sections.SectionState state = workFlow.FindState(section, true);
                Sections.SectionState state = workFlow.FindState(section, false);
                
                
                if (nStatus == 2)
                {
                    //state.InProgress();
                    state.CopyState(Sections.State.RUN, Sections.WorkFlowManager.Instance.InProgressColor);
                    ArrayList arList = workFlow.FindNext(state);
                    foreach ( Sections.SectionState next in arList)
                    {
                        if (next != null)
                        {
                            next.CopyState(Sections.State.INPUT, Sections.WorkFlowManager.Instance.InputWaitColor);
                            //next.InputWait();
                        }
                    }
                }
                else if (nStatus == 3)
                {
                    //state.Complete();
                    state.CopyState(Sections.State.DONE, Sections.WorkFlowManager.Instance.CompleteColor);
                    state.ProcessDirections = nDirection;
                }
                else if (nStatus == 4)
                {
                    state.CopyState(Sections.State.INPUT, Sections.WorkFlowManager.Instance.InputWaitColor);
                    //state.InputWait();
                }
                else if (nStatus == 5)
                {
                    state.CopyState(Sections.State.SKIP, Sections.WorkFlowManager.Instance.SkipColor);
                    //state.Skip();
                }
            }

            string strDisasterPath = dicDisasterFullPath[disaster];
            string strActionStepPath = GetActionStepPath(disaster.ActionSteps, panel.ActionStepID);

            if (strActionStepPath.Length == 0)
                return false;
			strDisasterPath = strDisasterPath.Replace((char)0x06, '/');
            AddGridRowScenario(strDisasterPath + '/'+ strActionStepPath, panel.ActionStepID, isRealMode, isRegular, isNormal, nActionStepHistoryID);

            return true;
        }

        public ArrayList GetAllPanelSections(ArrayList arrPanels)
        {
            ArrayList arrSections = new ArrayList();

            foreach (Sections.PanelSectionEx panel in arrPanels)
            {
                foreach (Sections.Section section in panel.Sections)
                {
                    arrSections.Add(section);
                }
            }

            return arrSections;
        }

        public Sections.Section FindSection(int nComponentID, int nComponentType, ArrayList arrSections)
        {
            foreach (Sections.Section section in arrSections)
            {
                if ((int)section.GetComponentType() == nComponentType)
                {
                    if (section.Data.ID == nComponentID)
                        return section;
                }
            }
        
            return null;
        }

        /*private bool LoadActionStepComponent(int nActionStepHistoryID, int nActionStepID, DisasterInfo disaster, DateTime dtBegin, bool isRealMode)
        {
            ActionStepInfo info = null;

            foreach (ActionStepInfo _info in disaster.ActionSteps)
            {
                if (_info.ActionStepID == nActionStepID)
                {
                    info = _info;
                    break;
                }
            }

            if (info == null)
                return false;

            Data_ActionStep actionStep = new Data_ActionStep();

            actionStep.ID = nActionStepID;
            actionStep.BeginTime = dtBegin;
            actionStep.DisasterID = disaster.DisasterID;
            actionStep.Iteration = info.Iteration;
            actionStep.IterationType = info.IterationType;
            actionStep.ParentStepID = info.ParentStepID;
            actionStep.PeriodType = info.PeriodType;
            actionStep.ProcessTime = info.ProcessTime;
            actionStep.ProcessTimeType = info.ProcessTimeType;
            actionStep.StepName = info.ActionStepName;
            actionStep.WeekdayOption = info.WeekDayOption;

            TabPage tabPage = FormMain.Instance.GetPageHome().AddTabPage(actionStep);

            // 1. ActionStep내의 StepMember List 받아오기
            // 2. Tab에 Panel 추가하기
            // 3. Panel에 Component 추가하기
            // 4. Component별 상태정보 넣기

            return tabPage != null;
        }*/

        /*private void FileWrite(string ID) // 현재까지 읽은 디비 갯수 입력
        {
            string strPath = Application.StartupPath + "\\SOPMonitoringReceiveMessage.txt";
            StreamWriter WriteFile = new StreamWriter(strPath, false, Encoding.Unicode);
            WriteFile.Write(ID);
            WriteFile.Close();
            WriteFile.Dispose();
        }*/

        // 기존에 실행되고 있던 SOP를 불러온다.
        public bool LoadHistory(WebDBManager dbMgr, SOPManager sopMgr)
        {
            Dictionary<string, DisasterInfo> dicRegularNormal = sopMgr.GetSOPDictionary(true, true);
            Dictionary<string, DisasterInfo> dicRegularAbnormal = sopMgr.GetSOPDictionary(true, false);
            Dictionary<string, DisasterInfo> dicNonregularNormal = sopMgr.GetSOPDictionary(false, true);
            Dictionary<string, DisasterInfo> dicNonregularAbnormal = sopMgr.GetSOPDictionary(false, false);

            // Key : 양수이면 실제모드의 ActionStepID
            //       음수이면 훈련모드의 ActionStepID
            Dictionary<int, Data_ActionStepHistory> dicActionStepHistories = new Dictionary<int,Data_ActionStepHistory>();

            if (!LoadActionStepHistory(dbMgr, dicActionStepHistories))
                return false;

            // 기존의 LoadHistory(...)가 너무 많은 SubQuery로 인하여 DB 부하가 많은 관계로 SubQuery를 사용하지 않고 
            // dicActionStepHistories를 사용하는 버전으로 변경
            //if (!LoadHistory(dbMgr, dicRegularNormal, true, true, true))
            if (!LoadHistory(dbMgr, dicRegularNormal, dicActionStepHistories, true, true, true))
            {
                m_isLoadComponentHistory = true;
                return false;
            }
            //if (!LoadHistory(dbMgr, dicRegularNormal, false, true, true))
            if (!LoadHistory(dbMgr, dicRegularNormal, dicActionStepHistories, false, true, true))
            {
                m_isLoadComponentHistory = true;
                return false;
            }

            //if (!LoadHistory(dbMgr, dicRegularAbnormal, true, true, false))
            if (!LoadHistory(dbMgr, dicRegularAbnormal, dicActionStepHistories, true, true, false))
            {
                m_isLoadComponentHistory = true;
                return false;
            }
            
            //if (!LoadHistory(dbMgr, dicRegularAbnormal, false, true, false))
            if (!LoadHistory(dbMgr, dicRegularAbnormal, dicActionStepHistories, false, true, false))
            {
                m_isLoadComponentHistory = true;
                return false;
            }

            //if (!LoadHistory(dbMgr, dicNonregularNormal, true, false, true))
            if (!LoadHistory(dbMgr, dicNonregularNormal, dicActionStepHistories, true, false, true))
            {
                m_isLoadComponentHistory = true;
                return false;
            }
            
            //if (!LoadHistory(dbMgr, dicNonregularNormal, false, false, true))
            if (!LoadHistory(dbMgr, dicNonregularNormal, dicActionStepHistories, false, false, true))
            {
                m_isLoadComponentHistory = true;
                return false;
            }

            //if (!LoadHistory(dbMgr, dicNonregularAbnormal, true, false, false))
            if (!LoadHistory(dbMgr, dicNonregularAbnormal, dicActionStepHistories, true, false, false))
            {
                m_isLoadComponentHistory = true;
                return false;
            }
            
            //if (!LoadHistory(dbMgr, dicNonregularAbnormal, false, false, false))
            if (!LoadHistory(dbMgr, dicNonregularAbnormal, dicActionStepHistories, false, false, false))
            {
                m_isLoadComponentHistory = true;
                return false;
            }

            // 초기 로딩시 DB로부터 읽어들인 History는 HistoryManager의 Thread 및 SOPLog의 Timer를 통하여 최종 전달된다.
            // 따라서, 데이터가 최종적으로 전달된 후에 Log 보기 옵션을 [개별 보기]로 바꾼다.
            FormMain.Instance.GetPageHome().GetDockSOPLog().ReservationComboBoxChange(false);

            // DB로부터 현재 실행중인 SOP를 불러왔으면 그 가운데 하나를 선택한다.
            SelectCurrentSOP();

            m_isLoadComponentHistory = true;
            return true;
        }

        private void SelectCurrentSOP()
        {
            int nRowCount = dataGridScenario.Rows.Count;
            if (nRowCount == 0)
                return;

            if (nRowCount == 1)
                dataGridScenario.Rows[0].Selected = true;
            else
            {
                string strSQL = "Select ActionStepID, RealMode from CurrentActionStep where id = 1";
                ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

                if (arrResult == null || arrResult.Count < 2)
                {
                    dataGridScenario.Rows[0].Selected = true;
                }
                else
                {
                    int nActionStepID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
                    bool isRealMode = WebDBManager.GetIntField(arrResult[1].ToString(), 0) == 0 ? false : true;

                    for (int i = 0; i < nRowCount; i++)
                    {
                        DataGridViewRow row = dataGridScenario.Rows[i];
                        DataGridViewCell cell1 = row.Cells[0];
                        DataGridViewCell cell4 = row.Cells[3];

                        if ((bool)cell1.Tag == isRealMode && (int)cell4.Tag == nActionStepID)
                        {
                            row.Selected = true;
                            return;
                        }
                    }

                    dataGridScenario.Rows[0].Selected = true;
                }
            }
        }

        private void stopMenuItem_Click(object sender, EventArgs e)
        {
            FormMain.Instance.StopWorkflow(DateTime.Now);
        }

        private void AllStopMenuItem_Click(object sender, EventArgs e)
        {
            AllStopMenu();
        }

        private void AllDelMenuItem_Click(object sender, EventArgs e)
        {
            AllDelMenu();
        }

        private void AllStopMenu()
        {
            FormMain.Instance.AllStopWorkflow();
            AllDelMenuItem.Enabled = true;
            m_isAllStop = true;
        }

        private void AllDelMenu()
        {
             if (!m_isAllStop)
                 MessageBox.Show("시나리오를 모두 정지한 후 모두 삭제 하시기 바랍니다.");
             else
             {
                 int nTargetActionStep = -1;
                 string szName = "";
                 foreach (DataGridViewRow row in dataGridScenario.Rows)
                 {
                     nTargetActionStep = (int)row.Cells[3].Tag;
                     szName = row.Cells[3].Value.ToString();

                     if (nTargetActionStep == -1)
                         return;

                     bool bReal = ((szName.IndexOf("훈련모드") == -1) ? true : false);

                     Sections.SectionTabPage page = (Sections.SectionTabPage)Sections.TabPageManager.Instance.GetPage(nTargetActionStep, bReal);
                     
                     if (page != null)
                     {
                         Sections.TabPageManager.Instance.RemovePage(nTargetActionStep, bReal);
                         Sections.WorkFlowManager.Instance.Remove(nTargetActionStep, bReal);
                     }
                 }
                 dataGridScenario.Rows.Clear();
                 //FormMain.Instance.GetPageHome().TabControls.Visible = false;
                 FormMain.Instance.GetPageHome().panel.Visible = false;
                 FormMain.Instance.GetPageHome().SetBackgroundImage(false);
                 FormMain.Instance.WaitWorkflow();
                 FormMain.Instance.GetPageHome().GetDockScenario().GetBarLevelTree().UnSelectedNode();
                 FormMain.Instance.GetPageHome().ClearProcess();
                 m_isAllStop = false;
             }
        }

        public void toolStripMenuDisable(Sections.WorkFlowState state)
        {
            if (state == Sections.WorkFlowState.STANDBY)
            {
                deleteMenuItem.Enabled = true;
                AllDelMenuItem.Enabled = true;
            }
            else if (state == Sections.WorkFlowState.RUN)
            {
                deleteMenuItem.Enabled = false;
                AllDelMenuItem.Enabled = false;
            }
            else if (state == Sections.WorkFlowState.STOP || state == Sections.WorkFlowState.DONE)
            {
                deleteMenuItem.Enabled = true;
                AllDelMenuItem.Enabled = true;
            }

        }

        //public ArrayList GetDisasterName() //재난명 가져오기
        //{
        //    ArrayList arrDisastername = new ArrayList();
            
        //    foreach (DataGridViewRow row in dataGridScenario.Rows)
        //    {
        //        ScenarioInfo scenario = new ScenarioInfo();

        //        int nIndex = row.Cells[3].Value.ToString().LastIndexOf('/');
        //        scenario.Disastername = row.Cells[3].Value.ToString().Substring(0, nIndex);
        //        scenario.ActionID = (int)row.Cells[3].Tag;

        //        arrDisastername.Add(scenario);
        //    }

        //    return arrDisastername;
        //}

        public string GetDisasterName()
        {
            foreach (DataGridViewRow row in dataGridScenario.SelectedRows)
            {
                //string[] strValue = row.Cells[3].Value.ToString().Split('/');
                return row.Cells[3].Value.ToString();
            }

            return null;
        }
    }

    public class ScenarioInfo
    {
        private string m_strDisastername;
        private int m_nActionID;

        public string Disastername
        {
            get { return m_strDisastername; }
            set { m_strDisastername = value; }
        }
        
        public int ActionID
        {
            get { return m_nActionID; }
            set { m_nActionID = value; }
        }
    }
}
