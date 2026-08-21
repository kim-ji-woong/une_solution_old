using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using XtremePropertyGrid;

namespace SOPMonitoringSystem
{
    public partial class DockingLeftPropertiesLevel : Form
    {
        private PropertyGridItem m_itemLevel;
        private PropertyGridItem m_itemPeriod;
        private PropertyGridItem m_itemNumber;
        private PropertyGridItem m_itemProcessTime;
        private PropertyGridItem m_itemParent;
        private PropertyGridItem m_itemExecuteCount;
        private PropertyGridItem m_itemExecuteRecurrence;
        private PropertyGridItem m_itemBeginTime;
        private PropertyGridItem m_itemEndTime;

        private ArrayList m_arrActionStep = new ArrayList();
        private ArrayList m_arrLevelProperties = new ArrayList();
        public ArrayList LevelProperties
        {
            get { return m_arrLevelProperties; }
            set { m_arrLevelProperties = value; }
        }

        //private string m_strParent;
        //private string m_strLevelName;

        private string m_strTerm;
        private int m_nPeriodType;
        private DateTime m_dtBeginTime;
        private DateTime m_dtEndTime;
        private int m_nWeekDayOPtion;
        private string m_strNumber;
        private int m_nNumberType;
        private string m_strProcessTime;
        private int m_nProcessType;

        public string Number // 횟수
        {
            get { return m_strNumber; }
            set { m_strNumber = value; }
        }

        public int NumberType
        {
            get { return m_nNumberType; }
            set { m_nNumberType = value; }
        }

        public string Term // 기간
        {
            get { return m_strTerm; }
            set { m_strTerm = value; }
        }

        public int PeriodType //기간 타입
        {
            get { return m_nPeriodType; }
            set { m_nPeriodType = value; }
        }

        public DateTime BeginTime // 기간의 시작 시간
        {
            get { return m_dtBeginTime; }
            set { m_dtBeginTime = value; }
        }

        public DateTime EndTime // 기간의 끝 시간
        {
            get { return m_dtEndTime; }
            set { m_dtEndTime = value; }
        }
        
        public int WeekDayOPtion // 기간의 요일 옵션
        {
            get { return m_nWeekDayOPtion; }
            set { m_nWeekDayOPtion = value; }
        }

        public string ProcessTime // 처리시간
        {
            get { return m_strProcessTime; }
            set { m_strProcessTime = value; }
        }

        public int ProcessType
        {
            get { return m_nProcessType; }
            set { m_nProcessType = value; }
        }

        public DockingLeftPropertiesLevel()
        {
            InitializeComponent();
            Init();

            ClearProperties();
        }

        PropertyGridItem CategoryEtc = null;

        private void Init()
        {
            PropertyGridItem CategoryNormal = axPropertyGrid.AddCategory("일반");
            m_itemLevel = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "단계명", "");
            m_itemLevel.Id = ID.ID_ITEM_LEVEL;

            m_itemPeriod = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "기간", "");
            m_itemPeriod.Id = ID.ID_ITEM_PERIOD;

