using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DBUtility;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using SDMS.Report;
using UnE.Spatial;
using UnE.Sensor;

namespace SDMS
{
    public partial class ActionIntrusionPage : Form
    {
        private enum RefreshType { NO_REFRESH = 0, REFRESH_ALL, REFRESH_REACTION_TYPE };

        private Report.ReactionIntrusionManager m_ActionMgr = null;

        private string m_strEquipZoneName = "";

        private bool iscomboChange = false;
        private string strDate = "";

        private int SensorZoneHistoryID = -1;
        
        private string strManagerName;
        private string strPhoneNumber = "";

        private HwpCtrlData m_hwpCtrl = null;

        internal HwpCtrlData HwpCtrl
        {
            get { return m_hwpCtrl; }
            set { m_hwpCtrl = value; }
        }

        public System.Windows.Forms.DataGridView DataGridView1
        {
            get { return m_dataGridView; }
            set { m_dataGridView = value; }
        }

        private int storage = 0;
        private ArrayList SaveArr = new ArrayList();

        //현재 선택된 날짜(선택된 기간이 바뀌었는지 아닌지 알기 위한..)
        private DateTime m_SelectedMinDate = new DateTime();
        private DateTime m_SelectedMaxDate = new DateTime();
        private ArrayList m_arrSelectedZone = null;
        private string m_strSelectedBuildingGroup = "";
        private string m_strSelectedBuilding = "";
        private string m_strSelectedFloor = "";
        private int m_nSelectedReactionType = 0;
        private int m_nReadLastSensorReactionHistoryID = 0;

        private List<Report.ReactionIntrusionLog> m_allLogs = new List<Report.ReactionIntrusionLog>();

        //0 = 전체, 21 = 오작동, 22 = 화재신고, 23 = 무시된 데이터
        private int nReactionType = 0;
        public int ReactionType
        {
            get { return nReactionType; }
            set { nReactionType = value; }
        }

        public void SetlabelDate()
        {
            lblDefault.Visible = true;
            lblMinDate.Text = "데이터 없음";
            lblSelectDate.Text = "";
            label3.Visible = false;
            lblEquipZone.Text = "";
            label2.Text = "";
            label6.Text = "";
            label10.Text = "";
        }

        public ActionIntrusionPage(Report.ReactionIntrusionManager reactionMgr)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            FormMain.SetDoubleBuffer(m_dataGridView, true);

            m_hwpCtrl = new HwpCtrlData();

            m_ActionMgr = reactionMgr;

			lblSelectDate.Text = "최근 일주일 동안 가장 최근에 발생한";
            //label3.SetBounds(lblSelectDate.Right + 3, label3.Location.Y, label3.Size.Width, label3.Size.Height);

            lblEquipZone.SetBounds(lblSelectDate.Location.X + lblSelectDate.Width + 5, lblSelectDate.Location.Y, lblEquipZone.Width, lblEquipZone.Height);
            lblEquipZone.Location = new Point(lblEquipZone.Location.X - 10, lblEquipZone.Location.Y);
            label10.SetBounds(lblEquipZone.Location.X + lblEquipZone.Width - 10, lblEquipZone.Location.Y, label3.Width, label3.Height);
            label3.SetBounds(label10.Location.X + label10.Width, label10.Location.Y, label3.Width, label3.Height);

            //SetupDataGrid();

            label8.Visible = false;
        }


        private Button react_btnStartDate;
        private Button react_btnEndDate;
        private ComboBox react_cboStartTime;
        private ComboBox react_cboEndTime;
        private ComboBox cboActionIntrusionSelect;      
        private ComboBox react_cboSearchType;
        private Button react_btnSearch;

        private void InitLoadData()
        {
            ArrayList arrSelectZoneList = new ArrayList();

            arrSelectZoneList = ZoneManager.Instance.FindZoneList("모든 건물 그룹", "모든 건물", "모든 층");

            //Report.ReactionManager.Instance.ZoneSubmit(arrSelectZoneList, strStartDate, strEndDate);

            //최근1주일
            DateTime startDate = DateTime.Now.AddDays(-7);
            DateTime EndDate = DateTime.Now;

            //m_SelectedMinDate = startDate;
            //m_SelectedMaxDate = EndDate;
            //m_arrSelectedZone = arrSelectZoneList;

            //설정한 기간, ZoneList를 ZoneSubmit함수에 넘겨줌
            //m_ActionMgr.ZoneSubmit(arrSelectZoneList, startDate, EndDate,2);
            SetupDataGrid();

            //찾은 검색결과를 DataGrid로 출력
            //Load_DataGrid();
        }

