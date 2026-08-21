using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.GUI;
using SDMS;
using DBUtility2;
using System.Collections;
using System.Drawing;
using System.IO;
using UnE.Util.Unity;

namespace libExternalUI
{
    public class UIManager : IUIManager
    {
        private class Zone
        {
            private int m_nID = -1;
            private int m_nFloorIndex = 0;
            private string m_sceneName = "";
            private string m_strZoneName = "";

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public int FloorIndex
            {
                get { return m_nFloorIndex; }
                set { m_nFloorIndex = value; }
            }

            public string SceneName
            {
                get { return m_sceneName; }
                set { m_sceneName = value; }
            }

            public string ZoneName
            {
                get { return m_strZoneName; }
                set { m_strZoneName = value; }
            }
        }

        private ImageButton m_btnOutdoor = null;
        private ImageButton m_btnIndoor = null;
        private ImageButton m_btnWorkStatus = null;
        private FormMain m_frmMain = null;

        private Image m_imgOutdoorNormal = null;
        private Image m_imgIndoorNormal = null;
        private Image m_imgWorkStatusNormal = null;

        // Key : Floor Index
        private Dictionary<int, Zone> m_dicZoneScene = new Dictionary<int, Zone>();
        // Key : Scene Name
        private Dictionary<string, Zone> m_dicZoneSceneName = new Dictionary<string, Zone>();
        // Key : BuildingGroup ID
        // Value : Scene Name
        private Dictionary<int, string> m_dicBuildingGroupScene = new Dictionary<int, string>();
        private string m_strCurrentSceneName = "";

        private Control m_leftPanel = null;
        private const int m_nButtonHeight = 36;
        private const int m_nButtonWidth = 86;

        private FormWorkStatus m_frmWorkStatus = null;

        public static bool m_bCheckedbtnWorkStatus = false;

        private string m_strAccessSetPath = "AccessSet.txt";
        private string m_strAccessSetTempPath = "AccessSet_Temp.txt";

        private static UIManager m_instance = null;
        private int m_nResizeCount = 0;
        private int m_nParentWidth = 0;

        private Timer m_timer = null;
        private Panel4Unity m_unityPanel = null;

        public static UIManager Instance
        {
            get { return m_instance; }
        }

        public UIManager(Control parentCtrl)
        {
            int nButtonCount = 0;
            m_instance = this;

            Control btn = FindButton(parentCtrl, out nButtonCount);

            if (btn != null)
            {
                m_btnOutdoor = CreateOutdoorButton(btn.Parent.Parent, m_nButtonWidth, m_nButtonHeight, nButtonCount * m_nButtonHeight, btn.TabIndex + 1);
                m_btnIndoor = CreateIndoorButton(btn.Parent.Parent, m_nButtonWidth, m_nButtonHeight, (nButtonCount + 1) * m_nButtonHeight, m_btnOutdoor.TabIndex + 1);
                m_btnWorkStatus = CreateWorkStatusButton(btn.Parent.Parent, m_nButtonWidth, m_nButtonHeight, (nButtonCount + 2) * m_nButtonHeight, m_btnIndoor.TabIndex + 1);

                if (parentCtrl is FormMain)
                {
                    m_frmMain = (FormMain)parentCtrl;
                    ReadBuildingGroupScene();
                    ReadZoneScene();
                }
            }

            Control parent = (Control)m_frmMain.PageHome.ContentForm;
            m_unityPanel = (Panel4Unity)m_frmMain.PageHome.ContentForm.OutdoorView;

            m_frmWorkStatus = new FormWorkStatus();
            m_frmWorkStatus.TopLevel = false;
            //m_frmWorkStatus.Location = new System.Drawing.Point(parent.Width - m_frmWorkStatus.Size.Width, 0);
            parent.Controls.Add(m_frmWorkStatus);

            LoadSetting();

            m_timer = new Timer();
            m_timer.Interval = 500;
            m_timer.Tick += OnTimer;
            m_timer.Start();
        }

