using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sample.SampleLib
{
    public partial class FormYoutube : Form
    {
        private string m_strURL = "";

        public FormYoutube(string url)
        {
            InitializeComponent();
            m_strURL = url;
            this.TopLevel = false;
        }

        private void FormYoutube_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate(m_strURL);
        }
    }
}
