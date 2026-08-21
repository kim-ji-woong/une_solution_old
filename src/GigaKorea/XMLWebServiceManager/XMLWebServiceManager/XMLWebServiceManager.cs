using System;
using System.Collections.Generic;
using System.IO;
using XMLWebServiceManager.BIM;
using XMLWebServiceManager.Shapes;

namespace XMLWebServiceManager
{
    public class ProgressInfo
    {
        private string m_strMessage = "";
        private int m_nPercent = 0;

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public int Percent 
        {
            get { return m_nPercent; }
            set { m_nPercent = value; }
        }
    }

    /// <summary>
    /// <para> 건물정보를 가지고 있는 클래스 </para>
    /// <para> BuildingKey : 건물 KEY (string) </para>
    /// <para> Address : 건물 주소 (string) </para>
    /// <para> BuildingMenu : 건물용도 (string) </para>
    /// <para> FloorNo : 건물 층수 (string) </para>
    /// <para> BuildingName : 건물 이름 (string) </para>
    /// <para> UpdateInfo : 업데이트 정보 (string) </para>
    /// </summary>
    public class BulidingInfo
    {
        private string m_strBuildingKey = null;
        private string m_strAddress = null;
        private string m_strBuildingMenu = null;
        private string m_strFloorNo = null;
        private string m_strBuildingName = null;
        private string m_strUpdateInfo = null;

        public string BuildingKey
        {
            get { return m_strBuildingKey; }
            set { m_strBuildingKey = value; }
        }

        public string Address
        {
            get { return m_strAddress; }
            set { m_strAddress = value; }
        }
        
        public string BuildingMenu
        {
            get { return m_strBuildingMenu; }
            set { m_strBuildingMenu = value; }
        }

        public string FloorNo
        {
            get { return m_strFloorNo; }
            set { m_strFloorNo = value; }
        }

        public string BuildingName
        {
            get { return m_strBuildingName; }
            set { m_strBuildingName = value; }
        }

        public string UpdateInfo
        {
            get { return m_strUpdateInfo; }
            set { m_strUpdateInfo = value; }
        }
    }


    public class XMLWebManager
    {
        private string m_strErrorMessage = "";
        private Dictionary<int, POIType> m_dicPOITypes = new Dictionary<int, POIType>();
        private List<Shapes.POITypeProperty> m_poiTypePropertyList = new List<Shapes.POITypeProperty>();

        private WebServiceManager m_webManager = null;
        public WebServiceManager WebManager
        {
            get { return m_webManager; }
            set { m_webManager = value; }
        }

        /// <summary>
        /// 시도 리스트 받아오기
        /// </summary>
        /// <param name="strResultMessage"> 결과 메시지 (string) </param>
        /// <returns> 시도 이름 (string) - 시도 KEY (string)으로 구성된 Dictionary, 오류 발생시 Null 반환 결과 메시지 참고 </returns>
        public Dictionary<string, string> GetSidoList(ref string strResultMessage)
        {
            Dictionary<string, string> dicSido = null;

            //WebServiceManager webMgr = new WebServiceManager();

            // 로그인 하기
            //if (!webMgr.Login(ref strResultMessage))
            //    return null;
            if (m_webManager.Login(ref strResultMessage) == false)
                return null;

            // 시도 리스트 받아오기
            //dicSido = webMgr.GetSidoList(ref strResultMessage);
            dicSido = m_webManager.GetSidoList(ref strResultMessage);

            if (dicSido == null)
                return null;

            return dicSido;
        }

