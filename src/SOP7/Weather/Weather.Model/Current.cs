using System;

namespace Weather.Model
{
    public class Current
    {
        public enum Fields { WeatherSiteID, State, Temperature, SensibleTemp, Rain, Humidity, WindSpeed, WindDirection, Atm, UpdateTime };
        // 알수없음, 맑음, 천둥번개, 진눈깨비, 폭설, 눈, 폭우, 비, 흐림, 구름조금, 황사, 미세먼지
        public enum WeatherState { Unknown = 0, Sunshine, Thunder, SnowRain, HeavySnow, Snow, HeavyRain, Rain, Cloudy, Cloud, DustStorm, FineDust };

        private int m_nSiteID = -1;
        private int m_nState = (int)WeatherState.Unknown;
        private float m_fTemp = 0;
        private float? m_fSensibleTemp = null;
        private float m_fRain = 0;
        private float m_fHumidity = 0;
        private float? m_fWindSpeed = null;
        private int? m_nWindDir = null;
        private float? m_fAtm = null;
        private DateTime m_dtUpdate = new DateTime();

        public int WeatherSiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public int State
        {
            get { return m_nState; }
            set { m_nState = value; }
        }

        public float Temperature
        {
            get { return m_fTemp; }
            set { m_fTemp = value; }
        }

        public float? SensibleTemp
        {
            get { return m_fSensibleTemp; }
            set { m_fSensibleTemp = value; }
        }

        public float Rain
        {
            get { return m_fRain; }
            set { m_fRain = value; }
        }

        public float Humidity
        {
            get { return m_fHumidity; }
            set { m_fHumidity = value; }
        }

        public float? WindSpeed
        {
            get { return m_fWindSpeed; }
            set { m_fWindSpeed = value; }
        }

        public int? WindDirection
        {
            get { return m_nWindDir; }
            set { m_nWindDir = value; }
        }

        public float? Atm
        {
            get { return m_fAtm; }
            set { m_fAtm = value; }
        }

        public DateTime UpdateTime
        {
            get { return m_dtUpdate; }
            set { m_dtUpdate = value; }
        }

        public static string TableName
        {
            get { return "WeatherCurrent"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.SensibleTemp ||
                field == Fields.WindSpeed ||
                field == Fields.WindDirection ||
                field == Fields.Atm)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string StateToString(int state)
        {
            switch (state)
            {
                case (int)WeatherState.Sunshine:
                    return "맑음";

                case (int)WeatherState.Thunder:
                    return "천둥번개";

                case (int)WeatherState.SnowRain:
                    return "진눈깨비";

                case (int)WeatherState.HeavySnow:
                    return "강한 눈";

                case (int)WeatherState.Snow:
                    return "눈";

                case (int)WeatherState.HeavyRain:
                    return "강한 비";

                case (int)WeatherState.Rain:
                    return "비";

                case (int)WeatherState.Cloudy:
                    return "흐림";

                case (int)WeatherState.Cloud:
                    return "구름 조금";

                case (int)WeatherState.DustStorm:
                    return "황사";

                case (int)WeatherState.FineDust:
                    return "미세먼지";
            }

            return "알수없음";
        }
    }
}
