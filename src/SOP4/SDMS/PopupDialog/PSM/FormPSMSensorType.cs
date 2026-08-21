using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using DBUtility;

namespace SDMS.PopupDialog
{
    public partial class FormPSMSensorType : Form
    {
        private class Month
        {
            private int data = 0;
            
            public int Value
            {
                get { return data; }
                set { data = value; }
            }

            public Month()
            {
            }

            public Month(int nMonth)
            {
                data = nMonth;
            }

            public override string ToString()
            {
                if (data <= 0)
                    return "사용자 정의";

                if (data % 12 == 0)
                    return (data / 12).ToString() + "년";

                return data.ToString() + "개월";
            }
        }

        private int[] PREV_DEFINED_MONTH = { 1, 2, 3, 4, 5, 6, 12, 24, 36, 60 };
        private Dictionary<string, int> m_dicLifeTimeMonth = null;

        private UnE.PSM.PSMSensorType m_resultData = null;
        private FormPSMSensorLifeTime m_frmParent = null;

        public UnE.PSM.PSMSensorType Result
        {
            get { return m_resultData; }
        }

        public FormPSMSensorType(FormPSMSensorLifeTime frmParent)
        {
            this.DoubleBuffered = true;
            InitializeComponent();
            m_frmParent = frmParent;
        }

        private void FormPSMSensorType_Load(object sender, EventArgs e)
        {
            InitComboBox();
        }

        private void InitComboBox()
        {
            cboLifeTime.Items.Clear();

            List<int> monthList = new List<int>();

            foreach (int nMonth in PREV_DEFINED_MONTH)
            {
                monthList.Add(nMonth);
            }

            m_dicLifeTimeMonth = GetPSMSensorTypeMonthList();

            if (m_dicLifeTimeMonth != null)
            {
                foreach (KeyValuePair<string, int> pair in m_dicLifeTimeMonth)
                {
                    if (!monthList.Contains(pair.Value))
                        monthList.Add(pair.Value);
                }
            }

            monthList.Sort();

            foreach (int nMonth in monthList)
            {
                cboLifeTime.Items.Add(new Month(nMonth));
            }

            cboLifeTime.Items.Add(new Month());
            cboLifeTime.SelectedIndex = 0;
        }

        // Key : Type 이름
        // Value : 사용기한(개월수)
        private Dictionary<string, int> GetPSMSensorTypeMonthList()
        {
            string strSQL = "Select TypeName, LifeTimeMonth from PSMSensorType";
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            Dictionary<string, int> dicLifeTimeMonth = new Dictionary<string, int>();
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                string strTypeName = WebDBManager.GetStringField(arrResult[i]);
                VariousData<int> month = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (strTypeName == null || month == null)
                    continue;

                dicLifeTimeMonth[strTypeName] = month.Data;
            }

            return dicLifeTimeMonth;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            string strTypeName = textBoxTypeName.Text.Trim();

            if (strTypeName.Length == 0)
            {
                MessageBox.Show("센서 타입명을 입력하세요.");
                textBoxTypeName.Focus();
                return;
            }

            WebDBManager dbMgr = FormMain.Instance.DBManager;
            bool isExist = IsExistTypeName(dbMgr, strTypeName);
            
