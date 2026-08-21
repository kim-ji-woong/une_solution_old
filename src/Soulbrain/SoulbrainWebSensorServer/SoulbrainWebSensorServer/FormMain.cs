using DBUtility2;
using dnsDBUtil;
using SDMS.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace SoulbrainWebSensorServer
{
    public partial class FormMain : Form
    {
        private WebServiceManager m_webServiceMgr = null;
        //private WebDBManager m_dbManager = null;
        private dnsDBUtil.WebDBManager m_HrDBManager = null;
        private DataManager m_dataManager = null;
        private TeamEditor.DAL.DataManager m_memberDataManager = null;
        private Dashboard.DAL.DataManager m_dashboardDataManager = null;
        private Common.DAL.DataManager m_commonDataManager = null;
        private SOPManager.DAL.DataManager m_sopDataManager = null;

        private TeamEditor.BLL.ProcessManager m_processManager = null;

        private DirectDBManager m_wishDBManager = null;
        private WishDataManager m_wishDataManager = null;

        private SynchroManager m_synchroManager = null;
        

        private WSopDataManager m_wsopDataMgr = null;

        private List<DataDevice> m_listDevice = null;
        private bool m_shutdownThread = false;
        private int m_nShutdownThread = 0;                          // 쓰레드 실행 유무 판단 변수

        private int m_nThreadDeviceNum = 6;                         // 쓰레드 당 감시할 디바이스 갯수
        private int m_nThreadReloadSleep = 100 * 20;                // 쓰레드 다시 불러올 때 슬립타임
        private int m_nThreadSleep = 100 * 6;                       // 쓰레드 슬립타임
        private int m_nThreadWishSleep = 1000 * 60 * 5;             // WISH 쓰레드 슬립타임

        private System.Timers.Timer m_timerReload = null;           // 로그인 및 디바이스 조회 타이머
        private bool m_bTimerChk = false;                           // 이미 타이머 실행 유무 체크

        //LogManager m_logMgr = new LogManager();

        private DateTime m_dtLast = new DateTime();

        public FormMain()
        {
            InitializeComponent();

            // Soulbrain Web Rest API 관련 매니저
            m_webServiceMgr = new WebServiceManager();

            // WSOP DB 매니저
            InitDBSet();
            m_wsopDataMgr = new WSopDataManager(m_dataManager, m_memberDataManager);

            m_webServiceMgr.WSopDataMgr = m_wsopDataMgr;

            // 서버 소스
            m_timerReload = new System.Timers.Timer();
            m_timerReload.Interval = 1000 * 60 * 50;       // 1분(1초 * 60) * 50 = 50분
            //m_timerReload.Interval = 1000 * 60 * 1;       // 1분(1초 * 60) * 1 = 1분
            m_timerReload.Elapsed += new ElapsedEventHandler(timerReload_Elapsed);

            m_timerReload.Start();
            timerReload_Elapsed(null, null);

            Thread WatchWish = new Thread(() => WatchWishThread());
            WatchWish.Start();


            // HR 조직 정보 동기화
            m_processManager = new TeamEditor.BLL.ProcessManager(m_commonDataManager, m_memberDataManager, m_sopDataManager, m_dataManager);
            m_synchroManager = new SynchroManager(m_HrDBManager, m_memberDataManager, m_processManager);
            m_synchroManager.StartThread();
        }

        private void timerReload_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool bChk = false;

            // 타이머 실행 유무 체크
            if (m_bTimerChk == true)
                return;

            // 지난 로그 삭제
            DateTime dtNow = DateTime.Now;
            if ((dtNow - m_dtLast).TotalDays >= 1)
            {
                Logger.Instance.RemoveOldLogs();
                m_dtLast = DateTime.Now;
            }

            m_bTimerChk = true;                 // 타이머 실행 중 체크
            //m_shutdownThread = true;            // 실행 중인 쓰레드 중지

            m_nShutdownThread++;                // 실행 중인 쓰레드 중지
            if (m_nShutdownThread > 100)
                m_nShutdownThread = 1;

            Thread.Sleep(m_nThreadReloadSleep);       // 실행 중인 쓰레드 종료 시간

            while (!bChk)
            {
                // 로그인
                bChk = m_webServiceMgr.RequestLogin();
                if (bChk == false)
                {
                    //m_logMgr.Log_Info("Login Rest API 실패. 네트워크 확인바람.");
                    Logger.Instance.Write("Login Rest API 실패. 네트워크 확인바람.");
                    // 1분 후 재실행
                    Thread.Sleep(1000 * 60);
                    continue;
                }

                // 디바이스 조회
                bChk = m_webServiceMgr.RequestDeviceList();
                if (bChk == false)
                {
                    //m_logMgr.Log_Info("Device List 조회 Rest API 실패. 네트워크 확인바람.");
                    Logger.Instance.Write("Device List 조회 Rest API 실패. 네트워크 확인바람.");
                    // 1분 후 재실행
                    Thread.Sleep(1000 * 60);
                    continue;
                }

                // 디바이스 센서 데이터 조회 쓰레드 생성
                bChk = ReloadSensorThread();
                if (bChk == false)
                {
                    //m_logMgr.Log_Info("조회된 Device가 없어 실패.");
                    Logger.Instance.Write("Device List 조회 Rest API 실패. 네트워크 확인바람.");
                    // 1분 후 재실행
                    Thread.Sleep(1000 * 60);
                    continue;
                }
            }

            m_bTimerChk = false;
        }

        private bool ReloadSensorThread()
        {
            int nShutdownThread = -1;

            Dictionary<string, DataDevice> dicDevices = m_webServiceMgr.DicDevices;
            if (dicDevices == null || dicDevices.Count == 0)
                return false;

            // 쓰레드 실행
            //m_shutdownThread = false;
            nShutdownThread = m_nShutdownThread;

            m_listDevice = new List<DataDevice>(dicDevices.Values);

            for (int i = 0; i < m_listDevice.Count; i += m_nThreadDeviceNum)
            {
                int nIdx = i;

                Thread WatchDevice = new Thread(() => WatchDeviceThread(nIdx, nShutdownThread));
                WatchDevice.Start();
            }

            return true;
        }

        // ETC Sensor Data 업데이트 쓰레드
        private void WatchDeviceThread(int nNum, int nShutdownThread)
        {
            Console.WriteLine("create Thread: " + nNum.ToString());

            //while (!m_shutdownThread)
            while (m_nShutdownThread == nShutdownThread)
            {
                for (int i = nNum; i < nNum + m_nThreadDeviceNum; i++)
                {
                    if (i > m_listDevice.Count - 1)
                        break;
                    //Console.WriteLine("i: " + i.ToString());
                    DataDevice data = m_listDevice[i];

                    m_webServiceMgr.RequestSensorData(data);
                    m_wsopDataMgr.UpdateETCSensor(data);
                }

                Thread.Sleep(m_nThreadSleep);
            }

            Console.WriteLine("shutdownThread: " + nNum.ToString());
        }

        private void InitDBSet()
        {
            string strSiteID = ConfigurationManager.AppSettings.Get("SITE_ID");
            if (strSiteID == null || strSiteID.Length == 0)
                strSiteID = "10";

            string strDBName = ConfigurationManager.AppSettings.Get("DB_NAME");
            if (strDBName == null || strDBName.Length == 0)
                strDBName = "WSOP_10";

            string strDBType = ConfigurationManager.AppSettings.Get("DB_TYPE");
            if (strDBType == null || strDBType.Length == 0)
                strDBType = "0";

            string strWebServerURL = ConfigurationManager.AppSettings.Get("WebServerURL");
            if (strWebServerURL == null || strWebServerURL.Length == 0)
                strWebServerURL = "http://127.0.0.1:808";

            string strWishDBName = ConfigurationManager.AppSettings.Get("WISH_NAME");
            if (strWishDBName == null || strWishDBName.Length == 0)
                strWishDBName = "ESH_DB";

            string strWishDBType = ConfigurationManager.AppSettings.Get("WISH_TYPE");
            if (strWishDBType == null || strWishDBType.Length == 0)
                strWishDBType = "0";

            string strWishDBId = ConfigurationManager.AppSettings.Get("WISH_ID");
            if (strWishDBId == null || strWishDBId.Length == 0)
                strWishDBId = "wesh";

            string strWishDBPW = ConfigurationManager.AppSettings.Get("WISH_PW");
            if (strWishDBPW == null || strWishDBPW.Length == 0)
                strWishDBPW = "techn0$b";

            string strWishDBUrl = ConfigurationManager.AppSettings.Get("WISH_URL");
            if (strWishDBUrl == null || strWishDBUrl.Length == 0)
                strWishDBUrl = "192.168.11.58";


            string strHrDBName = ConfigurationManager.AppSettings.Get("HR_NAME");
            if (strHrDBName == null || strHrDBName.Length == 0)
                strHrDBName = "Soulbrain_HR";


            int nSiteID, nDBType;
            int.TryParse(strSiteID.Trim(), out nSiteID);
            int.TryParse(strDBType.Trim(), out nDBType);

            int nWishDBType;
            int.TryParse(strWishDBType.Trim(), out nWishDBType);

            //m_dbManager = new WebDBManager(strDBName, nDBType, nSiteID, strWebServerURL);
            m_dataManager = new DataManager(strDBName, nDBType, nSiteID, strWebServerURL);
            m_memberDataManager = new TeamEditor.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL);
            m_dashboardDataManager = new Dashboard.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL);
            m_commonDataManager = new Common.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL);
            m_sopDataManager = new SOPManager.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL);

            m_wishDBManager = DirectDBManager.MakeInstance((DBUtility2.DirectDBManager.DBType)nWishDBType, strWishDBUrl, strWishDBId, strWishDBPW, strWishDBName);
            m_wishDataManager = new WishDataManager(m_wishDBManager, m_dashboardDataManager);

            m_HrDBManager = new dnsDBUtil.WebDBManager(strHrDBName, nDBType, nSiteID, strWebServerURL);
        }

        private void WatchWishThread()
        {
            while (!m_shutdownThread)
            {
                string strErrorMessage = "";

                if (m_wishDataManager.ReloadCurrentWorkPermitData(out strErrorMessage) == false)
                {
                    Console.WriteLine(strErrorMessage);
                }

                Thread.Sleep(m_nThreadWishSleep);
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_shutdownThread = true;            // 실행 중인 쓰레드 중지
            m_nShutdownThread = -1;
        }
    }
}
