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

using DBUtility;

namespace TrainingEvaluation
{
    // 의견 내용
    public partial class FormComment : FormBase
    {
        WebDBManager m_dbMan = null;
        int m_evaID = 0;

        public FormComment(WebDBManager dbMan, int evaID)
        {
            InitializeComponent();

            pictureBoxIcon.Visible = false;
            btnMax.Visible = false;
            btnMin.Visible = false;

            m_dbMan = dbMan;
            m_evaID = evaID;

            InitGrid();
        }

        private void InitGrid()
        {
            gridComment.AllowUserToAddRows = false;
            gridComment.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridComment.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            gridComment.Columns.Add("gridComment_column1", "    작성 일자");
            gridComment.Columns.Add("gridComment_column2", "의견 내용");

            for (int i = 0; i < gridComment.ColumnCount; ++i)
            {
                gridComment.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                if (i == 0)
                {
                    gridComment.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    gridComment.Columns[i].Width = 100;
                }
                else
                {
                    gridComment.Columns[i].Width = 700;
                    gridComment.Columns[i].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }
            }

            string query = string.Format("SELECT CreateTime, Comment FROM evaluationcomment WHERE EvaluationHistoryID={0} ORDER BY ID DESC", m_evaID);
            ArrayList arrRes = m_dbMan.GetResultData(query, 0);
            if(arrRes != null && arrRes.Count != 0)
            {
                for (int i = 0; i < arrRes.Count; i+=2)
                {
                    string[] rows = new string[2];
                    DateTime dt = DateTime.Now;
                    rows[0] = WebDBManager.GetDateTimeField(arrRes[i], dt).ToString("yyyy-MM-dd");
                    rows[1] = WebDBManager.GetStringField(arrRes[i+1]);

                    gridComment.Rows.Add(rows);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FormCommentAdd form = new FormCommentAdd();
            form.ShowDialog();

            string strComment = form.comment.Trim();
            if (strComment == "")
                return;

            DateTime dt = DateTime.Now;
            string strTime = dt.ToShortDateString();

            int id = 0;
            string query = "SELECT MAX(ID) FROM evaluationcomment";
            ArrayList arrRes = m_dbMan.GetResultData(query, 0);
            if (arrRes != null && arrRes.Count != 0)
                id = WebDBManager.GetIntField(arrRes[0].ToString(), id);

            query = string.Format("INSERT INTO evaluationcomment (ID, CreateTime, Comment, EvaluationHistoryID) VALUES({0}, '{1}', '{2}', {3})",
                id+1, strTime, strComment, m_evaID);

            if(m_dbMan.GetResultData(query, 0) != null)
            {
                string[] rows = new string[2];
                rows[0] = strTime;
                rows[1] = strComment;

                gridComment.Rows.Add(rows);
            }
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
