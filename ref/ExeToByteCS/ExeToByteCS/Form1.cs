using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Diagnostics;


namespace ExeToByteCS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string szFileName = openFileDialog1.FileName;

                Read(szFileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private static readonly int CHUNK_SIZE = 1024;

        public void Read(String file)
        {
            String strPath = Application.StartupPath + "\\result.txt";
            if (File.Exists(strPath) == true)
            {
                File.Delete(strPath);
            }
           
            StreamWriter WriteFile = new StreamWriter(strPath, true, Encoding.Unicode);
            WriteFile.Write("byte [] fileData = {\n");
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs, new ASCIIEncoding()))
                {
                    byte[] chunk;

                    chunk = br.ReadBytes(CHUNK_SIZE);
                    while (chunk.Length > 0)
                    {
                        DumpBytes(chunk, chunk.Length, WriteFile);
                        chunk = br.ReadBytes(CHUNK_SIZE);
                    }
                }
            }
            WriteFile.Write("\n }; ");
            WriteFile.Close();
            WriteFile.Dispose();
        }

        public void DumpBytes(byte[] bdata, int len, StreamWriter WriteFile)
        {        
            int i;
            int j = 0;
            StringBuilder dumptext = new StringBuilder();
            for (i = 0; i < len; i++)
            {
                dumptext.Append(string.Format("0x{0:X2},", (int)bdata[i]));
                j++;
                if (j == 16)
                {
                    WriteFile.Write(dumptext);
                    WriteFile.Write("\n");                    
                    dumptext.Length = 0;
                    j = 0;
                }                
            }           
        }
    }
}
