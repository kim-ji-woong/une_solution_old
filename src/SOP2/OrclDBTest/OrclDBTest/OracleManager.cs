using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.OleDb;

namespace OrclDBTest
{
    public class OracleManager
    {
        private OleDbConnection m_Connection = null;
        private bool m_isConnection = false;

        public OracleManager(string strID, string strPW, string strDataSource)
        {
            m_Connection = new OleDbConnection();

            m_Connection.ConnectionString = string.Format("Provider=OraOLEDB.Oracle;USER ID={0};PASSWORD={1};DATA SOURCE={2};OLEDB.NET=True;",
                                                             strID,
                                                             strPW,
                                                             strDataSource);
        }

        public bool OpenConnection()
        {
            if (m_isConnection)
                return true;

            try
            {
                m_Connection.Open();
                m_isConnection = true;
                return true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }

            return false;
        }

        public void CloseConnection()
        {
            if (!m_isConnection)
                return;

            m_Connection.Close();
            m_isConnection = false;
        }

        public bool LoadTeamList(ArrayList arrTeams)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT DEPT_CODE, UP_CODE, DEPT_NAME, DEPT_ORDER, MANAGER from webadm.view_jojik_tbl", m_Connection);
            OleDbDataReader reader = null;

            try
            {
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    RegularTeam team = new RegularTeam();

                    team.TeamCode = (string)reader.GetValue(0);
                    team.ParentTeamCode = (string)reader.GetValue(1);
                    team.TeamName = (string)reader.GetValue(2);

                    try
                    {
                        Object obejct = reader.GetValue(3);                       
                        
                        team.TeamID = (int)Int32.Parse(reader.GetValue(3).ToString());
                    }
                    catch (Exception e)
                    {
                        team.TeamID = -1;
                    }
                    try
                    {
                        Object obejct = reader.GetValue(3);
                        team.TeamManager = (string)reader.GetValue(4);

                    }
                    catch (Exception e)
                    {
                        team.TeamManager = "";
                    }
                   

                    arrTeams.Add(team);
                }

                reader.Close();
            }
            catch (System.Exception ex)
            {
                if (reader != null)
                    reader.Close();

                System.Windows.Forms.MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        private RegularTeam FindTeam(string strTeamCode, ArrayList arrTeams)
        {
            foreach (RegularTeam team in arrTeams)
            {
                if (team.TeamCode == strTeamCode)
                    return team;
            }

            return null;
        }

        public bool LoadCompanyMemberList(/*Out*/ArrayList arrTeamMembers, /*In*/ArrayList arrTeams)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT DEPTNO, LEVELNO, MAILNO, NAME, TELNO, MOBILE_PHN, TITLE, JANG_YN, EMPNO from webadm.view_insa_tbl_sec", m_Connection);
            OleDbDataReader reader = null;

            try
            {
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string strTeamCode = "";
                    RegularTeamMember member = new RegularTeamMember();

                    try
                    {
                        strTeamCode = (string)reader.GetValue(0);
                    }
                    catch (Exception e)
                    {
                        strTeamCode = "";
                    }
                    if (strTeamCode == "")
                        continue;
                    RegularTeam team = FindTeam(strTeamCode, arrTeams);
                    member.Team = team;

                    try
                    {
                        member.LEVELNO = (string)reader.GetValue(1);
                    }
                    catch (Exception e)
                    {
                        member.LEVELNO = "";
                    }

                    try
                    {
                        member.MailAddress = (string)reader.GetValue(2);
                    }
                    catch (Exception e)
                    {
                        member.MailAddress = "";
                    }

                    try
                    {
                        member.NAME = (string)reader.GetValue(3);
                    }
                    catch (Exception e)
                    {
                        member.NAME = "";
                    }

                    try
                    {
                        member.TelNo = (string)reader.GetValue(4);
                    }
                    catch (Exception e)
                    {
                        member.TelNo = "";
                    }

                    try
                    {
                        member.HandPhoneNumber = (string)reader.GetValue(5);
                    }
                    catch (Exception e)
                    {
                        member.HandPhoneNumber = "";
                    }

                    try
                    {
                        member.Title = (string)reader.GetValue(6);
                    }
                    catch (Exception e)
                    {
                        member.Title = "";
                    }

                    string strYN = "";// (string)reader.GetValue(6);
                    try
                    {
                        strYN = (string)reader.GetValue(7);
                    }
                    catch (Exception e)
                    {
                        strYN = "";
                    }
                    member.IsTeamLeader = strYN == "Y" || strYN == "y";


                    try
                    {
                        member.EMPNO =reader.GetValue(8).ToString();
                    }
                    catch (Exception e)
                    {
                        member.EMPNO = "";
                    }
                    arrTeamMembers.Add(member);
                }

                reader.Close();
            }
            catch (System.Exception ex)
            {
                if (reader != null)
                    reader.Close();

                System.Windows.Forms.MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }
    }
}
