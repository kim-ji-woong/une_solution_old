using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EarthquakeSensorServer
{
    public partial class FormEarthquakeSimulation : Form
    {
        private int m_nIntensity = 0;
        private string m_strLocation = "";

        public int Intensity
        {
            get { return m_nIntensity; }
        }

        public string EarthLocation
        {
            get { return m_strLocation; }
        }

        public FormEarthquakeSimulation()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cboIntensity.SelectedIndex < 0)
                m_nIntensity = 0;
            else
                m_nIntensity = cboIntensity.SelectedIndex + 1;

            m_strLocation = textBoxLocation.Text.Trim();

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
