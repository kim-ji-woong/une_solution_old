using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using System.Collections;
using System.IO;

namespace PipeHistoryLocalService
{
    public class WritePipeHistory
    {
        private WebDBManager dbMgr = null;
        private int nPipeID;
        private List<string> filePaths = new List<string>();
        private string m_strLogFolder = "";
        public string LogFolder
        {
            get { return m_strLogFolder; }
            set { m_strLogFolder = value; }
        }
        private List<int> m_nPipeIDs = new List<int>();
        public List<int> nPipeIDs
        {
            get { return m_nPipeIDs; }
            set { m_nPipeIDs = value; }
        }

        public WritePipeHistory()
        {
            string location = System.Reflection.Assembly.GetEntryAssembly().Location;
            string strCurrentFolder = Path.GetDirectoryName(location);

            DBUtility.Utility util = new Utility();
            string path = strCurrentFolder + "\\config.ini";
            //m_strLogFolder = util.getinivalue("Option", "LogFolder", path);
            m_strLogFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            m_strLogFolder += "\\UNE\\KPX\\work";
           // if( m_strLogFolder == "")
            //{
                //string szPath = System.Reflection.Assembly.GetEntryAssembly().Location;
                //string szFullPath = System.IO.Directory.GetParent(szPath).FullName;
                //m_strLogFolder = szFullPath + "\\data";
           // }

            int nSiteID = 500;
            string siteID = util.getinivalue("Server Connection Info", "siteid");
            if (siteID != null)
                nSiteID = WebDBManager.GetIntField(siteID, 500);

            string szURL = util.getinivalue("Server Connection Info", "webserver_url");

            dbMgr = new WebDBManager(nSiteID);
            dbMgr.WebServerURL = szURL;
            dbMgr.DatabaseHost = "127.0.0.1";
            dbMgr.DatabaseName = "KPX";
            dbMgr.DatabasePort = "3306";
            dbMgr.DatabaseType = WebDBManager.DBType.mysql;

            LogFileManager.Instance.WriteLog("siteID : " + nSiteID.ToString());
            LogFileManager.Instance.WriteLog("URL : " + dbMgr.WebServerURL);

            if (m_strLogFolder.EndsWith("\\"))
                m_strLogFolder = m_strLogFolder.Substring(0, m_strLogFolder.Length - 1);
        }

        public void DisplayIDs()
        {
            m_nPipeIDs.Clear();

            string strSQL = "SELECT ID FROM kpx.pipe ORDER BY ID";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
            {
                Console.WriteLine("Tank History Select failed!");
                //LogFileManager.Instance.WriteLog("Pipe History Select failed!");
                return;
            }

            if (arrResult.Count == 0)
            {
                //LogFileManager.Instance.WriteLog(strSQL + ", Result Count is 0 ");
                return;
            }


            for (int i = 0; i < arrResult.Count; ++i)
            {
                m_nPipeIDs.Add(DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0));
            }
        }

        public void Start(bool bRealTime = false)
        {
            //LogFileManager.Instance.WriteLog("Start Function");
            filePaths.Clear();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            for (int i = 0; i < m_nPipeIDs.Count; ++i)
            {
                ReadDB(m_nPipeIDs[i], bRealTime);
                
                if (LogFileManager.Instance.AppClose)
                    break;
            } 

            //TEST
            //ReadDB(6, bRealTime);

            sw.Stop();
            Console.WriteLine("Pipe time : " + sw.Elapsed.ToString());
            LogFileManager.Instance.EndThread = true;
            //LogFileManager.Instance.WriteLog("End Function");
        }

        private void ReadDB(object pipeID, bool bRealTime)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            int nPipeID = (int)pipeID;
            if (nPipeID == 0)
                return;

            int nDefault = 0;
            DateTime dtDefault = new DateTime();

            this.nPipeID = nPipeID;

            string id = nPipeID.ToString();

