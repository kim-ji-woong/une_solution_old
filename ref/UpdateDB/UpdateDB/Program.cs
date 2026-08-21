using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using DBUtility;

namespace UpdateDB
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //string strPath = @"F:\Project\SOP\삼천포 유해화학물질\DB\Backup\20160504\only_psmsensorvalues\only_psmsensorvalues.sql";
            string strPath = Application.StartupPath + "\\script.sql";
            LoadScript(strPath);
        }

        static void LoadScript(string strPath)
        {
            if (!File.Exists(strPath))
            {
                MessageBox.Show("파일이 존재하지 않습니다.\r\n" + strPath);
                return;
            }

            int nSiteID = ReadSiteID();
            StreamReader reader = new StreamReader(strPath);
            WebDBManager dbMgr = new WebDBManager(nSiteID);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string strLower = strLine.ToLower();

                if (strLower.StartsWith("use") || strLower == "go")
                    continue;

                if (dbMgr.GetResultData(strLine, 0) == null)
                {
                    MessageBox.Show("DB Query에 오류가 있습니다.\r\n" + strLine);
                    reader.Close();
                    return;
                }
            }

            reader.Close();
            MessageBox.Show("DB Update 완료\r\n스마트 재난관리 시스템을 재시작하여 주십시오.");
        }

        static private int ReadSiteID()
        {
            int nSiteID = -1;

            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                System.Diagnostics.Trace.WriteLine("Site ID가 지정되지 않았습니다. ini파일을 확인하세요");
                Application.Exit();
                return nSiteID;
            }

            if (!int.TryParse(szSiteID, out nSiteID))
            {
                System.Diagnostics.Trace.WriteLine("잘못된 Site ID입니다.. ini파일을 확인하세요");
                Application.Exit();
                return nSiteID;
            }

            return nSiteID;
        }
    }
}
