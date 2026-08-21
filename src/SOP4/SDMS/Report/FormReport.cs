using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnE.Spatial;
using SDMS.Data;
using System.IO;

namespace SDMS
{
    public partial class FormReport : Form
    {
        private ReportMode m_nReportPage = ReportMode.DetectFireAnalyze;
        public ReportMode ReportPage
        {
            get { return m_nReportPage; }
            set { m_nReportPage = value; }
        }

        private SaveFileDialog saveFileDialog1 = new SaveFileDialog();

        private Report.ReactionManager m_DetectMgr = new Report.ReactionManager();
        private Report.ReactionManager m_ActionMgr = new Report.ReactionManager();

        private Report.ReactionPSMManager m_DetectPSMMgr = new Report.ReactionPSMManager();
        private Report.ReactionPSMManager m_ActionPSMMgr = new Report.ReactionPSMManager();

        private Report.ReactionIntrusionManager m_DetectIntrusionMgr = new Report.ReactionIntrusionManager();
        private Report.ReactionIntrusionManager m_ActionIntrusionMgr = new Report.ReactionIntrusionManager();

        private ParetoPage m_paretoPage = null;
        private DetectPage m_DetectPage = null;
        private NotOperationPage m_NotOperationPage = null;
        private ActionPage m_ActionPage = null;
        private SMSPage m_SMSPage = null;

        private ParetoPSMPage m_paretoPSMPage = null;
        private DetectPSMPage m_DetectPSMPage = null;
        private NotOperationPSMPage m_NotOperationPSMPage = null;
        private ActionPSMPage m_ActionPSMPage = null;
        private SMSPSMPage m_SMSPSMPage = null;
         
        private ParetoIntrusionPage m_paretoIntrusionPage = null;
        private DetectIntrusionPage m_DetectIntrusionPage = null;
        private NotOperationIntrusionPage m_NotOperationIntrusionPage = null;
        private ActionIntrusionPage m_ActionIntrusionPage = null;
        private SMSIntrusionPage m_SMSIntrusionPage = null;

        //private DetectPage m_DetectPage = null;
        //private NotOperationPage m_NotOperation = null;
        //private ActionPage m_ActionPage = null;
        //private SMSPage m_SMSPage = null;

        private List<Panel> m_pagePanels = new List<Panel>();

        private string m_strHWPPath = null;
        private string m_strLogoFileName = string.Empty;
        public FormReport()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            InitializeComponent();

            m_strLogoFileName = GetReportLogoFileName();

            m_DetectPage = new DetectPage(m_DetectMgr);
            m_paretoPage = new ParetoPage(UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR, m_DetectMgr, m_DetectPage);
            m_NotOperationPage = new NotOperationPage(m_DetectMgr);
            m_ActionPage = new ActionPage(m_ActionMgr);
            m_SMSPage = new SMSPage();

            if (UnE.SOP.ProxySOP.Instance.UsePSM)
            {
                m_DetectPSMPage = new DetectPSMPage(m_DetectPSMMgr);
                m_paretoPSMPage = new ParetoPSMPage(m_DetectPSMMgr, m_DetectPSMPage);
                m_NotOperationPSMPage = new NotOperationPSMPage(m_DetectPSMMgr);
                m_ActionPSMPage = new ActionPSMPage(m_ActionPSMMgr);
                m_SMSPSMPage = new SMSPSMPage();
            }

            m_DetectIntrusionPage = new DetectIntrusionPage(m_DetectIntrusionMgr);
            m_paretoIntrusionPage = new ParetoIntrusionPage(UnE.Sensor.IFacility.FacilityType.Intrusion_S1, m_DetectIntrusionMgr, m_DetectIntrusionPage);
            m_NotOperationIntrusionPage = new NotOperationIntrusionPage(m_DetectIntrusionMgr);
            m_ActionIntrusionPage = new ActionIntrusionPage(m_ActionIntrusionMgr);
            m_SMSIntrusionPage = new SMSIntrusionPage();

            m_paretoPage.Location = new Point(0, 0);
            m_paretoPage.TopLevel = false;
            m_paretoPage.Parent = m_paretoPanel;
            m_paretoPage.Dock = DockStyle.Fill;
            m_paretoPanel.Controls.Add(m_paretoPage);
            m_paretoPage.Show();

            m_DetectPage.Location = new Point(0, 0);
            m_DetectPage.TopLevel = false;
            m_DetectPage.Parent = m_DetectPanel;
            m_DetectPage.Dock = DockStyle.Fill;
            m_DetectPanel.Controls.Add(m_DetectPage);
            m_DetectPage.Show();

            m_NotOperationPage.Location = new Point(0, 0);
            m_NotOperationPage.TopLevel = false;
            m_NotOperationPage.Parent = m_NotOperationPanel;
            m_NotOperationPage.Dock = DockStyle.Fill;
            m_NotOperationPanel.Controls.Add(m_NotOperationPage);
            m_NotOperationPage.Show();

            m_ActionPage.Location = new Point(0, 0);
            m_ActionPage.TopLevel = false;
            m_ActionPage.Parent = m_ActionPanel;
            m_ActionPage.Dock = DockStyle.Fill;
            m_ActionPanel.Controls.Add(m_ActionPage);
            m_ActionPage.Show();

            m_SMSPage.Location = new Point(0, 0);
            m_SMSPage.TopLevel = false;
            m_SMSPage.Parent = m_SmsPanel;
            m_SMSPage.Dock = DockStyle.Fill;
            m_SmsPanel.Controls.Add(m_SMSPage);
            m_SMSPage.Show();

            if (UnE.SOP.ProxySOP.Instance.UsePSM)
            {
                m_paretoPSMPage.Location = new Point(0, 0);
                m_paretoPSMPage.TopLevel = false;
                m_paretoPSMPage.Parent = m_paretoPSMPanel;
                m_paretoPSMPage.Dock = DockStyle.Fill;
                m_paretoPSMPanel.Controls.Add(m_paretoPSMPage);
                m_paretoPSMPage.Show();

                m_DetectPSMPage.Location = new Point(0, 0);
                m_DetectPSMPage.TopLevel = false;
                m_DetectPSMPage.Parent = m_DetectPSMPanel;
                m_DetectPSMPage.Dock = DockStyle.Fill;
                m_DetectPSMPanel.Controls.Add(m_DetectPSMPage);
                m_DetectPSMPage.Show();

                m_NotOperationPSMPage.Location = new Point(0, 0);
                m_NotOperationPSMPage.TopLevel = false;
                m_NotOperationPSMPage.Parent = m_NotOperationPSMPanel;
                m_NotOperationPSMPage.Dock = DockStyle.Fill;
                m_NotOperationPSMPanel.Controls.Add(m_NotOperationPSMPage);
                m_NotOperationPSMPage.Show();

                m_ActionPSMPage.Location = new Point(0, 0);
                m_ActionPSMPage.TopLevel = false;
                m_ActionPSMPage.Parent = m_ActionPSMPanel;
                m_ActionPSMPage.Dock = DockStyle.Fill;
                m_ActionPSMPanel.Controls.Add(m_ActionPSMPage);
                m_ActionPSMPage.Show();

                m_SMSPSMPage.Location = new Point(0, 0);
                m_SMSPSMPage.TopLevel = false;
                m_SMSPSMPage.Parent = m_SmsPSMPanel;
                m_SMSPSMPage.Dock = DockStyle.Fill;
                m_SmsPSMPanel.Controls.Add(m_SMSPSMPage);
                m_SMSPSMPage.Show();
            }

