using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;


namespace PythonTest
{
    public delegate void OnCtrlBreak();

	public class ConsoleDebugger
	{
        public event OnCtrlBreak OnCtrlBreak;
 
		public static ConsoleDebugger m_Instance = null;
		public static ConsoleDebugger Instance
		{
			get 
			{
				if( m_Instance == null)
					m_Instance = new ConsoleDebugger();
				return m_Instance;
			}
		}

		private int m_nCodePage = 51949;
		public int CodePage
		{
			get { return m_nCodePage; }
			set 
			{
                m_nCodePage = value;
                if (m_bInitConsole == false)
                    return;
                try
                {                    
                    Encoding encoding = System.Text.Encoding.GetEncoding(m_nCodePage);
                    Console.OutputEncoding = encoding;  
                }
                catch (System.Exception)
                {                	
                }				              
			}
        }

        private string m_szTitle = "Python Debugger";
        public string Title
        {
            get { return m_szTitle; }
            set
            {
                m_szTitle = value;
                if (m_bInitConsole == false)
                    return;
                try
                {
                    Console.Title = m_szTitle;
                }
                catch (System.Exception)
                {

                }

            }
        }

        private static bool m_bInitConsole = false;
        
        public void Close()
        {
            if(m_bInitConsole == true)
                FreeConsole();
            m_bInitConsole = false;            
        }

        
        