        private void ActionIntrusionPage_Load(object sender, EventArgs e)
		{
            ArrayList arContorls = FormMain.Instance.GetActionIntrusionContorl();
            react_btnEndDate = (Button)arContorls[0];
            react_btnStartDate = (Button)arContorls[1];
            react_cboSearchType = (ComboBox)arContorls[2];
            cboActionIntrusionSelect = (ComboBox)arContorls[3];
            react_cboEndTime = (ComboBox)arContorls[4];
            react_cboStartTime = (ComboBox)arContorls[5];
            react_btnSearch = (Button)arContorls[6];

            cboActionIntrusionSelect.DataSource = reactionLogBindingSource; 
            InitLoadData();

            //이벤트처리
            //설정한 기간을 HistorySubmit함수에 넘겨줌
            //HistorySubmit를 통해 화재탐지나 수동신고 된 ReactionLog를 HistoryID로 찾아와서 Combobox에 넣음
            
            //Function_cboActionIntrusionSelect();

            this.react_btnSearch.Click += react_btnSearch_Click;
            //this.react_cboSearchType.SelectionChangeCommitted += new System.EventHandler(this.react_cboSearchType_SelectionChangeCommitted);

            //이벤트처리
            //사용자가 선택 한 Combobox의 HistoryID에 해당하는 전체 ReactionHistory를 가져옴
            this.cboActionIntrusionSelect.SelectionChangeCommitted += new System.EventHandler(this.cboActionIntrusionSelect_SelectionChangeCommitted);

            this.react_btnStartDate.TextChanged += new System.EventHandler(this.react_btnStartDate_TextChanged);
            this.react_btnEndDate.TextChanged += new System.EventHandler(this.react_btnEndDate_TextChanged);
            this.react_cboStartTime.SelectedIndexChanged += new System.EventHandler(this.react_cboStartTime_SelectedIndexChanged);
            this.react_cboEndTime.SelectedIndexChanged += new System.EventHandler(this.react_cboEndTime_SelectedIndexChanged);


            //react_btnStartDate_TextChanged(null, null);

            //if (react_cboStartTime.Items.Count > 0)
            //    react_cboStartTime.SelectedIndex = 0;

            //if (react_cboEndTime.Items.Count > 0)
            //    react_cboEndTime.SelectedIndex = 23;

            if (react_cboSearchType.Items.Count > 0)
                react_cboSearchType.SelectedIndex = 2;
            //react_btnSearch.PerformClick();

            if(cboActionIntrusionSelect.Items.Count > 0)
                cboActionIntrusionSelect.SelectedIndex = cboActionIntrusionSelect.Items.Count - 1;
            //cboActionIntrusionSelect_SelectionChangeCommitted(null, null);


            //최근6개월
            /*DateTime startDate = DateTime.Now.AddMonths(-6);
            DateTime EndDate = DateTime.Now;
            EndDate = EndDate.AddDays(1);

            m_SelectedMinDate = startDate;
            m_SelectedMaxDate = EndDate;
            m_arrSelectedZone = ZoneManager.Instance.FindZoneList("모든 건물 그룹", "모든 건물", "모든 층");*/
		}

