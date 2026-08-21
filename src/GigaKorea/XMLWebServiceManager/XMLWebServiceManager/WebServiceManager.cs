using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using UnE.Geometry;
using XMLWebServiceManager.BIM;
using XMLWebServiceManager.Shapes;

namespace XMLWebServiceManager
{
    public class WebServiceManager
    {
        public enum BoundartType { Line = 0, Arc, EArc };

        private string m_strID = "";
        private string m_strPW = "";
        private string m_strKey = "";
        private string m_strBuildingKey = "";

        private const string XML_HEADER = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>";
        private string BaseAddress = "https://sdesign.etri.re.kr:8443";
        private const string API = "/sdesignApi";

        private const string SUCCESS_CODE = "RS101";
        private string m_strDoubleFormat = "F1";

        // 속성 테이블
        private Dictionary<string, string> m_strPropTable = new Dictionary<string, string>();

        public WebServiceManager()
        {
            // SSL/TLS 상위 버전 호환 설정
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
        }

        public Dictionary<string, string> GetSidoList(ref string strResultMessage)
        {
            Dictionary<string, string> dicSidoList = null;
            int i;
            string strKey = "";
            string strValue = "";

            strResultMessage = "";
            string resResult = string.Empty;

            string strURL = BaseAddress + API + "/code/sido";

            string strXML = XML_HEADER;
            strXML += "<code>";
            strXML += "<state>";
            strXML += "<user_id>" + m_strID + "</user_id>";
            strXML += "<key_id>" + m_strKey + "</key_id>";
            strXML += "</state>";
            strXML += "</code>";

            byte[] bytes = Encoding.UTF8.GetBytes(strXML);
            int len = bytes.Count();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL));
            request.Method = "POST";
            request.ContentType = "application/xml; charset=utf-8";
            request.ContentLength = len + 3;

