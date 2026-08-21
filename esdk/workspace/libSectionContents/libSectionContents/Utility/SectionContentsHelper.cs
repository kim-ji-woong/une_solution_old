using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.SOP.Sections;
using UnE.SOP.Workstate;
using DBUtility2;
using Sections;
using SOPManager.Popup.SpecialMessagePanels;
using System.Collections;
using UnE.SOP;

namespace SectionContents.Utility
{
    public class SectionContentsHelper
    {
        public static void SetTitle(ISectionContents contents, string strTask, DateTime time, string strStatus)
        {
            if (contents != null && contents.Section != null)
            {
                SectionTabPage page = (SectionTabPage)contents.Section.GetParent().Parent;

                if (page == null)
                    return;

                WorkFlow work = WorkFlowManager.Instance.Get(page.ActionStepID, !page.VirtualMode);

                if (work != null)
                {
                    if (work.Option != null && work.Option.HasPosition && work.Option.DetectTime != null)
                    {
                        string strLocation = null;
                        VariousData<DateTime> dtDetect = null;
                        string strPSMMaterialName = null;
                        VariousData<int> psmDistance = null;
                        string strAmountSnowfall = null;
                        string strAlarmMessage = "";

                        GetOptionInfo(work.Option, out strLocation, out dtDetect, out strPSMMaterialName, out psmDistance, out strAmountSnowfall, out strAlarmMessage);
                        contents.Title = Parse(strTask, dtDetect, strLocation, strPSMMaterialName, psmDistance, strAmountSnowfall, strAlarmMessage);
                    }
                }
            }
        }

        private static bool GetOptionInfo(WorkflowOption option, out string strLocation, out VariousData<DateTime> dtDetect, out string strPSMMaterialName, out VariousData<int> psmDistance, out string strAmountSnowfall, out string strAlarmMessage)
        {
            strLocation = strPSMMaterialName = strAmountSnowfall = null;
            dtDetect = null;
            psmDistance = null;
            strAlarmMessage = "";

            if (option == null)
                return false;

            if (option.HasPosition)
                strLocation = option.PositionName;

            dtDetect = option.DetectTime;
            strAlarmMessage = option.AlarmMessage;

            if (option is WorkflowOptionPSM)
            {
                WorkflowOptionPSM _option = (WorkflowOptionPSM)option;

                if (_option.PSMMaterial != null)
                    strPSMMaterialName = _option.PSMMaterial.MaterialName;

                psmDistance = new VariousData<int>(_option.PSMDistance);
            }
            else if (option is WorkflowOptionSnowFall)
            {
                WorkflowOptionSnowFall _option = (WorkflowOptionSnowFall)option;

                if (_option.UseAmountSnowFall && _option.AmountSnowFall > 0.0)
                    strAmountSnowfall = _option.AmountSnowFall.ToString();
            }

            return true;
        }

        public static string Parse(string strMsg, VariousData<DateTime> dtDetect, string strLocation, string strPSMMaterialName, VariousData<int> psmDistance, string strAmountSnowfall, string strAlarmMessage)
        {
            UnE.SOP.Utility.SOPSimulatorScript.DataParameter param = new UnE.SOP.Utility.SOPSimulatorScript.DataParameter(strMsg, dtDetect == null ? DateTime.Now : dtDetect.Data, strLocation, strAlarmMessage);

            if (strPSMMaterialName != null && strPSMMaterialName.Length > 0)
            {
                param.PSMMaterialType = strPSMMaterialName;
                param.PSMDistance = psmDistance.Data;
            }

            if (strAmountSnowfall != null && strAmountSnowfall.Length > 0)
                param.AmountSnowfall = strAmountSnowfall;

            return UnE.SOP.Utility.SOPSimulatorScript.Parse(param);
        }

        public static void ChangeTitle(ISectionContents contents)
        {
            if (contents.Section == null)
                return;

            Section.ComponentType type = contents.Section.GetComponentType();

            if (type == Sections.Section.ComponentType.INTERNAL)
            {
                SectionDataInternal data = (SectionDataInternal)contents.Section.Data;

                string[] arrHeadTitle = { "(문자)", "(문자전파)", "(방송)", "(방송전파)" };
                string strSMS = "(문자)", strBroadcast = "(방송)";
                string strNumber, strTitle;

                GetContentsText(contents, out strNumber, out strTitle);

                foreach (string strHeadTitle in arrHeadTitle)
                {
                    if (strTitle.StartsWith(strHeadTitle))
                    {
                        strTitle = strTitle.Replace(strHeadTitle, "").Trim();
                        break;
                    }
                }

                if (data.UseBroadcast)
                {
                    if (strTitle.Contains(strBroadcast) == false)
                        contents.Title = strNumber + ". " + strBroadcast + strTitle;
                }
                else if (data.UseMobileApp)
                {
                    if (strTitle.Contains(strSMS) == false)
                        contents.Title = strNumber + ". " + strSMS + strTitle;
                }
            }
        }

