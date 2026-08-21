using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using DBUtility;

namespace UnE
{
    namespace SOP
    {
        public class SOPManager : UnE.SOP.Data.ISOPDataContainer
        {
            private WebDBManager m_dbMgr = null;

            // FullPath(Category/SubCategory/Disaster), DisasterInfo
            private Dictionary<string, DisasterInfo> m_dicSOPRegularNormal = new Dictionary<string, DisasterInfo>();
            private Dictionary<string, DisasterInfo> m_dicSOPRegularEmergency = new Dictionary<string, DisasterInfo>();
            private Dictionary<string, DisasterInfo> m_dicSOPNonRegularNormal = new Dictionary<string, DisasterInfo>();
            private Dictionary<string, DisasterInfo> m_dicSOPNonRegularEmergency = new Dictionary<string, DisasterInfo>();

            // Disaster, FullPath(Category/SubCategory/Disaster)
            private Dictionary<DisasterInfo, string> m_dicDisasterSOPFullPath = new Dictionary<DisasterInfo, string>();

            // DiasterID, DisasterInfo
            private Dictionary<int, DisasterInfo> m_dicDisasterRegularNormal = new Dictionary<int, DisasterInfo>();
            private Dictionary<int, DisasterInfo> m_dicDisasterRegularEmergency = new Dictionary<int, DisasterInfo>();
            private Dictionary<int, DisasterInfo> m_dicDisasterNonRegularNormal = new Dictionary<int, DisasterInfo>();
            private Dictionary<int, DisasterInfo> m_dicDisasterNonRegularEmergency = new Dictionary<int, DisasterInfo>();

            // FullPath(Category/SubCategory/Disaster)별 모든 버전의 DisasterInfo Version
            private Dictionary<string, ArrayList> m_dicSOPRegularNormalDisasterList = new Dictionary<string, ArrayList>();
            private Dictionary<string, ArrayList> m_dicSOPRegularEmergencyDisasterList = new Dictionary<string, ArrayList>();
            private Dictionary<string, ArrayList> m_dicSOPNonRegularNormalDisasterList = new Dictionary<string, ArrayList>();
            private Dictionary<string, ArrayList> m_dicSOPNonRegularEmergencyDisasterList = new Dictionary<string, ArrayList>();

            // DisasterID, VersionInfo
            private Dictionary<int, VersionInfo> m_dicVersionRegularNormal = new Dictionary<int, VersionInfo>();
            private Dictionary<int, VersionInfo> m_dicVersionRegularEmergency = new Dictionary<int, VersionInfo>();
            private Dictionary<int, VersionInfo> m_dicVersionNonRegularNormal = new Dictionary<int, VersionInfo>();
            private Dictionary<int, VersionInfo> m_dicVersionNonRegularEmergency = new Dictionary<int, VersionInfo>();

            // ActionStepID, ActionStepInfo
            private Dictionary<int, ActionStepInfo> m_dicActionStepInfo = new Dictionary<int, ActionStepInfo>();
            // Key : 상위 4바이트(1이면 실제모드, 0이면 훈련모드), 하위 4바이트(ActionStepID)
            // Value : ActionStepHistory ID
            private Dictionary<long, int> m_dicActionStepHistory = new Dictionary<long, int>();
            private int m_nCurrentActionStepID = -1;
            private bool m_isCurrentRealMode = false;

            // private TreeNode m_prevSelectedNode = null;
            //private int m_nPrevSelectedRow = -1;
            private bool m_isRegular = true;
            private bool m_isNormal = true;

            private ArrayList m_arrCompanyMember = new ArrayList();
            private Dictionary<int, Data_CompanyMember> m_dicCompanyMember = new Dictionary<int, Data_CompanyMember>();

            private string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

            // 비상 조직 ID, 연결된 조직 또는 개인의 정보 List
            private Dictionary<int, List<TemporaryMember>> m_dicTemporaryNormalTeamID = new Dictionary<int, List<TemporaryMember>>();
            private Dictionary<int, List<TemporaryMember>> m_dicTemporaryEmergencyTeamID = new Dictionary<int, List<TemporaryMember>>();
            // Key : TemporaryMemberList ID
            private Dictionary<int, TemporaryMember> m_dicTemporaryMembers = new Dictionary<int, TemporaryMember>();
            // 비상 조직 ID, 연결된 상시 조직의 ID List
            //private Dictionary<int, ArrayList> m_dicTemporaryNormalTeamID = new Dictionary<int, ArrayList>();
            //private Dictionary<int, ArrayList> m_dicTemporaryEmergencyTeamID = new Dictionary<int, ArrayList>();
            // RegularTeam별 팀원 List
            // RegularTeam ID, Data_CompanyMember List
            private Dictionary<int, ArrayList> m_dicRegularTeamMember = new Dictionary<int, ArrayList>();
            private Dictionary<int, Data_RegularTeam> m_dicRegularTeam = new Dictionary<int, Data_RegularTeam>();
            private Dictionary<int, Data_ControlRoom> m_dicControlRoom = new Dictionary<int, Data_ControlRoom>();
            private Dictionary<int, Data_ControlRoomMember> m_dicControlRoomMembers = new Dictionary<int, Data_ControlRoomMember>();
            // 사용자 정의조직 ID, 연결된 정의 조직 Data
            private Dictionary<int, Data_ExternalTeam> m_dicUserDefinedTeam = new Dictionary<int, Data_ExternalTeam>();
            // 외부 조직 ID, 연결된 외부 조직 Data
            private Dictionary<int, Data_ExternalTeam> m_dicExternalTeam = new Dictionary<int, Data_ExternalTeam>();
            // 평일 비상조직 ID, 연결된 평일 비상조직 Data
            private Dictionary<int, Data_NormalTeam> m_dicTemporaryNormalTeam = new Dictionary<int, Data_NormalTeam>();
            // 야간 및 휴일 비상조직 ID, 연결된 야간 및 휴일 비상조직 Data
            private Dictionary<int, Data_EmergencyTeam> m_dicTemporaryEmergencyTeam = new Dictionary<int, Data_EmergencyTeam>();

            // 협력업체 팀들
            private ArrayList m_arrExternalCompanyTeams = new ArrayList();
            // 협력업체 팀원들
            private ArrayList m_arrExternalCompanyMembers = new ArrayList();

            private Dictionary<int, Data_SOPGenUser> m_dicSOPGenUsers = new Dictionary<int, Data_SOPGenUser>();

            // 담당자 및 연락처 관리자
            private RoleMemberManager m_roleMemberMgr = null;

            public List<Data_RegularTeam> RegularTeams
            {
                get
                {
                    List<Data_RegularTeam> list = new List<Data_RegularTeam>();
                    list.AddRange(m_dicRegularTeam.Values);
                    return list;
                }
            }

            public ArrayList ExternalCompanyTeams
            {
                get { return m_arrExternalCompanyTeams; }
            }

            public Dictionary<int, Data_ControlRoom> ControlRoom
            {
                get { return m_dicControlRoom; }
            }

            public Dictionary<int, Data_ControlRoomMember> ControlRoomMembers
            {
                get { return m_dicControlRoomMembers; }
            } 

            public ArrayList ExternalCompanyMembers
            {
                get { return m_arrExternalCompanyMembers; }
            }

            public List<Data_ExternalTeam> UserDefineTeams
            {
                get 
                {
                    List<Data_ExternalTeam> list = new List<Data_ExternalTeam>();
                    list.AddRange(m_dicUserDefinedTeam.Values);
                    return list;
                }
            }

            public RoleMemberManager RoleMemberManager
            {
                get { return m_roleMemberMgr; }
            }

            public void AddUserDefineTeam(Data_ExternalTeam team)
            {
                if(m_dicUserDefinedTeam.ContainsKey(team.ID))
                {
                    return;
                }

                m_dicUserDefinedTeam.Add(team.ID, team);
            }

            private ArrayList m_arrDisaster = new ArrayList();
            public ArrayList DisasterList
            {
                get { return m_arrDisaster; }
                set { m_arrDisaster = value; }
            }

            private bool m_isOpened = false;

            private int m_nSiteID = 1;
            public SOPManager(WebDBManager dbMgr)
            {
                m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

                m_dbMgr = dbMgr;
                m_roleMemberMgr = new RoleMemberManager(m_dbMgr, this);
            }

            public bool Load(bool isRegular, bool isNormal, bool openAlways = false)
            {
                if (m_isOpened && !openAlways)
                {
                    if (m_isRegular == isRegular && m_isNormal == isNormal)
                        return true;
                }

                m_isRegular = isRegular;
                m_isNormal = isNormal;

                if (!LoadCompanyMember())
                    return Cancel();

                if (!LoadVersion())
                    return Cancel();

                if (!LoadSOP())
                    return Cancel();

                if (!LoadTemporaryTeam())
                    return Cancel();

                if (!LoadOtherTeams())
                    return Cancel();

                if (!LoadControlRoom())
                    return Cancel();

                if (!LoadControlRoomMembers())
                    return Cancel();

                LoadExternalCompanyTeams();
                LoadExternalCompanyMembers();

                LoadSOPGenUsers();

                ReadDisasterCategory();

                m_isOpened = true;
                return true;
            }

            private void LoadSOPGenUsers(int nSOPGenUserID = -1)
            {
                string strSQL;

                if (nSOPGenUserID < 0)
                    strSQL = "Select ID, MemberID, UserLevel, UserID, NickName from SOPGenUser where SiteID = " + m_nSiteID.ToString();
                else
                    strSQL = "Select ID, MemberID, UserLevel, UserID, NickName from SOPGenUser where ID = " + nSOPGenUserID.ToString() + " and SiteID = " + m_nSiteID.ToString();

                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return;

                string strGenUserIDs = "";
                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 4; i += 5)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    int nUserLevel = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                    string strUserID = WebDBManager.GetStringField(arrResult[i + 3], "null");
                    string strNickName = WebDBManager.GetStringField(arrResult[i + 4], "null");

                    if (nID < 0)
                        continue;

                    Data_SOPGenUser user = new Data_SOPGenUser();

                    user.ID = nID;

                    if (nMemberID >= 0)
                        user.MemberID = nMemberID;

                    user.UserLevel = nUserLevel;
                    user.UserID = strUserID;
                    user.NickName = strNickName;

                    m_dicSOPGenUsers[nID] = user;

                    if (strGenUserIDs.Length == 0)
                        strGenUserIDs = nID.ToString();
                    else
                        strGenUserIDs += ", " + nID.ToString();
                }

                if (strGenUserIDs.Length > 0)
                {
                    strSQL = "Select SOPGenUserID, DayLight, MemberType, MemberID, DisplayText, CallerPhoneNumber from SOPGenUserCommander where SOPGenUserID in (" + strGenUserIDs + ")";
                    arrResult = m_dbMgr.GetResultData(strSQL, 0);

                    if (arrResult == null)
                        return;

                    nResultCount = arrResult.Count;
                    Dictionary<long, global::Sections.SectionCommander> dicSectionCommander = new Dictionary<long,global::Sections.SectionCommander>();

                    for (int i = 0; i < nResultCount - 5; i += 6)
                    {
                        nSOPGenUserID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                        int nDayLight = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                        int nMemberType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                        int nMemberID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                        string strDisplayText = WebDBManager.GetStringField(arrResult[i + 4]);
                        string strCallerPhoneNumber = WebDBManager.GetStringField(arrResult[i + 5]);

                        if (strDisplayText == null)
                            strDisplayText = "";

                        Data_SOPGenUser user;
                        if (!m_dicSOPGenUsers.TryGetValue(nSOPGenUserID, out user))
                            continue;

                        global::Sections.SectionCommander commander;
                        long key = (((long)nMemberType) << 32) | (long)nMemberID;

                        if (!dicSectionCommander.TryGetValue(key, out commander))
                        {
                            commander = ProxySOP.Instance.SOPContainer.LoadSectionCommander(nMemberType, nMemberID, strDisplayText);
                        }

                        if (commander == null)
                            continue;

                        dicSectionCommander[key] = commander;

                        if (nDayLight == 0)
                            user.NightCommander = commander;
                        else if (nDayLight == 1)
                            user.DayLightCommander = commander;

                        commander.CallerPhoneNumber = strCallerPhoneNumber;
                    }
                }
            }

            public Data_SOPGenUser GetSOPGenUser(int nID)
            {
                Data_SOPGenUser user;

                if (m_dicSOPGenUsers.TryGetValue(nID, out user))
                    return user;

                return null;
            }

