using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPMonitoringSystem
{
    public partial class FormRightProgress : Form
    {
        private FormMain m_Main = null;

        public FormRightProgress(FormMain main)
        {
            InitializeComponent();

            m_Main = main;

            InitProgress();
            InitMission();
        }

        private void InitProgress()
        {
            ArrayList arrProgress = new ArrayList();
            arrProgress.Add("현재 시간");
            arrProgress.Add("최초 발동 시간");
            arrProgress.Add("경과 시간");

            foreach (string strValue in arrProgress)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = null;

                cell = new DataGridViewTextBoxCell();
                cell.Value = strValue;
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "00:00:00";
                gridRow.Cells.Add(cell);

                dataGridProgress.Rows.Add(gridRow);
            }
        }

        private void InitMission()
        {
            ArrayList arrMission = new ArrayList();
            arrMission.Add("진행률");
            arrMission.Add("전체 임무 수");
            arrMission.Add("수행된 임무 수");
            arrMission.Add("진행 중 임무 수");
            arrMission.Add("남은 임무 수");

            foreach (string strValue in arrMission)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = null;

                cell = new DataGridViewTextBoxCell();
                cell.Value = strValue;
                gridRow.Cells.Add(cell);

                if (dataGridSOPMission.RowCount == 0)
                {
                    cell = new DataGridViewTextBoxCell();
                    cell.Value = "00 %";
                    gridRow.Cells.Add(cell);
                }
                else
                {
                    cell = new DataGridViewTextBoxCell();
                    cell.Value = "000 개";
                    gridRow.Cells.Add(cell);
                }

                dataGridSOPMission.Rows.Add(gridRow);
            }
        }

        public void SetMissionInfo(int nTotalMissionCount, int nProcessedMissionCount, int nProcessingMissionCount)
        {
            if (nTotalMissionCount == 0)
            {
                dataGridSOPMission.Rows[0].Cells[1].Value = "00 %";
                dataGridSOPMission.Rows[1].Cells[1].Value = "000 개";
                dataGridSOPMission.Rows[2].Cells[1].Value = "000 개";
                dataGridSOPMission.Rows[3].Cells[1].Value = "000 개";
                dataGridSOPMission.Rows[4].Cells[1].Value = "000 개";
            }
            else
            {
                int nProcessedPercent = nProcessedMissionCount * 100 / nTotalMissionCount;
                int nRemainder = nTotalMissionCount - nProcessedMissionCount - nProcessingMissionCount;

                dataGridSOPMission.Rows[0].Cells[1].Value = string.Format("{0} %", nProcessedPercent);
                dataGridSOPMission.Rows[1].Cells[1].Value = string.Format("{0} 개", nTotalMissionCount);
                dataGridSOPMission.Rows[2].Cells[1].Value = string.Format("{0} 개", nProcessedMissionCount);
                dataGridSOPMission.Rows[3].Cells[1].Value = string.Format("{0} 개", nProcessingMissionCount);
                dataGridSOPMission.Rows[4].Cells[1].Value = string.Format("{0} 개", nRemainder);
            }
        }

        public void GetStartTime(DateTime date)
        {
            dataGridProgress.Rows[1].Cells[1].Value = date.ToString("HH:mm:ss");
        }

        public void GetElapsedTime(TimeSpan ts)
        {
            dataGridProgress.Rows[2].Cells[1].Value = ts.ToString();
        }
        public void SetCurrentTime(string strTime)
        {
            dataGridProgress.Rows[0].Cells[1].Value = strTime;
        }
    }
}
