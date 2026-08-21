using System;

namespace Common.Model.History
{
    public class ComponentHistory
    {
        public enum Fields { ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, CompleteCount, ShowBoard, AccessedUserID, CheckedNotify1, CheckedNotify2, CheckedRun, CheckedComplete, Description };

        private int m_nID = -1;
        private int m_nActionStepHistoryID = -1;
        private int m_nComponentID = -1;
        // PROCESS(0), DECISION(1), ANNOTATION(2), ENDPOINT(3), LINK(4), TRANSSOP(5), INTERNAL(6), EXTERNAL(7), NONE(8)
        private int m_nComponentType = -1;
        private DateTime m_dtTime;
        // 하위 2바이트(실행상태) : 대기상태(1), 실행중(2), 완료(3), 입력대기상태(4), 건너뜀 상태(5)
        // 상위 2바이트(실행방향, bit flag 조합) : 위쪽(1), 오른쪽(2), 아래쪽(4), 왼쪽(8)
        private int m_nStatus = -1;
        private string m_strTask = null;
        private int? m_nCompleteCount = null;
        private bool? m_showBoard = null;
        private int? m_nAccessedUserID = null;
        private int? m_nCheckedNotify1 = null;
        private int? m_nCheckedNotify2 = null;
        private int? m_nCheckedRun = null;
        private int? m_nCheckedComplete = null;
        private string m_strDescription = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }

        public int ComponentID
        {
            get { return m_nComponentID; }
            set { m_nComponentID = value; }
        }

        /// <summary>
        /// PROCESS(0), DECISION(1), ANNOTATION(2), ENDPOINT(3), LINK(4), TRANSSOP(5), INTERNAL(6), EXTERNAL(7), NONE(8)
        /// </summary>
        public int ComponentType
        {
            get { return m_nComponentType; }
            set { m_nComponentType = value; }
        }

        public DateTime Time
        {
            get { return m_dtTime; }
            set { m_dtTime = value; }
        }

        /// <summary>
        /// 하위 2바이트(실행상태) : 대기상태(1), 실행중(2), 완료(3), 입력대기상태(4), 건너뜀 상태(5)
        /// 상위 2바이트(실행방향, bit flag 조합) : 위쪽(1), 오른쪽(2), 아래쪽(4), 왼쪽(8)
        /// </summary>
        public int Status
        {
            get { return m_nStatus; }
            set { m_nStatus = value; }
        }

        public string Task
        {
            get { return m_strTask; }
            set { m_strTask = value; }
        }

        public int? CompleteCount
        {
            get { return m_nCompleteCount; }
            set { m_nCompleteCount = value; }
        }

        public bool? ShowBoard
        {
            get { return m_showBoard; }
            set { m_showBoard = value; }
        }

        public int? AccessedUserID
        {
            get { return m_nAccessedUserID; }
            set { m_nAccessedUserID = value; }
        }

        public int? CheckedNotify1
        {
            get { return m_nCheckedNotify1; }
            set { m_nCheckedNotify1 = value; }
        }

        public int? CheckedNotify2
        {
            get { return m_nCheckedNotify2; }
            set { m_nCheckedNotify2 = value; }
        }

        public int? CheckedRun
        {
            get { return m_nCheckedRun; }
            set { m_nCheckedRun = value; }
        }

        public int? CheckedComplete
        {
            get { return m_nCheckedComplete; }
            set { m_nCheckedComplete = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public static string TableName
        {
            get { return "SopHistoryComponent"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ID ||
                field == Fields.ActionStepHistoryID ||
                field == Fields.ComponentID ||
                field == Fields.ComponentType ||
                field == Fields.Time ||
                field == Fields.Status)
                isNullable = false;
            else
                isNullable = true;

            return field.ToString();
        }
    }
}
