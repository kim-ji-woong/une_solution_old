using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PSensorServer
{
    public class KPXServerManager
    {
        private static KPXServerManager m_Instance = null;
        public static KPXServerManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new KPXServerManager();
                return KPXServerManager.m_Instance; 
            }
           
        }

        private KPXServerManager()
        {
            ReadConfig();

            m_dbMgr = new DBUtility.WebDBManager(m_nSiteID);
            m_dbMgr.DatabaseHost = "127.0.0.1";
            
        }

        private JubixNetworkCommander m_Commander = null;

        public JubixNetworkCommander Commander
        {
            get { return m_Commander; }
        }


        private int m_nSiteID = 500;
        public int SiteID
        {
            get { return m_nSiteID; }
         }

        private string m_szDatabaseName = "KPX";
        public string DatabaseName
        {
            get { return m_szDatabaseName; }
        }

        private string m_szDatabaseHost = "127.0.0.1";
        public string DatabaseHost
        {
            get { return m_szDatabaseHost; }
        }

        private string m_szDatabaseUser = "sa";
        public string DatabaseUser
        {
            get { return m_szDatabaseUser; }
        }
        
        private string m_szDatabasePass = "9449966Ab";
        public string DatabasePass
        {
            get { return m_szDatabasePass; }
            set { m_szDatabasePass = value; }
        }

        private DBUtility.WebDBManager m_dbMgr;
        public DBUtility.WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        private string m_szLoggerIP = "";
        public string LoggerIP
        {
            get { return m_szLoggerIP; }
        }

        private int m_nLoggerPort = 33333;
        public int LoggerPort
        {
            get { return m_nLoggerPort; }

        }


        private int nSleepTime = 2000;

        private Thread m_CommandThread = null;

        private bool m_bExitThread = false;

        private int m_nAlarm = 0;

        public void ReadConfig()
        {
            DBUtility.Utility iniFile = new DBUtility.Utility("KPXConfig.ini");
            string szSiteID = iniFile.getinivalue("Server Connection Info", "siteid");

            int nSiteID = 500;
            if (szSiteID.Length > 0)
            {
                int.TryParse(szSiteID, out nSiteID);

            }
            m_nSiteID = nSiteID;

            m_szLoggerIP = iniFile.getinivalue("Jubix Logger", "ipaddr");

            string szPort = iniFile.getinivalue("Jubix Logger", "port");
            if (szPort.Length > 0)
            {
                int.TryParse(szPort, out m_nLoggerPort);
            }

            string szAlarm = iniFile.getinivalue("Jubxi Logger", "alarm");
            if (szAlarm.Length > 0)
            {
                int.TryParse(szAlarm, out m_nAlarm);
            }


        }

        internal void BeginCommander()
        {
            m_Commander = new JubixNetworkCommander(m_dbMgr, m_szLoggerIP, m_nSiteID);

            m_CommandThread = new Thread(ProcessCommand);
            m_CommandThread.Start();

        }

        private void ProcessCommand()
        {
            JubixNetwork.PipeSensorManager.Instance.ReadHistory();

            while(!m_bExitThread)
            {
                //if(ReadOption())
                {
                    JubixNetwork.PipeSensorManager.Instance.ChangeOption();
                    JubixNetwork.PipeSensorManager.Instance.ChangePipeOption();
                }

                JubixNetwork.PipeSensorManager.Instance.ReadAllCommand();
                Queue<JubixNetwork.JubixCommand> que = JubixNetwork.PipeSensorManager.Instance.CommandQue;
                if( que != null && que.Count > 0)
                {
                    int nCount = que.Count;
                    int nProcessCount = 0;
                    while(que.Count > 0)
                    {
                        JubixNetwork.JubixCommand cmd = que.Dequeue();

                        if (ProcessCommand(cmd) == true)
                        {
                            JubixNetwork.PipeSensorManager.Instance.RemoveCommand(cmd);
                        }
                        else
                        {
                            que.Enqueue(cmd);
                        }
                        Thread.Sleep(5);

                        if (2 * nCount < nProcessCount)
                            break;

                        nProcessCount++;
                    }
                }



                for (int i = 0; i < 10; i++)
                {
                    if (m_bExitThread == true)
                        break;
                    Thread.Sleep(nSleepTime / 10);
                }
            }
        }

        private bool ReadOption()
        {
            return false;
        }

        //0(Pipe Alarm Off & Siren Off), 1(Siren On), 2(Tank Alarm Off)
        private bool ProcessCommand(JubixNetwork.JubixCommand cmd)
        {
            if( cmd.Command == 0)
            {
                //int nPipeID = cmd.PipeID;
                //if( nPipeID > 0)
                //{
                    SendCommand(JubixNetwork.JUBIX_TCP_COMMAND.AA, false);
                    JubixNetwork.PipeSensorManager.Instance.ClearPipeAlarm(cmd);
                    return true;
                //}                
            }
            else if(cmd.Command == 1)
            {
                //if (m_nAlarm > 0)
                {
                    SendCommand(JubixNetwork.JUBIX_TCP_COMMAND.AA, true);
                }               
                return true;
            }       
            else if( cmd.Command == 2)
            {
                int nTankID = cmd.TankID;
                if( nTankID > 0)
                {
                    JubixNetwork.PipeSensorManager.Instance.ClearTankAlarm(cmd);
                    return true;
                }                
                
            }  
            // 작업 시작
            else if( cmd.Command == 4)
            {
                JubixNetwork.PipeSensorManager.Instance.BeginWork(cmd.PipeID, cmd.HistoryID);
                return true;
            }
            // 작업 종료
            else if( cmd.Command == 5)
            {
                JubixNetwork.PipeSensorManager.Instance.DoneWork(cmd.PipeID, cmd.HistoryID);
                return true;
            }
            return false;
        }

        internal void StopCommander()
        {
            if (m_Commander != null)
            {
                m_Commander.ClientProvider.Close();
                m_Commander.ReleaseThread();
                m_Commander.ShutdownSensorThread = true;
                m_Commander = null;
            }

            m_bExitThread = true;
        }

        internal void SendCommand(int p1, bool p2)
        {
            if(m_Commander != null)
            {
                JubixNetwork.JubixMessage msg = new JubixNetwork.JubixMessage((short)p1, p2);
                m_Commander.Send(msg.MakeByte(), m_Commander.ClientProvider);
            }
            
        }
    }
}