            public Data_SOPGenUser LoadSOPGenUser(int nID)
            {
                LoadSOPGenUsers(nID);
                return GetSOPGenUser(nID);
            }

            public bool LoadRegularMember()
            {
                if (!LoadCompanyMember())
                    return Cancel();

                if (!LoadTemporaryTeam())
                    return Cancel();

                return true;
            }

            public bool LoadExternalCompany()
            {
                if(!LoadExternalCompanyTeams())
                {
                    return Cancel();
                }
                if(!LoadExternalCompanyMembers())
                {
                    return Cancel();
                }
                return true;

            }

            private bool LoadExternalCompanyTeams()
            {
                WebDBManager dbMgr = m_dbMgr;

                //string strSQL = "select id, TeamName, ParentTeamID, CompanyID from ExternalCompanyTeam";
                string szText = "SELECT et.ID, et.TeamName, et.ParentTeamID FROM ExternalTeam as et WHERE et.SiteID = {0}";
                
                string strSQL = string.Format(szText, m_nSiteID);

                ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return false;

                m_arrExternalCompanyTeams.Clear();

                int nResultCount = arrResult.Count;

                // TeamID, Child Team
                Dictionary<int, ExternalCompanyTeam> dicTeam = new Dictionary<int, ExternalCompanyTeam>();
                // TeamID, Parent TeamID
                Dictionary<int, int> dicParent = new Dictionary<int, int>();

                for (int i = 0; i < nResultCount - 2; i += 3)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                    int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                    //int nCompanyID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

                    if (nID < 0)
                        continue;

                    ExternalCompanyTeam team = new ExternalCompanyTeam();
                    team.ID = nID;
                    team.TeamName = strTeamName;
                    team.CompanyID = nParentTeamID;

                    dicTeam[nID] = team;

                    if (nParentTeamID < 0)
                    {
                        team.CompanyID = nID;
                        m_arrExternalCompanyTeams.Add(team);
                    }
                    else
                        dicParent[nID] = nParentTeamID;
                }

                foreach (KeyValuePair<int, int> pair in dicParent)
                {
                    if (!dicTeam.ContainsKey(pair.Key))
                        continue;

                    if (!dicTeam.ContainsKey(pair.Value))
                        continue;

                    ExternalCompanyTeam team = dicTeam[pair.Key];
                    ExternalCompanyTeam teamParent = dicTeam[pair.Value];
                    team.ParentTeam = teamParent;

                    team.CompanyID = teamParent.CompanyID;
                    m_arrExternalCompanyTeams.Add(team);
                }

