using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sections;

namespace SOPManager
{
	public partial class FormToolBox : Form
	{
		public FormToolBox()
		{
			InitializeComponent();

			tmrUpdateCmd.Enabled = true;
			tmrUpdateCmd.Start();
		}

		private void btnZoomIn_Click(object sender, EventArgs e)
		{
			PanelSectionEx panel = FormMain.Instance.GetPageLevel().GetCurrentPanel();
			if( panel != null)
			{
				panel.ZoomIn();
			}
		}

		private void btnZoomOut_Click(object sender, EventArgs e)
		{
			PanelSectionEx panel = FormMain.Instance.GetPageLevel().GetCurrentPanel();
			if (panel != null)
			{

				panel.ZoomOut();
			}
		}

		private void btnFitView_Click(object sender, EventArgs e)
		{
			PanelSectionEx panel = FormMain.Instance.GetPageLevel().GetCurrentPanel();
			if (panel != null)
			{
				panel.FitView();
				panel.Invalidate();
			}
		}

		private void btnResetView_Click(object sender, EventArgs e)
		{
			PanelSectionEx panel = FormMain.Instance.GetPageLevel().GetCurrentPanel();
			if (panel != null)
			{
				panel.NormalView();
				panel.Invalidate();
			}
		}

		private void btnScreenShot_Click(object sender, EventArgs e)
		{
			PanelSectionEx panel = FormMain.Instance.GetPageLevel().GetCurrentPanel();
			if (panel != null)
			{
				Bitmap b = new Bitmap(panel.Width, panel.Height);
				panel.DrawToBitmap(b, new Rectangle(0, 0, b.Width, b.Height));
				Clipboard.SetImage((Image)b);
			}
		}

        // 홈뷰 저장하기
		private void btnSaveHome_Click(object sender, EventArgs e)
		{
			PanelSectionEx panel = FormMain.Instance.GetPageLevel().GetCurrentPanel();
			if (panel != null)
			{
				panel.SaveHomeView();
			}
		}

        // 홈뷰
		private void btnHomeView_Click(object sender, EventArgs e)
		{
			PanelSectionEx panel = FormMain.Instance.GetPageLevel().GetCurrentPanel();
			if (panel != null)
			{
				if(panel.HomeView())
					panel.Invalidate();
			}
		}

		private void tmrUpdateCmd_Tick(object sender, EventArgs e)
		{
			if (FormMain.Instance.GetPageLevel() == null)
				return;

			PanelSectionEx panel = FormMain.Instance.GetPageLevel().GetCurrentPanel();
			if (panel != null)
			{
				btnZoomIn.Enabled = true;
				btnZoomOut.Enabled = true;
				btnFitView.Enabled = true;
				btnResetView.Enabled = true;
				btnScreenShot.Enabled = true;
				
                btnPrint.Enabled = false;
				
                btnSaveHome.Enabled = true;
				btnHomeView.Enabled = true;

				btnArrangeX.Enabled = true;
				btnArrangeY.Enabled = true;
				btnSpaceX.Enabled = true;
				btnSpaceY.Enabled = true;

				btnMiddleX.Enabled = true;
				btnMiddleY.Enabled = true;

			}
			else
			{
				btnZoomIn.Enabled = false;
				btnZoomOut.Enabled = false;
				btnFitView.Enabled = false;
				btnResetView.Enabled = false;
				btnScreenShot.Enabled = false;
				btnPrint.Enabled = false;
				btnSaveHome.Enabled = false;
				btnHomeView.Enabled = false;

				btnArrangeX.Enabled = false;
				btnArrangeY.Enabled = false;
				btnSpaceX.Enabled = false;
				btnSpaceY.Enabled = false;

				btnMiddleX.Enabled = false;
				btnMiddleY.Enabled = false;

			}

		}