        private ImageButton CreateOutdoorButton(Control parent, int nButtonWidth, int nButtonHeight, int y, int nTabIndex)
        {
            ImageButton btn = new ImageButton();

            btn.BackColor = System.Drawing.Color.Transparent;
            btn.ButtonText = "";
            btn.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            btn.ImageClicked = global::libExternalUI.Properties.Resource.Outdoor_Click;
            btn.ImageDisabled = null;
            btn.ImageMouseOver = global::libExternalUI.Properties.Resource.Outdoor_Over;
            btn.ImageNormal = global::libExternalUI.Properties.Resource.Outdoor_Normal;
            btn.Location = new System.Drawing.Point(0, y);
            btn.Name = "btnOutdoor";
            btn.Owner = null;
            btn.Size = new System.Drawing.Size(nButtonWidth, nButtonHeight);
            btn.TabIndex = nTabIndex;
            btn.TabStop = false;
            btn.TextColor = System.Drawing.Color.Black;
            btn.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            btn.ToolTipText = "";
            btn.UseToolTip = false;
            btn.Visible = false;
            btn.WindowRateWidth = 1F;
            btn.Click += new System.EventHandler(this.OnClickOutdoor);

            // 외부화면이 선택된 상태가 된다.
            m_imgOutdoorNormal = btn.ImageNormal;
            btn.ImageNormal = btn.ImageClicked;

            parent.Controls.Add(btn);
            btn.Show();
            return btn;
        }

        private ImageButton CreateIndoorButton(Control parent, int nButtonWidth, int nButtonHeight, int y, int nTabIndex)
        {
            ImageButton btn = new ImageButton();

            btn.BackColor = System.Drawing.Color.Transparent;
            btn.ButtonText = "";
            btn.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            btn.ImageClicked = global::libExternalUI.Properties.Resource.Indoor_Click;
            btn.ImageDisabled = null;
            btn.ImageMouseOver = global::libExternalUI.Properties.Resource.Indoor_Over;
            btn.ImageNormal = global::libExternalUI.Properties.Resource.Indoor_Normal;
            btn.Location = new System.Drawing.Point(0, y);
            btn.Name = "btnIndoor";
            btn.Owner = null;
            btn.Size = new System.Drawing.Size(nButtonWidth, nButtonHeight);
            btn.TabIndex = nTabIndex;
            btn.TabStop = false;
            btn.TextColor = System.Drawing.Color.Black;
            btn.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            btn.ToolTipText = "";
            btn.UseToolTip = false;
            btn.Visible = false;
            btn.WindowRateWidth = 1F;
            btn.Click += new System.EventHandler(this.OnClickIndoor);

            m_imgIndoorNormal = btn.ImageNormal;

            parent.Controls.Add(btn);
            btn.Show();
            return btn;
        }

        private ImageButton CreateWorkStatusButton(Control parent, int nButtonWidth, int nButtonHeight, int y, int nTabIndex)
        {
            ImageButton btn = new ImageButton();

            btn.BackColor = System.Drawing.Color.Transparent;
            btn.ButtonText = "";
            btn.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            btn.ImageClicked = global::libExternalUI.Properties.Resources.WorkStatus_Click;
            btn.ImageDisabled = null;
            btn.ImageMouseOver = global::libExternalUI.Properties.Resources.WorkStatus_Over;
            btn.ImageNormal = global::libExternalUI.Properties.Resources.WorkStatus_Normal;
            btn.Location = new System.Drawing.Point(0, y);
            btn.Name = "btnWorkStatus";
            btn.Owner = null;
            btn.Size = new System.Drawing.Size(nButtonWidth, nButtonHeight);
            btn.TabIndex = nTabIndex;
            btn.TabStop = false;
            btn.TextColor = System.Drawing.Color.Black;
            btn.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            btn.ToolTipText = "";
            btn.UseToolTip = false;
            btn.Visible = false;
            btn.WindowRateWidth = 1F;
            btn.Click += new System.EventHandler(this.OnClickWorkStatus);

            m_imgWorkStatusNormal = btn.ImageNormal;

            parent.Controls.Add(btn);
            btn.Show();
            return btn;
        }

        private void OnClickOutdoor(object sender, EventArgs e)
        {
            if (m_dicBuildingGroupScene != null)
            {
                string strSceneName;

                if (m_dicBuildingGroupScene.TryGetValue(1, out strSceneName))
                {
                    m_unityPanel.SetSceneTitle("");

                    TooltipCCTVCtrl2.CloseAll();
                    m_frmMain.PageHome.ContentForm.OutdoorView.ClearPOI("");

                    m_frmMain.PageHome.ContentForm.HideAllAlarmZones();
                    m_frmMain.PageHome.ContentForm.SelectScene(strSceneName);
                    FormFloors.RemoveSelection();

                    m_btnIndoor.ImageNormal = m_imgIndoorNormal;
                    m_btnOutdoor.ImageNormal = m_btnOutdoor.ImageClicked;
                    m_btnIndoor.Refresh();
                    m_btnOutdoor.Refresh();
                }
            }
        }