                return true;
            }

            private bool LoadExternalCompanyMembers()
            {
                if (m_arrExternalCompanyTeams.Count == 0)
                    return false;

                //string strSQL = "select id, Name, PhoneNumber, IsTeamLeader, TeamID from ExternalCompanyMember";
                /*string szText = "SELECT ecm.id, ecm.Name, ecm.PhoneNumber, ecm.IsTeamLeader, ecm.TeamID FROM ExternalCompanyMember as ecm " +
                                "   INNER JOIN ExternalCompanyTeam AS ect ON ecm.TeamID = ect.ID " +
                                "   INNER JOIN ExternalTeam as et ON et.ID = ect.CompanyID AND et.SiteID = {0}";*/
                string szText = "select ExternalCompanyTeamID, ExternalCompanyMemberID, ecm.Name, ecm.PhoneNumber ";
                szText += "from ExternalCompanyMember as ecm, ExternalMemberList as eml, ExternalTeam as et ";
                szText += "where ecm.ID = eml.ExternalCompanyMemberID and eml.ExternalCompanyTeamID = et.ID and et.SiteID = {0}";

                string strSQL = string.Format(szText, m_nSiteID);

                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return false;

                m_arrExternalCompanyMembers.Clear();

                ExternalCompanyMember member;
                Dictionary<int, ExternalCompanyMember> dicMembers = new Dictionary<int, ExternalCompanyMember>();
                int nResultCount = arrResult.Count;

                string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

                for (int i = 0; i < nResultCount - 3; i += 4)
                {
                    int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    //bool isTeamLeader = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0) == 0 ? false : true;
                    string strName = WebDBManager.GetStringField(arrResult[i + 2], "");
                    string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 3], "");

                    if (nID < 0)
                        continue;

                    ExternalCompanyTeam team = FindExternalCompanyTeam(nTeamID);
                    if (team == null)
                        continue;

                    if (string.Compare(strPhoneNumber, "null", true) == 0)
                        strPhoneNumber = "";

                    try
                    {
                        if (strPhoneNumber.Length > 0)
                        {
                            strPhoneNumber = DBUtility.AES256Cipher.AES_decrypt(strPhoneNumber, key);
                            strPhoneNumber = ValidPhoneNumber(strPhoneNumber);
                        }
                    }
                    catch (System.Exception)
                    {
                    }

                    if (!dicMembers.TryGetValue(nID, out member))
                    {
                        member = new ExternalCompanyMember();

                        member.ID = nID;
                        member.MemberName = strName;
                        member.PhoneNumber = strPhoneNumber;

                        m_arrExternalCompanyMembers.Add(member);
                    }

                    member.Teams.Add(team);
                    team.Members.Add(member);
                    //member.TeamLeaders[team] = isTeamLeader;

                    /*ExternalCompanyMember member = new ExternalCompanyMember();
                    member.ID = nID;
                    member.MemberName = strName;
                    member.PhoneNumber = strPhoneNumber;
                    member.IsTeamLeader = isTeamLeader;
                    member.Team = team;

                    m_arrExternalCompanyMembers.Add(member);*/
                }

                return true;
            }

            public ExternalCompanyTeam FindExternalCompanyTeam(int nTeamID)
            {
                foreach (ExternalCompanyTeam team in m_arrExternalCompanyTeams)
                {
                    if (team.ID == nTeamID)
                        return team;
                }

                return null;
            }

            public List<ExternalCompanyTeam> GetExternalCompanyTeams(int nCompanyID)
            {
                List<ExternalCompanyTeam> teams = new List<ExternalCompanyTeam>();

                foreach (ExternalCompanyTeam team in m_arrExternalCompanyTeams)
                {
                    if (team.CompanyID == nCompanyID)
                        teams.Add(team);
                }

                return teams;
            }

            private void ReadDisasterCategory()
            {
                string strSql = "SELECT ID, CategoryName FROM DisasterCategory WHERE SiteID = " + m_nSiteID.ToString();
                ArrayList arrResult = m_dbMgr.GetResultData(strSql, 0);
                if (arrResult != null)
                {
                    for (int i = 0; i < arrResult.Count - 1; i += 2)
                    {
                        Data_DisasterCategory dataNew = new Data_DisasterCategory();
                        dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                        dataNew.CategoryName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");

                        m_arrDisaster.Add(dataNew);
                    }
                }                
            }

            private bool Cancel()
            {
                m_dicVersionRegularNormal.Clear();
                m_dicVersionRegularEmergency.Clear();
                m_dicVersionNonRegularNormal.Clear();
                m_dicVersionNonRegularEmergency.Clear();

                m_dicSOPRegularNormal.Clear();
                m_dicSOPRegularEmergency.Clear();
                m_dicSOPNonRegularNormal.Clear();
                m_dicSOPNonRegularEmergency.Clear();

                m_dicDisasterSOPFullPath.Clear();

                m_dicActionStepInfo.Clear();

                m_arrDisaster.Clear();

                m_isOpened = false;

                return false;
            }

            /*public bool GetDisasterInfo(int nDisasterID, out bool isNormal, out bool isRegular)
            {
                isNormal = isRegular = false;

                VersionInfo version;

                if (m_dicVersionRegularNormal.TryGetValue(nDisasterID, out version) ||
                    m_dicVersionRegularEmergency.TryGetValue(nDisasterID, out version) ||
                    m_dicVersionNonRegularNormal.TryGetValue(nDisasterID, out version) ||
                    m_dicVersionNonRegularEmergency.TryGetValue(nDisasterID, out version))
                {
                    isNormal = version.IsNormal;
                    isRegular = version.IsRegular;
                    return true;
                }

                string strSQL = "select v.IsNormal, v.IsRegular from Disaster as d, Version as v where d.VersionID = v.ID and d.ID = " + nDisasterID.ToString();
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null || arrResult.Count != 2)
                    return false;

                DBUtility.VariousData<int> normal = WebDBManager.GetIntField(arrResult[0].ToString());
                DBUtility.VariousData<int> regular = WebDBManager.GetIntField(arrResult[0].ToString());

                if (normal == null || regular == null)
                    return false;

                isNormal = normal.Data == 1;
                isRegular = regular.Data == 1;
                return true;
            }*/

            public string GetDisasterFullPath(DisasterInfo disaster)
            {
                Dictionary<DisasterInfo, string> dicDisasterFullPath = GetFullPathDictionary();

                string strFullPath;

                if (dicDisasterFullPath.TryGetValue(disaster, out strFullPath))
                    return strFullPath;

                return null;
            }

            public bool GetDisasterFullPath(DisasterInfo disaster, out string strCategoryName, out string strSubCategoryName)
            {
                Dictionary<DisasterInfo, string> dicDisasterFullPath = GetFullPathDictionary();

                string strFullPath;

                if (dicDisasterFullPath.TryGetValue(disaster, out strFullPath))
                {
                    char delimeter = (char)6;
                    int nIndex1 = strFullPath.IndexOf(delimeter);
                    int nIndex2 = strFullPath.IndexOf(delimeter, nIndex1 + 1);

                    if (nIndex1 >= 0 && nIndex2 > nIndex1)
                    {
                        strCategoryName = strFullPath.Substring(0, nIndex1).Trim();
                        strSubCategoryName = strFullPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();
                        return true;
                    }
                }

                strCategoryName = strSubCategoryName = "";
                return false;
            }

            public DisasterInfo GetDisasterFromActionStepID(int nActionStepID, out bool isNormal, out bool isRegular)
            {
                isNormal = isRegular = false;
                ActionStepInfo actionStep = GetActionStepInfo(nActionStepID);

                if (actionStep == null)
                {
                    if (!LoadDisasterActionStep(nActionStepID))
                        return null;

                    actionStep = GetActionStepInfo(nActionStepID);

                    if (actionStep == null)
                        return null;
                }

                return GetDisaster(actionStep.DisasterID, out isNormal, out isRegular);
            }

            // Version 변경으로 인하여 SOP FullPath와 Disaster 정보가 서로 맞지 않을수 있으므로
            // 이를 바로잡아준다.
            private void SetFullPath(DisasterInfo disaster, bool isNormal, bool isRegular)
            {
                string strFullPath;
                if (!m_dicDisasterSOPFullPath.TryGetValue(disaster, out strFullPath))
                    return;

                Dictionary<string, DisasterInfo> dicFullPathDisaster = null;

                if (isNormal)
                {
                    if (isRegular)
                        dicFullPathDisaster = m_dicSOPRegularNormal;
                    else
                        dicFullPathDisaster = m_dicSOPNonRegularNormal;
                }
                else
                {
                    if (isRegular)
                        dicFullPathDisaster = m_dicSOPRegularEmergency;
                    else
                        dicFullPathDisaster = m_dicSOPNonRegularEmergency;
                }

                dicFullPathDisaster[strFullPath] = disaster;
            }

            public DisasterInfo GetDisaster(int nDisasterID, out bool isNormal, out bool isRegular)
            {
                // 기존에 읽어놓은 데이터에 있으면 기존 데이터에서 꺼내온다.
                DisasterInfo disaster = _GetDisaster(nDisasterID, out isNormal, out isRegular);

                if (disaster != null)
                {
                    SetFullPath(disaster, isNormal, isRegular);
                    return disaster;
                }

                // 기존 데이터에서 못찾으면 DB에서 읽어온다.
                string szText = "SELECT ds.id, ds.DisasterName, sc.SubCategoryName, dc.CategoryName, ds.VersionID, v.isRegular, v.isNormal FROM disaster as ds " +
                                " INNER JOIN SubDisasterCategory as sc ON ds.SubDisasterID = sc.id " +
                                " INNER JOIN DisasterCategory as dc ON sc.DisasterID = dc.id  AND ds.ID = {0}" +
                                " INNER JOIN Version as v ON ds.VersionID = v.ID " +
                                " ORDER BY ds.DisasterName";

                string strSQL = string.Format(szText, nDisasterID);

                if (!LoadSOP(strSQL))
                    return null;

                disaster = _GetDisaster(nDisasterID, out isNormal, out isRegular);

                if (disaster != null)
                    SetFullPath(disaster, isNormal, isRegular);

                return disaster;
            }

            private DisasterInfo _GetDisaster(int nDisasterID, out bool isNormal, out bool isRegular)
            {
                isNormal = isRegular = false;

                DisasterInfo disaster;

                if (m_dicDisasterRegularNormal.TryGetValue(nDisasterID, out disaster))
                {
                    isNormal = isRegular = true;
                    return disaster;
                }
                else if (m_dicDisasterRegularEmergency.TryGetValue(nDisasterID, out disaster))
                {
                    isNormal = false;
                    isRegular = true;
                    return disaster;
                }
                else if (m_dicDisasterNonRegularNormal.TryGetValue(nDisasterID, out disaster))
                {
                    isNormal = true;
                    isRegular = false;
                    return disaster;
                }
                else if (m_dicDisasterNonRegularEmergency.TryGetValue(nDisasterID, out disaster))
                {
                    isNormal = false;
                    isRegular = false;
                    return disaster;
                }

                return null;
            }

            // DisasterID, VersionInfo
            public Dictionary<int, VersionInfo> GetVersionDictionary(bool isRegular, bool isNormal)
            {
                if (isRegular)
                {
                    if (isNormal)
                        return m_dicVersionRegularNormal;
                    else
                        return m_dicVersionRegularEmergency;
                }
                else
                {
                    if (isNormal)
                        return m_dicVersionNonRegularNormal;
                }

                return m_dicVersionNonRegularEmergency;
            }

            // FullPath(Category/SubCategory/Disaster), DisasterInfo
            public Dictionary<string, DisasterInfo> GetSOPDictionary(bool isRegular, bool isNormal)
            {
                if (isRegular)
                {
                    if (isNormal)
                        return m_dicSOPRegularNormal;
                    else
                        return m_dicSOPRegularEmergency;
                }
                else
                {
                    if (isNormal)
                        return m_dicSOPNonRegularNormal;
                }

                return m_dicSOPNonRegularEmergency;
            }

            // Disaster, FullPath(Category/SubCategory/Disaster)
            public Dictionary<DisasterInfo, string> GetFullPathDictionary()
            {
                return m_dicDisasterSOPFullPath;
            }

            // FullPath(Category/SubCategory/Disaster), DisasterInfo List
            public Dictionary<string, ArrayList> GetSOPDisasterListDictionary(bool isRegular, bool isNormal)
            {
                if (isRegular)
                {
                    if (isNormal)
                        return m_dicSOPRegularNormalDisasterList;
                    else
                        return m_dicSOPRegularEmergencyDisasterList;
                }
                else
                {
                    if (isNormal)
                        return m_dicSOPNonRegularNormalDisasterList;
                }

                return m_dicSOPNonRegularEmergencyDisasterList;
            }

            private bool LoadVersion()
            {
                m_dicVersionRegularNormal.Clear();
                m_dicVersionRegularEmergency.Clear();
                m_dicVersionNonRegularNormal.Clear();
                m_dicVersionNonRegularEmergency.Clear();

                //string strSQL = "select version.ID, version.VersionName, version.isRegular, version.isNormal, CompanyMember.MemberName, version.CreateTime, version.LastAccessTime, version.Description, Disaster.ID ";
                //strSQL += "from Version, SOPGenUser, CompanyMember, Disaster ";
                //strSQL += "where version.OwnerID = SOPGenUser.ID and SOPGenUser.MemberID = CompanyMember.ID and Version.ID = Disaster.VersionID order by Version.CreateTime";

                string szText = "SELECT v.ID, v.VersionName, v.isRegular, v.isNormal, sgu.MemberID, sgu.UserID, v.CreateTime, v.LastAccessTime, v.Description, ds.ID FROM Version as v " +
                                "   INNER JOIN SOPGenUser as sgu on  v.OwnerID = sgu.ID and sgu.SiteID = {0} " +
                                /*"   INNER JOIN CompanyMember as cm ON sgu.MemberID = cm.ID " +*/
                                "   INNER JOIN Disaster as ds ON v.ID = ds.VersionID " +
                                " ORDER BY v.CreateTime";

                string strSQL = string.Format(szText, m_nSiteID);
                return LoadVersion(strSQL);
            }

            private bool LoadVersion(string strSQL)
            {
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
                if (arrResult == null)
                    return false;

                DateTime dtDefault = new DateTime();

                int nCount = arrResult.Count;

                for (int i = 0; i < nCount - 9; i += 10)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                    string strVersionName = WebDBManager.GetStringField(arrResult[i + 1], "");
                    bool isRegular = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0) == 0 ? false : true;
                    bool isNormal = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0) == 0 ? false : true;
                    int nCompanyMemberID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                    string strUserID = WebDBManager.GetStringField(arrResult[i + 5], "");
                    //string strMemberName = WebDBManager.GetStringField(arrResult[i + 4], "");
                    DateTime dtBegin = WebDBManager.GetDateTimeField(arrResult[i + 6], dtDefault);
                    DateTime dtEnd = WebDBManager.GetDateTimeField(arrResult[i + 7], dtDefault);
                    string strDesc = WebDBManager.GetStringField(arrResult[i + 8], "");
                    int nDisasterID = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 0);

                    Data_CompanyMember member;
                    string strMemberName = "";

                    if (nCompanyMemberID < 0 || !m_dicCompanyMember.TryGetValue(nCompanyMemberID, out member))
                        strMemberName = strUserID;
                    else
                        strMemberName = member.MemberName;

                    VersionInfo version = new VersionInfo();
                    version.VersionID = nID;
                    version.VersionName = strVersionName;
                    version.UserName = strMemberName;
                    version.BeginTime = dtBegin;
                    version.LastAccessedTime = dtEnd;
                    version.Description = strDesc;
                    version.IsNormal = isNormal;
                    version.IsRegular = isRegular;

                    Dictionary<int, VersionInfo> dicVersion = GetVersionDictionary(isRegular, isNormal);
                    dicVersion[nDisasterID] = version;
                }
                return true;
            }

            private bool LoadVersion(int nVersionID)
            {
                string szText = "SELECT v.ID, v.VersionName, v.isRegular, v.isNormal, sgu.MemberID, sgu.UserID, v.CreateTime, v.LastAccessTime, v.Description, ds.ID FROM Version as v " +
                                "   INNER JOIN SOPGenUser as sgu on  v.OwnerID = sgu.ID and v.ID = {0}" +
                                "   INNER JOIN Disaster as ds ON v.ID = ds.VersionID " +
                                " ORDER BY v.CreateTime";

                string strSQL = string.Format(szText, nVersionID);
                return LoadVersion(strSQL);
            }

            char szDeli = (char)0x06;
            private bool LoadSOP()
            {
                m_dicSOPRegularNormal.Clear();
                m_dicSOPRegularEmergency.Clear();
                m_dicSOPNonRegularNormal.Clear();
                m_dicSOPNonRegularEmergency.Clear();

                m_dicDisasterSOPFullPath.Clear();

                //string strSQL = "select disaster.id, disaster.DisasterName, sc.SubCategoryName, dc.CategoryName, disaster.VersionID, Version.isRegular, Version.isNormal from disaster, SubDisasterCategory as sc, DisasterCategory as dc, Version ";
                //strSQL += "where disaster.SubDisasterID = sc.id and sc.DisasterID = dc.id and disaster.VersionID = Version.ID order by DisasterName";

                string szText = "SELECT ds.id, ds.DisasterName, sc.SubCategoryName, dc.CategoryName, ds.VersionID, v.isRegular, v.isNormal FROM disaster as ds " +
                                " INNER JOIN SubDisasterCategory as sc ON ds.SubDisasterID = sc.id " +
                                " INNER JOIN DisasterCategory as dc ON sc.DisasterID = dc.id  AND dc.SiteID = {0}" +
                                " INNER JOIN Version as v ON ds.VersionID = v.ID " +
                                " ORDER BY ds.DisasterName";

                string strSQL = string.Format(szText, m_nSiteID);
                return LoadSOP(strSQL);
            }

            private bool LoadSOP(string strSQL)
            {
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
                if (arrResult == null) return false;

                int nCount = arrResult.Count;
                if (nCount == 0) return true;

                string strDisasterIDs = "";
                Dictionary<int, DisasterInfo> dicDisaster = new Dictionary<int, DisasterInfo>();

                for (int i = 0; i < nCount - 6; i += 7)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                    string strDisasterName = WebDBManager.GetStringField(arrResult[i + 1], "");
                    string strSubCategoryName = WebDBManager.GetStringField(arrResult[i + 2], "");
                    string strCategoryName = WebDBManager.GetStringField(arrResult[i + 3], "");
                    int nVersionID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                    bool isRegular = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0) == 0 ? false : true;
                    bool isNormal = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0) == 0 ? false : true;

                    string strFullPath = strCategoryName + szDeli + strSubCategoryName + szDeli + strDisasterName;
                    DisasterInfo disaster = new DisasterInfo();
                    dicDisaster[nID] = disaster;

                    if (isNormal)
                    {
                        if (isRegular)
                            m_dicDisasterRegularNormal[nID] = disaster;
                        else
                            m_dicDisasterNonRegularNormal[nID] = disaster;
                    }
                    else
                    {
                        if (isRegular)
                            m_dicDisasterRegularEmergency[nID] = disaster;
                        else
                            m_dicDisasterNonRegularEmergency[nID] = disaster;
                    }

                    Dictionary<string, DisasterInfo> dicSOP = GetSOPDictionary(isRegular, isNormal);
                    Dictionary<DisasterInfo, string> dicSOPFullPath = GetFullPathDictionary();
                    Dictionary<string, ArrayList> dicSOPDisasterList = GetSOPDisasterListDictionary(isRegular, isNormal);

                    ArrayList arrDisasters = null;

                    if (dicSOPDisasterList.ContainsKey(strFullPath))
                        arrDisasters = dicSOPDisasterList[strFullPath];
                    else
                    {
                        arrDisasters = new ArrayList();
                        dicSOPDisasterList[strFullPath] = arrDisasters;
                    }

                    arrDisasters.Add(disaster);

                    disaster.DisasterID = nID;
                    disaster.VersionID = nVersionID;

                    if (strDisasterIDs.Length == 0)
                        strDisasterIDs = nID.ToString();
                    else
                        strDisasterIDs += ", " + nID.ToString();

                    if (dicSOP.ContainsKey(strFullPath))
                    {
                        DisasterInfo disasterLatest = dicSOP[strFullPath];
                        Dictionary<int, VersionInfo> dicVersion = GetVersionDictionary(isRegular, isNormal);

                        if (dicVersion != null)
                        {
                            if (dicVersion.ContainsKey(disasterLatest.DisasterID))// && dicVersion.ContainsKey(disaster.DisasterID))
                            {
                                if (!dicVersion.ContainsKey(disaster.DisasterID))
                                    LoadVersion(disaster.VersionID);

                                if (dicVersion.ContainsKey(disasterLatest.DisasterID) && dicVersion.ContainsKey(disaster.DisasterID))
                                {
                                    VersionInfo versionLatest = dicVersion[disasterLatest.DisasterID];
                                    VersionInfo versionCurrent = dicVersion[disaster.DisasterID];

                                    if (versionLatest.BeginTime < versionCurrent.BeginTime)
                                    {
                                        dicSOP[strFullPath] = disaster;
                                        dicSOPFullPath[disaster] = strFullPath;
                                    } 
                                }
                            }
                        }
                    }
                    else
                    {
                        dicSOP[strFullPath] = disaster;
                        dicSOPFullPath[disaster] = strFullPath;
                    }
                }

                if (!LoadDisasterActionSteps(dicDisaster, strDisasterIDs, m_dicActionStepInfo))
                    return false;
                /*if (strDisasterIDs.Length == 0)
                    return true;

                strSQL = string.Format("select ID, StepName, PeriodType, BeginTime, EndTIme, WeekDayOption, Iteration, IterationType, ProcessTime, ProcessTimeType, DisasterID, ParentStepID from ActionStep where DisasterID in ({0})", strDisasterIDs);
                arrResult = m_dbMgr.GetResultData(strSQL, 0);
                if (arrResult == null) return false;

                DateTime dtDefault = new DateTime();

                nCount = arrResult.Count;

                for (int i = 0; i < nCount - 9; i += 12)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                    string strStepName = WebDBManager.GetStringField(arrResult[i + 1], "");
                    int nPeriodType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                    DateTime dtBegin = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                    DateTime dtEnd = WebDBManager.GetDateTimeField(arrResult[i + 4], dtDefault);
                    int nWeekdayOpt = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 127);
                    int nIteration = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 1);
                    int nIterationType = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                    int nProcessTime = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0);
                    int nProcessTimeType = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 5);
                    int nDisasterID = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);
                    int nParentStepID = WebDBManager.GetIntField(arrResult[i + 11].ToString(), -1);

                    if (!dicDisaster.ContainsKey(nDisasterID))
                        continue;

                    ActionStepInfo actionStep = new ActionStepInfo();

                    actionStep.ActionStepID = nID;
                    actionStep.ActionStepName = strStepName;
                    actionStep.ParentStepID = nParentStepID;
                    actionStep.PeriodType = nPeriodType;
                    actionStep.BeginTime = dtBegin;
                    actionStep.EndTime = dtEnd;
                    actionStep.WeekDayOption = nWeekdayOpt;
                    actionStep.Iteration = nIteration;
                    actionStep.IterationType = nIterationType;
                    actionStep.ProcessTime = nProcessTime;
                    actionStep.ProcessTimeType = nProcessTimeType;
                    actionStep.DisasterID = nDisasterID;

                    DisasterInfo disaster = dicDisaster[nDisasterID];
                    disaster.ActionSteps.Add(actionStep);

                    m_dicActionStepInfo[actionStep.ActionStepID] = actionStep;
                }*/

                SortDisasterList();

                return true;
            }

            public bool LoadDisasterActionStep(int nActionStepID)
            {
                string strSQL = string.Format("select ID, StepName, PeriodType, BeginTime, EndTIme, WeekDayOption, Iteration, IterationType, ProcessTime, ProcessTimeType, DisasterID, ParentStepID from ActionStep where ID = {0}", nActionStepID);
                return LoadDisasterActionSteps(strSQL, null, m_dicActionStepInfo);
            }

            private bool LoadDisasterActionSteps(string strSQL, Dictionary<int, DisasterInfo> dicDisaster, Dictionary<int, ActionStepInfo> dicActionStepInfo)
            {
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
                if (arrResult == null)
                    return false;

                DateTime dtDefault = new DateTime();

                int nCount = arrResult.Count;
                bool isRegular, isNormal;

                for (int i = 0; i < nCount - 9; i += 12)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                    string strStepName = WebDBManager.GetStringField(arrResult[i + 1], "");
                    int nPeriodType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                    DateTime dtBegin = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                    DateTime dtEnd = WebDBManager.GetDateTimeField(arrResult[i + 4], dtDefault);
                    int nWeekdayOpt = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 127);
                    int nIteration = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 1);
                    int nIterationType = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                    int nProcessTime = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0);
                    int nProcessTimeType = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 5);
                    int nDisasterID = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);
                    int nParentStepID = WebDBManager.GetIntField(arrResult[i + 11].ToString(), -1);

                    DisasterInfo disaster = null;

                    if (dicDisaster == null)
                    {
                        disaster = GetDisaster(nDisasterID, out isNormal, out isRegular);
                    }
                    else
                    {
                        dicDisaster.TryGetValue(nDisasterID, out disaster);
                    }

                    if (disaster == null)
                        continue;

                    ActionStepInfo actionStep = new ActionStepInfo();

                    actionStep.ActionStepID = nID;
                    actionStep.ActionStepName = strStepName;
                    actionStep.ParentStepID = nParentStepID;
                    actionStep.PeriodType = nPeriodType;
                    actionStep.BeginTime = dtBegin;
                    actionStep.EndTime = dtEnd;
                    actionStep.WeekDayOption = nWeekdayOpt;
                    actionStep.Iteration = nIteration;
                    actionStep.IterationType = nIterationType;
                    actionStep.ProcessTime = nProcessTime;
                    actionStep.ProcessTimeType = nProcessTimeType;
                    actionStep.DisasterID = nDisasterID;

                    //DisasterInfo disaster = dicDisaster[nDisasterID];
                    disaster.ActionSteps.Add(actionStep);

                    if (dicActionStepInfo != null)
                        dicActionStepInfo[actionStep.ActionStepID] = actionStep;
                }

                return true;
            }

            public bool LoadDisasterActionSteps(Dictionary<int, DisasterInfo> dicDisaster, string strDisasterIDs, Dictionary<int, ActionStepInfo> dicActionStepInfo = null)
            {
                if (strDisasterIDs.Length == 0)
                    return true;

                string strSQL = string.Format("select ID, StepName, PeriodType, BeginTime, EndTIme, WeekDayOption, Iteration, IterationType, ProcessTime, ProcessTimeType, DisasterID, ParentStepID from ActionStep where DisasterID in ({0})", strDisasterIDs);
                return LoadDisasterActionSteps(strSQL, dicDisaster, dicActionStepInfo);
                /*ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
                if (arrResult == null)
                    return false;

                DateTime dtDefault = new DateTime();

                int nCount = arrResult.Count;

                for (int i = 0; i < nCount - 9; i += 12)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                    string strStepName = WebDBManager.GetStringField(arrResult[i + 1], "");
                    int nPeriodType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                    DateTime dtBegin = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                    DateTime dtEnd = WebDBManager.GetDateTimeField(arrResult[i + 4], dtDefault);
                    int nWeekdayOpt = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 127);
                    int nIteration = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 1);
                    int nIterationType = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                    int nProcessTime = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0);
                    int nProcessTimeType = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 5);
                    int nDisasterID = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);
                    int nParentStepID = WebDBManager.GetIntField(arrResult[i + 11].ToString(), -1);

                    if (!dicDisaster.ContainsKey(nDisasterID))
                        continue;

                    ActionStepInfo actionStep = new ActionStepInfo();

                    actionStep.ActionStepID = nID;
                    actionStep.ActionStepName = strStepName;
                    actionStep.ParentStepID = nParentStepID;
                    actionStep.PeriodType = nPeriodType;
                    actionStep.BeginTime = dtBegin;
                    actionStep.EndTime = dtEnd;
                    actionStep.WeekDayOption = nWeekdayOpt;
                    actionStep.Iteration = nIteration;
                    actionStep.IterationType = nIterationType;
                    actionStep.ProcessTime = nProcessTime;
                    actionStep.ProcessTimeType = nProcessTimeType;
                    actionStep.DisasterID = nDisasterID;

                    DisasterInfo disaster = dicDisaster[nDisasterID];
                    disaster.ActionSteps.Add(actionStep);

                    if (dicActionStepInfo != null)
                        dicActionStepInfo[actionStep.ActionStepID] = actionStep;
                }

                return true;*/
            }

            private void SortDisasterList()
            {
                SortDisasterList(GetSOPDisasterListDictionary(true, true));
                SortDisasterList(GetSOPDisasterListDictionary(true, false));
                SortDisasterList(GetSOPDisasterListDictionary(false, true));
                SortDisasterList(GetSOPDisasterListDictionary(false, false));
            }

            private void SortDisasterList(Dictionary<string, ArrayList> dicSOPDisasterList)
            {
                foreach (KeyValuePair<string, ArrayList> pair in dicSOPDisasterList)
                {
                    pair.Value.Sort();
                }
            }

            public bool IsOpened
            {
                get { return m_isOpened; }
            }

            public ArrayList CompanyMemberList
            {
                get { return m_arrCompanyMember; }
            }

            // strPhoneNumber에 빈칸이나 '-'등이 들어있을 경우 없앤다. 
            private string ValidPhoneNumber(string strPhoneNumber)
            {
                string strResult = "";
                int nLen = strPhoneNumber.Length;

                for (int i = 0; i < nLen; i++)
                {
                    char ch = strPhoneNumber.ElementAt(i);

                    if (ch != ' ' && ch != '\t' && ch != '-')
                        strResult += ch;
                }

                return strResult;
            }

            private bool LoadCompanyMember()
            {
                string strSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", m_nSiteID);
                ArrayList arrResult1 = m_dbMgr.GetResultData(strSQL, 0);
                if (arrResult1 == null || arrResult1.Count == 0)
                    return false;

                int nTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
                if (nTeamID == -1)
                    return false;

                ArrayList arrResult2 = ExecuteTeamList(m_dbMgr, nTeamID);
                //strSQL = string.Format("sp_TeamList2 {0}", nTeamID);
                //ArrayList arrResult2 = m_dbMgr.GetStoredProcedureData(strSQL, 0);
                if (arrResult2 == null || arrResult2.Count == 0)
                    return false;

                m_dicRegularTeam.Clear();

                string szTeamList = "";
                for (int i = 0; i < arrResult2.Count - 2; i += 3)
                {
                    int nID = WebDBManager.GetIntField(arrResult2[i].ToString(), -1);
                    string strTeamName = WebDBManager.GetStringField(arrResult2[i + 1], "");
                    int nParentTeamID = WebDBManager.GetIntField(arrResult2[i + 2].ToString(), -1);

                    if (nID < 0 || strTeamName.Length == 0 || strTeamName == "null")
                        continue;

                    if (szTeamList != "")
                    {
                        szTeamList += ",";
                    }
                    szTeamList += nID.ToString();

                    Data_RegularTeam team = new Data_RegularTeam();
                    team.ID = nID;
                    team.TeamName = strTeamName;
                    team.ParentTeamID = nParentTeamID;

                    m_dicRegularTeam[nID] = team;
                }

                if (szTeamList == "")
                {
                    return false;
                }

                foreach (KeyValuePair<int, Data_RegularTeam> pair in m_dicRegularTeam)
                {
                    Data_RegularTeam teamParent = null;

                    if (m_dicRegularTeam.TryGetValue(pair.Value.ParentTeamID, out teamParent))
                    {
                        teamParent.ChildTeams.Add(pair.Value);
                    }
                }

                string szText = "Select rm.RegularTeamID, rm.CompanyMemberID, rm.PositionID, MemberName, LevelID, MemberID, OfficePhoneNumber, PhoneNumber   ";
                szText += " FROM CompanyMember, RegularMemberList as rm WHERE CompanyMember.ID = rm.CompanyMemberID and rm.RegularTeamID in ({0})";
               
                /*string szText = "select ID, MemberName, LevelID, MemberID, OfficePhoneNumber, PhoneNumber " +
                                " FROM CompanyMember WHERE RegularTeamID in ({0})";*/

                //string strSQL = "select ID, MemberName, RegularTeamID, LevelID, PositionID, MemberID, SecondRegularTeamID, SecondPositionID, OfficePhoneNumber, PhoneNumber from CompanyMember";

                strSQL = string.Format(szText, szTeamList);

                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
                if (arrResult == null) return false;

                int nCount = arrResult.Count;
                if (nCount == 0) return true;

                m_arrCompanyMember.Clear();
                m_dicRegularTeamMember.Clear();
                m_dicCompanyMember.Clear();

                Data_CompanyMember member;

                for (int i = 0; i < nCount - 7; i += 8)
                {
                    int nRegularTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                    int nID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                    int nPositionID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                    string strMemberName = WebDBManager.GetStringField(arrResult[i + 3], "");
                    int nLevelID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                    string strMemberID = WebDBManager.GetStringField(arrResult[i + 5], "");
                    //int nSecondRegularTeamID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
                    //int nSecondPositionID = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                    string strOfficePhoneNumber = WebDBManager.GetStringField(arrResult[i + 6], "");
                    //string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 9], "");
                    string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 7], "");

                    Data_RegularTeam team = GetRegularTeam(nRegularTeamID);

                    if (team == null)
                        continue;

                    if (string.Compare(strPhoneNumber, "null", true) == 0 || strPhoneNumber == "")
                        strPhoneNumber = "";
                    else
                        strPhoneNumber = DBUtility.AES256Cipher.AES_decrypt(strPhoneNumber, key);

                    strPhoneNumber = ValidPhoneNumber(strPhoneNumber);

                    if (string.Compare(strOfficePhoneNumber, "null", true) == 0)
                        strOfficePhoneNumber = "";

                    if (!m_dicCompanyMember.TryGetValue(nID, out member))
                    {
                        member = new Data_CompanyMember();
                    }

                    member.ID = nID;
                    member.MemberName = strMemberName;
                    member.LevelID = nLevelID;
                    member.MemberID = strMemberID;
                    member.OfficePhoneNumber = strOfficePhoneNumber;
                    member.PhoneNumber = strPhoneNumber;

                    m_dicCompanyMember[nID] = member;
                    m_arrCompanyMember.Add(member);

                    member.TeamPositions[team] = nPositionID;

                    /*Data_CompanyMember data = new Data_CompanyMember();
                    data.ID = nID;
                    data.MemberName = strMemberName;
                    //data.RegularTeamID = nRegularTeamID;
                    data.LevelID = nLevelID;
                    //data.PositionID = nPositionID;
                    data.MemberID = strMemberID;
                    //data.SecondRegularTeamID = nSecondRegularTeamID;
                    //data.SecondPositionID = nSecondPositionID;
                    data.OfficePhoneNumber = strOfficePhoneNumber;
                    data.PhoneNumber = strPhoneNumber;

                    m_arrCompanyMember.Add(data);*/

                    ////////////////////////////////////////////////////////////////
                    // 팀별 팀원 정보 저장
                    ArrayList arrTeamMemberList = null;

                    if (m_dicRegularTeamMember.ContainsKey(nRegularTeamID))
                        arrTeamMemberList = m_dicRegularTeamMember[nRegularTeamID];
                    else
                    {
                        arrTeamMemberList = new ArrayList();
                        m_dicRegularTeamMember[nRegularTeamID] = arrTeamMemberList;
                    }

                    arrTeamMemberList.Add(member);
                    ////////////////////////////////////////////////////////////////
                }

                return true;
            }

            public bool LoadControlRoom()
            { 
                m_dicControlRoom.Clear();

                string strSQL = "select cr.ID, cr.RoomType, cr.LocationName, crt.TypeName from ControlRoom as cr, ControlRoomType as crt ";
                strSQL += "where cr.RoomType = crt.ID and crt.SiteID = " + m_nSiteID.ToString() + " order by cr.RoomType";

                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return false;

                //Data_ControlRoom teamRoot = GetRootControlRoomTeam(dicTeams);

                List<int> controlRoomIDs = new List<int>();
                List<int> roomTypeIDs = new List<int>();
                string strRoomTypeIDs = "";

                Data_ControlRoom root = new Data_ControlRoom();
                root.TeamName = Data_ControlRoom.ROOT_NAME;
                root.ID = Data_ControlRoom.ROOT_ID;
                root.ParentTeamID = -1;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 3; i += 4)
                {
                    int nControlRoomID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nRoomTypeID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    string strLocationName = WebDBManager.GetStringField(arrResult[i + 2]);
                    string strRoomType = WebDBManager.GetStringField(arrResult[i + 3]);

                    if (nControlRoomID < 0 || nRoomTypeID < 0 || strLocationName == null || strRoomType == null)
                        continue;

                    int nID = Data_ControlRoom.MakeID(nRoomTypeID, nControlRoomID, 0);

                    Data_ControlRoom team = new Data_ControlRoom();
                    team.ID = nID;
                    //team.ParentTeam = teamRoot;

                    team.ParentTeamID = root.ID;
                    root.ChildTeams.Add(team);

                    if (strLocationName == strRoomType)
                        team.TeamName = strLocationName;
                    else
                        team.TeamName = strLocationName + " " + strRoomType;
                     
                    m_dicControlRoom[nID] = team;

                    if (!roomTypeIDs.Contains(nRoomTypeID))
                    {
                        roomTypeIDs.Add(nRoomTypeID);

                        if (strRoomTypeIDs.Length == 0)
                            strRoomTypeIDs = nRoomTypeID.ToString();
                        else
                            strRoomTypeIDs += ", " + nRoomTypeID.ToString();
                    }

                    if (!controlRoomIDs.Contains(nControlRoomID))
                        controlRoomIDs.Add(nControlRoomID);
                }

                if (roomTypeIDs.Count == 0)
                    return true;

                strSQL = string.Format("Select ID, JobName, RoomType from ControlTeamJobPosition where RoomType in ({0})", strRoomTypeIDs);
                arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return false;

                nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 2; i += 3)
                {
                    int nPositionID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strJobName = WebDBManager.GetStringField(arrResult[i + 1]);
                    int nRoomTypeID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                    if (nPositionID < 0 || nRoomTypeID < 0 || strJobName == null)
                        continue;

                    foreach (int nControlRoomID in controlRoomIDs)
                    {
                        int nID = Data_ControlRoom.MakeID(nRoomTypeID, nControlRoomID, nPositionID);

                        Data_ControlRoom team = new Data_ControlRoom();
                        team.TeamName = strJobName;
                        team.ID = nID;

                        int nParentTeamID = Data_ControlRoom.MakeID(nRoomTypeID, nControlRoomID, 0);
                        Data_ControlRoom parentTeam;

                        if (m_dicControlRoom.TryGetValue(nParentTeamID, out parentTeam))
                        {
                            m_dicControlRoom[nID] = team;
                            parentTeam.ChildTeams.Add(team);
                        }
                    }
                }

                m_dicControlRoom[root.ID] = root;
                return true;
            }

            private bool m_lockControlRoomMembers = false;
            public bool LockControlRoomMembers
            {
                get { return m_lockControlRoomMembers; }
                set { m_lockControlRoomMembers = value; }
            }

            public bool LoadControlRoomMembers()
            {
                if (m_lockControlRoomMembers)
                    return true;

                m_dicControlRoomMembers.Clear();

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT cm.ID as MemberID, cm.MemberName, cm.PhoneNumber, RoomID, TeamID, JobPosition, MemberType, cr.RoomType");
                sb.Append("  FROM controlteammembers as ctm");
                sb.Append(" INNER JOIN companymember as cm ON ctm.MemberID=cm.id");
                sb.Append(" INNER JOIN controlroom as cr ON ctm.RoomID = cr.ID");
                sb.Append(" WHERE MemberType = 1");
                sb.Append("   AND TeamID in (select teamID from controlworkingteam)");
                sb.Append(" UNION ALL ");
                sb.Append("SELECT ctm.ID as MemberID, ecm.Name as MemberName, ecm.PhoneNumber, RoomID, TeamID, JobPosition, MemberType, cr.RoomType");
                sb.Append("  FROM controlteammembers as ctm ");
                sb.Append(" INNER JOIN externalcompanymember as ecm ON ctm.MemberID=ecm.ID");
                sb.Append(" INNER JOIN controlroom as cr ON ctm.RoomID = cr.ID");
                sb.Append(" WHERE MemberType = 4");
                sb.Append("   AND TeamID in (select teamID from controlworkingteam)");

                ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString(), 0);

                if (arrResult == null) return false;

                for (int i = 0; i < arrResult.Count; i+=8)
                {
                    int nMemberID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strMemberName = WebDBManager.GetStringField(arrResult[i + 1]);
                    string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 2]);
                    int nRoomID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                    int nTeamID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                    int nJobPosition = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                    int nMemberType = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                    int nRoomType = WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);

                    if (strPhoneNumber == null)
                        strPhoneNumber = "";
                    else
                        strPhoneNumber = DBUtility.AES256Cipher.AES_decrypt(strPhoneNumber, key);

                    strPhoneNumber = ValidPhoneNumber(strPhoneNumber);

                    Data_ControlRoomMember data = new Data_ControlRoomMember();
                    data.MemberID = nMemberID;
                    data.MemberName = strMemberName;
                    data.PhoneNumber = strPhoneNumber;
                    data.RoomID = nRoomID;
                    data.TeamID = nTeamID;
                    data.JobPosition = nJobPosition;
                    data.MemberType = nMemberType;
                    data.RoomType = nRoomType;

                    int nID = Data_ControlRoomMember.MakeID(data.RoomType, data.RoomID, data.JobPosition);

                    if (!m_dicControlRoomMembers.ContainsKey(nID))
                        m_dicControlRoomMembers.Add(nID, data); 
                }

                return true;
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

            private bool LoadTemporaryNormalTeams()
            {
                m_dicTemporaryNormalTeam.Clear();

                string strSQL = "Select ID, TeamName, ParentTeamID from TemporaryNormalTeam where SiteID = " + m_nSiteID.ToString();
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return false;

                int nResultCount = arrResult.Count;
                Dictionary<int, int> dicParentTeamIDs = new Dictionary<int,int>();

                for (int i=0;i<nResultCount-2;i+=3)
                {
                    DBUtility.VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    string strTeamName = WebDBManager.GetStringField(arrResult[i + 1]);
                    DBUtility.VariousData<int> parentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                    if (id == null || strTeamName == null)
                        continue;

                    if (parentTeamID != null)
                        dicParentTeamIDs[id.Data] = parentTeamID.Data;

                    Data_NormalTeam team = new Data_NormalTeam();

                    team.ID = id.Data;
                    team.TeamName = strTeamName;
                    m_dicTemporaryNormalTeam[team.ID] = team;
                }

                foreach (KeyValuePair<int, Data_NormalTeam> pair in m_dicTemporaryNormalTeam)
                {
                    int nParentTeamID;
                    Data_NormalTeam teamParent;

                    if (dicParentTeamIDs.TryGetValue(pair.Key, out nParentTeamID))
                    {
                        if (m_dicTemporaryNormalTeam.TryGetValue(nParentTeamID, out teamParent))
                        {
                            pair.Value.ParentTeam = teamParent;
                        }
                    }
                }

                return true;
            }
                
            private bool LoadTemporaryHolidyTeams()
            {
                m_dicTemporaryEmergencyTeam.Clear();

                string strSQL = "Select ID, TeamName, ParentTeamID from TemporaryEmergencyTeam where SiteID = " + m_nSiteID.ToString();
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return false;

                int nResultCount = arrResult.Count;
                Dictionary<int, int> dicParentTeamIDs = new Dictionary<int, int>();

                for (int i = 0; i < nResultCount - 2; i += 3)
                {
                    DBUtility.VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    string strTeamName = WebDBManager.GetStringField(arrResult[i + 1]);
                    DBUtility.VariousData<int> parentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                    if (id == null || strTeamName == null)
                        continue;

                    if (parentTeamID != null)
                        dicParentTeamIDs[id.Data] = parentTeamID.Data;

                    Data_EmergencyTeam team = new Data_EmergencyTeam();

                    team.ID = id.Data;
                    team.TeamName = strTeamName;
                    m_dicTemporaryEmergencyTeam[team.ID] = team;
                }

                foreach (KeyValuePair<int, Data_EmergencyTeam> pair in m_dicTemporaryEmergencyTeam)
                {
                    int nParentTeamID;
                    Data_EmergencyTeam teamParent;

                    if (dicParentTeamIDs.TryGetValue(pair.Key, out nParentTeamID))
                    {
                        if (m_dicTemporaryEmergencyTeam.TryGetValue(nParentTeamID, out teamParent))
                        {
                            pair.Value.ParentTeam = teamParent;
                        }
                    }
                }

                return true;
            }

            private bool LoadTemporaryTeam()
            {
                // RegularTeamID, Parent Team ID
                Dictionary<int, int> dicRegularTeam = new Dictionary<int, int>();

                /////////////////////////////////////////////////////////////////////////////
                // RegularTeam의 계층 구조를 Dictionary로 만들기

                string strSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", m_nSiteID);
                ArrayList arrResult1 = m_dbMgr.GetResultData(strSQL, 0);
                if (arrResult1 == null || arrResult1.Count == 0)
                    return false;

                int nTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
                if (nTeamID == -1)
                    return false;

                ArrayList arrResult = ExecuteTeamList(m_dbMgr, nTeamID);
                //strSQL = string.Format("sp_TeamList2 {0}", nTeamID);
                //ArrayList arrResult = m_dbMgr.GetStoredProcedureData(strSQL, 0);
                if (arrResult == null || arrResult.Count == 0)
                    return false;


               // string strSQL = "select id, ParentTeamID from RegularTeam";
                //ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

                //if (arrResult == null)
                //    return false;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 2; i += 3)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                    if (nID < 0)
                        continue;

                    dicRegularTeam[nID] = nParentTeamID;
                }
                /////////////////////////////////////////////////////////////////////////////

                LoadTemporaryNormalTeams();
                LoadTemporaryHolidyTeams();

                /////////////////////////////////////////////////////////////////////////////
                strSQL = "select link.ID, team.ID, link.MemberID, link.MemberType, link.IsTeamLeader, link.Role, link.MemberName from TemporaryNormalTeam as team, TemporaryMemberList as link where link.TemporaryTeamID = team.ID and link.IsNormal = 1 and team.SiteID = " + m_nSiteID.ToString();
                // 평일 비상조직에 대한 RegularTeamID Link List 만들기
                //strSQL = "select id, RegularTeamLink from TemporaryNormalTeam WHERE SiteID = " + m_nSiteID.ToString();
                arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return false;

                nResultCount = arrResult.Count;

                TemporaryMember.MemberType memberType;
                TemporaryMember.RoleType roleType;
                List<TemporaryMember> members;
                bool includeChildTeams = true;

                for (int i = 0; i < nResultCount - 6;i+=7 )
                {
                    int nLinkID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    int nMemberID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                    int nMemberType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                    int nTeamLeader = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                    int nRoleType = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                    string strMemberName = WebDBManager.GetStringField(arrResult[i + 6], "");

                    if (nID < 0 || nLinkID < 0)
                        continue;

                    if (nMemberID < 0)
                    {
                        includeChildTeams = false;
                        nMemberID = -nMemberID;
                    }
                    else
                        includeChildTeams = true;

                    if (!TemporaryMember.GetMemberType(nMemberType, out memberType))
                        continue;

                    if (!TemporaryMember.GetRoleType(nRoleType, out roleType))
                        roleType = TemporaryMember.RoleType.Unknown;

                    if (strMemberName == "null")
                        strMemberName = "";

                    if (!m_dicTemporaryNormalTeamID.TryGetValue(nID, out members))
                    {
                        members = new List<TemporaryMember>();
                        m_dicTemporaryNormalTeamID[nID] = members;
                    }

                    TemporaryMember member = new TemporaryMember(nID, true, nMemberID, nTeamLeader, memberType, roleType, strMemberName);
                    member.IncludeChildTeams = includeChildTeams;
                    members.Add(member);

                    m_dicTemporaryMembers[nLinkID] = member;
                }

                /*for (int i = 0; i < nResultCount - 1; i += 2)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strRegularTeamLink = WebDBManager.GetStringField(arrResult[i + 1], "");

                    ArrayList arrTeamIDList = GetRegularTeamList(strRegularTeamLink, dicRegularTeam);
                    if (arrTeamIDList != null)
                        m_dicTemporaryNormalTeamID[nID] = arrTeamIDList;
                }*/
                /////////////////////////////////////////////////////////////////////////////

                /////////////////////////////////////////////////////////////////////////////
                strSQL = "select link.ID, team.ID, link.MemberID, link.MemberType, link.IsTeamLeader, link.Role, link.MemberName from TemporaryEmergencyTeam as team, TemporaryMemberList as link where link.TemporaryTeamID = team.ID and link.IsNormal = 0 and team.SiteID = " + m_nSiteID.ToString();
                // 야간 및 휴일 비상조직에 대한 RegularTeamID Link List 만들기
                //strSQL = "select id, RegularTeamLink from TemporaryEmergencyTeam WHERE SiteID = " + m_nSiteID.ToString();
                arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return false;

                nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 6; i += 7)
                {
                    int nLinkID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    int nMemberID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                    int nMemberType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                    int nTeamLeader = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                    int nRoleType = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                    string strMemberName = WebDBManager.GetStringField(arrResult[i + 6], "");

                    if (nID < 0 || nLinkID < 0)
                        continue;

                    if (!TemporaryMember.GetMemberType(nMemberType, out memberType))
                        continue;

                    if (!TemporaryMember.GetRoleType(nRoleType, out roleType))
                        roleType = TemporaryMember.RoleType.Unknown;

                    if (strMemberName == "null")
                        strMemberName = "";

                    if (!m_dicTemporaryEmergencyTeamID.TryGetValue(nID, out members))
                    {
                        members = new List<TemporaryMember>();
                        m_dicTemporaryEmergencyTeamID[nID] = members;
                    }

                    TemporaryMember member = new TemporaryMember(nID, false, nMemberID, nTeamLeader, memberType, roleType, strMemberName);
                    members.Add(member);

                    m_dicTemporaryMembers[nLinkID] = member;
                }

                /*for (int i = 0; i < nResultCount - 1; i += 2)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strRegularTeamLink = WebDBManager.GetStringField(arrResult[i + 1], "");

                    ArrayList arrTeamIDList = GetRegularTeamList(strRegularTeamLink, dicRegularTeam);
                    if (arrTeamIDList != null)
                        m_dicTemporaryEmergencyTeamID[nID] = arrTeamIDList;
                }*/
                /////////////////////////////////////////////////////////////////////////////

                return true;
            }

            private bool LoadUserDefinedTeam(WebDBManager dbMgr, Dictionary<int, Data_ExternalTeam> dicOtherTeam)
            {
                string strSQL = string.Format("select id, TeamName, PhoneNumber, FaxNumber from UserDefinedTeam WHERE SiteID = {0}", m_nSiteID);
                ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return false;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 3; i += 4)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                    string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 2], "");
                    string strFaxNumber = WebDBManager.GetStringField(arrResult[i + 3], "");

                    Data_ExternalTeam data = new Data_ExternalTeam(nID, strTeamName, strPhoneNumber, strFaxNumber);
                    dicOtherTeam[nID] = data;
                }

                return true;
            }

            private bool LoadExternalTeam(WebDBManager dbMgr, Dictionary<int, Data_ExternalTeam> dicOtherTeam)
            {
                string strSQL = string.Format("select id, TeamName, PhoneNumber, FaxNumber, ParentTeamID from ExternalTeam WHERE SiteID = {0}", m_nSiteID);
                ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return false;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 4; i += 5)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                    string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 2], "");
                    string strFaxNumber = WebDBManager.GetStringField(arrResult[i + 3], "");
                    int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);

                    Data_ExternalTeam data = new Data_ExternalTeam(nID, strTeamName, strPhoneNumber, strFaxNumber);
                    dicOtherTeam[nID] = data;
                    data.ParentTeamID = nParentTeamID;
                }

                foreach (KeyValuePair<int, Data_ExternalTeam> pair in dicOtherTeam)
                {
                    Data_ExternalTeam teamParent = null;

                    if (dicOtherTeam.TryGetValue(pair.Value.ParentTeamID, out teamParent))
                    {
                        teamParent.ChildTeams.Add(pair.Value);
                    }
                }

                return true;
            }

            // 사용자 정의 조직과 외부 조직 정보를 읽어온다.
            public bool LoadOtherTeams()
            {
                WebDBManager dbMgr = m_dbMgr;

                if (!LoadUserDefinedTeam(dbMgr, m_dicUserDefinedTeam))
                    return false;
                if (!LoadExternalTeam(dbMgr, m_dicExternalTeam))
                    return false;

                return true;
            }

            /// <summary>
            /// 사용자가 편집한후 SOP수행시에 사용되는 UserDefinedTeam정보
            /// </summary>
            /// <param name="nActionStepHistoryID"></param>
            /// <returns></returns>
            /*public ArrayList GetUsingUserDefineTeamsByHistoryID( int nActionStepHistoryID)
            {
                ArrayList arResult = new ArrayList();
                WebDBManager dbMgr = m_dbMgr;

                string szTemp = "SELECT ID, UserDefinedTeamID, PhoneNumber, UserName " +
                               " FROM ActionStepUsingUserDefinedTeam where ActionStepHistoryID = {0}";
                string szSQL = string.Format(szTemp, nActionStepHistoryID);
                ArrayList arrResult = dbMgr.GetResultData(szSQL, 0);

                if (arrResult == null)
                    return null;
                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 3; i += 4)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nTeamID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                    string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 2], "");
                    string strUserName = WebDBManager.GetStringField(arrResult[i+3], "");

                    if (strUserName == null || strUserName == "null")
                        strUserName = "";

                   
                    Data_ExternalTeam orgTeam = GetUserDefinedTeamMember(nTeamID);
                    if( orgTeam != null)
                    {
                        Data_ExternalTeam team = new Data_ExternalTeam();
                        team.ID = nTeamID;
                        team.PhoneNumber = strPhoneNumber;
                        team.FaxNumber = orgTeam.FaxNumber;
                        team.Tag = strUserName;
                        team.TeamName = orgTeam.TeamName;

                        arResult.Add(team);
                    }
                }
                return arResult;
            }*/

            /*public void SaveUsingUserDefinedTeam(int nHistoryID, List<Data_UserDefinedTeam> arTeam)
            {
                if( arTeam == null)
                    return;

                WebDBManager dbMgr = m_dbMgr;
                // nHistoryID와 TeamID로 select
                // data가 있고 다르면 update
                // 없으면 insert
                foreach(Data_UserDefinedTeam team in arTeam)
                {
                    int nTeamID = team.ID;
                    string szPhoneNumber = team.PhoneNumber;
                    string szUserName = (team.Tag == null ? "" : team.Tag.ToString());

                    string szTemp = "SELECT ID, UserDefinedTeamID, PhoneNumber, UserName " +
                           " FROM ActionStepUsingUserDefinedTeam "+
                           " WHERE ActionStepHistoryID = {0} and UserDefinedTeamID = {1}";
                    string szSQL = string.Format(szTemp, nHistoryID, nTeamID);
                    ArrayList arrResult = dbMgr.GetResultData(szSQL, 0);

                    if (arrResult == null || arrResult.Count == 0)
                    {
                        // insert
                        int nID = GetMaxUsingTeamID() + 1;

                        string szTemp2 = "INSERT INTO ActionStepUsingUserDefinedTeam " +
                                        " (ID, ActionStepHistoryID, UserDefinedTeamID, PhoneNumber, UserName) " +
                                        "  VALUES ({0}, {1}, {2}, '{3}', '{4}')";

                        string szSQL2 = string.Format(szTemp2, nID, nHistoryID, nTeamID, szPhoneNumber, szUserName);
                        dbMgr.GetResultData(szSQL2, 0);
                    }
                    else
                    {
                        // Update

                        bool bSame = true;
                        int nResultCount = arrResult.Count;
                        if (nResultCount == 4)
                        {
                            int nDBID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
                            int nDBTeamID = WebDBManager.GetIntField(arrResult[1].ToString(), -1);

                            string strDBPhoneNumber = WebDBManager.GetStringField(arrResult[2], "");
                            string strDBUserName = WebDBManager.GetStringField(arrResult[3], "");

                            if (strDBUserName == null || strDBUserName == "null")
                                strDBUserName = "";

                            if (szPhoneNumber != strDBPhoneNumber)
                                bSame = false;
                            if (szUserName == strDBUserName)
                                bSame = false;
                        }

                        if (bSame == false)
                        {
                            string szTemp2 = "UPDATE ActionStepUsingUserDefinedTeam " +
                                             " SET PhoneNumber = '{0}', UserName = '{1}' " +
                                             " WHERE ActionStepHistoryID = {2} and UserDefinedTeamID = {3}";

                            string szSQL2 = string.Format(szTemp2, szPhoneNumber, szUserName, nHistoryID, nTeamID);
                            dbMgr.GetResultData(szSQL2, 0);
                        }
                    }                    
                }                
            }*/

            /// <summary>
            /// 해당 ActionStep에서 사용중인 모든 Team들의 ID들을 가져오는 함수
            /// </summary>
            /// <param name="nActionStep"></param>
            /// <param name="onlyCommander">
            /// 이 값이 true이면 발신자 정보만 가져온다.
            /// 이 값이 false이면 수신자 정보도 가져온다.
            /// </param>
            /// <returns>
            /// long => TeamType(int, 상위 4Byte) + TeamID(int, 하위 4Byte)
            /// Team Type : (0 : 평일 비상 조직, 1 : 휴일 비상 조직, 2 : 외부 기관, 3 : 사용자 정의 조직, 4 : 상시조직)
            /// </returns>
            public List<long> GetSOPTeamIDs(int nActionStep, bool onlyCommander)
            {
                List<long> teamIDs = new List<long>();

                GetProcessTeamIDs(nActionStep, teamIDs, !onlyCommander);
                //GetSOPTeamIDs(nActionStep, "Process", teamIDs, onlyCommander);
                GetSOPTeamIDs(nActionStep, "InternalTransmission", teamIDs, !onlyCommander, true);

                return teamIDs;
            }

            private void GetProcessTeamIDs(int nActionStep, List<long> teamIDs, bool selectTeamList)
            {
                string strFormat = "SELECT pm.CommanderMemberID, pm.CommanderMemberType FROM ProcessMission as pm, Process as p, StepMember as sm ";
                strFormat += "where pm.processID = p.ID and p.StepMemberID = sm.ID and sm.ActionStepID = {0} and pm.CommanderMemberType >= 0 and pm.CommanderMemberType <= 4";

                string strSQL = string.Format(strFormat, nActionStep);
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 1; i += 2)
                {
                    string szCommanderID = WebDBManager.GetStringField(arrResult[i], "");
                    DBUtility.VariousData<int> commanderMemberType = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                    if (commanderMemberType != null && (commanderMemberType.Data >= 0 && commanderMemberType.Data <= 4))
                    {
                        int nComID = -1;
                        if (int.TryParse(szCommanderID, out nComID))
                        {
                            long id = (((long)commanderMemberType.Data) << 32) | (long)nComID;

                            if (!teamIDs.Contains(id))
                                teamIDs.Add(id);
                        }
                    }
                }

                if (selectTeamList)
                    GetSOPTeamIDs(nActionStep, "Process", teamIDs, selectTeamList, false);
            }

            private void GetSOPTeamIDs(int nActionStep, string strTableName, List<long> teamIDs, bool selectTeamList, bool selectCommander)
            {
                string strFormat = "SELECT TeamList, CommanderMemberID, CommanderMemberType FROM {0} as p " +
                " INNER JOIN StepMember as sm on sm.ID = p.StepMemberID and sm.ActionStepID = {1} " +
                " WHERE ( p.TeamList like '%(0)%' OR p.TeamList like '%(1)%' or p.TeamList like '%(2)%' or p.TeamList like '%(3)%' or p.TeamList like '%(4)%' or p.TeamList like '%(10)%' " +
                " OR (p.CommanderMemberType >= 0 and p.CommanderMemberType <= 4) )";

                string strSQL = string.Format(strFormat, strTableName, nActionStep);
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 2; i += 3)
                {
                    string strTeamList = WebDBManager.GetStringField(arrResult[i], "");
                    string szCommanderID = WebDBManager.GetStringField(arrResult[i + 1], "");
                    DBUtility.VariousData<int> commanderMemberType = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                    if (selectTeamList)
                    {
                        string[] arrTokens = strTeamList.Split(',');
                        foreach (string strToken in arrTokens)
                        {
                            int nIndex1 = strToken.IndexOf('(');
                            int nIndex2 = strToken.IndexOf(')');

                            if (nIndex1 > 0 && nIndex2 > nIndex1 + 1)
                            {
                                string strID = strToken.Substring(0, nIndex1).Trim();
                                string strType = strToken.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

                                int nID, nType;

                                if (int.TryParse(strID, out nID) && int.TryParse(strType, out nType))
                                {
                                    if ((nType < 0 || nType > 4) && nType != 10)
                                        continue;

                                    long id = (((long)nType) << 32) | (long)nID;

                                    if (!teamIDs.Contains(id))
                                        teamIDs.Add(id);
                                }
                            }
                        }
                    }

                    if (selectCommander)
                    {
                        if (commanderMemberType != null && (commanderMemberType.Data >= 0 && commanderMemberType.Data <= 4))
                        {
                            int nComID = -1;
                            if (int.TryParse(szCommanderID, out nComID))
                            {
                                long id = (((long)commanderMemberType.Data) << 32) | (long)nComID;

                                if (!teamIDs.Contains(id))
                                    teamIDs.Add(id);
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// 해당 ActionStep에서 사용중인 UserDfeinedTeam을 가져오는 함수
            /// </summary>
            /// <param name="nActionStep"></param>
            /// <returns></returns>
            /*public ArrayList GetUsingUserDefineTeams( int nActionStep)
            {
                WebDBManager dbMgr = m_dbMgr;
                ArrayList arIDs = new ArrayList();

                string szTemp = "SELECT TeamList, CommanderMemberID, CommanderMemberType FROM Process as p " +
                " INNER JOIN StepMember as sm on sm.ID = p.StepMemberID and sm.ActionStepID = {0} " +
                " WHERE ( p.TeamList like '%(3)%' OR p.CommanderMemberType = 3 )";

                string strSQL = string.Format(szTemp, nActionStep);

                ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return arIDs;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 2; i += 3)
                {
                    string strTeamList = WebDBManager.GetStringField(arrResult[i], "");
                    string szCommanderID = WebDBManager.GetStringField(arrResult[i + 1], "");
                    DBUtility.VariousData<int> commanderMemberType = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                    string[] arrTokens = strTeamList.Split(',');
                    foreach (string strToken in arrTokens)
                    {
                        int nIndex1 = strToken.IndexOf('(');
                        int nIndex2 = strToken.IndexOf(')');

                        if (nIndex1 > 0 && nIndex2 > nIndex1 + 1)
                        {
                            string strID = strToken.Substring(0, nIndex1).Trim();
                            string strType = strToken.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

                            int nID, nType;

                            if (int.TryParse(strID, out nID) && int.TryParse(strType, out nType))
                            {
                                if (nType != 3)
                                    continue;

                                if (!arIDs.Contains(nID))
                                    arIDs.Add(nID);
                            }
                        }
                    }

                    if (commanderMemberType != null && commanderMemberType.Data == 3)
                    {
                        int nComID = -1;
                        if (int.TryParse(szCommanderID, out nComID))
                        {
                            if (!arIDs.Contains(nComID))
                                arIDs.Add(nComID);
                        }
                    }
                }

                string szTemp2 = "SELECT TeamList, CommanderMemberID, CommanderMemberType FROM InternalTransmission as p " +
                " INNER JOIN StepMember as sm on sm.ID = p.StepMemberID and sm.ActionStepID = {0} " +
                " WHERE ( p.TeamList like '%(3)%' OR p.CommanderMemberType = 3 )";

                string strSQL2 = string.Format(szTemp2, nActionStep);

                ArrayList arrResult2 = dbMgr.GetResultData(strSQL2, 0);

                if (arrResult2 == null)
                    return arIDs;

                int nResultCount2 = arrResult2.Count;

                for (int i = 0; i < nResultCount2 - 2; i += 3)
                {
                    string strTeamList = WebDBManager.GetStringField(arrResult2[i], "");
                    string szCommanderID = WebDBManager.GetStringField(arrResult2[i + 1], "");
                    DBUtility.VariousData<int> commanderMemberType = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                    string[] arrTokens = strTeamList.Split(',');
                    foreach (string strToken in arrTokens)
                    {
                        int nIndex1 = strToken.IndexOf('(');
                        int nIndex2 = strToken.IndexOf(')');

                        if (nIndex1 > 0 && nIndex2 > nIndex1 + 1)
                        {
                            string strID = strToken.Substring(0, nIndex1).Trim();
                            string strType = strToken.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

                            int nID, nType;

                            if (int.TryParse(strID, out nID) && int.TryParse(strType, out nType))
                            {
                                if (nType != 3)
                                    continue;

                                if (!arIDs.Contains(nID))
                                    arIDs.Add(nID);
                            }
                        }
                    }

                    if (commanderMemberType != null && commanderMemberType.Data == 3)
                    {
                        int nComID = -1;
                        if (int.TryParse(szCommanderID, out nComID))
                        {
                            if (!arIDs.Contains(nComID))
                                arIDs.Add(nComID);
                        }
                    }
                }

                return arIDs;
            }*/

            // TeamID, ... 형태로 되어 있는 strTeamList를 분석하여 Team ID들을 얻어온다.
            // ID가 양수이면 하위 조직도 포함하며, 음수이면 자기 자신만 포함한다.
            // ex) 1, 1, -2, 5
            /*private ArrayList GetRegularTeamList(string strTeamList, Dictionary<int, int> dicRegularTeam)
            {
                int nBeginIndex = 0;
                int nLen = strTeamList.Length;

                ArrayList arrRegularTeamIDList = new ArrayList();

                while (nBeginIndex < nLen)
                {
                    int nDotIndex = strTeamList.IndexOf(',', nBeginIndex);
                    if (nDotIndex < 0) break;

                    if (!GetRegularTeamList(strTeamList, nBeginIndex, nDotIndex, dicRegularTeam, arrRegularTeamIDList))
                        return null;

                    nBeginIndex = nDotIndex + 1;
                }

                if (!GetRegularTeamList(strTeamList, nBeginIndex, nLen, dicRegularTeam, arrRegularTeamIDList))
                    return null;

                return arrRegularTeamIDList;
            }

            private bool GetRegularTeamList(string strTeamList, int nBeginIndex, int nEndIndex, Dictionary<int, int> dicRegularTeam, ArrayList arrRegularTeamList)
            {
                if (strTeamList.Length == 0)
                    return true;

                string strID = strTeamList.Substring(nBeginIndex, nEndIndex - nBeginIndex);
                strID = DBUtility.Utility.TrimString(strID);
                if (strID == null || strID.ToString() == "null")
                    return true;
                try
                {
                    int nID = int.Parse(strID);
                    GetRegularTeamList(nID, dicRegularTeam, arrRegularTeamList);
                }
                catch (Exception)
                {
                    return true;
                }

                return true;
            }*/

            private void GetRegularTeamList(int nID, Dictionary<int, int> dicRegularTeam, ArrayList arrRegularTeamList)
            {
                if (nID < 0)
                    arrRegularTeamList.Add(-nID);
                else
                {
                    foreach (KeyValuePair<int, int> pair in dicRegularTeam)
                    {
                        if (pair.Key == nID)
                        {
                            if (!arrRegularTeamList.Contains(pair.Key))
                                arrRegularTeamList.Add(pair.Key);
                        }
                        else if (pair.Value == nID)
                        {
                            if (!arrRegularTeamList.Contains(pair.Key))
                                arrRegularTeamList.Add(pair.Key);
                            GetRegularTeamList(pair.Key, dicRegularTeam, arrRegularTeamList);
                        }
                    }
                }
            }

            public List<TemporaryMember> GetTemporaryMembers(int nTeamID, bool isNormal)
            {
                Dictionary<int, List<TemporaryMember>> dicTemporaryTeamID = isNormal ? m_dicTemporaryNormalTeamID : m_dicTemporaryEmergencyTeamID;
                List<TemporaryMember> members;

                if (!dicTemporaryTeamID.TryGetValue(nTeamID, out members))
                    return null;

                return members;
            }

            public TemporaryMember GetTemporaryMember(int nMemberID)
            {
                TemporaryMember member;

                if (m_dicTemporaryMembers.TryGetValue(nMemberID, out member))
                    return member;

                return null;
            }

            public Data_NormalTeam GetTemporaryNormalTeam(int nTeamID)
            {
                Data_NormalTeam team;

                if (m_dicTemporaryNormalTeam.TryGetValue(nTeamID, out team))
                    return team;

                return null;
            }

            public Data_EmergencyTeam GetTemporaryEmergencyTeam(int nTeamID)
            {
                Data_EmergencyTeam team;

                if (m_dicTemporaryEmergencyTeam.TryGetValue(nTeamID, out team))
                    return team;

                return null;
            }

            //public TemporaryMember GetTemporaryMember(int nMemberID, bool isNormal)
            //{
            //    Dictionary<int, List<TemporaryMember>> dicTemporaryTeamID = isNormal ? m_dicTemporaryNormalTeamID : m_dicTemporaryEmergencyTeamID;

            //    List<TemporaryMember> members = null;

            //    if (dicTemporaryTeamID.TryGetValue(nMemberID, out members))
            //    {
            //        TemporaryMember subMember = null, generalMember = null;

            //        foreach (TemporaryMember member in members)
            //        {
            //            if (member._RoleType == TemporaryMember.RoleType.Main || member._RoleType == TemporaryMember.RoleType.TeamLeader)
            //                return member;
            //            else if (member._RoleType == TemporaryMember.RoleType.Sub)
            //                subMember = member;
            //            else if (member._RoleType == TemporaryMember.RoleType.General)
            //                generalMember = member;
            //        }

            //        if (subMember != null)
            //            return subMember;
            //        else if (generalMember != null)
            //            return generalMember;
            //    }
            //    /*foreach (KeyValuePair<int, List<TemporaryMember>> pair in dicTemporaryTeamID)
            //    {
            //        foreach (TemporaryMember member in pair.Value)
            //        {
            //            if (member.MemberID == nMemberID)
            //                return member;
            //        }
            //    }*/

            //    return null;
            //}

            // 특정 비상조직에 속해있는 전체 팀원 리스트를 얻어온다.
            // 팀원들이 중복해서 입력되지는 않는다.
            // nTeamID : isNormal이 true이면 평일 비상조직, false이면 야간 및 휴일 비상조직의 ID
            // arrMemberList : 전체 팀원 리스트
            public bool GetCompanyMemberList(int nTeamID, bool isNormal, ref ArrayList arrMemberList)
            {
                //if (arrMemberList == null)
                //   return false;

                Dictionary<int, List<TemporaryMember>> dicTemporaryTeamID = isNormal ? m_dicTemporaryNormalTeamID : m_dicTemporaryEmergencyTeamID;
                List<TemporaryMember> members;

                if (!dicTemporaryTeamID.TryGetValue(nTeamID, out members))
                    return false;
                /*Dictionary<int, ArrayList> dicTemporaryTeamID = isNormal ? m_dicTemporaryNormalTeamID : m_dicTemporaryEmergencyTeamID;

                if (!dicTemporaryTeamID.ContainsKey(nTeamID))
                    return false;

                ArrayList arrRegularTeamIDList = dicTemporaryTeamID[nTeamID];*/

                foreach (TemporaryMember _member in members)
                //foreach (int nRegularTeamID in arrRegularTeamIDList)
                {
                    if (_member._MemberType != TemporaryMember.MemberType.RegularTeam)
                        continue;
                    //if (nRegularTeamID < 0)
                    //    continue;

                    if (_member._MemberType == TemporaryMember.MemberType.RegularTeam)
                    {
                        int nRegularTeamID = _member.MemberID;

                        if (!m_dicRegularTeamMember.ContainsKey(nRegularTeamID))
                        {
                            // 조직체계상 nRegularTeamID에 해당하는 팀은 존재하지만 팀원은 한명도 없는 경우
                            continue;
                        }

                        ArrayList arrTeamMember = m_dicRegularTeamMember[nRegularTeamID];

                        if (arrTeamMember == null)
                            return false;

                        foreach (Data_CompanyMember member in arrTeamMember)
                        {
                            //arrRegularTeamIDList.Add(member);
                            arrMemberList.Add(member);
                        }
                    }
                    else if (_member._MemberType == TemporaryMember.MemberType.CompanyMember)
                    {
                        Data_CompanyMember member;

                        if (!m_dicCompanyMember.TryGetValue(_member.MemberID, out member))
                            continue;

                        if (!arrMemberList.Contains(member))
                            arrMemberList.Add(member);
                    }
                }

                return true;
            }

            public Data_RegularTeam GetRegularTeam(int nTeamID)
            {
                Data_RegularTeam team;

                if (m_dicRegularTeam.TryGetValue(nTeamID, out team))
                    return team;

                return null;
            }

            public Data_CompanyMember GetRegularCompanyMember(int nCompanyMemberID)
            {
                Data_CompanyMember member;

                if (m_dicCompanyMember.TryGetValue(nCompanyMemberID, out member))
                    return member;

                return null;
            }

            public Data_ControlRoom GetControlRoom(int nTeamID)
            {
                Data_ControlRoom team;

                if (m_dicControlRoom.TryGetValue(nTeamID, out team))
                    return team;

                return null;
            }

            // 모든 교대근무자는 하위팀을 포함하여 리턴한다.
            // m_dicControlRoomMembers에는 현재 근무중인 멤버들의 정보만 포함되어 있다.
            public List<Data_ControlRoomMember> GetControlRoomMembers(int nTeamID)
            {
                //int nTeamID = (nJobPosition << 16) | (nRoomID << 8) | nRoomType;
                int nJobPosition = (nTeamID >> 16);
                int nRoomID = (nTeamID & 0xff00);
                int nRoomType = (nTeamID & 0xff);

                List<Data_ControlRoomMember> members = new List<Data_ControlRoomMember>();
                
                if (nTeamID == Data_ControlRoom.ROOT_ID)
                {
                    foreach (KeyValuePair<int, Data_ControlRoomMember> pair in m_dicControlRoomMembers)
                    {
                        members.Add(pair.Value);
                    }
                }
                else
                {
                    Dictionary<int, Data_ControlRoom> dicControlRooms = new Dictionary<int,Data_ControlRoom>();
                    GetControlRooms(nRoomType, nRoomID, nJobPosition, dicControlRooms);

                    Data_ControlRoomMember member = null;

                    foreach (KeyValuePair<int, Data_ControlRoom> pair in dicControlRooms)
                    {
                        if (m_dicControlRoomMembers.TryGetValue(pair.Key, out member))
                            members.Add(member);
                    }
                }

                return members;
            }

            private void GetControlRooms(int nRoomType, int nRoomID, int nJobPosition, Dictionary<int, Data_ControlRoom> dicControlRooms)
            {
                foreach (KeyValuePair<int, Data_ControlRoom> pair in m_dicControlRoom)
                {
                    int _nJobPosition = (pair.Key >> 16);
                    int _nRoomID = (pair.Key & 0xff00);
                    int _nRoomType = (pair.Key & 0xff);

                    if (nRoomID == 0 && nJobPosition == 0)
                    {
                        if (nRoomType == _nRoomType)
                            dicControlRooms[pair.Key] = pair.Value;
                    }
                    else if (nJobPosition == 0)
                    {
                        if (nRoomType == _nRoomType && nRoomID == _nRoomID)
                            dicControlRooms[pair.Key] = pair.Value;
                    }
                    else
                    {
                        if (nRoomType == _nRoomType && nRoomID == _nRoomID && nJobPosition == _nJobPosition)
                            dicControlRooms[pair.Key] = pair.Value;
                    }
                }
            }

            public bool GetRegularCompanyMemberList(int nRegularTeamID, ref ArrayList arrMemberList)
            {
                // 조직체계상 nRegularTeamID에 해당하는 팀은 존재하지만 티원은 한명도 없는 경우
                if (!m_dicRegularTeamMember.ContainsKey(nRegularTeamID))
                    return true;

                ArrayList arrTeamMember = m_dicRegularTeamMember[nRegularTeamID];

                if (arrTeamMember == null)
                    return false;

                foreach (Data_CompanyMember member in arrTeamMember)
                {
                    arrMemberList.Add(member);
                }

                return true;
            }

            public List<Data_CompanyMember> GetAllRegularCompanyMemberList()
            {
                List<Data_CompanyMember> members = new List<Data_CompanyMember>();

                foreach (KeyValuePair<int, Data_CompanyMember> pair in m_dicCompanyMember)
                {
                    members.Add(pair.Value);
                }

                return members;
            }

            public Data_ExternalTeam GetUserDefinedTeamMember(int nUserDefinedTeamID)
            {
                if (!m_dicUserDefinedTeam.ContainsKey(nUserDefinedTeamID))
                    return null;

                return m_dicUserDefinedTeam[nUserDefinedTeamID];
            }

            public ActionStepInfo GetActionStepInfo(int nActionStepID)
            {
                if (m_dicActionStepInfo.ContainsKey(nActionStepID))
                {
                    return m_dicActionStepInfo[nActionStepID];
                }

                return null;
            }

            public void RemoveActionStepInfo(int nActionStepID)
            {
                m_dicActionStepInfo.Remove(nActionStepID);
            }

            public void SetActionStepInfo(ActionStepInfo actionStepInfo)
            {
                m_dicActionStepInfo[actionStepInfo.ActionStepID] = actionStepInfo;
            }

            public VersionInfo GetActionStepVersionInfo(int nActionStepID)
            {
                if (m_dicActionStepInfo.ContainsKey(nActionStepID))
                {
                    ActionStepInfo actionStep = m_dicActionStepInfo[nActionStepID];

                    if (m_dicVersionRegularNormal.ContainsKey(actionStep.DisasterID))
                        return m_dicVersionRegularNormal[actionStep.DisasterID];
                    else if (m_dicVersionRegularEmergency.ContainsKey(actionStep.DisasterID))
                        return m_dicVersionRegularEmergency[actionStep.DisasterID];
                    else if (m_dicVersionNonRegularNormal.ContainsKey(actionStep.DisasterID))
                        return m_dicVersionNonRegularNormal[actionStep.DisasterID];
                    else if (m_dicVersionNonRegularEmergency.ContainsKey(actionStep.DisasterID))
                        return m_dicVersionNonRegularEmergency[actionStep.DisasterID];
                }

                return null;
            }

            public void RemoveActionStepVersionInfo(int nActionStepID)
            {
                if (m_dicActionStepInfo.ContainsKey(nActionStepID))
                {
                    ActionStepInfo actionStep2 = m_dicActionStepInfo[nActionStepID];

                    if (m_dicVersionRegularNormal.ContainsKey(actionStep2.DisasterID))
                        m_dicVersionRegularNormal.Remove(actionStep2.DisasterID);
                    else if (m_dicVersionRegularEmergency.ContainsKey(actionStep2.DisasterID))
                        m_dicVersionRegularEmergency.Remove(actionStep2.DisasterID);
                    else if (m_dicVersionNonRegularNormal.ContainsKey(actionStep2.DisasterID))
                        m_dicVersionNonRegularNormal.Remove(actionStep2.DisasterID);
                    else if (m_dicVersionNonRegularEmergency.ContainsKey(actionStep2.DisasterID))
                        m_dicVersionNonRegularEmergency.Remove(actionStep2.DisasterID);
                }
            }

            public void SetActionStepVersionInfo(ActionStepInfo actionStep, VersionInfo version)
            {
                if (version.IsNormal)
                {
                    if (version.IsRegular)
                        m_dicVersionRegularNormal[actionStep.DisasterID] = version;
                    else
                        m_dicVersionNonRegularNormal[actionStep.DisasterID] = version;
                }
                else
                {
                    if (version.IsRegular)
                        m_dicVersionRegularEmergency[actionStep.DisasterID] = version;
                    else
                        m_dicVersionNonRegularEmergency[actionStep.DisasterID] = version;
                }
            }

            public ArrayList GetSOPDisasterList(string strFullPath, bool isRegular, bool isNormal)
            {
                Dictionary<string, ArrayList> dicSOPDisasterList = GetSOPDisasterListDictionary(isRegular, isNormal);

                if (dicSOPDisasterList.ContainsKey(strFullPath))
                    return dicSOPDisasterList[strFullPath];

                return null;
            }

            public Data_ExternalTeam GetUserDefinedTeam(int nTeamID)
            {
                if (!m_dicUserDefinedTeam.ContainsKey(nTeamID))
                    return null;

                return m_dicUserDefinedTeam[nTeamID];
            }

            public Data_ExternalTeam GetExternalTeam(int nTeamID)
            {
                if (!m_dicExternalTeam.ContainsKey(nTeamID))
                    return null;

                return m_dicExternalTeam[nTeamID];
            }

            public void SetActionStepHistoryID(int nActionStepID, bool isRealMode, int nActionStepHistoryID)
            {
                long nHi = isRealMode ? (1 << 32) : 0;
                long nLow = nActionStepID;
                long nKey = nHi | nLow;
                m_dicActionStepHistory[nKey] = nActionStepHistoryID;


                UnE.SOP.Sections.ISOPPageContainer container = ProxySOP.Instance.PageContainer;
                if( container != null)
                {
                    container.SetActionStepHistoryID(nActionStepID, isRealMode, nActionStepHistoryID);
                }                
            }

            public void NewActionStepHistory(int nActionStepHistoryID)
            {
                UnE.SOP.Sections.ISOPPageContainer container = ProxySOP.Instance.PageContainer;
                if (container != null)
                {
                    container.NewActionStepHistory(nActionStepHistoryID);
                }
            }

            public int GetActionStepHistoryID(int nActionStepID, bool isRealMode)
            {
                long nHi = isRealMode ? (1 << 32) : 0;
                long nLow = nActionStepID;
                long nKey = nHi | nLow;

                if (m_dicActionStepHistory.ContainsKey(nKey))
                    return m_dicActionStepHistory[nKey];

                return -1;
            }

            // 종료된 SOP의 ActionStepHistoryID 정보를 삭제한다.
            public void RemoveActionStepHistoryID(int nActionStepID, bool isRealMode)
            {
                long nHi = isRealMode ? (1 << 32) : 0;
                long nLow = nActionStepID;
                long nKey = nHi | nLow;

                m_dicActionStepHistory.Remove(nKey);
            }

            public void SetCurrentActionStep(int nActionStepID, bool isRealMode)
            {
                m_nCurrentActionStepID = nActionStepID;
                m_isCurrentRealMode = isRealMode;
            }

            public int GetCUrrentActionStep(out bool isRealMode)
            {
                isRealMode = m_isCurrentRealMode;
                return m_nCurrentActionStepID;
            }
        }

    }
}