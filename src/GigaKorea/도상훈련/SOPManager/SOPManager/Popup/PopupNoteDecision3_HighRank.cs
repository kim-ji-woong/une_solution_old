using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPManager.Popup
{
    public partial class PopupNoteDecision3_HighRank : Form
    {
        private string m_strValue = "";
        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }

        private string m_strCategoryName = "";
        private string m_strSubCategoryName = "";

        private double WindowRateWidth;
        private double WindowRateHeight;

        public PopupNoteDecision3_HighRank(string strValue, string strCategoryName, string strSubCategoryName)
        {
            InitializeComponent();

            m_strValue = strValue;
            m_strCategoryName = strCategoryName;
            m_strSubCategoryName = strSubCategoryName;

            this.textBoxExpression.Text = m_strValue;

            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            WindowRateWidth = dWindowRate[0];
            WindowRateHeight = dWindowRate[1];

            UpdateControlSize();
        }

        private void PopupNoteDecision3_HighRank_Load(object sender, EventArgs e)
        {
            Init();
        }

        #region 4K, FullHD Resize
        public void UpdateControlSize()
        {
            HaveControl(this, WindowRateWidth, WindowRateHeight);
        }

        private void HaveControl(Control pctl, double WindowRateWidth, double WindowRateHeight)
        {
            foreach (Control ctl in pctl.Controls)
            {
                if (ctl.Controls.Count > 0)
                    HaveControl(ctl, WindowRateWidth, WindowRateHeight);

                FormMain.Instance.UpdateWindowRate(ctl, WindowRateWidth, WindowRateHeight, "나눔스퀘어");
            }
        }
        #endregion


        private void Init()
        {
            gridSystemType.Rows.Clear();

            List<SOPParameter> systemParameters = PopupSpecialMessage.GetSystemParameters(m_strCategoryName, m_strSubCategoryName);

            foreach (SOPParameter param in systemParameters)
            {
                AddVariable(gridSystemType, param);
            }

            gridUserType.Rows.Clear();

            ConfigData config = null;
            List<SOPParameter> userParameters = FormMain.Instance.GetPageLevel().GetBarConfig().GetCurrentVariables(out config);
            //List<SOPParameter> userParameters = FormMain.Instance.GetPageLevel().GetBarConfig().GetCurrentVariables(out m_strUserDefinedConfigName);

            if (userParameters != null)
            {
                foreach (SOPParameter param in userParameters)
                {
                    AddVariable(gridUserType, param);
                }
            }
        }

        private void AddVariable(DataGridView grid, SOPParameter param)
        {
            int nRowIndex = grid.Rows.Add();
            DataGridViewRow row = grid.Rows[nRowIndex];

            row.Cells[0].Value = "{" + param.VariableName + "}";
            row.Cells[1].Value = Sections.SectionDataDecision.GetVariableTypeName(param.Type);
            row.Cells[2].Value = param.Description;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_strValue = textBoxExpression.Text;
            if (PopupNoteDecision3.Instance.CheckExpression(m_strValue))
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void gridType_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 0)
                return;

            DataGridView grid = (DataGridView)sender;
            DataGridViewRow row = grid.Rows[e.RowIndex];

            if (row.IsNewRow)
                return;

            textBoxExpression.Paste(row.Cells[e.ColumnIndex].Value.ToString());
            textBoxExpression.Focus();
        }
    }
}
