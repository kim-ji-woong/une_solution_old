using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadMan
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string strProjectFile = null;

            if (args.Count() > 0)
                strProjectFile = GetPath(args[0]);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormFrame(new FormMain(strProjectFile)));
        }

        static string GetPath(string strPath)
        {
            if (strPath.StartsWith("\""))
            {
                strPath = strPath.Substring(1);

                if (strPath.EndsWith("\""))
                    strPath = strPath.Substring(0, strPath.Length - 1);
            }

            strPath = strPath.Trim();

            if (strPath.Length == 0)
                return null;

            return strPath;
        }
    }
}
