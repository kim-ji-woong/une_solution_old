using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IAQM_API_Test
{
    class Program
    {        
        // 세션 유지용 쿠키
		private static  CookieContainer m_CookieContainer = new CookieContainer();

        // Page Encoding
        private static Encoding m_PageEncoding = Encoding.UTF8;

        // REQUEST Parameter용
        private static SortedList<string, string> mParams = new SortedList<string, string>();

        private static string webServiceEntry = "http://unes.iptime.org:8112";

        static void Main(string[] args)
        {
            //웹서비스의 주소는 http://unes.iptime.org:8112/webservice.html 페이지를 읽어보면 나와있다.
            //해당 페이지의  <P>태그를 파싱하면 웹서비스 접속주소를 알 수 있다.
            

            TestInsertNode();

            TestInserSensorCode();

            TestInsertSensorValue();

            TestInsertSensorValue();
        }

        // 센서 코드 등록
        public static void TestInserSensorCode()
        {
            mParams.Clear();
            
            // get group ID

            string szAPIPath1 = "AQM/Config/sensorGroups";
            // 응답 예 : {"Config":{"group":-1}, "Result":-1}
            // 그룹중 같은 범주에 넣을 ID를 선택하여 GroupID로 지정한다.
            // 전체 그룹 가져오기
            JObject obj1 = JObject.Parse(GET(webServiceEntry, szAPIPath1, mParams));

            int nGroupID = 8192;

            addParam("SensorName", "온도");
            addParam("GroupID", ""+nGroupID);
            addParam("SensorCode", "8192");
            addParam("LimitNotice","0");
            addParam("LimitAttention","0");
            addParam("LimitWarning","0");
            addParam("LimitValueLaw","0");
            addParam("SensorUnit","1");
            addParam("LimitType","1");
            addParam("LimitNoticeBegin","0");
            addParam("LimitNoticeEnd","0");
            addParam("LimitAttentionBegin","0");
            addParam("LimitAttentionEnd","0");
            addParam("LimitWarningBegin","0");
            addParam("LimitWarningEnd","0");
            addParam("LimitValueLawBegin","0");
            addParam("LimitValueLawEnd","0");
            addParam("Remark", "테스트");


            string szAPIPath = "AQM/Sensor/value/new";

            // 응답 예 : {"Node":{"id": -1 }, "Result":-1}
            string szResult = POST(webServiceEntry, szAPIPath, mParams);
            if (szResult != null && szResult != "")
            {
                JObject obj2 = JObject.Parse(szResult);

                // Result는 바로 읽어진다.
                int nResult = (int)obj2["Result"];
                if (nResult >= 0)
                {
                    // OK
                }
                else
                {
                    // API 실행 에러             
                }
            }
            else
            {
                // API 호출 에러
            }       
        }

        // 실시간 센서값 등록
        public static void TestInsertSensorValue()
        {
            mParams.Clear();

            addParam("Node", "999");
            addParam("SensorCode", "8192");
            addParam("Value", "0");
            addParam("ExtraValue", "0");

            string szAPIPath = "AQM/Sensor/value/new";

            // 응답 예 : {"Node":{"id": -1 }, "Result":-1}
            string szResult = POST(webServiceEntry, szAPIPath, mParams);
            if (szResult != null && szResult != "")
            {
                JObject obj2 = JObject.Parse(szResult);
                
                // Result는 바로 읽어진다.
                int nResult = (int)obj2["Result"];
                if (nResult >= 0)
                {
                    // OK
                }
                else
                {
                    // API 실행 에러             
                }
            }
            else
            {
                // API 호출 에러
            }           
        }
          

        // Node ID 등록
        public static void TestInsertNode()
        {           
            // 1. 위치 ID가져오기
            string szAPIPath = "AQM/Area/id/서울특별시/용산구/청파동";

            // 응답 예 : {"Area":{"id": -1}, "Result":-1}
            JObject obj1 = JObject.Parse(GET(webServiceEntry, szAPIPath, mParams));
            int nAreaID = (int)obj1["Area"]["id"];
            int nResult = (int)obj1["Result"];

            if (nResult >= 0)
            {
                mParams.Clear();
                string szMaterialCodes = "8192,8448,12800,14336,20736,20992,21760";
                addParam("NodeID", "999");
                addParam("NodeName", "TestNode");
                addParam("NodePosX", "0");
                addParam("NodePosY", "0");
                addParam("Area", nAreaID.ToString());
                addParam("Materials", szMaterialCodes);

                szAPIPath = "AQM/Node/new";

                // 응답 예 : {"Node":{"id": -1 }, "Result":-1}
                string szResult = POST(webServiceEntry, szAPIPath, mParams);
                if( szResult != null && szResult != "")
                {
                    JObject obj2 = JObject.Parse(szResult);
                    // Result는 바로 읽어진다.
                    nResult = (int)obj2["Result"];
                    // Node의 ID는 아래와 같이 2단계
                    int nID = (int)obj2["Node"]["id"];

                    if (nResult >= 0)
                    {
                        // OK
                    }
                    else
                    {
                        // API 실행 에러             
                    }
                }
                else
                {
                    // API 호출 에러
                }                
            }
        }

        private static void addParam(string szParamName, string szParamValue)
        {
            mParams.Add(szParamName, szParamValue);
        }

        private static string GET(string strURL, string szAPI, SortedList<string, string> Params )
        {
            string resResult = string.Empty;
                        
            // Form 데이터 구성 
            UTF8Encoding enc = new UTF8Encoding();
            string formData = "";
            bool bFirst = true;
            foreach(string szKey in Params.Keys)
            {
                if(bFirst == false)
                    formData += "&";

                formData += szKey + "=";

                string szValue = Params[szKey];
                byte[] bytes1 = enc.GetBytes(szValue);
                string szUrlEncode = URLEncoding(bytes1);

                formData += szUrlEncode;
            }
            
            string sourceUrl = strURL + "/" + szAPI;
            if( formData != "")
            {
                sourceUrl += "?" + formData;
            }

            // Form 데이터 Submit
           
            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);               
            wReq.Method = "GET";
            wReq.ContentType = "application/x-www-form-urlencoded";
            wReq.ContentLength = 0;
			wReq.CookieContainer = m_CookieContainer;
            try
            {
                HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();
                   
                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, m_PageEncoding); 
                    
                resResult = readerPost.ReadToEnd();
                    
                readerPost.Close();
                respPostStream.Close();
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
                return "";
            }
            System.Diagnostics.Trace.WriteLine(resResult);
            return resResult;
        }

        private static string POST(string strURL, string szAPI, SortedList<string, string> Params)
        {
            string resResult = string.Empty;
            string sourceUrl = strURL + "/" + szAPI;

            UTF8Encoding enc = new UTF8Encoding();

            string formData = "";
            bool bFirst = true;
            foreach(string szKey in Params.Keys)
            {
                if(bFirst == false)
                    formData += "&";

                bFirst = false;

                formData += szKey + "=";

                string szValue = Params[szKey];
                byte[] bytes1 = enc.GetBytes(szValue);
                string szUrlEncode = URLEncoding(bytes1);

                formData += szUrlEncode;
            }

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(formData);

            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
                
            wReq.Method = "POST";
            wReq.ContentType = "application/x-www-form-urlencoded";
            wReq.ContentLength = bytes.Length;
            wReq.CookieContainer = m_CookieContainer;

            try
            {
                using (Stream writeStream = wReq.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
                HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, m_PageEncoding); 

                resResult = readerPost.ReadToEnd();

                readerPost.Close();
                respPostStream.Close();
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
                return "";
            }
            System.Diagnostics.Trace.WriteLine(resResult);
            return resResult;
        }

        private static char ConvertToHex(char cSource)
        {
            return "0123456789abcdef"[0x0f & cSource];
        }

        private static string URLEncoding(byte[] bytes)
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
    }
}
