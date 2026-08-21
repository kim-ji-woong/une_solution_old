using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BIMViewer
{
    public partial class FormLocation : Form
    {
        private string m_sID = "";
        private string m_sPW = "";
        private Dictionary<string, string> m_dicSidoList = null;
        private Dictionary<string, string> m_dicSggList = null;
        private Dictionary<string, string> m_dicEmdList = null;
        private List<string> m_sRoadList = null;
        private Dictionary<string, string> m_spaceUserList = null;

        private class cBulidingInfo
        {
            public string m_sBuildingMngno = null;
            public string m_sAddress = null;
            public string m_sBuildingMenu = null;
            public string m_sFloorNo = null;
            public string m_sUpdateInfo = null;
            public string m_sBuildingName = null;
        }
        private List<cBulidingInfo> m_cBuildingList = null;

        public string m_sSelBuildMngNo = ""; //선택한 건물고유키
        public string m_sSelAddress = "";

        private WebServiceManager m_webServiceMgr = null;

        public FormLocation(string strLoginID, string strLoginPW, Dictionary<string, string> spaceUserList)
        {
            InitializeComponent();

            m_webServiceMgr = new WebServiceManager();

            m_sID = strLoginID;
            m_sPW = strLoginPW;
            m_cBuildingList = new List<cBulidingInfo>();
            //this.Controls.Add(grdAdress);
            //grdAdress.ColumnCount = 4;
            m_spaceUserList = spaceUserList;
            grdAdress.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        //시도리스트 먼저.
        private void FormUploadLocationLoad(object sender, EventArgs e)
        {
            string strResult = m_webServiceMgr.GetSidoList(m_sID, m_sPW);
            if (strResult.Length == 0 || strResult.IndexOf("RS101") < 0)
                return;
        
            m_dicSidoList = GetSidoSggEmdNameCodeList(strResult);

            foreach (KeyValuePair<string, string> tmpPair in m_dicSidoList)
                cmbSido.Items.Add(tmpPair.Key);

            cmbSido.SelectedIndex = 0;
            
        }       
        //시도선택하면. 
        private void cmbSidoChanged(object sender, EventArgs e)
        {
            if(m_dicSggList != null) m_dicSggList.Clear();
            if(m_dicEmdList != null) m_dicEmdList.Clear();
            cmbSgg.Items.Clear();
            cmbEmd.Items.Clear();

            string strSidoValue;           
            m_dicSidoList.TryGetValue(cmbSido.SelectedItem.ToString(), out strSidoValue);
            if (strSidoValue == "")
                return;
            
            string strResult = m_webServiceMgr.GetSggList(strSidoValue, m_sID, m_sPW);
            if (strResult.Length == 0 || strResult.IndexOf("RS101") < 0)
                return;

            m_dicSggList = GetSidoSggEmdNameCodeList(strResult);

            foreach (KeyValuePair<string, string> tmpPair in m_dicSggList)
                cmbSgg.Items.Add(tmpPair.Key);

            //cmbSgg.SelectedIndex = 0;
            //cmbSgg.SelectedIndex = 4; //test

            if (cmbSgg.Items.Count > 0)
                cmbSgg.SelectedIndex = 0;

        }
        //시군구 선택하면.
        private void cmbSggChanged(object sender, EventArgs e)
        {
            if (m_dicEmdList != null) m_dicEmdList.Clear();
            cmbEmd.Items.Clear();

            string strSggValue;
            m_dicSggList.TryGetValue(cmbSgg.SelectedItem.ToString(), out strSggValue);
            if (strSggValue == "")
                return;

            if (!chkEmd.Checked)
                return;
            
            string strResult = m_webServiceMgr.GetEmdList(strSggValue, m_sID, m_sPW);
            if (strResult.Length == 0 || strResult.IndexOf("RS101") < 0)
                return;

            m_dicEmdList = GetSidoSggEmdNameCodeList(strResult);
            foreach (KeyValuePair<string, string> tmpPair in m_dicEmdList)
                cmbEmd.Items.Add(tmpPair.Key);

            cmbEmd.SelectedIndex = 0;
        }

        private void ChkEmd_CheckedChanged(object sender, EventArgs e)
        {
            cmbEmd.Enabled = chkEmd.Checked;

            if (cmbEmd.Enabled)
            {
                cmbSggChanged(null, null);
            }
            else
            {
                if (cmbEmd.Items.Count > 0) cmbEmd.Items.Clear();
                if (m_dicEmdList != null) m_dicEmdList.Clear();
            }                
        }

        private bool GetBlankRemovedName(string sName, out string sValuedName)
        {
            sValuedName = sName;
            if (sName.Length < 1) return false;

            int i = sName.IndexOf(" ");
            while(i > 0)
            {
                sValuedName = sValuedName.Remove(i, 1);
                i = sValuedName.IndexOf(" ");
            }

            if (sValuedName.Length < 1) return false;

            return true;
        }

        private bool GetDashRemovedName(string sName, out string sValuedName)
        {
            sValuedName = sName;
            if (sName.Length < 1) return false;

            int i = sValuedName.IndexOf("-");
            while (i > 0)
            {
                sValuedName = sValuedName.Remove(i, 1);
                i = sValuedName.IndexOf("-");
            }

            if (sValuedName.Length < 1) return false;

            return true;
        }

        //찾기누르면.
        private void RbtnSearch_Click(object sender, EventArgs e)
        {
            grdAdress.Rows.Clear();

            if (m_sRoadList != null) m_sRoadList.Clear();
            if (m_cBuildingList != null) m_cBuildingList.Clear();

            string sValuedRoadName = "";  
            //도로명에 빈칸 빼기
            if(!GetBlankRemovedName(txtRoadName.Text, out sValuedRoadName))
                return;

            //도로명에 - 빼기
            if (!GetDashRemovedName(sValuedRoadName, out sValuedRoadName))
                return;

            string strSggValue = "";
            string strSelEmd = "";

            m_dicSggList.TryGetValue(cmbSgg.SelectedItem.ToString(), out strSggValue);
            if (strSggValue =="")
                return;

            if(chkEmd.Checked)            
                strSelEmd = cmbEmd.SelectedItem.ToString().Trim();

            this.Cursor = Cursors.WaitCursor;

            string strResult = m_webServiceMgr.GetRoadList(strSggValue, strSelEmd, sValuedRoadName, m_sID, m_sPW);
            if (strResult.Length == 0 || strResult.IndexOf("RS101") < 0)
                return;

            //도로코드 완성
            m_sRoadList = GetRoadNameCodeList(strResult); 

            //완성한 도로코드마다 건물정보리스트 만들기
            foreach(string sRoadCode in m_sRoadList)
                MakeBuildInfoList(strSggValue, sRoadCode);

            //그리드에 표현
            ShowBuildingInfoList();
            grdAdress.CurrentCell = null;//맨처음 찾을때는 그리드에서 선택안되게
            this.Cursor = Cursors.Arrow;
        }

        void ShowBuildingInfoList()
        {
            grdAdress.Rows.Clear();
            int nRowIndex;
            foreach (cBulidingInfo bInfo in m_cBuildingList)
            {
                nRowIndex = grdAdress.Rows.Add();
                grdAdress.Rows[nRowIndex].Cells[0].Value = bInfo.m_sAddress;
                grdAdress.Rows[nRowIndex].Cells[1].Value = bInfo.m_sBuildingMenu;
                grdAdress.Rows[nRowIndex].Cells[2].Value = bInfo.m_sBuildingName;
                grdAdress.Rows[nRowIndex].Cells[3].Value = bInfo.m_sFloorNo;
                grdAdress.Rows[nRowIndex].Cells[4].Value = bInfo.m_sUpdateInfo;
            }
        }

        private string GetStringValue(string strFrom, string strKeyi, string strKeyj)
        {
            int i, j, cnt;
            cnt = strKeyi.Length;
            i = strFrom.IndexOf(strKeyi);

            if (i < 0) return "";

            j = strFrom.IndexOf(strKeyj);

            return strFrom.Substring(i + cnt, j - i - cnt);
        }

        private void GetBuildingNumber(out string sMainNumber, out string sSubNumber)
        {
            string sNumber ="";
            sMainNumber = "";
            sSubNumber = "";

            //공백삭제후.
            if (!GetBlankRemovedName(txtMainNumber.Text, out sNumber))
                return;

            //"-"로. 본번, 부번 구분
            int i = sNumber.IndexOf("-");
            if (i >= 0)
            {
                sMainNumber = sNumber.Substring(0, i);
                sSubNumber = sNumber.Substring(i + 1, sNumber.Length - i - 1);
            }
            else//본번만 입력시 뒤에 부번0
            {
                sMainNumber = sNumber;
                sSubNumber = "0";
            }
        }

        private void MakeBuildInfoList(string strSggValue, string sRoadCode)
        {
            //건물 번호에서 본번, 부번 추출.
            string sMainNumber = "";
            string sSubNumber = "";
            GetBuildingNumber(out sMainNumber, out sSubNumber);
            
            string strResult = m_webServiceMgr.GetBuildingList(strSggValue, sRoadCode, sMainNumber, sSubNumber, m_sID, m_sPW);
            if (strResult.Length == 0 || strResult.IndexOf("RS101") < 0)
                return;

            int i;
            i = strResult.IndexOf("<listNaviBuild>");//첫건물 탐색
            while (i > 0)
            {
                strResult = strResult.Substring(i);
                cBulidingInfo bInfo = new cBulidingInfo();
                //주소
                bInfo.m_sAddress += GetStringValue(strResult, "<sido_name>","</sido_name>") + " ";
                bInfo.m_sAddress += GetStringValue(strResult, "<sigungu_name>", "</sigungu_name>") + " ";
                bInfo.m_sAddress += GetStringValue(strResult, "<road_name>", "</road_name>") + " ";
                bInfo.m_sAddress += GetStringValue(strResult, "<build_main>", "</build_main>");// main build num
                string sBubun = GetStringValue(strResult, "<build_sub>", "</build_sub>");// sub build num
                if (sBubun != "0")
                    bInfo.m_sAddress += "-" + sBubun + " ";
                else
                    bInfo.m_sAddress += " ";

                //고유키
                bInfo.m_sBuildingMngno += GetStringValue(strResult, "<build_mng_no>", "</build_mng_no>");
                bInfo.m_sAddress += GetStringValue(strResult, "<build_mng_name>", "</build_mng_name>");
               //용도
                bInfo.m_sBuildingMenu += GetStringValue(strResult, "<build_mng_menu>", "</build_mng_menu>");
                //층수
                bInfo.m_sFloorNo += GetStringValue(strResult, "<ground_layer>", "</ground_layer>");
                //name
                //bInfo.m_sBuildingName += GetStringValue(strResult, "<building_name>", "</building_name>");
                bInfo.m_sBuildingName += GetStringValue(strResult, "<build_mng_name>", "</build_mng_name>");

                //마지막업데이트날짜. 
                bInfo.m_sUpdateInfo = "yy-mm-dd";
                
                m_cBuildingList.Add(bInfo);//건물정보한개.추가

                i = strResult.IndexOf("</listNaviBuild>");
                strResult = strResult.Substring(i);
                i = strResult.IndexOf("<listNaviBuild>");//다음건물 탐색
            }
        }

        private List<string> GetRoadNameCodeList(string strResult)
        {
            int i;
            string strRoadCode;
            List<string> sList = new List<string>();

            i = strResult.IndexOf("<listNaviBuildCode>");
            while (i > 0)
            {
               strResult = strResult.Substring(i);
               strRoadCode = GetStringValue(strResult, "<road_code>", "</road_code>");
             
                if(!sList.Contains(strRoadCode))//중복체크
                    sList.Add(strRoadCode);
                
                i = strResult.IndexOf("</listNaviBuildCode>");
                strResult = strResult.Substring(i);
                i = strResult.IndexOf("<listNaviBuildCode>");
            }

            return sList;
        }
        private Dictionary<string, string> GetSidoSggEmdNameCodeList(string strResult)
        {
            int i;
            string strKey = "";
            string strValue = "";
            Dictionary<string, string> dicList = new Dictionary<string, string>();

            i = strResult.IndexOf("<listAddrCode>");
            while (i > 0)
            {
                strResult = strResult.Substring(i);
                strValue = GetStringValue(strResult, "<code>", "</code>");                
                strKey = GetStringValue(strResult, "<codeName>", "</codeName>");
                dicList.Add(strKey, strValue);

                i = strResult.IndexOf("</listAddrCode>");
                strResult = strResult.Substring(i);
                i = strResult.IndexOf("<listAddrCode>");
            }

            return dicList;
        }

        private void RbtnOK_Click(object sender, EventArgs e)
        {
            //그리드에서 선택한 빌딩고유키얻어야함.
            if (grdAdress.SelectedRows == null)
            {
                MessageBox.Show("주소를 선택하세요.");
                return;
            }

            m_sSelBuildMngNo = m_cBuildingList[grdAdress.CurrentRow.Index].m_sBuildingMngno;
            m_sSelAddress = m_cBuildingList[grdAdress.CurrentRow.Index].m_sBuildingName;
            //m_sSelBuildMngNo = "3611011200104700005000001";//test : 누리학교. 세종자치시 고운동 만남로 326-14
            //m_sSelBuildMngNo = "4128710400123110000012295";//test :건설기술연구원. 고양시 일산서구 대화동 고양대로 315

            if (m_sSelBuildMngNo.Length == 0)
                this.DialogResult = DialogResult.No;

            this.DialogResult = DialogResult.OK;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private bool GetLastSpaceDate(string strResult, out string strDate, out string strUser)
        {
            int i;
            string tmpSDate, tmpSUser;
            strDate = strUser = "";
            i = strResult.IndexOf("<create_date>");
            if (i < 0) return false;

            DateTime lastDate = new DateTime();
            DateTime tmpDate = new DateTime();
            bool flag = false;
            while (i > 0)
            {
                strResult = strResult.Substring(i);
                tmpSDate = GetStringValue(strResult, "<create_date>", "</create_date>");
                tmpSUser = GetStringValue(strResult, "<create_user_id>", "</create_user_id>");

                if (m_spaceUserList.ContainsKey(tmpSUser))
                {
                    if (!flag)
                    {
                        lastDate = DateTime.Parse(tmpSDate);
                        strUser = tmpSUser;
                        flag = true;
                    }

                    tmpDate = DateTime.Parse(tmpSDate);
                    if (DateTime.Compare(tmpDate, lastDate) > 0)
                    {
                        lastDate = tmpDate;
                        strUser = tmpSUser;
                    }
                }

                i = strResult.IndexOf("</create_date>");
                strResult = strResult.Substring(i);
                i = strResult.IndexOf("<create_date>");
            }

            if (!flag) return false;

            strDate = lastDate.ToString();
            return true;
        }
        private void GrdAdress_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            m_sSelBuildMngNo = m_cBuildingList[grdAdress.SelectedRows[0].Index].m_sBuildingMngno;
            
            string strResult = m_webServiceMgr.GetLevelInfo(m_sSelBuildMngNo, m_sID, m_sPW);
            if (strResult.Length == 0 || strResult.IndexOf("RS101") < 0)
                return;

            string strDateAndUser = "";
            if (strResult.IndexOf("<create_date>") >= 0)
            {
                strDateAndUser += DateTime.Parse(GetStringValue(strResult, "<create_date>", "</create_date>")) + " / ";
                strDateAndUser += GetStringValue(strResult, "<create_user_id>", "</create_user_id>");
            }
            else
                strDateAndUser += "None";

            m_cBuildingList[grdAdress.SelectedRows[0].Index].m_sUpdateInfo = strDateAndUser;
            grdAdress.SelectedRows[0].Cells[4].Value = strDateAndUser;
        }

        //폼움직이게
        private Point mousePoint;
        private void FormLocation_MouseDown(object sender, MouseEventArgs e)
        {
            mousePoint = new Point(e.X, e.Y);
        }

        private void FormLocation_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                Location = new Point(this.Left - (mousePoint.X - e.X), this.Top - (mousePoint.Y - e.Y));
            }
        }

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mousePoint = new Point(e.X, e.Y);
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                Location = new Point(this.Left - (mousePoint.X - e.X), this.Top - (mousePoint.Y - e.Y));
            }
        }
    }
}
