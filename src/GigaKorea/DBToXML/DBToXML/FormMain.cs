using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using System.Collections;
using System.IO;

namespace DBToXML
{
    using Data;

    public partial class FormMain : Form
    {
        private WebDBManager m_dbMgr = new WebDBManager("UnE_BIM", 1);
        //private string m_strIniFileName = "path.ini";

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            string strSQL = "Select ID, Name, UnitOfLength, TimeStamp, Author from Project";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-4;i+=5)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> unit = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<DateTime> time = WebDBManager.GetDateTimeField(arrResult[i + 3]);
                string strAuthor = WebDBManager.GetStringField(arrResult[i + 4]);

                if (id == null || strName == null || unit == null || time == null)
                    continue;

                Project project = new Project();

                project.ID = id.Data;
                project.Name = strName;
                project.UnitType = (Project.UnitOfLength)unit.Data;
                project.TimeStamp = time.Data;
                project.Author = strAuthor;

                cboProjects.Items.Add(project);
            }

            if (cboProjects.Items.Count > 0)
                cboProjects.SelectedIndex = 0;
            else
                MessageBox.Show("DB에 데이터가 존재하지 않습니다.");

            //ReadIniFile();
        }

        /*private void ReadIniFile()
        {
            string strFilePath = GetIniFilePath();

            if (File.Exists(strFilePath) == false)
                return;

            StreamReader reader = new StreamReader(strFilePath, Encoding.UTF8);

            while (reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length > 0)
                {
                    textBoxXMLPath.Text = strLine;
                    checkBoxRememberPath.Checked = true;
                    break;
                }
            }

            reader.Close();
        }*/

        private void btnSavePath_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "XML Files|*.xml";
            dlg.FilterIndex = 0;
            dlg.Title = "XML 파일 저장";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                if (checkBoxSameAsProject.Checked)
                {
                    if (CheckSameAsProject(dlg.FileName))
                        return;
                }

                string strFileName = dlg.FileName;
                textBoxXMLPath.Text = strFileName;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string strFilePath = textBoxXMLPath.Text.Trim();

            if (strFilePath.Length > 0)
            {
                if (cboProjects.SelectedIndex < 0)
                {
                    cboProjects.Focus();
                    MessageBox.Show("저장할 Project가 선택되지 않았습니다.");
                    return;
                }

                Project project = (Project)cboProjects.Items[cboProjects.SelectedIndex];

                XMLManager mgr = new XMLManager();

                this.Cursor = Cursors.WaitCursor;
                bool result = mgr.SaveXML(strFilePath, project, m_dbMgr);
                this.Cursor = Cursors.Arrow;

                if (result)
                    MessageBox.Show("XML 파일이 생성되었습니다.");
                else
                    MessageBox.Show("XML 파일 생성이 실패하였습니다.");
            }
            else
            {
                textBoxXMLPath.Focus();
                MessageBox.Show("저장할 XML 파일의 경로를 입력하세요.");
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*string strFilePath = GetIniFilePath();

            if (checkBoxRememberPath.Checked)
            {
                StreamWriter writer = new StreamWriter(strFilePath, false, Encoding.UTF8);
                writer.Write(strFilePath);
                writer.Close();
            }
            else
            {
                if (File.Exists(strFilePath))
                    File.Delete(strFilePath);
            }*/
        }

        /*private string GetIniFilePath()
        {
            int nIndex = Application.ExecutablePath.LastIndexOf('\\');

            if (nIndex > 0)
            {
                string strFilePath = Application.ExecutablePath.Substring(0, nIndex + 1) + m_strIniFileName;
                return strFilePath;
            }

            return "";
        }*/

        private void checkBoxSameAsProject_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSameAsProject.Checked)
            {
                string strFilePath = textBoxXMLPath.Text.Trim();
                CheckSameAsProject(strFilePath);
            }
        }

        private bool CheckSameAsProject(string strFilePath)
        {
            if (cboProjects.SelectedIndex < 0)
                return false;

            Project project = (Project)cboProjects.Items[cboProjects.SelectedIndex];

            if (strFilePath.Length > 0)
            {
                int nIndex = strFilePath.LastIndexOf('\\');

                if (nIndex >= 0)
                {
                    string strFolderPath = strFilePath.Substring(0, nIndex + 1);
                    strFilePath = strFolderPath + project.Name + ".xml";
                    textBoxXMLPath.Text = strFilePath;
                    return true;
                }
            }
        
            return false;
        }

        private void cboProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProjects.SelectedIndex >= 0)
            {
                string strFilePath = textBoxXMLPath.Text.Trim();

                if (strFilePath.Length > 0)
                {
                    CheckSameAsProject(strFilePath);
                }
            }
        }
    }
}
