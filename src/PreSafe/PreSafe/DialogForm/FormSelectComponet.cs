using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PreSafe
{
    internal partial class FormSelectComponet : Form
    {
        private PointF[] m_arrDragDropShapeOrigin = null;
        
        public FormSelectComponet()
        {
            InitializeComponent();
            SetComponent();
        }

        object[] m_arComponent = 
		{
			"프로세스", global::PreSafe.Properties.Resources.btnComponent_process,
			"판단", global::PreSafe.Properties.Resources.btnComponent_dicision,
			"설명", global::PreSafe.Properties.Resources.btnComponent_annotation,
			"시작/끝", global::PreSafe.Properties.Resources.btnComponent_endpoint,
			//"링크", global::PreSafe.Properties.Resources.btnComponent_link,
			//"다른 SOP로 전환", global::PreSafe.Properties.Resources.btnComponent_transSOP,
			//"내부 상황전파", global::PreSafe.Properties.Resources.btnComponent_internal,
			//"외부 상황전파", global::PreSafe.Properties.Resources.btnComponent_external,
			//"상황전파", global::PreSafe.Properties.Resources.btnComponent_transmission
		};

        private Image SetComponentmage(string strValue)
        {
            for (int i = 0; i < m_arComponent.Length; i += 2)
            {
                if (strValue == (string)m_arComponent[i] || strValue.Contains((string)m_arComponent[i]))
                    return (Image)m_arComponent[i + 1];
            }
            return null;
        }

        private void SetComponent()
        {
            dataGridComponent.Height = 441;

            //string[] strRowsValue1 = new string[] { "프로세스", "판단", "설명", "시작/끝", "링크", "다른 SOP로 전환", "내부 상황전파", "외부 상황전파", "상황전파" };
            string[] strRowsValue1 = new string[] { "프로세스", "판단", "설명", "시작/끝" };
            int nGridViewHeight = dataGridComponent.Height / 9;

            dataGridComponent.ColumnCount = 2;            
            for (int i = 0; i < strRowsValue1.Length; i++)
            {
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewImageCell imgCell = new DataGridViewImageCell(); 
                imgCell.Value = SetComponentmage(strRowsValue1[i]);               
                imgCell.Tag = strRowsValue1[i]; 

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();   
                cell.Value = strRowsValue1[i];
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;

                row.Cells.Add(imgCell);
                row.Cells.Add(cell);
                row.Height = nGridViewHeight;

                dataGridComponent.Rows.Add(row);
            }
            dataGridComponent.ClearSelection();
        }
        
		public void ClearSelection()
		{
			dataGridComponent.ClearSelection();
		}

        private void dataGridComponent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int nRowIndex = e.RowIndex;

            FormContent contentForm = FormMain.Instance.ContentForm;
            
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
            else if (nRowIndex == 4)
            {
                m_arrDragDropShapeOrigin = Sections.SectionLink.GetDefaultShape();
                type = Sections.Section.ComponentType.LINK;
            }
            else if (nRowIndex == 5)
            {
                m_arrDragDropShapeOrigin = Sections.SectionTransSOP.GetDefaultShape();
                type = Sections.Section.ComponentType.TRANSSOP;
            }
            else if (nRowIndex == 6)
            {
                m_arrDragDropShapeOrigin = Sections.SectionInternal.GetDefaultShape();
                type = Sections.Section.ComponentType.INTERNAL;
            }
            else if (nRowIndex == 7)
            {
                m_arrDragDropShapeOrigin = Sections.SectionExternal.GetDefaultShape();
                type = Sections.Section.ComponentType.EXTERNAL;
            }
            else if (nRowIndex == 8)
            {
                m_arrDragDropShapeOrigin = Sections.SectionTransmission.GetDefaultShape();
                type = Sections.Section.ComponentType.TRANSMISSION;
            }
            else
                m_arrDragDropShapeOrigin = null;

            contentForm.SetDragDropShape(m_arrDragDropShapeOrigin, type);
            Invalidate();
        }

        private void FormSelectComponent_Load(object sender, EventArgs e)
        {
            ClearSelection();
        }


        public bool SetEnabled
        {
            get
            {
                return this.dataGridComponent.Enabled;
            }
            set
            {
                dataGridComponent.Enabled = value;
            }
        }
		
    }
}
