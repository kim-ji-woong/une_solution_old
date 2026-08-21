using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DBUtility;
using SDMS;
using UnE.Spatial;
using UnE.Sensor;

namespace SDMSServer
{
    public class IOManager
    {
        //EquipmentZone에 속해있는 SensorZone List(EquipmentZone, SensorZone List)
        private Dictionary<EquipmentZone, ArrayList> d_EquipZoneSensor = new Dictionary<EquipmentZone, ArrayList>();
        public Dictionary<EquipmentZone, ArrayList> D_EquipZoneSensor
        {
            get { return d_EquipZoneSensor; }
            set { d_EquipZoneSensor = value; }
        }

        //SensorZone(SensorZone ID, SensorZone)
        private Dictionary<int, SensorZone> d_SensorZone = new Dictionary<int, SensorZone>();

        // 같은 설비영역을 공유하며, Type이 같은 Sensor들을 하나의 그룹으로 묶어 관리한다.
        // Key : SensorZoneGroup의 ID인데 EquipZone ID와 SensorType의 조합이다.
        //       상위 4바이트 : EquipZone ID
        //       하위 4바이트 : SensorType(Facility.FacilityType)
        private Dictionary<long, SensorZoneGroup> m_dicSensorZoneGroup = new Dictionary<long, SensorZoneGroup>();

        private DBUtility.WebDBManager m_dbMgr = null;

        private int m_nSiteID = 1;
        public IOManager(DBUtility.WebDBManager dbMgr)
        {
            m_nSiteID = NetworkServer.Instance.SiteID;

            m_dbMgr = dbMgr;
			
			LoadSensorZone();
        }      

        public void LoadSensorZone()
        {
            //string strSQL = "select ID,Type, Connected, EquipZoneID, Data, OrgSensorID from SensorZone";
            // EquipmentZone에 추가된 SiteID를 이용하여 Site별 데이터를 구분하도록 수정함. skkim 2015.01.14
            string szText = "SELECT sz.ID,sz.Type,sz.Connected, sz.EquipZoneID, sz.Data, sz.OrgSensorID, sz.Zone " +
                            " FROM SensorZone as sz, EquipmentZone as ez WHERE sz.EquipZoneID = ez.ID and ez.SiteID = {0}";
            string strSQL = string.Format(szText, m_nSiteID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nConnected = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nData = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nLinkedSensorID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nZoneID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);

                EquipmentZone equipZone = null;

                if (ZoneManager.Instance.DicEquipZones.ContainsKey(nEquipZoneID))
                    equipZone = ZoneManager.Instance.DicEquipZones[nEquipZoneID];
                else
                {
                    equipZone = null;
                    //continue;
                }

                SensorZone sensorZone = new SensorZone();

                sensorZone.ID = nID;
                sensorZone.Type = IFacility.ToFacilityType(nType);
                sensorZone.IsConnected = nConnected == 1;
                sensorZone.SensorData = nData;
                sensorZone.LinkedSensorID = nLinkedSensorID;
                sensorZone.EquipZone = equipZone;
                sensorZone.ZoneID = nZoneID;

                d_SensorZone[nID] = sensorZone;

                if (equipZone != null)
                {
                    if (d_EquipZoneSensor.ContainsKey(equipZone))
                    {
                        ArrayList arrZones = d_EquipZoneSensor[equipZone];
                        arrZones.Add(sensorZone);
                    }
                    else
                    {
                        ArrayList arrZones = new ArrayList();
                        arrZones.Add(sensorZone);

                        d_EquipZoneSensor[equipZone] = arrZones;
                    }
                }

                SensorZoneGroup group = GetSensorZoneGroup(nEquipZoneID, sensorZone.Type);
                group.SensorDatas[sensorZone] = null;
            }
        }

        public SensorZone GetSensorZone(int nSensorID)
        {
            if (d_SensorZone.ContainsKey(nSensorID))
                return d_SensorZone[nSensorID];

            return null;
        }

        public SensorZone GetSensorZone(int nOrgSensorID, IFacility.FacilityType sensorType, EquipmentZone equipZone)
        {
            if (equipZone != null)
            {
                SensorZoneGroup group = GetSensorZoneGroup(equipZone, sensorType);

                if (group != null)
                {
                    foreach (KeyValuePair<SensorZone, object> pair in group.SensorDatas)
                    {
                        if (pair.Key.LinkedSensorID == nOrgSensorID)
                            return pair.Key;
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<int, SensorZone> pair in d_SensorZone)
                {
                    if (pair.Value.Type == sensorType && pair.Value.LinkedSensorID == nOrgSensorID)
                    {
                        return pair.Value;
                    }
                }
            }

            return null;
        }

        // nSensorZoneID가 포함된 SensorZoneGroup을 리턴한다.
        public SensorZoneGroup GetSensorZoneGroup(int nSensorZoneID)
        {
            foreach (KeyValuePair<long, SensorZoneGroup> pair in m_dicSensorZoneGroup)
            {
                SensorZoneGroup group = pair.Value;

                foreach (KeyValuePair<SensorZone, object> sensorData in group.SensorDatas)
                {
                    if (sensorData.Key.ID == nSensorZoneID)
                        return group;
                }
            }

            return null;
        }

        public SensorZoneGroup GetSensorZoneGroup(EquipmentZone equipZone, IFacility.FacilityType sensorType)
        {
            long nID = SensorZoneGroup.ToID(equipZone, sensorType);
            return GetSensorZoneGroup(nID, -1, equipZone, sensorType);
        }

        public SensorZoneGroup GetSensorZoneGroup(int nEquipZoneID, IFacility.FacilityType sensorType)
        {
            long nID = SensorZoneGroup.ToID(nEquipZoneID, sensorType);
            return GetSensorZoneGroup(nID, nEquipZoneID, null, sensorType);
        }

        private SensorZoneGroup GetSensorZoneGroup(long nSensorZoneGroupID, int nEquipZoneID, EquipmentZone equipZone, IFacility.FacilityType sensorType)
        {
            SensorZoneGroup group = null;

            if (m_dicSensorZoneGroup.TryGetValue(nSensorZoneGroupID, out group))
                return group;

            if (equipZone == null && nEquipZoneID >= 0)
                ZoneManager.Instance.DicEquipZones.TryGetValue(nEquipZoneID, out equipZone);

            group = new SensorZoneGroup();
            group.EquipmentZone = equipZone;
            group.SensorType = sensorType;

            m_dicSensorZoneGroup[group.ID] = group;
            return group;
        }
    }
}
