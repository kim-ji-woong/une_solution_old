using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;

namespace UnEService
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드, svc 및 config 파일에서 클래스 이름 "SearchService"을 변경할 수 있습니다.
    // 참고: 이 서비스를 테스트하기 위해 WCF 테스트 클라이언트를 시작하려면 솔루션 탐색기에서 SearchService.svc나 SearchService.svc.cs를 선택하고 디버깅을 시작하십시오.
    public class SearchService : ISearch
    {
        private string m_strHelpRootFolder = "";

        public SearchService()
        {
            m_strHelpRootFolder = System.Configuration.ConfigurationManager.AppSettings["Search.HelpRootFolder"].ToString();
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
