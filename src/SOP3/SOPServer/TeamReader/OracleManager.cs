using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.OleDb;
using System.IO;

namespace TeamReader
{
    public class OracleManager
    {
        private OleDbConnection m_Connection = null;
        private bool m_isConnection = false;
        private string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        // 개발모드일 경우 실제 Oracle DB가 아닌 Excel 파일에서 데이터를 얻어온다.
        private bool m_devMode = false;
        // csv 파일에서 데이터를 구분할 구분자
        private char m_chDelimeter = ',';

        private static char[] m_arrTrim = new char[] { ' ', '\t', '\r', '\n' };

        public OracleManager(string strID, string strPW, string strDataSource)
        {
            if (!m_devMode)
            {
                m_Connection = new OleDbConnection();

                m_Connection.ConnectionString = string.Format("Provider=OraOLEDB.Oracle;USER ID={0};PASSWORD={1};DATA SOURCE={2};OLEDB.NET=True;",
                                                                 strID,
                                                                 strPW,
                                                                 strDataSource);
            }
        }

        public bool OpenConnection()
        {
            if (m_devMode)
                return true;

            if (m_isConnection)
                return true;

			if (m_Connection != null && m_Connection.State == System.Data.ConnectionState.Open)
				return true;

            try
            {
                m_Connection.Open();
                m_isConnection = true;
                return true;
            }
            catch (Exception)
            {
				m_isConnection = false;
                //System.Windows.Forms.MessageBox.Show(e.Message);
            }

            return false;
        }

        public void CloseConnection()
        {
            if (m_devMode)
                return;

            if (!m_isConnection)
                return;

            m_Connection.Close();
            m_isConnection = false;
        }      

        private void TrimString(ref string str)
        {
            str = str.TrimStart(m_arrTrim);
            str = str.TrimEnd(m_arrTrim);
        }

        private bool GetTeamInfoFromCSV(string strLine, out string strTeamCode, out string strParentTeamCode, out string strTeamName, out string strTeamOrder, out string strTeamLeaderCode)
        {
            strTeamCode = strParentTeamCode = strTeamName = strTeamOrder = strTeamLeaderCode = "";

            int nIndex1 = strLine.IndexOf(m_chDelimeter, 0);
            if (nIndex1 < 0)
                return false;

            int nIndex2 = strLine.IndexOf(m_chDelimeter, nIndex1 + 1);
            if (nIndex2 < 0)
                return false;

            int nIndex3 = strLine.IndexOf(m_chDelimeter, nIndex2 + 1);
            if (nIndex3 < 0)
                return false;

            int nIndex4 = strLine.IndexOf(m_chDelimeter, nIndex3 + 1);
            if (nIndex4 < 0)
                return false;

            strTeamCode = strLine.Substring(0, nIndex1);
            strParentTeamCode = strLine.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
            strTeamName = strLine.Substring(nIndex2 + 1, nIndex3 - nIndex2 - 1);
            strTeamOrder = strLine.Substring(nIndex3 + 1, nIndex4 - nIndex3 - 1);
            strTeamLeaderCode = strLine.Substring(nIndex4 + 1);

            TrimString(ref strTeamCode);
            TrimString(ref strParentTeamCode);
            TrimString(ref strTeamName);
            TrimString(ref strTeamOrder);
            TrimString(ref strTeamLeaderCode);

            return true;
        }

