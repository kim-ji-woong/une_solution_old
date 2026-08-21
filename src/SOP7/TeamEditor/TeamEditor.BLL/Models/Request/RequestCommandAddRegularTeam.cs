using System;
using System.Collections.Generic;
using System.Text;
using TeamEditor.BLL.Models.Response;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.BLL.Models.Request
{
    public class RequestCommandAddRegularTeam : RequestCommand
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private string m_strName = "";
        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        private int m_nParentTeamID = -1;
        public int ParentTeamID
        {
            get { return m_nParentTeamID; }
            set { m_nParentTeamID = value; }
        }

        public override void SaveDB()
        {
            if (this.IsRedo)
            {
                //    if (nodes.Contains(m_node))
                //    {
                if (m_nID < 0)
                    AddTeam();
                else
                    CheckNAdd();
                //    }
            }
            else
            {
                if (m_nID >= 0)
                    RemoveTeam();
            }
        }

        private bool AddTeam()
        {
            if (m_nParentTeamID < 0)
            {
                return false;
            }

            string strErrorMessage;
            int nID = this.DataManager.GetSelectManager().GetMaxID("SopTeamRegular", out strErrorMessage);
            
            Regular regular = new Regular();
            regular.ID = nID;
            regular.TeamName = m_strName;
            regular.ParentTeamID = m_nParentTeamID;

            bool isCheck = this.DataManager.GetCreateManager().AddRegular(regular, out strErrorMessage);
            if (!isCheck)
            {
                return false;
            }

            m_nID = nID;

            return true;
        }

        private bool CheckNAdd()
        {
            string strErrorMessage;
            Regular company = this.DataManager.GetSelectManager().SelectRegular(m_nID, out strErrorMessage);
            //Regular company = this.DataManager.GetSelectManager().GetRegular(m_nID, out strErrorMessage);

            if (company == null)
                return false;

            if (company.ID == -1)
                return AddTeam();

            return true;
        }

        private void RemoveTeam()
        {
            RequestCommandRemoveRegularTeam cmd = new RequestCommandRemoveRegularTeam();
            cmd.DataManager = this.DataManager;
            cmd.IsRedo = true; // 추가했다가 삭제하는것이므로 true
            cmd.Key = this.Key;
            cmd.ID = m_nID;

            Regular regular = new Regular();
            regular.ID = m_nID;
            regular.TeamName = m_strName;
            regular.ParentTeamID = m_nParentTeamID;
            if (cmd.DeleteTeams == null)
                cmd.DeleteTeams = new List<Regular>();
            cmd.DeleteTeams.Add(regular);

            cmd.SaveDB();
        }
    }
}