        private void OnClickIndoor(object sender, EventArgs e)
        {
            if (m_dicZoneScene != null && m_frmMain != null && m_frmMain.PageHome != null && m_frmMain.PageHome.ContentForm != null)
            {
                Control parent = (Control)m_frmMain.PageHome.ContentForm;

                FormFloors frm = new FormFloors(m_btnIndoor, this);
                frm.TopLevel = false;
                frm.StartPosition = FormStartPosition.Manual;
                frm.Location = new Point(0, m_btnIndoor.Location.Y);
                parent.Controls.Add(frm);

                frm.BringToFront();
                frm.Show();

                m_btnOutdoor.ImageNormal = m_imgOutdoorNormal;
                m_btnIndoor.ImageNormal = m_btnIndoor.ImageClicked;
                m_btnIndoor.Refresh();
                m_btnOutdoor.Refresh();
            }
        }

        private void OnClickWorkStatus(object sender, EventArgs e)
        {
            if (m_bCheckedbtnWorkStatus == false)
            {
                Control parent = (Control)m_frmMain.PageHome.ContentForm;

                //if (m_frmWorkStatus.CheckAccessFloor() == true)
                //{
                    m_frmWorkStatus.BringToFront();
                    m_frmWorkStatus.Show();

                    m_bCheckedbtnWorkStatus = true;
                    m_btnWorkStatus.ImageNormal = m_btnWorkStatus.ImageClicked;
                    m_btnWorkStatus.Refresh();

                    SaveCheckStatus();
                //}
                //else
                //{
                //    MessageBox.Show("현재 출입한 인원이 없습니다.", "출입인원", MessageBoxButtons.OK);
                //}
            }
            else
            {
                m_frmWorkStatus.Hide();
                m_bCheckedbtnWorkStatus = false;
                m_btnWorkStatus.ImageNormal = m_imgWorkStatusNormal;
                m_btnWorkStatus.Refresh();

                SaveCheckStatus();
            }
        }

        private Control FindButton(Control parentCtrl, out int nButtonCount)
        {
            nButtonCount = 0;
            Dictionary<int, int> dicPos = new Dictionary<int, int>();

            foreach (Control ctrl in parentCtrl.Controls)
            {
                if (ctrl.Name == "panelLeft2")
                {
                    Control panel = FindButton(ctrl, "panelLeftItem");

                    if (panel != null)
                    {
                        m_leftPanel = panel;

                        int nPos = 0;
                        Control lastButton = null;

                        foreach (Control btn in panel.Controls)
                        {
                            if (btn is ImageButton)
                            {
                                nButtonCount++;
                                dicPos[btn.Location.Y] = btn.Location.Y;

                                if (lastButton == null)
                                {
                                    lastButton = btn;
                                    nPos = btn.Location.Y;
                                }
                                else if (nPos < btn.Location.Y)
                                {
                                    lastButton = btn;
                                    nPos = btn.Location.Y;
                                }
                            }
                        }

                        return lastButton;
                        /*Control btn = FindButton(panel, "btnFullScreen");
                        return btn;*/
                    }

                    break;
                }
            }

            return null;
        }

        private Control FindButton(Control parentCtrl, string strName)
        {
            foreach (Control ctrl in parentCtrl.Controls)
            {
                if (ctrl.Name == strName)
                {
                    return ctrl;
                }
            }

            return null;
        }

        private void ReadBuildingGroupScene()
        {
            string strSQL = "Select bg.ID, bgs.SceneName from BuildingGroup as bg, BuildingGroupScene as bgs where bg.ID = bgs.BuildingGroupID";
            ArrayList arrResult = m_frmMain.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> buildingGroupID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strSceneName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (buildingGroupID == null || strSceneName == null)
                    continue;

                m_dicBuildingGroupScene[buildingGroupID.Data] = strSceneName;
            }
        }

