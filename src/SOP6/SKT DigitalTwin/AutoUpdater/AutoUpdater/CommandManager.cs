using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Net;
using System.IO;
using System.Configuration;

namespace AutoUpdater
{
    public class CommandManager
    {
        public enum CommandType { Update = 0, Stop, Start };
        public enum CommandResultType { UnknownCommand = -1, UpdateSeccess = 0, UpdateFail, StopSuccess, StopFail, StartSuccess, StartFail };

        private const string m_strVersionFile = "update.ver";

        protected static string GetURL(WebDBManagerEx dbMgr, string strZipFileName)
        {
            string strDownloadBase = ConfigurationManager.AppSettings.Get("downloadBase");

            if (strDownloadBase == null || strDownloadBase.Length == 0 )
                return "";

            if (strDownloadBase.EndsWith("\\") == false)
                strDownloadBase += "\\";

            string strURL = strDownloadBase + dbMgr.SiteID.ToString() + "\\" + strZipFileName;
            /*string strURL = "";

            if (dbMgr.WebServerURL.EndsWith("/"))
                strURL = dbMgr.WebServerURL + "Site/" + dbMgr.SiteID.ToString() + "/" + strZipFileName;
            else
                strURL = dbMgr.WebServerURL + "/Site/" + dbMgr.SiteID.ToString() + "/" + strZipFileName;*/

            return strURL;
        }
#if !SERVICE
        protected static bool DownloadFile(WebDBManager dbMgr, string strURL, string strLocalFilePath, ref string strErrorMessage)
        {
            bool result = false;

            try
            {
                result = UpDownManager.DownloadFile(strURL, strLocalFilePath, dbMgr.WebServerURL, out strErrorMessage);
                //WebClient client = new WebClient();
                //client.DownloadFile(strURL, strLocalFilePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                strErrorMessage = ex.Message;
                return false;
            }

            return result;
        }
#endif
        protected static bool CheckVersion(bool isServer, string strUpdateVersion, out bool needUpdate, ref string strErrorMessage)
        {
            needUpdate = false;
            string strServerVersion = "", strClientVersion = "";

            if (ReadVersion(ref strServerVersion, ref strClientVersion) == false)
            {
                strErrorMessage = m_strVersionFile + " 파일을 읽을수 없습니다.";
                return false;
            }

            if (isServer)
                needUpdate = strServerVersion.CompareTo(strUpdateVersion) < 0;
            else
                needUpdate = strClientVersion.CompareTo(strUpdateVersion) < 0;

            return true;
        }

        private static bool ReadVersion(ref string strServerVersion, ref string strClientVersion)
        {
            if (File.Exists(m_strVersionFile) == false)
            {
                strServerVersion = strClientVersion = "V1.000";

                StreamWriter writer = new StreamWriter(m_strVersionFile, false, System.Text.Encoding.UTF8);
                writer.WriteLine("server : " + strServerVersion);
                writer.Write("client : " + strClientVersion);
                writer.Close();

                return true;
            }

            StreamReader reader = new StreamReader(m_strVersionFile, System.Text.Encoding.UTF8);
            bool readServer = false, readClient = false;

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                int nIndex = strLine.IndexOf(':');

                if (nIndex < 0)
                    continue;

                string strTag = strLine.Substring(0, nIndex).Trim();
                string strValue = strLine.Substring(nIndex + 1).Trim();

                if (strTag.ToLower() == "server")
                {
                    readServer = true;
                    strServerVersion = strValue;
                }
                else if (strTag.ToLower() == "client")
                {
                    readClient = true;
                    strClientVersion = strValue;
                }
            }

            reader.Close();
            return readServer && readClient;
        }

        public static void UpdateVersionFile(bool isServer, string strVersion)
        {
            StreamReader reader = new StreamReader(m_strVersionFile, System.Text.Encoding.UTF8);
            string strFile = "";

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                int nIndex = strLine.IndexOf(':');

                if (nIndex < 0)
                    continue;

                string strTag = strLine.Substring(0, nIndex).Trim();
                string strValue = strLine.Substring(nIndex + 1).Trim();

                if (strTag.ToLower() == "server")
                {
                    if (isServer)
                        strLine = strTag + " : " + strVersion;
                }
                else if (strTag.ToLower() == "client")
                {
                    if (isServer == false)
                        strLine = strTag + " : " + strVersion;
                }

                if (strFile.Length == 0)
                    strFile = strLine;
                else
                    strFile += "\r\n" + strLine;
            }

            reader.Close();

