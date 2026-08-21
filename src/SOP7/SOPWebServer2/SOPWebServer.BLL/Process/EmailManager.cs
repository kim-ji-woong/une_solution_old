using System.Collections.Generic;
using AgentFactory.BLL;
using dnsData.Alarm;
using SDMS.IDAL;
using SDMS.Model.Config;
using SDMS.Model.History;
using SDMS.Model.Sensor;
using SDMS.Model.Spatial;
using dnsData.Sensor;
using Common.Model.Option;
using TeamEditor.Model.Sop.Team;
using TeamEditor.BLL;
using System.Threading;
using dnsSopID;
using dnsSMS;

namespace SOPWebServer.BLL.Process
{
    using dnsEmail;
    using Models;

    public class EmailManager : BaseEmailManager
    {
        private class EmailData
        {
            public AlarmData Alarm
            {
                get;
                set;
            }

            public string Caller
            {
                get;
                set;
            }

            public ICollection<string> ListEmail
            {
                get;
                set;
            }

            public ICollection<int> RegularMemberIDs
            {
                get;
                set;
            }

            public string Message
            {
                get;
                set;
            }

            public int SensorReactionHistoryID { get; set; }

            public EmailData(AlarmData alarm, string strCaller, ICollection<string> listEmail, ICollection<int> regularMemberIDs, string strMessage, int nSensorReactionHistoryID)
            {
                Alarm = alarm;
                Caller = strCaller;
                ListEmail = listEmail;
                RegularMemberIDs = regularMemberIDs;
                Message = strMessage;
                SensorReactionHistoryID = nSensorReactionHistoryID;
            }
        }

        private MainManager m_mainManager = null;

        public EmailManager(Factory factory, MainManager mainManager)
            : base(factory)
        {
            factory.EmailManager = this;
            m_mainManager = mainManager;
        }

        //public override int SendSMS(string strCaller, ICollection<string> phoneNumbers, string strMessage, int nSensorReactionHistoryID)
        //{
        //    SMSData data = new SMSData(null, strCaller, phoneNumbers, null, strMessage, nSensorReactionHistoryID);

        //    Thread t = new Thread(new ParameterizedThreadStart(SendSMSThread));
        //    t.Start(data);

        //    return ErrorMessageType.SUCCESS;
        //}

        public override int SendEmail(AlarmData alarm, string strCaller, ICollection<string> listEmail, ICollection<int> regularMemberIDs, string strMessage, int nSensorReactionHistoryID)
        {
            EmailData data = new EmailData(alarm, strCaller, listEmail, regularMemberIDs, strMessage, nSensorReactionHistoryID);

            Thread t = new Thread(new ParameterizedThreadStart(SendEmailThread));
            t.Start(data);

            return ErrorMessageType.SUCCESS;
        }

        private void SendEmailThread(object arg)
        {
            EmailData data = (EmailData)arg;
            string strResultMsg = "";

            IEmailClient client = EmailClientFactory.CreateMailClient();

            if (client != null)
            {
                

                EmailContent contents = new EmailContent();
                contents.Caller = data.Caller;
                contents.EmailList.AddRange(data.ListEmail);
                contents.Message = data.Message;
                //contents.Tag = data.DBManager;
                contents.SensorReactionHistoryID = data.SensorReactionHistoryID;


                string strSubject = "";

                if (data.Message != null)
                {   // 이메일 제목에 개행 문자 제거 및 길이 제한(100글자 이내)
                    strSubject = data.Message;
                    strSubject = strSubject.Replace("\n", " ");

                    if (strSubject.Length > 100)
                    {
                        strSubject = strSubject.Substring(0, 100);
                    }
                }
                
                //contents.Title = data.Message;
                contents.Subject = strSubject;
                contents.TimeStamp = data.Alarm.TimeStamp;



                // 수신자번호 가운데 빈문자열이 있으면 없앤다.
                int nIndex = contents.EmailList.IndexOf("");

                if (nIndex >= 0)
                    contents.EmailList.RemoveAt(nIndex);

                //if (client.SendEmail(contents))
                //{
                    //if (data.Alarm != null)
                    //{
                    //    List<int> regularMemberIDs = new List<int>();
                    //    regularMemberIDs.AddRange(data.RegularMemberIDs);
                    //    SaveSMSHistory(data.Alarm, regularMemberIDs, data.Message);
                    //}
                //}
                client.SendEmail(contents, ref strResultMsg);
            }
        }

        private void SaveSMSHistory(AlarmData alarm, List<int> regularMemberIds, string strMessage)
        {
            m_mainManager.SDMSDataManager.GetCreateManager().CreateSMSHistory(alarm.SensorZoneHistoryID, alarm.SensorReactionHistoryID, strMessage, true, regularMemberIds);
        }
    }
}
