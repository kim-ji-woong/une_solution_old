using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SiteRegister
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            List<Site> sites = new List<Site>();

            // 삼천포
            sites.Add(new Site(1, "SOP_1", Site.DBType.sqlserver));
            // 영흥
            //sites.Add(new Site(2, "SOP_2", Site.DBType.sqlserver));
            // 광교
            sites.Add(new Site(3, "SOP_3", Site.DBType.mysql));
            // 서울대
            sites.Add(new Site(100, "EDU_100", Site.DBType.mysql));
            // 부산대
            sites.Add(new Site(101, "EDU_101", Site.DBType.sqlserver));
            // 충남대
            sites.Add(new Site(102, "EDU_102", Site.DBType.sqlserver));
            // 신한은행
            sites.Add(new Site(200, "BLD_200", Site.DBType.sqlserver));

            foreach (Site site in sites)
            {
                site.Write();
            }

            System.Diagnostics.Trace.WriteLine("Code Page : " + System.Text.Encoding.UTF8.CodePage);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }
    }
}
