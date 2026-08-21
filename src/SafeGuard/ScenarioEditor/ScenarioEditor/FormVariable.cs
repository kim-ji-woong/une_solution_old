using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScenarioEditor
{
    public partial class FormVariable : Form, IVariable
    {
        private CheckBox m_checkBox = null;
        private List<string> m_actions = new List<string>();
        private List<string> m_patientItems = new List<string>();

        public string MaterialName
        {
            get { return cboMaterialName.Text; }
        }

        public string Place
        {
            get { return textBoxPlace.Text; }
        }

        public string Reason
        {
            get { return cboReason.Text; }
        }

        public string Weather
        {
            get { return cboWeather.Text; }
        }

        public string Material
        {
            get { return textBoxMaterial.Text; }
        }

        public string CountOfDeath
        {
            get { return textBoxCountOfDeath.Text; }
        }

        public string CountOfBuilding
        {
            get { return textBoxCountOfBuilding.Text; }
        }

        public string InitialDistance
        {
            get { return textBoxInitialDistance.Text; }
        }

        public string Control
        {
            get { return FormMain.Instance.IsDayLight ? "팀장" : "선임계장"; }
        }

        public string Distance
        {
            get { return textBoxDistance.Text; }
        }

        public string MixedFactor
        {
            get { return cboMixedFactor.Text; }
        }

        public CheckBox CheckBox
        {
            get { return m_checkBox; }
            set
            {
                m_checkBox = value;
                m_checkBox.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            }
        }

        public List<string> Actions
        {
            get
            {
                m_actions.Clear();

                foreach (DataGridViewRow row in dataGridViewActionList.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    string strValue = row.Cells[0].Value.ToString();

                    if (strValue.Length > 0)
                        m_actions.Add(strValue);
                }

                return m_actions;
            }
        }

        public List<string> PatientItems
        {
            get
            {
                m_patientItems.Clear();

                foreach (DataGridViewRow row in dataGridViewPatient.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    string strValue = row.Cells[0].Value.ToString();

                    if (strValue.Length > 0)
                        m_patientItems.Add(strValue);
                }

                return m_patientItems;
            }
        }

        public FormVariable()
        {
            InitializeComponent();
        }

        private void FormVariable_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!FormMain.Instance.CloseApplication)
            {
                e.Cancel = true;
                m_checkBox.Checked = false;
                this.Hide();
            }
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (m_checkBox.Checked)
                this.Show();
            else
                this.Hide();
        }

        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete)
                return;

            DataGridView grid = (DataGridView)sender;

            if (grid.SelectedCells == null || grid.SelectedCells.Count == 0)
                return;

            DataGridViewRow row = grid.Rows[grid.SelectedCells[0].RowIndex];

            if (row.IsNewRow)
                return;

            grid.Rows.Remove(row);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            FormMain.Instance.Reset(this);
        }

        private void cboMaterialName_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*if (cboMaterialName.Text == "황산" || cboMaterialName.Text == "질산")
            {
                if (cboMixedFactor.Items.Count == 3)
                    cboMixedFactor.Items.Add("가연성물질");
            }
            else
            {
                if (cboMixedFactor.Items.Count == 4)
                    cboMixedFactor.Items.RemoveAt(3);
            }*/

            string strSelectedItem = cboMaterialName.SelectedItem.ToString();

            if (strSelectedItem == "벤젠")
                SetMixedFactor("열", "산화성물질", "없음");
            else if (strSelectedItem == "산화질소")
                SetMixedFactor("물", "가연성물질", "없음");
            else if (strSelectedItem == "암모니아")
                SetMixedFactor("물", "산", "열", "없음");
            else if (strSelectedItem == "염소")
                SetMixedFactor("물", "열", "없음");
            else if (strSelectedItem == "황산" || strSelectedItem == "질산")
                SetMixedFactor("물", "열", "가연성물질", "없음");
            else
                SetMixedFactor("물", "열", "없음");
        }

        private void SetMixedFactor(string strFactor1, string strFactor2, string strFactor3, string strFactor4 = null)
        {
            cboMixedFactor.Items.Clear();

            cboMixedFactor.Items.Add(strFactor1);
            cboMixedFactor.Items.Add(strFactor2);
            cboMixedFactor.Items.Add(strFactor3);

            if (strFactor4 != null)
                cboMixedFactor.Items.Add(strFactor4);
        }
    }

    public interface IVariable
    {
        string MaterialName
        {
            get;
        }

        string Place
        {
            get;
        }

        string Reason
        {
            get;
        }

        string Weather
        {
            get;
        }

        string Material
        {
            get;
        }

        string CountOfDeath
        {
            get;
        }

        string CountOfBuilding
        {
            get;
        }

        string InitialDistance
        {
            get;
        }

        string Control
        {
            get;
        }

        string Distance
        {
            get;
        }

        string MixedFactor
        {
            get;
        }
        List<string> Actions
        {
            get;
        }

        List<string> PatientItems
        {
            get;
        }
    }
}
