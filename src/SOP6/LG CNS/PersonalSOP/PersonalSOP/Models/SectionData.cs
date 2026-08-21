using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalSOP.Models
{
    public abstract class SectionData
    {
        private bool m_showMessageBox = true;

        private string m_strTitle = "";
        protected string m_strComponentID = "";
        
        protected int m_nSectionNumber = -1;
        public int SectionNumber
        {
            get { return m_nSectionNumber; }
            set { m_nSectionNumber = value; }
        }

        // DB Component ID
        private int m_nID = -1;

        // Key : Component ID
        // Value : Section ID
        // Section은 타입이 다르면 같은 ID가 존재할 수도 있는데, 어차피 Section 타입이 다르면 Component ID도 다르게 사용한다.
        protected static Dictionary<string, int> ID_LIST = new Dictionary<string, int>();
        //protected static ArrayList ID_LIST = new ArrayList();
        
        protected void MakeDefaultID(string strStepName, string strTeamName, Dictionary<string, int> dicIDCount, string strComponentType)
        {
            string strTag = strStepName + "_" + strTeamName + "_" + strComponentType;

            if (dicIDCount.ContainsKey(strTag))
            {
                int nTagCount = dicIDCount[strTag];

                m_strComponentID = string.Format("{0}_{1}", strTag, nTagCount + 1);
                dicIDCount[strTag] = nTagCount + 1;
            }
            else
            {
                m_strComponentID = strTag + "_1";
                dicIDCount[strTag] = 1;
            }

            ID_LIST[m_strComponentID] = this.m_nID;
            //ID_LIST.Add(m_strComponentID);
        }

        // strID가 이미 존재하는 ID인지 검사한다.
        // 존재하지 않으면 true, 존재하면 false를 리턴한다.
        protected bool CheckExist(string strID)
        {
            int nSectionID;

            if (ID_LIST.TryGetValue(strID, out nSectionID) == false)
                return true;

            if (nSectionID == m_nID)
                return true;

            return false;
            //return !ID_LIST.Contains(strID);
        }

        // Default ID는 [Component 고유 문자열 + '_' + 숫자]의 형식을 따른다.
        // strID가 Default ID Type인지 알려준다.
        protected static bool CheckDefaultStringType(string strID, out string strTag, out int nTagCount)
        {
            strTag = "";
            nTagCount = 0;

            int nLastIndex = strID.LastIndexOf('_');
            if (nLastIndex < 0)
                return false;

            string str = strID.Substring(nLastIndex + 1);

            try
            {
                nTagCount = int.Parse(str);
            }
            catch (Exception)
            {
                return false;
            }

            strTag = strID.Substring(0, nLastIndex);
            return true;
        }

        public abstract void SetDefaultID(string strStepName, string strTeamName);
        protected abstract void AddDefaultID(string strTag, int nTagCount);
        protected abstract void RemoveMaxDefaultCount(string strTag, int nTagCount);

        public string Title
        {
            get { return m_strTitle; }
            set { m_strTitle = value; }
        }

        // 문자열 ID
        public string ComponentID
        {
            get { return m_strComponentID; }
            set
            {
                if (m_strComponentID != value)
                {
                    if (!CheckExist(value))
                    {
                        if (m_showMessageBox)
                            System.Diagnostics.Trace.WriteLine(value + "\r\n이미 존재하는 ID입니다.");
                    }
                    else
                    {
                        string strTag;
                        int nTagCount;

                        if (CheckDefaultStringType(value, out strTag, out nTagCount))
                            AddDefaultID(strTag, nTagCount);
                        else
                        {
                            // 기존 ID가 Default String Type인지 검사한다.
                            if (CheckDefaultStringType(m_strComponentID, out strTag, out nTagCount))
                            {
                                // 기존 ID의 Tag Count가 최대값이면 최대값을 1 낮춰준다.
                                RemoveMaxDefaultCount(strTag, nTagCount);
                            }
                        }

                        ID_LIST[value] = this.m_nID;
                        //ID_LIST.Add(value);
                        m_strComponentID = value;
                    }
                }
            }
        }

        // DB Component ID
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public bool ShowMessageBox
        {
            get { return m_showMessageBox; }
            set { m_showMessageBox = value; }
        }

        private string m_strExprOrigin = "";
        public string ExpressionOrigin
        {
            get { return m_strExprOrigin; }
            set { m_strExprOrigin = value; }
        }

        private string m_strExpr = "";
        public string Expression
        {
            get { return m_strExpr; }
            set { m_strExpr = value; }
        }
        private bool m_bShowExpr = false;
        public bool ShowExpression
        {
            get { return m_bShowExpr; }
            set
            {
                m_bShowExpr = value;
                //m_bShowTempExpr = value;
            }
        }

        private bool m_bShowTempExpr = false;
        public bool ShowTempExpression
        {
            get { return m_bShowTempExpr; }
            set { m_bShowTempExpr = value; }
        }

        public void ResetShowExpression()
        {
            m_bShowTempExpr = m_bShowExpr;
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

    public class SOPTeam
    {
        // 0(평일), 1(휴일), 2(외부 기관), 3(사용자 정의 조직), 4(정규 조직)
        public enum SOPTeamType { None = -1, Normal = 0, Holiday, External, UserDefined, Regular, ControlRoom = 10 };

        private int m_nTeamID = -1;
        private SOPTeamType m_nTeamType = SOPTeamType.Normal;
        private string m_strTeamName = "";
        private int m_nLevelNo = 0;
        // 평일 비상조직 및 휴일 비상 조직과 연결된 조직 또는 개인 List
        // m_arrLinkedMembers의 데이터 Type은 SOPTeam을 사용하는 Client 측에서 각자 정의함
        private ArrayList m_arrLinkedMembers = null;
        // 하위팀 포함 여부
        private bool m_includeChildTeams = false;
        // 평일 비상조직 및 휴일 비상 조직과 연결된 상시 조직 ID List들
        // 외부 기관과 사용자 정의 조직은 이 값이 null
        //private ArrayList m_arrRegularTeamIDList = null;
        // m_isRegular이 true일 경우 TemporaryNormalTeam의 ID
        //              false일 경우 TemporaryEmergencyTeam의 ID
        //private bool m_isRegular = true;

        public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        public SOPTeamType TeamType
        {
            get { return m_nTeamType; }
            set { m_nTeamType = value; }
        }

        public int LevelNo
        {
            get { return m_nLevelNo; }
            set { m_nLevelNo = value; }
        }

        //public bool IsRegular
        //{
        //    get { return m_isRegular; }
        //    set { m_isRegular = value; }
        //}

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        // 평일 비상조직 및 휴일 비상 조직과 연결된 조직 또는 개인 List
        // m_arrLinkedMembers의 데이터 Type은 SOPTeam을 사용하는 Client 측에서 각자 정의함
        public ArrayList LinkedMembers
        {
            get { return m_arrLinkedMembers; }
            set { m_arrLinkedMembers = value; }
        }
        /*public ArrayList RegularTeamIDList
        {
            get { return m_arrRegularTeamIDList; }
            set { m_arrRegularTeamIDList = value; }
        }*/

        // 하위팀 포함 여부
        public bool IncludeChildTeams
        {
            get { return m_includeChildTeams; }
            set { m_includeChildTeams = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is SOPTeam)
            {
                SOPTeam team = (SOPTeam)obj;

                if (this.m_nTeamType == team.m_nTeamType && this.m_nTeamID == team.m_nTeamID)
                    return true;
            }

            return false;
        }
    }

    public class SectionDataProcess : SectionData
    {
        protected bool m_useProcessingTime = false;
        protected ProcessingTime m_processingTime = new ProcessingTime();
        protected ArrayList m_arrMissionItems = new ArrayList();
        protected ArrayList m_arrCheckedItems = new ArrayList();
        protected ArrayList m_arrTeamList = new ArrayList();
        // 임무메시지 전달 여부
        protected bool m_useMissionTransfer = false;
        // 임무메시지를 팀장에게만 보낼 것인가?
        // m_useMissionTransfer가 true일 경우만 사용됨
        protected bool m_transferTeamLeaderOnly = false;

        // Default 문자열을 사용하여 작성된 ID 개수
        protected static Dictionary<string, int> DEFAULT_ID_COUNT = new Dictionary<string, int>();

        protected SectionCommander m_commander = new SectionCommander();

        // 자동실행 여부
        protected bool m_autoRun = false;

        public static void ClearIDCount()
        {
            DEFAULT_ID_COUNT.Clear();
        }

        public override void SetDefaultID(string strStepName, string strTeamName)
        {
            MakeDefaultID(strStepName, strTeamName, DEFAULT_ID_COUNT, "Process");
            /*string strTag = strStepName + "_" + strTeamName + "_Process";

            if (DEFAULT_ID_COUNT.ContainsKey(strTag))
            {
                int nTagCount = DEFAULT_ID_COUNT[strTag];

                m_strComponentID = string.Format("{0}_{1}", strTag, nTagCount + 1);
                DEFAULT_ID_COUNT[strTag] = nTagCount + 1;
            }
            else
            {
                m_strComponentID = strTag + "_1";
                DEFAULT_ID_COUNT[strTag] = 1;
            }

            ID_LIST.Add(m_strComponentID);*/
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

        public bool UseProcessingTime
        {
            get { return m_useProcessingTime; }
            set { m_useProcessingTime = value; }
        }

        public ProcessingTime ProcessingTime
        {
            get { return m_processingTime; }
            set { m_processingTime = value; }
        }

        public ArrayList MissionItems
        {
            get { return m_arrMissionItems; }
        }

        public ArrayList CheckedItems
        {
            get { return m_arrCheckedItems; }
        }

        public ArrayList TeamList
        {
            get { return m_arrTeamList; }
        }

        // 임무메시지 전달 여부
        public bool MissionTransfer
        {
            get { return m_useMissionTransfer; }
            set { m_useMissionTransfer = value; }
        }

        // 임무메시지를 팀장에게만 보낼 것인가?
        // m_useMissionTransfer가 true일 경우만 사용됨
        public bool TransferTeamLeaderOnly
        {
            get { return m_transferTeamLeaderOnly; }
            set { m_transferTeamLeaderOnly = value; }
        }

        public SectionCommander Commander
        {
            get { return m_commander; }
            set { m_commander = value; }
        }

        public bool AutoRun
        {
            get { return m_autoRun; }
            set { m_autoRun = value; }
        }
    }

    public class ProcessingTime
    {
        public enum Type { MONTH, WEEK, DAY, HOUR, MINUTE, UNKNOWN }

        private Type m_type = Type.UNKNOWN;
        private int m_nTime = 0;

        public static bool IntToType(int nType, ref Type type)
        {
            switch (nType)
            {
                case 0:
                    type = Type.MONTH;
                    return true;

                case 1:
                    type = Type.WEEK;
                    return true;

                case 2:
                    type = Type.DAY;
                    return true;

                case 3:
                    type = Type.HOUR;
                    return true;

                case 4:
                    type = Type.MINUTE;
                    return true;

                case 5:
                    type = Type.UNKNOWN;
                    return true;

                default:
                    type = Type.UNKNOWN;
                    break;
            }

            return false;
        }

        public Type ProcessingType
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public int Time
        {
            get { return m_nTime; }
            set { m_nTime = value; }
        }
    }

    public class SectionCommander
    {
        // m_team이 NULL이면 SOP 제어권을 가진 계정의 최고위 명령권자가 되는데
        // 이는 SOPGenUserCommander에 표기되어 있다.
        private Models.SOPTeam m_team = null;
        // Commander가 팀의 수장이 아닌 일반 팀원인가?
        private bool m_isTeamMember = false;
        // Commander가 일반 팀원일 경우 해당 팀원의 ID
        // 해당 팀원의 팀은 m_team이며, 팀 Type은 m_team의 Type을 따른다.
        private int m_nTeamMemberID = -1;
        private string m_strDisplayText = "SOP 제어권 가진곳의 책임자";
        // 발신자 번호
        // 이 값이 null이거나 빈 문자열이면 Commander의 휴대전화번호를 사용한다.
        private string m_strCallerPhoneNumber = null;

        // m_team이 NULL이면 SOP 제어권을 가진 계정의 최고위 명령권자가 되는데
        // 이는 SOPGenUserCommander에 표기되어 있다.
        public Models.SOPTeam Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        // Commander가 팀의 수장이 아닌 일반 팀원인가?
        public bool IsTeamMember
        {
            get { return m_isTeamMember; }
            set { m_isTeamMember = value; }
        }

        // Commander가 일반 팀원일 경우 해당 팀원의 ID
        // 해당 팀원의 팀은 m_team이며, 팀 Type은 m_team의 Type을 따른다.
        public int TeamMemberID
        {
            get { return m_nTeamMemberID; }
            set { m_nTeamMemberID = value; }
        }

        public string DisplayText
        {
            get { return m_strDisplayText; }
            set
            {
                if (value != null)
                    m_strDisplayText = value;
            }
        }

        // 발신자 번호
        // 이 값이 null이거나 빈 문자열이면 Commander의 휴대전화번호를 사용한다.
        public string CallerPhoneNumber
        {
            get { return m_strCallerPhoneNumber; }
            set { m_strCallerPhoneNumber = value; }
        }

        public override string ToString()
        {
            return m_strDisplayText;
        }

        public virtual SectionCommander Clone()
        {
            SectionCommander commander = new SectionCommander();

            commander.m_team = this.m_team;
            commander.m_isTeamMember = this.m_isTeamMember;
            commander.m_nTeamMemberID = this.m_nTeamMemberID;
            commander.m_strDisplayText = this.m_strDisplayText;
            commander.m_strCallerPhoneNumber = this.m_strCallerPhoneNumber;

            return commander;
        }
    }

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