using System;
using System.Collections.Generic;
using System.Text;
using DBUtility;
using System.IO;
using System.Xml;

namespace SDMSAgent.SDMS
{
    public class UpdateManager
    {
        private static UpdateManager m_instance = null;
        public static UpdateManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new UpdateManager();
                    
                    m_instance.m_strUpdateSrcPath = FormMain.Instance.SdmsUpdateSrc;
                    m_instance.m_strUpdateTrgPath = FormMain.Instance.SdmsUpdateTrg;
                    m_instance.m_strUpdateTempPath = FormMain.Instance.SdmsUpdateTemp;
                }

                return m_instance;
            }
        }

        private string m_strUpdateSrcPath = "";
        private string m_strUpdateTrgPath = "";
        private string m_strUpdateTempPath = "";
        
        public void CheckUpdate()
        {
            if (m_strUpdateSrcPath.Length > 0 && m_strUpdateTrgPath.Length > 0 && m_strUpdateTempPath.Length > 0)
            {
                string strSrcFile = ReadSrc();

                if (strSrcFile != null)
                {
                    MakeEmpty(m_strUpdateTempPath);

                    if (ExtractToTrg(strSrcFile, m_strUpdateTempPath))
                    {
                        string strTargetFolderName = "";
                        string strFileListPath = MakeFileList(m_strUpdateTempPath, out strTargetFolderName);

                        if (strFileListPath != null && strFileListPath.Length > 0)
                        {
                            if (ExtractToTrg(strSrcFile, m_strUpdateTrgPath))
                            {
                                File.Delete(strSrcFile);

                                int nIndex = strFileListPath.LastIndexOf('\\');

                                if (nIndex >= 0)
                                {
                                    string strFileListName = strFileListPath.Substring(nIndex + 1);
                                    string strTrgPath = m_strUpdateTrgPath + "\\" + strTargetFolderName + "\\" + strFileListName;

                                    if (File.Exists(strTrgPath))
                                        File.Delete(strTrgPath);

                                    File.Copy(strFileListPath, strTrgPath);
                                }
                            }
                        }

                        MakeEmpty(m_strUpdateTempPath);
                    }
                }
            }
        }

        private string MakeFileList(string strPath, out string strTargetFolderName)
        {
            strTargetFolderName = "";

            string[] arrFiles = Directory.GetFiles(strPath);
            string strFileListPath = null, strTargetFileName = null;

            foreach (string strFile in arrFiles)
            {
                int nIndex = strFile.LastIndexOf('\\');

                if (nIndex >= 0)
                {
                    string _strFile = strFile.Substring(nIndex + 1);

                    if (string.Compare(_strFile, "update.xml", true) == 0)
                    {
                        strTargetFolderName = GetTargetFolder(strFile, out strTargetFileName);

                        if (strTargetFolderName != null && strTargetFileName != null &&
                            strTargetFolderName.Length > 0 && strTargetFileName.Length > 0)
                        {
                            strFileListPath = MakeFileList(strPath + "\\" + strTargetFolderName, strTargetFileName);
                        }

                        break;
                    }
                }
            }

            return strFileListPath;
        }

        private string MakeFileList(string strFolderPath, string strTargetFileName)
        {
            string strFileListPath = strFolderPath + "\\" + strTargetFileName;
            StreamWriter writer = new StreamWriter(strFileListPath, false, Encoding.UTF8);

            int nLen = strFolderPath.Length;
            string[] arrFiles = Directory.GetFiles(strFolderPath);

            foreach (string strFile in arrFiles)
            {
                if (strFile == strFileListPath)
                    continue;

                string strFileName = strFile.Substring(nLen + 1);
                writer.WriteLine(strFileName);
            }

            string[] arrFolders = Directory.GetDirectories(strFolderPath);

            foreach (string strFolder in arrFolders)
            {
                ReadFolder(writer, strFolder, strFolderPath);
            }

            writer.Close();
            return strFileListPath;
        }

        private void ReadFolder(StreamWriter writer, string strFolderPath, string strBaseFolderPath)
        {
            int nBaseLength = strBaseFolderPath.Length;
            string[] arrFiles = Directory.GetFiles(strFolderPath);

            foreach (string strFile in arrFiles)
            {
                string strFilePath = strFile.Substring(nBaseLength + 1);
                writer.WriteLine(strFilePath);
            }

            string[] arrFolders = Directory.GetDirectories(strFolderPath);

            foreach (string strFolder in arrFolders)
            {
                ReadFolder(writer, strFolder, strBaseFolderPath);
            }
        }

        private string GetTargetFolder(string strXMLPath, out string strTargetFile)
        {
            XmlTextReader reader = new XmlTextReader(strXMLPath);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(reader);

            strTargetFile = "";
            string strTargetFolder = "";

            foreach (XmlNode node in xmlDoc.ChildNodes)
            {
                if (string.Compare(node.Name, "update", true) == 0)
                {
                    strTargetFolder = ReadUpdate(node, out strTargetFile);
                    break;
                }
            }

            reader.Close();
            return strTargetFolder;
        }

        private string ReadUpdate(XmlNode node, out string strTargetFile)
        {
            strTargetFile = "";

            foreach (XmlNode child in node.ChildNodes)
            {
                if (string.Compare(child.Name, "versions", true) == 0)
                {
                    return ReadVersions(child, out strTargetFile);
                }
            }

            return null;
        }

        private string ReadVersions(XmlNode node, out string strTargetFile)
        {
            strTargetFile = "";

            // VersionID, VersionPath
            Dictionary<string, string> dicVersionPath = new Dictionary<string, string>();
            // VersionID, TargetFile
            Dictionary<string, string> dicVersionTarget = new Dictionary<string, string>();
            string strLastVersionID = "", strTargetVersionFile = "";

            foreach (XmlNode child in node.ChildNodes)
            {
                if (string.Compare(child.Name, "version", true) == 0)
                {
                    string strVersionPath = ReadVersion(child, out strTargetVersionFile);

                    if (strVersionPath != null && strTargetVersionFile != null)
                    {
                        XmlAttribute attr = child.Attributes[0];
                        string strVersionID = attr.Value;
                        dicVersionPath[strVersionID] = strVersionPath;
                        dicVersionTarget[strVersionID] = strTargetVersionFile;
                    }
                }
                else if (string.Compare(child.Name, "lastVersion", true) == 0)
                {
                    strLastVersionID = child.InnerText;
                }
            }

            if (dicVersionTarget.ContainsKey(strLastVersionID))
                strTargetFile = dicVersionTarget[strLastVersionID];

            if (dicVersionPath.ContainsKey(strLastVersionID))
            {
                return dicVersionPath[strLastVersionID];
            }

            return null;
        }

        private string ReadVersion(XmlNode node, out string strTargetFile)
        {
            strTargetFile = null;
            string strLocation = null;

            foreach (XmlNode child in node.ChildNodes)
            {
                if (string.Compare(child.Name, "location", true) == 0)
                {
                    strLocation = child.InnerText;
                }
                else if (string.Compare(child.Name, "target", true) == 0)
                {
                    XmlAttribute attr = child.Attributes[0];
                    strTargetFile = attr.Value;
                }
            }

            return strLocation;
        }

        private void MakeEmpty(string strPath)
        {
            string[] arrFiles = Directory.GetFiles(strPath);

            foreach (string strFile in arrFiles)
            {
                File.Delete(strFile);
            }

            string[] arrFolders = Directory.GetDirectories(strPath);

            foreach (string strFolder in arrFolders)
            {
                Directory.Delete(strFolder, true);
            }
        }

        private bool ExtractToTrg(string strSrcFile, string strTrgPath)
        {
            try
            {
                if (!Directory.Exists(strTrgPath))
                    Directory.CreateDirectory(strTrgPath);

                System.IO.FileStream fs = new System.IO.FileStream(strSrcFile,
                                                     System.IO.FileMode.Open,
                                             System.IO.FileAccess.Read, System.IO.FileShare.Read);

                ICSharpCode.SharpZipLib.Zip.ZipInputStream zis =
                                        new ICSharpCode.SharpZipLib.Zip.ZipInputStream(fs);

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
                FormMain.WriteLog("[ERROR] " + e.Message);
                return false;
            }

            return true;
            //return Core.UZip.ExtractFile(strSrcFile, strTrgPath);
        }

        private string ReadSrc()
        {
            string[] arrFiles = Directory.GetFiles(m_strUpdateSrcPath);

            foreach (string strFile in arrFiles)
            {
                int nIndex = strFile.LastIndexOf('.');

                if (nIndex >= 0)
                {
                    string strExt = strFile.Substring(nIndex + 1);

                    if (string.Compare(strExt, "zip", true) == 0)
                        return strFile;
                }
            }

            return null;
        }
    }
}
