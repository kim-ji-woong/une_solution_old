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
    public partial class FormNewUserDefinedParameterConfig : Form
    {
        private string m_strNewConfigName = "";
        private string m_strCopyFrom = null;
        private IEditItemOwner m_owner = null;

        public string NewConfigName
        {
            get { return m_strNewConfigName; }
        }

        public string CopyFrom
        {
            get { return m_strCopyFrom; }
        }

        public FormNewUserDefinedParameterConfig(List<string> configNames, IEditItemOwner owner)
        {
            InitializeComponent();

            m_owner = owner;
            cboSources.Items.Add("<비어 있음>");

            foreach (string strConfigName in configNames)
            {
                cboSources.Items.Add(strConfigName);
            }

            cboSources.SelectedIndex = 0;
            UpdateControlSize();
        }

        public void UpdateControlSize()
        {
            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            double WindowRateWidth = dWindowRate[0];
            double WindowRateHeight = dWindowRate[1];

            this.Size = new System.Drawing.Size((int)(this.Size.Width * WindowRateWidth), (int)(this.Size.Height * WindowRateHeight));

            FormMain.Instance.UpdateWindowRate(label1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(textBoxConfigName, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(label2, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(cboSources, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(btnOK, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(btnCancel, WindowRateWidth, WindowRateHeight);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string strConfigName = textBoxConfigName.Text.Trim();

            if (strConfigName.Length == 0)
            {
                textBoxConfigName.Focus();
                MessageBox.Show("새로운 설정 이름을 입력하세요");
                return;
            }

            if (strConfigName.Contains(' ') || strConfigName.Contains('{') || strConfigName.Contains('}'))
            {
                textBoxConfigName.Focus();
                MessageBox.Show("새로운 설정 이름에 빈칸이나 '{', '}'와 같은 문자는 사용할 수 없습니다.");
                return;
            }

            if (m_owner.IsValidName(strConfigName) == false)
            {
                textBoxConfigName.Focus();
                MessageBox.Show("사용할 수 없는 이름입니다.");
                return;
            }

            int nItemCount = cboSources.Items.Count;

            for (int i = 1; i < nItemCount;i++)
            {
                string strItemValue = cboSources.Items[i].ToString();

                if (strConfigName.ToLower() == strItemValue.ToLower())
                {
                    textBoxConfigName.Focus();
                    MessageBox.Show("이미 같은 이름이 존재합니다.");
                    return;
                }
            }

            if (cboSources.SelectedIndex > 0)
                m_strCopyFrom = cboSources.Items[cboSources.SelectedIndex].ToString();

            m_strNewConfigName = strConfigName;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
