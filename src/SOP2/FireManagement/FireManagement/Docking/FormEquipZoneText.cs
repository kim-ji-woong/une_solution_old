using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FireManagement.Docking
{
    public partial class FormEquipZoneText : Form
    {
        private DXFViewer.Text m_text = null;
        private DXFViewer.DXFControl m_dxfControl = null;

        public FormEquipZoneText(DXFViewer.DXFControl dxfControl, DXFViewer.Text text = null)
        {
            InitializeComponent();

            m_dxfControl = dxfControl;
            SetText(text);
        }

        public void SetText(DXFViewer.Text text)
        {
            m_text = text;

            if (m_text == null)
                textBox1.Text = "";
            else
                textBox1.Text = m_text.Title;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (m_text != null)
            {
                EquipmentZone equipZone = (EquipmentZone)m_text.Tag;

                if (equipZone != null)
                {
                    equipZone.ZoneName = textBox1.Text;
                }

                m_text.Title = textBox1.Text;
                m_dxfControl.Refresh();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnApply_Click(null, null);
                this.Hide();
            }
        }
    }
}
