using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireSensorServer.Data
{
    public class DataManager
    {
        private static DataManager m_instance = null;
        public static DataManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new DataManager();

                return m_instance;
            }
        }

        private static Dictionary<int, SensorInfo> m_dicSensorTagInfo = new Dictionary<int, SensorInfo>();
        
        public DataManager()
        {
        }

        public void LoadData(WebDBManager dbMgr)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select fs.ID as OrgSensorID, fs.Name, sz.ID as SensorZoneID, ");
            sb.Append("       fs.ZoneID, sz.EquipZoneID, sti.ID as SensorTagInfoID, SensorServerID, TagNo ");
            sb.Append("  From FireSensor as fs, SensorZone as sz, SensorTagInfo as sti ");
            sb.Append(" Where fs.ID = sz.OrgSensorID ");
            sb.Append("   And sz.ID = sti.SensorZoneID ");
            sb.Append("   And SensorType = 0 ");

            ArrayList arrResult = dbMgr.GetResultData(sb.ToString());
            if (arrResult == null)
                return;

            int arrCount = arrResult.Count;
            for (int i = 0; i < arrCount; i+=8)
            {
                VariousData<int> orgSensorID = WebDBManager.GetIntField(arrResult[i].ToString());
                string sensorName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i + 2].ToString());                
                VariousData<int> zoneID = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<int> equipzoneID = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> sensorTagInfoID = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                VariousData<int> sensorServerID = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                VariousData<int> tagNo = WebDBManager.GetIntField(arrResult[i + 7].ToString());

                if (orgSensorID == null || sensorZoneID == null || sensorTagInfoID == null || zoneID == null || equipzoneID == null ||
                    sensorServerID == null || tagNo == null)
                    continue;

                SensorInfo sensor = new SensorInfo();
                sensor.SensorZoneID = sensorZoneID.Data;
                sensor.SensorTagInfoID = sensorTagInfoID.Data;
                sensor.SensorName = sensorName;
                sensor.ZoneID = zoneID.Data;
                sensor.EquipZoneID = equipzoneID.Data;
                sensor.SensorServerID = sensorServerID.Data;
                sensor.TagNo = tagNo.Data;

                if (m_dicSensorTagInfo.ContainsKey(sensor.TagNo))
                    continue;

                m_dicSensorTagInfo.Add(sensor.TagNo, sensor);
            }
        }

        public SensorInfo FindSensorTag(int nReceiverID, int nUnitID, int nSystemID, int nLineID)
        {
            int nSensorTagID = GetSensorTagID(nReceiverID, nUnitID, nSystemID, nLineID);

            SensorInfo sensor;
            if (m_dicSensorTagInfo.TryGetValue(nSensorTagID, out sensor))
                return sensor;

            return null;
        }

        public static int GetSensorTagID(int nReceiverID, int nUnitID, int nSystemID, int nLineID)
        {
            // 맨 앞에 100000000 는 0으로 시작하는 수신때문에 그냥 붙인것
            // (수신기 * 1000000) + (유닛 * 10000) + (계통 * 1000) + 회선번호;
            return 100000000 + (nReceiverID * 1000000) + (nUnitID * 10000) + (nSystemID * 1000) + nLineID;
        }
    }
}
