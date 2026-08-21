using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;

namespace LostArticle
{
    public partial class FormMain : Form
    {
        private WebDBManager m_dbMgr = null;
        private Status m_currentStatus = null;

        public FormMain()
        {
            InitializeComponent();

            SetDBManager();
            ReadData();
        }

        private void ReadData()
        {
            if (m_dbMgr != null)
            {
                m_currentStatus = Status.ReadData(m_dbMgr);
                List<Article> articles = Article.ReadData(m_dbMgr, m_currentStatus);

                if (m_currentStatus != null && articles != null)
                {
                    m_currentStatus.Articles.AddRange(articles);
                }

                SetData();
            }
        }

        private void SetData()
        {
            if (m_currentStatus != null)
            {
                SetStatus(textBoxDeadCount, m_currentStatus.DeadCount, labelPrevDeadCount, labelDeadCountUnit);
                SetStatus(textBoxInjuryCount, m_currentStatus.InjuryCount, labelPrevInjuryCount, labelInjuryUnit);
                SetStatus(textBoxLostCount, m_currentStatus.LostCount, labelPrevLostCount, labelLostUnit);
                SetStatus(textBoxTankTemperature, m_currentStatus.TankTemperature, labelPrevTankTemperature, labelTankTemperatureUnit);
            }
        }

        private void SetStatus(TextBox textBox, int data, Label labelPrev, Label labelUnit)
        {
            textBox.Text = data.ToString();
            labelPrev.Text = string.Format("{0} {1}", data, labelUnit.Text);
        }

        private void SetStatus(TextBox textBox, VariousData<float> data, Label labelPrev, Label labelUnit)
        {
            if (data != null && data.Data >= 0)
            {
                textBox.Text = data.Data.ToString();
                labelPrev.Text = string.Format("{0:F1} {1}", data.Data, labelUnit.Text);
            }
            else
            {
                textBox.Text = "";
                labelPrev.Text = "-";
            }
        }

        private void SetDBManager()
        {
            string strSiteID = System.Configuration.ConfigurationManager.AppSettings["siteid"].ToString();
            string strWebServerURL = System.Configuration.ConfigurationManager.AppSettings["webserver"].ToString();
            string strDBName = System.Configuration.ConfigurationManager.AppSettings["dbname"].ToString();
            string strDBType = System.Configuration.ConfigurationManager.AppSettings["dbtype"].ToString();

            int nDBType;

            if (int.TryParse(strDBType, out nDBType) == false)
                return;

            int nSiteID = 0;

            if (int.TryParse(strSiteID, out nSiteID) == false)
                return;

            WebDBManager dbMgr = new WebDBManager(nSiteID);
            dbMgr.WebServerURL = strWebServerURL;
            dbMgr.DatabaseName = strDBName;
            dbMgr.DatabaseType = (WebDBManager.DBType)nDBType;

            m_dbMgr = dbMgr;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            textBoxBody.Focus();
        }

        private void btnApplyStatus_Click(object sender, EventArgs e)
        {
            if (IsChanged() == false)
            {
                MessageBox.Show("변경된 사항이 없습니다.");
                return;
            }

            TextBox textBox;
            string strErrorMessage;

            if (IsEmptyStatus(out textBox, out strErrorMessage))
            {
                textBox.Focus();
                MessageBox.Show(strErrorMessage);
                return;
            }

            ApplyStatus();
        }

        private void btnSendArticle_Click(object sender, EventArgs e)
        {
            if (m_currentStatus == null)
            {
                MessageBox.Show("인명피해 및 탱크정보가 먼저 설정되어야만 합니다.");
                return;
            }

            string strTitle = textBoxTitle.Text.Trim();

            if (strTitle.Length == 0)
            {
                textBoxTitle.Focus();
                MessageBox.Show("제목은 반드시 입력해야 합니다.");
                return;
            }

            string strMessage = textBoxBody.Text.Trim();

            if (Article.SaveDB(m_dbMgr, m_currentStatus, strTitle, strMessage))
            {
                textBoxTitle.Text = textBoxBody.Text = "";
                MessageBox.Show("메시지 전송이 완료되었습니다.");
            }
            else
                MessageBox.Show("메시지 전송이 실패하였습니다.");
        }