        public void SetHwpData()
        {
            System.IO.StreamWriter stream = null;
            try
            {
                stream = new System.IO.StreamWriter(Application.StartupPath + "\\report\\SaveDateTime.txt");
                //stream.WriteLine(strManagerName);
                //stream.WriteLine(label6.Text);
                stream.WriteLine("");
                stream.WriteLine(lblMinDate.Text);
                stream.WriteLine(label8.Text);
                stream.WriteLine(m_strEquipZoneName);
                //stream.WriteLine(label6.Text);
                stream.WriteLine("");
                stream.Close();
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }

            try
            {
                stream = new System.IO.StreamWriter(Application.StartupPath + "\\report\\SaveMemo.txt");
                stream.WriteLine(textBoxMemo.Text);
                stream.Close();
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }

        public void comboChange(bool cboChange)
        {
            iscomboChange = cboChange;
        }

        public void ComboTxtDate(string strdate, string strdateTime)
        {
            strDate = strdate;
            lblMinDate.Text = strdateTime;
        }

        private void SetupDataGrid()
        {
            this.Controls.Add(m_dataGridView);

            //dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dataGridView.ColumnCount = 6;

            m_dataGridView.Columns[0].Name = "No";
            m_dataGridView.Columns[0].Width = 80;
            m_dataGridView.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            m_dataGridView.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
           
            m_dataGridView.Columns[1].Name = "날짜";
            m_dataGridView.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dataGridView.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

            m_dataGridView.Columns[2].Name = "담당자";
            m_dataGridView.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dataGridView.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;

            m_dataGridView.Columns[3].Name = "분류";
            m_dataGridView.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            m_dataGridView.Columns[3].DefaultCellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            m_dataGridView.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            m_dataGridView.Columns[4].Name = "건물";
            m_dataGridView.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            m_dataGridView.Columns[4].DefaultCellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            m_dataGridView.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            m_dataGridView.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;

            m_dataGridView.Columns[5].Name = "층";
            m_dataGridView.Columns[5].Width = 70;
            m_dataGridView.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dataGridView.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;

            m_dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Font font = m_dataGridView.Font;
            m_dataGridView.Font = new Font("맑은 고딕", 12.0f);
            //m_dataGridView.ColumnHeadersDefaultCellStyle.Font = font;
        }

        public void SetLabelString(string lblBuilding)
        {
            label8.Text = lblBuilding;
        }

        public void Function_DataGrid(int ZoneHistoryID, int nReactionType)
        {
            label3.Visible = true;
            SaveArr.Clear();
            m_dataGridView.Rows.Clear();

            SensorZoneHistoryID = ZoneHistoryID;

            ArrayList arrSensorReactionHistory = new ArrayList();

            int nHwpTable = 8;
            int k = 0;

            int count = 0; 
            int nCount = 1;

            //사용자가 선택 한 Combobox의 HistoryID에 해당하는 전체 ReactionHistory를 가져옴
            arrSensorReactionHistory = m_ActionMgr.GetReactionLog(ZoneHistoryID);

            string strMemo = "";

            foreach (Report.ReactionIntrusionLog data in arrSensorReactionHistory)
            {
                //찾은 검색결과를 DataGrid로 출력
                if (!SetGridRows(data, count, nCount, GetReactionString(nReactionType)))
                    continue;

                if (strMemo.Length == 0)
                {
                    strMemo = GetMemo(data);
                }

				int HwpIndex = 0;
				HwpDataSet(nHwpTable, k, count, ref HwpIndex);

				nHwpTable += 6;
				count++;
				nCount++;       
            }
            
			if (iscomboChange == true)
            {
                //Rdo_NotProcess.Select();
                iscomboChange = false;
            }

            //조회기간
            //lblMinDate.Text = strDate;
            lblSelectDate.Text = strDate + "에";
            lblEquipZone.SetBounds(lblSelectDate.Location.X + lblSelectDate.Width + 5, lblSelectDate.Location.Y, lblEquipZone.Width, lblEquipZone.Height);
            lblEquipZone.Location = new Point(lblEquipZone.Location.X - 10, lblEquipZone.Location.Y);
            label10.SetBounds(lblEquipZone.Location.X + lblEquipZone.Width - 10, lblEquipZone.Location.Y, label3.Width, label3.Height);
            label3.SetBounds(label10.Location.X + label10.Width, label10.Location.Y, label3.Width, label3.Height);

            label6.SetBounds(label2.Location.X + label2.Width, label2.Location.Y, label6.Width, label6.Height);



            //원래있던 표의 줄 수를 저장함
            storage = m_dataGridView.Rows.Count;

            textBoxMemo.Text = strMemo;
        }

        private string GetMemo(Report.ReactionIntrusionLog log)
        {
            foreach (SensorReactionIntrusionLog log2 in log.ArrLogList)
            {
                if (log2.Memo.Length > 0)
                    return log2.Memo;
            }

            return "";
        }

        private void HwpDataSet(int nHwpTable, int k , int count, ref int HwpIndex)
        {
            for (k = nHwpTable; k < nHwpTable + 4; k++)
            {
                //데이터 수에 맞춰서 줄 늘림

                SaveArr.Add(m_dataGridView.Rows[count].Cells[HwpIndex].Value.ToString());

                HwpIndex++;
            }
        }

        public void FileWriter()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\report\\SaveData.txt"))
            {
                foreach (string line in SaveArr)
                {
                    {
                        file.WriteLine(line);
                    }
                }
                file.Close();
            }
        }

        private void ActionIntrusionPage_Resize(object sender, EventArgs e)
        {
            Point PtGrid = m_dataGridView.Location;
            //groupBox1.SetBounds(dataGridView1.Right - groupBox1.Width, groupBox1.Location.Y, groupBox1.Size.Width, groupBox1.Size.Height);

            Point ptGrid = m_dataGridView.Location;
            Size SizeGrid = m_dataGridView.Size;

            lblEquipZone.SetBounds(lblSelectDate.Location.X + lblSelectDate.Width + 5, lblSelectDate.Location.Y, lblEquipZone.Width, lblEquipZone.Height);
            label10.SetBounds(lblEquipZone.Location.X + lblEquipZone.Width + 5, lblEquipZone.Location.Y, label3.Width, label3.Height);
            label3.SetBounds(label10.Location.X + label10.Width + 5, label10.Location.Y, label3.Width, label3.Height);

            label6.SetBounds(label2.Location.X + label2.Width, label2.Location.Y, label6.Width, label6.Height);
        }


