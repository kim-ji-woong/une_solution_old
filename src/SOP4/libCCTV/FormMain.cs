using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using DBUtility;

namespace UnE.CCTV
{
    public partial class FormMain : Form, ICCTVFormOwner
    {
        public enum CCTVMode 
        {
            Default = 1,
            EquipZone = 2,
            FIRESituation = 3,
            PSMSituation = 4
        }


        private static FormMain m_Instance = null;
        public static FormMain Instance
        {
            get
            {
                return m_Instance;
            }
        }

        private bool m_bThumbnailMode = false;
        public bool ThumbnailMode
        {
            get
            {
                return m_bThumbnailMode;
            }
        }

        protected bool m_bClosed = false;
        public bool CloseApplication
        {
            get { return m_bClosed; }
            set { m_bClosed = value; }
        }


        private WebDBManager m_DbMgr = null;
        public WebDBManager DBManager
        {
            get { return m_DbMgr; }
        }

        private Pipelib.PassivePipeClient m_PipeServer;
        public Pipelib.PassivePipeClient PipeServer
        {
            get { return m_PipeServer; }
        }

        private Form4CCTV cctvForm = null;
        public Form4CCTV CCTVForm
        {
            get { return cctvForm; }
        }
        
        private log4net.ILog logger = null;
        public log4net.ILog Logger
        {
            get { return logger; }
            set { logger = value; }
        }


        private CCTVMode m_PrevMode = CCTVMode.Default;
        private CCTVMode m_Mode = CCTVMode.Default;
        public CCTVMode Mode
        {
            get { return m_Mode; }
        }

        public void AddPythonFunction()
        {
            ScriptProxy proxy = ScriptProxy.Instance;
            proxy.UserObject.SetDefaultCCTV = new Action(ShowDefaultCCTV);
            proxy.UserObject.ShowDefaultCCTV = new Action(ShowDefaultCCTV);

            proxy.UserObject.SendMsg = new Action<string>(m_PipeServer.Send);
            proxy.UserObject.SetViewerImage = new Action<int, string, string>(SetViewerImage);
            proxy.UserObject.ShowSituationCCTV = new Action<int>(ShowSituationCCTV);
            proxy.UserObject.ShowSituationCCTV2 = new Action<int, int>(ShowSituationCCTV2);
            proxy.UserObject.ShowNormalCCTV = new Action(ShowNormalCCTV);
            proxy.UserObject.SetCCTV = new Action<int, int, int, int>(SetCCTV);
            proxy.UserObject.SetPreset = new Action<int>(SetPreset);

            proxy.UserObject.SetHistoryID = new Action<int>(SetHistoryID);
            proxy.UserObject.SetTargetZone = new Action<int>(SetTargetZone);

            proxy.UserObject.EditEquipZoneCCTV = new Action<int>(EditEquipZoneCCTV);

            proxy.AddVariable("MainForm", CCTVFormFrame.Instance);
            proxy.UserObject.SetVisible = new Action<bool>(SetVisible);

            proxy.UserObject.SetVisible2 = new Action<bool>(SetVisibleNofeedBack);
            proxy.UserObject.ShowEquipZoneCCTVs = new Action<int>(ShowEquipZoneCCTVs);
            proxy.UserObject.SetTitle = new Action<string>(SetTitle);


            //proxy.UserObject.SetPSMImage = new Action<int>(SetPSMImage);
        }

        public void SetVisibleNofeedBack(bool bShow)
        {
            CCTVFormFrame.Instance.Visible = bShow;
        }

        public void SetVisible(bool bShow)
        {
            CCTVFormFrame.Instance.Visible = bShow;
            if (CCTVFormFrame.Instance.WindowState == FormWindowState.Minimized)
                CCTVFormFrame.Instance.WindowState = FormWindowState.Maximized;

            if (m_PipeServer != null)
            {
                if (bShow == true)
                    m_PipeServer.Send("SetVisible(True)");
                else
                    m_PipeServer.Send("SetVisible(False)");
            }
        }

        public void LoadBaseData()
        {
            CCTVManager.Instance.LoadCCTV(true);
            CCTVManager.Instance.LoadCCTV(false);

            ZoneManager.Instance.LoadBuildingData();
            ZoneManager.Instance.LoadZones();
            ZoneManager.Instance.LoadEquipmentZone();

            CCTVManager.Instance.LoadEquipZoneCCTV();
            ReadSDMSOptions();
        }

        private void ReadSDMSOptions()
        {
            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'UsePSM' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = m_DbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strValue = WebDBManager.GetStringField(arrResult[0]);

            if (strValue == null)
                return;

            if (strValue == "0" || strValue.Trim().ToLower() == "false")
                UnE.SOP.ProxySOP.Instance.UsePSM = false;
        }

