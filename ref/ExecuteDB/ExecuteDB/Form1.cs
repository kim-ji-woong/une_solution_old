using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Data.SqlClient;

namespace ExecuteDB
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            ExecuteDB();
        }

        private void ExecuteDB()
        {
            string strPath1 = @"G:\공유 드라이브\_UNE_DATA_20_DEV_DATA_설치\SOP\솔브레인\DB\20241115\WSOP10_모든데이터.sql";

            for (int i=0;i<10;i++)
            {
                //string strPath = strPath1 + string.Format("_{0:00}.sql", i);
                Thread t = new Thread(new ParameterizedThreadStart(RunDB));
                t.Start(strPath1);
            }
        }

        private void RunDB(object arg)
        {
            SqlConnection connection = new SqlConnection("Data Source=127.0.0.1;Initial Catalog=WSOP_10;User ID=sa;Password=9449966Ab");
            connection.Open();

            string strPath = arg.ToString();
            StreamReader reader = new StreamReader(strPath, Encoding.UTF8);

            bool writeLog = false;

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                if (strLine.StartsWith("INSERT [dbo].[SdmsHistorySensorZone]") || strLine.StartsWith("INSERT [dbo].[SdmsHistorySensorReaction]"))
                    continue;

                string strLower = strLine.ToLower();

                if (strLower.StartsWith("use") || strLower.StartsWith("go") || strLower.StartsWith("/*"))
                    continue;

                try
                {
                    SqlCommand command = new SqlCommand(strLine, connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }

                if (!writeLog)
                {
                    System.Diagnostics.Trace.WriteLine(strPath);
                    writeLog = true;
                }
            }

            connection.Close();
            System.Diagnostics.Trace.WriteLine("DB Updated!!!");
        }

        private int GetID(string strLine)
        {
            int index = strLine.IndexOf('(');

            if (index > 0)
            {
                index = strLine.IndexOf('(', index + 1);

                if (index > 0)
                {
                    int index2 = strLine.IndexOf(',', index + 1);

                    if (index2 > index)
                    {
                        string strID = strLine.Substring(index + 1, index2 - index - 1).Trim();

                        int id;

                        if (int.TryParse(strID, out id))
                            return id;
                    }
                }
            }

            return -1;
        }

        private void DivideFile(int nFileCount, string strPath)
        {
            StreamReader reader = new StreamReader(strPath);
            int nLineCount = 0;

            while (!reader.EndOfStream)
            {
                reader.ReadLine();
                nLineCount++;
            }

            reader.Close();

            reader = new StreamReader(strPath, Encoding.UTF8);
            int nLineIndex = 0;

            for (int i = 0; i < nFileCount; i++)
            {
                WriteFile(strPath, i, reader, nFileCount, nLineCount, ref nLineIndex);
            }

            reader.Close();
        }

        private void WriteFile(string strPath, int nFileIndex, StreamReader reader, int nFileCount, int nTotalLineCount, ref int nLineIndex)
        {
            int nDotIndex = strPath.LastIndexOf('.');

            string strFilePath = strPath.Substring(0, nDotIndex);
            string strExt = strPath.Substring(nDotIndex);
            string strPath2 = string.Format("{0}_{1:00}{2}", strFilePath, nFileIndex, strExt);

            StreamWriter writer = new StreamWriter(strPath2, false, Encoding.UTF8);
            int nLineCount = nTotalLineCount / nFileCount + 1;

            for (int i = nLineIndex; i < nLineIndex + nLineCount; i++)
            {
                string strLine = reader.ReadLine();
                writer.WriteLine(strLine);

                if (reader.EndOfStream)
                {
                    writer.Close();
                    nLineIndex = i + 1;
                    return;
                }
            }

            writer.Close();
            nLineIndex += nLineCount;
        }
    }
}