        private bool SetGridRows(Report.ReactionIntrusionLog data, int nRow, int nCount, string strReactionType)
		{
			if (data == null)
				return false;
			if (nRow < 0)
				return false;

			Zone zone = data.Zone;
			if (zone == null)
				return false;

			DateTime dtDate = data.Time;
			int nType = data.SensorType;
			int ReactionType = data.Type;

			Building buildingFind = zone.Building;
			string strBuildingName = buildingFind == null ? "" : buildingFind.BuildingName;
			string strFloorIndex = zone.Floor == null ? "" : zone.Floor.ToString();
			string strType = GetReactionString(ReactionType);

            SensorReactionIntrusionLog log = (SensorReactionIntrusionLog)data.ArrLogList[0];
            if (ReactionType == (int)SDMS.ReactionType.BEGIN_S1SVMS_STATUS || ReactionType == (int)SDMS.ReactionType.BEGIN_S1ACCESS_STATUS)
            {    
                string detectStr = SOPServer.EventTypeString.GetEventTypeDetectString(Convert.ToInt32(log.Param3));
                if (detectStr.Length > 0)
                {
                    //EMPoll
                    if (Convert.ToInt32(log.Param3) == (int)IFacility.FacilityType.ExternalAlarmBell) 
                        strType = detectStr; 
                    else
                    {
                        if (ReactionType == (int)SDMS.ReactionType.BEGIN_S1SVMS_STATUS)
                            strType = "S1SVMS " + detectStr + " 탐지";
                        else if (ReactionType == (int)SDMS.ReactionType.BEGIN_S1ACCESS_STATUS)
                            strType = "S1ACCESS " + detectStr + " 탐지";
                    }
                }
            }

            if (ReactionType == (int)SDMS.ReactionType.IGNORE_S1ACCESS_STATUS && Convert.ToInt32(log.Param3) == (int)IFacility.FacilityType.ExternalAlarmBell) 
                strType = "외부비상벨 꺼짐";  
            if (ReactionType == (int)SDMS.ReactionType.END_S1ACCESS_STATUS && Convert.ToInt32(log.Param3) == (int)IFacility.FacilityType.ExternalAlarmBell) 
                strType = "외부비상벨 종료";  

            //FacilityManagerGroup ManagerGroup = null;
            
            string strUserName = data.UserName;
            label6.Text = strUserName;

            //수동이면
            if (data.SensorType == 0)
            {
                if (data.Zone != null)
                {
                    lblEquipZone.Text = "【 " + data.Zone.ZoneName + " 】";
                    m_strEquipZoneName = data.Zone.ZoneName;
                    //lblEquipZone.Text = "";
                    //m_strEquipZoneName = "-";
                    label10.Text = "에서 발생한 수동신고에";
                }

            }
            else if (data.SensorType == 1)//자탐이면
            {
                if (data.equipZone != null)
                {
                    lblEquipZone.Text = "【 " + data.equipZone.ZoneName + " 】";
                    m_strEquipZoneName = data.equipZone.ZoneName;
                    label10.Text = "에서 발생한 방범 감지신호에";
                }
            }
            else
            {
                label10.Text = "";
            }

            if (strReactionType == "S1ACCESS 꺼짐" && Convert.ToInt32(log.Param3) == (int)IFacility.FacilityType.ExternalAlarmBell) label2.Text = "외부비상벨 꺼짐";
            else if (strReactionType == "S1ACCESS 종료" && Convert.ToInt32(log.Param3) == (int)IFacility.FacilityType.ExternalAlarmBell) label2.Text = "외부비상벨 종료"; 
            else label2.Text = strReactionType;

            if (strReactionType != "S1SVMS 꺼짐" && strReactionType != "S1ACCESS 꺼짐" && strReactionType != "외부비상벨 꺼짐" && strReactionType != "방범 탐지")
                label2.Text += " - ";

  
            if (buildingFind == null)
            {
                strBuildingName = zone.ZoneName;
            }

            EquipmentZone equipZone = null;


            //ArrayList arEquipzone = ZoneManager.Instance.GetEquipmentZoneList(zone);
            //if (arEquipzone != null && arEquipzone.Count > 0)
            //{
            //    equipZone = (EquipmentZone)arEquipzone[0];
            //}

            //if (equipZone != null)
            //{
            //    ManagerGroup = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(IFacility.FacilityType.FIRE_SENSOR, equipZone);
            //}

            //if (ManagerGroup == null)
            //    ManagerGroup = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(IFacility.FacilityType.FIRE_SENSOR, buildingFind);

            //if (ManagerGroup == null)
            //    ManagerGroup = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(IFacility.FacilityType.FIRE_SENSOR);

            //strManagerName = FormMain.Instance.DataManager.GetFacilityManagerName(ManagerGroup, ref strPhoneNumber);

            //strManagerName = data.UserName;
            strManagerName = data.ManagerName;

            if (strType.Trim().Length == 0)
                return false;

            // 같은 로그가 이미 기록되어 있는지 확인한다.
            if (ContainsType(strType, dtDate))
                return false;

			string[] rows = { "", "", strManagerName, strType, strBuildingName, strFloorIndex };
			m_dataGridView.Rows.Add(rows);
			m_dataGridView.Rows[nRow].Cells[0].Value = nCount;
			m_dataGridView.Rows[nRow].Cells[1].Value = dtDate;

            return true;
		}

