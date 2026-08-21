using dnsDBUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TeamEditor.IDAL;

namespace TeamEditor.DAL
{
    public class DeleteManager : QueryManager, IDelete
    {
        private DataManager m_dataMgr = null;

        public DeleteManager(DataManager dataMgr)
        {
            m_dataMgr = dataMgr;
            m_dbManager = dataMgr.GetDBManager() as WebDBManager;
        }

        public bool DeleteRegularMember(int id, out string strErrorMessage)
        {
            strErrorMessage = "";
            string strSQL = "Delete From SopTeamRegularMember Where id = " + id;
            ArrayList arrResults = m_dbManager.GetResultData(strSQL);

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool DeleteRegular(int id, out string strErrorMessage)
        {
            strErrorMessage = "";
            string strSQL = "Delete From SopTeamRegular Where id = " + id;
            ArrayList arrResults = m_dbManager.GetResultData(strSQL);

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool DeleteTemporary(int id, out string strErrorMessage)
        {
            strErrorMessage = "";
            string strSQL = "Delete From SopTeamTemporary Where id = " + id;
            ArrayList arrResults = m_dbManager.GetResultData(strSQL);

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool DeleteTemporaryMember(int id, out string strErrorMessage)
        {
            strErrorMessage = "";
            string strSQL = "Delete From SopTeamTemporaryMember Where id = " + id;
            ArrayList arrResults = m_dbManager.GetResultData(strSQL);

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool DeleteOptions(int id, out string strErrorMessage)
        {
            strErrorMessage = "";
            string strSQL = "Delete From SopTeamOptions Where id = " + id;
            ArrayList arrResults = m_dbManager.GetResultData(strSQL);

            if (arrResults == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        /// <summary>
        /// RegularMember와 관계된 테이블 데이터 모두 삭제
        /// </summary>
        /// <param name="id"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public bool DeleteRegularMember2(int id, out string strErrorMessage)
        {
            strErrorMessage = "";
            List<string> querys = new List<string>();
            querys.Add(string.Format("Delete From SopTeamTemporaryMember Where id = {0}", id));
            //strSQL = String.Format("DELETE FROM FacilityManager WHERE MemberType = 0 AND MemberID = {0} ", member.regularMember.ID);
            //strSQL = String.Format("DELETE FROM BuildingFacilityManager WHERE MemberType = 0 AND MemberID = {0} ", member.regularMember.ID);
            //strSQL = String.Format("DELETE FROM EquipZoneFacilityManager WHERE MemberType = 0 AND MemberID = {0} ", member.regularMember.ID);
            //strSQL = String.Format("DELETE FROM SOPGenUserCommander WHERE MemberType = 8 and MemberID = {0} ", member.regularMember.ID);
            //String.Format("UPDATE SOPGenUser SET MemberID = NULL WHERE MemberID = {0} ", member.regularMember.ID);
            querys.Add(string.Format("Delete From SopTeamRegularMember Where id = {0}", id));
            m_dbManager.BeginBatch();
            foreach (string sql in querys)
            {
                if (m_dbManager.GetBatchData(sql) == null)
                {
                    m_dbManager.BatchRollback();
                    strErrorMessage = m_dbManager.LastErrorMessage;
                    return false;
                }
            }
            m_dbManager.BatchCommit();
            return true;
        }
    }
}
