using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SDMS.Help;
using UnE.GUI;

namespace SDMS.PopupDialog.Report
{
    public enum ReportCategory { NONE, FIRE, PSM, SECURITY, EARTHQUAKE, TemperatureHumidity }

    public partial class FormReportMenu : PopupFormBase, IImageButtonOwner
    {
        private ReportCategory m_category = ReportCategory.FIRE;

        private ManualManager m_manualManager = null;

        public ImageButton BtnReport
        {
            get { return btnReport; }
            set { btnReport = value; }
        }
        public ImageButton BtnDetectAnalyze
        {
            get { return btnDetectAnalyze; }
            set { btnDetectAnalyze = value; }
        }
        public ImageButton BtnDetectHistory
        {
            get { return btnDetectHistory; }
            set { btnDetectHistory = value; }
        }
        public ImageButton BtnProcessHistory
        {
            get { return btnProcessHistory; }
            set { btnProcessHistory = value; }
        }
        public ImageButton BtnReactionHistory
        {
            get { return btnReactionHistory; }
            set { btnReactionHistory = value; }
        }
        public ImageButton BtnSMSHistory
        {
            get { return btnSMSHistory; }
            set { btnSMSHistory = value; }
        }

        private Timer m_timer = null;
        private bool m_sizeFull = false;
        public bool SizeFull
        {
            get { return m_sizeFull; }
            set { m_sizeFull = value; }
        }
         
        private List<ImageButton> m_BtnList = new List<ImageButton>();
        private Dictionary<ImageButton, bool> m_dicButtonUsing = new Dictionary<ImageButton, bool>();
        private int m_nUsingButtonCount = 5;

        public FormReportMenu(ReportCategory category)
        {
            InitializeComponent();

            this.m_category = category;

            InitCtrlSize(this);
            SetChildCtrlResize(this, this.Width, this.Height);
            //SetLocation();

            m_timer = new Timer();
            m_timer.Interval = 10;
            m_timer.Tick += timer_Tick;

            m_BtnList.Add(btnDetectAnalyze);
            m_BtnList.Add(btnDetectHistory);
            m_BtnList.Add(btnProcessHistory);
            m_BtnList.Add(btnReactionHistory);
            m_BtnList.Add(btnSMSHistory);
            m_nUsingButtonCount = m_BtnList.Count;

            SettingImageButton();
            m_manualManager = new ManualManager(this);
            SetManualID();
            SetLocation();

            this.UseFrmMove = false;
        }

        private Image ImgReportNormal = global::SDMS.Properties.Resources.FireReport_Default;
        private Image ImgReportClick = global::SDMS.Properties.Resources.FireReport_Click;

        private Image ImgDetectAnalyzeNormal = global::SDMS.Properties.Resources.FireDetectAnalyze_Default;
        private Image ImgDetectAnalyzeClick = global::SDMS.Properties.Resources.FireDetectAnalyze_Click;

        private Image ImgDetectHistoryNormal = global::SDMS.Properties.Resources.FireDetectHistory_Default;
        private Image ImgDetectHistoryClick = global::SDMS.Properties.Resources.FireDetectHistory_Click;

        private Image ImgProcessHistoryNormal = global::SDMS.Properties.Resources.FireProcessHistory_Default;
        private Image ImgProcessHistoryClick = global::SDMS.Properties.Resources.FireProcessHistory_Click;

        private Image ImgReactionHistoryNormal = global::SDMS.Properties.Resources.FireReactionHistory_Default;
        private Image ImgReactionHistoryClick = global::SDMS.Properties.Resources.FireReactionHistory_Click;

        private Image ImgSMSHistoryNormal = global::SDMS.Properties.Resources.FireSMSHistory_Default;
        private Image ImgSMSHistoryClick = global::SDMS.Properties.Resources.FireSMSHistory_Click;

