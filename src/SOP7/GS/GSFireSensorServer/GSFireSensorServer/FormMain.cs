using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using dnsCommunicateSopServer;
using System.Collections;
using dnsData.Sensor;
using System.Configuration;

namespace GSFireSensorServer
{

    public partial class FormMain : Form
    {

        private const string ALARM_METHOD = "POST";

        private Thread m_thread = null;

        private bool m_shutdownThread = false;
        private bool m_bIsCurrentAlarm = false;

        private string m_strAlarmFireURL = null;

        private SopQueryManager m_SopQueryMgr = null;
        private CrawlingManager m_crawlingManager = null;

        private DateTime m_dtLast = new DateTime();

        public FormMain()
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

            m_thread = new Thread(new ThreadStart(FireSensorReadThread));
            m_thread.Name = "FireSensor Tester";
            m_thread.Start();

            return;
        }

        public void Shutdown()
        {
            m_shutdownThread = true;
            m_thread.Abort();
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
                    // 지난 로그 삭제
                    DateTime dtNow = DateTime.Now;
                    if ((dtNow - m_dtLast).TotalDays >= 1)
                    {
                        Logger.Instance.RemoveOldLogs();
                        m_dtLast = DateTime.Now;
                    }

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

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Shutdown();
            m_crawlingManager.Quit();
        }
    }
}
