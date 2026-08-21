using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections.Concurrent;

namespace CCTVSingleViewer
{
    using Command;
    using System.Timers;
    using UnE.GUI;

    public partial class FormMain : Form, CCTVAlarmWatcher.IAlarmOwner
    {
        private const int SC_RESTORE = 0xF120;
        private const int SC_RESTORE2 = 0xF122;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_MAXIMIZE2 = 0xF032;
        private const int SC_MINIMIZE = 0xF020;

        private static CCTVPanel m_bigPanel = null;
        private Dictionary<CCTVPanel, CCTV> m_dicCCTVs = new Dictionary<CCTVPanel, CCTV>();

        private int m_nCCTVTop = -1, m_nCCTVBottom;
        private int m_nCCTVLeft, m_nCCTVRight;
        private int m_nCCTVMiddleHorz, m_nCCTVMiddleVert;

        private CCTVPanel m_selectedCCTV = null;
        private string m_strCCTVListPath = "CCTVList.txt";
        private string m_strEquipZoneCCTVListPath = "EquipZoneCCTVList.txt";
        private string m_strHomePath = "home.txt";
        private string m_strEquipZoneCCTVListTempPath = "EquipZoneCCTVList_Temp.txt";

        private static FormMain m_instance = null;
        private CCTVAlarmWatcher.AlarmWatcher m_alarmWatcher = null;
        // Key : EquipZone ID(DB의 값과 다름)
        private Dictionary<int, CCTV[]> m_dicEquipZoneCCTVList = new Dictionary<int, CCTV[]>();
        // Key : 열화상 CCTV ID
        private Dictionary<int, CCTV[]> m_dicAlarmCCTVList = new Dictionary<int, CCTV[]>();

        private ConcurrentDictionary<Alarm, Alarm> m_dicAlarms = new ConcurrentDictionary<Alarm, Alarm>();
        private Alarm m_currentAlarm = null;

        private CCTVPanel[] m_panels = new CCTVPanel[6];
        private CCTV[] m_homeCCTVs = new CCTV[6] { null, null, null, null, null, null };

        private CCTV[] m_prevCCTVs = new CCTV[6] { null, null, null, null, null, null };
        private CCTV[] m_nextCCTVs = new CCTV[6] { null, null, null, null, null, null };

        private SoundPlayerEx m_player = new SoundPlayerEx();
        private CommandManager m_cmdMgr = new CommandManager();

        private string m_strFilePath = String.Format(@"CheckSetting.txt");
        private List<System.Windows.Forms.Timer> m_listTimers = new List<System.Windows.Forms.Timer>();

        private int m_nSelectRowGroup = -1;
        private string m_strSelectRowGroup;
        private CCTV[] m_selectRowGroupCCTVs;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();
            InitCCTVSize();
            SetCCTV(null);

            labelStatus.Text = "";
            ShowStatus(false);

            tabControlHeader.ItemSize = new Size(80, 30);
            Application.AddMessageFilter(new MouseUpMessageFilter());

            m_panels[0] = panelCCTV1;
            m_panels[1] = panelCCTV2;
            m_panels[2] = panelCCTV3;
            m_panels[3] = panelCCTV4;
            m_panels[4] = panelCCTV5;
            m_panels[5] = panelCCTV6;

        
            m_cmdMgr.AddButton(btnUndo, true);
            m_cmdMgr.AddButton(btnRedo, false);
            //m_cmdMgr.AddButton(btnUndoTree, true);
            //m_cmdMgr.AddButton(btnUndoGrid, true);
            //m_cmdMgr.AddButton(btnUndoGroupSet, true);
            //m_cmdMgr.AddButton(btnRedoTree, false);
            //m_cmdMgr.AddButton(btnRedoGrid, false);
            //m_cmdMgr.AddButton(btnRedoGroupSet, false);

            plSetting.Parent = this.splitContainer1.Panel2;
            plAlarmList.Parent = this.splitContainer1.Panel2;
            lbAlarmNum.Text = "0";

            pbAlarm.Location = new Point(btnSetHome.Location.X + btnSetHome.Size.Width + 10, 12);

            LoadSetting();
            lbGroupName.Text = "";
        }

        private void InitCCTVSize()
        {
            m_nCCTVTop = panelCCTV1.Location.Y;
            m_nCCTVLeft = panelCCTV1.Location.X;
            m_nCCTVRight = panelTop.Size.Width - (panelCCTV3.Location.X + panelCCTV3.Size.Width);
            m_nCCTVMiddleHorz = panelCCTV2.Location.X - (panelCCTV1.Location.X + panelCCTV1.Size.Width);
            m_nCCTVMiddleVert = panelCCTV4.Location.Y - (panelCCTV1.Location.Y + panelCCTV1.Size.Height);
            m_nCCTVBottom = panelCCTVBody.Height - (panelCCTV4.Location.Y + panelCCTV4.Size.Height);
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            SetCCTVControls();
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            SetCCTVControls();
        }

        // FormMain의 크기가 변경될때 Split Distance가 바뀌지 않도록 한다.
        private void FixSplitDistance()
        {
            splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
        }

        private void UnFixSplitDistance()
        {
            splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.None;
        }

