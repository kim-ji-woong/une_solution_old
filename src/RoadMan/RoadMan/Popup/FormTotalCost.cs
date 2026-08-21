using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadMan
{
    public partial class FormTotalCost : Form
    {
        private string m_strAddress = "";
        private VariousData<long> m_nLandCost = null;
        private VariousData<long> m_nObjectCost = null;
        private VariousData<long> m_nAroundCost = null;

        public VariousData<long> LandCost
        {
            get { return m_nLandCost; }
        }

        public VariousData<long> ObjectCost
        {
            get { return m_nObjectCost; }
        }

        public VariousData<long> AroundCost
        {
            get { return m_nAroundCost; }
        }

        public FormTotalCost(string strAddress, VariousData<long> nLandCost, VariousData<long> nObjectCost, VariousData<long> nAroundCost)
        {
            InitializeComponent();

            m_strAddress = strAddress;

            if (nLandCost == null)
                m_nLandCost = null;
            else
                m_nLandCost = new VariousData<long>(nLandCost.Data);

            if (nObjectCost == null)
                m_nObjectCost = null;
            else
                m_nObjectCost = new VariousData<long>(nObjectCost.Data);

            if (nAroundCost == null)
                m_nAroundCost = null;
            else
                m_nAroundCost = new VariousData<long>(nAroundCost.Data);
        }

        private void FormTotalCost_Load(object sender, EventArgs e)
        {
            labelAddrName.Text = m_strAddress;

            long nTotalCost = 0;

            if (m_nLandCost == null || m_nLandCost.Data == 0)
                textBoxLandCost.Text = "";
            else
            {
                textBoxLandCost.Text = m_nLandCost.Data.ToString();
                nTotalCost += m_nLandCost.Data;
            }

            if (m_nObjectCost == null || m_nObjectCost.Data == 0)
                textBoxObjectCost.Text = "";
            else
            {
                textBoxObjectCost.Text = m_nObjectCost.Data.ToString();
                nTotalCost += m_nObjectCost.Data;
            }

            if (m_nAroundCost == null || m_nAroundCost.Data == 0)
                textBoxAroundCost.Text = "";
            else
            {
                textBoxAroundCost.Text = m_nAroundCost.Data.ToString();
                nTotalCost += m_nAroundCost.Data;
            }

            if (nTotalCost == 0)
                labelTotalCost.Text = "";
            else
                labelTotalCost.Text = string.Format("{0:###,###,###,###,###,###}원", nTotalCost);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            long nLandCost, nObjectCost, nAroundCost;

            if (!CheckLongTextBox(textBoxLandCost, "토지보상비는", out nLandCost))
                return;

            if (!CheckLongTextBox(textBoxObjectCost, "지장물 보상비는", out nObjectCost))
                return;

            if (!CheckLongTextBox(textBoxAroundCost, "개략 공사비는", out nAroundCost))
                return;

            if (nLandCost == 0)
                m_nLandCost = null;
            else
            {
                if (m_nLandCost == null)
                    m_nLandCost = new VariousData<long>(nLandCost);
                else
                    m_nLandCost.Data = nLandCost;
            }

            if (nObjectCost == 0)
                m_nObjectCost = null;
            else
            {
                if (m_nObjectCost == null)
                    m_nObjectCost = new VariousData<long>(nObjectCost);
                else
                    m_nObjectCost.Data = nObjectCost;
            }

            if (nAroundCost == 0)
                m_nAroundCost = null;
            else
            {
                if (m_nAroundCost == null)
                    m_nAroundCost = new VariousData<long>(nAroundCost);
                else
                    m_nAroundCost.Data = nAroundCost;
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private bool CheckLongTextBox(TextBox textBox, string strName, out long nData)
        {
            if (textBox.Text.Length == 0)
                nData = 0;
            else
            {
                if (!long.TryParse(textBox.Text, out nData))
                {
					string szMsg = strName + " 숫자가 아닙니다.";
                    UnE.Utility.UMessageBox.Show(this, szMsg, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //MessageBox.Show(strName + " 숫자가 아닙니다.");
                    return false;
                }

                if (nData < 0)
                {
					string szMsg = strName + " 0보다 작은 숫자를 입력할 수 없습니다.";
                    UnE.Utility.UMessageBox.Show(this, szMsg, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //MessageBox.Show(strName + " 0보다 작은 숫자를 입력할 수 없습니다.");
                    return false;
                }
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            long nLandCost = 0, nObjectCost = 0, nAroundCost = 0;

            if (textBoxLandCost.Text.Length > 0)
            {
                if (!long.TryParse(textBoxLandCost.Text, out nLandCost))
                {
                    labelTotalCost.Text = "-";
                    return;
                }
            }

            if (textBoxObjectCost.Text.Length > 0)
            {
                if (!long.TryParse(textBoxObjectCost.Text, out nObjectCost))
                {
                    labelTotalCost.Text = "-";
                    return;
                }
            }

            if (textBoxAroundCost.Text.Length > 0)
            {
                if (!long.TryParse(textBoxAroundCost.Text, out nAroundCost))
                {
                    labelTotalCost.Text = "-";
                    return;
                }
            }

            long nTotalCost = nLandCost + nObjectCost + nAroundCost;

            if (nTotalCost == 0)
                labelTotalCost.Text = "0원";
            else
                labelTotalCost.Text = string.Format("{0:###,###,###,###,###,###}원", nLandCost + nObjectCost + nAroundCost);
        }

        private void btnMillion_Click(object sender, EventArgs e)
        {
            if (sender == btnMillionLandCost)
            {
                textBoxLandCost.Text += "000000";
            }
            else if (sender == btnMillionObjectCost)
            {
                textBoxObjectCost.Text += "000000";
            }
            else if (sender == btnMillionAroundCost)
            {
                textBoxAroundCost.Text += "000000";
            }
        }
    }
}