        public static void ChangeTitle(ISectionContents contents, string strNewTitle, WorkflowOption option)
        {
            string strLocation = null;
            VariousData<DateTime> dtDetect = null;
            string strPSMMaterialName = null;
            VariousData<int> psmDistance = null;
            string strAmountSnowfall = null;
            string strAlarmMessage = "";

            if (GetOptionInfo(option, out strLocation, out dtDetect, out strPSMMaterialName, out psmDistance, out strAmountSnowfall, out strAlarmMessage) == false)
                return;

            if (strLocation != null && dtDetect != null)
                contents.Title = Parse(strNewTitle, dtDetect, strLocation, strPSMMaterialName, psmDistance, strAmountSnowfall, strAlarmMessage);
            else
                contents.Title = strNewTitle;
        }

        private static void GetContentsText(ISectionContents contents, out string strNumber, out string strTitle)
        {
            strNumber = "";
            strTitle = contents.Title;

            int nDotIndex = strTitle.IndexOf('.');

            if (nDotIndex <= 0)
                return;

            string str = strTitle.Substring(0, nDotIndex);

            int num;

            if (!int.TryParse(str, out num))
                return;

            strNumber = num.ToString();

            int len = strTitle.Length;

            for (int i = nDotIndex + 1; i < len; i++)
            {
                char ch = strTitle.ElementAt(i);

                if (i > nDotIndex + 1 || (ch != ' ' && ch != '\t'))
                {
                    strTitle = strTitle.Substring(i);
                    break;
                }
            }
        }

        public static void RunDecision(ISectionContents contents, ProcessButton btn, bool initState)
        {
            if (contents.Section == null || contents.Section.GetComponentType() != Sections.Section.ComponentType.DECISION)
                return;

            if (PostRunDecision(contents, btn))
            {
                if (initState)
                {
                    Sections.PanelSection panel = (Sections.PanelSection)contents.Section.GetParent();
                    SectionTabPage tabPage = (SectionTabPage)panel.Parent;
                    SectionState state = WorkFlowManager.Instance.Find(contents.Section, !tabPage.VirtualMode);

                    if (state != null)
                    {
                        // 재실행하는 것이므로 일단 초기화시킨다.
                        state.InitState();
                    }
                }

                btn.OnClick();
            }
        }

        private static bool PostRunDecision(ISectionContents contents, ProcessButton btn)
        {
            return true;
            /*int nItemCount = cboDecisions.Items.Count;

            for (int i = 0; i < nItemCount; i++)
            {
                DecisionProcessButton button = (DecisionProcessButton)cboDecisions.Items[i];

                if (button.ProcessButton == btn)
                {
                    m_systemCall = true;
                    cboDecisions.SelectedIndex = i;
                    m_systemCall = false;

                    btnExecute.Enabled = false;
                    m_prevDecisionProcessButton = button;
                    return true;
                }
            }

            return false;*/
        }

        public static void ChangeCommanderName(ISectionContents contents)
        {
            /*string strCommanderName = string.Empty;
            SectionState state = GetSectionState(contents);

            bool bChangeActor = false;
            bool bChangePerformer = false;

            if (state == null)
                return;

            if (contents.Section.GetComponentType() == Sections.Section.ComponentType.PROCESS)
            {
                if (contents.Commander != null)
                {
                    labelSender.Text = String.Format("( 발신자 : {0} )", m_commander.DisplayText);

                    // m_commander.Team이 null이 아닐경우 CommanderName이 바뀌는 일은 없다.
                    if (m_commander.Team == null)
                    {
                        bChangeActor = true;
                    }
                }

                LoadComponentAccessedUsers(state);

                int nRowCount = dataGridView.Rows.Count;
                ArrayList arrMissionItem = ((dataGridView.Tag as SectionProcess).Data as Sections.SectionDataProcess).MissionItems;

                for (int i = 0; i < nRowCount; i++)
                {
                    bChangePerformer = false;

                    string strUserName = null;
                    DataGridViewRow row = dataGridView.Rows[i];
                    MissionItem missionItem = arrMissionItem[i] as MissionItem;

                    if (missionItem.Commander != null)
                    {
                        // m_commander.Team이 null이 아닐경우 CommanderName이 바뀌는 일은 없다.
                        if (missionItem.Commander.Team == null)
                        {
                            bChangePerformer = true;
                        }
                    }

                    if (bChangeActor == false && bChangePerformer == false)
                        continue;

                    int nComponentHistoryID;
                    UnE.SOP.History.HistorySectionData.DetailData detail = GetLastDetailData(i, state.DetailDatas, out nComponentHistoryID);

                    if (detail != null)
                    {
                        strCommanderName = GetAccessedUserName(state, nComponentHistoryID, detail.Time);

                        if (strCommanderName != null)
                            strUserName = strCommanderName;
                    }

                    if (strUserName == null)
                    {
                        strUserName = GetCurrentAccessedUserName(state.Time == null ? DateTime.Now : state.Time.Data);

                        if (strUserName == null)
                            strUserName = m_strCommanderName2;
                    }

                    if (bChangeActor)
                    {
                        row.Cells[MISSION_ACTOR_INDEX].Value = strUserName;
                        row.Cells[MISSION_ACTOR_INDEX].ToolTipText = strUserName;
                    }

                    if (bChangePerformer)
                    {
                        row.Cells[MISSION_PERFORMER_INDEX].Value = strUserName;
                        row.Cells[MISSION_PERFORMER_INDEX].ToolTipText = strUserName;
                    }

                    strCommanderName = strUserName;
                }
            }
            else
            {
                Popup.MissionMessage.FormMissionMessage frm = GetFormMissionMessage();

                if (frm != null)
                    strCommanderName = frm.ChangeCommanderName(state);
            }

            if (bChangeActor == true)
            {
                labelSender.Text = (String.IsNullOrWhiteSpace(strCommanderName) ? string.Empty : String.Format("( 발신자 : {0} )", strCommanderName));
            }*/
        }

