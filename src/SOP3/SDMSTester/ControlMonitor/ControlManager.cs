using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using SDMSServer;
using System.Threading;

namespace ControlMonitoring
{
    public class ControlManager
    {
        // ControlClientOwner List
        private ArrayList m_arrClients = new ArrayList();
        private IControlClientOwner m_clientHasControl = null;

        private IControlClientOwner m_reservedNextControlClient = null;
        public IControlClientOwner ReservedNextControlClient
        {
            get { return m_reservedNextControlClient; }
            set { m_reservedNextControlClient = value; }
        }

        private static ControlManager m_instance = null;
        public static ControlManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new ControlManager();

                return m_instance;
            }
        }

        // PingCount 초과 등과 같이 비정상적으로 접속이 끊어진 Client가 제어권을 갖고 있었을 경우
        // 바로 제어권을 다른 Client에게 넘기지 않고 m_nAbNormalCloseTimeout(초) 동안 해당 Client의 재접속을 기다린다.
        private static int m_nAbNormalCloseTimeout = 6;

        // 이 값이 0보다 크면 새로 접속한 클라이언트에게 제어권이 부여되지 않는다.
        // 오직 이 값과 동일한 UserID를 가진 클라이언트만이 제어권을 가질수 있다.
        private int m_nControlSOPGenUserID = -1;
        public int ControlSOPGenUserID
        {
            get { return m_nControlSOPGenUserID; }
            set { m_nControlSOPGenUserID = value; }
        }

        private ControlManager()
        {
        }

        public IControlClientOwner ControlClient
        {
            get
            {
                lock (this)
                {
                    if (m_clientHasControl != null)
                    {
                        if (!m_arrClients.Contains(m_clientHasControl))
                        {
                            m_clientHasControl = null;
                        }
                    }
                }

                return m_clientHasControl;
            }
            set { m_clientHasControl = value; }
        }

        public IControlClientOwner FindClient(int nUserID)
        {
            foreach (IControlClientOwner client in m_arrClients)
            {
                if (client.GetControlClient().UserID == nUserID)
                    return client;
            }

            return null;
        }

        public IControlClientOwner FindClient(string strUserName)
        {
            foreach (IControlClientOwner client in m_arrClients)
            {
                if (client.GetControlClient().UserName == strUserName)
                    return client;
            }

            return null;
        }

        public bool FindClient(IControlClientOwner clientData)
        {
            return m_arrClients.Contains(clientData);
        }

        public void AddClient(IControlClientOwner clientData)
        {
            m_arrClients.Add(clientData);
        }

        public IControlClientOwner RemoveClient(int nUserID, string strIP)
        {
            foreach (IControlClientOwner client in m_arrClients)
            {
                if (client.GetControlClient().UserID == nUserID && client.GetControlClient().IP == strIP)
                {
                    m_arrClients.Remove(client);

                    ClearControlOwner(client, true);

                    return client;
                }
            }

            return null;
        }

        // normalClose : 정상적인 종료인가?
        public void RemoveClient(object client, bool normalClose)
        {
            m_arrClients.Remove(client);

            ClearControlOwner(client, normalClose);
        }

        // normalClose : 정상적인 종료인가?
        private void ClearControlOwner(object client, bool normalClose)
        {
            if (m_clientHasControl == client && !normalClose)
            {
                // 비정상 종료된 Client는 Timeout 시간동안 재접속 되기를 기다려준다.
                Thread t = new Thread(new ParameterizedThreadStart(ReconnectTimeoutThread));
                t.Start(client);
            }
            else
            {
                if (m_clientHasControl == client)
                {
                    m_clientHasControl = null;

                    // 제어권을 가진 Client가 접속 종료되었으므로 나머지 Client들 가운데
                    // 제어권을 넘겨받을 Client를 선정하여 제어권을 넘겨준다.
                    IControlClientOwner nextOwner = GetNextControlOwner(null);

                    if (nextOwner == null)
                        nextOwner = GetMostHighLevelClient(null);

                    if (nextOwner != null)
                        nextOwner.SetControl();
                    /////////////////////////////////////////////////////////////////////
                }
            }
        }

        // Timeout 시간동안 비정상적으로 접속 종료된 Client가 재접속 하기를 기다린다.
        private void ReconnectTimeoutThread(object arg)
        {
            IControlClientOwner prevClient = (IControlClientOwner)arg;
            if (prevClient == null)
                return;

            ControlClient prevCtrl = prevClient.GetControlClient();

            //Thread.Sleep(m_nAbNormalCloseTimeout * 1000);
            int nSleep = 0;
            int nTimeout = m_nAbNormalCloseTimeout * 1000;
            int nSleepTime = 1000;

            while (nSleep < nTimeout)
            {
                Thread.Sleep(nSleepTime);
                nSleep += nSleepTime;

                IControlClientOwner currentClient = ControlClient;
                if (currentClient == null)
                    continue;

                ControlClient currentCtrl = currentClient.GetControlClient();

                if (currentCtrl.UserID == prevCtrl.UserID && currentCtrl.IP == prevCtrl.IP &&
                    FindClient(prevCtrl.UserID) != null)
                {
                    // 비정상적으로 접속 종료된 Client가 Timeout 시간내에 다시 재접속 하였음
                    //System.Diagnostics.Trace.WriteLine("재접속 성공");
                    return;
                }
            }

            IControlClientOwner currentOwner = ControlClient;

            if (currentOwner == null || currentOwner == prevClient)
            {
                // 제어권을 원하는 Client들 가운데에서 제어권을 넘겨받을 Client를 고른다.
                IControlClientOwner nextOwner = GetNextControlOwner(null);

                if (nextOwner == null)
                {
                    // 제어권을 원하지 않는 Client들을 포함하여 제어권을 넘겨받을 Client를 고른다.
                    nextOwner = GetMostHighLevelClient(null);
                }

                ControlClient = null;
				
				try
				{
					if (nextOwner != null)
						nextOwner.SetControl();
				}
				catch (System.Exception)
				{				
				}

            }
        }

        // 현재 제어권을 가진 Client(currentOwner)외에 다음으로
        // 제어권을 가지게 될 Client를 리턴한다.
        public IControlClientOwner GetNextControlOwner(IControlClientOwner currentOwner)
        {
            IControlClientOwner nextOwner = null;

            foreach (IControlClientOwner client in m_arrClients)
            {
                if (client.GetControlClient().UserLevel != 0)
                {
                    if (client == currentOwner)
                        continue;

                    if (client.GetControlClient().Control_Type == ControlMonitoring.ControlClient.ControlType.WANT_CONTROL)
                    {
                        if (nextOwner == null)
                            nextOwner = client;
                        else
                        {

                            if (client.GetControlClient().UserLevel > nextOwner.GetControlClient().UserLevel)
                                nextOwner = client;
                        }
                    }
                }
            }

            return nextOwner;
        }

        // 가장 등급이 높은 Client를 리턴한다.
        public IControlClientOwner GetMostHighLevelClient(IControlClientOwner exceptClient)
        {
            IControlClientOwner highClient = null;

            foreach (IControlClientOwner client in m_arrClients)
            {
                if (client.GetControlClient().UserLevel != 0)
                {
                    if (client == exceptClient)
                        continue;

                    if (highClient == null)
                        highClient = client;
                    else
                    {
                        if (client.GetControlClient().UserLevel > highClient.GetControlClient().UserLevel)
                            highClient = client;
                    }
                }
            }

            return highClient;
        }
    }

    public class ControlClient
    {
        public enum ControlType { WANT_CONTROL = 0, NOT_WANT_CONTROL };

        private int m_nUserID = -1;
        // 값이 클수록 권한이 높음
        private int m_nUserLevel = -1;
        private string m_strIP = "";
        private string m_strUserName = "";
        private ControlType m_type = ControlType.WANT_CONTROL;
        private IControlClientOwner m_owner = null;

        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }

        // 값이 클수록 권한이 높음
        public int UserLevel
        {
            get { return m_nUserLevel; }
            set { m_nUserLevel = value; }
        }

        public string IP
        {
            get { return m_strIP; }
            set { m_strIP = value; }
        }

        public string UserName
        {
            get { return m_strUserName; }
            set { m_strUserName = value; }
        }

        public ControlType Control_Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public IControlClientOwner Owner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }
    }

    public interface IControlClientOwner
    {
        ControlClient GetControlClient();
        // 제어권을 넘겨받는다.
        void SetControl();
    }
}
