using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarthquakeSensorServer
{
    class EarthquakeData
    {
        private int m_nIntensity = 0;
        private float m_fMagnitude = -1.0f;
        private string m_strLocation = "";
        private DateTime m_timeStamp = new DateTime();

        public int Intensity
        {
            get { return m_nIntensity; }
            set { m_nIntensity = value; }
        }

        public float Magnitude
        {
            get { return m_fMagnitude; }
            set { m_fMagnitude = value; }
        }

        public string Location
        {
            get { return m_strLocation; }
            set { m_strLocation = value; }
        }

        public DateTime TimeStamp
        {
            get { return m_timeStamp; }
            set { m_timeStamp = value; }
        }
    }
}
