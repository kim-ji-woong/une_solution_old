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
    public partial class FormLayout : Form
    {
        private FormMain m_fromMain = null;

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

        public FormLayout(FormMain main)
        {
            InitializeComponent();

            m_fromMain = main;
        }

        public void Layout1()
        {
            panelLayout1.Dock = DockStyle.Fill;
        }
        
        public void Layout2()
        {
            Size sz = m_fromMain.GetPaneVirtool().Size;
            panelLayout1.Size = new Size((sz.Width / 2 - 1), Height);
            panelLayout2.Location = new Point((sz.Width / 2 + 1), 0);
            panelLayout2.Size = new Size((sz.Width / 2 - 1), Height);
            panelLayout2.Show();
        }
        
        public void Layout3()
        {
            Size sz = m_fromMain.GetPaneVirtool().Size;
            panelLayout1.Size = new Size(sz.Width, (sz.Height / 2 - 1));
            panelLayout3.Location = new Point(0, (sz.Height / 2 + 1) );
            panelLayout3.Size = new Size(Width, (Height / 2));
            panelLayout3.Show();
        }
        
        public void Layout4()
        {
            Size sz = m_fromMain.GetPaneVirtool().Size;
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
            SOPMonitoringSystem.WebDBManager dbMgr = m_fromMain.GetMain().GetDBManager();

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
                int nID = dbMgr.GetIntField(arrResult[i].ToString(), 0);
                string strEquipID = dbMgr.GetStringField(arrResult[i + 1].ToString(), "");
                int x = dbMgr.GetIntField(arrResult[i + 2].ToString(), 0);
                int y = dbMgr.GetIntField(arrResult[i + 3].ToString(), 0);
                int nFloorIndex = dbMgr.GetIntField(arrResult[i + 4].ToString(), 0);

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
            DrawPanel1();
            DrawPanel2();
            DrawPanel3();
            DrawPanel4();
        }

        private bool ReadMessageFile(VirtoolsViewer.Player player, string strFileName, out string strMessage)
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

                m_fromMain.GetSpace().SelectItem(strSelectedBuilding);
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

        public void ZoomIn()
        {
            bool result = true;
            if (m_isInit[0])
                result = m_player[0].SendMessage("Level", "ZoomIn");
        }

        public void ZoomOut()
        {
            if (m_isInit[0])
                m_player[0].SendMessage("Level", "ZoomOut");
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

        private void FormLayout_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
            }
        }
    }
}
