using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using FireSimulator;
using System.Xml.Linq;

namespace DBMaker
{
    public partial class FormMain : Form
    {
        private const string XML_VERSION = "1.3";

        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "XML 파일 (*.xml)|*.xml|모든 파일 (*.*)|*.*";
            dlg.Title = "프로젝트 파일 열기";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxXML.Text = dlg.FileName;
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            string strXML = textBoxXML.Text.Trim();

            if (strXML.Length == 0)
            {
                textBoxXML.Focus();
                MessageBox.Show("XML 경로를 입력하세요.");
                return;
            }

            if (System.IO.File.Exists(strXML) == false)
            {
                textBoxXML.Focus();
                MessageBox.Show("유효하지 않은 XML 경로입니다.");
                return;
            }

            Project project = ReadXML(strXML);

            if (project == null)
                MessageBox.Show("형식에 맞지 않는 XML이거나 XML 파일에 오류가 있습니다.");
            else
            {
                if (DBManager.SetData(project))
                    MessageBox.Show("DB 적용이 완료되었습니다.");
                else
                    MessageBox.Show("DB 적용이 실패하였습니다.\r\n" + DBManager.ErrorMessage);
                
                labelProcess.Text = "";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Project ReadXML(string strPath)
        {
            StreamReader reader = new StreamReader(strPath, Encoding.UTF8);
            string strXML = reader.ReadToEnd();
            reader.Close();

            XElement xml = XElement.Parse(strXML);

            if (xml.Name != "IndoorModelFile")
                return null;

            XAttribute attr = xml.Attribute("version");

            if (attr == null || attr.Value != XML_VERSION)
                return null;

            return Project.Read(xml);
        }

        public void ChangeStatus(string strStatus)
        {
            this.Invoke((MethodInvoker)delegate
            {
                labelProcess.Text = strStatus;
                this.Update();
            });
        }
    }
}
