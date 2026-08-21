using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutbreakInfo
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            int nArgs = 0;
            long nTime = 0;

            string strProjectName = "";
            string strLevelName = "";
            string strSpaceName = "";
            string strTime = "";

            DateTime dtTime;

            nArgs = args.Count();
            if (nArgs != 4)
                return;

            strProjectName = args[0];
            strLevelName = args[1];
            strSpaceName = args[2];
            strTime = args[3];
            nTime = Convert.ToInt64(strTime);

            dtTime = DateTime.FromBinary(nTime);


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain(strProjectName, strLevelName, strSpaceName, dtTime));
        }
    }
}
