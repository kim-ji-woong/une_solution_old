using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using dnsDBUtil;

namespace WebRTCConfigMaker
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            cboDBType.SelectedIndex = 0;
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    string strPath = openFileDialog.FileName;
                    textBoxConfig.Text = strPath;
                }
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            string strConfigPath = textBoxConfig.Text.Trim();

            if (strConfigPath.Length == 0)
            {
                textBoxConfig.Focus();
                MessageBox.Show("Config 파일 경로를 입력하세요.");
                return;
            }

            string strWebServerURL = textBoxWebServerURL.Text.Trim();

            if (strWebServerURL.Length == 0)
            {
                textBoxWebServerURL.Focus();
                MessageBox.Show("WebServerURL을 입력하세요.");
                return;
            }

            string strDBName = textBoxDBName.Text.Trim();

            if (strDBName.Length == 0)
            {
                textBoxDBName.Focus();
                MessageBox.Show("DBName을 입력하세요.");
                return;
            }

            WebDBManager dbMgr = new WebDBManager(strDBName, cboDBType.SelectedIndex, 1, strWebServerURL);
            List<CCTV> cctvs = ReadCCTVs(dbMgr);

            if (cctvs != null)
            {
                UpdateConfigFile(cctvs, strConfigPath);
            }
        }

        private void UpdateConfigFile(List<CCTV> cctvs, string strFilePath)
        {
            List<string> prevLines = new List<string>();
            StreamReader reader = new StreamReader(strFilePath, Encoding.UTF8);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine();
                prevLines.Add(strLine);

                if (strLine.Trim().StartsWith("\"streams\""))
                    break;
            }

            reader.Close();

            StreamWriter writer = new StreamWriter(strFilePath, false, Encoding.UTF8);

            foreach (string strLine in prevLines)
            {
                writer.WriteLine(strLine);
            }

            int cctvCount = cctvs.Count;

            for (int i=0;i<cctvCount;i++)
            {
                CCTV cctv = cctvs[i];

                writer.WriteLine("    \"" + cctv.ID.ToString() + "\": {");
                writer.WriteLine("      \"on_demand\": true,");
                writer.WriteLine("      \"disable_audio\": true,");
                writer.WriteLine("      \"url\": \"" + cctv.URL + "\"");
                writer.Write("    }");

                if (i < cctvCount - 1)
                    writer.WriteLine(",");
                else
                    writer.WriteLine("");
            }

            writer.WriteLine("  }");
            writer.Write("}");
            writer.Close();

            MessageBox.Show("파일이 변경되었습니다.");
        }

        private List<CCTV> ReadCCTVs(WebDBManager dbMgr)
        {
            string strSQL = "Select ID, URL from SdmsCCTV where URL is not NULL and len(URL) > 0";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
            {
                MessageBox.Show("DBError : " + dbMgr.LastErrorMessage);
                return null;
            }

            List<CCTV> cctvs = new List<CCTV>();
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strURL = WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null || strURL == null)
                    continue;

                CCTV cctv = new CCTV();

                cctv.ID = id.Data;
                cctv.URL = strURL;

                cctvs.Add(cctv);
            }

            return cctvs;
        }
    }

    public class CCTV
    {
        private int m_nID = -1;
        private string m_strURL = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string URL
        {
            get { return m_strURL; }
            set { m_strURL = value; }
        }
    }
}
