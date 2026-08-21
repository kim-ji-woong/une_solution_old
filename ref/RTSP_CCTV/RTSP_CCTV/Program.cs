using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTSP_CCTV
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //ReadFile(@"F:\Project\SOP\에너지산업현장훈련\2차년도\안전한국훈련\sop_3_kwanggyo.sql");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }

        //static void ReadFile(string strPath)
        //{
        //    System.IO.StreamWriter writer = new System.IO.StreamWriter("c:/temp/test.txt", false, System.Text.Encoding.UTF8);
        //    System.IO.StreamReader reader = new System.IO.StreamReader(strPath, System.Text.Encoding.UTF8);
        //    int nLineCount = 0, nLineTarget = 5000;

        //    while (reader.EndOfStream == false)
        //    {
        //        string strLine = reader.ReadLine().Trim();

        //        if (strLine.Length == 0)
        //            continue;

        //        if (strLine.Contains("INSERT INTO `psmsensorvalue"))
        //            continue;

        //        nLineCount++;

        //        if (strLine.Contains("INSERT INTO `zone"))
        //            writer.WriteLine(strLine);
        //        //System.Diagnostics.Trace.WriteLine(strLine);

        //        //if (nLineTarget == nLineCount)
        //        //    break;
        //    }

        //    reader.Close();
        //    writer.Close();
        //}
    }
}