        public static SectionState GetSectionState(ISectionContents contents)
        {
            if (contents.Section == null)
                return null;

            SectionTabPage page = (SectionTabPage)contents.Section.GetParent().Parent;
            WorkFlow work = (WorkFlow)WorkFlowManager.Instance.Get(page.ActionStepID, !page.VirtualMode);

            if (work == null)
                return null;

            return work.FindState(contents.Section);
        }

        public static string ChangeEarthquakeString(string str, WorkflowOptionEarthquake option, bool romanText = true)
        {
            // 지진규모를 입력한다.
            str = str.Replace(FormSpecialMessageHelpEarthquake.GetVariableString(FormSpecialMessageHelpEarthquake.VariableType.Magnitude), option.Magnitude.ToString());
            // 지진진도를 입력한다.
            string strInensity = romanText ? GetRomanString(option.Intensity) : option.Intensity.ToString();
            str = str.Replace(FormSpecialMessageHelpEarthquake.GetVariableString(FormSpecialMessageHelpEarthquake.VariableType.Intensity), strInensity);
            // 규모
            /*if (option.Mode == WorkflowOptionEarthquake.PowerMode.Magnitude)
            {
                // 지진규모를 입력한다.
                str = str.Replace(FormSpecialMessageHelpEarthquake.GetVariableString(FormSpecialMessageHelpEarthquake.VariableType.Magnitude), option.Magnitude.ToString());
            }
            // 진도
            else if (option.Mode == WorkflowOptionEarthquake.PowerMode.Intensity)
            {
                // 지진진도를 입력한다.
                str = str.Replace(FormSpecialMessageHelpEarthquake.GetVariableString(FormSpecialMessageHelpEarthquake.VariableType.Intensity), option.Intensity.ToString());
            }*/

            str = ChangeCommonString(str, option);
            return str;
        }

        private static string GetRomanString(int nIntensity)
        {
            if (nIntensity == 1)
                return "I";
            else if (nIntensity == 2)
                return "II";
            else if (nIntensity == 3)
                return "III";
            else if (nIntensity == 4)
                return "IV";
            else if (nIntensity == 5)
                return "V";
            else if (nIntensity == 6)
                return "VI";
            else if (nIntensity == 7)
                return "VII";
            else if (nIntensity == 8)
                return "VIII";
            else if (nIntensity == 9)
                return "IX";
            else if (nIntensity == 10)
                return "X";
            else if (nIntensity == 11)
                return "XI";
            else if (nIntensity == 12)
                return "XII";

            return nIntensity.ToString();
        }

        public static string ChangePSMString(string str, WorkflowOptionPSM option)
        {
            if (option.PSMMaterial != null)
            {
                str = str.Replace(FormSpecialMessageHelpPSM.GetVariableString(FormSpecialMessageHelpPSM.VariableType.PSMMaterial), option.PSMMaterial.MaterialName);
            }

            // option.PSMDistance는 미터
            str = str.Replace(FormSpecialMessageHelpPSM.GetVariableString(FormSpecialMessageHelpPSM.VariableType.PSMDistanceM), option.PSMDistance.ToString());
            str = str.Replace(FormSpecialMessageHelpPSM.GetVariableString(FormSpecialMessageHelpPSM.VariableType.PSMDistanceKM), string.Format("{0:F1}", option.PSMDistance / 1000.0));

            str = ChangeCommonString(str, option);
            return str;
        }

