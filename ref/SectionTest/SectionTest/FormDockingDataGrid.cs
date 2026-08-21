using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Data.Odbc;

namespace section
{

    public partial class FormDockingDataGrid : Form
    {

        private OleDbDataAdapter adapter = null;
        private DataSet ds = new DataSet();

        public FormDockingDataGrid()
        {
            InitializeComponent();

            
            string szDBPath = Application.StartupPath + "\\" + "Database1.accdb";
            
            bool bCreateTabe = false;
            // MDB파일이 없으면 만들어준다.
            if (File.Exists(szDBPath) == false)
            {
                FileStream fs = new FileStream(szDBPath, FileMode.CreateNew, FileAccess.Write);
                BinaryWriter writer = new BinaryWriter(fs, new ASCIIEncoding());
                writer.Write(NewMDB.FileData, 0, NewMDB.FileData.Length);
                writer.Close();
                fs.Close();                
                bCreateTabe = true;
            }         

            OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + szDBPath);
            conn.Open();

            if (bCreateTabe)
            {
                // 새로 만든 경우 Table을 Create 해준다.
                try
                {
                    string szSQL = @"create table Rectangle ([r_num] number,[p_num] number,[r_content] TEXT(255),[r_width] number,[r_height] number, [r_x] number,[r_y] number )";
                    OleDbCommand temp = conn.CreateCommand();
                    temp.CommandText = szSQL;
                    temp.ExecuteNonQuery();
                    temp.Dispose();
                }
                catch (System.Exception ex)
                {                    
                }             
            }

            string sql = "select * from Rectangle";
            adapter = new OleDbDataAdapter(sql, conn);
                        
            adapter.Fill(ds, "Rectangle");
            dataGridView1.DataSource = ds.Tables["Rectangle"];

            conn.Close();
        }

        public void UpdateData()
        {
            if (adapter != null)
            {
                ds.Clear();
                adapter.Fill(ds, "Rectangle");
                dataGridView1.DataSource = ds.Tables["Rectangle"];
            }
        }
    }
}
