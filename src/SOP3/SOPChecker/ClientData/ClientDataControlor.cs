using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

namespace SOPChecker
{
    public class ClientDataControlor : ClientData
    {
        private bool m_IsServiceTTSServer = false;
        private string m_szTTSServer = "TTSServerDotNetCmd";
        private string m_szSOPServer = "SOPServer";
        private string m_szSensorMonitor = "SOPMonitor";

        public ClientDataControlor(ServiceProvider provider)
        {            
            m_provider = provider;
            Type = ClientType.CONTROLOR;
        }

        // OnAccept() 이후 WhoIAm을 받은 뒤 처리해야 할 로직
        protected override bool ProcessFirstConnection(ConnectionState state)
        {
            m_state = state;
            return SendAllServerState(state); ;
        }

        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
			if (nHeader == TCP_ID.CHECK_ALL_SERVER)
			{
                ProcessCheckServer(bytes);
			}
            else if (nHeader == TCP_ID.START_SOP_SERVER)
            {
                ProcessStartSOPServer();
            }
            else if (nHeader == TCP_ID.START_TTS_SERVER)
            {
                ProcessStartTTSServer();
            }
            else if (nHeader == TCP_ID.STOP_SOP_SERVER)
            {
                ProcessStopSOPServer();
            }
            else if (nHeader == TCP_ID.STOP_TTS_SERVER)
            {
                ProcessStopTTSServer();
            }
            else if (nHeader == TCP_ID.START_SENSOR_MONITOR)
            {
                ProcessStartSensor();
            }
            else if (nHeader == TCP_ID.STOP_SENSOR_MONITOR)
            {
                ProcessStopSensor();
            }

            else if (nHeader == TCP_ID.START_BACKUP_LOG)
            {
                ProcessBackupLog();
            }
            return true;
        }

        private string GetExecutablePath()
        {
             return "C:\\TTSServer";
        }

        private System.Diagnostics.Process RunStartProcess(string strFileName, string args)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = strFileName + ".exe";
            startInfo.WorkingDirectory = GetExecutablePath();
            startInfo.ErrorDialog = false;
            startInfo.Arguments = args;

            System.Diagnostics.Process process;
            try
            {
                process = System.Diagnostics.Process.Start(startInfo);

                return process;
            }
            catch (Exception)
            {               
            }
            return null;
        }


        private bool SendServerState()
        {
            bool bRunTTS = ProcessManager.Instance.RunCheckProcess(m_szTTSServer);
            bool bRunSOP = ServiceManager.IsRunningSerivce(m_szSOPServer);
            bool bRunMonitor = ServiceManager.IsRunningSerivce(m_szSensorMonitor);
            
            int nCheckServer = 3;
            int nDataCount = nCheckServer * 2;
            int nSize = 6 + (nDataCount * 9);
            byte[] bytes = new byte[nSize];

            byte[] byteHeader = BitConverter.GetBytes((short)TCP_ID.SERVER_STATE);
            bytes[0] = byteHeader[0];
            bytes[1] = byteHeader[1];

            // SET DATA COUNT
            byte[] nCount = BitConverter.GetBytes(nCheckServer);
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            int nIndex = 6;

            byte[] ttsBytes = ServiceProvider.MakeBytes(bRunTTS == true ? 1 : 0);
            byte[] sopBytes = ServiceProvider.MakeBytes(bRunSOP == true ? 1 : 0);
            byte[] monBytes = ServiceProvider.MakeBytes(bRunMonitor == true ? 1 : 0);

            System.Buffer.BlockCopy(ttsBytes, 0, bytes, nIndex, ttsBytes.Length);
            nIndex += ttsBytes.Length;

            System.Buffer.BlockCopy(sopBytes, 0, bytes, nIndex, sopBytes.Length);
            nIndex += sopBytes.Length;

            System.Buffer.BlockCopy(monBytes, 0, bytes, nIndex, monBytes.Length);

            return m_provider.Send(bytes, 0, bytes.Length, this.m_state);	
        }

        public bool SendAllServerState(ConnectionState state)
        {
            return SendServerState();
        }

        public void ProcessCheckServer(byte[] bytes)
        { 
            SendServerState();
        }

        public void ProcessStartSOPServer()
        {
            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                if (!ServiceManager.IsRunningSerivce(m_szSOPServer))
                {
                    ServiceManager.StartService(m_szSOPServer, 300);
                }
            });
        }

        public void ProcessStartTTSServer()
        {
            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                if (!ProcessManager.Instance.RunCheckProcess(m_szTTSServer))
                {
                    // runt tts server
                    RunStartProcess(m_szTTSServer, "");
                }
            });
        }

        public void ProcessStopSOPServer()
        {
            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                //if (ServiceManager.IsRunningSerivce(m_szSOPServer))
                //{
                //    ServiceManager.StopService(m_szSOPServer, 300);
                //}
                if (ProcessManager.Instance.RunCheckProcess(m_szSOPServer))
                {
                    Process proc = ProcessManager.Instance.GetProcess(m_szSOPServer);
                    if (proc == null)
                        return;

                    try
                    {
                        proc.Kill();
                    }
                    catch (System.Exception)
                    {
                    }
                }
            });
        }

        public void ProcessStopTTSServer()
        {
          
            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                if (ProcessManager.Instance.RunCheckProcess(m_szTTSServer))
                {
                    Process proc = ProcessManager.Instance.GetProcess(m_szTTSServer);
                    if (proc == null)
                        return;
                         
                    try
                    {
                        proc.Kill();
                    }
                    catch (System.Exception)
                    {
                    }                
                }
            });
        }

        public void ProcessStartSensor()
        {
            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                if (!ServiceManager.IsRunningSerivce(m_szSensorMonitor))
                {
                    ServiceManager.StartService(m_szSensorMonitor, 300);
                }
            });
        }
        
        public void ProcessStopSensor()
        {
            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                //if (ServiceManager.IsRunningSerivce(m_szSensorMonitor))
                //{
                //    ServiceManager.StopService(m_szSensorMonitor, 300);
                //}
                if (ProcessManager.Instance.RunCheckProcess("SensorMonitor"))
                {
                    Process proc = ProcessManager.Instance.GetProcess("SensorMonitor");
                    if (proc == null)
                        return;

                    try
                    {
                        proc.Kill();
                    }
                    catch (System.Exception)
                    {
                    }
                }
            });
        }

        private bool SendGetLog()
        {  
            int nDataCount = 0 * 2;
            int nSize = 6 + (nDataCount * 9);
            byte[] bytes = new byte[nSize];

            byte[] byteHeader = BitConverter.GetBytes((short)TCP_ID.GET_BACKUP_LOG);
            bytes[0] = byteHeader[0];
            bytes[1] = byteHeader[1];

            // SET DATA COUNT
            byte[] nCount = BitConverter.GetBytes(0);
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];            

            return m_provider.Send(bytes, 0, bytes.Length, this.m_state);
        }

        private void CompleteBackupLog()
        {
            SendGetLog();
        }
        public void ProcessBackupLog()
        {
            LogBackup backup = new LogBackup();
            backup.Callback += new LogBackupCallback(CompleteBackupLog);
            backup.GatherServerLog();

        }
    }
}
