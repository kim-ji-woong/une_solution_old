using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Text;
using UnE.Overlay;

namespace RoadMan
{
	public partial class FormMemo : Form
	{
		private OverlayPanel mOverlayPane = null;

		public FormMemo()
		{
			InitializeComponent();

			pictureBoxLineColor.BackColor = Color.Red;
			pictureBoxTextColor.BackColor = Color.Yellow;

			comboBox1.SelectedIndex = 0;

			comboBox3.Items.Clear();
			for (int i = 0; i < m_FontSizeList.Length; i++)
			{
				string szText = string.Format("{0}", m_FontSizeList[i]);
				comboBox3.Items.Add(szText);
			}

			InstalledFontCollection installedFontCollection = new InstalledFontCollection();
			m_fontFamilies = installedFontCollection.Families;
			int count = m_fontFamilies.Length;
			for (int j = 0; j < count; ++j)
			{
				string familyName = m_fontFamilies[j].Name;
				cmbFontList.Items.Add(familyName);
			}
			cmbFontList.SelectedIndex = CheckFontName(mFontName);
			comboBox3.SelectedIndex = CheckFontSize("10");

			btnOnOff.Checked = true;

			switch( m_nDrawMode)
			{
                case UnE.Overlay.DrawMode.LINE:
					btnLine.Checked = true;
					break;
                case UnE.Overlay.DrawMode.FREE_DRAW:
					btnFreeDraw.Checked = true;
					break;
                case UnE.Overlay.DrawMode.RECT:
					btnRect.Checked = true;
					break;
                case UnE.Overlay.DrawMode.ELLIPSE:
					btnEllipse.Checked = true;
					break;
                case UnE.Overlay.DrawMode.TEXT:
					btnText.Checked = true;
					break;
                case UnE.Overlay.DrawMode.SELECT:
					btnSelect.Checked = true;
					break;
			}
		}

		public void SetPanel(OverlayPanel panel)
		{
			mOverlayPane = panel;
			mOverlayPane.LineColor = pictureBoxLineColor.BackColor;
			mOverlayPane.TextColor = pictureBoxTextColor.BackColor;
			mOverlayPane.DrawMode = this.m_nDrawMode;
			mOverlayPane.LineThick = 1.0f;
			//mOverlayPane.VisibleOverlay = m_bVisibleOverlay;
            m_bVisibleOverlay = mOverlayPane.VisibleOverlay;

			// font setting		
			mOverlayPane.FontName = mFontName;
			mOverlayPane.FontSize = mFontSize;

			mOverlayPane.FontStyleBold = m_bBold;
			mOverlayPane.FontStyleItalic = m_bItalic;
			mOverlayPane.FontStyleUnderLine = m_bUnderline;



		}
		
		private bool m_bUnderline = false;
		private bool m_bItalic = false;
		private bool m_bBold = false;

		private bool m_bVisibleOverlay = true;
        private UnE.Overlay.DrawMode m_nDrawMode = UnE.Overlay.DrawMode.LINE;
		private string mFontName = "맑은 고딕";
		private int mFontSize = 10;
		private int mFontIndex = -1;
		private int mSizeIndex = -1;

		private int[] m_FontSizeList = { 8, 9, 10, 11, 12, 13, 16, 18, 20, 22, 24, 26, 28 };
		private FontFamily[] m_fontFamilies;

		private void btnDeleteAll_Click(object sender, EventArgs e)
		{
			if (mOverlayPane!= null)
			{
                if (mOverlayPane.EntityList.Count > 0)
                {
                    if (UnE.Utility.UMessageBox.Show(this, "이 도면에 있는 모든 메모가 삭제됩니다.\r\n계속 진행하시겠습니까?", "경고", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                        return;
                }

				mOverlayPane.Clear();
				mOverlayPane.Invalidate();
			}			
		}

        private void btnSelect_Click(object sender, EventArgs e)
		{
            m_nDrawMode = UnE.Overlay.DrawMode.SELECT;
			mOverlayPane.DrawMode = m_nDrawMode;
		}	

		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (mOverlayPane!= null)
			{
				if (mOverlayPane.SelectObject != null)
				{
					mOverlayPane.Remove(mOverlayPane.SelectObject);
					mOverlayPane.SelectObject = null;
					mOverlayPane.Invalidate();
				}
			}			
		}

		private void pictureBoxLineColor_Click(object sender, EventArgs e)
		{
			colorDialog1.Color = pictureBoxLineColor.BackColor;
			colorDialog1.AllowFullOpen = true;
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				pictureBoxLineColor.BackColor = colorDialog1.Color;

				if (mOverlayPane != null)
					mOverlayPane.LineColor = colorDialog1.Color;
			}
		}

