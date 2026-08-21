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
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;


namespace UnE.SenarioMaker
{
    internal delegate void OnCtrlBreak();

    internal class ConsoleDebugger
	{
		public event OnCtrlBreak OnCtrlBreak;

        private static object m_InterLock = new object();

        private SafeFileHandle hStdOut, hStdErr, hStdIn;
 
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
        public bool IsInitConsole
        {
            get { return m_bInitConsole; }          
        }

        private int m_nTop = 0;
        public int Top
        {
            get { return m_nTop; }
            set { m_nTop = value; }
        }

        private int m_nLeft = 0;
        public int Left
        {
            get { return m_nLeft; }
            set { m_nLeft = value; }
        }


		public void Close()
		{
            try
            {
                if (m_bInitConsole == true)
                {
                    FreeConsole();

                    if (hStdOut != null && hStdOut.IsClosed == false)
                        if (hStdOut.IsInvalid == true)
                            hStdOut.Close();
                    if (hStdErr != null && hStdErr.IsClosed == false)
                        if (hStdErr.IsInvalid == true)
                            hStdErr.Close();
                    if (hStdIn != null && hStdIn.IsClosed == false)
                        if (hStdIn.IsInvalid == true)
                            hStdIn.Close();
                }
            }
			catch(Exception)
            {

            }
			m_bInitConsole = false;            
		}
        	
		
		public static void Write(bool value ,ConsoleColor c = ConsoleColor.White )
		{
			if( m_bInitConsole == false)
				return;

            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ForegroundColor = color;
		}

		public static void Write(char value ,ConsoleColor c = ConsoleColor.White )
		{
			if( m_bInitConsole == false)
				return;

            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ForegroundColor = color;
		}

		public static void Write(char[] buffer ,ConsoleColor c = ConsoleColor.White )
		{
			if( m_bInitConsole == false)
				return;

            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.Write(buffer);
            Console.ForegroundColor = color;
		}

		public static void Write(decimal value,ConsoleColor c = ConsoleColor.White)
		{
			if( m_bInitConsole == false)
				return;

            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ForegroundColor = color;
		}

		public static void Write(double value,ConsoleColor c = ConsoleColor.White)
		{
			if( m_bInitConsole == false)
				return;

            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ForegroundColor = color;
		}
		public static void Write(float value,ConsoleColor c = ConsoleColor.White)
		{
			if( m_bInitConsole == false)
				return;

            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ForegroundColor = color;
		}
		public static void Write(int value,ConsoleColor c = ConsoleColor.White)
		{
			if( m_bInitConsole == false)
				return;

            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ForegroundColor = color;
		}
		public static void Write(long value,ConsoleColor c = ConsoleColor.White)
		{
			if( m_bInitConsole == false)
				return;

            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ForegroundColor = color;
		}
		public static void Write(object value,ConsoleColor c = ConsoleColor.White)
		{
			if( m_bInitConsole == false)
				return;

            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ForegroundColor = color;
		}
		public static void Write(string value,ConsoleColor c = ConsoleColor.White)
		{

			if( m_bInitConsole == false)
				return;

            lock (m_InterLock)
            {
                try
                {
                    ConsoleColor color = Console.ForegroundColor;
                    Console.ForegroundColor = c;
                    Console.Write(value);
                    Console.ForegroundColor = color;
                }
                catch (System.Exception)
                {
                	
                }
                
            }
            
		}

		public static void Write(uint value,ConsoleColor c = ConsoleColor.White)
		{
			if( m_bInitConsole == false)
				return;

            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ForegroundColor = color;
		}
		public static void Write(ulong value,ConsoleColor c = ConsoleColor.White)
		{
			if( m_bInitConsole == false)
				return;

            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.Write(value);
            Console.ForegroundColor = color;
		}

		public static void Write(string format, object arg0, ConsoleColor c = ConsoleColor.White)
		{
			if (m_bInitConsole == false)
				return;
            ConsoleColor color = Console.ForegroundColor;
       
			Console.ForegroundColor = c;
			Console.Write(format, arg0);
            Console.ForegroundColor = color;
		}
		public static void Write(string format, object[] arg,ConsoleColor c = ConsoleColor.White)
		{
			if (m_bInitConsole == false)
				return;

            ConsoleColor color = Console.ForegroundColor;

            Console.ForegroundColor = c;
            Console.Write(format, arg);
            Console.ForegroundColor = color;
		}
		public static void Write(char[] buffer, int index, int count,ConsoleColor c = ConsoleColor.White)
		{
			if (m_bInitConsole == false)
				return;
            ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = c;
			Console.Write(buffer, index, count);
            Console.ForegroundColor = color;
		}

		public static void Write(string format, object arg0, object arg1,ConsoleColor c = ConsoleColor.White)
		{
			if (m_bInitConsole == false)
				return;
            ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = c;
			Console.Write(format, arg0, arg1);
            Console.ForegroundColor = color;
		}
		public static void Write(string format, object arg0, object arg1, object arg2,ConsoleColor c = ConsoleColor.White)
		{
			if (m_bInitConsole == false)
				return;
            ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = c;
			Console.Write(format, arg0, arg1, arg2);
            Console.ForegroundColor = color;
		}
		public static void Write(string format, object arg0, object arg1, object arg2, object arg3, ConsoleColor c = ConsoleColor.White)
		{
			if (m_bInitConsole == false)
				return;
            ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = c;
			Console.Write(format, arg0, arg1, arg2, arg3);
            Console.ForegroundColor = color;
		}

