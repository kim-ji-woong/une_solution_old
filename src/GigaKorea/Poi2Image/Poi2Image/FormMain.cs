using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace Poi2Image
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

            FormPOI frm = new FormPOI();
            frm.ShowDialog();
        }

        void MakePng(POI poi, string strFilePath, int nSize)
        {
            Size size = poi.Size;

            // UNE 전용 (선 14포인트, 비트맵 여분 +30)
            Bitmap bitmap = new Bitmap(Convert.ToInt32(size.Width * 1f + 30), Convert.ToInt32(size.Height * 1f + 30));
            //Bitmap bitmap = new Bitmap(size.Width * 1, size.Height * 1);

            Graphics g = Graphics.FromImage(bitmap);

            //g.TranslateTransform(size.Width * 0.25f, size.Height * 0.25f);
            //g.TranslateTransform((float)(nSize * 0.25), (float)(nSize * 0.25));

            ImageCodecInfo png = GetEncoderInfo("image/png");

            /*Size size = poi.Size;
            Bitmap bitmap = new Bitmap(Convert.ToInt32(nSize * 1.5), Convert.ToInt32(nSize * 1.5));
            //Bitmap bitmap = new Bitmap(size.Width * 2, size.Height * 2);
            Graphics g = Graphics.FromImage(bitmap);
            g.TranslateTransform(size.Width * 0.25f, size.Height * 0.25f);
            //g.TranslateTransform((float)(nSize * 0.25), (float)(nSize * 0.25));
            ImageCodecInfo png = GetEncoderInfo("image/png");*/

            double dScale = 2.0;
            //double dScale = (double)nSize / (double)size.Width;

            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 75);
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = encoderParam;

            Color colorImage = Color.Black;

            if (rbtnYellow.Checked)
                colorImage = yellowColor.BackColor;
            else
                colorImage = blackColor.BackColor;

            poi.Render(g, dScale, colorImage);
            bitmap.Save(strFilePath, png, encoderParams);
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            dlg.Description = "POI 파일이 있는 폴더를 선택하세요";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxFolderPath.Text = dlg.SelectedPath;
            }

        }

        private void btnCreateImage_Click(object sender, EventArgs e)
        {
            string strPath = textBoxFolderPath.Text.Trim();

            if (strPath.Length == 0)
            {
                textBoxFolderPath.Focus();
                MessageBox.Show("POI 파일이 있는 폴더를 지정하세요.");
                return;
            }

            if (Directory.Exists(strPath) == false)
            {
                textBoxFolderPath.Focus();
                MessageBox.Show("잘못된 경로입니다.");
                return;
            }

            int nFileCount = 0;
            string[] files = Directory.GetFiles(strPath, "*.poi");

            foreach (string strFile in files)
            {
                POI poi = POI.FromFile(strFile);

                if (poi != null)
                {
                    string strPngPath = ToPngFilePath(strFile);
                    MakePng(poi, strPngPath, 32);
                    nFileCount++;
                }
            }

            MessageBox.Show(string.Format("총 {0}개의 파일이 생성되었습니다.", nFileCount));
        }

        private string ToPngFilePath(string strFilePath)
        {
            int nIndex = strFilePath.LastIndexOf('.');

            if (nIndex < 0)
                return strFilePath + ".png";

            return strFilePath.Substring(0, nIndex + 1) + "png";
        }
    }
}
