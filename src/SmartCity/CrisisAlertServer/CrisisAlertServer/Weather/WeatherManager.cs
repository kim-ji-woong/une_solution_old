using CrisisAlertServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrisisAlertServer.Weather
{
    public class WeatherManager
    {
        private FormMain m_form = null;
        private DataManager m_dataMgr = null;
        private WebServiceManager m_webServiceMgr = null;
        private Thread m_WeatherThread = null;

        private bool m_shutdownThread = false;
        public void Shutdown()
        { 
            m_shutdownThread = true;
            m_WeatherThread.Abort();
        }

        private bool m_startThread = false;
        public void StartThread()
        {
            m_startThread = true;
            m_form.ShowTextMessage("기상청 서비스 시작");
        }
        public void StopThread()
        {
            m_startThread = false;
            m_form.ShowTextMessage("기상청 서비스 종료");
        }

        public WeatherManager (FormMain form)
        {
            m_form = form;
            m_dataMgr = m_form.DataManager;
            m_webServiceMgr = form.WebManager;

            m_WeatherThread = new Thread(new ThreadStart(WeatherThread));
            m_WeatherThread.Name = "Weather.Sender";
            m_WeatherThread.Start();
        }

        private void WeatherThread()
        {
            while (!m_shutdownThread)
            {
                if (m_startThread)
                {
                    LoadWeatherData();

                    Thread.Sleep(60 * 60 * 1000);
                }
            }
        }

        private bool LoadWeatherData()
        {
            // 없으면 기상청 데이터 받기
            //WriteMessage("기상청 데이터 조회 중...");
            m_form.ShowTextMessage("기상청 데이터 조회 중...");
            ReadMidWeatherData();
            ReadLongWeatherData();

            // 받은 데이터로 5일자 예상기온 생성
            DateTime dtToday = DateTime.Today;
            DateTime dtYesterday = dtToday.AddDays(-1);

            string strDate = dtToday.ToString("yyyyMMdd");
            LoadExpectTemp(strDate);

            strDate = dtYesterday.ToString("yyyyMMdd");
            if (!LoadExpectTemp(strDate))
            {
                // 어제 일자 예상기온 데이터가 있는지 확인 >> 없으면 오늘 예상기온 데이터로 생성
                ExceptionExpectTemp();
            }

            return true;
        }

        private bool ReadMidWeatherData()
        {
            List<DataMidWeather> listMidWeathers = new List<DataMidWeather>();

            DataMidTemp midTemp = null;
            string strDate = "";

            DateTime dtToday = DateTime.Today;
            DateTime dtDawn = new DateTime(dtToday.Year, dtToday.Month, dtToday.Day, 5, 0, 0);
            DateTime dtMorning = new DateTime(dtToday.Year, dtToday.Month, dtToday.Day, 11, 0, 0);
            DateTime dtAfternoon = new DateTime(dtToday.Year, dtToday.Month, dtToday.Day, 17, 0, 0);

            if (m_webServiceMgr.ReadMidWeather(listMidWeathers))
            {
                midTemp = new DataMidTemp();

                DateTime dtAnnounceTime = new DateTime();
                string strAnnounceTime = "";
                string strYear = "", strMonth = "", strDay = "", strHour = "", strMinute = "";


                if (listMidWeathers.Count > 0)
                {
                    strAnnounceTime = listMidWeathers[0].AnnounceTime;
                    strYear = strAnnounceTime.Substring(0, 4);
                    strMonth = strAnnounceTime.Substring(4, 2);
                    strDay = strAnnounceTime.Substring(6, 2);
                    strHour = strAnnounceTime.Substring(8, 2);
                    strMinute = strAnnounceTime.Substring(10, 2);

                    strDate = strYear + "년 " + strMonth + "월 " + strDay + "일";
                    dtAnnounceTime = new DateTime(Convert.ToInt32(strYear), Convert.ToInt32(strMonth), Convert.ToInt32(strDay), Convert.ToInt32(strHour), Convert.ToInt32(strMinute), 0);

                    if (DateTime.Compare(dtAnnounceTime, dtAfternoon) >= 0 || DateTime.Compare(dtAnnounceTime, dtDawn) < 0)
                    {
                        // 17시부터 ~익일 5시 이전
                        //0 : 오늘오후
                        //1 : 내일오전
                        //2 : 내일오후
                        //3 : 모레오전
                        //4 : 모레오후

                        midTemp.AnnounceTime = strAnnounceTime;
                        midTemp.AfterOneDay = listMidWeathers[2].Ta;
                        midTemp.AfterTwoDay = listMidWeathers[4].Ta;
                    }
                    else if (DateTime.Compare(dtAnnounceTime, dtDawn) >= 0 && DateTime.Compare(dtAnnounceTime, dtMorning) < 0)
                    {
                        // 5시부터 ~11시 이전
                        //0 : 오늘오전
                        //1 : 오늘오후
                        //2 : 내일오전
                        //3 : 내일오후
                        //4 : 모래오전
                        //5 : 모레오후

                        midTemp.AnnounceTime = strAnnounceTime;
                        midTemp.AfterOneDay = listMidWeathers[3].Ta;
                        midTemp.AfterTwoDay = listMidWeathers[5].Ta;
                    }
                    else if (DateTime.Compare(dtAnnounceTime, dtMorning) >= 0 && DateTime.Compare(dtAnnounceTime, dtAfternoon) < 0)
                    {
                        // 11시부터 ~ 17시 이전
                        //0 : 오늘오후
                        //1 : 내일오전
                        //2 : 내일오후
                        //3 : 모레오전
                        //4 : 모레오후

                        midTemp.AnnounceTime = strAnnounceTime;
                        midTemp.AfterOneDay = listMidWeathers[2].Ta;
                        midTemp.AfterTwoDay = listMidWeathers[4].Ta;
                    }
                }
            }

            if (midTemp == null)
            {
                //WriteMessage("단기예보 조회 실패, 서버 관리자에게 문의하세요.");
                m_form.ShowTextMessage("단기예보 조회 실패, 서버 관리자에게 문의하세요.");
                return false;
            }
            else
                //WriteMessage(strDate + " 단기예보 조회 성공");
                m_form.ShowTextMessage(strDate + " 단기예보 조회 성공");

            // 수집된 날짜 DB 체크 후 저장
            string strChkDate = midTemp.AnnounceTime;
            strChkDate = strChkDate.Substring(0, 8);

            DataMidTemp dataMidCheck = null;
            dataMidCheck = m_dataMgr.GetMidTemp(strChkDate);

            if (dataMidCheck == null)
                m_dataMgr.InsertMidTemp(midTemp);

            return true;
        }

        private bool ReadLongWeatherData()
        {
            DateTime dtNow = DateTime.Now;
            DateTime dtMorning = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 6, 0, 0);
            DateTime dtAfternoon = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 18, 0, 0);

            DataLongWeather longWeather = null;
            DataLongTemp longTemp = null;

            string strDate = "", strDateMessage = "";

            if (DateTime.Compare(dtNow, dtMorning) < 0)
            {
                // 오전6시 전이라면 전날 18시 데이터 받기
                DateTime dtYesterday = dtNow.AddDays(-1);
                //longWeather = new DataLongWeather();
                strDate = dtYesterday.ToString("yyyyMMdd1800");
                strDateMessage = dtYesterday.ToString("yyyy년 MM월 dd일");

                m_webServiceMgr.ReadLongWeather(strDate, out longWeather);
            }
            else //if (DateTime.Compare(dtNow, dtMorning) >= 0)
            {
                // 오전6시 이후이라면 당일 오전6시 데이터 받기
                //longWeather = new DataLongWeather();
                strDate = dtMorning.ToString("yyyyMMdd0600");
                strDateMessage = dtMorning.ToString("yyyy년 MM월 dd일");

                m_webServiceMgr.ReadLongWeather(strDate, out longWeather);
            }

            if (longWeather.AnnounceTime == "")
            {
                //WriteMessage("중기예보 조회 실패, 서버 관리자에게 문의하세요.");
                m_form.ShowTextMessage("중기예보 조회 실패, 서버 관리자에게 문의하세요.");
                return false;
            }
            else
                //WriteMessage(strDateMessage + " 중기예보 조회 성공");
                m_form.ShowTextMessage(strDateMessage + " 중기예보 조회 성공");


            longTemp = new DataLongTemp();
            longTemp.AnnounceTime = strDate;
            longTemp.AfterThreeDay = longWeather.TaMax3;
            longTemp.AfterFourDay = longWeather.TaMax4;
            longTemp.AfterFiveDay = longWeather.TaMax5;
            longTemp.AfterSixDay = longWeather.TaMax6;
            longTemp.AfterSevenDay = longWeather.TaMax7;
            longTemp.AfterEightDay = longWeather.TaMax8;
            longTemp.AfterNineDay = longWeather.TaMax9;
            longTemp.AfterTenDay = longWeather.TaMax10;

            // 수집된 날짜 DB 체크 후 저장
            string strChkDate = longTemp.AnnounceTime;
            strChkDate = strChkDate.Substring(0, 8);

            DataLongTemp dataLongCheck = null;
            dataLongCheck = m_dataMgr.GetLongTemp(strChkDate);

            if (dataLongCheck == null)
                m_dataMgr.InsertLongTemp(longTemp);

            return true;
        }

        private bool LoadExpectTemp(string strDate)
        {
            DataMidTemp dataMid = null;
            DataLongTemp dataLong = null;
            DataExpectTemp dataExpect = null;

            // 날짜 데이터 확인 후 진행
            dataExpect = m_dataMgr.GetExpectTemp(strDate);

            if (dataExpect != null)
                return true;

            dataMid = m_dataMgr.GetMidTemp(strDate);
            dataLong = m_dataMgr.GetLongTemp(strDate);

            if (dataMid != null && dataLong != null)
            {
                dataExpect = new DataExpectTemp();
                dataExpect.AnnounceTime = strDate;
                dataExpect.AfterOneDay = dataMid.AfterOneDay;
                dataExpect.AfterTwoDay = dataMid.AfterTwoDay;
                dataExpect.AfterThreeDay = dataLong.AfterThreeDay;
                dataExpect.AfterFourDay = dataLong.AfterFourDay;
                dataExpect.AfterFiveDay = dataLong.AfterFiveDay;
                dataExpect.AfterSixDay = dataLong.AfterSixDay;
            }
            else
                return false;

            m_dataMgr.InsertExpectTemp(dataExpect);

            return true;
        }

        private bool ExceptionExpectTemp()
        {
            DateTime dtToday = DateTime.Today;
            DateTime dtYesterday = dtToday.AddDays(-1);
            string strDate = dtToday.ToString("yyyyMMdd");

            DataExpectTemp dataExpectYesterday = null;
            DataExpectTemp dataExpectToday = null;
            dataExpectToday = m_dataMgr.GetExpectTemp(strDate);

            if (dataExpectToday == null)
                return false;

            strDate = dtYesterday.ToString("yyyyMMdd");
            dataExpectYesterday = new DataExpectTemp();
            dataExpectYesterday.AnnounceTime = strDate;
            dataExpectYesterday.AfterOneDay = dataExpectToday.AfterOneDay;
            dataExpectYesterday.AfterTwoDay = dataExpectToday.AfterOneDay;
            dataExpectYesterday.AfterThreeDay = dataExpectToday.AfterTwoDay;
            dataExpectYesterday.AfterFourDay = dataExpectToday.AfterThreeDay;
            dataExpectYesterday.AfterFiveDay = dataExpectToday.AfterFourDay;
            dataExpectYesterday.AfterSixDay = dataExpectToday.AfterFiveDay;

            m_dataMgr.InsertExpectTemp(dataExpectYesterday);

            return true;
        }
    }
}
