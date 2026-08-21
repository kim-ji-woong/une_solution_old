using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Net;
using UnE.GUI;
using SDMS.Help;
using DBUtility2;

namespace SDMS.PopupDialog
{
    public partial class FormPSMTankDetail : PopupFormBase, UnE.Control.ICCTVCtrlOwner
    {
        private int m_nHeightFold = 270;
        private int m_nHeightUnFold = 620;
        private UnE.PSM.PSMTank m_tank = null;
        private string m_strPSMManualPath = "";
        private int m_nRemainBitmap = 0;
        private VariousData<float> m_fInitRemains = null;

        private float m_fZeroValue = 0.0000f;

        public UnE.PSM.PSMTank Tank
        {
            get { return m_tank; }
            set
            {
                if (m_tank != value)
                {
                    if (m_tank != null)
                    {
                        // 수동으로 입력한 잔량 정보를 DB에 저장
                        SaveRemains();
                    }

                    m_tank = value;

                    if (m_tank == null)
                        m_fInitRemains = null;
                    else
                    {
                        if (m_tank.Remains == null)
                            m_fInitRemains = null;
                        else
                            m_fInitRemains = new VariousData<float>(m_tank.Remains.Data < 0.0f ? -m_tank.Remains.Data : m_tank.Remains.Data);
                    }

                    SetTankData();
                    SetMSDSButton();
                }
            }
        }

        private ManualManager m_manualManager = null;

        public FormPSMTankDetail(UnE.PSM.PSMTank tank)
        {
            this.DoubleBuffered = true;

            InitializeComponent();

            btnMSDS.Parent = panelRight;
            btnPSMMaterial.Parent = panelRight;
            picTankLocation.Parent = panelRight;
            picRemains.Parent = panelRight;
            picMaterialName.Parent = panelRight;
            picCapacity.Parent = panelRight;

            lblValueMaterialName.Parent = picMaterialName;
            lblValueTankLocation.Parent = picTankLocation;
            lblValueRemains.Parent = picRemains;
            textBoxRemains.Parent = picRemains;
            lblValueCapacity.Parent = picCapacity;
            imgPSMUsual.Parent = panelLeft;

            InitCtrlSize(this);
            SetChildCtrlResize(this, 459, 323); 

            InitEvent();

            Tank = tank;

            label1.Text = tank.Name;

            //imgPSMUsual.UseSingleLoop = false;
            labelRemains.Parent = imgPSMUsual;
            labelRemains.Text = "";
            labelRemains.BringToFront();

            m_manualManager = new ManualManager(this);
            SetManualID();
        }

        private void InitEvent()
        {
            this.Load += FormPSMTankDetail_Load;

            this.chkMonitoring.CheckedChanged += chkMonitoring_CheckedChanged;
            this.chkCCTV.CheckedChanged += chkCCTV_CheckedChanged;

            this.btnSelectUsed.Click += btnSelectUsed_Click;
        }

        private void LoadOptionData()
        {
            PeridComboBoxItem itemSelected = new PeridComboBoxItem();
            itemSelected.DisplayText = "최근 일주일간";
            itemSelected.Value = PeridComboBoxItem.LastPeriod.WEEK_ONE;
            cmbSelectUsed.Items.Add(itemSelected);

            PeridComboBoxItem item = new PeridComboBoxItem();
            item.DisplayText = "최근 한달간";
            item.Value = PeridComboBoxItem.LastPeriod.MONTH_ONE;
            cmbSelectUsed.Items.Add(item);

            item = new PeridComboBoxItem();
            item.DisplayText = "최근 3개월간";
            item.Value = PeridComboBoxItem.LastPeriod.MONTH_THREE;
            cmbSelectUsed.Items.Add(item);

            item = new PeridComboBoxItem();
            item.DisplayText = "최근 6개월간";
            item.Value = PeridComboBoxItem.LastPeriod.MONTH_SIX;
            cmbSelectUsed.Items.Add(item);

            item = new PeridComboBoxItem();
            item.DisplayText = "최근 1년간";
            item.Value = PeridComboBoxItem.LastPeriod.YEAR_ONE;
            cmbSelectUsed.Items.Add(item);

            cmbSelectUsed.SelectedItem = itemSelected;
        }


        #region Event Func

