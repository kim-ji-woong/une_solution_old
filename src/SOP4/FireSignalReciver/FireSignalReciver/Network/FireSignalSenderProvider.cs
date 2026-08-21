using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

namespace FireSignalReciver
{
    public class FireSignalSenderProvider : ClientProvider
    {
        private static log4net.ILog logger = null;

        private Dictionary<int, Reciver> m_ReciverState = new Dictionary<int, Reciver>();

        public FireSignalSenderProvider(NetworkManager mgr)
            : base(mgr)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            ArrayList arReciverList = (ArrayList)FireSignalReciver.Instance.IoMgr.GetReciverList().Clone();

            if (arReciverList != null)
            {
                arReciverList.Reverse();
                for (int i = 0; i < arReciverList.Count; i++)
                {
                    Reciver reciver = (Reciver)arReciverList[i];

                    m_ReciverState.Add(reciver.ReciverID, reciver);

                }
            }
        }

        protected override bool OnReceive(byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            PingCount = 0;

            if(nHeader == TCP_ID.SENSOR_DATA)
            {
                ProcessSensorData(arrDatas);
                System.Diagnostics.Trace.WriteLine("Process Sensor Data : " + arrDatas.Count + " ," + arrDatas[1]);
            }
            else if (nHeader == TCP_ID.ALL_RECIVER_STATE)
            {
                ProcessReciverState(arrDatas);
                System.Diagnostics.Trace.WriteLine("Process ReciverState : " + arrDatas.Count + " ," + arrDatas[1]);
            }
            return true;
        }        

        public void RequestSensorDatas()
        {
            SendData(TCP_ID.SENSOR_DATA);
        }

        public void RequestReciverState()
        {
            SendData(TCP_ID.ALL_RECIVER_STATE);
        }

        private void ProcessSensorData(ArrayList arDatas)
        {

            for(int i = 0 ; i < arDatas.Count ; i += 5)
            {
                string szTime = (string)arDatas[i];
                int nReciverID = (int)arDatas[i + 1];
                bool bOff = (bool)arDatas[i + 2];
                string szCode = (string)arDatas[i + 3];
                string szCircuit = (string)arDatas[i + 4];

                if (szCode == "SW41N")
                {
                    SendReset(nReciverID);
                    continue;
                }

                int nCircuit = GetCurcuit(szCircuit);
                if(nCircuit >= 0)
                {
                    ProcessSensorData(nReciverID, nCircuit, bOff);
                }                
                System.Diagnostics.Trace.WriteLine("Signal :" + szTime + "," + nReciverID + "," + bOff + "," + szCode + "," + szCircuit);
            }
        }

        private int GetCurcuit(string szValue)
        {
            //12-000-018-0
            try
            {

                string[] sp = szValue.Split(new char[]{'-'}, StringSplitOptions.RemoveEmptyEntries);
                if (sp.Length < 3)
                    return -1;

                string szTag = sp[2];

                System.Diagnostics.Trace.WriteLine("회로번호 : " + szTag);
                int nCurcuitID = -1;
                if (int.TryParse(szTag, out nCurcuitID))
                {
                    return nCurcuitID;
                }
            }
            catch(Exception)
            { }            
            return -1;
        }

        private void ProcessSensorData(int nReciverID, int nCurcuit, bool bOff)
        {
            if (nCurcuit < 0)
                return;            
            byte nData = 0;     

            if (m_ReciverState.ContainsKey(nReciverID))
            {
                Reciver reciver = m_ReciverState[nReciverID];
                if(reciver != null)
                {
                    Curcuit curcuit = null;
                    if (reciver.Curcuits.ContainsKey(nCurcuit))
                    {
                        curcuit = reciver.Curcuits[nCurcuit];
                    }

                    // 회로번호가 없는 경우
                    if (curcuit == null)
                    {
                        logger.Info("없는 회로 번호 : " + nCurcuit);
                        return;
                    }

                    if( bOff == true)
                    {
                        nData = 0;
                    }
                    else
                    {
                        nData = 1;
                    }

                    SendSensorData(curcuit, nData);
                }                
            }             
        }

        private void SendSensorData(Curcuit curcuit, int nData)
        {
            int nCurcuit = curcuit.TagNum;

            logger.Info("[SOP서버로 회로 이름 " + curcuit.Name + " 에 대해 " + nData.ToString() + " 값 전송, 회로" + nCurcuit + "]");
            Debug.WriteLine("[SOP서버로 회로 이름 " + curcuit.Name + " 에 대해 " + nData.ToString() + " 값 전송, 회로" + nCurcuit + "]");

            int nSensorZoneID = curcuit.TargetZoneID;

            int nTagNum = curcuit.TagNum;
            int nSensorType = curcuit.SensorType;

            m_mgr.SendSensorData(nSensorZoneID, curcuit.ID, nSensorType, nData, "", nTagNum.ToString());

            //m_mgr.SendSensorData(curcuit.ReciverID, nCurcuit, nData);
        }

        private void SendReset(int nReciverID)
        {
            if (m_ReciverState.ContainsKey(nReciverID))
            {
                Reciver reciver = m_ReciverState[nReciverID];
                int nData = 0;
                foreach (KeyValuePair<int, Curcuit> pair in reciver.Curcuits)
                {
                    Curcuit curcuit = pair.Value;
                    SendSensorData(curcuit, nData);
                    Thread.Sleep(50);
                }
                logger.Info("[SOP서버로 수신반 리셋 : " + nReciverID + "]");
            }
        }

        private void ProcessReciverState(ArrayList arDatas)
        {
            try
            {
                for (int i = 0; i < arDatas.Count; i += 2)
                {
                    int id = (int)arDatas[i];
                    int state = (int)arDatas[i + 1];

                    if(m_ReciverState.ContainsKey(id))
                    {
                        Reciver reciver = m_ReciverState[id];
                        if (state > 0)
                            reciver.IsConnected = true;
                        else
                        {
                            reciver.IsConnected = false;
                        }
                        if (state >= 10)
                            reciver.RecivedPoll = true;
                        else
                            reciver.RecivedPoll = false;
                    }
                }

                m_mgr.SendAllReciverState();
            }catch(Exception)
            {

            }            
        }
    }
}
