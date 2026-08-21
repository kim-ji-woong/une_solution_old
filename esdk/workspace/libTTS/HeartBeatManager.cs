using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace libTTS
{
    class HeartBeatManager
    {
        private VariousData<int> m_id = null;

        // 방송서버가 작동중임을 알려준다.
        public void HeartBeat(WebDBManager dbMgr, int nState)
        {
            if (m_id == null)
            {
                InsertHeartBeat(dbMgr);
            }

            if (m_id != null)
            {
                DateTime nDate = DateTime.Now;
                string strSQL = string.Format("UPDATE BroadcastState SET HEARTBEAT= '{0} {1:00}:{2:00}:{3:00}', BSTATE ={4} WHERE ID = {5}"
                    , nDate.ToShortDateString(), nDate.Hour, nDate.Minute, nDate.Second, nState, m_id.Data);

                dbMgr.GetResultData(strSQL);
            }
        }

        private void InsertHeartBeat(WebDBManager dbMgr)
        {
            ReadStateID(dbMgr);

            if (m_id == null)
            {
                DateTime nDate = DateTime.Now;

                string strSQL = string.Format("INSERT INTO BroadcastState (HOSTADDRESS, HEARTBEAT, BSTATE, BDescription, SiteID) VALUES ('', '{0} {1:00}:{2:00}:{3:00}', 0, '', {4})"
                    , nDate.ToShortDateString(), nDate.Hour, nDate.Minute, nDate.Second, dbMgr.SiteID);

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    ReadStateID(dbMgr);
                }
            }
        }

        private void ReadStateID(WebDBManager dbMgr)
        {
            string strSQL = "Select ID from BroadcastState where SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            if (arrResult.Count > 0)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                if (id != null)
                    m_id = id;
            }
        }
    }
}
