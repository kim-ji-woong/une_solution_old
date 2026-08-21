using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;

namespace ControlTeamEditor
{
    public class DataManager
    {
        private DBUtility.WebDBManager m_dbMgr= null;
        private int m_nSiteID = 1;

        private DataTeam m_teamRegularRoot = null;

        private Dictionary<int, DataTeam> m_dicRegularTeams = new Dictionary<int, DataTeam>();
        private Dictionary<DataTeam, ArrayList> m_dicRegularTeamMembers = new Dictionary<DataTeam, ArrayList>();
        private Dictionary<int, DataCompanyMember> m_dicRegularMembers = new Dictionary<int, DataCompanyMember>();

        private Dictionary<int, DataTeam> m_dicExternalTeams = new Dictionary<int, DataTeam>();
        private Dictionary<DataTeam, ArrayList> m_dicExternalTeamMembers = new Dictionary<DataTeam, ArrayList>();
        private Dictionary<int, DataCompanyMember> m_dicExternalMembers = new Dictionary<int, DataCompanyMember>();
        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });
        
        public DataTeam RegularTeamRoot
        {
            get { return m_teamRegularRoot; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        // 정규조직 혹은 외부협력업체 팀원들 리스트를 리턴
        public ArrayList GetTeamMembers(DataTeam team)
        {
            if (m_dicRegularTeamMembers.ContainsKey(team))
                return m_dicRegularTeamMembers[team];

            return null;
        }

        public DataManager(DBUtility.WebDBManager dbMgr, int nSiteID)
        {
            m_dbMgr = dbMgr;
            m_nSiteID = nSiteID;

            LoadData();
        }

        public void LoadData()
        {
            m_teamRegularRoot = LoadRegularTeam(m_dbMgr, m_dicRegularTeams);
            LoadCompanyMember(m_dbMgr, m_dicRegularTeams);

            string strExternalTeamIDs = LoadExternalTeam(m_dbMgr, m_dicExternalTeams);
            LoadExternalCompanyMember(m_dbMgr, m_dicExternalTeams, strExternalTeamIDs);

            LoadControlRoom();

            LoadControlTeam();

            LoadControlJobPosition();

            LoadContorlWorkingTeam();

            LoadControlMembers();

            LoadControlBasicMembers();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //  Control Room
        //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // 각 제어실들의 타입
        private Dictionary<int, DataControlRoomType> m_dicRoomTypes = new Dictionary<int, DataControlRoomType>();
        // 타입별 제어실 리스트
        private Dictionary<DataControlRoomType, List<DataControlRoom>> m_dicRoomTypeRooms = new Dictionary<DataControlRoomType, List<DataControlRoom>>();
        // 각 제어실을 표현하는 ControlRoom을 이름별 저장
        private Dictionary<string, DataControlRoom> m_dicNameRooms = new Dictionary<string, DataControlRoom>();
        // 각 제어실을 표현하는 ControlRoom을 ID별 저장
        private Dictionary<int, DataControlRoom> m_dicIdRooms = new Dictionary<int, DataControlRoom>();

        public Dictionary<int, DataControlRoomType> GetControlRoomTypes()
        {
            return m_dicRoomTypes;
        }

        public DataControlRoomType GetControlRoomType(string strRoomType)
        {
            foreach (KeyValuePair<int, DataControlRoomType> pair in m_dicRoomTypes)
            {
                if (pair.Value.Description == strRoomType || pair.Value.RoomType == strRoomType)
                    return pair.Value;
            }

            return null;
        }

        public List<DataControlRoom> GetControlRooms(DataControlRoomType roomType)
        {
            List<DataControlRoom> rooms;

            if (m_dicRoomTypeRooms.TryGetValue(roomType, out rooms))
                return rooms;

            return null;
        }

        public List<DataControlRoom> GetControlRooms()
        {
            List<DataControlRoom> list = new List<DataControlRoom>();
            list.AddRange(m_dicIdRooms.Values);
            return list;
        }

        public DataControlRoom GetControlRoom(int nID)
        {
            if (m_dicIdRooms.ContainsKey(nID))
            {
                return m_dicIdRooms[nID];
            }
            return null;
        }

        public DataControlRoom GetControlRoom(string szName)
        {
            if(m_dicNameRooms.ContainsKey(szName))
            {
                return m_dicNameRooms[szName];
            }
            return null;
        }

        private bool LoadControlRoomType()
        {
            string strSQL = "Select ID, TypeName, Description from ControlRoomType where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTypeName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strDescription = WebDBManager.GetStringField(arrResult[i + 2], "");

                m_dicRoomTypes[nID] = new DataControlRoomType(nID, strTypeName, strDescription);
            }

            return true;
        }

        private bool LoadControlRoom()
        {
            if (!LoadControlRoomType())
                return false;

            string strSQL = "SELECT ID, RoomType, LocationName, DisplayText, Descritpion FROM ControlRoom";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return false;

            int nCount = arrResult.Count;
            if (nCount == 0)
                return true;

            for (int i = 0; i < nCount - 4; i += 5)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nRoomTypeID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string szLocationName = DBUtility.WebDBManager.GetStringField(arrResult[i + 2], "");            
                string szDisplayText = DBUtility.WebDBManager.GetStringField(arrResult[i + 3], "");
                string szDescription = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], "");

                DataControlRoomType roomType;

                if (!m_dicRoomTypes.TryGetValue(nRoomTypeID, out roomType))
                    continue;

                DataControlRoom room = new DataControlRoom();

                room.ID = nID;
                room.LocationName = szLocationName;
                room.DisplayText = szDisplayText;
                room.RoomType = roomType;
                room.Descritpion = szDescription;

                m_dicNameRooms[szLocationName] = room;
                m_dicIdRooms[nID] = room;

                List<DataControlRoom> rooms = null;

                if (!m_dicRoomTypeRooms.TryGetValue(roomType, out rooms))
                {
                    rooms = new List<DataControlRoom>();
                    m_dicRoomTypeRooms[roomType] = rooms;
                }

                rooms.Add(room);
            }
            return true;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //  Control Team
        //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // RoomType별 근무조
        private Dictionary<DataControlRoomType, List<DataControlTeam>> m_dicRoomTypeControlTeams = new Dictionary<DataControlRoomType, List<DataControlTeam>>();
        // 각 제어실을 표현하는 ControlTeam을 이름별 저장
        private Dictionary<string, DataControlTeam> m_dicNameTeams = new Dictionary<string, DataControlTeam>();
        // 각 제어실을 표현하는 ControlTeam을 ID별 저장
        private Dictionary<int, DataControlTeam> m_dicIdTeams = new Dictionary<int, DataControlTeam>();

        public List<DataControlTeam> GetControlTeams(DataControlRoomType roomType)
        {
            List<DataControlTeam> teams;

            if (m_dicRoomTypeControlTeams.TryGetValue(roomType, out teams))
                return teams;

            return null;
        }

        public List<DataControlTeam> GetControlTeams()
        {
            List<DataControlTeam> list = new List<DataControlTeam>();
            list.AddRange(m_dicIdTeams.Values);
            return list;
        }

        public DataControlTeam GetControlTeam(int nID)
        {
            if (m_dicIdTeams.ContainsKey(nID))
            {
                return m_dicIdTeams[nID];
            }
            return null;
        }

        public DataControlTeam GetControlTeam(string szName)
        {
            if (m_dicNameTeams.ContainsKey(szName))
            {
                return m_dicNameTeams[szName];
            }
            return null;
        }

        private bool LoadControlTeam()
        {
            string strSQL = "SELECT ID, RoomType, TeamName, DisplayText, Description FROM ControlTeam";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return false;

            int nCount = arrResult.Count;
            if (nCount == 0)
                return true;

            for (int i = 0; i < nCount - 4; i += 5)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nRoomTypeID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string szTeamName = DBUtility.WebDBManager.GetStringField(arrResult[i + 2], "");
                string szDisplayText = DBUtility.WebDBManager.GetStringField(arrResult[i + 3], "");
                string szDescription = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], "");

                DataControlRoomType roomType;

                if (!m_dicRoomTypes.TryGetValue(nRoomTypeID, out roomType))
                    continue;

                DataControlTeam team = new DataControlTeam();

                team.ID = nID;
                team.RoomType = roomType;
                team.TeamName = szTeamName;
                team.DisplayText = szDisplayText;
                team.Descritpion = szDescription;

                m_dicNameTeams[szTeamName] = team;
                m_dicIdTeams[nID] = team;

                List<DataControlTeam> teams;

                if (!m_dicRoomTypeControlTeams.TryGetValue(roomType, out teams))
                {
                    teams = new List<DataControlTeam>();
                    m_dicRoomTypeControlTeams[roomType] = teams;
                }

                teams.Add(team);
            }
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //  ControlTeamJobPosition
        //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // RoomType별 근무자 직별 리스트
        private Dictionary<DataControlRoomType, List<DataControlTeamJobPosition>> m_dicRoomTypeJobPositions = new Dictionary<DataControlRoomType, List<DataControlTeamJobPosition>>();
        // ControlTeamJobPosition을 이름별 저장
        private Dictionary<string, DataControlTeamJobPosition> m_dicNameJobs = new Dictionary<string, DataControlTeamJobPosition>();
        // ControlTeamJobPosition을 ID별 저장
        private Dictionary<int, DataControlTeamJobPosition> m_dicIdJobs = new Dictionary<int, DataControlTeamJobPosition>();

        public List<DataControlTeamJobPosition> GetJobPositions(DataControlRoomType roomType)
        {
            List<DataControlTeamJobPosition> positions;

            if (m_dicRoomTypeJobPositions.TryGetValue(roomType, out positions))
                return positions;

            return null;
        }

        public List<DataControlTeamJobPosition> GetJobPositions()
        {
            List<DataControlTeamJobPosition> list = new List<DataControlTeamJobPosition>();
            list.AddRange(m_dicIdJobs.Values);
            return list;
        }

        public DataControlTeamJobPosition GetJobPosition(int nID)
        {
            if (m_dicIdJobs.ContainsKey(nID))
            {
                return m_dicIdJobs[nID];
            }
            return null;
        }

        public DataControlTeamJobPosition GetJobPosition(string szName)
        {
            if (m_dicNameJobs.ContainsKey(szName))
            {
                return m_dicNameJobs[szName];
            }
            return null;
        }
        private bool LoadControlJobPosition()
        {
            string strSQL = "SELECT ID, JobName, RoomType, Description FROM ControlTeamJobPosition";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return false;

            int nCount = arrResult.Count;
            if (nCount == 0)
                return true;

            for (int i = 0; i < nCount - 3; i += 4)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string szJobName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
                int nRoomTypeID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                string szDescription = DBUtility.WebDBManager.GetStringField(arrResult[i + 3], "");

                DataControlRoomType roomType;

                if (!m_dicRoomTypes.TryGetValue(nRoomTypeID, out roomType))
                    continue;

                DataControlTeamJobPosition job = new DataControlTeamJobPosition();

                job.ID = nID;
                job.JobName = szJobName;
                job.RoomType = roomType;
                job.Descritpion = szDescription;

                m_dicNameJobs[szJobName] = job;
                m_dicIdJobs[nID] = job;

                List<DataControlTeamJobPosition> positions;

                if (!m_dicRoomTypeJobPositions.TryGetValue(roomType, out positions))
                {
                    positions = new List<DataControlTeamJobPosition>();
                    m_dicRoomTypeJobPositions[roomType] = positions;
                }

                positions.Add(job);
            }
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //  LoadContorlWorkingTeam / SaveContorlWorkingTeam
        //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private List<DataControlWorkingTeam> m_WorkList = new List<DataControlWorkingTeam>();
        private Dictionary<int, DataControlWorkingTeam> m_dicWorkTeams = new Dictionary<int, DataControlWorkingTeam>();

        public List<DataControlWorkingTeam> GetWorkTeams()
        {
            return m_WorkList;
        }

        public DataControlWorkingTeam GetWorkTeam(int roomID)
        {
            if (m_dicWorkTeams.ContainsKey(roomID))
            {
                DataControlWorkingTeam work = m_dicWorkTeams[roomID];
                return work;
            }
            return null;
        }

        private int GetContorlWorkingTeamID()
        {
            string strSQL = "select max(id) from ControlWorkingTeam";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null || arrResult.Count == 0)
                return 0;

            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        public void SaveControlWorkingTeam(DataControlWorkingTeam workingTeam)
        {
            StringBuilder sb = new StringBuilder();
            if (workingTeam.ID <= 0)
            {
                int nID = GetContorlWorkingTeamID();
                workingTeam.ID = nID + 1;
                if (workingTeam.Team != null)
                {
                    sb.Append("INSERT INTO ControlWorkingTeam (ID, RoomID, TeamID, Description) VALUES ");
                    sb.AppendFormat(" ({0}, {1}, {2}, '{3}') ", workingTeam.ID, workingTeam.Room.ID, workingTeam.Team.ID, "");
                }
                else
                {
                    sb.Append("INSERT INTO ControlWorkingTeam (ID, RoomID, TeamID, Description) VALUES ");
                    sb.AppendFormat(" ({0}, {1}, {2}, '{3}') ", workingTeam.ID, workingTeam.Room.ID, "NULL", "");
                }
            }
            else
            {
                if (workingTeam.Team != null)
                {
                    sb.AppendFormat("UPDATE ControlWorkingTeam SET TeamID = {0} ", workingTeam.Team.ID);
                    sb.AppendFormat(" WHERE ID = {0}", workingTeam.ID);
                }
                else
                {
                    sb.AppendFormat("UPDATE ControlWorkingTeam SET TeamID = NULL ");
                    sb.AppendFormat(" WHERE ID = {0}", workingTeam.ID);
                }
            }
            string strSQL = sb.ToString();
            m_dbMgr.GetResultData(strSQL, 0);
        }

        public void SaveControlWorkingTeams()
        {
            foreach (DataControlWorkingTeam data in m_WorkList)
            {
                StringBuilder sb = new StringBuilder();
                if (data.ID <= 0)
                {
                    int nID = GetContorlWorkingTeamID();
                    data.ID = nID + 1;
                    if (data.Team != null)
                    {
                        sb.Append("INSERT INTO ControlWorkingTeam (ID, RoomID, TeamID, Description) VALUES ");
                        sb.AppendFormat(" ({0}, {1}, {2}, '{3}') ", data.ID, data.Room.ID, data.Team.ID, "");
                    }
                    else
                    {
                        sb.Append("INSERT INTO ControlWorkingTeam (ID, RoomID, TeamID, Description) VALUES ");
                        sb.AppendFormat(" ({0}, {1}, {2}, '{3}') ", data.ID, data.Room.ID, "NULL", "");
                    }
                }
                else
                {
                    if (data.Team != null)
                    {
                        sb.AppendFormat("UPDATE ControlWorkingTeam SET TeamID = {0} ", data.Team.ID);
                        sb.AppendFormat(" WHERE ID = {0}", data.ID);
                    }
                    else
                    {
                        sb.AppendFormat("UPDATE ControlWorkingTeam SET TeamID = NULL ");
                        sb.AppendFormat(" WHERE ID = {0}", data.ID);
                    }
                }
                string strSQL = sb.ToString();
                m_dbMgr.GetResultData(strSQL, 0);
            }
        }

        private bool LoadContorlWorkingTeam()
        {
            m_WorkList.Clear();
            m_dicWorkTeams.Clear();

            List<DataControlRoom> rooms = GetControlRooms();

            foreach(DataControlRoom room in rooms)
            {
                DataControlWorkingTeam work = new DataControlWorkingTeam();
                work.Room = room;
                work.ID = -1;
                work.Team = null;

                m_WorkList.Add(work);
                m_dicWorkTeams.Add(room.ID, work);
            }            
            
            string strSQL = "SELECT ID, TeamID, RoomID, Description FROM ControlWorkingTeam";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return false;

            int nCount = arrResult.Count;
            if (nCount == 0)
                return true;

            for (int i = 0; i < nCount - 3; i += 4)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i+1].ToString(), 0);
                int nRoomID = DBUtility.WebDBManager.GetIntField(arrResult[i+2].ToString(), 0);
                string szDescription = DBUtility.WebDBManager.GetStringField(arrResult[i + 3], "");

                if(m_dicWorkTeams.ContainsKey(nRoomID))
                {
                    DataControlWorkingTeam work = m_dicWorkTeams[nRoomID];
                    work.ID = nID;
                    work.Team = GetControlTeam(nTeamID);
                    work.Descritpion = szDescription;
                }                
            }
            return true;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //  LoadControlMembers / SaveControlTeamMembers
        //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Key : 상위부터
        //       2Byte(사용하지 않음) 2Byte(Room ID), 2Byte(Team ID), 2Byte(Job ID)
        private Dictionary<long, DataControlTeamMember> m_dicControlMembers = new Dictionary<long, DataControlTeamMember>();
        private Dictionary<long, DataControlTeamMember> m_dicControlBasicMembers = new Dictionary<long, DataControlTeamMember>();
        //private List<DataControlTeamMember> m_Members = new List<DataControlTeamMember>();

        private int GetControlTeamMemberID()
        {
            string strSQL = "select max(id) from ControlTeamMembers";                       
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null || arrResult.Count == 0)
                return 0;

            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        private int GetControlTeamBasicMemberID()
        {
            string strSQL = "select max(id) from ControlTeamBasicMembers";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null || arrResult.Count == 0)
                return 0;

            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        /// <summary>
        /// 단일 멤버 저장
        /// </summary>
        /// <param name="key"></param>
        public void SaveControlTeamMember(DataControlRoom room, DataControlTeam team, DataControlTeamJobPosition job)
        {
            long key = MakeControlTeamMemberKey(room, team, job);

            if (m_dicControlMembers.ContainsKey(key) == false)
                return;

            DataControlTeamMember data = m_dicControlMembers[key];

            StringBuilder sb = new StringBuilder();

            int nBasicMemberID = -1;
            bool isBlankToBasicMember = false;

            string strIFNull = m_dbMgr.DatabaseType == DBUtility.WebDBManager.DBType.sqlserver ? "ISNULL" : "IFNULL";

            sb.Append("SELECT " + strIFNull + "(MAX(ID), -1) ");
            sb.Append("FROM ControlTeamBasicMembers ");
            sb.AppendFormat(" WHERE RoomID = {0} AND TeamID = {1} AND JobPosition = {2} AND MemberID IS NULL",
                data.Room.ID, data.Team.ID, data.JobPosition.ID);

            ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString(), 0);
            if (arrResult != null)
            {
                if (arrResult.Count > 0)
                {
                    nBasicMemberID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

                    if (nBasicMemberID > -1)
                    {
                        isBlankToBasicMember = true;
                    }
                }
            }

            sb.Clear();

            if (data.ID <= 0)
            {
                int nID = GetControlTeamMemberID();
                data.ID = nID + 1;
                if (data.Member != null)
                {
                    sb.Append("INSERT INTO ControlTeamMembers (ID, RoomID, TeamID, JobPosition, MemberType,MemberID,Description) VALUES ");
                    sb.AppendFormat(" ({0}, {1}, {2}, {3}, {4}, {5}, '{6}') ", data.ID, data.Room.ID, data.Team.ID, data.JobPosition.ID, (int)data.MemberType, data.Member.ID, "");

                    if (isBlankToBasicMember)
                        sb.AppendFormat("UPDATE ControlTeamBasicMembers SET MemberID = {0} WHERE ID = {1} ", data.Member.ID, nBasicMemberID);

                }
                else
                {
                    sb.Append("INSERT INTO ControlTeamMembers (ID, RoomID, TeamID, JobPosition, MemberType,MemberID,Description) VALUES ");
                    sb.AppendFormat(" ({0}, {1}, {2}, {3}, {4}, {5}, '{6}') ", data.ID, data.Room.ID, data.Team.ID, data.JobPosition.ID, (int)data.MemberType, "NULL", "");
                }
            }
            else
            {
                if (data.Member != null)
                {
                    sb.AppendFormat("UPDATE ControlTeamMembers SET MemberType = {0} , MemberID = {1} ", (int)data.MemberType, data.Member.ID);
                    sb.AppendFormat(" WHERE ID = {0}", data.ID);

                    if (isBlankToBasicMember)
                        sb.AppendFormat("UPDATE ControlTeamBasicMembers SET MemberID = {0} WHERE ID = {1} ", data.Member.ID, nBasicMemberID);

                }
                else
                {
                    sb.AppendFormat("UPDATE ControlTeamMembers SET MemberType = {0} , MemberID = NULL ", (int)data.MemberType);
                    sb.AppendFormat(" WHERE ID = {0}", data.ID);
                }
            }
            string strSQL = sb.ToString();
            m_dbMgr.GetResultData(strSQL, 0);
        }

        /// <summary>
        /// 단일 멤버 저장 (기본데이터)
        /// 저장 전의 값이 기본 근무조원데이터와 동일한 근무조원데이터가 있으면 같이 변경시켜줌.
        /// </summary>
        /// <param name="key"></param>
        public void SaveControlTeamBasicMember(DataControlRoom room, DataControlTeam team, DataControlTeamJobPosition job)
        {
            long key = MakeControlTeamMemberKey(room, team, job);

            if (m_dicControlBasicMembers.ContainsKey(key) == false)
                return;

            DataControlTeamMember data = m_dicControlBasicMembers[key];
            StringBuilder sb = new StringBuilder();

            int nSameMemberID = -1;
            bool bHasSameMember = false;

            string strIFNull = m_dbMgr.DatabaseType == DBUtility.WebDBManager.DBType.sqlserver ? "ISNULL" : "IFNULL";

            string strSameMemberFindSQL = "SELECT " + strIFNull + "(MAX(A.ID), -1),";
            strSameMemberFindSQL += " A.MemberID,";
            strSameMemberFindSQL += " B.MemberID ";
            strSameMemberFindSQL += " FROM ControlTeamMembers AS A INNER JOIN ControlTeamBasicMembers AS B ON (A.RoomID = B.RoomID AND A.TeamID = B.TeamID AND A.JobPosition = B.JobPosition AND A.MemberType = B.MemberType )";
            strSameMemberFindSQL += String.Format(" WHERE A.RoomID = {0} AND A.TeamID = {1} AND A.JobPosition = {2}  AND A.MemberType = {3}",
                data.Room.ID, data.Team.ID, data.JobPosition.ID, (int)data.MemberType);
            strSameMemberFindSQL += " GROUP BY A.MemberID, B.MemberID ";

            ArrayList arrResult = m_dbMgr.GetResultData(strSameMemberFindSQL, 0);
            if (arrResult != null)
            {
                if (arrResult.Count > 0)
                {
                    nSameMemberID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

                    if (WebDBManager.GetIntField(arrResult[1].ToString(), -1) == WebDBManager.GetIntField(arrResult[2].ToString(), -1))
                    {
                        bHasSameMember = true;
                    }
                }
            }

            /*if (nSameMemberID == -1)
            {
                sb.Append("INSERT INTO ControlTeamMembers (ID, RoomID, TeamID, JobPosition, MemberType,MemberID,Description) VALUES ");
                sb.AppendFormat(" ({0}, {1}, {2}, {3}, {4}, {5}, '{6}') ", GetControlTeamMemberID() + 1, data.Room.ID, data.Team.ID, data.JobPosition.ID, (int)data.MemberType, (data.Member != null) ? data.Member.ID.ToString() : "NULL", "");
            }
            else if (bHasSameMember)
            {
                sb.AppendFormat("UPDATE ControlTeamMembers SET MemberID = {0} ", (data.Member != null) ? data.Member.ID.ToString() : "NULL");
                sb.AppendFormat(" WHERE ID = {0}", nSameMemberID);
            }*/


            if (data.ID <= 0)
            {
                int nID = GetControlTeamBasicMemberID();
                data.ID = nID + 1;
                if (data.Member != null)
                {
                    sb.Append("INSERT INTO ControlTeamBasicMembers (ID, RoomID, TeamID, JobPosition, MemberType,MemberID,Description) VALUES ");
                    sb.AppendFormat(" ({0}, {1}, {2}, {3}, {4}, {5}, '{6}') ", data.ID, data.Room.ID, data.Team.ID, data.JobPosition.ID, (int)data.MemberType, data.Member.ID, "");
                }
                else
                {
                    sb.Append("INSERT INTO ControlTeamBasicMembers (ID, RoomID, TeamID, JobPosition, MemberType,MemberID,Description) VALUES ");
                    sb.AppendFormat(" ({0}, {1}, {2}, {3}, {4}, {5}, '{6}') ", data.ID, data.Room.ID, data.Team.ID, data.JobPosition.ID, (int)data.MemberType, "NULL", "");
                }
            }
            else
            {
                if (data.Member != null)
                {
                    sb.AppendFormat("UPDATE ControlTeamBasicMembers SET MemberType = {0} , MemberID = {1} ", (int)data.MemberType, data.Member.ID);
                    sb.AppendFormat(" WHERE ID = {0}", data.ID);
                }
                else
                {
                    sb.AppendFormat("UPDATE ControlTeamBasicMembers SET MemberType = {0} , MemberID = NULL ", (int)data.MemberType);
                    sb.AppendFormat(" WHERE ID = {0}", data.ID);
                }
            }
            string strSQL = sb.ToString();
            m_dbMgr.GetResultData(strSQL, 0);
        }

        /// <summary>
        /// 전체 멤버 저장
        /// </summary>
        public void SaveControlTeamMembers()
        {     
            foreach (KeyValuePair<long, DataControlTeamMember> pair in m_dicControlMembers)
            {
                DataControlTeamMember data = pair.Value;

                StringBuilder sb = new StringBuilder();
                if (data.ID <= 0)
                {
                    int nID = GetControlTeamMemberID();
                    data.ID = nID + 1;
                    if (data.Member != null)
                    {
                        sb.Append("INSERT INTO ControlTeamMembers (ID, RoomID, TeamID, JobPosition, MemberType,MemberID,Description) VALUES ");
                        sb.AppendFormat(" ({0}, {1}, {2}, {3}, {4}, {5}, '{6}') ", data.ID, data.Room.ID, data.Team.ID, data.JobPosition.ID, (int)data.MemberType, data.Member.ID, "");
                    }
                    else
                    {
                        sb.Append("INSERT INTO ControlTeamMembers (ID, RoomID, TeamID, JobPosition, MemberType,MemberID,Description) VALUES ");
                        sb.AppendFormat(" ({0}, {1}, {2}, {3}, {4}, {5}, '{6}') ", data.ID, data.Room.ID, data.Team.ID, data.JobPosition.ID, (int)data.MemberType, "NULL", "");
                    }
                }
                else
                {                   
                    if (data.Member != null)
                    {
                        sb.AppendFormat("UPDATE ControlTeamMembers SET MemberType = {0} , MemberID = {1} ", (int)data.MemberType, data.Member.ID);
                        sb.AppendFormat(" WHERE ID = {0}", data.ID);
                    }
                    else
                    {
                        sb.AppendFormat("UPDATE ControlTeamMembers SET MemberType = {0} , MemberID = NULL ", (int)data.MemberType);
                        sb.AppendFormat(" WHERE ID = {0}", data.ID);
                    }                   
                }
                string strSQL = sb.ToString();
                m_dbMgr.GetResultData(strSQL, 0);
            }
        }

        /// <summary>
        /// 전체 멤버 저장 (기준데이터)
        /// </summary>
        public void SaveControlTeamBasicMembers()
        {
            foreach (KeyValuePair<long, DataControlTeamMember> pair in m_dicControlBasicMembers)
            {
                DataControlTeamMember data = pair.Value;

                StringBuilder sb = new StringBuilder();
                if (data.ID <= 0)
                {
                    int nID = GetControlTeamBasicMemberID();
                    data.ID = nID + 1;
                    if (data.Member != null)
                    {
                        sb.Append("INSERT INTO ControlTeamBasicMembers (ID, RoomID, TeamID, JobPosition, MemberType,MemberID,Description) VALUES ");
                        sb.AppendFormat(" ({0}, {1}, {2}, {3}, {4}, {5}, '{6}') ", data.ID, data.Room.ID, data.Team.ID, data.JobPosition.ID, (int)data.MemberType, data.Member.ID, "");
                    }
                    else
                    {
                        sb.Append("INSERT INTO ControlTeamBasicMembers (ID, RoomID, TeamID, JobPosition, MemberType,MemberID,Description) VALUES ");
                        sb.AppendFormat(" ({0}, {1}, {2}, {3}, {4}, {5}, '{6}') ", data.ID, data.Room.ID, data.Team.ID, data.JobPosition.ID, (int)data.MemberType, "NULL", "");
                    }
                }
                else
                {
                    if (data.Member != null)
                    {
                        sb.AppendFormat("UPDATE ControlTeamBasicMembers SET MemberType = {0} , MemberID = {1} ", (int)data.MemberType, data.Member.ID);
                        sb.AppendFormat(" WHERE ID = {0}", data.ID);
                    }
                    else
                    {
                        sb.AppendFormat("UPDATE ControlTeamBasicMembers SET MemberType = {0} , MemberID = NULL ", (int)data.MemberType);
                        sb.AppendFormat(" WHERE ID = {0}", data.ID);
                    }
                }
                string strSQL = sb.ToString();
                m_dbMgr.GetResultData(strSQL, 0);
            }
        }

        private long MakeControlTeamMemberKey(DataControlRoom room, DataControlTeam team, DataControlTeamJobPosition job)
        {
            ushort nRoomID = room == null ? ushort.MaxValue : (ushort)room.ID;
            ushort nTeamID = team == null ? ushort.MaxValue : (ushort)team.ID;
            ushort nJobID = job == null ? ushort.MaxValue : (ushort)job.ID;

            long key = (((long)nRoomID) << 32) | (((long)nTeamID) << 16) | (long)nJobID;
            return key;
        }

        private long MakeControlTeamMemberKey(int nRoomID, int nTeamID, int nJobID)
        {
            ushort _nRoomID = (ushort)nRoomID;
            ushort _nTeamID = (ushort)nTeamID;
            ushort _nJobID = (ushort)nJobID;

            long key = (((long)_nRoomID) << 32) | (((long)_nTeamID) << 16) | (long)_nJobID;
            return key;
        }

        public DataControlTeamMember GetControlTeamMember(DataControlRoom room, DataControlTeam team, DataControlTeamJobPosition job)
        {
            long key = MakeControlTeamMemberKey(room, team, job);
            DataControlTeamMember member = null;

            if (m_dicControlMembers.TryGetValue(key, out member))
                return member;

            return null;
        }

        public DataControlTeamMember GetControlTeamBasicMember(DataControlRoom room, DataControlTeam team, DataControlTeamJobPosition job)
        {
            long key = MakeControlTeamMemberKey(room, team, job);
            DataControlTeamMember member = null;

            if (m_dicControlBasicMembers.TryGetValue(key, out member))
                return member;

            return null;
        }

        private void LoadControlMembers()
        {
            m_dicControlMembers.Clear();

            //List<DataControlRoom> rooms = GetControlRooms();
            //List<DataControlTeam> teams = GetControlTeams();
            //List<DataControlTeamJobPosition> jobs = GetJobPositions();

            string strRoomIDs = "";

            foreach (KeyValuePair<int, DataControlRoomType> pair in m_dicRoomTypes)
            {
                List<DataControlRoom> rooms = GetControlRooms(pair.Value);
                List<DataControlTeam> teams = GetControlTeams(pair.Value);
                List<DataControlTeamJobPosition> jobs = GetJobPositions(pair.Value);

                foreach (DataControlRoom room in rooms)
                {
                    if (strRoomIDs.Length == 0)
                        strRoomIDs = room.ID.ToString();
                    else
                        strRoomIDs += ", " + room.ID.ToString();

                    foreach (DataControlTeam team in teams)
                    {
                        foreach (DataControlTeamJobPosition job in jobs)
                        {
                            DataControlTeamMember data = new DataControlTeamMember();
                            data.Room = room;
                            data.Team = team;
                            data.JobPosition = job;

                            long key = MakeControlTeamMemberKey(room, team, job);
                            m_dicControlMembers[key] = data;
                        }
                    }
                }
            }

            /*foreach(DataControlRoom room in rooms)
            {
                if (strRoomIDs.Length == 0)
                    strRoomIDs = room.ID.ToString();
                else
                    strRoomIDs += ", " + room.ID.ToString();

                foreach (DataControlTeam team in teams)
                {
                    foreach (DataControlTeamJobPosition job in jobs)
                    {
                        DataControlTeamMember data = new DataControlTeamMember();
                        data.Room = room;
                        data.Team = team;
                        data.JobPosition = job;

                        long key = MakeControlTeamMemberKey(room, team, job);
                        m_dicControlMembers[key] = data;
                    }
                }
            }*/
            
            if (strRoomIDs.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT ID, RoomID, TeamID, JobPosition, MemberType, MemberID FROM ControlTeamMembers ");
                sb.AppendFormat(" WHERE RoomID in ({0})", strRoomIDs);

                string strSQL = sb.ToString();
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
                if (arrResult != null && arrResult.Count > 0)
                {
                    int nCount = arrResult.Count;

                    for (int i = 0; i < nCount - 5; i += 6)
                    {
                        int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                        int nRoomID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                        int nTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                        int nJobID = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                        int nMemberType = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                        int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);

                        DataControlTeamMember.ControlMemberType memberType = DataControlTeamMember.ToMemberType(nMemberType);

                        if (memberType == DataControlTeamMember.ControlMemberType.None)
                            continue;

                        long key = MakeControlTeamMemberKey(nRoomID, nTeamID, nJobID);
                        DataControlTeamMember data;

                        if (!m_dicControlMembers.TryGetValue(key, out data))
                            continue;

                        DataCompanyMember member = null;

                        if (nMemberType == (int)DataControlTeamMember.ControlMemberType.RegularMember)
                            member = GetCompanyMember(nMemberID);
                        else if (nMemberType == (int)DataControlTeamMember.ControlMemberType.ExternalMember)
                            member = GetExternalMember(nMemberID);

                        data.ID = nID;
                        data.Member = member;
                        data.MemberType = memberType;
                    }
                }      
            }
        }

        private void LoadControlBasicMembers()
        {
            m_dicControlBasicMembers.Clear();

            string strRoomIDs = "";

            foreach (KeyValuePair<int, DataControlRoomType> pair in m_dicRoomTypes)
            {
                List<DataControlRoom> rooms = GetControlRooms(pair.Value);
                List<DataControlTeam> teams = GetControlTeams(pair.Value);
                List<DataControlTeamJobPosition> jobs = GetJobPositions(pair.Value);

                foreach (DataControlRoom room in rooms)
                {
                    if (strRoomIDs.Length == 0)
                        strRoomIDs = room.ID.ToString();
                    else
                        strRoomIDs += ", " + room.ID.ToString();

                    foreach (DataControlTeam team in teams)
                    {
                        foreach (DataControlTeamJobPosition job in jobs)
                        {
                            DataControlTeamMember data = new DataControlTeamMember();
                            data.Room = room;
                            data.Team = team;
                            data.JobPosition = job;

                            if (room.LocationName == "통합방재센터")
                                data.MemberType = DataControlTeamMember.ControlMemberType.ExternalMember;

                            long key = MakeControlTeamMemberKey(room, team, job);
                            m_dicControlBasicMembers[key] = data;
                        }
                    }
                }
            }

            if (strRoomIDs.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT ID, RoomID, TeamID, JobPosition, MemberType, MemberID FROM ControlTeamBasicMembers ");
                sb.AppendFormat(" WHERE RoomID in ({0})", strRoomIDs);

                string strSQL = sb.ToString();
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
                if (arrResult != null && arrResult.Count > 0)
                {
                    int nCount = arrResult.Count;

                    for (int i = 0; i < nCount - 5; i += 6)
                    {
                        int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                        int nRoomID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                        int nTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                        int nJobID = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                        int nMemberType = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                        int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);

                        DataControlTeamMember.ControlMemberType memberType = DataControlTeamMember.ToMemberType(nMemberType);

                        if (memberType == DataControlTeamMember.ControlMemberType.None)
                            continue;

                        long key = MakeControlTeamMemberKey(nRoomID, nTeamID, nJobID);
                        DataControlTeamMember data;

                        if (!m_dicControlBasicMembers.TryGetValue(key, out data))
                            continue;

                        DataCompanyMember member = null;

                        if (nMemberType == (int)DataControlTeamMember.ControlMemberType.RegularMember)
                            member = GetCompanyMember(nMemberID);
                        else if (nMemberType == (int)DataControlTeamMember.ControlMemberType.ExternalMember)
                            member = GetExternalMember(nMemberID);

                        data.ID = nID;
                        data.Member = member;
                        data.MemberType = memberType;
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public DataCompanyMember GetExternalMember(int nMemberID)
        {
            DataCompanyMember member = null;

            if (m_dicExternalMembers.TryGetValue(nMemberID, out member))
                return member;

            return null;
        }

        public DataCompanyMember GetCompanyMember(int nMemberID)
        {
            if (m_dicRegularMembers.ContainsKey(nMemberID))
            {
                return m_dicRegularMembers[nMemberID];
            }
            return null;
        }

        private DataTeam GetCompany(Dictionary<int, DataTeam> dicTeams)
        {
            foreach (KeyValuePair<int, DataTeam> pair in dicTeams)
            {
                if (pair.Value.IsCompany)
                    return pair.Value;
            }
            return null;
        }

        private DataTeam GetCompany(ArrayList arrCompanies, int nCompanyID)
        {
            foreach (DataTeam team in arrCompanies)
            {
                if (team.ID == nCompanyID)
                    return team;
            }
            return null;
        }

        private DataTeam LoadRegularTeam(DBUtility.WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
            //string szSQL = "SELECT R.ID, R.TeamName, R.ParentTeamID FROM RegularTeam as R";

            string strSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", m_nSiteID);
            ArrayList arrResult1 = dbMgr.GetResultData(strSQL, 0);
            if (arrResult1 == null || arrResult1.Count == 0)
                return null;

            int nTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if (nTeamID == -1)
                return null;

            ArrayList arrResult = ExecuteTeamList(dbMgr, nTeamID);
            //strSQL = string.Format("sp_TeamList2 {0}", nTeamID);
            //ArrayList arrResult = dbMgr.GetStoredProcedureData(strSQL, 0);
            if (arrResult == null || arrResult.Count == 0)
                return null;

            // 자신의 Team, 부모 팀의 ID
            Dictionary<DataTeam, int> dicParentID = new Dictionary<DataTeam, int>();

            int nCount = arrResult.Count;

            for (int i = 0; i < nCount - 2; i += 3)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szTeamName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                DataTeam data = new DataTeam();
                data.ID = nID;
                data.TeamName = szTeamName;
                data.External = false;

                dicTeams[nID] = data;
                dicParentID[data] = nParentTeamID;
            }

            DataTeam teamRoot = null;
            foreach (KeyValuePair<DataTeam, int> pair in dicParentID)
            {
                if (pair.Value < 0)
                {
                    teamRoot = pair.Key;
                    teamRoot.IsCompany = true;
                    continue;
                }

                if (!dicTeams.ContainsKey(pair.Value))
                    continue;

                DataTeam teamParent = dicTeams[pair.Value];
                pair.Key.ParentTeam = teamParent;
            }
            return teamRoot;
        }

        public static ArrayList ExecuteTeamList(WebDBManager dbMgr, int nRootTeamID, string strTableName = "RegularTeam")
        {
            string strSQL = "Select ID, TeamName, ParentTeamID from " + strTableName + " order by ParentTeamID, ID";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            if (nRootTeamID == 0)
                return arrResult;

            int nResultCount = arrResult.Count;

            ArrayList arrNewResult = new ArrayList();
            Dictionary<int, int> dicParentID = new Dictionary<int, int>();

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                VariousData<int> parentID = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                if (dicParentID.Count == 0)
                {
                    if (nID == nRootTeamID)
                    {
                        dicParentID[nID] = nID;

                        arrNewResult.Add(arrResult[i]);
                        arrNewResult.Add(arrResult[i + 1]);
                        arrNewResult.Add(arrResult[i + 2]);
                    }
                }
                else
                {
                    if (parentID == null)
                        continue;

                    if (dicParentID.ContainsKey(parentID.Data))
                    {
                        dicParentID[nID] = nID;

                        arrNewResult.Add(arrResult[i]);
                        arrNewResult.Add(arrResult[i + 1]);
                        arrNewResult.Add(arrResult[i + 2]);
                    }
                }
            }

            return arrNewResult;
        }

        public bool LoadCompanyMember(DBUtility.WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
            string strSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", m_nSiteID);
            ArrayList arrResult1 = dbMgr.GetResultData(strSQL, 0);
            if (arrResult1 == null || arrResult1.Count == 0)
                return false;

            int nTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if (nTeamID == -1)
                return false;

            ArrayList arrResult2 = ExecuteTeamList(dbMgr, nTeamID);
            //strSQL = string.Format("sp_TeamList2 {0}", nTeamID);
            //ArrayList arrResult2 = dbMgr.GetStoredProcedureData(strSQL, 0);
            if (arrResult2 == null || arrResult2.Count == 0)
                return false;

            string szTeamList = "";
            for (int i = 0; i < arrResult2.Count - 2; i += 3)
            {
                string szTeamID = WebDBManager.GetStringField(arrResult2[i].ToString(), "");
                if (szTeamList != "")
                {
                    szTeamList += ",";
                }
                szTeamList += szTeamID;
            }

            if (szTeamList == "")
            {
                return false;
            }

            Dictionary<int, string> dicJobSubPositions = LoadJobSubPosition(dbMgr);

            string szText = "select rm.RegularTeamID, rm.CompanyMemberID, rm.PositionID, rm.SubPositionID, MemberName, LevelID, MemberID, OfficePhoneNumber, PhoneNumber " +
                            " FROM CompanyMember as cm, RegularMemberList as rm WHERE cm.ID = rm.CompanyMemberID and rm.RegularTeamID in ({0})";

            if (String.IsNullOrWhiteSpace(szTeamList) == true)
                szTeamList = "-1";

            strSQL = string.Format(szText, szTeamList);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            DataCompanyMember member;

            for (int i = 0; i < nCount - 8; i += 9)
            {
                int nRegularTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                int nPositionID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                int nSubPositionID = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0);
                string strMemberName = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], "");
                int nLevelID = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
                string strMemberID = DBUtility.WebDBManager.GetStringField(arrResult[i + 6], "");
                //int nSecondRegularTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
                //int nSecondPositionID = DBUtility.WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                string strOfficePhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 7], "");
                string strPhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 8], "");

                if (string.Compare(strPhoneNumber, "null", true) == 0 || strPhoneNumber == "")
                    strPhoneNumber = "";
                else
                    strPhoneNumber = DBUtility.AES256Cipher.AES_decrypt(strPhoneNumber, key);

                strPhoneNumber = ValidPhoneNumber(strPhoneNumber);

                if (string.Compare(strOfficePhoneNumber, "null", true) == 0)
                    strOfficePhoneNumber = "";

                if (!dicTeams.ContainsKey(nRegularTeamID))
                    continue;

                DataTeam team = dicTeams[nRegularTeamID];

                if (!m_dicRegularMembers.TryGetValue(nID, out member))
                {
                    member = new DataCompanyMember();

                    member.ID = nID;
                    member.MemberName = strMemberName;
                    member.LevelID = nLevelID;
                    member.MemberID = strMemberID;
                    member.OfficePhoneNumber = strOfficePhoneNumber;
                    member.PhoneNumber = strPhoneNumber;

                    m_dicRegularMembers[nID] = member;
                }

                ArrayList arrMembers = null;
                if (m_dicRegularTeamMembers.ContainsKey(team))
                    arrMembers = m_dicRegularTeamMembers[team];
                else
                {
                    arrMembers = new ArrayList();
                    m_dicRegularTeamMembers[team] = arrMembers;
                }

                string strSubPositionName = "";

                if (dicJobSubPositions != null)
                {
                    if (!dicJobSubPositions.TryGetValue(nSubPositionID, out strSubPositionName))
                        strSubPositionName = "";
                }

                arrMembers.Add(member);
                member.TeamPositions[team] = new JobPosition(nPositionID, strSubPositionName);
                ////////////////////////////////////////////////////////////////
            }

            foreach (KeyValuePair<DataTeam, ArrayList> pair in m_dicRegularTeamMembers)
            {
                pair.Value.Sort();
            }
            return true;
        }

        private Dictionary<int, string> LoadJobSubPosition(WebDBManager dbMgr)
        {
            string strSQL = "Select ID, Name from JobSubPosition";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            Dictionary<int, string> dicJobSubPositions = new Dictionary<int, string>();
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strSubPositionName = WebDBManager.GetStringField(arrResult[i + 1], "");

                if (nID < 0)
                    continue;

                dicJobSubPositions[nID] = strSubPositionName;
            }

            return dicJobSubPositions;
        }
        
        private string ValidPhoneNumber(string strPhoneNumber)
        {
            string strResult = "";
            int nLen = strPhoneNumber.Length;

            for (int i = 0; i < nLen; i++)
            {
                char ch = strPhoneNumber[i];

                if (ch != ' ' && ch != '\t' && ch != '-')
                    strResult += ch;
            }
            return strResult;
        }

        public List<DataCompanyMember> SearchCompanyMembers(string strMemberName)
        {
            List<DataCompanyMember> members = new List<DataCompanyMember>();

            if (strMemberName.Length > 0)
            {
                foreach (KeyValuePair<int, DataCompanyMember> pair in m_dicRegularMembers)
                {
                    if (pair.Value.MemberName.Contains(strMemberName))
                    {
                        members.Add(pair.Value);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<DataTeam, ArrayList> pair in m_dicRegularTeamMembers)
                {
                    foreach (DataCompanyMember member in pair.Value)
                    {
                        members.Add(member);
                    }
                }
            }

            return members;
        }

        public List<DataCompanyMember> SearchExternalCompanyMembers(string strMemberName)
        {
            List<DataCompanyMember> members = new List<DataCompanyMember>();

            if (strMemberName.Length > 0)
            {
                foreach (KeyValuePair<int, DataCompanyMember> pair in m_dicExternalMembers)
                {
                    if (pair.Value.MemberName.Contains(strMemberName))
                    {
                        members.Add(pair.Value);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<DataTeam, ArrayList> pair in m_dicExternalTeamMembers)
                {
                    foreach (DataCompanyMember member in pair.Value)
                    {
                        members.Add(member);
                    }
                }
            }

            return members;
        }

        private string LoadExternalTeam(DBUtility.WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
            string strExternalTeamIDs = "";

            string strSQL = string.Format("SELECT ID, TeamName, ParentTeamID FROM ExternalTeam WHERE SiteID = {0}", m_nSiteID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null || arrResult.Count == 0)
                return strExternalTeamIDs;

            // 자신의 Team, 부모 팀의 ID
            Dictionary<DataTeam, int> dicParentID = new Dictionary<DataTeam, int>();

            int nCount = arrResult.Count;

            for (int i = 0; i < nCount - 2; i += 3)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szTeamName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                DataTeam data = new DataTeam();
                data.ID = nID;
                data.TeamName = szTeamName;
                data.External = false;

                dicTeams[nID] = data;
                dicParentID[data] = nParentTeamID;

                if (strExternalTeamIDs.Length == 0)
                    strExternalTeamIDs = nID.ToString();
                else
                    strExternalTeamIDs += ", " + nID.ToString();
            }

            foreach (KeyValuePair<DataTeam, int> pair in dicParentID)
            {
                DataTeam team = null;

                if (!dicTeams.TryGetValue(pair.Value, out team))
                    team = null;

                pair.Key.ParentTeam = team;
            }

            return strExternalTeamIDs;
        }

        private bool LoadExternalCompanyMember(DBUtility.WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams, string strExternalTeamIDs)
        {
            Dictionary<int, string> dicExternalJobPositions = LoadExternalJobPosition(dbMgr);

            string szText = "select eml.ExternalCompanyTeamID, eml.ExternalCompanyMemberID, eml.JobPositionID, ecm.Name " +
                            " FROM ExternalCompanyMember as ecm, ExternalMemberList as eml WHERE ecm.ID = eml.ExternalCompanyMemberID and eml.ExternalCompanyTeamID in ({0})";

            if (String.IsNullOrWhiteSpace(strExternalTeamIDs) == true)
                strExternalTeamIDs = "-1";

            string strSQL = string.Format(szText, strExternalTeamIDs);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            DataCompanyMember member;

            for (int i = 0; i < nCount - 3; i += 4)
            {
                int nExternalTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                int nPositionID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                string strMemberName = DBUtility.WebDBManager.GetStringField(arrResult[i + 3], "");

                if (!dicTeams.ContainsKey(nExternalTeamID))
                    continue;

                DataTeam team = dicTeams[nExternalTeamID];

                if (!m_dicExternalMembers.TryGetValue(nID, out member))
                {
                    member = new DataCompanyMember();

                    member.ID = nID;
                    member.MemberName = strMemberName;
                    
                    m_dicExternalMembers[nID] = member;
                }

                ArrayList arrMembers = null;
                if (m_dicExternalTeamMembers.ContainsKey(team))
                    arrMembers = m_dicExternalTeamMembers[team];
                else
                {
                    arrMembers = new ArrayList();
                    m_dicExternalTeamMembers[team] = arrMembers;
                }

                string strPositionName = "";

                if (dicExternalJobPositions != null)
                {
                    if (!dicExternalJobPositions.TryGetValue(nPositionID, out strPositionName))
                        strPositionName = "";
                }

                arrMembers.Add(member);
                member.TeamPositions[team] = new JobPosition(nPositionID, strPositionName);
                ////////////////////////////////////////////////////////////////
            }

            foreach (KeyValuePair<DataTeam, ArrayList> pair in m_dicRegularTeamMembers)
            {
                pair.Value.Sort();
            }
            return true;
        }

        private Dictionary<int, string> LoadExternalJobPosition(WebDBManager dbMgr)
        {
            string strSQL = "Select ID, PositionName from ExternalJobPosition";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            Dictionary<int, string> dicExternalJobPositions = new Dictionary<int, string>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strPositionName = WebDBManager.GetStringField(arrResult[i + 1], "");

                if (nID < 0)
                    continue;

                dicExternalJobPositions[nID] = strPositionName;
            }

            return dicExternalJobPositions;
        }
    }

}
