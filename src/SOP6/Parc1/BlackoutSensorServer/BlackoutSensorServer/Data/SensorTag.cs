using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using UnE.Sensor;

namespace BlackoutSensorServer.Data
{
    public class SensorTag
    {
        private int m_nID = -1;
        private string m_strSensorName = "";
        private int m_nSensorZoneID = -1;
        private int m_nEquipZoneID = -1;
        private string[] m_codes = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string SensorName
        {
            get { return m_strSensorName; }
            set { m_strSensorName = value; }
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

        public string[] Codes
        {
            get { return m_codes; }
            set { m_codes = value; }
        }

        public static Dictionary<int, SensorTag> ReadSensors(WebDBManager dbMgr)
        {
            string strSQL = "Select ID, SensorName, EquipZoneID, SensorZoneID, Description from SensorTagInfo where SensorType = " + ((int)IFacility.FacilityType.BLACKOUT).ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                throw new Exception();

            Dictionary<int, SensorTag> dicSensors = new Dictionary<int, SensorTag>();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());                
                string strSensorName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strCodes = WebDBManager.GetStringField(arrResult[i + 4]);

                if (id == null || strSensorName == null || equipZoneID == null || sensorZoneID == null || strCodes == null || strCodes.Length == 0)
                    continue;

                SensorTag sensorTag = new SensorTag();
                sensorTag.ID = id.Data;
                sensorTag.SensorName = strSensorName;
                sensorTag.EquipmentZoneID = equipZoneID.Data;
                sensorTag.SensorZoneID = sensorZoneID.Data;
                sensorTag.Codes = strCodes.Split(',');

                dicSensors[id.Data] = sensorTag;
            }

            return dicSensors;
        }
    }
}
