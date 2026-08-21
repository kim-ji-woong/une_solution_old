using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherSimulator
{
    // struct와 같이 null이 허용되지 않는 데이터를 위한 Wrapper 클래스
    public class VariousData<DataType>
    {
        private DataType data;

        public DataType Data
        {
            get { return data; }
            set { data = value; }
        }

        public VariousData()
        {
        }

        public VariousData(DataType data)
        {
            this.data = data;
        }
    }

    public class WeatherData
    {
        public enum DataType { RainNWind = 0, Typhoon, Earthquake };

        protected int m_nID = -1;
        // 기상 데이터들이 발생했거나 발생할 것으로 예측되는 시간
        protected DateTime m_time;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        // 기상 데이터들이 발생했거나 발생할 것으로 예측되는 시간
        public DateTime Time
        {
            get { return m_time; }
            set { m_time = value; }
        }

        public string GetTimeString()
        {
            return MakeTimeString(m_time);
        }

        public static string MakeTimeString(DateTime time)
        {
            return string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}",
                time.Year, time.Month, time.Day, time.Hour, time.Minute);
        }
    }

    public class RainNWind : WeatherData
    {
        // 시간당 강우량(mm/h)
        private VariousData<float> m_fRainHour = null;
        // 일별 강우량(mm/h)
        private VariousData<float> m_fRainDay = null;
        // 평균 풍속(m/sec)
        private VariousData<float> m_fWindSpeedAve = null;
        // 순간 최대 풍속(m/sec)
        private VariousData<float> m_fWindSpeedMax = null;
        // 강우 발생 지역
        private string m_strRegion = null;

        // 시간당 강우량(mm/h)
        public VariousData<float> RainHour
        {
            get { return m_fRainHour; }
            set { m_fRainHour = value; }
        }

        // 일별 강우량(mm/h)
        public VariousData<float> RainDay
        {
            get { return m_fRainDay; }
            set { m_fRainDay = value; }
        }

        // 평균 풍속(m/sec)
        public VariousData<float> WindSpeedAve
        {
            get { return m_fWindSpeedAve; }
            set { m_fWindSpeedAve = value; }
        }

        // 순간 최대 풍속(m/sec)
        public VariousData<float> WindSpeedMax
        {
            get { return m_fWindSpeedMax; }
            set { m_fWindSpeedMax = value; }
        }

        // 강우 발생 지역
        public string Region
        {
            get { return m_strRegion; }
            set { m_strRegion = value; }
        }
    }

    public class Typhoon : WeatherData
    {
        public enum Direction
        {
            North = 0,
            NNEast,
            NorthEast,
            ENorthE,
            East,
            ESouthE,
            SouthEast,
            SSEast,
            South,
            SSWest,
            SouthWest,
            WSouthW,
            West,
            WNorthW,
            NorthWest,
            NNWest
        }

        // 태풍의 중심 위치
        private string m_strCenterLocation = null;
        // 태풍 중심 기압(hPa)
        private VariousData<float> m_fCenterPressure = null;
        // 최대 풍속(m/sec)
        private VariousData<float> m_fMaxSpeed = null;
        // 태풍 반경(km)
        private VariousData<float> m_fWindRadius = null;
        // 태풍의 진행 방향
        private VariousData<Direction> m_windDir = null;
        // 태풍의 진행속도(km/hour)
        private VariousData<float> m_fMoveSpeed = null;
        private string m_strEtc = null;

        // 태풍의 중심 위치
        public string CenterLocation
        {
            get { return m_strCenterLocation; }
            set { m_strCenterLocation = value; }
        }

        // 태풍 중심 기압(hPa)
        public VariousData<float> CenterPressure
        {
            get { return m_fCenterPressure; }
            set { m_fCenterPressure = value; }
        }

        // 최대 풍속(m/sec)
        public VariousData<float> MaxSpeed
        {
            get { return m_fMaxSpeed; }
            set { m_fMaxSpeed = value; }
        }

        // 태풍 반경(km)
        public VariousData<float> WindRadius
        {
            get { return m_fWindRadius; }
            set { m_fWindRadius = value; }
        }

        // 태풍의 진행 방향
        public VariousData<Direction> WindDirection
        {
            get { return m_windDir; }
            set { m_windDir = value; }
        }

        // 태풍의 진행속도(km/hour)
        public VariousData<float> MoveSpeed
        {
            get { return m_fMoveSpeed; }
            set { m_fMoveSpeed = value; }
        }

        public string Etc
        {
            get { return m_strEtc; }
            set { m_strEtc = value; }
        }

        public static bool ToDirection(int nDirection, out Direction dir)
        {
            dir = Direction.North;

            if (nDirection < (int)Direction.North || nDirection > (int)Direction.NNWest)
                return false;

            dir = (Direction)nDirection;
            return true;
        }
    }

    public class Earthquake : WeatherData
    {
        // 지진 발생장소
        private string m_strLocation = null;
        // 진도(0 ~ 10)
        private VariousData<float> m_fStrength = null;
        // 지진해일 높이(meter)
        private VariousData<float> m_fTsunamiHeight = null;
        private string m_strEtc = null;

        // 지진 발생장소
        public string Location
        {
            get { return m_strLocation; }
            set { m_strLocation = value; }
        }

        // 진도(0 ~ 10)
        public VariousData<float> Strength
        {
            get { return m_fStrength; }
            set { m_fStrength = value; }
        }

        // 지진해일 높이(meter)
        public VariousData<float> TsunamiHeight
        {
            get { return m_fTsunamiHeight; }
            set { m_fTsunamiHeight = value; }
        }

        public string Etc
        {
            get { return m_strEtc; }
            set { m_strEtc = value; }
        }
    }
}
