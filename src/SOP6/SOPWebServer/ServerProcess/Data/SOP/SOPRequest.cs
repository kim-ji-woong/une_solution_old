using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Sensor;

namespace ServerProcess.Data.SOP
{
    /// <summary>
    /// SOP 실행요청
    /// </summary>
    public class SOPRequest
    {
        private bool m_isRealMode = false;
        private string m_strSOPFullPath = "";
        private List<string> m_sopParameters = new List<string>();
        private IFacility.FacilityType m_sensorType = IFacility.FacilityType.NONE;

        public IFacility.FacilityType SensorType
        {
            get { return m_sensorType; }
            set { m_sensorType = value; }
        }

        public bool RealMode
        {
            get { return m_isRealMode; }
            set { m_isRealMode = value; }
        }

        public string SOPFullPath
        {
            get { return m_strSOPFullPath; }
            set { m_strSOPFullPath = value; }
        }

        public SOPRequest(bool isRealMode, string strFullPath)
        {
            m_isRealMode = isRealMode;
            m_strSOPFullPath = strFullPath;
        }

        public void AddParameter(string strParam)
        {
            m_sopParameters.Add(strParam);
        }

        public int GetParameterCount()
        {
            return m_sopParameters.Count;
        }

        public string GetParameter(int nIndex)
        {
            if (nIndex < 0 || nIndex >= m_sopParameters.Count)
                return null;

            return m_sopParameters[nIndex];
        }
    }
}
