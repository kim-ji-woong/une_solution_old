using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TcpLib2;
using UnE.SOP;
using UnE.Spatial;
using UnE.Sensor;
using IntegratedManagement4;

namespace SDMS
{
    public class ClientProviderInternal : ClientServiceProvider
    {
        private int m_nID = 100;
        private NetworkWebManager m_mgr = null;
        private int m_nPingCount = 0;
        private byte[] m_arrReceived = null;
        // OnReceive()에서 전달받는 데이터(ReceivedData)가 아직 완결되지 않은 Packet일 경우 다음 OnReceive() 호출시 데이터를
        // 합치기 위한 임시 버퍼


        //private byte[] m_arrTemp = null;

        // 현재 OnReceive()에서 받은 데이터를 처리중인가?
        private bool m_isReadingProcess = false;

        public bool IsReadingProcess
        {
            get { return m_isReadingProcess; }
        }

        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        public ClientProviderInternal(NetworkWebManager mgr)
        {
            m_mgr = mgr;
            this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
            this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        }
        
        public override void OnReceiveData()
        {
            OnReceive(ReceivedData);
        }

        private bool OnReceive(byte[] bytes)
        {
            try
            {
                if (bytes != null)
                {
                    m_isReadingProcess = true;

                    m_arrReceived = bytes;

                    int nBytesCount = m_arrReceived.Count();

                    if (nBytesCount > 0)
                    {
                        // 이미 종료되었어야 할 접속이 유지되고 있는 경우는 해당 접속을 강제로 종료시킨다.
                        if (m_mgr.ClientProviderInternal != this)
                        {
                            this.Close();
                            m_isReadingProcess = false;
                            return true;
                        }

                        m_nPingCount = 0;
                        SendData(TCP_ID.I_AM_HERE);

                        // 
                        m_mgr.RecvLog(m_arrReceived, m_nID);

                        short nHeader;
                        ArrayList arrDatas = NetworkWebManager.ReadBytes(m_arrReceived, out nHeader);

                        if (nHeader == TCP_ID.ARE_YOU_THERE)
                        {
                            //SendData(TCP_ID.I_AM_HERE);
                        }
                        else if (nHeader == TCP_ID.WHO_ARE_YOU)
                        {
                            SendData(TCP_ID.WHO_I_AM, TCP_TYPE.INTEGER, BitConverter.GetBytes((int)TCP_CLIENT.SDMS_CLIENT));
                        }
                        else if (nHeader == TCP_ID.INTERNAL_MESSAGE)
                            ProcessInternalMessage(arrDatas);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            m_isReadingProcess = false;
            return true;
        }

        private void ProcessInternalMessage(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount == 0 || arrDatas[0].GetType() != typeof(byte))
                return;

            byte command = (byte)arrDatas[0];

            if (command == InternalMessage.SOP_SIMULATOR_2_SDMS)
            {
                if (nDataCount >= 2 || arrDatas[1] is short)
                {
                    short func = (short)arrDatas[1];

                    if (func == InternalMessage.SopSimulatorToSdms.SET_CHECK_POSITION)
                        ProcessSetCheckPosition(arrDatas);
                    else if (func == InternalMessage.SopSimulatorToSdms.SET_LAST_POSITION)
                        ProcessSetLastPosition(arrDatas);
                    else if (func == InternalMessage.SopSimulatorToSdms.REMOVE_DISASTER_POS)
                        ProcessRemoveDisasterPos(arrDatas);
                    else if (func == InternalMessage.SopSimulatorToSdms.NULL_LAST_POSITION)
                        ProcessNullLastPosition(arrDatas);
                    else if (func == InternalMessage.SopSimulatorToSdms.UPDATE_3D_VIEW)
                        ProcessUpdate3DView(arrDatas);
                    else if (func == InternalMessage.SopSimulatorToSdms.TOGGLE_MINIMUM_WINDOW)
                        ProcessToggleMinimumWindow(arrDatas);
                    else if (func == InternalMessage.SopSimulatorToSdms.EARTHQUAKE_EVENT)
                        ProcessEarthquakeEvent(arrDatas);
                    else if (func == InternalMessage.SopSimulatorToSdms.EARTHQUAKE_EVENT_IS_FINISHED)
                        ProcessAsk_EarthquakeEventIsFinished(arrDatas);
                    else if (func == InternalMessage.SopSimulatorToSdms.ON_CHECK_POSITION_END)
                        ProcessOnCheckPositionEnd(arrDatas);
                    else if (func == InternalMessage.SopSimulatorToSdms.SHOW_BUILDING_COLLAPSE)
                        ProcessShowBuildingCollapse(arrDatas);
                    else if (func == InternalMessage.SopSimulatorToSdms.CLOSE_BUILDING_COLLAPSE)
                        ProcessCloseBuildingCollapse(arrDatas);
                    else if (func == InternalMessage.SopSimulatorToSdms.TOGGLE_CCTV)
                        ProcessToggleCCTV(arrDatas);
                    else if (func == InternalMessage.SopSimulatorToSdms.SHOW_WINDOW)
                        ProcessShowWindow(arrDatas);
                    else if (func == InternalMessage.SopSimulatorToSdms.OPEN_SOP_ON_SENSOR_DETECT)
                        ProcessSOPOnSensorDetect(arrDatas);
                }
            }
        }

        private void ProcessSOPOnSensorDetect(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 2)
            {
                if (arrDatas[1] is bool)
                {
                    bool loadSOP = (bool)arrDatas[1];
                    ProxySOP.Instance.OpenSOPOnSensorDetect = loadSOP;
                }
            }
        }

        private void ProcessShowWindow(ArrayList arrDatas)
        {
            Proxy.StubWorker.Instance.ShowWindow();
        }

        private void ProcessToggleCCTV(ArrayList arrDatas)
        {
            Proxy.StubWorker.Instance.ToggleCCTV();
        }

        private void ProcessCloseBuildingCollapse(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 3)
            {
                if (arrDatas[2] is string)
                {
                    string szBuildingID = (string)arrDatas[2];

                    Proxy.StubWorker.Instance.CloseBuildingCollapse(szBuildingID);
                }
            }
        }

