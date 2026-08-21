using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace HSMS
{
    public partial class FormAlarmDistance : Form
    {
        private Dictionary<string, float> m_dicWorkerToZoneDistance = new Dictionary<string, float>();
        private Dictionary<string, float> m_dicWorkerToEquipDistance = new Dictionary<string, float>();

        public FormAlarmDistance()
        {
            InitializeComponent();
        }

        private void FormAlarmDistance_Shown(object sender, EventArgs e)
        {
            DataManager dataMgr = FormMain.Instance.DataMgr;

            textBoxWorkerToCarDistanceBoth.Text = string.Format("{0:F1}", dataMgr.WorkerToCarDistanceBoth);
            textBoxWorkerToCarDistanceOneSide.Text = string.Format("{0:F1}", dataMgr.WorkerToCarDistanceOneSide);
            //textBoxWorkerToZoneDistance.Text = string.Format("{0:F1}", dataMgr.WorkerToZoneDistance);
            //textBoxWorkerToEquipDistance.Text = string.Format("{0:F1}", dataMgr.WorkerToEquipDistance);
            textBoxCoGas.Text = string.Format("{0:F1}", dataMgr.COGasTolerance);
            textBoxMethaneGas.Text = string.Format("{0:F1}", dataMgr.MethaneTolerance);

            InitZone(dataMgr);
            InitEquip(dataMgr);
        }

        private void InitZone(DataManager dataMgr)
        {
            int nZoneGroupCount = dataMgr.GetZoneGroupCount();

            for (int i = 0; i < nZoneGroupCount; i++)
            {
                ZoneGroup group = dataMgr.GetZoneGroup(i);
                cmbWorkerToZoneDistance.Items.Add(group);
            }

            if (cmbWorkerToZoneDistance.Items.Count > 0)
                cmbWorkerToZoneDistance.SelectedIndex = 0;
        }

        private void InitEquip(DataManager dataMgr)
        {
            int nEquipGroupCount = dataMgr.GetEquipmentGroupCount();

            for (int i=0;i<nEquipGroupCount;i++)
            {
                EquipmentGroup group = dataMgr.GetEquipmentGroup(i);
                cmbWorkerToEquipDistance.Items.Add(group);
            }

            if (cmbWorkerToEquipDistance.Items.Count > 0)
                cmbWorkerToEquipDistance.SelectedIndex = 0;
        }

        private void cmbWorkerToZoneDistance_SelectedIndexChanged(object sender, EventArgs e)
        {
            ZoneGroup group = (ZoneGroup)cmbWorkerToZoneDistance.SelectedItem;
            if (group == null)
                return;

            float fDistance;

            if (FormMain.Instance.DataMgr.GetWorkerToZoneDistance(group.ToString(), out fDistance))
                textBoxWorkerToZoneDistance.Text = string.Format("{0:F1}", fDistance);
        }

        private void cmbWorkerToEquipDistance_SelectedIndexChanged(object sender, EventArgs e)
        {
            EquipmentGroup group = (EquipmentGroup)cmbWorkerToEquipDistance.SelectedItem;
            if (group == null)
                return;

            float fDistance;

            if (FormMain.Instance.DataMgr.GetWorkerToEquipDistance(group.ToString(), out fDistance))
                textBoxWorkerToEquipDistance.Text = string.Format("{0:F1}", fDistance);
        }

        private bool CheckTextBox(TextBox textBox, string strTag, ref float fDistance)
        {
            if (textBox.Text.Length == 0)
            {
                MessageBox.Show("[" + strTag + "]에 값을 입력하세요.");
                textBox.Focus();
                return false;
            }

            if (!float.TryParse(textBox.Text, out fDistance))
            {
                MessageBox.Show("숫자만 입력 가능합니다.");
                textBox.Focus();
                return false;
            }

            if (fDistance < 0.0f)
            {
                MessageBox.Show("0보다 큰 숫자만 입력 가능합니다.");
                textBox.Focus();
                return false;
            }

            return true;
        }

        private bool CheckTextBox(out float fWorkerToCarDistanceBoth, out float fWorkerToCarDistanceOneSide, out float fWorkerToZoneDistance, out float fWorkerToEquipDistance, out float fCOGasTolerance, out float fMethaneTolerance)
        {
            fWorkerToCarDistanceBoth = fWorkerToCarDistanceOneSide = fWorkerToZoneDistance = fWorkerToEquipDistance = 0.0f;
            fCOGasTolerance = fMethaneTolerance = 0.0f;

            if (!CheckTextBox(textBoxWorkerToCarDistanceBoth, "작업자와 차량간 안전거리 - 상호 접근", ref fWorkerToCarDistanceBoth))
                return false;

            if (!CheckTextBox(textBoxWorkerToCarDistanceOneSide, "작업자와 차량간 안전거리 - 한쪽에서 접근", ref fWorkerToCarDistanceOneSide))
                return false;

            if (!CheckTextBox(textBoxWorkerToZoneDistance, "작업자와 위험영역간 안전거리", ref fWorkerToZoneDistance))
                return false;

            if (!CheckTextBox(textBoxWorkerToEquipDistance, "작업자와 위험설비간 안전거리", ref fWorkerToEquipDistance))
                return false;

            if (!CheckTextBox(textBoxCoGas, "일산화탄소 안전농도", ref fCOGasTolerance))
                return false;

            if (!CheckTextBox(textBoxMethaneGas, "메탄가스 안전농도", ref fMethaneTolerance))
                return false;

            return true;
        }

        // Return 값 : 변경된 값이 있는가?
        private bool CheckWorkerToZoneDistance()
        {
            DataManager dataMgr = FormMain.Instance.DataMgr;
            ArrayList arrRemove = new ArrayList();
            float fDistance;

            foreach (KeyValuePair<string, float> pair in m_dicWorkerToZoneDistance)
            {
                if (dataMgr.GetWorkerToZoneDistance(pair.Key, out fDistance))
                {
                    if (pair.Value == fDistance)
                        arrRemove.Add(pair.Key);
                }
            }

            foreach (string strZoneGroupName in arrRemove)
            {
                m_dicWorkerToZoneDistance.Remove(strZoneGroupName);
            }

            return m_dicWorkerToZoneDistance.Count > 0;
        }

        // Return 값 : 변경된 값이 있는가?
        private bool CheckWorkerToEquipDistance()
        {
            DataManager dataMgr = FormMain.Instance.DataMgr;
            ArrayList arrRemove = new ArrayList();
            float fDistance;

            foreach (KeyValuePair<string, float> pair in m_dicWorkerToEquipDistance)
            {
                if (dataMgr.GetWorkerToEquipDistance(pair.Key, out fDistance))
                {
                    if (pair.Value == fDistance)
                        arrRemove.Add(pair.Key);
                }
            }

            foreach (string strEquipGroupName in arrRemove)
            {
                m_dicWorkerToEquipDistance.Remove(strEquipGroupName);
            }

            return m_dicWorkerToEquipDistance.Count > 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DBConn conn = new DBConn("HSMS");

            DataManager dataMgr = FormMain.Instance.DataMgr;
            float fWorkerToCarDistanceBoth, fWorkerToCarDistanceOneSide, fWorkerToZoneDistance, fWorkerToEquipDistance, fCOGasTolerance, fMethaneTolerance;

            if (!CheckTextBox(out fWorkerToCarDistanceBoth, out fWorkerToCarDistanceOneSide, out fWorkerToZoneDistance, out fWorkerToEquipDistance, out fCOGasTolerance, out fMethaneTolerance))
                return;

            const int nDataCount = 6;
            ArrayList arrDatas = new ArrayList();
            bool[] arrChanged = new bool[nDataCount] { false, false, false, false, false, false };
            float[] arrDistance = new float[nDataCount] { fWorkerToCarDistanceBoth, fWorkerToCarDistanceOneSide, fWorkerToZoneDistance, fWorkerToEquipDistance, fCOGasTolerance, fMethaneTolerance };

            if (fWorkerToCarDistanceBoth != dataMgr.WorkerToCarDistanceBoth)
            {
                // DB는 Server에서 값을 바꾸고, DataManager의 데이터는 Server로부터
                // CHANGE_DB_DATA_LIST를 되돌려 받을때 바꾸어준다.
                /*EditAlarmDistance editAlarmDistance = new EditAlarmDistance();
                editAlarmDistance.SQLType = EditAlarmDistance.WorkerToCarDistanceBoth;

                editAlarmDistance.ItemValue = fWorkerToCarDistanceBoth;

                if (editAlarmDistance.Update(conn))*/
                    arrChanged[0] = true;
            }

            if (fWorkerToCarDistanceOneSide != dataMgr.WorkerToCarDistanceOneSide)
            {
                /*EditAlarmDistance editAlarmDistance = new EditAlarmDistance();
                editAlarmDistance.SQLType = EditAlarmDistance.WorkerToCarDistanceOneSide;

                editAlarmDistance.ItemValue = fWorkerToCarDistanceOneSide;

                if (editAlarmDistance.Update(conn))*/
                    arrChanged[1] = true;
            }

            //if (fWorkerToZoneDistance != dataMgr.WorkerToZoneDistance)
            //{
            //    /*EditAlarmDistance editAlarmDistance = new EditAlarmDistance();
            //    editAlarmDistance.SQLType = EditAlarmDistance.WorkerToZoneDistance;

            //    editAlarmDistance.ItemValue = fWorkerToZoneDistance;

            //    if (editAlarmDistance.Update(conn))*/
            //        arrChanged[2] = true;
            //}
            arrChanged[2] = CheckWorkerToZoneDistance();

            //if (fWorkerToEquipDistance != dataMgr.WorkerToEquipDistance)
            //{
            //    /*EditAlarmDistance editAlarmDistance = new EditAlarmDistance();
            //    editAlarmDistance.SQLType = EditAlarmDistance.WorkerToEquipDistance;

            //    editAlarmDistance.ItemValue = fWorkerToEquipDistance;

            //    if (editAlarmDistance.Update(conn))*/
            //        arrChanged[3] = true;
            //}
            arrChanged[3] = CheckWorkerToEquipDistance();

            if (fCOGasTolerance != dataMgr.COGasTolerance)
                arrChanged[4] = true;

            if (fMethaneTolerance != dataMgr.MethaneTolerance)
                arrChanged[5] = true;

            bool isChanged = false;

            for (int i = 0; i < nDataCount; i++)
            {
                arrDatas.Add(arrChanged[i]);

                if (arrChanged[i])
                {
                    isChanged = true;

                    if (i == 2)
                    {
                        foreach (KeyValuePair<string, float> pair in m_dicWorkerToZoneDistance)
                        {
                            arrDatas.Add(pair.Key);
                            arrDatas.Add(pair.Value);
                        }
                    }
                    else if (i == 3)
                    {
                        foreach (KeyValuePair<string, float> pair in m_dicWorkerToEquipDistance)
                        {
                            arrDatas.Add(pair.Key);
                            arrDatas.Add(pair.Value);
                        }
                    }
                    else
                        arrDatas.Add(arrDistance[i]);
                }
            }

            if (isChanged)
                FormMain.Instance.NetMgr.SendDBDataList(ChangeDataType.ALARM_DISTANCE, arrDatas);

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBoxWorkerToZoneDistance_TextChanged(object sender, EventArgs e)
        {
            if (textBoxWorkerToZoneDistance.Text.Length == 0)
                return;

            float fDistance;
            if (!float.TryParse(textBoxWorkerToZoneDistance.Text, out fDistance))
                return;

            if (fDistance >= 0.0f)
            {
                m_dicWorkerToZoneDistance[cmbWorkerToZoneDistance.Text] = fDistance;
            }
        }

        private void textBoxWorkerToEquipDistance_TextChanged(object sender, EventArgs e)
        {
            if (textBoxWorkerToEquipDistance.Text.Length == 0)
                return;

            float fDistance;
            if (!float.TryParse(textBoxWorkerToEquipDistance.Text, out fDistance))
                return;

            if (fDistance >= 0.0f)
            {
                m_dicWorkerToEquipDistance[cmbWorkerToEquipDistance.Text] = fDistance;
            }
        }
    }
}
