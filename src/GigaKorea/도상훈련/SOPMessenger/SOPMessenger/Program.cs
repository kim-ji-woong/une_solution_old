using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FireSimulator;

namespace SOPMessenger
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
            {
                MessageBox.Show("서버에 전달할 인자가 입력되지 않았습니다.\r\n예)SOPMessender 3");
                return;
            }

            int nParam;
            string strParam = args[0].Trim();

            if (int.TryParse(strParam, out nParam) == false)
            {
                MessageBox.Show(string.Format("잘못된 인자가 입력되었습니다.\r\n{0}에서 {1} 사이의 정수만 입력 가능합니다.", TCP_ID.REPORT_FIRE, TCP_ID.CLEAR_FINEDUST2));
                return;
            }

            if (nParam < (int)TCP_ID.REPORT_FIRE || nParam > (int)TCP_ID.CLEAR_FINEDUST2)
            {
                MessageBox.Show(string.Format("잘못된 인자가 입력되었습니다.\r\n{0}에서 {1} 사이의 정수만 입력 가능합니다.", TCP_ID.REPORT_FIRE, TCP_ID.CLEAR_FINEDUST2));
                return;
            }

            int nPort = 6000;
            string strServer = System.Configuration.ConfigurationSettings.AppSettings["server"];

            ClientProvider provider = new ClientProvider();

            if (provider.Connect(strServer, nPort))
            {
                provider.SendData(nParam);
                provider.Close();
            }
        }
    }
}
