using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOPMonitoringSystem
{
    class Data_CompanyMember
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

    class Data_DispasterCategory
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

    class Data_Building
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

    class Data_BuildingGroup
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

}