        public static void Write(bool value ,ConsoleColor c = ConsoleColor.White )
        {
            if( m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ResetColor();
        }

        public static void Write(char value ,ConsoleColor c = ConsoleColor.White )
        {
            if( m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ResetColor();
        }

        public static void Write(char[] buffer ,ConsoleColor c = ConsoleColor.White )
        {
            if( m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.Write(buffer);
            Console.ResetColor();
        }

        public static void Write(decimal value,ConsoleColor c = ConsoleColor.White)
        {
            if( m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ResetColor();
        }

        public static void Write(double value,ConsoleColor c = ConsoleColor.White)
        {
            if( m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ResetColor();
        }
        public static void Write(float value,ConsoleColor c = ConsoleColor.White)
        {
            if( m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ResetColor();
        }
        public static void Write(int value,ConsoleColor c = ConsoleColor.White)
        {
            if( m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ResetColor();
        }
        public static void Write(long value,ConsoleColor c = ConsoleColor.White)
        {
            if( m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ResetColor();
        }
        public static void Write(object value,ConsoleColor c = ConsoleColor.White)
        {
            if( m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ResetColor();
        }
        public static void Write(string value,ConsoleColor c = ConsoleColor.White)
        {
            if( m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ResetColor();
        }

        public static void Write(uint value,ConsoleColor c = ConsoleColor.White)
        {
            if( m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ResetColor();
        }
        public static void Write(ulong value,ConsoleColor c = ConsoleColor.White)
        {
            if( m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ResetColor();
        }

        public static void Write(string format, object arg0, ConsoleColor c = ConsoleColor.White)
        {
            if (m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.Write(format, arg0);
            Console.ResetColor();
        }
        public static void Write(string format, object[] arg,ConsoleColor c = ConsoleColor.White)
        {
            if (m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.Write(format, arg);
            Console.ResetColor();
        }
        public static void Write(char[] buffer, int index, int count,ConsoleColor c = ConsoleColor.White)
        {
            if (m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.Write(buffer, index, count);
            Console.ResetColor();
        }

        public static void Write(string format, object arg0, object arg1,ConsoleColor c = ConsoleColor.White)
        {
            if (m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.Write(format, arg0, arg1);
            Console.ResetColor();
        }
        public static void Write(string format, object arg0, object arg1, object arg2,ConsoleColor c = ConsoleColor.White)
        {
            if (m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.Write(format, arg0, arg1, arg2);
            Console.ResetColor();
        }
        public static void Write(string format, object arg0, object arg1, object arg2, object arg3, ConsoleColor c = ConsoleColor.White)
        {
            if (m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.Write(format, arg0, arg1, arg2, arg3);
            Console.ResetColor();
        }

        public static void WriteLine()
        {
            if (m_bInitConsole == false)
                return;
            Console.WriteLine();
        }
        public static void WriteLine(bool value, ConsoleColor c = ConsoleColor.White)
        {
            Write(value, c);
            WriteLine();
        }

        public static void WriteLine(char value, ConsoleColor c = ConsoleColor.White)
        {
            Write(value, c);
            WriteLine();
        }
        public static void WriteLine(char[] buffer, ConsoleColor c = ConsoleColor.White)
        {
            Write(buffer, c);
            WriteLine();
        }

        public static void WriteLine(decimal value, ConsoleColor c = ConsoleColor.White)
        {
            Write(value, c);
            WriteLine();
        }
        public static void WriteLine(double value, ConsoleColor c = ConsoleColor.White)
        {
            Write(value, c);
            WriteLine();
        }
        public static void WriteLine(float value, ConsoleColor c = ConsoleColor.White)
        {
            Write(value, c);
            WriteLine();
        }
        public static void WriteLine(int value, ConsoleColor c = ConsoleColor.White)
        {
            Write(value, c);
            WriteLine();
        }
        public static void WriteLine(long value, ConsoleColor c = ConsoleColor.White)
        {
            Write(value, c);
            WriteLine();
        }
        public static void WriteLine(object value, ConsoleColor c = ConsoleColor.White)
        {
            Write(value, c);
            WriteLine();
        }
        public static void WriteLine(string value, ConsoleColor c = ConsoleColor.White)
        {
            Write(value, c);
            WriteLine();
        }
        public static void WriteLine(uint value, ConsoleColor c = ConsoleColor.White)
        {
            Write(value, c);
            WriteLine();
        }
        public static void WriteLine(ulong value, ConsoleColor c = ConsoleColor.White)
        {
            Write(value, c);
            WriteLine();
        }

        public static void WriteLine(string format, object arg0, ConsoleColor c = ConsoleColor.White)
        {
            Write(format, arg0, c);
            WriteLine();
        }

        public static void WriteLine(string szFormat, object[] objs, ConsoleColor c = ConsoleColor.White)
        {
            if (m_bInitConsole == false)
                return;

            Console.ForegroundColor = c;
            Console.WriteLine(szFormat, objs);
            Console.ResetColor();
        }

        public static void WriteLine(char[] buffer, int index, int count, ConsoleColor c = ConsoleColor.White)
        {
            Write(buffer, index, count, c);
            WriteLine();
        }
        public static void WriteLine(string format, object arg0, object arg1, ConsoleColor c = ConsoleColor.White)
        {
            Write(format, arg0, arg1, c);
            WriteLine();
        }
        public static void WriteLine(string format, object arg0, object arg1, object arg2, ConsoleColor c = ConsoleColor.White)
        {
            Write(format, arg0, arg1, arg2, c);
            WriteLine();
        }
        public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3, ConsoleColor c = ConsoleColor.White)
        {
            Write(format, arg0, arg1, arg2, arg3, c);
            WriteLine();
        }      		
		

		public ConsoleDebugger()
		{            
		}

        private bool HandlerEventRoutine(uint dwControlType)
        {
            if (dwControlType == 0)
            {
                if (OnCtrlBreak != null)
                {
                    OnCtrlBreak();

                }
                return true;
            }
            if (dwControlType == 2)
                return false;

            return true;
        }
             

		public void CreateConsole()
		{
			AllocConsole();

            SetConsoleCtrlHandler(HandlerEventRoutine, true);

            //SafeFileHandle safeFileHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            //FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            //Encoding encoding = System.Text.Encoding.GetEncoding(m_nCodePage);
            //StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
            //standardOutput.AutoFlush = true;
            //Console.SetOut(standardOutput);
                        

   			Console.Title = m_szTitle;

            

			InitConsoleHandles();

            EnableCloseButton(GetSystemMenu(GetConsoleWindow(), false), false);
            
		}

		private void InitConsoleHandles()
		{
            SafeFileHandle hStdOut, hStdErr, hStdOutDup, hStdErrDup, hStdIn, hStdInDup;
			BY_HANDLE_FILE_INFORMATION bhfi;
			hStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
			hStdErr = GetStdHandle(STD_ERROR_HANDLE);
            hStdIn = GetStdHandle(STD_INPUT_HANDLE);

			// Get current process handle
			IntPtr hProcess = Process.GetCurrentProcess().Handle;
			// Duplicate Stdout handle to save initial value
			DuplicateHandle(hProcess, hStdOut, hProcess, out hStdOutDup,
			0, true, DUPLICATE_SAME_ACCESS);
			// Duplicate Stderr handle to save initial value
            DuplicateHandle(hProcess, hStdErr, hProcess, out hStdErrDup,
            0, true, DUPLICATE_SAME_ACCESS);

            DuplicateHandle(hProcess, hStdIn, hProcess, out hStdInDup,
            0, true, DUPLICATE_SAME_ACCESS);
			// Attach to console window – this may modify the standard handles
			AttachConsole(ATTACH_PARENT_PROCESS);
			// Adjust the standard handles
			if (GetFileInformationByHandle(GetStdHandle(STD_OUTPUT_HANDLE), out bhfi))
			{
				SetStdHandle(STD_OUTPUT_HANDLE, hStdOutDup);
			}
			else
			{
				SetStdHandle(STD_OUTPUT_HANDLE, hStdOut);
			}
            if (GetFileInformationByHandle(GetStdHandle(STD_ERROR_HANDLE), out bhfi))
            {
                SetStdHandle(STD_ERROR_HANDLE, hStdErrDup);
            }
            else
            {
                SetStdHandle(STD_ERROR_HANDLE, hStdErr);
            }

            if (GetFileInformationByHandle(GetStdHandle(STD_INPUT_HANDLE), out bhfi))
            {
                SetStdHandle(STD_INPUT_HANDLE, hStdInDup);
            }
            else
            {
                SetStdHandle(STD_INPUT_HANDLE, hStdIn);
            }

            m_bInitConsole = true;
            
		}

        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32")]
        static extern bool SetConsoleCtrlHandler(HandlerRoutine HandlerRoutine, bool Add);
        delegate bool HandlerRoutine(uint dwControlType);
		[DllImport("kernel32.dll")]
		static extern bool AllocConsole();
		[DllImport("kernel32.dll")]
		static extern bool AttachConsole(UInt32 dwProcessId);
        [DllImport("Kernel32")]
        public static extern void FreeConsole();
		[DllImport("kernel32.dll")]
		private static extern bool GetFileInformationByHandle(SafeFileHandle hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);
		[DllImport("kernel32.dll")]
		private static extern SafeFileHandle GetStdHandle(UInt32 nStdHandle);
		[DllImport("kernel32.dll")]
		private static extern bool SetStdHandle(UInt32 nStdHandle, SafeFileHandle hHandle);
		[DllImport("kernel32.dll")]
        private static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, SafeFileHandle hSourceHandle, IntPtr hTargetProcessHandle, out SafeFileHandle lpTargetHandle, UInt32 dwDesiredAccess, Boolean bInheritHandle, UInt32 dwOptions);


        private const UInt32 ATTACH_PARENT_PROCESS = 0xFFFFFFFF;
        private const UInt32 STD_OUTPUT_HANDLE = 0xFFFFFFF5;
        private const UInt32 STD_ERROR_HANDLE = 0xFFFFFFF4;
        private const UInt32 STD_INPUT_HANDLE = 0xFFFFFFF6;
        private const UInt32 DUPLICATE_SAME_ACCESS = 2;

        struct BY_HANDLE_FILE_INFORMATION
        {
            public UInt32 FileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWriteTime;
            public UInt32 VolumeSerialNumber;
            public UInt32 FileSizeHigh;
            public UInt32 FileSizeLow;
            public UInt32 NumberOfLinks;
            public UInt32 FileIndexHigh;
            public UInt32 FileIndexLow;
        }
      
        internal const UInt32 SC_CLOSE = 0xF060;
        internal const UInt32 MF_ENABLED = 0x00000000;
        internal const UInt32 MF_GRAYED = 0x00000001;
        internal const UInt32 MF_DISABLED = 0x00000002;
        internal const uint MF_BYCOMMAND = 0x00000000;


        private static void EnableCloseButton(IntPtr menuHandle, bool bEnabled)
        {            
            EnableMenuItem(menuHandle, SC_CLOSE, (uint)(MF_ENABLED | (bEnabled ? MF_ENABLED : MF_GRAYED)));
        }

	}
}
