using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using System.Collections;
using System.Windows.Forms;

namespace SensorTester
{
    public class DataManager
    {
        private WebDBManager m_dbMgr = null;
        private int m_nSiteID = 100;

        private Dictionary<int, BuildingGroup> m_dicBuildingGroups = new Dictionary<int, BuildingGroup>();
        private Dictionary<int, Building> m_dicBuildings = new Dictionary<int, Building>();
        private Dictionary<int, Zone> m_dicZones = new Dictionary<int, Zone>();
        private Dictionary<int, EquipmentZone> m_dicEquipZones = new Dictionary<int, EquipmentZone>();
        private Dictionary<int, SensorZone> m_dicSensorZones = new Dictionary<int, SensorZone>();
        private Dictionary<int, SensorTag> m_dicSensorTags = new Dictionary<int,SensorTag>();

        private List<SensorTag> m_listAddedSensorTags = new List<SensorTag>();


        public DataManager(WebDBManager dbMgr, int nSiteID)
        {
            m_dbMgr = dbMgr;
            m_nSiteID = nSiteID;

            LoadDatas();
        }

        public SensorZone GetSensorZone(int nSensorZoneID)
        {
            SensorZone sensorZone;

            if (m_dicSensorZones.TryGetValue(nSensorZoneID, out sensorZone))
                return sensorZone;

            return null;
        }

        public Zone GetZoneForSearch(int nSameNameSearchCnt, string strSearchZoneName)
        {
            Zone zone = null;

            int cnt = 1;

            foreach (Zone item in from items in m_dicZones.Values.AsEnumerable()
                                  where items.Name.IndexOf(strSearchZoneName) != -1
                                  select items)
            {
                if (nSameNameSearchCnt == cnt++)
                {
                    zone = item;
                    break;
                }
            }

            return zone;
        }

        public SensorTag GetSensorTagForSearch(int nSameNameSearchCnt, string strSearchSensorName)
        {
            SensorTag sensor = null;

            int cnt = 1;

            foreach (SensorTag item in from items in m_dicSensorTags.Values.AsEnumerable()
                                       where items.SensorName.IndexOf(strSearchSensorName) != -1
                                       select items)
            {
                if (nSameNameSearchCnt == cnt++)
                {
                    sensor = item;
                    break;
                }
            }

            return sensor;
        }

