using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HSMS
{
    public partial class FormEditMember : Form, UnE.GUI.IRibbonButtonOwner
    {
        public ToolStripStatusLabel GetStatusLabel()
        {
            return null;
        }

        private FormLoginMain m_formParent = null;
        public FormEditMember(FormLoginMain form)
        {
            InitializeComponent();

            this.TopLevel = false;

            m_formParent = form;
            MouseDown += new MouseEventHandler(m_formParent.FormLoginMain_MouseDown);
            MouseMove += new MouseEventHandler(m_formParent.FormLoginMain_MouseMove);
            MouseUp += new MouseEventHandler(m_formParent.FormLoginMain_MouseUp);     

            initButton();
        }

        private void initButton()
        {
            this.btnMemberDelete.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnEditMember.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        }

                //인터페이스 메서드 구현
        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {
        }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
        }

        private void FormEditMember_Load(object sender, EventArgs e)
        {
            btnMemberDelete.Owner = this;
            btnEditMember.Owner = this;
            btnCancel.Owner = this;
        }

        private void btnMemberDelete_Click(object sender, EventArgs e)
        {
            m_formParent.ShowDeleteForm();
        }

        private void btnEditMember_Click(object sender, EventArgs e)
        {
            m_formParent.ShowChangePassForm();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_formParent.ShowLoginForm();
        }
    }
}
