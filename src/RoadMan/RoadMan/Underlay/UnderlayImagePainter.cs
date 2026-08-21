using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;

namespace UnE.Underlay
{
	public delegate void InvalidateControl();
	public class UnderlayImagePainter
	{
		public event InvalidateControl InvalidateControl;

		private DXFViewer.DXFControl m_ctrlViewer = null;
		public UnderlayImagePainter(DXFViewer.DXFControl control)
		{
			m_ctrlViewer = control;			
		}

		private PointF m_OffsetPt;
		public PointF Offset
		{
			get { return m_OffsetPt; }
		}

		private SizeF m_SizeImage;
		public SizeF Size
		{
			get { return m_SizeImage; }
		}

		private Image m_UnderImage = null;

		/*private bool m_bUseUnderImage = true;
		public bool UseUnderImage
		{
			get { return m_bUseUnderImage; }
			set { m_bUseUnderImage = value; }
		}*/

        // Image 경로가 상대경로인가?
        private bool m_relativePath = false;
        private string m_strRelativePath = "";

        // Image 경로가 상대경로인가?
        public bool IsRelativePath
        {
            get { return m_relativePath; }
            set { m_relativePath = value; }
        }

        public string RelativePath
        {
            get { return m_strRelativePath; }
            set { m_strRelativePath = value; }
        }

		private string m_szImagePath = "";
		public string ImagePath
		{
			get { return m_szImagePath; }
		}

		public void SetImage(string szImagePath)
		{
			m_szImagePath = szImagePath;
			if (m_UnderImage != null)
				m_UnderImage.Dispose();
			m_UnderImage = null;
						
			if (!File.Exists(szImagePath))
				return;						

			m_UnderImage = Image.FromFile(szImagePath);
			
			try
			{
				m_UnderImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
			}
			catch(Exception)
			{

			}
			Invalidate();
		}

		public void SetSize(float width , float height)
		{
			m_SizeImage.Width = width;
			m_SizeImage.Height = height;
			Invalidate();
		}
		
		public void SetOffset(float x, float y)
		{
			m_OffsetPt.X = x;
			m_OffsetPt.Y = y;

			Invalidate();
		}

		public void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			//Matrix m = g.Transform.Clone();

			//g.ScaleTransform(1.0f, -1.0f);
			
			if (m_UnderImage != null)
			{
                if (RoadMan.Options.Instance.VisibleBackgroundImage)
				    g.DrawImage(m_UnderImage, m_OffsetPt.X, m_OffsetPt.Y, m_SizeImage.Width, m_SizeImage.Height);
			}

			//g.Transform = m;
		}

		public void Invalidate()
		{
			if (InvalidateControl != null)
			{
				InvalidateControl();
			}
		}

	}
}
