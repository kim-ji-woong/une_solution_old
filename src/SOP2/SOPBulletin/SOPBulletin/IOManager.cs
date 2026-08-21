using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace SOPBulletin
{
    // SOP Data를 DB에 저장 및 불러오기 담당
    public class IOManager
    {
        public IOManager()
        {
        }

        private bool LoadProcessMission(WebDBManager dbMgr, int nProcessID, ArrayList arrMissionItems)
        {
            string strSQL = string.Format("Select ID, missionText from ProcessMission where ProcessID = {0}", nProcessID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMissionText = WebDBManager.GetStringField(arrResult[i + 1], "");
                //int nTransmission = dbMgr.GetIntField(arrResult[i + 2].ToString(), -1);

                Sections.MissionItem item = new Sections.MissionItem();

                item.Mission = strMissionText;
                //item.Transmission = nTransmission;

                arrMissionItems.Add(item);
            }

            return true;
        }

        private bool LoadCheckedItems(WebDBManager dbMgr, int nProcessID, ArrayList arrCheckedItems)
        {
            string strSQL = string.Format("Select ID, Category, SubCategory, TaskName, TargetCount, Position from CheckTask where ProcessID = {0}", nProcessID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strCategory = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strSubCategory = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strTaskName = WebDBManager.GetStringField(arrResult[i + 3], "");
                int nTargetCount = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                string strPosition = WebDBManager.GetStringField(arrResult[i + 5], "");

                Sections.CheckedItem item = new Sections.CheckedItem();

                item.Category = strCategory;
                item.SubCategory = strSubCategory;
                item.Item = strTaskName;
                item.ItemCount = nTargetCount;
                item.Location = strPosition;

                arrCheckedItems.Add(item);
            }

            return true;
        }

        private bool LoadProcess(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, Sections.PanelSection panel)
        {
            string strSQL = "select id, x, y, width, height, text, TeamList, ComponentID, ProcessTime, ProcessTimeType, useProcessTime, useMissionMessage";
            strSQL += ", onlyTeamLeader from Process where StepMemberID = " + nStepMemberID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 12; i += 13)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
                float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
                float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strTeamList = WebDBManager.GetStringField(arrResult[i + 6], "");
                string strComponentID = WebDBManager.GetStringField(arrResult[i + 7], "");
                int nProcessTime = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0);
                int nProcessTimeType = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 0);
                bool useProcessTime = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 0) > 0 ? true : false;
                bool useMissionMessage = WebDBManager.GetIntField(arrResult[i + 11].ToString(), 0) > 0 ? true : false;
                bool onlyTeamLeader = WebDBManager.GetIntField(arrResult[i + 12].ToString(), 0) > 0 ? true : false;

                Sections.SectionProcess section = new Sections.SectionProcess(panel, x, y);
                Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)section.Data;
                sectionData.ShowMessageBox = false;
                dicSections[nID] = section;

                section.RectSize = new SizeF(fWidth, fHeight);
                section.TextUP = strText;
                //section.TextDown = GetTeamList(dbMgr, strTeamList, ref sectionData, ref dicNormal, ref dicEmergency, ref dicUserDefined, ref dicExternal, ref dicRegular);

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
                sectionData.ProcessingTime.Time = nProcessTime;

                Sections.ProcessingTime.Type type = Sections.ProcessingTime.Type.UNKNOWN;
                if (!Sections.ProcessingTime.IntToType(nProcessTimeType, ref type))
                    return false;

                sectionData.ProcessingTime.ProcessingType = type;
                sectionData.UseProcessingTime = useProcessTime;
                sectionData.MissionTransfer = useMissionMessage;
                sectionData.TransferTeamLeaderOnly = onlyTeamLeader;

                if (!LoadProcessMission(dbMgr, nID, sectionData.MissionItems))
                    return false;

                if (!LoadCheckedItems(dbMgr, nID, sectionData.CheckedItems))
                    return false;
            }

            return true;
        }

        private bool LoadDecision(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, Sections.PanelSection panel)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID from Decision where StepMemberID = " + nStepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
                float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
                float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");

                Sections.SectionDecision section = new Sections.SectionDecision(panel, x, y);
                Sections.SectionDataDecision sectionData = (Sections.SectionDataDecision)section.Data;
                sectionData.ShowMessageBox = false;
                dicSections[nID] = section;

                section.RectSize = new SizeF(fWidth, fHeight);
                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
            }

            return true;
        }

        private bool LoadEndPoint(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, Sections.PanelSection panel)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, isBegin from EndPoint where StepMemberID = " + nStepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
                float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
                float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");
                bool isBegin = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0) == 0 ? false : true;

                Sections.SectionEndPoint section = new Sections.SectionEndPoint(panel, x, y);
                Sections.SectionDataEndPoint sectionData = (Sections.SectionDataEndPoint)section.Data;
                sectionData.ShowMessageBox = false;
                dicSections[nID] = section;

                section.RectSize = new SizeF(fWidth, fHeight);
                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
                sectionData.IsBegin = isBegin;
            }

            return true;
        }

        private bool LoadTransSOP(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, Sections.PanelSection panel)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, LinkedActionStepID, Description from TransSOP where StepMemberID = " + nStepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
                float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
                float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");
                int nLinkedActionStepID = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 8], "");

                Sections.SectionTransSOP section = new Sections.SectionTransSOP(panel, x, y);
                Sections.SectionDataTransSOP sectionData = (Sections.SectionDataTransSOP)section.Data;
                sectionData.ShowMessageBox = false;
                dicSections[nID] = section;

                section.RectSize = new SizeF(fWidth, fHeight);
                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
                sectionData.LinkedActionStepID = nLinkedActionStepID;
                sectionData.Description = strDescription;
            }

            return true;
        }

        private bool LoadInternal(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, Sections.PanelSection panel)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, usePopupMessage, useMobileApp, useBroadcast from InternalTransmission where StepMemberID = " + nStepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 9; i += 10)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
                float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
                float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");
                bool usePopupMessage = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0) == 0 ? false : true;
                bool useMobileApp = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0) == 0 ? false : true;
                bool useBroadcast = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 0) == 0 ? false : true;

                Sections.SectionInternal section = new Sections.SectionInternal(panel, x, y);
                Sections.SectionDataInternal sectionData = (Sections.SectionDataInternal)section.Data;
                sectionData.ShowMessageBox = false;
                dicSections[nID] = section;
                
                section.RectSize = new SizeF(fWidth, fHeight);
                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
                sectionData.UsePopupMessage = usePopupMessage;
                sectionData.UseMobileApp = useMobileApp;
                sectionData.UseBroadcast = useBroadcast;
            }

            return true;
        }

        private bool LoadExternal(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, Sections.PanelSection panel)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, useSMS, SMSText, SMSExternalTeamIDList, useEFax, FaxExternalTeamIDList from ExternalTransmission where StepMemberID = " + nStepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 11; i += 12)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
                float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
                float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");
                bool useSMS = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0) == 0 ? false : true;
                string strSMSText = WebDBManager.GetStringField(arrResult[i + 8], "");
                string strSMSExternalTeamIDList = WebDBManager.GetStringField(arrResult[i + 9], "");
                bool useEFax = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 0) == 0 ? false : true;
                string strFaxExternalTeamIDList = WebDBManager.GetStringField(arrResult[i + 11], "");

                Sections.SectionExternal section = new Sections.SectionExternal(panel, x, y);
                Sections.SectionDataExternal sectionData = (Sections.SectionDataExternal)section.Data;
                sectionData.ShowMessageBox = false;
                dicSections[nID] = section;

                section.RectSize = new SizeF(fWidth, fHeight);
                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;
                sectionData.UseSMS = useSMS;
                sectionData.UseFax = useEFax;
                sectionData.SMSMessage = strSMSText;
            }

            return true;
        }

        private bool LoadTransmission(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, Sections.PanelSection panel)
        {
            string strSQL = "select id, x, y, width, height, text, ComponentID, useInternalPopupMessage, useInternalMobileApp, useInternalBroadcast, "
                + "useExternalSMS, externalSMSText, SMSExternalTeamIDList, useExternalFax, FaxExternalTeamIDList from Transmission where StepMemberID = " + nStepMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 14; i += 15)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
                float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
                float fWidth = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float fHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                string strText = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strComponentID = WebDBManager.GetStringField(arrResult[i + 6], "");

                bool useInternalPopupMessage = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0) == 0 ? false : true;
                bool useInternalMobileApp = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0) == 0 ? false : true;
                bool useInternalBroadcast = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 0) == 0 ? false : true;

                bool useExternalSMS = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 0) == 0 ? false : true;
                string strExternalSMSText = WebDBManager.GetStringField(arrResult[i + 11], "");
                string strSMSExternalTeamIDList = WebDBManager.GetStringField(arrResult[i + 12], "");
                bool useExternalFax = WebDBManager.GetIntField(arrResult[i + 13].ToString(), 0) == 0 ? false : true;
                string strFaxExternalTeamIDList = WebDBManager.GetStringField(arrResult[i + 14], "");

                Sections.SectionTransmission section = new Sections.SectionTransmission(panel, x, y);
                Sections.SectionDataTransmission sectionData = (Sections.SectionDataTransmission)section.Data;
                sectionData.ShowMessageBox = false;
                dicSections[nID] = section;

                section.RectSize = new SizeF(fWidth, fHeight);
                section.Title = strText;

                sectionData.ID = nID;
                sectionData.Title = strText;
                sectionData.ComponentID = strComponentID;

                sectionData.DataInternal.UsePopupMessage = useInternalPopupMessage;
                sectionData.DataInternal.UseMobileApp = useInternalMobileApp;
                sectionData.DataInternal.UseBroadcast = useInternalBroadcast;

                sectionData.DataExternal.UseSMS = useExternalSMS;
                sectionData.DataExternal.UseFax = useExternalFax;
                sectionData.DataExternal.SMSMessage = strExternalSMSText;
            }

            return true;
        }

        public bool LoadSections(WebDBManager dbMgr, int nStepMemberID, Dictionary<int, Sections.Section> dicSections, Sections.PanelSection panel)
        {
            if (!LoadProcess(dbMgr, nStepMemberID, dicSections, panel))
                return false;
            if (!LoadDecision(dbMgr, nStepMemberID, dicSections, panel))
                return false;
            if (!LoadEndPoint(dbMgr, nStepMemberID, dicSections, panel))
                return false;
            if (!LoadTransSOP(dbMgr, nStepMemberID, dicSections, panel))
                return false;
            if (!LoadInternal(dbMgr, nStepMemberID, dicSections, panel))
                return false;
            if (!LoadExternal(dbMgr, nStepMemberID, dicSections, panel))
                return false;
            if (!LoadTransmission(dbMgr, nStepMemberID, dicSections, panel))
                return false;

            return true;
        }

        public static void GetProcessCheckedNotify(Sections.SectionProcess section, out int nCheckedNotify1, out int nCheckedNotify2)
        {
            nCheckedNotify1 = 0;
            nCheckedNotify2 = 0;

            Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;
            if (data == null)
                return;

            int nMissionCount = (int)data.MissionItems.Count;

            for (int i = 0; i < nMissionCount; i++)
            {
                int nSMSFlag = 0;
                int nBroadcastFlag = 0;// 1 << i;

                nCheckedNotify1 |= nSMSFlag;
                nCheckedNotify2 |= nBroadcastFlag;
            }
        }

        public static void GetInternalCheckedNotify(Sections.SectionInternal section, out int nCheckedNotify1)
        {
            nCheckedNotify1 = 0;

            Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;
            if (data == null)
                return;

            if (data.UsePopupMessage)
                nCheckedNotify1 |= 1;

            if (data.UseMobileApp)
                nCheckedNotify1 |= 2;

            if (data.UseBroadcast)
                nCheckedNotify1 |= 4;
        }

        public static void GetExternalCheckedNotify(Sections.SectionExternal section, out int nCheckedNotify1, out int nCheckedNotify2)
        {
            nCheckedNotify1 = 0;
            nCheckedNotify2 = 0;
            Sections.SectionDataExternal data = (Sections.SectionDataExternal)section.Data;
            if (data == null)
                return;

            int nIdx = 0;
            int nBit = 0;
            if (data.UseSMS)
            {
                foreach (Sections.ExternalTeamData exTeam in data.SMSReceivers)
                {
                    nBit = 1 << nIdx;
                    nCheckedNotify1 |= nBit;
                    nIdx++;
                    if (nIdx == 16)
                        break;
                }
            }
            else
            {
                nCheckedNotify1 = 0;
            }

            nIdx = 0;
            if (data.UseFax)
            {
                foreach (Sections.ExternalTeamData exTeam in data.FaxReceivers)
                {
                    nBit = 1 << nIdx;
                    nCheckedNotify2 |= nBit;
                    nIdx++;
                    if (nIdx == 16)
                        break;
                }
            }
            else
            {
                nCheckedNotify2 = 0;
            }

            if (data.UseSMS)
                nCheckedNotify1 |= (1 << 31);

            if (data.UseFax)
                nCheckedNotify2 |= (1 << 31);
        }

        public static void GetTransmissionCheckedNotify(Sections.SectionTransmission section, out int nCheckedNotify1, out int nCheckedNotify2)
        {
            nCheckedNotify1 = 0;
            nCheckedNotify2 = 0;
            Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)section.Data;
            if (data == null)
                return;

            if (data.DataInternal.UsePopupMessage)
                nCheckedNotify1 |= 1;

            if (data.DataInternal.UseMobileApp)
                nCheckedNotify1 |= 2;

            if (data.DataInternal.UseBroadcast)
                nCheckedNotify1 |= 4;

            int nIdx = 3;
            int nBit = 0;
            if (data.DataExternal.UseSMS)
            {
                foreach (Sections.ExternalTeamData exTeam in data.DataExternal.SMSReceivers)
                {
                    nBit = 1 << nIdx;
                    nCheckedNotify1 |= nBit;
                    nIdx++;
                    if (nIdx == 16)
                        break;
                }
            }
            else
            {
                nCheckedNotify1 = 0;
            }

            nIdx = 0;
            if (data.DataExternal.UseFax)
            {
                foreach (Sections.ExternalTeamData exTeam in data.DataExternal.FaxReceivers)
                {
                    nBit = 1 << nIdx;
                    nCheckedNotify2 |= nBit;
                    nIdx++;
                    if (nIdx == 16)
                        break;
                }
            }
            else
            {
                nCheckedNotify2 = 0;
            }

            if (data.DataExternal.UseSMS)
                nCheckedNotify1 |= (1 << 31);

            if (data.DataExternal.UseFax)
                nCheckedNotify2 |= (1 << 31);
        }
    }
}
