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
using DBUtility2;

namespace Updater
{
    public class AutoUpdater
    {
        private bool m_bExitUpdate = false;
        public bool IsExitUpdate
        {
            get { return m_bExitUpdate; }
        }

        private ArrayList m_arUpdateInfo = new ArrayList();

        public AutoUpdater()
        {
            InitUpdate();
        }


        private string m_strServerAddr = "";
        private string m_szUpdateURL = "";
        private int m_nSiteID = 1;

        public void InitUpdate()
        {
            m_nSiteID = LoadSiteID();
            string strServerURL = RegUtil.ReadRegValue("Server Connection Info", "webserver_url", m_nSiteID);
            if( strServerURL == "")
            {
                strServerURL = "http://172.18.101.50:8080/SOP/";
            }            

            int nIndex1 = strServerURL.IndexOf("http://");
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
            }

            if (!updateURL)
                m_szUpdateURL = "http://" + m_strServerAddr + ":8080/update/";
        }

        public int LoadSiteID()
        {
            Utility ini = new Utility();
            string strSiteID = ini.getinivalue("Server Connection Info", "siteid");
            //string strSiteID = m_dbMgr.LoadIni("siteid", "Server Connection Info");

            int nSiteID = 1;

            if (strSiteID.Length > 0)
            {
                int.TryParse(strSiteID, out nSiteID);
            }

            return nSiteID;
        }

