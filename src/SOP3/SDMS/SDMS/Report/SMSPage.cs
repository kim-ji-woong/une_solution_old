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

namespace SDMS
{
    public partial class SMSPage : Form
    {
        private SmSHistoryManager m_HistoryManager = new SmSHistoryManager();
        private ArrayList m_arrSelectedZone = new ArrayList();
        private DateTime m_SelectedMinDate;
        private DateTime m_SelectedMaxDate;

        public SMSPage()
        {
            InitializeComponent();
        }

        private void SMSPage_Load(object sender, EventArgs e)
        {
            InitLoadData();
            //화재탐지페이지가 처음 로드될 때 이벤트 한 번 실행
            FormMain.Instance.proc_cboLatelyDate_SelectedIndexChanged(sender, e);
        }

        private void InitLoadData()
        {
            ArrayList arSelectZoneList = ZoneManager.Instance.FindZoneList("모든 건물 그룹", "모든 건물", "모든 층");

            //최근6개월
            DateTime dtStart = DateTime.Now.AddMonths(-6);
            DateTime dtEnd = DateTime.Now;

            //설정한 기간, ZoneList를 ZoneSubmit함수에 넘겨줌
            ZoneSubmit(arSelectZoneList, dtStart, dtEnd);

            SetupDataGrid();

            //찾은 검색결과를 DataGrid로 출력
            LoadDataGrid();            
        }

        public void ZoneSubmit(ArrayList arZoneList, DateTime dtStart, DateTime dtEnd)
        {
            m_arrSelectedZone.Clear();
            foreach(Zone zone in arZoneList)
            {
                ArrayList arEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);
                if (arEquipZones!= null)
                    m_arrSelectedZone.AddRange(arEquipZones);
            }
            //m_arrSelectedZone = arZoneList;

            m_SelectedMinDate = dtStart;
            m_SelectedMaxDate = dtEnd;

            lblMaxDate.Text = m_SelectedMaxDate.ToShortDateString();
            lblMinDate.Text = m_SelectedMinDate.ToShortDateString();

            m_HistoryManager.ZoneSubmit(m_arrSelectedZone,arZoneList, dtStart, dtEnd);

            LoadDataGrid();
        }

        private void SetupDataGrid()
        {
        }

        public void LoadDataGrid()
        {
            m_dataGridView.ClearSelection();
            m_dataGridView.Rows.Clear();

            ArrayList arDatas = m_HistoryManager.HistoryData;
            if (arDatas == null)
                return;

            int nCount = 1;
            foreach(SmsHistory data in arDatas)
            {
                DataGridViewRow row = new DataGridViewRow();
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
                nCount++;
            }
        }

        private void m_dataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {           
        }

       
        private void ShowDetailSendList(object sender , EventArgs e)
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
    }
}
