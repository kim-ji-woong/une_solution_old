using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using UnEService;

namespace UnEServiceHost
{
    public partial class FormMain : Form
    {
        private ServiceHost m_hostWebDB = null;
        private ServiceHost m_hostUpload = null;
        private ServiceHost m_hostDownload = null;
        private ServiceHost m_hostSearch = null;

        public FormMain()
        {
            InitializeComponent();

            m_hostWebDB = new ServiceHost(typeof(UnEService.WebDBService));
            m_hostWebDB.Open();

            m_hostUpload = new ServiceHost(typeof(UnEService.UploadService));
            m_hostUpload.Open();

            m_hostDownload = new ServiceHost(typeof(UnEService.DownloadService));
            m_hostDownload.Open();

            m_hostSearch = new ServiceHost(typeof(UnEService.SearchService));
            m_hostSearch.Open();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_hostWebDB.Close();
            m_hostUpload.Close();
            m_hostDownload.Close();
            m_hostSearch.Close();
        }
    }
}
