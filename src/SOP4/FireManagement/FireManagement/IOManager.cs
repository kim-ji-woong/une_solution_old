using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Net;
using DBUtility;

namespace FireManagement
{
    public class IOManager
    {
        private string FTP_ID = new string(new char[] { 's', 'o', 'p', '_', 'u', 's', 'e', 'r' });
        private string FTP_PW = new string(new char[] { '1', '0', '4', '7', 'A', 'b', '@', '#' });

        // \ / : * ? " < > | 와 같이 파일명에 사용할 수 없는 특수 문자들은 다른 문자열로 치환한다.
        private static Dictionary<string, string> SPECIAL_ORIGIN2FILE = new Dictionary<string, string>();
        private static Dictionary<string, string> SPECIAL_FILE2ORIGIN = new Dictionary<string, string>();

        // Zone에 속해있는 EquipmentZone List
        private Dictionary<Zone, ArrayList> m_dicZoneEquipZones = new Dictionary<Zone, ArrayList>();
        // 전체 Zone
        // Zone ID, Zone
        private Dictionary<int, Zone> m_dicZones = new Dictionary<int, Zone>();
        // Building에 속해있는 Zone List
        // Building ID, Zone List
        private Dictionary<int, ArrayList> m_dicBuildingZones = new Dictionary<int, ArrayList>();
        // Building ID, Building
        private Dictionary<int, Building> m_dicBuildings = new Dictionary<int, Building>();
        // Building Group, Building Group에 연계된 Building들
        private Dictionary<BuildingGroup, ArrayList> m_dicBuildingGroups = new Dictionary<BuildingGroup, ArrayList>();
        // BuildingID가 -1인 외부 공간들...
        // Zone ID, Zone
        private Dictionary<int, Zone> m_dicOutdoorZones = new Dictionary<int, Zone>();
        // FireEquipment ID, Equipment History List
        private Dictionary<int, ArrayList> m_dicEquipmentHistory = new Dictionary<int, ArrayList>();
        private Dictionary<int, ArrayList> m_dicDBEquipmentHistory = new Dictionary<int, ArrayList>();
        // Zone별 소방설비들
        private Dictionary<Zone, ArrayList> m_dicZoneEquipments = new Dictionary<Zone, ArrayList>();
        // System에 저장된 Zone별 소방설비들(Tablet에서는 FMF, PC에서는 DB)
        // 프로그램 종료시까지 데이터 바뀌지 않음(DB나 FMF에 저장한 후에도 변하지 않음)
        private Dictionary<Zone, ArrayList> m_dicDBZoneEquipments = new Dictionary<Zone, ArrayList>();
        // Key : RFID Tag
        private Dictionary<string, FireEquipment> m_dicRFIDFireEquipments = new Dictionary<string, FireEquipment>();
        // Key : 설비Type + 관리번호(EquipID)
        private Dictionary<string, FireEquipment> m_dicEquipIDFireEquipments = new Dictionary<string, FireEquipment>();
        // 실행중 설비정보가 변경된 Zone들
        private ArrayList m_arrChangedZones = new ArrayList();

        private string m_strTabletDataFile = "FEData\\data.fmf";

        public IOManager()
        {
            if (SPECIAL_ORIGIN2FILE.Count == 0)
                SetSpecialStrings();
        }

        private static void SetSpecialStrings()
        {
            SetSpecialStrings("\\", "[@_1_@]");
            SetSpecialStrings("/", "[@_2_@]");
            SetSpecialStrings(":", "[@_3_@]");
            SetSpecialStrings("*", "[@_4_@]");
            SetSpecialStrings("?", "[@_5_@]");
            SetSpecialStrings("\"", "[@_6_@]");
            SetSpecialStrings("<", "[@_7_@]");
            SetSpecialStrings(">", "[@_8_@]");

            SetSpecialStrings("|", "[@_9_@]");
        }

        private static void SetSpecialStrings(string strOrigin, string strFile)
        {
            SPECIAL_ORIGIN2FILE[strOrigin] = strFile;
            SPECIAL_FILE2ORIGIN[strFile] = strOrigin;
        }

        // strSrc를 File명에 사용할 수 있는 특수 문자들로 바꾼다.
        public static string ToFileString(string strSrc)
        {
            string strTrg = strSrc;

            foreach (KeyValuePair<string, string> pair in SPECIAL_ORIGIN2FILE)
            {
                strTrg = strTrg.Replace(pair.Key, pair.Value);
            }

            return strTrg;
        }

        public static string ToOriginString(string strSrc)
        {
            string strTrg = strSrc;

            foreach (KeyValuePair<string, string> pair in SPECIAL_FILE2ORIGIN)
            {
                strTrg = strTrg.Replace(pair.Key, pair.Value);
            }

            return strTrg;
        }

        public void LoadDB()
        {
            LoadBuildings();
            LoadZones();
            LoadEquipmentZones();
            LoadFireEquipments();
            LoadFireEquipmentHistory();
        }

        System.Drawing.PointF DxfToImage(double x, double y)
        {
            float fx = 0.0f, fy = 0.0f;
            
            return new System.Drawing.PointF(fx, fy);
        }
        

        private void LoadFireEquipments()
        {
            WebDBManager dbMgr = FormMain2.Instance.DBManager;

            //string strSQL = "select ID, RFIDTag, EquipID, RFIDTagID, DxfObjID, EquipType, ZoneID, X, Y, Description from FireEquipment";
            StringBuilder szb = new StringBuilder();
            szb.Append("select fe.ID, fe.RFIDTag, fe.EquipID, fe.RFIDTagID, fe.DxfObjID, fe.EquipType, fe.ZoneID, fe.X, fe.Y, fe.Description");
            szb.Append(" FROM FireEquipment as fe");
            szb.Append(" INNER JOIN Zone as z on  z.ID = fe.ZoneID and z.ID != -1");
            szb.Append(" WHERE z.SiteID = {0}");

            string szSQL = string.Format(szb.ToString(), FormMain2.Instance.SiteID);
            ArrayList arrResult = dbMgr.GetResultData(szSQL, 0);

            if (arrResult == null)
                return;

            int nLimitEquipType = (int)FireEquipment.EquipmentType.UNKNOWN;

            // FMF는 미터 단위이며 DXF는 mm 단위를 사용한다.
            float fFlag = 1 / FormMain2.Instance.GetUnitFlag(UnitOfLength.METER);

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 9; i += 10)
            {
                int nEquipType = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
                if (nEquipType <= 0 || nEquipType >= nLimitEquipType)
                    continue;

                FireEquipment equip = new FireEquipment();

                equip.ID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                equip.RFIDTag = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
                equip.EquipID = DBUtility.WebDBManager.GetStringField(arrResult[i + 2], "");
                equip.RFIDTagID = DBUtility.WebDBManager.GetStringField(arrResult[i + 3], "");
                equip.DXFObjID = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], "");
                //equip.Type = (FireEquipment.EquipmentType)DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
                equip.Type = (FireEquipment.EquipmentType)nEquipType;
                int nZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                float x = DBUtility.WebDBManager.GetFloatField(arrResult[i + 7].ToString(), 0.0f);
                float y = DBUtility.WebDBManager.GetFloatField(arrResult[i + 8].ToString(), 0.0f);
                equip.Description = DBUtility.WebDBManager.GetStringField(arrResult[i + 9], "");