        private bool LoadTeamListFromCSV(Dictionary<string, RegularTeam> dicRegularTeam, Dictionary<RegularTeam, string> dicTeamLeader)
        {
            StreamReader reader = new StreamReader("삼천포 인사DB관련_orcl db 자료_부서.csv", Encoding.UTF8);

            if (reader.EndOfStream)
            {
                reader.Close();
                return false;
            }

            // Title
            reader.ReadLine();

            string strTeamCode, strParentTeamCode, strTeamName, strTeamOrder, strTeamLeaderCode;

            // 자신의 Team, 부모 조직의 Team Code
            Dictionary<RegularTeam, string> dicTeamCode = new Dictionary<RegularTeam, string>();
            Dictionary<string, Tree<RegularTeam>.Node> dicTeamNode = new Dictionary<string, Tree<RegularTeam>.Node>();

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();
                if (!GetTeamInfoFromCSV(strLine, out strTeamCode, out strParentTeamCode, out strTeamName, out strTeamOrder, out strTeamLeaderCode))
                    break;

                RegularTeam team = new RegularTeam();

                team.TeamCode = strTeamCode;
                dicTeamCode[team] = strParentTeamCode;
                team.TeamName = strTeamName;

                // 부서직제 무시
                /*try
                {    
                    team.TeamID = (int)Int32.Parse(strTeamOrder);
                }
                catch (Exception e)
                {
                    team.TeamID = -1;
                }*/

                try
                {
                    dicTeamLeader[team] = strTeamLeaderCode;
                }
                catch (Exception)
                {
                    dicTeamLeader[team] = "";
                }

                dicRegularTeam[team.TeamCode] = team;
                dicTeamNode[team.TeamCode] = new Tree<RegularTeam>.Node(team);
            }

            reader.Close();

            // 삼천포 code 8580인 Node
            Tree<RegularTeam>.Node rootNode = null;

            foreach (KeyValuePair<string, RegularTeam> pair in dicRegularTeam)
            {
                RegularTeam team = pair.Value;

                if (dicTeamNode.ContainsKey(team.TeamCode))
                {
                    Tree<RegularTeam>.Node node = dicTeamNode[team.TeamCode];

                    if (node.Data.TeamCode == "8580")
                        rootNode = node;

                    if (dicTeamCode.ContainsKey(team))
                    {
                        strParentTeamCode = dicTeamCode[team];

                        if (dicTeamNode.ContainsKey(strParentTeamCode))
                        {
                            Tree<RegularTeam>.Node nodeParent = dicTeamNode[strParentTeamCode];
                            if (!nodeParent.Contains(node))
                                nodeParent.Add(node);
                        }
                    }
                }
            }

            dicRegularTeam.Clear();

            if (rootNode != null)
                AddTeam(rootNode, dicRegularTeam);

            return true;
        }

        // dicTeamLeader : Team, 부서장 사번
        public bool LoadTeamList(Dictionary<string, RegularTeam> dicRegularTeam, Dictionary<RegularTeam, string> dicTeamLeader)
        {
            if (m_devMode)
                return LoadTeamListFromCSV(dicRegularTeam, dicTeamLeader);

            OleDbCommand cmd = new OleDbCommand("SELECT DEPT_CODE, UP_CODE, DEPT_NAME, DEPT_ORDER, MANAGER from webadm.view_jojik_tbl", m_Connection);
            OleDbDataReader reader = null;

            // 자신의 Team, 부모 조직의 Team Code
            Dictionary<RegularTeam, string> dicTeamCode = new Dictionary<RegularTeam, string>();
            Dictionary<string, Tree<RegularTeam>.Node> dicTeamNode = new Dictionary<string, Tree<RegularTeam>.Node>();

            try
            {
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    RegularTeam team = new RegularTeam();

                    team.TeamCode = (string)reader.GetValue(0);
                    dicTeamCode[team] = (string)reader.GetValue(1);
                    team.TeamName = (string)reader.GetValue(2);

                    // 부서직제 무시
                    /*try
                    {    
                        team.TeamID = (int)Int32.Parse(reader.GetValue(3).ToString());
                    }
                    catch (Exception e)
                    {
                        team.TeamID = -1;
                    }*/

                    try
                    {
                        dicTeamLeader[team] = (string)reader.GetValue(4);
                    }
                    catch (Exception)
                    {
                        dicTeamLeader[team] = "";
                    }

                    dicRegularTeam[team.TeamCode] = team;
                    dicTeamNode[team.TeamCode] = new Tree<RegularTeam>.Node(team);
                }

                reader.Close();
            }
            catch (System.Exception)
            {
                if (reader != null)
                    reader.Close();

                //System.Windows.Forms.MessageBox.Show(ex.Message);
                return false;
            }

