using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace WindowsFormsApplication2
{
	public partial class MainForm : Form
	{
				
		private float m_fScale = 1.0f;

		private FormPrintPageSetup formSetup = null;

		private FormPrintFrame frameSetup = null;

		private bool m_bFitToPage = false;

		public MainForm()
		{
			InitializeComponent();
	

			CreateFormPrintPageSetup();
		}

		private void printPreviewDialog1_Load(object sender, EventArgs e)
		{

		}
		private void GetScale()
		{
			double a = mPrintDocument.Length;
			double b = mPrintDocument.UnitValue;


			double t = 0.0393700787;

			if(mPrintDocument.LengthOfUnit == System.Drawing.Printing.UPrintDocument.LengthUnit.mm)
			{
				t = 1.0;
			}

			b = b * t;

						

			if (b == 0)
				return;

			m_fScale = (float)(a / b);
			label1.Text = string.Format("스케일: {0}", m_fScale);
		}

		
		private void CreateFormPrintPageSetup()
		{
			formSetup = new FormPrintPageSetup();
			formSetup.PrinterSettings = new System.Drawing.Printing.PrinterSettings();
			formSetup.PageSettings = mPrintDocument.DefaultPageSettings;
			mPrintDocument.DefaultPageSettings.Margins = new Margins(3, 3, 3, 3);
			formSetup.EnableMetric = false;
			formSetup.Document = mPrintDocument;

			frameSetup = new FormPrintFrame(formSetup);
			frameSetup.Text = "인쇄 설정";
			frameSetup.ShowCloseButton = true;
			frameSetup.ShowMaxButton = false;
			frameSetup.ShowMinButton = false;
			frameSetup.FrameMaximized = false;
			frameSetup.Sizable = false;
			frameSetup.Size = new Size(588, 397);
			frameSetup.StartPosition = FormStartPosition.CenterScreen;
			frameSetup.WindowState = FormWindowState.Normal;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if(pictureBox1.BackgroundImage!= null)
			{
				Size imageSize = new Size(pictureBox1.BackgroundImage.Size.Width, pictureBox1.BackgroundImage.Size.Height);
				mPrintDocument.DrawingSize = imageSize;
			}
			

			if (frameSetup.ShowDialog(this) == DialogResult.OK)
			{
				GetScale();

				
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{			
			FormPrintPreview formPreview = new FormPrintPreview();
			FormPrintFrame frame = new FormPrintFrame(formPreview);
			frame.Text = "인쇄 미리 보기";
			
			frame.WindowState = FormWindowState.Maximized;
			formPreview.PreviewContorl.Document = mPrintDocument;
			frame.WindowState = FormWindowState.Maximized;
			frame.ShowDialog(this);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			printDialog1.PrinterSettings = formSetup.PrinterSettings;
	
			
			printDialog1.AllowCurrentPage = true;			
			printDialog1.AllowSelection = true;			
			printDialog1.AllowSomePages = true;

			printDialog1.Document = mPrintDocument;
			printDialog1.AllowPrintToFile = true;
			printDialog1.ShowNetwork = true;
			printDialog1.UseEXDialog = true;
		
			
			if (printDialog1.ShowDialog() == DialogResult.OK)
			{
				
				mPrintDocument.Print();
			}
		}

		private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			if (pictureBox1.BackgroundImage == null)
				return;

			
			UPrintDocument document = (UPrintDocument)sender;
			if (document == null)
				return;
			
			GetScale();
			Rectangle page = e.MarginBounds;

			e.Graphics.ResetTransform();


			float offsetX = (float)document.OffsetX;
			float offsetY = (float)document.OffsetY;

			bool bUpsideDown = document.UpsideDown;
			if (bUpsideDown == true)
			{
				float fWidth = e.PageBounds.Width * 0.5f;
				float fHeight = e.PageBounds.Height * 0.5f;
				e.Graphics.TranslateTransform(fWidth, fHeight);
				e.Graphics.RotateTransform(180.0f);
				e.Graphics.TranslateTransform(-fWidth, -fHeight);

			}
			//if (document.FitToPage == true)
			//{
			//	e.Graphics.DrawImage(pictureBox1.BackgroundImage, page);
			//}
			//else	


			// 문서의 Margin을 Draw영역에서 제외한다.
			Region region = new Region(e.MarginBounds);
			e.Graphics.Clip = region;
			// 이미지의 사이즈를 구한다.
			Size imageSize = new Size(pictureBox1.BackgroundImage.Size.Width, pictureBox1.BackgroundImage.Size.Height);

			if( document.PrintOnCenter == true)
			{	
				// 화면 스케일을 적용
				e.Graphics.ScaleTransform(m_fScale, m_fScale);

				// Scale에 따른 크기 변화랑을 Position에 적용한다.
				float dx = (imageSize.Width / m_fScale - imageSize.Width) * 0.5f;
				float dy = (imageSize.Height / m_fScale - imageSize.Height) * 0.5f;

				// 이미지가 중심에 오도록 Scale이 적용된 Image의 크기를 고려하여 Position을 구한다.
				float transX = dx + ((page.Width - imageSize.Width) * 0.5f + page.Location.X ) / m_fScale;
				float transY = dy + ((page.Height - imageSize.Height) * 0.5f + page.Location.Y) / m_fScale;

				RectangleF imgRect = new RectangleF(transX, transY, imageSize.Width, imageSize.Height);
				e.Graphics.DrawImage(pictureBox1.BackgroundImage, imgRect);		

			}
			else
			{
				// 화면 스케일을 적용
				e.Graphics.ScaleTransform(m_fScale, m_fScale);

				// Scale에 따른 크기 변화랑을 Position에 적용한다.
				float dx = (offsetX / m_fScale - offsetX) * 0.5f;
				float dy = (offsetY / m_fScale - offsetY) * 0.5f;

				// 이미지가 Offset 위치에 오도록 Scale을 고려하여 Position을 구한다.
				float transX = dx + (offsetX + page.Location.X) / m_fScale;
				float transY = dy + (offsetY + page.Location.Y) / m_fScale;

				RectangleF imgRect = new RectangleF(transX, transY, imageSize.Width, imageSize.Height);
				e.Graphics.DrawImage(pictureBox1.BackgroundImage, imgRect);		

			}

			

		}

		private void button4_Click(object sender, EventArgs e)
		{
			openFileDialog1.DefaultExt = "jpg";
			openFileDialog1.Filter = "Image files (*.jpg)|*.jpg|All files (*.*)|*.*";

			if( openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				string szFile = openFileDialog1.FileName;
				Image image = Bitmap.FromFile(szFile);
				pictureBox1.BackgroundImage = image;
			}
		}
		
	
		private void button5_Click(object sender, EventArgs e)
		{
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void MainForm_Load(object sender, EventArgs e)
		{

		}

		private void button5_Click_1(object sender, EventArgs e)
		{
			if (pictureBox1.BackgroundImageLayout == ImageLayout.Stretch)
				pictureBox1.BackgroundImageLayout = ImageLayout.Center;
			else
				pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
		}
	}
}
