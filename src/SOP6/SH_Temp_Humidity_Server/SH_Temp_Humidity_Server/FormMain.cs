using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;

namespace SH_Temp_Humidity_Server
{
    using Data;

    public partial class FormMain : Form
    {
        private AlarmManager m_alarmManager = new AlarmManager();

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            m_alarmManager.Start();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_alarmManager.Stop();
        }
    }
}
