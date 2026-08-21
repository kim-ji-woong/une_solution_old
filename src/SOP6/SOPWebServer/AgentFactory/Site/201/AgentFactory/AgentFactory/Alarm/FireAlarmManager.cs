using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace AgentFactory.Alarm
{
    public static class FireAlarmManager
    {
        public static List<ClientMessage> CheckNewAlarm(DirectDBManager dbMgr, AlarmData alarm, List<AlarmData> alarms)
        {
            string strSensorZoneIDs = "";

            for (int i=alarms.Count-1;i>=0;i--)
            {
                AlarmData prevAlarm = alarms[i];

                if (prevAlarm.SensorType != UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR)
                {
                    alarms.RemoveAt(i);
                }
                else
                {
                    if (strSensorZoneIDs.Length == 0)
                        strSensorZoneIDs = prevAlarm.SensorZoneID.ToString();
                    else
                        strSensorZoneIDs += "," + prevAlarm.SensorZoneID.ToString();
                }
            }

            List<ClientMessage> messages = new List<ClientMessage>();

            if (strSensorZoneIDs.Length == 0)
            {
                ClientMessage message = AlarmManager.MakeClientMessage(alarm, AlarmManager.AlarmStep.Step2, SOPWebServer.ClientType.SDMS, -1);
                messages.Add(message);
            }
            else
            {
                strSensorZoneIDs += "," + alarm.SensorZoneID.ToString();
                alarms.Add(alarm);

                AlarmManager.AlarmStep step = GetAlarmStep(dbMgr, strSensorZoneIDs, alarms, alarm.SensorZoneID);

                if (step == AlarmManager.AlarmStep.None)
                    return null;

                ClientMessage message = AlarmManager.MakeClientMessage(alarms, step, SOPWebServer.ClientType.SDMS, -1);
                messages.Add(message);
            }

            return messages;
        }

        public static List<ClientMessage> CheckClearAlarm(DirectDBManager dbMgr, AlarmData alarm, List<AlarmData> alarms)
        {
            string strSensorZoneIDs = "";

            for (int i = alarms.Count - 1; i >= 0; i--)
            {
                AlarmData prevAlarm = alarms[i];

                if (prevAlarm.SensorZoneID == alarm.SensorZoneID)
                {
                    alarms.RemoveAt(i);
                    continue;
                }

                if (prevAlarm.SensorType != UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR)
                {
                    alarms.RemoveAt(i);
                }
                else
                {
                    if (strSensorZoneIDs.Length == 0)
                        strSensorZoneIDs = prevAlarm.SensorZoneID.ToString();
                    else
                        strSensorZoneIDs += "," + prevAlarm.SensorZoneID.ToString();
                }
            }

            List<ClientMessage> messages = null;

            if (strSensorZoneIDs.Length == 0)
            {
                // 하나밖에 없는 알람이 제거되므로 추가로 할일은 없다.
            }
            else
            {
                AlarmManager.AlarmStep step = GetAlarmStep(dbMgr, strSensorZoneIDs, alarms, -1);

                if (step != AlarmManager.AlarmStep.None)
                {
                    messages = new List<ClientMessage>();
                    ClientMessage message = AlarmManager.MakeClientMessage(alarms, step, SOPWebServer.ClientType.SDMS, -1);
                    messages.Add(message);
                }
            }

            AlarmManager.RemoveAlarmStep(alarm);
            return messages;
        }

        public static AlarmManager.AlarmStep GetAlarmStep(DirectDBManager dbMgr, string strSensorZoneIDs, List<AlarmData> alarms, int nSensorZoneID)
        {
            if (dbMgr.Connect() == false)
                return AlarmManager.AlarmStep.None;

            string strSQL = "Select sz.ID, sz.Zone, z.FloorIndex, z.BuildingID from SensorZone as sz, Zone as z where sz.Zone = z.ID and sz.ID in (" + strSensorZoneIDs + ")";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            //dbMgr.Close();

            if (arrResult == null)
            {
                return AlarmManager.AlarmStep.None;
            }

            // Key : Zone ID
            // Value : 해당 Zone에서 발생한 Sensor 신호 개수
            Dictionary<int, int> dicZoneCount = new Dictionary<int, int>();
            List<int> floorIndexList = new List<int>();

            int nSensorCount;
            int nBuildingID = -1;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-3;i+=4)
            {
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> zoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> floorIndex = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> buildingID = WebDBManager.GetIntField(arrResult[i + 3].ToString());

                if (sensorZoneID == null || zoneID == null || floorIndex == null || buildingID == null)
                    continue;

                if (nSensorZoneID < 0)
                    nSensorZoneID = sensorZoneID.Data;

                if (sensorZoneID.Data == nSensorZoneID)
                {
                    nBuildingID = buildingID.Data;
                    break;
                }
            }

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> zoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> floorIndex = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> buildingID = WebDBManager.GetIntField(arrResult[i + 3].ToString());

                if (sensorZoneID == null || zoneID == null || floorIndex == null || buildingID == null)
                    continue;

                if (buildingID.Data != nBuildingID)
                {
                    // 다른 Building에 있는 Alarm들은 무시한다.
                    foreach (AlarmData alarm in alarms)
                    {
                        if (alarm.SensorZoneID == sensorZoneID.Data)
                        {
                            alarms.Remove(alarm);
                            break;
                        }
                    }

                    continue;
                }

                if (floorIndexList.Contains(floorIndex.Data) == false)
                    floorIndexList.Add(floorIndex.Data);

                if (dicZoneCount.TryGetValue(zoneID.Data, out nSensorCount))
                    dicZoneCount[zoneID.Data] = nSensorCount + 1;
                else
                    dicZoneCount[zoneID.Data] = 1;
            }

            int nZoneCount = dicZoneCount.Count;
            bool multiSensorZone = false;

            foreach (KeyValuePair<int, int> pair in dicZoneCount)
            {
                if (pair.Value > 1)
                {
                    multiSensorZone = true;
                    break;
                }
            }

            bool linear = false;
            int nFloorCount = floorIndexList.Count;

            if (nFloorCount > 0)
            {
                floorIndexList.Sort();
                int nPrev = floorIndexList[0];

                for (int i=1;i<nFloorCount;i++)
                {
                    int nCurrent = floorIndexList[i];

                    if (nPrev + 1 == nCurrent)
                    {
                        linear = true;
                        break;
                    }

                    nPrev = nCurrent;
                }
            }

            if (linear)
                return AlarmManager.AlarmStep.Step4;
            else if (multiSensorZone)
                return AlarmManager.AlarmStep.Step3;

            return AlarmManager.AlarmStep.Step2;
        }
    }
}
