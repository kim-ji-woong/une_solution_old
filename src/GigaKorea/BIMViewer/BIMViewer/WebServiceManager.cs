using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using UnE.Geometry;
using System.ComponentModel;
using System.Windows.Forms;

namespace BIMViewer
{
    using BIM;
    using BIMViewer.Shapes;
    using System.Configuration;
    using System.Threading;

    public class WebServiceManager
    {
        public enum BoundartType { Line = 0, Arc, EArc };

        private const string XML_HEADER = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>";
        private string BaseAddress = "";

        private const string API = "/sdesignApi";

        private const string DefaultID = "user_spatial";
        private const string DefaultPassword = "spatial1234";

        private const string SUCCESS_CODE = "RS101";

        private string m_strID = "";
        private string m_strPW = "";

        private string m_strDoubleFormat = "F1";

        //ym.
        private Dictionary<string, string> m_strPropTable = new Dictionary<string, string>();

        public WebServiceManager(string strID = "", string strPW = "")
        {
            // SSL/TLS 상위 버전 호환 설정
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

            if (strID.Length == 0)
                m_strID = DefaultID;
            else
                m_strID = strID;

            if (strPW.Length == 0)
                m_strPW = DefaultPassword;
            else
                m_strPW = strPW;

            this.BaseAddress = ConfigurationManager.AppSettings.Get("WebServiceBaseURL");
            if (this.BaseAddress == null || this.BaseAddress.Length == 0)
                this.BaseAddress = "https://sdesign.etri.re.kr:8443";

            m_strPropTable.Add("벽체정보", "PROP_001");
            m_strPropTable.Add("강성비정형성여부", "PROP_002");
            m_strPropTable.Add("필로티구조물여부", "PROP_003");
            m_strPropTable.Add("업종", "PROP_004");
            m_strPropTable.Add("재질", "PROP_005");
            m_strPropTable.Add("마감재", "PROP_006");
            m_strPropTable.Add("방화문유무", "PROP_007");
            m_strPropTable.Add("방화구역유무", "PROP_008");
            m_strPropTable.Add("Thick", "PROP_009");
            m_strPropTable.Add("Height", "PROP_010");
        }

        //ym. for Up Down progressbar 
        private BackgroundWorker worker = null;
        private uProgressForm m_ufrm = null;

        private int m_updownType = 0;
        private string m_downFileName = "";
        private bool m_bUpDownResult = false;
        private Project m_updownProject = null;
        private Dictionary<int, POIType> m_updownDicPOITypes = null;
        private string m_updownBuildingKey = "";
        private string m_updownLoginID = "";
        private string m_updownLoginKey = "";
        private Dictionary<string, string> m_updownPoiUserList = null;

        private int m_nUpDownErrCode = 0;

        private void Worker_RunWorkerCompleted_Old(object sender, RunWorkerCompletedEventArgs e)
        {
            DateTime dtStart = m_ufrm.StartTime;
            DateTime dtEnd = DateTime.Now;

            TimeSpan tsWork = dtEnd - dtStart;
            string strWork = ChangeTimeToString(tsWork);

            m_ufrm.Close();

            if (e.Error != null)
                throw e.Error;

            if (m_bUpDownResult)
            {
                if (m_updownType == 0) MessageBox.Show("UpLoad 완료\n\n시작시간: " + dtStart.ToString() + "\n종료시간: " + dtEnd.ToString() + "\n소요시간: " + strWork, "완료");
                else MessageBox.Show("DownLoad 완료\n\n시작시간: " + dtStart.ToString() + "\n종료시간: " + dtEnd.ToString() + "\n소요시간: " + strWork, "완료");
            }
            else
            {
                if (m_updownType == 0) MessageBox.Show("UpLoad 실패\n\n시작시간: " + dtStart.ToString() + "\n종료시간: " + dtEnd.ToString() + "\n소요시간: " + strWork, "실패");
                else MessageBox.Show("DownLoad 실패\n\n시작시간: " + dtStart.ToString() + "\n종료시간: " + dtEnd.ToString() + "\n소요시간: " + strWork, "실패");
            }

            m_ufrm = null;
            worker.Dispose();
            worker = null;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DateTime dtStart = m_ufrm.StartTime;
            DateTime dtEnd = DateTime.Now;

            TimeSpan tsWork = dtEnd - dtStart;
            string strWork = ChangeTimeToString(tsWork);

            m_ufrm.Close();

            if (e.Error != null)
                throw e.Error;

            if (m_bUpDownResult)
            {
                if (m_updownType == 0)
                    MessageBox.Show("UpLoad 완료\n\n시작시간: " + dtStart.ToString() + "\n종료시간: " + dtEnd.ToString() + "\n소요시간: " + strWork, "완료");

                else MessageBox.Show("DownLoad 완료\n\n시작시간: " + dtStart.ToString() + "\n종료시간: " + dtEnd.ToString() + "\n소요시간: " + strWork, "완료");
            }
            else
            {
                if (m_updownType == 0)
                {
                    if (m_nUpDownErrCode == 1)
                        MessageBox.Show("UpLoad 실패\n\n시작시간: " + dtStart.ToString() + "\n종료시간: " + dtEnd.ToString() + "\n소요시간: " + strWork, "실패");
                }
                else MessageBox.Show("DownLoad 실패\n\n시작시간: " + dtStart.ToString() + "\n종료시간: " + dtEnd.ToString() + "\n소요시간: " + strWork, "실패");
            }

            m_ufrm = null;
            worker.Dispose();
            worker = null;

            // 선택된 프로젝트 리로드
            FormMain.Instance.ReloadProject();
        }

        public string ChangeTimeToString(TimeSpan timeSpan)
        {
            string strDate = "";

            if (timeSpan.Days != 0)
                strDate += timeSpan.Days + "일 ";
            if (timeSpan.Hours != 0)
                strDate += timeSpan.Hours + "시간 ";
            if (timeSpan.Minutes != 0)
                strDate += timeSpan.Minutes + "분 ";
            if (timeSpan.Seconds != 0)
                strDate += timeSpan.Seconds + "초";

            return strDate;
        }

