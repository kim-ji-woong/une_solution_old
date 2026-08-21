using System;
using System.Collections.Generic;
using System.Windows.Forms;

using GDK;

namespace GDK_tester
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            g2main.app_initialize(G2LANGUAGE.ID.ENGLISH);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new form_admin());

            g2main.app_finalize();
        }
    }
}