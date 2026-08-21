using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SoilMan.TabPages
{
    public class TabPageManager
    {
        private class PageData
        {
            private IGridPage m_page = null;
            private CheckBox m_checkBox = null;
            private Label m_label = null;
            private UnE.Controls.MergedDataGridView m_grid = null;
            private Button m_btnSave = null;

            public IGridPage Page
            {
                get { return m_page; }
                set { m_page = value; }
            }

            public CheckBox CheckBox
            {
                get { return m_checkBox; }
                set { m_checkBox = value; }
            }

            public Label Label
            {
                get { return m_label; }
                set { m_label = value; }
            }

            public Button Button
            {
                get { return m_btnSave; }
                set { m_btnSave = value; }
            }

            public UnE.Controls.MergedDataGridView DataGrid
            {
                get { return m_grid; }
                set { m_grid = value; }
            }

            public PageData()
            {
            }

            public PageData(IGridPage page, CheckBox checkBox, Button btn, Label label, UnE.Controls.MergedDataGridView grid)
            {
                m_page = page;
                m_checkBox = checkBox;
                m_label = label;
                m_grid = grid;
                m_btnSave = btn;
            }
        }

        private bool m_enableSave = false;
        private bool m_editMode = false;
        private static TabPageManager m_instance = null;

        private int m_nLabelRightPadding = 10;

        private int m_nCheckBoxPosX = -1, m_nCheckBoxPosY = -1;
        private int m_nLabelUnitPosX = -1, m_nLabelUnitPosY = -1;
        private int m_nButtonPosX = -1, m_nButtonPosY = -1;

        private Dictionary<Form, PageData> m_dicPageDatas = new Dictionary<Form, PageData>();

        private string m_strConfigFilePath = "";

        public static TabPageManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new TabPageManager();

                return m_instance;
            }
        }

        public string ConfigFilePath
        {
            get { return m_strConfigFilePath; }
        }

        protected TabPageManager()
        {
            m_strConfigFilePath = Application.StartupPath + "\\Config.ini";
        }

        public void InitStyle(Form frm, UnE.Controls.MergedDataGridView grid, CheckBox checkBoxEditMode, Button btnSave, Label labelUnit, string strUnit)
        {
            checkBoxEditMode.Checked = m_editMode;
            btnSave.Enabled = m_enableSave;

            labelUnit.Visible = strUnit.Length != 0;
            labelUnit.Text = strUnit;

            if (m_nCheckBoxPosX < 0)
            {
                m_nCheckBoxPosX = checkBoxEditMode.Location.X;
                m_nCheckBoxPosY = checkBoxEditMode.Location.Y;

                m_nLabelUnitPosX = labelUnit.Location.X;
                m_nLabelUnitPosY = labelUnit.Location.Y;

                m_nButtonPosX = btnSave.Location.X;
                m_nButtonPosY = btnSave.Location.Y;
            }
            else
            {
                checkBoxEditMode.Location = new System.Drawing.Point(m_nCheckBoxPosX, m_nCheckBoxPosY);
                labelUnit.Location = new System.Drawing.Point(m_nLabelUnitPosX, m_nLabelUnitPosY);
                btnSave.Location = new System.Drawing.Point(m_nButtonPosX, m_nButtonPosY);
            }

            InitGridStyle(grid);

            frm.ShowInTaskbar = false;
            frm.TopLevel = false;

            PageData data = new PageData((IGridPage)frm, checkBoxEditMode, btnSave, labelUnit, grid);
            m_dicPageDatas[frm] = data;
            
            frm.Resize += new EventHandler(this.OnSize);
            checkBoxEditMode.CheckedChanged += new EventHandler(this.OnCheckedChanged);
            btnSave.Click += new EventHandler(this.btnSave_Click);
            grid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnGridKeyDown);

            OnSize(frm, null);
            OnCheckedChanged(checkBoxEditMode, null);
        }

        private void OnGridKeyDown(object sender, KeyEventArgs e)
        {
            UnE.Controls.MergedDataGridView grid = (UnE.Controls.MergedDataGridView)sender;

            if (grid.ReadOnly)
                return;

            Form frmGridOwner = null;
            
            foreach (DataGridViewCell cell in grid.SelectedCells)
            {
                if (cell.ReadOnly)
                    continue;

                if (cell.Value != null)
                {
                    cell.Value = null;

                    if (frmGridOwner == null)
                        frmGridOwner = FindGridOwner(grid);

                    if (frmGridOwner != null)
                        OnDataChanged(frmGridOwner);
                }
            }
        }

        private Form FindGridOwner(UnE.Controls.MergedDataGridView grid)
        {
            foreach (KeyValuePair<Form, PageData> pair in m_dicPageDatas)
            {
                if (pair.Value.DataGrid == grid)
                    return pair.Key;
            }

            return null;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // EUC-KR : 51949
            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            StreamWriter writer = new StreamWriter(m_strConfigFilePath, false, encEUC_KR);

            foreach (KeyValuePair<Form, PageData> pair in m_dicPageDatas)
            {
                writer.WriteLine(pair.Value.Page.ConfigSectionName);

                int nColumnCount = pair.Value.DataGrid.Columns.Count;

                foreach (DataGridViewRow row in pair.Value.DataGrid.Rows)
                {
                    // 0번 열은 Index 번호이므로 추가하지 않는다.
                    for (int i=1;i<nColumnCount;i++)
                    {
                        DataGridViewCell cell = row.Cells[i];
                        string strValue = cell.Value == null ? "" : cell.Value.ToString().Trim();

                        if (i == 1)
                            writer.Write(strValue);
                        else
                            writer.Write("\t" + strValue);
                    }

                    writer.WriteLine();
                }
            }

            writer.Close();

            Button btn = (Button)sender;
            btn.Enabled = m_enableSave = false;
        }

        private void InitGridStyle(UnE.Controls.MergedDataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 25;

            grid.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(220, 230, 242);
            grid.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.HeaderCell.Style.BackColor = System.Drawing.Color.FromArgb(147, 205, 221);
            }
        }

        public void OnTabChanged(TabPage page)
        {
            foreach (object obj in page.Controls)
            {
                if (obj is IGridPage)
                {
                    Form frm = (Form)obj;
                    PageData data;

                    if (m_dicPageDatas.TryGetValue(frm, out data))
                    {
                        data.CheckBox.Checked = m_editMode;
                        data.Button.Enabled = m_enableSave;

                        OnSize(obj, null);
                    }

                    break;
                }
            }
        }

        public void OnDataChanged(Form frm)
        {
            PageData data;

            if (m_dicPageDatas.TryGetValue(frm, out data))
            {
                data.Button.Enabled = true;
            }

            m_enableSave = true;
        }

        protected void OnSize(object sender, EventArgs e)
        {
            Form frm = (Form)sender;

            if (frm == null)
                return;

            PageData data;

            if (!m_dicPageDatas.TryGetValue(frm, out data))
                return;

            if (data.Label.Visible)
            {
                int x = frm.Size.Width - (data.Label.Size.Width + m_nLabelRightPadding);
                data.Label.Location = new System.Drawing.Point(x, data.Label.Location.Y);
            }
        }

        private void OnCheckedChanged(object sender, EventArgs e)
        {
            if (sender == null)
                return;

            PageData data = null;

            foreach (KeyValuePair<Form, PageData> pair in m_dicPageDatas)
            {
                if (pair.Value.CheckBox == sender)
                {
                    data = pair.Value;
                    break;
                }
            }

            if (data == null)
                return;

            data.Page.EditMode = data.CheckBox.Checked;
            m_editMode = data.CheckBox.Checked;
        }
    }

    public interface IGridPage
    {
        bool EditMode
        {
            get;
            set;
        }

        string ConfigSectionName
        {
            get;
        }
    }
}
