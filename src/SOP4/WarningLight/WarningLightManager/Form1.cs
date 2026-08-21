using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DBUtility;

namespace WarningLightManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        WebDBManager dbMgr = new WebDBManager(3);
        private void button1_Click(object sender, EventArgs e)
        {
            string szSQL = "UPDATE WarningLight SET CH1 = 1, CH2 = 1 WHERE ID = 609";
            dbMgr.GetResultData(szSQL, 0);   

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string szSQL = "UPDATE WarningLight SET CH1 =0, CH2 = 0 WHERE ID = 609";
            dbMgr.GetResultData(szSQL, 0);   

        }
    }
}
