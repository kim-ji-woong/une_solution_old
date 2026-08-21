using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPGen
{
    class SectionTextBox : TextBox
    {
        private Section m_section = null;

        public SectionTextBox(Section section)
        {
            InitializeComponent();
            //SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            m_section = section;
        }

        private void InitializeComponent()
        {
            this.KeyDown += new KeyEventHandler(this.OnKeyDown); ;
        }

        protected void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                m_section.Select(false, null);
                m_section.GetParent().Invalidate(m_section.InvalidateRectArea);
            }
        }
    }
}
