using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DidUIEditor.Popups
{
    /// <summary>
    /// 페이지 삭제, 순서 지정 
    /// </summary>
    public partial class FormPageSetting : Form
    {
        private List<Page> m_pages = null;
        public FormPageSetting()
        {
            InitializeComponent();

            DisplayPage();
        }

        private void DisplayPage()
        {

            if (FormMain.Instance.Mode == Mode.Normal)
                m_pages = FormMain.Instance.HaveNormalPages;
            else if (FormMain.Instance.Mode == Mode.Emergency)
                m_pages = FormMain.Instance.HaveEmergencyPages;

            if (m_pages == null || m_pages.Count == 0)
                return;

            foreach (Page page in m_pages)
            {
                int index = dataGridView1.Rows.Add(page.Name);
                dataGridView1.Rows[index].Tag = page;
                dataGridView1.ReadOnly = true;
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows == null || dataGridView1.SelectedRows.Count == 0)
                return;

            DataGridViewRow row = dataGridView1.SelectedRows[0];
            if (row == null)
                return;
            
            if (row.Index == 0)
                return;

            Page page = row.Tag as Page;

            int chgIndex = row.Index - 1;
            dataGridView1.Rows.InsertCopy(row.Index, chgIndex);
            dataGridView1.Rows[chgIndex].Cells[0].Value = page.Name;
            dataGridView1.Rows[chgIndex].Selected = true;
            dataGridView1.Rows.Remove(row);            
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows == null || dataGridView1.SelectedRows.Count == 0)
                return;

            DataGridViewRow row = dataGridView1.SelectedRows[0];
            if (row == null)
                return;

            if (dataGridView1.Rows.Count - 1 == row.Index)
                return;

            int rowIndex = row.Index;
            Page page = row.Tag as Page;

            int chgIndex = row.Index + 1;
            dataGridView1.Rows.Remove(row);

            DataGridViewRow newRow = new DataGridViewRow();
            newRow.Tag = page;
            
            dataGridView1.Rows.Insert(chgIndex, newRow);
            dataGridView1.Rows[chgIndex].Cells[0].Value = page.Name;
            dataGridView1.Rows[chgIndex].Selected = true;
            dataGridView1.Rows[rowIndex].Selected = false;            
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows == null || dataGridView1.SelectedRows.Count == 0)
                return;

            DataGridViewRow row = dataGridView1.SelectedRows[0];
            if (row == null)
                return;

            dataGridView1.Rows.Remove(row);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //List<Page> pages = FormMain.Instance.HaveNormalPages;

            if (FormMain.Instance.Mode == Mode.Normal)
            {
                FormMain.Instance.HaveNormalPages.Clear();

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewRow row = dataGridView1.Rows[i];
                    FormMain.Instance.HaveNormalPages.Add((Page)row.Tag);
                    FormMain.Instance.HaveNormalPages[i].Name = (i + 1).ToString();
                }
            }
            else if (FormMain.Instance.Mode == Mode.Emergency)
            {
                FormMain.Instance.HaveEmergencyPages.Clear();

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewRow row = dataGridView1.Rows[i];
                    FormMain.Instance.HaveEmergencyPages.Add((Page)row.Tag);
                    FormMain.Instance.HaveEmergencyPages[i].Name = (i + 1).ToString();
                }
            }

            this.DialogResult = DialogResult.Yes;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }
    }
}
