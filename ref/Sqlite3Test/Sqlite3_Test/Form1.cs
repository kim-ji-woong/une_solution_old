using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace WindowsFormsApplication13
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


            string strConn = @"Data Source=C:\Temp\mydb.db;Password=1234;";
            string sql = GetTableSQL ();
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(strConn);
                
                conn.Open();
                //conn.ChangePassword("1234");

                SQLiteCommand cmd = new SQLiteCommand (sql, conn);
                cmd.ExecuteNonQuery ();

                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine ("Caught exception: " + e.Message);
            }
            finally 
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        static string GetTableSQL () 
        {
            return @"
                CREATE TABLE [Employees] (
                [EmpID] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                [EmpName] NVARCHAR(128) UNIQUE NOT NULL,
                [EmpSalary] FLOAT NOT NULL
                );
            ";
        }

    }
}
