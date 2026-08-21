using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KpxDbDump
{
    public partial class Form1 : Form
    {
        public static DBUtility.WebDBManager dbMgr;

        public Form1()
        {
            InitializeComponent();

            dbMgr = new DBUtility.WebDBManager(500);
            dbMgr.WebServerURL = "http://183.104.147.144:18080/SOP";
            dbMgr.DatabaseHost = "127.0.0.1";

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToAddRows = false; 
            LoadTables();
        }

        private void LoadTables()
        {
            ArrayList arrResult = dbMgr.GetResultData("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'KPX'", 0);
            if (arrResult == null) return;

            for (int i = 0; i < arrResult.Count; i++)
            {
                string tableName = DBUtility.WebDBManager.GetStringField(arrResult[i]);

                comboBox1.Items.Add(tableName);                
            }

            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
        }

        /// <summary>
        /// columnName, columnDataType
        /// </summary>
        Dictionary<string, string> dicCurTableInfo = new Dictionary<string, string>();
        DataTable dt = null;
        private void DisplayTableData(string tableName)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                dataGridView1.DataSource = null;
                dicCurTableInfo.Clear();

                //ArrayList arrResult = dbMgr.GetResultData("SELECT count(*) FROM INFORMATION_SCHEMA.columns WHERE table_name='pipe'", 0);
                ArrayList arrResult = dbMgr.GetResultData("select COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.columns where table_schema='kpx' and table_name='" + tableName + "'", 0);
                if (arrResult == null) return;

                int columnCount = arrResult.Count / 2;

                dt = new DataTable();

                for (int i = 0; i < arrResult.Count; i += 2)
                {
                    string columnName = DBUtility.WebDBManager.GetStringField(arrResult[i]);
                    string columnType = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);

                    //dataGridView1.Columns.Add(columnName, columnName);
                    dt.Columns.Add(columnName);
                    dicCurTableInfo.Add(columnName, columnType);
                }

                arrResult = dbMgr.GetResultData("select * from " + tableName, 0);
                if (arrResult == null) return;

                List<object[]> objs = new List<object[]>();
                List<object> obj = new List<object>();
                 
                for (int i = 0; i < arrResult.Count; i += columnCount)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        obj.Add(arrResult[i + j].ToString()); 
                    }
                    dt.Rows.Add(obj.ToArray());
                    objs.Add(obj.ToArray());
                    obj.Clear();
                }

                dataGridView1.DataSource = dt;

                //foreach (object[] item in objs)
                //{
                //    dataGridView1.Rows.Add(item);
                //}
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message);
            }      
        } 

        private void button_search_Click(object sender, EventArgs e)
        {
            DisplayTableData(comboBox1.SelectedItem.ToString());
        }

        private void button_script_Click(object sender, EventArgs e)
        { 
            if (dicCurTableInfo.Count == 0) return;

            string filePath = Application.StartupPath + @"\Script\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + comboBox1.SelectedItem.ToString() + ".log";
            string dirPath = Application.StartupPath + @"\Script";

            DirectoryInfo di = new DirectoryInfo(dirPath);
            FileInfo fi = new FileInfo(filePath);
             
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (!di.Exists) Directory.CreateDirectory(dirPath);
                if (!fi.Exists)
                { 
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        List<string> columnNames = new List<string>();
                        foreach (KeyValuePair<string, string> item in dicCurTableInfo)
                        {
                            columnNames.Add(item.Key);
                        }

                        StringBuilder sb = null;
                        sb = new StringBuilder();
                        
                        //1. 
                        //foreach (DataGridViewRow row in dataGridView1.Rows)
                        //{
                        //    int nCount = 0;
                        //    sb.Clear();
                        //    sb.Append("INSERT INTO " + comboBox1.SelectedItem.ToString() + "(" + string.Join(", ", columnNames.ToArray()) + ") VALUES (");
                        //    foreach (KeyValuePair<string, string> item in dicCurTableInfo)
                        //    {
                        //        object rowVal = row.Cells[item.Key].Value;
                        //        if (rowVal.ToString() == "null")
                        //            sb.Append("NULL");
                        //        else
                        //        {
                        //            if (item.Value == "int")
                        //                sb.Append(Convert.ToInt32(rowVal));
                        //            else if (item.Value == "varchar")
                        //                sb.Append("'" + rowVal.ToString() + "'");
                        //            else if (item.Value == "double")
                        //                sb.Append(Convert.ToDouble(rowVal));
                        //            else if (item.Value == "datetime")
                        //                sb.Append("'" + Convert.ToDateTime(rowVal) + "'");
                        //            else
                        //                sb.Append(rowVal.ToString());
                        //        }

                        //        if (nCount < dicCurTableInfo.Count - 1)
                        //            sb.Append(", ");
                        //        nCount++;
                        //    }
                        //    sb.Append(");");
                        //    sw.WriteLine(sb.ToString());
                        //    sw.Flush();
                        //}                         

                        //2.
                        foreach (DataRow row in dt.Rows)
                        {
                            int nCount = 0;
                            sb = new StringBuilder();
                            sb.Append("INSERT INTO " + comboBox1.SelectedItem.ToString() + "(" + string.Join(", ", columnNames.ToArray()) + ") VALUES (");
                            foreach (KeyValuePair<string, string> item in dicCurTableInfo)
                            {
                                object rowVal = row[item.Key];
                                if (rowVal.ToString() == "null")
                                    sb.Append("NULL");
                                else
                                {
                                    if (item.Value == "int")
                                        sb.Append(Convert.ToInt32(rowVal));
                                    else if (item.Value == "varchar")
                                        sb.Append("'" + rowVal.ToString() + "'");
                                    else if (item.Value == "double")
                                        sb.Append(Convert.ToDouble(rowVal));
                                    else if (item.Value == "datetime")
                                        sb.Append("'" + Convert.ToDateTime(rowVal) + "'");
                                    else
                                        sb.Append(rowVal.ToString());
                                }

                                if (nCount < dicCurTableInfo.Count - 1)
                                    sb.Append(", ");
                                nCount++;
                            }
                            sb.Append(");");
                            sw.WriteLine(sb.ToString());
                            sw.Flush();
                        }
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filePath))
                    {
                        List<string> columnNames = new List<string>();
                        foreach (KeyValuePair<string, string> item in dicCurTableInfo)
                        {
                            columnNames.Add(item.Key);
                        }

                        StringBuilder sb = null;

                        foreach (DataRow row in dt.Rows)
                        {
                            int nCount = 0;
                            sb = new StringBuilder();
                            sb.Append("INSERT INTO " + comboBox1.SelectedItem.ToString() + "(" + string.Join(", ", columnNames.ToArray()) + ") VALUES (");
                            foreach (KeyValuePair<string, string> item in dicCurTableInfo)
                            {
                                object rowVal = row[item.Key];
                                if (rowVal.ToString() == "null")
                                    sb.Append("NULL");
                                else
                                {
                                    if (item.Value == "int")
                                        sb.Append(Convert.ToInt32(rowVal));
                                    else if (item.Value == "varchar")
                                        sb.Append("'" + rowVal.ToString() + "'");
                                    else if (item.Value == "double")
                                        sb.Append(Convert.ToDouble(rowVal));
                                    else if (item.Value == "datetime")
                                        sb.Append("'" + Convert.ToDateTime(rowVal) + "'");
                                    else
                                        sb.Append(rowVal.ToString());
                                }

                                if (nCount < dicCurTableInfo.Count - 1)
                                    sb.Append(", ");
                                nCount++;
                            }
                            sb.Append(");");
                            sw.WriteLine(sb.ToString());
                            sw.Flush();
                        }
                        sw.Close();
                    }
                }
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message);                
            }
        }

        private void button_sqlSearch_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                //dataGridView2.DataSource = null; 

                ////ArrayList arrResult = dbMgr.GetResultData("SELECT count(*) FROM INFORMATION_SCHEMA.columns WHERE table_name='pipe'", 0);
                //ArrayList arrResult = dbMgr.GetResultData(textBox1.Text, 0);            
                //if (arrResult == null) return;
                
                //List<object[]> objs = new List<object[]>();
                //List<object> obj = new List<object>();
                
                //for (int i = 0; i < arrResult.Count; i += columnCount)
                //{
                //    for (int j = 0; j < columnCount; j++)
                //    {
                //        obj.Add(arrResult[i + j].ToString());
                //    }
                //    dt.Rows.Add(obj.ToArray());
                //    objs.Add(obj.ToArray());
                //    obj.Clear();
                //}

                //dataGridView1.DataSource = dt;

                ////foreach (object[] item in objs)
                ////{
                ////    dataGridView1.Rows.Add(item);
                ////}
                //this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message);
            }      
        }

        private void button1_ex_Click(object sender, EventArgs e)
        {
            ArrayList aa = dbMgr.GetResultData(textBox1.Text, 0); 
        }
    }
}