        private void UpdateFile(UpdateInfo info)
        {
            WebClient webClient = new WebClient();

            string szDirName = GetUpdateFilePath() + "\\" + info.Location;
            string szFileName = szDirName + "\\" + "filelist.txt";
            if (!Directory.Exists(szDirName))
                Directory.CreateDirectory(szDirName);

            if (File.Exists(szFileName))
            {
                File.Delete(szFileName);
            }
            string szURL = m_szUpdateURL + info.Location + "/" + info.FileName;
            
            webClient.DownloadFile(new Uri(szURL), szFileName);


            if (File.Exists(szFileName))
            {
                StreamReader file = new StreamReader(szFileName, System.Text.Encoding.Default);

                while (!file.EndOfStream)
                {
                    string szDownFileName = file.ReadLine();

                    string szLocalFileName = "";

                    string szAddPath = "";
                    if (szDownFileName.Contains('\\'))
                    {
                        string[] paths = szDownFileName.Split('\\');

                        string fileName = paths[paths.Length - 1];

                        szAddPath = szDownFileName.Replace(fileName, "");


                        string szDirPath = szDirName + "\\" + szAddPath;

                        if (!Directory.Exists(szDirPath))
                        {
                            Directory.CreateDirectory(szDirPath);
                        }

                    }

                    szLocalFileName = szDirName + "\\" + szDownFileName;



                    szDownFileName = szDownFileName.Replace('\\', '/');

                    string szDownURL = m_szUpdateURL + info.Location + "/" + szDownFileName;

                    try
                    {
                        webClient.DownloadFile(new Uri(szDownURL), szLocalFileName);

                        // 원본 파일 복사

                        string szPath = Assembly.GetEntryAssembly().Location;
                        string szTargetDir = Directory.GetParent(szPath).FullName;


                        string szCpyPath = szTargetDir + "\\" + szDownFileName;
                        File.Copy(szLocalFileName, szCpyPath, true);


                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.StackTrace);
                    }

                }

                file.Close();
                file.Dispose();
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

        //public void AutoUpdate()
        //{
        //    Thread t = new Thread(BeginUpdate);
        //    t.Start();
        //}

        //private void BeginUpdate()
        //{
        //    try
        //    {
        //        WebClient webClient = new WebClient();

        //        string szFileName = GetUpdateFilePath() + "\\" + "update.xml";
        //        if (File.Exists(szFileName))
        //        {
        //            File.Delete(szFileName);
        //        }

        //        try
        //        {
        //            webClient.DownloadFile(new Uri(m_szUpdateURL + "update.xml"), szFileName);
        //        }
        //        catch (Exception)
        //        {
        //            Thread.Sleep(1000);
        //            m_bExitUpdate = true;
        //            return;
        //        }

        //        string szValue = RegUtil.ReadRegValue("Update Info", "Current");
        //        int nCurrentVersion = -1;

        //        int.TryParse(szValue, out nCurrentVersion);

        //        if (File.Exists(szFileName))
        //        {
        //            XmlTextReader reader = new XmlTextReader(szFileName);
        //            XmlDocument xmlDoc = new XmlDocument();
        //            xmlDoc.Load(reader);

        //            string strVersion = ReadLastVersion(xmlDoc);
        //            int nTargetID = -1;
        //            int.TryParse(strVersion, out nTargetID);

        //            foreach (XmlNode node in xmlDoc.ChildNodes)
        //            {
        //                if (node.Name == "update")
        //                {
        //                    ReadUpdate(node);
        //                }
        //            }

        //            if (m_arUpdateInfo.Count > 0)
        //            {
        //                foreach (UpdateInfo info in m_arUpdateInfo)
        //                {
        //                    if (info.ID <= nTargetID)
        //                    {
        //                        if (nCurrentVersion < info.ID)
        //                        {
        //                            // 각 info 별로 update 진행
        //                            try
        //                            {
        //                                UpdateFile(info);
        //                            }
        //                            catch (Exception) { }

        //                        }
        //                    }
        //                }
        //            }
        //            reader.Close();
        //        }

        //        // 업데이트 폴더를 삭제
        //        string szDeleteDir = GetUpdateFilePath();
        //        if (Directory.Exists(szDeleteDir))
        //        {
        //            Directory.Delete(szDeleteDir, true);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    m_bExitUpdate = true;
        //}

        public bool CheckUpdateXML()
        {
            bool m_bNeedUpdate = false;
            try
            {
                m_arUpdateInfo.Clear();
                WebClient webClient = new WebClient();

                string szFileName = GetUpdateFilePath() + "\\" + "update.xml";
                if (File.Exists(szFileName))
                {
                    File.Delete(szFileName);
                }

                try
                {
                    webClient.DownloadFile(new Uri(m_szUpdateURL + "update.xml"), szFileName);
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                    return false;
                }

                string strCurrentVersion = RegUtil.ReadRegValue("Update Info", "Current", m_nSiteID);
                //int nCurrentVersion = -1;

                //int.TryParse(szValue, out nCurrentVersion);

                if (File.Exists(szFileName))
                {
                    XmlTextReader reader = new XmlTextReader(szFileName);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(reader);

                    string strTargetVersion = ReadLastVersion(xmlDoc);
                    //int nTargetID = -1;
                    //int.TryParse(strVersion, out nTargetID);

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
                        foreach (UpdateInfo info in m_arUpdateInfo)
                        {
                            if (string.Compare(info.ID, strTargetVersion) <= 0)
                            //if (info.ID <= nTargetID)
                            {
                                if (string.Compare(strCurrentVersion, info.ID) < 0)
                                //if (nCurrentVersion < info.ID)
                                {
                                    m_bNeedUpdate = true;
                                    break;
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
            return m_bNeedUpdate;
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
                    /*string a = attr.Value;
                    int nID = -1;
                    if (!int.TryParse(a, out nID))
                    {
                        continue;
                    }*/

                    string strVersionID = attr.Value;

                    UpdateInfo info = new UpdateInfo();
                    info.ID = strVersionID;
                    info.Name = child.ChildNodes[0].InnerText;
                    info.Time = Convert.ToDateTime(child.ChildNodes[1].InnerText);
                    info.ForceUpdate = child.ChildNodes[2].InnerText == "true" ? true : false;
                    info.Location = child.ChildNodes[3].InnerText;
                    info.Revision = child.ChildNodes[5].InnerText;

                    XmlAttribute attr2 = child.ChildNodes[4].Attributes[0];
                    info.FileName = attr2.Value;

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

        private string m_szFileName = "";

        public string FileName
        {
            get { return m_szFileName; }
            set { m_szFileName = value; }
        }

        public int CompareTo(object obj)
        {
            UpdateInfo info = (UpdateInfo)obj;
            return string.Compare(this.ID, info.ID);
        }
    }
}