        /// <summary>
        /// 시군구 리스트 받아오기
        /// </summary>
        /// <param name="strSidoKey"> 시군구 조회 할 해당 시도 Key (string) </param>
        /// <param name="strResultMessage"> 결과 메시지 (string) </param>
        /// <returns> 시군구 이름 (string) - 시군구 KEY (string)으로 구성된 Dictionary, 오류 발생시 Null 반환 결과 메시지 참고 </returns>
        public Dictionary<string, string> GetSigunguList(string strSidoKey, ref string strResultMessage)
        {
            Dictionary<string, string> dicSigungu = null;

            //WebServiceManager webMgr = new WebServiceManager();

            // 로그인 하기
            //if (!webMgr.Login(ref strResultMessage))
            //    return null;
            if (m_webManager.Login(ref strResultMessage) == false)
                return null;

            // 시군구 리스트 받아오기
            //dicSigungu = webMgr.GetSigunguList(strSidoKey, ref strResultMessage);
            dicSigungu = m_webManager.GetSigunguList(strSidoKey, ref strResultMessage);

            if (dicSigungu == null)
                return null;

            return dicSigungu;
        }

        /// <summary>
        /// 읍면동 리스트 받아오기
        /// </summary>
        /// <param name="strSigunguKey"> 읍면동 조회 할 해당 시군구 Key (string) </param>
        /// <param name="strResultMessage"> 결과 메시지 (string) </param>
        /// <returns> 읍면동 이름 (string) - 읍면동 KEY (string)으로 구성된 Dictionary, 오류 발생시 Null 반환 결과 메시지 참고 </returns>
        public Dictionary<string, string> GetDongList(string strSigunguKey, ref string strResultMessage)
        {
            Dictionary<string, string> dicDong = null;

            //WebServiceManager webMgr = new WebServiceManager();

            // 로그인 하기
            //if (!webMgr.Login(ref strResultMessage))
            //    return null;
            if (m_webManager.Login(ref strResultMessage) == false)
                return null;

            // 동 리스트 받아오기
            //dicDong = webMgr.GetDongList(strSigunguKey, ref strResultMessage);
            dicDong = m_webManager.GetDongList(strSigunguKey, ref strResultMessage);

            if (dicDong == null)
                return null;

            return dicDong;
        }

        /// <summary>
        /// 건물정보 리스트 받아오기
        /// </summary>
        /// <param name="strSigunguKey"> 건물리스트 조회 할 해당 시군구 Key (string) </param>
        /// <param name="strDongName"> 건물리스트 조회 할 해당 읍면동 이름 (string), Null 또는 빈문자열 입력시 시군구 해당하는 전지역 조회 </param>
        /// <param name="strLoadName"> 건물리스트 조회 할 해당 도로명 (string) </param>
        /// <param name="strBulidingNum"> 건물리스트 조회 할 해당 건물번호 Key (string), Null 또는 빈문자열 입력시 도로명 해당하는 전지역 조회 </param>
        /// <param name="strResultMessage"> 결과 메시지 (string) </param>
        /// <returns> BulidingInfo 클래스로 구성된 List, 오류 발생시 Null 반환 결과 메시지 참고  </returns>
        public List<BulidingInfo> GetBulidingInfoList(string strSigunguKey, string strDongName, string strLoadName, string strBulidingNum, ref string strResultMessage)
        {
            List<BulidingInfo> listBulidingInfo = null;
            strResultMessage = "";

            if (strSigunguKey == null || strSigunguKey == "")
            {
                strResultMessage = "시군구 키 값이 없습니다. 확인해주세요.";
                return null;
            }

            if (strDongName == null)
                strDongName = "";

            if (strLoadName == null)
                strLoadName = "";

            if (strBulidingNum == null)
                strBulidingNum = "";

            //WebServiceManager webMgr = new WebServiceManager();

            // 로그인 하기
            //if (!webMgr.Login(ref strResultMessage))
            //    return null;
            if (m_webManager.Login(ref strResultMessage) == false)
                return null;

            // 빌딩리스트 불러오기
            //listBulidingInfo = webMgr.GetBulidingInfoList(strSigunguKey, strDongName, strLoadName, strBulidingNum, ref strResultMessage);
            listBulidingInfo = m_webManager.GetBulidingInfoList(strSigunguKey, strDongName, strLoadName, strBulidingNum, ref strResultMessage);

            return listBulidingInfo;
        }

