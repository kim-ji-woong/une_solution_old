using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace HwpEXEproject
{

    class Program
    {

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.SysInt)]
		private static extern IntPtr GetStdHandle(int handle);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle(IntPtr handle);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FreeConsole();

		static void HideConsole()
		{
			var ptr = GetStdHandle(-11);
			if (!CloseHandle(ptr))
				throw new Win32Exception();

			ptr = IntPtr.Zero;

			if (!FreeConsole())
				throw new Win32Exception();
		}

        [STAThread]
        static void Main()
        {
			//HideConsole();
			try
			{				
				ApplicationContext ctx = new ApplicationContext();
				ctx.ThreadExit += new EventHandler(ExitMain);
				Form1 form1 = new Form1();
				form1.CreateHWP();
				ctx.MainForm = form1;
				//Application.Run(ctx);

				form1.Dispose();
				Application.Exit();
				
			}
			catch (System.Exception ex)
			{			
			}
					

			
	    }


        static void ExitMain(object sender, EventArgs e)
        {
            try
            {
				foreach (Process process in Process.GetProcesses())
				{
                    
					if (process.ProcessName.ToUpper().StartsWith("HwpExeProject"))              
					{
						process.Kill();
					}
				}

            }
            catch
            (Exception)
            {
            }
        }

    }
}
