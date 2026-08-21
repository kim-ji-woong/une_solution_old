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

namespace HWPReportMaker
{
    public partial class FormMain : Form
    {
        private List<Project> m_listProject = new List<Project>();
        private string m_strResultFilePath = "";

        public FormMain()
        {
            InitializeComponent();
            Init();
        }

        public FormMain(string strFolderPath, string strTargetPath, string strResultFilePath)
        {
            InitializeComponent();
            Init();
            MakeReport(strFolderPath, strTargetPath, strResultFilePath, true);
        }

        private void Init()
        {
            axHwpCtrl1.CreateControl();

            //보안모듈 등록
            const string strRegRoot = @"SoftWare\HNC\HwpCtrl\Modules";
            string strFilePath = Application.StartupPath + @"\FilePathCheckerModule.dll";

            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(strRegRoot, true);
            if (regKey == null)
                regKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(strRegRoot);

            regKey.SetValue("FilePathCheckerModule", strFilePath);
            regKey.Close();
        }

        public void MakeReport(string strFolderPath, string strTargetPath, string strResultFilePath, bool noSave)
        {
            if (!ReadXMLFiles(strFolderPath))
                return;

            m_strResultFilePath = strResultFilePath;

            HWPManager mgr = new HWPManager(m_listProject, axHwpCtrl1);
            bool result = mgr.Save(strTargetPath, noSave);

            if (!noSave)
            {
                System.IO.StreamWriter writer = new StreamWriter(m_strResultFilePath, false, Encoding.UTF8);
                writer.WriteLine(result ? 1 : 0);
                writer.Close();
            }
        }

        private bool ReadXMLFiles(string strFolderPath)
        {
            string[] arrFiles = Directory.GetFiles(strFolderPath);
            XMLManager mgr = new XMLManager();

            List<string> arrXMLFiles = new List<string>();

            foreach (string strFile in arrFiles)
            {
                int nIndex = strFile.LastIndexOf('.');

                if (nIndex < 0)
                    continue;

                string strExt = strFile.Substring(nIndex + 1);

                if (string.Compare(strExt, "xml", true) == 0)
                {
                    Project prj = mgr.ReadXML(strFile);

                    if (prj == null)
                        return false;
                    else
                        m_listProject.Add(prj);

                    arrXMLFiles.Add(strFile);
                }
            }

            // 읽은 XML은 삭제한다.
            foreach (string strFile in arrXMLFiles)
            {
                File.Delete(strFile);
            }

            return true;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
                    
            dlg.Filter = "아래한글 files (*.hwp)|*.hwp|All files (*.*)|*.*";
			dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.axHwpCtrl1.SaveAs(dlg.FileName);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.IO.StreamWriter writer = new StreamWriter(m_strResultFilePath, false, Encoding.UTF8);
            writer.Close();
        }
    }
}
