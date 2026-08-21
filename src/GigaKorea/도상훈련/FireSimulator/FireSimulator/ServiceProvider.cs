using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections;

namespace FireSimulator
{
    class ServiceProvider : TcpServiceProvider
    {
        private ConcurrentDictionary<ConnectionState, ClientData> m_dicClients = new ConcurrentDictionary<ConnectionState, ClientData>();
        private IListener m_listener = null;

        public ServiceProvider(IListener listener = null)
        {
            m_listener = listener;
        }

        public override void OnAcceptConnection(ConnectionState state)
        {
            ClientData data = new ClientData(this, m_listener);
            state.Tag = data;

            if (m_dicClients.TryAdd(state, data))
            {
                if (FormMain.Instance != null)
                {
                    SendAlarmList(state);
                    FormMain.Instance.UpdateClient(m_dicClients.Count);
                }
            }
        }

        public override void OnDropConnection(ConnectionState state)
        {
            ClientData data;
            m_dicClients.TryRemove(state, out data);

            if (FormMain.Instance != null)
                FormMain.Instance.UpdateClient(m_dicClients.Count);
        }

        public override object Clone()
        {
            return this;
        }

        public override bool OnReceiveData(ConnectionState state)
        {
            if (!base.OnReceiveData(state))
                return false;

            state.LengthAdd = false;

            ClientData client = (ClientData)state.Tag;
            if (client == null)
                return false;

            bool bResult = client.OnReceiveData(state, state.RecivedBuffer, false);
            state.RecivedBuffer = null;
            return bResult;
        }

        private void SendAlarmList(ConnectionState state)
        {
            List<Alarm> alarms = FormMain.Instance.GetAlarms();
            Project project = FormMain.Instance.GetProject();

            if (project == null)
                return;

            foreach (Alarm alarm in alarms)
            {
                // 데이터에 데이터 길이 붙이지 않게 설정
                state.LengthAdd = false;

                SendAlarm(project, alarm, state, TCP_ID.REPORT_FIRE);
            }
        }

        public void SendAlarm(Project project, Alarm alarm, ConnectionState state, short nHeader)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(project.Name);
            arrDatas.Add(alarm.Level.ID);
            arrDatas.Add(alarm.Space.ID);
            arrDatas.Add(alarm.TimeStamp.ToBinary());

            byte[] bytes = TcpHelper.MakeBytes(nHeader, arrDatas);
            Send(bytes, state);


            if (nHeader == TCP_ID.REPORT_OUTBREAK)
            {
                arrDatas = new ArrayList();

                arrDatas.Add(project.Name);
                arrDatas.Add(alarm.Level.Name);
                arrDatas.Add(alarm.Space.Name);
                arrDatas.Add(alarm.TimeStamp.ToBinary());

                bytes = TcpHelper.MakeBytes(TCP_ID.POPUP_OUTBREAK, arrDatas);
                Send(bytes, state);
            }
        }

        

        public void SendClear(Project project, Alarm alarm, DateTime dtClear, ConnectionState state, short nHeader)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(project.Name);
            arrDatas.Add(alarm.Level.ID);
            arrDatas.Add(alarm.Space.ID);
            arrDatas.Add(dtClear.ToBinary());

            byte[] bytes = TcpHelper.MakeBytes(nHeader, arrDatas);
            Send(bytes, state);
        }

        public bool Send(byte[] bytes, ConnectionState state)
        {
            bool success = false;

            try
            {
                if (state.Connected == false)
                {
                    OnDropConnection(state);
                    return false;
                }

                success = state.WriteAsync(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                OnDropConnection(state);
                return false;
            }

            return success;
        }

        public void SendAlarm(Alarm alarm, Project project, short nHeader)
        {
            List<ConnectionState> states = m_dicClients.Keys.ToList();

            foreach (ConnectionState state in states)
            {
                try
                {
                    // 데이터에 데이터 길이 붙이지 않게 설정
                    state.LengthAdd = false;

                    SendAlarm(project, alarm, state, nHeader);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine("SendAlarm Exception : " + e.Message);
                }
            }
        }

        public void SendOutbreak(int nActionStepHistoryID, int nProcessID)
        {
            List<ConnectionState> states = m_dicClients.Keys.ToList();

            foreach (ConnectionState state in states)
            {
                try
                {
                    // 데이터에 데이터 길이 붙이지 않게 설정
                    state.LengthAdd = false;

                    SendOutbreak(nActionStepHistoryID, nProcessID, state);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine("SendOutbreak Exception : " + e.Message);
                }
            }
        }

        public void SendOutbreak(int nActionStepHistoryID, int nProcessID, ConnectionState state)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nActionStepHistoryID);
            arrDatas.Add("Process");
            arrDatas.Add(nProcessID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            Send(bytes, state);
        }

        public void SendClear(Alarm alarm, Project project, short nHeader)
        {
            DateTime dtNow = DateTime.Now;
            List<ConnectionState> states = m_dicClients.Keys.ToList();

            foreach (ConnectionState state in states)
            {
                try
                {
                    SendClear(project, alarm, dtNow, state, nHeader);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine("SendClear Exception : " + e.Message);
                }
            }
        }
    }
}