        private void Worker_DoUpLoad_Work(object sender, DoWorkEventArgs e)
        {   // TODO: 업로드 기능 수정중

            m_bUpDownResult = false;
            m_nUpDownErrCode = 1;

            // FireSafetyManager
            // 프로젝트 ID 체크
            // XML 속성에 프로젝트 ID 얻어오기
            string strProjectID = GetProjectID();

            if (strProjectID == null)
            {
                // ID가 없을 경우 공간정보를 업로드 한 뒤에 소방설비 업로드 진행 표시 후 취소
                MessageBox.Show("공간정보를 다운받아 작업하시고 소방설비 POI 업로드를 진행해주십시오.", "취소", MessageBoxButtons.OK);
                m_nUpDownErrCode = 0;
                return;
            }
            else if (m_updownBuildingKey != strProjectID)
            {
                // 프로젝트 ID가 다를 경우 주소지에 맞는 정보가 아님을 표시 후 취소
                MessageBox.Show("주소지에 맞는 공간정보가 아닙니다.", "취소", MessageBoxButtons.OK);
                m_nUpDownErrCode = 0;
                return;
            }


            //ID가 같을 경우
            // level 체크
            List<Level> levels = m_updownProject.Levels;
            Dictionary<string, string> dicRemoveLevel = new Dictionary<string, string>();
            Dictionary<string, string> dicLevel = ReadLevelNames(m_updownBuildingKey, m_updownLoginID, m_updownLoginKey);

            int nLevelCount = 0;
            nLevelCount = dicLevel.Count();


            // Level ID 수정(변경) 유무
            bool bCheck = false;

            // 노아서버 Level과 XML Level 비교 
            bCheck = CheckModifity(levels, out dicRemoveLevel);

            if (dicRemoveLevel.Count == 0 && bCheck == false)
            {
                // level 같을 경우 수정 여부 확인 뒤, 속성값 수정
                if (MessageBox.Show("수정하시겠습니까?", "확인", MessageBoxButtons.YesNo)
                    == System.Windows.Forms.DialogResult.Yes)
                {
                    // level ID 같으면 poi 수정
                    if (!UploadModifityPOI())
                        return;
                }
                else
                {
                    m_nUpDownErrCode = 0;
                    return;
                }
            }
            else if (nLevelCount == dicRemoveLevel.Count)
            {
                // level 다르고 Name이 일치하지 않을 경우 공간정보가 일치하지 않으므로 확인하고 소방설비 POI 업로드를 진행해주세요. 취소
                MessageBox.Show("주소지 건물에 공간정보가 일치하지 않습니다. 공간정보를 다시 업로드하시고 소방설비 POI 업로드를 진행해주십시오.", "취소", MessageBoxButtons.OK);
                m_nUpDownErrCode = 0;
                return;
            }
            else if (dicRemoveLevel.Count != 0 || bCheck == true)
            {
                // level 다를 경우 도면 수정 여부를 확인 뒤 
                if (MessageBox.Show("작업 중인 도면과 서버의 도면이 일치하지 않을 수 있습니다. 그래도 강제적으로 POI를 업로드 하시겠습니까?", "확인", MessageBoxButtons.YesNo)
                    == System.Windows.Forms.DialogResult.Yes)
                {
                    // level 다르고 Name이 일치할 경우 Level ID 변환 후 POI 수정
                    if (!UploadModifityLevel())
                        return;
                }
                else
                {
                    m_nUpDownErrCode = 0;
                    return;
                }
            }

            // 노아서버 XML 다운받기 (서버 XML과 동기화)
            if (!DownloadXML())
                return;

            m_bUpDownResult = true;
        }

        private string GetProjectID()
        {
            string retProjectID = "";
            List<Property> properties = m_updownProject.Properties;

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

        private bool CheckModifity(List<Level> levels, out Dictionary<string, string> dicRemoveLevel)
        {   // 노아서버 Level과 XML Level 비교 

            Dictionary<string, string> dicLevel = new Dictionary<string, string>();

            // 노아서버 현재 레벨 조회
            dicLevel = ReadLevelNames(m_updownBuildingKey, m_updownLoginID, m_updownLoginKey);
            dicRemoveLevel = ReadLevelNames(m_updownBuildingKey, m_updownLoginID, m_updownLoginKey);

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
            }

            return bRetModifity;
        }

        private bool UploadModifityPOI()
        {
            List<Level> levels = m_updownProject.Levels;

            int i = 1;
            int lvcnt = levels.Count;

            foreach (Level level in levels)
            {
                string strLevelID = level.XMLID;

                //poi데이터 업로드.
                Dictionary<Shapes.POI, string> dicPOIIDs = UploadPOIs_NEW(level, m_updownBuildingKey, strLevelID, m_updownLoginID, m_updownLoginKey);

                if (dicPOIIDs == null)
                    return false;

                if (UploadPOIWires_NEW(level.Wires, dicPOIIDs, level, strLevelID, m_updownLoginID, m_updownLoginKey) == false)
                    return false;

                //Progerssbar...
                double percent = (double)i;
                percent = percent / (double)lvcnt;
                percent = percent * 100;
                percent = (int)percent;
                string sMessage = level.Name + " 층 업로드중 : " + percent.ToString() + " %";
                worker.ReportProgress((int)percent, sMessage);
                i++;
            }

            return true;
        }

        private bool UploadModifityLevel()
        {
            List<Level> levels = m_updownProject.Levels;
            Dictionary<string, string> dicLevel = ReadLevelNames(m_updownBuildingKey, m_updownLoginID, m_updownLoginKey);

            int i = 1;
            int lvcnt = levels.Count;

            foreach (Level level in levels)
            {
                string strLevelID = level.XMLID;

                if (dicLevel.ContainsKey(level.XMLID))
                {
                    // 기존 Level ID가 있을 경우 poi데이터 업로드
                    Dictionary<Shapes.POI, string> dicPOIIDs = UploadPOIs_NEW(level, m_updownBuildingKey, strLevelID, m_updownLoginID, m_updownLoginKey);

                    if (dicPOIIDs == null)
                        return false;

                    if (UploadPOIWires_NEW(level.Wires, dicPOIIDs, level, strLevelID, m_updownLoginID, m_updownLoginKey) == false)
                        return false;
                }
                else if (dicLevel.ContainsValue(level.Name))
                {
                    // Level ID가 다르고 똑같은 Level Name이 있을 경우 새로운 Level ID 값으로 데이터 갱신
                    
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
                    Dictionary<Shapes.POI, string> dicPOIIDs = UploadPOIs_NEW(level, m_updownBuildingKey, strLevelID, m_updownLoginID, m_updownLoginKey);

                    if (dicPOIIDs == null)
                        return false;

                    if (UploadPOIWires_NEW(level.Wires, dicPOIIDs, level, strLevelID, m_updownLoginID, m_updownLoginKey) == false)
                        return false;

                }

                //Progerssbar...
                double percent = (double)i;
                percent = percent / (double)lvcnt;
                percent = percent * 100;
                percent = (int)percent;
                string sMessage = level.Name + " 층 업로드중 : " + percent.ToString() + " %";
                worker.ReportProgress((int)percent, sMessage);
                i++;
            }

            return true;
        }

