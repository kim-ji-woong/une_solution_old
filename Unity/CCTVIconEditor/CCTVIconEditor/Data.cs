using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVIconEditor
{
    public class Zone
    {
        private int m_nID = -1;
        private string m_strZoneName = "";
        private string m_strSceneName = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        public string SceneName
        {
            get { return m_strSceneName; }
            set { m_strSceneName = value; }
        }

        public override string ToString()
        {
            return m_strZoneName;
        }
    }

    public class CCTV
    {
        private int m_nID = -1;
        private string m_strCameraName = "";
        private Zone m_zone = null;
        private float x = 0.0f;
        private float y = 0.0f;
        private float z = 0.0f;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strCameraName; }
            set { m_strCameraName = value; }
        }

        public Zone Zone
        {
            get { return m_zone; }
            set { m_zone = value; }
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }
    }
}
