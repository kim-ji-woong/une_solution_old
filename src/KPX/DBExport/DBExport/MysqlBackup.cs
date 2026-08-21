using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using MySql.Data.MySqlClient;

namespace DBExport
{
    public partial class MysqlBackup : Form
    {
        Timer timer = null;
        WebDBManager dbMgr = null;

        public MysqlBackup()
        {
            InitializeComponent();
            dbMgr = new WebDBManager(500);
            dbMgr.DatabaseHost = "127.0.0.1";

            SetSystemLog("[" +DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] MysqlBackup Start! ");

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.notifyIcon1.Visible = true;
            this.Hide();
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;

            //timer = new Timer();
            //timer.Interval = 86400000; //86400000; // 하루
            //timer.Tick += timer_Tick;
            //timer.Start();
            //timer_Tick(null, null);

            this.Load += MysqlBackup_Load; 
        }

        void MysqlBackup_Load(object sender, EventArgs e)
        {
            LoadTables();
            MakeSql();  

            Application.Exit();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            // 최근 일주일 데이터만 보관
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(DBExport.Properties.Settings.Default.outputPath);
                foreach (FileInfo fileInfo in dirInfo.GetFiles())
                {
                    if (!fileInfo.Name.Contains(".sql") && !fileInfo.Name.Contains(".zip"))
                        continue;

                    string strDate = fileInfo.Name.Replace(".sql", "").Replace(".zip", "");
                    int nYear = Convert.ToInt32(strDate.Substring(0, 4));
                    int nMonth = Convert.ToInt32(strDate.Substring(4, 2));
                    int nDay = Convert.ToInt32(strDate.Substring(6, 2));

                    DateTime date = new DateTime(nYear, nMonth, nDay);
                    DateTime now = DateTime.Now;
                    TimeSpan span = now - date;

                    if (span.Days > 7)
                        File.Delete(fileInfo.FullName);
                }
            }
            catch (Exception ex)
            {
                SetSystemLog("timer_Tick() " + ex.Message);
            }
            
            // 전체
            string ymd = DateTime.Now.ToString("yyyyMMdd") + ".sql";
            string downPath = DBExport.Properties.Settings.Default.downloadLocalPath + ymd;
            if (ProcCmd(downPath))
            {
                string zipName = DBExport.Properties.Settings.Default.outputPath + ymd.Replace(".sql", ".zip");
                string outputPath = DBExport.Properties.Settings.Default.outputPath + ymd;
                Compress(zipName, downPath, ymd);
            }

            // 테이블 단위
            //LoadTables();
            //string ymd = DateTime.Now.ToString("yyyyMMdd");
             
            //DirectoryInfo dirInfo2 = new DirectoryInfo(@"C:\KpxTest\20180731\"/*DBExport.Properties.Settings.Default.downloadLocalPath + ymd*/);
            //if (!dirInfo2.Exists)
            //    dirInfo2.Create();
            
            //string downPath = dirInfo2.FullName;
            //if (ProcCmd(downPath))
            //{
            //    string zipName = DBExport.Properties.Settings.Default.outputPath + ymd.Replace(".sql", ".zip");
            //    string outputPath = DBExport.Properties.Settings.Default.outputPath + ymd;
            //    Compress(zipName, downPath, ymd);
            //}
        }

