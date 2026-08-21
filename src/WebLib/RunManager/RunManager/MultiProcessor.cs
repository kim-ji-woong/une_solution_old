using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace RunManager
{
    public class MultiProcessor
    {
        private bool m_isProcessing = false;
        // 현재 실행중인 프로세스가 들어있는 폴더들
        private ArrayList m_arrProcessingFolder = new ArrayList();
        // 현재 실행중인 Process List
        // Process Full Path, db의 LIB_CONTROLLER idx
        //private Dictionary<string, int> m_dicCurrentProcess = new Dictionary<string, int>();
        // 현재 실행중인 Process List
        // Process ID, Process 부가 정보
        private Dictionary<int, ProcessInfo> m_dicCurrentProcess = new Dictionary<int, ProcessInfo>();
        private string m_strBasicFolderName = "ControllerAgent";
        private System.Diagnostics.Process outProcess = null;

        public MultiProcessor(System.Diagnostics.Process outProcess)
        {
            this.outProcess = outProcess;
        }

        public bool IsProcessing
        {
            get { return m_isProcessing; }
        }

        public void Run(DBManagerMySQL dbMgr)
        {
            try
            {
                m_isProcessing = true;

                CheckRunStatus(dbMgr);
                ProcessQuest(dbMgr);
                
                m_isProcessing = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}\r\n예외가 발생하였습니다.\r\n원인 제거후 프로그램을 재실행 하여 주세요", e.Message));
                //dbMgr.CloseConnection();
            }
        }

        private int GetPID(string strFolder)
        {
            string strPIDFilePath = strFolder + "\\PID.txt";
            System.IO.StreamReader SRead = new System.IO.StreamReader(strPIDFilePath, System.Text.Encoding.UTF8);

            string strLine = SRead.ReadLine();
            SRead.Close();

            try
            {
                int nPID = int.Parse(strLine);
                return nPID;
            }
            catch (Exception)
            {
            }

            return 0;
        }

        // 실행중으로 db에 표시된 Process들이 현재도 계속 실행중인지 여부를 검사하여 실행이 끝났을 경우
        // db에 해당 데이터를 업데이트 한다.
        private void CheckRunStatus(DBManagerMySQL dbMgr)
        {
            string strSQL = "select idx, dir from LIB_CONTROLLER where CONTROLLER = 2";

            //System.Data.SqlClient.SqlDataReader reader;
            MySql.Data.MySqlClient.MySqlDataReader reader;
            dbMgr.ReadDB(strSQL, null, out reader);

            if (reader == null)
                return;

            while (reader.Read())
            {
                int nID = dbMgr.GetField<int>(reader[0], 0);
                string strFolder = dbMgr.GetStringField(reader[1], "");
                strFolder = strFolder.Replace('/', '\\');

                int nPID = GetPID(strFolder);
                if (nPID == 0)
                    continue;

                m_dicCurrentProcess[nPID] = new ProcessInfo(nID, strFolder + "\\" + dbMgr.OutExePath);
                //m_dicCurrentProcess[strFolder + "\\" + dbMgr.OutExePath] = nID;
            }

            reader.Close();

            ArrayList arrRemoveIDs = null;
            int nRemoveCount = RunCheckProcess(dbMgr.OutExeName, m_dicCurrentProcess, out arrRemoveIDs);
            if (nRemoveCount == 0)
                return;

            string strIDs = "(" + ((int)arrRemoveIDs[0]).ToString();

            for (int i = 1; i < nRemoveCount; i++)
            {
                strIDs += ", " + ((int)arrRemoveIDs[i]).ToString();
            }

            strIDs += ")";

            strSQL = "update LIB_CONTROLLER set CONTROLLER = 0 where idx in " + strIDs;
            dbMgr.Execute(strSQL);
        }

        private int RunCheckProcess(string strProcessName, Dictionary<int, ProcessInfo> dicProcess, out ArrayList arrRemoveIDs)
        //private int RunCheckProcess(string strProcessName, Dictionary<string, int> dicProcess, ArrayList arrRemoveIDs)
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            arrRemoveIDs = new ArrayList();
            ArrayList arrRemoveList = new ArrayList();

            foreach (KeyValuePair<int, ProcessInfo> pair in dicProcess)
            {
                arrRemoveList.Add(pair.Key);
                arrRemoveIDs.Add(pair.Value.DBIndex);
            }
            
            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName == strProcessName)
                {
                    int nPID = process.Id;

                    if (dicProcess.ContainsKey(nPID))
                    {
                        int nIndex = arrRemoveList.IndexOf(nPID);

                        if (nIndex >= 0)
                        {
                            arrRemoveList.RemoveAt(nIndex);
                            arrRemoveIDs.RemoveAt(nIndex);
                        }
                    }

                    /*string strFullPath = process.Modules[0].FileName;

                    if (dicProcess.ContainsKey(strFullPath))
                        arrRunningList.Add(dicProcess[strFullPath]);*/
                }
            }

            foreach (int nPID in arrRemoveList)
            {
                dicProcess.Remove(nPID);
            }

            /*arrRunningList.Sort();

            ArrayList arrRemoveString = new ArrayList();

            foreach (KeyValuePair<string, int> pair in dicProcess)
            {
                if (arrRunningList.BinarySearch(pair.Value) < 0)
                {
                    arrRemoveIDs.Add(pair.Value);
                    arrRemoveString.Add(pair.Key);
                }
            }

            foreach (string strPath in arrRemoveString)
            {
                dicProcess.Remove(strPath);
            }*/

            return arrRemoveIDs.Count;
        }

        // Return 값 : 새로 실행시킨 exe의 개수
        private int ProcessQuest(DBManagerMySQL dbMgr)
        {
            string strSQL = "select idx, msg from LIB_CONTROLLER where CONTROLLER = 1";

            //System.Data.SqlClient.SqlDataReader reader;
            MySql.Data.MySqlClient.MySqlDataReader reader;
            dbMgr.ReadDB(strSQL, null, out reader);

            if (reader == null)
                return 0;

            // idx, msg
            Dictionary<int, string> dicRequest = new Dictionary<int, string>();

            while (reader.Read())
            {
                int nID = dbMgr.GetField<int>(reader[0], 0);
                string strMsg = dbMgr.GetStringField(reader[1], "");

                dicRequest[nID] = strMsg;
            }

            reader.Close();

            ArrayList arrFolders = GetNewFolderList(dicRequest.Count);
            if (arrFolders == null)
                return 0;

            if (m_dicCurrentProcess.Count > 0)
                arrFolders = GetNewFolderList(dicRequest.Count);

            MakeFolders(arrFolders);
            Dictionary<string, string> dicParamFiles = MakeParamFiles(dbMgr, Application.StartupPath + "\\");
            // 1. 폴더가 존재하지 않으면 새로 만든다.
            // 2. input.csv 파일을 생성한다.
            // 3. dicRequest value를 strMsg에서 실행 모듈의 FullPath로 바꾼다.
            //ArrangeDatas(arrFolders, Application.StartupPath + "\\" + dbMgr.OutExePath, "\\" + dbMgr.OutExePath, dicRequest);
            ArrangeDatas(arrFolders, dicParamFiles, "\\" + dbMgr.OutExePath, dicRequest);


            return RunModules(dbMgr, dicRequest, dbMgr.OutExeName);
        }

        // Return 값 : srcPath, trgFile
        private Dictionary<string, string> MakeParamFiles(DBManagerMySQL dbMgr, string strSrcPath)
        {
            Dictionary<string, string> dicParamFiles = new Dictionary<string, string>();

            // 실행 모듈
            dicParamFiles[strSrcPath + dbMgr.OutExePath] = "\\" + dbMgr.OutExePath;

            foreach (string strParamFile in dbMgr.ParamFiles)
            {
                dicParamFiles[strSrcPath + strParamFile] = "\\" + strParamFile;
            }

            return dicParamFiles;
        }

        private int RunModules(DBManagerMySQL dbMgr, Dictionary<int, string> dicRequests, string strProcessName)
        {
            if (outProcess == null)
                return 0;

            int nRunCount = 0;

            foreach (KeyValuePair<int, string> pair in dicRequests)
            {
                outProcess.StartInfo.FileName = pair.Value;

                // 프로세스 시작위치 지정
                int nIndex2 = pair.Value.LastIndexOf('\\');
                if (nIndex2 >= 0)
                    outProcess.StartInfo.WorkingDirectory = pair.Value.Substring(0, nIndex2);

                if (outProcess.Start())
                {
                    string strPIDFilePath = pair.Value.Substring(0, pair.Value.LastIndexOf('\\')) + "\\PID.txt";
                    System.IO.StreamWriter SWrite = new System.IO.StreamWriter(strPIDFilePath, false, System.Text.Encoding.UTF8);

                    // PID.txt에 ProcessID를 저장시킨다.
                    SWrite.WriteLine(outProcess.Id);
                    SWrite.Close();

                    m_dicCurrentProcess[outProcess.Id] = new ProcessInfo(pair.Key, pair.Value);

                    int nIndex = pair.Value.LastIndexOf('\\');

                    if (nIndex >= 0)
                    {
                        string strFolder = pair.Value.Substring(0, nIndex);
                        strFolder = strFolder.Replace('\\', '/');

                        string strSQL = string.Format("Update LIB_CONTROLLER set CONTROLLER = 2, dir = '{0}' where idx = {1}",
                            strFolder, pair.Key);

                        dbMgr.Execute(strSQL);
                    }

                    nRunCount++;
                }
            }

            return nRunCount;

            /*System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            int nOriginCount = dicRequests.Count;
            ArrayList arrRunningList = new ArrayList();

            for (int i = 0; i < 10; i++)
            {
                foreach (System.Diagnostics.Process process in processList)
                {
                    if (process.ProcessName == strProcessName)
                    {
                        string strFullPath = process.Modules[0].FileName;
                        int nID = FindValue(dicRequests, strFullPath);

                        if (nID >= 0)
                        {
                            dicRequests.Remove(nID);
                            m_dicCurrentProcess[strFullPath] = nID;

                            int nIndex = strFullPath.LastIndexOf('\\');

                            if (nIndex >= 0)
                            {
                                string strFolder = strFullPath.Substring(0, nIndex);

                                string strSQL = string.Format("Update LIB_CONTROLLER set CONTROLLER = 2, dir = '{0}' where idx = {1}",
                                    strFolder, nID);

                                dbMgr.Execute(strSQL);
                            }
                        }
                    }
                }

                // 실행 여부가 완전히 파악될때까지 10초간 기다림
                if (dicRequests.Count == 0)
                    break;

                System.Threading.Thread.Sleep(1000);
                /////////////////////////////////////////////
            }

            return nOriginCount - dicRequests.Count;*/
        }

        // Return 값 : Key
        private int FindValue(Dictionary<int, string> dicRequests, string strValue)
        {
            foreach (KeyValuePair<int, string> pair in dicRequests)
            {
                if (pair.Value == strValue)
                    return pair.Key;
            }

            return -1;
        }

        // 1. 폴더가 존재하지 않으면 새로 만든다.
        // 2. input.csv 파일을 생성한다.
        // 3. dicRequest value를 strMsg에서 실행 모듈의 FullPath로 바꾼다.
        //private void ArrangeDatas(ArrayList arrFolders, string strSrcModulePath, string strTrgModulePath, Dictionary<int, string> dicRequests)
        private void ArrangeDatas(ArrayList arrFolders, Dictionary<string, string> dicParamFiles, string strTrgModulePath, Dictionary<int, string> dicRequests)
        {
            int i = 0;
            Dictionary<int, string> dicTemp = new Dictionary<int, string>();

            foreach (KeyValuePair<int, string> pair in dicRequests)
            {
                string strFolder = (string)arrFolders[i++];

                foreach (KeyValuePair<string, string> param in dicParamFiles)
                {
                    try
                    {
                        System.IO.File.Copy(param.Key, strFolder + param.Value);
                    }
                    catch (System.IO.IOException)
                    {
                        // 이미 파일이 존재하는 경우
                    }
                }
                /*try
                {
                    System.IO.File.Copy(strSrcModulePath, strFolder + strTrgModulePath);
                }
                catch (System.IO.IOException)
                {
                    // 이미 파일이 존재하는 경우
                }*/

                string strInputFilePath = strFolder + "\\input.csv";
                System.IO.StreamWriter SWrite = new System.IO.StreamWriter(strInputFilePath, false, System.Text.Encoding.ASCII);

                //SWrite.WriteLine(pair.Value);
                SWrite.Write(pair.Value);
                SWrite.Close();

                dicTemp[pair.Key] = strFolder + strTrgModulePath;
            }

            foreach (KeyValuePair<int, string> pair in dicTemp)
            {
                dicRequests[pair.Key] = pair.Value;
            }
        }

        private void MakeFolders(ArrayList arrFolders)
        {
            foreach (string strFolder in arrFolders)
            {
                if (!System.IO.Directory.Exists(strFolder))
                {
                    int nIndex = strFolder.LastIndexOf('\\');

                    if (nIndex < 0)
                        System.IO.Directory.CreateDirectory(strFolder);
                    else
                    {
                        string strSubFolder = strFolder.Substring(nIndex + 1);
                        System.IO.Directory.CreateDirectory(strSubFolder);
                    }
                }
            }
        }

        // nRequestCount 만큼의 새 Folder 이름들을 ArrayList에 담아 리턴한다.
        private ArrayList GetNewFolderList(int nRequestCount)
        {
            if (nRequestCount == 0)
                return null;

            ArrayList arrFolders = new ArrayList();
            ArrayList arrNumber = new ArrayList();

            foreach (KeyValuePair<int, ProcessInfo> pair in m_dicCurrentProcess)
            {
                ProcessInfo process = pair.Value;

                int nIndex = process.FilePath.LastIndexOf('\\');
                if (nIndex < 0)
                    continue;

                string strPath = process.FilePath.Substring(0, nIndex);
                
                int n_Index = strPath.LastIndexOf('_');
                string strNumber = strPath.Substring(n_Index + 1);

                try
                {
                    int num = int.Parse(strNumber);
                    arrNumber.Add(num);
                }
                catch (Exception)
                {
                }
            }
            //foreach (KeyValuePair<string, int> pair in m_dicCurrentProcess)
            //{
            //    int nIndex = pair.Key.LastIndexOf('\\');
            //    if (nIndex < 0)
            //        continue;

            //    string strPath = pair.Key.Substring(0, nIndex);
            //    nIndex = strPath.LastIndexOf('\\');

            //    if (nIndex >= 0)
            //        strPath = strPath.Substring(0, nIndex);

            //    int n_Index = strPath.LastIndexOf('_');
            //    string strNumber = strPath.Substring(n_Index + 1);

            //    try
            //    {
            //        int num = int.Parse(strNumber);
            //        arrNumber.Add(num);
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}

            arrNumber.Sort();

            int nNumberCount = arrNumber.Count;

            if (nNumberCount == 0)
                MakeFolders(arrFolders, 0, nRequestCount, ref nRequestCount);
            else
            {
                for (int i = 0; i < nNumberCount && nRequestCount > 0; i++)
                {
                    if (i == nNumberCount - 1)
                    {
                        MakeFolders(arrFolders, (int)arrNumber[i] + 1, (int)arrNumber[i] + 1 + nRequestCount, ref nRequestCount);
                        //for (int j = (int)arrNumber[i] + 1; nRequestCount > 0; j++, nRequestCount--)
                        //{
                        //    string strFullPath = string.Format("{0}\\{1}_{2}",
                        //        Application.StartupPath, m_strBasicFolderName, j);
                        //    arrFolders.Add(strFullPath);
                        //}

                        break;
                    }
                    else
                    {
                        MakeFolders(arrFolders, (int)arrNumber[i] + 1, (int)arrNumber[i + 1], ref nRequestCount);
                        //for (int j = (int)arrNumber[i] + 1; j < (int)arrNumber[i + 1] && nRequestCount > 0; j++, nRequestCount--)
                        //{
                        //    string strFullPath = string.Format("{0}\\{1}_{2}",
                        //        Application.StartupPath, m_strBasicFolderName, j);
                        //    arrFolders.Add(strFullPath);
                        //}
                    }
                }
            }

            return arrFolders;
        }

        private void MakeFolders(ArrayList arrFolders, int nBeginIndex, int nEndIndex, ref int nRequestCount)
        {
            for (int j = nBeginIndex; j < nEndIndex && nRequestCount > 0; j++, nRequestCount--)
            {
                string strFullPath = string.Format("{0}\\{1}_{2}",
                    Application.StartupPath, m_strBasicFolderName, j);
                arrFolders.Add(strFullPath);
            }
        }
    }

    public class ProcessInfo
    {
        private int m_nDBIndex = -1;
        private string m_strFilePath = "";

        public ProcessInfo()
        {
        }

        public ProcessInfo(int nDBIndex, string strFilePath)
        {
            m_nDBIndex = nDBIndex;
            m_strFilePath = strFilePath;
        }

        public int DBIndex
        {
            get { return m_nDBIndex; }
            set { m_nDBIndex = value; }
        }

        public string FilePath
        {
            get { return m_strFilePath; }
            set { m_strFilePath = value; }
        }
    }
}