		public static void WriteLine()
		{
			if (m_bInitConsole == false)
				return;
            try
            {
                Console.WriteLine();
            }
            catch (System.Exception)
            {
            	
            }
			
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
			if (dwControlType == 0 || dwControlType == 1)
			{
				if (OnCtrlBreak != null)
				{
					OnCtrlBreak();
				}
				return true;
			}
			if (dwControlType == 2 || dwControlType == 5 || dwControlType == 6)
				return false;

			return true;
		}

        private bool m_bEnableLogger = false;
        public bool EnableLogger
        {
            get { return m_bEnableLogger; }
            set { m_bEnableLogger = value; }
        }

     

        static HandlerRoutine _handler;


        private IntPtr ptrConsole;
        private bool m_bCreatedConsole = false;
        public bool CreatedConsole
        {
            get { return m_bCreatedConsole; }
        }

		public void CreateConsole(bool bGUI = true)
		{
            InitConsoleHandles(bGUI);
            _handler += new HandlerRoutine(HandlerEventRoutine);

            SetConsoleCtrlHandler(_handler, true);

            ptrConsole = GetConsoleWindow();
            
            int style = GetWindowLong(ptrConsole, GWL_EXSTYLE);
            style &= ~WS_EX_APPWINDOW;
            SetWindowLong(ptrConsole, GWL_EXSTYLE, style);
            
            ShowWindow(ptrConsole, SW_HIDE);

            Console.Title = m_szTitle;
            
            EnableCloseButton(GetSystemMenu(ptrConsole, false), false);
			Console.OpenStandardInput();
            SetWindowPos(ptrConsole, 0, m_nLeft, m_nTop, 0, 0, SWP_NOSIZE);

            

            m_bInitConsole = true;
		}
        
        
        public void Clear()
        {
            Console.Clear();
        }

        public void ShowConsole(bool bShow)
        {
            if (m_bInitConsole == true)
            {
                if (bShow == true)
                {
                    ShowWindow(ptrConsole, SW_SHOW);
                }
                else
                {
                    ShowWindow(ptrConsole, SW_HIDE);
                }
            }
            
        }
       

		private void InitConsoleHandles(bool bGUI = true)
		{
			Close();

			UInt32 nResult = 1;
            if(bGUI == true)
            {
 
                nResult = AllocConsole();
                //ptrConsole = GetConsoleWindow();
                //ShowWindow(ptrConsole, SW_HIDE);
            }
            else
            {
                AttachConsole(ATTACH_PARENT_PROCESS);
                //ptrConsole = GetConsoleWindow();
                //ShowWindow(ptrConsole, SW_HIDE);
                        
            }
            
            hStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
            hStdErr = GetStdHandle(STD_ERROR_HANDLE);
            hStdIn = GetStdHandle(STD_INPUT_HANDLE);
          
            if (nResult != 0)
            {
                SetStdHandle(STD_OUTPUT_HANDLE, hStdOut);
                SetStdHandle(STD_ERROR_HANDLE, hStdErr);
                SetStdHandle(STD_INPUT_HANDLE, hStdIn);

                
            }
            else
            {
                UInt32 nError = GetLastError();
                Debug.WriteLine(nError);
            }
			
		}
        public static int WS_EX_APPWINDOW = 0x40000;
        public static int GWL_EXSTYLE = -20;   

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
		[DllImport("user32.dll")]
		static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

		[DllImport("kernel32")]
		static extern UInt32 GetLastError();
		[DllImport("kernel32")]
		static extern bool SetConsoleCtrlHandler(HandlerRoutine HandlerRoutine, bool Add);
		delegate bool HandlerRoutine(uint dwControlType);
		[DllImport("kernel32.dll")]
		static extern UInt32 AllocConsole();
		[DllImport("kernel32.dll")]
		static extern UInt32 AttachConsole(UInt32 dwProcessId);
		[DllImport("Kernel32")]
		public static extern void FreeConsole();
		[DllImport("kernel32.dll")]
		private static extern SafeFileHandle GetStdHandle(UInt32 nStdHandle);
		[DllImport("kernel32.dll")]
		private static extern bool SetStdHandle(UInt32 nStdHandle, SafeFileHandle hHandle);
		[DllImport("kernel32.dll")]
		private static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, SafeFileHandle hSourceHandle, IntPtr hTargetProcessHandle, out SafeFileHandle lpTargetHandle, UInt32 dwDesiredAccess, Boolean bInheritHandle, UInt32 dwOptions);

		[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
		public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOW = 5;

        private const int SWP_NOSIZE = 0x0001;
		private const UInt32 ATTACH_PARENT_PROCESS = 0xFFFFFFFF;
		private const UInt32 STD_OUTPUT_HANDLE = 0xFFFFFFF5;
		private const UInt32 STD_ERROR_HANDLE = 0xFFFFFFF4;
		private const UInt32 STD_INPUT_HANDLE = 0xFFFFFFF6;
		private const UInt32 DUPLICATE_SAME_ACCESS = 2;

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
