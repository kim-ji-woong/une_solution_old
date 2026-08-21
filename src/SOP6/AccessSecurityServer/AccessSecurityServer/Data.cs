using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;

namespace AccessSecurityServer
{
    public static class Facility
    {
        // 모든 Facility 및 소방설비와 센서들의 Type 정보를 기록
        public enum FacilityType
        {
            NONE = -1,
            // 서울대학교 e재난 시스템 - S1시스템 통합으로 추가됨
            // skkim     2017-03-14
            Intrusion_S1 = 900,  // SVMS 침입
            Loiter_S1 = 901,     // SVMS 배회
            Slip_S1 = 902,   // SVMS 쓰러짐
            Steal_S1 = 903,       // SVMS 도난
            Abandoned_S1 = 904,           // SVMS 방치
            VirtualFence_S1 = 905,      // SVMS 가상펜스
            Fire_S1 = 906,              // SVMS 화재
            EmergencyBell_S1 = 907,     // SVMS 비상벨

            GeneralIntrusionT1_S1 = 1001,  // S1Access 일반침입1
            GeneralIntrusionT2_S1 = 1002,   // S1Access 일반 침입2
            InternalIntrusionT3_S1 = 1003,// S1Access 내부침입
            VaultIntrusionT4_S1 = 1004,   // S1Access 금고침입
            FireF1_S1 = 2000,             // S1Access 화재
            CustomerEmergencyC1_S1 = 2100, // S1Access 고객비상
            CustomerEmergencyC2_S1 = 2110,// S1Access 고객 비상
            RescueQQ_S1 = 2200,           // S1Access 구급
            GasG1_S1 = 2300,               // S1Access 가스
            BlackoutAbnormalityU1_S1 = 3000, // S1Access 정전이상
            LeakAbnormalityU4_S1 = 3004,     // S1Access 누수이상
            SynthesisAlertAbnormalityU8_S1 = 3008 // S1Access 종합경보반 이상

        };

        private static Dictionary<int, FacilityType> m_dicFacilityType = null;
        private static object m_lockObj = new object();
        // nFacilityType : DB 스키마에 정의된 값
        public static FacilityType ToFacilityType(int nFacilityType)
        {
            lock (m_lockObj)
            {
                if (m_dicFacilityType == null)
                {

                    m_dicFacilityType = new Dictionary<int, FacilityType>();

                    Array arValues = Enum.GetValues(typeof(FacilityType));
                    foreach (FacilityType type in arValues)
                    {
                        m_dicFacilityType[(int)type] = type;
                    }
                }
            }

            FacilityType fType;

            if (m_dicFacilityType.TryGetValue(nFacilityType, out fType))
                return fType;

            return FacilityType.NONE;
        }
    }

