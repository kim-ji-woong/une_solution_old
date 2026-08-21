using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnE.SOP
{
    public class SOPCheckData
    {
        private int m_nSensorZoneID;
        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        private int m_nActionStepHistoryID;
        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }

        private DateTime m_bTouchTime;
        public DateTime TouchTime
        {
            get { return m_bTouchTime; }
            set { m_bTouchTime = value; }
        }

        private DateTime m_bCheckTime;
        public DateTime CheckTime
        {
            get { return m_bCheckTime; }
            set { m_bCheckTime = value; }
        }

        // 입력대기로 인한 자동종료 사용할 것인가?
        private bool m_bCloseNoInput;
        public bool CloseNoInput
        {
            get { return m_bCloseNoInput; }
            set { m_bCloseNoInput = value; }
        }

        // 입력대기로 인한 자동종료 대기시간(초)
        private int m_bCloseNoInputTime;
        public int CloseNoInputTime
        {
            get { return m_bCloseNoInputTime; }
            set { m_bCloseNoInputTime = value; }
        }

        // 센서신호 복구시 종료할 것인가?
        // ReciveSensorClsseTime가 true이면 CloseSensorWaitTime시간 이후 종료
        // ReciveSensorClsseTime가 false이면 즉시 종료
        private bool m_bCloseSensorClose;
        public bool CloseSensorClose
        {
            get { return m_bCloseSensorClose; }
            set { m_bCloseSensorClose = value; }
        }

        // 센서신호 복구시 일정시간 이후 종료할 때 지연시간(초)
        private int m_nCloseSensorWaitTime;
        public int CloseSensorWaitTime
        {
            get { return m_nCloseSensorWaitTime; }
            set { m_nCloseSensorWaitTime = value; }
        }

        private bool m_bReciveSensorClose;
        public bool ReciveSensorClose
        {
            get { return m_bReciveSensorClose; }
            set { m_bReciveSensorClose = value; }
        }

        // 센서신호 복구시 일정시간 이후 종료할 것인가?
        // 이 값이 true이면 CloseSensorClose도 true이어야 한다.
        private bool m_bReciveSensorClsseTime;
        public bool ReciveSensorClsseTime
        {
            get { return m_bReciveSensorClsseTime; }
            set { m_bReciveSensorClsseTime = value; }
        }


        private bool m_bCheckedSensorClose = false;
        public bool CheckedSensorClose
        {
            get { return m_bCheckedSensorClose; }
            set { m_bCheckedSensorClose = value; }
        }

        private bool m_bCheckedTimeClose = false;
        public bool CheckedTimeClose
        {
            get { return m_bCheckedTimeClose; }
            set { m_bCheckedTimeClose = value; }
        }

        private bool m_bCheckedSensorClose2 = false;
        public bool CheckedSensorClose2
        {
            get { return m_bCheckedSensorClose2; }
            set { m_bCheckedSensorClose2 = value; }
        }

        private bool m_bCheckedTimeClose2 = false;
        public bool CheckedTimeClose2
        {
            get { return m_bCheckedTimeClose2; }
            set { m_bCheckedTimeClose2 = value; }
        }


        private PopupSOPClose form = new PopupSOPClose();
        public PopupSOPClose Form
        {
            get { return form; }
            set { form = value; }
        }

        public int SensorZoneHistoryID { get; set; }

        private UnE.Sensor.IFacility.FacilityType m_sensorType = Sensor.IFacility.FacilityType.NONE;
        public UnE.Sensor.IFacility.FacilityType SensorType
        {
            get { return m_sensorType; }
            set { m_sensorType = value; }
        }

        // 동시에 여러단계의 SOP가 실행중일 경우 가장 높은 단계의 SOP가 몇단계인지 알려준다.
        private int m_nMaxActionStepIndex = -1;
        public int MaxActionStepIndex
        {
            get { return m_nMaxActionStepIndex; }
            set { m_nMaxActionStepIndex = value; }
        }
    }
}
