using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.SOP.Sections;
using System.Collections;
using UnE.SOP;
using Sections;

namespace SOPMonitoringSystem
{
    public class SMSHelper
    {
        // 문자메시지 발신자 번호
        public static string GetSMSCaller(ISectionContents contents)
        {
            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'SMSCaller' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return "";

            string strPhoneNumber = WebDBManager.GetStringField(arrResult[0]);
            return strPhoneNumber == null ? "" : strPhoneNumber;
        }

        public static void GetSOPTeamPhoneNumbers(SOPTeam team, bool onlyTeamLeader, Dictionary<string, string> dicPhoneNumbers)
        {
            if (team.TeamType == SOPTeam.SOPTeamType.Normal || team.TeamType == SOPTeam.SOPTeamType.Holiday)   // 평일 조직 또는 야간 조직
            {
                List<TemporaryMember> members = new List<TemporaryMember>();

                if (ReadTemporaryTeamMemberList(team.TeamType == SOPTeam.SOPTeamType.Normal, team.IncludeChildTeams, team.TeamID, members))
                {
                    AddTemporaryMemberPhoneNumbers(dicPhoneNumbers, members);
                }
            }
            else if (team.TeamType == SOPTeam.SOPTeamType.External)    // 협력 회사 혹은 외부 기관
            {
                AddExternalMemberPhoneNumbers(team.TeamID, team.IncludeChildTeams, dicPhoneNumbers);
            }
            else if (team.TeamType == SOPTeam.SOPTeamType.UserDefined)    // 사용자 정의 조직
            {
                Data_UserDefinedTeam userDefinedTeam = DataManager.Instance.LoadUserDefinedTeam(team.TeamID);

                if (userDefinedTeam != null)
                {
                    dicPhoneNumbers[userDefinedTeam.PhoneNumber] = userDefinedTeam.TeamName + ";";
                }
            }
            else if (team.TeamType == SOPTeam.SOPTeamType.Regular)    // 정규 조직
            {
                List<DataCompanyMember> companyMembers = DataManager.Instance.GetCompanyMembers(team.TeamID, team.IncludeChildTeams);

                if (companyMembers == null)
                    return;

                foreach (DataCompanyMember companyMember in companyMembers)
                {
                    dicPhoneNumbers[companyMember.PhoneNumber] = GetCompanyMemberInfo(companyMember);
                }
            }
            else if (team.TeamType == SOPTeam.SOPTeamType.ControlRoom)    // 교대근무자
            {
                List<Data_ControlRoomMember> controlRoomMembers = FormSOP.Instance.SOPManager.GetControlRoomMembers(team.TeamID);

                if (controlRoomMembers == null)
                    return;

                foreach (Data_ControlRoomMember member in controlRoomMembers)
                {
                    dicPhoneNumbers[member.PhoneNumber] = team.TeamName + ";" + member.MemberName;
                }
            }
        }

        public static bool ReadTemporaryTeamMemberList(bool isNormal, bool includeChildTeams, int nTeamID, List<TemporaryMember> members)
        {
            return IOManager.ReadTemporaryTeamMemberList((WebDBManager)UnE.SOP.ProxySOP.Instance.DBManager, isNormal, includeChildTeams, nTeamID, members);
        }

