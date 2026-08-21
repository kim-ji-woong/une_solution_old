using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPBulletin
{
    public partial class DockingProgress : Form
    {
        public DockingProgress()
        {
            InitializeComponent();

            AddRowSOPProgress();
            DrawProgressBar(0);
        }

        private void AddRowSOPProgress()
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = null;

            for (int i = 0; i < dataGridView.ColumnCount; i++ )
            {
                cell = new DataGridViewTextBoxCell();
                cell.Value = "00";
                gridRow.Cells.Add(cell);
            }
            
            gridRow.Height = 50;
            dataGridView.Rows.Add(gridRow);
        }

        public void MissionCount(int nCell, int nCount)
        {
            dataGridView.Rows[0].Cells[nCell].Value = nCount.ToString();
        }

        public void DrawProgressBar(int nPercent)
        {
            progressBar.Value = nPercent;

            label4.Text = nPercent.ToString() + "%";
            label4.Location = new Point(label2.Location.X + progressBar.Width * nPercent / 100, label4.Location.Y);

            //progressBar.CreateGraphics().DrawString(nPercent.ToString() + "%", new Font("맑은 고딕", (float)10.25, FontStyle.Bold), Brushes.Red, 
            //                              new PointF(progressBar.Width / 2 - 10, progressBar.Height / 2 - 7));

            //using (Graphics gr = progressBar.CreateGraphics())
            //{
            //    gr.DrawString(nPercent.ToString() + "%", SystemFonts.DefaultFont, Brushes.White,
            //        new PointF(progressBar.Width / 2 - (gr.MeasureString(nPercent.ToString() + "%", SystemFonts.DefaultFont).Width / 2.0F),
            //            progressBar.Height / 2 - (gr.MeasureString(nPercent.ToString() + "%", SystemFonts.DefaultFont).Height / 2.0F)));
            //}

        }

        public void UpdateProcessInfo(ActionStepHistoryData data)
        {
            if (data == null)
                return;

            if (dataGridView.Rows.Count == 0)
                return;

            HistoryManager historyMgr = FormMain.Instance.HistoryManager;

            int nProcessingMissionCount = historyMgr.GetProcessingMissionCount(data.ActionStepID);
            int nSkippedMissionCount = historyMgr.GetSkippedMissionCount(data.ActionStepID);
            int nTotalMissionCount = historyMgr.GetTotalMissionCount(data.ActionStepID);
            int nCompletedMissionCount = historyMgr.GetCompletedMissionCount(data.ActionStepID);

            if (nTotalMissionCount < 0)
            {
                dataGridView.Rows[0].Cells[0].Value = "-";
                dataGridView.Rows[0].Cells[1].Value = "-";
                dataGridView.Rows[0].Cells[2].Value = "-";
                dataGridView.Rows[0].Cells[3].Value = "-";
                dataGridView.Rows[0].Cells[4].Value = "-";

                DrawProgressBar(0);
            }
            else
            {
                dataGridView.Rows[0].Cells[0].Value = nTotalMissionCount.ToString();
                dataGridView.Rows[0].Cells[1].Value = nCompletedMissionCount < 0 ? "-" : nCompletedMissionCount.ToString();
                dataGridView.Rows[0].Cells[2].Value = nProcessingMissionCount < 0 ? "-" : nProcessingMissionCount.ToString();
                dataGridView.Rows[0].Cells[4].Value = nSkippedMissionCount < 0 ? "-" : nSkippedMissionCount.ToString();

                if (nCompletedMissionCount >= 0 && nProcessingMissionCount >= 0 && nSkippedMissionCount >= 0)
                {
                    dataGridView.Rows[0].Cells[3].Value = (nTotalMissionCount - nCompletedMissionCount - nProcessingMissionCount - nSkippedMissionCount).ToString();

                    if (nTotalMissionCount == 0)
                        DrawProgressBar(0);
                    else
                        DrawProgressBar(nCompletedMissionCount * 100 / nTotalMissionCount);
                }
                else
                {
                    dataGridView.Rows[0].Cells[3].Value = "-";
                    DrawProgressBar(0);
                }
            }
        }

        private void DockingProgress_KeyDown(object sender, KeyEventArgs e)
        {
            FormMain.Instance.OnKeyDown(sender, e);
        }

        public void SetContextMenu(ContextMenuStrip menu)
        {
            this.ContextMenuStrip = menu;
        }
    }
}
