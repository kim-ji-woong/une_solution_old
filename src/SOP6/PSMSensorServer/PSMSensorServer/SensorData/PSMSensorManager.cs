using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DBUtility2;
using System.IO;
using System.Data.SqlClient;

namespace PSMSensorServer
{
    public class PSMSensorManager
    {
        private int m_nSiteID = 1;
        private WebDBManager mDBMgr = null;
        private Thread m_DensityCheckThread = null;

        private string SENSOR_VALUE_BASE_FOLDER = null;

        private GasDetector.DetectorManager dm = new GasDetector.DetectorManager();
        public GasDetector.DetectorManager Detector
        {
            get { return dm; }
        }

        private static PSMSensorManager m_instance = null;
        public static PSMSensorManager Instance
        {
            get { return m_instance; }
        }

        //private NetworkWebClient m_mgrWeb = null;
        public PSMSensorManager(NetworkWebClient clientWeb)
        {
            m_nSiteID = PSMNetworkServer.Instance.DBManager.SiteID;

            mDBMgr = PSMNetworkServer.Instance.DBManager;
            /*if (m_nSiteID == 1)
            {
                mDBMgr = new DBUtility.WebDBManager("SOP3");
                mDBMgr.DatabaseName = "SOP3_REV";
                mDBMgr.WebServerURL = "http://172.18.101.50:8080/SUP_SUB";
            }
            else if (m_nSiteID == 2)
            {
                mDBMgr = new DBUtility.WebDBManager("SOP4");
            }*/

            //m_mgr = client;
            //m_mgrWeb = clientWeb;

            m_instance = this;
            SetSensorValuesLogFolder();
        }

        private void SetSensorValuesLogFolder()
        {
            string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            string strFolder = Path.GetDirectoryName(exePath);
            SENSOR_VALUE_BASE_FOLDER = strFolder + "\\PSMSensorValuesLog";

            if (Directory.Exists(SENSOR_VALUE_BASE_FOLDER) == false)
                Directory.CreateDirectory(SENSOR_VALUE_BASE_FOLDER);
        }

        public void RequestAlarm(int nSensorID)
        {
            foreach (PSMSensorInfo sensor in m_arPSMSensors)
            {
                if (sensor.SensorZoneID == nSensorID)
                {
                    sensor.RequestAlarm = true;
                    break;
                }
            }
        }

        public void RequestReset(int nSensorID)
        {
            foreach (PSMSensorInfo sensor in m_arPSMSensors)
            {
                if (sensor.SensorZoneID == nSensorID)
                {
                    sensor.RequestReset = true;
                    break;
                }
            }
        }

        public void BuzzerSet(int nSensorID, int nOnOff)
        {
            foreach (PSMSensorInfo sensor in m_arPSMSensors)
            {
                if (sensor.SensorZoneID == nSensorID)
                {
                    sensor.SetBuzzer(nOnOff);
                    break;
                }
            }
        }

        private Action<int, int, float, int, int> mNotifyAction = null;
        public void BeginServer(Action<int, int, float, int, int> onNotify)
        {
            try
            {
                for (int i = 1; i <= 24; i++)
                {
                    dm.SetNotify(i, 0, 0, false);
                    dm.SetNotify(i, 0, 1, false);
                    dm.SetNotify(i, 0, 2, false);
                }

                mNotifyAction = onNotify;
                dm.OnNotifyAlarm += GasDetector_OnNotifyAlarm;
                dm.Start();

            }
            catch(Exception)
            { }

            LoadPSMSensor();
            m_DensityCheckThread = new Thread(CheckDensity);
            m_DensityCheckThread.Name = "Sensor value Check";
            m_DensityCheckThread.Start();
        }

        private void GasDetector_OnNotifyAlarm(int nComm, int nAlarmUnit, float fValue, int nChannel, int nStatus)
        {
            if(mNotifyAction != null)
            {
                mNotifyAction.Invoke(nComm, nAlarmUnit, fValue, nChannel, nStatus);
            }           
        }

        public void StopServer()
        {
            try
            {
                m_bReleaseThread = true;
                if (m_DensityCheckThread != null)
                    m_DensityCheckThread.Join(2000);
            }
            catch(Exception)
            {}            

            dm.OnNotifyAlarm -= GasDetector_OnNotifyAlarm;
            dm.End();

            mNotifyAction = null;
        }