            m_paretoIntrusionPage.Location = new Point(0, 0);
            m_paretoIntrusionPage.TopLevel = false;
            m_paretoIntrusionPage.Parent = m_paretoIntrusionPanel;
            m_paretoIntrusionPage.Dock = DockStyle.Fill;
            m_paretoIntrusionPanel.Controls.Add(m_paretoIntrusionPage);
            m_paretoIntrusionPage.Show();

            m_DetectIntrusionPage.Location = new Point(0, 0);
            m_DetectIntrusionPage.TopLevel = false;
            m_DetectIntrusionPage.Parent = m_DetectIntrusionPanel;
            m_DetectIntrusionPage.Dock = DockStyle.Fill;
            m_DetectIntrusionPanel.Controls.Add(m_DetectIntrusionPage);
            m_DetectIntrusionPage.Show();

            m_NotOperationIntrusionPage.Location = new Point(0, 0);
            m_NotOperationIntrusionPage.TopLevel = false;
            m_NotOperationIntrusionPage.Parent = m_NotOperationIntrusionPanel;
            m_NotOperationIntrusionPage.Dock = DockStyle.Fill;
            m_NotOperationIntrusionPanel.Controls.Add(m_NotOperationIntrusionPage);
            m_NotOperationIntrusionPage.Show();

            m_ActionIntrusionPage.Location = new Point(0, 0);
            m_ActionIntrusionPage.TopLevel = false;
            m_ActionIntrusionPage.Parent = m_ActionIntrusionPanel;
            m_ActionIntrusionPage.Dock = DockStyle.Fill;
            m_ActionIntrusionPanel.Controls.Add(m_ActionIntrusionPage);
            m_ActionIntrusionPage.Show();

            m_SMSIntrusionPage.Location = new Point(0, 0);
            m_SMSIntrusionPage.TopLevel = false;
            m_SMSIntrusionPage.Parent = m_SmsIntrusionPanel;
            m_SMSIntrusionPage.Dock = DockStyle.Fill;
            m_SmsIntrusionPanel.Controls.Add(m_SMSIntrusionPage);
            m_SMSIntrusionPage.Show();

            SetVisibleHWPExport();
            SetPanelVisible();
        }

        private void SetVisibleHWPExport()
        {
            bool isHwpSetup = false;
            isHwpSetup = m_DetectPage.HwpCtrl.GetRegistry(ref m_strHWPPath);

            m_paretoPage.SetVisibleHWPExport(isHwpSetup);
            m_DetectPage.SetVisibleHWPExport(isHwpSetup);
            m_NotOperationPage.SetVisibleHWPExport(isHwpSetup);
            m_ActionPage.SetVisibleHWPExport(isHwpSetup);

            if (UnE.SOP.ProxySOP.Instance.UsePSM)
            {
                m_paretoPSMPage.SetVisibleHWPExport(isHwpSetup);
                m_DetectPSMPage.SetVisibleHWPExport(isHwpSetup);
                m_NotOperationPSMPage.SetVisibleHWPExport(isHwpSetup);
                m_ActionPSMPage.SetVisibleHWPExport(isHwpSetup);
            }

            m_paretoIntrusionPage.SetVisibleHWPExport(isHwpSetup);
            m_DetectIntrusionPage.SetVisibleHWPExport(isHwpSetup);
            m_NotOperationIntrusionPage.SetVisibleHWPExport(isHwpSetup);
            m_ActionIntrusionPage.SetVisibleHWPExport(isHwpSetup);
        }

        private void AddPanel(Panel panel)
        {
            m_pagePanels.Add(panel);
            panel.Visible = false;
        }

        private void ShowPanel(ReportMode mode)
        {
            foreach (Panel panel in m_pagePanels)
            {
                panel.Visible = false;
            }

            m_nReportPage = mode;

            switch (mode)
            {
                case ReportMode.DetectFireAnalyze:
                    m_paretoPanel.Visible = true;
                    break;

                case ReportMode.DetectFire:
                    m_DetectPanel.Visible = true;
                    break;

                case ReportMode.ProcessFire:
                    m_NotOperationPanel.Visible = true;
                    break;

                case ReportMode.ActionFire:
                    m_ActionPanel.Visible = true;
                    break;

                case ReportMode.SMSFire:
                    m_SmsPanel.Visible = true;
                    break;

                case ReportMode.DetectPSMAnalyze:
                    m_paretoPSMPanel.Visible = true;
                    break;

                case ReportMode.DetectPSM:
                    m_DetectPSMPanel.Visible = true;
                    break;

                case ReportMode.ProcessPSM:
                    m_NotOperationPSMPanel.Visible = true;
                    break;

                case ReportMode.ActionPSM:
                    m_ActionPSMPanel.Visible = true;
                    break;

                case ReportMode.SMSPSM:
                    m_SmsPSMPanel.Visible = true;
                    break;

                case ReportMode.DetectIntrusionAnalyze:
                    m_paretoIntrusionPanel.Visible = true;
                    break;

                case ReportMode.DetectIntrusion:
                    m_DetectIntrusionPanel.Visible = true;
                    break;

                case ReportMode.ProcessIntrusion:
                    m_NotOperationIntrusionPanel.Visible = true;
                    break;

                case ReportMode.ActionIntrusion:
                    m_ActionIntrusionPanel.Visible = true;
                    break;

                case ReportMode.SMSIntrusion:
                    m_SmsIntrusionPanel.Visible = true;
                    break;
            } 
        }

        private void SetPanelVisible()
        {
            AddPanel(m_paretoPanel);
            AddPanel(m_DetectPanel);
            AddPanel(m_NotOperationPanel);
            AddPanel(m_ActionPanel);
            AddPanel(m_SmsPanel);
            AddPanel(m_paretoPSMPanel);
            AddPanel(m_DetectPSMPanel);
            AddPanel(m_NotOperationPSMPanel);
            AddPanel(m_ActionPSMPanel);
            AddPanel(m_SmsPSMPanel); 
            AddPanel(m_paretoIntrusionPanel);
            AddPanel(m_DetectIntrusionPanel);
            AddPanel(m_NotOperationIntrusionPanel);
            AddPanel(m_ActionIntrusionPanel);
            AddPanel(m_SmsIntrusionPanel);
            ShowPanel(m_nReportPage);
        }

