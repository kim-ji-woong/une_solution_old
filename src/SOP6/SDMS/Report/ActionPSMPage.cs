using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DBUtility2;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Reflection;
using SDMS.Report;
using UnE.Spatial;
using UnE.Sensor;
using libSensorProcess;
using UnE.GUI;
using SDMS.Help;

namespace SDMS
{
    public partial class ActionPSMPage : FormReportBase
    {
        private enum RefreshType { NO_REFRESH = 0, REFRESH_ALL, REFRESH_REACTION_TYPE };

        private Report.ReactionPSMManager m_ActionMgr = null;

        private string m_strEquipZoneName = "";

        private bool iscomboChange = false;
        private string strDate = "";

        private int SensorZoneHistoryID = -1;

        private string m_strNotifierName = String.Empty;

        private HwpCtrlData m_hwpCtrl = null;
        internal HwpCtrlData HwpCtrl
        {
            get { return m_hwpCtrl; }
            set { m_hwpCtrl = value; }
        }

        public System.Windows.Forms.DataGridView DataGridViewControl
        {
            get { return gvMain; }
            set { gvMain = value; }
        }

        private int storage = 0;
        private ArrayList SaveArr = new ArrayList();

        //현재 선택된 날짜(선택된 기간이 바뀌었는지 아닌지 알기 위한..)
        private DateTime m_SelectedMinDate = new DateTime();
        private DateTime m_SelectedMaxDate = new DateTime();
        private ArrayList m_arrSelectedZone = new ArrayList();
        private string m_strSelectedLocation = "";
        private int m_nSelectedReactionType = 0;
        private int m_nReadLastSensorReactionHistoryID = 0;
        private bool isFormLoaded = false;

        // 전체 PSM Tank들이 위치한 Zone List
        // Key : 위치명
        private Dictionary<string, Zone> m_dicAllTankZones = null;
        private List<Report.ReactionLog> m_allLogs = new List<Report.ReactionLog>();

        //0 = 전체, 21 = 오작동, 63 = 누출신고, 23 = 무시된 데이터
        private int nReactionType = 0;
        public int ActionReactionType
        {
            get { return nReactionType; }
            set { nReactionType = value; }
        }

        public void SetLabelDate()
        {
            lblDefault.Visible = true;
            lblMinDate.Text = "데이터 없음";
            lblSelectDate.Text = "";
            lblStatementEnd.Visible = false;
            lblEquipZone.Text = "";
            lblResult.Text = "";
            lblActorName.Text = "";
            lblAlarmType.Text = "";
        }

        private ManualManager m_manualManager = null;

        public ActionPSMPage(Report.ReactionPSMManager reactionMgr)
        {   
            InitializeComponent();

            this.DoubleBuffered = true;
            FormMain.SetDoubleBuffer(gvMain, true);

            m_hwpCtrl = new HwpCtrlData();

            m_ActionMgr = reactionMgr;

            lblSelectDate.Text = "최근 일주일 동안 가장 최근에 발생한";
            //label3.SetBounds(lblSelectDate.Right + 3, label3.Location.Y, label3.Size.Width, label3.Size.Height);

            lblEquipZone.SetBounds(lblSelectDate.Location.X + lblSelectDate.Width + 5, lblSelectDate.Location.Y, lblEquipZone.Width, lblEquipZone.Height);
            lblEquipZone.Location = new Point(lblEquipZone.Location.X + 5, lblEquipZone.Location.Y);
            lblAlarmType.SetBounds(lblEquipZone.Location.X + lblEquipZone.Width + 5, lblEquipZone.Location.Y, lblStatementEnd.Width, lblStatementEnd.Height);
            lblStatementEnd.SetBounds(lblAlarmType.Location.X + lblAlarmType.Width, lblAlarmType.Location.Y, lblStatementEnd.Width, lblStatementEnd.Height);

            //InitGridView();

            lblSearchLocation.Visible = false;
            m_dicAllTankZones = GetPSMTankZones();

            this.InitCtrlSize(this);
            FormMain.Instance.CustomizeGridView(gvMain);

            m_manualManager = new ManualManager(this);
            SetManualID();  
        }

