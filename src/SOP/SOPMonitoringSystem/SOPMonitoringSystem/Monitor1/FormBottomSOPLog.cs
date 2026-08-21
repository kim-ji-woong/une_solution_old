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
    public partial class FormBottomSOPLog : Form
    {
        private Dictionary<SOPData, ArrayList> m_dicTasks = new Dictionary<SOPData, ArrayList>();
        private SOPData m_currentSOP = null;

        public FormBottomSOPLog()
        {
            InitializeComponent();
            InitGrid();
        }

        private void InitGrid()
        {
            foreach (DataGridViewColumn column in gridLog.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            gridLog.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        public ArrayList GetTaskArray(SOPData data)
        {
            if (m_dicTasks.ContainsKey(data))
                return m_dicTasks[data];

            ArrayList taskArray = new ArrayList();
            m_dicTasks[data] = taskArray;
            return taskArray;
        }

        public void SetCurrentSOP(SOPData data)
        {
            if (data != m_currentSOP)
            {
                gridLog.Rows.Clear();

                if (data != null)
                {
                    if (m_dicTasks.ContainsKey(data))
                        AddTask(m_dicTasks[data], 0, data);
                }
            }

            m_currentSOP = data;
        }

        public void AddTask(ArrayList arrTask, int nIndex, SOPData data = null)
        {
            if (data == null)
            {
                data = m_currentSOP;
                if (m_currentSOP == null) return;
            }

            int nTaskCount = arrTask.Count;

            for (int i = nIndex; i < nTaskCount; i++)
            {
                Task task = (Task)arrTask[i];

                DataGridViewRow row = new DataGridViewRow();
                
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = task.ProcessTime;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = data.Name;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = " " + task.ProcessName;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = task.MemberName;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = task.TaskName;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = task.Status;
                row.Cells.Add(cell);

                gridLog.Rows.Add(row);
            }
        }
    }

    public class Task
    {
        private SectionEx m_section = null;
        private string m_strProcessName = "";
        private string m_strMember = "";
        private string m_strTask = "";
        private string m_strStatus = "";
        private string m_strProcessTime = "";

        public SectionEx Section
        {
            get { return m_section; }
            set { m_section = value; }
        }

        public string ProcessName
        {
            get { return m_strProcessName; }
            set { m_strProcessName = value; }
        }

        public string MemberName
        {
            get { return m_strMember; }
            set { m_strMember = value; }
        }

        public string TaskName
        {
            get { return m_strTask; }
            set { m_strTask = value; }
        }

        public string Status
        {
            get { return m_strStatus; }
            set { m_strStatus = value; }
        }

        public string ProcessTime
        {
            get { return m_strProcessTime; }
            set { m_strProcessTime = value; }
        }
    }
}