        private void FormReport_VisibleChanged(object sender, EventArgs e)
        {
            FormMain.Instance.PageHome.Redraw3DView();

            //if (this.Visible == true && m_szGroupName != "")
            //{

            //    //SelectReport(m_szGroupName, m_szBuildingName, m_szFloorName, mStartDate, mEndDate);
            //}
        }

        private DateTime m_dtStartDate;
        private DateTime m_dtEndDate;

        /// <summary>
        /// 0:분
        /// 1:시
        /// 2:일
        /// 3:주
        /// 4:월
        /// 5:연
        /// </summary>
        private int m_nSplitUnitOfMeansure = -1;
        private int m_nSplitUnitOfMeansureDetail = -1;
        private int m_nViewCount = -1;

        public void LoadReport(string strGroupName, string strBuildingName, string strFloorName, DateTime startDate, DateTime EndDate, int nSplitUnitOfMeansure, int nSplitUnitOfMeansureDetail, int nViewCount)
        {
            m_dtStartDate = startDate;
            m_dtEndDate = EndDate;
            m_nSplitUnitOfMeansure = nSplitUnitOfMeansure;
            m_nSplitUnitOfMeansureDetail = nSplitUnitOfMeansureDetail;
            m_nViewCount = nViewCount;

            string strStartDate = startDate.ToShortDateString();
            string strEndDate = EndDate.ToShortDateString();

            ArrayList arrSelectZoneList = ZoneManager.Instance.FindZoneList(strGroupName, strBuildingName, strFloorName);

            /*m_NotOperation.ComboTxtDate(strStartDate, strEndDate);
            m_DetectPage.ComboSubmit(strGroupName, strBuildingName, strFloorName);
            m_NotOperation.ComboSubmit(strGroupName, strBuildingName, strFloorName);
            m_paretoPage.ComboSubmit(strGroupName, strBuildingName, strFloorName);

            m_ActionPage.SetLabelString(strGroupName + "  " + strBuildingName + "  " + strFloorName);
            m_ActionPSMPage.SetLabelString(strGroupName + "  " + strBuildingName + "  " + strFloorName);

            m_DetectMgr.DataClear();
            m_DetectMgr.ZoneSubmit(arrSelectZoneList, startDate, EndDate);

            m_paretoPage.Load_DataGrid(arrSelectZoneList, nViewCount);
            m_DetectPage.Load_DataGrid();
            m_NotOperation.Load_DataGrid();

            m_SMSPage.ZoneSubmit(arrSelectZoneList, startDate, EndDate);
            m_SMSPage.LoadDataGrid();

            //그래프그리기
            m_DetectPage.CreateBarChart(startDate, EndDate, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount);
            m_NotOperation.CreateBarChart(m_NotOperation.PercentBarChart);*/

            if (m_nReportPage == ReportMode.DetectFireAnalyze)
            {
                m_paretoPage.ComboSubmit(strGroupName, strBuildingName, strFloorName);
                m_paretoPage.Load_DataGrid(arrSelectZoneList, strGroupName, strBuildingName, strFloorName, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount);
                //m_paretoPage.Load_DataGrid(arrSelectZoneList, nViewCount);
            }
            else if (m_nReportPage == ReportMode.DetectFire)
            {
                if (m_DetectMgr.NeedRefresh(arrSelectZoneList, startDate, EndDate, m_DetectPage.RefreshCheckData))
                {
                    m_DetectPage.RefreshCheckData.ViewCount = nViewCount;

                    m_DetectMgr.DataClear();
                    m_DetectMgr.ZoneSubmit(arrSelectZoneList, startDate, EndDate);

                    m_DetectPage.ComboSubmit(strGroupName, strBuildingName, strFloorName);
                    m_DetectPage.Load_DataGrid(m_paretoPage.SensorHistories, m_paretoPage.EquipZoneHistories);
                    m_DetectPage.CreateBarChart(startDate, EndDate, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount);
                }
                else if (m_DetectPage.RefreshCheckData.ViewCount != nViewCount)
                {
                    // 다른 조건은 모두 동일한데 Graph의 최대 표기 개수만 달라진 경우
                    m_DetectPage.RefreshCheckData.ViewCount = nViewCount;
                    m_DetectPage.CreateBarChart(startDate, EndDate, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount);
                }
            }
            else if (m_nReportPage == ReportMode.ProcessFire)
            {
                if (m_DetectMgr.NeedRefresh(arrSelectZoneList, startDate, EndDate, m_NotOperationPage.RefreshCheckData))
                {
                    m_DetectMgr.DataClear();
                    m_DetectMgr.ZoneSubmit(arrSelectZoneList, startDate, EndDate);

                    m_NotOperationPage.ComboTxtDate(startDate.ToShortDateString(), EndDate.ToShortDateString());
                    m_NotOperationPage.ComboSubmit(strGroupName, strBuildingName, strFloorName);
                    m_NotOperationPage.Load_DataGrid();
                    m_NotOperationPage.CreateBarChart(m_NotOperationPage.PercentBarChart);
                }
            }
            else if (m_nReportPage == ReportMode.ActionFire)
            {
                m_ActionPage.SetLabelString(strGroupName + "  " + strBuildingName + "  " + strFloorName);
            }
            else if (m_nReportPage == ReportMode.SMSFire)
            {
                if (m_DetectMgr.NeedRefresh(arrSelectZoneList, startDate, EndDate, m_SMSPage.RefreshCheckData))
                {
                    m_SMSPage.ZoneSubmit(arrSelectZoneList, startDate, EndDate);
                    m_SMSPage.LoadDataGrid();
                }
            } 
        }

        public void LoadReportForDetectPSMAnalyze(int[] nBuildingIDs, DateTime startDate, DateTime EndDate, int nSplitUnitOfMeansure, int nSplitUnitOfMeansureDetail, int nViewCount, string strLocationName)
        {
            m_dtStartDate = startDate;
            m_dtEndDate = EndDate;
            m_nSplitUnitOfMeansure = nSplitUnitOfMeansure;
            m_nSplitUnitOfMeansureDetail = nSplitUnitOfMeansureDetail;
            m_nViewCount = nViewCount;

            string strStartDate = startDate.ToShortDateString();
            string strEndDate = EndDate.ToShortDateString();

            ArrayList arrSelectZoneList = null;

            if (nBuildingIDs[0] == -1)
            {
                arrSelectZoneList = ZoneManager.Instance.FindZoneList("모든 건물 그룹", "모든 건물", "모든 층");
                m_paretoPSMPage.ComboSubmit("모든 시설");
            }
            else
            {
                arrSelectZoneList = ZoneManager.Instance.FindZoneListByEquipZoneID(nBuildingIDs);
                m_paretoPSMPage.ComboSubmit(strLocationName);
            }

            //if (m_DetectPSMMgr.NeedRefresh(arrSelectZoneList, startDate, EndDate, m_DetectPSMPage.RefreshCheckData))
            {
                m_DetectPSMMgr.DataClear();
                m_DetectPSMMgr.ZoneSubmit(arrSelectZoneList, startDate, EndDate);

                m_paretoPSMPage.Load_DataGrid(arrSelectZoneList, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, m_nViewCount);
            }
            /*else if (m_paretoPSMPage.ViewCount != m_nViewCount)
            {
                // 다른 조건은 모두 동일한데 Graph의 최대 표기 개수만 달라진 경우
                m_paretoPSMPage.Load_DataGrid(arrSelectZoneList, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, m_nViewCount);
            }*/
        }

