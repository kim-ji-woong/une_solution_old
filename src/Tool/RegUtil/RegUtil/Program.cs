using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace RegUtil
{
    static class Program
    {
        private static void Run(string strFilePath, string strRegPath)
        {
            if (!File.Exists(strFilePath))
                return;

            StreamReader reader = new StreamReader(strFilePath, System.Text.Encoding.UTF8);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();
                if (strLine.Length == 0)
                    continue;

                string strMajorVersion = strLine.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
                strMajorVersion = strMajorVersion.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

                int nIndex = strRegPath.LastIndexOf('\\');

                if (nIndex >= 0)
                {
                    string strRegKey = strRegPath.Substring(0, nIndex);

                    if (!Editor.IsValidPath(strRegKey))
                    //if (!Editor.IsValidPath(HKeyType.CURRENT_USER, @"Software\UnE\Update Info"))
                    {
                        if (!Editor.InsertKey(strRegKey))
                        //if (!Editor.InsertKey(HKeyType.CURRENT_USER, @"Software\UnE\Update Info"))
                        {
                            reader.Close();
                            return;
                        }
                    }

                    Editor.SetValue(strRegPath, strMajorVersion);
                    //Editor.SetValue(HKeyType.CURRENT_USER, @"Software\UnE\Update Info\Current", strMajorVersion);
                }

                break;
            }

            reader.Close();
        }

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] argc)
        {
            if (argc == null || argc.Length < 2)
            {
                System.Windows.Forms.MessageBox.Show("argc is empty");
                return;
            }

            string strRegPath = "";

            for (int i = 1; i < argc.Length; i++)
            {
                if (strRegPath.Length == 0)
                    strRegPath = argc[i];
                else
                    strRegPath += " " + argc[i];
            }

            Run(argc[0], strRegPath);
            //bool isValid = Editor.IsValidPath(HKeyType.CURRENT_USER, @"Software\UnE\Update Info\InstallDate");
            //bool isSuccess = Editor.DeletePath(HKeyType.CURRENT_USER, @"Software\UnE\Update Info\test");
            //bool isSuccess = Editor.InsertKey(HKeyType.CURRENT_USER, @"Software\UnE\Update Info\test");
            //bool isSuccess = Editor.SetValue(HKeyType.CURRENT_USER, @"Software\UnE\Update Info\test\testValue", "1234");
        }
    }
}
