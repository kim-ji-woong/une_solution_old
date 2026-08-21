using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlClient;

namespace HSMS
{
    class EditWorker : ChangedData
    {
        private int m_nSQLType = 0;
        private DataWorker m_worker = null;
        private DataManager m_DataMgr = null;

        private VariousData<string> m_MemberID = null;
        private VariousData<int> m_EnterLevel = null;


        public int ID
        {
            get { return m_worker== null ? -1 : m_worker.ID; }
        }
        public string MemberID
        {
            set { m_MemberID = new VariousData<string>(value); }
        }
        public int EnterLevel
        {
            set { m_EnterLevel = new VariousData<int>(value); }
        }


        public new int SQLType
        {
            get { return m_nSQLType; }
            set { m_nSQLType = value; }
        }

        public DataWorker Worker
        {
            get { return m_worker; }
            set { m_worker = value; }
        }

        public EditWorker()
        {
            m_DataMgr = FormMain.Instance.DataMgr;
        }

        public override bool Update(DBConn conn)
        {
            if (m_worker == null)
                return false;
            
            try
            {
                 NetworkManager netMgr = FormMain.Instance.NetMgr;
                //수정
                if (m_nSQLType == ChangedData.UPDATE)
                {
                    ArrayList arrDatas = new ArrayList();
                    arrDatas.Add((int)ChangeDataType.WORKER);
                    arrDatas.Add(m_nSQLType);
                    arrDatas.Add(m_worker.ID);
                    arrDatas.Add(m_worker.EnterLevel);

                    byte[] sendBytes = ClientProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA, arrDatas);
                    netMgr.Send(sendBytes, netMgr.ClientProvider); 
                }
                else if (m_nSQLType == ChangedData.DELETE)//삭제
                {                    
                    ArrayList arrDatas = new ArrayList();                    
                    arrDatas.Add((int)ChangeDataType.WORKER);
                    arrDatas.Add(m_nSQLType);
                    arrDatas.Add(m_worker.ID);
                    arrDatas.Add(m_worker.MemberID);

                    byte[] sendBytes = ClientProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA, arrDatas);

                    netMgr.Send(sendBytes, netMgr.ClientProvider);                   

                    //m_worker.ID = -1;
                    //m_worker.EnterLevel = -1;
                }
                else if (m_nSQLType == ChangedData.INSERT)//삽입
                {
                    int nSiteID = FormMain.Instance.SiteID;

                    ArrayList arrDatas = new ArrayList();
                    arrDatas.Add((int)ChangeDataType.WORKER);
                    arrDatas.Add(m_nSQLType);
                    arrDatas.Add(m_worker.MemberID);
                    arrDatas.Add(m_worker.EnterLevel);                    
                    arrDatas.Add(nSiteID);
                    arrDatas.Add(m_worker.SensorDetect);

                    byte[] sendBytes = ClientProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA, arrDatas);

                    netMgr.Send(sendBytes, netMgr.ClientProvider);                 
                }
                return true;
            }
            catch (System.Exception)
            {                
            }            
            return false;
        }

       
        public override void AddToManager(IChangedDataManager mgr)
        {
            //ArrayList arrDatas = mgr.GetDataList();
            //Type type = typeof(EditWorker);

            //foreach (ChangedData data in arrDatas)
            //{
            //    //이미 같은 데이터가 편집되었는가?
            //    if (data.GetType() == type)
            //    {
            //        EditWorker editWorker = (EditWorker)data;
            //        if (editWorker.Worker.MemberID == this.Worker.MemberID && editWorker.Worker.EnterLevel == this.Worker.EnterLevel)
            //        {
            //            editWorker.m_nSQLType = this.m_nSQLType;

            //            //데이터 넣었다가 뺀거면 값을 바꿀 필요가 없음
            //            if (editWorker.ID < 0)
            //            {
            //                if (this.m_nSQLType == 2)
            //                {
            //                    arrDatas.Remove(data);
            //                    mgr.SomethingChanged(null);
            //                    return;
            //                }
            //            }

            //            if (this.m_MemberID != null)
            //                editWorker.m_MemberID = this.m_MemberID;
            //            if (this.m_EnterLevel != null)
            //                editWorker.m_EnterLevel = this.m_EnterLevel;

            //            return;
            //        }
            //    }
            //}

            //mgr.SomethingChanged(this);
        }

        public static void ProcessChangeDataList(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            for (int i = 1; i < nDataCount; i++)
            {
                try
                {
                    int nSqlType = (int)arrDatas[i];

                    if (nSqlType == (int)ChangedData.UPDATE)
                        i = ProcessUpdate(arrDatas, i + 1);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        // Return 값 : Last Index
        private static int ProcessUpdate(ArrayList arrDatas, int nIndex)
        {
            int nWorkerID = (int)arrDatas[nIndex++];
            string strMemberID = (string)arrDatas[nIndex++];
            int nEnterLevel = (int)arrDatas[nIndex++];
            int nSiteID = (int)arrDatas[nIndex++];
            bool isDetect = (bool)arrDatas[nIndex];

            DataManager dataMgr = FormMain.Instance.DataMgr;
            DataWorker worker = dataMgr.GetWorkerFromID(nWorkerID);

            if (worker != null)
            {
                worker.MemberID = strMemberID;
                worker.EnterLevel = nEnterLevel;
                worker.DBEnterLevel = nEnterLevel;
                worker.SiteID = nSiteID;
                worker.SensorDetect = isDetect;
            }

            return nIndex;
        }
    }
}
