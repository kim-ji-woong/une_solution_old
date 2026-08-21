using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BroadRunner
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            SOPBulletin.ModuleManager.Instance.AddRelativePath(".");
			SOPBulletin.ModuleManager.Instance.RegisterModules();

            int nSiteID;
            string strDBName;
            ReadArguments(args, out nSiteID, out strDBName);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain(nSiteID, strDBName));
        }

        static void ReadArguments(string[] args, out int nSiteID, out string strDBName)
        {
            nSiteID = 1;
            strDBName = "SOP4";

            int nCount = args.Count();

            if (nCount >= 1)
            {
                if (!int.TryParse(args[0], out nSiteID))
                    return;

                if (nCount >= 2)
                    strDBName = args[1];
            }
        }
    }
}
