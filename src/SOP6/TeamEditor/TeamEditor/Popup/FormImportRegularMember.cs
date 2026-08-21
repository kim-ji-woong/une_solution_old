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

namespace TeamEditor
{
    public partial class FormImportRegularMember : Form
    {
        private ArrayList m_arrHeaderPosition = null;
        private ArrayList m_arrImportData = null;

        public ArrayList HeaderPosition { get { return m_arrHeaderPosition; } }
        public ArrayList ImportData { get { return m_arrImportData; } }


        public FormImportRegularMember()
        {
            InitializeComponent();

            m_arrHeaderPosition = new ArrayList();
            m_arrImportData = new ArrayList();

            SetHeaderAlignment();
        }


        private void SetHeaderAlignment()
        {
            foreach (DataGridViewColumn column in this.gvMain.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }
        
        public DataGridViewRow MakeNewRow()
        {
            if (this.gvMain.AllowUserToAddRows)
            {
                DataGridViewRow row = (DataGridViewRow)this.gvMain.Rows[this.gvMain.Rows.Count - 1].Clone();
                this.gvMain.Rows.Add(row);

                return this.gvMain.Rows[this.gvMain.Rows.Count - 2];
            }
            else
            {
                this.gvMain.AllowUserToAddRows = true;

                DataGridViewRow row = (DataGridViewRow)this.gvMain.Rows[this.gvMain.Rows.Count - 1].Clone();
                this.gvMain.Rows.Add(row);

                this.gvMain.AllowUserToAddRows = false;
            }

            return this.gvMain.Rows[this.gvMain.Rows.Count - 1];
        }

        
        #region Button Click

        private void btnFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "Excel File |*.csv";

            if (openDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                gvMain.Rows.Clear();
                m_arrImportData.Clear();

                StreamReader sr = new StreamReader(openDlg.FileName, Encoding.GetEncoding("euc-kr"));

                int nindex = 0;
                string strHeaderData = sr.ReadLine(); // 첫행은 컬럼 헤더
                foreach (string headerData in strHeaderData.Split(','))
                {
                    m_arrHeaderPosition.Add(headerData);
                }

                while (sr.EndOfStream == false)
                {
                    // 사번에 따른 중복 체크를 반드시한다.
                    string strData = sr.ReadLine();

                    DataGridViewRow row = MakeNewRow();

                    nindex = 0;

                    foreach (string data in strData.Split(','))
                    {
                        row.Cells[nindex++].Value = data;
                        m_arrImportData.Add(data);
                    }

                }

                sr.Close();

            }

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion Button Click


    }
}
