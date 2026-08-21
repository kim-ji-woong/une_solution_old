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

namespace DoorSensorServer
{
    public partial class Form1 : Form
    {
        private List<IThreadObject> m_threadItems = new List<IThreadObject>();
        private bool m_shutdownThread = false;

        public Form1()
        {
            InitializeComponent();

            m_threadItems.Add(new TeamReader());

            Thread t = new Thread(new ThreadStart(MainThread));
            t.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
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
