using System;
using System.Collections.Generic;
using System.Text;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.BLL.Models.Request
{
    public class RequestCommandRemoveRegularTeam : RequestCommand
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        // 하위 팀 ID
        private List<Regular> m_deleteTeams = null;
        public List<Regular> DeleteTeams
        {
            get { return m_deleteTeams; }
            set { m_deleteTeams = value; }
        }

        private List<RegularMember> m_deleteMembers = null;
        public List<RegularMember> DeleteMembers
        {
            get { return m_deleteMembers; }
            set { m_deleteMembers = value; }
        }

        public override void SaveDB()
        {
            if (this.IsRedo)
            {
                RemoveDB();
            }
            else
            {
                Rollback();
            }
        }

        private void RemoveDB()
        {
            string strErrorMessage;
                        
            if (m_deleteMembers != null)
            {
                // 하위팀에 속해있는 직원까지 삭제
                for (int i = 0; i < m_deleteMembers.Count; i++)
                {
                    this.DataManager.GetDeleteManager().DeleteRegularMember(m_deleteMembers[i].ID, out strErrorMessage);
                } 
            }

            if (m_deleteTeams != null)
            {
                // 하위팀부터 삭제하기 위해 역순으로 정렬한다.
                m_deleteTeams.Reverse();

                // 하위팀까지 삭제
                for (int i = 0; i < m_deleteTeams.Count; i++)
                {
                    this.DataManager.GetDeleteManager().DeleteRegular(m_deleteTeams[i].ID, out strErrorMessage);
                } 
            }
        }

        private void Rollback()
        {
            string strErrorMessage;

            if (m_deleteTeams != null)
            {
                foreach (Regular item in m_deleteTeams)
                {
                    this.DataManager.GetCreateManager().AddRegular(item, out strErrorMessage);
                } 
            }

            if (m_deleteMembers != null)
            {
                foreach (RegularMember item in m_deleteMembers)
                {
                    this.DataManager.GetCreateManager().AddRegularMember(item, out strErrorMessage);
                } 
            }
        }
    }
}