        // strType, dtDate에 해당하는 값이 이미 존재하는지 검사한다.
        private bool ContainsType(string strType, DateTime dtDate)
        {
            string strTime = dtDate.ToString();

            foreach (DataGridViewRow row in m_dataGridView.Rows)
            {
                if (row.Cells[1].Value != null && row.Cells[1].Value.ToString() == strTime)
                {
                    if (row.Cells[3].Value != null && row.Cells[3].Value.ToString() == strType)
                        return true;
                }
            }

            return false;
        }

        public string GetReactionString(int nReactionType)
        {
            string strType = "";

            switch (nReactionType)
            {
                //case 0: strType = "상황 시작";
                case (int)SDMS.ReactionType.BEGIN_STATUS: strType = "방범 탐지";
                    break;
                case (int)SDMS.ReactionType.RUN_DETECT_BROADCAST: strType = "사내 방송 실시(탐지)";
                    break;
                case (int)SDMS.ReactionType.RUN_REPORT_BROADCAST: strType = "사내 방송 실시(신고)";
                    break;
                case (int)SDMS.ReactionType.SEND_SMS: strType = "문자메시지 발송";
                    break;
                case (int)SDMS.ReactionType.SEND_DETECT_SMS: strType = "문자메시지 발송(탐지)";
                    break;
                case (int)SDMS.ReactionType.SEND_REPORT_SMS: strType = "문자메시지 발송(신고)";
                    break;
                case (int)SDMS.ReactionType.SEND_MALFUNCTION_SMS: strType = "문자메시지 발송(오작동)";
                    break;
                case (int)SDMS.ReactionType.SEND_REPAIR_SMS: strType = "문자메시지 발송(복구)";
                    break;
                case (int)SDMS.ReactionType.MALFUNCTION: strType = "오작동 처리";
                    break;
                case (int)SDMS.ReactionType.NOTIFY_FIRE: strType = "화재 신고";
                    break;
                case (int)SDMS.ReactionType.IGNORE_FIRE: strType = "화재신호 꺼짐";
                    break;
                case (int)SDMS.ReactionType.RUN_SOP: strType = "SOP 발동";
                    break;
                case (int)SDMS.ReactionType.RUN_N_CANCEL_SOP: strType = "SOP 실행후 취소";
                    break;
                case (int)SDMS.ReactionType.FINISH_SOP: strType = "SOP 종료";
                    break;
                case (int)SDMS.ReactionType.IGNORE_SOP: strType = "SOP 실행않고 상황 종료";
                    break;
                case (int)SDMS.ReactionType.END_STATUS: strType = "상황해제";
                    break;
                case (int)SDMS.ReactionType.BEGIN_S1SVMS_STATUS: strType = "S1SVMS 탐지";
                    break;
                case (int)SDMS.ReactionType.IGNORE_S1SVMS_STATUS: strType = "S1SVMS 꺼짐";
                    break;
                case (int)SDMS.ReactionType.END_S1SVMS_STATUS: strType = "S1SVMS 종료";
                    break;
                case (int)SDMS.ReactionType.BEGIN_S1ACCESS_STATUS: strType = "S1ACCESS 탐지";
                    break;
                case (int)SDMS.ReactionType.IGNORE_S1ACCESS_STATUS: strType = "S1ACCESS 꺼짐";
                    break;
                case (int)SDMS.ReactionType.END_S1ACCESS_STATUS: strType = "S1ACCESS 종료";
                    break;
                default:
                    break;
            }

            return strType;
        }

