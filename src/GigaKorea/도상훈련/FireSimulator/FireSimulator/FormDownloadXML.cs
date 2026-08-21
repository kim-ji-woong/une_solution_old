using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XMLWebServiceManager;

namespace FireSimulator
{
    public partial class FormDownloadXML : Form
    {
        // 라이브러리 클래스 
        private XMLWebManager m_XmlWebmgr = new XMLWebManager();

        private Dictionary<string, string> m_dicSidoList = null;
        private Dictionary<string, string> m_dicSigunguList = null;
        private Dictionary<string, string> m_dicDongList = null;

        private List<BulidingInfo> m_listBulidingInfo = null;

        private string m_strBuildingKey = ""; // 선택한 건물 키
        private string m_strBuildingName = ""; // 선택한 건물 이름

        private string m_strResultMessage = "";
        private string m_strFileName = "";

        private uProgressForm m_ufrm = null;
        private BackgroundWorker worker = null;

        private ProgressInfo m_progressInfo = new ProgressInfo();

        private static FormDownloadXML m_instance = null;
        public static FormDownloadXML Instance
        {
            get { return m_instance; }
        }

        public FormDownloadXML(WebServiceManager webManager)
        {
            InitializeComponent();
            m_instance = this;

            m_XmlWebmgr.WebManager = webManager;
        }

        private void FormDownloadXML_Load(object sender, EventArgs e)
        {   // 시도 리스트 Init
            string strResultMessage = "";
            // 시도 리스트 받기
            m_dicSidoList = m_XmlWebmgr.GetSidoList(ref strResultMessage);

            if (m_dicSidoList == null)
                return;

            foreach (KeyValuePair<string, string> tmpPair in m_dicSidoList)
                cmbSido.Items.Add(tmpPair.Key);

            if (cmbSido.Items.Count > 0)
                cmbSido.SelectedIndex = 0;
        }

        private void cmbSido_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_dicSigunguList != null) m_dicSigunguList.Clear();
            if (m_dicDongList != null) m_dicDongList.Clear();
            cmbSigungu.Items.Clear();
            cmbDong.Items.Clear();

            // 시도 Key 가져오기
            string strSidoValue;
            m_dicSidoList.TryGetValue(cmbSido.SelectedItem.ToString(), out strSidoValue);
            if (strSidoValue == "")
                return;

            string strResultMessage = "";
            // 시군구 리스트 가져오기
            m_dicSigunguList = m_XmlWebmgr.GetSigunguList(strSidoValue, ref strResultMessage);

            if (m_dicSigunguList == null)
                return;

            foreach (KeyValuePair<string, string> tmpPair in m_dicSigunguList)
                cmbSigungu.Items.Add(tmpPair.Key);

