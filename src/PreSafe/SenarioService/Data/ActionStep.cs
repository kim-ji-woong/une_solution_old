using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreSafe
{
    public class ActionStep
    {
        // PeriodType : 기간 Type : 0(사용 안함), 1(날짜 옵션, n1월 n2일 ~ m1월 m2일까지), 2(시간 옵션, n1시 n2분 ~ m1월 m2일까지), 3(날짜 옵션 + 시간 옵션),
        //                                      11(고정 년도 사용 + 날짜 옵션), 12(고정 년도 사용 + 시간 옵션), 13(고정 년도 사용 + 날짜 옵션 + 시간 옵션)
        // WeekDayOption : 요일 옵션(bit 연산), bit : 1(일요일), 2(월요일), 4(화요일), 8(수요일), 16(목요일), 32(금요일), 64(토요일)
        // Iteration : 반복 회수
        // IterationType : 반복 회수 옵션 : 0(전체 기간중 몇회), 1(년중 몇회), 2(월중 몇회), 3(주중 몇회), 4(하루중 몇회), 5(시간당 몇회)
        // ProcessTimeType : 처리시간 옵션, 0(개월), 1(주), 2(일), 3(시간), 4(분)

        private int m_nID;
        private string m_strStepName;
        private int m_nPeriodType;
        private DateTime m_dtBeginTime;
        private DateTime m_dtEndTime;
        private int m_nWeekdayOption = 127;
        private int m_nIteration;
        private int m_nIterationType;
        private int m_nProcessTime;
        private int m_nProcessTimeType = 5;
        private int m_nDisasterID;
        private int m_nParentStepID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public int PeriodType
        {
            get { return m_nPeriodType; }
            set { m_nPeriodType = value; }
        }
        public DateTime BeginTime
        {
            get { return m_dtBeginTime; }
            set { m_dtBeginTime = value; }
        }
        public DateTime EndTime
        {
            get { return m_dtEndTime; }
            set { m_dtEndTime = value; }
        }
        public int WeekdayOption
        {
            get { return m_nWeekdayOption; }
            set { m_nWeekdayOption = value; }
        }
        public int Iteration
        {
            get { return m_nIteration; }
            set { m_nIteration = value; }
        }
        public int IterationType
        {
            get { return m_nIterationType; }
            set { m_nIterationType = value; }
        }
        public int ProcessTime
        {
            get { return m_nProcessTime; }
            set { m_nProcessTime = value; }
        }
        public int ProcessTimeType
        {
            get { return m_nProcessTimeType; }
            set { m_nProcessTimeType = value; }
        }
        public int DisasterID
        {
            get { return m_nDisasterID; }
            set { m_nDisasterID = value; }
        }
        public int ParentStepID
        {
            get { return m_nParentStepID; }
            set { m_nParentStepID = value; }
        }

        public bool m_bSelected = false;
        public bool Selected
        {
            get { return m_bSelected; }
            set
            {
                m_bSelected = value;
                
            }
        }

        private string m_szTeamName = "";
        public string TeamName
        {
            get { return m_szTeamName; }
            set
            {
                m_szTeamName = value;               
            }
        }

        public string StepName
        {
            get { return m_strStepName; }
            set
            {               
                m_strStepName = value;
            }
        }

        private ArrayList m_arScriptSection = new ArrayList();
        public ArrayList SectionList
        {
            get { return m_arScriptSection; }
            set { m_arScriptSection = value; }
        }


    }
}