        public void LoadReportForDetectPSM(int[] nBuildingIDs, DateTime startDate, DateTime EndDate, int nSplitUnitOfMeansure, int nSplitUnitOfMeansureDetail, int nViewCount, string strLocationName)
        {
            m_dtStartDate = startDate;
            m_dtEndDate = EndDate;
            m_nSplitUnitOfMeansure = nSplitUnitOfMeansure;
            m_nSplitUnitOfMeansureDetail = nSplitUnitOfMeansureDetail;
            m_nViewCount = nViewCount;

            string strStartDate = startDate.ToShortDateString();
            string strEndDate = EndDate.ToShortDateString();

            ArrayList arrSelectZoneList = null;

            if (nBuildingIDs[0] == -1)
            {
                arrSelectZoneList = ZoneManager.Instance.FindZoneList("모든 건물 그룹", "모든 건물", "모든 층");
                m_DetectPSMPage.ComboSubmit("모든 시설");
            }
            else
            {
                arrSelectZoneList = ZoneManager.Instance.FindZoneListByEquipZoneID(nBuildingIDs);
                m_DetectPSMPage.ComboSubmit(strLocationName);
            }

            if (m_DetectPSMMgr.NeedRefresh(arrSelectZoneList, startDate, EndDate, m_DetectPSMPage.RefreshCheckData))
            {
                m_DetectPSMPage.RefreshCheckData.ViewCount = nViewCount;

                m_DetectPSMMgr.DataClear();
                m_DetectPSMMgr.ZoneSubmit(arrSelectZoneList, startDate, EndDate);

                m_DetectPSMPage.Load_DataGrid(m_paretoPSMPage.SensorHistories, m_paretoPSMPage.TankHistories, m_paretoPSMPage.EquipZoneHistories, m_paretoPSMPage.MaterialHistories);

                //그래프그리기
                m_DetectPSMPage.CreateBarChart(startDate, EndDate, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount);
            }
            else if (m_DetectPSMPage.RefreshCheckData.ViewCount != nViewCount)
            {
                // 다른 조건은 모두 동일한데 Graph의 최대 표기 개수만 달라진 경우
                m_DetectPSMPage.RefreshCheckData.ViewCount = nViewCount;

                //그래프그리기
                m_DetectPSMPage.CreateBarChart(startDate, EndDate, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount);
            }
        }

        public void LoadReportForNotOperationPSM(int[] nBuildingIDs, DateTime startDate, DateTime EndDate, string strLocationName)
        {
            m_dtStartDate = startDate;
            m_dtEndDate = EndDate;

            string strStartDate = startDate.ToShortDateString();
            string strEndDate = EndDate.ToShortDateString();

            ArrayList arrSelectZoneList = null;

            if (nBuildingIDs[0] == -1)
            {
                arrSelectZoneList = ZoneManager.Instance.FindZoneList("모든 건물 그룹", "모든 건물", "모든 층");
                m_NotOperationPSMPage.ComboSubmit("모든 시설");
            }
            else
            {
                arrSelectZoneList = ZoneManager.Instance.FindZoneListByEquipZoneID(nBuildingIDs);
                m_NotOperationPSMPage.ComboSubmit(strLocationName);  
            }

            m_NotOperationPSMPage.ComboTxtDate(strStartDate, strEndDate);

            if (m_DetectPSMMgr.NeedRefresh(arrSelectZoneList, startDate, EndDate, m_NotOperationPSMPage.RefreshCheckData))
            {
                m_DetectPSMMgr.DataClear();
                m_DetectPSMMgr.ZoneSubmit(arrSelectZoneList, startDate, EndDate);

                m_NotOperationPSMPage.Load_DataGrid();

                m_NotOperationPSMPage.CreateBarChart(m_NotOperationPSMPage.PercentBarChart);
            }
        }

        public void LoadReportForSMSPSM(int[] nBuildingIDs, DateTime startDate, DateTime EndDate, string strLocationName)
        {
            m_dtStartDate = startDate;
            m_dtEndDate = EndDate;

            string strStartDate = startDate.ToShortDateString();
            string strEndDate = EndDate.ToShortDateString();

            ArrayList arrSelectZoneList = null;

            if (nBuildingIDs[0] == -1)
            {
                arrSelectZoneList = ZoneManager.Instance.FindZoneList("모든 건물 그룹", "모든 건물", "모든 층");
            }
            else
            {
                arrSelectZoneList = ZoneManager.Instance.FindZoneListByEquipZoneID(nBuildingIDs);
            }

            if (m_DetectPSMMgr.NeedRefresh(arrSelectZoneList, startDate, EndDate, m_SMSPSMPage.RefreshCheckData))
            {
                m_SMSPSMPage.ZoneSubmit(arrSelectZoneList, startDate, EndDate);
                m_SMSPSMPage.LoadDataGrid();
            }
        }
         
