using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.PSM;
using System.Collections;
using DBUtility2;
using UnE.Spatial;

namespace SDMS
{
    public class PSMManager
    {
        private class SensorSchedule
        {
            private int m_nSensorID = -1;
            private int m_nStatus = -1;
            private VariousData<DateTime> m_nBeginTime = null;
            private VariousData<DateTime> m_nEndTime = null;

            public int SensorID
            {
                get { return m_nSensorID; }
                set { m_nSensorID = value; }
            }

            public int Status
            {
                get { return m_nStatus; }
                set { m_nStatus = value; }
            }

            public VariousData<DateTime> BeginTime
            {
                get { return m_nBeginTime; }
                set { m_nBeginTime = value; }
            }

            public VariousData<DateTime> EndTime
            {
                get { return m_nEndTime; }
                set { m_nEndTime = value; }
            }
        }

        private class SensorValuesData : IComparable
        {
            private ArrayList m_arrDatas = null;
            private DateTime m_timeStamp = new DateTime();
            private int m_nTableIndex = -1;

            public ArrayList Datas
            {
                get { return m_arrDatas; }
                set { m_arrDatas = value; }
            }

            public DateTime TimeStamp
            {
                get { return m_timeStamp; }
                set { m_timeStamp = value; }
            }

            public int TableIndex
            {
                get { return m_nTableIndex; }
                set { m_nTableIndex = value; }
            }

            public int CompareTo(object obj)
            {
                SensorValuesData data = (SensorValuesData)obj;
                return this.TimeStamp.CompareTo(data.TimeStamp);
            }
        }

        private static PSMManager m_instance = null;

        public static PSMManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new PSMManager();

