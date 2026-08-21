using dnsDBUtil;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MuxFireSensorServer
{
    public class SensorManager
    {
        private static SensorManager m_instance = null;

        // Key : TagID
        private Dictionary<int, SensorTag> m_dicSensorTagInfo = new Dictionary<int, SensorTag>();

        public static SensorManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new SensorManager();

                return m_instance;
            }
        }

        private SensorManager()
        {
        }

        public void LoadData(WebDBManager dbMgr)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select tag.ID, tag.TagNo, fire.Name, SensorType, sz.ID as SensorZoneID ");
            sb.Append("  From SdmsSensorFire as fire, SdmsSensorZone as sz, SdmsSensorTagInfo as tag ");
            sb.Append(" Where fire.ID = sz.OrgSensorID ");
            sb.Append("   And sz.ID = tag.SensorZoneID ");
            sb.Append("   And sz.SensorType = 0 ");

            ArrayList arrResult = dbMgr.GetResultData(sb.ToString());

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-4;i+=5)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> tagNo = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strSensorName = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<int> sensorType = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i + 4].ToString());

                if (id == null || tagNo == null || strSensorName == null || sensorType == null || sensorZoneID == null)
                    continue;

                SensorTag sensor = new SensorTag();

                sensor.ID = id.Data;
                sensor.TagNo = tagNo.Data;
                sensor.SensorName = strSensorName;
                sensor.SensorType = sensorType.Data;
                sensor.SensorZoneID = sensorZoneID.Data;

                m_dicSensorTagInfo[sensor.TagNo] = sensor;
            }
        }

        public SensorTag FindSensorTag(int nReceiverID, int nRelayTeam, int nLoopID, int nRelayID, int nTagID)
        {
            try
            {
                int nSensorTagID = GetSensorTagID(nReceiverID, nRelayTeam, nLoopID, nRelayID, nTagID);
                //int nSensorTagID = nReceiverID * 10000000 + nLoopID * 100000 + nRelayID * 100 + nTagID;
                SensorTag sensor;

                if (m_dicSensorTagInfo.TryGetValue(nSensorTagID, out sensor))
                    return sensor;
            }
            catch (System.Exception ex)
            {
                Logger.Instance.Write("[ERROR] SensorManager.cs > SensorTag FindSensorTag(int, int, int, int, int) :" + ex.Message);
                Logger.Instance.Write("[ERROR Parameter] " + nReceiverID + "," + nRelayTeam + "," + nLoopID + "," + nRelayID + "," + nTagID);
            }

            return null;
        }

        public SensorTag FindSensorTag2(int nSensorTagNo)
        {
            SensorTag sensor;

            if (m_dicSensorTagInfo.TryGetValue(nSensorTagNo, out sensor))
                return sensor;

            return null;
        }

        public static int GetSensorTagID(int nReceiverID, int nRelayTeam, int nLoopID, int nRelayID, int nTagID)
        {
            //return nReceiverID * 100000 + nLoopID * 10000 + nRelayID * 10 + nTagID;
            //1000000000 + 수신기*10000000 + 중계반*100000 + Loop*10000 + Relay*10 + TagID
            return 1000000000 + nReceiverID * 10000000 + nRelayTeam * 100000 + nLoopID * 10000 + nRelayID * 10 + nTagID;
        }
    }
}
