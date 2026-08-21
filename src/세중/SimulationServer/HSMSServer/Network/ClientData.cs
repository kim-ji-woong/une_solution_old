using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;

namespace HSMSServer
{
    public class ClientData
    {
        private ServiceProvider m_provider = null;
        private byte[] m_arrTempBytes = null;
        private ConnectionState m_state;
        
        public ClientData(ServiceProvider provider, ConnectionState state)
        {
            m_provider = provider;
            m_state = state;
        }

        public void OnReceivedData(byte[] bytes)
        {
            CheckValidation(bytes);
        }

        private void OnReceive(byte[] bytes)
        {
            string strBody = "";
            string strHeader = GetHeaderString(bytes, ref strBody);

            if (strHeader == null)
                return;

            m_provider.RecvLog(bytes, strHeader + ":" + strBody, m_state);

            if (strHeader == "#SYSALARM")
            {
                ProcessSysAlarm(strBody);
            }
        }

        private bool ProcessSysAlarm(string strData)
        {
            string[] arrDatas = strData.Split(',');

            int nDataCount = arrDatas.Count();

            if (nDataCount != 3)
                return false;

            string strDeviceType = arrDatas[0];
            string strDeviceID = arrDatas[1];
            string strAlarmCode = arrDatas[2];

            bool result = FormMain.Instance.DataManager.ProcessSysAlarm(strDeviceID, strAlarmCode);

            string strResult = result ? "0" : "-1";
            string strSend = "#SYSALARM:ALL," + strDeviceID + "," + strResult;
            
            return m_provider.Send(strSend, m_state);
        }

        private string GetHeaderString(byte[] bytes, ref string strBody)
        {
            // EUC-KR : 51949
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            string strReceived = encEUC_KR.GetString(bytes);

            int nIndex = strReceived.IndexOf(':');

            if (nIndex < 0)
                return null;

            strBody = strReceived.Substring(nIndex + 1);
            return strReceived.Substring(0, nIndex);
        }

        private void CheckValidation(byte[] bytes)
        {
            int nArrCount = bytes.Count();

            for (int i = 0; i < nArrCount; i++)
            {
                byte b = bytes[i];

                if (b == 0x0d)
                {
                    if (i == nArrCount - 1)
                    {
                        OnReceive(ConcatenateBytes(ref m_arrTempBytes, bytes));
                        return;
                    }
                    else
                    {
                        if (i > 0)
                        {
                            byte[] arrBytes1 = new byte[i];
                            System.Buffer.BlockCopy(bytes, 0, arrBytes1, 0, i);
                            OnReceive(ConcatenateBytes(ref m_arrTempBytes, arrBytes1));
                        }
                        else
                            m_arrTempBytes = null;

                        int nCount2 = nArrCount - i - 1;

                        if (nCount2 > 0)
                        {
                            byte[] arrBytes2 = new byte[nCount2];
                            System.Buffer.BlockCopy(bytes, i + 1, arrBytes2, 0, nCount2);
                            CheckValidation(arrBytes2);
                        }

                        return;
                    }
                }
            }

            m_arrTempBytes = bytes;
        }

        private byte[] ConcatenateBytes(ref byte[] bytes1, byte[] bytes2)
        {
            if (bytes1 == null)
                return bytes2;

            int nCount1 = bytes1.Count();

            if (nCount1 == 0)
            {
                bytes1 = null;
                return bytes2;
            }

            int nCount2 = bytes2 == null ? 0 : bytes2.Count();

            byte[] arrBytes = new byte[nCount1 + nCount2];

            System.Buffer.BlockCopy(bytes1, 0, arrBytes, 0, nCount1);

            if (nCount2 > 0)
                System.Buffer.BlockCopy(bytes2, 0, arrBytes, nCount1, nCount2);

            bytes1 = null;
            return arrBytes;
        }
    }
}
