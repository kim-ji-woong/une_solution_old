using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace SensorMonitor
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>

        // 모듈이름, 폴더를 명시하면 로드됩니다.
        static string[] m_LoadModules = 
        {
            //"IronPython","python",
            //"IronPython.Modules", "python",
            //"Microsoft.Dynamic", "python", 
            //"Microsoft.Scripting","python",              
            //"Microsoft.Scripting.Metadata", "python"
        };

        // 폴더를 명시하면 dll을 검색하여 로드합니다.
        static string[] m_Add_Path =
        {
            "python",
            "common",
            "SOP"
        };

        [STAThread]
        static void Main(string[] args)
        {


            try
            {
                log4net.Config.XmlConfigurator.Configure();
            }
            catch (System.Exception)
            {
            }

            bool bAddedPath = AddPath();
            if (bAddedPath == true)
            {
                // Application.Restart();
            }

            string strServerAddr = null;
            string strTitle = null;

            if (args.Count() > 0)
                strServerAddr = args[0];

            if (args.Count() > 1)
                strTitle = args[1];

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain(strServerAddr, strTitle));
        }

        static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string strPath = Application.ExecutablePath;
            string szPath = Path.GetDirectoryName(strPath);
            string name = args.Name.Substring(0, args.Name.IndexOf(','));

            if (m_LoadModules.Length > 0)
            {
                for (int i = 0; i < m_LoadModules.Length; i += 2)
                {
                    if (name.Equals(m_LoadModules[i]))
                    {
                        return Assembly.LoadFile(szPath + "\\" + m_LoadModules[i + 1] + "\\" + name + ".dll");
                    }
                }
            }
            for (int i = 0; i < m_Add_Path.Length; i++)
            {
                string szFileName = szPath + "\\" + m_Add_Path[i] + "\\" + name + ".dll";
                if (File.Exists(szFileName))
                {
                    return Assembly.LoadFile(szFileName);
                }
            }
            return null;
        }

        static bool AddPath()
        {
            bool bAddedPath = false;
            string szPath = Environment.GetEnvironmentVariable("Path");

            for (int i = 0; i < m_Add_Path.Length; i++)
            {
                string szFileName = "..\\" + m_Add_Path[i] + ";";
                if (!szPath.Contains(szFileName))
                {
                    if (szPath.Length == 0)
                        szPath += szFileName;
                    else
                    {
                        char c = szPath[szPath.Length - 1];
                        if (c == ';')
                        {
                            szPath += szFileName;
                        }
                        else
                        {
                            szPath += ";";
                            szPath += szFileName;
                        }
                    }
                    bAddedPath = true;
                }
            }

            if (bAddedPath == true)
            {
                Environment.SetEnvironmentVariable("Path", szPath, EnvironmentVariableTarget.User);
            }
            return bAddedPath;
        }
    }
}
