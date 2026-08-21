using System.Collections.Generic;

namespace Weather.BLL.Models.Response
{
    using Model;

    public class ResponseWeatherInfo : MessageResult
    {
        private List<WeatherData> m_datas = new List<WeatherData>();

        public List<WeatherData> Datas
        {
            get { return m_datas; }
            set { m_datas = value; }
        }

        public ResponseWeatherInfo()
        {
        }

        public ResponseWeatherInfo(bool success, string strMessage)
        {
            Success = success;
            Message = strMessage;
        }
    }

    public class WeatherData
    {
        private Site m_site = null;
        private Current m_current = null;
        private SpecialReport m_specialReport = null;

        public Site Site
        {
            get { return m_site; }
            set { m_site = value; }
        }

        public Current Current
        {
            get { return m_current; }
            set { m_current = value; }
        }

        public SpecialReport SpecialReport
        {
            get { return m_specialReport; }
            set { m_specialReport = value; }
        }
    }
}
