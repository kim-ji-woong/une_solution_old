using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace SOPMonitoringSystem
{
    public class WebDBManager : DBUtility.WebDBManager
    {
        private FormMain m_Main = null;
        
        private string m_strSirenPath = "";
        private string m_strDoorBellPath = "";

        private int m_nLevel = -1;

        private static bool m_isLoadSMSAddText = false;
        private static string m_strSmsAddText = "";
        private static string m_strSmsCaller = "";

        public WebDBManager(FormMain main)
        {
            m_Main = main;
            m_strSirenPath = LoadIni("siren_file");
            m_strDoorBellPath = LoadIni("doorbell_file");
            m_strSmsCaller = LoadIni("sms_caller");
        }

        // ExternalCompanyMember 휴대폰 암호화
        private void EncryptExternalCompanyMember()
        {
            string strSQL = "select id, PhoneNumber from ExternalCompanyMember";
            
            ArrayList arrResult = GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });
            //System.IO.StreamWriter writer = new System.IO.StreamWriter("c:/UnE/ExternalCompanyMember.sql", false, Encoding.UTF8);

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = GetIntField(arrResult[i].ToString(), -1);
                string strPhoneNumber = GetStringField(arrResult[i + 1], "");

                if (nID < 0)
                    continue;

                if (string.Compare(strPhoneNumber, "null", true) == 0)
                    strPhoneNumber = "";

                bool isValid;
                strPhoneNumber = ValidPhoneNumber(strPhoneNumber, out isValid);

                if (!isValid)
                    continue;

                string strEncrypt = strPhoneNumber.Length == 0 ? "" : DBUtility.AES256Cipher.AES_encrypt(strPhoneNumber, key);
                //writer.WriteLine(string.Format("Update ExternalCompanyMember set PhoneNumber = '{0}' where id = {1};", strEncrypt, nID));
            }

            //writer.Close();
        }

        // strPhoneNumber에 빈칸이나 '-'등이 들어있을 경우 없앤다. 
        public static string ValidPhoneNumber(string strPhoneNumber, out bool isValid)
        {
            isValid = true;

            string strResult = "";
            int nLen = strPhoneNumber.Length;

            for (int i = 0; i < nLen; i++)
            {
                char ch = strPhoneNumber.ElementAt(i);

                if (ch != ' ' && ch != '\t' && ch != '-')
                {
                    if (ch >= '0' && ch <= '9')
                        strResult += ch;
                    else
                    {
                        isValid = false;
                        return "";
                    }
                }
            }

            return strResult;
        }

        public string SMS_ADD_TEXT
        {
            get
            {
                if (m_isLoadSMSAddText)
                    return m_strSmsAddText;
                else
                {
                    m_strSmsAddText = LoadIni("sms_add_text", "Server Connection Info");
                    m_isLoadSMSAddText = true;
                }

                return m_strSmsAddText;
            }
        }

        // User 권한
        public int Level
        {
            get { return m_nLevel; }
            set { m_nLevel = value; }
        }

        public int GetGenUserLevel(int nGenUserID)
        {
            string strSQL = "select UserLevel from SOPGenUser where ID = " + nGenUserID.ToString();
            ArrayList arrResult = GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            return GetIntField(arrResult[0].ToString(), -1);
        }

        public string SirenPath
        {
            get { return m_strSirenPath; }
        }

        public string DoorBellPath
        {
            get { return m_strDoorBellPath; }
        }

        public static string SMSCaller
        {
            get { return m_strSmsCaller; }
        }
    }
}
