using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

namespace HelpViewer
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            /*if (!RegUtil.CheckIEOption(Application.ExecutablePath))
                return;*/

            Process process = Process.GetCurrentProcess();
            
            // 이미 같은 Process가 실행중이면 중복 실행되지 않도록 한다.
            if (RunCheckProcess(process.ProcessName, process.Id))
                return;

            OpenOption option = ReadOption(args);

            //Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain(option));
            //Application.Run(new FormFrame(new FormMain(option)));
        }

        static private OpenOption ReadOption(string[] args)
        {
            OpenOption option = new OpenOption();

            int nArgumentCount = args.Count();

            for (int i=0;i<nArgumentCount-1;i+=2)
            {
                string strTagName = args[i];
                string strValue = args[i + 1];

                if (string.Compare(strTagName, "siteid", true) == 0)
                {
                    int nSiteID;

                    if (int.TryParse(strValue, out nSiteID))
                    {
                        option.SiteID = nSiteID;
                        option.Option = OpenOption.URLOption.SITE_ID;
                    }
                }
                else if (string.Compare(strTagName, "url", true) == 0)
                {
                    option.Option = OpenOption.URLOption.URL;
                    option.URL = strValue;
                }
                else if (string.Compare(strTagName, "encoding", true) == 0)
                {
                    int nEncoding;

                    if (int.TryParse(strValue, out nEncoding))
                    {
                        option.Encoding = System.Text.Encoding.GetEncoding(nEncoding);
                    }
                }
                else if (string.Compare(strTagName, "SelectNode", true) == 0)
                {
                    option.SetBeginSelection(OpenOption.SelectionOption.NODE, strValue);
                }
                else if (string.Compare(strTagName, "SelectID", true) == 0)
                {
                    option.SetBeginSelection(OpenOption.SelectionOption.ID, strValue);
                }
                else if (string.Compare(strTagName, "AppName", true) == 0)
                {
                    option.ApplicationName = strValue;
                }
            }

            return option;
        }

        //strProcessName을 가진 프로그램이 실행중인지 체크
        static bool RunCheckProcess(string strProcessName, int nCurrentID)
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.Id == nCurrentID)
                    continue;

                if (process.ProcessName == strProcessName)
                    return true;
            }

            return false;
        }
    }
}
