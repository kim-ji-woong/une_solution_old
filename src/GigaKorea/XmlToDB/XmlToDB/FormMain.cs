using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace XmlToDB
{
    public partial class FormMain : Form
    {
        private string m_strWaitMessage = "XML 파일을 끌어다 놓으세요.";
        private int m_nMessageCount = 0;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Count() == 1)
                {
                    string strFileName = files[0].ToLower();

                    if (strFileName.EndsWith("xml"))
                    {
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                }
            }

            e.Effect = DragDropEffects.None;
        }

        private void FormMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Count() == 1)
            {
                string strFileName = files[0].ToLower();

                if (strFileName.EndsWith("xml"))
                {
                    labelHelp.AllowDrop = this.AllowDrop = false;
                    this.Cursor = Cursors.WaitCursor;

                    string strConnection = ConfigurationManager.ConnectionStrings["BIMDBConnection"].ConnectionString;

                    using (SqlConnection connection = new SqlConnection(strConnection))
                    {
                        connection.Open();

                        SetMessage("DB 변환중입니다.");
                        DBManager mgr = new DBManager();

                        if (mgr.XmlToDB(strFileName, connection))
                        {
                            this.Cursor = Cursors.Arrow;
                            BeginMessage("DB 입력이 완료되었습니다.", false);
                        }
                        else
                        {
                            this.Cursor = Cursors.Arrow;
                            BeginMessage(mgr.ErrorMessage, true);
                        }
                    }

                    this.Cursor = Cursors.Arrow;
                    labelHelp.AllowDrop = this.AllowDrop = true;
                }
            }
        }

        private void SetMessage(string strMessage)
        {
            if (m_nMessageCount > 0)
            {
                timerMessage.Stop();
                m_nMessageCount = 0;
            }

            labelHelp.ForeColor = Color.Black;
            labelHelp.Text = strMessage;
            this.Refresh();
        }

        private void BeginMessage(string strMessage, bool isError)
        {
            if (m_nMessageCount == 0)
            {
                labelHelp.ForeColor = isError ? Color.Red : Color.Black;
                labelHelp.Text = strMessage;
                timerMessage.Start();
            }
        }

        private void timerMessage_Tick(object sender, EventArgs e)
        {
            if (++m_nMessageCount >= 10)
            {
                m_nMessageCount = 0;
                labelHelp.ForeColor = Color.Black;
                labelHelp.Text = m_strWaitMessage;
                timerMessage.Stop();
            }
        }
    }
}
