using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOPManager
{
    /*class Data_CompanyMember
    {
        private int m_nID;
        private string m_strMemberName;
        private int m_nRegularTeamID;
        private int m_nTemporaryTeamID;
        private int m_nLevelID;
        private int m_nPositionID;
        private int m_nTemporaryPositionID;
        private string m_strMemberID;

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

        public int RegularTeamID
        {
            get { return m_nRegularTeamID; }
            set { m_nRegularTeamID = value; }
        }

        public int TemporaryTeamID
        {
            get { return m_nTemporaryTeamID; }
            set { m_nTemporaryTeamID = value; }
        }

        public int LevelID
        {
            get { return m_nLevelID; }
            set { m_nLevelID = value; }
        }

        public int PositionID
        {
            get { return m_nPositionID; }
            set { m_nPositionID = value; }
        }

        public int TemporaryPositionID
        {
            get { return m_nTemporaryPositionID; }
            set { m_nTemporaryPositionID = value; }
        }

        public string MemberID
        {
            get { return m_strMemberID; }
            set { m_strMemberID = value; }
        }
    }*/

    public class Data_SOPGenUser
    {
        private int m_nID;
        private int m_nMemberID;
        private string m_strUserName;
        private int m_nUserLevel;
        private int m_nTeamID;
        private string m_strPassword;
        private string m_strUserID;

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

        public string Password
        {
            get { return m_strPassword; }
            set { m_strPassword = value; }
        }

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
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

    public class Data_DisasterType
	{
		private int m_nID;
		private string m_strDisasterName;
		private int m_nSubDisasterID;

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
	}

    public class Data_Disaster : IComparable
    {
		private int m_nID = -1;
		private string m_strDisasterName = "";
		private int m_nSubDisasterID = -1;
		private int m_nVersionID = -1;
        private string m_strDescription ="";

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
            set { m_strDescription= value; }
        }

        public int CompareTo(object obj)
        {
            Data_Disaster disaster = (Data_Disaster)obj;
            return this.m_strDisasterName.CompareTo(disaster.m_strDisasterName);
        }
    }

    public class Data_RegularTeam
    {
        private int m_nID;
        private string m_strTeamName;
        private int m_nParentTeamID;

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
    }

    public class Data_ControlRoom
    {
        public const string ROOT_NAME = "교대 근무자";
        public const int ROOT_ID = 0;

        public Data_ControlRoom()
        {
            TeamName = ROOT_NAME;
            ID = ROOT_ID;
        }

        private int m_nID;          
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private string m_strTeamName;
        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        private int m_nParentTeamID;
        public int ParentTeamID
        {
            get { return m_nParentTeamID; }
            set { m_nParentTeamID = value; }
        }

        public static int MakeID(int nRoomTypeID, int nControlRoomID, int nControlTeamJobPositionID)
        {
            int nID = (nControlTeamJobPositionID << 16) | (nControlRoomID << 8) | nRoomTypeID;
            return nID;
        }

        public static void GetParams(int nID, out int nRoomTypeID, out int nControlRoomID, out int nControlTeamJobPositionID)
        {
            nRoomTypeID = nID & 0xff;
            nControlRoomID = (nID & 0xff00) >> 8;
            nControlTeamJobPositionID = nID >> 16;
        }

        private int m_nRoomTypeID;
        public int RoomTypeID
        {
            get { return m_nRoomTypeID; }
            set { m_nRoomTypeID = value; }
        }

        public int ControlRoomID
        {
            get { return ((ID & 0xff00) >> 8); }
        }

        public int ControlTeamJobPositionID
        {
            get { return (ID >> 16); }
        }

        private Data_ControlRoom m_teamParent = null;
        public Data_ControlRoom ParentTeam
        {
            get { return m_teamParent; }
            set
            {
                if (m_teamParent != null)
                    m_teamParent.RemoveChild(this);

                m_teamParent = value;

                if (m_teamParent != null)
                    m_teamParent.AddChild(this);
            }
        }
        protected void RemoveChild(Data_ControlRoom team)
        {
            if (team != null)
                m_arrChildTeams.Remove(team);
        }

        protected void AddChild(Data_ControlRoom team)
        {
            if (!m_arrChildTeams.Contains(team))
                m_arrChildTeams.Add(team);
        }

        private System.Collections.ArrayList m_arrChildTeams = new System.Collections.ArrayList();
        public System.Collections.ArrayList ChildTeams
        {
            get { return m_arrChildTeams; }
        }

        public override string ToString()
        {
            return m_strTeamName;
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
        private int m_nID;
        private string m_strBuildingName;
        private int m_nGroupID;
        private int m_nMaxFloor;
        private int m_nMinFloor;

        public int BuildingID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string BuildingName
        {
            get { return m_strBuildingName; }
            set { m_strBuildingName = value; }
        }

        public int BuildingGroupID
        {
            get { return m_nGroupID; }
            set { m_nGroupID = value; }
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

    public class Data_BuildingGroup
    {
        private int m_nID;
        private string m_strGroupName;
        private int m_nSiteID;
        private string m_strSiteName;

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

    public class Data_TemporaryTeam
    {
        /*public enum TeamType
        {
            Unknown = -1,
            RegularTeam = 0,        // 정규조직
            CompanyMember,          // 정직원
            ExternalCompanyTeam,    // 사용안함
            ExternalTeam,           // 외부 협력업체 회사 및 팀
            ExternalCompanyMember,  // 외부 협력업체 팀원
            UserDefinedTeam,        // 사용자 정의 조직
            JobLevel                // 직급
        };*/

        private int m_nID = 0;
        //private int m_nTeamID = 0;
        private int m_nParentTeamID = 0;
        private string m_strTeamName = "";
        //private int m_nLevelNo = 0;
        //protected TeamType m_teamType = TeamType.Unknown;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        /*public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }*/

        public int ParentTeamID
        {
            get { return m_nParentTeamID; }
            set { m_nParentTeamID = value; }
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        /*public int LevelNo
        {
            get { return m_nLevelNo; }
            set { m_nLevelNo = value; }
        }

        public TeamType GetTeamType()
        {
            return m_teamType;
        }

        public void SetTeamType(TeamType type)
        {
            m_teamType = type;
        }

        public static bool TryToTeamType(int nTeamType, out TeamType teamType)
        {
            teamType = TeamType.Unknown;

            if (nTeamType <= (int)TeamType.Unknown || nTeamType > (int)TeamType.JobLevel)
                return false;

            teamType = (TeamType)nTeamType;
            return true;
        }*/
    }

    public class Data_NormalTeam : Data_TemporaryTeam
    {
    }

    public class Data_EmergencyTeam : Data_TemporaryTeam
    {
    }

    /*class Data_NormalTeam
    {
        private int m_nID;
        private string m_strTeamName;
        private int m_nParentTeamID;
        private string m_strGroupName;
        private int m_nLevelNo;
        private string m_strDescription;
        private string m_strRegularTeamLink;
        
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
        public string RegularTeamLink
        {
            get { return m_strRegularTeamLink; }
            set { m_strRegularTeamLink = value; }
        }
    }

    class Data_EmergencyTeam
    {
        private int m_nID;
        private string m_strTeamName;
        private int m_nParentTeamID;
        private string m_strGroupName;
        private int m_nLevelNo;
        private string m_strDescription;
        private string m_strRegularTeamLink;

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
        public string RegularTeamLink
        {
            get { return m_strRegularTeamLink; }
            set { m_strRegularTeamLink = value; }
        }
    }*/

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

        // SOP의 표준 대응단계
        private static string[] StandardActionStepName = new string[] { "예방", "대비", "대응", "복구" };

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

		public Data_ActionStep Clone()
		{	
			Data_ActionStep org = new Data_ActionStep();
			org.BeginTime = this.BeginTime;
			org.DisasterID = this.DisasterID;
			org.EndTime = this.EndTime;
			org.ID = this.ID;
			org.Iteration = this.Iteration;
			org.IterationType = this.IterationType;
			org.ParentStepID = this.ParentStepID;
			org.PeriodType = this.PeriodType;
			org.ProcessTime = this.ProcessTime;
			org.ProcessTimeType = this.ProcessTimeType;
			org.StepName = this.StepName;
			org.WeekdayOption = this.WeekdayOption;	
			return org;
		}

        public static void SetStandardActionStepNames(List<string> actionStepNames)
        {
            if (actionStepNames == null || actionStepNames.Count == 0)
                return;

            if (actionStepNames.Count != StandardActionStepName.Count())
                StandardActionStepName = new string[actionStepNames.Count];

            for (int i = 0; i < actionStepNames.Count; i++)
            {
                StandardActionStepName[i] = actionStepNames[i];
            }
        }

        public static string[] StandardActionStepNames
        {
            get { return StandardActionStepName; }
        }

        public static int GetActionStepIndex(string strActionStepName, List<string> oldActionStepNames = null)
        {
            int nIndex = -1;
            int nCount = StandardActionStepName.Count();

            for (int i = 0; i < nCount; i++)
            {
                if (strActionStepName == StandardActionStepName[i])
                {
                    nIndex = i;
                    break;
                }
            }

            if (nIndex < 0)
            {
                System.Diagnostics.Trace.WriteLine("Unknown StepName : " + strActionStepName);
                nIndex = 0;
            }

            if (oldActionStepNames == null)
                return nIndex;

            if (oldActionStepNames.Count >= nIndex)
                return nIndex;

            for (int i = nIndex; i < oldActionStepNames.Count; i++)
            {
                string strStepName = oldActionStepNames[i];

                if (GetActionStepIndex(strStepName) > nIndex)
                    return i;
            }

            return oldActionStepNames.Count;
        }
    }

    public class Data_UserDefinedTeam
    {
        private int m_nID;
        private string m_strTeamName;
        private string m_strPhoneNumber;
        private string m_strFaxNumber;

        public int ID
        {
            get { return m_nID; }
            set 
            { 
                m_nID = value; 
                
                if( m_nID == -1 && m_strTeamName == "최초발견자")
                {
                    int i = 0;
                    i++;
                }

            }
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

    public class Data_ExternalTeam
    {
        private int m_nID = -1;
        private string m_strTeamName = "";
        private string m_strPhoneNumber = "";
        private string m_strFaxNumber = "";

        public int ID
        {
            get { return m_nID; }
            set 
            { 
                m_nID = value; }
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

        private int m_nParentID = -1;
        public int ParentID
        {
            get { return m_nParentID; }
            set { m_nParentID = value; }
        }

        private Data_ExternalTeam m_nParentTeam = null;
        public Data_ExternalTeam ParentTeam
        {
            get { return m_nParentTeam; }
            set { m_nParentTeam = value; }
        }
    }

    //public class Data_ExternalCompanyTeam
    //{
    //    private int m_nID = -1;
    //    private string m_strTeamName = "";
    //    private Data_ExternalCompanyTeam m_teamParent = null;
    //    private Data_ExternalTeam m_company = null;
    //    private string m_strPhoneNumber = "";
    //    private string m_strFaxNumber = "";

    //    public int ID
    //    {
    //        get { return m_nID; }
    //        set { m_nID = value; }
    //    }
        
    //    public string TeamName
    //    {
    //        get { return m_strTeamName; }
    //        set { m_strTeamName = value; }
    //    }

    //    public Data_ExternalTeam Company
    //    {
    //        get { return m_company; }
    //        set { m_company = value; }
    //    }

    //    public Data_ExternalCompanyTeam ParentTeam
    //    {
    //        get { return m_teamParent; }
    //        set { m_teamParent = value; }
    //    }

    //    public string PhoneNumber
    //    {
    //        get { return m_strPhoneNumber; }
    //        set { m_strPhoneNumber = value; }
    //    }

    //    public string FaxNumber
    //    {
    //        get { return m_strFaxNumber; }
    //        set { m_strFaxNumber = value; }
    //    }
    //}

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

    public class ConfigData
    {
        public enum ConfigType { NOT_USE, FILE, NEW, EDIT };

        private ConfigType m_dataType = ConfigType.FILE;
        private string m_strText = "";
        private string m_strDescription = "";
        private List<SOPParameter> m_variables = new List<SOPParameter>();
        private bool m_isChanged = false;
        private int m_nID = -1;

        public ConfigType Type
        {
            get { return m_dataType; }
            set
            {
                m_dataType = value;

                if (m_dataType == ConfigType.NEW)
                    m_strText = "<새로 만들기...>";
                else if (m_dataType == ConfigType.EDIT)
                    m_strText = "<편집...>";
                else if (m_dataType == ConfigType.NOT_USE)
                    m_strText = "사용안함";
            }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

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

        public bool IsChanged
        {
            get { return m_isChanged; }
            set { m_isChanged = value; }
        }

        public ConfigData(ConfigType type)
        {
            Type = type;
        }

        public ConfigData(ConfigType type, string strText)
        {
            m_strText = strText;
            Type = type;
        }

        public ConfigData Clone(string strConfigName)
        {
            ConfigData trg = new ConfigData(this.Type);
            trg.Text = strConfigName;
            trg.Description = this.Description;
            trg.ID = this.ID;

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
}
