using System;
using System.Collections.Generic;
using System.Text;

namespace SDMS.BLL.Models.Response
{
    public class ResponseSensorCount : MessageResult
    {
        // 전체 화재센서 개수
        private int m_nFireSensorCount = 0;
        // 사용할수 없는 화재센서 개수
        private int m_nDisabledFireSensorCount = 0;
        // 전체 누출센서 개수
        private int m_nPSMSensorCount = 0;
        // 사용할수 없는 누출센서 개수
        private int m_nDisabledPSMSensorCount = 0;
        // 전체 기타센서 개수
        private int m_nEtcSensorCount = 0;
        // 사용할수 없는 기타센서 개수
        private int m_nDisabledEtcSensorCount = 0;
        // 전체 CCTV 개수
        private int m_nCCTVCount = 0;
        // 사용할수 없는 CCTV 개수
        private int m_nDisabledCCTVCount = 0;

        // 전체 화재센서 개수
        public int FireSensorCount
        {
            get { return m_nFireSensorCount; }
            set { m_nFireSensorCount = value; }
        }

        // 사용할수 없는 화재센서 개수
        public int DisabledFireSensorCount
        {
            get { return m_nDisabledFireSensorCount; }
            set { m_nDisabledFireSensorCount = value; }
        }

        // 전체 누출센서 개수
        public int PsmSensorCount
        {
            get { return m_nPSMSensorCount; }
            set { m_nPSMSensorCount = value; }
        }

        // 사용할수 없는 누출센서 개수
        public int DisabledPSMSensorCount
        {
            get { return m_nDisabledPSMSensorCount; }
            set { m_nDisabledPSMSensorCount = value; }
        }

        // 전체 기타센서 개수
        public int EtcSensorCount
        {
            get { return m_nEtcSensorCount; }
            set { m_nEtcSensorCount = value; }
        }

        // 사용할수 없는 기타센서 개수
        public int DisabledEtcSensorCount
        {
            get { return m_nDisabledEtcSensorCount; }
            set { m_nDisabledEtcSensorCount = value; }
        }

        // 전체 CCTV 개수
        public int CctvCount
        {
            get { return m_nCCTVCount; }
            set { m_nCCTVCount = value; }
        }

        // 사용할수 없는 CCTV 개수
        public int DisabledCCTVCount
        {
            get { return m_nDisabledCCTVCount; }
            set { m_nDisabledCCTVCount = value; }
        }
    }
}