        private void SetItemsClear()
        {
            SetlabelDate();
            DataGridView1.Rows.Clear();
            int nTemp = react_cboSearchType.SelectedIndex;
            react_cboSearchType.SelectedIndex = -1;
            reactionLogBindingSource.Clear();
            //cboActionIntrusionSelect.Items.Clear();

            if(nTemp == 0)
            {
                Function_cboActionIntrusionSelect(898);
            }
            else if(nTemp == 1)
            {
                Function_cboActionIntrusionSelect(21);
            }
            else if(nTemp == 2)
            {
                Function_cboActionIntrusionSelect(919);
                Function_cboActionIntrusionSelect(939);
            }

            react_cboSearchType.SelectedIndex = nTemp;

            cboActionIntrusionSelect.SelectedIndex = cboActionIntrusionSelect.Items.Count - 1;

            cboActionIntrusionSelect_SelectionChangeCommitted(null, null);
        }
        



        private void react_btnStartDate_TextChanged(object sender, EventArgs e)
        {
            if (react_btnStartDate.Text == react_btnEndDate.Text)
            {
                react_cboStartTime.Enabled = true;
                react_cboEndTime.Enabled = true;
            }
            else
            {
                react_cboStartTime.Enabled = false;
                react_cboEndTime.Enabled = false;
            }
            //SetItemsClear();
        }

        private void react_btnEndDate_TextChanged(object sender, EventArgs e)
        {
            if (react_btnStartDate.Text == react_btnEndDate.Text)
            {
                react_cboStartTime.Enabled = true;
                react_cboEndTime.Enabled = true;
            }
            else
            {
                react_cboStartTime.Enabled = false;
                react_cboEndTime.Enabled = false;
            }
            //SetItemsClear();
        }