        private void ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                UnE.Utility.UMessageBox.Show("Site ID가 지정되지 않았습니다. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            int nSiteId = 1;
            if (int.TryParse(szSiteID, out nSiteId))
            {
                UnE.SOP.ProxySOP.Instance.SiteID = nSiteId;
            }
            else
            {
                UnE.Utility.UMessageBox.Show("잘못된 Site ID입니다.. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            UnE.SOP.ProxySOP.Instance.SiteID = nSiteId;
        }

        /// <summary>
        /// 특정 모니터의 시작위치 구하기
        /// </summary>
        /// <param name="nMonitor">대상 모니터 번호, 1부터 시작</param>
        /// <returns>대상 모니터의 시작위치</returns>
        public Point GetMonitorPosition(int nMonitor)
        {
            Screen[] sc;
            sc = Screen.AllScreens;

            if (sc.Length == 0)
            {
                return new Point(0, 0);
            }

            string szNum = nMonitor.ToString();
            int nIdx = -1;
            for (int i = 0; i < sc.Length; i++)
            {
                if (sc[i].DeviceName.IndexOf(szNum) != -1)
                {
                    nIdx = i;
                    break;
                }
            }

            if (nIdx == -1)
                nIdx = 0;

            if (sc.Length >= nIdx)
            {
                return sc[nIdx].Bounds.Location;
            }
            return new Point(0, 0);
        }


        /// <summary>
        /// 특정 모니터 전체를 Form이 사용하도록 설정
        /// </summary>
        /// <param name="form">대상 Form</param>
        /// <param name="nDisplay">대상 모니터</param>
        /// <returns>true면 완료/false면 1번모니터로 설정</returns>
        private bool SetMonitorForm(Form form, int nDisplay)
        {
            Screen[] sc;
            sc = Screen.AllScreens;
            if (form == null)
                return false;


            if (sc.Length == 0)
            {
                return false;
            }

            string szNum = nDisplay.ToString();
            int nIdx = -1;
            for (int i = 0; i < sc.Length; i++)
            {
                if (i == (nDisplay - 1))
                //if (sc[i].DeviceName.IndexOf(szNum) != -1)
                {
                    nIdx = i;
                    break;
                }
            }

            if (nIdx == -1)
                nIdx = 0;

            if (sc.Length > nIdx)
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = sc[nIdx].Bounds.Location;
                form.Size = new Size(sc[nIdx].Bounds.Width, sc[nIdx].Bounds.Height);

                form.WindowState = FormWindowState.Maximized;
            }
            else
            {
                form.WindowState = FormWindowState.Maximized;
            }

            return true;
        }


        private string m_nPipeName = "CCTVPipe";

        public FormMain(int nSiteID, string szPipeName)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            m_nPipeName = szPipeName;
            UnE.SOP.ProxySOP.Instance.SiteID = nSiteID;
            //ReadSiteID();
            m_Instance = this;
            InitializeComponent();

            ScriptProxy proxy = ScriptProxy.Instance;
            UnE.SOP.ProxySOP.Instance.ShowCCTVForm = true;

            FormMain.KillProcess("CCTVViewer");

            m_DbMgr = new WebDBManager(nSiteID);
            /*if (nSiteID == 2)
                m_DbMgr = new WebDBManager("SOP4");
            else if (nSiteID == 1)
                m_DbMgr = new WebDBManager("SOP3");*/
                                   
            LoadBaseData();

        

            

            FormCCTVList formList = new FormCCTVList();
            ProxyCCTV.Instance.CCTVList = formList;

            m_PipeServer = new Pipelib.PassivePipeClient(szPipeName);
            m_PipeServer.OnReciveMessage += OnReciveMessage;
            m_PipeServer.BeginPipe();

            cctvForm = new Form4CCTV(this, "SOP");
            cctvForm.TopLevel = false;
            cctvForm.Dock = DockStyle.Fill;
            this.Controls.Add(cctvForm);
                        
            cctvForm.SetOwner(this);
            cctvForm.SetDefaultCCTV();

            CCTVList cvList = cctvForm.GetCCTVList(null);
            if (cvList != null)
            {
                ArrayList arrCCTVs = cvList.GetAllCCTV();
                if (arrCCTVs != null)
                {
                    cctvForm.SetCCTV(arrCCTVs, null);                   
                }
            }

            cctvForm.Show();
            AddPythonFunction();

            LoadCompass();

            m_LogTimer.Enabled = true;
            m_LogTimer.Start();
        }

        public void ShowEquipZoneCCTVs(int nEquipZoneID)
        {
            EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);
            ShowEquipZoneCCTVs(equipZone);
        }