            m_itemNumber = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "횟수", "");
            m_itemNumber.Id = ID.ID_ITEM_NUMBER;

            m_itemProcessTime = CategoryNormal.AddChildItem(PropertyItemType.PropertyItemString, "처리시간", "");
            m_itemProcessTime.Id = ID.ID_ITEM_PROCESS_TIME;
            CategoryNormal.Expanded = true;

            //PropertyGridItem CategoryEtc = axPropertyGrid.AddCategory("기타");
            CategoryEtc = axPropertyGrid.AddCategory("기타");
            m_itemParent = CategoryEtc.AddChildItem(PropertyItemType.PropertyItemString, "부모 단계", "");
            m_itemParent.Id = ID.ID_ITEM_PARENT;
            CategoryEtc.Expanded = true;

            PropertyGridItem CategoryInfo = axPropertyGrid.AddCategory("실행이력");
            m_itemExecuteCount = CategoryInfo.AddChildItem(PropertyItemType.PropertyItemString, "총 실행 횟수", "");
            m_itemExecuteCount.Id = ID.ID_ITEM_EXECUTE_COUNT;
            m_itemExecuteRecurrence = CategoryInfo.AddChildItem(PropertyItemType.PropertyItemEnum, "실행 회차", 1);
            m_itemExecuteRecurrence.Id = ID.ID_ITEM_EXECUTE_RECURRENCE;
            m_itemBeginTime = CategoryInfo.AddChildItem(PropertyItemType.PropertyItemString, "시작 시간", "");
            m_itemBeginTime.Id = ID.ID_ITEM_BEGIN_TIME;
            m_itemEndTime = CategoryInfo.AddChildItem(PropertyItemType.PropertyItemString, "종료 시간", "");
            m_itemEndTime.Id = ID.ID_ITEM_END_TIME;
            CategoryInfo.Expanded = true;
        }

        public void ClearProperties()
        {
            //m_itemLevel.Value = "";
            m_itemPeriod.Value = "";
            m_itemNumber.Value = "";
            m_itemProcessTime.Value = "";
            m_itemParent.Value = "";
        }

        public void AddTitle(string strValue)
        {
            labelTitle.Text = strValue;
        }

        public string GetTitle()
        {
            return labelTitle.Text;
        }

        public void GetDisasterInfo(DisasterInfo disaster)
        {
            m_arrActionStep = disaster.ActionSteps;
        }

        public void GetLevelProperties(TabPage tabPage)
        {
            Sections.SectionTabPage page = (Sections.SectionTabPage)tabPage;

            foreach (ActionStepInfo data in m_arrActionStep)
            {
                if (page.Text == data.ActionStepName)
                {
                    // 단계명
                    m_itemLevel.Value = data.ActionStepName;

                    // 기간
                    int nType = data.PeriodType;
                    int nWeekday = data.WeekDayOption;
                    m_itemPeriod.Value = GetPeriodData(data, nType);

                    // 횟수
                    string strNumberType = GetNumberType(data.IterationType, data.Iteration);
                    m_itemNumber.Value = strNumberType;

                    // 처리시간
                    string strProcessType = GetProcessType(data.ProcessTime, data.ProcessTimeType);
                    m_itemProcessTime.Value = strProcessType;

                    // 부모단계
                    m_itemParent.Value = GetParentName(m_arrActionStep, data.ParentStepID);

                    int nCount = History.HistoryManager.Instance.ActionStepHistory.GetCompletedCount(data.ActionStepID, !page.VirtualMode); //총 실행횟수
                    m_itemExecuteCount.Value = nCount;

                    m_itemExecuteRecurrence.Constraints.Clear();

                    if (nCount < 1)
                    {
                        m_itemExecuteRecurrence.Value = "";
                        m_itemBeginTime.Value = "";
                        m_itemEndTime.Value = "";

                        return;
                    }

                    for (int i = 1; i < nCount+1 ; i++)
                    { 
                        History.ActionStepUnitHistory unitHistory = History.HistoryManager.Instance.ActionStepHistory.GetHistory(data.ActionStepID, !page.VirtualMode, i-1); // 실행 회차

                        if (unitHistory != null)
                        {
                            m_itemExecuteRecurrence.Constraints.Add(i.ToString(), i);
                        }
                    }

                    m_itemExecuteRecurrence.MaskedText = nCount.ToString();
                    m_itemExecuteRecurrence.Selected = true;

                    int nIndex = (int)m_itemExecuteRecurrence.Value;
                    History.ActionStepUnitHistory unitHistory2 = History.HistoryManager.Instance.ActionStepHistory.GetHistory(data.ActionStepID, !page.VirtualMode, nIndex-1);

                    if (unitHistory2 != null)
                    {
                        DateTime begin = unitHistory2.BeginTime;
                        m_itemBeginTime.Value = begin.ToString();
                        DateTime end = unitHistory2.EndTime;
                        m_itemEndTime.Value = end.ToString();
                    }                    
                    else
                    {
                        m_itemBeginTime.Value = "";
                        m_itemEndTime.Value = "";
                    }
                   
                }
            }
        }

        public void AddProperties(History.ActionStepUnitHistory unitHistory)
        {
            if (unitHistory != null)
            {
                int nCount = m_itemExecuteRecurrence.Constraints.Count + 1;
                m_itemExecuteRecurrence.Constraints.Add(nCount.ToString(), nCount);
                m_itemExecuteRecurrence.Value = nCount;
            }

            DateTime begin = unitHistory.BeginTime;
            m_itemBeginTime.Value = begin.ToString();

            DateTime end = unitHistory.EndTime;
            m_itemEndTime.Value = end.ToString();
        }

        private void axPropertyGrid_ValueChanged(object sender, AxXtremePropertyGrid._DPropertyGridEvents_ValueChangedEvent e)
        {
            switch(e.item.Id)
            {
                case ID.ID_ITEM_EXECUTE_RECURRENCE:
                    Sections.SectionTabPage page = (Sections.SectionTabPage)FormMain.Instance.GetPageHome().TabControls.SelectedTab;
                    if (page != null)
                    {
                        int nActionStepID = page.ActionStepID;
                        int nIndex = (int)m_itemExecuteRecurrence.Value;

                        History.ActionStepUnitHistory unitHistory2 = History.HistoryManager.Instance.ActionStepHistory.GetHistory(nActionStepID, !page.VirtualMode, nIndex);

                        DateTime begin = unitHistory2.BeginTime;
                        m_itemBeginTime.Value = begin.ToString();

                        DateTime end = unitHistory2.EndTime;
                        m_itemEndTime.Value = end.ToString();
                    }

                    break;
            }
        }

        private void axPropertyGrid_InplaceButtonDown(object sender, AxXtremePropertyGrid._DPropertyGridEvents_InplaceButtonDownEvent e)
        {
            //switch (e.button.Item.Id)
            //{
            //    case ID.ID_ITEM_EXECUTE_RECURRENCE:
            //        int n = 0;
            //        break;
            //}
            //    case ID.ID_ITEM_PERIOD: // 기간
            //        PopupProcessTerm popupTerm = new PopupProcessTerm();
            //        popupTerm.SetTerm(m_strTerm);
            //        if (popupTerm.ShowDialog() == DialogResult.OK)
            //        {
            //            e.button.Item.Value = m_strTerm;
            //            LevelOptionList(ID.ID_ITEM_PERIOD, m_strTerm);
            //        }
            //        break;
            //    case ID.ID_ITEM_NUMBER:
            //        PopupProcessNumber popupNumber = new PopupProcessNumber();
            //        m_nNumberType = popupNumber.SetNumberType(m_strNumber);
            //        if (popupNumber.ShowDialog() == DialogResult.OK)
            //        {
            //            e.button.Item.Value = m_strNumber;
            //            LevelOptionList(ID.ID_ITEM_NUMBER, m_strNumber);
            //        }
            //        break;
            //    case ID.ID_ITEM_PROCESS_TIME:
            //        PopupProcessTime popupProcessTime = new PopupProcessTime();
            //        popupProcessTime.ItemID = ID.ID_ITEM_PROCESS_TIME;
            //        popupProcessTime.SetProcessingTime(m_strProcessTime);
            //        if (popupProcessTime.ShowDialog() == DialogResult.OK)
            //        {
            //            e.button.Item.Value = m_strProcessTime;
            //            LevelOptionList(ID.ID_ITEM_PROCESS_TIME, m_strProcessTime);
            //        }
            //        break;
            //    case ID.ID_ITEM_PARENT:
            //        AddParentName();
            //        break;
            //}
        }

        private string GetParentName(ArrayList arr, int nParentStepID)
        {
            foreach (ActionStepInfo data in arr)
            {
                if (data.ActionStepID == nParentStepID)
                    return data.ActionStepName;
            }
            return null;
        }

        private string GetPeriodData(ActionStepInfo data, int nType)
        {
            m_nWeekDayOPtion = data.WeekDayOption;

            DateTime dtBegin = data.BeginTime;
            DateTime dtEnd = data.EndTime;

            m_dtBeginTime = dtBegin;
            m_dtEndTime = dtEnd;

            string strBeginYear = dtBegin.Year.ToString();
            string strEndYear = dtEnd.Year.ToString();

            string strBeginMonth = dtBegin.Month.ToString();
            string strBeginDay = dtBegin.Day.ToString();

            string strBeginHour = dtBegin.Hour.ToString();
            string strBeginMinute = dtBegin.Minute.ToString();

            string strEndMonth = dtEnd.Month.ToString();
            string strEndDay = dtEnd.Day.ToString();

            string strEndHour = dtEnd.Hour.ToString();
            string strEndMinute = dtEnd.Minute.ToString();

            string strValue = "";
            // 0(사용 안함), 1(날짜 옵션, n1월 n2일 ~ m1월 m2일까지), 2(시간 옵션, n1시 n2분 ~ m1월 m2일까지), 3(날짜 옵션 + 시간 옵션),
            // 기간 Type이 10보다 크거나 같으면 고정 년도 사용
            switch (nType)
            {
                case 1: // 날짜
                    strValue = strBeginMonth + "/" + strBeginDay + " ~ " + strEndMonth + "/" + strEndDay;
                    break;
                case 2: // 시간
                    strValue = strBeginHour + ":" + strBeginMinute + " ~ " + strEndHour + ":" + strEndMinute;
                    break;
                case 3: // 날짜 + 시간
                    strValue = strBeginMonth + "/" + strBeginDay + " " + strBeginHour + ":" + strBeginMinute + " ~ " +
                                        strEndMonth + "/" + strEndDay + " " + strEndHour + ":" + strEndMinute;
                    break;
                case 11: // 고정년도 + 1
                    strValue = strBeginYear + "/" + strBeginMonth + "/" + strBeginDay + " ~ " + strEndYear + "/" + strEndMonth + "/" + strEndDay;
                    break;
                case 12: // 고정년도 + 2
                    strValue = strBeginYear + " " + strBeginHour + ":" + strBeginMinute + " ~ " + strEndYear + " " + strEndHour + ":" + strEndMinute;
                    break;
                case 13: // 고정년도 + 3
                    strValue = strBeginYear + "/" + strBeginMonth + "/" + strBeginDay + " " + strBeginHour + ":" + strBeginMinute + " ~ " +
                                        strEndYear + "/" + strEndMonth + "/" + strEndDay + " " + strEndHour + ":" + strEndMinute;
                    break;
                default:
                    strValue = "사용안함";
                    break;
            }

            return strValue;
        }

        public string GetNumberType(int nType, int nNumber)
        {
            string[] strOption = { "전체기간중", "연중", "월중", "주중", "하루중", "시간당" };

            if (nNumber == 0)
                nNumber = 1;
            
            string strValue = strOption[nType] + " " + nNumber.ToString() + "회";

            return strValue;
        }

        public string GetProcessType(int nTime, int nType)
        {
            string[] strOption = { "개월", "주", "일", "시간", "분", "알수없음" };
            string strValue = "";
            if (nType == 5)
                strValue = strOption[nType];
            else
                strValue = nTime + " " + strOption[nType];

            return strOption[nType];
        }

    }
}