        private void ReadZoneScene()
        {
            string strSQL = "Select Zone.ID, zs.SceneName, Zone.FloorIndex, ZoneName from Zone, ZoneScene as zs where Zone.ID = zs.ZoneID";
            ArrayList arrResult = m_frmMain.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                VariousData<int> zoneID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strSceneName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> floorIndex = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 3]);

                if (zoneID == null || strSceneName == null || floorIndex == null || strZoneName == null)
                    continue;

                Zone zone = new Zone();
                zone.ID = zoneID.Data;
                zone.SceneName = strSceneName;
                zone.FloorIndex = floorIndex.Data;
                zone.ZoneName = strZoneName;

                m_dicZoneScene[floorIndex.Data] = zone;
                m_dicZoneSceneName[strSceneName] = zone;
            }
        }

        public void OnFloorClick(int nFloorIndex)
        {
            Zone zone;

            if (m_dicZoneScene.TryGetValue(nFloorIndex, out zone))
            {
                TooltipCCTVCtrl2.CloseAll();
                m_frmMain.PageHome.ContentForm.HideAllAlarmZones();
                m_frmMain.PageHome.ContentForm.SelectScene(zone.SceneName);

                int nLayerState = m_frmMain.ReadLayerState();
                bool showCCTV = (nLayerState & (int)LAYER_TYPE.CCTV) == (int)LAYER_TYPE.CCTV;

                //LoadCCTVFile(m_frmMain.PageHome.ContentForm.IndoorView, true, zone.ID, showCCTV);
                LoadCCTVFile(m_frmMain.PageHome.ContentForm.OutdoorView, true, zone.ID, showCCTV);
            }
        }


        private void LoadCCTVFile(UnE.Sensor.ISensorTooltipOwner owner, bool isIndoor, int nZoneID, bool visible)
        {
            if (owner != null)
            {
                // 화면상의 모든 POI들을 제거한다.
                owner.ClearPOI("");

                if (!SDMS.CCTVManager.Instance.LoadCCTVFile(owner, isIndoor, nZoneID, visible))
                    return;
            }
        }

        public void ShowControl(object arg)
        {
            if (arg != null && arg is string)
            {
                string strMode = (string)arg;

                if (strMode == "Monitoring" || strMode == "Admin")
                {
                    m_btnOutdoor.Show();
                    m_btnIndoor.Show();
                    m_btnWorkStatus.Show();
                }
                else
                {
                    m_btnOutdoor.Hide();
                    m_btnIndoor.Hide();
                    m_btnWorkStatus.Hide();
                }
            }
        }

        public void HideControl(object arg)
        {

        }

        public void OnResize()
        {
            if (m_leftPanel != null)
            {
                int nVisibleCount = 0;

                foreach (Control ctrl in m_leftPanel.Controls)
                {
                    if (ctrl == m_btnIndoor || ctrl == m_btnOutdoor)
                        continue;

                    if (ctrl.Visible)
                        nVisibleCount++;
                }

                Relocate(m_nButtonHeight * nVisibleCount, m_btnOutdoor);
                Relocate(m_nButtonHeight * (nVisibleCount + 1), m_btnIndoor);
                Relocate(m_nButtonHeight * (nVisibleCount + 2), m_btnWorkStatus);
            }

            // 초기 위치 잡기 전까지 4번 호출
            if (m_frmWorkStatus != null && m_nResizeCount < 4)
            {
                Control parent = (Control)m_frmMain.PageHome.ContentForm;
                m_frmWorkStatus.Location = new System.Drawing.Point(parent.Width - m_frmWorkStatus.Size.Width, 0);
                m_nResizeCount++;
                m_nParentWidth = parent.Size.Width;
            }
            else
            {
                Control parent = (Control)m_frmMain.PageHome.ContentForm;

                int nSub = m_nParentWidth - parent.Size.Width;
                m_nParentWidth = parent.Size.Width;

                m_frmWorkStatus.Location = new Point(m_frmWorkStatus.Location.X - nSub, m_frmWorkStatus.Location.Y);

            }
        }

        private void Relocate(int y, ImageButton btn)
        {
            if (btn.Location.Y != y)
                btn.Location = new Point(btn.Location.X, y);
        }

        public Control GetPageBackstageHome()
        {
            if (m_frmMain == null)
                return null;

            foreach (Control ctrl in m_frmMain.Controls)
            {
                //System.Diagnostics.Trace.WriteLine(ctrl.Name);
                if (ctrl.Name == "panelBottom")
                {
                    foreach (Control ctrl2 in ctrl.Controls)
                    {
                        if (ctrl2.Name == "PageBackstageHome")
                        {
                            return ctrl2;
                        }
                    }
                }
            }

            return null;
        }

        public void SetExternalFormStatus()
        {
            m_bCheckedbtnWorkStatus = false;
            m_btnWorkStatus.ImageNormal = m_imgWorkStatusNormal;
            m_btnWorkStatus.Refresh();

            SaveCheckStatus();
        }

        private void LoadSetting()
        {
            try
            {
                char sp = ':';
                string[] spStrings = null;

                if (!CheckAccessFile())
                {
                    NewSettingFile();
                    return;
                }

                if (File.Exists(m_strAccessSetPath))
                {
                    StreamReader reader = new StreamReader(m_strAccessSetPath, Encoding.Default);

                    while (reader.EndOfStream == false)
                    {
                        string strLine = reader.ReadLine();
                        spStrings = strLine.Split(sp);

                        string strTitle = spStrings[0].Trim();
                        string strValue = spStrings[1].Trim();

                        if (strTitle == "btnCheck")
                        {
                            if (strValue == "True")
                            {
                                //if (m_frmWorkStatus.CheckAccessFloor() == true)
                                //{
                                    m_frmWorkStatus.BringToFront();
                                    m_frmWorkStatus.Show();

                                    m_bCheckedbtnWorkStatus = true;
                                    m_btnWorkStatus.ImageNormal = m_btnWorkStatus.ImageClicked;
                                    m_btnWorkStatus.Refresh();
                                //}
                                //else
                                //{
                                //    m_bCheckedbtnWorkStatus = false;
                                //    SaveCheckStatus();
                                //}
                                
                            }
                        }
                    }

                    reader.Close();
                }
                else
                {
                    NewSettingFile();
                }
            }
            catch
            {

            }
        }

        private void NewSettingFile()
        {
            StreamWriter writer = new StreamWriter(m_strAccessSetPath, false, Encoding.Default);
            string strDate = "btnCheck : False";
            writer.WriteLine(strDate);
            strDate = "AccessDate : " + string.Format("{0}000000", DateTime.Now.ToString("yyyyMMdd"));
            writer.WriteLine(strDate);

            writer.Close();
        }

        private void SaveCheckStatus()
        {
            try
            {
                char sp = ':';
                string[] spStrings = null;

                StreamReader reader = new StreamReader(m_strAccessSetPath, Encoding.Default);
                StreamWriter writer = new StreamWriter(m_strAccessSetTempPath, false, Encoding.Default);

                while (reader.EndOfStream == false)
                {
                    string strLine = reader.ReadLine().Trim();
                    spStrings = strLine.Split(sp);

                    string strTitle = spStrings[0].Trim();
                    string strValue = spStrings[1].Trim();

                    if (strTitle == "btnCheck")
                    {
                        strLine = string.Format("btnCheck : {0}", m_bCheckedbtnWorkStatus);
                    }

                    writer.WriteLine(strLine);
                }

                reader.Close();
                writer.Close();

                FileInfo file = new FileInfo(m_strAccessSetTempPath);

                if (file.Exists)
                {
                    file.CopyTo(m_strAccessSetPath, true);
                    file.Delete();
                }
            }
            catch
            {

            }
        }

        private bool CheckAccessFile()
        {
            bool bRet = true;

            if (File.Exists(m_strAccessSetPath))
            {
                try
                {
                    char sp = ':';
                    string[] spStrings = null;
                    bool bBtnCheck = false;
                    bool bAccessDate = false;

                    StreamReader reader = new StreamReader(m_strAccessSetPath, Encoding.Default);

                    while (reader.EndOfStream == false)
                    {
                        string strLine = reader.ReadLine().Trim();
                        spStrings = strLine.Split(sp);

                        string strTitle = spStrings[0].Trim();
                        string strValue = spStrings[1].Trim();

                        if (strTitle == "btnCheck")
                        {
                            bBtnCheck = true;
                        }

                        if (strTitle == "AccessDate")
                        {
                            bAccessDate = true;
                        }

                    }

                    reader.Close();

                    if (bBtnCheck != true || bAccessDate != true)
                    {
                        FileInfo file = new FileInfo(m_strAccessSetPath);
                        file.Delete();
                        bRet = false;
                    }
                }
                catch
                {

                }
            }

            return bRet;
        }

        public string GetWebServerURL()
        {
            return m_frmMain.DBManager.WebServerURL;
        }

        private void OnTimer(object sender, EventArgs e)
        {
            if (m_strCurrentSceneName != m_unityPanel.CurrentSceneName)
            {
                m_strCurrentSceneName = m_unityPanel.CurrentSceneName;
                Zone zone;

                if (m_dicZoneSceneName.TryGetValue(m_strCurrentSceneName, out zone))
                {
                    m_unityPanel.SetSceneTitle(zone.ZoneName);
                }
                else
                    m_unityPanel.SetSceneTitle("");
            }
        }
    }
}
