using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace libSplash
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //int nServerPort = 0;
            long callWindowHandle = 0;
            int nProcessID = 0;
            string strInitFilePath = null;

            if (args.Count() >= 1)
            {
                long.TryParse(args[0].Trim(), out callWindowHandle);
                //int.TryParse(args[0].Trim(), out nServerPort);
            }

            if (args.Count() >= 2)
            {
                int.TryParse(args[1].Trim(), out nProcessID);
            }

            if (args.Count() >= 3)
                strInitFilePath = args[2];

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormSplash((IntPtr)callWindowHandle, nProcessID, strInitFilePath));
            //Application.Run(new FormSplash(nServerPort, strInitFilePath));
        }
    }
}
