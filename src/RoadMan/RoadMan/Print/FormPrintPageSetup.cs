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
using System.Drawing.Imaging;
using RoadMan;

namespace UnE.Utility.Print
{
	public partial class FormPrintPageSetup : Form
	{
		private int m_prevPrintAreaIndex = 0;
		private int m_prevPaperSizeIndex = 0;
		private bool m_prevPageOrient = false;
		private bool m_prevPageUpsideDown = false;

		private bool m_prevCenterPage = true;
		private double m_prevOffsetX = 10.0f;
		private double m_prevOffsetY = 10.0f;


		private double m_prevLength = 1.0;
		private double m_prevUnitLength = 1.0;

		private int m_prevUnitSelectedIndex = 0;
		private int m_prevScaleSelectedIndex = 0;

		private bool m_prevFitPage = true;
		
		public System.Drawing.Printing.PrinterSettings PrinterSettings
		{
			get { return pageSetupDialog1.PrinterSettings; }
			set 
			{
				pageSetupDialog1.PrinterSettings = value;
				SetPaperList();
			}
		}

		public System.Drawing.Printing.PageSettings PageSettings
		{
			get { return pageSetupDialog1.PageSettings; }
			set 
			{
				pageSetupDialog1.PageSettings = value;
				SetPageSetting(value);
			}
		}

		public bool EnableMetric
		{
			get { return pageSetupDialog1.EnableMetric; }
			set { pageSetupDialog1.EnableMetric = value; }
		}

		public DXFViewer.UPrintDocument Document
		{
			get { return (DXFViewer.UPrintDocument)pageSetupDialog1.Document; }
			set { pageSetupDialog1.Document = value; }
		}

		private bool m_bPageCenter = true;
		public bool PrintPageCenter
		{
			get { return m_bPageCenter; }
			set { m_bPageCenter = value; }
		}

		private bool m_bUpsideDown = false;
		public bool UpsideDown
		{
			get { return m_bUpsideDown; }
			set { m_bUpsideDown = value; }
		}


		private double m_dOffsetX = 10.0;
		public double OffsetX
		{
			get { return m_dOffsetX; }
			set { m_dOffsetX = value; }
		}

		private double m_dOffsetY = 10.0;
		public double OffsetY
		{
			get { return m_dOffsetY; }
			set { m_dOffsetY = value; }
		}

		private double m_dLength = 1.0;
		public double Length
		{
			get { return m_dLength; }
			set { m_dLength = value; }
		}

		private double m_dUnitLength = 1.0;
		public double UnitLength
		{
			get { return m_dUnitLength; }
			set { m_dUnitLength = value; }
		}

		public new DialogResult DialogResult
		{
			get { return base.DialogResult; }
			set
			{
 
				if( value == DialogResult.Cancel)
				{
					RestoreData();
				}
				base.DialogResult = value; 
			}
		}


		public FormPrintPageSetup()
		{
			InitializeComponent();

			cmbPrintArea.SelectedIndex = 0;
		}