        private void SetCCTVControls()
        {
            if (m_nCCTVTop < 0)
                return;

            int nAreaWidth, nAreaHeight;

            if (panelTop.Visible)
            {
                nAreaWidth = panelCCTVBody.Size.Width;
                nAreaHeight = panelCCTVBody.Size.Height;
            }
            else
            {
                nAreaWidth = panelCCTVBody.Size.Width;
                nAreaHeight = this.ClientRectangle.Height;
            }

            if (m_bigPanel == null)
            {
                int nCCTVWidth = (nAreaWidth - m_nCCTVLeft - m_nCCTVMiddleHorz * 2 - m_nCCTVRight) / 3;
                int nCCTVHeight = (nAreaHeight - m_nCCTVTop - m_nCCTVMiddleVert - m_nCCTVBottom) / 2;

                panelCCTV1.Location = new Point(panelCCTV1.Location.X, m_nCCTVTop);
                panelCCTV1.Size = new Size(nCCTVWidth, nCCTVHeight);

                panelCCTV2.Location = new Point(m_nCCTVLeft + nCCTVWidth + m_nCCTVMiddleHorz, panelCCTV1.Location.Y);
                panelCCTV2.Size = panelCCTV1.Size;

                panelCCTV3.Location = new Point(panelCCTV2.Location.X + nCCTVWidth + m_nCCTVMiddleHorz, panelCCTV1.Location.Y);
                panelCCTV3.Size = panelCCTV1.Size;

                panelCCTV4.Location = new Point(panelCCTV1.Location.X, panelCCTV1.Location.Y + nCCTVHeight + m_nCCTVMiddleVert);
                panelCCTV4.Size = panelCCTV1.Size;

                panelCCTV5.Location = new Point(panelCCTV2.Location.X, panelCCTV4.Location.Y);
                panelCCTV5.Size = panelCCTV1.Size;

                panelCCTV6.Location = new Point(panelCCTV3.Location.X, panelCCTV4.Location.Y);
                panelCCTV6.Size = panelCCTV1.Size;

                foreach (CCTVPanel panel in m_panels)
                {
                    if (panel != null && panel.Visible == false)
                        panel.Show();
                }
            }
            else
            {
                int nCCTVWidth = nAreaWidth - m_nCCTVLeft - m_nCCTVRight;
                int nCCTVHeight = nAreaHeight - m_nCCTVTop - m_nCCTVBottom;

                m_bigPanel.Location = new Point(panelCCTV1.Location.X, m_nCCTVTop);
                m_bigPanel.Size = new Size(nCCTVWidth, nCCTVHeight);
                
                foreach (CCTVPanel panel in m_panels)
                {
                    if (panel != null && m_bigPanel != panel)
                        panel.Hide();
                }
            }

            //tabControlBody.Size = new Size(splitContainer1.Panel1.Size.Width, splitContainer1.Panel1.ClientSize.Height - tabControlHeader.Size.Height);
            tabControlBody.Size = new Size(splitContainer1.Panel1.Size.Width - 10, splitContainer1.Panel1.ClientSize.Height - 95);
            tabControlHeader.Size = new Size(splitContainer1.Panel1.Size.Width, tabControlHeader.Size.Height);
            tabControlHeader.Location = new Point(tabControlHeader.Location.X, tabControlBody.Size.Height);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Dictionary<int, CCTV> dicCCTVs = ReadCCTVList();
            ReadEquipZoneCCTVList(dicCCTVs);
            ReadHome(dicCCTVs);

            m_alarmWatcher = new CCTVAlarmWatcher.AlarmWatcher(this);
            m_alarmWatcher.Run();

            int nSplitWidth = splitContainer1.Panel1.Size.Width;
            this.WindowState = FormWindowState.Maximized;
            splitContainer1.SplitterDistance = nSplitWidth;
        }

        private void FormMain_FormClosing(object sender, EventArgs e)
        {
            m_alarmWatcher.Stop();
            WriteHome();
        }

        private void ReadHome(Dictionary<int, CCTV> dicCCTVs)
        {
            if (dicCCTVs == null)
                return;

            if (File.Exists(m_strHomePath) == false)
                return;

            int nLineIndex = 0;
            StreamReader reader = new StreamReader(m_strHomePath, Encoding.Default);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                {
                    nLineIndex++;
                    continue;
                }

                int nCCTVID;

                if (int.TryParse(strLine, out nCCTVID) == false)
                {
                    nLineIndex++;
                    continue;
                }

                CCTV cctv;

                if (dicCCTVs.TryGetValue(nCCTVID, out cctv))
                {
                    m_homeCCTVs[nLineIndex] = cctv;
                }

                nLineIndex++;

                if (nLineIndex >= 6)
                    break;
            }

