using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SendSensorResult
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args == null || args.Length < 1)
                return;

            string strEvtID;
            int isReal;

            if (WebServiceManager.GetParameter(args, out strEvtID, out isReal))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain(isReal, strEvtID));
            }
        }
    }
}
