using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace RoadMan
{
	public class CaptureTool
	{
		private SaveFileDialog mFileDialog = new SaveFileDialog();
		private DXFViewer.DXFControl mCtrl = null;

		private PanelDXFViewer mPanel = null;

		public CaptureTool(PanelDXFViewer panel)
		{
			mCtrl = panel.DXFControl;
			mPanel = panel;
			
		}

		private Image CreateScreenImage()
		{
			try
			{
				Bitmap bitmap = new Bitmap(mCtrl.Size.Width, mCtrl.Size.Height, PixelFormat.Format32bppPArgb);
				mCtrl.DrawToBitmap(bitmap, new Rectangle(0, 0, mCtrl.Size.Width, mCtrl.Size.Height));
				return (Image)bitmap;
			}
			catch (Exception)
			{
			}			
			return null;
		}


		private Image mTargetImage = null;
		public Image TargetImage
		{
			get { return mTargetImage; }			
		}

		public void CaptureFullScreen()
		{
			CancelRectWindow();

			m_RectMode = false;
			Image image = CreateScreenImage();
			Clipboard.SetImage(image);
			mTargetImage = image;
		}

		public void SaveImage()
		{
			if (mTargetImage != null)
			{
				SaveFileImage(mTargetImage);
			}

			//mTargetImage.Dispose();
			//mTargetImage = null;
		}

		public void SaveImageFullScreen()
		{
			if (m_RectMode == true)
				return;

			if( mTargetImage != null)
			{
				SaveFileImage(mTargetImage);
			}

			mTargetImage.Dispose();
			mTargetImage = null;
		}
		
		private void SaveFileImage(Image image)
		{
			mFileDialog.Filter =
				"Jpeg files (*.jpg)|*.jpg|Png files (*.png)|*.png|Bmp files (*.bmp)|*.bmp|All files (*.*)|*.*";

			mFileDialog.DefaultExt = "png";
			mFileDialog.RestoreDirectory = true;			
			mFileDialog.FileName = "";

			if (mFileDialog.ShowDialog() == DialogResult.OK)
			{
				string szFileName = mFileDialog.FileName;
				
				string szExt = Path.GetExtension(szFileName);
				szExt = szExt.ToLower();
				if( szExt == ".jpg" || szExt == ".jpeg")
				{
					image.Save(szFileName, ImageFormat.Jpeg);
				}
				else if (szExt == ".bmp")
				{
					image.Save(szFileName, ImageFormat.Bmp);
				}
				else if (szExt == ".png")
				{
					image.Save(szFileName, ImageFormat.Png);
				}
				else
				{
                    UnE.Utility.UMessageBox.Show(mPanel, "지원하지 않는 파일 확장자입니다.", "이미지 저장 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}				
			}
		}

		private Image CropImage(Image img, Rectangle cropArea)
		{
			Bitmap bmpImage = new Bitmap(img);
			return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
		}

		public void CaptureRectWindow()
		{		
			if (mPanel != null)
			{
				mPanel.ScreenRectPainter.OnSelectScreenRect += CaptureRectWindow;
				mPanel.ScreenSelectMode = true;
			}
		}


		private bool m_RectMode = true;
		public void SaveImageRectWindow()
		{
			if( m_RectMode == false)
				return;
			if( mTargetImage != null)
			{
				SaveFileImage(mTargetImage);
			}
			mTargetImage.Dispose();
			mTargetImage = null;
		}	

		public void CancelRectWindow()
		{
			if (mPanel != null)
			{
				mPanel.ScreenRectPainter.OnSelectScreenRect -= CaptureRectWindow;
				mPanel.ScreenRectPainter.Clear();
				mPanel.ScreenSelectMode = false;
				mPanel.DXFControl.Refresh();
			}
		}

		public void CaptureRectWindow(Rectangle rect)
		{
            if (rect.Width == 0 || rect.Height == 0)
                return;

			Image image = CreateScreenImage();


			Rectangle rectCrop = new Rectangle();
			rectCrop.Location = new Point(rect.X + 1, rect.Y + 1);
			rectCrop.Width = rect.Width - 1;
			rectCrop.Height = rect.Height - 1;

			Image wndImage = CropImage(image, rectCrop);


			Clipboard.SetImage(wndImage);
			mTargetImage = wndImage;

			image.Dispose();

			m_RectMode = true;

			FormMain.Instance.EnsureSaveImage();
		}
	}
}
