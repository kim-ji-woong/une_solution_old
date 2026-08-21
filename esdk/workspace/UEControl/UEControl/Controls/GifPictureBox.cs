using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnE.Controls
{
    public class GifPictureBox : PictureBox
    {
        private bool m_onlyLastImage = false;
        private bool m_useSingleLoop = true;
        private int m_nFrameCount = 0;

        private int m_nDrawingCount = 0;
        private System.Drawing.Imaging.FrameDimension m_dimension = null;

        public bool UseSingleLoop
        {
            get { return m_useSingleLoop; }
            set
            {
                m_useSingleLoop = value;

                if (m_useSingleLoop)
                {
                    this.Enabled = true;
                    m_nDrawingCount = 0;
                }
            }
        }

        public bool OnlyLastImage
        {
            get { return m_onlyLastImage; }
            set { m_onlyLastImage = value; }
        }

        public new System.Drawing.Image Image
        {
            get { return base.Image; }
            set
            {
                base.Image = value;
                m_dimension = null;
                m_nDrawingCount = 0;

                if (this.Image != null)
                {
                    this.Enabled = Image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif);
                    CalcFrame();
                }
            }
        }

        public GifPictureBox()
        {
            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (m_useSingleLoop)
            {
                if (m_dimension != null && m_nFrameCount > 0)
                {
                    if (++m_nDrawingCount == m_nFrameCount)
                    {
                        this.Image.SelectActiveFrame(m_dimension, m_nFrameCount - 1);
                        this.Enabled = false;
                    }
                }
            }

            if (m_onlyLastImage && m_nFrameCount > 1 && m_dimension != null)
            {
                this.Image.SelectActiveFrame(m_dimension, m_nFrameCount - 1);
                this.Enabled = false;
            }

            base.OnPaint(pe);
        }

        protected void CalcFrame()
        {
            if (m_dimension != null)
                return;

            if (this.Image != null)
            {
                System.Guid[] guids = this.Image.FrameDimensionsList;
                m_dimension = new System.Drawing.Imaging.FrameDimension(guids[0]);

                m_nFrameCount = this.Image.GetFrameCount(m_dimension);
            }
        }
    }
}
