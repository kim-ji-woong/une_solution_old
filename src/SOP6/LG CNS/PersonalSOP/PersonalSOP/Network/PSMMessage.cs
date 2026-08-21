using System.Collections;

namespace PersonalSOP.Network
{
    public class PSMMessage : Message
    {
        private string m_strTankName = "";
        private double m_dTemperature = 0.0;
        private string m_strSOPFullPath = "";
        private bool m_isRealMode = false;

        public string SOPFullPath
        {
            get { return m_strSOPFullPath; }
            set { m_strSOPFullPath = value; }
        }

        public PSMMessage(string strTankName, double dTemperature, string strSOPFullPath, bool isRealMode)
        {
            m_strTankName = strTankName;
            m_dTemperature = dTemperature;
            m_strSOPFullPath = strSOPFullPath;
        }

        public override int GetHeader()
        {
            return SOPWebServer.Header.RUN_SOP;
        }

        public override byte[] GetBytes()
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(m_isRealMode);
            arrDatas.Add(m_strSOPFullPath);
            arrDatas.Add("{location} : " + m_strTankName);
            arrDatas.Add("{temperature} : " + string.Format("double : {0:F1}", m_dTemperature));

            return SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
        }

        public override bool SendToSOPSimulator()
        {
            return true;
        }
    }
}