        private void ApplyStatus()
        {
            int nDeadCount = int.Parse(textBoxDeadCount.Text.Trim());
            int nInjuryCount = int.Parse(textBoxInjuryCount.Text.Trim());
            int nLostCount = int.Parse(textBoxLostCount.Text.Trim());
            VariousData<float> tankTemperature = null;

            string strTemp = textBoxTankTemperature.Text.Trim();

            if (strTemp.Length > 0)
                tankTemperature = new VariousData<float>(float.Parse(strTemp));

            if (Status.SaveDB(m_dbMgr, ref m_currentStatus, nDeadCount, nInjuryCount, nLostCount, tankTemperature))
            {
                SetPrev(textBoxDeadCount, labelDeadCountUnit, labelPrevDeadCount);
                SetPrev(textBoxInjuryCount, labelInjuryUnit, labelPrevInjuryCount);
                SetPrev(textBoxLostCount, labelLostUnit, labelPrevLostCount);
                SetPrev(textBoxTankTemperature, labelTankTemperatureUnit, labelPrevTankTemperature);
                MessageBox.Show("전송이 완료되었습니다.");
            }
            else
                MessageBox.Show("전송이 실패하였습니다.");
        }

        private void SetPrev(TextBox textBox, Label labelUnit, Label labelPrev)
        {
            string strValue = textBox.Text.Trim();

            if (strValue.Length > 0)
                labelPrev.Text = strValue + " " + labelUnit.Text;
            else
                labelPrev.Text = "-";
        }

        private bool IsChanged()
        {
            if (IsChanged(textBoxDeadCount, labelPrevDeadCount, labelDeadCountUnit))
                return true;

            if (IsChanged(textBoxInjuryCount, labelPrevInjuryCount, labelInjuryUnit))
                return true;

            if (IsChanged(textBoxLostCount, labelPrevLostCount, labelLostUnit))
                return true;

            if (IsChanged(textBoxTankTemperature, labelPrevTankTemperature, labelTankTemperatureUnit))
                return true;

            return false;
        }

        private bool IsChanged(TextBox textBox, Label labelPrev, Label labelUnit)
        {
            string strCurrent = textBox.Text.Trim();
            string strPrev = labelPrev.Text.Replace(labelUnit.Text, "").Trim();
            return !strCurrent.Equals(strPrev);
        }

        private bool IsEmptyStatus(out TextBox textBox, out string strErrorMessage)
        {
            textBox = null;
            strErrorMessage = "";

            if (IsEmpty(textBoxDeadCount, "사망자 숫자", ref textBox, ref strErrorMessage))
                return true;
            if (IsEmpty(textBoxInjuryCount, "부상자 숫자", ref textBox, ref strErrorMessage))
                return true;
            if (IsEmpty(textBoxLostCount, "실종자 숫자", ref textBox, ref strErrorMessage))
                return true;
            if (IsValidFloat(textBoxTankTemperature, "탱크온도", ref textBox, ref strErrorMessage) == false)
                return true;

            return false;
        }

        private bool IsEmpty(TextBox textBox, string strTag, ref TextBox _textBox, ref string strErrorMessage)
        {
            string strText = textBox.Text.Trim();

            if (strText.Length == 0)
            {
                _textBox = textBox;
                strErrorMessage = strTag + "를 입력하세요.";
                return true;
            }

            int nCount;

            if (int.TryParse(strText, out nCount) == false || nCount < 0)
            {
                _textBox = textBox;
                strErrorMessage = strTag + "는 0 또는 0 보다 큰 정수형태의 값이어야 합니다.";
                return true;
            }

            return false;
        }

        private bool IsValidFloat(TextBox textBox, string strTag, ref TextBox _textBox, ref string strErrorMessage)
        {
            string strText = textBox.Text.Trim();

            if (strText.Length == 0)
            {
                return true;
            }

            float fData;

            if (float.TryParse(strText, out fData) == false)
            {
                _textBox = textBox;
                strErrorMessage = strTag + "는 숫자형태의 값이어야 합니다.";
                return false;
            }

            return true;
        }

        private void btnInitialize_Click(object sender, EventArgs e)
        {
            if (m_currentStatus == null)
                return;

            if (MessageBox.Show("모든 데이터를 초기화 합니다.\r\n계속 하시겠습니까?", "확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (Status.Initialize(m_dbMgr, m_currentStatus))
                {
                    textBoxTitle.Text = textBoxBody.Text = "";
                    textBoxDeadCount.Text = textBoxInjuryCount.Text = textBoxLostCount.Text = "0";
                    textBoxTankTemperature.Text = "";

                    SetPrev(textBoxDeadCount, labelDeadCountUnit, labelPrevDeadCount);
                    SetPrev(textBoxInjuryCount, labelInjuryUnit, labelPrevInjuryCount);
                    SetPrev(textBoxLostCount, labelLostUnit, labelPrevLostCount);
                    SetPrev(textBoxTankTemperature, labelTankTemperatureUnit, labelPrevTankTemperature);

                    MessageBox.Show("초기화 되었습니다.");
                }
                else
                    MessageBox.Show("초기화에 실패하였습니다.");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
