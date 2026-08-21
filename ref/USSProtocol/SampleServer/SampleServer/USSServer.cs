using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using System.Collections;
using libUSS;

namespace SampleServer
{
    public class USSServer
    {
        private TcpServer m_server = null;
        private ServiceProvider m_provider = null;
        private int m_nPort = 0;
        private bool m_isOpened = false;
        private IServiceOwner m_owner = null;

        public ServiceProvider Provider
        {
            get { return m_provider; }
        }

        public USSServer(int nPort, IServiceOwner owner)
        {
            m_nPort = nPort;
            m_owner = owner;
        }

        public bool BeginServer()
        {
            if (m_nPort > 0)
            {
                if (m_provider != null)
                {
                    m_provider.ReleaseThread();
                }

                m_provider = new ServiceProvider(m_owner);

                m_server = new TcpServer(m_provider, m_nPort);
                m_server.ConnectionLog = ConnectionLogClient.Instance;
                m_isOpened = m_server.Start();
            }

            return m_isOpened;
        }

        public void StopServer()
        {
            if (m_provider != null)
            {
                if (m_isOpened)
                {
                    m_server.Stop();
                    m_isOpened = false;
                }

                m_provider.ReleaseThread();
                m_provider = null;
            }
        }

        public void SendFireSignal(bool fireOn, int nSensorID, DateTime timeStamp)
        {
            byte on = fireOn ? (byte)1 : (byte)0;
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(on);
            arrDatas.Add(nSensorID);
            arrDatas.Add(timeStamp);
            byte[] bytes = BinaryHelper.MakeBytes(Header.FIRE_SENSOR_DATA, arrDatas);

            if (bytes != null && m_provider != null)
            {
                m_provider.Send(bytes, 0, bytes.Length, EventType.Fire);
            }
        }

        public void SendPowerOffSignal(bool powerOff, int nBuildingID, int nSpaceID, DateTime timeStamp)
        {
            byte on = powerOff ? (byte)1 : (byte)0;
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(on);
            arrDatas.Add(nBuildingID);
            arrDatas.Add(nSpaceID);
            arrDatas.Add(timeStamp);
            byte[] bytes = BinaryHelper.MakeBytes(Header.POWER_OFF_DATA, arrDatas);

            if (bytes != null && m_provider != null)
            {
                m_provider.Send(bytes, 0, bytes.Length, EventType.PowerOff);
            }
        }

        public void SendEarthquakeSignal(int nIntensity, DateTime timeStamp)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(EarthquakeDataType.Intensity);
            arrDatas.Add(nIntensity);
            arrDatas.Add(timeStamp);
            byte[] bytes = BinaryHelper.MakeBytes(Header.EARTH_QUAKE_DATA, arrDatas);

            if (bytes != null && m_provider != null)
            {
                m_provider.Send(bytes, 0, bytes.Length, EventType.Earthquake);
            }
        }

        public void SendWindSignal(int nSensorID, float fWindSpeed, DateTime timeStamp)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nSensorID);
            arrDatas.Add(fWindSpeed);
            arrDatas.Add(timeStamp);
            byte[] bytes = BinaryHelper.MakeBytes(Header.WIND_SENSOR_DATA, arrDatas);

            if (bytes != null && m_provider != null)
            {
                m_provider.Send(bytes, 0, bytes.Length, EventType.Wind);
            }
        }
    }
}
