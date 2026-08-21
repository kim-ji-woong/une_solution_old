using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sections;
using System.Collections;
using UnE.SOP;
using UnE.SOP.Sections;

namespace SOPMonitoringSystem
{
    public class SectionCommanderInfo
    {
        public static SectionCommander GetCommanderInfo(Sections.Section section, out string strCommanderName, out string strCommanderName2, out string strCommanderPhoneNumber)
        {
            strCommanderName = strCommanderName2 = strCommanderPhoneNumber = "";
            SectionCommander commander = null;

            Sections.Section.ComponentType type = section.GetComponentType();

            if (type == Sections.Section.ComponentType.PROCESS)
            {
                Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

                if (data.Commander != null)
                    commander = data.Commander.Clone();
            }
            else if (type == Sections.Section.ComponentType.INTERNAL)
            {
                Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;

                if (data.Commander != null)
                    commander = data.Commander.Clone();
            }

            if (commander != null)
            {
                strCommanderName = GetSectionCommanderName(commander, out strCommanderName2, out strCommanderPhoneNumber);
            }

            return commander;
        }

        private static string GetSectionCommanderName(Sections.SectionCommander commander, out string strCommanderName2, out string strPhoneNumber)
        {
            string strDisplayText = null;
            strPhoneNumber = strCommanderName2 = "";

            if (commander == null)
                return "";

            if (commander.DisplayText != null && commander.DisplayText.Length > 0)
                strDisplayText = commander.DisplayText;

            if (commander.Team == null)
            {
                bool isDayLight = Popup.SOPLoader.IsNormal(DateTime.Now);

                if (isDayLight)
                {
                    if (FormSOP.Instance.SOPGenUserCommanderDayLight != null)
                        commander = FormSOP.Instance.SOPGenUserCommanderDayLight;
                    else if (FormSOP.Instance.SOPGenUserCommanderNightHoliday != null)
                        commander = FormSOP.Instance.SOPGenUserCommanderNightHoliday;
                    else
                        return "";
                }
                else
                {
                    if (FormSOP.Instance.SOPGenUserCommanderNightHoliday != null)
                        commander = FormSOP.Instance.SOPGenUserCommanderNightHoliday;
                    else if (FormSOP.Instance.SOPGenUserCommanderDayLight != null)
                        commander = FormSOP.Instance.SOPGenUserCommanderDayLight;
                    else
                        return "";
                }
            }

            strCommanderName2 = GetCommanderMemberName(commander, ref strPhoneNumber);

            if (strDisplayText == null)
                strDisplayText = strCommanderName2;

            if (commander.CallerPhoneNumber != null && commander.CallerPhoneNumber.Length > 0)
                strPhoneNumber = commander.CallerPhoneNumber;

            return strDisplayText;
        }

