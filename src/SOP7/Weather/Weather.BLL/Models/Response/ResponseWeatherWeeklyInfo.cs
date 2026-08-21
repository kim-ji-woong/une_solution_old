using System.Collections.Generic;

namespace Weather.BLL.Models.Response
{
    using Model;

    public class ResponseWeatherWeeklyInfo : MessageResult
    {
        private List<WeatherWeeklyData> m_datas = null;

        public List<WeatherWeeklyData> Datas
        {
            get { return m_datas; }
            set { m_datas = value; }
        }
    }

    public class WeatherWeeklyData
    {
        private Site m_site = null;
        private Weekly m_weekly = null;

        public Site Site
        {
            get { return m_site; }
            set { m_site = value; }
        }

        public Weekly Weekly
        {
            get { return m_weekly; }
            set { m_weekly = value; }
        }
    }
}