            if (cmbSigungu.Items.Count > 0)
                cmbSigungu.SelectedIndex = 0;
        }

        private void cmbSigungu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_dicDongList != null) m_dicDongList.Clear();
            cmbDong.Items.Clear();

            string strSigunguValue;
            m_dicSigunguList.TryGetValue(cmbSigungu.SelectedItem.ToString(), out strSigunguValue);
            if (strSigunguValue == "")
                return;


            // 읍면동 체크 확인
            if (!chkDong.Checked)
                return;

            string strResultMessage = "";

            // 읍면동 찾기 
            m_dicDongList = m_XmlWebmgr.GetDongList(strSigunguValue, ref strResultMessage);

            if (m_dicDongList == null)
                return;

            foreach (KeyValuePair<string, string> tmpPair in m_dicDongList)
                cmbDong.Items.Add(tmpPair.Key);

            if (cmbDong.Items.Count > 0)
                cmbDong.SelectedIndex = 0;
        }

        private void cmbDong_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbDong.Enabled = chkDong.Checked;

            if (cmbDong.Enabled)
            {
                cmbSigungu_SelectedIndexChanged(null, null);
            }
            else
            {
                if (cmbDong.Items.Count > 0) cmbDong.Items.Clear();
                if (m_dicDongList != null) m_dicDongList.Clear();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            gridAddress.Rows.Clear();

            if (m_listBulidingInfo != null) m_listBulidingInfo.Clear();

            string strSigunguValue = "";
            string strDongValue = "";

            // 시군구 Key 가져오기
            m_dicSigunguList.TryGetValue(cmbSigungu.SelectedItem.ToString(), out strSigunguValue);
            if (strSigunguValue == "")
                return;

            // 읍면동이 선택되었다면 읍면동 이름 가져오기
            if (chkDong.Checked)
                strDongValue = cmbDong.SelectedItem.ToString().Trim();

            this.Cursor = Cursors.WaitCursor;

            // 해당 건물리스트 조회하기
            string strResultMessage = "";
            m_listBulidingInfo = m_XmlWebmgr.GetBulidingInfoList(strSigunguValue, strDongValue, txtRoadName.Text, txtBulidingNum.Text, ref strResultMessage);


            if (m_listBulidingInfo == null)
                return;

            // 그리드에 표현
            ShowBuildingInfoList();
            gridAddress.CurrentCell = null;//맨처음 찾을때는 그리드에서 선택안되게
            this.Cursor = Cursors.Arrow;
        }

        void ShowBuildingInfoList()
        {
            gridAddress.Rows.Clear();
            int nRowIndex;
            foreach (BulidingInfo bInfo in m_listBulidingInfo)
            {
                nRowIndex = gridAddress.Rows.Add();
                gridAddress.Rows[nRowIndex].Cells[0].Value = bInfo.Address;
                gridAddress.Rows[nRowIndex].Cells[1].Value = bInfo.BuildingMenu;
                gridAddress.Rows[nRowIndex].Cells[2].Value = bInfo.BuildingName;
                gridAddress.Rows[nRowIndex].Cells[3].Value = bInfo.FloorNo;
                gridAddress.Rows[nRowIndex].Cells[4].Value = bInfo.UpdateInfo;
            }
        }

        private void gridAddress_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // 해당 건물 key 가져오기
            m_strBuildingKey = m_listBulidingInfo[gridAddress.SelectedRows[0].Index].BuildingKey;

            string strResultMessage = "";
            // 업데이트 정보 가져오기
            string UpdateInfo = m_XmlWebmgr.GetUpdateInfo(m_strBuildingKey, ref strResultMessage);

            if (UpdateInfo == null)
                return;

            // 업데이트 정보 넣기
            m_listBulidingInfo[gridAddress.SelectedRows[0].Index].UpdateInfo = UpdateInfo;
            gridAddress.SelectedRows[0].Cells[colUpdateInfo.Index].Value = UpdateInfo;
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            //그리드에서 선택한 빌딩고유키얻어야함.
            if (gridAddress.CurrentRow == null)
            {
                MessageBox.Show("주소를 선택하세요.");
                return;
            }

            m_strBuildingKey = m_listBulidingInfo[gridAddress.CurrentRow.Index].BuildingKey;
            m_strBuildingName = m_listBulidingInfo[gridAddress.CurrentRow.Index].BuildingName;

            // 저장위치
            //SaveFileDialog dialog = new SaveFileDialog();
            //dialog.Filter = "XML Files|*.xml|All FIles|*.*";
            //dialog.FilterIndex = 0;
            //dialog.Title = "XML 저장";
            //dialog.FileName = m_strBuildingName + ("(DownLoad)");

            //if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            //    m_strFileName = dialog.FileName;
            //else
            //    return;
            // 고정경로
            string strTEMP = Environment.GetEnvironmentVariable("TEMP");
            if (strTEMP == null || strTEMP.Length == 0)
            {
                MessageBox.Show("환경변수 TEMP 를 찾을 수 없습니다. 관리자에게 문의해주세요.");
                return;
            }

            m_strFileName = strTEMP + "\\다부처건물.xml";

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoDownLoad_Work;

            m_progressInfo = new ProgressInfo();
            m_ufrm = new uProgressForm(uProgressForm.Type.Download, m_progressInfo);
            m_ufrm.StartPosition = FormStartPosition.CenterParent;

            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
            m_ufrm.ShowDialog();//modal
        }

        private void Worker_DoDownLoad_Work(object sender, DoWorkEventArgs e)
        {
            FormMain formMain = (FormMain)Owner;
            formMain.FilePath = m_strFileName;

            // 해당 건물 XML 다운로드 받기
            if (m_XmlWebmgr.DownloadXMLFile(m_strFileName, m_strBuildingKey, ref m_strResultMessage, ref m_progressInfo) == true)
            {   // xml 다운로드 성공
                formMain.FilePath = m_strFileName;
            }
            else
            {
                formMain.FilePath = null;
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DateTime dtStart = m_ufrm.StartTime;
            DateTime dtEnd = DateTime.Now;

            TimeSpan tsWork = dtEnd - dtStart;
            string strWork = ChangeTimeToString(tsWork);

            m_ufrm.Close();
            m_ufrm = null;

            if (worker != null)
            {
                worker.Dispose();
                worker = null;
            }

            if (m_strResultMessage == "")
                return;
            else
            {
                string strMessage = m_strResultMessage + "\n\n시작시간: " + dtStart.ToString() + "\n종료시간: " + dtEnd.ToString() + "\n소요시간: " + strWork;
                MessageBox.Show(strMessage, "작업 결과");

                this.Close();
            }
        }

        public void CancleWorker()
        {
            if (worker != null)
            {
                worker.Dispose();
                worker = null;
            }
        }

        public string ChangeTimeToString(TimeSpan timeSpan)
        {
            string strDate = "";

            if (timeSpan.Days != 0)
                strDate += timeSpan.Days + "일 ";
            if (timeSpan.Hours != 0)
                strDate += timeSpan.Hours + "시간 ";
            if (timeSpan.Minutes != 0)
                strDate += timeSpan.Minutes + "분 ";
            if (timeSpan.Seconds != 0)
                strDate += timeSpan.Seconds + "초";

            return strDate;
        }
    }
}
