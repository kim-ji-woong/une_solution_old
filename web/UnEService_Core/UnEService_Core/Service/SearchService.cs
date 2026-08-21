using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnEService_Core.Interface;

namespace UnEService_Core.Service
{
    public class SearchService : ISearch
    {
        private string m_strHelpRootFolder = "";

        private static readonly object _lock = new object();
        private static SearchService instance;
        public static SearchService Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new SearchService();
                    }
                    return instance;
                }
            }
        }

        public SearchService()
        {
            m_strHelpRootFolder = Startup.Configuration.GetSection("AppConfiguration").GetSection("searchHelpRootFolder").Value;
        }

        public bool Search(string strURL, out List<string> files, out List<string> folders)
        {
            files = new List<string>();
            folders = new List<string>();
            Uri uri = new Uri(strURL);

            string strFolder = uri.LocalPath;

            if (strFolder.StartsWith("/"))
                strFolder = strFolder.Substring(1).Replace('/', '\\');

            if (m_strHelpRootFolder.EndsWith("\\"))
                strFolder = m_strHelpRootFolder + strFolder;
            else
                strFolder = m_strHelpRootFolder + "\\" + strFolder;

            if (Directory.Exists(strFolder) == false)
                return false;

            string[] _files = Directory.GetFiles(strFolder);
            
            foreach (string strFile in _files)
            {
                int nIndex = strFile.LastIndexOf('\\');

                if (nIndex > 0)
                {
                    string file = strFile.Substring(nIndex + 1);
                    files.Add(file);
                }
            }

            string[] _folders = Directory.GetDirectories(strFolder);
            
            foreach (string folder in _folders)
            {
                int nIndex = folder.LastIndexOf('\\');
                
                if (nIndex > 0)
                {
                    string _folder = folder.Substring(nIndex + 1);
                    folders.Add(_folder);
                }
            }

            return true;
        }
    }
}