                return m_instance;
            }
        }

        // Key : Material ID
        private Dictionary<int, PSMMaterial> m_dicPSMMaterials = null;
        // Key : Sensor ID
        private Dictionary<int, PSMSensor> m_dicPSMSensor = null;
        // Key : Tank ID
        private Dictionary<int, PSMTank> m_dicPSMTank = null;
        // Key : Sensor Value Index Number
        private Dictionary<int, Dictionary<DateTime, Dictionary<DateTime, double>>> m_dicPSMSensorValue = null;
        private Dictionary<int, DateTime> m_dicPSMSensorValueLastLoadedDate = null;
        private Dictionary<string, PSMSensorType> m_dicPSMSensorType = null;

        private DateTime m_dtLoadedSensorValue = new DateTime(2000, 1, 1);


        private PSMManager()
        {
            m_dicPSMSensorValue = new Dictionary<int, Dictionary<DateTime, Dictionary<DateTime, double>>>();
            m_dicPSMSensorValueLastLoadedDate = new Dictionary<int, DateTime>();
            //LoadSensorValueData();

            //System.Threading.Thread thread = new System.Threading.Thread(LoadSensorValueData);
            //thread.Start();
        }


        public PSMMaterial GetMaterial(int nMaterialID)
        {
            if (m_dicPSMMaterials == null)
                m_dicPSMMaterials = ReadPSMMaterials();

            if (m_dicPSMMaterials == null)
                return null;

            PSMMaterial material = null;
            m_dicPSMMaterials.TryGetValue(nMaterialID, out material);
            return material;
        }

        public void AddMaterial(PSMMaterial material)
        {
            if (m_dicPSMMaterials == null)
                m_dicPSMMaterials = ReadPSMMaterials();

            if (m_dicPSMMaterials == null)
                return;

            m_dicPSMMaterials[material.ID] = material;
        }

        public PSMTank GetTank(int nTankID)
        {
            if (m_dicPSMTank == null)
                m_dicPSMTank = ReadPSMTanks();

            if (m_dicPSMTank == null)
                return null;

            PSMTank tank = null;
            m_dicPSMTank.TryGetValue(nTankID, out tank);
            return tank;
        }

        public void AddTank(PSMTank tank)
        {
            if (m_dicPSMTank == null)
                m_dicPSMTank = ReadPSMTanks();

            if (m_dicPSMTank == null)
                return;

            m_dicPSMTank[tank.ID] = tank;
        }

        public PSMSensor GetSensor(int nSensorID)
        {
            if (m_dicPSMSensor == null)
                m_dicPSMSensor = ReadPSMSensors();

            if (m_dicPSMSensor == null)
                return null;

            PSMSensor sensor = null;
            m_dicPSMSensor.TryGetValue(nSensorID, out sensor);
            return sensor;
        }

        public void AddSensor(PSMSensor sensor)
        {
            if (m_dicPSMSensor == null)
                m_dicPSMSensor = ReadPSMSensors();

            if (m_dicPSMSensor == null)
                return;

            m_dicPSMSensor[sensor.ID] = sensor;
        }


        public List<PSMSensor> GetSensorByBuilding(int nBuildingID)
        {
            if (m_dicPSMSensor == null)
                m_dicPSMSensor = ReadPSMSensors();

            if (m_dicPSMSensor == null)
                return null;

            List<PSMSensor> liReturn = new List<PSMSensor>();
            bool isAdded = false;

            foreach (PSMSensor sensor in m_dicPSMSensor.Values)
            {
                isAdded = false;

                if (nBuildingID == -1)
                {
                    liReturn.Add(sensor);
                }
                else
                {
                    foreach (PSMTank tank in sensor.LinkedTankList)
                    {
                        if (isAdded == true)
                            break;

                        foreach (UnE.Spatial.Zone zone in tank.EquipZone.LinkedZoneList)
                        {
                            if (zone.Building.ID == nBuildingID)
                            {
                                liReturn.Add(sensor);
                                isAdded = true;
                                break;
                            }
                        }
                    }
                }
            }

            return liReturn;
        }

        public List<PSMTank> GetTanks()
        {
            if (m_dicPSMTank == null)
                m_dicPSMTank = ReadPSMTanks();

            if (m_dicPSMTank == null)
                return null;

            return m_dicPSMTank.Values.ToList<PSMTank>();
        }

        public List<PSMSensor> GetSensors()
        {
            if (m_dicPSMSensor == null)
                m_dicPSMSensor = ReadPSMSensors();

            if (m_dicPSMSensor == null)
                return null;

            return m_dicPSMSensor.Values.ToList<PSMSensor>();
        }

        public List<UnE.Spatial.Building> GetTankBuildings()
        {
            if (m_dicPSMTank == null)
                m_dicPSMTank = ReadPSMTanks();

            if (m_dicPSMTank == null)
                return null;

            List<UnE.Spatial.Building> liReturn = new List<UnE.Spatial.Building>();

            foreach (PSMTank tank in m_dicPSMTank.Values)
            {
                if (liReturn.Contains(tank.EquipZone.LinkedZone.Building) == false)
                    liReturn.Add(tank.EquipZone.LinkedZone.Building);
                
            }

            return liReturn;
        }

        private bool ShowDeletedPSMSensorValues()
        {
            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'ShowDeletedPSMSensorLog' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            VariousData<int> nOption = WebDBManager.GetIntField(arrResult[0].ToString());

            if (nOption != null && nOption.Data == 1)
                return true;

            return false;
        }

        private ArrayList GetPSMSensorValues(string strSelect, string strWhere, int nTimeValueIndex)
        {
            //DateTime time1 = DateTime.Now;
            if (nTimeValueIndex < 0)
                return null;

            // 시간순서대로 정렬하기 위하여 SensorValuesData 클래스를 사용한다.
            List<SensorValuesData> datas = new List<SensorValuesData>();
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            for (int i=1;i<=10;i++)
            {
                string strSQL = strSelect + " from PSMSensorValue" + i.ToString() + " " + strWhere;
                ArrayList arrResult = dbMgr.GetResultData(strSQL);

                if (arrResult == null)
                    return null;
                else if (arrResult.Count <= nTimeValueIndex)
                    continue;

                VariousData<DateTime> time = WebDBManager.GetDateTimeField(arrResult[nTimeValueIndex]);

                if (time == null)
                    return null;

                SensorValuesData data = new SensorValuesData();
                data.TimeStamp = time.Data;
                data.Datas = arrResult;
                data.TableIndex = i;

                datas.Add(data);
            }

            datas.Sort();
            ArrayList results = new ArrayList();

            foreach (SensorValuesData data in datas)
            {
                results.AddRange(data.Datas);
            }

            //DateTime time2 = DateTime.Now;
            //WriteLog("Value 쪼개기", time1, time2);
            return results;
        }

        /*private void WriteLog(string strLog, DateTime time1, DateTime time2)
        {
            TimeSpan span = time2 - time1;
            System.Diagnostics.Trace.WriteLine(strLog + " : " + span.TotalSeconds + "초");
        }*/

        public Dictionary<DateTime, Dictionary<DateTime, double>> GetSensorData(ref PSMSensor sensor)
        {
            // 한달 전 데이터부터 로드
            m_dtLoadedSensorValue = DateTime.Now.AddMonths(-1);
            bool showDeletedLog = ShowDeletedPSMSensorValues();

            sensor = GetSensor(sensor.ID);

            Dictionary<DateTime, Dictionary<DateTime, double>> dicReturn = null;

            if (m_dicPSMSensorValueLastLoadedDate.ContainsKey(sensor.SensorValueIndex) == false)
            {
                m_dicPSMSensorValueLastLoadedDate.Add(sensor.SensorValueIndex, new DateTime(m_dtLoadedSensorValue.Ticks));
                m_dicPSMSensorValue.Add(sensor.SensorValueIndex, new Dictionary<DateTime, Dictionary<DateTime, double>>());
            }

            ArrayList arrResult = GetPSMSensorValues(string.Format("SELECT ValueTime, SensorValue{0}",  sensor.SensorValueIndex), string.Format("WHERE ValueTime > '{0}' ORDER BY ValueTime ASC", m_dicPSMSensorValueLastLoadedDate[sensor.SensorValueIndex].ToString("yyyy-MM-dd HH:mm:ss")), 0);

            //string strSQL = String.Format("SELECT ValueTime, SensorValue{0} FROM PSMSensorValues WHERE ValueTime > CONVERT(DATETIME, '{1}') ORDER BY ValueTime ASC", sensor.SensorValueIndex, m_dicPSMSensorValueLastLoadedDate[sensor.SensorValueIndex].ToString("yyyy-MM-dd HH:mm:ss"));
            //WebDBManager dbMgr = FormMain.Instance.DBManager;

            //ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return dicReturn;

            for (int i = 0; i < arrResult.Count - 1; i += 2)
            {
                DateTime dtDateTime = WebDBManager.GetDateTimeField(arrResult[i].ToString()).Data;
                double dData = Convert.ToDouble(WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0));

                DateTime dtDate = new DateTime(dtDateTime.Year, dtDateTime.Month, dtDateTime.Day);

                m_dicPSMSensorValueLastLoadedDate[sensor.SensorValueIndex] = dtDateTime;

                Dictionary<DateTime, Dictionary<DateTime, double>> dicPSMSensorValue = m_dicPSMSensorValue[sensor.SensorValueIndex];
                Dictionary<DateTime, double> dicValues = null;

                if (dicPSMSensorValue.TryGetValue(dtDate, out dicValues) == false)
                {
                    dicValues = new Dictionary<DateTime, double>();
                    dicPSMSensorValue[dtDate] = dicValues;
                }
                /*if (m_dicPSMSensorValue[sensor.SensorValueIndex].ContainsKey(dtDate) == false)
                    m_dicPSMSensorValue[sensor.SensorValueIndex].Add(dtDate, new Dictionary<DateTime, double>());*/

                if (dicValues.ContainsKey(dtDateTime) == true)
                    continue;
                /*if (m_dicPSMSensorValue[sensor.SensorValueIndex][dtDate].ContainsKey(dtDateTime) == true)
                    continue;*/

                if (dData < 0)
                {
                    if (showDeletedLog)
                        dData = -dData;
                    else
                        dData = 0;
                }

                // 모든 물질의 측정 데이터는 최대값이 존재하고 그 이상 넘어가지 않도록 데이터 수정
                switch (GetMaterial(sensor.MaterialType).Name)
                {
                    case "염산":
                        if (dData > 10)
                            dData = 10;
                        break;
                    case "가성소다":
                        if (dData > 1)
                            dData = 1;
                        break;
                    default:
                        if (dData > 100)
                            dData = 100;
                        break;
                }

                dicValues[dtDateTime] = dData;
                //m_dicPSMSensorValue[sensor.SensorValueIndex][dtDate].Add(dtDateTime, dData);
            }

            RemoveSensorValueData(sensor.SensorValueIndex);

            dicReturn = m_dicPSMSensorValue[sensor.SensorValueIndex];

            return dicReturn;

            //Dictionary<DateTime, double> dicReturn = new Dictionary<DateTime, double>();

            //string strSQL = String.Format("SELECT ValueTime, SensorValue{0} FROM PSMSensorValues ORDER BY ValueTime ASC", sensor.SensorValueIndex);
            //WebDBManager dbMgr = FormMain.Instance.DBManager;

            //ArrayList arrResult = dbMgr.GetResultData(strSQL);

            //if (arrResult == null)
            //    return null;

            //for (int i = 0; i < arrResult.Count - 1; i += 2)
            //{
            //    double dData = Convert.ToDouble(WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0));

            //    if (dData < 0)
            //        dData = 0;

            //    // 모든 물질의 측정 데이터는 최대값이 존재하고 그 이상 넘어가지 않도록 데이터 수정
            //    switch (GetMaterial(sensor.MaterialType).Name)
            //    {
            //        case "염산":
            //            if (dData > 10)
            //                dData = 10;
            //            break;
            //        case "가성소다":
            //            if (dData > 1)
            //                dData = 1;
            //            break;
            //        default:
            //            if (dData > 100)
            //                dData = 100;
            //            break;
            //    }

            //    dicReturn.Add(WebDBManager.GetDateTimeField(arrResult[i].ToString()).Data, dData);
            //}

            //return dicReturn;

            //LoadSensorValueData();

            //return m_dicPSMSensorValue[sensor.SensorValueIndex];
        }

        public Dictionary<DateTime, double> GetSensorData(PSMSensor sensor, DateTime dtStart, DateTime dtEnd)
        {
            Dictionary<DateTime, double> dicReturn = null;
            Dictionary<DateTime, Dictionary<DateTime, double>> dicOrignData = GetSensorData(ref sensor);
            if (dicOrignData == null)
                return dicReturn;

            dicReturn = new Dictionary<DateTime, double>();

            DateTime dtStrDate = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day);
            DateTime dtEndDate = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day);

            foreach (Dictionary<DateTime, double> data in from datas in dicOrignData
                                                          where datas.Key >= dtStrDate && datas.Key <= dtEndDate
                                                          orderby datas.Key ascending
                                                          select datas.Value
                                                           )
            {
                foreach (KeyValuePair<DateTime, double> pair in from datas in data
                                                                where datas.Key >= dtStart && datas.Key <= dtEnd
                                                                orderby datas.Key ascending
                                                                select datas
                                                           )
                {
                    dicReturn.Add(pair.Key, pair.Value);
                }
            }

            return dicReturn;

            //Dictionary<DateTime, double> dicReturn = new Dictionary<DateTime, double>();

            //string strSQL = String.Format("SELECT ValueTime, SensorValue{0} FROM PSMSensorValues WHERE ValueTime BETWEEN CONVERT(DATETIME, '{1}') AND CONVERT(DATETIME, '{2}') ORDER BY ValueTime ASC", sensor.SensorValueIndex, strDateStart, strDateEnd);
            //WebDBManager dbMgr = FormMain.Instance.DBManager;

            //ArrayList arrResult = dbMgr.GetResultData(strSQL);

            //if (arrResult == null)
            //    return null;

            //for (int i = 0; i < arrResult.Count - 1; i += 2)
            //{
            //    double dData = Convert.ToDouble(WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0));

            //    if (dData < 0)
            //        dData = 0;

            //    dicReturn.Add(WebDBManager.GetDateTimeField(arrResult[i].ToString()).Data, dData);
            //}

            //return dicReturn;
        }

        public int[] GetTankLocationEquipZoneID(string strLocationName)
        {
            if (m_dicPSMTank == null)
                m_dicPSMTank = ReadPSMTanks();

            if (m_dicPSMTank == null)
                return null;

            List<int> liEquipZoneID = new List<int>();

            foreach (PSMTank tank in from tanks in m_dicPSMTank.Values
                                     where tanks.LocationName == strLocationName
                                     select tanks)
            {
                if (liEquipZoneID.Contains(tank.EquipZone.ID) == false)
                    liEquipZoneID.Add(tank.EquipZone.ID);
            }

            return liEquipZoneID.ToArray();
        }

        private Dictionary<int, PSMMaterial> ReadPSMMaterials()
        {
            string strSQL = String.Format("SELECT ID, MaterialName, UOM, PageNo, EvacInitDistance, EvacDayDistance,EvacNightDistance FROM PSMMaterial");
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            Dictionary<int, PSMMaterial> dicMaterials = new Dictionary<int, PSMMaterial>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6 ; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strMaterialName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strUOM = WebDBManager.GetStringField(arrResult[i + 2]);
                int nPageNo = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

                float eInitDist = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                float eDayDist = WebDBManager.GetFloatField(arrResult[i + 5].ToString(), 0.0f);
                float eNigDist = WebDBManager.GetFloatField(arrResult[i + 6].ToString(), 0.0f);


                PSMMaterial material = new PSMMaterial();

                material.ID = nID;
                material.Name = strMaterialName;
                material.UOM = strUOM;
                material.PageNo = nPageNo;

                material.InitEvacDistance = eInitDist;
                material.DayEvacDistance = eDayDist;
                material.NightEvacDistance = eNigDist;


                dicMaterials[nID] = material;
            }

            return dicMaterials;
        }

        private Dictionary<int, PSMTank> ReadPSMTanks()
        {
            string strSQL = "SELECT t.ID, t.TankName, t.EquipZoneID, t.MaterialType, ";
            strSQL += "t.Capacity, t.Remains, t.UnitName, m.ID, t.LocationName, t.BroadcastName, t.AreaType ";
            strSQL += "FROM PSMTank AS t ";
            strSQL += "INNER JOIN PSMMaterial AS m ON (t.MaterialType = m.ID) ";

            WebDBManager dbMgr = FormMain.Instance.DBManager;
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            Dictionary<int, PSMTank> dicTank = new Dictionary<int, PSMTank>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 10 ; i += 11)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTankName = WebDBManager.GetStringField(arrResult[i + 1]);
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nMaterialType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                VariousData<float> fCapacity = WebDBManager.GetFloatField(arrResult[i + 4].ToString());
                VariousData<float> fRemains = WebDBManager.GetFloatField(arrResult[i + 5].ToString());
                string strUnitName = WebDBManager.GetStringField(arrResult[i + 6]);
                int nMaterialID = WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                string strLocationName = WebDBManager.GetStringField(arrResult[i + 8]);
                string strBroadcastName = WebDBManager.GetStringField(arrResult[i + 9]);
                int nAraeType = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);

                PSMTank tank = new PSMTank();

                tank.ID = nID;
                tank.Name = strTankName;
                tank.EquipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);
                tank.Material = GetMaterial(nMaterialID);
                tank.BroadcastName = strBroadcastName;
                tank.LocationName = strLocationName;
                tank.Capacity = fCapacity;
                tank.Remains = fRemains;
                tank.UnitName = strUnitName;
                tank.AreaType = (PSMTank.Area)nAraeType;
            
                dicTank[nID] = tank;
            }

            return dicTank;
        }

        public void ReadPSMSensorTypes()
        {
            string strSQL = "Select TypeName, LifeTimeMonth from PSMSensorType";
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return;

            // 삭제된 SensorType 리스트
            List<PSMSensorType> removeList = new List<PSMSensorType>();

            if (m_dicPSMSensorType == null)
                m_dicPSMSensorType = new Dictionary<string, PSMSensorType>();
            else
            {
                foreach (KeyValuePair<string, PSMSensorType> pair in m_dicPSMSensorType)
                {
                    removeList.Add(pair.Value);
                }
            }

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                string strTypeName = WebDBManager.GetStringField(arrResult[i]);
                VariousData<int> month = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (strTypeName == null || month == null)
                    continue;

                PSMSensorType sensorType = AddPSMSensorType(strTypeName, month.Data);

                if (removeList.Contains(sensorType))
                    removeList.Remove(sensorType);
            }

            foreach (PSMSensorType sensorType in removeList)
            {
                RemoveSensorType(sensorType.TypeName);
            }
        }

        public PSMSensorType GetPSMSensorType(string strTypeName)
        {
            if (strTypeName == null)
                return null;

            PSMSensorType sensorType = null;

            if (m_dicPSMSensorType.TryGetValue(strTypeName, out sensorType))
                return sensorType;

            return null;
        }

        private Dictionary<int, PSMSensor> ReadPSMSensors()
        {
            if (m_dicPSMSensorType == null)
                ReadPSMSensorTypes();

            string strSQL = "Select ID, TankIDList, SensorName, X, Y, CurrentData, LimitLevel1, LimitLevel2, LimitLevel3, SensorValueIdx, MaterialType, EquipZoneID, InstallDate, SensorTypeName, Department, DepartmentPhoneNumber, DefLimitLevel1, DefLimitLevel2, DefLimitLevel3, AllowReceiveLevel1Alarm, AllowReceiveLevel2Alarm, AllowReceiveLevel3Alarm from PSMSensor";

            WebDBManager dbMgr = FormMain.Instance.DBManager;

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            SensorSchedule sensorStatus = null;
            Dictionary<int, SensorSchedule> dicSensorStatus = LoadSensorStatusData();

            Dictionary<int, PSMSensor> dicSensors = new Dictionary<int, PSMSensor>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 21; i += 22)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTankIDList = WebDBManager.GetStringField(arrResult[i + 1]);
                string strSensorName = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<float> x = WebDBManager.GetFloatField(arrResult[i + 3].ToString());
                VariousData<float> y = WebDBManager.GetFloatField(arrResult[i + 4].ToString());
                float fCurrentData = WebDBManager.GetFloatField(arrResult[i + 5].ToString(), -1);
                float fLimitLevel1 = WebDBManager.GetFloatField(arrResult[i + 6].ToString(), -1);
                float fLimitLevel2 = WebDBManager.GetFloatField(arrResult[i + 7].ToString(), -1);
                float fLimitLevel3 = WebDBManager.GetFloatField(arrResult[i + 8].ToString(), -1);
                int nSensorValueIdx = WebDBManager.GetIntField(arrResult[i + 9].ToString(), -1);
                int nMaterialType = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 11].ToString(), -1);
                VariousData<DateTime> installDate = WebDBManager.GetDateTimeField(arrResult[i + 12].ToString());
                string strSensorTypeName = WebDBManager.GetStringField(arrResult[i + 13]);

                string szDepartment = WebDBManager.GetStringField(arrResult[i + 14]);
                string szDepphone = WebDBManager.GetStringField(arrResult[i + 15]);
                float fDefLimitLevel1 = WebDBManager.GetFloatField(arrResult[i + 16].ToString(), -1);
                float fDefLimitLevel2 = WebDBManager.GetFloatField(arrResult[i + 17].ToString(), -1);
                float fDefLimitLevel3 = WebDBManager.GetFloatField(arrResult[i + 18].ToString(), -1);
                bool allowReceiveLevel1Alarm = WebDBManager.GetIntField(arrResult[i + 19].ToString(), 0) == 0 ? false : true;
                bool allowReceiveLevel2Alarm = WebDBManager.GetIntField(arrResult[i + 20].ToString(), 0) == 0 ? false : true;
                bool allowReceiveLevel3Alarm = WebDBManager.GetIntField(arrResult[i + 21].ToString(), 0) == 0 ? false : true;

                PSMSensor sensor = new PSMSensor();

                sensor.ID = nID;
                sensor.Name = strSensorName;
                sensor.CurrentData = fCurrentData;
                sensor.LimitLevel1 = fLimitLevel1;
                sensor.LimitLevel2 = fLimitLevel2;
                sensor.LimitLevel3 = fLimitLevel3;
                sensor.SensorValueIndex = nSensorValueIdx;
                sensor.MaterialType = nMaterialType;
                sensor.EquipZoneID = nEquipZoneID;
                sensor.InstallDate = installDate;
                sensor.Department = szDepartment;
                sensor.PhoneNumber = szDepphone;
                sensor.DefLimitLevel1 = fDefLimitLevel1;
                sensor.DefLimitLevel2 = fDefLimitLevel2;
                sensor.DefLimitLevel3 = fDefLimitLevel3;
                sensor.AllowReceiveLevel1Alarm = allowReceiveLevel1Alarm;
                sensor.AllowReceiveLevel2Alarm = allowReceiveLevel2Alarm;
                sensor.AllowReceiveLevel3Alarm = allowReceiveLevel3Alarm;

                if (strSensorTypeName != null)
                {
                    PSMSensorType sensorType;

                    if (m_dicPSMSensorType.TryGetValue(strSensorTypeName, out sensorType))
                        sensor.SensorType = sensorType;
                }

                List<PSMTank> tankList = GetTankList(strTankIDList);

                foreach (PSMTank tank in tankList)
                {
                    sensor.AddTank(tank);
                }

                if (!dicSensorStatus.TryGetValue(nID, out sensorStatus))
                    sensorStatus = null;
                else
                {
                    sensor.SensorStatus = PSMSensor.ToStatus(sensorStatus.Status);
                    sensor.BeginWorkTime = sensorStatus.BeginTime;
                    sensor.EndWorkTime = sensorStatus.EndTime;
                }

                if (x != null && y != null && sensor != null)
                {
                    sensor.Position = new UnE.Geometry.Vertex2D(x.Data, y.Data);
                }

                dicSensors[nID] = sensor;
            }

            return dicSensors;
        }

        private List<PSMTank> GetTankList(string strTankIDList)
        {
            int nID;
            string[] ids = strTankIDList.Split(',');

            List<PSMTank> tankList = new List<PSMTank>();

            foreach (string id in ids)
            {
                if (!int.TryParse(id.Trim(), out nID))
                    continue;

                PSMTank tank = GetTank(nID);
                tankList.Add(tank);
            }

            return tankList;
        }

        private Dictionary<int, SensorSchedule> LoadSensorStatusData()
        {
            List<int> localOffSensorIDList = PopupDialog.FormPSMSensorWork.ReadLocalOffSensorIDList();

            string strSQL = "Select SensorID, Status, BeginTime, EndTime from PSMSensorSchedule";
            WebDBManager dbMgr = FormMain.Instance.DBManager;
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            Dictionary<int, SensorSchedule> dicSensorStatus = new Dictionary<int, SensorSchedule>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nSensorID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nStatus = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                VariousData<DateTime> dtBegin = WebDBManager.GetDateTimeField(arrResult[i + 2]);
                VariousData<DateTime> dtEnd = WebDBManager.GetDateTimeField(arrResult[i + 3]);

                SensorSchedule status = new SensorSchedule();
                status.SensorID = nSensorID;
                status.Status = nStatus;
                status.BeginTime = dtBegin;
                status.EndTime = dtEnd;

                if (localOffSensorIDList.Contains(nSensorID))
                    status.Status = (int)UnE.PSM.PSMSensor.Status.LocalOff;

                dicSensorStatus[nSensorID] = status;
            }

            return dicSensorStatus;
        }

        private void LoadSensorValueData()
        {
            foreach (PSMSensor item in ReadPSMSensors().Values)
            {
                PSMSensor sensor = item;
                GetSensorData(ref sensor);
            }

            //string strSQL = "SELECT {0} ValueTime FROM PSMSensorValues WHERE ValueTime > CONVERT(DATETIME, '{1}')";
            //string strValueFields = String.Empty;

            //List<int> liValueIndex = new List<int>();

            //foreach (PSMSensor sensor in from sensors in ReadPSMSensors().Values
            //                             orderby sensors.SensorValueIndex ascending
            //                             select sensors
            //                             )
            //{
            //    liValueIndex.Add(sensor.SensorValueIndex);
            //    strValueFields += String.Format("SensorValue{0}, ", sensor.SensorValueIndex);
            //}

            //strSQL = String.Format(strSQL, strValueFields, m_dtLoadedSensorValue.ToString("yyyy-MM-dd HH:mm:ss"));

            //WebDBManager dbMgr = FormMain.Instance.DBManager;
            //ArrayList arrResult = dbMgr.GetResultData(strSQL);

            //if (arrResult == null)
            //    return;

            //int nResultCount = arrResult.Count;

            //for (int i = 0; i < nResultCount - liValueIndex.Count; i += liValueIndex.Count + 1)
            //{
            //    DateTime dt = WebDBManager.GetDateTimeField(arrResult[i + liValueIndex.Count]).Data;

            //    for (int nIndex = 0; nIndex < liValueIndex.Count; nIndex++)
            //    {
            //        double dData = Convert.ToDouble(WebDBManager.GetFloatField(arrResult[i + nIndex].ToString(), 0));

            //        if (dData < 0)
            //            dData = 0;

            //        if (m_dicPSMSensorValue.ContainsKey(liValueIndex[nIndex]) == false)
            //        {
            //            m_dicPSMSensorValue.Add(liValueIndex[nIndex], new Dictionary<DateTime, double>());
            //        }

            //        if (m_dicPSMSensorValue[liValueIndex[nIndex]].ContainsKey(dt) == false)
            //        {
            //            m_dicPSMSensorValue[liValueIndex[nIndex]].Add(dt, dData);
            //        }

            //    }

            //    m_dtLoadedSensorValue = dt;
            //}

        }

        private void RemoveSensorValueData(int nSensorValueIndex)
        {
            DateTime dtLimitDate = m_dicPSMSensorValueLastLoadedDate[nSensorValueIndex].AddMonths(-1);
            DateTime dtLimitDay = new DateTime(dtLimitDate.Year, dtLimitDate.Month, dtLimitDate.Day);

            Dictionary<DateTime, Dictionary<DateTime, double>> dicPSMSensorValue = m_dicPSMSensorValue[nSensorValueIndex];

            List<DateTime> liDateTime = new List<DateTime>();

            foreach (DateTime dt in dicPSMSensorValue.Keys)
            //foreach (DateTime dt in m_dicPSMSensorValue[nSensorValueIndex].Keys)
            {
                if (dt < dtLimitDate)
                    liDateTime.Add(dt);
            }

            foreach (DateTime dt in liDateTime)
            {
                dicPSMSensorValue.Remove(dt);
                //m_dicPSMSensorValue[nSensorValueIndex].Remove(dt);
            }

            liDateTime.Clear();

            Dictionary<DateTime, double> dicValues = null;

            if (dicPSMSensorValue.TryGetValue(dtLimitDay, out dicValues) == false)
                return;
            //if (m_dicPSMSensorValue[nSensorValueIndex].ContainsKey(dtLimitDay) == false)
            //    return;

            foreach (DateTime dt in dicValues.Keys)
            //foreach (DateTime dt in m_dicPSMSensorValue[nSensorValueIndex][dtLimitDay].Keys)
            {
                if (dt < dtLimitDate)
                    liDateTime.Add(dt);
            }

            foreach (DateTime dt in liDateTime)
            {
                dicValues.Remove(dt);
                //m_dicPSMSensorValue[nSensorValueIndex][dtLimitDay].Remove(dt);
            }

        }

        // dtBegin과 dtEnd 사이의 값을 메모리와 DB 두군데 모두에서 지운다.
        public void RemoveSensorValueDBData(int nSensorValueIndex, DateTime dtBegin, DateTime dtEnd)
        {
            DateTime dayBegin = new DateTime(dtBegin.Year, dtBegin.Month, dtBegin.Day);
            DateTime dayEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day);
            Dictionary<DateTime, Dictionary<DateTime, double>> dicPSMSensorValue = m_dicPSMSensorValue[nSensorValueIndex];

            List<DateTime> beginDayList = new List<DateTime>();
            List<DateTime> endDayList = new List<DateTime>();
            List<DateTime> midDayList = new List<DateTime>();
            List<DateTime> beginEndDayList = new List<DateTime>();

            foreach (DateTime dt in dicPSMSensorValue.Keys)
            {
                if (dt >= dayBegin && dt <= dayEnd)
                {
                    // dt가 시작일일 경우
                    if (dt == dayBegin)
                    {
                        // dt가 종료일일 경우
                        if (dt == dayEnd)
                            beginEndDayList.Add(dt);
                        else
                            beginDayList.Add(dt);
                    }
                    // dt가 종료일일 경우
                    else if (dt == dayEnd)
                        endDayList.Add(dt);
                    else
                        midDayList.Add(dt);
                }
            }

            List<DateTime> removeList = new List<DateTime>();
            Dictionary<DateTime, double> dicValues = null;

            foreach (DateTime dt in beginEndDayList)
            {
                if (dicPSMSensorValue.TryGetValue(dt, out dicValues))
                {
                    foreach (KeyValuePair<DateTime, double> pair in dicValues)
                    {
                        if (pair.Key >= dtBegin && pair.Key <= dtEnd)
                            removeList.Add(pair.Key);
                    }

                    foreach (DateTime dtRemove in removeList)
                    {
                        dicValues[dtRemove] = 0.0;
                    }

                    removeList.Clear();
                }
            }

            foreach (DateTime dt in beginDayList)
            {
                if (dicPSMSensorValue.TryGetValue(dt, out dicValues))
                {
                    foreach (KeyValuePair<DateTime, double> pair in dicValues)
                    {
                        if (pair.Key >= dtBegin)
                            removeList.Add(pair.Key);
                    }

                    foreach (DateTime dtRemove in removeList)
                    {
                        dicValues[dtRemove] = 0.0;
                    }

                    removeList.Clear();
                }
            }

            foreach (DateTime dt in endDayList)
            {
                if (dicPSMSensorValue.TryGetValue(dt, out dicValues))
                {
                    foreach (KeyValuePair<DateTime, double> pair in dicValues)
                    {
                        if (pair.Key <= dtEnd)
                            removeList.Add(pair.Key);
                    }

                    foreach (DateTime dtRemove in removeList)
                    {
                        dicValues[dtRemove] = 0.0;
                    }

                    removeList.Clear();
                }
            }

            foreach (DateTime dt in midDayList)
            {
                foreach (KeyValuePair<DateTime, double> pair in dicValues)
                {
                    dicValues[pair.Key] = 0.0;
                }
            }

            string strBeginTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtBegin.Year, dtBegin.Month, dtBegin.Day, dtBegin.Hour, dtBegin.Minute, dtBegin.Second);
            string strEndTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtEnd.Year, dtEnd.Month, dtEnd.Day, dtEnd.Hour, dtEnd.Minute, dtEnd.Second);
            //string strSQL = string.Format("Update PSMSensorValues set SensorValue{0} = -SensorValue{0} where ValueTime >= '{1}' and ValueTime <= '{2}' and SensorValue{0} > 0", nSensorValueIndex, strBeginTime, strEndTime);

            //FormMain.Instance.DBManager.GetResultData(strSQL);

            for (int i=1;i<=10;i++)
            {
                string strSQL = string.Format("Update PSMSensorValue{0} set SensorValue{1} = '-' + SensorValue{1} where ValueTime >= '{2}' and ValueTime <= '{3}' and CharIndex('-', SensorValue{1}) = 0", i, nSensorValueIndex, strBeginTime, strEndTime);
                //string strSQL = string.Format("Update PSMSensorValue{0} set SensorValue{1} = -SensorValue{1} where ValueTime >= '{2}' and ValueTime <= '{3}' and SensorValue{1} > 0", i, nSensorValueIndex, strBeginTime, strEndTime);
                FormMain.Instance.DBManager.GetResultData(strSQL);
            }
        }

        // nLifeTimeMonth : 사용기한(개월수)
        public PSMSensorType AddPSMSensorType(string strTypeName, int nLifeTimeMonth)
        {
            PSMSensorType sensorType = null;

            if (m_dicPSMSensorType.TryGetValue(strTypeName, out sensorType))
            {
                sensorType.LifeTimeMonth = nLifeTimeMonth;
                return sensorType;
            }

            sensorType = new PSMSensorType(strTypeName, nLifeTimeMonth);
            m_dicPSMSensorType[strTypeName] = sensorType;
            return sensorType;
        }

        public Dictionary<string, PSMSensorType> GetPSMSensorTypes()
        {
            return m_dicPSMSensorType;
        }

        public PSMSensorType RemoveSensorType(string strTypeName)
        {
            PSMSensorType sensorType = null;

            if (m_dicPSMSensorType.TryGetValue(strTypeName, out sensorType))
            {
                m_dicPSMSensorType.Remove(strTypeName);

                if (m_dicPSMSensor != null)
                {
                    foreach (KeyValuePair<int, PSMSensor> pair in m_dicPSMSensor)
                    {
                        if (pair.Value.SensorType == sensorType)
                            pair.Value.SensorType = null;
                    }
                }
            }

            return sensorType;
        }
    }
}