        private void ProcessShowBuildingCollapse(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 4)
            {
                if (arrDatas[2] is string && arrDatas[3] is string)
                {
                    string szBuildingID = (string)arrDatas[2];
                    string szDisplayName = (string)arrDatas[3];

                    Proxy.StubWorker.Instance.ShowBuildingCollapse(szBuildingID, szDisplayName);
                }
            }
        }

        private void ProcessOnCheckPositionEnd(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 3)
            {
                if (arrDatas[2] is bool)
                {
                    bool bResult = (bool)arrDatas[2];

                    Proxy.StubWorker.Instance.OnCheckPositionEnd(bResult);
                }
            }
        }

        private void ProcessAsk_EarthquakeEventIsFinished(ArrayList arrDatas)
        {
            Proxy.StubWorker.Instance.Ask_EarthquakeEventIsFinished();
        }

        private void ProcessEarthquakeEvent(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 6)
            {
                if (arrDatas[2] is int && arrDatas[3] is float && arrDatas[4] is string && arrDatas[5] is bool)
                {
                    int nIntensity = (int)arrDatas[2];
                    float fMagnitude = (float)arrDatas[3];
                    string strPosition = (string)arrDatas[4];
                    bool isRealMode = (bool)arrDatas[5];

                    Proxy.StubWorker.Instance.EarthquakeEvent(nIntensity, fMagnitude, strPosition, isRealMode);
                }
            }
        }

        private void ProcessToggleMinimumWindow(ArrayList arrDatas)
        {
            Proxy.StubWorker.Instance.ToggleMinimumWindow();
        }

        private void ProcessUpdate3DView(ArrayList arrDatas)
        {
            Proxy.StubWorker.Instance.Update3DView();
        }

        private void ProcessNullLastPosition(ArrayList arrDatas)
        {
            Proxy.StubWorker.Instance.NullLastPosition();
        }

        private void ProcessRemoveDisasterPos(ArrayList arrDatas)
        {
            Proxy.StubWorker.Instance.RemoveDisasterPos();
        }

        private void ProcessSetLastPosition(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 15)
            {
                if (arrDatas[2] is string && arrDatas[3] is string && arrDatas[4] is string && arrDatas[5] is string && arrDatas[6] is float
                    && arrDatas[7] is int && arrDatas[8] is int && arrDatas[9] is int && arrDatas[10] is int && arrDatas[11] is float
                    && arrDatas[12] is float && arrDatas[13] is float && arrDatas[14] is int)
                {
                    string strDisasterName = (string)arrDatas[2];
                    string strPositionName = (string)arrDatas[3];
                    string strBroadcastName = (string)arrDatas[4];
                    string strBuildingID = (string)arrDatas[5];
                    float fFloorIndex = (float)arrDatas[6];
                    int nActionStepHistoryID = (int)arrDatas[7];
                    int nIconID = (int)arrDatas[8];
                    int nPSMDistance = (int)arrDatas[9];
                    string strPSMMaterial = (string)arrDatas[10];
                    float x = (float)arrDatas[11];
                    float y = (float)arrDatas[12];
                    float z = (float)arrDatas[13];
                    int nZoneID = (int)arrDatas[14];

                    Proxy.StubWorker.Instance.SetLastPosition(strDisasterName, strPositionName, strBroadcastName, strBuildingID, fFloorIndex, nActionStepHistoryID, nIconID, nPSMDistance, strPSMMaterial, x, y, z, nZoneID);
                }
            }
        }

        private void ProcessSetCheckPosition(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 16)
            {
                if (arrDatas[2] is string && arrDatas[3] is string && arrDatas[4] is string && arrDatas[5] is string && arrDatas[6] is float
                    && arrDatas[7] is int && arrDatas[8] is int && arrDatas[9] is int && arrDatas[10] is string && arrDatas[11] is float 
                    && arrDatas[12] is float && arrDatas[13] is float && arrDatas[14] is int && arrDatas[15] is bool)
                {
                    string strDisasterName = (string)arrDatas[2];
                    string strPositionName = (string)arrDatas[3];
                    string strBroadcastName = (string)arrDatas[4];
                    string strBuildingID = (string)arrDatas[5];
                    float fFloorIndex = (float)arrDatas[6];
                    int nActionStepHistoryID = (int)arrDatas[7];
                    int nIconID = (int)arrDatas[8];
                    int nPSMDistance = (int)arrDatas[9];
                    string strPSMMaterial = (string)arrDatas[10];
                    float x = (float)arrDatas[11];
                    float y = (float)arrDatas[12];
                    float z = (float)arrDatas[13];
                    int nZoneID = (int)arrDatas[14];
                    bool isChecked = (bool)arrDatas[15];

                    Proxy.StubWorker.Instance.SetCheckPosition(strDisasterName, strPositionName, strBroadcastName, strBuildingID, fFloorIndex, nActionStepHistoryID, nIconID, nPSMDistance, strPSMMaterial, x, y, z, nZoneID, isChecked);
                }
            }
        }

        public void SendData(short header, ArrayList arrDatas)
        {
            byte[] bytes = NetworkWebManager.MakeBytes(header, arrDatas);

            try
            {
                if (this.IsClientDisposed == false)
                    m_mgr.Send(bytes, this, m_nID);
            }
            catch (Exception)
            {
                this.Close();
            }
        }

        // header 1 Byte로만 이루어진 데이터
        public void SendData(short header)
        {
            byte[] bytes = new byte[6];

            byte[] nHader = BitConverter.GetBytes(header);
            byte[] nCount = BitConverter.GetBytes(0);

            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            try
            {
                if (this.Client.Client != null)
                {
                    if (this.Client.Client.Connected == true)
                        m_mgr.Send(bytes, this, m_nID);
                }
            }
            catch (Exception)
            {
                this.Close();
            }
        }

        public void SendData(short header, byte dataHeader, byte[] datas)
        {
            if (header < 0)
                return;

            if (datas.Length >= 10000)
                return;
            if (datas == null || datas.Length == 0)
                return;

            byte[] sndData = new byte[datas.Length + 11];

            byte[] nHader = BitConverter.GetBytes(header);
            byte[] nCount = BitConverter.GetBytes(1);

            // SET MESSAGE HeADER
            sndData[0] = nHader[0];
            sndData[1] = nHader[1];

            // SET DATA COUNT
            sndData[2] = nCount[0];
            sndData[3] = nCount[1];
            sndData[4] = nCount[2];
            sndData[5] = nCount[3];

            // SET DATA TYPE
            sndData[6] = dataHeader;

            // SET DATA LENGTH
            byte[] lengthData = BitConverter.GetBytes(datas.Length);
            for (int i = 0; i < 4; i++)
            {
                if (lengthData.Length > i)
                {
                    sndData[7 + i] = lengthData[i];
                }
            }

            // SET DATA
            for (int i = 0; i < datas.Length; i++)
            {
                sndData[i + 11] = datas[i];
            }

            try
            {
                if (this.IsClientDisposed == false)
                    m_mgr.Send(sndData, this, m_nID);
            }
            catch (Exception)
            {
                this.Close();
            }
        }

        public override void OnDropConnection()
        {
            
        }

        public new void Close()
        {
            base.Close();
        }
    }
}