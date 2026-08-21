using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace HelpViewer
{
    using SearchService;

    public static class WebSearch
    {
        private static System.Text.Encoding m_PageEncoding = SetPageEncoding();

        private static ISearch m_proxy = null;
        private static ChannelFactory<ISearch> m_factory = null;

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

        // IndexSearch.jsp 사용
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

        // SearchService 사용
        public static string SearchURL2(string strURL, string strWebServerURL)
        {
            ChannelFactory<ISearch> factory;
            ISearch proxy = GetProxy(strWebServerURL, out factory);

            SearchResponse response = proxy.Search(new SearchRequest(strURL));

            if (response.SearchResult == false)
                return "<InvalidPath/>";

            string strFile = "", strFolder = "";

            foreach (string file in response.files)
            {
                if (strFile.Length == 0)
                    strFile = file;
                else
                    strFile += ";" + file;
            }

            foreach (string folder in response.folders)
            {
                if (strFolder.Length == 0)
                    strFolder = folder;
                else
                    strFolder += ";" + folder;
            }

            return "<File>" + strFile + "</File><Folder>" + strFolder + "</Folder>";
        }

        private static ISearch GetProxy(string strWebServerURL, out ChannelFactory<ISearch> factory)
        {
            if (m_proxy != null)
            {
                factory = m_factory;
                return m_proxy;
            }

            //ServiceEndpoint ep = MakeEndpoint(strWebServerURL, "SearchService", typeof(ISearch));
            Uri uri = new Uri(strWebServerURL + "/SearchService.svc");

            ServiceEndpoint ep = new ServiceEndpoint(
                ContractDescription.GetContract(typeof(ISearch)),
                new BasicHttpBinding(),
                new EndpointAddress(uri));

            factory = new ChannelFactory<ISearch>(ep);
            ISearch proxy = factory.CreateChannel();

            m_proxy = proxy;
            m_factory = factory;

            return proxy;
        }

        /*private static ServiceEndpoint MakeEndpoint(string strWebServerURL, string strServiceName, Type contractType)
        {
            System.Xml.XmlDictionaryReaderQuotas readerQuotas = new System.Xml.XmlDictionaryReaderQuotas();
            readerQuotas.MaxDepth = 128;
            readerQuotas.MaxStringContentLength = 2147483647;
            readerQuotas.MaxArrayLength = 2147483647;
            readerQuotas.MaxBytesPerRead = 31457280;
            readerQuotas.MaxNameTableCharCount = 16384;

            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MessageEncoding = WSMessageEncoding.Text;
            binding.MaxBufferPoolSize = 31457280;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.ReaderQuotas = readerQuotas;

            Uri uri = new Uri(strWebServerURL + "/" + strServiceName + ".svc");
            ServiceEndpoint ep = new ServiceEndpoint(
                ContractDescription.GetContract(contractType),
                binding,
                new EndpointAddress(uri));

            return ep;
        }*/
    }
}
