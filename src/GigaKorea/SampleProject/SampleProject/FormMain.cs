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

namespace SampleProject
{
    public partial class FormMain : Form
    {
        public enum Type { Normal = 0, Download = 1 }

        private Type m_type = Type.Normal;

        private Dictionary<string, string> m_dicSidoList = null;
        private Dictionary<string, string> m_dicSigunguList = null;
        private Dictionary<string, string> m_dicDongList = null;
        ProgressInfo m_progressInfo = new ProgressInfo();

        private List<BulidingInfo> m_listBulidingInfo = null;

        private string m_strBuildingKey = ""; // 선택한 건물 키
        private string m_strBuildingName = ""; // 선택한 건물 이름

        private string m_strResultMessage = "";
        private string m_strFileName = "";

        // 라이브러리 클래스 
        private XMLWebManager m_XmlWebmgr = new XMLWebManager();

        // 디버깅용 타이머
        private System.Timers.Timer m_timer = null;

        private uProgressForm m_ufrm = null;
        private BackgroundWorker worker = null;

        //private FormLogin formLogin = null;

        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public FormMain(WebServiceManager webManager, Type type)
        {
            InitializeComponent();
            m_instance = this;
            m_type = type;

            //formLogin = new FormLogin(this, m_XmlWebmgr.WebManager);
            //formLogin.StartPosition = FormStartPosition.CenterParent;
            //formLogin.ShowDialog();
            m_XmlWebmgr.WebManager = webManager;

            // 디버깅 테스트
            m_timer = new System.Timers.Timer();
            m_timer.Interval = 1000;
            m_timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimer);
            m_timer.Start();
        }

        private void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("Progress Info: " + m_progressInfo.Message + ", " + m_progressInfo.Percent + "%");
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // 다운로드 모드
            if (m_type == Type.Download)
            {   // 업로드 버튼 숨김 처리
                btnUpload.Visible = false;
            }

            // 시도 리스트 Init
            string strResultMessage = "";

            // 시도 리스트 받기
            m_dicSidoList = m_XmlWebmgr.GetSidoList(ref strResultMessage);
            // GetSidoList 정보
            /// <summary>
            /// 시도 리스트 받아오기
            /// </summary>
            /// <param name="strResultMessage"> 결과 메시지 (string) </param>
            /// <returns> 시도 이름 (string) - 시도 KEY (string)으로 구성된 Dictionary, 오류 발생시 Null 반환 결과 메시지 참고 </returns>


            if (m_dicSidoList == null)
                return;

            foreach (KeyValuePair<string, string> tmpPair in m_dicSidoList)
                cmbSido.Items.Add(tmpPair.Key);

