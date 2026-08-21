using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KpxUserAcceptance
{
    public partial class MainForm : Form
    {
        public static DBUtility.WebDBManager dbMgr;

        private WaitManager m_waitManager = new WaitManager();
        private UserManager m_userManager = new UserManager();
        private UserGroupManager m_userGroupManager = new UserGroupManager();

        public MainForm()
        {
            InitializeComponent();
             
            dbMgr = new DBUtility.WebDBManager(500);
            dbMgr.DatabaseName = "KPX";
            dbMgr.DatabasePort = "3306";
            dbMgr.DatabaseType = DBUtility.WebDBManager.DBType.mysql;            
            dbMgr.WebServerURL = Login.WebServerURL;
            dbMgr.DatabaseHost = "127.0.0.1";
            InitDBManager(dbMgr);

            InitGridView();
            
            this.FormClosed += MainForm_FormClosed;
        }

        public static void InitDBManager(DBUtility.WebDBManager mgr)
        {
            /*mgr.WebServerURL = Login.WebServerURL;
            mgr.DatabaseHost = "127.0.0.1";
            mgr.DatabaseName = "KPX";
            mgr.DatabaseType = DBUtility.WebDBManager.DBType.mysql;*/
        }

        void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        #region GridView 초기화
        private void InitGridView()
        {
            Color color = Color.FromArgb(158, 222, 239);

            m_waitManager.DBManager = dbMgr;
            m_waitManager.Grid = dataGridView_wait;
            m_waitManager.InitGrid(color);

            m_userManager.DBManager = dbMgr;
            m_userManager.Grid = dataGridView_user;
            m_userManager.ContextMenu = menuUser;
            m_userManager.InitGrid(color);

            m_userGroupManager.DBManager = dbMgr;
            m_userGroupManager.Grid = gridUserGroup;
            m_userGroupManager.ContextMenu = menuUserGroup;
            m_userGroupManager.InitGrid(color);

            button_watiSearch_Click(null, null);
        }
        #endregion

        #region 버튼 이벤트
        private void button_waitOk_Click(object sender, EventArgs e)
        {
            m_waitManager.Save();
        }

        private void button_watiSearch_Click(object sender, EventArgs e)
        {
            m_waitManager.ReadUserGroups();
            m_waitManager.Refresh();
        }

        private void button_modifyOk_Click(object sender, EventArgs e)
        {
            m_userManager.Save();
        }

        private void button_modifySearch_Click(object sender, EventArgs e)
        {
            m_userManager.ReadUserGroups();
            m_userManager.Refresh();
        } 
        #endregion

        private void 관리자변경ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdminRegister pop = new AdminRegister();
            if (pop.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("관리자가 변경되었습니다.");
            }
        }

        private void 비밀번호변경ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangePassword pop = new ChangePassword();
            if (pop.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("비밀번호가 변경되었습니다.");

            }
        }

        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = tabControl1.SelectedIndex;

            if (nSelectedIndex < 0 || nSelectedIndex >= tabControl1.TabCount)
                return;

            TabPage page = tabControl1.TabPages[nSelectedIndex];

            if (page == tabPage_wait)
                button_watiSearch_Click(null, null);
            else if (page == tabPage_user)
                button_modifySearch_Click(null, null);
            else if (page == tabPage_userGroup)
                btnSearch_Click(null, null);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            m_userGroupManager.Refresh();
        }

        /*private void btnEdit_Click(object sender, EventArgs e)
        {
            gridUserGroup.ReadOnly = false;
            gridUserGroup.AllowUserToAddRows = true;

            foreach (DataGridViewRow row in gridUserGroup.Rows)
            {
                row.Cells[0].ReadOnly = true;
            }
        }*/

        private void btnApply_Click(object sender, EventArgs e)
        {
            m_userGroupManager.Save();
        }
    }

    public class ComboBoxItem
    {
        public int Value { get; set; }
        public string Display { get; set; }
        public ComboBoxItem(int value, string display)
        {
            this.Value = value;
            this.Display = display;
        }
    }
}
