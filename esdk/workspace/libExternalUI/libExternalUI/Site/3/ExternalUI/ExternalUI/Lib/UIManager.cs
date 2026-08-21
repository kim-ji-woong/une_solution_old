using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libExternalUI;
using System.Windows.Forms;
using DBUtility2;
using UnE.GUI;
using System.Drawing;
using System.IO;
using SDMS;

namespace libExternalUI.Lib
{
    public class UIManager : IUIManager
    {
        private Control m_parentCtrl = null;
        private FormEarthquakeDetection m_frmEarthquakeDetection = null;
        private FormWorkStatus m_frmWorkStatus = null;
        private FormAirQuality m_frmAirQuality = null;


        private ImageButton m_btnEarthquakeDetection = null;
        private ImageButton m_btnWorkStatus = null;
        private ImageButton m_btnAirQuality = null;

        public static bool bCheckedbtnEarthquakeDetection = false;
        public static bool bCheckedbtnAirQuality = false;
        public static bool bCheckedbtnWorkStatus = false;

        private Color m_OrangeColor = Color.FromArgb(0xF7, 0xA9, 0x2B);

        private string m_strFilePath = String.Format(@"CheckButton.txt");

        private Timer m_timer = null;

        private WebDBManager m_dbMgr = null;

        #region AlarmZoneGroupSet_Data
        private ComboBox m_cmbFireDetect = null;
        private RealTimeInfoPane m_panelLog = null;
        private libSensorProcess.ProcessIF m_prevProcess = null;
        // Key : ZoneID
        // Value : GroupSetName
        private Dictionary<int, string> m_dicZoneGroupSet = new Dictionary<int, string>();
        #endregion

        public WebDBManager DBMgr
        {
            get { return m_dbMgr; }
        }
        
        private static UIManager m_instance = null;
        public static UIManager Instance
        {
            get { return m_instance; }
        }

        public UIManager(Control parent)
        {
            m_instance = this;
            m_parentCtrl = parent;

            Control panel = FindToolbarPanel(parent.Parent.Parent);

            if (panel != null)
            {
                m_btnEarthquakeDetection = CreateEarthquakeDetectionButton(panel.Parent);
                m_btnWorkStatus = CreateWorkStatusButton(panel.Parent);
                m_btnAirQuality = CreateAirQualityButton(panel.Parent);
            }

            SetAlarmGroupZone((FormMain)parent.Parent.Parent);

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

                    if (spStrings.Length == 3)
                    {
                        if (spStrings[0] == "True")
                        {
                            bCheckedbtnEarthquakeDetection = true;
                            SetButtonBackColor(m_btnEarthquakeDetection);
                        }

                        if (spStrings[1] == "True")
                        {
                            bCheckedbtnAirQuality = true;
                            SetButtonBackColor(m_btnAirQuality);
                        }

                        if (spStrings[2] == "True")
                        {
                            bCheckedbtnWorkStatus = true;
                            SetButtonBackColor(m_btnWorkStatus);
                        }
                    }
                }

                inputFile.Close();
            }
            catch
            {

            }


            m_frmEarthquakeDetection = new FormEarthquakeDetection();
            m_frmAirQuality = new FormAirQuality();
            m_frmWorkStatus = new FormWorkStatus();

            
            Init();
            Loadini();
            m_dbMgr = new WebDBManager(m_nSiteID);

