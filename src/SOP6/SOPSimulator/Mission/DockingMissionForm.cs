using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPMonitoringSystem
{
    public partial class DockingMissionForm : Form
    {
        public DockingMissionForm()
        {
            InitializeComponent();
            
        }


        public void AddComponentContents(ComponentContents frmContet)
        {
            this.Controls.Add(frmContet);

        }
    }
}