        public static string ChangeClimateString(string str, WorkflowOptionSnowFall option)
        {
            if (option.UseAmountSnowFall)
            {
                str = str.Replace(FormSpecialMessageHelpClimate.GetVariableString(FormSpecialMessageHelpClimate.VariableType.SNOW_DEPTH), string.Format("{0:F0}", option.AmountSnowFall));
            }

            str = ChangeCommonString(str, option);
            return str;
        }

        public static string ChangeClimateString(string str, WorkflowOptionWind option)
        {
            str = str.Replace(FormSpecialMessageHelpClimate.GetVariableString(FormSpecialMessageHelpClimate.VariableType.WIND_SPEED), string.Format("{0:F1}", option.WindSpeed));
            str = ChangeCommonString(str, option);
            return str;
        }

        public static string ChangeCommonString(string str, WorkflowOption option)
        {
            if (option.DetectTime != null)
            {
                string strTime = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'", option.DetectTime.Data.Year, option.DetectTime.Data.Month, option.DetectTime.Data.Day, option.DetectTime.Data.Hour, option.DetectTime.Data.Minute, option.DetectTime.Data.Second);
                str = str.Replace(FormSpecialMessageHelpTime.GetVariableString(FormSpecialMessageHelpTime.VariableType.Time), strTime);
            }

            if (option.HasPosition)
            {
                str = str.Replace(FormSpecialMessageHelpLocation.GetVariableString(FormSpecialMessageHelpLocation.VariableType.Location), "'" + option.PositionName + "'");
            }

            if (option.WorkFlow != null)
            {
                string strSOPMode = "", strSOPFullMode = "";
                int nRealMode = option.WorkFlow.RunMode == WorkFlowMode.REAL ? 1 : 0;

                if (UnE.SOP.Utility.SOPSimulatorScript.GetSOPModeString(ref strSOPMode, nRealMode))
                {
                    str = str.Replace(FormSpecialMessageHelpSOPMode.GetVariableString(FormSpecialMessageHelpSOPMode.VariableType.SOPMode), "'" + strSOPMode + "'");
                }

                if (UnE.SOP.Utility.SOPSimulatorScript.GetSOPFullModeString(ref strSOPFullMode, nRealMode))
                {
                    str = str.Replace(FormSpecialMessageHelpSOPMode.GetVariableString(FormSpecialMessageHelpSOPMode.VariableType.SOPFullMode), "'" + strSOPFullMode + "'");
                }
            }

            ChangeUserDefinedString(ref str, option);
            return str;
        }

        private static void ChangeUserDefinedString(ref string str, WorkflowOption option)
        {
            if (option.UserDefinedParameters.Count > 0)
            {
                int nBeginIndex = 0;
                int nIndex = str.IndexOf('{', nBeginIndex);

                while (nIndex >= 0)
                {
                    int nIndex2 = str.IndexOf('}', nIndex + 1);

                    if (nIndex2 < 0)
                        break;

                    string strVariable = str.Substring(nIndex + 1, nIndex2 - nIndex - 1);
                    string strVariable2 = strVariable.Trim();

                    bool find = false;

                    foreach (KeyValuePair<UnE.SOP.SOPParameter, string> pair in option.UserDefinedParameters)
                    {
                        if (string.Compare(strVariable2, pair.Key.VariableName, true) == 0)
                        {
                            if (pair.Key.Type == SectionDataDecision.VariableType.STRING)
                                str = str.Replace("{" + strVariable + "}", "'" + pair.Value + "'");
                            else
                                str = str.Replace("{" + strVariable + "}", pair.Value);

                            nBeginIndex = nIndex + pair.Value.Length;
                            find = true;
                            break;
                        }
                    }

                    if (find == false)
                        nBeginIndex = nIndex2 + 1;

                    nIndex = str.IndexOf('{', nBeginIndex);
                }
            }

            ChangeBooleanType(ref str);
            /*string[] tokens = str.Split(new char[] { ' ', '\t' });
            str = "";

            foreach (string strToken in tokens)
            {
                if (str.Length > 0)
                    str += " ";

                if (strToken == "참" || string.Compare(strToken, "true", true) == 0)
                    str += "1";
                else if (strToken == "거짓" || string.Compare(strToken, "false", true) == 0)
                    str += "0";
                else
                    str += strToken;
            }*/
        }

        private static void ChangeBooleanType(ref string str)
        {
            string[] keys = new string[] { "참", "true", "거짓", "false" };
            string[] values = new string[] { "1", "1", "0", "0" };
            string strLower = str.ToLower();

            for (int i = 0; i < keys.Count(); i++)
            {
                int nBeginIndex = 0;

                while (nBeginIndex < str.Length)
                {
                    int nIndex = FindExpressionWordIndex(strLower, keys[i], nBeginIndex);

                    if (nIndex >= 0)
                    {
                        str = str.Substring(0, nIndex) + values[i] + str.Substring(nIndex + keys[i].Length);
                        strLower = str.ToLower();
                        nBeginIndex = nIndex + keys[i].Length;
                    }
                    else
                        break;
                }
            }
        }

