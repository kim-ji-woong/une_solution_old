using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadMan
{
	public partial class FormOption : Form
	{
		private float dX = 0.0f;
		private float dY = 0.0f;
        private PanelDXFViewer m_panel = null;

        private bool m_checkObjectZoom;
        private bool m_visibleBackImage;
        private bool m_completeRatio;

        private Dictionary<PanelDXFViewer, PanelData> m_dicPanelDatas = new Dictionary<PanelDXFViewer, PanelData>();

		public FormOption()
		{
			InitializeComponent();

			//CheckControlValue();
			/*PanelDXFViewer viewer = FormMain.Instance.CurrentPanel;
			if (viewer != null)
			{
				dX = 0.0f;// (float)viewer.DXFControl.MovedVertex.x;
				dY = 0.0f;//(float)viewer.DXFControl.MovedVertex.y;

				//viewer.UnderImagePainter.UseUnderImage = true;

				editOffsetX.Text = string.Format("{0}", viewer.UnderImagePainter.Offset.X - dX);
				editOffsetY.Text = string.Format("{0}", viewer.UnderImagePainter.Offset.Y - dY);
				editWidth.Text = string.Format("{0}", viewer.UnderImagePainter.Size.Width);
				editHeight.Text = string.Format("{0}", viewer.UnderImagePainter.Size.Height);
				
				editImagePath.Text = viewer.UnderImagePainter.ImagePath;			
				
			}
			//829, 517
			this.Size = new Size(525, 517);
			groupBox3.Location = new Point(546, 6);*/
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
            RollBack();
			//SetColor(backupColor);

            if (m_panel != null)
            {
                m_panel.SelectRectPainter.OnSelectRect -= CalcImageBound;
                m_panel.SelectMode = false;
            }

			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Close();
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
            Commit();
			/*Options.Instance.BackupCount = Convert.ToInt32(textBoxBackupFileCount.Text);

			ApplyImage();

            Options.Instance.ZoomInOnSelectStreet = ckbObjectZoom.Checked;

			SetColor(pictureBoxBackColor.BackColor);*/

            if (m_panel != null)
            {
                m_panel.SelectRectPainter.OnSelectRect -= CalcImageBound;
                m_panel.SelectMode = false;
            }
			
			DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Close();
		}

        private void Commit()
        {
            Options.Instance.BackupCount = (int)textBoxBackupFileCount.Tag;

            if (pictureBoxBackColor.BackColor != Options.Instance.BackColor)
                Options.Instance.BackColor = pictureBoxBackColor.BackColor;

            ApplyImage();

            foreach (KeyValuePair<PanelDXFViewer, PanelData> pair in m_dicPanelDatas)
            {
                if (pair.Value.RegionName != null)
                    pair.Key.RegionName = pair.Value.RegionName;
            }
        }

        private void RollBack()
        {
            bool needRefresh = false;

            if (pictureBoxBackColor.BackColor != Options.Instance.BackColor)
            {
                SetColor(Options.Instance.BackColor);
                needRefresh = true;
            }

            if (Options.Instance.VisibleBackgroundImage != m_visibleBackImage)
            {
                Options.Instance.VisibleBackgroundImage = m_visibleBackImage;
                needRefresh = true;
            }

            if (Options.Instance.ZoomOnSelectStreet != m_checkObjectZoom)
                Options.Instance.ZoomOnSelectStreet = m_checkObjectZoom;

            if (Options.Instance.CompleteRatioByArea != m_completeRatio)
            {
                Options.Instance.CompleteRatioByArea = m_completeRatio;

                m_panel.ProcessScheduleForm.SetCompleteRatio(Options.Instance.CompleteRatioByArea);
                m_panel.ProcessResultForm.SetCompleteRatio(Options.Instance.CompleteRatioByArea);
            }

            if (needRefresh)
                m_panel.DXFControl.Refresh();
        }

		private void FormOption_Load(object sender, EventArgs e)
		{
            m_panel = FormMain.Instance.CurrentPanel;

			textBoxBackupFileCount.Text = Options.Instance.BackupCount.ToString();
            textBoxBackupFileCount.Tag = Options.Instance.BackupCount;
            pictureBoxBackColor.BackColor = Options.Instance.BackColor;

            m_visibleBackImage = checkBoxShowBackgroundImage.Checked = Options.Instance.VisibleBackgroundImage;
            m_checkObjectZoom = ckbObjectZoom.Checked = Options.Instance.ZoomOnSelectStreet;

            if (Options.Instance.CompleteRatioByArea)
            {
                radioAreaRatio.Checked = true;
                m_completeRatio = true;
            }
            else
            {
                radioLengthRatio.Checked = true;
                m_completeRatio = false;
            }

            ShowArrowButtons(false);
		}

		private void btnOpenFile_Click(object sender, EventArgs e)
		{
			openFileDialog1.Filter =
				"Jpeg files (*.jpg)|*.jpg|Png files (*.png)|*.png|Bmp files (*.bmp)|*.bmp|All files (*.*)|*.*";

			openFileDialog1.DefaultExt = "png";
			openFileDialog1.RestoreDirectory = true;
			openFileDialog1.Multiselect = false;
			openFileDialog1.FileName = "";
			if(openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				string szFileName = openFileDialog1.FileName;
				editImagePath.Text = szFileName;
			}
		}

		private void CheckControlValue()
		{
            if (m_panel != null)
			{
                PointF ptLeftDown = m_panel.GetLBCornerPos();
				editOffsetX.Text = string.Format("{0}", ptLeftDown.X);
				editOffsetY.Text = string.Format("{0}", ptLeftDown.Y);

                SizeF size = m_panel.GetDrawSize();
				editWidth.Text = string.Format("{0}", size.Width);
				editHeight.Text = string.Format("{0}", size.Height);
			}
		}



        private void checkBoxSelectOnScreen_CheckedChanged(object sender, EventArgs e)
		{
			bool bChecked = !checkBoxSelectOnScreen.Checked;
			//if( bChecked == true)
			{
				editOffsetX.Enabled = bChecked;
				editOffsetY.Enabled = bChecked;
				editWidth.Enabled = bChecked;
				editHeight.Enabled = bChecked;
			}
		}

        private void checkBoxShowBackgroundImage_CheckedChanged(object sender, EventArgs e)
        {
            if (Options.Instance.VisibleBackgroundImage != checkBoxShowBackgroundImage.Checked)
            {
                Options.Instance.VisibleBackgroundImage = checkBoxShowBackgroundImage.Checked;

                DXFViewer.DXFControl ctrl = FormMain.Instance.CurrentDXFControl;

                if (ctrl != null)
                    ctrl.Refresh();
            }
        }

		private void CalcImageBound(RectangleF rect)
		{
            if (m_panel != null)
			{
                m_panel.SelectRectPainter.OnSelectRect -= CalcImageBound;
                m_panel.SelectMode = false;
			}

			editOffsetX.Text = string.Format("{0}", rect.X - dX);
			editOffsetY.Text = string.Format("{0}", rect.Y - dY);
			editWidth.Text = string.Format("{0}", rect.Width);
			editHeight.Text = string.Format("{0}", rect.Height);

			ApplyImage();
		}

		private void ApplyImage()
		{
			string szText1 = editOffsetX.Text;
			string szText2 = editOffsetY.Text;

			float x = 0.0f;
			float y = 0.0f;

			if (!float.TryParse(szText1, out x))
				return;

			if (!float.TryParse(szText2, out y))
				return;



			float width = 0.0f;
			float height = 0.0f;
			szText1 = editWidth.Text;
			szText2 = editHeight.Text;
			if (!float.TryParse(szText1, out width))
				return;

			if (!float.TryParse(szText2, out height))
				return;

            if (m_panel != null)
			{
				//viewer.UnderImagePainter.UseUnderImage = true;
                m_panel.UnderImagePainter.SetOffset(x + dX, y + dY);
                m_panel.UnderImagePainter.SetSize(width, height);
                m_panel.UnderImagePainter.SetImage(editImagePath.Text);
			}			
		}

		private void btnApplayImage_Click(object sender, EventArgs e)
		{
			if( btnApplayImage.Text == "위치조정종료")
			{
				btnApplayImage.Text = "위치적용하기";

                if (m_panel != null)
				{
                    m_panel.SelectRectPainter.OnSelectRect -= CalcImageBound;
                    m_panel.SelectMode = false;
				}
				//829, 517
                ShowArrowButtons(false);
			}
			else
			{
				btnApplayImage.Text = "위치조정종료";

                if (m_panel != null)
				{
                    m_panel.SelectRectPainter.OnSelectRect += CalcImageBound;
                    m_panel.SelectMode = true;
				}
                ShowArrowButtons(true);
			}

            ApplyImage();
		}

        private void ShowArrowButtons(bool visible)
        {
            if (visible)
            {
                this.Size = new Size(829, 517);
                groupBox3.Location = new Point(546 - 30, 6);
            }
            else
            {
                this.Size = new Size(525, 517);
                groupBox3.Location = new Point(546, 6);
            }
        }

		private void PreviewApply()
		{
			string szText1 = editOffsetX.Text;
			string szText2 = editOffsetY.Text;

			float x = 0.0f;
			float y = 0.0f;

			if (!float.TryParse(szText1, out x))
				return;

			if (!float.TryParse(szText2, out y))
				return;

			float width = 0.0f;
			float height = 0.0f;
			szText1 = editWidth.Text;
			szText2 = editHeight.Text;
			if (!float.TryParse(szText1, out width))
				return;

			if (!float.TryParse(szText2, out height))
				return;

            if (m_panel != null)
			{
				//viewer.UnderImagePainter.UseUnderImage = true;
                m_panel.UnderImagePainter.SetOffset(x + dX, y + dY);
                m_panel.UnderImagePainter.SetSize(width, height);
				//viewer.UnderImagePainter.SetImage(editImagePath.Text);
			}	
		}


		private float m_div = 10.0f;
        private void btnIncreaseWidth_Click(object sender, EventArgs e)
		{
			float fwidth = 0.0f;
			string szText1 = editWidth.Text;
		
			if (!float.TryParse(szText1, out fwidth))
				return;
			fwidth += m_div;

			editWidth.Text = string.Format("{0}", fwidth);

			PreviewApply();
		}

        private void btnReduceWidth_Click(object sender, EventArgs e)
		{
			float fwidth = 0.0f;
			string szText1 = editWidth.Text;

			if (!float.TryParse(szText1, out fwidth))
				return;
			fwidth -= m_div;

			editWidth.Text = string.Format("{0}", fwidth);

			PreviewApply();
		}

        private void btnIncreaseHeight_Click(object sender, EventArgs e)
		{
			float fwidth = 0.0f;
			string szText1 = editHeight.Text;

			if (!float.TryParse(szText1, out fwidth))
				return;
			fwidth += m_div;

			editHeight.Text = string.Format("{0}", fwidth);

			PreviewApply();
		}

        private void btnReduceHeight_Click(object sender, EventArgs e)
		{
			float fwidth = 0.0f;
			string szText1 = editHeight.Text;

			if (!float.TryParse(szText1, out fwidth))
				return;
			fwidth -= m_div;

			editHeight.Text = string.Format("{0}", fwidth);

			PreviewApply();
		}

        private void btnUp_Click(object sender, EventArgs e)
		{
			float fwidth = 0.0f;
			string szText1 = editOffsetY.Text;

			if (!float.TryParse(szText1, out fwidth))
				return;
			fwidth += m_div;

			editOffsetY.Text = string.Format("{0}", fwidth);

			PreviewApply();
		}

        private void btnDown_Click(object sender, EventArgs e)
		{
			float fwidth = 0.0f;
			string szText1 = editOffsetY.Text;

			if (!float.TryParse(szText1, out fwidth))
				return;
			fwidth -= m_div;

			editOffsetY.Text = string.Format("{0}", fwidth);

			PreviewApply();
		}

        private void btnRight_Click(object sender, EventArgs e)
		{
			float fwidth = 0.0f;
			string szText1 = editOffsetX.Text;

			if (!float.TryParse(szText1, out fwidth))
				return;
			fwidth += m_div;

			editOffsetX.Text = string.Format("{0}", fwidth);

			PreviewApply();
		}

        private void btnLeft_Click(object sender, EventArgs e)
		{
			float fwidth = 0.0f;
			string szText1 = editOffsetX.Text;

			if (!float.TryParse(szText1, out fwidth))
				return;
			fwidth -= m_div;

			editOffsetX.Text = string.Format("{0}", fwidth);

			PreviewApply();
		}

		private void editDivision_TextChanged(object sender, EventArgs e)
		{
			string szText = editDivision.Text;

			float temp = 0.0f;

			if(float.TryParse(szText, out temp))
			{
				m_div = temp;
			}
			else
			{
				string szMsg = "숫자만 입력 가능합니다.";
                UnE.Utility.UMessageBox.Show(this, szMsg, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);		
				
				editDivision.Text = string.Format("{0}", m_div);
			}
		}

		private void editDivision_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			
		}

        private void btnSettingStreets_Click(object sender, EventArgs e)
        {
            if (m_panel == null)
                return;

            if (m_panel.SettingStreet == null)
            {
                FormSettingStreetName frm = new FormSettingStreetName(m_panel);
				DialogFormFrame frameStreet = new DialogFormFrame(frm);
				frameStreet.Show(FormMain.Instance);
            }
        }

        private void FormOption_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormMain.Instance.OptionForm = null;
        }

        public void SetPanel(PanelDXFViewer panel)
        {
            m_panel = panel;

            btnSettingStreets.Enabled = m_panel != null;

            if (tabControl1.SelectedTab.Text == "도면")
            {
                SetDXFTab();
            }
        }

		private void btnSetLayer_Click(object sender, EventArgs e)
		{
            if (m_panel != null)
			{
                FormLayer form = m_panel.LayerForm;
				if( form != null)
				{
					FormShowLayer showLayer = new FormShowLayer();


					showLayer.TopMost = true;
					showLayer.AllLayers = form.GetLayerList();

					DialogFormFrame frameLayer = new DialogFormFrame(showLayer);
					frameLayer.ShowDialog(this);


					form.ChangeShowLayer();					
				}
			}
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			colorDialog1.AllowFullOpen = true;
			colorDialog1.FullOpen = true;
			colorDialog1.Color = pictureBoxBackColor.BackColor;

			if( colorDialog1.ShowDialog() == DialogResult.OK)
			{
				pictureBoxBackColor.BackColor = colorDialog1.Color;
			}
		}

		private void SetColor(Color color)
		{
			int nCount = FormMain.Instance.GetTabPageCount();
			for (int i = 0; i < nCount; i++)
			{
				TabPage page = FormMain.Instance.GetTabPage(i);
				PanelDXFViewer pane = (PanelDXFViewer)page.Tag;
				if (pane != null)
				{
					pane.DXFControl.BackColor = color;
				}
			}
		}

		private void btnDeleteImage_Click(object sender, EventArgs e)
		{
            if (m_panel != null)
			{
				//viewer.UnderImagePainter.UseUnderImage = true;
                m_panel.UnderImagePainter.SetOffset(0, 0);
                m_panel.UnderImagePainter.SetSize(0, 0);
                m_panel.UnderImagePainter.SetImage("");
			}

			dX = 0.0f;// (float)viewer.DXFControl.MovedVertex.x;
			dY = 0.0f;//(float)viewer.DXFControl.MovedVertex.y;

			//viewer.UnderImagePainter.UseUnderImage = true;

			editOffsetX.Text = "";
			editOffsetY.Text = "";
			editWidth.Text = "";
			editHeight.Text = "";

			editImagePath.Text = "";

			if (btnApplayImage.Text == "위치조정종료")
			{
				btnApplayImage.Text = "위치적용하기";

                if (m_panel != null)
				{
                    m_panel.SelectRectPainter.OnSelectRect -= CalcImageBound;
                    m_panel.SelectMode = false;
				}
				this.Size = new Size(525, 466);
				groupBox3.Location = new Point(546, 24);
			}
			
		}

        private void textBoxBackupFileCount_TextChanged(object sender, EventArgs e)
        {
            if (textBoxBackupFileCount.Text.Length == 0)
            {
                textBoxBackupFileCount.Tag = 0;
            }
            else
            {
                int nFileCount;

                if (!int.TryParse(textBoxBackupFileCount.Text, out nFileCount) || nFileCount < 0)
                {
                    UnE.Utility.UMessageBox.Show(this, "[백업파일 개수]는 0이상의 정수값만 사용할 수 있습니다.");

                    if (textBoxBackupFileCount.Tag == null)
                        textBoxBackupFileCount.Text = "";
                    else
                        textBoxBackupFileCount.Text = ((int)textBoxBackupFileCount.Tag).ToString();
                }
                else
                    textBoxBackupFileCount.Tag = nFileCount;
            }
        }

        private void ckbObjectZoom_CheckedChanged(object sender, EventArgs e)
        {
            Options.Instance.ZoomOnSelectStreet = ckbObjectZoom.Checked;
        }

        private void pictureBoxBackColor_Click(object sender, EventArgs e)
        {
            colorDialog1.AllowFullOpen = true;
            colorDialog1.FullOpen = true;
            colorDialog1.Color = pictureBoxBackColor.BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBoxBackColor.BackColor = colorDialog1.Color;
                SetColor(pictureBoxBackColor.BackColor);
                m_panel.DXFControl.Refresh();
            }
        }

        private void radioRatio_CheckedChanged(object sender, EventArgs e)
        {
            if (radioAreaRatio.Checked)
                Options.Instance.CompleteRatioByArea = true;
            else
                Options.Instance.CompleteRatioByArea = false;

            m_panel.ProcessScheduleForm.SetCompleteRatio(Options.Instance.CompleteRatioByArea);
            m_panel.ProcessResultForm.SetCompleteRatio(Options.Instance.CompleteRatioByArea);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == null)
                return;

            if (tabControl1.SelectedTab.Text == "도면")
            {
                SetDXFTab();

                textBoxRegionName.Select();
                textBoxRegionName.Select(0, 0);
            }
            else if (tabControl1.SelectedTab.Text == "일반")
            {
                textBoxBackupFileCount.Select();
                textBoxBackupFileCount.Select(0, 0);
            }
        }

        private void SetDXFTab()
        {
            if (m_panel != null)
            {
                dX = 0.0f;// (float)viewer.DXFControl.MovedVertex.x;
                dY = 0.0f;//(float)viewer.DXFControl.MovedVertex.y;

                //viewer.UnderImagePainter.UseUnderImage = true;

                editOffsetX.Text = string.Format("{0}", m_panel.UnderImagePainter.Offset.X - dX);
                editOffsetY.Text = string.Format("{0}", m_panel.UnderImagePainter.Offset.Y - dY);
                editWidth.Text = string.Format("{0}", m_panel.UnderImagePainter.Size.Width);
                editHeight.Text = string.Format("{0}", m_panel.UnderImagePainter.Size.Height);

                editImagePath.Text = m_panel.UnderImagePainter.ImagePath;

                PanelData panelData = null;
                m_dicPanelDatas.TryGetValue(m_panel, out panelData);

                if (panelData != null && panelData.RegionName != null)
                    textBoxRegionName.Text = panelData.RegionName;
                else if (m_panel.RegionName.Length > 0)
                    textBoxRegionName.Text = m_panel.RegionName;
                else
                    textBoxRegionName.Text = FormMain.Instance.GetProjectName(m_panel.DXFFilePath);
            }
            //829, 517
            this.Size = new Size(525, 517);
            groupBox3.Location = new Point(546, 6);
        }

        private void textBoxRegionName_TextChanged(object sender, EventArgs e)
        {
            if (m_panel != null)
            {
                PanelData data = null;

                if (!m_dicPanelDatas.TryGetValue(m_panel, out data))
                {
                    data = new PanelData();
                    m_dicPanelDatas[m_panel] = data;
                }

                data.RegionName = textBoxRegionName.Text;
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

        private class PanelData
        {
            private string m_strRegionName = null;

            public string RegionName
            {
                get { return m_strRegionName; }
                set { m_strRegionName = value; }
            }
        }
	}
}
