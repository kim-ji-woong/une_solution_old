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
using System.IO;

namespace WindowsFormsApplication13
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

			System.Diagnostics.Trace.WriteLine("BEGIN : " + DateTime.Now.ToLongTimeString());
			BackupManager.Instance.BackupData();


			string szFileName = "c:\\temp\\temp.sql";

			StreamReader reader = new StreamReader(szFileName);

			
            string strConn = @"Data Source=:memory:";           
            SQLiteConnection conn = null;
            try
            {

				System.Diagnostics.Trace.WriteLine("BEGIN INSERT : " + DateTime.Now.ToLongTimeString());
                conn = new SQLiteConnection(strConn);
				conn.Open();
				string szSQL = "begin";

				while (szSQL != null && szSQL != "")
				{
					szSQL =  reader.ReadLine();
					if (szSQL != null && szSQL != "")
					{
						//string sql = reader.ReadToEnd();

						SQLiteCommand cmd = new SQLiteCommand(szSQL, conn);
						//System.Diagnostics.Trace.WriteLine("INSERT : " + szSQL);
						cmd.ExecuteNonQuery();
					}
					
				}
               
                //conn.ChangePassword("1234");


				

				System.Diagnostics.Trace.WriteLine("END INSERT : " + DateTime.Now.ToLongTimeString());
            }
            catch (Exception e)
            {
                Console.WriteLine ("Caught exception: " + e.Message);
				if (conn != null)
				{
					conn.Close();
				}
            }
            finally 
            {
                
            }


			SQLiteConnection source = new SQLiteConnection("Data Source=c:\\temp\\test.db");
			source.Open();
			// save memory db to file
			conn.BackupDatabase(source, "main", "main", -1, null, 0);
			source.Close();
			conn.Close();

			System.Diagnostics.Trace.WriteLine("DUMP END : " + DateTime.Now.ToLongTimeString());
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