        private static int FindExpressionWordIndex(string str, string strWord, int nBeginIndex)
        {
            int nIndex = str.IndexOf(strWord, nBeginIndex);

            if (nIndex < 0)
                return -1;

            char chBegin = (char)0;
            char chEnd = (char)0;
            int nWordLen = strWord.Length;

            if (nIndex > 0)
                chBegin = str.ElementAt(nIndex - 1);

            if (nIndex + nWordLen < str.Length)
                chEnd = str.ElementAt(nIndex + nWordLen);

            if (!CheckExpressionCharacter(chBegin) || !CheckExpressionCharacter(chEnd))
                return -1;

            return nIndex;
        }

        private static bool CheckExpressionCharacter(char ch)
        {
            if (ch == (char)0 || ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n' || ch == '<' ||
                ch == '>' || ch == '=' || ch == '*' || ch == '/' || ch == '+' || ch == '-' || ch == '(' || ch == ')')
                return true;

            return false;
        }

        // nComponentHistoryID : 이 값이 0보다 작으면 ComponentHistoryID가 생성된 이후에 데이터가 옮겨진다.
        // onlyLastData : 이 값이 true이면 detailDatas에 여러 데이터가 저장될 경우 가장 마지막에 저장된 데이터만 남긴다.
        //                DB에 중복 로그를 저장하지 않도록 하기 위함이다.
        public static void SetDetailDatas(ISectionContents contents, Dictionary<int, List<UnE.SOP.History.HistorySectionData.DetailData>> detailDatas, int nComponentHistoryID = -1, bool onlyLastData = true, MissionData mission = null)
        {
            if (contents.Section == null)
                return;

            Section.ComponentType type = contents.Section.GetComponentType();

            if (type == Sections.Section.ComponentType.PROCESS)
            {
                SectionDataProcess data = (SectionDataProcess)contents.Section.Data;

                bool isSendSMS, isComplete;
                string strSender, strItem, strTeamName, strPerformer;
                VariousData<DateTime> completeTime, executeTime, unCompleteTime;

                for (int i=0;i<data.MissionItems.Count;i++)
                {
                    if (mission != null && i != mission.MissionIndex)
                        continue;

                    if (mission != null)
                    {
                        if (mission.GetProcessType() == MissionData.ProcessType.SendSMS)
                        {
                            UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData((int)UnE.SOP.History.HistorySectionData.DetailData.DataType.SEND_SMS, mission.TimeStamp);
                            AddDetailData(i, detailDatas, detail, nComponentHistoryID);
                        }
                        else if (mission.GetProcessType() == MissionData.ProcessType.CheckedComplete)
                        {
                            UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData((int)UnE.SOP.History.HistorySectionData.DetailData.DataType.COMPLETE_CHECKED, mission.TimeStamp);
                            AddDetailData(i, detailDatas, detail, nComponentHistoryID);
                        }
                        else if (mission.GetProcessType() == MissionData.ProcessType.UncheckedComplete)
                        {
                            UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData((int)UnE.SOP.History.HistorySectionData.DetailData.DataType.COMPLETE_UNCHECKED, mission.TimeStamp);
                            AddDetailData(i, detailDatas, detail, nComponentHistoryID);
                        }
                    }
                    else
                    {
                        if (contents.GetItem(i, out isSendSMS, out isComplete, out strSender, out strItem, out strTeamName, out strPerformer, out executeTime, out completeTime, out unCompleteTime) == false)
                            continue;

                        if (isComplete && completeTime != null)
                        {
                            // 완료 버튼이 눌려졌다.
                            UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData((int)UnE.SOP.History.HistorySectionData.DetailData.DataType.COMPLETE_CHECKED, completeTime.Data);
                            AddDetailData(i, detailDatas, detail, nComponentHistoryID);
                        }
                        else
                        {
                            // 완료 버튼이 해제됐다.
                            UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData((int)UnE.SOP.History.HistorySectionData.DetailData.DataType.COMPLETE_UNCHECKED, unCompleteTime == null ? DateTime.Now : unCompleteTime.Data);
                            AddDetailData(i, detailDatas, detail, nComponentHistoryID);
                        }
                    }
                }
            }
            else if (type == Sections.Section.ComponentType.INTERNAL)
            {
                bool isBroadcast, isExcute, isComplete, useSiren;
                int nBroadcastCount;
                VariousData<DateTime> completeTime, executeTime, unCompleteTime;
                string strMessage;

                if (contents.GetItem(out isBroadcast, out isExcute, out isComplete, out nBroadcastCount, out useSiren, out executeTime, out completeTime, out unCompleteTime, out strMessage))
                {
                    if (isBroadcast)
                    {
                        if (isExcute && executeTime != null)
                        {
                            string strLog = string.Format("{0}, {1}, {2}", 1, useSiren ? 1 : 0, strMessage);
                            UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData(strLog, executeTime.Data);
                            AddDetailData(UnE.SOP.History.HistorySectionData.DetailData.RUN_BROADCAST_INTERNAL, detailDatas, detail, nComponentHistoryID);
                        }

                        if (isComplete && completeTime != null)
                        {
                            // 완료버튼을 누른 경우
                            UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData(1, completeTime.Data);
                            AddDetailData(UnE.SOP.History.HistorySectionData.DetailData.COMPLETE_BROADCAST_INTERNAL, detailDatas, detail, nComponentHistoryID);
                        }
                        else if (unCompleteTime != null)
                        {
                            // 완료버튼을 해제한 경우
                            UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData(0, unCompleteTime.Data);
                            AddDetailData(UnE.SOP.History.HistorySectionData.DetailData.COMPLETE_BROADCAST_INTERNAL, detailDatas, detail, nComponentHistoryID);
                        }
                    }
                    else
                    {
                        // 구현해야함
                        string strCommanderText = "";

                        if (isExcute && executeTime != null)
                        {
                            string strLog = string.Format("[{0}], [{1}], {2}", strCommanderText, contents.TeamName, strMessage);
                            UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData(strLog, executeTime.Data);
                            AddDetailData(UnE.SOP.History.HistorySectionData.DetailData.RUN_SMS_INTERNAL, detailDatas, detail, nComponentHistoryID);
                        }

                        if (isComplete && completeTime != null)
                        {
                            // 완료버튼을 누른 경우
                            UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData(1, completeTime.Data);
                            AddDetailData(UnE.SOP.History.HistorySectionData.DetailData.COMPLETE_SMS_INTERNAL, detailDatas, detail, nComponentHistoryID);
                        }
                        else if (unCompleteTime != null)
                        {
                            // 완료버튼을 해제한 경우
                            UnE.SOP.History.HistorySectionData.DetailData detail = new UnE.SOP.History.HistorySectionData.DetailData(0, unCompleteTime.Data);
                            AddDetailData(UnE.SOP.History.HistorySectionData.DetailData.COMPLETE_SMS_INTERNAL, detailDatas, detail, nComponentHistoryID);
                        }
                    }
                }
            }

            if (onlyLastData)
            {
                List<UnE.SOP.History.HistorySectionData.DetailData> datas = null;
                UnE.SOP.History.HistorySectionData.DetailData lastData = null;
                int nKey = -1;
                bool isFirst = true;

                foreach (KeyValuePair<int, List<UnE.SOP.History.HistorySectionData.DetailData>> pair in detailDatas)
                {
                    foreach (UnE.SOP.History.HistorySectionData.DetailData data in pair.Value)
                    {
                        if (isFirst)
                        {
                            lastData = data;
                            datas = pair.Value;
                            nKey = pair.Key;
                            isFirst = false;
                        }
                        else
                        {
                            if ((lastData.Time == null && data.Time != null) ||
                                (lastData.Time != null && data.Time != null && lastData.Time.Data < data.Time.Data))
                            {
                                lastData = data;
                                datas = pair.Value;
                                nKey = pair.Key;
                            }
                        }
                    }
                }

                if (lastData != null)
                {
                    detailDatas.Clear();
                    datas.Clear();
                    datas.Add(lastData);
                    detailDatas[nKey] = datas;
                }
            }
        }

