using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MultiLoader
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            List<string> files = ReadFile();

            foreach (string strPath in files)
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = strPath;
                startInfo.WorkingDirectory = Application.StartupPath;
                startInfo.ErrorDialog = true;
                startInfo.Arguments = "";

                System.Diagnostics.Process process;
                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }
            }
        }

        static List<string> ReadFile()
        {
            string strPath = Application.StartupPath + "\\MultiLoader.ini";

            List<string> files = new List<string>();

            try
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(strPath, System.Text.Encoding.UTF8);

                while (!reader.EndOfStream)
                {
                    string strLine = reader.ReadLine().Trim();

                    if (strLine.Length == 0)
                        continue;

                    files.Add(strLine);
                }

                reader.Close();
            }
            catch (Exception)
            {
            }

            return files;
        }
    }
}
