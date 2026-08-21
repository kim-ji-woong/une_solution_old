using System;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace DivideFile
{
    public partial class FormMain : Form
    {
        private string m_strPrev = "";

        public FormMain()
        {
            InitializeComponent();
            m_strPrev = textBoxCount.Text.Trim();
        }

        private void textBoxCount_TextChanged(object sender, EventArgs e)
        {
            string strText = textBoxCount.Text.Trim();

            if (strText.Length > 0)
            {
                int num;

                if (int.TryParse(strText, out num) == false || num <= 0)
                {
                    textBoxCount.Text = m_strPrev;
                    return;
                }
            }

            m_strPrev = textBoxCount.Text;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "모든 파일 (*.*)|*.*";
            dlg.Title = "파일 열기";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxFilePath.Text = dlg.FileName;
            }
        }

        private void FormMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void FormMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length == 1)
            {
                textBoxFilePath.Text = files[0];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strFilePath = textBoxFilePath.Text.Trim();

            if (strFilePath.Length == 0)
            {
                textBoxFilePath.Focus();
                MessageBox.Show("파일 경로를 입력하세요.");
                return;
            }
            
            if (File.Exists(strFilePath) == false)
            {
                textBoxFilePath.Focus();
                MessageBox.Show("존재하지 않는 파일 경로입니다.");
                return;
            }

            string strText = textBoxCount.Text.Trim();

            if (strText.Length == 0)
            {
                textBoxCount.Focus();
                MessageBox.Show("파일을 몇개로 나눌것인지 결정해 주세요.");
                return;
            }

            int nDivide;

            if (int.TryParse(strText, out nDivide) == false)
            {
                textBoxCount.Focus();
                MessageBox.Show("파일을 몇개로 나눌것인지 결정해 주세요.");
                return;
            }

            Divide(nDivide, strFilePath);
            MessageBox.Show("파일이 분할되었습니다.");
        }

        private void Divide(int nDivide, string strFilePath)
        {
            StreamReader reader = new StreamReader(strFilePath, Encoding.UTF8);
            int nLineCount = 0;

            while (reader.EndOfStream == false)
            {
                reader.ReadLine();
                nLineCount++;
            }

            reader.Close();

            reader = new StreamReader(strFilePath, Encoding.UTF8);
            int nIndex = 1, nCount = 0;

            StreamWriter writer = new StreamWriter(GetFilePath(nIndex, strFilePath), false, Encoding.UTF8);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine();
                nCount++;

                int nIndexCount = nLineCount * nIndex / nDivide;
                
                if (nCount > nIndexCount)
                {
                    if (nIndex < nDivide)
                    {
                        nIndex++;
                        writer.Close();
                        writer = new StreamWriter(GetFilePath(nIndex, strFilePath), false, Encoding.UTF8);
                    }
                }

                writer.WriteLine(strLine);
            }

            reader.Close();
            writer.Close();
        }

        private string GetFilePath(int nIndex, string strFilePath)
        {
            int nDotIndex = strFilePath.LastIndexOf('.');

            if (nDotIndex < 0)
                return string.Format("{0}_{1:00}", strFilePath, nIndex);

            string strFile = strFilePath.Substring(0, nDotIndex);
            string strExt = strFilePath.Substring(nDotIndex);

            return string.Format("{0}_{1:00}{2}", strFile, nIndex, strExt);
        }
    }
}
