using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections;
using XtremeDockingPane;
using System.Net;

namespace SOPDisasterSystem
{
    public partial class FormMain : Form
    {
        private FormLeftSpace m_frmSpace = null;
        private FormBottomLog m_frmLog = new FormBottomLog();
        private FormRightSummary m_frmSummary = new FormRightSummary();
        private FormRightSituation m_frmSituation = null;
        
        private FormLayout m_Layout = null;

        private Form[] arrDocking = new Form[5];
        private SOPMonitoringSystem.FormMain m_frmMain = null;

		private Dictionary<string, string> m_dicInsideCMO = new Dictionary<string, string>();
        private string m_strOutsideCMO = "";
        private ArrayList m_arrTempResult = null;

        ArrayList m_arrBuildingInfo = new ArrayList();
        ArrayList m_arrGroup = new ArrayList();
        ArrayList m_arrBuilding = new ArrayList();

        private bool m_isDown = false;

        protected string m_strSkinFolder;

        public FormMain(SOPMonitoringSystem.FormMain main)
        {
            InitializeComponent();

            m_frmMain = main;

            string strSkinFolder = StylesPath();
            Skin_Load(strSkinFolder);

            GetBuildingInfo(ref m_arrBuildingInfo);
            CreatePane();
            tsViewCtrl_ImageLoad();

            GetSpace().GetBuildingGroup(ref m_arrGroup);
            GetSpace().GetBuilding(ref m_arrBuilding);

            FormLayoutLoad();
            LayoutView(1);

            ReadCMO();
        }
        
        public void Skin_Load(string strSkinFolder)
        {
            axSkinFramework1.LoadSkin(strSkinFolder + "Vista.cjstyles", "");
            axSkinFramework1.ApplyWindow(this.Handle.ToInt32());
            this.BackColor = axSkinFramework1.GetColor(XtremeSkinFramework.XTPColorManagerColor.STDCOLOR_BTNFACE);
        }

        public string StylesPath()
        {
            string strExePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            System.IO.Directory.Exists(strExePath + "\\Styles\\");

            return strExePath + "\\Styles\\";
        }