        private void FormPSMTankDetail_Load(object sender, EventArgs e)
        {
            LoadManualPath();

            // 팝업창 최초 로드시 작업
            // 1. 데이터 출력
            // 2. CCTV값 설정
            // 3. 유해물질탱크 잔량에 대해서 이미징 처리

            LoadOptionData();
            SetTankData();
            SetMSDSButton();
        }

        private void LoadManualPath()
        {
            //WebDBManager dbMgr = FormMain.Instance.DBManager;

            //string strSQL = string.Format("Select PropertyValue from OptionSDMS where PropertyName = 'PSMMaterialManualPath' and SiteID = {0}", UnE.SOP.ProxySOP.Instance.SiteID);
            //ArrayList arrResult = dbMgr.GetResultData(strSQL);

            //if (arrResult == null || arrResult.Count == 0)
            //    return;

            ////m_strPSMManualPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\" + WebDBManager.GetStringField(arrResult[0]);
            //btnPSMMaterial.Enabled = m_strPSMManualPath != null;
        }

        private void SetMSDSButton()
        {
            bool isEnable = false;

            if (m_tank != null && m_tank.Material != null && m_tank.Material.Name != "부생연료유")
                isEnable = true;

            this.btnMSDS.Enabled = isEnable;
        }

        // 수동으로 입력한 잔량 정보를 DB에 저장
        private void SaveRemains()
        {
            if (textBoxRemains.Visible)
            {
                string strValue = "NULL";
                VariousData<float> remains = null;
                float fRemains = -1.0f;

                if (float.TryParse(textBoxRemains.Text.Trim(), out fRemains))
                {
                    if (fRemains > 0.0f)
                        strValue = "-" + textBoxRemains.Text.Trim();
                    else if (fRemains == 0.0f)
                        strValue = "-0.0001";

                    remains = new VariousData<float>(fRemains);
                }

                if (m_fInitRemains == null && remains == null)
                    return;
                else if (m_fInitRemains != null && remains != null && m_fInitRemains.Data == remains.Data)
                    return;

                DateTime dtNow = DateTime.Now;
                string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

                string strSQL = string.Format("Update PSMTank set Remains = {0}, RemainUpdateTime = '{1}' where ID = {2}", strValue, strTime, m_tank.ID);
                FormMain.Instance.DBManager.GetResultData(strSQL);
            }
        }