        public void SettingImageButton()
        {
            m_dicButtonUsing[btnDetectAnalyze] = true;
            m_dicButtonUsing[btnDetectHistory] = true;
            m_dicButtonUsing[btnProcessHistory] = true;
            m_dicButtonUsing[btnReactionHistory] = true;
            m_dicButtonUsing[btnSMSHistory] = true;

            if (m_category == ReportCategory.PSM) 
            {
                ImgReportNormal = global::SDMS.Properties.Resources.PsmReport_Default;
                ImgReportClick = global::SDMS.Properties.Resources.PsmReport_Click; 

                ImgDetectAnalyzeNormal = global::SDMS.Properties.Resources.PsmDetectAnalyze_Default;
                ImgDetectAnalyzeClick = global::SDMS.Properties.Resources.PsmDetectAnalyze_Click; 

                ImgDetectHistoryNormal = global::SDMS.Properties.Resources.PsmDetectHistoryDefault;
                ImgDetectHistoryClick = global::SDMS.Properties.Resources.PsmDetectHistory_Click; 

                ImgProcessHistoryNormal = global::SDMS.Properties.Resources.PsmProcessHistory_Default;
                ImgProcessHistoryClick = global::SDMS.Properties.Resources.PsmProcessHistory_Click; 

                ImgReactionHistoryNormal= global::SDMS.Properties.Resources.PsmReactionHistory_Default;
                ImgReactionHistoryClick= global::SDMS.Properties.Resources.PsmReactionHistory_Click;

                ImgSMSHistoryNormal = global::SDMS.Properties.Resources.PsmSMSHistory_Default;
                ImgSMSHistoryClick = global::SDMS.Properties.Resources.PsmSMSHistory_Click;
            }
            else if (m_category == ReportCategory.SECURITY)
            {
                ImgReportNormal= global::SDMS.Properties.Resources.SecurityReport_Default;
                ImgReportClick = global::SDMS.Properties.Resources.SecurityReport_Click; 

                ImgDetectAnalyzeNormal = global::SDMS.Properties.Resources.SecurityDetectAnalyze_Default;
                ImgDetectAnalyzeClick = global::SDMS.Properties.Resources.SecurityDetectAnalyze_Click; 

                ImgDetectHistoryNormal = global::SDMS.Properties.Resources.SecurityDetectHistoryDefault;
                ImgDetectHistoryClick = global::SDMS.Properties.Resources.SecurityDetectHistory_Click; 

                ImgProcessHistoryNormal = global::SDMS.Properties.Resources.SecurityProcessHistory_Default;
                ImgProcessHistoryClick = global::SDMS.Properties.Resources.SecurityProcessHistory_Click; 

                ImgReactionHistoryNormal = global::SDMS.Properties.Resources.SecurityReactionHistory_Default;
                ImgReactionHistoryClick = global::SDMS.Properties.Resources.SecurityReactionHistory_Click;

                ImgSMSHistoryNormal = global::SDMS.Properties.Resources.SecuritySMSHistory_Default;
                ImgSMSHistoryClick = global::SDMS.Properties.Resources.SecuritySMSHistory_Click;
            }
            else if (m_category == ReportCategory.EARTHQUAKE)
            {
                ImgReportNormal = global::SDMS.Properties.Resources.EarthquakeReport_Default;
                ImgReportClick = global::SDMS.Properties.Resources.EarthquakeReport_Click;

                ImgDetectAnalyzeNormal = global::SDMS.Properties.Resources.EarthquakeDetectAnalyze_Click;
                ImgDetectAnalyzeClick = global::SDMS.Properties.Resources.EarthquakeDetectAnalyze_Click;

                ImgDetectHistoryNormal = global::SDMS.Properties.Resources.EarthquakeDetectHistory_Default;
                ImgDetectHistoryClick = global::SDMS.Properties.Resources.EarthquakeDetectHistory_Click;

                ImgProcessHistoryNormal = global::SDMS.Properties.Resources.EarthquakeProcessHistory_Default;
                ImgProcessHistoryClick = global::SDMS.Properties.Resources.EarthquakeProcessHistory_Click;

                ImgReactionHistoryNormal = global::SDMS.Properties.Resources.EarthquakeReactionHistory_Default;
                ImgReactionHistoryClick = global::SDMS.Properties.Resources.EarthquakeReactionHistory_Click;

                ImgSMSHistoryNormal = global::SDMS.Properties.Resources.EarthquakeSMSHistory_Default;
                ImgSMSHistoryClick = global::SDMS.Properties.Resources.EarthquakeSMSHistory_Click;

                m_dicButtonUsing[btnDetectAnalyze] = false;
                m_dicButtonUsing[btnDetectHistory] = true;
                m_dicButtonUsing[btnProcessHistory] = false;
                m_dicButtonUsing[btnReactionHistory] = true;
                m_dicButtonUsing[btnSMSHistory] = false;
            }
            else if (m_category == ReportCategory.TemperatureHumidity)
            {
                ImgReportNormal = global::SDMS.Properties.Resources.THReport_Default;
                ImgReportClick = global::SDMS.Properties.Resources.THReport_Click;

                ImgDetectAnalyzeNormal = global::SDMS.Properties.Resources.THDetectAnalyze_Click;
                ImgDetectAnalyzeClick = global::SDMS.Properties.Resources.THDetectAnalyze_Click;

                ImgDetectHistoryNormal = global::SDMS.Properties.Resources.THDetectHistory_Default;
                ImgDetectHistoryClick = global::SDMS.Properties.Resources.THDetectHistory_Click;

                ImgReactionHistoryNormal = global::SDMS.Properties.Resources.THReactionHistory_Default;
                ImgReactionHistoryClick = global::SDMS.Properties.Resources.THReactionHistory_Click;

                m_dicButtonUsing[btnDetectAnalyze] = true;
                m_dicButtonUsing[btnDetectHistory] = true;
                m_dicButtonUsing[btnProcessHistory] = false;
                m_dicButtonUsing[btnReactionHistory] = true;
                m_dicButtonUsing[btnSMSHistory] = false;
            }

            if (m_category == ReportCategory.FIRE)
            {
                BtnReport.ImageNormal = ImgDetectAnalyzeClick;
                BtnReport.ImageMouseOver = ImgDetectAnalyzeClick;
                BtnReport.ImageClicked = ImgDetectAnalyzeClick;

                clickImg = ImgDetectAnalyzeClick;
            }
            else
            {
                BtnReport.ImageNormal = ImgReportNormal;
                BtnReport.ImageMouseOver = ImgReportClick;

                BtnReport.ImageClicked = ImgReportClick;
            }

            if (m_category == ReportCategory.FIRE)
                BtnDetectAnalyze.ImageNormal = ImgDetectAnalyzeClick;
            else
                BtnDetectAnalyze.ImageNormal = ImgDetectAnalyzeNormal;
            BtnDetectAnalyze.ImageMouseOver = ImgDetectAnalyzeClick;
            BtnDetectAnalyze.ImageClicked = ImgDetectAnalyzeClick;

            BtnDetectHistory.ImageNormal = ImgDetectHistoryNormal;
            BtnDetectHistory.ImageMouseOver = ImgDetectHistoryClick;
            BtnDetectHistory.ImageClicked = ImgDetectHistoryClick;

            BtnProcessHistory.ImageNormal = ImgProcessHistoryNormal;
            BtnProcessHistory.ImageMouseOver = ImgProcessHistoryClick;
            BtnProcessHistory.ImageClicked = ImgProcessHistoryClick;

            BtnReactionHistory.ImageNormal = ImgReactionHistoryNormal;
            BtnReactionHistory.ImageMouseOver = ImgReactionHistoryClick;
            BtnReactionHistory.ImageClicked = ImgReactionHistoryClick;

            BtnSMSHistory.ImageNormal = ImgSMSHistoryNormal;
            BtnSMSHistory.ImageMouseOver = ImgSMSHistoryClick;
            BtnSMSHistory.ImageClicked = ImgSMSHistoryClick;
        }

