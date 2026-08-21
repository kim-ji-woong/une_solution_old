using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace MuxFireSensorServer
{
    using Data;

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

        public void LoadData(DirectDBManagerEx dbMgr)
        {
            string strSQL = "Select ID, TagNo, TagID, SensorName, SensorType, SensorZoneID from SensorTagInfo";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-5;i+=6)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> tagNo = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> tagID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string strSensorName = WebDBManager.GetStringField(arrResult[i + 3]);
                VariousData<int> sensorType = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i + 5].ToString());

                if (id == null || tagNo == null || tagID == null || strSensorName == null || sensorType == null || sensorZoneID == null)
                    continue;

                SensorTag sensor = new SensorTag();

                sensor.ID = id.Data;
                sensor.TagNo = tagNo.Data;
                sensor.TagID = tagID.Data;
                sensor.SensorName = strSensorName;
                sensor.SensorType = sensorType.Data;
                sensor.SensorZoneID = sensorZoneID.Data;

                m_dicSensorTagInfo[sensor.TagID] = sensor;
            }
        }

        public SensorTag FindSensorTag(int nReceiverID, int nLoopID, int nRelayID, int nTagID)
        {
            int nSensorTagID = GetSensorTagID(nReceiverID, nLoopID, nRelayID, nTagID);
            //int nSensorTagID = nReceiverID * 10000000 + nLoopID * 100000 + nRelayID * 100 + nTagID;
            SensorTag sensor;

            if (m_dicSensorTagInfo.TryGetValue(nSensorTagID, out sensor))
                return sensor;

            return null;
        }

        public static int GetSensorTagID(int nReceiverID, int nLoopID, int nRelayID, int nTagID)
        {
            return nReceiverID * 10000000 + nLoopID * 100000 + nRelayID * 100 + nTagID;
        }
    }
}
