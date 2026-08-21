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
using UnE.Spatial;
using UnE.Sensor;
using SDMS.Report;
using SDMS.Help;

namespace SDMS
{
    public partial class SMSIntrusionPage : FormReportBase
    {
        private SmSHistoryManager m_HistoryManager = new SmSHistoryManager();
        private ArrayList m_arrSelectedZone = new ArrayList();
        private DateTime m_SelectedMinDate;
        private DateTime m_SelectedMaxDate;

        private Report.ReactionIntrusionManager.RefreshCheckData m_checkData = new Report.ReactionIntrusionManager.RefreshCheckData();

        public Report.ReactionIntrusionManager.RefreshCheckData RefreshCheckData
        {
            get { return m_checkData; }
        }

        private ManualManager m_manualManager = null;

        public SMSIntrusionPage()
        { 
            InitializeComponent(); 

            this.DoubleBuffered = true;
            FormMain.SetDoubleBuffer(m_dataGridView, true);

            InitCtrlSize(this);

            FormMain.Instance.CustomizeGridView(m_dataGridView);
            this.Resize += SMSIntrusionPage_Resize;

            m_manualManager = new ManualManager(this);
            SetManualID();
        }

        void SMSIntrusionPage_Resize(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("HHmmss") + " / SMSIntrusionPage1");

            SetChildCtrlResize(this, 0, 0);
            SetupDataGrid();

            lblMinDate.Location = new Point(label5.Location.X + label5.Width, lblMinDate.Location.Y);
            lblMaxDate.Location = new Point(lblMinDate.Location.X + lblMinDate.Width, lblMinDate.Location.Y);
            label4.Location = new Point(lblMaxDate.Location.X + lblMaxDate.Width + 5, lblMinDate.Location.Y);
            label6.Location = new Point(label4.Location.X + label4.Width, lblMinDate.Location.Y);
            lblBuilding.Location = new Point(label6.Location.X + label6.Width, lblMinDate.Location.Y);