        public void ResetImageButton()
        {
            BtnReport.ImageNormal = ImgReportNormal;
            BtnReport.ImageMouseOver = ImgReportClick;
            BtnReport.ImageClicked = ImgReportClick;
             
            BtnDetectAnalyze.ImageNormal = ImgDetectAnalyzeNormal;
            BtnDetectHistory.ImageNormal = ImgDetectHistoryNormal;
            BtnProcessHistory.ImageNormal = ImgProcessHistoryNormal;
            BtnReactionHistory.ImageNormal = ImgReactionHistoryNormal;
            BtnSMSHistory.ImageNormal = ImgSMSHistoryNormal;

            BtnReport.Refresh();
            BtnDetectAnalyze.Refresh();
            BtnDetectHistory.Refresh();
            BtnProcessHistory.Refresh();
            BtnReactionHistory.Refresh();
            BtnSMSHistory.Refresh();

            clickImg = null;
        }

        #region IImageButtonOwner 멤버

        public void OnImageButtonMouseDown(object sender, MouseEventArgs e)
        {
            
        }

        public void OnImageButtonMouseUp(object sender, MouseEventArgs e)
        {
            if (m_manualManager.IsHelpMode)
                return;

            ImageButton btn = (ImageButton)sender;
            int nID = FormMain.Instance.GetButtonID(btn);
            FormMain.Instance.PageHome.OnCommandExcute(nID);

            SetButtonBackColor(btn);
             
            FormMain.Instance.CheckReportButton(btn);

            btnReport_Click(null, null);
            FormMain.Instance.ResetOtherReportMenu(m_category);
        }

