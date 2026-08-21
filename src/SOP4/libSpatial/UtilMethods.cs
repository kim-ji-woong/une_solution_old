using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnE.Util
{
    public class UtilMethods
    {
        public static int ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                throw new Exception("Site ID가 지정되지 않았습니다. ini파일을 확인하세요. 실행 오류");                
            }

            int nSiteId = 1;
            if (int.TryParse(szSiteID, out nSiteId))
            {
                return nSiteId;
            }
            else
            {
                throw new Exception("잘못된 Site ID입니다.. ini파일을 확인하세요. 실행 오류");
            }
        }

        public static void DeleteFolder(string strFolderPath)
        {
            string[] arrFiles = System.IO.Directory.GetFiles(strFolderPath);

            foreach (string strFile in arrFiles)
            {
                System.IO.File.Delete(strFile);
            }

            string[] arrFolders = System.IO.Directory.GetDirectories(strFolderPath);

            foreach (string strFolder in arrFolders)
            {
                DeleteFolder(strFolder);
            }

            System.IO.Directory.Delete(strFolderPath);
        }


        public static void KillProcess(string strProcessName)
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName == strProcessName && process.HasExited == false)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception)
                    { }
                    //break;
                }
            }
        }
    }
}