        private static void AddDetailData(int nRowIndex, Dictionary<int, List<UnE.SOP.History.HistorySectionData.DetailData>> detailDatas, UnE.SOP.History.HistorySectionData.DetailData detail, int nComponentHistoryID)
        {
            detail.DataIndex = new VariousData<int>(nRowIndex);

            foreach (KeyValuePair<int, List<UnE.SOP.History.HistorySectionData.DetailData>> pair in detailDatas)
            {
                foreach (UnE.SOP.History.HistorySectionData.DetailData data in pair.Value)
                {
                    // 이미 같은 데이터가 존재하면 다시 저장하지 않는다.
                    if (data.Equals(detail))
                        return;
                }
            }

            List<UnE.SOP.History.HistorySectionData.DetailData> details = null;

            if (!detailDatas.TryGetValue(nComponentHistoryID, out details))
            {
                details = new List<UnE.SOP.History.HistorySectionData.DetailData>();
                detailDatas[nComponentHistoryID] = details;
            }

            if (!details.Contains(detail))
                details.Add(detail);
        }

        public static string ChangeText(string strMessage, WorkflowOption option, bool earthquakeRomanText = true)
        {
            if (option == null)
                return strMessage;

            if (option is UnE.SOP.Workstate.WorkflowOptionEarthquake)
            {
                strMessage = ChangeEarthquakeString(strMessage, (UnE.SOP.Workstate.WorkflowOptionEarthquake)option, earthquakeRomanText);
            }
            else if (option is UnE.SOP.Workstate.WorkflowOptionPSM)
            {
                strMessage = ChangePSMString(strMessage, (UnE.SOP.Workstate.WorkflowOptionPSM)option);
            }
            else if (option is UnE.SOP.Workstate.WorkflowOptionSnowFall)
            {
                strMessage = ChangeClimateString(strMessage, (UnE.SOP.Workstate.WorkflowOptionSnowFall)option);
            }
            else if (option is WorkflowOptionWind)
            {
                strMessage = SectionContentsHelper.ChangeClimateString(strMessage, (WorkflowOptionWind)option);
            }
            else if (option != null)
            {
                strMessage = ChangeCommonString(strMessage, option);
            }

            UnE.SOP.Utility.SOPSimulatorScript.DataParameter param = new UnE.SOP.Utility.SOPSimulatorScript.DataParameter(strMessage, option.DetectTime == null ? DateTime.Now : option.DetectTime.Data, option.PositionName, option.AlarmMessage);
            return UnE.SOP.Utility.SOPSimulatorScript.Parse(param);
        }

