using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TeamManagementSystem
{
    //public partial class SectionGridView : Component
    //{
    //    public SectionGridView()
    //    {
    //        InitializeComponent();
    //    }

    //    public SectionGridView(IContainer container)
    //    {
    //        container.Add(this);

    //        InitializeComponent();
    //    }
    //}

    public partial class SectionGridView : DataGridView
    {
        private SectionGrid m_section = null;

        public SectionGridView(SectionGrid section)
        {
            m_section = section;
        }
    }

}
