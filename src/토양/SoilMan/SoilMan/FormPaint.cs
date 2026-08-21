using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoilMan
{
    public partial class FormPaint : Form
    {
        FormMain form = null;
        public FormPaint()
        {
            InitializeComponent();
            form = FormMain.Instance;
        }


        public void RefreshEvent(bool bRefresh)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
            cell1.Value = dataGridView1.Rows.Count + 1;
            row.Cells.Add(cell1);

            DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
            cell2.Value = "Refresh";
            row.Cells.Add(cell2);

            DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
            cell3.Value = bRefresh;
            row.Cells.Add(cell3);


            dataGridView1.Rows.Add(row);
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            textBox1.Text = textBox1.Text + "\n\rREFRESH EVENT";
            textBox1.Text = textBox1.Text + st.ToString();
        }


        private DateTime beingPaint;
        private DateTime endPaint;
        public void BeginPaint(bool bImage)
        {
            label1.ForeColor = Color.Blue;           
            label1.Text = "Begin Paint";
            beingPaint = DateTime.Now;
        }

        public void EndPaint(bool bImage)
        {            
            label1.ForeColor = Color.Red;
            label1.Text = "End Paint";

            endPaint = DateTime.Now;

            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
            cell1.Value = dataGridView1.Rows.Count + 1;
            row.Cells.Add(cell1);

            DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
            cell2.Value = (endPaint - beingPaint).TotalMilliseconds;
            row.Cells.Add(cell2);

            DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
            cell3.Value = bImage;
            row.Cells.Add(cell3);


            dataGridView1.Rows.Add(row);


            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            textBox1.Text = textBox1.Text + "\n\rPAINT EVENT";
            textBox1.Text = textBox1.Text + st.ToString();
        }

        private DateTime beingPan;
        private DateTime endPan;
        public void BeginPan()
        {
            beingPan = DateTime.Now;
            
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
            cell1.Value = dataGridView1.Rows.Count + 1;
            row.Cells.Add(cell1);

            DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
            cell3.Value = "-";
            row.Cells.Add(cell3);

            DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
            cell2.Value = "START";
            row.Cells.Add(cell2);

            dataGridView1.Rows.Add(row);
        }

        public void EndPan()
        {

            endPan = DateTime.Now;

            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
            cell1.Value = dataGridView1.Rows.Count + 1;
            row.Cells.Add(cell1);

            DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
            cell3.Value = "-";
            row.Cells.Add(cell3);

            DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
            cell2.Value = (endPan - beingPan).TotalMilliseconds;
            row.Cells.Add(cell2);

            dataGridView1.Rows.Add(row);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.IO.File.WriteAllText(@"C:\temp\refresh_stack.txt", textBox1.Text);
            textBox1.Text = "";
        }
    }
}
