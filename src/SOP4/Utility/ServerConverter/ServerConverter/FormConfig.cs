using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;

namespace ServerConverter
{
    public partial class FormConfig : Form
    {
        private class ServerOption
        {
            private string m_strServerURL = "";
            private string m_strFile1 = "";
            private string m_strFile2 = "";
            private string m_strFile3 = "";
            private int m_nFileIndex1 = -1;
            private int m_nFileIndex2 = -1;
            private int m_nFileIndex3 = -1;

            public string ServerURLPath
            {
                get { return m_strServerURL; }
                set { m_strServerURL = value; }
            }

            public string File1Path
            {
                get { return m_strFile1; }
                set { m_strFile1 = value; }
            }

            public string File2Path
            {
                get { return m_strFile2; }
                set { m_strFile2 = value; }
            }

            public string File3Path
            {
                get { return m_strFile3; }
                set { m_strFile3 = value; }
            }

            public int File1Index
            {
                get { return m_nFileIndex1; }
                set { m_nFileIndex1 = value; }
            }

            public int File2Index
            {
                get { return m_nFileIndex2; }
                set { m_nFileIndex2 = value; }
            }

            public int File3Index
            {
                get { return m_nFileIndex3; }
                set { m_nFileIndex3 = value; }
            }
        }

        private const string LocalServerURL = "LocalServerURL";
        private const string RemoteServerURL = "RemoteServerURL";

        private const string LocalFile1 = "LocalFile1";
        private const string LocalFile2 = "LocalFile2";
        private const string LocalFile3 = "LocalFile3";
        private const string RemoteFile1 = "RemoteFile1";
        private const string RemoteFile2 = "RemoteFile2";
        private const string RemoteFile3 = "RemoteFile3";

        private DataGridViewRow m_rowLocal = null;
        private DataGridViewRow m_rowRemote = null;
        private RegistryKey m_keyOpt = null;

        private ServerOption m_optLocal = new ServerOption();
        private ServerOption m_optRemote = new ServerOption();

        public FormMain.Option Option
        {
            get { return ReadOption(); }
            set { WriteOption(value); }
        }

        public FormConfig()
        {
            InitializeComponent();

            InitGrid();
            ReadRegistry();
        }

        private void FormConfig_Load(object sender, EventArgs e)
        {
            ReadRegistry();
        }

