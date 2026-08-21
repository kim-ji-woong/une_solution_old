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
    public partial class FormAddEquip : Form, Ubists.IReaderOwner
    {
        private bool m_isWorking = false;

        public FormAddEquip()
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
            comboEquipType.SelectedIndex = 0;
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
            else if (comboEquipType.SelectedIndex < 0)
            {
                MessageBox.Show("설비종류가 설정되어야 합니다.");
                return false;
            }
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
                    float fUnitFlag = FormMain2.Instance.GetUnitFlag(DXFViewer.UnitOfLength.METER);
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
            FireEquipment.EquipmentType type = (FireEquipment.EquipmentType)(comboEquipType.SelectedIndex + 1);

            bool isValid = FormMain2.Instance.IOManager.CheckRFIDDuplication(textBoxRFID.Text, textBoxRFIDTagID.Text, 
                type, textBoxEquipID.Text, textBoxLocationName.Text, x, y);

            if (!isValid)
                return null;

            FireEquipment equip = FormMain2.Instance.DXFManager.FindEquipment(textBoxRFID.Text);

            if (equip == null)
            {
               // equip = FormMain2.Instance.ViewControl.LeftBar.AddNewEquipment(textBoxRFID.Text, textBoxRFIDTagID.Text,
                //            textBoxEquipID.Text, type, x, y, textBoxLocationName.Text);
            }
            else
            {
                equip.LinkedShape.Selected = true;
               // FormMain2.Instance.ViewControl.LeftBar.SelectShape(equip.LinkedShape);
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
                    UnE.Geometry.Vertex2D vMove = FormMain2.Instance.DXFControl.MovedVertex;
                    equip.LinkedShape.Move(equip.Position.X + vMove.x, equip.Position.Y + vMove.y);
                }*/
            }

            return equip;
        }

        private void buttonComplete_Click(object sender, EventArgs e)
        {
            if (IsEmpty())
            {
                Hide();
                return;
            }

            float x = 0.0f, y = 0.0f;
            if (!InvalidCheck(ref x, ref y))
                return;

            FireEquipment equip = AddEquipment(x, y);

            if (equip != null)
            {
                equip.LinkedShape.Selected = true;
                //FormMain2.Instance.ViewControl.LeftBar.SelectShape(equip.LinkedShape);
                FormMain2.Instance.Refresh();
                Hide();
            }
        }

        private void buttonAddNClear_Click(object sender, EventArgs e)
        {
            if (IsEmpty())
                return;

            float x = 0.0f, y = 0.0f;
            if (!InvalidCheck(ref x, ref y))
                return;

            FireEquipment equip = AddEquipment(x, y);

            if (equip != null)
            {
                equip.LinkedShape.Selected = true;
                //FormMain2.Instance.ViewControl.LeftBar.SelectShape(equip.LinkedShape);
                FormMain2.Instance.Refresh();
                Reset();
            }
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

        private void radioRFID_CheckedChanged(object sender, EventArgs e)
        {
            textBoxRFID.Enabled = false;

            if (m_isWorking && !FormMain2.Instance.IsPCMode)
            {
                FormMain2.Instance.RFIDReader.Owner = this;
                FormMain2.Instance.RFIDReader.StartReading();
            }
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

        private void checkBoxUseScren_CheckedChanged(object sender, EventArgs e)
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

        public void ScreenInput(double x, double y)
        {
            if (checkBoxUseScren.Checked)
            {
                textBoxX.Text = x.ToString();
                textBoxY.Text = y.ToString();

                float fUnitFlag = FormMain2.Instance.GetUnitFlag(DXFViewer.UnitOfLength.METER);
                float X = (float)(x / fUnitFlag);
                float Y = (float)(y / fUnitFlag);

                DXFViewer.Shape shape = FormMain2.Instance.DXFManager.MakeTempShape((FireEquipment.EquipmentType)(comboEquipType.SelectedIndex + 1), X, Y);
                if (shape != null)
                    FormMain2.Instance.DXFControl.Refresh();
            }
        }

        private void FormAddEquip_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_isWorking = false;

            FormMain2.Instance.ViewControl.SetRFIDOwner();
            FormMain2.Instance.DXFManager.ClearTeampShape();
            //FormMain2.Instance.EnableEdit();
            //FormMain2.Instance.RFIDReader.FinishReading();
            //FormMain2.Instance.RFIDReader.Owner = null;
        }

        public void Hide()
        {
            m_isWorking = false;

            FormMain2.Instance.ViewControl.SetRFIDOwner();
            FormMain2.Instance.DXFManager.ClearTeampShape();
            //FormMain2.Instance.EnableEdit();
            //FormMain2.Instance.RFIDReader.FinishReading();
            //FormMain2.Instance.RFIDReader.Owner = null;

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

            textBoxRFID.Text = strTag;

            FireEquipment equip = FormMain2.Instance.DXFManager.FindEquipment(strTag);

            if (equip != null)
            {
                float fUnitFlag = FormMain2.Instance.GetUnitFlag(DXFViewer.UnitOfLength.METER);

                textBoxRFIDTagID.Text = equip.RFIDTagID;
                textBoxEquipID.Text = equip.EquipID;
                textBoxLocationName.Text = equip.Description;
                textBoxX.Text = (equip.Position.X * fUnitFlag).ToString();
                textBoxY.Text = (equip.Position.Y * fUnitFlag).ToString();

                comboEquipType.SelectedIndex = ((int)equip.Type) - 1;
            }
        }

        public bool IsWorking
        {
            get { return m_isWorking; }
        }
    }
}
