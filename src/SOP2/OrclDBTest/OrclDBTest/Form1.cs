using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Data.OleDb;

namespace OrclDBTest
{
    public partial class Form1 : Form
    {

        private OleDbConnection m_Connection;
        private OleDbCommand m_Command;

        private bool m_isConnection = false;
        ArrayList arrUser = new ArrayList();

        public Form1()
        {
            InitializeComponent();

            m_Connection = new OleDbConnection();

            m_Connection.ConnectionString = string.Format("Provider=OraOLEDB.Oracle;USER ID={0};PASSWORD={1};DATA SOURCE={2};OLEDB.NET=True;",
                                                             "UNE",
                                                             "99669966",
                                                            "ORCL2");

            m_Command = new OleDbCommand("SELECT * FROM COMPANYMEMBER", m_Connection);

            try
            {
                m_Connection.Open();
                
                OleDbDataReader reader = m_Command.ExecuteReader();
                while (reader.Read())
                {
                    CompanyMember dataNew = new CompanyMember();
                    dataNew.MemberID = (int)((Decimal)reader.GetValue(0));
                    dataNew.MemberName = (string)reader.GetValue(1);
                  
                    arrUser.Add(dataNew);
                }

                foreach (CompanyMember member in arrUser)
                {
                    DataGridViewRow gridRow = new DataGridViewRow();
                    DataGridViewCell cell = null;

                    cell = new DataGridViewTextBoxCell();
                    cell.Value = member.MemberID;
                    gridRow.Cells.Add(cell);

                    cell = new DataGridViewTextBoxCell();
                    cell.Value = member.MemberName;
                    gridRow.Cells.Add(cell);

                    dataGridView1.Rows.Add(gridRow);
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                m_Connection.Close();
            }

        }

        public T GetField<T>(object dataSrc, T dataDefault)
        {
            T result;

            try
            {
                result = (T)dataSrc;
            }
            catch (Exception)
            {
                result = dataDefault;
            }

            return result;
        }
    }

    public class CompanyMember
    {
        private int m_nMemberID;
        private string m_strMemberName;

        public int MemberID
        {
            get { return m_nMemberID; }
            set { m_nMemberID = value; }
        }
        public string MemberName
        {
            get { return m_strMemberName; }
            set { m_strMemberName = value; }
        }
    }
}
