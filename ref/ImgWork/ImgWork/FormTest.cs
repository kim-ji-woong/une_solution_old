using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgWork
{
    public partial class FormTest : Form
    {
        public FormTest()
        {
            InitializeComponent();
        }

        private void FormTest_Load(object sender, EventArgs e)
        {
            string szPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            mImageView.SetImage(Bitmap.FromFile(szPath + "\\1r403-0-342-ea152-401-f-001.png"));
            mImageView.FitView();
        }

        private void ImageView_SizeChanged(object sender, EventArgs e)
        {
            mImageView.Invalidate();
        }

        private void ImageView_Resize(object sender, EventArgs e)
        {
            mImageView.OnPanelResize();
        }

        private void ImageView_Paint(object sender, PaintEventArgs e)
        {
            mImageView.OnPanelPaint(e);
        }

        private void ImageView_MouseDown(object sender, MouseEventArgs e)
        {
            mImageView.OnMouseDown(sender, e);
            mImageView.Invalidate();
        }

        private void ImageView_MouseUp(object sender, MouseEventArgs e)
        {
            mImageView.OnMouseUp(sender, e);
            mImageView.Invalidate();
        }

        private void ImageView_MouseMove(object sender, MouseEventArgs e)
        {
            mImageView.OnMouseMove(sender, e);
            mImageView.Refresh();
        }

        public void ImageView_FitView(object sender, EventArgs e)
        {
            mImageView.ResetTransform();
            mImageView.FitView();
            mImageView.Refresh();
        }
    }
}
