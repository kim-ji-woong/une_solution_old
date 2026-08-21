using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DBUtility;
using SDMS;

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
                    continue;

                SensorZone sensorZone = new SensorZone();

                sensorZone.ID = nID;
                sensorZone.Type = nType;
                sensorZone.IsConnected = nConnected == 1;
                sensorZone.SensorData = nData;
                sensorZone.LinkedSensorID = nLinkedSensorID;
                sensorZone.EquipZone = equipZone;
                sensorZone.ZoneID = nZoneID;

                d_SensorZone[nID] = sensorZone;

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
        }

        public SensorZone GetSensorZone(int nSensorID)
        {
            if (d_SensorZone.ContainsKey(nSensorID))
                return d_SensorZone[nSensorID];

            return null;
        }

    }
}
