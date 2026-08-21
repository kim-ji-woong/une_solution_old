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
    public partial class FormStartEventOptionSnow : Form
    {
        private WorkflowOptionSnowFall m_option = null;

        public FormStartEventOptionSnow(WorkflowOptionSnowFall option)
        {
            InitializeComponent();
            m_option = option;

            if (m_option != null)
            {
                groupBoxAmountSnowfall.Visible = m_option.UseAmountSnowFall;

                if (m_option.AmountSnowFall > 0.0)
                    textBoxAmountSnowfall.Text = string.Format("{0:F1}", m_option.AmountSnowFall);
            }
        }

        private void textBoxAmountSnowfall_TextChanged(object sender, EventArgs e)
        {
            string strAmountSnowFall = textBoxAmountSnowfall.Text;

            if (strAmountSnowFall.Length == 0)
                m_option.AmountSnowFall = 0.0;
            else
            {
                double dAmountSnowFall;

                if (!double.TryParse(strAmountSnowFall.Trim(), out dAmountSnowFall))
                    m_option.AmountSnowFall = 0.0;
                else
                    m_option.AmountSnowFall = dAmountSnowFall;
            }
        }
    }
}
