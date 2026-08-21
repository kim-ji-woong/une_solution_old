using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace FireManagement
{
    public partial class Form1 : Form
    {
        private FireEquipment m_selectedEquipment = null;
        private FormEquipList frmEquipList = null;

        private DXFViewer.ShapeGroup shapeGroup = null;

        public Form1(DXFViewer.ShapeGroup shapeGroup)
        {
            InitializeComponent();

            frmEquipList = FormMain2.Instance.ViewControl.LeftBar;

            this.shapeGroup = shapeGroup;
            SetEquipment(shapeGroup);
        }

        private void SetEquipment(DXFViewer.ShapeGroup shapeGroup)
        {
            for (int i = 0; i < shapeGroup.GetShapeCount(); i++)
            {
                DXFViewer.Shape shape = shapeGroup.GetShape(i);

                FireEquipment equip = FormMain2.Instance.ViewControl.LeftBar.FindEquipment(shape);

                DataGridViewRow row = new DataGridViewRow();
                row.Height = 30;

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = " " + FireEquipment.GetTypeName(equip.Type);
                row.Cells.Add(cell);


                cell = new DataGridViewTextBoxCell();
                cell.Value = " " + equip.EquipID;
                row.Cells.Add(cell);

                ArrayList arr = new ArrayList();
                arr.Add(equip);
                arr.Add(shape);

                row.Tag = arr;
                dataGridGroup.Rows.Add(row);

                if (equip.LinkedShape == null)
                    FormMain2.Instance.DXFManager.AddEquipmentObjectToDXF(equip);
                else
                {
                    DXFViewer.Layer layer = FormMain2.Instance.GetEquipmentLayer(equip.Type);

                    if (layer != null && !layer.Shapes.Contains(equip.LinkedShape))
                        layer.Add(equip.LinkedShape);
                }

                //if (equip.LinkedShape != null)
                //    dicShape[equip.LinkedShape] = row;
            }
        }

        private void dataGridGroup_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            frmEquipList.ClearSelection();

            DataGridView grid = (DataGridView)sender;
            ArrayList arr = (ArrayList)grid.Rows[e.RowIndex].Tag;
            FireEquipment equip = (FireEquipment)arr[0];
            DXFViewer.Shape shape = (DXFViewer.Shape)arr[1];

            grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;

            if (equip.LinkedShape != null)
                equip.LinkedShape.Selected = true;

            if (m_selectedEquipment == equip)
                return;
            else
            {
                if (m_selectedEquipment != null && m_selectedEquipment.LinkedShape != null)
                    m_selectedEquipment.LinkedShape.Selected = false;

                m_selectedEquipment = equip;

                frmEquipList.SelectShape(shape);

                EventManager.Instance.ProcessEvent(Event.EQUIP_SELECTED, m_selectedEquipment);
                FormMain2.Instance.DXFControl.Refresh();
            }
        }
    }
}
