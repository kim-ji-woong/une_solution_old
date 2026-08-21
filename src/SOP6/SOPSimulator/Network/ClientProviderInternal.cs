using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Net.Sockets;
using SOPMonitoringSystem;
using System.Windows.Forms;
using SDMS;
using UnE.SOP;
using IntegratedManagement4;
using System.Threading;

namespace SOPMonitoringSystem
{
    public class ClientProviderInternal : ClientServiceProvider
    {
        private NetworkWebManager m_mgr = null;
        private int m_nPingCount = 0;
        private byte[] m_arrReceived = null;
        // OnReceive()에서 전달받는 데이터(ReceivedData)가 아직 완결되지 않은 Packet일 경우 다음 OnReceive() 호출시 데이터를
        // 합치기 위한 임시 버퍼
        private byte[] m_arrTemp = null;
        private string m_strTag = "Internal";

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

                    if (m_arrTemp != null)
                    {
                        int nReceivedCount = m_arrReceived.Length;
                        int nTempCount = m_arrTemp.Length;

                        byte[] arrBuffer = new byte[nReceivedCount + nTempCount];
                        Array.Copy(m_arrTemp, arrBuffer, nTempCount);
                        Array.Copy(m_arrReceived, 0, arrBuffer, nTempCount, nReceivedCount);

                        m_arrReceived = arrBuffer;
                        m_arrTemp = null;
                    }

                    int nBytesCount = m_arrReceived.Count();

                    if (nBytesCount > 0)
                    {
                        m_nPingCount = 0;

                        if (!CheckValidation(m_arrReceived))
                        {
                            m_arrTemp = m_arrReceived;
                            m_isReadingProcess = false;
                            return false;
                        }

                        //m_mgr.RecvLog(m_arrReceived);

                        //int nHeader = (int)BitConverter.ToInt16(m_arrReceived, 0);
                        short nHeader;
                        ArrayList arrDatas = TcpLib2.TcpHelper.ReadBytes(m_arrReceived, out nHeader);

                        if (arrDatas == null)
                        {
                            m_isReadingProcess = false;
                            return false;
                        }

                        if (nHeader == TCP_ID.ARE_YOU_THERE)
                        {
                            // 이미 종료되었어야 할 접속이 유지되고 있는 경우는 해당 접속을 강제로 종료시킨다.
                            if (m_mgr.ClientProviderInternal != this)
                            {
                                this.Close();
                                m_isReadingProcess = false;
                                return true;
                            }

                            ProcessAreYouThere(arrDatas);
                            //SendData(TCP_ID.I_AM_HERE);
                        }
                        else if (nHeader == TCP_ID.WHO_ARE_YOU)
                        {
                            //SendData(TCP_ID.WHO_I_AM, TCP_TYPE.INTEGER, BitConverter.GetBytes((int)ClientType.SOP_SIMULATOR));
                            SendWhoIAm();
                        }
                        else if (nHeader == TCP_ID.INTERNAL_MESSAGE)
                            ProcessInternalMessage(arrDatas);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("InternalMessage Receive Error : " + e.Message);
            }

            m_isReadingProcess = false;
            return true;
        }

        private void ProcessAreYouThere(ArrayList arrDatas)
        {
            arrDatas.Clear();
            arrDatas.Add(FormSOP.Instance.HasControl);

            byte[] bytes = TcpLib2.TcpHelper.MakeBytes(TCP_ID.I_AM_HERE, arrDatas);
            Send(bytes, m_strTag);
        }

        private int Send(byte[] bytes, string strTag = "")
        {
            if (IsClientDisposed == true)
                return -1;

            if (IsConnected == false)
            {
                Thread.Sleep(1000);
                if (IsConnected == false)
                    return -1;
            }

            int nResult = Send(bytes, 0, bytes.Length);

            /*if (nResult > 0)
            {
                if (!IsLogOpened)
                    return nResult;

                if (bytes[0] != TCP_ID.I_AM_HERE || !m_exceptPingLog)
                {
                    string strLog = string.Format("SendMessage : Header({0}), Length({1}), SOPSimulator", (int)bytes[0], (int)bytes.Length);

                    if (strTag.Length > 0)
                        strLog += " " + strTag;


                    string strBytes = "";

                    foreach (byte b in bytes)
                    {
                        if (strBytes.Length == 0)
                            strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                        else
                            strBytes += string.Format(" {0:X2}", (int)b);
                    }

                    WriteLineLog(strLog + strBytes);
                }
            }*/

            return nResult;
        }