        private void react_cboStartTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            //SetItemsClear();
        }

        private void react_cboEndTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            //SetItemsClear();
        }

        public void cboActionIntrusionSelect_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetlabelDate();

            //선택한 Combobox를 ReactionLog클래스 형태로 가져옴
            Report.ReactionIntrusionLog data = (Report.ReactionIntrusionLog)cboActionIntrusionSelect.SelectedItem;

            if (data == null)
                return;


            lblDefault.Visible = false;

            string strTime = String.Format("{0}년 {1}월 {2}일 {3} {4}시 {5}분", data.Time.Year, data.Time.Month, data.Time.Day, (data.Time.Hour < 12 ? "오전" : "오후"), data.Time.Hour > 12 ? data.Time.Hour - 12 : data.Time.Hour, data.Time.Minute);

            //Label에 표시할 내용(기간)
            if (react_cboSearchType.SelectedIndex == -1)
            {
                ComboTxtDate("최근 일주일 동안 가장 최근", strTime);
            }
            else
            {
                ComboTxtDate(data.Time.ToString(), strTime);
            }


            if (data != null)
            {
                //사용자가 선택 한 Combobox의 HistoryID에 해당하는 전체 ReactionHistory를 가져옴
                Function_DataGrid(data.HistoryID, data.Type);
            }
        }

        public void react_btnSearch_Click(object sender, EventArgs e)
        {
            if (react_cboSearchType.SelectedIndex == 0)
            {
                nReactionType = 898;
                Function_cboActionIntrusionSelect(nReactionType);
            }
            else if (react_cboSearchType.SelectedIndex == 1)
            {
                nReactionType = 21;
                Function_cboActionIntrusionSelect(nReactionType);
            }
            else if (react_cboSearchType.SelectedIndex == 2)
            {
                nReactionType = 919;
                Function_cboActionIntrusionSelect(nReactionType);
            }

            cboActionIntrusionSelect.SelectedIndex = cboActionIntrusionSelect.Items.Count - 1;
            cboActionIntrusionSelect_SelectionChangeCommitted(null, null);
        }

        public void DetectIntrusionPageDoubleClick(int nSensorZoneHistoryID = 0)
        {
            if (react_cboSearchType.SelectedIndex == 0)
            {
                nReactionType = 898;
                Function_cboActionIntrusionSelect(nReactionType);
            }
            else if (react_cboSearchType.SelectedIndex == 1)
            {
                nReactionType = 21;
                Function_cboActionIntrusionSelect(nReactionType);
            }
            else if (react_cboSearchType.SelectedIndex == 2)
            {
                nReactionType = 919;
                Function_cboActionIntrusionSelect(nReactionType);
            }
        }

        private void react_cboSearchType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (react_cboSearchType.SelectedIndex == 0)
            {
                nReactionType = 898;
                Function_cboActionIntrusionSelect(nReactionType);
            }
            else if (react_cboSearchType.SelectedIndex == 1)
            {
                nReactionType = 21;
                Function_cboActionIntrusionSelect(nReactionType);
            }
            else if (react_cboSearchType.SelectedIndex == 2)
            {
                nReactionType = 919;
                Function_cboActionIntrusionSelect(nReactionType);
            }

            cboActionIntrusionSelect.SelectedIndex = cboActionIntrusionSelect.Items.Count - 1;
            cboActionIntrusionSelect_SelectionChangeCommitted(null, null);

            //label초기화
            //SetlabelDate();
        }

        private int GetMaxSensorReactionHistoryID()
        {
            string strSQL = "Select max(ID) from SensorReactionHistory";
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return -1;

            return id.Data;
        }

        // 이전 검색시간과 동일하면 true를 리턴한다.
        private bool CheckTime(DateTime dtStart, DateTime dtEnd, int nReactionHistoryID)
        {
            if (m_SelectedMinDate == dtStart && m_SelectedMaxDate == dtEnd)
                return true;
            else if (m_SelectedMinDate == dtStart &&
                m_SelectedMinDate.Year == dtEnd.Year && m_SelectedMinDate.Month == dtEnd.Month && m_SelectedMinDate.Day == dtEnd.Day &&
                m_nReadLastSensorReactionHistoryID == nReactionHistoryID)
            {
                // 이전 검색조건과 모두 일치하면서, CurrentTime의 시간만 다를 경우
                // 이렇게 하는 이유는 EndTime이 현재날짜가 아닐수도 있기 때문
                return true;
            }

            m_SelectedMinDate = dtStart;
            m_SelectedMaxDate = dtEnd;
            m_nReadLastSensorReactionHistoryID = nReactionHistoryID;
            return false;
        }

        private RefreshType NeedRefresh(int nReactionType, DateTime dtStart, DateTime dtEnd)
        {
            string strBuildingGroup = "", strBuilding = "", strFloor = "";
            FormMain.Instance.GetFireBuildingInfo(ref strBuildingGroup, ref strBuilding, ref strFloor);

            int nSensorHistoryID = GetMaxSensorReactionHistoryID();

            if (m_strSelectedBuildingGroup == strBuildingGroup && m_strSelectedBuilding == strBuilding && m_strSelectedFloor == strFloor)
            {
                if (m_nSelectedReactionType == nReactionType)
                {
                    if (CheckTime(dtStart, dtEnd, nSensorHistoryID))
                    {
                        m_nReadLastSensorReactionHistoryID = nSensorHistoryID;
                        return RefreshType.NO_REFRESH;
                    }
                }
                else
                {
                    m_nSelectedReactionType = nReactionType;

                    if (CheckTime(dtStart, dtEnd, nSensorHistoryID))
                    {
                        m_nReadLastSensorReactionHistoryID = nSensorHistoryID;
                        return RefreshType.REFRESH_REACTION_TYPE;
                    }
                }
            }
            else
            {
                m_strSelectedBuildingGroup = strBuildingGroup;
                m_strSelectedBuilding = strBuilding;
                m_strSelectedFloor = strFloor;

                m_arrSelectedZone = ZoneManager.Instance.FindZoneList(m_strSelectedBuildingGroup, m_strSelectedBuilding, m_strSelectedFloor);
            }

            m_SelectedMinDate = dtStart;
            m_SelectedMaxDate = dtEnd;
            m_nSelectedReactionType = nReactionType;
            m_nReadLastSensorReactionHistoryID = nSensorHistoryID;
            return RefreshType.REFRESH_ALL;
        }

        private void RefreshReactionType(int nReactionType)
        {
            m_nSelectedReactionType = nReactionType;

            cboActionIntrusionSelect.DataSource = null;
            reactionLogBindingSource.Clear();

            foreach (Report.ReactionIntrusionLog log in m_allLogs)
            {
                //화재신고만   
                if (nReactionType == 898)
                {
                    if (log.Type == 899 || log.Type == 898 || log.Type == 921)
                        reactionLogBindingSource.Add(log);
                }
                else if (nReactionType == 21) //오작동신고 포함
                {
                    if (log.Type == 899 || log.Type == 898 || log.Type == 21 || log.Type == 921)
                        reactionLogBindingSource.Add(log);
                }
                else if (nReactionType == 919 || nReactionType == 939) //무시된 데이터 포함
                {
                    if (log.Type == 899 || log.Type == 898 || log.Type == 21 || log.Type == 919 || log.Type == 939 || log.Type == 921)
                        reactionLogBindingSource.Add(log);
                }
            }

            cboActionIntrusionSelect.DataSource = reactionLogBindingSource;
            this.m_dataGridView.Rows.Clear();
        }

        public void Function_cboActionIntrusionSelect(int nReactionType = 919)
        {
            DateTime dtStart = DateTime.ParseExact(react_btnStartDate.Text, "yyyy-MM-dd", null);
            DateTime dtEnd = DateTime.ParseExact(react_btnEndDate.Text, "yyyy-MM-dd", null);

            RefreshType refreshType = NeedRefresh(nReactionType, dtStart, dtEnd);

            if (refreshType == RefreshType.NO_REFRESH)
                return;
            else if (refreshType == RefreshType.REFRESH_REACTION_TYPE)
            {
                RefreshReactionType(nReactionType);
                return;
            }

            m_nSelectedReactionType = nReactionType;

            if (cboActionIntrusionSelect != null)
            {
                reactionLogBindingSource.Clear();
                //cboActionIntrusionSelect.Items.Clear();
            }

            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            int start_Hour = 0;
            int End_Hour = 0;

            //시간이 비활성화일 때
            if (react_cboStartTime.Enabled == false && react_cboEndTime.Enabled == false)
            {
                start_Hour = 0;
                End_Hour = 24;
            }
            else
            {
                for (int i = 0; i < 25; i++)
                {
                    if (react_cboStartTime.Text == i + "시")
                    {
                        start_Hour = i;
                    }
                    if (react_cboEndTime.Text == i + "시")
                    {
                        End_Hour = i;
                    }
                }
            }

            dtStart = dtStart.AddHours(start_Hour);
            //dtStart = dtStart.AddMinutes(59);
            //dtStart = dtStart.AddSeconds(59);
            dtEnd = dtEnd.AddHours(End_Hour - 1);
            dtEnd = dtEnd.AddMinutes(59);
            dtEnd = dtEnd.AddSeconds(59);

            cboActionIntrusionSelect.DataSource = null;
            reactionLogBindingSource.Clear();
            m_allLogs.Clear();
            //cboActionIntrusionSelect.Items.Clear();

            m_ActionMgr.DataClear();
            //설정한 기간, ZoneList를 ZoneSubmit함수에 넘겨줌
            m_ActionMgr.ZoneSubmit(m_arrSelectedZone, dtStart, dtEnd, 2);

            //HistorySubmit를 통해 화재탐지나 수동신고 된 ReactionLog를 HistoryID로 찾아와서 Combobox에 넣음
            ArrayList arrComboData = m_ActionMgr.HistorySubmit(dtStart, dtEnd);

            //cboActionIntrusionSelect.Items.Clear();
            foreach (Report.ReactionIntrusionLog log in arrComboData)
            {
                //방범신고만   
                if (nReactionType == 898)
                {
                    if (log.Type == 0 || log.Type == 899 || log.Type == 898 || log.Type == 921)
                        reactionLogBindingSource.Add(log);
                        //cboActionIntrusionSelect.Items.Add(log);
                }
                else if (nReactionType == 21) //오작동신고 포함
                {
                    if (log.Type == 0 || log.Type == 899 || log.Type == 898 || log.Type == 21 || log.Type == 921)
                        reactionLogBindingSource.Add(log);
                        //cboActionIntrusionSelect.Items.Add(log);
                }
                else if (nReactionType == 919 || nReactionType == 939) //무시된 데이터 포함
                {
                    if (log.Type == 0 || log.Type == 899 || log.Type == 898 || log.Type == 21 || log.Type == 919 || log.Type == 939 || log.Type == 921)
                        reactionLogBindingSource.Add(log);
                        //cboActionIntrusionSelect.Items.Add(log);
                }

                m_allLogs.Add(log);
            }

            cboActionIntrusionSelect.DataSource = reactionLogBindingSource;
            this.m_dataGridView.Rows.Clear();
        }

        private void btnSaveHWP_Click(object sender, EventArgs e)
        {
            btnSaveHWP.Enabled = false;
            PageBackstageHome.Instance.FrmReport.SaveHWPForAction();
            btnSaveHWP.Enabled = true;
        }

        public void SelectHistory(int nSensorZoneHistoryID)
        {
            foreach (Report.ReactionIntrusionLog log in cboActionIntrusionSelect.Items)
            {
                if (log.HistoryID == nSensorZoneHistoryID)
                {
                    cboActionIntrusionSelect.SelectedItem = log;
                    cboActionIntrusionSelect_SelectionChangeCommitted(null, null);
                    break;
                }
            }
        }

        public void SetVisibleHWPExport(bool visible)
        {
            btnSaveHWP.Visible = visible;
        } 
    }     
}