            m_timer = new Timer();
            m_timer.Interval = 1000;
            m_timer.Tick += M_timer_Tick;
            m_timer.Start();
        }

        private int m_nSiteID = 3;

        private void Loadini()
        {
            DBUtility2.Utility util = new DBUtility2.Utility();
            string siteId = util.getinivalue("Server Connection Info", "siteid");
            if (siteId != null && siteId.Length > 0)
            {
                int.TryParse(siteId, out m_nSiteID);
            }
        }

        //private int m_nTimerCount = 0;

        private void M_timer_Tick(object sender, EventArgs e)
        {
            //if (m_nTimerCount++ % 5 == 0)
            {
                m_frmAirQuality.DisplayAirquaility();
                m_frmWorkStatus.DisplayWorkStatus();
                m_frmEarthquakeDetection.DisplayEarthquake();

                DateTime dtNow = DateTime.Now;
                if (dtNow.Hour == 23 && dtNow.Minute == 59 && dtNow.Second == 59)
                    m_frmWorkStatus.Init();
            }

            AlarmMonitoring();
        }

        private void Init()
        {
            int nParentWidth = m_parentCtrl.ClientSize.Width;
            int nParentHeight = m_parentCtrl.ClientSize.Height;

            int nFormWidth = nParentWidth / 3;
            int nFormHeight = nParentHeight / 3;

            m_frmEarthquakeDetection.Location = new System.Drawing.Point(nParentWidth - m_frmEarthquakeDetection.Size.Width, 0);
            //m_frmEarthquakeDetection.Size = new System.Drawing.Size(nFormWidth, nFormHeight);
            m_parentCtrl.Controls.Add(m_frmEarthquakeDetection);

            m_frmWorkStatus.Location = new System.Drawing.Point(nParentWidth - m_frmWorkStatus.Size.Width, nFormHeight);
            //m_frmAirQuality.Size = new System.Drawing.Size(nFormWidth, nFormHeight);
            m_parentCtrl.Controls.Add(m_frmWorkStatus);

            m_frmAirQuality.Location = new System.Drawing.Point(nParentWidth - m_frmAirQuality.Size.Width, nFormHeight * 2);
            //m_frmAirQuality.Size = new System.Drawing.Size(nFormWidth, nFormHeight);
            m_parentCtrl.Controls.Add(m_frmAirQuality);
        }

        public void ShowControl(object arg)
        {
            int nParentWidth = m_parentCtrl.ClientSize.Width;
            int nParentHeight = m_parentCtrl.ClientSize.Height;

            int nFormWidth = nParentWidth / 3;
            int nFormHeight = nParentHeight / 3;

            if (arg != null && arg is int)
            {
                int option = (int)arg;

                if (option == 1)
                {
                    m_frmEarthquakeDetection.Location = new System.Drawing.Point(nParentWidth - m_frmEarthquakeDetection.Size.Width, 0);
                    
                    m_frmEarthquakeDetection.Show();
                    m_frmEarthquakeDetection.BringToFront();
                }
                else if (option == 2)
                {
                    m_frmWorkStatus.Location = new System.Drawing.Point(nParentWidth - m_frmWorkStatus.Width, m_frmEarthquakeDetection.Height);

                    m_frmWorkStatus.Show();
                    m_frmWorkStatus.BringToFront();
                }
                else if (option == 3)
                {
                    m_frmAirQuality.Location = new System.Drawing.Point(nParentWidth - m_frmAirQuality.Width, m_frmEarthquakeDetection.Height + m_frmWorkStatus.Height);

                    m_frmAirQuality.Show();
                    m_frmAirQuality.BringToFront();
                }
                else if (option == 4)
                {
                    m_frmEarthquakeDetection.Show();
                    m_frmWorkStatus.Show();
                    m_frmAirQuality.Show();
                }
            }

            if (arg != null && arg is string)
            {
                string strMode = (string)arg;

                if (strMode == "Monitoring" || strMode == "Admin")
                {
                    m_btnEarthquakeDetection.Show();
                    m_btnWorkStatus.Show();
                    m_btnAirQuality.Show();

                    if (bCheckedbtnEarthquakeDetection == true)
                    {
                        ShowControl(1);
                    }
                    
                    if (bCheckedbtnWorkStatus == true)
                    {
                        ShowControl(2);
                    }

                    if (bCheckedbtnAirQuality == true)
                    {
                        ShowControl(3);
                    }
                }
                else
                {
                    m_btnEarthquakeDetection.Hide();
                    m_btnWorkStatus.Hide();
                    m_btnAirQuality.Hide();
                    m_frmEarthquakeDetection.Hide();
                    m_frmWorkStatus.Hide();
                    m_frmAirQuality.Hide();
                }
            }
        }

        public void HideControl(object arg)
        {
            if (arg != null && arg is int)
            {
                int option = (int)arg;

                if (option == 1)
                    m_frmEarthquakeDetection.Hide();
                else if (option == 2)
                    m_frmWorkStatus.Hide();
                else if (option == 3)
                    m_frmAirQuality.Hide();
                else if (option == 4)
                {
                    m_frmEarthquakeDetection.Hide();
                    m_frmWorkStatus.Hide();
                    m_frmAirQuality.Hide();
                }
            }

            /*if (arg != null && arg is string)
            {
                string str = (string)arg;

                if (str == "AppClose")
                    m_runThread = false;
            }*/
        }

        public void OnResize()
        {
            Control btn = FindToolbarMaxY(m_parentCtrl.Parent.Parent);

            if (btn != null)
            {
                m_btnEarthquakeDetection.Location = new Point(0, btn.Location.Y + btn.Size.Height);
                m_btnWorkStatus.Location = new Point(0, m_btnEarthquakeDetection.Location.Y + m_btnEarthquakeDetection.Size.Height);
                m_btnAirQuality.Location = new Point(0, m_btnWorkStatus.Location.Y + m_btnWorkStatus.Size.Height);

                m_btnEarthquakeDetection.TabIndex = btn.TabIndex + 1;
                m_btnWorkStatus.TabIndex = m_btnEarthquakeDetection.TabIndex + 1;
                m_btnAirQuality.TabIndex = m_btnAirQuality.TabIndex + 1;
            }

            int nParentWidth = m_parentCtrl.ClientSize.Width;
            int nParentHeight = m_parentCtrl.ClientSize.Height;
            int nFormHeight = nParentHeight / 3;

            m_frmEarthquakeDetection.Location = new System.Drawing.Point(nParentWidth - m_frmEarthquakeDetection.Size.Width, 0);
            m_frmWorkStatus.Location = new System.Drawing.Point(nParentWidth - m_frmWorkStatus.Width, m_frmEarthquakeDetection.Height);
            m_frmAirQuality.Location = new System.Drawing.Point(nParentWidth - m_frmAirQuality.Width, m_frmEarthquakeDetection.Height + m_frmWorkStatus.Height);
        }

        private Control FindButton(Control parentCtrl)
        {
            foreach (Control ctrl in parentCtrl.Controls)
            {
                if (ctrl.Name == "panelLeft2")
                {
                    Control panel = FindButton(ctrl, "panelLeftItem");

                    if (panel != null)
                    {
                        Control btn = FindButton(panel, "btnHome");
                        return btn;
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

        private Control FindComboBox(Control parentCtrl)
        {
            foreach (Control ctrl in parentCtrl.Controls)
            {
                if (ctrl.Name == "panelTop2")
                {
                    Control panel = FindControl(ctrl, "panelTop3DTabItemCtrl");

                    if (panel != null)
                    {
                        Control combo = FindControl(panel, "cmbFireDetect");
                        return combo;
                    }

                    break;
                }
            }

            return null;
        }

        private Control FindControl(Control parentCtrl, string strName)
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

        private Control FindToolbarPanel(Control parentCtrl)
        {
            Control retCtrl = null;

            foreach (Control ctrl in parentCtrl.Controls)
            {
                if (ctrl.Name == "panelLeft2")
                {
                    retCtrl = FindButton(ctrl, "panelLeftItem");
                }
            }

            return retCtrl;
        }

        private Control FindToolbarMaxY(Control parentCtrl)
        {
            Control retCtrl = null;
            Control panel = FindToolbarPanel(parentCtrl);

            foreach (Control item in panel.Controls)
            {
                if (item.Visible == true)
                {
                    if (retCtrl == null)
                        retCtrl = item;

                    if (item.Location.Y > retCtrl.Location.Y)
                        retCtrl = item;
                }
            }

            return retCtrl;
        }

        //private ImageButton CreateEarthquakeDetectionButton(Control parent, Control btnPrev)
        private ImageButton CreateEarthquakeDetectionButton(Control parent)
        {
            ImageButton btn = new ImageButton();

            btn.BackColor = System.Drawing.Color.Transparent;
            btn.ButtonText = "";
            btn.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            btn.ImageClicked = global::libExternalUI.Properties.Resources.EarthquakeDetection_Click;
            btn.ImageDisabled = null;
            btn.ImageMouseOver = global::libExternalUI.Properties.Resources.EarthquakeDetection_Click;
            btn.ImageNormal = global::libExternalUI.Properties.Resources.EarthquakeDetection_Default;
            btn.Location = new System.Drawing.Point(0, 324);
            btn.Name = "EarthquakeDetection";
            btn.Owner = null;
            btn.Size = new System.Drawing.Size(86, 36);
            //btn.TabIndex = btnPrev.TabIndex + 1;
            btn.TabStop = false;
            btn.TextColor = System.Drawing.Color.Black;
            btn.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            btn.ToolTipText = "";
            btn.UseToolTip = false;
            btn.Visible = false;
            btn.WindowRateWidth = 1F;
            btn.Click += new System.EventHandler(this.btnEarthquakeDetection_Click);
            

            parent.Controls.Add(btn);
            btn.Show();
            return btn;
        }

        private ImageButton CreateWorkStatusButton(Control parent)
        {
            ImageButton btn = new ImageButton();

            btn.BackColor = System.Drawing.Color.Transparent;
            btn.ButtonText = "";
            btn.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            btn.ImageClicked = global::libExternalUI.Properties.Resources.WorkStatus_Click;
            btn.ImageDisabled = null;
            btn.ImageMouseOver = global::libExternalUI.Properties.Resources.WorkStatus_Click;
            btn.ImageNormal = global::libExternalUI.Properties.Resources.WorkStatus_Default;
            btn.Location = new System.Drawing.Point(0, 360);
            btn.Name = "EarthquakeDetection";
            btn.Owner = null;
            btn.Size = new System.Drawing.Size(86, 36);
            //btn.TabIndex = btnPrev.TabIndex + 1;
            btn.TabStop = false;
            btn.TextColor = System.Drawing.Color.Black;
            btn.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            btn.ToolTipText = "";
            btn.UseToolTip = false;
            btn.Visible = false;
            btn.WindowRateWidth = 1F;
            btn.Click += new System.EventHandler(this.btnWorkStatus_Click);

            parent.Controls.Add(btn);
            btn.Show();
            return btn;
        }

        private ImageButton CreateAirQualityButton(Control parent)
        {
            ImageButton btn = new ImageButton();

            btn.BackColor = System.Drawing.Color.Transparent;
            btn.ButtonText = "";
            btn.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            btn.ImageClicked = global::libExternalUI.Properties.Resources.AirQuality_Click;
            btn.ImageDisabled = null;
            btn.ImageMouseOver = global::libExternalUI.Properties.Resources.AirQuality_Click;
            btn.ImageNormal = global::libExternalUI.Properties.Resources.AirQuality_Default;
            btn.Location = new System.Drawing.Point(0, 396);
            btn.Name = "EarthquakeDetection";
            btn.Owner = null;
            btn.Size = new System.Drawing.Size(86, 36);
            //btn.TabIndex = btnPrev.TabIndex + 1;
            btn.TabStop = false;
            btn.TextColor = System.Drawing.Color.Black;
            btn.TextFont = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            btn.ToolTipText = "";
            btn.UseToolTip = false;
            btn.Visible = false;
            btn.WindowRateWidth = 1F;
            btn.Click += new System.EventHandler(this.btnAirQuality_Click);

            parent.Controls.Add(btn);
            btn.Show();
            return btn;
        }

        private void btnEarthquakeDetection_Click(object sender, EventArgs e)
        {
            if (bCheckedbtnEarthquakeDetection)
                this.HideControl(1);
            else
                this.ShowControl(1);

            bCheckedbtnEarthquakeDetection = !bCheckedbtnEarthquakeDetection;

            SetButtonBackColor(m_btnEarthquakeDetection);
            SaveCheckFile();
        }

        private void btnWorkStatus_Click(object sender, EventArgs e)
        {
            if (bCheckedbtnWorkStatus)
                this.HideControl(2);   
            else
                this.ShowControl(2);

            bCheckedbtnWorkStatus = !bCheckedbtnWorkStatus;

            SetButtonBackColor(m_btnWorkStatus);
            SaveCheckFile();
        }

        private void btnAirQuality_Click(object sender, EventArgs e)
        {
            if (bCheckedbtnAirQuality)
                this.HideControl(3);
            else
                this.ShowControl(3);

            bCheckedbtnAirQuality = !bCheckedbtnAirQuality;

            SetButtonBackColor(m_btnAirQuality);
            SaveCheckFile();
        }

        private void SetButtonBackColor(ImageButton button)
        {
            Control panel = FindToolbarPanel(m_parentCtrl.Parent.Parent);

            if (panel.Parent.Contains(button))
            {
                foreach (Control item in panel.Parent.Controls)
                {
                    if (item == button)
                    {
                        if (button.BackColor == Color.Transparent)
                            button.BackColor = m_OrangeColor;
                        else
                            button.BackColor = Color.Transparent;
                    }

                }
            }
        }

        private void SaveCheckFile()
        {
            StreamWriter outputFile;
            outputFile = new StreamWriter(m_strFilePath, false, Encoding.Default);

            string strQurey = string.Format("{0},{1},{2}", bCheckedbtnEarthquakeDetection, bCheckedbtnAirQuality, bCheckedbtnWorkStatus);

            outputFile.WriteLine(strQurey);

            outputFile.Close();
        }

        public static void TransferExternalForm(object param)
        {
            UIManager.Instance.SetExternalFormStatus(param);
        }
        public void SetExternalFormStatus(object param)
        {

            if ((int)param == 1)
            {
                bCheckedbtnEarthquakeDetection = !bCheckedbtnEarthquakeDetection;
                SetButtonBackColor(m_btnEarthquakeDetection);
            }
            else if ((int)param == 2)
            {
                bCheckedbtnWorkStatus = !bCheckedbtnWorkStatus;
                SetButtonBackColor(m_btnWorkStatus);
            }
            else if ((int)param == 3)
            {
                bCheckedbtnAirQuality = !bCheckedbtnAirQuality;
                SetButtonBackColor(m_btnAirQuality);
            }

            SaveCheckFile();
        }

        #region AlarmZoneGroupSet
        private void SetAlarmGroupZone(FormMain frmMain)
        {
            Control cmb = FindComboBox(frmMain);

            if (cmb != null && cmb is ComboBox)
            {
                m_cmbFireDetect = (ComboBox)cmb;
            }

            Control panel = FindControl(frmMain, "panelLog");

            if (panel != null && panel is RealTimeInfoPane)
            {
                m_panelLog = (RealTimeInfoPane)panel;
            }

            if (m_cmbFireDetect != null && m_panelLog != null)
            {
                //System.Threading.Thread t = new System.Threading.Thread(AlarmMonitoringThread);
                //t.Start();
            }

            ReadAlarmZoneGroupSet(frmMain.DBManager);
        }

        private void ReadAlarmZoneGroupSet(WebDBManager dbMgr)
        {
            string strSQL = "Select ZoneID, GroupSetName from AlarmZoneGroupSet";
            System.Collections.ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> zoneID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strGroupSetName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (zoneID == null || strGroupSetName == null)
                    continue;

                m_dicZoneGroupSet[zoneID.Data] = strGroupSetName;
            }
        }

        private void AlarmMonitoring()
        {
            object item = null;

            m_cmbFireDetect.Invoke((MethodInvoker)delegate
            {
                item = m_cmbFireDetect.SelectedItem;
            });

            if (item != null)
            {
                libSensorProcess.ProcessIF process = (libSensorProcess.ProcessIF)item;

                if (process != m_prevProcess)
                {
                    if (process.TargetZone != null && process.TargetZone.LinkedZone != null && m_panelLog.RealTimeInfo.Length > 0)
                    {
                        if (m_panelLog.RealTimeInfo.Contains("GroupSet") == false)
                        {
                            string strGroupSetName;

                            if (m_dicZoneGroupSet.TryGetValue(process.TargetZone.LinkedZone.ID, out strGroupSetName))
                            {
                                m_panelLog.RealTimeInfo = m_panelLog.RealTimeInfo + ", GroupSet : " + strGroupSetName;

                                m_cmbFireDetect.Invoke((MethodInvoker)delegate
                                {
                                    m_panelLog.Refresh();
                                });
                            }
                        }

                        m_prevProcess = process;
                    }
                }
            }
        }
        #endregion
    }
}
