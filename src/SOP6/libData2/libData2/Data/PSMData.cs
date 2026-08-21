using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnE
{
    namespace PSM
    {
        public class PSMTank
        {
            // NONE = 미설정, ETC = 기타, AREA_12 = 12호기, AREA_34 = 34호기, AREA_56 = 56호기, WATER = 물처리 및 탈황폐수처리
            public enum Area { NONE = -1, ETC = 0, AREA_12 = 1, AREA_34 = 2, AREA_56 = 3, WATER = 4 }


            private int m_nTankID = -1;
            private string m_strTankName = "";
            private Spatial.EquipmentZone m_equipZone = null;
            private string m_strBoundaries = null;
            private PSMMaterial m_material = null;
            // 이 값이 0보다 작으면 수동으로 입력한 값이다.
            // 0보다 같거나 크면 외부 시스템으로부터 전달받은 값이다.
            private DBUtility2.VariousData<float> m_fRemains = null;
            private DBUtility2.VariousData<float> m_fCapacity = null;
            private string m_strUnitName = "";
            private string m_strBroadcastName = "";
            private string m_strLocationName = "";
            // 초기 이격거리(미터)
            private int m_nEvacInitDistance = -1;
            // 주간 방호 대피거리(미터)
            private int m_nEvacDayDistance = -1;
            // 야간 방호 대피거리(미터)
            private int m_nEvacNightDistance = -1;
            // 탱크에 대한 검색용 설비 위치
            private Area m_areaType = Area.NONE;

            private List<PSMSensor> m_sensorList = new List<PSMSensor>();

            public int ID
            {
                get { return m_nTankID; }
                set { m_nTankID = value; }
            }

            public string Name
            {
                get { return m_strTankName; }
                set { m_strTankName = value; }
            }

            public Spatial.EquipmentZone EquipZone
            {
                get { return m_equipZone; }
                set { m_equipZone = value; }
            }

            public string Boundaries
            {
                get { return m_strBoundaries; }
                set { m_strBoundaries = value; }
            }

            public PSMMaterial Material
            {
                get { return m_material; }
                set { m_material = value; }
            }

            // 잔량
            // 이 값이 0보다 작으면 수동으로 입력한 값이다.
            // 0보다 같거나 크면 외부 시스템으로부터 전달받은 값이다.
            public DBUtility2.VariousData<float> Remains
            {
                get { return m_fRemains; }
                set { m_fRemains = value; }
            }

            // 최대 용량
            public DBUtility2.VariousData<float> Capacity
            {
                get { return m_fCapacity; }
                set { m_fCapacity = value; }
            }

            // 용량의 단위
            public string UnitName
            {
                get { return m_strUnitName; }
                set { m_strUnitName = value; }
            }

            public string BroadcastName
            {
                get { return m_strBroadcastName; }
                set { m_strBroadcastName = value; }
            }

            public string LocationName
            {
                get { return m_strLocationName; }
                set { m_strLocationName = value; }
            }

            // 초기 이격거리(미터)
            public int EvacInitDistance
            {
                get { return m_nEvacInitDistance; }
                set { m_nEvacInitDistance = value; }
            }

            // 주간 방호 대피거리(미터)
            public int EvacDayDistance
            {
                get { return m_nEvacDayDistance; }
                set { m_nEvacDayDistance = value; }
            }

            // 야간 방호 대피거리(미터)
            public int EvacNightDistance
            {
                get { return m_nEvacNightDistance; }
                set { m_nEvacNightDistance = value; }
            }

            // 탱크에 대한 검색용 설비 위치
            public Area AreaType
            {
                get { return m_areaType; }
                set { m_areaType = value; }
            }


            public List<PSMSensor> LinkedSensorList
            {
                get { return m_sensorList; }
            }

            public void AddSensor(PSMSensor sensor)
            {
                if (sensor == null)
                    return;

                if (!m_sensorList.Contains(sensor))
                {
                    m_sensorList.Add(sensor);
                    sensor.AddTank(this);
                }
            }

            public void RemoveSensor(PSMSensor sensor)
            {
                if (sensor == null)
                    return;

                if (m_sensorList.Contains(sensor))
                {
                    m_sensorList.Remove(sensor);
                    sensor.RemoveTank(this);
                }
            }
        }

        public class PSMMaterial
        {
            private int m_nID = -1;
            private string m_strName = "";
            private string m_strUOM = "";
            // [유해화학물질 특성] 매뉴얼의 Page 번호
            private int m_nManualPageNo = -1;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string Name
            {
                get { return m_strName; }
                set { m_strName = value; }
            }

            public string UOM
            {
                get { return m_strUOM; }
                set { m_strUOM = value; }
            }

            // [유해화학물질 특성] 매뉴얼의 Page 번호
            public int PageNo
            {
                get { return m_nManualPageNo; }
                set { m_nManualPageNo = value; }
            }

            private float m_fInitEvacDistance = 0.0f;
            public float InitEvacDistance
            {
                get { return m_fInitEvacDistance; }
                set { m_fInitEvacDistance = value; }
            }

            private float m_fDayEvacDistance = 0.0f;
            public float DayEvacDistance
            {
                get { return m_fDayEvacDistance; }
                set { m_fDayEvacDistance = value; }
            }

            private float m_fNightEvacDistance = 0.0f;
            public float NightEvacDistance
            {
                get { return m_fNightEvacDistance; }
                set { m_fNightEvacDistance = value; }
            }


            public override string ToString()
            {
                return Name;
            }
        }

        // OriginSensor 클래스
        public class PSMSensor //: UnE.Sensor.IFacility
        {
            public enum Status { Unknown = -1, On, Off, Off4Work, LocalOff };

            private string m_strName = "";
            private UnE.Geometry.Vertex2D m_pos = null;
            // 현재 센서 데이터
            private float m_fCurrentData = -1.0f;
            // 1단계 알람을 발생시킬 센서 데이터 하한치
            private float m_fLimitLevel1 = -1.0f;
            // 2단계 알람을 발생시킬 센서 데이터 하한치
            private float m_fLimitLevel2 = -1.0f;
            // 3단계 알람을 발생시킬 센서 데이터 하한치
            private float m_fLimitLevel3 = -1.0f;
            // 1단계 알람을 발생시킬 센서 데이터 하한치 초기값
            private float m_fDefLimitLevel1 = -1.0f;
            // 2단계 알람을 발생시킬 센서 데이터 하한치 초기값
            private float m_fDefLimitLevel2 = -1.0f;
            // 3단계 알람을 발생시킬 센서 데이터 하한치 초기값
            private float m_fDefLimitLevel3 = -1.0f;
            private List<PSMTank> m_tankList = new List<PSMTank>();
            private Status m_status = Status.Unknown;
            private DBUtility2.VariousData<DateTime> m_beginWorkTime = null;
            private DBUtility2.VariousData<DateTime> m_endWorkTime = null;
            private DBUtility2.VariousData<int> m_nCurrentAlarmDepth = null;
            // 망번호
            private int m_nReceiverID = -1;
            // 회선번호
            private int m_nTagNo = -1;
            // 센서 측정데이터 인덱스 번호
            private int m_nSensorValueIndex = -1;
            private PSMSensorZone m_sensorZone = null;
            //private bool m_recursive = false;
            private int m_nID = -1;
            private int m_nEquipZoneID = -1;
            private DBUtility2.VariousData<DateTime> m_installDate = null;
            private PSMSensorType m_sensorType = null;
            // 1단계 알람신호에 대하여 알람 처리를 할 것인가?
            private bool m_allowReceiveLevel1Alarm = true;
            // 2단계 알람신호에 대하여 알람 처리를 할 것인가?
            private bool m_allowReceiveLevel2Alarm = true;
            // 3단계 알람신호에 대하여 알람 처리를 할 것인가?
            private bool m_allowReceiveLevel3Alarm = true;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string Name
            {
                get { return m_strName; }
                set { m_strName = value; }
            }

            public UnE.Geometry.Vertex2D Position
            {
                get { return m_pos; }
                set { m_pos = value; }
            }

            public float CurrentData
            {
                get { return m_fCurrentData; }
                set { m_fCurrentData = value; }
            }

            // 1단계 알람을 발생시킬 센서 데이터 하한치
            public float LimitLevel1
            {
                get { return m_fLimitLevel1; }
                set { m_fLimitLevel1 = value; }
            }

            // 2단계 알람을 발생시킬 센서 데이터 하한치
            public float LimitLevel2
            {
                get { return m_fLimitLevel2; }
                set { m_fLimitLevel2 = value; }
            }

            // 3단계 알람을 발생시킬 센서 데이터 하한치
            public float LimitLevel3
            {
                get { return m_fLimitLevel3; }
                set { m_fLimitLevel3 = value; }
            }

            // 1단계 알람을 발생시킬 센서 데이터 하한치 초기값
            public float DefLimitLevel1
            {
                get { return m_fDefLimitLevel1; }
                set { m_fDefLimitLevel1 = value; }
            }

            // 2단계 알람을 발생시킬 센서 데이터 하한치 초기값
            public float DefLimitLevel2
            {
                get { return m_fDefLimitLevel2; }
                set { m_fDefLimitLevel2 = value; }
            }

            // 3단계 알람을 발생시킬 센서 데이터 하한치 초기값
            public float DefLimitLevel3
            {
                get { return m_fDefLimitLevel3; }
                set { m_fDefLimitLevel3 = value; }
            }

            public List<PSMTank> LinkedTankList
            {
                get { return m_tankList; }
            }

            public Status SensorStatus
            {
                get { return m_status; }
                set
                {
                    if (m_status != value)
                    {
                        bool spreadValue = m_status == Status.Off4Work || value == Status.Off4Work;
                        m_status = value;

                        // 특정 센서에 대하여 작업중 표시를 한다는 것은 해당 센서가 감시하는 탱크가 작업중이라는 의미이므로
                        // 같은 탱크를 감시하는 모든 센서들의 상태를 같이 변화시킨다.
                        if (spreadValue)
                        {
                            List<PSMSensor> sensors = GetSameSensors();

                            foreach (PSMSensor sensor in sensors)
                            {
                                sensor.m_status = value;
                            }

                            sensors.Clear();
                        }
                    }
                }
            }

            public DBUtility2.VariousData<DateTime> BeginWorkTime
            {
                get { return m_beginWorkTime; }
                set
                {
                    if (!CheckSameTime(m_beginWorkTime, value))
                    {
                        m_beginWorkTime = value;

                        // 같은 탱크들을 감시하는 센서들의 작업 시작시간을 동일하게 맞춘다.
                        List<PSMSensor> sensors = GetSameSensors();

                        foreach (PSMSensor sensor in sensors)
                        {
                            sensor.m_beginWorkTime = value;
                        }

                        sensors.Clear();
                    }
                }
            }

            public DBUtility2.VariousData<DateTime> EndWorkTime
            {
                get { return m_endWorkTime; }
                set
                {
                    if (!CheckSameTime(m_endWorkTime, value))
                    {
                        m_endWorkTime = value;

                        // 같은 탱크들을 감시하는 센서들의 작업 종료시간을 동일하게 맞춘다.
                        List<PSMSensor> sensors = GetSameSensors();

                        foreach (PSMSensor sensor in sensors)
                        {
                            sensor.m_endWorkTime = value;
                        }

                        sensors.Clear();
                    }
                }
            }

            public UnE.Sensor.IFacility.FacilityType Type
            {
                get { return UnE.Sensor.IFacility.FacilityType.PSM_SENSOR; }
            }

            public int GetLayerID()
            {
                return SDMS.ID.ID_LAYER_PSM_SENSOR;
            }

            public string IconPath
            {
                get { return "유해화학물질"; }
            }

            public string DisconnectIconPath
            {
                get { return "유해화학물질"; }
            }

            public DBUtility2.VariousData<int> CurrentAlarmDepth
            {
                set { m_nCurrentAlarmDepth = value; }
                get { return m_nCurrentAlarmDepth; }
            }

            // 망번호
            public int ReceiverID
            {
                set { m_nReceiverID = value; }
                get { return m_nReceiverID; }
            }

            // 회선번호
            public int TagNo
            {
                set { m_nTagNo = value; }
                get { return m_nTagNo; }
            }

            // Tape 방식의 센서인가?
            public bool IsTapeType
            {
                get
                {
                    if (LinkedTankList.Count == 0)
                        return false;

                    PSMTank tank = LinkedTankList[0];

                    if (tank.Material == null)
                        return false;

                    // 물질정보에 단위가 없으면 Tape 방식이다.
                    return tank.Material.UOM.Length == 0;
                }
            }
            
            // 센서 측정데이터 인덱스 번호
            public int SensorValueIndex
            {
                get { return m_nSensorValueIndex; }
                set { m_nSensorValueIndex = value; }
            }

            public int EquipZoneID
            {
                get { return m_nEquipZoneID; }
                set { m_nEquipZoneID = value; }
            }

            public DBUtility2.VariousData<DateTime> InstallDate
            {
                get { return m_installDate; }
                set { m_installDate = value; }
            }

            public PSMSensorType SensorType
            {
                get { return m_sensorType; }
                set { m_sensorType = value; }
            }

            // 1단계 알람신호에 대하여 알람 처리를 할 것인가?
            public bool AllowReceiveLevel1Alarm
            {
                get { return m_allowReceiveLevel1Alarm; }
                set { m_allowReceiveLevel1Alarm = value; }
            }

            // 2단계 알람신호에 대하여 알람 처리를 할 것인가?
            public bool AllowReceiveLevel2Alarm
            {
                get { return m_allowReceiveLevel2Alarm; }
                set { m_allowReceiveLevel2Alarm = value; }
            }
            
            // 3단계 알람신호에 대하여 알람 처리를 할 것인가?
            public bool AllowReceiveLevel3Alarm
            {
                get { return m_allowReceiveLevel3Alarm; }
                set { m_allowReceiveLevel3Alarm = value; }
            }

            //public PSMSensorZone SensorZone
            //{
            //    get { return m_sensorZone; }
            //    set
            //    {
            //        if (!m_recursive)
            //        {
            //            // 무한루프에 빠지는 것을 막는다.
            //            m_recursive = true;

            //            if (m_sensorZone != value)
            //            {
            //                if (m_sensorZone != null)
            //                    m_sensorZone.OrgSensor = null;

            //                m_sensorZone = value;

            //                if (value != null)
            //                    value.OrgSensor = this;
            //            }

            //            m_recursive = false;
            //        }
            //    }
            //}

            private bool CheckSameTime(DBUtility2.VariousData<DateTime> time1, DBUtility2.VariousData<DateTime> time2)
            {
                if (time1 == null && time2 == null)
                    return true;
                else if (time1 == null || time2 == null)
                    return false;

                return time1.Data == time2.Data;
            }

            public void AddTank(PSMTank tank)
            {
                if (tank == null)
                    return;

                if (!m_tankList.Contains(tank))
                {
                    m_tankList.Add(tank);
                    tank.AddSensor(this);
                }
            }

            public void RemoveTank(PSMTank tank)
            {
                if (tank == null)
                    return;

                if (m_tankList.Contains(tank))
                {
                    m_tankList.Remove(tank);
                    tank.RemoveSensor(this);
                }
            }

            // 같은 탱크들을 감시하는 Sensor들을 얻어온다.
            public List<PSMSensor> GetSameSensors()
            {
                List<PSMSensor> sensors = new List<PSMSensor>();

                foreach (PSMTank tank in m_tankList)
                {
                    foreach (PSMSensor sensor in tank.LinkedSensorList)
                    {
                        if (sensor == this)
                            continue;

                        if (!sensors.Contains(sensor))
                            sensors.Add(sensor);
                    }
                }

                return sensors;
            }

            public static Status ToStatus(int nStatus)
            {
                if (nStatus < (int)Status.On || nStatus > (int)Status.LocalOff)
                    return Status.Unknown;

                return (Status)nStatus;
            }

            private int m_nMaterialType = 0;
            public int MaterialType
            {
                get { return m_nMaterialType; }
                set { m_nMaterialType = value; }
            }

            public override string ToString()
            {
                return m_strName;
            }

            private string m_szDepartment = "";
            public string Department
            {
                get { return m_szDepartment; }
                set { m_szDepartment = value; }
            }

            private string m_szPhoneNumber = "";
            public string PhoneNumber
            {
                get { return m_szPhoneNumber; }
                set { m_szPhoneNumber = value; }
             }
        }

        public class PSMSensorType
        {
            private string m_strTypeName = "";
            // 센서 사용기한(개월수)
            private int m_nLifeTimeMonth = 0;

            public string TypeName
            {
                get { return m_strTypeName; }
                set { m_strTypeName = value; }
            }

            // 센서 사용기한(개월수)
            public int LifeTimeMonth
            {
                get { return m_nLifeTimeMonth; }
                set { m_nLifeTimeMonth = value; }
            }

            public PSMSensorType()
            {
            }

            public PSMSensorType(string strTypeName, int nLifeTimeMonth)
            {
                m_strTypeName = strTypeName;
                m_nLifeTimeMonth = nLifeTimeMonth;
            }

            public override string ToString()
            {
                return TypeName;
            }
        }

        // SensorZone 클래스
        public class PSMSensorZone : UnE.Sensor.ISensor
        {
            // OrgSensorID의 값은 m_originSensor.ID와 동일하다.
            protected PSMSensor m_originSensor = null;
            //private bool m_recursive = false;

            public PSMSensor OrgSensor
            {
                get { return m_originSensor; }
                set
                {
                    //if (!m_recursive)
                    {
                        // 무한루프에 빠지는 것을 막는다.
                        //m_recursive = true;

                        if (m_originSensor != value)
                        {
                            //if (m_originSensor != null)
                            //    m_originSensor.SensorZone = null;

                            m_originSensor = value;

                            //if (value != null)
                            //    value.SensorZone = this;
                        }

                        //m_recursive = false;
                    }
                }
            }

            public override UnE.Sensor.IFacility.FacilityType Type
            {
                get { return UnE.Sensor.IFacility.FacilityType.PSM_SENSOR; }
            }

            override public int GetLayerID()
            {
                if (m_originSensor == null)
                    return 0;

                return m_originSensor.GetLayerID();
            }

            public override string IconPath
            {
                get
                {
                    if (m_strIconPath == null)
                    {
                        return m_originSensor == null ? "" : m_originSensor.IconPath;
                    }

                    return m_strIconPath;
                }
                set
                {
                    m_strIconPath = value;
                }
            }

            public override string DisconnectIconPath
            {
                get { return m_originSensor == null ? "" : m_originSensor.DisconnectIconPath; }
            }
        }
    }
}