            try
            {
                DateTime timeCur = DateTime.Now;
                DateTime timeYearBefore = DateTime.Now.AddYears(-1); 
                DateTime dt;

                //TEST
                //for (int i = 11; i <= 11; ++i)
                for (int i = 1; i <= 12; ++i)
                {
                    if (bRealTime && timeCur.Month != i)
                        continue;
                     
                    int nYear = timeCur.Year;
                    if (i > timeCur.Month)
                        nYear -= 1;
                     
                    // Last File Read
                    long nTime = 0;
                    string lastFilePath = string.Format("{3}\\{0}\\{1}\\{2}\\LastData.dat", nPipeID, nYear, i, m_strLogFolder);
                    if (File.Exists(lastFilePath))
                    {
                        // 파일공유 옵션 추가
                        using (BinaryReader reader = new BinaryReader(File.Open(lastFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                        {
                            int length = (int)reader.BaseStream.Length;
                            if (length == 8)
                            {
                                nTime = reader.ReadInt64();  // 종료시간
                            }
                        }
                    }
                     
                    if (nTime != 0 && timeCur.Month != i)
                        continue;

                    dt = DateTime.FromBinary(nTime);

                    // 1년이 지난 데이터는 저장하지 않음
                    DateTime delDt = new DateTime(nYear, i, dt.Day, dt.Hour, dt.Minute, dt.Second);
                    if (delDt <= timeYearBefore)
                        continue;

                    // PSensorServer 통신이 끊겨서 1년 지난 데이터가 삭제되지 않았을 때
                    // 1년전 해당 월(Month)의 데이터가 현재 데이터로 찍히는 현상 방지
                    // -> 현재 시간보다 LastDate가 더 크면 싹 삭제하고 다시 작성함
                    if (delDt > timeCur)
                    {
                        List<int> deleteFiles = new List<int>();
                        for (int j = 1; j <= dt.Day; j++)
                        {
                            deleteFiles.Add(j);
                        }

                        foreach (int file in deleteFiles)
                        {
                            string filePath = string.Format("{3}\\{0}\\{1}\\{2}\\", nPipeID, nYear, i, m_strLogFolder);
                            if (File.Exists(filePath + file + ".dat"))
                            {
                                File.Delete(filePath + file + ".dat");
                            }
                            if (File.Exists(filePath + file + "_temp.dat"))
                            {
                                File.Delete(filePath + file + "_temp.dat");
                            }
                            if (File.Exists(filePath + file + "_time.txt"))
                            {
                                File.Delete(filePath + file + "_time.txt");
                            }
                        }

                        dt = new DateTime(nYear, i, 1, 0, 0, 0);
                    }

                    string strTime = string.Format("{0}{1:00}{2:00}{3:00}", dt.Day, dt.Hour, dt.Minute, dt.Second);

                    //string query = string.Format("SELECT TimeStamp, Pressure, Flow FROM kpx.pipehistory_{0}_{1:D2} Where KeyTime > {2}", nPipeID, i, strTime);

                    string query = string.Format("SELECT TimeStamp, Pressure, ifnull(tank.Flow, 0) as Flow FROM pipehistory_{0}_{1:D2} as pipe " +
                                                  " LEFT JOIN zzz_flowhistory_{0}_{1:D2} as tank ON tank.KeyTime = pipe.KeyTime WHERE pipe.KeyTime > {2}", nPipeID, i, strTime);

                    ArrayList arrResult = dbMgr.GetResultData(query, 0);
                    if (arrResult == null)
                    {
                        continue;
                    }
                    if (arrResult.Count == 0) continue;

                    if (arrResult.Count > 0)
                    {

                    }

                    // WorkHistory 가져오기
                    strTime = DBUtility.WebDBManager.MakeDateTimeString(dt);
                    List<WorkHistory> listWorkHIstory = new List<WorkHistory>();
                    if (!bRealTime)
                        query = string.Format("select Tankid, BeginTime, EndTime from kpx.workhistory where Pipeid = {0} and (EndTime >= date_format('{1}', '%Y-%m-%d %H:%i:%s') or EndTime IS null) and TankID IS NOT NULL", nPipeID, strTime);
                    else
                        query = string.Format("select Tankid, BeginTime, EndTime from kpx.workhistory where Pipeid = {0} and BeginTime <= date_format('{1}', '%Y-%m-%d %H:%i:%s') and (EndTime >= date_format('{2}', '%Y-%m-%d %H:%i:%s') or EndTime IS null) and TankID IS NOT NULL", nPipeID, strTime, strTime);

                    // TEST
                    //string strTime2 = "2017-11-10 15:00:00";
                    //query = string.Format("select Tankid, BeginTime, EndTime from kpx.workhistory where Pipeid = {0} and BeginTime >= date_format('{1}', '%Y-%m-%d %H:%i:%s') and (EndTime >= date_format('{2}', '%Y-%m-%d %H:%i:%s') or EndTime IS null) and TankID IS NOT NULL", nPipeID, strTime2, strTime);
                  
                    ArrayList arrWorks = dbMgr.GetResultData(query, 0);
                    if (arrWorks != null && arrWorks.Count != 0)
                    {
                        int arrCnt = arrWorks.Count;
                        for (int k = 0; k < arrCnt; k += 3)
                        {
                            WorkHistory data = new WorkHistory();
                            data.tankID = WebDBManager.GetIntField(arrWorks[k].ToString(), nDefault);
                            data.sTime = WebDBManager.GetDateTimeField(arrWorks[k + 1], dtDefault);
                            data.eTime = WebDBManager.GetDateTimeField(arrWorks[k + 2], dtDefault);

                            //data.sTimeLong = data.sTime.ToBinary();
                            //data.eTimeLong = data.eTime.ToBinary();

                            if (data.eTime.Ticks < data.sTime.Ticks)
                                data.bEnd = false;
                            listWorkHIstory.Add(data);

                            if (LogFileManager.Instance.AppClose)
                                break;
                        }
                    }

                    if (LogFileManager.Instance.AppClose)
                        break;

                    dt = WebDBManager.GetDateTimeField(arrResult[0], dtDefault);

                    // 디렉토리 생성
                    CreateDir(nPipeID, dt);

                    int day = dt.Day;
                    List<WriteData> listData = new List<WriteData>();
                    Dictionary<int, List<FlowData>> dicFlow = new Dictionary<int, List<FlowData>>();

                    // Data가 써지기 전에 읽는 현상 해소를 위해
                    if (bRealTime)
                        System.Threading.Thread.Sleep(1000);

                    bool bWrite = false;
                    int cnt = arrResult.Count;
                    for (int k = 0; k < cnt; k += 3)
                    {
                        if (LogFileManager.Instance.AppClose)
                            break;

                        dt = WebDBManager.GetDateTimeField(arrResult[k], dtDefault);
                        float pressure = WebDBManager.GetFloatField(arrResult[k + 1].ToString(), 0f);
                        float dFlow = 0;//WebDBManager.GetFloatField(arrResult[k + 2].ToString(), 0f);

                        int nTankID = GetTankID(listWorkHIstory, dt);
                        if (nTankID != 0)
                        {
                            strTime = string.Format("{0}{1:00}{2:00}{3:00}", dt.Day, dt.Hour, dt.Minute, dt.Second);

                            if (!bRealTime)
                            {
                                if (!dicFlow.ContainsKey(nTankID))
                                {
                                    query = string.Format("SELECT flow, keytime from zzz_flowhistory_{0}_{1:D2}", nTankID, i);
                                    ArrayList arrFolws = dbMgr.GetResultData(query, 0);

                                    if (arrFolws != null && arrFolws.Count != 0)
                                    {
                                        float flowPrev = -1;
                                        List<FlowData> list = new List<FlowData>();
                                        int cntPipes = arrFolws.Count;
                                        for (int m = 0; m < cntPipes; m += 2)
                                        {
                                            float fFlow = WebDBManager.GetFloatField(arrFolws[m].ToString(), 0f);
                                            if (flowPrev != fFlow)
                                            {
                                                int keyTime = WebDBManager.GetIntField(arrFolws[m + 1].ToString(), -1);
                                                FlowData data = new FlowData(fFlow, keyTime);
                                                list.Add(data);
                                                flowPrev = fFlow;
                                            }
                                        }
                                        dicFlow.Add(nTankID, list);
                                    }
                                    else
                                    {
                                        List<FlowData> list = new List<FlowData>();
                                        dicFlow.Add(nTankID, list);
                                    }
                                }

                                int nKeyTime = WebDBManager.GetIntField(strTime, -1);
                                dFlow = GetFlow(dicFlow, nTankID, nKeyTime);
                                if (dFlow == 0)
                                {
                                    query = string.Format("SELECT flow from zzz_flowhistory_{0}_{1:D2} where keytime = {2}", nTankID, i, strTime);
                                    ArrayList arrFolws2 = dbMgr.GetResultData(query, 0);

                                    if (arrFolws2 != null && arrFolws2.Count != 0)
                                        dFlow = WebDBManager.GetFloatField(arrFolws2[0].ToString(), dFlow);
                                }
                            }
                            else
                            {
                                query = string.Format("SELECT flow from zzz_flowhistory_{0}_{1:D2} where keytime = {2}", nTankID, i, strTime);
                                ArrayList arrFolws2 = dbMgr.GetResultData(query, 0);

                                if (arrFolws2 != null && arrFolws2.Count != 0)
                                    dFlow = WebDBManager.GetFloatField(arrFolws2[0].ToString(), dFlow);
                            }
                            
                            /*strTime = string.Format("{0}{1:00}{2:00}{3:00}", dt.Day, dt.Hour, dt.Minute, dt.Second);
                            query = string.Format("SELECT flow from zzz_flowhistory_{0}_{1:D2} where keytime = {2}", nTankID, i, strTime);
                            ArrayList arrFolws = dbMgr.GetResultData(query, 0);

                            if (arrFolws == null)
                            {
                                k -= 3;
                                continue;
                            }
                            else if (arrFolws.Count != 0)
                                dFlow = WebDBManager.GetFloatField(arrFolws[0].ToString(), dFlow);*/

                            //if(dFlow == 0)
                            //    Console.WriteLine(query + "\n Flow : " + dFlow);
                        }

                        if (day != dt.Day)
                        {
                            string fileName = string.Format("{4}\\{0}\\{1}\\{2}\\{3}_temp.dat", nPipeID, dt.Year, dt.Month, day, m_strLogFolder);
                            WriteFile(fileName, listData, nTime);
                            day = dt.Day;
                            listData.Clear();
                            bWrite = true;
                        }
                        listData.Add(new WriteData(dt, pressure, dFlow, nTankID));

                        if (LogFileManager.Instance.AppClose)
                            break;
                    }
                    if (LogFileManager.Instance.AppClose)
                        break;

                    if (listData.Count != 0)
                    {
                        string fileName = string.Format("{4}\\{0}\\{1}\\{2}\\{3}_temp.dat", nPipeID, dt.Year, dt.Month, dt.Day, m_strLogFolder);
                        WriteFile(fileName, listData, nTime);
                        bWrite = true;
                    }

                    if(bWrite)
                    {
                        DateTime curTime = DateTime.Now;

                        // LastData
                        // 프로그램이 중간에 종료되었을 경우 파일의 무결성 체크를 위해 종료시간을 저장
                        lastFilePath = string.Format("{3}\\{0}\\{1}\\{2}\\LastData_temp.dat", nPipeID, dt.Year, dt.Month, m_strLogFolder);

                        // 파일공유 옵션 추가
                        using (BinaryWriter writer = new BinaryWriter(File.Open(lastFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)))
                        {
                            writer.Write(dt.Ticks);
                        }
                        filePaths.Add(lastFilePath);

                        // 원본 파일로 파일복사
                        cnt = filePaths.Count;
                        for (int k = 0; k < cnt; ++k)
                        {
                            string[] names = filePaths[k].Split('_');

                            // 파일 read, write 충돌로 인한 더미파일 생성
                            string path = names[0] + "_dummy.dat";
                            BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
                            writer.Close();
                            File.Copy(filePaths[k], names[0] + ".dat", true);
                            File.Delete(path);
                        }
                        filePaths.Clear(); 
                    }
                }
                Console.WriteLine("end time pipe " + nPipeID + " : " + sw.Elapsed.ToString());

                System.Threading.Thread.Sleep(100);
            }
            catch (Exception e)
            {
                Console.WriteLine("DB Read Error : " + e.Message);
            }
        }

        private float GetFlow(Dictionary<int, List<FlowData>> dic, int nTankID, int nKeyTime)
        {
            float flow = 0;

            foreach (KeyValuePair<int, List<FlowData>> kvp in dic)
            {
                if (kvp.Key == nTankID)
                {
                    List<FlowData> datas = kvp.Value;
                    int cnt = datas.Count;
                    if (cnt == 0)
                        return 0;

                    flow = datas[0].flow;
                    for (int i = 0; i < cnt; ++i)
                    {
                        if (datas[i].keyTime >= nKeyTime)
                            return flow;

                        flow = datas[i].flow;
                    }
                }
            }
            return 0;
        }

        private int GetTankID(List<WorkHistory> list, DateTime dt)
        {
            int cnt = list.Count;
            for (int i = 0; i < cnt; ++i)
            {
                if (list[i].sTime.Ticks <= dt.Ticks)
                {
                    if (!list[i].bEnd)
                    {
                        return list[i].tankID;
                    }
                    else
                    {
                        if (list[i].eTime.Ticks >= dt.Ticks)
                            return list[i].tankID;
                    }
                }
               
                //if ((list[i].sTime.Ticks <= dt.Ticks && !list[i].bEnd) || (list[i].sTime.Ticks <= dt.Ticks && list[i].eTime.Ticks >= dt.Ticks))
                //    return list[i].tankID;
            }
            return 0;
        }

        private void CreateDir(int id, DateTime dt)
        {
            string dir = string.Format("{0}\\{1}\\{2}\\{3}", m_strLogFolder, id, dt.Year, dt.Month);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        } 

        private void WriteFile(string path, List<WriteData> listData, long time)
        {
            FileMode mode = FileMode.Create;
            if (File.Exists(path) && time != 0)
                mode = FileMode.Append;

            // 파일공유 옵션 추가
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, mode, FileAccess.Write, FileShare.ReadWrite)))
            {
                foreach (WriteData data in listData)
                {
                    writer.Write(data.dt.Ticks);
                    writer.Write(data.pressure);
                    writer.Write(data.flow);
                    writer.Write(data.tankID);
                }
            }
            LogFileManager.Instance.WriteLog("Write : " + path);
            lock (filePaths)
                filePaths.Add(path);
        }

        /// <summary>
        /// Registry에서 서버 URL가져오기
        /// </summary>
        public string GetServerConnectionInfo(int nSiteID)
        {
            string strSection = "Server Connection Info";

            string webServerURL = RegUtil.ReadRegValue(strSection, "webserver_url", nSiteID);
            if (webServerURL == null || webServerURL == "")
            {
                webServerURL = GetDefaultWebServerURL(nSiteID);
                RegUtil.WriteRegValue(strSection, "webserver_url", webServerURL, nSiteID);
            }

            return webServerURL;
        }

        private string GetDefaultWebServerURL(int nSiteID)
        {
            switch (nSiteID)
            {
                // 삼천포
                case 1:
                    return "http://172.18.101.50:8080/SOP";
                // 영흥
                case 2:
                    return "http://172.20.127.150:8080/SOP";
                // 에너지 과제(광교지사)
                case 3:
                    return "http://192.168.0.195:8080/SOP";
                // 서울대학교
                case 100:
                    return "http://192.168.250.41:8080/SOP";
                // KPX
                case 500:
                    return "http://192.168.250.41:8080/SOP";
                default:
                    return "";
            }
        }
    }

    public class FlowData
    {
        public float flow;
        public int keyTime;

        public FlowData(float p, int kt)
        {
            flow = p;
            keyTime = kt;
        }
    }

    public class WriteData
    {
        public DateTime dt;
        public float pressure;
        public float flow;
        public int tankID;

        public WriteData(DateTime t, float p, float f, int nTankID)
        {
            dt = t;
            pressure = p;
            flow = f;
            tankID = nTankID;
        }
    }

    public class WorkHistory
    {
        public int tankID;
        public DateTime sTime;
        public DateTime eTime;
        public bool bEnd = true;

        //public long sTimeLong;
        //public long eTimeLong;
    }
}
