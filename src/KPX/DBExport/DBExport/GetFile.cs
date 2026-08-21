using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBExport
{
    public partial class GetFile : Form
    {
        private Timer timer = null;

        public GetFile()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.notifyIcon1.Visible = true;
            this.Hide();
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;

            timer = new Timer();
            timer.Interval = 86400000;
            timer.Tick += timer_Tick;
            timer.Start();
            timer_Tick(null, null);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Download();
        }

        private void Download()
        {
            try
            {
                string downloadWebUrl = "http://192.168.0.182:8080/Download/"; // DBExport.Properties.Settings.Default.downloadWebUrl;
                string downloadLocalPath = DBExport.Properties.Settings.Default.downloadLocalPath;
                if (downloadWebUrl.Length == 0 || downloadLocalPath.Length == 0)
                    return;

                DateTime dt = DateTime.Now;
                string getToday = dt.ToString("yyyyMMdd") + ".zip";
                string getYesterday = dt.AddDays(-1).ToString("yyyyMMdd") + ".zip";

                System.Net.WebClient web = new System.Net.WebClient();

                Uri uri = new Uri(downloadLocalPath);

                CredentialCache credentials = new CredentialCache();
                NetworkCredential netCredential = new NetworkCredential("sop", "sop");
                credentials.Add(uri, "Basic", netCredential);
                web.Credentials = new NetworkCredential("sop", "sop");

                System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(DBExport.Properties.Settings.Default.downloadLocalPath);
                if (!dirInfo.Exists)
                    dirInfo.Create();

                try
                {
                    System.IO.FileInfo fileInfo2 = new System.IO.FileInfo(downloadLocalPath + getYesterday);
                    if (!fileInfo2.Exists)
                        web.DownloadFile(downloadWebUrl + getYesterday, downloadLocalPath + getYesterday);
                }
                catch (Exception ex)
                { 
                }

                try
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(downloadLocalPath + getToday);
                    if (!fileInfo.Exists)
                        web.DownloadFile(downloadWebUrl + getToday, downloadLocalPath + getToday);
                }
                catch (Exception ex)
                {  
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Dispose();
            this.Close();
        }
    }
}
