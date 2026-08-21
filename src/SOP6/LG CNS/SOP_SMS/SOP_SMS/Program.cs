using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Collections;

namespace SOP_SMS
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Count() < 2)
                return;

            string strFilePath = Process.GetCurrentProcess().ProcessName + ".aid";

            int nActionStepHistoryID = GetActionStepHistoryID(strFilePath);
            if (nActionStepHistoryID > 0)
            {
                int nSensorType;

                if (int.TryParse(args[0].Trim(), out nSensorType))
                {
                    string strMessage = args[1];

                    ArrayList arrDatas = new ArrayList();
                    arrDatas.Add(nActionStepHistoryID);
                    arrDatas.Add(nSensorType);
                    arrDatas.Add(strMessage);

                    if (args.Count() >= 3)
                    {
                        // MMS로 전송할 이미지
                        arrDatas.Add(args[2].Trim());
                    }

                    if (args.Count() >= 4)
                    {
                        int withURL;

                        if (int.TryParse(args[3].Trim(), out withURL))
                        {
                            arrDatas.Add(withURL);
                        }
                    }

                    Thread t = new Thread(new ParameterizedThreadStart(Run));
                    t.Start(arrDatas);
                    /*Dictionary<string, int> dicMembers = MemberManager.GetMemberList(nSensorType);

                    if (dicMembers != null)
                    {
                        SMSManager.SendSMS(dicMembers, nActionStepHistoryID, strMessage);
                    }*/
                }
            }
        }

        private static string ChangeTime(string strMessage, string strTime)
        {
            while (true)
            {
                int nIndex = strMessage.IndexOf("{time");

                if (nIndex < 0)
                    break;

                int nIndex2 = strMessage.IndexOf('}', nIndex);

                if (nIndex2 < 0)
                    break;

                strMessage = strMessage.Substring(0, nIndex) + strTime + strMessage.Substring(nIndex2 + 1);
            }

            return strMessage;
        }

        private static void Run(object arg)
        {
            ArrayList arrDatas = (ArrayList)arg;

            int nActionStepHistoryID = (int)arrDatas[0];
            int nSensorType = (int)arrDatas[1];
            string strMessage = (string)arrDatas[2];

            string strPosition, strTime, strSOPMode;

            if (MemberManager.ReadActionStepHistoryInfo(nActionStepHistoryID, out strPosition, out strTime, out strSOPMode) == false)
                return;

            strMessage = strMessage.Replace("{SOPMode}", strSOPMode);
            strMessage = strMessage.Replace("{location}", strPosition);
            strMessage = ChangeTime(strMessage, strTime);

            Dictionary<string, int> dicMembers = MemberManager.GetMemberList(nSensorType);
            if (dicMembers != null)
            {
                string strImage = null;
                bool withURL = true;

                if (arrDatas.Count > 3)
                {
                    strImage = (string)arrDatas[3];

                    if (strImage.Length == 0 || string.Compare(strImage, "none", true) == 0)
                        strImage = null;

                    if (arrDatas.Count > 4)
                    {
                        if (arrDatas[4] is int)
                        {
                            int nWithURL = (int)arrDatas[4];
                            withURL = nWithURL == 1;
                        }
                    }
                }

                if (strImage == null)
                    SMSManager.SendSMS(dicMembers, nActionStepHistoryID, strMessage, withURL);
                else
                {
                    SMSManager.SendMMS(dicMembers, nActionStepHistoryID, strMessage, strImage, withURL);
                    return;
                }
            }

            if (NetworkWebManager.Instance != null)
            {
                while (NetworkWebManager.Instance.IsComplete == false)
                {
                    Thread.Sleep(1000);
                    System.Diagnostics.Trace.WriteLine("Delay");
                }
            }
            else
                System.Diagnostics.Trace.WriteLine("NetworkWebManager is null");
        }

        private static int GetActionStepHistoryID(string strFilePath)
        {
            byte[] bytes;
            int nActionStepHistoryID;

            using (var fs = File.Open(strFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);

                string str = System.Text.Encoding.UTF8.GetString(bytes);

                if (int.TryParse(str.Trim(), out nActionStepHistoryID))
                    return nActionStepHistoryID;
            }
            /*if (File.Exists(strFilePath))
            {
                StreamReader reader = new StreamReader(strFilePath);
                string strLine = reader.ReadLine();
                reader.Close();

                File.Delete(strFilePath);

                int nActionStepHistoryID;

                if (int.TryParse(strLine.Trim(), out nActionStepHistoryID))
                    return nActionStepHistoryID;
            }*/

            return -1;
        }
    }
}
