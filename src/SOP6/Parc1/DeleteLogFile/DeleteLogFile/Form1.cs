using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeleteLogFile
{
    public partial class Form1 : Form
    {
        private StreamWriter m_sw = null;

        public Form1()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.ContextMenuStrip = contextMenuStrip1;
        }

        private string[] GetFileNames()
        {
            string[] paths = DeleteLogFile.Properties.Settings.Default.DeletePath.Split('|');

            return paths;
        }

        private void DeleteFile(string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return;

            try
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    if (!File.Exists(paths[i]))
                        continue;

                    
                    FileInfo fi = new FileInfo(paths[i]);

                    File.Delete(paths[i]);
                    WriteLog("Delete File : " + paths[i] + " / Length : " + fi.Length);
                    
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_sw = new StreamWriter(Application.StartupPath + "\\DeleteLogfile.txt", true);

            string[] paths = GetFileNames();
            DeleteFile(paths);

            this.Close();
        }

        private void WriteLog(string txt)
        {
            m_sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + txt);
            m_sw.Flush();
        }
    }
}