		private void btnCancel_Click(object sender, EventArgs e)
		{
			CancelWndSelect();
			// 변경되기 이전값으로 Document의 Value를 저장한다.
			SaveDocumentValue();

			DialogResult = DialogResult.Cancel;

			if( ParentForm != null)
			{
				ParentForm.DialogResult = DialogResult;
				ParentForm.Visible = false;
			}
			else
			{
				Close();
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			CancelWndSelect();
			// 변경된 값을 먼저 저장후 Document의 Value를 저장한다.
			SaveData();


			if( Document != null)
			{
				Document.Print();
			}
			
			DialogResult = DialogResult.OK;
			if (ParentForm != null)
			{
				ParentForm.DialogResult = DialogResult;
				ParentForm.Visible = false;
			}
			else
			{
				Close();
			}
		}

		private void btnPreview_Click(object sender, EventArgs e)
		{
			CancelWndSelect();

			CalculateRatio();
		
			Document.OffsetX = m_dOffsetX;
			Document.OffsetY = m_dOffsetY;

			FormPrintPreview formPreview = new FormPrintPreview();
			formPreview.Text = "인쇄 미리 보기";
		
			FormPrintFrame frame = new FormPrintFrame(formPreview);
			frame.Text = "인쇄 미리 보기";
			frame.WindowState = FormWindowState.Maximized;
			formPreview.PreviewContorl.Document = pageSetupDialog1.Document;
			frame.WindowState = FormWindowState.Maximized;			
			frame.ShowDialog(this);
		}

		private void SetPaperList()
		{
			cmbPageList.Items.Clear();

			int nSelectedIdx = -1;
			foreach (PaperSize size in PrinterSettings.PaperSizes)
			{

				if (size.PaperName == "Specify size")
					continue;

				int nIdx = cmbPageList.Items.Add(size.PaperName);
				
				if(size.PaperName.IndexOf("A4") != -1)
				{
					if (nSelectedIdx == -1)
						nSelectedIdx = nIdx;
				}
			}

			if (nSelectedIdx > -1)
				cmbPageList.SelectedIndex = nSelectedIdx;

			m_prevPaperSizeIndex = nSelectedIdx;
		}
		
		private void SetPageSetting(System.Drawing.Printing.PageSettings setting)
		{
			if( setting.Landscape == true)
			{
				radioHorzPrint.Checked = true;
				radioVertPrint.Checked = false;
			}
		}


		private void FormPrintPageSetup_Load(object sender, EventArgs e)
		{
			if( Document.WindowPrintMode == true)
			{
				cmbPrintArea.SelectedIndex = 1;
			}
			else
			{
				cmbPrintArea.SelectedIndex = 0;
			}
		
			m_prevPrintAreaIndex = 0;
						
			ckbFitPage.Checked = true;
			ckbPageCenter.Checked = true;
			CalculateRatio();

			ckbFitPage.Checked = false;
			ckbFitPage.Checked = true;

            radioVertPrint.Select();
		}

		private void FormPrintPageSetup_FormClosing(object sender, FormClosingEventArgs e)
		{
		}
				
		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if(ckbPageCenter.Checked == true)
			{
				editOffsetX.Enabled = false;
				editOffsetY.Enabled = false;
				m_bPageCenter = true;
				Document.PrintOnCenter = true;
			}
			else
			{
				editOffsetX.Enabled = true;
				editOffsetY.Enabled = true;

				m_bPageCenter = false;
				Document.PrintOnCenter = false;
			}
		}

		private void CalculateRatio()
		{
			int width = Document.DefaultPageSettings.PaperSize.Width;
			int height = Document.DefaultPageSettings.PaperSize.Height;

			float wratio = 1.0f;
			float hratio = 1.0f;

			Size size = (Size)Document.DrawingSize;
			if( m_bPageCenter == true)
			{
				
				wratio = (float)((double)size.Width) / (float)width;
				hratio = (float)((double)size.Height) / (float)height;
			}
			else
			{
				wratio = (float)((double)size.Width + m_dOffsetX) / (float)width;
				hratio = (float)((double)size.Height + m_dOffsetY) / (float)height;
			}
			Document.DrawingSize = size;

			float ratio = (wratio > hratio ? wratio : hratio);

			editLength.Text = "1";

			if (m_prevUnitSelectedIndex == 1)
			{
				ratio = ratio * 25.4f;
			}

			if (ratio == 0)
				ratio = 1;

			Document.UnitValue = ratio;
			Document.Length = 1;
			m_dUnitLength = ratio;
			m_dLength = 1;

			editUnit.Text = ratio.ToString();
		}
				
		private void ckbFitPage_CheckedChanged(object sender, EventArgs e)
		{
			if( ckbFitPage.Checked == true)
			{
				cmbScale.Enabled = false;
				cmbUnit.Enabled = false;

				editLength.Enabled = false;
				editUnit.Enabled = false;

				Document.FitToPage = true;

				CalculateRatio();
			}
			else
			{
				cmbScale.Enabled = true;
				cmbUnit.Enabled = true;

				editLength.Enabled = true;
				editUnit.Enabled = true;

				Document.FitToPage = false;

				
			}
		}

		private void radioHorzPrint_CheckedChanged(object sender, EventArgs e)
		{
			if (Document == null)
				return;

			if( radioHorzPrint.Checked == true)
			{
				Document.DefaultPageSettings.Landscape = true;

                if (ckbUpsideDown.Checked)
                    pictureBoxPrintDirection.BackgroundImage = global::RoadMan.Properties.Resources._64Horizontal_reverse_normal;
                else
                    pictureBoxPrintDirection.BackgroundImage = global::RoadMan.Properties.Resources._64Horizontal_normal;
			}
		}

