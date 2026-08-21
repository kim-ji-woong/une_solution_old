using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using DidUIEditor;
using DidViewer.Composition;
using DidViewer.uCustomize;
using UnE.Sensor;
using System.Media;
using System.Diagnostics;
//using static UnE.Sensor.IFacility;

namespace DidViewer
{
    public enum Mode { Normal = 0, Emergency = 1, TraningMode = 2 /*훈련모드*/ }
    public enum EmergencyMode { Fire = 0, PSM, Earthquake, Space }

    public partial class FormMain : Form
    {
        private Mode m_Mode = Mode.Normal;
        public Mode Mode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }

        private WebServerManager m_webMgr = null;
        public WebServerManager WebMgr
        {
            get { return m_webMgr; }
            set { m_webMgr = value; }
        }

        private List<Page> m_haveNormalPages = new List<Page>();
        public List<Page> HaveNormalPages
        {
            get { return m_haveNormalPages; }
            set { m_haveNormalPages = value; }
        }

        private Dictionary<int, Panel> m_dicHaveNormalPages = new Dictionary<int, Panel>();
       
        private List<Page> m_haveEmergencyPages = new List<Page>();
        public List<Page> HaveEmergencyPages
        {
            get { return m_haveEmergencyPages; }
            set { m_haveEmergencyPages = value; }
        }

        private Dictionary<int, Panel> m_dicHaveEmergencyPages = new Dictionary<int, Panel>();
        private Dictionary<int, Panel> m_dicHaveTraningPages = new Dictionary<int, Panel>();

        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        private int m_nCurPageIndex = 0;
        private int m_nMaxPageIndex = 0;

        private string m_strLocalFilePath = Application.StartupPath + "\\Files";

        private Timer m_timer = null;
        private int m_nSec = 1;
        private Page m_curPage = null;
        private EmergencyPage m_curEmergencyPage = null;

        private int m_nSecCheckVersion = 10;
        private int m_nSec2 = 1;
        private int m_nSecEmergency = 5;

        private int m_nCurVersion = 0;

        private Image m_imgBack = null;

        private int m_nSiteID = 3;

        private IrisScanManager m_irisScanMgr = null;
        private WebDBManager m_dbMgr = null;        

        private SoundPlayer m_sound = null;
        private bool m_bSound = false;

        public FormMain()
        {
            this.SetStyle(System.Windows.Forms.ControlStyles.UserPaint, true);
            this.SetStyle(System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer | System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(System.Windows.Forms.ControlStyles.EnableNotifyMessage, true);

            InitializeComponent();

            m_instance = this;

            this.Size = new Size(1920, 1080);
            
            m_webMgr = new WebServerManager();
            m_webMgr.LocalFilePath = m_strLocalFilePath;

            m_dbMgr = new WebDBManager(3);
            m_irisScanMgr = new IrisScanManager(m_dbMgr);
            
            m_sound = new SoundPlayer();
            m_sound.SoundLocation = Application.StartupPath + "\\FireSignalAlarm.WAV";            
            m_imgBack = global::DidViewer.Properties.Resources.BackgroundNormal;
            
            m_timer = new Timer();
            m_timer.Interval = 1000;
            m_timer.Tick += M_timer_Tick;
            m_timer.Start();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Loadini();
            CheckVersion(true);

            //m_dataMgr.LoadCompanyInfo();
            
            m_irisScanMgr.DisplayUser();
                        
            //MakeEmergencyPanel();
            
            InitCommonPanel();

            this.pnUI.Location = new Point(0, m_frmTop.Height);
            this.pnUI.Size = new Size(1920, 1080 - m_frmTop.Height - m_frmBottom.Height);
            this.pnUIEmergency.Location = new Point(0, m_frmTop.Height);
            this.pnUIEmergency.Size = new Size(1920, 1080 - m_frmTop.Height - m_frmBottom.Height);
            this.pnUITraning.Location = new Point(0, m_frmTop.Height);
            this.pnUITraning.Size = new Size(1920, 1080 - m_frmTop.Height - m_frmBottom.Height);

            LoadXML();

            MakeTraningPanel();
            RefreshDisaster();

            if (m_Mode == Mode.Normal)
                SetMode(Mode.Normal, true);
            //m_irisScanMgr.DisplayMember();

            RefreshPage();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            //FormEffect(this);
        }

        private void FormEffect(Form fm)
        {
            double[] opacity = new double[] { 0.1d, 0.3d, 0.7d, 0.8d, 0.9d, 1.0d };
            int cnt = 0;
            System.Windows.Forms.Timer tm = new System.Windows.Forms.Timer();
            {
                fm.RightToLeftLayout = false;
                fm.Opacity = 0d;
                tm.Interval = 200;   // 나타나는 속도를 조정함.          
                tm.Tick += delegate (object obj, EventArgs e)
                {
                    if ((cnt + 1 > opacity.Length) || fm == null)
                    {
                        tm.Stop();
                        tm.Dispose();
                        tm = null;
                        return;
                    }
                    else
                    {
                        fm.Opacity = opacity[cnt++];
                    }
                };
                tm.Start();
            }
        }

        public void SetDoubleBuffer(Panel panel, bool bEnabled)
        {
            Type dgvType1 = panel.GetType();
            System.Reflection.PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            pi1.SetValue(panel, bEnabled, null);
        }
        public void SetDoubleBuffer(PictureBox pic, bool bEnabled)
        {
            Type dgvType1 = pic.GetType();
            System.Reflection.PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            pi1.SetValue(pic, bEnabled, null);
        }

        public void SetDoubleBuffer(Label label, bool bEnabled)
        {
            Type dgvType1 = label.GetType();
            System.Reflection.PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            pi1.SetValue(label, bEnabled, null);
        }
        
        private void M_timer_Tick(object sender, EventArgs e)
        {            
            if (m_Mode == Mode.TraningMode)
            {

            }
            //if ((m_Mode == Mode.Normal && m_haveNormalPages.Count == 0) || (m_Mode != Mode.Normal && m_emergencyPages.Count == 0))
            //{
            //    pnUI.Controls.Clear();
                
            //    return;
            //}

            if (m_Mode == DidViewer.Mode.TraningMode && !m_bSound)
            {
                m_sound.PlayLooping();
                m_bSound = true;
            }
            if (m_Mode == DidViewer.Mode.Emergency && !m_bSound)
            {
                m_sound.PlayLooping();
                m_bSound = true;
            }
            if (m_Mode == DidViewer.Mode.Normal && m_bSound)
            {
                m_sound.Stop();
                m_bSound = false;
            }

            m_frmTop.HiCount = m_irisScanMgr.HiCount.ToString();
            m_frmTop.ByeCount = m_irisScanMgr.ByeCount.ToString();
            m_frmTop.StayCount = m_irisScanMgr.StayCount.ToString();

            DateTime dtNow = DateTime.Now;
            if (dtNow.Hour == 23 && dtNow.Minute == 59 && dtNow.Second == 59)
                Init();

            System.Globalization.CultureInfo cultures = System.Globalization.CultureInfo.CreateSpecificCulture("ko-KR");
            m_frmBottom.DateText = dtNow.ToString("yyyy년 MM월 dd일 ddd요일");
            m_frmBottom.TimeText = dtNow.ToString("HH:mm:ss");

            if (m_nSec2 >= m_nSecCheckVersion)
            {
                bool bRefreshXML = CheckVersion(false);
                if (bRefreshXML)
                {
                    m_timer.Stop();
                    m_nCurPageIndex = 0;

                    ReleaseMemory();

                    pnUI.Controls.Clear();

                    AllDeleteFile();

                    LoadXML();
                    RefreshPage();

                    m_timer.Start();
                }

                m_nSec2 = 1;
            }
            else
                m_nSec2++;

            m_irisScanMgr.DisplayUser();
            DisplayRealSpace();

            if (m_Mode == Mode.Normal && m_curPage == null)
                return;
            else if (m_Mode == Mode.Emergency && m_curEmergencyPage == null)
                return;

            int playSecond = (m_Mode == Mode.Normal) ? m_curPage.PlaySeconds : m_nSecEmergency;
            if (playSecond <= m_nSec)
            {
                if (m_Mode == Mode.Normal)
                {
                    if (m_curPage.strBackgroundIMG == "systemstyle1.png")
                    {
                        Panel panel = m_dicHaveNormalPages[m_nCurPageIndex];
                        foreach (Control ctrl in panel.Controls)
                        {
                            if (ctrl is uCompanyGroup)
                            {
                                m_nSec = 1;

                                uCompanyGroup companyGroup = ctrl as uCompanyGroup;
                                int lastIndex = -1;
                                companyGroup.MakePanel(m_irisScanMgr.DicCompanies, companyGroup.ViewCompanyInfoIndex, ref lastIndex);
                                companyGroup.ViewCompanyInfoIndex = lastIndex;

                                // 마지막 업체까지 보여줬는가 ?  
                                if (companyGroup.ViewCompanyInfoIndex < m_irisScanMgr.StayCompanyCount)
                                    return;
                                else
                                    break;                                
                            }
                        }                  
                        /*
                        if (m_curPage.ViewCompanyInfoIndex < m_irisScanMgr.StayCompanyCount)
                        {
                            Panel panel = m_dicHaveNormalPages[m_nCurPageIndex];
                            foreach (Control ctrl in panel.Controls)
                            {
                                if (ctrl is uCompanyGroup)
                                {
                                    uCompanyGroup companyGroup = ctrl as uCompanyGroup;

                                    int lastIndex = -1;
                                    companyGroup.MakePanel(m_irisScanMgr.DicCompanies, m_curPage.ViewCompanyInfoIndex, ref lastIndex);
                                    m_curPage.ViewCompanyInfoIndex = lastIndex;
                                    m_nSec = 1;
                                    break;
                                }
                            }
                            return;
                        }
                        else// if (m_curPage.ViewCompanyInfoIndex >= m_irisScanMgr.StayCompanyCount)
                        {
                            // 마지막까지 보여줬다면 첫번째로 돌려놓기
                            m_curPage.ViewCompanyInfoIndex = -1;

                            Panel panel = m_dicHaveNormalPages[m_nCurPageIndex];
                            foreach (Control ctrl in panel.Controls)
                            {
                                if (ctrl is uCompanyGroup)
                                {
                                    uCompanyGroup companyGroup = ctrl as uCompanyGroup;

                                    int lastIndex = -1;
                                    companyGroup.MakePanel(m_irisScanMgr.DicCompanies, m_curPage.ViewCompanyInfoIndex, ref lastIndex);
                                    m_curPage.ViewCompanyInfoIndex = lastIndex;
                                    m_nSec = 1;
                                    break;
                                }
                            }
                        }
                        */
                    } 
                }
                else if (m_Mode == Mode.Emergency)
                {
                    if (m_curEmergencyPage.Index == 0)
                    {                        
                        Panel panel = m_dicHaveEmergencyPages[m_nCurPageIndex];
                        foreach (Control ctrl in panel.Controls)
                        {
                            ctrl.Visible = true;
                            if (ctrl is uEmergency)
                            {
                                m_nSec = 1;

                                uEmergency companyGroup = ctrl as uEmergency;
                                // 마지막 업체까지 보여줬는가 ?
                                if (companyGroup.ViewCompanyInfoIndex < companyGroup.ArrShowInfo.Count / 3 && companyGroup.ViewCompanyInfoIndex > 0)
                                {
                                    companyGroup.SetCompanyData();
                                    return;
                                }
                                else //if (companyGroup.ViewCompanyInfoIndex >= companyGroup.ArrShowInfo.Count / 3 && companyGroup.ArrShowInfo.Count / 3 > 9)
                                {
                                    // 마지막까지 보여줬다면 첫번째로 돌려놓기                                    
                                    companyGroup.ViewCompanyInfoIndex = 0;
                                    companyGroup.SetCompanyData();                                    
                                }                                
                                break;
                            }
                        }
                    }
                }
                
                if (m_nCurPageIndex + 1 > m_nMaxPageIndex)
                    m_nCurPageIndex = 0;
                else
                    m_nCurPageIndex++;

                if ((m_Mode == Mode.Normal && m_haveNormalPages.Count > 1) ||
                    (m_Mode == Mode.Emergency && m_emergencyPages.Count > 1) ||
                     m_Mode == Mode.TraningMode)
                {
                    if (m_Mode == Mode.Normal) // 재난 화면에는 동영상이 안들어감
                    {
                        try
                        {
                            SetMovieStatus(m_curPage, true);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.Message);
                        }
                    }

                    SetCurrentPage();
                    RefreshPage(); 
                }

                m_nSec = 1;                
            }
            else
            {
                m_nSec++;
                RefreshDisaster();
            }         
        }

