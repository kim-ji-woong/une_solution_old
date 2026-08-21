using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace IronPython
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Owner Form Create
            Form1 f = new Form1();

            // Init Text commander
            f.Commander.InitCommander();

            // Create Console and Begin Input Thread
            if (f.Commander.BeginCommnander())
            {
                // Create Python Context
                f.AddPythonFunction();
            } 
            Application.Run(f);
            Environment.Exit(0);
        }
    }
}