		private void radioVertPrint_CheckedChanged(object sender, EventArgs e)
		{
			if (Document == null)
				return;

			if( radioVertPrint.Checked == true)
			{
				Document.DefaultPageSettings.Landscape = false;

                if (ckbUpsideDown.Checked)
                    pictureBoxPrintDirection.BackgroundImage = global::RoadMan.Properties.Resources._64Vertical_reverse_normal;
                else
                    pictureBoxPrintDirection.BackgroundImage = global::RoadMan.Properties.Resources._64Vertical_normal;
			}
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
			if( ckbUpsideDown.Checked == true)
			{
				m_bUpsideDown = true;
				Document.UpsideDown = true;

                if (radioVertPrint.Checked)
                    pictureBoxPrintDirection.BackgroundImage = global::RoadMan.Properties.Resources._64Vertical_reverse_normal;
                else
                    pictureBoxPrintDirection.BackgroundImage = global::RoadMan.Properties.Resources._64Horizontal_reverse_normal;
			}
			else
			{
				m_bUpsideDown = false;
				Document.UpsideDown = false;

                if (radioVertPrint.Checked)
                    pictureBoxPrintDirection.BackgroundImage = global::RoadMan.Properties.Resources._64Vertical_normal;
                else
                    pictureBoxPrintDirection.BackgroundImage = global::RoadMan.Properties.Resources._64Horizontal_normal;
			}
		}

		
		private void cmbPageList_SelectedIndexChanged(object sender, EventArgs e)
		{
			int nSelectedIndex = cmbPageList.SelectedIndex;
			if (nSelectedIndex == -1 || Document == null)
				return;

			string szName = cmbPageList.SelectedItem.ToString();
			foreach (PaperSize size in PrinterSettings.PaperSizes)
			{
				if (size.PaperName == szName)
				{
					Document.DefaultPageSettings.PaperSize = size;
					break;
				}
			}

			if( Document.FitToPage == true)
				CalculateRatio();
		}
		
		private void cmbPrintArea_SelectedIndexChanged(object sender, EventArgs e)
		{
			

			int nIdx = cmbPrintArea.SelectedIndex;

			if( nIdx == 0)
			{
				btnSelectWnd.Visible = false;
				mPreviewPane.Visible = false;
				if (Document != null)
					Document.WindowPrintMode = false;
			}
			else if( nIdx == 1)
			{
				//RoadMan.PanelDXFViewer panel = RoadMan.FormMain.Instance.CurrentPanel;
				//if (panel != null)
				//	backImage = CreateScreenImage(panel);
				//mPreviewPane.BackgroundImage = backImage;
				btnSelectWnd.Visible = true;
				mPreviewPane.Visible = true;
				
				
			}
		}


		public void ChangeInch(double mmValue, out double inchValue)
		{
			inchValue = 0;
			
			inchValue = mmValue * 0.0393700787;

			inchValue = Math.Round(inchValue, 7);

			
		}

		public void ChangeMiliMeter(double inchValue, out double mmValue)
		{
			mmValue = 0;
			mmValue = inchValue * 25.4;
			mmValue = Math.Round(mmValue, 5);

		}


		public double GetMiliMeterValue(double value)
		{
			//if (Document.LengthOfUnit == UPrintDocument.LengthUnit.mm)
				return value;
			//double result = 0.0;
			//ChangeMiliMeter(value, out result);
			//return result;
		}