        private bool LoadDatas()
        {
            if (LoadBuildingGroup())
            {
                if (LoadBuilding())
                {
                    if (LoadZone())
                    {
                        if (LoadEquipmentZone())
                        {
                            if (LoadSensorZone())
                            {
                                if (LoadSensorTag())
                                    return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool LoadSensorTag()
        {
            string strSQL = "select st.ID, st.SensorServerID, st.TagNo, st.SensorName, st.SensorType, st.SensorZoneID from SensorTagInfo as st, SensorServerInfo as ss where st.SensorServerID = ss.ID";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            SensorZone sensorZone;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nReceiverID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nTagID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                string strSensorName = WebDBManager.GetStringField(arrResult[i + 3], "null");
                int nSensorType = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nSensorZoneID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);

                if (nID < 0 || nSensorZoneID < 0)
                    continue;

                //if (nReceiverID == 0)
                 //   continue;

                if (!m_dicSensorZones.TryGetValue(nSensorZoneID, out sensorZone))
                    continue;

                SensorTag tag = new SensorTag();
                tag.ID = nID;
                tag.ReceiverID = nReceiverID;
                tag.SensorTagID = nTagID;
                tag.SensorName = strSensorName;
                
                tag.TagType = (SensorTag.SensorType)nSensorType;
                
                tag.SensorZone = sensorZone;

                m_dicSensorTags[nID] = tag;
            }

            return true;
        }

        private bool LoadSensorZone()
        {
            string strSQL = "select sz.ID, sz.EquipZoneID, sz.Data from SensorZone as sz, EquipmentZone as ez where EquipZoneID > 0 and EquipZoneID = ez.ID and ez.SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            EquipmentZone equipZone = null;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nData = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                if (nID < 0)
                    continue;

                if (!m_dicEquipZones.TryGetValue(nEquipZoneID, out equipZone))
                    continue;

                SensorZone sensorZone = new SensorZone();
                sensorZone.ID = nID;
                sensorZone.EquipmentZone = equipZone;
                sensorZone.SensorData = nData;

                m_dicSensorZones[nID] = sensorZone;
            }

            return true;
        }

        private bool LoadBuildingGroup()
        {
            string strSQL = "select ID, GroupName from BuildingGroup where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1], "null");

                if (nID < 0 || strName == "null" || strName.Length == 0)
                    continue;

                BuildingGroup group = new BuildingGroup();
                group.ID = nID;
                group.Name = strName;

                m_dicBuildingGroups[nID] = group;
            }

            return true;
        }

        private bool LoadBuilding()
        {
            string strSQL = "select Building.ID, BuildingName, BuildingGroupID, BroadCastingText, Building.DisplayText from Building, BuildingGroup where Building.BuildingGroupID = BuildingGroup.ID and BuildingGroup.SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            BuildingGroup group;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nBuildingGroupID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                string strBroadcastingName = WebDBManager.GetStringField(arrResult[i + 3], "null");
                string strDisplayText = WebDBManager.GetStringField(arrResult[i + 4], "null");

                if (nID < 0 || nBuildingGroupID < 0 || (strBroadcastingName == "null" || strBroadcastingName.Length == 0) && (strDisplayText == "null" || strDisplayText.Length == 0))
                    continue;

                if (!m_dicBuildingGroups.TryGetValue(nBuildingGroupID, out group))
                    continue;

                Building building = new Building();
                building.ID = nID;
                building.Name = strDisplayText == "null" || strDisplayText.Length == 0 ? strBroadcastingName : strDisplayText;
                building.BuildingGroup = group;

                m_dicBuildings[nID] = building;
            }

            return true;
        }

        private bool LoadZone()
        {
            string strSQL = "select ID, BuildingID, DisplayText from Zone where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            Building building = null;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nBuildingID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string strBroadcastingName = WebDBManager.GetStringField(arrResult[i + 2], "null");

                if (nID <= 0 || strBroadcastingName == "null" || strBroadcastingName.Length == 0)
                    continue;

                if (nBuildingID < 0)
                    building = null;
                else
                {
                    if (!m_dicBuildings.TryGetValue(nBuildingID, out building))
                        continue;
                }

                Zone zone = new Zone();
                zone.ID = nID;
                zone.Name = strBroadcastingName;
                zone.Building = building;
                zone.IsOutdoor = building == null;

                m_dicZones[nID] = zone;
            }

            return true;
        }

        private bool LoadEquipmentZone()
        {
            string strSQL = "select ID, LinkedZoneIDList, BroadcastName from EquipmentZone where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            Zone zone = null;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strLinkedZoneIDs = WebDBManager.GetStringField(arrResult[i + 1], "null");
                string strBroadcastingName = WebDBManager.GetStringField(arrResult[i + 2], "null");

                if (nID <= 0 || strLinkedZoneIDs == "null" || strLinkedZoneIDs.Length == 0 || strBroadcastingName == "null" || strBroadcastingName.Length == 0)
                    continue;

                List<int> ids = GetIDList(strLinkedZoneIDs);

                if (ids == null)
                    return false;

                EquipmentZone equipZone = new EquipmentZone();

                equipZone.ID = nID;
                equipZone.Name = strBroadcastingName;
                
                foreach (int nZoneID in ids)
                {
                    if (!m_dicZones.TryGetValue(nZoneID, out zone))
                        continue;
                    else
                        equipZone.LinkedZones.Add(zone);
                }

                m_dicEquipZones[nID] = equipZone;
            }

            return true;
        }

        private List<int> GetIDList(string strIDs)
        {
            List<int> ids = new List<int>();
            string[] arrTokens = strIDs.Split(',');

            int nID;

            foreach (string strToken in arrTokens)
            {
                if (int.TryParse(strToken.Trim(), out nID))
                    ids.Add(nID);
                else
                    return null;
            }

            return ids;
        }

