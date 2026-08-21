using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadMan
{
    public partial class FormScheduleDetail : Form
    {
        private ScheduleProperty m_property = null;

        private FormScheduleProperty m_frmScheduleProperty = null;
        private DataGridViewRow m_row = null;

        private ImportanceData m_importanceDataTemp = null;
        private List<LandAddressData> m_landAddressDataTemp = null;

        private DateTime m_dtFinal;
        private DateTime m_dtFirst;

        private IClosingWorker m_closingWorker = null;
        private PanelDXFViewer m_panel = null;

        public FormScheduleProperty SchedulePropertyForm
        {
            get { return m_frmScheduleProperty; }
            set { m_frmScheduleProperty = value; }
        }

        public DataGridViewRow Row
        {
            get { return m_row; }
            set { m_row = value; }
        }

        public ScheduleProperty ScheduleProperty
        {
            get { return m_property; }
        }

        public IClosingWorker ClosingWorker
        {
            get { return m_closingWorker; }
            set { m_closingWorker = value; }
        }

        public FormScheduleDetail(ScheduleProperty prop, PanelDXFViewer panel)
        {
            InitializeComponent();
            m_property = prop;
            m_panel = panel;

            if (prop != null)
            {
                if (prop.Importance == null)
                    prop.Importance = new ImportanceData();

                m_importanceDataTemp = prop.Importance.Clone();
                m_landAddressDataTemp = new List<LandAddressData>();
				CloneLandAddressDatas((List<LandAddressData>)prop.LandAddressDatas, (List<LandAddressData>)m_landAddressDataTemp);
            }

            if (m_panel != null)
                m_panel.ScheduleDetailForm = this;
        }

        public void DisableEdit()
        {
            checkBoxEdit.Enabled = false;
        }

        private void CloneLandAddressDatas(List<LandAddressData> datasSrc, List<LandAddressData> datasTrg)
        {
            datasTrg.Clear();

            foreach (LandAddressData data in datasSrc)
            {
                LandAddressData data2 = data.Clone();
                datasTrg.Add(data2);
            }
        }

        private void FormScheduleDetail_Load(object sender, EventArgs e)
        {
            if (m_property != null)
            {
                labelAddrName.Text = m_property.StreetName;
                labelImportance.Text = DoubleString(m_property.Importance.Importance, 2);
                textBoxWidth.Text = DoubleString(m_property.Width);
                textBoxArea.Text = DoubleString(m_property.Area);
                textBoxLength.Text = LongString<int>(m_property.Length);
                labelLandAddr.Text = m_property.GetFirstNLastLandAddressString();

                if (m_property.FinalDate == null)
                {
                    dtPickerFinal.Tag = null;
                    m_dtFinal = dtPickerFinal.Value;
                }
                else
                {
                    dtPickerFinal.Value = m_property.FinalDate.Data;
                    dtPickerFinal.Tag = m_property.FinalDate.Data;
                }

                if (m_property.FirstDate == null)
                {
                    dtPickerFirst.Tag = null;
                    m_dtFirst = dtPickerFirst.Value;
                }
                else
                {
                    dtPickerFirst.Value = m_property.FirstDate.Data;
                    dtPickerFirst.Tag = m_property.FirstDate.Data;
                }
                
                SetComboBoxText(cboCategory, m_property.Category);
                SetComboBoxText(cboSubCategory, m_property.SubCategory);
                cboComplete.SelectedIndex = m_property.IsComplete ? 1 : 0;

                textBoxRiceField.Text = DoubleString(m_property.RiceField);
                textBoxField.Text = DoubleString(m_property.Field);
                textBoxLand.Text = DoubleString(m_property.Land);
                textBoxETC.Text = DoubleString(m_property.ETC);

                labelTotalCost.Text = m_property.TotalCost;
                textBoxLandCost.Text = LongString(m_property.LandCost);
                textBoxObjectCost.Text = LongString(m_property.ObjectCost);
                textBoxAroundCost.Text = LongString(m_property.AroundCost);

                textBoxWidth.Select();
                textBoxWidth.Select(0, 0);
            }

            EnableControl(checkBoxEdit.Checked);
        }

        private void EnableControl(bool enabled)
        {
            textBoxWidth.Enabled = textBoxArea.Enabled = textBoxLength.Enabled = enabled;
            dtPickerFinal.Enabled = dtPickerFirst.Enabled = /*btnImportanceDetail.Enabled = btnLandAddress.Enabled =*/ enabled;
            cboCategory.Enabled = cboSubCategory.Enabled = cboComplete.Enabled = enabled;
            textBoxRiceField.Enabled = textBoxField.Enabled = textBoxLand.Enabled = textBoxETC.Enabled = enabled;
            textBoxLandCost.Enabled = textBoxAroundCost.Enabled = textBoxObjectCost.Enabled = enabled;
            btnMillionLandCost.Enabled = btnMillionAroundCost.Enabled = btnMillionObjectCost.Enabled = enabled;
        }

        private void SetComboBoxText(ComboBox cbo, string strText)
        {
            if (strText.Length == 0)
            {
                cbo.SelectedIndex = -1;
                return;
            }

            int nIndex = cbo.Items.IndexOf(strText);

            if (nIndex >= 0)
            {
                cbo.SelectedIndex = nIndex;
            }
            else
            {
                cbo.Items.Add(strText);
                cbo.SelectedIndex = cbo.Items.Count - 1;
            }
        }

        private string DoubleString(VariousData<double> data, int nCount = 0)
        {
            if (data == null)
                return "";

            string strFormat = "{0:F" + nCount.ToString() + "}";
            return string.Format(strFormat, data.Data);
        }

        private string DoubleString(double data, int nCount = 0)
        {
            string strFormat = "{0:F" + nCount.ToString() + "}";
            return string.Format(strFormat, data);
        }

        private string LongString<T>(VariousData<T> data)
        {
            if (data == null)
                return "";

            return data.Data.ToString();
        }

        private void btnImportanceDetail_Click(object sender, EventArgs e)
        {
            SettingImportance_ScheduleDetail setting = new SettingImportance_ScheduleDetail(labelImportance, m_importanceDataTemp);
            FormImportance frm = new FormImportance(setting);

            if (!checkBoxEdit.Checked)
                frm.DisableEdit();

			DialogFormFrame frameDetail = new DialogFormFrame(frm);
			frameDetail.ShowDialog(this);
        }

        
        private DataGridViewCell GetCell<T>()
        {
            if (m_row == null)
                return null;

            Type type = typeof(T);

            foreach (DataGridViewCell cell in m_row.Cells)
            {
                if (cell.Tag != null && cell.Tag.GetType() == type)
                    return cell;
            }

            return null;
        }

        private void btnLandAddress_Click(object sender, EventArgs e)
        {
            SettingLandNumber_ScheduleDetail setting = new SettingLandNumber_ScheduleDetail(labelLandAddr, m_landAddressDataTemp);
            FormLandNumber frm = new FormLandNumber(setting, m_property.StreetName);

            if (!checkBoxEdit.Checked)
                frm.DisableEdit();

			DialogFormFrame frameDetail = new DialogFormFrame(frm);
			frameDetail.ShowDialog(this);
        }

        private void textBoxCost_TextChanged(object sender, EventArgs e)
        {
            long nLandCost = 0, nObjectCost = 0, nAroundCost = 0;

            if (textBoxLandCost.Text.Length > 0)
            {
                if (!long.TryParse(textBoxLandCost.Text, out nLandCost))
                {
                    labelTotalCost.Text = "-";
                    return;
                }
            }

            if (textBoxObjectCost.Text.Length > 0)
            {
                if (!long.TryParse(textBoxObjectCost.Text, out nObjectCost))
                {
                    labelTotalCost.Text = "-";
                    return;
                }
            }

            if (textBoxAroundCost.Text.Length > 0)
            {
                if (!long.TryParse(textBoxAroundCost.Text, out nAroundCost))
                {
                    labelTotalCost.Text = "-";
                    return;
                }
            }

            long nTotalCost = nLandCost + nObjectCost + nAroundCost;

            if (nTotalCost == 0)
                labelTotalCost.Text = "0원";
            else
                labelTotalCost.Text = string.Format("{0:###,###,###,###,###,###}원", nLandCost + nObjectCost + nAroundCost);
        }

        /*private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            SetThousandMark(textBox);
            SetTotalText();
        }

        private void SetTotalText()
        {
            long nLandCost = GetTextBoxCost(textBoxLandCost);
            long nObjectCost = GetTextBoxCost(textBoxObjectCost);
            long nAroundCost = GetTextBoxCost(textBoxAroundCost);

            labelTotalCost.Text = (nLandCost + nObjectCost + nAroundCost).ToString();
        }

        private long GetTextBoxCost(TextBox textBox)
        {
            string[] arrText = textBox.Text.Split(',');
            string strNumber = "";

            foreach (string strText in arrText)
            {
                strNumber += strText;
            }

            long nData;

            if (!long.TryParse(strNumber, out nData))
                return 0;

            return nData;
        }

        private void SetThousandMark(TextBox textBox)
        {
            if (textBox.Text.Length < 3)
                return;

            string[] arrText = textBox.Text.Split(',');
            string strNumber = "";

            foreach (string strText in arrText)
            {
                strNumber += strText;
            }

            if (strNumber.Length > 0 && strNumber[0] == '0')
                return;

            long nData;

            if (!long.TryParse(strNumber, out nData))
                return;

            string strResult = "";
            int nIndex = 1;

            for (int i=strNumber.Length - 1;i>=0;i--)
            {
                strResult += strNumber[i];

                if (nIndex++ == 3 && i > 0)
                {
                    strResult += ",";
                    nIndex = 1;
                }
            }

            strResult.Reverse();
            textBox.Text = strResult;
        }*/

        private void btnOK_Click(object sender, EventArgs e)
        {
            double dWidth, dArea, dRiceField, dField, dLand, dETC;
            long nLandCost, nObjectCost, nAroundCost;
            int nLength;

            if (!CheckText(out dWidth, out dArea, out nLength, out dRiceField, out dField, out dLand, out dETC, out nLandCost, out nObjectCost, out nAroundCost))
                return;

            m_property.Importance.CopyFrom(m_importanceDataTemp);
            
            if (m_property.Width == null)
                m_property.Width = new VariousData<double>(dWidth);
            else
                m_property.Width.Data = dWidth;

            if (m_property.Area == null)
                m_property.Area = new VariousData<double>(dArea);
            else
                m_property.Area.Data = dArea;

            if (m_property.Length == null)
                m_property.Length = new VariousData<int>(nLength);
            else
                m_property.Length.Data = nLength;

			CloneLandAddressDatas(m_landAddressDataTemp, (List < LandAddressData >)m_property.LandAddressDatas);

            if (dtPickerFinal.Tag == null)
            {
                if (m_dtFinal != dtPickerFinal.Value)
                {
                    m_property.FinalDate = new VariousData<DateTime>(dtPickerFinal.Value);
                }
            }
            else
            {
                m_property.FinalDate.Data = dtPickerFinal.Value;
            }

            if (dtPickerFirst.Tag == null)
            {
                if (m_dtFirst != dtPickerFirst.Value)
                {
                    m_property.FirstDate = new VariousData<DateTime>(dtPickerFirst.Value);
                }
            }
            else
            {
                m_property.FirstDate.Data = dtPickerFirst.Value;
            }
            
            m_property.Category = cboCategory.Text;
            m_property.SubCategory = cboSubCategory.Text;
            m_property.IsComplete = cboComplete.SelectedIndex == 1 ? true : false;

            if (m_property.RiceField == null)
                m_property.RiceField = new VariousData<double>(dRiceField);
            else
                m_property.RiceField.Data = dRiceField;

            if (m_property.Field == null)
                m_property.Field = new VariousData<double>(dField);
            else
                m_property.Field.Data = dField;

            if (m_property.Land == null)
                m_property.Land = new VariousData<double>(dLand);
            else
                m_property.Land.Data = dLand;

            if (m_property.ETC == null)
                m_property.ETC = new VariousData<double>(dETC);
            else
                m_property.ETC.Data = dETC;

            if (m_property.LandCost == null)
                m_property.LandCost = new VariousData<long>(nLandCost);
            else
                m_property.LandCost.Data = nLandCost;

            if (m_property.ObjectCost == null)
                m_property.ObjectCost = new VariousData<long>(nObjectCost);
            else
                m_property.ObjectCost.Data = nObjectCost;

            if (m_property.AroundCost == null)
                m_property.AroundCost = new VariousData<long>(nAroundCost);
            else
                m_property.AroundCost.Data = nAroundCost;

            if (m_frmScheduleProperty != null && m_row != null)
            {
                m_frmScheduleProperty.UpdateRow(m_row, m_property, -1);
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void UpdateRow()
        {
            DataGridViewCell cell = GetCell<ImportanceData>();

            if (cell != null)
                cell.Value = string.Format("{0:F2}", m_property.Importance.Importance);
        }

        private bool CheckText(out double dWidth, out double dArea, out int nLength, out double dRiceField, out double dField, out double dLand, out double dETC,
                            out long nLandCost, out long nObjectCost, out long nAroundCost)
        {
            dWidth = dArea = dRiceField = dField = dLand = dETC = 0.0;
            nLandCost = nObjectCost = nAroundCost = 0;
            nLength = 0;

            if (!CheckDoubleTextBox(textBoxWidth, "도로폭은", out dWidth))
                return false;

            if (!CheckDoubleTextBox(textBoxArea, "결정면적은", out dArea))
                return false;

            if (!CheckIntTextBox(textBoxLength, "연장은", out nLength))
                return false;

            if (!CheckDoubleTextBox(textBoxRiceField, "지목현황(전)은", out dRiceField))
                return false;

            if (!CheckDoubleTextBox(textBoxField, "지목현황(답)은", out dField))
                return false;

            if (!CheckDoubleTextBox(textBoxLand, "지목현황(대지)은", out dLand))
                return false;

            if (!CheckDoubleTextBox(textBoxETC, "지목현황(기타)은", out dETC))
                return false;

            if (!CheckLongTextBox(textBoxLandCost, "토지보상비는", out nLandCost))
                return false;

            if (!CheckLongTextBox(textBoxObjectCost, "지장물 보상비는", out nObjectCost))
                return false;

            if (!CheckLongTextBox(textBoxAroundCost, "개략 공사비는", out nAroundCost))
                return false;

            return true;
        }

        private bool CheckDoubleTextBox(TextBox textBox, string strName, out double dData)
        {
            if (textBox.Text.Length == 0)
                dData = 0.0;
            else
            {
                if (!double.TryParse(textBox.Text, out dData))
                {
					string szMsg = strName + " 숫자가 아닙니다.";
                    UnE.Utility.UMessageBox.Show(this, szMsg, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    //MessageBox.Show(strName + " 숫자가 아닙니다.");
                    return false;
                }

                if (dData < 0.0)
                {
					string szMsg = strName + " 0보다 작은 숫자를 입력할 수 없습니다.";
                    UnE.Utility.UMessageBox.Show(this, szMsg, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    //MessageBox.Show(strName + " 0보다 작은 숫자를 입력할 수 없습니다.");
                    return false;
                }
            }

            return true;
        }

        private bool CheckLongTextBox(TextBox textBox, string strName, out long nData)
        {
            if (textBox.Text.Length == 0)
                nData = 0;
            else
            {
                if (!long.TryParse(textBox.Text, out nData))
                {
					string szMsg = strName + " 숫자가 아닙니다.";
                    UnE.Utility.UMessageBox.Show(this, szMsg, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //MessageBox.Show(strName + " 숫자가 아닙니다.");
                    return false;
                }

                if (nData < 0)
                {
					string szMsg = strName + " 0보다 작은 숫자를 입력할 수 없습니다.";
                    UnE.Utility.UMessageBox.Show(this, szMsg, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //MessageBox.Show(strName + " 0보다 작은 숫자를 입력할 수 없습니다.");
                    return false;
                }
            }

            return true;
        }

        private bool CheckIntTextBox(TextBox textBox, string strName, out int nData)
        {
            if (textBox.Text.Length == 0)
                nData = 0;
            else
            {
                if (!int.TryParse(textBox.Text, out nData))
                {
                    string szMsg = strName + " 숫자가 아닙니다.";
                    UnE.Utility.UMessageBox.Show(this, szMsg, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                if (nData < 0)
                {
                    string szMsg = strName + " 0보다 작은 숫자를 입력할 수 없습니다.";
                    UnE.Utility.UMessageBox.Show(this, szMsg, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnMillion_Click(object sender, EventArgs e)
        {
            if (sender == btnMillionLandCost)
            {
                textBoxLandCost.Text += "000000";
            }
            else if (sender == btnMillionObjectCost)
            {
                textBoxObjectCost.Text += "000000";
            }
            else if (sender == btnMillionAroundCost)
            {
                textBoxAroundCost.Text += "000000";
            }
        }

        private void FormScheduleDetail_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_closingWorker != null)
                m_closingWorker.OnClosing();

            if (m_panel != null)
            {
                m_panel.ScheduleDetailForm = null;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (msg.Msg == WindowMessage.WM_KEYDOWN ||
                msg.Msg == WindowMessage.WM_CHAR ||
                msg.Msg == WindowMessage.WM_SYSKEYDOWN)
            {
                if (keyData == Keys.F1)
                {
                    FormMain.Instance.ShowHelp();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private class SettingImportance_ScheduleDetail : FormImportance.SettingImportance
        {
            private Label m_label = null;
            private ImportanceData m_importanceData = null;

            public override ImportanceData Data
            {
                get { return m_importanceData; }
                set
                {
                    m_importanceData.CopyFrom(value);
                    m_label.Text = string.Format("{0:F2}", value.Importance);
                    /*if (m_property != null)
                    {
                        if (m_cell != null)
                            m_cell.Value = string.Format("{0:F2}", value.Importance);

                        if (value != null)
                            m_property.Importance.CopyFrom(value);
                    }*/
                }
            }

            public SettingImportance_ScheduleDetail(Label label, ImportanceData data)
            {
                m_label = label;
                m_importanceData = data;
            }

            public override void Close()
            {
            }
        }

        private class SettingLandNumber_ScheduleDetail : FormLandNumber.SettingLandNumber
        {
            private Label m_label = null;
            private List<LandAddressData> m_listData = null;

            public override List<LandAddressData> Data
            {
                get { return m_listData; }
                set
                {
                    m_label.Text = ScheduleProperty.GetFirstNLastLandAddressString(value);
                }
            }

            public SettingLandNumber_ScheduleDetail(Label label, List<LandAddressData> listData)
            {
                m_label = label;
                m_listData = listData;
            }

            public override void Close()
            {
            }
        }

        public interface IClosingWorker
        {
            void OnClosing();
        }

        private void checkBoxEdit_CheckedChanged(object sender, EventArgs e)
        {
            EnableControl(checkBoxEdit.Checked);
        }
    }
}
