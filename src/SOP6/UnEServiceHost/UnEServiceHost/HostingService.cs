using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using UnEService;

namespace UnEServiceHost
{
    partial class HostingService : ServiceBase
    {
        private ServiceHost m_hostWebDB = null;
        private ServiceHost m_hostUpload = null;
        private ServiceHost m_hostDownload = null;
        private ServiceHost m_hostSearch = null;

        public HostingService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 여기에 서비스를 시작하는 코드를 추가합니다.
            m_hostWebDB = new ServiceHost(typeof(UnEService.WebDBService));
            m_hostWebDB.Open();

            m_hostUpload = new ServiceHost(typeof(UnEService.UploadService));
            m_hostUpload.Open();

            m_hostDownload = new ServiceHost(typeof(UnEService.DownloadService));
            m_hostDownload.Open();

            m_hostSearch = new ServiceHost(typeof(UnEService.SearchService));
            m_hostSearch.Open();
        }

        protected override void OnStop()
        {
            // TODO: 서비스를 중지하는 데 필요한 작업을 수행하는 코드를 여기에 추가합니다.
            m_hostWebDB.Close();
            m_hostUpload.Close();
            m_hostDownload.Close();
            m_hostSearch.Close();
        }
    }
}
