using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KpxUserAcceptance
{
    public partial class AdminRegister : Form
    {
        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });
        public static DBUtility.WebDBManager dbMgr;

        public string adminPhoneNumber = string.Empty;

        public AdminRegister()
        {
            InitializeComponent();

            dbMgr = new DBUtility.WebDBManager(500);
            dbMgr.DatabaseName = "KPX";
            dbMgr.DatabasePort = "3306";
            dbMgr.DatabaseType = DBUtility.WebDBManager.DBType.mysql;
            dbMgr.WebServerURL = Login.WebServerURL;
            dbMgr.DatabaseHost = "127.0.0.1";
            MainForm.InitDBManager(dbMgr);

            Color color = Color.FromArgb(158, 222, 239);
             
            SettingGridView(dataGridView1, "Id", "ID", color);
            dataGridView1.Columns["Id"].Visible = false;
            SettingGridView(dataGridView1, "UserName", "사용자명", color);
            SettingGridView(dataGridView1, "PhoneNumber", "핸드폰 번호", color);

            DisplayUser();
        }

        public void SettingGridView(DataGridView gridView, string columnsName, string headerText, Color colHeaderBackground, int columnsWidth = 0)
        {
            gridView.Columns.Add(columnsName, headerText);
            gridView.Columns[columnsName].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridView.Columns[columnsName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridView.RowHeadersVisible = false;
            gridView.AllowUserToAddRows = false;
            gridView.RowHeadersVisible = false;
            gridView.ReadOnly = true;
            gridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridView.BackgroundColor = Color.White;
            gridView.ColumnHeadersDefaultCellStyle.BackColor = colHeaderBackground;
            gridView.EnableHeadersVisualStyles = false;
            gridView.Columns[columnsName].SortMode = DataGridViewColumnSortMode.NotSortable;
            gridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            gridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            gridView.RowTemplate.Height = gridView.ColumnHeadersHeight = 40;
            gridView.MultiSelect = false;

            if (columnsWidth != 0)
            {

                gridView.Columns[columnsName].Width = columnsWidth;
                gridView.Columns[columnsName].MinimumWidth = columnsWidth;
            }
        }

        private void DisplayUser()
        {
            try
            {
                dataGridView1.Rows.Clear();
                string strQuery = "SELECT ID, UserName, PhoneNumber FROM User WHERE Mobile=1 AND id NOT IN (select userid from admin)";

                ArrayList arrResult = dbMgr.GetResultData(strQuery, 0);
                if (arrResult == null) return;

                for (int i = 0; i < arrResult.Count; i += 3)
                {
                    int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strUserName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);
                    string strPhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 2]); 

                    string strDECPhoneNumber = string.Empty;
                    if (strPhoneNumber.Length > 0)
                        strDECPhoneNumber = DBUtility.AES256Cipher.AES_decrypt(strPhoneNumber, key);

                    dataGridView1.Rows.Add(nID, strUserName, strDECPhoneNumber);
                }
            }
            catch (ApplicationException app)
            {
                MessageBox.Show(app.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow == null) throw new ApplicationException("관리자를 선택하세요.");

                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Id"].Value);
                adminPhoneNumber = dataGridView1.CurrentRow.Cells["PhoneNumber"].Value.ToString();

                string strQuery = "UPDATE Admin SET UserID=" + id;

                if (dbMgr.GetResultData(strQuery, 0) != null)
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            catch (ApplicationException app)
            {
                MessageBox.Show(app.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
