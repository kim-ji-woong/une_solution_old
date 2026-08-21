using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DBUtility;
using JubixNetwork;

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

            KPXAlarmChecker.Instance.BeginThread();
        }

        private void ProcessCommand()
        {
            
            while(!m_bExitThread)
            {

                if (KPXAlarmChecker.Instance.ReadyToRead())
                {

                    KPXAlarmChecker.Instance.ReadAllCommand();
                    Queue<JubixNetwork.JubixCommand> que = KPXAlarmChecker.Instance.CommandQue;
                    if (que != null && que.Count > 0)
                    {
                        int nCount = que.Count;
                        int nProcessCount = 0;
                        while (que.Count > 0)
                        {
                            JubixNetwork.JubixCommand cmd = que.Dequeue();

                            if (ProcessCommand(cmd) == true)
                            {
                                KPXAlarmChecker.Instance.RemoveCommand(cmd);
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
                }
                

                for (int i = 0; i < 10; i++)
                {
                    if (m_bExitThread == true)
                        break;
                    Thread.Sleep(nSleepTime / 10);
                }
            }
        }

        //0(Alarm Off & Siren Off), 1(Siren On), 2(Alarm Off & Siren Dontcare),4(작업 시작),5(작업 종료),6(Alarm Option)
        private bool ProcessCommand(JubixNetwork.JubixCommand cmd)
        {
            if( cmd.Command == 0)
            {
                // 경광등을 해제한다.
                SendCommand(JubixNetwork.JUBIX_TCP_COMMAND.AA, false);                    
                // 알람을 클리어 한다.
                KPXAlarmChecker.Instance.ClearAlarm(cmd);
                return true;                            
            }
            else if(cmd.Command == 1)
            {
                // 경광등을 켠다.
                SendCommand(JubixNetwork.JUBIX_TCP_COMMAND.AA, true);                              
                return true;
            }       
            else if( cmd.Command == 2)
            {
                KPXAlarmChecker.Instance.ClearAlarm(cmd);
                return true;
            }  
            // 작업 시작
            else if( cmd.Command == 4)
            {
                if (cmd.PipeID < -1)
                {
                    int nLinkData = cmd.PipeID;
                    cmd.PipeID = -1;
                    KPXAlarmChecker.Instance.BeginWork(cmd, nLinkData);
                }
                else
                {
                    KPXAlarmChecker.Instance.BeginWork(cmd);
                }

                // 알람을 클리어 한다.
                //KPXAlarmChecker.Instance.BeginWork(cmd);
                return true;
            }
            // 작업 종료
            else if( cmd.Command == 5)
            {
                if (cmd.PipeID < -1)
                {
                    int nLinkData = cmd.PipeID;
                    cmd.PipeID = -1;
                    KPXAlarmChecker.Instance.DoneWork(cmd);
                }
                else
                {
                    KPXAlarmChecker.Instance.DoneWork(cmd);
                }

                
                return true;
            }
            // 알람 옵션 변경
            else if (cmd.Command == 6)
            {
                ChangeAlarmOption(cmd);
                if(!KPXAlarmChecker.Instance.ChangeOption(cmd))
                {
                    System.Diagnostics.Trace.WriteLine("Change Alarm Option Fail!");
                }
                return true;
            }
            // 알람 옵션 변경
            else if (cmd.Command == 7)
            {
                ChangeAlarmOption(cmd);
                if (!KPXAlarmChecker.Instance.ChangeOption(cmd))
                {
                    System.Diagnostics.Trace.WriteLine("Change Alarm Option Fail!");
                }
                return true;
            }
            else if (cmd.Command == 8)
            {
                int nTankID = cmd.TankID;
                int nPipeID = cmd.PipeID;

                SetTankStableValue(nTankID, nPipeID, cmd.CreateTime); 
                return true;
            }
            else if (cmd.Command == 9)
            {
                ChangeAlarmOption(cmd);
                if (!KPXAlarmChecker.Instance.ChangeOption(cmd))
                {
                    System.Diagnostics.Trace.WriteLine("Change Alarm Option Fail!");
                }
                return true;
            }
            return false;
        }

        public void SetTankStableValue(int nTankID, int nPipeID, DateTime dtTime)
        {
            List<WorkInfo> infolist = KPXAlarmChecker.Instance.WorkManager.GetWorks(nTankID);
            if (infolist != null)
            {
                foreach (WorkInfo info in infolist)
                {
                    if (info.PipeID == nPipeID)
                    {
                        KPXAlarmChecker.Instance.SetStableValue(info, dtTime);
                    }
                    else
                    {
                        KPXAlarmChecker.Instance.SetStableFlowValue(info, dtTime);
                    }
                }
            }
        }

     

        private void ChangeAlarmOption(JubixCommand cmd)
        {
            //throw new NotImplementedException();
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

            KPXAlarmChecker.Instance.ReleaseThread();
        }

        internal void SendCommand(int p1, bool p2)
        {
            if(m_Commander != null)
            {
                JubixNetwork.JubixMessage msg = new JubixNetwork.JubixMessage((short)p1, p2);
                m_Commander.Send(msg.MakeByte(), m_Commander.ClientProvider);
            }            
        }

        internal void KPXSimulationStart()
        {

        }

        internal void KPXSimulationEnd()
        {

        }
    }    
}
