using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPMonitoringSystem
{
    public partial class PopupCheckItem : Form
    {
        private Sections.SectionProcess m_section;

        public PopupCheckItem()
        {
            InitializeComponent();
            //dataGridViewMission.Rows.Add();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void GetCheckItem(Sections.SectionProcess section)
        {
           // PropertiesProcess propertiesProcess = FormSOP.Instance.GetPageHome().GetDockProperties().GetPropertiesProcess();
            m_section = section;
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;
            string strID = data.ComponentID;

            if (data.CheckedItems == null || data.CheckedItems.Count == 0) return;

            int nIndex = 0;
            dataGridViewMission.Rows.Clear();
            foreach (Sections.CheckedItem check in data.CheckedItems)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = check.Category;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = check.SubCategory;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = check.Item;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = check.ItemCount;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = check.Location;
                gridRow.Cells.Add(cell);

                dataGridViewMission.Rows.Add(gridRow);
                nIndex++;
            }
        }

    }
}
