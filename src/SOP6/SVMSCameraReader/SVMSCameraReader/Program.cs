using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SVMSCameraReader
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args == null || args.Count() == 0)
                return;

            int nSiteID;

            if (int.TryParse(args[0], out nSiteID) == false)
            {
                MessageBox.Show("잘못된 전달인자입니다.");
                return;
            }

            int nPort = 0;

            if (args.Count() >= 2)
            {
                int.TryParse(args[1], out nPort);
            }

            // 초
            int nTimeout = 10;

            if (args.Count() >= 3)
            {
                int.TryParse(args[2], out nTimeout);
            }

            DBManager mgr = new DBManager(nSiteID, nPort);

            if (mgr.UpdateCCTVList())
            {
                nTimeout *= 10;

                for (int i=0;i<nTimeout;i++)
                {
                    System.Threading.Thread.Sleep(100);

                    if (mgr.CloseApp)
                        return;
                }

                if (nPort > 0)
                    Network.UDPClient.SendMessage(Network.Header.TimeoutClose, null, nPort);
            }
        }
    }
}
