using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CrisisAlertManager.Data
{
    class WebServiceManager
    {
        private const string BaseAddress = "http://apis.data.go.kr/1360000";
        private const string XML_HEADER = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>";
        private const string ServiceKey = "N7btoJzSjDUofiEvhmwj5EmDGxE4UP92YYXMfHqqQY%2BU%2B%2F5izsxJgOLfMSzbG%2BahGT6Gj286mPIgSNSb1pzu8w%3D%3D";

        private string SendQuery(string strXML, string strURL, bool noCodeCheck, out string strErrorMessage, string strMethodType = "GET")
        {
            strErrorMessage = "";
            string url = BaseAddress;

            if (strURL.StartsWith("/"))
                url += strURL;
            else
                url += "/" + strURL;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Method = strMethodType;

            if (strXML != null)
            {
                strXML = XML_HEADER + strXML;

                byte[] bytes = Encoding.UTF8.GetBytes(strXML);
                int len = bytes.Count();

                request.ContentType = "application/xml; charset=utf-8";
                request.ContentLength = len + 3;
            }

            string strResult = "";

            try
            {
                if (strXML != null)
                {
                    StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);
                    writer.Write(strXML);
                    writer.Close();
                }

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                strResult = readerPost.ReadToEnd().Trim();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                /*{
                    StreamWriter writer = new StreamWriter(string.Format("C:/temp/response_{0}.xml", m_nQueryCount++), false, Encoding.UTF8);
                    writer.Write(strResult);
                    writer.Close();
                }*/

                if (strResult.StartsWith("<") == false)
                {
                    strErrorMessage = strResult;
                    return "";
                }

                if (noCodeCheck)
                    return strResult;

            }
            catch (System.Net.WebException ex)
            {
                strErrorMessage = ex.Message;
            }

            return "";
        }

        public bool TestRest()
        {
            string strURL = string.Format("VilageFcstInfoService/getUltraSrtFcst?serviceKey=" + ServiceKey + "&numOfRows=10&pageNo=1&base_date=20200527&base_time=0630&nx=55&ny=127");
            string strErrorMessage;

            string strResult = SendQuery(null, strURL, true, out strErrorMessage);

            return true;
        }

    }
}