        private static string GetCommanderMemberName(Sections.SectionCommander commander, ref string strPhoneNumber)
        {
            string strDisplayText = null;

            if (commander.Team == null)
                return "";

            if (commander.DisplayText != null && commander.DisplayText.Length > 0)
                strDisplayText = commander.DisplayText;

            if (commander.IsTeamMember)
            {
                ArrayList arrMembers = new ArrayList();

                if (commander.Team.TeamType == SOPTeam.SOPTeamType.Regular)
                {
                    if (!FormSOP.Instance.SOPManager.GetRegularCompanyMemberList(commander.Team.TeamID, ref arrMembers))
                        return "";

                    foreach (Data_CompanyMember member in arrMembers)
                    {
                        if (member.ID == commander.TeamMemberID)
                        {
                            if (strDisplayText == null)
                                strDisplayText = member.MemberName;
                            strPhoneNumber = member.PhoneNumber;
                            break;
                        }
                    }
                }
                else if (commander.Team.TeamType == SOPTeam.SOPTeamType.External)
                {
                    foreach (ExternalCompanyMember member in FormSOP.Instance.SOPManager.ExternalCompanyMembers)
                    {
                        if (member.ID == commander.TeamMemberID)
                        {
                            if (strDisplayText == null)
                                strDisplayText = member.MemberName;
                            strPhoneNumber = member.PhoneNumber;
                            break;
                        }
                    }
                }
                else if (commander.Team.TeamType == SOPTeam.SOPTeamType.Normal)
                {
                    if (!GetTemporaryMemberInfo(commander.TeamMemberID, true, ref strDisplayText, ref strPhoneNumber))
                        return "";
                }
                else if (commander.Team.TeamType == SOPTeam.SOPTeamType.Holiday)
                {
                    if (!GetTemporaryMemberInfo(commander.TeamMemberID, false, ref strDisplayText, ref strPhoneNumber))
                        return "";
                }
                else if (commander.Team.TeamType == SOPTeam.SOPTeamType.ControlRoom)
                {
                    if (!FormSOP.Instance.SOPManager.ControlRoomMembers.ContainsKey(commander.Team.TeamID))
                        return "";
                    else
                    {
                        strPhoneNumber = FormSOP.Instance.SOPManager.ControlRoomMembers[commander.Team.TeamID].PhoneNumber;
                        if (strDisplayText == null)
                            strDisplayText = FormSOP.Instance.SOPManager.ControlRoomMembers[commander.Team.TeamID].MemberName;
                    }
                }
            }
            else
            {
                if (commander.Team.TeamType == SOPTeam.SOPTeamType.Regular)
                {
                    Data_CompanyMember companyMember;
                    Data_RegularTeam team = GetRegularTeamLeaderInfo(commander.Team.TeamID, out companyMember);

                    if (team == null)
                        return "";

                    strPhoneNumber = companyMember.PhoneNumber;

                    if (strDisplayText != null)
                        strDisplayText = team.TeamName + "장";
                }
                else if (commander.Team.TeamType == SOPTeam.SOPTeamType.External)
                {
                    ExternalCompanyTeam team = FormSOP.Instance.SOPManager.FindExternalCompanyTeam(commander.Team.TeamID);

                    if (team == null)
                        return "";

                    if (team.Members == null || team.Members.Count == 0)
                        return "";

                    ExternalCompanyMember member = team.Members[0];

                    strPhoneNumber = member.PhoneNumber;

                    if (strDisplayText != null)
                        strDisplayText = team.TeamName;
                }
                else if (commander.Team.TeamType == SOPTeam.SOPTeamType.UserDefined)
                {

                    // Edit by skkim 2015-08-31
                    // action step에서 사용중인 UserDefine팀은 TabPage에 저장된다.
                    // 각 ActionStep마다 다른 UserDefine팀을 갖도록 수정함
                    //Data_ExternalTeam team = FormSOP.Instance.SOPManager.GetUserDefinedTeam(commander.Team.TeamID);
                    SectionTabPage page = (SectionTabPage)FormSOP.Instance.GetPageHome().TabControls.SelectedTab;
                    if (page != null)
                    {
                        Data_UserDefinedTeam team = page.GetUserDefinedTeamMember(commander.Team.TeamID);
                        if (team == null)
                            return "";

                        if (team.Tag != null)
                        {
                            DataRoleMember roleMember = (DataRoleMember)team.Tag;
                            strPhoneNumber = roleMember.PhoneNumber == null ? "" : roleMember.PhoneNumber;
                        }

                        if (strDisplayText != null)
                            strDisplayText = team.TeamName;
                    }
                }
                else if (commander.Team.TeamType == SOPTeam.SOPTeamType.Normal)
                {
                    TemporaryMember member = GetTemporaryMainMember(commander.Team.TeamID, true);

                    string strDisplayText2 = "";

                    if (!GetTemporaryMemberInfo(member, ref strDisplayText2, ref strPhoneNumber))
                        return "";

                    if (strDisplayText == null)
                        strDisplayText = strDisplayText2;
                }
                else if (commander.Team.TeamType == SOPTeam.SOPTeamType.Holiday)
                {
                    TemporaryMember member = GetTemporaryMainMember(commander.Team.TeamID, false);

                    string strDisplayText2 = "";

                    if (!GetTemporaryMemberInfo(member, ref strDisplayText2, ref strPhoneNumber))
                        return "";

                    if (strDisplayText == null)
                        strDisplayText = strDisplayText2;
                }
                else if (commander.Team.TeamType == SOPTeam.SOPTeamType.ControlRoom)
                {
                    if (!FormSOP.Instance.SOPManager.ControlRoomMembers.ContainsKey(commander.Team.TeamID))
                        return "";
                    else
                    {
                        strPhoneNumber = FormSOP.Instance.SOPManager.ControlRoomMembers[commander.Team.TeamID].PhoneNumber;
                        if (strDisplayText == null)
                            strDisplayText = FormSOP.Instance.SOPManager.ControlRoomMembers[commander.Team.TeamID].MemberName;
                    }
                }
            }

            if (strDisplayText == null)
                return "";

            return strDisplayText;
        }

