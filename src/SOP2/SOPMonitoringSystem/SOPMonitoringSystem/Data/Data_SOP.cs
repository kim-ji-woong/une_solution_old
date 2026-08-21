using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SOPMonitoringSystem
{
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

    class Data_CompanyMember
    {
        private int m_nID;
        private string m_strMemberName = "";
        private int m_nRegularTeamID;
        private int m_nTemporaryTeamID;
        private int m_nLevelID;
        private int m_nPositionID;
        private int m_nTemporaryPositionID;
        private string m_strMemberID = "";
        private string m_strPhoneNumber = "";
        private string m_strOfficePhoneNumber = "";

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

        public string MemberID
        {
            get { return m_strMemberID; }
            set { m_strMemberID = value; }
        }

        public int SecondRegularTeamID
        {
            get { return m_nTemporaryTeamID; }
            set { m_nTemporaryTeamID = value; }
        }

        public int SecondPositionID
        {
            get { return m_nTemporaryPositionID; }
            set { m_nTemporaryPositionID = value; }
        }

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
    }

    class Data_SOPGenUser
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

    class Data_DisasterCategory
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

    class Data_SubDisasterCategory
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

    class Data_Disaster
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
            set { m_strDescription= value; }
        }
    }

    class Data_RegularTeam
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

    class Data_SearchMember
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
 
    class Data_Task
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

    class Data_Site
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

    class Data_EquipmentInfo
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

    class Data_NormalTeam
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
    }

    class Data_CheckTask
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
    }

    class Data_ActionStepHistory
    {
        private int m_nID = -1;
        private int m_nActionStepID = -1;
        private bool m_isRealMode = false;
        private DateTime m_beginTime;
        private DateTime m_endTime;
        private DateTime m_cancelTime;
        //private DateTime m_PausedTime;
        private DateTime m_detectTime;

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

        public DateTime EndTime
        {
            get { return m_endTime; }
            set { m_endTime = value; }
        }

        public DateTime CancelTime
        {
            get { return m_cancelTime; }
            set { m_cancelTime = value; }
        }

        public DateTime PausedTime
        {
            get { return m_cancelTime; }
            set { m_cancelTime = value; }
        }

        public DateTime DetectTime
        {
            get { return m_detectTime; }
            set { m_detectTime = value; }
        }
    }

    class Data_UserDefinedTeam
    {
        private int m_nID;
        private string m_strTeamName;
        private string m_strPhoneNumber;
        private string m_strFaxNumber;

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
    }

    public class Data_ExternalTeam
    {
        private int m_nID = -1;
        private string m_strTeamName = "";
        private string m_strPhoneNumber = "";
        private string m_strFaxNumber = "";

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
    }

    class Data_Version
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
    }

    public class VersionInfo
    {
        private int m_nVersionID = -1;
        private string m_strVersionName = "";
        private string m_strUserName = "";
        private DateTime m_dtBegin;
        private DateTime m_dtEnd;
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

        public DateTime EndTime
        {
            get { return m_dtEnd; }
            set { m_dtEnd = value; }
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
    
    
    public class HistoryDiasterPosition
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

    }

    public class ExternalCompanyTeam
    {
        private int m_nTeamID = -1;
        private ExternalCompanyTeam m_teamParent = null;
        private string m_strTeamName = "";
        private int m_nCompanyID = -1;

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
    }

    public class ExternalCompanyMember
    {
        private int m_nID = -1;
        private string m_strMemberName = "";
        private bool m_isTeamLeader = false;
        private ExternalCompanyTeam m_team = null;
        private string m_strPhoneNumber = "";

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

        public bool IsTeamLeader
        {
            get { return m_isTeamLeader; }
            set { m_isTeamLeader = value; }
        }

        public ExternalCompanyTeam Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
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
}
