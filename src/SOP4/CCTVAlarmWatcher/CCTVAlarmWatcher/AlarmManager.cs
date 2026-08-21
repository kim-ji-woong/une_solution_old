using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using System.Collections;

namespace CCTVAlarmWatcher
{
    public class AlarmManager
    {
        private static AlarmManager m_instance = null;
        // Key : CCTV ID
        private Dictionary<int, CCTVAlarm> m_dicCCTV = new Dictionary<int,CCTVAlarm>();

        public static AlarmManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new AlarmManager();

                return m_instance;
            }
        }

        private AlarmManager()
        {
        }

        public bool ReadCCTVList(WebDBManager dbMgr, int nSiteID)
        {
            string strSQL = "select CCTVAlarm.ID, CCTV_ID, CCTVAlarm.EquipZoneID, CameraName, SensorZone.ID, SensorTagInfo.ID ";
            strSQL += "from CCTVAlarm, CCTV, SensorZone, SensorTagInfo ";
            strSQL += "where CCTV_ID = CCTV.ID and SensorZone.OrgSensorID = CCTV.ID and SensorZone.Type = " + CCTVAlarm.SensorType.ToString() + " ";
            strSQL += "and SensorTagInfo.SensorZoneID = SensorZone.ID and SensorTagInfo.SensorType = SensorZone.Type and SiteID = " + nSiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> cctvID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string strCameraName = WebDBManager.GetStringField(arrResult[i + 3]);
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> sensorTagInfoID = WebDBManager.GetIntField(arrResult[i + 5].ToString());

                if (id == null || cctvID == null || equipZoneID == null || strCameraName == null || sensorZoneID == null || sensorTagInfoID == null)
                    continue;

                CCTVAlarm cctv = new CCTVAlarm();
                cctv.ID = id.Data;
                cctv.CCTVID = cctvID.Data;
                cctv.EquipZoneID = equipZoneID.Data;
                cctv.CameraName = strCameraName;
                cctv.SensorZoneID = sensorZoneID.Data;
                cctv.SensorTagInfoID = sensorTagInfoID.Data;

                m_dicCCTV[cctvID.Data] = cctv;
            }

            return true;
        }

        public CCTVAlarm GetCCTV(int nCCTVID)
        {
            CCTVAlarm cctv = null;

            if (m_dicCCTV.TryGetValue(nCCTVID, out cctv))
                return cctv;

            return null;
        }
    }
}
