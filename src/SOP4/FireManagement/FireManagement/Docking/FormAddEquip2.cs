using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnE.GUI;

namespace FireManagement
{
    public partial class FormAddEquip2 : Form, Ubists.IReaderOwner
    {

        private bool m_isWorking = false;

        private FireEquipment.EquipmentType m_type = FireEquipment.EquipmentType.FE;

        public FormAddEquip2()
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

            checkBoxUseScren.Checked = true;
            //comboEquipType.SelectedIndex = 0;
        }

        private bool IsEmpty()
        {
            if (textBoxRFID.Text.Length > 0)
                return false;
            else if (textBoxEquipID.Text.Length > 0)
                return false;
            else if (textBoxX.Text.Length > 0 || textBoxY.Text.Length > 0)
                return false;

            return true;
        }

        private bool InvalidCheck(ref float x, ref float y)
        {
            if (textBoxRFID.Text.Length == 0)
            {
                MessageBox.Show("RFID 값이 입력되지 않았습니다.");
                return false;
            }
            //else if (comboEquipType.SelectedIndex < 0)
            //{
            //    MessageBox.Show("설비종류가 설정되어야 합니다.");
            //    return false;
            //}
            else if (textBoxEquipID.Text.Length == 0)
            {
                MessageBox.Show("설비 관리번호가 입력되지 않았습니다.");
                return false;
            }
            else
            {
                if (textBoxX.Text.Length == 0 || textBoxY.Text.Length == 0)
                {
                    MessageBox.Show("설비 위치가 입력되지 않았습니다.");
                    return false;
                }

                try
                {
                    // 이미 Meter 단위계로 바뀐 값이기 때문에 millimeter 단위계로 변환하여 입력시킨다.
                    float fUnitFlag = FormMain2.Instance.GetUnitFlag(UnitOfLength.METER);
                    x = float.Parse(textBoxX.Text) / fUnitFlag;
                    y = float.Parse(textBoxY.Text) / fUnitFlag;
                }
                catch (Exception)
                {
                    MessageBox.Show("설비 위치는 숫자만 입력할 수 있습니다.");
                    return false;
                }
            }

            return true;
        }

        private FireEquipment AddEquipment(float x, float y)
        {
            //FireEquipment.EquipmentType type = (FireEquipment.EquipmentType)(comboEquipType.SelectedIndex + 1);

            FireEquipment.EquipmentType type = m_type;

            bool isValid = FormMain2.Instance.IOManager.CheckRFIDDuplication(textBoxRFID.Text, textBoxRFIDTagID.Text,
                type, textBoxEquipID.Text, textBoxLocationName.Text, x, y);

            if (!isValid)
                return null;

            FireEquipment equip = FormMain2.Instance.DXFManager.FindEquipment(textBoxRFID.Text);

            if (equip == null)
            {
                equip = FormMain2.Instance.ViewControl.LeftBar.AddNewEquipment(textBoxRFID.Text, textBoxRFIDTagID.Text,
                            textBoxEquipID.Text, type, x, y, textBoxLocationName.Text);
            }
            else
            {
                FormMain2.Instance.DrawingControl.SelectShape(equip.LinkedShape, true);
                FormMain2.Instance.ViewControl.LeftBar.SelectShape(equip.LinkedShape);
                FormMain2.Instance.Refresh();
                MessageBox.Show("이미 같은 RFID를 가진 설비가 존재합니다.");
                return null;
                /*equip.EquipID = textBoxEquipID.Text;
                equip.Position = new PointF(x, y);
                equip.RFIDTag = textBoxRFID.Text;
                equip.RFIDTagID = textBoxRFIDTagID.Text;
                equip.Type = type;

                if (equip.LinkedShape != null)
                {
                    UnE.Geometry.Vertex2D vMove = FormMain.Instance.DXFControl.MovedVertex;
                    equip.LinkedShape.Move(equip.Position.X + vMove.x, equip.Position.Y + vMove.y);
                }*/
            }

            return equip;
        }

