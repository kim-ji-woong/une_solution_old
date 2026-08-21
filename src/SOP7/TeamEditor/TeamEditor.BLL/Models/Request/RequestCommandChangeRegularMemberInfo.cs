using System;
using System.Collections.Generic;
using System.Text;
using TeamEditor.BLL.Models.Response;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.BLL.Models.Request
{
    public class RequestCommandChangeRegularMemberInfo : RequestCommand
    {
        // 변경된 정보
        private string m_strInfoType = "";
        public string InfoType
        {
            get { return m_strInfoType; }
            set { m_strInfoType = value; }
        }

        private object m_OrgData = null;
        public object OrgData
        {
            get { return m_OrgData; }
            set { m_OrgData = value; }
        }

        private object m_ChgData = null;
        public object ChgData
        {
            get { return m_ChgData; }
            set { m_ChgData = value; }
        }

        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private int m_nRegularID = -1;
        public int RegularID
        {
            get { return m_nRegularID; }
            set { m_nRegularID = value; }
        }

        private string m_strMemberName = "";
        public string MemberName
        {
            get { return m_strMemberName; }
            set { m_strMemberName = value; }
        }

        private string m_strMemberID = "";
        public string MemberID
        {
            get { return m_strMemberID; }
            set { m_strMemberID = value; }
        }

        private string m_strOfficePhoneNumber = "";
        public string OfficePhoneNumber
        {
            get { return m_strOfficePhoneNumber; }
            set { m_strOfficePhoneNumber = value; }
        }

        private string m_strPhoneNumber = "";
        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        private int? m_nJobLevelID = -1;
        public int? JobLevelID
        {
            get { return m_nJobLevelID; }
            set { m_nJobLevelID = value; }
        }

        private int? m_nJobPositionID = -1;
        public int? JobPositionID
        {
            get { return m_nJobPositionID; }
            set { m_nJobPositionID = value; }
        }

        public override void SaveDB()
        {
            if (m_nID < 0)
            {
                if (this.IsRedo)
                {
                    AddDB();
                }
                else
                {

                }
            }
            else
            {
                if (this.IsRedo)
                    UpdateDB();
                else
                    RemoveDB();
            }
        }

        public bool AddDB()
        {
            string strErrorMessage;
            int nID = this.DataManager.GetSelectManager().GetMaxID("SopTeamRegularMember", out strErrorMessage);
            
            RegularMember member = new RegularMember();
            member.ID = nID;
            member.RegularID = m_nRegularID;
            member.MemberName = m_strMemberName;
            member.MemberID = m_strMemberID;
            member.OfficePhoneNumber = m_strOfficePhoneNumber;
            member.PhoneNumber = m_strPhoneNumber;
            member.JobLevelID = m_nJobLevelID;
            member.JobPositionID = m_nJobPositionID;

            RegularMember createMember = this.DataManager.GetCreateManager().AddRegularMember(member, out strErrorMessage);
            if (createMember == null)
            {
                return false;
            }

            m_nID = nID;

            return true;
        }

        private void UpdateDB()
        {
            if (m_strInfoType.Length == 0)
                return;

            string strErrorMessage = null;

            Dictionary<RegularMember.Fields, object> dicConditions = new Dictionary<RegularMember.Fields, object>();
            dicConditions.Add(RegularMember.Fields.ID, m_nID);

            Dictionary<RegularMember.Fields, object> dicSets = new Dictionary<RegularMember.Fields, object>();

            string value = m_ChgData.ToString();
            if (!this.IsRedo)
            {
                if (m_OrgData == null)
                    value = "";
                else
                    value = m_OrgData.ToString();
            }

            RegularMember.Fields field = RegularMember.Fields.MemberName;

            switch (m_strInfoType)
            {  
                case "MemberID":
                    field = RegularMember.Fields.MemberID;
                    break;
                case "OfficePhoneNumber":
                    field = RegularMember.Fields.OfficePhoneNumber;
                    break;
                case "PhoneNumber":
                    field = RegularMember.Fields.PhoneNumber;
                    value = LoadManager.EncryptString(value);
                    break;
                case "JobLevelID":
                    field = RegularMember.Fields.JobLevelID;
                    break;
                case "JobPositionID":
                    field = RegularMember.Fields.JobPositionID;
                    break;
            }

            dicSets.Add(field, value);

            this.DataManager.GetUpdateManager().UpdateRegularMember(dicSets, dicConditions, out strErrorMessage);            
        }

        public void RemoveDB()
        {
            RequestCommandRemoveRegularMember cmd = new RequestCommandRemoveRegularMember();
            cmd.DataManager = this.DataManager;
            cmd.ID = m_nID;
            cmd.RemoveDB();
        }
    }
}
