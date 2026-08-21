using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace SOPDisasterSystem
{
    public partial class FormLayout2 : Form
    {
        private FormMain m_frmMain = null;

        private int m_nLayout = 1;

        private VirtoolsViewer.Player[] m_player = { new VirtoolsViewer.Player(), new VirtoolsViewer.Player(), new VirtoolsViewer.Player(), new VirtoolsViewer.Player() };
        private string m_strOutsideCMO = "";
        private Dictionary<string, string> m_dicInsideCMO = null;
        private bool[] m_isInit = { false, false, false, false };
        private bool[] m_isResize = { false, false, false, false };

        private DateTime m_time;
        private string m_strDownloadLabel = "";
        private string m_strCMOFolderPath = ".\\";
        private string[] m_strSelectedBuilding = new string[4] {"", "", "", ""};
        private VirtoolsViewer.Player m_currentPlayer = null;
        private int[] m_nAutoNaviOpt = new int[4] {0, 0, 0, 0};

        private MouseInfo m_mouseInfo = new MouseInfo();

        public enum ZoomMode { ZOOM_IN = 0, ZOOM_OUT, NONE };

        private ZoomMode m_zoomMode = ZoomMode.NONE;

        private SOPMonitoringSystem.Data_Building m_buildingCurrent = null;
        
        public FormLayout2(FormMain main)
        {
            InitializeComponent();

            m_frmMain = main;
        }

        public void Layout1()
        {
            panelLayout1.Dock = DockStyle.Fill;
            
            m_mouseInfo.Player = m_player[0];
            m_frmMain.SetFloorStatus(false, -1, -1);
        }
        
        public void Layout2()
        {
            Size sz = m_frmMain.GetPaneVirtool().Size;
            panelLayout1.Size = new Size((sz.Width / 2 - 1), Height);
            panelLayout2.Location = new Point((sz.Width / 2 + 1), 0);
            panelLayout2.Size = new Size((sz.Width / 2 - 1), Height);
            panelLayout2.Show();
            
            m_mouseInfo.Player = m_player[0];
            m_frmMain.SetFloorStatus(false, -1, -1);
        }
        
        public void Layout3()
        {
            Size sz = m_frmMain.GetPaneVirtool().Size;
            panelLayout1.Size = new Size(sz.Width, (sz.Height / 2 - 1));
            panelLayout3.Location = new Point(0, (sz.Height / 2 + 1) );
            panelLayout3.Size = new Size(Width, (Height / 2));
            panelLayout3.Show();

            m_mouseInfo.Player = m_player[0];

            SOPMonitoringSystem.Data_Building buildingSelected = m_frmMain.GetSpace().GetSelectedBuilding();
            m_buildingCurrent = buildingSelected;

            if (buildingSelected == null)
                m_frmMain.SetFloorStatus(false, -1, -1);
            else
                m_frmMain.SetFloorStatus(true, buildingSelected.MinFloor, buildingSelected.MaxFloor);
        }
        
        public void Layout4()
        {
            Size sz = m_frmMain.GetPaneVirtool().Size;
            panelLayout1.Size = new Size((sz.Width / 2 - 1), (sz.Height / 2));
            panelLayout2.Location = new Point((sz.Width / 2 + 1), 0);
            panelLayout2.Size = new Size((sz.Width / 2 - 1), (sz.Height / 2));

            panelLayout3.Location = new Point(0, (sz.Height / 2 + 2));
            panelLayout3.Size = new Size((sz.Width / 2 - 1), (Height / 2));
            panelLayout4.Location = new Point((sz.Width / 2 + 1), (sz.Height / 2 + 2));
            panelLayout4.Size = new Size((sz.Width / 2 - 1), (sz.Height / 2));
            panelLayout2.Show();
            panelLayout3.Show();
            panelLayout4.Show();

            m_mouseInfo.Player = m_player[0];

            SOPMonitoringSystem.Data_Building buildingSelected = m_frmMain.GetSpace().GetSelectedBuilding();
            m_buildingCurrent = buildingSelected;

            if (buildingSelected == null)
                m_frmMain.SetFloorStatus(false, -1, -1);
            else
                m_frmMain.SetFloorStatus(true, buildingSelected.MinFloor, buildingSelected.MaxFloor);
        }

        public void LayoutHide()
        {
            panelLayout1.Dock = DockStyle.None;
            panelLayout2.Hide();
            panelLayout3.Hide();
            panelLayout4.Hide();
        }

        public void SetFilePath(string strCMOFolderPath, string strOutsideFilePath, Dictionary<string, string> dicInsideCMO)
        {
            m_strCMOFolderPath = strCMOFolderPath;
            m_strOutsideCMO = strOutsideFilePath;
            m_dicInsideCMO = dicInsideCMO;
        }
        
        public void SetLayoutMode(int nLayout)
        {
            m_nLayout = nLayout;
        }
        
        private void FormLayout_Resize(object sender, EventArgs e)
        {
            switch(m_nLayout)
            {
                case 1:
                    Layout1();
                    break;
                case 2:
                    Layout2();
                    break;
                case 3:
                    Layout3();
                    break;
                case 4:
                    Layout4();
                    break;
            }
        }

        private void ReadFireEquip(int nBuildingGroupID, string strFileName, string strEquipType)
        {
            SOPMonitoringSystem.WebDBManager dbMgr = m_frmMain.GetMain().DBManager;

            string strSQL = string.Format("Select fe.ID, fe.EquipID, fe.X, fe.Y zon.FloorIndex from FireEquipment as fe inner join Zone as zon on ZoneID = zon.ID and ZoneID in (select id from Zone where BuildingID in (select id from Building where BuildingGroupID = {0})) order by FloorIndex", nBuildingGroupID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            StreamWriter sw = null;
            int nPrevFloorIndex = -10000;

            int nResultCount = arrResult.Count;
            int nFloorHeight = 5000;    // 5000 mm(5미터)

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strEquipID = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                int x = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                int y = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0);
                int nFloorIndex = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);

                if (nFloorIndex != nPrevFloorIndex)
                {
                    nPrevFloorIndex = nFloorIndex;
                    string strFilePath = string.Format("{0}{1}F.txt", m_strCMOFolderPath + strFileName, nFloorIndex + 1);
                    
                    if (sw != null)
                        sw.Close();

                    sw = new StreamWriter(strFilePath, false, Encoding.Default);
                }

                sw.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}", strEquipType, x, y, nFloorHeight * nFloorIndex, strEquipID));
            }

            if (sw != null)
                sw.Close();
        }

        private void ReadFireEquip()
        {
            // 1, 2호기
            ReadFireEquip(1, "Plant12Fire", "소화기");
            // 3, 4호기
            ReadFireEquip(2, "Plant34Fire", "소화기");
            // 5, 6호기
            ReadFireEquip(3, "Plant56Fire", "소화기");
        }

        private void FormLayout_Load(object sender, EventArgs e)
        {
            // 메시지 파일들의 내용을 모두 지운다.
            InitMessageFiles();

            m_strDownloadLabel = label1.Text;
            m_time = DateTime.Now;
            timer1.Start();
        }

        private void InitMessageFiles()
        {
            if (System.IO.File.Exists("ObjectNameA.txt"))
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter("ObjectNameA.txt", false);
                sw.Close();
            }

            if (System.IO.File.Exists("ObjectNameB.txt"))
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter("ObjectNameB.txt", false);
                sw.Close();
            }

            if (System.IO.File.Exists("ObjectNameC.txt"))
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter("ObjectNameC.txt", false);
                sw.Close();
            }

            if (System.IO.File.Exists("ObjectNameD.txt"))
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter("ObjectNameD.txt", false);
                sw.Close();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DrawPanel();
            Zoom();
        }

        private void Zoom()
        {
            VirtoolsViewer.Player player = m_mouseInfo.Player;

            if (player == null)
            {
                if (m_isInit[0])
                    player = m_player[0];
                else
                    return;
            }

            if (player != null)
            {
                if (m_zoomMode == ZoomMode.ZOOM_IN)
                    player.SendMessage("Level", "ZoomIn");
                else if (m_zoomMode == ZoomMode.ZOOM_OUT)
                    player.SendMessage("Level", "ZoomOut");
            }
        }

        private void DrawPanel()
        {
            DrawPanel1();
            DrawPanel2();
            DrawPanel3();
            DrawPanel4();
        }

        private bool ReadMessageFile(VirtoolsViewer.Player player, string strFileName, out string strMessage)
        {
            if (File.Exists(strFileName))
            {
                try
                {
                    System.IO.StreamReader reader = new System.IO.StreamReader(strFileName, Encoding.Default);
                    strMessage = reader.ReadLine();
                    reader.Close();

                    if (strMessage != null && strMessage.Length > 0)
                    {
                        // 파일 내용을 지운다.
                        System.IO.StreamWriter sw = new System.IO.StreamWriter(strFileName, false);
                        sw.Close();
                        return true;
                    }
                }
                catch (FileNotFoundException)
                {
                    strMessage = null;
                }
            }
            else
            { 
                strMessage = null; 
            }

            return false;
        }

        private void SelectOutdoorBuilding(ref string strSelectedBuilding, string strMessage)
        {
            if (strMessage.Contains("BLD")) // BuildingType
            {
                if (strMessage.Contains("GA01"))
                    strSelectedBuilding = "1, 2호기 및 부속 건물\\터빈 및 주 제어실";
                else if (strMessage.Contains("GA02"))
                    strSelectedBuilding = "1, 2호기 및 부속 건물\\보일러실";
                else if (strMessage.Contains("GA03"))
                    strSelectedBuilding = "1, 2호기 및 부속 건물\\물처리실";
                else if (strMessage.Contains("GB01"))
                    strSelectedBuilding = "3, 4호기 및 부속 건물\\터빈 및 주 제어실";
                else if (strMessage.Contains("GB02"))
                    strSelectedBuilding = "3, 4호기 및 부속 건물\\보일러실 (3호기)";
                else if (strMessage.Contains("GB03"))
                    strSelectedBuilding = "3, 4호기 및 부속 건물\\보일러실 (4호기)";
                else if (strMessage.Contains("GC01"))
                    strSelectedBuilding = "5, 6호기 및 부속 건물\\터빈 및 주 제어실";
                else if (strMessage.Contains("GC02"))
                    strSelectedBuilding = "5, 6호기 및 부속 건물\\보일러실 (5호기)";
                else if (strMessage.Contains("GC03"))
                    strSelectedBuilding = "5, 6호기 및 부속 건물\\보일러실 (6호기)";
                else
                {
                    strSelectedBuilding = "";
                    return;
                }

                m_frmMain.GetSpace().SelectItem(strSelectedBuilding);
            }
        }

        private void DrawPanel1()
        {
            if (m_isInit[0])
            {
                string strMessage;
                if (ReadMessageFile(m_player[0], "ObjectNameA.txt", out strMessage))
                {
                    SelectOutdoorBuilding(ref m_strSelectedBuilding[0], strMessage);
                }

                if (m_isResize[0])
                {
                    m_player[0].Resize(0, 0, panelLayout1.ClientSize.Width, panelLayout1.ClientSize.Height);
                    m_isResize[0] = false;
                    Layout1();
                }

                m_player[0].Process();
            }
            else
            {
                label1.Show();

                DateTime t = DateTime.Now;
                if (t.Second != m_time.Second)
                {
                    if (label1.Text.Length > m_strDownloadLabel.Length + 5)
                        label1.Text = m_strDownloadLabel;
                    else
                        label1.Text = label1.Text + ".";

                    m_time = t;
                }

                int x = this.ClientRectangle.Width / 2 - (int)label1.Font.Size * m_strDownloadLabel.Length / 2;
                int y = this.ClientRectangle.Height / 2;

                this.label1.Location = new Point(x, y);

                if (m_strOutsideCMO != "")
                {                    
                    m_isInit[0] = m_player[0].InitPlayer(this.Handle, panelLayout1.Handle, m_strOutsideCMO);

                    if (m_isInit[0])
                    {
                        m_currentPlayer = m_player[0];
                        m_player[0].SendMessage("Level", "GetObjectNameA");
                        m_player[0].PauseInput(true);
                        label1.Hide();

                        panelLayout1.SetParentForm(this);
                    }
                }
            }
        }

        private void DrawPanel2()
        {
            if (m_nLayout != 2 && m_nLayout != 4)
                return;

            if (m_isInit[1])
            {
                string strMessage;
                if (ReadMessageFile(m_player[1], "ObjectNameB.txt", out strMessage))
                {
                    SelectOutdoorBuilding(ref m_strSelectedBuilding[1], strMessage);
                }

                m_player[1].Process();

                if (m_isResize[1])
                {
                    m_player[1].Resize(0, 0, panelLayout2.ClientSize.Width, panelLayout2.ClientSize.Height);
                    m_isResize[1] = false;
                    Layout2();
                }
            }
            else
            {
                if (m_strOutsideCMO != "")
                {
                    m_isInit[1] = m_player[1].InitPlayer(this.Handle, panelLayout2.Handle, m_strOutsideCMO);
                    if (m_isInit[1])
                    {
                        m_player[1].SendMessage("Level", "GetObjectNameB");
                        m_player[1].SendMessage("Level", "HomeView");
                        m_player[1].PauseInput(true);

                        panelLayout2.SetParentForm(this);
                    }
                }
            }
        }

        private void DrawPanel3()
        {
            if (m_nLayout != 3 && m_nLayout != 4)
                return;

            if (m_isInit[2])
            {
                string strMessage;
                if (ReadMessageFile(m_player[2], "ObjectNameC.txt", out strMessage))
                {
                    // Indoor
                }

                if (m_isResize[2])
                {
                    m_player[2].Resize(0, 0, panelLayout3.ClientSize.Width, panelLayout3.ClientSize.Height);
                    m_isResize[2] = false;
                    Layout3();
                }

                m_player[2].Process();
            }
            else
            {
                if (m_dicInsideCMO.ContainsKey("Inside"))
                {
                    m_isInit[2] = m_player[2].InitPlayer(this.Handle, panelLayout3.Handle, m_dicInsideCMO["Inside"]);
                    if (m_isInit[2])
                    {
                        m_player[2].SendMessage("Level", "GetObjectNameC");
                        m_player[2].SendMessage("Level", "ShowPlant34_1F");
                        m_player[2].PauseInput(true);

                        panelLayout3.SetParentForm(this);
                    }
                }
            }
        }

        private void DrawPanel4()
        {
            if (m_nLayout != 4)
                return;

            if (m_isInit[3])
            {
                string strMessage;
                if (ReadMessageFile(m_player[3], "ObjectNameD.txt", out strMessage))
                {
                    SelectOutdoorBuilding(ref m_strSelectedBuilding[3], strMessage);
                }

                if (m_isResize[3])
                {
                    m_player[3].Resize(0, 0, panelLayout4.ClientSize.Width, panelLayout4.ClientSize.Height);
                    m_isResize[3] = false;
                    Layout4();
                }

                m_player[3].Process();
            }
            else
            {
                if (m_strOutsideCMO != "")
                {
                    m_isInit[3] = m_player[3].InitPlayer(this.Handle, panelLayout4.Handle, m_strOutsideCMO);
                    if (m_isInit[3])
                    {
                        m_player[3].SendMessage("Level", "GetObjectNameD");
                        m_player[3].SendMessage("Level", "AllView");
                        m_player[3].PauseInput(true);

                        panelLayout4.SetParentForm(this);
                    }
                }
            }
        }

        private void panelLayout1_SizeChanged(object sender, EventArgs e)
        {
            if (m_isInit[0])
                m_player[0].Resize(0, 0, panelLayout1.ClientSize.Width, panelLayout1.ClientSize.Height);
            else
                m_isResize[0] = true;
        }

        private void panelLayout2_SizeChanged(object sender, EventArgs e)
        {
            if (m_isInit[1])
                m_player[1].Resize(0, 0, panelLayout2.ClientSize.Width, panelLayout2.ClientSize.Height);
            else
                m_isResize[1] = true;
        }

        private void panelLayout3_SizeChanged(object sender, EventArgs e)
        {
            if (m_isInit[2])
                m_player[2].Resize(0, 0, panelLayout3.ClientSize.Width, panelLayout3.ClientSize.Height);
            else
                m_isResize[2] = true;
        }

        private void panelLayout4_SizeChanged(object sender, EventArgs e)
        {
            if (m_isInit[3])
                m_player[3].Resize(0, 0, panelLayout4.ClientSize.Width, panelLayout4.ClientSize.Height);
            else
                m_isResize[3] = true;
        }

        private void panelLayout1_MouseLeave(object sender, EventArgs e)
        {
            if (m_isInit[0])
            {
                m_player[0].PauseInput(true);
                //if (m_currentPlayer == m_player[0])
                //    m_currentPlayer = null;
            }
        }

        private void panelLayout1_MouseEnter(object sender, EventArgs e)
        {
            if (m_isInit[0])
            {
                m_player[0].PauseInput(false);
                m_currentPlayer = m_player[0];
            }
        }

        private void panelLayout2_MouseEnter(object sender, EventArgs e)
        {
            if (m_isInit[1])
            {
                m_player[1].PauseInput(false);
                m_currentPlayer = m_player[1];
            }
        }

        private void panelLayout2_MouseLeave(object sender, EventArgs e)
        {
            if (m_isInit[1])
            {
                m_player[1].PauseInput(true);
                //if (m_currentPlayer == m_player[1])
                //    m_currentPlayer = null;
            }
        }

        private void panelLayout3_MouseEnter(object sender, EventArgs e)
        {
            if (m_isInit[2])
            {
                m_player[2].PauseInput(false);
                m_currentPlayer = m_player[2];
            }
        }

        private void panelLayout3_MouseLeave(object sender, EventArgs e)
        {
            if (m_isInit[2])
            {
                m_player[2].PauseInput(true);
                //if (m_currentPlayer == m_player[2])
                //    m_currentPlayer = null;
            }
        }

        private void panelLayout4_MouseEnter(object sender, EventArgs e)
        {
            if (m_isInit[3])
            {
                m_player[3].PauseInput(false);
                m_currentPlayer = m_player[3];
            }
        }

        private void panelLayout4_MouseLeave(object sender, EventArgs e)
        {
            if (m_isInit[3])
            {
                m_player[3].PauseInput(true);
                //if (m_currentPlayer == m_player[0])
                //    m_currentPlayer = null;
            }
        }

        public VirtoolsViewer.Player GetCurrentPlayer()
        {
            return m_currentPlayer;
        }

        public void AutoNavigation(VirtoolsViewer.Player player)
        {
            if (player == null)
                return;

            int nIndex = -1;

            if (player == m_player[0])
                nIndex = 0;
            else if (player == m_player[1])
                nIndex = 1;
            else if (player == m_player[2])
                nIndex = 2;
            else if (player == m_player[3])
                nIndex = 3;
            else
                return;

            if (m_nAutoNaviOpt[nIndex] == 0)
                player.SendMessage("Level", "PlayLine001");
            else if (m_nAutoNaviOpt[nIndex] == 1)
                player.SendMessage("Level", "ReStopPlay");
            else
                player.SendMessage("Level", "ReStopPlay");

            m_nAutoNaviOpt[nIndex]++;
            if (m_nAutoNaviOpt[nIndex] > 2) m_nAutoNaviOpt[nIndex] = 1;
        }

        public void ShowIndoor(int nFloorIndex)
        {
            if (m_isInit[3])
            {
                // 현재 4층까지만 있음
                if (nFloorIndex >= 1 && nFloorIndex <= 4)
                {
                    // ShowPlant34_2F
                    string strMsg = string.Format("ShowPlant34_{0}F", nFloorIndex);
                    m_player[3].SendMessage("Level", strMsg);
                }
            }
        }

        private void setDisasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
