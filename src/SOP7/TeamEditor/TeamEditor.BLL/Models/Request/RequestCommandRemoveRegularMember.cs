using System;
using System.Collections.Generic;
using System.Text;
using TeamEditor.BLL.Models.Response;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.BLL.Models.Request
{
    public class RequestCommandRemoveRegularMember : RequestCommand
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private RegularMember m_member = null;
        public RegularMember Member
        {
            get { return m_member; }
            set { m_member = value; }
        }

        public override void SaveDB()
        {
            if (this.IsRedo)
            {
                RemoveDB();
            }
            else
            {
                AddDB();
            }
        }

        private bool AddDB()
        {
            RequestCommandChangeRegularMemberInfo cmd = new RequestCommandChangeRegularMemberInfo();
            cmd.DataManager = this.DataManager;
            
            string strErrorMessage;
            int nID = this.DataManager.GetSelectManager().GetMaxID("SopTeamRegularMember", out strErrorMessage);
            
            RegularMember member = new RegularMember();
            cmd.ID = nID;
            cmd.RegularID = m_member.RegularID;
            cmd.MemberName = m_member.MemberName;
            cmd.MemberID = m_member.MemberID;
            cmd.OfficePhoneNumber = m_member.OfficePhoneNumber;
            cmd.PhoneNumber = member.PhoneNumber;
            cmd.JobLevelID = member.JobLevelID;
            cmd.JobPositionID = member.JobPositionID;

            return cmd.AddDB();
        }

        public void RemoveDB()
        {
            string strErrorMessage = null;
            this.DataManager.GetDeleteManager().DeleteRegularMember(m_nID, out strErrorMessage);
        }
    }
}
