using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Sensor;
using UnE.Spatial;

namespace SDMS_Building.Report.ReportPopup
{
    public partial class PopupDetailLog : Form
    {
        private ReactionManager m_reactionMgr = null;
        private int m_nHistoryID = -1;

        public PopupDetailLog(ReactionManager reactionMgr, int nHistoryID)
        {
            InitializeComponent();

            FormMain.SetDoubleBuffer(dgvAction, true);
            FormMain.SetDoubleBuffer(dgvSMS, true);

            m_reactionMgr = reactionMgr;
            m_nHistoryID = nHistoryID;

            FormMain.Instance.CustomGridView(dgvAction, 10.0f, Color.FromArgb(0x25, 0x31, 0x50), Color.White, Color.FromArgb(0xf3, 0xf4, 0xfa), Color.FromArgb(0x25, 0x31, 0x50), DataGridViewContentAlignment.MiddleCenter);
            FormMain.Instance.CustomGridView(dgvSMS, 10.0f, Color.FromArgb(0x25, 0x31, 0x50), Color.White, Color.FromArgb(0xf3, 0xf4, 0xfa), Color.FromArgb(0x25, 0x31, 0x50), DataGridViewContentAlignment.MiddleCenter);

            InitGridView();
        }

        private void PopupDetailLog_Load(object sender, EventArgs e)
        {
            DisplayReaction();
            DisplaySMSHistory();
        }

