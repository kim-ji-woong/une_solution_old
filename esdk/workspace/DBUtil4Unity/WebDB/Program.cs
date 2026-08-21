using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using System.Collections;
using System.IO;

namespace WebDB
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            int nSiteID = int.Parse(args[0]);
            RunQuery(nSiteID, args[1], args[2]);
        }

        private static void RunQuery(int nSiteID, string strFileName, string strSQL)
        {
            WebDBManager dbMgr = new WebDBManager(nSiteID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            WriteFile(arrResult, strFileName);
        }

        private static void WriteFile(ArrayList arrResult, string strFileName)
        {
            string strFilePath = Application.StartupPath + "\\" + strFileName + "_temp.txt";
            string strFileTargetPath = Application.StartupPath + "\\" + strFileName + ".txt";
            StreamWriter writer = new StreamWriter(strFilePath, false, System.Text.Encoding.UTF8);

            if (arrResult == null)
                writer.Write("null");
            else
            {
                int nResultCount = arrResult.Count;

                for (int i=0;i<nResultCount;i++)
                {
                    string strLine = arrResult[i].ToString().Trim();

                    if (i == nResultCount - 1)
                        writer.Write(strLine);
                    else
                        writer.WriteLine(strLine);
                }
            }

            writer.Close();

            File.Delete(strFileTargetPath);
            File.Move(strFilePath, strFileTargetPath);
        }
    }
}