    public class Location
    {
        // AccessDB(S1) View_External_Device의 데이터
        private int m_nID = -1;
        // AccessDB(S1) View_External_Device의 데이터
        private string m_strLocationName = "";
        // SOPDB EquipmentZone의 데이터
        private int m_nEquipmentZoneID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strLocationName; }
            set { m_strLocationName = value; }
        }

        public int EquipZoneID
        {
            get { return m_nEquipmentZoneID; }
            set { m_nEquipmentZoneID = value; }
        }
    }

    public class DeviceType
    {
        private static Dictionary<int, DeviceType> m_dicType = new Dictionary<int, DeviceType>();

        private int m_nDeviceType = -1;
        private string m_strDeviceTypeName = "";

        public int TypeID
        {
            get { return m_nDeviceType; }
            set { m_nDeviceType = value; }
        }

        public string TypeName
        {
            get { return m_strDeviceTypeName; }
            set { m_strDeviceTypeName = value; }
        }

        public static Dictionary<int, DeviceType> DeviceTypes
        {
            get { return m_dicType; }
        }
    }

    public class Device
    {
        private int m_nID = -1;
        private string m_strDeviceName = "";
        private Location m_location = null;
        private DeviceType m_deviceType = null;
        private Alarm.StateType m_alarmState = Alarm.StateType.NONE;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strDeviceName; }
            set { m_strDeviceName = value; }
        }

        public AccessSecurityServer.Location Location
        {
            get { return m_location; }
            set { m_location = value; }
        }

        public AccessSecurityServer.DeviceType DeviceType
        {
            get { return m_deviceType; }
            set { m_deviceType = value; }
        }

        public Alarm.StateType AlarmState
        {
            get { return m_alarmState; }
            set { m_alarmState = value; }
        }
    }

    public class Alarm
    {
        public enum StateType
        {
            NONE = 0,
            GENERAL_INTRUSION1 = 1001,
            GENERAL_INTRUSION2 = 1002,
            INTERNAL_INTRUSION = 1003,
            VAULT_INTRUSION = 1004,
            FIRE = 2000,
            CUSTOMER_EMERGENCY1 = 2100,
            CUSTOMER_EMERGENCY2 = 2110,
            /*RESCUE = 2200,
            GAS_LEAK = 2300,
            BLACK_OUT = 3000,
            WATER_LEAK = 3004,
            ABNORMAL_PANEL = 3008,*/
            UNKNOWN = -1
        }

        private int m_nID = -1;
        private StateType m_alarmState = StateType.NONE;
        // 알람이 발생한 시간(Device 시간)
        private VariousData<DateTime> m_dtEvent = null;
        // 알람을 받은 시간
        private VariousData<DateTime> m_dtReceived = null;
        private Device m_device = null;
        private string m_strState = null;
        private string m_strPrevState = null;
        private string m_strCardNo = null;
        private string m_strContent1 = null;
        private string m_strContent2 = null;
        private string m_strContent3 = null;
        private string m_strContent4 = null;

        private static Dictionary<int, StateType> m_dicAlarmStateType = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public StateType AlarmState
        {
            get { return m_alarmState; }
            set { m_alarmState = value; }
        }

        // 알람이 발생한 시간(Device 시간)
        public VariousData<DateTime> EventTime
        {
            get { return m_dtEvent; }
            set { m_dtEvent = value; }
        }

        // 알람을 받은 시간
        public VariousData<DateTime> ReceivedTime
        {
            get { return m_dtReceived; }
            set { m_dtReceived = value; }
        }

        public AccessSecurityServer.Device Device
        {
            get { return m_device; }
            set { m_device = value; }
        }

        public string State
        {
            get { return m_strState; }
            set { m_strState = value; }
        }

        public string PrevState
        {
            get { return m_strPrevState; }
            set { m_strPrevState = value; }
        }

        public string CardNo
        {
            get { return m_strCardNo; }
            set { m_strCardNo = value; }
        }

        public string Content1
        {
            get { return m_strContent1; }
            set { m_strContent1 = value; }
        }

        public string Content2
        {
            get { return m_strContent2; }
            set { m_strContent2 = value; }
        }

        public string Content3
        {
            get { return m_strContent3; }
            set { m_strContent3 = value; }
        }

        public string Content4
        {
            get { return m_strContent4; }
            set { m_strContent4 = value; }
        }

        public string GetLocationName()
        {
            if (m_device == null)
                return "";

            if (m_device.Location == null)
                return "";

            return m_device.Location.Name;
        }

        public static StateType ToAlarmState(string strState)
        {
            int nStateType = -1;
            int.TryParse(strState, out nStateType);

            if (m_dicAlarmStateType == null)
            {
                m_dicAlarmStateType = new Dictionary<int, StateType>();

                foreach (StateType type in Enum.GetValues(typeof(StateType)))
                {
                    m_dicAlarmStateType[(int)type] = type;
                }
            }

            StateType sType;

            if (m_dicAlarmStateType.TryGetValue(nStateType, out sType))
                return sType;

            return StateType.UNKNOWN;
        }

        public static string ToAlarmString(StateType type)
        {
            if (type == StateType.GENERAL_INTRUSION1)
                return "침입(T1)";
            else if (type == StateType.GENERAL_INTRUSION2)
                return "침입(T2)";
            else if (type == StateType.INTERNAL_INTRUSION)
                return "침입(T3)";
            else if (type == StateType.VAULT_INTRUSION)
                return "침입(T4)";
            else if (type == StateType.FIRE)
                return "화재(F1)";
            else if (type == StateType.CUSTOMER_EMERGENCY1)
                return "여자화장실 내부비상벨";//"고객비상(C1)";
            else if (type == StateType.CUSTOMER_EMERGENCY2)
                return "여자화장실 내부비상벨";//"고객비상(C1)";
            /*else if (type == StateType.RESCUE)
                return "구급(QQ)";
            else if (type == StateType.GAS_LEAK)
                return "가스(G1)";
            else if (type == StateType.BLACK_OUT)
                return "정전이상(U1)";
            else if (type == StateType.WATER_LEAK)
                return "누수이상(U4)";
            else if (type == StateType.ABNORMAL_PANEL)
                return "종합경보반 이상(U8)";*/

            return "알수없음";
        }
    }
}
