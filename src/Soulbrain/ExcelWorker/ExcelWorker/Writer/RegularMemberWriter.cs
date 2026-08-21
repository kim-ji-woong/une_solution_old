using System.Collections.Generic;
using TeamEditor.DAL;
using TeamEditor.Model.Sop.Team;
using dnsDBUtil;

namespace ExcelWorker.Writer
{
    public class RegularMemberWriter : ExcelWriter
    {
        private DataManager m_teamDataManager = null;

        public RegularMemberWriter(string strFilePath)
            : base(strFilePath)
        {
            WebDBManager dbMgr = (WebDBManager)m_dataManager.GetDBManager();
            m_teamDataManager = new DataManager(dbMgr.DatabaseName, (int)dbMgr.DatabaseType, dbMgr.SiteID, dbMgr.WebServerURL);
        }

        protected override string GetSubject()
        {
            return "조직정보";
        }

        protected override ICollection<SheetData> ReadSheetDatas(out string strErrorMessage)
        {
            if (m_dataManager == null)
            {
                strErrorMessage = "DB에 연결할 수 없습니다.";
                return null;
            }

            Dictionary<int, string> dicJobLevels = new Dictionary<int, string>();
            Dictionary<string, List<RegularMember>> dicTeamMembers = GetTeamRegularMembers(dicJobLevels, out strErrorMessage);

            if (dicTeamMembers == null)
                return null;

            SheetData sheetData = new SheetData(GetSubject());

            sheetData.Titles[0] = "부서(필수)";
            sheetData.Titles[1] = "이름(필수)";
            sheetData.Titles[2] = "사번(선택)";
            sheetData.Titles[3] = "휴대폰(필수)";
            sheetData.Titles[4] = "직급(선택)";
            sheetData.Titles[5] = "이메일(필수)";

            List<string> teams = new List<string>();
            List<string> names = new List<string>();
            List<string> memberIDs = new List<string>();
            List<string> phoneNumbers = new List<string>();
            List<string> jobLevels = new List<string>();
            List<string> emails = new List<string>();

            sheetData.ColumnDatas[0] = teams;
            sheetData.ColumnDatas[1] = names;
            sheetData.ColumnDatas[2] = memberIDs;
            sheetData.ColumnDatas[3] = phoneNumbers;
            sheetData.ColumnDatas[4] = jobLevels;
            sheetData.ColumnDatas[5] = emails;

            string strJobLevel;

            foreach (KeyValuePair<string, List<RegularMember>> pair in dicTeamMembers)
            {
                foreach (RegularMember member in pair.Value)
                {
                    teams.Add(pair.Key);
                    names.Add(member.MemberName);
                    memberIDs.Add(member.MemberID != null ? member.MemberID : "");
                    phoneNumbers.Add(member.PhoneNumber != null ? member.PhoneNumber : "");
                    emails.Add(member.Email != null ? member.Email : "");

                    if (member.JobLevelID != null && dicJobLevels.TryGetValue((int)member.JobLevelID, out strJobLevel))
                        jobLevels.Add(strJobLevel);
                    else
                        jobLevels.Add("");
                }
            }

            List<SheetData> sheets = new List<SheetData>();
            sheets.Add(sheetData);
            return sheets;
        }

        // Key : Team Path
        private Dictionary<string, List<RegularMember>> GetTeamRegularMembers(Dictionary<int, string> dicJobLevels, out string strErrorMessage)
        {
            List<Regular> teams = m_teamDataManager.GetSelectManager().SelectRegulars(out strErrorMessage);

            if (teams == null)
                return null;

            List<RegularMember> members = m_teamDataManager.GetSelectManager().SelectRegularMembers(out strErrorMessage);

            if (members == null)
                return null;

            // Key : Team ID
            // Value : Team Path
            Dictionary<int, string> dicTeamPath = new Dictionary<int, string>();
            Dictionary<int, Regular> dicTeams = new Dictionary<int, Regular>();

            foreach (Regular team in teams)
            {
                dicTeams[team.ID] = team;
            }

            foreach (Regular team in teams)
            {
                string strPath = Reader.RegularMemberReader.GetTeamPath(team, dicTeams);
                dicTeamPath[team.ID] = strPath;
            }

            string strTeamPath;
            Dictionary<string, List<RegularMember>> dicTeamMembers = new Dictionary<string, List<RegularMember>>();

            foreach (RegularMember member in members)
            {
                member.PhoneNumber = Reader.RegularMemberReader.DecryptPhoneNumber(member.PhoneNumber);

                if (dicTeamPath.TryGetValue(member.RegularID, out strTeamPath))
                {
                    List<RegularMember> teamMembers;

                    if (dicTeamMembers.TryGetValue(strTeamPath, out teamMembers) == false)
                    {
                        teamMembers = new List<RegularMember>();
                        dicTeamMembers[strTeamPath] = teamMembers;
                    }

                    if (member.JobLevelID != null)
                        dicJobLevels[(int)member.JobLevelID] = "";

                    teamMembers.Add(member);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine("Unknown Team ID : " + member.RegularID.ToString());
                }
            }

            string strJobLevelIDs = "";

            foreach (KeyValuePair<int, string> pair in dicJobLevels)
            {
                if (strJobLevelIDs.Length == 0)
                    strJobLevelIDs = pair.Key.ToString();
                else
                    strJobLevelIDs += "," + pair.Key.ToString();
            }

            if (strJobLevelIDs.Length > 0)
            {
                string strCondition = string.Format("PropertyName = '{0}' and PropertyID in ({1})", Reader.RegularMemberReader.JobLevelProperty, strJobLevelIDs);
                List<Options> options = m_teamDataManager.GetSelectManager().SelectOptions(strCondition, out strErrorMessage);

                if (options == null)
                    return null;

                foreach (Options option in options)
                {
                    dicJobLevels[option.PropertyID] = option.PropertyValue;
                }
            }

            return dicTeamMembers;
        }
    }
}
