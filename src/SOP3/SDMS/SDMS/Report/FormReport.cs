using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SDMS
{
    public partial class FormReport : Form
    {
        private int m_nReportPage = 1;
        public int ReportPage
        {
            get { return m_nReportPage; }
            set { m_nReportPage = value; }
        }
        
        private SaveFileDialog saveFileDialog1 = new SaveFileDialog();
       
        private Report.ReactionManager m_ActionMgr = new Report.ReactionManager();
        private Report.ReactionManager m_DetectMgr = new Report.ReactionManager();

        private DetectPage m_DetectPage = null;
        private NotOperationPage m_NotOperation = null;
        private ActionPage m_ActionPage = null;
        private SMSPage m_PageSMS = null;
            
        public FormReport()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            InitializeComponent();

            m_DetectPage = new DetectPage(m_DetectMgr);
            m_NotOperation = new NotOperationPage(m_DetectMgr);
            m_ActionPage = new ActionPage(m_ActionMgr);

            m_PageSMS = new SMSPage();

            m_DetectPage.Location = new Point(0, 0);
            m_DetectPage.TopLevel = false;
            m_DetectPage.Parent = m_DetectPanel;
            m_DetectPage.Dock = DockStyle.Fill;
            m_DetectPanel.Controls.Add(m_DetectPage);
            m_DetectPage.Show();

            m_NotOperation.Location = new Point(0, 0);
            m_NotOperation.TopLevel = false;
            m_NotOperation.Parent = m_NotOperationPanel;
            m_NotOperation.Dock = DockStyle.Fill;
            m_NotOperationPanel.Controls.Add(m_NotOperation);
            m_NotOperation.Show();

            m_ActionPage.Location = new Point(0, 0);
            m_ActionPage.TopLevel = false;
            m_ActionPage.Parent = m_ActionPanel;
            m_ActionPage.Dock = DockStyle.Fill;
            m_ActionPanel.Controls.Add(m_ActionPage);
            m_ActionPage.Show();

            m_PageSMS.Location = new Point(0, 0);
            m_PageSMS.TopLevel = false;
            m_PageSMS.Parent = m_SmsPanel;
            m_PageSMS.Dock = DockStyle.Fill;
            m_SmsPanel.Controls.Add(m_PageSMS);
            m_PageSMS.Show();

            m_DetectPanel.Visible = true;
            m_NotOperationPanel.Visible = false;
            m_ActionPanel.Visible = false;
            m_SmsPanel.Visible = false;
        }

        private void FormReport_VisibleChanged(object sender, EventArgs e)
        {
            FormMain.Instance.PageHome.Redraw3DView();
        }

        private void m_NotOperationPanel_Resize(object sender, EventArgs e)
        {            
        }

        public void SelectReport(string strGroupName, string strBuildingName, string strFloorName, DateTime startDate, DateTime EndDate)
        {
            string strStartDate = startDate.ToShortDateString();
            string strEndDate = EndDate.ToShortDateString();

            ArrayList arrSelectZoneList = ZoneManager.Instance.FindZoneList(strGroupName, strBuildingName, strFloorName);
            
            m_NotOperation.ComboTxtDate(strStartDate, strEndDate);
            m_DetectPage.ComboSubmit(strGroupName, strBuildingName, strFloorName);
            m_NotOperation.ComboSubmit(strGroupName, strBuildingName, strFloorName);

            m_ActionPage.SetLabelString(strGroupName + "  " + strBuildingName + "  " + strFloorName);

            m_DetectMgr.DataClear();
            m_DetectMgr.ZoneSubmit(arrSelectZoneList, startDate, EndDate);

            m_DetectPage.Load_DataGrid();
            m_NotOperation.Load_DataGrid();

          
            m_PageSMS.ZoneSubmit(arrSelectZoneList, startDate, EndDate);
            m_PageSMS.LoadDataGrid();

            //그래프그리기
            m_DetectPage.CreateLineChart(startDate, EndDate);
            m_NotOperation.createBarChart(m_NotOperation.PercentBarChart);   
        }

        public void ShowDetectReport()
        {
            m_nReportPage = 1;
            m_DetectPanel.Visible = true;
            m_NotOperationPanel.Visible = false;
            m_ActionPanel.Visible = false;
            m_SmsPanel.Visible = false;
        }

        public void ShowProcessHistoryReport()
        {            
            m_nReportPage = 2;
            m_DetectPanel.Visible = false;
            m_NotOperationPanel.Visible = true;
            m_ActionPanel.Visible = false;
            m_SmsPanel.Visible = false;
        }

        public void ShowReactionHistoryReport()
        {
            m_nReportPage = 3;
            m_DetectPanel.Visible = false;
            m_NotOperationPanel.Visible = false;
            m_ActionPanel.Visible = true;
            m_SmsPanel.Visible = false;
        }

        public void ShowSmsHistoryReport()
        {
            m_nReportPage = 4;
            m_DetectPanel.Visible = false;
            m_NotOperationPanel.Visible = false;
            m_ActionPanel.Visible = false;
            m_SmsPanel.Visible = true;
        }
        
        public bool SaveHWP2()
        {
            bool isHwpSetup = false;
            isHwpSetup = m_DetectPage.HwpCtrl.GetRegistry();

            //한글 설치여부
            if (isHwpSetup == false)
            {
                MessageBox.Show("아래한글이 설치되지 않았습니다.");
                return false;
            }
            
            string SavePath = "";
            saveFileDialog1.Filter = "한글 문서 (*.hwp)|*.hwp";

            if (m_nReportPage == 1) //탐지
            {
                saveFileDialog1.FileName = "화재_탐지_보고서";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    //화면캡쳐
                    m_DetectPage.ControllCapture();
                    m_DetectPage.FileWriter();
                    m_DetectPage.SetHwpData();

                    SavePath = saveFileDialog1.FileName;
                    //공백제거
                    SavePath = subGap(SavePath);
                    SavePath = SavePath.Replace("\\", "/");
                    SavePath = SavePath.Replace("/", "\\\\");

                    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                    info.Arguments = "1 " + SavePath;
                    info.CreateNoWindow = true;
                    info.FileName = Application.StartupPath + "\\HwpReport.exe";

                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.StartInfo = info;

                    process.Start();
                    this.Cursor = Cursors.WaitCursor;

                    int nCount = 0;
                    bool bSuccess = true;
                    while (process.HasExited == false)
                    {
                        process.WaitForExit(500);

                        if (30 == nCount)
                        {
                            process.Kill();
                            MessageBox.Show("오류 발생");
                            bSuccess = false;
                            break;
                        }
                    }

                    if (bSuccess == true)
                        MessageBox.Show("저장되었습니다.");


                    this.Cursor = Cursors.Default;
                }               
            }
            else if (m_nReportPage == 2) //처리
            {
                saveFileDialog1.FileName = "처리_이력_보고서";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    m_NotOperation.ControllCapture();
                    m_NotOperation.FileWriter();
                    m_NotOperation.SetHwpData();

                    SavePath = saveFileDialog1.FileName;
                    //공백제거
                    SavePath = subGap(SavePath);
                    SavePath = SavePath.Replace("\\", "/");
                    SavePath = SavePath.Replace("/", "\\\\");

                    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                    info.Arguments = "2 " + SavePath;
                    info.CreateNoWindow = true;
                    info.FileName = Application.StartupPath + "\\HwpReport.exe";

                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.StartInfo = info;
                    process.Start();
                    this.Cursor = Cursors.WaitCursor;

                    int nCount = 0;
                    bool bSuccess = true;
                    while (process.HasExited == false)
                    {
                        process.WaitForExit(500);

                        if (30 == nCount)
                        {
                            process.Kill();
                            MessageBox.Show("오류 발생");
                            bSuccess = false;
                            break;
                        }
                    }

                    if (bSuccess == true)
                        MessageBox.Show("저장되었습니다.");

                    this.Cursor = Cursors.Default;
                }
            }
            return true;
        }

        public bool SaveHWP()
        {
            bool isHwpSetup = false;
            isHwpSetup = m_DetectPage.HwpCtrl.GetRegistry();

            //한글 설치여부
            if (isHwpSetup == false)
            {
                MessageBox.Show("아래한글이 설치되지 않았습니다.");
                return false;
            }

            string SavePath = "";
            
            saveFileDialog1.Filter = "한글 문서 (*.hwp)|*.hwp";
            saveFileDialog1.FileName = "대응_이력_보고서";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                m_ActionPage.FileWriter();
                m_ActionPage.SetHwpData();

                SavePath = saveFileDialog1.FileName;
                SavePath = subGap(SavePath);
                SavePath = SavePath.Replace("\\", "/");
                SavePath = SavePath.Replace("/", "\\\\");

                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                info.Arguments = "3 " + SavePath;
                info.CreateNoWindow = true;
                info.FileName = Application.StartupPath + "\\HwpReport.exe";

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo = info;

                process.Start();

                this.Cursor = Cursors.WaitCursor;
                int nCount = 0;
                bool bSuccess = true;
                while (process.HasExited == false)
                {
                    process.WaitForExit(500);

                    if (30 == nCount)
                    {
                        process.Kill();
                        MessageBox.Show("오류 발생");
                        bSuccess = false;
                        break;
                    }
                }

                if (bSuccess == true)
                    MessageBox.Show("저장되었습니다.");

                this.Cursor = Cursors.Default;
            }
            return true;
        }
        
        //공백 제거
        private string subGap(string _str)
        {
            int num = 0;//중간 띄어쓰기 위치
            string tmp = _str;
            while (tmp.IndexOf(" ") > 0)
            {
                num = tmp.IndexOf(" ");
                string tmp1 = tmp.Substring(0, num);

                tmp1 += "_" + tmp.Substring(num + 1);
                tmp = tmp1;
            }
            return tmp;
        }

        public void SetComboText(string szStartDate, string szEndDate)
        {
            m_NotOperation.ComboTxtDate(szStartDate, szEndDate);
        }
    }
}
