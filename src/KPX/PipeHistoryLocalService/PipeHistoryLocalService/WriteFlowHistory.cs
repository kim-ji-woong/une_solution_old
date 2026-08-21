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
    public class WriteFlowHistory
    {
        private WebDBManager dbMgr = null;
        private int nTankID;
        private List<string> filePaths = new List<string>();
        private string m_strLogFolder = "";
        public string LogFolder
        {
            get { return m_strLogFolder; }
            set { m_strLogFolder = value; }
        }
        private List<int> m_nTankIDs = new List<int>();
        public List<int> nTankIDs
        {
            get { return m_nTankIDs; }
            set { m_nTankIDs = value; }
        }

        public WriteFlowHistory()
        {
            string location = System.Reflection.Assembly.GetEntryAssembly().Location;
            string strCurrentFolder = Path.GetDirectoryName(location);

            DBUtility.Utility util = new Utility();
            string path = strCurrentFolder + "\\config.ini";
            //m_strLogFolder = util.getinivalue("Option", "LogFolder", path);
            m_strLogFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            m_strLogFolder += "\\UNE\\KPX\\flow";
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
            //dbMgr.DatabaseHost = "192.168.0.211";
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
            m_nTankIDs.Clear();

            string strSQL = "SELECT ID FROM kpx.tank ORDER BY ID";
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
                m_nTankIDs.Add(DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0)); 
            }
        }

        public void Start(bool bRealTime = false)
        {
            //LogFileManager.Instance.WriteLog("Start Function");
            filePaths.Clear();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            DisplayIDs();

            for (int i = 0; i < m_nTankIDs.Count; ++i)
            {
                ReadDB(m_nTankIDs[i], bRealTime); 

                if (LogFileManager.Instance.AppClose)
                    break;
            }
            //ReadDB(1, bRealTime);

            sw.Stop();
            Console.WriteLine("Tank time : " + sw.Elapsed.ToString());
            LogFileManager.Instance.EndThread = true;
            //LogFileManager.Instance.WriteLog("End Function");
        }

        private void ReadDB(object tankID, bool bRealTime)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            int nTankID = (int)tankID;
            if (nTankID == 0)
                return;

            this.nTankID = nTankID;

            string id = nTankID.ToString();

            try
            {
                DateTime timeCur = DateTime.Now;
                DateTime timeYearBefore = DateTime.Now.AddYears(-1);
 
                DateTime dt;

                //for (int i = 9; i <= 9; ++i)
                for (int i = 1; i <= 12; ++i)
                {
                    if (bRealTime && timeCur.Month != i)
                        continue;

                    int nYear = timeCur.Year;
                    if (i > timeCur.Month)
                        nYear -= 1;

                    // Last File Read
                    long nTime = 0;
                    string lastFilePath = string.Format("{3}\\{0}\\{1}\\{2}\\LastData.dat", nTankID, nYear, i, m_strLogFolder);
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

                    DateTime lastDT = DateTime.FromBinary(nTime);

                    // 1년이 지난 데이터는 저장하지 않음
                    DateTime delDt = new DateTime(nYear, i, lastDT.Day, lastDT.Hour, lastDT.Minute, lastDT.Second);
                    if (delDt <= timeYearBefore)
                        continue;

                    // PSensorServer 통신이 끊겨서 1년 지난 데이터가 삭제되지 않았을 때
                    // 1년전 해당 월(Month)의 데이터가 현재 데이터로 찍히는 현상 방지
                    // -> 현재 시간보다 LastDate가 더 크면 싹 삭제하고 다시 작성함
                    if (delDt > timeCur)
                    {                        
                        List<int> deleteFiles = new List<int>();
                        for (int j = 1; j <= lastDT.Day; j++)
                        {
                            deleteFiles.Add(j);
                        }

                        foreach (int file in deleteFiles)
                        {
                            string filePath = string.Format("{3}\\{0}\\{1}\\{2}\\", nTankID, nYear, i, m_strLogFolder);
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

                        lastDT = new DateTime(nYear, i, 1, 0, 0, 0);
                    }

                    string strLastTime = string.Format("{0}{1:00}{2:00}{3:00}", lastDT.Day, lastDT.Hour, lastDT.Minute, lastDT.Second);

                    string query = string.Format("SELECT KeyTime, flow, temperture, level  FROM kpx.zzz_flowhistory_{0}_{1:D2} Where KeyTime > {2}", nTankID, i, strLastTime);
                    ArrayList arrResult = dbMgr.GetResultData(query, 0);
                    if (arrResult == null)
                    {
                        continue;
                    }
                    if (arrResult.Count == 0) 
                        continue;

                    int nKeyTime = WebDBManager.GetIntField(arrResult[0].ToString(), 0);
                    dt = GetDateTime(nYear, i, nKeyTime);

                    // WorkHistory
                    float press = 0;
                    string strTime = DBUtility.WebDBManager.MakeDateTimeString(dt);
                    List<WorkHistory> listWorkHIstory = new List<WorkHistory>();
                    query = string.Format("select PipeID, BeginTime, EndTime from kpx.workhistory where PipeID IS NOT NULL AND TankID = {0} and (EndTime >= date_format('{1}', '%Y-%m-%d %H:%i:%s') or EndTime IS null)",
                        nTankID, strTime);
                    ArrayList arrWorks = dbMgr.GetResultData(query, 0);

                    // 디렉토리 생성
                    CreateDir(nTankID, nYear, i);

                    int day = dt.Day;
                    List<WriteFlowData> listData = new List<WriteFlowData>();

                    Dictionary<int, List<PipeData>> dicPipe = new Dictionary<int, List<PipeData>>();

                    // Data가 써지기 전에 읽는 현상 해소를 위해
                    if (bRealTime)
                        System.Threading.Thread.Sleep(1000);

                    bool bWrite = false;
                    int cnt = arrResult.Count;
                    for (int k = 0; k < cnt; k += 4 )
                    {
                        nKeyTime = WebDBManager.GetIntField(arrResult[k].ToString(),0);
                        float fFlow = WebDBManager.GetFloatField(arrResult[k+1].ToString(), 0f);
                        float fTemp = WebDBManager.GetFloatField(arrResult[k + 2].ToString(), 0f);
                        float fLevel = WebDBManager.GetFloatField(arrResult[k + 3].ToString(), 0f);

                        dt = GetDateTime(nYear, i, nKeyTime);
                       
                        if (day != dt.Day)
                        {
                            string fileName = string.Format("{4}\\{0}\\{1}\\{2}\\{3}_temp.dat", nTankID   , dt.Year, dt.Month, day, m_strLogFolder);
                            WriteFile(fileName, listData, nTime);
                            day = dt.Day;
                            listData.Clear();
                            bWrite = true;
                        }

                        if (arrWorks != null && arrWorks.Count != 0)
                        {
                            bool bAdd = false;
                            List<int> ids = GetPipeID(arrWorks, dt);

                            for (int n = 0; n < ids.Count; ++n)
                            {
                                if (!bRealTime) // 처음 로딩
                                {
                                    if (!dicPipe.ContainsKey(ids[n]))
                                    {
                                        query = string.Format("SELECT Pressure, KeyTime from pipehistory_{0}_{1:D2} where TimeStamp >= date_format('{2}', '%Y-%m-%d %H:%i:%s')",
                                            ids[n], i, strTime);
                                        ArrayList arrPipes = dbMgr.GetResultData(query, 0);
                                        if (arrPipes != null && arrPipes.Count != 0)
                                        {
                                            float pressPrev = -1;
                                            List<PipeData> list = new List<PipeData>();
                                            int cntPipes = arrPipes.Count;
                                            for (int m = 0; m < cntPipes; m += 2)
                                            {
                                                float fPress = WebDBManager.GetFloatField(arrPipes[m].ToString(), 0f);
                                                if (pressPrev != fPress)
                                                {
                                                    int keyTime = WebDBManager.GetIntField(arrPipes[m + 1].ToString(), 0);
                                                    PipeData data = new PipeData(fPress, keyTime);
                                                    list.Add(data);
                                                    pressPrev = fPress;
                                                }
                                            }
                                            dicPipe.Add(ids[n], list);
                                        }
                                        else
                                        {
                                            List<PipeData> list = new List<PipeData>();
                                            dicPipe.Add(ids[n], list);
                                        }
                                        //query = string.Format("SELECT Pressure from pipehistory_{0}_{1:D2} where keytime = {2}", nPipeID, i, nKeyTime);
                                        //ArrayList arrPipes = dbMgr.GetResultData(query, 0);
                                        //if (arrPipes != null && arrPipes.Count != 0)
                                        //    press = WebDBManager.GetFloatField(arrPipes[0].ToString(), 0);
                                    }

                                    press = GetPress(dicPipe, ids[n], nKeyTime);

                                    if (press == 0)
                                    {
                                        query = string.Format("SELECT Pressure from pipehistory_{0}_{1:D2} where keytime = {2}", ids[n], i, nKeyTime);
                                        ArrayList arrPipes2 = dbMgr.GetResultData(query, 0);
                                        if (arrPipes2 != null && arrPipes2.Count != 0)
                                            press = WebDBManager.GetFloatField(arrPipes2[0].ToString(), 0);
                                    }
                                }
                                else // 실시간
                                {
                                    query = string.Format("SELECT Pressure from pipehistory_{0}_{1:D2} where keytime = {2}", ids[n], i, nKeyTime);
                                    ArrayList arrPipes2 = dbMgr.GetResultData(query, 0);
                                    if (arrPipes2 != null && arrPipes2.Count != 0)
                                        press = WebDBManager.GetFloatField(arrPipes2[0].ToString(), 0);
                                }

                                listData.Add(new WriteFlowData(dt, fFlow, fTemp, fLevel, ids[n], press));
                                bAdd = true;

                                if (LogFileManager.Instance.AppClose)
                                    break;
                            }
                            if (!bAdd)
                            {
                                listData.Add(new WriteFlowData(dt, fFlow, fTemp, fLevel, 0, press));
                                bAdd = true;
                            }
                        }
                        else
                            listData.Add(new WriteFlowData(dt, fFlow, fTemp, fLevel, 0, press));

                        if (LogFileManager.Instance.AppClose)
                            break;
                    }

                    if (LogFileManager.Instance.AppClose)
                        break;

                    if (listData.Count != 0)
                    {
                        string fileName = string.Format("{4}\\{0}\\{1}\\{2}\\{3}_temp.dat", nTankID, dt.Year, dt.Month, dt.Day, m_strLogFolder);
                        WriteFile(fileName, listData, nTime);
                        bWrite = true;
                    }
                    listData.Clear();

                    // LastData
                    if (bWrite)
                    {
                        // 프로그램이 중간에 종료되었을 경우 파일의 무결성 체크를 위해 종료시간을 저장
                        lastFilePath = string.Format("{3}\\{0}\\{1}\\{2}\\LastData_temp.dat", nTankID, dt.Year, dt.Month, m_strLogFolder);

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

                    System.Threading.Thread.Sleep(100);
                }
                Console.WriteLine("end time tank " + nTankID + " : " + sw.Elapsed.ToString());
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                Console.WriteLine("DB Read Error : " + e.Message);
            }
        }

        private float GetPress(Dictionary<int, List<PipeData>> dic, int nPipeID, int nKeyTime)
        {
            float press = 0;

            foreach(KeyValuePair<int,List<PipeData>> kvp in dic)
            {
                if(kvp.Key == nPipeID)
                {
                    List<PipeData> datas = kvp.Value;
                    int cnt = datas.Count;
                    if (cnt == 0)
                        return 0;

                    press = datas[0].press;
                    for(int i=0; i<cnt; ++i)
                    {
                        if (datas[i].keyTime >= nKeyTime)
                            return press;

                        press = datas[i].press;
                    }
                }
            }
            return 0;
        }

        private List<int> GetPipeID(ArrayList list, DateTime dt)
        {
            List<int> ids = new List<int>();

            DateTime dtDefault = new DateTime();
            int cnt = list.Count;
            for (int i = 0; i < cnt; i+=3)
            {
                DateTime dt1 = WebDBManager.GetDateTimeField(list[i+1], dtDefault);
                if (dt1.Ticks <= dt.Ticks)
                {
                    DateTime dt2 = WebDBManager.GetDateTimeField(list[i + 2], dtDefault);
                    if (dt2.Ticks < dt1.Ticks || dt2.Ticks >= dt.Ticks)
                    {
                        ids.Add(WebDBManager.GetIntField(list[i].ToString(), 0));
                        if (ids.Count >= 2)
                            return ids;
                    }
                }
            }
            return ids;
        }

        private DateTime GetDateTime(int nYear, int nMonth, int nKeyTime)
        {
            int nDay = nKeyTime / 1000000;

            int nTemp = (nKeyTime - (nDay * 1000000));
            int nHour = nTemp / 10000;

            nTemp = (nTemp - (nHour * 10000));

            int nMin = nTemp / 100;
            int nSec = nTemp - (nMin * 100);

            return new DateTime(nYear, nMonth, nDay, nHour, nMin, nSec);
        }

        private void CreateDir(int id, int nYear, int nMonth)
        {
            string dir = string.Format("{3}\\{0}\\{1}\\{2}", id, nYear, nMonth, m_strLogFolder);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        private void WriteFile(string path, List<WriteFlowData> listData, long time)
        {
            FileMode mode = FileMode.Create;
            if (File.Exists(path) && time != 0)
                mode = FileMode.Append;

            // 파일공유 옵션 추가
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, mode, FileAccess.Write, FileShare.ReadWrite)))
            {
                foreach (WriteFlowData data in listData)
                {
                    writer.Write(data.dt.Ticks);
                    writer.Write(data.flow);
                    writer.Write(data.temp);
                    writer.Write(data.level);
                    writer.Write(data.pipeID);
                    writer.Write(data.press);
                }
            }
            LogFileManager.Instance.WriteLog("Write : " + path);
            lock (filePaths)
                filePaths.Add(path);
        } 

        public class WriteFlowData
        {
            public DateTime dt;
            public float temp;
            public float flow;
            public float level;
            public int pipeID;
            public float press;

            public WriteFlowData(DateTime t, float f, float te, float l, int id, float p)
            {
                dt = t;
                temp = te;
                flow = f;
                level = l;
                pipeID = id;
                press = p;
            }
        }

        public class PipeData
        {
            public float press;
            public int keyTime;

            public PipeData(float p, int kt)
            {
                press = p;
                keyTime = kt;
            }
        }

    }



}
