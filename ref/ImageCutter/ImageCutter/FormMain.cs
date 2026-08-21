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

namespace ImageCutter
{
    public partial class FormMain : Form
    {
        private int m_nImageWidth = -1;
        private int m_nImageHeight = -1;
        private int m_nTileWidth = -1;
        private int m_nTileHeight = -1;

        public FormMain()
        {
            InitializeComponent();
        }

        private void btnImagePath_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "이미지 파일 (*.bmp, *.jpg, *.png, *.gif, *.tif, *.tga)|*.bmp; *.jpg; *.png; *.gif; *.tif; *.tga" ;
            openFileDialog1.RestoreDirectory = true ;

            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxFilePath.Text = openFileDialog1.FileName;
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            string strFilePath = textBoxFilePath.Text.Trim();

            if (strFilePath.Length == 0)
            {
                textBoxFilePath.Focus();
                MessageBox.Show("이미지 경로를 입력하세요.");
                return;
            }

            if (File.Exists(strFilePath) == false)
            {
                textBoxFilePath.Focus();
                MessageBox.Show("이미지 경로가 잘못되었습니다.");
                return;
            }

            if (m_nTileWidth <= 0)
            {
                textBoxHorz.Focus();
                MessageBox.Show("타일 이미지의 가로 크기를 입력하세요.");
                return;
            }

            if (m_nTileHeight <= 0)
            {
                textBoxVert.Focus();
                MessageBox.Show("타일 이미지의 세로 크기를 입력하세요.");
                return;
            }

            string strFolderPath = "";

            if (CutImage(out strFolderPath))
            {
                MessageBox.Show(strFolderPath + "에 타일 이미지 생성이 완료되었습니다.");
            }
            else
                MessageBox.Show("타일 이미지 생성이 실패하였습니다.");
        }

        private void textBoxFilePath_TextChanged(object sender, EventArgs e)
        {
            string strPath = textBoxFilePath.Text.Trim();

            if (strPath.Length == 0 || File.Exists(strPath) == false)
                m_nImageHeight = m_nImageWidth = -1;
            else
            {
                try
                {
                    Image img = Image.FromFile(strPath);

                    m_nImageWidth = img.Size.Width;
                    m_nImageHeight = img.Size.Height;
                }
                catch (Exception)
                {
                    m_nImageHeight = m_nImageWidth = -1;
                }
            }

            if (m_nImageWidth <= 0 || m_nImageHeight <= 0)
                labelImageSize.Visible = false;
            else
            {
                labelImageSize.Text = string.Format("{0} X {1}", m_nImageWidth, m_nImageHeight);
                labelImageSize.Visible = true;
            }
        }

        private void textBoxHorz_TextChanged(object sender, EventArgs e)
        {
            int nWidth;

            if (int.TryParse(textBoxHorz.Text.Trim(), out nWidth))
                m_nTileWidth = nWidth;
            else
                m_nTileWidth = -1;

            SetTileInfo();
        }

        private void textBoxVert_TextChanged(object sender, EventArgs e)
        {
            int nHeight;

            if (int.TryParse(textBoxVert.Text.Trim(), out nHeight))
                m_nTileHeight = nHeight;
            else
                m_nTileHeight = -1;

            SetTileInfo();
        }

        private void SetTileInfo()
        {
            if (m_nImageWidth <= 0 || m_nImageHeight <= 0 || m_nTileWidth <= 0 || m_nTileHeight <= 0)
                groupBoxTileInfo.Visible = false;
            else
            {
                int horz = m_nImageWidth / m_nTileWidth;
                int vert = m_nImageHeight / m_nTileHeight;

                if (m_nImageWidth % m_nTileWidth > 0)
                    horz++;

                if (m_nImageHeight % m_nTileHeight > 0)
                    vert++;

                labelTileInfo.Text = string.Format("{0} X {1}", horz, vert);
                groupBoxTileInfo.Visible = true;
            }
        }

        private bool CutImage(out string strFolderPath)
        {
            string strPath = textBoxFilePath.Text.Trim();
            int nIndex = strPath.LastIndexOf('\\');

            strFolderPath = ".\\";

            if (nIndex >= 0)
                strFolderPath = strPath.Substring(0, nIndex + 1);
            
            strFolderPath += "Tile";

            if (Directory.Exists(strFolderPath) == false)
                Directory.CreateDirectory(strFolderPath);

            int nIndex2 = strPath.LastIndexOf('.');

            if (nIndex2 < 0)
                return false;

            string strImageName = nIndex < 0 ? strPath.Substring(0, nIndex2) : strPath.Substring(nIndex + 1, nIndex2 - nIndex - 1);
            string strExt = strPath.Substring(nIndex2 + 1);

            //panelScreen.Location = new Point(this.Size.Width + 100, panelScreen.Location.Y);

            int horz = m_nImageWidth / m_nTileWidth;
            int vert = m_nImageHeight / m_nTileHeight;

            if (m_nImageWidth % m_nTileWidth > 0)
                horz++;

            if (m_nImageHeight % m_nTileHeight > 0)
                vert++;

            Image img = Image.FromFile(strPath);

            for (int i=0;i<horz;i++)
            {
                int left = m_nTileWidth * i;
                int right = m_nTileWidth * (i + 1);

                if (right > m_nImageWidth)
                    right = m_nImageWidth;

                for (int j=0;j<vert;j++)
                {
                    int top = m_nTileHeight * j;
                    int bottom = m_nTileHeight * (j + 1);

                    if (bottom > m_nImageHeight)
                        bottom = m_nImageHeight;

                    int width = right - left;
                    int height = bottom - top;

                    try
                    {
                        Bitmap bmp = new Bitmap(width, height);
                        Graphics target = Graphics.FromImage(bmp);
                        target.DrawImage(img, -left, -top);

                        string strFileName = strImageName + "_" + i.ToString() + "_" + j.ToString() + "." + strExt;
                        bmp.Save(strFolderPath + "\\" + strFileName);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.Message);
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
