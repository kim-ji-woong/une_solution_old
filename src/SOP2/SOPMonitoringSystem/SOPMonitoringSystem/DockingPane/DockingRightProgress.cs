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
    public partial class DockingRightProgress : Form
    {
        private DateTime m_dtBegin;

        public DockingRightProgress()
        {
            InitializeComponent();

            InitProgress();
            InitMission();
        }

        private void InitProgress()
        {
            ArrayList arrProgress = new ArrayList();
            arrProgress.Add(" 현재 시간");
            arrProgress.Add(" 최초 발동 시간");
            arrProgress.Add(" 경과 시간");

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
            arrMission.Add(" 진행률");
            arrMission.Add(" 전체 임무 수");
            arrMission.Add(" 수행된 임무 수");
            arrMission.Add(" 진행 중 임무 수");
            arrMission.Add(" 남은 임무 수");
            arrMission.Add(" 건너뛴 임무 수");

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

        public void SetMissionInfo(int nTotalMissionCount, int nProcessedMissionCount, int nProcessingMissionCount, int nSkippedMissionCount)
        {
            if (nTotalMissionCount == 0)
            {
                dataGridSOPMission.Rows[0].Cells[1].Value = "00 %";
                dataGridSOPMission.Rows[1].Cells[1].Value = "000 개";
                dataGridSOPMission.Rows[2].Cells[1].Value = "000 개";
                dataGridSOPMission.Rows[3].Cells[1].Value = "000 개";
                dataGridSOPMission.Rows[4].Cells[1].Value = "000 개";
                dataGridSOPMission.Rows[5].Cells[1].Value = "000 개";
            }
            else
            {
                if (nTotalMissionCount == 0)
                    return;

                int nProcessedPercent = nProcessedMissionCount * 100 / nTotalMissionCount;
                int nRemainder = nTotalMissionCount - nProcessedMissionCount - nProcessingMissionCount - nSkippedMissionCount;

                dataGridSOPMission.Rows[0].Cells[1].Value = string.Format("{0} %", nProcessedPercent);
                dataGridSOPMission.Rows[1].Cells[1].Value = string.Format("{0} 개", nTotalMissionCount);
                dataGridSOPMission.Rows[2].Cells[1].Value = string.Format("{0} 개", nProcessedMissionCount);
                dataGridSOPMission.Rows[3].Cells[1].Value = string.Format("{0} 개", nProcessingMissionCount);
                dataGridSOPMission.Rows[4].Cells[1].Value = string.Format("{0} 개", nRemainder);
                dataGridSOPMission.Rows[5].Cells[1].Value = string.Format("{0} 개", nSkippedMissionCount);
            }
        }

        /*public void GetStartTime(DateTime date)
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
        }*/

        public void SetStartTime(DateTime dtBegin)
        {
            m_dtBegin = dtBegin;
            dataGridProgress.Rows[1].Cells[1].Value = string.Format("{0:00}:{1:00}:{2:00}", dtBegin.ToShortDateString(), dtBegin.Hour, dtBegin.Minute, dtBegin.Second);
        }

        public void SetCurrentTime(DateTime dtCurrent, TimeInfo timeEnd = null, TimeInfo timeCancel = null)
        {
            TimeSpan sp = dtCurrent - m_dtBegin;

            if (timeEnd != null)
            {
                sp = timeEnd.m_time - m_dtBegin;
            }
            else if (timeCancel != null)
            {
                sp = timeCancel.m_time - m_dtBegin;
            }

            if (sp.Days > 0)
            {
                dataGridProgress.Rows[1].Cells[1].Value = string.Format("{0} {1:00}:{2:00}:{3:00}", m_dtBegin.ToShortDateString(), m_dtBegin.Hour, m_dtBegin.Minute, m_dtBegin.Second);
                dataGridProgress.Rows[0].Cells[1].Value = string.Format("{0} {1:00}:{2:00}:{3:00}", dtCurrent.ToShortDateString(), dtCurrent.Hour, dtCurrent.Minute, dtCurrent.Second);
                dataGridProgress.Rows[2].Cells[1].Value = string.Format("{0}일 {1:00}:{2:00}:{3:00}", sp.Days, sp.Hours, sp.Minutes, sp.Seconds);
            }
            else
            {
                dataGridProgress.Rows[1].Cells[1].Value = string.Format("{0:00}:{1:00}:{2:00}", m_dtBegin.Hour, m_dtBegin.Minute, m_dtBegin.Second);
                dataGridProgress.Rows[0].Cells[1].Value = string.Format("{0:00}:{1:00}:{2:00}", dtCurrent.Hour, dtCurrent.Minute, dtCurrent.Second);
                dataGridProgress.Rows[2].Cells[1].Value = string.Format("{0:00}:{1:00}:{2:00}", sp.Hours, sp.Minutes, sp.Seconds);
            }
        }

        public void Initialize(DateTime dtCurrent)
        {
            dataGridSOPMission.Rows[0].Cells[1].Value = "00 %";
            dataGridSOPMission.Rows[1].Cells[1].Value = "000 개";
            dataGridSOPMission.Rows[2].Cells[1].Value = "000 개";
            dataGridSOPMission.Rows[3].Cells[1].Value = "000 개";
            dataGridSOPMission.Rows[4].Cells[1].Value = "000 개";
            dataGridSOPMission.Rows[5].Cells[1].Value = "000 개";

            dataGridProgress.Rows[0].Cells[1].Value = string.Format("{0:00}:{1:00}:{2:00}", dtCurrent.Hour, dtCurrent.Minute, dtCurrent.Second);
            dataGridProgress.Rows[1].Cells[1].Value = "00:00:00";
            dataGridProgress.Rows[2].Cells[1].Value = "00:00:00";
        }
    }
}
