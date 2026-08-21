using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.SOP.Workstate;

namespace SOPMonitoringSystem.Popup
{
    public partial class FormStartEventOptionEarthquake : Form
    {
        private WorkflowOptionEarthquake m_option = null;

        public FormStartEventOptionEarthquake(WorkflowOptionEarthquake option)
        {
            InitializeComponent();
            m_option = option;

            if (m_option != null)
            {
                if (m_option.Magnitude > 0.0)
                    textBoxMagnit.Text = string.Format("{0:F1}", m_option.Magnitude);

                string strItem = m_option.Intensity.ToString();
                int nIndex = cboIntensity.Items.IndexOf(strItem);

                if (nIndex >= 0)
                    cboIntensity.SelectedIndex = nIndex;
            }
        }

        private void cboIntensity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_option != null)
            {
                if (cboIntensity.SelectedIndex < 0)
                    m_option.Intensity = 0;
                else
                    m_option.Intensity = (int)int.Parse(cboIntensity.Text);
            }
        }

        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            if (m_option != null)
            {
                if (radioMagnit.Checked)
                {
                    m_option.Mode = WorkflowOptionEarthquake.PowerMode.Magnitude;
                    textBoxMagnit.Enabled = true;
                    cboIntensity.Enabled = false;
                }
                else if (radioIntens.Checked)
                {
                    m_option.Mode = WorkflowOptionEarthquake.PowerMode.Intensity;
                    textBoxMagnit.Enabled = false;
                    cboIntensity.Enabled = true;
                }
                else
                {
                    m_option.Mode = WorkflowOptionEarthquake.PowerMode.Unknown;
                    textBoxMagnit.Enabled = false;
                    cboIntensity.Enabled = false;
                }
            }
        }

        private void textBoxMagnit_TextChanged(object sender, EventArgs e)
        {
            string strMagnitude = textBoxMagnit.Text;

            if (strMagnitude.Length == 0)
                m_option.Magnitude = 0.0;
            else
            {
                double dMagnitude;

                if (!double.TryParse(strMagnitude.Trim(), out dMagnitude))
                    m_option.Magnitude = 0.0;
                else
                    m_option.Magnitude = dMagnitude;
            }
        }
    }
}
