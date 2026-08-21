using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HSMSServer2
{
    public class ClientProvider : TcpLib2.ClientServiceProvider
    {
        private static bool m_bDBReloading = false;
        public static bool DatabaseReloading
        {
            get { return m_bDBReloading; }
            set { m_bDBReloading = value; }
        }

        private NetworkClient m_netMgr = null;
        private byte[] m_arrTempBytes = null;

        private int m_nPingCount = 0;
        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        public ClientProvider(NetworkClient mgr)
        {
            m_netMgr = mgr;
            LengthAdd = false;
        }

        public override void OnReceiveData()
        {
            m_nPingCount = 0;
            CheckValidation(ReceivedData);
        }

        public override void OnDropConnection()
        {
            //FormMain.Instance.OnDropConnection();
        }

        private void OnReceive(byte[] bytes)
        {
            string strBody = "";
            string strHeader = GetHeaderString(bytes, ref strBody);

            if (strHeader == null)
                return;

            m_netMgr.RecvLog(bytes, strHeader + ":" + strBody);

            if (strHeader == "#DEVINFO")
            {
                if(m_bDBReloading == false)
                {
                    ProcessLocInfo(strBody);
                }                
            }
            else if (strHeader == "#SYSALARM")
            {
                ProcessSysAlarm(strBody);
            }
        }

        public void SendPing()
        {
            //string strMsg = "#SYSALARM:0,0";
            string strMsg = "#SYSALARM:ALL,0,0";

            // EUC-KR : 51949
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            byte[] bytes = encEUC_KR.GetBytes(strMsg + "\r").ToArray();

            m_netMgr.Send(bytes, strMsg, this);
        }

        private bool ProcessSysAlarm(string strData)
        {
            string[] arrDatas = strData.Split(',');

            int nDataCount = arrDatas.Count();

            if (nDataCount < 3)
                return false;

            string strDeviceType = arrDatas[0];
            string strDeviceID = arrDatas[1];
            int nErrorCode;
            
            if (!int.TryParse(arrDatas[2], out nErrorCode))
                return false;
            
            // m_netMgr 전달

            return true;
        }

        private bool ProcessLocInfo(string strData)
        {
            string[] arrDatas = strData.Split(',');

            int nDataCount = arrDatas.Count();

            if (nDataCount < 9)
                return false;

            string strDeviceType = arrDatas[0];
            string strDeviceID = arrDatas[1];
            string strDeviceStatus = arrDatas[2];

            double x, y, latitude, longitude, methaneGas, coGas;

            if (!double.TryParse(arrDatas[3], out x))
                return false;
            if (!double.TryParse(arrDatas[4], out y))
                return false;
            if (!double.TryParse(arrDatas[5], out latitude))
                return false;
            if (!double.TryParse(arrDatas[6], out longitude))
                return false;
            if (!double.TryParse(arrDatas[7], out methaneGas))
                return false;
            if (!double.TryParse(arrDatas[8], out coGas))
                return false;

            double tx = x;
            double ty = y;
            TransformCoord(x, y, out tx, out ty);

            if (strDeviceType == "LT")
                m_netMgr.OnReceiveSensorLocation(strDeviceID, tx, ty);
            else if (strDeviceType == "GS")
                m_netMgr.OnReceiveSensorGas(strDeviceID, methaneGas, coGas);
            else
                m_netMgr.OnReceiveSensorData(strDeviceID, tx, ty, methaneGas, coGas);

            return true;
        }

        private void TransformCoord(double x, double y, out double tx , out double ty)
        {
            tx = x;
            ty = y;

            //float originX = ModelManager.Instance.OriginX;
           // float originY = ModelManager.Instance.OriginY;

            float minX = ModelManager.Instance.MinX;
            float minY = ModelManager.Instance.MinY;

            float maxX = ModelManager.Instance.MaxX;
            float maxY = ModelManager.Instance.MaxY;

            float dy = maxY - minX;
            float dx = maxX - minY;

            float t1 = (float)x / dx;
            float t2 = (float)y / dy;

            float w = t1 * ModelManager.Instance.Width;
            float h = t2 * ModelManager.Instance.Height;

            tx = w;
            ty = h;
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

        private string GetHeaderString(byte[] bytes, ref string strBody)
        {
            // EUC-KR : 51949
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            string strReceived = encEUC_KR.GetString(bytes);

            int nIndex = strReceived.IndexOf(':');

            if (nIndex < 0)
                return null;

            strBody = strReceived.Substring(nIndex + 1);
            strBody = strBody.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });
            return strReceived.Substring(0, nIndex);
        }
    }
}
