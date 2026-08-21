using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Sensor;
using DBUtility2;
using System.Collections;

namespace USSFireSensorServer.Data
{
    public class SensorTag
    {
        private int m_nID = -1;
        private int m_nReceiverID = -1;
        private int m_nTagNo = -1;
        private int m_nTagID = -1;
        private string m_strSensorName = "";
        private int m_nSensorType = -1;
        private int m_nSensorZoneID = -1;
        private int m_nEquipZoneID = -1;
        private int m_nZoneID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int ReceiverID
        {
            get { return m_nReceiverID; }
            set { m_nReceiverID = value; }
        }

        public int TagNo
        {
            get { return m_nTagNo; }
            set { m_nTagNo = value; }
        }

        public int TagID
        {
            get { return m_nTagID; }
            set { m_nTagID = value; }
        }

        public string SensorName
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
        }

        public int SensorType
        {
            get { return m_nSensorType; }
            set { m_nSensorType = value; }
        }

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        public int EquipmentZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        // Key : SensorTagID
        public static Dictionary<int, SensorTag> ReadFireSensors(WebDBManager dbMgr)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select sti.ID, SensorServerID, TagNo, TagID, SensorName, SensorType, sti.EquipZoneID, SensorZoneID, sz.Zone ");
            sb.Append("  From SensorTagInfo as sti, SensorZone as sz ");
            sb.Append(" Where SensorType = " + ((int)IFacility.FacilityType.FIRE_SENSOR).ToString());
            sb.Append("  And sti.SensorZoneID = sz.ID ");

            ArrayList arrResult = dbMgr.GetResultData(sb.ToString());

            if (arrResult == null)
                throw new Exception();

            Dictionary<int, SensorTag> dicSensors = new Dictionary<int, SensorTag>();

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-8;i+=9)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> receiverID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> tagNo = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> tagID = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strSensorName = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<int> sensorType = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i + 7].ToString());
                VariousData<int> zoneID = WebDBManager.GetIntField(arrResult[i + 8].ToString());

                if (id == null || tagNo == null || tagID == null || strSensorName == null ||
                    sensorType == null || equipZoneID == null || sensorZoneID == null || receiverID == null || zoneID == null)
                    continue;

                SensorTag sensorTag = new SensorTag();

                sensorTag.ID = id.Data;
                sensorTag.ReceiverID = receiverID.Data;
                sensorTag.TagNo = tagNo.Data;
                sensorTag.TagID = tagID.Data;
                sensorTag.SensorName = strSensorName;
                sensorTag.SensorType = sensorType.Data;
                sensorTag.EquipmentZoneID = equipZoneID.Data;
                sensorTag.SensorZoneID = sensorZoneID.Data;
                sensorTag.ZoneID = zoneID.Data;

                dicSensors[tagID.Data] = sensorTag;
            }

            return dicSensors;
        }
    }
}
