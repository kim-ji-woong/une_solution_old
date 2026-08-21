using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteTool
{
    public partial class MainForm : Form
    {
        DbManager m_dbMgr = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnOpenDB_Click(object sender, EventArgs e)
        {
            if (m_dbMgr != null)
            {
                if (MessageBox.Show("기존 연결을 종료할까요?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    m_dbMgr.Close();
                    NoConnection();
                }
                else
                    return; 
            } 

            OpenFileDialog dig = new OpenFileDialog();
            dig.Filter = "(*.db)|*.db";
            if (dig.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtFilePath.Text = dig.FileName;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            bool suc = false;
            try
            {
                m_dbMgr = new DbManager();

                if (chkUsePw.Checked)
                {
                    if (txtPw.Text.Length == 0)
                        throw new ApplicationException("비밀번호를 입력하세요\r\n비밀번호가 없다면 체크 해제하세요");

                    suc = m_dbMgr.Connect(txtFilePath.Text, txtPw.Text);
                }
                else
                    suc = m_dbMgr.Connect(txtFilePath.Text, "");

                if (suc)
                {
                    GetTableList();
                    MessageBox.Show("연결 성공");
                }
                else
                {
                    NoConnection();
                    MessageBox.Show("연결 실패");
                }
            }
            catch (Exception ex)
            {
                NoConnection();
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEnc_Click(object sender, EventArgs e)
        {
            ChgPassword frm = new ChgPassword(m_dbMgr);
            frm.StartPosition = FormStartPosition.CenterParent;
            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
            {
                GetTableList();
                MessageBox.Show("변경 후 재접속되었습니다.");
                txtPw.Text = frm.strChgPw;
            }
            else
            {
                NoConnection();
                MessageBox.Show("실패. 연결이 끊어졌으니 재접속 해야함");
            }
        }

        private void btnDec_Click(object sender, EventArgs e)
        {
            bool suc = m_dbMgr.ChgPw("");
            if (suc)
            {
                txtPw.Text = "";
                GetTableList();
                MessageBox.Show("변경 후 재접속되었습니다.");
            }
            else
            {
                NoConnection();
                MessageBox.Show("실패. 연결이 끊어졌으니 재접속 해야함");
            }
        }
        
        private void chkUsePw_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUsePw.Checked)
            {
                txtPw.ReadOnly = false;
            }
            else
            {
                txtPw.ReadOnly = true;
            }
        }

        private void GetTableList()
        {
            comboBox1.Items.Clear();

            DataTable dt = m_dbMgr.GetDataTable("SELECT name FROM sqlite_master WHERE type = 'table'");
            if (dt == null || dt.Rows.Count == 0)
                return;

            foreach (DataRow row in dt.Rows)
            {
                comboBox1.Items.Add(row["name"].ToString());
            }

            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;

            if (comboBox1.SelectedItem == null)
                return;

            if (comboBox1.SelectedItem.ToString().Length == 0)
                return;

            DataTable dt = m_dbMgr.GetDataTable("SELECT * FROM " + comboBox1.SelectedItem.ToString());
            if (dt == null || dt.Rows.Count == 0)
                return;

            dataGridView1.DataSource = dt;
        }

        private void NoConnection()
        {
            comboBox1.Items.Clear();
            dataGridView1.DataSource = null;
            m_dbMgr = null;
        }
    }
}