        private void LoadXML()
        {
            bool bXML = m_webMgr.Download("DID_UI.xml");
            if (bXML)
            {
                XMLManager xmlMgr = new XMLManager();
                xmlMgr.LoadXML();

                MakeNormalPanel();

                SetCurrentPage();
            }
        }

        private void MakeNormalPanel()
        {
            pnUI.Controls.Clear();

            m_dicHaveNormalPages.Clear();
            for (int i = 0; i < m_haveNormalPages.Count; i++)
            {
                Page page = m_haveNormalPages[i];

                Panel makePanel = CreatePagePanel(pnUI, page);
                m_dicHaveNormalPages[i] = makePanel;
                makePanel.Visible = false;
            }
        }

        private void MakeEmergencyPanel()
        {
            pnUIEmergency.Controls.Clear();
            m_dicHaveEmergencyPages.Clear();

            for (int i = 0; i < m_emergencyPages.Count; i++)
            {
                EmergencyPage page = m_emergencyPages[i];
                
                Panel makePanel = CreatePagePanel(page);
                makePanel.Name = page.EmergencyMode + "_" + page.Index;
                makePanel.Visible = false;
                m_dicHaveEmergencyPages[i] = makePanel;
            }

            if (m_Mode == Mode.Emergency)
                m_nCurPageIndex = 0;
        }

        private string m_strTraningText = "";
        private int m_nTraningEquipZoneID = -1;

