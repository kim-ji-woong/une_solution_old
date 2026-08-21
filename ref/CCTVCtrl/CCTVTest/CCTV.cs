using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnECCTV
{
    public class CCTV
    {
        private int m_nID = 0;
        private string m_strCameraName = "";
        private string m_strChannel1URL = "";
        private string m_strChannel2URL = "";
        private string m_strChannel3URL = "";

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

        public string Channel1URL
        {
            get { return m_strChannel1URL; }
            set { m_strChannel1URL = value; }
        }

        public string Channel2URL
        {
            get { return m_strChannel2URL; }
            set { m_strChannel2URL = value; }
        }

        public string Channel3URL
        {
            get { return m_strChannel3URL; }
            set { m_strChannel3URL = value; }
        }
    }
}
