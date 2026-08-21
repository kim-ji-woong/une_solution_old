using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace SOPManager
{
	public partial class BarComponent : Form
	{
		private PointF[] m_arrDragDropShapeOrigin = null;
        double fComponentFontSize;
        int nGridViewHeight;
        
		public BarComponent()
		{
			InitializeComponent();
            clsDoubleBuffer.SetDoubleBuffer(dataGridComponent,true);

			InitPage();

            fComponentFontSize = 10f;
            dataGridComponent.Height = 441;
            nGridViewHeight = 40;

            SetComponent();
		}

        private string[] strRowsValue1 = new string[] { "프로세스", "판단", "설명", "시작/끝", /*"링크",*/ /*"다른 SOP로 전환",*/ "상황전파"/*, "외부 상황전파" , "상황전파" */};
		private object[] m_arComponent =
		{
			"프로세스", global::SOPManager.Properties.Resources.__COMPONENT_Process,global::SOPManager.Properties.Resources.__COMPONENT_ProcessClick,
			"판단", global::SOPManager.Properties.Resources.__COMPONENT_Dicision,global::SOPManager.Properties.Resources.__COMPONENT_DicisionClick,
			"설명", global::SOPManager.Properties.Resources.__COMPONENT_Annotation,global::SOPManager.Properties.Resources.__COMPONENT_AnnotationClick,
			"시작/끝", global::SOPManager.Properties.Resources.__COMPONENT_BeginEnd,global::SOPManager.Properties.Resources.__COMPONENT_BeginEndClick,
            //"링크", global::SOPManager.Properties.Resources.btnComponent_link,
			//"다른 SOP로 전환", global::SOPManager.Properties.Resources.__COMPONENT_TransSOP,
			"상황전파", global::SOPManager.Properties.Resources.__COMPONENT_Internal,global::SOPManager.Properties.Resources.__COMPONENT_InternalClick,
			//"외부 상황전파", global::SOPManager.Properties.Resources.btnComponent_external,
			//"상황전파", global::SOPManager.Properties.Resources.btnComponent_transmission
		};

		private Image SetComponentmage(string strValue,Boolean bSelect = false)
		{
            for (int i = 0; i < m_arComponent.Length; i += 3)
            {
                if (strValue == (string)m_arComponent[i] || strValue.Contains((string)m_arComponent[i]))
                {
                    Bitmap _bmp = (Bitmap)m_arComponent[i + 1];
                    Bitmap _tmp = new Bitmap(_bmp);

                    Bitmap _bmpClick = (Bitmap)m_arComponent[i + 2];
                    Bitmap _tmpClick = new Bitmap(_bmpClick);

                    m_arComponent[i + 1] = _tmp;
                    m_arComponent[i + 2] = _tmpClick;

                    if (bSelect == false)
                        return _tmp;
                    else
                        return _tmpClick;
                }
            }
            return null;
		}
        
        public void event_WinRateChanged()
        {
            //this.Parent.Size = new System.Drawing.Size((int)((float)this.Parent.Size.Width * FormMain.Instance.WindowWidthRate) 
            //                                                             , (int)((float)this.Parent.Size.Height * FormMain.Instance.WindowHeightRate));

            double fLabelFontSize = label1.Font.Size * FormMain.Instance.WindowWidthRate;

            label1.Font = new Font(Program.prgFont, (float)fLabelFontSize, FontStyle.Bold);

            panelTop.Size = new Size(panelTop.Size.Width, (int)((float)panelTop.Size.Height * FormMain.Instance.WindowHeightRate));

            //dataGridComponent.Columns[0].Width = (int)((float)dataGridComponent.Columns[0].Width * FormMain.Instance.WindowWidthRate);                        
            dataGridComponent.Columns[0].Width = dataGridComponent.Width;

            dataGridComponent.Rows.Clear();

            SetComponent();
        }

		private void SetComponent()
		{
            //int nGridViewHeight = dataGridComponent.Height / strRowsValue1.Length;
            nGridViewHeight = (int)((double)nGridViewHeight * FormMain.Instance.WindowHeightRate);
            fComponentFontSize = fComponentFontSize * FormMain.Instance.WindowWidthRate;

			//dataGridComponent.ColumnCount = 2;

            for (int i = 0; i < strRowsValue1.Length; i++)
			{
				DataGridViewRow row = new DataGridViewRow();                

				DataGridViewImageCell imgCell = new DataGridViewImageCell();                
                Image _img = SetComponentmage(strRowsValue1[i]);                
                imgCell.Value = _img;
				imgCell.Tag = strRowsValue1[i];
				imgCell.ToolTipText = strRowsValue1[i];
                imgCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                imgCell.ImageLayout = DataGridViewImageCellLayout.Zoom;
                row.Cells.Add(imgCell);                

                //DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                //cell.Value = strRowsValue1[i];
                //cell.ToolTipText = strRowsValue1[i];
                //cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                //cell.Style.ForeColor = Color.White;
                //cell.Style.Font = new System.Drawing.Font(Program.prgFont, (float)fComponentFontSize, FontStyle.Bold);
                
                //row.Cells.Add(cell);

                row.DefaultCellStyle.BackColor = Color.FromArgb(43, 43, 43);
                row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(247, 169, 43);

                row.Height =_img.Height / 2;                

				dataGridComponent.Rows.Add(row);
			}            

			dataGridComponent.ClearSelection();
		}

        private void SetSubCategoryRow(int nCategoryID) { }

        private void InitPage() { }

        private void BarComponent_MouseMove(object sender, MouseEventArgs e) { }		

		public void ClearSelection(){ dataGridComponent.ClearSelection(); }

        private void dataGridComponent_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            for (int index = 0; index < strRowsValue1.Length; index++)
            {
                if (index != e.RowIndex)
                {
                    dataGridComponent.Rows[index].Cells[0].Value = m_arComponent[(index * 3) + 1];
                    dataGridComponent.Rows[index].DefaultCellStyle.BackColor = Color.FromArgb(43, 43, 43);
                }
                else
                {
                    dataGridComponent.Rows[index].Cells[0].Value = m_arComponent[(index * 3) + 2];
                    dataGridComponent.Rows[index].DefaultCellStyle.BackColor = Color.FromArgb(247, 169, 43);
                }
            }
        }

		private void dataGridComponent_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			int nRowIndex = e.RowIndex;           

			FormPageSOP pageLevel = FormMain.Instance.GetPageLevel();

			Sections.Section.ComponentType type = Sections.Section.ComponentType.NONE;

			if (nRowIndex == 0)
			{
				m_arrDragDropShapeOrigin = Sections.SectionProcess.GetDefaultShape();
				type = Sections.Section.ComponentType.PROCESS;
			}
			else if (nRowIndex == 1)
			{
				m_arrDragDropShapeOrigin = Sections.SectionDecision.GetDefaultShape();
				type = Sections.Section.ComponentType.DECISION;
			}
			else if (nRowIndex == 2)
			{
				m_arrDragDropShapeOrigin = Sections.SectionAnnotation.GetDefaultShape();
				type = Sections.Section.ComponentType.ANNOTATION;
			}
			else if (nRowIndex == 3)
			{
				m_arrDragDropShapeOrigin = Sections.SectionEndPoint.GetDefaultShape();
				type = Sections.Section.ComponentType.ENDPOINT;
			}
            //else if (nRowIndex == 4)
            //{
            //    m_arrDragDropShapeOrigin = Sections.SectionLink.GetDefaultShape();
            //    type = Sections.Section.ComponentType.LINK;
            //}
            //else if (nRowIndex == 4)
            //{
            //    m_arrDragDropShapeOrigin = Sections.SectionTransSOP.GetDefaultShape();
            //    type = Sections.Section.ComponentType.TRANSSOP;
            //}
			else if (nRowIndex == 4)
			{
				m_arrDragDropShapeOrigin = Sections.SectionInternal.GetDefaultShape();
				type = Sections.Section.ComponentType.INTERNAL;

				//m_arrDragDropShapeOrigin = Sections.SectionGroup.GetDefaultShape();
				//type = Sections.Section.ComponentType.GROUP;
			}
            //else if (nRowIndex == 6)
            //{
            //    m_arrDragDropShapeOrigin = Sections.SectionExternal.GetDefaultShape();
            //    type = Sections.Section.ComponentType.EXTERNAL;
            //}
            //else if (nRowIndex == 7)
            //{
            //    m_arrDragDropShapeOrigin = Sections.SectionTransmission.GetDefaultShape();
            //    type = Sections.Section.ComponentType.TRANSMISSION;
            //}
			else
				m_arrDragDropShapeOrigin = null;

			if (nRowIndex > 0 && nRowIndex <= 6)
			{
				string strValue = strRowsValue1[nRowIndex];
				FormMain.Instance.SetStatusText("새 컴포넌트 추가 : " + strValue);
			}

			pageLevel.SetDragDropShape(m_arrDragDropShapeOrigin, type);
            Invalidate();
		}

		private void BarComponent_Load(object sender, EventArgs e)
		{
			ClearSelection();
		}
	}

    public static class clsDoubleBuffer
    {
        public static void SetDoubleBuffer(this Control contorl, bool setting)
        {
            Type dgvType = contorl.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(contorl, setting, null);
        }
    }
}