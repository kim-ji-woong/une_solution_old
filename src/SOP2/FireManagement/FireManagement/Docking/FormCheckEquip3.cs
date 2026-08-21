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
    public partial class FormCheckEquip3 : Form, Ubists.IReaderOwner
    {
        private bool m_isWorking = false;

        private FormEquipState m_formEquipState = null;


        public FormCheckEquip3()
        {
            InitializeComponent();
        }


        private void InitControl()
        {
            if (FormMain2.Instance.IsPCMode)
            {
                radioManual.Checked = true;
                radioRFID.Enabled = false;
            }
            else
                radioRFID.Checked = true;
        }

        public void Hide()
        {
            m_isWorking = false;
            FormMain2.Instance.ViewControl.SetRFIDOwner();
            //FormMain2.Instance.EnableEdit();
            base.Hide();
        }

        public void Show(FireEquipment equipSelected = null)
        {
            m_isWorking = true;

            if (radioRFID.Checked && !FormMain2.Instance.IsPCMode)
            {
                FormMain2.Instance.RFIDReader.Owner = this;
                if (!FormMain2.Instance.RFIDReader.StartReading())
                {
                    m_isWorking = false;
                    FormMain2.Instance.ViewControl.SetRFIDOwner();
                    //FormMain2.Instance.EnableEdit();
                    return;
                }
            }

            if (equipSelected != null)
                SetEquipment(equipSelected);

            base.Show();
        }

        private void btnEquipState_Click(object sender, EventArgs e)
        {
            if (m_formEquipState == null || m_formEquipState.IsDisposed)
            {
                m_formEquipState = new FormEquipState();

                m_formEquipState.StartPosition = FormStartPosition.Manual;
                Point pt = FormMain2.Instance.ViewControl.PointToScreen(new Point(FormMain2.Instance.ViewControl.PanelRightBar.Location.X - m_formEquipState.Size.Width -5
                    , FormMain2.Instance.ViewControl.PanelRightBar.Location.Y));

                m_formEquipState.Location = new Point(pt.X, pt.Y);
            }
            m_formEquipState.ShowDialog();
        }

        private void buttonComplete_Click(object sender, EventArgs e)
        {
            if (IsEmpty())
            {
                //Hide();
                return;
            }

            FireEquipment equip;
            FireEquipmentHistory history;

            if (MakeEquipmentHistory(out equip, out history))
            {
                LogManager.Instance.WriteCheckLog(equip, history);
                Reset();
               // Hide();
                //FormMain2.Instance.ViewControl.ButtonClose();
                MessageBox.Show("완료 되었습니다.");
            }
        }

        public void OnReadTag(string strTag)
        {
            if (textBoxRFID.Text == strTag)
                return;

            textBoxRFID.Text = strTag;

            FireEquipment equip = FormMain2.Instance.DXFManager.FindEquipment(strTag);

            if (equip != null)
            {
                LogManager.Instance.WriteCheckLog(equip);
                SetEquipment(equip);
            }
        }

        private FireEquipmentHistory GetLastHistory(FireEquipment equip)
        {
            DXFManager dxfMgr = FormMain2.Instance.DXFManager;

            if (dxfMgr.EquipmentHistory.ContainsKey(equip))
                return dxfMgr.EquipmentHistory[equip];

            if (equip.ID < 0)
                return null;

            IOManager ioMgr = FormMain2.Instance.IOManager;

            ArrayList arrEquipHistory = ioMgr.FindEquipmentHistoryList(equip.ID);
            if (arrEquipHistory == null)
                return null;

            int nHistoryCount = arrEquipHistory.Count;
            if (nHistoryCount == 0)
                return null;

            return (FireEquipmentHistory)arrEquipHistory[nHistoryCount - 1];
        }

        public void SetEquipment(FireEquipment equip)
        {
            textBoxRFID.Text = equip.RFIDTag;
            textBoxRFIDTagID.Text = equip.RFIDTagID;
            textBoxEquipID.Text = equip.EquipID;
            textBoxEquipType.Text = FireEquipment.GetTypeName(equip.Type);

            FireEquipmentHistory history = GetLastHistory(equip);

            btnEquipState.Tag = pictureBoxGroup;

            if (history != null)
            {
                int tmp = (int)history.Status;
                GetStatusText(tmp);
                
                //comboBoxStatus.SelectedIndex = (int)history.Status;
                textBoxOpinion.Text = history.CheckersOpinion;
                textBoxLastCheckedTime.Text = string.Format("{0} {1}:{2}:{3}", history.Time.ToShortDateString(), history.Time.Hour, history.Time.Minute, history.Time.Second);
            }
            else
            {
                pictureBoxGroup.Tag = (int)FireEquipmentHistory.EquipmentStatus.NORMAL;

                pictureBoxGroup.Text = "앙호";
                pictureBoxGroup.Refresh();
                
                //comboBoxStatus.SelectedIndex = (int)FireEquipmentHistory.EquipmentStatus.NORMAL;
                textBoxOpinion.Text = "";
                textBoxLastCheckedTime.Text = "";
            }
        }

        public void GetStatusText(int status)
        {
            int tmp = (int)status;

            string strHistoryStatus = "";
            switch (tmp)
            {
                case 0: strHistoryStatus = "앙호";
                    break;
                case 1: strHistoryStatus = "불량/고장";
                    break;
                case 2: strHistoryStatus = "수리중";
                    break;
                case 3: strHistoryStatus = "기타";
                    break;
            }

            pictureBoxGroup.Tag = tmp;
            pictureBoxGroup.Text = strHistoryStatus;
            pictureBoxGroup.Refresh();
        }
        
        private void radioRFID_CheckedChanged(object sender, EventArgs e)
        {
            if (m_isWorking && !FormMain2.Instance.IsPCMode)
            {
                FormMain2.Instance.RFIDReader.Owner = this;
                FormMain2.Instance.RFIDReader.StartReading();
            }
        }

        private void radioManual_CheckedChanged(object sender, EventArgs e)
        {

        }

        private bool IsEmpty()
        {
            if (textBoxEquipID.Text.Length > 0)
                return false;

            return true;
        }

        public void Reset()
        {
            textBoxRFID.Text = "";
            textBoxRFIDTagID.Text = "";
            textBoxEquipID.Text = "";
            textBoxEquipType.Text = "";
            textBoxOpinion.Text = "";
            textBoxLastCheckedTime.Text = "";
            pictureBoxGroup.Text = "";
            pictureBoxGroup.Refresh();
        }

        private bool MakeEquipmentHistory(out FireEquipment equip, out FireEquipmentHistory history)
        {
            string strRFID = textBoxRFID.Text;
            DXFManager dxfMgr = FormMain2.Instance.DXFManager;

            equip = null;
            history = null;

            if (strRFID.Length > 0)
            {
                equip = dxfMgr.FindEquipment(strRFID);
                if (equip == null)
                    return false;
            }
            else
            {
                string strEquipID = textBoxEquipID.Text;

                foreach (FireEquipment equipment in dxfMgr.Equipments)
                {
                    if (equipment.EquipID == strEquipID)
                    {
                        equip = equipment;
                        break;
                    }
                }

                if (equip == null)
                    return false;
            }

            return MakeEquipmentHistory(equip, out history);
        }

        private bool MakeEquipmentHistory(FireEquipment equip, out FireEquipmentHistory history)
        {
            Dictionary<FireEquipment, FireEquipmentHistory> dicEquipmentHistory = FormMain2.Instance.DXFManager.EquipmentHistory;
            history = null;

            if (dicEquipmentHistory.ContainsKey(equip))
            {
                history = dicEquipmentHistory[equip];

                // 시스템에 저장된 History라면 새로운 History를 생성한다.
                if (!history.IsNewHistory)
                {
                    history = new FireEquipmentHistory();
                    dicEquipmentHistory[equip] = history;
                }
            }
            else
            {
                history = new FireEquipmentHistory();
                dicEquipmentHistory[equip] = history;
            }

            history.EquipmentID = equip.ID;
            history.CheckersOpinion = textBoxOpinion.Text;
            //history.Status = (FireEquipmentHistory.EquipmentStatus)comboBoxStatus.SelectedIndex;
            history.Status = (FireEquipmentHistory.EquipmentStatus)pictureBoxGroup.Tag;
            history.Time = DateTime.Now;



            FormMain2.Instance.ViewControl.FrmEquipHistory.AddEquipmentHistory(dicEquipmentHistory);
            FormMain2.Instance.ViewControl.LeftBar.SetEquipments(FormMain2.Instance.CurrentEquipments);

            return true;
        }

        public bool IsWorking
        {
            get { return m_isWorking; }
        }
    }
}
