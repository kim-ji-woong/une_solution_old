using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BIMViewer
{
    using BIM;

    public partial class ProjectView : UserControl
    {
        private IProjectOwner m_owner = null;
        private Project m_selectedProject = null;

        public ProjectView(IProjectOwner owner)
        {
            InitializeComponent();
            m_owner = owner;
        }

        public void SetProject(Project project)
        {
            //gridProject.Rows.Clear();
            m_selectedProject = null;

            if (project != null)
            {        
                string localFilePath = project.LocalFilePath;
                int index = localFilePath.LastIndexOf(@"\");
                string projectName = localFilePath.Substring(index + 1);
                projectName = projectName.Replace(".xml", "");

                int nRowIndex = gridProject.Rows.Add();
                gridProject.Rows[nRowIndex].Cells[0].Value = projectName; //project.Name;
                gridProject.Rows[nRowIndex].Cells[1].Value = "-";
                gridProject.Rows[nRowIndex].Tag = project;
            }

            if (project == null || gridProject.Rows.Count == 0)
                m_owner.OnSelectProject(null);
            else
            {
                gridProject.Rows[0].Cells[0].Selected = true;
                m_owner.OnSelectProject((Project)gridProject.Rows[0].Tag);
                m_selectedProject = (Project)gridProject.Rows[0].Tag;
            }
        }

        public void SetProjects(List<Project> projects)
        {
            gridProject.Rows.Clear();
            m_selectedProject = null;

            if (projects != null)
            {
                foreach (Project project in projects)
                {
                    int nRowIndex = gridProject.Rows.Add();
                    gridProject.Rows[nRowIndex].Cells[0].Value = project.Name;
                    gridProject.Rows[nRowIndex].Tag = project;
                }
            }

            if (projects == null || gridProject.Rows.Count == 0)
                m_owner.OnSelectProject(null);
            else
            {
                gridProject.Rows[0].Cells[0].Selected = true;
                m_owner.OnSelectProject((Project)gridProject.Rows[0].Tag);
                m_selectedProject = (Project)gridProject.Rows[0].Tag;
            }
        }

        private void gridProject_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.RowIndex >= 0)
                {
                    Project project = (Project)gridProject.Rows[e.RowIndex].Tag;

                    //ym.0828.그리드뷰에서 -누르면, 해당프로젝트 삭제하기
                    if (e.ColumnIndex == 1)
                    {
                        if (gridProject.Rows.Count > 0)
                        {
                            m_owner.OnDeleteProject(project);
                            gridProject.Rows.Remove(gridProject.Rows[e.RowIndex]);
                            if (gridProject.Rows.Count == 0)
                            {
                                m_selectedProject = null;
                                m_owner.OnSelectProject(null);
                            }
                            else
                            {
                                gridProject.Rows[0].Selected = true;//맨처음게 선택되게함.
                                m_owner.OnSelectProject((Project)gridProject.Rows[0].Tag);
                                m_selectedProject = (Project)gridProject.Rows[0].Tag;
                            }
                        }
                    }
                    else if (project != null && m_selectedProject != project)
                    {
                        m_owner.OnSelectProject(project);
                        m_selectedProject = project;
                    }
                }
            }
        }
        public void SetSelectProject(Project project)
        {
            foreach (DataGridViewRow row in gridProject.Rows)
            {
                Project rowProject = row.Tag as Project;
                if (rowProject == null)
                    continue;

                if (project == rowProject)
                {
                    row.Selected = true;
                    break;
                }
            }
        }

        public Project GetProject(int projectID)
        {
            foreach (DataGridViewRow row in gridProject.Rows)
            {
                Project project = row.Tag as Project;
                if (project == null)
                    continue;

                if (project.ID == projectID)
                    return project;
            }

            return null;
        }

        public void ReloadProject(Project project)
        {   // XML 노아 서버에 업로드 후 XML 다운받아 다시 불러오기 작업

            string localFilePath = project.LocalFilePath;
            int index = localFilePath.LastIndexOf(@"\");
            string projectName = localFilePath.Substring(index + 1);
            projectName = projectName.Replace(".xml", "");

            foreach (DataGridViewRow row in gridProject.Rows)
            {
                string strName = row.Cells[0].Value.ToString();

                if (strName == projectName)
                {
                    row.Tag = project;
                    break;
                }
            }
        }
    }

    public interface IProjectOwner
    {
        void OnSelectProject(Project project);
        void OnDeleteProject(Project project);
    }
}
