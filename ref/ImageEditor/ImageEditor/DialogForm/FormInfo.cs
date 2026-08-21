using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageEditor
{
    public partial class FormInfo : Form
    {
        private Size m_szPanel;
        public Size PanelSize
        {
            get { return m_szPanel; }
            set { m_szPanel = value; }
        }

        public FormInfo()
        {
            InitializeComponent();
            InitDataGridView();
        }

        private void FormInfo_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private Color m_LineColor;
        public Color LineColor
        {
            get { return m_LineColor; }
            set { m_LineColor = value; }
        }

        private void InitDataGridView()
        {
            //이미지 정보
            ImageInfoGrid.ClearSelection();
            ImageInfoGrid.Rows.Clear();

            ImageInfoGrid.ColumnCount = 2;

            string[] arrStrValue = { "가로", "세로", "파일" };
            
            for(int i=0; i<arrStrValue.Length; i++)
            {
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewCell cell = null;

                cell = new DataGridViewTextBoxCell();
                cell.Value = arrStrValue[i];
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "";
                row.Cells.Add(cell);

                ImageInfoGrid.Rows.Add(row);
            }

            //선택 정보
            SelectInfoGrid.ClearSelection();
            SelectInfoGrid.Rows.Clear();

            SelectInfoGrid.ColumnCount = 2;

            string[] arrStrValue2 = {"X", "Y", "가로", "세로", "색상" };

            for (int i = 0; i < arrStrValue2.Length; i++)
            {
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewCell cell = null;

                cell = new DataGridViewTextBoxCell();
                cell.Value = arrStrValue2[i];
                
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "";
                row.Cells.Add(cell);

                SelectInfoGrid.Rows.Add(row);

            }

            //가운데정렬
            ImageInfoGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ImageInfoGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            SelectInfoGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            SelectInfoGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            SetInfoGridColor(Color.Black);

            ImageInfoGrid.DefaultCellStyle.Font = new Font("맑은 고딕", 9);
            SelectInfoGrid.DefaultCellStyle.Font = new Font("맑은 고딕", 9);

        


            
        }
        
        public void SetImageGrid(int nWidth, int nHeight, string strFileName)
        {
            ImageInfoGrid.Rows[0].Cells[1].Value = nWidth;
            ImageInfoGrid.Rows[1].Cells[1].Value = nHeight;
            ImageInfoGrid.Rows[2].Cells[1].Value = strFileName;

            m_szPanel = new Size(nWidth, nHeight);
        }

        public void SetImageGrid(int nWidth, int nHeight)
        {
            ImageInfoGrid.Rows[0].Cells[1].Value = nWidth;
            ImageInfoGrid.Rows[1].Cells[1].Value = nHeight;

            m_szPanel = new Size(nWidth, nHeight);
        }

        public void SetImageGridNmae(string strFileName)
        {
            ImageInfoGrid.Rows[2].Cells[1].Value = strFileName;
        }

        public void SetInfoGridColor(Color color)
        {
            DataGridViewCellStyle cs = SelectInfoGrid.DefaultCellStyle.Clone();
            cs.BackColor = color;

            SelectInfoGrid.Rows[4].Cells[1].Style = cs;

            m_LineColor = color;
        }
        public void SetSelectInfoGrid(int x, int y, int width, int height)
        {
            SelectInfoGrid.Rows[0].Cells[1].Value = x;
            SelectInfoGrid.Rows[1].Cells[1].Value = y;
            SelectInfoGrid.Rows[2].Cells[1].Value = width;
            SelectInfoGrid.Rows[3].Cells[1].Value = height;
        }

        private void SelectInfoGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //색상선택
            if (e.RowIndex == 4 && e.ColumnIndex == 1)
            {
                ColorDialog colorDialog = new ColorDialog();
                colorDialog.AllowFullOpen = true;
                colorDialog.ShowHelp = true;
                colorDialog.AnyColor = true;

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    SetInfoGridColor(colorDialog.Color);
                    SelectInfoGrid.ClearSelection();

                    m_LineColor = colorDialog.Color;
                }
            }
        }
    }
}