            reader.Close();
            SetCCTVArray(m_homeCCTVs, true);
        }

        private void WriteHome()
        {
            StreamWriter writer = new StreamWriter(m_strHomePath, false, Encoding.Default);

            foreach (CCTV cctv in m_homeCCTVs)
            {
                if (cctv != null)
                    writer.WriteLine(cctv.ID);
                else
                    writer.WriteLine();
            }
            /*foreach (CCTVPanel panel in m_panels)
            {
                if (panel.CCTV != null)
                    writer.WriteLine(panel.CCTV.ID);
                else
                    writer.WriteLine();
            }*/
            
            writer.Close();
        }

        private void ReadEquipZoneCCTVList(Dictionary<int, CCTV> dicCCTVs)
        {
            if (dicCCTVs == null)
                return;

            if (File.Exists(m_strEquipZoneCCTVListPath) == false)
                return;

            gridGroupSet.Rows.Clear();

            bool isAlarmCCTV = false;
            StreamReader reader = new StreamReader(m_strEquipZoneCCTVListPath, Encoding.Default);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] tokens = strLine.Split('\t');

                int nEquipZoneID;

                if (int.TryParse(tokens[0].Trim(), out nEquipZoneID) == false)
                {
                    if (tokens[0].Trim() == "열화상")
                        isAlarmCCTV = true;

                    continue;
                }

                int nRowIndex = gridGroupSet.Rows.Add();
                DataGridViewRow row = gridGroupSet.Rows[nRowIndex];

                CCTV[] arrList = GetCCTVList(tokens, dicCCTVs);

                row.Cells[0].Value = nRowIndex + 1;
                row.Cells[0].Tag = nEquipZoneID;
                row.Cells[1].Value = tokens[1].Trim();
                row.Tag = arrList;

                m_dicEquipZoneCCTVList[nEquipZoneID] = arrList;

                if (isAlarmCCTV)
                {
                    foreach (CCTV cctv in arrList)
                    {
                        if (cctv != null)
                        {
                            m_dicAlarmCCTVList[cctv.ID] = arrList;
                            break;
                        }
                    }
                }
            }

            reader.Close();
        }

        private CCTV[] GetCCTVList(string[] tokens, Dictionary<int, CCTV> dicCCTVs)
        {
            CCTV cctv;
            CCTV[] arrList = new CCTV[6] { null, null, null, null, null, null };

            for (int i = 2; i < tokens.Count(); i++)
            {
                int nCCTVID = -1;

                if (int.TryParse(tokens[i].Trim(), out nCCTVID))
                {
                    if (dicCCTVs.TryGetValue(nCCTVID, out cctv))
                        arrList[i - 2] = cctv;
                }
            }

            return arrList;
        }

        private void SetCCTVAccount(Dictionary<string, List<string>> dicCCTVAccounts, string strType, string[] tokens, out string strUserID, out string strPW)
        {
            strUserID = strPW = "";
            List<string> accounts;

            if (tokens.Count() >= 3)
            {
                strUserID = tokens[1].Trim();
                strPW = tokens[2].Trim();

                if (dicCCTVAccounts.TryGetValue(strType, out accounts) == false)
                {
                    accounts = new List<string>();
                    dicCCTVAccounts[strType] = accounts;
                }
                else
                    accounts.Clear();

                accounts.Add(strUserID);
                accounts.Add(strPW);
            }
            else
            {
                if (dicCCTVAccounts.TryGetValue(strType, out accounts))
                {
                    if (accounts.Count == 2)
                    {
                        strUserID = accounts[0];
                        strPW = accounts[1];
                    }
                }
            }
        }

        private Dictionary<int, CCTV> ReadCCTVList()
        {
            if (File.Exists(m_strCCTVListPath) == false)
                return null;

            Dictionary<int, CCTV> dicCCTVs = new Dictionary<int, CCTV>();
            // CCTV 계정정보
            // Key : CCTV Type
            // Value : User ID, Password
            Dictionary<string, List<string>> dicCCTVAccounts = new Dictionary<string, List<string>>();

            StreamReader reader = new StreamReader(m_strCCTVListPath, Encoding.Default);
            CCTV.CCTVType cctvType = CCTV.CCTVType.RTSP;

            string strUserID = "", strPW = "";

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] tokens = strLine.Split('\t');

                int nID;

                if (int.TryParse(tokens[0].Trim(), out nID) == false)
                {
                    string strType = tokens[0].Trim();

                    if (string.Compare(strType, "Divisys", true) == 0)
                        cctvType = CCTV.CCTVType.Divisys;
                    else if (string.Compare(strType, "RTSP", true) == 0)
                        cctvType = CCTV.CCTVType.RTSP;
                    else if (string.Compare(strType, "WESP", true) == 0)
                        cctvType = CCTV.CCTVType.WESP;

                    SetCCTVAccount(dicCCTVAccounts, strType, tokens, out strUserID, out strPW);
                    continue;
                }

                CCTV cctv = new CCTV();

                cctv.ID = nID;
                cctv.CameraName = tokens[1].Trim();
                cctv.BuildingName = tokens[2].Trim();
                cctv.FloorName = tokens[3].Trim();
                cctv.ZoneName = tokens[4].Trim();
                cctv.ChannelNormalURL = tokens[5].Trim();
                cctv.Type = cctvType;
                cctv.UserID = strUserID;
                cctv.Password = strPW;

                if (tokens.Length >= 7)
                    cctv.ChannelBigURL = tokens[6].Trim();

                if (tokens.Length >= 8)
                    cctv.ChannelSmallURL = tokens[7].Trim();

                int nRowIndex = gridCCTV.Rows.Add();
                DataGridViewRow row = gridCCTV.Rows[nRowIndex];

                row.Cells[0].Value = cctv.ID;
                row.Cells[1].Value = cctv.CameraName;

                row.Tag = cctv;

                // GroupSet에 번호별 그리드 
                nRowIndex = gridGroupCCTV.Rows.Add();
                row = gridGroupCCTV.Rows[nRowIndex];

                row.Cells[0].Value = cctv.ID;
                row.Cells[1].Value = cctv.CameraName;

                row.Tag = cctv;

                AddTreeItem(cctv);
                // GroupSet에 영역별 트리 
                AddTreeGroupItem(cctv);
                dicCCTVs[nID] = cctv;
            }

            reader.Close();
            treeCCTV.ExpandAll();
            treeGroupCCTV.ExpandAll();
            return dicCCTVs;
        }

        private void AddTreeItem(CCTV cctv)
        {
            foreach (TreeNode node in treeCCTV.Nodes)
            {
                if (node.Tag == null && node.Text == cctv.BuildingName)
                {
                    AddTreeItem(node, cctv);
                    return;
                }
            }

            TreeNode buildingNode = treeCCTV.Nodes.Add(cctv.BuildingName);
            AddTreeItem(buildingNode, cctv);
        }

        private void AddTreeGroupItem(CCTV cctv)
        {
            foreach (TreeNode node in treeGroupCCTV.Nodes)
            {
                if (node.Tag == null && node.Text == cctv.BuildingName)
                {
                    AddTreeItem(node, cctv);
                    return;
                }
            }

            TreeNode buildingNode = treeGroupCCTV.Nodes.Add(cctv.BuildingName);
            AddTreeItem(buildingNode, cctv);
        }

        private void AddTreeItem(TreeNode node, CCTV cctv)
        {
            if (node.Level == 0)
            {
                if (cctv.FloorName.Length > 0)
                {
                    foreach (TreeNode child in node.Nodes)
                    {
                        if (child.Tag == null && child.Text == cctv.FloorName)
                        {
                            AddTreeItem(child, cctv);
                            return;
                        }
                    }

                    TreeNode floorNode = node.Nodes.Add(cctv.FloorName);
                    AddTreeItem(floorNode, cctv);
                }
                else
                {
                    TreeNode cctvNode = node.Nodes.Add(cctv.CameraName);
                    cctvNode.Tag = cctv;
                }
            }
            else if (node.Level == 1)
            {
                if (cctv.ZoneName.Length > 0)
                {
                    foreach (TreeNode child in node.Nodes)
                    {
                        if (child.Tag == null && child.Text == cctv.ZoneName)
                        {
                            AddTreeItem(child, cctv);
                            return;
                        }
                    }

                    TreeNode zoneNode = node.Nodes.Add(cctv.ZoneName);
                    AddTreeItem(zoneNode, cctv);
                }
                else
                {
                    TreeNode cctvNode = node.Nodes.Add(cctv.CameraName);
                    cctvNode.Tag = cctv;
                }
            }
            else if (node.Level == 2)
            {
                TreeNode cctvNode = node.Nodes.Add(cctv.CameraName);
                cctvNode.Tag = cctv;
            }
        }

        private void tabControlHeader_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlHeader.SelectedTab == tabPageIndexTree)
                tabControlBody.SelectedTab = tabPageTree;
            else if (tabControlHeader.SelectedTab == tabPageIndexGrid)
                tabControlBody.SelectedTab = tabPageGrid;
            else
                tabControlBody.SelectedTab = tabPageGroupSetBody;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (sender == btnConnectGrid)
            {
                if (gridCCTV.SelectedCells.Count == 0)
                    return;

                CCTV cctv = (CCTV)gridCCTV.SelectedCells[0].OwningRow.Tag;
                SetCCTV(cctv);
            }
            else if (sender == btnConnectTree)
            {
                if (treeCCTV.SelectedNode == null || treeCCTV.SelectedNode.Tag == null)
                    return;

                CCTV cctv = (CCTV)treeCCTV.SelectedNode.Tag;
                SetCCTV(cctv);
            }
            else if (sender == btnConnectGroupSet)
            {
                if (gridGroupSet.SelectedCells.Count == 0)
                    return;

                CCTV[] cctv = (CCTV[])gridGroupSet.SelectedCells[0].OwningRow.Tag;
                SetCCTVArray(cctv);
            }
        }

        private void SetCCTVArray(CCTV[] cctvs, bool systemInput = false)
        {
            CCTV[] prevCCTVs = null;

            if (systemInput == false)
            {
                prevCCTVs = new CCTV[6];

                for (int i = 0; i < 6; i++)
                {
                    prevCCTVs[i] = m_panels[i].CCTV;
                }
            }

            if (cctvs == null)
            {
                foreach (CCTVPanel panel in m_panels)
                {
                    panel.Connect(null);
                }
            }
            else
            {
                for (int i=0;i<6;i++)
                {
                    m_panels[i].Connect(cctvs[i]);
                }
            }

            if (systemInput == false)
            {
                CCTV[] nextCCTVs = new CCTV[6];

                for (int i = 0; i < 6; i++)
                {
                    nextCCTVs[i] = m_panels[i].CCTV;
                }

                OnChangeCCTV(prevCCTVs, nextCCTVs);
            }
        }

        private void SetGroupInfo(CCTV[] cctvs)
        {
            gridGroupInfo.Rows.Clear();

            for (int i = 0; i < cctvs.Length; i++)
            {
                int nRowIndex = gridGroupInfo.Rows.Add();
                DataGridViewRow row = gridGroupInfo.Rows[nRowIndex];

                row.Cells[0].Value = i + 1;
                
                if (cctvs[i] != null)
                    row.Cells[1].Value = cctvs[i].CameraName;
                else
                    row.Cells[1].Value = "";
            }
        }

        public void SetGroupInfo()
        {
            gridGroupInfo.Rows.Clear();
            int i = 1;

            foreach (CCTVPanel panel in m_panels)
            {
                int nRowIndex = gridGroupInfo.Rows.Add();
                DataGridViewRow row = gridGroupInfo.Rows[nRowIndex];

                row.Cells[0].Value = i;
                i++;

                if (panel.CCTV != null)
                    row.Cells[1].Value = panel.CCTV.CameraName;
                else
                    row.Cells[1].Value = "";
            }
        }

        public void EnableSetHome()
        {
            btnSetHome.Enabled = true;
        }

        public void DisableSetHome()
        {
            btnSetHome.Enabled = false;
        }

        public void EnableSetSaveGroup()
        {
            btnSaveGroupSet.Enabled = true;
        }

        public void DisableSetSaveGroup()
        {
            btnSaveGroupSet.Enabled = false;
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            SetCCTV(null);
        }

        private void btnSetHome_Click(object sender, EventArgs e)
        {
            int nIndex = 0;

            DialogResult result = MessageBox.Show("최초 6개 화면으로 설정하시겠습니까?", "Home 설정", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                foreach (CCTVPanel panel in m_panels)
                {
                    m_homeCCTVs[nIndex++] = panel.CCTV;
                }
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            m_nSelectRowGroup = -1;
            lbGroupName.Text = "";

            if (tabPageGroupSetBody_.IsChecked == true)
            {
                plGroupSet.Visible = true;
                gridGroupSet.Visible = true;
                gridGroupSet.ClearSelection();
                plGroup.Visible = false;
                gridGroupInfo.Visible = false;
                plModifityHeader.Visible = false;
                treeGroupCCTV.Visible = false;
                gridGroupCCTV.Visible = false;
            }

            SetCCTVArray(m_homeCCTVs);
            DisableSetHome();
            DisableSetSaveGroup();
        }

        private void treeCCTV_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null || e.Node.Tag == null)
                return;

            if (e.Button == MouseButtons.Left)
            {
                CCTV cctv = (CCTV)e.Node.Tag;
                SetCCTV(cctv);
            }
        }

        private void gridCCTV_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.RowIndex < 0)
                    return;

                DataGridViewRow row = gridCCTV.Rows[e.RowIndex];
                CCTV cctv = (CCTV)row.Tag;
                SetCCTV(cctv);
            }
        }

        private void gridGroupCCTV_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.RowIndex < 0)
                    return;

                DataGridViewRow row = gridGroupCCTV.Rows[e.RowIndex];
                CCTV cctv = (CCTV)row.Tag;
                SetCCTV(cctv);
            }
        }


        private void gridGroupSet_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.RowIndex < 0)
                    return;

                SetCCTVArray(m_selectRowGroupCCTVs);
                SetGroupInfo(m_selectRowGroupCCTVs);
                lbGroupName.Text = m_strSelectRowGroup;

                EnableSetHome();
            }
        }


        private void FormMain_ResizeBegin(object sender, EventArgs e)
        {
            FixSplitDistance();
        }

        private void FormMain_ResizeEnd(object sender, EventArgs e)
        {
            UnFixSplitDistance();
        }

        public void SetCCTV(CCTV cctv)
        {
            if (m_selectedCCTV != null)
            {
                CCTV cctvPrev = m_selectedCCTV.CCTV;
                m_selectedCCTV.Connect(cctv);
                OnChangeCCTV(m_selectedCCTV, cctvPrev, cctv);
            }
        }

        public void SelectCCTV(CCTVPanel cctv, bool isSelected)
        {
            if (m_selectedCCTV != cctv)
            {
                if (m_selectedCCTV != null)
                    m_selectedCCTV.IsSelected = false;

                m_selectedCCTV = cctv;
            }
        }

        public void ShowStatus(bool visible)
        {
            // 항상 표시를 인하여 수정 - 20200116 KDR
            //panelTop.Visible = visible;
            SetCCTVControls();

            if (visible)
                PlaySound();
            else
                StopSound();
        }

        private void PlaySound()
        {
            string szWavPath = "FireSignalAlarm.WAV";
            if (System.IO.File.Exists(szWavPath))
            {
                m_player.SoundLocation = szWavPath;
                m_player.Play();
            }
        }

        private void StopSound()
        {
            m_player.Stop();
        }

        protected override void WndProc(ref Message m)
        {
            // WM_SYSCOMMAND
            if (m.Msg == 0x0112)
            {
                int wParam = (int)m.WParam;

                if (wParam == SC_RESTORE || wParam == SC_RESTORE2 ||
                    wParam == SC_MAXIMIZE || wParam == SC_MINIMIZE ||
                    wParam == SC_MAXIMIZE2)
                {
                    FixSplitDistance();
                }
            }
            
            base.WndProc(ref m);
        }

        private Alarm FindAlarm(Alarm alarm)
        {
            List<Alarm> alarms = m_dicAlarms.Values.ToList();

            foreach (Alarm _alarm in alarms)
            {
                if (alarm.IsSame(_alarm))
                    return _alarm;
            }

            return null;
        }

        private void AddAlarm(Alarm alarm)
        {
            m_dicAlarms[alarm] = alarm;
        
            if (cbAutoCancle.Checked == true)
            {
                System.Windows.Forms.Timer tt = new System.Windows.Forms.Timer();
                tt.Interval = Convert.ToInt32(tbAutoCancleMinute.Text) * 60 * 1000;
                
                tt.Tick += (s, e) =>
                {
                    ClearAlarm(alarm.CCTV.ID);
                    tt.Stop();
                    m_listTimers.Remove(tt);
                };

                m_listTimers.Add(tt);

                tt.Start();
            }
        

            labelStatus.Text = alarm.AlarmText;
            m_currentAlarm = alarm;
            
            pbAlarm.Visible = true;
            pbAlarmOval.Visible = true;
            
            lbAlarmNum.Text = m_dicAlarms.Count().ToString();
            lbAlarmNum.BringToFront();
            lbAlarmNum.Visible = true;

            labelStatus.Location = new Point(pbAlarm.Location.X + pbAlarm.Size.Width + 10, 19);
            btnClearAlarm.Location = new Point(labelStatus.Location.X + labelStatus.Size.Width, 20);
            btnClearAlarm.Visible = true;
            
            ShowStatus(true);
            SetAlarmCCTV(alarm);
        }

        private void SetAlarmCCTV(Alarm alarm)
        {
            if (alarm.FromCCTV)
            {
                CCTV[] arrCCTVs;

                if (m_dicAlarmCCTVList.TryGetValue(alarm.CCTV.ID, out arrCCTVs))
                    SetCCTVArray(arrCCTVs);
                /*foreach (KeyValuePair<int, CCTV[]> pair in m_dicEquipZoneCCTVList)
                {
                    if (pair.Value.Contains<CCTV>(alarm.CCTV))
                    {
                        SetCCTVArray(pair.Value);
                        break;
                    }
                }*/
            }
            else
            {
                CCTV[] cctvs;

                if (m_dicEquipZoneCCTVList.TryGetValue(alarm.EquipZoneID, out cctvs))
                    SetCCTVArray(cctvs);
            }
        }

        private void RemoveAlarm(Alarm alarm, DateTime timeStamp)
        {
            Alarm removed;

            if (m_dicAlarms.TryRemove(alarm, out removed))
            {
                List<Alarm> alarms = m_dicAlarms.Values.ToList();

                if (m_currentAlarm == alarm)
                {
                    if (alarms.Count == 0)
                    {
                        m_currentAlarm = null;
                        ShowStatus(false);
                        SetCCTVArray(null);

                        if (plAlarmList.Visible == true)
                            plAlarmList.Visible = false;
                    }
                    else
                    {
                        alarm = alarms[alarms.Count - 1];
                        m_currentAlarm = alarm;
                        labelStatus.Text = alarm.AlarmText;

                        pbAlarm.Visible = true;
                        pbAlarmOval.Visible = true;

                        lbAlarmNum.Text = m_dicAlarms.Count().ToString();
                        lbAlarmNum.BringToFront();
                        lbAlarmNum.Visible = true;

                        labelStatus.Location = new Point(pbAlarm.Location.X + pbAlarm.Size.Width + 10, 19);
                        btnClearAlarm.Location = new Point(labelStatus.Location.X + labelStatus.Size.Width, 20);
                        btnClearAlarm.Visible = true;

                        AlarmList_Load();

                        ShowStatus(true);
                        SetAlarmCCTV(alarm);
                    }
                }
            }
        }

        public void OnAlarmOn(CCTVAlarmWatcher.AlarmType alarmType, int nCCTVID, DateTime timeStamp)
        {
            CCTV cctv = null;

            foreach (DataGridViewRow row in gridCCTV.Rows)
            {
                CCTV _cctv = (CCTV)row.Tag;

                if (_cctv != null && _cctv.ID == nCCTVID)
                {
                    cctv = _cctv;
                    break;
                }
            }

            if (cctv == null)
                return;

            Alarm alarm = new Alarm();
            alarm.AlarmType = alarmType;
            alarm.CCTV = cctv;
            alarm.FromCCTV = true;
            alarm.TimeStamp = timeStamp;

            if (FindAlarm(alarm) == null)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    AddAlarm(alarm);
                });
            }
        }

        public void OnAlarmOff(int nCCTVID, DateTime timeStamp)
        {
            CCTV cctv = null;

            foreach (DataGridViewRow row in gridCCTV.Rows)
            {
                CCTV _cctv = (CCTV)row.Tag;

                if (_cctv != null && _cctv.ID == nCCTVID)
                {
                    cctv = _cctv;
                    break;
                }
            }

            if (cctv == null)
                return;

            Alarm alarm = null;
            List<Alarm> alarms = m_dicAlarms.Values.ToList();

            foreach (Alarm _alarm in alarms)
            {
                if (_alarm.CCTV == cctv)
                {
                    alarm = _alarm;
                    break;
                }
            }

            if (alarm == null)
                return;

            this.Invoke((MethodInvoker)delegate
            {
                RemoveAlarm(alarm, timeStamp);
            });
        }

        public void OnAlarmOn2(CCTVAlarmWatcher.AlarmType alarmType, int nEquipZoneID, DateTime timeStamp)
        {
            foreach (DataGridViewRow row in gridGroupSet.Rows)
            {
                int nID = (int)row.Cells[0].Tag;

                if (nID == nEquipZoneID)
                {
                    string strEquipZoneName = row.Cells[1].Value.ToString();

                    Alarm alarm = new Alarm();
                    alarm.AlarmType = alarmType;
                    alarm.EquipZoneID = nEquipZoneID;
                    alarm.EquipZoneName = strEquipZoneName;
                    alarm.FromCCTV = false;
                    alarm.TimeStamp = timeStamp;

                    if (FindAlarm(alarm) == null)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            AddAlarm(alarm);
                        });
                    }

                    return;
                }
            }
        }

        public void OnAlarmOff2(int nEquipZoneID, DateTime timeStamp)
        {
            Alarm alarm = null;
            List<Alarm> alarms = m_dicAlarms.Values.ToList();

            foreach (Alarm _alarm in alarms)
            {
                if (_alarm.FromCCTV == false && _alarm.EquipZoneID == nEquipZoneID)
                {
                    alarm = _alarm;
                    break;
                }
            }

            if (alarm == null)
                return;

            this.Invoke((MethodInvoker)delegate
            {
                RemoveAlarm(alarm, timeStamp);
            });
        }

        public bool OnRButtonUp(Message m)
        {
            int x = (short)(ushort)m.LParam;
            int y = (short)(ushort)((uint)m.LParam >> 16);
            //int x = m.LParam.ToInt32() & 0x0000ffff;
            //int y = (m.LParam.ToInt32() >> 16) & 0x0000ffff;

            Control control = Control.FromHandle(m.HWnd);
            CCTVPanel panel = null;

            while (panel == null && control != null)
            {
                foreach (CCTVPanel _panel in m_panels)
                {
                    if (control == _panel)
                    {
                        panel = _panel;
                        break;
                    }
                }

                control = control.Parent;
            }

            if (panel != null)
            {
                if (x >= panel.Location.X && x <= panel.Location.X + panel.Size.Width &&
                    y >= panel.Location.Y && y <= panel.Location.Y + panel.Size.Height)
                {
                    panel.OnRMouseUp(x, y);
                    return true;
                }
                else
                {
                    x += panel.Location.X;
                    y += panel.Location.Y;
                    panel = GetPanel(ref x, ref y);

                    if (panel == null)
                        System.Diagnostics.Trace.WriteLine("Out of Area");
                    else
                    {
                        panel.OnRMouseUp(x, y);
                    }

                    return true;
                }
            }

            return false;
        }

        private CCTVPanel GetPanel(ref int x, ref int y)
        {
            foreach (CCTVPanel panel in m_panels)
            {
                if (HitTest(panel, ref x, ref y))
                {
                    return panel;
                }
            }

            return null;
        }

        private bool HitTest(CCTVPanel panel, ref int x, ref int y)
        {
            if (x >= panel.Location.X && x <= panel.Location.X + panel.Size.Width &&
                y >= panel.Location.Y && y <= panel.Location.Y + panel.Size.Height)
            {
                x = x - panel.Location.X;
                y = y - panel.Location.Y;
                return true;
            }

            return false;
        }

        public void OnChangeCCTV(CCTVPanel panel, CCTV cctvPrev, CCTV cctvNext)
        {
            for (int i=0;i<6;i++)
            {
                if (panel == m_panels[i])
                {
                    m_prevCCTVs[i] = cctvPrev;
                    m_nextCCTVs[i] = cctvNext;
                }
                else
                {
                    m_prevCCTVs[i] = m_panels[i].CCTV;
                    m_nextCCTVs[i] = m_panels[i].CCTV;
                }
            }

            m_cmdMgr.AddCommand(new CommandOne(panel, cctvPrev, cctvNext));
        }

        public void OnChangeCCTV(CCTV[] prevCCTVs, CCTV[] nextCCTVs)
        {
            m_prevCCTVs = prevCCTVs;
            m_nextCCTVs = nextCCTVs;

            m_cmdMgr.AddCommand(new CommandAll(m_panels, m_prevCCTVs, m_nextCCTVs));
        }

        public bool OnLButtonDoubleClick(CCTVPanel panel, out bool isBig)
        {
            isBig = false;

            if (panel == null)
                return false;

            if (m_bigPanel == panel)
            {
                MakeSmallPanel(panel);
                isBig = false;
            }
            else
            {
                MakeBigPanel(panel);
                isBig = true;
            }

            return true;
        }

        private void treeGroupCCTV_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                TreeNode node = (TreeNode)e.Item;

                if (node.Tag != null && node.Tag is CCTV)
                {
                    CCTV cctv = (CCTV)node.Tag;
                    treeGroupCCTV.DoDragDrop(cctv, DragDropEffects.All);
                }
            }
        }

        private void btnClearAlarm_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("알람을 해지하고 처음화면으로 돌아가시겠습니까?", "알람해지", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                labelStatus.Text = "";
                pbAlarm.Visible = false;
                btnClearAlarm.Visible = false;
                pbAlarmOval.Visible = false;
                lbAlarmNum.Visible = false;

                if (m_currentAlarm != null)
                {
                    OnAlarmOff(m_currentAlarm.CCTV.ID, DateTime.Now);
                    btnHome_Click(null, null);
                }
            }
        }

        private void gridCCTV_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CCTV)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void gridCCTV_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.RowIndex < 0)
                    return;

                DataGridViewRow row = gridCCTV.Rows[e.RowIndex];

                if (row.Tag == null)
                    return;

                CCTV cctv = (CCTV)row.Tag;
                gridCCTV.DoDragDrop(cctv, DragDropEffects.All);
            }
        }

        private void gridGroupCCTV_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.RowIndex < 0)
                    return;

                DataGridViewRow row = gridGroupCCTV.Rows[e.RowIndex];

                if (row.Tag == null)
                    return;

                CCTV cctv = (CCTV)row.Tag;
                gridGroupCCTV.DoDragDrop(cctv, DragDropEffects.All);
            }
        }

        private void panelCCTV_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        public void panelCCTV_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (sender is CCTVPanel)
                    DoDragNDrop((CCTVPanel)sender);
            }
        }

        private void DoDragNDrop(CCTVPanel panel)
        {
            if (panel.CCTV != null)
            {
                panel.DoDragDrop(panel, DragDropEffects.All);
            }
        }

        private void left_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CCTVPanel)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void left_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CCTVPanel)))
            {
                CCTVPanel panel = (CCTVPanel)e.Data.GetData(typeof(CCTVPanel));
                panel.ClearCCTV();

                SetGroupInfo();
                EnableSetHome();
                EnableSetSaveGroup();
            }
        }

        private void tabPageIndexTree__Click(object sender, EventArgs e)
        {
            if (tabPageIndexTree_.IsChecked == false)
            {
                tabPageIndexTree_.IsChecked = true;
                tabPageIndexGrid_.IsChecked = false;
                tabPageGroupSetBody_.IsChecked = false;
                tabControlBody.SelectedTab = tabPageTree;

                btnSetHome.Visible = true;
                btnModifityGroupSet.Visible = false;
                btnSaveGroupSet.Visible = false;

                lbGroupName.Visible = false;
                pbAlarm.Location = new Point(btnSetHome.Location.X + btnSetHome.Size.Width + 10, 12);
                labelStatus.Location = new Point(pbAlarm.Location.X + pbAlarm.Size.Width + 10, 19);
                btnClearAlarm.Location = new Point(labelStatus.Location.X + labelStatus.Size.Width, 20);

                tabPageIndexTree_.Refresh();
                tabPageIndexGrid_.Refresh();
                tabPageGroupSetBody_.Refresh();
            }
        }

        private void tabPageIndexGrid__Click(object sender, EventArgs e)
        {
            if (tabPageIndexGrid_.IsChecked == false)
            {
                tabPageIndexTree_.IsChecked = false;
                tabPageIndexGrid_.IsChecked = true;
                tabPageGroupSetBody_.IsChecked = false;
                tabControlBody.SelectedTab = tabPageGrid;

                btnSetHome.Visible = true;
                btnModifityGroupSet.Visible = false;
                btnSaveGroupSet.Visible = false;

                lbGroupName.Visible = false;
                pbAlarm.Location = new Point(btnSetHome.Location.X + btnSetHome.Size.Width + 10, 12);
                labelStatus.Location = new Point(pbAlarm.Location.X + pbAlarm.Size.Width + 10, 19);
                btnClearAlarm.Location = new Point(labelStatus.Location.X + labelStatus.Size.Width, 20);

                tabPageIndexTree_.Refresh();
                tabPageIndexGrid_.Refresh();
                tabPageGroupSetBody_.Refresh();
            }
        }

        private void tabPageGroupSetBody__Click(object sender, EventArgs e)
        {
            if (tabPageGroupSetBody_.IsChecked == false)
            {
                tabPageIndexTree_.IsChecked = false;
                tabPageIndexGrid_.IsChecked = false;
                tabPageGroupSetBody_.IsChecked = true;
                tabControlBody.SelectedTab = tabPageGroupSetBody;

                btnSetHome.Visible = false;
                btnModifityGroupSet.Visible = true;
                btnSaveGroupSet.Visible = true;

                lbGroupName.Visible = true;
                plGroupSet.Visible = true;
                gridGroupSet.Visible = true;
                plGroup.Visible = false;
                gridGroupInfo.Visible = false;
                plModifityHeader.Visible = false;
                treeGroupCCTV.Visible = false;
                gridGroupCCTV.Visible = false;
                DisableSetSaveGroup();

                pbAlarm.Location = new Point(btnSaveGroupSet.Location.X + btnSaveGroupSet.Size.Width + 10, 12);
                labelStatus.Location = new Point(pbAlarm.Location.X + pbAlarm.Size.Width + 10, 19);
                btnClearAlarm.Location = new Point(labelStatus.Location.X + labelStatus.Size.Width, 20);

                tabPageIndexTree_.Refresh();
                tabPageIndexGrid_.Refresh();
                tabPageGroupSetBody_.Refresh();
            }
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            if (plSetting.Visible == false)
            {
                plSetting.BringToFront();
                plSetting.Visible = true;
            }
            else
                plSetting.Visible = false;
        }

        private void btnAlarmList_Click(object sender, EventArgs e)
        {
            if (m_dicAlarms.Count() > 0 && plAlarmList.Visible == false)
            {
                AlarmList_Load();
                plAlarmList.BringToFront();
                plAlarmList.Visible = true;
            }
            else
            {
                plAlarmList.Visible = false;
            }

        }

        private void AlarmList_Load()
        {
            List<Alarm> alarms = m_dicAlarms.Values.ToList();
            plAlarmList.Controls.Clear();
            plAlarmList.Height = m_dicAlarms.Count() * 25 + 35;

            for (int i = 0; i < alarms.Count; i++)
            {
                Label lbAlarm = new Label();
                lbAlarm.AutoSize = true;
                lbAlarm.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                lbAlarm.Location = new Point(15,(i * 25) + 20);
                lbAlarm.Text = alarms[i].AlarmListText;

                plAlarmList.Controls.Add(lbAlarm);

                ImageButton btnAlarmClear = new ImageButton();
                btnAlarmClear.Name = alarms[i].CCTV.ID.ToString();
                btnAlarmClear.ImageClicked = global::CCTVSingleViewer.Properties.Resources.btnClearAlarm_Click;
                btnAlarmClear.ImageMouseOver = global::CCTVSingleViewer.Properties.Resources.btnClearAlarm_MouseOver;
                btnAlarmClear.ImageNormal = global::CCTVSingleViewer.Properties.Resources.btnClearAlarm_Normal;
                btnAlarmClear.Size = new System.Drawing.Size(18, 18);
                btnAlarmClear.Location = new Point(lbAlarm.Location.X + lbAlarm.Size.Width + 5, lbAlarm.Location.Y - 1);
                btnAlarmClear.Click += btnAlarmClear_Click;

                plAlarmList.Controls.Add(btnAlarmClear);
            }
        }

        private void btnAlarmClear_Click(object sender, EventArgs e)
        {
            ImageButton btn = (ImageButton)sender;
            int nCCTVID = Convert.ToInt32(btn.Name);

            DialogResult result = MessageBox.Show("해당 알람을 해지 하시겠습니까?", "알람해지", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                ClearAlarm(nCCTVID);
            }
        }

        private void ClearAlarm(int nCCTVID)
        {
            if (nCCTVID == m_currentAlarm.CCTV.ID)
            {
                labelStatus.Text = "";
                pbAlarm.Visible = false;
                btnClearAlarm.Visible = false;
                pbAlarmOval.Visible = false;
                lbAlarmNum.Visible = false;

                OnAlarmOff(m_currentAlarm.CCTV.ID, DateTime.Now);
                btnHome_Click(null, null);
            }
            else
            {
                CCTV cctv = null;

                foreach (DataGridViewRow row in gridCCTV.Rows)
                {
                    CCTV _cctv = (CCTV)row.Tag;

                    if (_cctv != null && _cctv.ID == nCCTVID)
                    {
                        cctv = _cctv;
                        break;
                    }
                }

                if (cctv == null)
                    return;

                Alarm alarm = null;
                List<Alarm> alarms = m_dicAlarms.Values.ToList();

                foreach (Alarm _alarm in alarms)
                {
                    if (_alarm.CCTV == cctv)
                    {
                        alarm = _alarm;
                        break;
                    }
                }

                if (alarm == null)
                    return;

                this.Invoke((MethodInvoker)delegate
                {
                    Alarm removed;

                    m_dicAlarms.TryRemove(alarm, out removed);
                });

                lbAlarmNum.Text = m_dicAlarms.Count().ToString();
            }

            AlarmList_Load();
        }

        private void TimeTrace(string strLog)
        {
            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("[{0:00}:{1:00}:{2:00}] : ", dtNow.Hour, dtNow.Minute, dtNow.Second);
            System.Diagnostics.Trace.WriteLine(strTime + strLog);
        }

        private void MakeBigPanel(CCTVPanel panel)
        {
            m_dicCCTVs.Clear();

            m_bigPanel = panel;
            SetCCTVControls();

            /*foreach (CCTVPanel cctvPanel in m_panels)
            {
                m_dicCCTVs[cctvPanel] = cctvPanel.CCTV;
                cctvPanel.Connect(null);
            }*/

            CCTV cctv;

            if (m_dicCCTVs.TryGetValue(m_bigPanel, out cctv))
            {
                m_bigPanel.Connect(cctv, false);
            }
        }

        private void tbAutoCancleMinute_KeyPress(object sender, KeyPressEventArgs e)
        {
            //숫자만 입력되도록 필터링
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))    //숫자와 백스페이스를 제외한 나머지를 바로 처리
            {
                e.Handled = true;
            }
        }

        private void cbAutoCancle_Click(object sender, EventArgs e1)
        {
            if (cbAutoCancle.Checked == true)
            {
                List<Alarm> alarms = m_dicAlarms.Values.ToList();

                foreach (Alarm alarm in alarms)
                {
                    System.Windows.Forms.Timer tt = new System.Windows.Forms.Timer();
                    tt.Interval = Convert.ToInt32(tbAutoCancleMinute.Text) * 60 * 1000;

                    tt.Tick += (s, e2) =>
                    {
                        ClearAlarm(alarm.CCTV.ID);
                        tt.Stop();
                        m_listTimers.Remove(tt);
                    };

                    m_listTimers.Add(tt);

                    tt.Start();
                }
            }
            else
            {
                DialogResult result = MessageBox.Show("자동 알람해지 기능이 중지됩니다.", "자동 알람해지 설정", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    if (m_listTimers.Count > 0)
                    {
                        for (int i = 0; i < m_listTimers.Count; i++)
                        {
                            m_listTimers[i].Stop();
                        }
                    }
                }
                else
                {
                    cbAutoCancle.Checked = true;
                }
            }

            SaveSetting();
        }

        private void tbAutoCancleMinute_KeyUp(object sender, KeyEventArgs e)
        {
            SaveSetting();
        }

        private void MakeSmallPanel(CCTVPanel panel)
        {
            m_bigPanel = null;
            SetCCTVControls();

            foreach (KeyValuePair<CCTVPanel, CCTV> pair in m_dicCCTVs)
            {
                pair.Key.Connect(pair.Value);
            }

            m_dicCCTVs.Clear();
        }

        private void btnModifityGroupSet_Click(object sender, EventArgs e)
        {
            if (m_nSelectRowGroup < 0)
            {
                MessageBox.Show("그룹을 선택해주세요.", "그룹설정");
                return;
            }

            plGroupSet.Visible = false;
            gridGroupSet.Visible = false;

            plGroup.Visible = true;
            plModifityHeader.Visible = true;
            gridGroupInfo.Visible = true;

            SetCCTVArray(m_selectRowGroupCCTVs);
            SetGroupInfo(m_selectRowGroupCCTVs);
            lbGroupName.Text = m_strSelectRowGroup;

            EnableSetHome();

            if (btnModifityTree.IsChecked == true)
            {
                treeGroupCCTV.Visible = true;
                gridGroupCCTV.Visible = false;
            }
            else
            {
                treeGroupCCTV.Visible = false;
                gridGroupCCTV.Visible = true;
            }
        }

        private void btnModifityTree_Click(object sender, EventArgs e)
        {
            if (btnModifityTree.IsChecked == false)
            {
                btnModifityTree.IsChecked = true;
                btnModifityGrid.IsChecked = false;
                treeGroupCCTV.Visible = true;
                gridGroupCCTV.Visible = false;

                btnModifityTree.Refresh();
                btnModifityGrid.Refresh();
            }
        }

        private void btnModifityGrid_Click(object sender, EventArgs e)
        {
            if (btnModifityGrid.IsChecked == false)
            {
                btnModifityTree.IsChecked = false;
                btnModifityGrid.IsChecked = true;
                treeGroupCCTV.Visible = false;
                gridGroupCCTV.Visible = true;

                btnModifityTree.Refresh();
                btnModifityGrid.Refresh();
            }
        }

        private void btnSaveGroupSet_Click(object sender, EventArgs e)
        {
            if (m_nSelectRowGroup < 0)
            {
                MessageBox.Show("그룹을 선택해주세요.", "그룹설정");
                return;
            }

            DialogResult result = MessageBox.Show("변경사항이 적용 됩니다. \n해당 그룹이 현재 상태로 적용됩니다.", "그룹셋 설정", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                SaveEquipZoneCCTVList();

                // 수정된 EquipZoneCCTVList 파일로 다시 GroupSet 수정
                Dictionary<int, CCTV> dicCCTVs = ReadCCTVList();
                ReadEquipZoneCCTVList(dicCCTVs);

                plGroupSet.Visible = true;
                gridGroupSet.Visible = true;
                plGroup.Visible = false;
                gridGroupInfo.Visible = false;
                plModifityHeader.Visible = false;
                treeGroupCCTV.Visible = false;
                gridGroupCCTV.Visible = false;
            }
        }

        private void SaveEquipZoneCCTVList()
        {
            StreamReader reader = new StreamReader(m_strEquipZoneCCTVListPath, Encoding.Default);
            StreamWriter writer = new StreamWriter(m_strEquipZoneCCTVListTempPath, false, Encoding.Default);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] tokens = strLine.Split('\t');

                int nEquipZoneID;
                string sEquipZoneName;

                if (int.TryParse(tokens[0].Trim(), out nEquipZoneID) == false)
                {
                    writer.WriteLine(strLine);

                    continue;
                }

                if (nEquipZoneID == m_nSelectRowGroup)
                {
                    sEquipZoneName = tokens[1].Trim();

                    strLine = nEquipZoneID + "\t" + sEquipZoneName;

                    foreach (CCTVPanel panel in m_panels)
                    {
                        strLine += "\t";

                        if (panel.CCTV != null)
                            strLine += panel.CCTV.ID;
                    }
                }

                writer.WriteLine(strLine);
            }

            reader.Close();
            writer.Close();

            FileInfo file = new FileInfo(m_strEquipZoneCCTVListTempPath);

            if (file.Exists)
            {
                file.CopyTo(m_strEquipZoneCCTVListPath, true);
            }
        }

        private void treeCCTV_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                TreeNode node = (TreeNode)e.Item;

                if (node.Tag != null && node.Tag is CCTV)
                {
                    CCTV cctv = (CCTV)node.Tag;
                    treeCCTV.DoDragDrop(cctv, DragDropEffects.All);
                }
            }
        }

        private void gridGroupSet_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.RowIndex < 0)
                    return;

                DataGridViewRow row = gridGroupSet.Rows[e.RowIndex];
                m_selectRowGroupCCTVs = (CCTV[])row.Tag;
                m_nSelectRowGroup = (int)row.Cells[0].Tag;
                m_strSelectRowGroup = (string)row.Cells[1].Value;
            }
        }

        private void LoadSetting()
        {
            char sp = ',';
            string strChkData;
            string[] spStrings = null;

            StreamReader inputFile;

            try
            {
                inputFile = new StreamReader(m_strFilePath);
                

                if ((strChkData = inputFile.ReadLine()) != null)
                {
                    spStrings = strChkData.Split(sp);

                    if (spStrings.Length == 1 || spStrings.Length == 2)
                    {
                        if (spStrings[0] == "True")
                        {
                            cbAutoCancle.Checked = true;
                        }

                        if (spStrings[1] != null)
                        {
                            tbAutoCancleMinute.Text = spStrings[1];
                        }
                    }
                }

                inputFile.Close();
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
        }

        private void SaveSetting()
        {
            StreamWriter outputFile;
            outputFile = new StreamWriter(m_strFilePath, false, Encoding.Default);

            string strSetting = string.Format("{0},{1}", cbAutoCancle.Checked, tbAutoCancleMinute.Text);

            outputFile.WriteLine(strSetting);
            outputFile.Close();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {

        }

        
    }

    public class SoundPlayerEx : System.Media.SoundPlayer
    {
        private bool m_isPlaying = false;

        public new void Play()
        {
            if (m_isPlaying)
                Stop();

            m_isPlaying = true;
            base.PlayLooping();
        }

        public new void Stop()
        {
            base.Stop();
            m_isPlaying = false;
        }

        protected override void Dispose(bool disposing)
        {
            Stop();
            base.Dispose(disposing);
        }
    }

    
}
