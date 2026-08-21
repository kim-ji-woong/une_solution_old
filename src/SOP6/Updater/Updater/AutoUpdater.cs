using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Threading;

namespace Updater
{
    public class AutoUpdater
    {
        private ArrayList m_arUpdateInfo = new ArrayList();

        private int m_nSiteID = 1;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        private string m_strServerAddr = "";
        public string ServerAddress
        {
            get { return m_strServerAddr; }
            set { m_strServerAddr = value; }
        }

        private string m_szUpdateURL = "";
        public string UpdateURL
        {
            get { return m_szUpdateURL; }
            set { m_szUpdateURL = value; }
        }

        private bool m_bExitUpdate = false;
        public bool IsExitUpdate
        {
            get { return m_bExitUpdate; }
        }

        private string szLstFileName = "filelist.txt";

        private string szUpdateXML = "update.xml";

        public AutoUpdater()
        {
            string szSiteID = RegUtil.ReadINI("Server Connection Info", "siteid");
            if(!int.TryParse(szSiteID, out m_nSiteID))
            {
                m_nSiteID = 1;
            }

            InitUpdate();
        }

        public void InitUpdate()
        {
            try
            {
                string strServerURL = RegUtil.ReadRegValue("Server Connection Info", "webserver_url2", m_nSiteID);
                if (strServerURL == "")
                {
                    strServerURL = "http://172.18.101.50:8080/SOP/";
                }
                m_szUpdateURL = strServerURL + "/Update/";
                /*int nIndex1 = strServerURL.IndexOf("http://");
                int nIndex2 = strServerURL.LastIndexOf(':');
                string strURL = strServerURL;

                if (nIndex1 >= 0 && nIndex2 >= 0)
                {
                    int nBeginIndex = nIndex1 + "http://".Length;
                    strURL = strServerURL.Substring(nBeginIndex, nIndex2 - nBeginIndex);
                }
                else if (nIndex1 >= 0)
                {
                    int nBeginIndex = nIndex1 + "http://".Length;
                    strURL = strServerURL.Substring(nBeginIndex);
                }
                else if (nIndex2 >= 0)
                {
                    strURL = strServerURL.Substring(0, nIndex2);
                }

                System.Net.IPAddress[] addr = System.Net.Dns.GetHostAddresses(strURL);
                m_strServerAddr = addr[0].ToString();
                //m_szUpdateURL = "http://" + m_strServerAddr + ":8080/update/";

                bool updateURL = false;

                if (nIndex2 >= 0)
                {
                    int nIndex3 = strServerURL.IndexOf('/', nIndex2 + 1);

                    if (nIndex3 >= 0)
                    {
                        m_szUpdateURL = "http://" + m_strServerAddr + ":" + strServerURL.Substring(nIndex2 + 1, nIndex3 - nIndex2 - 1) + "/update/";
                        updateURL = true;
                    }
                }*/

                //if (!updateURL)
                    //m_szUpdateURL = "http://" + m_strServerAddr + ":8080/update/";
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("서버의 주소를 받아올 수 없습니다.");
                System.Windows.Forms.Application.Exit();
            }
        }

        private static char ConvertToHex(char cSource)
        {
            return "0123456789abcdef"[0x0f & cSource];
        }

        public static string URLEncoding(byte[] bytes)
        {
            string strResult = "";

            foreach (byte element in bytes)
            {
                if ((element >= '0' && element <= '9') ||   // 숫자
                    (element >= 'a' && element <= 'z') ||   // 소문자
                    (element >= 'A' && element <= 'Z') ||   // 대문자
                    (element == '!' || element == '*' || element == '(' || element == ')' || element == '_' || element == '-')) // 그 외의 특수기호들
                {
                    strResult += (char)element;
                }
                else
                {
                    strResult += "%";
                    strResult += ConvertToHex((char)((int)element >> 4));
                    strResult += ConvertToHex((char)element);
                }
            }
            return strResult;
        }

        private string URLEncoding(string strOrigin)
        {
            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes = enc.GetBytes(strOrigin);
            return URLEncoding(bytes);
        }

