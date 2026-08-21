using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            g2main.app_initialize(G2LANGUAGE.ID.ENGLISH);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new form_watch());

            g2main.app_finalize();
        }
    }
}
