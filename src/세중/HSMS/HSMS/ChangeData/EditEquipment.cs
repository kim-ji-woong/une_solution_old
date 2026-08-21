using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace HSMS
{
    class EditEquipment : ChangedData
    {
        private int m_nSQLType = 0;
        private DataEquip m_equip = null;
        private EquipmentGroup m_equipGroup = null;

        private VariousData<string> m_Code = null;
        private ArrayList m_arrDatas = null;

        public ArrayList Datas
        {
            get { return m_arrDatas; }
            set { m_arrDatas = value; }
        }

        public int ID
        {
            get { return m_equip == null ? -1 : m_equip.ID; }
        }
        public string Code
        {
            set { m_Code = new VariousData<string>(value); }
        }

        public EquipmentGroup EquipmentGroup
        {
            get { return m_equipGroup; }
            set { m_equipGroup = value; }
        }

        public new int SQLType
        {
            get { return m_nSQLType; }
            set { m_nSQLType = value; }
        }

        public DataEquip Equip
        {
            get { return m_equip; }
            set { m_equip = value; }
        }

        public EditEquipment()
        {
            
        }

        public override bool Update(DBConn conn)
        {
            if (m_equip == null)
                return false;
            try
            {
                ArrayList arrDatas = m_arrDatas;

                if (arrDatas == null)
                    return false;

                if (m_nSQLType == ChangedData.UPDATE)
                {
                    if (m_equipGroup == null)
                        return false;

                    arrDatas.Add(m_nSQLType);
                    arrDatas.Add(m_equip.ID);
                    arrDatas.Add(this.EquipmentGroup.GroupName);
                }
                else if (m_nSQLType == ChangedData.DELETE)
                {
                    arrDatas.Add(m_nSQLType);
                    arrDatas.Add(m_equip.ID);
                }
                else if (m_nSQLType == ChangedData.INSERT)
                {
                    int nSiteID = FormMain.Instance.SiteID;

                    DataManager dataMgr = FormMain.Instance.DataMgr;
                    Dictionary<string, EquipmentRawData> dicEquipRawData = dataMgr.DicEquipRawDatas;
                    if (dicEquipRawData.ContainsKey(m_equip.Name))
                    {
                        arrDatas.Add(m_nSQLType);
                        arrDatas.Add(m_equip.Code);
                        arrDatas.Add(this.EquipmentGroup.GroupName);
                        EquipmentRawData equipRawData = dicEquipRawData[m_equip.Name];
                        string strBoundary = equipRawData.Boundary;
                        arrDatas.Add(strBoundary);

                        string strSensorPos = equipRawData.SensorPos;
                        arrDatas.Add(strSensorPos);
                        string strSensorFinishPos = equipRawData.SensorFinishPos;
                        arrDatas.Add(strSensorFinishPos);
                        string strSensorDirVector = equipRawData.SensorDirVector;
                        arrDatas.Add(strSensorDirVector);
                        string strTextCenter = equipRawData.TextCenter;
                        arrDatas.Add(strTextCenter);
                        arrDatas.Add(nSiteID);
                    }
                }
                /*NetworkManager netMgr = FormMain.Instance.NetMgr;
                //수정
                if (m_nSQLType == ChangedData.UPDATE)
                {                   
                }
                else if (m_nSQLType == ChangedData.DELETE)//삭제
                {
                    ArrayList arrDatas = new ArrayList();
                    arrDatas.Add((int)ChangeDataType.EQUIP);
                    arrDatas.Add(m_nSQLType);
                    arrDatas.Add(m_equip.ID);

                    byte[] sendBytes = ClientProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA, arrDatas);
                    netMgr.Send(sendBytes, netMgr.ClientProvider);
                    return true;
                }
                else if (m_nSQLType == ChangedData.INSERT)//삽입
                {
                    int nSiteID = FormMain.Instance.SiteID;

                    DataManager dataMgr = FormMain.Instance.DataMgr;
                    Dictionary<string, EquipmentRawData> dicEquipRawData = dataMgr.DicEquipRawDatas;
                    if (dicEquipRawData.ContainsKey(m_equip.Name))
                    {
                        ArrayList arrDatas = new ArrayList();
                        arrDatas.Add((int)ChangeDataType.EQUIP);
                        arrDatas.Add(m_nSQLType);
                        arrDatas.Add(m_equip.Code);
                        EquipmentRawData equipRawData = dicEquipRawData[m_equip.Name];
                        string strBoundary = equipRawData.Boundary;
                        arrDatas.Add(strBoundary);

                        string strSensorPos = equipRawData.SensorPos;
                        arrDatas.Add(strSensorPos);
                        string strSensorFinishPos = equipRawData.SensorFinishPos;
                        arrDatas.Add(strSensorFinishPos);
                        string strSensorDirVector = equipRawData.SensorDirVector;
                        arrDatas.Add(strSensorDirVector);
                        string strTextCenter = equipRawData.TextCenter;
                        arrDatas.Add(strTextCenter);
                        arrDatas.Add(nSiteID);
                        byte[] sendBytes = ClientProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA, arrDatas);

                        netMgr.Send(sendBytes, netMgr.ClientProvider);
                        return true;
                    }
                }*/
            }
            catch (System.Exception)
            {
            }
            return true;
        }
        public override void AddToManager(IChangedDataManager mgr)
        {
            throw new NotImplementedException();
        }

        public static bool ProcessChangeDataList2(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            for (int i = 1; i < nDataCount; i++)
            {
                try
                {
                    int nSqlType = (int)arrDatas[i];

                    if (nSqlType == (int)ChangedData.UPDATE)
                        i = ProcessUpdate2(arrDatas, i + 1);
                    else if (nSqlType == (int)ChangedData.DELETE)
                        i = ProcessDelete2(arrDatas, i + 1);
                    else if (nSqlType == (int)ChangedData.INSERT)
                    {
                        i = ProcessInsert(arrDatas, i + 1);

                        if (i < 0)
                            return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

        private static int ProcessInsert(ArrayList arrDatas, int nIndex)
        {
            DataManager dataMgr = FormMain.Instance.DataMgr;

            int nEquipID = (int)arrDatas[nIndex++];
            string szEquipName = (string)arrDatas[nIndex++];
            string strEquipGroupName = (string)arrDatas[nIndex++];
            string szBoundary = (string)arrDatas[nIndex++];
            string szSensorPos = (string)arrDatas[nIndex++];
            string szSensorFinishPos = (string)arrDatas[nIndex++];
            string szSensorDirVector = (string)arrDatas[nIndex++];
            string szTextCenter = (string)arrDatas[nIndex++];
            int nSiteID = (int)arrDatas[nIndex++];

            Dictionary<string, DataEquip> dicEquip = ERPManager.Instance.DicEquips;
            if (dicEquip.ContainsKey(szEquipName))
            {
                DataEquip equip = dicEquip[szEquipName];

                if (equip != null)
                {
                    equip.SiteID = nSiteID;
                    UnE.Geometry.Polygon polygon = dataMgr.GetPolygon(szBoundary);
                    if (polygon != null)
                    {
                        UnE.Geometry.Vertex2D vEquipOrigin = dataMgr.ResetPolygonCoords(polygon);
                        UnE.Geometry.Vertex2D vSensorPos = dataMgr.GetVertex(szSensorPos);
                        UnE.Geometry.Vertex2D vSensorFinishPos = dataMgr.GetVertex(szSensorFinishPos);
                        UnE.Geometry.Vertex2D vSensorDirVector = dataMgr.GetVertex(szSensorDirVector);
                        equip.Boundary = polygon;

                        if (vSensorPos != null)
                            equip.SensorPosition = vSensorPos;

                        if (vSensorFinishPos != null)
                            equip.SensorFinishPosition = vSensorFinishPos;

                        if (vSensorDirVector != null)
                            equip.SensorDirVector = vSensorDirVector;

                        equip.Boundary = polygon;
                        equip.OriginPosition = vEquipOrigin;

                        EquipmentGroup group = dataMgr.FindEquipmentGroup(strEquipGroupName);

                        if (group == null)
                        {
                            if (strEquipGroupName == EquipmentGroup.DefaultEquipmentGroup.GroupName ||
                                strEquipGroupName == EquipmentGroup.DefaultEquipmentGroup.ToString())
                                group = EquipmentGroup.DefaultEquipmentGroup;
                            else
                                group = new EquipmentGroup(strEquipGroupName);

                            dataMgr.AddEquipmentGroup(group);
                        }

                        equip.EquipmentGroup = group;
                        equip.ID = nEquipID;
                    }

                    dataMgr.AddEquip(equip);
                }
            }

            return nIndex - 1;
        }

        // Return 값 : Last Index
        private static int ProcessDelete2(ArrayList arrDatas, int nIndex)
        {
            int nEquipID = (int)arrDatas[nIndex++];

            DataManager dataMgr = FormMain.Instance.DataMgr;
            DataEquip equip = dataMgr.GetEquipFromID(nEquipID);

            if (equip != null)
            {
                equip.ID = -1;
                dataMgr.RemoveEquip(equip);
            }

            return nIndex - 1;
        }

        // Return 값 : Last Index
        private static int ProcessUpdate2(ArrayList arrDatas, int nIndex)
        {
            int nEquipID = (int)arrDatas[nIndex++];
            string strEquipGroupName = (string)arrDatas[nIndex++];

            DataManager dataMgr = FormMain.Instance.DataMgr;
            DataEquip equip = dataMgr.GetEquipFromID(nEquipID);

            if (equip != null)
            {
                EquipmentGroup group = dataMgr.FindEquipmentGroup(strEquipGroupName);

                if (group == null)
                {
                    if (strEquipGroupName == EquipmentGroup.DefaultEquipmentGroup.GroupName ||
                        strEquipGroupName == EquipmentGroup.DefaultEquipmentGroup.ToString())
                        group = EquipmentGroup.DefaultEquipmentGroup;
                    else
                        group = new EquipmentGroup(strEquipGroupName);

                    dataMgr.AddEquipmentGroup(group);
                }

                equip.EquipmentGroup = group;
            }

            return nIndex - 1;
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

        public static void ProcessChangeAlarmDistance(ArrayList arrDatas)
        {
            float fWorkerToCarDistanceBoth, fWorkerToCarDistanceOneSide/*, fWorkerToZoneDistance, fWorkerToEquipDistance*/;
            float fCoGasTolerance, fMethaneTolerance;
            int nIndex = 1;

            Dictionary<string, float> dicWorkerToZoneDistance = new Dictionary<string, float>();
            Dictionary<string, float> dicWorkerToEquipDistance = new Dictionary<string, float>();

            if (GetAlarmDistance(arrDatas, ref nIndex, out fWorkerToCarDistanceBoth))
                FormMain.Instance.DataMgr.WorkerToCarDistanceBoth = fWorkerToCarDistanceBoth;

            if (GetAlarmDistance(arrDatas, ref nIndex, out fWorkerToCarDistanceOneSide))
                FormMain.Instance.DataMgr.WorkerToCarDistanceOneSide = fWorkerToCarDistanceOneSide;

            if (GetAlarmDistance(arrDatas, ref nIndex, dicWorkerToZoneDistance))
            {
                foreach (KeyValuePair<string, float> pair in dicWorkerToZoneDistance)
                {
                    FormMain.Instance.DataMgr.SetWorkerToZoneDistance(pair.Key, pair.Value);
                }
                //FormMain.Instance.DataMgr.WorkerToZoneDistance = fWorkerToZoneDistance;
            }

            if (GetAlarmDistance(arrDatas, ref nIndex, dicWorkerToEquipDistance))
            {
                foreach (KeyValuePair<string, float> pair in dicWorkerToEquipDistance)
                {
                    FormMain.Instance.DataMgr.SetWorkerToEquipDistance(pair.Key, pair.Value);
                }
                //FormMain.Instance.DataMgr.WorkerToEquipDistance = fWorkerToEquipDistance;
            }

            if (GetAlarmDistance(arrDatas, ref nIndex, out fCoGasTolerance))
                FormMain.Instance.DataMgr.COGasTolerance = fCoGasTolerance;

            if (GetAlarmDistance(arrDatas, ref nIndex, out fMethaneTolerance))
                FormMain.Instance.DataMgr.MethaneTolerance = fMethaneTolerance;
        }

        private static bool GetAlarmDistance(ArrayList arrDatas, ref int nIndex, Dictionary<string, float> dicAlarmDistance)
        {
            int nDataCount = arrDatas.Count;

            if (nIndex >= nDataCount)
                return false;

            try
            {
                bool isChanged = (bool)arrDatas[nIndex++];

                if (isChanged)
                {
                    while (nIndex < nDataCount)
                    {
                        if (arrDatas[nIndex].GetType() == typeof(bool))
                            break;

                        string strItemName = (string)arrDatas[nIndex++];
                        float fDistance = (float)arrDatas[nIndex++];
                        dicAlarmDistance[strItemName] = fDistance;
                    }

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        private static bool GetAlarmDistance(ArrayList arrDatas, ref int nIndex, out float fDistance)
        {
            fDistance = 0.0f;
            int nDataCount = arrDatas.Count;

            if (nIndex >= nDataCount)
                return false;

            try
            {
                bool isChanged = (bool)arrDatas[nIndex++];

                if (isChanged)
                {
                    fDistance = (float)arrDatas[nIndex++];
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        // Return 값 : Last Index
        private static int ProcessUpdate(ArrayList arrDatas, int nIndex)
        {
            int nEquipID = (int)arrDatas[nIndex++];
            string strEquipCode = (string)arrDatas[nIndex++];
            int nSiteID = (int)arrDatas[nIndex++];
            bool isDetect = (bool)arrDatas[nIndex];

            DataManager dataMgr = FormMain.Instance.DataMgr;
            DataEquip equip = dataMgr.GetEquipFromID(nEquipID);

            if (equip != null)
            {
                equip.Code = strEquipCode;
                equip.SiteID = nSiteID;
                equip.SensorDetect = isDetect;
            }

            return nIndex;
        }
    }
}
