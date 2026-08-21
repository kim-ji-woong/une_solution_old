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
    public partial class FormMessage : Form
    {
        public FormMessage()
        {
            InitializeComponent();
            
        }

        private void FormMessage_Load(object sender, EventArgs e)
        {
            SetCheckBox();
        }

        private void FormMessage_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void SetCheckBox()
        {
            bool bChecked = FormMain.Instance.DataMgr.MessageChecked;
            if (bChecked == true)
                checkBox1.Checked = true;
            else
                checkBox1.Checked = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //DBConn conn = new DBConn("HSMS");

            if (checkBox1.Checked == true)
            {
                EditMessage editMessage = new EditMessage();
                editMessage.Checked = true;
                editMessage.Update(null);
            }
            else
            {
                EditMessage editMessage = new EditMessage();
                editMessage.Checked = false;
                editMessage.Update(null);
            }

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
