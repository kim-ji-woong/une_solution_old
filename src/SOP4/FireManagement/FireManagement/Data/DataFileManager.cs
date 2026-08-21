using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.IO;

// PC에서 Tablet에서 데이터를 옮기기 위한 용도
// 또는 Tablet에서 옮겨진 데이터를 열어서 사용하기 위한 용도로 사용

namespace FireManagement
{
    public class DataFileManager
    {
        private string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });
        private string m_strErrorMessage = "";

        // FMF에 있는 빌딩 ID는 유효한 값이 아닐수 있으므로, FMF에 있는 ID와 실제 Building 정보를 기억해둔다.
        private Dictionary<int, Building> m_dicFMFBuildings = new Dictionary<int, Building>();
        // FMF에 있는 Zone ID는 유효한 값이 아닐수 있으므로, FMF에 있는 ID와 실제 Zone 정보를 기억해둔다.
        private Dictionary<int, Zone> m_dicFMFZones = new Dictionary<int, Zone>();
        private FMFHeader m_header = null;
        // 시스템 로딩시 처음 읽는 상황인가?
        private bool m_isFirstRead = false;

        public FMFHeader Header
        {
            get { return m_header; }
        }

        // 시스템 로딩시 처음 읽는 상황인가?
        public bool FirstRead
        {
            get { return m_isFirstRead; }
            set { m_isFirstRead = value; }
        }

        public bool ExportData(string strPath)
        {
            IOManager ioMgr = FormMain2.Instance.IOManager;

            // 현재 화면에 나타난 Zone에서 변경된 것이 있는지 검사한다.
            if (FormMain2.Instance.CurrentZone != null)
                ioMgr.CompareZoneEquipmentsToDB(FormMain2.Instance.CurrentZone);

            if (FormMain2.Instance.IsPCMode)
            {
                return Export(strPath, true);
            }
            else
            {
                // 프로그램이 켜져있는 동안에도 수정된 설비 정보가 전에 데이터에 적용되어 있어야 하므로, 메모리 상의 데이터들도 갱신해준다.
                DXFManager dxfMgr = FormMain2.Instance.DXFManager;
                ioMgr.ApplyEquipments(dxfMgr.Equipments, FormMain2.Instance.CurrentZone);
                ioMgr.ApplyEquipmentHistory(dxfMgr.EquipmentHistory);

                // PC로 전달할 버전은 수정된 설비 데이터만 만들면 된다.
                if (!Export(strPath, false))
                    return false;

                // 수정된 설비 데이터가 태블릿에 저장되어야 하므로 파일을 수정한다.
                string strTableFilePath = System.Windows.Forms.Application.StartupPath + "\\" + ioMgr.TabletDataFile;
                if (!Export(strTableFilePath, true))
                    return false;
            }

            return true;
        }

        private bool Export(string strPath, bool isPCMode)
        {
            MemoryStream stream = MakeXML(isPCMode, strPath);
            if (stream == null)
                return false;

            //MakeFMF(stream, strPath);
            return true;
        }

        public bool ImportData(string strPath, ref bool isPCMode)
        {
            if (!File.Exists(strPath))
                return false;

            Stream stream = ReadFMF(strPath);
            if (stream == null)
                return false;

            if (!ReadXML(stream, ref isPCMode))
            {
                stream.Close();
                return false;
            }

            stream.Close();
            return true;
        }

        private bool ReadXML(Stream stream, ref bool isPCMode)
        {
            m_dicFMFBuildings.Clear();
            m_dicFMFZones.Clear();

            XmlReader reader = XmlReader.Create(stream);

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "FireManagement", true) == 0)
                            {
                                if (!ReadXML(reader, ref isPCMode))
                                    return false;
                            }
                            else
                            {
                                m_strErrorMessage = "잘못된 FMF 파일입니다.";
                                reader.Close();
                                return false;
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                reader.Close();
                return false;
            }

            return true;
        }

        private bool ReadXML(XmlReader reader, ref bool isPCMode)
        {
            bool stop = false;

            try
            {
                if (reader.HasAttributes)
                {
                    while (reader.MoveToNextAttribute())
                    {
                        if (string.Compare(reader.Name, "madeBy", true) == 0)
                        {
                            isPCMode = string.Compare(reader.Value, "PC", true) == 0;

                            // Tablet 모드에서는 PC 모드에서 만든 파일만 사용해야 한다.
                            if (!FormMain2.Instance.IsPCMode && !isPCMode)
                                return false;
                        }
                    }

                    reader.MoveToElement();
                }

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Header", true) == 0)
                            {
                                if (!ReadHeader(reader))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "BuildingGroupList", true) == 0)
                            {
                                if (!ReadBuildingGroupList(reader))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "BuildingList", true) == 0)
                            {
                                if (!ReadBuildingList(reader))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "ZoneList", true) == 0)
                            {
                                if (!ReadZoneList(reader))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "FireEquipmentList", true) == 0)
                            {
                                if (!ReadFireEquipmentList(reader))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "FireEquipmentHistoryList", true) == 0)
                            {
                                if (!ReadFireEquipmentHistoryList(reader))
                                    return false;
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                reader.Close();
                return false;
            }

            return true;
        }

        private void PassElement(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        return;
                }
            }
        }

        private bool ReadFireEquipmentHistoryList(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return true;

            bool stop = false;
            Dictionary<int, FireEquipment> dicEquipments = new Dictionary<int, FireEquipment>();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "FireEquipmentHistory", true) == 0)
                        {
                            if (!ReadFireEquipmentHistory(reader, dicEquipments))
                                return false;
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return true;
        }

        private bool ReadFireEquipmentHistory(XmlReader reader, Dictionary<int, FireEquipment> dicEquipments)
        {
            if (reader.IsEmptyElement)
                return false;

            bool stop = false;
            FireEquipmentHistory history = new FireEquipmentHistory();
            history.IsNewHistory = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "ID", true) == 0)
                        {
                            int nID = -1;
                            if (!ReadInt(reader, ref nID))
                                return false;

                            history.ID = nID;
                        }
                        else if (string.Compare(reader.Name, "FireEquipmentID") == 0)
                        {
                            int nEquipID = -1;
                            if (!ReadInt(reader, ref nEquipID))
                                return false;

                            /*FireEquipment equip = null;

                            if (dicEquipments.ContainsKey(nEquipID))
                                equip = dicEquipments[nEquipID];
                            else
                            {
                                equip = FormMain2.Instance.DXFManager.FindEquipment(nEquipID);
                                if (equip == null)
                                {
                                    m_strErrorMessage = string.Format("확인할 수 없는 설비ID [{0}]가 존재합니다.", nEquipID);
                                    return false;
                                }

                                dicEquipments[nEquipID] = equip;
                            }*/

                            history.EquipmentID = nEquipID;
                        }
                        else if (string.Compare(reader.Name, "Time") == 0)
                        {
                            DateTime time = new DateTime();
                            if (!ReadDateTime(reader, ref time))
                                return false;

                            history.Time = time;
                        }
                        else if (string.Compare(reader.Name, "Status") == 0)
                        {
                            int nStatus = 0;
                            if (!ReadInt(reader, ref nStatus))
                                return false;

                            history.Status = (FireEquipmentHistory.EquipmentStatus)nStatus;
                        }
                        else if (string.Compare(reader.Name, "CheckersOpinion") == 0)
                        {
                            string strOpinion = "";
                            if (!ReadElementText(reader, ref strOpinion))
                                return false;

                            history.CheckersOpinion = strOpinion;
                        }
                        else if (string.Compare(reader.Name, "Description", true) == 0)
                        {
                            string strDescription = "";
                            if (!ReadElementText(reader, ref strDescription))
                                return false;

                            history.Description = strDescription;
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            Dictionary<int, ArrayList> dicEquipmentHistory = FormMain2.Instance.IOManager.EquipmentHistory;
            AddEquipmentHistory(dicEquipmentHistory, history);

            if (m_isFirstRead)
            {
                Dictionary<int, ArrayList> dicDBEquipmentHistory = FormMain2.Instance.IOManager.DBEquipmentHistory;
                AddEquipmentHistory(dicDBEquipmentHistory, new FireEquipmentHistory(history));
            }

            return true;
        }

        private void AddEquipmentHistory(Dictionary<int, ArrayList> dicEquipmentHistory, FireEquipmentHistory history)
        {
            if (dicEquipmentHistory.ContainsKey(history.EquipmentID))
            {
                ArrayList arrHistory = dicEquipmentHistory[history.EquipmentID];
                if (!ContainsHistory(arrHistory, history))
                    arrHistory.Add(history);
            }
            else
            {
                ArrayList arrHistory = new ArrayList();
                arrHistory.Add(history);
                dicEquipmentHistory[history.EquipmentID] = arrHistory;
            }
        }

        private bool ContainsHistory(ArrayList arrHistory, FireEquipmentHistory history)
        {
            foreach (FireEquipmentHistory equipHistory in arrHistory)
            {
                if (equipHistory.Time == history.Time)
                    return true;
            }

            return false;
        }

        private bool ReadFireEquipmentList(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return true;

            bool stop = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "FireEquipment", true) == 0)
                        {
                            if (!ReadFireEquipment(reader))
                                return false;
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return true;
        }

        private bool ReadFireEquipment(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return false;

            bool stop = false;
            FireEquipment equip = new FireEquipment();

            // FMF는 미터 단위이며 DXF는 mm 단위를 사용한다.
            float fFlag = 1 / FormMain2.Instance.GetUnitFlag(UnitOfLength.METER); ;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "ID", true) == 0)
                        {
                            int nID = -1;
                            if (!ReadInt(reader, ref nID))
                                return false;

                            equip.ID = nID;
                        }
                        else if (string.Compare(reader.Name, "RFIDTag") == 0)
                        {
                            string strRFID = "";
                            if (!ReadElementText(reader, ref strRFID))
                                return false;

                            equip.RFIDTag = strRFID;
                        }
                        else if (string.Compare(reader.Name, "EquipID") == 0)
                        {
                            string strEquipID = "";
                            if (!ReadElementText(reader, ref strEquipID))
                                return false;

                            equip.EquipID = strEquipID;
                        }
                        else if (string.Compare(reader.Name, "RFIDTagID") == 0)
                        {
                            string strRFIDTagID = "";
                            if (!ReadElementText(reader, ref strRFIDTagID))
                                return false;

                            equip.RFIDTagID = strRFIDTagID;
                        }
                        else if (string.Compare(reader.Name, "DxfObjID") == 0)
                        {
                            string strDxfObjID = "";
                            if (!ReadElementText(reader, ref strDxfObjID))
                                return false;

                            equip.DXFObjID = strDxfObjID;
                        }
                        else if (string.Compare(reader.Name, "EquipType", true) == 0)
                        {
                            int nType = -1;
                            if (!ReadInt(reader, ref nType))
                                return false;

                            equip.Type = (FireEquipment.EquipmentType)nType;
                        }
                        else if (string.Compare(reader.Name, "ZoneID", true) == 0)
                        {
                            int nZoneID = -1;
                            if (!ReadInt(reader, ref nZoneID))
                                return false;

                            Zone zone = null;
                            if (m_dicFMFZones.ContainsKey(nZoneID))
                                zone = m_dicFMFZones[nZoneID];

                            //Zone zone = FormMain2.Instance.IOManager.FindZone(nZoneID);
                            if (zone == null)
                            {
                                m_strErrorMessage = string.Format("확인할 수 없는 ZoneID [{0}]가 존재합니다.", nZoneID);
                                return false;
                            }

                            equip.Zone = zone;
                        }
                        else if (string.Compare(reader.Name, "X", true) == 0)
                        {
                            float x = 0.0f;
                            if (!ReadFloat(reader, ref x))
                                return false;

                            equip.Position = new System.Drawing.PointF(x * fFlag, equip.Position.Y);
                        }
                        else if (string.Compare(reader.Name, "Y", true) == 0)
                        {
                            float y = 0.0f;
                            if (!ReadFloat(reader, ref y))
                                return false;

                            equip.Position = new System.Drawing.PointF(equip.Position.X, y * fFlag);
                        }
                        else if (string.Compare(reader.Name, "Description") == 0)
                        {
                            string strDescription = "";
                            if (!ReadElementText(reader, ref strDescription))
                                return false;

                            equip.Description = strDescription;
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            IOManager ioMgr = FormMain2.Instance.IOManager;
            FireEquipment oldEquip = null;

            if (equip.Zone != null)
            {
                // DXF ID 중복검사 무시
                //oldEquip = ioMgr.FindEquipment(equip.DXFObjID, equip.Zone);

                if (oldEquip != null)
                {
                    int nID = oldEquip.ID;
                    oldEquip.FromCopy(equip);
                    oldEquip.ID = nID;
                }
                else
                {
                    oldEquip = equip;
                    FormMain2.Instance.IOManager.AddEquipment(equip, equip.Zone);

                    if (m_isFirstRead)
                        FormMain2.Instance.IOManager.AddDBEquipment(new FireEquipment(equip), equip.Zone);
                }
            }

            /*ArrayList arrEquipments = FormMain2.Instance.DXFManager.Equipments;

            if (!arrEquipments.Contains(oldEquip))
                arrEquipments.Add(oldEquip);*/

            return true;
        }

        private bool ReadZoneList(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return true;

            bool stop = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Zone", true) == 0)
                        {
                            if (!ReadZone(reader))
                                return false;
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            Dictionary<int, ArrayList> buildingZones = FormMain2.Instance.IOManager.BuildingZones;

            // 층별로 정렬
            foreach (KeyValuePair<int, ArrayList> pair in buildingZones)
            {
                pair.Value.Sort();
            }

            return true;
        }

        private bool ReadZone(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return false;

            bool stop = false;
            Zone zone = new Zone();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "ID", true) == 0)
                        {
                            int nID = -1;
                            if (!ReadInt(reader, ref nID))
                                return false;

                            zone.ID = nID;
                        }
                        else if (string.Compare(reader.Name, "ZoneName") == 0)
                        {
                            string strZoneName = "";
                            if (!ReadElementText(reader, ref strZoneName))
                                return false;

                            zone.ZoneName = strZoneName;
                        }
                        else if (string.Compare(reader.Name, "BuildingID") == 0)
                        {
                            int nBuildingID = -1;
                            if (!ReadInt(reader, ref nBuildingID))
                                return false;

                            if (nBuildingID > 0)
                            {
                                Building building = null;

                                if (m_dicFMFBuildings.ContainsKey(nBuildingID))
                                    building = m_dicFMFBuildings[nBuildingID];

                                //Building building = FormMain2.Instance.IOManager.FindBuilding(nBuildingID);
                                if (building == null)
                                {
                                    m_strErrorMessage = string.Format("확인할 수 없는 BuildingID [{0}]가 존재합니다.", nBuildingID);
                                    return false;
                                }

                                zone.Building = building;
                            }
                        }
                        else if (string.Compare(reader.Name, "FloorIndex") == 0)
                        {
                            int nFloorIndex = 0;
                            if (!ReadInt(reader, ref nFloorIndex))
                                return false;

                            zone.FloorIndex = nFloorIndex;
                        }
                        else if (string.Compare(reader.Name, "AddFloor") == 0)
                        {
                            float fAddFloor = 0.0f;
                            if (!ReadFloat(reader, ref fAddFloor))
                                return false;

                            zone.AddFloor = fAddFloor;
                        }
                        else if (string.Compare(reader.Name, "DXFName") == 0)
                        {
                            string strDXFName = "";
                            if (!ReadElementText(reader, ref strDXFName))
                                return false;

                            //zone.DXFFilePath = strDXFName;

                            int nLen = strDXFName.Length;

                            if (nLen > 0 && strDXFName.ElementAt(nLen - 1) != '\\')
                            {
                                zone.DXFFilePath = System.Windows.Forms.Application.StartupPath + "\\" + FormMain2.Instance.IndoorFolderPath + "\\" + strDXFName;
                                //zone.DXFFilePath = System.Windows.Forms.Application.StartupPath + "\\FEData\\DXF\\" + strDXFName;
                            }
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            Zone zoneOld = null;
            Dictionary<int, Zone> dicZones = FormMain2.Instance.IOManager.AllZones;
            Dictionary<int, ArrayList> buildingZones = FormMain2.Instance.IOManager.BuildingZones;

            int nFMFID = zone.ID;

            if (FormMain2.Instance.IsPCMode)
            {
                if (zone.Building != null)
                    zone.ID = FormMain2.Instance.IOManager.GetZoneID(zone.Building, zone.FloorIndex, zone.DXFFilePath);
                else
                {
                    int nID = -1;
                    if (FormMain2.Instance.IOManager.GetOutdoorZoneID(zone.ZoneName, ref nID))
                        zone.ID = nID;
                }
            }

            if (zone.ID < 0)
                return false;

            if (dicZones.ContainsKey(zone.ID))
            {
                zoneOld = dicZones[zone.ID];
                zoneOld.CopyFrom(zone);
            }
            else
            {
                zoneOld = zone;
                dicZones[zone.ID] = zoneOld;
            }

            if (zone.Building != null)
            {
                if (buildingZones.ContainsKey(zone.Building.ID))
                {
                    ArrayList arrZones = buildingZones[zone.Building.ID];

                    if (!FindZone(arrZones, zone))
                        arrZones.Add(zone);
                }
                else
                {
                    ArrayList arrZones = new ArrayList();
                    buildingZones[zone.Building.ID] = arrZones;
                    arrZones.Add(zone);
                }
            }

            m_dicFMFZones[nFMFID] = zoneOld;

            if (FormMain2.Instance.IsPCMode)
                FormMain2.Instance.CurrentZone = zoneOld;

            // 파일에서 zoneOld에 해당하는 설비 정보들을 새로 읽을 예정이므로
            // 기존 설비 정보는 삭제한다.
            ArrayList arrEquipments = FormMain2.Instance.IOManager.GetEquipments(zoneOld);
            arrEquipments.Clear();

            FormMain2.Instance.IOManager.ClearZoneEquipmentHistoryList(zoneOld);

            if (FormMain2.Instance.IsPCMode)
            {
                // FMF에서 읽은 Zone들은 기본적으로 DB와 다르다고 간주한다.
                FormMain2.Instance.IOManager.AddChangedZone(zoneOld);
            }

            return true;
        }

        private bool FindZone(ArrayList arrZones, Zone zone)
        {
            foreach (Zone zoneData in arrZones)
            {
                if (zone.ID > 0 && zoneData.ID == zone.ID)
                {
                    return true;
                }
                else
                {
                    if (zoneData.Building == zone.Building &&
                        zoneData.FloorIndex == zone.FloorIndex &&
                        zoneData.AddFloor == zone.AddFloor)
                        return true;
                }
            }

            return false;
        }

        private bool ReadBuildingList(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return true;

            bool stop = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Building", true) == 0)
                        {
                            if (!ReadBuilding(reader))
                                return false;
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return true;
        }

        private bool ReadBuilding(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return false;

            bool stop = false;
            Building building = new Building();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "ID", true) == 0)
                        {
                            int nID = -1;
                            if (!ReadInt(reader, ref nID))
                                return false;

                            building.ID = nID;
                        }
                        else if (string.Compare(reader.Name, "BuildingID") == 0)
                        {
                            string strBuildingID = "";
                            if (!ReadElementText(reader, ref strBuildingID))
                                return false;

                            building.BuildingID = strBuildingID;
                        }
                        else if (string.Compare(reader.Name, "BuildingCode") == 0)
                        {
                            string strBuildingCode = "";
                            if (!ReadElementText(reader, ref strBuildingCode))
                                return false;

                            building.BuildingCode = strBuildingCode;
                        }
                        else if (string.Compare(reader.Name, "BuildingName") == 0)
                        {
                            string strBuildingName = "";
                            if (!ReadElementText(reader, ref strBuildingName))
                                return false;

                            building.BuildingName = strBuildingName;
                        }
                        else if (string.Compare(reader.Name, "BuildingGroupID") == 0)
                        {
                            int nBuildingGroupID = -1;
                            if (!ReadInt(reader, ref nBuildingGroupID))
                                return false;

                            if (nBuildingGroupID > 0)
                            {
                                BuildingGroup buildingGroup = FormMain2.Instance.IOManager.FindBuildingGroup(nBuildingGroupID);
                                if (buildingGroup == null)
                                {
                                    m_strErrorMessage = string.Format("확인할 수 없는 BuildingGroupID [{0}]가 존재합니다.", nBuildingGroupID);
                                    return false;
                                }

                                building.BuildingGroup = buildingGroup;
                            }
                        }
                        else if (string.Compare(reader.Name, "MaxFloor") == 0)
                        {
                            int nMaxFloorIndex = -1;
                            if (!ReadInt(reader, ref nMaxFloorIndex))
                                return false;

                            building.MaxFloorIndex = nMaxFloorIndex;
                        }
                        else if (string.Compare(reader.Name, "MinFloor") == 0)
                        {
                            int nMinFloorIndex = -1;
                            if (!ReadInt(reader, ref nMinFloorIndex))
                                return false;

                            building.MinFloorIndex = nMinFloorIndex;
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            IOManager ioMgr = FormMain2.Instance.IOManager;
            Dictionary<int, Building> dicBuildings = ioMgr.AllBuildings;

            if (building.ID < 0)
                return false;

            int nFMFID = building.ID;

            if (FormMain2.Instance.IsPCMode)
                building.ID = ioMgr.GetBuildingID(building.BuildingID);

            if (building.ID < 0)
                return false;

            Building oldBuilding = null;

            if (dicBuildings.ContainsKey(building.ID))
            {
                oldBuilding = dicBuildings[building.ID];
                oldBuilding.CopyFrom(building);
            }
            else
            {
                oldBuilding = building;
                dicBuildings[building.ID] = oldBuilding;
            }

            m_dicFMFBuildings[nFMFID] = oldBuilding;

            if (oldBuilding.BuildingGroup != null)
            {
                if (ioMgr.AllBuildingGroups.ContainsKey(oldBuilding.BuildingGroup))
                {
                    ArrayList arrBuildings = ioMgr.AllBuildingGroups[oldBuilding.BuildingGroup];
                    if (!arrBuildings.Contains(oldBuilding))
                        arrBuildings.Add(oldBuilding);
                }
            }

            return true;
        }

        private bool ReadHeader(XmlReader reader)
        {
            m_header = new FMFHeader();

            if (reader.IsEmptyElement)
                return true;

            bool stop = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Version", true) == 0)
                        {
                            string strVersion = "";
                            if (!ReadElementText(reader, ref strVersion))
                                return false;

                            m_header.Version = strVersion;
                        }
                        else if (string.Compare(reader.Name, "Date", true) == 0)
                        {
                            DateTime dtTime = new DateTime();
                            if (!ReadDateTime(reader, ref dtTime))
                                return false;

                            m_header.Time = dtTime;
                        }
                        else if (string.Compare(reader.Name, "Writer", true) == 0)
                        {
                            string strWriter = "";
                            if (!ReadElementText(reader, ref strWriter))
                                return false;

                            m_header.Writer = strWriter;
                        }
                        else if (string.Compare(reader.Name, "Description", true) == 0)
                        {
                            string strDescription = "";
                            if (!ReadElementText(reader, ref strDescription))
                                return false;

                            m_header.Description = strDescription;
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return true;
        }

        private bool ReadBuildingGroupList(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return true;

            bool stop = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "BuildingGroup", true) == 0)
                        {
                            if (!ReadBuildingGroup(reader))
                                return false;
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return true;
        }

        private bool ReadElementText(XmlReader reader, ref string strText)
        {
            if (reader.IsEmptyElement)
                return true;

            bool stop = false;
            strText = "";

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        strText = reader.Value;
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return true;
        }

        private bool ReadInt(XmlReader reader, ref int nData)
        {
            if (reader.IsEmptyElement)
                return false;

            string strText = "";
            if (!ReadElementText(reader, ref strText))
                return false;

            try
            {
                nData = int.Parse(strText);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool ReadFloat(XmlReader reader, ref float fData)
        {
            if (reader.IsEmptyElement)
                return false;

            string strText = "";
            if (!ReadElementText(reader, ref strText))
                return false;

            try
            {
                fData = float.Parse(strText);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool ReadDateTime(XmlReader reader, ref DateTime time)
        {
            if (reader.IsEmptyElement)
                return false;

            string strText = "";
            if (!ReadElementText(reader, ref strText))
                return false;

            try
            {
                time = Convert.ToDateTime(strText);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool ReadBuildingGroup(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return false;

            bool stop = false;
            BuildingGroup buildingGroup = new BuildingGroup();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "ID", true) == 0)
                        {
                            int nID = -1;
                            if (!ReadInt(reader, ref nID))
                                return false;

                            buildingGroup.ID = nID;
                        }
                        else if (string.Compare(reader.Name, "GroupName") == 0)
                        {
                            string strGroupName = "";
                            if (!ReadElementText(reader, ref strGroupName))
                                return false;

                            buildingGroup.BuildingGroupName = strGroupName;
                        }
                        else
                            PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            IOManager ioMgr = FormMain2.Instance.IOManager;
            Dictionary<BuildingGroup, ArrayList> dicBuildingGroups = FormMain2.Instance.IOManager.AllBuildingGroups;

            BuildingGroup oldBuildingGroup = ioMgr.FindBuildingGroup(buildingGroup.ID);

            if (oldBuildingGroup == null)
            {
                oldBuildingGroup = buildingGroup;
                dicBuildingGroups[oldBuildingGroup] = new ArrayList();
            }
            else
            {
                oldBuildingGroup.CopyFrom(buildingGroup);
            }

            return true;
        }

        private Stream ReadFMF(string strPath)
        {
            FileStream stream = new FileStream(strPath, FileMode.Open, FileAccess.Read);
            return stream;
            /*StreamReader reader = new StreamReader(strPath);
            string strEncrypted = reader.ReadToEnd();
            reader.Close();

            AES256Cipher cipher = new AES256Cipher();
            string strDecryptedXML = cipher.AES_decrypt(strEncrypted, key);

            int nIndex = strDecryptedXML.IndexOf('<');
            if (nIndex < 0)
                return null;

            Stream stream = new MemoryStream(UTF8Encoding.Default.GetBytes(strDecryptedXML));
            return stream;*/
        }

        // FireEquipment Management Format
        private void MakeFMF(MemoryStream stream, string strPath)
        {
            AES256Cipher cipher = new AES256Cipher();

            string strXML = Encoding.UTF8.GetString(stream.ToArray());
            string strEncryptedXML = cipher.AES_encrypt(strXML, key);

            StreamWriter writer = new StreamWriter(strPath, false, Encoding.UTF8);
            writer.Write(strEncryptedXML);
            writer.Close();
        }

        private MemoryStream MakeXML(bool isPCMode, string strPath)
        {
            XmlTextWriter writer = InitWriter(strPath);

            writer.WriteStartElement("FireManagement");

            if (isPCMode)
            {
                writer.WriteStartAttribute("madeBy");
                writer.WriteString("PC");
                writer.WriteEndAttribute();

                if (!MakeHeader(writer))
                {
                    writer.Close();
                    return null;
                }

                if (!MakeBuildingGroupListPC(writer))
                {
                    writer.Close();
                    return null;
                }

                if (!MakeBuildingListPC(writer))
                {
                    writer.Close();
                    return null;
                }

                if (!MakeZoneListPC(writer))
                {
                    writer.Close();
                    return null;
                }

                // PC 버전은 모든 DB 데이터를 저장한다.
                if (!MakeEquipmentListPC(writer))
                {
                    writer.Close();
                    return null;
                }

                // PC 버전은 모든 DB 데이터를 저장한다.
                if (!MakeEquipmentHistoryListPC(writer))
                {
                    writer.Close();
                    return null;
                }
            }
            else
            {
                writer.WriteStartAttribute("madeBy");
                writer.WriteString("Tablet");
                writer.WriteEndAttribute();

                if (!MakeHeader(writer))
                {
                    writer.Close();
                    return null;
                }

                if (!MakeBuildingGroupListTablet(writer))
                {
                    writer.Close();
                    return null;
                }

                if (!MakeBuildingListTablet(writer))
                {
                    writer.Close();
                    return null;
                }

                if (!MakeZoneListTablet(writer))
                {
                    writer.Close();
                    return null;
                }

                // 현재 작업중인 DXF 도면창이 아니라 변경된 모든 Zone들의 설비 정보를 저장한다.
                if (!MakeEquipmentListTablet(writer))
                {
                    writer.Close();
                    return null;
                }

                // 현재 작업중인 DXF 도면창이 아니라 변경된 모든 Zone들의 설비 정보를 저장한다.
                if (!MakeEquipmentHistoryListTablet(writer))
                {
                    writer.Close();
                    return null;
                }
            }

            writer.WriteEndDocument();
            writer.Close();

            return new MemoryStream();
        }

        /*private MemoryStream MakeXML(bool isPCMode)
        {
            MemoryStream stream = new MemoryStream();
            XmlWriter writer = InitWriter(stream);

            writer.WriteStartElement("FireManagement");

            //writer.WriteStartAttribute("xmlns:xsi");
            //writer.WriteString("http://www.w3.org/2001/XMLSchema-instance");
            //writer.WriteEndAttribute();

            if (isPCMode)
            {
                if (!MakeBuildingGroupList(writer))
                {
                    writer.Close();
                    return null;
                }

                if (!MakeBuildingList(writer))
                {
                    writer.Close();
                    return null;
                }

                if (!MakeZoneList(writer))
                {
                    writer.Close();
                    return null;
                }

                // PC 버전은 모든 DB 데이터를 저장한다.
                if (!MakeEquipmentListPC(writer))
                {
                    writer.Close();
                    return null;
                }

                // PC 버전은 모든 DB 데이터를 저장한다.
                if (!MakeEquipmentHistoryListPC(writer))
                {
                    writer.Close();
                    return null;
                }
            }
            else
            {
                // Tablet 버전은 현재 작업중인 DXF 도면창의 데이터만 저장한다.
                if (!MakeEquipmentListTablet(writer))
                {
                    writer.Close();
                    return null;
                }

                // Tablet 버전은 현재 작업중인 DXF 도면창의 데이터만 저장한다.
                if (!MakeEquipmentHistoryListTablet(writer))
                {
                    writer.Close();
                    return null;
                }
            }

            writer.WriteEndDocument();
            writer.Close();

            return stream;
        }*/

        private bool MakeHeader(XmlWriter writer)
        {
            writer.WriteStartElement("Header");

            writer.WriteStartElement("Version");
            writer.WriteString("V1.0");
            writer.WriteEndElement();

            DateTime dtNow = DateTime.Now;
            string strDateTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            writer.WriteStartElement("Date");
            writer.WriteString(strDateTime);
            writer.WriteEndElement();

            writer.WriteStartElement("Writer");
            writer.WriteEndElement();

            writer.WriteStartElement("Description");
            writer.WriteEndElement();

            writer.WriteEndElement();
            return true;
        }

        // PC 버전은 모든 DB 데이터를 저장한다.
        private bool MakeEquipmentHistoryListPC(XmlWriter writer)
        {
            writer.WriteStartElement("FireEquipmentHistoryList");

            Dictionary<int, ArrayList> dicEquipHistory = FormMain2.Instance.IOManager.EquipmentHistory;

            foreach (KeyValuePair<int, ArrayList> pair in dicEquipHistory)
            {
                ArrayList arrHistory = pair.Value;

                foreach (FireEquipmentHistory history in arrHistory)
                {
                    if (!MakeEquipmentHistory(writer, history))
                        return false;
                }
            }

            writer.WriteEndElement();
            return true;
        }

        // 현재 작업중인 DXF 도면창이 아니라 변경된 모든 Zone들의 설비 정보를 저장한다.
        private bool MakeEquipmentHistoryListTablet(XmlWriter writer)
        {
            writer.WriteStartElement("FireEquipmentHistoryList");

            DXFManager dxfMgr = FormMain2.Instance.DXFManager;
            IOManager ioMgr = FormMain2.Instance.IOManager;

            ArrayList arrChangedZones = ioMgr.ChangedZones;

            foreach (Zone zone in arrChangedZones)
            {
                ArrayList arrEquipments = ioMgr.GetEquipments(zone);

                foreach (FireEquipment equip in arrEquipments)
                {
                    ArrayList arrEquipHistory = ioMgr.FindEquipmentHistoryList(equip.ID);
                    if (arrEquipHistory == null)
                        continue;

                    foreach (FireEquipmentHistory history in arrEquipHistory)
                    {
                        if (!MakeEquipmentHistory(writer, history))
                            return false;
                    }
                }
            }

            /*foreach (FireEquipment equip in dxfMgr.Equipments)
            {
                ArrayList arrEquipHistory = ioMgr.FindEquipmentHistoryList(equip.ID);
                if (arrEquipHistory == null)
                    continue;

                foreach (FireEquipmentHistory history in arrEquipHistory)
                {
                    if (!MakeEquipmentHistory(writer, history))
                        return false;
                }
            }*/

            writer.WriteEndElement();
            return true;
        }

        private bool MakeEquipmentHistory(XmlWriter writer, FireEquipmentHistory history)
        {
            writer.WriteStartElement("FireEquipmentHistory");

            writer.WriteStartElement("ID");
            writer.WriteString(history.ID.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("FireEquipmentID");
            writer.WriteString(history.EquipmentID.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Time");
            writer.WriteString(string.Format("{0} {1}:{2}:{3}", history.Time.ToShortDateString(), history.Time.Hour, history.Time.Minute, history.Time.Second));
            writer.WriteEndElement();

            writer.WriteStartElement("Status");
            writer.WriteString(((int)history.Status).ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("CheckersOpinion");
            writer.WriteString(history.CheckersOpinion);
            writer.WriteEndElement();

            writer.WriteStartElement("Description");
            writer.WriteString(history.Description);
            writer.WriteEndElement();

            writer.WriteEndElement();

            // 시스템에 저장된 History로 인식
            history.IsNewHistory = false;
            return true;
        }

        // PC 버전은 모든 DB 데이터를 저장한다.
        private bool MakeEquipmentListPC(XmlWriter writer)
        {
            writer.WriteStartElement("FireEquipmentList");

            IOManager ioMgr = FormMain2.Instance.IOManager;
            Dictionary<int, Zone> dicZones = ioMgr.AllZones;

            foreach (KeyValuePair<int, Zone> pair in dicZones)
            {
                ArrayList arrEquipments = ioMgr.GetEquipments(pair.Value);
                if (arrEquipments == null)
                    continue;

                foreach (FireEquipment equip in arrEquipments)
                {
                    if (!MakeEquipment(writer, equip))
                        return false;
                }
            }

            writer.WriteEndElement();
            return true;
        }

        // 현재 작업중인 DXF 도면창이 아니라 변경된 모든 Zone들의 설비 정보를 저장한다.
        private bool MakeEquipmentListTablet(XmlWriter writer)
        {
            writer.WriteStartElement("FireEquipmentList");

            IOManager ioMgr = FormMain2.Instance.IOManager;
            ArrayList arrChangedZone = ioMgr.ChangedZones;

            foreach (Zone zone in arrChangedZone)
            {
                ArrayList arrEquipments = ioMgr.GetEquipments(zone);

                foreach (FireEquipment equip in arrEquipments)
                {
                    if (!MakeEquipment(writer, equip))
                        return false;
                }
            }
            /*ArrayList arrEquipments = FormMain2.Instance.DXFManager.Equipments;
            
            foreach (FireEquipment equip in arrEquipments)
            {
                if (!MakeEquipment(writer, equip))
                    return false;
            }*/

            writer.WriteEndElement();
            return true;
        }

        private bool MakeEquipment(XmlWriter writer, FireEquipment equip)
        {
            writer.WriteStartElement("FireEquipment");

            writer.WriteStartElement("ID");
            writer.WriteString(equip.ID.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("RFIDTag");
            writer.WriteString(equip.RFIDTag);
            writer.WriteEndElement();

            writer.WriteStartElement("EquipID");
            writer.WriteString(equip.EquipID);
            writer.WriteEndElement();

            writer.WriteStartElement("RFIDTagID");
            writer.WriteString(equip.RFIDTagID);
            writer.WriteEndElement();

            writer.WriteStartElement("DxfObjID");
            writer.WriteString(equip.DXFObjID);
            writer.WriteEndElement();

            writer.WriteStartElement("EquipType");
            writer.WriteString(((int)equip.Type).ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("ZoneID");
            
            if (equip.Zone == null)
                writer.WriteString("-1");
            else
                writer.WriteString(equip.Zone.ID.ToString());

            writer.WriteEndElement();

            float x = equip.Position.X;
            float y = equip.Position.Y;

            //if (!FormMain2.Instance.IsPCMode)
            {
                float fUnitFlag = FormMain2.Instance.GetUnitFlag(UnitOfLength.METER);
                x *= fUnitFlag;
                y *= fUnitFlag;
            }

            writer.WriteStartElement("X");
            writer.WriteString(x.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Y");
            writer.WriteString(y.ToString());
            writer.WriteEndElement();
            
            writer.WriteStartElement("Description");
            writer.WriteString(equip.Description);
            writer.WriteEndElement();

            writer.WriteEndElement();
            return true;
        }

        private bool MakeZoneListPC(XmlWriter writer)
        {
            writer.WriteStartElement("ZoneList");

            Dictionary<int, Zone> dicZones = FormMain2.Instance.IOManager.AllZones;

            foreach (KeyValuePair<int, Zone> pair in dicZones)
            {
                if (!MakeZone(writer, pair.Value))
                    return false;
            }

            writer.WriteEndElement();
            return true;
        }

        // 현재 화면에 열려있는 Zone만 저장하는 것이 아니라 변경된 모든 Zone들을 저장한다.
        private bool MakeZoneListTablet(XmlWriter writer)
        {
            writer.WriteStartElement("ZoneList");

            ArrayList arrChangedZones = FormMain2.Instance.IOManager.ChangedZones;

            foreach (Zone zone in arrChangedZones)
            {
                if (!MakeZone(writer, zone))
                    return false;
            }

            writer.WriteEndElement();
            return true;

            /*Zone zone = FormMain2.Instance.CurrentZone;
            if (zone == null)
                return false;

            writer.WriteStartElement("ZoneList");

            if (!MakeZone(writer, zone))
                return false;

            writer.WriteEndElement();
            return true;*/
        }

        // strDXFFIlePath는 파일의 전체 경로를 담고 있는데, 이 정보는 PC가 바뀌면 의미가 없으므로
        // 실행경로/FEData/DXF 이후의 경로만 추출한다.
        private string GetDXFRelativePath(string strDXFFilePath)
        {
            int nIndex = strDXFFilePath.LastIndexOf('\\');

            if (nIndex < 0)
                return strDXFFilePath;

            nIndex = strDXFFilePath.LastIndexOf('\\', nIndex - 1);

            if (nIndex < 0)
                return strDXFFilePath;

            return strDXFFilePath.Substring(nIndex + 1);
        }

        private bool MakeZone(XmlWriter writer, Zone zone)
        {
            writer.WriteStartElement("Zone");

            writer.WriteStartElement("ID");
            writer.WriteString(zone.ID.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("ZoneName");
            writer.WriteString(zone.ZoneName);
            writer.WriteEndElement();

            writer.WriteStartElement("BuildingID");

            if (zone.Building == null)
                writer.WriteString("-1");
            else
                writer.WriteString(zone.Building.ID.ToString());

            writer.WriteEndElement();

            writer.WriteStartElement("FloorIndex");
            writer.WriteString(zone.FloorIndex.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("AddFloor");
            writer.WriteString(string.Format("{0:f1}", zone.AddFloor));
            writer.WriteEndElement();

            writer.WriteStartElement("DXFName");
            //writer.WriteString(zone.DXFFilePath);
            writer.WriteString(GetDXFRelativePath(zone.DXFFilePath));
            writer.WriteEndElement();

            writer.WriteEndElement();
            return true;
        }

        private bool MakeBuildingListPC(XmlWriter writer)
        {
            writer.WriteStartElement("BuildingList");

            Dictionary<int, Building> dicBuildings = FormMain2.Instance.IOManager.AllBuildings;

            foreach (KeyValuePair<int, Building> pair in dicBuildings)
            {
                if (!MakeBuilding(writer, pair.Value))
                    return false;
            }

            writer.WriteEndElement();
            return true;
        }

        private bool MakeBuildingListTablet(XmlWriter writer)
        {
            writer.WriteStartElement("BuildingList");

            ArrayList arrChangedZones = FormMain2.Instance.IOManager.ChangedZones;

            foreach (Zone zone in arrChangedZones)
            {
                if (zone.Building == null)
                    continue;

                if (!MakeBuilding(writer, zone.Building))
                    return false;
            }

            writer.WriteEndElement();

            /*Zone zone = FormMain2.Instance.CurrentZone;
            if (zone == null)
                return false;

            if (zone.Building == null)
                return true;

            writer.WriteStartElement("BuildingList");

            if (!MakeBuilding(writer, zone.Building))
                return false;

            writer.WriteEndElement();*/
            return true;
        }

        private bool MakeBuilding(XmlWriter writer, Building building)
        {
            writer.WriteStartElement("Building");

            writer.WriteStartElement("ID");
            writer.WriteString(building.ID.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("BuildingID");
            writer.WriteString(building.BuildingID);
            writer.WriteEndElement();

            writer.WriteStartElement("BuildingCode");
            writer.WriteString(building.BuildingCode);
            writer.WriteEndElement();

            writer.WriteStartElement("BuildingName");
            writer.WriteString(building.BuildingName);
            writer.WriteEndElement();

            writer.WriteStartElement("BuildingGroupID");

            if (building.BuildingGroup == null)
                writer.WriteString("-1");
            else
                writer.WriteString(building.BuildingGroup.ID.ToString());

            writer.WriteEndElement();

            writer.WriteStartElement("MaxFloor");
            writer.WriteString(building.MaxFloorIndex.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("MinFloor");
            writer.WriteString(building.MinFloorIndex.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();
            return true;
        }

        private bool MakeBuildingGroupListPC(XmlWriter writer)
        {
            writer.WriteStartElement("BuildingGroupList");

            Dictionary<BuildingGroup, ArrayList> dicBuildingGroups = FormMain2.Instance.IOManager.AllBuildingGroups;

            foreach (KeyValuePair<BuildingGroup, ArrayList> pair in dicBuildingGroups)
            {
                if (!MakeBuildingGroup(writer, pair.Key))
                    return false;
            }

            writer.WriteEndElement();
            return true;
        }

        private bool MakeBuildingGroupListTablet(XmlWriter writer)
        {
            writer.WriteStartElement("BuildingGroupList");

            ArrayList arrChangedZones = FormMain2.Instance.IOManager.ChangedZones;

            foreach (Zone zone in arrChangedZones)
            {
                if (zone.Building == null)
                    continue;

                if (!MakeBuildingGroup(writer, zone.Building.BuildingGroup))
                    return false;
            }

            writer.WriteEndElement();

            /*Zone zone = FormMain2.Instance.CurrentZone;
            if (zone == null)
                return false;

            if (zone.Building == null)
                return true;

            writer.WriteStartElement("BuildingGroupList");

            if (!MakeBuildingGroup(writer, zone.Building.BuildingGroup))
                return false;

            writer.WriteEndElement();*/
            return true;
        }

        private bool MakeBuildingGroup(XmlWriter writer, BuildingGroup buildingGroup)
        {
            writer.WriteStartElement("BuildingGroup");

            writer.WriteStartElement("ID");
            writer.WriteString(buildingGroup.ID.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("GroupName");
            writer.WriteString(buildingGroup.BuildingGroupName);
            writer.WriteEndElement();

            writer.WriteEndElement();
            return true;
        }

        /*private XmlWriter InitWriter(MemoryStream stream)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;

            XmlWriter writer = XmlWriter.Create(stream, settings);
            writer.WriteStartDocument();

            return writer;
        }*/

        private XmlTextWriter InitWriter(string strPath)
        {
            XmlTextWriter writer = new XmlTextWriter(strPath, Encoding.UTF8);

            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();

            return writer;
        }
    }

    public class FMFHeader
    {
        private string m_strVersion = "";
        private DateTime m_dtTime;
        private string m_strWriter = "";
        private string m_strDescription = "";

        public string Version
        {
            get { return m_strVersion; }
            set { m_strVersion = value; }
        }

        public DateTime Time
        {
            get { return m_dtTime; }
            set { m_dtTime = value; }
        }

        public string Writer
        {
            get { return m_strWriter; }
            set { m_strWriter = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }
    }
}
