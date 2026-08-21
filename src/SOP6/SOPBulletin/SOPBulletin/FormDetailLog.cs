using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPBulletin
{
    public partial class FormDetailLog : Form
    {
        private ComponentHistory m_history = null;

        public FormDetailLog(ComponentHistory componentHistory)
        {
            InitializeComponent();
            m_history = componentHistory;
        }

        private void FormDetailLog_Load(object sender, EventArgs e)
        {
            if (m_history == null || m_history.SectionState == null)
                return;

            labelSectionTitle.Text =  HistoryManager2.GetDetailTask(m_history.SectionState.Section.Data);

            foreach (ComponentHistory history in m_history.AllHistories)
            {
                DataGridViewRow row = DockingRealTime.MakeNewRow(dataGridView1);

                row.Cells[0].Value = row.Index + 1;
                row.Cells[1].Value = string.Format("{0:00}:{1:00}:{2:00}", history.Time.Hour, history.Time.Minute, history.Time.Second);
                row.Cells[2].Value = history.Task;
                row.Cells[3].Value = ComponentHistory.ToHistoryTypeString(history.Type);
            }
        }
    }
}
