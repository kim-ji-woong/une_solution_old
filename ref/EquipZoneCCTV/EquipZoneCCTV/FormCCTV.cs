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
using System.Collections;

namespace EquipZoneCCTV
{
    public partial class FormCCTV : Form
    {
        private WebDBManager m_dbMgr = null;
        private int m_nCCTVID = -1;

        public FormCCTV(WebDBManager dbMgr)
        {
            InitializeComponent();
            m_dbMgr = dbMgr;
            labelCameraName.Text = "";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            int nCCTVID;

            if (int.TryParse(textBoxCCTVID.Text.Trim(), out nCCTVID) == false)
            {
                textBoxCCTVID.Focus();
                MessageBox.Show("CCTV ID를 입력하세요");
                return;
            }

            string strSQL = "Select CameraName, URL from CCTV where ID = " + nCCTVID;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 2)
            {
                m_nCCTVID = -1;
                return;
            }

            string strCameraName = WebDBManager.GetStringField(arrResult[0]);
            string strURL = WebDBManager.GetStringField(arrResult[1]);

            if (strCameraName != null && strURL != null)
            {
                labelCameraName.Text = strCameraName;
                textBoxURL.Text = strURL;
                m_nCCTVID = nCCTVID;
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (m_nCCTVID < 0)
            {
                MessageBox.Show("CCTV 조회를 먼저 하세요");
                return;
            }

            string strURL = textBoxURL.Text.Trim();
            string strSQL = "Update CCTV set URL = '" + strURL + "' where ID = " + m_nCCTVID.ToString();

            if (m_dbMgr.GetResultData(strSQL) != null)
            {
                MessageBox.Show("URL이 변경되었습니다.");
            }
            else
            {
                MessageBox.Show("URL 변경이 실패하였습니다.");
            }
        }
    }
}
