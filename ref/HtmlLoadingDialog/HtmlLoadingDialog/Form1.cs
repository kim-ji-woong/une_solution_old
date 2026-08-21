using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HtmlLoadingDialog
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string strPath = Application.StartupPath;
            webBrowser1.Navigate(strPath + "\\htmlpage\\help1.html");
        }
    }
}