        // dicPhoneNumbers : Key와 Value가 같은 값이다.
        //                   중복으로 전화번호가 입력되는걸 방지하기 위해 List 대신 Dictioanry를 사용한다.
        public static void GetTemporaryMemberPhoneNumbers(TemporaryMember member, Dictionary<string, string> dicPhoneNumbers)
        {
            List<string> phoneNumbers = new List<string>();

            if (member._MemberType == TemporaryMember.MemberType.CompanyMember)
            {
                DataCompanyMember companyMember = DataManager.Instance.GetCompanyMember(member.MemberID);

                if (companyMember == null)
                    return;

                dicPhoneNumbers[companyMember.PhoneNumber] = companyMember.PhoneNumber;
            }
            else if (member._MemberType == TemporaryMember.MemberType.RegularTeam)
            {
                if (member.TeamLeader == 1)
                {
                    Data_RegularTeam team = FormSOP.Instance.SOPManager.GetRegularTeam(member.MemberID);

                    if (team == null)
                        return;

                    int nLeaderID = GetRegularTeamLeaderID(team);

                    if (nLeaderID < 0)
                        return;

                    DataCompanyMember companyMember = DataManager.Instance.GetCompanyMember(nLeaderID);

                    if (companyMember == null)
                        return;

                    dicPhoneNumbers[companyMember.PhoneNumber] = companyMember.PhoneNumber;
                }
                else
                {
                    List<DataCompanyMember> companyMembers = DataManager.Instance.GetCompanyMembers(member.MemberID, member.IncludeChildTeams);

                    if (companyMembers == null)
                        return;

                    foreach (DataCompanyMember companyMember in companyMembers)
                    {
                        dicPhoneNumbers[companyMember.PhoneNumber] = companyMember.PhoneNumber;
                    }
                }
            }
            else if (member._MemberType == TemporaryMember.MemberType.ExternalCompanyMember)
            {
                DataExternalMember externalMember = DataManager.Instance.GetExternalMember(member.MemberID);

                if (externalMember != null)
                    dicPhoneNumbers[externalMember.PhoneNumber] = externalMember.PhoneNumber;
            }
            else if (member._MemberType == TemporaryMember.MemberType.ExternalTeam || member._MemberType == TemporaryMember.MemberType.ExternalCompanyTeam)
            {
                AddExternalMemberPhoneNumbers(member.MemberID, member.IncludeChildTeams, dicPhoneNumbers);
            }
            else if (member._MemberType == TemporaryMember.MemberType.UserDefinedTeam)
            {
                string strPhoneNumber, strTeamName;

                if (DataManager.Instance.GetUserDefinedTeamInfo(member.MemberID, out strPhoneNumber, out strTeamName))
                {
                    dicPhoneNumbers[strPhoneNumber] = strTeamName + ";";
                }
            }
            else if (member._MemberType == TemporaryMember.MemberType.JobLevel)
            {
                List<DataCompanyMember> companyMembers = DataManager.Instance.GetCompanyMembers(member.MemberID);

                if (companyMembers == null)
                    return;

                foreach (DataCompanyMember companyMember in companyMembers)
                {
                    dicPhoneNumbers[companyMember.PhoneNumber] = GetCompanyMemberInfo(companyMember);
                }
            }
        }

