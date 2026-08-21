using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
using libSMS;

namespace AlarmButtonSimulator
{
    public class NetworkManager
    {
        // nIndex : 1이면 1번, 3이면 3번 버튼
        public void OnReceive(int nIndex)
        {
            bool useSMS, useBroadcast, useBroadcastSiren;
            string strSMSMessage, strBroadcastMessage, strSMSCaller;
            List<string> phoneNumbers = FormMain.Instance.GetMessageInfo(nIndex, out useSMS, out useBroadcast, out useBroadcastSiren, out strSMSMessage, out strBroadcastMessage, out strSMSCaller);

            if (useSMS && phoneNumbers != null && strSMSMessage.Length > 0)
            {
                new Thread(() =>
                {
                    SendSMS(phoneNumbers, strSMSCaller, strSMSMessage);
                }).Start();
            }

            if (useBroadcast && strBroadcastMessage.Length > 0)
            {
                RunBroadcast(strBroadcastMessage, useBroadcastSiren, FormMain.Instance.SiteID);
            }
        }

        private void RunBroadcast(string strBroadcastMessage, bool useBroadcastSiren, int nSiteID)
        {
            string strFormat = "Insert into Broadcast (Text, UseSiren, PlayOption, RepeatCount, AddTime, SiteID) ";
            strFormat += "values ('{0}', {1}, {2}, {3}, '{4} {5:00}:{6:00}:{7:00}', {8})";

            DateTime dtNow = DateTime.Now;
            string strSQL = string.Format(strFormat, strBroadcastMessage, useBroadcastSiren ? 1 : 0, 1, 1, dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second, nSiteID);
            FormMain.Instance.DBManager.GetResultData(strSQL, 0);
        }

        private bool SendSMS(List<string> phoneNumbers, string strSendPhoneNumber, string strMsg)
        {
            ArrayList arrPhoneNumbers = new ArrayList();
            arrPhoneNumbers.AddRange(phoneNumbers);

            // 비번인 근무자는 제외한다.
            ArrayList arrValidPhoneNumbers = ControlTeamEditor.VaildMemberPhoneNumber.IsVaildPhoneNumber(arrPhoneNumbers, FormMain.Instance.DBManager);

            if (arrValidPhoneNumbers == null)
                return true;

            System.Diagnostics.Trace.WriteLine("SendSMS Thread");

            string strServerIP = GetServerIP(FormMain.Instance.SiteID);
            IMessageClient messageClient = MessageClientFactory.CreateMessageClient(FormMain.Instance.SiteID, strServerIP);

            if (messageClient == null)
                return false;

            int nMessageLength = messageClient.GetMessageLength();

            // 메시지가 길이제한을(80바이트) 넘어서 여러개로 쪼개어질 경우
            // [메시지 Index/메시지 개수]를 메시지 앞에 붙여넣도록 한다.
            ArrayList arrMessages = (new MessageDivider(nMessageLength)).MakeMessageList(strMsg);
            //ArrayList arrMessages = MakeMessageList(strMsg);

            messageClient.BeginSend();

            foreach (string szMsg in arrMessages)
            {
                foreach (string szPhone in arrValidPhoneNumbers)
                {
                    messageClient.SendSMS(strSendPhoneNumber, szPhone, szMsg);
                }
            }

            messageClient.EndSend();
            System.Diagnostics.Trace.WriteLine("SendSMS Thread Finish");

            return true;
        }

        private string GetServerIP(int nSiteID)
        {
            string strServerURL = DBUtility.RegUtil.ReadRegValue("Server Connection Info", "webserver_url", nSiteID);
            if (strServerURL == null || strServerURL == "")
                strServerURL = FormMain.Instance.DBManager.WebServerURL;

            int nIndex1 = strServerURL.IndexOf("http://");
            int nIndex2 = strServerURL.LastIndexOf(':');
            string strURL = strServerURL;

            if (nIndex1 >= 0 && nIndex2 >= 0)
            {
                int nBeginIndex = nIndex1 + "http://".Length;
                strURL = strServerURL.Substring(nBeginIndex, nIndex2 - nBeginIndex);
            }
            else if (nIndex1 >= 0)
            {
                int nBeginIndex = nIndex1 + "http://".Length;
                strURL = strServerURL.Substring(nBeginIndex);
            }
            else if (nIndex2 >= 0)
            {
                strURL = strServerURL.Substring(0, nIndex2);
            }

            System.Net.IPAddress[] addr = System.Net.Dns.GetHostAddresses(strURL);
            return addr[0].ToString();
        }
    }
}