        private List<PSMSensorInfo> m_arPSMSensors = new List<PSMSensorInfo>();
        private void LoadPSMSensor()
        {


            m_arPSMSensors.Clear();
            //string szSQL = "SELECT sti.ID, ssi.ReciverID, sti.TagNo, sti.SensorName, sti.EquipZoneID, sti.SensorZoneID, sz.OrgSensorID" +
            //               " FROM SensorTagInfo as sti "+
            //               " INNER JOIN SensorServerInfo as ssi on sti.SensorServerID = ssi.ID "+
            //               " INNER JOIN SensorZone as sz on sti.SensorZoneID = sz.ID "+
            //               " WHERE sti.SensorType = 11";



            //string szSQL = "SELECT sti.ID, ssi.ReciverID, sti.TagNo, sti.SensorName, sti.EquipZoneID, sti.SensorZoneID, sz.OrgSensorID, ps.SensorName, ps.SensorValueIdx" +
            //                " , ps.LimitLevel1, ps.LimitLevel2, ps.LimitLevel3 " +          
            //                " FROM SensorTagInfo as sti " +
            //                " INNER JOIN SensorServerInfo as ssi on sti.SensorServerID = ssi.ID " +
            //                " INNER JOIN SensorZone as sz on sti.SensorZoneID = sz.ID " +
            //                " INNER JOIN PSMSensor as ps on sz.OrgSensorID = ps.ID " +
            //                " WHERE sti.SensorType = 11 order by sz.OrgSensorID";

            string szSQL = "SELECT sti.ID, ssi.ReciverID, sti.TagNo, sti.SensorName, sti.EquipZoneID, sti.SensorZoneID, sz.OrgSensorID, ps.SensorName, ps.SensorValueIdx" +
                            " , ps.LimitLevel1, ps.LimitLevel2, ps.LimitLevel3 " +
                            " FROM SensorTagInfo as sti " +
                            " INNER JOIN SensorServerInfo as ssi on sti.SensorServerID = ssi.ID " +
                            " INNER JOIN SensorZone as sz on sti.SensorZoneID = sz.ID " +
                            " INNER JOIN PSMSensor as ps on sz.OrgSensorID = ps.ID "
                            + " WHERE sti.SensorType = 11  order by sz.OrgSensorID";

            string strSQL = string.Format(szSQL, m_nSiteID);

            ArrayList arrResult = mDBMgr.GetResultData(strSQL);
            int nResultCount = arrResult.Count;
            if (arrResult == null)
                return;

            for (int i = 0; i < nResultCount - 11; i += 12)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nReciverID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nTagNo = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                string szSensorName = WebDBManager.GetStringField(arrResult[i + 3], "");
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nSensorZoneID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nSensorID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);

                string szOrgSensorName = WebDBManager.GetStringField(arrResult[i + 7], "");
                int nIdx = WebDBManager.GetIntField(arrResult[i + 8].ToString(), -1);
                
                float fLevel1 = WebDBManager.GetFloatField(arrResult[i + 9].ToString(), 0.0f);
                float fLevel2 = WebDBManager.GetFloatField(arrResult[i + 10].ToString(), 0.0f);
                float fLevel3 = WebDBManager.GetFloatField(arrResult[i + 11].ToString(), 0.0f);

                PSMSensorInfo info = new PSMSensorInfo();
                info.ReciverID = nReciverID;
                info.TagID = nTagNo;
                info.EquipZoneID = nEquipZoneID;
                info.SensorZoneID = nSensorZoneID;
                info.SensorID = nSensorID;
                info.ValueIdx = nIdx;

                info.LimitLevel1 = fLevel1;
                info.LimitLevel2 = fLevel2;
                info.LimitLevel3 = fLevel3;

