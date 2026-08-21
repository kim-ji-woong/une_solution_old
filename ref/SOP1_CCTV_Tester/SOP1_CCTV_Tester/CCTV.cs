using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOP1_CCTV_Tester
{
    public class CCTV
    {
        private int m_nID = -1;
        private string m_strCameraName = "";
        private string m_strIP = "";
        private string m_strPort = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string CameraName
        {
            get { return m_strCameraName; }
            set { m_strCameraName = value; }
        }

        public string IP
        {
            get { return m_strIP; }
            set { m_strIP = value; }
        }

        public string Port
        {
            get { return m_strPort; }
            set { m_strPort = value; }
        }

        public override string ToString()
        {
            return CameraName;
        }

        public CCTV()
        {
        }

        public CCTV(int nID, string strCameraName, string strIP, string strPort)
        {
            m_nID = nID;
            m_strCameraName = strCameraName;
            m_strIP = strIP;
            m_strPort = strPort;
        }
    }
}