        public void SetTankData()
        {
            if (m_tank == null)
            {
                lblValueMaterialName.Text = ":";
                labelRemains.Text = "";
                lblValueRemains.Text = "";
                lblValueTankLocation.Text = "";
                lblValueCapacity.Text = "";
                SetRemainsImage(0.0f);
                return;
            }

            if (m_tank.LocationName == m_tank.Name)
                this.Text = m_tank.Name;
            else
                this.Text = m_tank.LocationName + " " + m_tank.Name;

            //UnE.Sensor.CCTV[] cctvs = CCTVManager.Instance.GetCCTVArray(m_tank.EquipZone);

            //if (cctvs != null && cctvs.Count() > 0)
            //    SetCCTV(cctvs[0]);

            if (m_tank.Material != null)
                lblValueMaterialName.Text = m_tank.Material.Name;
            else
                lblValueMaterialName.Text = "";

            if (m_tank.Remains != null && m_tank.Capacity != null)
            {
                float fRemains = m_tank.Remains.Data < 0.0f ? -m_tank.Remains.Data : m_tank.Remains.Data;
                float fCapacity = m_tank.Capacity.Data < 0.0f ? -m_tank.Capacity.Data : m_tank.Capacity.Data;

                if (m_tank.Remains.Data >= 0.0f)
                {
                    textBoxRemains.Visible = false;
                    //lblValueRemains.Location = new Point(textBoxRemains.Location.X, lblValueRemains.Location.Y);

                    lblValueRemains.Text = string.Format("{0}{1}", FormPSMList.GetGasVolumeString(fRemains), m_tank.UnitName);
                }
                else
                {
                    textBoxRemains.Visible = true;
                    //lblValueRemains.Location = new Point(textBoxRemains.Location.X + textBoxRemains.Size.Width + 2, lblValueRemains.Location.Y);

                    fRemains = (fRemains == m_fZeroValue ? 0.0f : fRemains);

                    textBoxRemains.Text = string.Format("{0}", FormPSMList.GetGasVolumeString(fRemains));
                    lblValueRemains.Text = m_tank.UnitName;
                }

                if (fCapacity != 0.0f)
                {
                    float fRatio = fRemains / fCapacity * 100;

                    if (fCapacity < 0.0f)
                        fRatio = 0.0f;
                    else if (fRatio > 100.0f)
                        fRatio = 100.0f;

                    SetRemainsImage(fRatio);
                    labelRemains.Text = string.Format("{0:F1} %", FormPSMList.GetGasVolumeString(fRatio));
                    labelRemains.BringToFront();

                }
                else
                {
                    SetRemainsImage(0.0f);
                    labelRemains.Text = "";
                    labelRemains.BringToFront();
                }
            }
            else
            {
                textBoxRemains.Visible = true;
                lblValueRemains.Location = new Point(textBoxRemains.Location.X + textBoxRemains.Size.Width + 2, lblValueRemains.Location.Y);

                textBoxRemains.Text = "";
                lblValueRemains.Text = m_tank.UnitName;
                SetRemainsImage(0.0f);
                labelRemains.Text = "";
            }

            if (m_tank.Capacity == null || m_tank.Capacity.Data <= 0.0f)
                lblValueCapacity.Text = "-";
            else
                lblValueCapacity.Text = string.Format("{0} {1}", FormPSMList.GetGasVolumeString(m_tank.Capacity.Data), m_tank.UnitName);

            /*if (m_tank.Remains >= 0.0f)
            {
                lblValueRemains.Text = string.Format("{0:F1}{1}", FormPSMList.GetGasVolumeString(m_tank.Remains), m_tank.UnitName);

                if (m_tank.Capacity != 0.0f)
                    labelRemains.Text = string.Format("{0:F1}", FormPSMList.GetGasVolumeString(m_tank.Remains / m_tank.Capacity));
                else
                    labelRemains.Text = "";
            }
            else
            {
                lblValueRemains.Text = string.Format("-{0}", m_tank.UnitName);
                labelRemains.Text = "";
            }*/

            lblValueTankLocation.Text = m_tank.LocationName;
            labelRemains.BringToFront();
            SetLocation(); 
        }

        private void SetRemainsImage(float fRatio)
        {
            try
            {
                int nBitmap;
                Bitmap bmp = GetRemainsImage(fRatio, out nBitmap);

                if (m_nRemainBitmap != nBitmap)
                {
                    m_nRemainBitmap = nBitmap;
                    imgPSMUsual.Image = bmp;
                }
                else if (nBitmap == 0)
                {
                    imgPSMUsual.Image = global::SDMS.Properties.Resources._0;
                    m_nRemainBitmap = 0;
                }
            }
            catch (Exception)
            {
            }

        }

