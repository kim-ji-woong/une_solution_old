using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPMonitoringSystem
{
    public partial class PopupProgressReport : Form
    {
        private Image m_imgPlay = null, m_imgCancel = null, m_imgComplete = null;

        public PopupProgressReport()
        {
            InitializeComponent();
        }

        private void InitImage()
        {
            //// 실행, 재실행
            //Bitmap bmpPlay = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.report_red);
            //// 취소
            //Bitmap bmpCancel = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.report_blue);
            ////완료
            //Bitmap bmpComplete = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.report_gray);

            //ImageList imgListPlay = new ImageList();
            //imgListPlay.ImageSize = new Size(16, 16);
            //imgListPlay.Images.AddStrip(bmpPlay);

            //ImageList imgListCancel = new ImageList();
            //imgListCancel.ImageSize = new Size(16, 16);
            //imgListCancel.Images.AddStrip(bmpCancel);

            //ImageList imgListComplete = new ImageList();
            //imgListComplete.ImageSize = new Size(16, 16);
            //imgListComplete.Images.AddStrip(bmpComplete);

            ////현재 동작이 무엇인지  
            ////재난카테고리 가져오기
            //int nType = 0;

            //m_imgPlay = imgListPlay.Images[nType];
            //m_imgCancel = imgListCancel.Images[nType];
            //m_imgComplete = imgListComplete.Images[nType];
            
        }
        
        public void AddProgressReport()
        {
            //DataGridViewRow gridRow = new DataGridViewRow();
            //DataGridViewCell cell = new DataGridViewImageCell();
            //cell.Value = "";
            //gridRow.Cells.Add(cell);

            //cell = new DataGridViewTextBoxCell();
            //cell.Value = "";
            //gridRow.Cells.Add(cell);

            //cell = new DataGridViewImageCell();
            //cell.Value = "";
            //gridRow.Cells.Add(cell);

            //dataGridViewReport.Rows.Add(gridRow);
        }
    }
}
