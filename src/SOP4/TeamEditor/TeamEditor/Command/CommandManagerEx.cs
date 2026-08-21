using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamEditor.Command
{
    public class CommandManagerEx : UnE.Command.CommandManager
    {
        private class CommandButtonStatus
        {
            private bool m_bSaveEnable = false;
            private bool m_bEditEnable = true;
            private bool m_bReDoEnable = false;
            private bool m_bUnDoEnable = false;

            public bool SaveEnable
            {
                get { return m_bSaveEnable; }
                set { m_bSaveEnable = value; }
            }

            public bool EditEnable
            {
                get { return m_bEditEnable; }
                set { m_bEditEnable = value; }
            }

            public bool ReDoEnable
            {
                get { return m_bReDoEnable; }
                set { m_bReDoEnable = value; }
            }

            public bool UnDoEnable
            {
                get { return m_bUnDoEnable; }
                set { m_bUnDoEnable = value; }
            }
        }

        public event EventHandler SaveForOptionData;

        private DBUtility.WebDBManager m_dbMgr = null;
        private UnE.GUI.RibbonButton m_btnSave = null;
        private UnE.GUI.RibbonButton m_btnEdit = null;
        private UnE.GUI.RibbonButton m_btnReDo = null;
        private UnE.GUI.RibbonButton m_btnUnDo = null;

        private CommandButtonStatus m_btnLastStatus = null;
        private CommandButtonStatus m_btnEnableStatus = null;

        private bool m_bTargetIsOptionMode = false;

        private int m_nSaveIndex = -1;
        private int m_nSDMSConfig = 0;

        public CommandManagerEx(UnE.GUI.RibbonButton btnUndo, UnE.GUI.RibbonButton btnRedo, UnE.GUI.RibbonButton btnSave, UnE.GUI.RibbonButton btnEdit, DBUtility.WebDBManager dbMgr)
            : base(btnUndo, btnRedo)
        {
            m_btnSave = btnSave;
            m_btnEdit = btnEdit;
            m_btnReDo = btnRedo;
            m_btnUnDo = btnUndo;

            m_dbMgr = dbMgr;

            m_btnLastStatus = new CommandButtonStatus();
            m_btnEnableStatus = new CommandButtonStatus();

            if (m_btnSave != null)
            {
                m_btnSave.Click += new System.EventHandler(this.OnClickSave);
                m_btnSave.Enabled = false;
            }

            if (m_btnEdit != null)
            {
                m_btnEdit.Click += new System.EventHandler(this.OnEditSave);
            }

        }

        private void OnEditSave(object sender, EventArgs e)
        {
            if (m_btnEdit.IsChecked)
            {
                m_btnReDo.Enabled = m_btnEnableStatus.ReDoEnable;
                m_btnUnDo.Enabled = m_btnEnableStatus.UnDoEnable;
                m_btnSave.Enabled = m_btnEnableStatus.SaveEnable;
            }
            else
            {
                m_btnEnableStatus.ReDoEnable = m_btnReDo.Enabled;
                m_btnEnableStatus.UnDoEnable = m_btnUnDo.Enabled;
                m_btnEnableStatus.SaveEnable = m_btnSave.Enabled;

                m_btnReDo.Enabled =
                m_btnUnDo.Enabled =
                m_btnSave.Enabled = false;
            }
        }

        private void OnClickSave(object sender, EventArgs e)
        {
            if (m_bTargetIsOptionMode == true)
            {
                if (SaveForOptionData != null)
                {
                    SaveForOptionData(sender, e);
                }
            }
            else
            {
                if (m_dbMgr != null)
                {
                    SaveDB();
                }
            }
        }

        private void SaveDB()
        {
            /*foreach (CommandEx cmd in m_commands)
            {
                cmd.SaveDB(m_dbMgr);
            }*/

            m_nSDMSConfig = 0;
            int nCommandCount = m_commands.Count;

            if (m_nSaveIndex < m_nIndex)
            {
                // Redo 실행중
                for (int i = m_nSaveIndex + 1; i <= m_nIndex; i++)
                {
                    CommandEx cmd = (CommandEx)m_commands[i];
                    cmd.SaveDB(m_dbMgr, true);
                }

            }
            else
            {
                // Undo 실행중
                for (int i = m_nSaveIndex; i > m_nIndex; i--)
                {
                    CommandEx cmd = (CommandEx)m_commands[i];
                    cmd.SaveDB(m_dbMgr, false);
                }
            }

            // Undo Save
            /*for (int i = nCommandCount - 1; i > m_nIndex;i-- )
            {
                CommandEx cmd = (CommandEx)m_commands[i];
                cmd.SaveDB(m_dbMgr, false);
            }

            // Redo Save
            for (int i = 0; i <= m_nIndex; i++)
            {
                CommandEx cmd = (CommandEx)m_commands[i];
                cmd.SaveDB(m_dbMgr, true);
            }*/

            // TeamEditor에서 변경사항이 생겼음을 SOP Server에게 알린다.
            UpdateSDMSConfig();
            m_nSDMSConfig = 0;

            SaveLastAccessedMemberTime(DateTime.Now);

            if (m_btnSave != null)
            {
                m_btnSave.Enabled = false;
            }

            m_nSaveIndex = m_nIndex;
        }

        // 마지막으로 인사정보(정규조직, 협력업체 조직, 비상조직, 사용자정의, 기타 직원들... 포함)가 수정된 시간을 저장한다.
        private void SaveLastAccessedMemberTime(DateTime time)
        {
            string strSQL = "Select ID from OptionSOPSimulator where PropertyName = 'LastAccessedMemberTime' and SiteID = " + FormMain.Instance.SiteID.ToString();
            System.Collections.ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            string strTime = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);

            if (arrResult.Count == 1)
            {
                DBUtility.VariousData<int> nID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString());

                if (nID == null)
                    return;

                strSQL = "Update OptionSOPSimulator set PropertyValue = " + strTime + " where ID = " + nID.Data.ToString();
                m_dbMgr.GetResultData(strSQL, 0);
            }
            else
            {
                string strFormat = "Insert into OptionSOPSimulator (PropertyName, PropertyValue, SiteID, Description) values ";
                strFormat += "('LastAccessedMemberTime', {0}, {1}, '마지막으로 인사정보(외부, 사용자정의 포함)가 수정된 시간')";

                strSQL = string.Format(strFormat, strTime, FormMain.Instance.SiteID.ToString());
                m_dbMgr.GetResultData(strSQL, 0);
            }
        }

        public override void AddCommand(UnE.Command.Command cmd)
        {
            base.AddCommand(cmd);

            if (m_btnSave != null)
                m_btnSave.Enabled = true;
        }

        public override void Clear()
        {
            base.Clear();

            if (m_btnSave != null)
                m_btnSave.Enabled = false;
        }

        public override void Undo()
        {
            base.Undo();

            if (m_nIndex != m_nSaveIndex && m_btnSave != null)
                m_btnSave.Enabled = true;
            else
                m_btnSave.Enabled = false;
            /*if (m_nIndex < 0 && m_btnSave != null)
                m_btnSave.Enabled = false;*/
        }

        public override void Redo()
        {
            base.Redo();

            if (m_nIndex != m_nSaveIndex && m_btnSave != null)
                m_btnSave.Enabled = true;
            else
                m_btnSave.Enabled = false;
            /*if (m_btnSave != null)
                m_btnSave.Enabled = true;*/
        }

        /// <summary>
        /// 사용자가 보는 화면이 옵션화면인 경우 커맨드 버튼의 대상 변경
        /// </summary>
        /// <param name="bTargetIsOptionMode"></param>
        public void ChangeCommandTarget(bool bTargetIsOptionMode)
        {
            // 기능 미사용.
            return;

            if (bTargetIsOptionMode == m_bTargetIsOptionMode)
                return;

            m_bTargetIsOptionMode = bTargetIsOptionMode;

            if (bTargetIsOptionMode == true)
            {
                m_btnLastStatus.SaveEnable = m_btnSave.Enabled;
                m_btnLastStatus.EditEnable = m_btnEdit.Enabled;
                m_btnLastStatus.ReDoEnable = m_btnReDo.Enabled;
                m_btnLastStatus.UnDoEnable = m_btnUnDo.Enabled;

                m_btnSave.Enabled = true;
                m_btnEdit.Enabled =
                m_btnReDo.Enabled =
                m_btnUnDo.Enabled = false;
            }
            else
            {
                m_btnSave.Enabled = m_btnLastStatus.SaveEnable;
                m_btnEdit.Enabled = m_btnLastStatus.EditEnable;
                m_btnReDo.Enabled = m_btnLastStatus.ReDoEnable;
                m_btnUnDo.Enabled = m_btnLastStatus.UnDoEnable;
            }
        }

        public void SetSDMSConfig(int nConfig)
        {
            m_nSDMSConfig |= nConfig;
        }

        // TeamEditor에서 변경사항이 생겼음을 SOP Server에게 알린다.
        protected bool UpdateSDMSConfig()
        {
            string strTableName = "OptionSDMS", strPropertyName = "SDMSConfig";

            string strSQL = string.Format("Select PropertyValue from {0} where PropertyName = '{1}'", strTableName, strPropertyName);
            System.Collections.ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            if (arrResult.Count == 0)
            {
                strSQL = string.Format("Insert into {0} (PropertyName, PropertyValue, Description, SiteID) values ('{1}', '{2}', NULL, {3})",
                    strTableName, strPropertyName, m_nSDMSConfig, FormMain.Instance.SiteID);

                arrResult = m_dbMgr.GetResultData(strSQL, 0);
            }
            else
            {
                int nConfig = 0;
                DBUtility.VariousData<int> config = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString());

                if (config == null)
                    nConfig = m_nSDMSConfig;
                else
                    nConfig = config.Data | m_nSDMSConfig;

                strSQL = string.Format("Update {0} set PropertyValue = '{1}' where PropertyName = '{2}'", strTableName, nConfig, strPropertyName);
                arrResult = m_dbMgr.GetResultData(strSQL, 0);
            }

            return arrResult != null;
        }
    }
}
