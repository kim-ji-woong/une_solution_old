using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using System.Collections;
using UnE.PSM;

namespace SDMSServer
{
    public class PSMManager
    {
        // UnE.Alarm.AlarmType으로 대체
        // [2018/02/06] 김지웅
        /*public enum HistoryDataType
        {
            CLEAR_PSM_ALARM = 20,
            PSM_ALARM_1 = 21,
            PSM_ALARM_2 = 22,
            PSM_ALARM_3 = 23,
        }*/
        
        private static PSMManager m_instance = null;

        private Dictionary<int, PSMTank> m_dicTanks = new Dictionary<int, PSMTank>();
        private Dictionary<int, PSMMaterial> m_dicMaterials = new Dictionary<int, PSMMaterial>();
        private Dictionary<int, PSMSensor> m_dicSensors = new Dictionary<int, PSMSensor>();

        public static PSMManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new PSMManager();

                return m_instance;
            }
        }

        private PSMManager()
        {
        }

        public bool Load(WebDBManager dbMgr)
        {
            if (!LoadMaterials(dbMgr))
                return false;

            if (!LoadTanks(dbMgr))
                return false;

            if (!LoadSensors(dbMgr))
                return false;

            System.Threading.Thread t = new System.Threading.Thread(SensorStatusThread);
            t.Start();

            return true;
        }

        protected static void SensorStatusThread()
        {
            while (!NetworkServer.Instance.FinishProcess)
            {
                PSMManager.Instance.CheckSensorStatus();
                System.Threading.Thread.Sleep(1000);
            }
        }

        public void CheckSensorStatus()
        {
            DateTime dtNow = DateTime.Now;

            foreach (KeyValuePair<int, PSMSensor> pair in m_dicSensors)
            {
                if (pair.Value.SensorStatus == PSMSensor.Status.Off4Work)
                {
                    // 현재 시간이 작업중 시간일 경우
                    if (pair.Value.BeginWorkTime != null && pair.Value.EndWorkTime != null &&
                        dtNow >= pair.Value.BeginWorkTime.Data && dtNow <= pair.Value.EndWorkTime.Data)
                    {
                        // sensor와 관련된 알람을 해제한다.
                        ClientDataSDMS.ClearPSMSensorAlarm(pair.Value, -1);
                    }
                    // 현재 시간이 작업전 시간일 경우
                    else if (pair.Value.BeginWorkTime != null && pair.Value.EndWorkTime != null &&
                        dtNow <= pair.Value.EndWorkTime.Data)
                        continue;
                    else
                    {
                        // 작업시간이 지났으니 On 상태로 돌려놓는다.
                        pair.Value.SensorStatus = PSMSensor.Status.On;
                        ClientDataSDMS.ClearPSMSensorAlarmNChangeStatusDB(pair.Value, -1, true, true);
                    }
                }
            }
        }

        private bool LoadMaterials(WebDBManager dbMgr)
        {
            m_dicMaterials.Clear();

            string strSQL = "Select ID, MaterialName, UOM from PSMMaterial";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strMaterialName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strUOM = WebDBManager.GetStringField(arrResult[i + 2]);

                if (strMaterialName == null || strMaterialName.Length == 0)
                    continue;

                if (strUOM == null)
                    strUOM = "";

                PSMMaterial material = new PSMMaterial();
                material.ID = nID;
                material.Name = strMaterialName;
                material.UOM = strUOM;

                m_dicMaterials[nID] = material;
            }

            return true;
        }

        private bool LoadTanks(WebDBManager dbMgr)
        {
            m_dicTanks.Clear();

            string strSQL = "Select ID, TankName, EquipZoneID, Boundary, MaterialType, Capacity, Remains, UnitName, LocationName, BroadcastName from PSMTank";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            PSMMaterial material;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-9;i+=10)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTankName = WebDBManager.GetStringField(arrResult[i + 1]);
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                string strBounary = WebDBManager.GetStringField(arrResult[i + 3]);
                int nMaterialType = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                float fCapacity = WebDBManager.GetFloatField(arrResult[i + 5].ToString(), -1.0f);
                float fRemains = WebDBManager.GetFloatField(arrResult[i + 6].ToString(), -1.0f);
                string strUnitName = WebDBManager.GetStringField(arrResult[i + 7]);
                string strLocationName = WebDBManager.GetStringField(arrResult[i + 8]);
                string strBroadcastName = WebDBManager.GetStringField(arrResult[i + 9]);
                //int nEvacInitDistance = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);
                //int nEvacDayDistance = WebDBManager.GetIntField(arrResult[i + 11].ToString(), -1);
                //int nEvacNightDistance = WebDBManager.GetIntField(arrResult[i + 12].ToString(), -1);

                if (strTankName == null || strUnitName == null || strLocationName == null || strBroadcastName == null)
                    continue;

                if (strBounary == null)
                    strBounary = "";

                if (!m_dicMaterials.TryGetValue(nMaterialType, out material))
                    continue;

                PSMTank tank = new PSMTank();

                tank.ID = nID;
                tank.Name = strTankName;
                tank.EquipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                tank.Boundaries = strBounary;
                tank.Material = material;
                tank.Capacity = fCapacity < 0 ? null : new VariousData<float>(fCapacity);
                tank.Remains = fRemains < 0 ? null : new VariousData<float>(fRemains);
                tank.UnitName = strUnitName;
                tank.LocationName = strLocationName;
                tank.BroadcastName = strBroadcastName;
                //tank.EvacInitDistance = nEvacInitDistance;
                //tank.EvacDayDistance = nEvacDayDistance;
                //tank.EvacNightDistance = nEvacNightDistance;

                m_dicTanks[nID] = tank;
            }

            return true;
        }

        private bool LoadSensors(WebDBManager dbMgr)
        {
            m_dicSensors.Clear();

            string strSQL = "Select ps.ID, ps.SensorName, ps.X, ps.Y, ps.CurrentData, ps.LimitLevel1, ps.LimitLevel2, ps.LimitLevel3, ps.TankIDList, ps.EquipZoneID, pss.Status, pss.BeginTime, pss.EndTime, ps.DefLimitLevel1, ps.DefLimitLevel2, ps.DefLimitLevel3, ps.AllowReceiveLevel1Alarm, ps.AllowReceiveLevel2Alarm, ps.AllowReceiveLevel3Alarm from PSMSensor as ps, PSMSensorSchedule as pss where ps.ID = pss.SensorID";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 18;i+=19 )
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strSensorName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<float> x = WebDBManager.GetFloatField(arrResult[i + 2].ToString());
                VariousData<float> y = WebDBManager.GetFloatField(arrResult[i + 3].ToString());
                float fCurrentData = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), -1.0f);
                float fLimitLevel1 = WebDBManager.GetFloatField(arrResult[i + 5].ToString(), -1.0f);
                float fLimitLevel2 = WebDBManager.GetFloatField(arrResult[i + 6].ToString(), -1.0f);
                float fLimitLevel3 = WebDBManager.GetFloatField(arrResult[i + 7].ToString(), -1.0f);
                string strTankIDList = WebDBManager.GetStringField(arrResult[i + 8]);
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 9].ToString(), -1);
                int nStatus = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);
                VariousData<DateTime> dtBegin = WebDBManager.GetDateTimeField(arrResult[i + 11]);
                VariousData<DateTime> dtEnd = WebDBManager.GetDateTimeField(arrResult[i + 12]);
                float fDefLimitLevel1 = WebDBManager.GetFloatField(arrResult[i + 13].ToString(), -1);
                float fDefLimitLevel2 = WebDBManager.GetFloatField(arrResult[i + 14].ToString(), -1);
                float fDefLimitLevel3 = WebDBManager.GetFloatField(arrResult[i + 15].ToString(), -1);
                bool allowReceiveLevel1Alarm = WebDBManager.GetIntField(arrResult[i + 16].ToString(), 0) == 0 ? false : true;
                bool allowReceiveLevel2Alarm = WebDBManager.GetIntField(arrResult[i + 17].ToString(), 0) == 0 ? false : true;
                bool allowReceiveLevel3Alarm = WebDBManager.GetIntField(arrResult[i + 18].ToString(), 0) == 0 ? false : true;

                if (strSensorName == null)
                    strSensorName = "";

                PSMSensor sensor = new PSMSensor();

                sensor.ID = nID;
                sensor.Name = strSensorName;

                if (x != null && y != null)
                    sensor.Position = new UnE.Geometry.Vertex2D(x.Data, y.Data);

                sensor.CurrentData = fCurrentData;
                sensor.LimitLevel1 = fLimitLevel1;
                sensor.LimitLevel2 = fLimitLevel2;
                sensor.LimitLevel3 = fLimitLevel3;
                sensor.DefLimitLevel1 = fDefLimitLevel1;
                sensor.DefLimitLevel2 = fDefLimitLevel2;
                sensor.DefLimitLevel3 = fDefLimitLevel3;
                sensor.AllowReceiveLevel1Alarm = allowReceiveLevel1Alarm;
                sensor.AllowReceiveLevel2Alarm = allowReceiveLevel2Alarm;
                sensor.AllowReceiveLevel3Alarm = allowReceiveLevel3Alarm;
                sensor.EquipZoneID = nEquipZoneID;

                sensor.SensorStatus = PSMSensor.ToStatus(nStatus);
                sensor.BeginWorkTime = dtBegin;
                sensor.EndWorkTime = dtEnd;

                AddTanks(sensor, strTankIDList);

                m_dicSensors[nID] = sensor;
            }

            return true;
        }

        private void AddTanks(PSMSensor sensor, string strTankIDList)
        {
            if (strTankIDList == null)
                return;

            int nID;
            PSMTank tank;
            string[] tokens = strTankIDList.Split(',');

            foreach (string strID in tokens)
            {
                if (!int.TryParse(strID.Trim(), out nID))
                    continue;

                if (!m_dicTanks.TryGetValue(nID, out tank))
                    continue;

                sensor.AddTank(tank);
            }
        }

        public PSMSensor GetSensor(int nSensorID)
        {
            PSMSensor sensor;

            if (m_dicSensors.TryGetValue(nSensorID, out sensor))
                return sensor;

            return null;
        }

        public PSMTank GetTank(int nTankID)
        {
            PSMTank tank;

            if (m_dicTanks.TryGetValue(nTankID, out tank))
                return tank;

            return null;
        }

        public PSMMaterial GetMaterial(int nMaterialID)
        {
            PSMMaterial material;

            if (m_dicMaterials.TryGetValue(nMaterialID, out material))
                return material;

            return null;
        }

        public bool ProcessSensorData(int nSensorTagInfoID, int nSensorZoneID, int nAlarmDepth, bool bTest , out int nSensorZoneHistoryID, out SensorZone sensorZone, out int nPrevSensorHistoryID, out int nSensorData)
        {
            nSensorZoneHistoryID = -1;
            nPrevSensorHistoryID = -1;
            nSensorData = -1;

            sensorZone = NetworkServer.Instance.IOManager.GetSensorZone(nSensorZoneID);

            if (sensorZone == null)
                return false;

            PSMSensor sensor = null;

            if (!m_dicSensors.TryGetValue(sensorZone.LinkedSensorID, out sensor))
                return false;

            if (PSMManager.IsOff(sensor))
            {
                OffSensorZoneGroupSensor(sensorZone);
                return false;
            }

            if (nAlarmDepth == 1)   // 1단계 알람
            {
                /*if (sensor.CurrentData == sensor.LimitLevel1)
                    return true;
                else*/
                {
                    if (!UpdateSensorData(sensor, sensor.LimitLevel1))
                        return false;
                }
            }
            else if (nAlarmDepth == 2)  // 2단계 알람
            {
                /*if (sensor.CurrentData == sensor.LimitLevel2)
                    return true;
                else*/
                {
                    if (!UpdateSensorData(sensor, sensor.LimitLevel2))
                        return false;
                }
            }
            else if (nAlarmDepth == 3)  // 3단계 알람
            {
                /*if (sensor.CurrentData == sensor.LimitLevel3)
                    return true;
                else*/
                {
                    if (!UpdateSensorData(sensor, sensor.LimitLevel3))
                        return false;
                }
            }
            else if (nAlarmDepth == 0)  // 알람 해제
            {
                /*if (sensor.CurrentData < sensor.LimitLevel1)
                    return true;
                else*/
                {
                    if (!UpdateSensorData(sensor, 0.0f))
                        return false;
                }
            }
            else
                return false;

            bool connected;

            nSensorZoneHistoryID = SensorManager.Instance.ProcessSensorData(sensorZone.Type, nSensorTagInfoID, sensorZone.ID, (int)UnE.Alarm.AlarmType.PSM_ALARM_1 - 1 + nAlarmDepth, out nSensorZoneID, out nSensorData, out connected, ref nPrevSensorHistoryID);
            //nSensorZoneHistoryID = SensorManager.Instance.ProcessSensorData(sensorZone.Type, sensorZone.ID, (int)HistoryDataType.CLEAR_PSM_ALARM + nAlarmDepth, out nSensorZoneID, out nSensorData, out connected, ref nPrevSensorHistoryID);

            // sensorZone이 아니라 같은 SensorZoneGroup 내에 있는 다른 Sensor를 이용한 SensorZoneHistory가 존재할 경우
            if (nSensorZoneID >= 0 && nSensorZoneID != sensorZone.ID)
            {
                sensorZone = NetworkServer.Instance.IOManager.GetSensorZone(nSensorZoneID);
            }

            return true;
        }

        private void OffSensorZoneGroupSensor(SensorZone sensorZone)
        {
            SensorZoneGroup group = NetworkServer.Instance.IOManager.GetSensorZoneGroup(sensorZone.EquipZone, sensorZone.Type);

            // Off 상태인 Sensor의 Data는 null로 만든다.
            group.SensorDatas[sensorZone] = null;
        }

        private bool UpdateSensorData(PSMSensor sensor, float fSensorData)
        {
            /*if (sensor == null || sensor.ID < 0)
                return false;

            string strSQL = string.Format("update PSMSensor set CurrentData = {0} where ID = {1}", fSensorData, sensor.ID);

            if (NetworkServer.Instance.DBManager.GetResultData(strSQL, 0) == null)
                return false;

            sensor.CurrentData = fSensorData;*/
            return true;
        }

        // 센서가 사용중지 상태인가?
        public static bool IsOff(PSMSensor sensor, DateTime time)
        {
            if (sensor.SensorStatus == PSMSensor.Status.Off)
                return true;

            if (sensor.SensorStatus == PSMSensor.Status.Off4Work)
            {
                if (sensor.BeginWorkTime != null && sensor.EndWorkTime != null)
                {
                    if (time >= sensor.BeginWorkTime.Data && time <= sensor.EndWorkTime.Data)
                        return true;
                }
            }

            return false;
        }

        // 센서가 사용중지 상태인가?
        public static bool IsOff(PSMSensor sensor)
        {
            return IsOff(sensor, DateTime.Now);
        }
    }
}