        public void LoadReportForIntrusion(string strGroupName, string strBuildingName, string strFloorName, DateTime startDate, DateTime EndDate, int nSplitUnitOfMeansure, int nSplitUnitOfMeansureDetail, int nViewCount)
        {
            m_dtStartDate = startDate;
            m_dtEndDate = EndDate;
            m_nSplitUnitOfMeansure = nSplitUnitOfMeansure;
            m_nSplitUnitOfMeansureDetail = nSplitUnitOfMeansureDetail;
            m_nViewCount = nViewCount;

            string strStartDate = startDate.ToShortDateString();
            string strEndDate = EndDate.ToShortDateString();

            ArrayList arrSelectZoneList = ZoneManager.Instance.FindZoneList(strGroupName, strBuildingName, strFloorName);
             
            if (m_nReportPage == ReportMode.DetectIntrusionAnalyze)
            {
                m_paretoIntrusionPage.ComboSubmit(strGroupName, strBuildingName, strFloorName);
                m_paretoIntrusionPage.Load_DataGrid(arrSelectZoneList, strGroupName, strBuildingName, strFloorName, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount);
            }
            else if (m_nReportPage == ReportMode.DetectIntrusion)
            {
                if (m_DetectIntrusionMgr.NeedRefresh(arrSelectZoneList, startDate, EndDate, m_DetectIntrusionPage.RefreshCheckData))
                {
                    m_DetectIntrusionPage.RefreshCheckData.ViewCount = nViewCount;

                    m_DetectIntrusionMgr.DataClear();
                    m_DetectIntrusionMgr.ZoneSubmit(arrSelectZoneList, startDate, EndDate);

                    m_DetectIntrusionPage.ComboSubmit(strGroupName, strBuildingName, strFloorName);
                    m_DetectIntrusionPage.Load_DataGrid(m_paretoIntrusionPage.SensorHistories, m_paretoIntrusionPage.EquipZoneHistories);
                    m_DetectIntrusionPage.CreateBarChart(startDate, EndDate, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount);
                }
                else if (m_DetectIntrusionPage.RefreshCheckData.ViewCount != nViewCount)
                {
                    // 다른 조건은 모두 동일한데 Graph의 최대 표기 개수만 달라진 경우
                    m_DetectIntrusionPage.RefreshCheckData.ViewCount = nViewCount;
                    m_DetectIntrusionPage.CreateBarChart(startDate, EndDate, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount);
                }
            }
            else if (m_nReportPage == ReportMode.ProcessIntrusion)
            {
                if (m_DetectIntrusionMgr.NeedRefresh(arrSelectZoneList, startDate, EndDate, m_NotOperationIntrusionPage.RefreshCheckData))
                {
                    m_DetectIntrusionMgr.DataClear();
                    m_DetectIntrusionMgr.ZoneSubmit(arrSelectZoneList, startDate, EndDate);

                    m_NotOperationIntrusionPage.ComboTxtDate(startDate.ToShortDateString(), EndDate.ToShortDateString());
                    m_NotOperationIntrusionPage.ComboSubmit(strGroupName, strBuildingName, strFloorName);
                    m_NotOperationIntrusionPage.Load_DataGrid();
                    m_NotOperationIntrusionPage.CreateBarChart(m_NotOperationIntrusionPage.PercentBarChart);
                }
            }
            else if (m_nReportPage == ReportMode.ActionIntrusion)
            {
                m_ActionIntrusionPage.SetLabelString(strGroupName + "  " + strBuildingName + "  " + strFloorName);
            }
            else if (m_nReportPage == ReportMode.SMSIntrusion)
            {
                if (m_DetectIntrusionMgr.NeedRefresh(arrSelectZoneList, startDate, EndDate, m_SMSIntrusionPage.RefreshCheckData))
                {
                    m_SMSIntrusionPage.ZoneSubmit(arrSelectZoneList, startDate, EndDate);
                    m_SMSIntrusionPage.LoadDataGrid();
                }
            }
        }

        public void ShowDetectAnalyze()
        {
            ShowPanel(ReportMode.DetectFireAnalyze);
        }

        public void ShowDetectReport()
        {
            ShowPanel(ReportMode.DetectFire);
        }

        public void ShowProcessHistoryReport()
        {
            ShowPanel(ReportMode.ProcessFire);
        }

        public void ShowReactionHistoryReport()
        {
            ShowPanel(ReportMode.ActionFire);
        }

        public void ShowSmsHistoryReport()
        {
            ShowPanel(ReportMode.SMSFire);
        }

        public void ShowDetectPSMAnalyze()
        {
            ShowPanel(ReportMode.DetectPSMAnalyze);
        }

        public void ShowDetectPSMReport()
        {
            ShowPanel(ReportMode.DetectPSM);
        }

        public void ShowNotOperationPSMReport()
        {
            ShowPanel(ReportMode.ProcessPSM);
        }

        public void ShowActionPSMReport()
        {
            ShowPanel(ReportMode.ActionPSM);
        }

        public void ShowSMSPSMReport()
        {
            ShowPanel(ReportMode.SMSPSM);
        }

        public void ShowDetectIntrusionAnalyze()
        {
            ShowPanel(ReportMode.DetectIntrusionAnalyze);
        }

        public void ShowDetectIntrusionReport()
        {
            ShowPanel(ReportMode.DetectIntrusion);
        }

        public void ShowProcessIntrusionHistoryReport()
        {
            ShowPanel(ReportMode.ProcessIntrusion);
        }

        public void ShowReactionIntrusionHistoryReport()
        {
            ShowPanel(ReportMode.ActionIntrusion);
        }

        public void ShowSmsIntrusionHistoryReport()
        {
            ShowPanel(ReportMode.SMSIntrusion);
        }

        private string GetStringDisaster(ReportMode mode)
        {
            string strDisaster = "";
            if (mode == ReportMode.DetectFireAnalyze || mode == ReportMode.DetectFire || mode == ReportMode.ProcessFire || mode == ReportMode.ActionFire)
                strDisaster = "화재";
            else if (mode == ReportMode.DetectPSMAnalyze || mode == ReportMode.DetectPSM || mode == ReportMode.ProcessPSM || mode == ReportMode.ActionPSM)
                strDisaster = "누출";
            else if (mode == ReportMode.DetectIntrusionAnalyze || mode == ReportMode.DetectIntrusion || mode == ReportMode.ProcessIntrusion || mode == ReportMode.ActionIntrusion)
                strDisaster = "방범";

            return strDisaster;
        }

