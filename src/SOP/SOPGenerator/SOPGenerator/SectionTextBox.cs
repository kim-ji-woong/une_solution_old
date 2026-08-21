using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPGen
{
    public class SectionTextBox : TextBox
    {
        private Section m_section = null;
        private bool m_isTextChanging = false;

        public SectionTextBox(Section section)
        {
            InitializeComponent();
            //SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            m_section = section;
        }

        private void InitializeComponent()
        {
            this.KeyDown += new KeyEventHandler(this.OnKeyDown);

            //Multiline = true;
        }

        protected void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnLostFocus(null);
                //m_section.GetParent().Invalidate(m_section.InvalidateRectArea, true);
                //m_section.GetParent().Update();
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (!m_isTextChanging)
            {
                m_isTextChanging = true;
                SectionTimeText section = (SectionTimeText)m_section;

                string strPrev = section.GetPrevText();
                if (Text == strPrev)
                {
                    section.Select(false, null);
                    section.GetParent().Refresh();
                    m_isTextChanging = false;
                    return;
                }

                section.OnTextChanged(Text);

                if (!section.CheckDuplicateTeamSchedule())
                {
                    Text = strPrev;
                    section.OnTextChanged(Text);
                }

                section.Select(false, null);
                section.GetParent().Refresh();
                m_isTextChanging = false;
            }
        }
    }
}
