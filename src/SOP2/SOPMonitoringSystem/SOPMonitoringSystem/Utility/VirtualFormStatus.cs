using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPMonitoringSystem
{
    public class FormStatus
    {
        private Label m_labelMode = null;
        private PictureBox m_pictureBoxStatus = null;
        private Label m_labelStatus = null;

        public FormStatus(Label labelMode, PictureBox pictureBoxStatus, Label labelStatus)
        {
            m_labelMode = labelMode;
            m_pictureBoxStatus = pictureBoxStatus;
            m_labelStatus = labelStatus;
        }

        public void RealMode(bool isRealMode)
        {
            if (isRealMode)
                m_labelMode.Text = "실제모드";
            else
                m_labelMode.Text = "훈련모드";
        }

        public void StatusBoard(Sections.WorkFlowState state)
        {
            switch (state)
            {
                case Sections.WorkFlowState.STANDBY:
                case Sections.WorkFlowState.WAIT:
                    m_pictureBoxStatus.Image = global::SOPMonitoringSystem.Properties.Resources.Wait_Status;
                    m_labelStatus.Text = "대기";
                    break;
                case Sections.WorkFlowState.RUN:
                    m_pictureBoxStatus.Image = global::SOPMonitoringSystem.Properties.Resources.Run_Status;
                    m_labelStatus.Text = "실행";
                    break;
                case Sections.WorkFlowState.STOP:
                    m_pictureBoxStatus.Image = global::SOPMonitoringSystem.Properties.Resources.Stop_Status;
                    m_labelStatus.Text = "정지";
                    break;
                case Sections.WorkFlowState.DONE:
                    m_pictureBoxStatus.Image = global::SOPMonitoringSystem.Properties.Resources.Complete_Status;
                    m_labelStatus.Text = "완료";
                    break;
            }
        }
    }
}