		private int m_LastUnitIndex = 0;
		private void ChangeUnit(int nIdx)
		{
			if( m_LastUnitIndex == nIdx)
				return;
			
			m_LastUnitIndex = nIdx;

			// mm -> inch 계산
			if(nIdx == 1)
			{
				// calc value
				//double dLength = 0.0;
				//double.TryParse(editLength.Text, out dLength);

				double dLengthPerUnit = 0.0;
				double.TryParse(editUnit.Text, out dLengthPerUnit);

				double dOffsetX = 0.0;
				double.TryParse(editOffsetX.Text, out dOffsetX);

				double dOffsetY = 0.0;
				double.TryParse(editOffsetY.Text, out dOffsetY);

				// set text box

				//double rdLength = 0.0f;
				//ChangeInch(dLength, out rdLength);

				double rdLengthPerUnit = 0.0f;
				ChangeInch(dLengthPerUnit, out rdLengthPerUnit);

				double rdOffsetX = 0.0f;
				ChangeInch(dOffsetX, out rdOffsetX);

				double rdOffsetY = 0.0f;
				ChangeInch(dOffsetY, out rdOffsetY);

				//editLength.Text = rdLength.ToString();
				editUnit.Text = rdLengthPerUnit.ToString();
				editOffsetX.Text = rdOffsetX.ToString();
				editOffsetY.Text = rdOffsetY.ToString();

			}
			// inch -> mm 계산
			else if( nIdx == 0)
			{
				// calc value
				//double dLength = 0.0;
				//double.TryParse(editLength.Text, out dLength);

				double dLengthPerUnit = 0.0;
				double.TryParse(editUnit.Text, out dLengthPerUnit);

				double dOffsetX = 0.0;
				double.TryParse(editOffsetX.Text, out dOffsetX);

				double dOffsetY = 0.0;
				double.TryParse(editOffsetY.Text, out dOffsetY);

				// set text box

				//double rdLength = 0.0f;
				//ChangeMiliMeter(dLength, out rdLength);

				double rdLengthPerUnit = 0.0f;
				ChangeMiliMeter(dLengthPerUnit, out rdLengthPerUnit);

				double rdOffsetX = 0.0f;
				ChangeMiliMeter(dOffsetX, out rdOffsetX);

				double rdOffsetY = 0.0f;
				ChangeMiliMeter(dOffsetY, out rdOffsetY);

				//editLength.Text = rdLength.ToString();
				editUnit.Text = rdLengthPerUnit.ToString();
				editOffsetX.Text = rdOffsetX.ToString();
				editOffsetY.Text = rdOffsetY.ToString();

			}
		}

		private void cmbUnit_SelectedIndexChanged(object sender, EventArgs e)
		{
			int nSelectedIdx = cmbUnit.SelectedIndex;
			if (nSelectedIdx == -1)
				return;

			string szName = cmbUnit.SelectedItem.ToString();
			lbUnit1.Text = szName;
			lbUnit2.Text = szName;
			

			if( nSelectedIdx == 0)
			{
				Document.LengthOfUnit = DXFViewer.LengthUnit.mm;
			}
			else
			{
				Document.LengthOfUnit = DXFViewer.LengthUnit.inch;
			}
			ChangeUnit(nSelectedIdx);

			ParseLength();
		}
		private void ParseLength()
		{
			double d1 = 0.0;
			double d2 = 0.0;

			double.TryParse(editLength.Text, out d1);
			double.TryParse(editUnit.Text, out d2);
			Document.Length = d1;
			Document.UnitValue = GetMiliMeterValue(d2);

		}

		private void cmbScale_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cmbScale.SelectedItem == null)
				return;

			string szText = cmbScale.SelectedItem.ToString();

			if (szText != "CUSTOM")
			{
				string[] vars = szText.Split(':');

				editLength.Text = vars[0];
				editUnit.Text = vars[1];
			}

