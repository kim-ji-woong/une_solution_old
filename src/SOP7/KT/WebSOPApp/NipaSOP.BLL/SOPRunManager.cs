using System;
using System.Collections;
using System.Collections.Generic;
using SOPManager.BLL.Models.SOP;
using Common.Model.History;
using SOPManager.Model.Sop.Component;
using TeamEditor.Model.Sop.Team;
using SDMS.Model.Spatial;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace NipaSOP.BLL
{
    using IDAL;
    using Model.Sop;
    using Models.Response;

    public class SOPRunManager
    {
        private string[] m_strActionStepNames = new string[] { "관심", "주의", "경계", "심각" };
        private const string Delimeter = "&*&";
        private string m_strSmsUrl = "";

        public SOPRunManager(string strSmsUrl)
        {
            m_strSmsUrl = strSmsUrl;
        }


        public ResponseRunSOP RunSOP(int nBeginCode, ProcessManager processManager)
        {
            string strErrorMessage = null;
            StartInfo startInfo = processManager.DataManager.GetSelectManager().SelectStartInfo(nBeginCode, out strErrorMessage);

            if (startInfo == null)
            {
                if (strErrorMessage == null)
                {
                    strErrorMessage = string.Format("BeginCode {0}에 해당하는 SOP 정보를 찾을수 없습니다.", nBeginCode);
                    return new ResponseRunSOP(false, strErrorMessage);
                }
                else
                    return new ResponseRunSOP(false, strErrorMessage);
            }

            ReadStandardActionStepNames(processManager);
            LocationLinkedSOP sop = processManager.DataManager.GetSelectManager().SelectLocationLinkedSOP(startInfo.FacilityID, out strErrorMessage);

            if (sop == null)
            {
                if (strErrorMessage == null)
                {
                    strErrorMessage = string.Format("FacilityID {0}에 해당하는 SOP 정보를 찾을수 없습니다.", startInfo.FacilityID);
                    return new ResponseRunSOP(false, strErrorMessage);
                }
                else
                    return new ResponseRunSOP(false, strErrorMessage);
            }

            SOPData sopData = GetLinkedSOP(processManager, sop.DisasterCategoryID, sop.SubDisasterCategoryID, sop.DisasterName, out strErrorMessage);

            if (sopData == null)
                return new ResponseRunSOP(false, strErrorMessage);
            else
                RemoveStartInfo(nBeginCode, processManager);

            ActionStepData actionStepData = GetExcuteActionStep(sopData);
            if (actionStepData != null)
            {
                ActionStepHistory history = BeginSOP(processManager, DateTime.Now, actionStepData, sop.FacilityID, out strErrorMessage);

                if (history != null)
                {
                    Facility facility = processManager.DataManager.GetSelectManager().SelectFacility(startInfo.FacilityID, out strErrorMessage);

                    if (facility == null)
                        return new ResponseRunSOP(false, strErrorMessage);

                    ResponseRunSOP response = new ResponseRunSOP(true, "");
                    
                    response.ActionStepHistoryID = history.ID;
                    response.AccessMode = startInfo.AccessMode;
                    response.AccessToken = startInfo.AccessToken;
                    response.ServiceType = startInfo.ServiceType;
                    response.SiteID = facility.SiteID.ToString();

                    SetSopParamsToActionStepHistory(response, sop.FacilityID, history, processManager);
                    return response;
                }
            }

            return new ResponseRunSOP(false, strErrorMessage);
        }

        private void SetSopParamsToActionStepHistory(ResponseRunSOP response, int nFacilityID, ActionStepHistory history, ProcessManager processManager)
        {
            string strErrorMessage;
            history.Description = string.Format("{0}{1}{2}{1}{3}{1}{4}", response.AccessMode, Delimeter, response.AccessToken, response.ServiceType, nFacilityID);
            bool result = processManager.CommonDataManager.GetUpdateManager().UpdateActionStepHistory(history, out strErrorMessage);

            if (result)
            {
                System.Diagnostics.Trace.WriteLine("Update success ActionStepHistory.Description : " + history.Description);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Update fail ActionStepHistory.Description : " + strErrorMessage);
            }
        }

        /// <summary>
        /// 시작
        /// </summary>
        public ActionStepHistory BeginSOP(ProcessManager processManager, DateTime beginTime, ActionStepData actionStepData, int nFacilityID, out string strErrorMessage)
        {
            string strPosition = GetDisasterPosition(processManager, nFacilityID, out strErrorMessage);

            if (strPosition == null)
                return null;

            ActionStepHistory newHistory = ExcuteSOP(processManager, beginTime, actionStepData.ActionStep.ID, strPosition, null);
            if (newHistory != null)
            {
                List<ArrowData> arrowDatas = actionStepData.StepMemberDatas[0].Arrows;
                List<SectionData> sectionDatas = actionStepData.StepMemberDatas[0].Sections;
                SectionData currentSectionData = actionStepData.StepMemberDatas[0].Sections[0]; // SOP가 새로 시작했으므로 첫번째가 현재section이 된다

                CheckAutoSection(processManager, arrowDatas, sectionDatas, currentSectionData.ComponentType, currentSectionData.ID, currentSectionData.Text, "", newHistory);
                return newHistory;
            }

            return null;
        }

        private string GetDisasterPosition(ProcessManager processManager, int nFacilityID, out string strErrorMessage)
        {
            Facility facility = processManager.DataManager.GetSelectManager().SelectFacility(nFacilityID, out strErrorMessage);

            if (facility == null)
                return null;

            return facility.DisplayName;
        }

        private void CheckAutoSection(ProcessManager processManager, List<ArrowData> arrowDatas, List<SectionData> sectionDatas, int componentType, int componentID, string sectionText, string decisionValue, ActionStepHistory history)
        {
            // 시작
            ProgressSOP(processManager, history, componentID, componentType, (int)SOPSimulator.BLL.SOPRunManager.SectionStatus.DONE, null, sectionText);

            while (true)
            {
                SectionData nextSection = GetNextSection(arrowDatas, sectionDatas, componentType, componentID, decisionValue);
                if (nextSection != null)
                {
                    // 실행중
                    ProgressSOP(processManager, history, nextSection.ID, nextSection.ComponentType, (int)SOPSimulator.BLL.SOPRunManager.SectionStatus.RUN, null, nextSection.Text);
                    if (nextSection.AutoRun == null || nextSection.AutoRun == false)
                        break;
                    else
                    {
                        // 완료
                        ProgressSOP(processManager, history, nextSection.ID, nextSection.ComponentType, (int)SOPSimulator.BLL.SOPRunManager.SectionStatus.DONE, null, nextSection.Text);
                        componentType = nextSection.ComponentType;
                        componentID = nextSection.ID;
                        //_decisionValue = ""; // 어떻게 해야 함 ???????
                    }
                }
                else
                {
                    // 종료
                    if (componentType == 3)
                    {
                        CloseSOP(processManager, history);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// SOP를 종료상태로 만든다
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool CloseSOP(ProcessManager processManager, ActionStepHistory actionStepHistory)
        {
            string strErrorMessage = null;

            actionStepHistory.EndTime = DateTime.Now;

            if (!processManager.CommonDataManager.GetUpdateManager().UpdateActionStepHistory(actionStepHistory, out strErrorMessage))
                return false;

            return true;
        }

        private SectionData GetNextSection(List<ArrowData> arrowDatas, List<SectionData> sectionDatas, int componentType, int componentID)
        {
            return GetNextSection(arrowDatas, sectionDatas, componentType, componentID, "");
        }

        private SectionData GetNextSection(List<ArrowData> arrowDatas, List<SectionData> sectionDatas, int componentType, int componentID, string decisionValue)
        {
            int curComponentType = componentType << 24;
            int curComponentID = curComponentType | componentID;

            int arrowCount = arrowDatas.Count;
            for (var i = 0; i < arrowCount; i++)
            {
                ArrowData arrowData = arrowDatas[i];

                int beginComponentID = arrowData.BeginComponentID;
                int endComponentID = arrowData.EndComponentID;

                if (curComponentID == beginComponentID)
                {
                    string text = arrowData.Text;

                    int endSectionType, endSectionID;
                    GetComponentInfo(arrowData.EndComponentID, out endSectionType, out endSectionID);

                    if (endSectionType == 2) // 설명으로 이어진 화살표는 패스
                        continue;

                    // 판단문은 분기에 맞춰 다음 임무로 진행한다
                    if (componentType == 1)
                    {
                        string resultText = text.ToLower();

                        if (decisionValue.ToLower() != resultText)
                            continue;
                    }

                    int dataCount = sectionDatas.Count;
                    for (var j = 0; j < dataCount; j++)
                    {
                        if (sectionDatas[j].ComponentType == endSectionType && sectionDatas[j].ID == endSectionID)
                        {
                            // 다음 임무 추출
                            return sectionDatas[j];
                        }
                    }
                }
            }

            if (componentType != 3)
            {
                int nextSectionNumber = -1;
                int sectionsCount = sectionDatas.Count;
                for (int i = 0; i < sectionsCount; i++)
                {
                    if (sectionDatas[i].ID == componentID && sectionDatas[i].ComponentType == componentType)
                    {
                        if (sectionDatas[i].SectionNumber == null)
                            break;

                        nextSectionNumber = (int)sectionDatas[i].SectionNumber + 1;
                        continue;
                    }

                    if (nextSectionNumber > 0 && sectionDatas[i].SectionNumber != null && (int)sectionDatas[i].SectionNumber == nextSectionNumber)
                        return sectionDatas[i];
                }
            }

            return null;
        }

        private void GetComponentInfo(int arrowComponenID, out int componentType, out int componentID)
        {
            componentType = arrowComponenID >> 24;
            componentID = arrowComponenID & 0xffffff;
        }

        private ComponentHistory ProgressSOP(ProcessManager processManager, ActionStepHistory actionStepHistory, int componentID, int componentType, int status, int? userID, string text, string addDescription = "")
        {
            string strErrorMessage = null;

            string strDescription = GetStringStatus(componentType, status, text);
            if (addDescription.Length > 0)
                strDescription += "_" + addDescription;

            strDescription = strDescription.Replace("'", "''");

            ComponentHistory history = processManager.CommonDataManager.GetCreateManager().CreateComponentHistory(
                actionStepHistory.ID, componentID, componentType, DateTime.Now, status, null, null, null
                , userID, null, null, null, null, strDescription);

            actionStepHistory.LastAccessedTime = DateTime.Now;
            processManager.CommonDataManager.GetUpdateManager().UpdateActionStepHistory(actionStepHistory, out strErrorMessage);

            return history;
        }

        private string GetStringStatus(int componentType, int nStatus, string text)
        {
            string strType = "";
            if (componentType == 0)
                strType = "Process";
            else if (componentType == 1)
                strType = "Decision";
            else if (componentType == 3)
                strType = "EndPoint";
            else if (componentType == 6)
                strType = "Internal";

            //NORMAL = 1, RUN = 2, DONE = 3, INPUT = 4, SKIP = 5
            string strStatus = "";
            if (nStatus == (int)SOPSimulator.BLL.SOPRunManager.SectionStatus.NORMAL)
                strStatus = "대기";
            else if (nStatus == (int)SOPSimulator.BLL.SOPRunManager.SectionStatus.RUN)
                strStatus = "실행중";
            else if (nStatus == (int)SOPSimulator.BLL.SOPRunManager.SectionStatus.DONE)
                strStatus = "완료";

            string strDescription = string.Format("{0}_{1}_{2}", strType, text, strStatus);

            return strDescription;
        }

        /// <summary>
        /// SOP를 실행상태로 만든다
        /// </summary>
        private ActionStepHistory ExcuteSOP(ProcessManager processManager, DateTime? beginTime, int actionStepID, string position, int? userID)
        {
            DateTime dtTime = DateTime.Now;
            if (beginTime != null)
                dtTime = (DateTime)beginTime;

            ActionStepHistory history = processManager.CommonDataManager.GetCreateManager().CreateActionStepHistory(
                actionStepID, dtTime, true, null, dtTime, null, dtTime, position, userID, null, null, null, null);

            return history;
        }

        private void ReadStandardActionStepNames(ProcessManager processManager)
        {
            string strErrorMessage;
            List<Common.Model.Option.Options> options = processManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SOPSimulator, "StandardActionStepNames", out strErrorMessage);

            if (options == null || options.Count == 0)
                return;

            Common.Model.Option.Options option = options[0];

            if (option.PropertyValue == null)
                return;

            string[] stepNames = option.PropertyValue.Split(',');

            if (stepNames.Length < 4)
                return;

            m_strActionStepNames[0] = stepNames[0].Trim();
            m_strActionStepNames[1] = stepNames[1].Trim();
            m_strActionStepNames[2] = stepNames[2].Trim();
            m_strActionStepNames[3] = stepNames[3].Trim();
        }

        /// <summary>
        /// 실행할 SOP 단계를 선택한다
        /// 알람으로 실행되는 SOP의 경우 알람단계에 맞는 단계로 실행한다
        /// 수동으로 시작하는 경우 마지막 단계로 실행한다
        /// </summary>
        /// <param name="alarmDepth"></param>
        /// <returns></returns>
        private ActionStepData GetExcuteActionStep(SOPData sopData)
        {
            List<int> depthIndexs = GetContainsStepIndex(sopData);
            if (depthIndexs.Count == 0)
                return null;

            int executeDepth = -1;

            foreach (int index in depthIndexs)
            {
                if (executeDepth < 0)
                    executeDepth = index;
                else if (executeDepth < index)
                    executeDepth = index;
            }
            
            if (executeDepth > 0)
            {
                ActionStepData data = GetActionStep(sopData, executeDepth - 1);
                return data;
            }

            return null;
        }

        private ActionStepData GetActionStep(SOPData sopData, int index)
        {
            int actionStepsCount = sopData.ActionStepDatas.Count;
            for (int i = 0; i < actionStepsCount; i++)
            {
                ActionStepData actionStepData = sopData.ActionStepDatas[i];
                if (actionStepData.ActionStep == null)
                    continue;

                if (m_strActionStepNames.Length > index && actionStepData.StepName == m_strActionStepNames[index])
                {
                    return actionStepData;
                }
            }

            return null;
        }

        /// <summary>
        /// SopData에 들어있는 단계 정보
        /// </summary>
        /// <param name="sopData"></param>
        /// <returns></returns>
        private List<int> GetContainsStepIndex(SOPData sopData)
        {
            List<int> depthIndexs = new List<int>();

            int actionStepsCount = sopData.ActionStepDatas.Count;
            for (int i = 0; i < actionStepsCount; i++)
            {
                ActionStepData actionStepData = sopData.ActionStepDatas[i];
                if (actionStepData.ActionStep == null)
                    continue;

                for (int j = 0; j < m_strActionStepNames.Length; j++)
                {
                    if (actionStepData.StepName == m_strActionStepNames[j])
                    {
                        depthIndexs.Add(j + 1);
                        break;
                    }
                }
            }

            return depthIndexs;
        }

        private void RemoveStartInfo(int nBeginCode, ProcessManager processManager)
        {
            string strErrorMessage;
            bool result = processManager.DataManager.GetDeleteManager().DeleteStartInfo(nBeginCode, out strErrorMessage);

            if (result == false)
                System.Diagnostics.Trace.WriteLine("RemoveStartInfo Fail : " + strErrorMessage);
        }

        private SOPData GetLinkedSOP(ProcessManager processManager, int disasterCategoryID, int subDisasterCategoryID, string disasterName, out string strErrorMessage)
        {
            strErrorMessage = null;

            Dictionary<SOPManager.Model.Sop.Category.Disaster.Fields, object> dicCondition = new Dictionary<SOPManager.Model.Sop.Category.Disaster.Fields, object>();
            dicCondition.Add(SOPManager.Model.Sop.Category.Disaster.Fields.SubDisasterCategoryID, subDisasterCategoryID);
            dicCondition.Add(SOPManager.Model.Sop.Category.Disaster.Fields.DisasterName, disasterName);

            string strCondition = string.Format("{0}.OwnerID = {1}.ID AND {0}.ID = {2}.VersionID AND {2}.{3}={4} AND {2}.{5}='{6}' AND {3}=(Select {7} From {8} Where {9}={10})",
                SOPManager.Model.Sop.Category.Version.TableName,
                SOPManager.Model.Sop.Account.User.TableName,
                SOPManager.Model.Sop.Category.Disaster.TableName,
                SOPManager.Model.Sop.Category.Disaster.Fields.SubDisasterCategoryID,
                subDisasterCategoryID,
                SOPManager.Model.Sop.Category.Disaster.Fields.DisasterName,
                disasterName,
                SOPManager.Model.Sop.Category.SubDisasterCategory.Fields.ID,
                SOPManager.Model.Sop.Category.SubDisasterCategory.TableName,
                SOPManager.Model.Sop.Category.SubDisasterCategory.Fields.DisasterCategoryID,
                disasterCategoryID);

            ArrayList arrResult = processManager.SOPDataManager.GetSelectManager().JoinDisasterUserVersion(strCondition, out strErrorMessage);
            if (arrResult == null)
                return null;

            SOPManager.Model.Sop.Category.Version selectedVersion = null;

            int resultCount = arrResult.Count;
            for (int i = 0; i < resultCount; i += 3)
            {
                SOPManager.Model.Sop.Category.Version version = arrResult[i + 2] as SOPManager.Model.Sop.Category.Version;
                if (version == null)
                    continue;

                if (selectedVersion == null || selectedVersion.LastAccessTime < version.LastAccessTime)
                {
                    selectedVersion = version;
                }

            }

            if (selectedVersion == null)
                return null;

            SOPManager.BLL.ProcessManager processMgr = new SOPManager.BLL.ProcessManager(processManager.CommonDataManager, processManager.SOPDataManager, processManager.TeamDataManager, null);
            SOPManager.BLL.Models.Response.ResponseOpen response = processMgr.GetLoadManager().OpenDB(selectedVersion.ID);
            return response.SOPData;
        }

        public bool ProgressSpread(string sopKey, int actionStepHistoryID, int componentType, int componentID, int dataIndex, int componentStatus, int? userID, bool isSMS, bool isEmail, bool isBroadcast, bool isSiren, ProcessManager processManager, SOPSimulator.BLL.SOPRunManager sopSimulatorRunManager)
        {
            try
            {
                ActionStepData actionStepData = sopSimulatorRunManager.GetActionStepData(sopKey, actionStepHistoryID);
                if (actionStepData == null)
                    return false;

                if (actionStepData._ActionStepHistory != null)
                    UpdateActionStepHistory(actionStepData._ActionStepHistory, processManager);

                foreach (SectionData section in actionStepData.StepMemberDatas[0].Sections)
                {
                    if (section.ID == componentID && section.ComponentType == componentType)
                    {
                        List<string> messages = new List<string>();
                        if (componentType == 0)
                        {
                            if (dataIndex == -1) // 전체 세부 목록 모두 전파
                            {
                                for (int i = 0; i < section.Missions.Count; i++)
                                {
                                    messages.Add(section.Missions[i].MissionText);
                                }
                            }
                            else
                            {
                                if (section.Missions.Count < dataIndex)
                                    return false;

                                messages.Add(section.Missions[dataIndex].MissionText);
                            }
                        }
                        else
                        {
                            messages.Add(section.Message);
                        }

                        SOPManager.BLL.ProcessManager processMgr = new SOPManager.BLL.ProcessManager(
                            processManager.CommonDataManager, processManager.SOPDataManager, processManager.TeamDataManager, null);

                        for (int i = 0; i < messages.Count; i++)
                        {
                            string message = sopSimulatorRunManager.ReplaceMessage(messages[i], actionStepData._ActionStepHistory.Position, actionStepData._ActionStepHistory.BeginTime.ToString(), processMgr.GetLoadManager());

                            //if (ProgressSpread(section.Receivers, message, isSMS, isEmail, isBroadcast, isSiren))
                            {
                                int dataI = 2; // 0: 체크해제, 1: 체크, 10: 문자메시지전파, 20: 메일전파, 30: 방송전파
                                string addDescription = "";
                                if (isSMS)
                                {
                                    addDescription += "문자메시지전파";
                                    dataI = 10;

                                    if (SendSMS(section.Receivers, message, actionStepData._ActionStepHistory, processManager))
                                    {
                                        ComponentHistory history = ProgressSOP(processManager, actionStepData._ActionStepHistory, componentID, componentType, componentStatus, userID, section.Text, addDescription);
                                        if (history == null)
                                            return false;

                                        int index = dataIndex;
                                        if (index == -1)
                                            index = i;

                                        ComponentHistoryDetail detail = processManager.CommonDataManager.GetCreateManager().CreateComponentHistoryDetail(
                                            history.ID, index, dataI, null, null, DateTime.Now);
                                    }
                                }
                                if (isEmail)
                                {
                                    addDescription += "메일전파";
                                    dataI = 20;

                                    if (SendEmail(message))
                                    {
                                        ComponentHistory history = ProgressSOP(processManager, actionStepData._ActionStepHistory, componentID, componentType, componentStatus, userID, section.Text, addDescription);
                                        if (history == null)
                                            return false;

                                        int index = dataIndex;
                                        if (index == -1)
                                            index = i;

                                        ComponentHistoryDetail detail = processManager.CommonDataManager.GetCreateManager().CreateComponentHistoryDetail(
                                            history.ID, index, dataI, null, null, DateTime.Now);
                                    }
                                }
                                if (isBroadcast)
                                {
                                    addDescription += "방송전파";
                                    dataI = 30;

                                    if (SendBroadcast(message))
                                    {
                                        ComponentHistory history = ProgressSOP(processManager, actionStepData._ActionStepHistory, componentID, componentType, componentStatus, userID, section.Text, addDescription);
                                        if (history == null)
                                            return false;

                                        int index = dataIndex;
                                        if (index == -1)
                                            index = i;

                                        ComponentHistoryDetail detail = processManager.CommonDataManager.GetCreateManager().CreateComponentHistoryDetail(
                                            history.ID, index, dataI, null, null, DateTime.Now);
                                    }
                                }
                            }
                        }
                        break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void UpdateActionStepHistory(ActionStepHistory actionStepHistory, ProcessManager processManager)
        {
            string strErrorMessage;
            ActionStepHistory history = processManager.CommonDataManager.GetSelectManager().SelectActionStepHistory(actionStepHistory.ID, out strErrorMessage);

            if (history != null)
                actionStepHistory.Description = history.Description;
        }

        private bool SendSMS(List<Receiver> receivers, string message, ActionStepHistory actionStepHistory, ProcessManager processManager)
        {
            if (actionStepHistory.Description == null)
                return true;

            string strAccessMode, strAccessToken, strServiceType;
            int nSiteID;

            if (GetSOPParams(processManager, actionStepHistory.Description, out strAccessMode, out strAccessToken, out strServiceType, out nSiteID) == false)
                return true;

            foreach (Receiver receiver in receivers)
            {
                string strTeamName = null;

                if (receiver.TeamType == (int)Receiver.TeamDataType.TemporaryNormalTeam)
                    strTeamName = GetTemporaryTeamName(receiver.TeamID, true, processManager);
                else if (receiver.TeamType == (int)Receiver.TeamDataType.TemporaryEmergencyTeam)
                    strTeamName = GetTemporaryTeamName(receiver.TeamID, false, processManager);
                else if (receiver.TeamType == (int)Receiver.TeamDataType.RegularTeam)
                    strTeamName = GetRegularTeamName(receiver.TeamID, processManager);

                if (strTeamName == null)
                    continue;

                ArrayList arrDatas = new ArrayList();
                arrDatas.Add(strAccessMode);
                arrDatas.Add(strAccessToken);
                arrDatas.Add(strServiceType);
                arrDatas.Add(nSiteID);
                arrDatas.Add(strTeamName);
                arrDatas.Add(message);

                Thread t = new Thread(new ParameterizedThreadStart(SendSMSThread));
                t.Start(arrDatas);
            }

            return true;
        }

        private bool SendEmail(string message)
        {
            return true;
        }

        private bool SendBroadcast(string message)
        {
            return true;
        }

        private void SendSMSThread(object param)
        {
            ArrayList arrDatas = (ArrayList)param;
            string strAccessMode = (string)arrDatas[0];
            string strAccessToken = (string)arrDatas[1];
            string strServiceType = (string)arrDatas[2];
            int nSiteID = (int)arrDatas[3];
            string strTeamName = (string)arrDatas[4];
            string message = (string)arrDatas[5];

            string resResult = string.Empty;

            string strURL = m_strSmsUrl;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL));
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("accessMode", strAccessMode);
            request.Headers.Add("accessToken", strAccessToken);
            request.Headers.Add("serviceType", strServiceType);

            JObject jsonBody = new JObject();
            jsonBody.Add("message", message);
            jsonBody.Add("siteId", nSiteID);
            jsonBody.Add("team", strTeamName);

            string strJson = jsonBody.ToString();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strJson);
            //System.Diagnostics.Trace.WriteLine("length : " + bytes.Length);
            request.ContentLength = bytes.Length + 3;

            try
            {
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
                writer.Write(strJson);
                writer.Close();

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                System.Diagnostics.Trace.WriteLine("SendSMSThread : " + strJson);
                Logger.Instance.Write("SendSMSThread : " + strJson);
                System.Diagnostics.Trace.WriteLine("Success : " + resResult);
                Logger.Instance.Write("Success : " + resResult);
                //json = JObject.Parse(resResult);
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("Fail : " + ex.Message);
                Logger.Instance.Write("Fail : " + ex.Message);
            }
        }

        private bool GetSOPParams(ProcessManager processManager, string strParams, out string strAccessMode, out string strAccessToken, out string strServiceType, out int nSiteID)
        {
            strAccessMode = strAccessToken = strServiceType = null;
            nSiteID = 0;

            string[] delimeters = new string[1] { Delimeter };
            string[] tokens = strParams.Split(delimeters, StringSplitOptions.None);

            if (tokens.Length < 4)
                return false;

            strAccessMode = tokens[0].Trim();
            strAccessToken = tokens[1].Trim();
            strServiceType = tokens[2].Trim();

            int nFacilityID;

            if (int.TryParse(tokens[3].Trim(), out nFacilityID) == false)
                return false;

            string strErrorMessage;
            Zone zone = processManager.SDMSDataManager.GetSelectManager().SelectZone(nFacilityID, out strErrorMessage);

            if (zone == null || zone.BuildingID == null)
                return false;

            nSiteID = (int)zone.BuildingID;

            return true;
        }

        private string GetRegularTeamName(int nTeamID, ProcessManager processManager)
        {
            string strErrorMessage;
            Regular team = processManager.TeamDataManager.GetSelectManager().SelectRegular(nTeamID, out strErrorMessage);

            if (team == null)
            {
                if (strErrorMessage != null)
                    System.Diagnostics.Trace.WriteLine("GetRegularTeamName Error : " + strErrorMessage);
                else
                    System.Diagnostics.Trace.WriteLine("GetRegularTeamName Fail : No Teams");

                return null;
            }

            return team.TeamName;
        }

        private string GetTemporaryTeamName(int nTeamID, bool isNormal, ProcessManager processManager)
        {
            Dictionary<Temporary.Fields, object> dicConditions = new Dictionary<Temporary.Fields, object>();
            dicConditions[Temporary.Fields.IsNormal] = isNormal;
            dicConditions[Temporary.Fields.ID] = nTeamID;

            string strErrorMessage;
            List<Temporary> temporaryTeams = processManager.TeamDataManager.GetSelectManager().SelectTemporaries(dicConditions, out strErrorMessage);

            if (temporaryTeams == null)
            {
                if (strErrorMessage != null)
                {
                    System.Diagnostics.Trace.WriteLine("GetTemporaryNormalTeamName Error : " + strErrorMessage);
                }

                return null;
            }

            if (temporaryTeams.Count == 0)
            {
                System.Diagnostics.Trace.WriteLine("GetTemporaryNormalTeamName Fail : No Teams");
                return null;
            }

            return temporaryTeams[0].TeamName;
        }
    }
}
