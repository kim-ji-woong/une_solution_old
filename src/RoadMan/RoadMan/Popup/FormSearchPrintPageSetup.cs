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
	public partial class FormGridPrintPageSetup : Form
	{
		private int m_prevPrintAreaIndex = 0;
		private int m_prevPaperSizeIndex = 0;
		private bool m_prevPageOrient = false;

		private int m_prevMarginLeft = 10;
		private int m_prevMarginRight = 10;
		private int m_prevMarginTop = 10;
		private int m_prevMarginBottom = 10;
		
		private int m_prevUnitSelectedIndex = 0;
		private int m_prevScaleSelectedIndex = 0;
		
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

		public PrintDocument Document
		{
			get { return (PrintDocument)pageSetupDialog1.Document; }
			set { pageSetupDialog1.Document = value; }
		}

		
		private int m_dMarginLeft = 20;
		public int MarginLeft
		{
			get { return m_dMarginLeft; }
			set { m_dMarginLeft = value; }
		}

		private int m_dMarginTop = 20;
		public int MarginTop
		{
			get { return m_dMarginTop; }
			set { m_dMarginTop = value; }
		}

		private int m_dMarginRight = 20;
		public int MarginRight
		{
			get { return m_dMarginRight; }
			set { m_dMarginRight = value; }
		}

		private int m_dMarginBottom = 20;
		public int MarginBottom
		{
			get { return m_dMarginBottom; }
			set { m_dMarginBottom = value; }
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
		
		public FormGridPrintPageSetup()
		{
			InitializeComponent();
			
		}
		
		private void btnCancel_Click(object sender, EventArgs e)
		{
			// 변경되기 이전값으로 Document의 Value를 저장한다.
			//SaveDocumentValue();

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
			// 변경된 값을 먼저 저장후 Document의 Value를 저장한다.
			//SaveData();

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


		private void previewCloseBtn_Click(object sender, EventArgs e)
		{
			if( frame != null)
			{
				frame.Close();
			}
		}

		private FormPrintFrame frame = null;
		private PrintPreviewDialog printPreviewDialog1 = null;
		private void btnPreview_Click(object sender, EventArgs e)
		{	
			printPreviewDialog1 = new PrintPreviewDialog();
			printPreviewDialog1.Text = "인쇄 미리 보기";
			printPreviewDialog1.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			printPreviewDialog1.Document = this.Document;

			int nOffset1 = (int)Math.Round((double)m_dMarginLeft * 100.0 / 25.4);
			int nOffset2 = (int)Math.Round((double)m_dMarginTop * 100.0 / 25.4);
			int nOffset3 = (int)Math.Round((double)m_dMarginRight * 100.0 / 25.4);
			int nOffset4 = (int)Math.Round((double)m_dMarginBottom * 100.0 / 25.4);

			printPreviewDialog1.Document.DefaultPageSettings.Margins.Left = nOffset1;
			printPreviewDialog1.Document.DefaultPageSettings.Margins.Top = nOffset2;
			printPreviewDialog1.Document.DefaultPageSettings.Margins.Right = nOffset3;
			printPreviewDialog1.Document.DefaultPageSettings.Margins.Bottom = nOffset4;

			ToolStripButton btn = (ToolStripButton)((ToolStrip)printPreviewDialog1.Controls[1]).Items[9];
			btn.Click += previewCloseBtn_Click;

			frame = new FormPrintFrame(printPreviewDialog1);
			frame.Text = "인쇄 미리 보기";			
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
			m_prevPrintAreaIndex = 0;				
			
		}

		private void FormPrintPageSetup_FormClosing(object sender, FormClosingEventArgs e)
		{
		}
			

		private void radioHorzPrint_CheckedChanged(object sender, EventArgs e)
		{
			if (Document == null)
				return;

			if( radioHorzPrint.Checked == true)
			{
				Document.DefaultPageSettings.Landscape = true;
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
                pictureBoxPrintDirection.BackgroundImage = global::RoadMan.Properties.Resources._64Vertical_normal;
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

		}

		public void RestoreData()
		{

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

		private void marginLeft_Leave(object sender, EventArgs e)
		{
			string szText = marginLeft.Text;
			int temp = 0;
			if (!int.TryParse(szText, out temp))
			{
				UnE.Utility.UMessageBox.Show(this, "정수형 값만 입력 가능합니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				marginLeft.Text = m_dMarginLeft.ToString();
			}
			else
			{
				m_dMarginLeft = temp;
			}
		}

		private void marginTop_Leave(object sender, EventArgs e)
		{
			string szText = marginTop.Text;
			int temp = 0;
			if (!int.TryParse(szText, out temp))
			{
                UnE.Utility.UMessageBox.Show(this, "정수형 값만 입력 가능합니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				marginTop.Text = m_dMarginTop.ToString();
			}
			else
			{
				m_dMarginTop = temp;
			}
		}		
		
		private void marginLeft_TextChanged(object sender, EventArgs e)
		{
			string szText = marginLeft.Text;
			if (szText == "")
				return;

			int temp = 0;
			if (!int.TryParse(szText, out temp))
			{
				UnE.Utility.UMessageBox.Show(this, "정수형 값만 입력 가능합니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				marginLeft.Text = m_dMarginLeft.ToString();
			}
			else
			{
				m_dMarginLeft = temp;
			}
		}



		private void marginTop_TextChanged(object sender, EventArgs e)
		{
			string szText = marginTop.Text;
			if (szText == "")
				return;

			int temp = 0;
			if (!int.TryParse(szText, out temp))
			{
				UnE.Utility.UMessageBox.Show(this, "정수형 값만 입력 가능합니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				marginTop.Text = m_dMarginTop.ToString();
			}
			else
			{
				m_dMarginTop = temp;
			}
		}

		private void marginRight_TextChanged(object sender, EventArgs e)
		{
			string szText = marginRight.Text;
			if (szText == "")
				return;

			int temp = 0;
			if (!int.TryParse(szText, out temp))
			{
				UnE.Utility.UMessageBox.Show(this, "정수형 값만 입력 가능합니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				marginRight.Text = m_dMarginRight.ToString();
			}
			else
			{
				m_dMarginRight = temp;
			}
		}

		private void marginRight_Leave(object sender, EventArgs e)
		{
			string szText = marginRight.Text;
			if (szText == "")
				return;

			int temp = 0;
			if (!int.TryParse(szText, out temp))
			{
				UnE.Utility.UMessageBox.Show(this, "정수형 값만 입력 가능합니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				marginRight.Text = m_dMarginRight.ToString();
			}
			else
			{
				m_dMarginRight = temp;
			}
		}

		private void marginBottom_TextChanged(object sender, EventArgs e)
		{
			string szText = marginBottom.Text;
			if (szText == "")
				return;

			int temp = 0;
			if (!int.TryParse(szText, out temp))
			{
				UnE.Utility.UMessageBox.Show(this, "정수형 값만 입력 가능합니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				marginBottom.Text = m_dMarginBottom.ToString();
			}
			else
			{
				m_dMarginBottom = temp;
			}
		}

		private void marginBottom_Leave(object sender, EventArgs e)
		{
			string szText = marginBottom.Text;
			if (szText == "")
				return;

			int temp = 0;
			if (!int.TryParse(szText, out temp))
			{
				UnE.Utility.UMessageBox.Show(this, "정수형 값만 입력 가능합니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				marginBottom.Text = m_dMarginBottom.ToString();
			}
			else
			{
				m_dMarginBottom = temp;
			}
		}

		private string m_szHeaderText = "";
		public string HeaderText
		{
			get { return m_szHeaderText; }			
			set 
			{
				m_szHeaderText = value; 

				if( editHeader != null)
				{
					editHeader.Text = m_szHeaderText;
				}
			}
		}		


		private bool m_bShowHeader = false;
		public bool ShowHeader
		{
			get { return m_bShowHeader; }
			set 
			{
				m_bShowHeader = value;
				EnabledHeader(value);
			}
		}

		private void EnabledHeader(bool bVal)
		{
			editHeader.Enabled = bVal;
			ckbShoweader.Checked = bVal;
		}

		private bool m_bShowDate = false;
		public bool ShowDate
		{
			get { return m_bShowDate; }
			set 
			{ 
				m_bShowDate = value;
				ckbShowDate.Checked = value;
			}
		}
			
	
		
		private void ckbShoweader_CheckedChanged(object sender, EventArgs e)
		{
			m_bShowHeader = ckbShoweader.Checked;
			editHeader.Enabled = m_bShowHeader;			
		}

		private void ckbShowDate_CheckedChanged(object sender, EventArgs e)
		{
			m_bShowDate = ckbShowDate.Checked;
		}
				

		private void editHeader_TextChanged(object sender, EventArgs e)
		{
			string szText = editHeader.Text;
			if (m_szHeaderText == szText)
				return;
			m_szHeaderText = szText;
		}

		private void editHeader_Leave(object sender, EventArgs e)
		{
			string szText = editHeader.Text;
			if (m_szHeaderText == szText)
				return;
			m_szHeaderText = szText;
		}		
	}
}