        #endregion 

        public void FromDetectPageToActionPage()
        {
            SetButtonBackColor(btnReactionHistory);
             
            BtnReport.ImageNormal = ImgReactionHistoryClick;
            BtnReport.ImageMouseOver = ImgReactionHistoryClick;
            BtnReport.ImageClicked = ImgReactionHistoryClick;
            BtnReport.Refresh();
        }

        private Image clickImg = null;
        public void SetButtonBackColor(ImageButton btn)
        { 
            foreach (ImageButton item in m_BtnList)
            { 
                if (item == btnDetectAnalyze)
                {
                    if (item.Name == btn.Name) // RemoteControl에서 넘어오는 경우도 있으므로 Name으로 판단한다.
                    {
                        item.ImageNormal = ImgDetectAnalyzeClick;
                        clickImg = ImgDetectAnalyzeClick;
                    }
                    else
                        item.ImageNormal = ImgDetectAnalyzeNormal; 
                }
                else if (item == btnDetectHistory)
                {
                    if (item.Name == btn.Name)
                    {
                        item.ImageNormal = ImgDetectHistoryClick;
                        clickImg = ImgDetectHistoryClick;
                    }
                    else
                        item.ImageNormal = ImgDetectHistoryNormal;
                }
                else if (item == btnProcessHistory)
                {
                    if (item.Name == btn.Name)
                    {
                        item.ImageNormal = ImgProcessHistoryClick;
                        clickImg = ImgProcessHistoryClick;
                    }
                    else
                        item.ImageNormal = ImgProcessHistoryNormal;
                }
                else if (item == btnReactionHistory)
                {
                    if (item.Name == btn.Name)
                    {
                        item.ImageNormal = ImgReactionHistoryClick;
                        clickImg = ImgReactionHistoryClick;
                    }
                    else
                        item.ImageNormal = ImgReactionHistoryNormal;
                }
                else if (item == btnSMSHistory)
                {
                    if (item.Name == btn.Name)
                    {
                        item.ImageNormal = ImgSMSHistoryClick;
                        clickImg = ImgSMSHistoryClick;
                    }
                    else
                        item.ImageNormal = ImgSMSHistoryNormal;
                }
                item.Refresh();
            }

            if (clickImg != null)
            {
                btnReport.ImageNormal = clickImg;
                btnReport.ImageMouseOver = clickImg;
                btnReport.ImageClicked = clickImg;
            }
        } 


        public void SetLocation()
        {
            int horizontal = 10;
            int vertical = 9; 
            if (FormMain.Instance.Resolution == Resolution.FullHD)
            {
                horizontal = (int)(horizontal * 0.5);
                vertical = (int)(vertical * 0.5); 
            }
            else if (FormMain.Instance.Resolution == SDMS.Resolution.Other)
            {
                horizontal = (int)(horizontal * 0.75);
                vertical = (int)(vertical * 0.75); 
            }
             
            btnReport.Location = new Point(horizontal, vertical);
            ArrangeButtonHorizontal(btnReport, btnDetectAnalyze, horizontal);
            ArrangeButtonHorizontal(btnDetectAnalyze, btnDetectHistory, horizontal);
            ArrangeButtonHorizontal(btnDetectHistory, btnProcessHistory, horizontal);
            ArrangeButtonHorizontal(btnProcessHistory, btnReactionHistory, horizontal);
            ArrangeButtonHorizontal(btnReactionHistory, btnSMSHistory, horizontal);

            ChangeLocationFromNotUsed();
        }