		private void btnArrangeY_Click(object sender, EventArgs e)
		{
			PanelSectionEx panel = FormMain.Instance.GetPageLevel().GetCurrentPanel();
			if (panel != null)
			{
				ArrayList arSection = (ArrayList)panel.SelectedSectionList.Clone();
				Section selectedSsection =  panel.SelectedSection;
				if( selectedSsection != null && !arSection.Contains(selectedSsection))
				{
					arSection.Add(selectedSsection);
				}


				Section minSection = null;
				float mMinX = float.MaxValue;
				float mMinY = float.MaxValue;
				foreach (Section section in arSection)
				{
					if( section.Position.X< mMinX)
					{
						mMinX = section.Position.X;
						mMinY = section.Position.Y;
						minSection = section;
					}
				}
				if (minSection != null)
				{
					UndoRedoManager.Instance.SaveSnapshot("높이 맞추기");

					foreach (Section section in arSection)
					{
						PointF pt = new PointF(section.Position.X, mMinY);
						panel.SectionMove(section, pt);				
					}
					panel.Invalidate();
				}				
			}
		}

		private void btnArrangeX_Click(object sender, EventArgs e)
		{
			PanelSectionEx panel = FormMain.Instance.GetPageLevel().GetCurrentPanel();
			if (panel != null)
			{
				ArrayList arSection = (ArrayList)panel.SelectedSectionList.Clone();
				Section selectedSsection = panel.SelectedSection;
				if (selectedSsection != null && !arSection.Contains(selectedSsection))
				{
					arSection.Add(selectedSsection);
				}


				Section minSection = null;
				float mMinX = float.MaxValue;
				float mMinY = float.MaxValue;
				foreach (Section section in arSection)
				{
					if (section.Position.Y < mMinY)
					{
						mMinX = section.Position.X;
						mMinY = section.Position.Y;
						minSection = section;
					}
				}
				if (minSection != null)
				{
					UndoRedoManager.Instance.SaveSnapshot("위치 맞추기");

					foreach (Section section in arSection)
					{
						PointF pt = new PointF(mMinX, section.Position.Y);	
						panel.SectionMove(section, pt);
											
					}
					panel.Invalidate();
				}
			}
		}

		internal class CompareSectionX : IComparer
		{
			int IComparer.Compare(object x, object y)
			{

				Section section1 = (Section)x;
				Section section2 = (Section)y;

				if (section1.Position.X >= section2.Position.X)
					return 1;
				return -1;
			}
		}

		internal class CompareSectionY : IComparer
		{
			int IComparer.Compare(object x, object y)
			{

				Section section1 = (Section)x;
				Section section2 = (Section)y;

				if (section1.Position.Y >= section2.Position.Y)
					return 1;
				return -1;
			}
		}

		private void btnSpaceX_Click(object sender, EventArgs e)
		{
			PanelSectionEx panel = FormMain.Instance.GetPageLevel().GetCurrentPanel();
			if (panel != null)
			{
				ArrayList arSection = (ArrayList)panel.SelectedSectionList.Clone();
				Section selectedSsection = panel.SelectedSection;
				if (selectedSsection != null && !arSection.Contains(selectedSsection))
				{
					arSection.Add(selectedSsection);
				}

				int nCount = arSection.Count;
				if (nCount <= 1)
				{
					return;
				}

				try
				{
					arSection.Sort(new CompareSectionX());
				}
				catch (Exception)
				{
				}

				Section startSection = (Section)arSection[0];
				Section endSection = (Section)arSection[nCount - 1];

				if( startSection != null && endSection != null)
				{
					float totalLength = endSection.Position.X - startSection.Position.X;
					float fDiv = totalLength / (float)(nCount - 1);
					float fStart = startSection.Position.X;
					float fValue = fStart;

					UndoRedoManager.Instance.SaveSnapshot("가로 간격 맞추기");

					foreach (Section section in arSection)
					{
						float fY = section.Position.Y;
						PointF pt = new PointF(fValue, fY);
						panel.SectionMove(section, pt);
						fValue += fDiv;
					}
					panel.Invalidate();
				}
			}				
		}

