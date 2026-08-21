using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TTSSimulator
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 5)
                return;

            bool isSiren = args[0] == "0" ? false : true;
            int nRepeat = 0;

            int.TryParse(args[1], out nRepeat);

            string strServerName = GetMessage(args[2]);
            string strPort = GetMessage(args[3]);
            string strMessage = GetMessage(args[4]);
            string strResultFilePath = args.Length >= 6 ? GetMessage(args[5]) : "";

            Broadcaster.Run(strServerName, strPort, isSiren, nRepeat, strMessage, strResultFilePath);
            
            ApplicationContext ctx = new ApplicationContext();
            ctx.ThreadExit += new EventHandler(ExitMain);
            
            Application.Exit();
        }

        static string GetMessage(string strMsg)
        {
            if (strMsg.StartsWith("\""))
            {
                strMsg = strMsg.Substring(1);

                if (strMsg.EndsWith("\""))
                    strMsg = strMsg.Substring(0, strMsg.Length - 1);
            }

            return strMsg;
        }

        static void ExitMain(object sender, EventArgs e)
        {
            try
            {
                foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcesses())
                {
                    if (process.ProcessName.ToUpper().StartsWith("TTSSimulator"))
                    {
                        process.Kill();
                        break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