		private void pictureBoxTextColor_Click(object sender, EventArgs e)
		{
			colorDialog1.Color = pictureBoxTextColor.BackColor;
			colorDialog1.AllowFullOpen = true;
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				pictureBoxTextColor.BackColor = colorDialog1.Color;

				if (mOverlayPane != null)
					mOverlayPane.TextColor = colorDialog1.Color;
			}
		}

		private float m_nLineThick = 1.0f;
		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{

			string szText = comboBox1.SelectedItem.ToString();
			if (szText == null || szText == "")
				return;

			szText = szText.Replace("px", "");

			float fLineThick = 1.0f;
			if (!float.TryParse(szText, out fLineThick))
			{
				fLineThick = 1.0f;
			}
			
			m_nLineThick = fLineThick;
			if (mOverlayPane!= null)
			{
				mOverlayPane.LineThick = fLineThick;
				if (mOverlayPane.TargetControl != null)
					mOverlayPane.TargetControl.Focus();
			}			
		}

		public Color LineColor
		{
			get { return pictureBoxLineColor.BackColor; }
			set { pictureBoxLineColor.BackColor = value; }
		}

		public Color TextColor
		{
			get { return pictureBoxTextColor.BackColor; }
			set { pictureBoxTextColor.BackColor = value; }
		}

		private void FormMemo_Load(object sender, EventArgs e)
		{
            SetTooltip(btnOnOff, "메모 켜기/끄기");
            SetTooltip(btnDeleteAll, "모든 메모 삭제");
            SetTooltip(btnDelete, "선택 삭제");
            SetTooltip(btnSelect, "메모 선택");
            SetTooltip(btnLine, "선 그리기");
            SetTooltip(btnFreeDraw, "자유곡선 그리기");
            SetTooltip(btnRect, "사각형 그리기");
            SetTooltip(btnEllipse, "타원 그리기");
            SetTooltip(btnText, "텍스트");
            SetTooltip(btnUnderline, "밑줄");
            SetTooltip(btnItalic, "이탤릭");
            SetTooltip(btnStrong, "굵게");
            SetTooltip(pictureBoxLineColor, "선 색상");
            SetTooltip(pictureBoxTextColor, "텍스트 색상");

            btnOnOff.Checked = mOverlayPane.VisibleOverlay;
            btnOnOff.Select();
		}

        private void SetTooltip(Control ctrl, string strTooltipText)
        {
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(ctrl, strTooltipText);
        }

		private void btnLine_CheckedChanged(object sender, EventArgs e)
		{
			if(btnLine.Checked == true)
			{
                m_nDrawMode = UnE.Overlay.DrawMode.LINE;
				
				btnSelect.Checked = false;
				btnFreeDraw.Checked = false;
				btnRect.Checked = false;
				btnEllipse.Checked = false;
				btnText.Checked = false;

				if (mOverlayPane != null)
				{
					mOverlayPane.DrawMode = m_nDrawMode;
					mOverlayPane.Focus();
				}
			}
		}

		private void btnFreeDraw_CheckedChanged(object sender, EventArgs e)
		{
			if (btnFreeDraw.Checked == true)
			{
                m_nDrawMode = UnE.Overlay.DrawMode.FREE_DRAW;

				btnSelect.Checked = false;
				btnLine.Checked = false;
				btnRect.Checked = false;
				btnEllipse.Checked = false;
				btnText.Checked = false;

				if (mOverlayPane != null)
				{
					mOverlayPane.DrawMode = m_nDrawMode;
					mOverlayPane.Focus();
				}
			}
		}

		private void btnRect_CheckedChanged(object sender, EventArgs e)
		{
			if (btnRect.Checked == true)
			{
                m_nDrawMode = UnE.Overlay.DrawMode.RECT;

				btnSelect.Checked = false;
				btnLine.Checked = false;
				btnFreeDraw.Checked = false;
				btnEllipse.Checked = false;
				btnText.Checked = false;

				if (mOverlayPane != null)
				{
					mOverlayPane.DrawMode = m_nDrawMode;
					mOverlayPane.Focus();
				}	
			}
		}

		private void btnEllipse_CheckedChanged(object sender, EventArgs e)
		{
			if (btnEllipse.Checked == true)
			{
                m_nDrawMode = UnE.Overlay.DrawMode.ELLIPSE;
				
				btnSelect.Checked = false;
				btnLine.Checked = false;
				btnFreeDraw.Checked = false;
				btnRect.Checked = false;
				btnText.Checked = false;

				if (mOverlayPane != null)
				{
					mOverlayPane.DrawMode = m_nDrawMode;
					mOverlayPane.Focus();
				}	
			}
		}
				
		private void btnText_CheckedChanged(object sender, EventArgs e)
		{
			if (btnText.Checked == true)
			{
                m_nDrawMode = UnE.Overlay.DrawMode.TEXT;
				
				btnSelect.Checked = false;
				btnLine.Checked = false;
				btnFreeDraw.Checked = false;
				btnRect.Checked = false;
				btnEllipse.Checked = false;

				if (mOverlayPane != null)
				{
					mOverlayPane.DrawMode = m_nDrawMode;
					mOverlayPane.Focus();
				}				
			}			
		}

