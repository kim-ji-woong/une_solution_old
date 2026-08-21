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

namespace ImageDivider
{
    public partial class FormMain : Form
    {
        private string m_strPrevPath = null;
        private string m_strIni = "log.dat";

        public FormMain()
        {
            InitializeComponent();
            cboAddPixel.SelectedIndex = 1;
            ReadFile();
        }

        private void ReadFile()
        {
            if (File.Exists(m_strIni))
            {
                StreamReader reader = new StreamReader(m_strIni, Encoding.UTF8);
                m_strPrevPath = reader.ReadLine().Trim();
                reader.Close();
            }
        }

        private void WriteFile()
        {
            string strPath = textBoxFilePath.Text.Trim();

            if (strPath.Length == 0)
            {
                if (Directory.Exists(strPath))
                    Directory.Delete(strPath, true);
            }
            else
            {
                StreamWriter writer = new StreamWriter(m_strIni, false, Encoding.UTF8);
                writer.Write(strPath);
                writer.Close();
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            dlg.Description = "원본 이미지 파일 경로 선택";
            dlg.ShowNewFolderButton = false;

            if (m_strPrevPath != null && m_strPrevPath.Length > 0)
                dlg.SelectedPath = m_strPrevPath;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxFilePath.Text = dlg.SelectedPath;
                m_strPrevPath = dlg.SelectedPath;
            }
        }

        private void btnDivide_Click(object sender, EventArgs e)
        {
            string strPath = textBoxFilePath.Text.Trim();

            if (strPath.Length == 0)
            {
                textBoxFilePath.Focus();
                MessageBox.Show("원본 파일이 존재하는 폴더의 경로를 입력하세요.");
                return;
            }

            if (Directory.Exists(strPath) == false)
            {
                textBoxFilePath.Focus();
                MessageBox.Show("잘못된 폴더의 경로입니다.");
                return;
            }

            string strHorizontal = textBoxHorizontal.Text.Trim();

            if (strHorizontal.Length == 0)
            {
                textBoxHorizontal.Focus();
                MessageBox.Show("파일의 가로 너비를 입력하세요.");
                return;
            }

            int nHorz, nVert;

            if (int.TryParse(strHorizontal, out nHorz) == false || nHorz <= 0)
            {
                textBoxHorizontal.Focus();
                MessageBox.Show("파일의 가로 너비는 0보다 큰 정수로 입력되어야만 합니다.");
                return;
            }

            string strVertical = textBoxVertical.Text.Trim();

            if (strVertical.Length == 0)
            {
                textBoxVertical.Focus();
                MessageBox.Show("파일의 세로 너비를 입력하세요.");
                return;
            }

            if (int.TryParse(strVertical, out nVert) == false || nVert <= 0)
            {
                textBoxVertical.Focus();
                MessageBox.Show("파일의 세로 높이는 0보다 큰 정수로 입력되어야만 합니다.");
                return;
            }

            DivideFile(strPath, nHorz, nVert);
        }

