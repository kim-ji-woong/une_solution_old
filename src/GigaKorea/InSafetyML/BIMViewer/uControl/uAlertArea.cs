using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BIMViewer.BIM;

namespace BIMViewer.uControl
{
    public partial class uAlertArea : UserControl
    {
        private AlertArea m_uAlertArea = null;
        private string m_strAlertAreaName = "";     // 경계구역 이름 변경 체크
        private BIM.Level m_level = null;

        string m_strGroup = "";
        string m_strType = "";

        public uAlertArea()
        {
            InitializeComponent();
        }

        public void SetAlertAreaData(AlertArea alertArea, string strLevelName, BIM.Level level)
        {
            //층수, 객체번호, 객체명
            m_uAlertArea = alertArea;
            m_level = level;
            txtObject.Text = " 경계구역";
            txtID.Text = m_uAlertArea.XMLID;
            lblFloor.Text = strLevelName;

            string strName = m_uAlertArea.Name;
            int i;

            i = strName.IndexOf("_");
            txtAlertAreaName.Text = " " + strName.Substring(0, i);
            m_strAlertAreaName = " " + strName.Substring(0, i);

            InitGroupList();
            InitTypeList();

            cmbGroup.SelectedIndex = 0;
            cmbType.SelectedIndex = 0;
            txtGroup.Enabled = true;
            txtType.Enabled = true;

            foreach (Property pro in alertArea.Properties)
            {
                if (pro.Name == "grouping")
                {
                    txtGroup.Text = pro.Value;
                    cmbGroup.SelectedItem = pro.Value;
                    m_strGroup = pro.Value;

                    if (pro.Value != "")
                        txtGroup.Enabled = false;
                }
                else if (pro.Name == "alertAreaType")
                {
                    txtType.Text = pro.Value;
                    cmbType.SelectedItem = pro.Value;
                    m_strType = pro.Value;

                    if (pro.Value != "")
                        txtType.Enabled = false;
                }
            }

        }

        public void UpdateUserData()
        {
            bool bUnderChk = false;
            bool bChangeChk = false;

            string strUnder = "B";
            string strFloor = lblFloor.Text;

            bUnderChk = strFloor.Contains(strUnder);

            // 경계구역명, 그룹
            int i;
            i = lblFloor.Text.IndexOf(" ");

            if (!bUnderChk)
                m_uAlertArea.Name = txtAlertAreaName.Text.Trim() + "_level" + lblFloor.Text.Substring(2, lblFloor.Text.Length - i - 1) + "F";
            else
                m_uAlertArea.Name = txtAlertAreaName.Text.Trim() + "_levelB" + lblFloor.Text.Substring(2, lblFloor.Text.Length - i - 1) + "F";

            foreach (Property prop in m_uAlertArea.Properties)
            {
                if (prop.Name == "grouping")
                {
                    prop.Value = txtGroup.Text;
                    FormMain.Instance.AddAlertAreaGroup(txtGroup.Text);
                }
                else if (prop.Name == "alertAreaType")
                {
                    prop.Value = txtType.Text;
                    FormMain.Instance.AddAlertAreaType(txtType.Text);
                }
            }

            // 변경 유무 체크
            if (m_strAlertAreaName != txtAlertAreaName.Text)
                bChangeChk = true;
            if (m_strGroup != txtGroup.Text)
                bChangeChk = true;
            if (m_strType != txtType.Text)
                bChangeChk = true;

            // 변경이 되었다면 Level XML ID 수정
            if (bChangeChk == true)
            {
                if (!bUnderChk)
                    m_level.XMLID = "level" + lblFloor.Text.Substring(2, lblFloor.Text.Length - i - 1) + "F";
                else
                    m_level.XMLID = "levelB" + lblFloor.Text.Substring(2, lblFloor.Text.Length - i - 1) + "F";
            }
        }

        private void InitGroupList()
        {
            cmbGroup.Items.Clear();

            List<string> listGroup = FormMain.Instance.AlertAreaGroup;

            foreach (string strGroup in listGroup)
            {
                cmbGroup.Items.Add(strGroup);
            }
        }

        private void InitTypeList()
        {
            cmbType.Items.Clear();

            List<string> listType = FormMain.Instance.AlertAreaType;

            foreach (string strType in listType)
            {
                cmbType.Items.Add(strType);
            }
        }

        private void cmbGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGroup.SelectedIndex == 0)
            {
                txtGroup.Text = "";
                txtGroup.Enabled = true;
            }
            else
            {
                txtGroup.Text = cmbGroup.Text;
                txtGroup.Enabled = false;
            }
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbType.SelectedIndex == 0)
            {
                txtType.Text = "";
                txtType.Enabled = true;
            }
            else
            {
                txtType.Text = cmbType.Text;
                txtType.Enabled = false;
            }
        }
    }
}