        private void UpdateFile(UpdateInfo info)
        {
            WebClient webClient = new WebClient();
            
            string szDirName = GetUpdateFilePath();
            string szFileName = szDirName + "\\" + info.Name + ".zip";
            if (!Directory.Exists(szDirName))
                Directory.CreateDirectory(szDirName);

            if (File.Exists(szFileName))
            {
                File.Delete(szFileName);
            }
            if (Directory.Exists(szDirName + "\\" + info.Name))
            {
                Directory.Delete(szDirName + "\\" + info.Name);
            }

            string szURL = "";

            if (m_szUpdateURL.EndsWith("/"))
                szURL = m_szUpdateURL + info.Name + ".zip";
            else
                szURL = m_szUpdateURL + "/" + info.Name + ".zip";

            //string szURL = m_szUpdateURL + URLEncoding(info.Location) + "/" + URLEncoding(info.FileName);

            webClient.DownloadFile(new Uri(szURL), szFileName);

            ExtractToTrg(szFileName, szDirName + "\\" + info.Name);

            // 예외 처리
            bool isCopy = false;
            string[] dirs = Directory.GetDirectories(szDirName + "\\" + info.Name);
            if (dirs.Length > 0)
            {
                foreach (string item in dirs)
                {
                    int index1 = item.LastIndexOf("\\") + 1;
                    string temp = item.Substring(index1, item.Length - index1);
                    if (temp == info.Name)
                    {
                        Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(item, GetDescPath(), true);
                        isCopy = true;
                    }
                }
            }
        
            if (!isCopy)
                Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(szDirName + "\\" + info.Name, GetDescPath(), true);

            //if (File.Exists(szFileName))
            //{
            //    StreamReader file = new StreamReader(szFileName, System.Text.Encoding.Default);

            //    while (!file.EndOfStream)
            //    {
            //        string szDownFileName = file.ReadLine();

            //        string szLocalFileName = "";

            //        string szAddPath = "";
            //        if (szDownFileName.Contains('\\'))
            //        {
            //            string[] paths = szDownFileName.Split('\\');

            //            string fileName = paths[paths.Length - 1];

            //            szAddPath = szDownFileName.Replace(fileName, "");


            //            string szDirPath = szDirName + "\\" + szAddPath;

            //            if (!Directory.Exists(szDirPath))
            //            {
            //                Directory.CreateDirectory(szDirPath);
            //            }
            //        }

            //        szLocalFileName = szDirName + "\\" + szDownFileName;
            //        szDownFileName = szDownFileName.Replace('\\', '/');

            //        string[] downFiles = szDownFileName.Split('/');
            //        string strDownURLFile = "";

            //        foreach (string strFile in downFiles)
            //        {
            //            if (strDownURLFile.Length == 0)
            //                strDownURLFile += URLEncoding(strFile);
            //            else
            //                strDownURLFile += "/" + URLEncoding(strFile);
            //        }

            //        string szDownURL = m_szUpdateURL + URLEncoding(info.Location) + "/" + strDownURLFile;//URLEncoding(szDownFileName);
            //        try
            //        {
            //            webClient.DownloadFile(new Uri(szDownURL), szLocalFileName);

            //            // 원본 파일 복사

            //            string szPath = Assembly.GetEntryAssembly().Location;
            //            string szTargetDir = Directory.GetParent(szPath).FullName;

            //            string szCpyPath = szTargetDir + "\\" + szDownFileName;
            //            CheckFilePath(ref szCpyPath);
            //            File.Copy(szLocalFileName, szCpyPath, true);
            //        }
            //        catch (Exception e)
            //        {
            //            Debug.WriteLine(e.StackTrace);
            //        }
            //    }
            //    file.Close();
            //    file.Dispose();
            //}
        }
        
        private void CheckFilePath(ref string strPath)
        {
            strPath = strPath.Replace('/', '\\');

            int nIndex = strPath.LastIndexOf('\\');

            if (nIndex < 0)
                return;

            string strFolder = strPath.Substring(0, nIndex);

            if (!System.IO.Directory.Exists(strFolder))
            {
                System.IO.Directory.CreateDirectory(strFolder);
            }
        }

