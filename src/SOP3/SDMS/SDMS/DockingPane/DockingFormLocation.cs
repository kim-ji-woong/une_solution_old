using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SDMS
{
    public partial class DockingFormLocation : Form
    {
        public DockingFormLocation()
        {
            InitializeComponent();
            SetPOI(null);
        }

        public void SetPOI(POI poi)
        {
            if (poi == null || poi.Zone == null)
                labelLocation.Text = "";
            else
                labelLocation.Text = poi.Zone.BroadcastName;
        }
    }
}
