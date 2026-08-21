using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace DoorSensorServer
{
    public class TeamReader : IThreadObject
    {
        private class Team
        {
            private int m_nID = -1;
            private string m_strName = "";
            private Team m_teamParent = null;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string Name
            {
                get { return m_strName; }
                set { m_strName = value; }
            }

            public Team ParentTeam
            {
                get { return m_teamParent; }
                set { m_teamParent = value; }
            }
        }

        private class CardData
        {
            private string m_strCardNo = "";
            private string m_strMemberName = "";
            private Team m_team = null;
            private string m_strTime = "";

            public string Time
            {
                get { return m_strTime; }
                set { m_strTime = value; }
            }

            public string CardNo
            {
                get { return m_strCardNo; }
                set { m_strCardNo = value; }
            }

            public string MemberName
            {
                get { return m_strMemberName; }
                set { m_strMemberName = value; }
            }

            public Team Team
            {
                get { return m_team; }
                set { m_team = value; }
            }

            public string TeamPath
            {
                get
                {
                    string strTeamPath = "";

                    Team team = m_team;

                    while (team != null)
                    {
                        if (strTeamPath.Length > 0)
                            strTeamPath = team.Name + "/" + strTeamPath;
                        else
                            strTeamPath = team.Name;

                        team = team.ParentTeam;
                    }

                    return strTeamPath;
                }
            }

            // Key : 최상위 팀으로부터의 경로
            public bool SetCompany(string strCompany, Dictionary<string, Team> dicTeams, WebDBManager dbMgr)
            {
                string strParent = "";
                string[] tokens = strCompany.Split('-');

                Team teamParent = null;
                Team team = null;

                foreach (string strToken in tokens)
                {
                    string strTeamName = strToken.Trim();
                    string strTeamPath = strParent.Length == 0 ? strTeamName : strParent + "/" + strTeamName;

                    if (dicTeams.TryGetValue(strTeamPath, out team) == false)
                        team = ReadTeam(strTeamName, teamParent, dbMgr);

                    if (team != null)
                        teamParent = team;
                    else
                        break;
                }

                m_team = team;
                return m_team != null;
            }

            private Team ReadTeam(string strTeamName, Team teamParent, WebDBManager dbMgr)
            {
                string strSQL = "Select ID from RegularTeam where TeamName = '" + strTeamName + "'";

                if (teamParent != null)
                    strSQL += " and ParentTeamID = " + teamParent.ID.ToString();
                else
                    strSQL += " and ParentTeamID is NULL";

                ArrayList arrResult = dbMgr.GetResultData(strSQL);

                if (arrResult == null)
                    return null;

                if (arrResult.Count > 0)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                    if (id == null)
                        return null;

                    Team team = new Team();

                    team.ID = id.Data;
                    team.Name = strTeamName;
                    team.ParentTeam = teamParent;

                    return team;
                }

                return InsertTeam(strTeamName, teamParent, dbMgr);
            }

            private Team InsertTeam(string strTeamName, Team teamParent, WebDBManager dbMgr)
            {
                string strSQL = "Insert into RegularTeam (ID, TeamName, ParentTeamID) values ((Select ISNULL(max(ID), 0) + 1 from RegularTeam) + 1, '" + strTeamName + "', ";
                string strSQL2 = "Select ID from RegularTeam where TeamName = '" + strTeamName + "' and ";

                if (teamParent == null)
                {
                    strSQL += "NULL)";
                    strSQL2 += "ParentTeamID is NULL";
                }
                else
                {
                    strSQL += teamParent.ID.ToString() + ")";
                    strSQL2 += "ParentTeamID = " + teamParent.ID.ToString();
                }

                if (dbMgr.GetResultData(strSQL) == null)
                    return null;

                ArrayList arrResult = dbMgr.GetResultData(strSQL2);

                if (arrResult == null || arrResult.Count == 0)
                    return null;

                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                if (id == null)
                    return null;

                Team team = new Team();

                team.ID = id.Data;
                team.Name = strTeamName;
                team.ParentTeam = teamParent;

                return team;
            }
        }

        private DateTime m_dtPrev = new DateTime();
        private WebDBManager m_dbMgr = null;
        private DirectDBManager m_dbInsa = null;

        // Key : Table 이름
        // Value : 마지막으로 DB에서 읽은 시간
        private Dictionary<string, string> m_dicLastReadDateTime = new Dictionary<string, string>();

        public TeamReader()
        {
            Init();
        }

        private bool Init()
        {
            int nSiteID, nDBType;

            if (ReadConfig("siteid", out nSiteID) == false)
                return false;

            if (ReadConfig("dbtype", out nDBType) == false)
                return false;

            string strDBName = System.Configuration.ConfigurationManager.AppSettings["dbname"].ToString().Trim();

            if (strDBName.Length == 0)
                return false;

            string strWebServerURL = System.Configuration.ConfigurationManager.AppSettings["webserver"].ToString().Trim();

            if (strWebServerURL.Length == 0)
                return false;

            m_dbMgr = new WebDBManager(strDBName, nSiteID);
            m_dbMgr.WebServerURL = strWebServerURL;
            m_dbMgr.DatabaseType = (WebDBManager.DBType)nDBType;

            string strInsaIP = System.Configuration.ConfigurationManager.AppSettings["insa_ip"].ToString().Trim();

            if (strInsaIP.Length == 0)
                return false;

            string strInsaDB = System.Configuration.ConfigurationManager.AppSettings["insa_db"].ToString().Trim();

            if (strInsaDB.Length == 0)
                return false;

            if (ReadConfig("insa_type", out nDBType) == false)
                return false;

            string strInsaUID = System.Configuration.ConfigurationManager.AppSettings["insa_uid"].ToString().Trim();

            if (strInsaUID.Length == 0)
                return false;

            string[] tokens = null;
            int nIndex1 = strInsaUID.IndexOf(' ');
            int nIndex2 = strInsaUID.IndexOf('\t');

            if (nIndex1 > 0)
                tokens = strInsaUID.Split(' ');
            else if (nIndex2 > 0)
                tokens = strInsaUID.Split('\t');

            if (tokens == null || tokens.Count() < 2)
                return false;

            string strID = tokens[0].Trim();
            string strPW = tokens[1].Trim();
            m_dbInsa = DirectDBManager.MakeInstance((DirectDBManager.DBType)nDBType, strInsaIP, strID, strPW, strInsaDB);

            return true;
        }

        private bool ReadConfig(string strName, out int value)
        {
            string strValue = System.Configuration.ConfigurationManager.AppSettings[strName].ToString().Trim();
            return int.TryParse(strValue, out value);
        }

        public void Run()
        {
            if (m_dbMgr == null || m_dbInsa == null)
                return;

            DateTime dtNow = DateTime.Now;

            if (dtNow.Year != m_dtPrev.Year || dtNow.Month != m_dtPrev.Month || dtNow.Day != m_dtPrev.Day)
            {
                m_dtPrev = dtNow;

                // Key : 최상위 팀으로부터의 경로
                Dictionary<string, Team> dicTeams = new Dictionary<string, Team>();
                // Key : 카드 번호
                Dictionary<string, CardData> dicCardDatas = new Dictionary<string, CardData>();

                ReadDB("EXPORTEVENT_H", dicTeams, dicCardDatas);
                ReadDB("EXPORTEVENT_R", dicTeams, dicCardDatas);
                ReadDB("EXPORTEVENT_T1", dicTeams, dicCardDatas);
                ReadDB("EXPORTEVENT_T2", dicTeams, dicCardDatas);

                ReadCompanyMembers(dicCardDatas.Values.ToList());
            }
        }

        private void ReadDB(string strTableName, Dictionary<string, Team> dicTeams, Dictionary<string, CardData> dicCardDatas)
        {
            string strLastDateTime = "";

            if (m_dicLastReadDateTime.TryGetValue(strTableName, out strLastDateTime) == false)
                strLastDateTime = "0";

            string strSQL = string.Format("Select ATime, Cardno, Username, Company from {0} where len(Cardno) > 0 and len(Username) > 0 and len(Company) > 0 and ATime > '{1}' ", strTableName, strLastDateTime);
            strSQL += string.Format("and Concat(cardno, atime) in (Select Concat(Cardno, Max(ATime)) from {0} group by Cardno) order by ATime", strTableName);

            DirectDBManager dbMgr = m_dbInsa.Clone();

            if (dbMgr.Connect() == false)
                return;

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                dbMgr.Close();
                return;
            }

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-3;i+=4)
            {
                string strTime = WebDBManager.GetStringField(arrResult[i]);
                string strCardNo = WebDBManager.GetStringField(arrResult[i + 1]);
                string strUserName = WebDBManager.GetStringField(arrResult[i + 2]);
                string strCompany = WebDBManager.GetStringField(arrResult[i + 3]);

                if (strTime == null || strCardNo == null || strUserName == null || strCompany == null)
                    continue;

                strLastDateTime = strTime;

                CardData data = new CardData();
                data.CardNo = strCardNo;
                data.MemberName = strUserName;
                data.Time = strTime;

                if (data.SetCompany(strCompany, dicTeams, m_dbMgr) == false)
                    continue;

                dicTeams[data.TeamPath] = data.Team;

                CardData oldData;

                if (dicCardDatas.TryGetValue(strCardNo, out oldData))
                {
                    if (string.Compare(oldData.Time, data.Time) < 0)
                        dicCardDatas[strCardNo] = data;
                }
                else
                    dicCardDatas[strCardNo] = data;
            }

            m_dicLastReadDateTime[strTableName] = strLastDateTime;
        }

        private void ReadCompanyMembers(List<CardData> cardDatas)
        {
            int nCount = 0;
            // Key : Card 번호
            Dictionary<string, CardData> datas = new Dictionary<string, CardData>();

            for (int i=cardDatas.Count-1;i>=0;i--)
            {
                CardData data = cardDatas[i];
                datas[data.CardNo] = data;

                // 카드 데이터가 너무 많을 경우 한꺼번에 쿼리할 경우 쿼리가 너무 커질 우려가 있어
                // 최대 100개 단위로만 쿼리한다.
                if (++nCount >= 100)
                {
                    ReadCompanyMembers(datas);

                    nCount = 0;
                    datas.Clear();
                }
            }

            if (nCount > 0)
                ReadCompanyMembers(datas);
        }

        // Key : Card 번호
        private void ReadCompanyMembers(Dictionary<string, CardData> datas)
        {
            string strCardNos = "";

            foreach (KeyValuePair<string, CardData> pair in datas)
            {
                if (strCardNos.Length == 0)
                    strCardNos = "'" + pair.Key + "'";
                else
                    strCardNos += ", '" + pair.Key + "'";
            }

            if (strCardNos.Length == 0)
                return;

            string strSQL = "Select team.ID, member.ID, member.MemberName, member.MemberID ";
            strSQL += "from CompanyMember as member, RegularTeam as team, RegularMemberList as rml ";
            strSQL += "where member.ID = rml.CompanyMemberID and team.ID = rml.RegularTeamID and member.MemberID in (" + strCardNos + ")";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            CardData cardData;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-3;i+=4)
            {
                VariousData<int> teamID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> memberID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 2]);
                string strCardNo = WebDBManager.GetStringField(arrResult[i + 3]);

                if (teamID == null || memberID == null || strMemberName == null || strCardNo == null)
                    continue;

                if (datas.TryGetValue(strCardNo, out cardData))
                {
                    datas.Remove(strCardNo);

                    if (cardData.MemberName == strMemberName && cardData.Team.ID == teamID.Data)
                    {
                        // 변경사항 없음
                        continue;
                    }
                    else
                    {
                        if (cardData.MemberName != strMemberName)
                            UpdateMemberName(memberID.Data, cardData.MemberName);

                        if (cardData.Team.ID != teamID.Data)
                            UpdateMemberTeam(memberID.Data, cardData.Team.ID, teamID.Data);
                    }
                }
            }

            foreach (KeyValuePair<string, CardData> pair in datas)
            {
                InsertMember(pair.Value.MemberName, pair.Value.CardNo, pair.Value.Team.ID);
            }
        }

        private void InsertMember(string strMemberName, string strCardNo, int nTeamID)
        {
            string strSQL = "Select ID, MemberName from CompanyMember where MemberID = '" + strCardNo + "'";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nMemberID = -1;

            if (arrResult.Count >= 2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
                string strName = WebDBManager.GetStringField(arrResult[1]);

                if (id == null || strName == null)
                    return;

                nMemberID = id.Data;
                
                if (strMemberName != strName)
                {
                    strSQL = "Update CompanyMember set MemberName = '" + strMemberName + "' where ID = " + id.Data.ToString();

                    if (m_dbMgr.GetResultData(strSQL) == null)
                        return;
                }
            }

            if (nMemberID < 0)
            {
                strSQL = "Insert into CompanyMember (ID, MemberName, LevelID, MemberID, OfficePhoneNumber, PhoneNumber, SubLevelID) values ((Select ISNULL(max(ID), 0) + 1 from CompanyMember) + 1, '" + strMemberName + "', ";
                strSQL += "0, '" + strCardNo + "', NULL, NULL, NULL)";

                if (m_dbMgr.GetResultData(strSQL) == null)
                    return;

                strSQL = "Select ID from CompanyMember where MemberID = '" + strCardNo + "'";

                arrResult = m_dbMgr.GetResultData(strSQL);

                if (arrResult == null || arrResult.Count == 0)
                    return;

                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                if (id == null)
                    return;

                nMemberID = id.Data;
            }

            strSQL = "Insert into RegularMemberList (RegularTeamID, CompanyMemberID, PositionID, SubPositionID, GroupPositionID) values (";
            strSQL += string.Format("{0}, {1}, 0, NULL, NULL)", nTeamID, nMemberID);
            m_dbMgr.GetResultData(strSQL);
        }

        private void UpdateMemberTeam(int nID, int nTeamID, int nOldTeamID)
        {
            string strSQL = string.Format("Update RegularMemberList set RegularTeamID = {0} where RegularTeamID = {1} and CompanyMemberID = {2}", nTeamID, nOldTeamID, nID);
            m_dbMgr.GetResultData(strSQL);
        }

        private void UpdateMemberName(int nID, string strMemberName)
        {
            string strSQL = "Update CompanyMember set MemberName = '" + strMemberName + "' where ID = " + nID.ToString();
            m_dbMgr.GetResultData(strSQL);
        }
    }
}
