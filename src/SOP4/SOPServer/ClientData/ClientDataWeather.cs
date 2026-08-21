using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using SDMS;
using System.Collections;

namespace SDMSServer
{
    public class ClientDataWeather : ClientData
    {
        public enum WeatherInfoType { UpdateData = 0 };

        public ClientDataWeather(ServiceProvider provider)
        {
            m_provider = provider;
            ClientType = TCP_CLIENT.SOP_WEATHER;
        }

		protected override bool ProcessFirstConnection(ConnectionState state)
		{
			return true;
		}

        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            if (nHeader == TCP_ID.WEATHER_INFO)
            {
                SetWeatherInfo(arrDatas, bytes);
            }

            return true;
        }

        private void SetWeatherInfo(ArrayList arrDatas, byte[] bytes)
        {
            if (arrDatas.Count == 0 || (arrDatas[0] is int) == false)
                return;

            int nDataType = (int)arrDatas[0];

            if (nDataType == (int)WeatherInfoType.UpdateData)
            {
                m_provider.SendData(bytes, false, TCP_CLIENT.SDMS_CLIENT);
            }
        }
    }
}
