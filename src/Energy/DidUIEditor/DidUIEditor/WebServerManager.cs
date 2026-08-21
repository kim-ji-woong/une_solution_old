using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DidUIEditor
{
    public class WebServerManager
    {
        private DBUtility2.WebDBManager m_dbMgr = null;
        private string m_strWebServerUploadPath = "";

        private string m_strLocalFilePath = "";
        public string LocalFilePath
        {
            get { return m_strLocalFilePath; }
            set { m_strLocalFilePath = value; }
        }
        private string m_strDownloadWebServerURL = "";
        public WebServerManager()
        {
            int nSiteID = ReadSiteID();
            
            m_dbMgr = new DBUtility2.WebDBManager(nSiteID);
            if (m_strDownloadWebServerURL.Length == 0)
                m_strDownloadWebServerURL = m_dbMgr.WebServerURL;
            ReadUploadPath();
        }

        private int ReadSiteID()
        {
            DBUtility2.Utility util = new DBUtility2.Utility();

            string szDownloadURL = util.getinivalue("Server Connection Info", "DownloadWebServerURL");
            if (szDownloadURL != null && szDownloadURL.Length > 0)
            {
                m_strDownloadWebServerURL = szDownloadURL;
            }

            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID != null && szSiteID.Length > 0)
            {
                int nSiteId = 1;
                if (int.TryParse(szSiteID, out nSiteId))
                    return nSiteId;
            }
            
            return -1;
        }

        private void ReadUploadPath()
        {
            DBUtility2.Utility util = new DBUtility2.Utility();
            string path = util.getinivalue("Setting", "WebServerUploadPath");
            if (path != null && path.Length > 0)
                m_strWebServerUploadPath = path;
        }

        public void Upload(string filePath)
        {
            string strErrorMsg = "";
            try
            {                
                DBUtility2.UpDownManager.UploadFile(filePath, m_strDownloadWebServerURL, out strErrorMsg);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + "\r\n" + strErrorMsg);
            }
        }
        
        public bool Download(string fileName)
        {
            if (fileName.Length == 0)
                return false;
                        
            string strErrorMsg = "";
            string webServerFilePath = m_strWebServerUploadPath + "\\" + fileName;
            string copyFilePath = MakeMediaFilePath(fileName);
            
            try
            {
                if (File.Exists(copyFilePath))
                    File.Delete(copyFilePath);

                DBUtility2.UpDownManager.DownloadFile(webServerFilePath, copyFilePath, m_strDownloadWebServerURL, out strErrorMsg);
                //DBUtility2.UpDownManager.DownloadFile(webServerFilePath, copyFilePath, "http://192.168.0.214", out strErrorMsg);
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show("서버 upload 경로의 " + webServerFilePath + "을 확인하세요");
                //MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        public bool DownloadEscape(string fileName)
        {
            if (fileName.Length == 0)
                return false;

            string strErrorMsg = "";
            string webServerFilePath = m_strWebServerUploadPath + "\\Escape\\" + fileName;
            string copyFilePath = MakeEscapeFilePath(fileName);

            try
            {
                if (File.Exists(copyFilePath))
                    return true;
                                
                DBUtility2.UpDownManager.DownloadFile(webServerFilePath, copyFilePath, m_strDownloadWebServerURL, out strErrorMsg);               
                //DBUtility2.UpDownManager.DownloadFile(webServerFilePath, copyFilePath, "http://192.168.0.214", out strErrorMsg);
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show("서버 upload 경로의 " + webServerFilePath + "을 확인하세요");
                //MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        public bool DownloadOutdoor(string fileName)
        {
            if (fileName.Length == 0)
                return false;

            string strErrorMsg = "";
            string webServerFilePath = m_strWebServerUploadPath + "\\Outdoor\\" + fileName;
            string copyFilePath = MakeOutdoorFilePath(fileName);

            try
            {
                if (File.Exists(copyFilePath))
                    return true;

                DBUtility2.UpDownManager.DownloadFile(webServerFilePath, copyFilePath, m_strDownloadWebServerURL, out strErrorMsg);
                //DBUtility2.UpDownManager.DownloadFile(webServerFilePath, copyFilePath, "http://192.168.0.214", out strErrorMsg);
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show("서버 upload 경로의 " + webServerFilePath + "을 확인하세요");
                //MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        private string GetFileName(string filePath)
        {
            int index = filePath.LastIndexOf(@"\");
            if (index < 0)
                return filePath;

            string fileName = filePath.Substring(index + 1);
            return fileName;
        }

        public string MakeMediaFilePath(string fileName)
        {
            if (!Directory.Exists(m_strLocalFilePath))
                Directory.CreateDirectory(m_strLocalFilePath);

            return m_strLocalFilePath + "\\" + fileName;
        }

        public string MakeEscapeFilePath(string fileName)
        {
            if (!Directory.Exists(m_strLocalFilePath + "\\Escape\\"))
                Directory.CreateDirectory(m_strLocalFilePath + "\\Escape\\");

            return m_strLocalFilePath + "\\Escape\\" + fileName;
        }

        public string MakeOutdoorFilePath(string fileName)
        {
            if (!Directory.Exists(m_strLocalFilePath + "\\Outdoor\\"))
                Directory.CreateDirectory(m_strLocalFilePath + "\\Outdoor\\");

            return m_strLocalFilePath + "\\Outdoor\\" + fileName;
        }
    }
}