        private void InitGrid()
        {
            m_rowLocal = MakeNewRow();
            m_rowRemote = MakeNewRow();

            m_rowLocal.HeaderCell.Value = "로컬서버";
            m_rowRemote.HeaderCell.Value = "원격서버";

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private DataGridViewRow MakeNewRow()
        {
            if (dataGridView1.AllowUserToAddRows)
            {
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Clone();
                dataGridView1.Rows.Add(row);

                return dataGridView1.Rows[dataGridView1.Rows.Count - 2];
            }
            else
            {
                dataGridView1.AllowUserToAddRows = true;

                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Clone();
                dataGridView1.Rows.Add(row);

                dataGridView1.AllowUserToAddRows = false;
            }

            return dataGridView1.Rows[dataGridView1.Rows.Count - 1];
        }

        private void ReadRegistry()
        {
            if (m_keyOpt != null)
            {
                m_keyOpt.Close();
                m_keyOpt = null;
            }

            string szKey = "Software";
            RegistryKey keySoftware = Registry.CurrentUser.OpenSubKey(szKey, true);

            if (keySoftware == null)
                return;

            szKey = "UNE";
            RegistryKey keyUnE = keySoftware.OpenSubKey(szKey, true);

            if (keyUnE == null)
            {
                keyUnE = keySoftware.CreateSubKey(szKey);
            }

            szKey = "DevServerOption";
            m_keyOpt = keyUnE.OpenSubKey(szKey, true);

            if (m_keyOpt == null)
            {
                m_keyOpt = keyUnE.CreateSubKey(szKey);

                m_keyOpt.SetValue(LocalServerURL, "");
                m_keyOpt.SetValue(RemoteServerURL, "");
                m_keyOpt.SetValue(LocalFile1, "");
                m_keyOpt.SetValue(RemoteFile1, "");
                m_keyOpt.SetValue(LocalFile2, "");
                m_keyOpt.SetValue(RemoteFile2, "");
                m_keyOpt.SetValue(LocalFile3, "");
                m_keyOpt.SetValue(RemoteFile3, "");
            }
            else
            {
                object localServerURL = m_keyOpt.GetValue(LocalServerURL);
                object localFile1 = m_keyOpt.GetValue(LocalFile1);
                object localFile2 = m_keyOpt.GetValue(LocalFile2);
                object localFile3 = m_keyOpt.GetValue(LocalFile3);

                SetGrid(localServerURL, localFile1, localFile2, localFile3, m_rowLocal, m_optLocal);

                object remoteServerURL = m_keyOpt.GetValue(RemoteServerURL);
                object remoteFile1 = m_keyOpt.GetValue(RemoteFile1);
                object remoteFile2 = m_keyOpt.GetValue(RemoteFile2);
                object remoteFile3 = m_keyOpt.GetValue(RemoteFile3);

                SetGrid(remoteServerURL, remoteFile1, remoteFile2, remoteFile3, m_rowRemote, m_optRemote);
            }

            keyUnE.Close();
            keySoftware.Close();
        }

        private void SetGrid(object serverURL, object file1, object file2, object file3, DataGridViewRow row, ServerOption optServer)
        {
            string strFile1, strFile2, strFile3;
            int nLineIndex1, nLineIndex2, nLineIndex3;

            GetFileOption(file1, out strFile1, out nLineIndex1);
            GetFileOption(file2, out strFile2, out nLineIndex2);
            GetFileOption(file3, out strFile3, out nLineIndex3);

            row.Cells[0].Value = serverURL;
            row.Cells[1].Value = optServer.File1Path = strFile1;

            if (serverURL == null)
                optServer.ServerURLPath = null;
            else
                optServer.ServerURLPath = serverURL.ToString();

            if (nLineIndex1 >= 0)
                row.Cells[2].Value = optServer.File1Index = nLineIndex1;
            else
            {
                row.Cells[2].Value = null;
                optServer.File1Index = -1;
            }

            row.Cells[3].Value = optServer.File2Path = strFile2;

            if (nLineIndex2 >= 0)
                row.Cells[4].Value = optServer.File2Index = nLineIndex2;
            else
            {
                row.Cells[4].Value = null;
                optServer.File2Index = -1;
            }

            row.Cells[5].Value = optServer.File3Path = strFile3;

            if (nLineIndex3 >= 0)
                row.Cells[6].Value = optServer.File3Index = nLineIndex3;
            else
            {
                row.Cells[6].Value = null;
                optServer.File3Index = -1;
            }
        }

        private void GetFileOption(object file, out string strFIle, out int nLineIndex)
        {
            strFIle = null;
            nLineIndex = -1;

            if (file == null)
                return;

            string str = file.ToString();
            int nIndex = str.LastIndexOf('\\');

            if (nIndex < 0)
            {
                strFIle = str;
                return;
            }

            string strLineIndex = str.Substring(nIndex + 1);

            if (!int.TryParse(strLineIndex, out nLineIndex))
            {
                strFIle = str;
                return;
            }

            strFIle = str.Substring(0, nIndex);
        }

        private void SetRegistry()
        {
            if (m_keyOpt == null)
                return;

            SetRegistry(m_rowLocal, LocalServerURL, LocalFile1, LocalFile2, LocalFile3, m_optLocal);
            SetRegistry(m_rowRemote, RemoteServerURL, RemoteFile1, RemoteFile2, RemoteFile3, m_optRemote);
        }

        private void SetRegistry(DataGridViewRow row, string strServerURL, string strFile1, string strFile2, string strFile3, ServerOption optServer)
        {
            if (row.Cells[0].Value == null)
            {
                optServer.ServerURLPath = null;
                m_keyOpt.SetValue(strServerURL, "");
            }
            else
            {
                optServer.ServerURLPath = row.Cells[0].Value.ToString();
                m_keyOpt.SetValue(strServerURL, optServer.ServerURLPath);
            }

            string strFilePath;
            int nLineIndex;

            SetFile(row, 1, strFile1, out strFilePath, out nLineIndex);
            optServer.File1Path = strFilePath;
            optServer.File1Index = nLineIndex;

            SetFile(row, 3, strFile2, out strFilePath, out nLineIndex);
            optServer.File2Path = strFilePath;
            optServer.File2Index = nLineIndex;

            SetFile(row, 5, strFile3, out strFilePath, out nLineIndex);
            optServer.File3Path = strFilePath;
            optServer.File3Index = nLineIndex;
        }

        private void SetFile(DataGridViewRow row, int nColumnIndex, string strFile, out string strFilePath, out int nLineIndex)
        {
            strFilePath = null;
            nLineIndex = -1;

            if (row.Cells[nColumnIndex].Value == null)
                m_keyOpt.SetValue(strFile, "");
            else
            {
                if (row.Cells[nColumnIndex + 1].Value != null && int.TryParse(row.Cells[nColumnIndex + 1].Value.ToString(), out nLineIndex))
                {
                    strFilePath = row.Cells[nColumnIndex].Value.ToString();
                    m_keyOpt.SetValue(strFile, strFilePath + "\\" + nLineIndex.ToString());
                }
                else
                {
                    strFilePath = row.Cells[nColumnIndex].Value.ToString();
                    nLineIndex = -1;

                    m_keyOpt.SetValue(strFile, strFilePath);
                }
            }
        }

        private FormMain.Option ReadOption()
        {
            string szKey = @"Software\UNE\Server Connection Info";
            RegistryKey key = Registry.CurrentUser.OpenSubKey(szKey);

            if (key == null)
                return FormMain.Option.None;

            object value = key.GetValue("webserver_url");

            key.Close();

            if (value == null)
                return FormMain.Option.None;

            string strValue = value.ToString();

            if (m_optLocal != null && strValue == m_optLocal.ServerURLPath)
                return FormMain.Option.Local;
            else if (m_optRemote != null && strValue == m_optRemote.ServerURLPath)
                return FormMain.Option.Remote;

            return FormMain.Option.None;
        }

        private void WriteOption(FormMain.Option opt)
        {
            if (opt == FormMain.Option.Local && m_optLocal != null && m_optLocal.ServerURLPath != null)
                WriteServerURL(m_optLocal.ServerURLPath);
            else if (opt == FormMain.Option.Remote && m_optRemote != null && m_optRemote.ServerURLPath != null)
                WriteServerURL(m_optRemote.ServerURLPath);
        }

        private void WriteServerURL(string strURLPath)
        {
            string szKey = "Software";
            RegistryKey keySoftware = Registry.CurrentUser.OpenSubKey(szKey, true);

            if (keySoftware == null)
                return;

            szKey = "UNE";
            RegistryKey keyUnE = keySoftware.OpenSubKey(szKey, true);

            if (keyUnE == null)
            {
                keyUnE = keySoftware.CreateSubKey(szKey);
            }

            szKey = "Server Connection Info";
            RegistryKey keyConnectionInfo = keyUnE.OpenSubKey(szKey, true);

            if (keyConnectionInfo == null)
            {
                keyConnectionInfo = keyUnE.CreateSubKey(szKey);
            }

            keyConnectionInfo.SetValue("webserver_url", strURLPath);

            keyConnectionInfo.Close();
            keyUnE.Close();
            keySoftware.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SetRegistry();
            m_keyOpt.Close();
            m_keyOpt = null;

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_keyOpt.Close();
            m_keyOpt = null;

            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        public void SetLocal()
        {
            SetOption(m_optLocal);
        }

        public void SetRemote()
        {
            SetOption(m_optRemote);
        }

        private void SetOption(ServerOption opt)
        {
            if (opt == null)
                return;

            if (opt.ServerURLPath != null)
                WriteServerURL(opt.ServerURLPath);
            else
                WriteServerURL("");

            WriteFile(opt.File1Path, opt.File1Index);
            WriteFile(opt.File2Path, opt.File2Index);
            WriteFile(opt.File3Path, opt.File3Index);
        }

        private void WriteFile(string strFilePath, int nLineIndex)
        {
            if (strFilePath == null || strFilePath.Length == 0 || nLineIndex < 0)
                return;

            if (!File.Exists(strFilePath))
                return;

            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);

            List<string> lines = new List<string>();
            StreamReader reader = new StreamReader(strFilePath, encEUC_KR);

            for (int nLineCount=1;!reader.EndOfStream;nLineCount++)
            {
                string strLine = reader.ReadLine();
                
                if (nLineCount == nLineIndex)
                {
                    int nIndex = strLine.IndexOf("webserver_url");

                    if (nIndex > 0)
                        strLine = strLine.Substring(nIndex);
                }
                else if (strLine.StartsWith("webserver_url"))
                {
                    strLine = "#" + strLine;
                }

                lines.Add(strLine);
            }

            reader.Close();

            StreamWriter writer = new StreamWriter(strFilePath, false, encEUC_KR);

            foreach (string strLine in lines)
            {
                writer.WriteLine(strLine);
            }

            writer.Close();
        }
    }
}