        private void MakeTraningPanel()
        {
            pnUITraning.Controls.Clear();
            m_dicHaveTraningPages.Clear();

            for (int i = 1; i <= 7; i++)
            {
                Image img = null;
                
                switch (i)
                {
                    case 1:
                        break;
                    case 2:
                        img = global::DidViewer.Properties.Resources.Traning2;
                        break;
                    case 3:
                        img = global::DidViewer.Properties.Resources.Traning3;
                        break;
                    case 4:
                        img = global::DidViewer.Properties.Resources.Traning4;
                        break;
                    case 5:
                        img = global::DidViewer.Properties.Resources.Traning5;
                        break;
                    case 6:
                        img = global::DidViewer.Properties.Resources.Traning6;
                        break;
                    case 7:
                        img = global::DidViewer.Properties.Resources.Traning7;
                        break;
                }

                Panel pn = new Panel();
                pn.Location = new Point(0, 0);
                pn.Size = new Size(1920, 838);
                pn.Parent = pnUITraning;
                pn.Paint += (s, e) =>
                {
                    Image backImage = global::DidViewer.Properties.Resources.bg_fire_middle;
                    Graphics g = e.Graphics;
                    g.DrawImage(backImage, 0, 0, this.Width, this.Height);
                };

                Label label = new Label();
                label.Font = new System.Drawing.Font("나눔스퀘어 Bold", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                label.Location = new Point(39, 10);
                label.AutoSize = true;
                label.BackColor = Color.Transparent;
                label.Text = "";
                pn.Controls.Add(label);

                PictureBox pic = new PictureBox();
                pic.Image = img;                
                pic.Size = new Size(1839, 700);
                pic.Location = new Point(39, 122);
                pic.BackColor = Color.Transparent;
                pic.SizeMode = PictureBoxSizeMode.StretchImage;

                pn.Controls.Add(pic);

                m_dicHaveTraningPages.Add(i - 1, pn);
            }
        }

        private void SetCurrentPage()
        {
            if (m_Mode == Mode.Normal)
            {
                if (m_haveNormalPages.Count - 1 >= m_nCurPageIndex)
                {
                    m_curPage = m_haveNormalPages[m_nCurPageIndex];
                    m_nMaxPageIndex = m_haveNormalPages.Count - 1;
                }
                else
                    return;
            }
            else if (m_Mode == Mode.Emergency)
            {
                if (m_emergencyPages.Count - 1 >= m_nCurPageIndex)
                {
                    m_curEmergencyPage = m_emergencyPages[m_nCurPageIndex];
                    m_nMaxPageIndex = m_emergencyPages.Count - 1;
                }
                else
                    return;
            }
            else if (m_Mode == Mode.TraningMode)
            {
                if (m_dicHaveTraningPages.Count - 1 >= m_nCurPageIndex)
                {
                    //m_curEmergencyPage = m_emergencyPages[m_nCurPageIndex];
                    m_nMaxPageIndex = m_dicHaveTraningPages.Count - 1;
                }
                else
                    return;
            }
        }

        private bool CheckVersion(bool first)
        {
            m_webMgr.Download("did_update.txt");

            string txtUpdate = MakeMediaFilePath("did_update.txt");
            if (!File.Exists(txtUpdate))
                return false;

            string version = "";
            using (StreamReader sr = new StreamReader(txtUpdate))
            {
                version = sr.ReadLine();
            }

            int nVersion = 0;
            if (int.TryParse(version, out nVersion))
            {
                if (!first)
                {
                    if (m_nCurVersion < nVersion)
                    {
                        m_nCurVersion = nVersion;
                        return true;
                    }
                }
                else
                    m_nCurVersion = nVersion;
            }

            return false;
        }

        public void RefreshPage()
        {
            if (m_Mode == Mode.Normal && m_curPage == null)
                return;
            else if (m_Mode == Mode.Emergency && m_curEmergencyPage == null)
                return;

            if (IsUpdateIris)
                UpdateIris();

            Panel panel = null;
            if (m_Mode == Mode.Normal)
            {
                panel = m_dicHaveNormalPages[m_nCurPageIndex];
            
                if (panel == null)
                    return;

                if (m_curPage.strBackgroundIMG == "systemstyle1.png")
                {
                    foreach (Control ctrl in panel.Controls)
                    {
                        if (ctrl is uCompanyGroup)
                        {
                            uCompanyGroup companyGroup = ctrl as uCompanyGroup;

                            int lastIndex = -1;
                            bool showPage = true;
                            ArrayList arr = companyGroup.MakeData(m_irisScanMgr.DicCompanies);
                            if (arr.Count == 0)
                            {
                                if (m_nCurPageIndex + 1 > m_nMaxPageIndex)
                                    m_nCurPageIndex = 0;
                                else
                                    m_nCurPageIndex++;

                                SetCurrentPage();
                                RefreshPage();
                                m_nSec = 1;

                                return;
                            }

                            break;
                        }
                    }
                }

                for (int i = 0; i < pnUI.Controls.Count; i++)
                {
                    if (panel == pnUI.Controls[i])
                        pnUI.Controls[i].Visible = true;
                    else
                        pnUI.Controls[i].Visible = false;
                }

                //pnUI.Controls.Add(panel);
                try
                {
                    SetMovieStatus(m_curPage, false);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
                if (pnUIEmergency.Visible)
                    pnUIEmergency.Visible = false;
                if (pnUITraning.Visible)
                    pnUITraning.Visible = false;
                if (!pnUI.Visible)
                    pnUI.Visible = true;

                if (m_bgName.Length > 0)
                    m_bgName = "";
            }
            else if (m_Mode == Mode.Emergency)
            {
                panel = m_dicHaveEmergencyPages[m_nCurPageIndex];

                if (panel == null)
                    return;
                
                if (m_bgName != m_curEmergencyPage.EmergencyMode.ToString()) // 배경 바꾸면 자꾸 깜빡거림
                {
                    if (m_frmTop == null || m_frmBottom == null)
                        InitCommonPanel();

                    m_bgName = m_curEmergencyPage.EmergencyMode.ToString();
                    if (m_curEmergencyPage.EmergencyMode == EmergencyMode.Fire)
                    {
                        //m_frmTop.BackgroundImage = global::DidViewer.Properties.Resources.bg_fire_top;
                        //m_frmBottom.BackgroundImage = global::DidViewer.Properties.Resources.bg_fire_bottom;

                        m_frmTop.BackImage = global::DidViewer.Properties.Resources.bg_fire_top;
                        m_frmTop.Refresh();
                        m_frmBottom.BackImage = global::DidViewer.Properties.Resources.bg_fire_bottom;
                        m_frmBottom.Refresh();
                    }
                    else if (m_curEmergencyPage.EmergencyMode == EmergencyMode.PSM)
                    {
                        //m_frmTop.BackgroundImage = global::DidViewer.Properties.Resources.bg_psm_top;
                        //m_frmBottom.BackgroundImage = global::DidViewer.Properties.Resources.bg_psm_bottom;

                        m_frmTop.BackImage = global::DidViewer.Properties.Resources.bg_psm_top;
                        m_frmTop.Refresh();
                        m_frmBottom.BackImage = global::DidViewer.Properties.Resources.bg_psm_bottom;
                        m_frmBottom.Refresh();
                    }
                    else if (m_curEmergencyPage.EmergencyMode == EmergencyMode.Space)
                    {
                        //m_frmTop.BackgroundImage = global::DidViewer.Properties.Resources.bg_space_top;
                        //m_frmBottom.BackgroundImage = global::DidViewer.Properties.Resources.bg_space_bottom;

                        m_frmTop.BackImage = global::DidViewer.Properties.Resources.bg_space_top;
                        m_frmTop.Refresh();
                        m_frmBottom.BackImage = global::DidViewer.Properties.Resources.bg_space_bottom;
                        m_frmBottom.Refresh();
                    }
                    else if (m_curEmergencyPage.EmergencyMode == EmergencyMode.Earthquake)
                    {
                        //m_frmTop.BackgroundImage = global::DidViewer.Properties.Resources.bg_earthquake_top;
                        //m_frmBottom.BackgroundImage = global::DidViewer.Properties.Resources.bg_earthquake_bottom;

                        m_frmTop.BackImage = global::DidViewer.Properties.Resources.bg_earthquake_top;
                        m_frmTop.Refresh();
                        m_frmBottom.BackImage = global::DidViewer.Properties.Resources.bg_earthquake_bottom;
                        m_frmBottom.Refresh();
                    }
                }

                for (int i = 0; i < pnUIEmergency.Controls.Count; i++)
                {
                    if (panel == pnUIEmergency.Controls[i])
                        pnUIEmergency.Controls[i].Visible = true;
                    else
                        pnUIEmergency.Controls[i].Visible = false;
                }
                //pnUI.Controls.Add(panel);
                if (pnUI.Visible)
                    pnUI.Visible = false;
                if (!pnUIEmergency.Visible)
                    pnUIEmergency.Visible = true;
                if (pnUITraning.Visible)
                    pnUITraning.Visible = false;

            }
            else if (m_Mode == Mode.TraningMode)
            {
                panel = m_dicHaveTraningPages[m_nCurPageIndex];

                if (panel == null)
                    return;

                if (m_bgName != "화재")
                {
                    m_bgName = "화재";
                    
                    m_frmTop.BackImage = global::DidViewer.Properties.Resources.bg_fire_top;
                    m_frmTop.Refresh();
                    m_frmBottom.BackImage = global::DidViewer.Properties.Resources.bg_fire_bottom;
                    m_frmBottom.Refresh();
                }

                for (int i = 0; i < pnUITraning.Controls.Count; i++)
                {
                    if (panel == pnUITraning.Controls[i])
                        pnUITraning.Controls[i].Visible = true;
                    else
                        pnUITraning.Controls[i].Visible = false;
                }
                
                if (pnUIEmergency.Visible)
                    pnUIEmergency.Visible = false;
                if (!pnUITraning.Visible)
                    pnUITraning.Visible = true;
                if (!pnUI.Visible)
                    pnUI.Visible = false;
            }
        }

        private string m_bgName = "";

        private void SetMode(Mode mode, bool isLoad = false)
        {
            if (!isLoad)
            {
                if (m_Mode == mode)
                    return;
            }

            if (mode == Mode.Normal && (m_Mode == Mode.Emergency || m_Mode == Mode.TraningMode))
            {
                // 평상시 배경으로 변경
                m_frmTop.BackImage = global::DidViewer.Properties.Resources.bg_normal_top;
                m_frmTop.Refresh();
                m_frmBottom.BackImage = global::DidViewer.Properties.Resources.bottom_bg;
                m_frmBottom.Refresh();
            }

            m_Mode = mode;
        }

        private uTop m_frmTop = null;
        private uBottom m_frmBottom = null;

        private void InitCommonPanel()
        {
            //Top, Bottom 공통으로 쓰는 패널 초기화

            m_frmTop = new uTop();
            m_frmTop.Location = new Point(0, 0);
            m_frmTop.BackgroundImageLayout = ImageLayout.Stretch;
            //m_frmTop.BackColor = Color.Transparent;
            this.Controls.Add(m_frmTop);

            m_frmTop.MouseDown += m_frmTop_MouseDown;
            m_frmTop.MouseMove += m_frmTop_MouseMove;
            m_frmTop.MouseUp += m_frmTop_MouseUp;
            m_frmTop.MouseDoubleClick += m_frmTop_MouseDoubleClick;

            m_frmBottom = new uBottom();
            m_frmBottom.Location = new Point(0, 999);
            m_frmBottom.BackgroundImageLayout = ImageLayout.Stretch;
            this.Controls.Add(m_frmBottom);
        }

        #region Form 이동 변수
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptOrigin = new Point();
        #endregion

        #region 폼 이동
        void m_frmTop_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                SetWindowPosition(this);
            }
        }

