using dnsDBUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireSensorServer.Data
{
    /// <summary>
    /// DB에서 가져온 값 관리
    /// </summary>
    public class DataManager
    {
        private WebDBManager m_dbMgr = null;
        private static List<SensorInfo> m_dicSensorTagInfo = new List<SensorInfo>();
        public static List<SensorInfo> DicSensorTagInfo
        {
            get { return m_dicSensorTagInfo; }
            set { m_dicSensorTagInfo = value; }
        }

        public DataManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;

            LoadData();
        }

        public void LoadData()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select sz.ID as SensorZoneID, tag.ID as SensorTagInfoID, fire.Name, fire.ZoneID ");
            sb.Append("     , sz.EquipZoneID, tag.SensorServerID, tag.TagNo ");
            sb.Append("  From SdmsSensorFire as fire, SdmsSensorZone as sz, SdmsSensorTagInfo as tag ");
            sb.Append(" Where fire.ID = sz.OrgSensorID ");
            sb.Append("   And sz.ID = tag.SensorZoneID ");
            sb.Append("   And sz.SensorType = 0 And Name like '%T1-15-10 에어폼방출-10%' And tag.ID=1248 ");

            ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString());
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> sensorTagInfoID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string sensorName = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<int> zoneID = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<int> equipzoneID = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> sensorServerID = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                VariousData<int> tagNo = WebDBManager.GetIntField(arrResult[i + 6].ToString());

                if (sensorZoneID == null || sensorTagInfoID == null || zoneID == null || equipzoneID == null ||
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

                m_dicSensorTagInfo.Add(sensor);
            }
        }
    }
}