        public void ShowEquipZoneCCTVs(EquipmentZone zone)
        {
            m_PrevMode = m_Mode;
            m_Mode = CCTVMode.EquipZone;
            ProxyCCTV.Instance.EquipZoneCCTVMode = true;
            ProxyCCTV.Instance.CurrentEquipZone = zone;

            ShowNormalCCTV();

            string strOutdoorFilePath, strIndoorFilePath;
            DownloadEquipZoneImage(zone, out strOutdoorFilePath, out strIndoorFilePath);

            if (strOutdoorFilePath.Length > 0)
                SetViewerImage(3, strOutdoorFilePath, zone.ZoneName);

            if (strIndoorFilePath.Length > 0)
                SetViewerImage(4, strIndoorFilePath, zone.ZoneName + "(실내)");

            ShowSituationCCTV();

            if (zone == null)
                return;

            if (cctvForm == null)
                return;

            CCTV[] arrCCTVs = CCTVManager.Instance.GetCCTVArray(zone);

            if (arrCCTVs == null)
            {
                cctvForm.SetCCTV(Form4CCTV.CCTV_POSITION.TM, null);
                cctvForm.SetCCTV(Form4CCTV.CCTV_POSITION.BM, null);
                cctvForm.SetCCTV(Form4CCTV.CCTV_POSITION.BR, null);
                cctvForm.SetCCTV(Form4CCTV.CCTV_POSITION.TR, null);
                cctvForm.SetCCTV(Form4CCTV.CCTV_POSITION.TL, null);
                cctvForm.SetCCTV(Form4CCTV.CCTV_POSITION.BL, null);
            }
            else
            {
                int nCountCCTV = arrCCTVs.Length;

                //화면의 위치는 -> TL = 0, TM = 1, BM = 4, BL = 3, BR = 5, TR = 2
                //DB에서 로딩 순서는 TM, BM, BR, TR, TL, BL 이다 
                if (nCountCCTV > 0)
                    cctvForm.SetCCTV(Form4CCTV.CCTV_POSITION.TM, arrCCTVs[0]);
                else
                    cctvForm.SetCCTV(Form4CCTV.CCTV_POSITION.TM, null);

                if (nCountCCTV > 1)
                    cctvForm.SetCCTV(Form4CCTV.CCTV_POSITION.BM, arrCCTVs[1]);
                else
                    cctvForm.SetCCTV(Form4CCTV.CCTV_POSITION.BM, null);

                if (nCountCCTV > 2)
                    cctvForm.SetCCTV(Form4CCTV.CCTV_POSITION.BR, arrCCTVs[2]);
                else
                    cctvForm.SetCCTV(Form4CCTV.CCTV_POSITION.BR, null);
                if (nCountCCTV > 3)
                    cctvForm.SetCCTV(Form4CCTV.CCTV_POSITION.TR, arrCCTVs[3]);
                else
                    cctvForm.SetCCTV(Form4CCTV.CCTV_POSITION.TR, null);

                if (nCountCCTV > 4)
                    cctvForm.SetCCTV(Form4CCTV.CCTV_POSITION.TL, arrCCTVs[4]);
                else
                    cctvForm.SetCCTV(Form4CCTV.CCTV_POSITION.TL, null);

                if (nCountCCTV > 5)
                    cctvForm.SetCCTV(Form4CCTV.CCTV_POSITION.BL, arrCCTVs[5]);
                else
                    cctvForm.SetCCTV(Form4CCTV.CCTV_POSITION.BL, null);
            }
        }

        private void DownloadEquipZoneImage(EquipmentZone equipZone, out string strOutdoorFilePath, out string strIndoorFilePath)
        {
            strOutdoorFilePath = strIndoorFilePath = "";
            string strOutdoorTarget = "EquipZoneOutdoorFolder";
            string strIndoorTarget = "EquipZoneIndoorFolder";

            string strSQL = string.Format("Select PropertyName, PropertyValue from OptionSDMS where (PropertyName = '{0}' or PropertyName = '{1}') and SiteID = {2}",
                strOutdoorTarget, strIndoorTarget, UnE.SOP.ProxySOP.Instance.SiteID);
            ArrayList arrResult = m_DbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            string strRootURL = GetRootURL();
            string strOutdoorURL = "", strIndoorURL = "";

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                string strPropertyName = WebDBManager.GetStringField(arrResult[i]);
                string strPropertyValue = WebDBManager.GetStringField(arrResult[i + 1]);

                if (strPropertyValue == null || strPropertyValue.Length == 0)
                    continue;

                strPropertyValue = strPropertyValue.Trim();

                if (!strPropertyValue.StartsWith("/"))
                    strPropertyValue = "/" + strPropertyValue;

                if (strPropertyName == strOutdoorTarget)
                    strOutdoorURL = strRootURL + strPropertyValue;
                else if (strPropertyName == strIndoorTarget)
                    strIndoorURL = strRootURL + strPropertyValue;
            }

            System.Net.WebClient web = new System.Net.WebClient();
            string strImageFileName = "/" + equipZone.ID.ToString() + ".png";

            try
            {
                if (strOutdoorURL.Length > 0)
                    strOutdoorFilePath = DownloadFile(web, strOutdoorURL + strImageFileName, "EquipZoneOutdoorImage.png");

                if (strIndoorURL.Length > 0)
                    strIndoorFilePath = DownloadFile(web, strIndoorURL + strImageFileName, "EquipZoneIndoorImage.png");
            }
            catch (Exception)
            { 
            }

