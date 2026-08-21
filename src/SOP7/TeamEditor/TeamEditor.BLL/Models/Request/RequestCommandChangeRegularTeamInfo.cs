using System;
using System.Collections.Generic;
using System.Text;
using TeamEditor.BLL.Models.Response;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.BLL.Models.Request
{
    public class RequestCommandChangeRegularTeamInfo : RequestCommand
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private string m_strNewData = "";
        public string NewData
        {
            get { return m_strNewData; }
            set { m_strNewData = value; }
        }

        private string m_strOrgData = "";
        public string OrgData
        {
            get { return m_strOrgData; }
            set { m_strOrgData = value; }
        }

        public override void SaveDB()
        {
            string strErrorMessage = null;

            Dictionary<Regular.Fields, object> dicConditions = new Dictionary<Regular.Fields, object>();
            dicConditions.Add(Regular.Fields.ID, m_nID);
            
            Dictionary<Regular.Fields, object> dicSets = new Dictionary<Regular.Fields, object>();

            if (this.IsRedo)
                dicSets.Add(Regular.Fields.TeamName, m_strNewData);
            else
                dicSets.Add(Regular.Fields.TeamName, m_strOrgData);

            DataManager.GetUpdateManager().UpdateRegular(dicSets, dicConditions, out strErrorMessage);            
        }
    }
}
