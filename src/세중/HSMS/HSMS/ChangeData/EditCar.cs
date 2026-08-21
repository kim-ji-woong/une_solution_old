using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace HSMS
{
    class EditCar : ChangedData
    {
        private int m_nSQLType = 0;
        private DataCar m_car = null;
        private VariousData<string> m_Code = null;
        
        public int ID
        {
            get { return m_car == null ? -1 : m_car.ID; }
        }
        public string Code
        {
            set { m_Code = new VariousData<string>(value); }
        }

        public new int SQLType
        {
            get { return m_nSQLType; }
            set { m_nSQLType = value; }
        }

        public DataCar Car
        {
            get { return m_car; }
            set { m_car = value; }
        }

        public EditCar()
        {            
        }

        public override bool Update(DBConn conn)
        {
            if (m_car == null)
                return false;

            NetworkManager netMgr = FormMain.Instance.NetMgr;

            if (m_nSQLType == ChangedData.DELETE)//삭제
            {
                ArrayList arData = new ArrayList();
                arData.Add((int)ChangeDataType.CAR);
                arData.Add(m_nSQLType);
                arData.Add(m_car.ID);
                arData.Add(m_car.Code);
                arData.Add(m_car.SiteID);
                arData.Add(m_car.SensorDetect);

                byte[] sendBytes = ClientProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA, arData);
                netMgr.Send(sendBytes, netMgr.ClientProvider);                 

                return true;
            }
            else if (m_nSQLType == ChangedData.INSERT)//삽입
            {
                ArrayList arData = new ArrayList();
                arData.Add((int)ChangeDataType.CAR);
                arData.Add(m_nSQLType);
                arData.Add(m_car.Code);
                arData.Add(m_car.SiteID);
                arData.Add(m_car.SensorDetect);

                byte[] sendBytes = ClientProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA, arData);
                netMgr.Send(sendBytes, netMgr.ClientProvider);
                return true;
            }
            return false;
        }
        public override void AddToManager(IChangedDataManager mgr)
        {
            throw new NotImplementedException();
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
            int nCarID = (int)arrDatas[nIndex++];
            string strCarNumber = (string)arrDatas[nIndex++];
            int nSiteID = (int)arrDatas[nIndex++];
            bool isDetect = (bool)arrDatas[nIndex];

            DataManager dataMgr = FormMain.Instance.DataMgr;
            DataCar car = dataMgr.GetCarFromID(nCarID);

            if (car != null)
            {
                car.Number = strCarNumber;
                car.SiteID = nSiteID;
                car.SensorDetect = isDetect;
            }

            return nIndex;
        }
    }
}
