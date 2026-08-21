using System;
using TcpLib2;
using SDMS;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

namespace SOPChecker
{
    public class ClientDataServerMonitor : ClientData
    {

        public ClientDataServerMonitor(ServiceProvider provider)
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
            else if( nHeader == TCP_ID.START_SERVER_FROM_MONITOR)
            {
                ProcessStartServer(nHeader, arrDatas);
            }
            else if( nHeader == TCP_ID.STOP_SERVER_FROM_MONITOR)
            {
                ProcessStopServer(nHeader, arrDatas);
            }
            else if (nHeader == TCP_ID.START_BACKUP_LOG)
            {
                ProcessBackupLog();
            }
            return true;
        }

        private System.Diagnostics.Process RunStartProcess(string szWorkingPath, string strFileName, string args)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = strFileName + ".exe";
            startInfo.WorkingDirectory = szWorkingPath;
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
            System.Collections.Generic.List<ServerInfo> serverList = ServerManager.Instance.GetServerList();
            int nCheckServer = serverList.Count;

            ArrayList arDatas = new ArrayList();
            foreach(ServerInfo info in serverList)
            {
                int nState = ServerManager.Instance.GetServerState(info);
                arDatas.Add(nState);
            }            

            byte[] datas = ServiceProvider.MakeBytes(TCP_ID.SERVER_STATE, arDatas);
            return m_provider.Send(datas, 0, datas.Length, this.m_state);	
        }

        public bool SendAllServerState(ConnectionState state)
        {
            return SendServerState();
        }

        public void ProcessCheckServer(byte[] bytes)
        { 
            SendServerState();
        }


        public void ProcessStartServer(int nHeader, ArrayList arDatas)
        {

            int nServerID = (int)arDatas[0];

            ServerInfo info = ServerManager.Instance.GetServer(nServerID);
            if( info != null)
            {
                if(info.IsService == true)
                {       
                    string szServerName = info.ServerName;
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        if (!ServiceManager.IsRunningSerivce(szServerName))
                        {
                            ServiceManager.StartService(szServerName, 300);
                        }
                    });   
                }
                else
                {
                    string szProcessName = info.ServerName;
                    if (!ProcessManager.Instance.RunCheckProcess(szProcessName))
                    {
                        RunStartProcess(info.FilePath, szProcessName, "");
                    }
                }              
            } 
        }

        public void ProcessStopServer(int nHader, ArrayList arDatas)
        {
            int nServerID = (int)arDatas[0];

            ServerInfo info = ServerManager.Instance.GetServer(nServerID);
            if (info != null)
            {
                if (info.IsService == true)
                {
                    string szServerName = info.ServerName;
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        if (ServiceManager.IsRunningSerivce(szServerName))
                        {
                            ServiceManager.StopService(szServerName, 300);
                        }
                    });
                }
                else
                {
                    string szProcessName = info.ServerName;
                    if (ProcessManager.Instance.RunCheckProcess(szProcessName))
                    {
                        Process proc = ProcessManager.Instance.GetProcess(szProcessName);
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
                }
            } 
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