        #region MySqlBackup
        private void DBConnectBackup()
        {
            try
            {
                string constring = DBExport.Properties.Settings.Default.conStr;

                string fileName = DateTime.Now.ToString("yyyyMMdd") + ".sql";
                string filePath = DBExport.Properties.Settings.Default.outputPath + fileName;

                if (!System.IO.File.Exists(filePath))
                {
                    using (MySqlConnection conn = new MySqlConnection(constring))
                    {
                        conn.Open();
                        if (conn.State == ConnectionState.Open)
                        {
                            using (MySqlCommand cmd = new MySqlCommand())
                            {
                                using (MySqlBackup mb = new MySqlBackup(cmd))
                                {
                                    cmd.Connection = conn;
                                    mb.ExportToFile(filePath);
                                    conn.Close();
                                }
                            }

                            string zipName = filePath.Replace(".sql", ".zip");
                            Compress(zipName, filePath, fileName);
                            //FtpUpload(fileName.Replace(".sql", ".zip"));                  
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetSystemLog("DBConnectBackup() : " + ex.Message);
            }
        } 
        #endregion
         
        #region 압축
        public bool Compress(string zipPath, string filePath, string fileName)
        {
            try
            {
                SetSystemLog("begin compress : " + zipPath);

                FileStream fsOut = File.Create(zipPath);
                ZipOutputStream zipStream = new ZipOutputStream(fsOut);

                zipStream.SetLevel(9);
                 
                FileInfo fi = new FileInfo(filePath);

                string entryName = ZipEntry.CleanName(fileName);
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime;
                newEntry.Size = fi.Length;
                zipStream.PutNextEntry(newEntry);

                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filePath))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
                 
                zipStream.IsStreamOwner = true;
                zipStream.Close();

                File.Delete(filePath);

                SetSystemLog("end compress : " + zipPath);

                return true;
            }
            catch (Exception ex)
            {
                SetSystemLog("Compress() : " + ex.Message);
                return false;
            }
        }   
        #endregion

        #region FTP
        private bool FtpUpload(string zipFileName)
        {
            string file2 = DBExport.Properties.Settings.Default.outputPath + @"\" + zipFileName;
            string ftpPath = DBExport.Properties.Settings.Default.ftpPath + zipFileName;
            string userID = "ftpune";
            string pwd = "9449966Ab";

            try
            {
                Uri targetFileUri = new Uri(ftpPath);
                FtpWebRequest ftpWebRequest = WebRequest.Create(targetFileUri) as FtpWebRequest;
                ftpWebRequest.Credentials = new NetworkCredential(userID, pwd);
                ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;

                FileStream sourceFileStream = new FileStream(file2, FileMode.Open, FileAccess.Read);
                Stream targetStream = ftpWebRequest.GetRequestStream();

                byte[] bufferByteArray = new byte[1024];

                while (true)
                {
                    int byteCount = sourceFileStream.Read(bufferByteArray, 0, bufferByteArray.Length);

                    if (byteCount == 0)
                        break;

                    targetStream.Write(bufferByteArray, 0, byteCount);
                }

                targetStream.Close();
                sourceFileStream.Close();
            }
            catch
            {
                return false;
            }
            return true;
        } 
        #endregion

        private List<string> tables = new List<string>();
        private void LoadTables()
        {
            tables.Clear();
            ArrayList arrResult = dbMgr.GetResultData("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'KPX'", 0);
            if (arrResult == null) return;

            for (int i = 0; i < arrResult.Count; i++)
            {
                string tableName = DBUtility.WebDBManager.GetStringField(arrResult[i]);
                if (!tableName.Contains("pipehistory") && !tableName.Contains("flowhistory"))
                    tables.Add(tableName);  
            } 
        }

        private void MakeSql()
        {
            try
            {
                StringBuilder sb = new StringBuilder(); 
                foreach (string strTableName in tables)
                { 
                    sb.AppendLine("DELETE FROM " + strTableName + ";");

                    ArrayList arrColumnInfo = dbMgr.GetResultData(
                        "SELECT column_name, data_type FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'KPX' AND table_name='" + strTableName + "'", 0);

                    if (arrColumnInfo == null || arrColumnInfo.Count == 0)
                        continue;

                    Dictionary<string, string> dicTableInfo = new Dictionary<string, string>();

                    for (int i = 0; i < arrColumnInfo.Count; i += 2)
                    {
                        string strColumnName = DBUtility.WebDBManager.GetStringField(arrColumnInfo[i]);
                        string strColumnType = DBUtility.WebDBManager.GetStringField(arrColumnInfo[i + 1]);

                        dicTableInfo.Add(strColumnName, strColumnType);
                    }

                    int nColumnCount = dicTableInfo.Count;
                    string columns = string.Join(", ", dicTableInfo.Keys);

                    ArrayList arrResult = dbMgr.GetResultData("SELECT " + columns + " FROM " + strTableName, 0);
                    if (arrResult == null || arrResult.Count == 0) 
                        continue;

                    for (int i = 0; i < arrResult.Count; i += nColumnCount)
                    {
                        int iCnt = 0;
                        if (i == 0)
                            sb.Append("INSERT INTO " + strTableName + " VALUES ");
                        else
                            sb.Append(",");

                        sb.Append("(");
                        for (int j = 0; j < nColumnCount; j++)
                        {
                            if (j > 0)
                                sb.Append(",");
                            string dataType = dicTableInfo.Values.ElementAt(iCnt);
                            object value = arrResult[i + iCnt];

                            if (value.ToString() == "null") 
                                sb.Append("NULL");
                            else
                            {
                                if (dataType.ToUpper() == "INT" || dataType.ToUpper() == "FLOAT")
                                    sb.Append(value);
                                else
                                    sb.Append("'" + value + "'");
                            }
                            iCnt++;
                        }
                        sb.Append(")");
                    }

                    sb.AppendLine(";");
                }

                if (sb.Length > 0)
                {
                    FileStream fs = new FileStream(DBExport.Properties.Settings.Default.writePath + DateTime.Now.ToString("yyyy-MM-dd") + ".sql", FileMode.Create, FileAccess.Write);
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(sb.ToString());

                    }
                    fs.Close();
                }

                if (Directory.Exists(DBExport.Properties.Settings.Default.writePath))
                {
                    DateTime dtNow = DateTime.Now;

                    List<string> delFiles = new List<string>();
                    DirectoryInfo dir = new DirectoryInfo(DBExport.Properties.Settings.Default.writePath);
                    foreach (FileInfo item in dir.GetFiles())
                    {
                        DateTime dtFile = new DateTime();                        
                        if (DateTime.TryParse(item.Name.Replace(".sql", ""), out dtFile) && item.Extension == ".sql")
                        {
                            //DateTime dtFile = DateTime.Parse(item.Name.Replace(".sql", ""));
                            if ((dtNow - dtFile).TotalDays > 180)
                                delFiles.Add(item.FullName); 
                        }
                    }

                    foreach (string item in delFiles)
                    {
                        File.Delete(item);
                    }
                }

            }
            catch (Exception ex)
            {
                SetSystemLog(ex.Message);
            }
        }
         
