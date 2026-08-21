using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;

namespace SDMSAgent
{
    public partial class FormMain : Form
    {
        private WebDBManager m_dbMgr = null;
        private CommandHandling commandHandling = null;

        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        private string m_strDownloadURL = "";
        public string DownloadURL
        {
            get { return m_strDownloadURL; }
        }

        // 압축파일 다운받을 위치, upload.jsp 
        private string m_strUpdateSrcPath = "";
        public string UpdateSrcPath
        {
            get { return m_strUpdateSrcPath; }
        }

        // 압축풀어서 fileList 구성할 위치
        private string m_strUpdateTempPath = "";
        public string UpdateTempPath
        {
            get { return m_strUpdateTempPath; }
        }
        
        #region sdms update 할때 사용할 path (기존 SOPChecker에서 사용하던 로직)
        private string m_strSdmsUpdateSrc = "";
        public string SdmsUpdateSrc { get { return m_strSdmsUpdateSrc; } }
        private string m_strSdmsUpdateTrg = "";
        public string SdmsUpdateTrg { get { return m_strSdmsUpdateTrg; } }
        private string m_strSdmsUpdateTemp = "";
        public string SdmsUpdateTemp { get { return m_strSdmsUpdateTemp; } } 
        #endregion

        public FormMain()
        {
            InitializeComponent();
            
            WriteLog("Hi");

            m_instance = this;
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.notifyIcon1.Visible = true;
            this.Hide();
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
             
            DBConnect();

            m_strDownloadURL = SDMSAgent.Properties.Settings.Default.DownloadURL;
            if (m_strDownloadURL.Length == 0)
            {
                WriteLog("[ERROR] Download URL null"); 
            }
            m_strUpdateSrcPath = SDMSAgent.Properties.Settings.Default.UpdateSrcPath;
            if (m_strUpdateSrcPath.Length == 0)
            {
                WriteLog("[ERROR] Download URL null");
            }
            m_strUpdateTempPath = SDMSAgent.Properties.Settings.Default.UpdateTempPath;
            if (m_strUpdateTempPath.Length == 0)
            {
                WriteLog("[ERROR] Download URL null");
            }

            if (!Directory.Exists(m_strDownloadURL))
                Directory.CreateDirectory(m_strDownloadURL);

            if (!Directory.Exists(m_strUpdateSrcPath))
                Directory.CreateDirectory(m_strUpdateSrcPath);

            if (!Directory.Exists(m_strUpdateTempPath))
                Directory.CreateDirectory(m_strUpdateTempPath);

            //SDMS UPDATE PATH
            m_strSdmsUpdateSrc = SDMSAgent.Properties.Settings.Default.SDMS_update_src;
            m_strSdmsUpdateTrg = SDMSAgent.Properties.Settings.Default.SDMS_update_trg;
            m_strSdmsUpdateTemp = SDMSAgent.Properties.Settings.Default.SDMS_update_temp;

            if (!Directory.Exists(m_strSdmsUpdateSrc))
                Directory.CreateDirectory(m_strSdmsUpdateSrc);
            if (!Directory.Exists(m_strSdmsUpdateTrg))
                Directory.CreateDirectory(m_strSdmsUpdateTrg);
            if (!Directory.Exists(m_strSdmsUpdateTemp))
                Directory.CreateDirectory(m_strSdmsUpdateTemp);

            commandHandling = new CommandHandling(m_dbMgr);
            timer1.Start();
        }