            System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("HHmmss") + " / SMSIntrusionPage2");
        }

        private void SMSIntrusionPage_Load(object sender, EventArgs e)
        {
            //SetupDataGrid();
            InitLoadData();
            //화재탐지페이지가 처음 로드될 때 이벤트 한 번 실행
            //FormMain.Instance.proc_cboLatelyDate_SelectedIndexChanged(sender, e);
        }

        private void InitLoadData()
        {
            ArrayList arSelectZoneList = ZoneManager.Instance.FindZoneList("모든 건물 그룹", "모든 건물", "모든 층");

            //최근6개월
            DateTime dtStart = DateTime.Now.AddDays(-6);
            DateTime dtEnd = DateTime.Now;

            //설정한 기간, ZoneList를 ZoneSubmit함수에 넘겨줌
            //ZoneSubmit(arSelectZoneList, dtStart, dtEnd);

            //찾은 검색결과를 DataGrid로 출력
            //LoadDataGrid();
        }

        public void ZoneSubmit(ArrayList arZoneList, DateTime dtStart, DateTime dtEnd)
        {
            m_arrSelectedZone.Clear();
            foreach (Zone zone in arZoneList)
            {
                List<EquipmentZone> arEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);
                if (arEquipZones != null)
                    m_arrSelectedZone.AddRange(arEquipZones);
            }
            //m_arrSelectedZone = arZoneList;

            m_SelectedMinDate = dtStart;
            m_SelectedMaxDate = dtEnd;

            lblMaxDate.Text = m_SelectedMaxDate.ToString("yyyy년 M월 d일 까지");
            lblMinDate.Text = m_SelectedMinDate.ToString("yyyy년 M월 d일 부터");

            m_HistoryManager.ZoneSubmit(m_arrSelectedZone, arZoneList, dtStart, dtEnd, UnE.Sensor.IFacility.FacilityType.Security_Sensor);

            LoadDataGrid();

            lblMinDate.Location = new Point(label5.Location.X + label5.Width, lblMinDate.Location.Y);
            lblMaxDate.Location = new Point(lblMinDate.Location.X + lblMinDate.Width, lblMinDate.Location.Y);
            label4.Location = new Point(lblMaxDate.Location.X + lblMaxDate.Width + 5, lblMinDate.Location.Y);
            label6.Location = new Point(label4.Location.X + label4.Width, lblMinDate.Location.Y);
            lblBuilding.Location = new Point(label6.Location.X + label6.Width, lblMinDate.Location.Y);
        }

        private void SetupDataGrid()
        {
            /*m_dataGridView.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            m_dataGridView.Columns[2].DefaultCellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            m_dataGridView.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            m_dataGridView.Columns[4].DefaultCellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);*/

            float sizePer = 1f;
            if (FormMain.Instance.Resolution == Resolution.FullHD)
                sizePer = 0.5f;
            else if (FormMain.Instance.Resolution == Resolution.Other)
                sizePer = 0.75f;

            Font font = m_dataGridView.Font;
            m_dataGridView.Font = new Font(Program.prgFont, (int)(24.0f * sizePer));
            //m_dataGridView.ColumnHeadersDefaultCellStyle.Font = font;
        }

        public void LoadDataGrid()
        {
            m_dataGridView.ClearSelection();
            m_dataGridView.DataSource = null;
            sMSPageGridDataBindingSource.Clear();
            //m_dataGridView.Rows.Clear();

            ArrayList arDatas = m_HistoryManager.HistoryData;
            if (arDatas == null)
                return;

            int nCount = 0;
            foreach (SmsHistory data in arDatas)
            {
                Report.SMSPageGridData gridData = new Report.SMSPageGridData();
                gridData.No = ++nCount;
                gridData.TimeStamp = data.Time;
                gridData.SendCount = (data.CompanyMemberList.Count + data.ExteanlMemberList.Count) + "명";

                if (data.EquipZone != null)
                    gridData.Location = data.EquipZone.ZoneName;
                else
                    gridData.Location = "[수동신고] " + data.Zone.ZoneName;

                if (data.IsAuto == true)
                    gridData.SendText = "시스템 전송 : " + data.Message;
                else
                    gridData.SendText = "수동 전송 : " + data.Message;

                sMSPageGridDataBindingSource.Add(gridData);

                /*DataGridViewRow row = new DataGridViewRow();
                row.Tag = data;

                DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                cell1.Value = nCount;
                row.Cells.Add(cell1);

                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = data.Time.ToString();
                row.Cells.Add(cell2);

                string szName = "-";
                if (data.EquipZone != null)
                {
                    szName = data.EquipZone.ZoneName;
                }
                else
                {
                    szName = "[수동신고] " + data.Zone.ZoneName;
                }
                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                cell3.Value = szName;
                row.Cells.Add(cell3);

                DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                cell4.Value = (data.CompanyMemberList.Count + data.ExteanlMemberList.Count) + "명";
                cell4.Tag = (data.CompanyMemberList.Count + data.ExteanlMemberList.Count);
                row.Cells.Add(cell4);

                string szMessage = "";
                if (data.IsAuto == true)
                {
                    szMessage = "시스템 전송 : " + data.Message;
                }
                else
                {
                    szMessage = "수동 전송 : " + data.Message;
                }

                DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                cell5.Value = szMessage;
                row.Cells.Add(cell5);

                m_dataGridView.Rows.Add(row);
                row.Height = m_dataGridView.RowTemplate.Height;
                nCount++;*/
            }

            m_dataGridView.DataSource = sMSPageGridDataBindingSource;

            for (int i=0;i<nCount;i++)
            {
                DataGridViewRow row = m_dataGridView.Rows[i];
                SmsHistory data = (SmsHistory)arDatas[i];

                row.Tag = data;
                row.Cells[3].Tag = data.CompanyMemberList.Count + data.ExteanlMemberList.Count;
                row.Height = m_dataGridView.RowTemplate.Height;
            }
        }


        private void ShowDetailSendList(object sender, EventArgs e)
        {
            SmsHistory data = (SmsHistory)m_PopupMenu.Tag;
            FormSMSHistory form = new FormSMSHistory();
            form.SetData(data);
            PageBackstageHome.ShowTranslucentForm(form, 200, 100, form.Width, form.Size.Height, ID.ID_VIEW_SMS);
        }

        private ContextMenu m_PopupMenu = null;
        private MenuItem m_DetailMenu = null;

        private void m_dataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (m_PopupMenu == null)
                {
                    m_PopupMenu = new ContextMenu();
                    m_DetailMenu = new MenuItem("세부 내역 확인");
                    m_DetailMenu.Click += ShowDetailSendList;
                    m_PopupMenu.MenuItems.Add(m_DetailMenu);
                }

                int currentMouseOverRow = m_dataGridView.HitTest(e.X, e.Y).RowIndex;
                if (currentMouseOverRow >= 0)
                {
                    DataGridViewRow row = m_dataGridView.Rows[currentMouseOverRow];
                    if (row.Selected == false)
                    {
                        m_dataGridView.ClearSelection();
                        row.Selected = true;
                    }

                    SmsHistory data = (SmsHistory)row.Tag;
                    string szText = string.Format("세부 내역 확인 [{0}]", data.IsAuto == true ? "시스템" : "수동");
                    m_DetailMenu.Text = szText;

                    Point pt = e.Location;

                    m_PopupMenu.Tag = data;
                    m_PopupMenu.Show(m_dataGridView, pt);
                }
                else
                {
                    m_PopupMenu.Tag = null;
                }
            }
        }

        private void m_dataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                {
                    DataGridViewRow row = m_dataGridView.Rows[e.RowIndex];

                    if (m_PopupMenu == null)
                    {
                        m_PopupMenu = new ContextMenu();
                        m_DetailMenu = new MenuItem("세부 내역 확인");
                        m_DetailMenu.Click += ShowDetailSendList;
                        m_PopupMenu.MenuItems.Add(m_DetailMenu);
                    }

                    if (row.Tag != null && row.Tag is SmsHistory)
                    {
                        m_PopupMenu.Tag = row.Tag;
                        ShowDetailSendList(null, null);
                    }
                }
            }
        }

        private void CloseReportMenu()
        {
            FormMain.Instance.CloseOtherReportMenu(PopupDialog.Report.ReportCategory.NONE);
        }

        private void this_MouseDown(object sender, MouseEventArgs e)
        {
            CloseReportMenu();
        }

        private void SetManualID()
        {
            m_manualManager.Handle = this.Handle;

            m_manualManager.Clear();

            m_manualManager.SetID(this, "SDMS_Report_SMS_Security");
            m_manualManager.SetID(label2, "SDMS_Report_SMS_Security");
            m_manualManager.SetID(m_dataGridView, "SDMS_Report_SMS_Security"); 

            m_manualManager.ProcessEvent();
        }
    }
}
