using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Sections
{
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

    public class CheckedItem
    {
        private string m_strCategory = "";
        private string m_strSubCategory = "";
        private string m_strItem = "";
        private int m_nItemCount = -1;
        private string m_strLocation = "";

        public string Category
        {
            get { return m_strCategory; }
            set { m_strCategory = value; }
        }

        public string SubCategory
        {
            get { return m_strSubCategory; }
            set { m_strSubCategory = value; }
        }

        public string Item
        {
            get { return m_strItem; }
            set { m_strItem = value; }
        }

        public int ItemCount
        {
            get { return m_nItemCount; }
            set { m_nItemCount = value; }
        }

        public string Location
        {
            get { return m_strLocation; }
            set { m_strLocation = value; }
        }
    }

    public class MissionItem
    {
        // 0 : 내부 상황전파 1: 외부 상황전파
        //private int m_nTransmission;

        // 0 : 구두 , 1 : 전화, 2 : 무전기, 3 : 기타
        private int m_nTransmissionType = 2;
        
        private string m_strMission;
        private ArrayList m_arrCheckItem = null;
        private bool bCheck = true;
        private string m_strTarget = "";

        private SectionCommander m_strTeamList = null;
        public SectionCommander Commander
        {
            get { return m_strTeamList; }
            set { m_strTeamList = value; }
        }

        public string Target
        {
            get { return m_strTarget; }
            set { m_strTarget = value; }
        }
        /*public int Transmission
        {
            get { return m_nTransmission; }
            set { m_nTransmission = value; }
        }*/
        public int TransmissionType
        {
            get { return m_nTransmissionType; }
            set { m_nTransmissionType = value; }
        }
        public string Mission
        {
            get { return m_strMission; }
            set { m_strMission = value; }
        }

        public ArrayList ArrCheckItem
        {
            get { return m_arrCheckItem; }
            set { m_arrCheckItem = value; }
        }

        public bool CheckItem
        {
            get { return bCheck; }
            set { bCheck = value; }
        }
    }

    // 비상 조직
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

    public class SOPMember
    {
        private int m_nMemberID;
        private string m_strMemberName;
        private int m_nLevelID;

        public int MemberID
        {
            get { return m_nMemberID; }
            set { m_nMemberID = value; }
        }
        public string MemberName
        {
            get { return m_strMemberName; }
            set { m_strMemberName = value; }
        }
        public int LevelID
        {
            get { return m_nLevelID; }
            set { m_nLevelID = value; }
        }
    }

    // Section의 발신자
    public class SectionCommander
    {
        // m_team이 NULL이면 SOP 제어권을 가진 계정의 최고위 명령권자가 되는데
        // 이는 SOPGenUserCommander에 표기되어 있다.
        private Sections.SOPTeam m_team = null;
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
        public Sections.SOPTeam Team
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
                if( value != null)
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
}
