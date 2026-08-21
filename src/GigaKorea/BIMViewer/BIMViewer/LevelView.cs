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

    public partial class LevelView : UserControl
    {
        private Level m_selectedLevel = null;
        private ILevelOwner m_owner = null;
        private Project m_project = null;

        public LevelView(ILevelOwner owner)
        {
            InitializeComponent();
            m_owner = owner;
        }

        public void SetLevels(List<Level> levels, Project project) 
        {
            gridLevel.Rows.Clear();
            m_selectedLevel = null;
            m_project = project;

            if (levels != null)
            {
                foreach (Level level in levels)
                {
                    int nRowIndex = gridLevel.Rows.Add();
                    gridLevel.Rows[nRowIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    gridLevel.Rows[nRowIndex].Cells[0].Value = level.Name;
                    gridLevel.Rows[nRowIndex].Tag = level;
                }
            }
        }

        private void gridLevel_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                Level level = (Level)gridLevel.Rows[e.RowIndex].Tag;

                if (level != null)
                    m_owner.OpenLevel(level, m_project);
            }
        }

        private void tsMenuOpenLevel_Click(object sender, EventArgs e)
        {
            if (m_selectedLevel != null)
                m_owner.OpenLevel(m_selectedLevel, m_project);
        }

        private void gridLevel_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (gridLevel.SelectedCells.Count > 0)
                {
                    m_selectedLevel = (Level)gridLevel.Rows[gridLevel.SelectedCells[0].RowIndex].Tag;

                    if (m_selectedLevel != null)
                    {
                        System.Drawing.Rectangle rect = gridLevel.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                        contextMenuLevel.Show(gridLevel, e.X + rect.Left, e.Y + rect.Top);
                    }
                }
            }
        }

        public void SetSelectLevel(Level level)
        {
            foreach (DataGridViewRow row in gridLevel.Rows)
            {
                Level rowLevel = row.Tag as Level;
                if (rowLevel == null)
                    continue;

                if (level == rowLevel)
                {
                    row.Selected = true;
                    break;
                }
                else
                    row.Selected = false;
            }
        }

        public Level GetLevel(int levelID)
        {
            foreach (DataGridViewRow row in gridLevel.Rows)
            {
                Level level = row.Tag as Level;
                if (level == null)
                    continue;

                if (level.ID == levelID)
                    return level;
            }

            return null;
        }

        public void DeleteGridLevels()
        {
            if (gridLevel.Rows.Count > 0)
                gridLevel.Rows.Clear();
            m_selectedLevel = null;
            m_project = null;
        }
    }

    public interface ILevelOwner
    {
        void OpenLevel(Level level, Project project);
    }
}