                Zone zone = FindZone(nZoneID);
                if (zone == null)
                    continue;

                equip.Zone = zone;

                

                equip.Position = new System.Drawing.PointF(x * fFlag, y * fFlag);

                if (m_dicZoneEquipments.ContainsKey(zone))
                {
                    ArrayList arrEquipments = m_dicZoneEquipments[zone];
                    arrEquipments.Add(equip);
                }
                else
                {
                    ArrayList arrEquipments = new ArrayList();
                    arrEquipments.Add(equip);
                    m_dicZoneEquipments[zone] = arrEquipments;
                }

                if (m_dicDBZoneEquipments.ContainsKey(zone))
                {
                    ArrayList arrDBEquipments = m_dicDBZoneEquipments[zone];
                    arrDBEquipments.Add(new FireEquipment(equip));
                }
                else
                {
                    ArrayList arrDBEquipments = new ArrayList();
                    arrDBEquipments.Add(new FireEquipment(equip));
                    m_dicDBZoneEquipments[zone] = arrDBEquipments;
                }

                m_dicRFIDFireEquipments[equip.RFIDTag] = equip;
                m_dicEquipIDFireEquipments[GetEquipIDString(equip)] = equip;
            }
        }

        // 설비 Type + 관리번호의 값을 리턴한다.
        private string GetEquipIDString(FireEquipment equip)
        {
            return GetEquipIDString(equip.Type, equip.EquipID);
        }

        // 설비 Type + 관리번호의 값을 리턴한다.
        private string GetEquipIDString(FireEquipment.EquipmentType type, string strEquipID)
        {
            return ((int)type).ToString() + "_" + strEquipID;
        }

        private void LoadFireEquipmentHistory()
        {
            WebDBManager dbMgr = FormMain2.Instance.DBManager;

            //string strSQL = "select ID, FireEquipmentID, SOPGenUserID, Time, Status, CheckersOpinion, Description from FireEquipmentHistory";
 
            StringBuilder szb = new StringBuilder();
            szb.Append("select fh.ID, fh.FireEquipmentID, fh.SOPGenUserID, fh.Time, fh.Status, fh.CheckersOpinion, fh.Description ");
            szb.Append(" FROM FireEquipmentHistory as fh");
            szb.Append(" INNER JOIN FireEquipment as fe ON fe.EquipID = fh.FireEquipmentID");
            szb.Append(" INNER JOIN Zone as z on  z.ID = fe.ZoneID and z.ID != -1");
            szb.Append(" WHERE z.SiteID = {0}");

            string szSQL = string.Format(szb.ToString(), FormMain2.Instance.SiteID);
            ArrayList arrResult = dbMgr.GetResultData(szSQL, 0);

            if (arrResult == null)
                return;

            DateTime dtDefault = new DateTime();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nEquipID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nSOPGenUserID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                DateTime time = DBUtility.WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                int nStatus = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                string strOpinion = DBUtility.WebDBManager.GetStringField(arrResult[i + 5], "");
                string strDescription = DBUtility.WebDBManager.GetStringField(arrResult[i + 6], "");

                FireEquipmentHistory history = new FireEquipmentHistory();

                history.ID = nID;
                history.EquipmentID = nEquipID;
                history.SOPGenUserID = nSOPGenUserID;
                history.Time = time;
                history.Status = (FireEquipmentHistory.EquipmentStatus)nStatus;
                history.CheckersOpinion = strOpinion;
                history.Description = strDescription;
                history.IsNewHistory = false;

                if (m_dicEquipmentHistory.ContainsKey(nEquipID))
                {
                    ArrayList arrHistory = m_dicEquipmentHistory[nEquipID];
                    arrHistory.Add(history);
                }
                else
                {
                    ArrayList arrHistory = new ArrayList();
                    arrHistory.Add(history);
                    m_dicEquipmentHistory[nEquipID] = arrHistory;
                }

                if (m_dicDBEquipmentHistory.ContainsKey(nEquipID))
                {
                    ArrayList arrHistory = m_dicDBEquipmentHistory[nEquipID];
                    arrHistory.Add(new FireEquipmentHistory(history));
                }
                else
                {
                    ArrayList arrHistory = new ArrayList();
                    arrHistory.Add(new FireEquipmentHistory(history));
                    m_dicDBEquipmentHistory[nEquipID] = arrHistory;
                }
            }
        }


        public void LoadBuildings()
        {
            WebDBManager webDB = FormMain2.Instance.DBManager;

            //string strSQL = "select Building.id, BuildingID, BuildingCode, BuildingName, BuildingGroupID, MaxFloor, MinFloor,"
            //		 + "BuildingGroup.GroupName, BuildingGroup.TextCenter, Building.BroadCastingText ";
            //strSQL += "from Building, BuildingGroup where Building.BuildingGroupID = BuildingGroup.ID";

            string szText = "SELECT bd.id, bd.BuildingID, bd.BuildingCode, bd.BuildingName, bd.BuildingGroupID, " +
                            "  bd.MaxFloor, bd.MinFloor, bg.GroupName " +
                            "  FROM Building AS bd INNER JOIN BuildingGroup AS bg ON bd.BuildingGroupID = bg.ID AND bg.SiteID = {0} " +
                            "  ORDER BY bg.ID,  bd.ID";

            string strSQL = string.Format(szText, FormMain2.Instance.SiteID);

            ArrayList arrResult = webDB.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

            Dictionary<int, BuildingGroup> dicBuildingGroup = new Dictionary<int, BuildingGroup>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strBuildingID = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
                string strBuildingCode = DBUtility.WebDBManager.GetStringField(arrResult[i + 2], "");
                string strBuildingName = DBUtility.WebDBManager.GetStringField(arrResult[i + 3], "");
                int nBuildingGroupID = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nMaxFloorID = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nMinFloorID = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                string strBuildingGroupName = DBUtility.WebDBManager.GetStringField(arrResult[i + 7], "");

                Building building = new Building();

                if (dicBuildingGroup.ContainsKey(nBuildingGroupID))
                    building.BuildingGroup = dicBuildingGroup[nBuildingGroupID];
                else
                {
                    BuildingGroup group = new BuildingGroup();
                    group.ID = nBuildingGroupID;
                    group.BuildingGroupName = strBuildingGroupName;

                    dicBuildingGroup[nBuildingGroupID] = group;
                    building.BuildingGroup = group;
                }

                building.ID = nID;
                building.BuildingName = strBuildingName;
                building.MaxFloorIndex = nMaxFloorID;
                building.MinFloorIndex = nMinFloorID;
                building.BuildingCode = strBuildingCode;
                building.BuildingID = strBuildingID;

                m_dicBuildings[nID] = building;

                if (m_dicBuildingGroups.ContainsKey(building.BuildingGroup))
                {
                    ArrayList arrBuildings = m_dicBuildingGroups[building.BuildingGroup];
                    arrBuildings.Add(building);
                }
                else
                {
                    ArrayList arrBuildings = new ArrayList();
                    arrBuildings.Add(building);

                    m_dicBuildingGroups[building.BuildingGroup] = arrBuildings;
                }
            }

            // 외부 공간을 위한 BuildingGroup
            BuildingGroup outdoorGroup = new BuildingGroup();
            outdoorGroup.BuildingGroupName = "외부 공간";
            m_dicBuildingGroups[outdoorGroup] = new ArrayList();
        }

        private void LoadEquipmentZones()
        {
            WebDBManager dbMgr = FormMain2.Instance.DBManager;

            //string strSQL = "select id, ZoneName, Boundary, LinkedZoneIDList, type, BroadcastName, TextCenter from EquipmentZone";
            string szSQL = "select id, ZoneName, Boundary, LinkedZoneIDList, type, BroadcastName, TextCenter from EquipmentZone where SiteID = {0}";
            string strSQL = string.Format(szSQL, FormMain2.Instance.SiteID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strBoundary = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strLinkedZones = WebDBManager.GetStringField(arrResult[i + 3], "");
                int nType = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                string strBroadcastName = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strZoneTextCenter = WebDBManager.GetStringField(arrResult[i + 6], "");

                if (nID == 0)
                    continue;

                if (nType < (int)EquipmentZone.EquipZoneType.SENSOR_TYPE ||
                    nType > (int)EquipmentZone.EquipZoneType.OTHER_TYPE)
                    continue;

                EquipmentZone equipZone = new EquipmentZone();

                equipZone.ID = nID;
                equipZone.ZoneName = strZoneName;
                equipZone.ZoneType = (EquipmentZone.EquipZoneType)nType;
                equipZone.BroadcastName = strBroadcastName;

                strLinkedZones = strLinkedZones.Trim();
                string[] szIds = strLinkedZones.Split(',');

                for (int j = 0; j < szIds.Length; j++)
                {
                    string szID = szIds[j];
                    int nZoneID = -1;
                    if (int.TryParse(szID, out nZoneID))
                    {
                        Zone zone = FindZone(nZoneID);

                        if (zone == null)
                            continue;

                        if (!equipZone.LinkedZoneList.Contains(zone))
                        {
                            equipZone.LinkedZoneList.Add(zone);

                            ArrayList arrEquipZones = null;

                            if (m_dicZoneEquipZones.ContainsKey(zone))
                                arrEquipZones = m_dicZoneEquipZones[zone];
                            else
                            {
                                arrEquipZones = new ArrayList();
                                m_dicZoneEquipZones[zone] = arrEquipZones;
                            }

                            if (!arrEquipZones.Contains(equipZone))
                                arrEquipZones.Add(equipZone);
                        }
                    }
                }
                
                try
                {
                    equipZone.Polygon = MakeZonePolygon(strBoundary);
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                }

                if (string.Compare(strZoneTextCenter, "null", true) == 0)
                    SetNotShowingText(equipZone);

                ParseEquipZoneTextCenter(equipZone, strZoneTextCenter);
            }
        }

        private void SetNotShowingText(EquipmentZone equipZone)
        {
            foreach (Zone zone in equipZone.LinkedZoneList)
            {
                if (!equipZone.NotShowingZone.Contains(zone))
                    equipZone.NotShowingZone.Add(zone);
            }
        }

        private void ParseEquipZoneTextCenter(EquipmentZone equipZone, string strZoneTextCenter)
        {
            strZoneTextCenter = strZoneTextCenter.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
            strZoneTextCenter = strZoneTextCenter.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

            int nBeginIndex = 0;
            int nZoneID;
            double x, y;

            while (nBeginIndex >= 0)
            {
                int nIndex = strZoneTextCenter.IndexOf(')', nBeginIndex);

                if (nIndex < 0)
                    break;

                string strToken = strZoneTextCenter.Substring(nBeginIndex, nIndex - nBeginIndex);
                int nIndex2 = strToken.IndexOf('(');

                if (nIndex2 < 0)
                    break;

                nIndex = strZoneTextCenter.IndexOf(',', nIndex + 1);

                if (nIndex >= 0)
                    nBeginIndex = GetNotEmptyIndex(strZoneTextCenter, nIndex + 1);
                else
                    nBeginIndex = -1;

                string strZoneID = strToken.Substring(0, nIndex2);

                if (!int.TryParse(strZoneID, out nZoneID))
                    break;

                if (!m_dicZones.ContainsKey(nZoneID))
                    break;

                Zone zone = m_dicZones[nZoneID];

                string strCoord = strToken.Substring(nIndex2 + 1);

                if (string.Compare(strCoord, "null", true) == 0)
                {
                    if (!equipZone.NotShowingZone.Contains(zone))
                        equipZone.NotShowingZone.Add(zone);

                    continue;
                }

                int nIndex3 = strToken.IndexOf(',');

                if (nIndex3 < 0)
                    break;

                string strX = strToken.Substring(nIndex2 + 1, nIndex3 - nIndex2 - 1);
                string strY = strToken.Substring(nIndex3 + 1);

                if (!double.TryParse(strX, out x))
                    break;

                if (!double.TryParse(strY, out y))
                    break;

                UnE.Geometry.Vertex2D vCenter = new UnE.Geometry.Vertex2D(x, y);
                equipZone.ZoneTextCenter[zone] = vCenter;
            }
        }

        private int GetNotEmptyIndex(string str, int nBeginIndex, int nEndIndex = -1)
        {
            if (nEndIndex < 0)
                nEndIndex = str.Length - 1;

            for (int i = nBeginIndex; i <= nEndIndex; i++)
            {
                char ch = str.ElementAt(i);

                if (ch != ' ' && ch != '\t' && ch != '\r' && ch != '\n')
                    return i;
            }

            return -1;
        }

        private UnE.Geometry.Polygon MakeZonePolygon(string szBoundary)
        {
            float dx = 121902.5858f; //120894.0548f + 1008.531f;
		    float dy = 157152.8453f; //157659.0963f - 506.251f;

            if (szBoundary == null || szBoundary == "")
                return null;

            UnE.Geometry.Polygon poly = new UnE.Geometry.Polygon();
            int start_idx = 0;
            bool bEnd = false;
            
            do
            {
                int idx = szBoundary.IndexOf(',', start_idx);
                if (idx == -1)
                    break;
                string szPosX = szBoundary.Substring(start_idx, idx - start_idx);
                start_idx = idx + 1;


                idx = szBoundary.IndexOf(',', start_idx);
                string szPosY = "";

                if (idx == -1)
                {
                    int nLength = szBoundary.Length - start_idx;
                    szPosY = szBoundary.Substring(start_idx, nLength);
                    bEnd = true;
                }
                else
                    szPosY = szBoundary.Substring(start_idx, idx - start_idx);

                start_idx = idx + 1;
                double x = Double.Parse(szPosX);
                double y = Double.Parse(szPosY);
                UnE.Geometry.Vertex2D pos = new UnE.Geometry.Vertex2D(x, y);

                float pos3DX = ((float)x - dx);
                float pos3DZ = dy + (float)y;

                poly.AddVertex(pos);
                if (bEnd == true)
                    break;
            }
            while (start_idx < szBoundary.Length);
            
            return poly;
        }

        private void LoadZones()
        {
            if (!System.IO.Directory.Exists(Application.StartupPath + "\\FEData"))
                System.IO.Directory.CreateDirectory(Application.StartupPath + "\\FEData");

            WebDBManager dbMgr = FormMain2.Instance.DBManager;
            string strFtpUrl = dbMgr.LoadIni("dxf_ftp_url");

            if (strFtpUrl == "")
                return;

            //string strSQL = "select id, ZoneName, BuildingID, FloorIndex, AddFloor, Boundary, DXFFileName, DXFAccessedTime, _3DFileName, _3DAccessedTime from Zone";
            //string szTemp = "select id, ZoneName, BuildingID, FloorIndex, AddFloor, Boundary, DXFFileName, DXFAccessedTime, _3DFileName, _3DAccessedTime from Zone where SiteID = {0}";

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT id, ZoneName, BuildingID, FloorIndex, AddFloor, Boundary, DXFFileName, DXFAccessedTime, _3DFileName, _3DAccessedTime");
            sb.Append(",DxfTL, DxfBR, ImgTL, ImgBR, Azimuth");
            sb.AppendFormat(" FROM Zone WHERE SiteID = {0} and id != -1", FormMain2.Instance.SiteID);

            string strSQL = sb.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            DateTime dtDefault = new DateTime();
            int nResultCount = arrResult.Count;
            
            for (int i = 0; i < nResultCount - 14; i += 15)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strZoneName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
                int nBuildingID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFloorIndex = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strAddFloor = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], "0.0");
                string strBoundary = DBUtility.WebDBManager.GetStringField(arrResult[i + 5], "");
                string strDXFFileName = DBUtility.WebDBManager.GetStringField(arrResult[i + 6], "");
                DateTime dtDXF = DBUtility.WebDBManager.GetDateTimeField(arrResult[i + 7], dtDefault);
                string str3DFileName = DBUtility.WebDBManager.GetStringField(arrResult[i + 8], "");
                DateTime dt3D = DBUtility.WebDBManager.GetDateTimeField(arrResult[i + 9], dtDefault);
                
                string szDxfTL = DBUtility.WebDBManager.GetStringField(arrResult[i + 10], "");
                string szDxfBR = DBUtility.WebDBManager.GetStringField(arrResult[i + 11], "");
                string szImgTL = DBUtility.WebDBManager.GetStringField(arrResult[i + 12], "");
                string szImgBR = DBUtility.WebDBManager.GetStringField(arrResult[i + 13], "");
                float fAzimuth = DBUtility.WebDBManager.GetFloatField(arrResult[i + 14].ToString(), 0.0f);

                /*string strChangedZoneName = ToFileString(strZoneName);
                string strFolderPath = Application.StartupPath + "\\FEData\\" + strChangedZoneName;

                if (NeedDownload(strFolderPath, strDXFFileName, "DXF.log", dtDXF))
                {
                    if (DownloadFile(strFtpUrl, strFolderPath, strDXFFileName, FTP_ID, FTP_PW))
                        WriteLog(strFolderPath, "DXF.log", dtDXF);
                }

                if (NeedDownload(strFolderPath, str3DFileName, "3d.log", dt3D))
                {
                    if (DownloadFile(strFtpUrl, strFolderPath, str3DFileName, FTP_ID, FTP_PW))
                        WriteLog(strFolderPath, "3d.log", dt3D);
                }*/

                Zone zone = new Zone();

                zone.ID = nID;
                zone.ZoneName = strZoneName;
                zone.FloorIndex = nFloorIndex;
                zone.Azimuth = fAzimuth;

                string[] xy = szDxfTL.Split(',');
                double x = 0.0f, y = 0.0f;
                if (xy.Length == 2)
                {
                    double.TryParse(xy[0], out x);
                    double.TryParse(xy[1], out y);

                    zone.DxfTL = new UnE.Geometry.Vertex2D(x, y);
                }
                else
                {
                    zone.DxfTL = new UnE.Geometry.Vertex2D(0, 0);
                }

                string[] xy2 = szDxfBR.Split(',');
                if (xy2.Length == 2)
                {
                    double.TryParse(xy2[0], out x);
                    double.TryParse(xy2[1], out y);

                    zone.DxfBR = new UnE.Geometry.Vertex2D(x, y);
                }
                else
                {
                    zone.DxfBR = new UnE.Geometry.Vertex2D(0, 0);
                }

                string[] xy3 = szImgTL.Split(',');
                int fx = 0, fy = 0;
                if (xy3.Length == 2)
                {
                    int.TryParse(xy3[0], out fx);
                    int.TryParse(xy3[1], out fy);

                    zone.ImageTL = new System.Drawing.Point(fx, fy);
                }
                else
                {
                    zone.ImageTL = new System.Drawing.Point(0, 0);
                }

                string[] xy4 = szImgBR.Split(',');
                if (xy4.Length == 2)
                {
                    int.TryParse(xy4[0], out fx);
                    int.TryParse(xy4[1], out fy);

                    zone.ImageBR = new System.Drawing.Point(fx, fy);
                }
                else
                {
                    zone.ImageBR = new System.Drawing.Point(0, 0);
                }
						
                
                try
                {
                    zone.Polygon = MakeZonePolygon(strBoundary);
                }
                catch (System.Exception)
                {
                    zone.Polygon = null;
                    //MessageBox.Show("Make polygo error!!");
                }
                try
                {
                    zone.AddFloor = string.Compare(strAddFloor, "null", true) == 0 ? 0.0f : float.Parse(strAddFloor);
                }
                catch (Exception)
                {
                    zone.AddFloor = 0.0f;
                }

                if (strDXFFileName != "")
                {
                    zone.DXFFilePath = Application.StartupPath + "\\" + FormMain2.Instance.IndoorFolderPath + "\\" + strDXFFileName;
                    //zone.DXFFilePath = Application.StartupPath + "\\FEData\\DXF\\" + strDXFFileName;
                }
                else
                {
                    zone.DXFFilePath = Application.StartupPath + "\\" + FormMain2.Instance.IndoorFolderPath + "\\Blank.png";
                }
              
                if (m_dicBuildings.ContainsKey(nBuildingID))
                    zone.Building = m_dicBuildings[nBuildingID];
                
                m_dicZones[nID] = zone;

                if (nBuildingID < 0)
                    m_dicOutdoorZones[nID] = zone;

                if (zone.Building != null)
                {
                    if (m_dicBuildingZones.ContainsKey(zone.Building.ID))
                    {
                        ArrayList arrZones = m_dicBuildingZones[zone.Building.ID];
                        arrZones.Add(zone);
                    }
                    else
                    {
                        ArrayList arrZones = new ArrayList();
                        m_dicBuildingZones[zone.Building.ID] = arrZones;
                        arrZones.Add(zone);
                    }
                }
            }

            // 층별로 정렬
            foreach (KeyValuePair<int, ArrayList> pair in m_dicBuildingZones)
            {
                pair.Value.Sort();
            }
        }

        public bool GetOutdoorZoneID(string strZoneName, ref int nZoneID)
        {
            int nZoneCount = 0;

            foreach (KeyValuePair<int, Zone> pair in m_dicZones)
            {
                if (pair.Value.Building == null)
                {
                    if (pair.Value.ZoneName == strZoneName)
                    {
                        nZoneID = pair.Value.ID;
                        nZoneCount++;
                    }
                }
            }

            if (nZoneCount > 2)
            {
                System.Diagnostics.Trace.WriteLine(strZoneName + " Count : " + nZoneCount.ToString());
            }

            // 같은 이름을 가진 Zone이 둘 이상이면 어떤 ID인지 구별할 수 없다.
            return nZoneCount == 1;
        }

        public int GetZoneID(Building building, int nFloorIndex, string strDXFPath)
        {
            foreach (KeyValuePair<int, Zone> pair in m_dicZones)
            {
                if (pair.Value.Building != null && building != null)
                {
                    if (pair.Value.Building.BuildingID != building.BuildingID)
                        continue;
                }
                else if (pair.Value.Building == null && building != null)
                    continue;
                else if (pair.Value.Building != null && building == null)
                    continue;

                if (pair.Value.FloorIndex != nFloorIndex)
                    continue;

                if (pair.Value.DXFFilePath == strDXFPath)
                    return pair.Value.ID;
            }

            return -1;
        }

        public int GetBuildingID(string strBuildingID)
        {
            foreach (KeyValuePair<int, Building> pair in m_dicBuildings)
            {
                if (pair.Value.BuildingID == strBuildingID)
                    return pair.Value.ID;
            }

            return -1;
        }

        private void WriteLog(string strFolderPath, string strLogFile, DateTime dtFile)
        {
            string strShortTime = dtFile.ToShortDateString() + " " + dtFile.ToShortTimeString();

            StreamWriter sw = new StreamWriter(strFolderPath + "\\" + strLogFile);
            sw.WriteLine(strShortTime);
            sw.Close();
        }

        private bool DownloadFile(string strFtpUrl, string strFolderPath, string strFileName, string strID, string strPW)
        {
            if (strFileName.Length == 0)
                return true;

            int nIndex = strFileName.LastIndexOf('/');

            if (nIndex < 0)
                nIndex = strFileName.LastIndexOf('\\');

            string strLocalFile = nIndex < 0 ? strFileName : strFileName.Substring(nIndex + 1);

            FtpWebRequest reqFtp = (FtpWebRequest)WebRequest.Create(strFtpUrl + "/" + strFileName);

            reqFtp.UseBinary = true;
            reqFtp.UsePassive = true;
            reqFtp.KeepAlive = true;

            // 사용할 기능 설정
            reqFtp.Method = WebRequestMethods.Ftp.DownloadFile;
            reqFtp.Credentials = new NetworkCredential(strID, strPW);//"sop_user", "1047Ab@#");

            // 요청에 대한 응답을 받습니다.
            FtpWebResponse resFtp = (FtpWebResponse)reqFtp.GetResponse();

            Stream ftpStream = resFtp.GetResponseStream();

            FileStream localFileStream = new FileStream(strFolderPath + "\\" + strLocalFile, FileMode.Create);

            /* Buffer for the Downloaded Data */
            int nBuffSize = 1024;
            byte[] byteBuffer = new byte[nBuffSize];

            int bytesRead = ftpStream.Read(byteBuffer, 0, nBuffSize);
            /* Download the File by Writing the Buffered Data Until the Transfer is Complete */
            try
            {
                while (bytesRead > 0)
                {
                    localFileStream.Write(byteBuffer, 0, bytesRead);
                    bytesRead = ftpStream.Read(byteBuffer, 0, nBuffSize);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            /* Resource Cleanup */
            localFileStream.Close();
            ftpStream.Close();
            resFtp.Close();

            return true;
        }

        private bool NeedDownload(string strFolderPath, string strFileName, string strLogFileName, DateTime dtFile)
        {
            // 나중에 살릴것
            return false;
            if (!FormMain2.Instance.IsPCMode)
                return false;

            if (!Directory.Exists(strFolderPath))
            {
                Directory.CreateDirectory(strFolderPath);
            }

            if (string.Compare(strFileName, "null") == 0)
                return false;

            string strFilePath = strFolderPath + "\\" + strLogFileName;
            bool needDownload = true;

            if (File.Exists(strFilePath))
            {
                StreamReader reader = new StreamReader(strFilePath);
                string strTime = reader.ReadLine();
                reader.Close();

                DateTime time = Convert.ToDateTime(strTime);

                if (time >= dtFile)
                    needDownload = false;
            }

            return needDownload;
        }

        public Zone FindZone(Building building, float fFloorIndex)
        {
            int nFloorIndex = fFloorIndex > 0.0f ? (int)(fFloorIndex + 0.01f) : (int)(fFloorIndex - 0.01f);
            string strAddFloor = string.Format("{0:f1}", fFloorIndex - nFloorIndex);
            int i = 0;
            foreach (KeyValuePair<int, Zone> pair in m_dicZones)
            {
                Zone zone = pair.Value;
                i++;
                if (zone.Building == null)
                    continue;

                if (zone.Building != null && zone.Building.ToString() == building.ToString() && zone.FloorIndex == nFloorIndex)
                {
                    if (strAddFloor == string.Format("{0:f1}", zone.AddFloor))
                        return zone;
                }
            }

            return null;
        }

        public Zone FindZone(int nZoneID)
        {
            if (nZoneID == -1)
                return null;

            if (m_dicZones.ContainsKey(nZoneID))
                return m_dicZones[nZoneID];

            return null;
        }

        public BuildingGroup FindBuildingGroup(int nBuildingGroupID)
        {
            foreach (KeyValuePair<BuildingGroup, ArrayList> pair in m_dicBuildingGroups)
            {
                if (pair.Key.ID == nBuildingGroupID)
                    return pair.Key;
            }

            return null;
        }

        public Building FindBuilding(int nBuildingID)
        {
            if (m_dicBuildings.ContainsKey(nBuildingID))
                return m_dicBuildings[nBuildingID];

            return null;
        }

        // RFID Tag로 찾는다.
        public FireEquipment FindEquipment(string strRFIDTag)
        {
            if (m_dicRFIDFireEquipments.ContainsKey(strRFIDTag))
                return m_dicRFIDFireEquipments[strRFIDTag];

            return null;
        }

        // 설비 Type과 관리번호로 찾는다.
        public FireEquipment FindEquipment(FireEquipment.EquipmentType type, string strEquipID)
        {
            string strKey = GetEquipIDString(type, strEquipID);

            if (m_dicEquipIDFireEquipments.ContainsKey(strKey))
                return m_dicEquipIDFireEquipments[strKey];

            return null;
        }

        // DXF 도면 정보로부터 설비를 찾는다.
        public FireEquipment FindEquipment(string strDxfObjID, Zone zone)
        {
            if (!m_dicZoneEquipments.ContainsKey(zone))
                return null;

            ArrayList arrEquipments = m_dicZoneEquipments[zone];

            foreach (FireEquipment equip in arrEquipments)
            {
                if (equip.DXFObjID == strDxfObjID)
                    return equip;
            }

            return null;
        }

        public ArrayList FindEquipmentHistoryList(int nEquipmentID)
        {
            if (m_dicEquipmentHistory.ContainsKey(nEquipmentID))
                return m_dicEquipmentHistory[nEquipmentID];
            return null;
        }

        public ArrayList FindDBEquipmentHistoryList(int nEquipmentID)
        {
            if (m_dicDBEquipmentHistory.ContainsKey(nEquipmentID))
                return m_dicDBEquipmentHistory[nEquipmentID];
            return null;
        }

        // zone 내에 포함된 모든 설비들의 History를 삭제한다.
        public void ClearZoneEquipmentHistoryList(Zone zone)
        {
            ArrayList arrEquipments = GetEquipments(zone);

            foreach (FireEquipment equip in arrEquipments)
            {
                m_dicEquipmentHistory.Remove(equip.ID);
            }
        }

        public void AddEquipmentHistory(FireEquipmentHistory history)
        {
            if (history == null || !history.IsNewHistory)
                return;

            ArrayList arrHistory = FindEquipmentHistoryList(history.EquipmentID);

            if (arrHistory != null)
            {
                int nCount = arrHistory.Count;

                if (nCount > 0)
                {
                    FireEquipmentHistory lastHistory = (FireEquipmentHistory)arrHistory[nCount - 1];

                    // 마지막에 저장된 History가 아직 System에 저장되지 않은 것이라면
                    // history로 대체한다.
                    if (lastHistory.IsNewHistory)
                        arrHistory.RemoveAt(nCount - 1);
                }

                arrHistory.Add(history);
            }
            else
            {
                arrHistory = new ArrayList();
                arrHistory.Add(history);
                m_dicEquipmentHistory[history.EquipmentID] = arrHistory;
            }
        }
    
        private bool CheckRFIDDuplication(string strRFID, ArrayList arrEquipmnets)
        {
            foreach (FireEquipment equip in arrEquipmnets)
            {
                // CurrentZone은 따로 검사한다.
                if (equip.RFIDTag == strRFID)
                {
                    string strMsg = string.Format("이미 같은 RFID가 [{0}]에 존재합니다.\r\n[{1}], [{2}]\r\n기존 설비의 RFID 값을 확인해 주십시오.",
                        equip.Zone.ZoneName, strRFID, equip.EquipID);
                    MessageBox.Show(strMsg);
                    return false;
                }
            }

            return true;
        }

        public bool CheckRFIDDuplication(string strRFID)
        {
            Zone zoneCurrent = FormMain2.Instance.CurrentZone;

            if (zoneCurrent != null && m_dicZoneEquipments.ContainsKey(zoneCurrent))
            {
                ArrayList arrEquipments = m_dicZoneEquipments[zoneCurrent];

                if (!CheckRFIDDuplication(strRFID, arrEquipments))
                    return false;
            }

            foreach (KeyValuePair<Zone, ArrayList> pair in m_dicZoneEquipments)
            {
                // CurrentZone은 따로 검사한다.
                if (pair.Key == zoneCurrent)
                    continue;

                if (!CheckRFIDDuplication(strRFID, pair.Value))
                    return false;
            }

            return true;
        }

        public bool CheckRFIDDuplication(string strRFID, string strRFIDTagID, FireEquipment.EquipmentType type, string strEquipID, string strLocationName, float x, float y)
        {
            Zone zoneCurrent = FormMain2.Instance.CurrentZone;
            if (zoneCurrent == null)
                return false;

            if (m_dicZoneEquipments.ContainsKey(zoneCurrent))
            {
                ArrayList arrEquipments = m_dicZoneEquipments[zoneCurrent];

                if (!CheckRFIDDuplication(strRFID, arrEquipments))
                    return false;
            }

            foreach (KeyValuePair<Zone, ArrayList> pair in m_dicZoneEquipments)
            {
                // CurrentZone은 따로 검사한다.
                if (pair.Key == zoneCurrent)
                    continue;

                if (!CheckRFIDDuplication(strRFID, pair.Value))
                    return false;
                /*foreach (FireEquipment equip in pair.Value)
                {
                    // CurrentZone은 따로 검사한다.
                    if (equip.Zone != zoneCurrent && equip.RFIDTag == strRFID)
                    {
                        string strMsg = string.Format("이미 같은 RFID가 [{0}]에 존재합니다.\r\n[{1}], [{2}]\r\n기존 설비의 RFID 값을 확인해 주십시오.",
                            pair.Key.ZoneName, strRFID, equip.EquipID);
                        MessageBox.Show(strMsg);
                        return false;
                    }
                }*/
            }

            return true;
        }

        // DB에서 비교
        //public bool CheckRFIDDuplication(string strRFID, string strRFIDTagID, FireEquipment.EquipmentType type, string strEquipID, string strLocationName, float x, float y)
        //{
        //    Zone zoneCurrent = FormMain2.Instanc.CurrentZone;
        //    if (zoneCurrent == null)
        //        return false;

        //    WebDBManager dbMgr = FormMain2.Instance.DBManager;

        //    string strSQL = string.Format("select fe.id, RFIDTagID, EquipID, ZoneID, z.ZoneName from FireEquipment as fe, Zone as z where RFIDTag = '{0}' and fe.ZoneID = z.ID", strRFID);
        //    ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

        //    if (arrResult == null)
        //        return false;

        //    int nResultCount = arrResult.Count;
        //    string strRemoveIDs = "";

        //    for (int i = 0; i < nResultCount - 5; i += 6)
        //    {
        //        int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
        //        string _strRFIDTagID = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
        //        string _strEquipID = DBUtility.WebDBManager.GetStringField(arrResult[i + 2], "");
        //        int nZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
        //        string strZoneName = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], "");

        //        if (nZoneID != zoneCurrent.ID)
        //        {
        //            string strMsg = string.Format("이미 같은 RFID가 [{0}]에 존재합니다.\r\n[{1}], [{2}]\r\n기존 설비의 RFID 값을 확인해 주십시오.", 
        //                strZoneName, strRFID, _strEquipID);
        //            MessageBox.Show(strMsg);
        //            return false;
        //            /*string strMsg = string.Format("이미 같은 RFID가 '{0}'에 존재합니다.\r\n'{1}', '{2}'\r\n해당 Tag 정보를 삭제하고 '{3}'에 새로운 설비로 등록하시겠습니까?",
        //                strZoneName, strRFID, _strEquipID, zoneCurrent.ZoneName);

        //            if (MessageBox.Show(strMsg, "RFID 중복", MessageBoxButtons.YesNo) == DialogResult.Yes)
        //            {
        //                if (strRemoveIDs.Length == 0)
        //                    strRemoveIDs = nID.ToString();
        //                else
        //                    strRemoveIDs += ", " + nID.ToString();
        //            }
        //            else
        //                return false;*/
        //        }
        //    }

        //    if (strRemoveIDs.Length > 0)
        //    {
        //        strSQL = string.Format("delete from FireEquipment where id in ({0})", strRemoveIDs);
        //        if (dbMgr.GetResultData(strSQL, 0) == null)
        //            return false;
        //    }

        //    return true;
        //}

        public void ClearEquipments()
        {
            m_dicZoneEquipments.Clear();
            m_dicRFIDFireEquipments.Clear();
            m_dicEquipIDFireEquipments.Clear();
        }

        public void ClearDBEquipments()
        {
            m_dicDBZoneEquipments.Clear();
        }

        public ArrayList GetEquipments(Zone zone)
        {
            if (m_dicZoneEquipments.ContainsKey(zone))
                return m_dicZoneEquipments[zone];

            ArrayList arrEquipments = new ArrayList();
            m_dicZoneEquipments[zone] = arrEquipments;

            return arrEquipments;
        }

        public ArrayList GetDBEquipments(Zone zone)
        {
            if (m_dicDBZoneEquipments.ContainsKey(zone))
                return m_dicDBZoneEquipments[zone];

            ArrayList arrEquipments = new ArrayList();
            m_dicDBZoneEquipments[zone] = arrEquipments;

            return arrEquipments;
        }

        public bool AddEquipment(FireEquipment equip, Zone zone)
        {
            if (equip == null || zone == null)
                return false;

            if (m_dicZoneEquipments.ContainsKey(zone))
            {
                ArrayList arrEquipments = m_dicZoneEquipments[zone];

                if (!arrEquipments.Contains(equip))
                    arrEquipments.Add(equip);
            }
            else
            {
                ArrayList arrEquipments = new ArrayList();
                arrEquipments.Add(equip);
                m_dicZoneEquipments[zone] = arrEquipments;
            }

            m_dicRFIDFireEquipments[equip.RFIDTag] = equip;
            m_dicEquipIDFireEquipments[GetEquipIDString(equip)] = equip;

            return true;
        }

        public bool AddDBEquipment(FireEquipment equip, Zone zone)
        {
            if (equip == null || zone == null)
                return false;

            if (m_dicDBZoneEquipments.ContainsKey(zone))
            {
                ArrayList arrEquipments = m_dicDBZoneEquipments[zone];

                if (!arrEquipments.Contains(equip))
                    arrEquipments.Add(equip);
            }
            else
            {
                ArrayList arrEquipments = new ArrayList();
                arrEquipments.Add(equip);
                m_dicDBZoneEquipments[zone] = arrEquipments;
            }

            return true;
        }

        // 수정된 설비 정보를 전체 Data에 반영한다.
        public void ApplyEquipments(ArrayList arrEquipments, Zone zone)
        {
            if (zone == null)
                return;

            if (m_dicZoneEquipments.ContainsKey(zone))
            {
                ArrayList arrOldEquipments = m_dicZoneEquipments[zone];

                foreach (FireEquipment oldEquip in arrOldEquipments)
                {
                    m_dicRFIDFireEquipments.Remove(oldEquip.RFIDTag);
                    m_dicEquipIDFireEquipments.Remove(GetEquipIDString(oldEquip));
                }

                arrOldEquipments.Clear();

                foreach (FireEquipment equip in arrEquipments)
                {
                    arrOldEquipments.Add(equip);
                    m_dicRFIDFireEquipments[equip.RFIDTag] = equip;
                    m_dicEquipIDFireEquipments[GetEquipIDString(equip)] = equip;
                }
            }
            else
            {
                ArrayList arrNewEquipments = new ArrayList();

                foreach (FireEquipment equip in arrEquipments)
                {
                    arrNewEquipments.Add(equip);
                    m_dicRFIDFireEquipments[equip.RFIDTag] = equip;
                    m_dicEquipIDFireEquipments[GetEquipIDString(equip)] = equip;
                }

                m_dicZoneEquipments[zone] = arrNewEquipments;
            }
        }

        public void ApplyEquipmentHistory(Dictionary<FireEquipment, FireEquipmentHistory> dicEquipmentHistory)
        {
            foreach (KeyValuePair<FireEquipment, FireEquipmentHistory> pair in dicEquipmentHistory)
            {
                if (pair.Key.ID < 0)
                    continue;

                ArrayList arrHistory = null;

                if (!m_dicEquipmentHistory.ContainsKey(pair.Key.ID))
                {
                    arrHistory = new ArrayList();
                    m_dicEquipmentHistory[pair.Key.ID] = arrHistory;
                }
                else
                    arrHistory = m_dicEquipmentHistory[pair.Key.ID];

                if (!arrHistory.Contains(pair.Value))
                    arrHistory.Add(pair.Value);
            }

            // 시스템에 저장된 History로 지정
            foreach (KeyValuePair<int, ArrayList> pair in m_dicEquipmentHistory)
            {
                ArrayList arrHistory = pair.Value;

                foreach (FireEquipmentHistory history in arrHistory)
                {
                    history.IsNewHistory = false;
                }
            }
        }

        public ArrayList GetBuildingZones(int nBuildingID)
        {
            if (m_dicBuildingZones.ContainsKey(nBuildingID))
                return m_dicBuildingZones[nBuildingID];

            return null;
        }

        public void DeleteEquipment(FireEquipment equip)
        {
            if (equip.Zone != null)
            {
                if (m_dicZoneEquipments.ContainsKey(equip.Zone))
                {
                    ArrayList arrEquipments = m_dicZoneEquipments[equip.Zone];
                    arrEquipments.Remove(equip);
                }
            }

            if (equip.ID > 0)
            {
                if (m_dicEquipmentHistory.ContainsKey(equip.ID))
                {
                    m_dicEquipmentHistory.Remove(equip.ID);
                }
            }
        }

        // 시스템에 저장된 설비 데이터들을 zone에 있는 것과 일치시킨다.
        public bool MakeSameEquipments(Zone zone)
        {
            ArrayList arrEquipments = GetEquipments(zone);
            ArrayList arrDBEquipments = GetDBEquipments(zone);

            int nEquipCount1 = arrEquipments.Count;
            int nEquipCount2 = arrDBEquipments.Count;

            if (nEquipCount1 != nEquipCount2)
                return false;

            for (int i = 0; i < nEquipCount1; i++)
            {
                FireEquipment equip1 = (FireEquipment)arrEquipments[i];
                FireEquipment equip2 = (FireEquipment)arrDBEquipments[i];

                if (!equip1.IsSame(equip2))
                    return false;

                ArrayList arrEquipHistory1 = FindEquipmentHistoryList(equip1.ID);
                ArrayList arrEquipHistory2 = FindDBEquipmentHistoryList(equip2.ID);

                if (arrEquipHistory1 == null && arrEquipHistory2 == null)
                    continue;
                else if (arrEquipHistory1 != null && arrEquipHistory2 == null)
                {
                    arrEquipHistory2 = new ArrayList();
                    m_dicDBEquipmentHistory[equip2.ID] = arrEquipHistory2;
                }

                int nHistoryCount1 = arrEquipHistory1.Count;
                arrEquipHistory2.Clear();

                for (int j = 0; j < nHistoryCount1; j++)
                {
                    FireEquipmentHistory history1 = (FireEquipmentHistory)arrEquipHistory1[j];
                    FireEquipmentHistory history2 = new FireEquipmentHistory(history1);
                    arrEquipHistory2.Add(history2);
                }
            }

            // 설비정보가 일치하므로 혹시 m_arrChangedZones에 들어있다면 제거한다.
            m_arrChangedZones.Remove(zone);
            return true;
        }

        // zone에 있는 설비 데이터들이 시스템에 저장된 것과 일치하는지 여부를 검사한다.
        // 일치하지 않는 Zone은 m_arrChangedZones에 저장된다.
        // Return 값 : true(시스템과 일치)
        //             false(시스템과 일치하지 않음)
        public bool CompareZoneEquipmentsToDB(Zone zone)
        {
            ArrayList arrEquipments = GetEquipments(zone);
            ArrayList arrDBEquipments = GetDBEquipments(zone);

            int nEquipCount1 = arrEquipments.Count;
            int nEquipCount2 = arrDBEquipments.Count;

            if (nEquipCount1 != nEquipCount2)
                goto RETURN_FALSE;

            for (int i = 0; i < nEquipCount1; i++)
            {
                FireEquipment equip1 = (FireEquipment)arrEquipments[i];
                FireEquipment equip2 = (FireEquipment)arrDBEquipments[i];

                if (!equip1.IsSame(equip2))
                    goto RETURN_FALSE;

                ArrayList arrEquipHistory1 = FindEquipmentHistoryList(equip1.ID);
                ArrayList arrEquipHistory2 = FindDBEquipmentHistoryList(equip2.ID);

                if (arrEquipHistory1 == null && arrEquipHistory2 == null)
                    continue;
                else if (arrEquipHistory1 == null || arrEquipHistory2 == null)
                    goto RETURN_FALSE;

                int nHistoryCount1 = arrEquipHistory1.Count;
                int nHistoryCount2 = arrEquipHistory2.Count;

                if (nHistoryCount1 != nHistoryCount2)
                    goto RETURN_FALSE;

                // History가 다른 경우는 맨 뒤의 경우가 다른것이 대부분이므로 뒤에서부터 검사한다.
                for (int j = nHistoryCount1 - 1; j >= 0; j--)
                {
                    FireEquipmentHistory history1 = (FireEquipmentHistory)arrEquipHistory1[j];
                    FireEquipmentHistory history2 = (FireEquipmentHistory)arrEquipHistory2[j];

                    if (!history1.IsSame(history2))
                        goto RETURN_FALSE;
                }
            }

            // 설비정보가 일치하므로 혹시 m_arrChangedZones에 들어있다면 제거한다.
            m_arrChangedZones.Remove(zone);
            return true;

        RETURN_FALSE:
            if (!m_arrChangedZones.Contains(zone))
                m_arrChangedZones.Add(zone);
            return false;
        }

        public void AddChangedZone(Zone zone)
        {
            if (!m_arrChangedZones.Contains(zone))
                m_arrChangedZones.Add(zone);
        }

        public void WriteEquipmentLog(StreamWriter writer, DateTime dtNow, string strDelimeter)
        {
            int nTotalEquipCount = 0, nTotalFECount = 0, nTotalHDCount = 0, nTotalFACount = 0, nTotalFRCount = 0;
            string strLog = "";

            foreach (KeyValuePair<Zone, ArrayList> pair in m_dicZoneEquipments)
            {
                int nFECount = 0, nHDCount = 0, nFACount = 0, nFRCount = 0;

                foreach (FireEquipment equip in pair.Value)
                {
                    if (equip.Type == FireEquipment.EquipmentType.FE)
                        nFECount++;
                    else if (equip.Type == FireEquipment.EquipmentType.HD)
                        nHDCount++;
                    else if (equip.Type == FireEquipment.EquipmentType.FA)
                        nFACount++;
                    else if (equip.Type == FireEquipment.EquipmentType.FR)
                        nFRCount++;

                }

                if (strLog.Length == 0)
                    strLog = GetZoneInfo(pair.Key, strDelimeter) + strDelimeter + nFECount.ToString() + strDelimeter + nHDCount.ToString() + strDelimeter + nFACount.ToString();
                else
                    strLog += "\r\n" + GetZoneInfo(pair.Key, strDelimeter) + strDelimeter + nFECount.ToString() + strDelimeter + nHDCount.ToString() + strDelimeter + nFACount.ToString();

                nTotalFECount += nFECount;
                nTotalHDCount += nHDCount;
                nTotalFACount += nFACount;
                nTotalFRCount += nFRCount;
            }

            nTotalEquipCount = nTotalFECount + nTotalHDCount + nTotalFACount + nTotalFRCount;

            writer.WriteLine(string.Format("{0}년 {1}월 {2}일 현재 설비 전체 개수{4}{3}", 
                dtNow.Year, dtNow.Month, dtNow.Day, nTotalEquipCount, strDelimeter));

            if (nTotalFECount > 0)
                writer.WriteLine(string.Format("전체 소화기 개수{0}{1}", strDelimeter, nTotalFECount));

            if (nTotalHDCount > 0)
                writer.WriteLine(string.Format("전체 소화전 개수{0}{1}", strDelimeter, nTotalHDCount));

            if (nTotalFACount > 0)
                writer.WriteLine(string.Format("전체 발신기 개수{0}{1}", strDelimeter, nTotalFACount));

            if (nTotalFRCount > 0)
                writer.WriteLine(string.Format("전체 수신반 개수{0}{1}", strDelimeter, nTotalFRCount));

            writer.WriteLine();
            writer.WriteLine();

            writer.WriteLine(string.Format("건물그룹{0}건물명{0}Zone이름{0}소화기개수{0}소화전개수{0}발신기개수{0}수신반개수", strDelimeter));
            writer.WriteLine(strLog);
            writer.WriteLine();
        }

        private string GetZoneInfo(Zone zone, string strDelimeter)
        {
            if (zone.Building == null)
                return strDelimeter + strDelimeter + zone.ZoneName;

            return zone.Building.BuildingGroup.BuildingGroupName + strDelimeter + zone.Building.BuildingName + strDelimeter + zone.ZoneName;
        }

        public ArrayList GetEquipmentZoneList(Zone zone)
        {
            if (!m_dicZoneEquipZones.ContainsKey(zone))
            {
                ArrayList arrEquipZones = new ArrayList();
                m_dicZoneEquipZones[zone] = arrEquipZones;
                return arrEquipZones;
            }

            return m_dicZoneEquipZones[zone];
        }

        public void AddEquipmentZone(EquipmentZone equipZone, Zone zone)
        {
            if (equipZone == null || zone == null)
                return;

            if (!m_dicZoneEquipZones.ContainsKey(zone))
            {
                ArrayList arrEquipZones = new ArrayList();
                m_dicZoneEquipZones[zone] = arrEquipZones;
                arrEquipZones.Add(equipZone);
            }
            else
            {
                ArrayList arrEquipZones = m_dicZoneEquipZones[zone];
                arrEquipZones.Add(equipZone);
            }
        }

        public Dictionary<int, Zone> AllZones
        {
            get { return m_dicZones; }
        }

        public Dictionary<BuildingGroup, ArrayList> AllBuildingGroups
        {
            get { return m_dicBuildingGroups; }
        }

        public Dictionary<int, Building> AllBuildings
        {
            get { return m_dicBuildings; }
        }

        public Dictionary<int, Zone> OutdoorZones
        {
            get { return m_dicOutdoorZones; }
        }

        public Dictionary<int, ArrayList> EquipmentHistory
        {
            get { return m_dicEquipmentHistory; }
            set { m_dicEquipmentHistory = value; }
        }

        public Dictionary<int, ArrayList> DBEquipmentHistory
        {
            get { return m_dicDBEquipmentHistory; }
            set { m_dicDBEquipmentHistory = value; }
        }

        public string TabletDataFile
        {
            get { return m_strTabletDataFile; }
        }

        // Building별 Zone List
        // Building ID, Zone List
        public Dictionary<int, ArrayList> BuildingZones
        {
            get { return m_dicBuildingZones; }
        }

        public ArrayList ChangedZones
        {
            get { return m_arrChangedZones; }
        }
    }  
}