        // 사용하지 않는 버튼들은 안보이도록 한다.
        private void ChangeLocationFromNotUsed()
        {
            List<ImageButton> usingButtons = new List<ImageButton>();
            List<ImageButton> notUsingButtons = new List<ImageButton>();
            List<Point> btnLocations = new List<Point>();

            foreach (ImageButton btn in m_BtnList)
            {
                if (m_dicButtonUsing[btn] == true)
                    usingButtons.Add(btn);
                else
                    notUsingButtons.Add(btn);

                btnLocations.Add(btn.Location);
            }

            if (notUsingButtons.Count > 0)
            {
                for (int i=0;i<usingButtons.Count;i++)
                {
                    usingButtons[i].Location = btnLocations[i];
                }

                for (int i = notUsingButtons.Count - 1, j = m_BtnList.Count - 1; i >= 0; i--, j--)
                {
                    notUsingButtons[i].Location = btnLocations[j];
                }
            }

            m_nUsingButtonCount = usingButtons.Count;
            /*Dictionary<ImageButton, ImageButton> dicChangedButtons = new Dictionary<ImageButton, ImageButton>();

            for (int i=0;i<m_BtnList.Count;i++)
            {
                ImageButton btn = m_BtnList[i];

                if (btn.Tag != null && btn.Tag is bool)
                {
                    bool use = (bool)btn.Tag;

                    if (use == false)
                    {
                        ImageButton btn2 = GetUsingButton(i + 1, dicChangedButtons);

                        if (btn2 != null)
                            ChangeLocation(btn, btn2);

                        nUsingButtonCount--;
                    }
                }
            }

            m_nUsingButtonCount = nUsingButtonCount;*/
        }

        /*private void ChangeLocation(Control ctrl1, Control ctrl2)
        {
            Point pt1 = ctrl1.Location;
            ctrl1.Location = ctrl2.Location;
            ctrl2.Location = pt1;
        }

        private ImageButton GetUsingButton(int nIndex, Dictionary<ImageButton, ImageButton> dicChangedButtons)
        {
            for (int i=nIndex;i<m_BtnList.Count;i++)
            {
                ImageButton btn = m_BtnList[i];

                if (btn.Tag == null || (btn.Tag is bool && ((bool)btn.Tag) == true))
                {
                    if (dicChangedButtons.ContainsKey(btn))
                        continue;
                    else
                    {
                        dicChangedButtons[btn] = btn;
                        return btn;
                    }
                }
            }

            return null;
        }*/

        private void ArrangeButtonHorizontal(ImageButton btnPrev, ImageButton btnNext, int horizontal)
        {
            btnNext.Location = new Point(btnPrev.Location.X + btnPrev.Size.Width + horizontal, btnPrev.Location.Y);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            int maxWidth = 742;
            int minWidth = 132;
            int gap = 40;
            int imgWidth = 122;

            maxWidth -= (m_BtnList.Count - m_nUsingButtonCount) * imgWidth;

            double sizePer = 1;
            if (FormMain.Instance.Resolution == Resolution.FullHD)
            {
                sizePer = sizePer * 0.5;
            }
            else if (FormMain.Instance.Resolution == SDMS.Resolution.Other)
            {
                sizePer = sizePer * 0.75;
            }

            maxWidth = (int)(maxWidth * sizePer);
            minWidth = (int)(minWidth * sizePer);
            gap = (int)(gap * sizePer);

            if (m_sizeFull)
            {
                if (this.Width >= maxWidth)
                    m_timer.Enabled = false;
                else
                {
                    if (this.Width + gap > maxWidth)
                    {
                        this.Width = maxWidth;
                        //this.Width += maxWidth - this.Width;
                    }
                    else
                        this.Width += gap;
                }
            }
            else
            {
                if (this.Width <= minWidth)
                    m_timer.Enabled = false;
                else
                {
                    if (this.Width - gap < minWidth)
                    {
                        this.Width = minWidth;
                        //this.Width -= minWidth - this.Width;
                    }
                    else
                        this.Width -= gap;
                }
            }
        }
        public bool IsAuto = false;
        public void btnReport_Click(object sender, EventArgs e)
        {
            if (m_sizeFull)
            {
                if (clickImg != null)
                {
                    btnReport.ImageNormal = clickImg;
                    btnReport.ImageMouseOver = clickImg;
                    btnReport.ImageClicked = clickImg;
                    btnReport.Refresh();
                }
                
                m_timer.Enabled = true;
                m_sizeFull = false;
            }
            else
            {
                btnReport.ImageNormal = ImgReportNormal;
                btnReport.ImageMouseOver = ImgReportClick;
                btnReport.ImageClicked = ImgReportClick;
                btnReport.Refresh();


                m_timer.Enabled = true;
                m_sizeFull = true;

                FormMain.Instance.CloseOtherReportMenu(m_category);
            } 
        }

