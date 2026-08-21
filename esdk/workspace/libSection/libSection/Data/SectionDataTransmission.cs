using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Sections
{
    public class SectionDataTransmission : SectionData
    {
        // 내부 상황전파용 데이터
        private InternalData m_dataInternal = new InternalData();
        // 외부 상황전파용 데이터
        private ExternalData m_dataExternal = new ExternalData();

        // Default 문자열을 사용하여 작성된 ID 개수
        protected static Dictionary<string, int> DEFAULT_ID_COUNT = new Dictionary<string, int>();

        public static void ClearIDCount()
        {
            DEFAULT_ID_COUNT.Clear();
        }

        public override void SetDefaultID(string strStepName, string strTeamName)
        {
            MakeDefaultID(strStepName, strTeamName, DEFAULT_ID_COUNT, "Transmission");
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

        public InternalData DataInternal
        {
            get { return m_dataInternal; }
            set { m_dataInternal = value; }
        }

        public ExternalData DataExternal
        {
            get { return m_dataExternal; }
            set { m_dataExternal = value; }
        }

        public class InternalData
        {
            // PC Popup Message
            private bool m_usePopupMessage = true;
            private bool m_useMobileApp = true;
            private bool m_useBroadcast = true;
            private string m_szMessage = "";
            public string BroadcastMessage
            {
                get { return m_szMessage; }
                set { m_szMessage = value; }
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
        }

        public class ExternalData
        {
            // 상황전파 사용할 것인가?
            private bool m_useSMS = true;
            private bool m_useFax = true;
            private string m_strSMSMessage = "";
            private ArrayList m_arrSMSReceivers = new ArrayList();
            private ArrayList m_arrFaxReceivers = new ArrayList();

            public bool UseSMS
            {
                get { return m_useSMS; }
                set { m_useSMS = value; }
            }

            public bool UseFax
            {
                get { return m_useFax; }
                set { m_useFax = value; }
            }

            public string SMSMessage
            {
                get { return m_strSMSMessage; }
                set { m_strSMSMessage = value; }
            }

            public ArrayList SMSReceivers
            {
                get { return m_arrSMSReceivers; }
            }

            public ArrayList FaxReceivers
            {
                get { return m_arrFaxReceivers; }
            }
        }
    }
}