        private void DivideFile(string strPath, int nHorz, int nVert)
        {
            string[] files = Directory.GetFiles(strPath);

            if (files.Count() == 0)
                return;

            int nHSourceCount, nVSourceCount;
            Dictionary<int, int> dicHorzSize = new Dictionary<int, int>();
            Dictionary<int, int> dicVertSize = new Dictionary<int, int>();

            if (GetFileCount(files, dicHorzSize, dicVertSize, out nHSourceCount, out nVSourceCount) == false)
                return;

            int nTotalWidth = dicHorzSize[nHSourceCount - 1];
            int nTotalHeight = dicVertSize[nVSourceCount - 1];

            // 정확하게 nHorz와 nVert 단위로 분할하게 되면 나중에 이미지를 합쳐서 그릴때 이미지 사이에
            // 줄이 나타날수 있다.
            // 이 현상을 막기 위하여 addPixel만큼을 더해서 이미지를 만든다.
            int addPixel = cboAddPixel.SelectedIndex;
            int nTotalWidth2 = nTotalWidth - nHorz;
            int nTotalHeight2 = nTotalHeight - nVert;

            int nHTargetCount = nTotalWidth2 / (nHorz - addPixel) + 1;
            int nVTargetCount = nTotalHeight2 / (nVert - addPixel) + 1;

            if (nTotalWidth2 % nHorz > 0)
                nHTargetCount++;

            if (nTotalHeight2 % nVert > 0)
                nVTargetCount++;
            /*int nHTargetCount = nTotalWidth / nHorz;
            int nVTargetCount = nTotalHeight / nVert;

            if (nTotalWidth % nHorz > 0)
                nHTargetCount++;

            if (nTotalHeight % nVert > 0)
                nVTargetCount++;*/

            string strFolderName = strPath + "_Divide";

            if (Directory.Exists(strFolderName))
                Directory.Delete(strFolderName, true);

            Directory.CreateDirectory(strFolderName);

            try
            {
                for (int i=0;i<nHTargetCount;i++)
                {
                    for (int j=0;j<nVTargetCount;j++)
                    {
                        Image img = DrawImage(files, nHSourceCount, nVSourceCount, nHTargetCount, nVTargetCount, i, j, nHorz, nVert, dicHorzSize, dicVertSize, addPixel);

                        string strFileName = string.Format("{0}\\{1:000}_{2:000}.png", strFolderName, j + 1, i + 1);
                        img.Save(strFileName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }

                MessageBox.Show("파일이 생성되었습니다.");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private Image DrawImage(string[] files, int nHSourceCount, int nVSourceCount, int nHTargetCount, int nVTargetCount, int nHTargetIndex, int nVTargetIndex, int width, int height, Dictionary<int, int> dicHorzSize, Dictionary<int, int> dicVertSize, int addPixel)
        {
            int xPos = width * nHTargetIndex;
            int yPos = height * nVTargetIndex;

            if (nHTargetIndex > 0)
                xPos = width + (width - addPixel) * (nHTargetIndex - 1) - addPixel;

            if (nVTargetIndex > 0)
                yPos = height + (height - addPixel) * (nVTargetIndex - 1) - addPixel;

            int nHSourceIndex = nHSourceCount - 1;
            int nVSourceIndex = nVSourceCount - 1;
            int nSourceBeginX = 0, nSourceBeginY = 0;
            int nPrev = 0;

            for (int i=0;i<nHSourceCount-1;i++)
            {
                int nPos = dicHorzSize[i];

                if (nPos > xPos)
                {
                    nHSourceIndex = i;
                    break;
                }

                nPrev = nPos;
            }

            nSourceBeginX = xPos - nPrev;
            nPrev = 0;

            for (int i=0;i<nVSourceCount-1;i++)
            {
                int nPos = dicVertSize[i];

                if (nPos > yPos)
                {
                    nVSourceIndex = i;
                    break;
                }

                nPrev = nPos;
            }

            nSourceBeginY = yPos - nPrev;

            int nFileIndex = nVSourceIndex * nHSourceCount + nHSourceIndex;
            string strFilePath = files[nFileIndex];

            Image imgOrigin = Image.FromFile(strFilePath);
            int nOriginWidth = imgOrigin.Width;
            int nOriginHeight = imgOrigin.Height;

            int nImageWidth = width;
            int nImageHeight = height;
            int w = nImageWidth, h = nImageHeight;

            if (nSourceBeginX + nImageWidth > nOriginWidth)
            {
                if (nHSourceIndex == nHSourceCount - 1)
                {
                    nImageWidth = nOriginWidth - nSourceBeginX;
                    w = nImageWidth;
                }
                else
                {
                    int nNextWidth = dicHorzSize[nHSourceIndex + 1] - dicHorzSize[nHSourceIndex];
                    w = nOriginWidth - nSourceBeginX;

                    if (width - w > nNextWidth)
                        nImageWidth = w + nNextWidth;
                }
            }

            if (nSourceBeginY + nImageHeight > nOriginHeight)
            {
                if (nVSourceIndex == nVSourceCount - 1)
                {
                    nImageHeight = nOriginHeight - nSourceBeginY;
                    h = nImageHeight;
                }
                else
                {
                    int nNextHeight = dicVertSize[nVSourceIndex + 1] - dicVertSize[nVSourceIndex];
                    h = nOriginHeight - nSourceBeginY;

                    if (height - h > nNextHeight)
                        nImageHeight = h + nNextHeight;
                }
            }

            Image img = new Bitmap(nImageWidth, nImageHeight);
            Graphics g = Graphics.FromImage(img);
            g.DrawImage(imgOrigin, new Rectangle(0, 0, w, h), new Rectangle(nSourceBeginX, nSourceBeginY, w, h), GraphicsUnit.Pixel);
            imgOrigin.Dispose();

            if (nImageWidth > w)
                DrawNextImage(g, w, 0, nImageWidth - w, h, 0, nSourceBeginY, nHSourceCount, nHSourceIndex + 1, nVSourceIndex, files);

            if (nImageHeight > h)
                DrawNextImage(g, 0, h, w, nImageHeight - h, nSourceBeginX, 0, nHSourceCount, nHSourceIndex, nVSourceIndex + 1, files);

            if (nImageWidth > w && nImageHeight > h)
                DrawNextImage(g, w, h, nImageWidth - w, nImageHeight - h, 0, 0, nHSourceCount, nHSourceIndex + 1, nVSourceIndex + 1, files);

            g.Dispose();
            return img;
        }

        private void DrawNextImage(Graphics g, int x, int y, int w, int h, int nSourceBeginX, int nSourceBeginY, int nHSourceCount, int nHSourceIndex, int nVSourceIndex, string[] files)
        {
            int nFileIndex = nVSourceIndex * nHSourceCount + nHSourceIndex;
            string strFilePath = files[nFileIndex];

            Image imgOrigin = Image.FromFile(strFilePath);
            g.DrawImage(imgOrigin, new Rectangle(x, y, w, h), new Rectangle(nSourceBeginX, nSourceBeginY, w, h), GraphicsUnit.Pixel);
            imgOrigin.Dispose();
        }

        private bool GetFileCount(string[] files, Dictionary<int, int> dicHorzSize, Dictionary<int, int> dicVertSize, out int nHCount, out int nVCount)
        {
            nHCount = nVCount = 0;
            int nFileCount = files.Count();

            if (nFileCount == 0)
                return false;

            int nHSize = 0, nVSize = 0;
            int nFileIndex = 0;

            for (int i=0;i<nFileCount;i++)
            {
                string strFilePath = files[i];
                Image image = Image.FromFile(strFilePath);

                nHSize += image.Width;
                dicHorzSize[i] = nHSize;
                image.Dispose();

                nFileIndex = i;

                if (GetNextFile(files, ref nFileIndex) == null)
                {
                    nHCount = i + 1;
                    break;
                }
            }

            if (nHCount == 0)
                return false;

            for (int i=0;i<nFileCount;i+=nHCount)
            {
                string strFilePath = files[i];
                Image image = Image.FromFile(strFilePath);

                nVSize += image.Height;
                dicVertSize[i / nHCount] = nVSize;
                nVCount++;
                image.Dispose();
            }

            return true;
        }

        // 같은 행에 해당하는 다음 이미지 파일을 얻어온다.
        // nFileIndex가 이번행의 마지막 이미지 파일을 가르키고 있다면 null을 리턴한다.
        private string GetNextFile(string[] files, ref int nFileIndex)
        {
            if (files.Count() == nFileIndex + 1)
                return null;

            string strCurrent = files[nFileIndex];
            string strNext = files[nFileIndex + 1];

            int nIndexCurrent_ = strCurrent.LastIndexOf('_');
            int nIndexNext_ = strNext.LastIndexOf('_');

            string strCurrentPrev = strCurrent.Substring(0, nIndexCurrent_);
            string strNextPrev = strNext.Substring(0, nIndexNext_);

            // nFileIndex가 이번행의 마지막 이미지 파일이다.
            if (strCurrentPrev != strNextPrev)
                return null;

            nFileIndex++;
            return strNext;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            WriteFile();
        }
    }
}
