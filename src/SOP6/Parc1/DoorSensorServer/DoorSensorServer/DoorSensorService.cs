using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DoorSensorServer
{
    partial class DoorSensorService : ServiceBase
    {
        private List<IThreadObject> m_threadItems = new List<IThreadObject>();
        private bool m_shutdownThread = false;

        public DoorSensorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 여기에 서비스를 시작하는 코드를 추가합니다.

            if (!Directory.Exists(@"C:\Temp"))
                Directory.CreateDirectory(@"C:\Temp");

            StreamWriter sw = new StreamWriter(@"C:\Temp\ServiceState_Door.txt", true);
            sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Door Sensor Service Start");
            sw.Flush();

            m_threadItems.Add(new TeamReader());

            Thread t = new Thread(new ThreadStart(MainThread));
            t.Start();
        }

        protected override void OnStop()
        {
            // TODO: 서비스를 중지하는 데 필요한 작업을 수행하는 코드를 여기에 추가합니다.
            m_shutdownThread = true;
        }

        private void MainThread()
        {
            while (m_shutdownThread == false)
            {
                foreach (IThreadObject obj in m_threadItems)
                {
                    if (m_shutdownThread)
                        break;

                    obj.Run();
                }

                Thread.Sleep(1000);
            }
        }
    }
}
