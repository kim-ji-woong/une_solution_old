using DBUtility2;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USSServerReadEvent
{
    public partial class Form1 : Form
    {
        private string m_strURL = "";
        private DBUtility2.WebDBManager m_dbMgr = null;

        private Timer m_timer = null;

        private StreamWriter m_sw = new StreamWriter(@"C:\UNE\Log\USSServerReadEvent.txt", true);
        public Form1()
        {
            InitializeComponent();
                       
            m_strURL = System.Configuration.ConfigurationManager.AppSettings["earthWindURL"].ToString().Trim();

            if (m_strURL.Length == 0)
                return;

            m_strURL = m_strURL.Replace("abcdefghijk", "&");

            m_sw.WriteLine(m_strURL);

            m_dbMgr = new WebDBManager(201);

            m_timer = new Timer();
            m_timer.Interval = 1000;
            m_timer.Tick += M_timer_Tick;
            m_timer.Start();
        }

        private void M_timer_Tick(object sender, EventArgs e)
        {
            ReadEvent(m_strURL);
        }
        
        private void ReadEvent(string strURL)
        {
            string resResult = string.Empty;

            try
            {
                Uri uri = new Uri(strURL); // string 을 Uri 로 형변환
                HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(uri);

                wReq.Method = "GET";
                wReq.ServicePoint.Expect100Continue = false;

                using (HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse())
                {
                    Stream respPostStream = wRes.GetResponseStream();
                    StreamReader readerPost = new StreamReader(respPostStream, Encoding.GetEncoding("EUC-KR"), true);

                    resResult = readerPost.ReadToEnd();
                }

                //resResult = "<b>Warning</b>:  file_get_contents(): Filename cannot be empty in < b > D:\\bid.project\\bid_proc.php\\cwinfo.parcone\\module\\util\\e - gen.une.php </ b > on line < b > 140 </ b >{    \"bid_state\": {        \"state\": \"success\",        \"addmsg\": \"\",        \"proccode\": \"sendtoune\"    },    \"bid_data\": {        \"genmax\": {            \"mmi\": 1,            \"wspeed_a02\": 0,            \"wspeed_c02\": 2.04,            \"alarm_a02\": null,            \"alarm_c02\": null        }    }}";

                bool result;
                List<JObject> objList = ReadJson(resResult, out result);

                if (objList == null || objList.Count < 5)
                    return;

                JToken token;
                VariousData<int> intensity = null, wAlarmOfficeA = null, wAlarmOfficeB = null;
                VariousData<double> wSpeedOfficeA = null, wSpeedOfficeB = null;

                foreach (JObject obj in objList)
                {
                    if (obj.TryGetValue("mmi", out token))
                    {
                        GetJsonValue(token, out intensity);
                    }
                    else if (obj.TryGetValue("wspeed_a02", out token))
                    {
                        GetJsonValue(token, out wSpeedOfficeA);
                    }
                    else if (obj.TryGetValue("wspeed_c02", out token))
                    {
                        GetJsonValue(token, out wSpeedOfficeB);
                    }
                    else if (obj.TryGetValue("alarm_a02", out token))
                    {
                        GetJsonValue(token, out wAlarmOfficeA);
                    }
                    else if (obj.TryGetValue("alarm_c02", out token))
                    {
                        GetJsonValue(token, out wAlarmOfficeB);
                    }
                }

                if (intensity == null)
                    intensity = new VariousData<int>(0);
                if (wSpeedOfficeA == null)
                    wSpeedOfficeA = new VariousData<double>(0.0);
                if (wSpeedOfficeB == null)
                    wSpeedOfficeB = new VariousData<double>(0.0);
                if (wAlarmOfficeA == null)
                    wAlarmOfficeA = new VariousData<int>(0);
                if (wAlarmOfficeB == null)
                    wAlarmOfficeB = new VariousData<int>(0);

                m_sw.WriteLine(intensity.Data + ", " + wSpeedOfficeA.Data + ", " + wSpeedOfficeB.Data + ", " + wAlarmOfficeA.Data + ", " + wAlarmOfficeB.Data);
                m_sw.Flush();

                string strSQL = "Insert Into earthWind (EventTime, intensity, wSpeed_a02, wSpeed_c02, alarm_a02, alarm_c02) Values ('{0}',{1},{2},{3},{4},{5})";
                strSQL = string.Format(strSQL, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), intensity.Data, wSpeedOfficeA.Data, wSpeedOfficeB.Data, wAlarmOfficeA.Data, wAlarmOfficeB.Data);
                if (m_dbMgr.GetResultData(strSQL) == null)
                {
                    m_sw.WriteLine("query error : " + m_dbMgr.LastErrorMessage);
                    m_sw.Flush();
                }
                
                /*System.Diagnostics.Trace.WriteLine("진도 : " + intensity.Data);
                System.Diagnostics.Trace.WriteLine("풍속 OfficeA  : " + wSpeedOfficeA.Data);
                System.Diagnostics.Trace.WriteLine("풍속 OfficeB : " + wSpeedOfficeB.Data);
                System.Diagnostics.Trace.WriteLine("강풍 위험단계 OfficeA : " + wAlarmOfficeA.Data);
                System.Diagnostics.Trace.WriteLine("강풍 위험단계 OfficeB : " + wAlarmOfficeB.Data);*/
            }
            catch (System.Net.WebException ex)
            {
                m_sw.WriteLine("WebException : " + ex.Message);
                m_sw.Flush();
            }
        }

        private bool GetJsonValue(JToken token, out VariousData<int> value)
        {
            string str = token.ToString();
            int data;

            if (int.TryParse(str, out data))
            {
                value = new VariousData<int>(data);
                return true;
            }

            value = null;
            return false;
        }

        private bool GetJsonValue(JToken token, out VariousData<double> value)
        {
            string str = token.ToString();
            double data;

            if (double.TryParse(str, out data))
            {
                value = new VariousData<double>(data);
                return true;
            }

            value = null;
            return false;
        }

        private string GetJson(string str)
        {
            if (str.StartsWith("["))
            {
                str = str.Substring(1, str.Length - 2).Trim();
            }
            else if (str.StartsWith("{"))
                return str;
            else
            {
                str = "{" + str + "}";
            }

            return str;
        }

        private List<JObject> ReadJson(string strJson, out bool result)
        {
            List<JObject> objectList = new List<JObject>();
            result = false;

            int index = strJson.IndexOf("{");
            if (index > 0)
                strJson = strJson.Remove(0, index);

            JObject obj = JObject.Parse(strJson);
            JToken token;

            if (obj.TryGetValue("bid_data", out token))
            {
                if (token.Count() > 0)
                {
                    JToken token2 = token.ElementAt(0);

                    string strToken = token2.ToString();
                    string str = GetJson(strToken);

                    JObject data = JObject.Parse(str);

                    if (data.TryGetValue("genmax", out token))
                    {
                        try
                        {
                            int nChildCount = token.Count();

                            for (int i = 0; i < nChildCount; i++)
                            {
                                JToken _token = token.ElementAt(i);

                                strToken = _token.ToString();
                                str = GetJson(strToken);

                                data = JObject.Parse(str);
                                objectList.Add(data);
                            }
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Trace.WriteLine(e.Message);
                        }

                        return objectList;
                    }
                }
            }

            return null;
        }
    }
}
