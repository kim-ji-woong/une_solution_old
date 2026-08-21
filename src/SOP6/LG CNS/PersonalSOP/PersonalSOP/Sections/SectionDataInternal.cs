using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace PersonalSOP.Sections
{
    using Models;

    public class SectionDataInternal : SectionData
    {
        // PC Popup Message
        protected bool m_usePopupMessage = false;
        protected bool m_useMobileApp = true;
        protected bool m_useBroadcast = false;
        // 자동실행 여부
        protected bool m_autoRun = false;

        // Default 문자열을 사용하여 작성된 ID 개수
        protected static Dictionary<string, int> DEFAULT_ID_COUNT = new Dictionary<string, int>();

        public static void ClearIDCount()
        {
            DEFAULT_ID_COUNT.Clear();
        }

        public override void SetDefaultID(string strStepName, string strTeamName)
        {
            MakeDefaultID(strStepName, strTeamName, DEFAULT_ID_COUNT, "Internal");
        }

        protected override void AddDefaultID(string strTag, int nTagCount)
        {
            DEFAULT_ID_COUNT[strTag] = nTagCount;
        }

        // nTagCount가 strTag에 대한 최대값이면 최대값을 1만큼 낮춰준다.
        protected override void RemoveMaxDefaultCount(string strTag, int nTagCount)
        {
            if (DEFAULT_ID_COUNT.ContainsKey(strTag))
            {
                if (DEFAULT_ID_COUNT[strTag] == nTagCount)
                    DEFAULT_ID_COUNT[strTag] = nTagCount - 1;
            }
        }

        private string m_szBroadcastMessage = null;
        public string BroadcastMessage
        {
            get { return m_szBroadcastMessage; }
            set { m_szBroadcastMessage = value; }
        }

        public bool UsePopupMessage
        {
            get { return m_usePopupMessage; }
            set { m_usePopupMessage = value; }
        }

        public bool UseMobileApp
        {
            get { return m_useMobileApp; }
            set { m_useMobileApp = value; }
        }

        public bool UseBroadcast
        {
            get { return m_useBroadcast; }
            set { m_useBroadcast = value; }
        }

        protected bool m_bTransferTeamLeaderOnly = true;
        public bool TransferTeamLeaderOnly
        {
            get { return m_bTransferTeamLeaderOnly; }
            set { m_bTransferTeamLeaderOnly = value; }
        }

        protected ArrayList m_arTeamList = new ArrayList();
        public ArrayList TeamList
        {
            get { return m_arTeamList; }
            set { m_arTeamList = value; }
        }

        protected SectionCommander m_Commander = new SectionCommander();
        public SectionCommander Commander
        {
            get { return m_Commander; }
            set { m_Commander = value; }
        }

        private bool m_bUseSiren = false;
        public bool UseSiren
        {
            get { return m_bUseSiren; }
            set { m_bUseSiren = value; }
        }

        private int m_nRepeatCount = 1;
        public int RepeatCount
        {
            get { return m_nRepeatCount; }
            set { m_nRepeatCount = value; }
        }

        public bool AutoRun
        {
            get { return m_autoRun; }
            set { m_autoRun = value; }
        }
    }
}