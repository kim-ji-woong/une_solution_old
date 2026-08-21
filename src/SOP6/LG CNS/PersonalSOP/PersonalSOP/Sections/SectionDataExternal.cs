using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace PersonalSOP.Sections
{
    using Models;

    public class SectionDataExternal : SectionData
    {
        // 상황전파 사용할 것인가?
        protected bool m_useSMS = true;
        protected bool m_useFax = false;
        protected string m_strSMSMessage = "";
        protected ArrayList m_arrSMSReceivers = new ArrayList();
        protected ArrayList m_arrFaxReceivers = new ArrayList();

        // Default 문자열을 사용하여 작성된 ID 개수
        protected static Dictionary<string, int> DEFAULT_ID_COUNT = new Dictionary<string, int>();

        public static void ClearIDCount()
        {
            DEFAULT_ID_COUNT.Clear();
        }

        public override void SetDefaultID(string strStepName, string strTeamName)
        {
            MakeDefaultID(strStepName, strTeamName, DEFAULT_ID_COUNT, "External");
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

        // 상황전파(문자메시지) 사용할 것인가?
        public bool UseSMS
        {
            get { return m_useSMS; }
            set { m_useSMS = value; }
        }

        // 상황전파(eFax) 사용할 것인가?
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
    public class ExternalTeamData
    {
        protected int m_nTeamID = -1;
        protected string m_strTeamName = "";
        // "-"나 빈칸없이 숫자만 존재함
        protected string m_strPhoneNumber = "";
        protected string m_strFaxNumber = "";


        protected int m_nParentTeamID = -1;
        public int ParentTeamID
        {
            get { return m_nParentTeamID; }
            set { m_nParentTeamID = value; }
        }

        public ExternalTeamData()
        {
        }

        public ExternalTeamData(int nTeamID, string strTeamName, string strPhoneNumber, string strFaxNumber)
        {
            m_nTeamID = nTeamID;
            m_strTeamName = strTeamName;
            m_strPhoneNumber = strPhoneNumber;
            m_strFaxNumber = strFaxNumber;
        }

        public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        public string FaxNumber
        {
            get { return m_strFaxNumber; }
            set { m_strFaxNumber = value; }
        }
    }

}