            web.Dispose();
        }

        private string DownloadFile(System.Net.WebClient web, string strURL, string strLocalFileName)
        {
            string strFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.Templates);
            string strFilePath = strFolder + "\\" + strLocalFileName;

            if (System.IO.File.Exists(strFilePath))
                System.IO.File.Delete(strFilePath);

            try
            {
                web.DownloadFile(strURL, strFilePath);
            }
            catch (Exception e)
            {
                strFilePath = "";
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return strFilePath;
        }

        private string GetRootURL()
        {
            string strURL = m_DbMgr.WebServerURL;

            int nIndex = strURL.IndexOf("//");

            if (nIndex >= 0)
            {
                int nIndex2 = strURL.IndexOf('/', nIndex + 2);

                if (nIndex2 >= 0)
                    strURL = strURL.Substring(0, nIndex2);
            }
            else
            {
                int nIndex2 = strURL.IndexOf('/');

                if (nIndex2 >= 0)
                    strURL = strURL.Substring(0, nIndex2);
            }

            return strURL;
        }

        public void ShowDefaultCCTV()
        {
            ProxyCCTV.Instance.EquipZoneCCTVMode = false;
            m_PrevMode = m_Mode;
            m_Mode = CCTVMode.Default;
            m_nSituationMode = (int)m_Mode;
            ShowNormalCCTV();
            cctvForm.SetDefaultCCTV();

            CCTVList cvList = cctvForm.GetCCTVList(null);

            if (cvList != null)
            {
                ArrayList arrCCTVs = cvList.GetAllCCTV();
                if (arrCCTVs != null)
                {
                    cctvForm.SetCCTV(arrCCTVs, null);
                }
            }
        }


        private void DisablePictureBox()
        {
            Form4CCTV form = FormMain.Instance.CCTVForm;
            if (form != null && form.IsDisposed == false)
            {
                if (CCTVForm.GetContent(0) != null && CCTVForm.GetContent(0) is PictureBox)
                {
                    CCTVForm.SetPanel(0, null, true);
                }

                if (CCTVForm.GetContent(3) != null && CCTVForm.GetContent(3) is PictureBox)
                {
                    CCTVForm.SetPanel(3, null, true);
                }
                if (CCTVForm.GetContent(4) != null && CCTVForm.GetContent(4) is PictureBox)
                {
                    CCTVForm.SetPanel(4, null, true);
                }

                if (CCTVForm.GetContent(5) != null && CCTVForm.GetContent(5) is PictureBox)
                {
                    CCTVForm.SetPanel(5, null, true);
                }
            }
        }

        public void ShowNormalCCTV()
        {
            DisablePictureBox();

            CCTVFormFrame.Instance.DetectPosition.Visible = false;
        }

        private int m_nLastCCTV1 = -1;
        private int m_nLastCCTV2 = -1;
        private int m_nLastCCTV3 = -1;
        private int m_nLastCCTV4 = -1;

        private string m_imgLast1 = null;
        private string m_imgLast3 = null;
        private string m_imgLast4 = null;
        private string m_imgLast5 = null;

        public void SetPreset(int nType)
        {
            cctvForm.SetPreset(nType);
        }

        public void SetCCTV(int v1, int v2, int v3, int v4)
        {
            if (m_nPrevHistoryID == m_nLastHistoryID)
                return;

            if (cctvForm != null && cctvForm.IsDisposed == false)
            {
                if ((m_Mode == CCTVMode.FIRESituation || m_Mode == CCTVMode.PSMSituation)  && m_nSituationMode != 3)
                {
                    m_nLastCCTV1 = v1;
                    m_nLastCCTV2 = v2;
                    m_nLastCCTV3 = v3;
                    m_nLastCCTV4 = v4;

                    if (m_imgLast1 != null)
                        SetViewerImage(1, m_imgLast1, m_strImageTitle1);
                    if (m_imgLast3 != null)
                        SetViewerImage(2, m_imgLast3, m_strImageTitle2);

                    if (m_Mode == CCTVMode.PSMSituation )
                    {
                        
                        if (m_imgLast4 != null)
                            SetViewerImage(3, m_imgLast4, m_strImageTitle3);
                        if (m_imgLast5 != null)
                            SetViewerImage(4, m_imgLast5, m_strImageTitle4);
                    }
                  

                    SetLastCCTV();


                }
                else
                {
                    CCTV cctv1 = CCTVManager.Instance.GetCCTV(v1);
                    cctvForm.SetCCTV((Form4CCTV.CCTV_POSITION)1, cctv1);

                    CCTV cctv2 = CCTVManager.Instance.GetCCTV(v2);
                    cctvForm.SetCCTV((Form4CCTV.CCTV_POSITION)4, cctv2);

                    CCTV cctv3 = CCTVManager.Instance.GetCCTV(v3);
                    cctvForm.SetCCTV((Form4CCTV.CCTV_POSITION)5, cctv3);

                    CCTV cctv4 = CCTVManager.Instance.GetCCTV(v4);
                    cctvForm.SetCCTV((Form4CCTV.CCTV_POSITION)2, cctv4);
                }
             
            }
        }

        private void SetLastCCTV()
        {
            CCTV cctv1 = CCTVManager.Instance.GetCCTV(m_nLastCCTV1);
            cctvForm.SetCCTV((Form4CCTV.CCTV_POSITION)1, cctv1);

            CCTV cctv2 = CCTVManager.Instance.GetCCTV(m_nLastCCTV4);
            cctvForm.SetCCTV((Form4CCTV.CCTV_POSITION)4, cctv2);

            CCTV cctv3 = CCTVManager.Instance.GetCCTV(m_nLastCCTV2);
            cctvForm.SetCCTV((Form4CCTV.CCTV_POSITION)5, cctv3);

            CCTV cctv4 = CCTVManager.Instance.GetCCTV(m_nLastCCTV3);
            cctvForm.SetCCTV((Form4CCTV.CCTV_POSITION)2, cctv4);
        }


        //public void ShowTempPSMCCTV()
        //{
        //    ProxyCCTV.Instance.EquipZoneCCTVMode = false;

        //    m_PrevMode = m_Mode;

        //    m_Mode = CCTVMode.Default;

        //    Form4CCTV form = FormMain.Instance.CCTVForm;
        //    if (form != null && form.IsDisposed == false)
        //    {
        //        SetLastCCTV();

        //        if (form.GetContent(0) != null && form.GetContent(0).GetType() != typeof(PictureBox))
        //            form.SetPanel(0, form.PictureBox1, false);
        //        if (form.GetContent(3) != null && form.GetContent(3).GetType() != typeof(PictureBox))
        //            form.SetPanel(3, form.PictureBox2, false);

        //        if (form.GetContent(4) != null && form.GetContent(4).GetType() != typeof(PictureBox))
        //            form.SetPanel(4, form.PictureBox3, false);
        //        if (form.GetContent(5) != null && form.GetContent(5).GetType() != typeof(PictureBox))
        //            form.SetPanel(5, form.PictureBox4, false);
        //    }            

        //    CCTVFormFrame.Instance.DetectPosition.Visible = true;
        //}

        public void ShowSituationCCTV()
        {

            //SetLastCCTV();

            if (m_PrevMode == CCTVMode.FIRESituation)
            {
                if (m_imgLast1 != null)
                    SetViewerImage(1, m_imgLast1, m_strImageTitle1);
                if (m_imgLast3 != null)
                    SetViewerImage(2, m_imgLast3, m_strImageTitle2);
                ShowSituationCCTV(1);
            }
            else if (m_PrevMode == CCTVMode.PSMSituation)
            {
                if (m_imgLast1 != null)
                    SetViewerImage(1, m_imgLast1, m_strImageTitle1);
                if (m_imgLast3 != null)
                    SetViewerImage(2, m_imgLast3, m_strImageTitle2);
                if (m_imgLast4 != null)
                    SetViewerImage(3, m_imgLast4, m_strImageTitle3);
                if (m_imgLast5 != null)
                    SetViewerImage(4, m_imgLast5, m_strImageTitle4);
                
                ShowSituationCCTV(2);
            }
        }


        private int m_nSituationMode = 0;
        public void ShowSituationCCTV(int nMode)
        {
            ProxyCCTV.Instance.EquipZoneCCTVMode = false;

            m_PrevMode = m_Mode;

            if (nMode == 1)
            {
                m_Mode = CCTVMode.FIRESituation;

                Form4CCTV form = FormMain.Instance.CCTVForm;
                if (form != null && form.IsDisposed == false)
                {
                    if (m_nSituationMode != 3)
                    {
                        if (form.PictureBox1.BackgroundImage != null)
                            m_imgLast1 = m_szImagePath1;

                        if (form.PictureBox2.BackgroundImage != null)
                            m_imgLast3 = m_szImagePath2;

                        if (form.PictureBox3.BackgroundImage != null)
                            m_imgLast4 = m_szImagePath3;

                        if (form.PictureBox4.BackgroundImage != null)
                            m_imgLast5 = m_szImagePath4;
                    }


                    DisablePictureBox();

                    SetLastCCTV();

                    if (form.GetContent(0) != null && !(form.GetContent(0) is PictureBox))
                        form.SetPanel(0, form.PictureBox1, false);
                    if (form.GetContent(3) != null && !(form.GetContent(3) is PictureBox))
                        form.SetPanel(3, form.PictureBox2, false);

                }
            }
            else if (nMode == 2)
            {

                m_Mode = CCTVMode.PSMSituation;

                Form4CCTV form = FormMain.Instance.CCTVForm;
                if (form != null && form.IsDisposed == false)
                {
                    if (m_nSituationMode != 3)
                    {
                        if (form.PictureBox1.BackgroundImage != null)
                            m_imgLast1 = m_szImagePath1;

                        if (form.PictureBox2.BackgroundImage != null)
                            m_imgLast3 = m_szImagePath2;

                        if (form.PictureBox3.BackgroundImage != null)
                            m_imgLast4 = m_szImagePath3;

                        if (form.PictureBox4.BackgroundImage != null)
                            m_imgLast5 = m_szImagePath4;
                    }

                    SetLastCCTV();

                    if (form.GetContent(0) != null && !(form.GetContent(0) is PictureBox))
                        form.SetPanel(0, form.PictureBox1, false);
                    if (form.GetContent(3) != null && !(form.GetContent(3) is PictureBox))
                        form.SetPanel(3, form.PictureBox2, false);

                    if (form.GetContent(4) != null && !(form.GetContent(4) is PictureBox))
                        form.SetPanel(4, form.PictureBox3, false);
                    if (form.GetContent(5) != null && !(form.GetContent(5) is PictureBox))
                        form.SetPanel(5, form.PictureBox4, false);
                }
            }
            else if (nMode == 3)
            {

                //m_Mode = CCTVMode.Default;

                Form4CCTV form = FormMain.Instance.CCTVForm;
                if (form != null && form.IsDisposed == false)
                {
                    //if (form.PictureBox1.BackgroundImage != null)
                    //    m_imgLast1 = m_szImagePath1;

                    //if (form.PictureBox2.BackgroundImage != null)
                    //    m_imgLast3 = m_szImagePath2;

                    //if (form.PictureBox3.BackgroundImage != null)
                    //    m_imgLast4 = m_szImagePath3;

                    //if (form.PictureBox4.BackgroundImage != null)
                    //    m_imgLast5 = m_szImagePath4;


                    //SetLastCCTV();

                    if (form.GetContent(0) != null && !(form.GetContent(0) is PictureBox))
                        form.SetPanel(0, form.PictureBox1, false);
                    if (form.GetContent(3) != null && !(form.GetContent(3) is PictureBox))
                        form.SetPanel(3, form.PictureBox2, false);

                    if (form.GetContent(4) != null && !(form.GetContent(4) is PictureBox))
                        form.SetPanel(4, form.PictureBox3, false);
                    if (form.GetContent(5) != null && !(form.GetContent(5) is PictureBox))
                        form.SetPanel(5, form.PictureBox4, false);
                }
            }

            CCTVFormFrame.Instance.DetectPosition.Visible = true;

            m_nSituationMode = nMode;
        }

        public void ShowSituationCCTV2(int nMode, int nEquipZoneID)
        {
            EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);

            if (equipZone != null)
            {
                ShowSituationCCTV(nMode);
                ProxyCCTV.Instance.CurrentEquipZone = equipZone;
            }
        }


        private CCTVMode mSavedMode = CCTVMode.Default;
        public void SaveLastState()
        {
            mSavedMode = m_Mode;
        }

        public void ShowLastCCTV()
        {
            m_Mode = mSavedMode;
            if( m_Mode == CCTVMode.Default)
            {
                ShowDefaultCCTV();
            }
            else
            {
                if (mSavedMode == CCTVMode.FIRESituation)
                    ShowSituationCCTV(1);
                else if (mSavedMode == CCTVMode.PSMSituation)
                    ShowSituationCCTV(2);
            }
        }

        //public void SetPSMImage(int nPSMSensorID)
        //{
        //    if (nPSMSensorID < 0)
        //        return;

        //    string szText = "SELECT Image01 ,Image02 ,Image03 FROM PSMSensorLinkedPicture WHERE SensorID = {0}";
        //    string szSQL = string.Format(szText, nPSMSensorID);


        //    ArrayList arResult = m_DbMgr.GetResultData(szSQL, 0);
        //    if (arResult == null || arResult.Count < 3)
        //        return;

        //    string szImgPath1 = WebDBManager.GetStringField(arResult[0], "");
        //    string szImgPath2 = WebDBManager.GetStringField(arResult[1], "");
        //    string szImgPath3 = WebDBManager.GetStringField(arResult[2], "");

        //    SetPSMMode(szImgPath1, szImgPath2, szImgPath3);
        //}

        public void SetViewerImage(int nView, Image imge)
        {
            if (imge == null)
                return;

            if (nView == 2)
            {
                try
                {
                    // 초기화
                    if (cctvForm.PictureBox2.BackgroundImage != null)
                        cctvForm.PictureBox2.BackgroundImage.Dispose();

                    if (UnE.SOP.ProxySOP.Instance.SiteID == 2)
                    {
                        cctvForm.PictureBox2.BackColor = Color.White;
                        cctvForm.PictureBox2.BackgroundImageLayout = ImageLayout.Center;

                        if (imge != null)
                            cctvForm.PictureBox2.BackgroundImage = (Image)new Bitmap(imge, 640, 300);

                        if (m_imgCompass != null)
                        {
                            cctvForm.PictureBox2.Image = m_imgCompass;
                        }
                    }
                    else
                    {
                        cctvForm.PictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
                        cctvForm.PictureBox2.BackgroundImage = imge;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                }
            }
            else if (nView == 3)
            {
                try
                {
                    // 초기화
                    if (cctvForm.PictureBox3.BackgroundImage != null)
                        cctvForm.PictureBox3.BackgroundImage.Dispose();

            
                    cctvForm.PictureBox3.BackgroundImageLayout = ImageLayout.Stretch;
                    cctvForm.PictureBox3.BackgroundImage = imge;                    
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                }
            }
            else if (nView == 4)
            {
                try
                {
                    // 초기화
                    if (cctvForm.PictureBox4.BackgroundImage != null)
                        cctvForm.PictureBox4.BackgroundImage.Dispose();

                    cctvForm.PictureBox4.BackgroundImageLayout = ImageLayout.Stretch;
                    cctvForm.PictureBox4.BackgroundImage = imge;
                   
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                }
            } 
        }

        private string m_szImagePath1 = "", m_strImageTitle1 = "";
        private string m_szImagePath2 = "", m_strImageTitle2 = "";
        private string m_szImagePath3 = "", m_strImageTitle3 = "";
        private string m_szImagePath4 = "", m_strImageTitle4 = "";

        public void SetViewerImage(int nView, string szPath, string strTitle)
        {
            if(nView == 1)
            {
                try
                {
                    // 초기화
                    cctvForm.PictureBox1.Title = "";
                    if (cctvForm.PictureBox1.BackgroundImage != null)
                    {
                        cctvForm.PictureBox1.BackgroundImage.Dispose();
                        cctvForm.PictureBox1.BackgroundImage = null;
                    }


                    if( System.IO.File.Exists(szPath))
                    {
                       
                        string strFilePath = GetCloneFilePath(szPath);

                        m_szImagePath1 = strFilePath;
                        m_strImageTitle1 = strTitle;
                        System.IO.File.Copy(szPath, strFilePath, true);

                        cctvForm.PictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
                        cctvForm.PictureBox1.BackgroundImage = (Image)(Image.FromFile(strFilePath));
                        cctvForm.PictureBox1.Title = strTitle;

                        try
                        {
                            System.IO.File.Delete(strFilePath);
                        }
                        catch(Exception ex)
                        {

                        }
                    }
                    else
                    {
                        if (cctvForm.PictureBox1.BackgroundImage != null)
                         cctvForm.PictureBox1.BackgroundImage.Dispose();
                        cctvForm.PictureBox1.BackgroundImage = null;
                        cctvForm.PictureBox1.Title = strTitle;
                    }
                                  
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                }

            }
            else if(nView == 2)
            {
                try
                {

                    //try
                    //{
                    //    if (cctvForm.PictureBox1.BackgroundImage != null)
                     //       cctvForm.PictureBox1.BackgroundImage.Dispose();
                     //   cctvForm.PictureBox1.BackgroundImage = null;                       
                    //}catch(Exception)
                    //{ }
                    // 초기화
                    cctvForm.PictureBox2.Title = "";
                    if (cctvForm.PictureBox2.BackgroundImage != null)
                    {
                        cctvForm.PictureBox2.BackgroundImage.Dispose();
                        cctvForm.PictureBox2.BackgroundImage = null;
                    }

                    if (System.IO.File.Exists(szPath))
                    {
                        string strFilePath = GetCloneFilePath(szPath);
                        m_szImagePath2 = strFilePath;
                        m_strImageTitle2 = strTitle;
                        System.IO.File.Copy(szPath, strFilePath, true);

                        if (UnE.SOP.ProxySOP.Instance.SiteID == 2)
                        {
                            Image imge = (Image)(Image.FromFile(strFilePath));
                            cctvForm.PictureBox2.BackColor = Color.White;
                            cctvForm.PictureBox2.BackgroundImageLayout = ImageLayout.Center;
                            cctvForm.PictureBox2.BackgroundImage = (Image)new Bitmap(imge, 640, 300);
                            cctvForm.PictureBox2.Title = strTitle;

                            if (m_imgCompass != null)
                            {
                                cctvForm.PictureBox2.Image = m_imgCompass;
                            }

                            try
                            {
                                System.IO.File.Delete(strFilePath);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        else
                        {
                            cctvForm.PictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
                            cctvForm.PictureBox2.BackgroundImage = (Image)(Image.FromFile(strFilePath));
                            cctvForm.PictureBox2.Title = strTitle;

                            try
                            {
                                System.IO.File.Delete(strFilePath);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    else
                    {
                        if (cctvForm.PictureBox2.BackgroundImage != null)
                            cctvForm.PictureBox2.BackgroundImage.Dispose();
                        cctvForm.PictureBox2.BackgroundImage = null;
                        cctvForm.PictureBox2.Title = strTitle;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                }
            }
            else if (nView == 3)
            {
                try
                {
                    // 초기화
                    cctvForm.PictureBox3.Title = "";
                    if (cctvForm.PictureBox3.BackgroundImage != null)
                    {
                        cctvForm.PictureBox3.BackgroundImage.Dispose();
                        cctvForm.PictureBox3.BackgroundImage = null;
                    }

                    if (System.IO.File.Exists(szPath))
                    {
                        string strFilePath = GetCloneFilePath(szPath);
                        m_szImagePath3 = strFilePath;
                        m_strImageTitle3 = strTitle;
                        System.IO.File.Copy(szPath, strFilePath, true);
                        Image imge = (Image)(Image.FromFile(strFilePath));

                        cctvForm.PictureBox3.BackgroundImageLayout = ImageLayout.Stretch;
                        cctvForm.PictureBox3.BackgroundImage = imge;
                        cctvForm.PictureBox3.Title = strTitle;

                        try
                        {
                            System.IO.File.Delete(strFilePath);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {
                        cctvForm.PictureBox3.BackgroundImage.Dispose();
                        cctvForm.PictureBox3.BackgroundImage = null;
                        cctvForm.PictureBox3.Title = strTitle;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                }
            }
            else if (nView == 4)
            {
                try
                {
                    // 초기화
                    cctvForm.PictureBox4.Title = "";
                    if (cctvForm.PictureBox4.BackgroundImage != null)
                    {
                        cctvForm.PictureBox4.BackgroundImage.Dispose();
                        cctvForm.PictureBox4.BackgroundImage = null;
                    }

                    if (System.IO.File.Exists(szPath))
                    {
                        string strFilePath = GetCloneFilePath(szPath);
                        m_szImagePath4 = strFilePath;
                        m_strImageTitle4 = strTitle;
                        System.IO.File.Copy(szPath, strFilePath, true);
                        Image imge = (Image)(Image.FromFile(strFilePath));

                        cctvForm.PictureBox4.BackgroundImageLayout = ImageLayout.Stretch;
                        cctvForm.PictureBox4.BackgroundImage = imge;
                        cctvForm.PictureBox4.Title = strTitle;

                        try
                        {
                            System.IO.File.Delete(strFilePath);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {
                        cctvForm.PictureBox4.BackgroundImage.Dispose();
                        cctvForm.PictureBox4.BackgroundImage = null;
                        cctvForm.PictureBox4.Title = strTitle;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                }
            }  
        }
         
        private string GetCloneFilePath(string strFilePath)
        {
            int nIndex = strFilePath.LastIndexOf('.');

            if (nIndex < 0)
                return strFilePath + "_clone";

            string strFileOrigin = strFilePath.Substring(0, nIndex);
            string strExt = strFilePath.Substring(nIndex);
            return strFileOrigin + "_clone" + strExt;
        }

        private int m_nLastHistoryID = -1;
        private int m_nPrevHistoryID = -2;
        public void SetHistoryID(int nHistoryID)
        {
            
            m_nPrevHistoryID = m_nLastHistoryID;                
            m_nLastHistoryID = nHistoryID;

        }

        private Zone m_TargetZone = null;
        public Zone TargetZone
        {
            get { return m_TargetZone; }
            set { m_TargetZone = value; }
        }

        public void SetTargetZone(int nZoneID)
        {
            if( nZoneID == -1)
            {
                m_TargetZone = null;
            }
            else
            {
                Zone zone = ZoneManager.Instance.GetZone(nZoneID);
                m_TargetZone = zone;

                CCTVFormFrame.Instance.DetectPosition.Text = m_TargetZone.DisplayText;
               
            }
        }

        public void SetTitle(string strTitle)
        {
            CCTVFormFrame.Instance.DetectPosition.Text = strTitle;
            CCTVFormFrame.Instance.DetectPosition.Visible = true;
        }

        public void EditEquipZoneCCTV(int nCCTVID)
        {
            CCTV cctv = CCTVManager.Instance.GetCCTV(nCCTVID);

            Form4CCTV.CCTV_POSITION pos = cctvForm.SetCCTV(cctv);
            //   public enum CCTV_POSITION { TL = 0, TM = 1, BM = 4, BL = 3, BR = 5, TR = 2, UNKNOWN = 6 }
            if (ProxyCCTV.Instance.EquipZoneCCTVMode && pos != Form4CCTV.CCTV_POSITION.UNKNOWN)
            {
                int nIdx = -1;
                //nIdx = (int)pos;

                //DB에서 로딩 순서는 TM, BM, BR, TR, TL, BL 이다 
                if (pos == Form4CCTV.CCTV_POSITION.TM)
                    nIdx = 0;
                else if (pos == Form4CCTV.CCTV_POSITION.BM)
                    nIdx = 1;
                else if (pos == Form4CCTV.CCTV_POSITION.BR)
                    nIdx = 2;
                else if (pos == Form4CCTV.CCTV_POSITION.TR)
                    nIdx = 3;
                else if (pos == Form4CCTV.CCTV_POSITION.TL)
                    nIdx = 4;
                else if (pos == Form4CCTV.CCTV_POSITION.BL)
                    nIdx = 5;

                if (nIdx >= 0 && nIdx < 6)
                {
                    EditEquipZoneCCTV editEquipZoneCCTV = CCTVManager.Instance.UpdateEquipZoneCCTV(nIdx, nCCTVID, ProxyCCTV.Instance.CurrentEquipZone);
                    if (editEquipZoneCCTV != null)
                        editEquipZoneCCTV.Update(FormMain.Instance.DBManager);
                }
            }
            
        }

        private void _EditEquipZoneCCTV(int nCCTVID, bool editCCTVMode)
        {
            CCTV cctv = CCTVManager.Instance.GetCCTV(nCCTVID);

            Form4CCTV.CCTV_POSITION pos = cctvForm.SetCCTV(cctv, false);

            if (editCCTVMode && pos != Form4CCTV.CCTV_POSITION.UNKNOWN)
            {
                int nIdx = -1;

                nIdx = (int)pos;
                if (pos == Form4CCTV.CCTV_POSITION.TM)
                    nIdx = 0;
                else if (pos == Form4CCTV.CCTV_POSITION.BM)
                    nIdx = 1;
                else if (pos == Form4CCTV.CCTV_POSITION.BR)
                    nIdx = 2;
                else if (pos == Form4CCTV.CCTV_POSITION.TR)
                    nIdx = 3;
                else if (pos == Form4CCTV.CCTV_POSITION.TL)
                    nIdx = 4;
                else if (pos == Form4CCTV.CCTV_POSITION.BL)
                    nIdx = 5;

                if (nIdx >= 0 && nIdx < 6)
                {
                    EditEquipZoneCCTV editEquipZoneCCTV = CCTVManager.Instance.UpdateEquipZoneCCTV(nIdx, nCCTVID, ProxyCCTV.Instance.CurrentEquipZone);
                    if (editEquipZoneCCTV != null)
                        editEquipZoneCCTV.Update(FormMain.Instance.DBManager);
                }
            }

        }

        public void SetCCTV(CCTV cctv)
        {
            if( m_Mode != CCTVMode.FIRESituation)
            {
                if (ProxyCCTV.Instance.EquipZoneCCTVMode)
                {
                    if (cctv == null)
                        EditEquipZoneCCTV(-1);
                    else
                        EditEquipZoneCCTV(cctv.ID);
                }
                else
                {
                    /*if (cctvForm != null)
                    {
                        cctvForm.SetCCTV(cctv);
                    }*/

                    // PSM List에서 CCTV 보기 모드
                    if (m_nSituationMode == 3)
                    {
                        if (cctv == null)
                            _EditEquipZoneCCTV(-1, true);
                        else
                            _EditEquipZoneCCTV(cctv.ID, true);
                    }
                    else
                    {
                        if (cctvForm != null)
                        {
                            cctvForm.SetCCTV(cctv);
                        }
                    }
                }

            }            
        }
            


        public void OnReciveMessage(string szMsg)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    // Run Command
                    ScriptProxy.Instance.RunPythonScript(szMsg);
                });
            }
            catch (Exception)
            {

            }           
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        public static void KillProcess(string strProcessName)
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName == strProcessName)
                {
                    process.Kill();
                }
            }
        }
        
        private void LogTimer_Tick(object sender, EventArgs e)
        {
            List<PythonLogger.Entry> logs = ScriptProxy.Instance.Logger.GetAll();            
            foreach(PythonLogger.Entry entry in logs)
            {
                logger.Debug("Python : " + entry.ToString());
            }
        }

        private Image m_imgCompass = null;
        public void LoadCompass()
        {
            try
            {
                if (m_imgCompass == null)
                {
                    string szMediaPath = FormMain.EnginPath() + "Media\\";
                    string szIconPath = szMediaPath + "models\\Compass.png";

                    Image temp = Image.FromFile(szIconPath);

                    m_imgCompass = (Image)new Bitmap(temp, 100, 100);
                }
            }
            catch (Exception)
            {
                m_imgCompass = null;
            }
        }

        public static string EnginPath()
        {
            string szMainPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
            string szWorkPath = szMainPath;
            if (File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;

            szWorkPath = szMainPath + "common\\";
            if (File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;

            szWorkPath = szMainPath + "SOP\\";
            if (File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;

            return szMainPath;
        }
    }
}
