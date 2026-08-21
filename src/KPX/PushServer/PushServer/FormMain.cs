using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Collections;

namespace PushServer
{
    public partial class FormMain : Form
    {
        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        private DBUtility.WebDBManager dbMgr;
        Timer timer = null;

        Dictionary<string, DateTime> m_dicAlarmSend = new Dictionary<string, DateTime>();

        //ini
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, int def, StringBuilder retVal, int size, string filePath); 

        /// <summary>
        /// 문자 받을 핸드폰 번호 List
        /// </summary>
        //List<string> adminPhoneNumbers = new List<string>();
        /// <summary>
        /// PUSH 알람 받을 Device Id List
        /// </summary>
        List<MobileDevice> m_devices = new List<MobileDevice>();
        //List<string> deviceIDs = new List<string>();

        public FormMain()
        {
            InitializeComponent();

            SetSystemLog("PUSH SERVER START");

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.ContextMenuStrip = contextMenuStrip1;

            dbMgr = new DBUtility.WebDBManager(500);
            // Release
            dbMgr.DatabaseHost = "127.0.0.1";
            // Test
            //dbMgr.DatabaseHost = "192.168.0.211";
             
            InitIniFile();
            
            // TEST
            //WritePrivateProfileString("AlarmID", "PipeMaxAlarmId", "1542", Application.StartupPath + @"\MaxAlarmId.ini"); 

            this.timer = new Timer();
            this.timer.Interval = 1000;
            this.timer.Tick += timer_Tick;
            this.timer.Start();
        }
         
