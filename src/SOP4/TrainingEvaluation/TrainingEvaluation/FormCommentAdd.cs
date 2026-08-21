using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrainingEvaluation
{
    // 의견 내용 추가
    public partial class FormCommentAdd : FormBase
    {
        string m_strComment = "";

        public string comment { get { return m_strComment; } }

        public FormCommentAdd()
        {
            InitializeComponent();
             
            pictureBoxIcon.Visible = false;
            btnMax.Visible = false;
            btnMin.Visible = false;
            btnClose.Visible = false;
            txtComment.Select();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            m_strComment = txtComment.Text;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_strComment = "";
            this.Close();
        }
    }
}