        public static void AddDecisionProcessButton(List<DecisionProcessButton> buttons, ProcessButtonManager mgr, Arrow.ArrowPosition pos)
        {
            ProcessButton btn = mgr.FindButton(pos);

            if (btn != null && btn.Data != null)
            {
                foreach (Arrow arrow in btn.Data.Arrows)
                {
                    if (arrow.EndLink.GetComponentType() == Sections.Section.ComponentType.ANNOTATION)
                        continue;

                    string strArrowText = "";

                    if (arrow.Text.Length > 0)
                    {
                        if (arrow.EndLink.Data.SectionNumber > 0)
                            strArrowText = string.Format("{0}({1})", arrow.EndLink.Data.SectionNumber, arrow.Text);
                        else
                            strArrowText = string.Format("{0}({1})", arrow.EndLink.Title, arrow.Text);
                    }
                    else
                    {
                        if (arrow.EndLink.Data.SectionNumber > 0)
                        {
                            if (arrow.EndLink.GetComponentType() == Sections.Section.ComponentType.ENDPOINT)
                                strArrowText = string.Format("{0}({1})", arrow.EndLink.Data.SectionNumber, arrow.EndLink.Title);
                            else
                                strArrowText = string.Format("{0}", arrow.EndLink.Data.SectionNumber);
                        }
                        else
                            strArrowText = string.Format("{0}", arrow.EndLink.Title);
                    }

                    DecisionProcessButton button = new DecisionProcessButton(strArrowText, btn);
                    buttons.Add(button);
                }
            }
        }

        public static void SendLogState(ISectionContents contents, SectionState state = null, WorkFlow workFlow = null, MissionData mission = null)
        {
            SectionTabPage page = null;

            if (state == null)
            {
                PanelSection panel = contents.Section.GetParent();
                page = (SectionTabPage)panel.Parent;
                state = WorkFlowManager.Instance.Find(contents.Section, !page.VirtualMode);
            }

            if (state != null)
            {
                if (workFlow == null)
                {
                    if (page == null)
                    {
                        page = (SectionTabPage)contents.Section.GetParent().Parent;
                    }

                    workFlow = WorkFlowManager.Instance.Get(page.ActionStepID, !page.VirtualMode);
                }

                if (workFlow != null)
                {
                    SetDetailDatas(contents, state.DetailDatas, -1, false, mission);
                    workFlow.LogState(contents.Section, state, state.CheckedRun, state.CheckedComplete);
                }
            }
        }

        // 리턴값 : 문자메시지 수신자 리스트
        public static ArrayList GetSMSInfo(ISectionContents contents, out string strSender, out Dictionary<string, string> dicPhoneNumbers)
        {
            dicPhoneNumbers = null;
            strSender = "";

            if (contents.ContentsOwner == null)
                return null;

            strSender = contents.ContentsOwner.GetSMSCaller(contents);

            if (strSender.Length > 0)
            {
                ArrayList arrTeamList;
                bool onlyTeamLeader;
                string strReceiverNames;

                ArrayList arrPhoneNumbers = GetReceiverInfo(contents.Section, contents.ContentsOwner, out strReceiverNames, out arrTeamList, out onlyTeamLeader, out dicPhoneNumbers);

                if (arrPhoneNumbers != null && dicPhoneNumbers != null)
                {
                    int nArrCount = arrPhoneNumbers.Count;

                    for (int i=nArrCount-1;i>=0;i--)
                    {
                        string strPhoneNumber = (string)arrPhoneNumbers[i];

                        if (strPhoneNumber.Length == 0)
                        {
                            arrPhoneNumbers.RemoveAt(i);
                            dicPhoneNumbers.Remove(strPhoneNumber);
                        }
                    }
                }

                return arrPhoneNumbers;
            }

            return null;
        }

