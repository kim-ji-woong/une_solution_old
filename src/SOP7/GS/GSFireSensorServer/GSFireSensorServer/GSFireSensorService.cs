using dnsCommunicateSopServer;
using dnsData.Sensor;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GSFireSensorServer
{
    partial class GSFireSensorService : ServiceBase
    {
        private const string ALARM_METHOD = "POST";

        private Thread m_thread = null;

        private bool m_shutdownThread = false;
        private bool m_bIsCurrentAlarm = false;

        private string m_strAlarmFireURL = null;

        private SopQueryManager m_SopQueryMgr = null;
        private CrawlingManager m_crawlingManager = null;

        public GSFireSensorService()
        {
            InitializeComponent();

            string strErrorMessage = "";

            m_crawlingManager = new CrawlingManager();

            if (m_crawlingManager.InitCrawling(out strErrorMessage) == false)
            {
                Trace.WriteLine(strErrorMessage);
                Logger.Instance.Write("InitCrawling 오류 " + strErrorMessage);

                return;
            }

            Init();
        }

        private void Init()
        {
            string strAlarmFireURL = ConfigurationManager.AppSettings.Get("ALARM_FIRE_URL");
            if (strAlarmFireURL == null || strAlarmFireURL.Length == 0)
                strAlarmFireURL = "http://127.0.0.1:44379/api/FireSensor";

            m_strAlarmFireURL = strAlarmFireURL;

            m_SopQueryMgr = new SopQueryManager();

            return;
        }

        public void Shutdown()
        {
            m_shutdownThread = true;
            m_thread.Abort();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 여기에 서비스를 시작하는 코드를 추가합니다.

            m_thread = new Thread(new ThreadStart(FireSensorReadThread));
            m_thread.Name = "FireSensor Tester";
            m_thread.Start();
        }

        private void FireSensorReadThread()
        {
            while (!m_shutdownThread)
            {
                string strErrorMessage = "";

                if (m_crawlingManager.ConnectVitconSite(out strErrorMessage) == false)
                {
                    // 에러 로그 기록
                    Logger.Instance.Write("ConnectVitconSite 오류 " + strErrorMessage);
                    Thread.Sleep(5000);
                    continue;
                }

                while (!m_shutdownThread)
                {
                    CrawlingManager.StateType stateType = m_crawlingManager.ReadFireSensorData(out strErrorMessage);

                    if (stateType == CrawlingManager.StateType.Error)
                    {
                        // 에러 로그 기록
                        Logger.Instance.Write("ReadFireSensorData 오류 " + strErrorMessage);
                        Thread.Sleep(5000);
                        break;
                    }
                    else if (stateType == CrawlingManager.StateType.Alarm)
                    {   // 화재 발생
                        m_bIsCurrentAlarm = true;
                        Console.WriteLine("화재 발생!!");

                        // 알람 신호 
                        ArrayList arrData = new ArrayList();
                        arrData.Add((int)Facility.FacilityType.FIRE_SENSOR);
                        arrData.Add(1);
                        arrData.Add(1);
                        arrData.Add(true);

                        m_SopQueryMgr.SendAlarmQuery(arrData, ALARM_METHOD, m_strAlarmFireURL);
                    }
                    else if (stateType == CrawlingManager.StateType.Normal && m_bIsCurrentAlarm == true)
                    {   // 화재 종료 첫 신호
                        m_bIsCurrentAlarm = false;
                        Console.WriteLine("화재 종료");

                        // 알람 종료 신호
                        ArrayList arrData = new ArrayList();
                        arrData.Add((int)Facility.FacilityType.FIRE_SENSOR);
                        arrData.Add(1);
                        arrData.Add(1);
                        arrData.Add(false);

                        m_SopQueryMgr.SendAlarmQuery(arrData, ALARM_METHOD, m_strAlarmFireURL);
                    }

                    Thread.Sleep(300);
                }
            }
        }

        protected override void OnStop()
        {
            // TODO: 서비스를 중지하는 데 필요한 작업을 수행하는 코드를 여기에 추가합니다.
            Shutdown();
            m_crawlingManager.Quit();
        }
    }
}
