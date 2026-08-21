using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace TeamEditor
{
    static class Program
    {
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

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string strMutexName = "SOPSingle-TeamEditor";
            System.Threading.Mutex mutex = new System.Threading.Mutex(true, strMutexName);

            // 1초 동안 뮤텍스를 획득하려 대기
            TimeSpan tsWait = new TimeSpan(0, 0, 1);
            bool success = mutex.WaitOne(tsWait);

            // 실패하면 프로그램 종료
            if (!success)
            {
                return;
            }

            if (args == null || args.Count() < 3)
            {
                args = new string[] { "1", "1", "1" };
            }

            bool bAddedPath = AddPath();
            if (bAddedPath == true)
            {
                // Application.Restart();
            }

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);

            int nSOPGenUserID = -1, nSiteID = -1;

            if (args.Length < 3)
            {
                MessageBox.Show("프로그램 실행인자가 없습니다.");
                return;
            }

            string strSOPGenUserRealName = args[2];

            for (int i = 3; i < args.Length; i++)
            {
                strSOPGenUserRealName += " " + args[i];
            }

            if (!int.TryParse(args[0], out nSOPGenUserID) || !int.TryParse(args[1], out nSiteID))
            {
                MessageBox.Show("잘못된 실행인자입니다.\r\n프로그램을 종료합니다.");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormFrame(new FormMain(nSOPGenUserID, strSOPGenUserRealName, nSiteID)));
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