                m_arPSMSensors.Add(info);
            }
        }

        // 서버에게 아무것도 전달하지 않은 상태로 만든다.
        public void InitReceiverState()
        {
            ArrayList arReciverList = PSMNetworkServer.Instance.IOManager.GetPSMReciverList();
            foreach (Reciver receiver in arReciverList)
            {
                receiver.PrevConnected = -1;
            }
        }

        public void SaveAllSensorServerInfo(bool isConnected)
        {
            ArrayList arReciverList = PSMNetworkServer.Instance.IOManager.GetPSMReciverList();
            foreach (Reciver reciver in arReciverList)
            {
                SaveSensorServerInfo(reciver.ID, isConnected);
            }
        }

        private void SaveSensorServerInfo(int nReciverID, bool bOnline)
        {
            string szText = "UPDATE SensorServerInfo SET ConnectionState = {0} WHERE ID = {1}";
            string szSQL = string.Format(szText, (bOnline == true ? 1 : 0), nReciverID);
            mDBMgr.GetResultData(szSQL);
        }

        private void SavePSMLevel(PSMSensorInfo info)
        {
            string szText = "UPDATE PSMSensor SET CurrentLevel = {0} WHERE ID = {1}";
            string szSQL = string.Format(szText, info.Level, info.SensorID);
            mDBMgr.GetResultData(szSQL);
        }

        private void SavePSMDensity(PSMSensorInfo info)
        {
            string szText = "UPDATE PSMSensor SET CurrentData = {0} WHERE ID = {1}";
            string szSQL = string.Format(szText, info.Density, info.SensorID);
            mDBMgr.GetResultData(szSQL);
        }

        private string GetTableName(int nID)
        {
            string szTableName = "PSMSensorValue";
            string szTableNum = "";
            if (nID < 170000)
            {
                szTableNum = "1";
            }
            else if (nID < 340000 && nID >= 170000)
            {
                szTableNum = "2";
            }
            else if (nID < 510000 && nID >= 340000)
            {
                szTableNum = "3";
            }
            else if (nID < 680000 && nID >= 510000)
            {
                szTableNum = "4";
            }
            else if (nID < 850000 && nID >= 680000)
            {
                szTableNum = "5";
            }
            else if (nID < 1020000 && nID >= 850000)
            {
                szTableNum = "6";
            }
            else if (nID < 1190000 && nID >= 1020000)
            {
                szTableNum = "7";
            }
            else if (nID < 1360000 && nID >= 1190000)
            {
                szTableNum = "8";                
            }
            else if (nID < 1530000 && nID >= 1360000)
            {
                szTableNum = "9";
            }
            else if (nID >= 1530000)
            {
                szTableNum = "10";
            }

            return (szTableName + szTableNum);
        }

        private int GetLastID()
        {

            ArrayList arTotal = new ArrayList();
            for( int i = 1 ; i <= 10 ; i++)
            {
                 string sqlID = string.Format("SELECT TOP 1 ID,ValueTime FROM PSMSensorValue{0} order by ValueTime desc",i);
                ArrayList arrResult = mDBMgr.GetResultData(sqlID);
                if (arrResult != null)
                {
                    arTotal.AddRange(arrResult);
                }
            }

            int nMaxDtID = 0;
            DateTime maxDT = new DateTime();
            for( int i = 0 ; i < arTotal.Count; i+=2)
            {
                int nID = WebDBManager.GetIntField(arTotal[i].ToString(), -1);
                VariousData<DateTime> dt = WebDBManager.GetDateTimeField(arTotal[i+1].ToString());

                if( i == 0 )
                {
                    maxDT = dt.Data;
                    nMaxDtID = nID;
                }
                else
                {
                    if( maxDT < dt.Data)
                    {
                        nMaxDtID = nID;
                    }
                }
            }
            return nMaxDtID;
        }

        private int m_nMaxRecord = 1720000;
        private int m_nInsertID = 0;
        private void SavePSMAllSensorValue(List<PSMSensorInfo> arPSMInfos)
        {
            int nInsertID = m_nInsertID;

            if (nInsertID == 0)
            {
                nInsertID = GetLastID();
            }

            nInsertID++;
            if (nInsertID >= m_nMaxRecord)
                nInsertID = 1;

            string szTableName = GetTableName(nInsertID);

            DateTime dtNow = DateTime.Now;
            string strDateTimeField = string.Format("{0} {1}:{2}:{3}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second);

            string szText = string.Format("SELECT TOP 1 [ID]  FROM {0} WHERE ID = {1}", szTableName, nInsertID);

            try
            {
                WebDBManager dbMgr = PSMNetworkServer.Instance.DBManager;
                ArrayList arrResult = dbMgr.GetResultData(szText);

                float value = 0.0f;

                if (arrResult != null && arrResult.Count > 0)
                {
                    string szSQL = "UPDATE " + szTableName + " SET ValueTime = '" + strDateTimeField + "'";

                    foreach (PSMSensorInfo info in arPSMInfos)
                    {
                        value = (info.Density == -999 ? 0 : info.Density);
                        if (value == 0.0f)
                        {
                            szSQL += string.Format(" , SensorValue{0} = NULL ", info.ValueIdx);
                        }
                        else
                        {
                            szSQL += string.Format(" , SensorValue{0} = '{1:F2}' ", info.ValueIdx, value);
                        }
                    }

                    szSQL += " WHERE ID = " + nInsertID;
                    dbMgr.GetResultData(szSQL);
                }
                else
                {
                    string szHeader = string.Format("INSERT INTO {0} (ID ,ValueTime ", szTableName);
                    string szValues = string.Format(" ) VALUES ( {0}, '{1}' ", nInsertID, strDateTimeField);

                    foreach (PSMSensorInfo info in arPSMInfos)
                    {
                        value = (info.Density == -999 ? 0 : info.Density);
                        szHeader += string.Format(" , SensorValue{0} ", info.ValueIdx);

                        if (value == 0.0f)
                        {
                            szValues += " , NULL ";
                        }
                        else
                        {
                            szValues += string.Format(" , '{0:F2}' ", value);
                        }
                    }
                    szValues += ")";
                    string szSQL = szHeader + szValues;

                    dbMgr.GetResultData(szSQL);
                }

                m_nInsertID = nInsertID;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

            /*SqlDataReader reader = null;
            try
            {
                SqlConnection con = new SqlConnection(connectionMgr.GetConnectionInfo());
                con.Open();
                {
                    float value = 0.0f;
                    SqlCommand cmd = new SqlCommand(szText, con);
                    reader = cmd.ExecuteReader();
                    if( reader.Read())
                    {
                        reader.Close();
                        
                        string szSQL = "UPDATE " + szTableName + " SET ValueTime = '" + strDateTimeField + "'";
                        foreach (PSMSensorInfo info in arPSMInfos)
                        {
                            value = (info.Density == -999 ? 0 : info.Density);
                            if( value == 0.0f)
                            {
                                szSQL += string.Format(" , SensorValue{0} = NULL ", info.ValueIdx);
                            }
                            else
                            {
                                szSQL += string.Format(" , SensorValue{0} = '{1:F2}' ", info.ValueIdx, value);
                            }
                        }
                        szSQL += " WHERE ID = " + nInsertID;
                        //mDBMgr.GetResultData(szSQL);

                        SqlCommand cmd1 = new SqlCommand(szSQL, con);
                        cmd1.ExecuteScalar();
                    }
                    else
                    {
                        reader.Close();

                        string szHeader = string.Format("INSERT INTO {0} (ID ,ValueTime ", szTableName);
                        string szValues = string.Format(" ) VALUES ( {0}, '{1}' ", nInsertID, strDateTimeField);

                       
                        foreach (PSMSensorInfo info in arPSMInfos)
                        {
                            value = (info.Density == -999 ? 0 : info.Density);
                            szHeader += string.Format(" , SensorValue{0} ", info.ValueIdx);

                            if (value == 0.0f)
                            {
                                szValues += " , NULL ";
                            }
                            else
                            {
                                szValues += string.Format(" , '{0:F2}' ", value);
                            }
                        }
                        szValues += ")";
                        string szSQL = szHeader + szValues;

                        SqlCommand cmd2 = new SqlCommand(szSQL, con);
                        cmd2.ExecuteScalar();
                    }
                   
                }
                con.Close();

                m_nInsertID = nInsertID;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);

            }*/

            

            WritePSMSensorValuesLog(dtNow, strDateTimeField, arPSMInfos);
        }

        private void WritePSMSensorValuesLog(DateTime time, string strDateTime, List<PSMSensorInfo> arPSMInfos)
        {
            string strFolderName = time.Year.ToString();
            string strFolderPath = SENSOR_VALUE_BASE_FOLDER + "\\" + strFolderName;

            if (Directory.Exists(strFolderPath) == false)
                Directory.CreateDirectory(strFolderPath);

            string strFilePath = string.Format("{0}\\{1:00}.log", strFolderPath, time.Month);

            StreamWriter writer = null;

            if (File.Exists(strFilePath))
            {
                writer = new StreamWriter(strFilePath, true, Encoding.UTF8);
            }
            else
            {
                // 달이 바뀌었다.
                writer = new StreamWriter(strFilePath, false, Encoding.UTF8);

                Thread t = new Thread(new ParameterizedThreadStart(ZipNDeletePrevLogFile));
                t.Start(time);
            }

            string strLine = strDateTime;

            foreach (PSMSensorInfo info in arPSMInfos)
            {
                if (info.Density > 0)
                {
                    strLine += "\t" + info.ValueIdx.ToString() + "\t" + info.Density.ToString();
                }
            }

            writer.WriteLine(strLine);
            writer.Close();
        }

        // 이전달의 로그파일을 압축하고, 원본은 삭제한다.
        private void ZipNDeletePrevLogFile(object arg)
        {
            DateTime time = (DateTime)arg;

            string strFolderName = time.Month == 1 ? (time.Year - 1).ToString() : time.Year.ToString();
            string strFolderPath = SENSOR_VALUE_BASE_FOLDER + "\\" + strFolderName;

            if (Directory.Exists(strFolderPath) == false)
                return;

            string strFileName = string.Format("{0:00}", time.Month == 1 ? 12 : time.Month - 1);
            string strFilePath = string.Format("{0}\\{1}.log", strFolderPath, strFileName);

            if (File.Exists(strFilePath) == false)
                return;

            string strZipFilePath = string.Format("{0}\\{1}.zip", strFolderPath, strFileName);

            // 압축파일 생성
            using (var zip = System.IO.Compression.ZipFile.Open(strZipFilePath, System.IO.Compression.ZipArchiveMode.Create))
            {
                var entry = zip.CreateEntry(strFileName + ".log");

                using (var stream = File.OpenRead(strFilePath))
                {
                    using (var entryStream = entry.Open())
                    {
                        stream.CopyTo(entryStream);
                    }
                }
            }

            // 원본 삭제
            File.Delete(strFilePath);
        }

        private bool ValidSensorValue(PSMSensorInfo sensor)
        {
            float fValue = sensor.Density;
            int nAlarm = sensor.Level;

            if(nAlarm == 0)
            {
                if (sensor.LimitLevel1 > fValue)
                {
                    return true;
                }
                else
                    return false;
            }

            if(nAlarm == 1)
            {

            }

            return true;
        }

        private bool m_bReleaseThread = false;
        private void CheckDensity()
        {
            List<Reciver> changedReceivers = new List<Reciver>();

            while (!m_bReleaseThread)
            {
                if (m_arPSMSensors == null || m_arPSMSensors.Count == 0)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Thread.Sleep(100);
                        if (m_bReleaseThread == true)
                            break;
                    }
                }
                else
                {
                    foreach (PSMSensorInfo sensor in m_arPSMSensors)
                    {
                        bool bOnline = dm.GetOnline(sensor.ReciverID);
                        float fValue = dm.GetDensity(sensor.ReciverID, sensor.TagID);
                        int nAlarm1 = dm.GetStatus(sensor.ReciverID, sensor.TagID, 0);
                        int nAlarm2 = (dm.GetStatus(sensor.ReciverID, sensor.TagID, 1) == 1 ? 2 : 0);
                        int nAlarm3 = (dm.GetStatus(sensor.ReciverID, sensor.TagID, 2) == 1 ? 4 : 0);
                        int nAlarm4 = (dm.GetStatus(sensor.ReciverID, sensor.TagID, 3) == 1 ? 8 : 0);
                        int nAlarm = (nAlarm1 + nAlarm2 + nAlarm3 + nAlarm4);
                        if (nAlarm < 0)
                            nAlarm = 16;
                        // 가성소다 : DI(16번 채널)의경우 레벨값이 동작값이므로 아래 같이 처리하도록 함(kjw요청)
                        // add by skkim 2016-03-30
                        if (sensor.SensorID == 46)
                        {
                            int nValue = 0;
                            if (nAlarm > 0 && nAlarm < 16)
                            {
                                nValue = 1;
                            }
                            sensor.Density = nAlarm;
                            sensor.Level = nValue;

                            if (bOnline == false)
                            {
                                sensor.Density = -999.0f;
                                SavePSMDensity(sensor);
                            }
                            else
                            {
                                SavePSMDensity(sensor);
                            }

                            SavePSMLevel(sensor);
                        }
                        else
                        {
                            sensor.Density = fValue;
                            sensor.Level = nAlarm;
                            //if(!ValidSensorValue(sensor))
                            //{
                            //    sensor.Density = -999.0f;
                            //    sensor.Level = 0;
                            //}

                            SavePSMDensity(sensor);
                            SavePSMLevel(sensor);
                        }

                        if (sensor.RequestAlarm == true)
                        {
                            sensor.RequestAlarm = false;

                            dm.RequestNotify(sensor.ReciverID);

                        }

                        if (sensor.RequestReset == true)
                        {
                            sensor.RequestReset = false;

                            int nUnit = sensor.ReciverID;
                            dm.SetControlRegister(nUnit, 5, 0, 1);
                            dm.SetReset(nUnit);
                        }

                        if (sensor.Buzzer == true)
                        {
                            sensor.Buzzer = false;
                            int nUnit = sensor.ReciverID;
                            int nValue = sensor.BuzzerValue;
                            dm.SetControlRegister(nUnit, 5, 1, nValue);
                        }

                        if (m_bReleaseThread == true)
                            break;
                    }

                    changedReceivers.Clear();
                    SavePSMAllSensorValue(m_arPSMSensors);

                    ArrayList arReciverList = PSMNetworkServer.Instance.IOManager.GetPSMReciverList();
                    foreach (Reciver reciver in arReciverList)
                    {
                        bool bOnline = dm.GetOnline(reciver.ReciverID);
                        int nOnline = bOnline ? 1 : 0;

                        // 수신반의 접속정보가 바뀌었는지 확인
                        if (reciver.PrevConnected < 0 || reciver.PrevConnected != nOnline)
                            changedReceivers.Add(reciver);

                        if (bOnline != reciver.IsConnected)
                        {                            
                            reciver.IsConnected = bOnline;
                        }
                        // DB에 직접 쓰지 않고 SOPWebServer를 통하여 값을 전달하도록 한다.
                        //SaveSensorServerInfo(reciver.ID, bOnline);
                    }

                    if (changedReceivers.Count > 0)
                        NetworkWebClient.Instance.SendReceiverState(changedReceivers);

                    for (int i = 0; i < 10; i++)
                    {
                        Thread.Sleep(250);
                        if (m_bReleaseThread == true)
                            break;
                    }
                }

            }
        }       

        internal class PSMSensorInfo
        {
            private int m_nReciverID = -1;
            internal int ReciverID
            {
                get { return m_nReciverID; }
                set { m_nReciverID = value; }
            }

            private int m_nTagID = -1;
            internal int TagID
            {
                get { return m_nTagID; }
                set { m_nTagID = value; }
            }

            private int m_nSensorID = -1;
            internal int SensorID
            {
                get { return m_nSensorID; }
                set { m_nSensorID = value; }
            }

            private int m_nEquipZoneID = -1;
            internal int EquipZoneID
            {
                get { return m_nEquipZoneID; }
                set { m_nEquipZoneID = value; }
            }

            private int m_nSensorZoneID = -1;
            internal int SensorZoneID
            {
                get { return m_nSensorZoneID; }
                set { m_nSensorZoneID = value; }
            }

            private float m_fDensity = 0.0f;
            internal float Density
            {
                get { return m_fDensity; }
                set { m_fDensity = value; }
            }

            private int m_nLevel = 16;

            public int Level
            {
                get { return m_nLevel; }
                set { m_nLevel = value; }
            }

            private int m_nValueIdx = -1;
            public int ValueIdx
            {
                get { return m_nValueIdx; }
                set { m_nValueIdx = value; }
            }

            private bool m_bRequestAlarm = false;
            public bool RequestAlarm
            {
                get { return m_bRequestAlarm; }
                set { m_bRequestAlarm = value; }
            }

            private bool m_bRequestReset = false;
            public bool RequestReset
            {
                get { return m_bRequestReset; }
                set { m_bRequestReset = value; }
            }
            
            public void SetBuzzer(int nOnOff)
            {
                m_bSetBuzzer = true;
                m_nBuzzerValue = nOnOff;
            }

            private bool m_bSetBuzzer = false;
            public bool Buzzer
            {
                get { return m_bSetBuzzer; }
                set { m_bSetBuzzer = value; }
            }
            private int m_nBuzzerValue = -1;
            public int BuzzerValue
            {
                get { return m_nBuzzerValue; }
                set { m_nBuzzerValue = value; }
            }

            private float m_fLimitLevel1 = 0.0f;
            public float LimitLevel1
            {
                get { return m_fLimitLevel1; }
                set { m_fLimitLevel1 = value; }
            }

            private float m_fLimitLevel2 = 0.0f;
            public float LimitLevel2
            {
                get { return m_fLimitLevel2; }
                set { m_fLimitLevel2 = value; }
            }

            private float m_fLimitLevel3 = 0.0f;
            public float LimitLevel3
            {
                get { return m_fLimitLevel3; }
                set { m_fLimitLevel3 = value; }
            }


        }
    }
}