		private void btnSpaceY_Click(object sender, EventArgs e)
		{
			PanelSectionEx panel = FormMain.Instance.GetPageLevel().GetCurrentPanel();
			if (panel != null)
			{
				ArrayList arSection = (ArrayList)panel.SelectedSectionList.Clone();
				Section selectedSsection = panel.SelectedSection;
				if (selectedSsection != null && !arSection.Contains(selectedSsection))
				{
					arSection.Add(selectedSsection);
				}

				int nCount = arSection.Count;
				if (nCount <= 1)
				{
					return;
				}
				
				try
				{
					arSection.Sort(new CompareSectionY());
				}
				catch(Exception)
				{
				}
				

				Section startSection = (Section)arSection[0];
				Section endSection = (Section)arSection[nCount - 1];
                

				if(startSection != null && endSection != null)
				{
					float totalLength = endSection.Position.Y - startSection.Position.Y;
                    float totalHeight = 0.0f;
                    foreach (Section section in arSection)
                    {
                        if( section != endSection)
                            totalHeight += section.RectSize.Height;
                    }

                    totalLength -= totalHeight;

					float fDiv = totalLength / (float)(nCount - 1);

					float fStart = startSection.Position.Y;
					float fValue = fStart;

					UndoRedoManager.Instance.SaveSnapshot("세로 간격 맞추기");

					foreach (Section section in arSection)
					{
						float fX = section.Position.X;						
						PointF pt = new PointF(fX, fValue);
						panel.SectionMove(section, pt);
                        
                        fValue += section.RectSize.Height;
						fValue += fDiv;
					}
					panel.Invalidate();
				}
			}
		}

		private void btnMiddleX_Click(object sender, EventArgs e)
		{
			// 가로 중심선 맞추기
			PanelSectionEx panel = FormMain.Instance.GetPageLevel().GetCurrentPanel();
			if (panel != null)
			{
				ArrayList arSection = (ArrayList)panel.SelectedSectionList.Clone();
				Section selectedSsection = panel.SelectedSection;
				if (selectedSsection != null && !arSection.Contains(selectedSsection))
				{
					arSection.Add(selectedSsection);
				}


				Section minSection = null;
				float mMinX = float.MaxValue;
				float mMinY = float.MaxValue;
				foreach (Section section in arSection)
				{
					if (section.Position.X < mMinX)
					{
						mMinX = section.Position.X;
						mMinY = section.Position.Y;
						minSection = section;
					}
				}
				if (minSection != null)
				{
					float halfAdjust = minSection.RectSize.Height / 2.0f;


					UndoRedoManager.Instance.SaveSnapshot("높이 맞추기");

					foreach (Section section in arSection)
					{

						float halfAdjustX = halfAdjust - section.RectSize.Height / 2.0f;

						PointF pt = new PointF(section.Position.X, mMinY + halfAdjustX);
						panel.SectionMove(section, pt);
					}
					panel.Invalidate();
				}
			}
		}

		private void btnMiddleY_Click(object sender, EventArgs e)
		{

			// 세로 중심선 맞추기
			PanelSectionEx panel = FormMain.Instance.GetPageLevel().GetCurrentPanel();
			if (panel != null)
			{
				ArrayList arSection = (ArrayList)panel.SelectedSectionList.Clone();
				Section selectedSsection = panel.SelectedSection;
				if (selectedSsection != null && !arSection.Contains(selectedSsection))
				{
					arSection.Add(selectedSsection);
				}


				Section minSection = null;
				float mMinX = float.MaxValue;
				float mMinY = float.MaxValue;
				foreach (Section section in arSection)
				{
					if (section.Position.Y < mMinY)
					{
						mMinX = section.Position.X;
						mMinY = section.Position.Y;
						minSection = section;
					}
				}


				if (minSection != null)
				{
					float halfAdjust = minSection.RectSize.Width / 2.0f;

					UndoRedoManager.Instance.SaveSnapshot("위치 맞추기");

					foreach (Section section in arSection)
					{
						float halfAdjustX = halfAdjust - section.RectSize.Width / 2.0f;

						PointF pt = new PointF(mMinX + halfAdjustX, section.Position.Y);
						panel.SectionMove(section, pt);

					}
					panel.Invalidate();
				}
			}
		}

        private void FormToolBox_Load(object sender, EventArgs e)
        {

        }

        private void FormToolBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmrUpdateCmd.Stop();
            tmrUpdateCmd.Enabled = false;
        }
	}
}