        private void SetManualID()
        {
            m_manualManager.Handle = this.Handle;

            m_manualManager.Clear();
            if (m_category == ReportCategory.FIRE)
            {
                m_manualManager.SetID(this, "SDMS_Report_Fire");
                m_manualManager.SetID(btnReport, "SDMS_Report_Fire");
                m_manualManager.SetID(btnDetectAnalyze, "SDMS_Report_Pareto_Fire");
                m_manualManager.SetID(btnDetectHistory, "SDMS_Report_Detect_Fire");
                m_manualManager.SetID(btnProcessHistory, "SDMS_Report_Process_Fire");
                m_manualManager.SetID(btnReactionHistory, "SDMS_Report_Action_Fire");
                m_manualManager.SetID(btnSMSHistory, "SDMS_Report_SMS_Fire"); 
            }
            else if (m_category == ReportCategory.PSM)
            {
                m_manualManager.SetID(this, "SDMS_Report_PSM");
                m_manualManager.SetID(btnReport, "SDMS_Report_PSM");
                m_manualManager.SetID(btnDetectAnalyze, "SDMS_Report_Pareto_PSM");
                m_manualManager.SetID(btnDetectHistory, "SDMS_Report_Detect_PSM");
                m_manualManager.SetID(btnProcessHistory, "SDMS_Report_Process_PSM");
                m_manualManager.SetID(btnReactionHistory, "SDMS_Report_Action_PSM");
                m_manualManager.SetID(btnSMSHistory, "SDMS_Report_SMS_PSM"); 
            }
            else if (m_category == ReportCategory.SECURITY)
            {
                m_manualManager.SetID(this, "SDMS_Report_Security");
                m_manualManager.SetID(btnReport, "SDMS_Report_Security");
                m_manualManager.SetID(btnDetectAnalyze, "SDMS_Report_Pareto_Security");
                m_manualManager.SetID(btnDetectHistory, "SDMS_Report_Detect_Security");
                m_manualManager.SetID(btnProcessHistory, "SDMS_Report_Process_Security");
                m_manualManager.SetID(btnReactionHistory, "SDMS_Report_Action_Security");
                m_manualManager.SetID(btnSMSHistory, "SDMS_Report_SMS_Security"); 
            }
            else if (m_category == ReportCategory.EARTHQUAKE)
            {
                m_manualManager.SetID(this, "SDMS_Report_Earthquake");
                m_manualManager.SetID(btnReport, "SDMS_Report_Earthquake");
                m_manualManager.SetID(btnDetectAnalyze, "SDMS_Report_Pareto_Earthquake");
                m_manualManager.SetID(btnDetectHistory, "SDMS_Report_Detect_Earthquake");
                m_manualManager.SetID(btnProcessHistory, "SDMS_Report_Process_Earthquake");
                m_manualManager.SetID(btnReactionHistory, "SDMS_Report_Action_Earthquake");
                m_manualManager.SetID(btnSMSHistory, "SDMS_Report_SMS_Earthquake");
            }
            else if (m_category == ReportCategory.TemperatureHumidity)
            {
                m_manualManager.SetID(this, "SDMS_Report_TH");
                m_manualManager.SetID(btnReport, "SDMS_Report_TH");
                m_manualManager.SetID(btnDetectAnalyze, "SDMS_Report_Pareto_TH");
                m_manualManager.SetID(btnDetectHistory, "SDMS_Report_Detect_TH");
                m_manualManager.SetID(btnProcessHistory, "SDMS_Report_Process_TH");
                m_manualManager.SetID(btnReactionHistory, "SDMS_Report_Action_TH");
                m_manualManager.SetID(btnSMSHistory, "SDMS_Report_SMS_TH");
            }

            m_manualManager.ProcessEvent();
        } 
    }
}
