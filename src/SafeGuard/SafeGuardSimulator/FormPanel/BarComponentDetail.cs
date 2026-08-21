using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sections;

namespace SOPManager
{
    public partial class BarComponentDetail : Form
    {
        private Section m_section = null;

        public Section Section
        {
            get { return m_section; }
            set
            {
                if (m_section != value)
                {
                    m_section = value;

                    if (m_section == null)
                        Clear();
                    else
                        SetData();
                }
            }
        }

        public BarComponentDetail()
        {
            InitializeComponent();
        }

        private void Clear()
        {
            textBoxTitle.Text = "";
            gridBody.Rows.Clear();
        }

        private void SetData()
        {
            textBoxTitle.Text = m_section.Title;
            gridBody.Rows.Clear();

            if (m_section is SectionProcess)
            {
                SectionDataProcess data = (SectionDataProcess)m_section.Data;

                foreach (MissionItem item in data.MissionItems)
                {
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    cell.Value = item.Mission;

                    DataGridViewRow row = new DataGridViewRow();
                    row.Cells.Add(cell);
                    gridBody.Rows.Add(row);
                }
            }
        }
    }
}
