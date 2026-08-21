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
    public partial class FormSimpleData : Form
    {
        private int m_nIntensity = 0;
        private float m_fMagnitude = -1.0f;
        private string m_strLocation = "";

        public int Intensity
        {
            get { return m_nIntensity; }
        }

        public float Magnitude
        {
            get { return m_fMagnitude; }
            set { m_fMagnitude = value; }
        }

        public string Location
        {
            get { return m_strLocation; }
        }

        public FormSimpleData()
        {
            InitializeComponent();
            cboIntensity.SelectedIndex = 8;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            m_nIntensity = cboIntensity.SelectedIndex;
            
            if (float.TryParse(textBoxMagnitude.Text.Trim(), out m_fMagnitude) == false || m_fMagnitude <= 0.0f)
            {
                m_fMagnitude = -1.0f;
            }

            m_strLocation = textBoxLocation.Text.Trim();

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void FormSimpleData_Load(object sender, EventArgs e)
        {
            if (m_fMagnitude > 0.0f)
                textBoxMagnitude.Text = string.Format("{0:F1}", m_fMagnitude);
        }
    }
}