        private string GetUpdateFilePath()
        {
            string szPath = Assembly.GetEntryAssembly().Location;
            string szFullPath = Directory.GetParent(szPath).FullName;
            szFullPath += "\\update";

            if (!Directory.Exists(szFullPath))
                Directory.CreateDirectory(szFullPath);
        
            return szFullPath;
        }

        private string GetDescPath()
        {
            string szPath = Assembly.GetEntryAssembly().Location;
            string szFullPath = Directory.GetParent(szPath).FullName;

            return szFullPath;
        }

        public void AutoUpdate()
        {
            Thread t = new Thread(BeginUpdate);
            t.Start();
        }

        public bool CheckUpdateXML()
        {
            bool m_bNeedUpdate = false;
            try
            {
                WebClient webClient = new WebClient();

                string szFileName = GetUpdateFilePath() + "\\" + szUpdateXML;
                if (File.Exists(szFileName))
                {
                    File.Delete(szFileName);
                }

                try
                {
                    webClient.DownloadFile(new Uri(m_szUpdateURL + szUpdateXML), szFileName);
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                    return false;
                }

                string strTargetVersion = "";
                string strCurrentVersion = RegUtil.ReadRegValue("Update Info", "Current", m_nSiteID);

                if (strCurrentVersion.Length == 0)
                {
                    DateTime dtNow = DateTime.Now;
                    string strTime = string.Format("{0}.{1:00}.{2:00}_{3:00}{4:00}{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
                    RegUtil.WriteRegValue("Update Info", "Current", "1.000", m_nSiteID);
                    RegUtil.WriteRegValue("Update Info", "InstallDate", strTime, m_nSiteID);
                }

                if (File.Exists(szFileName))
                {
                    XmlTextReader reader = new XmlTextReader(szFileName);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(reader);

                    strTargetVersion = ReadLastVersion(xmlDoc);

                    foreach (XmlNode node in xmlDoc.ChildNodes)
                    {
                        if (node.Name == "update")
                        {
                            ReadUpdate(node);
                            break;
                        }
                    }

                    List<UpdateInfo> updates = new List<UpdateInfo>();

                    if (m_arUpdateInfo.Count > 0)
                    {
                        foreach (UpdateInfo info in m_arUpdateInfo)
                        {
                            if (string.Compare(info.ID, strTargetVersion) <= 0)
                            {
                                if (string.Compare(strCurrentVersion, info.ID) < 0)
                                {
                                    m_bNeedUpdate = true;
                                    updates.Add(info);
                                    //break;
                                    /*string szValue2 = RegUtil.ReadRegValue("Update Info", "InstallDate");
                                    if (szValue2 == null || szValue2 == "")
                                    {
                                        m_bNeedUpdate = true;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            DateTime dt = Convert.ToDateTime(szValue2);
                                            if (info.Time > dt)
                                            {
                                                m_bNeedUpdate = true;
                                            }
                                            else
                                            {
                                                int i = 0;
                                                i++;
                                            }
                                        }
                                        catch (System.Exception)
                                        {                                        	
                                        }
                                    }*/
                                }
                            }
                        }
                    }
                    reader.Close();

                    foreach (UpdateInfo info in updates)
                    {
                        UpdateFile(info);
                    }
                }

                try
                {
                    // 업데이트 폴더를 삭제
                    string szDeleteDir = GetUpdateFilePath();
                    if (Directory.Exists(szDeleteDir))
                    {
                        Directory.Delete(szDeleteDir, true);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                }

                if (m_bNeedUpdate)
                {
                    RegUtil.WriteRegValue("Update Info", "Current", strTargetVersion, m_nSiteID);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
            return m_bNeedUpdate;
        }

        public void BeginUpdate()
        {
            try
            {
                Thread.Sleep(3000);

                m_arUpdateInfo.Clear();
                WebClient webClient = new WebClient();

                string szFileName = GetUpdateFilePath() + "\\" + szUpdateXML;
                if (File.Exists(szFileName))
                {
                    File.Delete(szFileName);
                }

                try
                {
                    webClient.DownloadFile(new Uri(m_szUpdateURL + szUpdateXML), szFileName);
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                    m_bExitUpdate = true;
                    return;
                }
                
                if (File.Exists(szFileName))
                {
                    XmlTextReader reader = new XmlTextReader(szFileName);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(reader);

                    string strTargetVersion = ReadLastVersion(xmlDoc);
                    foreach (XmlNode node in xmlDoc.ChildNodes)
                    {
                        if (node.Name == "update")
                        {
                            ReadUpdate(node);
                            break;
                        }
                    }

                    if (m_arUpdateInfo.Count > 0)
                    {
                        string strCurrentVersion = RegUtil.ReadRegValue("Update Info", "Current", m_nSiteID);

                        foreach (UpdateInfo info in m_arUpdateInfo)
                        {
                            if (string.Compare(info.ID, strTargetVersion) <= 0)
                            {
                                if (string.Compare(strCurrentVersion, info.ID) < 0)
                                {                                    
                                    // 각 info 별로 update 진행                                 
                                    UpdateFile(info);
                                    RegUtil.WriteRegValue("Update Info", "Current", info.ID.ToString(), m_nSiteID);
                                    strCurrentVersion = info.ID;
                                }
                            }
                        }
                    }
                    reader.Close();
                }

                // 업데이트 폴더를 삭제
                string szDeleteDir = GetUpdateFilePath();
                if (Directory.Exists(szDeleteDir))
                {
                    Directory.Delete(szDeleteDir, true);
                }
            }
            catch (Exception)
            {
            }           
            m_bExitUpdate = true;
        }

        private void ReadUpdate(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "versions")
                {
                    ReadVersion(child);
                }
            }
            m_arUpdateInfo.Sort();
        }
        
        private void ReadVersion(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "version")
                {
                    XmlAttribute attr = child.Attributes[0];

                    string strVersionID = attr.Value;
                    if (strVersionID.Length == 0)
                        continue;

                    UpdateInfo info = new UpdateInfo();
                    info.ID = strVersionID;
                    info.Name = child.ChildNodes[0].InnerText;
                    info.Time = Convert.ToDateTime(child.ChildNodes[1].InnerText);
                    info.ForceUpdate = child.ChildNodes[2].InnerText == "true" ? true : false;
                    info.Location = child.ChildNodes[3].InnerText;
                    info.Revision = child.ChildNodes[4].InnerText;

                    //XmlAttribute attr2 = child.ChildNodes[4].Attributes[0];
                    //info.FileName = attr2.Value;

                    m_arUpdateInfo.Add(info);
                }
            }
        }