			ParseLength();
		}


		private void SaveData()
		{
			m_prevUnitSelectedIndex = cmbUnit.SelectedIndex;
			m_prevPaperSizeIndex = cmbPageList.SelectedIndex;
			m_prevPrintAreaIndex = cmbPrintArea.SelectedIndex;

			double.TryParse(editOffsetX.Text, out m_prevOffsetX);
			double.TryParse(editOffsetY.Text, out m_prevOffsetY);
			m_prevCenterPage = ckbPageCenter.Checked;

			m_prevPageOrient = radioHorzPrint.Checked;
			m_prevPageUpsideDown = ckbUpsideDown.Checked;

			double.TryParse(editLength.Text, out m_prevLength);
			double.TryParse(editUnit.Text, out m_prevUnitLength);			
			m_prevScaleSelectedIndex = cmbScale.SelectedIndex;

			m_prevFitPage = ckbFitPage.Checked;

			SaveDocumentValue();			
		}

		private void RestoreData()
		{
			cmbUnit.SelectedIndex = m_prevUnitSelectedIndex;

			cmbPageList.SelectedIndex = m_prevPaperSizeIndex;
			cmbPrintArea.SelectedIndex = m_prevPrintAreaIndex;
			
			editOffsetX.Text = m_prevOffsetX.ToString();
			editOffsetY.Text = m_prevOffsetY.ToString();
			ckbPageCenter.Checked = m_prevCenterPage;

			radioHorzPrint.Checked = m_prevPageOrient;
			radioVertPrint.Checked = !m_prevPageOrient;

			ckbUpsideDown.Checked = m_prevPageUpsideDown;

			editLength.Text = m_prevLength.ToString();
			editUnit.Text = m_prevUnitLength.ToString();			
			cmbScale.SelectedIndex = m_prevScaleSelectedIndex;

			ckbFitPage.Checked = m_prevFitPage;

			
			if (m_prevFitPage == true)
			{
				CalculateRatio();				
			}


			SaveDocumentValue();
		}

		private void SaveDocumentValue()
		{
			// 용지 설정
			if (m_prevUnitSelectedIndex == 0)
			{
				Document.LengthOfUnit = DXFViewer.LengthUnit.mm;
			}
			else
			{
				Document.LengthOfUnit = DXFViewer.LengthUnit.inch;
			}

			string szName = cmbPageList.SelectedItem.ToString();
			foreach (PaperSize size in PrinterSettings.PaperSizes)
			{
				if (size.PaperName == szName)
				{
					Document.DefaultPageSettings.PaperSize = size;
					break;
				}
			}

			// Page관련 설정
			Document.OffsetX = GetMiliMeterValue(m_prevOffsetX);
			Document.OffsetY = GetMiliMeterValue(m_prevOffsetY);
			Document.PrintOnCenter = m_prevCenterPage;

			// 용지 방향 관련 설정
			Document.Landscape = m_prevPageOrient;
			Document.DefaultPageSettings.Landscape = m_prevPageOrient;
			Document.UpsideDown = m_prevPageUpsideDown;

			// 스케일 관련 설정
			Document.FitToPage = m_prevFitPage;

			Document.Length = m_prevLength;
			Document.UnitValue = GetMiliMeterValue(m_prevUnitLength);
		}

		private void FormPrintPageSetup_VisibleChanged(object sender, EventArgs e)
		{
			if( Visible == true)
			{
				RestoreData();
			}
			else
			{
				RoadMan.PanelDXFViewer panel = RoadMan.FormMain.Instance.CurrentPanel;
				if (panel != null)
				{
					panel.ScreenRectPainter.Clear();
					panel.ScreenSelectMode = false;
				}
			}
		}

		private void editOffsetX_Leave(object sender, EventArgs e)
		{
			string szText = editOffsetX.Text;
			double temp = 0.0f;
			if (!double.TryParse(szText, out temp))
			{
                UnE.Utility.UMessageBox.Show(this, "실수형 값만 입력 가능합니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				editOffsetX.Text = m_dOffsetX.ToString();
			}
			else
			{
				m_dOffsetX = temp;
			}
		}

		private void editOffsetY_Leave(object sender, EventArgs e)
		{
			string szText = editOffsetY.Text;
			double temp = 0.0f;
			if (!double.TryParse(szText, out temp))
			{
                UnE.Utility.UMessageBox.Show(this, "실수형 값만 입력 가능합니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				editOffsetY.Text = m_dOffsetY.ToString();
			}
			else
			{
				m_dOffsetY = temp;
			}
		}
		
		private void editLength_Leave(object sender, EventArgs e)
		{
			string szText = editLength.Text;
			double temp = 0.0f;
			if (!double.TryParse(szText, out temp))
			{
                UnE.Utility.UMessageBox.Show(this, "실수형 값만 입력 가능합니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				editLength.Text = m_dLength.ToString();
			}
			else
			{
				m_dLength = temp;
			}
		}

		private void editUnit_Leave(object sender, EventArgs e)
		{
			string szText = editUnit.Text;
			double temp = 0.0f;
			if (!double.TryParse(szText, out temp))
			{
                UnE.Utility.UMessageBox.Show(this, "실수형 값만 입력 가능합니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				editUnit.Text = m_dUnitLength.ToString();
			}
			else
			{
				m_dUnitLength = temp;
			}
		}

		private void editOffsetX_TextChanged(object sender, EventArgs e)
		{
			string szText = editOffsetX.Text;
			if (szText == "")
				return;

			double temp = 0.0f;
			if (!double.TryParse(szText, out temp))
			{
                UnE.Utility.UMessageBox.Show(this, "실수형 값만 입력 가능합니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				editOffsetX.Text = m_dOffsetX.ToString();
			}
			else
			{
				m_dOffsetX = temp;
			}
		}

		private void editOffsetY_TextChanged(object sender, EventArgs e)
		{
			string szText = editOffsetY.Text;
			if (szText == "")
				return;

			double temp = 0.0f;
			if (!double.TryParse(szText, out temp))
			{
                UnE.Utility.UMessageBox.Show(this, "실수형 값만 입력 가능합니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				editOffsetY.Text = m_dOffsetY.ToString();
			}
			else
			{
				m_dOffsetY = temp;
			}
		}

		private void editLength_TextChanged(object sender, EventArgs e)
		{
			string szText = editLength.Text;
			if (szText == "")
				return;

			double temp = 0.0f;
			if (!double.TryParse(szText, out temp))
			{
                UnE.Utility.UMessageBox.Show(this, "실수형 값만 입력 가능합니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				editLength.Text = m_dLength.ToString();
			}
			else
			{
				m_dLength = temp;
			}

		}

		private void editUnit_TextChanged(object sender, EventArgs e)
		{

			string szText = editUnit.Text;
			if (szText == "")
				return;

			double temp = 0.0f;
			if (!double.TryParse(szText, out temp))
			{
                UnE.Utility.UMessageBox.Show(this, "실수형 값만 입력 가능합니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				editUnit.Text = m_dUnitLength.ToString();
			}
			else
			{
				m_dUnitLength = temp;
			}
		}

		private void CancelWndSelect()
		{
			RoadMan.PanelDXFViewer panel = RoadMan.FormMain.Instance.CurrentPanel;
			if (panel != null)
			{
                panel.ScreenRectPainter.Clear();
				panel.ScreenRectPainter.OnSelectScreenRect -= OnSelectRect;
				panel.ScreenSelectMode = false;
                panel.DXFControl.Refresh();
			}
		}

		public void OnSelectRect(Rectangle rect)
		{
			this.ParentForm.WindowState = FormWindowState.Normal;

			RoadMan.PanelDXFViewer panel = RoadMan.FormMain.Instance.CurrentPanel;
			if (panel != null)
			{				
				panel.ScreenRectPainter.OnSelectScreenRect -= OnSelectRect;
				panel.ScreenSelectMode = false;				
			}

			Document.WindowPrintMode = true;
			Document.DrawingRectSize = rect;
		}

		private void btnSelectWnd_Click(object sender, EventArgs e)
		{
			RoadMan.PanelDXFViewer panel = RoadMan.FormMain.Instance.CurrentPanel;
			if( panel != null)
			{
				panel.ScreenRectPainter.OnSelectScreenRect += OnSelectRect;
				panel.ScreenSelectMode = true;
			}
			this.ParentForm.WindowState = FormWindowState.Minimized;
		}

		private Pen m_RectPen = new Pen(Color.Aqua);

		private Image CreateScreenImage(RoadMan.PanelDXFViewer panel)
		{
			try
			{
				DXFViewer.DXFControl mCtrl = panel.DXFControl;
				Bitmap bitmap = new Bitmap(mCtrl.Size.Width, mCtrl.Size.Height, PixelFormat.Format32bppPArgb);
				mCtrl.DrawToBitmap(bitmap, new Rectangle(0, 0, mCtrl.Size.Width, mCtrl.Size.Height));
				return (Image)bitmap;
			}
			catch (Exception)
			{
			}
			return null;
		}
		//private Image backImage =  null;
	
		private void mPreviewPane_Paint(object sender, PaintEventArgs e)
		{
			RoadMan.PanelDXFViewer panel = RoadMan.FormMain.Instance.CurrentPanel;
			if (panel != null && Document != null)
			{
				if( Document.WindowPrintMode)
				{
					Rectangle rect = (Rectangle)Document.DrawingRectSize;
					
					int w1 = panel.Width;
					int w2 = mPreviewPane.Width;

					float ww = (float)w2 / (float)w1;

					int h1 = panel.Height;
					int h2 = mPreviewPane.Height;

					float hh = (float)h2 / (float)h1;

					
					float x = rect.X * ww;
					float y = rect.Y * hh;
					float width = rect.Width * ww;
					float height = rect.Height * hh;
					
					e.Graphics.DrawRectangle(m_RectPen, x, y, width, height);

				}
			}			
		}

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (msg.Msg == WindowMessage.WM_KEYDOWN ||
                msg.Msg == WindowMessage.WM_CHAR ||
                msg.Msg == WindowMessage.WM_SYSKEYDOWN)
            {
                if (keyData == Keys.F1)
                {
                    FormMain.Instance.ShowHelp();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
	}
}