        private Bitmap GetRemainsImage(float fRatio, out int nBitmap)
        {
            if (fRatio >= 0.0f && fRatio < 2.5f)
            {
                nBitmap = 0;
                return SDMS.Properties.Resources._0;
            }
            else if (fRatio >= 2.5f && fRatio < 7.5f)
            {
                nBitmap = 5;
                return SDMS.Properties.Resources._5;
            }
            else if (fRatio >= 7.5f && fRatio < 12.5f)
            {
                nBitmap = 10;
                return SDMS.Properties.Resources._10;
            }
            else if (fRatio >= 12.5f && fRatio < 17.5f)
            {
                nBitmap = 15;
                return SDMS.Properties.Resources._15;
            }
            else if (fRatio >= 17.5f && fRatio < 22.5f)
            {
                nBitmap = 20;
                return SDMS.Properties.Resources._20;
            }
            else if (fRatio >= 22.5f && fRatio < 27.5f)
            {
                nBitmap = 25;
                return SDMS.Properties.Resources._25;
            }
            else if (fRatio >= 27.5f && fRatio < 32.5f)
            {
                nBitmap = 30;
                return SDMS.Properties.Resources._30;
            }
            else if (fRatio >= 32.5f && fRatio < 37.5f)
            {
                nBitmap = 35;
                return SDMS.Properties.Resources._35;
            }
            else if (fRatio >= 37.5f && fRatio < 42.5f)
            {
                nBitmap = 40;
                return SDMS.Properties.Resources._40;
            }
            else if (fRatio >= 42.5f && fRatio < 47.5f)
            {
                nBitmap = 45;
                return SDMS.Properties.Resources._45;
            }
            else if (fRatio >= 47.5f && fRatio < 52.5f)
            {
                nBitmap = 50;
                return SDMS.Properties.Resources._50;
            }
            else if (fRatio >= 52.5f && fRatio < 57.5f)
            {
                nBitmap = 55;
                return SDMS.Properties.Resources._55;
            }
            else if (fRatio >= 57.5f && fRatio < 62.5f)
            {
                nBitmap = 60;
                return SDMS.Properties.Resources._60;
            }
            else if (fRatio >= 62.5f && fRatio < 67.5f)
            {
                nBitmap = 65;
                return SDMS.Properties.Resources._65;
            }
            else if (fRatio >= 67.5f && fRatio < 72.5f)
            {
                nBitmap = 70;
                return SDMS.Properties.Resources._70;
            }
            else if (fRatio >= 72.5f && fRatio < 77.5f)
            {
                nBitmap = 75;
                return SDMS.Properties.Resources._75;
            }
            else if (fRatio >= 77.5f && fRatio < 82.5f)
            {
                nBitmap = 80;
                return SDMS.Properties.Resources._80;
            }
            else if (fRatio >= 82.5f && fRatio < 87.5f)
            {
                nBitmap = 85;
                return SDMS.Properties.Resources._85;
            }
            else if (fRatio >= 87.5f && fRatio < 92.5f)
            {
                nBitmap = 90;
                return SDMS.Properties.Resources._90;
            }
            else if (fRatio >= 92.5f && fRatio < 97.5f)
            {
                nBitmap = 95;
                return SDMS.Properties.Resources._95;
            }

            nBitmap = 100;
            return SDMS.Properties.Resources._100;
        }

