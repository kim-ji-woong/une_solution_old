using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SMS_P_Simulator
{
    public partial class Form1 : Form
    {
        private int m_nTotalByteLength = 0;
        private int m_nSendMessageCount = 0;
        private int m_nMessageID = 0;
        private ArrayList m_arrMessageList = new ArrayList();

        ezSMSComponent.ISMS m_sms = new ezSMSComponent.SMS();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxDisasterCategory.SelectedIndex = 0;
            comboBoxActionStep.SelectedIndex = 0;
            comboBoxActionSubStep.SelectedIndex = 0;
            comboBoxLocation.SelectedIndex = 0;

            radioLess80.Checked = true;
            radioBeginSOP.Checked = true;

            m_sms.ServiceCode = "020026C9FCC7C39E41A88C2CF52D00D7BAA6";
            ezSMSComponent.LoginInfo login = m_sms.Login("121.254.175.25", 4545, "unes", "unes0101");
        }

        private void comboBoxDisasterCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = comboBoxDisasterCategory.SelectedIndex;

            if (nSelectedIndex == 0)
                SetNatureDisaster();
            else
            {
                comboBoxLocation.Enabled = true;

                if (nSelectedIndex == 1)
                    SetFireDisaster();
                else if (nSelectedIndex == 2)
                    SetPollutionDisaster();
                else if (nSelectedIndex == 3)
                    SetTerrorDisaster();
                else if (nSelectedIndex == 4)
                    SetSavingLifeDisaster();
                else if (nSelectedIndex == 5)
                    SetEtcDisaster();
            }
        }

        private void SetNatureDisaster()
        {
            comboBoxDisasterSubCategory.Items.Clear();

            comboBoxDisasterSubCategory.Items.Add("태풍");
            comboBoxDisasterSubCategory.Items.Add("지진");
            comboBoxDisasterSubCategory.Items.Add("폭설");
            comboBoxDisasterSubCategory.Items.Add("침수");
            comboBoxDisasterSubCategory.Items.Add("일반재해");

            comboBoxLocation.Enabled = false;

            comboBoxDisasterSubCategory.SelectedIndex = 0;
        }

        private void SetFireDisaster()
        {
            comboBoxDisasterSubCategory.Items.Clear();

            comboBoxDisasterSubCategory.Items.Add("화재");
            
            comboBoxDisasterSubCategory.SelectedIndex = 0;
        }

        private void SetPollutionDisaster()
        {
            comboBoxDisasterSubCategory.Items.Clear();

            comboBoxDisasterSubCategory.Items.Add("오염");
            
            comboBoxDisasterSubCategory.SelectedIndex = 0;
        }

        private void SetTerrorDisaster()
        {
            comboBoxDisasterSubCategory.Items.Clear();

            comboBoxDisasterSubCategory.Items.Add("테러");
            
            comboBoxDisasterSubCategory.SelectedIndex = 0;
        }

        private void SetSavingLifeDisaster()
        {
            comboBoxDisasterSubCategory.Items.Clear();

            comboBoxDisasterSubCategory.Items.Add("119상황");
            
            comboBoxDisasterSubCategory.SelectedIndex = 0;
        }

        private void SetEtcDisaster()
        {
            comboBoxDisasterSubCategory.Items.Clear();

            comboBoxDisasterSubCategory.Items.Add("SOP상황");

            comboBoxDisasterSubCategory.SelectedIndex = 0;
        }

        private void btnMakeMessage_Click(object sender, EventArgs e)
        {
            m_nSendMessageCount = 0;
            m_arrMessageList.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            if (radioBeginSOP.Checked)
                MakeBeginMessage();
            else if (radioEndSOP.Checked)
                MakeEndMessage();
            else if (radioNormal.Checked)
                MakeNormal();
            else if (radioInternal.Checked)
                MakeInternal();
            else if (radioMission.Checked)
                MakeMission();
        }

        private int FixedID()
        {
            int num = 0;

            /*if (comboBoxActionSubStep.SelectedIndex >= 0)
                num = 1000 * comboBoxActionSubStep.SelectedIndex;

            if (comboBoxActionStep.SelectedIndex >= 0)
                num += 100 * comboBoxActionStep.SelectedIndex;*/

            if (comboBoxDisasterSubCategory.SelectedIndex >= 0)
                num += 10 * comboBoxDisasterSubCategory.SelectedIndex;

            if (comboBoxDisasterCategory.SelectedIndex >= 0)
                num += comboBoxDisasterCategory.SelectedIndex + 1;

            return num;
        }

        private string MakeID()
        {
            //string strID = To64String(++m_nMessageID);
            string strID = To64String(FixedID());

            int nLen = strID.Length;

            for (int i = 0; i < 3 - nLen; i++)
                strID = "0" + strID;

            return strID;
        }

        private void MakeMission()
        {
            dataGridView1.Columns.Add("colID", "ID");
            dataGridView1.Columns.Add("colType", "구분");
            dataGridView1.Columns.Add("colTime", "임무발생시간");
            dataGridView1.Columns.Add("colMissionCount", "임무개수");
            dataGridView1.Columns.Add("colMission", "임무내용");

            string strID = MakeID();
            string strType = To64String(4);
            string strCurrentTime = GetCurrentTimeString();
            string strProcessingTime = "000";
            string strMissionCount = "01";

            string strCheckedItemCount = "00";
            string strMission = "임무";

            string strMessage = strCurrentTime + strProcessingTime + strMissionCount + "\t";

            DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = strID;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strType;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strCurrentTime;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strMissionCount;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strMission;
            row.Cells.Add(cell);

            if (checkBoxUseItem.Checked)
            {
                dataGridView1.Columns.Add("colCategory", "대분류");
                dataGridView1.Columns.Add("colSubCategory", "중분류");
                dataGridView1.Columns.Add("colCheckedItem", "점검내용");
                dataGridView1.Columns.Add("colItemCount", "개수");

                strCheckedItemCount = "01";
                string strCategory = "-", strSubCategory = "-", strCheckedItem = "-";
                string strCount = "00";

                if (radioOver80.Checked)
                {
                    strCategory = "설비점검";
                    strSubCategory = "소방설비점검";
                    strCheckedItem = "소화기 및 소화전 이상 유무 점검";
                }

                strMessage += strCheckedItemCount + "\t" + strMission + "\t" + strCategory + "\t" + strSubCategory + "\t" + strCheckedItem + "\t" + strCount + "\t";

                cell = new DataGridViewTextBoxCell();
                cell.Value = strCategory;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = strSubCategory;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = strCheckedItem;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = strCount;
                row.Cells.Add(cell);
            }
            else
                strMessage += strCheckedItemCount + "\t" + strMission + "\t";

            dataGridView1.Rows.Add(row);

            MakeMessageList(strID, strType, strMessage);
        }

        private void MakeInternal()
        {
            dataGridView1.Columns.Add("colID", "ID");
            dataGridView1.Columns.Add("colType", "구분");
            dataGridView1.Columns.Add("colEvent", "SOP 이벤트");
            dataGridView1.Columns.Add("colTime", "시작시간");

            if (comboBoxDisasterCategory.SelectedIndex != 0)
            {
                dataGridView1.Columns.Add("colPlace", "재난발생위치");
                dataGridView1.Columns.Add("colFloor", "층Index");
                dataGridView1.Columns.Add("colX", "X");
                dataGridView1.Columns.Add("colY", "Y");
            }

            string strMessage = "";

            string strEvent = comboBoxDisasterSubCategory.Text + "/" + comboBoxActionStep.Text;
            if (comboBoxActionSubStep.SelectedIndex != 0)
                strEvent += "/" + comboBoxActionSubStep.Text;

            string strID = MakeID();

            string strType = To64String(11);

            strMessage += strEvent + "\t";

            string strCurrentTime = GetCurrentTimeString();
            strMessage += strCurrentTime + "\t";

            //string strPlace = "1,2호기";
            string strPlace = comboBoxLocation.Text;

            if (radioOver80.Checked)
                strPlace = "대구광역시 달서구 달구벌대로 1095 계명대학교 첨단산업지원센터 207호";

            strMessage += strPlace + "\t";

            DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = strID;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strType;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strEvent;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strCurrentTime;
            row.Cells.Add(cell);

            if (comboBoxDisasterCategory.SelectedIndex != 0)
            {
                string strFloor = "1";
                string strXPos = To64String(100000);
                string strYPos = To64String(200000);

                strMessage += strFloor + "\t" + strXPos + "\t" + strYPos + "\t";

                cell = new DataGridViewTextBoxCell();
                cell.Value = strPlace;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "1";
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = To64String(100000);
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = To64String(200000);
                row.Cells.Add(cell);
            }

            dataGridView1.Rows.Add(row);

            MakeMessageList(strID, strType, strMessage);
        }

        private void MakeNormal()
        {
            dataGridView1.Columns.Add("colID", "ID");
            dataGridView1.Columns.Add("colType", "구분");
            dataGridView1.Columns.Add("colTime", "공지사항");

            string strID = MakeID();
            string strType = To64String(3);

            string strMessage = "공지사항";

            if (radioOver80.Checked)
            {
                strMessage = "전직원에게 알려드립니다. 2013년 1월 1일 전국적인 폭설이 예상됩니다. 전 직원께서는 폭설예보에 대비한 조치사항을 숙지하시어 분담업무를 수행하시기 바랍니다.";
            }

            DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = strID;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strType;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strMessage;
            row.Cells.Add(cell);

            dataGridView1.Rows.Add(row);

            MakeMessageList(strID, strType, strMessage + "\t");
        }

        private void MakeEndMessage()
        {
            dataGridView1.Columns.Add("colID", "ID");
            dataGridView1.Columns.Add("colType", "구분");
            dataGridView1.Columns.Add("colTime", "시작시간");

            string strID = MakeID();
            //string strMessageNum = "01";
            string strType = To64String(20);
            string strCurrentTime = GetCurrentTimeString();

            //string strMessage = strID + strMessageNum + strType + "\t" + strCurrentTime + "\t";
            //m_arrMessageList.Add(strMessage);
            string strMessage = strCurrentTime + "\t";
            MakeMessageList(strID, strType, strMessage);

            DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = strID;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strType;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strCurrentTime;
            row.Cells.Add(cell);

            dataGridView1.Rows.Add(row);
        }

        private void MakeBeginMessage()
        {
            dataGridView1.Columns.Add("colID", "ID");
            //dataGridView1.Columns.Add("colNum", "메시지 번호");
            dataGridView1.Columns.Add("colType", "구분");
            dataGridView1.Columns.Add("colEvent", "SOP 이벤트");
            dataGridView1.Columns.Add("colTime", "시작시간");

            if (comboBoxDisasterCategory.SelectedIndex != 0)
            {
                dataGridView1.Columns.Add("colPlace", "재난발생위치");
                dataGridView1.Columns.Add("colFloor", "층Index");
                dataGridView1.Columns.Add("colX", "X");
                dataGridView1.Columns.Add("colY", "Y");
            }

            string strMessage = "";

            string strEvent = comboBoxDisasterSubCategory.Text + "/" + comboBoxActionStep.Text;
            if (comboBoxActionSubStep.SelectedIndex != 0)
                strEvent += "/" + comboBoxActionSubStep.Text;

            string strID = MakeID();
           
            if (strID.Length < 3)
            {
                for (int i = 0; i < 3 - strID.Length; i++)
                    strID = "0" + strID;
            }

            string strType = To64String(10);
            
            strMessage += strEvent + "\t";

            string strCurrentTime = GetCurrentTimeString();
            strMessage += strCurrentTime + "\t";

            //string strPlace = "1,2호기";
            string strPlace = comboBoxLocation.Text;

            if (radioOver80.Checked)
                strPlace = "대구광역시 달서구 달구벌대로 1095 계명대학교 첨단산업지원센터 207호";

            strMessage += strPlace + "\t";

            DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = strID;
            row.Cells.Add(cell);

            /*cell = new DataGridViewTextBoxCell();
            cell.Value = strMessageNum;
            row.Cells.Add(cell);*/

            cell = new DataGridViewTextBoxCell();
            cell.Value = strType;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strEvent;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strCurrentTime;
            row.Cells.Add(cell);

            if (comboBoxDisasterCategory.SelectedIndex != 0)
            {
                string strFloor = "1";
                string strXPos = To64String(100000);
                string strYPos = To64String(200000);

                strMessage += strFloor + "\t" + strXPos + "\t" + strYPos + "\t";

                cell = new DataGridViewTextBoxCell();
                cell.Value = strPlace;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "1";
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = To64String(100000);
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = To64String(200000);
                row.Cells.Add(cell);
            }
            
            dataGridView1.Rows.Add(row);
            MakeMessageList(strID, strType, strMessage);
        }

        private string GetCurrentTimeString()
        {
            DateTime time = DateTime.Now;

            int nTime = time.Year - 2000;
            nTime = nTime * 100 + time.Month;
            nTime = nTime * 100 + time.Day;
            nTime = nTime * 100 + time.Hour;
            nTime = nTime * 100 + time.Minute;

            return To64String(nTime);
        }

        private int MakeMessageList(string strID, string strType, string strMessage)
        {
            // ID(3) + Num(2) + 구분(1) + 탭문자(1)
            //int nHeaderLength = 7;

            int nByteLength = 0, nTotalByteLength = 0;
            int nMessageNum = 0, nBeginIndex = 0;
            int nLen = strMessage.Length;

            for (int i = 0; i < nLen; i++)
            {
                if (strMessage.ElementAt(i) < 256)
                    nByteLength++;
                else
                    nByteLength += 2;

                if ((nByteLength == 72 && (i < nLen - 1 && strMessage.ElementAt(i + 1) >= 256)) ||
                    nByteLength == 73)
                {
                    string strMessageNum = To64String(++nMessageNum);
                    if (strMessageNum.Length < 2)
                        strMessageNum = "0" + strMessageNum;

                    string strMsg = strID + strMessageNum + strType + "\t" + strMessage.Substring(nBeginIndex, i - nBeginIndex + 1);
                    nBeginIndex = i + 1;

                    m_arrMessageList.Add(strMsg);
                    nTotalByteLength += nByteLength + 7;

                    nByteLength = 0;
                }
            }

            if (nBeginIndex < nLen)
            {
                string strMessageNum = To64String(++nMessageNum);
                if (strMessageNum.Length < 2)
                    strMessageNum = "0" + strMessageNum;

                string strMsg = strID + strMessageNum + strType + "\t" + strMessage.Substring(nBeginIndex);
                m_arrMessageList.Add(strMsg);
                nTotalByteLength += nByteLength + 7;
            }

            int nMessageCount = m_arrMessageList.Count;

            labelStatus.Visible = true;
            labelStatus.Text = string.Format("0 / {0}\r\n(Total {1} Byte)", nMessageCount, nTotalByteLength);
            m_nTotalByteLength = nTotalByteLength;

            return nMessageCount;
        }

        private void radioMission_CheckedChanged(object sender, EventArgs e)
        {
            //checkBoxUseItem.Visible = true;
        }

        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxUseItem.Visible = false;
        }

        static private string To64String(int nData)
        {
            if (nData == 0)
                return "0";

            if (nData < 0)
                throw new Exception("음수는 64진법 문자열로 변환할 수 없습니다.");

            // double의 한계 오차 때문에 0.1을 더한다.
            double dLog = System.Math.Log((double)nData, 64.0) + 0.1;
            int nLog = (int)dLog;

            int nLogResult = 1;

            for (int i = 0; i < nLog; i++)
                nLogResult *= 64;

            if (nLogResult > nData)
            {
                nLog--;
                nLogResult /= 64;
            }

            if (nLog < 0)
                throw new Exception("정수값을 문자열로 변환할 수 없습니다.");

            string strResult = "";

            for (int i = 0; i <= nLog; i++)
            {
                int n = nData % 64;
                nData /= 64;

                strResult = To64Char(n) + strResult;
            }

            return strResult;
        }

        static private char To64Char(int nData)
        {
            if (nData < 10)
                return (char)('0' + nData);
            else if (nData < 36)
                return (char)('a' + nData - 10);
            else if (nData < 62)
                return (char)('A' + nData - 36);
            else if (nData == 62)
                return '(';
            else if (nData == 63)
                return ')';
            else
                throw new Exception("64진수 문자로 변환할 수 없는 값입니다.");

            return '0';
        }

        private bool SendMessage(ezSMSComponent.Receivers receiver)
        {
            ezSMSComponent.SendResults results = m_sms.SendSMS(textBoxCaller.Text, receiver);

            foreach (ezSMSComponent.SendResult result in results)
            {
                if (result.Result != ezSMSComponent.EZSMS_RESULT.EZSMS_SUCCEEDED)
                {
                    MessageBox.Show("메시지 전송에 실패하였습니다.");
                    return false;
                }
            }

            return true;
        }

        private void btnTrans_Click(object sender, EventArgs e)
        {
            if (textBoxPhoneNumber.Text.Length == 0)
            {
                MessageBox.Show("수신 전화번호를 입력해 주세요");
                return;
            }
            else if (textBoxCaller.Text.Length == 0)
            {
                MessageBox.Show("발신 전화번호를 입력해 주세요");
                return;
            }

            int nMessageCount = m_arrMessageList.Count;

            if (nMessageCount == 0)
            {
                m_nSendMessageCount = 0;
                MessageBox.Show("메시지 생성을 눌러 주세요");
                return;
            }
            else if (nMessageCount == 1)
            {
                ezSMSComponent.Receivers receiver = m_sms.CreateReceivers();
                receiver.AddDirect(textBoxPhoneNumber.Text, (string)m_arrMessageList[0], ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, DateTime.Now);

                if (SendMessage(receiver))
                    labelStatus.Text = string.Format("1 / {0}\r\n(Total {1} Byte)", 1, m_nTotalByteLength);

                m_nSendMessageCount = 0;
            }
            else
            {
                ezSMSComponent.Receivers receiver = m_sms.CreateReceivers();

                if (checkBoxOneByOne.Checked)
                {
                    receiver.AddDirect(textBoxPhoneNumber.Text, (string)m_arrMessageList[m_nSendMessageCount++], ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, DateTime.Now);

                    if (SendMessage(receiver))
                        labelStatus.Text = string.Format("{0} / {1}\r\n(Total {2} Byte)", m_nSendMessageCount, nMessageCount, m_nTotalByteLength);

                    if (m_nSendMessageCount >= nMessageCount)
                        m_nSendMessageCount = 0;
                }
                else
                {
                    for (int i = m_nSendMessageCount; i < nMessageCount; i++)
                    {
                        receiver.AddDirect(textBoxPhoneNumber.Text, (string)m_arrMessageList[i], ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, DateTime.Now);
                    }

                    if (SendMessage(receiver))
                        labelStatus.Text = string.Format("{0} / {0}\r\n(Total {1} Byte)", nMessageCount, m_nTotalByteLength);

                    m_nSendMessageCount = 0;
                }
            }
        }

        private void comboBoxLocation_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateLocation();
            }
        }

        private void UpdateLocation()
        {
            string str = comboBoxLocation.Text;

            foreach (string strItem in comboBoxLocation.Items)
            {
                if (str == strItem)
                    return;
            }

            comboBoxLocation.Items.Add(str);
        }
    }
}