		private void FormMemo_Enter(object sender, EventArgs e)
		{
			if(mOverlayPane != null)
			{
				mOverlayPane.Focus();
			}
		}

		private void cmbFontList_TextChanged(object sender, EventArgs e)
		{			
		}

		private int CheckFontName(string szText)
		{
			//bool bFind = false;
			for (int j = 0; j < m_fontFamilies.Length; ++j)
			{
				string familyName = m_fontFamilies[j].Name;
				if(familyName == szText)
				{
					//bFind = true;
					return j;
				}
			}
			cmbFontList.SelectedText = mFontName;
			return -1;
		}

		private int CheckFontSize(string szText)
		{
			int nIdx = -1;
			if (!int.TryParse(szText, out nIdx))
				return -1;

			for (int j = 0; j < m_FontSizeList.Length; ++j)
			{
				if (nIdx == m_FontSizeList[j])
					return j;				
			}
			return -1;
		}	

		private void cmbFontList_SelectedIndexChanged(object sender, EventArgs e)
		{
			int nIdx = cmbFontList.SelectedIndex;
			if (nIdx == -1)
				return;

			mFontName = cmbFontList.Items[nIdx].ToString();


			if (mOverlayPane != null)
				mOverlayPane.FontName = mFontName;
			mFontIndex = nIdx;
		}

		private void cmbFontList_KeyDown(object sender, KeyEventArgs e)
		{
			if( e.KeyCode == Keys.Enter)
			{
				string szText = cmbFontList.SelectedText;
				int nIdx = CheckFontName(szText);
				if( nIdx != -1)
				{
					cmbFontList.SelectedIndex = nIdx;
				}
				else
				{
					cmbFontList.SelectedIndex = mFontIndex;
				}
			}
		}

		private void cmbFontList_Leave(object sender, EventArgs e)
		{
			string szText = cmbFontList.SelectedText;
			int nIdx = CheckFontName(szText);
			if (nIdx != -1)
			{
				cmbFontList.SelectedIndex = nIdx;
			}
			else
			{
				cmbFontList.SelectedIndex = mFontIndex;
			}
		}

		private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
		{
			int nIdx = comboBox3.SelectedIndex;
			if (nIdx == -1)
				return;

			string szText = comboBox3.Items[nIdx].ToString();
			mFontSize = int.Parse(szText);
			
			if (mOverlayPane != null)
				mOverlayPane.FontSize = mFontSize;
			mSizeIndex = nIdx;
		}

		private void comboBox3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				string szText = comboBox3.SelectedText;
				int nIdx = CheckFontSize(szText);
				if (nIdx != -1)
				{
					comboBox3.SelectedIndex = nIdx;
				}
				else
				{
					comboBox3.SelectedIndex = mSizeIndex;
				}
			}
		}

		private void comboBox3_Leave(object sender, EventArgs e)
		{
			string szText = comboBox3.SelectedText;
			int nIdx = CheckFontSize(szText);
			if (nIdx != -1)
			{
				comboBox3.SelectedIndex = nIdx;
			}
			else
			{
				comboBox3.SelectedIndex = mSizeIndex;				
			}
		}
		
		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if(btnOnOff.Checked == true)
			{
				m_bVisibleOverlay = true;

				if (mOverlayPane != null)
					mOverlayPane.VisibleOverlay = true;
			}
			else
			{
				m_bVisibleOverlay = false;
				if (mOverlayPane != null)
					mOverlayPane.VisibleOverlay = false;
			}
		}

		private void btnUnderline_CheckedChanged(object sender, EventArgs e)
		{
			m_bUnderline = btnUnderline.Checked;
			if( mOverlayPane != null)
			{
				mOverlayPane.FontStyleUnderLine = m_bUnderline;
			}
		}

		private void btnItalic_CheckedChanged(object sender, EventArgs e)
		{
			m_bItalic = btnItalic.Checked;
			if (mOverlayPane != null)
			{
				mOverlayPane.FontStyleItalic = m_bItalic;
			}
		}

		private void btnStrong_CheckedChanged(object sender, EventArgs e)
		{
			m_bBold = btnStrong.Checked;
			if (mOverlayPane != null)
			{
				mOverlayPane.FontStyleBold = m_bBold;
			}
		}

		private void btnSelect_CheckedChanged(object sender, EventArgs e)
		{
			if (btnSelect.Checked == true)
			{
                m_nDrawMode = UnE.Overlay.DrawMode.SELECT;
				
				btnLine.Checked = false;
				btnFreeDraw.Checked = false;
				btnRect.Checked = false;
				btnEllipse.Checked = false;
				btnText.Checked = false;

				if (mOverlayPane != null)
				{
					mOverlayPane.DrawMode = m_nDrawMode;
					mOverlayPane.Focus();
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
