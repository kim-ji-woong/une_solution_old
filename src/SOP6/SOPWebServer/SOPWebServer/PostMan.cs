using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace SOPWebServer
{
    public class PostMan : ServerProcess.IPostMan
    {
        private IPostMan m_postMan = null;
        private ServerProcess.Client.ClientData m_clientData = null;

        public IClientChannel ClientChannel
        {
            get { return (IClientChannel)m_postMan; }
        }

        public ServerProcess.Client.ClientData ClientData
        {
            get { return m_clientData; }
            set { m_clientData = value; }
        }

        public PostMan()
        {
        }

        public PostMan(IPostMan postMan)
        {
            m_postMan = postMan;
        }

        public void OnRing(int header, byte[] messages)
        {
            if (m_postMan != null)
            {
                m_postMan.OnRing(header, messages);
                SendLog(header, messages);
            }
        }

        private void SendLog(int header, byte[] messages)
        {
            // 정기적으로 보내는 신호는 굳이 기록하지 않는다.
            if (header == SOPWebServer.Header.ARE_YOU_THERE || header == SOPWebServer.Header.I_AM_HERE || header == SOPWebServer.Header.CONTROL_CLIENT)
                return;

            string strClientType = "Unknown", strClientSubType = "Unknown", strSessionID = "Unknown";
            string strIP = "";
            int nPort = -1;

            if (m_clientData != null)
            {
                strClientType = SOPWebServer.ClientType.ToString(m_clientData.ClientType);
                strClientSubType = SOPWebServer.ClientSubType.ToString(m_clientData.ClientSubType);
                strSessionID = m_clientData.SessionID;
                strIP = m_clientData.IP;
                nPort = m_clientData.Port;
            }

            string strLog = "";

            if (messages == null)
            {
                strLog = string.Format("SendMessage to {3}:{4} : Header({0}), Length(0) to ClientType({1}), ClientSubType({2})",
                    header,
                    strClientType, strClientSubType, strIP, nPort);
            }
            else
            {
                strLog = string.Format("SendMessage to {5}:{6} : Header({0}), Length({1}) to ClientType({2}), ClientSubType({3})\r\n{4}",
                    header, messages.Count(),
                    strClientType, strClientSubType,
                    Logger.GetByteString(messages),
                    strIP, nPort);
            }

            PostOffice.Instance.GetLogger().Write(strLog);
        }
    }
}