        private bool ProcCmd(string path)
        {
            try
            {
                ProcessStartInfo procInfo = new ProcessStartInfo();
                procInfo.FileName = "cmd";
                procInfo.CreateNoWindow = true;
                procInfo.UseShellExecute = false;
                procInfo.WorkingDirectory = DBExport.Properties.Settings.Default.MySqlSetupPath;

                procInfo.RedirectStandardOutput = true;
                procInfo.RedirectStandardInput = true;
                procInfo.RedirectStandardError = true;
                procInfo.WindowStyle = ProcessWindowStyle.Hidden;

                // 전체
                using (Process proc = Process.Start(procInfo))
                {
                    SetSystemLog("begin dump : " + path);

                    proc.EnableRaisingEvents = true;
                    proc.Exited += proc_Exited;
                    proc.StandardInput.Write("mysqldump -u sa -p9449966Ab kpx --no-autocommit=1 --single-transaction=1> " + path + Environment.NewLine);

                    proc.StandardInput.Flush();
                    proc.StandardInput.Close();
                    proc.WaitForExit();
                    proc.Close();

                    SetSystemLog("end dump : ");

                    return true;
                }                

                // 테이블 단위 
                //SetSystemLog("begin dump : " + path);
                
                //using (Process proc = Process.Start(procInfo))
                //{  
                //    proc.EnableRaisingEvents = true;
                //    proc.StandardInput.AutoFlush = true;
                //    proc.Exited += proc_Exited;

                //    foreach (string item in tables)
                //    {
                //        string name = item + ".sql";
                //        proc.StandardInput.Write("mysqldump -u sa -p9449966Ab kpx " + item + " --no-autocommit=1 --single-transaction=1> " + path + name + Environment.NewLine);
                //        proc.StandardInput.Flush();
                //    }
                //    //proc.WaitForExit();
                //    proc.StandardInput.Close();
                //    proc.Close();
                //} 
                //SetSystemLog("end dump : ");
                //return true;
            }
            catch (Exception ex)
            {
                SetSystemLog(ex.Message);
                return false;
            }
        }

        void proc_Exited(object sender, EventArgs e)
        {
            
        }
         
        public void SetSystemLog(string content)
        {
            string filePath = DBExport.Properties.Settings.Default.writePath + "DBExport.log";
            string dirPath = DBExport.Properties.Settings.Default.writePath;

            DirectoryInfo di = new DirectoryInfo(dirPath);
            FileInfo fi = new FileInfo(filePath);

            try
            {
                if (!di.Exists) Directory.CreateDirectory(dirPath);
                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        sw.WriteLine("[MysqlBackup " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]    " + content);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filePath))
                    {
                        sw.WriteLine("[MysqlBackup " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]    " + content);
                        sw.Close();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Dispose();
            this.Close();
        } 
    }
}