        /// <summary>
        /// 업데이트 정보 받아오기
        /// </summary>
        /// <param name="strBuildingKey"> 업데이트 정보를 조회 할 해당 건물 Key (string) </param>
        /// <param name="strResultMessage"> 결과 메시지 (string) </param>
        /// <returns> 업데이트 일자 및 계정 ID으로 구성된 문자열(string), 오류 발생시 Null 반환 결과 메시지 참고 </returns>
        public string GetUpdateInfo(string strBuildingKey, ref string strResultMessage)
        {
            string strUpdateInfo = null;
            strResultMessage = "";

            WebServiceManager webMgr = new WebServiceManager();

            // 로그인 하기
            //if (!webMgr.Login(ref strResultMessage))
            //    return null;
            if (m_webManager.Login(ref strResultMessage) == false)
                return null;

            //strUpdateInfo = webMgr.GetLevelInfo(strBuildingKey, ref strResultMessage);
            strUpdateInfo = m_webManager.GetLevelInfo(strBuildingKey, ref strResultMessage);

            return strUpdateInfo;
        }

        /// <summary>
        /// 다부처 건물정보 XML 다운로드
        /// </summary>
        /// <param name="strXMLFile"> 다운로드 받을 경로 및 파일명 (string) </param>
        /// <param name="strBuildingKey">  다운로드 받을 해당 건물 Key 값 (string) </param>
        /// <param name="strResultMessage"> 결과 메시지 (string) </param>
        /// <returns> 성공, 실패 여부 (bool) </returns>
        public bool DownloadXMLFile(string strXMLFile, string strBuildingKey, ref string strResultMessage, ref ProgressInfo progressInfo)
        {
            WebServiceManager webMgr = new WebServiceManager();

            // 로그인 하기
            //if (!webMgr.Login(ref strResultMessage))
            //    return false;
            if (m_webManager.Login(ref strResultMessage) == false)
                return false;

            progressInfo.Message = "건물데이터 Downloading...";

            // 다운로드 받기
            //if (webMgr.SaveXMLFile(strXMLFile, strBuildingKey, ref strResultMessage) == false)
            if (m_webManager.SaveXMLFile(strXMLFile, strBuildingKey, ref strResultMessage) == false)
                return false;

            strResultMessage = "성공적으로 다운로드 하였습니다.";

            return true;
        }

        /// <summary>
        /// 다부처 건물정보 XML 업로드
        /// </summary>
        /// <param name="strXMLFile"> 업로드 시킬 XML 경로 및 파일명 (string) </param>
        /// <param name="strBuildingKey"> 업로드 할 건물 Key 값 (string) </param>
        /// <param name="strResultMessage"> 결과 메시지 (string) </param>
        /// <returns> 성공, 실패 여부 (bool) </returns>
        public bool UploadXMLFile(string strXMLFile, string strBuildingKey, ref string strResultMessage, ref ProgressInfo progressInfo)
        {
            if (File.Exists(strXMLFile) == false)
            {
                strResultMessage = "Error 해당 파일을 찾을 수 없습니다.";
                return false;
            }

            // XML 파일 Project 객체로 불러오기 -------------------------------
            XMLManager mgr = new XMLManager();
            Project project = mgr.ReadProject(strXMLFile, m_dicPOITypes, ref strResultMessage);

            if (project == null)
                return false;
            else
                project.LocalFilePath = strXMLFile;

            if (mgr.ReadLevels(project, m_dicPOITypes, ref strResultMessage) == false)
                return false;
            //------------------------------------------------------------------

            
            WebServiceManager webMgr = new WebServiceManager();

            // 로그인 하기
            //if (!webMgr.Login(ref strResultMessage))
            //    return false;
            if (m_webManager.Login(ref strResultMessage) == false)
                return false;

            // 업로드 하기
            //if (!webMgr.UploadProject(project, strBuildingKey, ref strResultMessage, ref progressInfo))
            if (!m_webManager.UploadProject(project, strBuildingKey, ref strResultMessage, ref progressInfo))
                return false;

            strResultMessage = "성공적으로 업로드 하였습니다.";

            return true;
        }
    }
}
