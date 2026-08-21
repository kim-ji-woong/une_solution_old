using SensorMaker.BLL.Models.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMaker.BLL.Models.Request
{
    using Data.Sensor;

    public class RequestData
    {
        private RequestSaveXML m_requestSaveXML = null;
        private RequestSensorExcelFile m_requestSensorExcelFile = null;
        private RequestUploadModelFile m_requestUploadModelFile = null;
        private RequestRemoveTempFile m_requestRemoveTempFile = null;
        private RequestOpenTempXML m_requestOpenTempXML = null;

        public RequestSaveXML RequestSaveXML
        {
            get { return m_requestSaveXML; }
            set { m_requestSaveXML = value; }
        }

        public RequestSensorExcelFile RequestSensorExcelFile
        {
            get { return m_requestSensorExcelFile; }
            set { m_requestSensorExcelFile = value; }
        }

        public RequestUploadModelFile RequestUploadModelFile
        {
            get { return m_requestUploadModelFile; }
            set { m_requestUploadModelFile = value; }
        }

        public RequestRemoveTempFile RequestRemoveTempFile
        {
            get { return m_requestRemoveTempFile; }
            set { m_requestRemoveTempFile = value; }
        }

        public RequestOpenTempXML RequestOpenTempXML
        {
            get { return m_requestOpenTempXML; }
            set { m_requestOpenTempXML = value; }
        }
    }

    public class RequestSensorExcelFile
    {
        private List<FireSensor> m_fireSensors = null;
        private List<PSMSensor> m_psmSensors = null;
        private List<EtcSensor> m_etcSensors = null;
        private List<CCTVSensor> m_cctvs = null;

        public List<FireSensor> FireSensors
        {
            get { return m_fireSensors; }
            set { m_fireSensors = value; }
        }

        public List<PSMSensor> PSMSensors
        {
            get { return m_psmSensors; }
            set { m_psmSensors = value; }
        }

        public List<EtcSensor> EtcSensors
        {
            get { return m_etcSensors; }
            set { m_etcSensors = value; }
        }

        public List<CCTVSensor> Cctvs
        {
            get { return m_cctvs; }
            set { m_cctvs = value; }
        }
    }

    public class RequestOpenTempXML
    {
        private int m_nUserID = -1;
        private string m_strUserName = "";

        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }

        public string UserName
        {
            get { return m_strUserName; }
            set { m_strUserName = value; }
        }
    }
}
