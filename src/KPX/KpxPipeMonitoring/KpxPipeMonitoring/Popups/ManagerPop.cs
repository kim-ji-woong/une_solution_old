using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KpxPipeMonitoring.Popups
{
    public partial class ManagerPop : Form
    {
        public ManagerPop()
        {
            this.DoubleBuffered = true;
            InitializeComponent();

            FormEditManager.SetDoubleBuffer(dataGridView1, true);
            FormEditManager.SetDoubleBuffer(dataGridView2, true);
        }
    }
}