        private void InitIniFile()
        {
            try
            {
                string query = string.Format("select ID from alarmhistory order by id desc limit 1");
                ArrayList arrResult = dbMgr.GetResultData(query, 0);
                if (arrResult != null && arrResult.Count > 0)
                {
                    string maxAlarmID = DBUtility.WebDBManager.GetStringField(arrResult[0]);
                    WritePrivateProfileString("AlarmID", "PipeMaxAlarmId", maxAlarmID, Application.StartupPath + @"\MaxAlarmId.ini");
                }

                query = string.Format("select ID from tankleakhistory order by id desc limit 1");
                arrResult = dbMgr.GetResultData(query, 0);
                if (arrResult != null && arrResult.Count > 0)
                {
                    string maxAlarmID = DBUtility.WebDBManager.GetStringField(arrResult[0]);
                    WritePrivateProfileString("AlarmID", "SulfuricMaxAlarmId", maxAlarmID, Application.StartupPath + @"\MaxAlarmId.ini");
                }
            }
            catch (Exception ex)
            {
                SetSystemLog("[ERROR] InitIniFile() : " + ex.Message);
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                //DisplayUseSms();

                //bool readDevices = false;
                //DisplayAdmin();
                int maxMin = 60;
                string query = "SELECT PropertyValue FROM options WHERE PropertyName='SMSDuration'";
                ArrayList arrResult = dbMgr.GetResultData(query, 0);
                if (arrResult != null && arrResult.Count != 0)
                    maxMin = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 60);

                DisplayCertRequest();
                if (certRequest)
                {
                    SendMessage("PTMS에서 새로운 승인 요청이 있습니다.", certRequestAdminPhoneNumber);
                }

                // 온도, 압력, 유량, 레벨 알람.
                DisplayPipeAlarm();
                if (alertAlarmList != null && alertAlarmList.Count > 0)
                {
                    DisplayUseSms(); 
                    DisplayAdmin(); 
                    //readDevices = true;
                    //List<string> phoneNumbers = new List<string>();

                    //foreach (MobileDevice device in m_devices)
                    //{
                    //    if (device.UserGroup != null && device.UserGroup.UsePipe)
                    //        phoneNumbers.Add(device.PhoneNumber);
                    //}

                    for (int i = 0; i < alertAlarmList.Count; i++)
                    {
                        StringBuilder msg = new StringBuilder();
                        StringBuilder smsMsg = new StringBuilder();

                        string range = "-";
                        if (alertAlarmList[i].StandardValue != -999 && alertAlarmList[i].StandardValue != -9999)
                            range = String.Format("{0:F1}", alertAlarmList[i].StandardValue - Math.Abs(alertAlarmList[i].StandardRange)) + "~" + String.Format("{0:F1}", alertAlarmList[i].StandardValue + Math.Abs(alertAlarmList[i].StandardRange));

                        //탱크온도상승 = 1,
                        //탱크온도하강 = 2,
                        //탱크최고레벨 = 4,
                        ////탱크유량증가 = 8,
                        ////탱크유량감소 = 16,
                        //압력상승 = 256,
                        //압력하강 = 512,
                        //유량증가 = 1024,
                        //유량감소 = 2048

                        smsMsg.AppendLine("[" + alertAlarmList[i].strStatus + "]");

                        string tName = "";
                        if (alertAlarmList[i].TankName != null)
                            tName = alertAlarmList[i].TankName;
                        string pName = "";
                        if (alertAlarmList[i].PipeName != null)
                            pName = alertAlarmList[i].PipeName;
                        string status = alertAlarmList[i].Status.ToString();

                        //PUSH
                        string msgType = "";
                        if (alertAlarmList[i].Status == 1)
                        {
                            msgType = "TankAlarm";
                            msg.AppendLine(msgType);
                            msg.AppendFormat(" {0},{1},{2},{3}", alertAlarmList[i].TankName, alertAlarmList[i].strStatus, String.Format("{0:F1}", alertAlarmList[i].RealValue), alertAlarmList[i].StandardValue + "도 이상");

                            smsMsg.AppendFormat("{0},{1},{2}", alertAlarmList[i].TankName, String.Format("{0:F1}", alertAlarmList[i].RealValue), alertAlarmList[i].StandardValue + "도 이상");
                        }
                        else if (alertAlarmList[i].Status == 2)
                        {
                            msgType = "TankAlarm";
                            msg.AppendLine(msgType);
                            msg.AppendFormat(" {0},{1},{2},{3}", alertAlarmList[i].TankName, alertAlarmList[i].strStatus, String.Format("{0:F1}", alertAlarmList[i].RealValue), alertAlarmList[i].StandardValue + "도 이하");

                            smsMsg.AppendFormat("{0},{1},{2}", alertAlarmList[i].TankName, String.Format("{0:F1}", alertAlarmList[i].RealValue), alertAlarmList[i].StandardValue + "도 이하");
                        }
                        else if (alertAlarmList[i].Status == 4)
                        {
                            msgType = "TankAlarm";
                            msg.AppendLine(msgType);
                            msg.AppendFormat(" {0},{1},{2},{3}", alertAlarmList[i].TankName, alertAlarmList[i].strStatus, String.Format("{0:F1}", alertAlarmList[i].RealValue), alertAlarmList[i].StandardValue + "이상");

                            smsMsg.AppendFormat("{0},{1},{2}", alertAlarmList[i].TankName, String.Format("{0:F1}", alertAlarmList[i].RealValue), alertAlarmList[i].StandardValue + "이상");
                        }
                        else if (alertAlarmList[i].Status == 256 || alertAlarmList[i].Status == 512)
                        {
                            msgType = "PipeAlarm";
                            msg.AppendLine(msgType);
                            msg.AppendFormat(" {0},{1},{2},{3}", alertAlarmList[i].PipeName, alertAlarmList[i].strStatus, String.Format("{0:F1}", alertAlarmList[i].RealValue), "(" + range + ")");

                            smsMsg.AppendFormat("{0},{1},{2}", alertAlarmList[i].PipeName, String.Format("{0:F1}", alertAlarmList[i].RealValue), "(" + range + ")");
                        }
                        else if (alertAlarmList[i].Status == 1024 || alertAlarmList[i].Status == 2048)
                        {
                            msgType = "TankAlarm";
                            msg.AppendLine(msgType);
                            msg.AppendFormat(" {0},{1},{2},{3}", alertAlarmList[i].TankName, alertAlarmList[i].strStatus, String.Format("{0:F1}", alertAlarmList[i].RealValue), "(" + range + ")");

                            smsMsg.AppendFormat("{0},{1},{2}", alertAlarmList[i].TankName, String.Format("{0:F1}", alertAlarmList[i].RealValue), "(" + range + ")");
                        }

                        // 지정된 시간 이내일 경우 메세지를 다시 보내지 않는다.
                        string key = MakeKey(tName, pName, status);
                        if (m_dicAlarmSend.ContainsKey(key))
                        {
                            DateTime value = DateTime.Now;
                            if (!m_dicAlarmSend.TryGetValue(key, out value))
                                continue;

                            DateTime now = DateTime.Now;
                            TimeSpan ts = now - value;
                            int min = (int)ts.TotalMinutes;
                            //int min = ts.Minutes * 60 + ts.Seconds;

                            if (min >= maxMin)
                            {
                                SendMsg(msgType, smsMsg.ToString(), msg.ToString());
                                m_dicAlarmSend[key] = now;
                            }
                        }
                        else
                        {
                            SendMsg(msgType, smsMsg.ToString(), msg.ToString());
                            m_dicAlarmSend.Add(key, DateTime.Now);
                        }
                        //
                    }
                }

                // 황산 알람
                DisplaySulfuricAlarm();
                if (sulfuricAlarmList != null && sulfuricAlarmList.Count > 0)
                {
                    DisplayUseSms();
                    DisplayAdmin();

                    for (int i = 0; i < sulfuricAlarmList.Count; i++)
                    {
                        StringBuilder msg = new StringBuilder();
                        StringBuilder smsMsg = new StringBuilder();

                        smsMsg.AppendLine("[황산 누출]");

                        string tName = "";
                        if (sulfuricAlarmList[i].TankName != null)
                            tName = sulfuricAlarmList[i].TankName;

                        //PUSH
                        string msgType = "황산 누출";
                        msg.AppendLine(msgType);
                        msg.AppendFormat(" {0}", sulfuricAlarmList[i].TankName);
                        smsMsg.AppendFormat("{0}", sulfuricAlarmList[i].TankName);

                        // 지정된 시간 이내일 경우 메세지를 다시 보내지 않는다.
                        string key = tName;
                        if (m_dicAlarmSend.ContainsKey(key))
                        {
                            DateTime value = DateTime.Now;
                            if (!m_dicAlarmSend.TryGetValue(key, out value))
                                continue;

                            DateTime now = DateTime.Now;
                            TimeSpan ts = now - value;
                            int min = (int)ts.TotalMinutes;
                            //int min = ts.Minutes * 60 + ts.Seconds;

                            if (min >= maxMin)
                            {
                                SendMsg(msgType, smsMsg.ToString(), msg.ToString());
                                m_dicAlarmSend[key] = now;
                            }
                        }
                        else
                        {
                            SendMsg(msgType, smsMsg.ToString(), msg.ToString());
                            m_dicAlarmSend.Add(key, DateTime.Now);
                        }
                        //
                    }
                }
            }
            catch (Exception ex)
            {
                SetSystemLog("[ERROR] timer_Tick(): " + ex.Message);
            }
        }

        private void SendMsg(string msgType, string strMsg, string msg)
        {
            try
            { 
                if (UseSms && strMsg.Length != 0)
                {
                    //List<string> phoneNumbers = new List<string>();
                    foreach (MobileDevice device in m_devices)
                    {
                        if (msgType == "TankAlarm" && device.UserGroup != null && device.UserGroup.UseTank && device.UserGroup.UseTankAlarm)
                            SendMessage(strMsg, device.PhoneNumber);
                        else if (msgType == "PipeAlarm" && device.UserGroup != null && device.UserGroup.UsePipe && device.UserGroup.UsePipeAlarm)
                            SendMessage(strMsg, device.PhoneNumber);
                        else if (msgType == "황산 누출" && device.UserGroup != null && device.UserGroup.UseTank && device.UserGroup.UseSulfuric)
                            SendMessage(strMsg, device.PhoneNumber); 
                    }
                    //SendMessage(smsMsg.ToString(), phoneNumbers); 
                }

                if (msg.Length == 0) return;

                foreach (MobileDevice device in m_devices)
                {
                    if (msgType == "TankAlarm" && device.UserGroup != null && device.UserGroup.UseTank && device.UserGroup.UseTankAlarm)
                        SendNotification(device.DeviceID, msgType, msg);
                    else if (msgType == "PipeAlarm" && device.UserGroup != null && device.UserGroup.UsePipe && device.UserGroup.UsePipeAlarm)
                        SendNotification(device.DeviceID, msgType, msg);
                    else if (msgType == "황산 누출" && device.UserGroup != null && device.UserGroup.UseTank && device.UserGroup.UseSulfuric)
                        SendNotification(device.DeviceID, msgType, msg);
                    //if (device.UserGroup != null && device.UserGroup.UsePipe)
                    //    SendNotification(device.DeviceID, msgType, msg.ToString());
                }
            }
            catch (Exception ex)
            {
                SetSystemLog("[ERROR] SendMsg(string, string, string) : " + ex.Message);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            //string strDeviceID = "eRYP8lcbEMI:APA91bEBbe6KjFdERCm5fddJyJgzZWA-5DwJRA9SC0HrKRbxdy3fe3MSWpb_Esj-DgMtKMwksGlKTWUQ-ywyfNQ_wHykqydZCi_sQjoK_S9FaJjqO2kSVGJ1bixGViJzhhqSg2KmD5wy"; // 김지웅
            string strDeviceID = "eT1DB8w0jjs:APA91bH4JBxX8W2gjQpCXLe4eCcQqTZ3l3UZr9EqR7H9y5aeZ55tiiS3iWPzH6PjhTEf8lfDFLLTqt7lHtID_-Dwa97fBmCTXa0s5bKNQzQRQLsGAKVFw7f1gDpnX4TOSoF6vYtDp-O8";
            SendNotification(strDeviceID, "PTMS 메시지", textBoxMessage.Text);
            //SendNotification("app registration token key string 152 bytes here", textBoxMessage.Text);
        }

        #region 문자전송할 관리자 조회
        private void DisplayAdmin()
        {
            try
            {
                m_devices.Clear();

                Dictionary<int, UserGroup> dicUserGroups = new Dictionary<int, UserGroup>();

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT CASE  ");
                sb.Append("        WHEN CompanyMemberID IS NULL AND ExternalMemberID IS NULL ");
                sb.Append("        THEN PhoneNumber  ");
                sb.Append("        WHEN CompanyMemberID IS NULL AND ExternalMemberID IS NOT NULL ");
                sb.Append("        THEN (SELECT PhoneNumber FROM ExternalCompanyMember as ecm WHERE ecm.ID=u.ExternalMemberID) ");
                sb.Append("        WHEN CompanyMemberID IS NOT NULL AND ExternalMemberID IS NULL ");
                sb.Append("        THEN (SELECT PhoneNumber FROM CompanyMember as cm WHERE cm.ID=u.CompanyMemberID) ");
                sb.Append("      END as PhoneNumber, DeviceID, ug.ID, ug.GroupName, ug.PipeAccess, ug.PipeItems, ug.TankAccess, ug.TankItems");
                sb.Append(" FROM User as u, UserGroup as ug ");
                sb.Append("WHERE u.UserGroup = ug.ID and Mobile=1 AND IsSMs=1 ");

                ArrayList arrResult = dbMgr.GetResultData(sb.ToString(), 0);
                for (int i = 0; i < arrResult.Count - 7; i += 8)
                {
                    string strPhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i]);
                    string strDeviceID = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);
                    DBUtility.VariousData<int> userGroupID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString());
                    string strGroupName = DBUtility.WebDBManager.GetStringField(arrResult[i + 3]);
                    DBUtility.VariousData<int> pipeAccess = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString());
                    string strPipeItems = DBUtility.WebDBManager.GetStringField(arrResult[i + 5]);
                    string strTankAccess = DBUtility.WebDBManager.GetStringField(arrResult[i + 6]);
                    string strTankItems = DBUtility.WebDBManager.GetStringField(arrResult[i + 7]);

                    if (strPhoneNumber == null || strDeviceID == null || userGroupID == null || pipeAccess == null ||
                        userGroupID == null || strGroupName == null || strTankAccess == null || strTankItems == null)
                        continue;

                    UserGroup group = null;

                    if (dicUserGroups.TryGetValue(userGroupID.Data, out group) == false)
                    {
                        group = new UserGroup();
                        group.ID = userGroupID.Data;
                        group.GroupName = strGroupName;
                        group.UsePipe = pipeAccess.Data == 1;
                        string[] arrPipeAlarm = strPipeItems.Split(',');
                        group.UsePipeAlarm = arrPipeAlarm.Contains("1");
                        group.UseTank = strTankAccess.Trim().Length > 0;
                        group.UseTankAlarm = strTankItems.Contains("9");
                        group.UseSulfuric = strTankItems.Contains("11");
                    }

                    MobileDevice device = new MobileDevice();
                    device.DeviceID = strDeviceID;
                    device.UserGroup = group;

                    string strDECPhoneNumber = DBUtility.AES256Cipher.AES_decrypt(strPhoneNumber, key);
                    device.PhoneNumber = strDECPhoneNumber;

                    m_devices.Add(device);
                }
            }
            catch (Exception ex)
            {
                SetSystemLog("[ERROR] DisplayAdmin() : " + ex.Message);
            }
        } 
        #endregion

        #region PUSH
        private string SendNotification(string deviceId, string strTitle, string message)
        {
            //if (deviceId != "eT1DB8w0jjs:APA91bH4JBxX8W2gjQpCXLe4eCcQqTZ3l3UZr9EqR7H9y5aeZ55tiiS3iWPzH6PjhTEf8lfDFLLTqt7lHtID_-Dwa97fBmCTXa0s5bKNQzQRQLsGAKVFw7f1gDpnX4TOSoF6vYtDp-O8") return "";
            string SERVER_API_KEY = "AAAAu97zr8E:APA91bFwR605Gsk_WmWQmnvvAcQGoRE_zlFnBXNH0v3LsPzgA-WthiYpVLNXe6YgIxc5-mLwXyHL0bnSvzOxsGfymbKdeeHyAPpi0KQR3TTvqPx5siemrgMUTJKReZryQr-mabibmJTo";

            var value = message;
            string resultStr = "";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8;";
                request.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));

                var postData =
                new
                {
                    /*data = new
                    {
                        title = "KPX Message",
                        //title = textBox1.Text,
                        body = message,
                    },*/

                    notification = new
                    {
                        body = message,
                        title = strTitle,
                        sound="default",
                    },

                    // FCM allows 1000 connections in parallel.
                    to = deviceId
                };

                //Linq to json
                string contentMsg = JsonConvert.SerializeObject(postData);
                System.Diagnostics.Trace.WriteLine("contentMsg = " + contentMsg);

                Byte[] byteArray = Encoding.UTF8.GetBytes(contentMsg);
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

            
                WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                resultStr = reader.ReadToEnd();
                System.Diagnostics.Trace.WriteLine("response: " + resultStr);
                reader.Close();
                responseStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                SetSystemLog("[ERROR] SendNotification(string, string, string) : " + ex.Message);
            }
            return resultStr;
        } 
        #endregion

        #region 문자 전송
        private void SendMessage(string szMessage, List<string> memberList)
        {
            try
            {
                string szCaller = "01057891562";
                ArrayList arrMessages = (new SOPServer.Data.MessageDivider()).MakeMessageList(szMessage);

                if (arrMessages == null)
                    return;

                using (libSMS.IMessageClient client = libSMS.MessageClientFactory.CreateMessageClient(500, "127.0.0.1"))
                {
                    foreach (string strMessage in arrMessages)
                    {
                        for (int i = 0; i < memberList.Count; i++)
                        {
                            client.SendSMS(szCaller, (string)memberList[i], strMessage);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                SetSystemLog("[ERROR] SendMessage(string, List<string>) : " + ex.Message);
            }
        }
        private void SendMessage(string szMessage, string member)
        {
            string szCaller = "01057891562";
            ArrayList arrMessages = (new SOPServer.Data.MessageDivider()).MakeMessageList(szMessage);

            if (arrMessages == null)
                return;

            try
            {
                using (libSMS.IMessageClient client = libSMS.MessageClientFactory.CreateMessageClient(500, "127.0.0.1"))
                {
                    foreach (string strMessage in arrMessages)
                    {
                        client.SendSMS(szCaller, member, strMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                SetSystemLog("[ERROR] SendMessage(string, string) : " + ex.Message);
            }
        } 
        #endregion

        #region 알람 감시
        private Dictionary<int, AlarmInfo> dicAlarmInfos = new Dictionary<int, AlarmInfo>();
        private List<AlarmInfo> alertAlarmList = new List<AlarmInfo>();

        private void DisplayPipeAlarm()
        {
            try
            {
                alertAlarmList.Clear();

                StringBuilder maxPipeAlarmId = new StringBuilder();
                GetPrivateProfileString("AlarmID", "PipeMaxAlarmId", 0, maxPipeAlarmId, 255, Application.StartupPath + @"\MaxAlarmId.ini");

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT ah.id as AlarmID, t.Name as tankName, p.Name as pipeName, at.id, description as status, RealValue, StandardValue, StandardRange ");
                sb.Append("  FROM alarmhistory as ah INNER JOIN alarmtype as at ON at.id=ah.AlarmType ");
                sb.Append("			              INNER JOIN tank as t ON ah.tankid=t.id ");
                sb.Append("                          LEFT OUTER JOIN Pipe as p ON ah.Pipeid=p.id ");
                if (maxPipeAlarmId.ToString().Length > 0)
                    sb.Append(" WHERE ah.id > " + maxPipeAlarmId.ToString() + " AND ah.EndTime IS NULL");
                sb.Append(" ORDER BY ah.id ");

                //sb.Append("SELECT p.id, Name, pah.id as AlarmId, pah.AlarmPressure, pah.Status, IFNULL((select Value from PipeStatus where id=pah.Status), '알수없음') as StrStatus  ");
                //sb.Append("  FROM Pipe as p INNER JOIN RecentAlarmHistory as rah ON p.id=rah.pipeid ");
                //sb.Append("                 INNER JOIN PipeAlarmHistory as pah ON rah.AlarmHistoryID=pah.ID ");
                //if (maxPipeAlarmId.ToString().Length > 0)
                //    sb.Append(" WHERE pah.ID > " + maxPipeAlarmId.ToString());
                //sb.Append(" ORDER BY pah.id ");

                ArrayList arrResult = dbMgr.GetResultData(sb.ToString(), 0);
                if (arrResult == null) return;

                int maxAlarmId = 0;

                for (int i = 0; i < arrResult.Count; i += 8)
                {
                    int nAlarmId = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strTankName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);
                    string strPipeName = DBUtility.WebDBManager.GetStringField(arrResult[i + 2]);
                    int nStatus = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                    string strStatus = DBUtility.WebDBManager.GetStringField(arrResult[i + 4]);
                    double nRealValue = (arrResult[i + 5].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + 5]);
                    double nStandardValue = (arrResult[i + 6].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + 6]);
                    double nStandardRange = (arrResult[i + 7].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + 7]);

                    AlarmInfo info = new AlarmInfo(nAlarmId, "TK-" + strTankName, strPipeName, nRealValue, nStandardValue, nStandardRange, nStatus, strStatus);
                    alertAlarmList.Add(info);

                    if (maxAlarmId < nAlarmId) maxAlarmId = nAlarmId;
                }

                if (maxAlarmId > 0)
                    WritePrivateProfileString("AlarmID", "PipeMaxAlarmId", maxAlarmId.ToString(), Application.StartupPath + @"\MaxAlarmId.ini");
            }
            catch (Exception ex)
            {
                SetSystemLog("[ERROR] DisplayPipeAlarm() : " + ex.Message);
            }
        }

        private List<AlarmInfo> sulfuricAlarmList = new List<AlarmInfo>();
        private void DisplaySulfuricAlarm()
        {
            try
            {
                sulfuricAlarmList.Clear();

                StringBuilder maxPipeAlarmId = new StringBuilder();
                GetPrivateProfileString("AlarmID", "SulfuricMaxAlarmId", 0, maxPipeAlarmId, 255, Application.StartupPath + @"\MaxAlarmId.ini");

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT tl.TankID, tlh.ID as HistoryID, (select Concat(Name, ' ', Type) from tank as t where t.id=tl.tankID) as tankName ");
                sb.Append("FROM TankLeak as tl INNER JOIN TankLeakHistory as tlh ON tl.HistoryID=tlh.ID ");
                if (maxPipeAlarmId.ToString().Length > 0)
                    sb.Append(" WHERE tlh.id > " + maxPipeAlarmId.ToString() + " AND tlh.EndTime IS NULL");
                sb.Append(" ORDER BY tlh.id ");

                ArrayList arrResult = dbMgr.GetResultData(sb.ToString(), 0);
                if (arrResult == null) return;

                int maxAlarmId = 0;

                for (int i = 0; i < arrResult.Count; i += 3)
                {
                    int nAlarmId = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    string strTankName = DBUtility.WebDBManager.GetStringField(arrResult[i + 2]);

                    AlarmInfo info = new AlarmInfo(nAlarmId, "TK-" + strTankName);
                    sulfuricAlarmList.Add(info);

                    if (maxAlarmId < nAlarmId) maxAlarmId = nAlarmId;
                }

                if (maxAlarmId > 0)
                    WritePrivateProfileString("AlarmID", "SulfuricMaxAlarmId", maxAlarmId.ToString(), Application.StartupPath + @"\MaxAlarmId.ini");
            }
            catch (Exception ex)
            {
                SetSystemLog("[ERROR] DisplaySulfuricAlarm() : " + ex.Message);
            }
        }  
        #endregion

        #region 인증요청
        private bool certRequest = false;
        private string certRequestAdminPhoneNumber = "";
        private void DisplayCertRequest()
        {
            try
            {
                certRequest = false;

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT count(*) FROM CertRequest WHERE CertCode IS NULL AND CertCodeLifeTime IS NULL AND MobileUserLevel IS NULL AND Defer=0 AND isconfirm=0 ");

                ArrayList arrResult = dbMgr.GetResultData(sb.ToString(), 0);
                if (arrResult == null) return;
                if (Convert.ToInt32(arrResult[0]) > 0)
                    certRequest = true;

                if (certRequest)
                {
                    arrResult = dbMgr.GetResultData("select phonenumber from user where id = (select userid from admin)", 0);
                    if (arrResult == null || arrResult.Count == 0)
                    {
                        certRequest = false;
                        return;
                    }

                    certRequestAdminPhoneNumber = DBUtility.AES256Cipher.AES_decrypt(arrResult[0].ToString(), key);
                    dbMgr.GetResultData("UPDATE CertRequest SET isconfirm=1", 0);
                }
            }
            catch (Exception ex)
            {
                SetSystemLog("[ERROR] DisplayCertRequest() : " + ex.Message);
            }
        } 
        #endregion

        #region 문자전송 여부
        private bool UseSms = false;
        private void DisplayUseSms()
        {
            try
            {
                UseSms = false;

                ArrayList arrResult = dbMgr.GetResultData("SELECT PropertyValue FROM Options WHERE PropertyName='UseSMS'", 0);
                if (arrResult == null) return;

                if (Convert.ToInt32(arrResult[0]) == 1)
                    UseSms = true;
            }
            catch (Exception ex)
            {
                SetSystemLog("[ERROR] DisplayUseSms() : " + ex.Message);
            }
        }  
        #endregion

        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            e.Cancel = true;
            this.notifyIcon1.Visible = true;
            this.Hide();
        }

        // 3개의 Data를 이용하여 하나의 Key를 만듦
        // 상중하 1,20,1,20,1,20로 shift
        // 한개가 가지는 최대 크기는 +-1048575
        // 최상위 1bit는 남겨둠
        private static long ToLong(int a1, int a2, int a3)
        {
            long b = ((uint)a3 & (uint.MaxValue >> 12));
            b = b << 42;
            long aa = (long)((uint)a2 & (uint.MaxValue >> 12)) << 21;
            b = b | aa;
            b = b | ((uint)a1 & (uint.MaxValue >> 12));
            return b;
        }

        // 3개의 Data(string)를 이용하여 하나의 Key를 만듦
        private string MakeKey(string s1, string s2, string s3)
        {
            return s1 + "_" + s2 + "_" + s3;
        }

        public void SetSystemLog(string content)
        {
            string filePath = @"D:\Tomcat 7.0\webapps\ROOT\SOP\KPX\SoundBtn.log";
            string dirPath = @"D:\Tomcat 7.0\webapps\ROOT\SOP\KPX";

            DirectoryInfo di = new DirectoryInfo(dirPath);
            FileInfo fi = new FileInfo(filePath);

            try
            {
                if (!di.Exists) Directory.CreateDirectory(dirPath);
                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        sw.WriteLine("[PUSH SERVER : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]    " + content);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filePath))
                    {
                        sw.WriteLine("[PUSH SERVER : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]    " + content);
                        sw.Close();
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }

    #region 배관, 탱크정보 클래스
    public class AlarmInfo
    {
        public int AlarmId { get; set; }
        public string TankName { get; set; }
        public string PipeName { get; set; } 
        public double RealValue { get; set; }
        public double StandardValue { get; set; }
        public double StandardRange { get; set; }
        public int Status { get; set; }
        public string strStatus { get; set; }

        public AlarmInfo(int alarmId, string tankName)
        {
            this.AlarmId = alarmId;
            this.TankName = tankName;
        }
        public AlarmInfo(int alarmId, string tankName, string pipeName, double realValue, double standardValue, double standardRange, int status, string strStatus)
        {
            this.AlarmId = alarmId;
            this.TankName = tankName;
            this.PipeName = pipeName; 
            this.RealValue = realValue;
            this.StandardValue = standardValue;
            this.StandardRange = standardRange;
            this.Status = status;
            this.strStatus = strStatus;
        }
    }

    public class PipeAlarmInfo
    {
        public int PipeID { get; set; }
        public string PipeName { get; set; }
        public int AlarmId { get; set; }
        public double Pressure { get; set; }
        public int Status { get; set; }
        public string StrStatus { get; set; }

        public PipeAlarmInfo(int pipeId, string pipeName, int alarmId, double pressure, int status, string strStatus)
        {
            this.PipeID = pipeId;
            this.PipeName = pipeName;
            this.AlarmId = alarmId;
            this.Pressure = pressure;
            this.Status = status;
            this.StrStatus = strStatus;
        }
    }
    public class TankAlarmInfo
    {
        public int TankID { get; set; }
        public string TankName { get; set; }
        public int AlarmId { get; set; }
        public int Status { get; set; }
        public string StrStatus { get; set; }

        public TankAlarmInfo(int tankId, string tankName, int alarmId, int status, string strStatus)
        {
            this.TankID = tankId;
            this.TankName = tankName;
            this.AlarmId = alarmId;
            this.Status = status;
            this.StrStatus = strStatus;
        }
    } 
    #endregion

    #region 모바일 기기 정보
    public class MobileDevice
    {
        private string m_strDeviceID = "";
        private string m_strPhoneNumber = "";
        private UserGroup m_userGroup = null;

        public string DeviceID
        {
            get { return m_strDeviceID; }
            set { m_strDeviceID = value; }
        }

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        public UserGroup UserGroup
        {
            get { return m_userGroup; }
            set { m_userGroup = value; }
        }
    }

    public class UserGroup
    {
        private int m_nID = -1;
        private string m_strGroupName = "";
        private bool m_usePipe = false;
        private bool m_usePipeAlarm = false;
        private bool m_useTank = false;
        private bool m_useTankAlarm = false;
        private bool m_useSulfuric = false; // 황산알람 표시 여부

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string GroupName
        {
            get { return m_strGroupName; }
            set { m_strGroupName = value; }
        }

        public bool UsePipe
        {
            get { return m_usePipe; }
            set { m_usePipe = value; }
        }

        public bool UsePipeAlarm
        {
            get { return m_usePipeAlarm; }
            set { m_usePipeAlarm = value; }
        }

        public bool UseTank
        {
            get { return m_useTank; }
            set { m_useTank = value; }
        }

        public bool UseTankAlarm
        {
            get { return m_useTankAlarm; }
            set { m_useTankAlarm = value; }
        }

        public bool UseSulfuric
        {
            get { return m_useSulfuric; }
            set { m_useSulfuric = value; }
        }
    }
    #endregion
}
