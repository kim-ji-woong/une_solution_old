using DBUtility2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TeamEditor.IDAL;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.BLL.WinForms.Command
{
    public class CommandChangeTeamInfo : CommandEx
    {
        private Team m_team = null;
        private ChangedData<string> m_teamName = null;
        private TreeNode m_node = null;
        private TeamTreeView.TeamType m_teamType = TeamTreeView.TeamType.REGULAR;

        private IDataManager m_dataManager = null;

        public Team Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        public ChangedData<string> TeamName
        {
            get { return m_teamName; }
            set { m_teamName = value; }
        }

        public TreeNode TreeNode
        {
            get { return m_node; }
            set { m_node = value; }
        }

        public TeamTreeView.TeamType TeamType
        {
            get { return m_teamType; }
            set { m_teamType = value; }
        }

        

        public CommandChangeTeamInfo()
        {
        }

        public CommandChangeTeamInfo(IDataManager dataManager, Team team, ChangedData<string> teamName, TreeNode node, TeamTreeView.TeamType teamType)
        {
            m_dataManager = dataManager;
            m_team = team;
            m_teamName = teamName;
            m_node = node;
            m_teamType = teamType;
        }

        public override void Do()
        {
            if (m_team == null || m_node == null)
                return;

            if (m_teamName != null)
            {
                m_team.TeamName = m_teamName.Changed;
                m_node.Text = m_teamName.Changed;
            }
        }

        public override void RollBack()
        {
            if (m_team == null || m_node == null)
                return;

            if (m_teamName != null)
            {
                m_team.TeamName = m_teamName.Origin;
                m_node.Text = m_teamName.Origin;
            }
        }

        public override void SaveDB(WebDBManager dbMgr, bool dir)
        {
            if (m_team == null || m_team.TeamID < 0)
                return;

            string strSQL = "";

            if (m_teamName != null)
            {
                if (strSQL.Length == 0)
                    strSQL = "TeamName = '" + m_team.TeamName + "'";
                else
                    strSQL = ", TeamName = '" + m_team.TeamName + "'";
            }

            if (strSQL.Length == 0)
                return;

            //if (m_teamType == TeamTreeView.TeamType.REGULAR)
            //    strSQL = "Update RegularTeam set " + strSQL + " where ID = " + m_team.TeamID.ToString();
            //else if (m_teamType == TeamTreeView.TeamType.TEMPORARY_NORMAL)
            //    strSQL = "Update TemporaryNormalTeam set " + strSQL + " where ID = " + m_team.TeamID.ToString();
            //else if (m_teamType == TeamTreeView.TeamType.TEMPORARY_EMERGENCY)
            //    strSQL = "Update TemporaryEmergencyTeam set " + strSQL + " where ID = " + m_team.TeamID.ToString();
            //else if (m_teamType == TeamTreeView.TeamType.EXTERNAL)
            //    strSQL = "Update ExternalTeam set " + strSQL + " where ID = " + m_team.TeamID.ToString();

            //dbMgr.GetResultData(strSQL);

            Company company = new Company();
            company.ID = m_team.TeamID;
            company.TeamName = m_team.TeamName;
            //company.ParentTeamID = m_team.

            string strErrorMessage;
            m_dataManager.GetUpdateManager().UpdateCompany(company, out strErrorMessage);

            if (m_teamType == TeamTreeView.TeamType.REGULAR)
                UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.REGULAR_TEAM);
            else if (m_teamType == TeamTreeView.TeamType.TEMPORARY_NORMAL)
                UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.TEMPARARY_NORMAL_TEAM);
            else if (m_teamType == TeamTreeView.TeamType.TEMPORARY_EMERGENCY)
                UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.TEMPARAY_EMERGENCY_TEAM);
            else if (m_teamType == TeamTreeView.TeamType.EXTERNAL)
                UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.EXTERNAL_TEAM);
        }
    }
}