        private static void AddExternalMemberPhoneNumbers(int nTeamID, bool includeChildTeams, Dictionary<string, string> dicPhoneNumbers)
        {
            List<DataExternalMember> externalMembers = DataManager.Instance.GetExternalMembers(nTeamID, includeChildTeams);

            if (externalMembers == null)
                return;

            foreach (DataExternalMember externalMember in externalMembers)
            {
                dicPhoneNumbers[externalMember.PhoneNumber] = GetExternalMemberInfo(externalMember);
            }
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

        private static void AddTemporaryMemberPhoneNumbers(Dictionary<string, string> dicPhoneNumbers, List<TemporaryMember> members)
        {
            Dictionary<int, string> dicCompanyMemberPhoneNumbers = new Dictionary<int, string>();

            foreach (TemporaryMember member in members)
            {
                if (member._MemberType == TemporaryMember.MemberType.CompanyMember)
                {
                    DataCompanyMember companyMember = DataManager.Instance.GetCompanyMember(member.MemberID);

                    if (companyMember == null)
                        continue;

                    dicPhoneNumbers[companyMember.PhoneNumber] = GetCompanyMemberInfo(companyMember);
                }
                else if (member._MemberType == TemporaryMember.MemberType.RegularTeam)
                {
                    if (member.TeamLeader == 1)
                    {
                        Data_RegularTeam team = FormSOP.Instance.SOPManager.GetRegularTeam(member.MemberID);

                        if (team == null)
                            continue;

                        int nLeaderID = GetRegularTeamLeaderID(team);

                        if (nLeaderID < 0)
                            continue;

                        DataCompanyMember companyMember = DataManager.Instance.GetCompanyMember(nLeaderID);

                        if (companyMember == null)
                            continue;

                        dicPhoneNumbers[companyMember.PhoneNumber] = GetCompanyMemberInfo(companyMember, team);
                    }
                    else
                    {
                        List<DataCompanyMember> companyMembers = DataManager.Instance.GetCompanyMembers(member.MemberID, member.IncludeChildTeams);

                        if (companyMembers == null)
                            continue;

                        foreach (DataCompanyMember companyMember in companyMembers)
                        {
                            dicPhoneNumbers[companyMember.PhoneNumber] = GetCompanyMemberInfo(companyMember);
                        }
                    }
                }
                else if (member._MemberType == TemporaryMember.MemberType.ExternalCompanyMember)
                {
                    DataExternalMember externalMember = DataManager.Instance.GetExternalMember(member.MemberID);

                    if (externalMember != null)
                        dicPhoneNumbers[externalMember.PhoneNumber] = GetExternalMemberInfo(externalMember);
                }
                else if (member._MemberType == TemporaryMember.MemberType.ExternalTeam || member._MemberType == TemporaryMember.MemberType.ExternalCompanyTeam)
                {
                    AddExternalMemberPhoneNumbers(member.MemberID, member.IncludeChildTeams, dicPhoneNumbers);
                }
                else if (member._MemberType == TemporaryMember.MemberType.UserDefinedTeam)
                {
                    string strPhoneNumber, strTeamName;

                    if (DataManager.Instance.GetUserDefinedTeamInfo(member.MemberID, out strPhoneNumber, out strTeamName))
                    {
                        dicPhoneNumbers[strPhoneNumber] = strTeamName + ";";
                    }
                }
                else if (member._MemberType == TemporaryMember.MemberType.JobLevel)
                {
                    List<DataCompanyMember> companyMembers = DataManager.Instance.GetCompanyMembers(member.MemberID);

                    if (companyMembers == null)
                        continue;

                    foreach (DataCompanyMember companyMember in companyMembers)
                    {
                        dicPhoneNumbers[companyMember.PhoneNumber] = GetCompanyMemberInfo(companyMember);
                    }
                }
            }
        }

        private static string GetExternalMemberInfo(DataExternalMember member)
        {
            DataTeam team = member.Team;

            if (team != null)
                return team.TeamName + ";" + member.Name;

            return ";" + member.Name;
        }

        private static string GetCompanyMemberInfo(DataCompanyMember member, Data_RegularTeam regularTeam = null)
        {
            if (regularTeam != null)
                return regularTeam.TeamName + ";" + member.MemberName;

            DataTeam team = member.GetFirstTeam();

            if (team != null)
                return team.TeamName + ";" + member.MemberName;

            return ";" + member.MemberName;
        }

        // 교대근무자를 감안하여 수신자 리스트를 조정한다.
        public static void CheckControlTeamValidPhoneNumbers(ArrayList phoneNumbers)
        {
            // 산출된 전화번호에서 근무표의 조원과 대조하여 유효한 전화번호만 색출
            ArrayList newPhoneNumbers = ControlTeamEditor.VaildMemberPhoneNumber.IsVaildPhoneNumber(phoneNumbers, ProxySOP.Instance.DBManager);

            if (newPhoneNumbers == phoneNumbers)
                return;
            else
            {
                phoneNumbers.Clear();
                phoneNumbers.AddRange(newPhoneNumbers);
            }
        }

        public static bool OnSendSMSClick(ArrayList phoneNumbers, string strSender, string strMessage, bool needConfirm, out string strErrorMessage)
        {
            strErrorMessage = "";

            if (phoneNumbers == null || phoneNumbers.Count == 0 || strSender.Length == 0 || strMessage.Length == 0)
                return false;

            bool bSendSMS = true;
            if (UnE.SOP.ProxySOP.Instance.ConfirmSendSMS == true && needConfirm)
            {
                if (UnE.SOP.ProxySOP.Instance.ConfirmSMSAll == false)
                {
                    MessageBoxEx msgBox = new MessageBoxEx();
                    msgBox.Text = "문자발송";
                    msgBox.ShowDialog();
                    if (msgBox.DialogResult != System.Windows.Forms.DialogResult.No)
                    {
                        if (msgBox.DialogResult == System.Windows.Forms.DialogResult.Ignore)
                        {
                            UnE.SOP.ProxySOP.Instance.ConfirmSMSAll = true;
                        }
                    }
                    else
                    {
                        bSendSMS = false;
                    }
                }
            }

            if (bSendSMS == true)
            {
                //AfterRunExecute();

                ArrayList arrCallList = phoneNumbers.Clone() as ArrayList;

                bSendSMS = UnE.SOP.SMS.SMSManager.Instance.SendSMS(arrCallList, strSender, strMessage);
                SetSMSDBHistory(strMessage, FormSOP.Instance.DBManager);
                //contents.SendLogState(m_section);

                if (bSendSMS == false)
                    strErrorMessage = "전송실패";
            }

            return bSendSMS;
        }

        private static void SetSMSDBHistory(string strMsg, WebDBManager dbMgr)
        {
            string strSQL = "Select ID from OptionSOPSimulator where PropertyName = 'LastSMSMessage' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            DateTime dtNow = DateTime.Now;
            int nID = 0;
            string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
            string strBody = strTime + "," + strMsg;

            if (arrResult == null || arrResult.Count == 0)
            {
                strSQL = "Select max(ID) from OptionSOPSimulator";
                arrResult = dbMgr.GetResultData(strSQL);

                if (arrResult == null || arrResult.Count == 0)
                    nID = 1;
                else
                {
                    nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1) + 1;

                    if (nID < 0)
                        nID = 1;
                }

                strSQL = "Insert into OptionSOPSimulator (ID, PropertyName, PropertyValue, Description, SiteID) values (";
                strSQL += string.Format("{0}, 'LastSMSMessage', '{1}', '마지막으로 발송된 문자메시지', {2})", nID, strBody, UnE.SOP.ProxySOP.Instance.SiteID);
                dbMgr.GetResultData(strSQL);
            }
            else
            {
                nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

                if (nID > 0)
                {
                    strSQL = string.Format("Update OptionSOPSimulator set PropertyValue = '{0}' where ID = {1}", strBody, nID);
                    dbMgr.GetResultData(strSQL);
                }
            }
        }
    }
}