        public bool SaveHWPForPareto(IParetoPage page)
        {
            bool isHwpSetup = false;
            if (page is SDMS.ParetoIntrusionPage)
                isHwpSetup = m_DetectIntrusionPage.HwpCtrl.GetRegistry(ref m_strHWPPath);
            else
                isHwpSetup = m_DetectPage.HwpCtrl.GetRegistry(ref m_strHWPPath);

            //한글 설치여부
            //if (isHwpSetup == false)
            //{
            //    MessageBox.Show("아래한글이 설치되지 않았습니다.");
            //    return false;
            //}

            bool isSuccess = false;
            ReportMode mode = page.GetReportMode();
            string strDisaster = GetStringDisaster(mode);

            string strSavePath = GetHWPFilePath(page.GetHWPFileName(), isHwpSetup);

            if (strSavePath == null)
                return isSuccess;

            //화면캡쳐
            page.ControllCapture();
            page.FileWriter();
            page.SetHwpData();

            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.CreateNoWindow = true;
            // 기존 Hwp 작성 방식
            //info.Arguments = ((int)mode).ToString() + " " + strSavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
            //info.FileName = Application.StartupPath + "\\HwpReport.exe";
            // 대용량 속도 문제로 개선한 Hml 작성 방식
            info.Arguments = 1 + " " + strDisaster + " " + strSavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
            info.FileName = Application.StartupPath + "\\HmlReport.exe";

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo = info;

            process.Start();
            this.Cursor = Cursors.WaitCursor;

            int nCount = 0;
            isSuccess = true;
            while (process.HasExited == false)
            {
                process.WaitForExit(500);

                if (30 == nCount)
                {
                    process.Kill();
                    MessageBox.Show("오류 발생");
                    isSuccess = false;
                    break;
                }
            }

            if (isSuccess == true)
            {
                if (isHwpSetup)
                    RunHWP(strSavePath);
                else
                {
                    int nIndex = strSavePath.LastIndexOf(@"\");
                    string filePath = strSavePath.Substring(0, nIndex);
                    System.Diagnostics.Process.Start(filePath);
                }
            }

            this.Cursor = Cursors.Default;
        
            return isSuccess;
        }

        public bool SaveHWPForDetectAndNotPoeration()
        {
            bool isHwpSetup = false;
            isHwpSetup = m_DetectPage.HwpCtrl.GetRegistry(ref m_strHWPPath);

            //한글 설치여부
            //if (isHwpSetup == false)
            //{
            //    MessageBox.Show("아래한글이 설치되지 않았습니다.");
            //    return false;
            //}

            string strSavePath = "";
            //saveFileDialog1.Filter = "한글 문서 (*.hwp)|*.hwp";

            string strDisaster = GetStringDisaster(m_nReportPage);
            if (m_nReportPage == ReportMode.DetectFire) //탐지
            {
                strSavePath = GetHWPFilePath("화재_탐지이력_보고서", isHwpSetup);

                if (strSavePath == null)
                    return false;
                //saveFileDialog1.FileName = "화재_탐지_보고서";
                //if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    //화면캡쳐
                    m_DetectPage.ControllCapture();
                    m_DetectPage.FileWriter();
                    m_DetectPage.SetHwpData();

                    //SavePath = saveFileDialog1.FileName;
                    //공백제거
                    //SavePath = subGap(SavePath);
                    //SavePath = SavePath.Replace("\\", "/");
                    //SavePath = SavePath.Replace("/", "\\\\"); 
                    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                    info.CreateNoWindow = true;
                    //info.Arguments = ((int)ReportMode.DetectFire).ToString() + " " + SavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
                    //info.FileName = Application.StartupPath + "\\HwpReport.exe";
                    info.Arguments = 2 + " " + strDisaster + " " + strSavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
                    info.FileName = Application.StartupPath + "\\HmlReport.exe";

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
                    {
                        if (isHwpSetup)
                            RunHWP(strSavePath);
                        else
                        {
                            int nIndex = strSavePath.LastIndexOf(@"\");
                            string filePath = strSavePath.Substring(0, nIndex);
                            System.Diagnostics.Process.Start(filePath);
                        }
                    }

                    this.Cursor = Cursors.Default;
                }
            }
            else if (m_nReportPage == ReportMode.ProcessFire) //처리
            {
                //saveFileDialog1.FileName = "처리_이력_보고서";
                //if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    strSavePath = GetHWPFilePath("화재_처리이력_보고서", isHwpSetup);

                    if (strSavePath == null)
                        return false;

                    m_NotOperationPage.ControllCapture();
                    m_NotOperationPage.FileWriter();
                    m_NotOperationPage.SetHwpData();

                    //SavePath = saveFileDialog1.FileName;
                    //공백제거
                    //SavePath = subGap(SavePath);
                    //SavePath = SavePath.Replace("\\", "/");
                    //SavePath = SavePath.Replace("/", "\\\\");

                    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                    info.CreateNoWindow = true;
                    //info.Arguments = ((int)ReportMode.ProcessFire).ToString() + " " + strSavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;                    
                    //info.FileName = Application.StartupPath + "\\HwpReport.exe";
                    info.Arguments = 3 + " " + strDisaster + " " + strSavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
                    info.FileName = Application.StartupPath + "\\HmlReport.exe";

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
                    {
                        if (isHwpSetup)
                            RunHWP(strSavePath);
                        else
                        {
                            int nIndex = strSavePath.LastIndexOf(@"\");
                            string filePath = strSavePath.Substring(0, nIndex);
                            System.Diagnostics.Process.Start(filePath);
                        }
                    }

                    this.Cursor = Cursors.Default;
                }
            }
            else if (m_nReportPage == ReportMode.DetectIntrusion) // 방범 탐지 이력
            {
                strSavePath = GetHWPFilePath("방범_탐지이력_보고서", isHwpSetup);

                if (strSavePath == null)
                    return false;
                //saveFileDialog1.FileName = "화재_탐지_보고서";
                //if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    //화면캡쳐
                    m_DetectIntrusionPage.ControllCapture();
                    m_DetectIntrusionPage.FileWriter();
                    m_DetectIntrusionPage.SetHwpData();

                    //SavePath = saveFileDialog1.FileName;
                    //공백제거
                    //SavePath = subGap(SavePath);
                    //SavePath = SavePath.Replace("\\", "/");
                    //SavePath = SavePath.Replace("/", "\\\\");

                    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                    info.CreateNoWindow = true;
                    //info.Arguments = ((int)ReportMode.DetectIntrusion).ToString() + " " + strSavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;                    
                    //info.FileName = Application.StartupPath + "\\HwpReport.exe";
                    info.Arguments = 2 + " " + strDisaster + " " + strSavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
                    info.FileName = Application.StartupPath + "\\HmlReport.exe";

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
                    {
                        if (isHwpSetup)
                            RunHWP(strSavePath);
                        else
                        {
                            int nIndex = strSavePath.LastIndexOf(@"\");
                            string filePath = strSavePath.Substring(0, nIndex);
                            System.Diagnostics.Process.Start(filePath);
                        }
                    }

                    this.Cursor = Cursors.Default;
                }
            }
            else if (m_nReportPage == ReportMode.ProcessIntrusion) //방범 처리 이력
            {
                //saveFileDialog1.FileName = "처리_이력_보고서";
                //if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    strSavePath = GetHWPFilePath("방범_처리이력_보고서", isHwpSetup);

                    if (strSavePath == null)
                        return false;

                    m_NotOperationIntrusionPage.ControllCapture();
                    m_NotOperationIntrusionPage.FileWriter();
                    m_NotOperationIntrusionPage.SetHwpData();

                    //SavePath = saveFileDialog1.FileName;
                    //공백제거
                    //SavePath = subGap(SavePath);
                    //SavePath = SavePath.Replace("\\", "/");
                    //SavePath = SavePath.Replace("/", "\\\\");

                    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                    info.CreateNoWindow = true;
                    //info.Arguments = ((int)ReportMode.ProcessIntrusion).ToString() + " " + strSavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
                    //info.FileName = Application.StartupPath + "\\HwpReport.exe";
                    info.Arguments = 3 + " " + strDisaster + " " + strSavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
                    info.FileName = Application.StartupPath + "\\HmlReport.exe";

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
                    {
                        if (isHwpSetup)
                            RunHWP(strSavePath);
                        else
                        {
                            int nIndex = strSavePath.LastIndexOf(@"\");
                            string filePath = strSavePath.Substring(0, nIndex);
                            System.Diagnostics.Process.Start(filePath);
                        }
                    }

                    this.Cursor = Cursors.Default;
                }
            }
            return true;
        }

        public bool SaveHWPForAction()
        {
            bool isHwpSetup = false;
            isHwpSetup = m_DetectPage.HwpCtrl.GetRegistry(ref m_strHWPPath);

            //한글 설치여부
            //if (isHwpSetup == false)
            //{
            //    MessageBox.Show("아래한글이 설치되지 않았습니다.");
            //    return false;
            //}

            string strSavePath  = string.Empty;
            if (m_nReportPage == ReportMode.ActionFire)
                strSavePath = GetHWPFilePath("화재_대응이력_보고서", isHwpSetup);
            else if (m_nReportPage == ReportMode.ActionIntrusion)
                strSavePath = GetHWPFilePath("방범_대응이력_보고서", isHwpSetup);

            if (strSavePath == null)
                return false;
            //string SavePath = "";

            //saveFileDialog1.Filter = "한글 문서 (*.hwp)|*.hwp";
            //saveFileDialog1.FileName = "대응_이력_보고서";
            //if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (m_nReportPage == ReportMode.ActionFire)
                {
                    m_ActionPage.FileWriter();
                    m_ActionPage.SetHwpData();
                }
                else if (m_nReportPage == ReportMode.ActionIntrusion)
                {
                    m_ActionIntrusionPage.FileWriter();
                    m_ActionIntrusionPage.SetHwpData();
                }

                string strDisaster = GetStringDisaster(m_nReportPage);

                //SavePath = saveFileDialog1.FileName;
                //SavePath = subGap(SavePath);
                //SavePath = SavePath.Replace("\\", "/");
                //SavePath = SavePath.Replace("/", "\\\\");

                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                info.CreateNoWindow = true;
                //if (m_nReportPage == ReportMode.ActionFire)
                //    info.Arguments = ((int)ReportMode.ActionFire).ToString() + " " + strSavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
                //else if (m_nReportPage == ReportMode.ActionIntrusion)
                //    info.Arguments = ((int)ReportMode.ActionIntrusion).ToString() + " " + strSavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
                //info.FileName = Application.StartupPath + "\\HwpReport.exe";                
                info.Arguments = 4 + " " + strDisaster + " " + strSavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
                info.FileName = Application.StartupPath + "\\HmlReport.exe";

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
                {
                    if (isHwpSetup)
                        RunHWP(strSavePath);
                    else
                    {
                        int nIndex = strSavePath.LastIndexOf(@"\");
                        string filePath = strSavePath.Substring(0, nIndex);
                        System.Diagnostics.Process.Start(filePath);
                    }
                }

                this.Cursor = Cursors.Default;
            }
            return true;
        }

        public bool SaveHWPForDetectPSM()
        {
            bool isHwpSetup = false;
            isHwpSetup = m_DetectPage.HwpCtrl.GetRegistry(ref m_strHWPPath);

            //한글 설치여부
            //if (isHwpSetup == false)
            //{
            //    MessageBox.Show("아래한글이 설치되지 않았습니다.");
            //    return false;
            //}

            string strSavePath = GetHWPFilePath("누출_탐지이력_보고서", isHwpSetup);

            if (strSavePath == null)
                return false;

            string strDisaster = GetStringDisaster(m_nReportPage);

            //화면캡쳐
            m_DetectPSMPage.ControllCapture();
            m_DetectPSMPage.FileWriter();
            m_DetectPSMPage.SetHwpData();

            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.CreateNoWindow = true;
            //info.Arguments = ((int)ReportMode.DetectPSM).ToString() + " " + strSavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
            //info.FileName = Application.StartupPath + "\\HwpReport.exe";
            info.Arguments = 2 + " " + strDisaster + " " + strSavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
            info.FileName = Application.StartupPath + "\\HmlReport.exe";

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
            {
                if (isHwpSetup)
                    RunHWP(strSavePath);
                else
                {
                    int nIndex = strSavePath.LastIndexOf(@"\");
                    string filePath = strSavePath.Substring(0, nIndex);
                    System.Diagnostics.Process.Start(filePath);
                }
            }

            this.Cursor = Cursors.Default;

            return true;
        }

        public bool SaveHWPForNotOperationPSM()
        {
            bool isHwpSetup = false;
            isHwpSetup = m_DetectPage.HwpCtrl.GetRegistry(ref m_strHWPPath);

            //한글 설치여부
            //if (isHwpSetup == false)
            //{
            //    MessageBox.Show("아래한글이 설치되지 않았습니다.");
            //    return false;
            //}

            string strSavePath = "";
            //saveFileDialog1.Filter = "한글 문서 (*.hwp)|*.hwp";

            strSavePath = GetHWPFilePath("누출_처리이력_보고서", isHwpSetup);

            if (strSavePath == null)
                return false;

            string strDisaster = GetStringDisaster(m_nReportPage);

            m_NotOperationPSMPage.ControllCapture();
            m_NotOperationPSMPage.FileWriter();
            m_NotOperationPSMPage.SetHwpData();

            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.CreateNoWindow = true;
            //info.Arguments = ((int)ReportMode.ProcessPSM).ToString() + " " + strSavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;            
            //info.FileName = Application.StartupPath + "\\HwpReport.exe";
            info.Arguments = 3 + " " + strDisaster + " " + strSavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
            info.FileName = Application.StartupPath + "\\HmlReport.exe";

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
            {
                if (isHwpSetup)
                    RunHWP(strSavePath);
                else
                {
                    int nIndex = strSavePath.LastIndexOf(@"\");
                    string filePath = strSavePath.Substring(0, nIndex);
                    System.Diagnostics.Process.Start(filePath);
                }
            }

            this.Cursor = Cursors.Default;

            return true;
        }

        public bool SaveHWPForActionPSM()
        {
            bool isHwpSetup = false;
            isHwpSetup = m_DetectPage.HwpCtrl.GetRegistry(ref m_strHWPPath);

            //한글 설치여부
            //if (isHwpSetup == false)
            //{
            //    MessageBox.Show("아래한글이 설치되지 않았습니다.");
            //    return false;
            //}

            string strSavePath = GetHWPFilePath("누출_대응이력_보고서", isHwpSetup);

            if (strSavePath == null)
                return false;

            string strDisaster = GetStringDisaster(m_nReportPage);

            m_ActionPSMPage.FileWriter();
            m_ActionPSMPage.SetHwpData();

            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.CreateNoWindow = true;
            //info.Arguments = ((int)ReportMode.ActionPSM).ToString() + " " + strSavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
            //info.FileName = Application.StartupPath + "\\HwpReport.exe";
            info.Arguments = 4 + " " + strDisaster + " " + strSavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
            info.FileName = Application.StartupPath + "\\HmlReport.exe";

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
            {
                if (isHwpSetup)
                    RunHWP(strSavePath);
                else
                {
                    int nIndex = strSavePath.LastIndexOf(@"\");
                    string filePath = strSavePath.Substring(0, nIndex);
                    System.Diagnostics.Process.Start(filePath);
                }
            }

            this.Cursor = Cursors.Default;

            return true;
        } 

        public bool SaveHWPForDetectAndNotPoerationIntrusion()
        {
            bool isHwpSetup = false;
            isHwpSetup = m_DetectPage.HwpCtrl.GetRegistry(ref m_strHWPPath);

            //한글 설치여부
            if (isHwpSetup == false)
            {
                MessageBox.Show("아래한글이 설치되지 않았습니다.");
                return false;
            }

            string SavePath = "";
            //saveFileDialog1.Filter = "한글 문서 (*.hwp)|*.hwp";

            if (m_nReportPage == ReportMode.DetectFire) //탐지
            {
                SavePath = GetHWPFilePath("화재_탐지이력_보고서", isHwpSetup);

                if (SavePath == null)
                    return false;
                //saveFileDialog1.FileName = "화재_탐지_보고서";
                //if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    //화면캡쳐
                    m_DetectPage.ControllCapture();
                    m_DetectPage.FileWriter();
                    m_DetectPage.SetHwpData();

                    //SavePath = saveFileDialog1.FileName;
                    //공백제거
                    //SavePath = subGap(SavePath);
                    //SavePath = SavePath.Replace("\\", "/");
                    //SavePath = SavePath.Replace("/", "\\\\");

                    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                    info.Arguments = ((int)ReportMode.DetectFire).ToString() + " " + SavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
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
                    {
                        RunHWP(SavePath);
                        //MessageBox.Show("저장되었습니다.");
                    }

                    this.Cursor = Cursors.Default;
                }
            }
            else if (m_nReportPage == ReportMode.ProcessFire) //처리
            {
                //saveFileDialog1.FileName = "처리_이력_보고서";
                //if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    SavePath = GetHWPFilePath("화재_처리이력_보고서", isHwpSetup);

                    if (SavePath == null)
                        return false;

                    m_NotOperationPage.ControllCapture();
                    m_NotOperationPage.FileWriter();
                    m_NotOperationPage.SetHwpData();

                    //SavePath = saveFileDialog1.FileName;
                    //공백제거
                    //SavePath = subGap(SavePath);
                    //SavePath = SavePath.Replace("\\", "/");
                    //SavePath = SavePath.Replace("/", "\\\\");

                    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                    info.Arguments = ((int)ReportMode.ProcessFire).ToString() + " " + SavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
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
                    {
                        RunHWP(SavePath);
                        //MessageBox.Show("저장되었습니다.");
                    }

                    this.Cursor = Cursors.Default;
                }
            }
            return true;
        }

        public bool SaveHWPForActionIntrusion()
        {
            bool isHwpSetup = false;
            isHwpSetup = m_DetectPage.HwpCtrl.GetRegistry(ref m_strHWPPath);

            //한글 설치여부
            if (isHwpSetup == false)
            {
                MessageBox.Show("아래한글이 설치되지 않았습니다.");
                return false;
            }

            string SavePath = GetHWPFilePath("대응_이력_보고서", isHwpSetup);

            if (SavePath == null)
                return false;
            //string SavePath = "";

            //saveFileDialog1.Filter = "한글 문서 (*.hwp)|*.hwp";
            //saveFileDialog1.FileName = "대응_이력_보고서";
            //if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                m_ActionPage.FileWriter();
                m_ActionPage.SetHwpData();

                //SavePath = saveFileDialog1.FileName;
                //SavePath = subGap(SavePath);
                //SavePath = SavePath.Replace("\\", "/");
                //SavePath = SavePath.Replace("/", "\\\\");

                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                info.Arguments = ((int)ReportMode.ActionFire).ToString() + " " + SavePath + " " + m_strLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
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
                {
                    RunHWP(SavePath);
                    //MessageBox.Show("저장되었습니다.");
                }

                this.Cursor = Cursors.Default;
            }
            return true;
        }

        private void RunHWP(string strFilePath)
        {
            string strHmlFilePath = strFilePath + ".hml";
            // 대용량인 경우 파일에 번호가 붙는다. ex) 화재탐지분석_날짜_1
            // 1번 파일을 열어준다.
            if (!File.Exists(strHmlFilePath))
            {
                int nIndex = strFilePath.LastIndexOf(@"\");
                string filePath = strFilePath.Substring(0, nIndex);
                foreach (string item in System.IO.Directory.GetFiles(filePath))
                {
                    if (item.Contains(strFilePath))
                    {
                        if (item == strFilePath + "_1.hml")
                        {
                            strHmlFilePath = item;
                            break;
                        }
                    }
                }
            }

            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.Arguments = strHmlFilePath;
            info.FileName = m_strHWPPath;

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo = info;

            process.Start();
        }

        // 저장할 한글 파일의 경로
        private string GetHWPFilePath(string strDocType, bool isHwpSetup)
        {
            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("__{0}{1:00}{2:00}_{3:00}{4:00}{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            try
            {
                string strFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (strFolderPath != null && strFolderPath.Length > 0)
                {
                    if (!System.IO.Directory.Exists(strFolderPath + "\\리포트"))
                        System.IO.Directory.CreateDirectory(strFolderPath + "\\리포트");

                    //return strFolderPath + "\\리포트\\" + strDocType + strTime + ".hwp";

                    if (!isHwpSetup)
                    {
                        string temp = strTime.Replace("__", "");
                        return strFolderPath + "\\리포트\\" + temp + "\\" + strDocType + strTime;
                    }

                    return strFolderPath + "\\리포트\\" + strDocType + strTime;
                }
            }
            catch (Exception)
            {
            }

            //SaveFileDialog dlg = new SaveFileDialog();

            //dlg.Filter = "한글 문서 (*.hwp)|*.hwp";

            //dlg.FileName = strDocType + "_" + strTime;

            //if (dlg.ShowDialog() == DialogResult.OK)
            //{
            //    string strSavePath = dlg.FileName;
            //    strSavePath = subGap(strSavePath);
            //    return strSavePath;
            //}

            return null;
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
            m_NotOperationPage.ComboTxtDate(szStartDate, szEndDate);
        }

        public void SelectActionPage(int nSensorZoneHistoryID)
        {
            m_ActionPage.SelectHistory(nSensorZoneHistoryID);
        }

        public void SelectPSMActionPage(int nSensorZoneHistoryID)
        {
            m_ActionPSMPage.SelectHistory(nSensorZoneHistoryID);
        }

        public void SelectActionIntrusionPage(int nSensorZoneHistoryID)
        {
            m_ActionIntrusionPage.SelectHistory(nSensorZoneHistoryID);
        }

        private string GetReportLogoFileName()
        { 
            string strSQL = "Select PropertyValue from OptionSdms where PropertyName='LogoFileName' and SiteID=" + UnE.SOP.ProxySOP.Instance.SiteID;
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0) return string.Empty;

            string logoName = DBUtility.WebDBManager.GetStringField(arrResult[0].ToString(), string.Empty);
             
            return logoName;
        }
    }
}