        private Dictionary<string, Zone> GetPSMTankZones()
        {
            Dictionary<string, Zone> zones = new Dictionary<string, Zone>();

            List<UnE.PSM.PSMTank> tanks = PSMManager.Instance.GetTanks();

            if (tanks == null)
                return zones;

            foreach (UnE.PSM.PSMTank tank in tanks)
            {
                if (tank.EquipZone == null || tank.EquipZone.LinkedZone == null)
                    continue;

                Zone zone = tank.EquipZone.LinkedZone;

                zones[tank.LocationName] = zone;
            }

            return zones;
        }

        private ImageButton btnStartDate;
        private ImageButton btnEndDate;
        private ComboBox cboStartTime;
        private ComboBox cboEndTime;
        private ComboBox cboPSMSelect;
        private ComboBox cboSearchType;
        private ImageButton btnSearch;


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
            //m_ActionMgr.ZoneSubmit(arrSelectZoneList, startDate, EndDate, true);
            InitGridView();

            //찾은 검색결과를 DataGrid로 출력
            //Load_DataGrid();
        }

        public void ParentPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (isFormLoaded)
            {
                RefreshReactionItem();
            }
        }
        
        private void ActionPSMPage_Load(object sender, EventArgs e)
        {
            ArrayList arContorls = FormMain.Instance.GetActionPSMContorl();
            btnEndDate = (ImageButton)arContorls[0];
            btnStartDate = (ImageButton)arContorls[1];
            cboSearchType = (ComboBox)arContorls[2];
            cboPSMSelect = (ComboBox)arContorls[3];
            cboEndTime = (ComboBox)arContorls[4];
            cboStartTime = (ComboBox)arContorls[5];
            btnSearch = (ImageButton)arContorls[6];

            cboPSMSelect.DataSource = reactionPSMLogBindingSource;
            InitLoadData();

            //이벤트처리
            //설정한 기간을 HistorySubmit함수에 넘겨줌
            //HistorySubmit를 통해 누출탐지나 수동신고 된 ReactionLog를 HistoryID로 찾아와서 Combobox에 넣음

            //LoadPSMReaction();

            //this.cboSearchType.SelectionChangeCommitted += (s, eArgs) => { RefreshReactionItem(); };
            this.btnSearch.Click += (s, eArgs) => { RefreshReactionItem(); };

            //이벤트처리
            //사용자가 선택 한 Combobox의 HistoryID에 해당하는 전체 ReactionHistory를 가져옴
            this.cboPSMSelect.SelectionChangeCommitted += (s, eArgs) => { SelectPSMReaction(); };

            this.btnStartDate.TextChanged += (s, eArgs) => { ChangeStartDateString(); };
            this.btnEndDate.TextChanged += (s, eArgs) => { ChangeEndDateString(); };
            this.cboStartTime.SelectedIndexChanged += (s, eArgs) => { ChangeTimeString(); };
            this.cboEndTime.SelectedIndexChanged += (s, eArgs) => { ChangeTimeString(); };


            if (cboSearchType.Items.Count > 0)
                cboSearchType.SelectedIndex = cboSearchType.Items.Count - 1;

            //RefreshReactionItem();

            if (cboPSMSelect.Items.Count > 0)
                cboPSMSelect.SelectedIndex = cboPSMSelect.Items.Count - 1;
            //SelectPSMReaction();
            isFormLoaded = true;

            //최근6개월
            /*DateTime startDate = DateTime.Now.AddMonths(-6);
            DateTime EndDate = DateTime.Now;
            EndDate = EndDate.AddDays(1);

            m_SelectedMinDate = startDate;
            m_SelectedMaxDate = EndDate;
            m_arrSelectedZone = ZoneManager.Instance.FindZoneList("모든 건물 그룹", "모든 건물", "모든 층");*/
        }

        private void ActionPSMPage_Resize(object sender, System.EventArgs e)
        {
            SetChildCtrlResize(this, 0, 0);
            SetGridViewSize();

            lblActorName.Location = new Point(lblResult.Location.X + lblResult.Width, lblResult.Location.Y);
            lblSelectDate.Location = new Point(lblActorName.Location.X + lblActorName.Width, lblSelectDate.Location.Y);
            lblEquipZone.Location = new Point(lblSelectDate.Location.X + lblSelectDate.Width + 5, lblSelectDate.Location.Y); 
            lblAlarmType.Location = new Point(lblEquipZone.Location.X + lblEquipZone.Width + 5, lblEquipZone.Location.Y);
            lblStatementEnd.Location = new Point(lblAlarmType.Location.X + lblAlarmType.Width, lblAlarmType.Location.Y); 
        }

        private int GetMaxSensorReactionHistoryID()
        {
            string strSQL = "Select max(ID) from SensorReactionHistory";
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

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
            string strLocation = "";
            FormMain.Instance.GetPSMLocationInfo(ref strLocation);

            int nSensorHistoryID = GetMaxSensorReactionHistoryID();

            if (m_strSelectedLocation == strLocation)
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
                m_strSelectedLocation = strLocation;
                GetSelectedZones(strLocation);
            }

            m_SelectedMinDate = dtStart;
            m_SelectedMaxDate = dtEnd;
            m_nSelectedReactionType = nReactionType;
            m_nReadLastSensorReactionHistoryID = nSensorHistoryID;
            return RefreshType.REFRESH_ALL;
        }

        private void GetSelectedZones(string strLocationName)
        {
            m_arrSelectedZone.Clear();
            UnE.Spatial.Zone selectedZone = null;

            if (m_dicAllTankZones.TryGetValue(strLocationName, out selectedZone))
                m_arrSelectedZone.Add(selectedZone);
            else
            {
                foreach (KeyValuePair<string, Zone> pair in m_dicAllTankZones)
                {
                    m_arrSelectedZone.Add(pair.Value);
                }
            }
        }

        private void RefreshReactionType(int nReactionType)
        {
            m_nSelectedReactionType = nReactionType;

            cboPSMSelect.DataSource = null;
            reactionPSMLogBindingSource.Clear();

            foreach (Report.ReactionLog log in m_allLogs)
            {
                //누출신고만
                if (nReactionType == (int)ReactionType.NOTIFY_SIGNAL)
                {
                    if (log.Type == (int)ReactionType.BEGIN_STATUS || log.Type == (int)ReactionType.NOTIFY_SIGNAL)
                        reactionPSMLogBindingSource.Add(log);
                }
                else if (nReactionType == (int)ReactionType.MALFUNCTION || nReactionType == (int)ReactionType.USER_RESET) //시스템복구 포함
                {
                    if (log.Type == (int)ReactionType.BEGIN_STATUS || log.Type == (int)ReactionType.NOTIFY_SIGNAL || log.Type == (int)ReactionType.MALFUNCTION || log.Type == (int)ReactionType.USER_RESET)
                        reactionPSMLogBindingSource.Add(log);
                }
                else if (nReactionType == (int)ReactionType.IGNORE_SIGNAL) //현장복구 포함
                {
                    if (log.Type == (int)ReactionType.BEGIN_STATUS || log.Type == (int)ReactionType.NOTIFY_SIGNAL || log.Type == (int)ReactionType.IGNORE_SIGNAL)
                        reactionPSMLogBindingSource.Add(log);
                }
                else if (nReactionType == (int)ReactionType.ETC) //모든 신호
                {
                    if (log.Type == (int)ReactionType.BEGIN_STATUS || log.Type == (int)ReactionType.NOTIFY_SIGNAL || log.Type == (int)ReactionType.MALFUNCTION || log.Type == (int)ReactionType.USER_RESET || log.Type == (int)ReactionType.IGNORE_SIGNAL)
                        reactionPSMLogBindingSource.Add(log);
                }
            }

            cboPSMSelect.DataSource = reactionPSMLogBindingSource;
            this.gvMain.Rows.Clear();
        }

        private void LoadPSMReaction(int nReactionType = (int)ReactionType.IGNORE_SIGNAL)
        {
            DateTime dtStart = DateTime.ParseExact(btnStartDate.Text, "yyyy-MM-dd", null);
            DateTime dtEnd = DateTime.ParseExact(btnEndDate.Text, "yyyy-MM-dd", null);

            RefreshType refreshType = NeedRefresh(nReactionType, dtStart, dtEnd);

            if (refreshType == RefreshType.NO_REFRESH)
                return;
            else if (refreshType == RefreshType.REFRESH_REACTION_TYPE)
            {
                RefreshReactionType(nReactionType);
                return;
            }

            m_nSelectedReactionType = nReactionType;

            if (cboPSMSelect != null)
            {
                reactionPSMLogBindingSource.Clear();
                //cboPSMSelect.Items.Clear();
            }

            WebDBManager dbMgr = FormMain.Instance.DBManager;

            int start_Hour = 0;
            int End_Hour = 0;

            //시간이 비활성화일 때
            if (cboStartTime.Enabled == false && cboEndTime.Enabled == false)
            {
                start_Hour = 0;
                End_Hour = 24;
            }
            else
            {
                for (int i = 0; i < 25; i++)
                {
                    if (cboStartTime.Text == i + "시")
                    {
                        start_Hour = i;
                    }
                    if (cboEndTime.Text == i + "시")
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

            cboPSMSelect.DataSource = null;
            reactionPSMLogBindingSource.Clear();
            m_allLogs.Clear();
            //cboPSMSelect.Items.Clear();

            m_ActionMgr.DataClear();
            //설정한 기간, ZoneList를 ZoneSubmit함수에 넘겨줌
            m_ActionMgr.ZoneSubmit(m_arrSelectedZone, dtStart, dtEnd, true);

            //HistorySubmit를 통해 누출탐지나 수동신고 된 ReactionLog를 HistoryID로 찾아와서 Combobox에 넣음
            ArrayList arrComboData = m_ActionMgr.HistorySubmit(dtStart, dtEnd);

            //cboPSMSelect.Items.Clear();
            foreach (Report.ReactionPSMLog log in arrComboData)
            {
                //누출신고만   
                if (nReactionType == (int)ReactionType.NOTIFY_SIGNAL)
                {
                    if (log.Type == (int)ReactionType.BEGIN_STATUS || log.Type == (int)ReactionType.NOTIFY_SIGNAL)
                        reactionPSMLogBindingSource.Add(log);
                }
                else if (nReactionType == (int)ReactionType.MALFUNCTION || nReactionType == (int)ReactionType.USER_RESET) //시스템복구 포함
                {
                    if (log.Type == (int)ReactionType.BEGIN_STATUS || log.Type == (int)ReactionType.NOTIFY_SIGNAL || log.Type == (int)ReactionType.MALFUNCTION || log.Type == (int)ReactionType.USER_RESET)
                        reactionPSMLogBindingSource.Add(log);
                }
                else if (nReactionType == (int)ReactionType.IGNORE_SIGNAL) //현장복구 포함
                {
                    if (log.Type == (int)ReactionType.BEGIN_STATUS || log.Type == (int)ReactionType.NOTIFY_SIGNAL || log.Type == (int)ReactionType.IGNORE_SIGNAL)
                        reactionPSMLogBindingSource.Add(log);
                }
                else if (nReactionType == (int)ReactionType.ETC) //모든 신호
                {
                    if (log.Type == (int)ReactionType.BEGIN_STATUS || log.Type == (int)ReactionType.NOTIFY_SIGNAL || log.Type == (int)ReactionType.MALFUNCTION || log.Type == (int)ReactionType.USER_RESET || log.Type == (int)ReactionType.IGNORE_SIGNAL)
                        reactionPSMLogBindingSource.Add(log);
                }
            }

            cboPSMSelect.DataSource = reactionPSMLogBindingSource;
            this.gvMain.Rows.Clear();
        }

        private void RefreshReactionItem()
        {
            if (cboSearchType.SelectedIndex == 0)
            {
                nReactionType = (int)ReactionType.NOTIFY_SIGNAL;
                LoadPSMReaction(nReactionType);
            }
            else if (cboSearchType.SelectedIndex == 1)
            {
                nReactionType = (int)ReactionType.MALFUNCTION;
                LoadPSMReaction(nReactionType);
            }
            else if (cboSearchType.SelectedIndex == 2)
            {
                nReactionType = (int)ReactionType.IGNORE_SIGNAL;
                LoadPSMReaction(nReactionType);
            }
            else if (cboSearchType.SelectedIndex == 3)
            {
                nReactionType = (int)ReactionType.ETC;
                LoadPSMReaction(nReactionType);
            }

            cboPSMSelect.SelectedIndex = cboPSMSelect.Items.Count - 1;
            SelectPSMReaction();
        }

        private void RefreshAll()
        {
            SetLabelDate();
            DataGridViewControl.Rows.Clear();

            int nTemp = cboSearchType.SelectedIndex;
            cboSearchType.SelectedIndex = -1;

            cboPSMSelect.Items.Clear();
            cboSearchType.SelectedIndex = nTemp;
            cboPSMSelect.SelectedIndex = cboPSMSelect.Items.Count - 1;

            RefreshReactionItem();
        }

        private void SelectPSMReaction()
        {
            SetLabelDate();

            //선택한 Combobox를 ReactionLog클래스 형태로 가져옴
            Report.ReactionPSMLog data = (Report.ReactionPSMLog)cboPSMSelect.SelectedItem;

            if (data == null)
                return;


            lblDefault.Visible = false;

            string strTime = String.Format("{0}년 {1}월 {2}일 {3} {4}시 {5}분", data.Time.Year, data.Time.Month, data.Time.Day, (data.Time.Hour < 12 ? "오전" : "오후"), data.Time.Hour > 12 ? data.Time.Hour - 12 : data.Time.Hour, data.Time.Minute);

            //Label에 표시할 내용(기간)
            if (cboSearchType.SelectedIndex == -1)
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
                PrintGridData(data.HistoryID, data.Type);
            }
        }

        public void SelectHistory(int nSensorZoneHistoryID)
        {
            foreach (Report.ReactionPSMLog log in cboPSMSelect.Items)
            {
                if (log.HistoryID == nSensorZoneHistoryID)
                {
                    cboPSMSelect.SelectedItem = log;
                    SelectPSMReaction(); 
                    //PrintGridData(log.HistoryID, log.Type);
                    break;
                }
            }
        }

        public string GetReactionString(int nReactionType, ReactionPSMLog log)
        {
            string strType = String.Empty;

            switch (nReactionType)
            {
                case (int)ReactionType.SEND_SMS: strType = "문자메시지 발송";
                    break;
                case (int)ReactionType.RUN_DETECT_BROADCAST: strType = "사내 방송 실시(탐지)";
                    break;
                case (int)ReactionType.RUN_REPORT_BROADCAST: strType = "사내 방송 실시(신고)";
                    break;
                case (int)ReactionType.SEND_DETECT_SMS: strType = "문자메시지 발송(탐지)";
                    break;
                case (int)ReactionType.SEND_REPORT_SMS: strType = "문자메시지 발송(신고)";
                    break;
                case (int)ReactionType.SEND_REPAIR_SMS: strType = "문자메시지 발송(복구)";
                    break;
                case (int)ReactionType.USER_RESET: 
                case (int)ReactionType.MALFUNCTION: strType = "시스템 복구 처리";
                    break;
                case (int)ReactionType.RUN_SOP: strType = "SOP 발동";
                    break;
                case (int)ReactionType.RUN_N_CANCEL_SOP: strType = "SOP 실행후 취소";
                    break;
                case (int)ReactionType.FINISH_SOP: strType = "SOP 종료";
                    break;
                case (int)ReactionType.IGNORE_SOP: strType = "SOP 실행않고 상황 종료";
                    break;
                case (int)ReactionType.END_STATUS: strType = "상황 해제";
                    break;
                case (int)ReactionType.BEGIN_STATUS: strType = (log.Level != 0) ? String.Format("누출 탐지 - {0}단계", log.Level) : "누출 탐지";
                    break;
                case (int)ReactionType.IGNORE_SIGNAL: strType = "현장 복구";
                    break;
                case (int)ReactionType.CHANGE_ALARM_DEPTH: strType = String.Format("누출 단계 변경 - {0}단계", log.Level);
                    break;
                case (int)ReactionType.NOTIFY_SIGNAL: strType = "누출 신고";
                    break;
                default:
                    break;
            }

            return strType;
        }

        public void SetLabelString(string lblBuilding)
        {
            lblSearchLocation.Text = lblBuilding;
        }


        #region Edit Grid View

        private int m_nColumnCount = 7;
        private int m_nNO_INDEX = 0;
        private int m_nTIME_INDEX = 1;
        private int m_nMATERIAL_INDEX = 2;
        private int m_nMANAGER_INDEX = 3;
        private int m_nBUILDING_INDEX = 4;
        private int m_nDETECT_LOCATION_INDEX = 5;
        private int m_nACTION_INDEX = 6;

        private void InitGridView()
        {
            this.Controls.Add(gvMain);

            gvMain.ColumnCount = m_nColumnCount;

            gvMain.Columns[m_nNO_INDEX].Name = "No";            
            gvMain.Columns[m_nNO_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gvMain.Columns[m_nNO_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            gvMain.Columns[m_nNO_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;

            gvMain.Columns[m_nTIME_INDEX].Name = "일시";            
            gvMain.Columns[m_nTIME_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gvMain.Columns[m_nTIME_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;

            gvMain.Columns[m_nMATERIAL_INDEX].Name = "물질";            
            gvMain.Columns[m_nMATERIAL_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gvMain.Columns[m_nMATERIAL_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;

            gvMain.Columns[m_nMANAGER_INDEX].Name = "담당자";            
            gvMain.Columns[m_nMANAGER_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gvMain.Columns[m_nMANAGER_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;

            gvMain.Columns[m_nBUILDING_INDEX].Name = "건물";            
            gvMain.Columns[m_nBUILDING_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            gvMain.Columns[m_nBUILDING_INDEX].DefaultCellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            gvMain.Columns[m_nBUILDING_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;

            gvMain.Columns[m_nDETECT_LOCATION_INDEX].Name = "누출 발생장소";            
            gvMain.Columns[m_nDETECT_LOCATION_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            gvMain.Columns[m_nDETECT_LOCATION_INDEX].DefaultCellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            gvMain.Columns[m_nDETECT_LOCATION_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;

            gvMain.Columns[m_nACTION_INDEX].Name = "분류";            
            gvMain.Columns[m_nACTION_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gvMain.Columns[m_nACTION_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            gvMain.Columns[m_nACTION_INDEX].DefaultCellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            gvMain.Columns[m_nACTION_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;

            gvMain.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gvMain.Columns[m_nDETECT_LOCATION_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            SetGridViewSize();
        }

        private void SetGridViewSize()
        {
            float sizePer = 1f;
            if (FormMain.Instance.Resolution == Resolution.FullHD)
                sizePer = 0.5f;
            else if (FormMain.Instance.Resolution == Resolution.Other)
                sizePer = 0.75f;

            if (gvMain.Columns.Count > m_nACTION_INDEX)
            {
                gvMain.Columns[m_nNO_INDEX].Width = (int)(160 * sizePer);
                gvMain.Columns[m_nMATERIAL_INDEX].Width = (int)(200 * sizePer);
                gvMain.Columns[m_nTIME_INDEX].Width = (int)(440 * sizePer);
                gvMain.Columns[m_nMANAGER_INDEX].Width = (int)(200 * sizePer);
                gvMain.Columns[m_nBUILDING_INDEX].Width = (int)(560 * sizePer);
                gvMain.Columns[m_nDETECT_LOCATION_INDEX].Width = (int)(600 * sizePer);
                gvMain.Columns[m_nACTION_INDEX].Width = (int)(560 * sizePer); 
            }

            gvMain.Font = new Font(Program.prgFont, (int)(24.0f * sizePer));
        }

        public void PrintGridData(int ZoneHistoryID, int nReactionType)
        {
            lblStatementEnd.Visible = true;
            SaveArr.Clear();
            gvMain.Rows.Clear();

            SensorZoneHistoryID = ZoneHistoryID;

            ArrayList arrSensorReactionHistory = new ArrayList();

            int nHwpTable = 8;
            int k = 0;

            int nRowNo = 0;

            //사용자가 선택 한 Combobox의 HistoryID에 해당하는 전체 ReactionHistory를 가져옴
            arrSensorReactionHistory = m_ActionMgr.GetReactionLog(ZoneHistoryID);

            string strMemo = "";

            foreach (Report.ReactionPSMLog data in arrSensorReactionHistory)
            {
                //찾은 검색결과를 DataGrid로 출력
                if (!SetGridRows(data, nRowNo, GetReactionString(nReactionType, data)))
                    continue;

                if (strMemo.Length == 0)
                {
                    strMemo = GetMemo(data);
                }

                int HwpIndex = 0;
                HwpDataSet(nHwpTable, k, nRowNo, ref HwpIndex);

                nHwpTable += 6;
                nRowNo++;
            }

            if (iscomboChange == true)
            {
                //Rdo_NotProcess.Select();
                iscomboChange = false;
            }

            //조회기간
            //lblMinDate.Text = strDate;
            lblSelectDate.Text = strDate + "에";

            lblActorName.Location = new Point(lblResult.Location.X + lblResult.Width, lblResult.Location.Y);
            lblSelectDate.Location = new Point(lblActorName.Location.X + lblActorName.Width, lblSelectDate.Location.Y);
            lblEquipZone.Location = new Point(lblSelectDate.Location.X + lblSelectDate.Width + 5, lblSelectDate.Location.Y);
            lblAlarmType.Location = new Point(lblEquipZone.Location.X + lblEquipZone.Width + 5, lblEquipZone.Location.Y);
            lblStatementEnd.Location = new Point(lblAlarmType.Location.X + lblAlarmType.Width, lblAlarmType.Location.Y); 


            //원래있던 표의 줄 수를 저장함
            storage = gvMain.Rows.Count;

            textBoxMemo.Text = strMemo;
        }

        private string GetMemo(Report.ReactionPSMLog log)
        {
            foreach (SensorReactionPSMLog log2 in log.ArrLogList)
            {
                if (log2.Memo.Length > 0)
                    return log2.Memo;
            }

            return "";
        }

        private bool SetGridRows(Report.ReactionPSMLog dataLog, int nRowNo, string strReactionType)
        {
            if (dataLog == null)
                return false;
            if (dataLog.Zone == null)
                return false;
            if (nRowNo < 0)
                return false;


            Zone zone = dataLog.Zone;
            Building buildingFind = zone.Building;

            string strBuildingName = buildingFind == null ? "" : buildingFind.BuildingName;
            string strType = GetReactionString(dataLog.Type, dataLog);

            if (dataLog.Type == (int)ReactionType.MALFUNCTION
              || dataLog.Type == (int)ReactionType.USER_RESET)
            {
                strType = strReactionType;
            }

            lblActorName.Text = dataLog.UserName;

            //수동이면
            /*if (dataLog.SensorType == 0)
            {
                if (dataLog.Zone != null)
                {
                    lblEquipZone.Text = "";
                    m_strEquipZoneName = "-";
                    lblAlarmType.Text = "발생한 수동신고에";
                }

            }
            else */if (dataLog.SensorType == (int)UnE.Sensor.IFacility.FacilityType.PSM_SENSOR)//누출이면
            {
                if (dataLog.equipZone != null)
                {
                    m_strEquipZoneName = dataLog.equipZone.ZoneName;
                    lblEquipZone.Text = "【 " + m_strEquipZoneName + " 】";
                    lblAlarmType.Text = "에서 발생한 누출 감지신호에";
                }
            }
            else
            {
                lblAlarmType.Text = "";
            }

            lblResult.Text = strReactionType;

            if (strReactionType != "현장 복구" && strReactionType.IndexOf("누출 탐지") != 0)
            {
                if (strReactionType == "누출 신고")
                {
                    m_strNotifierName = dataLog.UserName;
                }

                if (String.IsNullOrWhiteSpace(dataLog.UserName) == false)
                    lblResult.Text += " - ";
            }

            if (buildingFind == null)
            {
                strBuildingName = zone.ZoneName;
            }

            if (strType.Trim().Length == 0)
                return false;

            // 같은 로그가 이미 기록되어 있는지 확인한다.
            if (ContainsType(strType, dataLog.Time))
                return false;

            gvMain.Rows.Add();
            gvMain.Rows[nRowNo].Cells[m_nNO_INDEX].Value = nRowNo + 1;
            gvMain.Rows[nRowNo].Cells[m_nTIME_INDEX].Value = dataLog.Time;
            gvMain.Rows[nRowNo].Cells[m_nMATERIAL_INDEX].Value = dataLog.PSMMaterial.Name;
            gvMain.Rows[nRowNo].Cells[m_nMANAGER_INDEX].Value = "";
            gvMain.Rows[nRowNo].Cells[m_nBUILDING_INDEX].Value = strBuildingName;
            gvMain.Rows[nRowNo].Cells[m_nDETECT_LOCATION_INDEX].Value = m_strEquipZoneName;
            gvMain.Rows[nRowNo].Cells[m_nACTION_INDEX].Value = strType;

            if (dataLog.Type == (int)ReactionType.NOTIFY_SIGNAL
                || dataLog.Type == (int)ReactionType.USER_RESET
                || dataLog.Type == (int)ReactionType.MALFUNCTION)
            {
                gvMain.Rows[nRowNo].Cells[m_nMANAGER_INDEX].Value = dataLog.UserName;
            }

            return true;
        }

        // strType, dtDate에 해당하는 값이 이미 존재하는지 검사한다.
        private bool ContainsType(string strType, DateTime dtDate)
        {
            string strTime = dtDate.ToString();

            foreach (DataGridViewRow row in gvMain.Rows)
            {
                if (row.Cells[m_nTIME_INDEX].Value != null && row.Cells[m_nTIME_INDEX].Value.ToString() == strTime)
                {
                    if (row.Cells[m_nACTION_INDEX].Value != null && row.Cells[m_nACTION_INDEX].Value.ToString() == strType)
                        return true;
                }
            }

            return false;
        }

        #endregion


        #region Setting Date & Time String

        public void ComboTxtDate(string strdate, string strdateTime)
        {
            strDate = strdate;
            lblMinDate.Text = strdateTime;
        }

        private void ChangeStartDateString()
        {
            if (btnStartDate.Text == btnEndDate.Text)
            {
                cboStartTime.Enabled = true;
                cboEndTime.Enabled = true;
            }
            else
            {
                cboStartTime.Enabled = false;
                cboEndTime.Enabled = false;
            }
            //RefreshAll();
        }

        private void ChangeEndDateString()
        {
            if (btnStartDate.Text == btnEndDate.Text)
            {
                cboStartTime.Enabled = true;
                cboEndTime.Enabled = true;
            }
            else
            {
                cboStartTime.Enabled = false;
                cboEndTime.Enabled = false;
            }
            //RefreshAll();
        }

        private void ChangeTimeString()
        {
            //RefreshAll();
        }

        #endregion


        #region Export

        private void HwpDataSet(int nHwpTable, int k, int count, ref int HwpIndex)
        {
            for (k = nHwpTable; k < nHwpTable + 5; k++)
            {
                if (k == nHwpTable + 4)
                    HwpIndex += 2;
                //데이터 수에 맞춰서 줄 늘림

                SaveArr.Add(gvMain.Rows[count].Cells[HwpIndex].Value.ToString());

                HwpIndex++;
            }
        }

        public void SetHwpData()
        {
            System.IO.StreamWriter stream = null;
            try
            {
                stream = new System.IO.StreamWriter(Application.StartupPath + "\\report\\SaveDateTime.txt");
                //stream.WriteLine(strManagerName);
                stream.WriteLine(lblMinDate.Text);
                stream.WriteLine(lblSearchLocation.Text);
                stream.WriteLine(m_strEquipZoneName);
                //stream.WriteLine(m_strNotifierName);
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

        #endregion

        private void btnSaveHWP_Click(object sender, EventArgs e)
        {
            if (m_manualManager.IsHelpMode)
                return;

            CloseReportMenu();

            btnSaveHWP.Enabled = false;
            PageBackstageHome.Instance.FrmReport.SaveHWPForActionPSM();
            btnSaveHWP.Enabled = true;
        }

        public void SetVisibleHWPExport(bool visible)
        {
            btnSaveHWP.Visible = visible;
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

            m_manualManager.SetID(this, "SDMS_Report_Action_PSM");
            m_manualManager.SetID(lblReportTitle, "SDMS_Report_Action_PSM");
            m_manualManager.SetID(btnSaveHWP, "Action_PSM_ExportReport");
            m_manualManager.SetID(lblSelectDate, "Action_PSM_Grid");
            m_manualManager.SetID(lblEquipZone, "Action_PSM_Grid");
            m_manualManager.SetID(gvMain, "Action_PSM_Grid");
            m_manualManager.SetID(textBoxMemo, "Action_PSM_Grid");

            m_manualManager.ProcessEvent();
        }
    }
}