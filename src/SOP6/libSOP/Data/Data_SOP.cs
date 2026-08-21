using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DBUtility2;

namespace UnE
{
    namespace SOP
    {
        // SOP 자동 종료 옵션을 위한 class
        public class SOPCloseOption
        {
            public string CategroyName { get; set; }

            // 입력대기로 인한 자동종료 사용할 것인가?
            public bool UseCloseSOPWaitInputTime { get; set; }

            // 입력대기로 인한 자동종료 대기시간(분)
            public int CloseSOPWaitInputTime { get; set; }

            // 센서신호 복구시 즉시 종료할 것인가?
            public bool UseCloseSOPSensorReset { get; set; }

            // 센서신호 복구시 일정시간 이후 종료할 것인가?
            public bool UseCloseSOPSensorResetWaitTime { get; set; }

            // 센서신호 복구시 일정시간 이후 종료할 때 지연시간(분)
            public int CloseSOPSensorResetWaitTime { get; set; }

            public override string ToString()
            {
                return CategroyName;
            }
        }

        public class BroadcastMessage
        {
            protected int mID;
            public int ID
            {
                get { return mID; }
                set { mID = value; }
            }
            protected string message;
            public string Message
            {
                get { return message; }
                set { message = value; }
            }
            protected bool bUseSiren;
            public bool UseSiren
            {
                get { return bUseSiren; }
                set { bUseSiren = value; }
            }
            protected int mplayOption;
            public int PlayOption
            {
                get { return mplayOption; }
                set { mplayOption = value; }
            }
            protected int mRepeatCount;
            public int RepeatCount
            {
                get { return mRepeatCount; }
                set { mRepeatCount = value; }
            }

            protected DateTime mAddedTime;
            public System.DateTime AddTime
            {
                get { return mAddedTime; }
                set { mAddedTime = value; }
            }
        }

        public class Data_CompanyMember : IComparable
        {
            private int m_nID;
            private string m_strMemberName = "";
            //private int m_nRegularTeamID;
            //private int m_nTemporaryTeamID;
            private int m_nLevelID;
            //private int m_nPositionID;
            //private int m_nTemporaryPositionID;
            private string m_strMemberID = "";
            private string m_strPhoneNumber = "";
            private string m_strOfficePhoneNumber = "";
            // 팀별 직위
            private Dictionary<Data_RegularTeam, int> m_dicTeamPositions = new Dictionary<Data_RegularTeam, int>();

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string MemberName
            {
                get { return m_strMemberName; }
                set { m_strMemberName = value; }
            }

            /*public int RegularTeamID
            {
                get { return m_nRegularTeamID; }
                set { m_nRegularTeamID = value; }
            }*/

            public int LevelID
            {
                get { return m_nLevelID; }
                set { m_nLevelID = value; }
            }

            /*public int PositionID
            {
                get { return m_nPositionID; }
                set { m_nPositionID = value; }
            }*/

            public string MemberID
            {
                get { return m_strMemberID; }
                set { m_strMemberID = value; }
            }

            /*public int SecondRegularTeamID
            {
                get { return m_nTemporaryTeamID; }
                set { m_nTemporaryTeamID = value; }
            }

            public int SecondPositionID
            {
                get { return m_nTemporaryPositionID; }
                set { m_nTemporaryPositionID = value; }
            }*/

            public string PhoneNumber
            {
                get { return m_strPhoneNumber; }
                set { m_strPhoneNumber = value; }
            }

            public string OfficePhoneNumber
            {
                get { return m_strOfficePhoneNumber; }
                set { m_strOfficePhoneNumber = value; }
            }

            // 팀별 직위
            public Dictionary<Data_RegularTeam, int> TeamPositions
            {
                get { return m_dicTeamPositions; }
            }

            public bool ContainsTeam(int nTeamID)
            {
                foreach (KeyValuePair<Data_RegularTeam, int> pair in m_dicTeamPositions)
                {
                    if (pair.Key.ID == nTeamID)
                        return true;
                }

                return false;
            }

            // 직위에 따라 정렬한다.
            public int CompareTo(object obj)
            {
                Data_CompanyMember member = (Data_CompanyMember)obj;

                if (member == null)
                    return 1;

                foreach (KeyValuePair<Data_RegularTeam, int> pair in this.TeamPositions)
                {
                    int nJobPosition;

                    if (member.TeamPositions.TryGetValue(pair.Key, out nJobPosition))
                    {
                        return ControlTeamEditor.JobPosition.CompareJobPosition(pair.Value, nJobPosition);
                    }
                }

                if (this.TeamPositions.Count > 0 && member.TeamPositions.Count > 0)
                    return ControlTeamEditor.JobPosition.CompareJobPosition(this.TeamPositions.ElementAt(0).Value, member.TeamPositions.ElementAt(0).Value);
                else if (this.TeamPositions.Count > 0)
                    return 1;
                else if (member.TeamPositions.Count > 0)
                    return -1;

                return 0;
            }
        }

        public class Data_SOPGenUser
        {
            private int m_nID = -1;
            private int m_nMemberID = -1;
            private string m_strUserName = "";
            private int m_nUserLevel = -1;
            private int m_nTeamID = -1;
            private string m_strUserID = "";
            private string m_strNickName = "";
            private global::Sections.SectionCommander m_commanderDayLight = null;
            private global::Sections.SectionCommander m_commanderNight = null;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public int MemberID
            {
                get { return m_nMemberID; }
                set { m_nMemberID = value; }
            }

            public string UserName
            {
                get { return m_strUserName; }
                set { m_strUserName = value; }
            }

            public int UserLevel
            {
                get { return m_nUserLevel; }
                set { m_nUserLevel = value; }
            }

            public int TeamID
            {
                get { return m_nTeamID; }
                set { m_nTeamID = value; }
            }

            public string UserID
            {
                get { return m_strUserID; }
                set { m_strUserID = value; }
            }

            public string NickName
            {
                get { return m_strNickName; }
                set { m_strNickName = value; }
            }

            public global::Sections.SectionCommander DayLightCommander
            {
                get { return m_commanderDayLight; }
                set { m_commanderDayLight = value; }
            }

            public global::Sections.SectionCommander NightCommander
            {
                get { return m_commanderNight; }
                set { m_commanderNight = value; }
            }
        }

        public class Data_DisasterCategory
        {
            private int m_nID;
            private string m_strCategoryName;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string CategoryName
            {
                get { return m_strCategoryName; }
                set { m_strCategoryName = value; }
            }

            public override string ToString()
            {
                return m_strCategoryName;
            }
        }