            try
            {
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);
                writer.Write(strXML);
                writer.Close();

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("LoadDetailProp Fail : " + ex.Message);
                strResultMessage = "LoadDetailProp Fail : " + ex.Message;
                return null;
            }

            if (resResult.Length == 0 || resResult.IndexOf("RS101") < 0)
            {
                strResultMessage = "GetSido Rest API Response ERROR";
                return null;
            }

            dicSidoList = GetSidoSggEmdNameCodeList(resResult);

            return dicSidoList;
        }

        public Dictionary<string, string> GetSigunguList(string strSidoKey, ref string strResultMessage)
        {
            if (strSidoKey == "" || strSidoKey == null)
            {
                strResultMessage = "시도 Key 값이 없습니다.";
                return null;
            }

            strResultMessage = "";

            int i;
            string strKey = "";
            string strValue = "";
            Dictionary<string, string> dicSigunguList = null;

            string resResult = string.Empty;
            string strURL = BaseAddress + API + "/code/sigungu";

            string strXML = XML_HEADER;
            strXML += "<code>";
            strXML += "<paramSidoCode>" + strSidoKey + "</paramSidoCode>";
            strXML += "<state>";
            strXML += "<user_id>" + m_strID + "</user_id>";
            strXML += "<key_id>" + m_strKey + "</key_id>";
            strXML += "</state>";
            strXML += "</code>";

            byte[] bytes = Encoding.UTF8.GetBytes(strXML);
            int len = bytes.Count();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL));
            request.Method = "POST";
            request.ContentType = "application/xml; charset=utf-8";
            request.ContentLength = len + 3;

            try
            {
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);
                writer.Write(strXML);
                writer.Close();

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("LoadDetailProp Fail : " + ex.Message);
                strResultMessage = "LoadDetailProp Fail : " + ex.Message;
                return null;
            }

            if (resResult.Length == 0 || resResult.IndexOf("RS101") < 0)
            {
                strResultMessage = "GetSigungu Rest API Response ERROR";
                return null;
            }

            dicSigunguList = GetSidoSggEmdNameCodeList(resResult);

            return dicSigunguList;
        }

        public Dictionary<string, string> GetDongList(string strSigunguKey, ref string strResultMessage)
        {
            if (strSigunguKey == "" || strSigunguKey == null)
            {
                strResultMessage = "시군구 Key 값이 없습니다.";
                return null;
            }

            strResultMessage = "";

            int i;
            string strKey = "";
            string strValue = "";
            Dictionary<string, string> dicDongList = null;

            string resResult = string.Empty;

            string strURL = BaseAddress + API + "/code/dong";

            string strXML = XML_HEADER;
            strXML += "<code>";
            strXML += "<paramSigunguCode>" + strSigunguKey + "</paramSigunguCode>";
            strXML += "<state>";
            strXML += "<user_id>" + m_strID + "</user_id>";
            strXML += "<key_id>" + m_strKey + "</key_id>";
            strXML += "</state>";
            strXML += "</code>";

            byte[] bytes = Encoding.UTF8.GetBytes(strXML);
            int len = bytes.Count();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL));
            request.Method = "POST";
            request.ContentType = "application/xml; charset=utf-8";
            request.ContentLength = len + 3;

            try
            {
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);
                writer.Write(strXML);
                writer.Close();

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("LoadDetailProp Fail : " + ex.Message);
                strResultMessage = "LoadDetailProp Fail : " + ex.Message;
                return null;
            }

            if (resResult.Length == 0 || resResult.IndexOf("RS101") < 0)
            {
                strResultMessage = "GetDong Rest API Response ERROR";
                return null;
            }

            dicDongList = GetSidoSggEmdNameCodeList(resResult);

            return dicDongList;
        }

        private Dictionary<string, string> GetSidoSggEmdNameCodeList(string strResult)
        {
            int i;
            string strKey = "";
            string strValue = "";
            Dictionary<string, string> dicList = new Dictionary<string, string>();

            i = strResult.IndexOf("<listAddrCode>");
            while (i > 0)
            {
                strResult = strResult.Substring(i);
                strValue = GetStringValue(strResult, "<code>", "</code>");
                strKey = GetStringValue(strResult, "<codeName>", "</codeName>");
                dicList.Add(strKey, strValue);

                i = strResult.IndexOf("</listAddrCode>");
                strResult = strResult.Substring(i);
                i = strResult.IndexOf("<listAddrCode>");
            }

            return dicList;
        }

        private string GetStringValue(string strFrom, string strKeyi, string strKeyj)
        {
            int i, j, cnt;
            cnt = strKeyi.Length;
            i = strFrom.IndexOf(strKeyi);

            if (i < 0) return "";

            j = strFrom.IndexOf(strKeyj);

            return strFrom.Substring(i + cnt, j - i - cnt);
        }

        public List<BulidingInfo> GetBulidingInfoList(string strSigunguKey, string strDongName, string strLoadName, string strBulidingNum, ref string strResultMessage)
        {
            strResultMessage = "";
            List<BulidingInfo> listBulidingInfo = null;

            string strValuedRoadName = "";
            //도로명에 빈칸 빼기
            GetBlankRemovedName(strLoadName, out strValuedRoadName);

            //도로명에 - 빼기
            GetDashRemovedName(strValuedRoadName, out strValuedRoadName);

            // 도로 키 불러오기
            List<string> listRoad = GetRoadList(strSigunguKey, strDongName, strValuedRoadName, ref strResultMessage);

            if (listRoad == null)
                return null;

            // 빌딩리스트 불러오기
            listBulidingInfo = GetBuildingList(strSigunguKey, listRoad, strBulidingNum, ref strResultMessage);

            return listBulidingInfo;
        }

        private List<BulidingInfo> GetBuildingList(string strSigunguKey, List<string> listRoad, string strBulidingNum, ref string strResultMessage)
        {
            List<BulidingInfo> listBulidingInfo = new List<BulidingInfo>();

            //건물 번호에서 본번, 부번 추출.
            string strMainNumber = "";
            string strSubNumber = "";
            GetBuildingNumber(strBulidingNum, out strMainNumber, out strSubNumber);

            string resResult = string.Empty;
            string strURL = BaseAddress + API + "/convergence/buildingList";

            foreach (string strRoadKey in listRoad)
            {
                string strXML = XML_HEADER;
                strXML += "<convergence>";
                strXML += "<state>";
                strXML += "<user_id>" + m_strID + "</user_id>";
                strXML += "<key_id>" + m_strKey + "</key_id>";
                strXML += "</state>";
                strXML += "<paramSggCode>" + strSigunguKey + "</paramSggCode>";
                strXML += "<paramRoadCode>" + strRoadKey + "</paramRoadCode>";
                strXML += "<paramRoadMain>" + strMainNumber + "</paramRoadMain>";
                strXML += "<paramRoadSub>" + strSubNumber + "</paramRoadSub>";
                strXML += "<pageNo>" + "0" + "</pageNo>";
                strXML += "<pageSize>" + "1000" + "</pageSize>";
                strXML += "</convergence>";

                byte[] bytes = Encoding.UTF8.GetBytes(strXML);
                int len = bytes.Count();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL));
                request.Method = "POST";
                request.ContentType = "application/xml; charset=utf-8";
                request.ContentLength = len + 3;

                try
                {
                    StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);
                    writer.Write(strXML);
                    writer.Close();

                    HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                    Stream respPostStream = wRes.GetResponseStream();
                    StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                    resResult = readerPost.ReadToEnd();
                    request.Abort();
                    readerPost.Close();
                    respPostStream.Close();
                }
                catch (System.Net.WebException ex)
                {
                    System.Diagnostics.Trace.WriteLine("LoadDetailProp Fail : " + ex.Message);
                    strResultMessage = "LoadDetailProp Fail : " + ex.Message;
                    return null;
                }

                if (resResult.Length == 0 || resResult.IndexOf("RS101") < 0)
                {
                    strResultMessage = "GetBuildingList Rest API Response ERROR";
                    return null;
                }

                int i;
                i = resResult.IndexOf("<listNaviBuild>");//첫건물 탐색

                while (i > 0)
                {
                    resResult = resResult.Substring(i);
                    BulidingInfo bInfo = new BulidingInfo();

                    //주소
                    bInfo.Address += GetStringValue(resResult, "<sido_name>", "</sido_name>") + " ";
                    bInfo.Address += GetStringValue(resResult, "<sigungu_name>", "</sigungu_name>") + " ";
                    bInfo.Address += GetStringValue(resResult, "<road_name>", "</road_name>") + " ";
                    bInfo.Address += GetStringValue(resResult, "<build_main>", "</build_main>");// main build num
                    string sBubun = GetStringValue(resResult, "<build_sub>", "</build_sub>");// sub build num
                    if (sBubun != "0")
                        bInfo.Address += "-" + sBubun + " ";
                    else
                        bInfo.Address += " ";

                    bInfo.Address += GetStringValue(resResult, "<build_mng_name>", "</build_mng_name>");

                    //고유키
                    bInfo.BuildingKey += GetStringValue(resResult, "<build_mng_no>", "</build_mng_no>");
                    //용도
                    bInfo.BuildingMenu += GetStringValue(resResult, "<build_mng_menu>", "</build_mng_menu>");
                    //층수
                    bInfo.FloorNo += GetStringValue(resResult, "<ground_layer>", "</ground_layer>");
                    //이름
                    bInfo.BuildingName += GetStringValue(resResult, "<build_mng_name>", "</build_mng_name>");

                    // 마지막 업데이트 정보
                    //string strUpdateInfo = GetLevelInfo(bInfo.BuildingKey);

                    //if (strUpdateInfo != null)
                    //    bInfo.UpdateInfo = strUpdateInfo;
                    bInfo.UpdateInfo = "yy-mm-dd";

                    listBulidingInfo.Add(bInfo); // 건물정보 추가

                    i = resResult.IndexOf("</listNaviBuild>");
                    resResult = resResult.Substring(i);
                    i = resResult.IndexOf("<listNaviBuild>");//다음건물 탐색
                }
            }

            return listBulidingInfo;
        }

        public string GetLevelInfo(string strBuildingKey, ref string strResultMessage)
        {
            string resResult = string.Empty;
            strResultMessage = "";

            string strURL = BaseAddress + API + "/spatial/levelList";

            string strXML = XML_HEADER;
            strXML += "<spatial>";
            strXML += "<state>";
            strXML += "<user_id>" + m_strID + "</user_id>";
            strXML += "<key_id>" + m_strKey + "</key_id>";
            strXML += "</state>";
            strXML += "<level>";
            strXML += "<build_mng_no>" + strBuildingKey + "</build_mng_no>";
            strXML += "</level>";
            strXML += "</spatial>";

            byte[] bytes = Encoding.UTF8.GetBytes(strXML);
            int len = bytes.Count();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL));
            request.Method = "POST";
            request.ContentType = "application/xml; charset=utf-8";
            request.ContentLength = len + 3;

            try
            {
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);
                writer.Write(strXML);
                writer.Close();

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("LoadDetailProp Fail : " + ex.Message);
                strResultMessage = "LoadDetailProp Fail : " + ex.Message;
                return null;
            }

            if (resResult.Length == 0 || resResult.IndexOf("RS101") < 0)
            {
                strResultMessage = "Get levelList Rest API Response ERROR";
                return null;
            }

            string strDateAndUser = "";
            if (resResult.IndexOf("<create_date>") >= 0)
            {
                strDateAndUser += DateTime.Parse(GetStringValue(resResult, "<create_date>", "</create_date>")) + " / ";
                strDateAndUser += GetStringValue(resResult, "<create_user_id>", "</create_user_id>");
            }
            else
                strDateAndUser += "None";

            return strDateAndUser;
        }

        private void GetBuildingNumber(string strBulidingNum, out string sMainNumber, out string sSubNumber)
        {
            string sNumber = "";
            sMainNumber = "";
            sSubNumber = "";

            //공백삭제후.
            if (!GetBlankRemovedName(strBulidingNum, out sNumber))
                return;

            //"-"로. 본번, 부번 구분
            int i = sNumber.IndexOf("-");
            if (i >= 0)
            {
                sMainNumber = sNumber.Substring(0, i);
                sSubNumber = sNumber.Substring(i + 1, sNumber.Length - i - 1);
            }
            else//본번만 입력시 뒤에 부번0
            {
                sMainNumber = sNumber;
                sSubNumber = "0";
            }
        }

        private bool GetBlankRemovedName(string sName, out string sValuedName)
        {
            sValuedName = sName;
            if (sName.Length < 1) return false;

            int i = sName.IndexOf(" ");
            while (i > 0)
            {
                sValuedName = sValuedName.Remove(i, 1);
                i = sValuedName.IndexOf(" ");
            }

            if (sValuedName.Length < 1) return false;

            return true;
        }

        private bool GetDashRemovedName(string sName, out string sValuedName)
        {
            sValuedName = sName;
            if (sName.Length < 1) return false;

            int i = sValuedName.IndexOf("-");
            while (i > 0)
            {
                sValuedName = sValuedName.Remove(i, 1);
                i = sValuedName.IndexOf("-");
            }

            if (sValuedName.Length < 1) return false;

            return true;
        }

        private List<string> GetRoadList(string strSigunguKey, string strDongName, string strLoadName, ref string strResultMessage)
        {
            string resResult = string.Empty;
            string strURL = BaseAddress + API + "/code/addrRoad";

            string strXML = XML_HEADER;
            strXML += "<code>";
            strXML += "<state>";
            strXML += "<user_id>" + m_strID + "</user_id>";
            strXML += "<key_id>" + m_strKey + "</key_id>";
            strXML += "</state>";
            strXML += "<paramSggCode>" + strSigunguKey + "</paramSggCode>";
            strXML += "<paramRoadName>" + strLoadName + "</paramRoadName>";
            strXML += "<paramDongName>" + strDongName + "</paramDongName>";
            strXML += "</code>";


            byte[] bytes = Encoding.UTF8.GetBytes(strXML);
            int len = bytes.Count();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL));
            request.Method = "POST";
            request.ContentType = "application/xml; charset=utf-8";
            request.ContentLength = len + 3;

            try
            {
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);
                writer.Write(strXML);
                writer.Close();

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("LoadDetailProp Fail : " + ex.Message);
                strResultMessage = "LoadDetailProp Fail : " + ex.Message;
                return null;
            }

            if (resResult.Length == 0 || resResult.IndexOf("RS101") < 0)
            {
                strResultMessage = "GetRoad Rest API Response ERROR";
                return null;
            }

            int i;
            string strRoadCode;
            List<string> listRoad = new List<string>();

            i = resResult.IndexOf("<listNaviBuildCode>");
            while (i > 0)
            {
                resResult = resResult.Substring(i);
                strRoadCode = GetStringValue(resResult, "<road_code>", "</road_code>");

                if (!listRoad.Contains(strRoadCode))//중복체크
                    listRoad.Add(strRoadCode);

                i = resResult.IndexOf("</listNaviBuildCode>");
                resResult = resResult.Substring(i);
                i = resResult.IndexOf("<listNaviBuildCode>");
            }

            return listRoad;
        }

        public bool SaveXMLFile(string strXMLFile, string strBuildingKey, ref string strResultMessage)
        {
            bool bResult = false;
            string strResult = null;

            strResult = DownloadProject(strBuildingKey, ref strResultMessage);

            if (strResult == null)
                return bResult;

            XElement xTemp = XElement.Parse(strResult);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(strResult);

            foreach (XmlNode rootNode in xmlDoc.ChildNodes)
            {
                if (rootNode.Name == "IndoorModelFile")
                {
                    XmlAttribute version = xmlDoc.CreateAttribute("version");
                    version.Value = XMLManager.TARGET_VERSION;
                    // TODO: 다운로드 시에 속성 값 넣는 부분 수정이 필요 >> 노아 쪽에서 값을 넣을 수 있도록
                    rootNode.Attributes.Append(version);

                    foreach (XmlNode IndoorNode in rootNode.ChildNodes)
                    {
                        if (IndoorNode.Name == "ProjectInfo")
                        {
                            XmlAttribute unit = xmlDoc.CreateAttribute("unit");
                            unit.Value = "mm";
                            // TODO: 다운로드 시에 속성 값 넣는 부분 수정이 필요 >> 노아 쪽에서 값을 넣을 수 있도록
                            IndoorNode.Attributes.Append(unit);

                            XmlAttribute datetime = xmlDoc.CreateAttribute("datetime");
                            datetime.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            // TODO: 다운로드 시에 속성 값 넣는 부분 수정이 필요 >> 노아 쪽에서 값을 넣을 수 있도록
                            IndoorNode.Attributes.Append(datetime);

                            break;
                        }
                    }

                    break;
                }
            }

            xmlDoc.Save(strXMLFile);
            bResult = true;

            return bResult;
        }

        private string DownloadProject(string strBuildingKey, ref string strResultMessage)
        {
            string strResult = null;

            XElement xUserID = MakeElement("user_id", m_strID);
            XElement xKeyID = MakeElement("key_id", m_strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xBuildingID = MakeElement("build_mng_no", strBuildingKey);

            XElement xBuilding = new XElement("building");
            xBuilding.Add(xBuildingID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xBuilding);

            string strXML = xRoot.ToString();
            strResult = SendQuery(strXML, "spatial/spatialDetail2", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("ReadProject Error : " + strResultMessage);
                strResultMessage = "ReadProject Error : " + strResultMessage;
                return null;
            }

            return strResult;
        }



        public bool UploadProject(Project project, string strBuildingKey, ref string strResultMessage, ref ProgressInfo progressInfo)
        {
            if (project == null || strBuildingKey == null)
            {
                strResultMessage = "해당 프로젝트 또는 빌딩키가 없습니다. 확인 부탁드리겠습니다.";
                return false;
            }

            m_strBuildingKey = strBuildingKey;

            // 상세속성 조회
            bool bChk = GetGrpCodeIDs(m_strPropTable, ref strResultMessage);

            if (!bChk)
                return false;

            // XML 속성에 프로젝트 ID 얻어오기
            string strProjectID = GetProjectID(project);

            if (strProjectID == null)
            {   // 서버에 첫 업로드 XML 파일인 경우
                if (!UploadNewXML(project, ref strResultMessage, ref progressInfo))
                    return false;
            }
            else if (strBuildingKey != strProjectID)
            {
                // 프로젝트 ID가 다를 경우 주소지에 맞는 정보가 아님을 표시 후 취소
                strResultMessage = "주소지에 맞는 공간정보가 아닙니다.";
                return false;
            }
            else
            {   // 프로젝트 ID가 같을 경우

                // level 체크
                List<Level> levels = project.Levels;
                Dictionary<string, string> dicRemoveLevel = new Dictionary<string, string>();

                // Level ID 수정(변경) 유무
                bool bCheck = false;

                // 노아서버 Level과 XML Level 비교 
                bCheck = CheckModifity(levels, out dicRemoveLevel, ref strResultMessage);
                /*
                if (dicRemoveLevel.Count == 0 && bCheck == false)
                {   // level ID 같을 경우 수정된 내용이 없음!!
                    strResultMessage = "수정된 내용이 없습니다.";
                    return false;
                }
                else if (dicRemoveLevel.Count != 0 || bCheck == true)
                {   // level 다를 경우 수정할 레벨 수정
                    if (!UploadModifityLevel(project, dicRemoveLevel, ref strResultMessage, ref progressInfo))
                        return false;
                }
                */
                if (!UploadModifityLevel(project, dicRemoveLevel, ref strResultMessage, ref progressInfo))
                    return false;
            }

            return true;
        }


        private bool UploadModifityLevel(Project project, Dictionary<string, string> dicRemoveLevel, ref string strResultMessage, ref ProgressInfo progressInfo)
        {
            List<Property> properties = project.Properties;
            List<Level> levels = project.Levels;

            Dictionary<string, string> dicLevel = ReadLevelNames(m_strID, m_strKey, ref strResultMessage);

            if (dicLevel == null)
                return false;

            //int i = 1;
            //int lvcnt = levels.Count + dicRemoveLevel.Count;
            //double percent = (double)i;
            int i = 1;
            int nCount = dicRemoveLevel.Count + levels.Count;

            // 삭제할 레벨 삭제
            foreach (KeyValuePair<string, string> pair in dicRemoveLevel)
            {
                string strLevelID = pair.Key;
                string strLevelName = pair.Value;

                // 진행률 관련
                double dPercent = ((double)i / (double)nCount) * 100;
                string strMessage = strLevelName + " 층 삭제 중: " + dPercent.ToString() + "%";
                progressInfo.Message = strMessage;
                progressInfo.Percent = (int)dPercent;
                i++;

                if (RemoveLevelComponent(strLevelID, m_strID, m_strKey, ref strResultMessage) == false)
                    return false;
            }

            // 프로젝트 앵커노드 업로드
            AnchorNode anchor = project.AnchorNode;
            if (!UploaBuildingAnchorNode(anchor, m_strID, m_strKey, ref strResultMessage))
                return false;

            // 프로젝트 속성 업로드
            if (!UploaBuildingProperty(properties, m_strID, m_strKey, ref strResultMessage))
                return false;

            foreach (Level level in levels)
            {
                // 로그인(세션키 재발급
                if (Login(m_strID, m_strPW, ref strResultMessage) == false)
                    return false;

                string strLevelID = level.XMLID;

                // 진행률 관련
                double dPercent = ((double)i / (double)nCount) * 100;
                string strMessage = level.Name + " 층 업로드 중: " + dPercent.ToString() + "%";
                progressInfo.Message = strMessage;
                progressInfo.Percent = (int)dPercent;
                i++;

                if (dicLevel.ContainsKey(level.XMLID))
                {   // Level ID가 같을 경우 
                    // 객체별로 비교 후 추가, 삭제 및 속성 수정 
                    if (UpdateLevelComponent(level, out strResultMessage) == false)
                        return false;

                }
                else if (dicLevel.ContainsValue(level.Name))
                {   // Level ID가 다르고 똑같은 Level Name이 있을 경우 새로운 Level ID 값으로 데이터 갱신

                    // 똑같은 Level Name의 ID 가져오기
                    foreach (KeyValuePair<string, string> item in dicLevel)
                    {
                        if (item.Value == level.Name)
                        {
                            strLevelID = item.Key;
                            break;
                        }
                    }

                    // 똑같은 Level Name의 ID 값으로 데이터 갱신
                    if (UploadLevelComponent(level, strLevelID, ref strResultMessage) == false)
                        return false;
                }
                else
                {
                    // level을 업로드 후에 xmlID를 새로 갱신 후에 진행
                    strLevelID = UploadLevel(m_strBuildingKey, level, m_strID, m_strKey, ref strResultMessage);

                    // 새로 추가된 Level일 경우 새로 갱신
                    if (!UploadNewLevel(level, strLevelID, ref strResultMessage))
                        return false;
                }
            }

            return true;
        }

        private bool UpdateLevelComponent(Level level, out string strResultMessage)
        {
            strResultMessage = "";

            // 그리드 업데이트
            Dictionary<Wall, string> dicGridIDs = UpdateGrids(level.Walls, level.XMLID, m_strID, m_strKey, out strResultMessage);
            if (dicGridIDs == null)
                return false;

            // 벽체 업데이트
            Dictionary<Wall, string> dicWallIDs = UpdateWalls(dicGridIDs, level.XMLID, m_strID, m_strKey, out strResultMessage);
            if (dicWallIDs == null)
                return false;

            // 도어 업데이트
            if (UpdateDoors(dicWallIDs, level.XMLID, m_strID, m_strKey, out strResultMessage) == false)
                return false;

            // 윈도우 업데이트
            if (UpdateWindows(dicWallIDs, level.XMLID, m_strID, m_strKey, out strResultMessage) == false)
                return false;

            // 스페이스 업데이트
            Dictionary<Space, string> dicSpaceIDs = UpdateSpaces(level.Spaces, level.XMLID, m_strID, m_strKey, out strResultMessage);
            if (dicSpaceIDs == null)
                return false;

            // 벽체 링크 업데이트
            if (UploadSpaceWallLink(dicSpaceIDs, dicWallIDs, level.XMLID, m_strID, m_strKey, ref strResultMessage) == false)
                return false;

            // 경계구역 업데이트
            if (UpdateAlertAreas(level.AlertAreas, level.XMLID, m_strID, m_strKey, out strResultMessage) == null)
                return false;

            // 기둥 업데이트
            if (UpdateColumns(level.Columns, level.XMLID, m_strID, m_strKey, out strResultMessage) == false)
                return false;



            // .TODO: 토폴리지 오류 발생으로 인한 주석처리
            // 토폴리지 관련 업데이트
            //Dictionary<Topology, string> dicTopologyIDs = UploadTopologies_NEW(level.Topologies, level.XMLID, m_strID, m_strKey, ref strResultMessage);

            //if (dicTopologyIDs == null)
            //    return false;

            //Dictionary<Topology.Node, string> dicTopologyNodeIDs = UploadTopologyNodes(dicTopologyIDs, m_strID, m_strKey, ref strResultMessage);

            //if (dicTopologyNodeIDs == null)
            //    return false;

            //if (UploadTopologyNodeLinks(dicTopologyNodeIDs, m_strID, m_strKey, ref strResultMessage) == false)
            //    return false;




            // 공간정보만 업로드 하도록 주석처리
            //poi데이터 업로드.
            //Dictionary<Shapes.POI, string> dicPOIIDs = UploadPOIs_NEW(level.POIs, m_strBuildingKey, level.XMLID, m_strID, m_strKey, ref strResultMessage);
            //if (dicPOIIDs == null)
            //    return false;

            //// poi wire 업데이트
            //if (UploadPOIWires_NEW(level.Wires, dicPOIIDs, level, level.XMLID, m_strID, m_strKey, ref strResultMessage) == false)
            //    return false;

            return true;
        }

        private bool UpdateColumns(List<Column> columns, string strLevelID, string strID, string strKey, out string strResultMessage)
        {
            strResultMessage = "";

            if (columns == null)
            {
                strResultMessage = "columns가 null 값 입니다.";
                return false;
            }

            // 노아서버 Column 조회
            List<string> columnList = GetColumnList(strLevelID, strID, strKey, ref strResultMessage);
            if (columnList == null)
                return false;


            // Column 비교
            List<Column> addColumns = new List<Column>();
            Dictionary<Column, string> dicModifiColumns = new Dictionary<Column, string>();

            foreach (Column column in columns)
            {
                addColumns.Add(column);
            }

            foreach (Column column in columns)
            {
                foreach (string strColumnID in columnList)
                {
                    if (strColumnID == column.XMLID)
                    {   // 이미 존재하는 Column 추가,삭제 목록에서 제외
                        addColumns.Remove(column);
                        columnList.Remove(strColumnID);

                        dicModifiColumns[column] = column.XMLID;
                        break;
                    }
                }
            }


            // 삭제할 Column 노아서버에 삭제 요청
            if (RemoveColumns(columnList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            // 새로 추가될 Column 노아서버에 추가 요청
            if (UploadColumns(strLevelID, addColumns, strID, strKey, ref strResultMessage) == false)
                return false;

            // 수정할 Column 속성 노아서버에 수정 요청
            if (UpdateColumnProperty(dicModifiColumns, strLevelID, strID, strKey, out strResultMessage) == false)
                return false;

            return true;
        }

        private bool UpdateColumnProperty(Dictionary<Column, string> dicColumns, string strLevelID, string strID, string strKey, out string strResultMessage)
        {
            strResultMessage = "";

            foreach (KeyValuePair<Column, string> pair in dicColumns)
            {
                string strColumnID = pair.Value;

                // Column 속성 조회
                List<string> columnPropList = GetPropIDs(strColumnID, strID, strKey, "column", ref strResultMessage);

                if (columnPropList == null)
                    return false;

                // Column 속성 삭제
                if (!RemoveProps(strColumnID, columnPropList, strID, strKey, "column", ref strResultMessage))
                    return false;
            }

            // Column 속성 추가
            if (!UploadColumnProperty(dicColumns, strID, strKey, ref strResultMessage))
                return false;

            return true;
        }

        private Dictionary<AlertArea, string> UpdateAlertAreas(List<AlertArea> alertAreas, string strLevelID, string strID, string strKey, out string strResultMessage)
        {
            strResultMessage = "";

            if (alertAreas == null)
            {
                strResultMessage = "alertAreas가 null 값 입니다.";
                return null;
            }

            // 노아서버 경계구역 조회
            List<string> alertAreaList = GetAlertAreaList(strLevelID, strID, strKey, ref strResultMessage);
            if (alertAreaList == null)
                return null;

            List<AlertArea> addAlertAreas = new List<AlertArea>();
            Dictionary<AlertArea, string> dicModifiAlertAreas = new Dictionary<AlertArea, string>();

            foreach (AlertArea alertArea in alertAreas)
            {
                addAlertAreas.Add(alertArea);
            }


            // AlertArea 비교
            foreach (AlertArea area in alertAreas)
            {
                foreach (string strAlertAreaID in alertAreaList)
                {
                    if (strAlertAreaID == area.XMLID)
                    {   // 이미 존재하는 AlertArea 추가,삭제 목록에서 제외
                        addAlertAreas.Remove(area);
                        alertAreaList.Remove(strAlertAreaID);

                        dicModifiAlertAreas[area] = area.XMLID;
                        break;
                    }
                }
            }




            // 삭제할 경계구역 노아서버에 삭제 요청
            if (RemoveAlertAreas(alertAreaList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return null;

            // 추가할 경계구역 노아서버에 추가 요청
            Dictionary<AlertArea, string> dicAlertAreaIDs = UploadAlertAreas(addAlertAreas, strLevelID, strID, strKey, ref strResultMessage);
            if (dicAlertAreaIDs == null)
                return null;

            // 수정할 경계구역 속성 노아서버에 수정 요청
            if (UpdateAlertAreaProperty(dicModifiAlertAreas, strLevelID, strID, strKey, out strResultMessage) == false)
                return null;

            return dicAlertAreaIDs;
        }

        private bool UpdateAlertAreaProperty(Dictionary<AlertArea, string> dicAlertAreaIDs, string strLevelID, string strID, string strKey, out string strResultMessage)
        {
            strResultMessage = "";

            foreach (KeyValuePair<AlertArea, string> pair in dicAlertAreaIDs)
            {
                string strAlertID = pair.Value;

                // 경계구역 속성 조회
                List<string> alertAreaPropList = GetPropIDs(strAlertID, strID, strKey, "alertArea", ref strResultMessage);

                if (alertAreaPropList == null)
                    return false;

                // 경계구역 속성 삭제
                if (!RemoveProps(strAlertID, alertAreaPropList, strID, strKey, "alertArea", ref strResultMessage))
                {
                    Console.WriteLine("RemoveProps error " + strResultMessage);
                    //return false;
                }
            }

            if (!UploadAlertAreaProperty(dicAlertAreaIDs, strID, strKey, ref strResultMessage))
                return false;

            return true;
        }

        /// <summary>
        /// 공간정보 데이터 Upload용 클래스
        /// </summary>
        private class DataXMLID
        {
            private string m_strWallID = null;
            private string m_strDoorID = null;
            private string m_strWindowID = null;

            public string WallID
            {
                get { return m_strWallID; }
                set { m_strWallID = value; }
            }

            public string DoorID
            {
                get { return m_strDoorID; }
                set { m_strDoorID = value; }
            }

            public string WindowID
            {
                get { return m_strWindowID; }
                set { m_strWindowID = value; }
            }
        }

        private bool UpdateWindows(Dictionary<Wall, string> dicWalls, string strLevelID, string strID, string strKey, out string strResultMessage)
        {
            strResultMessage = "";

            // 노아서버 창문 조회
            List<string> windowList = GetWindowList(strLevelID, strID, strKey, ref strResultMessage);
            if (windowList == null)
                return false;

            // 현재 Local Window 조회
            Dictionary<Window, DataXMLID> dicWindows = new Dictionary<Window, DataXMLID>();
            Dictionary<Window, DataXMLID> dicAddWindows = new Dictionary<Window, DataXMLID>();
            Dictionary<Window, string> dicModifiWindows = new Dictionary<Window, string>();

            foreach (KeyValuePair<Wall, string> pair in dicWalls)
            {
                Wall wall = pair.Key;

                foreach (Window window in wall.Windows)
                {
                    DataXMLID data = new DataXMLID();
                    data.WallID = pair.Value;
                    data.WindowID = window.XMLID;

                    dicWindows[window] = data;
                    dicAddWindows[window] = data;
                }
            }

            // Window 비교
            foreach (KeyValuePair<Window, DataXMLID> pair in dicWindows)
            {
                Window window = pair.Key;
                DataXMLID data = pair.Value;

                if (data.WindowID == null)
                {
                    strResultMessage = "DataXMLID에 Window XMLID 값이 null 입니다.";
                    return false;
                }

                string strWindowID = data.WindowID;

                foreach (string strXMLID in windowList)
                {
                    if (strWindowID == strXMLID)
                    {   // 이미 존재하는 Window 추가,삭제 목록에서 제외
                        windowList.Remove(strXMLID);
                        dicAddWindows.Remove(window);

                        dicModifiWindows[window] = strWindowID;
                        break;
                    }
                }
            }



            // 삭제할 Window 노아서버에 삭제 요청
            if (RemoveWindows(windowList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            // 새로 추가할 Window 노아서버에 추가 요청
            if (UploadWindows(dicAddWindows, strLevelID, strID, strKey, out strResultMessage) == false)
                return false;

            // 수정할 Window 노아서버에 수정 요청
            if (UpdateWindowProperty(dicModifiWindows, strLevelID, strID, strKey, out strResultMessage) == false)
                return false;

            return true;
        }

        private bool UpdateWindowProperty(Dictionary<Window, string> dicWindows, string strLevelID, string strID, string strKey, out string strResultMessage)
        {
            strResultMessage = "";

            foreach (KeyValuePair<Window, string> pair in dicWindows)
            {
                string strWindowID = pair.Value;

                // 창문 속성 조회
                List<string> windowPropList = GetPropIDs(strWindowID, strID, strKey, "window", ref strResultMessage);

                if (windowPropList == null)
                    return false;

                // 창문 속성 삭제
                if (!RemoveProps(strWindowID, windowPropList, strID, strKey, "window", ref strResultMessage))
                    return false;
            }

            if (UploadWindowProperty(dicWindows, strID, strKey, ref strResultMessage) == false)
                return false;

            return true;
        }

        private bool UploadWindows(Dictionary<Window, DataXMLID> dicWindows, string strLevelID, string strID, string strKey, out string strResultMessage)
        {
            strResultMessage = "";

            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            int nWindowCount = 0;

            foreach (KeyValuePair<Window, DataXMLID> pair in dicWindows)
            {
                string strWallID = pair.Value.WallID;
                Window window = pair.Key;

                XElement xWindow = new XElement("windowList");

                XElement xWallID = new XElement("wall_id", strWallID);
                XElement xPosX = new XElement("x", GetDoubleString(window.Position.x));
                XElement xPosY = new XElement("y", GetDoubleString(window.Position.y));
                XElement xWidth = new XElement("width", GetDoubleString(window.Width));
                XElement xHeight = new XElement("height", GetDoubleString(window.Height));
                XElement xElevation = new XElement("elevation", GetDoubleString(window.Elevation));
                XElement xLevelID = new XElement("level_id", strLevelID);

                xWindow.Add(xWallID);
                xWindow.Add(xPosX);
                xWindow.Add(xPosY);
                xWindow.Add(xWidth);
                xWindow.Add(xHeight);
                xWindow.Add(xElevation);
                xWindow.Add(xLevelID);

                xRoot.Add(xWindow);
                nWindowCount++;
            }

            if (nWindowCount == 0)
                return true;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/window", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadWindows Error : " + strResultMessage);
                strResultMessage = "UploadWindows Error : " + strResultMessage;
                return false;
            }
            //------

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadDoor Error2 : " + strResult);
                strResultMessage = "UploadDoor Error2 : " + strResult;
                return false;
            }

            Dictionary<Window, string> dicWindowIDs = new Dictionary<Window, string>();
            List<Window> dlist = new List<Window>();// tmp Window List
            foreach (KeyValuePair<Window, DataXMLID> tmpPair in dicWindows)
            {
                Window wi = tmpPair.Key;
                dlist.Add(wi);
            }

            int nWindowIndex = 0;
            foreach (XElement element in xml.Elements())
            {
                XElement xLevel = element.Name == "windowList" ? element : null;

                if (xLevel != null)
                {
                    foreach (XElement child in xLevel.Elements())
                    {
                        string strWindowID = "";
                        if (child.Name == "window_id")
                        {
                            strWindowID = child.Value;

                            if (strWindowID.Length > 0)
                            {
                                if (nWindowIndex < dlist.Count)
                                {
                                    dicWindowIDs.Add(dlist[nWindowIndex], strWindowID);
                                    nWindowIndex++;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            if (!UploadWindowProperty(dicWindowIDs, strID, strKey, ref strResultMessage))
                return false;

            return true;
        }


        private bool UpdateDoors(Dictionary<Wall, string> dicWalls, string strLevelID, string strID, string strKey, out string strResultMessage)
        {
            strResultMessage = "";

            // 노아서버 Door 조회
            List<string> doorList = GetDoorList(strLevelID, strID, strKey, ref strResultMessage);
            if (doorList == null)
                return false;


            // 현재 Local Door 조회
            Dictionary<Door, DataXMLID> dicDoors = new Dictionary<Door, DataXMLID>();
            Dictionary<Door, DataXMLID> dicAddDoors = new Dictionary<Door, DataXMLID>();
            Dictionary<Door, string> dicModifiDoors = new Dictionary<Door, string>();

            foreach (KeyValuePair<Wall, string> pair in dicWalls)
            {
                Wall wall = pair.Key;

                foreach (Door door in wall.Doors)
                {
                    DataXMLID data = new DataXMLID();
                    data.WallID = pair.Value;
                    data.DoorID = door.XMLID;

                    dicDoors[door] = data;
                    dicAddDoors[door] = data;
                }
            }

            // Door 비교
            foreach (KeyValuePair<Door, DataXMLID> pair in dicDoors)
            {
                Door door = pair.Key;
                DataXMLID data = pair.Value;

                if (data.DoorID == null)
                {
                    strResultMessage = "DataXMLID에 Door XMLID 값이 null 입니다.";
                    return false;
                }
                    
                string strDoorID = data.DoorID;

                foreach (string strXMLID in doorList)
                {
                    if (strDoorID == strXMLID)
                    {   // 이미 존재하는 Door 추가,삭제 목록에서 제외
                        doorList.Remove(strXMLID);
                        dicAddDoors.Remove(door);

                        dicModifiDoors[door] = strDoorID;
                        break;
                    }
                }
            }


            // 삭제할 Door 노아서버에 삭제 요청
            if (RemoveDoors(doorList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            // 추가할 Door 노아서버에 추가 요청
            if (UploadDoors(dicAddDoors, strLevelID, strID, strKey, out strResultMessage) == false)
                return false;

            // 수정할 Door 노아서버에 수정 요청
            if (UpdateDoorProperty(dicModifiDoors, strLevelID, strID, strKey, out strResultMessage) == false)
                return false;

            return true;
        }

        private bool UpdateDoorProperty(Dictionary<Door, string> dicDoors, string strLevelID, string strID, string strKey, out string strResultMessage)
        {
            strResultMessage = "";

            foreach (KeyValuePair<Door, string> pair in dicDoors)
            {
                string strDoorID = pair.Value;

                // Door 속성 조회
                List<string> doorPropList = GetPropIDs(strDoorID, strID, strKey, "door", ref strResultMessage);

                if (doorPropList == null)
                    return false;

                // Door 속성 삭제
                if (!RemoveProps(strDoorID, doorPropList, strID, strKey, "door", ref strResultMessage))
                    return false;
            }

            // Door 속성 추가
            if (!UploadDoorProperty(dicDoors, strID, strKey, ref strResultMessage))
                return false;

            return true;
        }

        private bool UploadDoors(Dictionary<Door, DataXMLID> dicDoors, string strLevelID, string strID, string strKey, out string strResultMessage)
        {
            strResultMessage = "";

            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            int nDoorCount = 0;

            foreach (KeyValuePair<Door, DataXMLID> pair in dicDoors)
            {
                Door door = pair.Key;
                string strWallID = pair.Value.WallID;

                XElement xDoor = new XElement("doorList");

                XElement xWallID = new XElement("wall_id", strWallID);
                XElement xPosX = new XElement("x", GetDoubleString(door.Position.x));
                XElement xPosY = new XElement("y", GetDoubleString(door.Position.y));
                XElement xWidth = new XElement("width", GetDoubleString(door.Width));

                xDoor.Add(xWallID);
                xDoor.Add(xPosX);
                xDoor.Add(xPosY);
                xDoor.Add(xWidth);

                if (door.Hinge1 != null)
                {
                    XElement xHinge1X = new XElement("hinge_1x", GetDoubleString(door.Hinge1.x));
                    XElement xHinge1Y = new XElement("hinge_1y", GetDoubleString(door.Hinge1.y));

                    xDoor.Add(xHinge1X);
                    xDoor.Add(xHinge1Y);
                }

                if (door.Hinge2 != null)
                {
                    XElement xHinge2X = new XElement("hinge_2x", GetDoubleString(door.Hinge2.x));
                    XElement xHinge2Y = new XElement("hinge_2y", GetDoubleString(door.Hinge2.y));

                    xDoor.Add(xHinge2X);
                    xDoor.Add(xHinge2Y);
                }

                XElement xHeight = new XElement("height", GetDoubleString(door.Height));
                XElement xElevation = new XElement("elevation", GetDoubleString(door.Elevation));
                XElement xDoorType = new XElement("door_type", ((int)door.GetDoorType()).ToString());
                XElement xLevelID = new XElement("level_id", strLevelID);

                xDoor.Add(xHeight);
                xDoor.Add(xElevation);
                xDoor.Add(xDoorType);
                xDoor.Add(xLevelID);

                xRoot.Add(xDoor);
                nDoorCount++;
            }

            if (nDoorCount == 0)
                return true;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/door", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadDoors Error : " + strResultMessage);
                strResultMessage = "UploadDoors Error : " + strResultMessage;
                return false;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadDoor Error2 : " + strResult);
                strResultMessage = "UploadDoor Error2 : " + strResult;
                return false;
            }

            Dictionary<Door, string> dicDoorIDs = new Dictionary<Door, string>();
            List<Door> dlist = new List<Door>();// tmp Door List
            foreach (KeyValuePair<Door, DataXMLID> pair in dicDoors)
            {
                Door dr = pair.Key;

                dlist.Add(dr);
            }

            int nDoorIndex = 0;
            foreach (XElement element in xml.Elements())
            {
                XElement xLevel = element.Name == "doorList" ? element : null;

                if (xLevel != null)
                {
                    foreach (XElement child in xLevel.Elements())
                    {
                        string strDoorID = "";
                        if (child.Name == "door_id")
                        {
                            strDoorID = child.Value;

                            if (strDoorID.Length > 0)
                            {
                                if (nDoorIndex < dlist.Count)
                                {
                                    dicDoorIDs.Add(dlist[nDoorIndex], strDoorID);
                                    nDoorIndex++;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            if (!UploadDoorProperty(dicDoorIDs, strID, strKey, ref strResultMessage))
                return false;

            return true;
        }

        private Dictionary<Space, string> UpdateSpaces(List<Space> spaces, string strLevelID, string strID, string strKey, out string strResultMessage)
        {
            strResultMessage = "";

            if (spaces == null)
            {
                strResultMessage = "spaces가 null 값 입니다.";
                return null;
            }

            // 노아서버 Space 조회
            List<string> spaceList = GetSpaceList(strLevelID, strID, strKey, ref strResultMessage);
            if (spaceList == null)
                return null;

            // Local Space 데이터화
            List<Space> addSpaces = new List<Space>();
            Dictionary<Space, string> dicSpaceIDs = new Dictionary<Space, string>();
            Dictionary<Space, string> dicModifiSpaces = new Dictionary<Space, string>();

            foreach (Space space in spaces)
            {
                dicSpaceIDs[space] = space.XMLID;
                addSpaces.Add(space);
            }


            // Space 비교 
            foreach (Space space in spaces)
            {
                foreach (string strSpaceID in spaceList)
                {   
                    if (strSpaceID == space.XMLID)
                    {   // 이미 존재하는 Space 추가,삭제 목록에서 제외
                        spaceList.Remove(strSpaceID);
                        addSpaces.Remove(space);

                        // 수정목록
                        dicModifiSpaces[space] = space.XMLID;
                        break;
                    }
                }
            }




            // 삭제할 Space 노아서버에 삭제 요청
            if (RemoveSpaces(spaceList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return null;

            // 새로 추가할 Space 노아서버에 추가 요청
            Dictionary<Space, string> dicAddSpaceIDs = UploadSpaces(addSpaces, strLevelID, strID, strKey, ref strResultMessage);
            if (dicAddSpaceIDs == null)
                return null;

            if (UpdateSpaceProperty(dicModifiSpaces, strLevelID, strID, strKey, out strResultMessage) == false)
                return null;


            // 추가된 Wall을 Local Wall에 적용
            //foreach (KeyValuePair<Space, string> pair in dicSpaceIDs)
            //{
            //    Space space = pair.Key;

            //    if (dicAddSpaceIDs.ContainsKey(space))
            //    {
            //        string strSpaceID = dicAddSpaceIDs[space];
            //        dicSpaceIDs[space] = strSpaceID;
            //    }
            //}
            foreach (KeyValuePair<Space, string> pair in dicAddSpaceIDs)
            {
                Space space = pair.Key;

                if (dicSpaceIDs.ContainsKey(space))
                {
                    string strSpaceID = dicAddSpaceIDs[space];
                    dicSpaceIDs[space] = strSpaceID;
                }
            }


            return dicSpaceIDs;
        }

        private bool UpdateSpaceProperty(Dictionary<Space, string> dicSpaceIDs, string strLevelID, string strID, string strKey, out string strResultMessage)
        {
            strResultMessage = "";

            foreach (KeyValuePair<Space, string> pair in dicSpaceIDs)
            {
                string strSpaceID = pair.Value;

                // 공간 속성 조회
                List<string> spacePropList = GetPropIDs(strSpaceID, strID, strKey, "space", ref strResultMessage);
                if (spacePropList == null)
                    return false;

                // 공간 속성 삭제
                if (!RemoveProps(strSpaceID, spacePropList, strID, strKey, "space", ref strResultMessage))
                    return false;
            }

            if (!UploadSpaceProperty(dicSpaceIDs, strID, strKey, ref strResultMessage))
                return false;

            return true;
        }

        private Dictionary<Wall, string> UpdateWalls(Dictionary<Wall, string> dicGridIDs, string strLevelID, string strID, string strKey, out string strResultMessage)
        {
            strResultMessage = "";

            if (dicGridIDs == null)
            {
                strResultMessage = "dicGridIDs가 null 입니다.";
                return null;
            }

            // 노아서버 Wall 조회
            List<string> wallList = GetWallList(strLevelID, strID, strKey, ref strResultMessage);
            if (wallList == null)
                return null;

            // Local Wall 데이터화 
            Dictionary<Wall, string> dicAddWalls = new Dictionary<Wall, string>();
            Dictionary<Wall, string> dicWalls = new Dictionary<Wall, string>();

            foreach (KeyValuePair<Wall, string> pair in dicGridIDs)
            {
                Wall wall = pair.Key;
                string strGridID = pair.Value;

                dicAddWalls[wall] = strGridID;
            }

            // Wall 비교
            foreach (KeyValuePair<Wall, string> pair in dicGridIDs)
            {
                Wall wall = pair.Key;
                string strWallID = wall.XMLID;

                foreach (string strWall in wallList)
                {
                    if (strWallID == strWall)
                    {   // 이미 존재하는 Wall는 추가,삭제 목록에서 제외
                        wallList.Remove(strWall);
                        dicAddWalls.Remove(wall);

                        // 수정 목록에 추가
                        dicWalls[wall] = strWallID;
                        break;
                    }
                }
            }

            // 삭제할 Wall 노아서버에 삭제 요청
            if (RemoveWalls(wallList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return null;

            // 새로 추가할 Wall 노아서버에 추가 요청
            Dictionary<Wall, string> dicWallIDs = UploadWalls(dicAddWalls, strLevelID, strID, strKey, ref strResultMessage);
            if (dicWallIDs == null)
                return null;

            // Wall 속성 수정
            if (UpdateWallProperty(dicWalls, strLevelID, strID, strKey, ref strResultMessage) == false)
                return null;


            //// 추가된 Wall을 Local Wall에 적용
            //foreach (KeyValuePair<Wall, string> pair in dicGridIDs)
            //{
            //    Wall wall = pair.Key;

            //    if (dicWallIDs.ContainsKey(wall))
            //    {
            //        string strWallID = dicWallIDs[wall];
            //        dicGridIDs[wall] = strWallID;
            //    }
            //}

            // 추가된 Wall을 Local Wall에 적용
            foreach (KeyValuePair<Wall, string> pair in dicWallIDs)
            {
                Wall wall = pair.Key;
                string strWallID = pair.Value;

                if (dicGridIDs.ContainsKey(wall))
                {   
                    dicGridIDs[wall] = strWallID;
                }

                dicWalls[wall] = strWallID;
            }


            return dicWalls;
        }

        private bool UpdateWallProperty(Dictionary<Wall, string> dicWallIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            foreach (KeyValuePair<Wall, string> pair in dicWallIDs)
            {
                string strWallID = pair.Value;

                // 벽 속성 조회
                List<string> wallPropList = GetPropIDs(strWallID, strID, strKey, "wall", ref strResultMessage);
                if (wallPropList == null)
                    return false;

                // 벽 속성 삭제
                if (!RemoveProps(strWallID, wallPropList, strID, strKey, "wall", ref strResultMessage))
                    return false;
            }

            // 벽 속성 추가
            if (!UploadWallProperty(dicWallIDs, strID, strKey, ref strResultMessage))
                return false;

            return true;
        }

        private Dictionary<Wall, string> UpdateGrids(List<Wall> walls, string strLevelID, string strID, string strKey, out string strResultMessage)
        {
            strResultMessage = "";

            if (walls == null)
            {
                strResultMessage = "walls 데이터가 null 입니다.";
                return null;
            }

            // 노아서버 벽체선형 조회
            List<string> gridList = GetGridList(strLevelID, strID, strKey, ref strResultMessage);
            if (gridList == null)
                return null;

            // Local Grid 데이터화 
            Dictionary<Wall, string> dicGrids = new Dictionary<Wall, string>();
            List<Wall> addGrids = new List<Wall>();

            foreach (Wall wall in walls)
            {
                dicGrids[wall] = wall.GridID;
                addGrids.Add(wall);
            }



            // Grid 비교
            foreach (KeyValuePair<Wall, string> pair in dicGrids)
            {
                string strGridID = pair.Value;
                Wall wall = pair.Key;

                foreach (string strGrid in gridList)
                {
                    if (strGridID == strGrid)
                    {   // 이미 존재하는 grid 는 추가, 삭제 목록에서 제외
                        addGrids.Remove(wall);
                        gridList.Remove(strGrid);
                        break;
                    }
                }
            }



            // 삭제할 Grid 노아서버에 삭제 요청
            if (RemoveGrids(gridList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return null;

            // 추가된 Grid 노아서버에 추가 요청
            Dictionary<Wall, string> dicGridIDs = UploadGrids(addGrids, strLevelID, strID, strKey, ref strResultMessage);

            if (dicGridIDs == null)
                return null;



            // 추가된 Grid를 Local Grid에 적용
            //foreach (KeyValuePair<Wall, string> pair in dicGrids)
            //{
            //    Wall wall = pair.Key;

            //    if (dicGridIDs.ContainsKey(wall))
            //    {
            //        string strGridID = dicGridIDs[wall];
            //        dicGrids[wall] = strGridID;
            //    }
            //}
            foreach (KeyValuePair<Wall, string> pair in dicGridIDs)
            {
                Wall wall = pair.Key;

                if (dicGrids.ContainsKey(wall))
                {
                    string strGridID = dicGridIDs[wall];
                    dicGrids[wall] = strGridID;
                }
            }

            return dicGrids;
        }

        

        private string UploadLevel(string strBuildingID, Level level, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            XElement xLevel = new XElement("levelList");

            XElement xBuildingID = MakeElement("build_mng_no", strBuildingID);
            xLevel.Add(xBuildingID);

            XElement xElevation = MakeElement("elevation", GetDoubleString(level.Elevation));
            xLevel.Add(xElevation);

            XElement xName = MakeElement("level_name", level.Name);
            xLevel.Add(xName);

            xRoot.Add(xLevel);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/level", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadLevel Error : " + strResultMessage);
                strResultMessage = "UploadLevel Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadLevel Error2 : " + strResult);
                strResultMessage = "UploadLevel Error2 : " + strResult;
                return null;
            }

            int nLevelIndex = 0;
            string strRetID = "";
            Dictionary<Level, string> dicLevelIDs = new Dictionary<Level, string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xLevelList = element.Name == "levelList" ? element : null;

                if (xLevelList != null)
                {
                    foreach (XElement child in xLevelList.Elements())
                    {
                        string strLevelID = "";

                        if (child.Name == "level_id")
                        {
                            strLevelID = child.Value;

                            if (strLevelID.Length > 0)
                            {
                                dicLevelIDs[level] = strLevelID;
                                strRetID = strLevelID;
                                nLevelIndex++;

                                break;
                            }
                        }
                    }
                }
            }
            if (!UploadLevelProperty(dicLevelIDs, strID, strKey, ref strResultMessage))
                return null;

            return strRetID;
        }

        private bool UploadLevelComponent(Level level, string strLevelID, ref string strResultMessage)
        {
            Dictionary<Wall, string> dicGridIDs = UploadGrids_NEW(level.Walls, strLevelID, m_strID, m_strKey, ref strResultMessage);

            if (dicGridIDs == null)
                return false;

            Dictionary<Wall, string> dicWallIDs = UploadWalls_NEW(dicGridIDs, strLevelID, m_strID, m_strKey, ref strResultMessage);

            if (dicWallIDs == null)
                return false;

            if (UploadDoors_NEW(dicWallIDs, strLevelID, m_strID, m_strKey, ref strResultMessage) == false)
                return false;

            if (UploadWindows_NEW(dicWallIDs, strLevelID, m_strID, m_strKey, ref strResultMessage) == false)
                return false;

            Dictionary<Space, string> dicSpaceIDs = UploadSpaces_NEW(level.Spaces, strLevelID, m_strID, m_strKey, ref strResultMessage);

            if (dicSpaceIDs == null)
                return false;

            if (UploadSpaceWallLink(dicSpaceIDs, dicWallIDs, strLevelID, m_strID, m_strKey, ref strResultMessage) == false)
                return false;

            Dictionary<AlertArea, string> dicAlertAreaIDs = UploadAlertAreas_NEW(level.AlertAreas, strLevelID, m_strID, m_strKey, ref strResultMessage);

            if (dicAlertAreaIDs == null)
                return false;

            if (UploadColumns_NEW(strLevelID, level.Columns, m_strID, m_strKey, ref strResultMessage) == false)
                return false;



            // .TODO: 토폴리지 오류 발생으로 인한 주석처리
            //Dictionary<Topology, string> dicTopologyIDs = UploadTopologies_NEW(level.Topologies, strLevelID, m_strID, m_strKey, ref strResultMessage);

            //if (dicTopologyIDs == null)
            //    return false;

            //Dictionary<Topology.Node, string> dicTopologyNodeIDs = UploadTopologyNodes(dicTopologyIDs, m_strID, m_strKey, ref strResultMessage);

            //if (dicTopologyNodeIDs == null)
            //    return false;

            //if (UploadTopologyNodeLinks(dicTopologyNodeIDs, m_strID, m_strKey, ref strResultMessage) == false)
            //    return false;


            // 공간정보만 업로드 하도록 주석처리
            //poi데이터 업로드.
            //Dictionary<Shapes.POI, string> dicPOIIDs = UploadPOIs_NEW(level.POIs, m_strBuildingKey, strLevelID, m_strID, m_strKey, ref strResultMessage);
            //if (dicPOIIDs == null)
            //    return false;

            //if (UploadPOIWires_NEW(level.Wires, dicPOIIDs, level, strLevelID, m_strID, m_strKey, ref strResultMessage) == false)
            //    return false;

            return true;
        }

        private bool UploadPOIWires_NEW(List<Shapes.Wire> wires, Dictionary<Shapes.POI, string> dicPOIIDs, Level level, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            // 노아서버 POIWire 조회
            List<string> poiWireList = GetPOIWireList(strLevelID, strID, strKey, ref strResultMessage);


            // 조회된 POIWire 노아서버에 삭제 요청
            if (RemovePOIWires(poiWireList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            // POIWire 노아서버에 추가 요청
            if (UploadPOIWires(wires, dicPOIIDs, level, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            return true;
        }

        private Dictionary<Shapes.POI, string> UploadPOIs_NEW(List<Shapes.POI> pois, string strBuildingID, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            //List<Shapes.POI> pois = level.POIs;
            //string strLevelName = level.Name;

            // 노아서버 POI 조회
            List<string> POIList = GetPOIList(strBuildingID, strLevelID, strID, strKey, ref strResultMessage);

            if (POIList == null)
                return null;

            // 삭제될 POI
            List<string> removePOIList = GetPOIList(strBuildingID, strLevelID, strID, strKey, ref strResultMessage);

            if (removePOIList == null)
                return null;

            // 수정될 POI 
            Dictionary<Shapes.POI, string> dicModifityPOIs = new Dictionary<Shapes.POI, string>();
            // 추가될 POI 
            //Dictionary<Shapes.POI, string> dicAddPOIs = new Dictionary<Shapes.POI, string>();
            List<Shapes.POI> addPOIList = new List<Shapes.POI>();

            foreach (Shapes.POI poi in pois)
            {
                // 수정된 현황 체크
                bool bChk = false;

                foreach (string strPOIID in POIList)
                {
                    if (poi.XMLID == strPOIID)
                    {
                        bChk = true;

                        // 수정된 POI
                        dicModifityPOIs[poi] = poi.XMLID;
                        // 수정된 POI 으로 삭제할 항목에서 제거
                        removePOIList.Remove(strPOIID);
                    }
                }

                // 중복되지 않으므로 추가된 POI
                if (bChk == false)
                {
                    //dicAddPOIs[poi] = poi.XMLID;
                    addPOIList.Add(poi);
                }
            }

            // 삭제된 POI 노아서버에 삭제 요청
            if (RemovePOIs(removePOIList, strID, strKey, ref strResultMessage) == false)
                return null;

            // 추가된 POI 노아서버에 추가 요청
            Dictionary<Shapes.POI, string> dicPOIs = UploadPOIs(addPOIList, strBuildingID, strLevelID, strID, strKey, ref strResultMessage);

            if (dicPOIs == null)
                return null;

            // 수정된 POI 노아서버에 수정 요청
            if (UpdatePOIs(dicModifityPOIs, strBuildingID, strLevelID, strID, strKey, ref strResultMessage) == false)
                return null;

            Dictionary<Shapes.POI, string> dicPOIIDs = new Dictionary<Shapes.POI, string>();

            foreach (Shapes.POI poi in pois)
            {
                if (dicPOIs.ContainsKey(poi))
                {
                    dicPOIIDs[poi] = dicPOIs[poi];
                }
                else
                {
                    dicPOIIDs[poi] = poi.XMLID;
                }
            }

            return dicPOIIDs;
        }

        private bool UpdatePOIs(Dictionary<Shapes.POI, string> dicPOIs, string strBuildingID, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("convergence");
            xRoot.Add(xState);

            foreach (KeyValuePair<Shapes.POI, string> pair in dicPOIs)
            {
                Shapes.POI poi = pair.Key;

                if (poi.PoiType == null || poi.PoiType.Code == null || poi.PoiType.Code.Length == 0)
                    continue;

                XElement xEquip = new XElement("equip");

                XElement xEquipID = new XElement("equip_id", poi.XMLID);
                XElement xEquipName = new XElement("equip_name", poi.Name);
                XElement xEquipTypeCode = new XElement("equip_type_code", poi.PoiType.Code);//code? ID?

                XElement xLevel = new XElement("floor", strLevelID);
                XElement xBuilding = new XElement("build_mng_no", strBuildingID);
                XElement xPosX = new XElement("x", GetDoubleString(poi.Position.x));
                XElement xPosY = new XElement("y", GetDoubleString(poi.Position.y));
                XElement xAngle = new XElement("angle", GetDoubleString(poi.Angle));

                // POI Height 단위
                int nHeight = poi.Height * 10;
                XElement xHeight = new XElement("height", nHeight.ToString());

                xEquip.Add(xEquipID);
                xEquip.Add(xEquipName);
                xEquip.Add(xEquipTypeCode);
                xEquip.Add(xLevel);
                xEquip.Add(xBuilding);
                xEquip.Add(xPosX);
                xEquip.Add(xPosY);
                xEquip.Add(xAngle);
                xEquip.Add(xHeight);

                xRoot.Add(xEquip);

                string strXML = xRoot.ToString();
                string strResult = SendQuery(strXML, "convergence/fireEquip", true, out strResultMessage, "PUT");

                if (strResult.Length == 0)
                {
                    System.Diagnostics.Trace.WriteLine("UpdatePOIs Error : " + strResultMessage);
                    strResultMessage = "UpdatePOIs Error : " + strResultMessage;
                    return false;
                }

                XElement xml = XElement.Parse(strResult);

                if (xml == null)
                {
                    System.Diagnostics.Trace.WriteLine("UpdatePOIs Error2 : " + strResult);
                    strResultMessage = "UpdatePOIs Error2 : " + strResult;
                    return false;
                }
            }

            // poi 속성 업데이트
            if (UpdatePOIProperty(dicPOIs, strID, strKey, out strResultMessage) == false)
                return false;

            return true;
        }

        private bool UpdatePOIProperty(Dictionary<Shapes.POI, string> dicPOIs, string strID, string strKey, out string strResultMessage)
        {
            strResultMessage = "";

            foreach (KeyValuePair<Shapes.POI, string> pair in dicPOIs)
            {
                string strPOIID = pair.Value;

                // POI 속성 조회
                List<string> poiPropList = GetPOIPropIDs(strPOIID, strID, strKey, ref strResultMessage);

                if (poiPropList == null)
                    return false;

                // POI 속성 삭제
                if (!RemovePOIProps(strPOIID, poiPropList, strID, strKey, ref strResultMessage))
                    return false;
            }

            // POI 속성 등록
            if (!UploadPOIProperty(dicPOIs, strID, strKey, ref strResultMessage))
                return false;

            return true;
        }

        private Dictionary<Topology, string> UploadTopologies_NEW(List<Topology> topologies, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            Dictionary<Topology, string> dicTopologyIDs = new Dictionary<Topology, string>();

            if (topologies.Count == 0)
                return dicTopologyIDs;

            // 노아서버 Topology 조회
            List<string> topologyList = GetTopologyList(strLevelID, strID, strKey, ref strResultMessage);

            if (topologyList == null)
                return null;

            // 조회된 Topology 노아서버에 삭제 요청
            if (RemoveTopologes(topologyList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return null;

            dicTopologyIDs = UploadTopologies(topologies, strLevelID, strID, strKey, ref strResultMessage);

            if (dicTopologyIDs == null)
                return null;

            return dicTopologyIDs;
        }

        private bool UploadColumns_NEW(string strLevelID, List<Column> columns, string strID, string strKey, ref string strResultMessage)
        {
            // 노아서버 Column 조회
            List<string> columnList = GetColumnList(strLevelID, strID, strKey, ref strResultMessage);


            // 조회된 Column 노아서버에 삭제 요청
            if (RemoveColumns(columnList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            if (columns.Count == 0)
                return true;


            if (UploadColumns(strLevelID, columns, strID, strKey, ref strResultMessage) == false)
                return false;


            return true;
        }

        private Dictionary<AlertArea, string> UploadAlertAreas_NEW(List<AlertArea> alertAreas, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            // 노아서버 경계구역 조회
            List<string> alertAreaList = GetAlertAreaList(strLevelID, strID, strKey, ref strResultMessage);

            if (alertAreaList == null)
                return null;

            // 조회된 경계구역 노아서버에 삭제 요청
            if (RemoveAlertAreas(alertAreaList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return null;

            Dictionary<AlertArea, string> dicAlertAreaIDs = UploadAlertAreas(alertAreas, strLevelID, strID, strKey, ref strResultMessage);

            if (dicAlertAreaIDs == null)
                return null;

            return dicAlertAreaIDs;
        }

        private Dictionary<Space, string> UploadSpaces_NEW(List<Space> spaces, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            // 노아서버 공간 조회
            List<string> spaceList = GetSpaceList(strLevelID, strID, strKey, ref strResultMessage);

            if (spaceList == null)
                return null;

            // 조회된 공간 노아서버에 삭제 요청
            if (RemoveSpaces(spaceList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return null;

            Dictionary<Space, string> dicSpaceIDs = UploadSpaces(spaces, strLevelID, strID, strKey, ref strResultMessage);

            if (dicSpaceIDs == null)
                return null;

            return dicSpaceIDs;
        }

        private bool UploadWindows_NEW(Dictionary<Wall, string> dicWalls, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            // 노아서버 창문 조회
            List<string> windowList = GetWindowList(strLevelID, strID, strKey, ref strResultMessage);

            if (windowList == null)
                return false;

            // 삭제된 창문 노아서버에 삭제 요청
            if (RemoveWindows(windowList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            if (UploadWindows(dicWalls, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            return true;
        }

        private bool UploadDoors_NEW(Dictionary<Wall, string> dicWalls, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            // 노아서버 문 조회
            List<string> doorList = GetDoorList(strLevelID, strID, strKey, ref strResultMessage);

            if (doorList == null)
                return false;

            // 조회된 문 노아서버에 삭제 요청
            if (RemoveDoors(doorList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            if (UploadDoors(dicWalls, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            return true;
        }

        private Dictionary<Wall, string> UploadWalls_NEW(Dictionary<Wall, string> dicGridIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            // 노아서버 벽 조회
            List<string> wallList = GetWallList(strLevelID, strID, strKey, ref strResultMessage);

            if (wallList == null)
                return null;

            // 조회된 벽 노아서버에 삭제 요청
            if (RemoveWalls(wallList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return null;

            Dictionary<Wall, string> dicWallIDs = UploadWalls(dicGridIDs, strLevelID, strID, strKey, ref strResultMessage);

            if (dicWallIDs == null)
                return null;

            return dicWallIDs;
        }

        private Dictionary<Wall, string> UploadGrids_NEW(List<Wall> walls, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            // 노아서버 벽체선형 조회
            List<string> gridList = GetGridList(strLevelID, strID, strKey, ref strResultMessage);

            if (gridList == null)
                return null;

            // 조회된 벽체 노아서버에 삭제 요청
            if (RemoveGrids(gridList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return null;

            Dictionary<Wall, string> dicGridIDs = UploadGrids(walls, strLevelID, strID, strKey, ref strResultMessage);

            if (dicGridIDs == null)
                return null;

            return dicGridIDs;
        }

        private bool CheckModifity(List<Level> levels, out Dictionary<string, string> dicRemoveLevel, ref string strResultMessage)
        {   // 노아서버 Level과 XML Level 비교 

            Dictionary<string, string> dicLevel = new Dictionary<string, string>();

            // 노아서버 현재 레벨 조회
            dicLevel = ReadLevelNames(m_strID, m_strKey, ref strResultMessage);
            dicRemoveLevel = ReadLevelNames(m_strID, m_strKey, ref strResultMessage);

            // Level ID 수정(변경) 유무 체크
            bool bRetModifity = false;

            foreach (Level level in levels)
            {
                if (dicLevel.ContainsKey(level.XMLID))
                {
                    // 동일한 Level ID가 있기에 삭제 항목에서 제외
                    dicRemoveLevel.Remove(level.XMLID);
                }
                else if (dicLevel.ContainsValue(level.Name))
                {
                    foreach (KeyValuePair<string, string> item in dicLevel)
                    {
                        // 동일한 Level Name이 있기에 삭제 항목에서 제외
                        if (item.Value == level.Name)
                        {
                            dicRemoveLevel.Remove(item.Key);
                            bRetModifity = true;
                        }
                    }
                }
                else
                {
                    // 새로 추가된 층
                    bRetModifity = true;
                }
            }

            return bRetModifity;
        }

        // 새로운 XML 파일 업로드
        private bool UploadNewXML(Project project, ref string strResultMessage, ref ProgressInfo progressInfo)
        {   
            SetDoubleString(project);

            // 프로젝트 앵커노드 업로드
            AnchorNode anchor = project.AnchorNode;
            if (!UploaBuildingAnchorNode(anchor, m_strID, m_strKey, ref strResultMessage))
                return false;

            // 프로젝트 속성 업로드
            List<Property> properties = project.Properties;
            if (!UploaBuildingProperty(properties, m_strID, m_strKey, ref strResultMessage))
                return false;

            // 기존에 등록되어 있는 층 조회
            Dictionary<string, string> dicRemoveLevels = ReadLevelNames(m_strID, m_strKey, ref strResultMessage);
            if (dicRemoveLevels == null)
                return false;

            // 진행율 관련 
            int i = 1;
            int nCount = dicRemoveLevels.Count + project.Levels.Count;

            // 조회된 층 삭제
            foreach (KeyValuePair<string, string> pair in dicRemoveLevels)
            {
                string strLevelID = pair.Key;
                string strLevelName = pair.Value;

                double dPercent = ((double)i / (double)nCount) * 100;
                string strMessage = strLevelName + " 층 삭제 중: " + dPercent.ToString() + "%";
                progressInfo.Message = strMessage;
                progressInfo.Percent = (int)dPercent;
                i++;

                if (RemoveLevelComponent(strLevelID, m_strID, m_strKey, ref strResultMessage) == false)
                    return false;
            }

            Dictionary<Level, string> dicLevels = UploadLevels(m_strBuildingKey, project.Levels, m_strID, m_strKey, ref strResultMessage);

            if (dicLevels == null)
                return false;

            foreach(KeyValuePair<Level, string> pair in dicLevels)
            {
                // 로그인(세션키 재발급)
                if (Login(m_strID, m_strPW, ref strResultMessage) == false)
                    return false;

                Level level = pair.Key;

                if (!UploadNewLevel(level, pair.Value, ref strResultMessage))
                    return false;

                double dPercent = ((double)i / (double)nCount) * 100;
                string strMessage = level.Name + " 층 업로드 중: " + dPercent.ToString() + "%";
                progressInfo.Message = strMessage;
                progressInfo.Percent = (int)dPercent;
                i++;
            }

            return true;
        }

        private bool UploadNewLevel(Level level, string strLevelID, ref string strResultMessage)
        {
            Dictionary<Wall, string> dicGridIDs = UploadGrids(level.Walls, strLevelID, m_strID, m_strKey, ref strResultMessage);

            if (dicGridIDs == null)
                return false;

            Dictionary<Wall, string> dicWallIDs = UploadWalls(dicGridIDs, strLevelID, m_strID, m_strKey, ref strResultMessage);

            if (dicWallIDs == null)
                return false;

            Dictionary<Space, string> dicSpaceIDs = UploadSpaces(level.Spaces, strLevelID, m_strID, m_strKey, ref strResultMessage);

            if (dicSpaceIDs == null)
                return false;

            if (UploadSpaceWallLink(dicSpaceIDs, dicWallIDs, strLevelID, m_strID, m_strKey, ref strResultMessage) == false)
                return false;

            // 경계구역 업로드
            Dictionary<AlertArea, string> dicAlertAreaIDs = UploadAlertAreas(level.AlertAreas, strLevelID, m_strID, m_strKey, ref strResultMessage);

            if (dicAlertAreaIDs == null)
                return false;

            if (UploadDoors(dicWallIDs, strLevelID, m_strID, m_strKey, ref strResultMessage) == false)
                return false;

            if (UploadWindows(dicWallIDs, strLevelID, m_strID, m_strKey, ref strResultMessage) == false)
                return false;

            if (UploadColumns(strLevelID, level.Columns, m_strID, m_strKey, ref strResultMessage) == false)
                return false;


            // .TODO: 토폴리지 오류 발생으로 인한 주석처리
            //Dictionary<Topology, string> dicTopologyIDs = UploadTopologies(level.Topologies, strLevelID, m_strID, m_strKey, ref strResultMessage);

            //if (dicTopologyIDs == null)
            //    return false;

            //Dictionary<Topology.Node, string> dicTopologyNodeIDs = UploadTopologyNodes(dicTopologyIDs, m_strID, m_strKey, ref strResultMessage);

            //if (dicTopologyNodeIDs == null)
            //    return false;

            //if (UploadTopologyNodeLinks(dicTopologyNodeIDs, m_strID, m_strKey, ref strResultMessage) == false)
            //    return false;





            // 공간정보만 업로드 하도록 주석처리
            //poi데이터 업로드.
            //Dictionary<Shapes.POI, string> dicPOIIDs = UploadPOIs(level.POIs, m_strBuildingKey, strLevelID, m_strID, m_strKey, ref strResultMessage);

            //if (dicPOIIDs == null)
            //    return false;

            //if (UploadPOIWires(level.Wires, dicPOIIDs, level, strLevelID, m_strID, m_strKey, ref strResultMessage) == false)
            //    return false;

            return true;
        }

        private bool UploadPOIWires(List<Shapes.Wire> wires, Dictionary<Shapes.POI, string> dicPOIIDs, Level level, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            int nWireCount = 0;

            foreach (Shapes.Wire wire in wires)
            {
                if (wire.POIIcon == null || wire.POIIcon.PoiType == null || wire.POIIcon.PoiType.Code == null || wire.POIIcon.PoiType.Code.Length == 0)
                    continue;

                Shapes.POI beginPOI = level.FindPOI(wire.BeginPOI);

                if (beginPOI == null)
                    continue;

                Shapes.POI endPOI = level.FindPOI(wire.EndPOI);

                if (endPOI == null)
                    continue;

                string strBeginID = null, strEndID = null;

                if (dicPOIIDs.TryGetValue(beginPOI, out strBeginID) == false || dicPOIIDs.TryGetValue(endPOI, out strEndID) == false)
                    continue;

                XElement xEquip = new XElement("equipWireList");

                XElement xLevelID = new XElement("level_id", strLevelID);
                XElement xBeginPOI = new XElement("begin_equip", strBeginID);
                XElement xEndPOI = new XElement("end_equip", strEndID);
                XElement xEquipTypeCode = new XElement("equip_type_code", wire.POIIcon.PoiType.Code);
                XElement xLines = new XElement("lines", wire.Lines);

                xEquip.Add(xLevelID);
                xEquip.Add(xBeginPOI);
                xEquip.Add(xEndPOI);
                xEquip.Add(xEquipTypeCode);
                xEquip.Add(xLines);

                xRoot.Add(xEquip);
                nWireCount++;
            }

            if (nWireCount == 0)
                return true;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/equipWire", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadPOIWires Error : " + strResultMessage);
                strResultMessage = "UploadPOIWires Error : " + strResultMessage;
                return false;
            }

            return true;
        }

        private Dictionary<Shapes.POI, string> UploadPOIs(List<Shapes.POI> pois, string strBuildingID, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("convergence");
            xRoot.Add(xState);

            List<Shapes.POI> codePOIs = new List<Shapes.POI>();

            foreach (Shapes.POI poi in pois)
            {
                if (poi.PoiType == null || poi.PoiType.Code == null || poi.PoiType.Code.Length == 0)
                    continue;

                XElement xEquip = new XElement("equipList");

                XElement xEquipName = new XElement("equip_name", poi.Name);
                XElement xEquipTypeCode = new XElement("equip_type_code", poi.PoiType.Code);
                XElement xLevel = new XElement("floor", strLevelID);
                XElement xBuilding = new XElement("build_mng_no", strBuildingID);
                XElement xPosX = new XElement("x", GetDoubleString(poi.Position.x));
                XElement xPosY = new XElement("y", GetDoubleString(poi.Position.y));
                XElement xAngle = new XElement("angle", GetDoubleString(poi.Angle));

                // POI Height 단위
                int nHeight = poi.Height * 10;
                XElement xHeight = new XElement("height", nHeight.ToString());

                xEquip.Add(xEquipName);
                xEquip.Add(xEquipTypeCode);
                xEquip.Add(xLevel);
                xEquip.Add(xBuilding);
                xEquip.Add(xPosX);
                xEquip.Add(xPosY);
                xEquip.Add(xAngle);
                xEquip.Add(xHeight);

                xRoot.Add(xEquip);
                codePOIs.Add(poi);
            }

            Dictionary<Shapes.POI, string> dicPOIIDs = new Dictionary<Shapes.POI, string>();

            if (codePOIs.Count == 0)
                return dicPOIIDs;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "convergence/fireEquip", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadPOIs Error : " + strResultMessage);
                strResultMessage = "UploadPOIs Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadPOIs Error2 : " + strResult);
                strResultMessage = "UploadPOIs Error2 : " + strResult;
                return null;
            }

            int nPOIIndex = 0;
            int nPOICount = codePOIs.Count;

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "equipList" ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "equip_id")
                        {
                            if (nPOIIndex >= nPOICount)
                            {
                                System.Diagnostics.Trace.WriteLine("UploadPOIs Error : 응답받은 POI 개수가 전송한 POI 개수보다 많습니다.");
                                strResultMessage = "UploadPOIs Error : 응답받은 POI 개수가 전송한 POI 개수보다 많습니다.";
                                return null;
                            }

                            dicPOIIDs[codePOIs[nPOIIndex++]] = child.Value;
                            break;
                        }
                    }
                }
            }

            if (nPOIIndex != nPOICount)
            {
                System.Diagnostics.Trace.WriteLine("UploadPOIs Error : 응답받은 POI 개수와 전송한 POI 개수가 일치하지 않습니다.");
                strResultMessage = "UploadPOIs Error : 응답받은 POI 개수와 전송한 POI 개수가 일치하지 않습니다.";
                return null;
            }

            // POI 속성 등록
            if (!UploadPOIProperty(dicPOIIDs, strID, strKey, ref strResultMessage))
                return null;

            return dicPOIIDs;
        }

        private bool UploadPOIProperty(Dictionary<POI, string> dicPOIIDs, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("convergence");
            xRoot.Add(xState);

            bool flag = false;
            foreach (KeyValuePair<POI, string> item in dicPOIIDs)
            {
                POI poi = item.Key;
                string strPOIID = item.Value;

                foreach (Property prop in poi.Properties)
                {
                    XElement xWall = new XElement("equipPropList");

                    XElement xType = new XElement("equip_id", strPOIID);
                    XElement xCode = new XElement("property_code", GetPropertyCode(prop.Name));
                    XElement xValue = new XElement("property_value", prop.Value);

                    xWall.Add(xType);
                    xWall.Add(xCode);
                    xWall.Add(xValue);

                    xRoot.Add(xWall);
                    flag = true;
                }
            }
            if (!flag)
                return true;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "convergence/fireEquipProp", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadPOIProperty Error : " + strResultMessage);
                return false;
            }

            return true;
        }

        private bool UploadTopologyNodeLinks(Dictionary<Topology.Node, string> dicTopologyNodeIDs, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            string strLinkID = "";
            int nLinkCount = 0;

            foreach (KeyValuePair<Topology.Node, string> pair in dicTopologyNodeIDs)
            {
                Topology.Node node = pair.Key;

                foreach (Topology.Node link in node.LinkedNodes)
                {
                    if (dicTopologyNodeIDs.TryGetValue(link, out strLinkID) == false)
                        continue;

                    XElement xLinkNode = new XElement("topologyNodeLinkList");

                    XElement xNodeID = new XElement("node_id", pair.Value);
                    XElement xTargetID = new XElement("target_node_id", strLinkID);

                    xLinkNode.Add(xNodeID);
                    xLinkNode.Add(xTargetID);

                    xRoot.Add(xLinkNode);
                    nLinkCount++;
                }
            }

            if (nLinkCount == 0)
                return true;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/topologyNodeLink", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadTopologyNodeLinks Error : " + strResultMessage);
                strResultMessage = "UploadTopologyNodeLinks Error : " + strResultMessage;
                return false;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadTopologyNodes Error2 : " + strResult);
                strResultMessage = "UploadTopologyNodes Error2 : " + strResult;
                return false;
            }

            return true;
        }

        private Dictionary<Topology.Node, string> UploadTopologyNodes(Dictionary<Topology, string> dicTopologyIDs, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            List<Topology.Node> nodes = new List<Topology.Node>();

            foreach (KeyValuePair<Topology, string> pair in dicTopologyIDs)
            {
                Topology topology = pair.Key;

                foreach (Topology.Node node in topology.Nodes)
                {
                    XElement xTopologyNode = new XElement("topologyNodeList");

                    XElement xTopologyID = new XElement("topology_id", pair.Value);
                    XElement xPosX = new XElement("x", GetDoubleString(node.X));
                    XElement xPosY = new XElement("y", GetDoubleString(node.Y));

                    xTopologyNode.Add(xTopologyID);
                    xTopologyNode.Add(xPosX);
                    xTopologyNode.Add(xPosY);

                    xRoot.Add(xTopologyNode);
                    nodes.Add(node);
                }
            }

            Dictionary<Topology.Node, string> dicTopologyNodeIDs = new Dictionary<Topology.Node, string>();

            if (nodes.Count == 0)
                return dicTopologyNodeIDs;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/topologyNode", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadTopologyNodes Error : " + strResultMessage);
                strResultMessage = "UploadTopologyNodes Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadTopologyNodes Error2 : " + strResult);
                strResultMessage = "UploadTopologyNodes Error2 : " + strResult;
                return null;
            }

            int nNodeIndex = 0;
            int nNodeCount = nodes.Count;

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "topologyNodeList" ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "node_id")
                        {
                            if (nNodeIndex >= nNodeCount)
                            {
                                System.Diagnostics.Trace.WriteLine("UploadTopologyNodes Error : 응답받은 TopologyNode 개수가 전송한 TopologyNode 개수보다 많습니다.");
                                strResultMessage = "UploadTopologyNodes Error : 응답받은 TopologyNode 개수가 전송한 TopologyNode 개수보다 많습니다.";
                                return null;
                            }

                            dicTopologyNodeIDs[nodes[nNodeIndex++]] = child.Value;
                            break;
                        }
                    }
                }
            }

            if (nNodeIndex != nNodeCount)
            {
                System.Diagnostics.Trace.WriteLine("UploadTopologyNodes Error : 응답받은 TopologyNode 개수와 전송한 TopologyNode 개수가 일치하지 않습니다.");
                strResultMessage = "UploadTopologyNodes Error : 응답받은 TopologyNode 개수와 전송한 TopologyNode 개수가 일치하지 않습니다.";
                return null;
            }

            return dicTopologyNodeIDs;
        }

        private Dictionary<Topology, string> UploadTopologies(List<Topology> topologies, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            Dictionary<Topology, string> dicTopologyIDs = new Dictionary<Topology, string>();

            if (topologies.Count == 0)
                return dicTopologyIDs;

            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            foreach (Topology topology in topologies)
            {
                XElement xTopology = new XElement("topologyList");
                XElement xLevelID = new XElement("level_id", strLevelID);

                xTopology.Add(xLevelID);
                xRoot.Add(xTopology);
            }

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/topology", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadTopologies Error : " + strResultMessage);
                strResultMessage = "UploadTopologies Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadTopologies Error2 : " + strResult);
                strResultMessage = "UploadTopologies Error2 : " + strResult;
                return null;
            }

            int nTopologyIndex = 0;
            int nTopologyCount = topologies.Count;

            foreach (XElement element in xml.Elements())
            {
                XElement xTopology = element.Name == "topologyList" ? element : null;

                if (xTopology != null)
                {
                    foreach (XElement child in xTopology.Elements())
                    {
                        if (child.Name == "topology_id")
                        {
                            if (nTopologyIndex >= nTopologyCount)
                            {
                                System.Diagnostics.Trace.WriteLine("UploadTopologies Error : 응답받은 Topology 개수가 전송한 Topology 개수보다 많습니다.");
                                strResultMessage = "UploadTopologies Error : 응답받은 Topology 개수가 전송한 Topology 개수보다 많습니다.";
                                return null;
                            }

                            dicTopologyIDs[topologies[nTopologyIndex++]] = child.Value;
                            break;
                        }
                    }
                }
            }

            if (nTopologyIndex != nTopologyCount)
            {
                System.Diagnostics.Trace.WriteLine("UploadTopologies Error : 응답받은 Topology 개수와 전송한 Topology 개수가 일치하지 않습니다.");
                strResultMessage = "UploadTopologies Error : 응답받은 Topology 개수와 전송한 Topology 개수가 일치하지 않습니다.";
                return null;
            }

            return dicTopologyIDs;
        }

        private bool UploadColumns(string strLevelID, List<Column> columns, string strID, string strKey, ref string strResultMessage)
        {
            if (columns.Count == 0)
                return true;

            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            foreach (Column col in columns)
            {
                XElement xColumn = new XElement("columnList");

                if (col.Type == Column.ColumnType.Rect)
                {
                    XElement xType = new XElement("column_type", 0);
                    XElement xTlx = new XElement("tl_x", col.RectData.TopLeft.x);
                    XElement xTly = new XElement("tl_y", col.RectData.TopLeft.y);
                    XElement xBlx = new XElement("bl_x", col.RectData.BottomLeft.x);
                    XElement xBly = new XElement("bl_y", col.RectData.BottomLeft.y);
                    XElement xBrx = new XElement("br_x", col.RectData.BottomRight.x);
                    XElement xBry = new XElement("br_y", col.RectData.BottomRight.y);
                    XElement xLevelID = new XElement("level_id", strLevelID);

                    xColumn.Add(xType);
                    xColumn.Add(xTlx);
                    xColumn.Add(xTly);
                    xColumn.Add(xBlx);
                    xColumn.Add(xBly);
                    xColumn.Add(xBrx);
                    xColumn.Add(xBry);
                    xColumn.Add(xLevelID);
                }
                else
                {
                    XElement xType = new XElement("column_type", 1);
                    XElement xTlx = new XElement("tl_x", col.CircleData.Center.x);
                    XElement xTly = new XElement("tl_y", col.CircleData.Center.y);
                    XElement xBlx = new XElement("bl_x", col.CircleData.Radius);
                    XElement xLevelID = new XElement("level_id", strLevelID);

                    xColumn.Add(xType);
                    xColumn.Add(xTlx);
                    xColumn.Add(xTly);
                    xColumn.Add(xBlx);
                    xColumn.Add(xLevelID);
                }

                xRoot.Add(xColumn);

            }

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/column", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadColumn Error : " + strResultMessage);
                strResultMessage = "UploadColumn Error : " + strResultMessage;
                return false;
            }
            //--
            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadColumn Error2 : " + strResult);
                strResultMessage = "UploadColumn Error2 : " + strResult;
                return false;
            }

            Dictionary<Column, string> dicColumnIDs = new Dictionary<Column, string>();

            int nColumnIndex = 0;

            foreach (XElement element in xml.Elements())
            {
                XElement xLevel = element.Name == "columnList" ? element : null;

                if (xLevel != null)
                {
                    foreach (XElement child in xLevel.Elements())
                    {
                        string strColumnID = "";

                        if (child.Name == "column_id")
                        {
                            strColumnID = child.Value;

                            if (strColumnID.Length > 0)
                            {
                                if (nColumnIndex < columns.Count)
                                {
                                    Column column = columns[nColumnIndex];

                                    dicColumnIDs[column] = strColumnID;
                                    nColumnIndex++;
                                }

                                break;
                            }
                        }
                    }
                }
            }

            if (!UploadColumnProperty(dicColumnIDs, strID, strKey, ref strResultMessage))
                return false;

            return true;
        }

        private bool UploadColumnProperty(Dictionary<Column, string> dicColumnIDs, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            bool flag = false;
            foreach (KeyValuePair<Column, string> item in dicColumnIDs)
            {
                Column col = item.Key;
                string strColumnID = item.Value;

                foreach (Property prop in col.Properties)
                {
                    XElement xColumn = new XElement("columnPropList");

                    XElement xType = new XElement("column_id", strColumnID);
                    XElement xCode = new XElement("property_code", GetPropertyCode(prop.Name));
                    XElement xValue = new XElement("property_value", prop.Value);

                    xColumn.Add(xType);
                    xColumn.Add(xCode);
                    xColumn.Add(xValue);

                    xRoot.Add(xColumn);
                    flag = true;
                }
            }
            if (!flag)
                return true;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/columnProp", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadColumnProperty Error : " + strResultMessage);
                strResultMessage = "UploadColumnProperty Error : " + strResultMessage;
                return false;
            }

            return true;
        }

        private bool UploadWindows(Dictionary<Wall, string> dicWallIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            int nWindowCount = 0;

            foreach (KeyValuePair<Wall, string> pair in dicWallIDs)
            {
                Wall wall = pair.Key;

                foreach (Window window in wall.Windows)
                {
                    XElement xWindow = new XElement("windowList");

                    XElement xWallID = new XElement("wall_id", pair.Value);
                    XElement xPosX = new XElement("x", GetDoubleString(window.Position.x));
                    XElement xPosY = new XElement("y", GetDoubleString(window.Position.y));
                    XElement xWidth = new XElement("width", GetDoubleString(window.Width));
                    XElement xHeight = new XElement("height", GetDoubleString(window.Height));
                    XElement xElevation = new XElement("elevation", GetDoubleString(window.Elevation));
                    XElement xLevelID = new XElement("level_id", strLevelID);

                    xWindow.Add(xWallID);
                    xWindow.Add(xPosX);
                    xWindow.Add(xPosY);
                    xWindow.Add(xWidth);
                    xWindow.Add(xHeight);
                    xWindow.Add(xElevation);
                    xWindow.Add(xLevelID);

                    xRoot.Add(xWindow);
                    nWindowCount++;
                }
            }

            if (nWindowCount == 0)
                return true;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/window", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadWindows Error : " + strResultMessage);
                strResultMessage = "UploadWindows Error : " + strResultMessage;
                return false;
            }
            //------

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadDoor Error2 : " + strResult);
                strResultMessage = "UploadDoor Error2 : " + strResult;
                return false;
            }

            Dictionary<Window, string> dicWindowIDs = new Dictionary<Window, string>();
            List<Window> dlist = new List<Window>();// tmp Window List
            foreach (KeyValuePair<Wall, string> tmpPair in dicWallIDs)
            {
                Wall wall = tmpPair.Key;
                foreach (Window wi in wall.Windows)
                    dlist.Add(wi);
            }

            int nWindowIndex = 0;
            foreach (XElement element in xml.Elements())
            {
                XElement xLevel = element.Name == "windowList" ? element : null;

                if (xLevel != null)
                {
                    foreach (XElement child in xLevel.Elements())
                    {
                        string strWindowID = "";
                        if (child.Name == "window_id")
                        {
                            strWindowID = child.Value;

                            if (strWindowID.Length > 0)
                            {
                                if (nWindowIndex < dlist.Count)
                                {
                                    dicWindowIDs.Add(dlist[nWindowIndex], strWindowID);
                                    nWindowIndex++;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            if (!UploadWindowProperty(dicWindowIDs, strID, strKey, ref strResultMessage))
                return false;

            return true;
        }

        private bool UploadWindowProperty(Dictionary<Window, string> dicWindowIDs, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            bool flag = false;
            foreach (KeyValuePair<Window, string> item in dicWindowIDs)
            {
                Window window = item.Key;
                string strWindowID = item.Value;

                foreach (Property prop in window.Properties)
                {
                    XElement xWindow = new XElement("windowPropList");

                    XElement xType = new XElement("window_id", strWindowID);
                    XElement xCode = new XElement("property_code", GetPropertyCode(prop.Name));
                    XElement xValue = new XElement("property_value", prop.Value);

                    xWindow.Add(xType);
                    xWindow.Add(xCode);
                    xWindow.Add(xValue);

                    xRoot.Add(xWindow);
                    flag = true;
                }
            }
            if (!flag)
                return true;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/windowProp", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadDoorProperty Error : " + strResultMessage);
                strResultMessage = "UploadDoorProperty Error : " + strResultMessage;
                return false;
            }

            return true;
        }

        private bool UploadDoors(Dictionary<Wall, string> dicWallIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            int nDoorCount = 0;

            foreach (KeyValuePair<Wall, string> pair in dicWallIDs)
            {
                Wall wall = pair.Key;

                foreach (Door door in wall.Doors)
                {
                    XElement xDoor = new XElement("doorList");

                    XElement xWallID = new XElement("wall_id", pair.Value);
                    XElement xPosX = new XElement("x", GetDoubleString(door.Position.x));
                    XElement xPosY = new XElement("y", GetDoubleString(door.Position.y));
                    XElement xWidth = new XElement("width", GetDoubleString(door.Width));

                    xDoor.Add(xWallID);
                    xDoor.Add(xPosX);
                    xDoor.Add(xPosY);
                    xDoor.Add(xWidth);

                    if (door.Hinge1 != null)
                    {
                        XElement xHinge1X = new XElement("hinge_1x", GetDoubleString(door.Hinge1.x));
                        XElement xHinge1Y = new XElement("hinge_1y", GetDoubleString(door.Hinge1.y));

                        xDoor.Add(xHinge1X);
                        xDoor.Add(xHinge1Y);
                    }

                    if (door.Hinge2 != null)
                    {
                        XElement xHinge2X = new XElement("hinge_2x", GetDoubleString(door.Hinge2.x));
                        XElement xHinge2Y = new XElement("hinge_2y", GetDoubleString(door.Hinge2.y));

                        xDoor.Add(xHinge2X);
                        xDoor.Add(xHinge2Y);
                    }

                    XElement xHeight = new XElement("height", GetDoubleString(door.Height));
                    XElement xElevation = new XElement("elevation", GetDoubleString(door.Elevation));
                    XElement xDoorType = new XElement("door_type", ((int)door.GetDoorType()).ToString());
                    XElement xLevelID = new XElement("level_id", strLevelID);

                    xDoor.Add(xHeight);
                    xDoor.Add(xElevation);
                    xDoor.Add(xDoorType);
                    xDoor.Add(xLevelID);

                    xRoot.Add(xDoor);
                    nDoorCount++;
                }
            }

            if (nDoorCount == 0)
                return true;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/door", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadDoors Error : " + strResultMessage);
                strResultMessage = "UploadDoors Error : " + strResultMessage;
                return false;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadDoor Error2 : " + strResult);
                strResultMessage = "UploadDoor Error2 : " + strResult;
                return false;
            }

            Dictionary<Door, string> dicDoorIDs = new Dictionary<Door, string>();
            List<Door> dlist = new List<Door>();// tmp Door List
            foreach (KeyValuePair<Wall, string> tmpPair in dicWallIDs)
            {
                Wall wall = tmpPair.Key;
                foreach (Door dr in wall.Doors)
                    dlist.Add(dr);
            }

            int nDoorIndex = 0;
            foreach (XElement element in xml.Elements())
            {
                XElement xLevel = element.Name == "doorList" ? element : null;

                if (xLevel != null)
                {
                    foreach (XElement child in xLevel.Elements())
                    {
                        string strDoorID = "";
                        if (child.Name == "door_id")
                        {
                            strDoorID = child.Value;

                            if (strDoorID.Length > 0)
                            {
                                if (nDoorIndex < dlist.Count)
                                {
                                    dicDoorIDs.Add(dlist[nDoorIndex], strDoorID);
                                    nDoorIndex++;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            if (!UploadDoorProperty(dicDoorIDs, strID, strKey, ref strResultMessage))
                return false;

            return true;
        }

        private bool UploadDoorProperty(Dictionary<Door, string> dicDoorIDs, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            bool flag = false;
            foreach (KeyValuePair<Door, string> item in dicDoorIDs)
            {
                Door door = item.Key;
                string strDoorID = item.Value;

                foreach (Property prop in door.Properties)
                {
                    XElement xDoor = new XElement("doorPropList");

                    XElement xType = new XElement("door_id", strDoorID);
                    XElement xCode = new XElement("property_code", GetPropertyCode(prop.Name));
                    XElement xValue = new XElement("property_value", prop.Value);

                    xDoor.Add(xType);
                    xDoor.Add(xCode);
                    xDoor.Add(xValue);

                    xRoot.Add(xDoor);
                    flag = true;
                }
            }
            if (!flag)
                return true;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/doorProp", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadDoorProperty Error : " + strResultMessage);
                strResultMessage = "UploadDoorProperty Error : " + strResultMessage;
                return false;
            }

            return true;
        }

        private bool UploadAlertAreaBoundarys(Dictionary<AlertArea, string> dicAlertAreaIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            int nBoundaryCount = 0;

            foreach (KeyValuePair<AlertArea, string> pair in dicAlertAreaIDs)
            {
                AlertArea alertArea = pair.Key;
                List<Shapes.PathItem> items = alertArea.Boundary.GetBoundary();

                // 경계구역 바운더리 추가
                foreach (PathItem path in items)
                {
                    XElement xBoundary = new XElement("alertAreaBoundaryList");

                    XElement xSpaceID = new XElement("alertarea_id", pair.Value);
                    XElement xLevelID = new XElement("level_id", strLevelID);

                    xBoundary.Add(xSpaceID);
                    xBoundary.Add(xLevelID);

                    if (path.GetDrawType() == PathItem.DrawType.Line)
                    {
                        Vertex2D vBegin, vEnd, vMiddle = null;
                        path.GetVertex(out vBegin, out vEnd, out vMiddle);

                        XElement xType = MakeElement("boundary_type", ((int)BoundartType.Line).ToString());
                        XElement xBeginX = MakeElement("begin_x", GetDoubleString(vBegin.x));
                        XElement xBeginY = MakeElement("begin_y", GetDoubleString(vBegin.y));
                        XElement xEndX = MakeElement("end_x", GetDoubleString(vEnd.x));
                        XElement xEndY = MakeElement("end_y", GetDoubleString(vEnd.y));

                        xBoundary.Add(xType);
                        xBoundary.Add(xBeginX);
                        xBoundary.Add(xBeginY);
                        xBoundary.Add(xEndX);
                        xBoundary.Add(xEndY);
                    }
                    else if (path.GetDrawType() == PathItem.DrawType.Arc)
                    {
                        Arc2D arc = (Arc2D)path.GetEArc();

                        XElement xType = MakeElement("boundary_type", ((int)BoundartType.Arc).ToString());
                        XElement xCenterX = MakeElement("begin_x", GetDoubleString(arc.GetCenter().x));
                        XElement xCenterY = MakeElement("begin_y", GetDoubleString(arc.GetCenter().y));
                        XElement xRadius = MakeElement("third_x", GetDoubleString(arc.GetRadius()));
                        XElement xBeginAngle = MakeElement("begin_angle", GetDoubleString(arc.GetBeginAngle()));
                        XElement xAngle = MakeElement("angle", GetDoubleString(arc.GetAngle()));
                        XElement xClockwise = MakeElement("clock_wise", arc.IsClockWise() ? "1" : "0");

                        xBoundary.Add(xType);
                        xBoundary.Add(xCenterX);
                        xBoundary.Add(xCenterY);
                        xBoundary.Add(xRadius);
                        xBoundary.Add(xBeginAngle);
                        xBoundary.Add(xAngle);
                        xBoundary.Add(xClockwise);
                    }
                    else if (path.GetDrawType() == PathItem.DrawType.EArc)
                    {
                        EArc2D eArc = path.GetEArc();

                        XElement xType = MakeElement("boundary_type", ((int)BoundartType.EArc).ToString());
                        XElement xTLX = MakeElement("begin_x", GetDoubleString(eArc.GetTL().x));
                        XElement xTLY = MakeElement("begin_y", GetDoubleString(eArc.GetTL().y));
                        XElement xBLX = MakeElement("end_x", GetDoubleString(eArc.GetBL().x));
                        XElement xBLY = MakeElement("end_y", GetDoubleString(eArc.GetBL().y));
                        XElement xBRX = MakeElement("third_x", GetDoubleString(eArc.GetBR().x));
                        XElement xBRY = MakeElement("third_y", GetDoubleString(eArc.GetBR().y));
                        XElement xBeginAngle = MakeElement("begin_angle", GetDoubleString(eArc.GetBeginAngle()));
                        XElement xAngle = MakeElement("angle", GetDoubleString(eArc.GetAngle()));
                        XElement xClockwise = MakeElement("clock_wise", eArc.IsClockWise() ? "1" : "0");

                        xBoundary.Add(xType);
                        xBoundary.Add(xTLX);
                        xBoundary.Add(xTLY);
                        xBoundary.Add(xBLX);
                        xBoundary.Add(xBLY);
                        xBoundary.Add(xBRX);
                        xBoundary.Add(xBRY);
                        xBoundary.Add(xBeginAngle);
                        xBoundary.Add(xAngle);
                        xBoundary.Add(xClockwise);
                    }

                    xRoot.Add(xBoundary);
                    nBoundaryCount++;
                }
            }

            if (nBoundaryCount == 0)
                return true;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/alertAreaBoundary", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadAlertAreaBoundarys Error : " + strResultMessage);
                strResultMessage = "UploadAlertAreaBoundarys Error : " + strResultMessage;
                return false;
            }

            return true;
        }

        private Dictionary<AlertArea, string> UploadAlertAreas(List<AlertArea> alertAreas, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            Dictionary<AlertArea, string> dicAlertAreaIDs = new Dictionary<AlertArea, string>();

            if (alertAreas.Count == 0)
                return dicAlertAreaIDs;

            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            foreach (AlertArea alertArea in alertAreas)
            {
                XElement xAlertArea = new XElement("alertAreaList");

                XElement xLevelID = MakeElement("level_id", strLevelID);
                XElement xName = MakeElement("alertarea_name", alertArea.Name);

                xAlertArea.Add(xLevelID);
                xAlertArea.Add(xName);

                xRoot.Add(xAlertArea);
            }

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/alertArea", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadAlertAreas Error : " + strResultMessage);
                strResultMessage = "UploadAlertAreas Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadAlertAreas Error2 : " + strResult);
                strResultMessage = "UploadAlertAreas Error2 : " + strResult;
                return null;
            }

            int nAlertAreaIndex = 0;
            int nAlertAreaCount = alertAreas.Count;

            foreach (XElement element in xml.Elements())
            {
                XElement xAlertArea = element.Name == "alertAreaList" ? element : null;

                if (xAlertArea != null)
                {
                    foreach (XElement child in xAlertArea.Elements())
                    {
                        if (child.Name == "alertarea_id")
                        {
                            if (nAlertAreaIndex >= nAlertAreaCount)
                            {
                                System.Diagnostics.Trace.WriteLine("UploadAlertAreas Error : 응답받은 AlertArea 개수가 전송한 AlertArea 개수보다 많습니다.");
                                strResultMessage = "UploadAlertAreas Error : 응답받은 AlertArea 개수가 전송한 AlertArea 개수보다 많습니다.";
                                return null;
                            }

                            dicAlertAreaIDs[alertAreas[nAlertAreaIndex++]] = child.Value;
                            break;
                        }
                    }
                }
            }

            if (nAlertAreaIndex != nAlertAreaCount)
            {
                System.Diagnostics.Trace.WriteLine("UploadSpaces Error : 응답받은 AlertArea 개수와 전송한 AlertArea 개수가 일치하지 않습니다.");
                strResultMessage = "UploadSpaces Error : 응답받은 AlertArea 개수와 전송한 AlertArea 개수가 일치하지 않습니다.";
                return null;
            }
            if (!UploadAlertAreaProperty(dicAlertAreaIDs, strID, strKey, ref strResultMessage))
                return null;

            if (!UploadAlertAreaBoundarys(dicAlertAreaIDs, strLevelID, strID, strKey, ref strResultMessage))
                return null;

            return dicAlertAreaIDs;
        }

        private bool UploadAlertAreaProperty(Dictionary<AlertArea, string> dicAlertAreaIDs, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            bool flag = false;
            foreach (KeyValuePair<AlertArea, string> item in dicAlertAreaIDs)
            {
                AlertArea alertArea = item.Key;
                string strAlertAreaID = item.Value;

                foreach (Property prop in alertArea.Properties)
                {
                    XElement xAlertArea = new XElement("alertAreaPropList");

                    XElement xType = new XElement("alertarea_id", strAlertAreaID);
                    XElement xCode = new XElement("property_code", GetPropertyCode(prop.Name));
                    XElement xValue = new XElement("property_value", prop.Value);

                    xAlertArea.Add(xType);
                    xAlertArea.Add(xCode);
                    xAlertArea.Add(xValue);

                    xRoot.Add(xAlertArea);
                    flag = true;
                }
            }
            if (!flag)
                return true;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/alertAreaProp", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadAlertAreaProperty Error : " + strResultMessage);
                strResultMessage = "UploadAlertAreaProperty Error : " + strResultMessage;
                return false;
            }

            return true;
        }

        private bool UploadSpaceBoundarys(Dictionary<Space, string> dicSpaceIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            int nBoundaryCount = 0;

            foreach (KeyValuePair<Space, string> pair in dicSpaceIDs)
            {
                Space space = pair.Key;
                List<Shapes.PathItem> items = space.BoundaryData.GetBoundary();

                // 공간 바운더리 추가
                foreach (PathItem path in items)
                {
                    XElement xBoundary = new XElement("spaceBoundaryList");

                    XElement xSpaceID = new XElement("space_id", pair.Value);
                    XElement xLevelID = new XElement("level_id", strLevelID);

                    xBoundary.Add(xSpaceID);
                    xBoundary.Add(xLevelID);

                    if (path.GetDrawType() == PathItem.DrawType.Line)
                    {
                        Vertex2D vBegin, vEnd, vMiddle = null;
                        path.GetVertex(out vBegin, out vEnd, out vMiddle);

                        XElement xType = MakeElement("boundary_type", ((int)BoundartType.Line).ToString());
                        XElement xBeginX = MakeElement("begin_x", GetDoubleString(vBegin.x));
                        XElement xBeginY = MakeElement("begin_y", GetDoubleString(vBegin.y));
                        XElement xEndX = MakeElement("end_x", GetDoubleString(vEnd.x));
                        XElement xEndY = MakeElement("end_y", GetDoubleString(vEnd.y));

                        xBoundary.Add(xType);
                        xBoundary.Add(xBeginX);
                        xBoundary.Add(xBeginY);
                        xBoundary.Add(xEndX);
                        xBoundary.Add(xEndY);
                    }
                    else if (path.GetDrawType() == PathItem.DrawType.Arc)
                    {
                        Arc2D arc = (Arc2D)path.GetEArc();

                        XElement xType = MakeElement("boundary_type", ((int)BoundartType.Arc).ToString());
                        XElement xCenterX = MakeElement("begin_x", GetDoubleString(arc.GetCenter().x));
                        XElement xCenterY = MakeElement("begin_y", GetDoubleString(arc.GetCenter().y));
                        XElement xRadius = MakeElement("third_x", GetDoubleString(arc.GetRadius()));
                        XElement xBeginAngle = MakeElement("begin_angle", GetDoubleString(arc.GetBeginAngle()));
                        XElement xAngle = MakeElement("angle", GetDoubleString(arc.GetAngle()));
                        XElement xClockwise = MakeElement("clock_wise", arc.IsClockWise() ? "1" : "0");

                        xBoundary.Add(xType);
                        xBoundary.Add(xCenterX);
                        xBoundary.Add(xCenterY);
                        xBoundary.Add(xRadius);
                        xBoundary.Add(xBeginAngle);
                        xBoundary.Add(xAngle);
                        xBoundary.Add(xClockwise);
                    }
                    else if (path.GetDrawType() == PathItem.DrawType.EArc)
                    {
                        EArc2D eArc = path.GetEArc();

                        XElement xType = MakeElement("boundary_type", ((int)BoundartType.EArc).ToString());
                        XElement xTLX = MakeElement("begin_x", GetDoubleString(eArc.GetTL().x));
                        XElement xTLY = MakeElement("begin_y", GetDoubleString(eArc.GetTL().y));
                        XElement xBLX = MakeElement("end_x", GetDoubleString(eArc.GetBL().x));
                        XElement xBLY = MakeElement("end_y", GetDoubleString(eArc.GetBL().y));
                        XElement xBRX = MakeElement("third_x", GetDoubleString(eArc.GetBR().x));
                        XElement xBRY = MakeElement("third_y", GetDoubleString(eArc.GetBR().y));
                        XElement xBeginAngle = MakeElement("begin_angle", GetDoubleString(eArc.GetBeginAngle()));
                        XElement xAngle = MakeElement("angle", GetDoubleString(eArc.GetAngle()));
                        XElement xClockwise = MakeElement("clock_wise", eArc.IsClockWise() ? "1" : "0");

                        xBoundary.Add(xType);
                        xBoundary.Add(xTLX);
                        xBoundary.Add(xTLY);
                        xBoundary.Add(xBLX);
                        xBoundary.Add(xBLY);
                        xBoundary.Add(xBRX);
                        xBoundary.Add(xBRY);
                        xBoundary.Add(xBeginAngle);
                        xBoundary.Add(xAngle);
                        xBoundary.Add(xClockwise);
                    }

                    xRoot.Add(xBoundary);
                    nBoundaryCount++;
                }
            }

            if (nBoundaryCount == 0)
                return true;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/spaceBoundary", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadSpaceBoundarys Error : " + strResultMessage);
                strResultMessage = "UploadSpaceBoundarys Error : " + strResultMessage;
                return false;
            }

            return true;
        }

        private bool UploadSpaceWallLink(Dictionary<Space, string> dicSpaceIDs, Dictionary<Wall, string> dicWallIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            string strWallID;
            int nLinkCount = 0;

            foreach (KeyValuePair<Space, string> pair in dicSpaceIDs)
            {
                Space space = pair.Key;
                int nWallIndex = 1;

                foreach (Wall wall in space.Walls)
                {
                    if (dicWallIDs.TryGetValue(wall, out strWallID) == false)
                        continue;

                    // 조회 후에 이미 존재한다면 추가 등록 필요 없음.
                    List<string> spaceWallLinkList = GetSpaceWallLink(pair.Value, strWallID, strLevelID, strID, strKey, ref strResultMessage);

                    if (spaceWallLinkList.Count() > 0)
                        continue;

                    XElement xLink = new XElement("spaceWallLinkList");

                    XElement xSpaceID = new XElement("space_id", pair.Value);
                    XElement xLevelID = new XElement("level_id", strLevelID);
                    XElement xWallID = new XElement("wall_id", strWallID);
                    XElement xWallIndex = new XElement("wall_index", nWallIndex.ToString());

                    nWallIndex++;

                    xLink.Add(xSpaceID);
                    xLink.Add(xLevelID);
                    xLink.Add(xWallID);
                    xLink.Add(xWallIndex);

                    xRoot.Add(xLink);
                    nLinkCount++;
                }
            }

            if (nLinkCount == 0)
                return true;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/spaceWall", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadSpaceWallLink Error : " + strResultMessage);
                strResultMessage = "UploadSpaceWallLink Error : " + strResultMessage;
                return false;
            }

            return true;
        }

        private List<string> GetSpaceWallLink(string strSpaceID, string strWallID, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xSpaceWallLink = new XElement("spaceWallLink");

            XElement xSpaceID = MakeElement("space_id", strSpaceID);
            XElement xWallID = MakeElement("wall_id", strWallID);
            XElement xLevelID = MakeElement("level_id", strLevelID);
            xSpaceWallLink.Add(xSpaceID);
            xSpaceWallLink.Add(xWallID);
            xSpaceWallLink.Add(xLevelID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xSpaceWallLink);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/spaceWallList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetSpaceList Error : " + strResultMessage);
                strResultMessage = "GetSpaceList Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> spaceIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "spaceWallLinkList" ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "space_id")
                        {
                            spaceIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return spaceIDs;
        }

        private Dictionary<Space, string> UploadSpaces(List<Space> spaces, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            Dictionary<Space, string> dicSpaceIDs = new Dictionary<Space, string>();

            if (spaces.Count == 0)
                return dicSpaceIDs;

            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            foreach (Space space in spaces)
            {
                XElement xSpace = new XElement("spaceList");

                XElement xLevelID = MakeElement("level_id", strLevelID);
                XElement xName = MakeElement("space_name", space.Name);

                xSpace.Add(xLevelID);
                xSpace.Add(xName);

                xRoot.Add(xSpace);
            }

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/space", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadSpaces Error : " + strResultMessage);
                strResultMessage = "UploadSpaces Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadSpaces Error2 : " + strResult);
                strResultMessage = "UploadSpaces Error2 : " + strResult;
                return null;
            }

            int nSpaceIndex = 0;
            int nSpaceCount = spaces.Count;

            foreach (XElement element in xml.Elements())
            {
                XElement xSpace = element.Name == "spaceList" ? element : null;

                if (xSpace != null)
                {
                    foreach (XElement child in xSpace.Elements())
                    {
                        if (child.Name == "space_id")
                        {
                            if (nSpaceIndex >= nSpaceCount)
                            {
                                System.Diagnostics.Trace.WriteLine("UploadSpaces Error : 응답받은 Space 개수가 전송한 Space 개수보다 많습니다.");
                                strResultMessage = "UploadSpaces Error : 응답받은 Space 개수가 전송한 Space 개수보다 많습니다.";
                                return null;
                            }

                            dicSpaceIDs[spaces[nSpaceIndex++]] = child.Value;
                            break;
                        }
                    }
                }
            }

            if (nSpaceIndex != nSpaceCount)
            {
                System.Diagnostics.Trace.WriteLine("UploadSpaces Error : 응답받은 Space 개수와 전송한 Space 개수가 일치하지 않습니다.");
                strResultMessage = "UploadSpaces Error : 응답받은 Space 개수와 전송한 Space 개수가 일치하지 않습니다.";
                return null;
            }

            if (!UploadSpaceProperty(dicSpaceIDs, strID, strKey, ref strResultMessage))
                return null;

            if (!UploadSpaceBoundarys(dicSpaceIDs, strLevelID, m_strID, m_strKey, ref strResultMessage))
                return null;

            return dicSpaceIDs;
        }

        private bool UploadSpaceProperty(Dictionary<Space, string> dicSpaceIDs, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            bool flag = false;
            foreach (KeyValuePair<Space, string> item in dicSpaceIDs)
            {
                Space space = item.Key;
                string strSpaceID = item.Value;

                foreach (Property prop in space.Properties)
                {
                    XElement xSpace = new XElement("spacePropList");

                    XElement xType = new XElement("space_id", strSpaceID);
                    XElement xCode = new XElement("property_code", GetPropertyCode(prop.Name));
                    XElement xValue = new XElement("property_value", prop.Value);

                    xSpace.Add(xType);
                    xSpace.Add(xCode);
                    xSpace.Add(xValue);

                    xRoot.Add(xSpace);
                    flag = true;
                }
            }
            if (!flag)
                return true;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/spaceProp", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadSpaceProperty Error : " + strResultMessage);
                strResultMessage = "UploadSpaceProperty Error : " + strResultMessage;
                return false;
            }

            return true;
        }

        private bool UploadWallBoundarys(Dictionary<Wall, string> dicWallIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            int nBoundaryCount = 0;

            foreach (KeyValuePair<Wall, string> pair in dicWallIDs)
            {
                Wall wall = pair.Key;
                List<Shapes.PathItem> items = wall.BoundaryData.GetBoundary();

                // 벽체 바운더리 추가
                foreach (PathItem path in items)
                {
                    XElement xBoundary = new XElement("wallBoundaryList");

                    XElement xWallID = new XElement("wall_id", pair.Value);
                    XElement xLevelID = new XElement("level_id", strLevelID);

                    xBoundary.Add(xWallID);
                    xBoundary.Add(xLevelID);

                    if (path.GetDrawType() == PathItem.DrawType.Line)
                    {
                        Vertex2D vBegin, vEnd, vMiddle = null;
                        path.GetVertex(out vBegin, out vEnd, out vMiddle);

                        XElement xType = MakeElement("boundary_type", ((int)BoundartType.Line).ToString());
                        XElement xBeginX = MakeElement("begin_x", GetDoubleString(vBegin.x));
                        XElement xBeginY = MakeElement("begin_y", GetDoubleString(vBegin.y));
                        XElement xEndX = MakeElement("end_x", GetDoubleString(vEnd.x));
                        XElement xEndY = MakeElement("end_y", GetDoubleString(vEnd.y));

                        xBoundary.Add(xType);
                        xBoundary.Add(xBeginX);
                        xBoundary.Add(xBeginY);
                        xBoundary.Add(xEndX);
                        xBoundary.Add(xEndY);
                    }
                    else if (path.GetDrawType() == PathItem.DrawType.Arc)
                    {
                        Arc2D arc = (Arc2D)path.GetEArc();

                        XElement xType = MakeElement("boundary_type", ((int)BoundartType.Arc).ToString());
                        XElement xCenterX = MakeElement("begin_x", GetDoubleString(arc.GetCenter().x));
                        XElement xCenterY = MakeElement("begin_y", GetDoubleString(arc.GetCenter().y));
                        XElement xRadius = MakeElement("third_x", GetDoubleString(arc.GetRadius()));
                        XElement xBeginAngle = MakeElement("begin_angle", GetDoubleString(arc.GetBeginAngle()));
                        XElement xAngle = MakeElement("angle", GetDoubleString(arc.GetAngle()));
                        XElement xClockwise = MakeElement("clock_wise", arc.IsClockWise() ? "1" : "0");

                        xBoundary.Add(xType);
                        xBoundary.Add(xCenterX);
                        xBoundary.Add(xCenterY);
                        xBoundary.Add(xRadius);
                        xBoundary.Add(xBeginAngle);
                        xBoundary.Add(xAngle);
                        xBoundary.Add(xClockwise);
                    }
                    else if (path.GetDrawType() == PathItem.DrawType.EArc)
                    {
                        EArc2D eArc = path.GetEArc();

                        XElement xType = MakeElement("boundary_type", ((int)BoundartType.EArc).ToString());
                        XElement xTLX = MakeElement("begin_x", GetDoubleString(eArc.GetTL().x));
                        XElement xTLY = MakeElement("begin_y", GetDoubleString(eArc.GetTL().y));
                        XElement xBLX = MakeElement("end_x", GetDoubleString(eArc.GetBL().x));
                        XElement xBLY = MakeElement("end_y", GetDoubleString(eArc.GetBL().y));
                        XElement xBRX = MakeElement("third_x", GetDoubleString(eArc.GetBR().x));
                        XElement xBRY = MakeElement("third_y", GetDoubleString(eArc.GetBR().y));
                        XElement xBeginAngle = MakeElement("begin_angle", GetDoubleString(eArc.GetBeginAngle()));
                        XElement xAngle = MakeElement("angle", GetDoubleString(eArc.GetAngle()));
                        XElement xClockwise = MakeElement("clock_wise", eArc.IsClockWise() ? "1" : "0");

                        xBoundary.Add(xType);
                        xBoundary.Add(xTLX);
                        xBoundary.Add(xTLY);
                        xBoundary.Add(xBLX);
                        xBoundary.Add(xBLY);
                        xBoundary.Add(xBRX);
                        xBoundary.Add(xBRY);
                        xBoundary.Add(xBeginAngle);
                        xBoundary.Add(xAngle);
                        xBoundary.Add(xClockwise);
                    }

                    xRoot.Add(xBoundary);
                    nBoundaryCount++;
                }
            }

            if (nBoundaryCount == 0)
                return true;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/wallBoundary", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadWallBoundarys Error : " + strResultMessage);
                strResultMessage = "UploadWallBoundarys Error : " + strResultMessage;
                return false;
            }

            return true;
        }

        private Dictionary<Wall, string> UploadWalls(Dictionary<Wall, string> dicGridIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            Dictionary<Wall, string> dicWallIDs = new Dictionary<Wall, string>();

            if (dicGridIDs.Count == 0)
                return dicWallIDs;

            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            List<Wall> walls = new List<Wall>();

            foreach (KeyValuePair<Wall, string> pair in dicGridIDs)
            {
                Wall wall = pair.Key;
                XElement xWall = new XElement("wallList");

                XElement xLevelID = MakeElement("level_id", strLevelID);
                XElement xThick = MakeElement("thick", GetDoubleString(wall.Thick));
                XElement xHeight = MakeElement("height", GetDoubleString(wall.Height));
                XElement xComponentID = MakeElement("component_id", wall.Component.WebServiceCode);
                XElement xGridID = MakeElement("grid_id", pair.Value);

                xWall.Add(xLevelID);
                xWall.Add(xThick);
                xWall.Add(xHeight);
                xWall.Add(xComponentID);
                xWall.Add(xGridID);

                xRoot.Add(xWall);
                walls.Add(wall);
            }

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/wall", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadWalls Error : " + strResultMessage);
                strResultMessage = "UploadWalls Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadWalls Error2 : " + strResult);
                strResultMessage = "UploadWalls Error2 : " + strResult;
                return null;
            }

            int nWallIndex = 0;
            int nWallCount = walls.Count;

            foreach (XElement element in xml.Elements())
            {
                XElement xWall = element.Name == "wallList" ? element : null;

                if (xWall != null)
                {
                    foreach (XElement child in xWall.Elements())
                    {
                        if (child.Name == "wall_id")
                        {
                            if (nWallIndex >= nWallCount)
                            {
                                System.Diagnostics.Trace.WriteLine("UploadWalls Error : 응답받은 Wall 개수가 전송한 Wall 개수보다 많습니다.");
                                strResultMessage = "UploadWalls Error : 응답받은 Wall 개수가 전송한 Wall 개수보다 많습니다.";
                                return null;
                            }

                            dicWallIDs[walls[nWallIndex++]] = child.Value;
                            break;
                        }
                    }
                }
            }

            if (nWallIndex != nWallCount)
            {
                System.Diagnostics.Trace.WriteLine("UploadWalls Error : 응답받은 Wall 개수와 전송한 Wall 개수가 일치하지 않습니다.");
                strResultMessage = "UploadWalls Error : 응답받은 Wall 개수와 전송한 Wall 개수가 일치하지 않습니다.";
                return null;
            }

            if (!UploadWallProperty(dicWallIDs, strID, strKey, ref strResultMessage))
                return null;

            if (!UploadWallBoundarys(dicWallIDs, strLevelID, strID, strKey, ref strResultMessage))
                return null;

            return dicWallIDs;
        }

        private bool UploadWallProperty(Dictionary<Wall, string> dicWallIDs, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            bool flag = false;
            foreach (KeyValuePair<Wall, string> item in dicWallIDs)
            {
                Wall wall = item.Key;
                string strWallID = item.Value;

                foreach (Property prop in wall.Properties)
                {
                    XElement xWall = new XElement("wallPropList");

                    XElement xType = new XElement("wall_id", strWallID);
                    XElement xCode = new XElement("property_code", GetPropertyCode(prop.Name));
                    XElement xValue = new XElement("property_value", prop.Value);

                    xWall.Add(xType);
                    xWall.Add(xCode);
                    xWall.Add(xValue);

                    xRoot.Add(xWall);
                    flag = true;
                }
            }
            if (!flag)
                return true;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/wallProp", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadWallProperty Error : " + strResultMessage);
                strResultMessage = "UploadWallProperty Error : " + strResultMessage;
                return false;
            }

            return true;
        }

        private Dictionary<Wall, string> UploadGrids(List<Wall> walls, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            int nGridCount = 0;

            foreach (Wall wall in walls)
            {
                XElement xGrid = new XElement("gridList");

                XElement xGridType = MakeElement("grid_type", ((int)wall.GetGridType()).ToString());
                xGrid.Add(xGridType);

                if (wall.GetGridType() == Wall.GridType.Line)
                {
                    if (wall.Line == null)
                        continue;

                    Vertex2D vBegin = wall.Line.GetVertex(true);
                    Vertex2D vEnd = wall.Line.GetVertex(false);

                    XElement xBeginX = MakeElement("begin_x", GetDoubleString(vBegin.x));
                    XElement xBeginY = MakeElement("begin_y", GetDoubleString(vBegin.y));
                    XElement xEndX = MakeElement("end_x", GetDoubleString(vEnd.x));
                    XElement xEndY = MakeElement("end_y", GetDoubleString(vEnd.y));

                    xGrid.Add(xBeginX);
                    xGrid.Add(xBeginY);
                    xGrid.Add(xEndX);
                    xGrid.Add(xEndY);
                }
                else if (wall.GetGridType() == Wall.GridType.Arc)
                {
                    if (wall.Arc == null)
                        continue;

                    XElement xCenterX = MakeElement("begin_x", GetDoubleString(wall.Arc.GetCenter().x));
                    XElement xCenterY = MakeElement("begin_y", GetDoubleString(wall.Arc.GetCenter().y));
                    XElement xRadius = MakeElement("third_x", GetDoubleString(wall.Arc.GetRadius()));
                    XElement xBeginAngle = MakeElement("begin_angle", GetDoubleString(wall.Arc.GetBeginAngle()));
                    XElement xAngle = MakeElement("angle", GetDoubleString(wall.Arc.GetAngle()));
                    XElement xClockwise = MakeElement("clock_wise", wall.Arc.IsClockWise() ? "1" : "0");

                    xGrid.Add(xCenterX);
                    xGrid.Add(xCenterY);
                    xGrid.Add(xRadius);
                    xGrid.Add(xBeginAngle);
                    xGrid.Add(xAngle);
                    xGrid.Add(xClockwise);
                }
                else if (wall.GetGridType() == Wall.GridType.EArc)
                {
                    if (wall.EArc == null)
                        continue;

                    XElement xTLX = MakeElement("begin_x", GetDoubleString(wall.EArc.GetTL().x));
                    XElement xTLY = MakeElement("begin_y", GetDoubleString(wall.EArc.GetTL().y));
                    XElement xBLX = MakeElement("end_x", GetDoubleString(wall.EArc.GetBL().x));
                    XElement xBLY = MakeElement("end_y", GetDoubleString(wall.EArc.GetBL().y));
                    XElement xBRX = MakeElement("third_x", GetDoubleString(wall.EArc.GetBR().x));
                    XElement xBRY = MakeElement("third_y", GetDoubleString(wall.EArc.GetBR().y));
                    XElement xBeginAngle = MakeElement("begin_angle", GetDoubleString(wall.EArc.GetBeginAngle()));
                    XElement xAngle = MakeElement("angle", GetDoubleString(wall.EArc.GetAngle()));
                    XElement xClockwise = MakeElement("clock_wise", wall.EArc.IsClockWise() ? "1" : "0");

                    xGrid.Add(xTLX);
                    xGrid.Add(xTLY);
                    xGrid.Add(xBLX);
                    xGrid.Add(xBLY);
                    xGrid.Add(xBRX);
                    xGrid.Add(xBRY);
                    xGrid.Add(xBeginAngle);
                    xGrid.Add(xAngle);
                    xGrid.Add(xClockwise);
                }

                XElement xLevelID = MakeElement("level_id", strLevelID);
                xGrid.Add(xLevelID);

                xRoot.Add(xGrid);
                nGridCount++;
            }

            Dictionary<Wall, string> dicGridIDs = new Dictionary<Wall, string>();

            if (nGridCount == 0)
                return dicGridIDs;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/grid", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadGrids Error : " + strResultMessage);
                strResultMessage = "UploadGrids Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadGrids Error2 : " + strResult);
                strResultMessage = "UploadGrids Error2 : " + strResult;
                return null;
            }

            int nWallIndex = 0;
            int nWallCount = walls.Count;

            foreach (XElement element in xml.Elements())
            {
                XElement xGrid = element.Name == "gridList" ? element : null;

                if (xGrid != null)
                {
                    foreach (XElement child in xGrid.Elements())
                    {
                        if (child.Name == "grid_id")
                        {
                            if (nWallIndex >= nWallCount)
                            {
                                System.Diagnostics.Trace.WriteLine("UploadGrids Error : Grid 개수가 Wall 개수보다 많습니다.");
                                strResultMessage = "UploadGrids Error : Grid 개수가 Wall 개수보다 많습니다.";
                                return null;
                            }

                            dicGridIDs[walls[nWallIndex++]] = child.Value;
                            break;
                        }
                    }
                }
            }

            if (nWallIndex != nWallCount)
            {
                System.Diagnostics.Trace.WriteLine("UploadGrids Error : Grid 개수와 Wall 개수가 일치하지 않습니다.");
                strResultMessage = "UploadGrids Error : Grid 개수가 Wall 개수보다 많습니다.";
                return null;
            }

            return dicGridIDs;
        }

        private Dictionary<Level, string> UploadLevels(string strBuildingID, List<Level> levels, string strID, string strKey, ref string strResultMessage)
        {
            Dictionary<Level, string> dicLevelIDs = new Dictionary<Level, string>();

            if (levels.Count == 0)
                return dicLevelIDs;

            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            foreach (Level level in levels)
            {
                XElement xLevel = new XElement("levelList");

                XElement xBuildingID = MakeElement("build_mng_no", strBuildingID);
                xLevel.Add(xBuildingID);

                XElement xElevation = MakeElement("elevation", GetDoubleString(level.Elevation));
                xLevel.Add(xElevation);

                XElement xName = MakeElement("level_name", level.Name);
                xLevel.Add(xName);

                xRoot.Add(xLevel);
            }

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/level", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadLevels Error : " + strResultMessage);
                strResultMessage = "UploadLevels Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadLevels Error2 : " + strResult);
                strResultMessage = "UploadLevels Error2 : " + strResult;
                return null;
            }

            int nLevelIndex = 0;

            foreach (XElement element in xml.Elements())
            {
                XElement xLevel = element.Name == "levelList" ? element : null;

                if (xLevel != null)
                {
                    foreach (XElement child in xLevel.Elements())
                    {
                        string strLevelID = "";

                        if (child.Name == "level_id")
                        {
                            strLevelID = child.Value;

                            if (strLevelID.Length > 0)
                            {
                                if (nLevelIndex < levels.Count)
                                {
                                    Level level = levels[nLevelIndex];

                                    dicLevelIDs[level] = strLevelID;
                                    nLevelIndex++;
                                }

                                break;
                            }
                        }
                    }
                }
            }
            if (!UploadLevelProperty(dicLevelIDs, strID, strKey, ref strResultMessage))
                return null;

            return dicLevelIDs;
        }

        private bool UploadLevelProperty(Dictionary<Level, string> dicLevelIDs, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            bool flag = false;
            foreach (KeyValuePair<Level, string> item in dicLevelIDs)
            {
                Level level = item.Key;
                string strLevelID = item.Value;

                foreach (Property prop in level.Properties)
                {
                    XElement xSpace = new XElement("levelPropList");

                    XElement xType = new XElement("level_id", strLevelID);
                    XElement xCode = new XElement("property_code", GetPropertyCode(prop.Name));
                    XElement xValue = new XElement("property_value", prop.Value);

                    xSpace.Add(xType);
                    xSpace.Add(xCode);
                    xSpace.Add(xValue);

                    xRoot.Add(xSpace);
                    flag = true;
                }
            }
            if (!flag)
                return true;

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/levelProp", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadLevelProperty Error : " + strResultMessage);
                strResultMessage = "UploadLevelProperty Error : " + strResultMessage;
                return false;
            }

            return true;
        }

        private bool RemoveLevelComponent(string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            // 벽체선형 조회
            List<string> gridList = GetGridList(strLevelID, strID, strKey, ref strResultMessage);
            if (gridList == null)
                return false;

            // 조회된 벽체선형 삭제
            if (RemoveGrids(gridList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            // 문 조회
            List<string> doorList = GetDoorList(strLevelID, strID, strKey, ref strResultMessage);
            if (doorList == null)
                return false;

            // 문 삭제 요청
            if (RemoveDoors(doorList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            // 창문 조회
            List<string> windowList = GetWindowList(strLevelID, strID, strKey, ref strResultMessage);
            if (windowList == null)
                return false;

            // 창문 삭제 요청
            if (RemoveWindows(windowList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            // 벽 조회
            List<string> wallList = GetWallList(strLevelID, strID, strKey, ref strResultMessage);
            if (wallList == null)
                return false;

            // 벽 삭제 요청
            if (RemoveWalls(wallList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            // 공간 조회
            List<string> spaceList = GetSpaceList(strLevelID, strID, strKey, ref strResultMessage);
            if (spaceList == null)
                return false;

            // 공간 삭제
            if (RemoveSpaces(spaceList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            // 경계구역 조회
            List<string> alertAreaList = GetAlertAreaList(strLevelID, strID, strKey, ref strResultMessage);
            if (alertAreaList == null)
                return false;

            // 경계구역 삭제
            if (RemoveAlertAreas(alertAreaList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            // Column 조회
            List<string> columnList = GetColumnList(strLevelID, strID, strKey, ref strResultMessage);
            if (columnList == null)
                return false;

            // Column 삭제 
            if (RemoveColumns(columnList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            // TopologyNodeLink 조회
            List<string> topologyNodeLinkList = GetTopologyNodeLinkList(strLevelID, strID, strKey, ref strResultMessage);
            if (topologyNodeLinkList == null)
                return false;

            // TopologyNodeLink 삭제 요청
            if (RemoveTopologeNodeLinks(topologyNodeLinkList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            // TopologyNode 조회
            List<string> topologyNodeList = GetTopologyNodeList(strLevelID, strID, strKey, ref strResultMessage);
            if (topologyNodeList == null)
                return false;

            // TopologyNode 삭제 요청
            if (RemoveTopologeNodes(topologyNodeList, strID, strKey, ref strResultMessage) == false)
                return false;

            // Topology 조회
            List<string> topologyList = GetTopologyList(strLevelID, strID, strKey, ref strResultMessage);
            if (topologyList == null)
                return false;

            // Topology 삭제 요청
            if (RemoveTopologes(topologyList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            // POIWire 조회
            List<string> poiWireList = GetPOIWireList(strLevelID, strID, strKey, ref strResultMessage);
            if (poiWireList == null)
                return false;

            // POIWire 삭제 요청
            if (RemovePOIWires(poiWireList, strLevelID, strID, strKey, ref strResultMessage) == false)
                return false;

            // POI 조회
            List<string> POIList = GetPOIList(m_strBuildingKey, strLevelID, strID, strKey, ref strResultMessage);

            if (POIList == null)
                return false;

            // POI 삭제 요청
            if (RemovePOIs(POIList, strID, strKey, ref strResultMessage) == false)
                return false;

            // 레벨 속성 조회
            List<string> levelPropList = GetPropIDs(strLevelID, strID, strKey, "level", ref strResultMessage);

            if (levelPropList == null)
                return false;

            // 레벨 속성 삭제
            if (!RemoveProps(strLevelID, levelPropList, strID, strKey, "level", ref strResultMessage))
                return false;

            // 최종 레벨 삭제
            if (!RemoveLevel(strLevelID, strID, strKey, ref strResultMessage))
                return false;

            return true;
        }

        private bool RemoveLevel(string levelID, string strID, string strKey, ref string strResultMessage)
        {
            string strURL = string.Format("spatial/level/{0}/{1}/{2}", strID, strKey, levelID);

            string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strResultMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveLevel Error : " + strResultMessage);
                    strResultMessage = "RemoveLevel Error : " + strResultMessage;
                    return false;
                }
            }

            return true;
        }

        private List<string> GetPOIList(string strBuildingID, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xBuildingID = MakeElement("paramBuildMngNo", strBuildingID);
            //XElement xFloorID = MakeElement("floor", strLevelID);
            XElement xPageNo = MakeElement("pageNo", "1");
            XElement xPageSize = MakeElement("pageSize", "10000");

            XElement xRoot = new XElement("convergence");
            xRoot.Add(xState);
            xRoot.Add(xBuildingID);
            xRoot.Add(xPageNo);
            xRoot.Add(xPageSize);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "convergence/fireEquipList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetPOIList Error : " + strResultMessage);
                strResultMessage = "GetPOIList Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> POIIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "equipList" ? element : null;

                if (xNode != null)
                {
                    string strFloor = "";
                    string strPOIID = "";

                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "equip_id")
                        {
                            //POIIDs.Add(child.Value);
                            //break;
                            strPOIID = child.Value;
                            continue;
                        }
                        else if (child.Name == "floor")
                        {
                            strFloor = child.Value;
                            continue;
                        }
                    }

                    if (strFloor == strLevelID)
                    {
                        POIIDs.Add(strPOIID);
                    }
                }
            }

            return POIIDs;
        }

        private bool RemovePOIs(List<string> POIIDs, string strID, string strKey, ref string strResultMessage)
        {
            foreach (string strPOIID in POIIDs)
            {
                // POI 속성 조회
                List<string> poiPropList = GetPOIPropIDs(strPOIID, strID, strKey, ref strResultMessage);

                if (poiPropList == null)
                    return false;

                // POI 속성 삭제
                if (!RemovePOIProps(strPOIID, poiPropList, strID, strKey, ref strResultMessage))
                    return false;


                if (!RemovePOI(strPOIID, strID, strKey, ref strResultMessage))
                    return false;
            }

            return true;
        }

        private bool RemovePOI(string strPOIID, string strID, string strKey, ref string strResultMessage)
        {
            string strURL = string.Format("convergence/fireEquip/{0}/{1}/{2}", strID, strKey, strPOIID);

            string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strResultMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemovePOI Error : " + strResultMessage);
                    strResultMessage = "RemovePOI Error : " + strResultMessage;
                    return false;
                }
            }

            return true;
        }

        private List<string> GetPOIPropIDs(string strPOIID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xProp = new XElement("equipProp");

            XElement xID = MakeElement("equip_id", strPOIID);
            xProp.Add(xID);

            XElement xRoot = new XElement("convergence");
            xRoot.Add(xState);
            xRoot.Add(xProp);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "convergence/fireEquipPropList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetPOIPropIDs Error : " + strResultMessage);
                strResultMessage = "GetPOIPropIDs Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> propIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == ("equipPropList") ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "property_code")
                        {
                            propIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return propIDs;
        }

        private bool RemovePOIProps(string strColumnID, List<string> propIDs, string strID, string strKey, ref string strResultMessage)
        {
            foreach (string strPropID in propIDs)
            {
                string strURL = string.Format("convergence/fireEquipProp/{0}/{1}/{2}/{3}", strID, strKey, strColumnID, strPropID);

                string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

                if (strResult.Length == 0)
                {
                    if (strResultMessage != SUCCESS_CODE)
                    {
                        System.Diagnostics.Trace.WriteLine("RemovePOIProps Error : " + strResultMessage);
                        strResultMessage = "RemovePOIProps Error : " + strResultMessage;
                        return false;
                    }
                }
            }

            return true;
        }

        private List<string> GetPOIWireList(string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xEquipWire = new XElement("equipWire");

            XElement xLevelID = MakeElement("level_id", strLevelID);
            xEquipWire.Add(xLevelID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xEquipWire);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/equipWireList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetPOIWireList Error : " + strResultMessage);
                strResultMessage = "GetPOIWireList Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> equipWireIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "equipWireList" ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "wire_id")
                        {
                            equipWireIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return equipWireIDs;
        }

        private bool RemovePOIWires(List<string> poiWireIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            foreach (string strPOIWireID in poiWireIDs)
            {
                string strURL = string.Format("spatial/equipWire/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strPOIWireID);

                string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

                if (strResult.Length == 0)
                {
                    if (strResultMessage != SUCCESS_CODE)
                    {
                        System.Diagnostics.Trace.WriteLine("RemovePOIWires Error : " + strResultMessage);
                        strResultMessage = "RemovePOIWires Error : " + strResultMessage;
                        return false;
                    }
                }
            }

            return true;
        }

        private List<string> GetTopologyList(string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xColumn = new XElement("topology");

            XElement xLevelID = MakeElement("level_id", strLevelID);
            xColumn.Add(xLevelID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xColumn);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/topologyList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetTopologyList Error : " + strResultMessage);
                strResultMessage = "GetTopologyList Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> topologyIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "topologyList" ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "topology_id")
                        {
                            topologyIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return topologyIDs;
        }

        private bool RemoveTopologes(List<string> topologyIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            foreach (string strTopologyID in topologyIDs)
            {
                string strURL = string.Format("spatial/topology/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strTopologyID);

                string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

                if (strResult.Length == 0)
                {
                    if (strResultMessage != SUCCESS_CODE)
                    {
                        System.Diagnostics.Trace.WriteLine("RemoveTopologes Error : " + strResultMessage);
                        strResultMessage = "RemoveTopologes Error : " + strResultMessage;
                        return false;
                    }
                }
            }

            return true;
        }

        private List<string> GetTopologyNodeList(string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xColumn = new XElement("topologyNode");

            XElement xLevelID = MakeElement("level_id", strLevelID);
            xColumn.Add(xLevelID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xColumn);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/topologyNodeList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetTopologyNodeList Error : " + strResultMessage);
                strResultMessage = "GetTopologyNodeList Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> topologyNodeIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "topologyNodeList" ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "node_id")
                        {
                            topologyNodeIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return topologyNodeIDs;
        }

        private bool RemoveTopologeNodes(List<string> topologyNodeIDs, string strID, string strKey, ref string strResultMessage)
        {
            foreach (string strTopologyID in topologyNodeIDs)
            {
                string strURL = string.Format("spatial/topologyNode/{0}/{1}/{2}", strID, strKey, strTopologyID);

                string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

                if (strResult.Length == 0)
                {
                    if (strResultMessage != SUCCESS_CODE)
                    {
                        System.Diagnostics.Trace.WriteLine("RemoveTopologeNodes Error : " + strResultMessage);
                        strResultMessage = "RemoveTopologeNodes Error : " + strResultMessage;
                        return false;
                    }
                }
            }

            return true;
        }

        private List<string> GetTopologyNodeLinkList(string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xColumn = new XElement("topologyNodeLink");

            XElement xLevelID = MakeElement("level_id", strLevelID);
            xColumn.Add(xLevelID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xColumn);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/topologyNodeLinkList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetTopologyNodeLinkList Error : " + strResultMessage);
                strResultMessage = "GetTopologyNodeLinkList Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> topologyNodeLinkIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "topologyNodeLinkList" ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "link_node_id")
                        {
                            topologyNodeLinkIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return topologyNodeLinkIDs;
        }

        private bool RemoveTopologeNodeLinks(List<string> topologyNodeLinkIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            foreach (string strTopologyNodeLinkID in topologyNodeLinkIDs)
            {
                string strURL = string.Format("spatial/topologyNodeLink/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strTopologyNodeLinkID);

                string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

                if (strResult.Length == 0)
                {
                    if (strResultMessage != SUCCESS_CODE)
                    {
                        System.Diagnostics.Trace.WriteLine("RemoveTopologeNodeLinks Error : " + strResultMessage);
                        strResultMessage = "RemoveTopologeNodeLinks Error : " + strResultMessage;
                        return false;
                    }
                }
            }

            return true;
        }

        private List<string> GetColumnList(string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xColumn = new XElement("column");

            XElement xLevelID = MakeElement("level_id", strLevelID);
            xColumn.Add(xLevelID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xColumn);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/columnList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetColumnList Error : " + strResultMessage);
                strResultMessage = "GetColumnList Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> columnIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "columnList" ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "column_id")
                        {
                            columnIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return columnIDs;
        }

        private bool RemoveColumns(List<string> columnIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            foreach (string strColumnID in columnIDs)
            {
                // 컬럼 속성 조회
                List<string> columnPropList = GetPropIDs(strColumnID, strID, strKey, "column", ref strResultMessage);

                if (columnPropList == null)
                    return false;

                // 컬럼 속성 삭제
                if (!RemoveProps(strColumnID, columnPropList, strID, strKey, "column", ref strResultMessage))
                    return false;

                if (!RemoveColumn(strColumnID, strLevelID, strID, strKey, ref strResultMessage))
                    return false;
            }

            return true;
        }

        private bool RemoveColumn(string strColumnID, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            string strURL = string.Format("spatial/column/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strColumnID);

            string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strResultMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveColumn Error : " + strResultMessage);
                    strResultMessage = "RemoveColumn Error : " + strResultMessage;
                    return false;
                }
            }

            return true;
        }

        private List<string> GetAlertAreaList(string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xAlertArea = new XElement("alertArea");

            XElement xLevelID = MakeElement("level_id", strLevelID);
            xAlertArea.Add(xLevelID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xAlertArea);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/alertAreaList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetAlertAreaList Error : " + strResultMessage);
                strResultMessage = "GetAlertAreaList Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> alertAreaIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "alertAreaList" ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "alertarea_id")
                        {
                            alertAreaIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return alertAreaIDs;
        }

        private bool RemoveAlertAreas(List<string> alertAreaIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            foreach (string strAlertID in alertAreaIDs)
            {
                // 경계구역 속성 조회
                List<string> alertAreaPropList = GetPropIDs(strAlertID, strID, strKey, "alertArea", ref strResultMessage);

                if (alertAreaPropList == null)
                    return false;

                // 경계구역 속성 삭제
                if (!RemoveProps(strAlertID, alertAreaPropList, strID, strKey, "alertArea", ref strResultMessage))
                {
                    Console.WriteLine("RemoveProps error " + strResultMessage);
                    //return false;
                }
                    

                // 경계구역 바운더리 삭제
                // 경계구역 바운더리 조회
                List<string> checkIDs = GetAlertAreaBoundaryList(strAlertID, strID, strKey, ref strResultMessage);

                // 조회가 될 경우 삭제
                if (checkIDs.Count > 0)
                {
                    if (!RemoveAlertAreaBoundary(strAlertID, strLevelID, strID, strKey, ref strResultMessage))
                        return false;
                }

                if (!RemoveAlertArea(strAlertID, strLevelID, strID, strKey, ref strResultMessage))
                    return false;
            }

            return true;
        }

        private bool RemoveAlertArea(string strAlertAreaID, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            string strURL = string.Format("spatial/alertArea/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strAlertAreaID);

            string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strResultMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveAlertArea Error : " + strResultMessage);
                    strResultMessage = "RemoveAlertArea Error : " + strResultMessage;
                    return false;
                }
            }

            return true;
        }

        private List<string> GetAlertAreaBoundaryList(string strSpaceID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xAlertAreaBoundary = new XElement("alertAreaBoundary");

            XElement xAlertAreaID = MakeElement("alertarea_id", strSpaceID);
            xAlertAreaBoundary.Add(xAlertAreaID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xAlertAreaBoundary);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/alertAreaBoundaryList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetAlertAreaBoundaryList Error : " + strResultMessage);
                strResultMessage = "GetAlertAreaBoundaryList Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> alertAreaIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "alertAreaBoundaryList" ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "alertarea_id")
                        {
                            alertAreaIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return alertAreaIDs;
        }

        private bool RemoveAlertAreaBoundary(string strAlertAreaID, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            string strURL = string.Format("spatial/alertAreaBoundary/{0}/{1}/{2}", strID, strKey, strAlertAreaID);

            string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strResultMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveAlertAreaBoundary Error : " + strResultMessage);
                    strResultMessage = "RemoveAlertAreaBoundary Error : " + strResultMessage;
                    return false;
                }
            }

            return true;
        }

        private List<string> GetSpaceList(string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xSpace = new XElement("space");

            XElement xLevelID = MakeElement("level_id", strLevelID);
            xSpace.Add(xLevelID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xSpace);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/spaceList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetSpaceList Error : " + strResultMessage);
                strResultMessage = "GetSpaceList Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> spaceIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "spaceList" ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "space_id")
                        {
                            spaceIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return spaceIDs;
        }

        private bool RemoveSpaces(List<string> spaceIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            foreach (string strSpaceID in spaceIDs)
            {
                // 공간 속성 조회
                List<string> spacePropList = GetPropIDs(strSpaceID, strID, strKey, "space", ref strResultMessage);

                if (spacePropList == null)
                    return false;

                // 공간 속성 삭제
                if (!RemoveProps(strSpaceID, spacePropList, strID, strKey, "space", ref strResultMessage))
                    return false;

                // 공간 바운더리 삭제
                // 공간 바운더리 조회
                List<string> checkIDs = GetSpaceBoundaryList(strSpaceID, strID, strKey, ref strResultMessage);
                if (checkIDs == null)
                    return false;

                // 조회가 될 경우 삭제
                if (checkIDs.Count > 0)
                {
                    if (!RemoveSpaceBoundary(strSpaceID, strLevelID, strID, strKey, ref strResultMessage))
                        return false;
                }

                // TODO: 홀 바운더리 삭제

                if (!RemoveSpace(strSpaceID, strLevelID, strID, strKey, ref strResultMessage))
                    return false;
            }

            return true;
        }

        private bool RemoveSpace(string strSpaceID, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            string strURL = string.Format("spatial/space/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strSpaceID);

            string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strResultMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveSpace Error : " + strResultMessage);
                    strResultMessage = "RemoveSpace Error : " + strResultMessage;
                    return false;
                }
            }

            return true;
        }

        private List<string> GetSpaceBoundaryList(string strSpaceID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xSpaceBoundary = new XElement("spaceBoundary");

            XElement xSpaceID = MakeElement("space_id", strSpaceID);
            xSpaceBoundary.Add(xSpaceID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xSpaceBoundary);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/spaceBoundaryList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetSpaceBoundaryList Error : " + strResultMessage);
                strResultMessage = "GetSpaceBoundaryList Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> spaceIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "spaceBoundaryList" ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "space_id")
                        {
                            spaceIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return spaceIDs;
        }

        private bool RemoveSpaceBoundary(string strSpaceID, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            string strURL = string.Format("spatial/spaceBoundary/{0}/{1}/{2}", strID, strKey, strSpaceID);

            string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strResultMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveSpaceBoundary Error : " + strResultMessage);
                    strResultMessage = "RemoveSpaceBoundary Error : " + strResultMessage;
                    return false;
                }
            }

            return true;
        }

        private List<string> GetWallList(string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xWall = new XElement("wall");

            XElement xLevelID = MakeElement("level_id", strLevelID);
            xWall.Add(xLevelID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xWall);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/wallList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetWallList Error : " + strResultMessage);
                strResultMessage = "GetWallList Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> wallIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "wallList" ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "wall_id")
                        {
                            wallIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return wallIDs;
        }

        private bool RemoveWalls(List<string> wallIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            foreach (string strWallID in wallIDs)
            {
                // 벽 속성 조회
                List<string> wallPropList = GetPropIDs(strWallID, strID, strKey, "wall", ref strResultMessage);

                if (wallPropList == null)
                    return false;

                // 벽 속성 삭제
                if (!RemoveProps(strWallID, wallPropList, strID, strKey, "wall", ref strResultMessage))
                    return false;

                // 벽체 바운더리 조회  GetWallBoundaryList
                List<string> checkIDs = GetWallBoundaryList(strWallID, strID, strKey, ref strResultMessage);
                if (checkIDs == null)
                    return false;

                // 조회가 될 경우 삭제
                if (checkIDs.Count > 0)
                {
                    if (!RemoveWallBoundary(strWallID, strLevelID, strID, strKey, ref strResultMessage))
                        return false;
                }

                if (!RemoveWall(strWallID, strLevelID, strID, strKey, ref strResultMessage))
                    return false;
            }

            return true;
        }

        private bool RemoveWall(string strWallID, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            string strURL = string.Format("spatial/wall/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strWallID);

            string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strResultMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveWall Error : " + strResultMessage);
                    strResultMessage = "RemoveWall Error : " + strResultMessage;
                    return false;
                }
            }

            return true;
        }

        private List<string> GetWallBoundaryList(string strWallID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xWallBoundary = new XElement("wallBoundary");

            XElement xWallID = MakeElement("wall_id", strWallID);
            xWallBoundary.Add(xWallID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xWallBoundary);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/wallBoundaryList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetWallBoundaryList Error : " + strResultMessage);
                strResultMessage = "GetWallBoundaryList Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> wallIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "wallBoundaryList" ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "wall_id")
                        {
                            wallIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return wallIDs;
        }

        private bool RemoveWallBoundary(string strWallID, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            string strURL = string.Format("spatial/wallBoundary/{0}/{1}/{2}", strID, strKey, strWallID);

            string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strResultMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveWallBoundary Error : " + strResultMessage);
                    strResultMessage = "RemoveWallBoundary Error : " + strResultMessage;
                    return false;
                }
            }

            return true;
        }

        private List<string> GetWindowList(string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xWindow = new XElement("window");

            XElement xLevelID = MakeElement("level_id", strLevelID);
            xWindow.Add(xLevelID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xWindow);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/windowList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetWindowList Error : " + strResultMessage);
                strResultMessage = "GetWindowList Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> windowIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "windowList" ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "window_id")
                        {
                            windowIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return windowIDs;
        }

        private bool RemoveWindows(List<string> windowIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            foreach (string strWindowID in windowIDs)
            {
                // 창문 속성 조회
                List<string> windowPropList = GetPropIDs(strWindowID, strID, strKey, "window", ref strResultMessage);

                if (windowPropList == null)
                    return false;

                // 창문 속성 삭제
                if (!RemoveProps(strWindowID, windowPropList, strID, strKey, "window", ref strResultMessage))
                    return false;

                if (!RemoveWindow(strWindowID, strLevelID, strID, strKey, ref strResultMessage))
                    return false;
            }

            return true;
        }

        private bool RemoveWindow(string strWindowID, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            string strURL = string.Format("spatial/window/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strWindowID);

            string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strResultMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveWindow Error : " + strResultMessage);
                    strResultMessage = "RemoveWindow Error : " + strResultMessage;
                    return false;
                }
            }

            return true;
        }

        private List<string> GetDoorList(string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xDoor = new XElement("door");

            XElement xLevelID = MakeElement("level_id", strLevelID);
            xDoor.Add(xLevelID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xDoor);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/doorList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetDoorList Error : " + strResultMessage);
                strResultMessage = "GetDoorList Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> doorIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "doorList" ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "door_id")
                        {
                            doorIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return doorIDs;
        }

        private bool RemoveDoors(List<string> doorIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            foreach (string strDoorID in doorIDs)
            {
                // 문 속성 조회
                List<string> doorPropList = GetPropIDs(strDoorID, strID, strKey, "door", ref strResultMessage);

                if (doorPropList == null)
                    return false;

                // 문 속성 삭제
                if (!RemoveProps(strDoorID, doorPropList, strID, strKey, "door", ref strResultMessage))
                    return false;


                if (!RemoveDoor(strDoorID, strLevelID, strID, strKey, ref strResultMessage))
                    return false;
            }

            return true;
        }

        private bool RemoveDoor(string strDoorID, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            string strURL = string.Format("spatial/door/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strDoorID);

            string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strResultMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveDoor Error : " + strResultMessage);
                    strResultMessage = "RemoveDoor Error : " + strResultMessage;
                    return false;
                }
            }

            return true;
        }

        private List<string> GetGridList(string strLevelID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xGrid = new XElement("grid");

            XElement xLevelID = MakeElement("level_id", strLevelID);
            xGrid.Add(xLevelID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xGrid);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/gridList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetGridList Error : " + strResultMessage);
                strResultMessage = "GetGridList Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> gridIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "gridList" ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "grid_id")
                        {
                            gridIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return gridIDs;
        }

        private bool RemoveGrids(List<string> gridIDs, string strLevelID, string strID, string strKey, ref string strResultMessage)
        {

            foreach (string strGridID in gridIDs)
            {
                string strURL = string.Format("spatial/grid/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strGridID);

                string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

                if (strResult.Length == 0)
                {
                    if (strResultMessage != SUCCESS_CODE)
                    {
                        System.Diagnostics.Trace.WriteLine("RemoveGrids Error : " + strResultMessage);
                        strResultMessage = "RemoveGrids Error : " + strResultMessage;
                        return false;
                    }
                }
            }

            return true;
        }

        private Dictionary<string, string> ReadLevelNames(string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xLevel = new XElement("level");

            XElement xBuildingID = MakeElement("build_mng_no", m_strBuildingKey);
            xLevel.Add(xBuildingID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xLevel);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/levelList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("ReadLevels Error : " + strResultMessage);
                strResultMessage = "ReadLevels Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> levelIDs = new List<string>();
            Dictionary<string, string> dicLevels = new Dictionary<string, string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "levelList" ? element : null;
                string strLevelName = "";
                string strLevelID = "";

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "level_id")
                        {
                            strLevelID = child.Value;
                        }

                        if (child.Name == "level_name")
                        {
                            strLevelName = child.Value;
                        }
                    }

                    dicLevels[strLevelID] = strLevelName;
                }
            }

            return dicLevels;
        }

        private bool UploaBuildingProperty(List<Property> properties, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            // 해당 컬럼 속성 조회
            List<string> columnPropList = GetBuildingPropIDs(m_strBuildingKey, strID, strKey, ref strResultMessage);

            if (columnPropList == null)
                return false;

            // 조회된 속성 삭제
            if (!RemoveProps(m_strBuildingKey, columnPropList, strID, strKey, "building", ref strResultMessage))
                return false;

            XElement xColumn = new XElement("buildingPropList");
            XElement xType = new XElement("build_mng_no", m_strBuildingKey);
            XElement xCode = new XElement("property_code", GetPropertyCode("건물ID"));
            XElement xValue = new XElement("property_value", m_strBuildingKey);

            xColumn.Add(xType);
            xColumn.Add(xCode);
            xColumn.Add(xValue);

            xRoot.Add(xColumn);

            // 현재 컬럼 속성 등록
            foreach (Property prop in properties)
            {
                if (prop.Name == "건물ID")
                    continue;

                xColumn = new XElement("buildingPropList");
                xType = new XElement("build_mng_no", m_strBuildingKey);
                xCode = new XElement("property_code", GetPropertyCode(prop.Name));
                xValue = new XElement("property_value", prop.Value);

                xColumn.Add(xType);
                xColumn.Add(xCode);
                xColumn.Add(xValue);

                xRoot.Add(xColumn);
            }

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/buildingProp", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploaBuildingProperty Error : " + strResultMessage);
                strResultMessage = "UploaBuildingProperty Error : " + strResultMessage;
                return false;
            }

            return true;
        }

        private List<string> GetPropIDs(string strColumnID, string strID, string strKey, string strPropName, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xProp = new XElement(strPropName + "Prop");

            XElement xID = MakeElement(strPropName + "_id", strColumnID);

            // 경계구역 예외처리
            if (strPropName == "alertArea")
                xID = MakeElement("alertarea_id", strColumnID);
            
            
            xProp.Add(xID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xProp);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/" + strPropName + "PropList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine(strPropName + "GetProp Error : " + strResultMessage);
                strResultMessage = strPropName + "GetProp Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> propIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == (strPropName + "PropList") ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "property_code")
                        {
                            propIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return propIDs;
        }

        private bool RemoveProps(string strColumnID, List<string> propIDs, string strID, string strKey, string propName, ref string strResultMessage)
        {
            Dictionary<string, string> dicPropIDs = new Dictionary<string, string>();

            // 중복 방지
            foreach (string strPropID in propIDs)
            {
                dicPropIDs[strPropID] = strPropID;
            }

            foreach (KeyValuePair<string, string> pair in dicPropIDs)
            {
                string strPropID = pair.Value;

                string strURL = string.Format("spatial/" + propName + "Prop/{0}/{1}/{2}/{3}", strID, strKey, strColumnID, strPropID);

                string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

                if (strResult.Length == 0)
                {
                    if (strResultMessage != SUCCESS_CODE)
                    {
                        System.Diagnostics.Trace.WriteLine(propName + "RemoveProps Error : " + strResultMessage);
                        strResultMessage = propName + "RemoveProps Error : " + strResultMessage;

                        // .TODO: 경계구역 속성 제거 에러
                        return false;
                        //continue;
                    }
                }
            }

            return true;
        }

        private List<string> GetBuildingPropIDs(string strBuildingID, string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xProp = new XElement("buildingProp");

            XElement xID = MakeElement("build_mng_no", strBuildingID);
            xProp.Add(xID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xProp);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/buildingPropList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetBuildingPropIDs Error : " + strResultMessage);
                strResultMessage = "GetBuildingPropIDs Error : " + strResultMessage;
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> propIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == ("buildingPropList") ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "property_code")
                        {
                            propIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return propIDs;
        }

        private bool UploaBuildingAnchorNode(AnchorNode anchor, string strID, string strKey, ref string strResultMessage)
        {
            // 삭제 이전에 조회 후에 있을 경우
            string strChkNo = CheckAnchorNode(strID, strKey, ref strResultMessage);

            if (strChkNo == null)
                return false;
            else if (strChkNo != "")
            {
                // 앵커노드 속성 조회
                List<string> anchorNodePropList = GetAnchorNodePropIDs(m_strBuildingKey, strID, strKey);

                if (anchorNodePropList == null)
                    return false;

                // 조회된 속성 삭제
                if (!RemoveProps(m_strBuildingKey, anchorNodePropList, strID, strKey, "anchorNode", ref strResultMessage))
                    return false;

                // 기존의 앵커노드 삭제
                if (!RemoveAnchorNode(strID, strKey, ref strResultMessage))
                    return false;
            }

            if (anchor.Global == null || anchor.Local == null)
                return true;

            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            XElement xAnchorNode = new XElement("buildingAnchorNodeList");

            XElement xBuildNo = new XElement("build_mng_no", m_strBuildingKey);
            XElement xGlobalX = new XElement("global_x", GetDoubleString(anchor.Global.Position.x));
            XElement xGlobalY = new XElement("global_y", GetDoubleString(anchor.Global.Position.y));
            XElement xLocalX = new XElement("local_x", GetDoubleString(anchor.Local.Position.x));
            XElement xLocalY = new XElement("local_y", GetDoubleString(anchor.Local.Position.y));
            XElement xAngle = new XElement("angle", GetDoubleString(anchor.Local.Angle));
            XElement xUnitOfLength = new XElement("unitoflength", (int)anchor.Global.Unit);

            xAnchorNode.Add(xBuildNo);
            xAnchorNode.Add(xGlobalX);
            xAnchorNode.Add(xGlobalY);
            xAnchorNode.Add(xLocalX);
            xAnchorNode.Add(xLocalY);
            xAnchorNode.Add(xAngle);
            xAnchorNode.Add(xUnitOfLength);

            xRoot.Add(xAnchorNode);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/buildingAnchorNode", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploaBuildingAnchorNode Error : " + strResultMessage);
                strResultMessage = "UploaBuildingAnchorNode Error : " + strResultMessage;
                return false;
            }

            // 앵커노드 속성 업로드
            if (!UploadAnchorNodeProperty(anchor.Properties, m_strBuildingKey, strID, strKey))
                return false;

            return true;
        }

        private bool UploadAnchorNodeProperty(List<Property> properties, string strBuildingID, string strID, string strKey)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            bool flag = false;

            foreach (Property prop in properties)
            {
                XElement xSpace = new XElement("anchorNodePropList");

                XElement xType = new XElement("build_mng_no", strBuildingID);
                XElement xCode = new XElement("property_code", GetPropertyCode(prop.Name));
                XElement xValue = new XElement("property_value", prop.Value);

                xSpace.Add(xType);
                xSpace.Add(xCode);
                xSpace.Add(xValue);

                xRoot.Add(xSpace);
                flag = true;
            }

            if (!flag)
                return true;

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/anchorNodeProp", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadAnchorNodeProperty Error : " + strErrorMessage);
                return false;
            }

            return true;
        }

        private List<string> GetAnchorNodePropIDs(string strBuildingID, string strID, string strKey)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xProp = new XElement("anchorNodeProp");

            XElement xID = MakeElement("build_mng_no", strBuildingID);
            xProp.Add(xID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xProp);

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/anchorNodePropList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetAnchorNodePropIDs Error : " + strErrorMessage);
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            List<string> propIDs = new List<string>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == ("anchorNodePropList") ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "property_code")
                        {
                            propIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return propIDs;
        }

        private string CheckAnchorNode(string strID, string strKey, ref string strResultMessage)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xAnchorNode = new XElement("buildingAnchorNode");

            XElement xID = MakeElement("build_mng_no", m_strBuildingKey);
            xAnchorNode.Add(xID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xAnchorNode);

            strResultMessage = "";
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/buildingAnchorNodeList", true, out strResultMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("CheckAnchorNode Error : " + strResultMessage);
                strResultMessage = "CheckAnchorNode Error : " + strResultMessage;
                return null;
            }
                

            XElement xml = XElement.Parse(strResult);
            string strBuildingNo = "";

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == ("buildingAnchorNodeList") ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "build_mng_no")
                        {
                            strBuildingNo = child.Value;
                        }
                    }
                }
            }

            return strBuildingNo;
        }

        private bool RemoveAnchorNode(string strID, string strKey, ref string strResultMessage)
        {
            string strURL = string.Format("spatial/buildingAnchorNode/{0}/{1}/{2}", strID, strKey, m_strBuildingKey);

            string strResult = SendQuery(null, strURL, true, out strResultMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strResultMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveAnchorNode Error : " + strResultMessage);
                    strResultMessage = "RemoveAnchorNode Error : " + strResultMessage;
                    return false;
                }
            }

            return true;
        }

        private string GetDoubleString(double data)
        {
            return string.Format("{0:" + m_strDoubleFormat + "}", data);
        }

        private void SetDoubleString(Project project)
        {
            if (project.Unit == Project.UnitOfLength.MM)
                m_strDoubleFormat = "F0";
            else if (project.Unit == Project.UnitOfLength.CM)
                m_strDoubleFormat = "F1";
            else if (project.Unit == Project.UnitOfLength.M)
                m_strDoubleFormat = "F3";
        }

        private string GetProjectID(Project project)
        {
            string retProjectID = "";
            List<Property> properties = project.Properties;

            foreach (Property prop in properties)
            {
                string strPropName = prop.Name;

                if (strPropName == "건물ID")
                {
                    retProjectID = prop.Value;
                    return retProjectID;
                }
            }

            return null;
        }

        private bool GetGrpCodeIDs(Dictionary<string, string> dicPropTable, ref string strResultMessage)
        {
            strResultMessage = "";

            XElement xUserID = MakeElement("user_id", m_strID);
            XElement xKeyID = MakeElement("key_id", m_strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xGrpCode = MakeElement("paramGrpCode", "PROP_TYP");

            XElement xRoot = new XElement("common");
            xRoot.Add(xState);
            xRoot.Add(xGrpCode);

            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "code/sysCodeList", true, out strResultMessage);

            if (strResult.Length == 0)
                return false;

            XElement xml = XElement.Parse(strResult);

            dicPropTable.Clear();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == ("listCodeSys") ? element : null;

                if (xNode != null)
                {
                    string strCode = "";
                    string strTitle = "";

                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "sys_code")
                        {
                            strCode = child.Value;
                        }

                        if (child.Name == "sys_title")
                        {
                            strTitle = child.Value;
                        }
                    }

                    dicPropTable[strTitle] = strCode;
                }
            }

            return true;
        }

        public bool Login(string strID, string strPW, ref string strResultMessage)
        {
            m_strID = strID;
            m_strPW = strPW;

            string resResult = string.Empty;
            string strURL = BaseAddress + API + "/login";

            string strXML = XML_HEADER;
            strXML += "<login>";
            strXML += "<user_id>" + strID + "</user_id>";
            strXML += "<user_pwd>" + strPW + "</user_pwd>";
            strXML += "</login>";

            byte[] bytes = Encoding.UTF8.GetBytes(strXML);
            int len = bytes.Count();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL));
                request.Method = "POST";
                request.ContentType = "application/xml; charset=utf-8";
                request.ContentLength = len + 3;

                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);
                writer.Write(strXML);
                writer.Close();

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                return CheckLoginResult(resResult, out m_strID, out m_strKey, ref strResultMessage);
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("Login Fail : " + ex.Message);
                strResultMessage = "Login Fail : " + ex.Message;
            }
            catch (Exception e)
            {
                strResultMessage = "Login Fail : " + e.Message;
            }

            return false;
        }

        public bool Login(ref string strResultMessage)
        {   // m_strID, m_strPW 정보는 위에 Login 함수에서 저장

            //            m_strID = m_strPW = m_strKey = "";

            //#if UNE
            //            m_strID = "user_une";
            //            m_strPW = "9449966Ab";
            //#else
            //            m_strID = "user_spatial";
            //            m_strPW = "spatial1234";
            //#endif

            string resResult = string.Empty;
            string strURL = BaseAddress + API + "/login";

            string strXML = XML_HEADER;
            strXML += "<login>";
            strXML += "<user_id>" + m_strID + "</user_id>";
            strXML += "<user_pwd>" + m_strPW + "</user_pwd>";
            strXML += "</login>";

            byte[] bytes = Encoding.UTF8.GetBytes(strXML);
            int len = bytes.Count();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL));
                request.Method = "POST";
                request.ContentType = "application/xml; charset=utf-8";
                request.ContentLength = len + 3;

                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);
                writer.Write(strXML);
                writer.Close();

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                return CheckLoginResult(resResult, out m_strID, out m_strKey, ref strResultMessage);
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("Login Fail : " + ex.Message);
                strResultMessage = "Login Fail : " + ex.Message;
            }
            catch (Exception e)
            {
                strResultMessage = "Login Fail : " + e.Message;
            }

            return false;
        }

        private bool CheckLoginResult(string strXML, out string strID, out string strKey, ref string strResultMessage)
        {
            strID = strKey = "";

            if (strXML.Length == 0)
                return false;

            if (strXML.StartsWith("<") == false)
                return false;

            XElement xml = XElement.Parse(strXML);

            XElement id = FindElement(xml, "user_id");
            XElement key = FindElement(xml, "key_id");
            XElement code = FindElement(xml, "rsCode");

            if (id == null || key == null || code == null)
            {
                strResultMessage = "Response 값 오류, id, key, code 값이 제대로 반환되지 않음";
                return false;
            }

            if (ResultCode.SetCodeType(code.Value) != ResultCode.CodeType.Success)
            {
                strResultMessage = "로그인 실패, Success 코드가 반환되지 않음";
                return false;
            }
                
            strID = id.Value;
            strKey = key.Value;

            return true;
        }

        private XElement FindElement(XElement node, string strNodeName)
        {
            if (node.Name == strNodeName)
                return node;

            foreach (XElement element in node.Elements())
            {
                XElement _element = FindElement(element, strNodeName);

                if (_element != null)
                    return _element;
            }

            return null;
        }

        private XElement MakeElement(string strElementName, string strValue)
        {
            XElement x = new XElement(strElementName);
            x.SetValue(strValue);
            return x;
        }

        private string SendQuery(string strXML, string strURL, bool noCodeCheck, out string strErrorMessage, string strMethodType = "POST")
        {
            strErrorMessage = "";
            string url = BaseAddress + API;

            if (strURL.StartsWith("/"))
                url += strURL;
            else
                url += "/" + strURL;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Method = strMethodType;

            // XML 다운로드 초과 에러 문제
            request.Timeout *= 60;

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

                if (strResult.StartsWith("<") == false)
                {
                    // 응답코드 형식이 REST API 번호, 응답코드(TP4_073, RS101)식으로 변하여 , 기준으로 응답코드만 추출
                    string strCode = strResult;
                    int nIndex = strCode.IndexOf(",");
                    strCode = strCode.Substring(nIndex + 1);
                    strCode.Trim();

                    // 공백 제거 >> Trim()가 제대로 동작하지 않을 시
                    if (strCode[0] == ' ')
                        strCode = strCode.Substring(1);

                    strErrorMessage = strCode;
                    return "";
                }

                if (noCodeCheck)
                    return strResult;

                XElement xml = XElement.Parse(strResult);
                XElement code = FindElement(xml, "rsCode");

                if (code == null)
                {
                    strErrorMessage = strResult;
                    return "";
                }

                ResultCode.CodeType result = ResultCode.SetCodeType(code.Value);

                if (result == ResultCode.CodeType.Success)
                {
                    return strResult;
                }
                else if (result == ResultCode.CodeType.NoAuthority)
                {
                    strErrorMessage = "권한 없음";
                    return "";
                }
                else if (result == ResultCode.CodeType.NoUser)
                {
                    strErrorMessage = "사용자 없음";
                    return "";
                }
                else if (result == ResultCode.CodeType.DeletedUser)
                {
                    strErrorMessage = "삭제된 사용자";
                    return "";
                }
                else if (result == ResultCode.CodeType.InvalidParameter)
                {
                    strErrorMessage = "잘못된 전달인자";
                    return "";
                }
                else
                {
                    strErrorMessage = strResult;
                }
            }
            catch (System.Net.WebException ex)
            {
                strErrorMessage = ex.Message;
            }

            return "";
        }

        private string GetPropertyCode(string strPropertyName)
        {
            // 예외처리
            if (strPropertyName == "AlertAreaType")
                strPropertyName = "alertAreaType";

            //foreach (KeyValuePair<string, string> pair in m_strPropTable)
            //{
            //    if (pair.Key == strPropertyName)
            //        return pair.Value;
            //}
            if (m_strPropTable.ContainsKey(strPropertyName))
                return m_strPropTable[strPropertyName];
            else
                return "PROP_001";
        }
    }

    public class ResultCode
    {
        public enum CodeType { Success = 0, NoAuthority, NoUser, DeletedUser, InvalidParameter, Unknown };

        private CodeType m_type = CodeType.Unknown;

        public ResultCode(string strCode)
        {
            m_type = SetCodeType(strCode);
        }

        public CodeType GetCodeType()
        {
            return m_type;
        }

        public static CodeType SetCodeType(string strCode)
        {
            if (strCode == "RS101")
                return CodeType.Success;
            else if (strCode == "RS301")
                return CodeType.NoAuthority;
            else if (strCode == "RS401")
                return CodeType.NoUser;
            else if (strCode == "RS402")
                return CodeType.DeletedUser;
            else if (strCode == "RS700")
                return CodeType.InvalidParameter;

            return CodeType.Unknown;
        }
    }
}
