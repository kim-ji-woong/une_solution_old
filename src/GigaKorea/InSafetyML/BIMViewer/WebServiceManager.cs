using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public enum WorkingType { Fail = -1, Working = 1, Success };

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

            //GetGrpCodeIDs(m_updownLoginID, m_updownLoginKey);
            /*
            m_strPropTable.Add(        "벽체정보", "PROP_001");
            m_strPropTable.Add("강성비정형성여부", "PROP_002");
            m_strPropTable.Add("필로티구조물여부", "PROP_003");
            m_strPropTable.Add(            "업종", "PROP_004");
            m_strPropTable.Add(            "재질", "PROP_005");
            m_strPropTable.Add(          "마감재", "PROP_006");
            m_strPropTable.Add(      "방화문유무", "PROP_007");
            m_strPropTable.Add(    "방화구역유무", "PROP_008");
            m_strPropTable.Add(           "Thick", "PROP_009");
            m_strPropTable.Add(          "Height", "PROP_010");
            */
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
        private int m_nUpDownErrCode = 0;

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

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DateTime dtStart = m_ufrm.StartTime;
            DateTime dtEnd = DateTime.Now;

            TimeSpan tsWork = dtEnd - dtStart;
            string strWork = ChangeTimeToString(tsWork);

            Thread.Sleep(1000);
            m_ufrm.Close();
            m_ufrm.Dispose();
            m_ufrm = null;

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

            worker.Dispose();

            if (m_updownType == 0)
            {   // 선택된 프로젝트 리로드
                FormMain.Instance.ReloadProject();
            }
            
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            m_ufrm.SetProgress(e.UserState.ToString(), e.ProgressPercentage);
        }

        private bool UploadLevelComponent(Level level, string strLevelID)
        {
            Dictionary<Wall, string> dicGridIDs = UploadGrids_NEW(level.Walls, strLevelID, m_updownLoginID, m_updownLoginKey);

            if (dicGridIDs == null)
                return false;

            Dictionary<Wall, string> dicWallIDs = UploadWalls_NEW(dicGridIDs, strLevelID, m_updownLoginID, m_updownLoginKey);

            if (dicWallIDs == null)
                return false;

            if (!UploadWallBoundarys(dicWallIDs, strLevelID, m_updownLoginID, m_updownLoginKey))
                return false;


            if (UploadDoors_NEW(dicWallIDs, strLevelID, m_updownLoginID, m_updownLoginKey) == false)
                return false;

            if (UploadWindows_NEW(dicWallIDs, strLevelID, m_updownLoginID, m_updownLoginKey) == false)
                return false;








            Dictionary<Space, string> dicSpaceIDs = UploadSpaces_NEW(level.Spaces, strLevelID, m_updownLoginID, m_updownLoginKey);

            if (dicSpaceIDs == null)
                return false;

            if (UploadSpaceWallLink(dicSpaceIDs, dicWallIDs, strLevelID, m_updownLoginID, m_updownLoginKey) == false)
                return false;

            if (!UploadSpaceBoundarys(dicSpaceIDs, strLevelID, m_updownLoginID, m_updownLoginKey))
                return false;







            Dictionary<AlertArea, string> dicAlertAreaIDs = UploadAlertAreas_NEW(level.AlertAreas, strLevelID, m_updownLoginID, m_updownLoginKey);

            if (dicAlertAreaIDs == null)
                return false;

            if (!UploadAlertAreaBoundarys(dicAlertAreaIDs, strLevelID, m_updownLoginID, m_updownLoginKey))
                return false;






            

            if (UploadColumns_NEW(strLevelID, level.Columns, m_updownLoginID, m_updownLoginKey) == false)
                return false;






            Dictionary<Topology, string> dicTopologyIDs = UploadTopologies_NEW(level.Topologies, strLevelID, m_updownLoginID, m_updownLoginKey);

            if (dicTopologyIDs == null)
                return false;

            Dictionary<Topology.Node, string> dicTopologyNodeIDs = UploadTopologyNodes(dicTopologyIDs, m_updownLoginID, m_updownLoginKey);

            if (dicTopologyNodeIDs == null)
                return false;

            if (UploadTopologyNodeLinks(dicTopologyNodeIDs, m_updownLoginID, m_updownLoginKey) == false)
                return false;








            //poi데이터 업로드.
            Dictionary<Shapes.POI, string> dicPOIIDs = UploadPOIs_NEW(level, m_updownBuildingKey, strLevelID, m_updownLoginID, m_updownLoginKey);

            if (dicPOIIDs == null)
                return false;

            if (UploadPOIWires_NEW(level.Wires, dicPOIIDs, level, strLevelID, m_updownLoginID, m_updownLoginKey) == false)
                return false;

            return true;
        }

        private bool UploadNewLevel(Level level, string strLevelID)
        {
            Dictionary<Wall, string> dicGridIDs = UploadGrids(level.Walls, strLevelID, m_updownLoginID, m_updownLoginKey);

            if (dicGridIDs == null)
                return false;

            Dictionary<Wall, string> dicWallIDs = UploadWalls(dicGridIDs, strLevelID, m_updownLoginID, m_updownLoginKey);

            if (dicWallIDs == null)
                return false;

            if (!UploadWallBoundarys(dicWallIDs, strLevelID, m_updownLoginID, m_updownLoginKey))
                return false;

            Dictionary<Space, string> dicSpaceIDs = UploadSpaces(level.Spaces, strLevelID, m_updownLoginID, m_updownLoginKey);

            if (dicSpaceIDs == null)
                return false;

            if (UploadSpaceWallLink(dicSpaceIDs, dicWallIDs, strLevelID, m_updownLoginID, m_updownLoginKey) == false)
                return false;

            if (!UploadSpaceBoundarys(dicSpaceIDs, strLevelID, m_updownLoginID, m_updownLoginKey))
                return false;

            // 경계구역 업로드
            Dictionary<AlertArea, string> dicAlertAreaIDs = UploadAlertAreas(level.AlertAreas, strLevelID, m_updownLoginID, m_updownLoginKey);

            if (dicAlertAreaIDs == null)
                return false;

            if (!UploadAlertAreaBoundarys(dicAlertAreaIDs, strLevelID, m_updownLoginID, m_updownLoginKey))
                return false;

            if (UploadDoors(dicWallIDs, strLevelID, m_updownLoginID, m_updownLoginKey) == false)
                return false;

            if (UploadWindows(dicWallIDs, strLevelID, m_updownLoginID, m_updownLoginKey) == false)
                return false;

            if (UploadColumns(strLevelID, level.Columns, m_updownLoginID, m_updownLoginKey) == false)
                return false;

            Dictionary<Topology, string> dicTopologyIDs = UploadTopologies(level.Topologies, strLevelID, m_updownLoginID, m_updownLoginKey);

            if (dicTopologyIDs == null)
                return false;

            Dictionary<Topology.Node, string> dicTopologyNodeIDs = UploadTopologyNodes(dicTopologyIDs, m_updownLoginID, m_updownLoginKey);

            if (dicTopologyNodeIDs == null)
                return false;

            if (UploadTopologyNodeLinks(dicTopologyNodeIDs, m_updownLoginID, m_updownLoginKey) == false)
                return false;

            // 새로운 POI 코드를 넣어야 동작함.
            //poi데이터 업로드.
            Dictionary<Shapes.POI, string> dicPOIIDs = UploadPOIs(level.POIs, m_updownBuildingKey, strLevelID, m_updownLoginID, m_updownLoginKey);

            if (dicPOIIDs == null)
                return false;

            if (UploadPOIWires(level.Wires, dicPOIIDs, level, strLevelID, m_updownLoginID, m_updownLoginKey) == false)
                return false;

            return true;
        }


        private void Worker_DoUpLoad_Work(object sender, DoWorkEventArgs e)
        {
            // 업로드 기능 수정중
            m_bUpDownResult = false;
            m_nUpDownErrCode = 1;

            // 상세속성 조회
            //GetGrpCodeIDs(m_updownLoginID, m_updownLoginKey);
            // TODO: 상세속성 등록
            //AddPropertyCode(m_updownLoginID, m_updownLoginKey, "PROP_023", "PROP_TYP", "UTM_K", "AnchorNode 기준점");
            // TODO: REST API 권한 추가
            //InsertRestRole(m_updownLoginID, m_updownLoginKey, "UNE_SPACE", "TP4_099");

            // InSafetyML 업로드 진행사항
            // 프로젝트 ID 체크
            // XML 속성에 프로젝트 ID 얻어오기
            string strProjectID = GetProjectID();

            if (strProjectID == null)
            {
                // 프로젝트 ID가 없을 경우 새로 업로드 여부 확인 뒤 업로드 진행
                if (MessageBox.Show("주소지 건물에 대한 새로운 도면입니다. 공간정보를 업로드 하시겠습니까?", "확인", MessageBoxButtons.YesNo)
                    == System.Windows.Forms.DialogResult.Yes)
                {
                    // 새 업로드
                    if (!UploadNewProject())
                        return;
                    else
                    {
                        // 노아서버 XML 다운받기 (서버 XML과 동기화)
                        if (!DownloadXML())
                            return;

                        m_bUpDownResult = true;
                        return;
                    }
                }
                else
                {
                    m_nUpDownErrCode = 0;
                    return;
                }
            }
            else if (m_updownBuildingKey != strProjectID)
            {
                // 프로젝트 ID가 다를 경우 주소지에 맞는 정보가 아님을 표시 후 취소
                MessageBox.Show("주소지에 맞는 공간정보가 아닙니다.", "취소", MessageBoxButtons.OK);
                m_nUpDownErrCode = 0;
                return;
            }


            // 프로젝트 ID가 같을 경우
            // level 체크
            List<Level> levels = m_updownProject.Levels;
            Dictionary<string, string> dicRemoveLevel = new Dictionary<string, string>();

            // Level ID 수정(변경) 유무
            bool bCheck = false;

            // 노아서버 Level과 XML Level 비교 
            bCheck = CheckModifity(levels, out dicRemoveLevel);

            if (dicRemoveLevel.Count == 0 && bCheck == false)
            {
                // level ID 같을 경우 수정된 내용이 없음!!
                MessageBox.Show("수정된 내용이 없습니다.", "확인", MessageBoxButtons.OK);
                m_nUpDownErrCode = 0;
                return;

                //// level 같을 경우 수정 여부 확인 뒤, 속성값 수정
                //if (MessageBox.Show("수정하시겠습니까?", "확인", MessageBoxButtons.YesNo)
                //    == System.Windows.Forms.DialogResult.Yes)
                //{
                //    // 수정된 속성(project property, Space property) 값들 업로드
                //    if (!UploadModifityProperty())
                //        return;
                //}
                //else
                //{
                //    m_nUpDownErrCode = 0;
                //    return;
                //}
            }
            else if (dicRemoveLevel.Count != 0 || bCheck == true)
            {
                // level 다를 경우 도면 수정 여부를 확인 뒤 
                if (MessageBox.Show("같은 주소지 건물에 도면수정이 되었습니다. 공간정보를 수정하시겠습니까?", "확인", MessageBoxButtons.YesNo)
                    == System.Windows.Forms.DialogResult.Yes)
                {
                    // 수정할 레벨 수정
                    if (!UploadModifityLevel(dicRemoveLevel))
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

        private bool UploadNewProject()
        {
            SetDoubleString(m_updownProject);

            if (m_updownBuildingKey == null)
                return false;

            // 프로젝트 앵커노드 업로드
            AnchorNode anchor = m_updownProject.AnchorNode;
            if (!UploaBuildingAnchorNode(anchor, m_updownLoginID, m_updownLoginKey))
                return false;

            // 프로젝트 속성 업로드
            List<Property> properties = m_updownProject.Properties;
            if (!UploaBuildingProperty(properties, m_updownLoginID, m_updownLoginKey))
                return false;

            Dictionary<string, string> dicRemoveLevels = ReadLevelNames(m_updownBuildingKey, m_updownLoginID, m_updownLoginKey);

            int i = 1;
            int lvcnt = m_updownProject.Levels.Count + dicRemoveLevels.Count;
            double percent = (double)i;

            foreach (KeyValuePair<string, string> pair in dicRemoveLevels)
            {
                string strLevelID = pair.Key;
                string strLevelName = pair.Value;

                if (RemoveLevelComponent(strLevelID, m_updownLoginID, m_updownLoginKey) == false)
                    return false;

                //Progerssbar...
                percent = (double)i;
                percent = percent / (double)lvcnt;
                percent = percent * 100;
                percent = (int)percent;
                string sMessage = "기존의 " + strLevelName + " 층 정리중 : " + percent.ToString() + " %";
                worker.ReportProgress((int)percent, sMessage);
                i++;
            }

            Dictionary<Level, string> dicLevels = UploadLevels(m_updownBuildingKey, m_updownProject.Levels, m_updownLoginID, m_updownLoginKey);

            if (dicLevels == null)
                return false;

            foreach (KeyValuePair<Level, string> pair in dicLevels)
            {
                Level level = pair.Key;

                if (!UploadNewLevel(level, pair.Value))
                    return false;

                //Progerssbar...
                percent = (double)i;
                percent = percent / (double)lvcnt;
                percent = percent * 100;
                percent = (int)percent;
                string sMessage = pair.Key.Name + " 층 업로드중 : " + percent.ToString() + " %";
                worker.ReportProgress((int)percent, sMessage);
                i++;
            }

            return true;
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
                else
                {
                    // 새로 추가된 층
                    bRetModifity = true;
                }
            }

            return bRetModifity;
        }

        private bool UploadModifityProperty()
        {   // 수정된 Project property, Space property 업로드 

            List<Level> levels = m_updownProject.Levels;
            List<Property> properties = m_updownProject.Properties;

            // 프로젝트 속성 업로드
            if (!UploaBuildingProperty(properties, m_updownLoginID, m_updownLoginKey))
                return false;

            int i = 1;
            int lvcnt = levels.Count;

            // 공간 속성 업로드
            foreach (Level level in levels)
            {
                // TODO: 공간 속성정보, 경계구역 속성정보 변경 유무 체크
                // 공간 속성정보체크







                Dictionary<Space, string> dicSpaceIDs = UploadSpaces_NEW(level.Spaces, level.XMLID, m_updownLoginID, m_updownLoginKey);

                if (dicSpaceIDs == null)
                    return false;

                // 벽체와 공간 링크를 위해서 
                Dictionary<Wall, string> dicWallIDs = new Dictionary<Wall, string>();

                foreach (Wall wall in level.Walls)
                {
                    dicWallIDs[wall] = wall.XMLID;
                }

                if (UploadSpaceWallLink(dicSpaceIDs, dicWallIDs, level.XMLID, m_updownLoginID, m_updownLoginKey) == false)
                    return false;

                if (!UploadSpaceBoundarys(dicSpaceIDs, level.XMLID, m_updownLoginID, m_updownLoginKey))
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

        private bool CheckSpaceProp(Level level)
        {
            List<Space> listSpaces = level.Spaces;

            foreach (Space space in listSpaces)
            {
                string strSpaceID = space.XMLID;
                List<Property> spaceProps = space.Properties;

                List<Property> listProps = GetProps(strSpaceID, m_updownLoginID, m_updownLoginKey, "space");

                
            }

            return true;
        }

        private List<Property> GetProps(string strColumnID, string strID, string strKey, string strPropName)
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

            List<Property> props = new List<Property>();

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == (strPropName + "PropList") ? element : null;
                Property prop = new Property();

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "property_code")
                        {
                            //propIDs.Add(child.Value);
                            //break;
                            // TODO: 코드가 아닌 이름 값으로 변환이 필요!!
                            prop.Name = child.Value;
                        }
                        else if (child.Name == "property_value")
                        {
                            prop.Value = child.Value;
                        }
                    }

                    props.Add(prop);
                }
            }

            return props;
        }

        private bool UploadModifityLevel(Dictionary<string, string> dicRemoveLevel)
        {
            List<Property> properties = m_updownProject.Properties;
            List<Level> levels = m_updownProject.Levels;
            //List<string> listRemoveLevelIDs = new List<string>(dicRemoveLevel.Keys);
            Dictionary<string, string> dicLevel = ReadLevelNames(m_updownBuildingKey, m_updownLoginID, m_updownLoginKey);

            int i = 1;
            int lvcnt = levels.Count + dicRemoveLevel.Count;
            double percent = (double)i;


            // 삭제할 레벨 삭제
            foreach (KeyValuePair<string, string> pair in dicRemoveLevel)
            {
                string strLevelID = pair.Key;
                string strLevelName = pair.Value;

                if (RemoveLevelComponent(strLevelID, m_updownLoginID, m_updownLoginKey) == false)
                    return false;

                //Progerssbar...
                percent = (double)i;
                percent = percent / (double)lvcnt;
                percent = percent * 100;
                percent = (int)percent;
                string sMessage = "기존의 " + strLevelName + " 층 정리중 : " + percent.ToString() + " %";
                worker.ReportProgress((int)percent, sMessage);
                i++;
            }

            // 프로젝트 앵커노드 업로드
            AnchorNode anchor = m_updownProject.AnchorNode;
            if (!UploaBuildingAnchorNode(anchor, m_updownLoginID, m_updownLoginKey))
                return false;

            // 프로젝트 속성 업로드
            if (!UploaBuildingProperty(properties, m_updownLoginID, m_updownLoginKey))
                return false;

            foreach (Level level in levels)
            {
                string strLevelID = level.XMLID;

                if (dicLevel.ContainsKey(level.XMLID))
                {
                    //// 기존 Level ID가 있을 경우 공간 속성만 업로드
                    //Dictionary<Space, string> dicSpaceIDs = UploadSpaces_NEW(level.Spaces, level.XMLID, m_updownLoginID, m_updownLoginKey);

                    //if (dicSpaceIDs == null)
                    //    return false;

                    //// 벽체와 공간 링크를 위해서 
                    //Dictionary<Wall, string> dicWallIDs = new Dictionary<Wall, string>();

                    //foreach (Wall wall in level.Walls)
                    //{
                    //    dicWallIDs[wall] = wall.XMLID;
                    //}

                    //if (UploadSpaceWallLink(dicSpaceIDs, dicWallIDs, level.XMLID, m_updownLoginID, m_updownLoginKey) == false)
                    //    return false;

                    //if (!UploadSpaceBoundarys(dicSpaceIDs, level.XMLID, m_updownLoginID, m_updownLoginKey))
                    //    return false;

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
                    if (!UploadLevelComponent(level, strLevelID))
                        return false;
                }
                else
                {
                    // level을 업로드 후에 xmlID를 새로 갱신 후에 진행
                    strLevelID = UploadLevel(m_updownBuildingKey, level, m_updownLoginID, m_updownLoginKey);

                    // 새로 추가된 Level일 경우 새로 갱신
                    if (!UploadNewLevel(level, strLevelID))
                        return false;
                }

                //Progerssbar...
                percent = (double)i;
                percent = percent / (double)lvcnt;
                percent = percent * 100;
                percent = (int)percent;
                string sMessage = level.Name + " 층 업로드중 : " + percent.ToString() + " %";
                worker.ReportProgress((int)percent, sMessage);
                i++;
            }

            return true;
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

            xmlDoc.Save(m_downFileName);

            m_bUpDownResult = true;            
        }

        public void Upload(Project project, Dictionary<int, POIType> dicPOITypes, string strBuildingKey, string strID, string strKey)
        {
            m_updownType = 0;
            m_updownProject = project;
            m_updownDicPOITypes = dicPOITypes;
            m_updownBuildingKey = strBuildingKey;
            m_updownLoginID = strID;
            m_updownLoginKey = strKey;

            GetGrpCodeIDs(m_updownLoginID, m_updownLoginKey);

            DateTime time = DateTime.Now;
            time.ToString();

            m_ufrm = new uProgressForm(0);//0 is Uploading
            m_ufrm.StartPosition = FormStartPosition.CenterParent;
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoUpLoad_Work;
            worker.ProgressChanged += Worker_ProgressChanged;
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
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
            m_ufrm.ShowDialog();//modal
        }


        private void RemoveGridThread(out WorkingType chkGrid, string strLevelID, string strID, string strKey)
        {
            // 벽체선형 조회
            List<string> gridList = GetGridList(strLevelID, strID, strKey);
            if (gridList == null)
            {
                chkGrid = WorkingType.Fail;
                return;
            }

            // 조회된 벽체선형 삭제
            if (RemoveGrids(gridList, strLevelID, strID, strKey) == false)
            {
                chkGrid = WorkingType.Fail;
                return;
            }

            chkGrid = WorkingType.Success;
        }

        private void RemoveDoorThread(out WorkingType chkDoor, string strLevelID, string strID, string strKey)
        {
            // 문 조회
            List<string> doorList = GetDoorList(strLevelID, strID, strKey);
            if (doorList == null)
            {
                chkDoor = WorkingType.Fail;
                return;
            }

            // 문 삭제 요청
            if (RemoveDoors(doorList, strLevelID, strID, strKey) == false)
            {
                chkDoor = WorkingType.Fail;
                return;
            }

            chkDoor = WorkingType.Success;
        }

        private void RemoveWindowThread(out WorkingType chkWindow, string strLevelID, string strID, string strKey)
        {
            // 창문 조회
            List<string> windowList = GetWindowList(strLevelID, strID, strKey);
            if (windowList == null)
            {
                chkWindow = WorkingType.Fail;
                return;
            }

            // 창문 삭제 요청
            if (RemoveWindows(windowList, strLevelID, strID, strKey) == false)
            {
                chkWindow = WorkingType.Fail;
                return;
            }

            chkWindow = WorkingType.Success;
        }

        private void RemoveWallThread(out WorkingType chkWall, string strLevelID, string strID, string strKey)
        {
            // 벽 조회
            List<string> wallList = GetWallList(strLevelID, strID, strKey);
            if (wallList == null)
            {
                chkWall = WorkingType.Fail;
                return;
            }

            // 벽 삭제 요청
            if (RemoveWalls(wallList, strLevelID, strID, strKey) == false)
            {
                chkWall = WorkingType.Fail;
                return;
            }

            chkWall = WorkingType.Success;
        }

        private void RemoveSpaceThread(out WorkingType chkSpace, string strLevelID, string strID, string strKey)
        {
            // 공간 조회
            List<string> spaceList = GetSpaceList(strLevelID, strID, strKey);
            if (spaceList == null)
            {
                chkSpace = WorkingType.Fail;
                return;
            }

            // 공간 삭제
            if (RemoveSpaces(spaceList, strLevelID, strID, strKey) == false)
            {
                chkSpace = WorkingType.Fail;
                return;
            }

            chkSpace = WorkingType.Success;
        }

        private void RemoveAlertAreaThread(out WorkingType chkAlertArea, string strLevelID, string strID, string strKey)
        {
            // 경계구역 조회
            List<string> alertAreaList = GetAlertAreaList(strLevelID, strID, strKey);
            if (alertAreaList == null)
            {
                chkAlertArea = WorkingType.Fail;
                return;
            }

            // 경계구역 삭제
            if (RemoveAlertAreas(alertAreaList, strLevelID, strID, strKey) == false)
            {
                chkAlertArea = WorkingType.Fail;
                return;
            }

            chkAlertArea = WorkingType.Success;
        }

        private void RemoveColumnThread(out WorkingType chkColumn, string strLevelID, string strID, string strKey)
        {
            // Column 조회
            List<string> columnList = GetColumnList(strLevelID, strID, strKey);
            if (columnList == null)
            {
                chkColumn = WorkingType.Fail;
                return;
            }

            // Column 삭제 
            if (RemoveColumns(columnList, strLevelID, strID, strKey) == false)
            {
                chkColumn = WorkingType.Fail;
                return;
            }

            chkColumn = WorkingType.Success;
        }

        private void RemoveTopologyNodeLinkThread(out WorkingType chkTopologyNodeLink, string strLevelID, string strID, string strKey)
        {
            // TopologyNodeLink 조회
            List<string> topologyNodeLinkList = GetTopologyNodeLinkList(strLevelID, strID, strKey);
            if (topologyNodeLinkList == null)
            {
                chkTopologyNodeLink = WorkingType.Fail;
                return;
            }

            // TopologyNodeLink 삭제 요청
            if (RemoveTopologeNodeLinks(topologyNodeLinkList, strLevelID, strID, strKey) == false)
            {
                chkTopologyNodeLink = WorkingType.Fail;
                return;
            }

            chkTopologyNodeLink = WorkingType.Success;
        }

        private void RemoveTopologyNodeThread(out WorkingType chkTopologyNode, string strLevelID, string strID, string strKey)
        {
            // TopologyNode 조회
            List<string> topologyNodeList = GetTopologyNodeList(strLevelID, strID, strKey);
            if (topologyNodeList == null)
            {
                chkTopologyNode = WorkingType.Fail;
                return;
            }

            // TopologyNode 삭제 요청
            if (RemoveTopologeNodes(topologyNodeList, strID, strKey) == false)
            {
                chkTopologyNode = WorkingType.Fail;
                return;
            }

            chkTopologyNode = WorkingType.Success;
        }


        private void RemoveTopologyThread(out WorkingType chkTopology, string strLevelID, string strID, string strKey)
        {
            // Topology 조회
            List<string> topologyList = GetTopologyList(strLevelID, strID, strKey);
            if (topologyList == null)
            {
                chkTopology = WorkingType.Fail;
                return;
            }

            // Topology 삭제 요청
            if (RemoveTopologes(topologyList, strLevelID, strID, strKey) == false)
            {
                chkTopology = WorkingType.Fail;
                return;
            }

            chkTopology = WorkingType.Success;
        }

        private void RemovePOIWireThread(out WorkingType chkPOIWire, string strLevelID, string strID, string strKey)
        {
            // POIWire 조회
            List<string> poiWireList = GetPOIWireList(strLevelID, strID, strKey);
            if (poiWireList == null)
            {
                chkPOIWire = WorkingType.Fail;
                return;
            }

            // POIWire 삭제 요청
            if (RemovePOIWires(poiWireList, strLevelID, strID, strKey) == false)
            {
                chkPOIWire = WorkingType.Fail;
                return;
            }

            chkPOIWire = WorkingType.Success;
        }

        private void RemovePOIThread(out WorkingType chkPOI, string strLevelID, string strID, string strKey)
        {
            // POI 조회
            List<string> POIList = GetPOIList(m_updownBuildingKey, strLevelID, strID, strKey);
            if (POIList == null)
            {
                chkPOI = WorkingType.Fail;
                return;
            }

            // POI 삭제 요청
            if (RemovePOIs(POIList, strID, strKey) == false)
            {
                chkPOI = WorkingType.Fail;
                return;
            }

            chkPOI = WorkingType.Success;
        }

        /*
        private bool RemoveLevelComponent(string strLevelID, string strID, string strKey)
        {
            WorkingType chkGrid = WorkingType.Working;
            Thread removeGridThread = new Thread(() => RemoveGridThread(out chkGrid, strLevelID, strID, strKey));
            removeGridThread.Start();


            WorkingType chkDoor = WorkingType.Working;
            Thread removeDoorThread = new Thread(() => RemoveDoorThread(out chkDoor, strLevelID, strID, strKey));
            removeDoorThread.Start();


            WorkingType chkWindow = WorkingType.Working;
            Thread removeWindowThread = new Thread(() => RemoveWindowThread(out chkWindow, strLevelID, strID, strKey));
            removeWindowThread.Start();


            WorkingType chkWall = WorkingType.Working;
            Thread removeWallThread = new Thread(() => RemoveWallThread(out chkWall, strLevelID, strID, strKey));
            removeWallThread.Start();


            WorkingType chkSpace = WorkingType.Working;
            Thread removeSpaceThread = new Thread(() => RemoveSpaceThread(out chkSpace, strLevelID, strID, strKey));
            removeSpaceThread.Start();


            WorkingType chkAlertArea = WorkingType.Working;
            Thread removeAlertAreaThread = new Thread(() => RemoveAlertAreaThread(out chkAlertArea, strLevelID, strID, strKey));
            removeAlertAreaThread.Start();


            WorkingType chkColumn = WorkingType.Working;
            Thread removeColumnThread = new Thread(() => RemoveColumnThread(out chkColumn, strLevelID, strID, strKey));
            removeColumnThread.Start();


            WorkingType chkTopologyNodeLink = WorkingType.Working;
            Thread removeTopologyNodeLinkThread = new Thread(() => RemoveTopologyNodeLinkThread(out chkTopologyNodeLink, strLevelID, strID, strKey));
            removeTopologyNodeLinkThread.Start();


            WorkingType chkTopologyNode = WorkingType.Working;
            Thread removeTopologyNode = new Thread(() => RemoveTopologyNodeThread(out chkTopologyNode, strLevelID, strID, strKey));
            removeTopologyNode.Start();


            WorkingType chkTopology = WorkingType.Working;
            Thread removeTopology = new Thread(() => RemoveTopologyThread(out chkTopology, strLevelID, strID, strKey));
            removeTopology.Start();


            WorkingType chkPOIWire = WorkingType.Working;
            Thread removePOIWire = new Thread(() => RemovePOIWireThread(out chkPOIWire, strLevelID, strID, strKey));
            removePOIWire.Start();


            WorkingType chkPOI = WorkingType.Working;
            Thread removePOI = new Thread(() => RemovePOIThread(out chkPOI, strLevelID, strID, strKey));
            removePOI.Start();

            
            WorkingType chkThread = WorkingType.Working;
            
            while (chkThread == WorkingType.Working)
            {
                if (chkGrid == WorkingType.Success && chkDoor == WorkingType.Success && chkWindow == WorkingType.Success && chkWall == WorkingType.Success && chkSpace == WorkingType.Success
                     && chkAlertArea == WorkingType.Success && chkColumn == WorkingType.Success && chkTopologyNodeLink == WorkingType.Success && chkTopologyNode == WorkingType.Success
                      && chkTopology == WorkingType.Success && chkPOIWire == WorkingType.Success && chkPOI == WorkingType.Success)
                {
                    chkThread = WorkingType.Success;
                }
                else if (chkGrid == WorkingType.Fail || chkDoor == WorkingType.Fail || chkWindow == WorkingType.Fail || chkWall == WorkingType.Fail || chkSpace == WorkingType.Success
                     && chkAlertArea == WorkingType.Fail || chkColumn == WorkingType.Fail || chkTopologyNodeLink == WorkingType.Fail || chkTopologyNode == WorkingType.Success
                      && chkTopology == WorkingType.Fail || chkPOIWire == WorkingType.Fail || chkPOI == WorkingType.Fail)
                {
                    chkThread = WorkingType.Fail;
                }

                Thread.Sleep(1 * 1000);
            }

            if (chkThread == WorkingType.Fail)
                return false;

            // 레벨 속성 조회
            List<string> levelPropList = GetPropIDs(strLevelID, strID, strKey, "level");

            if (levelPropList == null)
                return false;

            // 레벨 속성 삭제
            if (!RemoveProps(strLevelID, levelPropList, strID, strKey, "level"))
                return false;

            // 최종 레벨 삭제
            if (!RemoveLevel(strLevelID, strID, strKey))
                return false;

            return true;

            // .TODO: 삭제 작업속도 업그레이드 방안
            // 각각의 삭제 스레드로 진행 >> 진행 상황을 체크하기 위한 매게변수 전달

            // 매게변수로 진행 상황을 체크
            // 다 완료되면 true
            // 하나라도 실패하면 fail
        }
        */

        
        private bool RemoveLevelComponent(string strLevelID, string strID, string strKey)
        {
            // 벽체선형 조회
            List<string> gridList = GetGridList(strLevelID, strID, strKey);

            // 조회된 벽체선형 삭제
            if (RemoveGrids(gridList, strLevelID, strID, strKey) == false)
                return false;

            // 문 조회
            List<string> doorList = GetDoorList(strLevelID, strID, strKey);

            // 문 삭제 요청
            if (RemoveDoors(doorList, strLevelID, strID, strKey) == false)
                return false;

            // 창문 조회
            List<string> windowList = GetWindowList(strLevelID, strID, strKey);

            // 창문 삭제 요청
            if (RemoveWindows(windowList, strLevelID, strID, strKey) == false)
                return false;

            // 벽 조회
            List<string> wallList = GetWallList(strLevelID, strID, strKey);

            // 벽 삭제 요청
            if (RemoveWalls(wallList, strLevelID, strID, strKey) == false)
                return false;

            // 공간 조회
            List<string> spaceList = GetSpaceList(strLevelID, strID, strKey);

            // 공간 삭제
            if (RemoveSpaces(spaceList, strLevelID, strID, strKey) == false)
                return false;

            // 경계구역 조회
            List<string> alertAreaList = GetAlertAreaList(strLevelID, strID, strKey);

            // 경계구역 삭제
            if (RemoveAlertAreas(alertAreaList, strLevelID, strID, strKey) == false)
                return false;

            // Column 조회
            List<string> columnList = GetColumnList(strLevelID, strID, strKey);

            // Column 삭제 
            if (RemoveColumns(columnList, strLevelID, strID, strKey) == false)
                return false;

            // TopologyNodeLink 조회
            List<string> topologyNodeLinkList = GetTopologyNodeLinkList(strLevelID, strID, strKey);

            // TopologyNodeLink 삭제 요청
            if (RemoveTopologeNodeLinks(topologyNodeLinkList, strLevelID, strID, strKey) == false)
                return false;

            // TopologyNode 조회
            List<string> topologyNodeList = GetTopologyNodeList(strLevelID, strID, strKey);

            // TopologyNode 삭제 요청
            if (RemoveTopologeNodes(topologyNodeList, strID, strKey) == false)
                return false;

            // Topology 조회
            List<string> topologyList = GetTopologyList(strLevelID, strID, strKey);

            // Topology 삭제 요청
            if (RemoveTopologes(topologyList, strLevelID, strID, strKey) == false)
                return false;

            // POIWire 조회
            List<string> poiWireList = GetPOIWireList(strLevelID, strID, strKey);

            if (poiWireList == null)
                return false;

            // POIWire 삭제 요청
            if (RemovePOIWires(poiWireList, strLevelID, strID, strKey) == false)
                return false;

            // POI 조회
            List<string> POIList = GetPOIList(m_updownBuildingKey, strLevelID, strID, strKey);

            if (POIList == null)
                return false;

            // POI 삭제 요청
            if (RemovePOIs(POIList, strID, strKey) == false)
                return false;

            // 레벨 속성 조회
            List<string> levelPropList = GetPropIDs(strLevelID, strID, strKey, "level");

            if (levelPropList == null)
                return false;

            // 레벨 속성 삭제
            if (!RemoveProps(strLevelID, levelPropList, strID, strKey, "level"))
                return false;

            // 최종 레벨 삭제
            if (!RemoveLevel(strLevelID, strID, strKey))
                return false;

            return true;
        }
        


        private bool RemoveLevel(string levelID, string strID, string strKey)
        {
            string strURL = string.Format("spatial/level/{0}/{1}/{2}", strID, strKey, levelID);
            string strErrorMessage;

            string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strErrorMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveLevel Error : " + strErrorMessage);
                    return false;
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

        // Return 값 : Level ID List
        private List<string> ReadLevels(string strBuildingID, string strID, string strKey)
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

            foreach (XElement element in xml.Elements())
            {
                XElement xNode = element.Name == "levelList" ? element : null;

                if (xNode != null)
                {
                    foreach (XElement child in xNode.Elements())
                    {
                        if (child.Name == "level_id")
                        {
                            levelIDs.Add(child.Value);
                            break;
                        }
                    }
                }
            }

            return levelIDs;
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

                return;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("LoadDetailProp Fail : " + ex.Message);
            }
        }
 
        // wall type 코드 . WTP_04 추가함.
        public void AddHandrailType(string strID, string strKey)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("common");
            xRoot.Add(xState);

            XElement x1 = new XElement("codeSys");
            XElement x2 = MakeElement("sys_code", "WTP_04");
            XElement x3 = MakeElement("grp_code", "WAL_TYPE");
            XElement x4 = MakeElement("sys_title", "Handrail");
            XElement x5 = MakeElement("description", "철재");

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

        public bool InsertRestRole(string strID, string strKey, string strUserGroupID, string strRestCode)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("common");
            xRoot.Add(xState);

            XElement xParamRestRole = new XElement("paramRestRole", strUserGroupID);
            XElement xParamApiId = new XElement("paramApiId", strRestCode);

            xRoot.Add(xParamRestRole);
            xRoot.Add(xParamApiId);

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "code/insertRestRole", true, out strErrorMessage, "PUT");

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("InsertRestRole Error : " + strErrorMessage);
                return false;
            }

            return true;
        }

        public string CreateUser(string strID, string strKey, string user_pwd, string user_name, string user_phone, string user_roles)
        {
            string resResult = string.Empty;

            string strURL = BaseAddress + API + "/user";

            string strXML = XML_HEADER;
            strXML += "<common>";
            strXML += "<state>";
            strXML += "<user_id>" + strID + "</user_id>";
            strXML += "<key_id>" + strKey + "</key_id>";
            strXML += "</state>";
            strXML += "<user>";
            strXML += "<user_pwd>" + user_pwd + "</user_pwd>";
            strXML += "<user_name>" + user_name + "</user_name>";
            strXML += "<user_phone>" + user_phone + "</user_phone>";
            strXML += "<user_roles>" + user_roles + "</user_roles>";
            strXML += "</user>";
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

                return resResult;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("LoadDetailProp Fail : " + ex.Message);
                return resResult;
            }
        }
        public string GetUserList(string strID, string strKey)
        {
            string resResult = string.Empty;

            string strURL = BaseAddress + API + "/userList";

            string strXML = XML_HEADER;
            strXML += "<common>";
            strXML += "<state>";
            strXML += "<user_id>" + strID + "</user_id>";
            strXML += "<key_id>" + strKey + "</key_id>";
            strXML += "</state>";
            strXML += "<user>";            
            strXML += "</user>";
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

                return resResult;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("LoadDetailProp Fail : " + ex.Message);
                return resResult;
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

        private string GetPropertyName(string strPropertyCode)
        {
            foreach (KeyValuePair<string, string> pair in m_strPropTable)
            {
                if (pair.Value == strPropertyCode)
                    return pair.Key;
            }
            return "PROP_001";
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

                xColumn = new XElement("buildingPropList");
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

        private bool UploaBuildingAnchorNode(AnchorNode anchor, string strID, string strKey)
        {
            string strChkNo = "";

            // 삭제 이전에 조회 후에 있을 경우
            strChkNo = CheckAnchorNode(strID, strKey);
            if (strChkNo == null)
                return false;

            if (strChkNo != "")
            {
                // 앵커노드 속성 조회
                List<string> anchorNodePropList = GetAnchorNodePropIDs(m_updownBuildingKey, strID, strKey);

                if (anchorNodePropList == null)
                    return false;

                // 조회된 속성 삭제
                if (!RemoveProps(m_updownBuildingKey, anchorNodePropList, strID, strKey, "anchorNode"))
                    return false;

                // 기존의 앵커노드 삭제
                if (!RemoveAnchorNode(strID, strKey))
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

            XElement xBuildNo = new XElement("build_mng_no", m_updownBuildingKey);
            XElement xGlobalX = new XElement("global_x", GetDoubleString(anchor.Global.Position.x));
            XElement xGlobalY = new XElement("global_y", GetDoubleString(anchor.Global.Position.y));
            XElement xLocalX = new XElement("local_x", GetDoubleString(anchor.Local.Position.x));
            XElement xLocalY = new XElement("local_y", GetDoubleString(anchor.Local.Position.y));
            XElement xAngle = new XElement("angle", GetDoubleString(anchor.Local.Angle));
            XElement xUnitOfLength = new XElement("unitoflength", ((int)anchor.Global.Unit));

            xAnchorNode.Add(xBuildNo);
            xAnchorNode.Add(xGlobalX);
            xAnchorNode.Add(xGlobalY);
            xAnchorNode.Add(xLocalX);
            xAnchorNode.Add(xLocalY);
            xAnchorNode.Add(xAngle);
            xAnchorNode.Add(xUnitOfLength);

            xRoot.Add(xAnchorNode);

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/buildingAnchorNode", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploaBuildingAnchorNode Error : " + strErrorMessage);
                return false;
            }

            // 앵커노드 속성 업로드
            if (!UploadAnchorNodeProperty(anchor.Properties, m_updownBuildingKey, strID, strKey))
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

        private string CheckAnchorNode(string strID, string strKey)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xAnchorNode = new XElement("buildingAnchorNode");

            XElement xID = MakeElement("build_mng_no", m_updownBuildingKey);
            xAnchorNode.Add(xID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);
            xRoot.Add(xAnchorNode);

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/buildingAnchorNodeList", true, out strErrorMessage);


            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetAnchorNode Error : " + strErrorMessage);
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

        private bool RemoveAnchorNode(string strID, string strKey)
        {
            string strURL = string.Format("spatial/buildingAnchorNode/{0}/{1}/{2}", strID, strKey, m_updownBuildingKey);
            string strErrorMessage;

            string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strErrorMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveAnchorNode Error : " + strErrorMessage);
                    return false;
                }
            }

            return true;
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

        private bool UploadColumns_NEW(string strLevelID, List<Column> columns, string strID, string strKey)
        {
            // 노아서버 Column 조회
            List<string> columnList = GetColumnList(strLevelID, strID, strKey);


            // 조회된 Column 노아서버에 삭제 요청
            if (RemoveColumns(columnList, strLevelID, strID, strKey) == false)
                return false;

            if (columns.Count == 0)
                return true;


            if (UploadColumns(strLevelID, columns, strID, strKey) == false)
                return false;


            return true;
        }

        private List<string> GetColumnList(string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/columnList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetColumnList Error : " + strErrorMessage);
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

        private bool RemoveColumns(List<string> columnIDs, string strLevelID, string strID, string strKey)
        {
            foreach (string strColumnID in columnIDs)
            {
                // 컬럼 속성 조회
                List<string> columnPropList = GetPropIDs(strColumnID, strID, strKey, "column");

                if (columnPropList == null)
                    return false;

                // 컬럼 속성 삭제
                if (!RemoveProps(strColumnID, columnPropList, strID, strKey, "column"))
                    return false;

                if (!RemoveColumn(strColumnID, strLevelID, strID, strKey))
                    return false;
            }

            return true;
        }

        private bool RemoveColumn(string strColumnID, string strLevelID, string strID, string strKey)
        {
            string strURL = string.Format("spatial/column/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strColumnID);
            string strErrorMessage;

            string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strErrorMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveColumn Error : " + strErrorMessage);
                    return false;
                }
            }

            return true;
        }

        private bool UpdateColumns(Dictionary<Column, string> dicColumns, string strLevelID, string strID, string strKey)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            foreach (KeyValuePair<Column, string> pair in dicColumns)
            {
                Column col = pair.Key;

                XElement xColumn = new XElement("column");

                if (col.Type == Column.ColumnType.Rect)
                {
                    XElement xColumnID = new XElement("column_id", col.XMLID);
                    XElement xType = new XElement("column_type", 0);
                    XElement xTlx = new XElement("tl_x", col.RectData.TopLeft.x);
                    XElement xTly = new XElement("tl_y", col.RectData.TopLeft.y);
                    XElement xBlx = new XElement("bl_x", col.RectData.BottomLeft.x);
                    XElement xBly = new XElement("bl_y", col.RectData.BottomLeft.y);
                    XElement xBrx = new XElement("br_x", col.RectData.BottomRight.x);
                    XElement xBry = new XElement("br_y", col.RectData.BottomRight.y);
                    XElement xLevelID = new XElement("level_id", strLevelID);

                    xColumn.Add(xColumnID);
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
                    XElement xColumnID = new XElement("column_id", col.XMLID);
                    XElement xType = new XElement("column_type", 1);
                    XElement xTlx = new XElement("tl_x", col.CircleData.Center.x);
                    XElement xTly = new XElement("tl_y", col.CircleData.Center.y);
                    XElement xBlx = new XElement("bl_x", col.CircleData.Radius);
                    XElement xLevelID = new XElement("level_id", strLevelID);

                    xColumn.Add(xColumnID);
                    xColumn.Add(xType);
                    xColumn.Add(xTlx);
                    xColumn.Add(xTly);
                    xColumn.Add(xBlx);
                    xColumn.Add(xLevelID);
                }

                xRoot.Add(xColumn);

                string strErrorMessage;
                string strXML = xRoot.ToString();
                string strResult = SendQuery(strXML, "spatial/column", true, out strErrorMessage, "PUT");

                if (strResult.Length == 0)
                {
                    System.Diagnostics.Trace.WriteLine("UpdateColumns Error : " + strErrorMessage);
                    return false;
                }

                XElement xml = XElement.Parse(strResult);

                if (xml == null)
                {
                    System.Diagnostics.Trace.WriteLine("UpdateColumns Error2 : " + strResult);
                    return false;
                }
            }

            if (!UploadColumnProperty_NEW(dicColumns, strID, strKey))
                return false;

            return true;
        }

        private bool UploadColumnProperty_NEW(Dictionary<Column, string> dicColumnIDs, string strID, string strKey)
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

                // 해당 컬럼 속성 조회
                List<string> columnPropList = GetPropIDs(strColumnID, strID, strKey, "column");

                if (columnPropList == null)
                    return false;

                // 조회된 속성 삭제
                if (!RemoveProps(strColumnID, columnPropList, strID, strKey, "column"))
                    return false;

                // 현재 컬럼 속성 등록
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
                System.Diagnostics.Trace.WriteLine("UploadColumnProperty_NEW Error : " + strErrorMessage);
                return false;
            }

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
            if (UploadPOIWires(wires, dicPOIIDs, level, strLevelID, strID, strKey) == false)
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
            //XElement xFloorID = MakeElement("floor", strLevelID);
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

            return true;
        }

        private bool UploadTopologyNodeLinks(Dictionary<Topology.Node, string> dicTopologyNodeIDs, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/topologyNodeLink", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadTopologyNodeLinks Error : " + strErrorMessage);
                return false;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadTopologyNodes Error2 : " + strResult);
                return false;
            }

            return true;
        }

        // Return 값
        //           Value : Topology ID
        private Dictionary<Topology.Node, string> UploadTopologyNodes(Dictionary<Topology, string> dicTopologyIDs, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/topologyNode", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadTopologyNodes Error : " + strErrorMessage);
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadTopologyNodes Error2 : " + strResult);
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
                return null;
            }

            return dicTopologyNodeIDs;
        }

        // Return 값
        //           Value : Topology ID
        private Dictionary<Topology, string> UploadTopologies(List<Topology> topologies, string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/topology", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadTopologies Error : " + strErrorMessage);
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadTopologies Error2 : " + strResult);
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
                return null;
            }

            return dicTopologyIDs;
        }

        private Dictionary<Topology, string> UploadTopologies_NEW(List<Topology> topologies, string strLevelID, string strID, string strKey)
        {
            Dictionary<Topology, string> dicTopologyIDs = new Dictionary<Topology, string>();

            if (topologies.Count == 0)
                return dicTopologyIDs;

            // 노아서버 Topology 조회
            List<string> topologyList = GetTopologyList(strLevelID, strID, strKey);

            if (topologyList == null)
                return null;


            // 조회된 Topology 노아서버에 삭제 요청
            if (RemoveTopologes(topologyList, strLevelID, strID, strKey) == false)
                return null;

            dicTopologyIDs = UploadTopologies(topologies, strLevelID, strID, strKey);

            if (dicTopologyIDs == null)
                return null;

            return dicTopologyIDs;
        }

        private List<string> GetTopologyList(string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/topologyList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetTopologyList Error : " + strErrorMessage);
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

        private bool RemoveTopologes(List<string> topologyIDs, string strLevelID, string strID, string strKey)
        {
            foreach (string strTopologyID in topologyIDs)
            {
                string strURL = string.Format("spatial/topology/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strTopologyID);
                string strErrorMessage;

                string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

                if (strResult.Length == 0)
                {
                    if (strErrorMessage != SUCCESS_CODE)
                    {
                        System.Diagnostics.Trace.WriteLine("RemoveTopologes Error : " + strErrorMessage);
                        return false;
                    }
                }
            }

            return true;
        }

        private List<string> GetTopologyNodeList(string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/topologyNodeList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetTopologyNodeList Error : " + strErrorMessage);
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

        private bool RemoveTopologeNodes(List<string> topologyNodeIDs, string strID, string strKey)
        {
            foreach (string strTopologyID in topologyNodeIDs)
            {
                string strURL = string.Format("spatial/topologyNode/{0}/{1}/{2}", strID, strKey, strTopologyID);
                string strErrorMessage;

                string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

                if (strResult.Length == 0)
                {
                    if (strErrorMessage != SUCCESS_CODE)
                    {
                        System.Diagnostics.Trace.WriteLine("RemoveTopologeNodes Error : " + strErrorMessage);
                        return false;
                    }
                }
            }

            return true;
        }

        private List<string> GetTopologyNodeLinkList(string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/topologyNodeLinkList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetTopologyNodeLinkList Error : " + strErrorMessage);
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

        private bool RemoveTopologeNodeLinks(List<string> topologyNodeLinkIDs, string strLevelID, string strID, string strKey)
        {
            foreach (string strTopologyNodeLinkID in topologyNodeLinkIDs)
            {
                string strURL = string.Format("spatial/topologyNodeLink/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strTopologyNodeLinkID);
                string strErrorMessage;

                string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

                if (strResult.Length == 0)
                {
                    if (strErrorMessage != SUCCESS_CODE)
                    {
                        System.Diagnostics.Trace.WriteLine("RemoveTopologeNodeLinks Error : " + strErrorMessage);
                        return false;
                    }
                }
            }

            return true;
        }


        private bool UploadWindowProperty(Dictionary<Window, string> dicWindowIDs, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/windowProp", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadDoorProperty Error : " + strErrorMessage);
                return false;
            }

            return true;
        }
        private bool UploadWindows(Dictionary<Wall, string> dicWallIDs, string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/window", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadWindows Error : " + strErrorMessage);
                return false;
            }
            //------

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadDoor Error2 : " + strResult);
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

            if (!UploadWindowProperty(dicWindowIDs, strID, strKey))
                return false;
            return true;
        }

        private bool UploadWindows_NEW(Dictionary<Wall, string> dicWalls, string strLevelID, string strID, string strKey)
        {
            // 노아서버 창문 조회
            List<string> windowList = GetWindowList(strLevelID, strID, strKey);

            if (windowList == null)
                return false;

            // 삭제된 창문 노아서버에 삭제 요청
            if (RemoveWindows(windowList, strLevelID, strID, strKey) == false)
                return false;

            if (UploadWindows(dicWalls, strLevelID, m_updownLoginID, m_updownLoginKey) == false)
                return false;

            return true;
        }

        private List<string> GetWindowList(string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/windowList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetWindowList Error : " + strErrorMessage);
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

        private bool RemoveWindows(List<string> windowIDs, string strLevelID, string strID, string strKey)
        {
            foreach (string strWindowID in windowIDs)
            {
                // 창문 속성 조회
                List<string> windowPropList = GetPropIDs(strWindowID, strID, strKey, "window");

                if (windowPropList == null)
                    return false;

                // 창문 속성 삭제
                if (!RemoveProps(strWindowID, windowPropList, strID, strKey, "window"))
                    return false;

                if (!RemoveWindow(strWindowID, strLevelID, strID, strKey))
                    return false;
            }

            return true;
        }

        private bool RemoveWindow(string strWindowID, string strLevelID, string strID, string strKey)
        {
            string strURL = string.Format("spatial/window/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strWindowID);
            string strErrorMessage;

            string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strErrorMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveWindow Error : " + strErrorMessage);
                    return false;
                }
            }

            return true;
        }

        private bool AddWindows(Dictionary<Window, string> dicWindows, string strLevelID, string strID, string strKey)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            int nWindowCount = 0;

            foreach (KeyValuePair<Window, string> pair in dicWindows)
            {
                Window window = pair.Key;

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


            if (nWindowCount == 0)
                return true;

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/window", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("AddWindows Error : " + strErrorMessage);
                return false;
            }
            //------

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("AddWindows Error2 : " + strResult);
                return false;
            }

            Dictionary<Window, string> dicWindowIDs = new Dictionary<Window, string>();
            List<Window> dlist = new List<Window>();// tmp Window List
            foreach (KeyValuePair<Window, string> tmpPair in dicWindows)
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

            if (!UploadWindowProperty(dicWindowIDs, strID, strKey))
                return false;

            return true;
        }


        private bool UploadWindowProperty_NEW(Dictionary<Window, string> dicWindowIDs, string strID, string strKey)
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

                // 해당 창문 속성 조회
                List<string> windowPropList = GetPropIDs(strWindowID, strID, strKey, "window");

                if (windowPropList == null)
                    return false;

                // 조회된 창문 속성 삭제
                if (!RemoveProps(strWindowID, windowPropList, strID, strKey, "window"))
                    return false;

                // 현재 공간에 대한 속성 등록
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/windowProp", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadWindowProperty_NEW Error : " + strErrorMessage);
                return false;
            }

            return true;
        }

        private bool UploadDoorProperty(Dictionary<Door, string> dicDoorIDs, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/doorProp", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadDoorProperty Error : " + strErrorMessage);
                return false;
            }

            return true;
        }
        private bool UploadDoors(Dictionary<Wall, string> dicWallIDs, string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/door", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadDoors Error : " + strErrorMessage);
                return false;
            }
            
            //------
           
            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadDoor Error2 : " + strResult);
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

            if (!UploadDoorProperty(dicDoorIDs, strID, strKey))
                return false;

            return true;
        }

        private bool UploadDoors_NEW(Dictionary<Wall, string> dicWalls, string strLevelID, string strID, string strKey)
        {
            // 노아서버 문 조회
            List<string> doorList = GetDoorList(strLevelID, strID, strKey);

            if (doorList == null)
                return false;

            // 조회된 문 노아서버에 삭제 요청
            if (RemoveDoors(doorList, strLevelID, strID, strKey) == false)
                return false;

            if (UploadDoors(dicWalls, strLevelID, strID, strKey) == false)
                return false;

            return true;
        }

        private List<string> GetDoorList(string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/doorList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetDoorList Error : " + strErrorMessage);
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

        private bool RemoveDoors(List<string> doorIDs, string strLevelID, string strID, string strKey)
        {
            foreach (string strDoorID in doorIDs)
            {
                // 문 속성 조회
                List<string> doorPropList = GetPropIDs(strDoorID, strID, strKey, "door");

                if (doorPropList == null)
                    return false;

                // 문 속성 삭제
                if (!RemoveProps(strDoorID, doorPropList, strID, strKey, "door"))
                    return false;


                if (!RemoveDoor(strDoorID, strLevelID, strID, strKey))
                    return false;
            }

            return true;
        }

        private bool RemoveDoor(string strDoorID, string strLevelID, string strID, string strKey)
        {
            string strURL = string.Format("spatial/door/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strDoorID);
            string strErrorMessage;

            string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strErrorMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveDoor Error : " + strErrorMessage);
                    return false;
                }
            }

            return true;
        }

        private bool AddDoors(Dictionary<Door, string> dicDoors, string strLevelID, string strID, string strKey)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            int nDoorCount = 0;

            foreach (KeyValuePair<Door, string> pair in dicDoors)
            {
                Door door = pair.Key;

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

            if (nDoorCount == 0)
                return true;

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/door", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("AddDoors Error : " + strErrorMessage);
                return false;
            }

            //------

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("AddDoors Error2 : " + strResult);
                return false;
            }

            Dictionary<Door, string> dicDoorIDs = new Dictionary<Door, string>();
            List<Door> dlist = new List<Door>();// tmp Door List
            foreach (KeyValuePair<Door, string> tmpPair in dicDoors)
            {
                Door dr = tmpPair.Key;
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

            if (!UploadDoorProperty(dicDoorIDs, strID, strKey))
                return false;

            return true;
        }

        private bool UploadDoorProperty_NEW(Dictionary<Door, string> dicDoorIDs, string strID, string strKey)
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

                // 해당 문에 대한 속성 조회
                List<string> doorPropList = GetPropIDs(strDoorID, strID, strKey, "door");

                if (doorPropList == null)
                    return false;

                // 조회된 속성 삭제
                if (!RemoveProps(strDoorID, doorPropList, strID, strKey, "door"))
                    return false;

                // 현재 문에 대한 속성 추가
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/doorProp", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadDoorProperty_NEW Error : " + strErrorMessage);
                return false;
            }

            return true;
        }

        private bool UploadSpaceWallLink(Dictionary<Space, string> dicSpaceIDs, Dictionary<Wall, string> dicWallIDs, string strLevelID, string strID, string strKey)
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
                    List<string> spaceWallLinkList = GetSpaceWallLink(pair.Value, strWallID, strLevelID, strID, strKey);

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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/spaceWall", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadSpaceWallLink Error : " + strErrorMessage);
                return false;
            }

            return true;
        }

        private List<string> GetSpaceWallLink(string strSpaceID, string strWallID, string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/spaceWallList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetSpaceList Error : " + strErrorMessage);
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

        private List<string> GetAlertAreaList(string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/alertAreaList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetAlertAreaList Error : " + strErrorMessage);
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

        private bool RemoveAlertAreas(List<string> alertAreaIDs, string strLevelID, string strID, string strKey)
        {
            foreach (string strAlertID in alertAreaIDs)
            {
                // 경계구역 속성 조회
                List<string> alertAreaPropList = GetPropIDs(strAlertID, strID, strKey, "alertArea");

                if (alertAreaPropList == null)
                    return false;

                // 경계구역 속성 삭제
                if (!RemoveProps(strAlertID, alertAreaPropList, strID, strKey, "alertArea"))
                    return false;

                // 경계구역 바운더리 삭제
                // 경계구역 바운더리 조회
                List<string> checkIDs = GetAlertAreaBoundaryList(strAlertID, strID, strKey);

                // 조회가 될 경우 삭제
                if (checkIDs.Count > 0)
                {
                    if (!RemoveAlertAreaBoundary(strAlertID, strLevelID, strID, strKey))
                        return false;
                }

                if (!RemoveAlertArea(strAlertID, strLevelID, strID, strKey))
                    return false;
            }

            return true;
        }

        private bool RemoveAlertArea(string strAlertAreaID, string strLevelID, string strID, string strKey)
        {
            string strURL = string.Format("spatial/alertArea/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strAlertAreaID);
            string strErrorMessage;

            string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strErrorMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveAlertArea Error : " + strErrorMessage);
                    return false;
                }
            }

            return true;
        }

        private List<string> GetAlertAreaBoundaryList(string strSpaceID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/alertAreaBoundaryList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetAlertAreaBoundaryList Error : " + strErrorMessage);
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

        private bool RemoveAlertAreaBoundary(string strAlertAreaID, string strLevelID, string strID, string strKey)
        {
            string strURL = string.Format("spatial/alertAreaBoundary/{0}/{1}/{2}", strID, strKey, strAlertAreaID);
            string strErrorMessage;

            string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strErrorMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveAlertAreaBoundary Error : " + strErrorMessage);
                    return false;
                }
            }

            return true;
        }

        private Dictionary<AlertArea, string> UploadAlertAreas_NEW(List<AlertArea> alertAreas, string strLevelID, string strID, string strKey)
        {
            // 노아서버 경계구역 조회
            List<string> alertAreaList = GetAlertAreaList(strLevelID, strID, strKey);

            if (alertAreaList == null)
                return null;

            // 조회된 경계구역 노아서버에 삭제 요청
            if (RemoveAlertAreas(alertAreaList, strLevelID, strID, strKey) == false)
                return null;

            Dictionary<AlertArea, string> dicAlertAreaIDs = UploadAlertAreas(alertAreas, strLevelID, strID, strKey);

            if (dicAlertAreaIDs == null)
                return null;

            return dicAlertAreaIDs;
        }

        // [Return 값]
        //             Value : AlertArea ID
        private Dictionary<AlertArea, string> UploadAlertAreas(List<AlertArea> alertAreas, string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/alertArea", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadAlertAreas Error : " + strErrorMessage);
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadAlertAreas Error2 : " + strResult);
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
                return null;
            }
            if (!UploadAlertAreaProperty(dicAlertAreaIDs, strID, strKey))
                return null;

            return dicAlertAreaIDs;
        }

        private bool UploadAlertAreaProperty(Dictionary<AlertArea, string> dicAlertAreaIDs, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/alertAreaProp", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadAlertAreaProperty Error : " + strErrorMessage);
                return false;
            }

            return true;
        }

        private bool UploadSpaceProperty(Dictionary<Space, string> dicSpaceIDs, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/spaceProp", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadSpaceProperty Error : " + strErrorMessage);
                return false;
            }

            return true;
        }
        // [Return 값]
        //             Value : Space ID
        private Dictionary<Space, string> UploadSpaces(List<Space> spaces, string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/space", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadSpaces Error : " + strErrorMessage);
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadSpaces Error2 : " + strResult);
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
                return null;
            }
            if (!UploadSpaceProperty(dicSpaceIDs, strID, strKey))
                return null;

            return dicSpaceIDs;
        }

        // [Return 값]
        //             Value : Space ID
        private Dictionary<Space, string> UploadSpaces_NEW(List<Space> spaces, string strLevelID, string strID, string strKey)
        {
            // 노아서버 공간 조회
            List<string> spaceList = GetSpaceList(strLevelID, strID, strKey);

            if (spaceList == null)
                return null;

            // 조회된 공간 노아서버에 삭제 요청
            if (RemoveSpaces(spaceList, strLevelID, strID, strKey) == false)
                return null;

            Dictionary<Space, string> dicSpaceIDs = UploadSpaces(spaces, strLevelID, strID, strKey);

            if (dicSpaceIDs == null)
                return null;

            return dicSpaceIDs;
        }

        private List<string> GetSpaceList(string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/spaceList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetSpaceList Error : " + strErrorMessage);
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

        private bool RemoveSpaces(List<string> spaceIDs, string strLevelID, string strID, string strKey)
        {
            foreach (string strSpaceID in spaceIDs)
            {
                // 공간 속성 조회
                List<string> spacePropList = GetPropIDs(strSpaceID, strID, strKey, "space");

                if (spacePropList == null)
                    return false;

                // 공간 속성 삭제
                if (!RemoveProps(strSpaceID, spacePropList, strID, strKey, "space"))
                    return false;

                // 공간 바운더리 삭제
                // 공간 바운더리 조회
                List<string> checkIDs = GetSpaceBoundaryList(strSpaceID, strID, strKey);

                // 조회가 될 경우 삭제
                if (checkIDs.Count > 0)
                {
                    if (!RemoveSpaceBoundary(strSpaceID, strLevelID, strID, strKey))
                        return false;
                }
                    
                // TODO: 홀 바운더리 삭제


                if (!RemoveSpace(strSpaceID, strLevelID, strID, strKey))
                    return false;
            }

            return true;
        }

        private bool RemoveSpace(string strSpaceID, string strLevelID, string strID, string strKey)
        {
            string strURL = string.Format("spatial/space/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strSpaceID);
            string strErrorMessage;

            string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strErrorMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveSpace Error : " + strErrorMessage);
                    return false;
                }
            }
        
            return true;
        }

        private bool UploadSpaceProperty_NEW(List<Space> spaceIDList, string strID, string strKey)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xRoot = new XElement("spatial");
            xRoot.Add(xState);

            bool flag = false;
            foreach (Space space in spaceIDList)
            {
                // 해당 공간 속성 조회
                List<string> spacePropList = GetPropIDs(space.XMLID, strID, strKey, "space");

                if (spacePropList == null)
                    return false;

                // 조회된 속성 삭제
                if (!RemoveProps(space.XMLID, spacePropList, strID, strKey, "space"))
                    return false;

                // 현재 공간에 대한 속성 등록
                foreach (Property prop in space.Properties)
                {
                    XElement xSpace = new XElement("spacePropList");

                    XElement xType = new XElement("space_id", space.XMLID);
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/spaceProp", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadSpaceProperty_NEW Error : " + strErrorMessage);
                return false;
            }

            return true;
        }

        private bool UploadWallProperty(Dictionary<Wall, string> dicWallIDs, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/wallProp", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadWallProperty Error : " + strErrorMessage);
                return false;
            }

            return true;
        }
        private Dictionary<Wall, string> UploadWalls(Dictionary<Wall, string> dicGridIDs, string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/wall", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadWalls Error : " + strErrorMessage);
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadWalls Error2 : " + strResult);
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
                return null;
            }

            if (!UploadWallProperty(dicWallIDs, strID, strKey))                         
                return null;            

            return dicWallIDs;
        }

        private Dictionary<Wall, string> UploadWalls_NEW(Dictionary<Wall, string> dicGridIDs, string strLevelID, string strID, string strKey)
        {
            // 노아서버 벽 조회
            List<string> wallList = GetWallList(strLevelID, strID, strKey);

            if (wallList == null)
                return null;

            // 조회된 벽 노아서버에 삭제 요청
            if (RemoveWalls(wallList, strLevelID, strID, strKey) == false)
                return null;

            Dictionary<Wall, string> dicWallIDs = UploadWalls(dicGridIDs, strLevelID, strID, strKey);

            if (dicWallIDs == null)
                return null;

            return dicWallIDs;
        }

        // Return 값 : Wall ID List
        private List<string> GetWallList(string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/wallList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetWallList Error : " + strErrorMessage);
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

        private bool RemoveWalls(List<string> wallIDs, string strLevelID, string strID, string strKey)
        {
            foreach (string strWallID in wallIDs)
            {
                // 벽 속성 조회
                List<string> wallPropList = GetPropIDs(strWallID, strID, strKey, "wall");

                if (wallPropList == null)
                    return false;

                // 벽 속성 삭제
                if (!RemoveProps(strWallID, wallPropList, strID, strKey, "wall"))
                    return false;

                // 벽체 바운더리 조회  GetWallBoundaryList
                List<string> checkIDs = GetWallBoundaryList(strWallID, strID, strKey);

                // 조회가 될 경우 삭제
                if (checkIDs.Count > 0)
                {
                    if (!RemoveWallBoundary(strWallID, strLevelID, strID, strKey))
                        return false;
                }

                if (!RemoveWall(strWallID, strLevelID, strID, strKey))
                    return false;
            }

            return true;
        }

        private bool RemoveWall(string strWallID, string strLevelID, string strID, string strKey)
        {
            string strURL = string.Format("spatial/wall/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strWallID);
            string strErrorMessage;

            string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strErrorMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveWall Error : " + strErrorMessage);
                    return false;
                }
            }

            return true;
        }

        private bool UploadWallProperty_NEW(Dictionary<Wall, string> dicWallIDs, string strID, string strKey)
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

                // 해당 벽에 대한 속성 조회
                List<string> wallPropList = GetPropIDs(strWallID, strID, strKey, "wall");

                if (wallPropList == null)
                    return false;

                // 조회된 속성 삭제
                if (!RemoveProps(wall.XMLID, wallPropList, strID, strKey, "wall"))
                    return false;

                // 현재 벽에 대한 속성 등록
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/wallProp", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadWallProperty_NEW Error : " + strErrorMessage);
                return false;
            }


            return true;
        }

        private bool UploadWallBoundarys(Dictionary<Wall, string> dicWallIDs, string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/wallBoundary", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadWallBoundarys Error : " + strErrorMessage);
                return false;
            }

            return true;
        }

        private List<string> GetWallBoundaryList(string strWallID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/wallBoundaryList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetWallBoundaryList Error : " + strErrorMessage);
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

        private bool RemoveWallBoundary(string strWallID, string strLevelID, string strID, string strKey)
        {
            //string strURL = string.Format("spatial/wallBoundary/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strWallID);
            string strURL = string.Format("spatial/wallBoundary/{0}/{1}/{2}", strID, strKey, strWallID);
            string strErrorMessage;

            string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strErrorMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveWallBoundary Error : " + strErrorMessage);
                    return false;
                }
            }

            return true;
        }

        private bool UploadAlertAreaBoundarys(Dictionary<AlertArea, string> dicAlertAreaIDs, string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/alertAreaBoundary", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadAlertAreaBoundarys Error : " + strErrorMessage);
                return false;
            }

            return true;
        }

        private bool UploadSpaceBoundarys(Dictionary<Space, string> dicSpaceIDs, string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/spaceBoundary", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadSpaceBoundarys Error : " + strErrorMessage);
                return false;
            }

            return true;
        }

        private List<string> GetSpaceBoundaryList(string strSpaceID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/spaceBoundaryList", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetSpaceBoundaryList Error : " + strErrorMessage);
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

        private bool RemoveSpaceBoundary(string strSpaceID, string strLevelID, string strID, string strKey)
        {
            //string strURL = string.Format("spatial/spaceBoundary/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strSpaceID);
            string strURL = string.Format("spatial/spaceBoundary/{0}/{1}/{2}", strID, strKey, strSpaceID);
            string strErrorMessage;

            string strResult = SendQuery(null, strURL, true, out strErrorMessage, "DELETE");

            if (strResult.Length == 0)
            {
                if (strErrorMessage != SUCCESS_CODE)
                {
                    System.Diagnostics.Trace.WriteLine("RemoveSpaceBoundary Error : " + strErrorMessage);
                    return false;
                }
            }

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

        // [Return 값]
        //             Value : Grid ID
        private Dictionary<Wall, string> UploadGrids(List<Wall> walls, string strLevelID, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/grid", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadGrids Error : " + strErrorMessage);
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadGrids Error2 : " + strResult);
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
                return null;
            }

            return dicGridIDs;
        }

        private Dictionary<Wall, string> UploadGrids_NEW(List<Wall> walls, string strLevelID, string strID, string strKey)
        {
            // 노아서버 벽체선형 조회
            List<string> gridList = GetGridList(strLevelID, strID, strKey);

            // 조회된 벽체 노아서버에 삭제 요청
            if (RemoveGrids(gridList, strLevelID, strID, strKey) == false)
                return null;

            Dictionary<Wall, string> dicGridIDs = UploadGrids(walls, strLevelID, strID, strKey);

            if (dicGridIDs == null)
                return null;

            return dicGridIDs;
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

        private bool RemoveGrids(List<string> gridIDs, string strLevelID, string strID, string strKey)
        {

            foreach (string strGridID in gridIDs)
            {
                string strURL = string.Format("spatial/grid/{0}/{1}/{2}/{3}", strID, strKey, strLevelID, strGridID);
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

        // [Return 값]
        //             Value : Level ID
        private string UploadLevel(string strBuildingID, Level level, string strID, string strKey)
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
       
            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/level", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadLevel Error : " + strErrorMessage);
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadLevel Error2 : " + strResult);
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
            if (!UploadLevelProperty(dicLevelIDs, strID, strKey))
                return null;

            return strRetID;
        }

        // [Return 값]
        //             Value : Level ID
        private Dictionary<Level, string> UploadLevels(string strBuildingID, List<Level> levels, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/level", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadLevels Error : " + strErrorMessage);
                return null;
            }

            XElement xml = XElement.Parse(strResult);

            if (xml == null)
            {
                System.Diagnostics.Trace.WriteLine("UploadLevels Error2 : " + strResult);
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
            if (!UploadLevelProperty(dicLevelIDs, strID, strKey))
                return null;

            return dicLevelIDs;
        }

        private bool UploadLevelProperty(Dictionary<Level, string> dicLevelIDs, string strID, string strKey)
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

            string strErrorMessage;
            string strXML = xRoot.ToString();
            string strResult = SendQuery(strXML, "spatial/levelProp", true, out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("UploadLevelProperty Error : " + strErrorMessage);
                return false;
            }

            return true;
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