        public static ArrayList GetReceiverInfo(Sections.Section section, ISectionContentsOwner owner, out string strReceiverName, out ArrayList arrTeamList, out bool onlyTeamLeader, out Dictionary<string, string> dicPhoneNumbers)
        {
            arrTeamList = null;
            strReceiverName = "";
            onlyTeamLeader = true;
            ArrayList arrReceiverPhoneNumbers = null;
            dicPhoneNumbers = null;
            //bool includeChildTeams = false;

            Sections.Section.ComponentType type = section.GetComponentType();

            if (type == Sections.Section.ComponentType.PROCESS)
            {
                Sections.SectionProcess process = (Sections.SectionProcess)section;

                strReceiverName = process.TextDown;

                Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

                onlyTeamLeader = /*true;*/data.TransferTeamLeaderOnly;
                arrTeamList = data.TeamList;

                /*string strNames;
                m_receiverPhoneNumbers = GetReceiverPhoneNumbers(data.TeamList, data.TransferTeamLeaderOnly, out strNames);

                if (m_receiverPhoneNumbers != null)
                    m_strReceiverName = strReceiverName;*/
            }
            else if (type == Sections.Section.ComponentType.INTERNAL)
            {
                Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;

                // 내부상황전파는 무조건 전체 팀원에게 보낸다.
                onlyTeamLeader = false;// data.TransferTeamLeaderOnly;
                arrTeamList = data.TeamList;
                //includeChildTeams = true;
            }

            if (arrTeamList != null)
            {
                strReceiverName = "";

                foreach (SOPTeam team in arrTeamList)
                {
                    string strTeamName = team.IncludeChildTeams ? team.TeamName + "(+)" : team.TeamName;

                    if (strReceiverName.Length == 0)
                        strReceiverName = strTeamName;
                    else
                        strReceiverName += ", " + strTeamName;
                }

                string strNames;
                arrReceiverPhoneNumbers = GetReceiverPhoneNumbers(owner, arrTeamList, onlyTeamLeader, /*includeChildTeams, */out strNames, out dicPhoneNumbers);

                if (strReceiverName == "")
                    strReceiverName = strNames;
            }

            return arrReceiverPhoneNumbers;
        }

        // dicPhoneNumbers : Key(전화번호), Value(팀이름 + 멤버이름)
        private static ArrayList GetReceiverPhoneNumbers(ISectionContentsOwner owner, ArrayList arrTeams, bool onlyTeamLeader, /*bool includeChildTeams, */out string strReceiverNames, out Dictionary<string, string> dicPhoneNumbers)
        {
            strReceiverNames = "";

            int nOriginalTeamCount = arrTeams.Count;

            int nTeamCount = arrTeams.Count;

            // 중복을 막기 위하여 Dictionary 사용
            dicPhoneNumbers = new Dictionary<string, string>();

            for (int i = 0; i < nTeamCount; i++)
            {
                SOPTeam teamData = (SOPTeam)arrTeams[i];

                // nOriginalTeamCount보다 같거나 큰 것들은 자식 팀들이다.
                if (i < nOriginalTeamCount)
                {
                    if (strReceiverNames.Length == 0)
                        strReceiverNames = teamData.TeamName;
                    else
                        strReceiverNames += ", " + teamData.TeamName;
                }

                owner.GetSOPTeamPhoneNumbers(teamData, onlyTeamLeader, dicPhoneNumbers);
            }

            ArrayList phoneNumbers = new ArrayList();

            foreach (KeyValuePair<string, string> pair in dicPhoneNumbers)
            {
                if (pair.Value.Length > 0)
                    phoneNumbers.Add(pair.Key);
            }

            Dictionary<string, string> dicPhoneNumbers2 = new Dictionary<string, string>();

            // 산출된 전화번호에서 근무표의 조원과 대조하여 유효한 전화번호만 색출
            owner.CheckControlTeamValidPhoneNumbers(phoneNumbers);
            string strMemberInfo;

            foreach (string strPhoneNumber in phoneNumbers)
            {
                if (dicPhoneNumbers.TryGetValue(strPhoneNumber, out strMemberInfo))
                {
                    dicPhoneNumbers2[strPhoneNumber] = strMemberInfo;
                }
            }

            dicPhoneNumbers = dicPhoneNumbers2;
            return phoneNumbers;
        }
    }
}
