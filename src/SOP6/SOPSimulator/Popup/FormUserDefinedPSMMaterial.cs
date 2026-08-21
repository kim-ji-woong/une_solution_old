using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPMonitoringSystem.Popup
{
    public partial class FormUserDefinedPSMMaterial : Form
    {
        private System.Collections.ICollection m_psmMaterials = null;
        private UnE.SOP.PSMMaterial m_material = null;

        public UnE.SOP.PSMMaterial Material
        {
            get { return m_material; }
            set { m_material = value; }
        }

        public FormUserDefinedPSMMaterial(System.Collections.ICollection psmMaterials)
        {
            InitializeComponent();

            m_psmMaterials = psmMaterials;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!CheckEmptyTextBox(textBoxMaterialName, "물질명을"))
                return;

            if (!CheckEmptyTextBox(textBoxInitDistance, "초기 이격거리를"))
                return;

            if (!CheckEmptyTextBox(textBoxDayDistance, "주간 방호대피거리를"))
                return;

            if (!CheckEmptyTextBox(textBoxNightDistance, "야간 방호대피거리를"))
                return;

            if (m_psmMaterials != null)
            {
                foreach (UnE.SOP.PSMMaterial material in m_psmMaterials)
                {
                    if (material.MaterialName == textBoxMaterialName.Text)
                    {
                        textBoxMaterialName.Focus();
                        MessageBox.Show(textBoxMaterialName.Text + "는 이미 존재하는 물질입니다.");
                        return;
                    }
                }
            }

            if (!CheckIntegerText(textBoxInitDistance, "초기 이격거리는"))
                return;

            if (!CheckIntegerText(textBoxDayDistance, "주간 방호대피거리는"))
                return;

            if (!CheckIntegerText(textBoxNightDistance, "야간 방호대피거리는"))
                return;

            m_material = new UnE.SOP.PSMMaterial();

            m_material.MaterialName = textBoxMaterialName.Text;
            m_material.InitDistance = int.Parse(textBoxInitDistance.Text);
            m_material.DayDistance = int.Parse(textBoxDayDistance.Text);
            m_material.NightDistance = int.Parse(textBoxNightDistance.Text);

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private bool CheckEmptyTextBox(TextBox textBox, string strTag)
        {
            if (textBox.Text.Length == 0)
            {
                textBox.Focus();
                MessageBox.Show(strTag + " 입력하세요");
                return false;
            }

            return true;
        }

        private bool CheckIntegerText(TextBox textBox, string strTag)
        {
            int data;

            if (!int.TryParse(textBox.Text, out data) || data < 0)
            {
                textBox.Focus();
                MessageBox.Show(strTag + " 0보다 같거나 작은 정수 형태의 값만 입력 가능합니다.");
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