        private void buttonComplete_Click(object sender, EventArgs e)
        {
            if (IsEmpty())
            {
                //Hide();
                return;
            }

            float x = 0.0f, y = 0.0f;
            if (!InvalidCheck(ref x, ref y))
                return;

            FireEquipment equip = AddEquipment(x, y);

            if (equip != null)
            {
                FormMain2.Instance.DrawingControl.SelectShape(equip.LinkedShape, true);
                FormMain2.Instance.ViewControl.LeftBar.SelectShape(equip.LinkedShape);
                FormMain2.Instance.Refresh();
                //Hide();

                //연속추가가 체크되어있으면 창을 닫을 필요가 없음.
                if (checkContinueAdd.Checked == false)
                {
                    FormMain2.Instance.ViewControl.ButtonClose();
                }       
            }

            Reset();
        }

        public void Reset()
        {
            textBoxRFID.Text = "";
            textBoxRFIDTagID.Text = "";
            textBoxEquipID.Text = "";
            textBoxLocationName.Text = "";
            textBoxX.Text = "";
            textBoxY.Text = "";
        }

        public void ScreenInput(double x, double y)
        {
            if (checkBoxUseScren.Checked)
            {
                textBoxX.Text = x.ToString();
                textBoxY.Text = y.ToString();

                float fUnitFlag = FormMain2.Instance.GetUnitFlag(UnitOfLength.METER);
                float X = (float)(x / fUnitFlag);
                float Y = (float)(y / fUnitFlag);

                object shape = FormMain2.Instance.DXFManager.MakeTempShape(m_type, X, Y);
                if (shape != null)
                    FormMain2.Instance.DrawingControl.Refresh();
            }
        }

        public void Hide()
        {
            m_isWorking = false;

            FormMain2.Instance.ViewControl.SetRFIDOwner();
            FormMain2.Instance.DXFManager.ClearTeampShape();
            //FormMain2.Instance.EnableEdit();
            //FormMain.Instance.RFIDReader.FinishReading();
            //FormMain.Instance.RFIDReader.Owner = null;

            base.Hide();
        }

        public void Show()
        {
            m_isWorking = true;

            if (radioRFID.Checked && !FormMain2.Instance.IsPCMode)
            {
                FormMain2.Instance.RFIDReader.Owner = this;
                if (!FormMain2.Instance.RFIDReader.StartReading())
                {
                    m_isWorking = false;

                    FormMain2.Instance.ViewControl.SetRFIDOwner();
                    FormMain2.Instance.DXFManager.ClearTeampShape();
                    //FormMain2.Instance.EnableEdit();
                    return;
                }
            }

            base.Show();
        }

        public void OnReadTag(string strTag)
        {
            if (textBoxRFID.Text == strTag)
                return;

            if (!FormMain2.Instance.IOManager.CheckRFIDDuplication(strTag))
                return;

            textBoxRFID.Text = strTag;

            FireEquipment equip = FormMain2.Instance.DXFManager.FindEquipment(strTag);

            if (equip != null)
            {
                float fUnitFlag = FormMain2.Instance.GetUnitFlag(UnitOfLength.METER);

                textBoxRFIDTagID.Text = equip.RFIDTagID;
                textBoxEquipID.Text = equip.EquipID;
                textBoxLocationName.Text = equip.Description;
                textBoxX.Text = (equip.Position.X * fUnitFlag).ToString();
                textBoxY.Text = (equip.Position.Y * fUnitFlag).ToString();

                //comboEquipType.SelectedIndex = ((int)equip.Type) - 1;
            }
        }

        public bool IsWorking
        {
            get { return m_isWorking; }
        }

        private void radioManual_CheckedChanged(object sender, EventArgs e)
        {
            textBoxRFID.Enabled = true;

            if (m_isWorking && !FormMain2.Instance.IsPCMode)
            {
                FormMain2.Instance.RFIDReader.Owner = null;
                FormMain2.Instance.RFIDReader.FinishReading();
            }
        }

