using System;

namespace Weather.Model
{
    public class Weekly
    {
        public enum Fields { WeatherSiteID, OneDayLaterTemp, OneDayLaterState, TwoDayLaterTemp, TwoDayLaterState, ThreeDayLaterTemp, ThreeDayLaterState, FourDayLaterTemp, FourDayLaterState, FiveDayLaterTemp, FiveDayLaterState, SixDayLaterTemp, SixDayLaterState, UpdateTime };
        // 알수없음, 맑음, 천둥번개, 진눈깨비, 폭설, 눈, 폭우, 비, 흐림, 구름조금, 황사, 미세먼지
        public enum WeatherState { Unknown = 0, Sunshine, Thunder, SnowRain, HeavySnow, Snow, HeavyRain, Rain, Cloudy, Cloud, DustStorm, FineDust };

        private int m_nSiteID = -1;
        private float m_fOneDayLaterTemp = 0;
        private int m_nOneDayLaterState = (int)WeatherState.Unknown;
        private float m_fTwoDayLaterTemp = 0;
        private int m_nTwoDayLaterState = (int)WeatherState.Unknown;
        private float m_fThreeDayLaterTemp = 0;
        private int m_nThreeDayLaterState = (int)WeatherState.Unknown;
        private float m_fFourDayLaterTemp = 0;
        private int m_nFourDayLaterState = (int)WeatherState.Unknown;
        private float m_fFiveDayLaterTemp = 0;
        private int m_nFiveDayLaterState = (int)WeatherState.Unknown;
        private float m_fSixDayLaterTemp = 0;
        private int m_nSixDayLaterState = (int)WeatherState.Unknown;
        private DateTime m_dtUpdate = new DateTime();

        public int WeatherSiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public float OneDayLaterTemp
        {
            get { return m_fOneDayLaterTemp; }
            set { m_fOneDayLaterTemp = value; }
        }

        public int OneDayLaterState
        {
            get { return m_nOneDayLaterState; }
            set { m_nOneDayLaterState = value; }
        }

        public float TwoDayLaterTemp
        {
            get { return m_fTwoDayLaterTemp; }
            set { m_fTwoDayLaterTemp = value; }
        }

        public int TwoDayLaterState
        {
            get { return m_nTwoDayLaterState; }
            set { m_nTwoDayLaterState = value; }
        }

        public float ThreeDayLaterTemp
        {
            get { return m_fThreeDayLaterTemp; }
            set { m_fThreeDayLaterTemp = value; }
        }

        public int ThreeDayLaterState
        {
            get { return m_nThreeDayLaterState; }
            set { m_nThreeDayLaterState = value; }
        }

        public float FourDayLaterTemp
        {
            get { return m_fFourDayLaterTemp; }
            set { m_fFourDayLaterTemp = value; }
        }

        public int FourDayLaterState
        {
            get { return m_nFourDayLaterState; }
            set { m_nFourDayLaterState = value; }
        }

        public float FiveDayLaterTemp
        {
            get { return m_fFiveDayLaterTemp; }
            set { m_fFiveDayLaterTemp = value; }
        }

        public int FiveDayLaterState
        {
            get { return m_nFiveDayLaterState; }
            set { m_nFiveDayLaterState = value; }
        }

        public float SixDayLaterTemp
        {
            get { return m_fSixDayLaterTemp; }
            set { m_fSixDayLaterTemp = value; }
        }

        public int SixDayLaterState
        {
            get { return m_nSixDayLaterState; }
            set { m_nSixDayLaterState = value; }
        }


        public DateTime UpdateTime
        {
            get { return m_dtUpdate; }
            set { m_dtUpdate = value; }
        }

        public static string TableName
        {
            get { return "WeatherWeekly"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
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
