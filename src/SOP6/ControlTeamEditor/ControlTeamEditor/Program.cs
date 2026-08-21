using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlTeamEditor
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args == null)
                return;

            string strTitle = "";
            System.Drawing.Point ptLocation = new System.Drawing.Point();
            int nSiteID = 1;

            int nCount = args.Count();

            if (nCount == 3)
            {
                strTitle = args[0].Trim();

                string[] tokens = args[1].Split(',');

                if (tokens.Count() == 2)
                {
                    int x, y;

                    if (int.TryParse(tokens[0].Trim(), out x) && int.TryParse(tokens[1].Trim(), out y))
                    {
                        ptLocation.X = x;
                        ptLocation.Y = y;
                    }
                }

                int.TryParse(args[2].Trim(), out nSiteID);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new FormMain());
            //Application.Run(new FormWorkSchedule());
            //Application.Run(new FormWorkSchedule2());
            Application.Run(new FormMemberWorkSchedule(nSiteID));
        }
    }
}
