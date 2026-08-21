using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlClient;

namespace ControlMonitoring
{
    public class ControllerInfo
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
        public int ControlCheck
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

    public class ControlCheckData
    {
        private int m_nID;
        private int m_nUserID;
        private DateTime m_time;
        private bool m_isControlCheck;

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
        public bool ControlCheck
        {
            get { return m_isControlCheck; }
            set { m_isControlCheck = value; }
        }
    }

    public class CompanymemberData
    {
        private int m_nID;
        private string m_strMemberName;
        private int m_nRegularTeamID;
        private int m_nLevelID;
        private int m_nPositionID;
        private string m_strMemberID;
        private int m_nSecondRegularTeamID;
        private int m_nSecondPositionID;
        private string m_strPhoneNumber;

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
            get { return m_nSecondRegularTeamID; }
            set { m_nSecondRegularTeamID = value; }
        }
        public int SecondPositionID
        {
            get { return m_nSecondPositionID; }
            set { m_nSecondPositionID = value; }
        }
        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }
    }

    public class SOPGenLevelData
    {
        private int m_nID;
        private string m_strLevelName;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public string LevelName
        {
            get { return m_strLevelName; }
            set { m_strLevelName = value; }
        }
    }

    public class SOPGenUserData
    {
        private int m_nID;
        private int m_nMemberID;
        private int m_nUserLevel;
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
        public int UserLevel
        {
            get { return m_nUserLevel; }
            set { m_nUserLevel = value; }
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
}
