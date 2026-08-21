using SDMS.Model.CCTV;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDMS.BLL.Models.Response
{
    public class ResponseUseSensor : MessageResult
    {
        ICollection<Data.Sensor.FireSensor> m_fireSensors = null;
        ICollection<Model.Sensor.Fire> m_disabledFireSensors = null;
        ICollection<Data.Sensor.PSMSensor> m_psmSensors = null;
        ICollection<Model.Sensor.PSM> m_disabledPSMSensors = null;
        ICollection<Data.Sensor.EtcSensor> m_etcSensors = null;
        ICollection<Model.Sensor.ETC> m_disabledEtcSensors = null;
        ICollection<Data.Sensor.CCTVSensor> m_cctvSensors = null;
        ICollection<CCTV> m_disabledCCTVs = null;


        // 전체 화재센서 
        public ICollection<Data.Sensor.FireSensor> FireSensors
        {
            get { return m_fireSensors; }
            set { m_fireSensors = value; }
        }

        // 사용할수 없는 화재센서 
        public ICollection<Model.Sensor.Fire> DisabledFireSensors
        {
            get { return m_disabledFireSensors; }
            set { m_disabledFireSensors = value; }
        }

        // 전체 누출센서 
        public ICollection<Data.Sensor.PSMSensor> PsmSensors
        {
            get { return m_psmSensors; }
            set { m_psmSensors = value; }
        }

        // 사용할수 없는 누출센서
        public ICollection<Model.Sensor.PSM> DisabledPSMSensors
        {
            get { return m_disabledPSMSensors; }
            set { m_disabledPSMSensors = value; }
        }

        // 전체 기타센서 
        public ICollection<Data.Sensor.EtcSensor> EtcSensors
        {
            get { return m_etcSensors; }
            set { m_etcSensors = value; }
        }

        // 사용할수 없는 기타센서 
        public ICollection<Model.Sensor.ETC> DisabledEtcSensors
        {
            get { return m_disabledEtcSensors; }
            set { m_disabledEtcSensors = value; }
        }

        // 전체 CCTV 
        public ICollection<Data.Sensor.CCTVSensor> CCTVs
        {
            get { return m_cctvSensors; }
            set { m_cctvSensors = value; }
        }

        // 사용할수 없는 CCTV 
        public ICollection<CCTV> DisabledCCTVs
        {
            get { return m_disabledCCTVs; }
            set { m_disabledCCTVs = value; }
        }
    }
}
