using System;
using System.Drawing;
using System.Windows.Forms;

namespace SOPMonitoringSystem
{

    public delegate void ThumbnailDraw(object sender, PaintEventArgs e);
 

    public partial class FormSectionPreview : PictureBox
    {
        private Bitmap mInternalImage = null;
        private Color mBackground;
        private Brush mBackBrush = new SolidBrush(Color.White);

        private Control mTitleBar = new Label();
        public System.Windows.Forms.Control TitleBar
        {
            get { return mTitleBar; }            
        }
        public System.Drawing.Color TitleBarColor
        {
            get { return mTitleBar.BackColor; }
            set { mTitleBar.BackColor = value; }
        }
        public string TitleBarCaption
        {
            get { return mTitleBar.Text; }
            set { mTitleBar.Text = value; }
        }

        public System.Drawing.Color ThumbnailBackColor
        {
            get { return mBackground; }
            set { 
                mBackground = value;
                //mThumbnailView.BackColor = mBackground;
                mBackBrush = new SolidBrush(mBackground);
            }
        }
        private Control targetContorl = null;
        public System.Windows.Forms.Control TargetContorl
        {
            get { return targetContorl; }
            set {
                targetContorl = value;
                if (TargetContorl != null)
                {
                    TargetContorl.Resize += new EventHandler(ControlResized);
                    Draw += new ThumbnailDraw(PreviewDraw);
                }
            }
        }
        private Size originSize = new Size(100, 100);
        public System.Drawing.Size OriginSize
        {
            get { return originSize; }
            set
            {
                originSize = value;
                SetSize(originSize);
            }
        }
        public event ThumbnailDraw Draw;

        public FormSectionPreview()
        {            
            mInternalImage = new Bitmap(originSize.Width, originSize.Width);
            //SetBounds(0, 0, 100, 100);
            this.Paint += new PaintEventHandler(SectionPreview_Paint);
            
        }

        public void PreviewDraw(object sender, PaintEventArgs e)
        {
            if (TargetContorl != null)
            {
                Bitmap bitmap = new Bitmap(TargetContorl.Size.Width, TargetContorl.Size.Height);
                TargetContorl.DrawToBitmap(bitmap, new Rectangle(0, 0, TargetContorl.Size.Width, TargetContorl.Size.Height));
                e.Graphics.DrawImage((Image)bitmap, new Rectangle(-5, -23, TargetContorl.Size.Width + 10, TargetContorl.Size.Height + 27), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel);
            }
        }

        protected void SectionPreview_Paint(object sender, PaintEventArgs e)
        {
            if (mInternalImage == null)
                return;
            
            Graphics g = Graphics.FromImage(mInternalImage);
            g.FillRectangle(mBackBrush, 0, 0, originSize.Width + 1, originSize.Height + 1);
            PaintEventArgs ex = new PaintEventArgs(g, e.ClipRectangle);
            if (Draw != null)
            {
                Draw(sender, ex);

                e.Graphics.DrawImage((Image)mInternalImage, 0, 0, Size.Width, Size.Height);
            }
        }
        private void SetSize(Size size)
        {
            originSize = size;
            mInternalImage = new Bitmap(size.Width, size.Height);
        }

        private void ControlResized(object sender, EventArgs e)
        {
            if (TargetContorl != null)
            {
                originSize = TargetContorl.Size;
                if (originSize.Width == 0 || originSize.Height == 0)
                    return;
                mInternalImage = new Bitmap(originSize.Width, originSize.Height);
            }            
        }       
    }
}
