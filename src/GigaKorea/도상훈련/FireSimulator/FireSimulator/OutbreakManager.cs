using DBUtility2;
using FireSimulator.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FireSimulator
{
    public class OutbreakManager
    {
        private bool m_shutdownThread = false;
        private WebDBManager m_dbMgr = null;

        private int m_nActionStepID = -1;
        private int m_nActionStepHistoryID = -1;

        public void Shoutdown()
        {
            m_shutdownThread = true;
        }

        public OutbreakManager (WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;

            Thread t = new Thread(new ThreadStart(ReloadThread));
            t.Start();
        }

        private void ReloadThread()
        {
            while (m_shutdownThread == false)
            {
                LoadOutbreakList();

                Thread.Sleep(1000);
            }
        }

        private bool LoadOutbreakList()
        {
            int nActionStepID = -1;
            int nActionStepHistoryID = -1;

            //0.액션스텝 히스토리에서 엔드 또는 켄슬 값이 널인 값을 조회
            bool bChk = LoadActionStepIDs(out nActionStepHistoryID, out nActionStepID);

            if (nActionStepHistoryID == -1 && m_nActionStepHistoryID != -1)
            {   // 상황이 발생했다가 상황이 종료된 경우
                FormMain.Instance.ClearOutbreakComboList();
                m_nActionStepID = -1;
                m_nActionStepHistoryID = -1;
                return false;
            }
            else if (nActionStepHistoryID == -1)
            {   // 상황이 없는 경우
                return false;
            }
            else if (nActionStepID == m_nActionStepID)
            {   // 같은 상황일 경우
                return true;
            }
            else
            {   // 상황이 발생한 경우 
                m_nActionStepID = nActionStepID;
                m_nActionStepHistoryID = nActionStepHistoryID;
            }
                

            //1.프로세스 테이블 검색하여 해당 프로세스를 객체 만들기
            Dictionary<int, ProcessData> dicProcess = LoadProcess(m_nActionStepID);

            if (dicProcess == null)
                return false;

            //2.arrow 테이블에서 해당하는 arrow 중 EndComponentID에 해당하는 프로세스는 자식 프로세스로 입력(bool으로 체크)
            if (!CheckProcess(m_nActionStepID, dicProcess))
                return false;

            //3.자식 프로세스가 아닌 것만 드롭박스 아이템으로 사용
            Dictionary<int, OutbreakData> dicOutbreakDatas = MakeOutbreak(m_nActionStepHistoryID, dicProcess);

            //4.가장 첫번째 프로세스는 제외(메인 프로세스로 해당)
            FormMain.Instance.ShowOutbreakComboList(dicOutbreakDatas);

            return true;
        }

        private bool LoadActionStepIDs(out int nActionStepHistoryID, out int nActionStepID)
        {
            nActionStepHistoryID = -1;
            nActionStepID = -1;

            string strSQL = string.Format("SELECT ID, ActionStepID FROM actionstephistory where EndTime IS NULL AND CancelTime IS NULL");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            for (int i = 0; i < nCount - 1; i += 2)
            {
                nActionStepHistoryID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                nActionStepID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
            }

            return true;
        }

        private Dictionary<int, ProcessData> LoadProcess(int nStepMemberID)
        {
            Dictionary<int, ProcessData> dicProcess = new Dictionary<int, ProcessData>();
            bool bFirst = true;

            string strSQL = string.Format("SELECT ID, Text FROM process where StepMemberID = " + nStepMemberID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return null;

            int nCount = arrResult.Count;
            if (nCount == 0) return dicProcess;

            for (int i = 0; i < nCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strText = WebDBManager.GetStringField(arrResult[i + 1], "");

                ProcessData process = new ProcessData();
                process.ID = nID;
                process.Text = strText;

                // 첫번째 프로세스가 메인 프로세스
                if (bFirst)
                {
                    process.First = true;
                    bFirst = false;
                }

                dicProcess[nID] = process;
            }

            return dicProcess;
        }

        private bool CheckProcess(int nStepMemberID, Dictionary<int, ProcessData> dicProcess)
        {
            List<int> listProcessID = new List<int>();

            string strSQL = string.Format("SELECT EndComponentID FROM arrow where StepMemberID = " + nStepMemberID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            for (int i = 0; i < nCount; i++)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);

                if (!listProcessID.Contains(nID))
                    listProcessID.Add(nID);
            }

            foreach (int nProcessID in listProcessID)
            {
                if (dicProcess.ContainsKey(nProcessID))
                    dicProcess[nProcessID].Child = true;
            }

            return true;
        }

        private Dictionary<int, OutbreakData> MakeOutbreak(int nActionStepHistoryID, Dictionary<int, ProcessData> dicProcess)
        {
            Dictionary<int, OutbreakData> dicOutbreakData = new Dictionary<int, OutbreakData>();

            foreach(KeyValuePair<int, ProcessData> pair in dicProcess)
            {
                ProcessData data = pair.Value;

                if (data.First == false && data.Child == false)
                {
                    OutbreakData outbreak = new OutbreakData();
                    outbreak.ProcessID = data.ID;
                    outbreak.ActionStepHistoryID = nActionStepHistoryID;
                    outbreak.Text = data.Text;

                    dicOutbreakData[data.ID] = outbreak;
                }
            }

            return dicOutbreakData;
        }
    }
}
