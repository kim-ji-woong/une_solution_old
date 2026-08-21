using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpLib2;

namespace SOPSimulator.Network
{
    public class TrainingLinkManager
    {
        private ClientProvider m_provider = new ClientProvider();

        private Thread m_ConnThread = null;
        private bool m_shutdownThread = false;
        private bool m_connectThread = true;

        private string m_strURL = "127.0.0.1";
        private int m_nPort = 1470;

        private static TrainingLinkManager m_instance = null;
        public static TrainingLinkManager Instance
        {
            get { return m_instance; }
        }

        public TrainingLinkManager()
        {
            m_provider.LengthAdd = false;

            m_ConnThread = new Thread(new ThreadStart(ConnectionThread));
            m_ConnThread.Name = "TrainingLink.Connection";
            m_ConnThread.Start();
        }

        public void Shutdown()
        {
            m_shutdownThread = true;
        }

        public void ReConnect()
        {
            m_connectThread = true;
        }

        private void ConnectionThread()
        {
            while (!m_shutdownThread)
            {
                if (m_connectThread)
                {
                    if (m_provider.Connect(m_strURL, m_nPort))
                    {
                        m_connectThread = false;
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public bool SendOpenData()
        {
            short nHeader = Header.SIMULATOR_OPEN;
            int nData = 0;

            byte[] arrHeader = BitConverter.GetBytes(nHeader);
            byte[] arrData = BitConverter.GetBytes(nData);

            byte[] datas = new byte[arrHeader.Length + arrData.Length];

            Array.Copy(arrHeader, 0, datas, 0, arrHeader.Length);
            Array.Copy(arrData, 0, datas, arrHeader.Length, arrData.Length);

            try
            {
                m_provider.Client.Client.Send(datas, 0, datas.Length, SocketFlags.None);

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public void OnReceive()
        {
            if (m_shutdownThread)
                return;

            short nType = Header.NONE;
            byte[] bytes = m_provider.ReceivedData;

            if (bytes == null)
                return;

            ArrayList arrDatas = TcpHelper.ReadBytes(bytes, out nType);

            if (nType == Header.POPUP_OUTBREAK)
            {
                ProcessPopupOutbreak(arrDatas);
            }
        }

        private bool CheckHeader(byte[] arrHeader, out short type)
        {
            byte[] arrMsgCode = new byte[2];

            short nMsgCode = 0;

            Array.Copy(arrHeader, 0, arrMsgCode, 0, arrMsgCode.Length);

            type = BitConverter.ToInt16(arrMsgCode, 0);

            return true;
        }

        private void ProcessPopupOutbreak(ArrayList arrDatas)
        {
            if (arrDatas.Count == 4 && arrDatas[0] is string && arrDatas[1] is string && arrDatas[2] is string && arrDatas[3] is long)
            {
                string strProjectName = (string)arrDatas[0];
                string strLevelName = (string)arrDatas[1];
                string strSpaceName = (string)arrDatas[2];

                long nTime = (long)arrDatas[3];
                string strTime = nTime.ToString();
                //DateTime time = DateTime.FromBinary((long)arrDatas[3]);

                string strValue = '"' + strProjectName + '"' + ' ' + '"' + strLevelName + '"' + ' ' + '"' + strSpaceName + '"' + ' ' + '"' + strTime + '"';

                // 팝업 띄우기
                ExecuteManager2 exeMgr2 = new ExecuteManager2();
                exeMgr2.Run(ExecuteManager2.APP_TYPE.OUTBREAK_INFO, strValue);
            }
        }
    }

    public class Header
    {
        public const short NONE = 0;
        public const short POPUP_OUTBREAK = 101;
        public const short SIMULATOR_OPEN = 102;

    }

}