        private void SendWhoIAm()
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add((int)TCP_CLIENT.SOP_SIMULATOR);

            byte[] bytes = TcpLib2.TcpHelper.MakeBytes(TCP_ID.WHO_I_AM, arrDatas);

            try
            {
                Send(bytes, m_strTag);
            }
            catch (Exception)
            {
                this.Close();
            }
        }

        private void ProcessInternalMessage(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount == 0 || arrDatas[0].GetType() != typeof(byte))
                return;

            byte command = (byte)arrDatas[0];

            if (command == InternalMessage.SDMS_2_SOP_SIMULATOR)
            {
                if (nDataCount >= 2 || arrDatas[1] is short)
                {
                    short func = (short)arrDatas[1];

                    if (func == InternalMessage.SdmsToSopSimulator.RUN_SOP_SIMULATOR)
                        ProcessRunSOPSimulator(arrDatas);
                    else if (func == InternalMessage.SdmsToSopSimulator.IGNORE_SOP)
                        ProcessIgnoreSOP(arrDatas);
                    else if (func == InternalMessage.SdmsToSopSimulator.COMPLETE_LOADING)
                        ProcessCompleteLoading(arrDatas);
                    else if (func == InternalMessage.SdmsToSopSimulator.SHOW_SOP_SIMULATOR)
                        ProcessShowSOPSimulator(arrDatas);
                    else if (func == InternalMessage.SdmsToSopSimulator.HIDE_SOP_SIMULATOR)
                        ProcessHideSOPSimulator(arrDatas);
                    else if (func == InternalMessage.SdmsToSopSimulator.SHOW_SOP_SIMULATOR_IF_INVISIBLE)
                        ProcessShowSOPSimulatorIfInvisible(arrDatas);
                    else if (func == InternalMessage.SdmsToSopSimulator.SHOW_HIDE_SOP_SIMULATOR)
                        ProcessShowHideSOPSimulator(arrDatas);
                    else if (func == InternalMessage.SdmsToSopSimulator.SHOW_HIDE_MISSION_STATUS)
                        ProcessShowHideMissionStatus(arrDatas);
                    else if (func == InternalMessage.SdmsToSopSimulator.OPEN_SOP_FIRE)
                        ProcessOpenSOPFire(arrDatas);
                    else if (func == InternalMessage.SdmsToSopSimulator.OPEN_SOP_PSM)
                        ProcessOpenSOPPSM(arrDatas);
                    else if (func == InternalMessage.SdmsToSopSimulator.OPEN_SOP_SECURITY)
                        ProcessOpenSOPSecurity(arrDatas);
                    else if (func == InternalMessage.SdmsToSopSimulator.ON_AFTER_LOADING_CCTV)
                        ProcessOnAfterLoadingCCTV(arrDatas);
                    else if (func == InternalMessage.SdmsToSopSimulator.SENSOR_CLOSE)
                        ProcessSensorClose(arrDatas);

                    else if (func == InternalMessage.SdmsToSopSimulator.ENABLE_CCTV)
                        ProcessEnableCCTV(arrDatas);
                    else if (func == InternalMessage.SdmsToSopSimulator.TOGGLE_SOP_BULLETIN)
                        ProcessToggleSOPBulletin(arrDatas);
                    else if (func == InternalMessage.SdmsToSopSimulator.ADD_LAST_HISTORY_DISASTER_POSITION)
                        ProcessAddLastHistoryDisasterPosition(arrDatas);
                    else if (func == InternalMessage.SdmsToSopSimulator.SET_WORK_FLOW_OPTION_POSITION)
                        ProcessSetWorkFlowOptionPosition(arrDatas);
                    else if (func == InternalMessage.SdmsToSopSimulator.EARTHQUAKE_EVENT_IS_FINISHED)
                        ProcessReply_EarthquakeEventIsFinished(arrDatas);
                    else if (func == InternalMessage.SdmsToSopSimulator.SAME_SENSORGROUP_RUNNING)
                        ProcessSameSensorGroupRunning(arrDatas);
                    else if (func == InternalMessage.SdmsToSopSimulator.SOP_POSITION_NAME)
                        ProcessSOPPositionName(arrDatas);
                }
            }
        }

        private void ProcessSOPPositionName(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 3 && arrDatas[2] is string)
            {
                string strPositionName = (string)arrDatas[2];
                StubWorker.Instance.SetSOPPositionName(strPositionName);
            }
        }

        private void ProcessSetWorkFlowOptionPosition(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 3 && arrDatas[2] is string)
            {
                string strPosition = (string)arrDatas[2];
                StubWorker.Instance.SetWorkFlowOptionPosition(strPosition);
            }
        }

        private void ProcessAddLastHistoryDisasterPosition(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 15)
            {
                if (arrDatas[2] is string && arrDatas[3] is string && arrDatas[4] is string && arrDatas[5] is string && arrDatas[6] is float
                    && arrDatas[7] is int && arrDatas[8] is int && arrDatas[9] is int && arrDatas[10] is string && arrDatas[11] is float
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

                    StubWorker.Instance.AddLastHistoryDisasterPosition(strDisasterName, strPositionName, strBroadcastName, strBuildingID, fFloorIndex, nActionStepHistoryID, nIconID, nPSMDistance, strPSMMaterial, x, y, z, nZoneID);
                }
            }
        }

        private void ProcessReply_EarthquakeEventIsFinished(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 3 && arrDatas[2] is bool)
            {
                bool isFinished = (bool)arrDatas[2];
                ProxyMessenger.Instance.EarthquakeEventIsFinished = isFinished;
            }
        }

        private void ProcessToggleSOPBulletin(ArrayList arrDatas)
        {
            StubWorker.Instance.ToggleSOPBulletin();
        }

        private void ProcessEnableCCTV(ArrayList arrDatas)
        {
            StubWorker.Instance.EnableCCTV();
        }

        private void ProcessSensorClose(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 4 && ( arrDatas[2] is int && arrDatas[3] is int))
            {
                int nSensorZoneID = (int)arrDatas[2];
                int nSensorZoneHistoryID = (int)arrDatas[3];
                StubWorker.Instance.SensorClose(nSensorZoneID, nSensorZoneHistoryID);
            }
        }

        private void ProcessSameSensorGroupRunning(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 4 && (arrDatas[2] is int && arrDatas[3] is int))
            {
                int nSensorZoneHistoryID1 = (int)arrDatas[2];
                int nSensorZoneHistoryID2 = (int)arrDatas[3];
                StubWorker.Instance.RegisterSameSensorGroupRunning(nSensorZoneHistoryID1, nSensorZoneHistoryID2);
            }
        }
        private void ProcessOnAfterLoadingCCTV(ArrayList arrDatas)
        {
            StubWorker.Instance.OnAfterLoadingCCTV();
        }

        private void ProcessOpenSOPSecurity(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 7)
            {
                if (arrDatas[2] is int && arrDatas[3] is long && arrDatas[4] is int && arrDatas[5] is int && arrDatas[6] is int)
                {
                    int nEquipZoneID = (int)arrDatas[2];
                    DateTime sopTime = DateTime.FromBinary((long)arrDatas[3]);
                    int nSensorZoneID = (int)arrDatas[4];
                    int nSensorHistoryID = (int)arrDatas[5];
                    int nSensorType = (int)arrDatas[6];

                    StubWorker.Instance.OpenSOP_Security(nEquipZoneID, sopTime, nSensorZoneID, nSensorHistoryID, nSensorType);
                }
            }
        }

        private void ProcessOpenSOPPSM(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 6)
            {
                if (arrDatas[2] is int && arrDatas[3] is long && arrDatas[4] is int && arrDatas[5] is int)
                {
                    int nEquipZoneID = (int)arrDatas[2];
                    DateTime sopTime = DateTime.FromBinary((long)arrDatas[3]);
                    int nSensorZoneID = (int)arrDatas[4];
                    int nSensorHistoryID = (int)arrDatas[5];

                    StubWorker.Instance.OpenSOP_PSM(nEquipZoneID, sopTime, nSensorZoneID, nSensorHistoryID);
                }
            }
        }

        private void ProcessOpenSOPFire(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 6)
            {
                if (arrDatas[2] is int && arrDatas[3] is long && arrDatas[4] is int && arrDatas[5] is int)
                {
                    int nZoneID = (int)arrDatas[2];
                    DateTime sopTime = DateTime.FromBinary((long)arrDatas[3]);
                    int nSensorZoneID = (int)arrDatas[4];
                    int nSensorHistoryID = (int)arrDatas[5];

                    StubWorker.Instance.OpenSOP_Fire(nZoneID, sopTime, nSensorZoneID, nSensorHistoryID);
                }
            }
        }

        private void ProcessShowHideMissionStatus(ArrayList arrDatas)
        {
            if (StubWorker.Instance.IsVisibleMissionStatus() == false)
                StubWorker.Instance.ShowMissionStatus();
            else
                StubWorker.Instance.HideMissionStatus();
        }

        private void ProcessShowHideSOPSimulator(ArrayList arrDatas)
        {
            if (StubWorker.Instance.IsVisibleSOPSimulator() == false)
                StubWorker.Instance.ShowSOPSimulator();
            else
                StubWorker.Instance.HideSOPSimulator();
        }

        private void ProcessShowSOPSimulatorIfInvisible(ArrayList arrDatas)
        {
            if (StubWorker.Instance.IsVisibleSOPSimulator() == false)
                StubWorker.Instance.ShowSOPSimulator();
        }

        private void ProcessHideSOPSimulator(ArrayList arrDatas)
        {
            StubWorker.Instance.HideSOPSimulator();
        }

        private void ProcessShowSOPSimulator(ArrayList arrDatas)
        {
            StubWorker.Instance.ShowSOPSimulator();
        }

        private void ProcessCompleteLoading(ArrayList arrDatas)
        {
            StubWorker.Instance.CompleteLoading();
        }

        private void ProcessIgnoreSOP(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 3 && arrDatas[2] is int)
            {
                int nSensorHistoryID = (int)arrDatas[2];
                StubWorker.Instance.IgnoreSOP(nSensorHistoryID);
            }
        }

        private void ProcessRunSOPSimulator(ArrayList arrDatas)
        {
            StubWorker.Instance.RunSOPSimulator();
        }

		private bool CheckValidation(byte[] bytes)
		{
			int length = bytes.Length;
			if (length < 6)
				return false;

			int nChunkCount = (int)BitConverter.ToInt16(bytes, 2);
			int nIndex = 6;

			for (int i = 0; i < nChunkCount; i++)
			{
				if (length < nIndex + 5)
					return false;

				int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

				if (length < nIndex + 5 + nDataLength)
					return false;

				nIndex += 5 + nDataLength;
			}

			if (length > nIndex)
			{
				byte[] bytes1 = new byte[nIndex];
				byte[] bytes2 = new byte[length - nIndex];

				Array.Copy(bytes, bytes1, nIndex);
				Array.Copy(bytes, nIndex, bytes2, 0, length - nIndex);

				OnReceive(bytes1);

				if (!OnReceive(bytes2))
					return false;

				m_arrReceived = null;
				return false;
			}

			return true;
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
                        Send(bytes, m_strTag);
                }
            }
            catch (Exception)
            {
                this.Close();
            }
        }

        public void SendInternalData(ArrayList arrDatas)
        {
            if (IsClientDisposed == true)
                return;

            if (IsConnected == false)
            {
                Thread.Sleep(1000);
                if (IsConnected == false)
                    return;
            }

            byte[] bytes = TcpHelper.MakeBytes(TCP_ID.INTERNAL_MESSAGE, arrDatas);
            this.Send(bytes, 0, bytes.Length);
            /*byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            m_mgr.SendMessage(SOPWebServer.Header.INTERNAL_MESSAGE, bytes);*/
        }

        public override void OnDropConnection()
        {
            m_arrTemp = null;
        }

        public new void Close()
        {
            base.Close();
            m_arrTemp = null;
        }
    }
}