        private void InitGridView()
        {
            for (int i = 0; i < dgvAction.Columns.Count; i++)
            {
                dgvAction.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            for (int i = 0; i < dgvSMS.Columns.Count; i++)
            {
                dgvSMS.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void DisplayReaction()
        {
            if (m_nHistoryID < 0)
                return;

            ArrayList arrSensorReactionHistory = m_reactionMgr.GetReactionLog(m_nHistoryID);

            int count = 0;
            int nCount = 1;

            foreach (Report.ReactionLog data in arrSensorReactionHistory)
            {
                //찾은 검색결과를 DataGrid로 출력
                if (!SetGridRows(data, count, nCount))
                    continue;

                count++;
                nCount++;
            }
        }

        private bool SetGridRows(Report.ReactionLog data, int nRow, int nCount)
        {
            if (data == null)
                return false;
            if (nRow < 0)
                return false;

            Zone zone = data.Zone;
            if (zone == null && data.SensorType != (int)IFacility.FacilityType.STRONG_WIND)
                return false;

            DateTime dtDate = data.Time;
            int nType = data.SensorType;
            int ReactionType = data.Type;
            
            Building buildingFind = (zone == null) ? null : zone.Building;
            string strBuildingName = buildingFind == null ? "전체" : buildingFind.BuildingName;
            string strFloorIndex = (zone == null || zone.Floor == null) ? "-" : zone.Floor.ToString();
            string strType = GetReactionString(ReactionType, (IFacility.FacilityType)data.SensorType);

            SensorReactionLog log = (SensorReactionLog)data.ArrLogList[0];
            int nDetectType;
            if (int.TryParse(log.Param3, out nDetectType))
            {
                if (ReactionType == (int)libSensorProcess.ReactionType.BEGIN_STATUS)
                {
                    string detectStr = SOPServer.EventTypeString.GetEventTypeDetectString(Convert.ToInt32(log.Param3));
                    if (detectStr.Length > 0)
                        strType = detectStr + " 탐지";
                }

                if (Convert.ToInt32(log.Param3) == (int)IFacility.FacilityType.STRONG_WIND || Convert.ToInt32(log.Param3) == (int)IFacility.FacilityType.Earthquake)
                {
                    strBuildingName = "전체";
                    strFloorIndex = "";
                }
            }
            
            string strUserName = data.UserName;

            //if (buildingFind == null)
            //{
            //    strBuildingName = (zone == null) "전체" : zone.ZoneName;
            //}

            string strManagerName = data.ManagerName;

            if (strType.Trim().Length == 0)
                return false;

            // 같은 로그가 이미 기록되어 있는지 확인한다.
            if (ContainsType(strType, dtDate))
                return false;
            
            string[] rows = { nCount.ToString(), dtDate.ToString(), strManagerName, strType, strBuildingName, strFloorIndex };
            dgvAction.Rows.Add(rows);

            return true;
        }

        // strType, dtDate에 해당하는 값이 이미 존재하는지 검사한다.
        private bool ContainsType(string strType, DateTime dtDate)
        {
            string strTime = dtDate.ToString();

            foreach (DataGridViewRow row in dgvAction.Rows)
            {
                if (row.Cells[colDate.Index].Value != null && row.Cells[colDate.Index].Value.ToString() == strTime)
                {
                    if (row.Cells[colType.Index].Value != null && row.Cells[colType.Index].Value.ToString() == strType)
                        return true;
                }
            }

            return false;
        }

        public string GetReactionString(int nReactionType, IFacility.FacilityType facilityType)
        {
            string strType = "";

            switch (nReactionType)
            {
                //case 0: strType = "상황 시작";
                case (int)libSensorProcess.ReactionType.BEGIN_STATUS:
                    strType = IFacility.GetFacilityTypeString(facilityType) + " 탐지";
                    break;
                case (int)libSensorProcess.ReactionType.RUN_DETECT_BROADCAST:
                    strType = "사내 방송 실시(탐지)";
                    break;
                case (int)libSensorProcess.ReactionType.RUN_REPORT_BROADCAST:
                    strType = "사내 방송 실시(신고)";
                    break;
                case (int)libSensorProcess.ReactionType.SEND_SMS:
                    strType = "문자메시지 발송";
                    break;
                case (int)libSensorProcess.ReactionType.SEND_DETECT_SMS:
                    strType = "문자메시지 발송(탐지)";
                    break;
                case (int)libSensorProcess.ReactionType.SEND_REPORT_SMS:
                    strType = "문자메시지 발송(신고)";
                    break;
                case (int)libSensorProcess.ReactionType.SEND_MALFUNCTION_SMS:
                    strType = "문자메시지 발송(오작동)";
                    break;
                case (int)libSensorProcess.ReactionType.SEND_REPAIR_SMS:
                    strType = "문자메시지 발송(복구)";
                    break;
                case (int)libSensorProcess.ReactionType.MALFUNCTION:
                    strType = "오작동 처리";
                    break;
                case (int)libSensorProcess.ReactionType.NOTIFY_SIGNAL:
                    strType = IFacility.GetFacilityTypeString(facilityType) + " 신고";
                    break;
                case (int)libSensorProcess.ReactionType.IGNORE_SIGNAL:
                    strType = "화재신호 꺼짐";
                    break;
                case (int)libSensorProcess.ReactionType.RUN_SOP:
                    strType = "SOP 발동";
                    break;
                case (int)libSensorProcess.ReactionType.RUN_N_CANCEL_SOP:
                    strType = "SOP 실행후 취소";
                    break;
                case (int)libSensorProcess.ReactionType.FINISH_SOP:
                    strType = "SOP 종료";
                    break;
                case (int)libSensorProcess.ReactionType.IGNORE_SOP:
                    strType = "SOP 실행않고 상황 종료";
                    break;
                case (int)libSensorProcess.ReactionType.END_STATUS:
                    strType = "상황해제";
                    break;
                default:
                    break;
            }

            return strType;
        }

        private void DisplaySMSHistory()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT sms.ID, CompanyMemberIDList, ExternalCompanyMemberIDList, SMSMessage, SendType, sr.Time, sr.Param1, sr.Param2 ");
            sb.Append("  FROM SDMSSMSHistory as sms INNER JOIN SensorReactionHistory as sr ON sms.ReactionHistoryID=sr.ID ");
            sb.AppendFormat(" WHERE sms.SensorHistoryID = {0} ", m_nHistoryID);

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(sb.ToString());
            if (arrResult == null)
                return;

            int count = 1;
            for (int i = 0; i < arrResult.Count; i+=8)
            {
                int nID = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strCompanyMemberList = DBUtility2.WebDBManager.GetStringField(arrResult[i + 1]);
                string strExternalCompanyMemberList = DBUtility2.WebDBManager.GetStringField(arrResult[i + 2]);
                string strMessage = DBUtility2.WebDBManager.GetStringField(arrResult[i + 3]);
                bool bSendType = DBUtility2.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1) == 1;
                DateTime dtTime = DBUtility2.WebDBManager.GetDateTimeField(arrResult[i + 5].ToString(), DateTime.Now);
                int nEquipZoneID = DBUtility2.WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                int nSensorZoneID = DBUtility2.WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);

                string strLocation = "";
                if (nSensorZoneID >= (int)SOPWebServer.Header.ManualReportDefaultID) // 수동신고인경우 param1 이 zoneid, 자동인경우 equipzoneid
                {
                    strLocation = "[수동신고]";
                    Zone zone = ZoneManager.Instance.GetZone(nEquipZoneID);
                    if (zone != null)
                    {
                        strLocation += zone.ZoneName;
                        if (zone.Floor != null)
                            strLocation += " " + zone.Floor.ToString();
                    }
                }
                else
                {
                    EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);
                    if (equipZone != null)
                    {
                        strLocation = equipZone.ZoneName;
                    }
                }

                if (bSendType)
                    strMessage = "시스템 전송 : " + strMessage;
                else
                    strMessage = "수동 전송 : " + strMessage;

                int memberCount = 0;
                string[] member = strCompanyMemberList.Split(',');
                for (int j = 0; j < member.Length; j++)
                {
                    if (member[j].Trim().Length == 0)
                        continue;

                    memberCount++;
                }

                string[] member2 = strExternalCompanyMemberList.Split(',');
                for (int j = 0; j < member2.Length; j++)
                {
                    if (member2[j].Trim().Length == 0)
                        continue;

                    memberCount++;
                }

                dgvSMS.Rows.Add(count, dtTime, strLocation, memberCount, strMessage);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
