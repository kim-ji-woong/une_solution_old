using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnEService_Core
{
    public class AppConfiguration
    {
        public string Host { get; set; }
        public string Id { get; set; }
        public string Pw { get; set; }
        public string CharSet { get; set; }
        public string TransactionTimeout { get; set; }
        public string UploadFolder { get; set; }
        public string LogFolder { get; set; }
        public string LogLifeTime { get; set; }
        public string LogFileTag { get; set; }
        public string SearchHelpRootFolder { get; set; }
    }
}
