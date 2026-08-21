using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPMonitoringSystem
{
    //public class SectionTextBox : Label
    //{
    //    private Section m_section = null;

    //    public SectionTextBox(Section section)
    //    {
    //        m_section = section;
    //    }
    //}

    public class SectionTextBox : TextBox
    {
        private Section m_section = null;

        public SectionTextBox(Section section)
        {
            m_section = section;
        }
    }
}
