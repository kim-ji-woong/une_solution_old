using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace SOPDisasterSystem
{
    public partial class FormModelLoading : Form
    {
        public static FormModelLoading iForm = new FormModelLoading();
        public FormModelLoading()
        {
            InitializeComponent();
          
        }

        private Form mParent = null;
        
        public void ThreadModal(Form parent)
        {
            mParent = parent;
            timer1.Interval = 1000;
            timer1.Start();
            //Thread t = new Thread(RunThread);
            //t.Start(parent);
        }

        public static void RunThread(object parent)
        {
            ((Form)parent).Invoke((MethodInvoker) delegate
            {
                FormLayout form = (FormLayout)parent;
                form.OpenModel();
            });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            FormLayout form = (FormLayout)mParent;
            form.OpenModel();
           
        }
    }
}
