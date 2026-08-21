using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BIMViewer
{
    public partial class FormSelectProject : Form
    {
        private List<BIM.Project> m_projects = null;
        private BIM.Project m_selectedProject = null;

        public BIM.Project SelectedProject
        {
            get { return m_selectedProject; }
        }

        public FormSelectProject(List<BIM.Project> projects)
        {
            InitializeComponent();
            m_projects = projects;
        }

        private void FormSelectProject_Load(object sender, EventArgs e)
        {
            if (m_projects != null)
            {
                foreach (BIM.Project project in m_projects)
                {
                    int nRowIndex = gridProjects.Rows.Add();
                    DataGridViewRow row = gridProjects.Rows[nRowIndex];

                    row.Cells[0].Value = nRowIndex + 1;
                    row.Cells[1].Value = project.Name;
                    row.Cells[2].Value = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", project.TimeStamp.Year, project.TimeStamp.Month, project.TimeStamp.Day, project.TimeStamp.Hour, project.TimeStamp.Minute, project.TimeStamp.Second);

                    row.Tag = project;
                }
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (gridProjects.SelectedCells.Count == 0)
            {
                MessageBox.Show("불러들일 Project를 선택해 주세요.");
                return;
            }

            int nRowIndex = gridProjects.SelectedCells[0].RowIndex;
            DataGridViewRow row = gridProjects.Rows[nRowIndex];

            m_selectedProject = (BIM.Project)row.Tag;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void gridProjects_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = gridProjects.Rows[e.RowIndex];

                m_selectedProject = (BIM.Project)row.Tag;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
