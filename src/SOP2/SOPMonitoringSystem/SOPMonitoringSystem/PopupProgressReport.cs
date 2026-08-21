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
        private Image m_imgRemove = null;

        public PopupProgressReport(FormMain frmParent)
        {
            InitializeComponent();
            InitRemoveImg();
        }

        private void InitRemoveImg()
        {
            Bitmap bmpRemove = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.report_remove);

            ImageList imgListRemove = new ImageList();
            imgListRemove.ImageSize = new Size(16, 16);
            imgListRemove.Images.AddStrip(bmpRemove);

            m_imgRemove = imgListRemove.Images[0];
        }

        private int GetDisasterType()
        {
            string strTitle = FormMain.Instance.GetPageHome().GetDockPropertiesLevel().GetTitle();
			string[] strDisaster = strTitle.Split((char)0x06);

            int nType = 0;

            if (strDisaster[0] == "자연재해")
                nType = 0;
            else if (strDisaster[0] == "화재")
                nType = 1;
            else if (strDisaster[0] == "유출사고")
                nType = 2;
            else if (strDisaster[0] == "테러")
                nType = 3;
            else if (strDisaster[0] == "인명구조 및 의료지원")
                nType = 4;
            else
                nType = 5;

            return nType;
        }

        private Image GetProgressImage(string strStatus, int nType)
        {
            Bitmap bmp = null;

            if (strStatus == "실행취소" || strStatus == "대기")
            {
                bmp = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.report_blue);
            }
            else if (strStatus == "실행 완료")
            {
                bmp = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.report_gray);
            }
            else // 실행, 재실행
            {
                bmp = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.report_red);
            }

            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(16, 16);
            imgList.Images.AddStrip(bmp);

            Image img = imgList.Images[nType];

            return img;
        }

        delegate void Ctrl_Invoke(string strStepMemberName, string strTeamList, string strComponentType, string strTask, string strStatus);

        public void AddProgressReport(string strStepMemberName, string strTeamList, string strComponentType, string strTask, string strStatus)
        {
            if (dataGridViewReport.InvokeRequired)
            {
                Ctrl_Invoke CI = new Ctrl_Invoke(AddProgressReport);

                if (!FormMain.Instance.CloseThread)
                    dataGridViewReport.Invoke(CI, strStepMemberName, strTeamList, strComponentType, strTask, strStatus);
            }
            else
            {
                if (strStatus == "건너뛰기") return;

                int nType = GetDisasterType();
                Image img = GetProgressImage(strStatus, nType);

                string strReport = "";
                if (strComponentType == "프로세스")
                {
                    strReport = strTeamList + strTask + " " + strStatus;
                }
                else
                {
                    strReport = strStepMemberName + strTask + " " + strStatus;
                }

                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewImageCell();
                cell.Value = img;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = strReport;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewImageCell();
                cell.Value = m_imgRemove;
                gridRow.Cells.Add(cell);

                dataGridViewReport.Rows.Add(gridRow);
            }
        }

        private void dataGridViewReport_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                dataGridViewReport.Rows.RemoveAt(e.RowIndex);
            }
        }

        private void PopupProgressReport_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormMain.Instance.InitReport();
        }
    }
}