        private void ReadCMO()
        {
            SOPMonitoringSystem.WebDBManager dbMgr = m_frmMain.GetDBManager();

            string strSQL = "Select Name, URL, AccessedTime from BluePrint where SiteID = 1";
            m_arrTempResult = dbMgr.GetResultData(strSQL, 0);

            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ReadCMO));
            t.Start(this);
        }

        static void ReadCMOHistory(ref Dictionary<string, string> dicCMOHistory)
        {
            string tempPath = System.IO.Path.GetTempPath();

            System.IO.StreamReader reader = null;

            try
            {
                reader = new System.IO.StreamReader(tempPath + "Outside.log", Encoding.Default);
                string strOutsideTime = reader.ReadLine();
                reader.Close();

                dicCMOHistory["Outside"] = strOutsideTime;
            }
            catch (System.IO.FileNotFoundException)
            {
            }

            try
            {
                reader = new System.IO.StreamReader(tempPath + "Inside.log", Encoding.Default);
                string strInsideTime = reader.ReadLine();
                reader.Close();

                dicCMOHistory["Inside"] = strInsideTime;
            }
            catch (System.IO.FileNotFoundException)
            {
            }
        }

        static private void DownloadCMOFile(Dictionary<string, string> dicCMOHistory, string strTag, string strShortTime, WebClient web, string strURL, Dictionary<string, string> dicCMO, ref string strPath)
        {
            string tempPath = System.IO.Path.GetTempPath();
            string localPath = tempPath + strTag + ".cmo";

            if (dicCMOHistory.ContainsKey(strTag) && dicCMOHistory[strTag] == strShortTime)
            {
                if (System.IO.File.Exists(localPath))
                {
                    if (dicCMO == null)
                        strPath = localPath;
                    else
                        dicCMO[strTag] = localPath;

                    return;
                }
            }

            web.DownloadFile(strURL, localPath);

            if (dicCMO == null)
                strPath = localPath;
            else
                dicCMO[strTag] = localPath;

            System.IO.StreamWriter sw = new System.IO.StreamWriter(tempPath + strTag + ".log", false, Encoding.Default);
            sw.WriteLine(strShortTime);
            sw.Close();
        }

        static private void ReadCMO(object param)
        {
            FormMain frm = (FormMain)param;
            SOPMonitoringSystem.WebDBManager dbMgr = frm.m_frmMain.GetDBManager();

            //string strSQL = "Select Name, URL, AccessedTime from BluePrint where SiteID = 1";
            ArrayList arrResult = frm.m_arrTempResult;//dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
            {
                MessageBox.Show("Virtools 파일을 받아올 수 없습니다.\r\n네트웍 상태가 올바른지 확인해 주세요", "File Download Error");
                Application.Exit();
                return;
            }

            WebClient web = new WebClient();
            string strNULL = null;

            Dictionary<string, string> dicCMOHistory = new Dictionary<string, string>();
            ReadCMOHistory(ref dicCMOHistory);            

            DateTime dtDefault = new DateTime();

            for (int i = 0; i < arrResult.Count - 2; i += 3)
            {
                string strName = dbMgr.GetStringField(arrResult[i].ToString(), "");
                string strURL = dbMgr.GetStringField(arrResult[i + 1].ToString(), "");
                DateTime dtAccessed = dbMgr.GetDateTimeField(arrResult[i + 2], dtDefault);

                string strShortTime = dtAccessed.ToShortDateString() + " " + dtAccessed.ToShortTimeString();

                if (!strURL.Contains("http:"))
                    continue;

                if (strName == "All")
                {
                    DownloadCMOFile(dicCMOHistory, "Outside", strShortTime, web, strURL, null, ref frm.m_strOutsideCMO);
                }
                else
                {
                    DownloadCMOFile(dicCMOHistory, "Inside", strShortTime, web, strURL, frm.m_dicInsideCMO, ref strNULL);
                }
            }

            if (frm.m_strOutsideCMO.Length == 0 || frm.m_dicInsideCMO.Count == 0)
            {
                MessageBox.Show("Virtools 파일을 받아올 수 없습니다.\r\n네트웍 상태가 올바른지 확인해 주세요", "File Download Error");
                Application.Exit();
                return;
            }

            frm.m_Layout.SetFilePath(System.IO.Path.GetTempPath(), frm.m_strOutsideCMO, frm.m_dicInsideCMO);
        }

        private void FormMain2_Resize(object sender, EventArgs e)
        {
            int left, top, right, bottom;

            axDockingPane.GetClientRect(out left, out top, out right, out bottom);
            panelVirtool.SetBounds(left, top, right - left, bottom - top);
        }

        private void tabCtrlMonitoring_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tabDisaster.Controls.Count > 0)
                tabDisaster.Controls.Remove(panelMain);
            if (tabEquipment.Controls.Count > 0)
                tabEquipment.Controls.Remove(panelMain);
            if (tabSensor.Controls.Count > 0)
                tabSensor.Controls.Remove(panelMain);
            if (tabCCTV.Controls.Count > 0)
                tabCCTV.Controls.Remove(panelMain);

            switch (tabCtrlMonitoring.SelectedIndex)
            {
                case 0:
                    tabDisaster.Controls.Add(panelMain);
                    break;
                case 1:
                    tabEquipment.Controls.Add(panelMain);
                    break;
                case 2:
                    tabSensor.Controls.Add(panelMain);
                    break;
                case 3:
                    tabCCTV.Controls.Add(panelMain);
                    break;
            }
        }

        public void CreatePane()
        {
            // Bottom
            Pane paneLog = axDockingPane.CreatePane(1, 300, 170, DockingDirection.DockBottomOf, null);
            paneLog.Title = "SOP Log";
            paneLog.Options = PaneOptions.PaneNoCloseable;

            // Left
            Pane paneSpace = axDockingPane.CreatePane(0, 250, 70, DockingDirection.DockLeftOf, null);
            paneSpace.Title = "공간구조";
            paneSpace.Options = PaneOptions.PaneNoCloseable;

            //Right
            Pane paneSituation = axDockingPane.CreatePane(2, 250, 120, DockingDirection.DockRightOf, null);
            paneSituation.Title = "상황";
            paneSituation.Options = PaneOptions.PaneNoCloseable;

            arrDocking[0] = new FormLeftSpace(this);
            m_frmSpace = (FormLeftSpace)arrDocking[0];

            arrDocking[1] = new FormBottomLog();
            m_frmLog = (FormBottomLog)arrDocking[1];

            arrDocking[2] = new FormRightSituation(this);
            m_frmSituation = (FormRightSituation)arrDocking[2];
        }

        private void axDockingPane_AttachPaneEvent(object sender, AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEvent e)
        {
            int nIndex = e.item.Id;

            if (nIndex == 0)
                e.item.Handle = arrDocking[0].Handle.ToInt32();
            else if (nIndex == 1)
                e.item.Handle = arrDocking[1].Handle.ToInt32();
            else if (nIndex == 2)
                e.item.Handle = arrDocking[2].Handle.ToInt32();
        }

        private void axDockingPane_ResizeEvent(object sender, EventArgs e)
        {
            int left, top, right, bottom;

            axDockingPane.GetClientRect(out left, out top, out right, out bottom);
            panelVirtool.SetBounds(left, top, right - left, bottom - top);
        }

        private void tsViewCtrl_ImageLoad()
        {
            Bitmap bmpViewCtrl = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.toolbar_ViewControl);
            ImageList ListViewCtrl = new ImageList();
            ListViewCtrl.ImageSize = new Size(24, 24);
            ListViewCtrl.Images.AddStrip(bmpViewCtrl);

            tsViewCtrl.ImageList = ListViewCtrl;

            tsbtnHomeView.ImageIndex = 0;
            tsbtnFullScreen.ImageIndex = 1;
            tsbtnZoomin.ImageIndex = 2;
            tsbtnZoomout.ImageIndex = 3;
            tsbtnMove.ImageIndex = 4;
            tsbtnPick.ImageIndex = 5;
            tsbtnOrbit.ImageIndex = 6;
            tsbtnLayout1.ImageIndex = 7;
            tsbtnLayout2.ImageIndex = 8;
            tsbtnLayout3.ImageIndex = 9;
            tsbtnLayout4.ImageIndex = 10;
        }

        private void FormLayoutLoad()
        {
            m_Layout = new FormLayout(this);
            m_Layout.TopLevel = false;
            m_Layout.Parent = this;
            splitContainer.Panel1.Controls.Add(m_Layout);
            m_Layout.Dock = DockStyle.Fill;
            m_Layout.Show();
        }

        public SOPMonitoringSystem.FormMain GetMain()
        {
            return m_frmMain;
        }

        public FormLeftSpace GetSpace()
        {
            return m_frmSpace;
        }

        public FormBottomLog GetLog()
        { 
            return m_frmLog;
        }
        
        public FormRightSummary GetSummary()
        {
            return m_frmSummary;
        }
        
        public FormRightSituation GetSituation()
        {
            return m_frmSituation;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            GetSituation().GridViewClearSelection();
            this.WindowState = FormWindowState.Maximized;
            this.Text += " " + m_frmMain.GetAppVersion();
        }

        private void tsbtnLayout1_Click(object sender, EventArgs e)
        {
            LayoutView(1);
        }

        private void tsbtnLayout2_Click(object sender, EventArgs e)
        {
            LayoutView(2);
        }

        private void tsbtnLayout3_Click(object sender, EventArgs e)
        {
            LayoutView(3);
        }

        private void tsbtnLayout4_Click(object sender, EventArgs e)
        {
            LayoutView(4);
        }

        public void LayoutView(int nLayout)
        {
            HideLayout();
            switch (nLayout)
            {
                case 1:
                    m_Layout.SetLayoutMode(1);
                    m_Layout.Layout1();                    
                    break;
                case 2:
                    m_Layout.SetLayoutMode(2);
                    m_Layout.Layout2();
                    break;
                case 3:
                    m_Layout.SetLayoutMode(3);
                    m_Layout.Layout3();
                    break;
                case 4:
                    m_Layout.SetLayoutMode(4);
                    m_Layout.Layout4();
                    break;
            }
         }

        private void HideLayout()
        {
            m_Layout.LayoutHide();
        }

        public Panel GetPaneVirtool()
        {
            return splitContainer.Panel1;
        }

        private void tsbtnLeft_Click(object sender, EventArgs e)
        {
            if (GetSpace().SelectSpace == null) return;

            string [] str = GetSpace().SelectSpace.Split('\\');

            int nMinFloor = 0;

            foreach (SOPMonitoringSystem.Data_BuildingGroup datagroup in m_arrGroup)
            {
                if(datagroup.GroupName == str[0])
                {
                    foreach (SOPMonitoringSystem.Data_Building databuilding in m_arrBuilding)
                    {
                        if(databuilding.BuildingName == str[1])
                        {
                            nMinFloor = databuilding.MinFloor;
                            break;
                        }
                    }
                }
            }

            int nTemp = 0;
            int nIndex = tsbtnFloor1.Text.IndexOf('B');
            string strTemp = Regex.Replace(tsbtnFloor1.Text, @"\D", "");
            if (nIndex < 0)
            {
                nTemp = int.Parse(strTemp) - 1;
            }
            else
            {
                nTemp = int.Parse(strTemp) * -1;
            }

            if (nMinFloor == nTemp) return;

            ChangeFloor(nTemp);
        }

        private void tsbtnRight_Click(object sender, EventArgs e)
        {
            if (GetSpace().SelectSpace == null) return;

            string[] str = GetSpace().SelectSpace.Split('\\');

            int nMaxFloor = 0;
            foreach (SOPMonitoringSystem.Data_BuildingGroup datagroup in m_arrGroup)
            {
                if (datagroup.GroupName == str[0])
                {
                    foreach (SOPMonitoringSystem.Data_Building databuilding in m_arrBuilding)
                    {
                        if (databuilding.BuildingName == str[1])
                        {
                            nMaxFloor = databuilding.MaxFloor;
                            break;
                        }
                    }
                }
            }

            int nTemp = 0;
            int nIndex = tsbtnFloor5.Text.IndexOf('B');
            string strTemp = Regex.Replace(tsbtnFloor5.Text, @"\D", "");
            if (nIndex < 0)
            {
                nTemp = int.Parse(strTemp) + 1;
            }
            else
            {
                nTemp = ((int.Parse(strTemp)-1 ) * -1) + 1;
            }

            if (nMaxFloor + 1 == nTemp) return;

            ChangeFloor(nTemp - 4);
        }

        public void ChangeFloor(int nFloorNumber)
        {
            if (nFloorNumber > 0)
            {
                tsbtnFloor1.Text = nFloorNumber.ToString() + "F";
                tsbtnFloor2.Text = (nFloorNumber + 1).ToString() + "F";
                tsbtnFloor3.Text = (nFloorNumber + 2).ToString() + "F";
                tsbtnFloor4.Text = (nFloorNumber + 3).ToString() + "F";
                tsbtnFloor5.Text = (nFloorNumber + 4).ToString() + "F";
            }
            else
            {
                string[] strFloor = new string[5];

                for (int i = 0; i < 5; i++)
                {
                    if (nFloorNumber + i > 0)
                        strFloor[i] = (nFloorNumber + i).ToString() + "F";
                    else if (nFloorNumber + i == 0)
                        strFloor[i] = "B" + (nFloorNumber + i + 1).ToString() + "F";
                    else
                        strFloor[i] = "B" + ((nFloorNumber + i - 1) * -1).ToString() + "F";
                }

                tsbtnFloor1.Text = strFloor[0];
                tsbtnFloor2.Text = strFloor[1];
                tsbtnFloor3.Text = strFloor[2];
                tsbtnFloor4.Text = strFloor[3];
                tsbtnFloor5.Text = strFloor[4];
            }
        }

        private void GetBuildingInfo(ref ArrayList arrInfo)
        {
            SOPMonitoringSystem.WebDBManager dbMgr = GetMain().GetDBManager();

            string strSQL = "SELECT *  FROM View_Equipment";

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            for (int i = 0; i < arrResult.Count - 10; i += 11)
            {
                SOPMonitoringSystem.Data_EquipmentInfo dataNew = new SOPMonitoringSystem.Data_EquipmentInfo();

                dataNew.EquipID = dbMgr.GetStringField(arrResult[i], "");
                dataNew.ZoneID = dbMgr.GetIntField(arrResult[i + 1].ToString(), 0);
                dataNew.ZoneName = dbMgr.GetStringField(arrResult[i + 2], "");
                dataNew.FloorIndex = dbMgr.GetIntField(arrResult[i + 3].ToString(), 0);
                dataNew.BuildingID = dbMgr.GetIntField(arrResult[i + 4].ToString(), 0);
                dataNew.BuildingName = dbMgr.GetStringField(arrResult[i + 5], "");
                dataNew.GroupID = dbMgr.GetIntField(arrResult[i + 6].ToString(), 0);
                dataNew.GroupName = dbMgr.GetStringField(arrResult[i + 7], "");
                dataNew.SiteName = dbMgr.GetStringField(arrResult[i + 8], "");
                dataNew.MaxFloor = dbMgr.GetIntField(arrResult[i + 9].ToString(), 0);
                dataNew.MinFloor = dbMgr.GetIntField(arrResult[i + 10].ToString(), 0);

                arrInfo.Add(dataNew);
            }
        }

        public ArrayList GetBuildingList()
        {
            return m_arrBuildingInfo;
        }

        private void btnFloorClick(ToolStripButton tsbtnFloor)
        {
            string strFloor = tsbtnFloor.Text;
            int nIndex = strFloor.IndexOf('F');

            if (nIndex >= 0)
                strFloor = strFloor.Substring(0, nIndex);

            try
            {
                int nFloorIndex = int.Parse(strFloor);
                m_Layout.ShowIndoor(nFloorIndex);
            }
            catch (Exception)
            {
            }
        }

        private void tsbtnFloor1_Click(object sender, EventArgs e)
        {
            btnFloorClick(tsbtnFloor1);
        }

        private void tsbtnFloor2_Click(object sender, EventArgs e)
        {
            btnFloorClick(tsbtnFloor2);
        }

        private void tsbtnFloor3_Click(object sender, EventArgs e)
        {
            btnFloorClick(tsbtnFloor3);
        }

        private void tsbtnFloor4_Click(object sender, EventArgs e)
        {
            btnFloorClick(tsbtnFloor4);
        }

        private void tsbtnFloor5_Click(object sender, EventArgs e)
        {

        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_frmMain.GetProcess().StopTimer();
            m_frmMain.GetProcess().StopThread();
            Application.Exit();
        }

        private void tsbtnZoomin_MouseDown(object sender, MouseEventArgs e)
        {
            m_isDown = true;
        }

        private void tsbtnZoomin_MouseUp(object sender, MouseEventArgs e)
        {
            m_isDown = false;
        }

        private void tsbtnZoomout_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void tsbtnZoomin_MouseHover(object sender, EventArgs e)
        {
            if (m_isDown)
            {
                MessageBox.Show("test1");
            }
        }

        private void tsbtnZoomout_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void tsbtnZoomout_MouseHover(object sender, EventArgs e)
        {

        }

        private void tsbtnZoomin_Click(object sender, EventArgs e)
        {
            m_Layout.ZoomIn();
        }

        private void tsbtnZoomout_Click(object sender, EventArgs e)
        {
            m_Layout.ZoomOut();
        }

        private void tsbtnHomeView_Click(object sender, EventArgs e)
        {
            VirtoolsViewer.Player player = m_Layout.GetCurrentPlayer();
            if (player == null)
                return;

            player.SendMessage("Level", "HomeView");
        }

        private void tsbtnFullScreen_Click(object sender, EventArgs e)
        {
            VirtoolsViewer.Player player = m_Layout.GetCurrentPlayer();
            if (player == null)
                return;

            player.SendMessage("Level", "AllView");
        }

        private void tsbtnAutoNavi_Click(object sender, EventArgs e)
        {
            VirtoolsViewer.Player player = m_Layout.GetCurrentPlayer();
            if (player == null)
                return;

            m_Layout.AutoNavigation(player);
        }

        /*private void tsbtnAutoNavi_MouseUp(object sender, MouseEventArgs e)
        {

        }*/
    }
}
