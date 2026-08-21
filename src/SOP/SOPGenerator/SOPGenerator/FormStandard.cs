using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPGen
{
    public partial class FormStandard : Form
    {
        private FormDocking m_Docking = null;

        string[] m_strValue = new string[4];

        public FormStandard(FormDocking dock)
        {
            InitializeComponent();

            m_Docking = dock;

            InitList();

            cboStandard.SelectedIndex = 0;
            textStandard.Text = m_strValue[cboStandard.SelectedIndex];
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            m_Docking.GetCircumstances().SetString(textStandard.Text);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cboStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            textStandard.Text = m_strValue[cboStandard.SelectedIndex];
        }

        private void InitList()
        {
            m_strValue[0] = "[정지사고] 4/10 10:23 ○○화력 ○호기 발전정지(용량 500㎿) \r\n예비율 10.5%";
            m_strValue[1] = "[감발사고] 4/10 10:23 ○○화력 ○호기 출력감발 200㎿ \r\n(용량 500㎿) 예비율 10.5%";
            m_strValue[2] = "[인명사고] 4/10 10:23 ○○화력 ○○장소에서 ○○사고로 \r\n○○소속 직원 ○명 사망";
            m_strValue[3] = "[재난사고] 4/10 10:23 ○○화력 ○호기 ○○장소에서 ○○화재 \r\n발생, 소방차 출동";
        }
    }
}
