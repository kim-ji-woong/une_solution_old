using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShapeFileReader.Attrib
{
    public partial class FormShapeAttrib : Form
    {
        private Drawing.PointShape m_shape = null;

        public FormShapeAttrib()
        {
            InitializeComponent();
        }

        public void SetShape(Drawing.PointShape shape)
        {
            if (m_shape == shape)
                return;

            dataGridView1.Rows.Clear();

            m_shape = shape;

            this.Text = "Shape ID : " + m_shape.ID.ToString();
            libShapeFile.ShapeInfo shapeInfo = m_shape.ShapeInfo;

            if (shapeInfo == null || m_shape.ID < 0)
                return;

            int nFieldCount = shapeInfo.GetFieldCount();

            for (int i = 0; i < nFieldCount; i++)
            {
                string strFieldName = shapeInfo.GetFieldName(i);
                string strFieldData = shapeInfo.GetFieldData(m_shape.ID, i);

                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = strFieldName;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = strFieldData;
                row.Cells.Add(cell);

                dataGridView1.Rows.Add(row);
            }
        }
    }
}