//            if (m_currentPlayer != null)
//                ZBobb.win32.SendMessage(panelLayout1.Handle, 0x0201, (IntPtr)0, (IntPtr)XY);
        }

        private void releaseDisasterToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        public void OnMouseDown(VirtoolPanel panel, int x, int y, MouseButtons btn)
        {
            m_mouseInfo.Button = btn;
            m_mouseInfo.Panel = panel;
            m_mouseInfo.X = x;
            m_mouseInfo.Y = y;

            if (panel == this.panelLayout1)
                m_mouseInfo.Player = m_player[0];
            else if (panel == this.panelLayout2)
                m_mouseInfo.Player = m_player[1];
            else if (panel == this.panelLayout3)
                m_mouseInfo.Player = m_player[2];
            else if (panel == this.panelLayout4)
                m_mouseInfo.Player = m_player[3];

            if (btn == System.Windows.Forms.MouseButtons.Right)
            {
                this.contextMenuStrip1.Show(panel, new Point(x, y));
            }
        }

        public void StartZoomIn()
        {
            m_zoomMode = ZoomMode.ZOOM_IN;
        }

        public void FinishZoomIn()
        {
            m_zoomMode = ZoomMode.NONE;
        }

        public void StartZoomOut()
        {
            m_zoomMode = ZoomMode.ZOOM_OUT;
        }

        public void FinishZoomOut()
        {
            m_zoomMode = ZoomMode.NONE;
        }

        public void SetCurrentBuilding(SOPMonitoringSystem.Data_Building building)
        {
            if (m_buildingCurrent == building)
                return;

            m_buildingCurrent = building;

            if (m_nLayout == 3 || m_nLayout == 4)
            {
                // Change 실내층
                //ShowIndoor(
                m_frmMain.SetFloorStatus(true, building.MinFloor, building.MaxFloor);
            }
        }
    }

    public class VirtoolPanel : Panel
    {
        private FormLayout2 m_frmParent = null;

        public void SetParentForm(FormLayout2 frm)
        {
            m_frmParent = frm;
        }

        private void OnMouseDown(IntPtr lParam, MouseButtons btn)
        {
            int y = lParam.ToInt32() >> 16;
            int x = lParam.ToInt32() & 0xffff;

            if (m_frmParent != null)
                m_frmParent.OnMouseDown(this, x, y, btn);
        }

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == 0x0204)    // RBUTTONDOWN
            {
                OnMouseDown(m.LParam, System.Windows.Forms.MouseButtons.Right);
            }
            else if (m.Msg == 0x0201)   // LBUTTONDOWN
            {
                OnMouseDown(m.LParam, System.Windows.Forms.MouseButtons.Left);
            }
            else if (m.Msg == 0x0207)   // MBUTTONDOWN
            {
                OnMouseDown(m.LParam, System.Windows.Forms.MouseButtons.Middle);
            }
            else
                base.DefWndProc(ref m);
        }
    }

    class MouseInfo
    {
        private MouseButtons btn = MouseButtons.None;
        private int x = 0, y = 0;
        private VirtoolPanel panel = null;
        private VirtoolsViewer.Player player = null;

        public MouseInfo()
        {
        }

        public MouseInfo(MouseButtons btn, int x, int y, VirtoolPanel panel, VirtoolsViewer.Player player)
        {
            this.btn = btn;
            this.x = x;
            this.y = y;
            this.panel = panel;
            this.player = player;
        }

        public MouseButtons Button
        {
            get { return btn; }
            set { btn = value; }
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public VirtoolPanel Panel
        {
            get { return panel; }
            set { panel = value; }
        }

        public VirtoolsViewer.Player Player
        {
            get { return player; }
            set { player = value; }
        }
    }
}
