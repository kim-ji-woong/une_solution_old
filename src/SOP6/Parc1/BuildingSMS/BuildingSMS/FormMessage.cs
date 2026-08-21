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

namespace BuildingSMS
{
    public partial class FormMessage : Form
    {
        private Building m_building = null;
        private int m_nFloorIndex = 0;
        private string m_strMessage = "";
        private SendManager m_sendManager = null;

        private int m_nReceiverIndex = 0;
        private int m_nReceiverGroupCount = 0;
        private bool m_finishCalc = false;
        // Key : 수신자 그룹 Index
        // Value : 층번호 List
        private Dictionary<int, List<int>> m_dicFloorList = new Dictionary<int, List<int>>();

        private List<string> m_strTeamTags = new List<string>();
        private WebDBManager m_dbMgr = null;
        private int m_nActionStepHistoryID = -1;
        private bool m_isNormal = true;

        private List<string> m_phoneNumbers = null;
        private Network.NetworkManager m_netMgr = null;

        public FormMessage(Building building, int nFloorIndex, string strMessage)
        {
            InitializeComponent();

            m_building = building;
            m_nFloorIndex = nFloorIndex;
            m_strMessage = strMessage;

            string strValue = System.Configuration.ConfigurationManager.AppSettings["teamTag"].ToString().Trim();

            if (strValue != null && strValue.Length > 0)
            {
                string[] tags = strValue.Split(',');

                foreach (string strTag in tags)
                {
                    m_strTeamTags.Add(strTag.Trim());
                }
            }

            m_dbMgr = ZoneManager.GetDBManager();

            m_nActionStepHistoryID = GetActionStepHistoryID();

            if (m_nActionStepHistoryID > 0)
            {
                bool isNormal;

                if (GetActionStepInfo(out isNormal))
                    m_isNormal = isNormal;
            }

            m_netMgr = new Network.NetworkManager(m_dbMgr);

            label1.Select();
        }

        private int GetActionStepHistoryID()
        {
            System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
            string strFileName = process.ProcessName + ".aid";

            if (System.IO.File.Exists(strFileName))
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(strFileName, Encoding.UTF8);
                string strActionStepID = reader.ReadLine().Trim();
                reader.Close();

                int nActionStepHistoryID;

                if (int.TryParse(strActionStepID, out nActionStepHistoryID))
                    return nActionStepHistoryID;

                System.IO.File.Delete(strFileName);
            }

            return -1;
        }

        private bool GetActionStepInfo(out bool isNormal)
        {
            isNormal = true;

            string strSQL = "Select ash.ID, step.ID, ash.RealMode, v.isNormal from ActionStepHistory as ash, ActionStep as step, Disaster as d, Version as v ";
            strSQL += "where ash.ActionStepID = step.ID and step.DisasterID = d.ID and d.VersionID = v.ID and ash.ID = " + m_nActionStepHistoryID.ToString();
            System.Collections.ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 4)
                return false;

            VariousData<int> normal = WebDBManager.GetIntField(arrResult[3].ToString());

            if (normal != null)
            {
                isNormal = normal.Data == 1;
                return true;
            }

            return false;
        }

        private void FormMessage_Load(object sender, EventArgs e)
        {
            if (m_building.BuildingName == "호텔")
                m_sendManager = new HotelManager(m_building, m_nFloorIndex);
            else if (m_building.BuildingName == "리테일")
                m_sendManager = new RetailManager(m_building, m_nFloorIndex);
            else if (m_building.BuildingName == "타워1")
                m_sendManager = new Tower01Manager(m_building, m_nFloorIndex);
            else if (m_building.BuildingName == "타워2")
                m_sendManager = new Tower02Manager(m_building, m_nFloorIndex);

            if (m_sendManager != null)
            {
                List<int> floors = new List<int>();
                bool isLast;

                if (GetNext(floors, out isLast) == false)
                {
                    MessageBox.Show("계산할 수 없습니다.");
                }
            }

            textBoxMessage.Text = m_strMessage;
        }

        private void btnSendSMS_Click(object sender, EventArgs e)
        {
            if (m_sendManager == null)
                return;

            string strMessage = textBoxMessage.Text.Trim();

            if (strMessage.Length == 0)
                return;

            SMSManager.SendSMS(m_phoneNumbers, strMessage, m_dbMgr.SiteID);

            if (btnNext.Enabled)
                btnNext_Click(null, null);
        }

        private bool GetNext(List<int> floors, out bool isLast)
        {
            bool result = false;
            int nReceiverIndex = m_nReceiverIndex;

            if (m_nReceiverGroupCount == 0)
            {
                result = m_sendManager.GetNext(floors, out isLast);
                m_finishCalc = isLast;

                m_nReceiverGroupCount++;

                if (result)
                    m_dicFloorList[m_nReceiverIndex] = floors;

                m_nReceiverIndex = m_nReceiverGroupCount;
            }
            else
            {
                List<int> prevFloors;

                if (m_dicFloorList.TryGetValue(m_nReceiverIndex, out prevFloors))
                {
                    result = true;
                    isLast = m_nReceiverIndex == m_nReceiverGroupCount - 1 && m_finishCalc;
                    floors.AddRange(prevFloors);
                }
                else
                {
                    result = m_sendManager.GetNext(floors, out isLast);
                    m_finishCalc = isLast;

                    m_nReceiverGroupCount++;

                    if (result)
                        m_dicFloorList[m_nReceiverIndex] = floors;

                    m_nReceiverIndex = m_nReceiverGroupCount;
                }
            }

            if (result)
                SetReceivers(nReceiverIndex);

            btnSendSMS.Enabled = m_phoneNumbers != null && m_phoneNumbers.Count > 0;
            btnNext.Enabled = m_nReceiverIndex < m_nReceiverGroupCount || !m_finishCalc;
            btnPrev.Enabled = m_nReceiverIndex > 1;

            return result;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (m_dicFloorList.ContainsKey(m_nReceiverIndex - 2))
            {
                SetReceivers(m_nReceiverIndex - 2);

                m_nReceiverIndex--;
                btnSendSMS.Enabled = m_phoneNumbers != null && m_phoneNumbers.Count > 0;
                btnNext.Enabled = true;
                btnPrev.Enabled = m_nReceiverIndex > 1;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (m_dicFloorList.ContainsKey(m_nReceiverIndex))
            {
                SetReceivers(m_nReceiverIndex);

                m_nReceiverIndex++;
                btnSendSMS.Enabled = m_phoneNumbers != null && m_phoneNumbers.Count > 0;
                btnNext.Enabled = m_nReceiverIndex < m_nReceiverGroupCount || !m_finishCalc;
                btnPrev.Enabled = true;
            }
            else
            {
                List<int> floors = new List<int>();
                bool isLast;

                GetNext(floors, out isLast);
            }
        }

        private void SetReceivers(int nReceiverIndex)
        {
            List<int> floorList;

            if (m_dicFloorList.TryGetValue(nReceiverIndex, out floorList) == false)
                return;

            string strLog;
            m_phoneNumbers = null;

            if (SMSManager.GetReceivers(m_dbMgr, m_isNormal, m_strTeamTags, m_building, floorList, out strLog, out m_phoneNumbers) == false)
            {
                m_phoneNumbers = null;
                textBoxReceivers.Text = "";
                btnSendSMS.Enabled = false;
            }
            else
            {
                textBoxReceivers.Text = strLog;
                btnSendSMS.Enabled = true;
            }
        }

        private void FormMessage_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_netMgr.Close();
        }
    }
}