            if (cmbSido.Items.Count > 0)
                cmbSido.SelectedIndex = 0;
        }

        // 시도를 선택하면
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
            // GetSigunguList 정보
            /// <summary>
            /// 시군구 리스트 받아오기
            /// </summary>
            /// <param name="strSidoKey"> 시군구 조회 할 해당 시도 Key (string) </param>
            /// <param name="strResultMessage"> 결과 메시지 (string) </param>
            /// <returns> 시군구 이름 (string) - 시군구 KEY (string)으로 구성된 Dictionary, 오류 발생시 Null 반환 결과 메시지 참고 </returns>


            if (m_dicSigunguList == null)
                return;

            foreach (KeyValuePair<string, string> tmpPair in m_dicSigunguList)
                cmbSigungu.Items.Add(tmpPair.Key);

            if (cmbSigungu.Items.Count > 0)
                cmbSigungu.SelectedIndex = 0;
        }

        // 시군구를 선택하면
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
            // GetDongList 정보
            /// <summary>
            /// 읍면동 리스트 받아오기
            /// </summary>
            /// <param name="strSigunguKey"> 읍면동 조회 할 해당 시군구 Key (string) </param>
            /// <param name="strResultMessage"> 결과 메시지 (string) </param>
            /// <returns> 읍면동 이름 (string) - 읍면동 KEY (string)으로 구성된 Dictionary, 오류 발생시 Null 반환 결과 메시지 참고 </returns>


            if (m_dicDongList == null)
                return;

            foreach (KeyValuePair<string, string> tmpPair in m_dicDongList)
                cmbDong.Items.Add(tmpPair.Key);

            if (cmbDong.Items.Count > 0)
                cmbDong.SelectedIndex = 0;
        }


        private void chkDong_CheckedChanged(object sender, EventArgs e)
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

        // 검색 버튼 클릭 시 
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
            // GetBulidingInfoList 정보
            /// <summary>
            /// 건물정보 리스트 받아오기
            /// </summary>
            /// <param name="strSigunguKey"> 건물리스트 조회 할 해당 시군구 Key (string) </param>
            /// <param name="strDongName"> 건물리스트 조회 할 해당 읍면동 이름 (string), Null 또는 빈문자열 입력시 시군구 해당하는 전지역 조회 </param>
            /// <param name="strLoadName"> 건물리스트 조회 할 해당 도로명 (string) </param>
            /// <param name="strBulidingNum"> 건물리스트 조회 할 해당 건물번호 Key (string), Null 또는 빈문자열 입력시 도로명 해당하는 전지역 조회 </param>
            /// <param name="strResultMessage"> 결과 메시지 (string) </param>
            /// <returns> BulidingInfo 클래스로 구성된 List, 오류 발생시 Null 반환 결과 메시지 참고  </returns>

            // BulidingInfo 클래스 정보
            /// <summary>
            /// <para> 건물정보를 가지고 있는 클래스 </para>
            /// <para> BuildingKey : 건물 KEY (string) </para>
            /// <para> Address : 건물 주소 (string) </para>
            /// <para> BuildingMenu : 건물용도 (string) </para>
            /// <para> FloorNo : 건물 층수 (string) </para>
            /// <para> BuildingName : 건물 이름 (string) </para>
            /// <para> UpdateInfo : 업데이트 정보 (string) </para>
            /// </summary>


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

        // 건물 리스트 클릭시
        private void gridAddress_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // 해당 건물 key 가져오기
            m_strBuildingKey = m_listBulidingInfo[gridAddress.SelectedRows[0].Index].BuildingKey;

            string strResultMessage = "";
            // 업데이트 정보 가져오기
            string UpdateInfo = m_XmlWebmgr.GetUpdateInfo(m_strBuildingKey, ref strResultMessage);
            // GetUpdateInfo 정보
            /// <summary>
            /// 업데이트 정보 받아오기
            /// </summary>
            /// <param name="strBuildingKey"> 업데이트 정보를 조회 할 해당 건물 Key (string) </param>
            /// <param name="strResultMessage"> 결과 메시지 (string) </param>
            /// <returns> 업데이트 일자 및 계정 ID으로 구성된 문자열(string), 오류 발생시 Null 반환 결과 메시지 참고 </returns>

            if (UpdateInfo == null)
                return;

            // 업데이트 정보 넣기
            m_listBulidingInfo[gridAddress.SelectedRows[0].Index].UpdateInfo = UpdateInfo;
            gridAddress.SelectedRows[0].Cells[colUpdateInfo.Index].Value = UpdateInfo;
        }

        // 다운로드 버튼 클릭시
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


            // 다운로드 모드
            if (m_type == Type.Download)
            {   // 고정 경로
                string strTEMP = Environment.GetEnvironmentVariable("TEMP");
                if (strTEMP == null || strTEMP.Length == 0)
                {
                    MessageBox.Show("환경변수 TEMP 를 찾을 수 없습니다. 관리자에게 문의해주세요.");
                    return;
                }

                m_strFileName = strTEMP + "\\다부처건물.xml";
            }
            else
            {
                // 저장위치
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "XML Files|*.xml|All FIles|*.*";
                dialog.FilterIndex = 0;
                dialog.Title = "XML 저장";
                dialog.FileName = m_strBuildingName + ("(DownLoad)");

                if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    m_strFileName = dialog.FileName;
                else
                    return;
            }
            

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoDownLoad_Work;

            m_progressInfo = new ProgressInfo();
            m_ufrm = new uProgressForm(uProgressForm.Type.Download, m_progressInfo);
            m_ufrm.StartPosition = FormStartPosition.CenterParent;

            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
            m_ufrm.ShowDialog();//modal

            //this.Cursor = Cursors.WaitCursor;

            //string strResultMessage = "";
            // 해당 건물 XML 다운로드 받기
            //bool bResult = m_XmlWebmgr.DownloadXMLFile(m_strFileName, m_strBuildingKey, ref strResultMessage, ref m_progressInfo);

            //MessageBox.Show(strResultMessage);
            //this.Cursor = Cursors.Arrow;
        }

        private void btnUpload_Click(object sender, EventArgs e)
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
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "XML Files|*.xml|All FIles|*.*";
            dialog.FilterIndex = 0;
            dialog.Title = "XML 열기";
            dialog.FileName = m_strBuildingName + ("(DownLoad)");

            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                m_strFileName = dialog.FileName;
            else
                return;

            //this.Cursor = Cursors.WaitCursor;
            //string strResultMessage = "";
            
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoUpLoad_Work;

            m_progressInfo = new ProgressInfo();
            m_ufrm = new uProgressForm(uProgressForm.Type.Upload, m_progressInfo);
            m_ufrm.StartPosition = FormStartPosition.CenterParent;

            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
            m_ufrm.ShowDialog();//modal

            //bool bResult = m_XmlWebmgr.UploadXMLFile(strFileName, m_strBuildingKey, ref strResultMessage, ref m_progressInfo);

            //MessageBox.Show(strResultMessage);
            //this.Cursor = Cursors.Arrow;
        }

        private void Worker_DoUpLoad_Work(object sender, DoWorkEventArgs e)
        {                
            bool bResult = m_XmlWebmgr.UploadXMLFile(m_strFileName, m_strBuildingKey, ref m_strResultMessage, ref m_progressInfo);
        }

        private void Worker_DoDownLoad_Work(object sender, DoWorkEventArgs e)
        {
            // 해당 건물 XML 다운로드 받기
            bool bResult = m_XmlWebmgr.DownloadXMLFile(m_strFileName, m_strBuildingKey, ref m_strResultMessage, ref m_progressInfo);
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
