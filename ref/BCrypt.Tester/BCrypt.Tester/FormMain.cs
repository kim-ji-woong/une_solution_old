using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BCrypt.Tester
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            string strInput = textBoxOrigin.Text.Trim();
            string strHash = BCrypt.Net.BCrypt.HashPassword(strInput, BCrypt.Net.BCrypt.GenerateSalt());
            textBoxHash.Text = strHash;
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            string strOrigin = textBoxOrigin2.Text.Trim();
            string strHash = textBoxHash2.Text.Trim();

            if (BCrypt.Net.BCrypt.Verify(strOrigin, strHash))
                MessageBox.Show("같은 데이터입니다.");
            else
                MessageBox.Show("데이터가 일치하지 않습니다.");
        }
    }
}
