using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataImportNServerReboot
{
    public partial class Form1 : Form
    {
        private string m_strDownloadServerURL = "";
        private string m_strDownloadServerPath = "";
        private string m_strLocalCopyPath = "";
        private string[] m_processList = null;
        private string m_strDatabaseName = "";
        private string m_strMdfFilePath = "";

        public Form1()
        {
            InitializeComponent();
            
            m_strDownloadServerURL = DataImportNServerReboot.Properties.Settings.Default.DownloadServerURL;
            m_strDownloadServerPath = DataImportNServerReboot.Properties.Settings.Default.DownloadServerPath;
            m_strLocalCopyPath = DataImportNServerReboot.Properties.Settings.Default.LocalCopyPath;
            m_processList = DataImportNServerReboot.Properties.Settings.Default.RebootProcessList.Split(',');
            m_strDatabaseName = DataImportNServerReboot.Properties.Settings.Default.DatabaseName;
            m_strMdfFilePath = DataImportNServerReboot.Properties.Settings.Default.mdfFilePath;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 1. 통합 서버에 있는 Backup 파일 다운로드
            try
            {
                string fileName = DateTime.Now.ToString("yyyyMMdd") + ".bak";
                bool isDownload = Download(fileName);
                if (!isDownload)
                    throw new ApplicationException("1. Download 실패 " + fileName);

                // 2. 실행중인 서비스 중지
                if (!StopService())
                    throw new ApplicationException("2. 실행중인 서비스 중지");

                System.Threading.Thread.Sleep(5000);

                // 3. 다운로드한 .bak로 Database Import 
                string strResultMessage = "";
                if (!DataImport(fileName, out strResultMessage))
                {

                    throw new ApplicationException("3. 다운로드한 .bak로 Database Import ");
                }

                // 4. 중지한 서비스 실행
                if (!StartService())
                    throw new ApplicationException("4. 중지한 서비스 실행");

                Logger.Instance.Write("[INFO] Complete");

                
            }
            catch (Exception ex)
            {
                Logger.Instance.Write("[Error] " + ex.Message);
            }
            Application.Exit();
        }

        private bool DataImport(string fileName, out string strResultMessage)
        {
            DBUtility2.WebDBManager webDBManager = new DBUtility2.WebDBManager();
            webDBManager.DatabaseName = m_strDatabaseName;
            webDBManager.DatabaseType = DBUtility2.WebDBManager.DBType.sqlserver;
            webDBManager.WebServerURL = "http://127.0.0.1";

            strResultMessage = "";
            try
            {
                webDBManager.GetResultData("ALTER DATABASE BLD_205 Set single_user with rollback immediate");

                ProcessStartInfo procInfo = new ProcessStartInfo();
                Process proc = new Process();

                procInfo.FileName = "cmd.exe";
                procInfo.CreateNoWindow = false;
                procInfo.UseShellExecute = false;

                procInfo.RedirectStandardInput = true;
                procInfo.RedirectStandardOutput = true;
                procInfo.RedirectStandardError = true;

                proc.StartInfo = procInfo;
                proc.Start();

                string strQuery1 = @"'" + m_strLocalCopyPath + @"\" + fileName + @"' WITH MOVE '" + m_strDatabaseName + "' TO '" + m_strMdfFilePath  + m_strDatabaseName + ".mdf'";
                //string strQuery1 = @"'C:\UNE\DataImportNServerReboot\BackupData\" + fileName + @"' WITH MOVE 'BLD_205' TO 'D:\SQL Server\MSSQLSERVER\MSSQL\DATA\BLD_205.mdf'";
                string strMoveLogFile = string.Format("MOVE '{0}_log' TO '{1}{0}_log.ldf', REPLACE, STATS = 10;", m_strDatabaseName, m_strMdfFilePath);
                string strQuery2 = string.Format("\"restore database {1} from disk = {0}, {2}\"", strQuery1, m_strDatabaseName, strMoveLogFile);                
                string strQuery = "sqlcmd -E -S 127.0.0.1 -Q " + strQuery2;

                // 명령어는 보낼때 NewLine도 같이 보내야 전송됨
                proc.StandardInput.Write("C:" + Environment.NewLine);
                proc.StandardInput.Write(strQuery + Environment.NewLine);
                proc.StandardInput.Close();

                string resultValue = proc.StandardOutput.ReadToEnd();
                strResultMessage = resultValue;
                proc.WaitForExit();
                proc.Close();

                Logger.Instance.Write("import result : " + strResultMessage);
                webDBManager.GetResultData("ALTER DATABASE BLD_205 Set multi_user with rollback immediate");

                return true;
            }
            catch (Exception ex)
            {
                webDBManager.GetResultData("ALTER DATABASE BLD_205 Set multi_user with rollback immediate");

                Logger.Instance.Write("[ERROR] DataImport " + ex.Message);
                return false;
            }
        }

        private bool StopService()
        {
            try
            {
                foreach (string process in m_processList)
                {
                    if (ServiceManager.IsRunningSerivce(process))
                    {
                        if (!ServiceManager.StopService(process, 5000))
                            throw new ApplicationException(process + " 서비스를 중지할 수 없습니다"); 
                    }
                }

                Process[] psm = Process.GetProcessesByName("PSMSensorServer");
                if (psm != null && psm.Length > 0)
                {
                    psm[0].Close();
                }

                return true;
            }
            catch (ApplicationException app)
            {                
                Logger.Instance.Write("[ERROR] StopService : " + app.Message);
                StartService();
                return false;
            }
            catch (Exception ex)
            {
                Logger.Instance.Write("[ERROR] StopService : " + ex.Message);
                StartService();
                return false;
            }
        }

        private bool StartService()
        {
            try
            {
                foreach (string process in m_processList)
                {
                    if (!ServiceManager.IsRunningSerivce(process))
                    {                        
                        if (!ServiceManager.StartService(process, 5000))
                            throw new ApplicationException(process + " 서비스를 시작할 수 없습니다"); 
                    }
                }

                Process[] psm = Process.GetProcessesByName("PSMSensorServer");
                if (psm != null && psm.Length == 0)
                {
                    psm[0].Close();
                }

                return true;
            }
            catch (ApplicationException app)
            {
                Logger.Instance.Write("[ERROR] StartService : " + app.Message);
                return false;
            }
            catch (Exception ex)
            {
                Logger.Instance.Write("[ERROR] StartService : " + ex.Message);
                return false;
            }
        }

        private bool Download(string fileName)
        {
            if (fileName.Length == 0)
                return false;

            string strErrorMsg = "";
            string webServerFilePath = m_strDownloadServerPath + "\\" + fileName;
            string copyFilePath = m_strLocalCopyPath + "\\" + fileName;

            try
            {
                if (File.Exists(copyFilePath))
                    File.Delete(copyFilePath);

                DBUtility2.UpDownManager.DownloadFile(webServerFilePath, copyFilePath, m_strDownloadServerURL, out strErrorMsg);
                //DBUtility2.UpDownManager.DownloadFile(webServerFilePath, copyFilePath, "http://192.168.0.214", out strErrorMsg);

                if (strErrorMsg.Length > 0)
                {
                    Logger.Instance.Write("[ERROR] Download(string) fileName:" + fileName + "/" + strErrorMsg);
                    return false;
                }
            }
            catch (Exception ex)
            {
                //WriteLog("[Download ERROR] " + ex.Message + "\r\n" + "webServerFilePath : " + webServerFilePath + "\r\n" + "copyFilePath : " + copyFilePath);
                Logger.Instance.Write("[ERROR] " + ex.Message);
                Logger.Instance.Write("[ERROR] Download(string) fileName:" + fileName + "/" + strErrorMsg);
                return false;
            }

            return true;
        }
    }
}
