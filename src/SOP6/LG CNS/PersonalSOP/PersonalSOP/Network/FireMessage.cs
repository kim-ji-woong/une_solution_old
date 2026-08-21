using System.Collections;

namespace PersonalSOP.Network
{
    public class FireMessage : Message
    {
        private string m_strLocation = "";
        private string m_strSOPFullPath = "";
        private bool m_isRealMode = false;

        public string SOPFullPath
        {
            get { return m_strSOPFullPath; }
            set { m_strSOPFullPath = value; }
        }

        public FireMessage(string strLocation, string strSOPFullPath, bool isRealMode)
        {
            m_strLocation = strLocation;
            m_strSOPFullPath = strSOPFullPath;
            m_isRealMode = isRealMode;
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
            arrDatas.Add("{location} : " + m_strLocation);
            return SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
        }

        public override bool SendToSOPSimulator()
        {
            return true;
        }
    }
}