        private bool DownloadXML()
        {
            string strResult = ReadProject(m_updownProject, m_updownLoginID, m_updownLoginKey, m_updownBuildingKey);
            if (strResult == null || strResult.Length == 0)
                return false;

            XElement xTemp = XElement.Parse(strResult);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(strResult);

            foreach (XmlNode rootNode in xmlDoc.ChildNodes)
            {
                if (rootNode.Name == "IndoorModelFile")
                {
                    XmlAttribute version = xmlDoc.CreateAttribute("version");
                    version.Value = XMLManager.TARGET_VERSION;
                    rootNode.Attributes.Append(version);

                    foreach (XmlNode IndoorNode in rootNode.ChildNodes)
                    {
                        if (IndoorNode.Name == "ProjectInfo")
                        {
                            XmlAttribute unit = xmlDoc.CreateAttribute("unit");
                            unit.Value = "mm";
                            IndoorNode.Attributes.Append(unit);

                            XmlAttribute datetime = xmlDoc.CreateAttribute("datetime");
                            datetime.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            IndoorNode.Attributes.Append(datetime);

                            break;
                        }
                    }

                    break;
                }
            }

            xmlDoc.Save(m_updownProject.LocalFilePath);

            return true;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            m_ufrm.SetProgress(e.UserState.ToString(), e.ProgressPercentage);
        }

        public void Upload(Project project, Dictionary<int, POIType> dicPOITypes, string strBuildingKey, string strID, string strKey, Dictionary<string, string> poiUserList)
        {
            m_updownType = 0;
            m_updownProject = project;
            m_updownDicPOITypes = dicPOITypes;
            m_updownBuildingKey = strBuildingKey;
            m_updownLoginID = strID;
            m_updownLoginKey = strKey;
            m_updownPoiUserList = poiUserList;

            GetGrpCodeIDs(m_updownLoginID, m_updownLoginKey);

            m_ufrm = new uProgressForm(0);//0 is Uploading
            m_ufrm.StartPosition = FormStartPosition.CenterParent;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoUpLoad_Work;
            worker.ProgressChanged += Worker_ProgressChanged;
            //worker.RunWorkerCompleted += Worker_RunWorkerCompleted_Old;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
            m_ufrm.ShowDialog();//modal
     }

        public void Download(Project project, string strLocalPath, string strBuildingKey, string strID, string strKey)
        {
            m_updownType = 1;
            m_updownProject = project;
            m_downFileName = strLocalPath;
            m_updownBuildingKey = strBuildingKey;
            m_updownLoginID = strID;
            m_updownLoginKey = strKey;

            GetGrpCodeIDs(m_updownLoginID, m_updownLoginKey);

            m_ufrm = new uProgressForm(1);//1 is Downloading
            m_ufrm.StartPosition = FormStartPosition.CenterParent;

            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoDownLoad_Work;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted_Old;
            worker.RunWorkerAsync();
            m_ufrm.ShowDialog();//modal
      }
                
        private void Worker_DoDownLoad_Work(object sender, DoWorkEventArgs e)
        {
            m_bUpDownResult = false;

            string strResult = ReadProject(m_updownProject, m_updownLoginID, m_updownLoginKey, m_updownBuildingKey);
            if (strResult == null || strResult.Length == 0)
                return;

            XElement xTemp = XElement.Parse(strResult);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(strResult);

            foreach (XmlNode rootNode in xmlDoc.ChildNodes)
            {
                if (rootNode.Name == "IndoorModelFile")
                {
                    XmlAttribute version = xmlDoc.CreateAttribute("version");
                    version.Value = XMLManager.TARGET_VERSION;
                    rootNode.Attributes.Append(version);

                    foreach (XmlNode IndoorNode in rootNode.ChildNodes)
                    {
                        if (IndoorNode.Name == "ProjectInfo")
                        {
                            XmlAttribute unit = xmlDoc.CreateAttribute("unit");
                            unit.Value = "mm";
                            IndoorNode.Attributes.Append(unit);

                            XmlAttribute datetime = xmlDoc.CreateAttribute("datetime");
                            datetime.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            IndoorNode.Attributes.Append(datetime);

                            break;
                        }
                    }

                    break;
                }
            }

            xmlDoc.Save(m_downFileName);

            m_bUpDownResult = true;
        }
  
        private bool RemoveLevelTest(string strID, string strKey)
        {
            string[] LevelIDs = {
                "level_2019051413051801",
                "level_2019051413051802",
                "level_2019051413051803",
                "level_2019051413051804",
                "level_2019051413051805",
                "level_2019051413051806",
                "level_2019051413051807",
                "level_2019051413051808",
                "level_2019051413051809",
                "level_2019051413051810",
                "level_2019051413051811",
                "level_2019051413051812",
                "level_2019051413051813",
                "level_2019051413051814"
            };

            foreach (string strLevelID in LevelIDs)
            {
                XElement xUserID = MakeElement("user_id", strID);
                XElement xKeyID = MakeElement("key_id", strKey);

                XElement xState = new XElement("state");
                xState.Add(xUserID);
                xState.Add(xKeyID);

                XElement xLevel = new XElement("level");

                XElement xBuildingID = MakeElement("id", strLevelID);
                xLevel.Add(xBuildingID);

                XElement xRoot = new XElement("spatial");
                xRoot.Add(xState);
                xRoot.Add(xLevel);

                string strErrorMessage;
                string strXML = xRoot.ToString();
                string strResult = SendQuery(strXML, "spatial/level", true, out strErrorMessage, "DELETE");

                if (strResult.Length == 0)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveLevel Error : " + strErrorMessage);
                    return false;
                }
                else
                    System.Diagnostics.Trace.WriteLine("RemoveLevel Success");
            }

            return true;
        }