            // 부모팀 지정
            /*foreach (KeyValuePair<string, RegularTeam> pair in dicRegularTeam)
            {
                RegularTeam team = pair.Value;

                if (dicTeamCode.ContainsKey(team))
                {
                    string strParentTeamCode = dicTeamCode[team];

                    if (dicRegularTeam.ContainsKey(strParentTeamCode))
                        team.ParentTeam = dicRegularTeam[strParentTeamCode];
                }
            }*/

            // 삼천포 code 8580인 Node
            Tree<RegularTeam>.Node rootNode = null;

            foreach (KeyValuePair<string, RegularTeam> pair in dicRegularTeam)
            {
                RegularTeam team = pair.Value;

                if (dicTeamNode.ContainsKey(team.TeamCode))
                {
                    Tree<RegularTeam>.Node node = dicTeamNode[team.TeamCode];

                    if (node.Data.TeamCode == "8580")
                        rootNode = node;

                    if (dicTeamCode.ContainsKey(team))
                    {
                        string strParentTeamCode = dicTeamCode[team];

                        if (dicTeamNode.ContainsKey(strParentTeamCode))
                        {
                            Tree<RegularTeam>.Node nodeParent = dicTeamNode[strParentTeamCode];
                            if (!nodeParent.Contains(node))
                                nodeParent.Add(node);
                        }
                    }
                }
            }

            dicRegularTeam.Clear();

            if (rootNode != null)
                AddTeam(rootNode, dicRegularTeam);

