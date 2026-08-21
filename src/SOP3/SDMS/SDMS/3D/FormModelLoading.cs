using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace SDMS
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
        }

        public static void RunThread(object parent)
        {
            ((Form)parent).Invoke((MethodInvoker) delegate
            {
                FormContent form = (FormContent)parent;
                form.OpenModel();
            });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            FormContent form = (FormContent)mParent;
            form.OpenModel();
           
        }
    }
}
