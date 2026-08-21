using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDMS_Building
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                MessageBox.Show("사용자 ID가 없습니다.");
            }
            else
            {
                int nSOPGenUserID;
                if (!int.TryParse(args[0], out nSOPGenUserID))
                {
                    MessageBox.Show("사용자 ID는 숫자 타입");
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain(nSOPGenUserID));
            }
        }
    }
}
