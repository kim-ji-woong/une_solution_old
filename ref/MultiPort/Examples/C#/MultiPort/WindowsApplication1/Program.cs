using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsApplication1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            try
            {
                log4net.Config.XmlConfigurator.Configure();
            }
            catch (System.Exception)
            {
            }

            char[] bytes = new char[]{ (char)0x50, (char)0x4F, (char)0x4C };
            string str = "";

            str += bytes[0];
            str += bytes[1];
            str += bytes[2];

            System.Diagnostics.Trace.WriteLine(str);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}