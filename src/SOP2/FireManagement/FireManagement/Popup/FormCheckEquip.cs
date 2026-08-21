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
    public partial class FormCheckEquip2 : Form, Ubists.IReaderOwner
    {
        private bool m_isWorking = false;

        public FormCheckEquip2()
        {
            InitializeComponent();

            InitControl();
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

        private void buttonApplyNClear_Click(object sender, EventArgs e)
        {
            if (IsEmpty())
                return;

            if (MakeEquipmentHistory())
                Reset();
        }

        private void buttonComplete_Click(object sender, EventArgs e)
        {
            if (IsEmpty())
                Hide();

            if (MakeEquipmentHistory())
            {
                Reset();
                Hide();
            }
        }

        private void FormCheckEquip_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_isWorking = false;
            FormMain2.Instance.ViewControl.SetRFIDOwner();
            //FormMain2.Instance.EnableEdit();
        }

        public void OnReadTag(string strTag)
        {
            if (textBoxRFID.Text == strTag)
                return;

            textBoxRFID.Text = strTag;

            FireEquipment equip = FormMain2.Instance.DXFManager.FindEquipment(strTag);

            if (equip != null)
                SetEquipment(equip);
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

            if (history != null)
            {
                comboBoxStatus.SelectedIndex = (int)history.Status;
                textBoxOpinion.Text = history.CheckersOpinion;
                textBoxLastCheckedTime.Text = string.Format("{0} {1}:{2}:{3}", history.Time.ToShortDateString(), history.Time.Hour, history.Time.Minute, history.Time.Second);
            }
            else
            {
                comboBoxStatus.SelectedIndex = (int)FireEquipmentHistory.EquipmentStatus.NORMAL;
                textBoxOpinion.Text = "";
                textBoxLastCheckedTime.Text = "";
            }
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
            if (m_isWorking && !FormMain2.Instance.IsPCMode)
            {
                FormMain2.Instance.RFIDReader.Owner = null;
                FormMain2.Instance.RFIDReader.FinishReading();
            }
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
        }

        private bool MakeEquipmentHistory()
        {
            string strRFID = textBoxRFID.Text;
            DXFManager dxfMgr = FormMain2.Instance.DXFManager;

            FireEquipment equip = null;

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

            return MakeEquipmentHistory(equip);
        }

        private bool MakeEquipmentHistory(FireEquipment equip)
        {
            Dictionary<FireEquipment, FireEquipmentHistory> dicEquipmentHistory = FormMain2.Instance.DXFManager.EquipmentHistory;
            FireEquipmentHistory history = null;

            if (dicEquipmentHistory.ContainsKey(equip))
                history = dicEquipmentHistory[equip];
            else
            {
                history = new FireEquipmentHistory();
                dicEquipmentHistory[equip] = history;
            }

            history.EquipmentID = equip.ID;
            history.CheckersOpinion = textBoxOpinion.Text;
            history.Status = (FireEquipmentHistory.EquipmentStatus)comboBoxStatus.SelectedIndex;
            history.Time = DateTime.Now;

            return true;
        }

        public bool IsWorking
        {
            get { return m_isWorking; }
        }
    }
}
