using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace PythonTest
{
    

    public class TextCommander : IDisposable
    {
        public void Dispose()
        {

        }

        public void InitCommander()
        {
            m_CommandThread = null;
        }

        private Thread m_CommandThread = null;
        public bool BeginCommnander(string szTitle = "Runtime Debugger")
        {
            if (m_CommandThread != null)
                return false;

            ConsoleDebugger.Instance.OnCtrlBreak += StopScript;
            ConsoleDebugger.Instance.CodePage = 51949;
            ConsoleDebugger.Instance.Title = szTitle;
            ConsoleDebugger.Instance.CreateConsole();

            PythonLogger logger = ScriptProxy.Instance.Logger;
            if (logger != null)
            {
                List<PythonLogger.Entry> list = logger.GetAll();
                foreach (PythonLogger.Entry entry in list)
                {
                    ConsoleDebugger.WriteLine(entry.ToString());
                }
            }            

            ConsoleDebugger.Write(">> ", ConsoleColor.Red);

            m_CommandThread = new Thread(ConsoleThread);
            m_CommandThread.Start(this);

            return true;
        }

        public void StopCommander()
        {
            ConsoleDebugger.Instance.OnCtrlBreak -= StopScript;
            ConsoleDebugger.Instance.Close();
            m_bExit = true;

            try
            {
                if (m_CommandThread != null)
                {
                    m_CommandThread.Abort();
                    m_CommandThread.Join();
                    m_CommandThread = null;
                }
               
            }
            catch (System.Exception)
            {            	
            }
            
        }

        private bool m_bScriptMode = false;
        public void StopScript()
        {
            m_bScriptMode = false;

            ThreadPool.QueueUserWorkItem((o) =>
            {
                Thread.Sleep(1000);
                var hWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                PostMessage(hWnd, WM_KEYDOWN, VK_RETURN, 0);
            }); 
        }

        private bool m_bExit = false;

        private void ConsoleThread(object param)
        {
            while (!m_bExit)
            {
                try
                {
                    string szText = "";
                    szText = Console.ReadLine();
                    if (szText == null)
                        continue;

                    if (szText.ToLower() == "exit")
                    {
                        ConsoleDebugger.Instance.Close();
                        m_bExit = true;
                    }
                    
                    else if (szText.ToLower() == "scriptbegin")
                    {
                        m_bScriptMode = true;
                        try
                        {
                            StringBuilder sb = new StringBuilder();                            
                            while (m_bScriptMode == true)
                            {
                                szText = Console.ReadLine();
                                sb.AppendLine(szText);
                            }
                            ScriptProxy.Instance.RunPythonScript(sb.ToString());
                            // get output redirect
                            PythonLogger logger = ScriptProxy.Instance.Logger;
                            List<PythonLogger.Entry> list = logger.GetAll();
                            foreach (PythonLogger.Entry entry in list)
                            {
                                string szLog = entry.ToString();
                                if (szLog.Contains("Fault"))
                                {
                                    ConsoleDebugger.WriteLine(entry.ToString(), ConsoleColor.Red);
                                }
                                else
                                    ConsoleDebugger.WriteLine(entry.ToString(), ConsoleColor.Green);
                            }
                        }
                        catch (System.Exception)
                        {
                            m_bScriptMode = false;
                        }
                        ConsoleDebugger.Write(">> ", ConsoleColor.Red);
                    }
                    else
                    {
                        ConsoleDebugger.WriteLine(szText);

                        //if (szText.Contains("import ") && !szText.Contains("sys"))
                        //{
                            
                        //    // Run Command
                        //    string szModule = szText.Replace("import ", "");
                        //    ScriptProxy.Instance.ImportModule(szModule);

                        //}
                        //else
                        {
                            // Run Command
                            ScriptProxy.Instance.RunPythonScript(szText);
                        }
                       
                        // get output redirect
                        PythonLogger logger = ScriptProxy.Instance.Logger;
                        List<PythonLogger.Entry> list = logger.GetAll();
                        foreach (PythonLogger.Entry entry in list)
                        {
                            string szLog = entry.ToString();
                            if (szLog.Contains("Fault"))
                            {
                                ConsoleDebugger.WriteLine(entry.ToString(), ConsoleColor.Red);
                            }
                            else
                                ConsoleDebugger.WriteLine(entry.ToString(), ConsoleColor.Green);
                        }
                        // print text



                        ConsoleDebugger.Write(">> ", ConsoleColor.Red);

                    }
                }
                catch (System.Exception ex)
                {
                    m_bExit = true;
                    ConsoleDebugger.Instance.Close();

                    Debug.WriteLine(ex.StackTrace);
                    Debug.WriteLine(ex.Message);
                }
            }

            m_CommandThread = null;
        }

        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        const int VK_RETURN = 0x0D;
        const int WM_KEYDOWN = 0x100;

    }


}
