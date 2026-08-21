using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SQLServerToMySQL
{
    public partial class FormMain : Form
    {
        private string[] m_strFilePathList = null;

        public FormMain()
        {
            InitializeComponent();

            labelResult.Text = "";
        }

        private void FormMain_DragDrop(object sender, DragEventArgs e)
        {
            labelResult.Text = "";

            if (m_strFilePathList != null)
            {
                int nFileCount = 0;

                foreach (string strFilePath in m_strFilePathList)
                {
                    if (strFilePath.Length > 0)
                    {
                        string strMySQLFilePath = GetMySQLFilePath(strFilePath);
                        ToMySQL(strFilePath, strMySQLFilePath);
                        nFileCount++;
                    }
                }

                labelResult.Text = string.Format("변환결과 : {0}개 파일 변환 완료", nFileCount);
            }
        }

        private void FormMain_DragEnter(object sender, DragEventArgs e)
        {
            if (GetFileName(e))
                e.Effect = DragDropEffects.Copy;
        }

        private string GetMySQLFilePath(string strSQLServerFilePath)
        {
            string strFilePath = "";
            int nIndex1 = strSQLServerFilePath.LastIndexOf('\\');
            int nIndex2 = strSQLServerFilePath.LastIndexOf('.');

            if (nIndex1 >= 0 && nIndex2 > nIndex1)
            {
                string strFileName = strSQLServerFilePath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                strFilePath = strSQLServerFilePath.Substring(0, nIndex1 + 1) + strFileName + "_MySQL" + strSQLServerFilePath.Substring(nIndex2);
            }
            else if (nIndex1 >= 0)
            {
                string strFileName = strSQLServerFilePath.Substring(nIndex1 + 1);
                strFilePath = strSQLServerFilePath.Substring(0, nIndex1 + 1) + strFileName + "_MySQL";
            }
            else if (nIndex2 >= 0)
            {
                string strFileName = strSQLServerFilePath.Substring(0, nIndex2);
                strFilePath = strFileName + "_MySQL" + strSQLServerFilePath.Substring(nIndex2);
            }
            else
            {
                strFilePath = strSQLServerFilePath + "_MySQL";
            }

            return strFilePath;
        }

        protected bool GetFileName(DragEventArgs e)
        {
            bool ret = false;

            m_strFilePathList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (m_strFilePathList != null && m_strFilePathList.Count() > 0)
                ret = true;

            return ret;
        }

        private void ToMySQL(string strMSSQLPath, string strMySQLPath)
        {
            Encoding encoding = GetEncoding(strMSSQLPath);
            StreamReader reader = new StreamReader(strMSSQLPath, encoding);
            StreamWriter writer = new StreamWriter(strMySQLPath, false, Encoding.UTF8);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();
                string strLineLow = strLine.ToLower();

                if (strLineLow.Length == 0 || strLineLow.StartsWith("use") || strLineLow.StartsWith("go"))
                    continue;

                strLine = strLine.Replace("[dbo].", "");
                strLine = strLine.Replace("[", "");
                strLine = strLine.Replace("]", "");

                writer.WriteLine(strLine + ";");
            }

            writer.Close();
            reader.Close();
        }

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;

            // EUC-KR : 51949
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            return encEUC_KR;
            //return Encoding.ASCII;
        }


    }
}