        private string ReadLastVersion(XmlDocument xmlDoc)
        {
            XmlNodeList list = xmlDoc.GetElementsByTagName("lastVersion");
            XmlNode node = list[0];
            string strLastVersion = node.InnerText;
            Debug.WriteLine(strLastVersion);
            return strLastVersion;
        }

        private bool ExtractToTrg(string strSrcFile, string strTrgPath)
        {
            try
            {
                if (!Directory.Exists(strTrgPath))
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
                return false;
            }

            return true;
        }
    }

    public class UpdateInfo : IComparable
    {
        private string m_szLocation = "";

        public string Location
        {
            get { return m_szLocation; }
            set { m_szLocation = value; }
        }
        private string m_szRevision = "";

        public string Revision
        {
            get { return m_szRevision; }
            set { m_szRevision = value; }
        }
        private bool m_bForceUpdate = false;

        public bool ForceUpdate
        {
            get { return m_bForceUpdate; }
            set { m_bForceUpdate = value; }
        }
        private DateTime time;

        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }
        private string m_szName = "";

        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }
        /*private int nID = -1;

        public int ID
        {
            get { return nID; }
            set { nID = value; }
        }*/
        private string strID = "";
        public string ID
        {
            get { return strID; }
            set { strID = value; }
        }

        /*private string m_szFileName = "";

        public string FileName
        {
            get { return m_szFileName; }
            set { m_szFileName = value; }
        }*/

        public int CompareTo(object obj)
        {
            UpdateInfo info = (UpdateInfo)obj;
            return string.Compare(this.ID, info.ID);
        }
    }
}
