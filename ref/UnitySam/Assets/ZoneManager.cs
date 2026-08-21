using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using DBUtility;
using UnityEngine;

namespace SDMS
{
    public class ZoneManager
    {
        private static ZoneManager m_Instance = null;

        public static ZoneManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new ZoneManager();
                return m_Instance;
            }
        }

        private Dictionary<int, EquipmentZone> m_dicEquipZones = new Dictionary<int, EquipmentZone>();

        public Dictionary<int, EquipmentZone> DicEquipZones
        {
            get { return m_dicEquipZones; }
            set { m_dicEquipZones = value; }
        }

        // Zone별 EquipmentZone List
        private Dictionary<Zone, ArrayList> m_dicZoneEquipZones = new Dictionary<Zone, ArrayList>();

        private Dictionary<int, EquipmentZone> m_dicOutdoorEquipZones = new Dictionary<int, EquipmentZone>();

        public Dictionary<int, EquipmentZone> DicOutdoorEquipZones
        {
            get { return m_dicOutdoorEquipZones; }
            set { m_dicOutdoorEquipZones = value; }
        }

        private Dictionary<int, Zone> m_dicZones = new Dictionary<int, Zone>();

        public Dictionary<int, Zone> DicZones
        {
            get { return m_dicZones; }
            set { m_dicZones = value; }
        }

        private Dictionary<int, Building> m_dicBuildings = new Dictionary<int, Building>();

        public Dictionary<int, Building> DicBuildings
        {
            get { return m_dicBuildings; }
            set { m_dicBuildings = value; }
        }

        private Dictionary<int, BuildingGroup> m_dicBuildingGroup = new Dictionary<int, BuildingGroup>();

        public Dictionary<int, BuildingGroup> DicBuildingGroup
        {
            get { return m_dicBuildingGroup; }
            set { m_dicBuildingGroup = value; }
        }

        private BuildingGroup m_outdoorBuildingGroup = new BuildingGroup();

        public BuildingGroup OutdoorBuildingGroup
        {
            get { return m_outdoorBuildingGroup; }
        }

        private Dictionary<int, Zone> m_dicOutdoorZones = new Dictionary<int, Zone>();

        public Dictionary<int, Zone> DicOutdoorZones
        {
            get { return m_dicOutdoorZones; }
            set { m_dicOutdoorZones = value; }
        }

        //Building에 속해있는 Zone List(Building ID, Zone List)
        private Dictionary<int, ArrayList> m_dicBuildingZones = new Dictionary<int, ArrayList>();

        public Dictionary<int, ArrayList> DicBuildingZones
        {
            get { return m_dicBuildingZones; }
            set { m_dicBuildingZones = value; }
        }

        private float dx = 121902.5858f; //120894.0548f + 1008.531f;

        public float Dx
        {
            get { return dx; }
            set { dx = value; }
        }

        private float dy = -157152.8453f; //157659.0963f - 506.251f;

        public float Dy
        {
            get { return dy; }
            set { dy = value; }
        }

        private int m_nSiteID = 1;

        private List<_3DText> m_3DTextList = new List<_3DText>();

        public List<_3DText> _3DTextList
        {
            get { return m_3DTextList; }
        }

        public ZoneManager()
        {
            m_nSiteID = ModelManager.Instance.SiteID;

            m_outdoorBuildingGroup.BuildingGroupName = "외부 영역";
        }

        public Encoding detectTextEncoding(byte[] b, out String text, int taster = 1000)
        {
            //byte[] b = File.ReadAllBytes(filename);

            //////////////// First check the low hanging fruit by checking if a
            //////////////// BOM/signature exists (sourced from http://www.unicode.org/faq/utf_bom.html#bom4)
            if (b.Length >= 4 && b[0] == 0x00 && b[1] == 0x00 && b[2] == 0xFE && b[3] == 0xFF) { text = Encoding.GetEncoding("utf-32BE").GetString(b, 4, b.Length - 4); return Encoding.GetEncoding("utf-32BE"); }  // UTF-32, big-endian 
            else if (b.Length >= 4 && b[0] == 0xFF && b[1] == 0xFE && b[2] == 0x00 && b[3] == 0x00) { text = Encoding.UTF32.GetString(b, 4, b.Length - 4); return Encoding.UTF32; }    // UTF-32, little-endian
            else if (b.Length >= 2 && b[0] == 0xFE && b[1] == 0xFF) { text = Encoding.BigEndianUnicode.GetString(b, 2, b.Length - 2); return Encoding.BigEndianUnicode; }     // UTF-16, big-endian
            else if (b.Length >= 2 && b[0] == 0xFF && b[1] == 0xFE) { text = Encoding.Unicode.GetString(b, 2, b.Length - 2); return Encoding.Unicode; }              // UTF-16, little-endian
            else if (b.Length >= 3 && b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF) { text = Encoding.UTF8.GetString(b, 3, b.Length - 3); return Encoding.UTF8; } // UTF-8
            else if (b.Length >= 3 && b[0] == 0x2b && b[1] == 0x2f && b[2] == 0x76) { text = Encoding.UTF7.GetString(b, 3, b.Length - 3); return Encoding.UTF7; } // UTF-7


            //////////// If the code reaches here, no BOM/signature was found, so now
            //////////// we need to 'taste' the file to see if can manually discover
            //////////// the encoding. A high taster value is desired for UTF-8
            if (taster == 0 || taster > b.Length) taster = b.Length;    // Taster size can't be bigger than the filesize obviously.


            // Some text files are encoded in UTF8, but have no BOM/signature. Hence
            // the below manually checks for a UTF8 pattern. This code is based off
            // the top answer at: http://stackoverflow.com/questions/6555015/check-for-invalid-utf8
            // For our purposes, an unnecessarily strict (and terser/slower)
            // implementation is shown at: http://stackoverflow.com/questions/1031645/how-to-detect-utf-8-in-plain-c
            // For the below, false positives should be exceedingly rare (and would
            // be either slightly malformed UTF-8 (which would suit our purposes
            // anyway) or 8-bit extended ASCII/UTF-16/32 at a vanishingly long shot).
            int i = 0;
            bool utf8 = false;
            while (i < taster - 4)
            {
                if (b[i] <= 0x7F) { i += 1; continue; }     // If all characters are below 0x80, then it is valid UTF8, but UTF8 is not 'required' (and therefore the text is more desirable to be treated as the default codepage of the computer). Hence, there's no "utf8 = true;" code unlike the next three checks.
                if (b[i] >= 0xC2 && b[i] <= 0xDF && b[i + 1] >= 0x80 && b[i + 1] < 0xC0) { i += 2; utf8 = true; continue; }
                if (b[i] >= 0xE0 && b[i] <= 0xF0 && b[i + 1] >= 0x80 && b[i + 1] < 0xC0 && b[i + 2] >= 0x80 && b[i + 2] < 0xC0) { i += 3; utf8 = true; continue; }
                if (b[i] >= 0xF0 && b[i] <= 0xF4 && b[i + 1] >= 0x80 && b[i + 1] < 0xC0 && b[i + 2] >= 0x80 && b[i + 2] < 0xC0 && b[i + 3] >= 0x80 && b[i + 3] < 0xC0) { i += 4; utf8 = true; continue; }
                utf8 = false; break;
            }
            if (utf8 == true)
            {
                text = Encoding.UTF8.GetString(b);
                return Encoding.UTF8;
            }


            // The next check is a heuristic attempt to detect UTF-16 without a BOM.
            // We simply look for zeroes in odd or even byte places, and if a certain
            // threshold is reached, the code is 'probably' UF-16.          
            double threshold = 0.1; // proportion of chars step 2 which must be zeroed to be diagnosed as utf-16. 0.1 = 10%
            int count = 0;
            for (int n = 0; n < taster; n += 2) if (b[n] == 0) count++;
            if (((double)count) / taster > threshold) { text = Encoding.BigEndianUnicode.GetString(b); return Encoding.BigEndianUnicode; }
            count = 0;
            for (int n = 1; n < taster; n += 2) if (b[n] == 0) count++;
            if (((double)count) / taster > threshold) { text = Encoding.Unicode.GetString(b); return Encoding.Unicode; } // (little-endian)


            // Finally, a long shot - let's see if we can find "charset=xyz" or
            // "encoding=xyz" to identify the encoding:
            for (int n = 0; n < taster - 9; n++)
            {
                if (
                    ((b[n + 0] == 'c' || b[n + 0] == 'C') && (b[n + 1] == 'h' || b[n + 1] == 'H') && (b[n + 2] == 'a' || b[n + 2] == 'A') && (b[n + 3] == 'r' || b[n + 3] == 'R') && (b[n + 4] == 's' || b[n + 4] == 'S') && (b[n + 5] == 'e' || b[n + 5] == 'E') && (b[n + 6] == 't' || b[n + 6] == 'T') && (b[n + 7] == '=')) ||
                    ((b[n + 0] == 'e' || b[n + 0] == 'E') && (b[n + 1] == 'n' || b[n + 1] == 'N') && (b[n + 2] == 'c' || b[n + 2] == 'C') && (b[n + 3] == 'o' || b[n + 3] == 'O') && (b[n + 4] == 'd' || b[n + 4] == 'D') && (b[n + 5] == 'i' || b[n + 5] == 'I') && (b[n + 6] == 'n' || b[n + 6] == 'N') && (b[n + 7] == 'g' || b[n + 7] == 'G') && (b[n + 8] == '='))
                    )
                {
                    if (b[n + 0] == 'c' || b[n + 0] == 'C') n += 8; else n += 9;
                    if (b[n] == '"' || b[n] == '\'') n++;
                    int oldn = n;
                    while (n < taster && (b[n] == '_' || b[n] == '-' || (b[n] >= '0' && b[n] <= '9') || (b[n] >= 'a' && b[n] <= 'z') || (b[n] >= 'A' && b[n] <= 'Z')))
                    { n++; }
                    byte[] nb = new byte[n - oldn];
                    Array.Copy(b, oldn, nb, 0, n - oldn);
                    try
                    {
                        string internalEnc = Encoding.ASCII.GetString(nb);
                        text = Encoding.GetEncoding(internalEnc).GetString(b);
                        return Encoding.GetEncoding(internalEnc);
                    }
                    catch { break; }    // If C# doesn't recognize the name of the encoding, break.
                }
            }


            // If all else fails, the encoding is probably (though certainly not
            // definitely) the user's local codepage! One might present to the user a
            // list of alternative encodings as shown here: http://stackoverflow.com/questions/8509339/what-is-the-most-common-encoding-of-each-language
            // A full list can be found using Encoding.GetEncodings();
            text = Encoding.Default.GetString(b);
            return Encoding.Default;
        }
        
        public void LoadBuildingData()
        {
            WebDBManager webDB = ModelManager.Instance.WebDB;

            string szText = "SELECT bd.id, bd.BuildingID, bd.BuildingCode, bd.BuildingName, bd.BuildingGroupID, " +
                            "  bd.MaxFloor, bd.MinFloor, bg.GroupName, " +
                            //bg.DisplayText, 
                            "bg.TextCenter, bd.BroadCastingText" +
                            //, bd.DisplayText " +
                            "  FROM Building AS bd INNER JOIN BuildingGroup AS bg ON bd.BuildingGroupID = bg.ID AND bg.SiteID = {0} " +
                            "  ORDER BY bg.ID,  bd.ID";

            string strSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = webDB.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;
 
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 9; i += 10)
            {
                try
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strBuildingID = WebDBManager.GetStringField(arrResult[i + 1], "");
                    string strBuildingCode = WebDBManager.GetStringField(arrResult[i + 2], "");

                    string strBuildingName = WebDBManager.GetStringField(arrResult[i + 3], "");
           
                    //string strBuildingName = Encoding.UTF8.(WebDBManager.GetStringField(arrResult[i + 3], ""))
                    int nBuildingGroupID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                    int nMaxFloorID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                    int nMinFloorID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                    string strBuildingGroupName = WebDBManager.GetStringField(arrResult[i + 7], "");
                    //string strBuildingGroupDisplayName = WebDBManager.GetStringField(arrResult[i + 8], "");
                    string strGroupNamePos = WebDBManager.GetStringField(arrResult[i + 8], "");
                    string strBroadcastName = WebDBManager.GetStringField(arrResult[i + 9], "");
                    //string strDisplayText = WebDBManager.GetStringField(arrResult[i + 11], "");

                    if (strBroadcastName == null || strBroadcastName.Equals("null"))
                    {
                        strBroadcastName = strBuildingName;
                    }
                    else
                    {
                        int nIdx = strBroadcastName.IndexOf('*');
                        if (nIdx != -1)
                        {
                            strBroadcastName = strBroadcastName.Substring(0, nIdx);
                        }
                    }

                    //if (strBuildingGroupDisplayName == null || strBuildingGroupDisplayName.Equals("null"))
                    //    strBuildingGroupDisplayName = "";

                    Building building = new Building();

                    if (m_dicBuildingGroup.ContainsKey(nBuildingGroupID))
                    {
                        building.BuildingGroup = m_dicBuildingGroup[nBuildingGroupID];
                    }
                    else
                    {
                        BuildingGroup group = new BuildingGroup();
                        group.BuildingGroupName = strBuildingGroupName;
                        //group.DisplayName = strBuildingGroupDisplayName;
                        group.GroupID = nBuildingGroupID;
                        //group.BuildingList.Add(building);

                        string[] xy = strGroupNamePos.Split(',');
                        float x = 0.0f, y = 0.0f;
                        if (xy.Length == 2)
                        {
                            float.TryParse(xy[0], out x);
                            float.TryParse(xy[1], out y);
                        }

                        group.TextCenterX = x;
                        group.TextCenterY = y;
                        m_dicBuildingGroup[nBuildingGroupID] = group;
                        building.BuildingGroup = group;
                    }

                    building.ID = nID;
                    building.BuildingName = strBuildingName;
                    building.MaxFloorIndex = nMaxFloorID;
                    building.MinFloorIndex = nMinFloorID;
                    building.BuildingCode = strBuildingCode;
                    building.BuildingID = strBuildingID;
                    building.BroadcastName = strBroadcastName;
                    building.DisplayText = strBuildingName;// strDisplayText;

                    building.BuildingGroup.BuildingList.Add(building);

                    m_dicBuildings[nID] = building;
                }
                catch (System.Exception ex)
                {
                   
                }
            }
        }

        private Triangulator MakeZonePolygon(string szBoundary)
        {
            if (szBoundary == null || szBoundary == "")
                return null;

            string[] arrTokens = szBoundary.Split('#');
            List<Triangulator> polygons = new List<Triangulator>();

            foreach (string strToken in arrTokens)
            {
                Triangulator polygon = StringToPolygon(strToken.Trim());

                if (polygon == null)
                    break;

                polygons.Add(polygon);
            }

            // Zone의 Polygon은 여러개일 수 있는데, 첫번째 Polygon만 리턴한다.
            if (polygons.Count == 0)
                return null;

            return polygons[0];
        }

        private Triangulator StringToPolygon(string szBoundary)
        {
            Triangulator poly = new Triangulator();
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
                float x = float.Parse(szPosX);
                float y = float.Parse(szPosY);
                

                float pos3DX = ((float)x - dx);
                float pos3DZ = dy + (float)y;

                Vector2 pos = new Vector2(-pos3DX, -pos3DZ);
                poly.AddPoint(pos);
                if (bEnd == true)
                    break;
            } while (start_idx < szBoundary.Length);
            return poly;
        }

        public void LoadZones()
        {
            WebDBManager webDB = ModelManager.Instance.WebDB;

            string strSQL = "select id, ZoneName, BuildingID, FloorIndex, Boundary, DXFFileName, DXFAccessedTime, _3DFileName, _3DAccessedTime, BroadcastName, AddFloor " +//, Azimuth, DisplayText from Zone" +
                            "  from Zone where SiteID = " + m_nSiteID.ToString();

            ArrayList arrResult = webDB.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            DateTime dtDefault = new DateTime();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 10; i += 11)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nBuildingID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFloorIndex = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strBoundary = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strDXFFileName = WebDBManager.GetStringField(arrResult[i + 5], "");
                DateTime dtDXF = WebDBManager.GetDateTimeField(arrResult[i + 6], dtDefault);
                string str3DFileName = WebDBManager.GetStringField(arrResult[i + 7], "");
                DateTime dt3D = WebDBManager.GetDateTimeField(arrResult[i + 8], dtDefault);
                string strBroadcastName = WebDBManager.GetStringField(arrResult[i + 9], "");
                string strAddFloor = WebDBManager.GetStringField(arrResult[i + 10], "0.0");
                float dAzimuth = 0.0f;// WebDBManager.GetFloatField(arrResult[i + 11].ToString(), 0.0f);
                string strDisplayText = strZoneName;// WebDBManager.GetStringField(arrResult[i + 12], "");

                Zone zone = new Zone();

                zone.ID = nID;
                zone.ZoneName = strZoneName;
                zone.FloorIndex = nFloorIndex;
                zone.Azimuth = dAzimuth;

                zone.DXFFilePath = strDXFFileName;

                if (zone.DXFFilePath.Length > 0 && zone.DXFFilePath != "null")
                {
                    //파일경로에서 파일이름을 따옴
                    string tmp = strDXFFileName;
                    string[] buf = tmp.Split('\\');


                    if (tmp != "Blank.png")
                    {
                        if (buf != null && buf.Length > 1)
                            zone.DXFFileName = buf[1];
                        else if (buf != null && buf.Length == 1)
                            zone.DXFFileName = buf[0];
                        else
                        {
                            zone.DXFFileName = "blank.png";
                            zone.DXFFilePath = "blank.png";
                        }
                    }
                    else
                    {
                        zone.DXFFileName = "blank.png";
                        zone.DXFFilePath = "blank.png";
                    }
                }
                else
                {
                    zone.DXFFileName = "blank.png";
                    zone.DXFFilePath = "blank.png";
                }

                if (strBroadcastName == "null" || strBroadcastName == "")
                    zone.BroadcastName = strZoneName;
                else
                    zone.BroadcastName = strBroadcastName;

                if (strDisplayText == "null" || strDisplayText == "")
                    zone.DisplayText = strZoneName;
                else
                    zone.DisplayText = strDisplayText;

                if (m_dicBuildings.ContainsKey(nBuildingID))
                {
                    zone.Building = m_dicBuildings[nBuildingID];
                    zone.Building.FloorList.Add(zone);
                }

                try
                {
                    zone.Polygon = MakeZonePolygon(strBoundary);
                }
                catch (System.Exception)
                {
                    //MessageBox.Show("Make polygo error!!");
                }

                //지하나 .2.5인 층들
                try
                {
                    //strAddFloor가 비었다면 0.0f
                    if (strAddFloor.Length == 0 || strAddFloor == "null")
                        zone.AddFloor = 0.0f;
                    else
                        zone.AddFloor = float.Parse(strAddFloor);
                }
                catch (Exception)
                {
                    zone.AddFloor = 0.0f;
                }

                zone.Floor.FloorIndex = (zone.FloorIndex + zone.AddFloor);

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
                        ArrayList arrZone = new ArrayList();
                        m_dicBuildingZones[zone.Building.ID] = arrZone;
                        arrZone.Add(zone);
                    }
                }
            }
        }

        public ArrayList GetZoneList(int nBuildingID)
        {
            if (DicBuildingZones.ContainsKey(nBuildingID))
                return DicBuildingZones[nBuildingID];

            return null;
        }

        public ArrayList GetZoneList(string buildingID)
        {
            ArrayList arList = new ArrayList();
            foreach (KeyValuePair<int, Zone> kv in m_dicZones)
            {
                Zone obj = kv.Value;
                if (obj.Building != null && buildingID == obj.Building.BuildingID)
                {
                    arList.Add(obj);
                }
            }
            return arList;
        }

        public Zone GetZone(string buildingID, float floorIndex)
        {
            Building building = GetBuilding(buildingID);
            if (building == null)
                return null;

            foreach (Zone zone in building.FloorList)
            {
                if (zone != null && zone.Floor.FloorIndex == floorIndex)
                {
                    //if (addFloor.Equals(zone.AddFloor))
                    //{
                    return zone;
                    //}
                }
            }
            return null;
        }

        public Zone GetZone(string buildingID, int floorIndex)
        {
            foreach (KeyValuePair<int, Zone> kv in m_dicZones)
            {
                Zone obj = kv.Value;
                if (obj.Building != null && buildingID == obj.Building.BuildingID)
                {
                    if (obj.FloorIndex == floorIndex && obj.AddFloor == 0.0f)
                    {
                        return obj;
                    }
                }
            }
            return null;
        }

        public Zone GetZone(int nZoneID)
        {
            if (m_dicZones.ContainsKey(nZoneID))
                return m_dicZones[nZoneID];

            return null;
        }

        public string GetBuildingID(int nID)
        {
            if (!m_dicBuildings.ContainsKey(nID))
                return "";
            Building b = m_dicBuildings[nID];
            return b.BuildingID;
        }

        public Building GetBuilding(string szBuildingID)
        {
            foreach (KeyValuePair<int, Building> kv in m_dicBuildings)
            {
                Building obj = kv.Value;
                if (szBuildingID == obj.BuildingID)
                {
                    return obj;
                }
            }
            return null;
        }

        //public string CheckZoneName(float x, float y)
        //{
        //    Zone zone = GetOutsideZone(x, y);
        //    return zone == null ? "" : zone.DisplayText;
        //}       

        public Zone FindZone(Building building, string strFloor)
        {
            Dictionary<int, Building> dicBuildings = ZoneManager.Instance.DicBuildings;
            foreach (KeyValuePair<int, Building> pair in dicBuildings)
            {
                Building obj = pair.Value;
                if (obj.BuildingID == building.BuildingID)
                {
                    ArrayList arZone = GetZoneList(building.ID);
                    if (arZone == null)
                        return null;
                    foreach (Zone zone in arZone)
                    {
                        if (strFloor == zone.Floor.ToString())
                            return zone;
                    }
                }
            }
            return null;
        }
              
        public EquipmentZone GetEquipZone(int nEquipZoneID)
        {
            if (m_dicEquipZones.ContainsKey(nEquipZoneID))
                return m_dicEquipZones[nEquipZoneID];
            return null;
        }

        public void LoadEquipmentZone()
        {
            WebDBManager webDB = ModelManager.Instance.WebDB;

            string strSQL = "select id, ZoneName, Boundary, LinkedZoneIDList, type, BroadcastName, DisplayText from EquipmentZone where ID > 0" +
                            " and SiteID = " + m_nSiteID.ToString();

            ArrayList arrResult = webDB.GetResultData(strSQL, 0);

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
                string strDisplayText = WebDBManager.GetStringField(arrResult[i + 6], "");

                EquipmentZone zone = new EquipmentZone();

                zone.ID = nID;
                zone.ZoneName = strZoneName;
                zone.ZoneType = (EquipmentZone.EquipZoneType)nType;

                if (strBroadcastName == "null" || strBroadcastName == "")
                    zone.BroadcastName = strZoneName;
                else
                    zone.BroadcastName = strBroadcastName;

                if (strDisplayText == "null" || strDisplayText == "")
                    zone.DisplayText = strZoneName;
                else
                    zone.DisplayText = strDisplayText;

                strLinkedZones = strLinkedZones.Trim();
                string[] szIds = strLinkedZones.Split(',');

                for (int j = 0; j < szIds.Length; j++)
                {
                    string szID = szIds[j];
                    int nZoneID = -1;
                    if (int.TryParse(szID, out nZoneID))
                    {
                        Zone realzone = GetZone(nZoneID);
                        if (zone.LinkedZone == null)
                        {
                            zone.LinkedZone = realzone;
                        }

                        if (realzone != null)
                        {
                            zone.LinkedZoneList.Add(realzone);
                            ArrayList arrEquipZones = null;

                            if (m_dicZoneEquipZones.ContainsKey(realzone))
                            {
                                arrEquipZones = m_dicZoneEquipZones[realzone];
                            }
                            else
                            {
                                arrEquipZones = new ArrayList();
                                m_dicZoneEquipZones[realzone] = arrEquipZones;
                            }

                            if (!arrEquipZones.Contains(zone))
                                arrEquipZones.Add(zone);
                        }
                    }
                }
                if (zone.Building != null)
                {
                    zone.Building.EquipZoneList.Add(zone);
                }

                try
                {
                    zone.Polygon = MakeZonePolygon(strBoundary);
                }
                catch (System.Exception ex)
                {
                    //Debug.WriteLine(ex.Message);
                    //Debug.WriteLine(ex.StackTrace);
                }

                //중복 EquipZone Boundary 점검
                //CheckPolygon(nID, zone.Polygon);

                m_dicEquipZones[nID] = zone;

                if (zone.Building == null)
                    m_dicOutdoorEquipZones[nID] = zone;
            }

            //Zone별 회로번호 얻어오기(Table_1)
            //LoadSensorTagInfo();
        }

        public ArrayList GetEquipmentZoneList(Zone zone)
        {
            if (m_dicZoneEquipZones.ContainsKey(zone))
                return m_dicZoneEquipZones[zone];

            return null;
        }

        public BuildingGroup FindBuildingGroup(string GroupName)
        {
            foreach (KeyValuePair<int, BuildingGroup> kv in m_dicBuildingGroup)
            {
                BuildingGroup group = kv.Value;
                if (group.BuildingGroupName == GroupName)
                    return group;
            }
            return null;
        }

        public Building FindBuilding(string BuildingName)
        {
            foreach (KeyValuePair<int, Building> kv in m_dicBuildings)
            {
                Building building = kv.Value;
                if (building.BuildingName == BuildingName || building.DisplayText == BuildingName)
                    return building;
            }
            return null;
        }

        public ArrayList FindZoneList(string GroupName, string BuildingName, string FloorName)
        {
            ArrayList arrSelectZoneList = new ArrayList();
            ArrayList arrGroupList = new ArrayList();
            ArrayList arrBuildingList = new ArrayList();
            ArrayList arrFloorList = new ArrayList();

            bool bAddOutterZone = false;
            if (GroupName == "모든 건물 그룹")
            {
                arrGroupList.AddRange(m_dicBuildingGroup.Values);
                bAddOutterZone = true;
            }
            else
            {
                if (GroupName == "외부 영역")
                {
                    bAddOutterZone = true;
                }
                else
                {
                    BuildingGroup group = FindBuildingGroup(GroupName);
                    if (group != null)
                        arrGroupList.Add(group);
                }
            }

            foreach (BuildingGroup group in arrGroupList)
            {
                if (BuildingName == "모든 건물")
                {
                    arrBuildingList.AddRange(group.BuildingList);
                }
                else
                {
                    Building building = FindBuilding(BuildingName);
                    if (building != null)
                        arrBuildingList.Add(building);
                }
            }

            foreach (Building building in arrBuildingList)
            {
                if (FloorName == "모든 층")
                {
                    arrFloorList.AddRange(building.FloorList);
                }
                else
                {
                    foreach (Zone floor in building.FloorList)
                    {
                        if (floor.Floor.ToString() == FloorName)
                        {
                            arrFloorList.Add(floor);
                            break;
                        }
                    }
                }
            }

            if (bAddOutterZone == true)
            {
                if (BuildingName == "모든 건물")
                {
                    arrFloorList.AddRange(m_dicOutdoorZones.Values);
                }
                else
                {
                    foreach (KeyValuePair<int, Zone> kv in m_dicOutdoorZones)
                    {
                        Zone zone = kv.Value;
                        if (zone.ZoneName == BuildingName)
                        {
                            arrFloorList.Add(zone);
                            break;
                        }
                    }
                }
            }

            return arrFloorList;
        }

        public void Load3DText()
        {
            WebDBManager webDB = ModelManager.Instance.WebDB;

            string strSQL = "select ID, Name, DisplayText, TextColor, TextFontHeight, TextCenter from _3DText where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = webDB.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strDisplaytext = WebDBManager.GetStringField(arrResult[i + 2], "");
                int nTextColor = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                float fFontHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), -1.0f);
                string strTextCenter = WebDBManager.GetStringField(arrResult[i + 5], "");

                if (nID < 0)
                    continue;

                if (strDisplaytext == null || strDisplaytext.Equals("null"))
                    strDisplaytext = "";

                string[] xy = strTextCenter.Split(',');
                float x = 0.0f, y = 0.0f;
                if (xy.Length == 2)
                {
                    float.TryParse(xy[0], out x);
                    float.TryParse(xy[1], out y);
                }

                _3DText text = new _3DText();

                text.ID = nID;
                text.Name = strName;
                text.DisplayText = strDisplaytext;

                //if (nTextColor > 0)
                //    text.TextColor = new VariousData<System.Drawing.Color>(System.Drawing.Color.FromArgb(nTextColor));

                //if (fFontHeight > 0.0f)
                //    text.TextFontHeight = new VariousData<float>(fFontHeight);

                text.TextCenterX = x;
                text.TextCenterY = y;

                m_3DTextList.Add(text);
            }
        }
    }
}