        private bool RemoveGrids(List<string> gridIDs, string strID, string strKey)
        {
            foreach (string strGridID in gridIDs)
            {
                string strURL = string.Format("spatial/grid/{0}/{1}/{2}", strID, strKey, strGridID);
                string strErrorMessage;

                string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

                if (strResult.Length == 0)
                {
                    if (strErrorMessage != SUCCESS_CODE)
                    {
                        System.Diagnostics.Trace.WriteLine("RemoveGrids Error : " + strErrorMessage);
                        return false;
                    }
                }
            }

            return true;
        }

        private bool ReadPOITypes(Dictionary<POIType, string> dicPOITypeCode, Dictionary<int, POIType> dicPOITypes, string strID, string strKey, string strGroupCode = "E-LARGE", POIType parent = null)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("code");
            xRoot.Add(xState);

            XElement xGroupCode = new XElement("paramGroupCode", strGroupCode);
            XElement xParent = null;

            if (strGroupCode == "E-MEDIUM")
                xParent = new XElement("paramLargeCode", parent.Code);
            else if (strGroupCode == "E-SMALL")
                xParent = new XElement("paramMediumCode", parent.Code);
            else if (strGroupCode == "E-DETAIL")
                xParent = new XElement("paramSmallCode", parent.Code);

            xRoot.Add(xGroupCode);