            return true;
        }

        private void AddTeam(Tree<RegularTeam>.Node node, Dictionary<string, RegularTeam> dicRegularTeam)
        {
            dicRegularTeam[node.Data.TeamCode] = node.Data;

            foreach (Tree<RegularTeam>.Node child in node.Children)
            {
                child.Data.ParentTeam = node.Data;
                AddTeam(child, dicRegularTeam);
            }
        }

        /*private RegularTeam FindTeam(string strTeamCode, ArrayList arrTeams)
        {
            foreach (RegularTeam team in arrTeams)
            {
                if (team.TeamCode == strTeamCode)
                    return team;
            }

            return null;
        }*/

        private bool GetMemberInfoFromCSV(string strLine, out string strMemberCode, out string strTeamCode, out string strLevelNo, out string strEMail, out string strMemberName, out string strOfficePhoneNumber, out string strMobilePhoneNumber, out string strPosition, out bool isTeamLeader)
        {
            strMemberCode = strTeamCode = strLevelNo = strEMail = strMemberName = strOfficePhoneNumber = strMobilePhoneNumber = strPosition = "";
            isTeamLeader = false;

            int nIndex1 = strLine.IndexOf(m_chDelimeter, 0);
            if (nIndex1 < 0)
                return false;

            int nIndex2 = strLine.IndexOf(m_chDelimeter, nIndex1 + 1);
            if (nIndex2 < 0)
                return false;

            int nIndex3 = strLine.IndexOf(m_chDelimeter, nIndex2 + 1);
            if (nIndex3 < 0)
                return false;

            int nIndex4 = strLine.IndexOf(m_chDelimeter, nIndex3 + 1);
            if (nIndex4 < 0)
                return false;

            int nIndex5 = strLine.IndexOf(m_chDelimeter, nIndex4 + 1);
            if (nIndex5 < 0)
                return false;

            int nIndex6 = strLine.IndexOf(m_chDelimeter, nIndex5 + 1);
            if (nIndex6 < 0)
                return false;

            int nIndex7 = strLine.IndexOf(m_chDelimeter, nIndex6 + 1);
            if (nIndex7 < 0)
                return false;

            int nIndex8 = strLine.IndexOf(m_chDelimeter, nIndex7 + 1);
            if (nIndex8 < 0)
                return false;

            strMemberCode = strLine.Substring(0, nIndex1);
            strTeamCode = strLine.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
            strLevelNo = strLine.Substring(nIndex2 + 1, nIndex3 - nIndex2 - 1);
            strEMail = strLine.Substring(nIndex3 + 1, nIndex4 - nIndex3 - 1);
            strMemberName = strLine.Substring(nIndex4 + 1, nIndex5 - nIndex4 - 1);
            strOfficePhoneNumber = strLine.Substring(nIndex5 + 1, nIndex6 - nIndex5 - 1);
            strMobilePhoneNumber = strLine.Substring(nIndex6 + 1, nIndex7 - nIndex6 - 1);
            strPosition = strLine.Substring(nIndex7 + 1, nIndex8 - nIndex7 - 1);
            string strTeamLeader = strLine.Substring(nIndex8 + 1);

            TrimString(ref strMemberCode);
            TrimString(ref strTeamCode);
            TrimString(ref strLevelNo);
            TrimString(ref strEMail);
            TrimString(ref strMemberName);
            TrimString(ref strOfficePhoneNumber);
            TrimString(ref strMobilePhoneNumber);
            TrimString(ref strPosition);
            TrimString(ref strTeamLeader);

            int nCodeLen = strMemberCode.Length;
            for (int i = 0; i < 8 - nCodeLen; i++)
                strMemberCode = "0" + strMemberCode;

            isTeamLeader = strTeamLeader == "Y";

            return true;
        }

        private bool LoadCompanyMemberListFromCSV(/*Out*/Dictionary<string, CompanyMember> dicCompanyMember, /*In*/Dictionary<string, RegularTeam> dicRegularTeam, Dictionary<RegularTeam, string> dicTeamLeader)
        {
            StreamReader reader = new StreamReader("삼천포 인사DB관련_orcl db 자료_인사.csv", Encoding.UTF8);

            if (reader.EndOfStream)
            {
                reader.Close();
                return false;
            }

            // Title
            reader.ReadLine();

            string strMemberCode, strTeamCode, strLevelNo, strEMail, strMemberName, strOfficePhoneNumber, strMobilePhoneNumber, strPosition;
            bool isTeamLeader;

            // 자신의 Team, 부모 조직의 Team Code
            Dictionary<RegularTeam, string> dicTeamCode = new Dictionary<RegularTeam, string>();
            Dictionary<string, Tree<RegularTeam>.Node> dicTeamNode = new Dictionary<string, Tree<RegularTeam>.Node>();

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();
                if (!GetMemberInfoFromCSV(strLine, out strMemberCode, out strTeamCode, out strLevelNo, out strEMail, out strMemberName, out strOfficePhoneNumber, out strMobilePhoneNumber, out strPosition, out isTeamLeader))
                    break;

                CompanyMember member = new CompanyMember();

                if (strTeamCode == "")
                    continue;

                if (dicRegularTeam.ContainsKey(strTeamCode))
                    member.Team = dicRegularTeam[strTeamCode];
                else
                    continue;

                try
                {
                    member.LevelID = int.Parse(strLevelNo);
                }
                catch (Exception)
                {
                    member.LevelID = 0;
                }

                // mail 주소 무시
                /*try
                {
                    member.MailAddress = strEMail;
                }
                catch (Exception e)
                {
                    member.MailAddress = "";
                }*/

                member.MemberName = strMemberName;
                member.OfficePhoneNumber = strOfficePhoneNumber;
                member.PhoneNumber = AES256Cipher.AES_encrypt(strMobilePhoneNumber, key);
                member.Title = strPosition;
                member.PositionID = isTeamLeader ? 2 : 1;
                member.MemberID = strMemberCode;
                
                dicCompanyMember[member.MemberID] = member;
            }

            reader.Close();

            // 부서장 설정
            foreach (KeyValuePair<RegularTeam, string> pair in dicTeamLeader)
            {
                RegularTeam team = pair.Key;

                if (dicCompanyMember.ContainsKey(pair.Value))
                    team.TeamLeader = dicCompanyMember[pair.Value];
            }

            return true;
        }

        public bool LoadCompanyMemberList(/*Out*/Dictionary<string, CompanyMember> dicCompanyMember, /*In*/Dictionary<string, RegularTeam> dicRegularTeam, Dictionary<RegularTeam, string> dicTeamLeader)
        {
            if (m_devMode)
                return LoadCompanyMemberListFromCSV(dicCompanyMember, dicRegularTeam, dicTeamLeader);

            OleDbCommand cmd = new OleDbCommand("SELECT DEPTNO, LEVELNO, MAILNO, NAME, TELNO, MOBILE_PHN, TITLE, JANG_YN, EMPNO from webadm.view_insa_tbl_sec", m_Connection);
            OleDbDataReader reader = null;

            try
            {
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string strTeamCode = "";
                    CompanyMember member = new CompanyMember();

                    try
                    {
                        strTeamCode = (string)reader.GetValue(0);
                    }
                    catch (Exception)
                    {
                        strTeamCode = "";
                    }
                    if (strTeamCode == "")
                        continue;

                    if (dicRegularTeam.ContainsKey(strTeamCode))
                        member.Team = dicRegularTeam[strTeamCode];
                    else
                        continue;

                    try
                    {
                        member.LevelID = int.Parse(reader.GetValue(1).ToString());
                    }
                    catch (Exception)
                    {
                        member.LevelID = 0;
                    }

                    // mail 주소 무시
                    /*try
                    {
                        member.MailAddress = (string)reader.GetValue(2);
                    }
                    catch (Exception e)
                    {
                        member.MailAddress = "";
                    }*/

                    try
                    {
                        member.MemberName = (string)reader.GetValue(3);
                    }
                    catch (Exception)
                    {
                        member.MemberName = "";
                    }

                    try
                    {
                        member.OfficePhoneNumber = (string)reader.GetValue(4);
                    }
                    catch (Exception)
                    {
                        member.OfficePhoneNumber = "";
                    }

                    try
                    {
                        //member.PhoneNumber = (string)reader.GetValue(5);
                        // aes 암호화
                        member.PhoneNumber = AES256Cipher.AES_encrypt((string)reader.GetValue(5), key);
                    }
                    catch (Exception)
                    {
                        member.PhoneNumber = "";
                    }

                    try
                    {
                        member.Title = (string)reader.GetValue(6);
                    }
                    catch (Exception)
                    {
                        member.Title = "";
                    }

                    string strYN = "";// (string)reader.GetValue(7);
                    try
                    {
                        strYN = (string)reader.GetValue(7);
                    }
                    catch (Exception)
                    {
                        strYN = "";
                    }

                    member.PositionID = strYN == "Y" || strYN == "y" ? 2 : 1;
                    
                    try
                    {
                        member.MemberID =reader.GetValue(8).ToString();
                    }
                    catch (Exception)
                    {
                        member.MemberID = "";
                    }

                    dicCompanyMember[member.MemberID] = member;
                }

                reader.Close();
            }
            catch (System.Exception)
            {
                if (reader != null)
                    reader.Close();

                //System.Windows.Forms.MessageBox.Show(ex.Message);
                return false;
            }

            // 부서장 설정
            foreach (KeyValuePair<RegularTeam, string> pair in dicTeamLeader)
            {
                RegularTeam team = pair.Key;

                if (dicCompanyMember.ContainsKey(pair.Value))
                    team.TeamLeader = dicCompanyMember[pair.Value];
            }

            return true;
        }
    }
}