        public static List<TemporaryMember> GetTemporaryMembers(int nTeamID, bool isDayLight, bool includeMain, bool includeSub, bool includeTeamLeader, bool includeOthers)
        {
            List<TemporaryMember> members2 = new List<TemporaryMember>();
            List<TemporaryMember> members = FormSOP.Instance.SOPManager.GetTemporaryMembers(nTeamID, isDayLight);

            if (members == null || members.Count == 0)
                return members2;

            foreach (TemporaryMember member in members)
            {
                if (member._RoleType == TemporaryMember.RoleType.Main && includeMain)
                    members2.Add(member);
                else if (member._RoleType == TemporaryMember.RoleType.Sub && includeSub)
                    members2.Add(member);
                else if (member._RoleType == TemporaryMember.RoleType.TeamLeader && includeTeamLeader)
                    members2.Add(member);
                else if (includeOthers)
                    members2.Add(member);
            }

            return members2;
        }

        // 1. 비상조직의 [정] 관리자를 찾는다.
        // 2. [정]이 없으면 [부] 관리자를 찾는다.
        // 3. 그도 없으면 해당 조직의 아무나 리턴한다.
        private static TemporaryMember GetTemporaryMainMember(int nTeamID, bool isDayLight)
        {
            List<TemporaryMember> members = FormSOP.Instance.SOPManager.GetTemporaryMembers(nTeamID, isDayLight);

            if (members == null || members.Count == 0)
                return null;

            TemporaryMember subMain = null, teamLeader = null;

            foreach (TemporaryMember member in members)
            {
                if (member._RoleType == TemporaryMember.RoleType.Main)
                    return member;
                else if (member._RoleType == TemporaryMember.RoleType.Sub && subMain == null)
                    subMain = member;
                else if (member._RoleType == TemporaryMember.RoleType.TeamLeader && teamLeader == null)
                    teamLeader = member;
            }

            if (subMain != null)
                return subMain;

            if (teamLeader != null)
                return teamLeader;

            return members[0];
        }

        private static bool GetTemporaryMemberInfo(int nMemberID, bool isDayLight, ref string strDisplayText, ref string strPhoneNumber)
        {
            TemporaryMember member = FormSOP.Instance.SOPManager.GetTemporaryMember(nMemberID);

            if (member == null)
                return false;

            string strDisplayText2 = "";

            if (!GetTemporaryMemberInfo(member, ref strDisplayText2, ref strPhoneNumber))
                return false;

            if (strDisplayText == null)
                strDisplayText = strDisplayText2;

            return true;
        }

        public static bool GetTemporaryMemberInfo(TemporaryMember member, ref string strDisplayName, ref string strPhoneNumber, ref string strMemberName)
        {
            if (member == null)
                return false;

            strDisplayName = member.MemberName;

            if (member._MemberType == TemporaryMember.MemberType.RegularTeam)
            {
                Data_CompanyMember companyMember;
                Data_RegularTeam team = GetRegularTeamLeaderInfo(member.MemberID, out companyMember);

                if (team == null)
                    return false;

                strMemberName = companyMember.MemberName;
                strPhoneNumber = companyMember.PhoneNumber;
                return true;
            }
            else if (member._MemberType == TemporaryMember.MemberType.ExternalCompanyTeam)
            {
                ExternalCompanyTeam team = FormSOP.Instance.SOPManager.FindExternalCompanyTeam(member.MemberID);

                if (team == null)
                    return false;

                return GetExternalFirstMemberPhoneNumber(team, ref strPhoneNumber, ref strMemberName);
            }
            else if (member._MemberType == TemporaryMember.MemberType.ExternalTeam)
            {
                List<ExternalCompanyTeam> teams = FormSOP.Instance.SOPManager.GetExternalCompanyTeams(member.MemberID);

                if (teams == null || teams.Count == 0)
                    return false;

                ExternalCompanyTeam team = teams[0];
                return GetExternalFirstMemberPhoneNumber(team, ref strPhoneNumber, ref strMemberName);
            }
            else if (member._MemberType == TemporaryMember.MemberType.CompanyMember)
            {
                Data_CompanyMember companyMember = FormSOP.Instance.SOPManager.GetRegularCompanyMember(member.MemberID);

                if (companyMember == null)
                    return false;

                strPhoneNumber = companyMember.PhoneNumber;
                strMemberName = companyMember.MemberName;
                return true;
            }
            else if (member._MemberType == TemporaryMember.MemberType.ExternalCompanyMember)
            {
                foreach (ExternalCompanyMember externalMember in FormSOP.Instance.SOPManager.ExternalCompanyMembers)
                {
                    if (externalMember.ID == member.MemberID)
                    {
                        strPhoneNumber = externalMember.PhoneNumber;
                        strMemberName = externalMember.MemberName;
                        return true;
                    }
                }

                return false;
            }
            else if (member._MemberType == TemporaryMember.MemberType.UserDefinedTeam)
            {
                Data_ExternalTeam team = FormSOP.Instance.SOPManager.GetUserDefinedTeam(member.MemberID);

                if (team == null)
                    return false;

                strPhoneNumber = team.PhoneNumber;
                return true;
            }

            return false;
        }