        public List<SensorTag> GetSensorTagByZone(Zone zone)
        {
            List<SensorTag> listSensorTag = new List<SensorTag>();

            foreach (EquipmentZone itemEquipmentZone in from itemEquipmentZones in m_dicEquipZones.Values.AsEnumerable()
                                                        where itemEquipmentZones.LinkedZones.Contains(zone)
                                                        select itemEquipmentZones
                                                        )
            {
                foreach (SensorZone itemSensorZone in from itemSensorZones in m_dicSensorZones.Values.AsEnumerable()
                                                      where itemSensorZones.EquipmentZone == itemEquipmentZone
                                                      select itemSensorZones
                                                      )
                {
                    foreach (SensorTag itemSensorTag in from itemSensorTags in m_dicSensorTags.Values.AsEnumerable()
                                                        where itemSensorTags.SensorZone == itemSensorZone
                                                        select itemSensorTags
                                                       )
                    {
                        listSensorTag.Add(itemSensorTag);

                    }

                }

            }


            return listSensorTag;
        }

        public void MakeSensorTagTree(TreeView tree, string strSearchWord, bool bFire, bool bAccess, bool bSVMS, bool bEmpoll, bool bSecom)
        {
            // changed by mwkim 2015-11-06. 검색할 단어로 존을 먼저 검색하고 난뒤, 센서를 검색함.
            // 1. Zone 검색
            // 2. SensorTag 검색

            // Z. 검색어가 없는경우에는 모든 센서를 나타낸다.

            // s1 용 센서테스터로 변경작업 2017-04-03 skkim
            // 센서 타입별로 선택가능하도록

            m_listAddedSensorTags.Clear();


            if (String.IsNullOrWhiteSpace(strSearchWord) == false)
            {
                // 이름이 일치하는 Zone을 먼저 찾고,
                // Zone에 매핑되는 SensorZone정보를 찾아 SensorTag를 알아내서 노드를 추가한다.
                foreach (KeyValuePair<int, Zone> pair in m_dicZones)
                {
                    if (pair.Value.Name.IndexOf(strSearchWord, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        foreach (SensorTag item in GetSensorTagByZone(pair.Value))
                        {
                            if(bFire == false && (int)item.TagType == 0 )
                            {
                                continue;
                            }
                            else if (bEmpoll == false && (int)item.TagType == 4000)
                            {
                                continue;
                            }
                            else if (bSVMS == false && ((int)item.TagType >= 900 && (int)item.TagType <= 920 ))
                            {
                                continue;
                            }
                            else if (bAccess == false && ((int)item.TagType >= 1000 && (int)item.TagType < 4000))
                            {
                                continue;
                            }
                            else if (bSecom == false && ((int)item.TagType >= 5001 && (int)item.TagType <= 5002))
                            {
                                continue;
                            }

                            if (m_listAddedSensorTags.Contains(item) == false)
                            {
                                m_listAddedSensorTags.Add(item);
                                    AddNode(tree, item);
                                
                                
                            }
                        }
                    }
                }

                foreach (SensorTag item in from items in m_dicSensorTags.Values.AsEnumerable()
                                           where items.SensorName.Contains(strSearchWord)
                                           && m_listAddedSensorTags.Contains(items) == false
                                           select items
                    )
                {

                    if (bFire == false && (int)item.TagType == 0)
                    {
                        continue;
                    }
                    else if (bEmpoll == false && (int)item.TagType == 4000)
                    {
                        continue;
                    }
                    else if (bSVMS == false && ((int)item.TagType >= 900 && (int)item.TagType <= 920))
                    {
                        continue;
                    }
                    else if (bAccess == false && ((int)item.TagType >= 1000 && (int)item.TagType < 4000))
                    {
                        continue;
                    }
                    else if (bSecom == false && ((int)item.TagType >= 5001 && (int)item.TagType <= 5002))
                    {
                        continue;
                    }
                    AddNode(tree, item);
                }
            }
            else
            {
                foreach (KeyValuePair<int, SensorTag> pair in m_dicSensorTags)
                {
                    if (bFire == false && (int)pair.Value.TagType == 0)
                    {
                        continue;
                    }
                    else if (bEmpoll == false && (int)pair.Value.TagType == 4000)
                    {
                        continue;
                    }
                    else if (bSVMS == false && ((int)pair.Value.TagType >= 900 && (int)pair.Value.TagType <= 920))
                    {
                        continue;
                    }
                    else if (bAccess == false && ((int)pair.Value.TagType >= 1000 && (int)pair.Value.TagType < 4000))
                    {
                        continue;
                    }
                    else if (bSecom == false && ((int)pair.Value.TagType >= 5001 && (int)pair.Value.TagType <= 5002))
                    {
                        continue;
                    }

                    AddNode(tree, pair.Value);
                }
            }


            tree.ExpandAll();
        }

        private void AddNode(TreeView tree, SensorTag tag)
        {
            SensorZone sensorZone = tag.SensorZone;
            EquipmentZone equipZone = sensorZone.EquipmentZone;
            
            foreach (Zone zone in equipZone.LinkedZones)
            {
                if (zone.IsOutdoor)
                {
                    TreeNode root = GetSensorParent(tree, tag);
                    TreeNode grp = GetOutdoorZoneRootNode(root);
                    TreeNode zoneNode = GetZoneNode(grp.Nodes, zone);
                    AddSensorTagNode(zoneNode, tag);
                }
                else if (zone.Building != null)
                {
                    TreeNode root = GetSensorParent(tree, tag);
                    TreeNode grp = GetBuildingGroupNode(root, zone.Building.BuildingGroup);
                    //TreeNode buildingNode = GetBuildingNode(grp.Nodes, zone.Building);
                    TreeNode zoneNode = GetZoneNode(grp.Nodes, zone);
                    AddSensorTagNode(zoneNode, tag);
                }

                // 첫번째 Zone에만 넣도록 한다.
                break;
            }
        }

        private TreeNode GetSensorParent(TreeView tree, SensorTag tag)
        {
            string szTagName = "화재센서";
            int nTag = 0;
            if ((int)tag.TagType == 0 || (int)tag.TagType == 5000)
            {
                nTag = 0;
            }
            else if ((int)tag.TagType == 4000)
            {
                nTag = 4000;
                szTagName = "EMPOLL";
            }
            else if ((int)tag.TagType >= 900 && (int)tag.TagType <= 920)
            {
                nTag = 900;
                szTagName = "SVMS";
            }
            else if ((int)tag.TagType >= 1000 && (int)tag.TagType < 4000)
            {
                nTag = 1000;
                szTagName = "ACCESS";
            }
            else if ((int)tag.TagType >= 5001 && (int)tag.TagType <= 5002)
            {
                nTag = 5001;
                szTagName = "Secom";
            }

            foreach (TreeNode node in tree.Nodes)
            {
                if ((int)node.Tag == nTag)
                    return node;
            }

            TreeNode groupNode = null;
            groupNode = tree.Nodes.Add(szTagName);          
            groupNode.Tag = nTag;
            return groupNode;
        }


        private TreeNode GetBuildingGroupNode(TreeNode tree, BuildingGroup group)
        {
            int nOutdoorZoneIndex = -1;

            foreach (TreeNode node in tree.Nodes)
            {
                if (node.Tag == null)
                    nOutdoorZoneIndex = tree.Nodes.IndexOf(node);
                else if (node.Tag == group)
                    return node;
            }

            TreeNode groupNode = null;

            if (nOutdoorZoneIndex < 0)
                groupNode = tree.Nodes.Add(group.Name);
            else
                groupNode = tree.Nodes.Insert(nOutdoorZoneIndex, group.Name);

            groupNode.Tag = group;
            return groupNode;
        }

        private TreeNode AddSensorTagNode(TreeNode node, SensorTag tag)
        {
            TreeNode tagNode = node.Nodes.Add(tag.SensorName);
            tagNode.Tag = tag;
            return tagNode;
        }

        private TreeNode GetOutdoorZoneRootNode(TreeNode tree)
        {
            foreach (TreeNode node in tree.Nodes)
            {
                if (node.Tag == null)
                    return node;
            }

            return tree.Nodes.Add("실외영역");
        }

        private TreeNode GetZoneNode(TreeNodeCollection nodes, Zone zone)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag == zone)
                    return node;
            }

            TreeNode zoneNode = nodes.Add(zone.Name);
            zoneNode.Tag = zone;
            return zoneNode;
        }

        private TreeNode GetBuildingNode(TreeNodeCollection nodes, Building building)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag == building)
                    return node;
            }

            TreeNode buildingNode = nodes.Add(building.Name);
            buildingNode.Tag = building;
            return buildingNode;
        }


        public SensorTag GetSensorTagBySensorZoneID(int nSensorZoneID)
        {
            SensorTag sensor = null;

            foreach (SensorTag item in from items in m_dicSensorTags.Values.AsEnumerable()
                                       where items.SensorZone != null
                                       && items.SensorZone.ID == nSensorZoneID
                                       select items)
            {
                sensor = item;
                break;
            }

            return sensor;
        }


    }
}