            StreamWriter writer = new StreamWriter(m_strVersionFile, false, System.Text.Encoding.UTF8);
            writer.Write(strFile);
            writer.Close();
        }

        protected static void DeleteFile(string strFilePath)
        {
            try
            {
                File.Delete(strFilePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        protected static void DeleteFolder(string strFolder)
        {
            string[] files = Directory.GetFiles(strFolder);

            foreach (string strFile in files)
            {
                DeleteFile(strFile);
            }

            string[] folders = Directory.GetDirectories(strFolder);

            foreach (string folder in folders)
            {
                DeleteFolder(folder);
            }

            try
            {
                Directory.Delete(strFolder);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }

        protected static bool UpdateFile(string strZipFilePath, string strTargetFolder, ref string strResultMessage)
        {
            string strSrcFolder;

            if (ExtractZipFile(strZipFilePath, out strSrcFolder, ref strResultMessage) == false)
                return false;

            bool result = CopyFolder(strSrcFolder, strTargetFolder, ref strResultMessage);

            // 임시 파일은 삭제한다.
            DeleteFolder(strSrcFolder);
            DeleteFile(strZipFilePath);
            return result;
        }

        // 파일 또는 폴더를 복사한다.
        // 기존 파일이 존재하면 덮어쓰기 한다.
        private static bool CopyFolder(string strSrcFolder, string strTargetFolder, ref string strResultMessage)
        {
            string[] files = Directory.GetFiles(strSrcFolder);

            try
            {
                foreach (string strFile in files)
                {
                    int nIndex = strFile.LastIndexOf('\\');
                    string strFileName = nIndex < 0 ? strFile : strFile.Substring(nIndex + 1).Trim();

                    File.Copy(strFile, strTargetFolder + "\\" + strFileName, true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                strResultMessage = ex.Message;
                return false;
            }

            string[] folders = Directory.GetDirectories(strSrcFolder);

            try
            {
                foreach (string strFolder in folders)
                {
                    int nIndex = strFolder.LastIndexOf('\\');
                    string strFolderName = nIndex < 0 ? strFolder : strFolder.Substring(nIndex + 1).Trim();

                    string trgFolder = strTargetFolder + "\\" + strFolderName;

                    if (Directory.Exists(trgFolder) == false)
                        Directory.CreateDirectory(trgFolder);

                    if (CopyFolder(strFolder, trgFolder, ref strResultMessage) == false)
                        return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                strResultMessage = ex.Message;
                return false;
            }

            return true;
        }

        private static bool ExtractZipFile(string strSrcFile, out string strTrgPath, ref string strResultMessage)
        {
            strTrgPath = "";

            try
            {
                string strFileName = strSrcFile;
                int nIndex = strSrcFile.LastIndexOf('\\');

                if (nIndex >= 0)
                    strFileName = strFileName.Substring(nIndex + 1);

                int nIndex2 = strFileName.LastIndexOf('.');

                if (nIndex2 >= 0)
                    strFileName = strFileName.Substring(0, nIndex2);

                //int nIndex = strSrcFile.LastIndexOf('\\');

                if (nIndex < 0)
                    return false;

                strTrgPath = strSrcFile.Substring(0, nIndex + 1).Trim() + strFileName;

                if (Directory.Exists(strTrgPath))
                    DeleteFolder(strTrgPath);

                Directory.CreateDirectory(strTrgPath);

                System.IO.FileStream fs = new System.IO.FileStream(strSrcFile, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);

                ICSharpCode.SharpZipLib.Zip.ZipInputStream zis = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(fs);

                ICSharpCode.SharpZipLib.Zip.ZipEntry ze;

                while ((ze = zis.GetNextEntry()) != null)
                {
                    if (!ze.IsDirectory)
                    {
                        string fileName = System.IO.Path.GetFileName(ze.Name);

                        string destDir = System.IO.Path.Combine(strTrgPath,
                                         System.IO.Path.GetDirectoryName(ze.Name));

                        if (false == Directory.Exists(destDir))
                        {
                            System.IO.Directory.CreateDirectory(destDir);
                        }

                        string destPath = System.IO.Path.Combine(destDir, fileName);

                        System.IO.FileStream writer = new System.IO.FileStream(
                                        destPath, System.IO.FileMode.Create,
                                                System.IO.FileAccess.Write,
                                                    System.IO.FileShare.Write);

                        byte[] buffer = new byte[2048];
                        int len;
                        while ((len = zis.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            writer.Write(buffer, 0, len);
                        }

                        writer.Close();
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                strResultMessage = e.Message;
                return false;
            }

            return true;
        }
    }
}