            if (isExist)
            {
                if (MessageBox.Show("이미 존재하는 센서 타입명입니다.\r\n덮어쓰시겠습니까?.", "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                {
                    textBoxTypeName.Focus();
                    return;
                }
            }

            int nMonth = 0;
            Month month = (Month)cboLifeTime.SelectedItem;

            if (month.Value == 0)
            {
                string strUserDefinedMonth = textBoxUserDefined.Text.Trim();

                if (strUserDefinedMonth.Length == 0)
                {
                    MessageBox.Show("사용기한(개월수)을 입력하세요.");
                    textBoxUserDefined.Focus();
                    return;
                }

                if (int.TryParse(strUserDefinedMonth, out nMonth) == false || nMonth <= 0)
                {
                    MessageBox.Show("사용기한(개월수)는 0보다 큰 정수를 입력해야만 합니다.");
                    textBoxUserDefined.Focus();
                    return;
                }
            }
            else
                nMonth = month.Value;

            UnE.PSM.PSMSensorType sensorType = SaveSensorType(dbMgr, strTypeName, nMonth, isExist);

            if (sensorType == null)
            {
                MessageBox.Show("값을 DB에 저장할 수 없습니다.\r\n네트웍 상태를 확인해 주세요");
                return;
            }

            m_resultData = sensorType;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void btnRemoveType_Click(object sender, EventArgs e)
        {
            string strTypeName = textBoxTypeName.Text.Trim();

            if (strTypeName.Length == 0)
            {
                MessageBox.Show("지우고자 하는 센서 타입명을 입력하세요");
                textBoxTypeName.Focus();
                return;
            }

            if (MessageBox.Show("[" + strTypeName + "] 및 연관 데이터를 삭제합니다.\r\n계속 진행하시겠습니까?", "주의", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                if (RemoveSensorType(strTypeName))
                {
                    m_dicLifeTimeMonth.Remove(strTypeName);
                    UnE.PSM.PSMSensorType sensorType = PSMManager.Instance.RemoveSensorType(strTypeName);

                    if (sensorType != null)
                    {
                        if (FormPSMList.Instance != null && FormPSMList.Instance.IsDisposed == false)
                        {
                            NetworkManager.Instance.SendRefreshSensorLifeTime();
                            //FormPSMList.Instance.CheckPSMSensorLifeTime();
                        }

                        m_frmParent.RemoveSensorType(sensorType);
                    }

                    // PSM List 갱신
                    MessageBox.Show("삭제되었습니다.");
                }
                else
                    MessageBox.Show("삭제할 수 없습니다.\r\n네트웍 상태를 확인해 주세요");
            }
        }

        private bool RemoveSensorType(string strTypeName)
        {
            string strSQL = "Update PSMSensor set SensorTypeName = NULL where SensorTypeName = '" + strTypeName + "'";

            if (FormMain.Instance.DBManager.GetResultData(strSQL, 0) == null)
                return false;

            strSQL = "Delete from PSMSensorType where TypeName = '" + strTypeName + "'";

            if (FormMain.Instance.DBManager.GetResultData(strSQL, 0) == null)
                return false;

            return true;
        }

        private UnE.PSM.PSMSensorType SaveSensorType(WebDBManager dbMgr, string strTypeName, int nMonth, bool isExist)
        {
            string strSQL = "";

            if (isExist)
            {
                strSQL = string.Format("Update PSMSensorType Set LifeTimeMonth = {0} where TypeName = '{1}'",
                    nMonth, strTypeName);
            }
            else
            {
                strSQL = string.Format("Insert into PSMSensorType (TypeName, LifeTimeMonth) values ('{0}', {1})",
                    strTypeName, nMonth);
            }
            
            if (dbMgr.GetResultData(strSQL, 0) != null)
            {
                UnE.PSM.PSMSensorType sensorType = PSMManager.Instance.AddPSMSensorType(strTypeName, nMonth);
                return sensorType;
            }

            return null;
        }

        private bool IsExistTypeName(WebDBManager dbMgr, string strTypeName)
        {
            if (m_dicLifeTimeMonth != null && m_dicLifeTimeMonth.ContainsKey(strTypeName))
                return true;

            // 편집하는 사이에 PSMSensorType이 변경되었을지 모르니 다시 DB를 검사한다.
            string strSQL = "Select LifeTimeMonth from PSMSensorType where TypeName = '" + strTypeName + "'";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            if (arrResult.Count > 0)
            {
                VariousData<int> month = WebDBManager.GetIntField(arrResult[0].ToString());

                if (month != null)
                {
                    PSMManager.Instance.AddPSMSensorType(strTypeName, month.Data);
                    return true;
                }
            }

            return false;
        }

        private void cboLifeTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboLifeTime.SelectedIndex == cboLifeTime.Items.Count - 1)
                lblUserDefined.Visible = textBoxUserDefined.Visible = lblMonth.Visible = true;
            else
                lblUserDefined.Visible = textBoxUserDefined.Visible = lblMonth.Visible = false;
        }
    }
}
