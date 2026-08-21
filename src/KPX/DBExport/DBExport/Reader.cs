using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBExport
{
    public partial class Reader : Form
    {
        public Reader()
        {
            InitializeComponent();


        }

        private void ReadFile()
        {
            int nIndex = textBox1.Text.LastIndexOf(@"\");
            string fileNameEx = textBox1.Text.Substring(nIndex + 1);

            int nIndexComma = fileNameEx.IndexOf(".");
            string fileName = fileNameEx.Substring(0, nIndexComma);

            int nFileCount = 1;

            string path = textBox2.Text + @"\" + fileName + @"\" + fileName + "_" + nFileCount + ".sql";
            DirectoryInfo dirInfo = new DirectoryInfo(textBox2.Text + @"\" + fileName + @"\");
            if (!dirInfo.Exists)
                dirInfo.Create();
            FileInfo fileInfo = new FileInfo(path);

            StringBuilder sbInsert = new StringBuilder();

            FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            {
                using (StreamReader sr = new StreamReader(textBox1.Text))
                {
                    StringBuilder sbOther = new StringBuilder();
                    string line;
                    int nLineCount = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (nLineCount > 0 && nLineCount % 500000 == 0)
                        {
                            nFileCount++;
                            path = textBox2.Text + @"\" + fileName + @"\" + fileName + "_" + nFileCount + ".sql";

                            sw.Flush();
                            sw.Close();
                            fs.Close();

                            fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                            sw = new StreamWriter(fs);
                            fileInfo = new FileInfo(path);
                        }
                        else
                        {
                            sw.WriteLine(line);
                        }
                        nLineCount++;
                    }
                }
            }
            sw.Flush();
            sw.Close();
            fs.Close();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = DBExport.Properties.Settings.Default.outputPath;
            dialog.Filter = "sql file|*.sql";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = dialog.FileName;
            }
        } 

        private void button2_Click(object sender, EventArgs e)
        {
            string outputPath = DBExport.Properties.Settings.Default.outputPath;
            int nIndex = outputPath.LastIndexOf(@"\");
            string selectedPath = outputPath.Substring(0, nIndex);

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = selectedPath;            
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox2.Text = dialog.SelectedPath;
            } 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0 || textBox2.Text.Length == 0)
                return;

            ReadFile();
        } 
    }
}