        public class Data_SubDisasterCategory
        {
            private int m_nID;
            private int m_nDisasterID;
            private string m_strCategoryName;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }
            public int DisasterID
            {
                get { return m_nDisasterID; }
                set { m_nDisasterID = value; }
            }
            public string CategoryName
            {
                get { return m_strCategoryName; }
                set { m_strCategoryName = value; }
            }
        }

        public class Data_Disaster
        {
            private int m_nID;
            private string m_strDisasterName;
            private int m_nSubDisasterID;
            private int m_nVersionID;
            private string m_strDescription;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }
            public string DisasterName
            {
                get { return m_strDisasterName; }
                set { m_strDisasterName = value; }
            }
            public int SubDisasterID
            {
                get { return m_nSubDisasterID; }
                set { m_nSubDisasterID = value; }
            }
            public int VersionID
            {
                get { return m_nVersionID; }
                set { m_nVersionID = value; }
            }
            public string Description
            {
                get { return m_strDescription; }
                set { m_strDescription = value; }
            }
        }

        public class Data_RegularTeam
        {
            private int m_nID;
            private string m_strTeamName;
            private int m_nParentTeamID;
            private List<Data_RegularTeam> m_childTeams = new List<Data_RegularTeam>();
            private object oTag = null;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string TeamName
            {
                get { return m_strTeamName; }
                set { m_strTeamName = value; }
            }

            public int ParentTeamID
            {
                get { return m_nParentTeamID; }
                set { m_nParentTeamID = value; }
            }

            public List<Data_RegularTeam> ChildTeams
            {
                get { return m_childTeams; }
            }

            public object Tag
            {
                get { return oTag; }
                set { oTag = value; }
            }

            public Data_RegularTeam Clone()
            {
                Data_RegularTeam team = new Data_RegularTeam();

                team.m_nID = this.m_nID;
                team.m_strTeamName = this.m_strTeamName;
                team.m_nParentTeamID = this.m_nParentTeamID;
                team.m_childTeams = this.m_childTeams;

                return team;
            }
        }

        public class Data_ControlRoom
        {
            public const string ROOT_NAME = "교대 근무자";
            public const int ROOT_ID = 0;

            private int m_nID;
            private string m_strTeamName;
            private int m_nParentTeamID;
            private List<Data_ControlRoom> m_childTeams = new List<Data_ControlRoom>();
            private object oTag = null;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string TeamName
            {
                get { return m_strTeamName; }
                set { m_strTeamName = value; }
            }

            public int ParentTeamID
            {
                get { return m_nParentTeamID; }
                set { m_nParentTeamID = value; }
            }

            public List<Data_ControlRoom> ChildTeams
            {
                get { return m_childTeams; }
            }

            public object Tag
            {
                get { return oTag; }
                set { oTag = value; }
            }

            public Data_ControlRoom Clone()
            {
                Data_ControlRoom team = new Data_ControlRoom();

                team.m_nID = this.m_nID;
                team.m_strTeamName = this.m_strTeamName;
                team.m_nParentTeamID = this.m_nParentTeamID;
                team.m_childTeams = this.m_childTeams;

                return team;
            }

            public static int MakeID(int nRoomTypeID, int nControlRoomID, int nControlTeamJobPositionID)
            {
                int nID = (nControlTeamJobPositionID << 16) | (nControlRoomID << 8) | nRoomTypeID;
                return nID;
            }
        }
        public class Data_ControlRoomMember
        {
            private int m_nMemberID;
            private string m_strMemberName;
            private string m_strPhoneNumber;
            private int m_nRoomID;
            private int m_nTeamID;
            private int m_nJobPosition;
            private int m_nMemberType;
            private int m_nRoomType;  

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

            public string PhoneNumber
            {
                get { return m_strPhoneNumber; }
                set { m_strPhoneNumber = value; }
            }

            public int RoomID
            {
                get { return m_nRoomID; }
                set { m_nRoomID = value; }
            }
            public int TeamID
            {
                get { return m_nTeamID; }
                set { m_nTeamID = value; }
            }
            public int JobPosition
            {
                get { return m_nJobPosition; }
                set { m_nJobPosition = value; }
            }
            public int MemberType
            {
                get { return m_nMemberType; }
                set { m_nMemberType = value; }
            }
            public int RoomType
            {
                get { return m_nRoomType; }
                set { m_nRoomType = value; }
            }  

            public static int MakeID(int nRoomType, int nRoomID, int nJobPosition)
            {
                int nID = (nJobPosition << 16) | (nRoomID << 8) | nRoomType;
                return nID;
            }
        }

        public class Data_SearchMember
        {
            private int m_nMemberID;
            private string m_strMemberName;
            private int m_nTeamID;
            private string m_strTeamName;
            private string m_strFullPathName;

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

            public string FullPathName
            {
                get { return m_strFullPathName; }
                set { m_strFullPathName = value; }
            }
        }

        public class Data_Task
        {
            private int m_nID;
            private int m_nStepMemberID;
            private string m_strTaskCategory;
            private string m_strTaskName;
            private string m_strDescription;

            public int TaskID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public int StepMemberID
            {
                get { return m_nStepMemberID; }
                set { m_nStepMemberID = value; }
            }

            public string TaskCategory
            {
                get { return m_strTaskCategory; }
                set { m_strTaskCategory = value; }
            }

            public string TaskName
            {
                get { return m_strTaskName; }
                set { m_strTaskName = value; }
            }

            public string Description
            {
                get { return m_strDescription; }
                set { m_strDescription = value; }
            }
        }

        public class Data_Building
        {
            private int m_nID = -1;
            private string m_strBuildingCode = "";
            private string m_strBuildingName = "";
            private string m_strBuildingID = "";
            private Data_BuildingGroup m_buildingGroup = null;
            private int m_nMaxFloorIndex = -1;
            private int m_nMinFloorIndex = -1;
            private string m_strBroadCastingText = null;

            public Data_Building()
            {
            }

            public Data_Building(int nID, string strBuildingID, string strBuildingCode, string strBuildingName, Data_BuildingGroup buildingGroup, int nMaxFloorIndex, int nMinFloorIndex, string strBroadCastingText)
            {
                m_nID = nID;
                m_strBuildingID = strBuildingID;
                m_strBuildingCode = strBuildingCode;
                m_strBuildingName = strBuildingName;
                m_buildingGroup = buildingGroup;
                m_nMaxFloorIndex = nMaxFloorIndex;
                m_nMinFloorIndex = nMinFloorIndex;
                m_strBroadCastingText = strBroadCastingText;
            }

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string BuildingID
            {
                get { return m_strBuildingID; }
                set { m_strBuildingID = value; }
            }

            public string BuildingCode
            {
                get { return m_strBuildingCode; }
                set { m_strBuildingCode = value; }
            }

            public string BuildingName
            {
                get { return m_strBuildingName; }
                set { m_strBuildingName = value; }
            }

            public Data_BuildingGroup BuildingGroup
            {
                get { return m_buildingGroup; }
                set { m_buildingGroup = value; }
            }

            public int MaxFloor
            {
                get { return m_nMaxFloorIndex; }
                set { m_nMaxFloorIndex = value; }
            }

            public int MinFloor
            {
                get { return m_nMinFloorIndex; }
                set { m_nMinFloorIndex = value; }
            }

            public string BroadCastingText
            {
                get { return m_strBroadCastingText == null ? m_strBuildingName : m_strBroadCastingText; }
                set { m_strBroadCastingText = value; }
            }
        }

        public class Data_BuildingGroup
        {
            private int m_nID = -1;
            private string m_strGroupName = "";
            private int m_nSiteID = -1;
            private string m_strSiteName = "";

            public Data_BuildingGroup()
            {
            }

            public Data_BuildingGroup(int nID, string strGroupName, int nSiteID, string strSiteName)
            {
                m_nID = nID;
                m_strGroupName = strGroupName;
                m_nSiteID = nSiteID;
                m_strSiteName = strSiteName;
            }

            public int GroupID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string GroupName
            {
                get { return m_strGroupName; }
                set { m_strGroupName = value; }
            }

            public int SiteID
            {
                get { return m_nSiteID; }
                set { m_nSiteID = value; }
            }

            public string SiteName
            {
                get { return m_strSiteName; }
                set { m_strSiteName = value; }
            }

        }

        public class Data_Site
        {
            private int m_nID;
            private string m_strSiteName;

            public int SiteID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string SiteName
            {
                get { return m_strSiteName; }
                set { m_strSiteName = value; }
            }

        }

        public class Data_EquipmentInfo
        {
            private string m_strEquipID;
            private int m_nZoneID;
            private string m_strZoneName;
            private int m_nFloorIndex;
            private int m_nBuildingID;
            private string m_strBuildingName;
            private int m_nGroupID;
            private string m_strGroupName;
            private string m_strSiteName;
            private int m_nMaxFloor;
            private int m_nMinFloor;

            public string EquipID
            {
                get { return m_strEquipID; }
                set { m_strEquipID = value; }
            }

            public int ZoneID
            {
                get { return m_nZoneID; }
                set { m_nZoneID = value; }
            }

            public string ZoneName
            {
                get { return m_strZoneName; }
                set { m_strZoneName = value; }
            }

            public int FloorIndex
            {
                get { return m_nFloorIndex; }
                set { m_nFloorIndex = value; }
            }

            public int BuildingID
            {
                get { return m_nBuildingID; }
                set { m_nBuildingID = value; }
            }

            public string BuildingName
            {
                get { return m_strBuildingName; }
                set { m_strBuildingName = value; }
            }

            public int GroupID
            {
                get { return m_nGroupID; }
                set { m_nGroupID = value; }
            }

            public string GroupName
            {
                get { return m_strGroupName; }
                set { m_strGroupName = value; }
            }

            public string SiteName
            {
                get { return m_strSiteName; }
                set { m_strSiteName = value; }
            }

            public int MaxFloor
            {
                get { return m_nMaxFloor; }
                set { m_nMaxFloor = value; }
            }

            public int MinFloor
            {
                get { return m_nMinFloor; }
                set { m_nMinFloor = value; }
            }
        }

        public class Data_NormalTeam
        {
            private int m_nID = -1;
            private string m_strTeamName = "";
            private Data_NormalTeam m_parentTeam = null;
            private List<Data_NormalTeam> m_childTeams = new List<Data_NormalTeam>();
            private string m_strGroupName = "";
            private int m_nLevelNo = -1;
            private string m_strDescription = "";
            //private string m_strRegularTeamLink;
            private object oTag = null;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string TeamName
            {
                get { return m_strTeamName; }
                set { m_strTeamName = value; }
            }
            
            public Data_NormalTeam ParentTeam
            {
                get { return m_parentTeam; }
                set { m_parentTeam = value; }
            }

            public List<Data_NormalTeam> ChildTeams
            {
                get { return m_childTeams; }
            }
            
            public string GroupName
            {
                get { return m_strGroupName; }
                set { m_strGroupName = value; }
            }
            
            public int LevelNo
            {
                get { return m_nLevelNo; }
                set { m_nLevelNo = value; }
            }
            
            public string Description
            {
                get { return m_strDescription; }
                set { m_strDescription = value; }
            }
            /*public string RegularTeamLink
            {
                get { return m_strRegularTeamLink; }
                set { m_strRegularTeamLink = value; }
            }*/

            public object Tag
            {
                get { return oTag; }
                set { oTag = value; }
            }

            public Data_NormalTeam Clone()
            {
                Data_NormalTeam team = new Data_NormalTeam();

                team.m_nID = this.m_nID;
                team.m_strTeamName = this.m_strTeamName;
                team.m_parentTeam = this.m_parentTeam;
                team.m_strGroupName = this.m_strGroupName;
                team.m_nLevelNo = this.m_nLevelNo;
                team.m_strDescription = this.m_strDescription;

                team.ChildTeams.Clear();
                team.ChildTeams.AddRange(this.ChildTeams);

                return team;
            }
        }

        public class Data_EmergencyTeam
        {
            private int m_nID = -1;
            private string m_strTeamName = "";
            private Data_EmergencyTeam m_parentTeam = null;
            private List<Data_EmergencyTeam> m_childTeams = new List<Data_EmergencyTeam>();
            private string m_strGroupName = "";
            private int m_nLevelNo = -1;
            private string m_strDescription = "";
            //private string m_strRegularTeamLink;
            private object oTag = null;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string TeamName
            {
                get { return m_strTeamName; }
                set { m_strTeamName = value; }
            }
            
            public Data_EmergencyTeam ParentTeam
            {
                get { return m_parentTeam; }
                set { m_parentTeam = value; }
            }

            public List<Data_EmergencyTeam> ChildTeams
            {
                get { return m_childTeams; }
            }
            
            public string GroupName
            {
                get { return m_strGroupName; }
                set { m_strGroupName = value; }
            }
            
            public int LevelNo
            {
                get { return m_nLevelNo; }
                set { m_nLevelNo = value; }
            }
            
            public string Description
            {
                get { return m_strDescription; }
                set { m_strDescription = value; }
            }
            /*public string RegularTeamLink
            {
                get { return m_strRegularTeamLink; }
                set { m_strRegularTeamLink = value; }
            }*/

            public object Tag
            {
                get { return oTag; }
                set { oTag = value; }
            }

            public Data_EmergencyTeam Clone()
            {
                Data_EmergencyTeam team = new Data_EmergencyTeam();

                team.m_nID = this.m_nID;
                team.m_strTeamName = this.m_strTeamName;
                team.m_parentTeam = this.m_parentTeam;
                team.m_strGroupName = this.m_strGroupName;
                team.m_nLevelNo = this.m_nLevelNo;
                team.m_strDescription = this.m_strDescription;

                team.ChildTeams.Clear();
                team.ChildTeams.AddRange(this.ChildTeams);

                return team;
            }
        }

        public class Data_CheckTask
        {
            private int m_nID;
            private int m_nProcessID;
            private string m_strCategory;
            private string m_strSubCategory;
            private string m_strTaskName;
            private int m_nTargetCount;
            private string m_strPosition;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }
            public int ProcessID
            {
                get { return m_nProcessID; }
                set { m_nProcessID = value; }
            }
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
            public string TaskName
            {
                get { return m_strTaskName; }
                set { m_strTaskName = value; }
            }
            public int TargetCount
            {
                get { return m_nTargetCount; }
                set { m_nTargetCount = value; }
            }
            public string Position
            {
                get { return m_strPosition; }
                set { m_strPosition = value; }
            }
        }

        public class Data_ActionStep
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
            private ConfigData m_userDefinedConfig = null;
            // 평일모드인가?
            private bool m_isNormal = true;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }
            public string StepName
            {
                get { return m_strStepName; }
                set { m_strStepName = value; }
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

            public ConfigData UserDefinedConfig
            {
                get { return m_userDefinedConfig; }
                set { m_userDefinedConfig = value; }
            }

            public bool IsNormal
            {
                get { return m_isNormal; }
                set { m_isNormal = value; }
            }
        }

        public class Data_ActionStepHistory
        {
            private int m_nID = -1;
            private int m_nActionStepID = -1;
            private bool m_isRealMode = false;
            private DateTime m_beginTime;
            private VariousData<DateTime> m_endTime = null;
            private VariousData<DateTime> m_cancelTime = null;
            private VariousData<DateTime> m_PausedTime = null;
            private DateTime m_detectTime;
            private string m_strPosition = null;
            private int m_nSelectedSectionID = -1;
            private int m_nSelectedSectionType = -1;
            private int m_nStartOption = -1;
            private HistoryDisasterNoPosition m_historyNoPositionInfo = null;
            private int m_nSensorZoneHistoryID = -1;
            private List<Data_ComponentHistory> m_componentHistories = new List<Data_ComponentHistory>();
            private bool m_isNormal = true;

            // m_componentHistories 가운데 나중에 생성된 History의 ID
            private int m_nMaxComponentHistoryIDFromServer = -1;
            private int m_nMaxComponentHistoryIDInClient = -1;

            public int SensorZoneHistoryID
            {
                get { return m_nSensorZoneHistoryID; }
                set { m_nSensorZoneHistoryID = value; }
            }
            
            public int StartOption
            {
                get { return m_nStartOption; }
                set { m_nStartOption = value; }
            }

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public int ActionStepID
            {
                get { return m_nActionStepID; }
                set { m_nActionStepID = value; }
            }

            public bool RealMode
            {
                get { return m_isRealMode; }
                set { m_isRealMode = value; }
            }

            public DateTime BeginTime
            {
                get { return m_beginTime; }
                set { m_beginTime = value; }
            }

            public VariousData<DateTime> EndTime
            {
                get { return m_endTime; }
                set { m_endTime = value; }
            }

            public VariousData<DateTime> CancelTime
            {
                get { return m_cancelTime; }
                set { m_cancelTime = value; }
            }

            public VariousData<DateTime> PausedTime
            {
                get { return m_PausedTime; }
                set { m_PausedTime = value; }
            }

            public DateTime DetectTime
            {
                get { return m_detectTime; }
                set { m_detectTime = value; }
            }

            public int SelectedSectionID
            {
                get { return m_nSelectedSectionID; }
                set { m_nSelectedSectionID = value; }
            }

            public int SelectedSectionType
            {
                get { return m_nSelectedSectionType; }
                set { m_nSelectedSectionType = value; }
            }

            public string Position
            {
                get { return m_strPosition; }
                set { m_strPosition = value; }
            }

            public HistoryDisasterNoPosition HistoryDisasterNoPositionInfo
            {
                get { return m_historyNoPositionInfo; }
                set { m_historyNoPositionInfo = value; }
            }

            public List<Data_ComponentHistory> ComponentHistories
            {
                get { return m_componentHistories; }
            }

            // 서버로부터 읽어들인 ComponentHistoryID 중 가장 큰 값
            public int MaxComponentHistoryIDFromServer
            {
                get { return m_nMaxComponentHistoryIDFromServer; }
                set { m_nMaxComponentHistoryIDFromServer = value; }
            }

            // 클라이언트가 사용한 ComponentHistoryID 중 가장 큰 값
            public int MaxComponentHistoryIDInClient
            {
                get { return m_nMaxComponentHistoryIDInClient; }
                set { m_nMaxComponentHistoryIDInClient = value; }
            }

            public bool IsNormal
            {
                get { return m_isNormal; }
                set { m_isNormal = value; }
            }
        }

        public class Data_ComponentHistory : IComparable
        {
            private int m_nID = -1;
            private int m_nComponentID = -1;
            private int m_nComponentType = -1;
            private DateTime m_timeStamp;
            private int m_nStatus = -1;
            // Nullable
            private string m_strTask = null;
            private VariousData<int> m_completeCount = null;
            private VariousData<int> m_showBoard = null;
            private int m_nAccessedUserID = -1;
            private VariousData<int> m_checkedNotify1 = null;
            private VariousData<int> m_checkedNotify2 = null;
            private VariousData<int> m_checkedRun = null;
            private VariousData<int> m_checkedComplete = null;
            private List<Data_ComponentHistoryDetail> m_detailDatas = new List<Data_ComponentHistoryDetail>();

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public int ComponentID
            {
                get { return m_nComponentID; }
                set { m_nComponentID = value; }
            }

            public int ComponentType
            {
                get { return m_nComponentType; }
                set { m_nComponentType = value; }
            }

            public DateTime TimeStamp
            {
                get { return m_timeStamp; }
                set { m_timeStamp = value; }
            }

            public int Status
            {
                get { return m_nStatus; }
                set { m_nStatus = value; }
            }

            // Nullable
            public string Task
            {
                get { return m_strTask; }
                set { m_strTask = value; }
            }

            public VariousData<int> CompleteCount
            {
                get { return m_completeCount; }
                set { m_completeCount = value; }
            }

            public VariousData<int> ShowBoard
            {
                get { return m_showBoard; }
                set { m_showBoard = value; }
            }

            public int AccessedUserID
            {
                get { return m_nAccessedUserID; }
                set { m_nAccessedUserID = value; }
            }

            public VariousData<int> CheckedNotify1
            {
                get { return m_checkedNotify1; }
                set { m_checkedNotify1 = value; }
            }

            public VariousData<int> CheckedNotify2
            {
                get { return m_checkedNotify2; }
                set { m_checkedNotify2 = value; }
            }

            public VariousData<int> CheckedRun
            {
                get { return m_checkedRun; }
                set { m_checkedRun = value; }
            }

            public VariousData<int> CheckedComplete
            {
                get { return m_checkedComplete; }
                set { m_checkedComplete = value; }
            }

            public List<Data_ComponentHistoryDetail> DetailDatas
            {
                get { return m_detailDatas; }
            }

            public int CompareTo(object obj)
            {
                Data_ComponentHistory history = (Data_ComponentHistory)obj;
                return this.ID.CompareTo(history.ID);
            }
        }

        public class Data_ComponentHistoryDetail
        {
            private int m_nDataIndex = -1;
            private VariousData<int> m_datai = null;
            private VariousData<float> m_dataf = null;
            private string m_datas = null;
            private VariousData<DateTime> m_timeStamp = null;

            public int DataIndex
            {
                get { return m_nDataIndex; }
                set { m_nDataIndex = value; }
            }

            public VariousData<int> Datai
            {
                get { return m_datai; }
                set { m_datai = value; }
            }

            public VariousData<float> Dataf
            {
                get { return m_dataf; }
                set { m_dataf = value; }
            }

            public string Datas
            {
                get { return m_datas; }
                set { m_datas = value; }
            }

            public VariousData<DateTime> TimeStamp
            {
                get { return m_timeStamp; }
                set { m_timeStamp = value; }
            }
        }

        public class Data_UserDefinedTeam
        {
            private int m_nID = -1;
            private string m_strTeamName = "";
            private string m_strPhoneNumber = "";
            private string m_strFaxNumber = "";
            private object oTag = null;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
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
            public object Tag
            {
                get { return oTag; }
                set { oTag = value; }
            }

            public Data_UserDefinedTeam()
            {
            }

            public Data_UserDefinedTeam(int nID, string strTeamName, string strPhoneNumber, string strFaxNumber)
            {
                m_nID = nID;
                m_strTeamName = strTeamName;
                m_strPhoneNumber = strPhoneNumber;
                m_strFaxNumber = strFaxNumber;
            }
        }

        public class Data_ExternalTeam
        {
            private int m_nID = -1;
            private string m_strTeamName = "";
            private string m_strPhoneNumber = "";
            private string m_strFaxNumber = "";
            private int m_nParentTeamID = -1;
            private List<Data_ExternalTeam> m_childTeams = new List<Data_ExternalTeam>();

            public Data_ExternalTeam()
            {
            }

            public Data_ExternalTeam(int nID, string strTeamName, string strPhoneNumber, string strFaxNumber)
            {
                m_nID = nID;
                m_strTeamName = strTeamName;
                m_strPhoneNumber = strPhoneNumber;
                m_strFaxNumber = strFaxNumber;
            }

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
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

            private object oTag = null;
            public object Tag
            {
                get { return oTag; }
                set { oTag = value; }
            }

            public int ParentTeamID
            {
                get { return m_nParentTeamID; }
                set { m_nParentTeamID = value; }
            }

            public List<Data_ExternalTeam> ChildTeams
            {
                get { return m_childTeams; }
            }
        }

        // TemporaryMemberList의 데이터를 표현
        public class TemporaryMember
        {
            public enum MemberType
            {
                RegularTeam = 0,
                CompanyMember,
                ExternalCompanyTeam,    // 외부 협력사의 팀
                ExternalTeam,           // 외부 협력사
                ExternalCompanyMember,
                UserDefinedTeam,
                JobLevel,               // 직급, 1이면 1직급, 2면 2직급
                Unknown
            }

            //역할 : 0(정), 1(부), 2(팀장), 3(일반)
            public enum RoleType { Main = 0, Sub, TeamLeader, General, Unknown };

            private int m_nTemporaryTeamID = -1;
            private bool m_isNormal = true;
            private int m_nMemberID = -1;
            // 1이면 팀장, 0이면 팀원이며 0보다 작으면 null 값이다.
            private int m_nTeamLeader = -1;
            private MemberType m_memberType = MemberType.Unknown;
            private RoleType m_roleType = RoleType.Unknown;
            private string m_strMemberName = "";
            // 하위팀을 포함하는가?
            private bool m_includeChildTeams = true;

            public int TemporaryTeamID
            {
                get { return m_nTemporaryTeamID; }
                set { m_nTemporaryTeamID = value; }
            }

            public bool IsNormal
            {
                get { return m_isNormal; }
                set { m_isNormal = value; }
            }

            public int MemberID
            {
                get { return m_nMemberID; }
                set { m_nMemberID = value; }
            }

            // 1이면 팀장, 0이면 팀원이며 0보다 작으면 null 값이다.
            public int TeamLeader
            {
                get { return m_nTeamLeader; }
                set { m_nTeamLeader = value; }
            }

            public MemberType _MemberType
            {
                get { return m_memberType; }
                set { m_memberType = value; }
            }

            public RoleType _RoleType
            {
                get { return m_roleType; }
                set { m_roleType = value; }
            }

            public string MemberName
            {
                get { return m_strMemberName; }
                set { m_strMemberName = value; }
            }

            public bool IncludeChildTeams
            {
                get { return m_includeChildTeams; }
                set { m_includeChildTeams = value; }
            }

            public TemporaryMember()
            {
            }

            public TemporaryMember(int nTemporaryTeamID, bool isNormal, int nMemberID, int nTeamLeader, MemberType memberType, RoleType roleType, string strMemberName)
            {
                m_nTemporaryTeamID = nTemporaryTeamID;
                m_isNormal = isNormal;
                m_nMemberID = nMemberID;
                m_nTeamLeader = nTeamLeader;
                m_memberType = memberType;
                m_roleType = roleType;
                m_strMemberName = strMemberName;
            }

            public static bool GetMemberType(int nMemberType, out MemberType memberType)
            {
                if (nMemberType < 0 || nMemberType >= (int)MemberType.Unknown)
                {
                    memberType = MemberType.Unknown;
                    return false;
                }

                memberType = (MemberType)nMemberType;
                return true;
            }

            public static bool GetRoleType(int nRoleType, out RoleType roleType)
            {
                if (nRoleType < 0 || nRoleType >= (int)RoleType.Unknown)
                {
                    roleType = RoleType.Unknown;
                    return false;
                }

                roleType = (RoleType)nRoleType;
                return true;
            }

            public static string GetRoleTypeString(RoleType roleType)
            {
                if (roleType == RoleType.Main)
                    return "정";
                else if (roleType == RoleType.Sub)
                    return "부";

                return "";
            }
        }

        public class Data_Version
        {
            private int m_nID;
            private int m_nRegular;
            private int m_nNormal;
            private DateTime m_dtCreateTime;
            private DateTime m_dtLastAccessTime;
            private string m_strVersionName;
            private int m_nOwnerID;
            private string m_strDescription;


            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }
            public int Regular
            {
                get { return m_nRegular; }
                set { m_nRegular = value; }
            }
            public int Normal
            {
                get { return m_nNormal; }
                set { m_nNormal = value; }
            }
            public DateTime CreateTime
            {
                get { return m_dtCreateTime; }
                set { m_dtCreateTime = value; }
            }
            public DateTime LastAccessTime
            {
                get { return m_dtLastAccessTime; }
                set { m_dtLastAccessTime = value; }
            }
            public string VersionName
            {
                get { return m_strVersionName; }
                set { m_strVersionName = value; }
            }
            public int OwnerID
            {
                get { return m_nOwnerID; }
                set { m_nOwnerID = value; }
            }
            public string Description
            {
                get { return m_strDescription; }
                set { m_strDescription = value; }
            }
        }

        public class DisasterInfo : IComparable
        {
            private string m_strDisasterName = "";
            private string m_strSubDisasterCategoryName = "";
            private string m_strDisasterCategoryName = "";
            private int m_nDisasterID = -1;
            private int m_nVersionID = -1;
            private ArrayList m_arrActionSteps = new ArrayList();

            public ActionStepInfo FindActionStep(int nActionStepID)
            {
                foreach (ActionStepInfo actionStep in m_arrActionSteps)
                {
                    if (actionStep.ActionStepID == nActionStepID)
                        return actionStep;
                }

                return null;
            }

            public int DisasterID
            {
                get { return m_nDisasterID; }
                set { m_nDisasterID = value; }
            }

            public int VersionID
            {
                get { return m_nVersionID; }
                set { m_nVersionID = value; }
            }

            public ArrayList ActionSteps
            {
                get { return m_arrActionSteps; }
            }

            public string DisasterName
            {
                get { return m_strDisasterName; }
                set { m_strDisasterName = value; }
            }

            public string SubDisasterCategoryName
            {
                get { return m_strSubDisasterCategoryName; }
                set { m_strSubDisasterCategoryName = value; }
            }

            public string DisasterCategoryName
            {
                get { return m_strDisasterCategoryName; }
                set { m_strDisasterCategoryName = value; }
            }

            public int CompareTo(object obj)
            {
                DisasterInfo disaster = (DisasterInfo)obj;

                if (this.m_nDisasterID > disaster.m_nDisasterID)
                    return 1;
                else if (this.m_nDisasterID < disaster.m_nDisasterID)
                    return -1;
                //else
                return 0;
            }
        }

        public class ActionStepInfo
        {
            private int m_nActionStepID = -1;
            private string m_strActionStepName = "";
            private int m_nParentStepID = -1;
            private int m_nPeriodType = -1;
            private DateTime m_timeBegin;
            private DateTime m_timeEnd;
            private int m_nWeekdayOption = 127;
            private int m_nIteration = 1;
            private int m_nIterationType = 0;
            private int m_nProcessTime = 1;
            private int m_nProcessTimeType = 5;
            private int m_nDisasterID = -1;
            // 평일모드인가?
            private bool m_isNormal = true;

            public Data_ActionStep ToData_ActionStep()
            {
                Data_ActionStep data = new Data_ActionStep();

                data.BeginTime = m_timeBegin;
                data.DisasterID = m_nDisasterID;
                data.EndTime = m_timeEnd;
                data.ID = m_nActionStepID;
                data.Iteration = m_nIteration;
                data.IterationType = m_nIterationType;
                data.ParentStepID = m_nParentStepID;
                data.PeriodType = m_nPeriodType;
                data.ProcessTime = m_nProcessTime;
                data.ProcessTimeType = m_nProcessTimeType;
                data.StepName = m_strActionStepName;
                data.WeekdayOption = m_nWeekdayOption;
                data.IsNormal = m_isNormal;

                return data;
            }

            public int ActionStepID
            {
                get { return m_nActionStepID; }
                set { m_nActionStepID = value; }
            }

            public string ActionStepName
            {
                get { return m_strActionStepName; }
                set { m_strActionStepName = value; }
            }

            public int ParentStepID
            {
                get { return m_nParentStepID; }
                set { m_nParentStepID = value; }
            }

            public int PeriodType
            {
                get { return m_nPeriodType; }
                set { m_nPeriodType = value; }
            }

            public DateTime BeginTime
            {
                get { return m_timeBegin; }
                set { m_timeBegin = value; }
            }

            public DateTime EndTime
            {
                get { return m_timeEnd; }
                set { m_timeEnd = value; }
            }

            public int WeekDayOption
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

            public bool IsNormal
            {
                get { return m_isNormal; }
                set { m_isNormal = value; }
            }
        }

        public class VersionInfo
        {
            private int m_nVersionID = -1;
            private string m_strVersionName = "";
            private string m_strUserName = "";
            private DateTime m_dtBegin;
            private DateTime m_dtLastAccessed;
            private string m_strDescription = "";
            private bool m_isRegular = true;    // 등록 모드인가?
            private bool m_isNormal = true;     // 평일 버전인가?

            public int VersionID
            {
                get { return m_nVersionID; }
                set { m_nVersionID = value; }
            }

            public string VersionName
            {
                get { return m_strVersionName; }
                set { m_strVersionName = value; }
            }

            public string UserName
            {
                get { return m_strUserName; }
                set { m_strUserName = value; }
            }

            public DateTime BeginTime
            {
                get { return m_dtBegin; }
                set { m_dtBegin = value; }
            }

            public DateTime LastAccessedTime
            {
                get { return m_dtLastAccessed; }
                set { m_dtLastAccessed = value; }
            }

            public string Description
            {
                get { return m_strDescription; }
                set { m_strDescription = value; }
            }

            public bool IsRegular
            {
                get { return m_isRegular; }
                set { m_isRegular = value; }
            }

            public bool IsNormal
            {
                get { return m_isNormal; }
                set { m_isNormal = value; }
            }
        }

        public class UserInfo
        {
            private int m_nID;
            private int m_nUserID;
            private DateTime m_time;
            private int m_nControlCheck;
            private string m_strMemberName;
            private string m_strMemberID;
            private int m_nUserLevel;
            private string m_strLevelName;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }
            public int UserID
            {
                get { return m_nUserID; }
                set { m_nUserID = value; }
            }
            public System.DateTime Time
            {
                get { return m_time; }
                set { m_time = value; }
            }
            public int ControlChecked
            {
                get { return m_nControlCheck; }
                set { m_nControlCheck = value; }
            }
            public string MemberName
            {
                get { return m_strMemberName; }
                set { m_strMemberName = value; }
            }
            public string MemberID
            {
                get { return m_strMemberID; }
                set { m_strMemberID = value; }
            }
            public int UserLevel
            {
                get { return m_nUserLevel; }
                set { m_nUserLevel = value; }
            }
            public string LevelName
            {
                get { return m_strLevelName; }
                set { m_strLevelName = value; }
            }
        }

        public class ControlCheck
        {
            private int m_nID;
            private int m_nUserID;
            private DateTime m_nTime;
            private int m_nControlChecked;
            private string m_strMemberName;
            private string m_strMemberID;
            private int m_nUserLevel;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }
            public int UserID
            {
                get { return m_nUserID; }
                set { m_nUserID = value; }
            }
            public System.DateTime Time
            {
                get { return m_nTime; }
                set { m_nTime = value; }
            }
            public int ControlChecked
            {
                get { return m_nControlChecked; }
                set { m_nControlChecked = value; }
            }
            public string MemberName
            {
                get { return m_strMemberName; }
                set { m_strMemberName = value; }
            }
            public string MemberID
            {
                get { return m_strMemberID; }
                set { m_strMemberID = value; }
            }
            public int UserLevel
            {
                get { return m_nUserLevel; }
                set { m_nUserLevel = value; }
            }
        }

        public class ControlUser
        {
            private int m_nControlUserID = -1;
            public int ControlUserID
            {
                get { return m_nControlUserID; }
                set { m_nControlUserID = value; }
            }
        }

        // 재난위치와 연관된 재난 정보들...
        public class HistoryDisasterPosition
        {
            private int mHistoryActionStepID = -1;
            public int HistoryActionStepID
            {
                get { return mHistoryActionStepID; }
                set { mHistoryActionStepID = value; }
            }
            private string szPoistionName = "";
            public string PoistionName
            {
                get { return szPoistionName; }
                set { szPoistionName = value; }
            }
            float xPos;
            public float X
            {
                get { return xPos; }
                set { xPos = value; }
            }
            float yPos;
            public float Y
            {
                get { return yPos; }
                set { yPos = value; }
            }
            float zPos;
            public float Z
            {
                get { return zPos; }
                set { zPos = value; }
            }
            private string szDiasterName = "";
            public string DisasterName
            {
                get { return szDiasterName; }
                set { szDiasterName = value; }
            }

            private float floorIndex = -999.0f;
            public float FloorIndex
            {
                get { return floorIndex; }
                set { floorIndex = value; }
            }

            private string szBuildingID;
            public string BuildingID
            {
                get { return szBuildingID; }
                set { szBuildingID = value; }
            }

            private int m_nIconID = -1;
            public int IconID
            {
                get { return m_nIconID; }
                set { m_nIconID = value; }
            }

            private string m_strPSMMaterialName = "";
            public string PSMMaterial
            {
                get { return m_strPSMMaterialName; }
                set { m_strPSMMaterialName = value; }
            }

            private int m_nPSMDistance = 0;
            public int PSMDistance
            {
                get { return m_nPSMDistance; }
                set { m_nPSMDistance = value; }
            }

            public bool UsePSM
            {
                get { return m_strPSMMaterialName == null || m_strPSMMaterialName.Length == 0 ? false : true; }
            }

            private int m_nZoneID = -1;
            public int ZoneID
            {
                get { return m_nZoneID; }
                set { m_nZoneID = value; }
            }

            // Building인경우 Building의 BroadcastName, Zone인경우 Zone의 BroadcastName
            private string m_szBroadcastName = "";
            public string BroadcastName
            {
                get { return m_szBroadcastName; }
                set { m_szBroadcastName = value; }
            }

        }

        // 재난위치와 관련되어 있지 않은 재난 정보들...
        public class HistoryDisasterNoPosition
        {
            private int mHistoryActionStepID = -1;
            public int HistoryActionStepID
            {
                get { return mHistoryActionStepID; }
                set { mHistoryActionStepID = value; }
            }

            private string m_strAmountSnowfall = "";
            public string AmountSnowfall
            {
                get { return m_strAmountSnowfall; }
                set { m_strAmountSnowfall = value; }
            }

            public bool UseAmountSnowfall
            {
                get { return m_strAmountSnowfall == null || m_strAmountSnowfall.Length == 0 ? false : true; }
            }

            private string m_strDisasterOptions = "";
            public string DisasterOptions
            {
                get { return m_strDisasterOptions; }
                set { m_strDisasterOptions = value; }
            }
        }

        public class ExternalCompanyTeam
        {
            private int m_nTeamID = -1;
            private ExternalCompanyTeam m_teamParent = null;
            private string m_strTeamName = "";
            private int m_nCompanyID = -1;
            private List<ExternalCompanyMember> m_members = new List<ExternalCompanyMember>();

            public int ID
            {
                get { return m_nTeamID; }
                set { m_nTeamID = value; }
            }

            public ExternalCompanyTeam ParentTeam
            {
                get { return m_teamParent; }
                set { m_teamParent = value; }
            }

            public string TeamName
            {
                get { return m_strTeamName; }
                set { m_strTeamName = value; }
            }

            public int CompanyID
            {
                get { return m_nCompanyID; }
                set { m_nCompanyID = value; }
            }

            public List<ExternalCompanyMember> Members
            {
                get { return m_members; }
            }
        }

        public class ExternalCompanyMember
        {
            private int m_nID = -1;
            private string m_strMemberName = "";
            //private bool m_isTeamLeader = false;
            //private ExternalCompanyTeam m_team = null;
            private string m_strPhoneNumber = "";
            //private Dictionary<ExternalCompanyTeam, bool> m_dicTeamLeaders = new Dictionary<ExternalCompanyTeam, bool>();
            private List<ExternalCompanyTeam> m_teams = new List<ExternalCompanyTeam>();

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string MemberName
            {
                get { return m_strMemberName; }
                set { m_strMemberName = value; }
            }

            /*public bool IsTeamLeader
            {
                get { return m_isTeamLeader; }
                set { m_isTeamLeader = value; }
            }

            public ExternalCompanyTeam Team
            {
                get { return m_team; }
                set { m_team = value; }
            }*/

            public string PhoneNumber
            {
                get { return m_strPhoneNumber; }
                set { m_strPhoneNumber = value; }
            }

            /*public Dictionary<ExternalCompanyTeam, bool> TeamLeaders
            {
                get { return m_dicTeamLeaders; }
            }*/

            public List<ExternalCompanyTeam> Teams
            {
                get { return m_teams; }
            }
        }

        public class MissionItemInfo
        {
            private bool m_useSMS = false;
            private bool m_useBroadcast = true;

            public bool UseSMS
            {
                get { return m_useSMS; }
                set { m_useSMS = value; }
            }

            public bool UseBroadcast
            {
                get { return m_useBroadcast; }
                set { m_useBroadcast = value; }
            }
        }

        /*public class Shelter
        {
            private int m_nID = -1;
            private string m_strShelterName = "";
            // 피난처가 다수의 장소일 수 있음
            private List<UnE.Geometry.Polygon> m_boundaries = new List<UnE.Geometry.Polygon>();
            private string m_strDescription = "";

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string ShelterName
            {
                get { return m_strShelterName; }
                set { m_strShelterName = value; }
            }

            // 피난처가 다수의 장소일 수 있음
            public List<UnE.Geometry.Polygon> Boundaries
            {
                get { return m_boundaries; }
            }

            public string Description
            {
                get { return m_strDescription; }
                set { m_strDescription = value; }
            }
        }*/

        //// struct와 같이 null이 허용되지 않는 데이터를 위한 Wrapper 클래스
        //public class VariousData<DataType>
        //{
        //    private DataType data;

        //    public DataType Data
        //    {
        //        get { return data; }
        //        set { data = value; }
        //    }

        //    public VariousData()
        //    {
        //    }

        //    public VariousData(DataType data)
        //    {
        //        this.data = data;
        //    }
        //}

        // 역할을 가진 담당자
        public class DataRoleMember
        {
            private int m_nID = -1;
            private string m_strMemberName = "";
            private string m_strPhoneNumber = "";
            private string m_strRole = "";    // 정 / 부
            private string m_strJobName = "";   // 직책 이름
            // 특정 개인이 아니라 DataRoleMember가 속해있는 팀의 전체 구성원들을 대표하는가?
            private bool m_allMembers = false;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string MemberName
            {
                get { return m_strMemberName; }
                set { m_strMemberName = value; }
            }

            public string PhoneNumber
            {
                get { return m_strPhoneNumber; }
                set { m_strPhoneNumber = value; }
            }

            public string Role
            {
                get { return m_strRole; }
                set { m_strRole = value; }
            }

            public string JobName
            {
                get { return m_strJobName; }
                set { m_strJobName = value; }
            }

            // 특정 개인이 아니라 DataRoleMember가 속해있는 팀의 전체 구성원들을 대표하는가?
            public bool AllMembers
            {
                get { return m_allMembers; }
                set { m_allMembers = value; }
            }

            public DataRoleMember()
            {
            }

            public DataRoleMember(string strMemberName, string strPhoneNumber, string strRole, string strJobName)
            {
                m_strMemberName = strMemberName;
                m_strPhoneNumber = strPhoneNumber;
                m_strRole = strRole;
                m_strJobName = strJobName;
            }
        }

        public class PSMMaterial
        {
            private int m_nMaterialID = -1;
            // 유해화학물질 이름
            private string m_strMaterialName = "";
            // 초기 이격거리(미터)
            private int m_nInitDistance = 0;
            // 주간 방호대피거리(미터)
            private int m_nDayDistance = 0;
            // 야간 방호대피거리(미터)
            private int m_nNightDistance = 0;

            public int MaterialID
            {
                get { return m_nMaterialID; }
                set { m_nMaterialID = value; }
            }

            public string MaterialName
            {
                get { return m_strMaterialName; }
                set { m_strMaterialName = value; }
            }

            public int InitDistance
            {
                get { return m_nInitDistance; }
                set { m_nInitDistance = value; }
            }

            public int DayDistance
            {
                get { return m_nDayDistance; }
                set { m_nDayDistance = value; }
            }

            public int NightDistance
            {
                get { return m_nNightDistance; }
                set { m_nNightDistance = value; }
            }

            public override string ToString()
            {
                return m_strMaterialName;
            }
        }

        public class ConfigData
        {
            private string m_strText = "";
            private string m_strDescription = "";
            private List<SOPParameter> m_variables = new List<SOPParameter>();
            
            public string Text
            {
                get { return m_strText; }
                set { m_strText = value; }
            }

            public string Description
            {
                get { return m_strDescription; }
                set { m_strDescription = value; }
            }

            public List<SOPParameter> Variables
            {
                get { return m_variables; }
            }

            public ConfigData()
            {
            }

            public ConfigData(string strText)
            {
                m_strText = strText;
            }

            public ConfigData Clone(string strConfigName)
            {
                ConfigData trg = new ConfigData();
                trg.Text = strConfigName;
                trg.Description = this.Description;

                foreach (SOPParameter param in this.Variables)
                {
                    trg.Variables.Add(param.Clone());
                }

                return trg;
            }

            public override string ToString()
            {
                return m_strText;
            }
        }

        public class SOPParameter
        {
            private string m_strVariableName = "";
            private global::Sections.SectionDataDecision.VariableType m_type = global::Sections.SectionDataDecision.VariableType.UNKNOWN;
            private string m_strDescription = "";

            public string VariableName
            {
                get { return m_strVariableName; }
                set { m_strVariableName = value; }
            }

            public global::Sections.SectionDataDecision.VariableType Type
            {
                get { return m_type; }
                set { m_type = value; }
            }

            public string Description
            {
                get { return m_strDescription; }
                set { m_strDescription = value; }
            }

            public SOPParameter Clone()
            {
                SOPParameter param = new SOPParameter();

                param.VariableName = this.VariableName;
                param.Type = this.Type;
                param.Description = this.Description;

                return param;
            }
        }
    }
}
