using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CrisisAlertServer.Data
{
    public class WebServiceManager
    {
        private string BaseAddress = "";
        private const string XML_HEADER = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>";
        private string ServiceKey = "";

        public WebServiceManager()
        {
            BaseAddress = ConfigurationManager.AppSettings.Get("WebServiceBaseURL");
            if (BaseAddress == null || BaseAddress.Length == 0)
                BaseAddress = "http://apis.data.go.kr/1360000";

            ServiceKey = ConfigurationManager.AppSettings.Get("WebServiceKey");
            if (ServiceKey == null || ServiceKey.Length == 0)
                ServiceKey = "N7btoJzSjDUofiEvhmwj5EmDGxE4UP92YYXMfHqqQY%2BU%2B%2F5izsxJgOLfMSzbG%2BahGT6Gj286mPIgSNSb1pzu8w%3D%3D";
        }

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

        public bool ReadMidWeather(List<DataMidWeather> listMidWeather)
        {
            string strRegID = "11H10701";    // 예보구역코드 (대구시: 11H10701)

            string strURL = string.Format("VilageFcstMsgService/getLandFcst?serviceKey=" + ServiceKey + "&numOfRows=10&pageNo=1&regId=" + strRegID);
            string strErrorMessage;

            string strResult = SendQuery(null, strURL, true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("ReadMidWeather Error : " + strErrorMessage);
                return false;
            }

            XElement xml = XElement.Parse(strResult);

            foreach (XElement element in xml.Elements())
            {
                XElement xBody = element.Name == "body" ? element : null;

                if (xBody != null)
                {
                    foreach (XElement xBodyElement in xBody.Elements())
                    {
                        XElement xItems = xBodyElement.Name == "items" ? xBodyElement : null;

                        if (xItems != null)
                        {
                            foreach (XElement xItemsElement in xItems.Elements())
                            {
                                XElement xItem = xItemsElement.Name == "item" ? xItemsElement : null;

                                if (xItem != null)
                                {
                                    string strAnnounceTime = null, strNumEf = null, strTa = null;
                                    DataMidWeather midWeather = new DataMidWeather();

                                    foreach (XElement child in xItem.Elements())
                                    {
                                        if (child.Name == "announceTime")
                                        {
                                            strAnnounceTime = child.Value.Trim();
                                        }
                                        else if (child.Name == "numEf")
                                        {
                                            strNumEf = child.Value.Trim();
                                        }
                                        else if (child.Name == "ta")
                                        {
                                            strTa = child.Value.Trim();
                                        }
                                    }

                                    if (strAnnounceTime != null && strNumEf != null && strTa != null)
                                    {
                                        midWeather.AnnounceTime = strAnnounceTime;
                                        midWeather.NumEf = strNumEf;
                                        midWeather.Ta = strTa;

                                        listMidWeather.Add(midWeather);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public bool ReadLongWeather(string strDate, out DataLongWeather longWeather)
        {
            longWeather = new DataLongWeather();

            string strRegID = "11H10701";    // 예보구역코드 (대구시: 11H10701)

            string strURL = string.Format("MidFcstInfoService/getMidTa?serviceKey=" + ServiceKey + "&numOfRows=10&pageNo=1&regId=" + strRegID + "&tmFc=" + strDate);
            string strErrorMessage;

            string strResult = SendQuery(null, strURL, true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("ReadLongWeather Error : " + strErrorMessage);
                return false;
            }

            XElement xml = XElement.Parse(strResult);

            foreach (XElement element in xml.Elements())
            {
                XElement xBody = element.Name == "body" ? element : null;

                if (xBody != null)
                {
                    foreach (XElement xBodyElement in xBody.Elements())
                    {
                        XElement xItems = xBodyElement.Name == "items" ? xBodyElement : null;

                        if (xItems != null)
                        {
                            foreach (XElement xItemsElement in xItems.Elements())
                            {
                                XElement xItem = xItemsElement.Name == "item" ? xItemsElement : null;

                                if (xItem != null)
                                {
                                    string strAnnounceTime = null;
                                    string strtaMax3 = null, strtaMax4 = null, strtaMax5 = null, strtaMax6 = null, strtaMax7 = null, strtaMax8 = null, strtaMax9 = null, strtaMax10 = null;
                                    
                                    longWeather = new DataLongWeather();

                                    foreach (XElement child in xItem.Elements())
                                    {
                                        if (child.Name == "taMax3")
                                        {
                                            strtaMax3 = child.Value.Trim();
                                        }
                                        else if (child.Name == "taMax4")
                                        {
                                            strtaMax4 = child.Value.Trim();
                                        }
                                        else if (child.Name == "taMax5")
                                        {
                                            strtaMax5 = child.Value.Trim();
                                        }
                                        else if (child.Name == "taMax6")
                                        {
                                            strtaMax6 = child.Value.Trim();
                                        }
                                        else if (child.Name == "taMax7")
                                        {
                                            strtaMax7 = child.Value.Trim();
                                        }
                                        else if (child.Name == "taMax8")
                                        {
                                            strtaMax8 = child.Value.Trim();
                                        }
                                        else if (child.Name == "taMax9")
                                        {
                                            strtaMax9 = child.Value.Trim();
                                        }
                                        else if (child.Name == "taMax10")
                                        {
                                            strtaMax10 = child.Value.Trim();
                                        }

                                    }

                                    if (strtaMax3 != null && strtaMax4 != null && strtaMax5 != null && strtaMax6 != null && strtaMax7 != null && 
                                        strtaMax8 != null && strtaMax9 != null && strtaMax10 != null)
                                    {
                                        longWeather.AnnounceTime = strDate;
                                        longWeather.TaMax3 = strtaMax3;
                                        longWeather.TaMax4 = strtaMax4;
                                        longWeather.TaMax5 = strtaMax5;
                                        longWeather.TaMax6 = strtaMax6;
                                        longWeather.TaMax7 = strtaMax7;
                                        longWeather.TaMax8 = strtaMax8;
                                        longWeather.TaMax9 = strtaMax9;
                                        longWeather.TaMax10 = strtaMax10;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

    }
}