        private void SetCCTV(UnE.Sensor.CCTV cctv)
        {
            if (cctv == null)
            {
                this.cctvCtrl1.Disconnect();
                return;
            }

            cctvCtrl1.CCTVOwner = this;

            if (cctvCtrl1.CCTVType == UnE.Control.CCTVTypes.None || (int)cctvCtrl1.CCTVType != cctv.CCTVType)
            {
                this.pnlMonitor.Controls.Remove(cctvCtrl1);

                this.cctvCtrl1 = new UnE.Control.CCTVCtrl((UnE.Control.CCTVTypes)cctv.CCTVType);
                cctvCtrl1.Visible = true;
                this.cctvCtrl1.Location = new System.Drawing.Point(3, 3);
                this.cctvCtrl1.Size = new Size(544, 332);
                this.cctvCtrl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
      | System.Windows.Forms.AnchorStyles.Left)
      | System.Windows.Forms.AnchorStyles.Right)));
                this.pnlMonitor.Controls.Add(cctvCtrl1);
            }

            cctvCtrl1.AddProperty("MediaType", "rtp-tcp");
            cctvCtrl1.AddProperty("Channel", cctv.Channel.ToString());
            cctvCtrl1.AddProperty("Stream", cctv.Stream.ToString());
            cctvCtrl1.AddProperty("HttpPort", cctv.HttpPort.ToString());
            cctvCtrl1.AddProperty("IPAddress", cctv.IPAddress);
            cctvCtrl1.AddProperty("Port", cctv.PortNo.ToString());
            cctvCtrl1.AddProperty("UserName", cctv.UserName);
            cctvCtrl1.AddProperty("Password", cctv.Password);

            if (cctvCtrl1.IsConnected == false)
                cctvCtrl1.Connect();
        }

        private void chkMonitoring_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkMonitoring.Checked == true)
            {
                this.Size = new Size(this.Size.Width, m_nHeightUnFold);
                this.pnlMonitor.Visible = true;
                this.cctvCtrl1.Connect();
            }
            else
            {
                this.Size = new Size(this.Size.Width, m_nHeightFold);
                this.pnlMonitor.Visible = true;
                this.cctvCtrl1.Disconnect();
            }
        }

        private void chkCCTV_CheckedChanged(object sender, EventArgs e)
        {
            // 주변 CCTV보기 체크박스 상태값 변환에 따른 이벤트 처리
            ShowEquipZoneCCTV(chkCCTV.Checked);
        }

        private void ShowEquipZoneCCTV(bool useEquipZone)
        {
            int nEquipZoneID = useEquipZone && m_tank != null && m_tank.EquipZone != null ? m_tank.EquipZone.ID : 0;
            FormMain.Instance.PageHome.ShowEquipZoneCCTVs(nEquipZoneID);
        }

        private void btnPSMMaterial_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_tank == null || m_tank.Material == null || m_manualManager.IsHelpMode)
                return;

            //// 해당 탱크에 대한 비상대응(?) 문서자료 팝업
            //if (m_strPSMManualPath == null || m_strPSMManualPath.Length == 0)
            //    return;

            //if (m_tank == null || m_tank.Material == null)
            //    return;

            //OpenMaterialPDF(m_tank.Material.Name);
            //int nPageNumber = m_tank == null || m_tank.Material == null ? -1 : m_tank.Material.PageNo;
            //OpenPDF(nPageNumber, m_strPSMManualPath);

            // 해당 탱크에 대한 비상대응(?) 문서자료 다운 및 팝업

            string szDir = Path.GetDirectoryName(Application.ExecutablePath) + "\\PSM\\MaterialDetail\\";
            string szFileName = String.Format("{0}{1}.pdf", szDir, m_tank.Material.Name);
            string szURL = String.Format("{0}{1}.pdf", FormMain.Instance.DBManager.WebServerURL.Replace("/SOP", "/Doc/PSMMaterialDetail/"), m_tank.Material.ID.ToString());

            if (Directory.Exists(szDir) == false)
                Directory.CreateDirectory(szDir);

            try
            {
                if (File.Exists(szFileName) == true)
                    File.Delete(szFileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return;
            }


            try
            {
                WebClient client = new WebClient();
                client.DownloadFile(szURL, szFileName);

                if (File.Exists(szFileName) == true)
                    OpenPDF(0, szFileName);

            }
            catch (Exception ex)
            {
                // 파일이 이미 열려있음.
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

        }

        private void btnMSDS_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_tank == null || m_tank.Material == null || m_manualManager.IsHelpMode)
                return;

            // MSDS다운
            string szDir = Path.GetDirectoryName(Application.ExecutablePath) + "\\PSM\\MSDS\\";
            string szFileName = String.Format("{0}{1}.pdf", szDir, m_tank.Material.Name);
            string szURL = String.Format("{0}{1}.pdf", FormMain.Instance.DBManager.WebServerURL.Replace("/SOP", "/Doc/MSDS/"), m_tank.Material.ID.ToString());

            if (Directory.Exists(szDir) == false)
                Directory.CreateDirectory(szDir);

            try
            {
                if (File.Exists(szFileName) == true)
                    File.Delete(szFileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return;
            }


            try
            {
                WebClient client = new WebClient();
                client.DownloadFile(szURL, szFileName);

                if (File.Exists(szFileName) == true)
                    OpenPDF(0, szFileName);
            }
            catch (Exception ex)
            {
                // 파일이 이미 열려있음.
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

        }

        private void OpenMaterialPDF(string strMaterialName)
        {
            string strPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\PSMMaterials\\" + strMaterialName + ".pdf";
            string strAcrobatPath = "";
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            if (GetRegistry(ref strAcrobatPath))
            {
                startInfo.Arguments = string.Format("/A \"page={0}&zoom=100\" \"{1}\"", 1, strPath);
                startInfo.FileName = strAcrobatPath;
            }
            else
                startInfo.FileName = strPath;

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo = startInfo;
            process.Start();
        }

        public static void OpenPDF(int nPageNumber, string strPath)
        {
            string strAcrobatPath = "";
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            if (nPageNumber > 0 && GetRegistry(ref strAcrobatPath))
            {
                startInfo.Arguments = string.Format("/A \"page={0}&zoom=100\" \"{1}\"", nPageNumber, strPath);
                startInfo.FileName = strAcrobatPath;
            }
            else
                startInfo.FileName = strPath;

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo = startInfo;
            process.Start();
        }

        public static bool GetRegistry(ref string strAcrobatPath)
        {
            const string AcrobatRoot = @"Applications\AcroRD32.exe";

            Microsoft.Win32.RegistryKey R = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(AcrobatRoot);

            if (R == null)
                return false;

            if (strAcrobatPath != null && strAcrobatPath.Length > 0)
                return true;

            strAcrobatPath = "";

            Microsoft.Win32.RegistryKey shell = R.OpenSubKey("shell");

            if (shell == null)
                return false;

            Microsoft.Win32.RegistryKey read = shell.OpenSubKey("Read");

            if (read == null)
                return false;

            Microsoft.Win32.RegistryKey command = read.OpenSubKey("command");

            if (command == null)
                return false;

            string[] names = command.GetValueNames();

            if (names == null || names.Count() == 0)
                return false;

            object value = command.GetValue(names[0]);

            if (value == null)
                return false;

            string strValue = value.ToString();

            int nIndex1 = strValue.IndexOf('\"');

            if (nIndex1 < 0)
                return false;

            int nIndex2 = strValue.IndexOf('\"', nIndex1 + 1);

            if (nIndex2 < 0)
                return false;

            string strPath = strValue.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
            strAcrobatPath = strPath;

            return true;
        }

        private void btnSelectUsed_Click(object sender, EventArgs e)
        {
            // cmbSelectUsed 의 선택값에 따른 총 사용량 조회
        }

        private void FormPSMTankDetail_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_tank != null)
                SaveRemains();
        }

        #endregion Event Func


        #region ComboBox Item Class

        private class ComboBoxItem
        {
            public string DisplayText { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return DisplayText;
            }
        }

        private class PeridComboBoxItem : ComboBoxItem
        {
            public enum LastPeriod { WEEK_ONE = 0, MONTH_ONE = 1, MONTH_THREE = 2, MONTH_SIX = 3, YEAR_ONE = 4 }
        }

        #endregion ComboBox Item Class

        // CCTV Mouse LButton Click
        public void OnMouseLButtonClick()
        {

        }

        // CCTV Mouse LButton DoubleClick
        public void OnMouseLButtonDoubleClick()
        {

        }

        private void labelRemains_EnabledChanged(object sender, EventArgs e)
        {

        }

        private void btnPSMMaterial_Click_1(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void SetLocation()
        {
            float sizePer = 1.0f;
            int nSpace = 26;
            int nSpace2 = 3;

            if (FormMain.Instance.Resolution == Resolution.FourK)
                sizePer = 2.0f;
            else if (FormMain.Instance.Resolution == Resolution.Other)
                sizePer = 1.5f;
            nSpace = (int)(nSpace * sizePer);
            nSpace2 = (int)(nSpace2 * sizePer);

            lblValueMaterialName.Location = new Point(nSpace, nSpace);
            lblValueTankLocation.Location = new Point(nSpace, nSpace);
            if (textBoxRemains.Visible)
            {
                textBoxRemains.Location = new Point(nSpace, nSpace - nSpace2);
                lblValueRemains.Location = new Point(textBoxRemains.Location.X + textBoxRemains.Width, nSpace);
            }
            else
                lblValueRemains.Location = new Point(nSpace, nSpace);
            lblValueCapacity.Location = new Point(nSpace, nSpace);

            labelRemains.Location = new Point(imgPSMUsual.Width / 2 - labelRemains.Width / 2, imgPSMUsual.Height / 2 - labelRemains.Height / 2);
        }

        private void SetManualID()
        {
            m_manualManager.Handle = this.Handle;

            m_manualManager.Clear();
            m_manualManager.SetID(this, "PSMList_Tank_PopupMenu");
            m_manualManager.SetID(btnMSDS, "PSMList_Tank_PopupMenu");
            m_manualManager.SetID(btnPSMMaterial, "PSMList_Tank_PopupMenu");
            m_manualManager.ProcessEvent();
        } 
    }    
}
namespace System.Windows.Forms
{
    public class LabelEx : Label
    {
        public LabelEx()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!Enabled)
            {
                SolidBrush drawBrush = new SolidBrush(Color.Black); //Choose colour

                e.Graphics.DrawString(Text, Font, drawBrush, 0f, 0f); //Dra whatever text was on the label
            }
            else
            {
                base.OnPaint(e); //Default Forecolours
            }
        }
    }
}