        private static bool GetTemporaryMemberInfo(TemporaryMember member, ref string strDisplayName, ref string strPhoneNumber)
        {
            string strMemberName = "";
            return GetTemporaryMemberInfo(member, ref strDisplayName, ref strPhoneNumber, ref strMemberName);
        }

        private static Data_RegularTeam GetRegularTeamLeaderInfo(int nTeamID, out Data_CompanyMember member)
        {
            member = null;
            Data_RegularTeam team = LoadRegularTeam(nTeamID);

            if (team == null)
                return null;

            int nCompanyMemberID = GetRegularTeamLeaderID(team.ID);

            if (nCompanyMemberID < 0)
            {
                // 명시적으로 팀장이 선언되어 있지 않으면 팀장과 가장 가까운 직책을 선택한다.
                // 그마저도 없으면 가장 먼저 등록된 팀원을 리턴한다.
                nCompanyMemberID = GetRegularTeamLeaderID(team);

                if (nCompanyMemberID < 0)
                    return null;
            }

            member = FormSOP.Instance.SOPManager.GetRegularCompanyMember(nCompanyMemberID);

            if (member == null)
                return null;

            return team;
        }

        private static bool GetExternalFirstMemberPhoneNumber(ExternalCompanyTeam team, ref string strPhoneNumber, ref string strMemberName)
        {
            if (team.Members == null || team.Members.Count == 0)
                return false;

            ExternalCompanyMember externalMember = team.Members[0];
            strPhoneNumber = externalMember.PhoneNumber;
            strMemberName = externalMember.MemberName;
            return true;
        }

        // 명시적으로 팀장이 선언되어 있지 않으면 팀장과 가장 가까운 직책을 선택한다.
        // 그마저도 없으면 가장 먼저 등록된 팀원을 리턴한다.
        private static int GetRegularTeamLeaderID(Data_RegularTeam team)
        {
            ArrayList arrMembers = new ArrayList();
            if (FormSOP.Instance.SOPManager.GetRegularCompanyMemberList(team.ID, ref arrMembers) == false)
                return -1;

            Data_CompanyMember teamLeader = null;

            foreach (Data_CompanyMember member in arrMembers)
            {
                if (teamLeader == null)
                    teamLeader = member;
                else
                {
                    int compare = teamLeader.CompareTo(member);

                    if (compare < 0)
                        teamLeader = member;
                    else if (compare == 0)
                    {
                        if (teamLeader.ID > member.ID)
                            teamLeader = member;
                    }
                }
            }

            if (teamLeader == null)
                return -1;

            return teamLeader.ID;
        }

        private static int GetRegularTeamLeaderID(int nTeamID)
        {
            string strSQL = "Select CompanyMemberID from RegularMemberList where RegularTeamID = " + nTeamID.ToString() + " and PositionID = 2";
            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            return WebDBManager.GetIntField(arrResult[0].ToString(), -1);
        }

        private static Data_RegularTeam LoadRegularTeam(int nTeamID)
        {
            string strSQL = "Select TeamName, ParentTeamID from RegularTeam where ID = " + nTeamID.ToString();
            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count != 2)
                return null;

            string strTeamName = WebDBManager.GetStringField(arrResult[0], "");
            int nParentTeamID = WebDBManager.GetIntField(arrResult[1].ToString(), -1);

            if (strTeamName == "null")
                return null;

            Data_RegularTeam team = new Data_RegularTeam();
            team.ID = nTeamID;
            team.ParentTeamID = nParentTeamID;
            team.TeamName = strTeamName;

            return team;
        }
    }
}