        void m_frmTop_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;

            m_isClicked = false;
        }

        void m_frmTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_isClicked)
                return;

            if (!m_bLeftMouseDown)
                return;

            Point ptScreen = Control.MousePosition;

            int dx = ptScreen.X - m_ptMove.X;
            int dy = ptScreen.Y - m_ptMove.Y;

            if (dx == 0 && dy == 0)
                return;

            Point ptCur = this.Location;
            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            m_ptMove.X += dx;
            m_ptMove.Y += dy;
        }

        void m_frmTop_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
                m_ptOrigin = this.Location;
            }

            m_isClicked = true;
        }

        public static bool SetWindowPosition(Form frm)
        {
            if (SetWindowPosition(frm, frm.Location))
                return true;

            Point ptBL = new Point(frm.Location.X, frm.Location.Y + frm.Size.Height);
            Point ptTR = new Point(frm.Location.X + frm.Size.Width, frm.Location.Y);
            Point ptBR = new Point(frm.Location.X + frm.Size.Width, frm.Location.Y + frm.Size.Height);
            Point ptMiddle = new Point((frm.Location.X + ptBR.X) / 2, (frm.Location.Y + ptBR.Y) / 2);

            if (SetWindowPosition(frm, ptBL))
                return true;
            if (SetWindowPosition(frm, ptTR))
                return true;
            if (SetWindowPosition(frm, ptBR))
                return true;
            if (SetWindowPosition(frm, ptMiddle))
                return true;

            return false;
        }

        private static bool SetWindowPosition(Form frm, Point pt)
        {
            foreach (Screen sc in Screen.AllScreens)
            {
                if (pt.X >= sc.Bounds.Left && pt.X <= sc.Bounds.Right &&
                    pt.Y >= sc.Bounds.Top && pt.Y <= sc.Bounds.Bottom)
                {
                    frm.Location = new Point(sc.Bounds.Left, sc.Bounds.Top);
                    return true;
                }
            }

            return false;
        } 
        #endregion

        private Panel CreatePagePanel(Panel pnParent, Page page)
        {
            uPanel pn = new uPanel();
            pn.BackgroundImageLayout = ImageLayout.Stretch;
            pn.Name = page.Name;
            pn.Page = page;
            pn.Parent = pnParent;

            if (page.PageType == PageType.System)
            {
                if (page.strBackgroundIMG == "systemstyle1.png")
                {
                    int lastIndex = -1;
                    uCompanyGroup companyGroup = new uCompanyGroup();
                    companyGroup.MakePanel(m_irisScanMgr.DicCompanies, -1, ref lastIndex);
                    companyGroup.ViewCompanyInfoIndex = lastIndex;
                    companyGroup.Location = new Point(40, 40);
                    pn.Controls.Add(companyGroup);
                }

                if (page.strBackgroundIMG != "systemstyle0.png" && page.strBackgroundIMG != "systemstyle1.png" && page.strBackgroundIMG != "systemstyle8.png")
                {
                    Image img1 = null;
                    Image img2 = null;
                    GetContentImage(page.strBackgroundIMG, ref img1, ref img2);

                    PictureBox pic1 = new PictureBox();
                    PictureBox pic2 = new PictureBox();
                    pic1.Location = new Point(40, 40);
                    pic2.Location = new Point(970, 40);
                    pic1.Size = pic2.Size = new Size(910, 768);
                    pic1.BackColor = pic2.BackColor = Color.Transparent;
                    pic1.SizeMode = pic2.SizeMode = PictureBoxSizeMode.StretchImage;
                    pic1.Image = img1;
                    pic2.Image = img2;

                    SetDoubleBuffer(pic1, true);
                    SetDoubleBuffer(pic2, true);

                    pn.Controls.Add(pic1);
                    pn.Controls.Add(pic2);
                }

                pn.BackgroundImage = m_imgBack;
            }

            if (page.strBackgroundIMG != null && page.strBackgroundIMG.Length > 0)
            {
                string filePath = MakeMediaFilePath(page.strBackgroundIMG);
                if (File.Exists(filePath))
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open))
                    {
                        Image img = Image.FromStream(fs);  //Image.FromFile(filePath);
                        pn.BackgroundImage = img;
                        pn.BackgroundImageLayout = ImageLayout.Stretch;
                    }
                }
            }

            if (pnParent == pnUI)
            {
                pn.Size = pnParent.Size;
                pn.Location = new Point(0, 0);
            }
            else
            {
                pn.Size = page.PageSize;
                pn.Location = page.PageLocation;
                pn.BackColor = Color.Yellow;
            }

            foreach (Page child in page.ChildPages)
            {
                CreatePagePanel(pn, child);
            }

            foreach (Media media in page.Medias)
            {
                uPanel pnMedia = CreateMediaPanel(media);
                pnMedia.Parent = pn;
                pnMedia.BringToFront();
            }

            
            SetDoubleBuffer(pn, true);

            return pn;
        }

        private uPanel CreateMediaPanel(Media media)
        {
            uPanel pnMedia = new uPanel();
            pnMedia.Page.PageType = PageType.None;
            pnMedia.Page.Medias.Add(media);
            pnMedia.Size = media.MediaSize;
            pnMedia.Location = new Point(media.MediaLocation.X, media.MediaLocation.Y - m_frmTop.Height);
            pnMedia.BackgroundImageLayout = ImageLayout.Stretch;
            pnMedia.BackColor = Color.Transparent;

            string filePath = MakeMediaFilePath(media.File);

            if (media.MediaType == MediaType.Image)
            {
                if (File.Exists(filePath))
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open))
                    {
                        Image img = Image.FromStream(fs);  //Image.FromFile(filePath);
                        pnMedia.BackgroundImage = img;                        
                    }
                }
            }
            else if (media.MediaType == MediaType.Movie)
            {
                if (File.Exists(filePath))
                {
                    if (media.Player == null)
                        media.Player = new AxWMPLib.AxWindowsMediaPlayer();

                    media.Player.Size = pnMedia.Size;
                    media.Player.Location = new Point(0, 0);
                    pnMedia.Controls.Add(media.Player);

                    media.SetPlayer(true, filePath);
                }
            }

            SetDoubleBuffer(pnMedia, true);

            return pnMedia;
        }

        private Panel CreatePagePanel(EmergencyPage page)
        {
            Panel panel = new Panel();
            panel.Location = new Point(0, 0);
            panel.Size = new Size(1920, 838);
            panel.Tag = page;
            panel.Parent = pnUIEmergency;
            panel.Paint += (s, e) =>
            {
                Image backImage = null;
                if (page.EmergencyMode == EmergencyMode.Fire)
                    backImage = global::DidViewer.Properties.Resources.bg_fire_middle;
                else if (page.EmergencyMode == EmergencyMode.PSM)
                    backImage = global::DidViewer.Properties.Resources.bg_psm_middle;
                else if (page.EmergencyMode == EmergencyMode.Space)
                    backImage = global::DidViewer.Properties.Resources.bg_space_middle;
                else if (page.EmergencyMode == EmergencyMode.Earthquake)
                    backImage = global::DidViewer.Properties.Resources.bg_earthquake_middle;

                if (backImage == null)
                    return;

                Graphics g = e.Graphics;
                g.DrawImage(backImage, 0, 0, this.Width, this.Height);
            };

            //if (page.EmergencyMode == EmergencyMode.Fire)
            //    panel.BackgroundImage = global::DidViewer.Properties.Resources.bg_fire_middle;
            //else if (page.EmergencyMode == EmergencyMode.PSM)
            //    panel.BackgroundImage = global::DidViewer.Properties.Resources.bg_psm_middle;
            //else if (page.EmergencyMode == EmergencyMode.Space)
            //    panel.BackgroundImage = global::DidViewer.Properties.Resources.bg_space_middle;
            //else if (page.EmergencyMode == EmergencyMode.Earthquake)
            //    panel.BackgroundImage = global::DidViewer.Properties.Resources.bg_earthquake_middle;

            Label label = new Label();
            label.Font = new System.Drawing.Font("나눔스퀘어 Bold", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            label.Location = new Point(39, 10);
            label.AutoSize = true;
            label.BackColor = Color.Transparent;
            label.Text = page.strMessage;
            if (page.EmergencyMode == EmergencyMode.Space)
                label.ForeColor = Color.White;
            panel.Controls.Add(label);

            if (page.Index == 0)
            {
                if (!File.Exists(m_strLocalFilePath + "\\Outdoor\\" + page.nEquipmentZoneID + ".png"))
                {
                    m_webMgr.DownloadOutdoor(page.nEquipmentZoneID + ".png");
                }

                if (File.Exists(m_strLocalFilePath + "\\Outdoor\\" + page.nEquipmentZoneID + ".png"))
                {
                    PictureBox pic = new PictureBox();
                    pic.Location = new Point(39, 122);
                    pic.Size = new Size(919, 754);
                    pic.SizeMode = PictureBoxSizeMode.StretchImage;

                    pic.Image = Image.FromFile(m_strLocalFilePath + "\\Outdoor\\" + page.nEquipmentZoneID + ".png");
                                        
                    panel.Controls.Add(pic);
                }

                uEmergency uFrm = new uEmergency(page.EmergencyMode, page.ArrShowInfo);
                uFrm.Location = new Point(1020, 82);
                uFrm.Show();
                panel.Controls.Add(uFrm);
            }
            else
            {
                string bDownloadFilePath = m_strLocalFilePath + "\\Escape\\" + page.nEquipmentZoneID + ".jpg";
                if (!File.Exists(bDownloadFilePath))
                {

                    bool bSuc = m_webMgr.DownloadEscape(page.nEquipmentZoneID + ".jpg");
                    if (!bSuc)
                    {
                        bSuc = m_webMgr.DownloadEscape(page.nEquipmentZoneID + ".png");
                        if (bSuc)
                            bDownloadFilePath = m_strLocalFilePath + "\\Escape\\" + page.nEquipmentZoneID + ".png";
                    }
                }

                if (File.Exists(bDownloadFilePath))
                {
                    PictureBox pic = new PictureBox();
                    pic.Location = new Point(214, 82);
                    pic.Size = new Size(1491, 757);
                    pic.Image = Image.FromFile(bDownloadFilePath);

                    pic.SizeMode = PictureBoxSizeMode.StretchImage;
                    panel.Controls.Add(pic);
                }
            }

            return panel;
        }

        private Image GetLogoImage(string systemPageName)
        {
            Image img = null;
            if (systemPageName == "systemstyle1.png")
                img = global::DidViewer.Properties.Resources.SystemLogo1;
            else if (systemPageName == "systemstyle8.png")
                img = global::DidViewer.Properties.Resources.systemLogo3;
            else
                img = global::DidViewer.Properties.Resources.systemLogo2;

            return img;
        }

        private Image GetTextImage(string systemPageName, ref Size size)
        {
            Image img = null;
            if (systemPageName == "systemstyle1.png")
            {
                img = global::DidViewer.Properties.Resources.systemText_현장_근로자_현황;
                size = new Size(282, 49);
            }
            else if (systemPageName == "systemstyle2.png")
            {
                img = global::DidViewer.Properties.Resources.systemText_일반위험_작업;
                size = new Size(506, 49);
            }
            else if (systemPageName == "systemstyle3.png")
            {
                img = global::DidViewer.Properties.Resources.systemText_화재_작업;
                size = new Size(431, 49);
            }
            else if (systemPageName == "systemstyle4.png")
            {
                img = global::DidViewer.Properties.Resources.systemText_정전_작업;
                size = new Size(431, 49);
            }
            else if (systemPageName == "systemstyle5.png")
            {
                img = global::DidViewer.Properties.Resources.systemText_밀폐공간_작업;
                size = new Size(506, 49);
            }
            else if (systemPageName == "systemstyle6.png")
            {
                img = global::DidViewer.Properties.Resources.systemText_고소_작업;
                size = new Size(431, 49);
            }
            else if (systemPageName == "systemstyle7.png")
            {
                img = global::DidViewer.Properties.Resources.systemText_굴착_작업;
                size = new Size(431, 49);
            }
            else if (systemPageName == "systemstyle8.png")
            {
                img = global::DidViewer.Properties.Resources.systemText_방재장비_배치도;
                size = new Size(272, 49);
            }

            return img;
        }

        private void GetContentImage(string systemPageName, ref Image img1, ref Image img2)
        {
            switch (systemPageName)
            {
                case "systemstyle2.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_2_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_2_2;
                    break;
                case "systemstyle3.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_3_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_3_2;
                    break;
                case "systemstyle4.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_4_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_4_2;
                    break;
                case "systemstyle5.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_5_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_5_2;
                    break;
                case "systemstyle6.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_6_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_6_2;
                    break;
                case "systemstyle7.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_7_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_7_2;
                    break;
                //case "systemstyle8.png":
                //    img1 = global::DidViewer.Properties.Resources.systemImg_8_1;
                //    img2 = global::DidViewer.Properties.Resources.systemImg_8_2;
                //    break;
                case "systemstyle9.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_9_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_9_2;
                    break;
                case "systemstyle10.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_10_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_10_2;
                    break;
                case "systemstyle11.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_11_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_11_2;
                    break;
                case "systemstyle12.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_12_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_12_2;
                    break;
                case "systemstyle13.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_13_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_13_2;
                    break;
                case "systemstyle14.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_14_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_14_2;
                    break;
                case "systemstyle15.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_15_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_15_2;
                    break;
                case "systemstyle16.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_16_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_16_2;
                    break;
                case "systemstyle17.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_17_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_17_2;
                    break;
                case "systemstyle18.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_18_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_18_2;
                    break;
                case "systemstyle19.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_19_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_19_2;
                    break;
                case "systemstyle20.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_20_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_20_2;
                    break;
                case "systemstyle21.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_21_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_21_2;
                    break;
                case "systemstyle22.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_22_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_22_2;
                    break;
                case "systemstyle23.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_23_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_23_2;
                    break;
                case "systemstyle24.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_24_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_24_2;
                    break;
                case "systemstyle25.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_25_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_25_2;
                    break;
                case "systemstyle26.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_26_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_26_2;
                    break;
                case "systemstyle27.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_27_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_27_2;
                    break;
                case "systemstyle28.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_28_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_28_2;
                    break;
                case "systemstyle29.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_29_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_29_2;
                    break;
                case "systemstyle30.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_30_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_30_2;
                    break;
                case "systemstyle31.png":
                    img1 = global::DidViewer.Properties.Resources.systemImg_31_1;
                    img2 = global::DidViewer.Properties.Resources.systemImg_31_2;
                    break;
            }
        }

        public string MakeMediaFilePath(string fileName)
        {
            if (!Directory.Exists(m_strLocalFilePath))
                Directory.CreateDirectory(m_strLocalFilePath);

            return m_strLocalFilePath + "\\" + fileName;
        }

        private void SetMovieStatus(Page page, bool stop)
        {
            foreach (Media media in page.Medias)
            {
                if (media.MediaType == MediaType.Movie)
                {
                    if (stop)
                        media.Player.Ctlcontrols.stop();
                    else
                    {
                        media.Player.Ctlcontrols.currentPosition = media.BeginSeconds;
                        media.Player.Ctlcontrols.play();

                    }
                }
            }

            foreach (Page item in page.ChildPages)
            {
                foreach (Page item2 in item.ChildPages)
                {
                    SetMovieStatus(item2, stop);
                }
            }
        }

        /// <summary>
        /// Viewer version 업데이트를 체크할 시간
        /// Read NpgSql connString
        /// </summary>
        private void Loadini()
        {
            DBUtility2.Utility util = new DBUtility2.Utility();
            string path = util.getinivalue("Setting", "CheckVersionSecond");
            if (path != null && path.Length > 0)
            {
                int sec;
                if (int.TryParse(path, out sec))
                    m_nSecCheckVersion = sec;
            }

            string siteID = util.getinivalue("Server Connection Info", "siteid");
            if (siteID != null && siteID.Length > 0)
            {
                int nSiteID;
                if (int.TryParse(siteID, out nSiteID))
                    m_nSiteID = nSiteID;
            }
        }

        private void ReleaseMemory()
        {
            for (int j = m_curPage.Medias.Count - 1; j >= 0; j--)
            {
                Media media = m_curPage.Medias[j];
                if (media.MediaType == MediaType.Movie)
                {
                    if (media.Player != null)
                    {
                        media.Player.Ctlcontrols.stop();

                        //media.Player.URL = "";
                        //media.Player.URL = null;
                        //media.Player.currentPlaylist.clear();
                        
                        media.Player.close();                        
                        media.Player.Dispose();
                        media.Player = null;
                    }
                }
            }
            m_curPage = null;
            
            for (int i = m_haveNormalPages.Count - 1; i >= 0; i--)
            {
                Page page = m_haveNormalPages[i];
                for (int j = page.Medias.Count - 1; j >= 0; j--)
                {
                    Media media = page.Medias[j];
                    if (media.MediaType == MediaType.Movie)
                    {
                        if (media.Player != null)
                        {
                            media.Player.URL = null;
                            media.Player.currentPlaylist.clear();
                            media.Player.close();
                            
                            media.Player.Dispose();
                            media.Player = null;
                        }
                    }
                }
            }

            for (int i = m_dicHaveNormalPages.Count - 1; i >= 0; i--)
            {
                Panel panel = m_dicHaveNormalPages[i];
                CtrlDispose(panel);
            }

            for (int i = m_haveEmergencyPages.Count - 1; i >= 0; i--)
            {
                Page page = m_haveEmergencyPages[i];
                for (int j = page.Medias.Count - 1; j >= 0; j--)
                {
                    Media media = page.Medias[j];
                    if (media.MediaType == MediaType.Movie)
                    {
                        if (media.Player != null)
                        {
                            //System.Runtime.InteropServices.Marshal.FinalReleaseComObject(media.Player);

                            media.Player.URL = null;
                            media.Player.currentPlaylist.clear();
                            media.Player.close();
                            media.Player.Dispose();
                            media.Player = null;
                        }
                    }
                }
            }

            for (int i = m_dicHaveEmergencyPages.Count - 1; i >= 0; i--)
            {
                Panel panel = m_dicHaveEmergencyPages[i];
                CtrlDispose(panel);
            }

            m_haveNormalPages.Clear();
            m_haveEmergencyPages.Clear();
            m_dicHaveNormalPages.Clear();
            m_dicHaveEmergencyPages.Clear();

            CtrlDispose(pnUI);
        }

        private void CtrlDispose(Control parentCtrl)
        {
            for (int i = parentCtrl.Controls.Count - 1; i >= 0; i--)
            {
                Control ctrl = parentCtrl.Controls[i];
                if (ctrl is uPanel)
                {
                    uPanel panel = ctrl as uPanel;
                    for (int j = panel.Page.Medias.Count - 1; j >= 0; j--)
                    {
                        Media media = panel.Page.Medias[j];
                        if (media.MediaType == MediaType.Movie)
                        {
                            if (media.Player != null)
                            {
                                //System.Runtime.InteropServices.Marshal.FinalReleaseComObject(media.Player);

                                media.Player.URL = null;
                                media.Player.currentPlaylist.clear();
                                media.Player.close();                                
                                media.Player.Dispose();
                                media.Player = null;                                
                            }
                        }
                        //else
                        //{
                            if (panel.BackgroundImage != null)
                            {
                                panel.BackgroundImage.Dispose();
                                panel.BackgroundImage = null;
                            }
                            
                        //}
                    }
                }

                CtrlDispose(ctrl);

                if (ctrl.BackgroundImage != null)
                {
                    ctrl.BackgroundImage.Dispose();
                    ctrl.BackgroundImage = null;
                }
            }

            if (parentCtrl != pnUI)
            {
                parentCtrl.Dispose();
                parentCtrl = null;
            }
        }

        List<string> m_downloadList = new List<string>();
        public List<string> DownloadList
        {
            get { return m_downloadList; }
            set { m_downloadList = value; }
        }

        private void AllDeleteFile()
        {
            foreach (string item in m_downloadList)
            {
                if (File.Exists(item))
                    File.Delete(item);
            }

            m_downloadList.Clear();
        }

        // 현재 Alarm이 발생중인 SensorReactionLog에 대한 Query 조건문
        private string GetAlarmReactionHistoryQueryString()
        {
            string strCondition = ((int)libSensorProcess.ReactionType.BEGIN_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.NOTIFY_SIGNAL).ToString();

            return "(" + strCondition + ")";
        }

        //현재 Alarm이 꺼진 SensorReactionLog에 대한 Query조건문
        private string GetAlarmOffReactionHistoryQueryString()
        {
            string strCondition = ((int)libSensorProcess.ReactionType.MALFUNCTION).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.IGNORE_SIGNAL).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.IGNORE_SOP).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.END_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.USER_RESET).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.TIME_OUT).ToString();

            return "(" + strCondition + ")";
        }

        private Dictionary<EmergencyMode, EmergencyPage> m_dicSensorIds = new Dictionary<EmergencyMode, EmergencyPage>();
        private void RefreshDisaster()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, szh.SensorID, szh.Param3 ");
            sb.Append("     , (select Type from sensorzone as z where z.id = srh.param2) as type ");
            sb.Append("  FROM SensorReactionHistory as srh ");
            sb.Append("INNER JOIN  SensorZoneHistory as szh on srh.SensorHistoryID = szh.ID ");
            sb.Append("WHERE SensorHistoryID in (SELECT srh2.SensorHistoryID FROM SensorReactionHistory as srh2 WHERE srh2.ReactionType in " + GetAlarmReactionHistoryQueryString() + " ) ");
            sb.Append(" AND SensorHistoryID not in (SELECT srh3.SensorHistoryID FROM SensorReactionHistory as srh3 WHERE srh3.ReactionType in " + GetAlarmOffReactionHistoryQueryString() + " ) ");
            sb.Append(" AND szh.SiteID = " + m_nSiteID.ToString());
            sb.Append(" ORDER BY srh.Time, szh.SensorID");
            
            ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString(), 0);

            if (arrResult == null)
                return;

            Dictionary<EmergencyMode, EmergencyPage> dicEmergencyModes = new Dictionary<EmergencyMode, EmergencyPage>();

            int nResultCount = arrResult.Count;
            if (nResultCount > 0)
            {
                //SetMode(Mode.Normal);
                //return;
                
                DateTime dtDefault = new DateTime();

                int nSensorID = -1;

                SortedList<int, int> keyExistList = new SortedList<int, int>();



                DateTime dtNow = DateTime.Now;
                DateTime dt24 = dtNow.AddHours(-24.0);

                for (int i = 0; i < nResultCount - 11; i += 13)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    int nReactionType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                    DateTime time = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                    string strMessage = WebDBManager.GetStringField(arrResult[i + 4], "");
                    int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                    string strParam2 = WebDBManager.GetStringField(arrResult[i + 6], "");
                    string strParam3 = WebDBManager.GetStringField(arrResult[i + 7], "");
                    string strParam4 = WebDBManager.GetStringField(arrResult[i + 8], "");
                    string strParam5 = WebDBManager.GetStringField(arrResult[i + 9], "");

                    if (nID < 0 || nHistoryID < 0)
                        continue;

                    nSensorID = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);

                    if (nReactionType == (int)libSensorProcess.ReactionType.BEGIN_STATUS || nReactionType == (int)libSensorProcess.ReactionType.CHANGE_ALARM_DEPTH)
                    {
                        nSensorID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                    }

                    string strSensorZoneIDs = WebDBManager.GetStringField(arrResult[i + 11]);
                    int nFacilityType = WebDBManager.GetIntField(arrResult[i + 12].ToString(), -1);

                    bool isTraningMode = false;

                    if (strSensorZoneIDs == null || strSensorZoneIDs.Length == 0)
                        CheckAlarmSensorZone(nSensorID, nHistoryID, nReactionType, strMessage, keyExistList, dicEmergencyModes, nEquipZoneID, nFacilityType, ref isTraningMode);
                    else
                    {
                        // SensorZoneHistory의 Param3에는 현재 발생한 알람과 연관된 센서중 작동한 SensorZone ID들이 담겨있다.
                        // [2019/10/31] 김지웅
                        string[] ids = strSensorZoneIDs.Split(',');
                        int id;

                        foreach (string strID in ids)
                        {
                            if (int.TryParse(strID.Trim(), out id))
                            {
                                CheckAlarmSensorZone(id, nHistoryID, nReactionType, strMessage, keyExistList, dicEmergencyModes, nEquipZoneID, nFacilityType, ref isTraningMode);
                                if (isTraningMode)
                                {
                                    //m_Mode = Mode.TraningMode;
                                    break;
                                }
                            }
                        }
                    }

                    if (isTraningMode)
                    {
                        if (nEquipZoneID != m_nTraningEquipZoneID)
                        {
                            if (m_dicHaveTraningPages.ContainsKey(0))
                            {
                                foreach (Control ctrl in m_dicHaveTraningPages[0].Controls)
                                {
                                    if (ctrl is PictureBox)
                                    {
                                        if (!File.Exists(m_strLocalFilePath + "\\Outdoor\\" + nEquipZoneID + ".png"))
                                        {
                                            m_webMgr.DownloadOutdoor(nEquipZoneID + ".png");
                                        }

                                        if (File.Exists(m_strLocalFilePath + "\\Outdoor\\" + nEquipZoneID + ".png"))
                                        {
                                            PictureBox pic = ctrl as PictureBox;
                                            pic.Image = Image.FromFile(m_strLocalFilePath + "\\Outdoor\\" + nEquipZoneID + ".png");
                                        }
                                    }
                                } 
                            }

                            m_nTraningEquipZoneID = nEquipZoneID;
                        }

                        if (strMessage != m_strTraningText)
                        {
                            foreach (KeyValuePair<int, Panel> item in m_dicHaveTraningPages)
                            {
                                foreach (Control ctrl in item.Value.Controls)
                                {
                                    if (ctrl is Label)
                                    {
                                        Label label = ctrl as Label;
                                        label.Text = strMessage;
                                        break;
                                    }
                                }
                            }

                            m_strTraningText = strMessage;
                        }

                        if (m_Mode != Mode.TraningMode)
                        {
                            m_Mode = Mode.TraningMode;
                            SetCurrentPage();

                            m_nSec = 1;
                            m_nCurPageIndex = 0;
                            RefreshPage();
                        }
                        return;
                    }
                }
            }

            bool isUpdate = false;

            List<EmergencyMode> deleteType = new List<EmergencyMode>();
            // 사라진 이벤트 체크
            foreach (KeyValuePair<EmergencyMode, EmergencyPage> item in m_dicSensorIds)
            {
                bool match = true;
                foreach (KeyValuePair<EmergencyMode, EmergencyPage> newItem in dicEmergencyModes)
                {
                    if (newItem.Value.strMessage == item.Value.strMessage)
                    {
                        match = false;
                        break;
                    }                            
                }

                if (match)
                {
                    deleteType.Add(item.Key);
                    isUpdate = true; 
                }
            }

            foreach (EmergencyMode item in deleteType)
            {
                m_dicSensorIds.Remove(item);
            }

            // 새로운 알람 체크
            foreach (KeyValuePair<EmergencyMode, EmergencyPage> item in dicEmergencyModes)
            {
                if (!m_dicSensorIds.ContainsKey(item.Key))
                {
                    m_dicSensorIds.Add(item.Key, item.Value);
                    isUpdate = true;
                }
            }

            if (isUpdate) // 알람 업데이트 내역이 있다면
            {
                if (m_dicSensorIds.Count == 0)
                    m_emergencyPages.Clear();
                else
                {
                    if (deleteType.Count > 0)
                    {
                        List<EmergencyPage> deletePages = new List<EmergencyPage>();
                        foreach (var item in m_emergencyPages)
                        {
                            if (deleteType.Contains(item.EmergencyMode))
                                deletePages.Add(item);
                        }

                        foreach (EmergencyPage item in deletePages)
                        {
                            m_emergencyPages.Remove(item);
                        }
                    }

                    ArrayList arr = MakeMemberOfCompanyData();

                    foreach (KeyValuePair<EmergencyMode, EmergencyPage> item in m_dicSensorIds)
                    {
                        if (m_emergencyPages.Where(p => p.EmergencyMode == item.Key).Count() <= 0)
                        {
                            EmergencyPage page = new EmergencyPage();
                            page.EmergencyMode = item.Key;
                            page.nEquipmentZoneID = item.Value.nEquipmentZoneID;
                            page.strMessage = item.Value.strMessage.Replace("\r\n", "");
                            page.Index = 0;                            
                            m_emergencyPages.Add(page);

                            EmergencyPage page2 = new EmergencyPage();
                            page2.EmergencyMode = item.Key;
                            page2.nEquipmentZoneID = item.Value.nEquipmentZoneID;
                            page2.strMessage = item.Value.strMessage.Replace("\r\n", "");
                            page2.Index = 1;
                            m_emergencyPages.Add(page2);
                        }
                    }

                    foreach (EmergencyPage item in m_emergencyPages)
                    {
                        if (item.Index == 0)
                            item.ArrShowInfo = arr;
                    }
                }

                if (m_dicSensorIds.Count == 0)
                {
                    SetMode(Mode.Normal);
                    MakeNormalPanel();
                }
                else
                {
                    SetMode(Mode.Emergency);
                    MakeEmergencyPanel();
                }
                SetCurrentPage();

                m_nSec = 1;
                m_nCurPageIndex = 0;
                RefreshPage();
            }
        }

        private List<EmergencyPage> m_emergencyPages = new List<EmergencyPage>();
        private ArrayList MakeMemberOfCompanyData()
        {
            ArrayList arr = new ArrayList();

            foreach (var item in m_irisScanMgr.DicCompanies)
            {
                int cnt = item.Value.Workers.Where(p => p.InWork).Count();
                if (cnt <= 0)
                    continue;

                arr.Add(item.Value.Name);
                arr.Add(item.Value.Workers[0].Location.Name);

                int hiCount = 0;
                int byeCount = 0;
                foreach (Worker item2 in item.Value.Workers)
                {
                    if (item2.InWork)
                        hiCount++;
                    else
                        byeCount++;
                }

                arr.Add(hiCount - byeCount);
            }

            return arr;
        }

        private bool CheckAlarmSensorZone(int nSensorZoneID, int nSensorZoneHistoryID, int nReactionType, string strMessage, SortedList<int, int> keyExistList, Dictionary<EmergencyMode, EmergencyPage> modes, int equipZoneID, int facilityType, ref bool isTraningMode)
        {
            isTraningMode = false;

            string szHashKey = nSensorZoneHistoryID.ToString() + "_-_" + nSensorZoneID + "_-_" + nReactionType.ToString() + "_-_" + strMessage;
            int nHash = szHashKey.GetHashCode();
            if (keyExistList.ContainsKey(nHash))
                return false;

            keyExistList.Add(nHash, nHash);

            bool isSuccess;
            libSensorProcess.ReactionType type = ToReactionType(nReactionType, out isSuccess);

            if (type == libSensorProcess.ReactionType.SEND_SMS || type == libSensorProcess.ReactionType.RUN_BROADCAST)
                return false;

            if (!isSuccess)
                return false;

            // 화학물질 센서는 통합처리되므로 data가 같은 SensorZone이므로 각기 SensorZone의 Data를 확인하도록 한다.
            // skkim 2016-02-26 
            string szText = "SELECT Data, Description FROM SensorZone WHERE ID = " + nSensorZoneID;
            ArrayList arrResult = m_dbMgr.GetResultData(szText);
            if (arrResult == null || arrResult.Count == 0)
                return false;
            
            int nSensorData = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            string strSensorName = WebDBManager.GetStringField(arrResult[1], "");

            UnE.Sensor.IFacility.FacilityType Ftype = IFacility.ToFacilityType(facilityType);            
            if (nSensorData == 1 || nSensorData == 21 || nSensorData == 22 || nSensorData == 23 || (Ftype == IFacility.FacilityType.Earthquake))
            {
                if (nReactionType == 22) // 화재 전파
                {
                    isTraningMode = true;
                }
                else
                {
                    EmergencyMode mode = EmergencyMode.Fire;
                    if (IFacility.IsFireSensorType(Ftype))
                        mode = EmergencyMode.Fire;
                    else if (IFacility.IsPSMSensorType(Ftype))
                    {
                        // 공기질인가 ?
                        if (strSensorName == "산소" || strSensorName == "이산화탄소" || strSensorName == "일산화탄소" || strSensorName == "메탄")
                            mode = EmergencyMode.Space;
                        else
                            mode = EmergencyMode.PSM;
                    }
                    else if (IFacility.IsEarthquakeSensorType(Ftype))
                    {
                        mode = EmergencyMode.Earthquake;
                    }

                    if (!modes.ContainsKey(mode))
                    {
                        EmergencyPage page = new EmergencyPage();
                        page.nEquipmentZoneID = equipZoneID;
                        page.strMessage = strMessage;
                        modes.Add(mode, page);
                        return true;
                    }
                }
            }

            return false;
        }

        private static Dictionary<int, libSensorProcess.ReactionType> m_dicReactionType = null;
        public static libSensorProcess.ReactionType ToReactionType(int nType, out bool isSuccess)
        {
            isSuccess = true;

            if (m_dicReactionType == null)
            {
                m_dicReactionType = new Dictionary<int, libSensorProcess.ReactionType>();

                foreach (libSensorProcess.ReactionType type in Enum.GetValues(typeof(libSensorProcess.ReactionType)))
                {
                    m_dicReactionType[(int)type] = type;
                }
            }

            libSensorProcess.ReactionType fType;

            if (m_dicReactionType.TryGetValue(nType, out fType))
                return fType;

            isSuccess = false;
            return libSensorProcess.ReactionType.ETC;
        }

        private void DisplayRealSpace()
        {
            ArrayList arrResult = m_dbMgr.GetResultData("SELECT Connected FROM airquaility");   
            if (arrResult == null || arrResult.Count == 0)
            {
                m_frmTop.PicWorking = false;

                if (m_frmTop.labelAir.Visible)
                    m_frmTop.labelAir.Visible = false;
                if (m_frmTop.labelAirValue.Visible)
                    m_frmTop.labelAirValue.Visible = false;
                return;
            }

            bool bVisible = false;
            for (int i = 0; i < arrResult.Count; i++)
            {
                int nConnected = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString(), 0);

                if (nConnected == 1)
                {
                    bVisible = true;
                    break;
                }
            }

            if (m_frmTop.PicWorking != bVisible)
                m_frmTop.PicWorking = bVisible;

            if (m_frmTop.labelAir.Visible != bVisible)
                m_frmTop.labelAir.Visible = bVisible;
            if (m_frmTop.labelAirValue.Visible != bVisible)
                m_frmTop.labelAirValue.Visible = bVisible;

            if (!bVisible)
                return;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT SensorName, Value, ShowDidViewer ");
            sb.Append("  FROM Airquaility ");

            string o2 = "-";  //산소
            string co2 = "-"; //이산화탄소
            string co = "-";  //일산화탄소
            string ch4 = "-"; //메탄
            string temp = "-";//온도
            string humi = "-";//습도

            arrResult = m_dbMgr.GetResultData(sb.ToString());

            bool bShowDidViewer = false;
            if (arrResult != null || arrResult.Count > 0)
            {
                for (int i = 0; i < arrResult.Count; i += 3)
                {
                    string strSensorName = DBUtility2.WebDBManager.GetStringField(arrResult[i]);
                    float nValue = DBUtility2.WebDBManager.GetFloatField(arrResult[i + 1].ToString(), -1.0f);
                    int nShowDidViewer = DBUtility2.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1); // DidViewer에 실내공기질 정보를 보여줄지 여부
                    bShowDidViewer = (nShowDidViewer == 0) ? false : true;

                    switch (strSensorName)
                    {
                        case "산소":
                            o2 = nValue.ToString();
                            break;
                        case "이산화탄소":
                            co2 = nValue.ToString();
                            break;
                        case "일산화탄소":
                            co = nValue.ToString();
                            break;
                        case "메탄":
                            ch4 = nValue.ToString();
                            break;
                        case "온도":
                            temp = nValue.ToString();
                            break;
                        case "습도":
                            humi = nValue.ToString();
                            break;
                    }
                }
            }
            m_frmTop.labelAirValue.Text = string.Format("{0} %\r\n{1} ppm \r\n{2} ppm\r\n{3} ppm\r\n{4} ℃ \r\n{5} %", o2, co2, co, ch4, temp, humi);

            if (m_frmTop.labelAir.Visible != bShowDidViewer)
                m_frmTop.labelAir.Visible = bShowDidViewer;
            if (m_frmTop.labelAirValue.Visible != bShowDidViewer)
                m_frmTop.labelAirValue.Visible = bShowDidViewer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="init">오전12:00가 되서 초기화 시키는것인가?</param>
        public void ResetEmergencyIris(bool init = false)
        {
            if (m_Mode != Mode.Emergency)
                return;

            ArrayList arr = MakeMemberOfCompanyData();
            foreach (KeyValuePair<int, Panel> ctrl in m_dicHaveEmergencyPages)
            {
                if ((ctrl.Value.Tag is EmergencyPage) == false)
                    continue;

                EmergencyPage page = ctrl.Value.Tag as EmergencyPage;
                if (page.Index != 0)
                    return;

                foreach (Control item in ctrl.Value.Controls)
                {
                    if (item is uEmergency)
                    {
                        uEmergency emergency = item as uEmergency;
                        emergency.ArrShowInfo = arr;
                        emergency.ViewCompanyInfoIndex = 0;
                        if (init)
                            emergency.ClearInfo();
                    }
                }
            }
        }

        public void ResetNormalIris(bool init = false)
        {
            if (m_Mode != Mode.Normal)
                return;
            
            foreach (KeyValuePair<int, Panel> ctrl in m_dicHaveNormalPages)
            {
                foreach (Control item in ctrl.Value.Controls)
                {
                    if (item is uCompanyGroup)
                    {
                        uCompanyGroup companyGroup = item as uCompanyGroup;
                        companyGroup.ClearInfo();
                        //int lastIndex = -1;
                        //companyGroup.MakePanel(m_irisScanMgr.DicCompanies, m_curPage.ViewCompanyInfoIndex, ref lastIndex);

                        companyGroup.ViewCompanyInfoIndex = -1;
                        //m_nSec = 1;
                        break;
                    } 
                }
            }
        }

        public void Init()
        {
            m_irisScanMgr.Init();

            //if (m_Mode == Mode.Emergency)
            {
                ResetEmergencyIris(true);
            }
            //else if (m_Mode == Mode.Normal)
            {
                ResetNormalIris(true);
            }
        }

        public bool IsUpdateIris = false;
        public void UpdateIris()
        {
            IsUpdateIris = false;
            foreach (KeyValuePair<int, Panel> ctrl in m_dicHaveNormalPages)
            {
                foreach (Control item in ctrl.Value.Controls)
                {
                    if (item is uCompanyGroup)
                    {
                        uCompanyGroup companyGroup = item as uCompanyGroup;
                        companyGroup.ClearInfo();
                        
                        int lastIndex = -1;
                        companyGroup.MakePanel(m_irisScanMgr.DicCompanies, 0, ref lastIndex);
                        companyGroup.ViewCompanyInfoIndex = lastIndex;
                        break;
                    }
                }
            }

            ArrayList arr = MakeMemberOfCompanyData();

            foreach (KeyValuePair<int, Panel> ctrl in m_dicHaveEmergencyPages)
            {
                foreach (Control item in ctrl.Value.Controls)
                {
                    if (item is uEmergency)
                    {
                        uEmergency emergency = item as uEmergency;
                        emergency.ClearInfo();
                        
                        int lastIndex = -1;
                        emergency.ArrShowInfo = arr;
                        emergency.SetCompanyData();
                        emergency.ViewCompanyInfoIndex = lastIndex;
                        break;
                    }
                }
            }
        }
    }
}