            if (xParent != null)
                xRoot.Add(xParent);

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "code/equipType", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("ReadPOITypes Error : " + strErrorMessage);
                return false;
            }

            XElement xml = XElement.Parse(strResult);

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "listEquipTypeCode" ? element : null;

                if (xNode != null)
                {
                    string strCode = null, strName = null;

                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "equip_type_code")
                        {
                            strCode = child.Value.Trim();

                            if (strName != null)
                                break;
                        }
                        else if (child.Name == "equip_type_title")
                        {
                            strName = child.Value.Replace('‧', '/').Trim();

                            if (strCode != null)
                                break;
                        }
                    }

                    if (strCode != null && strName != null)
                    {
                        POIType type = null;

                        if (parent == null)
                        {
                            foreach (KeyValuePair<int, POIType> pair in dicPOITypes)
                            {
                                POIType poiType = pair.Value;

                                if (poiType.Parent == parent && poiType.Name == strName)
                                {
                                    poiType.Code = strCode;
                                    type = poiType;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (POIType poiType in parent.ChildTypes)
                            {
                                if (poiType.Parent == parent && poiType.Name == strName)
                                {
                                    poiType.Code = strCode;
                                    type = poiType;
                                    break;
                                }
                            }
                        }

                        if (type != null)
                        {
                            dicPOITypeCode[type] = strCode;

                            string strParamGroupCode = "";

                            if (strGroupCode == "E-LARGE")
                                strParamGroupCode = "E-MEDIUM";
                            else if (strGroupCode == "E-MEDIUM")
                                strParamGroupCode = "E-SMALL";
                            else if (strGroupCode == "E-SMALL")
                                strParamGroupCode = "E-DETAIL";

                            if (strParamGroupCode.Length > 0)
                            {
                                if (ReadPOITypes(dicPOITypeCode, dicPOITypes, strID, strKey, strParamGroupCode, type) == false)
                                    return false;
                            }
                        }
                        else
                            System.Diagnostics.Trace.WriteLine("Unknown POIType : " + strCode + ", " + strName);
                    }
                }
            }

            return true;
        }


        private Dictionary<string, string> ReadLevelNames(string strBuildingID, string strID, string strKey)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xLevel = new XElement("level");

            XElement xBuildingID = MakeElement("build_mng_no", strBuildingID);
            xLevel.Add(xBuildingID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xLevel);

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/levelList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("ReadLevels Error : " + strErrorMessage);
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
                            //levelIDs.Add(child.Value);
                            strLevelID = child.Value;
                        }

                        if (child.Name == "level_name")
                        {
                            //levelIDs.Add(child.Value);
                            strLevelName = child.Value;
                        }
                    }

                    dicLevels[strLevelID] = strLevelName;
                }
            }

            //return levelIDs;
            return dicLevels;
        }

        // Return 값 : Grid ID List
        private List<string> GetGridList(string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/gridList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetGridList Error : " + strErrorMessage);
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

        private void DeleteCode(string strID, string strKey)
        {
             string resResult = string.Empty;
             string strURL = BaseAddress + API + "/code/sysCode";


             string strXML;

             strXML = "/" + strID + "/" + strKey + "/" + "PROP_006";
            strURL = strURL + strXML;

             byte[] bytes = Encoding.UTF8.GetBytes(strXML);
             int len = bytes.Count();

             HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL));
             request.Method = "DELETE";

            //delete 방식은 문자열 조절불필요
            // request.ContentType = "application/xml; charset=utf-8";
            // request.ContentLength = len + 3;
            HttpWebResponse response2 = (HttpWebResponse)request.GetResponse();
            if(response2.StatusCode == HttpStatusCode.OK)
            {
                return;
            }                                  
        }
        //ym Property 상세코드 조회
        private void PropertyCodeList(string strID, string strKey)
        {
            string resResult = string.Empty;
            string strURL = BaseAddress + API + "/code/sysCodeList";

            string strXML = XML_HEADER;
            strXML += "<common>";
            strXML += "<state>";
            strXML += "<user_id>" + strID + "</user_id>";
            strXML += "<key_id>" + strKey + "</key_id>";
            strXML += "</state>";
            strXML += "<paramGrpCode>" + "PROP_TYP" + "</paramGrpCode>";
            strXML += "</common>";

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

                return;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("LoadDetailProp Fail : " + ex.Message);
            }
        }

        //ym (원기둥, 사각기둥)재질, 구조벽 마감재, 문 방화문유무, 공간 방화구역유무, 층 높이 5가지 속성추가 (상세코드등록)
        public void AddPropertyCode(string strID, string strKey, string sys_code, string grp_code, string sys_title, string description)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("common");
            xRoot.Add(xState);

            XElement x1 = new XElement("codeSys");
            XElement x2 = MakeElement("sys_code", sys_code);//"PROP_010"
            XElement x3 = MakeElement("grp_code", grp_code);//"PROP_TYP"
            XElement x4 = MakeElement("sys_title", sys_title);//"Height"
            XElement x5 = MakeElement("description", description);//"Height"

            x1.Add(x2);
            x1.Add(x3);
            x1.Add(x4);
            x1.Add(x5);
            xRoot.Add(x1);

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "code/sysCode", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("PropertyCode add Error : " + strErrorMessage);
                return;
            }

            PropertyCodeList(strID, strKey);//등록한 코드 조회해봄.
        }

        private void AdressCode(string strID, string strKey)
        {
            string resResult = string.Empty;
            string strURL = BaseAddress + API + "/code/sido";

            string strXML = XML_HEADER;
            strXML += "<code>";
            strXML += "<state>";
            strXML += "<user_id>" + strID + "</user_id>";
            strXML += "<key_id>" + strKey + "</key_id>";
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

                //  if (!MakePropertyList(resResult))
                //     return false;

                return;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("LoadDetailProp Fail : " + ex.Message);
            }
        }
        
        private bool RemovePOILIst(List<string> poiIDList, string strID, string strKey)
        {
            foreach (string poiID in poiIDList)
            {
                string strURL = string.Format("convergence/fireEquip/{0}/{1}/{2}", strID, strKey, poiID);
                string strErrorMessage;

                string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

                if (strResult.Length == 0)
                {
                    if (strErrorMessage != SUCCESS_CODE)
                    {
                        System.Diagnostics.Trace.WriteLine("RemovePOILIst Error : " + strErrorMessage);
                        return false;
                    }
                }
            }

            return true;
        }
        private bool RemovePOIWireList(List<string> poiWireIDList, string strID, string strKey)
        {
            foreach (string poiWireID in poiWireIDList)
            {
                string strURL = string.Format("spatial/equipWire/{0}/{1}/{2}", strID, strKey, poiWireID);
                string strErrorMessage;

                string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

                if (strResult.Length == 0)
                {
                    if (strErrorMessage != SUCCESS_CODE)
                    {
                        System.Diagnostics.Trace.WriteLine("RemovePoiwire Error : " + strErrorMessage);
                        return false;
                    }
                }
            }

            return true;
        }
        private  List<string> GetPOIWireIDList(string strlevelID, string strID, string strKey, Dictionary<string, string> poiUserList)
        {
            string resResult = string.Empty;

            string strURL = BaseAddress + API + "/spatial/equipWireList";

            string strXML = XML_HEADER;
            strXML += "<spatial>";
            strXML += "<state>";
            strXML += "<user_id>" + strID + "</user_id>";
            strXML += "<key_id>" + strKey + "</key_id>";            
            strXML += "</state>";
            strXML += "<equipWire>";
            strXML += "<level_id>" + strlevelID + "</level_id>";
            strXML += "</equipWire>";            
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
                return null;
            }

            //poi사용자가올린. poi ID List 만들기
            List<string> strPOIWireIDs = new List<string>();
            XElement xml = XElement.Parse(resResult);
            foreach (XElement element in xml.Elements())
            {
                XElement xPoi = element.Name == "equipWireList" ? element : null;

                if (xPoi != null)
                {
                    bool flag = false;
                    string equip_id = "";
                    foreach (XElement child in xPoi.Elements())
                    {
                        if (child.Name == "wire_id")
                            equip_id = child.Value;

                        if (child.Name == "create_user_id" && poiUserList.ContainsKey(child.Value))
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (flag)
                        strPOIWireIDs.Add(equip_id);
                }
            }

            return strPOIWireIDs;
        }
        private List<string> GetPOIIDList(string strBuildingID, string strID, string strKey, Dictionary<string, string> poiUserList)
        {
            string resResult = string.Empty;

            string strURL = BaseAddress + API + "/convergence/fireEquipList";

            string strXML = XML_HEADER;
            strXML += "<convergence>";
            strXML += "<state>";
            strXML += "<user_id>" + strID + "</user_id>";
            strXML += "<key_id>" + strKey + "</key_id>";
            strXML += "</state>";
            strXML += "<paramBuildMngNo>" + strBuildingID + "</paramBuildMngNo>";
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
                return null;
            }

            //poi사용자가올린. poi ID List 만들기
            List<string> strPOIIDs = new List<string>();
            XElement xml = XElement.Parse(resResult);
            foreach (XElement element in xml.Elements())
            {
                XElement xPoi = element.Name == "equipList" ? element : null;

                if (xPoi != null)
                {
                    bool flag = false;
                    string equip_id = "";
                    foreach (XElement child in xPoi.Elements())
                    {
                        if (child.Name == "equip_id")
                            equip_id = child.Value;

                        if (child.Name == "create_user_id" && poiUserList.ContainsKey(child.Value))
                        {
                            flag = true;
                            break;
                        }
                   }

                    if (flag)
                        strPOIIDs.Add(equip_id);
                }
            }

            return strPOIIDs;
        }
        public string GetPOIInfo(string strBuildingID, string strID, string strKey)
        {
            string resResult = string.Empty;

            string strURL = BaseAddress + API + "/convergence/fireEquipList";

            string strXML = XML_HEADER;
            strXML += "<convergence>";
            strXML += "<state>";
            strXML += "<user_id>" + strID + "</user_id>";
            strXML += "<key_id>" + strKey + "</key_id>";
            strXML += "</state>";
            strXML += "<paramBuildMngNo>" + strBuildingID + "</paramBuildMngNo>";
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
                return resResult;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("LoadDetailProp Fail : " + ex.Message);
                return ex.Message;
            }
        }
    
        public string GetLevelInfo(string strBuildingID, string strID, string strKey)
        {
            string resResult = string.Empty;

            string strURL = BaseAddress + API + "/spatial/levelList";

            string strXML = XML_HEADER;
            strXML += "<spatial>";
            strXML += "<state>";
            strXML += "<user_id>" + strID + "</user_id>";
            strXML += "<key_id>" + strKey + "</key_id>";
            strXML += "</state>";
            strXML += "<level>";
            strXML += "<build_mng_no>" + strBuildingID + "</build_mng_no>";
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

                return resResult;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("LoadDetailProp Fail : " + ex.Message);
                return resResult;
            }
        }
        public string GetBuildingList(string sggCode, string roadCode, string mainNumber, string subNumber, string strID, string strKey)
        {
            string resResult = string.Empty;

            string strURL = BaseAddress + API + "/convergence/buildingList";

            string strXML = XML_HEADER;
            strXML += "<convergence>";
            strXML += "<state>";
            strXML += "<user_id>" + strID + "</user_id>";
            strXML += "<key_id>" + strKey + "</key_id>";
            strXML += "</state>";
            strXML += "<paramSggCode>" + sggCode + "</paramSggCode>";
            strXML += "<paramRoadCode>" + roadCode + "</paramRoadCode>";
            strXML += "<paramRoadMain>" + mainNumber + "</paramRoadMain>";
            strXML += "<paramRoadSub>" + subNumber + "</paramRoadSub>";
            strXML += "<pageNo>" + "0" + "</pageNo>";
            strXML += "<pageSize>" + "10000" + "</pageSize>";
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

                return resResult;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("LoadDetailProp Fail : " + ex.Message);
                return resResult;
            }
        }
        public string GetRoadList(string strSggCode, string strEmdName, string strRoadName, string strID, string strKey)
        {
            string resResult = string.Empty;

            string strURL = BaseAddress + API + "/code/addrRoad";

            string strXML = XML_HEADER;
            strXML += "<code>";
            strXML += "<state>";
            strXML += "<user_id>" + strID + "</user_id>";
            strXML += "<key_id>" + strKey + "</key_id>";
            strXML += "</state>";
            strXML += "<paramSggCode>" + strSggCode + "</paramSggCode>";
            strXML += "<paramRoadName>" + strRoadName + "</paramRoadName>";
            strXML += "<paramDongName>" + strEmdName + "</paramDongName>";
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

                return resResult;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("LoadDetailProp Fail : " + ex.Message);
                return resResult;
            }
        }
        public string GetEmdList(string strSggCode, string strID, string strKey)
        {
            string resResult = string.Empty;

            string strURL = BaseAddress + API + "/code/dong";

            string strXML = XML_HEADER;
            strXML += "<code>";
            strXML += "<paramSigunguCode>" + strSggCode + "</paramSigunguCode>";
            strXML += "<state>";
            strXML += "<user_id>" + strID + "</user_id>";
            strXML += "<key_id>" + strKey + "</key_id>";
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

                return resResult;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("LoadDetailProp Fail : " + ex.Message);
                return resResult;
            }
        }

        public string GetSggList(string strSidoCode, string strID, string strKey)
        {
            string resResult = string.Empty;

            string strURL = BaseAddress + API + "/code/sigungu";

            string strXML = XML_HEADER;
            strXML += "<code>";
            strXML += "<paramSidoCode>" + strSidoCode + "</paramSidoCode>";
            strXML += "<state>";
            strXML += "<user_id>" + strID + "</user_id>";
            strXML += "<key_id>" + strKey + "</key_id>";
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

                return resResult;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("LoadDetailProp Fail : " + ex.Message);
                return resResult;
            }
        }
        public string GetSidoList(string strID, string strKey)
        {
            string resResult = string.Empty;

            string strURL = BaseAddress + API + "/code/sido";

            string strXML = XML_HEADER;
            strXML += "<code>";
            strXML += "<state>";
            strXML += "<user_id>" + strID + "</user_id>";
            strXML += "<key_id>" + strKey + "</key_id>";
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

                return resResult;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("LoadDetailProp Fail : " + ex.Message);
                return resResult;
            }
        }
        private string GetPropertyCode(string strPropertyName)
        {
            foreach(KeyValuePair<string, string> pair in m_strPropTable)
            {
                if (pair.Key == strPropertyName)
                    return pair.Value;
            }
            return "PROP_001";
        }
        private bool UploadColumnProperty(Dictionary<Column, string> dicColumnIDs, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/columnProp", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadColumnProperty Error : " + strErrorMessage);
                return false;
            }

            return true;
        }
        private bool UploadColumns(string strLevelID, List<Column> columns, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/column", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadColumn Error : " + strErrorMessage);
                return false;
            }
            //--
            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadColumn Error2 : " + strResult);
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

            if (!UploadColumnProperty(dicColumnIDs, strID, strKey))
                return false;

            return true;
        }

        private bool UploadPOIWires(List<Shapes.Wire> wires, Dictionary<Shapes.POI, string> dicPOIIDs, Level level, string strLevelID, string strID, string strKey)
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
                                
                xEquip.Add(xBeginPOI);
                xEquip.Add(xEndPOI);
                xEquip.Add(xEquipTypeCode);
                xEquip.Add(xLines);
                xEquip.Add(xLevelID);

                xRoot.Add(xEquip);
                nWireCount++;
            }

            if (nWireCount == 0)
                return true;

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/equipWire", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadPOIWires Error : " + strErrorMessage);
                return false;
            }

            return true;
        }

        private bool UploadPOIWires_NEW(List<Shapes.Wire> wires, Dictionary<Shapes.POI, string> dicPOIIDs, Level level, string strLevelID, string strID, string strKey)
        {
            // 노아서버 POIWire 조회
            List<string> poiWireList = GetPOIWireList(strLevelID, strID, strKey);


            // 조회된 POIWire 노아서버에 삭제 요청
            if (RemovePOIWires(poiWireList, strLevelID, strID, strKey) == false)
                return false;

            // POIWire 노아서버에 추가 요청
            if (UpdatePOIWires(wires, dicPOIIDs, level, strLevelID, strID, strKey) == false)
                return false;

            return true;
        }

        private List<string> GetPOIWireList(string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/equipWireList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetPOIWireList Error : " + strErrorMessage);
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

        private bool RemovePOIWires(List<string> poiWireIDs, string strLevelID, string strID, string strKey)
        {
            foreach (string strPOIWireID in poiWireIDs)
            {
                string strURL = string.Format("spatial/equipWire/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strPOIWireID);
                string strErrorMessage;

                string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

                if (strResult.Length == 0)
                {
                    if (strErrorMessage != SUCCESS_CODE)
                    {
                        System.Diagnostics.Trace.WriteLine("RemovePOIWires Error : " + strErrorMessage);
                        return false;
                    }
                }
            }

            return true;
        }

        private bool UpdatePOIWires(List<Shapes.Wire> wires, Dictionary<Shapes.POI, string> dicPOIIDs, Level level, string strLevelID, string strID, string strKey)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

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

                XElement xEquip = new XElement("equipWire");

                XElement xWireID = new XElement("wire_id", strLevelID);
                XElement xLevelID = new XElement("level_id", strLevelID);
                XElement xBeginPOI = new XElement("begin_equip", strBeginID);
                XElement xEndPOI = new XElement("end_equip", strEndID);
                XElement xEquipTypeCode = new XElement("equip_type_code", wire.POIIcon.PoiType.Code);
                XElement xLines = new XElement("lines", wire.Lines);

                xEquip.Add(xWireID);
                xEquip.Add(xBeginPOI);
                xEquip.Add(xEndPOI);
                xEquip.Add(xEquipTypeCode);
                xEquip.Add(xLines);
                xEquip.Add(xLevelID);

                xRoot.Add(xEquip);

                string strErrorMessage;
                string strXML = xRoot.ToString();
                string strResult = SendQuery(strXML, "spatial/equipWire", true, out strErrorMessage, "PUT");

                if (strResult.Length == 0)
                {
                    System.Diagnostics.Trace.WriteLine("UpdatePOIWires Error : " + strErrorMessage);
                    return false;
                }

                XElement xml = XElement.Parse(strResult);

                if (xml == null)
                {
                    System.Diagnostics.Trace.WriteLine("UpdatePOIWires Error2 : " + strResult);
                    return false;
                }
            }

            return true;
        }


        // Return 값
        //           Value : POI ID
        private Dictionary<Shapes.POI, string> UploadPOIs(List<Shapes.POI> pois, string strBuildingID, string strLevelID, string strID, string strKey)
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
                XElement xEquipTypeCode = new XElement("equip_type_code", poi.PoiType.Code);//code? ID?
                
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "convergence/fireEquip", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadPOIs Error : " + strErrorMessage);
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadPOIs Error2 : " + strResult);
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
                return null;
            }

            // POI 속성 등록
            if (!UploadPOIProperty(dicPOIIDs, strID, strKey))
                return null;

            return dicPOIIDs;
        }

        private bool UploadPOIProperty(Dictionary<POI, string> dicPOIIDs, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "convergence/fireEquipProp", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadPOIProperty Error : " + strErrorMessage);
                return false;
            }

            return true;
        }

        private bool UpdatePOIProperty(Dictionary<POI, string> dicPOIIDs, string strID, string strKey)
        {

            foreach (KeyValuePair<POI, string> pair in dicPOIIDs)
            {
                POI poi = pair.Key;
                string strPOIID = poi.XMLID;

                // POI 속성 조회
                List<string> poiPropList = GetPOIPropIDs(strPOIID, strID, strKey);

                if (poiPropList == null)
                    return false;

                // POI 속성 삭제
                if (!RemovePOIProps(strPOIID, poiPropList, strID, strKey))
                    return false;
            }

            // POI 속성 등록
            if (!UploadPOIProperty(dicPOIIDs, strID, strKey))
                return false;

            return true;
        }

        // Return 값
        //           Value : POI ID
        private Dictionary<Shapes.POI, string> UploadPOIs_NEW(Level level, string strBuildingID, string strLevelID, string strID, string strKey)
        {
            List<Shapes.POI> pois = level.POIs;
            string strLevelName = level.Name;

            // 노아서버 POI 조회
            List<string> POIList = GetPOIList(strBuildingID, strLevelID, strID, strKey);

            if (POIList == null)
                return null;

            // 삭제될 POI
            List<string> removePOIList = GetPOIList(strBuildingID, strLevelID, strID, strKey);

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
            if (RemovePOIs(removePOIList, strID, strKey) == false)
                return null;

            // 추가된 POI 노아서버에 추가 요청
            Dictionary<Shapes.POI, string> dicPOIs = UploadPOIs(addPOIList, strBuildingID, strLevelID, strID, strKey);

            if (dicPOIs == null)
                return null;

            // 수정된 POI 노아서버에 수정 요청
            if (UpdatePOIs(dicModifityPOIs, strBuildingID, strLevelID, strID, strKey) == false)
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

        // Return 값 : POI ID List
        private List<string> GetPOIList(string strBuildingID, string strLevelID, string strID, string strKey)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xBuildingID = MakeElement("paramBuildMngNo", strBuildingID);

            // 1 - 10000 페이지를 검색하면 대략 전체 조회가 가능함 (노아 쪽에서 검색 조건을 수정해주지 않는 이상은 임시로 사용)
            XElement xPageNo = MakeElement("pageNo", "1");
            XElement xPageSize = MakeElement("pageSize", "10000");  

            XElement xRoot = new XElement("convergence");
            xRoot.Add(xState);
            xRoot.Add(xBuildingID);
            xRoot.Add(xPageNo);
            xRoot.Add(xPageSize);

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "convergence/fireEquipList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetPOIList Error : " + strErrorMessage);
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

        private bool RemovePOIs(List<string> POIIDs, string strID, string strKey)
        {
            foreach (string strPOIID in POIIDs)
            {
                // POI 속성 조회
                List<string> poiPropList = GetPOIPropIDs(strPOIID, strID, strKey);

                if (poiPropList == null)
                    return false;

                // POI 속성 삭제
                if (!RemovePOIProps(strPOIID, poiPropList, strID, strKey))
                    return false;


                if (!RemovePOI(strPOIID, strID, strKey))
                    return false;
            }

            return true;
        }

        private bool RemovePOI(string strPOIID, string strID, string strKey)
        {
            string strURL = string.Format("convergence/fireEquip/{0}/{1}/{2}", strID, strKey, strPOIID);
            string strErrorMessage;

            string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strErrorMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemovePOI Error : " + strErrorMessage);
                    return false;
                }
            }

            return true;
        }

        private List<string> GetPOIPropIDs(string strPOIID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "convergence/fireEquipPropList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetPOIPropIDs Error : " + strErrorMessage);
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

        private bool RemovePOIProps(string strColumnID, List<string> propIDs, string strID, string strKey)
        {
            foreach (string strPropID in propIDs)
            {
                string strURL = string.Format("convergence/fireEquipProp/{0}/{1}/{2}/{3}", strID, strKey, strColumnID, strPropID);
                string strErrorMessage;

                string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

                if (strResult.Length == 0)
                {
                    if (strErrorMessage != SUCCESS_CODE)
                    {
                        System.Diagnostics.Trace.WriteLine("RemovePOIProps Error : " + strErrorMessage);
                        return false;
                    }
                }
            }

            return true;
        }

        private bool UpdatePOIs(Dictionary<Shapes.POI, string> dicPOIs, string strBuildingID, string strLevelID, string strID, string strKey)
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

                string strErrorMessage;
                string strXML = xRoot.ToString();
                string strResult = SendQuery(strXML, "convergence/fireEquip", true, out strErrorMessage, "PUT");

                if (strResult.Length == 0)
                {
                    System.Diagnostics.Trace.WriteLine("UpdatePOIs Error : " + strErrorMessage);
                    return false;
                }

                XElement xml = XElement.Parse(strResult);

                if (xml == null)
                {
                    System.Diagnostics.Trace.WriteLine("UpdatePOIs Error2 : " + strResult);
                    return false;
                }
            }

            // POI 속성 수정
            if (!UpdatePOIProperty(dicPOIs, strID, strKey))
                return false;


            return true;
        }


        private List<string> GetPropIDs(string strColumnID, string strID, string strKey, string strPropName)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xProp = new XElement(strPropName + "Prop");

            XElement xID = MakeElement(strPropName + "_id", strColumnID);
            xProp.Add(xID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xProp);

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/" + strPropName + "PropList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine(strPropName + "GetProp Error : " + strErrorMessage);
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

        
        private bool RemoveProps(string strColumnID, List<string> propIDs, string strID, string strKey, string propName)
        {
            foreach (string strPropID in propIDs)
            {
                string strURL = string.Format("spatial/" + propName + "Prop/{0}/{1}/{2}/{3}", strID, strKey, strColumnID, strPropID);
                string strErrorMessage;

                string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

                if (strResult.Length == 0)
                {
                    if (strErrorMessage != SUCCESS_CODE)
                    {
                        System.Diagnostics.Trace.WriteLine(propName + "RemoveProps Error : " + strErrorMessage);
                        return false;
                    }
                }
            }

            return true;
        }

        private bool UploaBuildingProperty(List<Property> properties, string strID, string strKey)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            // 해당 컬럼 속성 조회
            List<string> columnPropList = GetBuildingPropIDs(m_updownBuildingKey, strID, strKey);

            if (columnPropList == null)
                return false;

            // 조회된 속성 삭제
            if (!RemoveProps(m_updownBuildingKey, columnPropList, m_updownLoginID, m_updownLoginKey, "building"))
                return false;

            XElement xColumn = new XElement("buildingPropList");
            XElement xType = new XElement("build_mng_no", m_updownBuildingKey);
            XElement xCode = new XElement("property_code", GetPropertyCode("건물ID"));
            XElement xValue = new XElement("property_value", m_updownBuildingKey);

            xColumn.Add(xType);
            xColumn.Add(xCode);
            xColumn.Add(xValue);

            xRoot.Add(xColumn);

            // 현재 컬럼 속성 등록
            foreach (Property prop in properties)
            {
                if (prop.Name == "건물ID")
                    continue;

                xType = new XElement("build_mng_no", m_updownBuildingKey);
                xCode = new XElement("property_code", GetPropertyCode(prop.Name));
                xValue = new XElement("property_value", prop.Value);

                xColumn.Add(xType);
                xColumn.Add(xCode);
                xColumn.Add(xValue);

                xRoot.Add(xColumn);
            }

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/buildingProp", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploaBuildingProperty Error : " + strErrorMessage);
                return false;
            }

            return true;
        }

        private List<string> GetBuildingPropIDs(string strBuildingID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/buildingPropList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetBuildingPropIDs Error : " + strErrorMessage);
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

        private void GetGrpCodeIDs(string strID, string strKey)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xGrpCode = MakeElement("paramGrpCode", "PROP_TYP");

            XElement xRoot = new XElement("common");
            xRoot.Add(xState);
            xRoot.Add(xGrpCode);

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "code/sysCodeList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetGrpCodeIDs Error : " + strErrorMessage);
                return;
            }

            XElement xml = XElement.Parse(strResult);

            m_strPropTable.Clear();

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

                    m_strPropTable[strTitle] = strCode;
                }
            }

            return;
        }

        
        
        private void WriteXML(string strBaseFolder, Level level, string strXML, string strErrorMessage, bool request)
        {
            string strPath = strBaseFolder + "\\" + level.Name;

            if (Directory.Exists(strPath) == false)
                Directory.CreateDirectory(strPath);

            if (request)
                strPath += "\\request.xml";
            else
                strPath += "\\response.txt";

            StreamWriter writer = new StreamWriter(strPath, false, Encoding.UTF8);
            writer.Write(strXML);

            if (strErrorMessage != null && strErrorMessage.Length > 0)
            {
                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine("[ErrorMessage]");
                writer.WriteLine(strErrorMessage);
            }

            writer.Close();
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

        private string GetDoubleString(double data)
        {
            return string.Format("{0:" + m_strDoubleFormat + "}", data);
        }

        private string UploadProject(Project project, string strID, string strKey, string strBuildingKey)
        {
            // return "34";
            return strBuildingKey;
        }

        private string ReadProject(Project project, string strID, string strKey, string strBuildingKey)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            //  XElement xBuildingID = MakeElement("build_mng_no", "34");
            XElement xBuildingID = MakeElement("build_mng_no", strBuildingKey);

            XElement xBuilding = new XElement("building");
            xBuilding.Add(xBuildingID);
            
            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xBuilding);

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/spatialDetail2", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("ReadProject Error : " + strErrorMessage);
                return null;
            }
            
            return strResult;
        }

        public bool Login(out string strID, out string strKey)
        {
            strID = strKey = "";

            string resResult = string.Empty;
            string strURL = BaseAddress + API + "/login";

            string strXML = XML_HEADER;
            strXML += "<login>";
            strXML += "<user_id>" + m_strID + "</user_id>";
            strXML += "<user_pwd>" + m_strPW + "</user_pwd>";
            strXML += "</login>";

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

                return CheckLoginResult(resResult, out strID, out strKey);
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("Login Fail : " + ex.Message);
            }

            return false;
        }

        private bool CheckLoginResult(string strXML, out string strID, out string strKey)
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
                return false;

            if (ResultCode.SetCodeType(code.Value) != ResultCode.CodeType.Success)
                return false;

            strID = id.Value;
            strKey = key.Value;
            return true;
        }

        //int m_nQueryCount = 0;

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
            request.Timeout *= 10;

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

                /*{
                    StreamWriter writer = new StreamWriter(string.Format("C:/temp/request_{0}.xml", m_nQueryCount), false, Encoding.UTF8);
                    writer.WriteLine("[" + strMethodType + "]");
                    writer.WriteLine(url);
                    writer.WriteLine();

                    if (strXML != null)
                        writer.Write(strXML);

                    writer.Close();
                }*/

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