        public static void WriteLog(string content)
        {
            string filePath = SDMSAgent.Properties.Settings.Default.LogFilePath;
            string dirPath = filePath.Substring(0, filePath.LastIndexOf(@"\"));
             
            DirectoryInfo di = new DirectoryInfo(dirPath);
            FileInfo fi = new FileInfo(filePath);

            try
            {
                if (!di.Exists) Directory.CreateDirectory(dirPath);
                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]    " + content);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filePath))
                    {
                        sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]    " + content);
                        sw.Close();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public static void WriteDrive(List<string> driveList)
        {
            string filePath = SDMSAgent.Properties.Settings.Default.GetDrivePath;
            string dirPath = filePath.Substring(0, filePath.LastIndexOf(@"\"));

            DirectoryInfo di = new DirectoryInfo(dirPath);
            FileInfo fi = new FileInfo(filePath);

            try
            {
                if (!di.Exists) Directory.CreateDirectory(dirPath);
                if (!fi.Exists)
                    File.CreateText(filePath);

                //{
                //    using (StreamWriter sw = new StreamWriter(filePath, false))
                //    {
                //        foreach (string item in driveList)
                //        {
                //            sw.WriteLine(item);
                //        }
                        
                //        sw.Close();
                //    }
                //}
                //else
                {
                    using (StreamWriter sw = new StreamWriter(filePath, false))
                    {
                        foreach (string item in driveList)
                        {
                            sw.WriteLine(item);
                        }

                        sw.Close();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void DBConnect()
        {
            int nSiteID = ReadSiteID();
            if (nSiteID > 0) 
                m_dbMgr = new WebDBManager(nSiteID); 
            else
            {
                m_dbMgr = new WebDBManager(0);
                m_dbMgr.WebServerURL = SDMSAgent.Properties.Settings.Default.WebServerUrl;
                m_dbMgr.DatabaseHost = SDMSAgent.Properties.Settings.Default.DatabaseHost;
                m_dbMgr.DatabaseName = SDMSAgent.Properties.Settings.Default.DatabaseName;
                m_dbMgr.DatabasePort = SDMSAgent.Properties.Settings.Default.DatabasePort;
                string dbType = SDMSAgent.Properties.Settings.Default.DataBaseType;
                if (dbType.Trim().ToUpper() == "MSSQL")
                    m_dbMgr.DatabaseType = WebDBManager.DBType.sqlserver;
                else if (dbType.Trim().ToUpper() == "MYSQL")
                    m_dbMgr.DatabaseType = WebDBManager.DBType.mysql;
            }
        }

        private int ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID != null && szSiteID.Length > 0)
            {
                int nSiteId = 1;
                if (int.TryParse(szSiteID, out nSiteId))
                    return nSiteId; 
            }
            return -1;  
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ReadCommand();
        }

        private void ReadCommand()
        {
            string strSQL = "Select ID, Command, TimeStamp, SearchPath, IsStop, IsStopService, StopName, IsUpdate, UpdateName, IsStart, IsStartService, StartName from SDMSCommand";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
            {
                WriteLog("query result is null, check db connect");
                return;
            }

            if (arrResult.Count == 0)
                return;

            bool needClose = false;
            int nResultCount = arrResult.Count;

            if (nResultCount > 0)
                WriteLog("ReadCommand ResultCount : " + nResultCount);

            for (int i = 0; i < nResultCount; i += 12)
            {    
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nCommand = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                VariousData<DateTime> timeStamp = WebDBManager.GetDateTimeField(arrResult[i + 2]);
                string strSearchPath = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");                
                int nIsStop = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nIsStopService = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                string strStopName = WebDBManager.GetStringField(arrResult[i + 6].ToString(), ""); 
                int nIsUpdate = WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                string strUpdateName = WebDBManager.GetStringField(arrResult[i + 8].ToString(), ""); 
                int nIsStart = WebDBManager.GetIntField(arrResult[i + 9].ToString(), -1);
                int nIsStartService = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);
                string strStartName = WebDBManager.GetStringField(arrResult[i + 11].ToString(), ""); 
                 
                CommandItem cmd = new CommandItem();
                cmd.ID = nID;
                cmd.CmdType = (CommandType)nCommand;
                cmd.TimeStamp = timeStamp.Data;
                cmd.SearchPath = strSearchPath;                
                cmd.IsStop = (nIsStop == 0) ? false : true;
                cmd.IsStopService = (nIsStopService == 0) ? false : true;
                cmd.StopName = strStopName;
                cmd.IsUpdate = (nIsUpdate == 0) ? false : true;
                cmd.UpdateName = strUpdateName;
                cmd.IsStart = (nIsStart == 0) ? false : true;
                cmd.IsStartService = (nIsStartService == 0) ? false : true;
                cmd.StartName = strStartName;

                if (commandHandling.Execute(cmd))
                {
                    // Agent 업데이트 할때만 
                    if (commandHandling.IsNeedClose)
                    {
                        needClose = true;
                        break;
                    }
                } 
            }

            if (needClose)
            {
                timer1.Stop();
                this.Close();

                WriteLog("Agent Reboot");
            }

            DeleteFile();
        }

        private void DeleteFile()
        {
            string strSQL = "Select ID, SearchPath from SDMSCommandHistory Where Result=2 And Command = " + (int)CommandType.DOWNLOAD;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                //VariousData<DateTime> dtTimeStamp = WebDBManager.GetDateTimeField(arrResult[i + 1]);
                string strSearchPath = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");

                //string strParam = dtTimeStamp.Data.ToString("yyyyMMddHHmmss") + "_" + (int)CommandType.DOWNLOAD;
                 
                commandHandling.DeleteFile(strSearchPath);

                m_dbMgr.GetResultData("UPDATE SDMSCommandHistory SET Result=3 WHERE ID=" + nID, 0);
            } 
        }

        bool isClose = false;
        private void tsMenuClose_Click(object sender, EventArgs e)
        {
            isClose = true;
            this.Close();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isClose)
            {
                e.Cancel = true;
                this.notifyIcon1.Visible = true;
                this.Hide();
            }
        }
    }
}
