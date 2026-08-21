using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace IronPython
{
    
    public class TextCommander : IDisposable
    {
        private Form m_OwnerForm = null;
        private ITextCommanderOwner m_Owner = null;
        private Thread m_CommandThread = null;
        private bool m_bExit = false;
        private bool m_bScriptMode = false;
        private bool m_bInitCommander = false;

        public bool IsInit
        {
            get { return m_bInitCommander; }
        }

        public void Dispose()
        {
        }
        
        public TextCommander(ITextCommanderOwner ownerForm)
        {
            try
            {
                m_Owner = ownerForm;
                m_OwnerForm = (Form)ownerForm;
            }
            catch (System.Exception)
            {
                throw new Exception("Form must be the owner of the TextCommanderOwner!!");
            }            
        }

        public void InitCommander()
        {           
            m_CommandThread = null;
            m_bInitCommander = true;
        }
                
        public bool BeginCommnander(string szTitle = "Runtime Debugger")
        {
            if (m_CommandThread != null)
                return false;

            ConsoleDebugger.Instance.OnCtrlBreak += StopScript;
            ConsoleDebugger.Instance.CodePage = 51949;
            ConsoleDebugger.Instance.Title = szTitle;
            ConsoleDebugger.Instance.CreateConsole();
             
            ConsoleDebugger.WriteLine("Init Python Runtime Commander", ConsoleColor.Green);
            ConsoleDebugger.Write(">> ", ConsoleColor.Red);

            m_bExit = false;
            m_CommandThread = new Thread(ConsoleThread);
            m_CommandThread.Start(this);

            return true;
        }

        public void StopCommander()
        {
            m_bInitCommander = false;
            ConsoleDebugger.Instance.OnCtrlBreak -= StopScript;
            ConsoleDebugger.Instance.Close();
            m_bExit = true;

            try
            {
                if (m_CommandThread != null)
                {
                    m_CommandThread.Abort();
                    //m_CommandThread.Join();
                    m_CommandThread = null;
                }
               
            }
            catch (System.Exception)
            {            	
            }            
        }
               
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
        private bool m_bLoggerOn = false;
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

                    string szTextLower = szText.ToLower().Trim();
                    
                    if (szTextLower == "exit")
                    {
                        ConsoleDebugger.Instance.Close();
                        m_bExit = true;
                    }
                    else if (szTextLower == "cls")
                    {
                        ConsoleDebugger.Instance.Clear();
                        ConsoleDebugger.WriteLine("Init Python Runtime Commander", ConsoleColor.Green);
                        ConsoleDebugger.Write(">> ", ConsoleColor.Red);
                    }
                    else if (szTextLower == "hide")
                    {
                        ConsoleDebugger.Instance.ShowConsole(false);
                        ConsoleDebugger.Write(">> ", ConsoleColor.Red);
                    }

                    else if (szTextLower == "loggeroff")
                    {
                        m_bLoggerOn = false;

                        if (m_Owner != null)
                            m_Owner.SetConsoleLog(false);
                        ConsoleDebugger.Write(">> ", ConsoleColor.Red);
                        
                    }
                    else if (szTextLower == "loggeron")
                    {
                        m_bLoggerOn = true;
                        if (m_Owner != null)
                            m_Owner.SetConsoleLog(true);
                        ConsoleDebugger.Write(">> ", ConsoleColor.Red);
                    }

                    else if (szTextLower == "scriptbegin")
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

                            m_OwnerForm.Invoke((MethodInvoker)delegate
                            {
                                ScriptProxy.Instance.RunPythonScript(sb.ToString());
                            });
                            // get output redirect
                            
                            
                        }
                        catch (System.Exception)
                        {
                            m_bScriptMode = false;
                        }
                        
                        if (m_bLoggerOn == false)
                        {
                            // Loggor가 Off인경우 Fault는 출력해준다.
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

                        ConsoleDebugger.Write(">> ", ConsoleColor.Red);
                    }
                    else
                    {
                        ConsoleDebugger.WriteLine(szText);

                        try
                        {
                            m_OwnerForm.Invoke((MethodInvoker)delegate
                            {
                                // Run Command
                                ScriptProxy.Instance.RunPythonScript(szText);
                            });
                        }
                        catch(Exception)
                        { }
                       

                        // Loggor가 Off인경우 Fault는 출력해준다.
                        if(m_bLoggerOn == false)
                        {
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
                        // print text
                        ConsoleDebugger.Write(">> ", ConsoleColor.Red);
                    }
                }
                catch (IronPython.Runtime.UnboundNameException exx)
                {
                    ScriptProxy.Instance.Logger.AddFault(exx);
                    
                    
                    ConsoleDebugger.WriteLine(exx.Message);
                    ConsoleDebugger.Write(">> ", ConsoleColor.Red);
                }
                catch (System.IO.IOException ex)
                {
                    //if (m_OwnerForm != null && m_OwnerForm.IsHandleCreated)
                    {
                        //m_OwnerForm.Invoke((MethodInvoker)delegate
                        //{
                            //ConsoleDebugger.Instance.CreateConsole();
                            //ConsoleDebugger.Write(">> ", ConsoleColor.Red);
                            m_bExit = false;
                            ScriptProxy.Instance.Logger.AddFault(ex);
                       // });
                    } 

                }
                catch (Exception e)
                {
                    ScriptProxy.Instance.Logger.AddFault(e);                   
                }
            }

            m_CommandThread = null;
        }

        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        private const int VK_RETURN = 0x0D;
        private const int WM_KEYDOWN = 0x100;

    }


}