        private void radioRFID_CheckedChanged(object sender, EventArgs e)
        {
            textBoxRFID.Enabled = false;

            if (m_isWorking && !FormMain2.Instance.IsPCMode)
            {
                FormMain2.Instance.RFIDReader.Owner = this;
                FormMain2.Instance.RFIDReader.StartReading();
            }
        }

        private void checkBoxUseScren_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBoxUseScren.Checked)
            {
                textBoxX.Enabled = false;
                textBoxY.Enabled = false;
            }
            else
            {
                textBoxX.Enabled = true;
                textBoxY.Enabled = true;
            }
        }

        private void FormAddEquip2_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_isWorking = false;

            FormMain2.Instance.ViewControl.SetRFIDOwner();
            FormMain2.Instance.DXFManager.ClearTeampShape();
            //FormMain2.Instance.EnableEdit();
            //FormMain.Instance.RFIDReader.FinishReading();
            //FormMain.Instance.RFIDReader.Owner = null;
        }


        private void SelectedEquipType(RibbonButton btn)
        {
            if (btn == btnFireExtingusher)
            {
                if (btnFireExtingusher.IsChecked == false)
                {
                    btnFireExtingusher.IsChecked = true;
                    btnFirePlug.IsChecked = false;
                    btnFireAlarm.IsChecked = false;
                    btnFireReciver.IsChecked = false;
                }

                pictureBoxCircle01.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle_01;
                pictureBoxCircle02.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle02;
                pictureBoxCircle03.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle03;
            }
            else if (btn == btnFirePlug)
            {
                if (btnFirePlug.IsChecked == false)
                {
                    btnFireExtingusher.IsChecked = false;
                    btnFirePlug.IsChecked = true;
                    btnFireAlarm.IsChecked = false;
                    btnFireReciver.IsChecked = false;
                }

                pictureBoxCircle01.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle02;
                pictureBoxCircle02.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle_01;
                pictureBoxCircle03.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle03;
            }
            else if (btn == btnFireAlarm)
            {
                if (btnFireAlarm.IsChecked == false)
                {
                    btnFireExtingusher.IsChecked = false;
                    btnFirePlug.IsChecked = false;
                    btnFireAlarm.IsChecked = true;
                    btnFireReciver.IsChecked = false;
                }

                pictureBoxCircle01.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle03;
                pictureBoxCircle03.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle_01;
                pictureBoxCircle02.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle02;
            }
            else if( btn == btnFireReciver)
            {
                if (btnFireReciver.IsChecked == false)
                {
                    btnFireExtingusher.IsChecked = false;
                    btnFirePlug.IsChecked = false;
                    btnFireAlarm.IsChecked = false;
                    btnFireReciver.IsChecked = true;
                }

                pictureBoxCircle01.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle03;
                pictureBoxCircle03.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle_01;
                pictureBoxCircle02.BackgroundImage = global::FireManagement.Properties.Resources.Bottomcircle02;
            
            }

            btnFireExtingusher.Refresh();
            btnFirePlug.Refresh();
            btnFireAlarm.Refresh();
        }


        private void btnFireExtingusher_Click(object sender, EventArgs e)
        {
            SelectedEquipType(btnFireExtingusher);
            m_type = FireEquipment.EquipmentType.FE;
        }

        private void btnFirePlug_Click(object sender, EventArgs e)
        {
            SelectedEquipType(btnFirePlug);
            m_type = FireEquipment.EquipmentType.HD;
        }

        private void btnFireAlarm_Click(object sender, EventArgs e)
        {
            SelectedEquipType(btnFireAlarm);
            m_type = FireEquipment.EquipmentType.FA;
        }
        private void btnFireReciver_Click(object sender, EventArgs e)
        {
            SelectedEquipType(btnFireReciver);
            m_type = FireEquipment.EquipmentType.FR;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            FormMain2.Instance.ViewControl.ButtonClose();
            Reset();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            FormMain2.Instance.ViewControl.ButtonClose();
        }

        
    }
}
