using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace HelpViewer
{
    public static class WebSearch
    {
        private static System.Text.Encoding m_PageEncoding = SetPageEncoding();

        public static System.Text.Encoding PageEncoding
        {
            get { return m_PageEncoding; }
            set { m_PageEncoding = value; }
        }

        private static char ConvertToHex(char cSource)
        {
            return "0123456789abcdef"[0x0f & cSource];
        }

        public static string URLEncoding(byte[] bytes)
        {
            string strResult = "";

            foreach (byte element in bytes)
            {
                if ((element >= '0' && element <= '9') ||   // 숫자
                    (element >= 'a' && element <= 'z') ||   // 소문자
                    (element >= 'A' && element <= 'Z') ||   // 대문자
                    (element == '!' || element == '*' || element == '(' || element == ')' || element == '_' || element == '-')) // 그 외의 특수기호들
                {
                    strResult += (char)element;
                }
                else
                {
                    strResult += "%";
                    strResult += ConvertToHex((char)((int)element >> 4));
                    strResult += ConvertToHex((char)element);
                }
            }
            return strResult;
        }

        private static System.Text.Encoding SetPageEncoding()
        {
            return System.Text.Encoding.GetEncoding(65001);
            //m_PageEncoding = System.Text.Encoding.GetEncoding(65001);
        }

        public static string SearchURL(string strURL, string strWebServerURL)
        {
            /*if (m_PageEncoding == null)
                SetPageEncoding();*/

            string resResult = string.Empty;
            string sourceUrl = strWebServerURL + "/SOP/IndexSearch.jsp";

            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes1 = enc.GetBytes(strURL);
            string strUrlEncode = URLEncoding(bytes1);

            string postData = "URL_PATH=" + strUrlEncode;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);

            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);

            //lock (this)
            {
                wReq.Method = "POST";
                //wReq.UserAgent = "Mozilla/4.0";
                wReq.ContentType = "application/x-www-form-urlencoded";
                wReq.ContentLength = bytes.Length;

                try
                {
                    using (Stream writeStream = wReq.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }

                    HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();

                    Stream respPostStream = wRes.GetResponseStream();

                    //System.Text.Encoding euckr = System.Text.Encoding.GetEncoding(51949);
                    StreamReader readerPost = new StreamReader(respPostStream, m_PageEncoding);

                    resResult = readerPost.ReadToEnd();

                    readerPost.Close();
                    respPostStream.Close();
                }
                catch (System.Net.WebException)
                {
                    //System.Windows.Forms.MessageBox.Show(e.Message);
                    return "";
                }
            }

            return resResult;
        }
    }
}
